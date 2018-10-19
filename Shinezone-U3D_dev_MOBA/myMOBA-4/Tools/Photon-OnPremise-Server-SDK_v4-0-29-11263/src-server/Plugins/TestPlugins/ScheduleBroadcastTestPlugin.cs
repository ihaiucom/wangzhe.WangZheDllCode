using System;
using System.Collections;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    using System.Collections.Generic;
    using System.Reflection;

    class ScheduleBroadcastTestPlugin : PluginBase
    {
        private object timer;

        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        private Hashtable GetCreateGameRequestGameProperties(ICallInfo info)
        {
            object resultObject;
            info.OperationRequest.Parameters.TryGetValue(248, out resultObject);
            var result = resultObject as Hashtable;
            return result ?? new Hashtable();
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            try
            {
                //before continue we expect the properties are NOT set
                if (this.PluginHost.GameProperties.ContainsKey("EventSize") || this.PluginHost.GameProperties.ContainsKey("Interval"))
                {
                    throw new Exception("Unexpected GameProperties are already set: EventSize or Interval");
                }

                // we check the request contains the expected game properties
                if (!info.Request.GameProperties.ContainsKey("EventSize") || !info.Request.GameProperties.ContainsKey("Interval"))
                {
                    throw new Exception("Unexpected GameProperties are NOT set: EventSize or Interval");
                }

                // we call explicit continue
                info.Continue();
                
                // or implicit by calling the base function
                //base.OnCreateGame(info);

                // after calling continue, we expect PluginHost.GameProperties IS populated
                if (!this.PluginHost.GameProperties.ContainsKey("EventSize") || !this.PluginHost.GameProperties.ContainsKey("Interval"))
                {
                    throw new Exception("Unexpected GameProperties are NOT set: EventSize or Interval");
                }
                var properties = this.PluginHost.GameProperties;
                this.SetTimer(properties);                
            }
            catch (Exception ex)
            {
                var msg = this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name + "():" + ex.Message;

                this.PluginHost.LogError(ex);

                if(info.IsNew)
                    info.Fail(msg);
                else
                    this.PluginHost.BroadcastErrorInfoEvent(msg);
            }
        }

        private void SetTimer(Hashtable properties)
        {
            var code = (byte)(int)properties["EventCode"];
            var size = (int)properties["EventSize"];
            var interval = (int)properties["Interval"];
            this.timer =
                this.PluginHost.CreateTimer(
                    () => this.PluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, code, new Dictionary<byte, object> { { 1, new byte[size] } }, 0),
                    interval,
                    interval);
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            try
            {
                // we check the request contains the expected game properties
                if (!info.Request.Properties.ContainsKey("EventSize") || !info.Request.Properties.ContainsKey("Interval"))
                {
                    info.Fail("Mising GameProperties in Request: EventSize or Interval");
                }

                if (timer != null)
                {
                    this.PluginHost.StopTimer(timer);
                    timer = null;
                }

                var properties = info.Request.Properties;
                this.SetTimer(properties);

                base.OnSetProperties(info);
            }
            catch (Exception ex)
            {
                info.Fail(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name + "():" + ex.Message);
            }

        }
    }
}

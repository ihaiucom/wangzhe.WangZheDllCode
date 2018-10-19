using System;
using System.Collections;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    using System.Collections.Generic;
    using System.Reflection;

    class ScheduleSetPropertiesTestPlugin : PluginBase
    {
        private object timer;

        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            try
            {
                info.Continue();

                var properties = info.Request.GameProperties;
                this.SetTimer(properties);                
            }
            catch (Exception ex)
            {
                info.Fail(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name + "():" + ex.Message);
            }
        }

        private void SetTimer(Hashtable properties)
        {
            this.PluginHost.SetProperties(0, new Hashtable() { { "Index", 1 } }, null, broadcast: false);

            var interval = (int)properties["Interval"];
            this.timer = this.PluginHost.CreateTimer(
                () =>
                    {
                        var x = (int)this.PluginHost.GameProperties["Index"];
                        this.PluginHost.SetProperties(0, new Hashtable() { { "Index", ++x } }, null, broadcast: true);
                    },
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

                if (this.timer != null)
                {
                    this.PluginHost.StopTimer(this.timer);
                    this.timer = null;
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

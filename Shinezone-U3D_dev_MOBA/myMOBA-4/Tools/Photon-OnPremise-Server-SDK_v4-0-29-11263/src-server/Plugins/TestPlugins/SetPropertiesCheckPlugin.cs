using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class SetPropertiesCheckPlugin : PluginBase
    {
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            var res = this.BeforeSetPropertiesPreCheck(info);
            if (!string.IsNullOrEmpty(res))
            {
                info.Fail(res);
                return;
            }

            try
            {
                base.BeforeSetProperties(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }

            res = this.BeforeSetPropertiesPostCheck(info);
            if (!string.IsNullOrEmpty(res))
            {
                this.PluginHost.BroadcastErrorInfoEvent(res, info);
            }
        }

        private string BeforeSetPropertiesPostCheck(IBeforeSetPropertiesCallInfo info)
        {
            var value = (string)info.Request.Properties["ActorProperty"];
            if (value == "BeforeSetPropertiesPostCheckFail")
            {
                return "BeforeSetPropertiesPostCheckFail is set. we fail";
            }
            return string.Empty;
        }

        private string BeforeSetPropertiesPreCheck(IBeforeSetPropertiesCallInfo info)
        {
            var value = (string) info.Request.Properties["ActorProperty"];
            if (value == "BeforeSetPropertiesPreCheckFail")
            {
                return "BeforeSetPropertiesPreCheckFail is set. we fail";
            }
            return string.Empty;
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            var res = this.CheckBeforeOnSetProperties(info);
            if (!string.IsNullOrEmpty(res))
            {
                info.Fail(res);
                return;
            }
            try
            {
                base.OnSetProperties(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }
            res = this.CheckAfterOnSetProperties(info);
            if (!string.IsNullOrEmpty(res))
            {
                this.PluginHost.BroadcastErrorInfoEvent(res, info);
            }
        }

        private string CheckAfterOnSetProperties(ISetPropertiesCallInfo info)
        {
            var value = (string)info.Request.Properties["ActorProperty"];
            if (value == "BeforeSetPropertiesPostCheckFail")
            {
                return "OnSetPropertiesPostCheckFail is set. we fail";
            }
            return string.Empty;
        }

        private string CheckBeforeOnSetProperties(ISetPropertiesCallInfo info)
        {
            var value = (string)info.Request.Properties["ActorProperty"];
            if (value == "OnSetPropertiesPreCheckFail")
            {
                return "OnSetPropertiesPreCheckFail is set. we fail";
            }
            return string.Empty;
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            this.BroadcastEvent(124, new Dictionary<byte, object>());
            base.ReportError(errorCode, exception, state);
        }
    }
}

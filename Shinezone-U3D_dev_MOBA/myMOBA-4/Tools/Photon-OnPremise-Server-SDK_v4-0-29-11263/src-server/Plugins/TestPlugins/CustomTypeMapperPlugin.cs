using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class CustomTypeMapperPlugin : PluginBase
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            base.OnRaiseEvent(info);


            var json = (string)((Hashtable)info.Request.Data)[0];
            var parsedJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            this.PluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, 123, new Dictionary<byte, object> { { 1, parsedJson }, }, CacheOperations.DoNotCache);

        }

        public override bool OnUnknownType(Type type, ref object obj)
        {
            if (type == typeof(Newtonsoft.Json.Linq.JValue) && obj != null)
            {
                var typedObj = (Newtonsoft.Json.Linq.JValue)obj;
                switch (typedObj.Type)
                {
                    case JTokenType.Null:
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case JTokenType.Integer:
                    case JTokenType.Float:
                    case JTokenType.String:
                    case JTokenType.Boolean:
                        obj = typedObj.Value;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}

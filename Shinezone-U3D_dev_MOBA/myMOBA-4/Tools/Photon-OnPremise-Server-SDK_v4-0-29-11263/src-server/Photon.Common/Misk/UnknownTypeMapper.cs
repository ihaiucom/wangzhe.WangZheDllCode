using System;
using Newtonsoft.Json.Linq;
using Photon.SocketServer;

namespace Photon.Common.Misk
{
    public class UnknownTypeMapper : IUnknownTypeMapper
    {
        public bool OnUnknownType(Type type, ref object obj)
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

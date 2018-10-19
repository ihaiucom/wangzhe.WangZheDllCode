using System;
using System.IO;
using Photon.SocketServer;

namespace Photon.Hive.Serialization
{
    public static class Serializer
    {
        public static byte[] Serialize(object obj)
        {
            byte[] data = null; 
            var photonRpc = Protocol.GpBinaryV162;
            using (var stream = new MemoryStream())
            {
                photonRpc.Serialize(stream, obj);
                data = stream.ToArray();
            }
            return data;
        }

        public static string SerializeBase64(object obj)
        {
            return Convert.ToBase64String(Serialize(obj));
        }

        public static object Deserialize(byte[] data)
        {
            var photonRpc = Protocol.GpBinaryV162;
            object obj;
            photonRpc.TryParse(data, 0, data.Length, out obj);
            return obj;
        }

        public static object DeserializeBase64(string data)
        {
            return Deserialize(Convert.FromBase64String(data));
        }
    }
}

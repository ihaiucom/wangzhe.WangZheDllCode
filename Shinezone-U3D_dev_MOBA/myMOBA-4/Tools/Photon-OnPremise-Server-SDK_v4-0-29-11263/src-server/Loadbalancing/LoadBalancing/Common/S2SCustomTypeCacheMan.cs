using System.IO;
using Photon.Hive.Plugin;
using Photon.SocketServer;
using Photon.SocketServer.Rpc.Protocols;

namespace Photon.LoadBalancing.Common
{
    public class S2SCustomTypeCacheMan
    {
        public const byte ExitGamesTypesRegion = 200;
        public static byte ExcludedActorInfo;
        public static byte LastType;

        protected static readonly IRpcProtocol protocol = Protocol.GpBinaryV162;

        static S2SCustomTypeCacheMan()
        {
            LastType = ExitGamesTypesRegion;
            ExcludedActorInfo = ++LastType;
        }

        #region Publics

        public virtual CustomTypeCache GetCustomTypeCache()
        {
            var cache = new CustomTypeCache();

            cache.TryRegisterType(typeof (ExcludedActorInfo), ExcludedActorInfo, SerializeExcludedActorInfo, DeserializeExcludedActorInfo);

            return cache;
        }

        #endregion

        #region Serializers

        #region ExcludedActorInfo

        private static byte[] SerializeExcludedActorInfo(object info)
        {
            using (var stream = new MemoryStream())
            {
                var excludedActorInfo = (ExcludedActorInfo)info;
                protocol.Serialize(stream, excludedActorInfo.UserId);
                protocol.Serialize(stream, excludedActorInfo.Reason);
                return stream.ToArray();
            }
        }

        private static object DeserializeExcludedActorInfo(byte[] info)
        {
            using (var stream = new MemoryStream(info))
            {
                object o;
                if (!protocol.TryParse(stream, out o))
                {
                    return null;
                }
                var result = new ExcludedActorInfo { UserId = (string)o };
                if (!protocol.TryParse(stream, out o))
                {
                    return null;
                }
                result.Reason = (byte)o;
                return result;
            }
        }

        #endregion

        #endregion
    }
}

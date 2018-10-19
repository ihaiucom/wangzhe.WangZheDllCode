using System.Collections;
using System.Collections.Generic;
using Photon.Hive;
using Photon.Hive.Caching;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.LoadBalancing.GameServer;
using SendParameters = Photon.SocketServer.SendParameters;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.OfflineExtra
{
    public class TestGame : Game
    {
        public TestGame(GameApplication application, string gameId, RoomCacheBase roomCache = null, 
            IPluginManager pluginManager = null, string pluginName = "", Dictionary<string, object> environment = null) 
            : base(application, gameId, roomCache, pluginManager, pluginName, environment)
        {
        }

        protected override bool ProcessBeforeJoinGame(JoinGameRequest joinRequest, SendParameters sendParameters, HivePeer peer)
        {
            if (joinRequest.ActorProperties != null && joinRequest.ActorProperties.ContainsKey("ProcessBeforeJoinException"))
            {
                peer = null;
                joinRequest.CacheSlice = 123;
            }
            return base.ProcessBeforeJoinGame(joinRequest, sendParameters, peer);
        }

        protected override bool ProcessBeforeSetProperties(HivePeer peer, SetPropertiesRequest request, Hashtable oldValues, SendParameters sendParameters)
        {
            var value = (string)request.Properties["ActorProperty"];
            if (value == "BeforeSetPropertiesExceptionInContinue")
            {
                peer = null;
                request.TargetActor = null;
                request.ActorNumber = 1;
            }
            return base.ProcessBeforeSetProperties(peer, request, oldValues, sendParameters);
        }

        protected override bool ProcessSetProperties(HivePeer peer, bool result, string errorMsg, SetPropertiesRequest request, SendParameters sendParameters)
        {
            var value = (string)request.Properties["ActorProperty"];
            if (value == "OnSetPropertiesExceptionInContinue")
            {
                request = null;
            }

            return base.ProcessSetProperties(peer, result, errorMsg, request, sendParameters);
        }
    }
}

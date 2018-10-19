using System.Collections.Generic;

namespace Photon.LoadBalancing.GameServer
{
    using Photon.Hive;
    using Photon.Hive.Messages;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    public class MasterServerConnection : MasterServerConnectionBase
    {
        public MasterServerConnection(GameApplication controller, string address, int port, int connectRetryIntervalSeconds)
            : base(controller, address, port, connectRetryIntervalSeconds)
        {
        }

        public void RemoveGameState(string gameId)
        {
            var masterPeer = this.GetPeer();
            if (masterPeer == null || masterPeer.IsRegistered == false)
            {
                return;
            }

            var parameter = new Dictionary<byte, object> { { (byte)ParameterCode.GameId, gameId }, };
            var eventData = new EventData { Code = (byte)ServerEventCode.RemoveGameState, Parameters = parameter };
            masterPeer.SendEvent(eventData, new SendParameters());
        }

        public override void UpdateAllGameStates()
        {
            var masterPeer = this.GetPeer();
            if (masterPeer == null || masterPeer.IsRegistered == false)
            {
                return;
            }

            foreach (var gameId in this.Application.GameCache.GetRoomNames())
            {
                Room room;
                if (this.Application.GameCache.TryGetRoomWithoutReference(gameId, out room))
                {
                    room.EnqueueMessage(new RoomMessage((byte)GameMessageCodes.ReinitializeGameStateOnMaster));
                }
            }
        }

        protected override OutgoingMasterServerPeer CreateServerPeer()
        {
            return new OutgoingMasterServerPeer(this);
        }
    }
}

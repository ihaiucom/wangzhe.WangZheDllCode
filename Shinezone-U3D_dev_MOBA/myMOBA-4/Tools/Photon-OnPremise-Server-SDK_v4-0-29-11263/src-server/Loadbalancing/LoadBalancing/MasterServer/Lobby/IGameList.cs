
using Photon.Common;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System;
    using System.Collections;

    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    public interface IGameList
    {
        int Count { get; }

        int PlayerCount { get; }

        void AddGameState(GameState gameState);

        int CheckJoinTimeOuts(TimeSpan timeOut);

        int CheckJoinTimeOuts(DateTime minDateTime);

        bool ContainsGameId(string gameId);

        IGameListSubscription AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount);
        void RemoveSubscription(PeerBase peer);

        void RemoveGameServer(IncomingGameServerPeer gameServer);

        bool RemoveGameState(string gameId);

        bool TryGetGame(string gameId, out GameState gameState);

        ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message);

        bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer gameServerPeer, out GameState gameState);

        void PublishGameChanges();

        void OnPlayerCountChanged(GameState gameState, int oldPlayerCount);

        void OnGameJoinableChanged(GameState gameState);
    }
}



using Photon.Common;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    using ExitGames.Logging;

    using Photon.Hive.Operations;
    using Photon.LoadBalancing.Events;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    using EventCode = Photon.LoadBalancing.Events.EventCode;

    public abstract class GameListBase : IGameList
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public AppLobby Lobby;

        protected Dictionary<string, GameState> changedGames;

        protected LinkedListDictionary<string, GameState> gameDict;

        protected HashSet<string> removedGames;

        protected readonly HashSet<PeerBase> peers = new HashSet<PeerBase>();

        protected LinkedListNode<GameState> nextJoinRandomStartNode;

        #region Constructors and Destructors

        protected GameListBase(AppLobby lobby)
        {
            this.Lobby = lobby;
            this.gameDict = new LinkedListDictionary<string, GameState>();
            this.changedGames = new Dictionary<string, GameState>();
            this.removedGames = new HashSet<string>();
        }

        #endregion


        #region Properties

        public int ChangedGamesCount
        {
            get
            {
                return this.changedGames.Count + this.removedGames.Count;
            }
        }

        public int Count
        {
            get
            {
                return this.gameDict.Count;
            }
        }

        public int PlayerCount { get; protected set; }

        #endregion

        public virtual void AddGameState(GameState gameState)
        {
            this.gameDict.Add(gameState.Id, gameState);
        }

        public int CheckJoinTimeOuts(int timeOutSeconds)
        {
            return this.CheckJoinTimeOuts(TimeSpan.FromSeconds(timeOutSeconds));
        }

        public int CheckJoinTimeOuts(TimeSpan timeOut)
        {
            DateTime minDate = DateTime.UtcNow.Subtract(timeOut);
            return this.CheckJoinTimeOuts(minDate);
        }

        public int CheckJoinTimeOuts(DateTime minDateTime)
        {
            int oldJoiningCount = 0;
            int joiningPlayerCount = 0;

            var toRemove = new List<GameState>();

            foreach (GameState gameState in this.gameDict)
            {
                if (gameState.JoiningPlayerCount > 0)
                {
                    oldJoiningCount += gameState.JoiningPlayerCount;
                    gameState.CheckJoinTimeOuts(minDateTime);

                    // check if there are still players left for the game
                    if (gameState.PlayerCount == 0)
                    {
                        toRemove.Add(gameState);
                    }

                    joiningPlayerCount += gameState.JoiningPlayerCount;
                }
            }

            // remove all games where no players left
            foreach (GameState gameState in toRemove)
            {
                this.RemoveGameState(gameState.Id);
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Checked join timeouts: before={0}, after={1}", oldJoiningCount, joiningPlayerCount);
            }

            return joiningPlayerCount;
        }

        public bool ContainsGameId(string gameId)
        {
            return this.gameDict.ContainsKey(gameId);
        }

        public virtual Hashtable GetAllGames(int maxCount)
        {
            if (maxCount <= 0)
            {
                maxCount = this.gameDict.Count;
            }

            var hashTable = new Hashtable(maxCount);

            int i = 0;
            foreach (GameState game in this.gameDict)
            {
                if (game.IsVisbleInLobby)
                {
                    Hashtable gameProperties = game.ToHashTable();
                    hashTable.Add(game.Id, gameProperties);
                    i++;
                }

                if (i == maxCount)
                {
                    break;
                }
            }

            return hashTable;
        }

        public virtual void OnPlayerCountChanged(GameState gameState, int oldPlayerCount)
        {
            this.PlayerCount = this.PlayerCount - oldPlayerCount + gameState.PlayerCount;
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("PlayerCount updated:newValue={0}, oldPlayerCount={1}, plyaerCount={2}", 
                    this.PlayerCount, oldPlayerCount, gameState.PlayerCount);
            }
        }

        public virtual void OnGameJoinableChanged(GameState gameState)
        {
        }

        public virtual void PublishGameChanges()
        {
            if (this.ChangedGamesCount > 0)
            {
                Hashtable gameList = this.GetChangedGames();

                var e = new GameListUpdateEvent { Data = gameList };
                var eventData = new EventData((byte)EventCode.GameListUpdate, e);
                ApplicationBase.Instance.BroadCastEvent(eventData, this.peers, new SendParameters());
            }
        }

        public virtual Hashtable GetChangedGames()
        {
            if (this.changedGames.Count == 0 && this.removedGames.Count == 0)
            {
                return null;
            }

            var hashTable = new Hashtable(this.changedGames.Count + this.removedGames.Count);

            foreach (GameState gameInfo in this.changedGames.Values)
            {
                if (gameInfo.IsVisbleInLobby)
                {
                    Hashtable gameProperties = gameInfo.ToHashTable();
                    hashTable.Add(gameInfo.Id, gameProperties);
                }
            }

            foreach (string gameId in this.removedGames)
            {
                hashTable.Add(gameId, new Hashtable { { (byte)GameParameter.Removed, true } });
            }

            this.changedGames.Clear();
            this.removedGames.Clear();

            return hashTable;
        }

        public virtual IGameListSubscription AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount)
        {
            var subscribtion = new Subscribtion(this, maxGameCount);
            this.peers.Add(peer);
            return subscribtion;
        }

        public virtual void RemoveSubscription(PeerBase peer)
        {
            this.peers.Remove(peer);
        }

        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            // find games belonging to the game server instance
            var instanceGames = this.gameDict.Where(gameState => gameState.GameServer == gameServer).ToList();

            // remove game server instance games
            foreach (var gameState in instanceGames)
            {
                this.RemoveGameState(gameState.Id);
            }
        }

        // override in GameChannelList, SqlGameList
        protected virtual bool RemoveGameState(GameState gameState)
        {
            if (log.IsDebugEnabled)
            {
                LogGameState("RemoveGameState:", gameState);
            }

            if (this.nextJoinRandomStartNode != null && this.nextJoinRandomStartNode.Value == gameState)
            {
                this.AdvanceNextJoinRandomStartNode();
            }

            gameState.OnRemoved();

            var gameId = gameState.Id;
            this.gameDict.Remove(gameId);
            this.changedGames.Remove(gameId);
            this.removedGames.Add(gameId);

            this.PlayerCount -= gameState.PlayerCount;

            return true;
        }

        public bool RemoveGameState(string gameId)
        {
            this.Lobby.Application.RemoveGame(gameId);

            GameState gameState;
            if (!this.gameDict.TryGet(gameId, out gameState))
            {
                return false;
            }

            return this.RemoveGameState(gameState);
        }

        public bool TryGetGame(string gameId, out GameState gameState)
        {
            return this.gameDict.TryGet(gameId, out gameState);
        }

        public abstract ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message);

        public virtual bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer incomingGameServerPeer, out GameState gameState)
        {
            if (!GetOrAddUpdatedGameState(updateOperation, out gameState))
            {
                return false;
            }

            bool oldVisible = gameState.IsVisbleInLobby;
            bool changed = gameState.Update(updateOperation);

            if (!changed)
            {
                return false;
            }

            if (log.IsDebugEnabled)
            {
                LogGameState("UpdateGameState: ", gameState);
            }

            this.HandleVisibility(gameState, oldVisible);

            return true;
        }

        protected void HandleVisibility(GameState gameState, bool oldVisible)
        {
            if (gameState.IsVisbleInLobby)
            {
                this.changedGames[gameState.Id] = gameState;

                if (oldVisible == false)
                {
                    this.removedGames.Remove(gameState.Id);
                }
            }
            else
            {
                if (oldVisible)
                {
                    this.changedGames.Remove(gameState.Id);
                    this.removedGames.Add(gameState.Id);
                }
            }
        }

        protected GameState GetGameState(string gameId)
        {
            GameState result;
            this.gameDict.TryGetValue(gameId, out result);
            return result;
        }

        protected bool GetOrAddUpdatedGameState(UpdateGameEvent updateOperation, out GameState gameState)
        {
            // try to get the game state 
            gameState = GetGameState(updateOperation.GameId);
            if (gameState == null)
            {
                if (updateOperation.Reinitialize)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Reinitialize: Add Game State {0}", updateOperation.GameId);
                    }

                    if (!this.Lobby.Application.TryGetGame(updateOperation.GameId, out gameState))
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Could not find game to reinitialize: {0}", updateOperation.GameId);
                        }

                        return false;
                    }

                    this.AddGameState(gameState);
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Game not found: {0}", updateOperation.GameId);
                    }

                    return false;
                }
            }
            return true;
        }

        private void AdvanceNextJoinRandomStartNode()
        {
            if (this.nextJoinRandomStartNode == null)
            {
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "Changed last join random match: oldGameId={0}, newGameId={1}",
                    this.nextJoinRandomStartNode.Value.Id,
                    this.nextJoinRandomStartNode.Next == null ? "{null}" : this.nextJoinRandomStartNode.Value.Id);
            }

            this.nextJoinRandomStartNode = this.nextJoinRandomStartNode.Next;
        }


        protected static void LogGameState(string prefix, GameState gameState)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "{0}id={1}, peers={2}, max={3}, open={4}, visible={5}, peersJoining={6}, inactive={7}, ispersistent={8}", 
                    prefix, 
                    gameState.Id, 
                    gameState.GameServerPlayerCount, 
                    gameState.MaxPlayer, 
                    gameState.IsOpen, 
                    gameState.IsVisible, 
                    gameState.JoiningPlayerCount,
                    gameState.InactivePlayerCount,
                    gameState.IsPersistent
                    );
            }
        }

        private class Subscribtion : IGameListSubscription
        {
            private readonly int maxGameCount;

            private readonly GameListBase gameList;

            public Subscribtion(GameListBase gameList, int maxGameCount)
            {
                this.gameList = gameList;
                this.maxGameCount = maxGameCount;
            }

            public Hashtable GetGameList()
            {
                var gl = this.gameList;
                if (gl == null)
                {
                    // subscription has been disposed (client has diconnect) during the request handling
                    return new Hashtable();
                }

                return gl.GetAllGames(this.maxGameCount);
            }
        }
    }
}
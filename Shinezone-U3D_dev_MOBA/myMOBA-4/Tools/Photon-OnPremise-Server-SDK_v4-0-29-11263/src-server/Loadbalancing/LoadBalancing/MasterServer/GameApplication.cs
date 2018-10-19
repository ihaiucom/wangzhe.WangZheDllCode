using Photon.Common.Plugins;

namespace Photon.LoadBalancing.MasterServer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using ExitGames.Logging;
    using ExitGames.Concurrency.Fibers;

    using Photon.SocketServer;
    using Photon.Common.LoadBalancer;
    using Photon.Common.Authentication;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.MasterServer.Lobby;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.Hive.Common.Lobby;
    using Photon.Hive.Plugin;
    using Photon.Hive.Configuration;

    using ServiceStack.Redis;

    public class GameApplication : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly string ApplicationId;

        public readonly string Version;

        public readonly LoadBalancer<IncomingGameServerPeer> LoadBalancer;

        public readonly PlayerCache PlayerOnlineCache;

        private readonly Dictionary<string, GameState> gameDict = new Dictionary<string, GameState>();

        protected PooledRedisClientManager redisManager;

        public LobbyFactory LobbyFactory { get; protected set; }

        public LobbyStatsPublisher LobbyStatsPublisher { get; protected set; }

        protected readonly PoolFiber fiber;
        private IDisposable expiryCheckDisposable;

        private readonly LinkedList<GameState.ExpiryInfo> expiryList = new LinkedList<GameState.ExpiryInfo>();

        public AuthTokenFactory TokenCreator { get; protected set; }

        public PluginTraits PluginTraits { get; protected set; }

        public GameApplication(string applicationId, string version, LoadBalancer<IncomingGameServerPeer> loadBalancer)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating application: appId={0}", applicationId);
            }

            this.ApplicationId = applicationId;
            this.LoadBalancer = loadBalancer;
            this.Version = version;
            this.PlayerOnlineCache = new PlayerCache();
            this.LobbyFactory = new LobbyFactory(this);
            this.LobbyFactory.Initialize();
            this.LobbyStatsPublisher = new LobbyStatsPublisher(
                this.LobbyFactory,
                MasterServerSettings.Default.LobbyStatsPublishInterval,
                MasterServerSettings.Default.LobbyStatsLimit);

            this.fiber = new PoolFiber();
            this.fiber.Start();

            if (MasterServerSettings.Default.PersistentGameExpiryMinute != 0)
            {
                var checkTime = MasterServerSettings.Default.GameExpiryCheckPeriod * 60000;
                this.expiryCheckDisposable = this.fiber.Schedule(this.CheckExpiredGames, checkTime);
            }

            this.SetupTokenCreator();

            this.UpdatePluginTraits();

            if (!this.ConnectToRedis())
            {
                return;
            }

            this.PopulateGameListFromRedis();
        }

        private void SetupTokenCreator()
        {
            string sharedKey = Settings.Default.AuthTokenKey;
            if (string.IsNullOrEmpty(sharedKey))
            {
                log.WarnFormat("AuthTokenKey not specified in config. Authentication tokens are not supported");
                return;
            }

            int expirationTimeSeconds = Settings.Default.AuthTokenExpiration;
            //if (expirationTimeSeconds <= 0)
            //{
            //    log.ErrorFormat("Authentication token expiration to low: expiration={0} seconds", expirationTimeSeconds);
            //}

            var expiration = TimeSpan.FromSeconds(expirationTimeSeconds);
            this.TokenCreator = new AuthTokenFactory();
            this.TokenCreator.Initialize(sharedKey, expiration);

            log.InfoFormat("TokenCreator intialized with an expiration of {0}", expiration);
        }

        private bool ConnectToRedis()
        {
            var redisDB = MasterServerSettings.Default.RedisDB.Trim();
            if (string.IsNullOrWhiteSpace(redisDB))
            {
                log.InfoFormat("Application {0}/{1} starts without Redis", this.ApplicationId, this.Version);
                return false;
            }

            PooledRedisClientManager manager = null;
            try
            {
                manager = new PooledRedisClientManager(redisDB);
                using (var client = manager.GetDisposableClient<RedisClient>())
                {
                    client.Client.Ping();
                }

                this.redisManager = manager;
            }
            catch (Exception e)
            {
                if (manager != null)
                {
                    manager.Dispose();
                }
                log.WarnFormat("Exception through connection to redis at '{0}'. Will continue without Redis", redisDB);
                log.WarnFormat("Exception: {0}", e);

                return false;
            }
            return true;
        }

        public virtual void OnClientConnected(MasterClientPeer peer)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnClientConnect: peerId={0}, appId={1}", peer.ConnectionId, this.ApplicationId);
            }

           // remove from player cache
            if (this.PlayerOnlineCache != null && string.IsNullOrEmpty(peer.UserId) == false)
            {
                this.PlayerOnlineCache.OnConnectedToMaster(peer.UserId);
            }
        }

        public virtual void OnClientDisconnected(MasterClientPeer peer)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnClientDisconnect: peerId={0}, appId={1}", peer.ConnectionId, this.ApplicationId);
            }

            // remove from player cache
            if (this.PlayerOnlineCache != null && string.IsNullOrEmpty(peer.UserId) == false)
            {
                this.PlayerOnlineCache.OnDisconnectFromMaster(peer.UserId);
            }

            // unsubscribe from lobby statistic events
            this.LobbyStatsPublisher.Unsubscribe(peer);
        }

        public bool GetOrCreateGame(string gameId, AppLobby lobby, byte maxPlayer, IncomingGameServerPeer gameServerPeer, out GameState gameState)
        {
            lock (this.gameDict)
            {
                if (this.gameDict.TryGetValue(gameId, out gameState))
                {
                    return false;
                }

                gameState = new GameState(lobby, gameId, maxPlayer, gameServerPeer);
                this.gameDict.Add(gameId, gameState);
                return true;
            }
        }

        public bool TryCreateGame(string gameId, AppLobby lobby, byte maxPlayer, IncomingGameServerPeer gameServerPeer, out GameState gameState)
        {
            bool result = false;

            lock (this.gameDict)
            {
                if (this.gameDict.TryGetValue(gameId, out gameState) == false)
                {
                    gameState = new GameState(lobby, gameId, maxPlayer, gameServerPeer);
                    this.gameDict.Add(gameId, gameState);
                    result = true;
                }
            }

            if (result)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Created game: gameId={0}, appId={1}", gameId, this.ApplicationId);
                }
            }

            return result;
        }

        public bool TryGetGame(string gameId, out GameState gameState)
        {
            lock (this.gameDict)
            {
                return this.gameDict.TryGetValue(gameId, out gameState);
            }
        }

        public void OnGameUpdateOnGameServer(UpdateGameEvent updateGameEvent, IncomingGameServerPeer gameServerPeer)
        {
            GameState gameState;

            lock (this.gameDict)
            {
                if (!this.gameDict.TryGetValue(updateGameEvent.GameId, out gameState))
                {
                    if (updateGameEvent.Reinitialize)
                    {
                        AppLobby lobby;
                        if (!this.LobbyFactory.GetOrCreateAppLobby(updateGameEvent.LobbyId, (AppLobbyType)updateGameEvent.LobbyType, out lobby))
                        {
                            // getting here should never happen
                            if (log.IsWarnEnabled)
                            {
                                log.WarnFormat("Could not get or create lobby: name={0}, type={1}", updateGameEvent.LobbyId, (AppLobbyType)updateGameEvent.LobbyType);
                            }
                            return;
                        }

                        if (!this.gameDict.TryGetValue(updateGameEvent.GameId, out gameState))
                        {
                            gameState = new GameState(lobby, updateGameEvent.GameId, updateGameEvent.MaxPlayers.GetValueOrDefault(0),
                                gameServerPeer);
                            this.gameDict.Add(updateGameEvent.GameId, gameState);
                        }
                    }
                }
            }

            if (gameState != null)
            {
                if (gameState.GameServer != gameServerPeer)
                {
                    return;
                }

                gameState.Lobby.UpdateGameState(updateGameEvent, gameServerPeer);
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Game to update not found: {0}", updateGameEvent.GameId);
            }
        }

        public void OnGameRemovedOnGameServer(string gameId)
        {
            bool found;
            GameState gameState;

            lock (this.gameDict)
            {
                found = this.gameDict.TryGetValue(gameId, out gameState);
            }

            if (found)
            {
                if (gameState.ShouldBePreservedInList)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Game '{0}' will be preserved in game list for {1} minutes", gameState.Id, 
                            MasterServerSettings.Default.PersistentGameExpiryMinute);
                    }

                    gameState.ResetGameServer();
                    this.AddGameToRedis(gameState);
                    this.AddGameToExpiryList(gameState);
                }
                else
                {
                    if (gameState.IsPersistent)
                    {
                        this.RemoveGameFromRedis(gameState);
                    }
                    gameState.Lobby.RemoveGame(gameId);
                }
            }
            else if (log.IsDebugEnabled)
            {
                log.DebugFormat("Game to remove not found: gameid={0}, appId={1}", gameId, this.ApplicationId);
            }
        }

        public bool RemoveGame(string gameId)
        {
            bool removed;

            lock (this.gameDict)
            {
                removed = this.gameDict.Remove(gameId);
            }

            if (log.IsDebugEnabled)
            {
                if (removed)
                {
                    log.DebugFormat("Removed game: gameId={0}, appId={1}", gameId, this.ApplicationId);
                }
                else
                {
                    log.DebugFormat("Game to remove not found: gameId={0}, appId={1}", gameId, this.ApplicationId);
                }
            }

            return removed;
        }

        public virtual void OnGameServerRemoved(IncomingGameServerPeer gameServer)
        {
            this.LobbyFactory.OnGameServerRemoved(gameServer);
        }

        private static byte[] SerializeNew(object obj)
        {
            byte[] data = null; 
            var photonRpc = Protocol.GpBinaryV162;
            using (var stream = new MemoryStream(4096))
            {
                photonRpc.Serialize(stream, obj);
                data = stream.ToArray();
            }
            return data;
        }

        private static object DeserializeNew(byte[] data)
        {
            var photonRpc = Protocol.GpBinaryV162;
            object obj;
            photonRpc.TryParse(data, 0, data.Length, out obj);
            return obj;
        }

        protected string GetRedisGameId(string gameId)
        {
            return string.Format("{0}/{1}_{2}", this.ApplicationId, 0, gameId);
        }

        protected void AddGameToRedis(GameState gameState)
        {
            if (this.redisManager == null)
            {
                return;
            }

            try
            {
                using (var redisClient = this.redisManager.GetDisposableClient<RedisClient>())
                {
                    // making redis game id
                    var redisGameId = this.GetRedisGameId(gameState.Id);
                    var data = SerializeNew(gameState.GetRedisData());

                    redisClient.Client.Set(redisGameId, data,
                        new TimeSpan(0, 0, MasterServerSettings.Default.PersistentGameExpiryMinute, 0));
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception during saving game '{0}' to redis. Exception:{1}", gameState.Id, e);
            }
        }

        protected void RemoveGameFromRedis(GameState gameState)
        {
            if (this.redisManager == null)
            {
                return;
            }

            try
            {
                using (var redisClient = this.redisManager.GetDisposableClient<RedisClient>())
                {
                    // making redis game id
                    var redisGameId = this.GetRedisGameId(gameState.Id);

                    redisClient.Client.Remove(redisGameId);
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception during removing game '{0}' from redis. Exception:{1}", gameState.Id, e);
            }
        }

        private void PopulateGameListFromRedis()
        {
            if (this.redisManager == null)
            {
                return;
            }
            try
            {
                var redisGameIdPattern = string.Format("{0}/{1}_*", this.ApplicationId, 0);
                using (var redisClient = this.redisManager.GetDisposableClient<RedisClient>())
                {
                    var keys = redisClient.Client.Keys(redisGameIdPattern);
                    foreach (var key in keys)
                    {
                        var keyString = Encoding.UTF8.GetString(key);
                        var gameData = redisClient.Client.Get(keyString);

                        if (gameData == null || gameData.Length == 0)
                        {
                            continue;
                        }

                        var redisData = (Hashtable)DeserializeNew(gameData);

                        var lobbyName = (string)redisData[GameState.LobbyNameId];
                        var lobbyType = (AppLobbyType)(byte)redisData[GameState.LobbyTypeId];
                        AppLobby lobby;
                        if (!this.LobbyFactory.GetOrCreateAppLobby(lobbyName, lobbyType, out lobby))
                        {
                            // getting here should never happen
                            if (log.IsWarnEnabled)
                            {
                                log.WarnFormat("Could not get or create lobby: name={0}, type={1}", lobbyName, lobbyType);
                            }
                            return;
                        }

                        lock (this.gameDict)
                        {
                            var gameState = new GameState(lobby, redisData);
                            this.gameDict.Add((string)redisData[GameState.GameId], gameState);
                            lobby.GameList.AddGameState(gameState);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                log.ErrorFormat("Exception in PopulateGameListFromRedis. {0}", e);
            }
        }

        private void CheckExpiredGames()
        {
            var now = DateTime.UtcNow;
            var timeout = new TimeSpan(0, 0, MasterServerSettings.Default.GameExpiryCheckPeriod, 0);
            lock (this.expiryList)
            {
                var node = this.expiryList.First;
                while (node != null)
                {
                    var state = node.Value.Game;
                    if (state.GameServer != null )
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Game '{0}' excluded from expiry list because has game server", state.Id);
                        }
                        node = this.RemoveFromExpiryList(state, node);
                    }
                    else if (now - node.Value.ExpiryStart > timeout)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Game '{0}' removed from lobby game list by timeout", state.Id);
                        }

                        state.Lobby.RemoveGame(state.Id);
                        node = this.RemoveFromExpiryList(state, node);
                    }
                    else
                    {
                        node = node.Next;
                    }
                }
            }

            if (ApplicationBase.Instance.Running)
            {
                var checkTime = MasterServerSettings.Default.GameExpiryCheckPeriod * 60000;
                this.expiryCheckDisposable = this.fiber.Schedule(this.CheckExpiredGames, checkTime);
            }
        }

        private LinkedListNode<GameState.ExpiryInfo> RemoveFromExpiryList(GameState state, LinkedListNode<GameState.ExpiryInfo> node)
        {
            state.ExpiryListNode = null;
            var remove = node;
            node = node.Next;
            this.expiryList.Remove(remove);
            return node;
        }

        private void AddGameToExpiryList(GameState gameState)
        {
            if (MasterServerSettings.Default.PersistentGameExpiryMinute != 0)
            {
                return;
            }

            lock (this.expiryList)
            {
                if (gameState.ExpiryListNode != null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Expiry time for game '{0}' updated", gameState.Id);
                    }

                    gameState.ExpiryListNode.Value.ExpiryStart = DateTime.UtcNow;
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Game '{0}' added to expiry list", gameState.Id);
                    }
                    gameState.ExpiryListNode = this.expiryList.AddLast(new GameState.ExpiryInfo(gameState, DateTime.UtcNow));
                }
            }
        }

        ~GameApplication()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Disposing game application:{0}/{1}", this.ApplicationId, this.Version);
            }

            if (this.expiryCheckDisposable != null)
            {
                this.expiryCheckDisposable.Dispose();
                this.expiryCheckDisposable = null;
            }

            if (this.fiber != null)
            {
                this.fiber.Enqueue(() => this.fiber.Dispose());
            }
        }

        private void UpdatePluginTraits()
        {
            var settings = PluginSettings.Default;
            if (settings.Enabled & settings.Plugins.Count > 0)
            {
                var pluginSettings = settings.Plugins[0];

                if (log.IsInfoEnabled)
                {
                    log.InfoFormat("Plugin configured: name={0}", pluginSettings.Name);

                    if (pluginSettings.CustomAttributes.Count > 0)
                    {
                        foreach (var att in pluginSettings.CustomAttributes)
                        {
                            log.InfoFormat("\tAttribute: {0}={1}", att.Key, att.Value);
                        }
                    }
                }

                var pluginInfo = new PluginInfo
                {
                    Name = pluginSettings.Name,
                    Version = pluginSettings.Version,
                    AssemblyName = pluginSettings.AssemblyName,
                    Type = pluginSettings.Type,
                    ConfigParams = pluginSettings.CustomAttributes
                };

                this.PluginTraits = PluginTraits.Create(pluginInfo);
                return;
            }

            this.PluginTraits = PluginTraits.Create(new PluginInfo());
        }

    }
}

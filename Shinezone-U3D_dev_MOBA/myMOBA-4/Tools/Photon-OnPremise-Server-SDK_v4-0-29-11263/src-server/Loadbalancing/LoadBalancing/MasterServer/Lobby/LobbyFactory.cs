
namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Collections.Generic;
    using System.Linq;

    using ExitGames.Logging;

    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.Hive.Common.Lobby;

    public class LobbyFactory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<KeyValuePair<string, AppLobbyType>, AppLobby> lobbyDict = new Dictionary<KeyValuePair<string, AppLobbyType>, AppLobby>();

        protected readonly GameApplication application;

        private readonly AppLobbyType defaultLobbyType; 

        private AppLobby defaultLobby;
      
        public LobbyFactory(GameApplication application) 
            : this(application, AppLobbyType.Default)
        {
        }
        
        public LobbyFactory(GameApplication application, AppLobbyType defaultLobbyType)
        {
            this.application = application;
            this.defaultLobbyType = defaultLobbyType; 
        }

        public void Initialize()
        {
            this.defaultLobby = this.CreateAppLobby(string.Empty, defaultLobbyType);

            var defaultLobbyKey = new KeyValuePair<string, AppLobbyType>(string.Empty, defaultLobbyType);
            this.lobbyDict.Add(defaultLobbyKey, this.defaultLobby);
        }
        
        // only returns true
        public bool GetOrCreateAppLobby(string lobbyName, AppLobbyType lobbyType , out AppLobby lobby)
        {
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobby = this.defaultLobby;
                return true;
            }

            var key = new KeyValuePair<string, AppLobbyType>(lobbyName, lobbyType);

            lock (this.lobbyDict)
            {
                if (this.lobbyDict.TryGetValue(key, out lobby))
                {
                    return true;
                }

                lobby = this.CreateAppLobby(lobbyName, lobbyType);
                this.lobbyDict.Add(key, lobby);
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Created lobby: name={0}, type={1}", lobbyName, lobbyType);
            }

            return true;
        }
        
        public void OnGameServerRemoved(IncomingGameServerPeer gameServerPeer)
        {
            this.defaultLobby.RemoveGameServer(gameServerPeer);

            lock (this.lobbyDict)
            {
                foreach (var lobby in this.lobbyDict.Values)
                {
                    lobby.RemoveGameServer(gameServerPeer);
                }
            }
        }

        public AppLobby[] GetLobbies(int maxItems)
        {
            lock (this.lobbyDict)
            {
                if (maxItems <= 0 || maxItems > this.lobbyDict.Count)
                {
                    return this.lobbyDict.Values.ToArray();
                }

                var list = this.lobbyDict.Values.Take(maxItems);
                return list.ToArray();
            }
        }

        public AppLobby[] GetLobbies(string[] lobbyNames, byte[] lobbyTypes)
        {
            if (lobbyNames.Length == 0)
            {
                return new AppLobby[0];
            }

            var  appLobbies = new AppLobby[lobbyNames.Length];

            lock (this.lobbyDict)
            {
                for (int i = 0; i < lobbyNames.Length; i++)
                {
                    var key = new KeyValuePair<string, AppLobbyType>(lobbyNames[i], (AppLobbyType)lobbyTypes[i]);
                    AppLobby lobby;
                    this.lobbyDict.TryGetValue(key, out lobby);
                    appLobbies[i] = lobby;
                }
            }

            return appLobbies;
        }

        protected virtual AppLobby CreateAppLobby(string lobbyName, AppLobbyType lobbyType)
        {
            return new AppLobby(this.application, lobbyName, lobbyType);
        }
    }
}

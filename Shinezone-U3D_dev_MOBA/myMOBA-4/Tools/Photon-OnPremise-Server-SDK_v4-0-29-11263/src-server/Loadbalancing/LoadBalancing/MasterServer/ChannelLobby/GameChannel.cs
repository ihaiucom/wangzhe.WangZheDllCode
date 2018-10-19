
namespace Photon.LoadBalancing.MasterServer.ChannelLobby
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    using ExitGames.Logging;

    using Photon.LoadBalancing.Common;
    using Photon.LoadBalancing.Events;
    using Photon.LoadBalancing.MasterServer.Lobby;
    using Photon.SocketServer;
    using Photon.Hive.Operations;

    public class GameChannel 
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameChannelKey key;

        private readonly LinkedListDictionary<string, GameState> gameDict = new LinkedListDictionary<string, GameState>();

        private readonly Dictionary<int, HashSet<PeerBase>> subscriptions = new Dictionary<int, HashSet<PeerBase>>();

        private readonly HashSet<string> removedGames = new HashSet<string>();

        private readonly HashSet<string> changedGames = new HashSet<string>();

        private readonly string propertyString;

        private readonly GameChannelList gameChannelList;

        public GameChannel(GameChannelList gameChannelList, GameChannelKey gamePropertyFilter)
        {
            this.key = gamePropertyFilter;
            this.gameChannelList = gameChannelList;

            foreach (var gameState in gameChannelList.GameDict.Values)
            {
                if (gameState.IsVisbleInLobby && this.GameProperties.IsSubsetOf(gameState.Properties))
                {
                    this.gameDict.Add(gameState.Id, gameState);
                }
            }

            var sb = new StringBuilder();
            bool seperator = false;
            foreach (DictionaryEntry entry in this.GameProperties)
            {
                if (seperator)
                {
                    sb.Append(" | ");
                }
                else
                {
                    seperator = true;
                }

                sb.AppendFormat("{0}:{1}", entry.Key, entry.Value);
            }

            this.propertyString = sb.ToString();

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Created new game channel: {0}", this.propertyString);
            }
        }

        public Hashtable GameProperties
        {
            get { return this.key.Properties; }
        }

        public Subscription AddSubscription(PeerBase peer, int gameCount)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("New Subscription: pid={0}, gc={1}, props={2}", peer.ConnectionId, gameCount, this.propertyString);
            }

            if (gameCount < 0)
            {
                gameCount = 0;
            }

            var subscription = new Subscription(this, peer, gameCount);
            HashSet<PeerBase> hashSet;
            if (this.subscriptions.TryGetValue(gameCount, out hashSet) == false)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Creating new hashset for game count = {0}", gameCount);
                }

                hashSet = new HashSet<PeerBase>();
                this.subscriptions.Add(gameCount, hashSet);
            }

            hashSet.Add(peer);
            return subscription;
        }

        public void OnGameUpdated(GameState gameState)
        {
            if (this.GameProperties.IsSubsetOf(gameState.Properties))
            {
                if (this.gameDict.ContainsKey(gameState.Id) == false)
                {
                    this.gameDict.Add(gameState.Id, gameState);

                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Added game: gameId={0}, props={1}", gameState.Id, this.propertyString);
                    }
                }

                this.changedGames.Add(gameState.Id);
                this.removedGames.Remove(gameState.Id);
            }
            else
            {
                if (this.gameDict.ContainsKey(gameState.Id))
                {
                    this.removedGames.Add(gameState.Id);
                    this.changedGames.Remove(gameState.Id);

                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Removed game: gameId={0}, props={1}", gameState.Id, this.propertyString);
                    }
                }
            }
        }

        public void OnGameRemoved(GameState gameState)
        {
            if (this.gameDict.ContainsKey(gameState.Id))
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Removed game: gameId={0}, props={1}", gameState.Id, this.propertyString);
                }

                this.removedGames.Add(gameState.Id);
                this.changedGames.Remove(gameState.Id);
            }
        }

        public void PublishGameChanges()
        {
            if (this.removedGames.Count == 0 && this.changedGames.Count == 0)
            {
                return;
            }

            foreach (var entry in this.subscriptions)
            {
                Hashtable games;
                if (entry.Key == 0)
                {
                    games = this.GetChangedGames();
                }
                else
                {
                    games = this.GetChangedGames(entry.Key);
                }

                if (games.Count > 0)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Publishing game changes: props={0}", this.propertyString);
                    }

                    var e = new GameListUpdateEvent { Data = games };
                    var eventData = new EventData((byte)Photon.LoadBalancing.Events.EventCode.GameListUpdate, e);
                    eventData.SendTo(entry.Value, new SendParameters());
                }
            }

            foreach (var gameId in this.removedGames)
            {
                this.gameDict.Remove(gameId);
            }

            this.changedGames.Clear();
            this.removedGames.Clear();
        }

        public Hashtable GetChangedGames()
        {
            var result = new Hashtable(this.removedGames.Count + this.changedGames.Count);

            foreach (var gameId in this.removedGames)
            {
                result.Add(gameId, new Hashtable { { (byte)GameParameter.Removed, true } });
            }

            foreach (var gameid in this.changedGames)
            {
                GameState gameState;
                if (this.gameDict.TryGet(gameid, out gameState))
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                }
            }

            return result;
        }

        public Hashtable GetChangedGames(int maxGameCount)
        {
            var result = new Hashtable();

            int i = 0;
            int removedCount = 0;

            var node = this.gameDict.First;

            while (node != null && i < maxGameCount)
            {
                var gameState = node.Value;
                if (this.removedGames.Contains(gameState.Id))
                {
                    result.Add(gameState.Id, new Hashtable { { (byte)GameParameter.Removed, true } });
                    removedCount++;
                }
                else if (this.changedGames.Contains(gameState.Id))
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                }

                i++;
                node = node.Next;
            }

            i = 0;
            while (node != null & i < removedCount)
            {
                var gameState = node.Value;

                if (this.removedGames.Contains(gameState.Id) == false)
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                    i++;
                }
                
                node = node.Next;
            }

            return result;
        }

        public Hashtable GetGameList(int maxCount)
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
                }

                i++;

                if (i == maxCount)
                {
                    break;
                }
            }

            return hashTable;
        }

        public class Subscription : IGameListSubscription
        {
            public readonly PeerBase Peer;

            public readonly GameChannel GameChannel;

            public readonly int GameCount;

            private bool disposed;

            public Subscription(GameChannel channel, PeerBase peer, int count)
            {
                this.Peer = peer;
                this.GameChannel = channel;
                this.GameCount = count;
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Disposing gamechannel subscription: pid={0}", this.Peer.ConnectionId);
                }

                HashSet<PeerBase> hashSet;
                if (this.GameChannel.subscriptions.TryGetValue(this.GameCount, out hashSet) == false)
                {
                    log.WarnFormat("Failed to find game channel hashset for game count = {0}", this.GameCount);
                    return;
                }

                if (hashSet.Remove(this.Peer) == false)
                {
                    log.WarnFormat("Failed to remove peer from channel: pid = {0}", this.Peer.ConnectionId);
                }

                if (hashSet.Count == 0)
                {
                    this.GameChannel.subscriptions.Remove(this.GameCount);
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Removed hashset for game count = {0}", this.GameCount); 
                    }

                    if (this.GameChannel.subscriptions.Count == 0)
                    {
                        this.GameChannel.gameChannelList.GameChannels.Remove(this.GameChannel.key);
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Removed game channel: {0}", this.GameChannel.propertyString);
                        }
                    }
                }
            }

            public Hashtable GetGameList()
            {
                return this.GameChannel.GetGameList(this.GameCount);
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;
using System.Linq;
using Photon.Hive.Plugin;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;

    using ExitGames.Logging;

    using Photon.Hive.Common.Lobby;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;
    using Photon.Hive.Common;
    using Photon.Hive.Operations;

    #endregion

    public class GameState
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly AppLobby Lobby;

        private DateTime createDateUtc = DateTime.UtcNow;

        private IncomingGameServerPeer gameServer;

        /// <summary>
        ///   Used to track peers which currently are joining the game.
        /// </summary>
        private readonly LinkedList<PeerState> joiningPeers = new LinkedList<PeerState>();

        private readonly List<string> activeUserIdList;
        private readonly List<string> inactiveUserIdList;
        private readonly List<ExcludedActorInfo> excludedActors;
        private readonly List<string> expectedUsersList; 

        private readonly Dictionary<object, object> properties = new Dictionary<object, object>();

        private bool isJoinable;

        #endregion

        #region Constants

        public const byte GameId = 0;
        public const byte InactiveCountId = 1;
        public const byte CreateDateId = 2;
        public const byte UserListId = 3;
        public const byte PropertiesId = 4;
        public const byte IsVisibleId = 5;
        public const byte IsOpenId = 6;
        public const byte MaxPlayerId = 7;
        public const byte LobbyNameId = 8;
        public const byte LobbyTypeId = 9;
        public const byte InactiveUsersId = 10;
        public const byte ExcludedUsersId = 11;
        public const byte ExpectedUsersId = 12;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "GameState" /> class.
        /// </summary>
        /// <param name="lobby">
        /// The lobby to which the game belongs.
        /// </param>
        /// <param name = "id">
        ///   The game id.
        /// </param>
        /// <param name = "maxPlayer">
        ///   The maximum number of player who can join the game.
        /// </param>
        /// <param name = "gameServerPeer">
        ///   The game server peer.
        /// </param>
        public GameState(AppLobby lobby, string id, byte maxPlayer, IncomingGameServerPeer gameServerPeer)
        {
            this.Lobby = lobby;
            this.Id = id;
            this.MaxPlayer = maxPlayer;
            this.IsOpen = true;
            this.IsVisible = true;
            this.HasBeenCreatedOnGameServer = false;
            this.GameServerPlayerCount = 0;
            this.gameServer = gameServerPeer;
            this.IsJoinable = this.CheckIsGameJoinable();
            this.activeUserIdList = new List<string>(maxPlayer > 0 ? maxPlayer : 5);
            this.inactiveUserIdList = new List<string>(maxPlayer > 0 ? maxPlayer : 5);
            this.expectedUsersList = new List<string>(maxPlayer > 0 ? maxPlayer : 5);
            this.excludedActors = new List<ExcludedActorInfo>();
        }

        public GameState(AppLobby lobby, Hashtable data)
        {
            this.Lobby = lobby;
            this.Id = (string)data[GameId];
            this.MaxPlayer = (int)data[MaxPlayerId];
            this.IsOpen = (bool)data[IsOpenId];
            this.IsVisible = (bool)data[IsVisibleId];

            this.InactivePlayerCount = (int)data[InactiveCountId];
            this.createDateUtc = DateTime.FromBinary((long)data[CreateDateId]);
            this.activeUserIdList = new List<string>((string[])data[UserListId]);
            this.inactiveUserIdList = new List<string>((string[])data[InactiveUsersId]);
            this.expectedUsersList = new List<string>((string[])data[ExpectedUsersId]);
            this.excludedActors = new List<ExcludedActorInfo>((ExcludedActorInfo[])data[ExcludedUsersId]);
            this.properties = (Dictionary<object, object>)data[PropertiesId];

            this.HasBeenCreatedOnGameServer = true;
            this.GameServerPlayerCount = 0;
            this.gameServer = null;
            this.IsJoinable = this.CheckIsGameJoinable();
        }

        #endregion

        #region Properties

        public DateTime CreateDateUtc
        {
            get
            {
                return this.createDateUtc;
            }
        }

        /// <summary>
        ///   Gets the address of the game server on which the game is or should be created.
        /// </summary>
        public IncomingGameServerPeer GameServer
        {
            get
            {
                return this.gameServer;
            }
        }

        internal class ExpiryInfo
        {
            public GameState Game { get; private set; }
            public DateTime ExpiryStart { get; set; }

            internal ExpiryInfo(GameState game, DateTime time)
            {
                this.Game = game;
                this.ExpiryStart = time;
            }
        };

        internal LinkedListNode<ExpiryInfo> ExpiryListNode { get; set; } 
        /// <summary>
        ///   Gets the number of players who joined the game on the game server.
        /// </summary>
        public int GameServerPlayerCount { get; private set; }

        /// <summary>
        ///   Gets the game id.
        /// </summary>
        public string Id { get; private set; }

        public int InactivePlayerCount { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the game is created on a game server instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is created on game server; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenCreatedOnGameServer { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the game is open for players to join the game.
        /// </summary>
        /// <value><c>true</c> if the game is open; otherwise, <c>false</c>.</value>
        public bool IsOpen { get; set; }

        public bool IsPersistent { get; set; }

        /// <summary>
        ///   Gets a value indicating whether this instance is visble in the lobby.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visble in lobby; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisbleInLobby
        {
            get
            {
                return this.IsVisible && this.HasBeenCreatedOnGameServer;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the game should be visible to other players.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the game is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; }

        /// <summary>
        ///   Gets the number of players currently joining the game.
        /// </summary>
        public int JoiningPlayerCount
        {
            get
            {
                return this.joiningPeers.Count;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of player for the game.
        /// </summary>
        public int MaxPlayer { get; set; }
        
        /// <summary>
        ///   Gets the number of players joined the game.
        /// </summary>
        public int PlayerCount
        {
            get
            {
                return this.GameServerPlayerCount + this.InactivePlayerCount + this.JoiningPlayerCount + this.YetExpectedUsersCount;
            }
        }

        public int YetExpectedUsersCount
        {
            get { return this.expectedUsersList.Count(userId => !this.ContainsUser(userId)); }
        }
        public Dictionary<object, object> Properties
        {
            get
            {
                return this.properties;
            }
        }

        public bool IsJoinable
        {
            get
            {
                return this.isJoinable;
            }

            private set
            {
                if (value != this.isJoinable)
                {
                    this.isJoinable = value;
                    this.Lobby.GameList.OnGameJoinableChanged(this);
                }
            }
        }

        public bool ShouldBePreservedInList
        {
            get
            {
                return this.IsPersistent && this.InactivePlayerCount > 0 
                    && (this.Lobby.LobbyType == AppLobbyType.AsyncRandomLobby);
            }
        }

        public bool CheckUserIdOnJoin { get; private set; }

        public List<ExcludedActorInfo> ExcludedActors { get { return this.excludedActors; } } 
        #endregion

        #region Public Methods

        public void AddPeer(ILobbyPeer peer)
        {
            if (this.ContainsUser(peer.UserId))
            {
                return;
            }
            var peerState = new PeerState(peer);

            this.AddPeerState(peerState);
        }

        public void CheckJoinTimeOuts(DateTime minDateTime)
        {
            if (this.joiningPeers.Count == 0)
            {
                return;
            }

            var oldPlayerCount = this.PlayerCount;

            var node = this.joiningPeers.First;
            while (node != null)
            {
                var peerState = node.Value;
                var nextNode = node.Next;

                if (peerState.UtcCreated < minDateTime)
                {
                    this.joiningPeers.Remove(node);

                    if (string.IsNullOrEmpty(peerState.UserId) == false)
                    {
                        if (this.Lobby.Application.PlayerOnlineCache != null)
                        {
                            this.Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(peerState.UserId);
                        }
                    }
                }

                node = nextNode;
            }

            if (oldPlayerCount != this.PlayerCount)
            {
                this.Lobby.GameList.OnPlayerCountChanged(this, oldPlayerCount);
            }

            this.IsJoinable = this.CheckIsGameJoinable();
        }

        // savedgames-poc:
        public void ResetGameServer()
        {
            this.gameServer = null;
        }

        public string GetServerAddress(ILobbyPeer peer)
        {
            string address;

            // savedgames-poc:
            if (this.gameServer == null)
            {
                IncomingGameServerPeer newGameServer;
                if (!this.Lobby.Application.LoadBalancer.TryGetServer(out newGameServer))
                {
                    throw new Exception("Failed to get server instance.");
                }
                this.gameServer = newGameServer;
                log.DebugFormat("GetServerAddress: game={0} got new host GS={1}", this.Id, this.gameServer.Key);
            }

            var useHostnames = peer.UseHostnames; // || config settimg ForceHostnames

            var useIPv4 = peer.LocalIPAddress.AddressFamily == AddressFamily.InterNetwork;
            switch (peer.NetworkProtocol)
            {
                case NetworkProtocolType.Udp:
                    address = useHostnames ? this.GameServer.UdpHostname : (useIPv4 ? this.GameServer.UdpAddress : this.GameServer.UdpAddressIPv6);
                    break;
                case NetworkProtocolType.Tcp:
                    address = useHostnames ? this.GameServer.TcpHostname : (useIPv4 ? this.GameServer.TcpAddress : this.GameServer.TcpAddressIPv6);
                    break;
                case NetworkProtocolType.WebSocket:
                    address = useHostnames ? this.GameServer.WebSocketHostname : (useIPv4 ? this.GameServer.WebSocketAddress : this.GameServer.WebSocketAddressIPv6);
                    break;
                case NetworkProtocolType.SecureWebSocket:
                    address = this.GameServer.SecureWebSocketHostname;
                    break;
                case NetworkProtocolType.Http:
                    if (peer.LocalPort == 443) // https:
                    {
                        address = this.gameServer.SecureHttpHostname;
                    }
                    else // http:
                    {
                        address = useIPv4 ? this.gameServer.HttpAddress : this.gameServer.HttpAddressIPv6;                        
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format("No GS address configured for Protocol {0} (Peer Type: {1})", peer.NetworkProtocol, ((PeerBase)peer).NetworkProtocol));
            }
            if (string.IsNullOrEmpty(address))
            {
                throw new NotSupportedException(
                    string.Format("No GS address configured for Protocol {0} (Peer Type: {1}, AddressFamily: {2})", peer.NetworkProtocol, peer.NetworkProtocol, peer.LocalIPAddress.AddressFamily));
            }
            return address;
        }

        public bool MatchGameProperties(Hashtable matchProperties)
        {
            if (matchProperties == null || matchProperties.Count == 0)
            {
                return true;
            }

            foreach (object key in matchProperties.Keys)
            {
                object gameProperty;
                if (!this.properties.TryGetValue(key, out gameProperty))
                {
                    return false;
                }

                if (gameProperty.Equals(matchProperties[key]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public Hashtable ToHashTable()
        {
            var h = new Hashtable();
            foreach (KeyValuePair<object, object> keyValue in this.properties)
            {
                h.Add(keyValue.Key, keyValue.Value);
            }

            h[(byte)GameParameter.PlayerCount] = (byte)this.PlayerCount;
            h[(byte)GameParameter.MaxPlayers] = (byte)this.MaxPlayer;
            h[(byte)GameParameter.IsOpen] = this.IsOpen;
            h.Remove((byte)GameParameter.IsVisible);

            return h;
        }

        public bool TrySetProperties(Hashtable gameProperties, out bool changed, out string debugMessage)
        {
            changed = false;

            byte? maxPlayer;
            bool? isOpen;
            bool? isVisible;
            object[] propertyFilter;
            string[] expectedUsers;
            
            if (!GameParameterReader.TryGetProperties(gameProperties, out maxPlayer, out isOpen, 
                out isVisible, out propertyFilter, out expectedUsers, out debugMessage))
            {
                return false;
            }

            if (maxPlayer.HasValue && maxPlayer.Value != this.MaxPlayer)
            {
                this.MaxPlayer = maxPlayer.Value;
                this.properties[(byte)GameParameter.MaxPlayers] = (byte)this.MaxPlayer;
                changed = true;
            }

            if (isOpen.HasValue && isOpen.Value != this.IsOpen)
            {
                this.IsOpen = isOpen.Value;
                this.properties[(byte)GameParameter.IsOpen] = this.MaxPlayer;
                changed = true;
            }

            if (isVisible.HasValue && isVisible.Value != this.IsVisible)
            {
                this.IsVisible = isVisible.Value;
                changed = true;
            }

            this.properties.Clear();
            foreach (DictionaryEntry entry in gameProperties)
            {
                if (entry.Value != null)
                {
                    this.properties[entry.Key] = entry.Value;
                }
            }

            debugMessage = string.Empty;
            this.IsJoinable = this.CheckIsGameJoinable();
            return true;
        }

        [Conditional("DEBUG")]
        void UpdateSanityChecks(UpdateGameEvent updateOperation)
        {
            if (updateOperation == null) throw new ArgumentNullException("updateOperation");

            //if (updateOperation.InactiveUsers != null)
            //{
            //    Debug.Assert(this.GameServerPlayerCount != updateOperation.ActorCount
            //      && updateOperation.InactiveCount != this.InactivePlayerCount,
            //      "Inactive added but neither ActorCount nor InactiveCount changed");
            //}

            // this may happen if user was not added on GS
            //if (updateOperation.RemovedUsers != null)
            //{
            //    Debug.Assert(this.GameServerPlayerCount != updateOperation.ActorCount || updateOperation.InactiveCount != this.InactivePlayerCount,
            //      "Player removed but neither ActorCount nor InactiveCount changed");
            //}
            //if (updateOperation.NewUsers != null)
            //{
            //    Debug.Assert(this.GameServerPlayerCount != updateOperation.ActorCount, "New added but ActorCount not changed");
            //}
        }

        public bool Update(UpdateGameEvent updateOperation)
        {
            if (updateOperation.Reinitialize)
            {
                this.StateCleanUp();
            }

            this.UpdateSanityChecks(updateOperation);

            var changed = false;

            if (this.HasBeenCreatedOnGameServer == false)
            {
                this.HasBeenCreatedOnGameServer = true;
                changed = true;
            }

            if (updateOperation.CheckUserIdOnJoin != null)
            {
                this.CheckUserIdOnJoin = updateOperation.CheckUserIdOnJoin.Value;
            }
            if (updateOperation.InactiveCount != this.InactivePlayerCount)
            {
                var oldPlayerCount = this.PlayerCount;
                this.InactivePlayerCount = updateOperation.InactiveCount;
                this.Lobby.GameList.OnPlayerCountChanged(this, oldPlayerCount);
                changed = true;
            }

            if (this.GameServerPlayerCount != updateOperation.ActorCount)
            {
                var oldPlayerCount = this.PlayerCount;
                this.GameServerPlayerCount = updateOperation.ActorCount;
                this.Lobby.GameList.OnPlayerCountChanged(this, oldPlayerCount);
                changed = true;
            }

            if (updateOperation.InactiveUsers != null)
            {
                foreach (var userId in updateOperation.InactiveUsers)
                {
                    this.OnPeerLeftGameOnGameServer(userId, deactivate: true);
                }
            }

            if (updateOperation.NewUsers != null)
            {
                foreach (var userId in updateOperation.NewUsers)
                {
                    this.OnPeerJoinedGameOnGameServer(userId);
                }
            }

            if (updateOperation.RemovedUsers != null)
            {
                foreach (var userId in updateOperation.RemovedUsers)
                {
                    this.OnPeerLeftGameOnGameServer(userId);
                }
            }

            if (updateOperation.FailedToAdd != null)
            {
                foreach (var userId in updateOperation.FailedToAdd)
                {
                    this.OnPeerFailedToJoinOnGameServer(userId);
                }
            }

            if (updateOperation.ExcludedUsers != null)
            {
                this.OnUsersExcluded(updateOperation.ExcludedUsers);
                changed = true;
            }

            if (updateOperation.ExpectedUsers != null)
            {
                this.OnExpectedListUpdated(updateOperation.ExpectedUsers);
                changed = true;
            }

            if (updateOperation.MaxPlayers.HasValue && updateOperation.MaxPlayers.Value != this.MaxPlayer)
            {
                this.MaxPlayer = updateOperation.MaxPlayers.Value;
                this.properties[(byte)GameParameter.MaxPlayers] = this.MaxPlayer;
                changed = true;
            }

            if (updateOperation.IsOpen.HasValue && updateOperation.IsOpen.Value != this.IsOpen)
            {
                this.IsOpen = updateOperation.IsOpen.Value;
                this.properties[(byte)GameParameter.IsOpen] = this.MaxPlayer;
                changed = true;
            }

            if (updateOperation.IsVisible.HasValue && updateOperation.IsVisible.Value != this.IsVisible)
            {
                this.IsVisible = updateOperation.IsVisible.Value;
                changed = true;
            }

            if (updateOperation.PropertyFilter != null)
            {
                var lobbyProperties = new HashSet<object>(updateOperation.PropertyFilter);

                var keys = new object[this.properties.Keys.Count];
                this.properties.Keys.CopyTo(keys, 0);

                foreach (var key in keys)
                {
                    if (lobbyProperties.Contains(key) == false)
                    {
                        this.properties.Remove(key);
                        changed = true;
                    }
                }

                // add max players even if it's not in the property filter
                // MaxPlayer is always reported to the client and available 
                // for JoinRandom matchmaking
                this.properties[(byte)GameParameter.MaxPlayers] = (byte)this.MaxPlayer;
            }

            if (updateOperation.GameProperties != null)
            {
                changed |= this.UpdateProperties(updateOperation.GameProperties);
            }

            this.IsJoinable = this.CheckIsGameJoinable();

            if (updateOperation.IsPersistent.HasValue && updateOperation.IsPersistent.Value != this.IsPersistent)
            {
                this.IsPersistent = updateOperation.IsPersistent.Value;
                changed = true;
            }

            return changed;
        }

        private void OnExpectedListUpdated(IEnumerable<string> expectedUsers)
        {
            var oldValue = this.PlayerCount;
            this.expectedUsersList.Clear();
            this.expectedUsersList.AddRange(expectedUsers);
            this.Lobby.GameList.OnPlayerCountChanged(this, oldValue);
        }

        private void OnUsersExcluded(IEnumerable<ExcludedActorInfo> usersToExclude)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Exclude list operations will be executed");
            }
            try
            {
                foreach (var excludedActorInfo in usersToExclude)
                {
                    if (excludedActorInfo.Reason == -1)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("User {0} will be removed from excluded list with flag {1}", excludedActorInfo.UserId, excludedActorInfo.Reason);
                        }
                        // remove user from exclude list
                        var index = this.excludedActors.FindIndex(ea => ea.UserId == excludedActorInfo.UserId);
                        if (index != -1)
                        {
                            this.excludedActors.RemoveAt(index);
                        }
                    }
                    else
                    {
                        this.excludedActors.Add(excludedActorInfo);
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("User {0} will be added to excluded list with flag {1}", excludedActorInfo.UserId, excludedActorInfo.Reason);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        public void OnRemoved()
        {
            this.RemoveActiveUsers();
        }
       
        public override string ToString()
        {
            return
                string.Format(
                    "GameState {0}: Lobby: {9}, PlayerCount: {1}, Created on GS: {2} at {3}, GSPlayerCount: {4}, IsOpen: {5}, IsVisibleInLobby: {6}, IsVisible: {7}, UtcCreated: {8}",
                    this.Id,
                    this.PlayerCount,
                    this.HasBeenCreatedOnGameServer,
                    this.gameServer != null ? this.gameServer.ToString() : string.Empty,
                    this.GameServerPlayerCount,
                    this.IsOpen,
                    this.IsVisbleInLobby,
                    this.IsVisible,
                    this.CreateDateUtc,
                    this.Lobby.LobbyName);
        }

        public Hashtable GetRedisData()
        {
            var h = new Hashtable
                        {
                            { GameId, this.Id },
                            { InactiveCountId, this.InactivePlayerCount},
                            { CreateDateId, this.createDateUtc.ToBinary()},
                            { UserListId, this.activeUserIdList.ToArray()},
                            { InactiveUsersId, this.inactiveUserIdList.ToArray()},
                            { ExcludedUsersId, this.excludedActors.ToArray()},
                            { ExpectedUsersId, this.expectedUsersList.ToArray()},
                            { PropertiesId, this.Properties},
                            { IsVisibleId, this.IsVisible},
                            { IsOpenId, this.IsOpen},
                            { MaxPlayerId, this.MaxPlayer},
                            { LobbyNameId, this.Lobby.LobbyName},
                            { LobbyTypeId, (byte) this.Lobby.LobbyType},
                        };
            return h;
        }

        public bool ContainsUser(string userId)
        {
            return this.inactiveUserIdList.Contains(userId) 
                || this.activeUserIdList.Contains(userId)
                || this.IsUserJoining(userId);
        }

        public bool IsUserInExcludeList(string userId)
        {
            return (-1 != this.ExcludedActors.FindIndex(x => x.UserId == userId));
        }

        public bool IsUserExpected(string userId)
        {
            return (-1 != this.expectedUsersList.IndexOf(userId));
        }

        public bool CheckSlots(string userId, string[] expectedUsers)
        {
            string errMsg;
            return this.CheckSlots(userId, expectedUsers, out errMsg);
        }

        public bool CheckSlots(string userId, string[] expectedUsers, out string errMsg)
        {
            errMsg = string.Empty;
            if (expectedUsers == null || this.MaxPlayer == 0)
            {
                return true;
            }
            var playerCount = this.PlayerCount + 
                expectedUsers.Count(expectedUser => !this.ContainsUser(expectedUser) && !this.IsUserExpected(expectedUser));
            playerCount += this.ContainsUser(userId) || this.IsUserExpected(userId) ? 0 : 1;

            if (this.MaxPlayer < playerCount)
            {
                errMsg = "MaxPlayer value is not big enough to reserve players slots";
                return false;
            }
            return true;
        }

        public void AddSlots(JoinGameRequest request)
        {
            if (request.AddUsers == null)
            {
                return;
            }

            foreach (var userId in request.AddUsers)
            {
                if (!this.IsUserExpected(userId))
                {
                    var oldValue = this.PlayerCount;
                    this.expectedUsersList.Add(userId);
                    if (!this.ContainsUser(userId))
                    {
                        this.Lobby.GameList.OnPlayerCountChanged(this, oldValue);
                    }
                }
            }
        }
        #endregion

        #region Methods

        private void AddPeerState(PeerState peerState)
        {
            var oldValue = this.PlayerCount;
            this.joiningPeers.AddLast(peerState);
            this.Lobby.GameList.OnPlayerCountChanged(this, oldValue);

            this.IsJoinable = this.CheckIsGameJoinable();

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Added peer: gameId={0}, userId={1}, joiningPeers={2}", this.Id, peerState.UserId, this.joiningPeers.Count);
            }

            // update player state in the online players cache
            if (this.Lobby.Application.PlayerOnlineCache != null && string.IsNullOrEmpty(peerState.UserId) == false)
            {
                this.Lobby.Application.PlayerOnlineCache.OnJoinedGamed(peerState.UserId, this);
            }
        }

        private bool IsUserJoining(string userId)
        {
            return this.joiningPeers.Any(joiningPeer => joiningPeer.UserId == userId);
        }

        private void StateCleanUp()
        {
            this.RemoveActiveUsers();
            this.inactiveUserIdList.Clear();
        }

        private void RemoveActiveUsers()
        {
            if (this.Lobby.Application.PlayerOnlineCache != null && this.activeUserIdList.Count > 0)
            {
                foreach (var playerId in this.activeUserIdList)
                {
                    this.Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(playerId);
                }
            }
            this.activeUserIdList.Clear();
        }

        /// <summary>
        ///   Invoked for peers which has joined the game on the game server instance.
        /// </summary>
        /// <param name = "userId">The user id of the peer joined.</param>
        private void OnPeerJoinedGameOnGameServer(string userId)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User joined on game server: gameId={0}, userId={1}", this.Id, userId);
            }

            // remove the peer from the joining list
            var removed = this.RemoveFromJoiningList(userId);
            if (removed == false && log.IsDebugEnabled)
            {
                log.DebugFormat("User not found in joining list: gameId={0}, userId={1}", this.Id, userId);
            }

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var oldValue = this.PlayerCount;

            this.inactiveUserIdList.Remove(userId);
            this.activeUserIdList.Add(userId);

            this.Lobby.GameList.OnPlayerCountChanged(this, oldValue);

            // update player state in the online players cache
            if (this.Lobby.Application.PlayerOnlineCache != null)
            {
                this.Lobby.Application.PlayerOnlineCache.OnJoinedGamed(userId, this);
            }
        }

        /// <summary>
        ///   Invoked for peers which has left the game on the game server instance.
        /// </summary>
        /// <param name = "userId">The user id of the peer left.</param>
        /// <param name="deactivate">whether player was deactivated or removed</param>
        private void OnPeerLeftGameOnGameServer(string userId, bool deactivate = false)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User left on game server: gameId={0}, userId={1}", this.Id, userId);
            }

            this.RemoveFromJoiningList(userId);

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            this.activeUserIdList.Remove(userId);
            if (deactivate)
            {
                if (!this.inactiveUserIdList.Contains(userId))
                {
                    this.inactiveUserIdList.Add(userId);
                }
            }
            else
            {
                this.inactiveUserIdList.Remove(userId);
                // user may be rejected during join process
            }

            // update player state in the online players cache
            if (this.Lobby.Application.PlayerOnlineCache != null)
            {
                this.Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(userId);
            }
        }

        private void OnPeerFailedToJoinOnGameServer(string userId)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User failed to join on game server: gameId={0}, userId={1}", this.Id, userId);
            }
            this.RemoveFromJoiningList(userId);

            if (string.IsNullOrEmpty(userId) == false)
            {
                if (this.Lobby.Application.PlayerOnlineCache != null)
                {
                    this.Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(userId);
                }
            }

            this.IsJoinable = this.CheckIsGameJoinable();
        }

        /// <summary>
        ///   Removes a peer with the specified user id from the list of joining peers.
        /// </summary>
        /// <param name = "userId">The user id of the peer to remove</param>
        /// <returns>True if the peer has been removed; otherwise false.</returns>
        private bool RemoveFromJoiningList(string userId)
        {
            if (userId == null)
            {
                userId = string.Empty;
            }

            var node = this.joiningPeers.First;

            while (node != null)
            {
                if (node.Value.UserId == userId)
                {
                    var oldValue = this.PlayerCount;
                    this.joiningPeers.Remove(node);
                    this.Lobby.GameList.OnPlayerCountChanged(this,  oldValue);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        private bool UpdateProperties(Hashtable props)
        {
            bool changed = false;

            foreach (DictionaryEntry entry in props)
            {
                object oldValue;

                if (this.properties.TryGetValue(entry.Key, out oldValue))
                {
                    if (entry.Value == null)
                    {
                        changed = true;
                        this.properties.Remove(entry.Key);
                    }
                    else
                    {
                        if (oldValue == null || !oldValue.Equals(entry.Value))
                        {
                            changed = true;
                            this.properties[entry.Key] = entry.Value;
                        }
                    }
                }
                else
                {
                    if (entry.Value != null)
                    {
                        changed = true;
                        this.properties[entry.Key] = entry.Value;
                    }
                }
            }

            return changed;
        }

        private bool CheckIsGameJoinable()
        {
            if (!this.IsOpen || !this.IsVisible || !(this.HasBeenCreatedOnGameServer || this.IsPersistent) 
                || (this.MaxPlayer > 0 && (this.PlayerCount) >= this.MaxPlayer))
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
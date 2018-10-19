// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Game type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;
using Photon.Hive.Common;

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using Photon.Hive;
    using Photon.Hive.Plugin;
    using Photon.Hive.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;
    using SendParameters = Photon.SocketServer.SendParameters;

    #endregion

    public class Game : HiveHostGame
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public readonly GameApplication Application;

        private static readonly HttpRequestQueueOptions DefaultHttpRequestQueueOptions = new HttpRequestQueueOptions(
            GameServerSettings.Default.HttpQueueMaxErrors,
            GameServerSettings.Default.HttpQueueMaxTimeouts,
            GameServerSettings.Default.HttpQueueRequestTimeout,
            GameServerSettings.Default.HttpQueueQueueTimeout,
            GameServerSettings.Default.HttpQueueMaxBackoffTime,
            GameServerSettings.Default.HttpQueueReconnectInterval,
            GameServerSettings.Default.HttpQueueMaxQueuedRequests,
            GameServerSettings.Default.HttpQueueMaxConcurrentRequests);

        private bool logRoomRemoval;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="application">The photon application instance the game belongs to.</param>
        /// <param name="gameId">The game id.</param>
        /// <param name="roomCache">The room cache the game belongs to.</param>
        /// <param name="pluginManager">plugin creator</param>
        /// <param name="pluginName">Plugin name which client expects to be loaded</param>
        /// <param name="environment">different environment parameters</param>
        /// <param name="executionFiber">Fiber which will execute all rooms actions</param>
        public Game(
            GameApplication application,
            string gameId,
            Hive.Caching.RoomCacheBase roomCache = null,
            IPluginManager pluginManager = null,
            string pluginName = "",
            Dictionary<string, object> environment = null,
            ExtendedPoolFiber executionFiber = null
            )
            : base(
                gameId,
                roomCache,
                null,
                GameServerSettings.Default.MaxEmptyRoomTTL,
                pluginManager,
                pluginName,
                environment,
                GameServerSettings.Default.LastTouchSecondsDisconnect * 1000,
                GameServerSettings.Default.LastTouchCheckIntervalSeconds * 1000,
                DefaultHttpRequestQueueOptions,
                executionFiber
            )
        {
            this.Application = application;

            if (this.Application.AppStatsPublisher != null)
            {
                this.Application.AppStatsPublisher.IncrementGameCount();
            }

            this.HttpForwardedOperationsLimit = GameServerSettings.Default.HttpForwardLimit;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.RemoveRoomPath = RemoveState.GameDisposeCalled;
            if (disposing)
            {
                this.RemoveGameStateFromMaster();

                if (this.Application.AppStatsPublisher != null)
                {
                    this.Application.AppStatsPublisher.DecrementGameCount();
                }
            }
            if (this.logRoomRemoval)
            {
                Log.WarnFormat("Room disposed. name:{0}", this.Name);
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            if (this.logRoomRemoval)
            {
                Log.WarnFormat("Room closed. name:{0}", this.Name);
            }
        }

        protected override void JoinFailureHandler(byte leaveReason, HivePeer peer, JoinGameRequest request)
        {
            base.JoinFailureHandler(leaveReason, peer, request);
            this.NotifyMasterUserFailedToAdd((GameClientPeer)peer);
        }

        protected override bool ProcessJoin(int actorNr, JoinGameRequest joinRequest, 
            SendParameters sendParameters, ProcessJoinParams prms, HivePeer peer)
        {
            if (!base.ProcessJoin(actorNr, joinRequest, sendParameters, prms, peer))
            {
                return false;
            }

            var gamePeer = (GameClientPeer)peer;
            // update game state at master server            
            var userId = gamePeer.UserId ?? string.Empty;

            this.NotifyMasterUserAdded(userId, joinRequest.AddUsers != null ? this.ActorsManager.ExpectedUsers.ToArray() : null);

            return true;
        }

        protected override bool ProcessCreateGame(HivePeer peer, JoinGameRequest joinRequest, SendParameters sendParameters)
        {
            if (base.ProcessCreateGame(peer, joinRequest, sendParameters))
            {
                // update game state at master server            
                this.UpdateGameStateOnMasterOnCreate(joinRequest, peer);
            }
            return true;
        }

        protected override void OnGamePropertiesChanged(SetPropertiesRequest request)
        {
            Log.DebugFormat("MaxPlayer={0}, IsOpen={1}, IsVisible={2}, #LobbyProperties={3}, #GameProperties={4}",
                request.newMaxPlayer, request.newIsOpen, request.newIsVisible, request.newLobbyProperties == null ? 0 : request.newLobbyProperties.Count(), request.newGameProperties == null ? 0 : request.newGameProperties.Count);

            var expectedList = this.ActorsManager.ExpectedUsers.ToArray();
            this.UpdateGameStateOnMaster(request.newMaxPlayer, request.newIsOpen, 
                request.newIsVisible, request.newLobbyProperties, request.newGameProperties, expectedList: expectedList);
        }

        protected override void DeactivateActor(Actor actor)
        {
            base.DeactivateActor(actor);
            // The room was not disposed because there are either players left or the
            // room has and EmptyRoomLiveTime set -> update game state on master.
            this.NotifyMasterUserDeactivated(actor.UserId);
        }

        protected override void CleanupActor(Actor actor)
        {
            base.CleanupActor(actor);
            this.NotifyMasterUserLeft(actor.UserId);
        }

        protected override void ExecuteOperation(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            try
            {
                //if (Log.IsDebugEnabled)
                //{
                //    Log.DebugFormat("Executing operation {0}", operationRequest.OperationCode);
                //}

                switch (operationRequest.OperationCode)
                {
                    // Lite operation code for join is not allowed in load balanced games.
                    case (byte)Photon.Hive.Operations.OperationCode.Join:
                        this.SendErrorResponse(peer, operationRequest.OperationCode, 
                            ErrorCode.OperationDenied, HiveErrorMessages.InvalidOperationCode, sendParameters);
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Game '{0}' userId '{1}' failed to join. msg:{2}", this.Name, peer.UserId,
                                HiveErrorMessages.InvalidOperationCode);
                        }
                        break;

                    case (byte)OperationCode.DebugGame:
                        var debugGameRequest = new DebugGameRequest(peer.Protocol, operationRequest);
                        if (peer.ValidateOperation(debugGameRequest, sendParameters) == false)
                        {
                            return;
                        }

                        this.LogOperation(peer, operationRequest);

                        debugGameRequest.OnStart();
                        this.HandleDebugGameOperation(peer, debugGameRequest, sendParameters);
                        debugGameRequest.OnComplete();
                        break;

                    // all other operation codes will be handled by the Lite game implementation
                    default:
                        base.ExecuteOperation(peer, operationRequest, sendParameters);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void ProcessMessage(Photon.Hive.Messages.IMessage message)
        {
            try
            {
                switch (message.Action)
                {
                    case (byte)GameMessageCodes.ReinitializeGameStateOnMaster:
                        if (this.ActorsManager.ActorsCount == 0)
                        {
                            Log.WarnFormat("Reinitialize tried to update GameState with ActorCount = 0. " + this);
                        }
                        else
                        {
                            var gameProperties = this.GetLobbyGameProperties(this.Properties.GetProperties());
                            object[] lobbyPropertyFilter = null;
                            if (this.LobbyProperties != null)
                            {
                                lobbyPropertyFilter = new object[this.LobbyProperties.Count];
                                this.LobbyProperties.CopyTo(lobbyPropertyFilter);
                            }

                            var excludedActors = this.ActorsManager.ExcludedActors.Count > 0 ? this.ActorsManager.ExcludedActors.ToArray() : null;
                            var expectedUsers = this.ActorsManager.ExpectedUsers.Count > 0 ? this.ActorsManager.ExpectedUsers.ToArray() : null;
                            this.UpdateGameStateOnMaster(this.MaxPlayers, this.IsOpen, this.IsVisible, lobbyPropertyFilter, gameProperties,
                                this.GetActiveUserIds(), null, true, this.GetInactiveUserIds(), excludedActors, expectedUsers,
                                this.RoomState.CheckUserOnJoin);
                        }

                        break;

                    case (byte)GameMessageCodes.CheckGame:
                        this.CheckGame();
                        break;

                    default:
                        base.ProcessMessage(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private string[] GetActiveUserIds()
        {
            if (this.RoomState.CheckUserOnJoin)
            {
                return this.ActorsManager.Actors.Select(a => a.UserId).ToArray();
            }
            return null;
        }

        private string[] GetInactiveUserIds()
        {
            if (this.RoomState.CheckUserOnJoin)
            {
                return this.ActorsManager.InactiveActors.Select(a => a.UserId).ToArray();
            }
            return null;
        }

        /// <summary>
        /// Check routine to validate that the game is valid (ie., it is removed from the game cache if it has no longer any actors etc.). 
        /// CheckGame() is called by the Application at regular intervals. 
        /// </summary>
        protected virtual void CheckGame()
        {
            if (this.ActorsManager.ActorsCount == 0 && this.RemoveTimer == null)
            {
                // double check if the game is still in cache: 
                Room room;
                if (this.Application.GameCache.TryGetRoomWithoutReference(this.Name, out room))
                {
                    Log.WarnFormat("Game with 0 Actors is still in cache:'{0}'. Actors Dump:'{1}', RemovePath:'{2}'", 
                        this.roomCache.GetDebugString(room.Name), this.ActorsManager.DumpActors(), this.RemoveRoomPathString);
                    this.logRoomRemoval = true;
                }
            }
        }

        protected virtual void NotifyMasterUserDeactivated(string userId)
        {
            var updateGameEvent = this.GetUpdateGameEvent();
            updateGameEvent.InactiveUsers = new[] { userId ?? string.Empty };
            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected virtual void NotifyMasterUserLeft(string userId)
        {
            var updateGameEvent = this.GetUpdateGameEvent();
            updateGameEvent.RemovedUsers = new[] { userId ?? string.Empty };

            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected virtual void NotifyMasterUserFailedToAdd(GameClientPeer peer)
        {
            var updateGameEvent = this.GetUpdateGameEvent();
            updateGameEvent.FailedToAdd = new[] { peer.UserId ?? string.Empty };

            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected virtual void NotifyMasterUserAdded(string userId, string[] slots)
        {
            var usrList = new[] {userId ?? string.Empty};
            this.NotifyMasterUserAdded(usrList, slots);
        }

        protected virtual void NotifyMasterUserAdded(string[] userIds, string[] slots)
        {
            var updateGameEvent = this.GetUpdateGameEvent();
            updateGameEvent.NewUsers = userIds;
            updateGameEvent.ExpectedUsers = slots;
            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected override void OnActorBanned(Actor actor)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("User {0} will be banned", actor.UserId);
            }

            var updateGameEvent = this.GetUpdateGameEvent();
            updateGameEvent.ExcludedUsers = new ExcludedActorInfo[]
            {
                new ExcludedActorInfo
                {
                    UserId = actor.UserId ?? string.Empty, 
                    Reason = RemoveActorReason.Banned,
                }
            };
            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected UpdateGameEvent GetUpdateGameEvent()
        {
            return new UpdateGameEvent
            {
                GameId = this.Name,
                ActorCount = (byte)this.ActorsManager.ActorsCount,
                IsPersistent = this.IsPersistent,
                InactiveCount = (byte)this.ActorsManager.InactiveActorsCount,
            };
        }

        protected virtual void UpdateGameStateOnMasterOnCreate(JoinGameRequest joinRequest, HivePeer peer)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("UpdateGameStateOnMasterOnCreate: game - '{0}'", this.Name);
            }

            var updateEvent = this.GetUpdateGameEvent();
            updateEvent.MaxPlayers = joinRequest.newMaxPlayer;
            updateEvent.IsOpen = joinRequest.newIsOpen;
            updateEvent.IsVisible = joinRequest.newIsVisible;
            updateEvent.PropertyFilter = joinRequest.newLobbyProperties;
            updateEvent.CheckUserIdOnJoin = this.RoomState.CheckUserOnJoin;
            updateEvent.NewUsers = new[] { peer.UserId ?? string.Empty };
            updateEvent.LobbyId = this.LobbyId;
            updateEvent.LobbyType = (byte)this.LobbyType;
            updateEvent.Reinitialize = true;

            var properties = this.GetLobbyGameProperties(joinRequest.GameProperties);
            if (properties != null && properties.Count > 0)
            {
                updateEvent.GameProperties = properties;
            }

            if (this.ActorsManager.InactiveActorsCount > 0)
            {
                updateEvent.InactiveUsers = this.ActorsManager.InactiveActors.Select(a => (a.UserId ?? string.Empty)).ToArray();
            }

            if (this.ActorsManager.ExpectedUsers.Count > 0)
            {
                updateEvent.ExpectedUsers = this.ActorsManager.ExpectedUsers.ToArray();
            }

            this.UpdateGameStateOnMaster(updateEvent);
        }

        protected virtual void UpdateGameStateOnMaster(
            byte? newMaxPlayer = null, 
            bool? newIsOpen = null,
            bool? newIsVisble = null,
            object[] lobbyPropertyFilter = null,
            Hashtable gameProperties = null, 
            string[] newUserId = null, 
            string removedUserId = null, 
            bool reinitialize = false,
            string[] inactiveList = null,
            ExcludedActorInfo[] excludedActorInfos = null,
            string[] expectedList = null,
            bool? checkUserIdOnJoin = null)
        {
            if (reinitialize && this.ActorsManager.ActorsCount == 0 && this.ActorsManager.InactiveActorsCount == 0)
            {
                Log.WarnFormat("Reinitialize tried to update GameState with ActorCount = 0 - {0}." , this.ToString());
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("UpdateGameStateOnMaster: game - '{0}', reinitialize:{1}", this.Name, reinitialize);
            }

            var updateGameEvent = new UpdateGameEvent
                {
                    GameId = this.Name,
                    ActorCount = (byte)this.ActorsManager.ActorsCount,
                    Reinitialize = reinitialize,
                    MaxPlayers = newMaxPlayer,
                    IsOpen = newIsOpen,
                    IsVisible = newIsVisble,
                    IsPersistent = this.IsPersistent,
                    InactiveCount = (byte)this.ActorsManager.InactiveActorsCount,
                    PropertyFilter = lobbyPropertyFilter,
                    CheckUserIdOnJoin = checkUserIdOnJoin,
                };

            // TBD - we have to send this in case we are re-joining and we are creating the room (load)
            if (reinitialize)
            {
                updateGameEvent.LobbyId = this.LobbyId;
                updateGameEvent.LobbyType = (byte)this.LobbyType;
            }

            if (gameProperties != null && gameProperties.Count > 0)
            {
                updateGameEvent.GameProperties = gameProperties;
            }

            if (newUserId != null)
            {
                updateGameEvent.NewUsers = newUserId;
            }

            if (removedUserId != null)
            {
                updateGameEvent.RemovedUsers = new[] { removedUserId };
            }

            if (excludedActorInfos != null)
            {
                updateGameEvent.ExcludedUsers = excludedActorInfos;
            }

            if (expectedList != null)
            {
                updateGameEvent.ExpectedUsers = expectedList;
            }

            this.UpdateGameStateOnMaster(updateGameEvent);
        }

        protected virtual void UpdateGameStateOnMaster(UpdateGameEvent updateEvent)
        {
            var eventData = new EventData((byte)ServerEventCode.UpdateGameState, updateEvent);
            if (this.Application.MasterServerConnection != null)
            {
                this.Application.MasterServerConnection.SendEventIfRegistered(eventData, new SendParameters());
            }
        }

        protected virtual void RemoveGameStateFromMaster()
        {
            var connection = this.Application.MasterServerConnection;
            if (connection != null)
            {
                connection.RemoveGameState(this.Name);
            }
        }
    }
}
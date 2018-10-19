// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LitePeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Inheritance class of <see cref="PeerBase" />.
//   The LitePeer dispatches incoming <see cref="OperationRequest" />s at <see cref="OnOperationRequest">OnOperationRequest</see>.
//   When joining a <see cref="Room" /> a <see cref="Caching.RoomReference" /> is stored in the <see cref="RoomReference" /> property.
//   An <see cref="IFiber" /> guarantees that all outgoing messages (events/operations) are sent one after the other.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.Common;
using Photon.Common.Authentication;
using Photon.Hive.Caching;
using Photon.Hive.Messages;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.Hive.WebRpc;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using PhotonHostRuntimeInterfaces;
using SendParameters = Photon.SocketServer.SendParameters;

namespace Photon.Hive
{
    /// <summary>
    ///   Inheritance class of <see cref = "PeerBase" />.  
    ///   The LitePeer dispatches incoming <see cref = "OperationRequest" />s at <see cref = "OnOperationRequest">OnOperationRequest</see>.
    ///   When joining a <see cref = "Room" /> a <see cref = "Caching.RoomReference" /> is stored in the <see cref = "RoomReference" /> property.
    ///   An <see cref = "IFiber" /> guarantees that all outgoing messages (events/operations) are sent one after the other.
    /// </summary>
    public class HivePeer : ClientPeer
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "HivePeer" /> class.
        /// </summary>
        public HivePeer(InitRequest request)
            : base(request)
        {
            this.UserId = String.Empty;
        }

        #endregion

        public static class JoinStages
        {
            public const byte Connected = 0;
            public const byte CreatingOrLoadingGame = 1;
            public const byte ConvertingParams = 2;
            public const byte CheckingCacheSlice = 3;
            public const byte AddingActor = 4;
            public const byte CheckAfterJoinParams = 5;
            public const byte ApplyActorProperties = 6;
            public const byte BeforeJoinComplete = 7;
            public const byte GettingUserResponse = 8;
            public const byte PublishingEvents = 9;
            public const byte EventsPublished = 10;
            public const byte Complete = 11;
        }

        #region Properties

        /// <summary>
        ///   Gets or sets a <see cref = "Caching.RoomReference" /> when joining a <see cref = "Room" />.
        /// </summary>
        public RoomReference RoomReference { get; set; }

        public string UserId { get; protected set; }

        public WebRpcHandler WebRpcHandler { get; set; }

        public Dictionary<string, object> AuthCookie { get; protected set; }

        public AuthenticationToken AuthToken { get; protected set; }

        internal byte JoinStage { get; set; }

        public int HttpRpcCallsLimit { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Checks if a operation is valid. If the operation is not valid
        ///   an operation response containing a desciptive error message
        ///   will be sent to the peer.
        /// </summary>
        /// <param name = "operation">
        ///   The operation.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <returns>
        ///   true if the operation is valid; otherwise false.
        /// </returns>
        public bool ValidateOperation(Operation operation, SendParameters sendParameters)
        {
            if (operation.IsValid)
            {
                return true;
            }

            var errorMessage = operation.GetErrorMessage();
            this.SendOperationResponse(new OperationResponse
                                            {
                                                OperationCode = operation.OperationRequest.OperationCode, 
                                                ReturnCode = (short)ErrorCode.OperationInvalid, 
                                                DebugMessage = errorMessage
                                            }, 
                                            sendParameters);
            return false;
        }

        /// <summary>
        ///   Checks if the the state of peer is set to a reference of a room.
        ///   If a room refrence is present the peer will be removed from the related room and the reference will be disposed. 
        ///   Disposing the reference allows the associated room factory to remove the room instance if no more references to the room exists.
        /// </summary>
        public void RemovePeerFromCurrentRoom(int reason, string detail)
        {
            // check if the peer already joined another game
            var r = this.RoomReference;
            if (r == null)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("RemovePeerFromCurrentRoom: Room Reference is null for p:{0}", this);
                }
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("RemovePeerFromCurrentRoom: Removing peer from room. p:{0}", this);
            }
            // remove peer from his current game.
            var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, new object[] { this, reason, detail });
            r.Room.EnqueueMessage(message);

            this.ReleaseRoomReference();
        }

        public void ReleaseRoomReference()
        {
            this.RequestFiber.Enqueue(this.ReleaseRoomReferenceInternal);
        }

        public void OnJoinFailed(ErrorCode result, string details)
        {
            this.RequestFiber.Enqueue(() => this.OnJoinFailedInternal(result, details));
        }

        public virtual bool IsThisSameSession(HivePeer peer)
        {
            return false;
        }

        public void ScheduleDisconnect(int time = 1000)
        {
            this.RequestFiber.Schedule(this.Disconnect, time);
        }
        #endregion

        #region Methods

        /// <summary>
        ///   Called by <see cref = "HandleJoinOperation" /> to get a room reference for a join operations.
        ///   This method can be overloaded by inheritors to provide custom room references.
        /// </summary>
        /// <param name = "joinRequest">The join request.</param>
        /// <param name="args">more arguments in order to create room</param>
        /// <returns>An <see cref = "Caching.RoomReference" /> instance.</returns>
        protected virtual RoomReference GetRoomReference(JoinGameRequest joinRequest, params object[] args)
        {
            return HiveGameCache.Instance.GetRoomReference(joinRequest.GameId, this, args);
        }

        protected virtual void HandleCreateGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // The JoinRequest from the Lite application is also used for create game operations to support all feaures 
            // provided by Lite games. 
            // The only difference is the operation code to prevent games created by a join operation. 
            // On "LoadBalancing" game servers games must by created first by the game creator to ensure that no other joining peer 
            // reaches the game server before the game is created.
            var createRequest = new JoinGameRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(createRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom(LeaveReason.SwitchRoom, "eventual switch from other room.");

            var pluginName = createRequest.Plugins != null&&createRequest.Plugins.Length > 0 ? createRequest.Plugins[0] : String.Empty;

            // try to create the game
            RoomReference gameReference;
            if (this.TryCreateRoom(createRequest.GameId, out gameReference, pluginName) == false)
            {
                var response = new OperationResponse
                {
                    OperationCode = (byte)OperationCode.CreateGame,
                    ReturnCode = (short)ErrorCode.GameIdAlreadyExists,
                    DebugMessage = HiveErrorMessages.GameAlreadyExist,
                };

                this.SendOperationResponse(response, sendParameters);
                return;
            }

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        /// <summary>
        ///   Enqueues game related operation requests in the peers current game.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <remarks>
        ///   The current for a peer is stored in the peers state property. 
        ///   Using the <see cref = "Room.EnqueueOperation" /> method ensures that all operation request dispatch logic has thread safe access to all room instance members since they are processed in a serial order. 
        ///   <para>
        ///     Inheritors can use this method to enqueue there custom game operation to the peers current game.
        ///   </para>
        /// </remarks>
        protected virtual void HandleGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (this.JoinStage != JoinStages.Complete)
            {
                this.OnWrongOperationStage(operationRequest, sendParameters);
                return;
            }

            // enqueue operation into game queue. 
            // the operation request will be processed in the games ExecuteOperation method.
            if (this.RoomReference != null)
            {
                this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Received game operation on peer without a game: peerId={0}", this.ConnectionId);
            }
        }

        /// <summary>
        ///   Handles the <see cref = "JoinGameRequest" /> to enter a <see cref = "HiveGame" />.
        ///   This method removes the peer from any previously joined room, finds the room intended for join
        ///   and enqueues the operation for it to handle.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleJoinGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (this.JoinStage != JoinStages.Connected)
            {
                this.OnWrongOperationStage(operationRequest, sendParameters);
                return;
            }

            // create join operation
            var joinRequest = new JoinGameRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom(LeaveReason.SwitchRoom, "eventual switch from other room.");

            // try to get the game reference from the game cache 
            RoomReference gameReference;
            var pluginTraits = this.GetPluginTraits();

            if (joinRequest.JoinMode > 0 || pluginTraits.AllowAsyncJoin)
            {
                var pluginName = joinRequest.Plugins != null && joinRequest.Plugins.Length > 0? joinRequest.Plugins[0] : String.Empty;
                gameReference = this.GetOrCreateRoom(joinRequest.GameId, pluginName);
            }
            else
            {
                if (this.TryGetRoomReference(joinRequest.GameId, out gameReference) == false)
                {
                    this.OnRoomNotFound(joinRequest.GameId);

                    var response = new OperationResponse
                    {
                        OperationCode = (byte)OperationCode.JoinGame,
                        ReturnCode = (short)ErrorCode.GameIdNotExists,
                        DebugMessage = HiveErrorMessages.GameIdDoesNotExist,
                    };

                    this.SendOperationResponse(response, sendParameters);

                    if (log.IsWarnEnabled)
                    {
                        log.WarnFormat("Game '{0}' userId '{1}' failed to join. msg:{2} -- peer:{3}", joinRequest.GameId, this.UserId, HiveErrorMessages.GameIdDoesNotExist, this);
                    }

                    return;
                }
            }

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        private void OnWrongOperationStage(OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.SendOperationResponse(new OperationResponse
            {
                OperationCode = operationRequest.OperationCode,
                ReturnCode = (short)ErrorCode.OperationDenied,
                DebugMessage = HiveErrorMessages.OperationIsNotAllowedOnThisJoinStage,
            }, sendParameters);

        }

        protected virtual PluginTraits GetPluginTraits()
        {
            return HiveGameCache.Instance.PluginManager.PluginTraits;
        }

        /// <summary>
        ///   Handles the <see cref = "JoinGameRequest" /> to enter a <see cref = "HiveGame" />.
        ///   This method removes the peer from any previously joined room, finds the room intended for join
        ///   and enqueues the operation for it to handle.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleJoinOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (this.JoinStage != JoinStages.Connected)
            {
                this.OnWrongOperationStage(operationRequest, sendParameters);
                return;
            }

            // create join operation
            var joinRequest = new JoinGameRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom(LeaveReason.SwitchRoom, "eventual switch from other room.");

            // get a game reference from the game cache 
            // the game will be created by the cache if it does not exists already
            var pluginName = joinRequest.Plugins != null&&joinRequest.Plugins.Length > 0 ? joinRequest.Plugins[0] : String.Empty;
            var gameReference = this.GetRoomReference(joinRequest, pluginName);

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        /// <summary>
        ///   Handles the <see cref = "LeaveRequest" /> to leave a <see cref = "HiveGame" />.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleLeaveOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // check if the peer have a reference to game 
            if (this.RoomReference == null)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Received leave operation on peer without a game: peerId={0}", this.ConnectionId);
                }

                return;
            }

            // enqueue the leave operation into game queue. 
            this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);

            this.ReleaseRoomReferenceInternal();
        }

        /// <summary>
        ///   Handles a ping operation.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandlePingOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode }, sendParameters);
        }

        /// <summary>
        ///   Called when client disconnects.
        ///   Ensures that disconnected players leave the game <see cref = "Room" />.
        ///   The player is not removed immediately but a message is sent to the room. This avoids
        ///   threading issues by making sure the player remove is not done concurrently with operations.
        /// </summary>
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnDisconnect: conId={0}, reason={1}, reasonDetail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }

            this.RemovePeerFromCurrentRoom((int)reasonCode, reasonDetail);
        }

        /// <summary>
        ///   Called when the client sends an <see cref = "OperationRequest" />.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnOperationRequest. Code={0}", operationRequest.OperationCode);
            }

            switch ((OperationCode)operationRequest.OperationCode)
            {
                case OperationCode.Authenticate:
                    //this.HandleAuthenticateOperation(request, sendParameters);
                    return;

                case OperationCode.CreateGame:
                    this.HandleCreateGameOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.JoinGame:
                    this.HandleJoinGameOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.Ping:
                    this.HandlePingOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.DebugGame:
                    //this.HandleDebugGameOperation(request, sendParameters);
                    return;

                case OperationCode.Join:
                    this.HandleJoinOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.Leave:
                    this.HandleLeaveOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.RaiseEvent:
                case OperationCode.GetProperties:
                case OperationCode.SetProperties:
                case OperationCode.ChangeGroups:
                    this.HandleGameOperation(operationRequest, sendParameters);
                    return;
                case OperationCode.Rpc:
                    if (this.WebRpcHandler != null)
                    {
                        this.WebRpcHandler.HandleCall(this, this.UserId, operationRequest, this.AuthCookie, sendParameters);
                        return;
                    }

                    this.SendOperationResponse(new OperationResponse
                    {
                        OperationCode = (byte)OperationCode.Rpc,
                        ReturnCode = (short)ErrorCode.OperationInvalid,
                        DebugMessage = "Rpc is not enabled",
                    },
                    sendParameters);

                    return;
            }

            var message = String.Format("Unknown operation code {0}", operationRequest.OperationCode);
            this.SendOperationResponse(new OperationResponse
            {
                OperationCode = operationRequest.OperationCode,
                ReturnCode = (short)ErrorCode.OperationInvalid, 
                DebugMessage = message
            }, sendParameters);
        }

        protected virtual RoomReference GetOrCreateRoom(string gameId, params object[] args)
        {
            return HiveGameCache.Instance.GetRoomReference(gameId, this, args);
        }

        protected virtual bool TryCreateRoom(string gameId, out RoomReference roomReference, params object[] args)
        {
            return HiveGameCache.Instance.TryCreateRoom(gameId, this, out roomReference, args);
        }

        protected virtual bool TryGetRoomReference(string gameId, out RoomReference roomReference)
        {
            return HiveGameCache.Instance.TryGetRoomReference(gameId, this, out roomReference);
        }

        protected virtual bool TryGetRoomWithoutReference(string gameId, out Room room)
        {
            return HiveGameCache.Instance.TryGetRoomWithoutReference(gameId, out room);
        }

        protected virtual void OnRoomNotFound(string gameId)
        {
        }

        private void OnJoinFailedInternal(ErrorCode result, string details)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnJoinFailedInternal: {0} - {1}", result, details);
            }

            // if join operation failed -> release the reference to the room
            if (result != ErrorCode.Ok && this.RoomReference != null)
            {
                this.ReleaseRoomReferenceInternal();
            }
        }

        private void ReleaseRoomReferenceInternal()
        {
            var r = this.RoomReference;
            if (r == null)
            {
                return;
            }

            // release the reference to the game
            // the game cache will recycle the game instance if no 
            // more refrences to the game are left.
            r.Dispose();

            // finally the peers state is set to null to indicate
            // that the peer is not attached to a room anymore.
            this.RoomReference = null;
        }

        #endregion

    }
}
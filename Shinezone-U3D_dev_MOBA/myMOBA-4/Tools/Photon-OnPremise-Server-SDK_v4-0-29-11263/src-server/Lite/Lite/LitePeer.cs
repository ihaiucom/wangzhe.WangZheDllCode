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

namespace Lite
{
    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Lite.Caching;
    using Lite.Messages;
    using Lite.Operations;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using PhotonHostRuntimeInterfaces;

    /// <summary>
    ///   Inheritance class of <see cref = "PeerBase" />.  
    ///   The LitePeer dispatches incoming <see cref = "OperationRequest" />s at <see cref = "OnOperationRequest">OnOperationRequest</see>.
    ///   When joining a <see cref = "Room" /> a <see cref = "Caching.RoomReference" /> is stored in the <see cref = "RoomReference" /> property.
    ///   An <see cref = "IFiber" /> guarantees that all outgoing messages (events/operations) are sent one after the other.
    /// </summary>
    public class LitePeer : ClientPeer
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LitePeer" /> class.
        /// </summary>
        public LitePeer(InitRequest initRequest)
            : base(initRequest)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a <see cref = "Caching.RoomReference" /> when joining a <see cref = "Room" />.
        /// </summary>
        public RoomReference RoomReference { get; set; }

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

            string errorMessage = operation.GetErrorMessage();
            this.SendOperationResponse(new OperationResponse { OperationCode = operation.OperationRequest.OperationCode, ReturnCode = -1, DebugMessage = errorMessage }, sendParameters);
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Called by <see cref = "HandleJoinOperation" /> to get a room reference for a join operations.
        ///   This method can be overloaded by inheritors to provide custom room references.
        /// </summary>
        /// <param name = "joinRequest">The join request.</param>
        /// <returns>An <see cref = "Caching.RoomReference" /> instance.</returns>
        protected virtual RoomReference GetRoomReference(JoinRequest joinRequest)
        {
            return LiteGameCache.Instance.GetRoomReference(joinRequest.GameId, this);
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
        ///   Handles the <see cref = "JoinRequest" /> to enter a <see cref = "LiteGame" />.
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
            // create join operation
            var joinRequest = new JoinRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom();

            // get a game reference from the game cache 
            // the game will be created by the cache if it does not exists already
            RoomReference gameReference = this.GetRoomReference(joinRequest);

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        /// <summary>
        ///   Handles the <see cref = "LeaveRequest" /> to leave a <see cref = "LiteGame" />.
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

            // release the reference to the game
            // the game cache will recycle the game instance if no 
            // more refrences to the game are left.
            this.RoomReference.Dispose();

            // finally the peers state is set to null to indicate
            // that the peer is not attached to a room anymore.
            this.RoomReference = null;
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

            if (this.RoomReference == null)
            {
                return;
            }

            var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, this);
            this.RoomReference.Room.EnqueueMessage(message);
            this.RoomReference.Dispose();
            this.RoomReference = null;
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
                case OperationCode.Ping:
                    this.HandlePingOperation(operationRequest, sendParameters);
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
            }

            string message = string.Format("Unknown operation code {0}", operationRequest.OperationCode);
            this.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = message }, sendParameters);
        }

        /// <summary>
        ///   Checks if the the state of peer is set to a reference of a room.
        ///   If a room refrence is present the peer will be removed from the related room and the reference will be disposed. 
        ///   Disposing the reference allows the associated room factory to remove the room instance if no more references to the room exists.
        /// </summary>
        protected virtual void RemovePeerFromCurrentRoom()
        {
            // check if the peer already joined another game
            if (this.RoomReference != null)
            {
                // remove peer from his current game.
                var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, this);
                this.RoomReference.Room.EnqueueMessage(message);
                
                // release room reference
                this.RoomReference.Dispose();
                this.RoomReference = null;
            }
        }
        #endregion
    }
}
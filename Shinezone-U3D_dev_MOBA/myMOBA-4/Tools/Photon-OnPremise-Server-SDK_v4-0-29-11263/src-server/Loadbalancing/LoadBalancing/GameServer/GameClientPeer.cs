// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameClientPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GamePeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common.Authentication;
using Photon.Hive.Plugin;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System;

    using ExitGames.Logging;

    using Photon.Hive;
    using Photon.Hive.Caching;
    using Photon.Hive.Operations;
    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;

    using AuthSettings = Photon.Common.Authentication.Configuration.Auth.AuthSettings;
    using ErrorCode = Photon.Common.ErrorCode;
    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;

    #endregion

    public class GameClientPeer : HivePeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication application;


        #endregion

        #region Constructors and Destructors

        public GameClientPeer(InitRequest initRequest, GameApplication application)
            : base(initRequest)
        {
            this.application = application;

            if (this.application.AppStatsPublisher != null)
            {
                this.application.AppStatsPublisher.IncrementPeerCount();
            }

            this.HttpRpcCallsLimit = CommonSettings.Default.HttpRpcCallsLimit;
        }

        #endregion

        #region Properties


        public DateTime LastActivity { get; protected set; }

        public byte LastOperation { get; protected set; }

        protected  bool IsAuthenticated { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format(
                "{0}: {1}",
                this.GetType().Name,
                string.Format(
                    "PID {0}, IsConnected: {1}, IsDisposed: {2}, Last Activity: Operation {3} at UTC {4} in Room {7}, IP {5}:{6}, ",
                    this.ConnectionId,
                    this.Connected,
                    this.Disposed,
                    this.LastOperation,
                    this.LastActivity,
                    this.RemoteIP,
                    this.RemotePort,
                    this.RoomReference == null ? string.Empty : this.RoomReference.Room.Name)); 
        }

        public override bool IsThisSameSession(HivePeer peer)
        {
            return this.AuthToken != null && peer.AuthToken != null && this.AuthToken.AreEqual(peer.AuthToken);
        }

        #endregion

        #region Methods

        protected override RoomReference GetRoomReference(JoinGameRequest joinRequest, params object[] args)
        {
            throw new NotSupportedException("Use TryGetRoomReference or TryCreateRoomReference instead.");
        }

        protected override void OnRoomNotFound(string gameId)
        {
            this.application.MasterServerConnection.RemoveGameState(gameId);
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            base.OnDisconnect(reasonCode, reasonDetail);

            if (this.application.AppStatsPublisher != null)
            {
                this.application.AppStatsPublisher.DecrementPeerCount();
            }
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                if (request.OperationCode != (byte)Photon.Hive.Operations.OperationCode.RaiseEvent)
                {
                    log.DebugFormat("OnOperationRequest: conId={0}, opCode={1}", this.ConnectionId, request.OperationCode);
                }
            }

            this.LastActivity = DateTime.UtcNow;
            this.LastOperation = request.OperationCode;

            if (request.OperationCode == (byte) OperationCode.Authenticate)
            {
                if (this.IsAuthenticated)
                {
                    this.SendOperationResponse(new OperationResponse(request.OperationCode)
                    {
                        ReturnCode = (short) ErrorCode.OperationDenied,
                        DebugMessage = LBErrorMessages.AlreadyAuthenticated
                    }, sendParameters);
                    return;
                }

                this.HandleAuthenticateOperation(request, sendParameters);
                return;
            }

            if (!this.IsAuthenticated)
            {
                this.SendOperationResponse(new OperationResponse(request.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationDenied,
                    DebugMessage = LBErrorMessages.NotAuthorized
                }, sendParameters);
                return;
            }

            switch (request.OperationCode)
            {
                case (byte)OperationCode.CreateGame:
                    this.HandleCreateGameOperation(request, sendParameters);
                    return;

                case (byte)OperationCode.JoinGame:
                    this.HandleJoinGameOperation(request, sendParameters);
                    return;

                case (byte)Photon.Hive.Operations.OperationCode.Leave:
                    this.HandleLeaveOperation(request, sendParameters);
                    return;

                case (byte)Photon.Hive.Operations.OperationCode.Ping:
                    this.HandlePingOperation(request, sendParameters);
                    return;

                case (byte)OperationCode.DebugGame:
                    this.HandleDebugGameOperation(request, sendParameters);
                    return;

                case (byte)Photon.Hive.Operations.OperationCode.RaiseEvent:
                case (byte)Photon.Hive.Operations.OperationCode.GetProperties:
                case (byte)Photon.Hive.Operations.OperationCode.SetProperties:
                case (byte)Photon.Hive.Operations.OperationCode.ChangeGroups:
                    this.HandleGameOperation(request, sendParameters);
                    return;

                case (byte)Hive.Operations.OperationCode.Rpc:

                    this.HandleRpcOperation(request, sendParameters);
                    return;

                default:
                    this.HandleUnknownOperationCode(request, sendParameters);
                    return;
            }

        }

        protected void HandleUnknownOperationCode(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Unknown operation code: OpCode={0}", operationRequest.OperationCode);
            }

            this.SendOperationResponse(
                new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.OperationInvalid,
                DebugMessage = LBErrorMessages.UnknownOperationCode
            }, sendParameters);
        }

        protected override RoomReference GetOrCreateRoom(string gameId, params object[] args)
        {
            return this.application.GameCache.GetRoomReference(gameId, this, args);
        }

        protected override bool TryCreateRoom(string gameId, out RoomReference roomReference, params object[] args)
        {
            return this.application.GameCache.TryCreateRoom(gameId, this, out roomReference, args);
        }

        protected override bool TryGetRoomReference(string gameId, out RoomReference roomReference)
        {
            return this.application.GameCache.TryGetRoomReference(gameId, this, out roomReference);
        }

        protected override bool TryGetRoomWithoutReference(string gameId, out Room room)
        {
            return this.application.GameCache.TryGetRoomWithoutReference(gameId, out room); 
        }

        public virtual string GetRoomCacheDebugString(string gameId)
        {
            return this.application.GameCache.GetDebugString(gameId); 
        }

        protected virtual void HandleAuthenticateOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var request = new AuthenticateRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(request, sendParameters) == false)
            {
                return;
            }

            if (request.ClientAuthenticationType == 255 
                || !string.IsNullOrEmpty(request.Token)
                || AuthSettings.Default.Enabled
                )
            {
                var response = this.HandleAuthenticateTokenRequest(request);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(
                        "HandleAuthenticateRequest - Token Authentication done. Result: {0}; msg={1}",
                        response.ReturnCode,
                        response.DebugMessage); 
                }

                this.SendOperationResponse(response, sendParameters);
                return;
            }

            this.HandleTokenlessAuthenticateRequest(operationRequest, sendParameters, request);
        }

        protected void SetupPeer(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                this.UserId = userId;
            }
            this.IsAuthenticated = true;
        }

        private void SetupPeer(AuthenticationToken authToken)
        {
            this.SetupPeer(authToken.UserId);
            this.AuthCookie = authToken.AuthCookie != null && authToken.AuthCookie.Count > 0 ? authToken.AuthCookie : null;
            this.AuthToken = authToken;
        }

        private void HandleTokenlessAuthenticateRequest(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            AuthenticateRequest request)
        {
            this.SetupPeer(request.UserId);

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("HandleTokenlessAuthenticateRequest - Token Authentication done. UserId: {0}", this.UserId);
            }

            var response = new OperationResponse { OperationCode = operationRequest.OperationCode };
            this.SendOperationResponse(response, sendParameters);
        }

        private OperationResponse HandleAuthenticateTokenRequest(AuthenticateRequest request)
        {
            OperationResponse operationResponse;

            var authToken = this.GetValidAuthToken(request, out operationResponse);
            if (operationResponse != null || authToken == null)
            {
                return operationResponse;
            }

            this.SetupPeer(authToken);
            // publish operation response
            var responseObject = new AuthenticateResponse { QueuePosition = 0 };
            return new OperationResponse(request.OperationRequest.OperationCode, responseObject);
        }

        private AuthenticationToken GetValidAuthToken(AuthenticateRequest authenticateRequest,
                                                      out OperationResponse operationResponse)
        {
            operationResponse = null;
            if (this.application.TokenCreator == null)
            {
                log.ErrorFormat("No custom authentication supported: AuthTokenKey not specified in config.");

                operationResponse = new OperationResponse(authenticateRequest.OperationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.InvalidAuthentication,
                    DebugMessage = ErrorMessages.AuthTokenTypeNotSupported
                };

                return null;
            }

            // validate the authentication token
            if (string.IsNullOrEmpty(authenticateRequest.Token))
            {
                operationResponse = new OperationResponse(authenticateRequest.OperationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.InvalidAuthentication,
                    DebugMessage = ErrorMessages.AuthTokenMissing
                };

                return null;
            }

            AuthenticationToken authToken;
            var tokenCreator = this.application.TokenCreator;
            if (!tokenCreator.DecryptAuthenticationToken(authenticateRequest.Token, out authToken))
            {
                log.WarnFormat("Could not decrypt authenticaton token: {0}", authenticateRequest.Token);

                operationResponse = new OperationResponse(authenticateRequest.OperationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.InvalidAuthentication,
                    DebugMessage = ErrorMessages.AuthTokenTypeNotSupported
                };

                return null;
            }

            if (authToken.ValidToTicks < DateTime.UtcNow.Ticks)
            {
                operationResponse = new OperationResponse(authenticateRequest.OperationRequest.OperationCode)
                {
                    ReturnCode = (short)Photon.Common.ErrorCode.AuthenticationTokenExpired,
                    DebugMessage = ErrorMessages.AuthTokenExpired
                };
                return null;
            }

            return authToken;
        }

        protected override PluginTraits GetPluginTraits()
        {
            return application.GameCache.PluginManager.PluginTraits;
        }

        protected virtual void HandleDebugGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var debugRequest = new DebugGameRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(debugRequest, sendParameters) == false)
            {
                return;
            }

            string debug = string.Format("DebugGame called from PID {0}. {1}", this.ConnectionId, this.GetRoomCacheDebugString(debugRequest.GameId));
            operationRequest.Parameters.Add((byte)ParameterCode.Info, debug);


            if (this.RoomReference == null)
            {
                Room room;
                // get a room without obtaining a reference:
                if (!this.TryGetRoomWithoutReference(debugRequest.GameId, out room))
                {
                    var response = new OperationResponse
                    {
                        OperationCode = (byte)OperationCode.DebugGame,
                        ReturnCode = (short)ErrorCode.GameIdNotExists,
                        DebugMessage = HiveErrorMessages.GameIdDoesNotExist
                    };


                    this.SendOperationResponse(response, sendParameters);
                    return;
                }

                room.EnqueueOperation(this, operationRequest, sendParameters);
            }
            else
            {
                this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
            }
        }

        protected virtual void HandleRpcOperation(OperationRequest request, SendParameters sendParameters)
        {
            if (this.WebRpcHandler != null)
            {
                this.WebRpcHandler.HandleCall(this, this.UserId, request, this.AuthCookie, sendParameters);
                return;
            }

            this.SendOperationResponse(new OperationResponse
            {
                OperationCode = request.OperationCode,
                ReturnCode = (short)ErrorCode.OperationInvalid,
                DebugMessage = LBErrorMessages.RpcIsNotEnabled,
            }, sendParameters);
        }


        #region Interface Implementation

        #endregion
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterClientPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterClientPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
/*
 <AuthSettings Enabled="true" ClientAuthenticationAllowAnonymous="false">
    <AuthProviders>
      <AuthProvider
     Name="Custom"
        AuthenticationType="0"
        AuthUrl=""
    Key1="Val1"
    Key2="Val2"
        />
      <AuthProvider
     Name="Facebook"
        AuthenticationType="2"
        AuthUrl=""
    secret="Val1"
    appid="Val2"
        />
    </AuthProviders>
  </AuthSettings>
 * * 
 * */

using Photon.Common.Authentication;
using Photon.Common.Authentication.CustomAuthentication;
using Photon.Hive.WebRpc;
using Photon.LoadBalancing.Common;
using Photon.SocketServer.Diagnostics;

namespace Photon.LoadBalancing.MasterServer
{
    #region using directives

    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;
    using ExitGames.Logging;
    using Photon.LoadBalancing.Master.OperationHandler;
    using Photon.LoadBalancing.MasterServer.Lobby;
    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using Photon.Hive.Common.Lobby;
    using Photon.Hive.Operations;
    using Photon.SocketServer.Rpc;

    using AppLobby = Photon.LoadBalancing.MasterServer.Lobby.AppLobby;
    using ErrorCode = Photon.Common.ErrorCode;
    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;

    #endregion

    public class MasterClientPeer : Peer, ILobbyPeer, ICustomAuthPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private GameApplication application;

        protected AuthenticationToken unencryptedAuthToken;

        private readonly TimeIntervalCounter httpForwardedRequests = new TimeIntervalCounter(new TimeSpan(0, 0, 1));

        #endregion

        #region Constructors and Destructors

        public MasterClientPeer(InitRequest initRequest)
            : base(initRequest)
        {
            this.SetCurrentOperationHandler(OperationHandlerInitial.Instance);

            this.RequestFiber.Enqueue(() =>
                    {
                        if (MasterApplication.AppStats != null)
                        {
                            MasterApplication.AppStats.IncrementMasterPeerCount();
                            MasterApplication.AppStats.AddSubscriber(this);
                        }
                    }
                );

            this.HttpRpcCallsLimit = CommonSettings.Default.HttpRpcCallsLimit;
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public bool UseHostnames
        {
            get
            {
                return this.IsIPv6ToIPv4Bridged;
            }  
        }

        public bool HasExternalApi { get; set; }

        public virtual GameApplication Application
        {
            get
            {
                return this.application;
            }

            protected set
            {
                if (this.application == value)
                {
                    return;
                }

                var oldApp = Interlocked.Exchange(ref this.application, value);
                if (oldApp != null)
                {
                    oldApp.OnClientDisconnected(this);
                }

                if (value != null)
                {
                    value.OnClientConnected(this);
                }
            }
        }

        protected AppLobby AppLobby { get; set; }

        public IGameListSubscription GameChannelSubscription { get; set; }

        public WebRpcHandler WebRpcHandler { get; set; }

        public int HttpRpcCallsLimit { get; protected set; }

        #endregion

        public virtual string GetEncryptedAuthenticationToken(AuthenticateRequest request)
        {
            var app = (MasterApplication)ApplicationBase.Instance;

            if (this.unencryptedAuthToken == null)
            {
                this.unencryptedAuthToken = app.TokenCreator.CreateAuthenticationToken(this.UserId, request);
            }

            return app.TokenCreator.EncryptAuthenticationToken(this.unencryptedAuthToken, true);
        }

        #region IAuthenticatePeer
        public virtual void OnCustomAuthenticationError(Photon.Common.ErrorCode errorCode, string debugMessage,
                                                      IAuthenticateRequest authenticateRequest, SendParameters sendParameters,
                                                      object state)
        {
            try
            {
                if (this.Connected == false)
                {
                    return;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Client custom authentication failed: appId={0}, result={1}, msg={2}", authenticateRequest.ApplicationId, errorCode, debugMessage);
                }

                var operationResponse = new OperationResponse((byte)Hive.Operations.OperationCode.Authenticate)
                {
                    ReturnCode = (short)errorCode,
                    DebugMessage = debugMessage,
                };

                this.SendOperationResponse(operationResponse, sendParameters);
                this.SetCurrentOperationHandler(OperationHandlerInitial.Instance);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                var errorResponse = new OperationResponse((byte)Hive.Operations.OperationCode.Authenticate) { ReturnCode = (short)Photon.Common.ErrorCode.InternalServerError };
                this.SendOperationResponse(errorResponse, sendParameters);
            }
        }

        public virtual void OnCustomAuthenticationResult(CustomAuthenticationResult customAuthResult, IAuthenticateRequest authenticateRequest,
                                                 SendParameters sendParameters, object state)
        {
            var authRequest = (AuthenticateRequest)authenticateRequest;
            var authSettings = (AuthSettings)state;
            this.RequestFiber.Enqueue(() => this.DoCustomAuthenticationResult(customAuthResult, authRequest, sendParameters, authSettings));
        }

        private void DoCustomAuthenticationResult(CustomAuthenticationResult customAuthResult, 
            AuthenticateRequest authRequest, SendParameters sendParameters, AuthSettings customAuthSettings)
        {
            if (this.Connected == false)
            {
                return;
            }

            try
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Client custom authentication callback: result={0}, msg={1}, userId={2}",
                        customAuthResult.ResultCode,
                        customAuthResult.Message,
                        this.UserId);
                }

                var operationResponse = new OperationResponse((byte)Hive.Operations.OperationCode.Authenticate)
                {
                    DebugMessage = customAuthResult.Message,
                    Parameters = new Dictionary<byte, object>()
                };

                switch (customAuthResult.ResultCode)
                {
                    default:
                        operationResponse.ReturnCode = (short)Photon.Common.ErrorCode.CustomAuthenticationFailed;
                        this.SendOperationResponse(operationResponse, sendParameters);
                        this.SetCurrentOperationHandler(OperationHandlerInitial.Instance);
                        return;

                    case CustomAuthenticationResultCode.Data:
                        operationResponse.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.Data, customAuthResult.Data } };
                        this.SendOperationResponse(operationResponse, sendParameters);
                        this.SetCurrentOperationHandler(OperationHandlerInitial.Instance);
                        return;

                    case CustomAuthenticationResultCode.Ok:
                        //apply user id from custom auth result
                        if (!string.IsNullOrEmpty(customAuthResult.UserId))
                        {
                            this.UserId = customAuthResult.UserId;
                        }
                        else if (!string.IsNullOrEmpty(authRequest.UserId))
                        {
                            this.UserId = authRequest.UserId;
                        }
                        else
                        {
                            this.UserId = Guid.NewGuid().ToString();
                        }
                        // create auth token and send response
                        this.CreateAuthTokenAndSendResponse(customAuthResult, authRequest, sendParameters, customAuthSettings, operationResponse);
                        this.SetCurrentOperationHandler(OperationHandlerDefault.Instance);
                        this.OnAuthSuccess(authRequest);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                var errorResponse = new OperationResponse((byte)Hive.Operations.OperationCode.Authenticate) { ReturnCode = (short)ErrorCode.InternalServerError };
                this.SendOperationResponse(errorResponse, sendParameters);
                this.SetCurrentOperationHandler(OperationHandlerInitial.Instance);
            }
        }

        protected virtual void CreateAuthTokenAndSendResponse(CustomAuthenticationResult customAuthResult, AuthenticateRequest authRequest,
            SendParameters sendParameters, AuthSettings authSettings, OperationResponse operationResponse)
        {
            var app = (MasterApplication)ApplicationBase.Instance;
            this.unencryptedAuthToken = app.TokenCreator.CreateAuthenticationToken(
                authRequest,
                authSettings,
                this.UserId,
                customAuthResult.AuthCookie);

            operationResponse.Parameters.Add((byte) ParameterCode.Token, this.GetEncryptedAuthenticationToken(authRequest));
            operationResponse.Parameters.Add((byte) ParameterCode.Data, customAuthResult.Data);
            operationResponse.Parameters.Add((byte)ParameterCode.Nickname, customAuthResult.Nickname);
            operationResponse.Parameters.Add((byte)ParameterCode.UserId, this.UserId);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        #endregion

        #region Methods

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }

            // remove peer from the lobby if he has joined one
            if (this.AppLobby != null)
            {
                this.AppLobby.RemovePeer(this);
                this.AppLobby = null;
            }
            
            // remove the peer from the application
            this.Application = null;

            // update application statistics
            if (MasterApplication.AppStats != null)
            {
                MasterApplication.AppStats.DecrementMasterPeerCount();
                MasterApplication.AppStats.RemoveSubscriber(this);
            }
        }

        private AuthenticationToken GetValidAuthToken(AuthenticateRequest authenticateRequest, out OperationResponse operationResponse)
        {
            operationResponse = null;
            var photonApplication = (MasterApplication)ApplicationBase.Instance;

            if (photonApplication.TokenCreator == null)
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
            var tokenCreator = photonApplication.TokenCreator;
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

        private void OnAuthSuccess(AuthenticateRequest request)
        {
            var app = (MasterApplication)ApplicationBase.Instance;
            this.Application = app.DefaultApplication;

            // check if the peer wants to receive lobby statistic events
            if (request.ReceiveLobbyStatistics)
            {
                this.Application.LobbyStatsPublisher.Subscribe(this);
            }
        }

        public OperationResponse HandleAuthenticate(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate operation request
            var authenticateRequest = new AuthenticateRequest(this.Protocol, operationRequest);
            if (authenticateRequest.IsValid == false)
            {
                return OperationHandlerBase.HandleInvalidOperation(authenticateRequest, log);
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "HandleAuthenticateRequest:appId={0};version={1};region={2};type={3};userId={4}",
                    authenticateRequest.ApplicationId,
                    authenticateRequest.ApplicationVersion,
                    authenticateRequest.Region,
                    authenticateRequest.ClientAuthenticationType,
                    authenticateRequest.UserId);
            }

            if (authenticateRequest.ClientAuthenticationType == 255 || !string.IsNullOrEmpty(authenticateRequest.Token))
            {
                var response = this.HandleAuthenticateTokenRequest(authenticateRequest);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("HandleAuthenticateRequest - Token Authentication done. Result: {0}; msg={1}", response.ReturnCode, response.DebugMessage);
                }

                if (response.ReturnCode == 0)
                {
                    this.SetCurrentOperationHandler(OperationHandlerDefault.Instance);
                    this.OnAuthSuccess(authenticateRequest);
                }

                return response;
            }

            // if authentication data is used it must be either a byte array or a string value
            if (authenticateRequest.ClientAuthenticationData != null)
            {
                var dataType = authenticateRequest.ClientAuthenticationData.GetType();
                if (dataType != typeof(byte[]) && dataType != typeof(string))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("HandleAuthenticateRequest - invalid type for auth data (datatype = {0}), request: {1}", dataType, operationRequest.ToString());
                    }

                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (short)ErrorCode.OperationInvalid,
                        DebugMessage = ErrorMessages.InvalidTypeForAuthData
                    };
                }
            }

            var app = (MasterApplication)ApplicationBase.Instance;

            // check if custom client authentication is required
            if (app.CustomAuthHandler.IsClientAuthenticationEnabled)
            {
                if (app.TokenCreator == null)
                {
                    log.WarnFormat("No custom authentication supported: AuthTokenKey not specified in config.");

                    var response = new OperationResponse(authenticateRequest.OperationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.InvalidAuthentication,
                        DebugMessage = ErrorMessages.AuthTokenTypeNotSupported
                    };

                    return response;
                }


                this.SetCurrentOperationHandler(OperationHandlerAuthenticating.Instance);

                var authSettings = new AuthSettings
                                   {
                                       IsAnonymousAccessAllowed = app.CustomAuthHandler.IsAnonymousAccessAllowed,
                                   };

                app.CustomAuthHandler.AuthenticateClient(this, authenticateRequest, authSettings, new SendParameters(), authSettings);
                return null;
            }

            // TBD: centralizing setting of userid
            this.UserId = authenticateRequest.UserId;


            // apply application to the peer
            this.SetCurrentOperationHandler(OperationHandlerDefault.Instance);

            this.OnAuthSuccess(authenticateRequest);

            // publish operation response
            return new OperationResponse(operationRequest.OperationCode);
        }

        private OperationResponse HandleAuthenticateTokenRequest(AuthenticateRequest request)
        {
            OperationResponse operationResponse;

            var authToken = this.GetValidAuthToken(request, out operationResponse);
            if (operationResponse != null || authToken == null)
            {
                return operationResponse;
            }

            this.UserId = authToken.UserId;
            this.unencryptedAuthToken = authToken;

            // publish operation response
            operationResponse = new OperationResponse(request.OperationRequest.OperationCode, new AuthenticateResponse { QueuePosition = 0 });
            operationResponse.Parameters.Add((byte)ParameterCode.Token, this.GetEncryptedAuthenticationToken(request));
            //operationResponse.Parameters.Add((byte)ParameterCode.Nickname, authToken.Nickname);
            //operationResponse.Parameters.Add((byte)ParameterCode.UserId, this.UserId);
            return operationResponse;
        }

        public OperationResponse HandleJoinLobby(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var joinLobbyRequest = new JoinLobbyRequest(this.Protocol, operationRequest);
            
            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinLobbyRequest, log, out response) == false)
            {
                return response;
            }

            if (joinLobbyRequest.LobbyType > 3)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = "Invalid lobby type " + joinLobbyRequest.LobbyType
                };
            }

            // remove peer from the currently joined lobby
            if (this.AppLobby != null)
            {
                this.AppLobby.RemovePeer(this);
                this.AppLobby = null;
            }

            AppLobby lobby;
            if (!this.Application.LobbyFactory.GetOrCreateAppLobby(joinLobbyRequest.LobbyName, (AppLobbyType)joinLobbyRequest.LobbyType , out lobby))
            {
                // getting here should never happen
                if (log.IsWarnEnabled)
                {
                    log.WarnFormat("Could not get or create lobby: name={0}, type={1}", joinLobbyRequest.LobbyName, (AppLobbyType)joinLobbyRequest.LobbyType);
                }
                return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (short)ErrorCode.InternalServerError,
                        DebugMessage = LBErrorMessages.CanNotCreateLobby,
                    };
            }

            this.AppLobby = lobby;
            this.AppLobby.JoinLobby(this, joinLobbyRequest, sendParameters);

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Joined lobby: {0}, {1}, u:'{2}'", joinLobbyRequest.LobbyName, joinLobbyRequest.LobbyType, this.UserId);
            }

            return null;
        }

        public OperationResponse HandleLeaveLobby(OperationRequest operationRequest)
        {
            this.GameChannelSubscription = null;

            if (this.AppLobby == null)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode, 
                    ReturnCode = (short)ErrorCode.Ok, 
                    DebugMessage = LBErrorMessages.LobbyNotJoined
                };
            }

            this.AppLobby.RemovePeer(this);
            this.AppLobby = null;

            return new OperationResponse(operationRequest.OperationCode);
        }

        public OperationResponse HandleCreateGame(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var createGameRequest = new CreateGameRequest(this.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(createGameRequest, log, out response) == false)
            {
                return response;
            }

            if (createGameRequest.LobbyType > 3)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = "Invalid lobby type " + createGameRequest.LobbyType
                };
            }

            AppLobby lobby;
            response = this.TryGetLobby(createGameRequest.LobbyName, createGameRequest.LobbyType, operationRequest.OperationCode, out lobby);
            if (response != null)
            {
                return response;
            }

            lobby.EnqueueOperation(this, operationRequest, sendParameters);
            return null;
        }

        private OperationResponse TryGetLobby(string lobbyName, byte lobbyType, byte operationCode, out AppLobby lobby)
        {
            if (string.IsNullOrEmpty(lobbyName) && this.AppLobby != null)
            {
                lobby = this.AppLobby;
                return null;
            }

            if (!this.Application.LobbyFactory.GetOrCreateAppLobby(lobbyName, (AppLobbyType) lobbyType, out lobby))
            {
                // getting here should never happen
                if (log.IsWarnEnabled)
                {
                    log.WarnFormat("Could not get or create lobby: name={0}, type={1}", lobbyName, lobbyType);
                }

                return new OperationResponse
                {
                    OperationCode = operationCode,
                    ReturnCode = (short)ErrorCode.InternalServerError,
                    DebugMessage = LBErrorMessages.LobbyNotExist,
                };

            }

            return null;
        }

        public OperationResponse HandleFindFriends(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate the operation request
            OperationResponse response;
            var operation = new FindFriendsRequest(this.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            // check if player online cache is available for the application
            var playerCache = this.Application.PlayerOnlineCache;
            if (playerCache == null)
            {
                return new OperationResponse((byte)OperationCode.FindFriends)
                {
                    ReturnCode = (short)ErrorCode.InternalServerError,
                    DebugMessage = "PlayerOnlineCache is not set!"
                };
            }

            playerCache.FiendFriends(this, operation, sendParameters);
            return null;
        }

        public OperationResponse HandleJoinGame(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var joinGameRequest = new JoinGameRequest(this.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinGameRequest, log, out response) == false)
            {
                return response;
            }

            GameState gameState;
            if (this.Application.TryGetGame(joinGameRequest.GameId, out gameState))
            {
                gameState.Lobby.EnqueueOperation(this, operationRequest, sendParameters);
                return null;
            }

            if (joinGameRequest.JoinMode == JoinModes.JoinOnly && !this.Application.PluginTraits.AllowAsyncJoin)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode, 
                    ReturnCode = (short)ErrorCode.GameIdNotExists, 
                    DebugMessage = HiveErrorMessages.GameIdDoesNotExist
                };
            }

            AppLobby lobby;
            response = this.TryGetLobby(joinGameRequest.LobbyName, joinGameRequest.LobbyType, operationRequest.OperationCode, out lobby);
            if (response != null)
            {
                return response;
            }
            lobby.EnqueueOperation(this, operationRequest, sendParameters);
            return null;
        }

        public OperationResponse HandleJoinRandomGame(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var joinRandomGameRequest = new JoinRandomGameRequest(this.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinRandomGameRequest, log, out response) == false)
            {
                return response;
            }

            AppLobby lobby;
            response = this.TryGetLobby(joinRandomGameRequest.LobbyName, 
                joinRandomGameRequest.LobbyType, operationRequest.OperationCode, out lobby);
            if (response != null)
            {
                return response;
            }

            lobby.EnqueueOperation(this, operationRequest, sendParameters);
            return null;
        }

        public OperationResponse HandleLobbyStatsRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse response;

            var getStatsRequest = new GetLobbyStatsRequest(this.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(getStatsRequest, log, out response) == false)
            {
                return response;
            }

            this.Application.LobbyStatsPublisher.EnqueueGetStatsRequest(this, getStatsRequest, sendParameters);
            return null;
        }

        public OperationResponse HandleRpcRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (this.WebRpcHandler != null)
            {
                if (this.HttpRpcCallsLimit > 0 && this.httpForwardedRequests.Increment(1) > this.HttpRpcCallsLimit)
                {
                    var resp = new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (short)ErrorCode.HttpLimitReached,
                        DebugMessage = HiveErrorMessages.HttpForwardedOperationsLimitReached
                    };

                    this.SendOperationResponse(resp, sendParameters);
                    return null;
                }

                this.WebRpcHandler.HandleCall(this, this.UserId, operationRequest, this.unencryptedAuthToken.AuthCookie, sendParameters);
                return null;
            }

            return new OperationResponse
            {
                OperationCode = operationRequest.OperationCode, 
                ReturnCode = (short)ErrorCode.OperationDenied, 
                DebugMessage = LBErrorMessages.RpcIsNotSetup
            };
        }

        #endregion
    }
}
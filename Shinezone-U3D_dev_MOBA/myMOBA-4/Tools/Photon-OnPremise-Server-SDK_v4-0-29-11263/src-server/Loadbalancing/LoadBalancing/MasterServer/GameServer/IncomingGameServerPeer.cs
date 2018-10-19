// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncomingGameServerPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IncomingGameServerPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;
using Photon.Common.LoadBalancer.Common;
using Photon.Common.LoadBalancer.LoadShedding;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.MasterServer.GameServer
{
    #region using directives

    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;

    using ExitGames.Logging;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.LoadBalancing.ServerToServer.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using OperationCode = Photon.LoadBalancing.ServerToServer.Operations.OperationCode;

    #endregion

    public class IncomingGameServerPeer : InboundS2SPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly MasterApplication application;

        #endregion

        #region Constructors and Destructors

        public IncomingGameServerPeer(InitRequest initRequest, MasterApplication application)
            : base(initRequest)
        {
            this.application = application;
            log.InfoFormat("game server connection from {0}:{1} established (id={2})", this.RemoteIP, this.RemotePort, this.ConnectionId);
            this.SetPrivateCustomTypeCache(this.application.GetS2SCustomTypeCache());
        }

        #endregion

        #region Properties

        public string Key { get; protected set; }
        
        public Guid? ServerId { get; protected set; }

        public string Address { get; protected set; }

        public string AddressIPv6 { get; protected set; }

        public string Hostname { get; protected set; }

        // IPv4
        public string TcpAddress { get; protected set; }

        public string UdpAddress { get; protected set; }

        public string WebSocketAddress { get; protected set; }

        public string HttpAddress { get; protected set; }

        // IPv6
        public string TcpAddressIPv6 { get; protected set; }

        public string UdpAddressIPv6 { get; protected set; }

        public string WebSocketAddressIPv6 { get; protected set; }

        public string HttpAddressIPv6 { get; protected set; }

        // Hostname
        public string TcpHostname { get; protected set; }

        public string UdpHostname { get; protected set; }

        public string WebSocketHostname { get; protected set; }

        public string HttpHostname { get; protected set; }

        public string SecureWebSocketHostname { get; protected set; }

        public string SecureHttpHostname { get; protected set; }

        public FeedbackLevel LoadLevel { get; private set; }

        public ServerState State { get; private set; }

        public int PeerCount { get; private set; }

        #endregion

        #region Public Methods

        public void RemoveGameServerPeerOnMaster()
        {
            if (this.ServerId.HasValue)
            {
                this.application.GameServers.OnDisconnect(this);
                this.application.LoadBalancer.TryRemoveServer(this);
                this.application.RemoveGameServerFromLobby(this); 
            }
        }

        public override string ToString()
        {
            if (this.ServerId.HasValue)
            {
                return string.Format("GameServer({2}) at {0}/{1}", this.TcpAddress, this.UdpAddress, this.ServerId);
            }

            return base.ToString();
        }

        #endregion

        #region Methods

        protected virtual Hashtable GetAuthlist()
        {
            return null;
        }

        protected virtual byte[] SharedKey
        {
            get { return null; }
        }

        protected virtual OperationResponse HandleRegisterGameServerRequest(OperationRequest request)
        {
            try
            {
                var registerRequest = new RegisterGameServer(this.Protocol, request);

                if (registerRequest.IsValid == false)
                {
                    string msg = registerRequest.GetErrorMessage();
                    log.ErrorFormat("RegisterGameServer contract error: {0}", msg);

                    return new OperationResponse(request.OperationCode) { DebugMessage = msg, ReturnCode = (short)ErrorCode.OperationInvalid };
                }

                IPAddress masterAddress = this.application.GetInternalMasterNodeIpAddress();
                var contract = new RegisterGameServerResponse { InternalAddress = masterAddress.GetAddressBytes() };

                // is master
                if (!this.application.IsMaster)
                {
                    return new OperationResponse(request.OperationCode, contract)
                               {
                                   ReturnCode = (short)ErrorCode.RedirectRepeat,
                                   DebugMessage = "RedirectRepeat"
                               };
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(
                        "Received register request: Address={0}, UdpPort={1}, TcpPort={2}, WebSocketPort={3}, SecureWebSocketPort={4}, HttpPort={5}, State={6}, Hostname={7}, IPv6Address={8}",
                        registerRequest.GameServerAddress,
                        registerRequest.UdpPort,
                        registerRequest.TcpPort,
                        registerRequest.WebSocketPort,
                        registerRequest.SecureWebSocketPort,
                        registerRequest.HttpPort,
                        (ServerState)registerRequest.ServerState,
                        registerRequest.GameServerHostName,
                        registerRequest.GameServerAddressIPv6);
                }

                this.Address = registerRequest.GameServerAddress;
                if (registerRequest.GameServerAddressIPv6 != null
                    && IPAddress.Parse(registerRequest.GameServerAddressIPv6).AddressFamily == AddressFamily.InterNetworkV6)
                {
                    this.AddressIPv6 = string.Format("[{0}]", IPAddress.Parse(registerRequest.GameServerAddressIPv6));
                }
                this.Hostname = registerRequest.GameServerHostName;

                if (registerRequest.UdpPort.HasValue)
                {
                    this.UdpAddress = string.IsNullOrEmpty(this.Address) ? null : string.Format("{0}:{1}", this.Address, registerRequest.UdpPort);
                    this.UdpAddressIPv6 = string.IsNullOrEmpty(this.AddressIPv6) ? null : string.Format("{0}:{1}", this.AddressIPv6, registerRequest.UdpPort);
                    this.UdpHostname = string.IsNullOrEmpty(this.Hostname) ? null : string.Format("{0}:{1}", this.Hostname, registerRequest.UdpPort);
                }

                if (registerRequest.TcpPort.HasValue)
                {
                    this.TcpAddress = string.IsNullOrEmpty(this.Address) ? null : string.Format("{0}:{1}", this.Address, registerRequest.TcpPort);
                    this.TcpAddressIPv6 = string.IsNullOrEmpty(this.AddressIPv6) ? null : string.Format("{0}:{1}", this.AddressIPv6, registerRequest.TcpPort); 
                    this.TcpHostname = string.IsNullOrEmpty(this.Hostname) ? null : string.Format("{0}:{1}", this.Hostname, registerRequest.TcpPort);
                }

                if (registerRequest.WebSocketPort.HasValue && registerRequest.WebSocketPort != 0)
                {
                    this.WebSocketAddress = string.IsNullOrEmpty(this.Address)
                                                ? null
                                                : string.Format("ws://{0}:{1}", this.Address, registerRequest.WebSocketPort);

                    this.WebSocketAddressIPv6 = string.IsNullOrEmpty(this.AddressIPv6)
                                                    ? null
                                                    : string.Format("ws://{0}:{1}", this.AddressIPv6, registerRequest.WebSocketPort);

                    this.WebSocketHostname = string.IsNullOrEmpty(this.Hostname)
                                                 ? null
                                                 : string.Format("ws://{0}:{1}", this.Hostname, registerRequest.WebSocketPort);
                }

                if (registerRequest.HttpPort.HasValue && registerRequest.HttpPort != 0)
                {
                    this.HttpAddress = string.IsNullOrEmpty(this.Address)
                                           ? null
                                           : string.Format("http://{0}:{1}{2}", this.Address, registerRequest.HttpPort, registerRequest.HttpPath);

                    this.HttpAddressIPv6 = string.IsNullOrEmpty(this.AddressIPv6)
                                               ? null
                                               : string.Format("http://{0}:{1}{2}", this.AddressIPv6, registerRequest.HttpPort, registerRequest.HttpPath);

                    this.HttpHostname = string.IsNullOrEmpty(this.Hostname)
                                            ? null
                                            : string.Format("http://{0}:{1}{2}", this.Hostname, registerRequest.HttpPort, registerRequest.HttpPath);
                }

                // HTTP & WebSockets require a proper domain name (especially for certificate validation on secure Websocket & HTTPS connections): 
                if (string.IsNullOrEmpty(this.Hostname))
                {
                    log.WarnFormat("HTTPs & Secure WebSockets not supported. GameServer {0} does not have a public hostname.", this.Address);
                }
                else
                {
                    if (registerRequest.SecureWebSocketPort.HasValue && registerRequest.SecureWebSocketPort != 0)
                    {
                        this.SecureWebSocketHostname = string.Format("wss://{0}:{1}", this.Hostname, registerRequest.SecureWebSocketPort);
                    }

                    if (registerRequest.SecureHttpPort.HasValue && registerRequest.SecureHttpPort != 0)
                    {
                        this.SecureHttpHostname = string.Format("https://{0}:{1}{2}", this.Hostname, registerRequest.SecureHttpPort, registerRequest.HttpPath);
                    }
                }

                this.ServerId = new Guid(registerRequest.ServerId);
                this.State = (ServerState)registerRequest.ServerState;

                this.Key = string.Format("{0}-{1}-{2}", registerRequest.GameServerAddress, registerRequest.UdpPort, registerRequest.TcpPort);

                log.Debug(
                    string.Format(
                        "Registered GameServerAddress={0} GameServerAddressIPv6={1}" + " TcpAddress={2} TcpAddressIPv6={3} UdpAddress={4} UdpAddressIPv6={5}"
                        + " WebSocketAddress={6} WebSocketAddressIPv6={7} HttpAddress={8} HttpAddressIPv6={9}"
                        + " SecureWebSocketAddress={10} SecureHttpAddress={11}",
                        this.Address,
                        this.AddressIPv6,
                        this.TcpAddress,
                        this.TcpAddressIPv6,
                        this.UdpAddress,
                        this.UdpAddressIPv6,
                        this.WebSocketAddress,
                        this.WebSocketAddressIPv6,
                        this.HttpAddress,
                        this.HttpAddressIPv6,
                        this.SecureWebSocketHostname,
                        this.SecureHttpHostname));

                this.application.GameServers.OnConnect(this);

                if (this.State == ServerState.Normal)
                {
                    this.application.LoadBalancer.TryAddServer(this, 0);
                }

                contract.AuthList = this.GetAuthlist();

                return new OperationResponse(request.OperationCode, contract);
            }
            catch (Exception e)
            {
                log.Error(e);
                return new OperationResponse(request.OperationCode) { DebugMessage = e.Message, ReturnCode = (short)ErrorCode.InternalServerError };
            }
        }

        protected virtual void HandleRemoveGameState(IEventData eventData)
        {
            var removeEvent = new RemoveGameEvent(this.Protocol, eventData);
            if (removeEvent.IsValid == false)
            {
                string msg = removeEvent.GetErrorMessage();
                log.ErrorFormat("RemoveGame contract error: {0}", msg);
                return;
            }

            this.application.DefaultApplication.OnGameRemovedOnGameServer(removeEvent.GameId);
        }

        protected virtual void HandleUpdateGameServerEvent(IEventData eventData)
        {
            var updateGameServer = new UpdateServerEvent(this.Protocol, eventData);
            if (updateGameServer.IsValid == false)
            {
                string msg = updateGameServer.GetErrorMessage();
                log.ErrorFormat("UpdateServer contract error: {0}", msg);
                return;
            }

            var previuosLoadLevel = this.LoadLevel;

            this.LoadLevel = (FeedbackLevel)updateGameServer.LoadIndex;
            this.PeerCount = updateGameServer.PeerCount;

            if ((ServerState)updateGameServer.State != this.State)
            {
                this.SetServerState((ServerState)updateGameServer.State);
            }
            else if (previuosLoadLevel != this.LoadLevel && this.State == ServerState.Normal)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("UpdateGameServer - from LoadLevel {0} to {1}, PeerCount {2}", previuosLoadLevel, this.LoadLevel, this.PeerCount);
                }
                
                if (!this.application.LoadBalancer.TryUpdateServer(this, this.LoadLevel))
                {
                    log.WarnFormat("Failed to update game server state for {0}", this.TcpAddress);
                }
            } 
        }

        private void SetServerState(ServerState serverState)
        {
            if (this.State == serverState)
            {
                return;
            }

            if (serverState < ServerState.Normal || serverState > ServerState.Offline)
            {
                log.WarnFormat("Invalid server state for {0}: old={1}, new={2}", this.TcpAddress, this.State, serverState);
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GameServer state changed for {0}: old={1}, new={2} ", this.TcpAddress, this.State, serverState);
            }

            this.State = serverState;

            switch (serverState)
            {
                case ServerState.Normal:
                    if (this.application.LoadBalancer.TryAddServer(this, this.LoadLevel) == false)
                    {
                        log.WarnFormat("Failed to add game server to load balancer: serverId={0}", this.ServerId);
                    }
                    break;

                case ServerState.OutOfRotation:
                    this.application.LoadBalancer.TryRemoveServer(this);
                    break;

                case ServerState.Offline:
                    this.application.LoadBalancer.TryRemoveServer(this);
                    this.application.RemoveGameServerFromLobby(this);
                    break;
            }
        }

        protected virtual void HandleUpdateGameState(IEventData eventData)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("HandleUpdateGameState");
            }

            var updateEvent = new UpdateGameEvent(this.Protocol, eventData);
            if (updateEvent.IsValid == false)
            {
                string msg = updateEvent.GetErrorMessage();
                log.ErrorFormat("UpdateGame contract error: {0}", msg);
                return;
            }

            this.application.DefaultApplication.OnGameUpdateOnGameServer(updateEvent, this);
        }

        private void HandleUpdateAppStatsEvent(IEventData eventData)
        {
            if (MasterApplication.AppStats != null)
            {
                var updateAppStatsEvent = new UpdateAppStatsEvent(this.Protocol, eventData);
                MasterApplication.AppStats.UpdateGameServerStats(this, updateAppStatsEvent.PlayerCount, updateAppStatsEvent.GameCount);
            }
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsInfoEnabled)
            {
                string serverId = this.ServerId.HasValue ? this.ServerId.ToString() : "{null}";
                log.InfoFormat("OnDisconnect: game server connection closed (connectionId={0}, serverId={1}, reason={2})", this.ConnectionId, serverId, reasonCode);
            }

            this.RemoveGameServerPeerOnMaster();
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            try
            {
                if (!this.ServerId.HasValue)
                {
                    log.WarnFormat("received game server event {0} but server is not registered", eventData.Code);
                    return;
                }

                switch ((ServerEventCode)eventData.Code)
                {
                    default:
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Received unknown event code {0}", eventData.Code);
                        }

                        break;

                    case ServerEventCode.UpdateServer:
                        this.HandleUpdateGameServerEvent(eventData);
                        break;

                    case ServerEventCode.UpdateGameState:
                        this.HandleUpdateGameState(eventData);
                        break;

                    case ServerEventCode.RemoveGameState:
                        this.HandleRemoveGameState(eventData);
                        break;

                    case ServerEventCode.UpdateAppStats:
                        this.HandleUpdateAppStatsEvent(eventData);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("OnOperationRequest: pid={0}, op={1}", this.ConnectionId, request.OperationCode);
                }

                OperationResponse response;

                switch ((OperationCode)request.OperationCode)
                {
                    default:
                        response = new OperationResponse(request.OperationCode)
                        {
                            ReturnCode = (short)ErrorCode.OperationInvalid, 
                            DebugMessage = LBErrorMessages.UnknownOperationCode
                        };
                        break;

                    case OperationCode.RegisterGameServer:
                        {
                            response = this.ServerId.HasValue
                                           ? new OperationResponse(request.OperationCode) { ReturnCode = (short)ErrorCode.InternalServerError, DebugMessage = "already registered" }
                                           : this.HandleRegisterGameServerRequest(request);
                            break;
                        }
                }

                this.SendOperationResponse(response, sendParameters);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutgoingMasterServerPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OutgoingMasterServerPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;
using Photon.Common.LoadBalancer.Common;
using Photon.Common.LoadBalancer.LoadShedding;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System;
    using System.Net;

    using ExitGames.Logging;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.LoadBalancing.ServerToServer.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using PhotonHostRuntimeInterfaces;

    using OperationCode = Photon.LoadBalancing.ServerToServer.Operations.OperationCode;

    #endregion

    public class OutgoingMasterServerPeer : OutboundS2SPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication application;

        private readonly MasterServerConnectionBase masterServerConnection;

        private bool redirected;

        private IDisposable updateLoop;

        #endregion

        #region Constructors and Destructors

        public OutgoingMasterServerPeer(MasterServerConnectionBase masterServerConnection)
            : base(masterServerConnection.Application)
        {
            this.masterServerConnection = masterServerConnection;
            this.application = masterServerConnection.Application;            
            this.SetPrivateCustomTypeCache(this.application.GetS2SCustomTypeCache());
        }

        #endregion

        #region Properties

        public bool IsRegistered { get; protected set; }

        #endregion

        #region Public Methods

        public void UpdateServerState(FeedbackLevel workload, int peerCount, ServerState state)
        {
            if (!this.IsRegistered)
            {
                return;
            }

            var contract = new UpdateServerEvent { LoadIndex = (byte)workload, PeerCount = peerCount, State = (int)state };
            var eventData = new EventData((byte)ServerEventCode.UpdateServer, contract);
            this.SendEvent(eventData, new SendParameters());
        }

        public void UpdateServerState()
        {
            if (this.Connected == false)
            {
                return;
            }

            this.UpdateServerState(
                this.application.WorkloadController.FeedbackLevel,
                this.application.PeerCount,
                this.application.WorkloadController.ServerState);
        }

        #endregion

        #region Methods

        protected virtual void HandleRegisterGameServerResponse(OperationResponse operationResponse)
        {
            var contract = new RegisterGameServerResponse(this.Protocol, operationResponse);
            if (!contract.IsValid)
            {
                if (operationResponse.ReturnCode != (short)ErrorCode.Ok)
                {
                    log.ErrorFormat("RegisterGameServer returned with err {0}: {1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
                }

                log.Error("RegisterGameServerResponse contract invalid: " + contract.GetErrorMessage());
                this.Disconnect();
                return;
            }

            switch (operationResponse.ReturnCode)
            {
                case (short)ErrorCode.Ok:
                    {
                        log.InfoFormat("Successfully registered at master server: serverId={0}", this.application.ServerId);
                        this.IsRegistered = true;
                        this.masterServerConnection.UpdateAllGameStates();
                        this.StartUpdateLoop();
                        this.OnRegisteredAtMaster(contract);
                        break;
                    }

                case (short)ErrorCode.RedirectRepeat:
                    {
                        // TODO: decide whether to connect to internal or external address (config)
                        // use a new peer since we otherwise might get confused with callbacks like disconnect
                        var address = new IPAddress(contract.InternalAddress);
                        log.InfoFormat("Connected master server is not the leader; Reconnecting to master at IP {0}...", address);
                        this.Reconnect(address); // don't use proxy for direct connections

                        // enable for external address connections
                        //// var address = new IPAddress(contract.ExternalAddress);
                        //// log.InfoFormat("Connected master server is not the leader; Reconnecting to node {0} at IP {1}...", contract.MasterNode, address);
                        //// this.Reconnect(address, contract.MasterNode);
                        break;
                    }

                default:
                    {
                        log.WarnFormat("Failed to register at master: err={0}, msg={1}, serverid={2}", operationResponse.ReturnCode, operationResponse.DebugMessage, this.application.ServerId);
                        this.Disconnect();
                        break;
                    }
            }
        }

        protected virtual void OnRegisteredAtMaster(RegisterGameServerResponse registerResponse)
        {

        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            this.IsRegistered = false;
            this.StopUpdateLoop();

            // if RegisterGameServerResponse tells us to connect somewhere else we don't need to reconnect here
            if (this.redirected)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} disconnected from master server: reason={1}, detail={2}, serverId={3}", this.ConnectionId, reasonCode, reasonDetail, this.application.ServerId);
                }
            }
            else
            {
                log.InfoFormat("connection to master closed (id={0}, reason={1}, detail={2}), serverId={3}", this.ConnectionId, reasonCode, reasonDetail, this.application.ServerId);
                this.masterServerConnection.ReconnectToMaster();
            }
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Received unknown operation code {0}", request.OperationCode);
            }

            var response = new OperationResponse
            {
                OperationCode = request.OperationCode, 
                ReturnCode = (short)ErrorCode.InternalServerError, 
                DebugMessage = LBErrorMessages.UnknownOperationCode,
            };
            this.SendOperationResponse(response, sendParameters);
        }
        
        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            try
            {
                switch ((OperationCode)operationResponse.OperationCode)
                {
                    default:
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("Received unknown operation code {0}", operationResponse.OperationCode);
                            }

                            break;
                        }

                    case OperationCode.RegisterGameServer:
                        {
                            this.HandleRegisterGameServerResponse(operationResponse);
                            break;
                        }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void Reconnect(IPAddress address)
        {
            this.redirected = true;

            log.InfoFormat("Reconnecting to master: serverId={0}", this.application.ServerId);

            this.masterServerConnection.ConnectToMaster(new IPEndPoint(address, this.RemotePort));
            this.Disconnect();
            this.Dispose();
        }

        protected virtual void Register()
        {
            var contract = new RegisterGameServer
                {
                    GameServerAddress = this.application.PublicIpAddress.ToString(),
                    GameServerHostName = GameServerSettings.Default.PublicHostName,
                    
                    UdpPort = GameServerSettings.Default.RelayPortUdp == 0 ? this.application.GamingUdpPort : GameServerSettings.Default.RelayPortUdp + this.application.GetCurrentNodeId() - 1,
                    TcpPort = GameServerSettings.Default.RelayPortTcp == 0 ? this.application.GamingTcpPort : GameServerSettings.Default.RelayPortTcp + this.application.GetCurrentNodeId() - 1,
                    WebSocketPort = GameServerSettings.Default.RelayPortWebSocket == 0 ? this.application.GamingWebSocketPort : GameServerSettings.Default.RelayPortWebSocket + this.application.GetCurrentNodeId() - 1,
                    SecureWebSocketPort = GameServerSettings.Default.RelayPortSecureWebSocket == 0 ? this.application.GamingSecureWebSocketPort : GameServerSettings.Default.RelayPortSecureWebSocket  + this.application.GetCurrentNodeId() - 1,
                    HttpPort = GameServerSettings.Default.RelayPortHttp == 0 ? this.application.GamingHttpPort : GameServerSettings.Default.RelayPortHttp + this.application.GetCurrentNodeId() - 1,
                    SecureHttpPort = this.application.GamingHttpsPort,
                    HttpPath = this.application.GamingHttpPath,
                    ServerId = this.application.ServerId.ToString(),
                    ServerState = (int)this.application.WorkloadController.ServerState
                };

            if (this.application.PublicIpAddressIPv6 != null)
            {
                contract.GameServerAddressIPv6 = this.application.PublicIpAddressIPv6.ToString();
            }

            if (log.IsInfoEnabled)
            {
                log.InfoFormat(
                    "Registering game server with address {0}, TCP {1}, UDP {2}, WebSocket {3}, Secure WebSocket {4}, HTTP {5}, ServerID {6}, Hostname {7}, IPv6Address {8}",
                    contract.GameServerAddress,
                    contract.TcpPort,
                    contract.UdpPort,
                    contract.WebSocketPort,
                    contract.SecureWebSocketPort,
                    contract.HttpPort,
                    contract.ServerId,
                    contract.GameServerHostName,
                    contract.GameServerAddressIPv6);
            }

            var request = new OperationRequest((byte)OperationCode.RegisterGameServer, contract);
            this.SendOperationRequest(request, new SendParameters());
        }

        protected void StartUpdateLoop()
        {
            if (this.updateLoop != null)
            {
                log.Error("Update Loop already started! Duplicate RegisterGameServer response?");
                this.updateLoop.Dispose();
            }

            this.updateLoop = this.RequestFiber.ScheduleOnInterval(this.UpdateServerState, 1000, 1000);
            this.application.WorkloadController.FeedbacklevelChanged += this.WorkloadController_OnFeedbacklevelChanged;
        }

        protected void StopUpdateLoop()
        {
            if (this.updateLoop != null)
            {
                this.updateLoop.Dispose();
                this.updateLoop = null;

                this.application.WorkloadController.FeedbacklevelChanged -= this.WorkloadController_OnFeedbacklevelChanged;
            }
        }

        private void WorkloadController_OnFeedbacklevelChanged(object sender, EventArgs e)
        {
            this.UpdateServerState();
        }

        #endregion

        protected override void OnConnectionEstablished(object responseObject)
        {
            this.masterServerConnection.OnConnectionEstablished(responseObject);
            this.RequestFiber.Enqueue(this.Register);
        }

        protected override void OnConnectionFailed(int errorCode, string errorMessage)
        {
            this.masterServerConnection.OnConnectionFailed(errorCode, errorMessage);
        }
    }
}
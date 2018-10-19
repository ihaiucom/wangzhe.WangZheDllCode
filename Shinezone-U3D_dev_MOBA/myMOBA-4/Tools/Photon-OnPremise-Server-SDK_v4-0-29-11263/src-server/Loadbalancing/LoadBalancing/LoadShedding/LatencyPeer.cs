// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LatencyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LatencyPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.LoadShedding
{
    #region

    using ExitGames.Logging;

    using Photon.LoadBalancing.GameServer;
    using Photon.SocketServer;

    using PhotonHostRuntimeInterfaces;
    using System.Threading;

    #endregion

    /// <summary>
    /// Peer implementation to handle requests from the <see cref="LatencyMonitor"/>.
    /// </summary>
    public class LatencyPeer : PeerBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly GameApplication Application;

        #endregion

        #region Constructors and Destructors

        public LatencyPeer(GameApplication application, IRpcProtocol protocol, IPhotonPeer nativePeer)
            : base(protocol, nativePeer)
        {
            this.Application = application;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Latency monitoring client connected, serverId={0}", this.Application.ServerId);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Latency monitoring client disconnected: reason={0}, detail{1}", reasonCode, reasonDetail);
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                default:
                    {
                        string message = string.Format("Unknown operation code {0}", operationRequest.OperationCode);
                        this.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = message }, sendParameters);
                        break;
                    }

                case 1:
                    {
                        var pingOperation = new LatencyOperation(this.Protocol, operationRequest.Parameters);
                        if (pingOperation.IsValid == false)
                        {
                            this.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = pingOperation.GetErrorMessage() }, sendParameters);
                            return;
                        }

                        Thread.Sleep(5);

                        var response = new OperationResponse(operationRequest.OperationCode, pingOperation);
                        this.SendOperationResponse(response, sendParameters);
                        break;
                    }
            }
        }

        #endregion
    }
}
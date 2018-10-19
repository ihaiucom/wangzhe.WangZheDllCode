// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectedToGameServer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The connected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates.LoadBalancing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using ExitGames.Client.Photon;
    using log4net;
    using Photon.StarDust.Client.Connections;
    using Photon.StarDust.Client.ConnectionStates.Lite;

    /// <summary>
    /// The connected.
    /// </summary>
    internal class ConnectedToGameServer : ConnectionStateBase
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly ConnectedToGameServer Instance = new ConnectedToGameServer();

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Implemented Interfaces

        #region IConnectionState


        public override void EnterState(ClientConnection client)
        {
            this.OpAuthenticate(client); 
            
            Counters.ReliableOperationsSent.Increment();
            WindowsCounters.ReliableOperationsSent.Increment();
        }

        private void OpAuthenticate(ClientConnection client)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)LoadBalancingEnums.ParameterCode.ApplicationId, Settings.VirtualAppId }, 
                    { (byte)LoadBalancingEnums.ParameterCode.Secret, client.AuthenticationTicket }, 
                };

            client.Peer.OpCustom(
                (byte)LoadBalancingEnums.OperationCode.Authenticate, data, true, 0, Settings.UseEncryption);
        }

        public override void StopClient(ClientConnection client)
        {
            Counters.ConnectedClients.Decrement();
            WindowsCounters.ConnectedClients.Decrement();

            client.State = Disconnected.Instance; 
        }

        public override void TransitState(ClientConnection client)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("AuthenticatedOnGameServer");
            }

            client.State = AuthenticatedOnGameServer.Instance;
            client.State.EnterState(client);
        }
        
        public override void OnOperationReturn(ClientConnection client, OperationResponse operationResponse)
        {
            Counters.ReceivedOperationResponse.Increment();
            WindowsCounters.ReceivedOperationResponse.Increment();

            switch (operationResponse.OperationCode)
            {
                case (byte)LoadBalancingEnums.OperationCode.Authenticate: 
                    if (operationResponse.ReturnCode == 0)
                    {
                       this.TransitState(client);
                    }
                    else
                    {
                        log.WarnFormat("OnOperationReturn: Authenticate on GS failed: ReturnCode: {0} ({1}). Disconnecting...", operationResponse.ReturnCode, operationResponse.DebugMessage);
                        client.Peer.Disconnect();
                    }
                    break; 

                default:
                    {
                        log.WarnFormat("OnOperationReturn: unexpected return code {0} of operation {1}", operationResponse.ReturnCode, operationResponse.OperationCode);
                        break;
                    }
            }
        }

        /// <summary>
        /// The on peer status callback.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        public override void OnPeerStatusCallback(ClientConnection client, StatusCode returnCode)
        {
            switch (returnCode)
            {
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.TimeoutDisconnect:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("{0}", returnCode);
                        }

                        client.OnDisconnected();
                        break;
                    }

                default:
                    {
                        log.WarnFormat("Connected: OnPeerStatusCallback: unexpected return code {0}", returnCode);
                        break;
                    }
            }
        }

        /// <summary>
        /// The on update.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public override void OnUpdate(ClientConnection client)
        {
            client.PeerService();
            client.EnqueueUpdate();
        }

        #endregion

        #endregion
    }
}
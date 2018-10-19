// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisconnectingFromMaster.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The connected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates.LoadBalancing
{
    using System.Collections.Generic;
    using System.Reflection;
    using ExitGames.Client.Photon;
    using log4net;
    using Photon.StarDust.Client.Connections;
    using Photon.StarDust.Client.ConnectionStates.Lite;

    /// <summary>
    /// The connected.
    /// </summary>
    internal class DisconnectingFromMaster : ConnectionStateBase
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly DisconnectingFromMaster Instance = new DisconnectingFromMaster();

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Implemented Interfaces

        #region IConnectionState


        public override void EnterState(ClientConnection client)
        {
            client.Peer.Disconnect();
            
            Counters.ConnectedClients.Decrement();
            WindowsCounters.ConnectedClients.Decrement();
        }

        public override void TransitState(ClientConnection client)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Waiting for GameServer connect");
            }
               
            client.State = WaitingForGameServerConnect.Instance;
            client.State.EnterState(client); 
        }

        public override void StopClient(ClientConnection client)
        {
            Counters.ConnectedClients.Decrement();
            WindowsCounters.ConnectedClients.Decrement();

            client.State = Disconnected.Instance; 
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
                case StatusCode.Disconnect:
                    // this is expected! automatically reconnect to GS, don't call OnDisconnect. 
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Disconnected from Master.");
                    }
                    this.TransitState(client);
                    break; 

                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.TimeoutDisconnect:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("{0}", returnCode);
                        }

                        Counters.ConnectedClients.Decrement();
                        WindowsCounters.ConnectedClients.Decrement();

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

        public override void OnUpdate(ClientConnection client)
        {
            client.PeerService();
            client.EnqueueUpdate();
        }
        #endregion

        #endregion
    }
}
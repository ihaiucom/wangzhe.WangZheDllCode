// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitingForGameServerConnect.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates.LoadBalancing
{
    using System;
    using System.Reflection;

    using ExitGames.Client.Photon;

    using log4net;

    using Photon.StarDust.Client.Connections;

    public class WaitingForGameServerConnect : ConnectionStateBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static WaitingForGameServerConnect Instance = new WaitingForGameServerConnect();  

        public override void EnterState(ClientConnection client)
        {
            string gameServerAddress = ((LoadBalancingClientConnection)client).GameServerAddress; 
            
            if (client.Peer.PeerState != PeerStateValue.Disconnected)
            {
                log.WarnFormat("Could not connect to GS: Peer is not disconencted ({0}). Stopping client...", client.Peer.PeerState);
                client.Peer.Disconnect();
            }
            else
            {
                if (client.Peer.Connect(gameServerAddress, "Game"))
                {
                    log.InfoFormat("Connecting to " + gameServerAddress);
                    this.OnUpdate(client);
                }
                else
                {
                    throw new InvalidOperationException("connect failed to " + gameServerAddress);
                }
            }
        }

        public override void OnPeerStatusCallback(ClientConnection client, StatusCode returnCode)
        {
            switch (returnCode)
            {
                // this is expected; client automatically sends a connect afterwards. 
                case StatusCode.Connect:
                    {
                        client.OnConnected();
                        break;
                    }

                case StatusCode.EncryptionEstablished:
                    {
                        client.OnEncryptionEstablished();
                        break;
                    }

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
                        log.WarnFormat("Waiting: OnPeerStatusCallback: unexpected return code {0}", returnCode);
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

        public override void TransitState(ClientConnection client)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Connected to GameServer");
            }

            client.State = ConnectedToGameServer.Instance;

            Counters.ConnectedClients.Increment();
            WindowsCounters.ConnectedClients.Increment();

            client.State.EnterState(client);
        }
    }
}

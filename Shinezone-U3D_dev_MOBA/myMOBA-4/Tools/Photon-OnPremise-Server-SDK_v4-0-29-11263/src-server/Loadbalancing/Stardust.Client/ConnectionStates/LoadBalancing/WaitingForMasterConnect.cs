// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitingForConnect.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The waiting for connect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates.LoadBalancing
{
    using System.Reflection;

    using ExitGames.Client.Photon;

    using log4net;

    using Photon.StarDust.Client.Connections;

    internal class WaitingForMasterConnect : ConnectionStateBase
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly WaitingForMasterConnect Instance = new WaitingForMasterConnect();

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Implemented Interfaces

        #region IConnectionState
        public override void TransitState(ClientConnection client)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Connected to Master");
            }

            client.State = ConnectedToMaster.Instance;
            
            Counters.ConnectedClients.Increment();
            WindowsCounters.ConnectedClients.Increment();

            client.State.EnterState(client);
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

        #endregion

        #endregion
    }
}
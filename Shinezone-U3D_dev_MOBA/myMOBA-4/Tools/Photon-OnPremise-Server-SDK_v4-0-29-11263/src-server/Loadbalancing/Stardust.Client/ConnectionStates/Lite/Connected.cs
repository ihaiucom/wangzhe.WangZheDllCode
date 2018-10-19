// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connected.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The connected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon.LoadBalancing;

namespace Photon.StarDust.Client.ConnectionStates.Lite
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using ExitGames.Client.Photon;

    using log4net;

    using Photon.StarDust.Client.Connections;

    internal class Connected : ConnectionStateBase
    {
        public static readonly Connected Instance = new Connected();

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Implemented Interfaces

        #region IConnectionState

        public override void EnterState(ClientConnection client)
        {
             // join: 
            this.OpJoin(client);

            Counters.ReliableOperationsSent.Increment();
            WindowsCounters.ReliableOperationsSent.Increment();
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
                log.Debug("Playing");
            }

            client.State = Playing.Instance;

            client.StartTimers();
        }

        public override void OnOperationReturn(ClientConnection client, OperationResponse operationResponse)
        {
            Counters.ReceivedOperationResponse.Increment();
            WindowsCounters.ReceivedOperationResponse.Increment();

            switch (operationResponse.OperationCode)
            {
                case OperationCode.JoinGame:
                    {
                        if (operationResponse.ReturnCode == 0)
                        {
                            this.TransitState(client);
                        }
                        else
                        {
                            log.WarnFormat("OnOperationReturn: unexpected return code {0} of operation {1}", operationResponse.ReturnCode, operationResponse.OperationCode);
                            client.Stop();
                        }

                        break;
                    }

                default:
                    {
                        log.WarnFormat("OnOperationReturn: unexpected return code {0} of operation {1}", operationResponse.ReturnCode, operationResponse.OperationCode);
                        break;
                    }
            }
        }

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
                        if (log.IsInfoEnabled)
                        {
                            log.InfoFormat("{0}", returnCode);
                        }

                        Counters.ConnectedClients.Decrement();
                        WindowsCounters.ConnectedClients.Decrement();

                        client.State = Disconnected.Instance; 
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

        private void OpJoin(ClientConnection client)
        {
            var wrap = new Dictionary<byte, object> { { ParameterCode.RoomName, client.GameName } };
            client.Peer.OpCustom(OperationCode.JoinGame, wrap, true, 0, Settings.UseEncryption);
        }

        #endregion

        #endregion
    }
}
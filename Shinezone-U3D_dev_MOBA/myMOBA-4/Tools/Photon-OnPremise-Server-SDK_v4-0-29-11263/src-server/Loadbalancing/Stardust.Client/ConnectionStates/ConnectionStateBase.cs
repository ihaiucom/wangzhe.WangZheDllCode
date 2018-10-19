// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStateBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates
{
    using System;
    using System.Reflection;

    using ExitGames.Client.Photon;

    using log4net;

    using Photon.StarDust.Client.Connections;

    public class ConnectionStateBase : IConnectionState
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public virtual void OnOperationReturn(ClientConnection client, OperationResponse operationResponse)
        {
            log.WarnFormat("OnOperationReturn: unexpected return code {0} of operation {1}", operationResponse.ReturnCode, operationResponse.OperationCode);
        }

        public virtual void OnPeerStatusCallback(ClientConnection client, StatusCode returnCode)
        {
            log.WarnFormat(
                "{1}: OnPeerStatusCallback - unexpected return code {0}", returnCode, client.GetHashCode());
        }


        public virtual void OnUpdate(ClientConnection client)
        {

        }

        public virtual void StopClient(ClientConnection client)
        {

        }

        public virtual void EnterState(ClientConnection client)
        {

        }

        public virtual void TransitState(ClientConnection client)
        {

        }
    }
}

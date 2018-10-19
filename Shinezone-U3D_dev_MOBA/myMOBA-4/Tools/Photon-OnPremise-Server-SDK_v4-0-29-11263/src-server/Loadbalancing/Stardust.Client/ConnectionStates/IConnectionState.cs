// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The i connection state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates
{
    using ExitGames.Client.Photon;

    using Photon.StarDust.Client.Connections;

    public interface IConnectionState
    {
        void OnOperationReturn(ClientConnection client, OperationResponse operationResponse);

        void OnPeerStatusCallback(ClientConnection client, StatusCode returnCode);

        void OnUpdate(ClientConnection client);

        ///<summary>
        /// Tear down the application and stop all clients. 
        /// </summary>
        void StopClient(ClientConnection client);
        

        /// <summary>
        /// Called to enter a certain state. 
        /// </summary>
        void EnterState(ClientConnection client); 

        /// <summary>
        /// Switch to the next logical state (in case of success) 
        /// </summary>
        void TransitState(ClientConnection client); 
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Codes of operations (defining their type, parameters incoming from clients and return values).
//   These codes match events (in parts).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    /// <summary>
    ///   Defines the operation codes used by the Lite application.
    ///   These codes match events (in parts).
    /// </summary>
    public enum OperationCode : byte
    {
        /// <summary>
        ///   The operation code for the <see cref="JoinGameRequest">join</see> operation.
        /// </summary>
        Join = 255, 

        /// <summary>
        ///   Operation code for the <see cref="LeaveRequest">leave</see> operation.
        /// </summary>
        Leave = 254, 

        /// <summary>
        ///   Operation code for the <see cref="RaiseEventRequest">raise event</see> operation.
        /// </summary>
        RaiseEvent = 253, 

        /// <summary>
        ///   Operation code for the <see cref="SetPropertiesRequest">set properties</see> operation.
        /// </summary>
        SetProperties = 252, 

        /// <summary>
        ///   Operation code for the <see cref="GetPropertiesRequest">get properties</see> operation.
        /// </summary>
        GetProperties = 251, 
        
        /// <summary>
        ///   Operation code for the ping operation.
        /// </summary>
        Ping = 249,

        /// <summary>
        ///   Operation code for the <see cref="ChangeGroups" /> operation.
        /// </summary>
        ChangeGroups = 248,

        // operation codes in load the balancing project
        Authenticate = 230,
        JoinLobby = 229,
        LeaveLobby = 228,
        CreateGame = 227,
        JoinGame = 226,
        JoinRandomGame = 225,

        // CancelJoinRandomGame = 224, currently not used 
        DebugGame = 223,
        FindFriends = 222,
        LobbyStats = 221,

        // Rpc call to external server
        Rpc = 219,
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    public enum OperationCode : byte
    {
        // operation codes inherited from lite
        //Join = 255 (Join is not used the in load balancing project)
        Leave = Photon.Hive.Operations.OperationCode.Leave,
        RaiseEvent = Photon.Hive.Operations.OperationCode.RaiseEvent,
        SetProperties = Photon.Hive.Operations.OperationCode.SetProperties,
        GetProperties = Photon.Hive.Operations.OperationCode.GetProperties,
        Ping = Photon.Hive.Operations.OperationCode.Ping,
        ChangeGroups = Photon.Hive.Operations.OperationCode.ChangeGroups,

        // operation codes in load the balancing project
        Authenticate = Photon.Hive.Operations.OperationCode.Authenticate, 
        JoinLobby = Photon.Hive.Operations.OperationCode.JoinLobby, 
        LeaveLobby = Photon.Hive.Operations.OperationCode.LeaveLobby, 
        CreateGame = Photon.Hive.Operations.OperationCode.CreateGame, 
        JoinGame = Photon.Hive.Operations.OperationCode.JoinGame, 
        JoinRandomGame = Photon.Hive.Operations.OperationCode.JoinRandomGame, 
        // CancelJoinRandomGame = 224, currently not used 
        DebugGame = Photon.Hive.Operations.OperationCode.DebugGame,
        FindFriends = Photon.Hive.Operations.OperationCode.FindFriends,
        LobbyStats = Photon.Hive.Operations.OperationCode.LobbyStats,
        Rpc = Photon.Hive.Operations.OperationCode.Rpc,
    }
}
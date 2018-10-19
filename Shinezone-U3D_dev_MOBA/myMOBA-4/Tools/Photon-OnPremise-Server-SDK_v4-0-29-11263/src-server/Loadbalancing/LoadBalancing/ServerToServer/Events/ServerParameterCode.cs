// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerParameterCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerParameterCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Events
{
    public enum ServerParameterCode
    {
        UdpAddress = 10, 
        TcpAddress = 11, 

        PeerCount = 20, 
        GameCount = 21, 
        LoadIndex = 22, 
        ServerState = 23,
        MaxPlayer = 24,
        IsOpen = 25,
        IsVisible = 26,
        LobbyPropertyFilter = 27,
        InactiveCount = 28,
        IsPersistent = 29,

        AuthList = 30, 

        NewUsers = 40,
        RemovedUsers = 41,
        Reinitialize = 42,
        CheckUserIdOnJoin = 43,
        InactiveUsers = 44,
        ExcludedUsers = 45,
        FailedToAdd = 46,
        ExpectedUsers = 47,
    }
}
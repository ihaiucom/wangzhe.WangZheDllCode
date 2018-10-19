// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerEventCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Events
{
    public enum ServerEventCode
    {
        ////InitInstance = 0, 
        UpdateServer = 1, 
        UpdateAppStats = 2, 
        UpdateGameState = 3, 
        RemoveGameState = 4, 
        AuthenticateUpdate = 10
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLobbyType.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Common.Lobby
{
    public enum AppLobbyType
    {
        /// <summary> (0) </summary>
        Default = 0, 

        /// <summary> (1) </summary>
        ChannelLobby = 1,

        /// <summary> (2) </summary>
        SqlLobby = 2, 

        /// <summary> (3) </summary>
        AsyncRandomLobby = 3,
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILobbyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ILobbyPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Net;

    using Photon.SocketServer;

    public interface ILobbyPeer
    {
        NetworkProtocolType NetworkProtocol { get; }

        IPAddress LocalIPAddress { get; }

        int LocalPort { get; }

        string UserId { get; }

        bool UseHostnames { get; }
    }
}
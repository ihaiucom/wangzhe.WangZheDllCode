// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Stores properties of peers which have joined a game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;

    #endregion

    /// <summary>
    /// Stores properties of peers which have joined a game.
    /// </summary>
    /// <remarks>
    /// To avoid storing a reference to a peer object this class creates a copy of the 
    /// peers UserId and BlockedUsers properties.
    /// This properties are needed for match making even if the peer has disconnected to
    /// join the game on the game server instance.
    /// if a reference to the peer object would be stored the garbage collector cannot 
    /// remove the peer object from memory and references to the native socket server
    /// objects cannot be disposed.
    /// </remarks>
    public class PeerState
    {
        public readonly DateTime UtcCreated;

        public readonly string UserId;

        public PeerState(string userId)
        {
            this.UtcCreated = DateTime.UtcNow;
            this.UserId = userId;
        }

        public PeerState(ILobbyPeer peer)
        {
            this.UserId = peer.UserId ?? string.Empty;
            this.UtcCreated = DateTime.UtcNow;
        }
    }
}
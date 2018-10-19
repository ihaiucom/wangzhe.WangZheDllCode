// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameServerCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameServerCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.GameServer
{
    #region using directives

    using System;
    using System.Collections.Generic;

    #endregion

    public class GameServerCollection : Dictionary<string, IncomingGameServerPeer>
    {
        #region Constants and Fields

        private readonly object syncRoot = new object();

        #endregion

        #region Public Methods

        public void OnConnect(IncomingGameServerPeer gameServerPeer)
        {
            if (!gameServerPeer.ServerId.HasValue)
            {
                throw new InvalidOperationException("server id cannot be null");
            }

            //Guid id = gameServerPeer.ServerId.Value;
            string key = gameServerPeer.Key;

            lock (this.syncRoot)
            {
                IncomingGameServerPeer peer;
                if (this.TryGetValue(key, out peer))
                {
                    if (gameServerPeer != peer)
                    {
                        peer.Disconnect();
                        peer.RemoveGameServerPeerOnMaster();
                        this.Remove(key);
                    }
                }

                this.Add(key, gameServerPeer);
            }
        }

        public void OnDisconnect(IncomingGameServerPeer gameServerPeer)
        {
            if (!gameServerPeer.ServerId.HasValue)
            {
                throw new InvalidOperationException("server id cannot be null");
            }

            //Guid id = gameServerPeer.ServerId.Value;
            string key = gameServerPeer.Key; 

            lock (this.syncRoot)
            {
                IncomingGameServerPeer peer;
                if (this.TryGetValue(key, out peer))
                {
                    if (peer == gameServerPeer)
                    {
                        this.Remove(key);
                    }
                }
            }
        }

        #endregion
    }
}
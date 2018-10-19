// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteClientConnection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.Connections
{
    using System;

    using Photon.StarDust.Client.ConnectionStates.Lite;

    internal class LiteClientConnection : ClientConnection
    {
        public LiteClientConnection(string gameName, int number)
            : base(gameName, number)
        {
        }

        public override void Start()
        {
            if (this.Peer.Connect(Settings.ServerAddress, Settings.PhotonApplication))
            {
                this.State = WaitingForConnect.Instance;
                this.EnqueueUpdate();
            }
            else
            {
                throw new InvalidOperationException("connect failed");
            }
        }
    }
}

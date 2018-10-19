// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancingClientConnection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.Connections
{
    using System;
    using Photon.StarDust.Client.ConnectionStates.LoadBalancing;

    internal class LoadBalancingClientConnection : ClientConnection
    {
        public string GameServerAddress { get; set; }

        public LoadBalancingClientConnection(string gameName, int number)
            : base(gameName, number)
        {
        }

        public override void OnDisconnected()
        {

        }

        public override void Start()
        {
            if (this.Peer.Connect(Settings.ServerAddress, Settings.PhotonApplication))
            {
                this.State = WaitingForMasterConnect.Instance;
                this.EnqueueUpdate();
            }
            else
            {
                throw new InvalidOperationException("connect failed");
            }
        }

        public override void OnEvent(ExitGames.Client.Photon.EventData eventData)
        {
            if (Enum.IsDefined(typeof(LoadBalancingEnums.EventCode), eventData.Code))
            {
                Counters.ReliableEventsReceived.Increment();
                WindowsCounters.ReliableEventsReceived.Increment();
            }

            else
            {
                base.OnEvent(eventData);    
            }
        }
    }
}

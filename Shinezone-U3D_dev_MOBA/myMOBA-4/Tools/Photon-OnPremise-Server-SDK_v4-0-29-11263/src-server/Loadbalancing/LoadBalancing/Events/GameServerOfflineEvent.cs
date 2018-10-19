// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameServerOfflineEvent.cs" company="">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    using Photon.SocketServer.Rpc;

    public class GameServerOfflineEvent : DataContract
    {
        [DataMember(Code = 0, IsOptional = true)]
        public int TimeLeft { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppStatsEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AppStatsEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    #region using directives

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer.Rpc;

    #endregion

    public class AppStatsEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.MasterPeerCount)]
        public int MasterPeerCount { get; set; }

        [DataMember(Code = (byte)ParameterCode.PeerCount)]
        public int PlayerCount { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameCount)]
        public int GameCount { get; set; }
    }
}
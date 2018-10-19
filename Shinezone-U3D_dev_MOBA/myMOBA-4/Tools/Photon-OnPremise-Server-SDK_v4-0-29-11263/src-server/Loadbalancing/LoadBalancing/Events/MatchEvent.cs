// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MatchEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    #region using directives

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer.Rpc;

    #endregion

    public class MatchEvent 
    {
        [DataMember(Code = (byte)ParameterCode.Address)]
        public string ServerAddress { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameId)]
        public string GameId { get; set; }
    }
}
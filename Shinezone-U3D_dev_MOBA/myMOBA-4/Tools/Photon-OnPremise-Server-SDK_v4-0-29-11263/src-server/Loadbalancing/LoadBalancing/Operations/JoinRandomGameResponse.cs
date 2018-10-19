// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRandomGameResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinRandomGameResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    #region

    using Photon.SocketServer.Rpc;

    #endregion

    public class JoinRandomGameResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterCode.Address)]
        public string Address { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameId)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)ParameterCode.NodeId)]
        public byte NodeId { get; set; }


        #endregion
    }
}
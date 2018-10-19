// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectRepeatResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RedirectRepeatResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    using Photon.SocketServer.Rpc;

    public class RedirectRepeatResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterCode.Address)]
        public string Address { get; set; }

        [DataMember(Code = (byte)ParameterCode.NodeId)]
        public byte NodeId { get; set; }

        #endregion
    }
}
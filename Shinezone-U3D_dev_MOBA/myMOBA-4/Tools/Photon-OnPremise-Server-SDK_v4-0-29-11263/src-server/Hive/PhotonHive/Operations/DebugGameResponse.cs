// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugGameResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using Photon.SocketServer.Rpc;

    public class DebugGameResponse
    {
        [DataMember(Code = (byte)ParameterKey.Address, IsOptional = true)]
        public string Address { get; set; }

        [DataMember(Code = (byte)ParameterKey.NodeId, IsOptional = true)]
        public byte NodeId { get; set; }

        [DataMember(Code = (byte)ParameterKey.Info)]
        public string Info { get; set; }
    }
}

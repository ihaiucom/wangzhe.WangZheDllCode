// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class DebugGameRequest : Operation
    {
        public DebugGameRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        public DebugGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = false)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)ParameterKey.Info, IsOptional = true)]
        public string Info { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Hive.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Photon.Hive.WebRpc
{
    public class WebRpcRequest : Operation
    {
        public WebRpcRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterKey.RpcCallParams, IsOptional = true)]
        public object RpcParams { get; set; }

        [DataMember(Code = (byte)ParameterKey.UriPath, IsOptional = false)]
        public string UriPath { get; set; }

        [DataMember(Code = (byte)ParameterKey.WebFlags, IsOptional = true)]
        public byte WebFlags { get; set; }
    }
}

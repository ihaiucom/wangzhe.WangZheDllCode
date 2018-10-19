// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscribeCounter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation subscribes to the PhotonApplication.CounterPublisher. It can be executed any time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The operation subscribes to the PhotonApplication.CounterPublisher. It can be executed any time.
    /// </summary>
    public class SubscribeCounter : Operation
    {
        public SubscribeCounter(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.CounterReceiveInterval)]
        public int ReceiveInterval { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }
}
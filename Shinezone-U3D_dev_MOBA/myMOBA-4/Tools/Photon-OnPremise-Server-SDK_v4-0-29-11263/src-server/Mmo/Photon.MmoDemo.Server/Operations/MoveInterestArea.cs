// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveInterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation moves an InterestArea. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The operation moves an InterestArea. 
    /// </summary>
    /// <remarks>
    /// This operation is allowed AFTER having entered a World with operation EnterWorld.
    /// </remarks>
    public class MoveInterestArea : Operation
    {
        public MoveInterestArea(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        // Default: #0.
        [DataMember(Code = (byte)ParameterCode.InterestAreaId, IsOptional = true)]
        public byte InterestAreaId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public Vector Position { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new MoveInterestAreaResponse { InterestAreaId = this.InterestAreaId };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class MoveInterestAreaResponse
    {
        // Default: #0.
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }
    }
}
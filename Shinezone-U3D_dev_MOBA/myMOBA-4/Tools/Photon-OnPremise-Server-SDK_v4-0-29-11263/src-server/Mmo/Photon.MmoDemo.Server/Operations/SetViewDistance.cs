// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetViewDistance.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The oeration sets view distsance for InterstArea.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The oeration sets view distsance for InterstArea.
    /// </summary>
    /// <remarks>
    /// This operation is allowed AFTER having entered a World with operation EnterWorld.
    /// </remarks>
    public class SetViewDistance : Operation
    {
        public SetViewDistance(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        // If not submitted the default interest area #0 is selected.
        [DataMember(Code = (byte)ParameterCode.InterestAreaId, IsOptional = true)]
        public byte InterestAreaId { get; set; }

        // Gets or sets the interest area's minimum view distance (the item subscribe threshold).
        [DataMember(Code = (byte)ParameterCode.ViewDistanceEnter)]
        public Vector ViewDistanceEnter { get; set; }

        // Gets or sets the interest area's maximum view distance (the item unsubscribe threshold).
        [DataMember(Code = (byte)ParameterCode.ViewDistanceExit)]
        public Vector ViewDistanceExit { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new SetViewDistanceResponse { InterestAreaId = this.InterestAreaId };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class SetViewDistanceResponse
    {
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }
    }
}
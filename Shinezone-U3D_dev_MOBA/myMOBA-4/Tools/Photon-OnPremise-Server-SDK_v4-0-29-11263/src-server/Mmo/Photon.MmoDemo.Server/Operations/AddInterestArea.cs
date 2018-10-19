// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddInterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation adds client's InterestArea.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The operation adds client's InterestArea.
    /// </summary>
    /// <remarks>
    /// This operation is allowed AFTER having entered a World with operation EnterWorld.
    /// </remarks>
    public class AddInterestArea : Operation
    {
        public AddInterestArea(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }

        // The id of an Item the new InterestArea should be attached to.
        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position, IsOptional = true)]
        public Vector Position { get; set; }

        // Gets or sets the interest area's minimum view distance (the region subscribe threshold).
        [DataMember(Code = (byte)ParameterCode.ViewDistanceEnter)]
        public Vector ViewDistanceEnter { get; set; }

        // Gets or sets the interest area's maximum view distance (the region unsubscribe threshold).
        [DataMember(Code = (byte)ParameterCode.ViewDistanceExit)]
        public Vector ViewDistanceExit { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new AddInterestAreaResponse { InterestAreaId = this.InterestAreaId };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }
 
    public class AddInterestAreaResponse
    {
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }
    }
}
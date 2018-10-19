// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetProperties.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation sets the properties of an Item. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using System.Collections;

    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The operation sets the properties of an Item. 
    /// </summary>
    /// <remarks>
    /// This operation is allowed AFTER having entered a World with operation EnterWorld.
    /// </remarks>
    public class SetProperties : Operation
    {
        public SetProperties(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        // If not submitted the MmoActor.Avatar is selected.
        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesSet, IsOptional = true)]
        public Hashtable PropertiesSet { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesUnset, IsOptional = true)]
        public ArrayList PropertiesUnset { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new SetPropertiesResponse { ItemId = this.ItemId };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class SetPropertiesResponse
    {
        // If not submitted the MmoActor.Avatar
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }
    }
}
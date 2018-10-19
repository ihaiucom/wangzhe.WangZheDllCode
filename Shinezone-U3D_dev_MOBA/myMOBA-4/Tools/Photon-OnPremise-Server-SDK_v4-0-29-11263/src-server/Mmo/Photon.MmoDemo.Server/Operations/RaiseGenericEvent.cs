// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaiseGenericEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation raises generic item event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Events;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This operation raises generic item event.
    /// </summary>
    public class RaiseGenericEvent : Operation
    {
        public RaiseGenericEvent(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        /// <summary>
        /// Gets or sets the custom event code.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.CustomEventCode)]
        public byte CustomEventCode { get; set; }

        /// <summary>
        /// Gets or sets the optional event content.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.EventData, IsOptional = true)]
        public object EventData { get; set; }

        [DataMember(Code = (byte)ParameterCode.EventReceiver)]
        public byte EventReceiver { get; set; }

        [DataMember(Code = (byte)ParameterCode.EventReliability)]
        public byte EventReliability { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true)]
        public string ItemId { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new RaiseGenericEventResponse { ItemId = this.ItemId };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class RaiseGenericEventResponse
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }
    }

}
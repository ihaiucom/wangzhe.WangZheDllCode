// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadarSubscribe.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation subscribes to Radar. It can be executed any time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This operation subscribes to Radar. It can be executed any time.
    /// </summary>
    public class RadarSubscribe : Operation
    {
        public RadarSubscribe(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new RadarSubscribeResponse { WorldName = this.WorldName };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class RadarSubscribeResponse
    {
        [DataMember(Code = (byte)ParameterCode.BoundingBox, IsOptional = true)]
        public BoundingBox BoundingBox { get; set; }

        [DataMember(Code = (byte)ParameterCode.TileDimensions, IsOptional = true)]
        public Vector TileDimensions { get; set; }

        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }
    }
}
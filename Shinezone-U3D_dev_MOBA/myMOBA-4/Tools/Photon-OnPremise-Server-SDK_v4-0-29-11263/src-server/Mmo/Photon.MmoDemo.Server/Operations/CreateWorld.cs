// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateWorld.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation creates new world.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Operations
{
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The operation creates new world.
    /// </summary>
    /// <remarks>
    /// This operation is allowed BEFORE having entered a World with operation EnterWorld.
    /// </remarks>
    public class CreateWorld : Operation
    {
        public CreateWorld(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.TileDimensions)]
        public Vector TileDimensions { get; set; }

        [DataMember(Code = (byte)ParameterCode.BoundingBox)]
        public BoundingBox BoundingBox { get; set; }

        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new CreateWorldResponse { WorldName = this.WorldName };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class CreateWorldResponse
    {
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }
    }
}
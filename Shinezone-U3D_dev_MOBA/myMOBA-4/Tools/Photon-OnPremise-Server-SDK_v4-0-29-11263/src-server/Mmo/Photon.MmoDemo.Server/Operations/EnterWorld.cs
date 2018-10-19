// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterWorld.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operation enters client into world.
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
    /// The operation enters client into world.
    /// </summary>
    /// <remarks>
    /// This operation is allowed BEFORE having entered an World with operation EnterWorld or AFTER having exited it with operaiton ExitWorld.
    /// </remarks>
    public class EnterWorld : Operation
    {
        public EnterWorld(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        // Gets or sets the id of the initial InterestArea.
        [DataMember(Code = (byte)ParameterCode.InterestAreaId, IsOptional = true)]
        public byte InterestAreaId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public Vector Position { get; set; }

        // Gets or sets the MmoActor.Avatar's initial properties.
        [DataMember(Code = (byte)ParameterCode.Properties, IsOptional = true)]
        public Hashtable Properties { get; set; }

        [DataMember(Code = (byte)ParameterCode.Rotation, IsOptional = true)]
        public Vector Rotation { get; set; }

        // Gets or sets the client's username. This will be the MmoActor.Avatar's Item Id.
        [DataMember(Code = (byte)ParameterCode.Username)]
        public string Username { get; set; }

        // Gets or sets the inner view distance (item subscribe threshold) of the initial InterestArea.
        [DataMember(Code = (byte)ParameterCode.ViewDistanceEnter)]
        public Vector ViewDistanceEnter { get; set; }

        // Gets or sets the outer view distance (item unsubscribe threshold) of the initial InterestArea.
        [DataMember(Code = (byte)ParameterCode.ViewDistanceExit)]
        public Vector ViewDistanceExit { get; set; }

        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new EnterWorldResponse { WorldName = this.WorldName };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }

    public class EnterWorldResponse
    {
        [DataMember(Code = (byte)ParameterCode.BoundingBox, IsOptional = true)]
        public BoundingBox BoundingBox { get; set; }

        [DataMember(Code = (byte)ParameterCode.TileDimensions, IsOptional = true)]
        public Vector TileDimensions { get; set; }

        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }
    }
}
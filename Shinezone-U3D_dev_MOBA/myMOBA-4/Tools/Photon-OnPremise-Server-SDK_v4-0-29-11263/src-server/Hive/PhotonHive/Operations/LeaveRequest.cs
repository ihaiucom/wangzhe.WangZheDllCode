// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeaveRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implements the Leave operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using Photon.Hive.Plugin;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Implements the Leave operation.
    /// </summary>
    public class LeaveRequest : Operation, ILeaveGameRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaveRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public LeaveRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public LeaveRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.IsInactive, IsOptional = true)]
        public bool IsCommingBack { get; set; }

        [DataMember(Code = (byte)ParameterKey.WebFlags, IsOptional = true)]
        public byte WebFlags { get; set; }

        public byte OperationCode
        {
            get { return this.OperationRequest.OperationCode; }
        }

        public System.Collections.Generic.Dictionary<byte, object> Parameters
        {
            get { return this.OperationRequest.Parameters; }
        }
    }
}
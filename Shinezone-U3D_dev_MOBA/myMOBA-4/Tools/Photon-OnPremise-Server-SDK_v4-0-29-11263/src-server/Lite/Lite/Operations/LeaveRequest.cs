// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeaveRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implements the Leave operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Implements the Leave operation.
    /// </summary>
    public class LeaveRequest : Operation
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
    }
}
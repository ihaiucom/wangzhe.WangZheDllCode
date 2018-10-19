// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetPropertiesRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The set properties operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The set properties operation.
    /// </summary>
    public class SetPropertiesRequest : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertiesRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public SetPropertiesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertiesRequest"/> class.
        /// </summary>
        public SetPropertiesRequest()
        {
        }

        /// <summary>
        /// Gets or sets ActorNumber.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Broadcast.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        public bool Broadcast { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Properties)]
        public Hashtable Properties { get; set; }
    }
}
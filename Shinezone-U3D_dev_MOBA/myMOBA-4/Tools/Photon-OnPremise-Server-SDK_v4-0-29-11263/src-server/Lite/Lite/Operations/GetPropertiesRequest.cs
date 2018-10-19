// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPropertiesRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The get properties request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    ///   The get properties request.
    /// </summary>
    public class GetPropertiesRequest : Operation
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "GetPropertiesRequest" /> class.
        /// </summary>
        /// <param name = "protocol">
        ///   The protocol.
        /// </param>
        /// <param name = "operationRequest">
        ///   Operation request containing the operation parameters.
        /// </param>
        public GetPropertiesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "GetPropertiesRequest" /> class.
        /// </summary>
        public GetPropertiesRequest()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the actor numbers for which to get the properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Actors, IsOptional = true)]
        public int[] ActorNumbers { get; set; }

        /// <summary>
        ///   Gets or sets ActorPropertyKeys.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public IList ActorPropertyKeys { get; set; }

        /// <summary>
        ///   Gets or sets GamePropertyKeys.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public IList GamePropertyKeys { get; set; }

        /// <summary>
        ///   Gets or sets PropertyType.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Properties, IsOptional = true)]
        public byte PropertyType { get; set; }

        #endregion
    }
}
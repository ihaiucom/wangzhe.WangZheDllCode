// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaiseEventRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implements the RaiseEvent operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using Lite.Caching;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    ///   Implements the RaiseEvent operation.
    /// </summary>
    public class RaiseEventRequest : Operation
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaiseEventRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public RaiseEventRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RaiseEventRequest"/> class.
        /// </summary>
        public RaiseEventRequest()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the actors which should receive the event.
        ///   If set to null or an empty array the event will be sent
        ///   to all actors in the room.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Actors, IsOptional = true)]
        public int[] Actors { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating how to use the <see cref = "EventCache" />.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if the event is sent to individual actors (submitted <see cref = "Actors" /> or <see cref = "Lite.Operations.ReceiverGroup.MasterClient" />).
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Cache, IsOptional = true)]
        public byte Cache { get; set; }

        /// <summary>
        ///   Gets or sets the hashtable containing the data to send.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public object Data { get; set; }

        /// <summary>
        ///   Gets or sets a byte containing the Code to send.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Code, IsOptional = true)]
        public byte EvCode { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether to flush the send queue.
        ///   Flushing the send queue will override the configured photon send delay.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Flush, IsOptional = true)]
        public bool Flush { get; set; }

        /// <summary>
        ///   Gets or sets the game id.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public string GameId { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref = "Lite.Operations.ReceiverGroup" /> for the event.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if <see cref = "Actors" /> are set.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.ReceiverGroup, IsOptional = true)]
        public byte ReceiverGroup { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref = "Lite.Operations.ReceiverGroup" /> for the event.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if <see cref = "Actors" /> are set.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Group, IsOptional = true)]
        public byte Group { get; set; }
        
        #endregion
    }
}
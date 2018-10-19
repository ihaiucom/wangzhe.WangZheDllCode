// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This class implements the Join operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using System.Collections;

    using Lite.Events;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class JoinRequest : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public JoinRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        public JoinRequest()
        {
        }

        /// <summary>
        /// Gets or sets custom actor properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the actor properties
        /// should be included in the <see cref="JoinEvent"/> event which 
        /// will be sent to all clients currently in the room.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        public bool BroadcastActorProperties { get; set; }

        /// <summary>
        /// Gets or sets the name of the game (room).
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GameId)]
        public string GameId { get; set; }

        /// <summary>
        /// Gets or sets custom game properties.
        /// </summary>
        /// <remarks>
        /// Game properties will only be applied for the game creator.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cached events are automaticly deleted for 
        /// actors which are leaving a room.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.DeleteCacheOnLeave, IsOptional = true)]
        public bool DeleteCacheOnLeave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if common room events (Join, Leave) will be suppressed.
        /// </summary>
        /// <remarks>
        /// This property will only be applied for the game creator.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.SuppressRoomEvents, IsOptional = true)]
        public bool SuppressRoomEvents { get; set; }

        /// <summary>
        /// Actor number, which will be used for rejoin
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNr { get; set; }
    }
}
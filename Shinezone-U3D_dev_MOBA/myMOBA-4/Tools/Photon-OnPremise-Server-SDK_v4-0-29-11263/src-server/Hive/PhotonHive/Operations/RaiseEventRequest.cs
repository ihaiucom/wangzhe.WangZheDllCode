// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaiseEventRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implements the RaiseEvent operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;

namespace Photon.Hive.Operations
{
    using Photon.Hive.Caching;
    using Photon.Hive.Plugin;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    ///   Implements the RaiseEvent operation.
    /// </summary>
    public class RaiseEventRequest : Operation, IRaiseEventRequest
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
        ///   Ignored if the event is sent to individual actors (submitted <see cref = "Actors" /> or <see cref = "Photon.Hive.Operations.ReceiverGroup.MasterClient" />).
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
        ///   Gets or sets the <see cref = "Photon.Hive.Operations.ReceiverGroup" /> for the event.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if <see cref = "Actors" /> are set.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.ReceiverGroup, IsOptional = true)]
        public byte ReceiverGroup { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref = "Photon.Hive.Operations.ReceiverGroup" /> for the event.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if <see cref = "Actors" /> are set.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Group, IsOptional = true)]
        public byte Group { get; set; }

        private byte webFlags;
        [DataMember(Code = (byte)ParameterKey.WebFlags, IsOptional = true)]
        public object internalWebFlags 
        {
            get { return this.webFlags; }
            set
            {
                if (value is bool)
                {
                    var bvalue = (bool)value;
                    webFlags = bvalue ? Plugin.WebFlags.HttpForward : (byte)0x0;
                    return;
                }
                if (value is byte)
                {
                    webFlags = (byte)value;
                    return;
                }
                throw new InvalidCastException("Property 'WebFlags' does not accept other types then 'bool' and 'byte'");
            } 
        }

        [DataMember(Code = (byte)ParameterKey.CacheSliceIndex, IsOptional = true)]
        public int? CacheSliceIndex { get; set; }

        public bool IsCacheOpRemoveFromCache { get { return this.Cache == (byte)CacheOperation.RemoveFromRoomCache; } }

        public bool IsCacheOpRemoveFromCacheForActorsLeft { get { return this.Cache == (byte)CacheOperation.RemoveFromCacheForActorsLeft; } }

        public bool IsCacheSliceIndexOperation { get { return this.Cache >= (byte)CacheOperation.SliceIncreaseIndex; } }

        public bool IsCacheOnlyOperation
        {
            get
            {
                return this.IsCacheSliceIndexOperation
                    || this.IsCacheOpRemoveFromCache
                    || this.IsCacheOpRemoveFromCacheForActorsLeft;
            }
        }

        public bool IsBroadcastOperation { get { return !this.IsCacheOnlyOperation; } }

        public bool HttpForward
        {
            get { return Plugin.WebFlags.ShouldHttpForward(this.webFlags); }
            set { this.webFlags = value ? Plugin.WebFlags.HttpForward : (byte)0x0; }
        }

        public byte WebFlags { get { return this.webFlags; } set { this.webFlags = value; } }

        #endregion

        public byte OperationCode
        {
            get { return this.OperationRequest.OperationCode; }
        }

        public System.Collections.Generic.Dictionary<byte, object> Parameters
        {
            get { return this.OperationRequest.Parameters; }
        }

        public string DumpRequest()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat("RaiseEventRequest dump:")
                .AppendFormat("Actors: {0}; Cache: {1}; CacheSliceIndex: {2};", this.DumpActorsList(), this.Cache, this.CacheSliceIndex)
                .AppendFormat("Data: {0};", this.Data != null ? "NOT null" : "null")
                .AppendFormat("EventCode: {0}; Flush: {1}; GameId: {2};", this.EvCode, this.Flush, this.GameId)
                .AppendFormat("Group: {0}; HttpForward: {1}; ReceiverGroup: {2}; ", this.Group, this.HttpForward, this.ReceiverGroup)
                .AppendFormat("WebFlags: {0};", this.webFlags);

            return stringBuilder.ToString();
        }

        private string DumpActorsList()
        {
            if (this.Actors == null)
            {
                return "null";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var actor in this.Actors)
            {
                stringBuilder.AppendFormat("{0}, ", actor);
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
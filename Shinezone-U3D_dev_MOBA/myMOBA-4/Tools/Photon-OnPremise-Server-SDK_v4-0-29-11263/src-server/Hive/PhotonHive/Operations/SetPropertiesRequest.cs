// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetPropertiesRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The set properties operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Photon.Hive.Operations
{
    using System.Collections;

    using Photon.Hive.Common;
    using Photon.Hive.Plugin;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;

    /// <summary>
    /// The set properties operation.
    /// </summary>
    public class SetPropertiesRequest : Operation, ISetPropertiesRequest
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
            if (!this.IsValid)
            {
                return;
            }

            // special handling for game and actor properties send by AS3/Flash (Amf3 protocol) or JSON clients
            if (protocol.ProtocolType == ProtocolType.Amf3V16 || protocol.ProtocolType == ProtocolType.Json)
            {
                if (this.UpdatingGameProperties)
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(this.Properties, null);
                    Utilities.ConvertAs3WellKnownPropertyKeys(this.ExpectedValues, null);
                }
                else
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(null, this.Properties);
                    Utilities.ConvertAs3WellKnownPropertyKeys(null, this.ExpectedValues);
                }
            }

            if (this.UpdatingGameProperties)
            {
                this.isValid = GameParameterReader.TryGetProperties(this.Properties, out this.newMaxPlayer,
                    out this.newIsOpen, out this.newIsVisible, 
                    out this.newLobbyProperties, out this.MasterClientId, 
                    out this.ExpectedUsers, out this.errorMessage);
            }
        }

        public SetPropertiesRequest(int actorNr, Hashtable properties, Hashtable expected, bool broadcast)
        {
            this.ActorNumber = actorNr;
            this.Properties = properties;
            this.ExpectedValues = expected;
            this.Broadcast = broadcast;

            if (this.UpdatingGameProperties)
            {
                this.isValid = GameParameterReader.TryGetProperties(this.Properties, out this.newMaxPlayer,
                    out this.newIsOpen, out this.newIsVisible, out this.newLobbyProperties, 
                    out this.MasterClientId, out this.ExpectedUsers, out this.errorMessage);
            }
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

        /// <summary>
        /// Expected values for properties, which we are going to set
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ExpectedValues, IsOptional = true)]
        public Hashtable ExpectedValues { get; set; }

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

        public bool HttpForward
        {
            get { return Plugin.WebFlags.ShouldHttpForward(this.webFlags); }
            set { this.webFlags = value ? Plugin.WebFlags.HttpForward : (byte)0x0; }
        }

        public byte WebFlags { get { return this.webFlags; } set { this.webFlags = value; } }

        public byte OperationCode
        {
            get { return this.OperationRequest.OperationCode; }
        }

        public System.Collections.Generic.Dictionary<byte, object> Parameters
        {
            get { return this.OperationRequest.Parameters; }
        }

        public bool UsingCAS { get { return this.ExpectedValues != null && this.ExpectedValues.Count != 0; } }

        public bool UpdatingGameProperties { get { return this.ActorNumber == 0 && this.Properties != null && this.Properties.Count > 0; } }

        //cached values of game properties which this request contains
        public byte? newMaxPlayer;
        public bool? newIsOpen;
        public bool? newIsVisible;
        public object[] newLobbyProperties;
        public Hashtable newGameProperties;
        public int? MasterClientId;
        public Actor SenderActor;
        public Actor TargetActor;
        public IEnumerable<Actor> PublishTo;
        public string[] ExpectedUsers;
    }
}
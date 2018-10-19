// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This class implements the Join operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Photon.Common;

namespace Photon.Hive.Operations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Photon.Hive.Common;
    using Photon.Hive.Events;
    using Photon.Hive.Plugin;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;

    public class JoinModes : Photon.Hive.Plugin.JoinModeConstants
    {
    }

    /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class JoinGameRequest : Operation, IJoinGameRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinGameRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public JoinGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            if (!this.IsValid)
            {
                return;
            }

            // special treatment for game and actor properties sent by AS3/Flash or JSON clients
            var protocolId = protocol.ProtocolType;
            if (protocolId == ProtocolType.Amf3V16 || protocolId == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(this.GameProperties, this.ActorProperties);
            }

            this.SetupRequest();
        }

        public void SetupRequest()
        {
            //TBD - set GameProperties to new Hashtable() instead of checking for null everywhere?
            this.properties = this.GameProperties ?? new Hashtable();

            if (this.properties != null && this.properties.Count > 0)
            {
                string[] expectedUsers;
                this.isValid = GameParameterReader.TryGetProperties(
                    this.properties,
                    out this.newMaxPlayer,
                    out this.newIsOpen,
                    out this.newIsVisible,
                    out this.newLobbyProperties,
                    out expectedUsers,
                    out this.errorMessage);
            }

            if (this.IsValid && this.AddUsers != null)
            {
                if (this.newMaxPlayer.HasValue && this.newMaxPlayer != 0 && this.AddUsers.Length >= this.newMaxPlayer)
                {
                    this.isValid = false;
                    this.errorMessage = "Reserved slots count is more then max player value";
                    return;
                }

                if (this.AddUsers.Any(string.IsNullOrEmpty))
                {
                    this.isValid = false;
                    this.errorMessage = "slot name can not be empty";
                    return;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinGameRequest"/> class.
        /// </summary>
        public JoinGameRequest()
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
        public virtual string GameId { get; set; }

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

        private int actorNr = 0;
        /// <summary>
        /// Actor number, which will be used for rejoin
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNr
        {
            get
            {
                return this.actorNr;
            }
            set
            {
                this.actorNr = value;
                //if (this.actorNr > 0 && this.CreateIfNotExists)
                if (!this.IsRejoining)
                {
                    this.JoinMode = JoinModes.RejoinOrJoin;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how long the room instance will be keeped alive 
        /// in the room cache after all peers have left the room.
        /// </summary>
        /// <remarks>
        /// This property will only be applied for the room creator.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.EmptyRoomLiveTime, IsOptional = true)]
        public int EmptyRoomLiveTime { get; set; }

        /// <summary>
        /// The time a player the room waits to allow a player to rejoin after a disconnect.
        /// If player should be allowed to return any time set the value less than 0.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.PlayerTTL, IsOptional = true)]
        public int PlayerTTL { get; set; }

        /// <summary>
        /// Set true to restrict useres to connect only once.
        /// Default is not to check.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.CheckUserOnJoin, IsOptional = true)]
        public bool CheckUserOnJoin { get; set; }

        /// <summary>
        /// The lowest slice of cached events the actor expects to recieve.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.CacheSliceIndex, IsOptional = true)]
        public int? CacheSlice { get; set; }

        [DataMember(Code = (byte)ParameterKey.LobbyName, IsOptional = true)]
        public string LobbyName { get; set; }

        [DataMember(Code = (byte)ParameterKey.LobbyType, IsOptional = true)]
        public byte LobbyType { get; set; }

        private object internalJoinMode;
        [DataMember(Code = (byte)ParameterKey.JoinMode, IsOptional = true)]
        internal object InternalJoinMode
        {
            get
            {
                return this.internalJoinMode;
            }
            set
            {
                this.internalJoinMode = value;
                var type = value.GetType();
                if (type == typeof(bool))
                {
                    if (this.JoinMode == JoinModes.JoinOnly && (bool)value)
                    {
                        this.JoinMode = JoinModes.CreateIfNotExists;
                    }
                    return;
                }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Double: // all numbers in our json parser are double
                        this.JoinMode = System.Convert.ToByte(value);
                        return;
                }
            }
        }

        /// <summary>Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin missmatch returning error code PluginMismatch 32757(0x7FFF - 10).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Plugins, IsOptional = true)]
        public string[] Plugins { get; set; }

        [DataMember(Code = (byte)ParameterKey.WebFlags, IsOptional = true)]
        public byte WebFlags { get; set; }

        // users to add as expected
        [DataMember(Code = (byte)ParameterKey.AddUsers, IsOptional = true)]
        public string[] AddUsers { get; set; }

        [DataMember(Code = (byte)ParameterKey.PublishUserId, IsOptional = true)]
        public bool PublishUserId { get; set; }

        [DataMember(Code = (byte)ParameterKey.ForceRejoin, IsOptional = true)]
        public bool ForceRejoin { get; set; }
        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public byte JoinMode { get; set; }

        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public bool CreateIfNotExists
        {
            get
            {
                return this.JoinMode == JoinModes.CreateIfNotExists;
            }
        }

        public byte OperationCode
        {
            get { return this.OperationRequest.OperationCode; }
        }

        public Dictionary<byte, object> Parameters
        {
            get { return this.OperationRequest.Parameters; }
        }

        //cached values of game properties which this request contains
        public byte? newMaxPlayer;
        public bool? newIsOpen;
        public bool? newIsVisible;
        public object[] newLobbyProperties;
        public Hashtable properties;

        public bool IsRejoining
        {
            get
            {
                return this.JoinMode == JoinModes.RejoinOnly || this.JoinMode == JoinModes.RejoinOrJoin;
            }
        }

        public ErrorCode FailureReason { get; protected set; }
        public string FailureMessage { get; protected set; }

        public Dictionary<string, object> GetCreateGameSettings(HiveGame game)
        {
            var settings = new Dictionary<string, object>();

            // set default properties
            if (newMaxPlayer.HasValue && newMaxPlayer.Value != game.MaxPlayers)
            {
                settings[HiveHostGameState.MaxPlayers.ToString()] = newMaxPlayer.Value;
            }

            if (newIsOpen.HasValue && newIsOpen.Value != game.IsOpen)
            {
                settings[HiveHostGameState.IsOpen.ToString()] = newIsOpen.Value;
            }

            if (newIsVisible.HasValue && newIsVisible.Value != game.IsVisible)
            {
                settings[HiveHostGameState.IsVisible.ToString()] = newIsVisible.Value;
            }

            settings[HiveHostGameState.LobbyId.ToString()] = this.LobbyName;
            settings[HiveHostGameState.LobbyType.ToString()] = this.LobbyType;

            if (newLobbyProperties != null)
            {
                settings[HiveHostGameState.CustomProperties.ToString()] =
                    GameParameterReader.GetLobbyGameProperties(this.GameProperties, new HashSet<object>(newLobbyProperties));
            }

            settings[HiveHostGameState.EmptyRoomTTL.ToString()] = this.EmptyRoomLiveTime;
            settings[HiveHostGameState.PlayerTTL.ToString()] = this.PlayerTTL;
            settings[HiveHostGameState.CheckUserOnJoin.ToString()] = this.CheckUserOnJoin;
            settings[HiveHostGameState.DeleteCacheOnLeave.ToString()] = this.DeleteCacheOnLeave;
            settings[HiveHostGameState.SuppressRoomEvents.ToString()] = this.SuppressRoomEvents;

            return settings;
        }

        public string GetNickname()
        {
            if (this.ActorProperties != null)
            {
                return (this.ActorProperties[(byte)255] as string) ?? string.Empty;
            }

            return string.Empty;
        }

        public void OnJoinFailed(ErrorCode reason, string msg)
        {
            this.FailureReason = reason;
            this.FailureMessage = msg;
        }
    }
}
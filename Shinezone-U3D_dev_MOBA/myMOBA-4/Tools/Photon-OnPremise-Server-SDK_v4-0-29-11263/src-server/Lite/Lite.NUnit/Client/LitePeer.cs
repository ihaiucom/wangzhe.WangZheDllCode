// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LitePeer.cs" company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <summary>
//   Peer implementing the Lite Application operations as simple API.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ExitGames.Client.Photon.Lite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    

    #region Lite Codes

    /// <summary>
    /// Lite - Event codes.
    /// These codes are defined by the Lite application's logic on the server side.
    /// Other application's won't necessarily use these.
    /// </summary>
    /// <remarks>If your game is built as extension of Lite, don't re-use these codes for your custom events.</remarks>
    public static class LiteEventCode
    {
        /// <summary>(255) Event Join: someone joined the game</summary>
        public const byte Join = 255;

        /// <summary>(254) Event Leave: someone left the game</summary>
        public const byte Leave = 254;

        /// <summary>(253) Event PropertiesChanged</summary>
        public const byte PropertiesChanged = 253;

        /// <summary>(252) Event Disconnect</summary>
        public const byte Disconnect = 252;
    }

    /// <summary>
    /// Lite - Keys of event-parameters that are defined by the Lite application logic.
    /// To keep things lean (in terms of bandwidth), we use byte keys to identify values in events within Photon.
    /// In Lite, you can send custom events by defining a EventCode and some content. This custom content is a Hashtable,
    /// which can use any type for keys and values. The parameter for operation RaiseEvent and the resulting
    /// Events use key (byte)245 for the custom content. The constant for this is: Data or
    /// <see cref="LiteEventKey.CustomContent" text="LiteEventKey.CustomContent Field" />.
    /// </summary>
    /// <remarks>
    /// If your game is built as extension of Lite, don't re-use these codes for your custom events.
    /// </remarks>
    public static class LiteEventKey
    {
        /// <summary>(254) Playernumber of the player who triggered the event.</summary>
        public const byte ActorNr = 254;

        /// <summary>(253) Playernumber of the player who is target of an event (e.g. changed properties).</summary>
        public const byte TargetActorNr = 253;

        /// <summary>(252) List of playernumbers currently in the room.</summary>
        public const byte ActorList = 252;

        /// <summary>(251) Set of properties (a Hashtable).</summary>
        public const byte Properties = 251;

        /// <summary>(249) Key for actor (player) property set (Hashtable).</summary>
        public const byte ActorProperties = 249;

        /// <summary>(248) Key for game (room) property set (Hashtable).</summary>
        public const byte GameProperties = 248;

        /// <summary>(245) Custom Content of an event (a Hashtable in Lite).</summary>
        public const byte Data = 245;

        /// <summary>
        /// (245) The Lite operation RaiseEvent will place the Hashtable with your custom event-content under this key.</summary>
        /// <remarks>Alternative for: Data!</remarks>
        public const byte CustomContent = Data;
    }

    /// <summary>
    /// Lite - Operation Codes.
    /// This enumeration contains the codes that are given to the Lite Application's
    /// operations. Instead of sending "Join", this enables us to send the byte 255.
    /// </summary>
    /// <remarks>
    /// Other applications (the MMO demo or your own) could define other operations and other codes.
    /// If your game is built as extension of Lite, don't re-use these codes for your custom events.
    /// </remarks>
    public static class LiteOpCode
    {
        [Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")]
        public const byte ExchangeKeysForEncryption = 250;

        /// <summary>(255) Code for OpJoin, to get into a room.</summary>
        public const byte Join = 255;

        /// <summary>(254) Code for OpLeave, to get out of a room.</summary>
        public const byte Leave = 254;

        /// <summary>(253) Code for OpRaiseEvent (not same as eventCode).</summary>
        public const byte RaiseEvent = 253;

        /// <summary>(252) Code for OpSetProperties.</summary>
        public const byte SetProperties = 252;

        /// <summary>(251) Operation code for OpGetProperties.</summary>
        public const byte GetProperties = 251;

        /// <summary>(248) Operation code to change interest groups in Rooms (Lite application and extending ones).</summary>
        public const byte ChangeGroups = 248;
    }

    /// <summary>
    /// Lite - keys for parameters of operation requests and responses (short: OpKey).
    /// </summary>
    /// <remarks>
    /// These keys match a definition in the Lite application (part of the server SDK).
    /// If your game is built as extension of Lite, don't re-use these codes for your custom events.
    ///
    /// These keys are defined per application, so Lite has different keys than MMO or your
    /// custom application. This is why these are not an enumeration.
    /// Lite and Lite Lobby will use the keys 255 and lower, to give you room for your own codes.
    ///
    /// Keys for operation-parameters could be assigned on a per operation basis, but
    /// it makes sense to have fixed keys for values which are used throughout the whole
    /// application.
    /// </remarks>
    public static class LiteOpKey
    {
        /// <summary>(255) Code of the room name. Used in OpJoin (Asid = Application Session ID).</summary>
        [Obsolete("Use GameId")]
        public const byte Asid = 255;

        /// <summary>(255) Code of the room name. Used in OpJoin.</summary>
        /// <remarks>Alternative for: Asid!</remarks>
        [Obsolete("Use GameId")]
        public const byte RoomName = 255;

        /// <summary>(255) Code of the game id (a unique room name). Used in OpJoin.</summary>
        public const byte GameId = 255;

        /// <summary>(254) Code of the Actor of an operation. Used for property get and set.</summary>
        public const byte ActorNr = 254;

        /// <summary>(253) Code of the target Actor of an operation. Used for property set. Is 0 for game</summary>
        public const byte TargetActorNr = 253;

        /// <summary>(252) Code for list of players in a room. Currently not used.</summary>
        public const byte ActorList = 252;

        /// <summary>
        /// (251) Code for property set (Hashtable). This key is used when sending only one set of properties.
        /// If either ActorProperties or GameProperties are used (or both), check those keys.
        /// </summary>
        public const byte Properties = 251;

        /// <summary>(250) Code for broadcast parameter of OpSetProperties method.</summary>
        public const byte Broadcast = 250;

        /// <summary>(249) Code for property set (Hashtable).</summary>
        public const byte ActorProperties = 249;

        /// <summary>(248) Code for property set (Hashtable).</summary>
        public const byte GameProperties = 248;

        /// <summary>(247) Code for caching events while raising them.</summary>
        public const byte Cache = 247;

        /// <summary>(246) Code to select the receivers of events (used in Lite, Operation RaiseEvent).</summary>
        public const byte ReceiverGroup = 246;

        /// <summary>(245) Code of data of an event. Used in OpRaiseEvent.</summary>
        public const byte Data = 245;

        /// <summary>(244) Code used when sending some code-related parameter, like OpRaiseEvent's event-code.</summary>
        /// <remarks>This is not the same as the Operation's code, which is no longer sent as part of the parameter Dictionary in Photon 3.</remarks>
        public const byte Code = 244;

        /// <summary>(240) Code for "group" operation-parameter (as used in Op RaiseEvent).</summary>
        public const byte Group = 240;

        /// <summary>
        /// (239) The "Remove" operation-parameter can be used to remove something from a list. E.g. remove groups from player's interest groups.
        /// </summary>
        public const byte Remove = 239;

        /// <summary>
        /// (238) The "Add" operation-parameter can be used to add something to some list or set. E.g. add groups to player's interest groups.
        /// </summary>
        public const byte Add = 238;
    }

    /// <summary>
    /// Lite - Flags for "types of properties", being used as filter in OpGetProperties.
    /// </summary>
    [Flags]
    public enum LitePropertyTypes : byte
    {
        /// <summary>(0x00) Flag type for no property type.</summary>
        None = 0x00,

        /// <summary>(0x01) Flag type for game-attached properties.</summary>
        Game = 0x01,

        /// <summary>(0x02) Flag type for actor related propeties.</summary>
        Actor = 0x02,

        /// <summary>(0x01) Flag type for game AND actor properties. Equal to 'Game'</summary>
        GameAndActor = Game | Actor
    }

    /// <summary>
    /// Lite - OpRaiseEvent allows you to cache events and automatically send them to joining players in a room.
    /// Events are cached per event code and player: Event 100 (example!) can be stored once per player.
    /// Cached events can be modified, replaced and removed.
    /// </summary>
    /// <remarks>
    /// Caching works only combination with ReceiverGroup options Others and All.
    /// </remarks>
    public enum EventCaching : byte
    {
        /// <summary>Default value (not sent).</summary>
        DoNotCache = 0,

        /// <summary>Will merge this event's keys with those already cached.</summary>
        [Obsolete]
        MergeCache = 1,

        /// <summary>Replaces the event cache for this eventCode with this event's content.</summary>
        [Obsolete]
        ReplaceCache = 2,

        /// <summary>Removes this event (by eventCode) from the cache.</summary>
        [Obsolete]
        RemoveCache = 3,

        /// <summary>Adds an event to the room's cache</summary>
        AddToRoomCache = 4,

        /// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
        AddToRoomCacheGlobal = 5,

        /// <summary>Remove fitting event from the room's cache.</summary>
        RemoveFromRoomCache = 6,

        /// <summary>Removes events of players who already left the room (cleaning up).</summary>
        RemoveFromRoomCacheForActorsLeft  = 7,

        /// <summary>Increase the index of the sliced cache.</summary>
        SliceIncreaseIndex = 10,

        /// <summary>Set the index of the sliced cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SliceSetIndex = 11,

        /// <summary>Purge cache slice with index. Exactly one slice is removed from cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeIndex = 12,

        /// <summary>Purge cache slices with specified index and anything lower than that. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeUpToIndex = 13,
    }

    /// <summary>
    /// Lite - OpRaiseEvent lets you chose which actors in the room should receive events.
    /// By default, events are sent to "Others" but you can overrule this.
    /// </summary>
    public enum ReceiverGroup : byte
    {
        /// <summary>Default value (not sent). Anyone else gets my event.</summary>
        Others = 0,

        /// <summary>Everyone in the current room (including this peer) will get this event.</summary>
        All = 1,

        /// <summary>The server sends this event only to the actor with the lowest actorNumber.</summary>
        /// <remarks>The "master client" does not have special rights but is the one who is in this room the longest time.</remarks>
        MasterClient = 2,
    }

    #endregion

    #region LitePeer - Constructor and Operations

    /// <summary>
    /// A LitePeer is an extended PhotonPeer and implements the operations offered by the "Lite" Application
    /// of the Photon Server SDK.
    /// </summary>
    /// <remarks>
    /// This class is used by our samples and allows rapid development of simple games. You can use rooms and
    /// properties and send events. For many games, this is a good start.
    ///
    /// Operations are prefixed as "Op" and are always asynchronous. In most cases, an OperationResult is
    /// provided by a later call to OnOperationResult.
    /// </remarks>
    public class LitePeer : PhotonPeer
    {
        /// <summary>
        /// Creates a LitePeer instance to connect and communicate with a Photon server.<para></para>
        /// Uses UDP as protocol (except in the Silverlight library).
        /// </summary>
        /// <param name="listener">Your IPhotonPeerListener implementation.</param>
        public LitePeer(IPhotonPeerListener listener) : base(listener, ConnectionProtocol.Udp)
        {
        }

        /// <summary>
        /// Creates a LitePeer instance to connect and communicate with a Photon server.<para></para>
        /// Uses UDP as protocol (except in the Silverlight library).
        /// </summary>
        protected LitePeer() : base(ConnectionProtocol.Udp)
        {
        }

        /// <summary>
        /// Creates a LitePeer instance to connect and communicate with a Photon server.
        /// </summary>
        protected LitePeer(ConnectionProtocol protocolType) : base(protocolType)
        {
        }

        /// <summary>
        /// Creates a LitePeer instance to communicate with Photon with your selection of protocol.
        /// We recommend UDP.
        /// </summary>
        /// <param name="listener">Your IPhotonPeerListener implementation.</param>
        /// <param name="protocolType">Protocol to use to connect to Photon.</param>
        public LitePeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
        {
        }

        /// <summary>
        /// Operation to handle this client's interest groups (for events in room).
        /// </summary>
        /// <remarks>
        /// Note the difference between passing null and byte[0]:
        ///   null won't add/remove any groups.
        ///   byte[0] will add/remove all (existing) groups.
        /// First, removing groups is executed. This way, you could leave all groups and join only the ones provided.
        /// </remarks>
        /// <param name="groupsToRemove">Groups to remove from interest. Null will not leave any. A byte[0] will remove all.</param>
        /// <param name="groupsToAdd">Groups to add to interest. Null will not add any. A byte[0] will add all current.</param>
        /// <returns></returns>
        public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            if (this.DebugOut >= DebugLevel.ALL)
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (groupsToRemove != null)
            {
                opParameters[(byte)LiteOpKey.Remove] = groupsToRemove;
            }
            if (groupsToAdd != null)
            {
                opParameters[(byte) LiteOpKey.Add] = groupsToAdd;
            }

            return this.OpCustom((byte)LiteOpCode.ChangeGroups, opParameters, true, 0);
        }
        
        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent)
        {
            return this.OpRaiseEvent(eventCode, sendReliable, customEventContent, 0, EventCaching.DoNotCache, null, ReceiverGroup.Others, 0);
        }

        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="channelId">Command sequence in which this command belongs. Must be less than value of ChannelCount property. Default: 0.</param>
        /// <param name="cache">Affects how the server will treat the event caching-wise. Can cache events for players joining later on or remove previously cached events. Default: DoNotCache.</param>
        /// <param name="targetActors">List of ActorNumbers (in this room) to send the event to. Overrides caching. Default: null.</param>
        /// <param name="receivers">Defines a target-player group. Default: Others.</param>
        /// <param name="interestGroup">Defines to which interest group the event is sent. Players can subscribe or unsibscribe to groups. Group 0 is always sent to all. Default: 0.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId, EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.Code] = (byte)eventCode;

            if (customEventContent != null)
            {
                opParameters[(byte)LiteOpKey.Data] = customEventContent;
            }
            if (cache != EventCaching.DoNotCache)
            {
                opParameters[(byte)LiteOpKey.Cache] = (byte)cache;
            }
            if (receivers != ReceiverGroup.Others)
            {
                opParameters[(byte)LiteOpKey.ReceiverGroup] = (byte)receivers;
            }
            if (interestGroup != 0)
            {
                opParameters[(byte)LiteOpKey.Group] = (byte)interestGroup;
            }
            if (targetActors != null)
            {
                opParameters[(byte)LiteOpKey.ActorList] = targetActors;
            }

            return this.OpCustom((byte)LiteOpCode.RaiseEvent, opParameters, sendReliable, channelId, false);
        }

        /// <summary>
        /// Send your custom data as event to an "interest group" in the current Room.
        /// </summary>
        /// <remarks>
        /// No matter if reliable or not, when an event is sent to a interest Group, some users won't get this data.
        /// Clients can control the groups they are interested in by using OpChangeGroups.
        /// </remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="interestGroup">The ID of the interest group this event goes to (exclusively). Grouo 0 sends to all.</param>
        /// <param name="customEventContent">Custom data you want to send along (use null, if none).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpRaiseEvent(byte eventCode, byte interestGroup, Hashtable customEventContent, bool sendReliable)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.Data] = customEventContent;
            opParameters[(byte)LiteOpKey.Code] = (byte)eventCode;
            if (interestGroup != 0)
            {
                opParameters[(byte)LiteOpKey.Group] = (byte)interestGroup;
            }

            return this.OpCustom((byte)LiteOpCode.RaiseEvent, opParameters, sendReliable, 0);
        }

        /// <summary>
        /// RaiseEvent tells the server to send an event to the other players within the same room.
        /// </summary>
        /// <remarks>
        /// This method is described in one of its overloads.
        /// </remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Custom data you want to send along (use null, if none).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable customEventContent, bool sendReliable)
        {
            return this.OpRaiseEvent(eventCode, customEventContent, sendReliable, 0);
        }

        /// <summary>
        /// RaiseEvent tells the server to send an event to the other players within the same room.
        /// </summary>
        /// <remarks>
        /// Type and content of the event can be defined by the client side at will. The server only
        /// forwards the content and eventCode to others in the same room.
        ///
        /// The eventCode should be used to define the event's type and content respectively.///
        /// Lite and Loadbalancing are using a few eventCode values already but those start with 255 and go down.
        /// Your eventCodes can start at 1, going up.
        ///
        /// The customEventContent is a Hashtable with any number of key-value pairs of
        /// <see cref="Serializable Datatypes" text="serializable datatypes" /> or null.
        /// Receiving clients can access this Hashtable as Parameter LiteEventKey.Data (see below).
        ///
        /// RaiseEvent can be used reliable or unreliable. Both result in ordered events but the unreliable ones
        /// might be lost and allow gaps in the resulting event sequence. On the other hand, they cause less
        /// overhead and are optimal for data that is replaced soon.
        ///
        /// Like all operations, RaiseEvent is not done immediately but when you call SendOutgoingCommands.
        ///
        /// It is recommended to keep keys (and data) as simple as possible (e.g. byte or short as key), as
        /// the data is typically sent multiple times per second. This easily adds up to a huge amount of data
        /// otherwise.
        /// </remarks>
        /// <example>
        /// <code>
        /// //send some position data (using byte-keys, as they are small):
        ///
        /// Hashtable evInfo = new Hashtable();
        /// Player local = (Player)players[playerLocalID];
        /// evInfo.Add((byte)STATUS_PLAYER_POS_X, (int)local.posX);
        /// evInfo.Add((byte)STATUS_PLAYER_POS_Y, (int)local.posY);
        ///
        /// peer.OpRaiseEvent(EV_MOVE, evInfo, true);  //EV_MOVE = (byte)1
        ///
        /// //receive this custom event in OnEvent():
        /// Hashtable data = (Hashtable)photonEvent[LiteEventKey.Data];
        /// switch (eventCode) {
        ///   case EV_MOVE:               //1 in this sample
        ///       p = (Player)players[actorNr];
        ///       if (p != null) {
        ///           p.posX = (int)data[(byte)STATUS_PLAYER_POS_X];
        ///           p.posY = (int)data[(byte)STATUS_PLAYER_POS_Y];
        ///       }
        ///       break;
        /// </code>
        ///
        /// Events from the Photon Server are internally buffered until they are
        /// <see cref="PhotonPeer.DispatchIncomingCommands" text="Dispatched" />, just
        /// like OperationResults.
        /// </example>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Custom data you want to send along (use null, if none).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="channelId">Number of channel (sequence) to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable customEventContent, bool sendReliable, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.Data] = customEventContent;
            opParameters[(byte)LiteOpKey.Code] = (byte)eventCode;

            return this.OpCustom((byte)LiteOpCode.RaiseEvent, opParameters, sendReliable, channelId);
        }

        /// <summary>
        /// RaiseEvent tells the server to send an event to the other players within the same room.
        /// </summary>
        /// <remarks>
        /// This method is described in one of its overloads.
        ///
        /// This variant has an optional list of targetActors. Use this to send the event only to
        /// specific actors in the same room, each identified by an actorNumber (or ID).
        ///
        /// This can be useful to implement private messages inside a room or similar.
        /// </remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Custom data you want to send along (use null, if none).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <param name="targetActors">List of actorNumbers that receive this event.</param>
        /// <returns>If operation could be enqueued for sending.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable customEventContent, bool sendReliable, byte channelId, int[] targetActors)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.Data] = customEventContent;
            opParameters[(byte)LiteOpKey.Code] = eventCode;
            if (targetActors != null)
            {
                opParameters[(byte)LiteOpKey.ActorList] = targetActors;
            }

            return this.OpCustom((byte)LiteOpCode.RaiseEvent, opParameters, sendReliable, channelId);
        }

        /// <summary>
        /// Calls operation RaiseEvent on the server, with full control of event-caching and the target receivers.
        /// </summary>
        /// <remarks>
        /// This method is described in one of its overloads.
        ///
        /// The cache parameter defines if and how this event will be cached server-side. Per event-code, your client
        /// can store events and update them and will send cached events to players joining the same room.
        ///
        /// The option EventCaching.DoNotCache matches the default behaviour of RaiseEvent.
        /// The option EventCaching.MergeCache will merge the costomEventContent into existing one.
        /// Values in the customEventContent Hashtable can be null to remove existing values.
        ///
        /// With the receivers parameter, you can chose who gets this event: Others (default), All (includes you as sender)
        /// or MasterClient. The MasterClient is the connected player with the lowest ActorNumber in this room.
        /// This player could get some privileges, if needed.
        ///
        /// Read more about Cached Events in the DevNet: http://doc.exitgames.com
        /// </remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Custom data you want to send along (use null, if none).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <param name="cache">Events can be cached (merged and removed) for players joining later on.</param>
        /// <param name="receivers">Controls who should get this event.</param>
        /// <returns>If operation could be enqueued for sending.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable customEventContent, bool sendReliable, byte channelId, EventCaching cache, ReceiverGroup receivers)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.Data] = customEventContent;
            opParameters[(byte)LiteOpKey.Code] = (byte)eventCode;

            if (cache != EventCaching.DoNotCache)
            {
               opParameters[(byte)LiteOpKey.Cache] = (byte)cache;
            }

            if (receivers != ReceiverGroup.Others)
            {
                opParameters[(byte)LiteOpKey.ReceiverGroup] = (byte)receivers;
            }

            return this.OpCustom((byte)LiteOpCode.RaiseEvent, opParameters, sendReliable, channelId, false);
        }

        /// <summary>
        /// Attaches or updates properties of the specified actor.
        /// </summary>
        /// <remarks>
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">Hashtable containing the properties to add or update.</param>
        /// <param name="actorNr">the actorNr is used to identify a player/peer in a game</param>
        /// <param name="broadcast">true will trigger an event LiteEventKey.PropertiesChanged with the updated properties in it</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpSetPropertiesOfActor(int actorNr, Hashtable properties, bool broadcast, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, properties);
            opParameters.Add((byte)LiteOpKey.ActorNr, actorNr);
            if (broadcast)
            {
                opParameters.Add((byte)LiteOpKey.Broadcast, broadcast);
            }

            return this.OpCustom((byte)LiteOpCode.SetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Attaches or updates properties of the current game.
        /// </summary>
        /// <remarks>
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">hashtable containing the properties to add or overwrite</param>
        /// <param name="broadcast">true will trigger an event LiteEventKey.PropertiesChanged with the updated
        ///                         properties in it</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpSetPropertiesOfGame(Hashtable properties, bool broadcast, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, properties);
            if (broadcast)
            {
                opParameters.Add((byte)LiteOpKey.Broadcast, broadcast);
            }

            return this.OpCustom((byte)LiteOpCode.SetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Gets all properties of the game and each actor.
        /// </summary>
        /// <remarks>
        /// The server returns only actor properties of players who are in the room when the
        /// operation executes (players might join or leave while the operation is on it's way).
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpGetProperties(byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, (byte)LitePropertyTypes.GameAndActor);

            return this.OpCustom((byte)LiteOpCode.GetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Gets selected properties of an actor.
        /// </summary>
        /// <remarks>
        /// The server returns only actor properties of players who are in the room when the
        /// operation executes (players might join or leave while the operation is on it's way).
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">optional, array of property keys to fetch</param>
        /// <param name="actorNrList">optional, a list of actornumbers to get the properties of</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpGetPropertiesOfActor(int[] actorNrList, string[] properties, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, LitePropertyTypes.Actor);
            if (properties != null)
            {
                opParameters.Add((byte)LiteOpKey.ActorProperties, properties);
            }

            if (actorNrList != null)
            {
                opParameters.Add((byte)LiteOpKey.ActorList, actorNrList);
            }

            return this.OpCustom((byte)LiteOpCode.GetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Gets selected properties of some actors.
        /// </summary>
        /// <remarks>
        /// The server returns only actor properties of players who are in the room when the
        /// operation executes (players might join or leave while the operation is on it's way).
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">array of property keys to fetch. optional (can be null).</param>
        /// <param name="actorNrList">optional, a list of actornumbers to get the properties of</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpGetPropertiesOfActor(int[] actorNrList, byte[] properties, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, LitePropertyTypes.Actor);
            if (properties != null)
            {
                opParameters.Add((byte)LiteOpKey.ActorProperties, properties);
            }

            if (actorNrList != null)
            {
                opParameters.Add((byte)LiteOpKey.ActorList, actorNrList);
            }

            return this.OpCustom((byte)LiteOpCode.GetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Gets selected properties of current game.
        /// </summary>
        /// <remarks>
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">array of property keys to fetch. optional (can be null).</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpGetPropertiesOfGame(string[] properties, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, LitePropertyTypes.Game);
            if (properties != null)
            {
                opParameters.Add((byte)LiteOpKey.GameProperties, properties);
            }

            return this.OpCustom((byte)LiteOpCode.GetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// Gets selected properties of current game.
        /// </summary>
        /// <remarks>
        /// Please read the general description of <see cref="Properties on Photon" />.
        /// </remarks>
        /// <param name="properties">array of property keys to fetch. optional (can be null).</param>
        /// <param name="channelId">Number of channel to use (starting with 0).</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpGetPropertiesOfGame(byte[] properties, byte channelId)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add((byte)LiteOpKey.Properties, LitePropertyTypes.Game);
            if (properties != null)
            {
                opParameters.Add((byte)LiteOpKey.GameProperties, properties);
            }

            return this.OpCustom((byte)LiteOpCode.GetProperties, opParameters, true, channelId);
        }

        /// <summary>
        /// This operation will join an existing room by name or create one if the name is not in use yet.
        ///
        /// Rooms (or games) are simply identified by name. We assume that users always want to get into a room - no matter
        /// if it existed before or not, so it might be a new one. If you want to make sure a room is created (new, empty),
        /// the client side might come up with a unique name for it (make sure the name was not taken yet).
        ///
        /// The application "Lite Lobby" lists room names and effectively allows the user to select a distinct one.
        ///
        /// Each actor (a.k.a. player) in a room will get events that are raised for the room by any player.
        ///
        /// To distinguish the actors, each gets a consecutive actornumber. This is used in events to mark who triggered
        /// the event. A client finds out it's own actornumber in the return callback for operation Join. Number 1 is the
        /// lowest actornumber in each room and the client with that actornumber created the room.
        ///
        /// Each client could easily send custom data around. If the data should be available to newcomers, it makes sense
        /// to use Properties.
        ///
        /// Joining a room will trigger the event <see cref="LiteEventCode.Join" text="LiteEventCode.Join" />, which contains
        /// the list of actorNumbers of current players inside the  room
        /// (<see cref="LiteEventKey.ActorList" text="LiteEventKey.ActorList" />). This also gives you a count of current
        /// players.
        /// </summary>
        /// <param name="gameName">Any identifying name for a room / game.</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpJoin(string gameName)
        {
            return this.OpJoin(gameName, null, null, false);
        }

        /// <summary>
        /// This operation will join an existing room by name or create one if the name is not in use yet.
        ///
        /// Rooms (or games) are simply identified by name. We assume that users always want to get into a room - no matter
        /// if it existed before or not, so it might be a new one. If you want to make sure a room is created (new, empty),
        /// the client side might come up with a unique name for it (make sure the name was not taken yet).
        ///
        /// The application "Lite Lobby" lists room names and effectively allows the user to select a distinct one.
        ///
        /// Each actor (a.k.a. player) in a room will get events that are raised for the room by any player.
        ///
        /// To distinguish the actors, each gets a consecutive actornumber. This is used in events to mark who triggered
        /// the event. A client finds out it's own actornumber in the return callback for operation Join. Number 1 is the
        /// lowest actornumber in each room and the client with that actornumber created the room.
        ///
        /// Each client could easily send custom data around. If the data should be available to newcomers, it makes sense
        /// to use Properties.
        ///
        /// Joining a room will trigger the event <see cref="LiteEventCode.Join" text="LiteEventCode.Join" />, which contains
        /// the list of actorNumbers of current players inside the  room
        /// (<see cref="LiteEventKey.ActorList" text="LiteEventKey.ActorList" />). This also gives you a count of current
        /// players.
        /// </summary>
        ///
        /// <param name="gameName">Any identifying name for a room / game.</param>
        /// <param name="gameProperties">optional, set of game properties, by convention: only used if game is new/created</param>
        /// <param name="actorProperties">optional, set of actor properties</param>
        /// <param name="broadcastActorProperties">optional, broadcast actor proprties in join-event</param>
        /// <returns>If operation could be enqueued for sending</returns>
        public virtual bool OpJoin(string gameName, Hashtable gameProperties, Hashtable actorProperties, bool broadcastActorProperties)
        {
            if (this.DebugOut >= DebugLevel.ALL)
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "OpJoin(" + gameName + ")");
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[(byte)LiteOpKey.GameId] = gameName;
            if (actorProperties != null)
            {
                opParameters[(byte)LiteOpKey.ActorProperties] = actorProperties;
            }

            if (gameProperties != null)
            {
                opParameters[(byte)LiteOpKey.GameProperties] = gameProperties;
            }

            if (broadcastActorProperties)
            {
                opParameters[(byte)LiteOpKey.Broadcast] = broadcastActorProperties;
            }

            return this.OpCustom((byte)LiteOpCode.Join, opParameters, true, 0, false);
        }

        /// <summary>
        /// Leave operation of the Lite Application (also in Lite Lobby).
        /// Leaves a room / game, but keeps the connection. This operations triggers the event <see cref="LiteEventCode.Leave" text="LiteEventCode.Leave" />
        /// for the remaining clients. The event includes the actorNumber of the player who left in key <see cref="LiteEventKey.ActorNr" text="LiteEventKey.ActorNr" />.
        /// </summary>
        /// <returns>
        /// Consecutive invocationID of the OP. Will throw Exception if not connected.
        /// </returns>
        public virtual bool OpLeave()
        {
            if (this.DebugOut >= DebugLevel.ALL)
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "OpLeave()");
            }

            return this.OpCustom((byte)LiteOpCode.Leave, null, true, 0);
        }

        #endregion
    }
}

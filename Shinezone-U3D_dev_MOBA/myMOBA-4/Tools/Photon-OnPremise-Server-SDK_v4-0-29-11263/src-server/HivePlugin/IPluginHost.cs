// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginHost.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Plugin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;


    /// <summary>
    /// Related to RaiseEvent operation.
    /// Lets you choose which actors in the room should receive events.
    /// </summary>
    public class ReciverGroup
    {
        /// <summary>Broadcast: Everyone in the current room (including this peer) will get the event.</summary>
        public const byte All = 0;
        /// <summary>Opponents (other actors) in the current room gets the event. (Anyone except me)</summary>
        public const byte Others = 1;
        /// <summary>Only actors subscribed to the specified interest group target will get the event.</summary>
        public const byte Group = 2;
    }

    /// <summary>
    /// Codes returned as a result of process of queued HTTP request. 
    /// </summary>
    public class HttpRequestQueueResult
    {
        /// <summary>
        /// The HTTP request was succesfully processed.
        /// </summary>
        /// <remarks>
        /// If the endpoint returns a successful HTTP status code. 
        /// i.e. 2xx codes.
        /// </remarks>
        public const byte Success = 0;

        /// <summary>
        /// The HTTP request timed out.
        /// </summary>
        /// <remarks>
        /// This happens if the endpoint does not return a response in a timely manner. 
        /// </remarks>
        public const byte RequestTimeout = 1;

        /// <summary>
        /// The HTTP request queue timed out.
        /// </summary>
        /// <remarks>
        /// A timer starts when a request is put into the HttpRequestQueue.
        /// A request can timeout if takes too much time inside the queue.
        /// </remarks>
        public const byte QueueTimeout = 2;

        /// <summary>
        /// If the application's respective HTTP queries' queue is in offline mode.
        /// </summary>
        /// <remarks>If this return code is received, no HttpRequest should be sent during 10 seconds which 
        /// is the time the HTTP queue takes to reconnect.
        /// </remarks>
        public const byte Offline = 3;

        /// <summary>
        /// The HTTP request queue is full.
        /// </summary>
        /// <remarks>
        /// This happens if the queue of HTTP queries has reached a certain threshold for the respective application.  
        /// </remarks>
        public const byte QueueFull = 4;

        /// <summary>
        /// An error has occurred while processing the HTTP request. 
        /// </summary>
        /// <remarks>
        /// If the request's URL couldn't be parsed or the hostname couldn't be resolved or
        /// the web service is unreachable. Also this may happen if the endpoint returns an 
        /// error HTTP status code. e.g. 400 (BAD REQUEST)
        /// </remarks>
        public const byte Error = 5;
    }

    /// <summary>
    /// Related to RaiseEvent operation. 
    /// Allows you to control room events events cache by setting the required option. 
    /// Events are cached per event code and player: Event 100 (example!) can be stored once per player.
    /// Cached events can be modified, replaced and removed.</summary>
    /// <remarks>
    /// Caching works only combination with ReceiverGroup options Others and All.
    /// </remarks>
    public class CacheOperations
    {
        /// <summary>Default value (not sent). Event will not be cached.</summary>
        public const byte DoNotCache = 0;
        /// <summary>Adds an event to the room's cache.</summary>
        public const byte AddToRoomCache = 4;
        /// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
        public const byte AddToRoomCacheGlobal = 5;
    }

    /// <summary>
    /// Reasons why an actor was excluded from a room: 
    /// removed from actors list and added to excluded list.
    /// <see cref="RemoveActorReason"/>
    /// </summary>
    public class RemoveActorReason
    {
        /// <summary>
        /// Actor was kicked from the room.
        /// </summary>
        public const byte Kick = 0;
        /// <summary>
        /// Actor was banned from the room.
        /// </summary>
        public const byte Banned = 1;
    }

    /// <summary>
    /// HTTP request to be sent.
    /// </summary>
    public struct HttpRequest
    {
        #region Constants and Fields

        /// <summary>
        /// Accept HTTP header. Specifies what media types are expected from response.
        /// </summary>
        public string Accept;

        /// <summary>
        /// Method that should be called when the HTTP response is received.
        /// </summary>
        public HttpRequestCallback Callback;

        /// <summary>
        /// ContentType HTTP header. Sets the type of the request's data.
        /// </summary>
        public string ContentType;

        /// <summary>
        /// HTTP request body data.
        /// </summary>
        public MemoryStream DataStream;

        /// <summary>
        /// Default predefined HTTP headers.
        /// </summary>
        public IDictionary<HttpRequestHeader, string> Headers;

        /// <summary>
        /// Key/Value strings: custom HTTP headers.
        /// </summary>
        public IDictionary<string, string> CustomHeaders;

        /// <summary>
        /// HTTP verb/method: e.g. GET, POST, PUT, DELETE, etc.
        /// </summary>
        public string Method;

        /// <summary>
        /// HTTP query absolute URL path.
        /// </summary>
        public string Url;

        public object UserState;

        /// <summary>
        /// Indicates whether the process of plugins fiber the should be blocked or not.
        /// </summary>
        public bool Async;

        #endregion
    }

    /// <summary>
    /// Delegate of HTTP request callback.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="userState"></param>
    public delegate void HttpRequestCallback(IHttpResponse response, object userState);

    /// <summary>
    /// Base interface of HTTP response.
    /// </summary>
    public interface IHttpResponse
    {
        #region Properties

        /// <summary>
        /// The corresponding HTTP request.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// The HTTP code returned from external web service.
        /// </summary>
        int HttpCode { get; }

        /// <summary>
        /// The human readable form of the returned <see cref="HttpCode"/>.
        /// </summary>
        string Reason { get; }

        /// <summary>
        /// HTTP response data returned from external web service as byte[].
        /// </summary>
        byte[] ResponseData { get; }

        /// <summary>
        /// HTTP response data returned from external web service as string.
        /// </summary>
        string ResponseText { get; }

        /// <summary>
        /// The <see cref="Photon.Hive.Plugin.HttpRequestQueueResult"/> returned from Photon servers.
        /// </summary>
        byte Status { get; }

        /// <summary>
        /// Reason of failure.
        /// </summary>
        int WebStatus { get; }

        #endregion
    }

    /// <summary>A serializable room actor entry.</summary>
    [Serializable]
    public class SerializableActor
    {
        /// <summary>
        /// Number inside the room of the actor.
        /// </summary>
        public int ActorNr { get; set; }
        /// <summary>
        /// Unique ID of the actor inside the room.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Non-Unique name of the actor inside the room.
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Indicates if the actor is currently joined to the room or not.
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Binary data of the actor.
        /// </summary>
        public string Binary { get; set; }
        /// <summary>
        /// Timestamp of when the actor left the room and became inactive.
        /// </summary>
        public DateTime? DeactivationTime { get; set; }
        /// <summary>
        /// Readable info about the actor for debug purposes.
        /// </summary>
        public Dictionary<byte, object> DEBUG_BINARY { get; set; } 
    }

    /// <summary>A serializable snapshot of the room's full state.</summary>
    [Serializable]
    public class SerializableGameState
    {
        /// <summary>
        /// Incremental counter internally used to save the number of the last joined actor.
        /// </summary>
        public int ActorCounter { get; set; }
        /// <summary>
        /// A list of all actors inside the room (active or inactive).
        /// </summary>
        public List<SerializableActor> ActorList { get; set; }
        /// <summary>
        /// Binary data of the room.
        /// </summary>
        public Dictionary<string, object> Binary { get; set; }
        /// <summary>
        /// Activates UserId checks on joining - allowing a users to be only once in the room. 
        /// Default is deactivated for backwards compatibility, but we recommend to use in future. 
        /// <remarks> Note: Should be active for saved games.</remarks>
        /// </summary>
        public bool CheckUserOnJoin { get; set; }
        /// <summary>
        /// Custom properties visible to the lobby.
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
        /// <summary>
        /// Default: false. If true, server cleans up roomcache of leaving players (their cached events get removed).
        /// </summary>
        public bool DeleteCacheOnLeave { get; set; }
        /// <summary>
        /// TTL (Time To Live) of the room, when empty (no active actors), before it gets removed from server. 
        /// Unless a player joins it meanwhile. 
        /// <remarks>
        /// - 12000 milliseconds.
        /// - Cloud threshold is 300000.
        /// - Default OnPremise threshold is 60000.</remarks>
        /// </summary>
        public int EmptyRoomTTL { get; set; }
        /// <summary>
        /// Defines whether game is open. it is not possible to join to closed game.
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// Defines whether game is visible in lobby or not.
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// The name of the lobby this room belongs to.
        /// </summary>
        public string LobbyId { get; set; }
        /// <summary>
        /// The type of the lobby this room belongs to.
        /// </summary>
        public int LobbyType { get; set; }
        /// <summary>
        /// List of string keys of custom room properties visible to the lobby.
        /// </summary>
        public ArrayList LobbyProperties { get; set; }
        /// <summary>
        /// Maximum of players allowed inside the room.
        /// </summary>
        public byte MaxPlayers { get; set; }
        /// <summary>
        /// TTL (Time To Live) of inactive actors inside the room before they get removed.
        /// </summary>
        public int PlayerTTL { get; set; }
        /// <summary>
        /// Default: false. If set to true, no room events are sent to the clients on join and leave. 
        /// This allows lobby-like rooms with lots of users.
        /// </summary>
        public bool SuppressRoomEvents { get; set; }
        /// <summary>
        /// Cache slice index.
        /// </summary>
        public int Slice { get; set; }
        /// <summary>
        /// Readable form of the room data for debug purposes.
        /// </summary>
        public Dictionary<string, object> DebugInfo { get; set; }
        /// <summary>
        /// A list of actors removed from the room and should not rejoin.
        /// </summary>
        public List<ExcludedActorInfo> ExcludedActors { get; set; }
        /// <summary>
        /// Indicates whether or not UserId of actors should be sent to each other.
        /// </summary>
        public bool PublishUserId { get; set; }
        /// <summary>
        /// TBD: Not used yet.
        /// </summary>
        public List<string> ExpectedUsers { get; set; }
    }


    /// <summary>A serializable container of exluded (explicitly removed) actor info.</summary>
    [Serializable]
    public class ExcludedActorInfo
    {
        /// <summary>Unique User ID of the removed actor.</summary>
        public string UserId { get; set; }
        /// <summary>Code of the reason of the actor's removal from the room. <see cref="RemoveActorReason"/></summary>
        public byte Reason { get; set; }
    }

    /// <summary>Base interface of actor class inside the room.</summary>
    public interface IActor
    {
        // access to properties
        #region Properties

        /// <summary>
        /// Used to identify a player/peer in a game - changes with every new game instances.
        /// </summary>
        int ActorNr { get; }

        /// <summary>
        /// The actor's custom properties.
        /// </summary>
        PropertyBag<object> Properties { get; }

        /// <summary>
        /// Global identify a player/peer in a game. Sent in OpAuthenticate when you connect
        /// When using the CustomAuth feature it can be set in the response of the Auth request.
        /// If the UserId is set in the CustomAuth response the value sent by client is ignored.
        /// </summary>
        string UserId { get; }

#if PLUGINS_0_9
        [Obsolete("Use Nickname instead")]
        string Username { get; }
#endif
        /// <summary>
        /// Non-unique name of the player for display purposes. Equivalent to Playername in most client sdk's.
        /// Former Username in the first release of Webhooks.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// When disconnect Actors they gow through to phases first they are set as inactive, 
        /// when the PlayerTTL elapses they are removed from the list of actors.
        /// A client can rejoin the game using its old actornr.
        /// If the room was created with the RoomOptions.CheckUserOnJoin = true setting.
        /// UserId's are only allowd to join once and on rejoin UserId and actornr have to match.
        /// The actornr on rejoin can be set to -1 then the UserId is used to match the actor.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Now called AuthCookie. 
        /// An encrypted object invisible to client, optionally returned by web service upon successful custom authentication. 
        /// </summary>
        object Secure { get; }

        #endregion
    }

    /// <summary>
    ///  The struct contains the parameters for Photon.SocketServer.PeerBase.SendOperationResponse(Photon.SocketServer.OperationResponse,Photon.SocketServer.SendParameters),
    ///  Photon.SocketServer.PeerBase.SendEvent(Photon.SocketServer.IEventData,Photon.SocketServer.SendParameters)
    ///  and Photon.SocketServer.ServerToServer.S2SPeerBase.SendOperationRequest(Photon.SocketServer.OperationRequest,Photon.SocketServer.SendParameters)
    ///  and contains the info about incoming data at Photon.SocketServer.PeerBase.OnOperationRequest(Photon.SocketServer.OperationRequest,Photon.SocketServer.SendParameters),
    ///  Photon.SocketServer.ServerToServer.S2SPeerBase.OnEvent(Photon.SocketServer.IEventData,Photon.SocketServer.SendParameters)
    ///  and Photon.SocketServer.ServerToServer.S2SPeerBase.OnOperationResponse(Photon.SocketServer.OperationResponse,Photon.SocketServer.SendParameters).
    /// </summary> 
    public struct SendParameters
    {
        /// <summary>
        ///  Gets or sets the channel id for the udp protocol.
        /// </summary>
        public byte ChannelId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data is sent encrypted.
        /// </summary>
        public bool Encrypted { get; set; }
         
        /// <summary>
        /// Gets or sets a value indicating whether to flush all queued data with the
        /// next send.  This overrides the configured send delay.
        /// </summary>
        public bool Flush { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to send the data unreliable.
        /// </summary>
        public bool Unreliable { get; set; }
    }


    /// <summary>
    /// Base interface that should be implemented as a wrapper of the game 
    /// that is hosting the plugin instance. 
    /// </summary>
    public interface IPluginHost
    {
        #region Properties

        /// <summary>
        /// Should contain game and application global properties (AppId, AppVersion, Cloud, Region, etc.)
        /// </summary>
        Dictionary<string,object> Environment { get; }

        /// <summary>
        /// List of all actors in game - including active and inactive.
        /// With each actor having a flag IsInactive <see cref="IActor"/>.
        /// </summary>
        IList<IActor> GameActors { get; }

        /// <summary>
        /// List of active actors in game.
        /// </summary>
        IList<IActor> GameActorsActive { get; }

        /// <summary>
        /// List of inactive active actors in game.
        /// </summary>
        IList<IActor> GameActorsInactive { get; }

        /// <summary>
        /// Unique game identifier. Called Roomname in most client sdk's.
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Game properties as set through the client per RoomOptions.CustomRoomProperties.
        /// </summary>
        Hashtable GameProperties { get; }

        /// <summary>
        /// The game properties that are published to the lobby - as defined by clients per RoomOptions.CustomRoomPropertiesForLobby.
        /// </summary>
        Dictionary<string, object> CustomGameProperties { get; }

        /// <summary>
        /// The actor number of the client defined as master. If the client disconnects a new master client will be selected by photon.
        /// Changes in the master client id are notified per <see cref="PluginBase.OnChangeMasterClientId"/>.
        /// </summary>
        int MasterClientId { get; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Send event to a specific list of actors inside the room or update events cache.
        /// </summary>
        /// <param name="recieverActors">Numbers of actors that should receive the event.</param>
        /// <param name="senderActor">The origin of this event. 
        /// - 0 if it should be the room itself (authorative event).
        /// - an actual actor number to impersonate.</param>
        /// <param name="evCode">Event code.</param>
        /// <param name="data">Event data.</param>
        /// <param name="cacheOp">Caching option. <see cref="CacheOperations"/></param>
        /// <param name="sendParameters"><see cref="SendParameters"/></param>
        void BroadcastEvent(IList<int> recieverActors, int senderActor, byte evCode, Dictionary<byte, object> data, byte cacheOp, SendParameters sendParameters = new SendParameters());

        /// <summary>
        /// Send event to a predefined target group of actors inside the room or update events cache.
        /// </summary>
        /// <param name="target">The group type. <see cref="ReciverGroup"/></param>
        /// <param name="senderActor">The origin of this event. 
        /// - 0 if it should be the room itself (authorative event).
        /// - an actual actor number to impersonate.</param>
        /// <param name="targetGroup">Target interest group.</param>
        /// <param name="evCode">Event code.</param>
        /// <param name="data">Event data.</param>
        /// <param name="cacheOp">Caching option. <see cref="CacheOperations"/></param>
        /// <param name="sendParameters"><see cref="SendParameters"/></param>
        void BroadcastEvent(byte target, int senderActor, byte targetGroup, byte evCode, Dictionary<byte, object> data, byte cacheOp, SendParameters sendParameters = new SendParameters());

        /// <summary>
        /// Used to inform clients of errors. If a callInfo is available in the context of the error then the overload with info should be used.
        /// </summary>
        /// <param name="message">error description</param>
        /// <param name="sendParameters">paramters which defines how event will be sent</param>
        void BroadcastErrorInfoEvent(string message, SendParameters sendParameters = new SendParameters());

        /// <summary>
        /// Used to inform clients of errors.
        /// </summary>
        /// <param name="message">error description</param>
        /// <param name="info">expected to be a callInfo in the error context, used for logging purposes</param>
        /// <param name="sendParameters">paramters which defines how event will be sent</param>
        void BroadcastErrorInfoEvent(string message, ICallInfo info, SendParameters sendParameters = new SendParameters());

        /// <summary>
        /// Creates a timer that triggers a callback once dueTimeMs elapses.
        /// Callbacks are called in the context the room fiber. 
        /// </summary>
        /// <param name="callback">Action called when timer is triggered</param>
        /// <param name="dueTimeMs">Time for callback to elapse</param>
        /// <returns>a timer object which can be canceld using <see cref="StopTimer" /></returns>
        object CreateOneTimeTimer(Action callback, int dueTimeMs);

        /// <summary>
        /// Creates a timers that triggers a callback once dueTimeMs elapses and
        /// repeats the callback every intervalMs.
        /// Callbacks are called in the context the room fiber. 
        /// </summary>
        /// <param name="callback">Action called when timer is triggered</param>
        /// <param name="dueTimeMs">Time for callback to elapse</param>
        /// <param name="intervalMs">intervall for callbacks to be called</param>
        /// <returns>a timer object which can be canceld using <see cref="StopTimer" /></returns>
        object CreateTimer(Action callback, int dueTimeMs, int intervalMs);

#if PLUGINS_0_9
        [ObsoleteAttribute("This method is obsolete. Call GetSerializableGameState() instead.", false)]
        Dictionary<byte, byte[]> GetGameStateAsByteArray();

        [ObsoleteAttribute("This method is obsolete. Call GetSerializableGameState() instead.", false)]
        Dictionary<string, object> GetGameState();
#endif
        /// <summary>
        /// The serializable game state can be used to recreate the room after it gets removed from memory.
        /// Also <see cref="SetGameState"/>.
        /// </summary>
        /// <returns>a serializable game state</returns>
        SerializableGameState GetSerializableGameState();

        /// <summary>
        /// The recommend way to make http requests to external systems.
        /// The http call is completly async, but it returns the result in a fiber compatible way.
        /// There are two modes for recieving a reponse:
        ///  - sync: this means effectivly pausing the fiber until the response is received. 
        ///    And where the callback is called before HttpRequest returns.
        ///  - async: HttpRequest will return immediately. Messages in the room fiber will continue to be processed.
        ///    The response is enqueued in the room fiber.
        /// </summary>
        /// <param name="request"></param>
        void HttpRequest(HttpRequest request);

        /// <summary>
        /// Add log entry at debug level.
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(object message);

        /// <summary>
        /// Add log entry at error level.
        /// </summary>
        /// <param name="message">message to log</param>
        void LogError(object message);

        /// <summary>
        /// Add log entry at fatal level.
        /// </summary>
        /// <param name="message">message to log</param>
        void LogFatal(object message);

        /// <summary>
        /// Add log entry at info level.
        /// </summary>
        /// <param name="message">message to log</param>
        void LogInfo(object message);

        /// <summary>
        /// Add log entry at warning level.
        /// </summary>
        /// <param name="message">message to log</param>
        void LogWarning(object message);

#if PLUGINS_0_9
        [ObsoleteAttribute("This method is obsolete. Call SetGameState(SerializableGameState state) instead.", false)]
        bool SetGameState(Dictionary<byte, byte[]> state);

        [ObsoleteAttribute("This method is obsolete. Call SetGameState(SerializableGameState state) instead.", false)]
        bool SetGameState(Dictionary<string, object> state);
#endif
        /// <summary>
        /// Assign a loaded/external game state to the room.
        /// </summary>
        /// <param name="state">serialzed state to affect to the room</param>
        /// <returns>If the room state was assigned to the room successfully.</returns>
        bool SetGameState(SerializableGameState state);

        /// <summary>
        /// Set properties (well known or custom) of the room or an actor.
        /// </summary>
        /// <param name="actorNr">Set to 0 if the properties belong to the room, 
        /// the target actor number otherwise.</param>
        /// <param name="properties">The properties to be updated. 
        /// Null values to delete. Keys can be existent already or new.</param>
        /// <param name="expected">The properties expected when update occurs. (CAS : "Check And Swap")</param>
        /// <param name="broadcast">Indicates whether to send the PropertiesChanged event to other actors or not.</param>
        /// <returns></returns>
        bool SetProperties(int actorNr, Hashtable properties, Hashtable expected, bool broadcast);

        /// <summary>
        /// Stop a timer.
        /// </summary>
        /// <param name="timer">The timer to stop.</param>
        void StopTimer(object timer);

        /// <summary>
        /// Remove an actor from the room's actors list and add to excluded actors list.
        /// </summary>
        /// <param name="actorNr">The number, inside the room, of the actor to remove.</param>
        /// <param name="reasonDetail">A more detailed message explaining why the actor is removed.</param>
        /// <returns>If the actor removal is successful.</returns>
        bool RemoveActor(int actorNr, string reasonDetail);

        /// <summary>
        /// Remove an actor from the room's actors list and add to excluded actors list.
        /// </summary>
        /// <param name="actorNr">The number, inside the room, of the actor to remove.</param>
        /// <param name="reason">A code for the reason. <see cref="RemoveActorReason"/> </param>
        /// <param name="reasonDetail">A more detailed message explaining why the actor is removed.</param>
        /// <returns>If the actor removal is successful.</returns>
        bool RemoveActor(int actorNr, byte reason, string reasonDetail);
 
        /// <summary>
        /// Registers new types/classes for de/serialization and the fitting methods
        /// to call for this type.
        /// </summary>
        /// <param name="type">custom (class) type to register.</param>
        /// <param name="typeCode">A byte-code used as shortcut during transfer of this type.</param>
        /// <param name="serializeFunction">Serialization method delegate to create a byte[] from a customType instance.</param>
        /// <param name="deserializeFunction">Deserliazation Method delegate to create customType instance from a byte[].</param>
        /// <returns>If the Type was registered successfully.</returns>
        /// <remarks>
        /// SerializeMethod and DeserializeMethod are complementary: Feed the product
        /// of serializeMethod to the constructor, to get a comparable instance of the
        /// object.  After registering a Type, it can be used in events and operations
        /// and will be serialized like built-in types.
        /// </remarks>
        bool TryRegisterType(Type type, byte typeCode, Func<object, byte[]> serializeFunction, Func<byte[], object> deserializeFunction);


        /// <summary>
        /// Returns <see cref="EnvironmentVersion"/> object which contains version of PhotonHivePlugin at build time and currently running
        /// </summary>
        EnvironmentVersion GetEnvironmentVersion();

        #endregion
    }

    /// <summary>
    /// Base interface that should be implemented by any plugin class.
    /// </summary>
    public interface IGamePlugin
    {
        /// <summary>
        /// Name of the plugin. Default is not allowed.
        /// This should be the name used when requesting the plugin from the client in CreateGame operation.
        /// This will be returned to the client in the CreateGame operation response.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version of the plugin.
        /// This will be returned to the client in the CreateGame operation response.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Flag used with webhooks plugin to indicate whether or not rooms should be persisted between connections.
        /// If true, SerializedGameState should be sent to web service before removing the room from memory.
        /// Also it could be loaded in OnCreateGame or BeforeJoin.
        /// </summary>
        bool IsPersistent { get; }

        #region Public Methods
        
        /// <summary>
        /// Plugin callback called when a game instance is about to be removed from Photon servers memory.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void BeforeCloseGame(IBeforeCloseGameCallInfo info);
        
        /// <summary>
        /// Plugin callback called when a peer is about to join a room.
        /// This is triggered by Op Join when a game instance is in Photon servers memory.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void BeforeJoin(IBeforeJoinGameCallInfo info);
        
        /// <summary>
        /// Plugin callback triggered by Op SetProperties.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void BeforeSetProperties(IBeforeSetPropertiesCallInfo info);
        
        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeCloseGame"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnCloseGame(ICloseGameCallInfo info);
        
        /// <summary>
        /// Plugin callback called when a game instance is about to be created on server.
        /// It can be triggered by Op CreateGame or Op JoinGame if JoinMode.CreateIfNotExists, JoinOrRejoin or RejoinOnly.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnCreateGame(ICreateGameCallInfo info);

#if PLUGINS_0_9
        #region Obsolete
        [Obsolete]
        void OnDisconnect(IDisconnectCallInfo info);
        #endregion //Obsolete
#endif

        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeJoin"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnJoin(IJoinGameCallInfo info);

        /// <summary>
        /// Plugin callback when a peer is disconnected from the room.
        /// The corresponding actor is either removed or marked as inactive.
        /// This can be triggered by an explicit or unexpected Disconnect or a call to Op Leave or RemoveActor.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnLeave(ILeaveGameCallInfo info);

        /// <summary>
        /// Plugin callback when Op RaiseEvent is called.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnRaiseEvent(IRaiseEventCallInfo info);

        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeSetProperties"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void OnSetProperties(ISetPropertiesCallInfo info);

        /// <summary>
        /// Callback triggered when trying to deseriliaze unknwon type.
        /// </summary>
        /// <param name="type">The Type of the object.</param>
        /// <param name="value">The object with unknown type.</param>
        /// <returns>If the unkown type could be handled successfully.</returns>
        bool OnUnknownType(Type type, ref object value);

        /// <summary>
        /// Initialize plugin instance.
        /// </summary>
        /// <param name="host">The game hosting the plugin.</param>
        /// <param name="config">The plugin assembly key/value configuration entries.</param>
        /// <param name="errorMsg">Error message in case something wrong happens when setting up the plugin instance.</param>
        /// <returns>If the plugin instance setup is successful.</returns>
        bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg);
        
        /// <summary>
        /// Callback to report an internal plugin error.
        /// </summary>
        /// <param name="errorCode">Code of the error. <see cref="Photon.Hive.Plugin.ErrorCodes"/></param>
        /// <param name="e">Exception thrown.</param>
        /// <param name="state">Optional object to be added in the report. It could help in debugging the error.</param>
        void ReportError(short errorCode, Exception e, object state = null);

#if PLUGINS_0_9
        #region Obsolete
        [Obsolete]
        bool SetupInstance(IPluginHost host, Dictionary<string, string> config);
        [Obsolete]
        void OnJoin(IJoinCallInfo info);
        [Obsolete]
        void OnLeave(ILeaveCallInfo info);
        #endregion //Obsolete
#endif
        #endregion
    }

    /// <summary>
    /// Join options used in <see cref="Photon.Hive.Plugin.IJoinGameCallInfo"/>.
    /// </summary>
    public class ProcessJoinParams
    {
        #region Constructors and Destructors

        /// <summary>Constructor.</summary>
        /// <remarks>All properties are initialized to true.</remarks>
        public ProcessJoinParams()
        {
            this.PublishCache = true;
            this.PublishJoinEvents = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether to send any cached events to the joining actor or not.
        /// </summary>
        public bool PublishCache { get; set; }

        /// <summary>
        /// Indicates whether to broadcast actor join event or not.
        /// </summary>
        public bool PublishJoinEvents { get; set; }

        /// <summary>
        /// Additional data to be returned to client in Join operation response.
        /// </summary>
        public Dictionary<byte, object> ResponseExtraParameters { get; set; }

        #endregion
    }
}
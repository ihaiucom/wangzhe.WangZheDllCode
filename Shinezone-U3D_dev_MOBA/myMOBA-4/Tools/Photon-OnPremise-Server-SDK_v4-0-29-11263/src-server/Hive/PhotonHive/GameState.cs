using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Photon.Hive.Caching;
using Photon.Hive.Collections;
using Photon.Hive.Common.Lobby;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.Hive.Serialization;

namespace Photon.Hive
{
    public enum HiveHostGameState
    {
        ActorCounter,
        ActorList,
        Binary,
        CheckUserOnJoin,
        CustomProperties,
        DeleteCacheOnLeave,
        EmptyRoomTTL,
        IsOpen,
        IsVisible,
        LobbyId,
        LobbyType,
        LobbyProperties,
        MaxPlayers,
        PlayerTTL,
        SuppressRoomEvents,
        Slice,
    }

    public class GameState 
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   Gets a PropertyBag instance used to store custom room properties.
        /// </summary>
        public PropertyBag<object> Properties { get; private set; }

        public int EmptyRoomLiveTime { get; set; }

        private readonly RoomEventCacheManager eventCache = new RoomEventCacheManager();

        /// <summary> 
        ///   Contains <see cref = "Caching.EventCache" />s for all actors.
        /// </summary>
        private readonly EventCacheDictionary actorEventCache = new EventCacheDictionary();

        private readonly GroupManager groupManager = new GroupManager();
        protected ActorsManager actorsManager;

        public GameState()
        {
            this.actorsManager = new ActorsManager();
            this.Properties = new PropertyBag<object>();
            this.MaxPlayers = 0;
        }

        public bool IsOpen
        {
            get
            {
                var property = this.Properties.GetProperty((byte)GameParameter.IsOpen);
                return property == null || (bool)property.Value;
            }
            set
            {
                this.Properties.Set((byte)GameParameter.IsOpen, value);
            }
        }
        public bool IsVisible
        {
            get
            {
                var property = this.Properties.GetProperty((byte)GameParameter.IsVisible);
                return property == null || (bool)property.Value;
            }
            set
            {
                this.Properties.Set((byte)GameParameter.IsVisible, value);
            }
        }
        public string LobbyId { get; set; }
        public bool CheckUserOnJoin { get; set; }
        public bool PublishUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cached events are automaticly deleted for 
        /// actors which are leaving a room.
        /// </summary>
        public bool DeleteCacheOnLeave { get; set; }

        /// <summary>
        /// Contains the keys of the game properties hashtable which should be listet in the lobby.
        /// </summary>
        public HashSet<object> LobbyProperties { get; set; }

        public AppLobbyType LobbyType { get; set; }

        public byte MaxPlayers
        {
            get
            {
                var property = this.Properties.GetProperty((byte) GameParameter.MaxPlayers);
                if (property != null)
                {
                    return (byte) property.Value;
                }
                return 0;
            }
            set
            {
                this.Properties.Set((byte) GameParameter.MaxPlayers, value);
            }
        }

        /// <summary>
        /// Player live time
        /// </summary>
        public int PlayerTTL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if common room events (Join, Leave) will suppressed.
        /// </summary>
        public bool SuppressRoomEvents { get; set; }

        public RoomEventCacheManager EventCache
        {
            get { return this.eventCache; }
        }

        /// <summary> 
        ///   Contains <see cref = "Caching.EventCache" />s for all actors.
        /// </summary>
        public EventCacheDictionary ActorEventCache
        {
            get { return this.actorEventCache; }
        }

        public GroupManager GroupManager
        {
            get { return this.groupManager; }
        }

        public ActorsManager ActorsManager
        {
            get { return this.actorsManager; }
        }

        public bool SetState(SerializableGameState state)
        {
            this.ActorsManager.ActorNumberCounter = state.ActorCounter;
            if (state.ActorList != null)
            {
                this.ActorsManager.DeserializeActors(state.ActorList);
            }

            if (!this.SetGameStateUencodedBinaryPart(state.Binary))
            {
                return false;
            }

            // - we now inlcude all properties in the binary state
            // - and decided it was confusing to loose type information
            // - so this filed is only for "Info" purposes
            // - only ignoring for now.

            this.CheckUserOnJoin = state.CheckUserOnJoin;
            this.DeleteCacheOnLeave = state.DeleteCacheOnLeave;

            this.EmptyRoomLiveTime = state.EmptyRoomTTL;
            this.IsOpen = state.IsOpen;
            this.IsVisible = state.IsVisible;
            this.PublishUserId = state.PublishUserId;

            this.LobbyId = state.LobbyId;
            this.LobbyType = (AppLobbyType)state.LobbyType;
            if (state.LobbyProperties != null)
            {
                this.LobbyProperties = new HashSet<object>(state.LobbyProperties.ToArray());
            }

            this.MaxPlayers = state.MaxPlayers;

            this.PlayerTTL = state.PlayerTTL;
            this.SuppressRoomEvents = state.SuppressRoomEvents;
            this.EventCache.Slice = state.Slice;

            this.Properties.Set((byte) GameParameter.MasterClientId, 0);

            this.ActorsManager.ExcludedActors = state.ExcludedActors ?? new List<ExcludedActorInfo>();
            this.ActorsManager.ExpectedUsers = state.ExpectedUsers ?? new List<string>();
            return true;
        }

        public bool SetState(Dictionary<string, object> state)
        {
            if (state.Keys.Contains("0"))
            {
                Log.ErrorFormat("Old style of serializaed data are used");
                return false;
            }

            var serializedState = new SerializableGameState();

            foreach (var entry in state)
            {
                // TBD - improve performance using a Dictionary<String,YourEnum>,
                // see http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c
                HiveHostGameState key;
                try
                {
                    key = (HiveHostGameState)Enum.Parse(typeof(HiveHostGameState), entry.Key, false);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                switch (key)
                {
                    case HiveHostGameState.ActorCounter:
//                        this.ActorsManager.ActorNumberCounter = (int)entry.Value;
                        serializedState.ActorCounter = (int) entry.Value;
                        break;
                    case HiveHostGameState.ActorList:
                    {
                        //var list = entry.Value as IList;
                        //this.ActorsManager.DeserializeActors(list);
                        var list = entry.Value as IList;

                        serializedState.ActorList = list.Cast<Dictionary<string, object>>().Select(d => d.ToSerializableActor()).ToList();
                    }
                        break;
                    case HiveHostGameState.CustomProperties:
                        // TBD - we now inlcude all properties in the binary state
                        //     - and decided it was confusing to loose type information
                        //     - so this filed is only for "Info" purposes
                        //     - only ignoring for now.
                        // this.Properties.SetProperties((IDictionary)entry.Value);
                        break;
                    case HiveHostGameState.CheckUserOnJoin:
                        //this.CheckUserOnJoin = (bool)entry.Value;
                        serializedState.CheckUserOnJoin = (bool) entry.Value;
                        break;
                    case HiveHostGameState.DeleteCacheOnLeave:
                        //this.DeleteCacheOnLeave = (bool)entry.Value;
                        serializedState.DeleteCacheOnLeave = (bool) entry.Value;
                        break;
                    case HiveHostGameState.EmptyRoomTTL:
//                        this.EmptyRoomLiveTime = (int)entry.Value;
                        serializedState.EmptyRoomTTL = (int)entry.Value;
                        break;
                    case HiveHostGameState.IsOpen:
//                        this.IsOpen = (bool)entry.Value;
                        serializedState.IsOpen = (bool)entry.Value;
                        break;
                    case HiveHostGameState.IsVisible:
                        //this.IsVisible = (bool)entry.Value;
                        serializedState.IsVisible = (bool)entry.Value;
                        break;
                    case HiveHostGameState.LobbyId:
                        serializedState.LobbyId = (string)entry.Value;
//                        this.LobbyId = (string)entry.Value;
                        break;
                    case HiveHostGameState.LobbyType:
                        serializedState.LobbyType = (int)(AppLobbyType)entry.Value;
//                        this.LobbyType = (AppLobbyType)entry.Value;
                        break;
                    case HiveHostGameState.LobbyProperties:
                        //var lobbyProperties = entry.Value as ArrayList;
                        //if (lobbyProperties != null)
                        //{
                        //    this.LobbyProperties = new HashSet<object>((lobbyProperties).ToArray());
                        //}
                        serializedState.LobbyProperties = entry.Value as ArrayList;
                        break;
                    case HiveHostGameState.MaxPlayers:
//                        this.MaxPlayers = Convert.ToByte(entry.Value);
                        serializedState.MaxPlayers = Convert.ToByte(entry.Value);
                        break;
                    case HiveHostGameState.PlayerTTL:
//                        this.PlayerTTL = (int)entry.Value;
                        serializedState.PlayerTTL = (int)entry.Value;
                        break;
                    case HiveHostGameState.SuppressRoomEvents:
//                        this.SuppressRoomEvents = (bool)entry.Value;
                        serializedState.SuppressRoomEvents = (bool)entry.Value;
                        break;
                    case HiveHostGameState.Slice:
                        serializedState.Slice = (int)entry.Value;
//                        this.EventCache.Slice = (int)entry.Value;
                        break;
                    case HiveHostGameState.Binary:
                        //var uencodedBinaryState = (Dictionary<string, object>)entry.Value;
                        //this.SetGameStateUencodedBinaryPart(uencodedBinaryState);
                        serializedState.Binary = (Dictionary<string, object>)entry.Value;
                        break;
                }
            }

            return SetState(serializedState);
        }

        bool SetGameStateUencodedBinaryPart(Dictionary<string, object> uencodedState)
        {
            if (uencodedState == null)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("state without binary part");
                }
                return true;
            }

            foreach (var entry in uencodedState)
            {
                if (!this.SetGameStateBinaryPart(Convert.ToByte(entry.Key), Serializer.DeserializeBase64(entry.Value.ToString())))
                {
                    return false;
                }
            }

            return true;
        }

        bool SetGameStateBinaryPart(byte key, object deserializedData)
        {
            switch (key)
            {
                case 18: // properties new
                {
                    var dict = deserializedData as IDictionary;
                    this.Properties.SetProperties(dict);
                }
                    break;
                case 19: // EventCache
                {
                    this.EventCache.SetDeserializedData(deserializedData as Dictionary<int, object[]>);
                }
                    break;
                case 20: // Groups
                {
                    var dict = deserializedData as Dictionary<byte, object[]>;
                    if (dict != null)
                    {
                        foreach (var group in dict)
                        {
                            foreach (int actorNr in group.Value)
                            {
                                this.GroupManager.AddActorToGroup(group.Key, this.ActorsManager.InactiveActorsGetActorByNumber(actorNr));
                            }
                        }
                    }
                    else
                    {
                        Log.ErrorFormat("Can't get groups from deserialized data");
                        return false;
                    }
                }
                    break;
            }


            return true;
        }

        private Dictionary<string, object> GetBinaryPartOfGameState(Dictionary<object, object> properties, 
            ref Dictionary<byte, ArrayList> actorGroups, out int evCount,out Dictionary<int, ArrayList> events)
        {
            var binary = new Dictionary<string, object>();

            if (this.GroupManager.Count > 0)
            {
                actorGroups = this.GroupManager.GetDataForSerialization();
                binary.Add("20", Serializer.SerializeBase64(actorGroups));
            }

            events = this.EventCache.GetSerializationData(out evCount);
            if (evCount > 0)
            {
                binary.Add("19", Serializer.SerializeBase64(events));
            }
            if (properties.Count > 0)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>" + Serializer.SerializeBase64(properties));
                binary.Add("18", Serializer.SerializeBase64(properties));
            }
            return binary;
        }

        public SerializableGameState GetSerializableGameState()
        {
            const bool withDebugInfo = true;

            Dictionary<string, object> customProperties;
            ArrayList lobbyProperties;
            var properties = PrepareProperties(out customProperties, out lobbyProperties);

            Dictionary<byte, ArrayList> actorGroups = null;
            int evCount;
            Dictionary<int, ArrayList> events;
            var binary = GetBinaryPartOfGameState(properties, ref actorGroups, out evCount, out events);

            var state = new SerializableGameState
            {
                ActorCounter = this.ActorsManager.ActorNumberCounter,
                ActorList = this.ActorsManager.SerializeActors(withDebugInfo),
                CheckUserOnJoin = this.CheckUserOnJoin,
                CustomProperties = customProperties,
                DeleteCacheOnLeave = this.DeleteCacheOnLeave,
                EmptyRoomTTL = this.EmptyRoomLiveTime,
                IsOpen = this.IsOpen,
                IsVisible = this.IsVisible,
                LobbyId = this.LobbyId,
                LobbyType = (int) this.LobbyType,
                LobbyProperties = lobbyProperties,
                MaxPlayers = this.MaxPlayers,
                PlayerTTL = this.PlayerTTL,
                SuppressRoomEvents = this.SuppressRoomEvents,
                Slice = this.EventCache.Slice,
                Binary = binary,
                ExcludedActors = this.actorsManager.ExcludedActors,
                ExpectedUsers = this.ActorsManager.ExpectedUsers,

            };

            if (withDebugInfo)
            {
                state.DebugInfo = new Dictionary<string, object>();
                if (properties.Count > 0)
                {
                    state.DebugInfo.Add("DEBUG_PROPERTIES_18", properties);
                }
                if (evCount > 0)
                {
                    state.DebugInfo.Add("DEBUG_EVENTS_19", events);
                }
                if (actorGroups != null && actorGroups.Count > 0)
                {
                    state.DebugInfo.Add("DEBUG_GROUPS_20", actorGroups);
                }
            }

            return state;
        }

        public Dictionary<string, object> GetState()
        {
            //const bool withDebugInfo = true;

            //var actorList = this.ActorsManager.SerializeActors(withDebugInfo);

            //Dictionary<string, object> customProperties;
            //ArrayList lobbyProperties;
            //var properties = PrepareProperties(out customProperties, out lobbyProperties);

            //Dictionary<byte, ArrayList> actorGroups = null;
            //int evCount;
            //Dictionary<int, ArrayList> events;
            //var binary = GetBinaryPartOfGameState(properties, ref actorGroups, out evCount, out events);

            //var state = new Dictionary<string, object>
            //{
            //    {HiveHostGameState.ActorCounter.ToString(), this.ActorsManager.ActorNumberCounter}
            //};
            //if (actorList.Count > 0)
            //{
            //    state.Add(HiveHostGameState.ActorList.ToString(), actorList);
            //}
            //state.Add(HiveHostGameState.CheckUserOnJoin.ToString(), this.CheckUserOnJoin);
            //if (customProperties.Count > 0)
            //{
            //    state.Add(HiveHostGameState.CustomProperties.ToString(), customProperties);
            //}
            //state.Add(HiveHostGameState.DeleteCacheOnLeave.ToString(), this.DeleteCacheOnLeave);
            //state.Add(HiveHostGameState.EmptyRoomTTL.ToString(), this.EmptyRoomLiveTime);
            //state.Add(HiveHostGameState.IsOpen.ToString(), this.IsOpen);
            //state.Add(HiveHostGameState.IsVisible.ToString(), this.IsVisible);
            //state.Add(HiveHostGameState.LobbyId.ToString(), this.LobbyId);
            //state.Add(HiveHostGameState.LobbyType.ToString(), (int)this.LobbyType);
            //state.Add(HiveHostGameState.LobbyProperties.ToString(), lobbyProperties);
            //state.Add(HiveHostGameState.MaxPlayers.ToString(), this.MaxPlayers);
            //state.Add(HiveHostGameState.PlayerTTL.ToString(), this.PlayerTTL);
            //state.Add(HiveHostGameState.SuppressRoomEvents.ToString(), this.SuppressRoomEvents);
            //state.Add(HiveHostGameState.Slice.ToString(), this.EventCache.Slice);
            //state.Add(HiveHostGameState.Binary.ToString(), binary);

            //if (withDebugInfo)
            //{
            //    if (properties.Count > 0)
            //    {
            //        state.Add("DEBUG_PROPERTIES_18", properties);
            //    }
            //    if (evCount > 0)
            //    {
            //        state.Add("DEBUG_EVENTS_19", events);
            //    }
            //    if (actorGroups != null && actorGroups.Count > 0)
            //    {
            //        state.Add("DEBUG_GROUPS_20", actorGroups);
            //    }
            //}

            var serializableState = this.GetSerializableGameState();
            var withDebugInfo = true;
            var state = new Dictionary<string, object>
            {
                {HiveHostGameState.ActorCounter.ToString(), this.ActorsManager.ActorNumberCounter}
            };
            if (serializableState.ActorList.Count > 0)
            {
                var l = serializableState.ActorList.Select(actor => actor.ToDictionary()).ToList();
                state.Add(HiveHostGameState.ActorList.ToString(), l);
            }
            state.Add(HiveHostGameState.CheckUserOnJoin.ToString(), serializableState.CheckUserOnJoin);
            if (serializableState.CustomProperties.Count > 0)
            {
                state.Add(HiveHostGameState.CustomProperties.ToString(), serializableState.CustomProperties);
            }
            state.Add(HiveHostGameState.DeleteCacheOnLeave.ToString(), serializableState.DeleteCacheOnLeave);
            state.Add(HiveHostGameState.EmptyRoomTTL.ToString(), serializableState.EmptyRoomTTL);
            state.Add(HiveHostGameState.IsOpen.ToString(), serializableState.IsOpen);
            state.Add(HiveHostGameState.IsVisible.ToString(), serializableState.IsVisible);
            state.Add(HiveHostGameState.LobbyId.ToString(), serializableState.LobbyId);
            state.Add(HiveHostGameState.LobbyType.ToString(), serializableState.LobbyType);
            state.Add(HiveHostGameState.LobbyProperties.ToString(), serializableState.LobbyProperties);
            state.Add(HiveHostGameState.MaxPlayers.ToString(), serializableState.MaxPlayers);
            state.Add(HiveHostGameState.PlayerTTL.ToString(), serializableState.PlayerTTL);
            state.Add(HiveHostGameState.SuppressRoomEvents.ToString(), serializableState.SuppressRoomEvents);
            state.Add(HiveHostGameState.Slice.ToString(), serializableState.Slice);
            state.Add(HiveHostGameState.Binary.ToString(), serializableState.Binary);

            if (withDebugInfo)
            {

                if (serializableState.DebugInfo.ContainsKey("DEBUG_PROPERTIES_18"))
                {
                    state.Add("DEBUG_PROPERTIES_18", serializableState.DebugInfo["DEBUG_PROPERTIES_18"]);
                }
                if (serializableState.DebugInfo.ContainsKey("DEBUG_EVENTS_19"))
                {
                    state.Add("DEBUG_EVENTS_19", serializableState.DebugInfo["DEBUG_EVENTS_19"]);
                }
                if (serializableState.DebugInfo.ContainsKey("DEBUG_GROUPS_20"))
                {
                    state.Add("DEBUG_GROUPS_20", serializableState.DebugInfo["DEBUG_GROUPS_20"]);
                }
            }
            return state;
        }

        private Dictionary<object, object> PrepareProperties(out Dictionary<string, object> customProperties, out ArrayList lobbyProperties)
        {
            var properties = new Dictionary<object, object>();
            customProperties = new Dictionary<string, object>();

            lobbyProperties = this.LobbyProperties != null ? new ArrayList(this.LobbyProperties.ToArray()) : null;

            foreach (var prop in this.Properties.AsDictionary())
            {
                if (this.LobbyProperties != null && this.LobbyProperties.Contains(prop.Key))
                {
                    customProperties.Add((string)prop.Key, prop.Value.Value);
                }

                // we decided to inlcude all properties binary
                // we could probably get rid of the wellknown properties
                if (prop.Key is byte 
                    && ((byte)prop.Key >= (byte)GameParameter.MinValue && ((byte)prop.Key != (byte)GameParameter.LobbyProperties)))
                {
                    continue;
                }

                properties.Add(prop.Key, prop.Value.Value);
            }
            return properties;
        }
    }
}
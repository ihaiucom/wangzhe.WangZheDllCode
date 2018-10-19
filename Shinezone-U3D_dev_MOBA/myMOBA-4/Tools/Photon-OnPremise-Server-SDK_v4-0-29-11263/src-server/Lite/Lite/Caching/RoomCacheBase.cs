// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomCacheBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Base class for room caches.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Caching
{
    #region using directives

    using System;
    using System.Collections.Generic;
    using System.Text;
    using ExitGames.Logging;
    using Lite.Diagnostics.OperationLogging;
    using Photon.SocketServer;

    #endregion

    /// <summary>
    /// Base class for room caches.
    /// </summary>
    public abstract class RoomCacheBase
    {
        /// <summary>
        /// An <see cref="ILogger"/> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>A Dictionary used to store room instances.</summary>
        protected readonly Dictionary<string, RoomInstance> RoomInstances = new Dictionary<string, RoomInstance>();

        /// <summary>used to syncronize acces to the cache.</summary>
        protected readonly object SyncRoot = new object();

        /// <summary>
        /// Tries to get room reference for a room with the specified id, without holding a reference to that room. 
        /// </summary>
        /// <param name="roomId">The room id.</param>
        /// <param name="room">The room, in case it exists.</param>
        /// <returns>
        /// True if the cache contains a room with the specified room id; otherwise, false.
        /// </returns>
        public bool TryGetRoomWithoutReference(string roomId, out Room room)
        {
            lock (this.SyncRoot)
            {
                RoomInstance roomInstance;
                if (!this.RoomInstances.TryGetValue(roomId, out roomInstance))
                {
                    room = null;
                    return false;
                }

                room = roomInstance.Room;
                return true;
            }
        }

        /// <summary>
        /// Gets a room reference for a room with a specified id.
        /// If the room with the specified id does not exists, a new room will be created.
        /// </summary>
        /// <param name="roomName">
        /// The room id.
        /// </param>
        /// <param name="ownerPeer">
        /// The peer that holds this reference.
        /// </param>
        /// <param name="args">
        /// Optionally arguments used for room creation.
        /// </param>
        /// <returns>
        /// a <see cref="RoomReference"/>
        /// </returns>
        public RoomReference GetRoomReference(string roomName, PeerBase ownerPeer, params object[] args)
        {
            lock (this.SyncRoot)
            {
                RoomInstance roomInstance;
                if (!this.RoomInstances.TryGetValue(roomName, out roomInstance))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Creating room instance: roomName={0}", roomName);
                    }

                    Room room = this.CreateRoom(roomName, args);
                    roomInstance = new RoomInstance(this, room);
                    this.RoomInstances.Add(roomName, roomInstance);
                }

                return roomInstance.AddReference(ownerPeer);
            }
        }

        /// <summary>
        /// Returns the names of all rooms that are currently cached in this <see cref="RoomCacheBase"/>.
        /// </summary>
        /// <returns>The list of room names.</returns>
        public List<string> GetRoomNames()
        {
            lock (this.SyncRoot)
            {
                return new List<string>(this.RoomInstances.Keys);
            }
        }

        /// <summary>
        /// Gathers debug information about the specified room (actors, peers, references etc.). 
        /// </summary>
        /// <param name="roomName">The room name.</param>
        /// <returns>A string with debug information.</returns>
        public virtual string GetDebugString(string roomName)
        {
            lock (this.SyncRoot)
            {
                RoomInstance roomInstance;
                if (!this.RoomInstances.TryGetValue(roomName, out roomInstance))
                {
                    return string.Format("RoomCache: No entry for room name {0}", roomName);
                }

                return string.Format("RoomCache: RoomInstance entry found for room {0}: {1}", roomName, roomInstance);
            }
        }

        /// <summary>
        /// Tries to create a new room.
        /// </summary>
        /// <param name="roomName">
        /// The room id.
        /// </param>
        /// <param name="ownerPeer">
        /// The peer that holds this reference.
        /// </param>
        /// <param name="roomReference">
        /// When this method returns true, contains a new <see cref="RoomReference"/> for the room 
        /// with the specified room id; otherwise, set to null. 
        /// </param>
        /// <param name="args">
        /// Optionally arguments used for room creation.
        /// </param>
        /// <returns>
        /// False if the cache contains a room with the specified room id; otherwise, true.
        /// </returns>
        public bool TryCreateRoom(string roomName, PeerBase ownerPeer, out RoomReference roomReference, params object[] args)
        {
            lock (this.SyncRoot)
            {
                if (this.RoomInstances.ContainsKey(roomName))
                {
                    roomReference = null;
                    return false;
                }

                Room room = this.CreateRoom(roomName, args);
                var roomInstance = new RoomInstance(this, room);
                this.RoomInstances.Add(roomName, roomInstance);
                roomReference = roomInstance.AddReference(ownerPeer);
                return true;
            }
        }

        /// <summary>
        /// Tries to get room reference for a room with the specified id. 
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="ownerPeer">
        /// The peer that holds this reference.
        /// </param>
        /// <param name="roomReference">
        /// When this method returns true, contains a new <see cref="RoomReference"/> for the room 
        /// with the specified room id; otherwise, set to null. 
        /// </param>
        /// <returns>
        /// True if the cache contains a room with the specified room id; otherwise, false.
        /// </returns>
        public bool TryGetRoomReference(string roomId, PeerBase ownerPeer, out RoomReference roomReference)
        {
            lock (this.SyncRoot)
            {
                RoomInstance roomInstance;
                if (!this.RoomInstances.TryGetValue(roomId, out roomInstance))
                {
                    roomReference = null;
                    return false;
                }

                roomReference = roomInstance.AddReference(ownerPeer);
                return true;
            }
        }

        /// <summary>
        /// Releases a room reference. 
        /// The related room instance will be removed from the cache if 
        /// no more references to the room exists.
        /// </summary>
        /// <param name="roomReference">
        /// The room reference to relaease.
        /// </param>
        public void ReleaseRoomReference(RoomReference roomReference)
        {
            Room room;

            lock (this.SyncRoot)
            {
                RoomInstance roomInstance;
                if (!this.RoomInstances.TryGetValue(roomReference.Room.Name, out roomInstance))
                {
                    return;
                }

                roomInstance.ReleaseReference(roomReference);

                // if there are still references to the room left 
                // the room stays into the cache
                if (roomInstance.ReferenceCount > 0)
                {
                    return;
                }

                // ask the room implementation if the room should be 
                // removed automaticly from the cache
                var shouldRemoveRoom = roomInstance.Room.BeforeRemoveFromCache();
                if (shouldRemoveRoom == false)
                {
                    return;
                }

                this.RoomInstances.Remove(roomInstance.Room.Name);
                room = roomInstance.Room;
            }

            if (room == null)
            {
                // the room hast not been removed from the cache
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Removed room instance: roomId={0}", room.Name);
            }

            room.Dispose();
            this.OnRoomRemoved(room);
        }

        /// <summary>
        /// Tries to remove a romm instance from the room cache. 
        /// The room will only be removed if there are no references to the romm instance left.
        /// </summary>
        /// <param name="room">
        /// The room to remove.
        /// </param>
        /// <returns>
        /// Returns true if the room was removed from the cache; otherwise false.
        /// </returns>
        public bool TryRemoveRoomInstance(Room room)
        {
            RoomInstance roomInstance;

            lock (this.SyncRoot)
            {
                if (this.RoomInstances.TryGetValue(room.Name, out roomInstance) == false)
                {
                    return false;
                }

                if (roomInstance.ReferenceCount > 0)
                {
                    return false;
                }

                this.RoomInstances.Remove(roomInstance.Room.Name);
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Removed room instance: roomId={0}", roomInstance.Room.Name);
            }

            roomInstance.Room.Dispose();
            this.OnRoomRemoved(roomInstance.Room);
            return true;
        }

        /// <summary>
        /// Must be implementated by inheritors to create new room instances.
        /// This method is called when a room reference is requesteted for a
        /// room that does not exists in the cache.
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="args">
        /// Optionally arguments used for room creation.
        /// </param>
        /// <returns>
        /// a new room
        /// </returns>
        protected abstract Room CreateRoom(string roomId, params object[] args);

        /// <summary>
        /// Invoked if the last reference for a room is released and the room was removed from the cache. 
        /// Can be overloaded by inheritors to provide a custom cleanup logic after a room has been disposed. 
        /// </summary>
        /// <param name="room">The <see cref="Room"/> that was removed from the cache.</param>
        protected virtual void OnRoomRemoved(Room room)
        {
        }

        /// <summary>
        /// Used to track references for a room instance.
        /// </summary>
        protected class RoomInstance
        {
            /// <summary>
            /// The references.
            /// </summary>
            private readonly Dictionary<Guid, RoomReference> references;

            /// <summary>
            /// The room factory.
            /// </summary>
            private readonly RoomCacheBase roomFactory;

            private readonly LogQueue logQueue;

            /// <summary>
            /// Initializes a new instance of the <see cref="RoomInstance"/> class.
            /// </summary>
            /// <param name="roomFactory">
            /// The room factory.
            /// </param>
            /// <param name="room">
            /// The room.
            /// </param>
            public RoomInstance(RoomCacheBase roomFactory, Room room)
            {
                this.roomFactory = roomFactory;
                this.Room = room;
                this.references = new Dictionary<Guid, RoomReference>();
                this.logQueue = new LogQueue("RoomInstance " + room.Name, LogQueue.DefaultCapacity);
            }

            /// <summary>
            /// Gets the number of references for the room instance.
            /// </summary>
            public int ReferenceCount
            {
                get
                {
                    return this.references.Count;
                }
            }

            /// <summary>
            /// Gets the room.
            /// </summary>
            public Room Room { get; private set; }

            /// <summary>
            /// Adds a reference to the room instance.
            /// </summary>
            /// <param name="ownerPeer">
            /// The peer that holds this reference.
            /// </param>
            /// <returns>
            /// a new <see cref="RoomReference"/>
            /// </returns>
            public RoomReference AddReference(PeerBase ownerPeer)
            {
                var reference = new RoomReference(this.roomFactory, this.Room, ownerPeer);
                this.references.Add(reference.Id, reference);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(
                        "Created room instance reference: roomName={0}, referenceCount={1}",
                        this.Room.Name,
                        this.ReferenceCount);
                }

                if (this.logQueue.Log.IsDebugEnabled)
                {
                    this.logQueue.Add(
                        new LogEntry(
                            "AddReference",
                            string.Format(
                                "RoomName={0}, ReferenceCount={1}, OwnerPeer={2}",
                                this.Room.Name,
                                this.ReferenceCount,
                                ownerPeer)));
                }
                
                return reference;
            }

            /// <summary>
            /// Releases a reference from this instance.
            /// </summary>
            /// <param name="reference">
            /// The room reference.
            /// </param>
            public void ReleaseReference(RoomReference reference)
            {
                this.references.Remove(reference.Id);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat(
                        "Removed room instance reference: roomName={0}, referenceCount={1}",
                        this.Room.Name,
                        this.ReferenceCount);
                }

                if (this.logQueue.Log.IsDebugEnabled)
                {

                    this.logQueue.Add(
                        new LogEntry(
                            "ReleaseReference",
                            string.Format(
                                "RoomName={0}, ReferenceCount={1}, OwnerPeer={2}",
                                this.Room.Name,
                                this.ReferenceCount,
                                reference.OwnerPeer)));
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("RoomInstance for Room {0}: {1} References", this.Room.Name, this.ReferenceCount).AppendLine();
                foreach (var reference in this.references)
                {
                    sb.AppendFormat("- Reference ID {0}, hold by Peer {1}", reference.Value.Id, reference.Value.OwnerPeer);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }
    }
}
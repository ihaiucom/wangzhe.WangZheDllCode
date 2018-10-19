// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomReference.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Used to observe references to room instances from a room cache.
//   A reference to a room should be released (disposed) if it is not
//   longer needed. The related cache observes the number of references
//   to a room and removes a room if it has no references left.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Caching
{
    #region using directives

    using System;

    using Photon.SocketServer;

    #endregion

    /// <summary>
    /// Used to observe references to room instances from a room cache.
    /// A reference to a room should be released (disposed) if it is not
    /// longer needed. The related cache observes the number of references
    /// to a room and removes a room if it has no references left. 
    /// </summary>
    /// <remarks>
    /// In the Lite application the room reference will be stored in a 
    /// peers state property whern the peer joines a room. When a 
    /// peer leaves a room the reference to the room will be disposed.
    /// This pattern ensures that room/game instances will not be 
    /// disposed if there are still peers/clients holding a reference 
    /// to the room/game.
    /// </remarks>
    public class RoomReference : IDisposable
    {
        /// <summary>
        /// The id.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        /// The room cache.
        /// </summary>
        private readonly RoomCacheBase roomCache;

        private readonly PeerBase ownerPeer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomReference"/> class.
        /// </summary>
        /// <param name="roomCache">
        /// The room cache.
        /// </param>
        /// <param name="room">
        /// The room.
        /// </param>
        /// <param name="ownerPeer">
        /// An <see cref="PeerBase"/> instance which obtained the room reference.
        /// </param>
        public RoomReference(RoomCacheBase roomCache, Room room, PeerBase ownerPeer)
        {
            this.roomCache = roomCache;
            this.id = Guid.NewGuid();
            this.Room = room;
            this.ownerPeer = ownerPeer;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RoomReference"/> class. 
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="RoomReference"/> is reclaimed by garbage collection.
        /// </summary>
        ~RoomReference()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the unique id for this instance.
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.id;
            }
        }

        public PeerBase OwnerPeer
        {
            get
            {
                return this.ownerPeer; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the room of this reference.
        /// </summary>
        /// <value>The room.</value>
        public Room Room { get; protected set; }

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Removes the room reference from the associated room cache.
        /// The related room instance will be removed from the cache if 
        /// no more references to the room exists.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.IsDisposed)
                {
                    this.roomCache.ReleaseRoomReference(this);
                    this.IsDisposed = true;
                }
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoActor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Represents a player in a world.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using Photon.SocketServer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a player in a world. 
    /// </summary>
    /// <remarks>
    /// An actor can receive events using InterestAreas and publish events using items.
    /// InterestAreas and items can be added, removed and moved within the world.
    /// </remarks>
    public class MmoActor : IDisposable
    {
        private readonly Dictionary<byte, InterestArea> interestAreas;

        // manual subscription
        protected readonly InterestItems interestItems;

        private readonly Dictionary<string, Item> ownedItems = new Dictionary<string,Item>();

        private readonly PeerBase peer;

        private readonly World world;

        protected MmoActor(PeerBase peer, World world)
        {
            this.peer = peer;
            this.world = world;
            this.interestAreas = new Dictionary<byte, InterestArea>();
            this.interestItems = new InterestItems(peer);
        }

        ~MmoActor()
        {
            this.Dispose(false);
        }

        public Item Avatar { get; set; }

        public PeerBase Peer { get { return this.peer; } }

        /// <summary>
        /// Gets the world the actor is member of.
        /// </summary>
        public World World { get { return this.world; } }

        public void AddInterestArea(InterestArea interestArea)
        {
            this.interestAreas.Add(interestArea.Id, interestArea);
        }

        public void AddItem(Item item)
        {
            if (item.Owner != this)
            {
                throw new ArgumentException("foreign owner forbidden");
            }

            ownedItems.Add(item.Id, item);
        }

        public bool RemoveInterestArea(byte interestAreaId)
        {
            return this.interestAreas.Remove(interestAreaId);
        }

        public bool RemoveItem(Item item)
        {
            return ownedItems.Remove(item.Id);
        }

        public bool TryGetInterestArea(byte interestAreaId, out InterestArea interestArea)
        {
            return this.interestAreas.TryGetValue(interestAreaId, out interestArea);
        }

        public bool TryGetItem(string itemid, out Item item)
        {
            return ownedItems.TryGetValue(itemid, out item);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the InterestAreas and destroys all owned items.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (InterestArea camera in this.interestAreas.Values)
                {
                    lock (camera.SyncRoot)
                    {
                        camera.Dispose();
                    }
                }

                this.interestAreas.Clear();

                foreach (Item item in ownedItems.Values)
                {
                    item.Destroy();
                    item.Dispose();
                    this.world.ItemCache.RemoveItem(item.Id);
                }

                this.ownedItems.Clear();
            }
        }
    }
}
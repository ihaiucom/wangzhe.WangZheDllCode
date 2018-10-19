// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Subscribes to regions in the area. Each region relays events of items in region.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ExitGames.Concurrency.Fibers;

    using Photon.SocketServer.Concurrency;
    using Photon.MmoDemo.Common;

    /// <summary>
    /// Subscribes to regions in the area. Each region relays events of items in region.
    /// Thread safety: All instance members require a lock on SyncRoot.
    /// </summary>
    public class InterestArea : IDisposable
    {
        // Locking the sync root guarantees thread safe access.
        public readonly object SyncRoot = new object();

        private readonly byte id;
        
        private readonly RequestItemEnterMessage requestItemEnterMessage;
        private readonly RequestItemExitMessage requestItemExitMessage;

        // Regions in the area
        private readonly HashSet<Region> regions;

        private readonly IFiber subscriptionManagementFiber;

        private readonly World world;

        // for debug only
        private BoundingBox currentInnerFocus;

        // for debug only
        private BoundingBox currentOuterFocus;

        private IDisposable itemMovementSubscription;

        private readonly Dictionary<Region, IDisposable> regionSubscriptions;

        protected InterestArea(byte id, World world)
        {
            this.id = id;
            this.world = world;
            this.requestItemEnterMessage = new RequestItemEnterMessage(this);
            this.requestItemExitMessage = new RequestItemExitMessage(this);
            this.regions = new HashSet<Region>();            
            this.subscriptionManagementFiber = new PoolFiber();
            this.subscriptionManagementFiber.Start();
            this.regionSubscriptions = new Dictionary<Region, IDisposable>();
        }

        ~InterestArea()
        {
            this.Dispose(false);
        }

        public Item AttachedItem { get; private set; }

        public byte Id { get { return this.id; } }

        public Vector Position { get; set; }

        /// <summary>
        /// Gets or sets the inner view distance (the item subscribe threshold).
        /// </summary>
        public Vector ViewDistanceEnter { get; set; }

        /// <summary>
        /// Gets or sets the outer view distance (the item unsubscribe threshold).
        /// </summary>
        public Vector ViewDistanceExit { get; set; }

        /// <summary>
        /// Attaching an Item to the InterestArea automatically updates the InterestArea's Position when the Item moves.
        /// </summary>
        /// <remarks>
        /// Detach the item with Detach.
        /// Thread safety: Requires enqueuing on the item's Item.Fiber and like all instance members a lock on SyncRoot.
        /// </remarks>
        public void AttachToItem(Item item)
        {
            if (this.AttachedItem != null)
            {
                throw new InvalidOperationException();
            }

            this.AttachedItem = item;
            this.Position = item.Position;

            IDisposable disposeSubscription = item.DisposeChannel.Subscribe(this.subscriptionManagementFiber, this.AttachedItem_OnItemDisposed);

            // move camera when item moves
            IDisposable positionSubscription = item.PositionUpdateChannel.Subscribe(this.subscriptionManagementFiber, this.AttachedItem_OnItemPosition);
            this.itemMovementSubscription = new UnsubscriberCollection(disposeSubscription, positionSubscription);
        }

        /// <summary>
        /// Detaches the InterestArea from an Item that was attached with AttachToItem.
        /// </summary>
        public void Detach()
        {
            if (this.AttachedItem != null)
            {
                this.itemMovementSubscription.Dispose();
                this.itemMovementSubscription = null;

                Item item = this.AttachedItem;
                this.AttachedItem = null;
            }
        }

        /// <summary>
        /// Updates the Region subscriptions that are used to detect Items in the nearby World.
        /// This method should be called after changing the InterestArea's Position.
        /// </summary>
        public void UpdateInterestManagement()
        {
            // update unsubscribe area
            BoundingBox focus = BoundingBox.CreateFromPoints(this.Position - this.ViewDistanceExit, this.Position + this.ViewDistanceExit);
            this.currentOuterFocus = focus.IntersectWith(this.world.Area);

            // get subscribe area
            focus = new BoundingBox { Min = this.Position - this.ViewDistanceEnter, Max = this.Position + this.ViewDistanceEnter };
            this.currentInnerFocus = focus.IntersectWith(this.world.Area);

            this.SubscribeRegions(this.world.GetRegions(this.currentInnerFocus));
            this.UnsubscribeRegionsNotIn(this.world.GetRegions(this.currentOuterFocus));

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the fiber used to manage the subscriptions, detaches any attached item and resolves all existing channel subscriptions.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.subscriptionManagementFiber.Dispose();

                // detach
                if (this.AttachedItem != null)
                {
                    this.itemMovementSubscription.Dispose();
                    this.itemMovementSubscription = null;
                    this.AttachedItem = null;
                }

                this.regions.Clear();
                foreach (var s in regionSubscriptions.Values)
                {
                    s.Dispose();
                }
                this.regionSubscriptions.Clear();
            }
        }

        /// <summary>
        /// Region enters area.
        /// </summary>
        protected virtual void OnRegionEnter(Region region)
        {
            var subscription = region.ItemRegionChangedChannel.Subscribe(this.subscriptionManagementFiber, OnItemRegionChange);
            regionSubscriptions.Add(region, subscription);
        }

        /// <summary>
        /// Region exits area.
        /// </summary>
        protected virtual void OnRegionExit(Region region)
        {
            IDisposable subscription;
            if (this.regionSubscriptions.TryGetValue(region, out subscription))
            {
                subscription.Dispose();
                this.regionSubscriptions.Remove(region);
            }
        }

        private void OnItemRegionChange(ItemRegionChangedMessage message)
        {
            var r0 = regions.Contains(message.Region0);
            var r1 = regions.Contains(message.Region1);
            if (r0 && r1)
            {
                // do nothing
            }
            else if (r0) // item exits area
            {
                this.OnItemExit(message.ItemSnapshot.Source);
            }
            else if (r1) // item enters area
            {
                this.OnItemEnter(message.ItemSnapshot);
            }
                
        }

        /// <summary>
        /// Item enters area
        /// </summary>
        public virtual void OnItemEnter(ItemSnapshot snapshot)
        {
        }

        /// <summary>
        /// Item exits area
        /// </summary>
        public virtual void OnItemExit(Item item)
        {
        }

        private void AttachedItem_OnItemDisposed(ItemDisposedMessage message)
        {
            MessageCounters.CounterReceive.Increment();

            lock (this.SyncRoot)
            {
                if (message.Source == this.AttachedItem)
                {
                    this.Detach();
                }
            }
        }

        private void AttachedItem_OnItemPosition(ItemPositionMessage message)
        {
            MessageCounters.CounterReceive.Increment();

            lock (this.SyncRoot)
            {
                if (this.AttachedItem == message.Source)
                {
                    this.Position = message.Position;
                    this.UpdateInterestManagement();
                }
            }
        }

        private void SubscribeRegions(IEnumerable<Region> newRegions)
        {
            foreach (Region r in newRegions)
            {
                if (!this.regions.Contains(r))
                {
                    this.regions.Add(r);
                    this.OnRegionEnter(r);
                    r.RequestItemEnterChannel.Publish(requestItemEnterMessage);
                }
            }
        }

        private void UnsubscribeRegionsNotIn(IEnumerable<Region> regionsToSurvive)
        {
            var toUnsubscribeEnumerable = regions.Except(regionsToSurvive);
            var toUnsubscribe = toUnsubscribeEnumerable.ToArray(); // make copy

            foreach (var r in toUnsubscribe)
            {
                this.regions.Remove(r);
                this.OnRegionExit(r);
                r.RequestItemExitChannel.Publish(requestItemExitMessage);
            }
        }
    }
}
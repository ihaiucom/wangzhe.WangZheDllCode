// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestItems.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Maintains subscriptions per item (manual).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Concurrency.Fibers;
using Photon.SocketServer.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photon.MmoDemo.Server
{
    /// <summary>
    /// Maintains subscriptions per item (manual).
    /// </summary>
    public class InterestItems
    {
        // Locking the sync root guarantees thread safe access.
        public readonly object SyncRoot = new object();

        private readonly Dictionary<Item, IDisposable> manualItemSubscriptions;

        private readonly Photon.SocketServer.PeerBase peer;
        private readonly IFiber itemEventFiber;
        private readonly IFiber subscriptionManagementFiber;

        public InterestItems(Photon.SocketServer.PeerBase peer)
        {
            this.peer = peer;
            this.manualItemSubscriptions = new Dictionary<Item, IDisposable>();
            this.itemEventFiber = new PoolFiber();
            this.itemEventFiber.Start();
            this.subscriptionManagementFiber = new PoolFiber();            
            this.subscriptionManagementFiber.Start();
        }

        /// <summary>
        /// Subscribes an Item manually. 
        /// </summary>
        /// <remarks>
        /// Unsubscribe with UnsubscribeItem.
        /// Thread safety: Requires enqueuing on the item's Item.Fiber and like all instance members a lock on SyncRoot.
        /// </remarks>
        public bool SubscribeItem(Item item)
        {
            if (this.manualItemSubscriptions.ContainsKey(item))
            {
                return false;
            }

            var messagesListener = item.EventChannel.Subscribe(this.itemEventFiber, SubscribedItem_OnItemEvent);

            IDisposable managementListener = item.DisposeChannel.Subscribe(this.subscriptionManagementFiber, this.SubscribedItem_OnItemDisposed);
            this.manualItemSubscriptions.Add(item, new UnsubscriberCollection(messagesListener, managementListener));
            return true;
        }

        /// <summary>
        /// Unsubscribe an Item that was manually subscribed with SubscribeItem.
        /// </summary>
        public bool UnsubscribeItem(Item item)
        {
            IDisposable subscription;
            if (this.manualItemSubscriptions.TryGetValue(item, out subscription))
            {
                subscription.Dispose();
                this.manualItemSubscriptions.Remove(item);
                return true;
            }

            return false;
        }

        private void SubscribedItem_OnItemDisposed(ItemDisposedMessage itemDisposeMessage)
        {
            MessageCounters.CounterReceive.Increment();

            // InterestArea have to be locked 
            lock (this.SyncRoot)
            {
                this.UnsubscribeItem(itemDisposeMessage.Source);
            }
        }
        private void SubscribedItem_OnItemEvent(ItemEventMessage m) 
        {
            this.peer.SendEvent(m.EventData, m.SendParameters);
        }

        protected void ClearManualSubscriptions()
        {
            foreach (KeyValuePair<Item, IDisposable> pair in this.manualItemSubscriptions)
            {
                pair.Value.Dispose();
            }

            this.manualItemSubscriptions.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.itemEventFiber.Dispose();
                this.subscriptionManagementFiber.Dispose();
                this.ClearManualSubscriptions();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

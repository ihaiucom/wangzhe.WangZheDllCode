// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientInterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This InterestArea subclass automatically subscribes to the region.ItemEventChannel
//   of every region in focus and forwards item events to the PeerBase.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Concurrency.Fibers;

    using Photon.SocketServer;
    using Photon.MmoDemo.Server.Events;
    using Photon.MmoDemo.Common;

    /// <summary>
    /// This InterestArea subclass automatically subscribes to the region.ItemEventChannel
    /// of every region in focus and forwards item events to the PeerBase.
    /// </summary>
    public class ClientInterestArea : InterestArea
    {

        private readonly Dictionary<Region, IDisposable> eventChannelSubscriptions;

        private readonly PeerBase peer;

        public ClientInterestArea(PeerBase peer, byte id, World world)
            : base(id, world)
        {
            this.peer = peer;
            this.eventChannelSubscriptions = new Dictionary<Region, IDisposable>();
        }

        /// <summary>
        /// Subscrives entered region.
        /// </summary>
        protected override void OnRegionEnter(Region region)
        {
            base.OnRegionEnter(region);
            // subscribibg to events relayed by region from it's items
            IDisposable messageReceiver = region.ItemEventChannel.Subscribe(this.peer.RequestFiber, this.Region_OnItemEvent);
            this.eventChannelSubscriptions[region] = messageReceiver;
        }

        /// <summary>
        /// Unsubscribe exited region.
        /// </summary>
        protected override void OnRegionExit(Region region)
        {
            base.OnRegionExit(region);
            IDisposable messageReceiver = this.eventChannelSubscriptions[region];
            this.eventChannelSubscriptions.Remove(region);
            messageReceiver.Dispose();
        }

        /// <summary>
        /// Event relayed by subscribed region from region's items.
        /// </summary>
        private void Region_OnItemEvent(ItemEventMessage message)
        {
            ItemEventMessage.CounterEventReceive.Increment();
            this.peer.SendEvent(message.EventData, message.SendParameters);
        }

        /// <summary>
        /// Notifies peer about Item entered area.
        /// </summary>
        public override void OnItemEnter(ItemSnapshot snapshot)
        {
            base.OnItemEnter(snapshot);
            var item = snapshot.Source;
            var subscribeEvent = new ItemSubscribed
            {
                ItemId = item.Id,
                ItemType = item.Type,
                Position = snapshot.Position,
                PropertiesRevision = snapshot.PropertiesRevision,
                InterestAreaId = this.Id,
                Rotation = snapshot.Rotation
            };

            var eventData = new EventData((byte)EventCode.ItemSubscribed, subscribeEvent);
            this.peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
        }

        /// <summary>
        /// Notifies peer about item exited area.
        /// </summary>
        public override void OnItemExit(Item item)
        {
            base.OnItemExit(item);
            var unsubscribeEvent = new ItemUnsubscribed { ItemId = item.Id, InterestAreaId = this.Id };
            var eventData = new EventData((byte)EventCode.ItemUnsubscribed, unsubscribeEvent);
            this.peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
        }

    }
}
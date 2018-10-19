// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Radar.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Subscribers of the Radar's Channel receive event RadarUpdate for all moving Items in the World.
//   The receive interval is configured with Settings.RadarUpdateInterval.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Concurrency.Fibers;

    using Photon.MmoDemo.Common;

    using Photon.MmoDemo.Server.Events;

    using Photon.SocketServer;
    using Photon.SocketServer.Concurrency;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Subscribers of the Radar's Channel receive event RadarUpdate for all moving Items in the World.
    /// The receive interval is configured with Settings.RadarUpdateInterval.
    /// </summary>
    public sealed class Radar : IDisposable
    {
        private readonly ActionQueue actionQueue;

        private readonly MessageChannel<ItemEventMessage> channel;

        private readonly IFiber fiber;

        private readonly Dictionary<Item, Vector> itemPositions;

        private readonly Dictionary<Item, IDisposable> itemSubscriptions;

        public Radar()
        {
            this.fiber = new PoolFiber();
            this.fiber.Start();
            this.channel = new MessageChannel<ItemEventMessage>(ItemEventMessage.CounterEventSend);
            this.itemPositions = new Dictionary<Item, Vector>();
            this.itemSubscriptions = new Dictionary<Item, IDisposable>();
            this.actionQueue = new ActionQueue(this, this.fiber);
        }

        /// <summary>
        /// Gets the channel that publishes event RadarUpdate.
        /// </summary>
        public MessageChannel<ItemEventMessage> Channel { get { return this.channel; } }

        /// <summary>
        /// Registers an Item with the radar.
        /// </summary>
        /// <remarks>
        /// The radar will receive position changes from the item and publish them with his Channel.
        /// The publish interval can be configured with Settings.RadarUpdateInterval.
        /// </remarks>
        public void AddItem(Item item, Vector position)
        {
            this.actionQueue.EnqueueAction(
                () =>
                {
                    this.itemPositions.Add(item, position);

                    // update radar every 10 seconds
                    IDisposable positionUpdates = item.PositionUpdateChannel.SubscribeToLast(
                        this.fiber, this.UpdatePosition, Settings.RadarUpdateInterval);
                    IDisposable disposeMessage = item.DisposeChannel.Subscribe(this.fiber, this.RemoveItem);
                    var unsubscriber = new UnsubscriberCollection(positionUpdates, disposeMessage);
                    this.itemSubscriptions.Add(item, unsubscriber);

                    this.PublishUpdate(item, position, false, true);
                });
        }

        /// <summary>
        /// Send event RadarUpdate for all registered Items to the peer.
        /// </summary>
        public void SendContentToPeer(MmoPeer peer)
        {
            this.actionQueue.EnqueueAction(() => this.PublishAll(peer));
        }

        /// <summary>
        /// Disposes the fiber and clears all subscriptions and dictionaries.
        /// </summary>
        public void Dispose()
        {
            this.fiber.Dispose();
            this.Channel.ClearSubscribers();
            foreach (IDisposable unsubscriber in this.itemSubscriptions.Values)
            {
                unsubscriber.Dispose();
            }

            this.itemSubscriptions.Clear();
            this.itemPositions.Clear();
        }

        private static object GetUpdateEvent(Item item, Vector position, bool remove)
        {
        return new RadarUpdate { ItemId = item.Id, ItemType = item.Type, Position = position, Remove = remove };                
        }

        private void PublishAll(Peer receiver)
        {
            foreach (KeyValuePair<Item, Vector> entry in this.itemPositions)
            {
                var message = GetUpdateEvent(entry.Key, entry.Value, false);
                var eventData = new EventData((byte)EventCode.RadarUpdate, message);
                receiver.SendEvent(eventData, new SendParameters { Unreliable = true, ChannelId = Settings.RadarEventChannel });
            }
        }

        private void PublishUpdate(Item item, Vector position, bool remove, bool unreliable)
        {
            var updateEvent = GetUpdateEvent(item, position, remove);
            IEventData eventData = new EventData((byte)EventCode.RadarUpdate, updateEvent);
            var message = new ItemEventMessage(item, eventData, new SendParameters { Unreliable = unreliable, ChannelId = Settings.RadarEventChannel });
            this.channel.Publish(message);
        }

        private void RemoveItem(ItemDisposedMessage message)
        {
            ////log.InfoFormat("remove item {0}", message.Source.Id);
            Item item = message.Source;
            this.itemPositions.Remove(item);
            this.itemSubscriptions[item].Dispose();
            this.itemSubscriptions.Remove(item);

            this.PublishUpdate(item, Vector.Zero, true, false);
        }

        private void UpdatePosition(ItemPositionMessage message)
        {
            Item item = message.Source;
            if (this.itemPositions.ContainsKey(item))
            {
                this.itemPositions[item] = message.Position;
                this.PublishUpdate(item, message.Position, false, true);
            }
        }
    }
}
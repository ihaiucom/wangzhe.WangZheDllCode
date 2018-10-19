// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Represents an entity in a world. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections;

    using ExitGames.Concurrency.Fibers;

    using Photon.SocketServer.Concurrency;
    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server.Events;

    /// <summary>
    /// Copy of current Item's state
    /// </summary>
    public class ItemSnapshot
    {
        public ItemSnapshot(Item source, Vector position, Vector rotation, Region worldRegion, int propertiesRevision)
        {
            this.Source = source;
            this.Position = position;
            this.Rotation = rotation;
            this.PropertiesRevision = propertiesRevision;
        }

        public Item Source { get; private set; }
        public Vector Position { get; private set; }
        public Vector Rotation { get; private set; }
        public int PropertiesRevision { get; private set; }
    }

    /// <summary>
    /// Represents an entity in a world. 
    /// </summary>
    /// <remarks>
    /// Items are event publisher and the counterpart to InterestAreas.
    /// </remarks>
    public class Item : IDisposable
    {
        private readonly string id;
       
        // Current region subscribes on item's events
        private readonly MessageChannel<ItemEventMessage> eventChannel;

        // Object (radar or attached) watches item position
        private readonly MessageChannel<ItemPositionMessage> positionUpdateChannel;

        // Object (radar or attached) watches item dispose
        private readonly MessageChannel<ItemDisposedMessage> disposeChannel;

        private readonly Hashtable properties;

        private readonly byte type;

        private readonly World world;

        private bool disposed;

        private IDisposable regionSubscription;

        public Item(Vector position, Vector rotation, Hashtable properties, MmoActorOperationHandler owner, string id, byte type, World world)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Owner = owner;
            this.eventChannel = new MessageChannel<ItemEventMessage>(ItemEventMessage.CounterEventSend);
            this.disposeChannel = new MessageChannel<ItemDisposedMessage>(MessageCounters.CounterSend);
            this.positionUpdateChannel = new MessageChannel<ItemPositionMessage>(MessageCounters.CounterSend);
            this.properties = properties ?? new Hashtable();
            if (properties != null)
            {
                this.PropertiesRevision++;
            }

            this.id = id;
            this.world = world;
            this.type = type;
        }

        ~Item()
        {
            this.Dispose(false);
        }

        public IFiber Fiber { get { return this.Owner.Peer.RequestFiber; } }

        public Region CurrentWorldRegion { get; private set; }

        public MessageChannel<ItemDisposedMessage> DisposeChannel { get { return this.disposeChannel; } }

        public bool Disposed { get { return this.disposed; } }

        public MessageChannel<ItemEventMessage> EventChannel { get { return this.eventChannel; } }

        public string Id { get { return this.id; } }

        public MmoActorOperationHandler Owner { get; private set; }

        public Vector Rotation { get; set; }

        public Vector Position { get; set; }

        public MessageChannel<ItemPositionMessage> PositionUpdateChannel { get { return this.positionUpdateChannel; } }

        public Hashtable Properties { get { return this.properties; } }

        public int PropertiesRevision { get; set; }

        public byte Type { get { return this.type; } }

        public World World { get { return this.world; } }

        public void Destroy()
        {
            this.OnDestroy();
        }

        /// <summary>
        /// Publishes a ItemPositionMessage in the PositionUpdateChannel
        /// Subscribes and unsubscribes regions if changed. 
        /// </summary>
        public void UpdateInterestManagement()
        {
            // inform attached interst area and radar
            ItemPositionMessage message = this.GetPositionUpdateMessage(this.Position);
            this.positionUpdateChannel.Publish(message);

            // update subscriptions if region changed
            Region prevRegion = this.CurrentWorldRegion;
            Region newRegion = this.World.GetRegion(this.Position);
            
            if (newRegion != this.CurrentWorldRegion)
            {
                this.CurrentWorldRegion = newRegion;

                if (this.regionSubscription != null)
                {
                    this.regionSubscription.Dispose();
                }

                var snapshot = this.GetItemSnapshot();
                var regMessage = new ItemRegionChangedMessage(prevRegion, newRegion, snapshot);

                if (prevRegion != null)
                {
                    prevRegion.ItemRegionChangedChannel.Publish(regMessage);
                }
                if (newRegion != null)
                {
                    newRegion.ItemRegionChangedChannel.Publish(regMessage);

                    this.regionSubscription = new UnsubscriberCollection(
                        this.EventChannel.Subscribe(this.Fiber, (m) => newRegion.ItemEventChannel.Publish(m)), // route events through region to interest area
                        newRegion.RequestItemEnterChannel.Subscribe(this.Fiber, (m) => { m.InterestArea.OnItemEnter(this.GetItemSnapshot()); }), // region entered interest area fires message to let item notify interest area about enter
                        newRegion.RequestItemExitChannel.Subscribe(this.Fiber, (m) => { m.InterestArea.OnItemExit(this); }) // region exited interest area fires message to let item notify interest area about exit
                    );
                }
                
            }
        }

        /// <summary>
        /// Updates the Properties and increments the PropertiesRevision.
        /// </summary>
        public void SetProperties(Hashtable propertiesSet, ArrayList propertiesUnset)
        {
            if (propertiesSet != null)
            {
                foreach (DictionaryEntry entry in propertiesSet)
                {
                    this.properties[entry.Key] = entry.Value;
                }
            }

            if (propertiesUnset != null)
            {
                foreach (object key in propertiesUnset)
                {
                    this.properties.Remove(key);
                }
            }

            this.PropertiesRevision++;
        }

        /// <summary>
        /// Creates an ItemSnapshot with a snapshot of the current attributes.
        /// </summary>
        protected internal ItemSnapshot GetItemSnapshot()
        {
            return new ItemSnapshot(this, this.Position, this.Rotation, this.CurrentWorldRegion, this.PropertiesRevision);
        }

        /// <summary>
        /// Creates an ItemPositionMessage with the current position.
        /// </summary>
        protected ItemPositionMessage GetPositionUpdateMessage(Vector position)
        {
            return new ItemPositionMessage(this, position);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Publishes a ItemDisposedMessage through the DisposeChannel and disposes all channels.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.regionSubscription != null)
                {
                    this.regionSubscription.Dispose();
                }
                this.CurrentWorldRegion = null;
                this.disposeChannel.Publish(new ItemDisposedMessage(this));
                this.eventChannel.Dispose();
                this.disposeChannel.Dispose();
                this.positionUpdateChannel.Dispose();

                this.disposed = true;
            }
        }

        /// <summary>
        /// Publishes event ItemDestroyed in the Item.EventChannel.
        /// </summary>
        protected void OnDestroy()
        {
            var eventInstance = new ItemDestroyed { ItemId = this.Id };
            var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
            var message = new ItemEventMessage(this, eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            this.EventChannel.Publish(message);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        public void Move(Vector position)
        {
            this.Position = position;
            this.UpdateInterestManagement();
        }

        /// <summary>
        /// Spawns the item.
        /// </summary>
        public void Spawn(Vector position)
        {
            this.Position = position;
            this.UpdateInterestManagement();
        }

        /// <summary>
        /// Checks wheter the actor is allowed to change the item.
        /// </summary>
        public bool GrantWriteAccess(MmoActorOperationHandler actor)
        {
            return this.Owner == actor;
        }
    }
}
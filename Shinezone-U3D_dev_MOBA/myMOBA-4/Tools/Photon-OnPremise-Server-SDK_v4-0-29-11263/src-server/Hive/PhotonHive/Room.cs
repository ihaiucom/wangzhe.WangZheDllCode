// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Room.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A room has <see cref="Actor" />s, can have properties, and provides an <see cref="ExecutionFiber" /> with a few wrapper methods to solve otherwise complicated threading issues:
//   All actions enqueued to the <see cref="ExecutionFiber" /> are executed in a serial order. Operations of all Actors in a room are handled via ExecutionFiber.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Photon.Hive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.Hive.Caching;
    using Photon.Hive.Events;
    using Photon.Hive.Messages;
    using Photon.Hive.Plugin;
    using Photon.SocketServer;

    using SendParameters = Photon.SocketServer.SendParameters;

    public interface IGameStateFactory
    {
        GameState Create();
    }
    /// <summary>
    ///   A room has <see cref = "Actor" />s, can have properties, and provides an <see cref = "ExecutionFiber" /> with a few wrapper methods to solve otherwise complicated threading issues:
    ///   All actions enqueued to the <see cref = "ExecutionFiber" /> are executed in a serial order. Operations of all Actors in a room are handled via ExecutionFiber.
    /// </summary>
    public class Room : IDisposable
    {
        #region Classes

        protected class DefaultGameStateFactory : IGameStateFactory
        {
            public GameState Create()
            {
                return new GameState();
            }
        }
        #endregion

        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly DefaultGameStateFactory defaultFactory = new DefaultGameStateFactory();

        /// <summary>
        ///   The room name.
        /// </summary>
        private readonly string name;

        protected readonly RoomCacheBase roomCache;

        private IDisposable removeTimer;

        protected readonly IGameStateFactory gameStateFactory;

        protected string RemoveRoomPathString;

        protected RemoveState RemoveRoomPath
        {
            get { return this.removeRoomPath; }
            set
            {
                this.RemoveRoomPathString += ";" + value;
                removeRoomPath = value;
            }
        }
        private RemoveState removeRoomPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Room" /> class without a room name.
        /// </summary>
        /// <param name = "name">
        ///   The room name.
        /// </param>
        /// <param name="roomCache">
        ///   The <see cref="RoomCacheBase"/> instance to which the room belongs.
        /// </param>
        /// <param name="gameStateFactory">Fatory for game state</param>
        /// <param name="maxEmptyRoomTTL">
        ///   A value indicating how long the room instance will be keeped alive 
        ///   in the room cache after all peers have left the room.
        /// </param>
        /// <param name="executionFiber">Fiber which will execute rooms actions</param>
        public Room(string name, RoomCacheBase roomCache = null, IGameStateFactory gameStateFactory = null, int maxEmptyRoomTTL = 0, ExtendedPoolFiber executionFiber = null)
            : this(name, executionFiber, roomCache, gameStateFactory, maxEmptyRoomTTL)
        {
            this.ExecutionFiber.Start();
            RemoveRoomPath = RemoveState.Alive;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Room" /> class.
        /// </summary>
        /// <param name = "name">
        ///   The room name.
        /// </param>
        /// <param name = "executionFiber">
        ///   The execution fiber used to synchronize access to this instance.
        /// </param>
        /// <param name="roomCache">
        ///   The <see cref="RoomCacheBase"/> instance to which the room belongs.
        /// </param>
        /// <param name="gameStateFactory">Fatory for game state</param>
        /// <param name="maxEmptyRoomTTL">
        ///   A value indicating how long the room instance will be keeped alive 
        ///   in the room cache after all peers have left the room.
        /// </param>
        protected Room(string name, ExtendedPoolFiber executionFiber, RoomCacheBase roomCache, IGameStateFactory gameStateFactory = null, int maxEmptyRoomTTL = 0)
        {
            this.name = name;

            this.gameStateFactory = gameStateFactory ?? defaultFactory;
            this.roomState = this.gameStateFactory.Create();
            this.ExecutionFiber = executionFiber ?? new ExtendedPoolFiber();

            this.roomCache = roomCache;
            this.MaxEmptyRoomTTL = maxEmptyRoomTTL;
        }

        /// <summary>
        ///   Finalizes an instance of the <see cref = "Room" /> class. 
        ///   This destructor will run only if the Dispose method does not get called.
        ///   It gives your base class the opportunity to finalize.
        ///   Do not provide destructors in types derived from this class.
        /// </summary>
        ~Room()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a <see cref = "PoolFiber" /> instance used to synchronize access to this instance.
        /// </summary>
        /// <value>A <see cref = "PoolFiber" /> instance.</value>
        public ExtendedPoolFiber ExecutionFiber { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether IsDisposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///   Gets the name (id) of the room.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how long the room instance will be keeped alive 
        /// in the room cache after all peers have left the room.
        /// </summary>
        public int EmptyRoomLiveTime 
        { 
            get { return this.roomState.EmptyRoomLiveTime; } 
            protected set { this.roomState.EmptyRoomLiveTime = value; } 
        }

        public int MaxEmptyRoomTTL { get; private set; }

        /// <summary>
        /// Timer for removing room
        /// </summary>
        protected IDisposable RemoveTimer { get; set; }

        /// <summary>
        ///   Gets a PropertyBag instance used to store custom room properties.
        /// </summary>
        public PropertyBag<object> Properties { get { return this.roomState.Properties; } }

        protected GameState roomState;

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
//            sb.AppendFormat("Room name: {0}, Actors: {1}, Properties: {2}", this.name, this.Actors.Count, this.Properties == null ? 0 : this.Properties.Count).AppendLine();
            sb.AppendFormat("Room name: {0}, Properties: {1}", this.name, this.Properties == null ? 0 : this.Properties.Count).AppendLine(); 
            
            //foreach (var actor in this.Actors)
            //{
            //    sb.AppendLine(actor.ToString());
            //}

            return sb.ToString();
        }

        /// <summary>
        /// Called by the <see cref="RoomCacheBase"/> if the room is about to be removed from the cache.
        /// </summary>
        /// <returns>
        /// True if the room should be automaticly by removed by the <see cref="RoomCacheBase"/>.
        /// False if the room has an custom remove implementation and will remove itself from the cache.
        /// </returns>
        /// <remarks>
        /// The default implementation checks if the EmptyRoomLiveTime value is set to a value greater zero.
        /// If it's set the method will return false to indicate that the cache should not remove this
        /// instance automaticly. The removal of the room is then scheduled using the <see cref="ExecutionFiber"/>.
        /// </remarks>
        public virtual bool BeforeRemoveFromCache()
        {
            if (this.EmptyRoomLiveTime <= 0)
            {
                return true;
            }

            // execute the schedule with the ExecutionFiber so properties
            // are accessed thread safe.
            this.ExecutionFiber.Enqueue(() => this.ScheduleRoomRemoval(this.EmptyRoomLiveTime));
            return false;
        }

        /// <summary>
        ///   Enqueues an <see cref = "IMessage" /> to the end of the execution queue.
        /// </summary>
        /// <param name = "message">
        ///   The message to enqueue.
        /// </param>
        /// <remarks>
        ///   <see cref = "ProcessMessage" /> is called sequentially for each operation request 
        ///   stored in the execution queue.
        ///   Using an execution queue ensures that messages are processed in order
        ///   and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueMessage(IMessage message)
        {
            this.ExecutionFiber.Enqueue(() => this.ProcessMessage(message));
        }

        /// <summary>
        ///   Enqueues an <see cref = "OperationRequest" /> to the end of the execution queue.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request to enqueue.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <remarks>
        ///   <see cref = "ExecuteOperation" /> is called sequentially for each operation request 
        ///   stored in the execution queue.
        ///   Using an execution queue ensures that operation request are processed in order
        ///   and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueOperation(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }

        /// <summary>
        ///   Schedules a message to be processed after a specified time.
        /// </summary>
        /// <param name = "message">
        ///   The message to schedule.
        /// </param>
        /// <param name = "timeMs">
        ///   The time in milliseconds to wait before the message will be processed.
        /// </param>
        /// <returns>
        ///   an <see cref = "IDisposable" />
        /// </returns>
        public IDisposable ScheduleMessage(IMessage message, long timeMs)
        {
            return this.ExecutionFiber.Schedule(() => this.ProcessMessage(message), timeMs);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name = "dispose">
        ///   <c>true</c> to release both managed and unmanaged resources; 
        ///   <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool dispose)
        {
            this.IsDisposed = true;

            if (dispose)
            {
                this.ExecutionFiber.Dispose();
                if (this.removeTimer != null)
                {
                    this.removeTimer.Dispose();
                    this.removeTimer = null;
                }
            }
        }

        /// <summary>
        ///   This method is invoked sequentially for each operation request 
        ///   enqueued in the <see cref = "ExecutionFiber" /> using the 
        ///   <see cref = "EnqueueOperation" /> method.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "operation">
        ///   The operation request.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void ExecuteOperation(HivePeer peer, OperationRequest operation, SendParameters sendParameters)
        {
        }

        /// <summary>
        ///   This method is invoked sequentially for each message enqueued 
        ///   by the <see cref = "EnqueueMessage" /> or <see cref = "ScheduleMessage" />
        ///   method.
        /// </summary>
        /// <param name = "message">
        ///   The message to process.
        /// </param>
        protected virtual void ProcessMessage(IMessage message)
        {
        }

        /// <summary>
        ///   Publishes an event to a single actor on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actor">
        ///   The <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(HiveEventBase e, Actor actor, SendParameters sendParameters)
        {
            var eventData = new EventData(e.Code, e);
            actor.Peer.SendEvent(eventData, sendParameters);
        }

        /// <summary>
        ///   Publishes an event to a list of actors on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actorList">
        ///   A list of <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(HiveEventBase e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            var peers = actorList.Select(actor => actor.Peer);
            var eventData = new EventData(e.Code, e);
            ApplicationBase.Instance.BroadCastEvent(eventData, peers, sendParameters);
        }

        /// <summary>
        ///   Publishes an event to a list of actors on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actorList">
        ///   A list of <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(EventData e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            var peers = actorList.Select(actor => actor.Peer);
            ApplicationBase.Instance.BroadCastEvent(e, peers, sendParameters);
        }

        /// <summary>
        /// Schedules the removal of the room instance from the cache.
        /// The room will be removed after the specified time if there are no room
        /// references left.
        /// </summary>
        /// <param name="roomLiveTime">
        /// The time to remove the room in milliseconds.
        /// </param>
        protected void ScheduleRoomRemoval(int roomLiveTime)
        {
            if (this.RemoveTimer != null)
            {
                this.RemoveTimer.Dispose();
                this.RemoveTimer = null;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Scheduling room romoval: roomName={0}, liveTime={1:N0}", this.Name, roomLiveTime);
            }

            this.RemoveTimer = this.ExecutionFiber.Schedule(this.TryRemoveRoomFromCache, roomLiveTime);
        }

        protected enum RemoveState
        {
            Alive,
            TryRemoveRoomFromCacheCalled,
            RoomRemoved,
            TryRemoveRoomFromCacheCalledRoomNotRemoved,
            ProcessCloseGameCalled,
            TriggerPluginOnCloseCalled,
            AliveGotNewPlayer,
            ScheduleTriggerPluginOnCloseCalled,
            ProcessBeforeCloseGameCalled,
            ProcessBeforeCloseGameCalledEmptyRoomLiveTimeLECalled,
            BeforeRemoveFromCacheCalled,
            BeforeRemoveFromCacheActionCalled,
            GameDisposeCalled
        }

        /// <summary>
        /// Removes the room instance from the cache if there are no references to the instance left.
        /// </summary>
        protected void TryRemoveRoomFromCache()
        {
            this.RemoveRoomPath = RemoveState.TryRemoveRoomFromCacheCalled;
            var removed = this.roomCache.TryRemoveRoomInstance(this);

            this.RemoveRoomPath = removed ? RemoveState.RoomRemoved : RemoveState.TryRemoveRoomFromCacheCalledRoomNotRemoved;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Tried to remove room: roomName={0}, removed={1}", this.Name, removed);
            }
        }

        public virtual void OnClose()
        {
        }

        public void Release()
        {
            this.ExecutionFiber.Enqueue(() =>
            {
                OnClose();
                Dispose();
            });
        }
        #endregion
    }
}
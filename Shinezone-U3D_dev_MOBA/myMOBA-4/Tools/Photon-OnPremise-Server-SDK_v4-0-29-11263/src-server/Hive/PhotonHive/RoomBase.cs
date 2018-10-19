// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RoomBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    #region using directives

    using System;

    using ExitGames.Concurrency.Fibers;

    using Lite.Messages;

    using Photon.SocketServer;

    #endregion

    /// <summary>
    /// Base class for custom rooms. 
    /// </summary>
    /// <typeparam name="TPeer">The type of peer for the room.</typeparam>
    public abstract class RoomBase<TPeer> : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomBase{TPeer}"/> class.
        /// </summary>
        protected RoomBase()
        {
            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();
        }

        /// <summary>
        /// Gets a <see cref="PoolFiber"/> instance used to synchronize access to this instance.
        /// </summary>
        /// <value>A <see cref="PoolFiber"/> instance.</value>
        public PoolFiber ExecutionFiber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Enqueues an <see cref="OperationRequest"/> to the end of the execution queue.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <param name="operationRequest">
        /// The operation request to enqueue.
        /// </param>
        /// <remarks>
        /// <see cref="ExecuteOperation"/> is called sequentially for each operation request 
        /// stored in the execution queue.
        /// Using an execution queue ensures that operation request are processed in order
        /// and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueOperation(TPeer peer, OperationRequest operationRequest)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest));
        }

        /// <summary>
        /// Enqueues an <see cref="IMessage"/> to the end of the execution queue.
        /// </summary>
        /// <param name="message">
        /// The message to enqueue.
        /// </param>
        /// <remarks>
        /// <see cref="ProcessMessage"/> is called sequentially for each operation request 
        /// stored in the execution queue.
        /// Using an execution queue ensures that messages are processed in order
        /// and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueMessage(IMessage message)
        {
            this.ExecutionFiber.Enqueue(() => this.ProcessMessage(message));
        }

        /// <summary>
        /// Schedules a message to be processed after a specified time.
        /// </summary>
        /// <param name="message">
        /// The message to schedule.
        /// </param>
        /// <param name="timeMs">
        /// The time in milliseconds to wait before the message will be processed.
        /// </param>
        /// <returns>
        /// an <see cref="IDisposable"/>
        /// </returns>
        public IDisposable ScheduleMessage(IMessage message, long timeMs)
        {
            return this.ExecutionFiber.Schedule(() => this.ProcessMessage(message), timeMs);
        }

        /// <summary>
        /// This method is invoked sequentially for each operation request 
        /// enqueued in the <see cref="ExecutionFiber"/> using the 
        /// <see cref="EnqueueOperation"/> method.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <param name="operation">
        /// The operation request.
        /// </param>
        protected abstract void ExecuteOperation(TPeer peer, OperationRequest operation);

        /// <summary>
        /// This method is invoked sequentially for each message enqueued 
        /// by the <see cref="EnqueueMessage"/> or <see cref="ScheduleMessage"/>
        /// method.
        /// </summary>
        /// <param name="message">
        /// The message to process.
        /// </param>
        protected abstract void ProcessMessage(IMessage message);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="dispose">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool dispose)
        {
            this.IsDisposed = true;

            if (dispose)
            {
                this.ExecutionFiber.Dispose();
            }
        }
    }
}
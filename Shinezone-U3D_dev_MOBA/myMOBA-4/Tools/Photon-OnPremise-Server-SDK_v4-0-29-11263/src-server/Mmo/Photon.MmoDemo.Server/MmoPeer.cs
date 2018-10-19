// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Photon Peer.
//   Keeps references to the optional Radar and CounterPublisher subscription.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using PhotonHostRuntimeInterfaces;
    using System;

    /// <summary>
    /// Photon Peer.
    /// </summary>
    /// <remarks>
    /// Keeps references to the optional Radar and CounterPublisher subscription.
    /// </remarks>
    public class MmoPeer : Peer
    {
        private readonly MmoInitialOperationHandler initialOpHandler;

        /// <summary>
        /// Counters are subscribed with operation SubscribeCounter and unsubscribed with OperationCode.UnsubscribeCounter.
        /// <summary/>
        public IDisposable CounterSubscription { get; set; }

        /// <summary>
        /// The radar is subscribed with operation RadarSubscribe.
        /// </summary>
        public IDisposable RadarSubscription { get; set; }

        public MmoPeer(InitRequest initRequest)
            : base(initRequest)
        {
            this.initialOpHandler = new MmoInitialOperationHandler(this);
            // this is the operation handler before entering a world
            this.SetInitialOperationhandler();
        }

        internal void SetInitialOperationhandler()
        {
            this.SetCurrentOperationHandler(initialOpHandler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.RadarSubscription != null)
                {
                    this.RadarSubscription.Dispose();
                    this.RadarSubscription = null;
                }

                if (this.CounterSubscription != null)
                {
                    this.CounterSubscription.Dispose();
                    this.CounterSubscription = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
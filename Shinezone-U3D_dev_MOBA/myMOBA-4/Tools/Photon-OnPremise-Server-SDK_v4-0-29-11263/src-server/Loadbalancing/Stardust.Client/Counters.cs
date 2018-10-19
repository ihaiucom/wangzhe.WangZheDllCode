// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Counters.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The counters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client
{
    using System;
    using System.Diagnostics;

    using ExitGames.Diagnostics.Counter;
    using ExitGames.Diagnostics.Monitoring;
    using ExitGames.Logging;

    /// <summary>
    /// The counters.
    /// </summary>
    public static class Counters
    {
        /// <summary>
        /// The connected clients.
        /// </summary>
        public static readonly NumericCounter ConnectedClients = new NumericCounter();

        /// <summary>
        /// Events that are flushed have a lower round trip time
        /// </summary>
        public static readonly AverageCounter FlushEventRoundTripTime = new AverageCounter();

        /// <summary>
        /// The number of flushed events received per second.
        /// </summary>
        public static readonly CountsPerSecondCounter FlushEventsReceived = new CountsPerSecondCounter();

        /// <summary>
        /// The number of flush invoking operations sent per second.
        /// </summary>
        public static readonly CountsPerSecondCounter FlushOperationsSent = new CountsPerSecondCounter();

        /// <summary>
        /// The received operation response.
        /// </summary>
        public static readonly CountsPerSecondCounter ReceivedOperationResponse = new CountsPerSecondCounter();

        /// <summary>
        /// The reliable event round trip time.
        /// </summary>
        public static readonly AverageCounter ReliableEventRoundTripTime = new AverageCounter();

        /// <summary>
        /// The received events.
        /// </summary>
        public static readonly CountsPerSecondCounter ReliableEventsReceived = new CountsPerSecondCounter();

        /// <summary>
        /// The send operations.
        /// </summary>
        public static readonly CountsPerSecondCounter ReliableOperationsSent = new CountsPerSecondCounter();

        /// <summary>
        /// The round trip time.
        /// </summary>
        public static readonly AverageCounter RoundTripTime = new AverageCounter();

        /// <summary>
        /// The round trip variance.
        /// </summary>
        public static readonly AverageCounter RoundTripTimeVariance = new AverageCounter();

        /// <summary>
        /// The unreliable event round trip time.
        /// </summary>
        public static readonly AverageCounter UnreliableEventRoundTripTime = new AverageCounter();

        /// <summary>
        /// The unreliable events received.
        /// </summary>
        public static readonly CountsPerSecondCounter UnreliableEventsReceived = new CountsPerSecondCounter();

        /// <summary>
        /// The unreliable operations sent.
        /// </summary>
        public static readonly CountsPerSecondCounter UnreliableOperationsSent = new CountsPerSecondCounter();
    }
}
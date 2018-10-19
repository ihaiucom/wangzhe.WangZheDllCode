// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageCounters.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Counters that keep track of the amount of messages sent and received from item channels.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using ExitGames.Diagnostics.Counter;

    /// <summary>
    /// Counters that keep track of the amount of messages sent and received from item channels.
    /// </summary>
    public static class MessageCounters
    {
        /// <summary>
        /// Used to count how many messages were received by InterestAreas (and sometimes items).
        /// </summary>
        public static readonly CountsPerSecondCounter CounterReceive = new CountsPerSecondCounter("ItemMessage.Receive");

        /// <summary>
        /// Used to count how many messages were sent by items (and sometimes InterestAreas).
        /// </summary>
        public static readonly CountsPerSecondCounter CounterSend = new CountsPerSecondCounter("ItemMessage.Send");
    }
}
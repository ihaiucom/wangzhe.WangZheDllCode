// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Counter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Counter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Diagnostics
{
    using ExitGames.Diagnostics.Counter;
    using ExitGames.Diagnostics.Monitoring;

    /// <summary>
    /// Counter on application level
    /// </summary>
    public static class Counter
    {
        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("Games")]
        public static readonly NumericCounter Games = new NumericCounter("Games");
    }
}
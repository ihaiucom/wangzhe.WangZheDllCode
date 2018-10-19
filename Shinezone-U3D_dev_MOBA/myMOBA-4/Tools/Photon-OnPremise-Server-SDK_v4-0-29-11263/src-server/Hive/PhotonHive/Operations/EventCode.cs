// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Codes of events (defining their type and keys).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using Photon.Hive.Events;

    /// <summary>
    ///   Event codes of events (defining their type and keys).
    /// </summary>
    public enum EventCode : byte
    {
        /// <summary>
        ///   Specifies that no event code is set.
        /// </summary>
        NoCodeSet = 0, 

        /// <summary>
        ///   The event code for the <see cref="JoinEvent"/>.
        /// </summary>
        Join = 255, 

        /// <summary>
        ///   The event code for the <see cref="LeaveEvent"/>.
        /// </summary>
        Leave = 254, 

        /// <summary>
        ///   The event code for the <see cref="PropertiesChangedEvent"/>.
        /// </summary>
        PropertiesChanged = 253,

        /// <summary>
        /// The event code for the <see cref="DisconnectEvent"/>.
        /// </summary>
        Disconnect = 252,

        /// <summary>
        /// The event code for the <see cref="ErrorInfoEvent"/>.
        /// </summary>
        ErrorInfo = 251,

        CacheSliceChanged = 250,

        EventCacheSlicePurged = 249,
    }
}
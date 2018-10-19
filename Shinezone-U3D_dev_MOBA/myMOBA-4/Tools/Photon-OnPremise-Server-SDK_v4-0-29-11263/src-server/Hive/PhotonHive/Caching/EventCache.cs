// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A cache for events that a stored for actors that join a game later.
    /// The key is the event code, the value the event content.
    /// The event cache is ordered by event code.
    /// </summary>
    [Serializable]
    public class EventCache : SortedDictionary<byte, Hashtable>
    {
    }
}
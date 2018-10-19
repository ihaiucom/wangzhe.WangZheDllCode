// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheOperation.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Parameter value of RaiseEventRequest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    /// <summary>
    ///   Parameter value of RaiseEventRequest.
    /// </summary>
    public enum CacheOperation
    {
        /// <summary>
        ///   Don't cache the event. (default)
        /// </summary>
        DoNotCache = 0, 

        /// <summary>
        ///   Merge cached event with data.
        /// </summary>
        MergeCache = 1, 

        /// <summary>
        ///   Replace cached event with data.
        /// </summary>
        ReplaceCache = 2, 

        /// <summary>
        ///   Remove cached event.
        /// </summary>
        RemoveCache = 3,

        /// <summary>
        /// Add to the room cache.
        /// </summary>
        AddToRoomCache = 4,

        AddToRoomCacheGlobal = 5,

        RemoveFromRoomCache = 6,

        RemoveFromCacheForActorsLeft = 7
    }
}
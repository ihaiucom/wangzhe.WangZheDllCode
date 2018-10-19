// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteGameCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The lite game cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Caching
{
    /// <summary>
    /// The cache for <see cref="LiteGame"/>s.
    /// </summary>
    public class LiteGameCache : RoomCacheBase
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static readonly LiteGameCache Instance = new LiteGameCache();

        /// <summary>
        /// Creates a new <see cref="LiteGame"/>.
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// A new <see cref="LiteGame"/>
        /// </returns>
        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new LiteGame(roomId, this);
        }
    }
}
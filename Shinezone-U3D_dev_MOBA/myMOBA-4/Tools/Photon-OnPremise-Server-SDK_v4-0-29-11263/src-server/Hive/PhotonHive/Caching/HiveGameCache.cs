// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiveGameCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.SocketServer;

namespace Photon.Hive.Caching
{
    using Photon.Hive.Plugin;

    /// <summary>
    /// The cache for <see cref="HiveGame"/>s.
    /// </summary>
    public class HiveGameCache : RoomCacheBase
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static readonly HiveGameCache Instance = new HiveGameCache();

        private readonly PluginManager pluginManager; 

        #endregion

        #region Properties

        public PluginManager PluginManager
        {
            get { return this.pluginManager; }
        }

        #endregion

        #region Construction

        public HiveGameCache()
        {
            this.pluginManager = new PluginManager(ApplicationBase.Instance.ApplicationRootPath);
        }

        #endregion

        #region Methods

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new HiveHostGame(roomId, this, null, 0, this.pluginManager, (string)args[0]);
        }

        #endregion
    }
}
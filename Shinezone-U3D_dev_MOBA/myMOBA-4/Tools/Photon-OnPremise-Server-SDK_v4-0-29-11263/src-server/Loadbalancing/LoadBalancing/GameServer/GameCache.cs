// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using Photon.Hive;
    using Photon.Hive.Caching;
    using Photon.Hive.Plugin;

    #endregion

    public class GameCache : RoomCacheBase
    {
        private readonly PluginManager pluginManager;

        public GameApplication Application { get; protected set; }
        public PluginManager PluginManager { get { return this.pluginManager; } }

        public GameCache(GameApplication application)
        {
            this.Application = application;
            this.pluginManager = new PluginManager(application.ApplicationRootPath);
        }

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new Game(this.Application, roomId, this, this.pluginManager, (string)args[0]);
        }
    }
}
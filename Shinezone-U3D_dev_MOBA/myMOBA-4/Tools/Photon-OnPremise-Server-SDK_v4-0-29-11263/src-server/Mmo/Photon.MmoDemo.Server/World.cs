// --------------------------------------------------------------------------------------------------------------------
// <copyright file="World.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   GridWorld with name, item cache and radar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using ExitGames.Logging;
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;

    /// <summary>
    /// GridWorld with name, item cache and radar.
    /// </summary>
    public class World : GridWorld
    {
        public readonly Radar Radar = new Radar();

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public World(string name, BoundingBox boundingBox, Vector tileDimensions)
            : base(boundingBox, tileDimensions)
        {
            this.Name = name;
            this.ItemCache = new ItemCache();
            log.InfoFormat("created world {0}", name);
        }

        public string Name { get; private set; }
        public ItemCache ItemCache { get; private set; }

    }
}
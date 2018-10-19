// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This is a cache for Worlds that have a unique name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using ExitGames.Threading;

    using Photon.MmoDemo.Server;
    using Photon.MmoDemo.Common;

    /// <summary>
    /// This is a cache for Worlds that have a unique name.
    /// </summary>
    public sealed class WorldCache : IDisposable
    {
        public static readonly WorldCache Instance = new WorldCache();

        private readonly Dictionary<string, World> dict;

        // Used to synchronize access to the cache.
        private readonly ReaderWriterLockSlim readWriteLock;

        // Prevents a default instance of the WorldCache class from being created. 
        private WorldCache()
        {
            this.dict = new Dictionary<string, World>();
            this.readWriteLock = new ReaderWriterLockSlim();
        }

        ~WorldCache()
        {
            this.Dispose(false);
        }

        public void Clear()
        {
            using (WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                this.dict.Clear();
            }
        }

        public bool TryCreate(string name, BoundingBox boundingBox, Vector tileDimensions, out World world)
        {
            using (WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                if (this.dict.TryGetValue(name, out world))
                {
                    return false;
                }

                world = new World(name, boundingBox, tileDimensions);
                this.dict.Add(name, world);
                return true;
            }
        }

        public bool TryGet(string name, out World world)
        {
            using (ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                return this.dict.TryGetValue(name, out world);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (World world in this.dict.Values)
                {
                    world.Dispose();
                }
            }

            this.readWriteLock.Dispose();
        }
    }
}
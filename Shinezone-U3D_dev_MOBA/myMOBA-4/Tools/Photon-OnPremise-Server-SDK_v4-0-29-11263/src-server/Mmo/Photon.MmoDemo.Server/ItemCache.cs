// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A cache for items. Each World has one item cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using ExitGames.Threading;

    /// <summary>
    /// A cache for items. Each World has one item cache.
    /// </summary>
    /// <remarks>
    /// It uses an ReaderWriterLockSlim to ensure thread safety.
    /// All members are thread safe.
    /// </remarks>
    public class ItemCache : IDisposable
    {
        private readonly Dictionary<string, Item> items = new Dictionary<string,Item>();

        private readonly int maxLockMilliseconds;

        private readonly ReaderWriterLockSlim readerWriterLock;

        public ItemCache()
        {
            this.maxLockMilliseconds = Settings.MaxLockWaitTimeMilliseconds;
            this.readerWriterLock = new ReaderWriterLockSlim();
        }

        ~ItemCache()
        {
            this.Dispose(false);
        }

        public bool AddItem(Item item)
        {
            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
            {
                if (this.items.ContainsKey(item.Id))
                {
                    return false;
                }

                this.items.Add(item.Id, item);
                return true;
            }
        }

        public bool RemoveItem(string itemId)
        {
            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
            {
                return this.items.Remove(itemId);
            }
        }

        public bool TryGetItem(string itemId, out Item item)
        {
            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
            {
                return this.items.TryGetValue(itemId, out item);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.readerWriterLock.Dispose();
                this.items.Clear();
            }

        }
   }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Photon.Hive.Caching;
using Photon.Hive.Diagnostics;
using Photon.Hive.Events;
using Photon.Hive.Operations;

namespace Photon.Hive.Collections
{
    public class RoomEventCacheManager
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected SortedList<int, RoomEventCache> eventCache = new SortedList<int, RoomEventCache>();

        protected IHiveGameAppCounters gameAppCounters = NullHiveGameAppCounters.Instance;

        private int currentSlice;

        #endregion

        #region Constructors/Destructors
        #endregion
        #region Properties
        public int Count { get { return this.eventCache.Count; } }

        public int Slice
        {
            get
            {
                return this.currentSlice;
            }
            
            set
            {
                this.AddSliceNX(value);
                this.currentSlice = value;
            }
        }

        public IEnumerable<int> Slices
        {
            get { return this.eventCache.Keys; }
        }

        public IEnumerable<CustomEvent> this[int slice]
        {
            get { return this.eventCache[slice]; }
        }
        #endregion//Properties

        public void SetGameAppCounters(IHiveGameAppCounters counters)
        {
            if (counters == null)
            {
                log.Error("Someone sets null counters to EventCache manager");
                counters = NullHiveGameAppCounters.Instance;
            }
            this.gameAppCounters = counters;
        }

        public int GetSliceSize(int slice)
        {
            return this.eventCache[slice].Count;
        }

        public CustomEvent GetCustomEvent(int slice, int number)
        {
            return this.eventCache[slice][number];
        }

        public bool HasSlice(int sliceId)
        {
            return this.eventCache.ContainsKey(sliceId);
        }

        public void AddSlice(int sliceId)
        {
            this.eventCache.Add(sliceId, new RoomEventCache());
            this.gameAppCounters.EventCacheSliceCountIncrement();
        }

        public void AddSliceNX(int sliceId)
        {
            if (!this.HasSlice(sliceId) || this.eventCache[sliceId] == null)
            {
                this.AddSlice(sliceId);
            }
        }

        public bool RemoveSlice(int slice)
        {
            if (slice == this.currentSlice)
                return false;

            IntRemoveSlice(slice);

            return true;
        }

        private bool IntRemoveSlice(int slice)
        {
            RoomEventCache cache;
            if (this.eventCache.TryGetValue(slice, out cache))
            {
                this.gameAppCounters.EventCacheTotalEventsDecrementBy(cache.Count);
                this.eventCache.Remove(slice);
                this.gameAppCounters.EventCacheSliceCountDecrement();
                return true;
            }
            return false;
        }

        public bool RemoveUpToSlice(int slice)
        {
            var result = false;

            var first = this.eventCache.Keys.First();

            for (var i = first; i < slice; i++)
            {
                if (IntRemoveSlice(i))
                {
                    result = true;
                }
            }
            return result;
        }

        public void RemoveEventsByActor(int actorNr)
        {
            foreach (var cache in this.eventCache.Values)
            {
                var count = cache.RemoveEventsByActor(actorNr);
                this.gameAppCounters.EventCacheTotalEventsDecrementBy(count);
            }
        }

        public void RemoveEventsFromCache(RaiseEventRequest raiseEventRequest)
        {
            foreach (var slice in this.eventCache.Values)
            {
                var removedCount = slice.RemoveEvents(raiseEventRequest);
                this.gameAppCounters.EventCacheTotalEventsDecrementBy(removedCount);
            }
        }

        public void RemoveEventsForActorsNotInList(IEnumerable<int> currentActorNumbers)
        {
            foreach (var slice in this.eventCache.Values)
            {
                var removedCount = slice.RemoveEventsForActorsNotInList(currentActorNumbers);
                this.gameAppCounters.EventCacheTotalEventsDecrementBy(removedCount);
            }
        }

        public void AddEvent(int slice, CustomEvent customEvent)
        {
            this.AddSliceNX(slice);
            this.eventCache[slice].AddEvent(customEvent);
            this.gameAppCounters.EventCacheTotalEventsIncrement();
        }

        public void AddEventToCurrentSlice(CustomEvent customEvent)
        {
            this.AddEvent(this.Slice, customEvent);
        }

        public void SetDeserializedData(Dictionary<int, object[]> dict)
        {
            foreach (var slice in dict)
            {
                if (this.eventCache.ContainsKey(slice.Key))
                {
                    RoomEventCache cache;
                    if (this.eventCache.TryGetValue(slice.Key, out cache))
                    {
                        this.gameAppCounters.EventCacheTotalEventsDecrementBy(cache.Count);
                    }
                    this.eventCache.Remove(slice.Key);
                }
                this.eventCache.Add(slice.Key, new RoomEventCache());
                foreach (IList<object> evdata in slice.Value)
                {
                    this.eventCache[slice.Key].AddEvent(new CustomEvent(evdata));
                    this.gameAppCounters.EventCacheTotalEventsDecrement();
                }
            }
        }

        public Dictionary<int, ArrayList> GetSerializationData(out int evCount)
        {
            evCount = 0;
            var events = new Dictionary<int, ArrayList>();
            foreach (var aslice in this.eventCache.Where(aslice => aslice.Value.Count > 0))
            {
                var evList = new ArrayList();
                foreach (var ev in aslice.Value)
                {
                    evList.Add(ev.AsList());
                    evCount++;
                }
                events.Add(aslice.Key, evList);
            }
            return events;
        }

    }
}

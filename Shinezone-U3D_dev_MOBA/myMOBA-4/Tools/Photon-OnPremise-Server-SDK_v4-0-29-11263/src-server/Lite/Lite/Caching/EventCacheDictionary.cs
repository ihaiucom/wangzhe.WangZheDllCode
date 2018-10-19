namespace Lite.Caching
{
    #region

    using System.Collections;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Dictionary implementation to store <see cref="EventCache"/> instances by actor number.
    /// </summary>
    public class EventCacheDictionary : IEnumerable<KeyValuePair<int, EventCache>>
    {
        private readonly Dictionary<int, EventCache> dictionary = new Dictionary<int, EventCache>();

        public EventCache GetOrCreateEventCache(int actorNumber)
        {
            EventCache eventCache;
            if (this.TryGetEventCache(actorNumber, out eventCache) == false)
            {
                eventCache = new EventCache();
                this.dictionary.Add(actorNumber, eventCache);
            }

            return eventCache;
        }

        public bool TryGetEventCache(int actorNumber, out EventCache eventCache)
        {
            return this.dictionary.TryGetValue(actorNumber, out eventCache);
        }

        public bool RemoveEventCache(int actorNumber)
        {
            return this.dictionary.Remove(actorNumber);
        }

        public void ReplaceEvent(int actorNumber, byte eventCode, Hashtable eventData)
        {
            var eventCache = this.GetOrCreateEventCache(actorNumber);
            if (eventData == null)
            {
                eventCache.Remove(eventCode);
            }
            else
            {
                eventCache[eventCode] = eventData;
            }
        }

        public bool RemoveEvent(int actorNumber, byte eventCode)
        {
            EventCache eventCache;
            if (!this.dictionary.TryGetValue(actorNumber, out eventCache))
            {
                return false;
            }

            return eventCache.Remove(eventCode);
        }

        public void MergeEvent(int actorNumber, byte eventCode, Hashtable eventData)
        {
            // if avent data is null the event will be removed from the cache
            if (eventData == null)
            {
                this.RemoveEvent(actorNumber, eventCode);
                return;
            }

            EventCache eventCache = this.GetOrCreateEventCache(actorNumber);

            Hashtable storedEventData;
            if (eventCache.TryGetValue(eventCode, out storedEventData) == false)
            {
                eventCache.Add(eventCode, eventData);
                return;
            }

            foreach (DictionaryEntry pair in eventData)
            {
                // null values are removed
                if (pair.Value == null)
                {
                    storedEventData.Remove(pair.Key);
                }
                else
                {
                    storedEventData[pair.Key] = pair.Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<int, EventCache>> GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }
    }
}
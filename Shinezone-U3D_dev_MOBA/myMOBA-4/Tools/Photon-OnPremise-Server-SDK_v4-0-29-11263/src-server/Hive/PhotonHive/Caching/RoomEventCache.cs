// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomEventCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The room event cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Caching
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Photon.Hive.Events;
    using Photon.Hive.Operations;

    #endregion

    [Serializable]
    public class RoomEventCache : IEnumerable<CustomEvent>
    {
        #region Constants and Fields

        private readonly List<CustomEvent> cachedRoomEvents = new List<CustomEvent>();

        #endregion

        #region Public Methods

        public CustomEvent this[int index]
        {
            get
            {
                return cachedRoomEvents[index];
            }
        }
        
        public int Count
        {
            get
            {
                return cachedRoomEvents.Count;
            }
        }

        public void AddEvents(IEnumerable<CustomEvent> e)
        {
            foreach (var ev in e)
            {
                this.AddEvent(ev);
            }
        }

        public void AddEvent(CustomEvent customeEvent)
        {
            this.cachedRoomEvents.Add(customeEvent);
        }

        public int RemoveEvents(RaiseEventRequest raiseEventRequest)
        {
            var removedCount = 0;
            if (raiseEventRequest.EvCode == 0 && raiseEventRequest.Actors == null && raiseEventRequest.Data == null)
            {
                removedCount = this.cachedRoomEvents.Count;
                this.cachedRoomEvents.Clear();
                return removedCount;
            }
            
            for (var i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                var cachedEvent = this.cachedRoomEvents[i];

                if (raiseEventRequest.EvCode != 0 && cachedEvent.Code != raiseEventRequest.EvCode)
                {
                    continue;
                }

                if (raiseEventRequest.Actors != null && raiseEventRequest.Actors.Length > 0)
                {
                    bool actorMatch = false;
                    for (int a = 0; a < raiseEventRequest.Actors.Length; a++)
                    {
                        if (cachedEvent.ActorNr != raiseEventRequest.Actors[a])
                        {
                            continue;
                        }

                        actorMatch = true;
                        break;
                    }

                    if (actorMatch == false)
                    {
                        continue;
                    }
                }

                if (raiseEventRequest.Data == null)
                {
                    ++removedCount;
                    this.cachedRoomEvents.RemoveAt(i);
                    continue;
                }

                if (Compare(raiseEventRequest.Data as Hashtable, cachedEvent.Data as Hashtable))
                {
                    ++removedCount;
                    this.cachedRoomEvents.RemoveAt(i);
                }
            }
            return removedCount;
        }

        public int RemoveEventsByActor(int actorNumber)
        {
            var removedCount = 0;
            for (var i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (this.cachedRoomEvents[i].ActorNr == actorNumber)
                {
                    this.cachedRoomEvents.RemoveAt(i);
                    ++removedCount;
                }
            }
            return removedCount;
        }

        public int RemoveEventsForActorsNotInList(IEnumerable<int> actorsNumbers)
        {
            var hashSet = new HashSet<int>(actorsNumbers);

            var removedCount = 0;
            for (var i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (hashSet.Contains(this.cachedRoomEvents[i].ActorNr) == false)
                {
                    this.cachedRoomEvents.RemoveAt(i);
                    ++removedCount;
                }
            }
            return removedCount;
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cachedRoomEvents.GetEnumerator();
        }

        #endregion

        #region IEnumerable<CustomEvent>

        public IEnumerator<CustomEvent> GetEnumerator()
        {
            return this.cachedRoomEvents.GetEnumerator();
        }

        #endregion

        #endregion

        #region Methods

        private static bool Compare(Hashtable h1, Hashtable h2)
        {
            if (h1 == null && h2 == null)
            {
                return true;
            }

            if (h1 == null || h2 == null)
            {
                return false;
            }

            foreach (DictionaryEntry entry in h1)
            {
                if (h2.ContainsKey(entry.Key) == false)
                {
                    return false;
                }

                var cachedParam = h2[entry.Key];
                if (entry.Value == null) 
                {
                    if (cachedParam != null)
                    {
                        return false;
                    }

                    continue;
                }

                if (entry.Value.Equals(cachedParam) == false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
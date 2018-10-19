// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomEventCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The room event cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Caching
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Lite.Events;
    using Lite.Operations;

    #endregion

    [Serializable]
    public class RoomEventCache : IEnumerable<CustomEvent>
    {
        #region Constants and Fields

        private readonly List<CustomEvent> cachedRoomEvents = new List<CustomEvent>();

        #endregion

        #region Public Methods

        public void AddEevents(IEnumerable<CustomEvent> e)
        {
            foreach (var ev in e)
            {
                AddEvent(ev);
            }
        }

        public void AddEvent(CustomEvent customeEvent)
        {
            this.cachedRoomEvents.Add(customeEvent);
        }

        public void RemoveEvents(RaiseEventRequest raiseEventRequest)
        {
            if (raiseEventRequest.EvCode == 0 && raiseEventRequest.Actors == null && raiseEventRequest.Data == null)
            {
                this.cachedRoomEvents.Clear();
                return;
            }
            
            for (int i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
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
                    this.cachedRoomEvents.RemoveAt(i);
                    continue;
                }

                if (Compare(raiseEventRequest.Data as Hashtable, cachedEvent.Data as Hashtable))
                {
                    this.cachedRoomEvents.RemoveAt(i);
                }
            }
        }

        public void RemoveEventsByActor(int actorNumber)
        {
            for (int i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (this.cachedRoomEvents[i].ActorNr == actorNumber)
                {
                    this.cachedRoomEvents.RemoveAt(i);
                }
            }
        }

        public void RemoveEventsForActorsNotInList(IEnumerable<int> actorsNumbers)
        {
            var hashSet = new HashSet<int>(actorsNumbers);

            for (int i = this.cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (hashSet.Contains(this.cachedRoomEvents[i].ActorNr) == false)
                {
                    this.cachedRoomEvents.RemoveAt(i);
                }
            }
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

                object cachedParam = h2[entry.Key];
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
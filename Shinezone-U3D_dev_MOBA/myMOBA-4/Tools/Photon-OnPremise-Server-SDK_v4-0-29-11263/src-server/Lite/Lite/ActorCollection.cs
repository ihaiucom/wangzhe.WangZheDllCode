// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A collection for <see cref="Actor" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Photon.SocketServer;

    /// <summary>
    /// A collection for <see cref="Actor"/>s.
    /// </summary>
    public class ActorCollection : List<Actor> 
    {
        /// <summary>
        /// Gets an actor by the actor number.
        /// </summary>
        /// <param name="actorNumber">
        /// The actor number.
        /// </param>
        /// <returns>
        /// Return the actor with the specified actor number if found.
        /// If no actor with the specified actor number exits null will be returned.
        /// </returns>
        public Actor GetActorByNumber(int actorNumber)
        {
            return this.FirstOrDefault(actor => actor.ActorNr == actorNumber);
        }

        /// <summary>
        /// Gets an actor by a specified peer.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <returns>
        /// Returns the actor for the specified peer or null 
        /// if no actor for the specified peer was found.
        /// </returns>
        public Actor GetActorByPeer(PeerBase peer)
        {
            return this.FirstOrDefault(actor => actor.Peer == peer);
        }

        /// <summary>
        /// Gets the actor numbers of all actors in this instance as an array.
        /// </summary>
        /// <returns>
        /// Array of the actor numbers.
        /// </returns>
        public IEnumerable<int> GetActorNumbers()
        {
            return this.Select(actor => actor.ActorNr);
        }

        /// <summary>
        /// Gets a list of actors in the room exluding a specified actor.
        /// This method can be used to get the actor list for an event, 
        /// where the actor causing the event should not be notified.
        /// </summary>
        /// <param name="actorToExclude">
        /// The actor to exclude.
        /// </param>
        /// <returns>
        /// the actors without <paramref name="actorToExclude"/>
        /// </returns>
        public IEnumerable<Actor> GetExcludedList(Actor actorToExclude)
        {
            return this.Where(actor => actor != actorToExclude);
        }

        /// <summary>
        /// Removes the actor for a a specified peer.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <returns>
        /// The <see cref="Actor"/> removed or <c>null</c> if no actor for the specified peer exists.
        /// </returns>
        public Actor RemoveActorByPeer(LitePeer peer)
        {
            int index = this.FindIndex(actor => actor.Peer == peer);
            if (index == -1)
            {
                return null;
            }

            Actor result = this[index];
            this.RemoveAt(index);
            return result;
        }

        /// <summary>
        /// Returns all actors with the given actor numbers.
        /// </summary>
        /// <param name="actors">
        /// The actor numbers.
        /// </param>
        /// <returns>
        /// The actors with the given actor numbers.
        /// </returns>
        public IEnumerable<Actor> GetActorsByNumbers(int[] actors)
        {
            if (!IsSorted(actors))
            {
                var clone = new int[actors.Length];
                Array.Copy(actors, clone, actors.Length);
                Array.Sort(clone);
                actors = clone;
            }

            for (int i = 0, j = 0; i < actors.Length && j < this.Count; i++)
            {
                // since both lists are sorted we don't have to start at the beginning at each iteration
                for (; j < this.Count; j++)
                {
                    if (this[j].ActorNr == actors[i])
                    {
                        yield return this[j];
                        break;
                    }
                }
            }
            
            //// return this.Where(actor => actors.Contains(actor.ActorNr));
        }

        /// <summary>
        /// Checks whether an array is sorted.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// True if the array is sorted, otherwise false.
        /// </returns>
        private static bool IsSorted(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] > array[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
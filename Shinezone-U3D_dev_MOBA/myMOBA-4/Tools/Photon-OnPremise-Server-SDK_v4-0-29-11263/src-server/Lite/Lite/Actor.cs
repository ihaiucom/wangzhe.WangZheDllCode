// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Actor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   An actor is the glue between <see cref="LitePeer" /> and <see cref="Room" />.
//   In addition to the peer it has a <see cref="ActorNr">number</see>, an <see cref="Id" /> and <see cref="Properties" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    using System.Collections.Generic;

    using Lite.Common;

    using Photon.SocketServer;

    /// <summary>
    /// An actor is the glue between <see cref="LitePeer"/> and <see cref="Room"/>.
    /// In addition to the peer it has a <see cref="ActorNr">number</see> and <see cref="Properties"/>.
    /// </summary>
    public class Actor
    {
        private readonly List<ActorGroup> groups = new List<ActorGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        public Actor()
        {
            this.Properties = new PropertyBag<object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="peer">
        /// The peer for this actor.
        /// </param>
        public Actor(PeerBase peer)
            : this()
        {
            this.Peer = peer;
        }

        /// <summary>
        /// Gets or sets the actor nr.
        /// </summary>
        public int ActorNr { get; set; }

        /// <summary>
        /// Gets or sets the peer.
        /// </summary>
        public PeerBase Peer { get; set; }

        /// <summary>
        /// Gets the actors custom properties.
        /// </summary>
        public PropertyBag<object> Properties { get; private set; }

        public override string ToString()
        {
            return string.Format("Actor {0}: Peer {1}", this.ActorNr, this.Peer);
        }

        public void AddGroup(ActorGroup group)
        {
            this.groups.Add(group);
            group.Add(this);
        }

        public void RemoveGroups(byte[] groupIds)
        {
            if (groupIds == null)
            {
                return;
            }
            if (groupIds.Length == 0)
            {
                this.RemoveAllGroups();
                return;
            }

            foreach (var group in groupIds)
            {
                this.RemoveGroup(group);
            }
        }

        private void RemoveGroup(byte group)
        {
            int actorGroupIndex = this.groups.FindIndex(g => g.GroupId == group);
            if (actorGroupIndex == -1)
            {
                return;
            }

            this.groups[actorGroupIndex].RemoveActorByPeer((LitePeer)this.Peer);
            this.groups.RemoveAt(actorGroupIndex);
        }

        private void RemoveAllGroups()
        {
            foreach (var group in this.groups)
            {
                group.RemoveActorByPeer((LitePeer)this.Peer);
            }

            this.groups.Clear();
        }
    }
}
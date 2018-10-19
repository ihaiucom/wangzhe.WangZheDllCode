// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Actor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Photon.Hive.Serialization;

namespace Photon.Hive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Photon.Hive.Plugin;

    /// <summary>
    /// An actor is the glue between <see cref="HivePeer"/> and <see cref="Room"/>.
    /// In addition to the peer it has a <see cref="ActorNr">number</see> and <see cref="Properties"/>.
    /// </summary>
    [Serializable]
    public class Actor : IActor
    {
        #region Constants and Fields

        private readonly List<ActorGroup> groups = new List<ActorGroup>();

        private readonly PropertyBag<object> properties = new PropertyBag<object>();

        public const byte BinrayPropertiesId = 1;
        public static readonly string ActorNrKey = HiveHostActorState.ActorNr.ToString();
        public static readonly string UserIdKey = HiveHostActorState.UserId.ToString();
        public static readonly string BinaryKey = HiveHostActorState.Binary.ToString();
        public static readonly string NicknameKey = HiveHostActorState.Nickname.ToString();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="peer">
        /// The peer for this actor.
        /// </param>
        public Actor(HivePeer peer)
        {
            this.Peer = peer;

            // we copy the UserId so it doesn't get lost on peer disconnect
            this.UserId = peer.UserId;
        }

        public Actor(int actorNr, string userId)
        {
            this.ActorNr = actorNr;
            this.UserId = userId;
        }

        protected Actor(int actorNr, string userId, IDictionary properties, DateTime? deactivationTime)
            : this(actorNr, userId)
        {
            if (properties != null)
            {
                this.Properties.SetProperties(properties);
            }
            this.DeactivationTime = deactivationTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the actor nr.
        /// </summary>
        public int ActorNr
        {
            get; set;
        }

        /// <summary>
        /// Reference to timer which will clean Actor after disconnect
        /// </summary>
        public IDisposable CleanUpTimer { get; set; }

        /// <summary>
        /// Time stamp when actor was deactivated
        /// </summary>
        public DateTime? DeactivationTime { get; set; }

        /// <summary>
        /// Gets or sets the peer.
        /// </summary>
        public HivePeer Peer { get; set; }

        public bool IsInactive { get { return this.Peer == null; } }
        public bool IsActive { get { return !this.IsInactive; } }
        /// <summary>
        /// Gets the actors custom properties.
        /// </summary>
        public PropertyBag<object> Properties
        {
            get
            {
                return this.properties;
            }
        }

        public string UserId { get; private set; }

        public string Nickname {
            get
            {
                //well known actor property nickname
                var prop = this.Properties.GetProperty((byte)255);
                if (prop == null)
                {
                    return string.Empty;
                }

                if (prop.Value == null)
                {
                    return string.Empty;
                }

                return prop.Value.ToString();
                //return prop != null ? (prop.Value != null ? prop.Value.ToString(): null) : null;
            }
        }

#if PLUGINS_0_9
        [Obsolete("Use Nickname instead")]
        public string Username { get { return this.Nickname; } }
#endif

        public object Secure
        {
            get
            {
                if (this.Peer != null)
                {
                    return this.Peer.AuthCookie;
                }
                return null;
            }
        }
        #endregion

        #region Indexers

        public object this[object key]
        {
            get
            {
                object result;
                if (this.properties.TryGetValue(key, out result))
                {
                    return result;
                }

                return null;
            }

            set
            {
                this.properties.Set(key, value);
            }
        }

        #endregion

        #region Public Methods

        public void Deactivate()
        {
            // remove actore from all joined groups
            // the curent list of joined groups will be kept to rejoin 
            // the groups if the peer reconnects to the game. 
            foreach(var group in this.groups)
            {
                group.Remove(this);
            }

            this.Peer = null;

            this.DeactivationTime = DateTime.Now;
        }

        public void Reactivate(HivePeer peer)
        {
            this.Peer = peer;

            // rejoin to all previously joined groups
            foreach(var group in this.groups)
            {
                group.Add(this);
            }

            if (this.CleanUpTimer != null)
            {
                this.CleanUpTimer.Dispose();
                this.CleanUpTimer = null;
            }

            this.DeactivationTime = null;
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

        public override string ToString()
        {
            return string.Format("Actor: {0}; Peer:'{1}', DeactivationTime:'{2}', UserId:'{3}',nick:'{4}'", 
                this.ActorNr, this.Peer, this.DeactivationTime, this.UserId, this.Nickname);
        }

        public static Actor Deserialize(SerializableActor a)
        {
            IDictionary properties = null;
            if (!string.IsNullOrEmpty(a.Binary))
            {
                var dic = (IDictionary)Serializer.DeserializeBase64(a.Binary);
                properties = (IDictionary)dic[BinrayPropertiesId];
            }
            return new Actor(a.ActorNr, a.UserId, properties, a.DeactivationTime);
        }

        public SerializableActor Serialize(bool withDebugInfo)
        {
            bool? isActive = null;
            if (this.IsActive)
            {
                isActive = true;
            }

            var item = new SerializableActor
                               {
                                   ActorNr = this.ActorNr,
                                   UserId = this.UserId,
                                   IsActive = isActive,
                                   Nickname = this.Nickname,
                                   DeactivationTime = this.DeactivationTime,
                               };
            if (this.Properties.Count > 0)
            {
                var actorBinState = new Dictionary<byte, object>
                    {
                        { BinrayPropertiesId, this.Properties.AsDictionary().ToDictionary(prop => prop.Key, prop => prop.Value.Value) }
                    };
                item.Binary = Serializer.SerializeBase64(actorBinState);
                if (withDebugInfo)
                {
                    item.DEBUG_BINARY = actorBinState;
                }
            }
            return item;
        }

        #endregion

        #region Methods

        private void RemoveAllGroups()
        {
            foreach (var group in this.groups)
            {
                group.RemoveActorByPeer(this.Peer);
            }

            this.groups.Clear();
        }

        private void RemoveGroup(byte group)
        {
            var actorGroupIndex = this.groups.FindIndex(g => g.GroupId == group);
            if (actorGroupIndex == -1)
            {
                return;
            }

            this.groups[actorGroupIndex].RemoveActorByPeer(this.Peer);
            this.groups.RemoveAt(actorGroupIndex);
        }

        #endregion
    }

    public static class ActorExtension
    {
        public static SerializableActor ToSerializableActor(this Dictionary<string, object> d)
        {
            var res = new SerializableActor
            {
                ActorNr = (int) d[Actor.ActorNrKey],
                UserId = d.ContainsKey(Actor.UserIdKey) ? (string) d[Actor.UserIdKey] : string.Empty,
                Nickname = d.ContainsKey(Actor.NicknameKey) ? (string) d[Actor.NicknameKey] : string.Empty,
                Binary = d.ContainsKey(Actor.BinaryKey) ? (string) d[Actor.BinaryKey] : string.Empty
            };

            if (d.ContainsKey("DEBUG_BINARY"))
            {
                var debug_bin = d["DEBUG_BINARY"] as Dictionary<string, object>;
                if (debug_bin != null)
                {
                    res.DEBUG_BINARY =  new Dictionary<byte, object>();
                    foreach (var entry in debug_bin)
                    {
                        var key = Convert.ToByte(entry.Key);
                        res.DEBUG_BINARY.Add(key, entry.Value);
                    }
                }
            }
            return res;
        }

        public static Dictionary<string, object> ToDictionary(this SerializableActor actor)
        {
            return new Dictionary<string, object>
            {
                {Actor.ActorNrKey, actor.ActorNr},
                {Actor.BinaryKey, actor.Binary},
                {Actor.NicknameKey, actor.Nickname},
                {Actor.UserIdKey, actor.UserId},
                {"DEBUG_BINARY", actor.DEBUG_BINARY},
            };
        }
    }
}
using System.Collections;
using System.Collections.Generic;

namespace Photon.Hive.Collections
{
    public class GroupManager
    {
        protected Dictionary<byte, ActorGroup> actorGroups = new Dictionary<byte, ActorGroup>();

        public int Count { get { return this.actorGroups.Count; }}

        public ActorGroup GetActorGroup(byte groupId)
        {
            ActorGroup group;
            if (!this.actorGroups.TryGetValue(groupId, out group))
            {
                group = new ActorGroup(groupId);
                this.actorGroups.Add(groupId, group);
            }

            return group;
        }

        public void AddToAllGroups(Actor actor)
        {
            // Add is [] => add to all groups? 
            foreach (var group in this.actorGroups.Values)
            {
                actor.AddGroup(group);
            }
        }

        public void AddActorToGroup(byte groupId, Actor actor)
        {
            var group = this.GetActorGroup(groupId);
            actor.AddGroup(group);
        }

        public Dictionary<byte, ArrayList> GetDataForSerialization()
        {
            var result = new Dictionary<byte, ArrayList>();
            foreach (var group in this.actorGroups)
            {
                result.Add(group.Key, new ArrayList());
                foreach (var actor in group.Value)
                {
                    result[group.Key].Add(actor.ActorNr);
                }
            }
            return result;
        }
    }
}

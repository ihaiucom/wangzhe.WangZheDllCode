namespace Lite
{
    public class ActorGroup : ActorCollection
    {
        public ActorGroup(byte id)
        {
            GroupId = id;
        }

        public byte GroupId { get; private set; }
    }
}

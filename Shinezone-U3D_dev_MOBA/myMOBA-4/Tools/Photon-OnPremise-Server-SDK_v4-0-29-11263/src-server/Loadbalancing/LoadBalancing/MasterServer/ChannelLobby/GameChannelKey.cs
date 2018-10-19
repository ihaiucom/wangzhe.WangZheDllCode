
namespace Photon.LoadBalancing.MasterServer.ChannelLobby
{
    using System;
    using System.Collections;

    using Photon.LoadBalancing.Common;

    public class GameChannelKey : IEquatable<GameChannelKey>
    {
        public readonly Hashtable Properties;

        private readonly int hashcode;

        public GameChannelKey(Hashtable properties)
        {
            this.Properties = properties;
            this.hashcode = DictionaryExtensions.GetHashCode(this.Properties);
        }

        public override int GetHashCode()
        {
            return this.hashcode;
        }

        public bool Equals(GameChannelKey other)
        {
            return DictionaryExtensions.Equals(this.Properties, other.Properties);
        }
    }
}

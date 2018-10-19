using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class MasterClientIdPlugin : PluginBase
    {
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected override void OnChangeMasterClientId(int oldId, int newId)
        {
            var d = new Dictionary<byte, object>
            {
                {0, "MasterClientId changed"}
            };
            this.BroadcastEvent(123, d);
        }
    }
}

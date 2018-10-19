using System.Collections;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class ChangeGamePropertiesOnJoinPlugin : TestPluginBase
    {
        public override void OnJoin(IJoinGameCallInfo info)
        {
            if (this.PluginHost.GameActors.Count == 2)
            {
                PluginHost.SetProperties(0, new Hashtable() { { (byte)254, false }, { (byte)253, false } }, null, true);
            }
            base.OnJoin(info);
        }
    }
}

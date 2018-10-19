using System.Collections;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class BanTestPlugin : TestPluginBase
    {
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            base.OnRaiseEvent(info);
            var data = (Hashtable) info.Request.Data;
            if ((bool) data[0])
            {
                //lets ban
                this.PluginHost.RemoveActor((int)data[1], RemoveActorReason.Banned, "bb");
            }
        }
    }
}

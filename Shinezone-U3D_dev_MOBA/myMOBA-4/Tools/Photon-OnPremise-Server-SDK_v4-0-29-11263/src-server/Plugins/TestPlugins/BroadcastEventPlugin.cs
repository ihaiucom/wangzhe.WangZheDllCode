using Photon.Hive.Plugin;

namespace TestPlugins
{
    class BroadcastEventPlugin : TestPluginBase
    {
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode == 1)
            {
                this.PluginHost.BroadcastEvent(new[] { 2, 25 }, 0, 1, null, 0);
            }
            else if (info.Request.EvCode == 2)
            {
                this.PluginHost.BroadcastEvent(new[] { 2, 3 }, 0, 1, null, 0);
            }
            base.OnRaiseEvent(info);
        }
    }
}

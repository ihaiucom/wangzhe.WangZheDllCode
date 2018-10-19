using Photon.Hive.Plugin;

namespace TestPlugins
{
    class RemovingActorPlugin : TestPluginBase
    {
        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.PluginHost.RemoveActor(info.ActorNr, RemoveActorReason.Kick, "");
            base.OnLeave(info);
        }
    }
}

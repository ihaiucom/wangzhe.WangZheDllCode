using System.Threading;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class SyncAsyncHttpTestPlugin : PluginBase
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode != 3)
            {
                var request = new HttpRequest
                {
                    Async = info.Request.EvCode == 1,
                    Url = "http://photon-forward.webscript.io/GameEvent",
                    Callback = this.HttpRequestCallback,
                    UserState = info,
                };
                // just to give next event time to reach plugin
                Thread.Sleep(100);
                this.PluginHost.HttpRequest(request);
            }
            else
            {
                this.PluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, info.Request.EvCode, null, 0);
            }
        }

        private void HttpRequestCallback(IHttpResponse response, object userState)
        {
            var info = (IRaiseEventCallInfo) userState;
            this.PluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, info.Request.EvCode, null, 0);
        }
    }
}

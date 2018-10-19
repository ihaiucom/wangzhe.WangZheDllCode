using Photon.Hive.Plugin;

namespace TestPlugins
{
    class WrongUrlTestPlugin : TestPluginBase
    {
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.PluginHost.HttpRequest(new HttpRequest
            {
                Url = "WrongUrl",
                Callback = this.HttpRequestCallback
            });
            base.OnRaiseEvent(info);
        }

        void HttpRequestCallback(IHttpResponse response, object userState)
        {
            if (response.Status == HttpRequestQueueResult.Error)
            {
                this.PluginHost.BroadcastErrorInfoEvent(response.Reason);
            }
        }

    }
}

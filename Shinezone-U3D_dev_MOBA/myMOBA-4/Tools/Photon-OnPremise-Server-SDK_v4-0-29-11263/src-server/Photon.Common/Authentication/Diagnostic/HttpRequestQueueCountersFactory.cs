using Photon.SocketServer.Net;

namespace Photon.Common.Authentication.Diagnostic
{
    public interface IHttpRequestQueueCountersFactory
    {
        IHttpRequestQueueCounters Create(string instanceName);
    }

    public class HttpRequestQueueCountersFactory : IHttpRequestQueueCountersFactory
    {
        public IHttpRequestQueueCounters Create(string instanceName)
        {
            return new HttpRequestQueueCounters(instanceName);
        }
    }
}

using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.Net;

namespace Photon.Common.Authentication.Diagnostic
{
    // subject to move into PhotonSocketServer
    public class HttpRequestQueueCounters : IHttpRequestQueueCounters
    {
        private static readonly IHttpQueueCountersInstance _Total;
        private readonly IHttpQueueCountersInstance instance;

        static HttpRequestQueueCounters()
        {
            _Total = HttpQueuePerformanceCounters.GetInstance("_Total");
        }

        public HttpRequestQueueCounters(string instanceName)
        {
            this.instance = HttpQueuePerformanceCounters.GetInstance(instanceName);
        }

        public virtual void HttpQueueRequestsIncrement()
        {
            _Total.IncrementQueueRequests();
            this.instance.IncrementQueueRequests();
        }

        public virtual void HttpQueueResponsesIncrement()
        {
            _Total.IncrementQueueResponses();
            this.instance.IncrementQueueResponses();
        }

        public virtual void HttpQueueSuccessIncrement()
        {
            _Total.IncrementQueueSuccesses();
            this.instance.IncrementQueueSuccesses();
        }

        public virtual void HttpQueueTimeoutIncrement()
        {
            _Total.IncrementQueueQueueTimeouts();
            this.instance.IncrementQueueQueueTimeouts();
        }

        public virtual void HttpQueueErrorsIncrement()
        {
            _Total.IncrementQueueErrors();
            this.instance.IncrementQueueErrors();
        }

        public virtual void HttpQueueOfflineResponsesIncrement()
        {
            _Total.IncrementQueueOfflineResponses();
            this.instance.IncrementQueueOfflineResponses();
        }

        public virtual void HttpQueueConcurrentRequestsIncrement()
        {
            _Total.IncrementQueueConcurrentRequests();
            this.instance.IncrementQueueConcurrentRequests();
        }

        public virtual void HttpQueueConcurrentRequestsDecrement()
        {
            _Total.DecrementQueueConcurrentRequests();
            this.instance.DecrementQueueConcurrentRequests();
        }

        public virtual void HttpQueueQueuedRequestsIncrement()
        {
            _Total.IncrementQueueQueuedRequests();
            this.instance.IncrementQueueQueuedRequests();
        }

        public virtual void HttpQueueQueuedRequestsDecrement()
        {
            _Total.DecrementQueueQueuedRequests();
            this.instance.DecrementQueueQueuedRequests();
        }

        public virtual void HttpRequestExecuteTimeIncrement(long ticks)
        {
            _Total.IncrementHttpRequestExecutionTime(ticks);
            this.instance.IncrementHttpRequestExecutionTime(ticks);
        }

        public virtual void HttpQueueOnlineQueueCounterIncrement()
        {
            _Total.IncrementQueueOnlineQueue();
            this.instance.IncrementQueueOnlineQueue();
        }

        public virtual void HttpQueueOnlineQueueCounterDecrement()
        {
            _Total.DecrementQueueOnlineQueue();
            this.instance.DecrementQueueOnlineQueue();
        }

        public virtual void HttpQueueBackedoffRequestsIncrement()
        {
            _Total.IncrementBackedOffRequests();
            this.instance.IncrementBackedOffRequests();
        }

        public virtual void HttpQueueBackedoffRequestsDecrement()
        {
            _Total.DecrementBackedOffRequests();
            this.instance.DecrementBackedOffRequests();
        }

        public virtual void HttpRequestIncrement()
        {
            _Total.IncrementHttpResponses();
            this.instance.IncrementHttpResponses();
        }

        public virtual void HttpSuccessIncrement()
        {
            _Total.IncrementHttpSuccesses();
            this.instance.IncrementHttpSuccesses();
        }

        public virtual void HttpTimeoutIncrement()
        {
            _Total.IncrementHttpRequestTimeouts();
            this.instance.IncrementHttpRequestTimeouts();
        }

        public virtual void HttpErrorsIncrement()
        {
            _Total.IncrementHttpErrors();
            this.instance.IncrementHttpErrors();
        }
    }
}

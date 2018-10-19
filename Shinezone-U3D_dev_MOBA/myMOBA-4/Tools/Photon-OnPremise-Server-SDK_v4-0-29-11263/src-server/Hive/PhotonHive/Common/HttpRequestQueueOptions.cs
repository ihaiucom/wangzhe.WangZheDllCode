namespace Photon.Hive.Common
{
    public class HttpRequestQueueOptions
    {

        public const int HTTP_QUEUE_MAX_ERRORS = 1;
        public const int HTTP_QUEUE_MAX_TIMEOUTS = 1;
        public const int HTTP_QUEUE_REQUEST_TIMEOUT = 10000;
        public const int HTTP_QUEUE_QUEUE_TIMEOUT = 30000;
        public const int HTTP_QUEUE_MAX_BACKOFF_TIME = 10000;
        public const int HTTP_QUEUE_RECONNECT_INTERVAL = 60000;
        public const int HTTP_QUEUE_MAX_QUEUED_REQUESTS = 5000;
        public const int HTTP_QUEUE_MAX_CONCURRENT_REQUESTS = 1;

        private readonly int httpQueueMaxErrors = HTTP_QUEUE_MAX_ERRORS;
        private readonly int httpQueueMaxTimeouts = HTTP_QUEUE_MAX_TIMEOUTS;
        private readonly int httpQueueRequestTimeout = HTTP_QUEUE_REQUEST_TIMEOUT;
        private readonly int httpQueueQueueTimeout = HTTP_QUEUE_QUEUE_TIMEOUT;
        private readonly int httpQueueMaxBackoffTime = HTTP_QUEUE_MAX_BACKOFF_TIME;
        private readonly int httpQueueReconnectInterval = HTTP_QUEUE_RECONNECT_INTERVAL;
        private readonly int httpQueueMaxQueuedRequests = HTTP_QUEUE_MAX_QUEUED_REQUESTS;
        private readonly int httpQueueMaxConcurrentRequests = HTTP_QUEUE_MAX_CONCURRENT_REQUESTS;

        public HttpRequestQueueOptions(
                int httpQueueMaxErrors = HTTP_QUEUE_MAX_ERRORS,
                int httpQueueMaxTimeouts = HTTP_QUEUE_MAX_TIMEOUTS,
                int httpQueueRequestTimeout = HTTP_QUEUE_REQUEST_TIMEOUT,
                int httpQueueQueueTimeout = HTTP_QUEUE_QUEUE_TIMEOUT,
                int httpQueueMaxBackoffTime = HTTP_QUEUE_MAX_BACKOFF_TIME,
                int httpQueueReconnectInterval = HTTP_QUEUE_RECONNECT_INTERVAL,
                int httpQueueMaxQueuedRequests = HTTP_QUEUE_MAX_QUEUED_REQUESTS,
                int httpQueueMaxConcurrentRequests = HTTP_QUEUE_MAX_CONCURRENT_REQUESTS
            )
        {
            this.httpQueueMaxErrors = httpQueueMaxErrors;// it is ok to have 0
            this.httpQueueMaxTimeouts = httpQueueMaxTimeouts;// it is ok to have 0

            if (httpQueueRequestTimeout != 0)
            {
                this.httpQueueRequestTimeout = httpQueueRequestTimeout;
            }

            if (httpQueueQueueTimeout != 0)
            {
                this.httpQueueQueueTimeout = httpQueueQueueTimeout;
            }

            if (httpQueueMaxBackoffTime != 0)
            {
                this.httpQueueMaxBackoffTime = httpQueueMaxBackoffTime;
            }

            if (httpQueueReconnectInterval != 0)
            {
                this.httpQueueReconnectInterval = httpQueueReconnectInterval;
            }

            if (httpQueueMaxQueuedRequests != 0)
            {
                this.httpQueueMaxQueuedRequests = httpQueueMaxQueuedRequests;
            }

            if (httpQueueMaxConcurrentRequests != 0)
            {
                this.httpQueueMaxConcurrentRequests = httpQueueMaxConcurrentRequests;
            }
        }

        public int HttpQueueQueueTimeout
        {
            get { return this.httpQueueQueueTimeout; }
        }

        public int HttpQueueMaxErrors
        {
            get { return this.httpQueueMaxErrors; }
        }

        public int HttpQueueMaxTimeouts
        {
            get { return this.httpQueueMaxTimeouts; }
        }

        public int HttpQueueRequestTimeout
        {
            get { return this.httpQueueRequestTimeout; }
        }

        public int HttpQueueMaxBackoffTime
        {
            get { return this.httpQueueMaxBackoffTime; }
        }

        public int HttpQueueReconnectInterval
        {
            get { return this.httpQueueReconnectInterval; }
        }

        public int HttpQueueMaxQueuedRequests
        {
            get { return this.httpQueueMaxQueuedRequests; }
        }

        public int HttpQueueMaxConcurrentRequests
        {
            get { return this.httpQueueMaxConcurrentRequests; }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientAuthenticationQueue.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpRequestQueueResultCode2 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.SocketServer.Net;

namespace Photon.Common.Authentication.CustomAuthentication
{
    public class AsyncHttpResponse
    {
        public AsyncHttpResponse(HttpRequestQueueResultCode status, bool rejectIfUnavailable, object state)
        {
            this.Status = status;
            this.State = state;
            this.RejectIfUnavailable = rejectIfUnavailable; 
        }

        public HttpRequestQueueResultCode Status { get; private set; }

        public object State { get; set; }

        public byte[] ResponseData { get; set; }

        public bool RejectIfUnavailable { get; set; }
    }

    public class ClientAuthenticationQueue
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly string Uri;

        public readonly string QueryStringParameters; 
   
        public readonly bool RejectIfUnavailable; 

        public readonly RoundRobinCounter RequestTimeCounter = new RoundRobinCounter(100);

        private readonly PoolFiber fiber;

        private readonly int requestTimeoutMilliseconds;

        private readonly RoundRobinCounter timeoutCounter = new RoundRobinCounter(100);

        private readonly HttpRequestQueue httpRequestQueue;

        public ClientAuthenticationQueue(string uri, string queryStringParameters, bool rejectIfUnavailable, int requestTimeout)
        {
            this.Uri = uri;
            this.QueryStringParameters = queryStringParameters; 

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Create authentication queue for adress {0}", this.Uri);
            }

            this.requestTimeoutMilliseconds = requestTimeout;
            this.RejectIfUnavailable = rejectIfUnavailable; 

            this.fiber = new PoolFiber();

            this.httpRequestQueue = new HttpRequestQueue(this.fiber);
        }

        #region Properties
        public int CurrentRequests { get { return this.httpRequestQueue.RunningRequestsCount; } }

        public TimeSpan ReconnectInterval
        {
            get
            {
                return this.httpRequestQueue.ReconnectInterval;
            }
            set
            {
                this.httpRequestQueue.ReconnectInterval = value;
            }
        }

        public TimeSpan QueueTimeout
        {
            get
            {
                return this.httpRequestQueue.QueueTimeout;
            }

            set
            {
                this.httpRequestQueue.QueueTimeout = value;
            }
        }

        public int MaxQueuedRequests
        {
            get
            {
                return this.httpRequestQueue.MaxQueuedRequests;
            }
            set
            {
                this.httpRequestQueue.MaxQueuedRequests = value;
            }
        }

        public int MaxConcurrentRequests
        {
            get
            {
                return this.httpRequestQueue.MaxConcurrentRequests;
            }

            set
            {
                this.httpRequestQueue.MaxConcurrentRequests = value;
            }
        }

        public int MaxErrorRequests
        {
            get
            {
                return this.httpRequestQueue.MaxErrorRequests;
            }

            set
            {
                this.httpRequestQueue.MaxErrorRequests = value;
            }
        }

        public int MaxTimedOutRequests
        {
            get
            {
                return this.httpRequestQueue.MaxTimedOutRequests;
            }

            set
            {
                this.httpRequestQueue.MaxTimedOutRequests = value;
            }
        }

        public int MaxBackoffTimeInMiliseconds
        {
            get
            {
                return this.httpRequestQueue.MaxBackoffInMilliseconds;
            }

            set
            {
                this.httpRequestQueue.MaxBackoffInMilliseconds = value;
            }
        }

        public object CustomData { get; set; }

        #endregion

        public void SetHttpRequestQueueCounters(IHttpRequestQueueCounters counters)
        {
            this.httpRequestQueue.SetCounters(counters);
        }

        public void EnqueueRequest(string clientQueryStringParamters, byte[] postData, Action<AsyncHttpResponse, ClientAuthenticationQueue> callback, object state)
        {
            this.fiber.Enqueue(() => this.ExecuteRequest(clientQueryStringParamters, postData, callback, state));
        }

        private void ExecuteRequest(string clientQueryStringParamters, byte[] postData, Action<AsyncHttpResponse, ClientAuthenticationQueue> callback, object state)
        {
            var clientAuthenticationRequestUrl = string.Empty;
            try
            {
                clientAuthenticationRequestUrl = this.ConcatenateQueryString(this.Uri, new[] { this.QueryStringParameters, clientQueryStringParamters });

                var webRequest = (HttpWebRequest)WebRequest.Create(clientAuthenticationRequestUrl);
                webRequest.Proxy = null;
                webRequest.Timeout = this.requestTimeoutMilliseconds;

                HttpRequestQueueCallback queueCallback = 
                    (result, httpRequest, userState) => 
                        this.fiber.Enqueue(() => this.OnCallback(result, httpRequest, userState, callback));


                if (postData != null)
                {
                    webRequest.Method = "POST";
                    this.httpRequestQueue.Enqueue(webRequest, postData, queueCallback, state);
                }
                else
                {
                    webRequest.Method = "GET";
                    this.httpRequestQueue.Enqueue(webRequest, queueCallback, state);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception during ExecuteRequest to url '{0}'. Exception:{1}", clientAuthenticationRequestUrl, ex);
                ThreadPool.QueueUserWorkItem(delegate { callback(new AsyncHttpResponse(HttpRequestQueueResultCode.Error, this.RejectIfUnavailable, state), this); });
            }
        }

        private void OnCallback(HttpRequestQueueResultCode resultCode, 
            AsyncHttpRequest result, object userState, Action<AsyncHttpResponse, ClientAuthenticationQueue> userCallback)
        {
            try
            {
                var url = result.WebRequest.RequestUri;
                byte[] responseData = null;
                var status = result.Status;
                var exception = result.Exception;

                this.RequestTimeCounter.AddValue((int)result.Elapsedtime.TotalMilliseconds);
                if (result.Response != null)
                {
                    responseData = result.Response;
                }

                byte[] resultResponseData = null;
                switch (resultCode)
                {
                    case HttpRequestQueueResultCode.Success:
                    {
                        if (log.IsDebugEnabled)
                        {
                            var responseString = string.Empty;
                            if (responseData != null)
                            {
                                responseString = Encoding.UTF8.GetString(responseData);
                            }

                            log.DebugFormat(
                                "Custom authentication result: uri={0}, status={1}, msg={2}, data={3}",
                                url,
                                status,
                                exception != null ? exception.Message : string.Empty,
                                responseString);
                        }

                        this.timeoutCounter.AddValue(0);
                        resultResponseData = responseData;
                    }
                        break;
                    case HttpRequestQueueResultCode.RequestTimeout:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Custom authentication timed out: uri={0}, status={1}, msg={2}",
                                url, status, exception != null ? exception.Message : string.Empty);
                        }
                        this.timeoutCounter.AddValue(1);
                    }
                        break;
                    case HttpRequestQueueResultCode.QueueFull:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat(
                                "Custom authentication error: queue is full. Requests count {0}, url:{1}, msg:{2}",
                                this.httpRequestQueue.QueuedRequestCount, url,
                                exception != null ? exception.Message : string.Empty);
                        }
                    }
                        break;
                    case HttpRequestQueueResultCode.QueueTimeout:
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Custom authentication error: Queue timedout. uri={0}, status={1}, msg={2}",
                                url, status, exception != null ? exception.Message : string.Empty);
                        }
                        break;
                    case HttpRequestQueueResultCode.Error:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Custom authentication error: uri={0}, status={1}, msg={2}",
                                url, status, exception != null ? exception.Message : string.Empty);
                        }
                    }
                        break;
                    case HttpRequestQueueResultCode.Offline:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Custom auth error. Queue is offline. url:{0}, status{1}, msg:{2}",
                                url, status, exception != null ? exception.Message : string.Empty);
                        }
                    }
                        break;
                }

                var response = new AsyncHttpResponse(resultCode, this.RejectIfUnavailable, userState)
                    {
                        ResponseData = resultResponseData
                    };

                ThreadPool.QueueUserWorkItem(delegate { userCallback(response, this); });
                return;
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception happened:{0}", e);
            }


            ThreadPool.QueueUserWorkItem(delegate { userCallback(new AsyncHttpResponse(HttpRequestQueueResultCode.Error, this.RejectIfUnavailable, userState), this); });
        }

        private string ConcatenateQueryString(string queryString, IEnumerable<string> queryStringsToAppend)
        {
            string result = queryString;

            foreach (var s in queryStringsToAppend)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (!result.Contains("?"))
                    {
                        result += "?";
                    }
                    else
                    {
                        if (!result.EndsWith("&"))
                        {
                            result += "&";
                        }
                    }

                    result += s;
                }
            }

            return result; 
        }

        public class RoundRobinCounter
        {
            private readonly int[] values;
            private int sum;
            private int pos;
            private int count;

            public RoundRobinCounter(int size)
            {
                this.values = new int[size];
            }

            public int Sum
            {
                get { return this.sum; }
            }

            public int Average
            {
                get { return this.Sum / (this.count > 0 ? this.count : 1); }
            }

            public void AddValue(int v)
            {
                if (this.count < this.values.Length)
                {
                    this.count++;
                }

                this.sum -= this.values[this.pos];  
                this.sum += v;
                this.values[this.pos] = v;
                this.pos = this.pos + 1;

                if (this.pos >= this.values.Length)
                {
                    this.pos = 0;
                }
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcHandler.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using ExitGames.Logging;
using Photon.Common;
using Photon.Hive.Common;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.Net;
using SendParameters = Photon.SocketServer.SendParameters;

namespace Photon.Hive.WebRpc
{
    public interface IRpcHandlerAppCounters
    {
        void IncrementWebHooskHttpSuccessCount();

        void IncrementWebHooskHttpErrorsCount();

        void IncrementWebHooskHttpTimeoutCount();

        void IncrementWebHooskQueueSuccessCount();

        void IncrementWebHooskQueueErrorsCount();

        void IncrementWebHooksHttpRequestExecTime(uint millisecods);
    }

    public class WebRpcHandler:  IHttpRequestQueueCounters
    {
        #region Fields
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly HttpRequestQueue httpRequestQueue;

        private readonly Dictionary<string, object> environment;

        private readonly string baseUrl;

        protected static readonly IHttpQueueCountersInstance _Total = HttpQueuePerformanceCounters.GetInstance("_Total");
        protected static readonly IHttpQueueCountersInstance countersInstance =
            HttpQueuePerformanceCounters.GetInstance(ApplicationBase.Instance.PhotonInstanceName + "_rpc");

        private static readonly IRpcHandlerAppCounters nullAppCounters = new NullAppCounters();
        private readonly IRpcHandlerAppCounters rpcAppCounters;
        private readonly int httpRequestTimeout;

        #endregion//Fields

        public WebRpcHandler(string baseUrl,
            Dictionary<string, object> environment,
            IRpcHandlerAppCounters rpcAppCounters = null,
            HttpRequestQueueOptions httpRequestQueueOptions = null)
            : this(baseUrl, environment, new HttpRequestQueue(), rpcAppCounters, httpRequestQueueOptions)
        {
        }

        public WebRpcHandler(string baseUrl,
            Dictionary<string, object> environment, HttpRequestQueue queue,
            IRpcHandlerAppCounters rpcAppCounters = null, 
            HttpRequestQueueOptions httpRequestQueueOptions = null)
        {
            this.rpcAppCounters = rpcAppCounters ?? nullAppCounters;
            this.baseUrl = baseUrl;
            this.environment = environment;

            if (httpRequestQueueOptions == null)
            {
                httpRequestQueueOptions = new HttpRequestQueueOptions();
            }

            this.httpRequestQueue = queue;
            this.httpRequestQueue.SetCounters(this);

            this.httpRequestQueue.MaxErrorRequests = httpRequestQueueOptions.HttpQueueMaxTimeouts;
            this.httpRequestQueue.MaxTimedOutRequests = httpRequestQueueOptions.HttpQueueMaxErrors;
            this.httpRequestQueue.ReconnectInterval = TimeSpan.FromMilliseconds(httpRequestQueueOptions.HttpQueueReconnectInterval);
            this.httpRequestQueue.QueueTimeout = TimeSpan.FromMilliseconds(httpRequestQueueOptions.HttpQueueQueueTimeout);
            this.httpRequestQueue.MaxQueuedRequests = httpRequestQueueOptions.HttpQueueMaxQueuedRequests;
            this.httpRequestQueue.MaxBackoffInMilliseconds = httpRequestQueueOptions.HttpQueueMaxBackoffTime;
            this.httpRequestQueue.MaxConcurrentRequests = httpRequestQueueOptions.HttpQueueMaxConcurrentRequests;

            this.httpRequestTimeout = httpRequestQueueOptions.HttpQueueRequestTimeout;
        }

        #region Public Methods

        public bool HandleCall(PeerBase peer, string userId, OperationRequest request, object authResultsSecure, SendParameters sp)
        {
            var rpcRequest = new WebRpcRequest(peer.Protocol, request);
            if (!rpcRequest.IsValid)
            {
                var msg = string.Format("Invalid RPC request format: {0}", rpcRequest.GetErrorMessage());
                Log.Error(msg);
                this.SendErrorResponse(peer, request.OperationCode, sp, (short)ErrorCode.OperationInvalid, msg);
                return false;
            }

            try
            {
                var data = this.SerializeRequest(rpcRequest, userId, authResultsSecure);

                var uri = this.MakeRequestUri(rpcRequest);

                return this.SendHttpRequest(peer, uri, data, rpcRequest);
            }
            catch (Exception e)
            {
                var msg = string.Format("Exception during RPC request handling {0}", e);
                Log.Error(msg);
                this.SendErrorResponse(peer, request.OperationCode, sp, msg: msg);
            }

            return true;
        }
        #endregion //Public Methods

        #region Private Methods

        private bool SendHttpRequest(PeerBase peer, string uri, string data, WebRpcRequest rpcRequest)
        {
            var binData = Encoding.UTF8.GetBytes(data);

            HttpRequestQueueCallback callback =
                (result, request, state) => this.HandleHttpResponse(peer, rpcRequest.OperationRequest.OperationCode, result, rpcRequest, request, uri);


            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Proxy = null;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Accept = "application/json";
            webRequest.Timeout = this.httpRequestTimeout;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("WebRpc request {0}:{1}", uri, data);
            }

            this.httpRequestQueue.Enqueue(webRequest, binData, callback, null);

            return true;
        }

        private string MakeRequestUri(WebRpcRequest rpcRequest)
        {
            var urlbase = this.baseUrl;
            var urltail = string.Empty;

            if (this.baseUrl.Contains("?"))
            {
                var split = this.baseUrl.Split('?');
                if (split.Length > 1)
                {
                    urlbase = split[0];
                    urltail = split[1];
                }
            }

            var url = urlbase + "/" + rpcRequest.UriPath + (rpcRequest.UriPath.Contains("?") ? "&" : "?") + urltail;
            url = url.Replace("{AppId}", ((string)this.environment["AppId"]).Trim().Replace(" ", string.Empty));
            url = url.Replace("{AppVersion}", ((string)this.environment["AppVersion"]).Trim().Replace(" ", string.Empty));
            url = url.Replace("{Region}", ((string)this.environment["Region"]).Trim().Replace(" ", string.Empty));
            url = url.Replace("{Cloud}", ((string)this.environment["Cloud"]).Trim().Replace(" ", string.Empty));

            return url;
        }

        private string SerializeRequest(WebRpcRequest request, string userId, object authResultsSecure)
        {
            if (request.RpcParams != null && typeof(Dictionary<string, object>) == request.RpcParams.GetType())
            {
                var rpcParams = (Dictionary<string, object>)request.RpcParams;

                rpcParams["AppId"] = this.environment == null ? null : this.environment["AppId"];
                rpcParams["AppVersion"] = this.environment == null ? null : this.environment["AppVersion"];
                rpcParams["Region"] = this.environment == null ? null : this.environment["Region"];
                rpcParams["UserId"] = userId;
                if (WebFlags.ShouldSendAuthCookie(request.WebFlags))
                {
                    rpcParams["AuthCookie"] = authResultsSecure;
                }

                return Newtonsoft.Json.JsonConvert.SerializeObject(rpcParams);
            }
            else
            {
                var rpcParams = new Dictionary<string, object>();

                rpcParams["AppId"] = this.environment == null ? null : this.environment["AppId"];
                rpcParams["AppVersion"] = this.environment == null ? null : this.environment["AppVersion"];
                rpcParams["Region"] = this.environment == null ? null : this.environment["Region"];
                rpcParams["UserId"] = userId;
                if (WebFlags.ShouldSendAuthCookie(request.WebFlags))
                {
                    rpcParams["AuthCookie"] = authResultsSecure;
                }
                if (request.RpcParams != null)
                {
                    rpcParams["RpcParams"] = request.RpcParams;
                }

                return Newtonsoft.Json.JsonConvert.SerializeObject(rpcParams);
            }
        }

        private void SendErrorResponse(PeerBase peer, byte opCode, SendParameters sp, short retCode = (short)ErrorCode.ExternalHttpCallFailed, string msg = "", Dictionary<byte, object> parameters = null)
        {
            var response = new OperationResponse(opCode)
                               {
                                   DebugMessage = msg,
                                   ReturnCode = retCode,
                                   Parameters = parameters
                               };

            peer.SendOperationResponse(response, sp);
        }

        private void HandleHttpResponse(PeerBase peer, byte operationCode, HttpRequestQueueResultCode result, WebRpcRequest rpcRequest, AsyncHttpRequest request, string requestUri)
        {
            if (result == HttpRequestQueueResultCode.Success)
            {
                this.HandleSuccessHttpResponse(peer, operationCode, rpcRequest, request);
            }
            else
            {
                this.HandleErrorHttpRespone(peer, operationCode, result, request, requestUri);
            }
        }

        private void HandleErrorHttpRespone(PeerBase peer, byte operationCode, HttpRequestQueueResultCode result, AsyncHttpRequest request, string requestUri)
        {
            string msg; 
            if (request != null)
            {
                msg = string.Format(
                "webRpc request to address '{0}' failed. HttpQueueResult:{3}, WebStatus:{1}, Exception :{2}",
                requestUri,
                request.WebStatus,
                request.Exception == null ? "(null)" : request.Exception.ToString(), result);
            }
            else
            {
                msg = string.Format("webRpc request to address '{0}' failed. HttpQueueResult:{1}", requestUri, result);
            }

            this.SendErrorResponse(peer, operationCode, new SendParameters(), msg: msg);

            if (result == HttpRequestQueueResultCode.QueueFull)
            {
                Log.Debug(msg);
            }
            else
            {
                Log.Error(msg);
            }
        }

        private void HandleSuccessHttpResponse(PeerBase peer, byte operationCode, WebRpcRequest rpcRequest, AsyncHttpRequest request)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var responseAsText = Encoding.UTF8.GetString(request.Response);
                if (string.IsNullOrEmpty(responseAsText))
                {
                    var msg = string.Format("WebRpc response for request to address {0} is empty", request.WebRequest.RequestUri);
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat(msg);
                    }

                    this.SendErrorResponse(peer, operationCode, new SendParameters(), msg: msg);
                    return;
                }
                WebRpcResponse rpcResponse;
                try
                {
                    rpcResponse = serializer.Deserialize<WebRpcResponse>(responseAsText);
                }
                catch (Exception ex)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("WebRpc failed to deserialize resonse. Uri={0} response={1} error={2}",
                            request.WebRequest.RequestUri, responseAsText, ex.Message);
                    }

                    var msg = string.Format("WebRpc failed to deserialize resonse. Uri={0} response={1} error={2}",
                        request.WebRequest.RequestUri, responseAsText.Substring(0, Math.Min(200, responseAsText.Length)), ex.Message);

                    this.SendErrorResponse(peer, operationCode, new SendParameters(), msg: msg);
                    return;
                }

                if (rpcResponse == null)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("WebRpc response for request to address {0} has wrong format: {1}",
                            request.WebRequest.RequestUri, responseAsText);
                    }

                    var msg = string.Format("WebRpc response for request to address {0} has wrong format: {1}",
                        request.WebRequest.RequestUri, responseAsText.Substring(0, Math.Min(200, responseAsText.Length)));

                    this.SendErrorResponse(peer, operationCode, new SendParameters(), msg: msg);
                    return;
                }

                var data = rpcResponse.Data as IEnumerable;
                var hasData = data != null && data.GetEnumerator().MoveNext();
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("WebRpc response {0} for request to address {1}: {2}", rpcResponse, request.WebRequest.RequestUri, responseAsText);
                }

                var path = rpcRequest.UriPath.Contains("?") ? rpcRequest.UriPath.Substring(0, rpcRequest.UriPath.IndexOf("?")) : rpcRequest.UriPath;
                var response = new OperationResponse
                {
                    OperationCode = operationCode,
                    ReturnCode = 0,
                    DebugMessage = hasData ? Newtonsoft.Json.JsonConvert.SerializeObject(rpcResponse.Data) : "",
                    Parameters = 
                        hasData 
                        ?
                        new Dictionary<byte, object>
                            {
                                { (byte)ParameterKey.RpcCallRetCode, rpcResponse.ResultCode },
                                { (byte)ParameterKey.RpcCallRetMessage, rpcResponse.Message },
                                { (byte)ParameterKey.UriPath, path },
                                { (byte)ParameterKey.RpcCallParams, rpcResponse.Data },
                            }
                        :
                        new Dictionary<byte, object>
                            {
                                { (byte)ParameterKey.RpcCallRetCode, rpcResponse.ResultCode },
                                { (byte)ParameterKey.RpcCallRetMessage, rpcResponse.Message },
                                { (byte)ParameterKey.UriPath, path },
                            }
                };

                peer.SendOperationResponse(response, new SendParameters());
            }
            catch (Exception e)
            {
                var msg = string.Format("Handling of successfull response failed with exception {0}", e);
                this.SendErrorResponse(peer, operationCode, new SendParameters(), msg: msg);

                Log.Error(msg);
            }
        }
        #endregion//Private methods

        #region Counter methods

        public void HttpQueueRequestsIncrement()
        {
            _Total.IncrementQueueRequests();
            countersInstance.IncrementQueueRequests();
        }

        public void HttpQueueResponsesIncrement()
        {
            _Total.IncrementQueueResponses();
            countersInstance.IncrementQueueResponses();
        }

        public void HttpQueueSuccessIncrement()
        {
            _Total.IncrementQueueSuccesses();
            countersInstance.IncrementQueueSuccesses();
            this.rpcAppCounters.IncrementWebHooskHttpSuccessCount();
        }

        public void HttpQueueTimeoutIncrement()
        {
            _Total.IncrementQueueQueueTimeouts();
            countersInstance.IncrementQueueQueueTimeouts();
        }

        public void HttpQueueErrorsIncrement()
        {
            _Total.IncrementQueueErrors();
            countersInstance.IncrementQueueErrors();
            this.rpcAppCounters.IncrementWebHooskQueueErrorsCount();
        }

        public void HttpQueueOfflineResponsesIncrement()
        {
            _Total.IncrementQueueOfflineResponses();
            countersInstance.IncrementQueueOfflineResponses();
        }

        public void HttpQueueConcurrentRequestsIncrement()
        {
            _Total.IncrementQueueConcurrentRequests();
            countersInstance.IncrementQueueConcurrentRequests();
        }

        public void HttpQueueConcurrentRequestsDecrement()
        {
            _Total.DecrementQueueConcurrentRequests();
            countersInstance.DecrementQueueConcurrentRequests();
        }

        public void HttpQueueQueuedRequestsIncrement()
        {
            _Total.IncrementQueueQueuedRequests();
            countersInstance.IncrementQueueQueuedRequests();
        }

        public void HttpQueueQueuedRequestsDecrement()
        {
            _Total.DecrementQueueQueuedRequests();
            countersInstance.DecrementQueueQueuedRequests();
        }

        public void HttpRequestExecuteTimeIncrement(long ticks)
        {
            _Total.IncrementHttpRequestExecutionTime(ticks);
            countersInstance.IncrementHttpRequestExecutionTime(ticks);
            this.rpcAppCounters.IncrementWebHooksHttpRequestExecTime((uint) (ticks/Stopwatch.Frequency));
        }

        public void HttpQueueOnlineQueueCounterIncrement()
        {
            _Total.IncrementQueueOnlineQueue();
            countersInstance.IncrementQueueOnlineQueue();
        }

        public void HttpQueueOnlineQueueCounterDecrement()
        {
            _Total.DecrementQueueOnlineQueue();
            countersInstance.DecrementQueueOnlineQueue();
        }

        public void HttpQueueBackedoffRequestsIncrement()
        {
            _Total.IncrementBackedOffRequests();
            countersInstance.IncrementBackedOffRequests();
        }

        public void HttpQueueBackedoffRequestsDecrement()
        {
            _Total.DecrementBackedOffRequests();
            countersInstance.DecrementBackedOffRequests();
        }

        public void HttpRequestIncrement()
        {
            _Total.IncrementHttpRequests();
            countersInstance.IncrementHttpRequests();
        }

        public void HttpSuccessIncrement()
        {
            _Total.IncrementHttpSuccesses();
            countersInstance.IncrementHttpSuccesses();
            this.rpcAppCounters.IncrementWebHooskHttpSuccessCount();
        }

        public void HttpTimeoutIncrement()
        {
            _Total.IncrementHttpRequestTimeouts();
            countersInstance.IncrementHttpRequestTimeouts();
            this.rpcAppCounters.IncrementWebHooskHttpTimeoutCount();
        }

        public void HttpErrorsIncrement()
        {
            _Total.IncrementHttpErrors();
            countersInstance.IncrementHttpErrors();
            this.rpcAppCounters.IncrementWebHooskHttpErrorsCount();
        }

        private class NullAppCounters : IRpcHandlerAppCounters
        {
            public void IncrementWebHooskHttpSuccessCount()
            {
            }

            public void IncrementWebHooskHttpErrorsCount()
            {
            }

            public void IncrementWebHooskHttpTimeoutCount()
            {
            }

            public void IncrementWebHooskQueueSuccessCount()
            {
            }

            public void IncrementWebHooskQueueErrorsCount()
            {
            }

            public void IncrementWebHooksHttpRequestExecTime(uint millisecods)
            {
            }
        }

        #endregion
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiveHostGame.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;
using Photon.Common;
using Photon.Hive.Common;

namespace Photon.Hive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.Hive.Caching;
    using Photon.Hive.Diagnostics.OperationLogging;
    using Photon.Hive.Events;
    using Photon.Hive.Operations;
    using Photon.Hive.Plugin;
    using Photon.SocketServer;
    using Photon.SocketServer.Net;
    using Photon.SocketServer.Rpc.Protocols;
    using Photon.SocketServer.Diagnostics;
    using SendParameters = Photon.SocketServer.SendParameters;


    public enum HiveHostActorState
    {
        ActorNr,
        Binary,
        UserId,
        Nickname,
    }


    public class HiveHostGame : HiveGame, IPluginHost, IHttpRequestQueueCounters, IUnknownTypeMapper
    {
        #region Constants and Fields
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly ILogger LogPlugin = LogManager.GetLogger("Photon.Hive.HiveGame.HiveHostGame.Plugin");

        private readonly AutoResetEvent onHttpResonseEvent = new AutoResetEvent(false);

        protected CallCounter PendingPluginContinue = new CallCounter();

        private readonly CustomTypeCache customTypeCache = new CustomTypeCache();

        private readonly HttpRequestQueue httpRequestQueue = new HttpRequestQueue();

        protected static readonly IHttpQueueCountersInstance _Total = HttpQueuePerformanceCounters.GetInstance("_Total");
        protected static readonly IHttpQueueCountersInstance CountersInstance =
            HttpQueuePerformanceCounters.GetInstance(ApplicationBase.Instance.PhotonInstanceName + "_game");

        private readonly int httpQueueRequestTimeout;

        protected CallEnv callEnv;

        private bool allowSetGameState = true;
        private bool failureOnCreate;
        private bool isClosed;
        private readonly IPluginManager pluginManager;
        private readonly TimeIntervalCounter httpForwardedRequests = new TimeIntervalCounter(new TimeSpan(0, 0, 1));

        #endregion

        #region Constructors and Destructors

        private static string GetHwId()
        {
            if (!string.IsNullOrEmpty(ApplicationBase.Instance.HwId)) return ApplicationBase.Instance.HwId;

            string password = System.Environment.MachineName + System.Environment.OSVersion + System.Environment.UserName;
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }

      public HiveHostGame(
            string gameName,
            RoomCacheBase roomCache,
            IGameStateFactory gameStateFactory = null,
            int maxEmptyRoomTTL = 0,
            IPluginManager pluginManager = null,
            string pluginName = "",
            Dictionary<string, object> environment = null,
            int lastTouchLimitMilliseconds = 0,
            int lastTouchCheckIntervalMilliseconds = 0,
            HttpRequestQueueOptions httpRequestQueueOptions = null,
            ExtendedPoolFiber executionFiber = null
            )
            : base(gameName, roomCache, gameStateFactory, maxEmptyRoomTTL, lastTouchLimitMilliseconds, lastTouchCheckIntervalMilliseconds, executionFiber)
        {
            this.pluginManager = pluginManager;

            if (httpRequestQueueOptions == null)
            {
                httpRequestQueueOptions = new HttpRequestQueueOptions();
            }

            this.httpRequestQueue.MaxErrorRequests = httpRequestQueueOptions.HttpQueueMaxTimeouts;
            this.httpRequestQueue.MaxTimedOutRequests = httpRequestQueueOptions.HttpQueueMaxErrors;
            this.httpRequestQueue.ReconnectInterval = TimeSpan.FromMilliseconds(httpRequestQueueOptions.HttpQueueReconnectInterval);
            this.httpRequestQueue.QueueTimeout = TimeSpan.FromMilliseconds(httpRequestQueueOptions.HttpQueueQueueTimeout);
            this.httpRequestQueue.MaxQueuedRequests = httpRequestQueueOptions.HttpQueueMaxQueuedRequests;
            this.httpRequestQueue.MaxBackoffInMilliseconds = httpRequestQueueOptions.HttpQueueMaxBackoffTime;
            this.httpRequestQueue.MaxConcurrentRequests = httpRequestQueueOptions.HttpQueueMaxConcurrentRequests;

            this.httpQueueRequestTimeout = httpRequestQueueOptions.HttpQueueRequestTimeout;

            this.httpRequestQueue.SetCounters(this);

            this.Environment = environment ?? new Dictionary<string, object>
                                       {
                                           {"AppId", GetHwId()},
                                           {"AppVersion", ""},
                                           {"Region", ""},
                                           {"Cloud", ""},
                                       };
            this.InitPlugin(pluginName);
            if (this.Plugin == null)
            {
                throw new Exception(string.Format("Failed to craete plugin '{0}'", pluginName));
            }

            var errorPlugin = this.Plugin as ErrorPlugin;
            if (errorPlugin != null)
            {
                Log.ErrorFormat("Game {0} is created with ErrorPlugin. message:{1}", this.Name, errorPlugin.Message);
            }

            this.customTypeCache.TypeMapper = this;
            this.callEnv = new CallEnv(this.Plugin, this.Name);
        }

        #endregion

        #region Properties

        public IGamePlugin Plugin
        {
            get
            {
                return this.PluginInstance.Plugin;
            }
        }
        public IPluginInstance PluginInstance { get; private set; }

        public Dictionary<string, object> Environment { get; set; }

        public int HttpForwardedOperationsLimit { get; protected set; }

        IList<IActor> IPluginHost.GameActors
        {
            get
            {
                return this.ActorsManager.AllActors.Select(a => (IActor)a).ToList();
            }
        }

        IList<IActor> IPluginHost.GameActorsActive
        {
            get
            {
                return this.ActorsManager.Actors.Select(a => (IActor)a).ToList();
            }
        }

        IList<IActor> IPluginHost.GameActorsInactive
        {
            get
            {
                return this.ActorsManager.InactiveActors.Select(a => (IActor)a).ToList();
            }
        }

        string IPluginHost.GameId
        {
            get
            {
                return this.Name;
            }
        }

        Hashtable IPluginHost.GameProperties
        {
            get
            {
                return this.Properties.GetProperties();
            }
        }

        Dictionary<string, object> IPluginHost.CustomGameProperties
        {
            get
            {
                if (this.LobbyProperties != null)
                {
                    var customProperties =
                        this.Properties.AsDictionary()
                            .Where(prop => this.LobbyProperties.Contains(prop.Key))
                            .ToDictionary(prop => (string)prop.Key, prop => prop.Value.Value);
                    return customProperties;
                }
                return new Dictionary<string, object>();
            }
        }

        public override bool IsPersistent
        {
            get
            {
                var isPersistent = false;
                try
                {
                    isPersistent = this.Plugin.IsPersistent;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return isPersistent;
            }
        }

        #endregion

        #region Indexers

        public object this[object key]
        {
            get
            {
                return this.Properties.GetProperty(key).Value;
            }

            set
            {
                this.Properties.Set(key, value);
            }
        }

        #endregion

        #region Public Methods

        public override bool BeforeRemoveFromCache()
        {
            this.RemoveRoomPath = RemoveState.BeforeRemoveFromCacheCalled;
            // we call the plugin and give it a chance to change the TTL - not sure it is a good idea ... but anyway.
            // we return fals to supress getting evicted from the cache.
            // and we enqueue the plugin call because we are holding a cache lock !!
            this.ExecutionFiber.Enqueue(
                () =>
                {
                    this.RemoveRoomPath = RemoveState.BeforeRemoveFromCacheActionCalled;
                    var request = new CloseRequest { EmptyRoomTTL = this.EmptyRoomLiveTime };
                    RequestHandler handler = () =>
                    {
                        try
                        {
                            return this.ProcessBeforeCloseGame(request);
                        }
                        catch (Exception e)
                        {
                            // here we can not rethrow because we are in fiber action
                            this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
                            this.TriggerPluginOnClose();
                            Log.Error(e);
                        }
                        return false;
                    };

                    var info = new BeforeCloseGameCallInfo(this.PendingPluginContinue, this.callEnv)
                                   {
                                       Request = request,
                                       Handler = handler,
                                       SendParams = new SendParameters(),
                                       FailedOnCreate = this.failureOnCreate,
                                   };
                    try
                    {
                        this.Plugin.BeforeCloseGame(info);
                    }
                    catch (Exception e)
                    {
                        this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
                    }
                });

            return false;
        }

        public override void OnClose()
        {
            // TBD
            Log.Debug("we are actually beeing released - not sure this should be called.");
            base.OnClose();
        }

        #endregion

        #region Implemented Interfaces

        #region IPluginHost

        void IPluginHost.BroadcastEvent(IList<int> receiverActors, int senderActor, byte evCode, Dictionary<byte, object> data, byte cacheOp,
            Photon.Hive.Plugin.SendParameters sendParameters)
        {
            var targets = receiverActors == null ? this.Actors : this.ActorsManager.ActorsGetActorsByNumbers(receiverActors.ToArray());

            this.BroadcastEventInternal(evCode, data, cacheOp, true, senderActor, targets, sendParameters);
        }

        void IPluginHost.BroadcastEvent(byte target, int senderActor, byte targetGroup, byte evCode, Dictionary<byte, object> data, byte cacheOp,
            Photon.Hive.Plugin.SendParameters sendParameters)
        {
            IEnumerable<Actor> actors;
            var updateEventCache = false;
            switch (target)
            {
                case ReciverGroup.All:
                    actors = this.Actors;
                    updateEventCache = true;
                    break;
                case ReciverGroup.Others:
                    actors = this.ActorsManager.ActorsGetExcludedList(senderActor);
                    updateEventCache = true;
                    break;
                case ReciverGroup.Group:
                    actors = this.GroupManager.GetActorGroup(targetGroup);
                    break;
                default:
                    Log.ErrorFormat("Unknown target {0} specified in BroadcastEvent", target);
                    throw new ArgumentException(string.Format("Unknown target {0} specified in BroadcastEvent", target));
            }

            this.BroadcastEventInternal(evCode, data, cacheOp, updateEventCache, senderActor, actors, sendParameters);
        }

        private void BroadcastEventInternal(byte evCode, Dictionary<byte, object> data,
            byte cacheOp, bool updateEventCache, int senderActor, IEnumerable<Actor> actors, Plugin.SendParameters sendParameters)
        {
            if (updateEventCache && cacheOp != (byte)CacheOperation.DoNotCache)
            {
                string msg;
                if (cacheOp != (byte)CacheOperation.AddToRoomCache
                    && cacheOp != (byte)CacheOperation.AddToRoomCacheGlobal)
                {
                    msg = string.Format("Unsupported value {0} for cacheOp", cacheOp);
                    Log.Error(msg);
                    throw new ArgumentException(msg);
                }

                if (cacheOp != (byte)CacheOperation.AddToRoomCacheGlobal && senderActor <= 0)
                {
                    msg = string.Format("Cache operation={0} requires existing sender number", cacheOp);
                    Log.Error(msg);
                    throw new ArgumentException(msg);
                }

                Actor actor = null;
                if (senderActor > 0)
                {
                    actor = this.ActorsManager.ActorsGetActorByNumber(senderActor);
                    if (actor == null)
                    {
                        msg = string.Format("Invalid senderActor={0} specified. Number may be 0 or existing", senderActor);
                        Log.Error(msg);
                        throw new ArgumentException(msg);
                    }
                }

                if (!this.UpdateEventCache(actor, evCode, data, cacheOp, out msg))
                {
                    Log.Error(msg);
                }
            }

            var ed = new EventData(evCode, data);
            this.PublishEvent(ed, actors, MakeSendParams(sendParameters));
        }

        private static SendParameters MakeSendParams(Plugin.SendParameters sendParameters)
        {
            return new SendParameters
            {
                ChannelId = sendParameters.ChannelId,
                Encrypted = sendParameters.Encrypted,
                Flush = sendParameters.Flush,
                Unreliable = sendParameters.Unreliable
            };
        }
        void IPluginHost.BroadcastErrorInfoEvent(string message, Plugin.SendParameters sendParameters)
        {
            this.PublishEvent(new ErrorInfoEvent(message), this.Actors, MakeSendParams(sendParameters));
        }

        void IPluginHost.BroadcastErrorInfoEvent(string message, ICallInfo info, Plugin.SendParameters sendParameters)
        {
            this.PublishEvent(new ErrorInfoEvent(message), this.Actors, MakeSendParams(sendParameters));
        }

        private Action GetTimerAction(Action callback)
        {
            return () =>
            {
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    this.Plugin.ReportError(ErrorCodes.UnhandledException, e);
                }
            };
        }

        object IPluginHost.CreateOneTimeTimer(Action callback, int dueTimeMs)
        {
            var action = this.GetTimerAction(callback);
            return this.ExecutionFiber.Schedule(action, dueTimeMs);
        }

        object IPluginHost.CreateTimer(Action callback, int dueTimeMs, int intervalMs)
        {
            return this.ExecutionFiber.ScheduleOnInterval(this.GetTimerAction(callback), dueTimeMs, intervalMs);
        }

        bool IPluginHost.RemoveActor(int actorNr, string reasonDetail)
        {
            return ((IPluginHost)this).RemoveActor(actorNr, 0, reasonDetail);
        }

        bool IPluginHost.RemoveActor(int actorNr, byte reason, string reasonDetail)
        {
            var actor = this.RemoveActor(actorNr, reason, reasonDetail);
            if (actor == null)
            {
                return false;
            }

            switch (reason)
            {
                case RemoveActorReason.Banned:
                    this.ApplyBanning(actor);
                    break;
                default:
                    break;
            }
            return true;
        }

        [ObsoleteAttribute("This method is obsolete. Call GetGameState() instead.", false)]
        public Dictionary<byte, byte[]> GetGameStateAsByteArray()
        {
            return null;
        }

        private class ResponseDataCarrier
        {
            public HttpRequestQueueResultCode Result { get; set; }
            public AsyncHttpRequest HttpRequest { get; set; }
            public object State { get; set; }
        }

        void IPluginHost.HttpRequest(HttpRequest request)
        {
            if (request.Callback == null)
            {
                var url = request.Url;
                Log.Debug("HttpRequest Callback is not set. Using default to log in case of error. " + url);

                request.Callback = (response, state) =>
                    {
                        if (response.Status != HttpRequestQueueResult.Success)
                        {
                            Log.Warn(
                                string.Format(
                                    "Request to '{0}' failed. reason={1}, httpcode={2} webstatus={3}, HttpQueueResult={4}.",
                                    url,
                                    response.Reason,
                                    response.HttpCode,
                                    response.WebStatus,
                                    response.Status));
                        }
                    };
            }

            var stateCarrier = new ResponseDataCarrier();
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                webRequest.Proxy = null;
                webRequest.Method = request.Method ?? "GET";
                webRequest.ContentType = request.ContentType;
                webRequest.Accept = request.Accept;
                webRequest.Timeout = this.httpQueueRequestTimeout;

                if (request.CustomHeaders != null)
                {
                    foreach (var kv in request.CustomHeaders)
                    {
                        webRequest.Headers.Add(kv.Key, kv.Value);
                    }
                }

                if (request.Headers != null)
                {
                    foreach (var kv in request.Headers)
                    {
                        webRequest.Headers.Add(kv.Key, kv.Value);
                    }
                }

                HttpRequestQueueCallback callback = (result, httpRequest, state) =>
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("callback for request is called.url:{0}.IsAsync:{1}",
                                httpRequest.WebRequest.RequestUri, request.Async);
                        }

                        if (request.Async)
                        {
                            this.ExecutionFiber.Enqueue(() => this.HttpRequestHttpCallback(request, result, httpRequest, state));
                        }
                        else
                        {
                            stateCarrier.HttpRequest = httpRequest;
                            stateCarrier.Result = result;
                            stateCarrier.State = state;

                            this.onHttpResonseEvent.Set();
                        }
                    };

                if (webRequest.Method == "POST")
                {
                    this.httpRequestQueue.Enqueue(webRequest, request.DataStream.ToArray(), callback, request.UserState, request.Async ? 3 : 0);
                }
                else
                {
                    this.httpRequestQueue.Enqueue(webRequest, callback, request.UserState, request.Async ? 3 : 0);
                }
            }
            catch (WebException e)
            {
                Log.Error(string.Format("Exception calling Url:{0}", request.Url), e);
                var response = new HttpResponseImpl(request, null, string.Empty, HttpRequestQueueResultCode.Error, 0, e.Message, (int)e.Status);
                request.Callback(response, request.UserState);
                return;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Exception calling Url:{0}", request.Url), ex);
                var response = new HttpResponseImpl(
                    request,
                    null,
                    string.Empty,
                    HttpRequestQueueResultCode.Error,
                    0,
                    ex.Message,
                    (int)WebExceptionStatus.UnknownError);
                request.Callback(response, request.UserState);
                return;
            }

            if (request.Async)
            {
                Log.Debug("HttpRequest() - NOT Waiting for HttpResponse.");
                // we return immediately without waiting for response
                return;
            }

            var timeout = this.httpQueueRequestTimeout + (int) this.httpRequestQueue.QueueTimeout.TotalMilliseconds + 1000;
            // waiting for our callback to release us
            Log.Debug("HttpRequest() - Waiting for HttpResponse.");
            if (!this.onHttpResonseEvent.WaitOne(timeout))
            {
                Log.WarnFormat("Plugin's sync http call timedout. url:{0}, Method:{1}, timeout:{2}", request.Url, request.Method, timeout);
                return;
            }
            Log.Debug("HttpRequest() - Done.");

            this.HttpRequestHttpCallback(request, stateCarrier.Result, stateCarrier.HttpRequest, stateCarrier.State);
        }

        private void HttpRequestHttpCallback(HttpRequest request, HttpRequestQueueResultCode result, AsyncHttpRequest httpRequest, object state)
        {
            var statusCode = -1;
            string statusDescription;
            byte[] responseData = null;

            try
            {
                switch (result)
                {
                    case HttpRequestQueueResultCode.Success:
                        statusCode = (int)httpRequest.WebResponse.StatusCode;
                        statusDescription = httpRequest.WebResponse.StatusDescription;
                        responseData = httpRequest.Response;
                        break;

                    case HttpRequestQueueResultCode.Error:
                        if (httpRequest.WebResponse != null)
                        {
                            statusCode = (int)httpRequest.WebResponse.StatusCode;
                            statusDescription = httpRequest.WebResponse.StatusDescription;
                        }
                        else
                        {
                            statusDescription = httpRequest.Exception.Message;
                        }

                        break;
                    default:
                        statusCode = -1;
                        statusDescription = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                // we should never get her
                statusDescription = ex.Message;
            }

            var response = new HttpResponseImpl(
                request,
                responseData,
                responseData == null ? null : Encoding.UTF8.GetString(responseData, 0, responseData.Length),
                result,
                statusCode,
                statusDescription,
                httpRequest == null ? -1 : (int)httpRequest.WebStatus);

            Log.Debug("Got HttpResoonse - executing callback.");


            // Sync request triggers an event to release the waiting Request thread to continue
            try
            {
                request.Callback(response, state);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (request.Async)
                {
                    this.Plugin.ReportError(ErrorCodes.AsyncCallbackException, ex, state);
                }
                else
                {
                    throw;
                }
            }
        }

        void IPluginHost.LogDebug(object message)
        {
            try
            {
                LogPlugin.Debug(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        void IPluginHost.LogError(object message)
        {
            try
            {
                LogPlugin.Error(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        void IPluginHost.LogFatal(object message)
        {
            try
            {
                LogPlugin.Fatal(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        void IPluginHost.LogInfo(object message)
        {
            try
            {
                LogPlugin.Info(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        void IPluginHost.LogWarning(object message)
        {
            try
            {
                LogPlugin.Warn(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public bool SetGameState(Dictionary<byte, byte[]> state)
        {
            return false;
        }

        bool IPluginHost.SetProperties(int actorNr, Hashtable properties, Hashtable expected, bool broadcast)
        {

            var request = new SetPropertiesRequest(actorNr, properties, expected, broadcast);

            string errorMsg;
            if (!this.ValidateAndFillSetPropertiesRequest(null, request, out errorMsg))
            {
                throw new Exception(errorMsg);
            }

            var propertiesUpdateResult = this.SetNewPropertyValues(request, out errorMsg);

            this.PublishResultsAndSetGameProperties(propertiesUpdateResult, errorMsg, request, null, new SendParameters());
            return true;
        }

        void IPluginHost.StopTimer(object timer)
        {
            var disposable = timer as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        bool IPluginHost.TryRegisterType(Type type, byte typeCode, Func<object, byte[]> serializeFunction, Func<byte[], object> deserializeFunction)
        {
            return this.customTypeCache.TryRegisterType(type, typeCode, serializeFunction, deserializeFunction);
        }

        bool IPluginHost.SetGameState(SerializableGameState state)
        {
            if (!this.allowSetGameState)
            {
                Log.ErrorFormat("Plugin {0} tries to set game state after call to 'Continue'. Game '{1}', stack\n{2}",
                    this.Plugin.Name, this.Name, GetCallStack());
                return false;
            }
            return this.RoomState.SetState(state);
        }

        EnvironmentVersion IPluginHost.GetEnvironmentVersion()
        {
            return this.PluginInstance.Version;
        }
        #endregion

        #region IHttpRequestQueueCounters

        public void HttpQueueRequestsIncrement()
        {
            _Total.IncrementQueueRequests();
            CountersInstance.IncrementQueueRequests();
        }

        public void HttpQueueResponsesIncrement()
        {
            _Total.IncrementQueueResponses();
            CountersInstance.IncrementQueueResponses();
        }

        public void HttpQueueSuccessIncrement()
        {
            _Total.IncrementQueueSuccesses();
            CountersInstance.IncrementQueueSuccesses();
            this.gameAppCounters.WebHooksQueueSuccessIncrement();
        }

        public void HttpQueueTimeoutIncrement()
        {
            _Total.IncrementQueueQueueTimeouts();
            CountersInstance.IncrementQueueQueueTimeouts();
        }

        public void HttpQueueErrorsIncrement()
        {
            _Total.IncrementQueueErrors();
            CountersInstance.IncrementQueueErrors();
            this.gameAppCounters.WebHooksQueueErrorIncrement();
        }

        public void HttpQueueOfflineResponsesIncrement()
        {
            _Total.IncrementQueueOfflineResponses();
            CountersInstance.IncrementQueueOfflineResponses();
        }

        public void HttpQueueConcurrentRequestsIncrement()
        {
            _Total.IncrementQueueConcurrentRequests();
            CountersInstance.IncrementQueueConcurrentRequests();
        }

        public void HttpQueueConcurrentRequestsDecrement()
        {
            _Total.DecrementQueueConcurrentRequests();
            CountersInstance.DecrementQueueConcurrentRequests();
        }

        public void HttpQueueQueuedRequestsIncrement()
        {
            _Total.IncrementQueueQueuedRequests();
            CountersInstance.IncrementQueueQueuedRequests();
        }

        public void HttpQueueQueuedRequestsDecrement()
        {
            _Total.DecrementQueueQueuedRequests();
            CountersInstance.DecrementQueueQueuedRequests();
        }

        public void HttpRequestExecuteTimeIncrement(long ticks)
        {
            _Total.IncrementHttpRequestExecutionTime(ticks);
            CountersInstance.IncrementHttpRequestExecutionTime(ticks);
        }

        public void HttpQueueOnlineQueueCounterIncrement()
        {
            _Total.IncrementQueueOnlineQueue();
            CountersInstance.IncrementQueueOnlineQueue();
        }

        public void HttpQueueOnlineQueueCounterDecrement()
        {
            _Total.DecrementQueueOnlineQueue();
            CountersInstance.DecrementQueueOnlineQueue();
        }

        public void HttpQueueBackedoffRequestsIncrement()
        {
            _Total.IncrementBackedOffRequests();
            CountersInstance.IncrementBackedOffRequests();
        }

        public void HttpQueueBackedoffRequestsDecrement()
        {
            _Total.DecrementBackedOffRequests();
            CountersInstance.DecrementBackedOffRequests();
        }

        public void HttpRequestIncrement()
        {
            _Total.IncrementHttpRequests();
            CountersInstance.IncrementHttpRequests();
        }

        public void HttpSuccessIncrement()
        {
            _Total.IncrementHttpSuccesses();
            CountersInstance.IncrementHttpSuccesses();
            this.gameAppCounters.WebHooksHttpSuccessIncrement();
        }

        public void HttpTimeoutIncrement()
        {
            _Total.IncrementHttpRequestTimeouts();
            CountersInstance.IncrementHttpRequestTimeouts();
            this.gameAppCounters.WebHooksHttpTimeoutIncrement();
        }

        public void HttpErrorsIncrement()
        {
            _Total.IncrementHttpErrors();
            CountersInstance.IncrementHttpErrors();
            this.gameAppCounters.WebHooksHttpErrorIncrement();
        }

        public void HttpResponseIncrement()
        {
            _Total.IncrementHttpResponses();
            CountersInstance.IncrementHttpResponses();
        }
        #endregion

        #region IUnknownTypeMapper

        public bool OnUnknownType(Type type, ref object obj)
        {
            try
            {
                return this.Plugin.OnUnknownType(type, ref obj);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(ErrorCodes.UnhandledException, e);
                return false;
            }
        }

        #endregion

        #endregion

        #region Methods

        private void ApplyBanning(Actor actor)
        {
            this.ActorsManager.AddToExcludeList(actor.UserId, RemoveActorReason.Banned);
            this.OnActorBanned(actor);
        }

        protected virtual void OnActorBanned(Actor actor)
        { }

        private Actor RemoveActor(int actorNr, int reason, string reasonDetail)
        {
            var actor = this.ActorsManager.ActorsGetActorByNumber(actorNr);
            if (actor != null)
            {
                actor.Peer.RemovePeerFromCurrentRoom(LeaveReason.PluginRequest, reasonDetail); //kicking player
                actor.Peer.Disconnect();
            }
            else
            {
                actor = this.ActorsManager.InactiveActorsGetActorByNumber(actorNr);
                if (actor != null)
                {
                    base.RemoveInactiveActor(actor);
                }
            }
            return actor;
        }

        public override void RemoveInactiveActor(Actor actor)
        {
            RequestHandler handler = () =>
            {
                try
                {
                    return this.ProcessRemoveInactiveActor(actor);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Exception: {0}", e);
                    throw;
                }
            };

            var info = new LeaveGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                ActorNr = actor.ActorNr,
                UserId = actor.UserId,
                Nickname = actor.Nickname,
                IsInactive = false,
                Reason = LeaveReason.PlayerTtlTimedOut,
                Request = null,
                Handler = handler,
                Peer = null,
                SendParams = new SendParameters(),
            };
            try
            {
                this.Plugin.OnLeave(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        protected override void HandleCreateGameOperation(HivePeer peer, SendParameters sendParameters, CreateGameRequest createGameRequest)
        {
            if (this.isClosed)
            {
                if (!this.ReinitGame())
                {
                    this.SendErrorResponse(peer, createGameRequest.OperationCode,
                        ErrorCode.InternalServerError, HiveErrorMessages.ReinitGameFailed, sendParameters);

                    createGameRequest.OnJoinFailed(ErrorCode.InternalServerError, HiveErrorMessages.ReinitGameFailed);

                    this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, createGameRequest);

                    if (Log.IsWarnEnabled)
                    {
                        Log.WarnFormat("Game '{0}' userId '{1}' failed to create game. msg:{2} -- peer:{3}", this.Name, peer.UserId, 
                            HiveErrorMessages.ReinitGameFailed, peer);
                    }

                    return;
                }
            }

            string msg;
            if (!this.ValidatePlugin(createGameRequest, out msg))
            {
                this.SendErrorResponse(peer, createGameRequest.OperationCode, ErrorCode.PluginMismatch, msg, sendParameters);
                createGameRequest.OnJoinFailed(ErrorCode.PluginMismatch, msg);
                this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, createGameRequest);

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Game '{0}' userId '{1}' failed to create game. msg:{2} -- peer:{3}", this.Name, peer.UserId, msg, peer);
                }
                return;
            }

            this.HandleCreateGameOperationInt(peer, sendParameters, createGameRequest);
        }

        private void HandleCreateGameOperationInt(HivePeer peer, SendParameters sendParameters,
            JoinGameRequest createGameRequest, bool fromJoin = false)
        {
            var createOptions = createGameRequest.GetCreateGameSettings(this);

            peer.SetPrivateCustomTypeCache(this.customTypeCache);

            RequestHandler handler = () =>
            {
                var oldValue = this.allowSetGameState;

                try
                {
                    this.allowSetGameState = false;

                    // since we allow to make changes to the original request sent by the client in a plugin
                    // we should check op.IsValid() - if not report error
                    createGameRequest.SetupRequest();

                    return this.ProcessCreateGame(peer, createGameRequest, sendParameters);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Exception: {0}", e);
                    this.allowSetGameState = oldValue;
                    throw;
                }
            };

            var info = new CreateGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                Request = createGameRequest,
                UserId = peer.UserId,
                Nickname = createGameRequest.GetNickname(),
                CreateOptions = createOptions,
                IsJoin = fromJoin,
                Handler = handler,
                Peer = peer,
                SendParams = sendParameters,
                OnFail = (onFailMsg, errorData) =>
                {
                    this.allowSetGameState = false;
                    this.failureOnCreate = true;

                    this.SendErrorResponse(peer, createGameRequest.OperationCode,
                        ErrorCode.PluginReportedError, onFailMsg, sendParameters, errorData);

                    peer.ReleaseRoomReference();
                    peer.ScheduleDisconnect(); // this gives the client a chance to get the reason
                }
            };

            try
            {
                this.Plugin.OnCreateGame(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        private bool ValidatePlugin(JoinGameRequest operation, out string msg)
        {
            if (operation.Plugins != null)
            {
                if (operation.Plugins.Length > 0)
                {
                    if (operation.Plugins.Length > 1)
                    {
                        msg = "Currently only one plugin per game supported.";
                        return false;
                    }

                    if (this.Plugin.Name != operation.Plugins[0])
                    {
                        var errorPlugin = this.Plugin as ErrorPlugin;
                        if (errorPlugin != null)
                        {
                            msg = string.Format("Plugin Mismatch requested='{0}' got ErrorPlugin with message:'{1}'", 
                                operation.Plugins[0], errorPlugin.Message);
                        }
                        else
                        {
                            msg = string.Format("Plugin Mismatch requested='{0}' got='{1}'", operation.Plugins[0], this.Plugin.Name);
                        }
                        return false;
                    }
                }
                else
                {
                    if (this.Plugin.Name != "Default")
                    {
                        msg = string.Format("Room is setup with unexpected plugin '{0}' - instead of default (none).", this.Plugin.Name);
                        return false;
                    }
                }
            }
            msg = string.Empty;
            return true;
        }

        protected override void HandleJoinGameOperation(HivePeer peer, SendParameters sendParameters, JoinGameRequest joinGameRequest)
        {
            if (this.isClosed)
            {
                if (!this.CheckGameCanBeCreated(peer, joinGameRequest))
                {
                    return;
                }

                if (!this.ReinitGame())
                {
                    this.SendErrorResponse(peer, joinGameRequest.OperationCode,
                        ErrorCode.InternalServerError, HiveErrorMessages.ReinitGameFailed, sendParameters);

                    joinGameRequest.OnJoinFailed(ErrorCode.InternalServerError, HiveErrorMessages.ReinitGameFailed);

                    this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, joinGameRequest);
                    if (Log.IsWarnEnabled)
                    {
                        Log.WarnFormat("Game '{0}' userId '{1}' failed to join. msg:{2}", this.Name, peer.UserId,
                            HiveErrorMessages.ReinitGameFailed);
                    }
                    return;
                }
            }

            //TBD do we still need this?
            string msg;
            if (!this.ValidatePlugin(joinGameRequest, out msg))
            {
                this.SendErrorResponse(peer, joinGameRequest.OperationCode, ErrorCode.PluginMismatch, msg, sendParameters);

                joinGameRequest.OnJoinFailed(ErrorCode.InternalServerError, HiveErrorMessages.ReinitGameFailed);
                this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, joinGameRequest);

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("HandleJoinGameOperation: Game '{0}' userId '{1}' failed to join. msg:{2} -- peer:{3}", this.Name, peer.UserId, msg, peer);
                }
                return;
            }

            if (this.ActorsManager.ActorNumberCounter == 0) // we were just beeing created
            {
                if (!this.CheckGameCanBeCreated(peer, joinGameRequest))
                {
                    return;
                }

                this.HandleCreateGameOperationInt(peer, sendParameters, joinGameRequest, true);
            }
            else
            {
                peer.SetPrivateCustomTypeCache(this.customTypeCache);

                RequestHandler handler = () =>
                {
                    try
                    {
                        return this.ProcessBeforeJoinGame(joinGameRequest, sendParameters, peer);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Exception: {0}", e);
                        throw;
                    }
                };
                var info = new BeforeJoinGameCallInfo(this.PendingPluginContinue, this.callEnv)
                {
                    Request = joinGameRequest,
                    UserId = peer.UserId,
                    Nickname = joinGameRequest.GetNickname(),
                    Handler = handler,
                    Peer = peer,
                    SendParams = sendParameters,
                    OnFail = (onFailMsg, errorData) =>
                    {
                        this.allowSetGameState = false;
                        joinGameRequest.OnJoinFailed(ErrorCode.PluginReportedError, onFailMsg);
                        this.OnJoinFailHandler(LeaveReason.ServerDisconnect, onFailMsg, errorData, peer, sendParameters, joinGameRequest);
                    }
                };

                try
                {

                    this.Plugin.BeforeJoin(info);
                }
                catch (Exception e)
                {
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
                }
            }
        }

        private bool CheckGameCanBeCreated(HivePeer peer, JoinGameRequest joinGameRequest)
        {
            if (joinGameRequest.OperationCode == (byte)OperationCode.Join 
                || joinGameRequest.JoinMode == JoinModes.CreateIfNotExists
                || joinGameRequest.JoinMode == JoinModes.RejoinOrJoin // for backwards compatibility - it seams some games expect this behavior - ISSUE: Codemasters uses RejoinOrJoin and now expects this to return false!
                || this.Plugin.IsPersistent)
            {
                return true;
            }

            this.SendErrorResponse(peer, joinGameRequest.OperationCode,
                ErrorCode.GameIdNotExists, HiveErrorMessages.GameIdDoesNotExist, new SendParameters());

            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat(
                    "CheckGameCanBeCreated: Game '{0}' userId '{1}' failed to join game. msg:'{2}' (JoinMode={3}) -- peer:{4}",
                    this.Name,
                    peer.UserId,
                    HiveErrorMessages.GameIdDoesNotExist,
                    joinGameRequest.JoinMode,
                    peer);
            }

            joinGameRequest.OnJoinFailed(ErrorCode.GameIdNotExists, HiveErrorMessages.GameIdDoesNotExist);

            this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, joinGameRequest);
            return false;
        }

        private void InitPlugin(string pluginName)
        {
            if (this.pluginManager == null)
            {
                this.PluginInstance = new PluginInstance() { Plugin = new PluginBase(), Version = new EnvironmentVersion() };

                string errorMsg;
                this.Plugin.SetupInstance(this, null, out errorMsg);
            }
            else
            {
                this.PluginInstance = this.pluginManager.GetGamePlugin(this, pluginName);
            }
        }

        protected bool ReinitGame()
        {
            if (this.Plugin == null)
            {
                Log.ErrorFormat("Reinit failed for game '{0}'. No plugin is set", this.Name);
                return false;
            }

            var pluginName = this.Plugin.Name;
            this.InitPlugin(pluginName);

            if (this.Plugin != null)
            {
                this.roomState = this.gameStateFactory.Create();

                this.EventCache.SetGameAppCounters(gameAppCounters);

                this.isClosed = false;
                this.allowSetGameState = true;
                this.failureOnCreate = false;
                this.callEnv = new CallEnv(this.Plugin, this.Name);
                return true;
            }

            Log.ErrorFormat("Reinit failed for game '{0}'. Failed to recreate plugin:'{1}'", this.Name, pluginName);
            return false;
        }

        /// <summary>
        /// Handles the <see cref="LeaveRequest"/> and calls <see cref="HiveGame.RemovePeerFromGame"/>.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <param name="sendParameters">
        /// The send Parameters.
        /// </param>
        /// <param name="leaveOperation">
        /// The leave Operation.
        /// </param>
        protected override void HandleLeaveOperation(HivePeer peer, SendParameters sendParameters, LeaveRequest leaveOperation)
        {
            var actor = this.GetActorByPeer(peer);
            if (actor != null)
            {
                RequestHandler handler = () =>
                {
                    try
                    {
                        return this.ProcessLeaveGame(actor.ActorNr, leaveOperation, sendParameters, peer);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Exception: {0}", e);
                        throw;
                    }
                };

                var info = new LeaveGameCallInfo(this.PendingPluginContinue, this.callEnv)
                {
                    ActorNr = actor.ActorNr,
                    UserId = peer.UserId,
                    Nickname = actor.Nickname,
                    IsInactive = leaveOperation != null && leaveOperation.IsCommingBack,
                    Reason = LeaveReason.LeaveRequest,
                    Request = leaveOperation,
                    Handler = handler,
                    Peer = peer,
                    SendParams = sendParameters,
                };
                try
                {
                    this.Plugin.OnLeave(info);
                }
                catch (Exception e)
                {
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
                }
            }
            else
            {
                this.LogQueue.Add(new LogEntry("HandleLeaveOperation", string.Format("Failed to find Actor for peer {0}", peer.ConnectionId)));
            }
        }

        /// <summary>
        ///   Handles the <see cref = "RaiseEventRequest" />: Sends a <see cref = "CustomEvent" /> to actors in the room.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "raiseEventRequest">
        ///   The operation
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected override void HandleRaiseEventOperation(HivePeer peer, RaiseEventRequest raiseEventRequest, SendParameters sendParameters)
        {
            // get the actor who send the operation request
            var actor = this.GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            if (raiseEventRequest.HttpForward
                && this.HttpForwardedOperationsLimit > 0
                && this.httpForwardedRequests.Increment(1) > this.HttpForwardedOperationsLimit)
            {
                this.SendErrorResponse(peer, raiseEventRequest.OperationCode,
                    ErrorCode.HttpLimitReached, HiveErrorMessages.HttpForwardedOperationsLimitReached, sendParameters);

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Game '{0}' userId '{1}' RaiseEvent denied. msg:{2} -- peer:{3}", this.Name, peer.UserId,
                        HiveErrorMessages.HttpForwardedOperationsLimitReached, peer);
                }
                return;
            }

            RequestHandler handler = () =>
            {
                try
                {
                    return this.ProcessRaiseEvent(peer, raiseEventRequest, sendParameters, actor);
                }
                catch (Exception e)
                {
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e, raiseEventRequest);
                    Log.Error(raiseEventRequest.DumpRequest());
                    Log.Error(this.ActorsManager.DumpActors());
                    Log.ErrorFormat("Master Client id = {0}, SuprressRoomEvent:{1}", this.MasterClientId, this.SuppressRoomEvents);
                    Log.Error(e);
                }
                return false;
            };

            var info = new RaiseEventCallInfo(this.PendingPluginContinue, this.callEnv)
                {
                    ActorNr = actor.ActorNr,
                    Request = raiseEventRequest,
                    UserId = peer.UserId,
                    Nickname = actor.Nickname,
                    Handler = handler,
                    Peer = peer,
                    SendParams = sendParameters,
                };
            try
            {
                this.Plugin.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        protected override void HandleRemovePeerMessage(HivePeer peer, int reason, string details)
        {
            if ((reason == LeaveReason.PlayerTtlTimedOut)
                || (reason == LeaveReason.LeaveRequest))
            {
                throw new ArgumentException("PlayerTtlTimeout and LeaveRequests are handled in their own routines.");
            }

            var actor = this.ActorsManager.ActorsGetActorByPeer(peer);
            var actorNr = actor != null ? actor.ActorNr : -1;

            var isInactive = reason != LeaveReason.PluginRequest && peer.JoinStage == HivePeer.JoinStages.Complete && this.PlayerTTL != 0;

            var mockRequest = new OperationRequest((byte)OperationCode.Leave)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.ActorNr, actorNr},
                    {(byte) ParameterKey.IsInactive, isInactive}
                }
            };
            var leaveRequest = new LeaveRequest(peer.Protocol, mockRequest);
            RequestHandler handler = () =>
            {
                try
                {
                    return this.ProcessHandleRemovePeerMessage(peer, isInactive);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Exception: {0}", e);
                    throw;
                }
            };
            var info = new LeaveGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                ActorNr = actorNr,
                UserId = peer.UserId,
                Nickname = actor != null ? actor.Nickname : string.Empty,
                IsInactive = isInactive,
                Reason = reason,
                Details = details,
                Handler = handler,
                Peer = peer,
                OperationRequest = leaveRequest,
                SendParams = new SendParameters()
            };
            try
            {
                this.Plugin.OnLeave(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        protected override void HandleSetPropertiesOperation(HivePeer peer, SetPropertiesRequest request, SendParameters sendParameters)
        {
            var actor = this.ActorsManager.ActorsGetActorByPeer(peer);

            if (actor == null)
            {
                this.SendErrorResponse(peer, request.OperationCode,
                    ErrorCode.OperationInvalid, HiveErrorMessages.PeetNotJoinedToRoom, sendParameters);

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Game '{0}' userId '{1}' SetProperties failed. msg:{2} -- peer:{3}", this.Name, peer.UserId,
                        HiveErrorMessages.PeetNotJoinedToRoom, peer);
                }
                return;
            }

            if (request.HttpForward
                && this.HttpForwardedOperationsLimit > 0
                && this.httpForwardedRequests.Increment(1) > this.HttpForwardedOperationsLimit)
            {
                this.SendErrorResponse(peer, request.OperationCode,
                    ErrorCode.HttpLimitReached, HiveErrorMessages.HttpForwardedOperationsLimitReached, sendParameters);

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Game '{0}' userId '{1}' SetProperties failed. msg:{2} -- peer:{3}", this.Name, peer.UserId,
                        HiveErrorMessages.HttpForwardedOperationsLimitReached, peer);
                }
                return;
            }

            RequestHandler handler = () =>
            {
                Hashtable oldValues = null;
                try
                {
                    string errorMsg;
                    if (!this.ValidateAndFillSetPropertiesRequest(peer, request, out errorMsg))
                    {
                        this.SendErrorResponse(peer, (byte)OperationCode.SetProperties, 
                            ErrorCode.OperationInvalid, errorMsg, sendParameters);
                        this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.SetPropertiesPreconditionsFail, null, errorMsg);
                        return false;
                    }
                    oldValues = this.GetOldPropertyValues(request);
                    return this.ProcessBeforeSetProperties(peer, request, oldValues, sendParameters);
                }
                catch (Exception e)
                {
                    if (oldValues != null)
                    {
                        this.RevertProperties(oldValues, request);
                    }
                    this.SendErrorResponse(peer, (byte)OperationCode.SetProperties, ErrorCode.InternalServerError, e.ToString(), sendParameters);
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e, request);
                    Log.Error(e);
                    return false;
                }
            };

            var info = new BeforeSetPropertiesCallInfo(this.PendingPluginContinue, this.callEnv)
                           {
                               Request = request,
                               UserId = peer.UserId,
                               Nickname = actor.Nickname,
                               Handler = handler,
                               Peer = peer,
                               SendParams = sendParameters,
                               ActorNr = actor.ActorNr,
                           };
            try
            {
                this.Plugin.BeforeSetProperties(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        protected virtual bool ProcessBeforeJoinGame(JoinGameRequest joinRequest, SendParameters sendParameters, HivePeer peer)
        {
            Actor actor;
            if (!this.JoinApplyGameStateChanges(peer, joinRequest, sendParameters, out actor))
            {
                this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, joinRequest);
                return false;
            }

            peer.JoinStage = HivePeer.JoinStages.BeforeJoinComplete;

            var info = new JoinGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                UserId = peer.UserId,
                Peer = peer,
                Nickname = actor.Nickname,
                ActorNr = actor.ActorNr,
                Request = joinRequest,
                JoinParams = new ProcessJoinParams(),
                OnFail = (reason, parameters) => this.OnJoinFailHandler(LeaveReason.PluginFailedJoin, reason, parameters, peer, sendParameters, joinRequest),
            };

            RequestHandler handler = () =>
            {
                try
                {
                    if (!this.ProcessJoin(actor.ActorNr, joinRequest, sendParameters, info.JoinParams, peer))
                    {
                        // here we suppose that error response is already sent
                        this.JoinFailureHandler(LeaveReason.ServerDisconnect, peer, joinRequest);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Exception:{0}", e);
                    throw;
                }
                return true;
            };

            info.Handler = handler;

            try
            {
                this.Plugin.OnJoin(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(ErrorCodes.UnhandledException, e);
            }
            return true;
        }

        protected virtual bool ProcessBeforeSetProperties(HivePeer peer, SetPropertiesRequest request, Hashtable oldValues, SendParameters sendParameters)
        {
            string errorMsg;
            if (!this.SetNewPropertyValues(request, out errorMsg))
            {
                this.RevertProperties(oldValues, request);
                this.SendErrorResponse(peer, (byte)OperationCode.SetProperties, ErrorCode.OperationInvalid, errorMsg, sendParameters);
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.SetPropertiesCASFail, null, errorMsg);
                return false;
            }

            RequestHandler handler = () =>
            {
                try
                {
                    return this.ProcessSetProperties(peer, true, string.Empty, request, sendParameters);
                }
                catch (Exception e)
                {
                    this.RevertProperties(oldValues, request);
                    this.SendErrorResponse(peer, (byte)OperationCode.SetProperties, ErrorCode.InternalServerError, e.ToString(), sendParameters);
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.SetPropertiesException, e, request);
                    Log.Error(e);
                    return false;
                }
            };

            // we checked that actor is not null in HandleSetPropertiesOperation, but 
            // it is still possible that actor will be null, because we do not know 
            // how plugin will handle OnBeforeSetProperties. if it will do http request, than client peer may disconnect
            // so, we still need to check that client is not null

            var actor = request.SenderActor;
            var info = new SetPropertiesCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                Request = request,
                Handler = handler,
                Peer = peer,
                UserId = peer.UserId,
                Nickname = actor != null ? actor.Nickname : string.Empty,
                SendParams = sendParameters,
                ActorNr = actor != null ? actor.ActorNr : -1,
                OnFail = (errorMessage, objects) => this.OnSetPropertiesFailHandler(errorMessage, objects, oldValues, request, peer, sendParameters),
            };
            try
            {
                this.Plugin.OnSetProperties(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
            return true;
        }

        private void OnSetPropertiesFailHandler(string errorMessage, Dictionary<byte, object> parameters, Hashtable oldPropertyValues, SetPropertiesRequest request, HivePeer peer, SendParameters sendParameters)
        {
            LogPlugin.Error(errorMessage);
            this.SendErrorResponse(peer, request.OperationCode,
                ErrorCode.PluginReportedError, errorMessage, sendParameters, parameters);
            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat("Game '{0}' userId '{1}' SetProperties plugin error. msg:{2} -- peer:{3}", this.Name, peer.UserId, errorMessage, peer);
            }

            this.RevertProperties(oldPropertyValues, request);
        }

        protected virtual bool ProcessSetProperties(HivePeer peer, bool result, string errorMsg, SetPropertiesRequest request, SendParameters sendParameters)
        {
            this.PublishResultsAndSetGameProperties(result, errorMsg, request, peer, sendParameters);
            return true;
        }

        protected virtual bool ProcessBeforeCloseGame(CloseRequest request)
        {
            // we currently allow the plugin to set the TTL - it could be changed
            // through pluginhost properties ... we could remove that feature.
            // plugin.OnClose() is responsible for saving
            this.EmptyRoomLiveTime = request.EmptyRoomTTL;

            this.RemoveRoomPath = RemoveState.ProcessBeforeCloseGameCalled;
            if (this.EmptyRoomLiveTime <= 0)
            {
                this.RemoveRoomPath = RemoveState.ProcessBeforeCloseGameCalledEmptyRoomLiveTimeLECalled;
                this.TriggerPluginOnClose();
                return true;
            }

            this.ExecutionFiber.Enqueue(() => this.ScheduleTriggerPluginOnClose(this.EmptyRoomLiveTime));
            return true;
        }

        private void ScheduleTriggerPluginOnClose(int roomLiveTime)
        {
            this.RemoveRoomPath = RemoveState.ScheduleTriggerPluginOnCloseCalled;
            if (this.RemoveTimer != null)
            {
                this.RemoveTimer.Dispose();
                this.RemoveTimer = null;
            }

            this.RemoveTimer = this.ExecutionFiber.Schedule(this.TriggerPluginOnClose, roomLiveTime);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Scheduled TriggerPluginOnClose: roomName={0}, liveTime={1:N0}", this.Name, roomLiveTime);
            }
        }

        private void TriggerPluginOnClose()
        {
            this.RemoveRoomPath = RemoveState.TriggerPluginOnCloseCalled;
            if (this.ActorsManager.ActorsCount > 0)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Room still has actors(Count={0}). We stop removing.room:{1}", this.ActorsManager.Count, this.Name);
                }
                // game already has players. stop closing it
                this.RemoveRoomPath = RemoveState.AliveGotNewPlayer;
                return;
            }

            RequestHandler handler = () =>
            {
                try
                {
                    return this.ProcessCloseGame(null);
                }
                catch (Exception e)
                {
                    this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
                    this.TryRemoveRoomFromCache();
                    Log.Error(e);
                }
                return false;
            };

            var info = new CloseGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                ActorCount = this.ActorsManager.InactiveActorsCount,
                Handler = handler,
                Peer = null,
                SendParams = new SendParameters(),
                FailedOnCreate = this.failureOnCreate,
                Request = new CloseRequest(),
            };

            try
            {
                this.Plugin.OnCloseGame(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
            finally
            {
                this.isClosed = true;
            }
        }

        private void OnJoinFailHandler(byte leaveReason, string reasonDetails, Dictionary<byte, object> parameters,
            HivePeer peer, SendParameters sendParameters, JoinGameRequest request)
        {
            this.SendErrorResponse(peer, request.OperationCode,
                ErrorCode.PluginReportedError, reasonDetails, sendParameters, parameters);

            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat("OnJoinFailHandler: Game '{0}' userId '{1}' failed to join. msg:{2} -- peer:{3}", this.Name, peer.UserId, reasonDetails, peer);
            }

            this.JoinFailureHandler(leaveReason, peer, request);
        }

        protected override void JoinFailureHandler(byte leaveReason, HivePeer peer, JoinGameRequest request)
        {
            base.JoinFailureHandler(leaveReason, peer, request);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("JoinFailureHandler is called for peer with reason:{0}.room:{1},p:{2}", request.FailureReason, this.Name, peer);
            }
            this.CallPluginOnLeaveIfJoinFailed(leaveReason, peer, request);
            peer.ScheduleDisconnect();
        }

        private void CallPluginOnLeaveIfJoinFailed(byte reason, HivePeer peer, JoinGameRequest request)
        {
            if (peer.JoinStage == HivePeer.JoinStages.Connected || peer.JoinStage == Hive.HivePeer.JoinStages.CreatingOrLoadingGame)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Peer join stage is {0}. CallPluginOnLeaveIfJoinFailed will be skiped. p:{1}", peer.JoinStage, peer);
                }
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Peer join stage is {0}. reason:{1}. CallPluginOnLeaveIfJoinFailed is called. p:{2}", peer.JoinStage, reason, peer);
            }

            var actor = this.GetActorByPeer(peer);

            RequestHandler handler = () =>
            {
                try
                {
                    this.RemovePeerFromGame(peer, false);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Exception: {0}", e);
                    throw;
                }
                return true;
            };

            var info = new LeaveGameCallInfo(this.PendingPluginContinue, this.callEnv)
            {
                ActorNr = actor != null ? actor.ActorNr : -1,
                UserId = actor != null ? actor.UserId : peer.UserId,
                Nickname = actor != null ? actor.Nickname : request.GetNickname(),
                IsInactive = false,
                Reason = reason,
                Request = null,
                Handler = handler,
                Peer = null,
                SendParams = new SendParameters(),
            };
            try
            {
                this.Plugin.OnLeave(info);
            }
            catch (Exception e)
            {
                this.Plugin.ReportError(Photon.Hive.Plugin.ErrorCodes.UnhandledException, e);
            }
        }

        protected virtual bool ProcessCloseGame(object state)
        {
            this.RemoveRoomPath = RemoveState.ProcessCloseGameCalled;
            // enqueuing the remove from cache ensures we perform tasks enqueued by the plugin 
            // before we trigger the room release.
            // TBD - we might need something to make sure we limit potential disruption (only relevant for developer plugins)
            this.ExecutionFiber.Enqueue(this.TryRemoveRoomFromCache);
            return true;
        }

        protected virtual bool ProcessCreateGame(HivePeer peer, JoinGameRequest joinRequest, SendParameters sendParameters)
        {
            var result = this.CreateGame(peer, joinRequest, sendParameters);

            this.ActorsManager.DeactivateActors(this);

            return result;
        }

        protected virtual bool ProcessJoin(int actorNr, JoinGameRequest joinRequest, SendParameters sendParameters, ProcessJoinParams prms, HivePeer peer)
        {
            return this.JoinSendResponseAndEvents(peer, joinRequest, sendParameters, actorNr, prms);
        }

        protected virtual bool ProcessLeaveGame(int actorNr, LeaveRequest request, SendParameters sendParameters, HivePeer peer)
        {
            this.LeaveOperationHandler(peer, sendParameters, request);
            return true;
        }

        protected virtual bool ProcessRaiseEvent(HivePeer peer, RaiseEventRequest raiseEventRequest, SendParameters sendParameters, Actor actor)
        {
            return this.RaiseEventOperationHandler(peer, raiseEventRequest, sendParameters, actor);
        }

        private bool ProcessRemoveInactiveActor(Actor actor)
        {
            base.RemoveInactiveActor(actor);
            return true;
        }

        private bool ProcessHandleRemovePeerMessage(HivePeer actorPeer, bool isCommingBack)
        {
            this.RemovePeerFromGame(actorPeer, isCommingBack);
            return true;
        }

        protected override OperationResponse GetUserJoinResponse(JoinGameRequest joinRequest, int actorNr, ProcessJoinParams prms)
        {
            var res = base.GetUserJoinResponse(joinRequest, actorNr, prms);
            if (this.Plugin.Name != "Default")
            {
                res.Parameters.Add((byte)ParameterKey.PluginName, this.Plugin.Name);
                res.Parameters.Add((byte)ParameterKey.PluginVersion, this.Plugin.Version);
            }
            return res;
        }

        public SerializableGameState GetSerializableGameState()
        {
            return this.RoomState.GetSerializableGameState();
        }

        public Dictionary<string, object> GetGameState()
        {
            return this.RoomState.GetState();
        }

        public bool SetGameState(Dictionary<string, object> state)
        {
            if (!this.allowSetGameState)
            {
                Log.ErrorFormat("Plugin {0} tries to set game state after call to 'Continue'. Game '{1}', stack\n{2}",
                    this.Plugin.Name, this.Name, GetCallStack());
                return false;
            }
            var res = this.RoomState.SetState(state);

            Log.DebugFormat("Loading Room: actorNumberCounter={0} DeleteCacheOnLeave={1} EmptyRoomLiveTime={2} IsOpen={3} IsVisible={4} LobbyId={5} LobbyType={6} MaxPlayers={7} PlayerTTL={8} SuppressRoomEvents={9}",
                this.ActorsManager.ActorNumberCounter,//0
                this.DeleteCacheOnLeave,//1
                this.EmptyRoomLiveTime,//2
                this.IsOpen,//3
                this.IsVisible,//4
                this.LobbyId,//5
                this.LobbyType,//6
                this.MaxPlayers,//7
                this.PlayerTTL,//8
                this.SuppressRoomEvents//9
                );

            return res;
        }

        protected static string GetCallStack()
        {
            var st = new StackTrace();
            var sb = new StringBuilder();
            sb.AppendLine("Stack:");
            var count = st.FrameCount;
            if (count > 10)
            {
                count -= 10;
            }

            for (var i = 1; i < count; ++i)
            {
                sb.AppendLine(st.GetFrame(i).GetMethod().ToString());
            }
            return sb.ToString();
        }

        #endregion
    }

    public class HttpResponseImpl : IHttpResponse
    {
        #region Constants and Fields

        private readonly int httpCode;

        private readonly string reason;

        private readonly byte[] responseData;

        private readonly string responseText;

        private readonly byte status;

        private readonly int webStatus;

        #endregion

        #region Constructors and Destructors

        public HttpResponseImpl(HttpRequest request, byte[] responseData, string responseText, HttpRequestQueueResultCode status, int httpCode, string reason, int webStatus)
        {
            this.Request = request;
            this.responseData = responseData;
            this.responseText = responseText;
            this.status = (byte)status;
            this.httpCode = httpCode;
            this.reason = reason;
            this.webStatus = webStatus;
        }

        #endregion

        #region Properties

        public HttpRequest Request { get; private set; }

        public int HttpCode
        {
            get
            {
                return this.httpCode;
            }
        }

        public string Reason
        {
            get
            {
                return this.reason;
            }
        }

        public byte[] ResponseData
        {
            get
            {
                return this.responseData;
            }
        }

        public string ResponseText
        {
            get
            {
                return this.responseText;
            }
        }

        public byte Status
        {
            get
            {
                return this.status;
            }
        }

        public int WebStatus
        {
            get
            {
                return this.webStatus;
            }
        }

        #endregion
    }
}
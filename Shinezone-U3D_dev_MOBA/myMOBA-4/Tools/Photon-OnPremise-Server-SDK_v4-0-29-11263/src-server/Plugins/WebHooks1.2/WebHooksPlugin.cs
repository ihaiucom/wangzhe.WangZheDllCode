using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Photon.Hive.Plugin.WebHooks
{
    public class WebHooksPlugin : PluginBase
    {

        #region Constants

        private const string BaseUrlKey = "BaseUrl";

        private const string GameCloseKey = "PathClose";

        private const string GameCreateKey = "PathCreate";

        private const string GameEventKey = "PathEvent";

        private const string GameHasErrorEventsKey = "HasErrorInfo";

        private const string GameIsPersistentKey = "IsPersistent";

        private const string GameJoinKey = "PathJoin";

        private const string GameLeaveKey = "PathLeave";

        private const string GameLoadKey = "PathLoad";

        private const string GamePropertiesKey = "PathGameProperties";

        private const string CustomHttpHeadersKey = "CustomHttpHeaders";

        #endregion

        #region Fields

        protected bool SuccesfullLoaded;

        private string baseUrl = string.Empty;

//        private Dictionary<string, string> pluginConfig;

        private string gameClosedUrl = string.Empty;

        private string gameCreatedUrl = string.Empty;

        private string gameEventUrl = string.Empty;

        private string gameJoinUrl = string.Empty;

        private string gameLeaveUrl = string.Empty;

        private string gameLoadUrl = string.Empty;

        private string gamePropertiesUrl = string.Empty;

        private Dictionary<string, string> customHttpHeaders;

        private bool hasErrorEvents;

        private bool isPersistentFlag;

        #endregion

        #region Public Properties

        public override bool IsPersistent
        {
            get
            {
                return this.isPersistentFlag && !string.IsNullOrEmpty(this.gameCreatedUrl) && !string.IsNullOrEmpty(this.gameClosedUrl);
            }
        }

        public override string Name
        {
            get
            {
                return "Webhooks";
            }
        }

        #endregion

        public WebHooksPlugin()
        {
            this.UseStrictMode = true;
        }

        #region Public Methods and Operators

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            if (!this.SuccesfullLoaded)
            {
                this.PluginHost.LogDebug("Skipped further OnCloseGame: not succesfullyloaded.");
                info.Continue();
                return;
            }

            if (this.IsPersistent)
            {
                this.PluginHost.LogDebug("OnCloseGame");

                var state = info.ActorCount > 0 ? this.GetGameState() : null;

                var actors = new List<object>();
                foreach (var actor in this.PluginHost.GameActors)
                {
                    actors.Add(new Actor { ActorNr = actor.ActorNr, UserId = actor.UserId });
                }

                this.PluginHost.LogDebug(string.Format("Http request to {0}", info.ActorCount > 0 ? "save" : "close"));

                this.PostJsonRequest(
                    this.gameClosedUrl,
                    new WebhooksRequest
                    {
                        Type = info.ActorCount > 0 ? "Save" : "Close",
                        GameId = this.PluginHost.GameId,
                        AppId = this.AppId,
                        AppVersion = this.AppVersion,
                        Region = this.Region,
                        ActorCount = info.ActorCount,
                        State = state,
                        AuthCookie = null,
                    },
                    this.SaveCallback,
                    info,
                    callAsync: false);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.gameClosedUrl))
                {
                    this.PostJsonRequest(
                        this.gameClosedUrl,
                        new WebhooksRequest
                        {
                            Type = "Close",
                            GameId = this.PluginHost.GameId,
                            AppId = this.AppId,
                            AppVersion = this.AppVersion,
                            Region = this.Region,
                            ActorCount = 0
                        },
                        this.LogIfFailedCallback,
                        callAsync: false);
                }

                info.Continue();
            }
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            if (!info.IsJoin)
            {
                var url = this.gameCreatedUrl;
                if (this.IsPersistent)
                {
                    this.PostJsonRequest(
                        url,
                        new WebhooksRequest
                        {
                            Type = "Create",
                            GameId = this.PluginHost.GameId,
                            AppId = this.AppId,
                            AppVersion = this.AppVersion,
                            Region = this.Region,
                            UserId = info.UserId,
                            Nickname = info.Nickname,
                            ActorNr = 1,
                            CreateOptions = info.CreateOptions,
                            AuthCookie = info.AuthCookie,
                        },
                        this.CreateGameCallback,
                        info,
                        callAsync: false);
                }
                else
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        this.PostJsonRequest(
                            url,
                            new WebhooksRequest
                            {
                                Type = "Create",
                                GameId = this.PluginHost.GameId,
                                AppId = this.AppId,
                                AppVersion = this.AppVersion,
                                Region = this.Region,
                                UserId = info.UserId,
                                Nickname = info.Nickname,
                                ActorNr = 1,
                                CreateOptions = info.CreateOptions,
                                AuthCookie = info.AuthCookie,
                            },
                            this.LogIfFailedCallback,
                            info,
                            callAsync: true);
                    }

                    this.SuccesfullLoaded = true;
                    info.Continue();
                }
            }
            else
            {
                this.PluginHost.LogDebug("OnCreateGame: Loading");

                if (this.IsPersistent)
                {
                    var url = !string.IsNullOrEmpty(this.gameLoadUrl) ? this.gameLoadUrl : this.gameCreatedUrl;

                    this.PluginHost.LogDebug(string.Format(
                        "OnCreateGame: sending http load game request: url={0} gameid={1} userid={2}",
                        this.gameLoadUrl,
                        this.PluginHost.GameId,
                        info.UserId));

                    this.PostJsonRequest(
                        url,
                        new WebhooksRequest
                        {
                            Type = "Load",
                            GameId = this.PluginHost.GameId,
                            AppId = this.AppId,
                            AppVersion = this.AppVersion,
                            Region = this.Region,
                            UserId = info.UserId,
                            Nickname = info.Nickname,
                            ActorNr = info.Request.ActorNr,
                            CreateIfNotExists = info.CreateIfNotExists,
                            CreateOptions = info.CreateOptions,
                            AuthCookie = info.AuthCookie,
                        },
                        this.LoadCallback,
                        info,
                        callAsync: false);
                }
                else
                {
                    if (!info.CreateIfNotExists)
                    {
                        //TBD: thsi should never be called, maybe we should move this to base plugin
                        throw new Exception("Join/Rejoin: missing CreateIfNotExists.");
                    }

                    this.SuccesfullLoaded = true;
                    info.Continue();
                }
            }
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            // no sync OnJoin support for now - since we already changed the room state when we get here.
            // to allow a clean sync (cancelable OnJoin) we need to 
            // a. run all the pre-checks
            // b. call OnJoin with the expected new actornr
            // c. on continue - add peer to room 

            if (!string.IsNullOrEmpty(this.gameJoinUrl))
            {
                // remove for callAsync=false
                info.Continue();

                this.PostJsonRequest(
                    this.gameJoinUrl,
                    new WebhooksRequest
                    {
                        Type = "Join",
                        GameId = this.PluginHost.GameId,
                        AppId = this.AppId,
                        AppVersion = this.AppVersion,
                        Region = this.Region,
                        UserId = info.UserId,
                        Nickname = info.Nickname,
                        ActorNr = info.ActorNr,
                        AuthCookie = info.AuthCookie,
                    },
                    this.LogIfFailedCallback,
                    info,
                    callAsync: true);
            }
            else
            {
                info.Continue();
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            base.OnLeave(info);

            var url = this.gameLeaveUrl;
            if (!string.IsNullOrEmpty(url)) // && info.ActorNr != -1 we don't restrict this anymore as in forwardplugin to see all leaves
            {
                this.PostJsonRequest(
                    url,
                    new WebhooksRequest
                    {
                        Type = LeaveReason.ToString(info.Reason),
                        GameId = this.PluginHost.GameId,
                        AppId = this.AppId,
                        AppVersion = this.AppVersion,
                        Region = this.Region,
                        UserId = info.UserId,
                        Nickname = info.Nickname,
                        ActorNr = info.ActorNr,
                        IsInactive = info.IsInactive,
                        Reason = info.Reason.ToString(),
                        AuthCookie = WebFlags.ShouldSendAuthCookie(info.Request.WebFlags) ? info.AuthCookie : null,
                    },
                    this.LogIfFailedCallback,
                    callAsync: true);
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            base.OnRaiseEvent(info);

            var raiseEventRequest = info.Request;
            var url = this.gameEventUrl;

            if (raiseEventRequest.HttpForward && !string.IsNullOrEmpty(url))
            {
                var state = WebFlags.ShouldSendState(info.Request.WebFlags) ? this.GetGameState() : null;

                this.PostJsonRequest(
                    url,
                    new WebhooksRequest
                    {
                        Type = "Event",
                        GameId = this.PluginHost.GameId,
                        AppId = this.AppId,
                        AppVersion = this.AppVersion,
                        Region = this.Region,
                        UserId = info.UserId,
                        Nickname = info.Nickname,
                        ActorNr = info.ActorNr,
                        Data = raiseEventRequest.Data,
                        State = state,
                        AuthCookie = WebFlags.ShouldSendAuthCookie(info.Request.WebFlags) ? info.AuthCookie : null,
                        EvCode =  raiseEventRequest.EvCode,
                    },
                    this.LogIfFailedCallback,
                    null,
                    callAsync: !WebFlags.ShouldSendSync(info.Request.WebFlags));
            }
        }

        private SerializableGameState GetGameState()
        {
            if (this.IsPersistent)
            {
                try
                {
                    return this.PluginHost.GetSerializableGameState();
                }
                catch (Exception ex)
                {
                    this.PluginHost.LogFatal(ex.Message);
                    throw;
                }
            }
            return null;
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            base.OnSetProperties(info);

            var setPropertiesRequest = info.Request;
            var url = this.gamePropertiesUrl;

            if (setPropertiesRequest.HttpForward && !string.IsNullOrEmpty(url))
            {
                var state = WebFlags.ShouldSendState(info.Request.WebFlags) ? this.GetGameState() : null;

                this.PostJsonRequest(
                    url,
                    new WebhooksRequest
                    {
                        Type = info.Request.ActorNumber == 0 ? "Game" : "Actor",
                        TargetActor = info.Request.ActorNumber == 0 ? null : (int?)info.Request.ActorNumber,
                        GameId = this.PluginHost.GameId,
                        AppId = this.AppId,
                        AppVersion = this.AppVersion,
                        Region = this.Region,
                        UserId = info.UserId,
                        Nickname = info.Nickname,
                        ActorNr = info.ActorNr,
                        Properties = setPropertiesRequest.Properties,
                        State = state,
                        AuthCookie = WebFlags.ShouldSendAuthCookie(info.Request.WebFlags) ? info.AuthCookie : null,
                    },
                    this.LogIfFailedCallback,
                    null,
                    callAsync: !WebFlags.ShouldSendSync(info.Request.WebFlags));
            }
        }

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            if (!base.SetupInstance(host, config, out errorMsg))
            {
                return false;
            }

            if (config != null && config.ContainsKey(BaseUrlKey))
            {
//                this.pluginConfig = config;

                this.baseUrl = this.GetKeyValue(config, BaseUrlKey);
                if (!string.IsNullOrEmpty(this.baseUrl))
                {
                    this.isPersistentFlag = this.GetBoolKeyValue(config, GameIsPersistentKey);
                    this.hasErrorEvents = this.GetBoolKeyValue(config, GameHasErrorEventsKey);

                    this.baseUrl = this.baseUrl.Trim();

                    this.gameCreatedUrl = this.GetUrl(config, GameCreateKey);
                    this.gameClosedUrl = this.GetUrl(config, GameCloseKey);
                    this.gameJoinUrl = this.GetUrl(config, GameJoinKey);
                    this.gameLeaveUrl = this.GetUrl(config, GameLeaveKey);
                    this.gameLoadUrl = this.GetUrl(config, GameLoadKey);
                    this.gamePropertiesUrl = this.GetUrl(config, GamePropertiesKey);
                    this.gameEventUrl = this.GetUrl(config, GameEventKey);
                    var headers = this.GetKeyValue(config, CustomHttpHeadersKey);
                    if (!string.IsNullOrEmpty(headers))
                    {
                        this.customHttpHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headers);
                    }

                    host.LogDebug(this.gameCreatedUrl);
                    host.LogDebug(this.gameClosedUrl);
                    host.LogDebug(this.gameEventUrl);

//                    this.PluginHost.LogDebug("SetupInstance config:" + config.SerializeToString());
                    return true;
                }
            }

            errorMsg = string.Format("Non null 'config' containing key '{0}' with non empty string value expected.", BaseUrlKey);
            return false;
        }

        #endregion

        #region Methods

        private void CreateGameCallback(IHttpResponse httpResponse, object userState)
        {
            string errorMsg;
            WebhooksResponse forwardResponse;

            if (!this.TryGetForwardResponse(httpResponse, out forwardResponse, out errorMsg))
            {
                var msg = string.Format("Failed to create game on {0} : {1}", httpResponse.Request.Url, errorMsg);
                this.ReportError(msg);
                ((ICallInfo)userState).Fail(msg);
            }
            else
            {
                this.SuccesfullLoaded = true;
                ((ICallInfo)userState).Continue();
            }
        }

        private bool GetBoolKeyValue(Dictionary<string, string> config, string key)
        {
            System.Diagnostics.Debug.Assert(config != null, "config shouldn't be null!");

            string s;
            if (config.TryGetValue(key, out s))
            {
                bool result;
                bool.TryParse(s, out result);
                return result;
            }

            return false;
        }

        private string GetKeyValue(Dictionary<string, string> config, string key)
        {
            System.Diagnostics.Debug.Assert(config != null, "config shouldn't be null!");

            string value;
            config.TryGetValue(key, out value);
            return value;
        }

        private string GetUrl(Dictionary<string, string> config, string key)
        {
            var setting = this.GetKeyValue(config, key);
            if (!string.IsNullOrEmpty(setting))
            {
                setting = setting.Trim();

                if (setting.Length != 0)
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

                    var url = urlbase + "/" + setting + "?" + urltail;
                    url = url.Replace("{AppId}", this.AppId.Trim().Replace(" ", string.Empty));
                    url = url.Replace("{AppVersion}", this.AppVersion.Trim().Replace(" ", string.Empty));
                    url = url.Replace("{Region}", this.AppVersion.Trim().Replace(" ", string.Empty));
                    url = url.Replace("{Cloud}", this.Cloud.Trim().Replace(" ", string.Empty));
                    return url;
                }
            }

            return string.Empty;
        }

        private void LoadCallback(IHttpResponse httpResponse, object userState)
        {
            string errorMsg;
            WebhooksResponse response;
            var callInfo = (ICreateGameCallInfo)userState;

            if (this.TryGetForwardResponse(httpResponse, out response, out errorMsg))
            {
                if (response.ResultCode == 0)
                {
                    if (response.State != null)
                    {
                        try
                        {
                            if (!this.PluginHost.SetGameState(response.State))
                            {
                                callInfo.Fail(string.Format("Failed to load state from {0}.", httpResponse.Request.Url));
                            }
                            else
                            {
                                this.SuccesfullLoaded = true;
                                callInfo.Continue();
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                callInfo.Fail(string.Format("Failed to load state from {0} : Exception='{1}'", httpResponse.Request.Url, ex.Message));
                            }
                            catch (Exception e)
                            {
                                this.ReportError(string.Format("Failed to load state from {0} : Exception='{1}'.", httpResponse.Request.Url, e));
                            }

                            this.ReportError(string.Format("Failed to load state from {0} : Exception='{1}'.", httpResponse.Request.Url, ex));
                        }
                    }
                    else
                    {
                        // should only be tha case for join with CreateIfNotExists, (we should probably have a reload flag)
                        this.PluginHost.LogDebug("Creating Game without loading any state - Rejoins will fail!");
                        this.SuccesfullLoaded = true;
                        callInfo.Continue();
                    }
                }
                else
                {
                    var msg = string.Format("Failed to load state from {0} : {1} {2}", httpResponse.Request.Url, response.ResultCode, response.Message);
                    this.ReportError(msg);
                    callInfo.Fail(msg);
                }
            }
            else
            {
                var msg = string.Format("Failed to load state from {0} : {1}", httpResponse.Request.Url, errorMsg);
                this.ReportError(msg);
                callInfo.Fail(msg);
            }
        }

        private void LogIfFailedCallback(IHttpResponse httpResponse, object userState)
        {
            string errorMsg;
            WebhooksResponse forwardResponse;

            if (!this.TryGetForwardResponse(httpResponse, out forwardResponse, out errorMsg))
            {
                this.ReportError(string.Format("Failed to forward request to {0} : {1}", httpResponse.Request.Url, errorMsg));
            }
        }

        private void PostJsonRequest(string url, WebhooksRequest forwardRequest, HttpRequestCallback callback, object userState = null, bool callAsync = false)
        {
            var stream = new MemoryStream();
            var json = JsonConvert.SerializeObject(forwardRequest, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
            });
            var data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);

            var request = new HttpRequest
            {
                Url = url,
                Method = "POST",
                Accept = "application/json",
                ContentType = "application/json",
                Callback = callback,
                UserState = userState,
                CustomHeaders = this.customHttpHeaders,
                DataStream = stream,
                Async = callAsync
            };

            this.PluginHost.LogDebug(string.Format("PostJsonRequest: {0} - {1}", url, json));

            this.PluginHost.HttpRequest(request);
        }

        private void ReportError(string msg)
        {
            this.PluginHost.LogError(msg);

            if (this.hasErrorEvents)
            {
                this.PluginHost.BroadcastErrorInfoEvent(msg);
            }
        }

        private void SaveCallback(IHttpResponse request, object userState)
        {
            var info = (ICloseGameCallInfo)userState;

            if (request.Status == HttpRequestQueueResult.Success)
            {
                this.PluginHost.LogDebug(string.Format("Http request to {0} - done.", info.ActorCount > 0 ? "save" : "close"));
                info.Continue();
                return;
            }

            var msg = string.Format("Failed save game request on {0} : {1}", this.gameClosedUrl, request.Reason);
            this.ReportError(msg);
            info.Continue();
        }

        private bool TryGetForwardResponse(IHttpResponse httpResponse, out WebhooksResponse response, out string errorMsg)
        {
            response = null;

            try
            {
                if (httpResponse.Status == HttpRequestQueueResult.Success)
                {
                    if (string.IsNullOrEmpty(httpResponse.ResponseText))
                    {
                        errorMsg = string.Format("Missing Response.");
                        return false;
                    }

                    var responseData = Encoding.UTF8.GetString(httpResponse.ResponseData);
                    this.PluginHost.LogDebug("TryGetForwardResponse:" + responseData);

                    if (!responseData.StartsWith("{") && !responseData.EndsWith("}"))
                    {
                        errorMsg = string.Format("Response is not valid json '{0}'.", responseData);
                        return false;
                    }

                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<WebhooksResponse>(responseData);
                    if (response != null)
                    {
                        if (response.ResultCode == 0)
                        {
                            errorMsg = string.Empty;
                            return true;
                        }

                        if (response.ResultCode == 255 && string.IsNullOrEmpty(response.Message))
                        {
                            errorMsg = string.Format("Unexpected Response '{0}'.", responseData);
                            return false;
                        }

                        errorMsg = string.Format("Error response ResultCode='{0}' Message='{1}'.", response.ResultCode, response.Message);
                    }
                    else
                    {
                        // since we prevalidate, we don't seam to get here
                        errorMsg = string.Format("Unexpected Response '{0}'.", responseData);
                    }
                }
                else
                {
                    errorMsg = string.Format(
                        "'{0}' httpcode={1} webstatus={2} response='{3}', HttpQueueResult={4}.",
                        httpResponse.Reason,
                        httpResponse.HttpCode,
                        httpResponse.WebStatus,
                        httpResponse.ResponseText,
                        httpResponse.Status);
                }
            }
            catch (Exception ex)
            {
                // since we prevalidate, we don't seam to get here
                errorMsg = ex.Message;
            }

            return false;
        }

        #endregion
    }
}

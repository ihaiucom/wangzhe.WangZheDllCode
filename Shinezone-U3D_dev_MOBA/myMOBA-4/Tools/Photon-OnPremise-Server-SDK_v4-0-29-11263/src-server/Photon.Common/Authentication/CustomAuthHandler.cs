using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.Common.Authentication.Configuration.Auth;
using Photon.Common.Authentication.CustomAuthentication;
using Photon.Common.Authentication.Data;
using Photon.Common.Authentication.Diagnostic;
using Photon.SocketServer;
using Photon.SocketServer.Net;

namespace Photon.Common.Authentication
{
    public class CustomAuthHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly PoolFiber fiber;

        protected Dictionary<ClientAuthenticationType, ClientAuthenticationQueue> authenticationServices = new Dictionary<ClientAuthenticationType, ClientAuthenticationQueue>();

        protected bool isAnonymousAccessAllowed;

        protected IHttpRequestQueueCountersFactory httpQueueCountersFactory;

        protected CustomAuthResultCounters.Instance TotalInstance = CustomAuthResultCounters.GetInstance("_Total");

        public CustomAuthHandler(IHttpRequestQueueCountersFactory factory)
        {
            this.fiber = new PoolFiber();
            this.fiber.Start();
            this.httpQueueCountersFactory = factory;
        }

        public bool IsAnonymousAccessAllowed
        {
            get
            {
                return this.isAnonymousAccessAllowed;
            }
            protected set
            {
                this.isAnonymousAccessAllowed = value;
            }
        }

        public bool IsClientAuthenticationEnabled { get; protected set; }

        public void AuthenticateClient(ICustomAuthPeer peer, IAuthenticateRequest authRequest, AuthSettings authSettings, SendParameters sendParameters, object state)
        {
            //TBD: why are we enqueuing could be done on the peers fiber
            this.fiber.Enqueue(() => 
                this.OnAuthenticateClient(peer, authRequest, authSettings, sendParameters, state)
            );
        }

        public void InitializeFromConfig()
        {
            this.IsClientAuthenticationEnabled = false;
            var config = Configuration.Auth.AuthSettings.Default;
            if (config == null)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("There is no configuration for custom auth in config");
                }
                return;
            }

            if (!config.Enabled)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("AuthSettings are disabled in config. No custom authentication");
                }
                return;
            }

            this.IsClientAuthenticationEnabled = true;
            this.isAnonymousAccessAllowed = config.ClientAuthenticationAllowAnonymous;

            foreach (AuthProvider provider in config.AuthProviders)
            {
                this.AddNewAuthProvider(provider.AuthUrl, provider.NameValuePairAsQueryString, provider.RejectIfUnavailable,
                    (ClientAuthenticationType)provider.AuthenticationType, ApplicationBase.Instance.PhotonInstanceName + "_" + provider.Name);
            }
        }

        protected void AddNewAuthProvider(string url, string nameValuePairAsQueryString, bool rejectIfUnavailable,
            ClientAuthenticationType authenticationType, string instanceName)
        {
            var authService = new ClientAuthenticationQueue(
                url,
                nameValuePairAsQueryString,
                rejectIfUnavailable,
                Settings.Default.HttpRequestTimeoutMS)
                              {
                                  MaxQueuedRequests = Settings.Default.MaxQueuedRequests,
                                  MaxConcurrentRequests = Settings.Default.MaxConcurrentRequests,
                                  ReconnectInterval = TimeSpan.FromSeconds(Settings.Default.ReconnectIntervalS),
                                  QueueTimeout = TimeSpan.FromSeconds(Settings.Default.QueueTimeoutS),
                                  MaxErrorRequests = Settings.Default.MaxErrorRequests,
                                  MaxTimedOutRequests = Settings.Default.MaxTimedOutRequests,
                                  MaxBackoffTimeInMiliseconds = Settings.Default.MaxBackoffTimeInMiliseconds,
                                  CustomData = CustomAuthResultCounters.GetInstance(instanceName)
                              };

            var counters = this.httpQueueCountersFactory != null ? this.httpQueueCountersFactory.Create(instanceName) : null;
            authService.SetHttpRequestQueueCounters(counters);
            this.authenticationServices.Add(authenticationType, authService);
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Auth Provider added. provider:{0}", url);
            }
        }

        protected virtual void OnAuthenticateClient(ICustomAuthPeer peer, IAuthenticateRequest authRequest, AuthSettings authSettings, SendParameters sendParameters, object state)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Authenticating client {0} - custom authentication type: {1}",
                         peer.ConnectionId,
                         authRequest.ClientAuthenticationType);
                }

                // when authentication data is provided check if
                // it is either a byte array or string and convert to byte array
                // if it's a string value
                byte[] authData = null;
                if (authRequest.ClientAuthenticationData != null)
                {
                    authData = authRequest.ClientAuthenticationData as byte[];
                    if (authData == null)
                    {
                        var stringData = authRequest.ClientAuthenticationData as string;
                        if (stringData == null)
                        {
                            peer.OnCustomAuthenticationError(
                                ErrorCode.CustomAuthenticationFailed,
                                "Authentication data type not supported",
                                authRequest,
                                sendParameters,
                                state);

                            this.IncrementFailedCustomAuth();
                            return;
                        }

                        authData = Encoding.UTF8.GetBytes(stringData);
                    }
                }

                if (string.IsNullOrEmpty(authRequest.ClientAuthenticationParams) && authData == null && this.isAnonymousAccessAllowed)
                {
                    // instant callback - treat as anonymous user: 
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Authenticate client: grant access as anonymous user: conId={0}", peer.ConnectionId);
                    }

                    var customResult = new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Ok };
                    peer.OnCustomAuthenticationResult(customResult, authRequest, sendParameters, state);
                    return;
                }

                // take auth type from auth request (default: custom)
                var authenticationType = (ClientAuthenticationType)authRequest.ClientAuthenticationType;

                ClientAuthenticationQueue authQueue;
                if (this.authenticationServices.TryGetValue(authenticationType, out authQueue) == false)
                {
                    if (log.IsWarnEnabled)
                    {
                        log.WarnFormat("Authentication type not supported: {0} for AppId={1}/{2}", authenticationType, 
                            authRequest.ApplicationId, authRequest.ApplicationVersion);
                    }

                    peer.OnCustomAuthenticationError(
                        ErrorCode.CustomAuthenticationFailed,
                        "Authentication type not supported",
                        authRequest,
                        sendParameters,
                        state);
                    this.IncrementFailedCustomAuth();
                    return;
                }

                var queueState = new AuthQueueState(peer, authRequest, sendParameters, state);
                authQueue.EnqueueRequest(authRequest.ClientAuthenticationParams, authData, this.AuthQueueResponseCallback, queueState);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void AuthQueueResponseCallback(AsyncHttpResponse response, ClientAuthenticationQueue queue)
        {
            var queueState = (AuthQueueState)response.State;
            var peer = queueState.Peer;
            var authRequest = queueState.AuthenticateRequest;
            var sendParameters = queueState.SendParameters;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Authenticate client finished: conId={0}, result={1}", peer.ConnectionId, response.Status);
            }

            switch (response.Status)
            {
                case HttpRequestQueueResultCode.Success:
                    {
                        if (response.ResponseData == null)
                        {
                            log.ErrorFormat("Custom authentication: failed. ResponseData is empty. AppId={0}/{1}", 
                                authRequest.ApplicationId, authRequest.ApplicationId);

                            return;
                        }

                        // deserialize
                        var responseString = Encoding.UTF8.GetString(response.ResponseData).Trim(new char[] { '\uFEFF', '\u200B' });
                        CustomAuthenticationResult customAuthResult;
                        try
                        {
                            customAuthResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomAuthenticationResult>(responseString);
                            //TBD: handle backward compatibility incustomAuthResult class
                            if (customAuthResult.AuthCookie == null)
                            {
                                customAuthResult.AuthCookie = customAuthResult.Secure;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.WarnFormat("Custom authentication: failed to deserialize response. AppId={0}/{1}, Response={2}, Uri={3}", 
                                authRequest.ApplicationId, authRequest.ApplicationVersion, responseString, queue.Uri);

                            peer.OnCustomAuthenticationError(
                                ErrorCode.CustomAuthenticationFailed, "Custom authentication deserialization failed: " + ex.Message, authRequest, sendParameters, queueState.State);
                            this.IncrementFailedCustomAuth();
                            return;
                        }

                        this.IncrementResultCounters(customAuthResult, (CustomAuthResultCounters.Instance)queue.CustomData);
                        peer.OnCustomAuthenticationResult(customAuthResult, authRequest, sendParameters, queueState.State);
                        return;
                    }

                case HttpRequestQueueResultCode.Offline:
                case HttpRequestQueueResultCode.QueueFull:
                case HttpRequestQueueResultCode.QueueTimeout:
                case HttpRequestQueueResultCode.RequestTimeout:
                case HttpRequestQueueResultCode.Error:
                    {
                        if (response.RejectIfUnavailable)
                        {
                            peer.OnCustomAuthenticationError(
                                ErrorCode.CustomAuthenticationFailed, "Custom authentication service error: " + response.Status, authRequest, sendParameters, queueState.State);
                            this.IncrementFailedCustomAuth();
                        }
                        else
                        {
                            var result = new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Ok };
                            peer.OnCustomAuthenticationResult(result, authRequest, sendParameters, queueState.State);
                        }
                        return;
                    }
            }
        }

        private void IncrementResultCounters(CustomAuthenticationResult customAuthResult, CustomAuthResultCounters.Instance instance)
        {
            switch (customAuthResult.ResultCode)
            {
                case CustomAuthenticationResultCode.Data:
                    this.TotalInstance.IncrementCustomAuthResultData();
                    instance.IncrementCustomAuthResultData();
                    break;
                case CustomAuthenticationResultCode.Ok:
                    this.TotalInstance.IncrementCustomAuthResultSuccess();
                    instance.IncrementCustomAuthResultSuccess();
                    break;
                default://CustomAuthenticationResultCode.Failed, CustomAuthenticationResultCode.ParameterInvalid
                    this.TotalInstance.IncrementCustomAuthResultFailed();
                    instance.IncrementCustomAuthResultFailed();
                    break;
            }
        }

        protected virtual void IncrementFailedCustomAuth()
        {
            
        }

        private class AuthQueueState
        {
            public readonly ICustomAuthPeer Peer;

            public readonly IAuthenticateRequest AuthenticateRequest;

            public readonly object State;

            public readonly SendParameters SendParameters;

            public AuthQueueState(ICustomAuthPeer peer, IAuthenticateRequest authenticateRequest, 
                SendParameters sendParameters, object state)
            {
                this.Peer = peer;
                this.AuthenticateRequest = authenticateRequest;
                this.State = state;
                this.SendParameters = sendParameters;
            }
        }
    }

}

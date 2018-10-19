using System;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class StrictModeFailurePlugin : PluginBase
    {
        private object timer;

        public StrictModeFailurePlugin()
        {
            this.UseStrictMode = true;
        }

        public override string Name
        {
            get { return this.GetType().Name; }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode == 0)
            {
                base.OnRaiseEvent(info);
            }
            else if (info.Request.EvCode == 1)
            {
                info.Defer();
            }
            else if (info.Request.EvCode == 2)
            {
                info.Cancel();
            }
            else if (info.Request.EvCode == 3)
            {
                info.Fail();
                this.PluginHost.BroadcastErrorInfoEvent("We called fail method");
            }
            else if (info.Request.EvCode == 4)
            {
                throw new Exception("Event 4 exception");
            }
            else if (info.Request.EvCode == 5)
            {
                var request = new HttpRequest
                {
                    Async = true,
                    Callback = HttpCallbackWithException,
                    Url = "http://photon.webscript.io/auth-demo/",
                };

                this.PluginHost.HttpRequest(request);
                info.Defer();
            }
            else if (info.Request.EvCode == 6)
            {
                var request = new HttpRequest
                {
                    Async = false,
                    Callback = HttpCallbackWithException,
                    Url = "http://photon.webscript.io/auth-demo/",
                };

                this.PluginHost.HttpRequest(request);
                info.Continue();
            }
            else if (info.Request.EvCode == 7)
            {
                this.timer = this.PluginHost.CreateOneTimeTimer(this.TimerAction, 100);
                info.Defer();
            }
            else if (info.Request.EvCode == 8)
            {
                this.timer = this.PluginHost.CreateTimer(this.TimerAction, 100, 100);
                info.Defer();
            }
        }

        private void TimerAction()
        {
            this.PluginHost.StopTimer(this.timer);
            throw new Exception("Timer callback exception simulation");
        }

        private void HttpCallbackWithException(IHttpResponse response, object userstate)
        {
            throw new Exception("Simulation of exception in http callback");
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            string msg;
            if (errorCode == ErrorCodes.UnhandledException)
            {
                msg = string.Format("Got error report with exception {0}", exception);
            }
            {
                msg = string.Format("Got error report with code: {0}", errorCode);
            }

            this.PluginHost.BroadcastErrorInfoEvent(msg);
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            if (info.Request.ActorNumber == 0)
            {
                base.BeforeSetProperties(info);
            }
            else if (info.Request.ActorNumber == 1)
            {
                info.Cancel();
            }
            else if (info.Request.ActorNumber == 2)
            {
                info.Defer();
            }
            else if (info.Request.ActorNumber == 3)
            {
                info.Fail();
                this.PluginHost.BroadcastErrorInfoEvent("We called fail method");
            }
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            if (!this.PluginHost.GameId.EndsWith("OnSetPropertiesForgotCall"))
            {
                base.OnSetProperties(info);
            }
        }

        public override void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            if (!this.PluginHost.GameId.EndsWith("BeforeJoinForgotCall"))
            {
                base.BeforeJoin(info);
            }
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            if (!this.PluginHost.GameId.EndsWith("OnJoinForgotCall"))
            {
                base.OnJoin(info);
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            if (!this.PluginHost.GameId.EndsWith("OnLeaveForgotCall"))
            {
                base.OnLeave(info);
            }
        }
    }
}

using System;
using NUnit.Framework;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class JoinFailuresCheckPlugin : PluginBase
    {
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public override void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            var res = this.CheckBeforeBeforeJoin();
            if (!string.IsNullOrEmpty(res))
            {
                info.Fail(res);

                if (this.PluginHost.GameActors.Count != 1)
                {
                    this.PluginHost.BroadcastErrorInfoEvent("this.PluginHost.GameActors.Count != 1", info);
                }
                return;
            }

            base.BeforeJoin(info);
            try
            {
                this.CheckAfterBeforeJoin();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            var res = this.CheckBeforeOnJoin();
            if (!string.IsNullOrEmpty(res))
            {
                info.Fail(res);

                if (this.PluginHost.GameActors.Count != 1)
                {
                    this.PluginHost.BroadcastErrorInfoEvent("this.PluginHost.GameActors.Count != 1", info);
                }
                return;
            }

            try
            {
                // this part is not used anymore

                //if (this.PluginHost.GameProperties.ContainsKey("BlockJoinEvents"))
                //{
                //    info.JoinParams.PublishJoinEvents = false;
                //    info.JoinParams.ResponseExtraParameters = new Dictionary<byte, object>{{0, "Value0"}};
                //}

                //var actor2 = this.PluginHost.GameActors[1];
                //if(actor2.Properties.GetProperty("DoNotPublishCache") != null)
                //{
                //    info.JoinParams.PublishCache = false;
                //}

                base.OnJoin(info);
                this.CheckAfterOnJoin();
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.BroadcastEvent(123, null);
            base.OnLeave(info);
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            if (exception != null)
            {
                this.PluginHost.BroadcastErrorInfoEvent(exception.ToString());
            }
        }

        private string CheckBeforeOnJoin()
        {
            Assert.AreEqual(2, this.PluginHost.GameActors.Count);
            if (this.PluginHost.GameActors.Count == 2 && this.PluginHost.GameProperties.ContainsKey("FailBeforeOnJoin"))
            {
                return "FailBeforeOnJoin are set";
            }

            return string.Empty;
        }

        private void CheckAfterOnJoin()
        {
            Assert.AreEqual(2, this.PluginHost.GameActors.Count);
            if (this.PluginHost.GameActors.Count == 2 && this.PluginHost.GameProperties.ContainsKey("FailAfterOnJoin"))
            {
                Assert.Fail("FailAfterOnJoin are set");
            }
        }

        private string CheckBeforeBeforeJoin()
        {
            Assert.AreEqual(1, this.PluginHost.GameActors.Count);

            if (this.PluginHost.GameActors.Count > 0 && this.PluginHost.GameProperties.Contains("FailBeforeJoinPreCondition"))
            {
                return "FailBeforeJoinPreCondition is set";
            }
            return string.Empty;
        }

        private void CheckAfterBeforeJoin()
        {
            if (this.PluginHost.GameId != "OnJoinCallsFail" 
                && this.PluginHost.GameId != "JoinLogicFailTest")
            {
                Assert.AreEqual(2, this.PluginHost.GameActors.Count);

                var actor2 = this.PluginHost.GameActors[1];
                Assert.IsTrue(1 <= actor2.Properties.Count);
                Assert.AreEqual("Actor2PropertyValue", actor2.Properties.GetProperty("Actor2Property").Value);
            }
            else
            {
                Assert.AreEqual(1, this.PluginHost.GameActors.Count);
            }
        }

    }
}

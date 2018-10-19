using System;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class JoinExceptionsPlugin : PluginBase
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
            this.BeforeJoinBeforeContinueFail(info);
            base.BeforeJoin(info);
            this.BeforeJoinAfterContinueFail(info);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            this.OnJoinBeforeContinueFail(info);
            base.OnJoin(info);
            this.OnJoinAfterContinueFail(info);
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.PluginHost.LogError(string.Format("OnLeave {0}", info.ActorNr));
            this.BroadcastEvent(123, null);
            base.OnLeave(info);
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.GameStateCheck(info);
            base.OnRaiseEvent(info);
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            this.BroadcastEvent(124, null);
            base.ReportError(errorCode, exception, state);
            this.PluginHost.LogError("ReportError");
        }

        private void BeforeJoinBeforeContinueFail(IBeforeJoinGameCallInfo info)
        {
            this.GenerateExceptionIf(info.Nickname, "BeforeJoinBeforeContinueFail");
        }

        private void BeforeJoinAfterContinueFail(IBeforeJoinGameCallInfo info)
        {
            this.GenerateExceptionIf(info.Nickname, "BeforeJoinAfterContinueFail");
        }

        private void OnJoinBeforeContinueFail(IJoinGameCallInfo info)
        {
            this.GenerateExceptionIf(info.Nickname, "OnJoinBeforeContinueFail");
        }

        private void OnJoinAfterContinueFail(IJoinGameCallInfo info)
        {
            this.GenerateExceptionIf(info.Nickname, "OnJoinAfterContinueFail");
        }

        private void GenerateExceptionIf(string nickname, string hit)
        {
            if (this.PluginHost.GameActors.Count < 1)
            {
                return;
            }

            if (nickname == hit)
            {
                throw new Exception(hit);
            }
        }

        private void GameStateCheck(IRaiseEventCallInfo info)
        {
        }
    }
}

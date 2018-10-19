using System;
using System.Collections.Generic;

namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Special plugin that causes most callbacks to fail.
    /// </summary>
    public class ErrorPlugin : IGamePlugin
    {
        private readonly string errorMsg;

        public string Name { get; private set; }
        public string Version { get { return "1.0"; } }

        public bool IsPersistent { get; private set; }
        public bool UseStrictMode { get; private set; }

        public string Message { get { return errorMsg; } }

        public ErrorPlugin(string msg)
        {
            this.IsPersistent = false;
            this.Name = "ErrorPlugin";
            this.errorMsg = msg;
            this.UseStrictMode = true;
        }
        /// <summary>
        /// Calls info.Continue
        /// </summary>
        /// <param name="info"></param>
        public void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            info.Continue();
        }
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            info.Fail(this.errorMsg);
        }
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            info.Fail(this.errorMsg);
        }
        /// <summary>
        /// Calls info.Continue
        /// </summary>
        /// <param name="info"></param>
        public void OnCloseGame(ICloseGameCallInfo info)
        {
            info.Continue();
        }
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void OnCreateGame(ICreateGameCallInfo info)
        {
            info.Fail(this.errorMsg);
        }

#if PLUGINS_0_9
        public void OnDisconnect(IDisconnectCallInfo info)
        {
            info.Continue();
        }
#endif
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void OnJoin(IJoinGameCallInfo info)
        {
            info.Fail(this.errorMsg);
        }
        /// <summary>
        /// Calls info.Continue
        /// </summary>
        /// <param name="info"></param>
        public void OnLeave(ILeaveGameCallInfo info)
        {
            info.Continue();
        }
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            info.Fail(this.errorMsg);
        }
        /// <summary>
        /// Calls info.Fail
        /// </summary>
        /// <param name="info"></param>
        public void OnSetProperties(ISetPropertiesCallInfo info)
        {
            info.Fail(this.errorMsg);
        }
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>False.</returns>
        public bool OnUnknownType(Type type, ref object value)
        {
            return false;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="e"></param>
        /// <param name="state"></param>
        public void ReportError(short errorCode, Exception e, object state = null)
        {
            
        }
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns>true.</returns>
        public bool SetupInstance(IPluginHost host, Dictionary<string, string> config)
        {
            return true;
        }

#if PLUGINS_0_9
        [Obsolete("Use overloaded version")]
        public void OnJoin(IJoinCallInfo info)
        {
            info.Fail(this.errorMsg);
        }

        [Obsolete("Use overloaded version")]
        public void OnLeave(ILeaveCallInfo info)
        {
            info.Continue();
        }
#endif
        /// <summary>
        /// Does nothing. Only sets "errorMessage" to empty string.
        /// Does not set plugin host, nor initiliaze plugin using configuration.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="errorMessage"></param>
        /// <returns>true</returns>
        public bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }
    }
}

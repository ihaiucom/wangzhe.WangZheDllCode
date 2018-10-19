// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Photon.Hive.Plugin
{
    using System.Collections.Generic;
    /// <summary>
    /// Base plugin class that should be extended to make custom ones.
    /// </summary>
    public class PluginBase : IGamePlugin
    {
        [Obsolete]
        public static Version PluginsVersion = new Version(1, 2);
        [Obsolete]
        public static Version BuildVersion = new Version();

        #region Properties
        /// <summary>
        /// Reference to the game hosting the plugin.
        /// </summary>
        public IPluginHost PluginHost { get; protected set; }
        /// <summary>
        /// Version of the application to which this plugin belongs to.
        /// </summary>
        public string AppVersion
        {
            get
            {
                if (this.PluginHost.Environment.ContainsKey("AppVersion"))
                {
                    return ((string) this.PluginHost.Environment["AppVersion"]);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// ID of the application to which this plugin belongs to.
        /// </summary>
        public string AppId
        {
            get
            {
                if (this.PluginHost.Environment.ContainsKey("AppId"))
                {
                    return (string) this.PluginHost.Environment["AppId"];
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Cloud region to which the application is connected to.
        /// </summary>
        public string Region
        {
            get
            {
                if (this.PluginHost.Environment.ContainsKey("Region"))
                {
                    return (string) this.PluginHost.Environment["Region"];
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Type of cloud the application is connected to. Public or Enterprise.
        /// </summary>
        public string Cloud
        {
            get
            {
                if (this.PluginHost.Environment.ContainsKey("Cloud"))
                {
                    return (string) this.PluginHost.Environment["Cloud"];
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Full string environment information.
        /// </summary>
        public string EnvironmentVerion 
        {
            get
            {
                var env = this.PluginHost.GetEnvironmentVersion();
                return string.Format("Build:{0};Plugin={1}", env.BuiltWithVersion, env.HostVersion);
            }
        }


        #endregion
        /// <summary>
        /// Constructor.
        /// </summary>
        public PluginBase()
        {
#if !PLUGINS_0_9
            this.UseStrictMode = true;
#endif
        }
        #region Implemented Interfaces

        #region IGamePlugin
        /// <summary>
        /// Name of the plugin. Default is not allowed.
        /// This should be the name used when requesting the plugin from the client in CreateGame operation.
        /// This will be returned to the client in the CreateGame operation response.
        /// </summary>
        public virtual string Name
        {
            get { return "Default"; }
        }
        /// <summary>
        /// Version of the plugin.
        /// This will be returned to the client in the CreateGame operation response.
        /// </summary>
        public virtual string Version
        {
            get { return "1.0"; }
        }
        /// <summary>
        /// Indicates whether or not serialized room state should be persisted between sessions.
        /// </summary>
        /// <remarks>This is mainly related to webhooks plugin in particular.</remarks>
        public virtual bool IsPersistent
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Indicates whether the plugin uses strict mode or not.
        /// If a plugin is in "strict mode" then all callbacks should be 
        /// processed one of the available methods. 
        /// If plugin version >= 1.0 then this should be true.
        /// If plugin version <= 0.9 then this should be false.
        /// </summary>
        public bool UseStrictMode { get; protected set; }
        /// <summary>
        /// Plugin callback called when a game instance is about to be removed from Photon servers memory.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            try
            {
                this.BeforeCloseGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback called when a peer is about to join a room.
        /// This is triggered by Op Join when a game instance is in Photon servers memory.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            try
            {
                this.BeforeJoin(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback triggered by Op SetProperties.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            try
            {
                this.BeforeSetProperties(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeCloseGame"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnCloseGame(ICloseGameCallInfo info)
        {
            try
            {
                this.OnCloseGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeCloseGame"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnCreateGame(ICreateGameCallInfo info)
        {
            try
            {
                this.OnCreateGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }

#if PLUGINS_0_9
        void IGamePlugin.OnDisconnect(IDisconnectCallInfo info)
        {
            try
            {
                this.OnDisconnect(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
#endif
        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeJoin"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnJoin(IJoinGameCallInfo info)
        {
            try
            {
                this.OnJoin(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback when a peer is disconnected from the room.
        /// The corresponding actor is either removed or marked as inactive.
        /// This can be triggered by an explicit or unexpected Disconnect or a call to Op Leave or RemoveActor.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnLeave(ILeaveGameCallInfo info)
        {
            try
            {
                this.OnLeave(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback when Op RaiseEvent is called.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                this.OnRaiseEvent(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Plugin callback called when info.Continue() is called inside <see cref="IGamePlugin.BeforeSetProperties"/>.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        void IGamePlugin.OnSetProperties(ISetPropertiesCallInfo info)
        {
            try
            {
                this.OnSetProperties(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                ((IGamePlugin)this).ReportError(ErrorCodes.UnhandledException, e);
                CallFailSafe(info, e.ToString());
            }
        }
        /// <summary>
        /// Callback triggered when trying to deseriliaze unknwon type.
        /// </summary>
        /// <param name="type">The Type of the object.</param>
        /// <param name="value">The object with unknown type.</param>
        /// <returns>If the unkown type could be handled successfully.</returns>
        bool IGamePlugin.OnUnknownType(Type type, ref object value)
        {
            return this.OnUnknownType(type, ref value);
        }
        
        private string GetPluginInfo()
        {
            return string.Format("Name:{0}, Version:{1} AppId:{2}, AppVersion:{3}", this.Name, this.Version, this.AppId, this.AppVersion);
        }
        /// <summary>
        /// Report an internal plugin error.
        /// </summary>
        /// <param name="errorCode">Code of the error. <see cref="Photon.Hive.Plugin.ErrorCodes"/></param>
        /// <param name="exception">Exception thrown.</param>
        /// <param name="state">Optional object to be added in the report. It could help in debugging the error.</param>
        void IGamePlugin.ReportError(short errorCode, Exception exception, object state)
        {
            switch (errorCode)
            {
                case ErrorCodes.MissingCallProcessing:
                {
                    var msg = string.Format("MissingCallProcessing for type:{1} in Plugin:'{0}'.", this.GetPluginInfo(), state);
                    this.PluginHost.LogError(msg);
                    break;
                }
                case ErrorCodes.UnhandledException:
                {
                    var msg = string.Format("UnhandledException:'{0}'. Exception: {1}", this.GetPluginInfo(), exception);
                    this.PluginHost.LogError(msg);
                    break;
                }
                case ErrorCodes.AsyncCallbackException:
                {
                    var msg = string.Format("AsyncCallbackException:'{0}'. Exception: {1}", this.GetPluginInfo(), exception);
                    this.PluginHost.LogError(msg);
                    break;
                }
                case ErrorCodes.SetPropertiesException:
                {
                    var msg = string.Format("SetPropertiesException:'{0}'. Exception: {1}", this.GetPluginInfo(), exception);
                    this.PluginHost.LogError(msg);
                    break;
                }
                default:
                {
                    //var msg = string.Format("Error in Plugin:'{0}'. Errorcode: {1}", this.GetPluginInfo(), errorCode);
                    //this.PluginHost.LogError(msg);
                    break;
                }
            }

            this.ReportError(errorCode, exception, state);
        }

#if PLUGINS_0_9
        void IGamePlugin.OnJoin(IJoinCallInfo info)
        {
            this.OnJoin(info);
            this.StrictModeCheck(info);
        }

        void IGamePlugin.OnLeave(ILeaveCallInfo info)
        {
            this.OnLeave(info);
            this.StrictModeCheck(info);
        }
#endif
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnCloseGame(ICloseGameCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnCreateGame(ICreateGameCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }

#if PLUGINS_0_9
        public virtual void OnDisconnect(IDisconnectCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
#endif
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnJoin(IJoinGameCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// Handles MasterClient switch if needs be. (if the leaving actor is MasterClient)
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnLeave(ILeaveGameCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);

            var masterClientId = this.PluginHost.MasterClientId;
            info.Continue();
            var newMasterClientId = this.PluginHost.MasterClientId;
            if (masterClientId != newMasterClientId)
            {
                this.OnChangeMasterClientId(masterClientId, newMasterClientId);
            }
        }
        /// <summary>
        /// MasterClient change callback. 
        /// </summary>
        /// <param name="oldId">Actor number of the old MasterClient.</param>
        /// <param name="newId">Actor number of the new MasterClient.</param>
        protected virtual void OnChangeMasterClientId(int oldId, int newId)
        {
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnSetProperties(ISetPropertiesCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Calls info.Continue(). Override to change.
        /// </summary>
        /// <param name="info">Data passed in the callback call.</param>
        public virtual void OnSetPropertiesFailed(ISetPropertiesFailedCallInfo info)
        {
            System.Diagnostics.Debug.Assert(this.PluginHost != null);
            info.Continue();
        }
        /// <summary>
        /// Does nothing. 
        /// Override to change.
        /// </summary>
        /// <param name="type">The Type of the object.</param>
        /// <param name="value">The object with unknown type.</param>
        /// <returns>False.</returns>
        public virtual bool OnUnknownType(Type type, ref object value)
        {
            return false;
        }
        /// <summary>
        /// Initialize plugin instance.
        /// </summary>
        /// <param name="host">The game hosting the plugin.</param>
        /// <param name="config">The plugin assembly key/value configuration entries.</param>
        /// <param name="errorMsg">Error message in case something wrong happens when setting up the plugin instance.</param>
        /// <returns>If the plugin instance setup is successful.</returns>
        public virtual bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            errorMsg = "";
            if (this.PluginHost != null)
            {
                errorMsg = "PluginHost is already set. Possible same instance of plugin used twice!";
                return false;
            }

            this.PluginHost = host;

            return (this.PluginHost != null);
        }
        /// <summary>
        /// Does nothing.
        /// Override to change.
        /// </summary>
        /// <param name="errorCode">Code of the error. <see cref="Photon.Hive.Plugin.ErrorCodes"/></param>
        /// <param name="exception">Exception thrown.</param>
        /// <param name="state">Optional object to be added in the report. It could help in debugging the error.</param>
        protected virtual void ReportError(short errorCode, Exception exception, object state)
        {

        }

#if PLUGINS_0_9
        #region Obsolete
        [Obsolete("Use overloaded version")]
        public virtual bool SetupInstance(IPluginHost host, Dictionary<string, string> config)
        {
            string errorMsg;
            return this.SetupInstance(host, config, out errorMsg);
        }

        [Obsolete("Use overloaded version")]
        public virtual void OnJoin(IJoinCallInfo info)
        {
            this.OnJoin((IJoinGameCallInfo)info);
        }

        [Obsolete("Use overloaded version")]
        public virtual void OnLeave(ILeaveCallInfo info)
        {
            this.OnLeave((ILeaveGameCallInfo)info);
        }
        #endregion
#endif
        #endregion

        #endregion

        #region Helpers
        /// <summary>
        /// Check if any of the processing method was called for the passed ICallInfo argument.
        /// </summary>
        /// <param name="callInfo"></param>
        protected virtual void StrictModeCheck(ICallInfo callInfo)
        {
            if (this.UseStrictMode && callInfo.IsNew)
            {
                var infoTypeName = callInfo.GetType().ToString();
                ((IGamePlugin)this).ReportError(Photon.Hive.Plugin.ErrorCodes.MissingCallProcessing, null, infoTypeName);
                callInfo.Fail(string.Format("none of {0}'s method were called", infoTypeName));
            }
        }

        /// <summary>
        /// Broadcast event to all actors joined to the room.
        /// </summary>
        /// <param name="code">Event code.</param>
        /// <param name="data">Event data.</param>
        protected void BroadcastEvent(byte code, Dictionary<byte, object> data)
        {
            this.PluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, code, data, 0);
        }

        private static void CallFailSafe(ICallInfo info, string errorMessage)
        {
            if (!info.IsProcessed)
            {
                info.Fail(errorMessage);
            }
        }

        #endregion

    }
}
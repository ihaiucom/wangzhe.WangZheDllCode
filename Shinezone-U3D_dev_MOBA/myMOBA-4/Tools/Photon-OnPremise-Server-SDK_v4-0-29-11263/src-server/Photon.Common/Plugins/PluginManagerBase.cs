// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ExitGames.Logging;
using Photon.Common.Plugins.Configuration;

namespace Photon.Common.Plugins
{
    public abstract class PluginManagerBase<IPluginFactory, IPlugin, IHost> : IPluginManager<IPlugin, IHost>
        where IPlugin : class
        where IPluginFactory : class
    {
        #region Constants and Fields

// ReSharper disable StaticFieldInGenericType
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();
// ReSharper restore StaticFieldInGenericType

        private Dictionary<string, string> pluginConfig;

        readonly Version currentPluginsVersion;
        private IPluginFactory pluginFactory;

        #endregion

        #region Constructors and Destructors

        protected PluginManagerBase(PluginInfo pluginInfo, string basePath)
        {
            this.currentPluginsVersion = typeof(IPlugin).Assembly.GetName().Version;
            this.UpdateConfiguration(pluginInfo, basePath);
        }

        protected PluginManagerBase(string basePath, PluginSettings settings)
        {
            this.currentPluginsVersion = typeof(IPlugin).Assembly.GetName().Version;
            try
            {
                if (settings.Enabled & settings.Plugins.Count > 0)
                {
                    var pluginSettings = settings.Plugins[0];

                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat("Plugin configured: name={0}", pluginSettings.Name);

                        if (pluginSettings.CustomAttributes.Count > 0)
                        {
                            foreach (var att in pluginSettings.CustomAttributes)
                            {
                                Log.InfoFormat("\tAttribute: {0}={1}", att.Key, att.Value);
                            }
                        }
                    }

                    var pluginInfo = new PluginInfo
                        {
                            Name = pluginSettings.Name, 
                            Version = pluginSettings.Version, 
                            AssemblyName = pluginSettings.AssemblyName,
                            Type = pluginSettings.Type, 
                            ConfigParams = pluginSettings.CustomAttributes
                        };

                    this.UpdateConfiguration(pluginInfo, string.IsNullOrEmpty(basePath) ? Environment.CurrentDirectory : basePath);
                }
                else
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Either plugins desabled or no plugins set");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion

        #region Properties
        public bool Initialized { get; protected set; }

        public Type Type4Load { get; private set; }

        public string PluginPath { get; private set; }

        #endregion

        #region Abstracts interface
        protected abstract IPlugin CreatePlugin(IPluginFactory factory, IHost sink, string pluginName, Dictionary<string, string> config, out string errorMsg);

        protected abstract IPlugin LoadErrorPlugin(IHost sink, string errorMsg);

        protected abstract IPlugin GetDefaultPlugin(IHost sink);
        #endregion

        #region Implemented Interfaces

        #region IPluginManager

        public virtual IPlugin GetPlugin(IHost sink, string pluginName)
        {
            if (this.pluginFactory == null)
            {
                if (string.IsNullOrEmpty(pluginName) || pluginName == "Default")
                {
                    return GetDefaultPlugin(sink);
                }
                return this.LoadErrorPlugin(sink, "PluginManager initialization failed.");
            }

            try
            {
                return this.CreatePluginWithFactory(sink, pluginName);
            }
            catch (Exception e)
            {
                var errorMsg = ExceptionToString(e);
                Log.Error(errorMsg);
                return this.LoadErrorPlugin(sink, errorMsg);
            }
        }

        private static string ExceptionToString(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} : {1}\n", exception.GetType().FullName, exception.Message);

            var ex = exception;
            DumpReflectionTypeLoadException(ex as ReflectionTypeLoadException, sb);
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                sb.Append("--> ").AppendFormat("{0} : {1}\n", ex.GetType().FullName, ex.Message);
                DumpReflectionTypeLoadException(ex as ReflectionTypeLoadException, sb);
            }
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(exception.StackTrace);
            return sb.ToString();
        }

        private static void DumpReflectionTypeLoadException(ReflectionTypeLoadException loadException, StringBuilder sb)
        {
            if (loadException != null)
            {
                sb.AppendLine("LoaderExceptions property value:");
                foreach (var loaderException in loadException.LoaderExceptions)
                {
                    sb.AppendFormat("[{0} : {1}]", loaderException.GetType().FullName, loaderException.Message).AppendLine();
                }
                sb.AppendLine("End of Loader Exceptions");
            }
        }

        private IPlugin CreatePluginWithFactory(IHost sink, string pluginName)
        {
            string errorMsg;
            var plugin = this.CreatePlugin(this.pluginFactory, sink, pluginName, this.pluginConfig, out errorMsg);
            if (plugin == null)
            {
                return this.LoadErrorPlugin(sink, errorMsg);
            }

            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("Plugin successfully created:Type:{0}, path:{1}", this.Type4Load, this.PluginPath);
            }

            return plugin;
        }

        #endregion

        #endregion

        #region Methods

        protected bool SetupPluginManager(string path, string typeName, Dictionary<string, string> config)
        {
            this.pluginConfig = config;
            this.PluginPath = path;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Plugin path is set to '{0}'", this.PluginPath);
            }

            try
            {
                var asm = Assembly.LoadFile(path);

                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat(
                        "Loaded Assembly Name={0}, Version={1}, Culture={2}, PublicKey token={3}, Path={4}",
                        asm.GetName().Name,
                        asm.GetName().Version,
                        asm.GetName().CultureInfo.Name,
                        (BitConverter.ToString(asm.GetName().GetPublicKeyToken())),
                        path);
                }

                var ipluginAssembly = typeof (IPlugin).Assembly;
                var pluginVersion = new Version();
                foreach (var an in asm.GetReferencedAssemblies())
                {
                    Log.InfoFormat(
                        "Referenced Assembly Name={0}, Version={1}, Culture={2}, PublicKey token={3}, Path={4}",
                        an.Name,
                        an.Version,
                        an.CultureInfo.Name,
                        (BitConverter.ToString(an.GetPublicKeyToken())),
                        an.CodeBase);

                    if (an.Name == ipluginAssembly.GetName().Name)
                    {
                        pluginVersion = an.Version;
                    }
                }

                if (pluginVersion.Major != currentPluginsVersion.Major || pluginVersion.MajorRevision != currentPluginsVersion.MajorRevision)
                {
                    Log.ErrorFormat("Plugins version Missmatch. PhotonHivePlugin Version:{0}; Plugin Name:{1} Version:{2}", currentPluginsVersion, asm.GetName().Name, pluginVersion);
                    return false;
                }

                this.CreateKnownType(typeName, asm);

                var obj = Activator.CreateInstance(this.Type4Load);

                this.pluginFactory = obj as IPluginFactory;

                if (this.pluginFactory == null)
                {
                    Log.ErrorFormat("Type {0} does not implement IPluginFactory interface", this.Type4Load.Name);
                    return false;
                }

                Log.InfoFormat("Plugin manager (version={0}) is setup. type={1};path={2};version={3}", 
                    currentPluginsVersion, typeName, path, pluginVersion);
                return true;
            }
            catch(Exception e)
            {
                Log.ErrorFormat("Got Exception during either loading of assembly {0} or type {1}.\nException {2}", path, typeName, e);
            }
            return false;
        }

        private void CreateKnownType(string typeName, Assembly asm)
        {
            this.Type4Load = asm.GetType(typeName, true);

            Log.InfoFormat("Plugin Type {0} from assembly {1} was successfuly created", typeName, asm.FullName);
        }

        #endregion

        #region Publics
        public void UpdateConfiguration(PluginInfo pluginInfo, string basePath)
        {
            try
            {
                var path = Path.Combine(basePath, pluginInfo.Path);
                this.Initialized = this.SetupPluginManager(path, pluginInfo.Type, pluginInfo.ConfigParams);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        #endregion
    }
}
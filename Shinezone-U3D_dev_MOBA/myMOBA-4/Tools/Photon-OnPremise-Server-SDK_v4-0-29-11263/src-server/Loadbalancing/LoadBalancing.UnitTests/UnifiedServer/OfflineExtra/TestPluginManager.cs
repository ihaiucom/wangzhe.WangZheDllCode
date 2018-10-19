using System;
using ExitGames.Logging;
using Photon.Hive.Plugin;
using TestPlugins;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.OfflineExtra
{
    public class TestPluginManager : IPluginManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly PluginFactory factory = new PluginFactory();
        public IPluginInstance GetGamePlugin(IPluginHost sink, string pluginName)
        {
            string errorMsg;
            try
            {
                var plugin = this.factory.Create(sink, pluginName, null, out errorMsg);
                if (plugin != null)
                {
                    return new PluginInstance {Plugin = plugin, Version = GetEnvironmentVersion()};
                }
                log.ErrorFormat("Plugin {0} creation failed with message: {1}", pluginName, errorMsg);
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                log.ErrorFormat("Plugin {0} creation failed with exception: {1}", pluginName, errorMsg);
            }

            return new PluginInstance { Plugin = new ErrorPlugin(errorMsg), Version = GetEnvironmentVersion()};
        }

        private static EnvironmentVersion GetEnvironmentVersion()
        {
            var currentPluginsVersion = typeof(PluginBase).Assembly.GetName().Version;
            return new EnvironmentVersion { BuiltWithVersion = currentPluginsVersion, HostVersion = currentPluginsVersion };
        }
    }
}

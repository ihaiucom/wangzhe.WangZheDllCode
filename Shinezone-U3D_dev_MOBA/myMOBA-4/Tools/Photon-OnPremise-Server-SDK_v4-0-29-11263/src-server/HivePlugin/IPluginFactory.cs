using System.Collections.Generic;

namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Base interface of plugin factory pattern.
    /// </summary>
    public interface IPluginFactory
    {
        /// <summary>
        /// Create and initialize a new plugin instance.
        /// </summary>
        /// <param name="gameHost">The game to host the plugin instance.</param>
        /// <param name="pluginName">The plugin name as requested by client in Op CreateGame.</param>
        /// <param name="config">The plugin assembly key/value configuration entries.</param>
        /// <param name="errorMsg">An eventual error message to return in case something goes wrong.</param>
        /// <returns>The plugin instance or null.</returns>
        IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg);
    }
}

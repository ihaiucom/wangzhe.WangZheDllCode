using System;

namespace Apollo
{
	public class PluginManager
	{
		private DictionaryView<string, PluginBase> pluginCollection = new DictionaryView<string, PluginBase>();

		public static PluginManager Instance = new PluginManager();

		private PluginManager()
		{
		}

		public void Add(PluginBase plugin)
		{
			if (plugin == null || string.IsNullOrEmpty(plugin.GetPluginName()))
			{
				throw new Exception("Plugin Name and plugin instance could not be empty or null");
			}
			string pluginName = plugin.GetPluginName();
			if (this.pluginCollection.ContainsKey(pluginName))
			{
				this.pluginCollection[pluginName] = plugin;
			}
			else
			{
				this.pluginCollection.Add(pluginName, plugin);
			}
		}

		public PluginBase GetPlugin(string name)
		{
			if (!string.IsNullOrEmpty(name) && this.pluginCollection.ContainsKey(name))
			{
				return this.pluginCollection[name];
			}
			return null;
		}

		public PluginBase GetCurrentPlugin()
		{
			string pluginName = ApolloCommon.ApolloInfo.PluginName;
			return this.GetPlugin(pluginName);
		}
	}
}

using System;

namespace Apollo.Plugins.WTLoginAdapter
{
	public class WTLoginAdapter : PluginBase
	{
		private const string pluginName = "WTLoginAdapter";

		public static WTLoginAdapter Instance = new WTLoginAdapter();

		private WTLoginAdapter()
		{
		}

		public override bool Install()
		{
			return true;
		}

		public override string GetPluginName()
		{
			return "WTLogin";
		}
	}
}

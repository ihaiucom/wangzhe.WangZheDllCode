using System;

namespace Apollo
{
	public class ApolloInfo
	{
		public string PluginName
		{
			get;
			set;
		}

		public int ServiceId
		{
			get;
			set;
		}

		public int MaxMessageBufferSize
		{
			get;
			set;
		}

		public string QQAppId
		{
			get;
			set;
		}

		public string WXAppId
		{
			get;
			set;
		}

		public ApolloInfo(string qqAppId, string wxAppId, int maxMessageBufferSize, string pluginName = "")
		{
			this.ServiceId = 255;
			this.MaxMessageBufferSize = maxMessageBufferSize;
			this.QQAppId = qqAppId;
			this.WXAppId = wxAppId;
			this.PluginName = pluginName;
		}

		public ApolloInfo(int serviceId, int maxMessageBufferSize, string pluginName = "")
		{
			this.ServiceId = serviceId;
			this.MaxMessageBufferSize = maxMessageBufferSize;
			this.PluginName = pluginName;
		}

		public ApolloInfo(int maxMessageBufferSize, string pluginName = "")
		{
			this.ServiceId = 10000;
			this.MaxMessageBufferSize = maxMessageBufferSize;
			this.PluginName = pluginName;
		}

		public ApolloInfo(string pluginName = "")
		{
			this.ServiceId = 10000;
			this.MaxMessageBufferSize = 102400;
			this.PluginName = pluginName;
		}
	}
}

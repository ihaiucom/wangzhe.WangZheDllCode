using System;

namespace Apollo
{
	public abstract class IApollo
	{
		private static IApollo instance;

		public static IApollo Instance
		{
			get
			{
				if (IApollo.instance == null)
				{
					IApollo.instance = new Apollo();
				}
				return IApollo.instance;
			}
		}

		public abstract void SetApolloLogger(ApolloLogPriority pri, ApolloLogHandler callback);

		public abstract ApolloResult Initialize(ApolloInfo platformInfo);

		public abstract bool SwitchPlugin(string pluginName);

		public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, string svrUrl);

		[Obsolete("Deprecated since 1.1.13, use CreateApolloConnection(ApolloPlatform platform,  String svrUrl) instead")]
		public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, uint permission, string svrUrl);

		[Obsolete("Deprecated since 1.1.2, use CreateApolloConnection(ApolloPlatform platform, UInt32 permission, String svrUrl) instead")]
		public abstract IApolloConnector CreateApolloConnection(ApolloPlatform platform, ApolloPermission permission, string svrUrl);

		public abstract void DestroyApolloConnector(IApolloConnector Connector);

		public abstract IApolloHttpClient CreateHttpClient();

		public abstract void DestoryHttpClient(IApolloHttpClient client);

		public abstract IApolloTalker CreateTalker(IApolloConnector connector);

		public abstract void DestroyTalker(IApolloTalker talker);

		public abstract IApolloAccountService GetAccountService();

		public abstract IApolloServiceBase GetService(int Type);
	}
}

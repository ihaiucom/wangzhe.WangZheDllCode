using System;

namespace Apollo
{
	public class ApolloCommon
	{
		public const string PluginName = "apollo";

		public const string ApolloPluginPluginName = "apollo";

		public const string ApolloExPluginName = "apollo";

		public const string ApolloTXPluginName = "apollo";

		public static byte MsgVersion = 1;

		private static uint msgSeq = 1u;

		private static ApolloInfo apolloInfo;

		public static uint MsgSeq
		{
			get
			{
				ApolloCommon.msgSeq += 2u;
				return ApolloCommon.msgSeq;
			}
		}

		public static ApolloInfo ApolloInfo
		{
			get
			{
				if (ApolloCommon.apolloInfo == null)
				{
					throw new Exception("IApollo.Instance.Initialize must be called before using Apollo!");
				}
				return ApolloCommon.apolloInfo;
			}
			set
			{
				ApolloCommon.apolloInfo = value;
			}
		}
	}
}

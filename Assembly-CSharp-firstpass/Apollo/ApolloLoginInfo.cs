using System;

namespace Apollo
{
	public class ApolloLoginInfo : ApolloStruct<ApolloLoginInfo>
	{
		public ApolloAccountInfo AccountInfo;

		public ApolloWaitingInfo WaitingInfo;

		public ApolloServerRouteInfo ServerInfo;

		public override ApolloLoginInfo FromString(string src)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(src);
			this.AccountInfo = apolloStringParser.GetObject<ApolloAccountInfo>("AccountInfo");
			this.WaitingInfo = apolloStringParser.GetObject<ApolloWaitingInfo>("WaitingInfo");
			string text = apolloStringParser.GetString("ServerInfo");
			if (text != null)
			{
				text = ApolloStringParser.ReplaceApolloStringQuto(text);
				this.ServerInfo = new ApolloServerRouteInfo();
				this.ServerInfo.FromString(text);
			}
			return this;
		}
	}
}

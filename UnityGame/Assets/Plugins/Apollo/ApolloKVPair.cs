using System;

namespace Apollo
{
	public class ApolloKVPair : ApolloStruct<ApolloKVPair>
	{
		public string Key;

		public string Value;

		public override ApolloKVPair FromString(string src)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(src);
			this.Key = apolloStringParser.GetString("key");
			this.Value = apolloStringParser.GetString("value");
			return this;
		}
	}
}

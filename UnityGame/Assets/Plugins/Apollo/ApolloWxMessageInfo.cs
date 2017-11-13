using System;

namespace Apollo
{
	public class ApolloWxMessageInfo
	{
		public ApolloWxMessageType Type;

		public ApolloWxMessageInfo(ApolloWxMessageType type)
		{
			this.Type = type;
		}

		internal virtual bool Pack(out string buffer)
		{
			buffer = "type=" + (int)this.Type;
			return true;
		}
	}
}

using System;

namespace Apollo
{
	public class ApolloWxButtonInfo
	{
		public ApolloWxButtonType Type;

		public ApolloWxButtonInfo(ApolloWxButtonType type)
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

using System;

namespace Apollo
{
	public class ApolloWxAppButton : ApolloWxButtonInfo
	{
		public string Name;

		public string MessageExt;

		public ApolloWxAppButton(ApolloWxButtonType type, string name, string messageExt) : base(type)
		{
			this.Name = name;
			this.MessageExt = messageExt;
		}

		internal override bool Pack(out string buffer)
		{
			base.Pack(out buffer);
			buffer = buffer + "&name=" + this.Name;
			buffer = buffer + "&messageExt=" + this.MessageExt;
			return true;
		}
	}
}

using System;

namespace Apollo
{
	public class ApolloWxWebButton : ApolloWxButtonInfo
	{
		public string Name;

		public string WebUrl;

		public ApolloWxWebButton(ApolloWxButtonType type, string name, string webUrl) : base(type)
		{
			this.Name = name;
			this.WebUrl = webUrl;
		}

		internal override bool Pack(out string buffer)
		{
			base.Pack(out buffer);
			buffer = buffer + "&name=" + this.Name;
			buffer = buffer + "&webUrl=" + ApolloStringParser.ReplaceApolloString(this.WebUrl);
			return true;
		}
	}
}

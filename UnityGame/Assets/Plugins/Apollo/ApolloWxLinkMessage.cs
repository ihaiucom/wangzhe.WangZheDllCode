using System;

namespace Apollo
{
	public class ApolloWxLinkMessage : ApolloWxMessageInfo
	{
		public string PicUrl;

		public string TargetUrl;

		public ApolloWxLinkMessage(ApolloWxMessageType type, string picUrl, string targetUrl) : base(type)
		{
			this.PicUrl = picUrl;
			this.TargetUrl = targetUrl;
		}

		internal override bool Pack(out string buffer)
		{
			base.Pack(out buffer);
			buffer = buffer + "&picUrl=" + ApolloStringParser.ReplaceApolloString(this.PicUrl);
			buffer = buffer + "&targetUrl=" + ApolloStringParser.ReplaceApolloString(this.TargetUrl);
			return true;
		}
	}
}

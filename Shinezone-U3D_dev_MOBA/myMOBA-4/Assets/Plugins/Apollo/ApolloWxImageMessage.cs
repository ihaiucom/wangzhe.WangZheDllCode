using System;

namespace Apollo
{
	public class ApolloWxImageMessage : ApolloWxMessageInfo
	{
		public string Url;

		public int Width;

		public int Height;

		public ApolloWxImageMessage(ApolloWxMessageType type, string url, int width, int height) : base(type)
		{
			this.Url = url;
			this.Width = width;
			this.Height = height;
		}

		internal override bool Pack(out string buffer)
		{
			base.Pack(out buffer);
			buffer = buffer + "&url=" + ApolloStringParser.ReplaceApolloString(this.Url);
			buffer = buffer + "&width=" + this.Width;
			buffer = buffer + "&height=" + this.Height;
			return true;
		}
	}
}

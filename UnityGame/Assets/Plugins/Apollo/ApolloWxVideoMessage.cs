using System;

namespace Apollo
{
	public class ApolloWxVideoMessage : ApolloWxMessageInfo
	{
		public string PicUrl;

		public int Width;

		public int Height;

		public string MediaUrl;

		public ApolloWxVideoMessage(ApolloWxMessageType type, string picUrl, int width, int height, string mediaUrl) : base(type)
		{
			this.PicUrl = picUrl;
			this.Width = width;
			this.Height = height;
			this.MediaUrl = mediaUrl;
		}

		internal override bool Pack(out string buffer)
		{
			base.Pack(out buffer);
			buffer = buffer + "&picUrl=" + ApolloStringParser.ReplaceApolloString(this.PicUrl);
			buffer = buffer + "&width=" + this.Width;
			buffer = buffer + "&height=" + this.Height;
			buffer = buffer + "&mediaUrl=" + ApolloStringParser.ReplaceApolloString(this.MediaUrl);
			return true;
		}
	}
}

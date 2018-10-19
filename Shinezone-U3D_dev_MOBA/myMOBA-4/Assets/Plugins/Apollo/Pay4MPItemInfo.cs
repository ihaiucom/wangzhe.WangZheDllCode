using System;

namespace Apollo
{
	public class Pay4MPItemInfo : PayInfo
	{
		public string reqType;

		public Pay4MPItemInfo()
		{
			this.Name = 1;
			base.Action = 8;
			this.reqType = "mp";
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			base.WriteTo(writer);
			writer.Write(this.reqType);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			base.ReadFrom(reader);
			reader.Read(ref this.reqType);
		}
	}
}

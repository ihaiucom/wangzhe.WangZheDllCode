using System;

namespace Apollo
{
	public class ApolloGroupResult : ApolloBufferBase
	{
		public ApolloResult result;

		public int errorCode;

		public int platform;

		public string desc;

		public ApolloQQGroupInfo mQQGroupInfo;

		public ApolloWXGroupInfo mWXGroupInfo;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.result);
			writer.Write(this.errorCode);
			writer.Write(this.platform);
			writer.Write(this.desc);
			writer.Write(this.mQQGroupInfo);
			writer.Write(this.mWXGroupInfo);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read<ApolloResult>(ref this.result);
			reader.Read(ref this.errorCode);
			reader.Read(ref this.platform);
			reader.Read(ref this.desc);
			reader.Read<ApolloQQGroupInfo>(ref this.mQQGroupInfo);
			reader.Read<ApolloWXGroupInfo>(ref this.mWXGroupInfo);
		}
	}
}

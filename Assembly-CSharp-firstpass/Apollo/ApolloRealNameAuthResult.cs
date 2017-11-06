using System;

namespace Apollo
{
	public class ApolloRealNameAuthResult : ApolloBufferBase
	{
		public ApolloPlatform Platform;

		public ApolloResult Ret;

		public int ErrorCode;

		public string Desc;

		public ApolloRealNameAuthResult()
		{
			this.Platform = ApolloPlatform.None;
			this.Ret = ApolloResult.Unknown;
			this.ErrorCode = -1;
			this.Desc = string.Empty;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write((int)this.Platform);
			writer.Write((int)this.Ret);
			writer.Write(this.ErrorCode);
			writer.Write(this.Desc);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			int platform = 0;
			int ret = 0;
			reader.Read(ref platform);
			reader.Read(ref ret);
			reader.Read(ref this.ErrorCode);
			reader.Read(ref this.Desc);
			this.Platform = (ApolloPlatform)platform;
			this.Ret = (ApolloResult)ret;
		}
	}
}

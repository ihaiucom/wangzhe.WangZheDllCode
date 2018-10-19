using System;

namespace Apollo
{
	public class RegisterInfo : ApolloBufferBase
	{
		[Obsolete("Deprecated since 1.1.12")]
		public string offerId;

		public string environment;

		[Obsolete("Deprecated since 1.1.12")]
		public string propUnit;

		public byte enableLog;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.offerId);
			writer.Write(this.environment);
			writer.Write(this.propUnit);
			writer.Write(this.enableLog);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.offerId);
			reader.Read(ref this.environment);
			reader.Read(ref this.propUnit);
			reader.Read(ref this.enableLog);
		}
	}
}

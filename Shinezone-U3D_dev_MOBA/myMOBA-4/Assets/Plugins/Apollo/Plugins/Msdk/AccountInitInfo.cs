using System;

namespace Apollo.Plugins.Msdk
{
	public class AccountInitInfo : ApolloBufferBase
	{
		public uint Permission;

		public AccountInitInfo(uint permission)
		{
			this.Permission = permission;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.Permission);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.Permission);
		}
	}
}

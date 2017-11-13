using System;

namespace Apollo
{
	public class CTdirUserRoleInfo : ApolloBufferBase
	{
		public int zoneID;

		public ulong roleID;

		public uint lastLoginTime;

		public string roleName;

		public string roleLevel;

		public uint appLen;

		public byte[] appBuff;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.zoneID);
			writer.Write(this.roleID);
			writer.Write(this.lastLoginTime);
			writer.Write(this.roleName);
			writer.Write(this.roleLevel);
			writer.Write(this.appLen);
			writer.Write(this.appBuff);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.zoneID);
			reader.Read(ref this.roleID);
			reader.Read(ref this.lastLoginTime);
			reader.Read(ref this.roleName);
			reader.Read(ref this.roleLevel);
			reader.Read(ref this.appLen);
			reader.Read(ref this.appBuff);
		}
	}
}

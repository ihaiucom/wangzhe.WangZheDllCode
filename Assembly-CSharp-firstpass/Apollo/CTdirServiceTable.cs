using System;

namespace Apollo
{
	public class CTdirServiceTable : ApolloBufferBase
	{
		public uint updateTime;

		public uint bitMap;

		public uint userAttr;

		public int zoneID;

		public uint appLen;

		public byte[] appBuff;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.updateTime);
			writer.Write(this.bitMap);
			writer.Write(this.userAttr);
			writer.Write(this.zoneID);
			writer.Write(this.appLen);
			writer.Write(this.appBuff);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.updateTime);
			reader.Read(ref this.bitMap);
			reader.Read(ref this.userAttr);
			reader.Read(ref this.zoneID);
			reader.Read(ref this.appLen);
			reader.Read(ref this.appBuff);
		}
	}
}

using System;

namespace Apollo
{
	public class ApolloQQGroupInfo : ApolloBufferBase
	{
		public string groupName;

		public string fingerMemo;

		public string memberNum;

		public string maxNum;

		public string ownerOpenid;

		public string unionid;

		public string zoneid;

		public string adminOpenids;

		public string groupOpenid;

		public string groupKey;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.groupName);
			writer.Write(this.fingerMemo);
			writer.Write(this.memberNum);
			writer.Write(this.maxNum);
			writer.Write(this.ownerOpenid);
			writer.Write(this.unionid);
			writer.Write(this.zoneid);
			writer.Write(this.adminOpenids);
			writer.Write(this.groupOpenid);
			writer.Write(this.groupKey);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.groupName);
			reader.Read(ref this.fingerMemo);
			reader.Read(ref this.memberNum);
			reader.Read(ref this.maxNum);
			reader.Read(ref this.ownerOpenid);
			reader.Read(ref this.unionid);
			reader.Read(ref this.zoneid);
			reader.Read(ref this.adminOpenids);
			reader.Read(ref this.groupOpenid);
			reader.Read(ref this.groupKey);
		}
	}
}

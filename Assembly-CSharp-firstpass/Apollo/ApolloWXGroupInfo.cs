using System;

namespace Apollo
{
	public class ApolloWXGroupInfo : ApolloBufferBase
	{
		public string openIdList;

		public string memberNum;

		public string chatRoomURL;

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.openIdList);
			writer.Write(this.memberNum);
			writer.Write(this.chatRoomURL);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.openIdList);
			reader.Read(ref this.memberNum);
			reader.Read(ref this.chatRoomURL);
		}
	}
}

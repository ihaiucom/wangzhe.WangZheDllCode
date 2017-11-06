using System;

namespace Apollo
{
	public class ApolloRealNameAuthInfo : ApolloBufferBase
	{
		public int ProvinceID;

		public ApolloIDType IdType;

		public string IdentityNum;

		public string Name;

		public string City;

		public ApolloRealNameAuthInfo()
		{
			this.ProvinceID = 0;
			this.IdType = ApolloIDType.IDCards;
			this.IdentityNum = string.Empty;
			this.Name = string.Empty;
			this.City = string.Empty;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write(this.ProvinceID);
			writer.Write((int)this.IdType);
			writer.Write(this.IdentityNum);
			writer.Write(this.Name);
			writer.Write(this.City);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			reader.Read(ref this.ProvinceID);
			reader.Read<ApolloIDType>(ref this.IdType);
			reader.Read(ref this.IdentityNum);
			reader.Read(ref this.Name);
			reader.Read(ref this.City);
		}
	}
}

using System;

namespace Apollo
{
	public class ApolloZoneRouteInfo : ApolloRouteInfoBase
	{
		public uint TypeId;

		public uint ZoneId;

		public ApolloZoneRouteInfo() : base(ApolloRouteType.Zone)
		{
			this.TypeId = 0u;
			this.ZoneId = 0u;
		}

		public ApolloZoneRouteInfo(uint typeId, uint zoneId) : base(ApolloRouteType.Zone)
		{
			this.TypeId = typeId;
			this.ZoneId = zoneId;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			base.WriteTo(writer);
			writer.Write(this.TypeId);
			writer.Write(this.ZoneId);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			base.ReadFrom(reader);
			reader.Read(ref this.TypeId);
			reader.Read(ref this.ZoneId);
		}

		protected override ApolloRouteInfoBase onCopyInstance()
		{
			return new ApolloZoneRouteInfo
			{
				TypeId = this.TypeId,
				ZoneId = this.ZoneId
			};
		}
	}
}

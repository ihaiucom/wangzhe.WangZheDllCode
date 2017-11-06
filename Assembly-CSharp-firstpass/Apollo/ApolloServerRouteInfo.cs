using System;

namespace Apollo
{
	public class ApolloServerRouteInfo : ApolloRouteInfoBase
	{
		public ulong ServerId;

		public ApolloServerRouteInfo() : base(ApolloRouteType.Server)
		{
			this.ServerId = 0uL;
		}

		public ApolloServerRouteInfo(ulong serverId) : base(ApolloRouteType.Server)
		{
			this.ServerId = serverId;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			base.WriteTo(writer);
			writer.Write(this.ServerId);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			base.ReadFrom(reader);
			reader.Read(ref this.ServerId);
		}

		protected override ApolloRouteInfoBase onCopyInstance()
		{
			return new ApolloServerRouteInfo
			{
				ServerId = this.ServerId
			};
		}

		public ApolloServerRouteInfo FromString(string data)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(data);
			this.ServerId = (ulong)apolloStringParser.GetUInt32("ServerId");
			return this;
		}
	}
}

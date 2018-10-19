using System;

namespace Apollo
{
	public abstract class ApolloRouteInfoBase : ApolloBufferBase
	{
		public ApolloRouteType RouteType
		{
			get;
			protected set;
		}

		protected ApolloRouteInfoBase(ApolloRouteType routeType)
		{
			this.RouteType = routeType;
		}

		public ApolloRouteInfoBase CopyInstance()
		{
			ApolloRouteInfoBase apolloRouteInfoBase = this.onCopyInstance();
			if (apolloRouteInfoBase != null)
			{
				apolloRouteInfoBase.RouteType = this.RouteType;
			}
			return apolloRouteInfoBase;
		}

		public override void WriteTo(ApolloBufferWriter writer)
		{
			writer.Write((int)this.RouteType);
		}

		public override void ReadFrom(ApolloBufferReader reader)
		{
			int routeType = 0;
			reader.Read(ref routeType);
			this.RouteType = (ApolloRouteType)routeType;
		}

		protected abstract ApolloRouteInfoBase onCopyInstance();
	}
}

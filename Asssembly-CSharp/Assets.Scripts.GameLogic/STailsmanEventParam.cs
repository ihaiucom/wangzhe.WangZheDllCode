using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct STailsmanEventParam
	{
		public PoolObjHandle<CTailsman> tailsman;

		public PoolObjHandle<ActorRoot> user;

		public STailsmanEventParam(PoolObjHandle<CTailsman> inTailsman, PoolObjHandle<ActorRoot> inUser)
		{
			this.tailsman = inTailsman;
			this.user = inUser;
		}
	}
}

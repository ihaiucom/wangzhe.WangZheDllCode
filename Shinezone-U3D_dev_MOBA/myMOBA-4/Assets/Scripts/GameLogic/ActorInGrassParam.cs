using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct ActorInGrassParam
	{
		public PoolObjHandle<ActorRoot> _src;

		public bool _bInGrass;

		public ActorInGrassParam(PoolObjHandle<ActorRoot> src, bool bInGrass)
		{
			this._src = src;
			this._bInGrass = bInGrass;
		}
	}
}

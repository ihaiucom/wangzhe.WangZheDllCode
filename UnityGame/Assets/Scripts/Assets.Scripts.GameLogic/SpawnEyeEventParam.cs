using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct SpawnEyeEventParam
	{
		public PoolObjHandle<ActorRoot> src;

		public VInt3 pos;

		public SpawnEyeEventParam(PoolObjHandle<ActorRoot> _src, VInt3 _pos)
		{
			this.src = _src;
			this.pos = _pos;
		}
	}
}

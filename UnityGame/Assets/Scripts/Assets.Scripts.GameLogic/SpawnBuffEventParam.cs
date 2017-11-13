using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct SpawnBuffEventParam
	{
		public uint showType;

		public uint floatTextID;

		public PoolObjHandle<ActorRoot> src;

		public SpawnBuffEventParam(uint _showType, uint _floatTextID, PoolObjHandle<ActorRoot> _src)
		{
			this.showType = _showType;
			this.floatTextID = _floatTextID;
			this.src = _src;
		}
	}
}

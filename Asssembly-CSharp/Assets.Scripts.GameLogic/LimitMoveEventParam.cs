using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct LimitMoveEventParam
	{
		public int totalTime;

		public int combineID;

		public PoolObjHandle<ActorRoot> src;

		public LimitMoveEventParam(int _time, int _combineID, PoolObjHandle<ActorRoot> _src)
		{
			this.totalTime = _time;
			this.combineID = _combineID;
			this.src = _src;
		}
	}
}

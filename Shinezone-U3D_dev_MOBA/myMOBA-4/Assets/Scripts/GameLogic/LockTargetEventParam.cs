using System;

namespace Assets.Scripts.GameLogic
{
	public struct LockTargetEventParam
	{
		public uint lockTargetID;

		public LockTargetEventParam(uint _targetID)
		{
			this.lockTargetID = _targetID;
		}
	}
}

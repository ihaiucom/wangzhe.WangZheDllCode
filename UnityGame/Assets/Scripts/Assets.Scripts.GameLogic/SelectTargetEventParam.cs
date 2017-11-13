using System;

namespace Assets.Scripts.GameLogic
{
	public struct SelectTargetEventParam
	{
		public uint commonAttackTargetID;

		public SelectTargetEventParam(uint _targetID)
		{
			this.commonAttackTargetID = _targetID;
		}
	}
}

using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct SkillChooseTargetEventParam
	{
		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public int iTargetCount;

		public SkillChooseTargetEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, int _iTargetCount)
		{
			this.src = _src;
			this.atker = _atker;
			this.iTargetCount = _iTargetCount;
		}
	}
}

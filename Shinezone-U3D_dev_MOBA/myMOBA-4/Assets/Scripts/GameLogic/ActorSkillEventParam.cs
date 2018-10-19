using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct ActorSkillEventParam
	{
		public SkillSlotType slot;

		public PoolObjHandle<ActorRoot> src;

		public ActorSkillEventParam(PoolObjHandle<ActorRoot> _src, SkillSlotType _slot = SkillSlotType.SLOT_SKILL_0)
		{
			this.src = _src;
			this.slot = _slot;
		}
	}
}

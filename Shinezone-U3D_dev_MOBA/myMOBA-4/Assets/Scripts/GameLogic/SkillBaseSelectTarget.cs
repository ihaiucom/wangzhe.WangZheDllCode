using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class SkillBaseSelectTarget
	{
		public virtual ActorRoot SelectTarget(SkillSlot UseSlot)
		{
			return null;
		}

		public virtual VInt3 SelectTargetDir(SkillSlot UseSlot)
		{
			return UseSlot.Actor.handle.forward;
		}
	}
}

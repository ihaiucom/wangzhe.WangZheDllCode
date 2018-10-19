using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseSelectTarget(SkillTargetRule.LowerHpEnermy)]
	public class SkillSelectLowerHpEnemy : SkillBaseSelectTarget
	{
		public override ActorRoot SelectTarget(SkillSlot UseSlot)
		{
			return SelectTargetHelper.GetTarget(UseSlot);
		}

		public override VInt3 SelectTargetDir(SkillSlot UseSlot)
		{
			ActorRoot target = SelectTargetHelper.GetTarget(UseSlot);
			if (target != null)
			{
				VInt3 vInt = target.location - UseSlot.Actor.handle.location;
				vInt.y = 0;
				return vInt.NormalizeTo(1000);
			}
			return UseSlot.Actor.handle.forward;
		}
	}
}

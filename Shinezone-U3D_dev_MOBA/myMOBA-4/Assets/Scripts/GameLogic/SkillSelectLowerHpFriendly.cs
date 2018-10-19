using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseSelectTarget(SkillTargetRule.LowerHpFriendly)]
	public class SkillSelectLowerHpFriendly : SkillBaseSelectTarget
	{
		public override ActorRoot SelectTarget(SkillSlot UseSlot)
		{
			return Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(UseSlot.Actor.handle, UseSlot.SkillObj.GetMaxSearchDistance(UseSlot.GetSkillLevel()), TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, false, true);
		}

		public override VInt3 SelectTargetDir(SkillSlot UseSlot)
		{
			ActorRoot lowestHpTarget = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(UseSlot.Actor.handle, UseSlot.SkillObj.GetMaxSearchDistance(UseSlot.GetSkillLevel()), TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, false, true);
			if (lowestHpTarget != null)
			{
				VInt3 vInt = lowestHpTarget.location - UseSlot.Actor.handle.location;
				vInt.y = 0;
				return vInt.NormalizeTo(1000);
			}
			return UseSlot.Actor.handle.forward;
		}
	}
}

using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseDetection(SkillUseRule.FriendlyUse)]
	public class SkillSelectNearestFriendlyDetection : SkillBaseDetection
	{
		public override bool Detection(SkillSlot slot)
		{
			int srchR;
			if (slot.SkillObj.AppointType == SkillRangeAppointType.Target)
			{
				srchR = slot.SkillObj.GetMaxSearchDistance(slot.GetSkillLevel());
			}
			else
			{
				srchR = slot.SkillObj.cfgData.iMaxAttackDistance;
			}
			return Singleton<TargetSearcher>.GetInstance().GetNearestFriendly(slot.Actor.handle, srchR, slot.SkillObj.cfgData.dwSkillTargetFilter) != null;
		}
	}
}

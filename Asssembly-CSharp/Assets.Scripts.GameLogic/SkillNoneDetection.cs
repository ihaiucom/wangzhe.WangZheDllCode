using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseDetection(SkillUseRule.AnyUse)]
	public class SkillNoneDetection : SkillBaseDetection
	{
		public override bool Detection(SkillSlot slot)
		{
			return true;
		}
	}
}

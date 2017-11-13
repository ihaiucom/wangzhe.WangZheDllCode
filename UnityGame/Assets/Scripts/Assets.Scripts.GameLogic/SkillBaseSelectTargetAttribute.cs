using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillBaseSelectTargetAttribute : Attribute
	{
		public SkillTargetRule TargetRule
		{
			get;
			set;
		}

		public SkillBaseSelectTargetAttribute(SkillTargetRule _rule)
		{
			this.TargetRule = _rule;
		}
	}
}

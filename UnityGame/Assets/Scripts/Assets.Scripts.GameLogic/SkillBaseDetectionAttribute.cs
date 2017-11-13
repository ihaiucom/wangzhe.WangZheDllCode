using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillBaseDetectionAttribute : Attribute
	{
		public SkillUseRule UseRule
		{
			get;
			set;
		}

		public SkillBaseDetectionAttribute(SkillUseRule _rule)
		{
			this.UseRule = _rule;
		}
	}
}

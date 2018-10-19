using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class SkillBaseDetection
	{
		public virtual bool Detection(SkillSlot slot)
		{
			return true;
		}
	}
}

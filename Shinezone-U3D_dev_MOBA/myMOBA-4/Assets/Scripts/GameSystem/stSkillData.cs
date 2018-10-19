using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct stSkillData
	{
		public int slotId;

		public int skillId;

		public int level;

		public string descTip;

		public string levelUpTip;

		public ResSkillCfgInfo cfgInfo;

		public ResSkillLvlUpInfo cfgLevelUpInfo;
	}
}

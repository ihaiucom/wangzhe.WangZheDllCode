using System;

namespace Assets.Scripts.GameLogic
{
	public class GET_SOUL_EXP_STATISTIC_INFO
	{
		public int iKillSoldierExpMax;

		public int iKillHeroExpMax;

		public int iKillOrganExpMax;

		public int iKillMonsterExpMax;

		public int iAddExpTotal;

		public GET_SOUL_EXP_STATISTIC_INFO()
		{
			this.iKillSoldierExpMax = 0;
			this.iKillHeroExpMax = 0;
			this.iKillOrganExpMax = 0;
			this.iKillMonsterExpMax = 0;
			this.iAddExpTotal = 0;
		}
	}
}

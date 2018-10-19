using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CTrophyRewardInfoSort : IComparer<CTrophyRewardInfo>
	{
		public int Compare(CTrophyRewardInfo l, CTrophyRewardInfo r)
		{
			bool flag = l.HasGotAward();
			bool flag2 = r.HasGotAward();
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return -1;
			}
			if (l.Cfg.dwTrophyLvl < r.Cfg.dwTrophyLvl)
			{
				return -1;
			}
			return 1;
		}
	}
}

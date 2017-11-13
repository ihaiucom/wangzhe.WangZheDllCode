using Assets.Scripts.GameLogic;
using System;

[CheatCommandEntry("关卡")]
internal class CheatCommandTrainLevelSelectHero
{
	[CheatCommandEntryMethod("切换训练关卡可用选择英雄", true, false)]
	public static string SwitchTrainLevelSelectHero()
	{
		CBattleGuideManager.CanSelectHeroOnTrainLevelEntry = !CBattleGuideManager.CanSelectHeroOnTrainLevelEntry;
		if (CBattleGuideManager.CanSelectHeroOnTrainLevelEntry)
		{
			return "done, can select hero!";
		}
		return "done, can not select hero!";
	}
}

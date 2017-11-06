using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using ResData;
using System;

public class MonsterDataHelper
{
	public static ResMonsterCfgInfo GetDataCfgInfo(int configID, int diffLevel)
	{
		if (diffLevel == 0)
		{
			diffLevel = Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_levelDifficulty;
		}
		ulong num = Convert.ToUInt64(configID);
		num <<= 32;
		long key = (long)(num + (ulong)((long)diffLevel));
		return GameDataMgr.monsterDatabin.GetDataByKey(key);
	}

	public static ResMonsterCfgInfo GetDataCfgInfoByCurLevelDiff(int configID)
	{
		SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
		return MonsterDataHelper.GetDataCfgInfo(configID, curLvelContext.m_levelDifficulty);
	}
}

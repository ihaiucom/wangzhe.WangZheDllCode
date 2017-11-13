using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using ResData;
using System;

public class OrganDataHelper
{
	public static ResOrganCfgInfo GetDataCfgInfo(int configID, int diffLevel)
	{
		ulong num = Convert.ToUInt64(configID);
		num <<= 32;
		long key = (long)(num + (ulong)((long)diffLevel));
		return GameDataMgr.organDatabin.GetDataByKey(key);
	}

	public static ResOrganCfgInfo GetDataCfgInfoByCurLevelDiff(int configID)
	{
		SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
		return OrganDataHelper.GetDataCfgInfo(configID, curLvelContext.m_levelDifficulty);
	}
}

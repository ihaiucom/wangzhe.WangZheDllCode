using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

[CheatCommand("英雄/历史战绩/增加历史战绩记录：", "增加一条历史战绩记录", 76)]
internal class SetPvpHisotryCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stAddFightHistory = new CSDT_CHEAT_ADD_FIGHTHISTORY();
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		CheatCmdRef.stAddFightHistory.stFightRecord.bGameType = 6;
		CheatCmdRef.stAddFightHistory.stFightRecord.bPlayerCnt = 4;
		CheatCmdRef.stAddFightHistory.stFightRecord.bWinCamp = 1;
		CheatCmdRef.stAddFightHistory.stFightRecord.dwGameStartTime = (uint)CRoleInfo.GetCurrentUTCTime();
		CheatCmdRef.stAddFightHistory.stFightRecord.dwGameTime = 1000u;
		for (int i = 0; i < 4; i++)
		{
			CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].bHeroLv = 1;
			CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].bPlayerLv = 1;
			CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].bPlayerCamp = ((i < 2) ? 1 : 2);
			int id = Random.Range(0, GameDataMgr.robotRookieHeroSkinDatabin.Count());
			ResFakeAcntHero dataByIndex = GameDataMgr.robotRookieHeroSkinDatabin.GetDataByIndex(id);
			CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].dwHeroID = dataByIndex.dwHeroID;
			StringHelper.StringToUTF8Bytes(i.ToString(), ref CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].szPlayerName);
			if (i == 0)
			{
				CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].ullPlayerUid = masterRoleInfo.playerUllUID;
				CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].iPlayerLogicWorldID = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
			}
			else
			{
				CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].ullPlayerUid = (ulong)((long)i);
				CheatCmdRef.stAddFightHistory.stFightRecord.astPlayerFightData[i].iPlayerLogicWorldID = i;
			}
		}
		Singleton<CPlayerPvpHistoryController>.GetInstance().AddSelfRecordData(CheatCmdRef.stAddFightHistory.stFightRecord);
		return CheatCommandBase.Done;
	}
}

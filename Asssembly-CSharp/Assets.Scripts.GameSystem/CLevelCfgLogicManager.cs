using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CLevelCfgLogicManager
	{
		public static ResDT_LevelCommonInfo GetPvpMapCommonInfo(byte mapType, uint mapId)
		{
			ResDT_LevelCommonInfo resDT_LevelCommonInfo = new ResDT_LevelCommonInfo();
			if (mapType == 1)
			{
				ResAcntBattleLevelInfo dataByKey = GameDataMgr.pvpLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey != null);
				resDT_LevelCommonInfo = dataByKey.stLevelCommonInfo;
			}
			else if (mapType == 3)
			{
				ResRankLevelInfo dataByKey2 = GameDataMgr.rankLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey2 != null);
				resDT_LevelCommonInfo = dataByKey2.stLevelCommonInfo;
			}
			else if (mapType == 4)
			{
				ResEntertainmentLevelInfo dataByKey3 = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey3 != null);
				resDT_LevelCommonInfo = dataByKey3.stLevelCommonInfo;
			}
			else if (mapType == 5)
			{
				ResRewardMatchLevelInfo dataByKey4 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey4 != null);
				resDT_LevelCommonInfo = dataByKey4.stLevelCommonInfo;
			}
			else if (mapType == 6)
			{
				ResGuildMatchLevelInfo dataByKey5 = GameDataMgr.guildMatchLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey5 != null);
				resDT_LevelCommonInfo = dataByKey5.stLevelCommonInfo;
			}
			else if (mapType == 2)
			{
				ResCounterPartLevelInfo dataByKey6 = GameDataMgr.cpLevelDatabin.GetDataByKey(mapId);
				DebugHelper.Assert(dataByKey6 != null);
				resDT_LevelCommonInfo = dataByKey6.stLevelCommonInfo;
			}
			if (resDT_LevelCommonInfo == null)
			{
				resDT_LevelCommonInfo = new ResDT_LevelCommonInfo();
			}
			return resDT_LevelCommonInfo;
		}

		public static SLevelContext CreatePvpLevelContext(byte mapType, uint mapID, COM_GAME_TYPE GameType, int difficult)
		{
			SLevelContext sLevelContext = new SLevelContext();
			sLevelContext.SetGameType(GameType);
			sLevelContext.m_mapType = (int)mapType;
			sLevelContext.m_levelDifficulty = difficult;
			sLevelContext.m_horizonEnableMethod = Horizon.EnableMethod.EnableAll;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(mapType, mapID);
			sLevelContext.InitPvpData(pvpMapCommonInfo, mapID);
			if (mapType == 4)
			{
				ResEntertainmentLevelInfo dataByKey = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapID);
				sLevelContext.m_entertainmentSubMapType = (int)dataByKey.bEntertainmentSubType;
			}
			else if (mapType == 5)
			{
				ResRewardMatchLevelInfo dataByKey2 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
				sLevelContext.m_SecondName = dataByKey2.szMatchName;
			}
			return sLevelContext;
		}

		public static ResLevelCfgInfo GetPveMapInfo(byte gameType, int levelID)
		{
			ResLevelCfgInfo resLevelCfgInfo = null;
			if (gameType == 2)
			{
				resLevelCfgInfo = GameDataMgr.levelDatabin.GetDataByKey((long)levelID);
			}
			else if (gameType == 0)
			{
				resLevelCfgInfo = GameDataMgr.levelDatabin.GetDataByKey((long)levelID);
			}
			else if (gameType == 7)
			{
				resLevelCfgInfo = GameDataMgr.burnMap.GetDataByKey((long)Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex));
			}
			else if (gameType == 8)
			{
				resLevelCfgInfo = GameDataMgr.arenaLevelDatabin.GetDataByKey((long)levelID);
			}
			else if (gameType == 3)
			{
			}
			if (resLevelCfgInfo == null)
			{
				if (gameType != 1)
				{
				}
				resLevelCfgInfo = new ResLevelCfgInfo();
			}
			return resLevelCfgInfo;
		}

		public static SLevelContext CreatePveLevelContext(SCPKG_STARTSINGLEGAMERSP InMessage)
		{
			SLevelContext sLevelContext = new SLevelContext();
			sLevelContext.SetGameType((COM_GAME_TYPE)InMessage.bGameType);
			ResLevelCfgInfo pveMapInfo = CLevelCfgLogicManager.GetPveMapInfo(InMessage.bGameType, InMessage.iLevelId);
			if (InMessage.bGameType == 2)
			{
				sLevelContext.InitPveData(pveMapInfo, 1);
				if (pveMapInfo.bGuideLevelSubType == 0)
				{
					sLevelContext.m_isMobaType = true;
				}
				else if (pveMapInfo.bGuideLevelSubType == 1)
				{
					sLevelContext.m_isMobaType = false;
				}
			}
			else if (InMessage.bGameType == 0)
			{
				sLevelContext.InitPveData(pveMapInfo, Singleton<CAdventureSys>.instance.currentDifficulty);
			}
			else if (InMessage.bGameType == 7)
			{
				sLevelContext.InitPveData(pveMapInfo, 1);
			}
			else if (InMessage.bGameType == 8)
			{
				sLevelContext.InitPveData(pveMapInfo, 1);
			}
			else if (InMessage.bGameType == 1)
			{
				byte bMapType = InMessage.stGameParam.stSingleGameRspOfCombat.bMapType;
				uint dwMapId = InMessage.stGameParam.stSingleGameRspOfCombat.dwMapId;
				sLevelContext = CLevelCfgLogicManager.CreatePvpLevelContext(bMapType, dwMapId, (COM_GAME_TYPE)InMessage.bGameType, 1);
				sLevelContext.m_isWarmBattle = Convert.ToBoolean(InMessage.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle);
				sLevelContext.SetWarmHeroAiDiff(InMessage.stGameParam.stSingleGameRspOfCombat.bAILevel);
			}
			else if (InMessage.bGameType == 3)
			{
			}
			return sLevelContext;
		}
	}
}

using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SLevelContext
	{
		private COM_GAME_TYPE m_gameType;

		public int m_mapID;

		public bool m_isMobaType;

		public enSelectType m_selectHeroType;

		public int m_levelDifficulty;

		public RES_LEVEL_HEROAITYPE m_heroAiType;

		public byte m_soulGrow;

		public int m_baseReviveTime = 15000;

		public uint m_dynamicPropertyConfig;

		public string m_miniMapPath;

		public string m_bigMapPath;

		public int m_mapWidth;

		public int m_mapHeight;

		public int m_bigMapWidth;

		public int m_bigMapHeight;

		public float m_mapFowScale;

		public float m_bigMapFowScale;

		public string m_levelName = string.Empty;

		public string m_levelDesignFileName = string.Empty;

		public string m_levelArtistFileName = string.Empty;

		public string m_musicStartEvent;

		public string m_musicEndEvent;

		public string m_ambientSoundEvent;

		public string m_musicBankResName;

		public Horizon.EnableMethod m_horizonEnableMethod;

		public uint m_soulID;

		public uint m_soulAllocId;

		public int m_extraSkillId;

		public int m_extraSkill2Id;

		public int m_extraPassiveSkillId;

		public int m_ornamentSkillId;

		public int m_ornamentSwitchCD;

		public bool m_bEnableFow;

		public bool m_bEnableShopHorizonTab;

		public bool m_bEnableOrnamentSlot;

		public int m_ornamentFirstSwitchCd;

		public int m_ornamentFirstSwitchCdEftTime;

		public bool m_isCanRightJoyStickCameraDrag;

		public bool m_isCameraFlip;

		public int m_fakeSightRange;

		private ResDT_LevelCommonInfo m_pvpLevelCommonInfo;

		public int m_mapType = -1;

		public int m_entertainmentSubMapType = -1;

		public ushort m_originalGoldCoinInBattle;

		public uint[] m_battleTaskOfCamps = new uint[3];

		public int m_pvpPlayerNum;

		public bool m_isBattleEquipLimit;

		public int m_headPtsUpperLimit;

		public int m_birthLevelConfig;

		public bool m_isShowHonor;

		public uint m_cooldownReduceUpperLimit;

		public byte m_isOpenExpCompensate;

		public ResDT_ExpCompensateInfo[] m_expCompensateInfo;

		public int m_soldierActivateDelay;

		public int m_soldierActivateCountDelay1;

		public int m_soldierActivateCountDelay2;

		public uint m_timeDuration;

		public uint m_addWinCondStarId;

		public uint m_addLoseCondStarId;

		public string m_SecondName = string.Empty;

		public string m_gameMatchName = string.Empty;

		public bool m_isWarmBattle;

		public ResBattleDynamicDifficulty m_warmHeroAiDiffInfo;

		public byte m_pauseTimes;

		public uint dwDeskId;

		public uint dwDeskSeq;

		private ResLevelCfgInfo m_pveLevelInfo;

		public RES_LEVEL_TYPE m_pveLevelType;

		public int m_chapterNo;

		public byte m_levelNo;

		public int m_passDialogId;

		public int m_preDialogId;

		public int m_failureDialogId;

		public bool m_isShowTrainingHelper;

		public bool canAutoAI;

		public byte m_reviveTimeMax;

		public int m_loseCondition;

		public ResDT_MapBuff[] m_mapBuffs;

		public ResDT_IntParamArrayNode[] m_starDetail = new ResDT_IntParamArrayNode[0];

		public ResDT_PveReviveInfo[] m_reviveInfo = new ResDT_PveReviveInfo[0];

		public CSDT_CAMP_EXT_INFO[] m_campExtInfos = new CSDT_CAMP_EXT_INFO[2];

		public void InitPveData(ResLevelCfgInfo levelCfg, int difficult)
		{
			this.m_isMobaType = false;
			this.m_pveLevelInfo = levelCfg;
			this.m_selectHeroType = enSelectType.enMutile;
			this.m_levelName = this.m_pveLevelInfo.szName;
			this.m_levelDesignFileName = this.m_pveLevelInfo.szDesignFileName;
			if (this.m_pveLevelInfo.szArtistFileName != null && this.m_pveLevelInfo.szArtistFileName.get_Length() > 0)
			{
				this.m_levelArtistFileName = this.m_pveLevelInfo.szArtistFileName;
			}
			this.m_mapWidth = this.m_pveLevelInfo.iMapWidth;
			this.m_mapHeight = this.m_pveLevelInfo.iMapHeight;
			this.m_bigMapWidth = this.m_pveLevelInfo.iBigMapWidth;
			this.m_bigMapHeight = this.m_pveLevelInfo.iBigMapHeight;
			this.m_miniMapPath = this.m_pveLevelInfo.szThumbnailPath;
			this.m_bigMapPath = this.m_pveLevelInfo.szBigMapPath;
			this.m_mapID = this.m_pveLevelInfo.iCfgID;
			this.m_chapterNo = this.m_pveLevelInfo.iChapterId;
			this.m_levelNo = this.m_pveLevelInfo.bLevelNo;
			this.m_levelDifficulty = difficult;
			this.m_pveLevelType = (RES_LEVEL_TYPE)this.m_pveLevelInfo.iLevelType;
			this.m_horizonEnableMethod = (Horizon.EnableMethod)this.m_pveLevelInfo.bEnableHorizon;
			this.m_isMobaType = (this.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP);
			if (this.m_isMobaType)
			{
				this.m_horizonEnableMethod = Horizon.EnableMethod.EnableAll;
			}
			this.m_passDialogId = this.m_pveLevelInfo.iPassDialogId;
			this.m_preDialogId = this.m_pveLevelInfo.iPreDialogId;
			this.m_failureDialogId = this.m_pveLevelInfo.iFailureDialogId;
			this.m_heroAiType = (RES_LEVEL_HEROAITYPE)this.m_pveLevelInfo.iHeroAIType;
			this.m_soulGrow = this.m_pveLevelInfo.bSoulGrow;
			this.m_baseReviveTime = (int)this.m_pveLevelInfo.dwReviveTime;
			this.m_dynamicPropertyConfig = this.m_pveLevelInfo.dwDynamicPropertyCfg;
			this.m_miniMapPath = this.m_pveLevelInfo.szThumbnailPath;
			this.m_bigMapPath = this.m_pveLevelInfo.szBigMapPath;
			this.m_mapWidth = this.m_pveLevelInfo.iMapWidth;
			this.m_mapHeight = this.m_pveLevelInfo.iMapHeight;
			this.m_mapFowScale = 1f;
			this.m_bigMapFowScale = 1f;
			this.m_musicStartEvent = this.m_pveLevelInfo.szMusicStartEvent;
			this.m_musicEndEvent = this.m_pveLevelInfo.szMusicEndEvent;
			this.m_ambientSoundEvent = this.m_pveLevelInfo.szAmbientSoundEvent;
			this.m_musicBankResName = this.m_pveLevelInfo.szBankResourceName;
			this.canAutoAI = (this.m_pveLevelInfo.bIsOpenAutoAI != 0);
			this.m_mapBuffs = this.m_pveLevelInfo.astMapBuffs;
			this.m_isShowTrainingHelper = true;
			this.m_soulID = this.m_pveLevelInfo.dwSoulID;
			this.m_soulAllocId = this.m_pveLevelInfo.dwSoulAllocId;
			this.m_starDetail = this.m_pveLevelInfo.astStarDetail;
			this.m_reviveInfo = this.m_pveLevelInfo.astReviveInfo;
			this.m_reviveTimeMax = this.m_pveLevelInfo.bReviveTimeMax;
			this.m_loseCondition = this.m_pveLevelInfo.iLoseCondition;
			this.m_extraSkillId = this.m_pveLevelInfo.iExtraSkillId;
			this.m_extraSkill2Id = this.m_pveLevelInfo.iExtraSkill2Id;
			this.m_extraPassiveSkillId = this.m_pveLevelInfo.iExtraPassiveSkillId;
			this.m_isCanRightJoyStickCameraDrag = (this.m_pveLevelInfo.bSupportCameraDrag > 0);
			this.m_isCameraFlip = false;
			this.m_pvpPlayerNum = (int)this.m_pveLevelInfo.bMaxAcntNum;
		}

		public void InitPvpData(ResDT_LevelCommonInfo levelCommonInfo, uint mapID)
		{
			this.m_isMobaType = true;
			this.m_pvpLevelCommonInfo = levelCommonInfo;
			this.m_mapID = (int)mapID;
			this.m_selectHeroType = (enSelectType)this.m_pvpLevelCommonInfo.stPickRuleInfo.bPickType;
			this.m_levelName = this.m_pvpLevelCommonInfo.szName;
			this.m_levelDesignFileName = this.m_pvpLevelCommonInfo.szDesignFileName;
			if (this.m_pvpLevelCommonInfo.szArtistFileName != null)
			{
				this.m_levelArtistFileName = this.m_pvpLevelCommonInfo.szArtistFileName;
			}
			this.m_mapWidth = this.m_pvpLevelCommonInfo.iMapWidth;
			this.m_mapHeight = this.m_pvpLevelCommonInfo.iMapHeight;
			this.m_bigMapWidth = this.m_pvpLevelCommonInfo.iBigMapWidth;
			this.m_bigMapHeight = this.m_pvpLevelCommonInfo.iBigMapHeight;
			this.m_miniMapPath = this.m_pvpLevelCommonInfo.szThumbnailPath;
			this.m_bigMapPath = this.m_pvpLevelCommonInfo.szBigMapPath;
			this.m_mapFowScale = this.m_pvpLevelCommonInfo.fMapFowScale;
			this.m_bigMapFowScale = this.m_pvpLevelCommonInfo.fBigMapFowScale;
			this.m_heroAiType = (RES_LEVEL_HEROAITYPE)this.m_pvpLevelCommonInfo.iHeroAIType;
			this.m_pvpPlayerNum = (int)this.m_pvpLevelCommonInfo.bMaxAcntNum;
			this.m_isBattleEquipLimit = (this.m_pvpLevelCommonInfo.bBattleEquipLimit > 0);
			this.m_headPtsUpperLimit = (int)this.m_pvpLevelCommonInfo.bHeadPtsUpperLimit;
			this.m_birthLevelConfig = (int)this.m_pvpLevelCommonInfo.bBirthLevelConfig;
			this.m_isShowHonor = (this.m_pvpLevelCommonInfo.bShowHonor > 0);
			this.m_cooldownReduceUpperLimit = this.m_pvpLevelCommonInfo.dwCooldownReduceUpperLimit;
			this.m_dynamicPropertyConfig = this.m_pvpLevelCommonInfo.dwDynamicPropertyCfg;
			this.m_originalGoldCoinInBattle = this.m_pvpLevelCommonInfo.wOriginalGoldCoinInBattle;
			this.m_battleTaskOfCamps[1] = this.m_pvpLevelCommonInfo.dwBattleTaskOfCamp1;
			this.m_battleTaskOfCamps[2] = this.m_pvpLevelCommonInfo.dwBattleTaskOfCamp2;
			this.m_musicStartEvent = this.m_pvpLevelCommonInfo.szMusicStartEvent;
			this.m_musicEndEvent = this.m_pvpLevelCommonInfo.szMusicEndEvent;
			this.m_musicBankResName = this.m_pvpLevelCommonInfo.szBankResourceName;
			this.m_ambientSoundEvent = this.m_pvpLevelCommonInfo.szAmbientSoundEvent;
			this.m_isOpenExpCompensate = this.m_pvpLevelCommonInfo.bIsOpenExpCompensate;
			this.m_expCompensateInfo = this.m_pvpLevelCommonInfo.astExpCompensateDetail;
			this.m_soldierActivateDelay = this.m_pvpLevelCommonInfo.iSoldierActivateDelay;
			this.m_soldierActivateCountDelay1 = this.m_pvpLevelCommonInfo.iSoldierActivateCountDelay1;
			this.m_soldierActivateCountDelay2 = this.m_pvpLevelCommonInfo.iSoldierActivateCountDelay2;
			this.m_timeDuration = this.m_pvpLevelCommonInfo.dwTimeDuration;
			this.m_addWinCondStarId = this.m_pvpLevelCommonInfo.dwAddWinCondStarId;
			this.m_addLoseCondStarId = this.m_pvpLevelCommonInfo.dwAddLoseCondStarId;
			this.m_soulID = this.m_pvpLevelCommonInfo.dwSoulID;
			this.m_soulAllocId = this.m_pvpLevelCommonInfo.dwSoulAllocId;
			this.m_extraSkillId = this.m_pvpLevelCommonInfo.iExtraSkillId;
			this.m_extraSkill2Id = this.m_pvpLevelCommonInfo.iExtraSkill2Id;
			this.m_extraPassiveSkillId = this.m_pvpLevelCommonInfo.iExtraPassiveSkillId;
			this.m_ornamentSkillId = this.m_pvpLevelCommonInfo.iOrnamentSkillId;
			this.m_ornamentSwitchCD = this.m_pvpLevelCommonInfo.iOrnamentSwitchCD;
			this.m_bEnableFow = (this.m_pvpLevelCommonInfo.bIsEnableFow > 0);
			this.m_bEnableOrnamentSlot = (this.m_pvpLevelCommonInfo.bIsEnableOrnamentSlot > 0);
			this.m_bEnableShopHorizonTab = (this.m_pvpLevelCommonInfo.bIsEnableShopHorizonTab > 0);
			this.m_ornamentFirstSwitchCd = this.m_pvpLevelCommonInfo.iOrnamentFirstSwitchCD;
			this.m_ornamentFirstSwitchCdEftTime = this.m_pvpLevelCommonInfo.iOrnamentFirstSwitchCDEftTime;
			this.m_isCanRightJoyStickCameraDrag = (this.m_pvpLevelCommonInfo.bSupportCameraDrag > 0);
			this.m_gameMatchName = this.m_pvpLevelCommonInfo.szGameMatchName;
			this.m_pauseTimes = this.m_pvpLevelCommonInfo.bPauseNum;
			this.m_isCameraFlip = (this.m_pvpLevelCommonInfo.bCameraFlip > 0);
			this.m_fakeSightRange = this.m_pvpLevelCommonInfo.iFakeSightRange;
		}

		public void SetGameType(COM_GAME_TYPE gType)
		{
			this.m_gameType = gType;
		}

		public COM_GAME_TYPE GetGameType()
		{
			return this.m_gameType;
		}

		public enSelectType GetSelectHeroType()
		{
			return this.m_selectHeroType;
		}

		public bool IsMobaMode()
		{
			return this.m_isMobaType;
		}

		public bool IsMobaModeWithOutGuide()
		{
			return this.m_isMobaType && !this.IsGameTypeGuide();
		}

		public bool IsRedBagMode()
		{
			if (this.m_pvpPlayerNum != 10)
			{
				return false;
			}
			bool result = false;
			if (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER || this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH || this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)
			{
				if (this.m_isWarmBattle)
				{
					result = true;
				}
				else
				{
					bool flag = Singleton<GamePlayerCenter>.GetInstance().IsHostPlayerHasCpuEnemy();
					result = !flag;
				}
			}
			return result;
		}

		public bool IsMultilModeWithWarmBattle()
		{
			return this.m_isWarmBattle || this.IsMultilModeWithoutWarmBattle();
		}

		public bool IsMultilModeWithoutWarmBattle()
		{
			return this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE && this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT && this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE && this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY && this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING && this.m_gameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA;
		}

		public bool IsLuanDouPlayMode()
		{
			return this.m_mapType == 4 && this.m_entertainmentSubMapType == 1;
		}

		public bool IsFireHolePlayMode()
		{
			return this.m_mapType == 4 && this.m_entertainmentSubMapType == 2;
		}

		public bool IsNorma5v5PlayMode()
		{
			return this.m_mapType == 1 && this.m_pvpPlayerNum == 10;
		}

		public bool IsGameTypeGuide()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE;
		}

		public bool IsGameTypeAdventure()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE;
		}

		public bool IsGameTypeActivity()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY;
		}

		public bool IsGameTypeBurning()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING;
		}

		public bool IsGameTypeArena()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA;
		}

		public bool IsGameTypeComBat()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT;
		}

		public bool IsGameTypePvpRoom()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM;
		}

		public bool IsGameTypePvpMatch()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH;
		}

		public bool IsGameTypeEntertainment()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT;
		}

		public bool IsGameTypeLadder()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER;
		}

		public bool IsGameTypeRewardMatch()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_REWARDMATCH;
		}

		public bool IsGameTypeGuildMatch()
		{
			return this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_GUILDMATCH;
		}

		public virtual bool IsSoulGrow()
		{
			return this.m_isMobaType || this.m_soulGrow > 0;
		}

		public bool IsGuildProfitGameType()
		{
			return this.IsGameTypeLadder() || this.IsGameTypePvpMatch() || this.IsGameTypeRewardMatch() || this.IsGameTypeEntertainment() || this.IsGameTypeGuildMatch();
		}

		public void SetWarmHeroAiDiff(byte aiLevel)
		{
			this.m_warmHeroAiDiffInfo = GameDataMgr.battleDynamicDifficultyDB.GetDataByKey((uint)aiLevel);
		}

		public void SetCampExtInfo(CSDT_CAMPINFO[] campInfos)
		{
			if (campInfos != null && campInfos.Length == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					this.m_campExtInfos[i] = campInfos[i].stExtInfo;
				}
			}
		}

		public CSDT_CAMP_EXT_GUILDMATCH[] GetCampExtGuildMatchInfo()
		{
			CSDT_CAMP_EXT_GUILDMATCH[] array = new CSDT_CAMP_EXT_GUILDMATCH[2];
			for (int i = 0; i < 2; i++)
			{
				if (this.m_campExtInfos[i].bExtType != 11)
				{
					return null;
				}
				array[i] = this.m_campExtInfos[i].stExtData.stGuildMatch;
			}
			return array;
		}

		public static void SetMasterPvpDetailWhenGameSettle(COMDT_GAME_INFO gameInfo)
		{
			byte bGameType = gameInfo.bGameType;
			byte bMapType = gameInfo.bMapType;
			uint iLevelID = (uint)gameInfo.iLevelID;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "masterRoleInfo is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			switch (bGameType)
			{
			case 1:
				Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stVsMachineInfo, (int)gameInfo.bGameResult);
				return;
			case 2:
			case 3:
			case 6:
			case 7:
			case 8:
				return;
			case 4:
				Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stLadderInfo, (int)gameInfo.bGameResult);
				Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
				return;
			case 5:
			case 10:
				if (gameInfo.bIsPKAI != 2)
				{
					byte bMaxAcntNum = CLevelCfgLogicManager.GetPvpMapCommonInfo(bMapType, iLevelID).bMaxAcntNum;
					switch (bMaxAcntNum)
					{
					case 2:
						Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stOneVsOneInfo, (int)gameInfo.bGameResult);
						goto IL_1D6;
					case 4:
						Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stTwoVsTwoInfo, (int)gameInfo.bGameResult);
						goto IL_1D6;
					case 6:
						Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stThreeVsThreeInfo, (int)gameInfo.bGameResult);
						goto IL_1D6;
					}
					if (bMaxAcntNum == 10)
					{
						Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stFiveVsFiveInfo, (int)gameInfo.bGameResult);
					}
					IL_1D6:
					Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
					return;
				}
				Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stVsMachineInfo, (int)gameInfo.bGameResult);
				return;
			case 9:
				if (gameInfo.bIsPKAI == 1)
				{
					Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stEntertainmentInfo, (int)gameInfo.bGameResult);
					Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
				}
				return;
			default:
				return;
			}
		}
	}
}

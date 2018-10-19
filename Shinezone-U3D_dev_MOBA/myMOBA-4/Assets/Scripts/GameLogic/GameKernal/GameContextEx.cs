using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.GameKernal
{
	public class GameContextEx : Singleton<GameContextEx>
	{
		public enum Camp
		{
			CAMP_MID,
			CAMP_1,
			CAMP_2,
			CAMP_MAX
		}

		public struct CommonGameContextData
		{
			public COM_GAME_TYPE GameType;

			public bool IsMultiPlayerGame;

			public int MapId;

			public bool IsMobaGame;

			public enSelectType SelectHeroType;

			public int LevelDifficulty;

			public RES_LEVEL_HEROAITYPE HeroAiType;

			public bool SoulGrowActive;

			public int BaseReviveTime;

			public uint DynamicPropertyConfigId;

			public string MiniMapPath;

			public string BigMapPath;

			public int MapWidth;

			public int MapHeight;

			public int BigMapWidth;

			public int BigMapHeight;

			public float MapFOWScale;

			public float BigMapFOWScale;

			public string MapName;

			public string MapDesignFileName;

			public string MapArtistFileName;

			public string MusicStartEvent;

			public string MusicEndEvent;

			public string AmbientSoundEvent;

			public string MusicBankResName;

			public Horizon.EnableMethod HorizonEnableMethod;

			public uint SoulId;

			public uint SoulAllocId;

			public int ExtraMapSkillId;

			public int ExtraMapSkill2Id;

			public int ExtraPassiveSkillId;

			public int OrnamentSkillId;

			public int OrnamentSwitchCd0;

			public bool EnableFOW;

			public bool EnableShopHorizonTab;

			public bool EnableOrnamentSlot;

			public bool CanRightJoyStickCameraDrag;

			public bool CameraFlip;
		}

		public struct MobaData
		{
			public bool IsValid;

			public int MapType;

			public RES_ENTERTAINMENT_MAP_SUB_TYPE EntertainmentMapSubType;

			public ushort OriginalGoldCoinInBattle;

			public int PlayerNum;

			public bool BattleEquipLimit;

			public int HeadPtsMaxLimit;

			public int BirthLevelConfig;

			public bool IsShowHonor;

			public uint CooldownReduceMaxLimit;

			public bool IsOpenExpCompensate;

			public ResDT_ExpCompensateInfo[] ExpCompensateInfo;

			public int SoldierActivateDelay;

			public int SoldierActivateCountDelay1;

			public int SoldierActivateCountDelay2;

			public uint TimeDuration;

			public uint AddWinCondStarId;

			public uint AddLoseCondStarId;

			public string SecondName;

			public string GameMatchName;

			public bool IsWarmBattle;

			public ResBattleDynamicDifficulty WarmHeroAiDiffInfo;

			public byte KFrapsLater;

			public uint KFrapsFreqMs;

			public byte PreActFrap;

			public uint RandomSeed;
		}

		public struct SoloGameData
		{
			public bool IsValid;

			public RES_LEVEL_TYPE PveLevelType;

			public int ChapterNo;

			public int LevelNo;

			public int FinDialogId;

			public int PreDialogId;

			public int FailureDialogId;

			public bool IsShowTrainingHelper;

			public bool CanAutoGame;

			public int ReviveTimeMax;

			public int LoseCondition;

			public ResDT_MapBuff[] MapBuffs;

			public ResDT_IntParamArrayNode[] StarDetail;

			public ResDT_PveReviveInfo[] ReviveInfo;
		}

		private List<GameContextEx.Camp> _enemyCampList = new List<GameContextEx.Camp>();

		private List<GameContextEx.Camp> _validCampList = new List<GameContextEx.Camp>();

		private GameContextEx.CommonGameContextData _gameContextCommonData = default(GameContextEx.CommonGameContextData);

		private GameContextEx.MobaData _gameContextMobaData = default(GameContextEx.MobaData);

		private GameContextEx.SoloGameData _gameContextSoloData = default(GameContextEx.SoloGameData);

		public GameContextEx.CommonGameContextData GameContextCommonInfo
		{
			get
			{
				return this._gameContextCommonData;
			}
		}

		public GameContextEx.MobaData GameContextMobaInfo
		{
			get
			{
				return this._gameContextMobaData;
			}
		}

		public GameContextEx.SoloGameData GameContextSoloInfo
		{
			get
			{
				return this._gameContextSoloData;
			}
		}

		public void InitSingleGame(SCPKG_STARTSINGLEGAMERSP svrGameInfo)
		{
			this._ResetAllInfo();
			if (svrGameInfo != null)
			{
				this._InitSingleGame(svrGameInfo);
			}
		}

		public void InitMultiGame(SCPKG_MULTGAME_BEGINLOAD svrGameInfo)
		{
			this._ResetAllInfo();
			if (svrGameInfo != null)
			{
				uint dwMapId = svrGameInfo.stDeskInfo.dwMapId;
				byte bMapType = svrGameInfo.stDeskInfo.bMapType;
				bool isWarmBattle = svrGameInfo.stDeskInfo.bIsWarmBattle > 0;
				byte bAILevel = svrGameInfo.stDeskInfo.bAILevel;
				this._InitMultiGame((COM_GAME_TYPE)svrGameInfo.bGameType, (int)bMapType, dwMapId, isWarmBattle, (int)bAILevel);
				this._InitSynchrConfig(svrGameInfo);
			}
		}

		private void InitCampData()
		{
			for (int i = 0; i < 3; i++)
			{
				this._validCampList.Add(GameContextEx.Camp.CAMP_1);
				this._enemyCampList.Add(GameContextEx.Camp.CAMP_1);
			}
		}

		public uint GetValidCampCount()
		{
			return 3u;
		}

		public GameContextEx.Camp GetValidFirstCamp()
		{
			return GameContextEx.Camp.CAMP_MID;
		}

		public GameContextEx.Camp GetValidLastCamp()
		{
			return GameContextEx.Camp.CAMP_2;
		}

		public ReadonlyContext<GameContextEx.Camp> GetEnemyCamps(GameContextEx.Camp camp, out uint validCount)
		{
			validCount = 1u;
			return new ReadonlyContext<GameContextEx.Camp>(this._enemyCampList);
		}

		public ReadonlyContext<GameContextEx.Camp> GetAllValidCamps(GameContextEx.Camp camp, out uint validCount)
		{
			validCount = 1u;
			return new ReadonlyContext<GameContextEx.Camp>(this._validCampList);
		}

		public void Test()
		{
			uint num = 1u;
			ReadonlyContext<GameContextEx.Camp> enemyCamps = this.GetEnemyCamps(GameContextEx.Camp.CAMP_1, out num);
			for (int i = 0; i < enemyCamps.Count; i++)
			{
			}
		}

		public enSelectType GetSelectHeroType()
		{
			return this._gameContextCommonData.SelectHeroType;
		}

		public bool IsLuanDouPlayMode()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT && this._gameContextMobaData.EntertainmentMapSubType == RES_ENTERTAINMENT_MAP_SUB_TYPE.RES_ENTERTAINMENT_MAP_SUB_TYPE_CHAOS_BATTLE;
		}

		public bool IsFireHolePlayMode()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT && this._gameContextMobaData.EntertainmentMapSubType == RES_ENTERTAINMENT_MAP_SUB_TYPE.RES_ENTERTAINMENT_MAP_SUB_TYPE_FIRE_HOLE;
		}

		public bool IsMultiPlayerGame()
		{
			return this._gameContextCommonData.IsMultiPlayerGame;
		}

		public bool IsMobaMode()
		{
			return this._gameContextCommonData.IsMobaGame;
		}

		public bool IsMobaModeWithOutGuide()
		{
			return this.IsMobaMode() && !this.IsGameTypeGuide();
		}

		public bool IsMultilModeWithWarmBattle()
		{
			return this._gameContextMobaData.IsWarmBattle || this.IsMultilModeWithoutWarmBattle();
		}

		public bool IsChasmChaosBattle()
		{
			return this._gameContextCommonData.MapId == 90001;
		}

		public bool IsMultilModeWithoutWarmBattle()
		{
			return this._gameContextCommonData.IsMobaGame && !this._gameContextMobaData.IsWarmBattle;
		}

		public COM_GAME_TYPE GetGameType()
		{
			return this._gameContextCommonData.GameType;
		}

		public bool IsGameTypeGuide()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE;
		}

		public bool IsGameTypeAdventure()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE;
		}

		public bool IsGameTypeActivity()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY;
		}

		public bool IsGameTypeBurning()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING;
		}

		public bool IsGameTypeArena()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA;
		}

		public bool IsGameTypeComBat()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT;
		}

		public bool IsGameTypePvpRoom()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM;
		}

		public bool IsGameTypePvpMatch()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH;
		}

		public bool IsGameTypeEntertainment()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT;
		}

		public bool IsGameTypeLadder()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER;
		}

		public bool IsGameTypeRewardMatch()
		{
			return this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_REWARDMATCH;
		}

		public virtual bool IsSoulGrow()
		{
			return this._gameContextCommonData.SoulGrowActive;
		}

		public override void Init()
		{
			this._ResetAllInfo();
			this.InitCampData();
		}

		private void _InitSingleGame(SCPKG_STARTSINGLEGAMERSP svrGameInfo)
		{
			this._gameContextCommonData.GameType = (COM_GAME_TYPE)svrGameInfo.bGameType;
			ResLevelCfgInfo levelCfg = null;
			if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE)
			{
				levelCfg = GameDataMgr.levelDatabin.GetDataByKey((long)svrGameInfo.iLevelId);
			}
			else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE)
			{
				levelCfg = GameDataMgr.levelDatabin.GetDataByKey((long)svrGameInfo.iLevelId);
				this._gameContextCommonData.LevelDifficulty = Singleton<CAdventureSys>.instance.currentDifficulty;
			}
			else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING)
			{
				levelCfg = GameDataMgr.burnMap.GetDataByKey((long)Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex));
			}
			else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA)
			{
				levelCfg = GameDataMgr.arenaLevelDatabin.GetDataByKey((long)svrGameInfo.iLevelId);
			}
			else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)
			{
				uint dwMapId = svrGameInfo.stGameParam.stSingleGameRspOfCombat.dwMapId;
				byte bMapType = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bMapType;
				bool isWarmBattle = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle > 0;
				byte bAILevel = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bAILevel;
				this._InitMultiGame((COM_GAME_TYPE)svrGameInfo.bGameType, (int)bMapType, dwMapId, isWarmBattle, (int)bAILevel);
				return;
			}
			this._SetSingleGame(levelCfg);
		}

		private void _SetSingleGame(ResLevelCfgInfo levelCfg)
		{
			if (levelCfg == null)
			{
				return;
			}
			this._gameContextCommonData.MapId = levelCfg.iCfgID;
			this._gameContextCommonData.IsMobaGame = (levelCfg.iLevelType == 0);
			this._gameContextCommonData.SelectHeroType = enSelectType.enMutile;
			this._gameContextCommonData.LevelDifficulty = Math.Max(1, this._gameContextCommonData.LevelDifficulty);
			this._gameContextCommonData.HeroAiType = (RES_LEVEL_HEROAITYPE)levelCfg.iHeroAIType;
			this._gameContextCommonData.SoulGrowActive = (levelCfg.bSoulGrow > 0);
			this._gameContextCommonData.BaseReviveTime = (int)levelCfg.dwReviveTime;
			this._gameContextCommonData.DynamicPropertyConfigId = levelCfg.dwDynamicPropertyCfg;
			this._gameContextCommonData.MiniMapPath = levelCfg.szThumbnailPath;
			this._gameContextCommonData.BigMapPath = levelCfg.szBigMapPath;
			this._gameContextCommonData.MapWidth = levelCfg.iMapWidth;
			this._gameContextCommonData.MapHeight = levelCfg.iMapHeight;
			this._gameContextCommonData.BigMapWidth = levelCfg.iBigMapWidth;
			this._gameContextCommonData.BigMapHeight = levelCfg.iBigMapHeight;
			this._gameContextCommonData.MapFOWScale = 1f;
			this._gameContextCommonData.BigMapFOWScale = 1f;
			this._gameContextCommonData.MapName = levelCfg.szName;
			this._gameContextCommonData.MapDesignFileName = levelCfg.szDesignFileName;
			this._gameContextCommonData.MapArtistFileName = levelCfg.szArtistFileName;
			this._gameContextCommonData.MusicStartEvent = levelCfg.szMusicStartEvent;
			this._gameContextCommonData.MusicEndEvent = levelCfg.szMusicEndEvent;
			this._gameContextCommonData.AmbientSoundEvent = levelCfg.szAmbientSoundEvent;
			this._gameContextCommonData.MusicBankResName = levelCfg.szBankResourceName;
			this._gameContextCommonData.HorizonEnableMethod = (Horizon.EnableMethod)((!this._gameContextCommonData.IsMobaGame) ? levelCfg.bEnableHorizon : (byte)1);
			this._gameContextCommonData.SoulId = levelCfg.dwSoulID;
			this._gameContextCommonData.SoulAllocId = levelCfg.dwSoulAllocId;
			this._gameContextCommonData.ExtraMapSkillId = levelCfg.iExtraSkillId;
			this._gameContextCommonData.ExtraMapSkill2Id = levelCfg.iExtraSkill2Id;
			this._gameContextCommonData.ExtraPassiveSkillId = levelCfg.iExtraPassiveSkillId;
			this._gameContextCommonData.OrnamentSkillId = 0;
			this._gameContextCommonData.OrnamentSwitchCd0 = 0;
			this._gameContextCommonData.EnableFOW = false;
			this._gameContextCommonData.EnableShopHorizonTab = false;
			this._gameContextCommonData.EnableOrnamentSlot = false;
			this._gameContextCommonData.CanRightJoyStickCameraDrag = (levelCfg.bSupportCameraDrag > 0);
			this._gameContextCommonData.CameraFlip = false;
			this._gameContextSoloData.IsValid = true;
			this._gameContextSoloData.PveLevelType = (RES_LEVEL_TYPE)levelCfg.iLevelType;
			this._gameContextSoloData.ChapterNo = levelCfg.iChapterId;
			this._gameContextSoloData.LevelNo = (int)levelCfg.bLevelNo;
			this._gameContextSoloData.FinDialogId = levelCfg.iPassDialogId;
			this._gameContextSoloData.PreDialogId = levelCfg.iPreDialogId;
			this._gameContextSoloData.FailureDialogId = levelCfg.iFailureDialogId;
			this._gameContextSoloData.IsShowTrainingHelper = (levelCfg.bShowTrainingHelper > 0);
			this._gameContextSoloData.CanAutoGame = (levelCfg.bIsOpenAutoAI == 0);
			this._gameContextSoloData.ReviveTimeMax = (int)levelCfg.bReviveTimeMax;
			this._gameContextSoloData.LoseCondition = levelCfg.iLoseCondition;
			this._gameContextSoloData.MapBuffs = levelCfg.astMapBuffs;
			this._gameContextSoloData.StarDetail = levelCfg.astStarDetail;
			this._gameContextSoloData.ReviveInfo = levelCfg.astReviveInfo;
		}

		private void _ResetAllInfo()
		{
			this._gameContextCommonData.GameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;
			this._gameContextCommonData.IsMultiPlayerGame = false;
			this._gameContextCommonData.MapId = 0;
			this._gameContextCommonData.IsMobaGame = false;
			this._gameContextCommonData.SelectHeroType = enSelectType.enNull;
			this._gameContextCommonData.LevelDifficulty = 0;
			this._gameContextCommonData.HeroAiType = RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_NULL;
			this._gameContextCommonData.SoulGrowActive = false;
			this._gameContextCommonData.BaseReviveTime = 15000;
			this._gameContextCommonData.DynamicPropertyConfigId = 0u;
			this._gameContextCommonData.MiniMapPath = string.Empty;
			this._gameContextCommonData.BigMapPath = string.Empty;
			this._gameContextCommonData.MapWidth = 0;
			this._gameContextCommonData.MapHeight = 0;
			this._gameContextCommonData.BigMapWidth = 0;
			this._gameContextCommonData.BigMapHeight = 0;
			this._gameContextCommonData.MapFOWScale = 1f;
			this._gameContextCommonData.BigMapFOWScale = 1f;
			this._gameContextCommonData.MapName = string.Empty;
			this._gameContextCommonData.MapDesignFileName = string.Empty;
			this._gameContextCommonData.MapArtistFileName = string.Empty;
			this._gameContextCommonData.MusicStartEvent = string.Empty;
			this._gameContextCommonData.MusicEndEvent = string.Empty;
			this._gameContextCommonData.AmbientSoundEvent = string.Empty;
			this._gameContextCommonData.MusicBankResName = string.Empty;
			this._gameContextCommonData.HorizonEnableMethod = Horizon.EnableMethod.INVALID;
			this._gameContextCommonData.SoulId = 0u;
			this._gameContextCommonData.SoulAllocId = 0u;
			this._gameContextCommonData.ExtraMapSkillId = 0;
			this._gameContextCommonData.ExtraMapSkill2Id = 0;
			this._gameContextCommonData.ExtraPassiveSkillId = 0;
			this._gameContextCommonData.OrnamentSkillId = 0;
			this._gameContextCommonData.OrnamentSwitchCd0 = 0;
			this._gameContextCommonData.EnableFOW = false;
			this._gameContextCommonData.EnableShopHorizonTab = false;
			this._gameContextCommonData.EnableOrnamentSlot = false;
			this._gameContextCommonData.CanRightJoyStickCameraDrag = false;
			this._gameContextCommonData.CameraFlip = false;
			this._gameContextMobaData.IsValid = false;
			this._gameContextMobaData.MapType = -1;
			this._gameContextMobaData.OriginalGoldCoinInBattle = 0;
			this._gameContextMobaData.PlayerNum = 0;
			this._gameContextMobaData.BattleEquipLimit = false;
			this._gameContextMobaData.HeadPtsMaxLimit = 0;
			this._gameContextMobaData.BirthLevelConfig = 0;
			this._gameContextMobaData.IsShowHonor = false;
			this._gameContextMobaData.CooldownReduceMaxLimit = 0u;
			this._gameContextMobaData.IsOpenExpCompensate = false;
			this._gameContextMobaData.ExpCompensateInfo = null;
			this._gameContextMobaData.SoldierActivateDelay = 0;
			this._gameContextMobaData.SoldierActivateCountDelay1 = 0;
			this._gameContextMobaData.SoldierActivateCountDelay2 = 0;
			this._gameContextMobaData.TimeDuration = 0u;
			this._gameContextMobaData.AddWinCondStarId = 0u;
			this._gameContextMobaData.AddLoseCondStarId = 0u;
			this._gameContextMobaData.SecondName = string.Empty;
			this._gameContextMobaData.GameMatchName = string.Empty;
			this._gameContextMobaData.IsWarmBattle = false;
			this._gameContextMobaData.WarmHeroAiDiffInfo = null;
			this._gameContextMobaData.RandomSeed = 0u;
			this._gameContextMobaData.KFrapsLater = 0;
			this._gameContextMobaData.KFrapsFreqMs = 0u;
			this._gameContextMobaData.PreActFrap = 0;
			this._gameContextSoloData.IsValid = false;
			this._gameContextSoloData.PveLevelType = RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP;
			this._gameContextSoloData.ChapterNo = 0;
			this._gameContextSoloData.LevelNo = 0;
			this._gameContextSoloData.FinDialogId = 0;
			this._gameContextSoloData.PreDialogId = 0;
			this._gameContextSoloData.FailureDialogId = 0;
			this._gameContextSoloData.IsShowTrainingHelper = false;
			this._gameContextSoloData.CanAutoGame = false;
			this._gameContextSoloData.ReviveTimeMax = 0;
			this._gameContextSoloData.LoseCondition = 0;
			this._gameContextSoloData.MapBuffs = null;
			this._gameContextSoloData.StarDetail = null;
			this._gameContextSoloData.ReviveInfo = null;
		}

		private void _InitSynchrConfig(SCPKG_MULTGAME_BEGINLOAD svrGameInfo)
		{
			this._gameContextMobaData.RandomSeed = svrGameInfo.dwRandomSeed;
			this._gameContextMobaData.KFrapsLater = svrGameInfo.bKFrapsLater;
			this._gameContextMobaData.KFrapsFreqMs = svrGameInfo.dwKFrapsFreqMs;
			this._gameContextMobaData.PreActFrap = svrGameInfo.bPreActFrap;
		}

		private void _InitMultiGame(COM_GAME_TYPE GameType, int mapType, uint mapID, bool isWarmBattle = false, int warmAiLv = 0)
		{
			ResDT_LevelCommonInfo pvpMapCommonInfo = this.GetPvpMapCommonInfo(mapType, mapID);
			this._gameContextCommonData.GameType = GameType;
			this._gameContextCommonData.IsMultiPlayerGame = true;
			this._gameContextCommonData.MapId = (int)mapID;
			this._gameContextCommonData.IsMobaGame = true;
			this._gameContextCommonData.SelectHeroType = (enSelectType)pvpMapCommonInfo.stPickRuleInfo.bPickType;
			this._gameContextCommonData.LevelDifficulty = 1;
			this._gameContextCommonData.HeroAiType = (RES_LEVEL_HEROAITYPE)pvpMapCommonInfo.iHeroAIType;
			this._gameContextCommonData.SoulGrowActive = true;
			this._gameContextCommonData.BaseReviveTime = 15000;
			this._gameContextCommonData.DynamicPropertyConfigId = pvpMapCommonInfo.dwDynamicPropertyCfg;
			this._gameContextCommonData.MiniMapPath = pvpMapCommonInfo.szThumbnailPath;
			this._gameContextCommonData.BigMapPath = pvpMapCommonInfo.szBigMapPath;
			this._gameContextCommonData.MapWidth = pvpMapCommonInfo.iMapWidth;
			this._gameContextCommonData.MapHeight = pvpMapCommonInfo.iMapHeight;
			this._gameContextCommonData.BigMapWidth = pvpMapCommonInfo.iBigMapWidth;
			this._gameContextCommonData.BigMapHeight = pvpMapCommonInfo.iBigMapHeight;
			this._gameContextCommonData.MapFOWScale = pvpMapCommonInfo.fBigMapFowScale;
			this._gameContextCommonData.BigMapFOWScale = pvpMapCommonInfo.fBigMapFowScale;
			this._gameContextCommonData.MapName = pvpMapCommonInfo.szName;
			this._gameContextCommonData.MapDesignFileName = pvpMapCommonInfo.szDesignFileName;
			this._gameContextCommonData.MapArtistFileName = pvpMapCommonInfo.szArtistFileName;
			this._gameContextCommonData.MusicStartEvent = pvpMapCommonInfo.szMusicStartEvent;
			this._gameContextCommonData.MusicEndEvent = pvpMapCommonInfo.szMusicEndEvent;
			this._gameContextCommonData.AmbientSoundEvent = pvpMapCommonInfo.szBankResourceName;
			this._gameContextCommonData.MusicBankResName = pvpMapCommonInfo.szAmbientSoundEvent;
			this._gameContextCommonData.HorizonEnableMethod = Horizon.EnableMethod.EnableAll;
			this._gameContextCommonData.SoulId = pvpMapCommonInfo.dwSoulID;
			this._gameContextCommonData.SoulAllocId = pvpMapCommonInfo.dwSoulAllocId;
			this._gameContextCommonData.ExtraMapSkillId = pvpMapCommonInfo.iExtraSkillId;
			this._gameContextCommonData.ExtraMapSkill2Id = pvpMapCommonInfo.iExtraSkill2Id;
			this._gameContextCommonData.ExtraPassiveSkillId = pvpMapCommonInfo.iExtraPassiveSkillId;
			this._gameContextCommonData.OrnamentSkillId = pvpMapCommonInfo.iOrnamentSkillId;
			this._gameContextCommonData.OrnamentSwitchCd0 = pvpMapCommonInfo.iOrnamentSwitchCD;
			this._gameContextCommonData.EnableFOW = (pvpMapCommonInfo.bIsEnableFow > 0);
			this._gameContextCommonData.EnableShopHorizonTab = (pvpMapCommonInfo.bIsEnableOrnamentSlot > 0);
			this._gameContextCommonData.EnableOrnamentSlot = (pvpMapCommonInfo.bIsEnableShopHorizonTab > 0);
			this._gameContextCommonData.CanRightJoyStickCameraDrag = (pvpMapCommonInfo.bSupportCameraDrag > 0);
			this._gameContextCommonData.CameraFlip = (pvpMapCommonInfo.bCameraFlip > 0);
			this._gameContextMobaData.IsValid = true;
			this._gameContextMobaData.MapType = mapType;
			this._gameContextMobaData.OriginalGoldCoinInBattle = pvpMapCommonInfo.wOriginalGoldCoinInBattle;
			this._gameContextMobaData.PlayerNum = (int)pvpMapCommonInfo.bMaxAcntNum;
			this._gameContextMobaData.BattleEquipLimit = (pvpMapCommonInfo.bBattleEquipLimit > 0);
			this._gameContextMobaData.HeadPtsMaxLimit = (int)pvpMapCommonInfo.bHeadPtsUpperLimit;
			this._gameContextMobaData.BirthLevelConfig = (int)pvpMapCommonInfo.bBirthLevelConfig;
			this._gameContextMobaData.IsShowHonor = (pvpMapCommonInfo.bShowHonor > 0);
			this._gameContextMobaData.CooldownReduceMaxLimit = pvpMapCommonInfo.dwCooldownReduceUpperLimit;
			this._gameContextMobaData.IsOpenExpCompensate = (pvpMapCommonInfo.bIsOpenExpCompensate > 0);
			this._gameContextMobaData.ExpCompensateInfo = pvpMapCommonInfo.astExpCompensateDetail;
			this._gameContextMobaData.SoldierActivateDelay = pvpMapCommonInfo.iSoldierActivateDelay;
			this._gameContextMobaData.SoldierActivateCountDelay1 = pvpMapCommonInfo.iSoldierActivateCountDelay1;
			this._gameContextMobaData.SoldierActivateCountDelay2 = pvpMapCommonInfo.iSoldierActivateCountDelay2;
			this._gameContextMobaData.TimeDuration = pvpMapCommonInfo.dwTimeDuration;
			this._gameContextMobaData.AddWinCondStarId = pvpMapCommonInfo.dwAddWinCondStarId;
			this._gameContextMobaData.AddLoseCondStarId = pvpMapCommonInfo.dwAddLoseCondStarId;
			this._gameContextMobaData.SecondName = string.Empty;
			this._gameContextMobaData.GameMatchName = pvpMapCommonInfo.szGameMatchName;
			this._gameContextMobaData.IsWarmBattle = false;
			this._gameContextMobaData.WarmHeroAiDiffInfo = null;
			this._gameContextMobaData.IsWarmBattle = isWarmBattle;
			this._gameContextMobaData.WarmHeroAiDiffInfo = ((!isWarmBattle) ? null : GameDataMgr.battleDynamicDifficultyDB.GetDataByKey((long)warmAiLv));
			if (mapType == 4)
			{
				ResEntertainmentLevelInfo dataByKey = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapID);
				this._gameContextMobaData.EntertainmentMapSubType = (RES_ENTERTAINMENT_MAP_SUB_TYPE)dataByKey.bEntertainmentSubType;
			}
			else if (mapType == 5)
			{
				ResRewardMatchLevelInfo dataByKey2 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
				this._gameContextMobaData.SecondName = dataByKey2.szMatchName;
			}
		}

		public ResDT_LevelCommonInfo GetPvpMapCommonInfo(int mapType, uint mapId)
		{
			ResDT_LevelCommonInfo resDT_LevelCommonInfo = null;
			switch (mapType)
			{
			case 1:
			{
				ResAcntBattleLevelInfo dataByKey = GameDataMgr.pvpLevelDatabin.GetDataByKey(mapId);
				resDT_LevelCommonInfo = dataByKey.stLevelCommonInfo;
				break;
			}
			case 2:
			{
				ResCounterPartLevelInfo dataByKey2 = GameDataMgr.cpLevelDatabin.GetDataByKey(mapId);
				resDT_LevelCommonInfo = dataByKey2.stLevelCommonInfo;
				break;
			}
			case 3:
			{
				ResRankLevelInfo dataByKey3 = GameDataMgr.rankLevelDatabin.GetDataByKey(mapId);
				resDT_LevelCommonInfo = dataByKey3.stLevelCommonInfo;
				break;
			}
			case 4:
			{
				ResEntertainmentLevelInfo dataByKey4 = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapId);
				resDT_LevelCommonInfo = dataByKey4.stLevelCommonInfo;
				break;
			}
			case 5:
			{
				ResRewardMatchLevelInfo dataByKey5 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapId);
				resDT_LevelCommonInfo = dataByKey5.stLevelCommonInfo;
				break;
			}
			}
			if (resDT_LevelCommonInfo == null)
			{
			}
			return resDT_LevelCommonInfo;
		}
	}
}

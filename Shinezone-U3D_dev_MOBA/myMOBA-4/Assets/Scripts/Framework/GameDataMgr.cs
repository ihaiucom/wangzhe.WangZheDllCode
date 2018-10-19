using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tsf4g_tdr_csharp;

namespace Assets.Scripts.Framework
{
	[MessageHandlerClass]
	public class GameDataMgr : Singleton<GameDataMgr>, IGameModule
	{
		public static DatabinTable<ResLevelCfgInfo, int> levelDatabin = null;

		public static DatabinTable<ResLevelCfgInfo, int> activityLevelDatabin = null;

		public static DatabinTable<ResLevelCfgInfo, int> burnMap = null;

		public static DatabinTable<ResLevelCfgInfo, int> arenaLevelDatabin = null;

		public static DatabinTable<ResAcntBattleLevelInfo, uint> pvpLevelDatabin = null;

		public static DatabinTable<ResCounterPartLevelInfo, uint> cpLevelDatabin = null;

		public static DatabinTable<ResRankLevelInfo, uint> rankLevelDatabin = null;

		public static DatabinTable<ResEntertainmentLevelInfo, uint> entertainLevelDatabin = null;

		public static DatabinTable<ResRewardMatchLevelInfo, uint> uinionBattleLevelDatabin = null;

		public static DatabinTable<ResGuildMatchLevelInfo, uint> guildMatchLevelDatabin = null;

		public static DatabinTable<ResGuildLevel, byte> guildLevelDatabin = null;

		public static DatabinTable<ResCommonSettle, uint> settleDatabin = null;

		public static DatabinTable<ResHeroCfgInfo, uint> heroDatabin = null;

		public static DatabinTable<ResHeroEnergyInfo, uint> heroEnergyDatabin = null;

		public static DatabinTable<ResSkillBeanCfgInfo, uint> skillBeanDatabin = null;

		public static DatabinTable<ResSkillCfgInfo, int> skillDatabin = null;

		public static DatabinTable<ResMonsterCfgInfo, long> monsterDatabin = null;

		public static DatabinTable<ResOrganCfgInfo, long> organDatabin = null;

		public static DatabinTable<ResSkillCombineCfgInfo, int> skillCombineDatabin = null;

		public static DatabinTable<ResSoldierWaveInfo, uint> soldierWaveDatabin = null;

		public static DatabinTable<ResHeroLvlUpInfo, uint> heroLvlUpDatabin = null;

		public static DatabinTable<ResPropInfo, uint> itemDatabin = null;

		public static DatabinTable<TowerHitConf, byte> towerHitDatabin = null;

		public static DatabinTable<ResBattleDynamicProperty, uint> battleDynamicPropertyDB = null;

		public static DatabinTable<ResBattleDynamicDifficulty, uint> battleDynamicDifficultyDB = null;

		public static DatabinTableMulti<ResActorLinesInfo, int> actorLinesDatabin = null;

		public static DatabinTable<ResAcntExpInfo, uint> acntExpDatabin = null;

		public static DatabinTable<ResGlobalInfo, uint> globalInfoDatabin = null;

		public static DatabinTable<ResChapterInfo, uint> chapterInfoDatabin = null;

		public static DatabinTable<ResTask, uint> taskDatabin = null;

		public static DatabinTable<ResPrerequisite, uint> taskPrerequisiteDatabin = null;

		public static DatabinTable<ResTaskReward, uint> taskRewardDatabin = null;

		public static DatabinTable<ResPvpLevelReward, int> resPvpLevelRewardDatabin = null;

		public static DatabinTable<ResEquipInfo, uint> equipInfoDatabin = null;

		public static DatabinTable<ResComposeInfo, uint> composeInfoDatabin = null;

		public static DatabinTable<ResBufDropInfo, uint> bufDropInfoDatabin = null;

		public static DatabinTable<ResEvaluateStarInfo, uint> evaluateCondInfoDatabin = null;

		public static DatabinTable<ResEvaluateStarInfo, uint> addWinLoseCondDatabin = null;

		public static DatabinTable<ResTextData, uint> textBubbleDatabin = null;

		public static DatabinTable<ResGuideTipInfo, int> guideTipDatabin = null;

		public static DatabinTable<ResShopInfo, uint> resShopInfoDatabin = null;

		public static DatabinTable<ResCoinBuyInfo, ushort> coninBuyDatabin = null;

		public static DatabinTable<ResText, ushort> s_textDatabin = null;

		public static DatabinTable<ResRuleText, ushort> s_ruleTextDatabin = null;

		public static DatabinTable<ResRandomSkillPassiveRule, int> randomSkillPassiveDatabin = null;

		public static DatabinTable<ResSkillPassiveCfgInfo, int> skillPassiveDatabin = null;

		public static DatabinTable<ResSkillMarkCfgInfo, int> skillMarkDatabin = null;

		public static DatabinTable<ResMonsterOrganLevelDynamicInfo, int> monsterOrganLvDynamicInfobin = null;

		public static DatabinTableMulti<ResActivity, uint> activityDatabin = null;

		public static DatabinTable<ResUnlockCondition, uint> unlockConditionDatabin = null;

		public static DatabinTable<ResSpecialFucUnlock, uint> specialFunUnlockDatabin = null;

		public static DatabinTable<ResLicenseInfo, uint> licenseDatabin = null;

		public static DatabinTable<NewbieGuideMainLineConf, uint> newbieMainLineDatabin = null;

		public static DatabinTable<NewbieWeakGuideMainLineConf, uint> newbieWeakMainLineDataBin = null;

		public static DatabinTable<NewbieGuideWeakConf, uint> newbieWeakDatabin = null;

		public static DatabinTableMulti<NewbieGuideScriptConf, uint> newbieScriptDatabin = null;

		public static DatabinTable<NewbieGuideSpecialTipConf, uint> newbieSpecialTipDatabin = null;

		public static DatabinTable<NewbieGuideBannerGuideConf, ushort> newbieBannerGuideDatabin = null;

		public static DatabinTableMulti<ResSoulLvlUpInfo, uint> soulLvlUpDatabin = null;

		public static DatabinTable<ResIncomeAllocRule, uint> incomeAllocDatabin = null;

		public static DatabinTable<ResShopType, ushort> shopTypeDatabin = null;

		public static DatabinTable<ResShopRefreshCost, uint> shopRefreshCostDatabin = null;

		public static DatabinTableMulti<ResHeroProficiency, byte> heroProficiencyDatabin = null;

		public static DatabinTable<ResRankGradeConf, byte> rankGradeDatabin = null;

		public static DatabinTable<ResRankRewardConf, uint> rankRewardDatabin = null;

		public static DatabinTableMulti<ResSoulAddition, int> soulAdditionDatabin = null;

		public static DatabinTable<ResGuildMisc, uint> guildMiscDatabin = null;

		public static DatabinTable<ResGuildIcon, uint> guildIconDatabin = null;

		public static DatabinTable<ResGuildBuilding, byte> guildBuildingDatabin = null;

		public static DatabinTable<ResGuildDonate, byte> guildDonateDatabin = null;

		public static DatabinTable<ResGuildRankReward, int> guildRankRewardDatabin = null;

		public static DatabinTable<ResGuildGradeConf, byte> guildGradeDatabin = null;

		public static DatabinTable<ResGuildShopStarIndexConf, uint> guildStarLevel = null;

		public static DatabinTable<ResSymbolInfo, uint> symbolInfoDatabin = null;

		public static DatabinTable<ResSymbolPos, byte> symbolPosDatabin = null;

		public static DatabinTable<ResHeroSymbolLvl, ushort> heroSymbolLvlDatabin = null;

		public static DatabinTableMulti<ResSymbolRcmd, uint> symbolRcmdDatabin = null;

		public static DatabinTable<ResGameTask, uint> gameTaskDatabin = null;

		public static DatabinTable<ResGameTaskGroup, uint> gameTaskGroupDatabin = null;

		public static DatabinTable<ShenFuInfo, uint> shenfuBin = null;

		public static DatabinTable<CharmLib, uint> charmLib = null;

		public static DatabinTable<ResTalentLib, uint> talentLib = null;

		public static DatabinTable<ResMiShuInfo, uint> miShuLib = null;

		public static DatabinTable<ResAcntPvpExpInfo, byte> acntPvpExpDatabin = null;

		public static DatabinTableMulti<ResBurningReward, uint> burnRewrad = null;

		public static DatabinTable<ResBurningBuff, int> burnBuffMap = null;

		public static DatabinTable<ResBattleParam, uint> battleParam = null;

		public static DatabinTable<ResPropertyValueInfo, byte> propertyValInfo = null;

		public static DatabinTable<ResHeroSkin, uint> heroSkinDatabin = null;

		public static DatabinTable<ResSkinQualityPicInfo, byte> skinQualityPicDatabin = null;

		public static DatabinTable<ResNpcOfArena, uint> npcOfArena = null;

		public static DatabinTable<ResRobotBattleList, uint> robotBattleListInfo = null;

		public static DatabinTable<ResRobotPower, uint> robotHeroInfo = null;

		public static DatabinTable<ResRobotName, uint> robotName = null;

		public static DatabinTable<ResRobotSubNameA, uint> robotSubNameA = null;

		public static DatabinTable<ResRobotSubNameB, uint> robotSubNameB = null;

		public static DatabinTable<ResRobotSubNameC, uint> robotSubNameC = null;

		public static DatabinTable<ResArenaOneDayReward, uint> arenaRewardDatabin = null;

		public static DatabinTable<ResRandomRewardStore, uint> randomRewardDB = null;

		public static DatabinTable<ResClrCD, uint> cdDatabin = null;

		public static DatabinTable<ResSignalInfo, byte> signalDatabin = null;

		public static DatabinTable<ResInBatMsgCfg, uint> inBattleMsgDatabin = null;

		public static DatabinTable<ResInBatChannelCfg, byte> inBattleChannelDatabin = null;

		public static DatabinTable<ResInBatMsgHeroActCfg, uint> inBattleHeroActDatabin = null;

		public static DatabinTable<ResShortcutDefault, uint> inBattleDefaultDatabin = null;

		public static DatabinTable<ResCouponsBuyInfo, uint> iosDianQuanBuyInfo = null;

		public static DatabinTable<ResCouponsBuyInfo, uint> androidDianQuanBuyInfo = null;

		public static DatabinTableMulti<ResClashAddition, uint> clashAdditionDB = null;

		public static DatabinTable<ResAchievement, uint> achieveDatabin = null;

		public static DatabinTable<ResTrophyLvl, uint> trophyDatabin = null;

		public static DatabinTable<ResSkillUnlock, ushort> addedSkiilDatabin = null;

		public static DatabinTable<ResCallMonster, ushort> callMonsterDatabin = null;

		public static DatabinTable<ResVIPCoupons, uint> resVipDianQuan = null;

		public static DatabinTable<ResHeroWakeInfo, long> heroAwakDatabin = null;

		public static DatabinTable<ResNobeInfo, byte> resNobeInfoDatabin = null;

		public static DatabinTable<ResHeroSelectTextData, uint> m_selectHeroChatDatabin = null;

		public static DatabinTable<ResEquipInBattle, ushort> m_equipInBattleDatabin = null;

		public static DatabinTable<ResRecommendEquipInBattle, long> m_recommendEquipInBattleDatabin = null;

		public static DatabinTable<ResEquipEval, byte> m_recommendEquipJudge = null;

		public static DatabinTable<ResChangeName, uint> changeNameDatabin = null;

		public static DatabinTable<ResFakeAcntSkill, ushort> robotPlayerSkillDatabin = null;

		public static DatabinTable<ResFakeAcntHero, ushort> robotRookieHeroSkinDatabin = null;

		public static DatabinTable<ResFakeAcntHero, ushort> robotVeteranHeroSkinDatabin = null;

		public static DatabinTable<ResBanHeroConf, long> banHeroBin = null;

		public static DatabinTable<ResCommReward, uint> commonRewardDatabin = null;

		public static DatabinTable<ResRewardMatchDetailConf, uint> unionRankRewardDetailDatabin = null;

		public static DatabinTable<ResRewardMatchReward, long> unionBattleWinCntRewardDatabin = null;

		public static DatabinTable<ResHornInfo, uint> speakerDatabin = null;

		public static DatabinTable<ResCreditLevelInfo, uint> creditLevelDatabin = null;

		public static DatabinTable<ResHonor, int> resHonor = null;

		public static DatabinTable<ResBattleFloatText, uint> floatTextDatabin = null;

		public static DatabinTable<ResRecruitmentReward, ushort> recruimentReward = null;

		public static DatabinTable<ResKNPriority, uint> killNotifyDatabin = null;

		public static DatabinTable<ResContinuKill, byte> continuKillDatabin = null;

		public static DatabinTable<ResMultiKill, byte> multiKillDatabin = null;

		public static DatabinTable<ResVoiceInteraction, uint> voiceInteractionDatabin = null;

		public static DatabinTable<ResAiParamConf, ushort> aiParamConfDataBin = null;

		public static DatabinTable<ResFamousMentor, uint> famousMentorDatabin = null;

		public static DatabinTable<ResDeadInfoText, byte> deadInfoTextDatabin = null;

		public static DatabinTable<ResDeadInfoCondition, byte> deadInfoConditionDatabin = null;

		public static DatabinTable<ResSpeedAdjustConfig, byte> speedAdjusterDatabin = null;

		public static DatabinTable<ResSocialEnum, byte> socialEnumDatabin = null;

		public static DatabinTable<ResSocialTags, int> socialTagsDataBin = null;

		public static DatabinTable<ResAskforTemplet, long> askForTemplateDatabin = null;

		public static DictionaryView<uint, ResWealCheckIn> wealCheckInDict = new DictionaryView<uint, ResWealCheckIn>();

		public static DictionaryView<uint, ResWealFixedTime> wealFixtimeDict = new DictionaryView<uint, ResWealFixedTime>();

		public static DictionaryView<uint, ResWealMultiple> wealMultipleDict = new DictionaryView<uint, ResWealMultiple>();

		public static DictionaryView<uint, ResCltWealExchange> wealExchangeDict = new DictionaryView<uint, ResCltWealExchange>();

		public static DictionaryView<uint, ResWealPointExchange> wealPointExchangeDict = new DictionaryView<uint, ResWealPointExchange>();

		public static DictionaryView<uint, ResPVPSpecItem> pvpSpecialItemDict = new DictionaryView<uint, ResPVPSpecItem>();

		public static DictionaryView<uint, ResWealCondition> wealConditionDict = new DictionaryView<uint, ResWealCondition>();

		public static DictionaryView<uint, ResWealText> wealNoticeDict = new DictionaryView<uint, ResWealText>();

		public static DictionaryView<uint, ResFillInPrice> wealCheckFillDict = new DictionaryView<uint, ResFillInPrice>();

		public static DictionaryView<uint, ResPVPRatio> pvpRatioDict = new DictionaryView<uint, ResPVPRatio>();

		public static DictionaryView<uint, ResSpecSale> specSaleDict = new DictionaryView<uint, ResSpecSale>();

		public static DictionaryView<uint, ResHeroPromotion> heroPromotionDict = new DictionaryView<uint, ResHeroPromotion>();

		public static DictionaryView<uint, ResSkinPromotion> skinPromotionDict = new DictionaryView<uint, ResSkinPromotion>();

		public static DictionaryView<enPayType, ResLuckyDrawPrice> mallRoulettePriceDict = new DictionaryView<enPayType, ResLuckyDrawPrice>();

		public static DictionaryView<long, ResLuckyDrawRewardForClient> mallRouletteRewardDict = new DictionaryView<long, ResLuckyDrawRewardForClient>();

		public static DictionaryView<uint, ResLuckyDrawExternReward> mallRouletteExternRewardDict = new DictionaryView<uint, ResLuckyDrawExternReward>();

		public static DictionaryView<uint, ResSaleRecommend> recommendProductDict = new DictionaryView<uint, ResSaleRecommend>();

		public static DictionaryView<uint, ResRandDrawInfo> recommendLotteryCtrlDict = new DictionaryView<uint, ResRandDrawInfo>();

		public static DictionaryView<long, ResRewardPoolInfo> recommendRewardDict = new DictionaryView<long, ResRewardPoolInfo>();

		public static DictionaryView<uint, ResBoutiqueConf> boutiqueDict = new DictionaryView<uint, ResBoutiqueConf>();

		public static DictionaryView<uint, ResRedDotInfo> redDotInfoDict = new DictionaryView<uint, ResRedDotInfo>();

		public static DictionaryView<long, ResRewardMatchTimeInfo> matchTimeInfoDict = new DictionaryView<long, ResRewardMatchTimeInfo>();

		public static DictionaryView<uint, ResRewardMatchConf> rewardMatchRewardDict = new DictionaryView<uint, ResRewardMatchConf>();

		public static DictionaryView<uint, ResRareExchange> rareExchangeDict = new DictionaryView<uint, ResRareExchange>();

		public static DictionaryView<uint, ResWealParam> resWealParamDict = new DictionaryView<uint, ResWealParam>();

		public static DictionaryView<uint, ResHeroShop> heroShopInfoDict = new DictionaryView<uint, ResHeroShop>();

		public static DictionaryView<uint, ResHeroSkinShop> skinShopInfoDict = new DictionaryView<uint, ResHeroSkinShop>();

		public static DictionaryView<ushort, ResHuoYueDuReward> huoyueduDict = new DictionaryView<ushort, ResHuoYueDuReward>();

		public static DictionaryView<uint, ResHeadImage> headImageDict = new DictionaryView<uint, ResHeadImage>();

		public static DictionaryView<uint, ResGlobalInfo> svr2CltCfgDict = new DictionaryView<uint, ResGlobalInfo>();

		public static DictionaryView<uint, ResBannerImage> svr2BannerImageDict = new DictionaryView<uint, ResBannerImage>();

		public static DictionaryView<uint, ListView<ResShopPromotion>> shopPromotionDict = new DictionaryView<uint, ListView<ResShopPromotion>>();

		public static DictionaryView<uint, ResRankSeasonConf> rankSeasonDict = new DictionaryView<uint, ResRankSeasonConf>();

		public override void Init()
		{
		}

		public void UpdateFrame()
		{
		}

		public void UnloadAllDataBin()
		{
			GameDataMgr.levelDatabin.Unload();
			GameDataMgr.burnMap.Unload();
			GameDataMgr.arenaLevelDatabin.Unload();
			GameDataMgr.pvpLevelDatabin.Unload();
			GameDataMgr.cpLevelDatabin.Unload();
			GameDataMgr.rankLevelDatabin.Unload();
			GameDataMgr.entertainLevelDatabin.Unload();
			GameDataMgr.uinionBattleLevelDatabin.Unload();
			GameDataMgr.guildMatchLevelDatabin.Unload();
			GameDataMgr.guildLevelDatabin.Unload();
			GameDataMgr.settleDatabin.Unload();
			GameDataMgr.heroEnergyDatabin.Unload();
			GameDataMgr.skillBeanDatabin.Unload();
			GameDataMgr.skillDatabin.Unload();
			GameDataMgr.monsterDatabin.Unload();
			GameDataMgr.organDatabin.Unload();
			GameDataMgr.skillCombineDatabin.Unload();
			GameDataMgr.soldierWaveDatabin.Unload();
			GameDataMgr.heroLvlUpDatabin.Unload();
			GameDataMgr.towerHitDatabin.Unload();
			GameDataMgr.battleDynamicPropertyDB.Unload();
			GameDataMgr.battleDynamicDifficultyDB.Unload();
			GameDataMgr.actorLinesDatabin.Unload();
			GameDataMgr.acntExpDatabin.Unload();
			GameDataMgr.globalInfoDatabin.Unload();
			GameDataMgr.chapterInfoDatabin.Unload();
			GameDataMgr.taskDatabin.Unload();
			GameDataMgr.taskPrerequisiteDatabin.Unload();
			GameDataMgr.taskRewardDatabin.Unload();
			GameDataMgr.resPvpLevelRewardDatabin.Unload();
			GameDataMgr.equipInfoDatabin.Unload();
			GameDataMgr.composeInfoDatabin.Unload();
			GameDataMgr.bufDropInfoDatabin.Unload();
			GameDataMgr.evaluateCondInfoDatabin.Unload();
			GameDataMgr.addWinLoseCondDatabin.Unload();
			GameDataMgr.textBubbleDatabin.Unload();
			GameDataMgr.guideTipDatabin.Unload();
			GameDataMgr.resShopInfoDatabin.Unload();
			GameDataMgr.coninBuyDatabin.Unload();
			GameDataMgr.s_textDatabin.Unload();
			GameDataMgr.s_ruleTextDatabin.Unload();
			GameDataMgr.randomSkillPassiveDatabin.Unload();
			GameDataMgr.skillPassiveDatabin.Unload();
			GameDataMgr.skillMarkDatabin.Unload();
			GameDataMgr.monsterOrganLvDynamicInfobin.Unload();
			GameDataMgr.unlockConditionDatabin.Unload();
			GameDataMgr.specialFunUnlockDatabin.Unload();
			GameDataMgr.licenseDatabin.Unload();
			GameDataMgr.newbieMainLineDatabin.Unload();
			GameDataMgr.newbieWeakMainLineDataBin.Unload();
			GameDataMgr.newbieWeakDatabin.Unload();
			GameDataMgr.newbieScriptDatabin.Unload();
			GameDataMgr.newbieSpecialTipDatabin.Unload();
			GameDataMgr.newbieBannerGuideDatabin.Unload();
			GameDataMgr.soulLvlUpDatabin.Unload();
			GameDataMgr.incomeAllocDatabin.Unload();
			GameDataMgr.shopTypeDatabin.Unload();
			GameDataMgr.shopRefreshCostDatabin.Unload();
			GameDataMgr.heroProficiencyDatabin.Unload();
			GameDataMgr.rankGradeDatabin.Unload();
			GameDataMgr.rankRewardDatabin.Unload();
			GameDataMgr.soulAdditionDatabin.Unload();
			GameDataMgr.guildMiscDatabin.Unload();
			GameDataMgr.guildIconDatabin.Unload();
			GameDataMgr.guildBuildingDatabin.Unload();
			GameDataMgr.guildDonateDatabin.Unload();
			GameDataMgr.guildRankRewardDatabin.Unload();
			GameDataMgr.guildGradeDatabin.Unload();
			GameDataMgr.guildStarLevel.Unload();
			GameDataMgr.symbolInfoDatabin.Unload();
			GameDataMgr.symbolPosDatabin.Unload();
			GameDataMgr.heroSymbolLvlDatabin.Unload();
			GameDataMgr.symbolRcmdDatabin.Unload();
			GameDataMgr.gameTaskDatabin.Unload();
			GameDataMgr.gameTaskGroupDatabin.Unload();
			GameDataMgr.shenfuBin.Unload();
			GameDataMgr.charmLib.Unload();
			GameDataMgr.talentLib.Unload();
			GameDataMgr.miShuLib.Unload();
			GameDataMgr.acntPvpExpDatabin.Unload();
			GameDataMgr.burnRewrad.Unload();
			GameDataMgr.burnBuffMap.Unload();
			GameDataMgr.battleParam.Unload();
			GameDataMgr.propertyValInfo.Unload();
			GameDataMgr.skinQualityPicDatabin.Unload();
			GameDataMgr.npcOfArena.Unload();
			GameDataMgr.robotBattleListInfo.Unload();
			GameDataMgr.robotHeroInfo.Unload();
			GameDataMgr.robotName.Unload();
			GameDataMgr.robotSubNameA.Unload();
			GameDataMgr.robotSubNameB.Unload();
			GameDataMgr.robotSubNameC.Unload();
			GameDataMgr.arenaRewardDatabin.Unload();
			GameDataMgr.cdDatabin.Unload();
			GameDataMgr.signalDatabin.Unload();
			GameDataMgr.inBattleMsgDatabin.Unload();
			GameDataMgr.inBattleChannelDatabin.Unload();
			GameDataMgr.inBattleHeroActDatabin.Unload();
			GameDataMgr.inBattleDefaultDatabin.Unload();
			GameDataMgr.iosDianQuanBuyInfo.Unload();
			GameDataMgr.androidDianQuanBuyInfo.Unload();
			GameDataMgr.clashAdditionDB.Unload();
			GameDataMgr.achieveDatabin.Unload();
			GameDataMgr.addedSkiilDatabin.Unload();
			GameDataMgr.callMonsterDatabin.Unload();
			GameDataMgr.trophyDatabin.Unload();
			GameDataMgr.resVipDianQuan.Unload();
			GameDataMgr.heroAwakDatabin.Unload();
			GameDataMgr.resNobeInfoDatabin.Unload();
			GameDataMgr.m_selectHeroChatDatabin.Unload();
			GameDataMgr.m_equipInBattleDatabin.Unload();
			GameDataMgr.m_recommendEquipInBattleDatabin.Unload();
			GameDataMgr.m_recommendEquipJudge.Unload();
			GameDataMgr.changeNameDatabin.Unload();
			GameDataMgr.robotPlayerSkillDatabin.Unload();
			GameDataMgr.robotRookieHeroSkinDatabin.Unload();
			GameDataMgr.robotVeteranHeroSkinDatabin.Unload();
			GameDataMgr.banHeroBin.Unload();
			GameDataMgr.commonRewardDatabin.Unload();
			GameDataMgr.unionRankRewardDetailDatabin.Unload();
			GameDataMgr.unionBattleWinCntRewardDatabin.Unload();
			GameDataMgr.speakerDatabin.Unload();
			GameDataMgr.creditLevelDatabin.Unload();
			GameDataMgr.resHonor.Unload();
			GameDataMgr.floatTextDatabin.Unload();
			GameDataMgr.recruimentReward.Unload();
			GameDataMgr.killNotifyDatabin.Unload();
			GameDataMgr.continuKillDatabin.Unload();
			GameDataMgr.multiKillDatabin.Unload();
			GameDataMgr.voiceInteractionDatabin.Unload();
			GameDataMgr.aiParamConfDataBin.Unload();
			GameDataMgr.famousMentorDatabin.Unload();
			GameDataMgr.speedAdjusterDatabin.Unload();
			GameDataMgr.deadInfoTextDatabin.Unload();
			GameDataMgr.deadInfoConditionDatabin.Unload();
		}

		public void UnloadReducedDatabin()
		{
			GameDataMgr.skillDatabin.Unload();
			GameDataMgr.skillCombineDatabin.Unload();
			GameDataMgr.skillPassiveDatabin.Unload();
		}

		public void ReloadDataBinOnFighting()
		{
			this.ReloadCommonDatabin();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsGameTypeGuide())
			{
				this.ReloadGuideDatabin();
			}
			if (curLvelContext.IsGameTypeAdventure())
			{
				this.ReloadPveDatabin();
			}
		}

		private void ReloadCommonDatabin()
		{
			GameDataMgr.skillDatabin.Reload();
			GameDataMgr.monsterDatabin.Reload();
			GameDataMgr.organDatabin.Reload();
			GameDataMgr.skillCombineDatabin.Reload();
			GameDataMgr.soldierWaveDatabin.Reload();
			GameDataMgr.levelDatabin.Reload();
			GameDataMgr.pvpLevelDatabin.Reload();
			GameDataMgr.battleDynamicPropertyDB.Reload();
			GameDataMgr.clashAdditionDB.Reload();
			GameDataMgr.actorLinesDatabin.Reload();
			GameDataMgr.acntExpDatabin.Reload();
			GameDataMgr.globalInfoDatabin.Reload();
			GameDataMgr.randomSkillPassiveDatabin.Reload();
			GameDataMgr.skillPassiveDatabin.Reload();
			GameDataMgr.skillMarkDatabin.Reload();
			GameDataMgr.battleParam.Reload();
			GameDataMgr.propertyValInfo.Reload();
			GameDataMgr.signalDatabin.Reload();
			GameDataMgr.m_equipInBattleDatabin.Reload();
			GameDataMgr.soulLvlUpDatabin.Reload();
			GameDataMgr.soulAdditionDatabin.Reload();
			GameDataMgr.voiceInteractionDatabin.Reload();
			GameDataMgr.incomeAllocDatabin.Reload();
			GameDataMgr.specialFunUnlockDatabin.Reload();
			GameDataMgr.unlockConditionDatabin.Reload();
			GameDataMgr.symbolInfoDatabin.Reload();
			GameDataMgr.evaluateCondInfoDatabin.Reload();
			GameDataMgr.floatTextDatabin.Reload();
			GameDataMgr.famousMentorDatabin.Reload();
			GameDataMgr.speedAdjusterDatabin.Reload();
			GameDataMgr.deadInfoConditionDatabin.Reload();
			GameDataMgr.deadInfoTextDatabin.Reload();
			GameDataMgr.inBattleMsgDatabin.Reload();
			GameDataMgr.inBattleChannelDatabin.Reload();
		}

		private void ReloadPveDatabin()
		{
		}

		private void ReloadGuideDatabin()
		{
			GameDataMgr.guideTipDatabin.Reload();
			GameDataMgr.newbieSpecialTipDatabin.Reload();
			GameDataMgr.newbieWeakMainLineDataBin.Reload();
			GameDataMgr.newbieWeakDatabin.Reload();
		}

		public IEnumerator LoadDataBin()
		{
			GameDataMgr.heroDatabin = new DatabinTable<ResHeroCfgInfo, uint>("Databin/Public/Actor/hero.bytes", "dwCfgID");
			GameDataMgr.settleDatabin = new DatabinTable<ResCommonSettle, uint>("Databin/Public/Global/CommonSettle.bytes", "dwSettleID");
			GameDataMgr.heroEnergyDatabin = new DatabinTable<ResHeroEnergyInfo, uint>("Databin/Public/Actor/heroEnergy.bytes", "dwEnergyID");
			GameDataMgr.skillDatabin = new DatabinTable<ResSkillCfgInfo, int>("Databin/Public/Skill/skill.bytes", "iCfgID");
			GameDataMgr.skillBeanDatabin = new DatabinTable<ResSkillBeanCfgInfo, uint>("Databin/Public/Skill/skillBean.bytes", "dwConfigID");
			GameDataMgr.monsterDatabin = new DatabinTable<ResMonsterCfgInfo, long>("Databin/Client/Actor/normalMonster.bytes", "iCfgID", "iDifficultyLevel");
			yield return null;

			GameDataMgr.organDatabin = new DatabinTable<ResOrganCfgInfo, long>("Databin/Client/Actor/organ.bytes", "iCfgID", "iDifficultyLevel");
			GameDataMgr.monsterOrganLvDynamicInfobin = new DatabinTable<ResMonsterOrganLevelDynamicInfo, int>("Databin/Client/Actor/monsterOrganLevelDynamicInfo.bytes", "iLevel");
			GameDataMgr.skillCombineDatabin = new DatabinTable<ResSkillCombineCfgInfo, int>("Databin/Public/Skill/skillcombine.bytes", "iCfgID");
			yield return null;

			GameDataMgr.soldierWaveDatabin = new DatabinTable<ResSoldierWaveInfo, uint>("Databin/Client/Actor/soldierwave.bytes", "dwSoldierWaveID");
			yield return null;

			GameDataMgr.heroLvlUpDatabin = new DatabinTable<ResHeroLvlUpInfo, uint>("Databin/Public/Actor/heroLvlUp.bytes", "dwLevelID");
			GameDataMgr.itemDatabin = new DatabinTable<ResPropInfo, uint>("Databin/Public/Item/Item.bytes", "dwID");
			GameDataMgr.levelDatabin = new DatabinTable<ResLevelCfgInfo, int>("Databin/Public/Level/Level.bytes", "iCfgID");
			GameDataMgr.towerHitDatabin = new DatabinTable<TowerHitConf, byte>("Databin/Client/Actor/TowerHit.bytes", "bOrganType");
			GameDataMgr.pvpLevelDatabin = new DatabinTable<ResAcntBattleLevelInfo, uint>("Databin/Public/Level/AcntBattleLevel.bytes", "dwMapId");
			GameDataMgr.entertainLevelDatabin = new DatabinTable<ResEntertainmentLevelInfo, uint>("Databin/Public/Level/EntertainmentLevel.bytes", "dwMapId");
			yield return null;

			GameDataMgr.rankLevelDatabin = new DatabinTable<ResRankLevelInfo, uint>("Databin/Public/Level/RankLevel.bytes", "dwMapId");
			GameDataMgr.battleDynamicPropertyDB = new DatabinTable<ResBattleDynamicProperty, uint>("Databin/Client/Level/BattleDynamicProperty.bytes", "dwKey");
			GameDataMgr.battleDynamicDifficultyDB = new DatabinTable<ResBattleDynamicDifficulty, uint>("Databin/Public/Level/ResBattleDynamicDifficulty.bytes", "dwID");
			GameDataMgr.clashAdditionDB = new DatabinTableMulti<ResClashAddition, uint>("Databin/Client/Level/ClashAddition.bytes", "dwAttackerMark");
			yield return null;

			GameDataMgr.actorLinesDatabin = new DatabinTableMulti<ResActorLinesInfo, int>("Databin/Client/Level/ActorLines.bytes", "iCfgID");
			GameDataMgr.acntExpDatabin = new DatabinTable<ResAcntExpInfo, uint>("Databin/Public/Global/AcntExpInfo.bytes", "dwLevel");
			GameDataMgr.globalInfoDatabin = new DatabinTable<ResGlobalInfo, uint>("Databin/Public/Global/GlobalConfInfo.bytes", "dwConfType");
			GameDataMgr.taskDatabin = new DatabinTable<ResTask, uint>("Databin/Public/Task/task.bytes", "dwTaskID");
			yield return null;

			GameDataMgr.taskPrerequisiteDatabin = new DatabinTable<ResPrerequisite, uint>("Databin/Public/Task/prerequisite.bytes", "dwPrerequisiteType");
			GameDataMgr.taskRewardDatabin = new DatabinTable<ResTaskReward, uint>("Databin/Public/Task/taskreward.bytes", "dwTaskRewardID");
			GameDataMgr.resPvpLevelRewardDatabin = new DatabinTable<ResPvpLevelReward, int>("Databin/Public/Global/PvpLevelReward.bytes", "iLevel");
			GameDataMgr.equipInfoDatabin = new DatabinTable<ResEquipInfo, uint>("Databin/Public/Item/Equip.bytes", "dwID");
			GameDataMgr.composeInfoDatabin = new DatabinTable<ResComposeInfo, uint>("Databin/Public/Item/ItemComp.bytes", "dwCompID");
			yield return null;

			GameDataMgr.bufDropInfoDatabin = new DatabinTable<ResBufDropInfo, uint>("Databin/Client/Level/BufDrop.bytes", "dwID");
			GameDataMgr.evaluateCondInfoDatabin = new DatabinTable<ResEvaluateStarInfo, uint>("Databin/Public/Level/EvaluateCond.bytes", "dwCondID");
			GameDataMgr.addWinLoseCondDatabin = new DatabinTable<ResEvaluateStarInfo, uint>("Databin/Public/Level/AddWinLoseCond.bytes", "dwCondID");
			GameDataMgr.chapterInfoDatabin = new DatabinTable<ResChapterInfo, uint>("Databin/Public/Level/Chapter.bytes", "dwChapterId");
			GameDataMgr.textBubbleDatabin = new DatabinTable<ResTextData, uint>("Databin/Client/Level/TextBubble.bytes", "dwTag");
			yield return null;

			GameDataMgr.guideTipDatabin = new DatabinTable<ResGuideTipInfo, int>("Databin/Client/Level/GuideTip.bytes", "iCfgID");
			GameDataMgr.resShopInfoDatabin = new DatabinTable<ResShopInfo, uint>("Databin/Public/Shop/ShopBuy.bytes", "dwID");
			GameDataMgr.coninBuyDatabin = new DatabinTable<ResCoinBuyInfo, ushort>("Databin/Public/Shop/CoinBuy.bytes", "wBuyFreq");
			yield return null;

			GameDataMgr.s_textDatabin = new DatabinTable<ResText, ushort>("Databin/Client/Text/Text.bytes", "wID");
			GameDataMgr.s_ruleTextDatabin = new DatabinTable<ResRuleText, ushort>("Databin/Client/Text/RuleText.bytes", "wID");
			GameDataMgr.randomSkillPassiveDatabin = new DatabinTable<ResRandomSkillPassiveRule, int>("Databin/Client/Skill/randompassive.bytes", "iRandomSkillPassiveKey");
			GameDataMgr.skillPassiveDatabin = new DatabinTable<ResSkillPassiveCfgInfo, int>("Databin/Client/Skill/skillpassive.bytes", "iCfgID");
			GameDataMgr.skillMarkDatabin = new DatabinTable<ResSkillMarkCfgInfo, int>("Databin/Client/Skill/skillmark.bytes", "iCfgID");
			yield return null;

			GameDataMgr.unlockConditionDatabin = new DatabinTable<ResUnlockCondition, uint>("Databin/Public/Global/unlockcondition.bytes", "dwUnlockID");
			GameDataMgr.specialFunUnlockDatabin = new DatabinTable<ResSpecialFucUnlock, uint>("Databin/Public/Global/specialfucunlock.bytes", "dwFucID");
			GameDataMgr.licenseDatabin = new DatabinTable<ResLicenseInfo, uint>("Databin/Public/Global/license.bytes", "dwLicenseID");
			yield return null;

			GameDataMgr.newbieMainLineDatabin = new DatabinTable<NewbieGuideMainLineConf, uint>("Databin/Client/NewbieGuide/NewbieGuideMainLineConf.bytes", "dwID");
			GameDataMgr.newbieWeakMainLineDataBin = new DatabinTable<NewbieWeakGuideMainLineConf, uint>("Databin/Client/NewbieGuide/NewbieWeakGuideMainLineConf.bytes", "dwID");
			GameDataMgr.newbieWeakDatabin = new DatabinTable<NewbieGuideWeakConf, uint>("Databin/Client/NewbieGuide/NewbieGuideWeakConf.bytes", "dwID");
			GameDataMgr.newbieScriptDatabin = new DatabinTableMulti<NewbieGuideScriptConf, uint>("Databin/Client/NewbieGuide/NewbieGuideScriptConf.bytes", "dwID");
			yield return null;

			GameDataMgr.newbieSpecialTipDatabin = new DatabinTable<NewbieGuideSpecialTipConf, uint>("Databin/Client/NewbieGuide/NewbieGuideSpecialTipConf.bytes", "dwID");
			GameDataMgr.newbieBannerGuideDatabin = new DatabinTable<NewbieGuideBannerGuideConf, ushort>("Databin/Client/NewbieGuide/NewbieGuideBannerGuideConf.bytes", "wID");
			yield return null;

			GameDataMgr.soulLvlUpDatabin = new DatabinTableMulti<ResSoulLvlUpInfo, uint>("Databin/Client/Soul/SoulLvlUp.bytes", "dwSoulAllocId");
			GameDataMgr.incomeAllocDatabin = new DatabinTable<ResIncomeAllocRule, uint>("Databin/Client/Soul/SoulExpAllocRule.bytes", "dwID");
			GameDataMgr.shopTypeDatabin = new DatabinTable<ResShopType, ushort>("Databin/Public/Shop/ShopType.bytes", "wShopType");
			yield return null;

			GameDataMgr.shopRefreshCostDatabin = new DatabinTable<ResShopRefreshCost, uint>("Databin/Public/Shop/RefreshCost.bytes", "dwID");
			GameDataMgr.heroProficiencyDatabin = new DatabinTableMulti<ResHeroProficiency, byte>("Databin/Public/Actor/heroProficiency.bytes", "bLevel");
			GameDataMgr.rankGradeDatabin = new DatabinTable<ResRankGradeConf, byte>("Databin/Public/Rank/RankGrade.bytes", "bGrade");
			GameDataMgr.rankRewardDatabin = new DatabinTable<ResRankRewardConf, uint>("Databin/Public/Rank/RankSeasonReward.bytes", "dwGradeId");
			GameDataMgr.soulAdditionDatabin = new DatabinTableMulti<ResSoulAddition, int>("Databin/Client/Soul/SoulAddition.bytes", "dwIncomeAllocId");
			yield return null;

			GameDataMgr.symbolInfoDatabin = new DatabinTable<ResSymbolInfo, uint>("Databin/Public/Item/Symbol.bytes", "dwID");
			GameDataMgr.symbolPosDatabin = new DatabinTable<ResSymbolPos, byte>("Databin/Public/Item/SymbolPos.bytes", "bSymbolPos");
			GameDataMgr.heroSymbolLvlDatabin = new DatabinTable<ResHeroSymbolLvl, ushort>("Databin/Public/Item/HeroSymbol.bytes", "wPvpLevel");
			GameDataMgr.symbolRcmdDatabin = new DatabinTableMulti<ResSymbolRcmd, uint>("Databin/Public/Item/SymbolRecmd.bytes", "dwRcmdID");
			yield return null;

			GameDataMgr.gameTaskDatabin = new DatabinTable<ResGameTask, uint>("Databin/Client/Battle/GameTask.bytes", "dwID");
			GameDataMgr.gameTaskGroupDatabin = new DatabinTable<ResGameTaskGroup, uint>("Databin/Client/Battle/GameTaskGroup.bytes", "dwKey");
			GameDataMgr.shenfuBin = new DatabinTable<ShenFuInfo, uint>("Databin/Client/Battle/ShenFuInfo.bytes", "dwShenFuId");
			GameDataMgr.charmLib = new DatabinTable<CharmLib, uint>("Databin/Client/Battle/CharmLib.bytes", "dwPoolId");
			GameDataMgr.guildMiscDatabin = new DatabinTable<ResGuildMisc, uint>("Databin/Public/Guild/GuildMisc.bytes", "dwConfType");
			GameDataMgr.guildIconDatabin = new DatabinTable<ResGuildIcon, uint>("Databin/Public/Guild/GuildIcon.bytes", "dwIcon");
			yield return null;

			GameDataMgr.guildBuildingDatabin = new DatabinTable<ResGuildBuilding, byte>("Databin/Public/Guild/GuildBuilding.bytes", "bBuildingType");
			GameDataMgr.guildLevelDatabin = new DatabinTable<ResGuildLevel, byte>("Databin/Public/Guild/GuildLevel.bytes", "bGuildLevel");
			GameDataMgr.guildDonateDatabin = new DatabinTable<ResGuildDonate, byte>("Databin/Public/Guild/GuildDonate.bytes", "bDonateType");
			GameDataMgr.guildRankRewardDatabin = new DatabinTable<ResGuildRankReward, int>("Databin/Public/Guild/GuildRankReward.bytes", "iStartRankNo");
			GameDataMgr.guildGradeDatabin = new DatabinTable<ResGuildGradeConf, byte>("Databin/Public/Guild/GuildGrade.bytes", "bIndex");
			GameDataMgr.guildStarLevel = new DatabinTable<ResGuildShopStarIndexConf, uint>("Databin/Public/Guild/GuildShopStarIndex.bytes", "dwBeginStar");
			yield return null;

			GameDataMgr.talentLib = new DatabinTable<ResTalentLib, uint>("Databin/Client/Talent/TalentLib.bytes", "dwID");
			GameDataMgr.miShuLib = new DatabinTable<ResMiShuInfo, uint>("Databin/Client/Talent/MiShuInfo.bytes", "dwID");
			GameDataMgr.acntPvpExpDatabin = new DatabinTable<ResAcntPvpExpInfo, byte>("Databin/Public/Global/AcntPvpExpInfo.bytes", "bLevel");
			yield return null;

			GameDataMgr.burnRewrad = new DatabinTableMulti<ResBurningReward, uint>("Databin/Public/Global/BurningReward.bytes", "dwAcntLevel");
			GameDataMgr.burnMap = new DatabinTable<ResLevelCfgInfo, int>("Databin/Public/Level/BurningLevel.bytes", "iCfgID");
			GameDataMgr.burnBuffMap = new DatabinTable<ResBurningBuff, int>("Databin/Public/Skill/BurningBuff.bytes", "iSkillCombineID");
			yield return null;

			GameDataMgr.battleParam = new DatabinTable<ResBattleParam, uint>("Databin/Client/Actor/battleParam.bytes", "dwID");
			GameDataMgr.propertyValInfo = new DatabinTable<ResPropertyValueInfo, byte>("Databin/Client/Actor/PropertyValueInfo.bytes", "bPropertyType");
			GameDataMgr.heroSkinDatabin = new DatabinTable<ResHeroSkin, uint>("Databin/Public/Actor/heroSkin.bytes", "dwID");
			GameDataMgr.skinQualityPicDatabin = new DatabinTable<ResSkinQualityPicInfo, byte>("Databin/Client/Actor/skinQualityPic.bytes", "bQualityId");
			GameDataMgr.npcOfArena = new DatabinTable<ResNpcOfArena, uint>("Databin/Public/Arena/NpcOfArena", "dwObjId");
			yield return null;

			GameDataMgr.robotBattleListInfo = new DatabinTable<ResRobotBattleList, uint>("Databin/Public/Global/RobotBattleList", "dwBattleListID");
			GameDataMgr.robotHeroInfo = new DatabinTable<ResRobotPower, uint>("Databin/Public/Global/RobotPower", "dwId");
			GameDataMgr.robotName = new DatabinTable<ResRobotName, uint>("Databin/Public/Global/RobotName", "dwObjId");
			GameDataMgr.robotSubNameA = new DatabinTable<ResRobotSubNameA, uint>("Databin/Public/Global/RobotSubNameA", "dwObjId");
			yield return null;

			GameDataMgr.robotSubNameB = new DatabinTable<ResRobotSubNameB, uint>("Databin/Public/Global/RobotSubNameB", "dwObjId");
			GameDataMgr.robotSubNameC = new DatabinTable<ResRobotSubNameC, uint>("Databin/Public/Global/RobotSubNameC", "dwObjId");
			GameDataMgr.arenaLevelDatabin = new DatabinTable<ResLevelCfgInfo, int>("Databin/Public/Level/ArenaLevel", "iCfgID");
			GameDataMgr.arenaRewardDatabin = new DatabinTable<ResArenaOneDayReward, uint>("Databin/Public/Arena/ArenaOneDayReward", "dwRankStart");
			GameDataMgr.randomRewardDB = new DatabinTable<ResRandomRewardStore, uint>("Databin/Public/Global/RandomRewardStore", "dwRewardID");
			yield return null;

			GameDataMgr.signalDatabin = new DatabinTable<ResSignalInfo, byte>("Databin/Client/Battle/SignalPanel", "bSignalID");
			GameDataMgr.cdDatabin = new DatabinTable<ResClrCD, uint>("Databin/Public/Shop/ClrCd", "dwType");
			GameDataMgr.inBattleMsgDatabin = new DatabinTable<ResInBatMsgCfg, uint>("Databin/Public/Battle/InBattleMsgCfg.bytes", "dwID");
			GameDataMgr.inBattleChannelDatabin = new DatabinTable<ResInBatChannelCfg, byte>("Databin/Public/Battle/InBattleChannelCfg.bytes", "bID");
			GameDataMgr.inBattleHeroActDatabin = new DatabinTable<ResInBatMsgHeroActCfg, uint>("Databin/Client/Battle/InBattleMsgHeroAct.bytes", "dwID");
			GameDataMgr.inBattleDefaultDatabin = new DatabinTable<ResShortcutDefault, uint>("Databin/Client/Battle/InBattleMsgDefault.bytes", "dwID");
			yield return null;

			GameDataMgr.iosDianQuanBuyInfo = new DatabinTable<ResCouponsBuyInfo, uint>("Databin/Client/Pay/IOSDiamondBuyInfo", "dwID");
			GameDataMgr.androidDianQuanBuyInfo = new DatabinTable<ResCouponsBuyInfo, uint>("Databin/Client/Pay/AndroidDiamondBuyInfo", "dwID");
			GameDataMgr.cpLevelDatabin = new DatabinTable<ResCounterPartLevelInfo, uint>("Databin/Public/Level/CounterPartLevel", "dwMapId");
			GameDataMgr.achieveDatabin = new DatabinTable<ResAchievement, uint>("Databin/Public/Achievement/Achievement.bytes", "dwID");
			GameDataMgr.trophyDatabin = new DatabinTable<ResTrophyLvl, uint>("Databin/Public/Achievement/TrophyLvlReward.bytes", "dwTrophyLvl");
			GameDataMgr.addedSkiilDatabin = new DatabinTable<ResSkillUnlock, ushort>("Databin/Public/Skill/SkillUnlock.bytes", "wAcntLevel");
			GameDataMgr.callMonsterDatabin = new DatabinTable<ResCallMonster, ushort>("Databin/Public/Skill/CallMonster.bytes", "wConfigID");
			GameDataMgr.resVipDianQuan = new DatabinTable<ResVIPCoupons, uint>("Databin/Public/Global/VIPCoupons.bytes", "dwVIPLevel");
			GameDataMgr.heroAwakDatabin = new DatabinTable<ResHeroWakeInfo, long>("Databin/Public/Actor/herowake.bytes", "dwHeroID", "dwStepID");
			GameDataMgr.resNobeInfoDatabin = new DatabinTable<ResNobeInfo, byte>("Databin/Public/Global/ResNobeInfo.bytes", "dwID");
			GameDataMgr.m_selectHeroChatDatabin = new DatabinTable<ResHeroSelectTextData, uint>("Databin/Client/Level/SelectHeroChat.bytes", "dwID");
			GameDataMgr.m_equipInBattleDatabin = new DatabinTable<ResEquipInBattle, ushort>("Databin/Public/Battle/EquipInBattle", "wID");
			GameDataMgr.m_recommendEquipInBattleDatabin = new DatabinTable<ResRecommendEquipInBattle, long>("Databin/Public/Battle/RecommendEquipInBattle", "wHeroID", "wCombinationID");
			GameDataMgr.m_recommendEquipJudge = new DatabinTable<ResEquipEval, byte>("Databin/Public/Battle/EquipEval", "bEvalID");
			GameDataMgr.changeNameDatabin = new DatabinTable<ResChangeName, uint>("Databin/Public/Global/ChangeName", "dwIndex");
			GameDataMgr.banHeroBin = new DatabinTable<ResBanHeroConf, long>("Databin/Public/Level/BanHero.bytes", "bMapType", "dwMapId");
			GameDataMgr.uinionBattleLevelDatabin = new DatabinTable<ResRewardMatchLevelInfo, uint>("Databin/Public/Level/RewardMatchLevel.bytes", "dwMapId");
			GameDataMgr.guildMatchLevelDatabin = new DatabinTable<ResGuildMatchLevelInfo, uint>("Databin/Public/Level/GuildMatchLevel.bytes", "dwMapId");
			GameDataMgr.commonRewardDatabin = new DatabinTable<ResCommReward, uint>("Databin/Public/Global/CommReward.bytes", "dwRewardID");
			GameDataMgr.unionRankRewardDetailDatabin = new DatabinTable<ResRewardMatchDetailConf, uint>("Databin/Public/Global/RewardMatchDetail.bytes", "dwId");
			GameDataMgr.unionBattleWinCntRewardDatabin = new DatabinTable<ResRewardMatchReward, long>("Databin/Public/Global/ResRewardMatchReward.bytes", "dwMapId", "bWinCnt");
			GameDataMgr.speakerDatabin = new DatabinTable<ResHornInfo, uint>("Databin/Public/Item/Horn.bytes", "dwID");
			yield return null;

			GameDataMgr.robotPlayerSkillDatabin = new DatabinTable<ResFakeAcntSkill, ushort>("Databin/Public/WarmBattle/FakeAcntSkill", "dwLevel");
			GameDataMgr.robotRookieHeroSkinDatabin = new DatabinTable<ResFakeAcntHero, ushort>("Databin/Public/WarmBattle/ResFakeAcntHeroLowLevel", "dwHeroID");
			GameDataMgr.robotVeteranHeroSkinDatabin = new DatabinTable<ResFakeAcntHero, ushort>("Databin/Public/WarmBattle/ResFakeAcntHeroHighLevel", "dwHeroID");
			yield return null;

			GameDataMgr.creditLevelDatabin = new DatabinTable<ResCreditLevelInfo, uint>("Databin/Public/Credit/creditLevel.bytes", "dwID");
			GameDataMgr.resHonor = new DatabinTable<ResHonor, int>("Databin/Public/Global/Honor.bytes", "iID");
			GameDataMgr.floatTextDatabin = new DatabinTable<ResBattleFloatText, uint>("Databin/Client/Battle/BattleFloatText.bytes", "dwID");
			GameDataMgr.recruimentReward = new DatabinTable<ResRecruitmentReward, ushort>("Databin/Public/Global/RecruitmentReward.bytes", "wID");
			GameDataMgr.killNotifyDatabin = new DatabinTable<ResKNPriority, uint>("Databin/Client/Battle/KNPriority.bytes", "dwKNValue");
			GameDataMgr.continuKillDatabin = new DatabinTable<ResContinuKill, byte>("Databin/Client/Text/ContinuKillText.bytes", "bKillCount");
			GameDataMgr.multiKillDatabin = new DatabinTable<ResMultiKill, byte>("Databin/Client/Text/MultiKillText.bytes", "bKillCount");
			GameDataMgr.voiceInteractionDatabin = new DatabinTable<ResVoiceInteraction, uint>("Databin/Client/Battle/VoiceInteraction.bytes", "dwConfigID");
			GameDataMgr.aiParamConfDataBin = new DatabinTable<ResAiParamConf, ushort>("Databin/Client/Battle/AIParamCfg.bytes", "wId");
			yield return null;

			GameDataMgr.deadInfoTextDatabin = new DatabinTable<ResDeadInfoText, byte>("Databin/Client/Text/DeadInfoText.bytes", "bId");
			GameDataMgr.deadInfoConditionDatabin = new DatabinTable<ResDeadInfoCondition, byte>("Databin/Client/Battle/DeadInfoCondition.bytes", "bConditionId");
			GameDataMgr.famousMentorDatabin = new DatabinTable<ResFamousMentor, uint>("Databin/Public/Mentor/FamousMentor.bytes", "dwID");
			GameDataMgr.speedAdjusterDatabin = new DatabinTable<ResSpeedAdjustConfig, byte>("Databin/Client/Actor/SpeedAdjuster.bytes", "bType");
			GameDataMgr.socialEnumDatabin = new DatabinTable<ResSocialEnum, byte>("Databin/Public/Social/SocialEnum.bytes", "dwID");
			GameDataMgr.socialTagsDataBin = new DatabinTable<ResSocialTags, int>("Databin/Public/Social/SocialTags.bytes", "dwID");
			GameDataMgr.askForTemplateDatabin = new DatabinTable<ResAskforTemplet, long>("Databin/Public/Actor/AskforTemp.bytes", "dwID", "wItemType");
		}

		[MessageHandler(2510)]
		public static void OnServerResDataNtf(CSPkg msg)
		{
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref msg.stPkgData.stResDataNtf.szResData, (int)msg.stPkgData.stResDataNtf.dwDataLen);
			byte b = 0;
			ushort step = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			while (tdrReadBuf.readUInt8(ref b) == TdrError.ErrorType.TDR_NO_ERROR)
			{
				CS_RES_DATA_TYPE cS_RES_DATA_TYPE = (CS_RES_DATA_TYPE)b;
				if (tdrReadBuf.readUInt16(ref step) != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return;
				}
				switch (cS_RES_DATA_TYPE)
				{
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_CHECKIN:
				{
					ResWealCheckIn resWealCheckIn = new ResWealCheckIn();
					if (resWealCheckIn.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealCheckInDict.ContainsKey(resWealCheckIn.dwID))
						{
							GameDataMgr.wealCheckInDict.Add(resWealCheckIn.dwID, resWealCheckIn);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_FILLINPRICE:
				{
					ResFillInPrice resFillInPrice = new ResFillInPrice();
					if (resFillInPrice.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealCheckFillDict.ContainsKey(resFillInPrice.dwID))
						{
							GameDataMgr.wealCheckFillDict.Add(resFillInPrice.dwID, resFillInPrice);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_FIXEDTIME:
				{
					ResWealFixedTime resWealFixedTime = new ResWealFixedTime();
					if (resWealFixedTime.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealFixtimeDict.ContainsKey(resWealFixedTime.dwID))
						{
							GameDataMgr.wealFixtimeDict.Add(resWealFixedTime.dwID, resWealFixedTime);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_MULTIPLE:
				{
					ResWealMultiple resWealMultiple = new ResWealMultiple();
					if (resWealMultiple.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealMultipleDict.ContainsKey(resWealMultiple.dwID))
						{
							GameDataMgr.wealMultipleDict.Add(resWealMultiple.dwID, resWealMultiple);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_CONDITION:
				{
					ResWealCondition resWealCondition = new ResWealCondition();
					if (resWealCondition.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealConditionDict.ContainsKey(resWealCondition.dwID))
						{
							GameDataMgr.wealConditionDict.Add(resWealCondition.dwID, resWealCondition);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_TEXT:
				{
					ResWealText resWealText = new ResWealText();
					if (resWealText.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealNoticeDict.ContainsKey(resWealText.dwID))
						{
							GameDataMgr.wealNoticeDict.Add(resWealText.dwID, resWealText);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_RANDOM_REWARD:
				{
					ResRandomRewardStore resRandomRewardStore = new ResRandomRewardStore();
					if (resRandomRewardStore.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						GameDataMgr.randomRewardDB.UpdataData(resRandomRewardStore.dwRewardID, resRandomRewardStore);
						GameDataMgr.randomRewardDB.isAllowUnLoad = false;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_SPEC_SALE:
				{
					ResSpecSale resSpecSale = new ResSpecSale();
					if (resSpecSale.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.specSaleDict.ContainsKey(resSpecSale.dwId))
						{
							GameDataMgr.specSaleDict.Add(resSpecSale.dwId, resSpecSale);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_HERO_PROMOTION:
				{
					ResHeroPromotion resHeroPromotion = new ResHeroPromotion();
					if (resHeroPromotion.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.heroPromotionDict.ContainsKey(resHeroPromotion.dwPromotionID))
						{
							GameDataMgr.heroPromotionDict.Add(resHeroPromotion.dwPromotionID, resHeroPromotion);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_SKIN_PROMOTION:
				{
					ResSkinPromotion resSkinPromotion = new ResSkinPromotion();
					if (resSkinPromotion.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.skinPromotionDict.ContainsKey(resSkinPromotion.dwPromotionID))
						{
							GameDataMgr.skinPromotionDict.Add(resSkinPromotion.dwPromotionID, resSkinPromotion);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_LUCKYDRAW_PRICE:
				{
					ResLuckyDrawPrice resLuckyDrawPrice = new ResLuckyDrawPrice();
					if (resLuckyDrawPrice.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						enPayType key = CMallSystem.ResBuyTypeToPayType((int)resLuckyDrawPrice.bMoneyType);
						if (!GameDataMgr.mallRoulettePriceDict.ContainsKey(key))
						{
							GameDataMgr.mallRoulettePriceDict.Add(key, resLuckyDrawPrice);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_LUCKYDRAW_REWARD:
				{
					ResLuckyDrawRewardForClient resLuckyDrawRewardForClient = new ResLuckyDrawRewardForClient();
					if (resLuckyDrawRewardForClient.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						long doubleKey = GameDataMgr.GetDoubleKey(resLuckyDrawRewardForClient.dwRewardPoolID, (uint)resLuckyDrawRewardForClient.bRewardIndex);
						if (!GameDataMgr.mallRouletteRewardDict.ContainsKey(doubleKey))
						{
							GameDataMgr.mallRouletteRewardDict.Add(doubleKey, resLuckyDrawRewardForClient);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_LUCKYDRAW_EXTERNREWARD:
				{
					ResLuckyDrawExternReward resLuckyDrawExternReward = new ResLuckyDrawExternReward();
					if (resLuckyDrawExternReward.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.mallRouletteExternRewardDict.ContainsKey((uint)resLuckyDrawExternReward.bMoneyType))
						{
							GameDataMgr.mallRouletteExternRewardDict.Add((uint)resLuckyDrawExternReward.bMoneyType, resLuckyDrawExternReward);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_SALE_RECOMMEND:
				{
					ResSaleRecommend resSaleRecommend = new ResSaleRecommend();
					if (resSaleRecommend.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.recommendProductDict.ContainsKey(resSaleRecommend.dwID))
						{
							GameDataMgr.recommendProductDict.Add(resSaleRecommend.dwID, resSaleRecommend);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_RAND_DRAW:
				{
					ResRandDrawInfo resRandDrawInfo = new ResRandDrawInfo();
					if (resRandDrawInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.recommendLotteryCtrlDict.ContainsKey(resRandDrawInfo.dwDrawID))
						{
							GameDataMgr.recommendLotteryCtrlDict.Add(resRandDrawInfo.dwDrawID, resRandDrawInfo);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_REWARDPOOL:
				{
					ResRewardPoolInfo resRewardPoolInfo = new ResRewardPoolInfo();
					if (resRewardPoolInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						long doubleKey2 = GameDataMgr.GetDoubleKey(resRewardPoolInfo.dwPoolID, (uint)resRewardPoolInfo.stRewardInfo.bIndex);
						if (!GameDataMgr.recommendRewardDict.ContainsKey(doubleKey2))
						{
							GameDataMgr.recommendRewardDict.Add(doubleKey2, resRewardPoolInfo);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_REDDOTTIPINFO:
				{
					ResRedDotInfo resRedDotInfo = new ResRedDotInfo();
					if (resRedDotInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.redDotInfoDict.ContainsKey(resRedDotInfo.dwIndex))
						{
							GameDataMgr.redDotInfoDict.Add(resRedDotInfo.dwIndex, resRedDotInfo);
						}
						flag4 = true;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_HEROSHOP:
				{
					ResHeroShop resHeroShop = new ResHeroShop();
					if (resHeroShop.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.heroShopInfoDict.ContainsKey(resHeroShop.dwCfgID))
						{
							GameDataMgr.heroShopInfoDict.Add(resHeroShop.dwCfgID, resHeroShop);
						}
						GameDataMgr.heroDatabin.isAllowUnLoad = false;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_HEROSKINSHOP:
				{
					ResHeroSkinShop resHeroSkinShop = new ResHeroSkinShop();
					if (resHeroSkinShop.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						flag = true;
						if (!GameDataMgr.skinShopInfoDict.ContainsKey(resHeroSkinShop.dwID))
						{
							GameDataMgr.skinShopInfoDict.Add(resHeroSkinShop.dwID, resHeroSkinShop);
						}
						GameDataMgr.heroSkinDatabin.isAllowUnLoad = false;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_EXCHANGE:
				{
					ResCltWealExchange resCltWealExchange = new ResCltWealExchange();
					if (resCltWealExchange.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealExchangeDict.ContainsKey(resCltWealExchange.dwID))
						{
							GameDataMgr.wealExchangeDict.Add(resCltWealExchange.dwID, resCltWealExchange);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_PVP_SPECITEM:
				{
					ResPVPSpecItem resPVPSpecItem = new ResPVPSpecItem();
					if (resPVPSpecItem.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.pvpSpecialItemDict.ContainsKey(resPVPSpecItem.dwID))
						{
							GameDataMgr.pvpSpecialItemDict.Add(resPVPSpecItem.dwID, resPVPSpecItem);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_REWARDMATCH_TIME:
				{
					ResRewardMatchTimeInfo resRewardMatchTimeInfo = new ResRewardMatchTimeInfo();
					if (resRewardMatchTimeInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						long doubleKey3 = GameDataMgr.GetDoubleKey((uint)resRewardMatchTimeInfo.bMapType, resRewardMatchTimeInfo.dwMapId);
						if (!GameDataMgr.matchTimeInfoDict.ContainsKey(doubleKey3))
						{
							GameDataMgr.matchTimeInfoDict.Add(doubleKey3, resRewardMatchTimeInfo);
						}
						flag5 = true;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_HUOYUEDU_REWARD:
				{
					ResHuoYueDuReward resHuoYueDuReward = new ResHuoYueDuReward();
					if (resHuoYueDuReward.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.huoyueduDict.ContainsKey(resHuoYueDuReward.wID))
						{
							GameDataMgr.huoyueduDict.Add(resHuoYueDuReward.wID, resHuoYueDuReward);
						}
						else
						{
							GameDataMgr.huoyueduDict[resHuoYueDuReward.wID] = resHuoYueDuReward;
						}
						Singleton<CTaskSys>.instance.model.huoyue_data.ParseHuoyuedu(resHuoYueDuReward);
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_BOUTIQUE:
				{
					ResBoutiqueConf resBoutiqueConf = new ResBoutiqueConf();
					if (resBoutiqueConf.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.boutiqueDict.ContainsKey(resBoutiqueConf.dwID))
						{
							GameDataMgr.boutiqueDict.Add(resBoutiqueConf.dwID, resBoutiqueConf);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_HEADIMAGE:
				{
					ResHeadImage resHeadImage = new ResHeadImage();
					if (resHeadImage.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.headImageDict.ContainsKey(resHeadImage.dwID))
						{
							GameDataMgr.headImageDict.Add(resHeadImage.dwID, resHeadImage);
						}
						flag2 = true;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_SRV2CLT_GLOBAL_CONF:
				{
					ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
					if (resGlobalInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.svr2CltCfgDict.ContainsKey(resGlobalInfo.dwConfType))
						{
							GameDataMgr.svr2CltCfgDict.Add(resGlobalInfo.dwConfType, resGlobalInfo);
						}
						flag3 = true;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_BANNERIMAGE:
				{
					ResBannerImage resBannerImage = new ResBannerImage();
					if (resBannerImage.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.svr2BannerImageDict.ContainsKey(resBannerImage.dwID))
						{
							GameDataMgr.svr2BannerImageDict.Add(resBannerImage.dwID, resBannerImage);
						}
						flag6 = true;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_REWARDMATCH:
				{
					ResRewardMatchConf resRewardMatchConf = new ResRewardMatchConf();
					if (resRewardMatchConf.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.rewardMatchRewardDict.ContainsKey(resRewardMatchConf.dwId))
						{
							GameDataMgr.rewardMatchRewardDict.Add(resRewardMatchConf.dwId, resRewardMatchConf);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_SHOPPROMOTION:
				{
					ResShopPromotion resShopPromotion = new ResShopPromotion();
					if (resShopPromotion.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.shopPromotionDict.ContainsKey(resShopPromotion.dwID))
						{
							ListView<ResShopPromotion> listView = new ListView<ResShopPromotion>();
							listView.Add(resShopPromotion);
							GameDataMgr.shopPromotionDict.Add(resShopPromotion.dwID, listView);
						}
						else
						{
							ListView<ResShopPromotion> listView2 = new ListView<ResShopPromotion>();
							if (GameDataMgr.shopPromotionDict.TryGetValue(resShopPromotion.dwID, out listView2))
							{
								listView2.Add(resShopPromotion);
								GameDataMgr.shopPromotionDict[resShopPromotion.dwID] = listView2;
							}
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_PROP:
				{
					ResPropInfo resPropInfo = new ResPropInfo();
					if (resPropInfo.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						GameDataMgr.itemDatabin.UpdataData(resPropInfo.dwID, resPropInfo);
						GameDataMgr.itemDatabin.isAllowUnLoad = false;
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_PTEXCHANGE:
				{
					ResWealPointExchange resWealPointExchange = new ResWealPointExchange();
					if (resWealPointExchange.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.wealPointExchangeDict.ContainsKey(resWealPointExchange.dwID))
						{
							GameDataMgr.wealPointExchangeDict.Add(resWealPointExchange.dwID, resWealPointExchange);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_RANK_SEASON:
				{
					ResRankSeasonConf resRankSeasonConf = new ResRankSeasonConf();
					if (resRankSeasonConf.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.rankSeasonDict.ContainsKey(resRankSeasonConf.dwSeasonId))
						{
							GameDataMgr.rankSeasonDict.Add(resRankSeasonConf.dwSeasonId, resRankSeasonConf);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_RAREEXCHANGE:
				{
					ResRareExchange resRareExchange = new ResRareExchange();
					if (resRareExchange.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.rareExchangeDict.ContainsKey(resRareExchange.dwID))
						{
							GameDataMgr.rareExchangeDict.Add(resRareExchange.dwID, resRareExchange);
						}
						continue;
					}
					return;
				}
				case CS_RES_DATA_TYPE.CS_RES_DATA_WEAL_PARAM:
				{
					ResWealParam resWealParam = new ResWealParam();
					if (resWealParam.unpack(ref tdrReadBuf, 0u) == TdrError.ErrorType.TDR_NO_ERROR)
					{
						if (!GameDataMgr.resWealParamDict.ContainsKey(resWealParam.dwID))
						{
							GameDataMgr.resWealParamDict.Add(resWealParam.dwID, resWealParam);
						}
						continue;
					}
					return;
				}
				}
				if (tdrReadBuf.skipForward((int)step) != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return;
				}
			}
			if (flag5)
			{
				Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
			}
			if (flag4)
			{
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
			}
			if (flag)
			{
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.SERVER_SKIN_DATABIN_READY);
			}
			if (flag2)
			{
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
			}
			if (flag6)
			{
				MonoSingleton<BannerImageSys>.GetInstance().LoadConfigServer();
			}
			if (flag3)
			{
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY);
				Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SetCoinGetLimitDailyCnt();
			}
		}

		public static bool IsHeroAvailable(uint heroCfgId)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(heroCfgId, out resHeroShop);
			if (resHeroShop == null)
			{
				return false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			if (masterRoleInfo.IsHaveHero(heroCfgId, true) || masterRoleInfo.IsFreeHero(heroCfgId))
			{
				return resHeroShop.bShowInMgr > 0;
			}
			return GameDataMgr.IsHeroAvailableAtShop(heroCfgId);
		}

		public static bool IsSkinAvailable(uint skinCfgId)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
			if (resHeroSkinShop == null)
			{
				return false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			if (masterRoleInfo.IsHaveHeroSkin(skinCfgId, true))
			{
				return resHeroSkinShop.bShowInMgr > 0;
			}
			return GameDataMgr.IsSkinAvailableAtShop(skinCfgId);
		}

		public static bool IsHeroCanBeGift(uint heroCfgId)
		{
			return GameDataMgr.IsHeroCanBuyForFriend(heroCfgId) || GameDataMgr.IsHeroCanBeAskFor(heroCfgId);
		}

		public static bool IsHeroCanBuyForFriend(uint heroCfgId)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(heroCfgId, out resHeroShop);
			return resHeroShop != null && resHeroShop.bIsPresent > 0;
		}

		public static bool IsHeroCanBeAskFor(uint heroCfgId)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(heroCfgId, out resHeroShop);
			return resHeroShop != null && resHeroShop.bIsAskfor > 0;
		}

		public static bool IsSkinCanBeGift(uint skinCfgId)
		{
			return GameDataMgr.IsSkinCanBuyForFriend(skinCfgId) || GameDataMgr.IsSkinCanBeAskFor(skinCfgId);
		}

		public static bool IsSkinCanBuyForFriend(uint skinCfgId)
		{
			bool result = false;
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
			if (resHeroSkinShop != null)
			{
				result = (resHeroSkinShop.bIsPresent > 0);
			}
			return result;
		}

		public static bool IsSkinCanBeAskFor(uint skinCfgId)
		{
			bool result = false;
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
			if (resHeroSkinShop != null)
			{
				result = (resHeroSkinShop.bIsAskfor > 0);
			}
			return result;
		}

		public static bool IsSkinAvailableAtShop(uint skinCfgId)
		{
			bool result = false;
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
			if (resHeroSkinShop != null && resHeroSkinShop.bShowInShop > 0)
			{
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				result = ((long)currentUTCTime >= (long)((ulong)resHeroSkinShop.dwOnTimeGen) && (long)currentUTCTime <= (long)((ulong)resHeroSkinShop.dwOffTimeGen));
			}
			return result;
		}

		public static bool IsSkinCanBuyNow(uint skinCfgId)
		{
			bool flag = false;
			if (GameDataMgr.IsSkinAvailableAtShop(skinCfgId))
			{
				ResHeroSkinShop resHeroSkinShop = null;
				GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
				if (resHeroSkinShop != null)
				{
					flag = (resHeroSkinShop.bIsBuyDiamond > 0 || resHeroSkinShop.bIsBuyCoupons > 0);
					if (!flag)
					{
						for (int i = 0; i < 5; i++)
						{
							uint num = resHeroSkinShop.PromotionID[i];
							if (num != 0u)
							{
								ResSkinPromotion resSkinPromotion = null;
								if (GameDataMgr.skinPromotionDict.TryGetValue(num, out resSkinPromotion) && resSkinPromotion != null && (ulong)resSkinPromotion.dwOnTimeGen <= (ulong)((long)CRoleInfo.GetCurrentUTCTime()) && (ulong)resSkinPromotion.dwOffTimeGen >= (ulong)((long)CRoleInfo.GetCurrentUTCTime()))
								{
									flag = (resSkinPromotion.bIsBuyDiamond > 0 || resSkinPromotion.bIsBuyCoupons > 0);
									if (flag)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			return flag;
		}

		public static bool IsHeroCanBePickByComputer(uint heroCfgId)
		{
			return GameDataMgr.IsHeroAvailableAtShop(heroCfgId);
		}

		public static bool IsHeroAvailableAtShop(uint heroCfgId)
		{
			bool result = false;
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(heroCfgId, out resHeroShop);
			if (resHeroShop != null && resHeroShop.bShowInShop > 0)
			{
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				result = ((long)currentUTCTime >= (long)((ulong)resHeroShop.dwOnTimeGen) && (long)currentUTCTime <= (long)((ulong)resHeroShop.dwOffTimeGen));
			}
			return result;
		}

		public static void GetAllHeroList(ref ListView<ResHeroCfgInfo> list, out int validCount, enHeroJobType job = enHeroJobType.All, bool onlyAvailableAtShop = false, bool onlyCanBeGift = false)
		{
			validCount = 0;
			if (list == null)
			{
				list = new ListLinqView<ResHeroCfgInfo>();
			}
			int count = list.Count;
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResHeroCfgInfo resHeroCfgInfo = current.Value as ResHeroCfgInfo;
				if (resHeroCfgInfo != null)
				{
					if (job == enHeroJobType.All || (byte)job == resHeroCfgInfo.bMainJob || (byte)job == resHeroCfgInfo.bMinorJob)
					{
						if (!onlyAvailableAtShop || GameDataMgr.IsHeroAvailableAtShop(resHeroCfgInfo.dwCfgID))
						{
							if (!onlyCanBeGift || GameDataMgr.IsHeroCanBeGift(resHeroCfgInfo.dwCfgID))
							{
								if (validCount < count)
								{
									list[validCount] = resHeroCfgInfo;
								}
								else
								{
									list.Add(resHeroCfgInfo);
								}
								validCount++;
							}
						}
					}
				}
			}
		}

		public static void GetAllSkinList(ref ListView<ResHeroSkin> list, out int validCount, enHeroJobType job = enHeroJobType.All, bool includeDefaultSkin = false, bool onlyAvailableAtShop = false, bool onlyCanBeGift = false)
		{
			validCount = 0;
			if (list == null)
			{
				list = new ListLinqView<ResHeroSkin>();
			}
			int count = list.Count;
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSkinDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResHeroSkin resHeroSkin = current.Value as ResHeroSkin;
				if (resHeroSkin != null)
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resHeroSkin.dwHeroID);
					if (dataByKey != null)
					{
						if (includeDefaultSkin || resHeroSkin.dwSkinID != 0u)
						{
							if (job == enHeroJobType.All || (byte)job == dataByKey.bMainJob || (byte)job == dataByKey.bMinorJob)
							{
								if (!onlyAvailableAtShop || GameDataMgr.IsSkinAvailableAtShop(resHeroSkin.dwID))
								{
									if (!onlyCanBeGift || (GameDataMgr.IsSkinCanBeGift(resHeroSkin.dwID) && GameDataMgr.IsSkinCanBuyNow(resHeroSkin.dwID)))
									{
										if (validCount < count)
										{
											list[validCount] = resHeroSkin;
										}
										else
										{
											list.Add(resHeroSkin);
										}
										validCount++;
									}
								}
							}
						}
					}
				}
			}
		}

		public static void ClearServerResData()
		{
			GameDataMgr.wealCheckInDict.Clear();
			GameDataMgr.wealFixtimeDict.Clear();
			GameDataMgr.wealMultipleDict.Clear();
			GameDataMgr.wealExchangeDict.Clear();
			GameDataMgr.wealPointExchangeDict.Clear();
			GameDataMgr.wealConditionDict.Clear();
			GameDataMgr.wealNoticeDict.Clear();
			GameDataMgr.wealCheckFillDict.Clear();
			GameDataMgr.pvpRatioDict.Clear();
			GameDataMgr.specSaleDict.Clear();
			GameDataMgr.heroPromotionDict.Clear();
			GameDataMgr.skinPromotionDict.Clear();
			GameDataMgr.mallRoulettePriceDict.Clear();
			GameDataMgr.mallRouletteRewardDict.Clear();
			GameDataMgr.mallRouletteExternRewardDict.Clear();
			Singleton<MySteryShop>.GetInstance().ClearSvrData();
			GameDataMgr.redDotInfoDict.Clear();
			GameDataMgr.heroShopInfoDict.Clear();
			GameDataMgr.skinShopInfoDict.Clear();
			GameDataMgr.recommendProductDict.Clear();
			GameDataMgr.recommendLotteryCtrlDict.Clear();
			GameDataMgr.recommendRewardDict.Clear();
			GameDataMgr.rewardMatchRewardDict.Clear();
			GameDataMgr.boutiqueDict.Clear();
			GameDataMgr.headImageDict.Clear();
			GameDataMgr.svr2CltCfgDict.Clear();
			GameDataMgr.svr2BannerImageDict.Clear();
			MonoSingleton<BannerImageSys>.GetInstance().ClearSeverData();
			GameDataMgr.shopPromotionDict.Clear();
			GameDataMgr.rankSeasonDict.Clear();
		}

		public static long GetDoubleKey(uint key1, uint key2)
		{
			ulong num = Convert.ToUInt64(key1);
			num <<= 32;
			return (long)(num + (ulong)key2);
		}

		public static uint GetGlobeValue(RES_GLOBAL_CONF_TYPE globeType)
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint)globeType);
			DebugHelper.Assert(dataByKey != null, "global conf {0} doesn't exist!", new object[]
			{
				(uint)globeType
			});
			if (dataByKey == null)
			{
				return 0u;
			}
			return dataByKey.dwConfValue;
		}

		public static uint GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE type)
		{
			ResGlobalInfo resGlobalInfo = null;
			GameDataMgr.svr2CltCfgDict.TryGetValue((uint)type, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return 0u;
			}
			return resGlobalInfo.dwConfValue;
		}
	}
}

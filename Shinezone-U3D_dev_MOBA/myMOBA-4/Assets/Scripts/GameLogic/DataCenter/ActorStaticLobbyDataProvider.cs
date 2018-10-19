using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	internal class ActorStaticLobbyDataProvider : ActorStaticDataProviderBase
	{
		protected bool BuildActorData(ResHeroCfgInfo heroCfg, ref ActorStaticData actorData)
		{
			actorData.TheBaseAttribute.EpType = heroCfg.dwEnergyType;
			actorData.TheBaseAttribute.BaseEp = heroCfg.iEnergy;
			actorData.TheBaseAttribute.EpGrowth = heroCfg.iEnergyGrowth;
			actorData.TheBaseAttribute.BaseEpRecover = heroCfg.iEnergyRec;
			actorData.TheBaseAttribute.PerLvEpRecover = heroCfg.iEnergyRecGrowth;
			actorData.TheBaseAttribute.BaseHp = heroCfg.iBaseHP;
			actorData.TheBaseAttribute.PerLvHp = heroCfg.iHpGrowth;
			actorData.TheBaseAttribute.BaseAd = heroCfg.iBaseATT;
			actorData.TheBaseAttribute.PerLvAd = heroCfg.iAtkGrowth;
			actorData.TheBaseAttribute.BaseAp = heroCfg.iBaseINT;
			actorData.TheBaseAttribute.PerLvAp = heroCfg.iSpellGrowth;
			actorData.TheBaseAttribute.BaseAtkSpeed = heroCfg.iBaseAtkSpd;
			actorData.TheBaseAttribute.PerLvAtkSpeed = heroCfg.iAtkSpdAddLvlup;
			actorData.TheBaseAttribute.BaseDef = heroCfg.iBaseDEF;
			actorData.TheBaseAttribute.PerLvDef = heroCfg.iDefGrowth;
			actorData.TheBaseAttribute.BaseRes = heroCfg.iBaseRES;
			actorData.TheBaseAttribute.PerLvRes = heroCfg.iResistGrowth;
			actorData.TheBaseAttribute.BaseHpRecover = heroCfg.iBaseHPAdd;
			actorData.TheBaseAttribute.PerLvHpRecover = heroCfg.iHPAddLvlup;
			actorData.TheBaseAttribute.CriticalChance = heroCfg.iCritRate;
			actorData.TheBaseAttribute.CriticalDamage = heroCfg.iCritEft;
			actorData.TheBaseAttribute.Sight = heroCfg.iSightR;
			actorData.TheBaseAttribute.MoveSpeed = heroCfg.iBaseSpeed;
			actorData.TheBaseAttribute.SoulExpGained = 0;
			actorData.TheBaseAttribute.GoldCoinInBattleGained = 0;
			actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = 0;
			actorData.TheBaseAttribute.ClashMark = 0u;
			actorData.TheBaseAttribute.RandomPassiveSkillRule = 0;
			actorData.TheBaseAttribute.PassiveSkillID1 = heroCfg.iPassiveID1;
			actorData.TheBaseAttribute.PassiveSkillID2 = heroCfg.iPassiveID2;
			actorData.TheBaseAttribute.DeadControl = (heroCfg.dwDeadControl == 1u);
			actorData.TheHeroOnlyInfo.HeroCapability = (int)heroCfg.bMainJob;
			actorData.TheHeroOnlyInfo.HeroDamageType = (int)heroCfg.bDamageType;
			actorData.TheHeroOnlyInfo.HeroAttackType = (int)heroCfg.bAttackType;
			actorData.TheHeroOnlyInfo.InitialStar = heroCfg.iInitialStar;
			actorData.TheHeroOnlyInfo.RecommendStandPos = heroCfg.iRecommendPosition;
			actorData.TheHeroOnlyInfo.AttackDistanceType = (int)heroCfg.bAttackDistanceType;
			actorData.TheHeroOnlyInfo.HeroNamePinYin = heroCfg.szNamePinYin;
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref heroCfg.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref heroCfg.szCharacterInfo);
			actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
			return true;
		}

		protected override bool BuildCallActorData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)actorData.TheActorMeta.HostConfigId);
			if (dataByKey == null)
			{
				base.ErrorMissingHeroConfig((uint)actorData.TheActorMeta.ConfigId);
				return false;
			}
			this.BuildActorData(dataByKey, ref actorData);
			ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey((uint)actorData.TheActorMeta.ConfigId);
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataByKey2.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataByKey2.szCharacterInfo);
			return true;
		}

		protected override bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)actorData.TheActorMeta.ConfigId);
			if (dataByKey == null)
			{
				base.ErrorMissingHeroConfig((uint)actorData.TheActorMeta.ConfigId);
				return false;
			}
			this.BuildActorData(dataByKey, ref actorData);
			return true;
		}

		protected override bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, (int)actorData.TheActorMeta.Difficuty);
			if (dataCfgInfo == null)
			{
				dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, 1);
			}
			if (dataCfgInfo == null)
			{
				base.ErrorMissingMonsterConfig((uint)actorData.TheActorMeta.ConfigId);
				return false;
			}
			actorData.TheBaseAttribute.EpType = 1u;
			actorData.TheBaseAttribute.BaseHp = dataCfgInfo.iBaseHP;
			actorData.TheBaseAttribute.PerLvHp = 0;
			actorData.TheBaseAttribute.BaseAd = dataCfgInfo.iBaseATT;
			actorData.TheBaseAttribute.PerLvAd = 0;
			actorData.TheBaseAttribute.BaseAp = dataCfgInfo.iBaseINT;
			actorData.TheBaseAttribute.PerLvAp = 0;
			actorData.TheBaseAttribute.BaseAtkSpeed = 0;
			actorData.TheBaseAttribute.PerLvAtkSpeed = 0;
			actorData.TheBaseAttribute.BaseDef = dataCfgInfo.iBaseDEF;
			actorData.TheBaseAttribute.PerLvDef = 0;
			actorData.TheBaseAttribute.BaseRes = dataCfgInfo.iBaseRES;
			actorData.TheBaseAttribute.PerLvRes = 0;
			actorData.TheBaseAttribute.BaseHpRecover = dataCfgInfo.iBaseHPAdd;
			actorData.TheBaseAttribute.PerLvHpRecover = 0;
			actorData.TheBaseAttribute.CriticalChance = 0;
			actorData.TheBaseAttribute.CriticalDamage = 0;
			actorData.TheBaseAttribute.Sight = dataCfgInfo.iSightR;
			actorData.TheBaseAttribute.MoveSpeed = dataCfgInfo.iBaseSpeed;
			actorData.TheBaseAttribute.SoulExpGained = dataCfgInfo.iSoulExp;
			actorData.TheBaseAttribute.GoldCoinInBattleGained = (int)dataCfgInfo.wStartingGoldCoinInBattle;
			actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = (int)dataCfgInfo.bGoldCoinInBattleRange;
			actorData.TheBaseAttribute.DynamicProperty = dataCfgInfo.dwDynamicPropertyCfg;
			actorData.TheBaseAttribute.ClashMark = dataCfgInfo.dwClashMark;
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfo.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfo.szCharacterInfo);
			actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
			return true;
		}

		protected override bool BuildOrganData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(actorData.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				base.ErrorMissingOrganConfig((uint)actorData.TheActorMeta.ConfigId);
				return false;
			}
			actorData.TheBaseAttribute.EpType = 1u;
			actorData.TheBaseAttribute.BaseHp = dataCfgInfoByCurLevelDiff.iBaseHP;
			actorData.TheBaseAttribute.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
			actorData.TheBaseAttribute.BaseAd = dataCfgInfoByCurLevelDiff.iBaseATT;
			actorData.TheBaseAttribute.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
			actorData.TheBaseAttribute.BaseAp = dataCfgInfoByCurLevelDiff.iBaseINT;
			actorData.TheBaseAttribute.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
			actorData.TheBaseAttribute.BaseAtkSpeed = 0;
			actorData.TheBaseAttribute.PerLvAtkSpeed = dataCfgInfoByCurLevelDiff.iAtkSpdAddLvlup;
			actorData.TheBaseAttribute.BaseDef = dataCfgInfoByCurLevelDiff.iBaseDEF;
			actorData.TheBaseAttribute.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
			actorData.TheBaseAttribute.BaseRes = dataCfgInfoByCurLevelDiff.iBaseRES;
			actorData.TheBaseAttribute.PerLvRes = dataCfgInfoByCurLevelDiff.iRESLvlup;
			actorData.TheBaseAttribute.BaseHpRecover = dataCfgInfoByCurLevelDiff.iBaseHPAdd;
			actorData.TheBaseAttribute.PerLvHpRecover = dataCfgInfoByCurLevelDiff.iHPAddLvlup;
			actorData.TheBaseAttribute.CriticalChance = 0;
			actorData.TheBaseAttribute.CriticalDamage = 0;
			actorData.TheBaseAttribute.Sight = dataCfgInfoByCurLevelDiff.iSightR;
			actorData.TheBaseAttribute.MoveSpeed = dataCfgInfoByCurLevelDiff.iBaseSpeed;
			actorData.TheBaseAttribute.SoulExpGained = dataCfgInfoByCurLevelDiff.iSoulExp;
			actorData.TheBaseAttribute.GoldCoinInBattleGained = (int)dataCfgInfoByCurLevelDiff.wGoldCoinInBattle;
			actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = 0;
			actorData.TheBaseAttribute.DynamicProperty = dataCfgInfoByCurLevelDiff.dwDynamicPropertyCfg;
			actorData.TheBaseAttribute.ClashMark = dataCfgInfoByCurLevelDiff.dwClashMark;
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
			actorData.TheOrganOnlyInfo.PhyArmorHurtRate = dataCfgInfoByCurLevelDiff.iPhyArmorHurtRate;
			actorData.TheOrganOnlyInfo.AttackRouteID = dataCfgInfoByCurLevelDiff.iAktRouteID;
			actorData.TheOrganOnlyInfo.DeadEnemySoldier = dataCfgInfoByCurLevelDiff.iDeadEnemySoldier;
			actorData.TheOrganOnlyInfo.NoEnemyAddPhyDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddPhyDef;
			actorData.TheOrganOnlyInfo.NoEnemyAddMgcDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddMgcDef;
			actorData.TheOrganOnlyInfo.HorizonRadius = dataCfgInfoByCurLevelDiff.iHorizonRadius;
			actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
			return true;
		}
	}
}

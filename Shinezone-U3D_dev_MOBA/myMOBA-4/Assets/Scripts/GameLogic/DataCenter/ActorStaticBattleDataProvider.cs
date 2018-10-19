using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	internal class ActorStaticBattleDataProvider : ActorStaticDataProviderBase
	{
		protected override bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
			actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
			return actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
		}

		protected override bool BuildCallActorData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
			actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
			return actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
		}

		protected override bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResMonsterCfgInfo resMonsterCfgInfo = this.ConsiderDifficultyToChooseMonsterCfg((uint)actorData.TheActorMeta.ConfigId, actorData.TheActorMeta.Difficuty);
			if (resMonsterCfgInfo == null)
			{
				resMonsterCfgInfo = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, (int)actorData.TheActorMeta.Difficuty);
				if (resMonsterCfgInfo == null)
				{
					resMonsterCfgInfo = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, 1);
				}
				if (resMonsterCfgInfo == null)
				{
					base.ErrorMissingMonsterConfig((uint)actorData.TheActorMeta.ConfigId);
					return false;
				}
			}
			DynamicAttributeInfo dynamicAttributeInfo = this.ConsiderMonsterDynamicInfo(resMonsterCfgInfo);
			actorData.TheBaseAttribute.EpType = 1u;
			actorData.TheBaseAttribute.BaseHp = ((dynamicAttributeInfo == null) ? resMonsterCfgInfo.iBaseHP : dynamicAttributeInfo.iBaseHP);
			actorData.TheBaseAttribute.PerLvHp = 0;
			actorData.TheBaseAttribute.BaseAd = ((dynamicAttributeInfo == null) ? resMonsterCfgInfo.iBaseATT : dynamicAttributeInfo.iAD);
			actorData.TheBaseAttribute.PerLvAd = 0;
			actorData.TheBaseAttribute.BaseAp = ((dynamicAttributeInfo == null) ? resMonsterCfgInfo.iBaseINT : dynamicAttributeInfo.iAP);
			actorData.TheBaseAttribute.PerLvAp = 0;
			actorData.TheBaseAttribute.BaseAtkSpeed = 0;
			actorData.TheBaseAttribute.PerLvAtkSpeed = 0;
			actorData.TheBaseAttribute.BaseDef = ((dynamicAttributeInfo == null) ? resMonsterCfgInfo.iBaseDEF : dynamicAttributeInfo.iDef);
			actorData.TheBaseAttribute.PerLvDef = 0;
			actorData.TheBaseAttribute.BaseRes = ((dynamicAttributeInfo == null) ? resMonsterCfgInfo.iBaseRES : dynamicAttributeInfo.iRes);
			actorData.TheBaseAttribute.PerLvRes = 0;
			actorData.TheBaseAttribute.BaseHpRecover = resMonsterCfgInfo.iBaseHPAdd;
			actorData.TheBaseAttribute.PerLvHpRecover = 0;
			actorData.TheBaseAttribute.CriticalChance = 0;
			actorData.TheBaseAttribute.CriticalDamage = 0;
			actorData.TheBaseAttribute.Sight = resMonsterCfgInfo.iSightR;
			actorData.TheBaseAttribute.MoveSpeed = resMonsterCfgInfo.iBaseSpeed;
			actorData.TheBaseAttribute.SoulExpGained = resMonsterCfgInfo.iSoulExp;
			actorData.TheBaseAttribute.GoldCoinInBattleGained = (int)resMonsterCfgInfo.wStartingGoldCoinInBattle;
			actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = (int)resMonsterCfgInfo.bGoldCoinInBattleRange;
			actorData.TheBaseAttribute.DynamicProperty = resMonsterCfgInfo.dwDynamicPropertyCfg;
			actorData.TheBaseAttribute.ClashMark = resMonsterCfgInfo.dwClashMark;
			actorData.TheBaseAttribute.RandomPassiveSkillRule = (int)resMonsterCfgInfo.bRandomPassiveSkillRule;
			actorData.TheBaseAttribute.PassiveSkillID1 = 0;
			actorData.TheBaseAttribute.PassiveSkillID2 = 0;
			actorData.TheBaseAttribute.DeadControl = false;
			actorData.TheMonsterOnlyInfo.MonsterBaseLevel = resMonsterCfgInfo.iLevel;
			actorData.TheMonsterOnlyInfo.SoldierType = resMonsterCfgInfo.bSoldierType;
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref resMonsterCfgInfo.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref resMonsterCfgInfo.szCharacterInfo);
			actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
			return true;
		}

		internal ResMonsterCfgInfo ConsiderDifficultyToChooseMonsterCfg(uint monsterCfgId, byte diff)
		{
			return MonsterDataHelper.GetDataCfgInfo((int)monsterCfgId, (int)diff);
		}

		internal DynamicAttributeInfo ConsiderMonsterDynamicInfo(ResMonsterCfgInfo monsterCfg)
		{
			int num = 0;
			DynamicAttributeInfo result = null;
			if (monsterCfg.iDynamicInfoType == 1)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && !curLvelContext.IsMobaMode())
				{
					ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)curLvelContext.m_mapID);
					if (dataByKey == null)
					{
						base.ErrorMissingLevelConfig((uint)curLvelContext.m_mapID);
						return null;
					}
					if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP)
					{
						num = (int)dataByKey.dwAIPlayerLevel;
					}
				}
			}
			if (num <= 0)
			{
				return null;
			}
			ResMonsterOrganLevelDynamicInfo dataByKey2 = GameDataMgr.monsterOrganLvDynamicInfobin.GetDataByKey((long)num);
			if (dataByKey2 == null)
			{
				return null;
			}
			switch (monsterCfg.bSoldierType)
			{
			case 1:
				result = dataByKey2.stMelee;
				break;
			case 2:
				result = dataByKey2.stRemote;
				break;
			case 3:
				result = dataByKey2.stSuper;
				break;
			case 4:
				result = dataByKey2.stAnYingDaJiang;
				break;
			case 5:
				result = dataByKey2.stHeiAnXianFeng;
				break;
			case 6:
				result = dataByKey2.stBaoZouJiangShi;
				break;
			case 7:
			case 8:
				result = dataByKey2.stBaoJun;
				break;
			}
			return result;
		}

		protected override bool BuildOrganData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(actorData.TheActorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				base.ErrorMissingOrganConfig((uint)actorData.TheActorMeta.ConfigId);
				return false;
			}
			DynamicAttributeInfo dynamicAttributeInfo = this.ConsiderOrganDynamicInfo(dataCfgInfoByCurLevelDiff);
			actorData.TheBaseAttribute.EpType = 1u;
			actorData.TheBaseAttribute.BaseHp = ((dynamicAttributeInfo == null) ? dataCfgInfoByCurLevelDiff.iBaseHP : dynamicAttributeInfo.iBaseHP);
			actorData.TheBaseAttribute.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
			actorData.TheBaseAttribute.BaseAd = ((dynamicAttributeInfo == null) ? dataCfgInfoByCurLevelDiff.iBaseATT : dynamicAttributeInfo.iAD);
			actorData.TheBaseAttribute.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
			actorData.TheBaseAttribute.BaseAp = ((dynamicAttributeInfo == null) ? dataCfgInfoByCurLevelDiff.iBaseINT : dynamicAttributeInfo.iAP);
			actorData.TheBaseAttribute.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
			actorData.TheBaseAttribute.BaseAtkSpeed = 0;
			actorData.TheBaseAttribute.PerLvAtkSpeed = dataCfgInfoByCurLevelDiff.iAtkSpdAddLvlup;
			actorData.TheBaseAttribute.BaseDef = ((dynamicAttributeInfo == null) ? dataCfgInfoByCurLevelDiff.iBaseDEF : dynamicAttributeInfo.iDef);
			actorData.TheBaseAttribute.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
			actorData.TheBaseAttribute.BaseRes = ((dynamicAttributeInfo == null) ? dataCfgInfoByCurLevelDiff.iBaseRES : dynamicAttributeInfo.iRes);
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
			actorData.TheBaseAttribute.RandomPassiveSkillRule = 0;
			actorData.TheBaseAttribute.PassiveSkillID1 = 0;
			actorData.TheBaseAttribute.PassiveSkillID2 = 0;
			actorData.TheBaseAttribute.DeadControl = false;
			actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szName);
			actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
			actorData.TheOrganOnlyInfo.OrganType = (int)dataCfgInfoByCurLevelDiff.bOrganType;
			actorData.TheOrganOnlyInfo.ShowInMinimap = (dataCfgInfoByCurLevelDiff.bShowInMinimap != 0);
			actorData.TheOrganOnlyInfo.PhyArmorHurtRate = dataCfgInfoByCurLevelDiff.iPhyArmorHurtRate;
			actorData.TheOrganOnlyInfo.AttackRouteID = dataCfgInfoByCurLevelDiff.iAktRouteID;
			actorData.TheOrganOnlyInfo.DeadEnemySoldier = dataCfgInfoByCurLevelDiff.iDeadEnemySoldier;
			actorData.TheOrganOnlyInfo.NoEnemyAddPhyDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddPhyDef;
			actorData.TheOrganOnlyInfo.NoEnemyAddMgcDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddMgcDef;
			actorData.TheOrganOnlyInfo.HorizonRadius = dataCfgInfoByCurLevelDiff.iHorizonRadius;
			actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
			return true;
		}

		internal DynamicAttributeInfo ConsiderOrganDynamicInfo(ResOrganCfgInfo organCfg)
		{
			int num = 0;
			DynamicAttributeInfo result = null;
			if (organCfg.iDynamicInfoType == 1)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && !curLvelContext.IsMobaMode())
				{
					ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)curLvelContext.m_mapID);
					if (dataByKey == null)
					{
						base.ErrorMissingLevelConfig((uint)curLvelContext.m_mapID);
						return null;
					}
					if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP)
					{
						num = (int)dataByKey.dwAIPlayerLevel;
					}
				}
			}
			if (num <= 0)
			{
				return null;
			}
			ResMonsterOrganLevelDynamicInfo dataByKey2 = GameDataMgr.monsterOrganLvDynamicInfobin.GetDataByKey((long)num);
			if (dataByKey2 == null)
			{
				return null;
			}
			switch (organCfg.bOrganType)
			{
			case 1:
				result = dataByKey2.stTurret;
				break;
			case 2:
				result = dataByKey2.stBase;
				break;
			case 3:
				result = dataByKey2.stBarrack;
				break;
			}
			return result;
		}
	}
}

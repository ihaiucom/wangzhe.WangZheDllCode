using AGE;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LoaderHelper
{
	public Dictionary<long, Dictionary<object, AssetRefType>> ageRefAssets2 = new Dictionary<long, Dictionary<object, AssetRefType>>();

	public Dictionary<string, bool> ageCheckerSet = new Dictionary<string, bool>();

	public Dictionary<string, bool> behaviorXmlSet = new Dictionary<string, bool>();

	public Dictionary<int, bool> randomSkillCheckerSet = new Dictionary<int, bool>();

	public List<long> skillIdList = new List<long>();

	public List<long> skillCombineList = new List<long>();

	public List<long> passiveSkillList = new List<long>();

	private static int retCnt;

	public Dictionary<object, AssetRefType> GetRefAssets(int markID, int configID)
	{
		Dictionary<object, AssetRefType> dictionary = null;
		if (markID == 0)
		{
			configID = 0;
		}
		long num = (long)markID << 32 | (long)configID;
		if (!this.ageRefAssets2.TryGetValue(num, ref dictionary))
		{
			dictionary = new Dictionary<object, AssetRefType>();
			this.ageRefAssets2.Add(num, dictionary);
		}
		return dictionary;
	}

	public List<ActorPreloadTab> GetActorPreload()
	{
		List<ActorPreloadTab> result = new List<ActorPreloadTab>();
		this.BuildStaticActor(ref result);
		this.BuildDynamicActor(ref result);
		this.BuildSpawnPoints(ref result);
		this.BuildCommonSpawnPoints(ref result);
		this.BuildSoldierRegions(ref result);
		this.BuildEquipActiveSkill(ref result);
		return result;
	}

	public void BuildEquipInBattle(ref ActorPreloadTab result)
	{
		Dictionary<long, object>.Enumerator enumerator = GameDataMgr.m_equipInBattleDatabin.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<long, object> current = enumerator.get_Current();
			ResEquipInBattle resEquipInBattle = current.get_Value() as ResEquipInBattle;
			if (resEquipInBattle != null)
			{
				string path = CUIUtility.s_Sprite_System_BattleEquip_Dir + StringHelper.UTF8BytesToString(ref resEquipInBattle.szIcon);
				result.AddSprite(path);
				if (resEquipInBattle.astEffectCombine != null && resEquipInBattle.astEffectCombine.Length > 0)
				{
					for (int i = 0; i < resEquipInBattle.astEffectCombine.Length; i++)
					{
						uint dwID = resEquipInBattle.astEffectCombine[i].dwID;
						this.AnalyseSkillCombine(ref result, (int)dwID);
					}
				}
				if (resEquipInBattle.astPassiveSkill != null && resEquipInBattle.astPassiveSkill.Length > 0)
				{
					for (int j = 0; j < resEquipInBattle.astPassiveSkill.Length; j++)
					{
						if (resEquipInBattle.astPassiveSkill[j].dwID > 0u)
						{
							this.AnalysePassiveSkill(ref result, (int)resEquipInBattle.astPassiveSkill[j].dwID);
						}
					}
				}
			}
		}
	}

	public void BuildStaticActor(ref List<ActorPreloadTab> list)
	{
		DebugHelper.Assert(MonoSingleton<GameLoader>.instance.staticActors != null, "Static actors cannot be null");
		for (int i = 0; i < MonoSingleton<GameLoader>.instance.staticActors.Count; i++)
		{
			DebugHelper.Assert(MonoSingleton<GameLoader>.instance.staticActors[i] != null, "actor config cannot be null at {0}", new object[]
			{
				i
			});
			if (MonoSingleton<GameLoader>.instance.staticActors[i] != null)
			{
				ActorMeta actorMeta = default(ActorMeta);
				actorMeta.ActorType = MonoSingleton<GameLoader>.instance.staticActors[i].ActorType;
				actorMeta.ConfigId = MonoSingleton<GameLoader>.instance.staticActors[i].ConfigID;
				actorMeta.ActorCamp = MonoSingleton<GameLoader>.instance.staticActors[i].CmpType;
				this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
			}
		}
	}

	public void BuildDynamicActor(ref List<ActorPreloadTab> list)
	{
		for (int i = 0; i < MonoSingleton<GameLoader>.instance.actorList.get_Count(); i++)
		{
			ActorMeta actorMeta = MonoSingleton<GameLoader>.instance.actorList.get_Item(i);
			this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
		}
	}

	public void BuildCommonSpawnPoints(ref ActorPreloadTab loadInfo)
	{
		CommonSpawnPoint[] array = Object.FindObjectsOfType<CommonSpawnPoint>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			CommonSpawnPoint commonSpawnPoint = array[i];
			if (!(commonSpawnPoint == null))
			{
				commonSpawnPoint.PreLoadResource(ref loadInfo, this);
			}
		}
	}

	public void BuildCommonSpawnPoints(ref List<ActorPreloadTab> list)
	{
		CommonSpawnPoint[] array = Object.FindObjectsOfType<CommonSpawnPoint>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			CommonSpawnPoint commonSpawnPoint = array[i];
			if (!(commonSpawnPoint == null))
			{
				commonSpawnPoint.PreLoadResource(ref list, this);
			}
		}
	}

	public void BuildSpawnPoints(ref List<ActorPreloadTab> list)
	{
		SpawnPoint[] array = Object.FindObjectsOfType<SpawnPoint>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			SpawnPoint spawnPoint = array[i];
			if (!(spawnPoint == null))
			{
				spawnPoint.PreLoadResource(ref list, this);
			}
		}
	}

	public void BuildSoldierRegions(ref List<ActorPreloadTab> list)
	{
		SoldierRegion[] array = Object.FindObjectsOfType<SoldierRegion>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			SoldierRegion soldierRegion = array[i];
			if (soldierRegion != null && soldierRegion.Waves != null && soldierRegion.Waves.Count > 0)
			{
				for (int j = 0; j < soldierRegion.Waves.Count; j++)
				{
					Assets.Scripts.GameLogic.SoldierWave soldierWave = soldierRegion.Waves[j];
					if (soldierWave != null && soldierWave.WaveInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo.Length > 0)
					{
						for (int k = 0; k < soldierWave.WaveInfo.astNormalSoldierInfo.Length; k++)
						{
							ResSoldierTypeInfo resSoldierTypeInfo = soldierWave.WaveInfo.astNormalSoldierInfo[k];
							if (resSoldierTypeInfo.dwSoldierID == 0u)
							{
								break;
							}
							ActorMeta actorMeta = default(ActorMeta);
							ActorMeta actorMeta2 = actorMeta;
							actorMeta2.ActorType = ActorTypeDef.Actor_Type_Monster;
							actorMeta2.ConfigId = (int)resSoldierTypeInfo.dwSoldierID;
							actorMeta = actorMeta2;
							this.AddPreloadActor(ref list, ref actorMeta, resSoldierTypeInfo.dwSoldierNum * 2f, 0);
						}
					}
				}
			}
		}
	}

	public void BuildAreaTrigger(ref ActorPreloadTab loadInfo)
	{
		AreaTrigger[] array = Object.FindObjectsOfType<AreaTrigger>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			AreaTrigger areaTrigger = array[i];
			this.AnalyseSkillCombine(ref loadInfo, areaTrigger.BuffID);
			this.AnalyseSkillCombine(ref loadInfo, areaTrigger.UpdateBuffID);
			this.AnalyseSkillCombine(ref loadInfo, areaTrigger.LeaveBuffID);
		}
	}

	private void BuildEquipActiveSkill(ref List<ActorPreloadTab> list)
	{
		int count = GameDataMgr.m_equipInBattleDatabin.count;
		ActorPreloadTab actorPreloadTab = new ActorPreloadTab();
		actorPreloadTab.ageActions = new List<AssetLoadBase>();
		actorPreloadTab.parPrefabs = new List<AssetLoadBase>();
		actorPreloadTab.mesPrefabs = new List<AssetLoadBase>();
		actorPreloadTab.spritePrefabs = new List<AssetLoadBase>();
		actorPreloadTab.soundBanks = new List<AssetLoadBase>();
		actorPreloadTab.behaviorXml = new List<AssetLoadBase>();
		for (int i = 0; i < count; i++)
		{
			ResEquipInBattle dataByIndex = GameDataMgr.m_equipInBattleDatabin.GetDataByIndex(i);
			if (dataByIndex != null && dataByIndex.dwActiveSkillID > 0u)
			{
				this.AnalyseSkill(ref actorPreloadTab, (int)dataByIndex.dwActiveSkillID);
				list.Add(actorPreloadTab);
			}
		}
	}

	public void BuildActionHelper(ref ActorPreloadTab loadInfo)
	{
		ActionHelper[] array = Object.FindObjectsOfType<ActionHelper>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			ActionHelper actionHelper = array[i];
			for (int j = 0; j < actionHelper.actionHelpers.Length; j++)
			{
				ActionHelperStorage actionHelperStorage = actionHelper.actionHelpers[j];
				DebugHelper.Assert(actionHelperStorage != null && actionHelperStorage.actionName != null, "storage is null or action name is null. storage = {0}, storage.actionName={1}", new object[]
				{
					(actionHelperStorage != null && actionHelperStorage.helperName != null) ? actionHelperStorage.helperName : "null",
					(actionHelperStorage != null && actionHelperStorage.actionName != null) ? actionHelperStorage.actionName : "null"
				});
				if (actionHelperStorage != null && !string.IsNullOrEmpty(actionHelperStorage.actionName) && !this.ageCheckerSet.ContainsKey(actionHelperStorage.actionName))
				{
					AssetLoadBase assetLoadBase = default(AssetLoadBase);
					assetLoadBase.assetPath = actionHelperStorage.actionName;
					loadInfo.ageActions.Add(assetLoadBase);
					this.ageCheckerSet.Add(actionHelperStorage.actionName, true);
				}
			}
		}
	}

	public void BuildActionTrigger(ref ActorPreloadTab loadInfo)
	{
		AreaEventTrigger[] array = Object.FindObjectsOfType<AreaEventTrigger>();
		if (array != null && array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				AreaEventTrigger areaEventTrigger = array[i];
				for (int j = 0; j < areaEventTrigger.TriggerActions.Length; j++)
				{
					TriggerActionWrapper triggerActionWrapper = areaEventTrigger.TriggerActions[j];
					if (triggerActionWrapper != null)
					{
						triggerActionWrapper.PreLoadResource(ref loadInfo, this.ageCheckerSet, this);
					}
					if (triggerActionWrapper != null && triggerActionWrapper.TriggerType == EGlobalTriggerAct.TriggerShenFu)
					{
						Singleton<ShenFuSystem>.instance.PreLoadResource(triggerActionWrapper, ref loadInfo, this);
					}
				}
				if (areaEventTrigger.PresetActWrapper != null)
				{
					areaEventTrigger.PresetActWrapper.PreLoadResource(ref loadInfo, this.ageCheckerSet, this);
				}
			}
		}
		GlobalTrigger[] array2 = Object.FindObjectsOfType<GlobalTrigger>();
		if (array2 != null && array2.Length > 0)
		{
			for (int k = 0; k < array2.Length; k++)
			{
				GlobalTrigger globalTrigger = array2[k];
				if (globalTrigger.TriggerMatches != null && globalTrigger.TriggerMatches.Length > 0)
				{
					for (int l = 0; l < globalTrigger.TriggerMatches.Length; l++)
					{
						CTriggerMatch cTriggerMatch = globalTrigger.TriggerMatches[l];
						if (cTriggerMatch != null && cTriggerMatch.ActionList != null && cTriggerMatch.ActionList.Length > 0)
						{
							for (int m = 0; m < cTriggerMatch.ActionList.Length; m++)
							{
								TriggerActionWrapper triggerActionWrapper2 = cTriggerMatch.ActionList[m];
								if (triggerActionWrapper2 != null)
								{
									triggerActionWrapper2.PreLoadResource(ref loadInfo, this.ageCheckerSet, this);
								}
							}
						}
					}
				}
				if (globalTrigger.ActionList != null && globalTrigger.ActionList.Length > 0)
				{
					for (int n = 0; n < globalTrigger.ActionList.Length; n++)
					{
						TriggerActionWrapper triggerActionWrapper3 = globalTrigger.ActionList[n];
						if (triggerActionWrapper3 != null)
						{
							triggerActionWrapper3.PreLoadResource(ref loadInfo, this.ageCheckerSet, this);
						}
					}
				}
			}
		}
	}

	public void GetMapSkills(out int skillId, out int skillId2, out int ornamentSkillId, out int passiveSkillId)
	{
		skillId = (skillId2 = (ornamentSkillId = (passiveSkillId = 0)));
		SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
		if (curLvelContext != null)
		{
			int mapID = curLvelContext.m_mapID;
			if (mapID > 0)
			{
				skillId = curLvelContext.m_extraSkillId;
				skillId2 = curLvelContext.m_extraSkill2Id;
				ornamentSkillId = curLvelContext.m_ornamentSkillId;
				passiveSkillId = curLvelContext.m_extraPassiveSkillId;
			}
		}
	}

	public void AddPreloadActor(ref List<ActorPreloadTab> list, ref ActorMeta actorMeta, float spawnCnt, int ownerSkinID = 0)
	{
		ActorStaticData inStaticData = default(ActorStaticData);
		IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
		actorDataProvider.GetActorStaticData(ref actorMeta, ref inStaticData);
		ActorServerData actorServerData = default(ActorServerData);
		IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
		actorDataProvider2.GetActorServerData(ref actorMeta, ref actorServerData);
		CActorInfo actorInfo = CActorInfo.GetActorInfo(inStaticData.TheResInfo.ResPath, enResourceType.BattleScene);
		if (actorInfo == null)
		{
			return;
		}
		for (int i = 0; i < list.get_Count(); i++)
		{
			ActorPreloadTab actorPreloadTab = list.get_Item(i);
			if (actorPreloadTab.theActor.ActorType == actorMeta.ActorType && actorPreloadTab.theActor.ConfigId == actorMeta.ConfigId)
			{
				if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || actorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
				{
					int actorMarkID = this.GetActorMarkID(actorMeta);
					if (actorMarkID != actorPreloadTab.MarkID)
					{
						goto IL_13B;
					}
					uint skillID = 0u;
					if (actorDataProvider2.GetActorServerCommonSkillData(ref actorMeta, out skillID))
					{
						this.AnalyseSkill(ref actorPreloadTab, (int)skillID);
					}
				}
				else if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && spawnCnt > 0f)
				{
					actorPreloadTab.spawnCnt += spawnCnt;
					list.set_Item(i, actorPreloadTab);
				}
				LoaderHelper.retCnt++;
				return;
			}
			IL_13B:;
		}
		ActorPreloadTab actorPreloadTab2 = new ActorPreloadTab();
		actorPreloadTab2.theActor = actorMeta;
		actorPreloadTab2.modelPrefab.assetPath = actorInfo.GetArtPrefabName((ownerSkinID != 0) ? ownerSkinID : ((int)actorServerData.SkinId), -1);
		actorPreloadTab2.modelPrefab.nInstantiate = 1;
		actorPreloadTab2.spawnCnt = spawnCnt;
		actorPreloadTab2.MarkID = this.GetActorMarkID(actorMeta);
		actorPreloadTab2.ageActions = new List<AssetLoadBase>();
		actorPreloadTab2.parPrefabs = new List<AssetLoadBase>();
		actorPreloadTab2.mesPrefabs = new List<AssetLoadBase>();
		if (actorServerData.SkinId != 0u)
		{
			actorInfo.PreLoadAdvanceSkin(actorPreloadTab2.mesPrefabs, actorServerData.SkinId, -1);
		}
		actorPreloadTab2.spritePrefabs = new List<AssetLoadBase>();
		actorPreloadTab2.soundBanks = new List<AssetLoadBase>();
		actorPreloadTab2.behaviorXml = new List<AssetLoadBase>();
		ActorStaticSkillData actorStaticSkillData = default(ActorStaticSkillData);
		for (int j = 0; j < 8; j++)
		{
			actorDataProvider.GetActorStaticSkillData(ref actorMeta, (ActorSkillSlot)j, ref actorStaticSkillData);
			this.AnalyseSkill(ref actorPreloadTab2, actorStaticSkillData.SkillId);
			this.AnalysePassiveSkill(ref actorPreloadTab2, actorStaticSkillData.PassiveSkillId);
		}
		if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || actorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
		{
			int skillID2;
			int skillID3;
			int skillID4;
			int passiveSkillID;
			this.GetMapSkills(out skillID2, out skillID3, out skillID4, out passiveSkillID);
			this.AnalyseSkill(ref actorPreloadTab2, skillID2);
			this.AnalyseSkill(ref actorPreloadTab2, skillID3);
			this.AnalyseSkill(ref actorPreloadTab2, skillID4);
			this.AnalysePassiveSkill(ref actorPreloadTab2, passiveSkillID);
			uint skillID5 = 0u;
			if (actorDataProvider2.GetActorServerCommonSkillData(ref actorMeta, out skillID5))
			{
				this.AnalyseSkill(ref actorPreloadTab2, (int)skillID5);
			}
			this.AnalyseHeroBornAndReviveAge(ref actorPreloadTab2, actorMeta.ConfigId);
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)actorMeta.ConfigId);
			if (dataByKey != null)
			{
				this.AnalysePassiveSkill(ref actorPreloadTab2, dataByKey.iPassiveID1);
				this.AnalysePassiveSkill(ref actorPreloadTab2, dataByKey.iPassiveID2);
			}
		}
		else if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
		{
			ActorStaticData actorStaticData = default(ActorStaticData);
			IGameActorDataProvider actorDataProvider3 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			actorDataProvider3.GetActorStaticData(ref actorMeta, ref actorStaticData);
			int randomPassiveSkillRule = actorStaticData.TheBaseAttribute.RandomPassiveSkillRule;
			if (randomPassiveSkillRule > 0 && !this.randomSkillCheckerSet.ContainsKey(randomPassiveSkillRule))
			{
				this.randomSkillCheckerSet.Add(randomPassiveSkillRule, true);
				ResRandomSkillPassiveRule dataByKey2 = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long)randomPassiveSkillRule);
				if (dataByKey2.astRandomSkillPassiveID1 != null && dataByKey2.astRandomSkillPassiveID1.Length > 0)
				{
					for (int k = 0; k < dataByKey2.astRandomSkillPassiveID1.Length; k++)
					{
						this.AnalysePassiveSkill(ref actorPreloadTab2, dataByKey2.astRandomSkillPassiveID1[k].iParam);
					}
				}
				if (dataByKey2.astRandomSkillPassiveID2 != null && dataByKey2.astRandomSkillPassiveID2.Length > 0)
				{
					for (int l = 0; l < dataByKey2.astRandomSkillPassiveID2.Length; l++)
					{
						this.AnalysePassiveSkill(ref actorPreloadTab2, dataByKey2.astRandomSkillPassiveID2[l].iParam);
					}
				}
			}
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(actorMeta.ConfigId);
			if (dataCfgInfoByCurLevelDiff != null && dataCfgInfoByCurLevelDiff.iBufDropID != 0 && dataCfgInfoByCurLevelDiff.iBufDropRate > 0)
			{
				ShenFuSystem.PreLoadShenfuResource(dataCfgInfoByCurLevelDiff.iBufDropID, ref actorPreloadTab2, this);
			}
			if (actorStaticData.TheMonsterOnlyInfo.SoldierType == 8 && dataCfgInfoByCurLevelDiff != null)
			{
				ListView<Assets.Scripts.GameLogic.SoldierWave> listView = new ListView<Assets.Scripts.GameLogic.SoldierWave>();
				int iKillByCamp1ChangeSoldierWave = dataCfgInfoByCurLevelDiff.iKillByCamp1ChangeSoldierWave;
				int iKillByCamp2ChangeSoldierWave = dataCfgInfoByCurLevelDiff.iKillByCamp2ChangeSoldierWave;
				listView.AddRange(SoldierRegion.GetWavesForPreLoad(iKillByCamp1ChangeSoldierWave));
				listView.AddRange(SoldierRegion.GetWavesForPreLoad(iKillByCamp2ChangeSoldierWave));
				for (int m = 0; m < listView.Count; m++)
				{
					Assets.Scripts.GameLogic.SoldierWave soldierWave = listView[m];
					if (soldierWave != null && soldierWave.WaveInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo.Length > 0)
					{
						for (int n = 0; n < soldierWave.WaveInfo.astNormalSoldierInfo.Length; n++)
						{
							ResSoldierTypeInfo resSoldierTypeInfo = soldierWave.WaveInfo.astNormalSoldierInfo[n];
							if (resSoldierTypeInfo.dwSoldierID == 0u)
							{
								break;
							}
							ActorMeta actorMeta2 = default(ActorMeta);
							ActorMeta actorMeta3 = actorMeta2;
							actorMeta3.ActorType = ActorTypeDef.Actor_Type_Monster;
							actorMeta3.ConfigId = (int)resSoldierTypeInfo.dwSoldierID;
							actorMeta2 = actorMeta3;
							this.AddPreloadActor(ref list, ref actorMeta2, resSoldierTypeInfo.dwSoldierNum * 2f, 0);
						}
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(actorInfo.deadAgePath))
		{
			actorPreloadTab2.ageActions.Add(new AssetLoadBase
			{
				assetPath = actorInfo.deadAgePath
			});
		}
		if (!string.IsNullOrEmpty(actorInfo.BtResourcePath) && !this.behaviorXmlSet.ContainsKey(actorInfo.BtResourcePath))
		{
			actorPreloadTab2.behaviorXml.Add(new AssetLoadBase
			{
				assetPath = actorInfo.BtResourcePath
			});
			this.behaviorXmlSet.Add(actorInfo.BtResourcePath, true);
		}
		actorPreloadTab2.soundBanks = new List<AssetLoadBase>();
		this.AnalyseSoundBanks(ref actorPreloadTab2, ref actorInfo, ref actorServerData);
		list.Add(actorPreloadTab2);
		if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
		{
			this.CheckCallMonsterSkill(actorInfo, ref list, ref actorMeta, (int)actorServerData.SkinId);
		}
		else if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
		{
			this.CheckOrganSoldierWave(ref list, ref actorMeta, inStaticData);
		}
	}

	private void CheckCallMonsterSkill(CActorInfo InActorInfo, ref List<ActorPreloadTab> InPreloadListRef, ref ActorMeta InParentActorMetaRef, int markID)
	{
		if (InActorInfo != null && InActorInfo.callMonsterConfigID > 0)
		{
			ResCallMonster dataByKey = GameDataMgr.callMonsterDatabin.GetDataByKey((long)InActorInfo.callMonsterConfigID);
			if (dataByKey != null)
			{
				ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo((int)dataByKey.dwMonsterID, 1);
				if (dataCfgInfo != null)
				{
					ActorMeta actorMeta = default(ActorMeta);
					actorMeta.ActorType = ActorTypeDef.Actor_Type_Monster;
					actorMeta.ActorCamp = InParentActorMetaRef.ActorCamp;
					actorMeta.ConfigId = dataCfgInfo.iCfgID;
					this.AddPreloadActor(ref InPreloadListRef, ref actorMeta, 2f, markID);
				}
			}
		}
	}

	private void CheckOrganSoldierWave(ref List<ActorPreloadTab> InPreloadListRef, ref ActorMeta InOrganActorMetaRef, ActorStaticData InStaticData)
	{
		if (InOrganActorMetaRef.ActorType == ActorTypeDef.Actor_Type_Organ && InStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
		{
			ResSoldierWaveInfo dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey((uint)InStaticData.TheOrganOnlyInfo.DeadEnemySoldier);
			int num = 0;
			while (dataByKey != null && ++num < 100)
			{
				for (int i = 0; i < dataByKey.astNormalSoldierInfo.Length; i++)
				{
					ResSoldierTypeInfo resSoldierTypeInfo = dataByKey.astNormalSoldierInfo[i];
					if (resSoldierTypeInfo.dwSoldierID == 0u)
					{
						break;
					}
					ActorMeta actorMeta = default(ActorMeta);
					ActorMeta actorMeta2 = actorMeta;
					actorMeta2.ActorType = ActorTypeDef.Actor_Type_Monster;
					actorMeta2.ConfigId = (int)resSoldierTypeInfo.dwSoldierID;
					actorMeta = actorMeta2;
					this.AddPreloadActor(ref InPreloadListRef, ref actorMeta, 2f, 0);
				}
				dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey(dataByKey.dwNextSoldierWaveID);
			}
		}
	}

	public string GetCheckerKey(string assetPath, int markID)
	{
		string text = assetPath;
		if (markID != 0)
		{
			text = "<<" + markID + ">>";
			text += assetPath;
		}
		return text;
	}

	public List<ActorPreloadTab> AnalyseAgeRefAssets(Dictionary<long, Dictionary<object, AssetRefType>> refAssetsDict)
	{
		List<ActorPreloadTab> list = new List<ActorPreloadTab>();
		Dictionary<long, Dictionary<object, AssetRefType>>.Enumerator enumerator = refAssetsDict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<long, Dictionary<object, AssetRefType>> current = enumerator.get_Current();
			long key = current.get_Key();
			int num = (int)(key >> 32);
			int num2 = (int)(key & (long)((ulong)-1));
			KeyValuePair<long, Dictionary<object, AssetRefType>> current2 = enumerator.get_Current();
			Dictionary<object, AssetRefType> value = current2.get_Value();
			ActorPreloadTab actorPreloadTab = new ActorPreloadTab();
			actorPreloadTab.ageActions = new List<AssetLoadBase>();
			actorPreloadTab.parPrefabs = new List<AssetLoadBase>();
			actorPreloadTab.mesPrefabs = new List<AssetLoadBase>();
			actorPreloadTab.spritePrefabs = new List<AssetLoadBase>();
			actorPreloadTab.soundBanks = new List<AssetLoadBase>();
			actorPreloadTab.behaviorXml = new List<AssetLoadBase>();
			actorPreloadTab.MarkID = num;
			actorPreloadTab.theActor.ConfigId = num2;
			list.Add(actorPreloadTab);
			Dictionary<object, AssetRefType>.Enumerator enumerator2 = value.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<object, AssetRefType> current3 = enumerator2.get_Current();
				AssetRefType value2 = current3.get_Value();
				KeyValuePair<object, AssetRefType> current4 = enumerator2.get_Current();
				object key2 = current4.get_Key();
				switch (value2)
				{
				case AssetRefType.Action:
				{
					AssetLoadBase assetLoadBase = default(AssetLoadBase);
					assetLoadBase.assetPath = (key2 as string);
					string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, num);
					if (!this.ageCheckerSet.ContainsKey(checkerKey))
					{
						actorPreloadTab.ageActions.Add(assetLoadBase);
						this.ageCheckerSet.Add(checkerKey, true);
					}
					break;
				}
				case AssetRefType.SkillID:
				{
					KeyValuePair<object, AssetRefType> current5 = enumerator2.get_Current();
					ulong num3 = (ulong)current5.get_Key();
					int skillID = (int)(num3 & (ulong)-1);
					this.AnalyseSkill(ref actorPreloadTab, skillID);
					break;
				}
				case AssetRefType.SkillCombine:
				{
					KeyValuePair<object, AssetRefType> current6 = enumerator2.get_Current();
					ulong num4 = (ulong)current6.get_Key();
					int combineId = (int)(num4 & (ulong)-1);
					this.AnalyseSkillCombine(ref actorPreloadTab, combineId);
					break;
				}
				case AssetRefType.Prefab:
				{
					AssetLoadBase assetLoadBase2 = default(AssetLoadBase);
					assetLoadBase2.assetPath = (key2 as string);
					actorPreloadTab.mesPrefabs.Add(assetLoadBase2);
					if (num != 0)
					{
						for (int i = 0; i < 3; i++)
						{
							AssetLoadBase assetLoadBase3 = default(AssetLoadBase);
							assetLoadBase3.assetPath = SkinResourceHelper.GetSkinResourceName(num2, num, assetLoadBase2.assetPath, i);
							actorPreloadTab.mesPrefabs.Add(assetLoadBase3);
						}
					}
					break;
				}
				case AssetRefType.Particle:
				{
					AssetLoadBase assetLoadBase4 = default(AssetLoadBase);
					KeyValuePair<object, AssetRefType> current7 = enumerator2.get_Current();
					assetLoadBase4.assetPath = (current7.get_Key() as string);
					actorPreloadTab.parPrefabs.Add(assetLoadBase4);
					if (num != 0)
					{
						for (int j = 0; j < 3; j++)
						{
							AssetLoadBase assetLoadBase5 = default(AssetLoadBase);
							assetLoadBase5.assetPath = SkinResourceHelper.GetSkinResourceName(num2, num, assetLoadBase4.assetPath, j);
							actorPreloadTab.parPrefabs.Add(assetLoadBase5);
						}
					}
					break;
				}
				case AssetRefType.MonsterConfigId:
				{
					KeyValuePair<object, AssetRefType> current8 = enumerator2.get_Current();
					ulong num5 = (ulong)current8.get_Key();
					int configId = (int)(num5 & (ulong)-1);
					ActorMeta actorMeta = default(ActorMeta);
					actorMeta.ActorType = ActorTypeDef.Actor_Type_Monster;
					actorMeta.ConfigId = configId;
					actorMeta.ActorCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
					this.AddPreloadActor(ref list, ref actorMeta, 0f, 0);
					break;
				}
				case AssetRefType.CallActorConfigId:
				{
					KeyValuePair<object, AssetRefType> current9 = enumerator2.get_Current();
					ulong num6 = (ulong)current9.get_Key();
					int configId2 = (int)(num6 & (ulong)-1);
					ActorMeta actorMeta2 = default(ActorMeta);
					actorMeta2.ActorType = ActorTypeDef.Actor_Type_Hero;
					actorMeta2.ConfigId = configId2;
					actorMeta2.ActorCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
					this.AddPreloadActor(ref list, ref actorMeta2, 0f, 0);
					break;
				}
				}
			}
		}
		return list;
	}

	public void AnalysePassiveSkill(ref ActorPreloadTab loadInfo, int passiveSkillID)
	{
		if (passiveSkillID <= 0)
		{
			return;
		}
		ResSkillPassiveCfgInfo dataByKey = GameDataMgr.skillPassiveDatabin.GetDataByKey((long)passiveSkillID);
		if (dataByKey == null)
		{
			return;
		}
		if (!this.passiveSkillList.Contains((long)passiveSkillID))
		{
			this.passiveSkillList.Add((long)passiveSkillID);
		}
		AssetLoadBase assetLoadBase = default(AssetLoadBase);
		assetLoadBase.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szActionName);
		string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, loadInfo.MarkID);
		if (!this.ageCheckerSet.ContainsKey(checkerKey))
		{
			loadInfo.ageActions.Add(assetLoadBase);
			this.ageCheckerSet.Add(checkerKey, true);
		}
		if (dataByKey.dwPassiveEventType == 8u)
		{
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam4);
		}
		else if (dataByKey.dwPassiveEventType == 3u || dataByKey.dwPassiveEventType == 7u)
		{
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam1);
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam2);
		}
		else if (dataByKey.dwPassiveEventType == 9u)
		{
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam3);
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam4);
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam5);
		}
		else if (dataByKey.dwPassiveEventType == 10u)
		{
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam3);
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam4);
			this.AnalyseSkillCombine(ref loadInfo, dataByKey.iPassiveEventParam5);
		}
	}

	public void AnalyseSkill(ref ActorPreloadTab loadInfo, int skillID)
	{
		if (skillID <= 0)
		{
			return;
		}
		ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long)skillID);
		if (dataByKey == null)
		{
			return;
		}
		if (!this.skillIdList.Contains((long)skillID))
		{
			this.skillIdList.Add((long)skillID);
		}
		AssetLoadBase assetLoadBase = default(AssetLoadBase);
		assetLoadBase.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szPrefab);
		string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, loadInfo.MarkID);
		if (this.ageCheckerSet.ContainsKey(checkerKey))
		{
			return;
		}
		loadInfo.ageActions.Add(assetLoadBase);
		this.ageCheckerSet.Add(checkerKey, true);
		string text = StringHelper.UTF8BytesToString(ref dataByKey.szGuidePrefab);
		if (!string.IsNullOrEmpty(text))
		{
			assetLoadBase.assetPath = text;
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
		}
		text = StringHelper.UTF8BytesToString(ref dataByKey.szGuideWarnPrefab);
		if (!string.IsNullOrEmpty(text))
		{
			assetLoadBase.assetPath = text;
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
		}
		text = StringHelper.UTF8BytesToString(ref dataByKey.szEffectPrefab);
		if (!string.IsNullOrEmpty(text))
		{
			assetLoadBase.assetPath = text;
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
		}
		text = StringHelper.UTF8BytesToString(ref dataByKey.szEffectWarnPrefab);
		if (!string.IsNullOrEmpty(text))
		{
			assetLoadBase.assetPath = text;
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
		}
	}

	public void AnalyseHeroBornAndReviveAge(ref ActorPreloadTab loadInfo, int configID)
	{
		ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)configID);
		if (dataByKey != null)
		{
			AssetLoadBase assetLoadBase = default(AssetLoadBase);
			assetLoadBase.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szBorn_Age);
			string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, loadInfo.MarkID);
			if (!this.ageCheckerSet.ContainsKey(checkerKey))
			{
				loadInfo.ageActions.Add(assetLoadBase);
				this.ageCheckerSet.Add(checkerKey, true);
			}
			AssetLoadBase assetLoadBase2 = default(AssetLoadBase);
			assetLoadBase2.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szRevive_Age);
			string checkerKey2 = this.GetCheckerKey(assetLoadBase2.assetPath, loadInfo.MarkID);
			if (!this.ageCheckerSet.ContainsKey(checkerKey2))
			{
				loadInfo.ageActions.Add(assetLoadBase2);
				this.ageCheckerSet.Add(checkerKey2, true);
			}
		}
	}

	public void AnalyseSkillMarkParticleSkin(ref ActorPreloadTab loadInfo, string _resName)
	{
		for (int i = 0; i < 3; i++)
		{
			AssetLoadBase assetLoadBase = default(AssetLoadBase);
			assetLoadBase.assetPath = SkinResourceHelper.GetSkinResourceName(loadInfo.theActor.ConfigId, loadInfo.MarkID, _resName, i);
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
		}
	}

	public void AnalyseSkillMarkParticle(ref ActorPreloadTab loadInfo, string _resName)
	{
		AssetLoadBase assetLoadBase = default(AssetLoadBase);
		string text = StringHelper.UTF8BytesToString(ref _resName);
		if (!string.IsNullOrEmpty(text))
		{
			assetLoadBase.assetPath = text;
			assetLoadBase.nInstantiate = 1;
			loadInfo.parPrefabs.Add(assetLoadBase);
			this.AnalyseSkillMarkParticleSkin(ref loadInfo, text);
		}
	}

	public void AnalyseSkillMark(ref ActorPreloadTab loadInfo, int markId)
	{
		if (markId > 0)
		{
			ResSkillMarkCfgInfo dataByKey = GameDataMgr.skillMarkDatabin.GetDataByKey((long)markId);
			if (dataByKey != null)
			{
				AssetLoadBase assetLoadBase = default(AssetLoadBase);
				assetLoadBase.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szActionName);
				string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, loadInfo.MarkID);
				if (!this.ageCheckerSet.ContainsKey(checkerKey))
				{
					loadInfo.ageActions.Add(assetLoadBase);
					this.ageCheckerSet.Add(checkerKey, true);
				}
				this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName1);
				this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName2);
				this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName3);
				this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName4);
				this.AnalyseSkillMarkParticle(ref loadInfo, dataByKey.szLayerEffectName5);
			}
		}
	}

	public void AnalyseSkillCombine(ref ActorPreloadTab loadInfo, int combineId)
	{
		if (combineId > 0)
		{
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)combineId);
			if (dataByKey != null)
			{
				if (!this.skillCombineList.Contains((long)combineId))
				{
					this.skillCombineList.Add((long)combineId);
				}
				AssetLoadBase assetLoadBase = default(AssetLoadBase);
				assetLoadBase.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szPrefab);
				string checkerKey = this.GetCheckerKey(assetLoadBase.assetPath, loadInfo.MarkID);
				if (!this.ageCheckerSet.ContainsKey(checkerKey))
				{
					loadInfo.ageActions.Add(assetLoadBase);
					this.ageCheckerSet.Add(checkerKey, true);
				}
				if (dataByKey.bIsShowBuff == 1 && !string.IsNullOrEmpty(dataByKey.szIconPath))
				{
					string path = CUIUtility.s_Sprite_Dynamic_Skill_Dir + dataByKey.szIconPath;
					if (!loadInfo.IsExistsSprite(path))
					{
						loadInfo.AddSprite(path);
					}
				}
				if (dataByKey.astSkillFuncInfo != null && dataByKey.astSkillFuncInfo.Length > 0)
				{
					for (int i = 0; i < dataByKey.astSkillFuncInfo.Length; i++)
					{
						ResDT_SkillFunc resDT_SkillFunc = dataByKey.astSkillFuncInfo[i];
						if ((resDT_SkillFunc.bSkillFuncType == 28 || resDT_SkillFunc.bSkillFuncType == 33 || resDT_SkillFunc.bSkillFuncType == 54 || resDT_SkillFunc.bSkillFuncType == 55 || resDT_SkillFunc.bSkillFuncType == 27 || resDT_SkillFunc.bSkillFuncType == 84 || resDT_SkillFunc.bSkillFuncType == 32) && resDT_SkillFunc.astSkillFuncParam != null && resDT_SkillFunc.astSkillFuncParam.Length != 0)
						{
							int num = 0;
							int num2 = 0;
							int combineId2 = 0;
							int combineId3 = 0;
							if (resDT_SkillFunc.astSkillFuncParam[0] != null)
							{
								num = resDT_SkillFunc.astSkillFuncParam[0].iParam;
							}
							if (resDT_SkillFunc.astSkillFuncParam[1] != null)
							{
								num2 = resDT_SkillFunc.astSkillFuncParam[1].iParam;
							}
							if (resDT_SkillFunc.astSkillFuncParam[5] != null)
							{
								combineId2 = resDT_SkillFunc.astSkillFuncParam[5].iParam;
							}
							if (resDT_SkillFunc.astSkillFuncParam[6] != null)
							{
								combineId3 = resDT_SkillFunc.astSkillFuncParam[6].iParam;
							}
							if (resDT_SkillFunc.bSkillFuncType == 28)
							{
								this.AnalyseSkillMark(ref loadInfo, num);
							}
							else if (resDT_SkillFunc.bSkillFuncType == 33)
							{
								if (combineId != num)
								{
									this.AnalyseSkillCombine(ref loadInfo, num);
								}
							}
							else if (resDT_SkillFunc.bSkillFuncType == 54)
							{
								this.AnalyseSkillCombine(ref loadInfo, num);
							}
							else if (resDT_SkillFunc.bSkillFuncType == 55)
							{
								this.AnalyseSkill(ref loadInfo, num2);
							}
							else if (resDT_SkillFunc.bSkillFuncType == 27)
							{
								this.AnalyseSkillCombine(ref loadInfo, combineId2);
								this.AnalyseSkillCombine(ref loadInfo, combineId3);
							}
							else if (resDT_SkillFunc.bSkillFuncType == 84)
							{
								this.AnalyseSkillCombine(ref loadInfo, num);
								this.AnalyseSkillCombine(ref loadInfo, num2);
							}
							else if (resDT_SkillFunc.bSkillFuncType == 32)
							{
								this.AnalyseSkillCombine(ref loadInfo, combineId3);
							}
						}
					}
				}
			}
		}
	}

	public void AnalyseSkillCombine(ref List<string> ageList, int combineId)
	{
		if (combineId > 0)
		{
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)combineId);
			if (dataByKey != null)
			{
				ageList.Add(StringHelper.UTF8BytesToString(ref dataByKey.szPrefab));
			}
		}
	}

	public void AnalyseSoundBanks(ref ActorPreloadTab loadInfo, ref CActorInfo charInfo, ref ActorServerData serverData)
	{
		if (charInfo.SoundBanks != null && charInfo.SoundBanks.Length > 0)
		{
			for (int i = 0; i < charInfo.SoundBanks.Length; i++)
			{
				AssetLoadBase assetLoadBase = default(AssetLoadBase);
				assetLoadBase.assetPath = charInfo.SoundBanks[i];
				loadInfo.soundBanks.Add(assetLoadBase);
			}
		}
		if (serverData.SkinId != 0u)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint)loadInfo.theActor.ConfigId, serverData.SkinId);
			if (heroSkin != null && !string.IsNullOrEmpty(heroSkin.szSkinSoundResPack))
			{
				AssetLoadBase assetLoadBase2 = default(AssetLoadBase);
				assetLoadBase2.assetPath = heroSkin.szSkinSoundResPack + "_SFX";
				loadInfo.soundBanks.Add(assetLoadBase2);
				assetLoadBase2 = default(AssetLoadBase);
				assetLoadBase2.assetPath = heroSkin.szSkinSoundResPack + "_VO";
				loadInfo.soundBanks.Add(assetLoadBase2);
			}
		}
	}

	public int GetActorMarkID(ActorMeta actorMeta)
	{
		int result = 0;
		IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
		ActorServerData actorServerData = default(ActorServerData);
		if (actorDataProvider != null && actorDataProvider.GetActorServerData(ref actorMeta, ref actorServerData))
		{
			int skinId = (int)actorServerData.SkinId;
			if (skinId != 0)
			{
				result = (int)CSkinInfo.GetSkinCfgId((uint)actorMeta.ConfigId, (uint)skinId);
			}
		}
		return result;
	}

	[DebuggerHidden]
	public IEnumerator GetGlobalPreload(ActorPreloadTab result)
	{
		LoaderHelper.<GetGlobalPreload>c__Iterator3F <GetGlobalPreload>c__Iterator3F = new LoaderHelper.<GetGlobalPreload>c__Iterator3F();
		<GetGlobalPreload>c__Iterator3F.result = result;
		<GetGlobalPreload>c__Iterator3F.<$>result = result;
		<GetGlobalPreload>c__Iterator3F.<>f__this = this;
		return <GetGlobalPreload>c__Iterator3F;
	}

	[DebuggerHidden]
	public IEnumerator ReduceSkillRefDatabin()
	{
		LoaderHelper.<ReduceSkillRefDatabin>c__Iterator40 <ReduceSkillRefDatabin>c__Iterator = new LoaderHelper.<ReduceSkillRefDatabin>c__Iterator40();
		<ReduceSkillRefDatabin>c__Iterator.<>f__this = this;
		return <ReduceSkillRefDatabin>c__Iterator;
	}
}

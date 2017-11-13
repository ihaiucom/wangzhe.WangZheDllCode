using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PreloadActionAssets
{
	private DictionaryView<string, Action> actionDict = new DictionaryView<string, Action>();

	private Dictionary<string, bool> prefabDict = new Dictionary<string, bool>();

	private Dictionary<string, bool> behaviourDict = new Dictionary<string, bool>();

	private float MaxDurationPerFrame = 0.1f;

	private float loadTime;

	private float curTime;

	private void AddAction(DictionaryView<string, Action> actions, byte[] actionNameUtf8)
	{
		string text = StringHelper.UTF8BytesToString(ref actionNameUtf8);
		if (text != null && !this.actionDict.ContainsKey(text) && !actions.ContainsKey(text))
		{
			actions.Add(text, null);
		}
	}

	private void AddAction(DictionaryView<string, Action> actions, string actionName)
	{
		if (actionName != null && !this.actionDict.ContainsKey(actionName) && !actions.ContainsKey(actionName))
		{
			actions.Add(actionName, null);
		}
	}

	public void AddRefPrefab(string prefabName, bool isParticle)
	{
		prefabName = CFileManager.EraseExtension(prefabName);
		if (!this.prefabDict.ContainsKey(prefabName))
		{
			this.prefabDict.Add(prefabName, isParticle);
		}
	}

	private void AddActionsFromSkillCombine(DictionaryView<string, Action> actions, int skillCombineID)
	{
		if (skillCombineID > 0)
		{
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)skillCombineID);
			if (dataByKey != null)
			{
				this.AddAction(actions, dataByKey.szPrefab);
			}
		}
	}

	private void AddActionsFromRandPassiveSkill(DictionaryView<string, Action> actions, int randPassiveSkillID)
	{
		if (randPassiveSkillID > 0)
		{
			ResRandomSkillPassiveRule dataByKey = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long)randPassiveSkillID);
			if (dataByKey != null)
			{
				if (dataByKey.astRandomSkillPassiveID1 != null && dataByKey.astRandomSkillPassiveID1.Length > 0)
				{
					for (int i = 0; i < dataByKey.astRandomSkillPassiveID1.Length; i++)
					{
						this.AddActionsFromPassiveSkill(actions, dataByKey.astRandomSkillPassiveID1[i].iParam);
					}
				}
				if (dataByKey.astRandomSkillPassiveID2 != null && dataByKey.astRandomSkillPassiveID2.Length > 0)
				{
					for (int j = 0; j < dataByKey.astRandomSkillPassiveID2.Length; j++)
					{
						this.AddActionsFromPassiveSkill(actions, dataByKey.astRandomSkillPassiveID2[j].iParam);
					}
				}
			}
		}
	}

	private void AddActionsFromPassiveSkill(DictionaryView<string, Action> actions, int passiveSkillID)
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
		this.AddAction(actions, dataByKey.szActionName);
	}

	private void AddActionsFromSkill(DictionaryView<string, Action> actions, int skillID)
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
		this.AddAction(actions, dataByKey.szPrefab);
		string text = StringHelper.UTF8BytesToString(ref dataByKey.szGuidePrefab);
		if (text != null && !this.prefabDict.ContainsKey(text))
		{
			this.prefabDict.Add(text, false);
		}
	}

	private void AddBehaviorTree(CActorInfo info)
	{
		if (info == null || info.BtResourcePath == null || info.BtResourcePath.get_Length() == 0)
		{
			return;
		}
		if (!this.behaviourDict.ContainsKey(info.BtResourcePath))
		{
			this.behaviourDict.Add(info.BtResourcePath, false);
		}
	}

	private void AddActionsFromActors(DictionaryView<string, Action> actions, List<PoolObjHandle<ActorRoot>> actors)
	{
		if (actors == null)
		{
			return;
		}
		for (int i = 0; i < actors.get_Count(); i++)
		{
			ActorRoot handle = actors.get_Item(i).handle;
			this.AddBehaviorTree(handle.CharInfo);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			ActorStaticSkillData actorStaticSkillData = default(ActorStaticSkillData);
			for (int j = 0; j < 8; j++)
			{
				actorDataProvider.GetActorStaticSkillData(ref handle.TheActorMeta, (ActorSkillSlot)j, ref actorStaticSkillData);
				if (actorStaticSkillData.SkillId != 0)
				{
					this.AddActionsFromSkill(actions, actorStaticSkillData.SkillId);
					this.AddActionsFromPassiveSkill(actions, actorStaticSkillData.PassiveSkillId);
				}
			}
		}
	}

	private void AddReferencedAssets(DictionaryView<string, Action> actions, Dictionary<object, AssetRefType> actionAssets)
	{
		Dictionary<object, AssetRefType>.Enumerator enumerator = actionAssets.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<object, AssetRefType> current = enumerator.get_Current();
			AssetRefType value = current.get_Value();
			switch (value)
			{
			case AssetRefType.Action:
			{
				KeyValuePair<object, AssetRefType> current2 = enumerator.get_Current();
				string key = current2.get_Key() as string;
				if (!this.actionDict.ContainsKey(key) && !actions.ContainsKey(key))
				{
					actions.Add(key, null);
				}
				break;
			}
			case AssetRefType.SkillID:
			{
				KeyValuePair<object, AssetRefType> current3 = enumerator.get_Current();
				ulong num = (ulong)current3.get_Key();
				int skillID = (int)(num & (ulong)-1);
				this.AddActionsFromSkill(actions, skillID);
				break;
			}
			case AssetRefType.SkillCombine:
			{
				KeyValuePair<object, AssetRefType> current4 = enumerator.get_Current();
				ulong num2 = (ulong)current4.get_Key();
				int skillCombineID = (int)(num2 & (ulong)-1);
				this.AddActionsFromSkillCombine(actions, skillCombineID);
				break;
			}
			case AssetRefType.Prefab:
			case AssetRefType.Particle:
			{
				KeyValuePair<object, AssetRefType> current5 = enumerator.get_Current();
				string text = current5.get_Key() as string;
				if (!this.prefabDict.ContainsKey(text))
				{
					this.prefabDict.Add(text, value == AssetRefType.Particle);
				}
				break;
			}
			case AssetRefType.MonsterConfigId:
			{
				KeyValuePair<object, AssetRefType> current6 = enumerator.get_Current();
				ulong num3 = (ulong)current6.get_Key();
				uint soldierID = (uint)(num3 & (ulong)-1);
				this.AddActionsFromSoldier(actions, soldierID);
				break;
			}
			}
		}
	}

	private void AddActionsFromSoldier(DictionaryView<string, Action> actions, uint soldierID)
	{
		ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int)soldierID);
		if (dataCfgInfoByCurLevelDiff == null)
		{
			return;
		}
		string path = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
		CActorInfo actorInfo = CActorInfo.GetActorInfo(path, enResourceType.BattleScene);
		if (actorInfo == null)
		{
			return;
		}
		this.AddBehaviorTree(actorInfo);
		string artPrefabName = actorInfo.GetArtPrefabName(0, -1);
		if (!this.prefabDict.ContainsKey(artPrefabName))
		{
			this.prefabDict.Add(artPrefabName, false);
		}
		if (dataCfgInfoByCurLevelDiff.SkillIDs != null && dataCfgInfoByCurLevelDiff.SkillIDs.Length > 0)
		{
			for (int i = 0; i < dataCfgInfoByCurLevelDiff.SkillIDs.Length; i++)
			{
				int skillID = dataCfgInfoByCurLevelDiff.SkillIDs[i];
				this.AddActionsFromSkill(actions, skillID);
			}
		}
	}

	private void AddActionsFromAreaTrigger(DictionaryView<string, Action> actions)
	{
		AreaTrigger[] array = Object.FindObjectsOfType<AreaTrigger>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			AreaTrigger areaTrigger = array[i];
			this.AddActionsFromSkillCombine(actions, areaTrigger.BuffID);
			this.AddActionsFromSkillCombine(actions, areaTrigger.UpdateBuffID);
		}
	}

	private void AddActionsFromActionHelper(DictionaryView<string, Action> actions)
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
				this.AddAction(actions, actionHelperStorage.actionName);
			}
		}
	}

	private void AddActionsFromSpawnPoints(DictionaryView<string, Action> actions)
	{
		SpawnPoint[] array = Object.FindObjectsOfType<SpawnPoint>();
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			SpawnPoint spawnPoint = array[i];
			if (spawnPoint != null && spawnPoint.TheActorsMeta.Length > 0)
			{
				for (int j = 0; j < spawnPoint.TheActorsMeta.Length; j++)
				{
					ActorMeta actorMeta = spawnPoint.TheActorsMeta[j];
					if (actorMeta.ConfigId > 0)
					{
						ActorTypeDef actorType = actorMeta.ActorType;
						if (actorType == ActorTypeDef.Actor_Type_Monster)
						{
							this.AddActionsFromSoldier(actions, (uint)actorMeta.ConfigId);
						}
					}
				}
			}
		}
	}

	private void AddActionsFromSoldierRegions(DictionaryView<string, Action> actions)
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
					SoldierWave soldierWave = soldierRegion.Waves[j];
					if (soldierWave != null && soldierWave.WaveInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo != null && soldierWave.WaveInfo.astNormalSoldierInfo.Length > 0)
					{
						for (int k = 0; k < soldierWave.WaveInfo.astNormalSoldierInfo.Length; k++)
						{
							ResSoldierTypeInfo resSoldierTypeInfo = soldierWave.WaveInfo.astNormalSoldierInfo[k];
							this.AddActionsFromSoldier(actions, resSoldierTypeInfo.dwSoldierID);
						}
					}
				}
			}
		}
	}

	private bool shouldWaitForNextFrame()
	{
		float num = Time.realtimeSinceStartup - this.curTime;
		if (num >= this.MaxDurationPerFrame)
		{
			this.loadTime += num;
			return true;
		}
		return false;
	}

	[DebuggerHidden]
	public IEnumerator Exec()
	{
		PreloadActionAssets.<Exec>c__Iterator41 <Exec>c__Iterator = new PreloadActionAssets.<Exec>c__Iterator41();
		<Exec>c__Iterator.<>f__this = this;
		return <Exec>c__Iterator;
	}
}

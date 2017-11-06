using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class ShenFuSystem : Singleton<ShenFuSystem>
	{
		public Dictionary<int, ShenFuObjects> _shenFuTriggerPool = new Dictionary<int, ShenFuObjects>();

		public List<PoolObjHandle<CTailsman>> m_charmList = new List<PoolObjHandle<CTailsman>>();

		public List<ShenFuObjects> _shenFuMonsterPool = new List<ShenFuObjects>();

		public override void Init()
		{
		}

		public override void UnInit()
		{
			this.ClearAll();
		}

		public void UpdateLogic(int inDelta)
		{
			if (this.m_charmList.get_Count() > 0)
			{
				int count = this.m_charmList.get_Count();
				for (int i = count - 1; i >= 0; i--)
				{
					this.m_charmList.get_Item(i).handle.UpdateLogic(inDelta);
				}
			}
			int j = 0;
			while (j < this._shenFuMonsterPool.get_Count())
			{
				ShenFuObjects shenFuObjects = this._shenFuMonsterPool.get_Item(j);
				shenFuObjects.PickTick -= inDelta;
				if (shenFuObjects.PickTick > 0)
				{
					this._shenFuMonsterPool.set_Item(j, shenFuObjects);
					j++;
				}
				else
				{
					uint num = shenFuObjects.PickUpRange * shenFuObjects.PickUpRange;
					List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
					int count2 = heroActors.get_Count();
					bool flag = false;
					for (int k = 0; k < count2; k++)
					{
						VInt3 location = heroActors.get_Item(k).handle.location;
						if ((location - (VInt3)shenFuObjects.ShenFu.transform.position).sqrMagnitude <= num)
						{
							this.OnShenFuEffect(heroActors.get_Item(k), shenFuObjects.ShenFuId);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						if (shenFuObjects.ShenFu != null)
						{
							Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(shenFuObjects.ShenFu);
						}
						this._shenFuMonsterPool.RemoveAt(j);
					}
					else
					{
						j++;
					}
				}
			}
		}

		public void AddCharm(PoolObjHandle<CTailsman> inCharm)
		{
			if (inCharm)
			{
				this.m_charmList.Add(inCharm);
			}
		}

		public void RemoveCharm(PoolObjHandle<CTailsman> inCharm)
		{
			if (inCharm)
			{
				this.m_charmList.Remove(inCharm);
			}
		}

		public void PreLoadResource(TriggerActionWrapper triggerActionWrapper, ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
		{
			if (triggerActionWrapper == null)
			{
				return;
			}
			ShenFuSystem.PreLoadShenfuResource(triggerActionWrapper.UpdateUniqueId, ref loadInfo, loadHelper);
		}

		public static void PreLoadShenfuResource(int shenfuId, ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
		{
			ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long)shenfuId);
			if (dataByKey == null)
			{
				return;
			}
			AssetLoadBase assetLoadBase = new AssetLoadBase
			{
				assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath)
			};
			loadInfo.mesPrefabs.Add(assetLoadBase);
			loadHelper.AnalyseSkillCombine(ref loadInfo, dataByKey.iBufId);
		}

		public void ClearAll()
		{
			Dictionary<int, ShenFuObjects>.Enumerator enumerator = this._shenFuTriggerPool.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ShenFuObjects> current = enumerator.get_Current();
				ShenFuObjects value = current.get_Value();
				if (value.ShenFu != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(value.ShenFu);
				}
			}
			this._shenFuTriggerPool.Clear();
			while (this.m_charmList.get_Count() > 0)
			{
				this.m_charmList.get_Item(0).handle.DoClearing();
			}
			for (int i = 0; i < this._shenFuMonsterPool.get_Count(); i++)
			{
				if (this._shenFuMonsterPool.get_Item(i).ShenFu != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this._shenFuMonsterPool.get_Item(i).ShenFu);
				}
			}
			this._shenFuMonsterPool.Clear();
		}

		public void OnShenfuStart(uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
		{
			if (trigger == null || shenFu == null)
			{
				return;
			}
			ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey(shenFuId);
			if (dataByKey == null)
			{
				return;
			}
			trigger.Radius = (int)dataByKey.dwGetRadius;
			ShenFuObjects shenFuObjects = default(ShenFuObjects);
			if (this.CreateShenFu(shenFuId, trigger.gameObject.transform.position, ref shenFuObjects, 0))
			{
				this._shenFuTriggerPool.Add(trigger.ID, shenFuObjects);
			}
		}

		public void OnShenfuHalt(uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
		{
		}

		public void OnShenFuEffect(PoolObjHandle<ActorRoot> actor, uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
		{
			ShenFuObjects shenFuObjects;
			if (this._shenFuTriggerPool.TryGetValue(trigger.ID, ref shenFuObjects))
			{
				if (shenFuObjects.ShenFu != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(shenFuObjects.ShenFu);
				}
				this._shenFuTriggerPool.Remove(trigger.ID);
			}
			this.OnShenFuEffect(actor, shenFuId);
		}

		public void OnShenFuStopped(TriggerActionShenFu inAction)
		{
			if (inAction == null)
			{
				return;
			}
			ShenFuObjects shenFuObjects;
			if (this._shenFuTriggerPool.TryGetValue(inAction.TriggerId, ref shenFuObjects))
			{
				if (shenFuObjects.ShenFu != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(shenFuObjects.ShenFu);
				}
				this._shenFuTriggerPool.Remove(inAction.TriggerId);
			}
		}

		public void CreateShenFuOnMonsterDead(PoolObjHandle<ActorRoot> monster, uint shenFuId)
		{
			if (!monster)
			{
				return;
			}
			ShenFuObjects shenFuObjects = default(ShenFuObjects);
			if (this.CreateShenFu(shenFuId, (Vector3)monster.handle.location, ref shenFuObjects, 2000))
			{
				this._shenFuMonsterPool.Add(shenFuObjects);
			}
		}

		public bool CreateShenFu(uint shenFuId, Vector3 createPosition, ref ShenFuObjects shenFuObj, int pickTick = 0)
		{
			ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey(shenFuId);
			if (dataByKey == null)
			{
				return false;
			}
			string prefabName = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath);
			shenFuObj.ShenFuId = shenFuId;
			shenFuObj.ShenFu = MonoSingleton<SceneMgr>.instance.InstantiateLOD(prefabName, false, SceneObjType.ActionRes, createPosition);
			shenFuObj.PickUpRange = dataByKey.dwGetRadius;
			shenFuObj.PickTick = pickTick;
			if (FogOfWar.enable)
			{
				COM_PLAYERCAMP horizonCamp = Singleton<WatchController>.instance.HorizonCamp;
				GameFowCollector.SetObjVisibleByFow(new PoolObjHandle<ActorRoot>(null), shenFuObj.ShenFu, Singleton<GameFowManager>.instance, horizonCamp);
			}
			return true;
		}

		public void OnShenFuEffect(PoolObjHandle<ActorRoot> actor, uint shenFuId)
		{
			ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey(shenFuId);
			if (dataByKey == null)
			{
				return;
			}
			BufConsumer bufConsumer = new BufConsumer(dataByKey.iBufId, actor, actor);
			if (bufConsumer.Use())
			{
			}
		}
	}
}

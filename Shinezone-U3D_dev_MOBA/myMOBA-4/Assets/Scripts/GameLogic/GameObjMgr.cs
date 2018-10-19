using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class GameObjMgr : Singleton<GameObjMgr>, IUpdateLogic
	{
		private uint _newActorID;

		public List<PoolObjHandle<ActorRoot>> GameActors = new List<PoolObjHandle<ActorRoot>>(100);

		public List<PoolObjHandle<ActorRoot>> DynamicActors = new List<PoolObjHandle<ActorRoot>>(10);

		public List<PoolObjHandle<ActorRoot>> StaticActors = new List<PoolObjHandle<ActorRoot>>(20);

		public List<PoolObjHandle<ActorRoot>> HeroActors = new List<PoolObjHandle<ActorRoot>>(10);

		public List<PoolObjHandle<ActorRoot>> OrganActors = new List<PoolObjHandle<ActorRoot>>(20);

		public List<PoolObjHandle<ActorRoot>> TowerActors = new List<PoolObjHandle<ActorRoot>>(16);

		public List<PoolObjHandle<ActorRoot>> MonsterActors = new List<PoolObjHandle<ActorRoot>>(20);

		public List<PoolObjHandle<ActorRoot>> CallActors = new List<PoolObjHandle<ActorRoot>>(10);

		private List<PoolObjHandle<ActorRoot>>[] CampsActors = new List<PoolObjHandle<ActorRoot>>[3];

		private List<KeyValuePair<uint, int>> DelayRecycle = new List<KeyValuePair<uint, int>>(10);

		public List<PoolObjHandle<ActorRoot>> SoldierActors = new List<PoolObjHandle<ActorRoot>>(40);

		public List<PoolObjHandle<ActorRoot>>[] CampsBullet = new List<PoolObjHandle<ActorRoot>>[3];

		public List<PoolObjHandle<ActorRoot>> FakeTrueEyes = new List<PoolObjHandle<ActorRoot>>(10);

		public Dictionary<int, List<ActorRoot>> CachedActors = new Dictionary<int, List<ActorRoot>>();

		public GameObject cachedRoot;

		public static bool isPreSpawnActors;

		private DefaultGameEventParam eventParamCache;

		public List<Vector3> PositionCamp1Records;

		public List<Vector3> PositionCamp2Records;

		public List<Vector3> PositionCampTotalRecords;

		public uint NewActorID
		{
			get
			{
				this._newActorID += 1u;
				return this._newActorID;
			}
			private set
			{
				this._newActorID = value;
			}
		}

		public override void Init()
		{
			this.NewActorID = 0u;
			for (int i = 0; i < 3; i++)
			{
				this.CampsActors[i] = new List<PoolObjHandle<ActorRoot>>(50);
			}
			for (int j = 0; j < 3; j++)
			{
				this.CampsBullet[j] = new List<PoolObjHandle<ActorRoot>>(50);
			}
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
			this.eventParamCache = new DefaultGameEventParam(new PoolObjHandle<ActorRoot>(null), new PoolObjHandle<ActorRoot>(null));
		}

		public override void UnInit()
		{
			this.ClearActor();
			base.UnInit();
		}

		public void UpdateLogic(int delta)
		{
			int count = this.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				this.GameActors[i].handle.UpdateLogic(delta);
			}
			int j = 0;
			while (j < this.DelayRecycle.Count)
			{
				uint key = this.DelayRecycle[j].Key;
				int num = this.DelayRecycle[j].Value - delta;
				if (num <= 0)
				{
					this.DestroyActor(key);
					this.DelayRecycle.RemoveAt(j);
				}
				else
				{
					this.DelayRecycle[j] = new KeyValuePair<uint, int>(key, num);
					j++;
				}
			}
		}

		public void LateUpdate()
		{
			int count = this.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.GameActors[i])
				{
					this.GameActors[i].handle.LateUpdate();
				}
			}
		}

		public void PrepareFight()
		{
			int count = this.StaticActors.Count;
			for (int i = 0; i < count; i++)
			{
				this.StaticActors[i].handle.InitActor();
				this.StaticActors[i].handle.PrepareFight();
			}
			int count2 = this.DynamicActors.Count;
			for (int j = 0; j < count2; j++)
			{
				this.DynamicActors[j].handle.InitActor();
				this.DynamicActors[j].handle.PrepareFight();
			}
			if (Singleton<BattleLogic>.GetInstance().m_LevelContext.IsGameTypeBurning())
			{
				BurnExpeditionUT.ApplyHP2Game(this.DynamicActors);
			}
			if (Singleton<WatchController>.GetInstance().CanShowActorIRPosMap())
			{
				if (this.PositionCamp1Records == null)
				{
					this.PositionCamp1Records = new List<Vector3>();
				}
				if (this.PositionCamp2Records == null)
				{
					this.PositionCamp2Records = new List<Vector3>();
				}
				if (this.PositionCampTotalRecords == null)
				{
					this.PositionCampTotalRecords = new List<Vector3>();
				}
				this.PositionCamp1Records.Clear();
				this.PositionCamp2Records.Clear();
				this.PositionCampTotalRecords.Clear();
			}
		}

		public void StartFight()
		{
			int count = this.StaticActors.Count;
			for (int i = 0; i < count; i++)
			{
				this.AddActor(this.StaticActors[i]);
			}
			int count2 = this.DynamicActors.Count;
			for (int j = 0; j < count2; j++)
			{
				this.AddActor(this.DynamicActors[j]);
			}
			int count3 = this.GameActors.Count;
			for (int k = 0; k < count3; k++)
			{
				this.GameActors[k].handle.StartFight();
			}
			if (Singleton<BattleLogic>.GetInstance().m_LevelContext.IsGameTypeBurning())
			{
				int curSelected_BuffId = (int)Singleton<BurnExpeditionController>.GetInstance().model.Get_CurSelected_BuffId();
				BurnExpeditionUT.ApplyBuff(this.DynamicActors, curSelected_BuffId);
				if (Singleton<CBattleSystem>.instance.FightForm != null)
				{
					Singleton<CBattleSystem>.instance.FightForm.GetBattleMisc().Show_BuffCD(0, BurnExpeditionUT.Get_Buff_CDTime(curSelected_BuffId));
				}
			}
			this.StaticActors.Clear();
			this.DynamicActors.Clear();
		}

		public void FightOver()
		{
			int count = this.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				this.GameActors[i].handle.FightOver();
			}
		}

		public void onActorDead(ref GameDeadEventParam prm)
		{
		}

		public void onActorDamage(ref HurtEventResultInfo info)
		{
			if (info.hurtInfo.hurtType != HurtTypeDef.Therapic && info.hurtInfo.hurtCount == 0 && ActorHelper.IsHostCtrlActor(ref info.atker) && info.src.handle.MatHurtEffect != null)
			{
				info.src.handle.MatHurtEffect.PlayHurtEffect();
			}
		}

		public void ClearActor()
		{
			Dictionary<int, List<ActorRoot>>.Enumerator enumerator = this.CachedActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, List<ActorRoot>> current = enumerator.Current;
				List<ActorRoot> value = current.Value;
				for (int i = 0; i < value.Count; i++)
				{
					ActorRoot actorRoot = value[i];
					GameObject gameObject = actorRoot.gameObject;
					actorRoot.ObjLinker.DetachActorRoot();
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject);
				}
				value.Clear();
			}
			this.CachedActors.Clear();
			int count = this.GameActors.Count;
			for (int j = 0; j < count; j++)
			{
				DebugHelper.Assert(this.GameActors[j]);
				if (!this.GameActors[j].handle.ObjLinker.isStatic)
				{
					GameObject gameObject2 = this.GameActors[j].handle.gameObject;
					this.GameActors[j].handle.ObjLinker.DetachActorRoot();
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject2);
				}
				else
				{
					GameObject gameObject3 = this.GameActors[j].handle.gameObject;
					this.GameActors[j].handle.ObjLinker.DetachActorRoot();
					UnityEngine.Object.DestroyObject(gameObject3);
				}
			}
			this.GameActors.Clear();
			for (int k = 0; k < this.StaticActors.Count; k++)
			{
				DebugHelper.Assert(count == 0);
				GameObject gameObject4 = this.StaticActors[k].handle.gameObject;
				this.StaticActors[k].handle.ObjLinker.DetachActorRoot();
				UnityEngine.Object.DestroyObject(gameObject4);
			}
			this.StaticActors.Clear();
			for (int l = 0; l < this.DynamicActors.Count; l++)
			{
				DebugHelper.Assert(count == 0);
				GameObject gameObject5 = this.DynamicActors[l].handle.gameObject;
				this.DynamicActors[l].handle.ObjLinker.DetachActorRoot();
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject5);
			}
			this.DynamicActors.Clear();
			this.HeroActors.Clear();
			this.OrganActors.Clear();
			this.TowerActors.Clear();
			this.MonsterActors.Clear();
			this.SoldierActors.Clear();
			this.FakeTrueEyes.Clear();
			for (int m = 0; m < 3; m++)
			{
				this.CampsActors[m].Clear();
			}
			this.DelayRecycle.Clear();
			this.NewActorID = 0u;
			for (int n = 0; n < 3; n++)
			{
				this.CampsBullet[n].Clear();
			}
		}

		public void AddActor(PoolObjHandle<ActorRoot> actor)
		{
			StringHelper.ClearFormater();
			StringHelper.Formater.Append(actor.handle.ActorControl.GetTypeName());
			StringHelper.Formater.Append(actor.handle.ObjID);
			StringHelper.Formater.Append("(");
			StringHelper.Formater.Append(actor.handle.TheStaticData.TheResInfo.Name);
			StringHelper.Formater.Append(")");
			actor.handle.name = StringHelper.Formater.ToString();
			actor.handle.gameObject.name = actor.handle.name;
			this.GameActors.Add(actor);
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				this.HeroActors.Add(actor);
			}
			else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				this.OrganActors.Add(actor);
				if (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)
				{
					this.TowerActors.Add(actor);
				}
			}
			else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				this.MonsterActors.Add(actor);
				MonsterWrapper monsterWrapper = actor.handle.AsMonster();
				if (monsterWrapper != null && monsterWrapper.cfgInfo != null)
				{
					RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE)monsterWrapper.cfgInfo.bMonsterType;
					if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
					{
						this.SoldierActors.Add(actor);
					}
				}
			}
			else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
			{
				this.FakeTrueEyes.Add(actor);
			}
			else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
			{
				this.CallActors.Add(actor);
			}
			actor.handle.isRecycled = false;
			this.CampsActors[(int)actor.handle.TheActorMeta.ActorCamp].Add(actor);
		}

		public void KillSoldiers()
		{
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.SoldierActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current)
				{
					PoolObjHandle<ActorRoot> current = enumerator.Current;
					current.handle.ValueComponent.actorHp = 0;
				}
			}
		}

		public void HoldDynamicActor(PoolObjHandle<ActorRoot> actor)
		{
			DebugHelper.Assert(!actor.handle.ObjLinker.isStatic);
			this.DynamicActors.Add(actor);
		}

		public void HoldStaticActor(PoolObjHandle<ActorRoot> actor)
		{
			DebugHelper.Assert(actor.handle.ObjLinker.isStatic);
			this.StaticActors.Add(actor);
		}

		public PoolObjHandle<ActorRoot> GetActor(uint ObjID)
		{
			if (ObjID == 0u)
			{
				return default(PoolObjHandle<ActorRoot>);
			}
			for (int i = 0; i < this.GameActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> result = this.GameActors[i];
				if (result.handle.ObjID == ObjID)
				{
					return result;
				}
			}
			return default(PoolObjHandle<ActorRoot>);
		}

		public bool TryGetFromCache(ref PoolObjHandle<ActorRoot> actor, ref ActorMeta actorMeta)
		{
			List<ActorRoot> list = null;
			if (!this.CachedActors.TryGetValue(actorMeta.ConfigId, out list))
			{
				return false;
			}
			if (list.Count == 0)
			{
				return false;
			}
			int index = list.Count - 1;
			actor = new PoolObjHandle<ActorRoot>(list[index]);
			list.RemoveAt(index);
			return true;
		}

		public int QueryEyeLeftTime(PoolObjHandle<ActorRoot> inEye)
		{
			if (!inEye)
			{
				return -1;
			}
			int count = this.DelayRecycle.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.DelayRecycle[i].Key == inEye.handle.ObjID)
				{
					return this.DelayRecycle[i].Value;
				}
			}
			return -1;
		}

		public PoolObjHandle<ActorRoot> SpawnCallActorEx(GameObject rootObj, ref ActorMeta actorMeta, VInt3 pos, VInt3 dir, bool useLobbyModel, bool addComponent)
		{
			if (actorMeta.Difficuty == 0)
			{
				actorMeta.Difficuty = (byte)Singleton<BattleLogic>.instance.GetCurLvelContext().m_levelDifficulty;
			}
			ActorStaticData theStaticData = default(ActorStaticData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			actorDataProvider.GetActorStaticData(ref actorMeta, ref theStaticData);
			ActorServerData actorServerData = default(ActorServerData);
			IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorServerDataProvider actorServerDataProvider = actorDataProvider2 as ActorServerDataProvider;
			actorServerDataProvider.GetCallActorServerData(ref actorMeta, ref actorServerData);
			if (actorMeta.SkinID == 0u)
			{
				actorMeta.SkinID = actorServerData.SkinId;
			}
			GameObject gameObject = null;
			ActorConfig component;
			if (rootObj == null)
			{
				rootObj = MonoSingleton<SceneMgr>.GetInstance().Spawn(typeof(ActorRoot).Name, (SceneObjType)theStaticData.TheActorMeta.ActorType, pos, dir);
				component = rootObj.GetComponent<ActorConfig>();
			}
			else
			{
				component = rootObj.GetComponent<ActorConfig>();
				Animation componentInChildren = rootObj.GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					gameObject = componentInChildren.gameObject;
				}
				rootObj.transform.parent = MonoSingleton<SceneMgr>.GetInstance().GetRoot((SceneObjType)theStaticData.TheActorMeta.ActorType).transform;
			}
			component.ConfigID = actorMeta.ConfigId;
			CActorInfo cActorInfo = null;
			if (!string.IsNullOrEmpty(theStaticData.TheResInfo.ResPath))
			{
				CActorInfo actorInfo = CActorInfo.GetActorInfo(theStaticData.TheResInfo.ResPath, enResourceType.BattleScene);
				if (actorInfo != null)
				{
					cActorInfo = (CActorInfo)UnityEngine.Object.Instantiate(actorInfo);
					string text = (!useLobbyModel) ? cActorInfo.GetArtPrefabName((int)actorMeta.SkinID, -1) : cActorInfo.GetArtPrefabNameLobby((int)actorMeta.SkinID);
					if (gameObject == null && !string.IsNullOrEmpty(text))
					{
						bool flag = false;
						gameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(text, false, SceneObjType.ActionRes, Vector3.zero, Quaternion.identity, out flag);
						if (gameObject != null)
						{
							Transform component2 = gameObject.GetComponent<Transform>();
							component2.SetParent(rootObj.transform);
							component2.localPosition = Vector3.zero;
							component2.localRotation = Quaternion.identity;
							TransformConfig transformConfigIfHaveOne = cActorInfo.GetTransformConfigIfHaveOne(ETransformConfigUsage.CharacterInGame);
							if (transformConfigIfHaveOne != null)
							{
								component2.localPosition += transformConfigIfHaveOne.Offset;
								component2.localScale *= transformConfigIfHaveOne.Scale;
							}
						}
					}
				}
			}
			PoolObjHandle<ActorRoot> result = component.AttachActorRoot(rootObj, ref actorMeta, cActorInfo);
			result.handle.TheStaticData = theStaticData;
			if (addComponent)
			{
				result.handle.Spawned();
			}
			return result;
		}

		public PoolObjHandle<ActorRoot> SpawnActorEx(GameObject rootObj, ref ActorMeta actorMeta, VInt3 pos, VInt3 dir, bool useLobbyModel, bool addComponent)
		{
			if (actorMeta.Difficuty == 0)
			{
				actorMeta.Difficuty = (byte)Singleton<BattleLogic>.instance.GetCurLvelContext().m_levelDifficulty;
			}
			ActorStaticData theStaticData = default(ActorStaticData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			actorDataProvider.GetActorStaticData(ref actorMeta, ref theStaticData);
			ActorServerData actorServerData = default(ActorServerData);
			IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			actorDataProvider2.GetActorServerData(ref actorMeta, ref actorServerData);
			if (actorMeta.SkinID == 0u)
			{
				actorMeta.SkinID = actorServerData.SkinId;
			}
			GameObject gameObject = null;
			ActorConfig component;
			if (rootObj == null)
			{
				rootObj = MonoSingleton<SceneMgr>.GetInstance().Spawn(typeof(ActorRoot).Name, (SceneObjType)theStaticData.TheActorMeta.ActorType, pos, dir);
				component = rootObj.GetComponent<ActorConfig>();
			}
			else
			{
				component = rootObj.GetComponent<ActorConfig>();
				Animation componentInChildren = rootObj.GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					gameObject = componentInChildren.gameObject;
				}
				rootObj.transform.parent = MonoSingleton<SceneMgr>.GetInstance().GetRoot((SceneObjType)theStaticData.TheActorMeta.ActorType).transform;
			}
			component.ConfigID = actorMeta.ConfigId;
			CActorInfo cActorInfo = null;
			if (!string.IsNullOrEmpty(theStaticData.TheResInfo.ResPath))
			{
				CActorInfo actorInfo = CActorInfo.GetActorInfo(theStaticData.TheResInfo.ResPath, enResourceType.BattleScene);
				if (actorInfo != null)
				{
					cActorInfo = (CActorInfo)UnityEngine.Object.Instantiate(actorInfo);
					string text = (!useLobbyModel) ? cActorInfo.GetArtPrefabName((int)actorMeta.SkinID, -1) : cActorInfo.GetArtPrefabNameLobby((int)actorMeta.SkinID);
					if (gameObject == null && !string.IsNullOrEmpty(text))
					{
						gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(text, Vector3.zero, Quaternion.identity, enResourceType.BattleScene);
						if (gameObject != null)
						{
							Transform component2 = gameObject.GetComponent<Transform>();
							component2.SetParent(rootObj.transform);
							component2.localPosition = Vector3.zero;
							component2.localRotation = Quaternion.identity;
							TransformConfig transformConfigIfHaveOne = cActorInfo.GetTransformConfigIfHaveOne(ETransformConfigUsage.CharacterInGame);
							if (transformConfigIfHaveOne != null)
							{
								component2.localPosition += transformConfigIfHaveOne.Offset;
								component2.localScale *= transformConfigIfHaveOne.Scale;
							}
						}
					}
				}
			}
			PoolObjHandle<ActorRoot> result = component.AttachActorRoot(rootObj, ref actorMeta, cActorInfo);
			result.handle.TheStaticData = theStaticData;
			if (addComponent)
			{
				result.handle.Spawned();
			}
			return result;
		}

		public void RecycleActor(PoolObjHandle<ActorRoot> actor, int delay = 0)
		{
			this.DelayRecycle.Add(new KeyValuePair<uint, int>(actor.handle.ObjID, delay));
		}

		public bool AddToCache(PoolObjHandle<ActorRoot> actor)
		{
			ActorRoot handle = actor.handle;
			if (!actor || handle.TheActorMeta.ConfigId == 0 || handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
			{
				return false;
			}
			int configId = handle.TheActorMeta.ConfigId;
			List<ActorRoot> list = null;
			if (!this.CachedActors.TryGetValue(configId, out list))
			{
				list = new List<ActorRoot>();
				this.CachedActors.Add(configId, list);
			}
			handle.DeactiveActor();
			list.Add(handle);
			if (!this.cachedRoot)
			{
				this.cachedRoot = new GameObject();
				this.cachedRoot.name = "CachedActorRoot";
			}
			handle.isRecycled = true;
			handle.myTransform.parent = this.cachedRoot.gameObject.transform;
			return true;
		}

		public void DestroyActor(uint ObjID)
		{
			int i = 0;
			while (i < this.GameActors.Count)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = this.GameActors[i];
				ActorRoot handle = poolObjHandle.handle;
				if (handle.ObjID == ObjID)
				{
					this.eventParamCache.src = poolObjHandle;
					Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, ref this.eventParamCache);
					int num = this.HeroActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.HeroActors.RemoveAt(num);
					}
					num = this.OrganActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.OrganActors.RemoveAt(num);
					}
					num = this.TowerActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.TowerActors.RemoveAt(num);
					}
					num = this.MonsterActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.MonsterActors.RemoveAt(num);
					}
					num = this.SoldierActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.SoldierActors.RemoveAt(num);
					}
					num = this.CallActors.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.CallActors.RemoveAt(num);
					}
					for (int j = 0; j < 3; j++)
					{
						num = this.CampsActors[j].FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
						if (num >= 0)
						{
							this.CampsActors[j].RemoveAt(num);
						}
					}
					num = this.FakeTrueEyes.FindIndex((PoolObjHandle<ActorRoot> item) => item.handle.ObjID == ObjID);
					if (num >= 0)
					{
						this.FakeTrueEyes.RemoveAt(num);
					}
					if (!this.AddToCache(poolObjHandle))
					{
						GameObject gameObject = handle.gameObject;
						if (handle.ObjLinker != null)
						{
							handle.ObjLinker.DetachActorRoot();
						}
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject);
					}
					this.GameActors.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public List<PoolObjHandle<ActorRoot>> GetCampActors(COM_PLAYERCAMP cmp)
		{
			return this.CampsActors[(int)cmp];
		}

		public PoolObjHandle<ActorRoot> GetActor(ActorFilterDelegate predicate)
		{
			for (int i = 0; i < this.GameActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> result = this.GameActors[i];
				if (predicate == null || predicate(ref result))
				{
					return result;
				}
			}
			return new PoolObjHandle<ActorRoot>(null);
		}

		public int GetHeroMaxLevel()
		{
			int num = 1;
			for (int i = 0; i < this.HeroActors.Count; i++)
			{
				if (this.HeroActors[i] && this.HeroActors[i].handle.ValueComponent != null && num < this.HeroActors[i].handle.ValueComponent.actorSoulLevel)
				{
					num = this.HeroActors[i].handle.ValueComponent.actorSoulLevel;
				}
			}
			return num;
		}

		public void AddBullet(ref PoolObjHandle<ActorRoot> actor)
		{
			this.CampsBullet[(int)actor.handle.TheActorMeta.ActorCamp].Add(actor);
		}

		public void RmvBullet(ref PoolObjHandle<ActorRoot> actor)
		{
			this.CampsBullet[(int)actor.handle.TheActorMeta.ActorCamp].Remove(actor);
		}

		public List<PoolObjHandle<ActorRoot>> GetCampBullet(COM_PLAYERCAMP camp)
		{
			return this.CampsBullet[(int)camp];
		}
	}
}

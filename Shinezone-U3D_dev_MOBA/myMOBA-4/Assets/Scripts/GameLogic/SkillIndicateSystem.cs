using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	internal class SkillIndicateSystem : Singleton<SkillIndicateSystem>
	{
		public const string CommonAttackIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/select_02";

		public const string TargetIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01";

		public const string DeadIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01";

		public const string LockTargetPrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01";

		public const string LockHeroPrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01";

		private GameObject indicatePrefab;

		private GameObject commonAttackPrefab;

		private GameObject lockTargetPrefab;

		private GameObject lockHeroPrefab;

		private Transform indicateTransform;

		private Transform commonAttackTransform;

		private Transform lockTargetTransform;

		private Transform lockHeroTransform;

		private PoolObjHandle<ActorRoot> targetObj;

		public override void Init()
		{
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
		}

		public override void UnInit()
		{
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/select_02");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01");
		}

		public void SetPrefabScaler(GameObject _obj, int _distance)
		{
			float num = (float)(_distance + 400) / 800f;
			_obj.transform.localScale = new Vector3(num, num, num);
		}

		private void OnFightPrepare(ref DefaultGameEventParam prm)
		{
			GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
			if (gameObject != null)
			{
				this.indicatePrefab = (GameObject)UnityEngine.Object.Instantiate(gameObject);
				this.HidePrefab(this.indicatePrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
				this.indicateTransform = this.indicatePrefab.transform;
			}
			gameObject = (Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/select_02", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject);
			if (gameObject != null)
			{
				this.commonAttackPrefab = (GameObject)UnityEngine.Object.Instantiate(gameObject);
				this.HidePrefab(this.commonAttackPrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.commonAttackPrefab, SceneObjType.ActionRes);
				this.commonAttackTransform = this.commonAttackPrefab.transform;
			}
			Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", typeof(GameObject), enResourceType.BattleScene, true, false);
			gameObject = (Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject);
			if (gameObject != null)
			{
				this.lockTargetPrefab = (GameObject)UnityEngine.Object.Instantiate(gameObject);
				this.HidePrefab(this.lockTargetPrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockTargetPrefab, SceneObjType.ActionRes);
				this.lockTargetTransform = this.lockTargetPrefab.transform;
			}
			gameObject = (Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject);
			if (gameObject != null)
			{
				this.lockHeroPrefab = (GameObject)UnityEngine.Object.Instantiate(gameObject);
				this.HidePrefab(this.lockHeroPrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
				this.lockHeroTransform = this.lockHeroPrefab.transform;
			}
		}

		private void OnFightOver(ref DefaultGameEventParam prm)
		{
			if (this.indicatePrefab != null)
			{
				UnityEngine.Object.Destroy(this.indicatePrefab);
				this.indicateTransform = null;
			}
			if (this.commonAttackPrefab != null)
			{
				UnityEngine.Object.Destroy(this.commonAttackPrefab);
				this.commonAttackTransform = null;
			}
			if (this.lockTargetPrefab != null)
			{
				UnityEngine.Object.Destroy(this.lockTargetPrefab);
				this.lockTargetTransform = null;
			}
			if (this.lockHeroPrefab != null)
			{
				UnityEngine.Object.Destroy(this.lockHeroPrefab);
				this.lockHeroTransform = null;
			}
		}

		public void PlayCommonAttackTargetEffect(ActorRoot _target)
		{
			if (_target != null && this.commonAttackPrefab != null && this.commonAttackTransform != null && _target.myTransform != null)
			{
				this.ShowPrefab(this.commonAttackPrefab);
				this.commonAttackTransform.position = _target.myTransform.position;
				this.commonAttackTransform.SetParent(_target.myTransform);
			}
		}

		public void StopCommonAttackTargetEffect()
		{
			if (this.commonAttackPrefab != null)
			{
				this.HidePrefab(this.commonAttackPrefab);
			}
			MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.commonAttackPrefab, SceneObjType.ActionRes);
		}

		private void ShowPrefab(GameObject _prefab)
		{
			if (_prefab != null)
			{
				_prefab.SetLayer("Actor", "Particles", false);
			}
		}

		private void HidePrefab(GameObject _prefab)
		{
			if (_prefab != null)
			{
				_prefab.SetLayer("Hide", false);
			}
		}

		private void OnSelectTarget(ref SelectTargetEventParam _param)
		{
			Vector3 position = Vector3.zero;
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.commonAttackTargetID);
			if (actor && this.indicatePrefab != null && this.indicateTransform != null && !ActorHelper.IsHostActor(ref actor))
			{
				this.targetObj = actor;
				this.ShowPrefab(this.indicatePrefab);
				if (this.targetObj.handle.CharInfo != null)
				{
					this.SetPrefabScaler(this.indicatePrefab, this.targetObj.handle.CharInfo.iCollisionSize.x);
				}
				else
				{
					this.SetPrefabScaler(this.indicatePrefab, 400);
				}
				position = actor.handle.myTransform.position;
				position.y += 0.2f;
				this.indicateTransform.position = position;
				this.indicateTransform.SetParent(actor.handle.myTransform);
				this.indicateTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				if (this.lockHeroPrefab != null && this.lockHeroTransform != null)
				{
					if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						float num = 0f;
						if (actor.handle.CharInfo != null)
						{
							num = (float)actor.handle.CharInfo.iBulletHeight / 1000f;
						}
						position.y += num;
						this.lockHeroTransform.position = position;
						this.lockHeroTransform.SetParent(actor.handle.myTransform);
						this.ShowPrefab(this.lockHeroPrefab);
					}
					else
					{
						this.HidePrefab(this.lockHeroPrefab);
						MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
					}
				}
			}
		}

		private void OnClearTarget(ref SelectTargetEventParam _param)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.commonAttackTargetID);
			if (actor && this.indicatePrefab != null)
			{
				this.targetObj = new PoolObjHandle<ActorRoot>(null);
				this.HidePrefab(this.indicatePrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
				if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.lockHeroPrefab != null)
				{
					this.HidePrefab(this.lockHeroPrefab);
					MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
				}
			}
		}

		private void OnLockTarget(ref LockTargetEventParam _param)
		{
			Vector3 position = Vector3.zero;
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.lockTargetID);
			if (actor && this.lockTargetPrefab != null && !ActorHelper.IsHostActor(ref actor))
			{
				this.targetObj = actor;
				this.ShowPrefab(this.lockTargetPrefab);
				if (actor.handle.CharInfo != null)
				{
					this.SetPrefabScaler(this.lockTargetPrefab, actor.handle.CharInfo.iCollisionSize.x);
				}
				else
				{
					this.SetPrefabScaler(this.lockTargetPrefab, 400);
				}
				position = actor.handle.myTransform.position;
				position.y += 0.2f;
				this.lockTargetTransform.position = position;
				this.lockTargetTransform.SetParent(actor.handle.myTransform);
				this.lockTargetTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				if (this.lockHeroPrefab != null)
				{
					if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						float num = 0f;
						if (actor.handle.CharInfo != null)
						{
							num = (float)actor.handle.CharInfo.iBulletHeight / 1000f;
						}
						this.ShowPrefab(this.lockHeroPrefab);
						position.y += num;
						this.lockHeroTransform.position = position;
						this.lockHeroTransform.SetParent(actor.handle.myTransform);
					}
					else
					{
						this.HidePrefab(this.lockHeroPrefab);
						MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
					}
				}
			}
		}

		private void OnClearLockTarget(ref LockTargetEventParam _param)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.lockTargetID);
			if (actor && this.lockTargetPrefab != null)
			{
				this.targetObj = new PoolObjHandle<ActorRoot>(null);
				this.HidePrefab(this.lockTargetPrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockTargetPrefab, SceneObjType.ActionRes);
				if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.lockHeroPrefab != null)
				{
					this.HidePrefab(this.lockHeroPrefab);
					MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
				}
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (this.targetObj && prm.src.handle.ObjID == this.targetObj.handle.ObjID && this.indicatePrefab != null)
			{
				this.HidePrefab(this.indicatePrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
			}
			if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", true, SceneObjType.ActionRes, prm.src.handle.myTransform.position);
				if (pooledGameObjLOD != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObjLOD, 5000, null, null, null);
				}
			}
		}
	}
}

using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class EffectPlayComponent : LogicComponent
	{
		private const string s_dyingGoldEffectPath = "Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01";

		private const string s_skillGesture = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move";

		private const string s_skillGestureCancel = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red";

		private const string s_skillGesture3 = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move";

		private const string s_kingOfKiller = "Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01";

		private const string s_ImmediateReviveEftPath = "Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01";

		public static string s_heroSoulLevelUpEftPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/shengji_tongongi_01";

		public static string s_heroSuckEftPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/Hunqiu_01";

		public static string s_heroHunHurtPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/Hun_hurt_01";

		private static string s_IntimacyStateGuy = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_JY_01";

		private static string s_IntimacyStateLover = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_LR_01";

		private static string s_IntimacyStateSidekick = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_JY_01";

		private static string s_IntimacyStateBesite = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_LR_01";

		private List<DuraEftPlayParam> soulSuckObjList = new List<DuraEftPlayParam>();

		public static int s_suckSoulMSec = 1000;

		private GameObject m_skillGestureGuide;

		private GameObject m_kingOfKillerEff;

		private bool m_bPlayed;

		private void ClearVariables()
		{
			this.m_skillGestureGuide = null;
			this.m_kingOfKillerEff = null;
			for (int i = 0; i < this.soulSuckObjList.get_Count(); i++)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.soulSuckObjList.get_Item(i).EftObj);
			}
			this.soulSuckObjList.Clear();
		}

		public override void OnUse()
		{
			base.OnUse();
			this.ClearVariables();
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddParticle(EffectPlayComponent.s_heroSoulLevelUpEftPath);
			preloadTab.AddParticle(EffectPlayComponent.s_heroSuckEftPath);
			preloadTab.AddParticle(EffectPlayComponent.s_heroHunHurtPath);
			preloadTab.AddParticle("Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red");
			preloadTab.AddParticle("Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01");
			preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01");
			preloadTab.AddParticle(EffectPlayComponent.s_IntimacyStateGuy);
			preloadTab.AddParticle(EffectPlayComponent.s_IntimacyStateLover);
			preloadTab.AddParticle(EffectPlayComponent.s_IntimacyStateSidekick);
			preloadTab.AddParticle(EffectPlayComponent.s_IntimacyStateBesite);
			string text = "Prefab_Skill_Effects/tongyong_effects/Indicator/";
			preloadTab.AddParticle(text + "blin_01");
			preloadTab.AddParticle(text + "blin_01_a");
			preloadTab.AddParticle(text + "blin_01_b");
			preloadTab.AddParticle(text + "blin_01_c");
		}

		public override void Init()
		{
			base.Init();
			this.actor.ValueComponent.SoulLevelChgEvent += new ValueChangeDelegate(this.OnHeroSoulLevelChange);
			this.actor.ActorControl.eventActorDead += new ActorDeadEventHandler(this.onActorDead);
		}

		public override void Uninit()
		{
			base.Uninit();
			this.actor.ValueComponent.SoulLevelChgEvent -= new ValueChangeDelegate(this.OnHeroSoulLevelChange);
			this.actor.ActorControl.eventActorDead -= new ActorDeadEventHandler(this.onActorDead);
		}

		public override void Fight()
		{
		}

		public override void Deactive()
		{
			this.ClearVariables();
			base.Deactive();
		}

		public override void UpdateLogic(int delta)
		{
			if (this.soulSuckObjList.get_Count() == 0)
			{
				return;
			}
			int i = 0;
			while (i < this.soulSuckObjList.get_Count())
			{
				DuraEftPlayParam duraEftPlayParam = this.soulSuckObjList.get_Item(i);
				if (this.UpdateSuckSoulEftMove(ref duraEftPlayParam, delta))
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(duraEftPlayParam.EftObj);
					this.soulSuckObjList.RemoveAt(i);
					this.PlayHunHurtEft();
				}
				else
				{
					this.soulSuckObjList.set_Item(i, duraEftPlayParam);
					i++;
				}
			}
		}

		private bool UpdateSuckSoulEftMove(ref DuraEftPlayParam eftParam, int delta)
		{
			Vector3 position = this.actor.myTransform.position;
			position.y += (float)this.actor.CharInfo.iBulletHeight * 0.001f;
			if (eftParam.EftObj == null)
			{
				return true;
			}
			Vector3 position2 = eftParam.EftObj.transform.position;
			eftParam.EftObj.transform.position = Vector3.Slerp(position2, position, (float)delta / (float)eftParam.RemainMSec);
			eftParam.RemainMSec -= delta;
			return eftParam.RemainMSec <= 0;
		}

		private void OnHeroSoulLevelChange()
		{
			if (!this.actorPtr || this.actorPtr.handle.ActorControl.IsDeadState || !this.actorPtr.handle.Visible || !this.actorPtr.handle.InCamera || this.actorPtr.handle.ValueComponent.actorSoulLevel <= 1 || this.actorPtr.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return;
			}
			Vector3 position = this.actor.myTransform.position;
			position = new Vector3(position.x, position.y + 0.24f, position.z);
			Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
			bool flag = false;
			string levelUpEftPath = ((HeroWrapper)this.actorPtr.handle.ActorControl).GetLevelUpEftPath(this.actorPtr.handle.ValueComponent.actorSoulLevel);
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(levelUpEftPath))
			{
				gameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(levelUpEftPath, true, SceneObjType.ActionRes, position, rot, out flag);
			}
			if (null == gameObject)
			{
				gameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(EffectPlayComponent.s_heroSoulLevelUpEftPath, true, SceneObjType.ActionRes, position, rot, out flag);
			}
			if (null == gameObject)
			{
				return;
			}
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(gameObject, 5000, null, null, null);
			Transform transform = this.actor.ActorMesh ? this.actor.ActorMesh.transform : this.actor.myTransform;
			string layerName = "Particles";
			if (transform && transform.gameObject.layer == LayerMask.NameToLayer("Hide"))
			{
				layerName = "Hide";
			}
			gameObject.transform.SetParent(transform);
			gameObject.SetLayer(layerName, false);
			Singleton<CSoundManager>.instance.PlayBattleSound("Level_Up", this.actorPtr, base.gameObject);
		}

		public void PlayHunHurtEft()
		{
			if (this.actor.ActorControl.IsDeadState)
			{
				return;
			}
			float y = (float)this.actor.CharInfo.iBulletHeight * 0.001f;
			Vector3 pos = this.actor.myTransform.localToWorldMatrix.MultiplyPoint(new Vector3(0f, y, 0f));
			Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(EffectPlayComponent.s_heroHunHurtPath, true, SceneObjType.ActionRes, pos, rot);
			if (pooledGameObjLOD != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObjLOD, 5000, null, null, null);
			}
		}

		public void PlaySuckSoulEft(PoolObjHandle<ActorRoot> src)
		{
			if (!src)
			{
				return;
			}
			if (!ActorHelper.IsCaptainActor(ref this.actorPtr) || !this.actor.Visible || !this.actor.InCamera)
			{
				return;
			}
			float z = (float)src.handle.CharInfo.iBulletHeight * 0.001f;
			Vector3 pos = src.handle.myTransform.localToWorldMatrix.MultiplyPoint(new Vector3(0f, 0f, z));
			Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(EffectPlayComponent.s_heroSuckEftPath, true, SceneObjType.ActionRes, pos, rot);
			if (pooledGameObjLOD != null)
			{
				DuraEftPlayParam duraEftPlayParam = default(DuraEftPlayParam);
				duraEftPlayParam.EftObj = pooledGameObjLOD;
				duraEftPlayParam.RemainMSec = EffectPlayComponent.s_suckSoulMSec;
				this.soulSuckObjList.Add(duraEftPlayParam);
			}
		}

		public void PlayDyingGoldEffect(PoolObjHandle<ActorRoot> inActor)
		{
			if (!inActor || inActor.handle.CharInfo == null || inActor.handle.gameObject == null)
			{
				return;
			}
			float num = (float)inActor.handle.CharInfo.iBulletHeight * 0.001f;
			Vector3 pos = inActor.handle.myTransform.localToWorldMatrix.MultiplyPoint(new Vector3(0f, num + 1f, 0f));
			Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01", true, SceneObjType.ActionRes, pos, rot);
			if (pooledGameObjLOD != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObjLOD, 5000, null, null, null);
			}
		}

		public void StartSkillGestureEffect()
		{
			this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_2, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move");
		}

		public void StartSkillGestureEffect3()
		{
			this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_3, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move");
		}

		public void StartSkillGestureEffectCancel()
		{
			this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_2, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red");
		}

		private void StartSkillGestureEffectShared(SkillSlotType inSlotType, string guidePath)
		{
			if (!this.actorPtr)
			{
				return;
			}
			SkillSlot skillSlot = this.actor.SkillControl.GetSkillSlot(inSlotType);
			if (skillSlot == null)
			{
				return;
			}
			this.EndSkillGestureEffect();
			Quaternion rot = Quaternion.LookRotation(Vector3.right);
			Vector3 position = this.actor.myTransform.position;
			position.y += 0.3f;
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(guidePath, true, SceneObjType.ActionRes, position, rot);
			if (pooledGameObjLOD != null)
			{
				pooledGameObjLOD.transform.SetParent(this.actor.myTransform);
				pooledGameObjLOD.SetActive(true);
				skillSlot.skillIndicator.SetPrefabScaler(pooledGameObjLOD, skillSlot.SkillObj.cfgData.iGuideDistance);
			}
			this.m_skillGestureGuide = pooledGameObjLOD;
		}

		public void EndSkillGestureEffect()
		{
			if (this.m_skillGestureGuide != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_skillGestureGuide);
				this.m_skillGestureGuide = null;
			}
		}

		public void StartKingOfKillerEffect()
		{
			if (!this.actorPtr)
			{
				return;
			}
			this.EndKingOfKillerEffect();
			Quaternion rot = Quaternion.LookRotation(Vector3.right);
			Vector3 position = this.actor.myTransform.position;
			position.y += 3.9f;
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01", true, SceneObjType.ActionRes, position, rot);
			if (pooledGameObjLOD != null)
			{
				pooledGameObjLOD.transform.SetParent(this.actor.myTransform);
				pooledGameObjLOD.SetActive(true);
			}
			this.m_kingOfKillerEff = pooledGameObjLOD;
		}

		public void EndKingOfKillerEffect()
		{
			if (this.m_kingOfKillerEff != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_kingOfKillerEff);
				this.m_kingOfKillerEff = null;
			}
		}

		public void ApplyIntimacyEffect()
		{
			if (this.m_bPlayed)
			{
				return;
			}
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.actorPtr.handle.TheActorMeta.PlayerId);
			if (player != null && player.IntimacyData != null)
			{
				string empty = string.Empty;
				if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
				{
					empty = EffectPlayComponent.s_IntimacyStateGuy;
				}
				else if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
				{
					empty = EffectPlayComponent.s_IntimacyStateLover;
				}
				else if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK)
				{
					empty = EffectPlayComponent.s_IntimacyStateSidekick;
				}
				else if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE)
				{
					empty = EffectPlayComponent.s_IntimacyStateBesite;
				}
				if (!string.IsNullOrEmpty(empty))
				{
					Vector3 position = this.actor.myTransform.position;
					GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(empty, true, SceneObjType.ActionRes, position);
					if (pooledGameObjLOD != null)
					{
						this.m_bPlayed = true;
						pooledGameObjLOD.transform.SetParent(this.actor.myTransform);
						pooledGameObjLOD.SetActive(true);
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObjLOD, 7000, null, null, null);
						ParticleSystem[] componentsInChildren = pooledGameObjLOD.GetComponentsInChildren<ParticleSystem>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							ParticleSystem particleSystem = componentsInChildren[i];
							if (particleSystem != null)
							{
								particleSystem.Play(true);
							}
						}
					}
				}
			}
		}

		public void onActorDead(ref GameDeadEventParam prm)
		{
			if (!prm.bImmediateRevive || !this.actorPtr || this.actorPtr != prm.src)
			{
				return;
			}
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01", true, SceneObjType.ActionRes, this.actorPtr.handle.myTransform.position);
			if (pooledGameObjLOD != null)
			{
				string layerName = "Particles";
				if (this.actorPtr.handle.gameObject.layer == LayerMask.NameToLayer("Hide"))
				{
					layerName = "Hide";
				}
				pooledGameObjLOD.SetLayer(layerName, false);
				pooledGameObjLOD.transform.SetParent(this.actorPtr.handle.myTransform);
				ParticleSystem component = pooledGameObjLOD.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Play(true);
				}
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObjLOD, 5000, null, null, null);
			}
		}

		public override void LateUpdate(int nDelta)
		{
		}
	}
}

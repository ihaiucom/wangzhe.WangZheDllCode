using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using CSProtocol;
using Pathfinding;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class OrganWrapper : ObjWrapper
	{
		public const string OrganAroundEffectHomePath = "Prefab_Characters/Prefab_Organ/AroundEffect/";

		public const string AttackLinkerPrefab = "AttackLinker";

		[HideInInspector]
		[NonSerialized]
		private NavmeshCut navmeshCut;

		private int _attackOneTargetCounter;

		private int _attackOneTargetCounterLast;

		private PoolObjHandle<ActorRoot> _myLastTarget = default(PoolObjHandle<ActorRoot>);

		private GameObject[] _aroundEffects;

		private LineRenderer _attackLinker;

		private AreaCheck attackAreaCheck;

		private int _aroundEnemyMonsterCount;

		private OrganHitEffect HitEffect;

		private bool m_bFirstAttacked;

		private int nOutCombatHpRecoveryTick;

		private int antiHiddenTimer;

		private List<PoolObjHandle<ActorRoot>> TarEyeList_;

		private long attackDistSqr;

		public ResOrganCfgInfo cfgInfo
		{
			get;
			private set;
		}

		public override int CfgReviveCD
		{
			get
			{
				return 2147483647;
			}
		}

		public bool isTower
		{
			get
			{
				return this.actor != null && (this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4);
			}
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			for (int i = 0; i < 3; i++)
			{
				preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/" + ((OrganAroundEffect)i).ToString());
			}
			preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker");
		}

		public override string GetTypeName()
		{
			return "OrganWrapper";
		}

		public override int TakeDamage(ref HurtDataInfo hurt)
		{
			if (!this.m_bFirstAttacked)
			{
				this.m_bFirstAttacked = true;
				if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_2);
				}
				else if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_1);
				}
			}
			hurt.iReduceDamage = 0;
			if (hurt.atker && hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && hurt.atkSlot == SkillSlotType.SLOT_SKILL_0)
			{
				if (this.cfgInfo.iBlockHeroAtkDamageMSec == 0 || (long)this.cfgInfo.iBlockHeroAtkDamageMSec >= (long)Singleton<FrameSynchr>.instance.LogicFrameTick)
				{
					hurt.iReduceDamage += this.cfgInfo.iBlockHeroAtkDamage;
				}
				if (this._aroundEnemyMonsterCount == 0 && (this.cfgInfo.iNoEnemyBlockHeroAtkDamageMSec == 0 || (long)this.cfgInfo.iNoEnemyBlockHeroAtkDamageMSec >= (long)Singleton<FrameSynchr>.instance.LogicFrameTick))
				{
					hurt.iReduceDamage += this.cfgInfo.iNoEnemyBlockHeroAtkDamage;
				}
			}
			return base.TakeDamage(ref hurt);
		}

		public override void OnUse()
		{
			base.OnUse();
			this.navmeshCut = null;
			this._attackOneTargetCounter = 0;
			this._attackOneTargetCounterLast = 0;
			this._myLastTarget = default(PoolObjHandle<ActorRoot>);
			this.nOutCombatHpRecoveryTick = 0;
			this._aroundEffects = null;
			this._attackLinker = null;
			this.attackAreaCheck = null;
			this._aroundEnemyMonsterCount = 0;
			this.m_bFirstAttacked = false;
			this.HitEffect = default(OrganHitEffect);
			this.cfgInfo = null;
			this.antiHiddenTimer = 0;
			this.TarEyeList_ = null;
			this.attackDistSqr = 0L;
		}

		public override void Init()
		{
			base.Init();
			SkillSlot skillSlot = this.actor.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0);
			if (skillSlot != null)
			{
				this.attackDistSqr = skillSlot.SkillObj.cfgData.iMaxAttackDistance;
				this.attackDistSqr *= this.attackDistSqr;
			}
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this.actor.isMovable = false;
			this.actor.isRotatable = false;
			VCollisionShape.InitActorCollision(this.actor);
			this.navmeshCut = base.gameObject.GetComponent<NavmeshCut>();
			if (this.navmeshCut)
			{
				this.navmeshCut.enabled = true;
			}
			this._aroundEffects = new GameObject[3];
			if (this.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && (this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 2 || this.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4))
			{
				this.attackAreaCheck = new AreaCheck(new ActorFilterDelegate(this.ActorMarkFilter), new AreaCheck.ActorProcess(this.ActorMarkProcess), Singleton<GameObjMgr>.GetInstance().GetCampActors(this.actor.GiveMyEnemyCamp()));
			}
			this._aroundEnemyMonsterCount = 0;
			this.cfgInfo = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(this.actor.TheActorMeta.ConfigId);
			if (this.cfgInfo != null)
			{
				this.actorSubType = this.cfgInfo.bOrganType;
			}
		}

		public override void Revive(bool auto)
		{
			base.Revive(auto);
			base.SetObjBehaviMode(ObjBehaviMode.Attack_Move);
		}

		public override void Fight()
		{
			base.Fight();
			this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
			FrameCommand<AttackPositionCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
			frameCommand.cmdId = 1u;
			frameCommand.cmdData.WorldPos = this.actor.location;
			base.CmdAttackMoveToDest(frameCommand, this.actor.location);
			if (this.isTower)
			{
				this.HitEffect.Reset(this);
			}
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE].baseValue = this.actor.TheStaticData.TheOrganOnlyInfo.PhyArmorHurtRate;
			this._aroundEnemyMonsterCount = 0;
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].addValue += this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].addValue += this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
			if (this.actor.HorizonMarker != null)
			{
				if (FogOfWar.enable)
				{
					if (this.GetActorSubType() == 1 || this.GetActorSubType() == 4)
					{
						this.actor.HorizonMarker.SightRadius = Horizon.QueryFowTowerSightRadius();
					}
					this.TarEyeList_ = new List<PoolObjHandle<ActorRoot>>();
				}
				else
				{
					this.actor.HorizonMarker.SightRadius = this.actor.TheStaticData.TheOrganOnlyInfo.HorizonRadius;
				}
			}
		}

		protected override void OnDead()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("Set_Theme", null);
			if (FogOfWar.enable)
			{
				this.ClearAntiHidden();
			}
			base.OnDead();
		}

		protected override void OnRevive()
		{
			base.OnRevive();
			base.EnableRVO(true);
		}

		public override bool RealUseSkill(SkillSlotType InSlot)
		{
			if (base.RealUseSkill(InSlot))
			{
				this._attackOneTargetCounter++;
				this._attackOneTargetCounterLast = 0;
				return true;
			}
			return false;
		}

		public override void OnMyTargetSwitch()
		{
			this._myLastTarget = base.myTarget;
			this._attackOneTargetCounterLast = this._attackOneTargetCounter;
			this._attackOneTargetCounter = 0;
		}

		public override void UpdateLogic(int nDelta)
		{
			this.actor.ActorAgent.UpdateLogic(nDelta);
			base.UpdateLogic(nDelta);
			if (this.attackAreaCheck != null)
			{
				this.attackAreaCheck.UpdateLogic(this.actor.ObjID);
			}
			if (base.IsInBattle)
			{
				this.nOutCombatHpRecoveryTick = 0;
			}
			else
			{
				this.nOutCombatHpRecoveryTick += nDelta;
				if (this.nOutCombatHpRecoveryTick >= 1000)
				{
					int nAddHp = this.cfgInfo.iOutBattleHPAdd * this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue / 10000;
					base.ReviveHp(nAddHp);
					this.nOutCombatHpRecoveryTick -= 1000;
				}
			}
			if (FogOfWar.enable)
			{
				this.UpdateAntiHiddenEyeHurt(nDelta);
			}
		}

		private static PoolObjHandle<ActorRoot> FindEyeTarget(List<PoolObjHandle<ActorRoot>> _srcList)
		{
			if (_srcList.get_Count() == 0)
			{
				return default(PoolObjHandle<ActorRoot>);
			}
			if (_srcList.get_Count() == 1)
			{
				return _srcList.get_Item(0);
			}
			_srcList.Sort(delegate(PoolObjHandle<ActorRoot> a, PoolObjHandle<ActorRoot> b)
			{
				EyeWrapper eyeWrapper = (EyeWrapper)a.handle.ActorControl;
				EyeWrapper eyeWrapper2 = (EyeWrapper)b.handle.ActorControl;
				int lifeTime = eyeWrapper.LifeTime;
				int lifeTime2 = eyeWrapper2.LifeTime;
				int targetHpRate = HitTriggerDurationContext.GetTargetHpRate(a);
				int targetHpRate2 = HitTriggerDurationContext.GetTargetHpRate(b);
				if (targetHpRate < targetHpRate2)
				{
					return -1;
				}
				if (targetHpRate > targetHpRate2)
				{
					return 1;
				}
				if (lifeTime > lifeTime2)
				{
					return -1;
				}
				if (lifeTime < lifeTime2)
				{
					return 1;
				}
				return 0;
			});
			return _srcList.get_Item(0);
		}

		private void UpdateAntiHiddenEyeHurt(int inDelta)
		{
			if (base.IsDeadState)
			{
				return;
			}
			if (this.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			if (this.actorSubType != 1 && this.actorSubType != 4 && this.actorSubType != 2)
			{
				return;
			}
			GlobalConfig instance = MonoSingleton<GlobalConfig>.instance;
			int defenseAntiHiddenFrameInterval = instance.DefenseAntiHiddenFrameInterval;
			this.antiHiddenTimer += inDelta;
			if (this.antiHiddenTimer >= instance.DefenseAntiHiddenInterval && (ulong)this.actor.ObjID % (ulong)((long)defenseAntiHiddenFrameInterval) == (ulong)Singleton<FrameSynchr>.instance.CurFrameNum % (ulong)((long)defenseAntiHiddenFrameInterval))
			{
				this.antiHiddenTimer = 0;
				List<PoolObjHandle<ActorRoot>> fakeTrueEyes = Singleton<GameObjMgr>.instance.FakeTrueEyes;
				int count = fakeTrueEyes.get_Count();
				for (int i = 0; i < count; i++)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = fakeTrueEyes.get_Item(i);
					if (poolObjHandle && !this.TarEyeList_.Contains(poolObjHandle))
					{
						ActorRoot handle = poolObjHandle.handle;
						if (handle.IsEnemyCamp(this.actor))
						{
							VInt3 worldLoc = new VInt3(handle.location.x, handle.location.z, 0);
							if (Singleton<GameFowManager>.instance.IsVisible(worldLoc, this.actor.TheActorMeta.ActorCamp) && (handle.location - this.actor.location).sqrMagnitudeLong2D < this.attackDistSqr)
							{
								this.TarEyeList_.Add(poolObjHandle);
							}
						}
					}
				}
				int count2 = this.TarEyeList_.get_Count();
				if (count2 > 0)
				{
					for (int j = count2 - 1; j >= 0; j--)
					{
						if (!fakeTrueEyes.Contains(this.TarEyeList_.get_Item(j)))
						{
							this.TarEyeList_.RemoveAt(j);
						}
					}
				}
				PoolObjHandle<ActorRoot> poolObjHandle2 = OrganWrapper.FindEyeTarget(this.TarEyeList_);
				if (poolObjHandle2)
				{
					BufConsumer bufConsumer = new BufConsumer(instance.DefenseAntiHiddenHurtId, poolObjHandle2, this.actorPtr);
					bufConsumer.Use();
				}
			}
		}

		private void ClearAntiHidden()
		{
			List<PoolObjHandle<ActorRoot>> fakeTrueEyes = Singleton<GameObjMgr>.instance.FakeTrueEyes;
			int count = this.TarEyeList_.get_Count();
			if (count > 0)
			{
				for (int i = count - 1; i >= 0; i--)
				{
					if (!fakeTrueEyes.Contains(this.TarEyeList_.get_Item(i)))
					{
						this.TarEyeList_.RemoveAt(i);
					}
				}
			}
			this.TarEyeList_.Clear();
		}

		public override void LateUpdate(int delta)
		{
			try
			{
				this.DrawAttackLinker();
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "exception in DrawAttackLinker:{0}, \n {1}", new object[]
				{
					ex.get_Message(),
					ex.get_StackTrace()
				});
			}
		}

		public int GetAttackCounter(PoolObjHandle<ActorRoot> inActor)
		{
			int result = (this._attackOneTargetCounter > 0) ? this._attackOneTargetCounter : ((inActor == this._myLastTarget) ? this._attackOneTargetCounterLast : 0);
			this._attackOneTargetCounterLast = 0;
			return result;
		}

		public void ShowAroundEffect(OrganAroundEffect oae, bool showOrHide, bool hideOther, float scale = 1f)
		{
			if (this._aroundEffects == null)
			{
				return;
			}
			GameObject gameObject = this._aroundEffects[(int)oae];
			if (showOrHide && null == gameObject)
			{
				GameObject gameObject2 = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/" + oae.ToString(), typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
				if (gameObject2 != null)
				{
					gameObject = (GameObject)Object.Instantiate(gameObject2);
					DebugHelper.Assert(gameObject != null);
					if (gameObject != null)
					{
						Transform transform = gameObject.transform;
						if (transform != null)
						{
							transform.SetParent(this.actor.myTransform);
							transform.localPosition = Vector3.zero;
							transform.localScale = Vector3.one;
							transform.localRotation = Quaternion.identity;
						}
						ParticleScaler[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleScaler>(true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].particleScale = scale;
							componentsInChildren[i].CheckAndApplyScale();
						}
					}
					this._aroundEffects[(int)oae] = gameObject;
				}
			}
			if (null != gameObject && gameObject.activeSelf != showOrHide)
			{
				gameObject.SetActive(showOrHide);
				if (showOrHide && oae == OrganAroundEffect.HostPlayerInAndHit)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_Prompt_fangyuta_atk", null);
				}
			}
			if (hideOther)
			{
				for (int j = 0; j < this._aroundEffects.Length; j++)
				{
					if (j != (int)oae && null != this._aroundEffects[j] && this._aroundEffects[j].activeSelf)
					{
						this._aroundEffects[j].SetActive(false);
					}
				}
			}
		}

		private void DrawAttackLinker()
		{
			if (base.myTarget && !base.myTarget.handle.ActorControl.IsDeadState && !this.actor.ActorControl.IsDeadState)
			{
				if (null == this._attackLinker)
				{
					GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
					if (gameObject != null)
					{
						this._attackLinker = ((GameObject)Object.Instantiate(gameObject)).GetComponent<LineRenderer>();
						if (this._attackLinker != null)
						{
							this._attackLinker.transform.SetParent(this.actor.myTransform);
							this._attackLinker.SetVertexCount(2);
							this._attackLinker.useWorldSpace = true;
							Vector3 b = new Vector3(0f, (float)this.actor.CharInfo.iAttackLineHeight * 0.001f, 0f);
							this._attackLinker.SetPosition(0, this.actor.myTransform.position + b);
						}
					}
				}
				if (null != this._attackLinker)
				{
					float y = (float)base.myTarget.handle.CharInfo.iBulletHeight * 0.001f;
					this._attackLinker.SetPosition(1, base.myTarget.handle.myTransform.position + new Vector3(0f, y, 0f));
					if (!this._attackLinker.gameObject.activeSelf)
					{
						this._attackLinker.gameObject.SetActive(true);
					}
				}
			}
			else if (null != this._attackLinker && this._attackLinker.gameObject.activeSelf)
			{
				this._attackLinker.gameObject.SetActive(false);
			}
		}

		private void ActorMarkProcess(PoolObjHandle<ActorRoot> _inActor, AreaCheck.ActorAction _action)
		{
			if (!_inActor)
			{
				return;
			}
			DebugHelper.Assert(this.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
			if (this.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			ActorTypeDef actorType = _inActor.handle.TheActorMeta.ActorType;
			if (actorType == ActorTypeDef.Actor_Type_Monster && !(_inActor.handle.ActorControl as MonsterWrapper).isCalledMonster)
			{
				if (_action == AreaCheck.ActorAction.Enter)
				{
					if (++this._aroundEnemyMonsterCount == 1)
					{
						this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].addValue -= this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
						this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].addValue -= this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
					}
				}
				else if (_action == AreaCheck.ActorAction.Leave && --this._aroundEnemyMonsterCount == 0)
				{
					this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].addValue += this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
					this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].addValue += this.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
				}
			}
			else if (actorType == ActorTypeDef.Actor_Type_Hero || actorType == ActorTypeDef.Actor_Type_EYE)
			{
				if (_action == AreaCheck.ActorAction.Enter)
				{
					_inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Organ, 1);
				}
				else if (_action == AreaCheck.ActorAction.Leave)
				{
					_inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Organ, -1);
				}
			}
		}

		private bool ActorMarkFilter(ref PoolObjHandle<ActorRoot> _inActor)
		{
			if (_inActor && !_inActor.handle.ActorControl.IsDeadState && !this.actor.ActorControl.IsDeadState)
			{
				if (FogOfWar.enable)
				{
					VInt3 worldLoc = new VInt3(_inActor.handle.location.x, _inActor.handle.location.z, 0);
					if (Singleton<GameFowManager>.instance.IsVisible(worldLoc, this.actor.TheActorMeta.ActorCamp))
					{
						long sqrMagnitudeLong2D = (this.actor.location - _inActor.handle.location).sqrMagnitudeLong2D;
						if (sqrMagnitudeLong2D <= this.attackDistSqr)
						{
							return true;
						}
					}
				}
				else
				{
					long sqrMagnitudeLong2D2 = (this.actor.location - _inActor.handle.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D2 <= this.attackDistSqr)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected override void OnHpChange()
		{
			if (this.isTower)
			{
				this.HitEffect.OnHpChanged(this);
			}
			base.OnHpChange();
		}

		public override int GetDetectedRadius()
		{
			if (this.cfgInfo != null && this.cfgInfo.iDetectedRadius > 0)
			{
				return this.cfgInfo.iDetectedRadius;
			}
			return base.GetDetectedRadius();
		}
	}
}

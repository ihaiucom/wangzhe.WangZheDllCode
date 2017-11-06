using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class MonsterWrapper : ObjWrapper
	{
		private const int MON_BATTLE_COOL_TICKS = 30;

		private int nOutCombatHpRecoveryTick;

		protected int lifeTime;

		protected int BornTime;

		private bool isBoss;

		protected VInt3 originalPos = VInt3.zero;

		private Vector3 originalMeshScale = Vector3.one;

		protected PoolObjHandle<ActorRoot> HostActor;

		protected SkillSlotType SpawnSkillSlot = SkillSlotType.SLOT_SKILL_VALID;

		protected bool bCopyedHeroInfo;

		private bool drageToHostWhenTooFarAway = true;

		private bool useHostValueProperty;

		private bool suicideWhenHostDead = true;

		protected int DistanceTestCount;

		private static readonly long CheckDistance = 225000000L;

		private readonly long BaronPretectDistance = 16000000L;

		private int endurance;

		private bool isEnduranceDown;

		public PursuitInfo Pursuit;

		private readonly int totalHpRate = 10000;

		private int dukeMustUseSkillHpRate;

		private static Vector3 _scaleCache;

		public ResMonsterCfgInfo cfgInfo
		{
			get;
			protected set;
		}

		public int Endurance
		{
			get
			{
				return this.endurance;
			}
			set
			{
				this.endurance = value;
			}
		}

		public bool IsEnduranceDown
		{
			get
			{
				return this.isEnduranceDown;
			}
			set
			{
				if (value != this.isEnduranceDown)
				{
				}
				this.isEnduranceDown = value;
			}
		}

		public SkillSlotType spawnSkillSlot
		{
			get
			{
				return this.SpawnSkillSlot;
			}
			protected set
			{
				this.SpawnSkillSlot = value;
			}
		}

		public PoolObjHandle<ActorRoot> hostActor
		{
			get
			{
				return this.HostActor;
			}
		}

		public bool isCalledMonster
		{
			get
			{
				return this.HostActor;
			}
		}

		public bool isDisplayHeroInfo
		{
			get
			{
				return this.bCopyedHeroInfo;
			}
			protected set
			{
				this.bCopyedHeroInfo = value;
			}
		}

		public bool UseHostValueProperty
		{
			get
			{
				return this.useHostValueProperty;
			}
			protected set
			{
				this.useHostValueProperty = value;
			}
		}

		public int LifeTime
		{
			get
			{
				return this.lifeTime;
			}
			set
			{
				this.lifeTime = value;
			}
		}

		public override int CfgReviveCD
		{
			get
			{
				return 2147483647;
			}
		}

		public override PoolObjHandle<ActorRoot> GetOrignalActor()
		{
			if (this.isCalledMonster)
			{
				return this.HostActor;
			}
			return base.GetOrignalActor();
		}

		public void SetHostActorInfo(ref PoolObjHandle<ActorRoot> InHostActor, SkillSlotType InSpawnSkillSlot, bool bInCopyedHeroInfo, bool bSuicideWhenHostDead, bool bDrageToHostWhenTooFarAway, bool bUseHostValueProperty)
		{
			this.HostActor = InHostActor;
			this.SpawnSkillSlot = InSpawnSkillSlot;
			this.isDisplayHeroInfo = bInCopyedHeroInfo;
			this.suicideWhenHostDead = bSuicideWhenHostDead;
			this.drageToHostWhenTooFarAway = bDrageToHostWhenTooFarAway;
			this.useHostValueProperty = bUseHostValueProperty;
			if (this.HostActor)
			{
				if (this.suicideWhenHostDead)
				{
					this.HostActor.handle.ActorControl.eventActorDead += new ActorDeadEventHandler(this.OnActorDead);
				}
				HeroWrapper heroWrapper = this.HostActor.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null)
				{
					heroWrapper.CallMonster = this.actorPtr;
				}
			}
		}

		private void OnCheckDistance(int seq)
		{
			if (++this.DistanceTestCount >= 15 && this.actor != null)
			{
				this.DistanceTestCount = 0;
				if ((this.HostActor.handle.location - this.actor.location).sqrMagnitudeLong2D >= MonsterWrapper.CheckDistance)
				{
					this.actor.location = this.HostActor.handle.location;
				}
			}
		}

		private void RemoveCheckDistanceTimer()
		{
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src == this.hostActor)
			{
				this.actor.Suicide();
				this.RemoveCheckDistanceTimer();
			}
		}

		protected override void InitDefaultState()
		{
			DebugHelper.Assert(this.actor != null && this.cfgInfo != null);
			this.BornTime = this.cfgInfo.iBornTime;
			if (this.BornTime > 0)
			{
				base.SetObjBehaviMode(ObjBehaviMode.State_Born);
				this.nextBehavior = ObjBehaviMode.State_Null;
			}
			else
			{
				base.InitDefaultState();
			}
		}

		public override string GetTypeName()
		{
			return "MonsterWrapper";
		}

		public override void Init()
		{
			base.Init();
			this.SpawnSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.isBoss = false;
			this.nOutCombatHpRecoveryTick = 0;
			this.lifeTime = 0;
			this.BornTime = 0;
			this.cfgInfo = null;
			this.originalPos = VInt3.zero;
			this.originalMeshScale = Vector3.one;
			this.HostActor = default(PoolObjHandle<ActorRoot>);
			this.spawnSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
			this.bCopyedHeroInfo = false;
			this.drageToHostWhenTooFarAway = true;
			this.suicideWhenHostDead = true;
			this.useHostValueProperty = false;
			this.DistanceTestCount = 0;
			this.endurance = 0;
			this.isEnduranceDown = false;
			this.Pursuit = null;
			this.dukeMustUseSkillHpRate = 0;
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this.originalPos = this.actor.location;
			if (this.actor.ActorMesh != null)
			{
				this.originalMeshScale = this.actor.ActorMesh.transform.localScale;
			}
			this.actor.isMovable = this.actor.ObjLinker.CanMovable;
			this.actor.MovementComponent = this.actor.CreateLogicComponent<PlayerMovement>(this.actor);
			this.actor.MatHurtEffect = this.actor.CreateActorComponent<MaterialHurtEffect>(this.actor);
			this.actor.ShadowEffect = this.actor.CreateActorComponent<UpdateShadowPlane>(this.actor);
			VCollisionShape.InitActorCollision(this.actor);
			this.cfgInfo = MonsterDataHelper.GetDataCfgInfo(this.actor.TheActorMeta.ConfigId, (int)this.actor.TheActorMeta.Difficuty);
			if (this.cfgInfo != null)
			{
				this.endurance = this.cfgInfo.iPursuitE;
			}
			DebugHelper.Assert(this.cfgInfo != null, "Failed find monster cfg by id {0}", new object[]
			{
				this.actor.TheActorMeta.ConfigId
			});
			if (this.cfgInfo != null && this.cfgInfo.bIsBoss > 0)
			{
				this.isBoss = true;
			}
			else
			{
				this.isBoss = false;
			}
			this.actorSubType = this.cfgInfo.bMonsterType;
			this.actorSubSoliderType = this.cfgInfo.bSoldierType;
			ResAiParamConf dataByKey = GameDataMgr.aiParamConfDataBin.GetDataByKey(1u);
			if (dataByKey != null)
			{
				this.dukeMustUseSkillHpRate = dataByKey.iParam;
			}
		}

		protected void UpdateBornLogic(int delta)
		{
			this.BornTime -= delta;
			if (this.BornTime <= 0)
			{
				base.InitDefaultState();
			}
		}

		public override void UpdateLogic(int delta)
		{
			this.actor.ActorAgent.UpdateLogic(delta);
			base.UpdateLogic(delta);
			if (base.IsBornState)
			{
				this.UpdateBornLogic(delta);
			}
			else
			{
				if (this.isEnduranceDown)
				{
					this.endurance -= delta;
					if (this.endurance < 0)
					{
						this.endurance = 0;
					}
				}
				if (this.lifeTime > 0)
				{
					this.lifeTime -= delta;
					if (this.lifeTime <= 0)
					{
						base.SetObjBehaviMode(ObjBehaviMode.State_Dead);
					}
				}
				if (this.actorSubSoliderType == 8 && this.myBehavior == ObjBehaviMode.State_AutoAI && !this.actor.SkillControl.isUsing)
				{
					List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
					Vector3 vec = this.actor.forward.vec3;
					long num = (long)this.cfgInfo.iPursuitR;
					num *= num;
					bool flag = false;
					int count = heroActors.get_Count();
					for (int i = 0; i < count; i++)
					{
						VInt3 location = heroActors.get_Item(i).handle.location;
						VInt3 vInt = location - this.actor.location;
						Vector3 vec2 = vInt.vec3;
						long sqrMagnitudeLong2D = vInt.sqrMagnitudeLong2D;
						if (sqrMagnitudeLong2D < this.BaronPretectDistance || (sqrMagnitudeLong2D < num && Vector3.Dot(vec, vec2) > 0f))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						base.SetObjBehaviMode(ObjBehaviMode.State_Idle);
					}
				}
				if (this.actor.ActorControl.IsDeadState || this.myBehavior != ObjBehaviMode.State_Idle)
				{
					this.nOutCombatHpRecoveryTick = 0;
				}
				else
				{
					this.nOutCombatHpRecoveryTick += delta;
					if (this.nOutCombatHpRecoveryTick >= 1000)
					{
						int nAddHp = this.cfgInfo.iOutCombatHPAdd * this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue / 10000;
						base.ReviveHp(nAddHp);
						this.nOutCombatHpRecoveryTick -= 1000;
					}
				}
				if (this.isCalledMonster && this.drageToHostWhenTooFarAway)
				{
					this.OnCheckDistance(0);
				}
			}
		}

		public override void Deactive()
		{
			if (this.actor.ActorMesh != null)
			{
				this.actor.ActorMesh.transform.localScale = this.originalMeshScale;
			}
			this.nOutCombatHpRecoveryTick = 0;
			this.HostActor = default(PoolObjHandle<ActorRoot>);
			this.spawnSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
			this.bCopyedHeroInfo = false;
			this.drageToHostWhenTooFarAway = true;
			this.suicideWhenHostDead = true;
			this.useHostValueProperty = false;
			this.DistanceTestCount = 0;
			base.Deactive();
		}

		public override void Fight()
		{
			base.Fight();
			COM_PLAYERCAMP actorCamp = this.actor.TheActorMeta.ActorCamp;
			if (Singleton<BattleLogic>.GetInstance().organControl.HadSoldierAddByOrgan(actorCamp))
			{
				this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].addRatio += Singleton<BattleLogic>.GetInstance().organControl.GetSoldierMaxHpAddByOrgan(actorCamp);
				this.actor.ValueComponent.actorHp = this.actor.ValueComponent.actorHpTotal;
				this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].addRatio += Singleton<BattleLogic>.GetInstance().organControl.GetSoldierPhycAttackAddByOrgan(actorCamp);
				this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].addRatio += Singleton<BattleLogic>.GetInstance().organControl.GetSoldierMagcAttackAddByOrgan(actorCamp);
				this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].addRatio += Singleton<BattleLogic>.GetInstance().organControl.GetSoldierPhycDefendAddByOrgan(actorCamp);
				this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].addRatio += Singleton<BattleLogic>.GetInstance().organControl.GetSoldierMagcDefendAddByOrgan(actorCamp);
				float num = 1f + (float)Singleton<BattleLogic>.GetInstance().organControl.GetSoldierShapeScaleAddByOrgan(actorCamp) / 10000f;
				MonsterWrapper._scaleCache.x = num;
				MonsterWrapper._scaleCache.y = num;
				MonsterWrapper._scaleCache.z = num;
				this.actor.myTransform.localScale = MonsterWrapper._scaleCache;
				this.actor.HudControl.hudHeight = Mathf.RoundToInt((float)this.actor.HudControl.hudHeight * num);
			}
			if (this.cfgInfo.iLeftBattleFrame > 0)
			{
				this.m_battle_cool_ticks = this.cfgInfo.iLeftBattleFrame;
			}
			else
			{
				this.m_battle_cool_ticks = 30;
			}
			if (!Singleton<FrameSynchr>.GetInstance().bActive)
			{
				this.m_battle_cool_ticks *= 2;
			}
			this._inBattleCoolTick = this.m_battle_cool_ticks;
			if (this.cfgInfo.iDropProbability == 101)
			{
				BattleMisc.BossRoot = this.actorPtr;
			}
			if (FogOfWar.enable && this.actor.HorizonMarker != null && this.GetActorSubType() == 1)
			{
				this.actor.HorizonMarker.SightRadius = Horizon.QuerySoldierSightRadius();
			}
		}

		protected override void OnDead()
		{
			if (this.cfgInfo.iBufDropID != 0 && (int)FrameRandom.Random(10000u) < this.cfgInfo.iBufDropRate)
			{
				Singleton<ShenFuSystem>.GetInstance().CreateShenFuOnMonsterDead(this.actorPtr, (uint)this.cfgInfo.iBufDropID);
			}
			if (this.isCalledMonster)
			{
				this.RemoveCheckDistanceTimer();
			}
			if (this.actorSubSoliderType == 8 && this.myLastAtker)
			{
				COM_PLAYERCAMP actorCamp = this.myLastAtker.handle.TheActorMeta.ActorCamp;
				int num = (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? this.cfgInfo.iKillByCamp1ChangeSoldierWave : this.cfgInfo.iKillByCamp2ChangeSoldierWave;
				if (num > 0)
				{
					ListView<SoldierRegion> soldierRegionsByCamp = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldierRegionsByCamp(actorCamp);
					for (int i = 0; i < soldierRegionsByCamp.Count; i++)
					{
						soldierRegionsByCamp[i].SwitchWave(num, true);
					}
				}
			}
			base.OnDead();
			bool flag = true;
			if (flag)
			{
				Singleton<GameObjMgr>.instance.RecycleActor(this.actorPtr, this.cfgInfo.iRecyleTime);
			}
			if (this.isCalledMonster && this.suicideWhenHostDead)
			{
				this.HostActor.handle.ActorControl.eventActorDead -= new ActorDeadEventHandler(this.OnActorDead);
			}
			if (this.actor.HorizonMarker != null)
			{
				this.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, false);
				this.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, false, false);
			}
		}

		protected override void OnRevive()
		{
			base.OnRevive();
			base.EnableRVO(true);
		}

		public override int TakeDamage(ref HurtDataInfo hurt)
		{
			if (base.IsBornState)
			{
				base.SetObjBehaviMode(ObjBehaviMode.State_Idle);
				this.nextBehavior = ObjBehaviMode.State_Null;
				base.PlayAnimation("Idle", 0f, 0, true);
			}
			if (this.actorSubSoliderType == 8 && this.myBehavior == ObjBehaviMode.State_Idle)
			{
				hurt.iReduceDamage = 0;
				hurt.iReduceDamage += this.cfgInfo.iBlockHeroAtkDamage;
			}
			int actorHp = this.actor.ValueComponent.actorHp;
			int result = base.TakeDamage(ref hurt);
			if (this.actorSubSoliderType == 8)
			{
				int num = this.dukeMustUseSkillHpRate;
				int num2 = this.totalHpRate / num;
				for (int i = 1; i <= num2; i++)
				{
					int num3 = this.totalHpRate - num * i;
					if (this.actor.ValueComponent.actorHp * this.totalHpRate / this.actor.ValueComponent.actorHpTotal < num3 && actorHp * this.totalHpRate / this.actor.ValueComponent.actorHpTotal > num3)
					{
						this.m_nextMustUseSkill = SkillSlotType.SLOT_SKILL_2;
						break;
					}
				}
			}
			return result;
		}

		protected override void OnBehaviModeChange(ObjBehaviMode oldState, ObjBehaviMode curState)
		{
			base.OnBehaviModeChange(oldState, curState);
			if (curState == ObjBehaviMode.State_Idle)
			{
				base.EnableRVO(false);
			}
		}

		public override bool DoesApplyExposingRule()
		{
			return this.actorSubType == 2;
		}

		public override bool DoesIgnoreAlreadyLit()
		{
			return false;
		}

		public override void BeAttackHit(PoolObjHandle<ActorRoot> atker, bool bExposeAttacker)
		{
			if (!this.actor.IsSelfCamp(atker.handle))
			{
				if (this.actorSubType == 2)
				{
					long num = (long)this.cfgInfo.iPursuitR;
					if (this.actor.ValueComponent.actorHp * 10000 / this.actor.ValueComponent.actorHpTotal > 500 || (atker.handle.location - this.originalPos).sqrMagnitudeLong2D < num * num)
					{
						if (this.actorSubSoliderType == 8)
						{
							Vector3 vec = this.actor.forward.vec3;
							Vector3 rhs = atker.handle.location.vec3 - this.actor.location.vec3;
							if (Vector3.Dot(vec, rhs) < 0f)
							{
								if ((atker.handle.location - this.actor.location).sqrMagnitudeLong2D < this.BaronPretectDistance)
								{
									base.SetInBattle();
									this.m_isAttacked = true;
								}
								else
								{
									for (int i = 0; i < Singleton<GameObjMgr>.instance.HeroActors.get_Count(); i++)
									{
										Vector3 rhs2 = Singleton<GameObjMgr>.instance.HeroActors.get_Item(i).handle.location.vec3 - this.actor.location.vec3;
										VInt3 vInt = Singleton<GameObjMgr>.instance.HeroActors.get_Item(i).handle.location - this.actor.location;
										if (vInt.sqrMagnitudeLong2D < this.BaronPretectDistance || (vInt.sqrMagnitudeLong2D < num * num && Vector3.Dot(vec, rhs2) > 0f))
										{
											base.SetInBattle();
											this.m_isAttacked = true;
											break;
										}
									}
								}
							}
							else
							{
								base.SetInBattle();
								this.m_isAttacked = true;
							}
							Singleton<SoundCookieSys>.instance.OnBaronAttacked(this, atker.handle);
						}
						else
						{
							base.SetInBattle();
							this.m_isAttacked = true;
						}
					}
				}
				else
				{
					base.SetInBattle();
					this.m_isAttacked = true;
				}
				atker.handle.ActorControl.SetInBattle();
				atker.handle.ActorControl.SetInAttack(this.actorPtr, bExposeAttacker);
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), atker);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, ref defaultGameEventParam);
			}
		}

		public override bool IsBossOrHeroAutoAI()
		{
			return this.isBoss;
		}
	}
}

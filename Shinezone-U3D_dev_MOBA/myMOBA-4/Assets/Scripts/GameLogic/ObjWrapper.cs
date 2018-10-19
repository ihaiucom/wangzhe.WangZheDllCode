using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using Pathfinding.RVO;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class ObjWrapper : LogicComponent
	{
		public const int MOVE_SLOWEST_SPEED = 1200;

		public const int MOVE_STANDARD_SPEED = 4000;

		private const int BATTLE_COOL_TICKS = 90;

		public ObjDeadMode deadMode;

		public ObjBehaviMode myBehavior;

		public ObjBehaviMode nextBehavior;

		[HideInInspector]
		private PoolObjHandle<ActorRoot> _myTarget = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
		public PoolObjHandle<ActorRoot> m_needToHelpOtherToActtackTarget = default(PoolObjHandle<ActorRoot>);

		protected int m_needToHelpOtherCount = -1;

		protected int m_needToHelpOtherWait;

		public PoolObjHandle<ActorRoot> m_needToHelpTarget = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
		protected int m_needSwithTargetCount = -1;

		protected int m_needSwithTargetWait;

		public bool m_isAttackEnemyIgnoreSrchrange;

		public PoolObjHandle<ActorRoot> m_TargetNeedToBeActtacked = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
        // 当前的正在执行的Move指令
		private IFrameCommand _curMoveCommand;

		private int _moveCmdTimeoutFrame;

		public int curMoveSeq;

		[HideInInspector]
		public IFrameCommand curSkillCommand;

		[HideInInspector]
		public SkillUseParam curSkillUseInfo = default(SkillUseParam);

		[HideInInspector]
		public PoolObjHandle<ActorRoot> myLastAtker = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
		public PoolObjHandle<ActorRoot> m_tauntMeActor = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
		public PoolObjHandle<ActorRoot> m_terrorMeActor = default(PoolObjHandle<ActorRoot>);

		public WaypointsHolder m_curWaypointsHolder;

		public Waypoint m_curWaypointTarget;

		public VInt3 m_curWaypointTargetPosition = VInt3.zero;

		public bool m_isCurWaypointEndPoint;

		public bool m_isStartPoint;

		public bool m_isControledByMan = true;

		public bool m_isAutoAI;

		public bool m_offline;

		public bool m_followOther;

		public uint m_leaderID;

		public bool m_isAttackedByEnemyHero;

		public bool m_isAttacked;

        private int[] NoAbility = new int[(int)ObjAbilityType.Max];

		protected byte actorSubType;

		protected byte actorSubSoliderType;

		public int m_battle_cool_ticks;

		public int _inBattleCoolTick;

		public int _inAttackCoolTick;

		private int m_reviveTick;

		public bool bForceNotRevive;

		public SkillSlotType m_nextMustUseSkill = SkillSlotType.SLOT_SKILL_VALID;

		protected ReviveContext m_reviveContext = new ReviveContext
		{
			ReviveLife = 10000,
			ReviveEnergy = 10000,
			bBaseRevive = true
		};

		public bool bSuicide;

		public List<KeyValuePair<uint, ulong>> hurtSelfActorList = new List<KeyValuePair<uint, ulong>>();

		public List<KeyValuePair<uint, ulong>> helpSelfActorList = new List<KeyValuePair<uint, ulong>>();

		public OutOfControl m_outOfControl = new OutOfControl(false, OutOfControlType.Null);

		private ObjBehaviMode m_beforeOutOfControlBehaviMode;

		public GeoPolygon m_rangePolygon;

		public GameObject m_deadPointGo;

		private ulong lastKillLogicTime;

		private PoolObjHandle<ActorRoot> lastHeroAtker = default(PoolObjHandle<ActorRoot>);

		private ulong lastHeroAtkLogicTime;

		public ulong lastExtraHurtByLowHpBuffTime;

		private static PoolObjHandle<ActorRoot> NullTarget = default(PoolObjHandle<ActorRoot>);

		public event ActorDeadEventHandler eventActorDead;

		public event ActorEventHandler eventActorRevive;

		public event ActorEventHandler eventActorAssist;

		public event ActorEventHandler eventActorEnterCombat;

		public event ActorEventHandler eventActorExitCombat;

		public PoolObjHandle<ActorRoot> LastHeroAtker
		{
			get
			{
				return this.lastHeroAtker;
			}
		}

		public IFrameCommand curMoveCommand
		{
			get
			{
				return this._curMoveCommand;
			}
			private set
			{
				this._curMoveCommand = value;
				this._moveCmdTimeoutFrame = 0;
			}
		}

		public bool IsInBattle
		{
			get
			{
				return this._inBattleCoolTick < this.m_battle_cool_ticks;
			}
		}

		public bool IsInAttack
		{
			get
			{
				return this._inAttackCoolTick < this.m_battle_cool_ticks;
			}
		}

		public int ReviveCooldown
		{
			get
			{
				return this.m_reviveTick;
			}
			set
			{
				this.m_reviveTick = ((value < 0) ? 0 : value);
			}
		}

		public bool IsNeedToHelpOther
		{
			get
			{
				return this.m_needToHelpOtherCount >= this.m_needToHelpOtherWait;
			}
		}

		public virtual int CfgReviveCD
		{
			get
			{
				return 15000;
			}
		}

		public bool CanRevive
		{
			get
			{
				return this.IsDeadState && this.ReviveCooldown <= 0;
			}
		}

		public int ChaseRange
		{
			get
			{
				Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
				if (nextSkill == null)
				{
					return 100;
				}
				if (nextSkill.cfgData.iMaxChaseDistance > 0)
				{
					return nextSkill.cfgData.iMaxChaseDistance;
				}
				return nextSkill.GetMaxSearchDistance(0);
			}
		}

		public int SearchRange
		{
			get
			{
				Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
				if (nextSkill != null)
				{
					return nextSkill.GetMaxSearchDistance(0);
				}
				return 100;
			}
		}

		public int AttackRange
		{
			get
			{
				Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
				if (nextSkill != null)
				{
					return nextSkill.cfgData.iMaxAttackDistance;
				}
				return 100;
			}
		}

		public int GreaterRange
		{
			get
			{
				Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
				if (nextSkill != null)
				{
					return nextSkill.cfgData.iGreaterAttackDistance;
				}
				return 100;
			}
		}

		public bool CanMove
		{
			get
			{
                return this.actor.isMovable && this.NoAbility[(int)ObjAbilityType.ObjAbility_Move] == 0;
			}
		}

		public bool CanRotate
		{
			get
			{
                return this.actor.isRotatable && this.NoAbility[(int)ObjAbilityType.ObjAbility_MoveRotate] == 0;
			}
		}

		public bool IsNeedSwitchTarget
		{
			get
			{
				return this.m_needSwithTargetCount >= this.m_needSwithTargetWait;
			}
		}

		public bool IsDeadState
		{
			get
			{
				return this.myBehavior == ObjBehaviMode.State_Dead;
			}
		}

		public bool IsBornState
		{
			get
			{
				return this.myBehavior == ObjBehaviMode.State_Born;
			}
		}

		public bool IsMoveState
		{
			get
			{
				return this.myBehavior == ObjBehaviMode.Direction_Move;
			}
		}

		public PoolObjHandle<ActorRoot> myTarget
		{
			get
			{
				return this._myTarget;
			}
			set
			{
				if (value != this._myTarget)
				{
					this.OnMyTargetSwitch();
					this._myTarget = value;
				}
			}
		}

		public bool IsEnableReviveContext()
		{
			return this.m_reviveContext.bEnable;
		}

		private void ClearVariables()
		{
			this.myBehavior = ObjBehaviMode.State_Idle;
			this.nextBehavior = ObjBehaviMode.State_Idle;
			this._myTarget.Release();
			this.m_needToHelpOtherToActtackTarget.Release();
			this.m_TargetNeedToBeActtacked.Release();
			this.m_needToHelpTarget.Release();
			this.ClearNeedToHelpOther();
			this.ClearNeedSwitchTarget();
			this.m_isAttackEnemyIgnoreSrchrange = false;
			this.curMoveCommand = null;
			this._moveCmdTimeoutFrame = 0;
			this.curMoveSeq = 0;
			this.curSkillUseInfo.Reset();
			this.curSkillCommand = null;
			this.myLastAtker.Release();
			this.m_tauntMeActor.Release();
			this.m_terrorMeActor.Release();
			this.m_curWaypointsHolder = null;
			this.m_curWaypointTarget = null;
			this.m_curWaypointTargetPosition = VInt3.zero;
			this.m_isCurWaypointEndPoint = false;
			this.m_isStartPoint = false;
			this.m_isControledByMan = true;
			this.m_isAutoAI = false;
			this.m_offline = false;
			this.m_followOther = false;
			this.m_leaderID = 0u;
			this.m_isAttackedByEnemyHero = false;
			this.m_isAttacked = false;
			Array.Clear(this.NoAbility, 0, this.NoAbility.Length);
			this.m_battle_cool_ticks = 0;
			this._inBattleCoolTick = 0;
			this._inAttackCoolTick = 0;
			this.m_reviveTick = 0;
			this.bForceNotRevive = false;
			this.deadMode = ObjDeadMode.DeadState_Normal;
			this.m_reviveContext = new ReviveContext
			{
				ReviveLife = 10000,
				ReviveEnergy = 10000,
				bBaseRevive = true
			};
			this.hurtSelfActorList.Clear();
			this.helpSelfActorList.Clear();
			this.m_outOfControl.ResetOnUse();
			this.m_beforeOutOfControlBehaviMode = ObjBehaviMode.State_Idle;
			this.m_rangePolygon = null;
			this.m_deadPointGo = null;
			this.lastHeroAtker.Release();
			this.lastHeroAtkLogicTime = 0uL;
			this.lastKillLogicTime = 0uL;
			this.lastExtraHurtByLowHpBuffTime = 0uL;
			this.eventActorDead = null;
			this.eventActorRevive = null;
			this.eventActorAssist = null;
			this.eventActorEnterCombat = null;
			this.eventActorExitCombat = null;
			this.bSuicide = false;
			this.m_nextMustUseSkill = SkillSlotType.SLOT_SKILL_VALID;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.actorSubType = 0;
			this.actorSubSoliderType = 0;
			this.ClearVariables();
		}

		public bool IsKilledByHero()
		{
			bool flag = this.lastHeroAtkLogicTime != 0uL && Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.lastHeroAtkLogicTime < 10000uL;
			return flag && this.LastHeroAtker;
		}

		public bool IsInMultiKill()
		{
			return this.lastKillLogicTime != 0uL && Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.lastKillLogicTime < 10000uL;
		}

		public void UpdateLastKillTime()
		{
			this.lastKillLogicTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		public virtual byte GetActorSubType()
		{
			return this.actorSubType;
		}

		public virtual byte GetActorSubSoliderType()
		{
			return this.actorSubSoliderType;
		}

		public virtual PoolObjHandle<ActorRoot> GetOrignalActor()
		{
			return this.actorPtr;
		}

		public virtual string GetTypeName()
		{
			return "ObjWrapper";
		}

		public void SetDeadMode(ObjDeadMode mode)
		{
			this.deadMode = mode;
		}

		public bool IsControlled()
		{
			return this.m_isControledByMan;
		}

		public int GetReviveTotalTime()
		{
			if (this.m_reviveContext.bEnable)
			{
				return (this.m_reviveContext.ReviveTime <= 0) ? this.CfgReviveCD : this.m_reviveContext.ReviveTime;
			}
			return this.CfgReviveCD;
		}

		public void ClearNeedToHelpOther()
		{
			this.m_needToHelpOtherCount = -1;
			this.m_needToHelpOtherWait = 0;
		}

		public void SetReviveContext(int reviveTime, int reviveLife, bool autoReset, bool bBaseRevive = true, bool bCDReset = false, int iReviveEpRate = 10000, int iReviveBuffId = 0, bool bIsPassiveSkill = true, uint uiBuffObjId = 0u)
		{
			this.m_reviveContext.ReviveLife = reviveLife;
			this.m_reviveContext.ReviveTime = reviveTime;
			this.m_reviveContext.AutoReset = autoReset;
			this.m_reviveContext.bBaseRevive = bBaseRevive;
			this.m_reviveContext.bCDReset = bCDReset;
			this.m_reviveContext.bEnable = true;
			this.m_reviveContext.ReviveEnergy = iReviveEpRate;
			this.m_reviveContext.iReviveBuffId = iReviveBuffId;
			this.m_reviveContext.bIsPassiveSkill = bIsPassiveSkill;
			this.m_reviveContext.uiBuffObjId = uiBuffObjId;
			if (this.m_reviveTick > 0 && this.m_reviveContext.ReviveTime >= 0)
			{
				this.m_reviveTick = this.m_reviveContext.ReviveTime;
			}
		}

		public void SetReviveContextEnable(bool bEnable)
		{
			this.m_reviveContext.bEnable = bEnable;
		}

		public void ResetReviveContext()
		{
			this.m_reviveContext.Reset();
		}

		public void SetInBattle()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
			if (curLvelContext != null && !curLvelContext.IsMobaMode() && ownerPlayer != null)
			{
				ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = ownerPlayer.GetAllHeroes();
				int count = allHeroes.Count;
				for (int i = 0; i < count; i++)
				{
					if (allHeroes[i])
					{
						allHeroes[i].handle.ActorControl.SetSelfInBattle();
					}
				}
			}
			else
			{
				this.SetSelfInBattle();
			}
		}

		public void SetSelfInBattle()
		{
			bool isInBattle = this.IsInBattle;
			this._inBattleCoolTick = 0;
			if (!isInBattle)
			{
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), base.GetActor());
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, ref defaultGameEventParam);
				if (this.eventActorEnterCombat != null)
				{
					this.eventActorEnterCombat(ref defaultGameEventParam);
				}
			}
		}

		public void SetSelfExitBattle()
		{
			this._inBattleCoolTick = this.m_battle_cool_ticks + 1;
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), base.GetActor());
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorExitCombat, ref defaultGameEventParam);
			if (this.eventActorExitCombat != null)
			{
				this.eventActorExitCombat(ref defaultGameEventParam);
			}
		}

		public virtual bool DoesApplyExposingRule()
		{
			return false;
		}

		public virtual bool DoesIgnoreAlreadyLit()
		{
			return true;
		}

		public virtual int QueryExposeDuration()
		{
			return Horizon.QueryExposeDurationNormal();
		}

		public virtual bool DoesApplyShowmarkRule()
		{
			return false;
		}

		public void SetInAttack(PoolObjHandle<ActorRoot> inAttackee, bool bExposeAttacker)
		{
			this._inAttackCoolTick = 0;
			if (FogOfWar.enable)
			{
				if (inAttackee)
				{
					this.actor.HorizonMarker.ExposeAndShowAsAttacker(inAttackee.handle.TheActorMeta.ActorCamp, bExposeAttacker);
				}
			}
			else if (bExposeAttacker)
			{
				this.actor.HorizonMarker.SetExposeMark(true, (!inAttackee) ? COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT : inAttackee.handle.TheActorMeta.ActorCamp, false);
			}
		}

		public void SetSelected(bool selected)
		{
			if (this.m_isControledByMan != selected)
			{
				this.m_isControledByMan = selected;
				this.curMoveSeq = 0;
				this.actor.ObjLinker.nPreMoveSeq = -1;
				if (!this.m_isControledByMan && !this.IsDeadState)
				{
					this.TerminateMove();
					this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
				}
			}
		}

		public void SetAutoAI(bool autoAI)
		{
			if (this.m_isAutoAI != autoAI)
			{
				this.m_isAutoAI = autoAI;
				if (this.m_isAutoAI && this.actor.SkillControl.SkillUseCache != null)
				{
					this.actor.SkillControl.SkillUseCache.Clear();
				}
				if (!this.m_isAutoAI && !this.IsDeadState)
				{
					this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
					this.TerminateMove();
				}
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorPtr, this.actorPtr);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref defaultGameEventParam);
			}
			if (!autoAI)
			{
				this.m_offline = false;
				this.m_followOther = false;
				this.m_leaderID = 0u;
			}
		}

		public void SetOffline(bool yesOrNot)
		{
			this.m_offline = yesOrNot;
			if (!this.m_offline)
			{
				this.m_followOther = false;
				this.m_leaderID = 0u;
			}
			if (this.m_isAutoAI != yesOrNot)
			{
				this.m_isAutoAI = yesOrNot;
				if (!this.m_isAutoAI && !this.IsDeadState)
				{
					this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
					this.TerminateMove();
				}
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorPtr, this.actorPtr);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref defaultGameEventParam);
			}
		}

		public void SetFollowOther(bool follow, uint leaderID)
		{
			this.m_followOther = follow;
			this.m_leaderID = leaderID;
			if (follow)
			{
				if (!this.m_isAutoAI)
				{
					this.m_isAutoAI = true;
					DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorPtr, this.actorPtr);
					Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref defaultGameEventParam);
				}
			}
			else
			{
				leaderID = 0u;
			}
		}

		public void SetObjBehaviMode(ObjBehaviMode newMode)
		{
			if (this.myBehavior == ObjBehaviMode.State_GameOver)
			{
				return;
			}
			if (this.myBehavior == ObjBehaviMode.State_Dead && newMode == ObjBehaviMode.State_GameOver)
			{
				return;
			}
			if (this.myBehavior == ObjBehaviMode.State_OutOfControl && this.m_outOfControl.m_isOutOfControl && newMode != ObjBehaviMode.State_Dead && newMode != ObjBehaviMode.State_GameOver)
			{
				return;
			}
			if (this.myBehavior != newMode)
			{
				ObjBehaviMode oldState = this.myBehavior;
				this.myBehavior = newMode;
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.actor.SkillControl.SkillUseCache != null)
				{
					this.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
				}
				this.OnBehaviModeChange(oldState, newMode);
			}
		}

		protected virtual void OnBehaviModeChange(ObjBehaviMode oldState, ObjBehaviMode curState)
		{
			if (this.actor != null)
			{
				if (curState == ObjBehaviMode.State_Dead)
				{
					this.OnDead();
				}
				else if (curState == ObjBehaviMode.State_Idle && oldState == ObjBehaviMode.State_Dead)
				{
					this.OnRevive();
				}
			}
			this.updateAffectActors();
		}

		protected virtual void OnDead()
		{
			PoolObjHandle<ActorRoot> poolObjHandle = (!this.myLastAtker) ? this.myLastAtker : this.myLastAtker.handle.ActorControl.GetOrignalActor();
			PoolObjHandle<ActorRoot> logicAtker = poolObjHandle;
			this.deadMode = ObjDeadMode.DeadState_Normal;
			this.TerminateMove();
			this.ClearMoveCommand();
			this.EnableRVO(false);
			if (this.actor.HudControl != null)
			{
				this.actor.HudControl.OnActorDead();
			}
			if (this.actor.BuffHolderComp != null)
			{
				this.actor.BuffHolderComp.OnDead(poolObjHandle);
			}
			if (this.m_reviveContext.bEnable)
			{
				this.m_reviveTick = ((this.m_reviveContext.ReviveTime <= 0) ? this.CfgReviveCD : this.m_reviveContext.ReviveTime);
			}
			else
			{
				this.m_reviveTick = this.CfgReviveCD;
			}
			SkillComponent skillControl = this.actor.SkillControl;
			skillControl.OnDead();
			if (!this.m_reviveContext.bEnable)
			{
				skillControl.ResetAllSkillSlot(true);
			}
			ObjWrapper actorControl = this.actor.ActorControl;
			if (actorControl.IsKilledByHero())
			{
				logicAtker = actorControl.LastHeroAtker;
			}
			GameDeadEventParam gameDeadEventParam = new GameDeadEventParam(this.actorPtr, this.myLastAtker, poolObjHandle, logicAtker, this.m_reviveContext.bEnable, this.bSuicide, null, this.m_reviveContext.bIsPassiveSkill);
			if (this.eventActorDead != null)
			{
				this.eventActorDead(ref gameDeadEventParam);
			}
			Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_ActorDead, ref gameDeadEventParam);
			Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_PostActorDead, ref gameDeadEventParam);
		}

		protected virtual void OnRevive()
		{
			this.actor.InitVisible();
			if (!this.actor.TheStaticData.TheBaseAttribute.DeadControl)
			{
				this.actor.HorizonMarker.ResetSight();
			}
			this.actor.ValueComponent.SetHpAndEpToInitialValue(this.m_reviveContext.ReviveLife, this.m_reviveContext.ReviveEnergy);
			if (this.m_reviveContext.bCDReset)
			{
				this.actor.SkillControl.ResetSkillCD();
			}
			if (this.actor.HudControl != null)
			{
				this.actor.HudControl.OnActorRevive();
			}
			this.PlayAnimation("Idle", 0f, 0, true);
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), this.myLastAtker);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, ref defaultGameEventParam);
			if (this.eventActorRevive != null)
			{
				this.eventActorRevive(ref defaultGameEventParam);
			}
			if (this.m_reviveContext.iReviveBuffId > 0 && this.actor.SkillControl != null)
			{
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.Init();
				if (this.m_reviveContext.uiBuffObjId > 0u)
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(this.m_reviveContext.uiBuffObjId);
					skillUseParam.SetOriginator(actor);
					skillUseParam.uiFromId = this.m_reviveContext.uiBuffObjId;
				}
				else
				{
					skillUseParam.SetOriginator(this.actorPtr);
					skillUseParam.uiFromId = this.actor.ObjID;
				}
				skillUseParam.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL;
				this.actor.SkillControl.SpawnBuff(this.actorPtr, ref skillUseParam, this.m_reviveContext.iReviveBuffId, false);
			}
			if (this.m_reviveContext.AutoReset)
			{
				this.ResetReviveContext();
			}
			if (this.actor.TheStaticData.TheBaseAttribute.DeadControl && this.GetNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl))
			{
				this.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl);
			}
			this.myLastAtker.Release();
			this.m_isAttackedByEnemyHero = false;
			this.lastHeroAtker.Release();
		}

		protected virtual void OnAssist(ref PoolObjHandle<ActorRoot> deadActor)
		{
			if (this.eventActorAssist != null)
			{
				PoolObjHandle<ActorRoot> orignalActor = this.GetOrignalActor();
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(deadActor, this.actorPtr, ref orignalActor);
				this.eventActorAssist(ref defaultGameEventParam);
			}
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this.ClearNeedToHelpOther();
			this.ClearNeedSwitchTarget();
			this.m_isAttackEnemyIgnoreSrchrange = false;
			this.curMoveCommand = null;
			this._moveCmdTimeoutFrame = 0;
			this.m_curWaypointsHolder = null;
			this.m_curWaypointTarget = null;
			this.m_isCurWaypointEndPoint = false;
			this.m_isStartPoint = false;
			this.m_isControledByMan = true;
			this.m_isAutoAI = false;
			this.m_offline = false;
			this.m_followOther = false;
			this.m_leaderID = 0u;
			this.m_isAttackedByEnemyHero = false;
			this.m_isAttacked = false;
			this.bForceNotRevive = false;
			this.actor.SkillControl = this.actor.CreateLogicComponent<SkillComponent>(this.actor);
			this.actor.ValueComponent = this.actor.CreateLogicComponent<ValueProperty>(this.actor);
			this.actor.HurtControl = this.actor.CreateLogicComponent<HurtComponent>(this.actor);
			this.actor.BuffHolderComp = this.actor.CreateLogicComponent<BuffHolderComponent>(this.actor);
			this.actor.AnimControl = this.actor.CreateLogicComponent<AnimPlayComponent>(this.actor);
			this.actor.HudControl = this.actor.CreateLogicComponent<HudComponent3D>(this.actor);
			this.actor.ActorAgent = this.actor.CreateActorComponent<ObjAgent>(this.actor);
			if (FogOfWar.enable)
			{
				this.actor.HorizonMarker = this.actor.CreateLogicComponent<HorizonMarkerByFow>(this.actor);
			}
			else
			{
				this.actor.HorizonMarker = this.actor.CreateLogicComponent<HorizonMarker>(this.actor);
			}
		}

		public override void Fight()
		{
			if (!Singleton<FrameSynchr>.GetInstance().bActive)
			{
				this.m_battle_cool_ticks = 180;
			}
			else
			{
				this.m_battle_cool_ticks = 90;
			}
			this._inBattleCoolTick = this.m_battle_cool_ticks;
			this._inAttackCoolTick = this.m_battle_cool_ticks;
			this.actor.ValueComponent.SoulLevelChgEvent += new ValueChangeDelegate(this.OnSoulLvlChange);
			this.actor.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHpChange);
			this.actor.ValueComponent.mActorValue.SetChangeEvent(RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new ValueChangeDelegate(this.OnHpChange));
			this.actor.ValueComponent.mActorValue.SetChangeEvent(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, new ValueChangeDelegate(this.OnMoveSpdChange));
			this.InitDefaultState();
		}

		public override void Deactive()
		{
			if (this.actor.ValueComponent != null)
			{
				this.actor.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHpChange);
				this.actor.ValueComponent.SoulLevelChgEvent -= new ValueChangeDelegate(this.OnSoulLvlChange);
			}
			this.ClearVariables();
			if (this.actor.ActorAgent != null)
			{
				this.actor.ActorAgent.StopCurAgeAction();
				this.actor.ActorAgent.btresetcurrrent();
				this.actor.ActorAgent.m_variables.Clear();
			}
			base.Deactive();
		}

		public override void Reactive()
		{
			base.Reactive();
			if (this.actor.ActorAgent != null)
			{
				this.actor.ActorAgent.Reset();
			}
			if (this.actor.ShadowEffect != null)
			{
				this.actor.ShadowEffect.ApplyShadowSettings();
			}
			this.EnableRVO(true);
			this.bSuicide = false;
		}

		protected virtual void InitDefaultState()
		{
			this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
			this.nextBehavior = ObjBehaviMode.State_Null;
		}

		public override void FightOver()
		{
			base.FightOver();
			this.SetObjBehaviMode(ObjBehaviMode.State_GameOver);
			this.TerminateMove();
			this.ClearMoveCommand();
			this.EnableRVO(false);
			this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Skill);
			this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Control);
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.m_needToHelpOtherCount >= 0)
			{
				this.m_needToHelpOtherCount += nDelta;
			}
			if (this.m_needSwithTargetCount >= 0)
			{
				this.m_needSwithTargetCount += nDelta;
			}
			if (this._inBattleCoolTick < this.m_battle_cool_ticks && ++this._inBattleCoolTick == this.m_battle_cool_ticks)
			{
				this.SetSelfExitBattle();
			}
			if (this._inAttackCoolTick < this.m_battle_cool_ticks)
			{
				this._inAttackCoolTick++;
			}
			if (this.myBehavior == ObjBehaviMode.Direction_Move && this.curMoveCommand != null && ++this._moveCmdTimeoutFrame > 60)
			{
				this.CmdStopMove();
				this.curMoveCommand = null;
			}
			if (this.IsDeadState)
			{
				this.m_reviveTick -= nDelta;
				if (this.ReviveCooldown <= 0 && !this.bForceNotRevive)
				{
					this.Revive(true);
				}
			}
			this.updateAffectActors();
		}

		protected void updateAffectActors()
		{
			ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
			int i = 0;
			while (i < this.hurtSelfActorList.Count)
			{
				if (logicFrameTick > this.hurtSelfActorList[i].Value + 10000uL)
				{
					this.hurtSelfActorList.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			i = 0;
			while (i < this.helpSelfActorList.Count)
			{
				if (logicFrameTick > this.helpSelfActorList[i].Value + 10000uL)
				{
					this.helpSelfActorList.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public virtual void Revive(bool auto)
		{
			this.actor.ActorAgent.StopCurAgeAction();
			this.m_outOfControl.m_isOutOfControl = false;
			this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
		}

		public void ClearMoveCommand()
		{
			this.curMoveCommand = null;
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), base.GetActor());
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref defaultGameEventParam);
		}

		public void ClearMoveCommandWithOutNotice()
		{
			this.curMoveCommand = null;
		}

		public void SetTauntTarget(PoolObjHandle<ActorRoot> tar)
		{
			this.m_tauntMeActor = tar;
		}

		public void SetTerrorActor(PoolObjHandle<ActorRoot> tar)
		{
			this.m_terrorMeActor = tar;
		}

		public virtual void SelectTarget(PoolObjHandle<ActorRoot> tar)
		{
			if (this.myTarget && tar)
			{
				if (this.myTarget.handle.ObjID != this.myTarget.handle.ObjID)
				{
				}
			}
			else if (!this.myTarget)
			{
			}
			this.myTarget = tar;
		}

		public void ClearTarget()
		{
			this.myTarget = ObjWrapper.NullTarget;
		}

		public bool GetNoAbilityFlag(ObjAbilityType abt)
		{
			return this.NoAbility[(int)abt] > 0;
		}

		public virtual void AddDisableSkillFlag(SkillSlotType _type, bool bForce = false)
		{
			if (this.actorPtr && this.actorPtr.handle.SkillControl != null)
			{
				this.actorPtr.handle.SkillControl.SetDisableSkillSlot(_type, true);
			}
		}

		public virtual void RmvDisableSkillFlag(SkillSlotType _type, bool bForce = false)
		{
			if (this.actorPtr && this.actorPtr.handle.SkillControl != null)
			{
				this.actorPtr.handle.SkillControl.SetDisableSkillSlot(_type, false);
			}
		}

		public virtual int AddNoAbilityFlag(ObjAbilityType abt)
		{
			this.NoAbility[(int)abt]++;
			return this.NoAbility[(int)abt];
		}

		public virtual int AddNoAbilityFlag(ObjAbilityType abt, int iValue)
		{
			this.NoAbility[(int)abt] += iValue;
			return this.NoAbility[(int)abt];
		}

		public virtual int RmvNoAbilityFlag(ObjAbilityType abt)
		{
			this.NoAbility[(int)abt]--;
			return this.NoAbility[(int)abt];
		}

		public virtual int ClrNoAbilityFlag(ObjAbilityType abt)
		{
			this.NoAbility[(int)abt] = 0;
			return 0;
		}

		private void ImmediateUseSkill(IFrameCommand cmd, ref SkillUseParam param, SkillSlot skillSlot)
		{
			if ((this.IsDeadState && !this.actor.TheStaticData.TheBaseAttribute.DeadControl) || !this.actor.SkillControl.IsEnableSkillSlot(param.SlotType) || this.actor.SkillControl.IsDisableSkillSlot(param.SlotType) || !this.actor.SkillControl.IsSkillUseValid(param.SlotType, ref param))
			{
				return;
			}
			if (this.actor.SkillControl.UseSkill(ref param, true))
			{
			}
		}

		public void CmdUseSkill(IFrameCommand cmd, ref SkillUseParam param)
		{
			SkillSlot skillSlot = null;
			int num = 0;
			if ((this.IsDeadState && !this.actor.TheStaticData.TheBaseAttribute.DeadControl) || this.actor.SkillControl.IsDisableSkillSlot(param.SlotType))
			{
				return;
			}
			if (!this.actor.SkillControl.TryGetSkillSlot(param.SlotType, out skillSlot))
			{
				return;
			}
			Skill skill = (skillSlot.NextSkillObj == null) ? skillSlot.SkillObj : skillSlot.NextSkillObj;
			if (skill != null && skill.AppointType != param.AppointType)
			{
				return;
			}
			if (!this.actor.ValueComponent.IsEnergyType(EnergyType.NoneResource))
			{
				if (!skillSlot.IsEnergyEnough)
				{
					return;
				}
				num = skillSlot.NextSkillEnergyCostTotal();
			}
			if (!skillSlot.IsSkillBeanEnough)
			{
				return;
			}
			if ((param.SlotType == SkillSlotType.SLOT_SKILL_9 || param.SlotType == SkillSlotType.SLOT_SKILL_10) && this.actor.EquipComponent != null && skill != null && !this.actor.EquipComponent.CheckEquipActiveSKillCanUse(param.SlotType, skill.SkillID))
			{
				return;
			}
			if (skill != null && skill.cfgData != null && skill.cfgData.bImmediateUse == 1)
			{
				if (this.actor.SkillControl.CurUseSkillSlot != null && this.actor.SkillControl.CurUseSkillSlot.SkillObj != null && this.actor.SkillControl.CurUseSkillSlot.SkillObj.cfgData != null && this.actor.SkillControl.CurUseSkillSlot.SkillObj.cfgData.bIsInterruptImmediateUseSkill == 1)
				{
					this.actor.SkillControl.AbortCurUseSkill((SkillAbortType)param.SlotType);
				}
				this.ImmediateUseSkill(cmd, ref param, skillSlot);
				this.actor.ValueComponent.actorEp -= num;
				skillSlot.UseSkillBean();
				skillSlot.StartSkillCD();
				return;
			}
			if (this.IsDeadState)
			{
				return;
			}
			if (!this.actor.SkillControl.AbortCurUseSkill((SkillAbortType)param.SlotType))
			{
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.actor.SkillControl.SkillUseCache.SetCacheSkillContext(cmd, param);
				}
				return;
			}
			this.actor.SkillControl.CurUseSkillSlot = skillSlot;
			this.curSkillCommand = cmd;
			this.curSkillUseInfo = param;
			if (param.TargetActor)
			{
				this.SelectTarget(param.TargetActor);
			}
			switch (param.SlotType)
			{
			case SkillSlotType.SLOT_SKILL_0:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_0);
				break;
			case SkillSlotType.SLOT_SKILL_1:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_1);
				break;
			case SkillSlotType.SLOT_SKILL_2:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_2);
				break;
			case SkillSlotType.SLOT_SKILL_3:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_3);
				break;
			case SkillSlotType.SLOT_SKILL_4:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_4);
				break;
			case SkillSlotType.SLOT_SKILL_5:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_5);
				break;
			case SkillSlotType.SLOT_SKILL_6:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_6);
				break;
			case SkillSlotType.SLOT_SKILL_7:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_7);
				break;
			case SkillSlotType.SLOT_SKILL_9:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_9);
				break;
			case SkillSlotType.SLOT_SKILL_10:
				this.SetObjBehaviMode(ObjBehaviMode.UseSkill_10);
				break;
			}
		}

		public void CacheNoramlAttack(IFrameCommand cmd, SkillSlotType InSlot)
		{
			if (this.IsDeadState || this.actor.SkillControl.IsDisableSkillSlot(InSlot))
			{
				return;
			}
			if (!this.actor.SkillControl.AbortCurUseSkill((SkillAbortType)InSlot))
			{
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.actor.SkillControl.SkillUseCache.SetCacheNormalAttackContext(cmd);
				}
			}
			else
			{
				this.ClearTarget();
				this.SetObjBehaviMode(ObjBehaviMode.Normal_Attack);
			}
		}

		public virtual bool RealUseSkill(SkillSlotType InSlot)
		{
			if (!this.actor.SkillControl.AbortCurUseSkill((SkillAbortType)InSlot))
			{
				return false;
			}
			if (this.IsDeadState || !this.actor.SkillControl.IsEnableSkillSlot(InSlot) || this.actor.SkillControl.IsDisableSkillSlot(InSlot) || !this.actor.SkillControl.IsSkillUseValid(InSlot, ref this.curSkillUseInfo))
			{
				return false;
			}
			this.curSkillCommand = null;
			SkillUseParam skillUseParam = this.curSkillUseInfo;
			if (!this.actor.SkillControl.UseSkill(ref skillUseParam, false))
			{
				return false;
			}
			SkillSlot skillSlot = null;
			if (!this.actor.SkillControl.TryGetSkillSlot(skillUseParam.SlotType, out skillSlot))
			{
				return false;
			}
			if (!this.actor.ValueComponent.IsEnergyType(EnergyType.NoneResource))
			{
				skillSlot.AutoSkillEnergyCost();
			}
			skillSlot.UseSkillBean();
			return true;
		}

		public void UseHpRecoverSkillToSelf()
		{
			if (this.IsDeadState)
			{
				return;
			}
			if (!this.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_4) && this.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_4))
			{
				Skill skill = this.GetSkill(SkillSlotType.SLOT_SKILL_4);
				if (skill != null && skill.cfgData != null && skill.cfgData.bSkillType == 3)
				{
					if (!this.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_SKILL_4))
					{
						return;
					}
					SkillUseParam skillUseParam = default(SkillUseParam);
					skillUseParam.Init(SkillSlotType.SLOT_SKILL_4, this.actor.ObjID);
					this.actor.SkillControl.UseSkill(ref skillUseParam, false);
					return;
				}
			}
			if (!this.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_6) && this.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_6))
			{
				Skill skill2 = this.GetSkill(SkillSlotType.SLOT_SKILL_6);
				if (skill2 != null && skill2.cfgData != null && skill2.cfgData.bSkillType == 3)
				{
					if (!this.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_SKILL_6))
					{
						return;
					}
					SkillUseParam skillUseParam2 = default(SkillUseParam);
					skillUseParam2.Init(SkillSlotType.SLOT_SKILL_6, this.actor.ObjID);
					this.actor.SkillControl.UseSkill(ref skillUseParam2, false);
					return;
				}
			}
		}

		public void UseGoHomeSkill()
		{
			if (this.IsDeadState)
			{
				return;
			}
			if (!this.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_6) && this.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_6))
			{
				Skill skill = this.GetSkill(SkillSlotType.SLOT_SKILL_6);
				if (skill != null && skill.cfgData != null && skill.cfgData.bSkillType == 1)
				{
					Skill curUseSkill = this.actor.SkillControl.CurUseSkill;
					if (curUseSkill != null && curUseSkill.SlotType == SkillSlotType.SLOT_SKILL_6)
					{
						return;
					}
					if (!this.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_SKILL_6))
					{
						return;
					}
					SkillUseParam skillUseParam = default(SkillUseParam);
					skillUseParam.Init(SkillSlotType.SLOT_SKILL_6, this.actor.ObjID);
					this.actor.SkillControl.UseSkill(ref skillUseParam, false);
				}
			}
		}

		public void PlayAnimation(string animationName, float blendTime, int layer, bool loop)
		{
			if (this.actor.ActorMesh == null)
			{
				return;
			}
			AnimPlayComponent animControl = this.actor.AnimControl;
			Animation component;
			if (animControl != null)
			{
				animControl.Play(new PlayAnimParam
				{
					animName = animationName,
					blendTime = blendTime,
					loop = loop,
					layer = layer,
					speed = 1f,
					forceOutOfStack = ((this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && (this.myBehavior == ObjBehaviMode.State_AutoAI || this.myBehavior == ObjBehaviMode.State_OutOfControl)) || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				});
			}
			else if ((component = this.actor.ActorMesh.GetComponent<Animation>()) != null)
			{
				if (blendTime > 0f)
				{
					component.CrossFade(animationName, blendTime);
				}
				else
				{
					component.Stop();
					component.Play(animationName);
				}
				AnimationState animationState = component[animationName];
				if (animationState)
				{
					animationState.wrapMode = ((!loop) ? WrapMode.Once : WrapMode.Loop);
				}
			}
		}

		public bool IsPlayingAnimation()
		{
			AnimPlayComponent animControl = this.actor.AnimControl;
			if (animControl != null)
			{
				return this.actor.ActorMeshAnimation.isPlaying;
			}
			Animation component = this.actor.ActorMesh.GetComponent<Animation>();
			return component != null && component.isPlaying;
		}

		public bool SetSkill(SkillSlotType InSlot, bool bSpecial)
		{
			if (this.actor == null)
			{
				return false;
			}
			if (!bSpecial)
			{
				SkillSlot skillSlot = this.actor.SkillControl.GetSkillSlot(InSlot);
				if (skillSlot == null || skillSlot.SkillObj == null || skillSlot.SkillObj.cfgData == null || !this.myTarget)
				{
					return false;
				}
				Skill skill = (skillSlot.NextSkillObj == null) ? skillSlot.SkillObj : skillSlot.NextSkillObj;
				if (skill.cfgData.bRangeAppointType == 2)
				{
					this.curSkillUseInfo = default(SkillUseParam);
					this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.location);
				}
				else if (skill.cfgData.bRangeAppointType == 1)
				{
					this.curSkillUseInfo = default(SkillUseParam);
					this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.ObjID);
				}
				else if (skill.cfgData.bRangeAppointType == 3)
				{
					this.curSkillUseInfo = default(SkillUseParam);
					this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.location - this.actor.location, false, this.myTarget.handle.ObjID);
				}
				else
				{
					this.curSkillUseInfo = default(SkillUseParam);
					this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.ObjID);
				}
			}
			else
			{
				this.curSkillUseInfo = default(SkillUseParam);
				this.curSkillUseInfo.Init(InSlot, this.actor.forward, true, 0u);
			}
			return true;
		}

		public bool CanUseSkill(SkillSlotType slot)
		{
			return !this.actor.SkillControl.IsDisableSkillSlot(slot) && this.actor.SkillControl.CanUseSkill(slot);
		}

		public Skill GetSkill(SkillSlotType slot)
		{
			return this.actor.SkillControl.FindSkill(slot);
		}

		public SkillSlot GetSkillSlot(SkillSlotType slot)
		{
			return this.actor.SkillControl.GetSkillSlot(slot);
		}

		public Skill GetNextSkill(SkillSlotType _slotType)
		{
			SkillSlot skillSlot = null;
			if (this.actor.SkillControl.TryGetSkillSlot(_slotType, out skillSlot))
			{
				Skill skill = (skillSlot.NextSkillObj == null) ? skillSlot.SkillObj : skillSlot.NextSkillObj;
				if (skill != null && skill.cfgData != null)
				{
					return skill;
				}
			}
			return null;
		}

		public ResSkillCfgInfo GetSkillCfgData(SkillSlotType slot)
		{
			Skill skill = this.actor.SkillControl.FindSkill(slot);
			return (skill == null) ? null : skill.cfgData;
		}

		public virtual void CmdMovePosition(IFrameCommand cmd, VInt3 dest)
		{
			if (this.IsDeadState)
			{
				return;
			}
			this.curMoveCommand = cmd;
            if (this.NoAbility[(int)ObjAbilityType.ObjAbility_Move] != 0)
			{
				return;
			}
			if (!this.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_MOVE))
			{
				return;
			}
			this.SetAutoAI(false);
			this.SetObjBehaviMode(ObjBehaviMode.Destination_Move);
		}

        // 执行MoveDirectionCommand
		public virtual void CmdMoveDirection(IFrameCommand cmd, int nDegree)
		{
			bool noAbilityFlag = actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl);
			if (IsDeadState && !noAbilityFlag)
			{
                // 处于“死亡控制？”状态中
				return;
			}

			if (noAbilityFlag)
			{
                // noAbilityFlag: true, IsDeadState: true/false
				actor.ActorControl.SetDeadMode(ObjDeadMode.DeadState_Move);
				curMoveCommand = cmd;
				SetAutoAI(false);
			}
			else
			{
                // noAbilityFlag： false, IsDeadState： false

                // 设置当前需要执行的移动指令
				curMoveCommand = cmd;
                // 中断当前技能
				if (!actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_MOVE))
				{
                    // 英雄单位如果无法中断当前技能，就缓存移动指令
					if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						actor.SkillControl.SkillUseCache.SetCacheMoveCommand(curMoveCommand);
					}
				}
                else if (NoAbility[(int)ObjAbilityType.ObjAbility_Move] == 0)
                {
                    SetAutoAI(false);
                    if (curSkillCommand == null || curSkillCommand.frameNum != curMoveCommand.frameNum)
                    {
                        SetObjBehaviMode(ObjBehaviMode.Direction_Move);
                    }
                }
			}
		}

		public void PreMoveDirection(IFrameCommand cmd, short nDegree, int nSeq)
		{
		}

		public void BTExMoveCmd()
		{
			if (this.curMoveCommand == null)
			{
				return;
			}
            if (this.curMoveCommand.cmdType == (byte)FRAMECMD_ID_DEF.FRAME_CMD_PLAYERMOVE)
			{
				FrameCommand<MoveToPosCommand> frameCommand = (FrameCommand<MoveToPosCommand>)this.curMoveCommand;
				this.RealMovePosition(frameCommand.cmdData.destPosition, frameCommand.cmdId);
			}
			else if (this.curMoveCommand.cmdType == (byte)CSSYNC_TYPE_DEF.CSSYNC_CMD_MOVE)
			{
				FrameCommand<MoveDirectionCommand> frameCommand2 = (FrameCommand<MoveDirectionCommand>)this.curMoveCommand;
				VInt3 direction = VInt3.right.RotateY((int)frameCommand2.cmdData.Degree);
				this.curMoveSeq = (int)frameCommand2.cmdData.nSeq;
				this.RealMoveDirection(direction, frameCommand2.cmdId);
			}
		}

		public void MoveToTarget()
		{
			if (this.myTarget)
			{
				this.RealMovePosition(this.myTarget.handle.location, 0u);
			}
		}

		public VInt3 GetRouteCurWaypointPos()
		{
			Waypoint curWaypointTarget = this.m_curWaypointTarget;
			VInt3 location = this.actor.location;
			Waypoint nextPoint = this.m_curWaypointsHolder.GetNextPoint(this.m_curWaypointTarget);
			if (nextPoint != null)
			{
				VInt3 vInt = new VInt3(this.m_curWaypointTarget.transform.position);
				VInt3 rhs = new VInt3(nextPoint.transform.position);
				VInt3 vInt2 = location - rhs;
				vInt2.y = 0;
				VInt3 vInt3 = vInt - rhs;
				vInt3.y = 0;
				VInt3 vInt4 = location - vInt;
				vInt4.y = 0;
				if (vInt2.sqrMagnitude < vInt3.sqrMagnitude || vInt2.sqrMagnitude < vInt4.sqrMagnitude || vInt4.sqrMagnitude < 1000000.0)
				{
					this.m_curWaypointTarget = nextPoint;
				}
				this.m_isCurWaypointEndPoint = false;
			}
			else
			{
				this.m_isCurWaypointEndPoint = true;
			}
			if (this.m_curWaypointTarget != curWaypointTarget && this.m_curWaypointTarget != null)
			{
				this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
			}
			long sqrMagnitudeLong2D = (base.actorLocation - this.m_curWaypointTargetPosition).sqrMagnitudeLong2D;
			if (sqrMagnitudeLong2D > (long)MonoSingleton<GlobalConfig>.instance.WaypointIgnoreDist)
			{
				return this.m_curWaypointTargetPosition;
			}
			Waypoint nextPoint2 = this.m_curWaypointsHolder.GetNextPoint(this.m_curWaypointTarget);
			if (nextPoint2 != null)
			{
				this.m_curWaypointTarget = nextPoint2;
				this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
			}
			else
			{
				this.m_isCurWaypointEndPoint = true;
			}
			return this.m_curWaypointTargetPosition;
		}

		public VInt3 GetRouteCurWaypointPosPre()
		{
			Waypoint curWaypointTarget = this.m_curWaypointTarget;
			VInt3 location = this.actor.location;
			Waypoint prePoint = this.m_curWaypointsHolder.GetPrePoint(this.m_curWaypointTarget);
			if (prePoint != null)
			{
				VInt3 vInt = new VInt3(this.m_curWaypointTarget.transform.position);
				VInt3 rhs = new VInt3(prePoint.transform.position);
				VInt3 vInt2 = location - rhs;
				vInt2.y = 0;
				VInt3 vInt3 = vInt - rhs;
				vInt3.y = 0;
				VInt3 vInt4 = location - vInt;
				vInt4.y = 0;
				if (vInt2.sqrMagnitude < vInt3.sqrMagnitude || vInt2.sqrMagnitude < vInt4.sqrMagnitude || vInt4.sqrMagnitude < 1000000.0)
				{
					this.m_curWaypointTarget = prePoint;
				}
				this.m_isStartPoint = false;
			}
			else
			{
				this.m_isStartPoint = true;
			}
			if (this.m_curWaypointTarget != curWaypointTarget && this.m_curWaypointTarget != null)
			{
				this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
			}
			long sqrMagnitudeLong2D = (base.actorLocation - this.m_curWaypointTargetPosition).sqrMagnitudeLong2D;
			if (sqrMagnitudeLong2D > (long)MonoSingleton<GlobalConfig>.instance.WaypointIgnoreDist)
			{
				return this.m_curWaypointTargetPosition;
			}
			Waypoint prePoint2 = this.m_curWaypointsHolder.GetPrePoint(this.m_curWaypointTarget);
			if (prePoint2 != null)
			{
				this.m_curWaypointTarget = prePoint2;
				this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
			}
			else
			{
				this.m_isStartPoint = true;
			}
			return this.m_curWaypointTargetPosition;
		}

		public bool IsCurWaypointValid()
		{
			return this.m_curWaypointTarget != null;
		}

		public virtual void RealMovePosition(VInt3 dest, uint id = 0u)
		{
			if (this.actor.MovementComponent != null)
			{
				dest.y = 0;
				this.actor.MovementComponent.SetMoveParam(dest, false, true, id);
                if (this.NoAbility[(int)ObjAbilityType.ObjAbility_Move] == 0)
				{
					this.actor.MovementComponent.ExcuteMove();
				}
			}
		}

		public virtual void RealMoveDirection(VInt3 direction, uint id = 0u)
		{
			if (this.actor.MovementComponent != null)
			{
				this.actor.MovementComponent.SetMoveParam(direction, true, true, id);
                if (this.NoAbility[(int)ObjAbilityType.ObjAbility_Move] == 0)
				{
					if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						this.EnableRVO(false);
					}
					this.actor.MovementComponent.ExcuteMove();
				}
			}
		}

		public void CmdStopMove()
		{
			if (this.IsDeadState)
			{
				if (this.GetNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl))
				{
					this.TerminateMove();
					this.ClearMoveCommand();
					this.SetDeadMode(ObjDeadMode.DeadState_Idle);
				}
			}
			else
			{
				this.TerminateMove();
				this.ClearMoveCommand();
				if (this.myBehavior == ObjBehaviMode.Direction_Move || this.myBehavior == ObjBehaviMode.Destination_Move)
				{
					this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
				}
			}
		}

		public bool IsUseAdvanceCommonAttack()
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
			return ownerPlayer != null && ownerPlayer.IsUseAdvanceCommonAttack();
		}

		public bool CancelCommonAttackMode()
		{
			bool flag = this.IsUseAdvanceCommonAttack();
			if (flag)
			{
				BaseAttackMode currentAttackMode = this.GetCurrentAttackMode();
				if (currentAttackMode != null)
				{
					return currentAttackMode.CancelCommonAttackMode();
				}
			}
			return flag;
		}

		public virtual void CmdCommonAttackMode(IFrameCommand cmd, sbyte Start, uint uiRealObjID)
		{
			if (this.IsDeadState)
			{
				return;
			}
			if ((int)Start == 0)
			{
				this.nextBehavior = ObjBehaviMode.State_Idle;
				if (!this.IsUseAdvanceCommonAttack() && this.actor.SkillControl.SkillUseCache != null)
				{
					this.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
				}
				if (this.actor.SkillControl.SkillUseCache != null)
				{
					this.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(false);
				}
				this.actor.SkillControl.SetCommonAttackIndicator(false);
			}
			else
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.actorPtr.handle.TheActorMeta.PlayerId);
				if (player != null && player.Captain.handle.ActorAgent != null && player.Captain.handle.ActorAgent.m_wrapper != null)
				{
					BaseAttackMode currentAttackMode = player.Captain.handle.ActorAgent.m_wrapper.GetCurrentAttackMode();
					if (currentAttackMode != null)
					{
						currentAttackMode.SetEnemyHeroAttackTargetID(uiRealObjID);
						currentAttackMode.SetCommonButtonType(Start);
					}
				}
				if (!this.actor.SkillControl.isUsing)
				{
					this.ClearTarget();
					this.curSkillCommand = cmd;
					this.SetObjBehaviMode(ObjBehaviMode.Normal_Attack);
				}
				else
				{
					this.ClearTarget();
					this.curSkillCommand = cmd;
					this.CacheNoramlAttack(cmd, SkillSlotType.SLOT_SKILL_0);
				}
				if (this.actor.SkillControl.SkillUseCache != null)
				{
					this.actor.SkillControl.SkillUseCache.SetCommonAttackMode(true);
					this.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(true);
					this.actor.SkillControl.SkillUseCache.SetNewAttackCommand();
				}
				this.actor.SkillControl.SetCommonAttackIndicator(true);
			}
		}

		public virtual void TerminateMove()
		{
			if (this.actor.MovementComponent != null)
			{
				this.actor.MovementComponent.StopMove();
			}
		}

		public void EnableRVO(bool enable)
		{
			if (this.actor.MovementComponent != null && this.actor.MovementComponent.Pathfinding != null)
			{
				MPathfinding pathfinding = this.actor.MovementComponent.Pathfinding;
				pathfinding.EnableRVO(enable);
			}
		}

		public void CmdAttackMoveToDest(IFrameCommand cmd, VInt3 dest)
		{
			if (this.IsDeadState)
			{
				return;
			}
			this.ClearTarget();
			this.ClearMoveCommand();
			this.curMoveCommand = cmd;
			this.SetObjBehaviMode(ObjBehaviMode.Attack_Move);
		}

		public void AttackAlongRoute(WaypointsHolder InRoute)
		{
			if (this.IsDeadState || !InRoute)
			{
				return;
			}
			this.ClearTarget();
			this.ClearMoveCommand();
			this.m_curWaypointsHolder = InRoute;
			this.m_curWaypointTarget = this.m_curWaypointsHolder.startPoint;
			if (this.m_curWaypointTarget != null)
			{
				this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
			}
			this.SetObjBehaviMode(ObjBehaviMode.Attack_Path);
		}

		public void AttackSelectActor(uint ObjID)
		{
			if (this.IsDeadState)
			{
				return;
			}
			this.ClearMoveCommand();
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(ObjID);
			if (actor)
			{
				this.SelectTarget(actor);
				this.SetObjBehaviMode(ObjBehaviMode.Attack_Lock);
			}
		}

		public void ForceAbortCurUseSkill()
		{
			this.actor.SkillControl.ForceAbortCurUseSkill();
		}

		public void DelayAbortCurUseSkill()
		{
			this.actor.SkillControl.DelayAbortCurUseSkill();
		}

		public bool IsTargetObjInSearchDistance()
		{
			Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
			if (nextSkill == null || !this.myTarget)
			{
				return false;
			}
			if (!this.myTarget || this.myTarget.handle.shape == null || this.myTarget.handle.ActorAgent == null || this.myTarget.handle.ActorControl.IsDeadState)
			{
				return false;
			}
			long num = (long)nextSkill.GetMaxSearchDistance(0);
			num += (long)this.myTarget.handle.ActorControl.GetDetectedRadius();
			num *= num;
			return (base.actorLocation - this.myTarget.handle.location).sqrMagnitudeLong2D <= num;
		}

		public virtual void BeAttackHit(PoolObjHandle<ActorRoot> atker, bool bExposeAttacker)
		{
			if (!this.actor.IsSelfCamp(atker.handle))
			{
				this.SetInBattle();
				atker.handle.ActorControl.SetInBattle();
				atker.handle.ActorControl.SetInAttack(this.actorPtr, bExposeAttacker);
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(base.GetActor(), atker);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, ref defaultGameEventParam);
				this.m_isAttacked = true;
			}
		}

		public virtual int TakeDamage(ref HurtDataInfo hurt)
		{
			if (hurt.atker && hurt.target && hurt.atker.handle.TheActorMeta.ActorCamp != hurt.target.handle.TheActorMeta.ActorCamp && hurt.hurtType != HurtTypeDef.Therapic)
			{
				this.myLastAtker = hurt.atker;
				PoolObjHandle<ActorRoot> orignalActor = this.myLastAtker.handle.ActorControl.GetOrignalActor();
				if (orignalActor && orignalActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.m_isAttackedByEnemyHero = true;
					this.lastHeroAtker = orignalActor;
					this.lastHeroAtkLogicTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				}
			}
			if (hurt.hurtType != HurtTypeDef.Therapic)
			{
				this.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_DAMAGE);
			}
			int result = this.actor.HurtControl.TakeDamage(ref hurt);
			if (hurt.atker && hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && hurt.hurtType != HurtTypeDef.Therapic && (this.actor.ValueComponent.actorHp * 100 < this.actor.ValueComponent.actorHpTotal * 95 || (this.myLastAtker && !this.myLastAtker.handle.AttackOrderReady)) && this.actor.ActorAgent != null)
			{
				this.actor.ActorAgent.SetInDanger();
			}
			return result;
		}

		public virtual int TakeBouncesDamage(ref HurtDataInfo hurt)
		{
			if (hurt.atker && hurt.target && hurt.atker.handle.TheActorMeta.ActorCamp != hurt.target.handle.TheActorMeta.ActorCamp && hurt.hurtType != HurtTypeDef.Therapic)
			{
				this.myLastAtker = hurt.atker;
				PoolObjHandle<ActorRoot> orignalActor = this.myLastAtker.handle.ActorControl.GetOrignalActor();
				if (orignalActor && orignalActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.m_isAttackedByEnemyHero = true;
					this.lastHeroAtker = orignalActor;
					this.lastHeroAtkLogicTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				}
			}
			return this.actor.HurtControl.TakeBouncesDamage(ref hurt);
		}

		public void NotifyAssistActor(ref PoolObjHandle<ActorRoot> assist)
		{
			if (!assist)
			{
				return;
			}
			assist.handle.ActorControl.OnAssist(ref this.actorPtr);
			assist.handle.BuffHolderComp.OnAssistEffect(ref this.actorPtr);
		}

		public void NotifySelfCampSelfBeAttacked(int srchR)
		{
			Singleton<TargetSearcher>.GetInstance().NotifySelfCampToAttack(this.actorPtr, srchR, this.myLastAtker);
		}

		public void NotifySelfCampSelfWillAttack(int srchR)
		{
			Singleton<TargetSearcher>.GetInstance().NotifySelfCampToAttack(this.actorPtr, srchR, this.myTarget);
		}

		public virtual void SetHelpToAttackTarget(PoolObjHandle<ActorRoot> helpActor, PoolObjHandle<ActorRoot> enemyActor)
		{
			this.m_needToHelpTarget = helpActor;
			this.m_needToHelpOtherToActtackTarget = enemyActor;
			if (this.m_needToHelpOtherCount < 0)
			{
				this.m_needToHelpOtherCount = 0;
			}
			this.m_needToHelpOtherWait = 0;
		}

		public void HelpToAttack()
		{
			this.myTarget = this.m_needToHelpOtherToActtackTarget;
			this.m_needToHelpOtherToActtackTarget.Release();
			this.ClearNeedToHelpOther();
		}

		public void CommanderNotifyToAttack(uint targetId, int srchR, bool ignoreSrchRange)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(targetId);
			if (actor)
			{
				COM_PLAYERCAMP actorCamp = this.actor.TheActorMeta.ActorCamp;
				Singleton<TargetSearcher>.GetInstance().CommanderNotifyToAttack(actorCamp, srchR, actor, ignoreSrchRange);
			}
		}

		public virtual void SetToAttackTarget(PoolObjHandle<ActorRoot> enemyActor, bool ignoreSrchRange = false)
		{
			this.m_TargetNeedToBeActtacked = enemyActor;
			if (this.m_needSwithTargetCount < 0)
			{
				this.m_needSwithTargetCount = 0;
			}
			this.m_needSwithTargetWait = 0;
			this.m_isAttackEnemyIgnoreSrchrange = ignoreSrchRange;
		}

		public void SwitchTarget()
		{
			if (this.IsNeedSwitchTarget && this.m_TargetNeedToBeActtacked)
			{
				this.myTarget = this.m_TargetNeedToBeActtacked;
			}
			this.ClearNeedSwitchTarget();
		}

		public void ClearNeedSwitchTarget()
		{
			this.m_needSwithTargetCount = -1;
			this.m_needSwithTargetWait = 0;
		}

		protected virtual void OnSoulLvlChange()
		{
		}

		public void OnShieldChange(int pType, int chgValue)
		{
			this.actor.HudControl.UpdateShieldValue((ProtectType)pType, chgValue);
			if (this.actor.FixedHudControl != null)
			{
				this.actor.FixedHudControl.UpdateShieldValue((ProtectType)pType, chgValue);
			}
		}

		protected virtual void OnHpChange()
		{
			if (this.actor.HudControl != null)
			{
				this.actor.HudControl.UpdateBloodBar(Math.Min(this.actor.ValueComponent.actorHp, this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue), this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
			}
			if (this.actor.FixedHudControl != null)
			{
				this.actor.FixedHudControl.UpdateBloodBar(Math.Min(this.actor.ValueComponent.actorHp, this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue), this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
			}
			if (this.actor.ValueComponent.actorHp <= 0 && !this.IsDeadState)
			{
				this.SetObjBehaviMode(ObjBehaviMode.State_Dead);
			}
		}

		public void UpdateAnimPlaySpeed()
		{
			if (this.actor.AnimControl != null && (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))
			{
				int num = this.actor.ValueComponent.actorMoveSpeed;
				num = ((num >= 1200) ? num : 1200);
				float speed = (float)num / 4000f;
				this.actor.AnimControl.SetAnimPlaySpeed("Run", speed);
			}
		}

		public void OnMoveSpdChange()
		{
			if (this.actor.ValueComponent == null || this.actor.ValueComponent.mActorValue == null)
			{
				return;
			}
			int num = this.actor.ValueComponent.actorMoveSpeed;
			num = ((num >= 1200) ? num : 1200);
			this.actor.ObjLinker.GroundSpeed = num;
			if (this.actor.MovementComponent != null)
			{
				this.actor.MovementComponent.maxSpeed = num;
				if (this.actor.MovementComponent.Pathfinding != null && this.actor.MovementComponent.Pathfinding.rvo != null)
				{
					RVOController rvo = this.actor.MovementComponent.Pathfinding.rvo;
					rvo.maxSpeed = num;
				}
			}
			if (this.actor.AnimControl != null)
			{
				float speed = (float)num / 4000f;
				this.actor.AnimControl.SetAnimPlaySpeed("Run", speed);
			}
			if (this.actor.ValueComponent.ObjValueStatistic != null)
			{
				int iMoveSpeedMax = this.actor.ValueComponent.ObjValueStatistic.iMoveSpeedMax;
				this.actor.ValueComponent.ObjValueStatistic.iMoveSpeedMax = ((iMoveSpeedMax <= num) ? num : iMoveSpeedMax);
			}
		}

		public bool CanAttack(ActorRoot target)
		{
			return this.actor.CanAttack(target);
		}

		public void ReviveHp(int nAddHp)
		{
			if (this.IsDeadState)
			{
				return;
			}
			this.actor.ValueComponent.actorHp += nAddHp;
		}

		public void AddHurtSelfActor(PoolObjHandle<ActorRoot> actor)
		{
			if (!actor)
			{
				return;
			}
			PoolObjHandle<ActorRoot> ptr = actor;
			if (ptr.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				if (ptr.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Call)
				{
					return;
				}
				CallActorWrapper callActorWrapper = ptr.handle.ActorControl as CallActorWrapper;
				if (callActorWrapper == null)
				{
					return;
				}
				ptr = callActorWrapper.GetHostActor();
			}
			if (!ptr)
			{
				return;
			}
			ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
			int i;
			for (i = 0; i < this.hurtSelfActorList.Count; i++)
			{
				if (this.hurtSelfActorList[i].Key == ptr.handle.ObjID)
				{
					this.hurtSelfActorList[i] = new KeyValuePair<uint, ulong>(ptr.handle.ObjID, logicFrameTick);
					break;
				}
			}
			if (i >= this.hurtSelfActorList.Count)
			{
				this.hurtSelfActorList.Add(new KeyValuePair<uint, ulong>(ptr.handle.ObjID, logicFrameTick));
			}
		}

		public void AddHelpSelfActor(PoolObjHandle<ActorRoot> actor)
		{
			if (!actor)
			{
				return;
			}
			PoolObjHandle<ActorRoot> ptr = actor;
			if (actor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				if (ptr.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Call)
				{
					return;
				}
				CallActorWrapper callActorWrapper = ptr.handle.ActorControl as CallActorWrapper;
				if (callActorWrapper == null)
				{
					return;
				}
				ptr = callActorWrapper.GetHostActor();
			}
			if (!ptr)
			{
				return;
			}
			ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
			int i;
			for (i = 0; i < this.helpSelfActorList.Count; i++)
			{
				if (this.helpSelfActorList[i].Key == ptr.handle.ObjID)
				{
					this.helpSelfActorList[i] = new KeyValuePair<uint, ulong>(ptr.handle.ObjID, logicFrameTick);
					break;
				}
			}
			if (i >= this.helpSelfActorList.Count)
			{
				this.helpSelfActorList.Add(new KeyValuePair<uint, ulong>(ptr.handle.ObjID, logicFrameTick));
			}
		}

		public void SetOutOfControl(bool isOutOfControl, OutOfControlType type)
		{
			this.m_outOfControl.m_isOutOfControl = isOutOfControl;
			this.m_outOfControl.m_outOfControlType = type;
			if (isOutOfControl)
			{
				if (this.myBehavior != ObjBehaviMode.State_OutOfControl)
				{
					this.m_beforeOutOfControlBehaviMode = this.myBehavior;
				}
				this.SetObjBehaviMode(ObjBehaviMode.State_OutOfControl);
			}
			else if (this.myBehavior != ObjBehaviMode.State_Dead && this.myBehavior != ObjBehaviMode.State_GameOver)
			{
				this.SetObjBehaviMode(this.m_beforeOutOfControlBehaviMode);
			}
		}

		public virtual bool IsBossOrHeroAutoAI()
		{
			return false;
		}

		public virtual void OnMyTargetSwitch()
		{
		}

		public virtual void CmdCommonLearnSkill(IFrameCommand cmd)
		{
		}

		public void SetLockTargetID(uint _targetID)
		{
			if (this.actor.LockTargetAttackModeControl != null)
			{
				this.actor.LockTargetAttackModeControl.SetLockTargetID(_targetID, false);
			}
		}

		public uint GetLockTargetID()
		{
			if (this.actor.LockTargetAttackModeControl != null)
			{
				return this.actor.LockTargetAttackModeControl.GetLockTargetID();
			}
			return 0u;
		}

		public virtual BaseAttackMode GetCurrentAttackMode()
		{
			return this.actor.DefaultAttackModeControl;
		}

		public virtual int GetDetectedRadius()
		{
			return (this.actor.shape == null) ? 0 : this.actor.shape.AvgCollisionRadius;
		}
	}
}

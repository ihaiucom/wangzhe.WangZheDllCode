using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SkillSlot
	{
		public const float MAX_KEY_PRESS_TIME = 0.1f;

		public const long SKILL_DISTANCE_CHECK_VALUE = 12000L;

		public PoolObjHandle<ActorRoot> Actor;

		public SkillChangeEvent skillChangeEvent;

		private int skillLevel;

		private SkillBean skillBean;

		private uint skillUseCount;

		private bool bLimitUse;

		private float keyPressTime;

		private uint skillTargetId;

		public ListValueView<uint> NextSkillTargetIDs = new ListValueView<uint>();

		private int changeSkillCDRate;

		private int eventTime;

		public Skill InitSkillObj;

		public long lLastUseTime;

		public SkillControlIndicator skillIndicator;

		public SkillSlotType SlotType
		{
			get;
			set;
		}

		public bool IsCDReady
		{
			get;
			set;
		}

		public int skillBeanAmount
		{
			get
			{
				return this.skillBean.GetBeanAmount();
			}
		}

		public bool bConsumeBean
		{
			get
			{
				return this.skillBean.ConsumeBean();
			}
		}

		public bool IsSkillBeanEnough
		{
			get
			{
				return this.skillBean.IsBeanEnough();
			}
		}

		public bool IsEnergyEnough
		{
			get
			{
				return this.Actor && this.Actor.handle != null && this.Actor.handle.ValueComponent != null && (this.Actor.handle.ValueComponent.IsEnergyType(EnergyType.NoneResource) || this.Actor.handle.ValueComponent.actorEp >= this.NextSkillEnergyCostTotal()) && this.IsSkillBeanEnough;
			}
		}

		public bool EnableButtonFlag
		{
			get
			{
				return this.Actor && this.Actor.handle != null && this.Actor.handle.ValueComponent != null && this.CurSkillCD <= 0 && this.IsEnergyEnough && this.skillLevel > 0 && this.skillBean.IsBeanEnough() && (!this.Actor.handle.ActorControl.IsDeadState || this.Actor.handle.TheStaticData.TheBaseAttribute.DeadControl);
			}
		}

		public CrypticInt32 CurSkillCD
		{
			get;
			set;
		}

		public PassiveSkill PassiveSkillObj
		{
			get;
			set;
		}

		public Skill SkillObj
		{
			get;
			set;
		}

		public Skill NextSkillObj
		{
			get;
			set;
		}

		private SkillSlot()
		{
		}

		public SkillSlot(SkillSlotType type)
		{
			this.IsCDReady = true;
			this.SlotType = type;
			this.bLimitUse = false;
			this.skillChangeEvent = new SkillChangeEvent(this);
			this.skillIndicator = new SkillControlIndicator(this);
			this.skillBean = new SkillBean(this);
			this.changeSkillCDRate = 0;
			this.lLastUseTime = 0L;
			this.CurSkillCD = 0;
			this.skillUseCount = 0u;
			this.keyPressTime = 0f;
			this.skillTargetId = 0u;
			this.NextSkillTargetIDs = new ListValueView<uint>();
		}

		public uint GetSkillUseCount()
		{
			return this.skillUseCount;
		}

		public void AddSkillUseCount()
		{
			if (this.skillUseCount >= 4294967295u)
			{
				this.skillUseCount = 0u;
			}
			this.skillUseCount += 1u;
			this.Actor.handle.BuffHolderComp.logicEffect.InitSkillSlotExtraHurt(this);
		}

		public void Reset()
		{
			this.Actor.Validate();
			if (this.PassiveSkillObj != null)
			{
				this.PassiveSkillObj.Reset();
			}
			this.NextSkillObj = null;
			this.CurSkillCD = 0;
			this.skillUseCount = 0u;
			this.IsCDReady = true;
			if (this.skillChangeEvent != null)
			{
				this.skillChangeEvent.Reset();
			}
			this.lLastUseTime = 0L;
			this.keyPressTime = 0f;
			this.skillTargetId = 0u;
			this.NextSkillTargetIDs.Clear();
		}

		public void SetSkillLevel(int _level)
		{
			this.skillLevel = _level;
			if (this.skillLevel == 0)
			{
				DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.SlotType, 0, this.Actor);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
			else if (this.skillLevel == 1)
			{
				DefaultSkillEventParam defaultSkillEventParam2 = new DefaultSkillEventParam(this.SlotType, 0, this.Actor);
				if (Singleton<WatchController>.GetInstance().IsWatching)
				{
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, this.Actor, ref defaultSkillEventParam2, GameSkillEventChannel.Channel_AllActor);
				}
				else
				{
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, this.Actor, ref defaultSkillEventParam2, GameSkillEventChannel.Channel_HostCtrlActor);
				}
			}
		}

		public void UseSkillBean()
		{
			this.skillBean.BeanUse();
		}

		public int GetSkillLevel()
		{
			return this.skillLevel;
		}

		public bool IsUnLock()
		{
			return this.skillLevel > 0;
		}

		public void ChangeMaxCDRate(int _rate)
		{
			this.changeSkillCDRate += _rate;
		}

		public void SetMaxCDRate(int inRate)
		{
			this.changeSkillCDRate = inRate;
		}

		public void ResetSkillObj(bool bDead)
		{
			this.SkillObj = this.InitSkillObj;
			if (bDead)
			{
				this.skillChangeEvent.Leave();
			}
			else
			{
				this.skillChangeEvent.Stop();
			}
			this.NextSkillObj = null;
		}

		public void ReadySkillObj()
		{
			if (this.NextSkillObj != null)
			{
				this.SkillObj = this.NextSkillObj;
				this.NextSkillObj = null;
				this.skillChangeEvent.Stop();
			}
		}

		public void ForceAbort()
		{
			if (this.SkillObj != null && !this.SkillObj.isFinish)
			{
				this.SkillObj.Stop();
				this.skillChangeEvent.Abort();
			}
		}

		public bool IsAbort(SkillAbortType _type)
		{
			return this.SkillObj == null || this.SkillObj.isFinish || this.SkillObj.canAbort(_type);
		}

		public bool ImmediateAbort(SkillAbortType _type)
		{
			if (this.SkillObj == null)
			{
				return true;
			}
			if (this.SkillObj.isFinish)
			{
				return true;
			}
			if (!this.SkillObj.canAbort(_type))
			{
				return false;
			}
			this.SkillObj.Stop();
			this.skillChangeEvent.Abort();
			return true;
		}

		public bool Abort(SkillAbortType _type)
		{
			if (this.SkillObj == null)
			{
				return true;
			}
			if (this.SkillObj.isFinish)
			{
				return true;
			}
			if (!this.SkillObj.canAbort(_type))
			{
				return false;
			}
			this.SkillObj.Stop();
			this.skillChangeEvent.Abort();
			return true;
		}

		public int GetSkillCDMax()
		{
			int num = this.SkillObj.cfgData.iCoolDown;
			if (this.SlotType == SkillSlotType.SLOT_SKILL_9 || this.SlotType == SkillSlotType.SLOT_SKILL_10)
			{
				if (this.Actor && this.Actor.handle.SkillControl.bZeroCd)
				{
					num = 0;
				}
				return num;
			}
			int num2 = this.skillLevel - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			num += this.SkillObj.cfgData.iCoolDownGrowth * num2;
			if (this.Actor)
			{
				int num3 = 0;
				ValueDataInfo valueDataInfo = this.Actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CDREDUCE];
				DebugHelper.Assert(valueDataInfo != null, "Failed get value data");
				if (valueDataInfo == null)
				{
					return 10;
				}
				if (valueDataInfo != null && this.SlotType != SkillSlotType.SLOT_SKILL_0 && this.SlotType != SkillSlotType.SLOT_SKILL_4 && this.SlotType != SkillSlotType.SLOT_SKILL_5 && this.SlotType != SkillSlotType.SLOT_SKILL_7)
				{
					num3 = valueDataInfo.totalValue;
				}
				int num4 = num3 + this.changeSkillCDRate;
				if (valueDataInfo.maxLimitValue > 0)
				{
					num4 = ((num4 > valueDataInfo.maxLimitValue) ? valueDataInfo.maxLimitValue : num4);
				}
				long num5 = (long)num * (10000L - (long)num4);
				num = (int)(num5 / 10000L);
			}
			num = ((num < 0) ? 0 : num);
			if (this.SlotType == SkillSlotType.SLOT_SKILL_0)
			{
				ValueDataInfo valueDataInfo2 = this.Actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD];
				DebugHelper.Assert(valueDataInfo2 != null, "Failed get value data skill 0");
				num = num * 10000 / (10000 + valueDataInfo2.totalValue);
				num = ((num < 0) ? 0 : num);
			}
			if (this.Actor && this.Actor.handle.SkillControl.bZeroCd && this.SlotType != SkillSlotType.SLOT_SKILL_0)
			{
				num = 0;
			}
			return num;
		}

		public void StartSkillCD()
		{
			this.CurSkillCD = this.GetSkillCDMax();
			this.IsCDReady = false;
			this.eventTime = 0;
			if (this.SlotType == SkillSlotType.SLOT_SKILL_7)
			{
				this.Actor.handle.SkillControl.ornamentFirstSwitchCdEftTime = 0;
			}
			DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.SlotType, this.CurSkillCD, this.Actor);
			if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
			{
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_AllActor);
			}
		}

		public void ResetSkillCD()
		{
			if (!this.IsCDReady)
			{
				this.CurSkillCD = 0;
				this.IsCDReady = true;
				if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
				{
					DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.SlotType, this.CurSkillCD, this.Actor);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_AllActor);
					if (this.IsCDReady)
					{
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					}
				}
			}
		}

		public void ChangeSkillCD(int _time)
		{
			if (!this.IsCDReady)
			{
				this.CurSkillCD -= _time;
				if (this.CurSkillCD < 0)
				{
					this.CurSkillCD = 0;
					this.IsCDReady = true;
				}
				if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
				{
					DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.SlotType, this.CurSkillCD, this.Actor);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_AllActor);
					if (this.IsCDReady)
					{
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					}
				}
			}
		}

		private void UpdateSkillCD(int nDelta)
		{
			if (!this.IsCDReady)
			{
				this.CurSkillCD -= nDelta;
				this.eventTime += nDelta;
				if (this.CurSkillCD <= 0)
				{
					this.CurSkillCD = 0;
					this.IsCDReady = true;
				}
				if (this.eventTime >= 200 || this.CurSkillCD == 0)
				{
					if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
					{
						DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.SlotType, this.CurSkillCD, this.Actor);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_AllActor);
						if (this.IsCDReady)
						{
							Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
						}
					}
					this.eventTime -= 200;
				}
			}
		}

		public int CurSkillEnergyCostTotal()
		{
			if (this.Actor && this.Actor.handle.SkillControl != null && this.Actor.handle.SkillControl.bZeroCd)
			{
				return 0;
			}
			int num = this.skillLevel;
			num = ((num > 0) ? num : 1);
			if (this.SkillObj != null)
			{
				return this.SkillObj.SkillEnergyCost(this.Actor, num);
			}
			return 0;
		}

		public void CurSkillEnergyCostTick()
		{
			if (!this.Actor || this.Actor.handle.ValueComponent == null || this.Actor.handle.ValueComponent.IsEnergyType(EnergyType.NoneResource))
			{
				return;
			}
			this.Actor.handle.ValueComponent.actorEp -= this.CurSkillEnergyCostTotal();
		}

		public void AutoSkillEnergyCost()
		{
			if (this.SkillObj != null && this.SkillObj.cfgData != null && this.SkillObj.cfgData.bAutoEnergyCost == 0)
			{
				this.CurSkillEnergyCostTick();
			}
		}

		public int NextSkillEnergyCostTotal()
		{
			if (this.Actor && this.Actor.handle.SkillControl != null && this.Actor.handle.SkillControl.bZeroCd)
			{
				return 0;
			}
			int num = this.skillLevel;
			num = ((num > 0) ? num : 1);
			if (this.NextSkillObj != null)
			{
				return this.NextSkillObj.SkillEnergyCost(this.Actor, num);
			}
			if (this.SkillObj != null)
			{
				return this.SkillObj.SkillEnergyCost(this.Actor, num);
			}
			return 0;
		}

		public void Init(ref PoolObjHandle<ActorRoot> _actor, Skill skill, PassiveSkill passive)
		{
			this.Actor = _actor;
			this.IsCDReady = true;
			this.CurSkillCD = 0;
			this.SkillObj = skill;
			this.InitSkillObj = skill;
			this.NextSkillObj = null;
			this.SkillObj.SlotType = this.SlotType;
			this.PassiveSkillObj = passive;
			if (this.PassiveSkillObj != null)
			{
				this.PassiveSkillObj.SlotType = this.SlotType;
			}
			this.skillBean.Init();
			this.skillTargetId = 0u;
			this.NextSkillTargetIDs.Clear();
		}

		public void UnInit()
		{
			this.Actor.Release();
			this.SkillObj = null;
			this.InitSkillObj = null;
			this.NextSkillObj = null;
			this.PassiveSkillObj = null;
			this.skillIndicator.UnInitIndicatePrefab(true);
			this.skillTargetId = 0u;
			this.NextSkillTargetIDs.Clear();
		}

		public void InitSkillControlIndicator()
		{
			this.skillIndicator.InitControlIndicator();
			this.skillIndicator.CreateIndicatePrefab(this.SkillObj);
		}

		public void LateUpdate(int nDelta)
		{
			this.skillIndicator.LateUpdate(nDelta);
		}

		public void UpdateLogic(int nDelta)
		{
			this.UpdateSkillCD(nDelta);
			this.skillChangeEvent.UpdateSkillCD(nDelta);
			if (this.IsUnLock())
			{
				this.skillBean.UpdateLogic(nDelta);
			}
			if (this.PassiveSkillObj != null && this.PassiveSkillObj.cfgData != null && this.IsSkillUnlock(this.PassiveSkillObj.cfgData.iSkillType))
			{
				this.PassiveSkillObj.UpdateLogic(nDelta);
			}
		}

		private bool IsSkillUnlock(int slotType)
		{
			if (slotType >= 1 && slotType <= 3 && this.Actor)
			{
				SkillSlot skillSlot = this.Actor.handle.SkillControl.GetSkillSlot((SkillSlotType)slotType);
				if (skillSlot != null)
				{
					return skillSlot.IsUnLock();
				}
			}
			return true;
		}

		public bool IsEnableSkillSlot()
		{
			return this.IsCDReady && this.IsUnLock() && !this.bLimitUse && (!this.Actor.handle.ActorControl.IsDeadState || this.Actor.handle.TheStaticData.TheBaseAttribute.DeadControl) && this.IsEnergyEnough;
		}

		public bool CanEnableSkillSlotByEnergy()
		{
			return this.IsCDReady && this.IsUnLock() && !this.bLimitUse && this.skillBean.IsBeanEnough() && !this.Actor.handle.ActorControl.IsDeadState;
		}

		public bool IsUseSkillJoystick()
		{
			return this.SkillObj != null && this.SkillObj.cfgData != null && (this.IsEnableSkillSlot() || this.Actor.handle.ActorControl.IsUseAdvanceCommonAttack()) && (this.SlotType != SkillSlotType.SLOT_SKILL_0 || this.Actor.handle.ActorControl.IsUseAdvanceCommonAttack());
		}

		public void SendSkillCooldownEvent()
		{
			if (!this.IsCDReady)
			{
				ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, this.Actor, ref actorSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public void SendSkillShortageEvent()
		{
			if (!this.IsEnergyEnough)
			{
				ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, this.Actor, ref actorSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public void SendSkillBeanShortageEvent()
		{
			if (!this.IsSkillBeanEnough)
			{
				ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_SkillBeanShortage, this.Actor, ref actorSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		private void ReadyUseSkillDefaultAttackMode(Skill readySkillObj, bool bForceSkillUseInDefaultPosition = false)
		{
			if (bForceSkillUseInDefaultPosition)
			{
				this.skillIndicator.SetSkillUseDefaultPosition();
			}
			else
			{
				this.skillTargetId = 0u;
				ActorRoot actorRoot = null;
				if (readySkillObj.AppointType != SkillRangeAppointType.Target)
				{
					actorRoot = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(this.Actor.handle, readySkillObj.cfgData.iMaxAttackDistance, TargetPriority.TargetPriority_Hero, readySkillObj.cfgData.dwSkillTargetFilter, true, true);
				}
				if (actorRoot != null)
				{
					this.skillTargetId = actorRoot.ObjID;
					this.skillIndicator.SetSkillUsePosition(actorRoot);
				}
				else
				{
					this.skillIndicator.SetSkillUseDefaultPosition();
				}
			}
			if (readySkillObj.AppointType == SkillRangeAppointType.Target && readySkillObj.cfgData.bSkillTargetRule != 2)
			{
				this.skillIndicator.SetGuildPrefabShow(false);
				this.skillIndicator.SetGuildWarnPrefabShow(false);
				this.skillIndicator.SetUseAdvanceMode(false);
				this.skillIndicator.SetSkillUseDefaultPosition();
				this.skillIndicator.SetEffectPrefabShow(false);
				this.skillIndicator.SetFixedPrefabShow(true);
			}
			else
			{
				this.skillIndicator.SetGuildPrefabShow(true);
				this.skillIndicator.SetGuildWarnPrefabShow(false);
				this.skillIndicator.SetUseAdvanceMode(true);
			}
		}

		private void ReadyUseSkillLockAttackMode(Skill readySkillObj, bool bForceSkillUseInDefaultPosition = false)
		{
			if (bForceSkillUseInDefaultPosition)
			{
				this.skillIndicator.SetSkillUseDefaultPosition();
			}
			else
			{
				this.skillTargetId = 0u;
				SelectEnemyType selectEnemyType = SelectEnemyType.SelectLowHp;
				uint num = this.Actor.handle.LockTargetAttackModeControl.GetLockTargetID();
				if (!this.Actor.handle.LockTargetAttackModeControl.IsValidLockTargetID(num))
				{
					Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.Actor);
					if (ownerPlayer != null)
					{
						selectEnemyType = ownerPlayer.AttackTargetMode;
					}
					if (readySkillObj.AppointType == SkillRangeAppointType.Target)
					{
						num = 0u;
					}
					else
					{
						int srchR = readySkillObj.cfgData.iMaxAttackDistance;
						uint dwSkillTargetFilter = readySkillObj.cfgData.dwSkillTargetFilter;
						if (selectEnemyType == SelectEnemyType.SelectLowHp)
						{
							num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.Actor, srchR, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
						}
						else
						{
							num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.Actor, srchR, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
						}
					}
				}
				PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(num);
				if (actor)
				{
					this.skillTargetId = actor.handle.ObjID;
					this.skillIndicator.SetSkillUsePosition(actor.handle);
				}
				else
				{
					this.skillIndicator.SetSkillUseDefaultPosition();
				}
			}
			if (readySkillObj.AppointType == SkillRangeAppointType.Target && readySkillObj.cfgData.bSkillTargetRule != 2)
			{
				this.skillIndicator.SetGuildPrefabShow(false);
				this.skillIndicator.SetGuildWarnPrefabShow(false);
				this.skillIndicator.SetUseAdvanceMode(false);
				this.skillIndicator.SetSkillUseDefaultPosition();
				this.skillIndicator.SetEffectPrefabShow(false);
				this.skillIndicator.SetFixedPrefabShow(true);
			}
			else
			{
				this.skillIndicator.SetGuildPrefabShow(true);
				this.skillIndicator.SetGuildWarnPrefabShow(false);
				this.skillIndicator.SetUseAdvanceMode(true);
			}
		}

		public void ReadyUseSkill(bool bForceSkillUseInDefaultPosition = false)
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			if (skill == null || skill.cfgData == null || skill.cfgData.bWheelType == 1)
			{
				return;
			}
			OperateMode playerOperateMode = ActorHelper.GetPlayerOperateMode(ref this.Actor);
			if (!Singleton<GameInput>.GetInstance().IsSmartUse() && playerOperateMode == OperateMode.DefaultMode)
			{
				this.ReadyUseSkillDefaultAttackMode(skill, bForceSkillUseInDefaultPosition);
			}
			else if (playerOperateMode == OperateMode.LockMode)
			{
				this.ReadyUseSkillLockAttackMode(skill, bForceSkillUseInDefaultPosition);
			}
		}

		private void RequestUseSkillGeneralMode()
		{
			bool flag = false;
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			this.skillIndicator.SetFixedPrefabShow(false);
			this.skillIndicator.SetGuildPrefabShow(false);
			this.skillIndicator.SetGuildWarnPrefabShow(false);
			if (Time.realtimeSinceStartup - this.keyPressTime < 0.1f)
			{
				return;
			}
			if (skill == null || skill.cfgData == null || !this.IsEnableSkillSlot())
			{
				return;
			}
			if (Singleton<SkillDetectionControl>.GetInstance().Detection((SkillUseRule)skill.cfgData.bSkillUseRule, this))
			{
				flag = this.SendRequestUseSkill();
				if (flag)
				{
					this.keyPressTime = Time.realtimeSinceStartup;
				}
			}
			if (!flag)
			{
				ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, this.Actor, ref actorSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		private void RequestUseSkillSelectMode(uint objID)
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			if (objID == 0u || skill == null || skill.cfgData == null || !this.IsEnableSkillSlot())
			{
				MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
				return;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor || actor.handle.ActorControl.IsDeadState)
			{
				MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
				return;
			}
			if (Singleton<SkillDetectionControl>.GetInstance().Detection((SkillUseRule)skill.cfgData.bSkillUseRule, this))
			{
				FrameCommand<UseObjectiveSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
				frameCommand.cmdData.ObjectID = objID;
				frameCommand.cmdData.SlotType = this.SlotType;
				frameCommand.Send();
			}
			else
			{
				MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
			}
		}

		public bool CanUseSkillWithEnemyHeroSelectMode()
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			return skill != null && skill.cfgData != null && this.IsEnableSkillSlot() && skill.cfgData.bSkillUseRule == 1;
		}

		private void RequestUseSkillEnemyHeroSelectMode(uint objID)
		{
			if (objID == 0u)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor || actor.handle.ActorControl.IsDeadState)
			{
				return;
			}
			if (this.CanUseSkillWithEnemyHeroSelectMode())
			{
				Skill arg_5C_0 = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
				FrameCommand<UseObjectiveSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
				frameCommand.cmdData.ObjectID = objID;
				frameCommand.cmdData.SlotType = this.SlotType;
				frameCommand.Send();
			}
		}

		public void RequestUseSkill(enSkillJoystickMode mode, uint objID)
		{
			if (mode == enSkillJoystickMode.General)
			{
				this.RequestUseSkillGeneralMode();
			}
			else if (mode == enSkillJoystickMode.SelectTarget || mode == enSkillJoystickMode.MapSelect || mode == enSkillJoystickMode.SelectNextSkillTarget || mode == enSkillJoystickMode.MapSelectOther || mode == enSkillJoystickMode.SelectTarget5)
			{
				this.RequestUseSkillSelectMode(objID);
			}
			else if (mode == enSkillJoystickMode.EnemyHeroSelect)
			{
				this.RequestUseSkillEnemyHeroSelectMode(objID);
			}
		}

		public void CancelUseSkill()
		{
			if (this.skillIndicator != null)
			{
				this.skillIndicator.UnInitIndicatePrefab(false);
			}
			this.keyPressTime = 0f;
		}

		public void DestoryIndicatePrefab()
		{
			if (this.skillIndicator != null)
			{
				this.skillIndicator.UnInitIndicatePrefab(true);
			}
		}

		private bool SendRequestUseSkillTarget(Skill readySkillObj)
		{
			uint num = 0u;
			BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
			FrameCommand<UseObjectiveSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
			if (currentAttackMode != null)
			{
				num = currentAttackMode.SelectSkillTarget(this);
			}
			if (num == 0u)
			{
				return false;
			}
			frameCommand.cmdData.ObjectID = num;
			frameCommand.cmdData.SlotType = this.SlotType;
			frameCommand.Send();
			return true;
		}

		private bool SendRequestUseSkillDir(Skill readySkillObj)
		{
			VInt3 vInt = VInt3.one;
			BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
			FrameCommand<UseDirectionalSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseDirectionalSkillCommand>();
			if (currentAttackMode != null)
			{
				vInt = currentAttackMode.SelectSkillDirection(this);
			}
			frameCommand.cmdData.SlotType = this.SlotType;
			if (vInt.x == 0 && vInt.z == 0)
			{
				vInt = this.Actor.handle.forward;
			}
			short degree = (short)((double)(IntMath.atan2(-vInt.z, vInt.x).single * 180f) / 3.1416);
			frameCommand.cmdData.Degree = degree;
			if (!this.skillIndicator.GetSkillBtnDrag())
			{
				frameCommand.cmdData.dwObjectID = this.skillTargetId;
			}
			frameCommand.Send();
			return true;
		}

		private bool SendRequestUseSkillPos(Skill readySkillObj)
		{
			bool flag = false;
			VInt3 zero = VInt3.zero;
			BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
			FrameCommand<UsePositionSkillCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UsePositionSkillCommand>();
			if (currentAttackMode != null)
			{
				flag = currentAttackMode.SelectSkillPos(this, out zero);
			}
			if (flag)
			{
				frameCommand.cmdData.Position = new VInt2(zero.x, zero.z);
				frameCommand.cmdData.SlotType = this.SlotType;
				frameCommand.Send();
				return true;
			}
			return false;
		}

		private bool SendRequestUseSkill()
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			if (skill == null)
			{
				return false;
			}
			if (skill.AppointType == SkillRangeAppointType.Target)
			{
				return this.SendRequestUseSkillTarget(skill);
			}
			if (skill.AppointType == SkillRangeAppointType.Directional)
			{
				this.SendRequestUseSkillDir(skill);
			}
			else if (skill.AppointType == SkillRangeAppointType.Pos)
			{
				this.SendRequestUseSkillPos(skill);
			}
			return true;
		}

		private bool IsValidSkillTargetFilter(ref PoolObjHandle<ActorRoot> _target)
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			return skill.cfgData.dwSkillTargetFilter == 0u || ((ulong)skill.cfgData.dwSkillTargetFilter & 1uL << (int)(_target.handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL;
		}

		public bool IsValidSkillTarget(ref PoolObjHandle<ActorRoot> _target)
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			if (!_target)
			{
				return false;
			}
			if (skill.cfgData.bSkillTargetRule == 0 || skill.cfgData.bSkillTargetRule == 1 || skill.cfgData.bSkillTargetRule == 3)
			{
				if (this.Actor.handle.IsEnemyCamp(_target) && this.IsValidSkillTargetFilter(ref _target))
				{
					return true;
				}
			}
			else if (skill.cfgData.bSkillTargetRule == 2)
			{
				if (this.Actor == _target)
				{
					return true;
				}
			}
			else if (skill.cfgData.bSkillTargetRule == 4)
			{
				if (this.Actor.handle.IsSelfCamp(_target) && this.IsValidSkillTargetFilter(ref _target))
				{
					return skill.cfgData.bWheelType != 1 || !(this.Actor == _target);
				}
			}
			else if (skill.cfgData.bSkillTargetRule == 5 && this.Actor.handle.IsEnemyCamp(_target) && this.IsValidSkillTargetFilter(ref _target) && this.NextSkillTargetIDs.IndexOf(_target.handle.ObjID) >= 0)
			{
				return true;
			}
			return false;
		}

		private bool IsValidSkillTarget(ref SkillUseParam _param)
		{
			return this.IsValidSkillTarget(ref _param.TargetActor);
		}

		public bool IsSkillUseValid(ref SkillUseParam _param)
		{
			Skill skill = (this.NextSkillObj != null) ? this.NextSkillObj : this.SkillObj;
			if (skill != null && skill.cfgData != null && this.Actor)
			{
				if (_param.SlotType != SkillSlotType.SLOT_SKILL_0 && this.Actor && this.Actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && _param.AppointType != skill.AppointType)
				{
					return false;
				}
				if (skill.AppointType == SkillRangeAppointType.Target || skill.AppointType == SkillRangeAppointType.Auto)
				{
					if (_param.bSpecialUse && this.SlotType == SkillSlotType.SLOT_SKILL_0)
					{
						return true;
					}
					if (!this.IsValidSkillTarget(ref _param))
					{
						return false;
					}
					long num = skill.cfgData.iMaxAttackDistance * 12000L / 10000L;
					if (_param.TargetActor.handle.ActorControl != null)
					{
						num += ((_param.TargetActor.handle.ActorControl != null) ? ((long)_param.TargetActor.handle.ActorControl.GetDetectedRadius()) : 0L);
					}
					else
					{
						num += ((_param.TargetActor.handle.shape != null) ? ((long)_param.TargetActor.handle.shape.AvgCollisionRadius) : 0L);
					}
					long num2 = num * num;
					long sqrMagnitudeLong2D = (this.Actor.handle.location - _param.TargetActor.handle.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D <= num2)
					{
						return true;
					}
				}
				else if (skill.AppointType == SkillRangeAppointType.Pos)
				{
					long num3 = skill.cfgData.iMaxAttackDistance * 12000L / 10000L;
					long num4 = num3 * num3;
					long sqrMagnitudeLong2D2 = (this.Actor.handle.location - _param.UseVector).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D2 <= num4)
					{
						return true;
					}
				}
				else
				{
					if (skill.AppointType == SkillRangeAppointType.Directional)
					{
						return true;
					}
					if (skill.AppointType == SkillRangeAppointType.Track)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

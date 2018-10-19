using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SkillComponent : LogicComponent
	{
		public Skill CurUseSkill;

		public SkillSlot CurUseSkillSlot;

		public SkillCache SkillUseCache;

		public SkillSlot[] SkillSlotArray = new SkillSlot[10];

		public ListView<BulletSkill> SpawnedBullets = new ListView<BulletSkill>();

		public int[] DisableSkill = new int[10];

		public int[] ForceDisableSkill = new int[10];

		public VInt3 RecordPosition = VInt3.zero;

		public int m_iSkillPoint;

		public CSkillStat stSkillStat;

		public bool bImmediateAttack;

		public int ornamentFirstSwitchCdEftTime;

		public ulong RotateBodyBulletFindEnemyLogicFrameTick;

		private bool bIsCurAtkUseSkill;

		public bool bZeroCd
		{
			get;
			private set;
		}

		public TalentSystem talentSystem
		{
			get;
			private set;
		}

		public bool bIsLastAtkUseSkill
		{
			get;
			private set;
		}

		public bool isUsing
		{
			get
			{
				return this.CurUseSkill != null;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.CurUseSkill = null;
			this.CurUseSkillSlot = null;
			this.SkillUseCache = null;
			this.talentSystem = null;
			this.bIsLastAtkUseSkill = false;
			this.bIsCurAtkUseSkill = false;
			this.RecordPosition = VInt3.zero;
			this.SpawnedBullets.Clear();
			this.m_iSkillPoint = 0;
			this.bZeroCd = false;
			this.ClearSkillSlot();
			this.stSkillStat = null;
			this.bImmediateAttack = false;
			this.ornamentFirstSwitchCdEftTime = 0;
			this.RotateBodyBulletFindEnemyLogicFrameTick = 0uL;
		}

		public override void Deactive()
		{
			for (int i = 0; i < this.SpawnedBullets.Count; i++)
			{
				if (this.SpawnedBullets[i].isFinish)
				{
					this.SpawnedBullets[i].Release();
				}
				else
				{
					this.SpawnedBullets[i].bManaged = false;
				}
			}
			this.SpawnedBullets.Clear();
			base.Deactive();
		}

		public override void Reactive()
		{
			base.Reactive();
			this.CurUseSkill = null;
			this.CurUseSkillSlot = null;
			this.RecordPosition = VInt3.zero;
			this.SpawnedBullets.Clear();
			this.m_iSkillPoint = 0;
			this.bZeroCd = false;
			this.ornamentFirstSwitchCdEftTime = 0;
			this.ResetAllSkillSlot(false);
			this.ResetSkillCD();
			for (int i = 0; i < 10; i++)
			{
				this.DisableSkill[i] = 0;
				this.ForceDisableSkill[i] = 0;
			}
			if (this.SkillSlotArray != null)
			{
				for (int j = 0; j < this.SkillSlotArray.Length; j++)
				{
					SkillSlot skillSlot = this.SkillSlotArray[j];
					if (skillSlot != null)
					{
						skillSlot.Reset();
					}
				}
			}
			if (this.talentSystem != null)
			{
				this.talentSystem.Reset();
			}
		}

		private void InitPassiveSkill()
		{
			int passiveSkillID = this.actorPtr.handle.TheStaticData.TheBaseAttribute.PassiveSkillID1;
			int passiveSkillID2 = this.actorPtr.handle.TheStaticData.TheBaseAttribute.PassiveSkillID2;
			if (passiveSkillID != 0)
			{
				this.CreateTalent(passiveSkillID, SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL, this.actorPtr.handle.ObjID);
			}
			if (passiveSkillID2 != 0)
			{
				this.CreateTalent(passiveSkillID2, SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL, this.actorPtr.handle.ObjID);
			}
		}

		private void InitRandomSkill()
		{
			if (this.actor != null)
			{
				this.InitRandomSkill(this.actor.TheStaticData.TheBaseAttribute.RandomPassiveSkillRule);
			}
		}

		public void InitRandomSkill(int inPassSkillRule)
		{
			int num = 0;
			if (inPassSkillRule != 0)
			{
				DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive || Singleton<FrameSynchr>.instance.isRunning);
				if (Singleton<FrameSynchr>.instance.isRunning)
				{
					ResRandomSkillPassiveRule dataByKey = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long)inPassSkillRule);
					if (dataByKey != null)
					{
						for (int i = 0; i < 20; i++)
						{
							if (dataByKey.astRandomSkillPassiveID1[i].iParam == 0)
							{
								break;
							}
							num++;
						}
						if (num > 0)
						{
							ushort num2 = FrameRandom.Random((uint)num);
							int iParam = dataByKey.astRandomSkillPassiveID1[(int)num2].iParam;
							this.CreateTalent(iParam, SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL, this.actorPtr.handle.ObjID);
							int iParam2 = dataByKey.astRandomSkillPassiveID2[(int)num2].iParam;
							if (iParam2 != 0)
							{
								this.CreateTalent(iParam2, SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL, this.actorPtr.handle.ObjID);
							}
						}
					}
				}
			}
		}

		public void ToggleZeroCd()
		{
			this.ResetSkillCD();
			this.bZeroCd = !this.bZeroCd;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", this.actorPtr, this.actor.ValueComponent.actorEp, this.actor.ValueComponent.actorEpTotal);
		}

		public override void Init()
		{
			base.Init();
			this.talentSystem = new TalentSystem();
			this.talentSystem.Init(this.actorPtr);
			this.stSkillStat = new CSkillStat();
			if (this.stSkillStat == null)
			{
				return;
			}
			this.stSkillStat.Initialize(this.actorPtr);
			this.InitRandomSkill();
			this.InitPassiveSkill();
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			IGameActorDataProvider actorDataProvider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			ActorStaticSkillData actorStaticSkillData = default(ActorStaticSkillData);
			this.ornamentFirstSwitchCdEftTime = Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_ornamentFirstSwitchCdEftTime;
			this.RotateBodyBulletFindEnemyLogicFrameTick = 0uL;
			int i = 0;
			while (i < 8)
			{
				if (i != 6)
				{
					goto IL_F3;
				}
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext.IsGameTypeGuide())
				{
					if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
					{
						goto IL_F3;
					}
				}
				else if (curLvelContext.IsMobaModeWithOutGuide() && curLvelContext.m_pvpPlayerNum == 10)
				{
					goto IL_F3;
				}
				IL_274:
				i++;
				continue;
				IL_F3:
				if ((i == 4 || i == 6 || i == 7) && this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					SLevelContext curLvelContext2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
					if (curLvelContext2 != null)
					{
						int mapID = curLvelContext2.m_mapID;
						if (mapID > 0)
						{
							int num = curLvelContext2.m_extraSkillId;
							if (i == 6)
							{
								num = curLvelContext2.m_extraSkill2Id;
								if (num <= 0)
								{
									goto IL_274;
								}
							}
							else if (i == 7)
							{
								if (!curLvelContext2.m_bEnableOrnamentSlot || curLvelContext2.m_ornamentSkillId <= 0)
								{
									goto IL_274;
								}
								num = curLvelContext2.m_ornamentSkillId;
							}
							else
							{
								this.CreateTalent(curLvelContext2.m_extraPassiveSkillId, SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL, 0u);
							}
							this.InitSkillSlot(i, num, 0);
							SkillSlot skillSlot = this.SkillSlotArray[i];
							if (skillSlot != null)
							{
								skillSlot.SetSkillLevel(1);
							}
						}
					}
					goto IL_274;
				}
				if (!actorDataProvider.GetActorStaticSkillData(ref this.actor.TheActorMeta, (ActorSkillSlot)i, ref actorStaticSkillData))
				{
					goto IL_274;
				}
				this.InitSkillSlot(i, actorStaticSkillData.SkillId, actorStaticSkillData.PassiveSkillId);
				if (i <= 3 && i >= 1 && Singleton<BattleLogic>.GetInstance().m_LevelContext.IsSoulGrow() && (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call))
				{
					goto IL_274;
				}
				SkillSlot skillSlot2 = this.SkillSlotArray[i];
				if (skillSlot2 == null)
				{
					goto IL_274;
				}
				skillSlot2.SetSkillLevel(1);
				goto IL_274;
			}
			uint num2 = 0u;
			if (actorDataProvider2.GetActorServerCommonSkillData(ref this.actor.TheActorMeta, out num2))
			{
				int num3 = 5;
				if (num2 != 0u)
				{
					this.InitSkillSlot(num3, (int)num2, 0);
					SkillSlot skillSlot3 = this.SkillSlotArray[num3];
					if (skillSlot3 != null)
					{
						skillSlot3.SetSkillLevel(1);
					}
				}
			}
			if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
			{
				this.SkillUseCache = new SkillCache();
			}
		}

		public void ResetSkillLevel()
		{
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
			int num = this.SkillSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null && (skillSlot.SlotType == SkillSlotType.SLOT_SKILL_1 || skillSlot.SlotType == SkillSlotType.SLOT_SKILL_2 || skillSlot.SlotType == SkillSlotType.SLOT_SKILL_3))
				{
					skillSlot.SetSkillLevel(0);
					if (captain == this.actorPtr && Singleton<CBattleSystem>.GetInstance().FightForm != null)
					{
						Singleton<CBattleSystem>.instance.FightForm.ClearSkillLvlStates(i);
					}
				}
			}
			if (captain == this.actorPtr && Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Singleton<CBattleSystem>.instance.FightForm.ResetSkillButtonManager(this.actorPtr, false, SkillSlotType.SLOT_SKILL_COUNT);
			}
			this.m_iSkillPoint = 0;
		}

		public override void Uninit()
		{
			base.Uninit();
			for (int i = 0; i < this.SpawnedBullets.Count; i++)
			{
				if (this.SpawnedBullets[i].isFinish)
				{
					this.SpawnedBullets[i].Release();
				}
				else
				{
					this.SpawnedBullets[i].bManaged = false;
				}
			}
			this.SpawnedBullets.Clear();
			for (int j = 0; j < 10; j++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[j];
				if (skillSlot != null && skillSlot.PassiveSkillObj != null && skillSlot.PassiveSkillObj.passiveEvent != null)
				{
					skillSlot.PassiveSkillObj.passiveEvent.UnInit();
				}
			}
			if (this.talentSystem != null)
			{
				this.talentSystem.UnInit();
			}
			if (this.stSkillStat != null)
			{
				this.stSkillStat.UnInit();
			}
		}

		public override void FightOver()
		{
			base.FightOver();
			for (int i = 0; i < 10; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null)
				{
					skillSlot.CancelUseSkill();
				}
			}
		}

		public void InitSkillSlot(int _slotType, int _skillID, int _passiveID)
		{
			if (!this.actorPtr)
			{
				DebugHelper.Assert(this.actorPtr);
				return;
			}
			Skill skill = new Skill(_skillID);
			PassiveSkill passive = null;
			if (_passiveID != 0)
			{
				passive = new PassiveSkill(_passiveID, this.actorPtr);
			}
			SkillSlot skillSlot = new SkillSlot((SkillSlotType)_slotType);
			skillSlot.Init(ref this.actorPtr, skill, passive);
			skillSlot.InitSkillControlIndicator();
			this.SkillSlotArray[_slotType] = skillSlot;
		}

		public void SetSkillIndicatorToCallMonster()
		{
			for (int i = 1; i <= 3; i++)
			{
				if (this.SkillSlotArray[i] != null && this.SkillSlotArray[i].SkillObj.cfgData.bSkillIndicatorFocusOnCallMonster == 1)
				{
					this.SkillSlotArray[i].skillIndicator.SetIndicatorToCallMonster();
				}
			}
		}

		public void CreateTalent(int _talentID, SKILL_USE_FROM_TYPE skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL, uint uiFromId = 0u)
		{
			this.talentSystem.InitTalent(_talentID, skillUseFrom, uiFromId);
		}

		private void ClearSkillSlot()
		{
			for (int i = 0; i < 10; i++)
			{
				this.DisableSkill[i] = 0;
				this.ForceDisableSkill[i] = 0;
				this.SkillSlotArray[i] = null;
			}
		}

		public bool IsIngnoreDisableSkill(SkillSlotType _type)
		{
			SkillSlot skillSlot = null;
			return _type >= SkillSlotType.SLOT_SKILL_0 && _type < SkillSlotType.SLOT_SKILL_COUNT && (this.TryGetSkillSlot(_type, out skillSlot) && skillSlot.SkillObj != null && skillSlot.SkillObj.cfgData != null && skillSlot.SkillObj.cfgData.bBIngnoreDisable == 1);
		}

		public bool IsSkillUseValid(SkillSlotType _type, ref SkillUseParam _param)
		{
			SkillSlot skillSlot;
			return this.TryGetSkillSlot(_type, out skillSlot) && skillSlot.IsSkillUseValid(ref _param);
		}

		public bool IsDisableSkillSlot(SkillSlotType _type)
		{
			return (!this.IsIngnoreDisableSkill(_type) || this.ForceDisableSkill[(int)_type] != 0) && _type >= SkillSlotType.SLOT_SKILL_0 && _type < SkillSlotType.SLOT_SKILL_COUNT && this.DisableSkill[(int)_type] > 0;
		}

		public void SetDisableSkillSlot(SkillSlotType _type, bool bAdd)
		{
			if (_type < SkillSlotType.SLOT_SKILL_0 || _type > SkillSlotType.SLOT_SKILL_COUNT)
			{
				return;
			}
			if (_type == SkillSlotType.SLOT_SKILL_COUNT)
			{
				for (int i = 0; i < 10; i++)
				{
					if (bAdd)
					{
						this.DisableSkill[i]++;
					}
					else
					{
						this.DisableSkill[i]--;
					}
				}
			}
			else if (bAdd)
			{
				this.DisableSkill[(int)_type]++;
			}
			else
			{
				this.DisableSkill[(int)_type]--;
			}
		}

		public void SetForceDisableSkillSlot(SkillSlotType _type, bool bAdd)
		{
			if (_type < SkillSlotType.SLOT_SKILL_0 || _type > SkillSlotType.SLOT_SKILL_COUNT)
			{
				return;
			}
			if (_type == SkillSlotType.SLOT_SKILL_COUNT)
			{
				for (int i = 0; i < 10; i++)
				{
					if (bAdd)
					{
						this.ForceDisableSkill[i]++;
					}
					else
					{
						this.ForceDisableSkill[i]--;
					}
				}
			}
			else if (bAdd)
			{
				this.ForceDisableSkill[(int)_type]++;
			}
			else
			{
				this.ForceDisableSkill[(int)_type]--;
			}
		}

		public bool TryGetSkillSlot(SkillSlotType _type, out SkillSlot _slot)
		{
			if (this.SkillSlotArray == null || _type < SkillSlotType.SLOT_SKILL_0 || _type >= SkillSlotType.SLOT_SKILL_COUNT)
			{
				_slot = null;
				return false;
			}
			_slot = this.SkillSlotArray[(int)_type];
			return _slot != null;
		}

		public bool IsUseSkillJoystick(SkillSlotType slot)
		{
			SkillSlot skillSlot;
			return !Singleton<GameInput>.GetInstance().IsSmartUse() && this.TryGetSkillSlot(slot, out skillSlot) && skillSlot.IsUseSkillJoystick();
		}

		public void ReadyUseSkillSlot(SkillSlotType slot, bool bForceSkillUseInDefaultPosition = false)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot))
			{
				skillSlot.ReadyUseSkill(bForceSkillUseInDefaultPosition);
			}
		}

		public GameObject GetPrefabEffectObj(SkillSlotType slot)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot) && skillSlot != null && skillSlot.skillIndicator != null)
			{
				return skillSlot.skillIndicator.effectPrefab;
			}
			return null;
		}

		public void SelectSkillTarget(SkillSlotType slot, Vector2 axis, bool isSkillCursorInCancelArea, bool isControlMove = true)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot))
			{
				skillSlot.skillIndicator.SelectSkillTarget(axis, isSkillCursorInCancelArea, isControlMove);
			}
		}

		public void RequestUseSkillSlot(SkillSlotType slot, enSkillJoystickMode mode, uint objID)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot))
			{
				skillSlot.RequestUseSkill(mode, objID);
			}
		}

		public uint GetAdvanceCommonAttackTarget()
		{
			return Singleton<CommonAttackSearcher>.GetInstance().AdvanceCommonAttackSearchEnemy(this.actorPtr, this.actor.ActorControl.SearchRange);
		}

		public void HostCancelUseSkillSlot(SkillSlotType slot, enSkillJoystickMode mode)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot))
			{
				skillSlot.CancelUseSkill();
				if (mode == enSkillJoystickMode.SelectTarget || mode == enSkillJoystickMode.SelectNextSkillTarget || mode == enSkillJoystickMode.SelectTarget5)
				{
					MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.actorPtr);
				}
				DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(slot, 0, this.actorPtr);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, this.actorPtr, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public SkillSlot GetSkillSlot(SkillSlotType slot)
		{
			SkillSlot result = null;
			this.TryGetSkillSlot(slot, out result);
			return result;
		}

		public void ResetAllSkillSlot(bool bDead)
		{
			for (int i = 0; i < 10; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null)
				{
					this.SkillSlotArray[i].ResetSkillObj(bDead);
					this.SkillSlotArray[i].skillIndicator.UnInitIndicatePrefab(false);
				}
			}
		}

		public void ResetSkillCD()
		{
			for (int i = 0; i < 10; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null)
				{
					this.SkillSlotArray[i].ResetSkillCD();
				}
			}
		}

		private void SkillInfoStatistic(ref SkillSlot stSkillSlot)
		{
			if (stSkillSlot == null)
			{
				return;
			}
			this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].uiUsedTimes += 1u;
			this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].bIsCurUseSkillHitHero = false;
			if (this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].iSkillCfgID == 0 || this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].iSkillCfgID == stSkillSlot.SkillObj.cfgData.iCfgID)
			{
				this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].iSkillCfgID = stSkillSlot.SkillObj.cfgData.iCfgID;
				this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].iAttackDistanceMax = Math.Max(this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].iAttackDistanceMax, stSkillSlot.SkillObj.cfgData.iMaxAttackDistance);
				long num = (long)(Time.realtimeSinceStartup * 1000f);
				uint val = (uint)(num - stSkillSlot.lLastUseTime);
				if (stSkillSlot.lLastUseTime != 0L)
				{
					uint num2 = this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].uiCDIntervalMin;
					num2 = Math.Min(num2, val);
					this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].uiCDIntervalMin = num2;
					uint num3 = this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].uiCDIntervalMax;
					num3 = Math.Max(num3, val);
					this.stSkillStat.SkillStatistictInfo[(int)stSkillSlot.SlotType].uiCDIntervalMax = num3;
				}
				stSkillSlot.lLastUseTime = num;
			}
		}

		private bool InternalUseSkill(ref SkillUseParam param, bool bImmediate = false)
		{
			SkillSlot skillSlot;
			if (!this.TryGetSkillSlot(param.SlotType, out skillSlot))
			{
				return false;
			}
			skillSlot.ReadySkillObj();
			Skill skillObj = skillSlot.SkillObj;
			if (!bImmediate)
			{
				this.CurUseSkill = skillObj;
				this.CurUseSkillSlot = skillSlot;
			}
			if (!skillObj.Use(this.actorPtr, ref param))
			{
				return false;
			}
			skillSlot.AddSkillUseCount();
			skillSlot.NextSkillTargetIDs.Clear();
			this.SkillInfoStatistic(ref skillSlot);
			this.bIsLastAtkUseSkill = this.bIsCurAtkUseSkill;
			if (param.SlotType == SkillSlotType.SLOT_SKILL_0)
			{
				this.bIsCurAtkUseSkill = false;
			}
			else
			{
				this.bIsCurAtkUseSkill = true;
			}
			ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(base.GetActor(), param.SlotType);
			Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, base.GetActor(), ref actorSkillEventParam, GameSkillEventChannel.Channel_AllActor);
			return true;
		}

		public bool UseSkill(ref SkillUseParam param, bool bImmediate = false)
		{
			return this.InternalUseSkill(ref param, bImmediate);
		}

		public override void LateUpdate(int nDelta)
		{
			if ((this.actorPtr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero || this.actorPtr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call) && ActorHelper.IsHostCtrlActor(ref this.actorPtr))
			{
				for (int i = 0; i < 10; i++)
				{
					SkillSlot skillSlot = this.SkillSlotArray[i];
					if (skillSlot != null)
					{
						skillSlot.LateUpdate(nDelta);
					}
				}
			}
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.CurUseSkill != null && this.CurUseSkill.isFinish)
			{
				this.CurUseSkill = null;
				this.CurUseSkillSlot = null;
			}
			for (int i = 0; i < this.SpawnedBullets.Count; i++)
			{
				BulletSkill bulletSkill = this.SpawnedBullets[i];
				if (bulletSkill != null)
				{
					bulletSkill.UpdateLogic(nDelta);
					if (bulletSkill.isFinish)
					{
						bulletSkill.Release();
						this.SpawnedBullets.RemoveAt(i);
						continue;
					}
				}
			}
			for (int j = 0; j < 10; j++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[j];
				if (skillSlot != null)
				{
					skillSlot.UpdateLogic(nDelta);
				}
			}
			if (this.talentSystem != null)
			{
				this.talentSystem.UpdateLogic(nDelta);
			}
		}

		public bool CanUseSkill(SkillSlotType slotType)
		{
			if (this.CurUseSkill != null && !this.CurUseSkill.canAbort((SkillAbortType)slotType))
			{
				return false;
			}
			Skill skill = this.FindSkill(slotType);
			SkillSlot skillSlot;
			return skill != null && skill.cfgData != null && this.TryGetSkillSlot(slotType, out skillSlot) && skillSlot.IsEnableSkillSlot();
		}

		public void ForceAbortCurUseSkill()
		{
			if (this.CurUseSkillSlot != null)
			{
				this.CurUseSkillSlot.ForceAbort();
			}
		}

		public void DelayAbortCurUseSkill()
		{
			if (this.CurUseSkillSlot != null && this.CurUseSkill != null)
			{
				if (!this.CurUseSkill.bProtectAbortSkill)
				{
					this.CurUseSkillSlot.ForceAbort();
				}
				else
				{
					this.CurUseSkill.bDelayAbortSkill = true;
				}
			}
		}

		public bool AbortCurUseSkill(SkillAbortType _type)
		{
			return this.CurUseSkillSlot == null || this.CurUseSkillSlot.Abort(_type);
		}

		public Skill FindSkill(SkillSlotType slot)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(slot, out skillSlot))
			{
				return skillSlot.SkillObj;
			}
			return null;
		}

		public bool IsSkillCDReady(SkillSlotType slot)
		{
			SkillSlot skillSlot;
			return this.TryGetSkillSlot(slot, out skillSlot) && skillSlot.IsCDReady;
		}

		public bool IsEnableSkillSlot(SkillSlotType slot)
		{
			SkillSlot skillSlot;
			return this.TryGetSkillSlot(slot, out skillSlot) && skillSlot.IsEnableSkillSlot();
		}

		public void OnDead()
		{
			for (int i = 0; i < 10; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null && skillSlot.skillChangeEvent != null && skillSlot.skillChangeEvent.IsActive())
				{
					skillSlot.skillChangeEvent.Abort();
				}
			}
			if (this.CurUseSkill != null)
			{
				this.CurUseSkill.Stop();
			}
			for (int j = 0; j < this.SpawnedBullets.Count; j++)
			{
				BulletSkill bulletSkill = this.SpawnedBullets[j];
				if (bulletSkill.IsDeadRemove)
				{
					bulletSkill.Stop();
					bulletSkill.Release();
					this.SpawnedBullets.RemoveAt(j);
					j--;
				}
			}
		}

		public void SetCommonAttackIndicator(bool bShow)
		{
			SkillSlot skillSlot;
			if (this.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_0, out skillSlot))
			{
				if (this.actor.ActorControl.IsUseAdvanceCommonAttack())
				{
					skillSlot.skillIndicator.SetFixedPrefabShow(bShow);
				}
				else
				{
					skillSlot.skillIndicator.SetGuildPrefabShow(bShow);
					skillSlot.skillIndicator.SetEffectPrefabShow(false);
				}
			}
		}

		private void RecycleOverLimitBullte(int _bulletTypeId, int _bulletUpperLimit)
		{
			if (_bulletUpperLimit > 0 && _bulletTypeId > 0)
			{
				int num = 0;
				int index = 0;
				for (int i = 0; i < this.SpawnedBullets.Count; i++)
				{
					if (this.SpawnedBullets[i].BulletTypeId == _bulletTypeId)
					{
						num++;
						if (num == 1)
						{
							index = i;
						}
					}
					if (num == _bulletUpperLimit)
					{
						this.SpawnedBullets[index].Stop();
						this.SpawnedBullets[index].Release();
						this.SpawnedBullets.RemoveAt(index);
						break;
					}
				}
			}
		}

		public PoolObjHandle<BulletSkill> SpawnBullet(SkillUseContext context, string _actionName, bool _bDeadRemove, bool _bAgeImmeExcute = false, int _bulletTypeId = 0, int _bulletUpperLimit = 0)
		{
			PoolObjHandle<BulletSkill> result = default(PoolObjHandle<BulletSkill>);
			if (context == null)
			{
				return result;
			}
			BulletSkill bulletSkill = ClassObjPool<BulletSkill>.Get();
			bulletSkill.Init(_actionName, _bDeadRemove, _bulletTypeId);
			bulletSkill.bAgeImmeExcute = _bAgeImmeExcute;
			this.RecycleOverLimitBullte(_bulletTypeId, _bulletUpperLimit);
			if (bulletSkill.Use(this.actorPtr, context))
			{
				this.SpawnedBullets.Add(bulletSkill);
				result = new PoolObjHandle<BulletSkill>(bulletSkill);
			}
			else
			{
				bulletSkill.Release();
			}
			return result;
		}

		public bool SpawnBuff(PoolObjHandle<ActorRoot> inTargetActor, ref SkillUseParam inParam, int inSkillCombineId, bool bExtraBuff = false)
		{
			if (!inTargetActor || inSkillCombineId <= 0)
			{
				return false;
			}
			BuffSkill buffSkill = ClassObjPool<BuffSkill>.Get();
			buffSkill.Init(inSkillCombineId);
			buffSkill.bExtraBuff = bExtraBuff;
			inParam.TargetActor = inTargetActor;
			inParam.Instigator = this.actor;
			bool flag = buffSkill.Use(this.actorPtr, ref inParam);
			if (!flag)
			{
				buffSkill.Release();
			}
			return flag;
		}

		public bool SpawnBuff(PoolObjHandle<ActorRoot> inTargetActor, SkillUseContext inContext, int inSkillCombineId, bool bExtraBuff = false)
		{
			BuffSkill buffSkill = null;
			return this.SpawnBuff(inTargetActor, inContext, inSkillCombineId, ref buffSkill, bExtraBuff);
		}

		public bool SpawnBuff(PoolObjHandle<ActorRoot> inTargetActor, SkillUseContext inContext, int inSkillCombineId, ref BuffSkill newSkill, bool bExtraBuff = false)
		{
			if (!inTargetActor || inContext == null || inSkillCombineId <= 0)
			{
				newSkill = null;
				return false;
			}
			BuffSkill buffSkill = ClassObjPool<BuffSkill>.Get();
			buffSkill.Init(inSkillCombineId);
			buffSkill.bExtraBuff = bExtraBuff;
			buffSkill.skillContext.Copy(inContext);
			buffSkill.skillContext.TargetActor = inTargetActor;
			buffSkill.skillContext.Instigator = this.actor;
			newSkill = buffSkill;
			bool flag = buffSkill.Use(this.actorPtr);
			if (!flag)
			{
				buffSkill.Release();
				newSkill = null;
			}
			return flag;
		}

		public bool RemoveBuff(PoolObjHandle<ActorRoot> inTargetActor, int inSkillCombineId)
		{
			if (inTargetActor)
			{
				inTargetActor.handle.BuffHolderComp.RemoveBuff(inSkillCombineId);
				return true;
			}
			return false;
		}

		public bool HasPunishSkill()
		{
			SkillSlot skillSlot = this.SkillSlotArray[5];
			return skillSlot != null && skillSlot.SkillObj != null && skillSlot.SkillObj.cfgData.bSkillType == 2;
		}

		public int GetAllSkillLevel()
		{
			int num = 0;
			for (int i = 1; i <= 3; i++)
			{
				SkillSlot skillSlot = this.SkillSlotArray[i];
				if (skillSlot != null)
				{
					num += skillSlot.GetSkillLevel();
				}
			}
			return num;
		}

		public void ChangePassiveParam(int _id, int _index, int _value)
		{
			if (this.talentSystem != null)
			{
				this.talentSystem.ChangePassiveParam(_id, _index, _value);
			}
		}
	}
}

using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class ValueProperty : LogicComponent
	{
		private CrypticInt32 _nObjCurHp = 1;

		public PropertyHelper mActorValue;

		public BaseEnergyLogic mEnergy;

		private int nHpRecoveryTick;

		private int nEpRecoveryTick;

		private CrypticInt32 _soulLevel = 1;

		private CrypticInt32 _soulExp = 0;

		private CrypticInt32 _soulMaxExp = 0;

		private CrypticInt32 m_goldCoinInBattle = 0;

		private CrypticInt32 m_goldCoinIncomeInBattle = 0;

		private CrypticInt32 m_MaxGoldCoinIncomeInBattle = 0;

		public ActorValueStatistic ObjValueStatistic;

		public event ValueChangeDelegate HpChgEvent;

		public event ValueChangeDelegate SoulLevelChgEvent;

		public int actorHp
		{
			get
			{
				return this._nObjCurHp;
			}
			set
			{
				int num = (value <= this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue) ? ((value >= 0) ? value : 0) : this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				CrypticInt32 nObjCurHp = this._nObjCurHp;
				if (nObjCurHp != num)
				{
					this._nObjCurHp = num;
					if (this.HpChgEvent != null)
					{
						this.HpChgEvent();
					}
					Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", this.actorPtr, num, this.actorHpTotal);
				}
			}
		}

		public int actorHpTotal
		{
			get
			{
				return this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			}
		}

		public int actorEp
		{
			get
			{
				return (this.mEnergy == null) ? 0 : this.mEnergy._actorEp;
			}
			set
			{
				if (this.mEnergy != null)
				{
					this.mEnergy.SetEpValue(value);
				}
			}
		}

		public int actorEpTotal
		{
			get
			{
				return (this.mEnergy == null) ? 1 : this.mEnergy.actorEpTotal;
			}
		}

		public int actorEpRecTotal
		{
			get
			{
				return (this.mEnergy == null) ? 0 : this.mEnergy.actorEpRecTotal;
			}
		}

		public int actorSoulLevel
		{
			get
			{
				return this._soulLevel;
			}
			set
			{
				if (!Singleton<BattleLogic>.instance.m_LevelContext.IsSoulGrow())
				{
					return;
				}
				VFactor a = new VFactor((long)this.actorHp, (long)this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
				VFactor a2 = new VFactor((long)this.actorEp, (long)this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue);
				bool flag = value > this._soulLevel || value == 1;
				this._soulLevel = value;
				this.mActorValue.SoulLevel = this.actorSoulLevel;
				this.SetSoulMaxExp();
				this.actorHp = (a * (long)this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue).roundInt;
				if (this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue > 0)
				{
					this.actorEp = (a2 * (long)this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue).roundInt;
				}
				if (this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					HeroWrapper heroWrapper = this.actor.ActorControl as HeroWrapper;
					if (heroWrapper != null)
					{
						PoolObjHandle<ActorRoot> callActor = heroWrapper.GetCallActor();
						if (callActor)
						{
							callActor.handle.ValueComponent.actorSoulLevel = this.actorSoulLevel;
						}
					}
				}
				if (flag && this.SoulLevelChgEvent != null)
				{
					this.SoulLevelChgEvent();
				}
				if (flag)
				{
					if (this.actor.SkillControl != null)
					{
						this.actor.SkillControl.m_iSkillPoint = value - this.actor.SkillControl.GetAllSkillLevel();
					}
					Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", this.actorPtr, value);
				}
			}
		}

		public int actorSoulExp
		{
			get
			{
				return this._soulExp;
			}
			set
			{
				this._soulExp = value;
			}
		}

		public int actorSoulMaxExp
		{
			get
			{
				return this._soulMaxExp;
			}
			set
			{
				this._soulMaxExp = value;
			}
		}

		public int actorMoveSpeed
		{
			get
			{
				MonsterWrapper monsterWrapper = this.actor.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.isCalledMonster && monsterWrapper.UseHostValueProperty)
				{
					return monsterWrapper.hostActor.handle.ValueComponent.actorMoveSpeed;
				}
				if (this.mActorValue != null)
				{
					int totalValue = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD].totalValue;
					int baseValue = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD].baseValue;
					return Singleton<SpeedAdjuster>.instance.HandleSpeedAdjust(baseValue, totalValue);
				}
				return 0;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.ClearVariables();
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			this._nObjCurHp = 1;
			this._soulLevel = 1;
			this._soulExp = 0;
			this._soulMaxExp = 0;
			this.m_goldCoinInBattle = 0;
			this.m_goldCoinIncomeInBattle = 0;
		}

		private void ClearVariables()
		{
			this._nObjCurHp = 1;
			this.mActorValue = null;
			this.nHpRecoveryTick = 0;
			this.nEpRecoveryTick = 0;
			this._soulLevel = 1;
			this._soulExp = 0;
			this._soulMaxExp = 0;
			this.HpChgEvent = null;
			this.SoulLevelChgEvent = null;
			this.ObjValueStatistic = null;
			this.m_goldCoinInBattle = 0;
			this.m_goldCoinIncomeInBattle = 0;
			this.m_MaxGoldCoinIncomeInBattle = 0;
			this.mEnergy = null;
		}

		public void ChangeActorEp(int value, int addType)
		{
			if (addType == 0)
			{
				this.actorEp += value;
			}
			else if (addType == 1)
			{
				this.actorEp = (int)((long)this.actorEp + (long)(this.actorEpTotal * value) / 10000L);
			}
		}

		public bool IsEnergyType(EnergyType energyType)
		{
			return this.mActorValue.EnergyType == energyType;
		}

		public override void Init()
		{
			base.Init();
			if (this.mActorValue == null)
			{
				this.mActorValue = new PropertyHelper();
				this.mActorValue.Init(ref this.actor.TheActorMeta);
			}
			if (this.ObjValueStatistic == null)
			{
				this.ObjValueStatistic = new ActorValueStatistic();
			}
			this.mEnergy = Singleton<EnergyCreater<BaseEnergyLogic, EnergyAttribute>>.GetInstance().Create((int)this.mActorValue.EnergyType);
			if (this.mEnergy == null)
			{
				this.mEnergy = new NoEnergy();
			}
			this.mEnergy.Init(this.actorPtr);
			this.actorHp = 0;
			this.actorEp = 0;
			this.SetHpAndEpToInitialValue(10000, 10000);
		}

		public override void Uninit()
		{
			base.Uninit();
			if (this.mEnergy != null)
			{
				this.mEnergy.Uninit();
			}
		}

		public void SetHpAndEpToInitialValue(int hpPercent = 10000, int epPercent = 10000)
		{
			this.actorHp = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue * hpPercent / 10000;
			this.mEnergy.ResetEpValue(epPercent);
		}

		public override void Deactive()
		{
			this.ClearVariables();
			base.Deactive();
		}

		public override void Reactive()
		{
			base.Reactive();
			this.Init();
		}

		public void ForceSoulLevelUp()
		{
			this.ForceSetSoulLevel(this.actorSoulLevel + 1);
		}

		public void ForceSetSoulLevel(int inNewLevel)
		{
			int maxSoulLvl = ValueProperty.GetMaxSoulLvl();
			if (inNewLevel > maxSoulLvl)
			{
				inNewLevel = maxSoulLvl;
			}
			if (inNewLevel < 1)
			{
				inNewLevel = 1;
			}
			this.actorSoulLevel = inNewLevel;
			int num = inNewLevel - 1;
			if (num >= 1)
			{
				ResSoulLvlUpInfo resSoulLvlUpInfo = Singleton<BattleLogic>.instance.incomeCtrl.QuerySoulLvlUpInfo((uint)num);
				if (resSoulLvlUpInfo != null)
				{
					this.actorSoulExp = (int)(resSoulLvlUpInfo.dwExp + 1u);
				}
			}
			else
			{
				this.actorSoulExp = 1;
			}
		}

		public void RecoverHp()
		{
			int nAddHp = this.actorHpTotal - this.actorHp;
			this.actor.ActorControl.ReviveHp(nAddHp);
		}

		public void RecoverEp()
		{
			if (!this.actor.ActorControl.IsDeadState)
			{
				this.actorEp = this.actorEpTotal;
			}
		}

		public override void Fight()
		{
			base.Fight();
			this.nHpRecoveryTick = 0;
			this.nEpRecoveryTick = 0;
			DebugHelper.Assert(this.mActorValue != null, "mActorValue = null data is error");
			if (this.mActorValue != null)
			{
				VFactor hpRate = this.GetHpRate();
				DebugHelper.Assert(this.actor != null, "actor is null ? impossible...");
				if (this.actor != null)
				{
					bool bPVPLevel = true;
					SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
					if (curLvelContext != null)
					{
						bPVPLevel = curLvelContext.IsMobaMode();
					}
					this.mActorValue.AddSymbolPageAttToProp(ref this.actor.TheActorMeta, bPVPLevel);
					IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
					ActorServerData actorServerData = default(ActorServerData);
					if (actorDataProvider != null && actorDataProvider.GetActorServerData(ref this.actor.TheActorMeta, ref actorServerData))
					{
						this.mActorValue.SetSkinProp((uint)this.actor.TheActorMeta.ConfigId, actorServerData.SkinId, true);
					}
				}
				this.SetHpByRate(hpRate);
			}
		}

		public override void UpdateLogic(int nDelta)
		{
			this.UpdateHpRecovery(nDelta);
			if (this.mEnergy != null)
			{
				this.mEnergy.UpdateLogic(nDelta);
			}
		}

		private void UpdateHpRecovery(int nDelta)
		{
			if (this.actor.ActorControl.IsDeadState)
			{
				this.nHpRecoveryTick = 0;
			}
			else
			{
				this.nHpRecoveryTick += nDelta;
				if (this.nHpRecoveryTick >= 5000)
				{
					this.actorHp += this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER].totalValue;
					this.nHpRecoveryTick -= 5000;
				}
			}
		}

		public void SetSoulMaxExp()
		{
			ResSoulLvlUpInfo resSoulLvlUpInfo = Singleton<BattleLogic>.instance.incomeCtrl.QuerySoulLvlUpInfo((uint)this.actorSoulLevel);
			if (resSoulLvlUpInfo == null)
			{
				return;
			}
			this.actorSoulMaxExp = (int)resSoulLvlUpInfo.dwExp;
		}

		public void AddSoulExp(int addVal, bool bFloatDigit, AddSoulType type)
		{
			if (!Singleton<BattleLogic>.instance.m_LevelContext.IsSoulGrow())
			{
				return;
			}
			this.actorSoulExp += addVal;
			while (this.actorSoulExp >= this.actorSoulMaxExp)
			{
				this.actorSoulExp -= this.actorSoulMaxExp;
				int num = this.actorSoulLevel + 1;
				int maxSoulLvl = ValueProperty.GetMaxSoulLvl();
				if (num > maxSoulLvl)
				{
					this.actorSoulLevel = maxSoulLvl;
					this.actorSoulExp = this.actorSoulMaxExp;
					break;
				}
				this.actorSoulLevel = num;
				this.ObjValueStatistic.iSoulExpMax = ((this.ObjValueStatistic.iSoulExpMax <= addVal) ? addVal : this.ObjValueStatistic.iSoulExpMax);
			}
			if (bFloatDigit && addVal > 0 && this.actor.Visible && ActorHelper.IsHostCtrlActor(ref this.actorPtr))
			{
				Singleton<CBattleSystem>.GetInstance().CreateBattleFloatDigit(addVal, DIGIT_TYPE.ReceiveSpirit, this.actor.myTransform.position);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", this.actorPtr, addVal, this.actorSoulExp, this.actorSoulMaxExp);
		}

		public void ChangeGoldCoinInBattle(int changeValue, bool isIncome, bool floatDigit = false, Vector3 position = default(Vector3), bool isLastHit = false, PoolObjHandle<ActorRoot> target = default(PoolObjHandle<ActorRoot>))
		{
			int num = this.m_goldCoinInBattle;
			this.m_goldCoinInBattle += changeValue;
			if (changeValue > 0 && isIncome)
			{
				this.m_goldCoinIncomeInBattle += changeValue;
				this.m_MaxGoldCoinIncomeInBattle = ((this.m_MaxGoldCoinIncomeInBattle <= changeValue) ? (CrypticInt32)changeValue : this.m_MaxGoldCoinIncomeInBattle);
			}
			DebugHelper.Assert(this.m_goldCoinInBattle >= 0, "Wo ri, zhe zenme keneng");
			bool flag = false;
			HeroWrapper heroWrapper = this.actorPtr.handle.ActorControl as HeroWrapper;
			if (heroWrapper != null)
			{
				PoolObjHandle<ActorRoot> callActor = heroWrapper.GetCallActor();
				if (callActor && callActor.handle.Visible && ActorHelper.IsHostCtrlActor(ref callActor))
				{
					flag = true;
				}
			}
			if (floatDigit && changeValue > 0 && isIncome && ((this.actor.Visible && ActorHelper.IsHostCtrlActor(ref this.actorPtr)) || flag))
			{
				if (position.x == 0f && position.y == 0f && position.z == 0f)
				{
					position = this.actor.myTransform.position;
				}
				Singleton<CBattleSystem>.GetInstance().CreateBattleFloatDigit(changeValue, (!isLastHit) ? DIGIT_TYPE.ReceiveGoldCoinInBattle : DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle, position);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", this.actorPtr, changeValue, isIncome, target);
		}

		public int GetGoldCoinInBattle()
		{
			return this.m_goldCoinInBattle;
		}

		public void ChangePhyAtkByPhyDefence()
		{
			ValueDataInfo valueDataInfo = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT];
			valueDataInfo.addValue -= valueDataInfo.totalAddValueByDefence;
			long num = (long)this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue * (long)valueDataInfo.convertRatioByDefence / 10000L;
			valueDataInfo.totalAddValueByDefence = (int)num;
			valueDataInfo.addValue += valueDataInfo.totalAddValueByDefence;
		}

		public void IncAttackWithExtraHP()
		{
			ValueDataInfo valueDataInfo = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT];
			valueDataInfo.addValue -= valueDataInfo.totalAddValueByExtraHP;
			ValueDataInfo valueDataInfo2 = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP];
			int num = valueDataInfo2.totalValue - valueDataInfo2.baseValue - valueDataInfo2.growValue;
			long num2 = (long)num * (long)valueDataInfo.convertRatioByExtraHP / 10000L;
			valueDataInfo.totalAddValueByExtraHP = (int)num2;
			valueDataInfo.addValue += valueDataInfo.totalAddValueByExtraHP;
		}

		public int GetGoldCoinIncomeInBattle()
		{
			return this.m_goldCoinIncomeInBattle;
		}

		public int GetMaxGoldCoinIncomeInBattle()
		{
			return this.m_MaxGoldCoinIncomeInBattle;
		}

		public static int GetMaxSoulLvl()
		{
			return Singleton<BattleLogic>.instance.incomeCtrl.GetSoulLvlUpInfoList().Count;
		}

		public VFactor GetHpRate()
		{
			DebugHelper.Assert(this.actorHpTotal > 0, " {0} GetHpRate actorHpTotal is zero", new object[]
			{
				this.actor.TheStaticData.TheResInfo.Name
			});
			if (this.actorHpTotal > 0)
			{
				return new VFactor((long)this.actorHp, (long)this.actorHpTotal);
			}
			return VFactor.one;
		}

		public void SetHpByRate(VFactor hpRate)
		{
			DebugHelper.Assert(hpRate.den != 0L, "SetHpByRate hpRate den is zero");
			if (hpRate.den != 0L)
			{
				this.actorHp = (hpRate * (long)this.actorHpTotal).roundInt;
			}
		}

		public void OnValuePropertyChangeByMgcEffect()
		{
			int totalEftRatioByMgc = this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc;
			int totalOldEftRatioByMgc = this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalOldEftRatioByMgc;
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc = totalOldEftRatioByMgc;
			VFactor a = new VFactor((long)this.actor.ValueComponent.actorHp, (long)this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftRatioByMgc = totalEftRatioByMgc;
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalEftValueByMgc = totalEftRatioByMgc * this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue / 10000;
			if (!this.actor.ActorControl.IsDeadState)
			{
				this.actor.ValueComponent.actorHp = (a * (long)this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue).roundInt;
			}
			totalEftRatioByMgc = this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalEftRatioByMgc;
			this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalEftValueByMgc = totalEftRatioByMgc * this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue / 10000;
		}
	}
}

using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillBean
	{
		private ResSkillBeanCfgInfo _cfgData;

		private SkillSlot _skillSlot;

		private int _beanAmount;

		private int _deltaTime;

		private int BeanAmount
		{
			get
			{
				return this._beanAmount;
			}
			set
			{
				if (value >= 0 && value <= this.upperLimit)
				{
					this._beanAmount = value;
					DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this._skillSlot.SlotType, this._beanAmount, this._skillSlot.Actor);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, this._skillSlot.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
				}
			}
		}

		private int upperLimit
		{
			get
			{
				if (this._cfgData.dwUpperLimitInterval > 0u)
				{
					return (int)((ulong)this._cfgData.dwUpperLimit + (ulong)((long)(this._skillSlot.GetSkillLevel() - 1) / (long)((ulong)this._cfgData.dwUpperLimitInterval) * (long)((ulong)this._cfgData.dwUpperLimitGrowth)));
				}
				return (int)this._cfgData.dwUpperLimit;
			}
		}

		private int cdTime
		{
			get
			{
				return (int)((ulong)this._cfgData.dwCDTime + (ulong)((long)(this._skillSlot.GetSkillLevel() - 1) / (long)((ulong)this._cfgData.dwCDTimeInterval) * (long)this._cfgData.iCDTimeIGrowth));
			}
		}

		public SkillBean(SkillSlot skillSlot)
		{
			this._skillSlot = skillSlot;
		}

		public void Init()
		{
			if (this._skillSlot == null || !this._skillSlot.Actor)
			{
				return;
			}
			if (this._skillSlot.Actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long)this._skillSlot.Actor.handle.TheActorMeta.ConfigId);
				int slotType = (int)this._skillSlot.SlotType;
				if (dataByKey != null && slotType >= 1 && slotType <= 3)
				{
					this._cfgData = GameDataMgr.skillBeanDatabin.GetDataByKey(dataByKey.astSkill[slotType].dwSkillBeanID);
				}
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this._cfgData == null)
			{
				return;
			}
			if (!this._skillSlot.Actor.handle.ActorControl.IsDeadState && this.BeanAmount < this.upperLimit)
			{
				if (this._deltaTime > 0)
				{
					this._deltaTime -= delta;
					this._deltaTime = ((this._deltaTime <= 0) ? 0 : this._deltaTime);
				}
				if (this._deltaTime <= 0)
				{
					this.BeanAmount++;
					this._deltaTime = this.cdTime;
				}
			}
		}

		public void BeanUse()
		{
			if (this._cfgData != null && this.BeanAmount > 0)
			{
				this.BeanAmount--;
			}
		}

		public bool IsBeanEnough()
		{
			return this._cfgData == null || this.BeanAmount > 0;
		}

		public int GetBeanAmount()
		{
			return this._beanAmount;
		}

		public bool ConsumeBean()
		{
			return this._cfgData != null;
		}
	}
}

using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveEvent
	{
		private const int MAX_EVENT_PARAM = 5;

		protected ResSkillPassiveCfgInfo cfgData;

		protected int[] localParams = new int[5];

		protected PoolObjHandle<ActorRoot> sourceActor;

		protected PoolObjHandle<ActorRoot> triggerActor = new PoolObjHandle<ActorRoot>(null);

		protected PassiveSkill passiveSkill;

		protected ListView<PassiveCondition> conditions = new ListView<PassiveCondition>();

		protected int deltaTime;

		public virtual void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			this.sourceActor = _actor;
			this.triggerActor.Release();
			this.passiveSkill = _skill;
			this.cfgData = _skill.cfgData;
			this.deltaTime = 0;
			for (int i = 0; i < this.conditions.Count; i++)
			{
				PassiveCondition passiveCondition = this.conditions[i];
				passiveCondition.Init(this.sourceActor, this, ref this.cfgData.astPassiveConditon[i]);
			}
			this.SetEventParam();
		}

		public virtual void InitCDTime(int _cdTime)
		{
			this.deltaTime = _cdTime;
		}

		public virtual int GetCDTime()
		{
			return this.deltaTime;
		}

		public virtual void UnInit()
		{
			for (int i = 0; i < this.conditions.Count; i++)
			{
				PassiveCondition passiveCondition = this.conditions[i];
				passiveCondition.UnInit();
			}
		}

		private void SetEventParam()
		{
			this.localParams[0] = this.cfgData.iPassiveEventParam1;
			this.localParams[1] = this.cfgData.iPassiveEventParam2;
			this.localParams[2] = this.cfgData.iPassiveEventParam3;
			this.localParams[3] = this.cfgData.iPassiveEventParam4;
			this.localParams[4] = this.cfgData.iPassiveEventParam5;
		}

		public bool ChangeEventParam(int _index, int _value)
		{
			if (_index < 0 && _index >= 5)
			{
				return false;
			}
			this.localParams[_index] = _value;
			return true;
		}

		public virtual void UpdateLogic(int _delta)
		{
			if (this.deltaTime > 0)
			{
				this.deltaTime -= _delta;
				this.deltaTime = ((this.deltaTime > 0) ? this.deltaTime : 0);
			}
		}

		protected void Reset()
		{
			for (int i = 0; i < this.conditions.Count; i++)
			{
				PassiveCondition passiveCondition = this.conditions[i];
				passiveCondition.Reset();
			}
		}

		protected bool Fit()
		{
			if (this.conditions.Count == 0 && this.deltaTime <= 0)
			{
				return true;
			}
			if (this.cfgData != null)
			{
				if (this.cfgData.bPassiveConditonMode == 0)
				{
					for (int i = 0; i < this.conditions.Count; i++)
					{
						PassiveCondition passiveCondition = this.conditions[i];
						if (passiveCondition.Fit() && this.deltaTime <= 0)
						{
							return true;
						}
					}
				}
				else
				{
					int num = 0;
					for (int j = 0; j < this.conditions.Count; j++)
					{
						PassiveCondition passiveCondition2 = this.conditions[j];
						if (passiveCondition2.Fit() && this.deltaTime <= 0)
						{
							num++;
						}
					}
					if (num == this.conditions.Count)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void Trigger()
		{
			SkillUseParam skillUseParam = default(SkillUseParam);
			skillUseParam.Init(this.passiveSkill.SlotType);
			skillUseParam.SetOriginator(this.sourceActor);
			if (this.passiveSkill.skillContext != null)
			{
				skillUseParam.skillUseFrom = this.passiveSkill.skillContext.skillUseFrom;
				skillUseParam.uiFromId = this.passiveSkill.skillContext.uiFromId;
			}
			if (!this.triggerActor)
			{
				skillUseParam.TargetActor = this.sourceActor;
			}
			else
			{
				skillUseParam.TargetActor = this.triggerActor;
			}
			this.passiveSkill.Use(this.sourceActor, ref skillUseParam);
			this.deltaTime = this.cfgData.iCoolDown;
		}

		public void AddCondition(PassiveCondition _condition)
		{
			this.conditions.Add(_condition);
		}

		public void SetTriggerActor(PoolObjHandle<ActorRoot> _actor)
		{
			this.triggerActor = _actor;
		}
	}
}

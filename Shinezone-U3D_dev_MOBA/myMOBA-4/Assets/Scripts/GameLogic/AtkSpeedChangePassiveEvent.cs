using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.AtkSpeedChangePassiveEvent)]
	public class AtkSpeedChangePassiveEvent : PassiveEvent
	{
		private int[] speedThreshold = new int[2];

		protected int[] skillCombineID = new int[3];

		private int lastInterval;

		private int curInterval;

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			base.Init(_actor, _skill);
			this.speedThreshold[0] = this.localParams[0];
			this.speedThreshold[1] = this.localParams[1];
			this.skillCombineID[0] = this.localParams[2];
			this.skillCombineID[1] = this.localParams[3];
			this.skillCombineID[2] = this.localParams[4];
			this.lastInterval = -1;
			this.curInterval = 0;
			this.AtkSpeedChange();
			if (_actor)
			{
				_actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD].ChangeEvent += new ValueChangeDelegate(this.AtkSpeedChange);
			}
		}

		public override void UnInit()
		{
			base.UnInit();
			if (this.sourceActor)
			{
				this.sourceActor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD].ChangeEvent -= new ValueChangeDelegate(this.AtkSpeedChange);
			}
		}

		private void SpawnSkillEffect(int _skillCombineID)
		{
			if (this.sourceActor)
			{
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.Init();
				skillUseParam.SetOriginator(this.sourceActor);
				this.sourceActor.handle.SkillControl.SpawnBuff(this.sourceActor, ref skillUseParam, _skillCombineID, true);
			}
		}

		private void RemoveSkillEffect(int _skillCombineID)
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.BuffHolderComp.RemoveBuff(_skillCombineID);
			}
		}

		public void AtkSpeedChange()
		{
			this.curInterval = 2;
			int totalValue = this.sourceActor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD].totalValue;
			for (int i = 0; i < this.speedThreshold.Length; i++)
			{
				if (totalValue < this.speedThreshold[i])
				{
					this.curInterval = i;
					break;
				}
			}
		}

		public override void UpdateLogic(int _delta)
		{
			if (this.curInterval != this.lastInterval)
			{
				if (this.lastInterval >= 0)
				{
					this.RemoveSkillEffect(this.skillCombineID[this.lastInterval]);
				}
				this.SpawnSkillEffect(this.skillCombineID[this.curInterval]);
				this.lastInterval = this.curInterval;
			}
		}
	}
}

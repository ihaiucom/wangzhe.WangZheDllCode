using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.TimePassiveEvent)]
	public class PassiveTimeEvent : PassiveEvent
	{
		private int totalTime;

		private int curTriggerCount;

		private int maxTriggerCount;

		protected int startTime;

		private bool bActive;

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			base.Init(_actor, _skill);
			this.startTime = 0;
			this.curTriggerCount = 0;
			this.totalTime = this.localParams[0];
			this.maxTriggerCount = this.localParams[1];
			this.bActive = true;
			if (this.maxTriggerCount <= 0)
			{
				this.maxTriggerCount = 2147483647;
			}
		}

		public override void UpdateLogic(int _delta)
		{
			base.UpdateLogic(_delta);
			if (!this.bActive)
			{
				return;
			}
			this.startTime += _delta;
			if (base.Fit())
			{
				if (this.startTime >= this.totalTime && this.curTriggerCount < this.maxTriggerCount)
				{
					base.Trigger();
					this.startTime -= this.totalTime;
					base.Reset();
					this.curTriggerCount++;
					if (this.curTriggerCount == this.maxTriggerCount)
					{
						this.bActive = false;
					}
				}
			}
			else
			{
				this.startTime = 0;
				base.Reset();
			}
		}
	}
}

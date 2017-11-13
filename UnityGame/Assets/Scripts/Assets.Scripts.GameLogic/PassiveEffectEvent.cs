using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.EffectPassiveEvent)]
	public class PassiveEffectEvent : PassiveEvent
	{
		private bool bTriggerFlag;

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			this.bTriggerFlag = false;
			base.Init(_actor, _skill);
		}

		private void RemoveSkillEffect()
		{
			PoolObjHandle<ActorRoot> ptr;
			if (this.triggerActor)
			{
				ptr = this.triggerActor;
			}
			else
			{
				ptr = this.sourceActor;
			}
			if (ptr)
			{
				if (this.localParams[0] != 0)
				{
					ptr.handle.BuffHolderComp.RemoveBuff(this.localParams[0]);
				}
				else if (this.localParams[1] != 0)
				{
					ptr.handle.BuffHolderComp.RemoveBuff(this.localParams[1]);
				}
			}
		}

		public override void UpdateLogic(int _delta)
		{
			base.UpdateLogic(_delta);
			if (base.Fit() && !this.sourceActor.handle.ActorControl.IsDeadState)
			{
				base.Trigger();
				this.bTriggerFlag = true;
			}
			else if (this.bTriggerFlag || this.sourceActor.handle.ActorControl.IsDeadState)
			{
				this.bTriggerFlag = false;
				this.RemoveSkillEffect();
			}
		}
	}
}

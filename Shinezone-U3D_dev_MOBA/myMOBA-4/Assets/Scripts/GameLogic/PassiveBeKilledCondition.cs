using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.BeKilledPassiveCondition)]
	public class PassiveBeKilledCondition : PassiveCondition
	{
		private bool bKilled;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bKilled = false;
			base.Init(_source, _event, ref _config);
			this.sourceActor.handle.ActorControl.eventActorDead += new ActorDeadEventHandler(this.onActorDead);
		}

		public override void UnInit()
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.ActorControl.eventActorDead -= new ActorDeadEventHandler(this.onActorDead);
			}
		}

		public override void Reset()
		{
			this.bKilled = false;
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src != this.sourceActor || (this.localParams[1] == 0 && prm.bSuicide) || (this.localParams[0] == 0 && prm.bImmediateRevive) || (this.localParams[0] == 1 && !prm.bIsPassiveSkillRevive))
			{
				return;
			}
			this.bKilled = true;
			this.rootEvent.SetTriggerActor(prm.logicAtker);
		}

		public override bool Fit()
		{
			return this.bKilled;
		}
	}
}

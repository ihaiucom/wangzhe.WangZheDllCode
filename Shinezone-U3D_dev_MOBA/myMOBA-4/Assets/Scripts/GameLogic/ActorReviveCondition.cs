using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.ActorReviveCondition)]
	public class ActorReviveCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			this.sourceActor.handle.ActorControl.eventActorRevive += new ActorEventHandler(this.onActorRevive);
		}

		public override void UnInit()
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.ActorControl.eventActorRevive -= new ActorEventHandler(this.onActorRevive);
			}
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onActorRevive(ref DefaultGameEventParam _prm)
		{
			if (_prm.src != this.sourceActor || (this.localParams[0] == 1 && this.sourceActor.handle.ActorControl.IsEnableReviveContext()))
			{
				return;
			}
			this.bTrigger = true;
		}

		public override bool Fit()
		{
			return this.bTrigger;
		}
	}
}

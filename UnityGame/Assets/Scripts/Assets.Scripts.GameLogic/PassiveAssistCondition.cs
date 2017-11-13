using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.AssistPassiveCondition)]
	public class PassiveAssistCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			this.sourceActor.handle.ActorControl.eventActorAssist += new ActorEventHandler(this.onActorAssist);
		}

		public override void UnInit()
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.ActorControl.eventActorAssist -= new ActorEventHandler(this.onActorAssist);
			}
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onActorAssist(ref DefaultGameEventParam prm)
		{
			if (prm.orignalAtker != this.sourceActor)
			{
				return;
			}
			if (base.CheckTargetSubType(prm.src, this.localParams[0], this.localParams[1]))
			{
				this.bTrigger = true;
				this.rootEvent.SetTriggerActor(prm.src);
			}
		}

		public override bool Fit()
		{
			return this.bTrigger;
		}
	}
}

using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.ExitBattlePassiveCondition)]
	public class ExitBattleCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			this.sourceActor.handle.ActorControl.eventActorEnterCombat += new ActorEventHandler(this.onActorEnterBattle);
			this.sourceActor.handle.ActorControl.eventActorExitCombat += new ActorEventHandler(this.onActorExitBattle);
			this.bTrigger = !this.sourceActor.handle.ActorAgent.m_wrapper.IsInBattle;
		}

		public override void UnInit()
		{
			if (this.sourceActor)
			{
				this.sourceActor.handle.ActorControl.eventActorEnterCombat -= new ActorEventHandler(this.onActorEnterBattle);
				this.sourceActor.handle.ActorControl.eventActorExitCombat -= new ActorEventHandler(this.onActorExitBattle);
			}
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onActorEnterBattle(ref DefaultGameEventParam _prm)
		{
			if (_prm.src != this.sourceActor)
			{
				return;
			}
			this.bTrigger = false;
		}

		private void onActorExitBattle(ref DefaultGameEventParam _prm)
		{
			if (_prm.src != this.sourceActor)
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

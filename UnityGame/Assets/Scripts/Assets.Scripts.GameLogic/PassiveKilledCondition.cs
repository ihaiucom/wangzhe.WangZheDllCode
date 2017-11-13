using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.KilledPassiveCondition)]
	public class PassiveKilledCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		public override void UnInit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.logicAtker != this.sourceActor || prm.bImmediateRevive)
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

using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.ImmuneDeadHurtCondition)]
	public class ImmuneDeadHurtCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmuneDeadHurt, new RefAction<DefaultGameEventParam>(this.onImmuneDeadHurt));
		}

		public override void UnInit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmuneDeadHurt, new RefAction<DefaultGameEventParam>(this.onImmuneDeadHurt));
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onImmuneDeadHurt(ref DefaultGameEventParam _prm)
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

using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.LimitMoveCondition)]
	public class LimitMoveCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			Singleton<GameSkillEventSys>.instance.AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorLimitMove));
			Singleton<GameSkillEventSys>.instance.AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorCancelLimitMove));
		}

		public override void UnInit()
		{
			Singleton<GameSkillEventSys>.instance.RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorLimitMove));
			Singleton<GameSkillEventSys>.instance.RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorCancelLimitMove));
		}

		private void onActorLimitMove(ref LimitMoveEventParam _param)
		{
			if (_param.src != this.sourceActor)
			{
				return;
			}
			this.bTrigger = true;
		}

		private void onActorCancelLimitMove(ref LimitMoveEventParam _param)
		{
			if (_param.src != this.sourceActor)
			{
				return;
			}
			this.bTrigger = false;
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		public override bool Fit()
		{
			return this.bTrigger;
		}
	}
}

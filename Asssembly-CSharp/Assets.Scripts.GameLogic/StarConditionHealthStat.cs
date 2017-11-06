using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(2)]
	internal class StarConditionHealthStat : StarCondition
	{
		private bool bHasFailure;

		private int LoweastHealthPercent = 100;

		private ActorTypeDef targetActorType
		{
			get
			{
				return (ActorTypeDef)base.ConditionInfo.KeyDetail[1];
			}
		}

		private int healthPercent
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		private int loweastHealthPercent
		{
			get
			{
				return this.LoweastHealthPercent;
			}
		}

		public override StarEvaluationStatus status
		{
			get
			{
				return this.bHasFailure ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.loweastHealthPercent
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
		}

		public override void Dispose()
		{
			base.Dispose();
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
		}

		private void OnActorDamage(ref HurtEventResultInfo info)
		{
			if (!this.bHasFailure && ActorHelper.IsHostActor(ref info.src))
			{
				int actorHp = info.src.handle.ValueComponent.actorHp;
				int actorHpTotal = info.src.handle.ValueComponent.actorHpTotal;
				int num = actorHp * 100 / actorHpTotal;
				if (num < this.LoweastHealthPercent)
				{
					this.LoweastHealthPercent = num;
				}
				bool flag = !SmartCompare.Compare<int>(num, this.healthPercent, this.operation);
				if (this.bHasFailure != flag)
				{
					this.bHasFailure = flag;
					this.TriggerChangedEvent();
				}
			}
		}
	}
}

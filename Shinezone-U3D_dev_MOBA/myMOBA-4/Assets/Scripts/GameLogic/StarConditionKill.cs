using System;

namespace Assets.Scripts.GameLogic
{
	[StarCondition(1)]
	internal class StarConditionKill : StarConditionKillBase
	{
		protected override ActorTypeDef targetType
		{
			get
			{
				return (ActorTypeDef)base.ConditionInfo.KeyDetail[0];
			}
		}

		protected override int targetID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[1];
			}
		}

		protected override bool isSelfCamp
		{
			get
			{
				return base.ConditionInfo.KeyDetail[2] == 0;
			}
		}

		protected override int targetCount
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		protected override void OnStatChanged()
		{
			this.TriggerChangedEvent();
		}
	}
}

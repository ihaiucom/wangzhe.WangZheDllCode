using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.NoDamagePassiveCondition)]
	public class PassiveNoHurtCondition : PassiveCondition
	{
		private bool bNoHurt = true;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bNoHurt = true;
			base.Init(_source, _event, ref _config);
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
		}

		public override void UnInit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
		}

		public override void Reset()
		{
			this.bNoHurt = true;
		}

		private void onActorDamage(ref HurtEventResultInfo info)
		{
			if (info.src != this.sourceActor)
			{
				return;
			}
			if (info.hpChanged < 0)
			{
				this.bNoHurt = false;
			}
		}

		public override bool Fit()
		{
			return !this.sourceActor.handle.ActorControl.IsDeadState && this.bNoHurt;
		}
	}
}

using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.ActorUpgradeCondition)]
	public class ActorUpgradeCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = false;
			base.Init(_source, _event, ref _config);
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onActorUpgrade));
		}

		public override void UnInit()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onActorUpgrade));
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void onActorUpgrade(PoolObjHandle<ActorRoot> hero, int level)
		{
			if (hero != this.sourceActor)
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

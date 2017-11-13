using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.BloodResource)]
	public class Blood : BaseEnergyLogic
	{
		public override int _actorEp
		{
			get
			{
				return this.actor.handle.ValueComponent.actorHp;
			}
			protected set
			{
				this.actor.handle.ValueComponent.actorHp = value;
			}
		}

		public override int actorEpTotal
		{
			get
			{
				return this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			}
		}

		public override int actorEpRecTotal
		{
			get
			{
				return 0;
			}
		}

		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.BloodResource;
			base.Init(_actor);
		}

		public override void UpdateLogic(int _delta)
		{
		}

		public override void ResetEpValue(int epPercent)
		{
		}
	}
}

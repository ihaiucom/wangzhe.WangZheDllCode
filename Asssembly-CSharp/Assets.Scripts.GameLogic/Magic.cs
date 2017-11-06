using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.MagicResource)]
	public class Magic : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.MagicResource;
			base.Init(_actor);
		}

		public override void ResetEpValue(int epPercent)
		{
			this._actorEp = this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue * epPercent / 10000;
		}

		protected override void UpdateEpValue()
		{
			this._actorEp += this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue / 5;
		}
	}
}

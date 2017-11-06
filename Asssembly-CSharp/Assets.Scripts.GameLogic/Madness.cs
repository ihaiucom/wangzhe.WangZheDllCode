using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.MadnessResource)]
	public class Madness : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.MadnessResource;
			base.Init(_actor);
		}

		protected override void UpdateEpValue()
		{
			this._actorEp += this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue / 5;
		}
	}
}

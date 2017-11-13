using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.NoneResource)]
	public class NoEnergy : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.NoneResource;
			base.Init(_actor);
		}

		public override void UpdateLogic(int _delta)
		{
		}
	}
}

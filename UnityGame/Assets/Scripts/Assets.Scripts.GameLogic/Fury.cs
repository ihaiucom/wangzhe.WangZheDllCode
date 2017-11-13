using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.FuryResource)]
	public class Fury : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.FuryResource;
			base.Init(_actor);
		}

		protected override void UpdateEpValue()
		{
			this._actorEp += this.cfgData.iRecAmount;
		}

		public override void ResetEpValue(int epPercent)
		{
			this._actorEp = 0;
		}

		protected override bool Fit()
		{
			return base.Fit() && !this.actor.handle.ActorControl.IsInBattle;
		}
	}
}

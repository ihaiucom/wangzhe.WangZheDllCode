using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.SpeedResource)]
	public class Speed : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.SpeedResource;
			base.Init(_actor);
		}

		public override void UpdateLogic(int _delta)
		{
		}

		public override void ResetEpValue(int epPercent)
		{
			this._actorEp = 0;
		}
	}
}

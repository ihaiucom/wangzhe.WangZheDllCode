using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	[Energy(EnergyType.EnergyResource)]
	public class Energy : BaseEnergyLogic
	{
		public override void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.energyType = EnergyType.EnergyResource;
			base.Init(_actor);
		}

		protected override void UpdateEpValue()
		{
			this._actorEp += this.cfgData.iRecAmount;
		}
	}
}

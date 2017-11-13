using System;

namespace Assets.Scripts.GameLogic
{
	public struct SpawnSoldierParam
	{
		public SoldierRegion SourceRegion;

		public SpawnSoldierParam(SoldierRegion sourceRegion)
		{
			this.SourceRegion = sourceRegion;
		}
	}
}

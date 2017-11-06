using System;

namespace Assets.Scripts.GameLogic
{
	public struct NextSpawnGroupsParam
	{
		public SpawnGroup OldGroup;

		public SpawnGroup[] NextGroups;

		public NextSpawnGroupsParam(SpawnGroup inOldGroup, SpawnGroup[] inNextGroups)
		{
			this.OldGroup = inOldGroup;
			this.NextGroups = inNextGroups;
		}
	}
}

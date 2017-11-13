using System;

namespace Assets.Scripts.GameLogic
{
	public struct SCommonSpawnEventParam
	{
		public VInt3 SpawnPos;

		public int LeftTime;

		public int AlertPreroll;

		public SpawnerWrapper.ESpawnObjectType SpawnObjType;

		public SCommonSpawnEventParam(VInt3 inPos, int inLeftTime, int inAlertPreroll, SpawnerWrapper.ESpawnObjectType inObjType)
		{
			this.SpawnPos = inPos;
			this.LeftTime = inLeftTime;
			this.AlertPreroll = inAlertPreroll;
			this.SpawnObjType = inObjType;
		}
	}
}

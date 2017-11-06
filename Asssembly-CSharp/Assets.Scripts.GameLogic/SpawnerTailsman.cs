using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SpawnerTailsman : SpawnerBase
	{
		public int TailsmanId;

		public STriggerCondActor[] SrcActorCond;

		public SpawnerTailsman(SpawnerWrapper inWrapper) : base(inWrapper)
		{
		}

		public override object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint)
		{
			CTailsman cTailsman = ClassObjPool<CTailsman>.Get();
			cTailsman.Init(this.TailsmanId, inWorldPos, this.SrcActorCond);
			return cTailsman;
		}
	}
}

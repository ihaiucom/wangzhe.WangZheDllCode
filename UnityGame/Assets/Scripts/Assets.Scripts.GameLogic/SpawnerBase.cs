using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public abstract class SpawnerBase
	{
		public SpawnerBase(SpawnerWrapper inWrapper)
		{
		}

		public virtual void Destroy()
		{
		}

		public abstract object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint);
	}
}

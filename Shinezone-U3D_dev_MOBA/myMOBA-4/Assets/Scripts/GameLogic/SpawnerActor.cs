using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.DataCenter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SpawnerActor : SpawnerBase
	{
		public ActorMeta TheActorMeta;

		public bool bSequentialMeta;

		public int InitRandPassSkillRule;

		public int[] InitBuffDemand;

		public GeoPolygon m_rangePolygon;

		public GameObject m_rangeDeadPoint;

		public SpawnerActor(SpawnerWrapper inWrapper) : base(inWrapper)
		{
		}

		public override object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint)
		{
			List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
			ActorMeta[] inActorMetaList = new ActorMeta[]
			{
				this.TheActorMeta
			};
			SpawnPoint.DoSpawn(inSpawnPoint, inWorldPos, inDir, this.bSequentialMeta, inActorMetaList, this.m_rangePolygon, this.m_rangeDeadPoint, null, this.InitBuffDemand, this.InitRandPassSkillRule, ref list);
			if (list.Count > 0 && list[0])
			{
				return list[0].handle;
			}
			return null;
		}
	}
}

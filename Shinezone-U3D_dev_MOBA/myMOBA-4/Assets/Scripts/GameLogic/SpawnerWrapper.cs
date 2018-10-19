using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class SpawnerWrapper
	{
		public enum ESpawnObjectType
		{
			Tailsman,
			Actor,
			Invalid
		}

		public SpawnerWrapper.ESpawnObjectType SpawnType;

		public STriggerCondActor[] SrcActorCond;

		[FriendlyName("生成对象配置ID")]
		public int ConfigId;

		public ActorMeta TheActorMeta;

		[FriendlyName("Meta信息非随机")]
		public bool bSequentialMeta;

		[FriendlyName("随机被动技能规则")]
		public int InitRandPassSkillRule;

		public int[] InitBuffDemand = new int[0];

		public GeoPolygon m_rangePolygon;

		public GameObject m_rangeDeadPoint;

		private SpawnerBase m_internalSpawner;

		public SpawnerWrapper()
		{
		}

		public SpawnerWrapper(SpawnerWrapper.ESpawnObjectType inSpawnType)
		{
			this.SpawnType = inSpawnType;
		}

		public void Init()
		{
			if (this.m_internalSpawner != null)
			{
				return;
			}
			SpawnerWrapper.ESpawnObjectType spawnType = this.SpawnType;
			if (spawnType != SpawnerWrapper.ESpawnObjectType.Tailsman)
			{
				if (spawnType == SpawnerWrapper.ESpawnObjectType.Actor)
				{
					this.m_internalSpawner = new SpawnerActor(this)
					{
						TheActorMeta = this.TheActorMeta,
						bSequentialMeta = this.bSequentialMeta,
						InitRandPassSkillRule = this.InitRandPassSkillRule,
						InitBuffDemand = this.InitBuffDemand,
						m_rangePolygon = this.m_rangePolygon,
						m_rangeDeadPoint = this.m_rangeDeadPoint
					};
				}
			}
			else
			{
				this.m_internalSpawner = new SpawnerTailsman(this)
				{
					TailsmanId = this.ConfigId,
					SrcActorCond = this.SrcActorCond
				};
			}
		}

		public void Destroy()
		{
			if (this.m_internalSpawner != null)
			{
				this.m_internalSpawner.Destroy();
				this.m_internalSpawner = null;
			}
		}

		public SpawnerBase GetActionInternal()
		{
			return this.m_internalSpawner;
		}

		public object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint)
		{
			if (this.m_internalSpawner != null)
			{
				return this.m_internalSpawner.DoSpawn(inWorldPos, inDir, inSpawnPoint);
			}
			return null;
		}

		public void PreLoadResource(ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
		{
			if (this.SpawnType == SpawnerWrapper.ESpawnObjectType.Tailsman)
			{
				CharmLib dataByKey = GameDataMgr.charmLib.GetDataByKey((long)this.ConfigId);
				if (dataByKey != null)
				{
					for (int i = 0; i < 10; i++)
					{
						if (dataByKey.astCharmId[i].iParam == 0)
						{
							break;
						}
						int iParam = dataByKey.astCharmId[i].iParam;
						ShenFuInfo dataByKey2 = GameDataMgr.shenfuBin.GetDataByKey((long)iParam);
						if (dataByKey2 != null)
						{
							AssetLoadBase item = new AssetLoadBase
							{
								assetPath = StringHelper.UTF8BytesToString(ref dataByKey2.szShenFuResPath)
							};
							loadInfo.mesPrefabs.Add(item);
							loadHelper.AnalyseSkillCombine(ref loadInfo, dataByKey2.iBufId);
						}
					}
				}
			}
		}

		public void PreLoadResource(ref List<ActorPreloadTab> list, LoaderHelper loadHelper)
		{
			if (this.SpawnType == SpawnerWrapper.ESpawnObjectType.Actor && this.TheActorMeta.ConfigId > 0)
			{
				loadHelper.AddPreloadActor(ref list, ref this.TheActorMeta, 1f, 0);
			}
		}
	}
}

using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SpawnPoint : FuncRegion
	{
		public ActorMeta[] TheActorsMeta = new ActorMeta[1];

		public SpawnPoint NextPoint;

		protected List<PoolObjHandle<ActorRoot>> m_spawnedList = new List<PoolObjHandle<ActorRoot>>();

		protected List<SpawnPoint> m_spawnPointList = new List<SpawnPoint>();

		protected int m_spawnPointOver;

		[HideInInspector]
		public Color PointColor = new Color(1f, 0f, 0f);

		[HideInInspector]
		public float radius = 0.25f;

		[FriendlyName("Meta信息非随机")]
		public bool bSequentialMeta;

		[FriendlyName("随机被动技能规则")]
		public int InitRandPassSkillRule;

		public int[] InitBuffDemand = new int[0];

		public GeoPolygon m_rangePolygon;

		public GameObject m_rangeDeadPoint;

		public event SpawnPointAllDeadEvent onAllDeadEvt
		{
			[MethodImpl(32)]
			add
			{
				this.onAllDeadEvt = (SpawnPointAllDeadEvent)Delegate.Combine(this.onAllDeadEvt, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onAllDeadEvt = (SpawnPointAllDeadEvent)Delegate.Remove(this.onAllDeadEvt, value);
			}
		}

		public event OnAllSpawned onAllSpawned
		{
			[MethodImpl(32)]
			add
			{
				this.onAllSpawned = (OnAllSpawned)Delegate.Combine(this.onAllSpawned, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onAllSpawned = (OnAllSpawned)Delegate.Remove(this.onAllSpawned, value);
			}
		}

		public void PreLoadResource(ref List<ActorPreloadTab> list, LoaderHelper loadHelper)
		{
			if (this.TheActorsMeta.Length > 0)
			{
				for (int i = 0; i < this.TheActorsMeta.Length; i++)
				{
					ActorMeta actorMeta = this.TheActorsMeta[i];
					if (actorMeta.ConfigId > 0)
					{
						loadHelper.AddPreloadActor(ref list, ref actorMeta, 1f, 0);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = this.PointColor;
			Gizmos.DrawSphere(base.transform.position, 0.15f);
		}

		protected virtual void Start()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		protected void OnDestroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		public override void UpdateLogic(int delta)
		{
		}

		public List<PoolObjHandle<ActorRoot>> GetSpawnedList()
		{
			return this.m_spawnedList;
		}

		protected void onActorDead(ref GameDeadEventParam prm)
		{
			int count = this.m_spawnedList.get_Count();
			int i = 0;
			while (i < this.m_spawnedList.get_Count())
			{
				if (!this.m_spawnedList.get_Item(i))
				{
					this.m_spawnedList.RemoveAt(i);
				}
				else
				{
					if (this.m_spawnedList.get_Item(i).Equals(prm.src))
					{
						this.m_spawnedList.RemoveAt(i);
						break;
					}
					i++;
				}
			}
			int count2 = this.m_spawnedList.get_Count();
			if (count2 == 0 && count2 < count)
			{
				this.onMyselfAllDead();
				prm.spawnPoint = this;
				Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, ref prm);
			}
		}

		protected void onSpawnPointAllDead(SpawnPoint inSpawnPoint)
		{
			if (this.m_spawnPointList.Contains(inSpawnPoint))
			{
				this.DecSpawnPointOver();
			}
		}

		private void onMyselfAllDead()
		{
			this.DecSpawnPointOver();
		}

		protected virtual void DecSpawnPointOver()
		{
			if (--this.m_spawnPointOver == 0 && this.onAllDeadEvt != null)
			{
				this.onAllDeadEvt(this);
			}
		}

		public static void DoSpawn(GameObject inGameObj, VInt3 bornPos, VInt3 bornDir, bool bInSeqMeta, ActorMeta[] inActorMetaList, GeoPolygon inPolygon, GameObject inDeadPoint, PursuitInfo pursuitInfo, int[] inBuffDemand, int inRandPassSkillRule, ref List<PoolObjHandle<ActorRoot>> inSpawnedList)
		{
			if (inGameObj == null || inActorMetaList == null || inActorMetaList.Length == 0 || inSpawnedList == null)
			{
				return;
			}
			List<ActorMeta> list = new List<ActorMeta>();
			if (bInSeqMeta)
			{
				list.AddRange(inActorMetaList);
			}
			else if (inActorMetaList.Length > 0)
			{
				int nMax = inActorMetaList.Length;
				int num = (int)FrameRandom.Random((uint)nMax);
				list.Add(inActorMetaList[num]);
			}
			List<ActorMeta>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ActorMeta current = enumerator.get_Current();
				if (current.ConfigId > 0 && current.ActorType != ActorTypeDef.Invalid)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
					if (!Singleton<GameObjMgr>.GetInstance().TryGetFromCache(ref poolObjHandle, ref current))
					{
						poolObjHandle = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref current, bornPos, bornDir, false, true);
						if (poolObjHandle)
						{
							poolObjHandle.handle.InitActor();
							poolObjHandle.handle.PrepareFight();
							Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
							poolObjHandle.handle.StartFight();
						}
					}
					else
					{
						poolObjHandle.handle.ReactiveActor(bornPos, bornDir);
					}
					if (poolObjHandle)
					{
						poolObjHandle.handle.ObjLinker.Invincible = current.Invincible;
						poolObjHandle.handle.ObjLinker.CanMovable = !current.NotMovable;
						poolObjHandle.handle.BornPos = bornPos;
						if (inPolygon != null && inDeadPoint != null)
						{
							poolObjHandle.handle.ActorControl.m_rangePolygon = inPolygon;
							poolObjHandle.handle.ActorControl.m_deadPointGo = inDeadPoint;
						}
						if (pursuitInfo != null && pursuitInfo.IsVaild())
						{
							MonsterWrapper monsterWrapper = poolObjHandle.handle.ActorControl as MonsterWrapper;
							if (monsterWrapper != null)
							{
								monsterWrapper.Pursuit = pursuitInfo;
							}
						}
						inSpawnedList.Add(poolObjHandle);
						if (inBuffDemand != null)
						{
							for (int i = 0; i < inBuffDemand.Length; i++)
							{
								int inBuffID = inBuffDemand[i];
								BufConsumer bufConsumer = new BufConsumer(inBuffID, poolObjHandle, poolObjHandle);
								bufConsumer.Use();
							}
						}
						if (inRandPassSkillRule > 0 && poolObjHandle.handle.SkillControl != null)
						{
							poolObjHandle.handle.SkillControl.InitRandomSkill(inRandPassSkillRule);
						}
					}
				}
			}
		}

		public void DoSpawn(PursuitInfo pursuitInfo)
		{
			SpawnPoint.DoSpawn(base.gameObject, (VInt3)base.gameObject.transform.position, (VInt3)base.gameObject.transform.forward, this.bSequentialMeta, this.TheActorsMeta, this.m_rangePolygon, this.m_rangeDeadPoint, pursuitInfo, this.InitBuffDemand, this.InitRandPassSkillRule, ref this.m_spawnedList);
			IEnumerator enumerator = this.m_spawnPointList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SpawnPoint spawnPoint = enumerator.get_Current() as SpawnPoint;
				if (spawnPoint)
				{
					spawnPoint.DoSpawn(pursuitInfo);
				}
			}
			this.m_spawnPointOver = this.m_spawnPointList.get_Count() + 1;
			if (this.onAllSpawned != null)
			{
				this.onAllSpawned(this);
			}
		}
	}
}

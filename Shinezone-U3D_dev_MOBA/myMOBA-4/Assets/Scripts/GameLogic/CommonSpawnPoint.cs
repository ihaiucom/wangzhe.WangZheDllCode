using Assets.Scripts.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class CommonSpawnPoint : FuncRegion
	{
		public enum ESpawnStyle
		{
			AllPoints,
			RandomSubPoint,
			Self
		}

		public CommonSpawnPoint NextPoint;

		protected ArrayList m_spawnedList = new ArrayList();

		protected ArrayList m_spawnPointList = new ArrayList();

		protected int m_spawnPointOver;

		[HideInInspector]
		public Color PointColor = new Color(1f, 1f, 1f);

		[HideInInspector]
		public float radius = 0.25f;

		[SerializeField]
		public SpawnerWrapper[] SpawnerList = new SpawnerWrapper[0];

		[FriendlyName("生成位置随机")]
		public bool RandomPos;

		[FriendlyName("随机位置内半径")]
		public int RandomPosRadiusInner;

		[FriendlyName("随机位置外半径")]
		public int RandomPosRadiusOuter;

		public CommonSpawnPoint.ESpawnStyle SpawnStyle;

		public event CommonSpawnPointAllDead onAllDeadEvt;

		public event OnCommonPointAllSpawned onAllSpawned;

		public void PreLoadResource(ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
		{
			SpawnerWrapper[] spawnerList = this.SpawnerList;
			for (int i = 0; i < spawnerList.Length; i++)
			{
				SpawnerWrapper spawnerWrapper = spawnerList[i];
				if (spawnerWrapper != null)
				{
					spawnerWrapper.PreLoadResource(ref loadInfo, loadHelper);
				}
			}
		}

		public void PreLoadResource(ref List<ActorPreloadTab> list, LoaderHelper loadHelper)
		{
			SpawnerWrapper[] spawnerList = this.SpawnerList;
			for (int i = 0; i < spawnerList.Length; i++)
			{
				SpawnerWrapper spawnerWrapper = spawnerList[i];
				if (spawnerWrapper != null)
				{
					spawnerWrapper.PreLoadResource(ref list, loadHelper);
				}
			}
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.color = this.PointColor;
			Gizmos.DrawSphere(base.transform.position, 0.15f);
		}

		private void Awake()
		{
			SpawnerWrapper[] spawnerList = this.SpawnerList;
			for (int i = 0; i < spawnerList.Length; i++)
			{
				SpawnerWrapper spawnerWrapper = spawnerList[i];
				if (spawnerWrapper != null)
				{
					spawnerWrapper.Init();
				}
			}
		}

		protected virtual void Start()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.onTailsmanUsed));
		}

		protected void OnDestroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.onTailsmanUsed));
		}

		public override void UpdateLogic(int delta)
		{
		}

		public ArrayList GetSpawnedList()
		{
			return this.m_spawnedList;
		}

		protected virtual void onActorDead(ref GameDeadEventParam prm)
		{
			int count = this.m_spawnedList.Count;
			int i = 0;
			while (i < this.m_spawnedList.Count)
			{
				if (this.m_spawnedList[i] == null)
				{
					this.m_spawnedList.RemoveAt(i);
				}
				else
				{
					if (this.m_spawnedList[i].Equals(prm.src.handle))
					{
						this.m_spawnedList.RemoveAt(i);
						break;
					}
					i++;
				}
			}
			int count2 = this.m_spawnedList.Count;
			if (count2 == 0 && count2 < count)
			{
				this.onMyselfAllDead();
			}
		}

		private void onTailsmanUsed(ref STailsmanEventParam prm)
		{
			int count = this.m_spawnedList.Count;
			this.m_spawnedList.Remove(prm.tailsman.handle);
			int count2 = this.m_spawnedList.Count;
			if (count2 == 0 && count2 < count)
			{
				this.onMyselfAllDead();
			}
		}

		protected void onSpawnPointAllDead(CommonSpawnPoint inSpawnPoint)
		{
			if (this.m_spawnPointList.Contains(inSpawnPoint))
			{
				this.DecSpawnPointOver();
			}
		}

		protected void onMyselfAllDead()
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

		protected void CalcPosDir(out VInt3 outPos, out VInt3 outDir)
		{
			outPos = (VInt3)base.gameObject.transform.position;
			outDir = (VInt3)base.gameObject.transform.forward;
			if (this.RandomPos)
			{
				int y = 0;
				int num = (int)FrameRandom.Random(2001u);
				if (num > 1000)
				{
					num -= 1000;
					num = -num;
				}
				int num2 = (int)FrameRandom.Random(2001u);
				if (num2 > 1000)
				{
					num2 -= 1000;
					num2 = -num2;
				}
				VInt3 ob = new VInt3(num, y, num2);
				Vector3 a = (Vector3)ob;
				a.Normalize();
				int num3 = 0;
				if (this.RandomPosRadiusOuter - this.RandomPosRadiusInner > 0)
				{
					num3 = (int)FrameRandom.Random((uint)(this.RandomPosRadiusOuter - this.RandomPosRadiusInner)) + this.RandomPosRadiusInner;
				}
				float d = (float)num3 * 0.001f;
				Vector3 ob2 = (Vector3)outPos + a * d;
				outPos = (VInt3)ob2;
				outDir = ob.NormalizeTo(1000);
			}
		}

		protected virtual void DoSpawnSelf()
		{
			VInt3 zero = VInt3.zero;
			VInt3 forward = VInt3.forward;
			this.CalcPosDir(out zero, out forward);
			SpawnerWrapper[] spawnerList = this.SpawnerList;
			for (int i = 0; i < spawnerList.Length; i++)
			{
				SpawnerWrapper spawnerWrapper = spawnerList[i];
				if (spawnerWrapper != null)
				{
					this.m_spawnedList.Add(spawnerWrapper.DoSpawn(zero, forward, base.gameObject));
				}
			}
		}

		private void DoSpawnSubPoint(CommonSpawnPoint subsp)
		{
			if (subsp != null)
			{
				subsp.DoSpawn();
			}
		}

		private void DoSpawnSubPointsAll()
		{
			IEnumerator enumerator = this.m_spawnPointList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CommonSpawnPoint subsp = enumerator.Current as CommonSpawnPoint;
				this.DoSpawnSubPoint(subsp);
			}
		}

		public virtual void DoSpawn()
		{
			switch (this.SpawnStyle)
			{
			case CommonSpawnPoint.ESpawnStyle.AllPoints:
				this.DoSpawnSelf();
				this.DoSpawnSubPointsAll();
				this.m_spawnPointOver += this.m_spawnPointList.Count + 1;
				break;
			case CommonSpawnPoint.ESpawnStyle.RandomSubPoint:
			{
				int num = (int)FrameRandom.Random((uint)(this.m_spawnPointList.Count + 1));
				if (num == 0)
				{
					this.DoSpawnSelf();
				}
				else
				{
					this.DoSpawnSubPoint(this.m_spawnPointList[num - 1] as CommonSpawnPoint);
				}
				this.m_spawnPointOver++;
				break;
			}
			case CommonSpawnPoint.ESpawnStyle.Self:
				this.DoSpawnSelf();
				this.m_spawnPointOver++;
				break;
			}
			if (this.onAllSpawned != null)
			{
				this.onAllSpawned(this);
			}
		}
	}
}

using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SpawnGroup : SpawnPoint
	{
		[HideInInspector]
		public int GroupId;

		[FriendlyName("需要触发器")]
		public bool bTriggerSpawn;

		[FriendlyName("第一次生成延迟")]
		public int StartUpDelay = 5000;

		[FriendlyName("生成间隔")]
		public int SpawnInternval = 10000;

		[FriendlyName("生成次数")]
		public int SpawnTimes;

		private int m_spawnTimer;

		private int m_spawnCounter;

		private bool m_bCountingSpawn;

		private Color GroupColor = new Color(0.8f, 0.1f, 0.1f, 0.8f);

		private SpawnPoint[] drawPoints;

		public SpawnGroup[] NextGroups = new SpawnGroup[0];

		[FriendlyName("失效时强制怪物死亡")]
		public bool KillCurrentWhenSwitch;

		[FriendlyName("失效时间")]
		public int InvalidTime;

		private int m_invalidTimer;

		[HideInInspector, SerializeField]
		public PursuitInfo Pursuit = new PursuitInfo();

		protected override void Start()
		{
			SpawnPoint nextPoint = this.NextPoint;
			while (nextPoint)
			{
				this.m_spawnPointList.Add(nextPoint);
				nextPoint.onAllDeadEvt += new SpawnPointAllDeadEvent(base.onSpawnPointAllDead);
				nextPoint = nextPoint.NextPoint;
			}
			base.Start();
		}

		private SpawnPoint[] FindChildrenPoints()
		{
			return base.GetComponentsInChildren<SpawnPoint>();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = this.GroupColor;
			Gizmos.DrawSphere(base.transform.position, 0.3f);
			this.drawPoints = this.FindChildrenPoints();
			if (this.drawPoints != null && this.drawPoints.Length > 0)
			{
				Gizmos.color = this.GroupColor;
				for (int i = 0; i < this.drawPoints.Length - 1; i++)
				{
					Vector3 vector = this.drawPoints[0].gameObject.transform.position;
					Vector3 vector2 = this.drawPoints[i + 1].gameObject.transform.position;
					Vector3 normalized = (vector2 - vector).normalized;
					float d = Vector3.Distance(vector2, vector) - this.drawPoints[i + 1].radius - this.drawPoints[0].radius;
					vector += normalized * this.drawPoints[0].radius;
					vector2 = vector + normalized * d;
					Gizmos.DrawLine(vector, vector2);
					this.drawPoints[i + 1].PointColor = this.GroupColor;
				}
				Gizmos.DrawIcon(new Vector3(this.drawPoints[0].transform.position.x, this.drawPoints[0].transform.position.y + this.drawPoints[0].radius * 3f, this.drawPoints[0].transform.position.z), "EndPoint", true);
			}
		}

		public override void UpdateLogic(int delta)
		{
			if (!this.isStartup)
			{
				return;
			}
			if (this.InvalidTime > 0)
			{
				this.m_invalidTimer += delta;
				if (this.m_invalidTimer >= this.InvalidTime)
				{
					this.GoNextGroups();
					return;
				}
			}
			if (!this.m_bCountingSpawn)
			{
				return;
			}
			if (this.SpawnTimes > 0 && this.m_spawnCounter <= 0)
			{
				return;
			}
			this.m_spawnTimer -= delta;
			if (this.m_spawnTimer <= 0)
			{
				this.m_spawnTimer = this.SpawnInternval;
				base.DoSpawn(this.Pursuit);
				this.m_bCountingSpawn = false;
				this.m_spawnCounter--;
			}
		}

		private void GoNextGroups()
		{
			base.Stop();
			if (this.KillCurrentWhenSwitch)
			{
				while (this.m_spawnedList.get_Count() > 0)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = this.m_spawnedList.get_Item(0);
					if (poolObjHandle)
					{
						poolObjHandle.handle.ActorControl.bSuicide = true;
						poolObjHandle.handle.ValueComponent.actorHp = 0;
						if (poolObjHandle)
						{
							poolObjHandle.handle.ActorControl.bSuicide = false;
							this.m_spawnedList.Remove(poolObjHandle);
						}
					}
				}
			}
			if (this.NextGroups.Length > 0)
			{
				SpawnGroup[] nextGroups = this.NextGroups;
				for (int i = 0; i < nextGroups.Length; i++)
				{
					SpawnGroup spawnGroup = nextGroups[i];
					if (spawnGroup != null)
					{
						spawnGroup.TriggerStartUp();
					}
				}
				NextSpawnGroupsParam nextSpawnGroupsParam = new NextSpawnGroupsParam(this, this.NextGroups);
				Singleton<GameEventSys>.instance.SendEvent<NextSpawnGroupsParam>(GameEventDef.Event_NextSpawnGroups, ref nextSpawnGroupsParam);
			}
		}

		public override void Stop()
		{
			base.Stop();
		}

		protected override void DecSpawnPointOver()
		{
			base.DecSpawnPointOver();
			if (this.m_spawnPointOver == 0)
			{
				this.m_bCountingSpawn = true;
				SGroupDeadEventParam sGroupDeadEventParam = default(SGroupDeadEventParam);
				sGroupDeadEventParam.sg = this;
				Singleton<GameEventSys>.instance.SendEvent<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, ref sGroupDeadEventParam);
				if (this.m_spawnCounter == 0 && this.SpawnTimes > 0)
				{
					this.GoNextGroups();
				}
			}
		}

		public override void Startup()
		{
			if (!this.bTriggerSpawn && !this.isStartup)
			{
				this.m_spawnTimer = this.StartUpDelay;
				this.m_spawnCounter = this.SpawnTimes;
				this.m_bCountingSpawn = true;
				base.Startup();
			}
		}

		public void TriggerStartUp()
		{
			if (!this.isStartup)
			{
				this.m_spawnTimer = this.StartUpDelay;
				this.m_spawnCounter = this.SpawnTimes;
				this.m_bCountingSpawn = true;
				base.Startup();
			}
		}

		public int GetSpawnTimer()
		{
			return this.m_spawnTimer;
		}

		public bool IsCountingDown()
		{
			return this.m_bCountingSpawn;
		}
	}
}

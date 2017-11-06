using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class CommonSpawnGroup : CommonSpawnPoint
	{
		public enum EIntervalRule
		{
			UsedUp,
			FixedInterval
		}

		[FriendlyName("需要触发器")]
		public bool bTriggerSpawn;

		[FriendlyName("第一次生成延迟")]
		public int StartUpDelay = 5000;

		[FriendlyName("生成间隔")]
		public int SpawnInternval = 10000;

		[FriendlyName("生成次数")]
		public int SpawnTimes;

		[FriendlyName("失效时间")]
		public int InvalidTime;

		private int m_invalidTimer;

		private int m_spawnTimer;

		private int m_spawnCounter;

		private bool m_bCountingSpawn;

		protected Color GroupColor = new Color(0.2f, 0.8f, 0.2f);

		private CommonSpawnPoint[] drawPoints;

		public CommonSpawnGroup.EIntervalRule IntervalRule;

		public int AlertPreroll;

		public bool bCountingSpawn
		{
			get
			{
				return this.m_bCountingSpawn;
			}
			private set
			{
				bool bCountingSpawn = this.m_bCountingSpawn;
				this.m_bCountingSpawn = value;
				if (!bCountingSpawn && this.m_bCountingSpawn)
				{
					SCommonSpawnEventParam sCommonSpawnEventParam = new SCommonSpawnEventParam((VInt3)base.gameObject.transform.position, this.m_spawnTimer, this.AlertPreroll, this.QuerySpawnObjType());
					Singleton<GameEventSys>.instance.SendEvent<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, ref sCommonSpawnEventParam);
				}
			}
		}

		protected virtual SpawnerWrapper.ESpawnObjectType QuerySpawnObjType()
		{
			SpawnerWrapper.ESpawnObjectType result = SpawnerWrapper.ESpawnObjectType.Invalid;
			if (this.SpawnerList.Length > 0)
			{
				result = this.SpawnerList[0].SpawnType;
			}
			return result;
		}

		protected override void Start()
		{
			base.Start();
			CommonSpawnPoint nextPoint = this.NextPoint;
			while (nextPoint)
			{
				this.m_spawnPointList.Add(nextPoint);
				nextPoint.onAllDeadEvt += new CommonSpawnPointAllDead(base.onSpawnPointAllDead);
				nextPoint = nextPoint.NextPoint;
			}
		}

		private CommonSpawnPoint[] FindChildrenPoints()
		{
			return base.GetComponentsInChildren<CommonSpawnPoint>();
		}

		protected override void OnDrawGizmos()
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
					this.Stop();
					return;
				}
			}
			if (!this.bCountingSpawn)
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
				this.DoSpawn();
				SCommonSpawnEventParam sCommonSpawnEventParam = new SCommonSpawnEventParam((VInt3)base.gameObject.transform.position, this.m_spawnTimer, this.AlertPreroll, this.QuerySpawnObjType());
				Singleton<GameEventSys>.instance.SendEvent<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupSpawn, ref sCommonSpawnEventParam);
				if (this.IntervalRule == CommonSpawnGroup.EIntervalRule.UsedUp)
				{
					this.bCountingSpawn = false;
				}
				this.m_spawnCounter--;
			}
		}

		protected virtual void GoNextGroup()
		{
			base.Stop();
		}

		protected override void DecSpawnPointOver()
		{
			base.DecSpawnPointOver();
			if (this.m_spawnPointOver == 0)
			{
				if (this.IntervalRule == CommonSpawnGroup.EIntervalRule.UsedUp)
				{
					this.bCountingSpawn = true;
				}
				SGroupDeadEventParam sGroupDeadEventParam = default(SGroupDeadEventParam);
				sGroupDeadEventParam.csg = this;
				Singleton<GameEventSys>.instance.SendEvent<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, ref sGroupDeadEventParam);
				if (this.m_spawnCounter == 0 && this.SpawnTimes > 0)
				{
					this.GoNextGroup();
				}
			}
		}

		public override void Startup()
		{
			if (!this.bTriggerSpawn && !this.isStartup)
			{
				this.m_spawnTimer = this.StartUpDelay;
				this.m_spawnCounter = this.SpawnTimes;
				this.bCountingSpawn = true;
				base.Startup();
			}
		}

		public void TriggerStartUp()
		{
			if (!this.isStartup)
			{
				this.m_spawnTimer = this.StartUpDelay;
				this.m_spawnCounter = this.SpawnTimes;
				this.bCountingSpawn = true;
				base.Startup();
			}
		}

		public int GetSpawnTimer()
		{
			return this.m_spawnTimer;
		}

		public int GetSpawnCounter()
		{
			return this.m_spawnCounter;
		}
	}
}

using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SoldierRegion : FuncRegion
	{
		public int WaveID;

		public int RouteID;

		public GameObject finalTarget;

		private int _mainlineWaveId;

		private int _emergentWaveId;

		private bool _isInEmergency;

		public GameObject AttackRoute;

		public bool bForceCompleteSpawn = true;

		[HideInInspector]
		[NonSerialized]
		public ListView<SoldierWave> Waves = new ListView<SoldierWave>();

		[HideInInspector]
		[NonSerialized]
		public SoldierWave CurrentWave;

		private SoldierWave _lastWave;

		private int curTick;

		private bool bShouldWait;

		private int waitTick;

		[HideInInspector]
		[NonSerialized]
		private bool bInited;

		private bool bShouldReset;

		public static bool bFirstSpawnEvent;

		private bool _isInitRouteType;

		public RES_SOLDIER_ROUTE_TYPE RouteType;

		public bool IsInEmergency
		{
			get
			{
				return this._isInEmergency;
			}
		}

		public void Awake()
		{
			this._isInitRouteType = false;
			this.LoadWave(this.WaveID);
			this._mainlineWaveId = 0;
			this._emergentWaveId = 0;
			this._isInEmergency = false;
		}

		public static ListView<SoldierWave> GetWavesForPreLoad(int inWaveId)
		{
			ListView<SoldierWave> listView = new ListView<SoldierWave>();
			DebugHelper.Assert(GameDataMgr.soldierWaveDatabin != null);
			for (ResSoldierWaveInfo dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey((uint)inWaveId); dataByKey != null; dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey(dataByKey.dwNextSoldierWaveID))
			{
				listView.Add(new SoldierWave(dataByKey));
			}
			return listView;
		}

		private void LoadWave(int theWaveID)
		{
			DebugHelper.Assert(GameDataMgr.soldierWaveDatabin != null);
			ResSoldierWaveInfo dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey((uint)theWaveID);
			int num = 0;
			if (dataByKey != null && !this._isInitRouteType)
			{
				this._isInitRouteType = true;
				this.RouteType = (RES_SOLDIER_ROUTE_TYPE)dataByKey.bRouteType;
			}
			while (dataByKey != null)
			{
				this.Waves.Add(new SoldierWave(this, dataByKey, num++));
				dataByKey = GameDataMgr.soldierWaveDatabin.GetDataByKey(dataByKey.dwNextSoldierWaveID);
			}
		}

		public void SwitchWave(int waveId, bool isEmergency)
		{
			if (isEmergency)
			{
				this._emergentWaveId = waveId;
				this._isInEmergency = true;
			}
			else
			{
				this._mainlineWaveId = waveId;
			}
		}

		private SoldierWave FindNextValidWave()
		{
			if (this.CurrentWave == null)
			{
				this.CurrentWave = ((this.Waves.Count <= 0) ? null : this.Waves[0]);
			}
			else
			{
				this.CurrentWave = ((this.CurrentWave.Index >= this.Waves.Count - 1) ? null : this.Waves[this.CurrentWave.Index + 1]);
			}
			if (this.CurrentWave != null && this._lastWave != null)
			{
				this.CurrentWave.ContinueState(this._lastWave);
				this._lastWave = null;
			}
			int inWaveIndex = (this.CurrentWave == null) ? (this.Waves.Count - 1) : this.CurrentWave.Index;
			SoldierWaveParam soldierWaveParam = new SoldierWaveParam(inWaveIndex, 0, this.GetNextRepeatTime(true), this.CurrentWave);
			Singleton<GameEventSys>.instance.SendEvent<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, ref soldierWaveParam);
			return this.CurrentWave;
		}

		public int GetTotalCount()
		{
			int num = 0;
			ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SoldierWave current = enumerator.Current;
				if (current.WaveInfo.dwRepeatNum == 0u)
				{
					return 0;
				}
				num += (int)current.WaveInfo.dwRepeatNum;
			}
			return num;
		}

		public int GetRepeatCountTill(int inWaveIndex)
		{
			int num = 0;
			int num2 = 0;
			ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
			while (enumerator.MoveNext() && num < inWaveIndex)
			{
				SoldierWave current = enumerator.Current;
				if (current.WaveInfo.dwRepeatNum == 0u)
				{
					return 0;
				}
				num2 += (int)current.WaveInfo.dwRepeatNum;
				num++;
			}
			int repeatCount = this.Waves[inWaveIndex].repeatCount;
			return num2 + repeatCount;
		}

		public int GetTotalTime()
		{
			int num = 0;
			ListView<SoldierWave>.Enumerator enumerator = this.Waves.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SoldierWave current = enumerator.Current;
				if (current.WaveInfo.dwRepeatNum == 0u)
				{
					return 0;
				}
				num += (int)current.WaveInfo.dwStartWatiTick;
				num += (int)(current.WaveInfo.dwIntervalTick * (current.WaveInfo.dwRepeatNum - 1u));
			}
			return num;
		}

		public int GetNextRepeatTime(bool bWaveOrRepeat)
		{
			if (this.CurrentWave == null)
			{
				return -1;
			}
			if (bWaveOrRepeat)
			{
				return (int)this.CurrentWave.WaveInfo.dwStartWatiTick;
			}
			bool flag = (long)this.CurrentWave.repeatCount >= (long)((ulong)this.CurrentWave.WaveInfo.dwRepeatNum);
			if (!flag)
			{
				return (int)(this.CurrentWave.WaveInfo.dwIntervalTick + this.CurrentWave.Selector.StatTotalCount * (uint)MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval);
			}
			bool flag2 = this.CurrentWave.Index + 1 < this.Waves.Count;
			if (flag2)
			{
				SoldierWave soldierWave = this.Waves[this.CurrentWave.Index + 1];
				return (int)(this.CurrentWave.Selector.StatTotalCount * (uint)MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval + this.CurrentWave.WaveInfo.dwIntervalTick + soldierWave.WaveInfo.dwStartWatiTick);
			}
			return -1;
		}

		public SoldierSpawnResult UpdateLogicSpec(int delta)
		{
			if (!this.isStartup)
			{
				return SoldierSpawnResult.UnStarted;
			}
			if (this.CurrentWave == null && !this.bShouldReset)
			{
				return SoldierSpawnResult.Completed;
			}
			if (this.CurrentWave == null || this.CurrentWave.IsInIdle)
			{
				this.TrySwitch();
			}
			if (this.bShouldWait || this.bShouldReset)
			{
				this.bShouldReset = false;
				this.curTick += delta;
				if (this.curTick <= this.waitTick)
				{
					return SoldierSpawnResult.ShouldWaitInterval;
				}
				this.FindNextValidWave();
				this.bShouldWait = false;
				this.curTick = 0;
				this.waitTick = 0;
				if (this.CurrentWave == null)
				{
					return SoldierSpawnResult.Completed;
				}
			}
			SoldierSpawnResult soldierSpawnResult = this.CurrentWave.Update(delta);
			if (soldierSpawnResult > SoldierSpawnResult.ThresholdShouldWait)
			{
				if (this._isInEmergency && this._emergentWaveId == 0)
				{
					this._isInEmergency = false;
					this.TrySwitch();
				}
				else
				{
					this.bShouldWait = true;
					this.curTick = 0;
					this.waitTick = (int)this.CurrentWave.WaveInfo.dwIntervalTick;
				}
			}
			return soldierSpawnResult;
		}

		private void TrySwitch()
		{
			int num = 0;
			if (this._isInEmergency)
			{
				if (this._emergentWaveId != 0)
				{
					num = this._emergentWaveId;
					this._emergentWaveId = 0;
					if (this._mainlineWaveId == 0)
					{
						if (this.CurrentWave != null)
						{
							this._mainlineWaveId = (int)this.CurrentWave.WaveInfo.dwSoldierWaveID;
						}
						else if (this.Waves.Count > 0)
						{
							this._mainlineWaveId = (int)this.Waves[0].WaveInfo.dwSoldierWaveID;
						}
					}
				}
			}
			else if (this._mainlineWaveId != 0)
			{
				num = this._mainlineWaveId;
				this._mainlineWaveId = 0;
			}
			if (num != 0)
			{
				this._lastWave = this.CurrentWave;
				this.CurrentWave = null;
				this.bShouldReset = true;
				this.waitTick = 0;
				this.curTick = 0;
				this.Waves.Clear();
				this.LoadWave(num);
			}
		}

		public override void Startup()
		{
			base.Startup();
			if (!this.bInited)
			{
				this.FindNextValidWave();
				this.bInited = true;
			}
		}

		public void ResetRegion()
		{
			this.CurrentWave = null;
			this.bShouldReset = true;
			this.waitTick = 0;
			this.curTick = 0;
			if (this.Waves != null)
			{
				for (int i = 0; i < this.Waves.Count; i++)
				{
					if (this.Waves[i] != null)
					{
						this.Waves[i].Reset();
					}
				}
			}
		}

		public bool IsMyCamp()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			return hostPlayer != null && hostPlayer.PlayerCamp == this.CampType;
		}
	}
}

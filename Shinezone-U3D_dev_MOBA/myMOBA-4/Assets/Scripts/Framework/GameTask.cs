using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class GameTask
	{
		protected enum State
		{
			AWAKE,
			READY,
			START,
			CLOSE
		}

		private delegate bool CompareOperatorFunc(int l, int r);

		private static GameTask.CompareOperatorFunc[] _compareFuncs;

		private ResGameTask _config;

		private GameTaskSys _rootSys;

		private GameTask.State _state;

		private int _current;

		private int _timer;

		private int _readyTimer;

		private ListView<GameTaskGroup> _ownerGroups;

		protected virtual int StartValue
		{
			get
			{
				return 0;
			}
		}

		public virtual int Target
		{
			get
			{
				return this._config.iTarget;
			}
		}

		public virtual float Progress
		{
			get
			{
				return (float)this.Current / (float)this.Target;
			}
		}

		public virtual bool IsGroup
		{
			get
			{
				return false;
			}
		}

		public GameTaskSys RootSys
		{
			get
			{
				return this._rootSys;
			}
		}

		protected ResGameTask Config
		{
			get
			{
				return this._config;
			}
		}

		public uint ID
		{
			get
			{
				return this._config.dwID;
			}
		}

		public string Type
		{
			get
			{
				return Utility.UTF8Convert(this._config.szType);
			}
		}

		public string Name
		{
			get
			{
				return Utility.UTF8Convert(this._config.szName);
			}
		}

		protected RES_COMPARE_OPERATOR_TYPE CompareOperator
		{
			get
			{
				if (this._config.bCompare > 0 && this._config.bCompare < 6)
				{
					return (RES_COMPARE_OPERATOR_TYPE)this._config.bCompare;
				}
				return RES_COMPARE_OPERATOR_TYPE.RES_COMPARE_OPERATOR_TYPE_BIGGER_EQUAL;
			}
		}

		public int TimeLimit
		{
			get
			{
				return Mathf.Abs(this._config.iTimeLimit);
			}
		}

		public int TimeReady
		{
			get
			{
				return this._config.iStartDelay;
			}
		}

		public bool StrictTime
		{
			get
			{
				return this._config.iTimeLimit < 0;
			}
		}

		public int TimeRemain
		{
			get
			{
				if (this._timer > 0)
				{
					int timerCurrent = Singleton<CTimerManager>.instance.GetTimerCurrent(this._timer);
					if (timerCurrent > -1)
					{
						return this.TimeLimit - timerCurrent;
					}
				}
				return 0;
			}
		}

		public string StartAction
		{
			get
			{
				return Utility.UTF8Convert(this._config.szStartAction);
			}
		}

		public string CloseAction
		{
			get
			{
				return Utility.UTF8Convert(this._config.szCloseAction);
			}
		}

		public int Current
		{
			get
			{
				return this._current;
			}
			set
			{
				if (value != this._current && this._state == GameTask.State.START)
				{
					bool achieving = this.Achieving;
					this._current = value;
					this.RootSys._OnTaskGoing(this);
					if (this.Achieving && !achieving && !this.StrictTime)
					{
						this.Close();
					}
				}
			}
		}

		public bool Active
		{
			get
			{
				return this._state == GameTask.State.START;
			}
		}

		public bool Closed
		{
			get
			{
				return this._state == GameTask.State.CLOSE;
			}
		}

		public bool Achieving
		{
			get
			{
				return GameTask._compareFuncs[this.CompareOperator - RES_COMPARE_OPERATOR_TYPE.RES_COMPARE_OPERATOR_TYPE_LESS](this.Current, this.Target);
			}
		}

		static GameTask()
		{
			// Note: this type is marked as 'beforefieldinit'.
			GameTask.CompareOperatorFunc[] expr_06 = new GameTask.CompareOperatorFunc[5];
			expr_06[0] = ((int l, int r) => l < r);
			expr_06[1] = ((int l, int r) => l <= r);
			expr_06[2] = ((int l, int r) => l == r);
			expr_06[3] = ((int l, int r) => l > r);
			expr_06[4] = ((int l, int r) => l >= r);
			GameTask._compareFuncs = expr_06;
		}

		protected virtual void OnInitial()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnClose()
		{
		}

		protected virtual void OnTimeOver()
		{
		}

		public void Initial(ResGameTask config, GameTaskSys rootSys)
		{
			this._rootSys = rootSys;
			this._config = config;
			this._current = 0;
			this._timer = 0;
			this._readyTimer = 0;
			this._ownerGroups = null;
			this._state = GameTask.State.AWAKE;
			DebugHelper.Assert(null != this._config, "GameTask.config must not be null!");
			this.OnInitial();
		}

		public void Destroy()
		{
			this.OnDestroy();
			Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
			Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
		}

		public void Start()
		{
			if (this._state != GameTask.State.AWAKE)
			{
				return;
			}
			if (this.TimeReady > 0)
			{
				Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
				this._readyTimer = Singleton<CTimerManager>.instance.AddTimer(this.TimeReady, 1, new CTimer.OnTimeUpHandler(this.OnReadyOver), true);
				this._state = GameTask.State.READY;
				this._rootSys._OnTaskReady(this);
			}
			else
			{
				this.DoStart();
			}
		}

		private void OnReadyOver(int timer)
		{
			Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
			this.DoStart();
		}

		private void DoStart()
		{
			this._current = this.StartValue;
			Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
			if (this.TimeLimit > 0)
			{
				this._timer = Singleton<CTimerManager>.instance.AddTimer(this.TimeLimit, 1, new CTimer.OnTimeUpHandler(this.OnTimeOut), true);
			}
			this._state = GameTask.State.START;
			this.OnStart();
			this.RootSys._OnTaskStart(this);
		}

		public void Close()
		{
			if (this._state != GameTask.State.START)
			{
				return;
			}
			Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
			this.OnClose();
			this._state = GameTask.State.CLOSE;
			this.RootSys._OnTaskClose(this);
			if (this._ownerGroups != null)
			{
				for (int i = 0; i < this._ownerGroups.Count; i++)
				{
					this._ownerGroups[i]._OnChildClosed(this);
				}
			}
		}

		private void OnTimeOut(int timer)
		{
			this.OnTimeOver();
			this.Close();
		}

		internal void _AddOwnerGroup(GameTaskGroup etg)
		{
			if (this._ownerGroups == null)
			{
				this._ownerGroups = new ListView<GameTaskGroup>();
			}
			this._ownerGroups.Add(etg);
		}
	}
}

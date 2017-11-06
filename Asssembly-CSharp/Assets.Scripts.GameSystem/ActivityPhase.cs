using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public abstract class ActivityPhase
	{
		public enum TimeState
		{
			NotStart,
			Started,
			Closed
		}

		public delegate void ActivityPhaseEvent(ActivityPhase ap);

		private Activity _owner;

		private ActivityPhase.TimeState _timeState;

		private int _secondSpan;

		private bool _marked;

		private ResRandomRewardStore _rewardStore;

		private int _rewardCount;

		private ResRandomRewardStore _extraRewardStore;

		private int _extraRewardCount;

		private int _exchangeCountOnce = 1;

		private static float _lastDrawTime;

		public event ActivityPhase.ActivityPhaseEvent OnTimeStateChange
		{
			[MethodImpl(32)]
			add
			{
				this.OnTimeStateChange = (ActivityPhase.ActivityPhaseEvent)Delegate.Combine(this.OnTimeStateChange, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnTimeStateChange = (ActivityPhase.ActivityPhaseEvent)Delegate.Remove(this.OnTimeStateChange, value);
			}
		}

		public event ActivityPhase.ActivityPhaseEvent OnMaskStateChange
		{
			[MethodImpl(32)]
			add
			{
				this.OnMaskStateChange = (ActivityPhase.ActivityPhaseEvent)Delegate.Combine(this.OnMaskStateChange, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnMaskStateChange = (ActivityPhase.ActivityPhaseEvent)Delegate.Remove(this.OnMaskStateChange, value);
			}
		}

		public abstract uint ID
		{
			get;
		}

		public abstract uint RewardID
		{
			get;
		}

		public abstract int StartTime
		{
			get;
		}

		public abstract int CloseTime
		{
			get;
		}

		public virtual uint ExtraRewardID
		{
			get
			{
				return 0u;
			}
		}

		public virtual bool InMultipleTime
		{
			get
			{
				return false;
			}
		}

		public virtual uint MultipleTimes
		{
			get
			{
				return (this.InMultipleTime && this.Owner.InMultipleTime) ? this.Owner.MultipleTimes : 0u;
			}
		}

		public virtual int Current
		{
			get
			{
				return this._owner.Current;
			}
		}

		public virtual int Target
		{
			get
			{
				return (int)(this.ID + 1u);
			}
		}

		public string StartTimeText
		{
			get
			{
				return ActivityPhase.SecondsToHMText(this.StartTime);
			}
		}

		public string CloseTimeText
		{
			get
			{
				return ActivityPhase.SecondsToHMText(this.CloseTime);
			}
		}

		public virtual string Tips
		{
			get
			{
				return this.StartTimeText + "~" + this.CloseTimeText;
			}
		}

		public Activity Owner
		{
			get
			{
				return this._owner;
			}
		}

		public bool Marked
		{
			get
			{
				return this._marked;
			}
			set
			{
				if (value != this._marked)
				{
					this._marked = value;
					this._NotifyStateChanged();
				}
			}
		}

		public ActivityPhase.TimeState timeState
		{
			get
			{
				return this._timeState;
			}
		}

		public virtual bool ReadyForGet
		{
			get
			{
				return this.timeState == ActivityPhase.TimeState.Started && !this._marked;
			}
		}

		public virtual bool AchieveStateValid
		{
			get
			{
				return false;
			}
		}

		public virtual bool Achieved
		{
			get
			{
				return false;
			}
		}

		public virtual int AchieveInDays
		{
			get
			{
				return 0;
			}
		}

		private ResRandomRewardStore RewardDrop
		{
			get
			{
				if (this._rewardStore == null && (this._rewardStore = GameDataMgr.randomRewardDB.GetDataByKey(this.RewardID)) != null)
				{
					this._rewardCount = 0;
					while (this._rewardCount < this._rewardStore.astRewardDetail.Length)
					{
						if (this._rewardStore.astRewardDetail[this._rewardCount].bItemType == 0 || this._rewardStore.astRewardDetail[this._rewardCount].bItemType >= 18)
						{
							break;
						}
						this._rewardCount++;
					}
				}
				return this._rewardStore;
			}
		}

		public int RewardCount
		{
			get
			{
				if (this.RewardDrop == null)
				{
					return 0;
				}
				return this._rewardCount;
			}
		}

		public string RewardDesc
		{
			get
			{
				if (this.RewardDrop == null)
				{
					return string.Empty;
				}
				return Utility.UTF8Convert(this.RewardDrop.szRewardDesc).Trim();
			}
		}

		private ResRandomRewardStore ExtraRewardDrop
		{
			get
			{
				if (this._extraRewardStore == null && (this._extraRewardStore = GameDataMgr.randomRewardDB.GetDataByKey(this.ExtraRewardID)) != null)
				{
					this._extraRewardCount = 0;
					while (this._extraRewardCount < this._extraRewardStore.astRewardDetail.Length)
					{
						if (this._extraRewardStore.astRewardDetail[this._extraRewardCount].bItemType == 0 || this._extraRewardStore.astRewardDetail[this._extraRewardCount].bItemType >= 18)
						{
							break;
						}
						this._extraRewardCount++;
					}
				}
				return this._extraRewardStore;
			}
		}

		public int ExtraRewardCount
		{
			get
			{
				if (this.ExtraRewardDrop != null)
				{
					return this._extraRewardCount;
				}
				return 0;
			}
		}

		public ActivityPhase(Activity owner)
		{
			this._owner = owner;
			this._timeState = ActivityPhase.TimeState.NotStart;
			this._secondSpan = 0;
			this._marked = false;
			this._rewardStore = null;
			this._rewardCount = 0;
			this._extraRewardStore = null;
			this._extraRewardCount = 0;
		}

		public virtual uint GetVipAddition(int vipFlagBit)
		{
			return 0u;
		}

		public virtual void Clear()
		{
		}

		private static string SecondsToHMText(int secondsInDay)
		{
			int num = secondsInDay / 3600;
			int num2 = (secondsInDay - num * 3600) / 60;
			return string.Format("{0:D2}:{1:D2}", num, num2);
		}

		internal void _NotifyStateChanged()
		{
			if (this.OnMaskStateChange != null)
			{
				this.OnMaskStateChange(this);
			}
		}

		public virtual bool AchieveJump()
		{
			return false;
		}

		public virtual void SetExchangeCountOnce(int exchangeCount)
		{
			this._exchangeCountOnce = Math.Max(exchangeCount, 1);
		}

		public virtual int GetExchangeCountOnce()
		{
			return this._exchangeCountOnce;
		}

		public virtual bool CheckTimeState()
		{
			ActivityPhase.TimeState timeState;
			if (this._owner.timeState == Activity.TimeState.Going)
			{
				if (this.StartTime > 0 && this.CloseTime > 0)
				{
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					DateTime dateTime2 = Utility.SecondsToDateTime(dateTime.get_Year(), dateTime.get_Month(), dateTime.get_Day(), this.StartTime);
					this._secondSpan = (int)(dateTime - dateTime2).get_TotalSeconds();
					if (this._secondSpan < 0)
					{
						this._secondSpan = -this._secondSpan;
						timeState = ActivityPhase.TimeState.NotStart;
					}
					else
					{
						DateTime dateTime3 = Utility.SecondsToDateTime(dateTime.get_Year(), dateTime.get_Month(), dateTime.get_Day(), this.CloseTime);
						this._secondSpan = (int)(dateTime - dateTime3).get_TotalSeconds();
						if (this._secondSpan < 0)
						{
							this._secondSpan = -this._secondSpan;
							timeState = ActivityPhase.TimeState.Started;
						}
						else
						{
							timeState = ActivityPhase.TimeState.Closed;
						}
					}
				}
				else
				{
					this._secondSpan = 0;
					timeState = ActivityPhase.TimeState.Started;
				}
			}
			else if (this._owner.timeState == Activity.TimeState.ForeShow || this._owner.timeState == Activity.TimeState.InHiding)
			{
				timeState = ActivityPhase.TimeState.NotStart;
			}
			else
			{
				timeState = ActivityPhase.TimeState.Closed;
			}
			if (timeState != this._timeState)
			{
				this._timeState = timeState;
				if (this.OnTimeStateChange != null)
				{
					this.OnTimeStateChange(this);
				}
				return true;
			}
			return false;
		}

		public void DrawReward()
		{
			if (Time.time < ActivityPhase._lastDrawTime + 0.75f)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2503u);
			cSPkg.stPkgData.stDrawWealReq.bWealType = (byte)this._owner.Type;
			cSPkg.stPkgData.stDrawWealReq.dwWealID = this._owner.ID;
			cSPkg.stPkgData.stDrawWealReq.dwPeriodID = this.ID;
			cSPkg.stPkgData.stDrawWealReq.dwDrawCnt = (uint)this.GetExchangeCountOnce();
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			ActivityPhase._lastDrawTime = Time.time;
		}

		public CUseable GetUseable(int index)
		{
			if (this.RewardDrop != null && index < this.RewardCount)
			{
				ResDT_RandomRewardInfo resDT_RandomRewardInfo = this.RewardDrop.astRewardDetail[index];
				return CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resDT_RandomRewardInfo.bItemType, (int)resDT_RandomRewardInfo.dwLowCnt, resDT_RandomRewardInfo.dwItemID);
			}
			return null;
		}

		public uint GetDropCount(int index)
		{
			if (this.RewardDrop != null)
			{
				ResDT_RandomRewardInfo resDT_RandomRewardInfo = this.RewardDrop.astRewardDetail[index];
				if (resDT_RandomRewardInfo.dwLowCnt == resDT_RandomRewardInfo.dwHighCnt)
				{
					return resDT_RandomRewardInfo.dwLowCnt;
				}
			}
			return 0u;
		}

		public CUseable GetExtraUseable(int index)
		{
			if (this.ExtraRewardDrop != null && index < this.ExtraRewardCount)
			{
				ResDT_RandomRewardInfo resDT_RandomRewardInfo = this.ExtraRewardDrop.astRewardDetail[index];
				return CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resDT_RandomRewardInfo.bItemType, (int)resDT_RandomRewardInfo.dwLowCnt, resDT_RandomRewardInfo.dwItemID);
			}
			return null;
		}
	}
}

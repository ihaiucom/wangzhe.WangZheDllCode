using Apollo;
using CSProtocol;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public abstract class Activity
	{
		public enum TimeState
		{
			InHiding,
			ForeShow,
			Going,
			Close
		}

		public delegate void ActivityEvent(Activity acty);

		private ActivitySys _sys;

		private ResDT_WealCommon _config;

		private ListView<ActivityPhase> _phaseList;

		private Activity.TimeState _timeState;

		private int _secondSpan;

		private bool _visited;

		public event Activity.ActivityEvent OnTimeStateChange;

		public event Activity.ActivityEvent OnMaskStateChange;

		public abstract uint ID
		{
			get;
		}

		public abstract COM_WEAL_TYPE Type
		{
			get;
		}

		public ActivitySys Sys
		{
			get
			{
				return this._sys;
			}
		}

		public uint Key
		{
			get
			{
				return Activity.GenKey(this.Type, this.ID);
			}
		}

		public ListView<ActivityPhase> PhaseList
		{
			get
			{
				return this._phaseList;
			}
		}

		public virtual string Name
		{
			get
			{
				return Utility.UTF8Convert(this._config.szName);
			}
		}

		public virtual string Wigets
		{
			get
			{
				return Utility.UTF8Convert(this._config.szWidgets);
			}
		}

		public virtual string Title
		{
			get
			{
				string text = Utility.UTF8Convert(this._config.szTitle);
				if (this.IsImageTitle)
				{
					return text.Substring(2);
				}
				return text;
			}
		}

		public virtual bool IsImageTitle
		{
			get
			{
				string text = Utility.UTF8Convert(this._config.szTitle);
				return text.Length > 2 && text.IndexOf("i:", 0, 2) == 0;
			}
		}

		public virtual string Icon
		{
			get
			{
				return Utility.UTF8Convert(this._config.szIcon);
			}
		}

		public virtual string Brief
		{
			get
			{
				return Utility.UTF8Convert(this._config.szBrief);
			}
		}

		public virtual string Content
		{
			get
			{
				return Utility.UTF8Convert(this._config.szDescContent);
			}
		}

		public virtual string TimeDesc
		{
			get
			{
				return Utility.UTF8Convert(this._config.szTimeDesc);
			}
		}

		public virtual string Tips
		{
			get
			{
				string text = Utility.UTF8Convert(this._config.szTips);
				string arg_25_0 = text;
				string arg_25_1 = "{C}";
				int current = this.Current;
				text = arg_25_0.Replace(arg_25_1, current.ToString());
				return text.Replace("{T}", this.Target.ToString());
			}
		}

		public virtual uint Sequence
		{
			get
			{
				return this._config.dwSortID;
			}
		}

		public virtual RES_WEAL_TIME_TYPE TimeType
		{
			get
			{
				return (RES_WEAL_TIME_TYPE)this._config.dwTimeType;
			}
		}

		public virtual long ShowTime
		{
			get
			{
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_LIMIT)
				{
					return (long)this._config.ullShowTime;
				}
				return this.StartTime;
			}
		}

		public virtual long StartTime
		{
			get
			{
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						if ((ulong)masterRoleInfo.AccountRegisterTime >= this._config.ullStartTime)
						{
							return masterRoleInfo.AccountRegisterTime_ZeroDay;
						}
                        return 0xf45c2700L;
					}
				}
				return (long)this._config.ullStartTime;
			}
		}

		public virtual long CloseTime
		{
			get
			{
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						if ((ulong)masterRoleInfo.AccountRegisterTime >= this._config.ullStartTime)
						{
							return masterRoleInfo.AccountRegisterTime_ZeroDay + (long)(86399uL * this._config.ullEndTime) - 1L;
						}
						return 0xf45c2700L;
					}
				}
				return (long)this._config.ullEndTime;
			}
		}

		public virtual RES_WEAL_COLORBAR_TYPE FlagType
		{
			get
			{
				if (this.timeState < Activity.TimeState.Going)
				{
					return RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NOTICE;
				}
				RES_WEAL_COLORBAR_TYPE rES_WEAL_COLORBAR_TYPE = (RES_WEAL_COLORBAR_TYPE)this._config.bColorBar;
				if (rES_WEAL_COLORBAR_TYPE == RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NEW && this.StartTime > 0L)
				{
					DateTime d = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					if ((d - Utility.ToUtcTime2Local(this.StartTime)).TotalDays > 2.0)
					{
						rES_WEAL_COLORBAR_TYPE = RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_HOT;
					}
				}
				return rES_WEAL_COLORBAR_TYPE;
			}
		}

		public virtual RES_WEAL_ENTRANCE_TYPE Entrance
		{
			get
			{
				return (RES_WEAL_ENTRANCE_TYPE)this._config.bEntrance;
			}
		}

		public virtual uint MultipleTimes
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

		public virtual int Current
		{
			get
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					if (!this.PhaseList[i].Marked)
					{
						return i;
					}
				}
				return this.PhaseList.Count;
			}
		}

		public virtual int Target
		{
			get
			{
				return this.PhaseList.Count;
			}
		}

		public virtual bool ReadyForGet
		{
			get
			{
				if (this.timeState != Activity.TimeState.Going)
				{
					return false;
				}
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					if (this.PhaseList[i].ReadyForGet)
					{
						return true;
					}
				}
				return false;
			}
		}

		public virtual bool ReadyForDot
		{
			get
			{
				return this.ReadyForGet || !this._visited;
			}
		}

		public virtual bool Completed
		{
			get
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					if (!this.PhaseList[i].Marked)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual ActivityPhase CurPhase
		{
			get
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					if (!this.PhaseList[i].Marked)
					{
						return this.PhaseList[i];
					}
				}
				if (this.PhaseList.Count > 0)
				{
					return this.PhaseList[this.PhaseList.Count - 1];
				}
				return null;
			}
		}

		public string UID
		{
			get
			{
				ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
				return string.Concat(new object[]
				{
					"sgame_",
					(accountInfo == null) ? "0" : accountInfo.OpenId,
					"_activity_",
					this.Key
				});
			}
		}

		public string PeriodText
		{
			get
			{
				string text = Utility.DateTimeFormatString(Utility.ToUtcTime2Local(this.StartTime), Utility.enDTFormate.DATE);
				string text2 = Utility.DateTimeFormatString(Utility.ToUtcTime2Local(this.CloseTime), Utility.enDTFormate.DATE);
				string text3 = this.TimeDesc;
				if (!string.IsNullOrEmpty(text3))
				{
					text3 = text3.Trim();
					if (text3.Length > 0)
					{
						text3 = text3.Replace("{S}", text);
						return text3.Replace("{C}", text2);
					}
				}
				return (!(text != text2)) ? text : (text + " ~ " + text2);
			}
		}

		public Activity.TimeState timeState
		{
			get
			{
				return this._timeState;
			}
		}

		public int secondSpan
		{
			get
			{
				return this._secondSpan;
			}
		}

		public string timeRemainText
		{
			get
			{
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER)
				{
					return Singleton<CTextManager>.GetInstance().GetText("activityForever");
				}
				if (this._timeState < Activity.TimeState.Going)
				{
					return Singleton<CTextManager>.GetInstance().GetText("activityNotStart");
				}
				if (this._timeState > Activity.TimeState.Going)
				{
					return Singleton<CTextManager>.GetInstance().GetText("activityTimeOver");
				}
				int num = this._secondSpan;
				int num2 = num / 86400;
				num -= num2 * 86400;
				int num3 = num / 3600;
				num -= num3 * 3600;
				int num4 = num / 60;
				num -= num4 * 60;
				string text = Singleton<CTextManager>.GetInstance().GetText("TIME_SPAN_FORMAT");
				text = text.Replace("{0}", num2.ToString());
				text = text.Replace("{1}", num3.ToString());
				text = text.Replace("{2}", num4.ToString());
				return text.Replace("{3}", num.ToString());
			}
		}

		public bool Visited
		{
			get
			{
				return this._visited;
			}
			set
			{
				if (this._visited != value)
				{
					this._visited = value;
					PlayerPrefs.SetInt(this.UID, (!this._visited) ? 0 : 1);
					PlayerPrefs.Save();
					this.NotifyMaskStateChanged();
				}
			}
		}

		public Activity(ActivitySys mgr, ResDT_WealCommon config)
		{
			this._sys = mgr;
			this._config = config;
			this._phaseList = new ListView<ActivityPhase>();
			this._timeState = Activity.TimeState.InHiding;
			this._secondSpan = 0;
		}

		public static uint GenKey(COM_WEAL_TYPE type, uint id)
		{
            return ((((uint)type) << 0x1c) | (0xfffffff & id));
		}

		protected void AddPhase(ActivityPhase ap)
		{
			this._phaseList.Add(ap);
		}

		public virtual void Start()
		{
			this._visited = (PlayerPrefs.GetInt(this.UID, 0) > 0);
		}

		public virtual void Clear()
		{
			for (int i = 0; i < this._phaseList.Count; i++)
			{
				this._phaseList[i].Clear();
			}
			this._phaseList.Clear();
		}

		public int GetTabID()
		{
			if (this._config != null)
			{
				return (int)this._config.bTabID;
			}
			return 0;
		}

		public virtual void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
		{
		}

		public virtual uint GetVipAddition(int vipFlagBit)
		{
			return 0u;
		}

		public virtual void SetPhaseMarks(ulong mask)
		{
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				this.PhaseList[i].Marked = ((mask & 1uL << i) > 0uL);
			}
			this.NotifyMaskStateChanged();
		}

		public virtual void SetPhaseMarked(uint phaseId)
		{
			if ((ulong)phaseId < (ulong)((long)this.PhaseList.Count))
			{
				this.PhaseList[(int)phaseId].Marked = true;
				this.NotifyMaskStateChanged();
			}
		}

		public virtual bool CheckTimeState()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			bool flag = false;
			Activity.TimeState timeState;
			if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_LIMIT || this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT || this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
			{
				if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT && ((ulong)masterRoleInfo.AccountRegisterTime < (ulong)this.StartTime || (ulong)masterRoleInfo.AccountRegisterTime > (ulong)this.CloseTime))
				{
					timeState = Activity.TimeState.Close;
				}
				else
				{
					long num = (long)CRoleInfo.GetCurrentUTCTime();
					this._secondSpan = (int)(num - this.ShowTime);
					if (this._secondSpan < 0)
					{
						this._secondSpan = -this._secondSpan;
						timeState = Activity.TimeState.InHiding;
					}
					else
					{
						this._secondSpan = (int)(num - this.StartTime);
						if (this._secondSpan < 0)
						{
							timeState = Activity.TimeState.ForeShow;
							this._secondSpan = -this._secondSpan;
						}
						else
						{
							this._secondSpan = (int)(num - this.CloseTime);
							if (this._secondSpan <= 0)
							{
								timeState = Activity.TimeState.Going;
								this._secondSpan = -this._secondSpan;
							}
							else
							{
								timeState = Activity.TimeState.Close;
							}
						}
					}
				}
			}
			else
			{
				if (this.TimeType != RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER)
				{
					return false;
				}
				timeState = Activity.TimeState.Going;
			}
			Activity.TimeState timeState2 = this._timeState;
			if (timeState != this._timeState)
			{
				this._timeState = timeState;
				flag = true;
			}
			if (this._timeState == Activity.TimeState.Going || timeState2 == Activity.TimeState.Going)
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					if (this.PhaseList[i].CheckTimeState())
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.NotifyTimeStateChanged();
			}
			return flag;
		}

		protected void NotifyMaskStateChanged()
		{
			if (this.OnMaskStateChange != null)
			{
				this.OnMaskStateChange(this);
			}
			this.Sys._NotifyStateChanged();
		}

		protected void NotifyTimeStateChanged()
		{
			if (this.OnTimeStateChange != null)
			{
				this.OnTimeStateChange(this);
			}
			this.Sys._NotifyStateChanged();
		}
	}
}

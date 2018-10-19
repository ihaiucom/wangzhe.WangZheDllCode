using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class ExeTaskPhase : ActivityPhase
	{
		private uint _id;

		private ResDT_WealConInfo _config;

		private int _current;

		private bool _achieved;

		private bool _achieveInLimit;

		public override uint ID
		{
			get
			{
				return this._id;
			}
		}

		public override uint RewardID
		{
			get
			{
				return this._config.dwFixedRewardID;
			}
		}

		public override uint ExtraRewardID
		{
			get
			{
				return this._config.dwLimitRewardID;
			}
		}

		public override int StartTime
		{
			get
			{
				return 0;
			}
		}

		public override int CloseTime
		{
			get
			{
				return 0;
			}
		}

		public override string Tips
		{
			get
			{
				int num = this.Current;
				int target = this.Target;
				if (num > target)
				{
					num = target;
				}
				int num2 = target - num;
				string text = Utility.UTF8Convert(this._config.szDesc);
				text = text.Replace("{C}", num.ToString());
				text = text.Replace("{T}", target.ToString());
				return text.Replace("{R}", num2.ToString());
			}
		}

		public override bool ReadyForGet
		{
			get
			{
				return this.Achieved && base.ReadyForGet;
			}
		}

		public override bool AchieveStateValid
		{
			get
			{
				return base.timeState == ActivityPhase.TimeState.Started;
			}
		}

		public override bool Achieved
		{
			get
			{
				return this._achieved;
			}
		}

		public override int AchieveInDays
		{
			get
			{
				if (this.LimitDays == 0u)
				{
					return 99;
				}
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return -1;
				}
				if (this._achieved)
				{
					if (this._achieveInLimit)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					DateTime d = Utility.ToUtcTime2Local((long)((ulong)CRoleInfo.GetCurrentUTCTime()));
					DateTime d2 = Utility.ToUtcTime2Local((long)((ulong)masterRoleInfo.AccountRegisterTime));
					d2 = new DateTime(d2.Year, d2.Month, d2.Day, 0, 0, 0);
					double totalDays = (d - d2).TotalDays;
					if (totalDays > this.LimitDays)
					{
						return -1;
					}
					return (int)Math.Ceiling(this.LimitDays - totalDays);
				}
			}
		}

		public RES_WEAL_CON_TYPE ConditionType
		{
			get
			{
				return (RES_WEAL_CON_TYPE)this._config.dwConType;
			}
		}

		public uint LimitDays
		{
			get
			{
				return this._config.dwLimitDays;
			}
		}

		public override int Target
		{
			get
			{
				return (int)this._config.dwGoalValue;
			}
		}

		public override int Current
		{
			get
			{
				return this._current;
			}
		}

		public ExeTaskPhase(Activity owner, uint id, ResDT_WealConInfo config) : base(owner)
		{
			this._id = id;
			this._config = config;
			this._achieved = false;
			this._achieveInLimit = false;
			this._current = 0;
		}

		public override bool AchieveJump()
		{
			if (this._config.dwConType == 14u && this._config.ReachConParam.Length >= 2 && this._config.ReachConParam[0] == 2u)
			{
				uint num = this._config.ReachConParam[1];
				MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.set(2, 1, new uint[]
				{
					num
				});
				string cDNUrl = MonoSingleton<BannerImageSys>.GetInstance().GetCDNUrl(num);
				Debug.Log("share jump " + cDNUrl);
				if (!string.IsNullOrEmpty(cDNUrl))
				{
					MonoSingleton<IDIPSys>.GetInstance().ShareActivityTask(cDNUrl);
				}
				return true;
			}
			if (this._config.dwConType != 14u || this._config.ReachConParam.Length < 2 || this._config.ReachConParam[0] != 3u)
			{
				return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)this._config.dwJumpEntry, 0, 0, null);
			}
			uint num2 = this._config.ReachConParam[1];
			if (this._config.ReachConParam[1] <= 0u)
			{
				return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)this._config.dwJumpEntry, 0, 0, null);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)this._config.dwJumpEntry, 0, 0, null);
			}
			bool flag = masterRoleInfo.IsHaveHero(num2, false);
			if (flag)
			{
				MonoSingleton<ShareSys>.GetInstance().RequestShareHeroSkinForm(num2, 0u, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO);
				return true;
			}
			flag = masterRoleInfo.IsHaveHeroSkin(num2, false);
			if (flag)
			{
				MonoSingleton<ShareSys>.GetInstance().RequestShareHeroSkinForm(0u, num2, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN);
				return true;
			}
			return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)this._config.dwJumpEntry, 0, 0, null);
		}

		internal void SetCurrent(int val)
		{
			if (val != this._current)
			{
				this._current = val;
				base._NotifyStateChanged();
			}
		}

		internal void SetAchiveve(bool achieved, bool achieveInLimit)
		{
			if (this._achieved != achieved || this._achieveInLimit != achieveInLimit)
			{
				this._achieved = achieved;
				this._achieveInLimit = achieveInLimit;
				base._NotifyStateChanged();
			}
		}
	}
}

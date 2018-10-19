using Assets.Scripts.UI;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class MultiGainPhase : ActivityPhase
	{
		private uint _id;

		private ResDT_WealMultiplePeriod _config;

		internal ushort _usedTimes;

		public override uint ID
		{
			get
			{
				return this._id;
			}
		}

		public string Desc
		{
			get
			{
				return Utility.UTF8Convert(this._config.szDesc);
			}
		}

		public override uint MultipleTimes
		{
			get
			{
				return this._config.dwMultipleRatio / 10000u;
			}
		}

		public RES_WEAL_GAME_TYPE GameType
		{
			get
			{
				return (RES_WEAL_GAME_TYPE)this._config.dwGameType;
			}
		}

		public ushort LimitTimes
		{
			get
			{
				return this._config.wLimitTimes;
			}
		}

		public ushort RemainTimes
		{
			get
			{
				return (ushort)(this.LimitTimes - this._usedTimes);
			}
		}

		public override uint RewardID
		{
			get
			{
				return 0u;
			}
		}

		public override int StartTime
		{
			get
			{
				return (int)this._config.dwStartTime;
			}
		}

		public override int CloseTime
		{
			get
			{
				return (int)this._config.dwEndTime;
			}
		}

		public override bool ReadyForGet
		{
			get
			{
				return false;
			}
		}

		public bool ReadyForGo
		{
			get
			{
				return base.ReadyForGet && ((this.LimitTimes > 0 && this.RemainTimes > 0) || this.LimitTimes == 0);
			}
		}

		public MultiGainPhase(Activity owner, uint id, ResDT_WealMultiplePeriod config) : base(owner)
		{
			this._id = id;
			this._config = config;
			this._usedTimes = 0;
		}

		public bool HasSubGameType(int subType)
		{
			return (this._config.dwGameSubTypeMask & 1u << subType) > 0u;
		}

		public override bool AchieveJump()
		{
			switch (this.GameType)
			{
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ADVENTURE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Adv_OpenChapterForm);
				return true;
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ACTIVITY:
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ARENA:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Explore_OpenForm);
				return true;
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_PVP_MATCH:
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_PVP_ROOM:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
				return true;
			case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_BURNING:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
				return true;
			}
			return false;
		}
	}
}

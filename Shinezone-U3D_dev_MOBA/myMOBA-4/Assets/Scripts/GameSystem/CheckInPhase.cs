using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CheckInPhase : ActivityPhase
	{
		private uint _id;

		private ResDT_WealCheckInDay _config;

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
				return this._config.dwRewardID;
			}
		}

		public override bool InMultipleTime
		{
			get
			{
				return false;
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
				return Singleton<CTextManager>.GetInstance().GetText("CheckInTips").Replace("{0}", this.Target.ToString());
			}
		}

		public override bool ReadyForGet
		{
			get
			{
				if (base.ReadyForGet)
				{
					CheckInActivity checkInActivity = (CheckInActivity)base.Owner;
					if ((long)checkInActivity.Current == (long)((ulong)this.ID))
					{
						DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
						DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)checkInActivity.LastCheckTime));
						if (checkInActivity.LastCheckTime == 0u || dateTime.DayOfYear != dateTime2.DayOfYear)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool ReadyForFill
		{
			get
			{
				CheckInActivity checkInActivity = (CheckInActivity)base.Owner;
				if (checkInActivity.CanFillIn && base.ReadyForGet && (long)checkInActivity.Current == (long)((ulong)this.ID))
				{
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)checkInActivity.LastCheckTime));
					if (this.ID < (uint)dateTime.Day && checkInActivity.LastCheckTime != 0u && dateTime.DayOfYear == dateTime2.DayOfYear)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CheckInPhase(Activity owner, uint id, ResDT_WealCheckInDay config) : base(owner)
		{
			this._id = id;
			this._config = config;
		}

		public override uint GetVipAddition(int vipFlagBit)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				int[] array = new int[]
				{
					1,
					16
				};
				uint num = 0u;
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = array[i];
					if ((vipFlagBit == 0 || vipFlagBit == num2) && this.HasVipAddition(num2) && masterRoleInfo.HasVip(num2))
					{
						num += base.Owner.GetVipAddition(num2);
						if (vipFlagBit != 0)
						{
							break;
						}
					}
				}
				return num;
			}
			return base.GetVipAddition(vipFlagBit);
		}

		public bool HasVipAddition(int vipFlagBit)
		{
			return (this._config.dwMultipleMask & (uint)vipFlagBit) > 0u;
		}

		public uint GetGameVipDoubleLv()
		{
			return this._config.dwMultipleVipLvl;
		}
	}
}

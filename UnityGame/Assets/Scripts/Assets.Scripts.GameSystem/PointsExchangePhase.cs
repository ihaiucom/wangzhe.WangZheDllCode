using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class PointsExchangePhase : ActivityPhase
	{
		private uint _id;

		public ResDT_PointExchange Config;

		internal ushort _usedTimes;

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
				return 0u;
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

		public override bool ReadyForGet
		{
			get
			{
				bool result = false;
				if (base.Owner.timeState != Activity.TimeState.Going || this.Config.bIsShowHotSpot == 0)
				{
					return false;
				}
				PointsExchangeActivity pointsExchangeActivity = base.Owner as PointsExchangeActivity;
				if (pointsExchangeActivity != null)
				{
					uint maxExchangeCount = pointsExchangeActivity.GetMaxExchangeCount((int)this.ID);
					uint exchangeCount = pointsExchangeActivity.GetExchangeCount((int)this.ID);
					uint dwPointCnt = this.Config.dwPointCnt;
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					result = (masterRoleInfo.JiFen >= dwPointCnt && (maxExchangeCount == 0u || exchangeCount < maxExchangeCount));
				}
				return result;
			}
		}

		public PointsExchangePhase(Activity owner, uint id, ResDT_PointExchange config) : base(owner)
		{
			this._id = id;
			this.Config = config;
			this._usedTimes = 0;
		}

		public int GetMaxExchangeCount()
		{
			int num = 0;
			if (base.Owner.timeState != Activity.TimeState.Going)
			{
				return 0;
			}
			PointsExchangeActivity pointsExchangeActivity = base.Owner as PointsExchangeActivity;
			if (pointsExchangeActivity != null)
			{
				uint maxExchangeCount = pointsExchangeActivity.GetMaxExchangeCount((int)this.ID);
				uint exchangeCount = pointsExchangeActivity.GetExchangeCount((int)this.ID);
				uint dwPointCnt = this.Config.dwPointCnt;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				num = (int)(masterRoleInfo.JiFen / dwPointCnt);
				if (maxExchangeCount > 0u)
				{
					num = Math.Min(num, (int)(maxExchangeCount - exchangeCount));
				}
			}
			return num;
		}
	}
}

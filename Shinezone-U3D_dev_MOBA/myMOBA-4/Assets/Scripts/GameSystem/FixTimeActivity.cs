using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class FixTimeActivity : Activity
	{
		private ResWealFixedTime _config;

		private bool _inMultipleTime;

		public override uint ID
		{
			get
			{
				return this._config.dwID;
			}
		}

		public override COM_WEAL_TYPE Type
		{
			get
			{
				return COM_WEAL_TYPE.COM_WEAL_FIXEDTIME;
			}
		}

		public override uint MultipleTimes
		{
			get
			{
				return this._config.dwMultipleRatio / 10000u;
			}
		}

		public override bool InMultipleTime
		{
			get
			{
				return this._inMultipleTime;
			}
		}

		public FixTimeActivity(ActivitySys mgr, ResWealFixedTime config) : base(mgr, config.stCommon)
		{
			this._config = config;
			this._inMultipleTime = false;
			ushort num = 0;
			while (num < this._config.wPeriodNum && (int)num < this._config.astPeriod.Length)
			{
				FixTimePhase ap = new FixTimePhase(this, (uint)num, this._config.astPeriod[(int)num]);
				base.AddPhase(ap);
				num += 1;
			}
		}

		public override bool CheckTimeState()
		{
			DateTime t = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			bool flag = false;
			for (int i = 0; i < this._config.astMultipleTime.Length; i++)
			{
				ResDT_DateTime resDT_DateTime = this._config.astMultipleTime[i];
				if (resDT_DateTime.ullStartTime > 0uL && resDT_DateTime.ullEndTime > 0uL && t >= Utility.ToUtcTime2Local((long)resDT_DateTime.ullStartTime) && t < Utility.ToUtcTime2Local((long)resDT_DateTime.ullEndTime))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = flag != this._inMultipleTime;
			this._inMultipleTime = flag;
			bool flag3 = base.CheckTimeState();
			if (!flag3 && flag2)
			{
				base.NotifyTimeStateChanged();
			}
			return flag3 || flag2;
		}

		public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
		{
			base.SetPhaseMarks(actvInfo.stFixedTime.ullRewardMask);
		}
	}
}

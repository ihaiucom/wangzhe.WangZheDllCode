using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class FixTimePhase : ActivityPhase
	{
		private uint _id;

		private ResDT_WealFixedTimeReward _config;

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

		public override bool InMultipleTime
		{
			get
			{
				return this._config.bIsMultiple == 1;
			}
		}

		public FixTimePhase(Activity owner, uint id, ResDT_WealFixedTimeReward config) : base(owner)
		{
			this._id = id;
			this._config = config;
		}
	}
}

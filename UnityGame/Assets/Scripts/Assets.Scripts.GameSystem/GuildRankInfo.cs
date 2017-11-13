using System;

namespace Assets.Scripts.GameSystem
{
	[Serializable]
	public class GuildRankInfo
	{
		public uint totalRankPoint;

		public uint seasonStartTime;

		public uint weekRankPoint;

		public GuildRankInfo()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.totalRankPoint = 0u;
			this.seasonStartTime = 0u;
			this.weekRankPoint = 0u;
		}
	}
}

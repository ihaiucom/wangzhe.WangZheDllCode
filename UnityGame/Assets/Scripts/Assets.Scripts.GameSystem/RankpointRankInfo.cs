using System;

namespace Assets.Scripts.GameSystem
{
	public class RankpointRankInfo
	{
		public ulong guildId;

		public uint rankNo;

		public uint rankScore;

		public uint guildHeadId;

		public string guildName;

		public byte guildLevel;

		public byte memberNum;

		public uint star;

		public RankpointRankInfo()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.guildId = 0uL;
			this.rankNo = 0u;
			this.rankScore = 0u;
			this.guildHeadId = 0u;
			this.guildName = null;
			this.guildLevel = 0;
			this.memberNum = 0;
			this.star = 0u;
		}
	}
}

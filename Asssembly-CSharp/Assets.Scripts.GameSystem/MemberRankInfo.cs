using System;

namespace Assets.Scripts.GameSystem
{
	public class MemberRankInfo
	{
		public uint maxRankPoint;

		public uint totalRankPoint;

		public uint killCnt;

		public uint deadCnt;

		public uint assistCnt;

		public uint weekRankPoint;

		public uint byGameRankPoint;

		public bool isSigned;

		public MemberRankInfo()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.maxRankPoint = 0u;
			this.totalRankPoint = 0u;
			this.killCnt = 0u;
			this.deadCnt = 0u;
			this.assistCnt = 0u;
			this.weekRankPoint = 0u;
			this.byGameRankPoint = 0u;
			this.isSigned = false;
		}
	}
}

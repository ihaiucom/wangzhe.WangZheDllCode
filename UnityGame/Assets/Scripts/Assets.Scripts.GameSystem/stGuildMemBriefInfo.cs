using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[Serializable]
	public struct stGuildMemBriefInfo
	{
		public ulong uulUid;

		public int dwLogicWorldId;

		public string sName;

		public uint dwLevel;

		public uint dwAbility;

		public string szHeadUrl;

		public uint dwGameEntity;

		public stVipInfo stVip;

		public uint dwClassOfRank;

		public byte gender;

		public int mentorLvl;

		public COMDT_RANKGRADE rankGrade;

		public void Reset()
		{
			this.uulUid = 0uL;
			this.dwLogicWorldId = 0;
			this.sName = null;
			this.dwLevel = 0u;
			this.dwAbility = 0u;
			this.dwGameEntity = 0u;
			this.stVip.Reset();
			this.dwClassOfRank = 0u;
			this.gender = 0;
		}
	}
}

using System;

namespace Assets.Scripts.GameSystem
{
	public struct stPrepareGuildBriefInfo
	{
		public ulong uulUid;

		public int dwLogicWorldId;

		public string sName;

		public byte bMemCnt;

		public uint dwHeadId;

		public uint dwRequestTime;

		public stGuildMemBriefInfo stCreatePlayer;

		public string sBulletin;

		public bool IsOnlyFriend;

		public void Reset()
		{
			this.uulUid = 0uL;
			this.dwLogicWorldId = 0;
			this.sName = null;
			this.bMemCnt = 0;
			this.dwHeadId = 0u;
			this.dwRequestTime = 0u;
			this.sBulletin = null;
			this.IsOnlyFriend = false;
			this.stCreatePlayer.Reset();
		}
	}
}

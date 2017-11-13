using System;

namespace Assets.Scripts.GameSystem
{
	public class PrepareGuildInfo
	{
		public stPrepareGuildBriefInfo stBriefInfo;

		public ListView<GuildMemInfo> m_MemList;

		public PrepareGuildInfo()
		{
			this.m_MemList = new ListView<GuildMemInfo>();
		}

		public void Reset()
		{
			this.stBriefInfo.Reset();
			this.m_MemList.Clear();
		}
	}
}

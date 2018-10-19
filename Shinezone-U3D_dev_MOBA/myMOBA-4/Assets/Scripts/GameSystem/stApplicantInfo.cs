using System;

namespace Assets.Scripts.GameSystem
{
	public struct stApplicantInfo
	{
		public stGuildMemBriefInfo stBriefInfo;

		public int dwApplyTime;

		public void Reset()
		{
			this.stBriefInfo.Reset();
			this.dwApplyTime = 0;
		}
	}
}

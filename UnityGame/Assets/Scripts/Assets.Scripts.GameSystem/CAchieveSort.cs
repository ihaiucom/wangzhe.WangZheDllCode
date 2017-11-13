using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CAchieveSort : IComparer<CAchieveItem2>
	{
		public int Compare(CAchieveItem2 l, CAchieveItem2 r)
		{
			uint mostRecentlyModifyTime = l.GetMostRecentlyModifyTime();
			uint mostRecentlyModifyTime2 = r.GetMostRecentlyModifyTime();
			return (mostRecentlyModifyTime <= mostRecentlyModifyTime2) ? 1 : -1;
		}
	}
}

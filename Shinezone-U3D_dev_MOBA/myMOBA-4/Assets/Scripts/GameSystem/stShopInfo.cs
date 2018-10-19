using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class stShopInfo
	{
		public RES_SHOP_TYPE enShopType;

		public string sName;

		public int dwAutoRefreshTime;

		public int dwManualRefreshTime;

		public int dwApRefreshTime;

		public int dwManualRefreshCnt;

		public int dwManualRefreshLimit;

		public int dwMaxRefreshTime;

		public int dwItemValidTime;

		public bool bManualRefreshSent;
	}
}

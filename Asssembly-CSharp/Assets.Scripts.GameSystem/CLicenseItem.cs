using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CLicenseItem
	{
		public uint m_licenseId;

		public ResLicenseInfo m_resLicenseInfo;

		public uint m_getSecond;

		public CLicenseItem(uint cfgId)
		{
			this.m_licenseId = cfgId;
			this.m_resLicenseInfo = GameDataMgr.licenseDatabin.GetDataByKey(cfgId);
			if (this.m_resLicenseInfo == null)
			{
				return;
			}
			this.m_getSecond = 0u;
		}
	}
}

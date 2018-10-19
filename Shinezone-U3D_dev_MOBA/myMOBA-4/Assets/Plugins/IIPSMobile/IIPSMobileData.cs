using System;
using System.Text;

namespace IIPSMobile
{
	public class IIPSMobileData
	{
		public IIPSMobileDataMgrInterface CreateDataMgr(string config)
		{
			IIPSMobileDataManager iIPSMobileDataManager = new IIPSMobileDataManager();
			if (!iIPSMobileDataManager.Init((uint)config.Length, Encoding.ASCII.GetBytes(config)))
			{
				return null;
			}
			return iIPSMobileDataManager;
		}
	}
}

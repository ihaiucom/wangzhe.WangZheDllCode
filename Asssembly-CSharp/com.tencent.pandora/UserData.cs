using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
	public class UserData
	{
		public string sRoleId = string.Empty;

		public string sOpenId = string.Empty;

		public string sServiceType = string.Empty;

		public string sAcountType = string.Empty;

		public string sArea = string.Empty;

		public string sPartition = string.Empty;

		public string sAppId = string.Empty;

		public string sAccessToken = string.Empty;

		public string sPayToken = string.Empty;

		public string sGameVer = string.Empty;

		public string sPlatID = string.Empty;

		public string sQQInstalled = string.Empty;

		public string sWXInstalled = string.Empty;

		public string sGameName = string.Empty;

		public bool IsRoleEmpty()
		{
			Logger.DEBUG(string.Empty);
			return string.IsNullOrEmpty(this.sRoleId);
		}

		public void Clear()
		{
			Logger.DEBUG(string.Empty);
			this.sRoleId = string.Empty;
			this.sOpenId = string.Empty;
			this.sServiceType = string.Empty;
			this.sAcountType = string.Empty;
			this.sArea = string.Empty;
			this.sPartition = string.Empty;
			this.sAppId = string.Empty;
			this.sAccessToken = string.Empty;
			this.sPayToken = string.Empty;
			this.sGameVer = string.Empty;
			this.sPlatID = string.Empty;
			this.sQQInstalled = string.Empty;
			this.sWXInstalled = string.Empty;
			this.sGameName = string.Empty;
		}

		public void Assgin(Dictionary<string, string> dictPara)
		{
			Logger.DEBUG(string.Empty);
			if (dictPara.ContainsKey("sRoleId"))
			{
				this.sRoleId = dictPara.get_Item("sRoleId");
			}
			if (dictPara.ContainsKey("sOpenId"))
			{
				this.sOpenId = dictPara.get_Item("sOpenId");
			}
			if (dictPara.ContainsKey("sServiceType"))
			{
				this.sServiceType = dictPara.get_Item("sServiceType");
			}
			if (dictPara.ContainsKey("sAcountType"))
			{
				this.sAcountType = dictPara.get_Item("sAcountType");
			}
			if (dictPara.ContainsKey("sArea"))
			{
				this.sArea = dictPara.get_Item("sArea");
			}
			if (dictPara.ContainsKey("sPartition"))
			{
				this.sPartition = dictPara.get_Item("sPartition");
			}
			if (dictPara.ContainsKey("sAppId"))
			{
				this.sAppId = dictPara.get_Item("sAppId");
			}
			if (dictPara.ContainsKey("sAccessToken"))
			{
				this.sAccessToken = dictPara.get_Item("sAccessToken");
			}
			if (dictPara.ContainsKey("sPayToken"))
			{
				this.sPayToken = dictPara.get_Item("sPayToken");
			}
			if (dictPara.ContainsKey("sGameVer"))
			{
				this.sGameVer = dictPara.get_Item("sGameVer");
			}
			if (dictPara.ContainsKey("sPlatID"))
			{
				this.sPlatID = dictPara.get_Item("sPlatID");
			}
			if (dictPara.ContainsKey("sQQInstalled"))
			{
				this.sQQInstalled = dictPara.get_Item("sQQInstalled");
			}
			if (dictPara.ContainsKey("sWXInstalled"))
			{
				this.sWXInstalled = dictPara.get_Item("sWXInstalled");
			}
			if (dictPara.ContainsKey("sGameName"))
			{
				this.sGameName = dictPara.get_Item("sGameName");
			}
		}

		public void Refresh(Dictionary<string, string> dictPara)
		{
			Logger.DEBUG(string.Empty);
			string text = dictPara.get_Item("sRoleId");
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(this.sRoleId) || text != this.sRoleId)
			{
				Logger.ERROR("role id changed");
				return;
			}
			if (dictPara.ContainsKey("sAccessToken"))
			{
				this.sAccessToken = dictPara.get_Item("sAccessToken");
			}
			if (dictPara.ContainsKey("sPayToken"))
			{
				this.sPayToken = dictPara.get_Item("sPayToken");
			}
		}
	}
}

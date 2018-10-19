using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CLicenseInfo
	{
		public ListView<CLicenseItem> m_licenseList = new ListView<CLicenseItem>();

		public void InitLicenseCfgInfo()
		{
			this.m_licenseList.Clear();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.licenseDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResLicenseInfo resLicenseInfo = (ResLicenseInfo)current.Value;
				CLicenseItem item = new CLicenseItem(resLicenseInfo.dwLicenseID);
				this.m_licenseList.Add(item);
			}
		}

		public void SetSvrLicenseData(COMDT_ACNT_LICENSE svrLicenseData)
		{
			if (this.m_licenseList.Count == 0)
			{
				this.InitLicenseCfgInfo();
			}
			for (int i = 0; i < (int)svrLicenseData.bLicenseCnt; i++)
			{
				this.SetLicenseItemData(svrLicenseData.astLicenseList[i].dwLicenseID, svrLicenseData.astLicenseList[i].dwGetSecond);
			}
		}

		public void SetLicenseItemData(uint licenseId, uint getSec)
		{
			for (int i = 0; i < this.m_licenseList.Count; i++)
			{
				if (licenseId == this.m_licenseList[i].m_licenseId)
				{
					this.m_licenseList[i].m_getSecond = getSec;
					return;
				}
			}
		}

		public CLicenseItem GetLicenseItemByIndex(int index)
		{
			if (index >= 0 && index < this.m_licenseList.Count)
			{
				return this.m_licenseList[index];
			}
			return null;
		}

		public bool CheckGetLicense(uint cfgId)
		{
			ResLicenseInfo dataByKey = GameDataMgr.licenseDatabin.GetDataByKey(cfgId);
			bool flag = false;
			if (dataByKey != null)
			{
				for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
				{
					if (dataByKey.UnlockArray[i] > 0u)
					{
						if (dataByKey.bIsAnd > 0)
						{
							flag &= Singleton<CFunctionUnlockSys>.GetInstance().CheckUnlock(dataByKey.UnlockArray[i]);
						}
						else
						{
							flag |= Singleton<CFunctionUnlockSys>.GetInstance().CheckUnlock(dataByKey.UnlockArray[i]);
						}
					}
				}
			}
			return flag;
		}

		public void ReviewLicenseList()
		{
			for (int i = 0; i < this.m_licenseList.Count; i++)
			{
				if (this.m_licenseList[i].m_getSecond == 0u && this.CheckGetLicense(this.m_licenseList[i].m_licenseId))
				{
					CLicenseInfo.ReqGetLicense(this.m_licenseList[i].m_licenseId);
				}
			}
		}

		public static void ReqGetLicense(uint licenseId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1350u);
			cSPkg.stPkgData.stLicenseGetReq.dwLicenseID = licenseId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(1351)]
		public static void ReceiveLicenseGetRsp(CSPkg msg)
		{
			SCPKG_CMD_LICENSE_RSP stLicenseGetRsp = msg.stPkgData.stLicenseGetRsp;
			if (stLicenseGetRsp.iResult == 0)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					masterRoleInfo.m_licenseInfo.SetLicenseItemData(stLicenseGetRsp.dwLicenseID, stLicenseGetRsp.dwLicenseTime);
				}
			}
			else
			{
				//switch (stLicenseGetRsp.iResult)
				//{
				//}
			}
		}
	}
}

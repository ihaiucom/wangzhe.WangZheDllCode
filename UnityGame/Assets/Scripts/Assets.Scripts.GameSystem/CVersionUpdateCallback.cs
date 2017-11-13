using IIPSMobile;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CVersionUpdateCallback : IIPSMobileVersionCallBackInterface
	{
		public delegate byte OnGetNewVersionInfoDelegate(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo);

		public delegate void OnProgressDelegate(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);

		public delegate void OnErrorDelegate(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);

		public delegate void OnSuccessDelegate();

		public delegate byte OnNoticeInstallApkDelegate(string path);

		public delegate byte OnActionMsgDelegate(string msg);

		public CVersionUpdateCallback.OnGetNewVersionInfoDelegate m_onGetNewVersionInfoDelegate;

		public CVersionUpdateCallback.OnProgressDelegate m_onProgressDelegate;

		public CVersionUpdateCallback.OnErrorDelegate m_onErrorDelegate;

		public CVersionUpdateCallback.OnSuccessDelegate m_onSuccessDelegate;

		public CVersionUpdateCallback.OnNoticeInstallApkDelegate m_onNoticeInstallApkDelegate;

		public CVersionUpdateCallback.OnActionMsgDelegate m_onActionMsgDelegate;

		public byte OnGetNewVersionInfo(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo)
		{
			if (this.m_onGetNewVersionInfoDelegate != null)
			{
				this.m_onGetNewVersionInfoDelegate(newVersionInfo);
			}
			return 1;
		}

		public void OnProgress(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
		{
			if (this.m_onProgressDelegate != null)
			{
				this.m_onProgressDelegate(curVersionStage, totalSize, nowSize);
			}
		}

		public void OnError(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode)
		{
			if (this.m_onErrorDelegate != null)
			{
				this.m_onErrorDelegate(curVersionStage, errorCode);
			}
		}

		public void OnSuccess()
		{
			if (this.m_onSuccessDelegate != null)
			{
				this.m_onSuccessDelegate();
			}
		}

		public void SaveConfig(uint bufferSize, IntPtr configBuffer)
		{
		}

		public byte OnNoticeInstallApk(string path)
		{
			if (this.m_onNoticeInstallApkDelegate != null)
			{
				this.m_onNoticeInstallApkDelegate(path);
			}
			return 1;
		}

		public byte OnActionMsg(string msg)
		{
			if (this.m_onActionMsgDelegate != null)
			{
				this.m_onActionMsgDelegate(msg);
			}
			return 1;
		}
	}
}

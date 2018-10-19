using System;

namespace IIPSMobile
{
	public interface IIPSMobileVersionCallBackInterface
	{
		byte OnGetNewVersionInfo(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo);

		void OnProgress(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);

		void OnError(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);

		void OnSuccess();

		void SaveConfig(uint bufferSize, IntPtr configBuffer);

		byte OnNoticeInstallApk(string path);

		byte OnActionMsg(string msg);
	}
}

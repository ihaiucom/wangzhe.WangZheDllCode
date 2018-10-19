using System;

namespace IIPSMobile
{
	public interface IIPSMobileVersionMgrInterface
	{
		bool MgrUnitVersionManager();

		bool MgrSetNextStage(bool goonWork);

		bool MgrCheckAppUpdate();

		void MgrCancelUpdate();

		short MgrGetCurDataVersion();

		uint MgrGetVersionMgrLastError();

		ulong MgrGetMemorySize();

		uint MgrGetActionDownloadSpeed();

		bool MgrPoll();

		bool InstallApk(string path);
	}
}

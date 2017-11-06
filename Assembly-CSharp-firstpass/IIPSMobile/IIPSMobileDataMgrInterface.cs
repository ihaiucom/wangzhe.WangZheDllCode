using System;

namespace IIPSMobile
{
	public interface IIPSMobileDataMgrInterface
	{
		bool Uninit();

		IIPSMobileDataReaderInterface GetDataReader();

		IIPSMobileDownloaderInterface GetDataDownloader(bool openProgressCallBack = false);

		IIPSMobileDataQueryInterface GetDataQuery();

		uint MgrGetDataMgrLastError();

		ulong MgrGetMemorySize();

		bool PollCallBack();
	}
}

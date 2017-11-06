using System;

namespace IIPSMobile
{
	public interface IIPSMobileDownloadCallbackInterface
	{
		void OnDownloadError(uint taskId, uint errorCode);

		void OnDownloadSuccess(uint taskId);

		void OnDownloadProgress(uint taskId, DataDownloader.DownloadInfo info);
	}
}

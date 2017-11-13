using System;

namespace IIPSMobile
{
	public interface IIPSMobileDownloaderInterface
	{
		bool Init(IIPSMobileDownloadCallbackInterface callback);

		bool StartDownload();

		bool PauseDownload();

		bool ResumeDownload();

		bool CancelDownload(uint taskId);

		uint GetDownloadSpeed();

		bool SetDownloadSpeed(uint downloadSpeed);

		bool DownloadIfsData(uint fileId, byte priority, ref uint taskId);

		bool DownloadLocalData(string downloadUrl, string savePath, byte priority, ref uint taskID, bool bDoBrokenResume);

		bool GetDownloadTaskInfo(uint taskId, ref DataDownloader.DownloadInfo downloadInfo);

		uint GetLastError();
	}
}

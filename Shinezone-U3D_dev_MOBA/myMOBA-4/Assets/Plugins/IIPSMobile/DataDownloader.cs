using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class DataDownloader : IIPSMobileDownloaderInterface
	{
		public struct DownloadInfo
		{
			public ulong needDownloadSize;

			public ulong downloadSize;

			public ulong fileSize;
		}

		public IntPtr mDownloader = IntPtr.Zero;

		private DownloadCallBack mCallback;

		public DataDownloader(IntPtr downloader)
		{
			this.mDownloader = downloader;
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte InitDataDownloader(IntPtr dataDownloader, IntPtr callback);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte StartDownload(IntPtr dataDownloader);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte PauseDownload(IntPtr dataDownloader);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte ResumeDonload(IntPtr dataDownloader);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetDownloadSpeed(IntPtr dataDownloader);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte SetDownloadSpeed(IntPtr dataDownloader, uint downloadSpeed);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte DownloadIfsData(IntPtr dataDownloader, uint fileId, byte priority, ref uint taskId);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte DownloadLocalData(IntPtr dataDownloader, [MarshalAs(UnmanagedType.LPStr)] string downloadurl, [MarshalAs(UnmanagedType.LPStr)] string filepath, byte priority, ref uint TaskID, byte bDoBrokenResume);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte GetDownloadTaskInfo(IntPtr dataDownloader, uint taskId, ref DataDownloader.DownloadInfo downloadInfo);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetLastDownloaderError(IntPtr dataDownloader);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte CancelDownload(IntPtr dataDownloader, uint taskId);

		public bool Init(IIPSMobileDownloadCallbackInterface callback)
		{
			if (this.mDownloader == IntPtr.Zero)
			{
				return false;
			}
			this.mCallback = new DownloadCallBack(callback);
			return DataDownloader.InitDataDownloader(this.mDownloader, this.mCallback.mCallBack) > 0;
		}

		public bool StartDownload()
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.StartDownload(this.mDownloader) > 0;
		}

		public bool PauseDownload()
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.PauseDownload(this.mDownloader) > 0;
		}

		public bool ResumeDownload()
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.ResumeDonload(this.mDownloader) > 0;
		}

		public bool CancelDownload(uint taskId)
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.CancelDownload(this.mDownloader, taskId) > 0;
		}

		public uint GetDownloadSpeed()
		{
			if (this.mDownloader == IntPtr.Zero)
			{
				return 0u;
			}
			return DataDownloader.GetDownloadSpeed(this.mDownloader);
		}

		public bool SetDownloadSpeed(uint downloadSpeed)
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.SetDownloadSpeed(this.mDownloader, downloadSpeed) > 0;
		}

		public bool DownloadIfsData(uint fileId, byte priority, ref uint taskId)
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.DownloadIfsData(this.mDownloader, fileId, priority, ref taskId) > 0;
		}

		public bool GetDownloadTaskInfo(uint taskId, ref DataDownloader.DownloadInfo downloadInfo)
		{
			return !(this.mDownloader == IntPtr.Zero) && DataDownloader.GetDownloadTaskInfo(this.mDownloader, taskId, ref downloadInfo) > 0;
		}

		public uint GetLastError()
		{
			if (this.mDownloader == IntPtr.Zero)
			{
				return 0u;
			}
			return DataDownloader.GetLastDownloaderError(this.mDownloader);
		}

		public bool DownloadLocalData(string downloadUrl, string savePath, byte priority, ref uint taskID, bool bDoBrokenResume)
		{
			if (this.mDownloader == IntPtr.Zero)
			{
				return false;
			}
			byte bDoBrokenResume2 = 0;
			if (bDoBrokenResume)
			{
				bDoBrokenResume2 = 1;
			}
			return DataDownloader.DownloadLocalData(this.mDownloader, downloadUrl, savePath, priority, ref taskID, bDoBrokenResume2) > 0;
		}
	}
}

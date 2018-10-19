using System;

namespace Assets.Scripts.GameSystem
{
	public enum enTMAssistantDownloadTaskState
	{
		DownloadSDKTaskState_WAITING = 1,
		DownloadSDKTaskState_DOWNLOADING,
		DownloadSDKTaskState_PAUSED,
		DownloadSDKTaskState_SUCCEED,
		DownloadSDKTaskState_FAILED,
		DownloadSDKTaskState_DELETE
	}
}

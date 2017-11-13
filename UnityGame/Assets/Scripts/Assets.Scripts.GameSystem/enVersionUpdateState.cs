using System;

namespace Assets.Scripts.GameSystem
{
	public enum enVersionUpdateState
	{
		None,
		StartCheckPathPermission,
		CheckPathPermission,
		StartCheckAppVersion,
		CheckAppVersion,
		DownloadApp,
		InstallApp,
		FinishUpdateApp,
		DownloadYYB,
		StartCheckFirstExtractResource,
		CheckFirstExtractResource,
		FirstExtractResource,
		FinishFirstExtractResouce,
		StartCheckResourceVersion,
		CheckResourceVersion,
		DownloadResource,
		FinishUpdateResource,
		Complete,
		End
	}
}

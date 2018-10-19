using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CYYBUpdateObserver : MonoBehaviour
	{
		public static string s_gameObjectName = "CYYBUpdateObserver";

		public CYYBUpdateManager.OnYYBCheckNeedUpdateInfo m_onYYBCheckNeedUpdateInfoHandler = delegate
		{
		};

		public CYYBUpdateManager.OnDownloadYYBProgressChanged m_onDownloadYYBProgressChangedHandler = delegate
		{
		};

		public CYYBUpdateManager.OnDownloadYYBStateChanged m_onDownloadYYBStateChangedHandler = delegate
		{
		};

		public void HandleMsgOnYYBCheckNeedUpdateInfo(string msg)
		{
			this.m_onYYBCheckNeedUpdateInfoHandler(msg);
		}

		public void HandleMsgOnDownloadYYBProgressChanged(string msg)
		{
			this.m_onDownloadYYBProgressChangedHandler(msg);
		}

		public void HandleMsgOnDownloadYYBStateChanged(string msg)
		{
			this.m_onDownloadYYBStateChangedHandler(msg);
		}
	}
}

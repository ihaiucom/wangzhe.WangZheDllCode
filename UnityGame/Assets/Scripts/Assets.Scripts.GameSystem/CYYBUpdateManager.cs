using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CYYBUpdateManager
	{
		public delegate void OnYYBCheckNeedUpdateInfo(string msg);

		public delegate void OnDownloadYYBProgressChanged(string msg);

		public delegate void OnDownloadYYBStateChanged(string msg);

		private AndroidJavaClass m_androidUtilityJavaClass;

		public CYYBUpdateManager(CYYBUpdateManager.OnYYBCheckNeedUpdateInfo onYYBCheckNeedUpdateInfoHandler, CYYBUpdateManager.OnDownloadYYBProgressChanged onDownloadYYBProgressChangedHandler, CYYBUpdateManager.OnDownloadYYBStateChanged onDownloadYYBStateChangedHandler)
		{
			GameObject gameObject = new GameObject(CYYBUpdateObserver.s_gameObjectName);
			CYYBUpdateObserver cYYBUpdateObserver = gameObject.AddComponent<CYYBUpdateObserver>();
			CYYBUpdateObserver cYYBUpdateObserver2 = cYYBUpdateObserver;
			cYYBUpdateObserver2.m_onYYBCheckNeedUpdateInfoHandler = (CYYBUpdateManager.OnYYBCheckNeedUpdateInfo)Delegate.Combine(cYYBUpdateObserver2.m_onYYBCheckNeedUpdateInfoHandler, onYYBCheckNeedUpdateInfoHandler);
			CYYBUpdateObserver cYYBUpdateObserver3 = cYYBUpdateObserver;
			cYYBUpdateObserver3.m_onDownloadYYBProgressChangedHandler = (CYYBUpdateManager.OnDownloadYYBProgressChanged)Delegate.Combine(cYYBUpdateObserver3.m_onDownloadYYBProgressChangedHandler, onDownloadYYBProgressChangedHandler);
			CYYBUpdateObserver cYYBUpdateObserver4 = cYYBUpdateObserver;
			cYYBUpdateObserver4.m_onDownloadYYBStateChangedHandler = (CYYBUpdateManager.OnDownloadYYBStateChanged)Delegate.Combine(cYYBUpdateObserver4.m_onDownloadYYBStateChangedHandler, onDownloadYYBStateChangedHandler);
			this.m_androidUtilityJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
			this.m_androidUtilityJavaClass.CallStatic("AddYYBSaveUpdateListener", new object[0]);
		}

		public void StartYYBCheckVersionInfo()
		{
			this.m_androidUtilityJavaClass.CallStatic("StartYYBCheckVersionInfo", new object[0]);
		}

		public void StartYYBSaveUpdate()
		{
			this.m_androidUtilityJavaClass.CallStatic("StartYYBSaveUpdate", new object[0]);
		}

		public int CheckYYBInstalled()
		{
			return this.m_androidUtilityJavaClass.CallStatic<int>("CheckYYBInstalled", new object[0]);
		}
	}
}

using ApolloUpdate;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using IIPSMobile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameSystem
{
	public class CVersionUpdateSystem : MonoSingleton<CVersionUpdateSystem>
	{
		public delegate void OnVersionUpdateComplete ();

		private const string c_homePageUrl = "http://pvp.qq.com";
		private const float c_appYYBCheckVersionMaxWaitingTime = 10f;
		private CVersionUpdateSystem.OnVersionUpdateComplete m_onVersionUpdateComplete;
		private enVersionUpdateState m_versionUpdateState;
		private static AndroidJavaClass s_androidUtilityJavaClass;
		private static string s_downloadedIFSSavePath = null;
		private static string s_ifsExtractPath = null;
		private static string s_firstExtractIFSName = null;
		private static string s_firstExtractIFSPath = null;
		private static string s_resourcePackerInfoSetFullPath = null;
		private static enPlatform s_platform = CVersionUpdateSystem.GetPlatform ();
		private static uint s_appID = 1u;
		private static enAppType s_appType = enAppType.Unknown;
		private static string s_appVersion = null;
		private static string s_cachedResourceVersionFilePath = null;
		private string m_cachedResourceVersion;
		private string m_cachedResourceBuildNumber;
		private int m_cachedResourceType;
		private string m_firstExtractResourceVersion;
		private string m_firstExtractResourceBuildNumber;
		private static uint[] s_serviceIDsForUpdataApp = new uint[]
		{
			165675099u,
			165675098u
		};
		private static uint[] s_serviceIDsForUpdateResource = new uint[]
		{
			165675024u,
			165675023u
		};
		private static uint[] s_serviceIDsForUpdateCompetitionApp = new uint[]
		{
			165675167u,
			165675166u
		};
		private static uint[] s_serviceIDsForUpdateCompetitionResource = new uint[]
		{
			165675170u,
			165675169u
		};
		private static string[][] s_IIPSServerUrls = new string[][]
		{
			new string[]
			{
				"tcp://mtcls.qq.com:50011",
				"tcp://61.151.224.100:50011",
				"tcp://58.251.61.169:50011",
				"tcp://203.205.151.237:50011",
				"tcp://203.205.147.178:50011",
				"tcp://183.61.49.177:50011",
				"tcp://183.232.103.166:50011",
				"tcp://182.254.4.176:50011",
				"tcp://182.254.10.82:50011",
				"tcp://140.207.127.61:50011",
				"tcp://117.144.242.115:50011"
			},
			new string[]
			{
				"tcp://middle.mtcls.qq.com:20001",
				"tcp://101.226.141.88:20001"
			},
			new string[]
			{
				"tcp://testa4.mtcls.qq.com:10001",
				"tcp://101.227.153.83:10001"
			},
			new string[]
			{
				"tcp://exp.mtcls.qq.com:10001",
				"tcp://61.151.234.47:10001",
				"tcp://182.254.42.103:10001",
				"tcp://140.207.62.111:10001",
				"tcp://140.207.123.164:10001",
				"tcp://117.144.242.28:10001",
				"tcp://117.144.243.174:10001",
				"tcp://103.7.30.91:10001",
				"tcp://101.227.130.79:10001"
			},
			new string[]
			{
				"tcp://testb4.mtcls.qq.com:10001",
				"tcp://101.227.153.86:10001"
			},
			new string[]
			{
				"tcp://testc.mtcls.qq.com:10001",
				"tcp://183.61.39.51:10001"
			},
			new string[]
			{
				"tcp://exp.mtcls.qq.com:10011",
				"tcp://61.151.234.47:10011",
				"tcp://182.254.42.103:10011",
				"tcp://140.207.62.111:10011",
				"tcp://140.207.123.164:10011",
				"tcp://117.144.242.28:10011",
				"tcp://117.144.243.174:10011",
				"tcp://103.7.30.91:10011",
				"tcp://101.227.130.79:10011"
			},
			new string[]
			{
				"tcp://testa4.mtcls.qq.com:10001",
				"tcp://101.227.153.83:10001"
			},
			new string[]
			{
				string.Empty
			}
		};
		private static enIIPSServerType s_IIPSServerType = GameVersion.IIPSServerType;
		private static bool s_logDebug = false;
		private IIPSMobileVersion m_IIPSVersionMgrFactory;
		private IIPSMobileVersionMgrInterface m_IIPSVersionMgr;
		private string m_downloadAppVersion;
		private string m_appDownloadUrl;
		private uint m_appDownloadSize;
		private uint m_appRecommendUpdateVersionMin;
		private uint m_appRecommendUpdateVersionMax;
		private bool m_appIsNoIFS;
		private string m_appSavePath;
		private bool m_appIsDelayToInstall;
		private CYYBUpdateManager m_appYYBUpdateManager;
		private int m_appEnableYYB;
		private float m_appYYBCheckVersionInfoStartTime;
		private bool m_appYYBCheckVersionInfoCallBackHandled;
		private string m_downloadResourceVersion;
		private bool m_resourceDownloadNeedConfirm;
		private bool m_resourceCheckCompatibility = true;
		private bool m_useCurseResourceDownloadSize;
		private uint m_resourceDownloadSize;
		private ListView<CAnnouncementInfo> m_announcementInfos = new ListView<CAnnouncementInfo> ();
		private bool m_isAnnouncementPanelOpened;
		private bool m_isError;
		private IIPSMobileErrorCodeCheck m_iipsErrorCodeChecker = new IIPSMobileErrorCodeCheck ();
		private static string s_versionUpdateFormPath = "UGUI/Form/System/VersionUpdate/Form_VersionUpdate.prefab";
		private static string s_waitingFormPath = "UGUI/Form/Common/Form_SendMsgAlert.prefab";
		private CUIFormScript m_versionUpdateFormScript;
		private ApolloUpdateSpeedCounter m_apolloUpdateSpeedCounter = new ApolloUpdateSpeedCounter ();
		private uint m_downloadCounter;
		private uint m_downloadSpeed;
		private DateTime m_reportAppDownloadStartTime = default(DateTime);
		private DateTime m_reportResourceDownloadStartTime = default(DateTime);
		private uint m_reportAppTotalDownloadSize;
		private uint m_reportAppDownloadSize;
		private uint m_reportResourceTotalDownloadSize;
		private uint m_reportResourceDownloadSize;
		private bool m_enablePreviousVersion;
		private uint m_previousKeyVersion;
		private bool m_previousKeyVersionLoaded;
		private bool s_isUpdateToPreviousVersion;

		private static string GetIIPSStreamingAssetsPath (string ifsName)
		{
			return string.Format("apk://{0}?assets/{1}", CVersionUpdateSystem.Android_GetApkAbsPath(), ifsName);
		}

		private static enPlatform GetPlatform ()
		{
			return enPlatform.Android;
		}

		private static bool I2B (int value)
		{
			return value > 0;
		}

		protected override void Init ()
		{
			CVersionUpdateSystem.s_downloadedIFSSavePath = CFileManager.GetCachePath ();
			CVersionUpdateSystem.s_ifsExtractPath = CFileManager.GetIFSExtractPath ();
			CVersionUpdateSystem.s_firstExtractIFSName = CFileManager.EraseExtension (CResourcePackerInfoSet.s_resourceIFSFileName) + ".png";
			CVersionUpdateSystem.s_firstExtractIFSPath = null;
			CVersionUpdateSystem.s_resourcePackerInfoSetFullPath = CFileManager.CombinePath (CVersionUpdateSystem.s_ifsExtractPath, CResourcePackerInfoSet.s_resourcePackerInfoSetFileName);
			CVersionUpdateSystem.s_appVersion = GameFramework.AppVersion;
			CVersionUpdateSystem.s_appType = enAppType.General;
			CVersionUpdateSystem.s_cachedResourceVersionFilePath = CFileManager.CombinePath (CFileManager.GetCachePath (), "Resource.bytes");
			this.m_versionUpdateState = enVersionUpdateState.None;
			this.m_cachedResourceVersion = CVersion.s_emptyResourceVersion;
			this.m_cachedResourceBuildNumber = CVersion.s_emptyBuildNumber;
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_JumpToHomePage, new CUIEventManager.OnUIEventHandler (this.OnJumpToHomePage));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_RetryCheckAppVersion, new CUIEventManager.OnUIEventHandler (this.OnRetryCheckApp));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_ConfirmUpdateApp, new CUIEventManager.OnUIEventHandler (this.OnConfirmUpdateApp));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_ConfirmUpdateAppNoWifi, new CUIEventManager.OnUIEventHandler (this.OnConfirmUpdateAppNoWifi));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_CancelUpdateApp, new CUIEventManager.OnUIEventHandler (this.OnCancelUpdateApp));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_QuitApp, new CUIEventManager.OnUIEventHandler (this.OnQuitApp));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, new CUIEventManager.OnUIEventHandler (this.OnRetryCheckFirstExtractResource));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_RetryCheckResourceVersion, new CUIEventManager.OnUIEventHandler (this.OnRetryCheckResourceVersion));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_ConfirmUpdateResource, new CUIEventManager.OnUIEventHandler (this.OnConfirmUpdateResource));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_ConfirmUpdateResourceNoWifi, new CUIEventManager.OnUIEventHandler (this.OnConfirmUpdateResourceNoWifi));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_ConfirmYYBSaveUpdateApp, new CUIEventManager.OnUIEventHandler (this.OnConfirmYYBSaveUpdateApp));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_OnAnnouncementListElementEnable, new CUIEventManager.OnUIEventHandler (this.OnAnnouncementListElementEnable));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_SwitchAnnouncementListElementToFront, new CUIEventManager.OnUIEventHandler (this.OnSwitchAnnouncementListElementToFront));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_SwitchAnnouncementListElementToBehind, new CUIEventManager.OnUIEventHandler (this.OnSwitchAnnouncementListElementToBehind));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_OnAnnouncementListSelectChanged, new CUIEventManager.OnUIEventHandler (this.OnAnnouncementListSelectChanged));
			Singleton<CUIEventManager>.GetInstance ().AddUIEventListener (enUIEventID.VersionUpdate_UpdateToPreviousVersion, new CUIEventManager.OnUIEventHandler (this.OnUpdateToPreviousVersion));
			CVersionUpdateSystem.s_androidUtilityJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
			CVersionUpdateSystem.s_firstExtractIFSPath = CVersionUpdateSystem.GetIIPSStreamingAssetsPath (CVersionUpdateSystem.s_firstExtractIFSName);
			this.m_enablePreviousVersion = false;
			this.m_previousKeyVersion = 0u;
			this.m_previousKeyVersionLoaded = false;
			this.s_isUpdateToPreviousVersion = false;
		}

		public static void SetIIPSServerType (enIIPSServerType iipsServerType)
		{
			CVersionUpdateSystem.s_IIPSServerType = iipsServerType;
		}

		public static bool IsEnableLogDebug ()
		{
			return CVersionUpdateSystem.s_logDebug;
		}

		public static void EnableLogDebug (bool enable)
		{
			CVersionUpdateSystem.s_logDebug = enable;
			Singleton<ApolloHelper>.GetInstance ().ChangeLogError (enable);
		}

		public static void SetIIPSServerTypeFromFile ()
		{
			TdirConfigData fileTdirAndTverData = TdirConfig.GetFileTdirAndTverData ();
			if (fileTdirAndTverData != null) {
				CVersionUpdateSystem.s_IIPSServerType = (enIIPSServerType)fileTdirAndTverData.versionType;
			}
		}

		public static enIIPSServerType GetIIPSServerType ()
		{
			return CVersionUpdateSystem.s_IIPSServerType;
		}

		public void StartVersionUpdate (CVersionUpdateSystem.OnVersionUpdateComplete onVersionUpdateComplete)
		{
			this.m_onVersionUpdateComplete = onVersionUpdateComplete;
			this.ReadCachedResourceInfo ();
			Singleton<CUIManager>.GetInstance ().OpenForm (CLoginSystem.s_splashFormPath, false, true);
			this.m_versionUpdateFormScript = Singleton<CUIManager>.GetInstance ().OpenForm (CVersionUpdateSystem.s_versionUpdateFormPath, false, true);
			this.UpdateUIVersionTextContent (CVersionUpdateSystem.s_appVersion, this.m_cachedResourceVersion);
			this.UpdateUIStateInfoTextContent (string.Empty);
			this.UpdateUIDownloadProgressTextContent (string.Empty);
			this.m_announcementInfos.Clear ();
			this.CloseAnnouncementPanel ();
			this.CloseConfirmPanel ();
			this.m_versionUpdateState = enVersionUpdateState.StartCheckPathPermission;
		}

		public void Update ()
		{
			switch (this.m_versionUpdateState) {
			case enVersionUpdateState.StartCheckPathPermission:
				this.StartCheckPathPermission ();
				break;
			case enVersionUpdateState.StartCheckAppVersion:
				this.StartCheckAppVersion ();
				break;
			case enVersionUpdateState.CheckAppVersion:
				this.CheckAppVersion ();
				break;
			case enVersionUpdateState.FinishUpdateApp:
				this.FinishUpdateApp ();
				break;
			case enVersionUpdateState.StartCheckFirstExtractResource:
				this.StartCheckFirstExtractResource ();
				break;
			case enVersionUpdateState.FinishFirstExtractResouce:
				this.FinishFirstExtractResource ();
				break;
			case enVersionUpdateState.StartCheckResourceVersion:
				this.StartCheckResourceVersion ();
				break;
			case enVersionUpdateState.FinishUpdateResource:
				this.FinishUpdateResource ();
				break;
			case enVersionUpdateState.Complete:
				this.Complete ();
				break;
			}
			this.UpdateIIPSVersionMgr ();
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_TotalProgress, enVersionUpdateFormWidget.Text_TotalPercent, (float)this.m_versionUpdateState / 16f);
		}

		public void Repair ()
		{
		}

		public bool ClearCachePath ()
		{
			return CFileManager.ClearDirectory (CFileManager.GetCachePath (), new string[]
			{
				".json",
				".flist",
				".res",
				".bytes"
			}, new string[]
			{
				CFileManager.s_ifsExtractFolder
			});
		}

		private void StartCheckPathPermission ()
		{
			if (string.IsNullOrEmpty (CFileManager.GetCachePath ())) {
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_PermissionFail"), enUIEventID.VersionUpdate_QuitApp, false);
				this.m_versionUpdateState = enVersionUpdateState.CheckPathPermission;
			} else if (CVersionUpdateSystem.s_IIPSServerType == enIIPSServerType.None) {
				this.ClearDownloadedApk ();
				this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
			} else {
				this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
			}
		}

		private void StartCheckAppVersion ()
		{
			Singleton<CUIManager>.GetInstance ().CloseMessageBox ();
			this.CloseConfirmPanel ();
			this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_CheckAppVersion"));
			this.UpdateUIDownloadProgressTextContent (string.Empty);
			this.m_downloadAppVersion = string.Empty;
			this.m_appDownloadUrl = string.Empty;
			this.m_appDownloadSize = 0u;
			this.m_appRecommendUpdateVersionMin = 0u;
			this.m_appRecommendUpdateVersionMax = 0u;
			this.m_appIsNoIFS = false;
			this.m_appSavePath = string.Empty;
			this.m_appIsDelayToInstall = false;
			this.m_appEnableYYB = 0;
			this.m_appYYBCheckVersionInfoStartTime = 0f;
			this.m_appYYBCheckVersionInfoCallBackHandled = false;
			this.CreateIIPSVersionMgr (this.GetCheckAppVersionJsonConfig (CVersionUpdateSystem.s_platform));
			if (this.m_IIPSVersionMgr == null || !this.m_IIPSVersionMgr.MgrCheckAppUpdate ()) {
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_AppUpdateFail"), 0, this.GetErrorResult (0u)), enUIEventID.VersionUpdate_RetryCheckAppVersion, false);

				// bsh: goto state StartCheckFirstExtractResource
				this.m_versionUpdateState = enVersionUpdateState.StartCheckFirstExtractResource;
				this.m_isError = false;
				return;

			}
			this.m_versionUpdateState = enVersionUpdateState.CheckAppVersion;
			this.m_isError = false;
		}

		private void CheckAppVersion ()
		{
			if (this.m_appYYBCheckVersionInfoStartTime > 0f && !this.m_appYYBCheckVersionInfoCallBackHandled && Time.realtimeSinceStartup - this.m_appYYBCheckVersionInfoStartTime >= 10f) {
				this.OpenAppUpdateConfirmPanel (true, false);
				this.m_appYYBCheckVersionInfoCallBackHandled = true;
			}
		}

		private void StartDownloadApp ()
		{
			this.CloseConfirmPanel ();
			if (CVersionUpdateSystem.s_platform == enPlatform.Android) {
				if (this.m_IIPSVersionMgr != null) {
					this.m_IIPSVersionMgr.MgrSetNextStage (true);
				}
				if ((!this.m_enablePreviousVersion || !this.s_isUpdateToPreviousVersion) && this.m_appIsNoIFS) {
					this.m_appIsDelayToInstall = true;
				}
				this.m_versionUpdateState = enVersionUpdateState.DownloadApp;
				this.m_reportAppDownloadStartTime = DateTime.Now;
				this.m_reportAppTotalDownloadSize = 0u;
				this.m_reportAppDownloadSize = 0u;
			} else {
				CUICommonSystem.OpenUrl (this.m_appDownloadUrl, false, 0);
				this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
			}
		}

		private void FinishUpdateApp ()
		{
			this.DisposeIIPSVersionMgr ();
			this.m_versionUpdateState = enVersionUpdateState.StartCheckFirstExtractResource;
		}

		private void StartCheckFirstExtractResource ()
		{
			this.m_versionUpdateState = enVersionUpdateState.CheckFirstExtractResource;
			this.m_firstExtractResourceVersion = string.Empty;
			this.m_firstExtractResourceBuildNumber = string.Empty;
			base.StartCoroutine (this.CheckFirstExtractResource ());
		}

		private IEnumerator CheckFirstExtractResource ()
		{
			UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_CheckFirstExtractResource"));
			if (IsFileExistInStreamingAssets (CVersionUpdateSystem.s_firstExtractIFSName) && 
				IsFileExistInStreamingAssets (CResourcePackerInfoSet.s_resourcePackerInfoSetFileName)) 
			{
				WWW www = new WWW (CFileManager.GetStreamingAssetsPathWithHeader (CResourcePackerInfoSet.s_resourcePackerInfoSetFileName));
				yield return www;
				int offset = 0;
				CResourcePackerInfoSet.ReadVersionAndBuildNumber (www.bytes, ref offset, ref m_firstExtractResourceVersion, ref m_firstExtractResourceBuildNumber);
				if ((m_cachedResourceType != (int)CVersionUpdateSystem.s_appType) || 
					(((CVersionUpdateSystem.s_IIPSServerType == enIIPSServerType.None) || 
				  		(CVersion.GetVersionNumber (m_cachedResourceVersion) <= CVersion.GetVersionNumber (m_firstExtractResourceVersion))) && 
					(!string.Equals (m_cachedResourceVersion, m_firstExtractResourceVersion) || 
				 		!string.Equals (m_cachedResourceBuildNumber, m_firstExtractResourceBuildNumber)))) 
				{
					ClearCachePath ();
					CreateIIPSVersionMgr (GetFirstExtractResourceJsonConfig (CVersionUpdateSystem.s_platform));
					if ((m_IIPSVersionMgr == null) || !m_IIPSVersionMgr.MgrCheckAppUpdate ()) 
					{
						Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_FirstExtractFail"), 0, GetErrorResult (0)), enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, false);
					}
					m_isError = false;
				}
			}

			// bsh: hack, forced to extract resource in apk
			m_versionUpdateState = enVersionUpdateState.FirstExtractResource; //enVersionUpdateState.FinishFirstExtractResouce;
		}

		private void StartFirstExtractResource ()
		{
			if (this.m_IIPSVersionMgr != null) {
				this.m_IIPSVersionMgr.MgrSetNextStage (true);
			}
			// bsh: don't update resource
			this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;//enVersionUpdateState.FirstExtractResource;
		}

		private void FinishFirstExtractResource ()
		{
			this.DisposeIIPSVersionMgr ();
			// bsh: hack, skip update resource
			m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
			/*
			if (CVersionUpdateSystem.s_IIPSServerType == enIIPSServerType.None) {
				this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
			} else {
				this.m_versionUpdateState = enVersionUpdateState.StartCheckResourceVersion;
			}
			*/
		}

		private void StartCheckResourceVersion ()
		{
			Singleton<CUIManager>.GetInstance ().CloseMessageBox ();
			this.CloseConfirmPanel ();
			this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_CheckResourceVersion"));
			this.m_downloadResourceVersion = string.Empty;
			this.m_resourceDownloadNeedConfirm = false;
			this.m_resourceCheckCompatibility = true;
			this.m_useCurseResourceDownloadSize = false;
			this.m_resourceDownloadSize = 0u;
			this.CreateIIPSVersionMgr (this.GetCheckResourceVersionJsonConfig (CVersionUpdateSystem.s_platform));
			if (this.m_IIPSVersionMgr == null || !this.m_IIPSVersionMgr.MgrCheckAppUpdate ()) {
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_ResourceUpdateFail"), 0, this.GetErrorResult (0u)), enUIEventID.VersionUpdate_RetryCheckResourceVersion, false);
			}
			this.m_versionUpdateState = enVersionUpdateState.CheckResourceVersion;
			this.m_isError = false;
		}

		private void StartDownloadResource ()
		{
			this.CloseConfirmPanel ();
			this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_PrepareDownloadResource"));
			this.UpdateUIDownloadProgressTextContent (string.Empty);
			if (this.m_IIPSVersionMgr != null) {
				this.m_IIPSVersionMgr.MgrSetNextStage (true);
			}
			this.m_apolloUpdateSpeedCounter.StopSpeedCounter ();
			this.m_downloadCounter = 0u;
			this.m_downloadSpeed = 0u;
			this.m_reportResourceDownloadStartTime = DateTime.Now;
			this.m_reportResourceTotalDownloadSize = 0u;
			this.m_reportResourceDownloadSize = 0u;
			this.m_versionUpdateState = enVersionUpdateState.DownloadResource;
		}

		private void FinishUpdateResource ()
		{
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
			if (CVersionUpdateSystem.s_platform == enPlatform.Android && this.m_appIsDelayToInstall) {
				if (!string.IsNullOrEmpty (this.m_appSavePath)) {
					this.Android_InstallAPK (this.m_appSavePath);
				}
				this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
			} else {
				this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_Complete"));
				this.m_versionUpdateState = enVersionUpdateState.Complete;
			}
			this.DisposeIIPSVersionMgr ();
		}

		public static void QuitApp ()
		{
			SGameApplication.Quit ();
			CVersionUpdateSystem.Android_ExitApp ();
		}

		private void Complete ()
		{
			this.m_versionUpdateState = enVersionUpdateState.End;
			base.StartCoroutine (this.VersionUpdateComplete ());
			Singleton<BeaconHelper>.GetInstance ().Event_CommonReport ("Event_VerUpdateFinish");
		}

		public void StartYYBCheckVersionInfo ()
		{
			if (this.m_appYYBUpdateManager != null) {
				this.m_appYYBUpdateManager.StartYYBCheckVersionInfo ();
			}
			this.m_appYYBCheckVersionInfoStartTime = Time.realtimeSinceStartup;
			this.m_appYYBCheckVersionInfoCallBackHandled = false;
		}

		public void StartYYBSaveUpdate ()
		{
			this.CloseConfirmPanel ();
			if (this.m_appYYBUpdateManager != null) {
				int num = this.m_appYYBUpdateManager.CheckYYBInstalled ();
				this.m_appYYBUpdateManager.StartYYBSaveUpdate ();
				if (num == 0) {
					this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
				} else {
					this.m_versionUpdateState = enVersionUpdateState.DownloadYYB;
					this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadYYB"));
				}
			}
		}

		private IEnumerator VersionUpdateComplete ()
		{
			OpenWaitingForm ();
			yield return null;

			Singleton<CResourceManager>.GetInstance ().LoadResourcePackerInfoSet ();
			PlayerPrefs.SetInt ("SplashHack", 1);
			var _textResource___0 = Singleton<CResourceManager>.GetInstance ().GetResource ("Config/Splash.txt", typeof(TextAsset), enResourceType.Numeric, false, true);
			if (_textResource___0 != null) {
				var _tAsstet___1 = _textResource___0.m_content as CBinaryObject;
				if ((null != _tAsstet___1) && (_tAsstet___1.m_data != null)) {
					var _content___2 = StringHelper.ASCIIBytesToString (_tAsstet___1.m_data);
					if ((_content___2 != null) && (_content___2.Trim () == "0")) {
						PlayerPrefs.SetInt ("SplashHack", 0);
					}
				}
			}
			PlayerPrefs.Save ();
			yield return StartCoroutine (Singleton<CResourceManager>.GetInstance ().LoadResidentAssetBundles ());

			yield return StartCoroutine (MonoSingleton<GameFramework>.GetInstance ().PrepareGameSystem ());

			CloseWaitingForm ();
			Singleton<CUIManager>.GetInstance ().CloseForm (CVersionUpdateSystem.s_versionUpdateFormPath);
			m_versionUpdateFormScript = null;
			if (m_onVersionUpdateComplete != null) {
				m_onVersionUpdateComplete ();
			}
		}

		private void ReadCachedResourceInfo ()
		{
			this.m_cachedResourceVersion = CVersion.s_emptyResourceVersion;
			this.m_cachedResourceBuildNumber = CVersion.s_emptyBuildNumber;
			this.m_cachedResourceType = 0;
			if (CFileManager.IsFileExist (CVersionUpdateSystem.s_cachedResourceVersionFilePath)) {
				byte[] array = CFileManager.ReadFile (CVersionUpdateSystem.s_cachedResourceVersionFilePath);
				int num = 0;
				if (array != null && array.Length > 4 && CMemoryManager.ReadInt (array, ref num) == array.Length) {
					this.m_cachedResourceVersion = CMemoryManager.ReadString (array, ref num);
					this.m_cachedResourceBuildNumber = CMemoryManager.ReadString (array, ref num);
					this.m_cachedResourceType = CMemoryManager.ReadByte (array, ref num);
				}
			}
		}

		private void WriteCachedResourceInfo ()
		{
			if (CFileManager.IsFileExist (CVersionUpdateSystem.s_cachedResourceVersionFilePath)) {
				CFileManager.DeleteFile (CVersionUpdateSystem.s_cachedResourceVersionFilePath);
			}
			if (CFileManager.IsFileExist (CVersionUpdateSystem.s_resourcePackerInfoSetFullPath)) {
				byte[] data = CFileManager.ReadFile (CVersionUpdateSystem.s_resourcePackerInfoSetFullPath);
				int num = 0;
				CResourcePackerInfoSet.ReadVersionAndBuildNumber (data, ref num, ref this.m_cachedResourceVersion, ref this.m_cachedResourceBuildNumber);
				this.m_cachedResourceType = (int)CVersionUpdateSystem.s_appType;
				data = new byte[1024];
				num = 0;
				int num2 = num;
				num += 4;
				CMemoryManager.WriteString (this.m_cachedResourceVersion, data, ref num);
				CMemoryManager.WriteString (this.m_cachedResourceBuildNumber, data, ref num);
				CMemoryManager.WriteByte ((byte)this.m_cachedResourceType, data, ref num);
				CMemoryManager.WriteInt (num, data, ref num2);
				CFileManager.WriteFile (CVersionUpdateSystem.s_cachedResourceVersionFilePath, data, 0, num);
				this.UpdateUIVersionTextContent (CVersionUpdateSystem.s_appVersion, this.m_cachedResourceVersion);
			}
		}

		private bool IsFileExistInStreamingAssets (string fileName)
		{
			return CVersionUpdateSystem.Android_IsFileExistInStreamingAssets (fileName);
		}

		private string GetCheckAppVersionJsonConfig (enPlatform platform)
		{
			string result = string.Empty;
			if (platform == enPlatform.IOS) {
				result = string.Format ("{{\r\n                            \"m_update_type\" : 4,\r\n                            \"log_debug\" : {5},\r\n\r\n                            \"basic_version\":\r\n                            {{\r\n                                \"m_server_url_list\" : [{0}],\r\n                                \"m_app_id\" : {1},\r\n                                \"m_service_id\" : {2},\r\n                                \"m_current_version_str\" : \"{3}\",\r\n                                \"m_retry_count\" : {4},\r\n                                \"m_retry_interval_ms\" : 1000,\r\n                                \"m_connect_timeout_ms\" : 1000,\r\n                                \"m_send_timeout_ms\" : 2000,\r\n                                \"m_recv_timeout_ms\" : 3000                                \r\n                            }}\r\n                    }}", new object[]
				{
					this.GetIIPSServerUrl (),
					CVersionUpdateSystem.s_appID,
					this.GetIIPSServiceIDForUpdateApp (CVersionUpdateSystem.s_IIPSServerType, platform),
					CVersionUpdateSystem.s_appVersion,
					this.GetIIPSServerAmount () + 2,
					(!CVersionUpdateSystem.s_logDebug) ? "false" : "true"
				});
			} else if (platform == enPlatform.Android) {
				result = string.Format ("{{\r\n                    \"basic_update\":\r\n                    {{\r\n                        \"m_ifs_save_path\" : \"{4}\",\r\n                        \"m_nextaction\" : \"basic_diffupdata\"\r\n                    }},\r\n\r\n                    \"basic_diffupdata\":             \r\n                    {{\r\n                        \"m_diff_config_save_path\" : \"{4}\",\r\n                        \"m_diff_temp_path\" : \"{4}\",\r\n                        \"m_nMaxDownloadSpeed\" : 102400000,\r\n                        \"m_apk_abspath\" : \"{5}\"\r\n                    }},\r\n\r\n                    \"m_update_type\" : 4,\r\n                    \"log_debug\" : {7},\r\n\r\n                    \"basic_version\":\r\n                    {{\r\n                        \"m_server_url_list\" : [{0}],\r\n                        \"m_app_id\" : {1},\r\n                        \"m_service_id\" : {2},\r\n                        \"m_current_version_str\" : \"{3}\",\r\n                        \"m_retry_count\" : {6},\r\n                        \"m_retry_interval_ms\" : 1000,\r\n                        \"m_connect_timeout_ms\" : 1000,\r\n                        \"m_send_timeout_ms\" : 2000,\r\n                        \"m_recv_timeout_ms\" : 3000,\r\n                        \"m_bGetConfigFromServer\" : true\r\n                    }}\r\n                }}", new object[]
				{
					this.GetIIPSServerUrl (),
					CVersionUpdateSystem.s_appID,
					this.GetIIPSServiceIDForUpdateApp (CVersionUpdateSystem.s_IIPSServerType, platform),
					"1.20.1.17", // bsh: this.GetUploadedAppVersion (),
					CVersionUpdateSystem.s_downloadedIFSSavePath,
					this.GetAndroidApkAbsPath (),
					this.GetIIPSServerAmount () + 2,
					(!CVersionUpdateSystem.s_logDebug) ? "false" : "true"
				});
			}
			return result;
		}

		private string GetFirstExtractResourceJsonConfig (enPlatform platform)
		{
			string empty = string.Empty;
			return string.Format ("{{\r\n                        \"basic_update\": \r\n                        {{ \r\n                            \"m_ifs_save_path\" : \"{4}\", \r\n                            \"m_nextaction\" : \"basic_diffupdata\" \r\n                        }}, \r\n                \r\n                        \"full_diff\":          \r\n\t\t\t\t        {{ \r\n\t\t\t\t\t        \"m_ifs_save_path\":\"{4}\", \r\n\t\t\t\t\t        \"m_file_extract_path\":\"{5}\" \r\n\t\t\t\t        }}, \r\n                \r\n                        \"m_update_type\" : 5,\r\n                        \"log_debug\" : {9},\r\n\r\n                        \"basic_version\":\r\n                        {{\r\n                            \"m_server_url_list\" : [{0}],\r\n                            \"m_app_id\" : {1},\r\n                            \"m_service_id\" : {2},\r\n                            \"m_current_version_str\" : \"{3}\",\r\n                            \"m_retry_count\" : {8},\r\n\t\t                    \"m_retry_interval_ms\" : 1000,\r\n\t\t                    \"m_connect_timeout_ms\" : 1000,\r\n\t\t                    \"m_send_timeout_ms\" : 2000,\r\n\t\t                    \"m_recv_timeout_ms\" : 3000\r\n                        }},\r\n\r\n                        \"first_extract\":\r\n\t\t\t\t        {{\r\n\t\t\t\t\t        \"m_ifs_extract_path\":\"{5}\",\r\n\t\t\t\t\t        \"m_ifs_res_save_path\":\"{4}\",\r\n\t\t\t\t\t        \"filelist\":[\r\n\t\t\t\t\t\t        {{\r\n\t\t\t\t\t\t\t        \"filename\":\"{6}\",\r\n\t\t\t\t\t\t\t        \"filepath\":\"{7}\"\r\n\t\t\t\t\t\t        }}\r\n\t\t\t\t\t        ]\r\n\t\t\t\t        }}\r\n                    }}\r\n                ", new object[]
			{
				this.GetIIPSServerUrl (),
				CVersionUpdateSystem.s_appID,
				this.GetIIPSServiceIDForUpdateResource (CVersionUpdateSystem.s_IIPSServerType, platform),
				this.m_firstExtractResourceVersion,
				CVersionUpdateSystem.s_downloadedIFSSavePath,
				CVersionUpdateSystem.s_ifsExtractPath,
				CVersionUpdateSystem.s_firstExtractIFSName,
				CVersionUpdateSystem.s_firstExtractIFSPath,
				this.GetIIPSServerAmount () + 2,
				(!CVersionUpdateSystem.s_logDebug) ? "false" : "true"
			});
		}

		private string GetCheckResourceVersionJsonConfig (enPlatform platform)
		{
			string empty = string.Empty;
			return string.Format ("{{\r\n                        \"basic_update\":\r\n                        {{\r\n                            \"m_ifs_save_path\" : \"{4}\",\r\n                            \"m_nextaction\" : \"basic_diffupdata\"\r\n                        }},\r\n                \r\n                        \"full_diff\":\r\n\t\t\t\t        {{ \r\n\t\t\t\t\t        \"m_ifs_save_path\":\"{4}\",\r\n\t\t\t\t\t        \"m_file_extract_path\":\"{5}\"\r\n\t\t\t\t        }},\r\n                \r\n                        \"m_update_type\" : 5,\r\n                        \"log_debug\" : {8},\r\n                        {7}\r\n\r\n                        \"basic_version\":\r\n                        {{\r\n                            \"m_server_url_list\" : [{0}],\r\n                            \"m_app_id\" : {1},\r\n                            \"m_service_id\" : {2},\r\n                            \"m_current_version_str\" : \"{3}\",\r\n                            \"m_retry_count\" : {6},\r\n\t\t                    \"m_retry_interval_ms\" : 1000,\r\n\t\t                    \"m_connect_timeout_ms\" : 1000,\r\n\t\t                    \"m_send_timeout_ms\" : 2000,\r\n\t\t                    \"m_recv_timeout_ms\" : 3000\r\n                        }}\r\n                    }}\r\n                ", new object[]
			{
				this.GetIIPSServerUrl (),
				CVersionUpdateSystem.s_appID,
				this.GetIIPSServiceIDForUpdateResource (CVersionUpdateSystem.s_IIPSServerType, platform),
				this.GetUploadedCachedResourceVersion (),
				CVersionUpdateSystem.s_downloadedIFSSavePath,
				CVersionUpdateSystem.s_ifsExtractPath,
				this.GetIIPSServerAmount () + 2,
				(!this.m_useCurseResourceDownloadSize) ? string.Empty : "\"need_down_size\" : true,",
				(!CVersionUpdateSystem.s_logDebug) ? "false" : "true"
			});
		}

		private string GetIIPSServerUrl ()
		{
			string[] array = CVersionUpdateSystem.s_IIPSServerUrls [(int)CVersionUpdateSystem.s_IIPSServerType];
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++) {
				if (i != array.Length - 1) {
					text += string.Format ("\"{0}\",", array [i]);
				} else {
					text += string.Format ("\"{0}\"", array [i]);
				}
			}
			return text;
		}

		private int GetIIPSServerAmount ()
		{
			string[] array = CVersionUpdateSystem.s_IIPSServerUrls [(int)CVersionUpdateSystem.s_IIPSServerType];
			return array.Length;
		}

		private uint GetIIPSServiceIDForUpdateApp (enIIPSServerType serverType, enPlatform platForm)
		{
			switch (serverType) {
			case enIIPSServerType.Official:
			case enIIPSServerType.Middle:
			case enIIPSServerType.Test:
			case enIIPSServerType.ExpOfficial:
			case enIIPSServerType.ExpTest:
			case enIIPSServerType.TestForTester:
				return CVersionUpdateSystem.s_serviceIDsForUpdataApp [(int)platForm];
			case enIIPSServerType.CompetitionOfficial:
			case enIIPSServerType.CompetitionTest:
				return CVersionUpdateSystem.s_serviceIDsForUpdateCompetitionApp [(int)platForm];
			default:
				return 0u;
			}
		}

		private uint GetIIPSServiceIDForUpdateResource (enIIPSServerType serverType, enPlatform platForm)
		{
			switch (serverType) {
			case enIIPSServerType.Official:
			case enIIPSServerType.Middle:
			case enIIPSServerType.Test:
			case enIIPSServerType.ExpOfficial:
			case enIIPSServerType.ExpTest:
			case enIIPSServerType.TestForTester:
				return CVersionUpdateSystem.s_serviceIDsForUpdateResource [(int)platForm];
			case enIIPSServerType.CompetitionOfficial:
			case enIIPSServerType.CompetitionTest:
				return CVersionUpdateSystem.s_serviceIDsForUpdateCompetitionResource [(int)platForm];
			default:
				return 0u;
			}
		}

		private string GetAndroidApkAbsPath ()
		{
			return CVersionUpdateSystem.Android_GetApkAbsPath ();
		}

		private void CreateIIPSVersionMgr (string config)
		{
			if (this.m_IIPSVersionMgr != null || this.m_IIPSVersionMgrFactory != null) {
				this.DisposeIIPSVersionMgr ();
			}
			CVersionUpdateCallback cVersionUpdateCallback = new CVersionUpdateCallback ();
			// bsh: use operator '+=" instead of Delegate.Combine() 
			/*
			CVersionUpdateCallback expr_23 = cVersionUpdateCallback;
			expr_23.m_onGetNewVersionInfoDelegate = (CVersionUpdateCallback.OnGetNewVersionInfoDelegate)Delegate.Combine (expr_23.m_onGetNewVersionInfoDelegate, new CVersionUpdateCallback.OnGetNewVersionInfoDelegate (this.OnGetNewVersionInfo));
			CVersionUpdateCallback expr_45 = cVersionUpdateCallback;
			expr_45.m_onProgressDelegate = (CVersionUpdateCallback.OnProgressDelegate)Delegate.Combine (expr_45.m_onProgressDelegate, new CVersionUpdateCallback.OnProgressDelegate (this.OnProgress));
			CVersionUpdateCallback expr_67 = cVersionUpdateCallback;
			expr_67.m_onErrorDelegate = (CVersionUpdateCallback.OnErrorDelegate)Delegate.Combine (expr_67.m_onErrorDelegate, new CVersionUpdateCallback.OnErrorDelegate (this.OnError));
			CVersionUpdateCallback expr_89 = cVersionUpdateCallback;
			expr_89.m_onSuccessDelegate = (CVersionUpdateCallback.OnSuccessDelegate)Delegate.Combine (expr_89.m_onSuccessDelegate, new CVersionUpdateCallback.OnSuccessDelegate (this.OnSuccess));
			CVersionUpdateCallback expr_AB = cVersionUpdateCallback;
			expr_AB.m_onNoticeInstallApkDelegate = (CVersionUpdateCallback.OnNoticeInstallApkDelegate)Delegate.Combine (expr_AB.m_onNoticeInstallApkDelegate, new CVersionUpdateCallback.OnNoticeInstallApkDelegate (this.OnNoticeInstallApk));
			CVersionUpdateCallback expr_CD = cVersionUpdateCallback;
			expr_CD.m_onActionMsgDelegate = (CVersionUpdateCallback.OnActionMsgDelegate)Delegate.Combine (expr_CD.m_onActionMsgDelegate, new CVersionUpdateCallback.OnActionMsgDelegate (this.OnActionMsg));
			*/
			cVersionUpdateCallback.m_onGetNewVersionInfoDelegate += new CVersionUpdateCallback.OnGetNewVersionInfoDelegate (OnGetNewVersionInfo);
			cVersionUpdateCallback.m_onProgressDelegate += new CVersionUpdateCallback.OnProgressDelegate (OnProgress);
			cVersionUpdateCallback.m_onErrorDelegate += new CVersionUpdateCallback.OnErrorDelegate (OnError);
			cVersionUpdateCallback.m_onSuccessDelegate += new CVersionUpdateCallback.OnSuccessDelegate (OnSuccess);
			cVersionUpdateCallback.m_onNoticeInstallApkDelegate += new CVersionUpdateCallback.OnNoticeInstallApkDelegate (OnNoticeInstallApk);
			cVersionUpdateCallback.m_onActionMsgDelegate += new CVersionUpdateCallback.OnActionMsgDelegate (OnActionMsg);

			this.m_IIPSVersionMgrFactory = new IIPSMobileVersion ();
			this.m_IIPSVersionMgr = this.m_IIPSVersionMgrFactory.CreateVersionMgr (cVersionUpdateCallback, config);
		}

		private void DisposeIIPSVersionMgr ()
		{
			if (this.m_IIPSVersionMgr != null) {
				this.m_IIPSVersionMgr.MgrUnitVersionManager ();
				this.m_IIPSVersionMgr = null;
			}
			if (this.m_IIPSVersionMgrFactory != null) {
				this.m_IIPSVersionMgrFactory.DeleteVersionMgr ();
				this.m_IIPSVersionMgrFactory = null;
			}
		}

		private void UpdateIIPSVersionMgr ()
		{
			if (this.m_IIPSVersionMgr != null) {
				this.m_IIPSVersionMgr.MgrPoll ();
			}
		}

		private bool IsInUpdateAppStage ()
		{
			return this.m_versionUpdateState >= enVersionUpdateState.StartCheckAppVersion && this.m_versionUpdateState <= enVersionUpdateState.FinishUpdateApp;
		}

		private bool IsInFirstExtractResourceStage ()
		{
			return this.m_versionUpdateState >= enVersionUpdateState.StartCheckFirstExtractResource && this.m_versionUpdateState <= enVersionUpdateState.FinishFirstExtractResouce;
		}

		private bool IsInUpdateResourceStage ()
		{
			return this.m_versionUpdateState >= enVersionUpdateState.StartCheckResourceVersion && this.m_versionUpdateState <= enVersionUpdateState.FinishUpdateResource;
		}

		public byte OnGetNewVersionInfo (IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo)
		{
			if (this.IsInUpdateAppStage ()) {
				if (CVersionUpdateSystem.I2B ((int)newVersionInfo.isAppUpdating)) {
					this.m_downloadAppVersion = string.Format ("{0}.{1}.{2}.{3}", new object[]
					{
						newVersionInfo.newAppVersion.programmeVersion.MajorVersion_Number,
						newVersionInfo.newAppVersion.programmeVersion.MinorVersion_Number,
						newVersionInfo.newAppVersion.programmeVersion.Revision_Number,
						newVersionInfo.newAppVersion.dataVersion.DataVersion
					});
					if (CVersionUpdateSystem.I2B ((int)newVersionInfo.isForcedUpdating)) {
						if (this.EnableYYBSaveUpdate ()) {
							this.CreateYYBUpdateManager ();
							this.StartYYBCheckVersionInfo ();
						} else {
							this.OpenAppUpdateConfirmPanel (true, false);
						}
						this.OpenAnnouncementPanel ();
					} else if (this.m_appRecommendUpdateVersionMin > 0u && CVersion.GetVersionNumber (CVersionUpdateSystem.s_appVersion) < this.m_appRecommendUpdateVersionMin) {
						if (this.EnableYYBSaveUpdate ()) {
							this.CreateYYBUpdateManager ();
							this.StartYYBCheckVersionInfo ();
						} else {
							this.OpenAppUpdateConfirmPanel (true, false);
						}
						this.OpenAnnouncementPanel ();
					} else if (this.m_appRecommendUpdateVersionMax > 0u && CVersion.GetVersionNumber (CVersionUpdateSystem.s_appVersion) > this.m_appRecommendUpdateVersionMax) {
						if (CVersionUpdateSystem.s_platform == enPlatform.Android) {
							this.ClearDownloadedApk ();
						}
						this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
					} else {
						this.OpenAppUpdateConfirmPanel (false, false);
						this.OpenAnnouncementPanel ();
					}
				} else {
					if (CVersionUpdateSystem.s_platform == enPlatform.Android) {
						this.ClearDownloadedApk ();
					}
					this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
				}
			} else if (this.IsInFirstExtractResourceStage ()) {
				this.StartFirstExtractResource ();
			} else if (this.IsInUpdateResourceStage ()) {
				this.m_downloadResourceVersion = string.Format ("{0}.{1}.{2}.{3}", new object[]
				{
					newVersionInfo.newAppVersion.programmeVersion.MajorVersion_Number,
					newVersionInfo.newAppVersion.programmeVersion.MinorVersion_Number,
					newVersionInfo.newAppVersion.programmeVersion.Revision_Number,
					newVersionInfo.newAppVersion.dataVersion.DataVersion
				});
				if (this.m_useCurseResourceDownloadSize) {
					this.m_resourceDownloadSize = (uint)newVersionInfo.needDownloadSize;
				}
				if (CVersionUpdateSystem.I2B ((int)newVersionInfo.isAppUpdating)) {
					if (this.m_resourceCheckCompatibility && !this.CheckResourceCompatibility ((!this.m_appIsDelayToInstall) ? CVersion.GetAppVersion () : this.m_downloadAppVersion, this.m_downloadResourceVersion)) {
						Singleton<CUIManager>.GetInstance ().OpenMessageBox (Singleton<CTextManager>.GetInstance ().GetText ("VersionIsLow"), enUIEventID.VersionUpdate_QuitApp, false);
						return 1;
					}
					if (this.m_resourceDownloadNeedConfirm && !this.m_appIsDelayToInstall) {
						this.OpenResourceUpdateConfirmPanel ();
						this.OpenAnnouncementPanel ();
					} else {
						this.StartDownloadResource ();
					}
				} else {
					if (this.m_resourceCheckCompatibility && !this.m_appIsDelayToInstall && !this.CheckResourceCompatibility (CVersion.GetAppVersion (), this.m_cachedResourceVersion)) {
						Singleton<CUIManager>.GetInstance ().OpenMessageBox (Singleton<CTextManager>.GetInstance ().GetText ("VersionIsLow"), enUIEventID.VersionUpdate_QuitApp, false);
						return 1;
					}
					this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
				}
			}
			return 1;
		}

		private bool EnableYYBSaveUpdate ()
		{
			return (!this.m_enablePreviousVersion || !this.s_isUpdateToPreviousVersion) && CVersionUpdateSystem.s_platform == enPlatform.Android && ((this.m_appEnableYYB == 1 && Singleton<ApolloHelper>.GetInstance ().GetChannelID () == 2002) || this.m_appEnableYYB == 2);
		}

		public void OnProgress (IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
		{
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, (totalSize != 0) ? (((float)nowSize) / ((float)totalSize)) : 0f);
			switch (curVersionStage) {
			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_ExtractData:
			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FirstExtract:
			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FullUpdate_Extract:
			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceExtract:
				this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_ExtractResource"));
				return;

			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FullUpdate:
			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceDownload:
				this.m_reportResourceTotalDownloadSize = (uint)totalSize;
				this.m_reportResourceDownloadSize = (uint)nowSize;
				this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadResource"));
				if (curVersionStage == IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceDownload) {
					if (this.m_downloadCounter == 0) {
						this.m_apolloUpdateSpeedCounter.StartSpeedCounter ();
					}
					this.m_apolloUpdateSpeedCounter.SetSize ((uint)nowSize);
					this.m_apolloUpdateSpeedCounter.SpeedCounter ();
					this.m_downloadSpeed = this.m_apolloUpdateSpeedCounter.GetSpeed ();
					this.m_downloadCounter++;
				} else {
					this.m_downloadSpeed = this.m_IIPSVersionMgr.MgrGetActionDownloadSpeed ();
				}
				this.UpdateUIDownloadProgressTextContent (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadResourceProgress"), this.GetDownloadTotalSize ((int)totalSize), this.GetDownloadSpeed ((int)this.m_downloadSpeed), !string.IsNullOrEmpty (this.m_downloadResourceVersion) ? string.Format ("(v{0})", this.m_downloadResourceVersion) : string.Empty));
				return;

			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_CreateApk:
				this.m_reportAppTotalDownloadSize = (uint)totalSize;
				this.m_reportAppDownloadSize = (uint)nowSize;
				this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadApp"));
				this.UpdateUIDownloadProgressTextContent (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadResourceProgress"), this.GetDownloadTotalSize ((int)totalSize), this.GetDownloadSpeed ((int)this.m_IIPSVersionMgr.MgrGetActionDownloadSpeed ()), string.Format ("(v{0})", this.m_downloadAppVersion)));
				return;

			case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_CheckApkMd5:
				this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_PrepareInstall"));
				this.UpdateUIDownloadProgressTextContent (string.Empty);
				return;
			}
		}

		public void OnError (IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode)
		{
			this.m_isError = true;
			Singleton<BeaconHelper>.GetInstance ().Event_CommonReport ("Event_VerUpdateFail");
			if (this.IsInUpdateAppStage ()) {
				enUIEventID confirmID = enUIEventID.VersionUpdate_RetryCheckAppVersion;
				IIPSMobileErrorCodeCheck.ErrorCodeInfo errorCodeInfo = this.m_iipsErrorCodeChecker.CheckIIPSErrorCode ((int)errorCode);
				if (errorCode == 6u || errorCode == 8u) {
					confirmID = enUIEventID.VersionUpdate_JumpToHomePage;
				}
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_AppUpdateFail"), errorCode.ToString (), this.GetErrorResult (errorCode)), confirmID, false);
				if (CVersionUpdateSystem.s_platform == enPlatform.Android && this.m_versionUpdateState == enVersionUpdateState.DownloadApp) {
					this.SendVersionUpdateReportEvent (this.m_reportAppDownloadStartTime, 0, CVersion.GetAppVersion (), this.m_downloadAppVersion, false, errorCode, this.m_reportAppTotalDownloadSize, this.m_reportAppDownloadSize, this.GetApkDownloadUrl ());
				}
			} else if (this.IsInFirstExtractResourceStage ()) {
				if (CVersionUpdateSystem.s_IIPSServerType == enIIPSServerType.None && errorCode == 154140677u) {
					this.m_isError = false;
				} else {
					Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_FirstExtractFail"), errorCode.ToString (), this.GetErrorResult (errorCode)), enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, false);
				}
			} else if (this.IsInUpdateResourceStage ()) {
				this.m_apolloUpdateSpeedCounter.StopSpeedCounter ();
				this.m_downloadCounter = 0u;
				this.m_downloadSpeed = 0u;
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_ResourceUpdateFail"), errorCode.ToString (), this.GetErrorResult (errorCode)), enUIEventID.VersionUpdate_RetryCheckResourceVersion, false);
				if (this.m_versionUpdateState == enVersionUpdateState.DownloadResource) {
					this.SendVersionUpdateReportEvent (this.m_reportResourceDownloadStartTime, 1, this.m_cachedResourceVersion, this.m_downloadResourceVersion, false, errorCode, this.m_reportResourceTotalDownloadSize, this.m_reportResourceDownloadSize, string.Empty);
				}
			}
		}

		public void OnSuccess ()
		{
			if (this.m_isError) {
				return;
			}
			if (this.IsInUpdateAppStage ()) {
				this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
			} else if (this.IsInFirstExtractResourceStage ()) {
				this.WriteCachedResourceInfo ();
				this.m_versionUpdateState = enVersionUpdateState.FinishFirstExtractResouce;
			} else if (this.IsInUpdateResourceStage ()) {
				this.m_apolloUpdateSpeedCounter.StopSpeedCounter ();
				this.m_downloadCounter = 0u;
				this.m_downloadSpeed = 0u;
				this.WriteCachedResourceInfo ();
				this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
				this.SendVersionUpdateReportEvent (this.m_reportResourceDownloadStartTime, 1, this.m_cachedResourceVersion, this.m_downloadResourceVersion, true, 0u, this.m_reportResourceTotalDownloadSize, this.m_reportResourceDownloadSize, string.Empty);
			}
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
		}

		public byte OnNoticeInstallApk (string path)
		{
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
			if (CVersionUpdateSystem.s_platform == enPlatform.Android) {
				this.SendVersionUpdateReportEvent (this.m_reportAppDownloadStartTime, 0, CVersion.GetAppVersion (), this.m_downloadAppVersion, true, 0u, this.m_reportAppTotalDownloadSize, this.m_reportAppDownloadSize, this.GetApkDownloadUrl ());
				if (this.m_appIsDelayToInstall) {
					this.m_appSavePath = path;
					this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
				} else {
					this.Android_InstallAPK (path);
					this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
				}
			}
			return 1;
		}

		public byte OnActionMsg (string msg)
		{
			if (CVersionUpdateSystem.s_platform == enPlatform.Android && 
			    !this.m_previousKeyVersionLoaded && 
			    this.m_versionUpdateState >= enVersionUpdateState.StartCheckAppVersion && 
			    this.m_versionUpdateState <= enVersionUpdateState.CheckAppVersion) 
			{
				string titleContentInMsg = this.GetTitleContentInMsg (msg, "Tversion_Config");
				if (!string.IsNullOrEmpty (titleContentInMsg)) {
					string paramValueInContent = this.GetParamValueInContent (titleContentInMsg, "szTversionSvrURL");
					if (!string.IsNullOrEmpty (paramValueInContent)) {
						try {
							this.m_previousKeyVersion = uint.Parse (paramValueInContent);
						} catch (Exception) {
							this.m_previousKeyVersion = 0u;
						}
					}
				}
				this.m_enablePreviousVersion = (this.m_previousKeyVersion > 0u);
				this.m_previousKeyVersionLoaded = true;
			}
			string titleContentInMsg2 = this.GetTitleContentInMsg (msg, "@App");
			if (!string.IsNullOrEmpty (titleContentInMsg2)) {
				this.m_appDownloadUrl = this.GetParamValueInContent (titleContentInMsg2, "url");
				string paramValueInContent2 = this.GetParamValueInContent (titleContentInMsg2, "size");
				string paramValueInContent3 = this.GetParamValueInContent (titleContentInMsg2, "minversion");
				string paramValueInContent4 = this.GetParamValueInContent (titleContentInMsg2, "maxversion");
				string paramValueInContent5 = this.GetParamValueInContent (titleContentInMsg2, "EnableYYB");
				string paramValueInContent6 = this.GetParamValueInContent (titleContentInMsg2, "AnnouncementUrl");
				string paramValueInContent7 = this.GetParamValueInContent (titleContentInMsg2, "AnnouncementExtension");
				string paramValueInContent8 = this.GetParamValueInContent (titleContentInMsg2, "NoIFS");
				if (!string.IsNullOrEmpty (paramValueInContent2)) {
					try {
						this.m_appDownloadSize = uint.Parse (paramValueInContent2);
					} catch (Exception) {
						this.m_appDownloadSize = 0u;
					}
				}
				if (!string.IsNullOrEmpty (paramValueInContent3)) {
					this.m_appRecommendUpdateVersionMin = CVersion.GetVersionNumber (paramValueInContent3);
				} else {
					this.m_appRecommendUpdateVersionMin = 0u;
				}
				if (!string.IsNullOrEmpty (paramValueInContent4)) {
					this.m_appRecommendUpdateVersionMax = CVersion.GetVersionNumber (paramValueInContent4);
				} else {
					this.m_appRecommendUpdateVersionMax = 0u;
				}
				if (!string.IsNullOrEmpty (paramValueInContent5)) {
					try {
						this.m_appEnableYYB = int.Parse (paramValueInContent5);
					} catch (Exception) {
						this.m_appEnableYYB = 0;
					}
				} else {
					this.m_appEnableYYB = 0;
				}
				this.InitializeAnnouncement (paramValueInContent6, paramValueInContent7);
				if (!string.IsNullOrEmpty (paramValueInContent8)) {
					try {
						this.m_appIsNoIFS = (int.Parse (paramValueInContent8) > 0);
					} catch (Exception) {
						this.m_appIsNoIFS = false;
					}
				} else {
					this.m_appIsNoIFS = false;
				}
			}
			string titleContentInMsg3 = this.GetTitleContentInMsg (msg, "@Resource");
			if (!string.IsNullOrEmpty (titleContentInMsg3)) {
				string paramValueInContent9 = this.GetParamValueInContent (titleContentInMsg3, "needconfirm");
				string paramValueInContent10 = this.GetParamValueInContent (titleContentInMsg3, "size");
				string paramValueInContent11 = this.GetParamValueInContent (titleContentInMsg3, "AnnouncementUrl");
				string paramValueInContent12 = this.GetParamValueInContent (titleContentInMsg3, "AnnouncementExtension");
				string paramValueInContent13 = this.GetParamValueInContent (titleContentInMsg3, "CheckCompatibility");
				this.m_resourceDownloadNeedConfirm = false;
				if (!string.IsNullOrEmpty (paramValueInContent9)) {
					this.m_resourceDownloadNeedConfirm = string.Equals (paramValueInContent9, "1");
				}
				this.m_resourceCheckCompatibility = true;
				if (!string.IsNullOrEmpty (paramValueInContent13)) {
					this.m_resourceCheckCompatibility = string.Equals (paramValueInContent13, "1");
				}
				if (!this.m_useCurseResourceDownloadSize && !string.IsNullOrEmpty (paramValueInContent10)) {
					try {
						this.m_resourceDownloadSize = uint.Parse (paramValueInContent10);
					} catch (Exception) {
						this.m_resourceDownloadSize = 0u;
					}
				}
				this.InitializeAnnouncement (paramValueInContent11, paramValueInContent12);
			}
			return 1;
		}

		public void OnYYBCheckNeedUpdateInfo (string msg)
		{
			if (this.m_appYYBCheckVersionInfoCallBackHandled) {
				return;
			}
			this.OpenAppUpdateConfirmPanel (true, string.Equals (msg, "1"));
			this.m_appYYBCheckVersionInfoCallBackHandled = true;
		}

		public void OnDownloadYYBProgressChanged (string msg)
		{
			int num = 1;
			int num2 = 1;
			string[] array = msg.Split (new char[]
			{
				','
			});
			if (array.Length >= 2) {
				try {
					num = int.Parse (array [0].Trim ());
					num2 = int.Parse (array [1].Trim ());
				} catch (Exception) {
					num = 1;
					num2 = 1;
				}
			}
			if (num2 < 0 && num > 0) {
				num2 = num;
			}
			this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, (float)num / (float)((num2 != 0) ? num2 : num));
			this.UpdateUIDownloadProgressTextContent (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadYYBProgress"), this.GetDownloadTotalSize (num2)));
		}

		public void OnDownloadYYBStateChanged (string msg)
		{
			int num = 0;
			int num2 = 0;
			string text = string.Empty;
			string[] array = msg.Split (new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++) {
				try {
					if (i == 0) {
						num = int.Parse (array [i].Trim ());
					} else if (i == 1) {
						num2 = int.Parse (array [i].Trim ());
					} else if (i == 2) {
						text = array [i];
					}
				} catch (Exception) {
				}
			}
			if (num == 5) {
				this.m_isError = true;
				Singleton<BeaconHelper>.GetInstance ().Event_CommonReport ("Event_VerUpdateFail");
				Singleton<CUIManager>.GetInstance ().OpenMessageBox (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadYYBFail"), num2.ToString ()), enUIEventID.VersionUpdate_RetryCheckAppVersion, false);
			} else if (num == 4) {
				this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
				this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
			} else if (num == 1) {
				this.UpdateUIProgress (enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 0f);
			}
			this.UpdateUIStateInfoTextContent (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_DownloadYYB"));
		}

		private void CreateYYBUpdateManager ()
		{
			if (this.m_appYYBUpdateManager == null) {
				this.m_appYYBUpdateManager = new CYYBUpdateManager (new CYYBUpdateManager.OnYYBCheckNeedUpdateInfo (this.OnYYBCheckNeedUpdateInfo), new CYYBUpdateManager.OnDownloadYYBProgressChanged (this.OnDownloadYYBProgressChanged), new CYYBUpdateManager.OnDownloadYYBStateChanged (this.OnDownloadYYBStateChanged));
			}
		}

		private string GetTitleContentInMsg (string msg, string title)
		{
			int num = msg.IndexOf (title);
			if (num >= 0) {
				int num2 = msg.IndexOf ("{", num);
				int num3 = msg.IndexOf ("}", num);
				if (num2 > 0 && num3 > 0) {
					return msg.Substring (num2 + 1, num3 - num2 + 1 - 2);
				}
			}
			return string.Empty;
		}

		private string GetParamValueInContent (string titleContent, string param)
		{
			string[] array = titleContent.Split (new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++) {
				string b;
				string result;
				this.GetParamPair (array [i], out b, out result);
				if (string.Equals (param, b)) {
					return result;
				}
			}
			return string.Empty;
		}

		private void GetParamPair (string paramPairStr, out string param, out string value)
		{
			param = string.Empty;
			value = string.Empty;
			string[] array = paramPairStr.Split (new char[]
			{
				':'
			}, 2);
			if (array != null && array.Length == 2) {
				param = this.RemoveQuotationMark (array [0].Trim ());
				value = this.RemoveQuotationMark (array [1].Trim ());
			}
		}

		private string RemoveQuotationMark (string str)
		{
			int num = str.IndexOf ('"');
			int num2 = str.LastIndexOf ('"');
			if (num >= 0 && num2 >= 0 && num != num2) {
				return str.Substring (num + 1, num2 - num - 1).Trim (new char[]
				{
					'\\'
				}).Trim ();
			}
			return str.Trim ();
		}

		private void OnJumpToHomePage (CUIEvent uiEvent)
		{
			CUICommonSystem.OpenUrl ("http://pvp.qq.com", false, 0);
			this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
		}

		private void OnRetryCheckApp (CUIEvent uiEvent)
		{
			this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
		}

		private void OnConfirmUpdateApp (CUIEvent uiEvent)
		{
			if (this.IsUseWifi () || CVersionUpdateSystem.s_platform == enPlatform.IOS) {
				this.StartDownloadApp ();
			} else {
				Singleton<CUIManager>.GetInstance ().OpenMessageBoxWithCancel (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_NoWifiConfirm"), this.GetSizeString ((int)this.m_appDownloadSize)), enUIEventID.VersionUpdate_ConfirmUpdateAppNoWifi, enUIEventID.None, Singleton<CTextManager>.GetInstance ().GetText ("Common_Confirm"), Singleton<CTextManager>.GetInstance ().GetText ("Common_Cancel"), false);
			}
		}

		private void OnConfirmUpdateAppNoWifi (CUIEvent uiEvent)
		{
			this.StartDownloadApp ();
		}

		private void OnCancelUpdateApp (CUIEvent uiEvent)
		{
			this.CloseConfirmPanel ();
			this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
		}

		private void OnQuitApp (CUIEvent uiEvent)
		{
			CVersionUpdateSystem.QuitApp ();
		}

		private void OnRetryCheckFirstExtractResource (CUIEvent uiEvent)
		{
			this.m_versionUpdateState = enVersionUpdateState.StartCheckFirstExtractResource;
		}

		private void OnRetryCheckResourceVersion (CUIEvent uiEvent)
		{
			this.m_versionUpdateState = enVersionUpdateState.StartCheckResourceVersion;
		}

		private void OnConfirmUpdateResource (CUIEvent uiEvent)
		{
			if (this.IsUseWifi ()) {
				this.StartDownloadResource ();
			} else {
				Singleton<CUIManager>.GetInstance ().OpenMessageBoxWithCancel (string.Format (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_NoWifiConfirm"), this.GetSizeString ((int)this.m_resourceDownloadSize)), enUIEventID.VersionUpdate_ConfirmUpdateResourceNoWifi, enUIEventID.None, Singleton<CTextManager>.GetInstance ().GetText ("Common_Confirm"), Singleton<CTextManager>.GetInstance ().GetText ("Common_Cancel"), false);
			}
		}

		private void OnConfirmUpdateResourceNoWifi (CUIEvent uiEvent)
		{
			this.StartDownloadResource ();
		}

		private void OnConfirmYYBSaveUpdateApp (CUIEvent uiEvent)
		{
			this.StartYYBSaveUpdate ();
		}

		private void OnAnnouncementListElementEnable (CUIEvent uiEvent)
		{
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript != null && this.m_announcementInfos != null && this.m_announcementInfos.Count > uiEvent.m_srcWidgetIndexInBelongedList) {
				CAnnouncementInfo cAnnouncementInfo = this.m_announcementInfos [uiEvent.m_srcWidgetIndexInBelongedList];
				GameObject widget = cUIListElementScript.GetWidget (0);
				GameObject widget2 = cUIListElementScript.GetWidget (1);
				if (cAnnouncementInfo.m_type == enAnnouncementType.Text) {
					if (widget != null) {
						widget.CustomSetActive (false);
					}
					if (widget2 != null) {
						widget2.CustomSetActive (true);
						CUIHttpTextScript component = widget2.GetComponent<CUIHttpTextScript> ();
						if (component != null) {
							component.SetTextUrl (cAnnouncementInfo.m_url, false);
						}
					}
				} else if (cAnnouncementInfo.m_type == enAnnouncementType.Image) {
					if (widget != null) {
						widget.CustomSetActive (true);
						CUIHttpImageScript component2 = widget.GetComponent<CUIHttpImageScript> ();
						if (component2 != null) {
							component2.SetImageUrl (cAnnouncementInfo.m_url);
						}
					}
					if (widget2 != null) {
						widget2.CustomSetActive (false);
					}
				}
			}
		}

		private void OnSwitchAnnouncementListElementToFront (CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget (3);
			if (widget != null) {
				CUIListScript component = widget.GetComponent<CUIListScript> ();
				if (component != null) {
					int num = component.GetSelectedIndex () - 1;
					if (num >= 0 && num < this.m_announcementInfos.Count) {
						component.SelectElement (num, true);
					}
				}
			}
		}

		private void OnSwitchAnnouncementListElementToBehind (CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget (3);
			if (widget != null) {
				CUIListScript component = widget.GetComponent<CUIListScript> ();
				if (component != null) {
					int num = component.GetSelectedIndex () + 1;
					if (num >= 0 && num < this.m_announcementInfos.Count) {
						component.SelectElement (num, true);
					}
				}
			}
		}

		private void OnAnnouncementListSelectChanged (CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null) {
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex ();
			cUIListScript.MoveElementInScrollArea (selectedIndex, false);
			this.EnableAnnouncementElementPointer (cUIListScript.GetLastSelectedIndex (), false);
			this.EnableAnnouncementElementPointer (selectedIndex, true);
			GameObject widget = uiEvent.m_srcFormScript.GetWidget (11);
			GameObject widget2 = uiEvent.m_srcFormScript.GetWidget (12);
			if (selectedIndex == 0) {
				widget.CustomSetActive (false);
				widget2.CustomSetActive (true);
			} else if (selectedIndex == this.m_announcementInfos.Count - 1) {
				widget.CustomSetActive (true);
				widget2.CustomSetActive (false);
			} else {
				widget.CustomSetActive (true);
				widget2.CustomSetActive (true);
			}
		}

		private void OnUpdateToPreviousVersion (CUIEvent uiEvent)
		{
			this.s_isUpdateToPreviousVersion = true;
			Singleton<CUIManager>.GetInstance ().CloseAllForm (new string[]
			{
				CLoginSystem.s_splashFormPath
			}, true, true);
			Singleton<GameStateCtrl>.GetInstance ().GotoState ("VersionUpdateState");
		}

		private void InitializeAnnouncement (string announcementUrl, string announcementExtension)
		{
			if (this.m_announcementInfos.Count > 0) {
				return;
			}
			if (!string.IsNullOrEmpty (announcementUrl) && !string.IsNullOrEmpty (announcementExtension)) {
				string[] array = announcementExtension.Split (new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++) {
					array [i] = array [i].Trim ();
					CAnnouncementInfo cAnnouncementInfo = new CAnnouncementInfo ();
					try {
						cAnnouncementInfo.m_type = this.GetAnnouncementType (array [i]);
						cAnnouncementInfo.m_url = announcementUrl.Replace ("%ID%", (i + 1).ToString ()).Replace ("%Extension%", array [i]);
						cAnnouncementInfo.m_pointerSequence = -1;
					} catch (Exception) {
					}
					this.m_announcementInfos.Add (cAnnouncementInfo);
				}
			}
		}

		private enAnnouncementType GetAnnouncementType (string extension)
		{
			if (string.Equals (extension, "txt")) {
				return enAnnouncementType.Text;
			}
			return enAnnouncementType.Image;
		}

		private string GetErrorResult (uint errorCode)
		{
			string key = "IIPS_Error_Result_Unknown";
			switch (this.m_iipsErrorCodeChecker.CheckIIPSErrorCode ((int)errorCode).m_nErrorType) {
			case 1:
				key = ((!this.IsUseWifi ()) ? "IIPS_Error_Result_NetworkError_NoWifi" : "IIPS_Error_Result_NetworkError");
				break;
			case 2:
				key = "IIPS_Error_Result_NetworkTimeout";
				break;
			case 3:
				key = "IIPS_Error_Result_DiskFull";
				break;
			case 4:
				key = "IIPS_Error_Result_OtherSystemError";
				break;
			case 5:
				key = "IIPS_Error_Result_OtherError";
				break;
			case 6:
				key = ((!this.IsUseWifi ()) ? "IIPS_Error_Result_NoSupportUpdate_NoWifi" : "IIPS_Error_Result_NoSupportUpdate");
				break;
			case 7:
				key = "IIPS_Error_Result_NotSure";
				break;
			case 8:
				key = ((!this.IsUseWifi ()) ? "IIPS_Error_Result_ApkException_NoWifi" : "IIPS_Error_Result_ApkException");
				break;
			}
			return Singleton<CTextManager>.GetInstance ().GetText (key);
		}

		private string GetDownloadTotalSize (int size)
		{
			return this.GetSizeString (size);
		}

		private string GetDownloadSpeed (int speed)
		{
			return string.Format ("{0}/s", this.GetSizeString (speed));
		}

		private string GetSizeString (int size)
		{
			if (size >= 1048576) {
				float f = (float)size / 1048576f;
				return string.Format ("{0}MB", Mathf.RoundToInt (f));
			}
			float f2 = (float)size / 1024f;
			return string.Format ("{0}KB", Mathf.RoundToInt (f2));
		}

		private void UpdateUIProgress (enVersionUpdateFormWidget progressBarWidget, enVersionUpdateFormWidget progressPercentTextWidget, float progress)
		{
			if (progress > 1f) {
				progress = 1f;
			}
			Slider uIComponent = this.GetUIComponent<Slider> (this.m_versionUpdateFormScript, progressBarWidget);
			if (uIComponent != null) {
				uIComponent.value = progress;
			}
			Text uIComponent2 = this.GetUIComponent<Text> (this.m_versionUpdateFormScript, progressPercentTextWidget);
			if (uIComponent2 != null) {
				uIComponent2.text = string.Format ("{0}%", (int)(progress * 100f));
			}
		}

		private void UpdateUIStateInfoTextContent (string content)
		{
			Text uIComponent = this.GetUIComponent<Text> (this.m_versionUpdateFormScript, enVersionUpdateFormWidget.Text_CurrentState);
			if (uIComponent != null) {
				uIComponent.text = content;
			}
		}

		private void UpdateUIDownloadProgressTextContent (string content)
		{
			Text uIComponent = this.GetUIComponent<Text> (this.m_versionUpdateFormScript, enVersionUpdateFormWidget.Text_UpdateInfo);
			if (uIComponent != null) {
				uIComponent.text = content;
			}
		}

		private void UpdateUIVersionTextContent (string appVersion, string resourceVersion)
		{
			this.UpdateUITextContent (enVersionUpdateFormWidget.Text_Version, string.Format ("App v{0}   Res v{1}", appVersion, resourceVersion));
		}

		private void UpdateUITextContent (enVersionUpdateFormWidget textWidget, string content)
		{
			Text uIComponent = this.GetUIComponent<Text> (this.m_versionUpdateFormScript, textWidget);
			if (uIComponent != null) {
				uIComponent.text = content;
			}
		}

		private T GetUIComponent<T> (CUIFormScript formScript, enVersionUpdateFormWidget widget) where T : MonoBehaviour
		{
			if (formScript == null) {
				return (T)((object)null);
			}
			GameObject widget2 = this.m_versionUpdateFormScript.GetWidget ((int)widget);
			if (widget2 == null) {
				return (T)((object)null);
			}
			return widget2.GetComponent<T> ();
		}

		private void OpenAppUpdateConfirmPanel (bool isForcedUpdating, bool useYYBSaveUpdate)
		{
			if (this.m_versionUpdateFormScript == null) {
				return;
			}
			GameObject widget = this.m_versionUpdateFormScript.GetWidget (17);
			if (widget != null) {
				widget.CustomSetActive (false);
			}
			GameObject widget2 = this.m_versionUpdateFormScript.GetWidget (5);
			if (widget2 != null) {
				widget2.CustomSetActive (true);
			}
			if (isForcedUpdating) {
				this.SetUpdateNotice (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_ForceUpdateClient"));
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Cancel, true, "Quit", enUIEventID.VersionUpdate_QuitApp);
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateApp);
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, useYYBSaveUpdate, "VersionUpdate_YYBSaveUpdate", enUIEventID.VersionUpdate_ConfirmYYBSaveUpdateApp);
			} else {
				this.SetUpdateNotice (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_RecommendUpdateClient"));
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Cancel, true, "Common_Cancel", enUIEventID.VersionUpdate_CancelUpdateApp);
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateApp);
				this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, false, string.Empty, enUIEventID.None);
			}
		}

		private void OpenResourceUpdateConfirmPanel ()
		{
			if (this.m_versionUpdateFormScript == null) {
				return;
			}
			GameObject widget = this.m_versionUpdateFormScript.GetWidget (17);
			if (widget != null) {
				widget.CustomSetActive (false);
			}
			GameObject widget2 = this.m_versionUpdateFormScript.GetWidget (5);
			if (widget2 != null) {
				widget2.CustomSetActive (true);
			}
			this.SetUpdateNotice (Singleton<CTextManager>.GetInstance ().GetText ("VersionUpdate_ForceUpdateResource"));
			this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Cancel, true, "Quit", enUIEventID.VersionUpdate_QuitApp);
			this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateResource);
			this.SetConfirmPanelButton (enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, false, string.Empty, enUIEventID.None);
		}

		private void CloseConfirmPanel ()
		{
			GameObject widget = (m_versionUpdateFormScript != null) ? m_versionUpdateFormScript.GetWidget (5) : null;
			if (widget != null) {
				widget.CustomSetActive (false);
			}
			GameObject widget2 = (m_versionUpdateFormScript != null) ? m_versionUpdateFormScript.GetWidget (17) : null;
			if (widget2 != null) {
				widget2.CustomSetActive (true);
			}
		}

		private void OpenAnnouncementPanel ()
		{
			if (this.m_versionUpdateFormScript == null || this.m_announcementInfos == null || this.m_announcementInfos.Count <= 0 || this.m_isAnnouncementPanelOpened) {
				return;
			}
			GameObject widget = this.m_versionUpdateFormScript.GetWidget (7);
			if (widget != null) {
				widget.CustomSetActive (true);
			}
			GameObject widget2 = this.m_versionUpdateFormScript.GetWidget (4);
			if (widget2 != null) {
				CUIContainerScript component = widget2.GetComponent<CUIContainerScript> ();
				if (component != null) {
					component.RecycleAllElement ();
					for (int i = 0; i < this.m_announcementInfos.Count; i++) {
						this.m_announcementInfos [i].m_pointerSequence = component.GetElement ();
					}
				}
			}
			GameObject widget3 = this.m_versionUpdateFormScript.GetWidget (3);
			if (widget3 != null) {
				CUIListScript component2 = widget3.GetComponent<CUIListScript> ();
				if (component2 != null) {
					component2.SetElementAmount (this.m_announcementInfos.Count);
					component2.SelectElement (0, true);
					this.EnableAnnouncementElementPointer (0, true);
				}
			}
			this.m_isAnnouncementPanelOpened = true;
		}

		private void CloseAnnouncementPanel ()
		{
			GameObject widget = (m_versionUpdateFormScript != null) ? m_versionUpdateFormScript.GetWidget (7) : null;
			if (widget != null) {
				widget.CustomSetActive (false);
			}
			this.m_isAnnouncementPanelOpened = false;
		}

		private void EnableAnnouncementElementPointer (int index, bool enabled)
		{
			if (index < 0 || index >= this.m_announcementInfos.Count) {
				return;
			}
			GameObject widget = this.m_versionUpdateFormScript.GetWidget (4);
			if (widget == null) {
				return;
			}
			CUIContainerScript component = widget.GetComponent<CUIContainerScript> ();
			if (component == null) {
				return;
			}
			if (index >= 0) {
				GameObject element = component.GetElement (this.m_announcementInfos [index].m_pointerSequence);
				if (element != null) {
					Transform transform = element.transform.FindChild ("Image_Pointer");
					if (transform != null) {
						transform.gameObject.CustomSetActive (enabled);
					}
				}
			}
		}

		private void OpenWaitingForm ()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance ().OpenForm (CVersionUpdateSystem.s_waitingFormPath, false, false);
			if (cUIFormScript != null) {
				cUIFormScript.transform.Find ("Panel/Panel").gameObject.CustomSetActive (false);
				cUIFormScript.transform.Find ("Panel/Image").gameObject.CustomSetActive (true);
			}
		}

		private void CloseWaitingForm ()
		{
			Singleton<CUIManager>.GetInstance ().CloseForm (CVersionUpdateSystem.s_waitingFormPath);
		}

		private void SetUpdateNotice (string noticeContent)
		{
			GameObject widget = this.m_versionUpdateFormScript.GetWidget (13);
			if (widget != null) {
				Text component = widget.GetComponent<Text> ();
				if (component != null) {
					component.text = noticeContent;
				}
			}
		}

		private void SetConfirmPanelButton (enVersionUpdateFormWidget widgetIndex, bool active, string textKey, enUIEventID eventID)
		{
			if (this.m_versionUpdateFormScript == null) {
				return;
			}
			GameObject widget = this.m_versionUpdateFormScript.GetWidget ((int)widgetIndex);
			if (widget == null) {
				return;
			}
			widget.CustomSetActive (active);
			if (!string.IsNullOrEmpty (textKey)) {
				Transform transform = widget.transform.FindChild ("Text");
				if (transform != null) {
					Text component = transform.gameObject.GetComponent<Text> ();
					if (component != null) {
						component.text = Singleton<CTextManager>.GetInstance ().GetText (textKey);
					}
				}
			}
			CUIEventScript component2 = widget.GetComponent<CUIEventScript> ();
			if (component2 != null) {
				component2.SetUIEvent (enUIEventType.Click, eventID);
			}
		}

		private bool IsUseWifi ()
		{
			return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		}

		private bool ClearDownloadedApk ()
		{
			return CFileManager.ClearDirectory (CFileManager.GetCachePath (), new string[]
			{
				".apk"
			}, null);
		}

		private string GetApkDownloadUrl ()
		{
			string text = CFileManager.CombinePath (CFileManager.GetCachePath (), this.m_downloadAppVersion);
			if (CFileManager.IsFileExist (text)) {
				string msg = File.ReadAllText (text);
				string titleContentInMsg = this.GetTitleContentInMsg (msg, "\"full\"");
				return this.GetParamValueInContent (titleContentInMsg, "url");
			}
			return string.Empty;
		}

		private void SendVersionUpdateReportEvent (DateTime startTime, int versionType, string currentVersion, string updateVersion, bool isSuccessful, uint errorCode, uint totalDownloadSize, uint downloadSize, string downloadUrl)
		{
			Debug.Log (string.Format ("Send \"Service_DownloadEvent\", startTime = {0}, versionType = {1}, currentVersion = {2}, updateVersion = {3}, isSuccessful = {4}, errorCode = {5}, totalDownloadSize = {6}, downloadSize = {7}, downloadUrl = {8}", new object[]
			{
				startTime.ToString ("yyyy-mm-dd HH:mm:ss"),
				versionType.ToString (),
				currentVersion,
				updateVersion,
				isSuccessful.ToString (),
				errorCode.ToString (),
				totalDownloadSize.ToString (),
				downloadSize.ToString (),
				downloadUrl
			}));
			if (isSuccessful && totalDownloadSize != downloadSize) {
				downloadSize = totalDownloadSize;
			}
			try {
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>> ();
				list.Add (new KeyValuePair<string, string> ("openid", string.Empty));
				list.Add (new KeyValuePair<string, string> ("begintime", Utility.ToUtcSeconds (startTime).ToString ()));
				list.Add (new KeyValuePair<string, string> ("versionType", versionType.ToString ()));
				list.Add (new KeyValuePair<string, string> ("Version", updateVersion));
				list.Add (new KeyValuePair<string, string> ("oldversion", currentVersion));
				list.Add (new KeyValuePair<string, string> ("errorCode", ((!isSuccessful) ? 1 : 0).ToString ()));
				list.Add (new KeyValuePair<string, string> ("gameerrorcode", errorCode.ToString ()));
				list.Add (new KeyValuePair<string, string> ("errorinfo", string.Empty));
				list.Add (new KeyValuePair<string, string> ("totaltime", (DateTime.Now - startTime).TotalMilliseconds.ToString ()));
				list.Add (new KeyValuePair<string, string> ("totalfilesize", totalDownloadSize.ToString ()));
				list.Add (new KeyValuePair<string, string> ("filesize", downloadSize.ToString ()));
				list.Add (new KeyValuePair<string, string> ("url", downloadUrl));
				list.Add (new KeyValuePair<string, string> ("final_url", string.Empty));
				Singleton<ApolloHelper>.GetInstance ().ApolloRepoertEvent ("Service_DownloadEvent", list, true);
			} catch (Exception ex) {
				Debug.Log (ex.ToString ());
			}
		}

		private bool CheckResourceCompatibility (string appVersion, string resourceVersion)
		{
			if (string.IsNullOrEmpty (appVersion) || string.IsNullOrEmpty (resourceVersion)) {
				return false;
			}
			int num = appVersion.LastIndexOf (".");
			if (num >= 0) {
				appVersion = appVersion.Substring (0, num);
			}
			num = resourceVersion.LastIndexOf (".");
			if (num >= 0) {
				resourceVersion = resourceVersion.Substring (0, num);
			}
			return string.Equals (appVersion, resourceVersion);
		}

		private string GetUploadedAppVersion ()
		{
			if (this.m_enablePreviousVersion && this.s_isUpdateToPreviousVersion && CVersion.GetVersionNumber (CVersionUpdateSystem.s_appVersion) < CVersion.GetVersionNumber (this.GetPreviousVirualNodeVersion ())) {
				return this.GetPreviousVirualNodeVersion ();
			}
			return CVersionUpdateSystem.s_appVersion;
		}

		private string GetUploadedCachedResourceVersion ()
		{
			if (this.m_enablePreviousVersion && this.IsPreviousApp () && CVersion.GetVersionNumber (this.m_cachedResourceVersion) < CVersion.GetVersionNumber (this.GetPreviousVirualNodeVersion ())) {
				return this.GetPreviousVirualNodeVersion ();
			}
			return this.m_cachedResourceVersion;
		}

		public bool IsEnablePreviousVersion ()
		{
			return this.m_enablePreviousVersion;
		}

		public bool IsPreviousApp ()
		{
			string[] array = CVersionUpdateSystem.s_appVersion.Split (new char[]
			{
				'.'
			});
			return array.Length >= 2 && this.m_previousKeyVersion > 0u && string.Equals (array [1].Trim (), this.m_previousKeyVersion.ToString ());
		}

		private string GetPreviousGrayNodeVersion ()
		{
			return string.Format ("{0}.{1}.0.0", (CVersionUpdateSystem.s_appType != enAppType.Exp) ? "1" : "0", this.m_previousKeyVersion);
		}

		private string GetPreviousVirualNodeVersion ()
		{
			return string.Format ("{0}.{1}.1.0", (CVersionUpdateSystem.s_appType != enAppType.Exp) ? "1" : "0", this.m_previousKeyVersion);
		}

		private void Android_InstallAPK (string path)
		{
			if (this.m_IIPSVersionMgr != null) {
				this.m_IIPSVersionMgr.InstallApk (path);
			}
		}

		private static string Android_GetApkAbsPath ()
		{
			return CVersionUpdateSystem.s_androidUtilityJavaClass.CallStatic<string> ("GetApkAbsPath", new object[0]);
		}

		private static bool Android_IsFileExistInStreamingAssets (string fileName)
		{
			return CVersionUpdateSystem.s_androidUtilityJavaClass.CallStatic<bool> ("IsFileExistInStreamingAssets", new object[]
			{
				fileName
			});
		}

		public static int Android_GetNetworkType ()
		{
			int result = -1;
			try {
				if (CVersionUpdateSystem.s_androidUtilityJavaClass != null) {
					result = CVersionUpdateSystem.s_androidUtilityJavaClass.CallStatic<int> ("GetNetworkType", new object[0]);
				}
			} catch (Exception ex) {
				Debug.Log ("networktype " + ex.ToString ());
			}
			return result;
		}

		private static void Android_ExitApp ()
		{
			CVersionUpdateSystem.s_androidUtilityJavaClass.CallStatic ("ExitApp", new object[0]);
		}
	}
}

using AGE;
using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CLoginSystem : Singleton<CLoginSystem>
	{
		public enum enLoginFormWidget
		{
			Juhua,
			UpdateSubModuleTimer,
			LoginContainer,
			ZoneContainer,
			StartGameContainer
		}

		public enum enLoadState
		{
			Login,
			Zone,
			StartGame
		}

		public const string str_visitor_uid = "visitorUid";

		public const int GET_ADDRESS_INTERVAL = 1000;

		public const string RECONNECT_TIMES = "SGAME_Reconnect_Times";

		public const string LAST_RECONNECT_FAILED_TIME = "SGAME_Reconnect_Failed_Time";

		public static string s_splashFormPath = "UGUI/Form/System/Login/Form_Splash_New.prefab";

		public static string sLoginFormPath = "UGUI/Form/System/Login/Form_Login.prefab";

		private CLoginSystem.enLoadState m_CurLoadState;

		private bool m_needRenewToken = true;

		private CUIFormScript m_Form;

		private IAsyncResult getAddressResult;

		private string login_port = string.Empty;

		public TdirUrl m_selectTdirUrl;

		private int m_UpdateApolloSwitchToLoginTimerSeq;

		private int m_CheckLoginTimeoutTimerSeq;

		private bool m_IsQuickLoginNotifySet;

		private int m_ReConnectTimes;

		private int m_ReConnectFailedTime;

		private bool m_ConnectLimitDeregulated;

		public float m_fLoginClickTime;

		public string m_nickName;

		public float m_fLoginBeginTime;

		private static bool m_bFirstLogin;

		private int m_zoneGroupSelectedIndex;

		private int m_zoneSelectIndex;

		public CLoginSystem.enLoadState CurLoadState
		{
			get
			{
				return this.m_CurLoadState;
			}
			set
			{
				this.m_CurLoadState = value;
				this.LoadSubModule();
			}
		}

		public override void Init()
		{
			base.Init();
			this.m_Form = null;
			this.getAddressResult = null;
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Login_Platform_Guest, new CUIEventManager.OnUIEventHandler(this.OnLogin_GuestLogin));
			instance.AddUIEventListener(enUIEventID.Login_Platform_QQ, new CUIEventManager.OnUIEventHandler(this.OnLogin_QQLogin));
			instance.AddUIEventListener(enUIEventID.Login_Platform_WX, new CUIEventManager.OnUIEventHandler(this.OnLogin_WxLogin));
			instance.AddUIEventListener(enUIEventID.Login_Platform_QRWechat, new CUIEventManager.OnUIEventHandler(this.OnLogin_QRWechatLogin));
			instance.AddUIEventListener(enUIEventID.Login_Platform_Quit, new CUIEventManager.OnUIEventHandler(this.OnLogin_Quit));
			instance.AddUIEventListener(enUIEventID.Login_Platform_WTLogin, new CUIEventManager.OnUIEventHandler(this.OnLogin_WtLogin));
			instance.AddUIEventListener(enUIEventID.Login_Platform_None, new CUIEventManager.OnUIEventHandler(this.OnLogin_None));
			instance.AddUIEventListener(enUIEventID.Login_Start_Game, new CUIEventManager.OnUIEventHandler(this.OnLogin_Start_Game));
			instance.AddUIEventListener(enUIEventID.Login_Platform_Logout, new CUIEventManager.OnUIEventHandler(this.OnPlatformLogout));
			instance.AddUIEventListener(enUIEventID.Login_Trans_Visitor_Yes, new CUIEventManager.OnUIEventHandler(this.OnConfirmTransferData));
			instance.AddUIEventListener(enUIEventID.Login_Trans_Visitor_No, new CUIEventManager.OnUIEventHandler(this.OnRejectTransferData));
			instance.AddUIEventListener(enUIEventID.Login_Change_Account_Yes, new CUIEventManager.OnUIEventHandler(this.OnChangeAccountYes));
			instance.AddUIEventListener(enUIEventID.Login_Change_Account_No, new CUIEventManager.OnUIEventHandler(this.OnChangeAccountNo));
			instance.AddUIEventListener(enUIEventID.Login_Enable_Start_Btn_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnEnableStartButton));
			instance.AddUIEventListener(enUIEventID.Login_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			instance.AddUIEventListener(enUIEventID.TDir_ZoneGroupSelect, new CUIEventManager.OnUIEventHandler(this.OnZoneGroupSelect));
			instance.AddUIEventListener(enUIEventID.TDir_ZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnZoneSelect));
			instance.AddUIEventListener(enUIEventID.TDir_LastZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnLastLoginZoneClick));
			instance.AddUIEventListener(enUIEventID.TDir_ShowZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnShowZoneSelcet));
			instance.AddUIEventListener(enUIEventID.TDir_BackToStartGame, new CUIEventManager.OnUIEventHandler(this.OnBackToStartGame));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(this.LoginSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Login_Canceled, new Action(this.LoginCancel));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloResult>(EventID.ApolloHelper_Login_Failed, new Action<ApolloResult>(this.LoginFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Login_Need_Real_Name_Auth, new Action(this.LoginNeedRealNameAuth));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloPlatform>(EventID.ApolloHelper_Platform_Not_Installed, new Action<ApolloPlatform>(this.PlatformNotInstalled));
			TdirMgr instance2 = MonoSingleton<TdirMgr>.GetInstance();
			instance2.SvrListLoaded = (TdirMgr.TdirManagerEvent)Delegate.Combine(instance2.SvrListLoaded, new TdirMgr.TdirManagerEvent(this.OnTDirLoadFinish));
			this.m_UpdateApolloSwitchToLoginTimerSeq = 0;
			this.m_IsQuickLoginNotifySet = false;
			this.m_ReConnectTimes = PlayerPrefs.GetInt("SGAME_Reconnect_Times", 0);
			this.m_ReConnectFailedTime = PlayerPrefs.GetInt("SGAME_Reconnect_Failed_Time", 0);
		}

		public override void UnInit()
		{
			this.m_Form = null;
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_Guest, new CUIEventManager.OnUIEventHandler(this.OnLogin_GuestLogin));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_QQ, new CUIEventManager.OnUIEventHandler(this.OnLogin_QQLogin));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_WTLogin, new CUIEventManager.OnUIEventHandler(this.OnLogin_WtLogin));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_None, new CUIEventManager.OnUIEventHandler(this.OnLogin_None));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_WX, new CUIEventManager.OnUIEventHandler(this.OnLogin_WxLogin));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_QRWechat, new CUIEventManager.OnUIEventHandler(this.OnLogin_QRWechatLogin));
			instance.RemoveUIEventListener(enUIEventID.Login_Platform_Quit, new CUIEventManager.OnUIEventHandler(this.OnLogin_Quit));
			instance.RemoveUIEventListener(enUIEventID.Login_Start_Game, new CUIEventManager.OnUIEventHandler(this.OnLogin_Start_Game));
			instance.RemoveUIEventListener(enUIEventID.Login_Trans_Visitor_Yes, new CUIEventManager.OnUIEventHandler(this.OnConfirmTransferData));
			instance.RemoveUIEventListener(enUIEventID.Login_Trans_Visitor_No, new CUIEventManager.OnUIEventHandler(this.OnRejectTransferData));
			instance.RemoveUIEventListener(enUIEventID.Login_Change_Account_Yes, new CUIEventManager.OnUIEventHandler(this.OnChangeAccountYes));
			instance.RemoveUIEventListener(enUIEventID.Login_Change_Account_No, new CUIEventManager.OnUIEventHandler(this.OnChangeAccountNo));
			instance.RemoveUIEventListener(enUIEventID.Login_Enable_Start_Btn_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnEnableStartButton));
			instance.RemoveUIEventListener(enUIEventID.Login_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			instance.RemoveUIEventListener(enUIEventID.TDir_ZoneGroupSelect, new CUIEventManager.OnUIEventHandler(this.OnZoneGroupSelect));
			instance.RemoveUIEventListener(enUIEventID.TDir_ZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnZoneSelect));
			instance.RemoveUIEventListener(enUIEventID.TDir_LastZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnLastLoginZoneClick));
			instance.RemoveUIEventListener(enUIEventID.TDir_ShowZoneSelect, new CUIEventManager.OnUIEventHandler(this.OnShowZoneSelcet));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(this.LoginSuccess));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Login_Canceled, new Action(this.LoginCancel));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloResult>(EventID.ApolloHelper_Login_Failed, new Action<ApolloResult>(this.LoginFailed));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Login_Need_Real_Name_Auth, new Action(this.LoginNeedRealNameAuth));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloPlatform>(EventID.ApolloHelper_Platform_Not_Installed, new Action<ApolloPlatform>(this.PlatformNotInstalled));
			TdirMgr instance2 = MonoSingleton<TdirMgr>.GetInstance();
			instance2.SvrListLoaded = (TdirMgr.TdirManagerEvent)Delegate.Remove(instance2.SvrListLoaded, new TdirMgr.TdirManagerEvent(this.OnTDirLoadFinish));
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_UpdateApolloSwitchToLoginTimerSeq);
			this.m_UpdateApolloSwitchToLoginTimerSeq = 0;
		}

		private void LoginCancel()
		{
			Debug.Log("*Login* Cancel" + Time.realtimeSinceStartup);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_CheckLoginTimeoutTimerSeq);
			ApolloPlatform curPlatform = Singleton<ApolloHelper>.GetInstance().CurPlatform;
			if (curPlatform != ApolloPlatform.Wechat)
			{
				if (curPlatform == ApolloPlatform.QQ)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Common_Login_QQ_Canceled", true, 1.5f, null, new object[0]);
					return;
				}
				if (curPlatform != ApolloPlatform.QRWechat)
				{
					Debug.LogWarning("user cancel on platform: " + Singleton<ApolloHelper>.instance.CurPlatform);
					return;
				}
			}
			Singleton<CUIManager>.GetInstance().OpenTips("Common_Login_Weixin_Canceled", true, 1.5f, null, new object[0]);
		}

		private void LoginFailed(ApolloResult r)
		{
			Debug.Log("*Login* Failed" + Time.realtimeSinceStartup);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_CheckLoginTimeoutTimerSeq);
			ApolloPlatform curPlatform = Singleton<ApolloHelper>.GetInstance().CurPlatform;
			if (curPlatform != ApolloPlatform.Wechat)
			{
				if (curPlatform != ApolloPlatform.QQ)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("授权失败，请重试", false, 1.5f, null, new object[0]);
					Debug.LogError(string.Format("login failed on platform: {0}", Singleton<ApolloHelper>.GetInstance().CurPlatform));
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Common_Login_QQ_Failed", true, 1.5f, null, new object[0]);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Common_Login_Weixin_Failed", true, 1.5f, null, new object[0]);
			}
		}

		private void QuickLoginDone(ApolloWakeupInfo wakeupInfo)
		{
			Singleton<ApolloHelper>.GetInstance().IsLastLaunchFrom3rdAPP = false;
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform = wakeupInfo.Platform;
			Singleton<ApolloHelper>.GetInstance().m_IsLastTriedPlatformSet = true;
			Singleton<ApolloHelper>.GetInstance().IsWXGameCenter = false;
			Singleton<ApolloHelper>.GetInstance().IsQQGameCenter = false;
			if (wakeupInfo.MessageExt == ApolloHelper.WX_MSGEXT_GAMECENTER)
			{
				if (!Singleton<LobbyLogic>.instance.isLogin)
				{
					Singleton<ApolloHelper>.GetInstance().IsWXGameCenter = true;
				}
			}
			else if (accountInfo == null || accountInfo.OpenId == wakeupInfo.OpenId)
			{
				MonoSingleton<ShareSys>.GetInstance().UnpackInviteSNSData(wakeupInfo.MessageExt, wakeupInfo.OpenId);
			}
			Debug.Log("WakeUpInfo:" + wakeupInfo.MessageExt);
			if (wakeupInfo.ExtensionInfo != null)
			{
				int i = 0;
				int count = wakeupInfo.ExtensionInfo.get_Count();
				while (i < count)
				{
					ApolloKVPair apolloKVPair = wakeupInfo.ExtensionInfo.get_Item(i);
					if (apolloKVPair != null && apolloKVPair.Key == ApolloHelper.QQ_USER_OPENID)
					{
						if (wakeupInfo.OpenId == string.Empty)
						{
							wakeupInfo.OpenId = apolloKVPair.Value;
						}
						break;
					}
					i++;
				}
				int j = 0;
				int count2 = wakeupInfo.ExtensionInfo.get_Count();
				while (j < count2)
				{
					ApolloKVPair apolloKVPair2 = wakeupInfo.ExtensionInfo.get_Item(j);
					if (apolloKVPair2 != null)
					{
						if (apolloKVPair2.Key == ApolloHelper.QQ_LAUNCH_FROM)
						{
							if (apolloKVPair2.Value == ApolloHelper.QQ_LAUNCH_FROM_GAMECENTER && !Singleton<LobbyLogic>.instance.isLogin)
							{
								Singleton<ApolloHelper>.GetInstance().IsQQGameCenter = true;
							}
						}
						else if (apolloKVPair2.Key == ApolloHelper.QQ_SHARE_GAMEDATA)
						{
							MonoSingleton<ShareSys>.GetInstance().UnpackInviteSNSData(apolloKVPair2.Value, wakeupInfo.OpenId);
						}
						Debug.Log("WakeUpInfo:key:" + apolloKVPair2.Key + "value:" + apolloKVPair2.Value);
					}
					j++;
				}
			}
			if (accountInfo != null && !string.IsNullOrEmpty(accountInfo.OpenId) && !string.IsNullOrEmpty(wakeupInfo.OpenId))
			{
				if (wakeupInfo.OpenId != accountInfo.OpenId)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Different_Account_Tip"), enUIEventID.Login_Change_Account_Yes, enUIEventID.Login_Change_Account_No, false);
					Singleton<ApolloHelper>.GetInstance().IsWXGameCenter = false;
					Singleton<ApolloHelper>.GetInstance().IsQQGameCenter = false;
				}
			}
			else if (wakeupInfo.state == ApolloWakeState.NeedLogin && Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState)
			{
				if (Singleton<ApolloHelper>.GetInstance().IsWXGameCenter)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Login_Platform_WX);
				}
				else if (Singleton<ApolloHelper>.GetInstance().IsQQGameCenter)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Login_Platform_QQ);
				}
			}
		}

		private void PlatformNotInstalled(ApolloPlatform platform)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_CheckLoginTimeoutTimerSeq);
			if (platform != ApolloPlatform.Wechat)
			{
				if (platform != ApolloPlatform.QQ)
				{
				}
			}
			else
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("Common_Platform_Weixin");
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Weixin_Not_Installed"), text), enUIEventID.Login_Platform_QRWechat, enUIEventID.None, false);
			}
		}

		private void LoginSuccess(ApolloAccountInfo info)
		{
			Debug.Log("*Login* SUCC" + Time.realtimeSinceStartup);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_UpdateApolloSwitchToLoginTimerSeq);
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_CheckLoginTimeoutTimerSeq);
			MonoSingleton<CTongCaiSys>.GetInstance().StartCheck(info);
			this.RegisterXGPush(info.OpenId);
			MonoSingleton<TGPSDKSys>.GetInstance().SetOpenID(info.OpenId);
			if (!NoticeSys.m_bShowLoginBefore)
			{
				Singleton<CTimerManager>.GetInstance().AddTimer(1, 1, delegate(int seq)
				{
					NoticeSys.m_bShowLoginBefore = true;
					Singleton<ApolloHelper>.GetInstance().ShowNotice(0, "1");
				});
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("totaltime", (Time.time - Singleton<CLoginSystem>.GetInstance().m_fLoginClickTime).ToString()));
			list.Add(new KeyValuePair<string, string>("errorCode", "0"));
			list.Add(new KeyValuePair<string, string>("error_msg", "null"));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_MSDKClientAuth", list, true);
		}

		private void LoginNeedRealNameAuth()
		{
			Debug.Log("*Login* NeedRealNameAuth" + Time.realtimeSinceStartup);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_CheckLoginTimeoutTimerSeq);
		}

		private void RegisterXGPush(string openID)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (androidJavaClass != null)
			{
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (@static != null)
				{
					Debug.Log("*Login*RegisterXGPush in unity " + Time.realtimeSinceStartup);
					@static.Call("RegisterXGPush", new object[]
					{
						openID
					});
				}
			}
		}

		private void UpdateApolloSwitchToLoginFlag(int seq)
		{
			Singleton<ApolloHelper>.GetInstance().IsSwitchToLoginPlatform = false;
		}

		private void LoginTimeout(int seq)
		{
			Debug.Log("*Login* LoginTimeout" + Time.realtimeSinceStartup);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			this.m_CheckLoginTimeoutTimerSeq = 0;
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Login_Canceled);
		}

		public void Draw()
		{
			if (!this.m_IsQuickLoginNotifySet)
			{
				Singleton<ApolloHelper>.GetInstance().RegisterQuickLoginHandler(new ApolloQuickLoginNotify(this.QuickLoginDone));
				this.m_IsQuickLoginNotifySet = true;
			}
			if (this.m_UpdateApolloSwitchToLoginTimerSeq == 0)
			{
				this.m_UpdateApolloSwitchToLoginTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(1000, -1, new CTimer.OnTimeUpHandler(this.UpdateApolloSwitchToLoginFlag));
			}
			Singleton<CUIManager>.GetInstance().OpenForm(CLoginSystem.s_splashFormPath, false, true);
			this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(CLoginSystem.sLoginFormPath, false, true);
			if (!CLoginSystem.m_bFirstLogin)
			{
				CLoginSystem.m_bFirstLogin = true;
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", "NULL"));
				list.Add(new KeyValuePair<string, string>("totaltime", Time.time.ToString()));
				list.Add(new KeyValuePair<string, string>("status", "0"));
				list.Add(new KeyValuePair<string, string>("errorCode", "0"));
				list.Add(new KeyValuePair<string, string>("error_msg", "null"));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_GameStart", list, true);
			}
			this.InitLoadState();
			if (!NoticeSys.m_bShowLoginBefore)
			{
				Singleton<CTimerManager>.GetInstance().AddTimer(1, 1, delegate(int seq)
				{
					NoticeSys.m_bShowLoginBefore = true;
					Singleton<ApolloHelper>.GetInstance().ShowNotice(0, "1");
				});
			}
		}

		private void InitLoadState()
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(this.m_needRenewToken);
			if (accountInfo != null)
			{
				if (Singleton<ApolloHelper>.GetInstance().JudgeLoginAccountInfo(ref accountInfo))
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, accountInfo);
				}
				else
				{
					Singleton<ApolloHelper>.GetInstance().Logout();
					this.CurLoadState = CLoginSystem.enLoadState.Login;
				}
			}
			else
			{
				this.CurLoadState = CLoginSystem.enLoadState.Login;
			}
		}

		public void CloseLogin()
		{
			if (this.m_Form != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CLoginSystem.s_splashFormPath);
				Singleton<CUIManager>.GetInstance().CloseForm(CLoginSystem.sLoginFormPath);
				this.m_Form = null;
			}
			this.m_ConnectLimitDeregulated = true;
			this.m_ReConnectTimes = 0;
			PlayerPrefs.SetInt("SGAME_Reconnect_Times", 0);
			PlayerPrefs.SetInt("SGAME_Reconnect_Failed_Time", 0);
		}

		private void RefreshZonePnl(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			containerTransform.gameObject.CustomSetActive(true);
			this.SetTdirZoneGroupList(containerTransform);
			this.SetLastLoginZone(containerTransform);
		}

		private void RefreshStartGamePnl(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			containerTransform.gameObject.CustomSetActive(true);
			Transform transform = containerTransform.Find("pnlStartGame/Panel");
			if (transform != null)
			{
				this.SetZone(transform.gameObject, this.m_selectTdirUrl);
			}
			this.ShowVersion(containerTransform);
			this.ConnectLimit(false, ApolloResult.Success);
			this.ShowSnsName(containerTransform);
		}

		private void RefreshLoginPnl(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			Transform transform = containerTransform.Find("pnlMobileLogin/btnGroup/btnWX");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(true);
			}
			Transform transform2 = containerTransform.Find("pnlMobileLogin/btnNone");
			if (transform2 != null)
			{
				if (Singleton<ApolloHelper>.GetInstance().IsNoneModeSupport)
				{
					transform2.gameObject.SetActive(true);
				}
				else
				{
					transform2.gameObject.SetActive(false);
				}
			}
			containerTransform.gameObject.CustomSetActive(true);
		}

		public void LoadUI()
		{
			DebugHelper.Assert(this.m_Form != null, "LoadUI:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			switch (this.CurLoadState)
			{
			case CLoginSystem.enLoadState.Login:
				CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Login/AndroidLogin", "pnlMobileLogin", this.m_Form.GetWidget(2), this.m_Form);
				break;
			case CLoginSystem.enLoadState.Zone:
				CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Login/SelectZone", "pnlSelectZone", this.m_Form.GetWidget(3), this.m_Form);
				break;
			case CLoginSystem.enLoadState.StartGame:
				CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Login/StartGame", "pnlStartGame", this.m_Form.GetWidget(4), this.m_Form);
				break;
			}
		}

		public bool UILoaded()
		{
			DebugHelper.Assert(this.m_Form != null, "UILoaded:Login Form Is Null");
			if (this.m_Form == null)
			{
				return false;
			}
			GameObject x = null;
			switch (this.CurLoadState)
			{
			case CLoginSystem.enLoadState.Login:
				x = Utility.FindChild(this.m_Form.gameObject, "LoginContainer/pnlMobileLogin");
				break;
			case CLoginSystem.enLoadState.Zone:
				x = Utility.FindChild(this.m_Form.gameObject, "ZoneContainer/pnlSelectZone");
				break;
			case CLoginSystem.enLoadState.StartGame:
				x = Utility.FindChild(this.m_Form.gameObject, "StartGameContainer/pnlStartGame");
				break;
			}
			return !(x == null);
		}

		public void LoadSubModule()
		{
			DebugHelper.Assert(this.m_Form != null, "LoadSubModule:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(2);
			GameObject widget2 = this.m_Form.GetWidget(4);
			GameObject widget3 = this.m_Form.GetWidget(3);
			bool flag = this.UILoaded();
			switch (this.m_CurLoadState)
			{
			case CLoginSystem.enLoadState.Login:
				widget.CustomSetActive(false);
				break;
			case CLoginSystem.enLoadState.Zone:
				widget3.CustomSetActive(false);
				break;
			case CLoginSystem.enLoadState.StartGame:
				widget2.CustomSetActive(false);
				break;
			}
			if (!flag)
			{
				this.m_Form.GetWidget(0).CustomSetActive(true);
				this.LoadUI();
			}
			if (!flag)
			{
				GameObject widget4 = this.m_Form.GetWidget(1);
				if (widget4 != null)
				{
					CUITimerScript component = widget4.GetComponent<CUITimerScript>();
					if (component != null)
					{
						component.ReStartTimer();
					}
				}
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Login_Update_Sub_Module);
			}
		}

		private void OnUpdateSubModule(CUIEvent uiEvent)
		{
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			DebugHelper.Assert(this.m_Form != null, "OnUpdateSubModule:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			this.m_Form.GetWidget(0).CustomSetActive(false);
			GameObject widget = this.m_Form.GetWidget(2);
			GameObject widget2 = this.m_Form.GetWidget(4);
			GameObject widget3 = this.m_Form.GetWidget(3);
			if (widget == null || widget2 == null || widget3 == null)
			{
				return;
			}
			switch (this.CurLoadState)
			{
			case CLoginSystem.enLoadState.Login:
				this.RefreshLoginPnl(widget.transform);
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				break;
			case CLoginSystem.enLoadState.Zone:
				this.RefreshZonePnl(widget3.transform);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(true);
				break;
			case CLoginSystem.enLoadState.StartGame:
				this.RefreshStartGamePnl(widget2.transform);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(false);
				break;
			}
		}

		private void OnLogin_GuestLogin(CUIEvent uiEvent)
		{
			this.m_fLoginClickTime = Time.time;
			Singleton<ApolloHelper>.GetInstance().Login(ApolloPlatform.Guest, false, 0uL, null);
		}

		private void test_StartSingleGame()
		{
			ActionManager.Instance.frameMode = true;
			Singleton<CRoleInfoManager>.GetInstance().SetMaterUUID(0uL);
			Singleton<CRoleInfoManager>.GetInstance().CreateRoleInfo(enROLEINFO_TYPE.PLAYER, 0uL, 1001);
			COMDT_NEWBIE_STATUS_BITS cOMDT_NEWBIE_STATUS_BITS = new COMDT_NEWBIE_STATUS_BITS();
			for (int i = 0; i < cOMDT_NEWBIE_STATUS_BITS.BitsDetail.Length; i++)
			{
				cOMDT_NEWBIE_STATUS_BITS.BitsDetail[i] = 18446744073709551615uL;
			}
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitGuidedStateBits(cOMDT_NEWBIE_STATUS_BITS);
			COMDT_CLIENT_BITS cOMDT_CLIENT_BITS = new COMDT_CLIENT_BITS();
			for (int j = 0; j < cOMDT_CLIENT_BITS.BitsDetail.Length; j++)
			{
				cOMDT_CLIENT_BITS.BitsDetail[j] = 18446744073709551615uL;
			}
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitNewbieAchieveBits(cOMDT_CLIENT_BITS);
			COMDT_NEWCLIENT_BITS cOMDT_NEWCLIENT_BITS = new COMDT_NEWCLIENT_BITS();
			for (int k = 0; k < cOMDT_NEWCLIENT_BITS.BitsDetail.Length; k++)
			{
				cOMDT_NEWCLIENT_BITS.BitsDetail[k] = 18446744073709551615uL;
			}
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitClientBits(cOMDT_NEWCLIENT_BITS);
			CSkinInfo.InitHeroSkinDicData();
			SCPKG_STARTSINGLEGAMERSP sCPKG_STARTSINGLEGAMERSP = new SCPKG_STARTSINGLEGAMERSP();
			sCPKG_STARTSINGLEGAMERSP.bGameType = 2;
			sCPKG_STARTSINGLEGAMERSP.iLevelId = 7;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc = new CSDT_BATTLE_PLAYER_BRIEF();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.bNum = 2;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].bObjType = 1;
			StringHelper.StringToUTF8Bytes("test-player1", ref sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].szName);
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].dwObjId = 1u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].bObjCamp = 1;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].dwLevel = 1u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].stDetail.stPlayerOfAcnt = new COMDT_PLAYERINFO_OF_ACNT();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].stDetail.stPlayerOfAcnt.ullUid = 0uL;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = 105u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = 0;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].bObjType = 2;
			StringHelper.StringToUTF8Bytes("npc1", ref sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].szName);
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].dwObjId = 2u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].bObjCamp = 2;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].dwLevel = 1u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].stDetail.stPlayerOfAcnt = new COMDT_PLAYERINFO_OF_ACNT();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].stDetail.stPlayerOfAcnt.ullUid = 0uL;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = 106u;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[1].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = 0;
			CSPkg cSPkg = CSPkg.New();
			cSPkg.stPkgData.stStartSingleGameRsp = sCPKG_STARTSINGLEGAMERSP;
			LobbyMsgHandler.onSingleGameLoad(cSPkg);
		}

		private void test_StartMultiGame()
		{
			GameObject gameObject = new GameObject("My Network Manager");
			Object.DontDestroyOnLoad(gameObject);
			MyMain myMain = gameObject.AddComponent<MyMain>();
			myMain.Connect();
		}

		private void OnLogin_QQLogin(CUIEvent uiEvent)
		{
			this.test_StartSingleGame();
		}

		private void OnLogin_WtLogin(CUIEvent uiEvent)
		{
			this.m_fLoginClickTime = Time.time;
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(2);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlWtLogin");
			DebugHelper.Assert(transform != null, "WtLogin Panel Is Null");
			if (transform == null)
			{
				return;
			}
			InputField component = transform.Find("btnGroup/UinInput").gameObject.GetComponent<InputField>();
			InputField component2 = transform.Find("btnGroup/PswInput").gameObject.GetComponent<InputField>();
			InputField component3 = transform.Find("btnGroup/OpenIdInput").gameObject.GetComponent<InputField>();
			if (component3 != null && component3.get_text().get_Length() > 0)
			{
				stUIEventParams par = default(stUIEventParams);
				par.tagStr = component3.get_text();
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Login_Platform_None, par);
				return;
			}
			if (component != null && component2 != null && component.get_text() != null && component.get_text().get_Length() > 0 && component2.get_text().get_Length() > 0)
			{
				PlayerPrefs.SetString("SGamePlayerUinCache", component.get_text());
				PlayerPrefs.SetString("SGamePlayerPwdCache", component2.get_text());
				Singleton<ApolloHelper>.GetInstance().Login(ApolloPlatform.WTLogin, false, Convert.ToUInt64(component.get_text()), component2.get_text());
			}
		}

		private void OnLogin_WxLogin(CUIEvent uiEvent)
		{
			this.test_StartMultiGame();
		}

		private void OnLogin_QRWechatLogin(CUIEvent uiEvent)
		{
			this.m_fLoginClickTime = Time.time;
			if (this.m_CheckLoginTimeoutTimerSeq == 0)
			{
				this.m_CheckLoginTimeoutTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(15000, 1, new CTimer.OnTimeUpHandler(this.LoginTimeout));
			}
			Singleton<ApolloHelper>.GetInstance().Login(ApolloPlatform.QRWechat, false, 0uL, null);
		}

		private void OnLogin_None(CUIEvent uiEvent)
		{
			this.m_fLoginClickTime = Time.time;
			if (Singleton<ApolloHelper>.GetInstance().IsNoneModeSupport)
			{
				string text = null;
				try
				{
					text = Application.persistentDataPath + "/customOpenId.txt";
					StreamReader streamReader = new StreamReader(text);
					if ((ApolloConfig.CustomOpenId = streamReader.ReadLine().Trim()) != null)
					{
						Debug.Log(string.Format("custom openid: {0}", ApolloConfig.CustomOpenId));
					}
				}
				catch (Exception ex)
				{
					ApolloConfig.CustomOpenId = null;
					Debug.Log(string.Format("File Not Found filePath: {0}, Exception {1}", text, ex.ToString()));
					Singleton<CUIManager>.GetInstance().OpenTips("File Not Found!", false, 1.5f, null, new object[0]);
					return;
				}
			}
			Singleton<ApolloHelper>.GetInstance().Login(ApolloPlatform.None, false, 0uL, null);
		}

		private void OnLogin_Start_Game(CUIEvent uiEvent)
		{
			if (MonoSingleton<TdirMgr>.GetInstance() == null)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			this.m_ReConnectTimes++;
			this.m_ConnectLimitDeregulated = false;
			Singleton<LobbySvrMgr>.GetInstance().connectFailHandler -= new LobbySvrMgr.ConnectFailHandler(this.OnLobbyConnectFail);
			Singleton<LobbySvrMgr>.GetInstance().connectFailHandler += new LobbySvrMgr.ConnectFailHandler(this.OnLobbyConnectFail);
			MonoSingleton<TdirMgr>.GetInstance().ChooseGameServer(this.m_selectTdirUrl);
		}

		private void OnLogin_Quit(CUIEvent uiEvent)
		{
			SGameApplication.Quit();
		}

		private void OnPlatformLogout(CUIEvent uiEvent)
		{
			this.m_needRenewToken = false;
			if (!Singleton<ApolloHelper>.GetInstance().Logout())
			{
				Singleton<ApolloHelper>.GetInstance().Logout();
			}
			Singleton<GameLogic>.GetInstance().OnPlayerLogout();
			Singleton<GameStateCtrl>.GetInstance().GotoState("LoginState");
		}

		private void OnLobbyConnectFail(ApolloResult result)
		{
			Debug.Log("CLoginSystem OnLobbyConnectFail called!");
			Singleton<LobbySvrMgr>.GetInstance().connectFailHandler -= new LobbySvrMgr.ConnectFailHandler(this.OnLobbyConnectFail);
			this.ConnectLimit(true, result);
		}

		private void ConnectLimit(bool showTips = true, ApolloResult result = ApolloResult.Success)
		{
			bool flag = false;
			float num = 0f;
			DebugHelper.Assert(this.m_Form != null, "ConnectLimit:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(4);
			DebugHelper.Assert(widget != null, "Start Game Container Is Null");
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlStartGame/btnStartGame");
			if (transform == null)
			{
				return;
			}
			Transform transform2 = transform.Find("CountDown");
			if (transform2 == null)
			{
				return;
			}
			Button component = transform.GetComponent<Button>();
			if (component == null)
			{
				return;
			}
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(transform.gameObject, "CountDown/timerEnableStartBtn");
			if (componetInChild == null)
			{
				return;
			}
			bool flag2 = false;
			bool flag3 = false;
			if (!this.m_ConnectLimitDeregulated && this.m_ReConnectTimes >= 6)
			{
				flag2 = true;
				num = 300f;
			}
			else if (!this.m_ConnectLimitDeregulated && this.m_ReConnectTimes >= 3)
			{
				flag2 = true;
				num = 10f;
			}
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			int num2 = (int)DateTime.get_Now().Subtract(dateTime.AddSeconds((double)this.m_ReConnectFailedTime)).get_TotalSeconds();
			num -= (float)num2;
			if (num <= 0f)
			{
				flag2 = false;
			}
			string text = string.Empty;
			switch (result)
			{
			case ApolloResult.PeerStopSession:
			case ApolloResult.TokenSvrError:
				goto IL_1AF;
			case ApolloResult.PkgNotCompleted:
			case ApolloResult.SendFailed:
				IL_193:
				switch (result)
				{
				case ApolloResult.Success:
					goto IL_240;
				case ApolloResult.NetworkException:
					flag3 = true;
					text = "网络异常";
					goto IL_240;
				case ApolloResult.Timeout:
					goto IL_1AF;
				}
				if (result == ApolloResult.TokenInvalid)
				{
					flag = true;
					flag3 = true;
					text = "授权失败，请重新登录";
					goto IL_240;
				}
				if (result != ApolloResult.ConnectFailed)
				{
					flag3 = true;
					text = string.Format("未知错误({0})", result);
					goto IL_240;
				}
				goto IL_236;
			case ApolloResult.StayInQueue:
				flag3 = true;
				text = "当前服务器人数过多需要排队";
				goto IL_240;
			case ApolloResult.SvrIsFull:
				flag3 = true;
				text = "当前服务器已满";
				goto IL_240;
			}
			goto IL_193;
			IL_1AF:
			IL_236:
			flag3 = true;
			text = "服务器未响应";
			IL_240:
			if (flag2)
			{
				if (showTips)
				{
					text = string.Format("{0}，{1}", text, string.Format("请等待{0}再尝试开始游戏", (num > 60f) ? string.Format("{0}分钟", (int)(num / 60f)) : string.Format("{0}秒", num)));
				}
				transform2.gameObject.CustomSetActive(true);
				CUICommonSystem.SetButtonEnable(component, false, false, true);
				componetInChild.SetTotalTime(num);
				componetInChild.StartTimer();
			}
			else
			{
				transform2.gameObject.CustomSetActive(false);
				CUICommonSystem.SetButtonEnable(component, true, true, true);
			}
			if (flag3)
			{
				PlayerPrefs.SetInt("SGAME_Reconnect_Times", this.m_ReConnectTimes);
				PlayerPrefs.SetInt("SGAME_Reconnect_Failed_Time", (int)DateTime.get_Now().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).get_TotalSeconds());
			}
			if (flag)
			{
				this.m_needRenewToken = false;
				Singleton<LobbyLogic>.GetInstance().GotoAccLoginPage();
			}
			if (showTips && text != string.Empty)
			{
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
			}
		}

		private void OnEnableStartButton(CUIEvent uiEvent)
		{
			this.m_ConnectLimitDeregulated = true;
			DebugHelper.Assert(this.m_Form != null, "OnEnableStartButton:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(4);
			DebugHelper.Assert(widget != null, "Start Game Container Is Null");
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("btnStartGame");
			if (transform == null)
			{
				return;
			}
			Transform transform2 = transform.Find("CountDown");
			Button component = transform.GetComponent<Button>();
			if (component == null)
			{
				return;
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
				CUICommonSystem.SetButtonEnable(component, true, true, true);
			}
		}

		private void OnZoneGroupSelect(CUIEvent uiEvent)
		{
			this.m_zoneGroupSelectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			this.SetTdirZoneList(this.m_zoneGroupSelectedIndex);
		}

		private void OnZoneSelect(CUIEvent uiEvent)
		{
			this.m_zoneSelectIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			List<TdirSvrGroup> svrUrlList = MonoSingleton<TdirMgr>.GetInstance().SvrUrlList;
			if (this.m_zoneGroupSelectedIndex >= svrUrlList.get_Count() || this.m_zoneSelectIndex >= svrUrlList.get_Item(this.m_zoneGroupSelectedIndex).tdirUrls.get_Count())
			{
				DebugHelper.Assert(false, "选区选服数组越界");
				return;
			}
			this.m_selectTdirUrl = svrUrlList.get_Item(this.m_zoneGroupSelectedIndex).tdirUrls.get_Item(this.m_zoneSelectIndex);
			this.CurLoadState = CLoginSystem.enLoadState.StartGame;
		}

		private void OnLastLoginZoneClick(CUIEvent uiEvent)
		{
			if (MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.nodeID == 0 || MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.name == null || MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.name.get_Length() == 0)
			{
				return;
			}
			this.ShowEneterViewWithSelected();
		}

		private void OnShowZoneSelcet(CUIEvent uiEvent)
		{
			this.CurLoadState = CLoginSystem.enLoadState.Zone;
		}

		private void OnConfirmTransferData(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1027u);
			cSPkg.stPkgData.stRspAcntTransVisitorSvrData.bAgree = 1;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			if (PlayerPrefs.HasKey("visitorUid"))
			{
				PlayerPrefs.DeleteKey("visitorUid");
			}
		}

		private void OnRejectTransferData(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1027u);
			cSPkg.stPkgData.stRspAcntTransVisitorSvrData.bAgree = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnBackToStartGame(CUIEvent uiEvent)
		{
			this.CurLoadState = CLoginSystem.enLoadState.StartGame;
		}

		private void OnChangeAccountYes(CUIEvent uiEvent)
		{
			this.m_needRenewToken = false;
			IState currentState = Singleton<GameStateCtrl>.GetInstance().GetCurrentState();
			if (currentState is LoginState)
			{
				Debug.Log("*Login* OnChangeAccountYes LoginState " + Time.realtimeSinceStartup);
				Singleton<ApolloHelper>.GetInstance().SwitchUser(true);
				if (Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform == ApolloPlatform.Wechat || Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform == ApolloPlatform.QRWechat || Singleton<ApolloHelper>.GetInstance().IsLastLaunchFrom3rdAPP)
				{
					Singleton<ApolloHelper>.GetInstance().IsLastLaunchFrom3rdAPP = false;
					Singleton<LobbyLogic>.GetInstance().GotoAccLoginPage();
					ApolloPlatform lastTriedPlatform = Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform;
					Singleton<ApolloHelper>.GetInstance().m_IsLastTriedPlatformSet = false;
					Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform = ApolloPlatform.None;
					Singleton<ApolloHelper>.GetInstance().Login(lastTriedPlatform, false, 0uL, null);
				}
			}
			else
			{
				Debug.Log("*Login* OnChangeAccountYes Not LoginState " + Time.realtimeSinceStartup);
				Singleton<ApolloHelper>.GetInstance().Logout();
				Singleton<ApolloHelper>.GetInstance().SwitchUser(false);
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1016u);
				cSPkg.stPkgData.stGameLogoutReq.iLogoutType = 0;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		private void OnChangeAccountNo(CUIEvent uiEvent)
		{
			Singleton<ApolloHelper>.GetInstance().SwitchUser(false);
			Singleton<ApolloHelper>.GetInstance().m_IsLastTriedPlatformSet = false;
			Singleton<ApolloHelper>.GetInstance().m_LastTriedPlatform = ApolloPlatform.None;
		}

		public void SetTdirZoneGroupList(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			Transform transform = containerTransform.Find("pnlSelectZone/TopCommon/Panel_Menu/ListZoneGroup");
			DebugHelper.Assert(transform != null, "Zone Group List Is Null");
			if (transform == null)
			{
				return;
			}
			CUIListScript component = transform.GetComponent<CUIListScript>();
			List<TdirSvrGroup> svrUrlList = MonoSingleton<TdirMgr>.GetInstance().SvrUrlList;
			component.SetElementAmount(svrUrlList.get_Count());
			for (int i = 0; i < svrUrlList.get_Count(); i++)
			{
				GameObject gameObject = component.GetElemenet(i).gameObject;
				Text component2 = gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
				component2.set_text(svrUrlList.get_Item(i).name);
			}
			component.SelectElement(this.m_zoneGroupSelectedIndex, true);
		}

		public void SetTdirZoneList(int index)
		{
			GameObject widget = this.m_Form.GetWidget(3);
			DebugHelper.Assert(widget != null, "Zone Contaier Is Null");
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlSelectZone/ListZones");
			DebugHelper.Assert(transform != null, "Zone List Is Null");
			if (transform == null)
			{
				return;
			}
			CUIListScript component = transform.GetComponent<CUIListScript>();
			List<TdirSvrGroup> svrUrlList = MonoSingleton<TdirMgr>.GetInstance().SvrUrlList;
			if (svrUrlList != null && index >= 0 && index < svrUrlList.get_Count())
			{
				List<TdirUrl> tdirUrls = svrUrlList.get_Item(index).tdirUrls;
				component.SetElementAmount(tdirUrls.get_Count());
				for (int i = 0; i < tdirUrls.get_Count(); i++)
				{
					GameObject gameObject = component.GetElemenet(i).gameObject;
					GameObject gameObject2 = gameObject.transform.Find("Panel").gameObject;
					this.SetZone(gameObject2, tdirUrls.get_Item(i));
				}
			}
		}

		public void OnTDirLoadFinish()
		{
			if (!(Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LoginState))
			{
				return;
			}
			DebugHelper.Assert(this.m_Form != null, "OnTDirLoadFinish:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			if (MonoSingleton<TdirMgr>.GetInstance().LastLoginUrl.nodeID == 0)
			{
				this.m_zoneGroupSelectedIndex = 1;
			}
			if (MonoSingleton<TdirMgr>.GetInstance().CheckTdirUrlValid(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir))
			{
				this.m_selectTdirUrl = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir;
				this.CurLoadState = CLoginSystem.enLoadState.StartGame;
			}
			else
			{
				this.CurLoadState = CLoginSystem.enLoadState.Zone;
			}
		}

		public void SetZone(GameObject zonePanel, TdirUrl tdirUrl)
		{
			Text component = zonePanel.transform.Find("ZoneName").gameObject.GetComponent<Text>();
			component.set_text(tdirUrl.name);
			Text component2 = zonePanel.transform.Find("State").gameObject.GetComponent<Text>();
			if (tdirUrl.flag == SvrFlag.New)
			{
				component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Zone_NEW"));
			}
			else if (tdirUrl.flag == SvrFlag.Recommend)
			{
				component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Zone_Recommend"));
			}
			else if (tdirUrl.flag == SvrFlag.Hot)
			{
				component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Zone_HOT"));
			}
			else
			{
				component2.set_text(string.Empty);
			}
			GameObject gameObject = zonePanel.transform.Find("StateImage").gameObject;
			if (!MonoSingleton<TdirMgr>.GetInstance().CheckTdirUrlValid(tdirUrl))
			{
				gameObject.CustomSetActive(false);
			}
			else
			{
				Animator component3 = zonePanel.GetComponent<Animator>();
				if (gameObject != null && component3 != null)
				{
					gameObject.CustomSetActive(true);
					if (tdirUrl.statu == TdirSvrStatu.CROWDED)
					{
						component3.Play("ServerState_1");
					}
					else if (tdirUrl.statu == TdirSvrStatu.HEAVY)
					{
						component3.Play("ServerState_1");
					}
					else if (tdirUrl.statu == TdirSvrStatu.FINE)
					{
						component3.Play("ServerState_3");
					}
					else if (tdirUrl.statu == TdirSvrStatu.UNAVAILABLE)
					{
						component3.Play("ServerState_4");
					}
				}
			}
		}

		public void SetLastLoginZone(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			Transform transform = containerTransform.Find("pnlSelectZone/LastLogin/Panel");
			if (transform == null)
			{
				return;
			}
			TdirUrl lastLoginUrl = MonoSingleton<TdirMgr>.GetInstance().LastLoginUrl;
			this.SetZone(transform.gameObject, lastLoginUrl);
		}

		private void SetNickName(ApolloRelation aRelation)
		{
			DebugHelper.Assert(this.m_Form != null, "SetNickName:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(4);
			DebugHelper.Assert(widget != null, "Start Game Container Is Null");
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlStartGame/pnlSnsName");
			Transform transform2 = widget.transform.Find("pnlStartGame/pnlSnsName/snsName");
			if (transform != null)
			{
				if (transform2 != null && aRelation.Result == ApolloResult.Success)
				{
					transform.gameObject.CustomSetActive(true);
					Text component = transform2.GetComponent<Text>();
					using (List<ApolloPerson>.Enumerator enumerator = aRelation.Persons.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ApolloPerson current = enumerator.get_Current();
							component.set_text(current.NickName);
							this.m_nickName = current.NickName;
						}
					}
				}
				else
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		public void ShowEneterViewWithSelected()
		{
			this.m_selectTdirUrl = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir;
			this.CurLoadState = CLoginSystem.enLoadState.StartGame;
		}

		private void ShowVersion(Transform containerTransform)
		{
			if (containerTransform == null)
			{
				return;
			}
			Transform transform = containerTransform.Find("pnlStartGame/pnlVersion/Text");
			if (transform != null)
			{
				Text component = transform.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(string.Format("App v{0} Res v{1} Build{2} R{3}", new object[]
					{
						GameFramework.AppVersion,
						CVersion.GetUsedResourceVersion(),
						CVersion.GetBuildNumber(),
						CVersion.GetRevisonNumber()
					}));
				}
			}
		}

		private void ShowSnsName(Transform containerTransform)
		{
			DebugHelper.Assert(this.m_Form != null, "ShowSnsName:Login Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			if (containerTransform == null)
			{
				return;
			}
			if (!Singleton<ApolloHelper>.GetInstance().GetMySnsInfo(new OnRelationNotifyHandle(this.SetNickName)))
			{
				Debug.LogError("Get sns failed");
				Transform transform = containerTransform.Find("pnlStartGame/pnlSnsName");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}
	}
}

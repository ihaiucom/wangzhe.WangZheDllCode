using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class ApolloHelper : Singleton<ApolloHelper>
{
	public const string sOpenIdFilePath = "/customOpenId.txt";

	public static string QQ_SHARE_GAMEDATA = "gamedata";

	public static string QQ_LAUNCH_FROM = "launchfrom";

	public static string QQ_LAUNCH_FROM_GAMECENTER = "sq_gamecenter";

	public static string WX_MSGEXT_GAMECENTER = "WX_GameCenter";

	public static string QQ_USER_OPENID = "user_openid";

	private ApolloInfo info;

	private IApolloAccountService accountService;

	private bool m_IsSwitchToLoginPlatform;

	private bool m_IsLoginEventHandlerRegistered;

	private bool m_IsLoginReturn;

	public ApolloPlatform CurPlatform;

	private bool m_IsWXGameCenter;

	private bool m_IsQQGameCenter;

	public string m_LastOpenID;

	public ApolloPlatform m_LastTriedPlatform;

	public bool m_IsLastTriedPlatformSet;

	public bool IsLastLaunchFrom3rdAPP;

	public bool IsNoneModeSupport;

	private int loginTimerSeq;

	private IApolloPayService payService;

	private RegisterInfo registerInfo;

	private IApolloSnsService snsService;

	private IApolloReportService reportService;

	private IApolloQuickLoginService quickLoginService;

	public bool m_bPayQQVIP;

	public bool m_bShareQQBox;

	public bool IsLoginReturn
	{
		get
		{
			return this.m_IsLoginReturn;
		}
	}

	public bool IsWXGameCenter
	{
		get
		{
			return this.m_IsWXGameCenter;
		}
		set
		{
			this.m_IsWXGameCenter = value;
		}
	}

	public bool IsQQGameCenter
	{
		get
		{
			return this.m_IsQQGameCenter;
		}
		set
		{
			this.m_IsQQGameCenter = value;
		}
	}

	public bool IsSwitchToLoginPlatform
	{
		get
		{
			return this.m_IsSwitchToLoginPlatform;
		}
		set
		{
			this.m_IsSwitchToLoginPlatform = value;
		}
	}

	public ApolloHelper()
	{
		this.info = new ApolloInfo(ApolloConfig.QQAppID, ApolloConfig.WXAppID, ApolloConfig.maxMessageBufferSize, string.Empty);
		IApollo.Instance.Initialize(this.info);
		IApollo.Instance.SetApolloLogger(ApolloLogPriority.None, null);
		this.accountService = IApollo.Instance.GetAccountService();
		this.payService = null;
		this.registerInfo = new RegisterInfo();
		this.snsService = (IApollo.Instance.GetService(1) as IApolloSnsService);
		this.reportService = (IApollo.Instance.GetService(3) as IApolloReportService);
		this.quickLoginService = (IApollo.Instance.GetService(7) as IApolloQuickLoginService);
		this.m_IsSwitchToLoginPlatform = false;
		this.m_IsLoginEventHandlerRegistered = false;
		this.m_IsLoginReturn = false;
		this.CurPlatform = ApolloPlatform.None;
		this.m_LastOpenID = null;
		this.m_LastTriedPlatform = ApolloPlatform.None;
		this.m_IsLastTriedPlatformSet = false;
		this.IsLastLaunchFrom3rdAPP = false;
		if (File.Exists(Application.persistentDataPath + "/customOpenId.txt"))
		{
			this.IsNoneModeSupport = true;
		}
		else
		{
			this.IsNoneModeSupport = false;
		}
		HttpDnsPolicy.Init();
	}

	public void ChangeLogError(bool bEnable)
	{
		if (bEnable)
		{
			IApollo.Instance.SetApolloLogger(ApolloLogPriority.Debug, null);
		}
		else
		{
			IApollo.Instance.SetApolloLogger(ApolloLogPriority.None, null);
		}
	}

	public bool IsLogin()
	{
		return this.GetAccountInfo(false) != null;
	}

	public void Login(ApolloPlatform platform, bool wait = true, ulong uin = 0uL, string pwd = null)
	{
		Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		if (wait)
		{
			if (this.loginTimerSeq > 0)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.loginTimerSeq);
			}
			this.loginTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(3000, 1, delegate(int sequence)
			{
				this.LoginHandler(platform, uin, pwd);
			});
		}
		else
		{
			this.LoginHandler(platform, uin, pwd);
		}
	}

	private void LoginHandler(ApolloPlatform platform, ulong uin = 0uL, string pwd = null)
	{
		switch (platform)
		{
		case ApolloPlatform.None:
		{
			if (ApolloConfig.CustomOpenId == null)
			{
			}
			ApolloResult apolloResult = NoneAccountService.Initialize(new NoneAccountInitInfo(ApolloConfig.CustomOpenId));
			if (apolloResult != ApolloResult.Success)
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", "NULL"));
				list.Add(new KeyValuePair<string, string>("totaltime", (Time.time - Singleton<CLoginSystem>.GetInstance().m_fLoginClickTime).ToString()));
				list.Add(new KeyValuePair<string, string>("errorCode", apolloResult.ToString()));
				list.Add(new KeyValuePair<string, string>("error_msg", "null"));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_MSDKClientAuth", list, true);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloResult>(EventID.ApolloHelper_Login_Failed, apolloResult);
				return;
			}
			break;
		}
		case ApolloPlatform.WTLogin:
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloResult>(EventID.ApolloHelper_Login_Failed, ApolloResult.Empty);
			return;
		}
		ApolloConfig.Uin = uin;
		ApolloConfig.Password = pwd;
		this.CurPlatform = platform;
		ApolloConfig.platform = platform;
		if (ApolloConfig.platform == ApolloPlatform.None && this.IsNoneModeSupport)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, this.GetAccountInfo(false));
		}
		else
		{
			this.OnLogin(platform);
		}
	}

	public bool Logout()
	{
		this.m_IsWXGameCenter = false;
		this.m_IsQQGameCenter = false;
		this.accountService.Logout();
		this.m_LastOpenID = null;
		return this.GetAccountInfo(false) == null;
	}

	public bool IsPlatformInstalled(ApolloPlatform platform)
	{
		return this.accountService.IsPlatformInstalled(platform);
	}

	public bool JudgeLoginAccountInfo(ref ApolloAccountInfo accountInfo)
	{
		return (accountInfo.Platform != ApolloPlatform.None || this.IsNoneModeSupport) && !string.IsNullOrEmpty(accountInfo.OpenId);
	}

	private void OnLogin(ApolloPlatform platform)
	{
		if (this.m_IsSwitchToLoginPlatform)
		{
			return;
		}
		Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		this.m_IsSwitchToLoginPlatform = true;
		if (!this.accountService.IsPlatformInstalled(platform))
		{
			if (this.CurPlatform == ApolloPlatform.Wechat)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloPlatform>(EventID.ApolloHelper_Platform_Not_Installed, ApolloPlatform.Wechat);
				return;
			}
			if (this.CurPlatform != ApolloPlatform.QQ && this.CurPlatform != ApolloPlatform.QRWechat)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloPlatform>(EventID.ApolloHelper_Platform_Not_Installed, platform);
				return;
			}
		}
		if (!this.m_IsLoginEventHandlerRegistered)
		{
			this.m_IsLoginEventHandlerRegistered = true;
			this.accountService.LoginEvent += new AccountLoginHandle(this.OnLoginEvent);
		}
		ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
		if (accountInfo != null)
		{
			this.m_LastOpenID = accountInfo.OpenId;
		}
		this.accountService.Login(platform);
	}

	private void OnLoginEvent(ApolloResult loginResult, ApolloAccountInfo accountInfo)
	{
		Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		Singleton<ApolloHelper>.GetInstance().IsLastLaunchFrom3rdAPP = false;
		Debug.Log(string.Concat(new object[]
		{
			"*Login*OnLoginEvent called ",
			loginResult,
			" ",
			Time.realtimeSinceStartup
		}));
		this.m_IsSwitchToLoginPlatform = false;
		if (loginResult == ApolloResult.Success)
		{
			ApolloConfig.platform = (this.CurPlatform = accountInfo.Platform);
			if (!string.IsNullOrEmpty(this.m_LastOpenID) && accountInfo.OpenId != this.m_LastOpenID)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Different_Account_Tip_Force"), enUIEventID.Login_Change_Account_Yes, false);
				return;
			}
			if (this.JudgeLoginAccountInfo(ref accountInfo))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, accountInfo);
			}
			else
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloResult>(EventID.ApolloHelper_Login_Failed, ApolloResult.Empty);
			}
		}
		else if (loginResult == ApolloResult.UserCancel)
		{
			BugLocateLogSys.Log(string.Format("Login Fail. User cancel", new object[0]));
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Login_Canceled);
		}
		else if (loginResult == ApolloResult.NeedRealNameAuth)
		{
			Debug.Log("Login NeedRealNameAuth.");
			BugLocateLogSys.Log(string.Format("Login NeedRealNameAuth.", new object[0]));
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Login_Need_Real_Name_Auth);
		}
		else
		{
			BugLocateLogSys.Log(string.Format("Login Fail. Error code is {0}", loginResult));
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", "NULL"));
			list.Add(new KeyValuePair<string, string>("totaltime", (Time.time - Singleton<CLoginSystem>.GetInstance().m_fLoginClickTime).ToString()));
			list.Add(new KeyValuePair<string, string>("errorCode", loginResult.ToString()));
			list.Add(new KeyValuePair<string, string>("error_msg", "null"));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_MSDKClientAuth", list, true);
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloResult>(EventID.ApolloHelper_Login_Failed, loginResult);
		}
		if (this.accountService == null)
		{
			BugLocateLogSys.Log("accountService == null");
		}
		this.m_IsLoginReturn = true;
		BugLocateLogSys.Log("LoginEvent Thread:" + Thread.CurrentThread.ManagedThreadId);
	}

	private void OnRefreshAccessTokenEvent(ApolloResult result, ListView<ApolloToken> tokenList)
	{
		Debug.Log(string.Concat(new object[]
		{
			"*Login*OnRefreshAccess called ",
			result,
			" ",
			Time.realtimeSinceStartup
		}));
		if (result == ApolloResult.Success)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null && this.JudgeLoginAccountInfo(ref accountInfo))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, accountInfo);
			}
		}
		else if (tokenList != null)
		{
			for (int i = 0; i < tokenList.Count; i++)
			{
			}
		}
	}

	public void RegisterQuickLoginHandler(ApolloQuickLoginNotify callback)
	{
		this.quickLoginService.SetQuickLoginNotify(callback);
	}

	public void SwitchUser(bool chg)
	{
		if (chg)
		{
			this.quickLoginService.SwitchUser(true);
		}
		else
		{
			this.quickLoginService.SwitchUser(false);
		}
	}

	public string GetRecordStr()
	{
		ApolloAccountInfo apolloAccountInfo = new ApolloAccountInfo();
		if (this.accountService == null)
		{
			BugLocateLogSys.Log("accountService == null");
			return "accountService == null";
		}
		ApolloResult record = this.accountService.GetRecord(ref apolloAccountInfo);
		if (record == ApolloResult.Success)
		{
			BugLocateLogSys.Log("GetRecord Success");
			return this.GetAccountInfoStr(ref apolloAccountInfo);
		}
		BugLocateLogSys.Log(string.Format("GetRecord result is {0}", record));
		return string.Empty;
	}

	public string GetAccountInfoStr(ref ApolloAccountInfo info)
	{
		string text = "===== Account Info =====\n";
		text += string.Format("OpenId:{0}\n", info.OpenId);
		text += string.Format("Pf:{0}\n", info.Pf);
		text += string.Format("PfKey:{0}\n", info.PfKey);
		text += string.Format("Platform:{0}\n", info.Platform);
		text += "TokenList Begin:\n";
		foreach (ApolloToken current in info.TokenList)
		{
			text += string.Format("{0}:{1}\n", current.Type, current.Value);
		}
		text += "TokenList End:\n";
		text += "===== Account Info =====";
		return text;
	}

	public string GetOpenID()
	{
		ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
		if (accountInfo != null)
		{
			return accountInfo.OpenId;
		}
		return string.Empty;
	}

	public ApolloAccountInfo GetAccountInfo(bool refreshToken = false)
	{
		ApolloAccountInfo apolloAccountInfo = new ApolloAccountInfo();
		if (ApolloConfig.platform == ApolloPlatform.None)
		{
			apolloAccountInfo.OpenId = ApolloConfig.CustomOpenId;
			apolloAccountInfo.Platform = ApolloPlatform.None;
			return apolloAccountInfo;
		}
		ApolloResult record = this.accountService.GetRecord(ref apolloAccountInfo);
		if (record == ApolloResult.Success)
		{
			if (this.CurPlatform == ApolloPlatform.None && this.CurPlatform != apolloAccountInfo.Platform)
			{
				ApolloConfig.platform = (this.CurPlatform = apolloAccountInfo.Platform);
			}
			return apolloAccountInfo;
		}
		if (record == ApolloResult.TokenInvalid && apolloAccountInfo != null && apolloAccountInfo.Platform == ApolloPlatform.Wechat && refreshToken)
		{
			this.accountService.RefreshAtkEvent -= new RefreshAccessTokenHandler(this.OnRefreshAccessTokenEvent);
			this.accountService.RefreshAtkEvent += new RefreshAccessTokenHandler(this.OnRefreshAccessTokenEvent);
			this.accountService.RefreshAccessToken();
			return null;
		}
		return null;
	}

	private void OnPaySuccess(ApolloBufferBase info)
	{
		ApolloPayResponseInfo apolloPayResponseInfo = info as ApolloPayResponseInfo;
		if (apolloPayResponseInfo.needRelogin != 0)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Need_Login);
			return;
		}
		string text = string.Format("status:{0},result code:{1},real num {2}, extendinfo ={3} ", new object[]
		{
			apolloPayResponseInfo.status,
			apolloPayResponseInfo.rstCode,
			apolloPayResponseInfo.realSaveNum,
			apolloPayResponseInfo.extendInfo
		});
		if (apolloPayResponseInfo.status == APO_PAY_STATUS.APO_PAYSTATE_PAYSUCC)
		{
			if (this.m_bPayQQVIP)
			{
				this.m_bPayQQVIP = false;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4200u);
				cSPkg.stPkgData.stQQVIPInfoReq.bReserved = 0;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Success);
		}
		else
		{
			Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_PayFail");
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Failed);
		}
		this.m_bPayQQVIP = false;
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.pay_type_result = apolloPayResponseInfo.rstCode.ToString();
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.callback_result = apolloPayResponseInfo.resultInerCode.ToString();
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.apollo_stage = apolloPayResponseInfo.needRelogin.ToString();
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.apollo_result = apolloPayResponseInfo.rstMsg;
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time - Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time;
		Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_quantity = apolloPayResponseInfo.realSaveNum.ToString();
		Singleton<BeaconHelper>.GetInstance().ReportBuyDianEvent();
	}

	public bool InitPay(ApolloAccountInfo accountInfo)
	{
		string payEnv = ApolloConfig.payEnv;
		if (accountInfo == null)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Need_Login);
			return false;
		}
		if (this.payService == null)
		{
			this.payService = (IApollo.Instance.GetService(2) as IApolloPayService);
			this.payService.PayEvent += new OnApolloPaySvrEvenHandle(this.OnPaySuccess);
		}
		this.registerInfo.environment = payEnv;
		this.registerInfo.enableLog = 1;
		if (this.payService.Initialize(this.registerInfo))
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Init_Success);
			return true;
		}
		Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Init_Failed);
		return false;
	}

	public bool Pay(string quantity, string productId = "")
	{
		if (!ApolloConfig.payEnabled)
		{
			return false;
		}
		ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
		if (!this.InitPay(accountInfo))
		{
			return false;
		}
		PayInfo payInfo = new PayInfo();
		string zoneId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
		payInfo.coinIcon = 2130837505;
		payInfo.offerId = ApolloConfig.offerID;
		payInfo.unit = "ge";
		payInfo.zoneId = zoneId;
		payInfo.valueChangeable = 0;
		payInfo.saveValue = quantity;
		if (!this.payService.Pay(payInfo))
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Failed);
			return false;
		}
		return true;
	}

	public bool PayQQVip(string serviceCode, string serviceName, int serviceType)
	{
		if (!ApolloConfig.payEnabled)
		{
			return false;
		}
		ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
		if (!this.InitPay(accountInfo))
		{
			return false;
		}
		this.m_bPayQQVIP = true;
		Pay4MonthInfo pay4MonthInfo = new Pay4MonthInfo();
		string zoneId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
		pay4MonthInfo.serviceCode = serviceCode;
		pay4MonthInfo.serviceName = serviceName;
		pay4MonthInfo.serviceType = (APO_PAY_MONTH_TYPE)serviceType;
		pay4MonthInfo.autoPay = 0;
		pay4MonthInfo.remark = "aid=mvip.youxi.inside.yxzj_1104466820";
		pay4MonthInfo.coinIcon = 2130837505;
		pay4MonthInfo.offerId = ApolloConfig.offerID;
		pay4MonthInfo.unit = "ge";
		pay4MonthInfo.zoneId = zoneId;
		pay4MonthInfo.valueChangeable = 0;
		pay4MonthInfo.saveValue = "1";
		if (!this.payService.Pay(pay4MonthInfo))
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Pay_Failed);
			return false;
		}
		return true;
	}

	public void ShowNotice(int Type, string scene)
	{
		if (CSysDynamicBlock.bLobbyEntryBlocked)
		{
			return;
		}
		if (Singleton<BattleLogic>.GetInstance().isRuning)
		{
			return;
		}
		this.GetNoticeData(Type, scene);
	}

	public void ShowIOSGuestNotice()
	{
		ApolloNoticeInfo apolloNoticeInfo = new ApolloNoticeInfo();
		ApolloNoticeData apolloNoticeData = new ApolloNoticeData();
		apolloNoticeData.MsgType = APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT;
		apolloNoticeData.ContentType = APOLLO_NOTICE_CONTENTTYPE.APO_SCONTENTTYPE_TEXT;
		apolloNoticeData.MsgTitle = "游客模式特别说明";
		apolloNoticeData.MsgContent = "敬爱的玩家，您正在使用游客模式进行游戏，游客模式下的游戏数据（包含付费数据）会在删除游戏、更换设备后清空。为了保障您的虚拟财产安全，以及让您获得更完善的游戏体验，我们建议您使用QQ/微信登录进行游戏！\n\n《王者荣耀》运营团队";
		apolloNoticeInfo.DataList.Add(apolloNoticeData);
		for (int i = 0; i < apolloNoticeInfo.DataList.Count; i++)
		{
			ApolloNoticeData apolloNoticeData2 = apolloNoticeInfo.DataList[i];
		}
		MonoSingleton<NoticeSys>.GetInstance().OnOpenForm(apolloNoticeInfo, NoticeSys.NOTICE_STATE.LOGIN_Before);
	}

	public List<string> GetNoticeUrl(int type, string scene)
	{
		List<string> list = new List<string>();
		if (Singleton<BattleLogic>.GetInstance().isRuning)
		{
			return list;
		}
		INotice notice = IApollo.Instance.GetService(5) as INotice;
		ApolloNoticeInfo apolloNoticeInfo = new ApolloNoticeInfo();
		notice.GetNoticeData((APOLLO_NOTICETYPE)type, scene, ref apolloNoticeInfo);
		for (int i = 0; i < apolloNoticeInfo.DataList.Count; i++)
		{
			ApolloNoticeData apolloNoticeData = apolloNoticeInfo.DataList[i];
			if (apolloNoticeData.ContentType == APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_WEB)
			{
				list.Add(apolloNoticeData.ContentUrl);
			}
		}
		return list;
	}

	public void GetNoticeData(int type, string scene)
	{
		try
		{
			INotice notice = IApollo.Instance.GetService(5) as INotice;
			ApolloNoticeInfo apolloNoticeInfo = new ApolloNoticeInfo();
			notice.GetNoticeData((APOLLO_NOTICETYPE)type, scene, ref apolloNoticeInfo);
			for (int i = 0; i < apolloNoticeInfo.DataList.Count; i++)
			{
				ApolloNoticeData apolloNoticeData = apolloNoticeInfo.DataList[i];
			}
			NoticeSys.NOTICE_STATE noticeState = NoticeSys.NOTICE_STATE.LOGIN_Before;
			if (scene == "1")
			{
				noticeState = NoticeSys.NOTICE_STATE.LOGIN_Before;
			}
			else if (scene == "2")
			{
				noticeState = NoticeSys.NOTICE_STATE.LOGIN_After;
			}
			MonoSingleton<NoticeSys>.GetInstance().OnOpenForm(apolloNoticeInfo, noticeState);
		}
		catch (Exception ex)
		{
			DebugHelper.Assert(false, "Error In GetNoticeData, {0}", new object[]
			{
				ex.Message
			});
		}
	}

	public void HideScrollNotice()
	{
		INotice notice = IApollo.Instance.GetService(5) as INotice;
		notice.HideNotice();
	}

	public void ApolloRepoertEvent(string eventName, List<KeyValuePair<string, string>> events, bool isReal)
	{
		try
		{
			IApolloReportService apolloReportService = IApollo.Instance.GetService(3) as IApolloReportService;
			apolloReportService.ApolloRepoertEvent(eventName, events, isReal);
		}
		catch (Exception var_1_20)
		{
		}
	}

	public string ToSnsHeadUrl(string url)
	{
		if (url != null && url.StartsWith("http://i.gtimg.cn"))
		{
			return url;
		}
		if (url != null && url.StartsWith("http://wx.qlogo.cn/"))
		{
			return string.Format("{0}/{1}", url, "96");
		}
		if (url != null && url.StartsWith("http://q.qlogo.cn/"))
		{
			return string.Format("{0}/{1}", url, "100");
		}
		switch (Singleton<ApolloHelper>.GetInstance().CurPlatform)
		{
		case ApolloPlatform.Wechat:
			return string.Format("{0}/{1}", url, "96");
		case ApolloPlatform.QQ:
		case ApolloPlatform.WTLogin:
			return string.Format("{0}/{1}", url, "100");
		default:
			return url;
		}
	}

	public string ToSnsHeadUrl(ref byte[] szUrl)
	{
		string text = StringHelper.UTF8BytesToString(ref szUrl);
		if (text != null && text.StartsWith("http://wx.qlogo.cn/"))
		{
			return string.Format("{0}/{1}", text, "96");
		}
		if (text != null && text.StartsWith("http://q.qlogo.cn/"))
		{
			return string.Format("{0}/{1}", text, "100");
		}
		switch (Singleton<ApolloHelper>.GetInstance().CurPlatform)
		{
		case ApolloPlatform.Wechat:
			return string.Format("{0}/{1}", text, "96");
		case ApolloPlatform.QQ:
		case ApolloPlatform.WTLogin:
			return string.Format("{0}/{1}", text, "100");
		default:
			return StringHelper.UTF8BytesToString(ref szUrl);
		}
	}

	public bool GetMySnsInfo(OnRelationNotifyHandle handler)
	{
		if (this.GetAccountInfo(false) == null)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ApolloHelper_Need_Login);
			return false;
		}
		if (this.snsService == null)
		{
			this.snsService = (IApollo.Instance.GetService(1) as IApolloSnsService);
			if (this.snsService == null)
			{
				return false;
			}
		}
		this.snsService.onRelationEvent -= handler;
		this.snsService.onRelationEvent += handler;
		switch (Singleton<ApolloHelper>.GetInstance().CurPlatform)
		{
		case ApolloPlatform.Wechat:
			return this.snsService.QueryMyInfo(ApolloPlatform.Wechat);
		case ApolloPlatform.QQ:
		case ApolloPlatform.WTLogin:
			return this.snsService.QueryMyInfo(ApolloPlatform.QQ);
		default:
			return false;
		}
	}

	public void ShareSendHeart(string openId, string title, string desc, string extInfo)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		MonoSingleton<ShareSys>.instance.OnShareCallBack();
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string targetUrl = "http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=" + accountInfo.OpenId + "&ADTAG=gameobj.msg_heart";
				string imgUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
				apolloSnsService.SendToQQGameFriend(1, openId, title, desc, targetUrl, imgUrl, title, "MSG_HEART_SEND", extInfo);
			}
		}
		else
		{
			apolloSnsService.SendToWXGameFriend(openId, title, desc, "9Wste6_dDgZtoVmC6CQTh0jj29kGEp0jrVSYrGWvtZLvSTDN9fUb-_sNjacaGITt", "messageExt", "MSG_heart_send", extInfo);
		}
	}

	public void ShareSendHeartPandroa(int actID, string openId, string title, string desc, string pic_url, string previewText, string gameTag, string extInfo, string mediaId, string messageExt)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		MonoSingleton<ShareSys>.instance.OnShareCallBack();
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string targetUrl = "http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=" + accountInfo.OpenId + "&ADTAG=gameobj.msg_heart";
				apolloSnsService.SendToQQGameFriend(actID, openId, title, desc, targetUrl, pic_url, previewText, gameTag, extInfo);
			}
		}
		else
		{
			apolloSnsService.SendToWXGameFriend(openId, title, desc, mediaId, messageExt, gameTag, extInfo);
		}
	}

	public void ShareInviteFriend(string openId, string title, string desc, string extInfo)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		Texture2D texture2D = Resources.Load("Share/120", typeof(Texture2D)) as Texture2D;
		if (texture2D == null)
		{
			DebugHelper.Assert(false, "Texture2D  Share/120 == null");
			return;
		}
		byte[] array = texture2D.EncodeToPNG();
		if (array != null)
		{
			int num = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (apolloSnsService == null)
		{
			DebugHelper.Assert(false, "IApollo.Instance.GetService(ApolloServiceType.Sns) == null");
			return;
		}
		MonoSingleton<ShareSys>.instance.OnShareCallBack();
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string targetUrl = "http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=" + accountInfo.OpenId + "&ADTAG=gameobj.msg_invite";
				string imgUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
				apolloSnsService.SendToQQGameFriend(1, openId, title, desc, targetUrl, imgUrl, title, "MSG_INVITE", extInfo);
			}
		}
		else
		{
			apolloSnsService.SendToWXGameFriend(openId, title, desc, "9Wste6_dDgZtoVmC6CQTh0jj29kGEp0jrVSYrGWvtZLvSTDN9fUb-_sNjacaGITt", "messageExt", "MSG_INVITE", extInfo);
		}
	}

	public void ShareToFriend(string title, string desc)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		Texture2D texture2D = Resources.Load("Share/120", typeof(Texture2D)) as Texture2D;
		if (texture2D == null)
		{
			return;
		}
		byte[] array = texture2D.EncodeToPNG();
		int thumbDataLen = 0;
		if (array != null)
		{
			thumbDataLen = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (apolloSnsService == null)
		{
			return;
		}
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string url = "http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=" + accountInfo.OpenId + "&ADTAG=gameobj.msg_invite";
				string thumbImageUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
				apolloSnsService.SendToQQ(ApolloShareScene.QSession, title, desc, url, thumbImageUrl);
			}
		}
		else
		{
			apolloSnsService.SendToWeixin(title, desc, "MSG_INVITE", array, thumbDataLen, "SendToWeixin_extInfo");
		}
	}

	public void EnableBugly()
	{
		if (this.reportService == null)
		{
			return;
		}
		this.reportService.EnableExceptionHandler(LogSeverity.LogException);
		this.reportService.ApolloReportInit(true, true);
	}

	public void ReportCatchExcption(Exception e)
	{
		IApolloReportService apolloReportService = IApollo.Instance.GetService(3) as IApolloReportService;
		apolloReportService.HandleException(e);
	}

	public void CustomLog(string str)
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
			androidJavaClass.CallStatic("dtLog", new object[]
			{
				DateTime.Now.ToString("yyyyMMdd_HHmmss ") + str
			});
			androidJavaClass.Dispose();
		}
		catch (Exception)
		{
		}
	}

	public int GetChannelID()
	{
		IApolloCommonService apolloCommonService = IApollo.Instance.GetService(8) as IApolloCommonService;
		if (apolloCommonService == null)
		{
			return 0;
		}
		string channelId = apolloCommonService.GetChannelId();
		int result = 0;
		bool flag = int.TryParse(channelId, out result);
		if (flag)
		{
			return result;
		}
		return 0;
	}

	public int GetRegisterChannelId()
	{
		IApolloCommonService apolloCommonService = IApollo.Instance.GetService(8) as IApolloCommonService;
		if (apolloCommonService == null)
		{
			return 0;
		}
		string registerChannelId = apolloCommonService.GetRegisterChannelId();
		int result = 0;
		bool flag = int.TryParse(registerChannelId, out result);
		if (flag)
		{
			return result;
		}
		return 0;
	}

	public COM_PRIVILEGE_TYPE GetCurrentLoginPrivilege()
	{
		COM_PRIVILEGE_TYPE result = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
		if (this.CurPlatform == ApolloPlatform.Wechat && this.m_IsWXGameCenter)
		{
			result = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN;
		}
		else if (this.CurPlatform == ApolloPlatform.QQ && this.m_IsQQGameCenter)
		{
			result = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN;
		}
		else if (this.CurPlatform == ApolloPlatform.Guest)
		{
			result = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_IOSVISITOR_LOGIN;
		}
		return result;
	}

	public void OpenWeiXinDeeplink(int linkType, string url)
	{
		IApolloCommonService apolloCommonService = IApollo.Instance.GetService(8) as IApolloCommonService;
		if (apolloCommonService != null)
		{
			string link = "INDEX";
			if (linkType == 0)
			{
				link = "INDEX";
			}
			else if (linkType == 1)
			{
				link = "DETAIL";
			}
			else if (linkType == 2)
			{
				link = "LIBRARY";
			}
			else if (linkType == 3)
			{
				link = url;
			}
			apolloCommonService.OpenWeiXinDeeplink(link);
		}
	}

	public void ShareQQBox(string actID, string boxID, string title, string desc)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		Texture2D texture2D = Resources.Load("Share/120", typeof(Texture2D)) as Texture2D;
		byte[] array = texture2D.EncodeToPNG();
		if (array != null)
		{
			int num = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			this.m_bShareQQBox = true;
			string url = string.Format("http://gamecenter.qq.com/giftbox/release/index/grap.html?actid={0}&_wv=1031&boxid={1}&appid={2}", actID, boxID, ApolloConfig.appID);
			string thumbImageUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
			apolloSnsService.SendToQQ(ApolloShareScene.QSession, title, desc, url, thumbImageUrl);
		}
	}

	public void ShareRedBox(int shareType, string url, string title, string desc, Texture2D tex, string imgurl)
	{
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		byte[] array = tex.EncodeToPNG();
		int imageDataLen = 0;
		if (array != null)
		{
			imageDataLen = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			apolloSnsService.SendToQQ((ApolloShareScene)shareType, title, desc, url, imgurl);
		}
		else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
		{
			apolloSnsService.SendToWeixinWithUrl((ApolloShareScene)shareType, title, desc, url, "MSG_INVITE", array, imageDataLen, "message Ext");
		}
	}

	public string GetAppId()
	{
		return (Singleton<ApolloHelper>.GetInstance().CurPlatform != ApolloPlatform.Wechat) ? ApolloConfig.QQAppID : ApolloConfig.WXAppID;
	}

	public string GetAppKey()
	{
		return (Singleton<ApolloHelper>.GetInstance().CurPlatform != ApolloPlatform.Wechat) ? ApolloConfig.QQAppKey : ApolloConfig.WXAppKey;
	}

	public string GetAccessToken(ApolloPlatform platform)
	{
		string result = string.Empty;
		ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
		if (accountInfo != null)
		{
			for (int i = 0; i < accountInfo.TokenList.Count; i++)
			{
				ApolloToken apolloToken = accountInfo.TokenList[i];
				if (platform == ApolloPlatform.Wechat)
				{
					if (apolloToken != null && apolloToken.Type == ApolloTokenType.Access)
					{
						result = apolloToken.Value;
					}
				}
				else if ((platform == ApolloPlatform.QQ || platform == ApolloPlatform.Guest) && apolloToken != null && apolloToken.Type == ApolloTokenType.Access)
				{
					result = apolloToken.Value;
				}
			}
		}
		return result;
	}

	public void InviteFriendToRoom(string title, string desc, string roomInfo)
	{
		Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		Texture2D texture2D = Resources.Load("Share/120", typeof(Texture2D)) as Texture2D;
		byte[] array = null;
		if (texture2D)
		{
			array = texture2D.EncodeToPNG();
		}
		int thumbDataLen = 0;
		if (array != null)
		{
			thumbDataLen = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (apolloSnsService == null)
		{
			return;
		}
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string url = string.Concat(new string[]
				{
					"http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=",
					accountInfo.OpenId,
					"&ADTAG=gameobj.msg_invite&",
					ApolloHelper.QQ_SHARE_GAMEDATA,
					"=",
					roomInfo
				});
				string thumbImageUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
				apolloSnsService.SendToQQ(ApolloShareScene.QSession, title, desc, url, thumbImageUrl);
			}
		}
		else
		{
			apolloSnsService.SendToWeixin(title, desc, "MSG_INVITE", array, thumbDataLen, roomInfo);
		}
	}

	public void ShareRecruitFriend(string title, string desc, string roomInfo)
	{
		if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
		{
			return;
		}
		Texture2D texture2D = Resources.Load("Share/120", typeof(Texture2D)) as Texture2D;
		byte[] array = null;
		if (texture2D)
		{
			array = texture2D.EncodeToPNG();
		}
		int imageDataLen = 0;
		if (array != null)
		{
			imageDataLen = array.Length;
		}
		IApolloSnsService apolloSnsService = IApollo.Instance.GetService(1) as IApolloSnsService;
		if (apolloSnsService == null)
		{
			return;
		}
		if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
		{
			ApolloAccountInfo accountInfo = this.GetAccountInfo(false);
			if (accountInfo != null)
			{
				string url = "http://youxi.vip.qq.com/m/act/b1068bb755_sgame_104290.html?_wv=1&QQ_SHARE_GAMEDATA=" + roomInfo;
				string thumbImageUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
				apolloSnsService.SendToQQ(ApolloShareScene.QSession, title, desc, url, thumbImageUrl);
			}
		}
		else
		{
			ApolloAccountInfo accountInfo2 = this.GetAccountInfo(false);
			if (accountInfo2 != null)
			{
				string url2 = string.Format("http://game.weixin.qq.com/cgi-bin/act?actid=4381&messageExt={0}&k=bZML6lvwyQRcCItiE6oDJg&q=0&jsapi_ticket=1#wechat_redirect", roomInfo);
				apolloSnsService.SendToWeixinWithUrl(ApolloShareScene.Session, title, desc, url2, "MSG_INVITE", array, imageDataLen, roomInfo);
			}
		}
	}

	public string GetPlatformStr()
	{
		if (this.CurPlatform == ApolloPlatform.Wechat)
		{
			return ApolloPlatform.Wechat.ToString();
		}
		return this.CurPlatform.ToString();
	}
}

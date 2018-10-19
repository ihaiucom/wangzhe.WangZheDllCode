using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

public class NetworkAccelerator
{
	public enum UserStatus
	{
		NotQualified,
		Qualified,
		FreeTrial,
		TrialExpired,
		InUse,
		Expired
	}

	public enum PopTipsType
	{
		Trial,
		Buy
	}


	public enum XunYouMode
	{
		Disable,
		Gray,
		Official,
		Free
	}

	public enum AccelRecommendation_State
	{
		ACCEL_RECOMMENDATION_NONE,
		ACCEL_RECOMMENDATION_NOTICE,
		ACCEL_RECOMMENDATION_WIFI
	}

	private static bool s_inited;

	private static bool s_started;

	private static string key = "827006BE-64F7-4082-B252-33ACF328A3A5";

	private static int KEY_GET_NETDELAY = 100;

	private static int KEY_GET_ACCEL_STAT = 102;

	private static int KEY_GET_ACCEL_EFFECT = 107;

	private static int KEY_LOG_LEVLE = 308;

	public static string PLAYER_PREF_NET_ACC = "NET_ACC";

	public static string PLAYER_PREF_AUTO_NET_ACC = "AUTO_NET_ACC";

	public static int LOG_LEVEL_DEBUG = 1;

	public static int LOG_LEVEL_INFO = 2;

	public static int LOG_LEVEL_WARNING = 3;

	public static int LOG_LEVEL_ERROR = 4;

	public static int LOG_LEVEL_FATAL = 5;

	private static NetworkAccelerator.XunYouMode s_mode;

	private static ushort m_Vport;

	private static string m_IP = string.Empty;

	private static bool m_bLog = true;

	public static bool started
	{
		get
		{
			return NetworkAccelerator.s_started;
		}
	}

	public static bool Inited
	{
		get
		{
			return NetworkAccelerator.s_inited;
		}
	}

	public static ushort Vport
	{
		get
		{
			return NetworkAccelerator.m_Vport;
		}
		set
		{
			NetworkAccelerator.m_Vport = value;
		}
	}

	public static string ConnectIP
	{
		get
		{
			return NetworkAccelerator.m_IP;
		}
		set
		{
			NetworkAccelerator.m_IP = value;
		}
	}

	public static NetworkAccelerator.XunYouMode Mode
	{
		get
		{
			return NetworkAccelerator.s_mode;
		}
	}

	public static void SetConnectIP(string ip, ushort port)
	{
		NetworkAccelerator.ConnectIP = ip;
		NetworkAccelerator.Vport = port;
	}

	public static void ClearConnectIP()
	{
		NetworkAccelerator.ConnectIP = string.Empty;
		NetworkAccelerator.Vport = 0;
	}

	public static string GetConnectIPstr()
	{
		return string.Format("{0}_{1}", NetworkAccelerator.ConnectIP, NetworkAccelerator.Vport);
	}

	private static void PrintLog(string log)
	{
		if (NetworkAccelerator.m_bLog)
		{
			Debug.Log("[NetSpeed ACC] " + log);
		}
	}

	public static void InitACC(NetworkAccelerator.XunYouMode mode = NetworkAccelerator.XunYouMode.Disable)
	{
		NetworkAccelerator.PrintLog("Begin Network Acc");
		NetworkAccelerator.setSDKMode(mode);
		if (mode == NetworkAccelerator.XunYouMode.Disable)
		{
			NetworkAccelerator.PrintLog("mode " + mode);
			return;
		}
		if (NetworkAccelerator.s_inited)
		{
			NetworkAccelerator.PrintLog("already init");
			NetworkAccelerator.Stop();
			return;
		}
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_TurnOn, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnOn));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_Ignore, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnIgore));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_WIFI_TurnOn, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnOnWIFI));
		NetworkAccelerator.PrintLog(string.Concat(new object[]
		{
			"key:",
			NetworkAccelerator.key,
			" mode ",
			mode
		}));
		try
		{
			AndroidJavaObject GMContext = null;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				GMContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			AndroidJavaClass GMClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (GMClass != null)
			{
				GMContext.Call("runOnUiThread", new object[]
				{
					new AndroidJavaRunnable(delegate
					{
						int num = GMClass.CallStatic<int>("init", new object[]
						{
							GMContext,
							1,
							NetworkAccelerator.key,
							"KingsGlory",
							"libapollo.so",
							13001
						});
						if (num >= 0)
						{
							NetworkAccelerator.PrintLog("Initialize GameMaster Success!");
							NetworkAccelerator.s_inited = true;
							NetworkAccelerator.setSDKMode(mode);
							NetworkAccelerator.SetUserToken();
							if (MonoSingleton<CTongCaiSys>.GetInstance().IsTongCaiUserAndCanUse())
							{
								int freeFlowUser = -1;
								if (MonoSingleton<CTongCaiSys>.GetInstance().supplierType == IspType.Dianxing)
								{
									freeFlowUser = 2;
								}
								else if (MonoSingleton<CTongCaiSys>.GetInstance().supplierType == IspType.Liantong)
								{
									freeFlowUser = 1;
								}
								else if (MonoSingleton<CTongCaiSys>.GetInstance().supplierType == IspType.Yidong)
								{
									freeFlowUser = 0;
								}
								NetworkAccelerator.setFreeFlowUser(freeFlowUser);
							}
							NetworkAccelerator.setGameId(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID);
							NetworkAccelerator.ChangeLogLevel(NetworkAccelerator.LOG_LEVEL_ERROR);
						}
						else
						{
							NetworkAccelerator.PrintLog("Initialize GameMaster Fail!, ret:" + num);
						}
					})
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("catch ex " + ex.ToString());
		}
	}

	public static bool IsNetAccConfigOpen()
	{
		return PlayerPrefs.GetInt(NetworkAccelerator.PLAYER_PREF_NET_ACC, 0) > 0;
	}

	public static bool IsAutoNetAccConfigOpen()
	{
		return PlayerPrefs.GetInt(NetworkAccelerator.PLAYER_PREF_AUTO_NET_ACC, 0) > 0;
	}

	public static void SetNetAccConfig(bool open)
	{
		if (open)
		{
			NetworkAccelerator.Start();
		}
		else
		{
			NetworkAccelerator.Stop();
		}
		PlayerPrefs.SetInt(NetworkAccelerator.PLAYER_PREF_NET_ACC, (!open) ? 0 : 1);
		PlayerPrefs.Save();
		MonoSingleton<GSDKsys>.GetInstance().StartGSDKSpeed(!open);
		NetworkAccelerator.PrintLog("SetNetAccConfig " + open);
	}

	public static void SetAutoNetAccConfig(bool open)
	{
		PlayerPrefs.SetInt(NetworkAccelerator.PLAYER_PREF_AUTO_NET_ACC, (!open) ? 0 : 1);
		PlayerPrefs.Save();
	}

	private static void OnEventTurnOn(CUIEvent uiEvent)
	{
		NetworkAccelerator.SetNetAccConfig(true);
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetACCturnOK", null, true);
	}

	private static void OnEventTurnIgore(CUIEvent uiEvent)
	{
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetACCIgore", null, true);
	}

	private static void OnEventTurnOnWIFI(CUIEvent uiEvent)
	{
		NetworkAccelerator.setWiFiAccelSwitch(true);
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetACCturnOKWIFI", null, true);
	}

	public static void setSDKMode(NetworkAccelerator.XunYouMode mode)
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setSDKMode", new object[]
				{
					(int)mode
				});
				NetworkAccelerator.s_mode = mode;
				NetworkAccelerator.PrintLog("setSDKMode " + NetworkAccelerator.s_mode);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static void SetUserToken()
	{
		ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
		if (accountInfo != null)
		{
			ApolloToken token = accountInfo.GetToken(ApolloTokenType.Access);
			if (token != null)
			{
				NetworkAccelerator.setUserToken(accountInfo.OpenId, token.Value);
			}
		}
	}

	private static void setUserToken(string openid, string token)
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setUserToken", new object[]
				{
					openid,
					token,
					ApolloConfig.GetAppID()
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	private static void setWiFiAccelSwitch(bool on)
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setWiFiAccelSwitch", new object[]
				{
					on
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static bool isAccelOpened()
	{
		bool result = false;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				result = androidJavaClass.CallStatic<bool>("isAccelOpened", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return result;
	}

	public static string getWebUIUrl()
	{
		string text = string.Empty;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				text = androidJavaClass.CallStatic<string>("getWebUIUrl", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		NetworkAccelerator.PrintLog("getWebUIUrl " + text);
		return text;
	}

	public static string getVIPValidTime()
	{
		string text = string.Empty;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				text = androidJavaClass.CallStatic<string>("getVIPValidTime", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		NetworkAccelerator.PrintLog("getVIPValidTime " + text);
		return text;
	}

	public static NetworkAccelerator.UserStatus GetUserStatus()
	{
		int num = 0;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				num = androidJavaClass.CallStatic<int>("getAccelerationStatus", new object[0]);
			}
			NetworkAccelerator.PrintLog("GetUserStatus " + (NetworkAccelerator.UserStatus)num);
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return (NetworkAccelerator.UserStatus)num;
	}

	public static void ChangeLogLevel(int level)
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		long num = (long)Mathf.Clamp(level, 1, 5);
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setLong", new object[]
				{
					NetworkAccelerator.KEY_LOG_LEVLE,
					num
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	private static bool Start()
	{
		if (!NetworkAccelerator.s_inited)
		{
			return NetworkAccelerator.s_started;
		}
		if (NetworkAccelerator.s_started)
		{
			return NetworkAccelerator.s_started;
		}
		bool flag = false;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				flag = androidJavaClass.CallStatic<bool>("start", new object[]
				{
					0
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		if (flag)
		{
			NetworkAccelerator.PrintLog("Start GameMaster Success!");
			NetworkAccelerator.s_started = true;
		}
		else
		{
			NetworkAccelerator.PrintLog("Start GameMaster Fail!");
			NetworkAccelerator.s_started = false;
		}
		return NetworkAccelerator.s_started;
	}

	private static bool Stop()
	{
		if (!NetworkAccelerator.s_inited)
		{
			return NetworkAccelerator.s_started;
		}
		if (!NetworkAccelerator.s_started)
		{
			return NetworkAccelerator.s_started;
		}
		bool flag = false;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				flag = androidJavaClass.CallStatic<bool>("stop", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		if (flag)
		{
			NetworkAccelerator.PrintLog("Stop GameMaster Success!");
			NetworkAccelerator.ClearUDPCache();
			NetworkAccelerator.s_started = false;
		}
		else
		{
			NetworkAccelerator.PrintLog("Stop GameMaster Fail!");
		}
		return NetworkAccelerator.s_started;
	}

	public static void SetEchoPort(int port)
	{
		NetworkAccelerator.PrintLog("Set UD Echo Port to :" + port);
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setUdpEchoPort", new object[]
				{
					port
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		NetworkAccelerator.PrintLog("Set UD Echo Port Success!");
	}

	public static void setRecommendationGameIP(string ip, int port)
	{
		NetworkAccelerator.PrintLog(string.Concat(new object[]
		{
			"setRecommendationGameIP :",
			ip,
			", port :",
			port
		}));
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setRecommendationGameIP", new object[]
				{
					ip,
					port
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		NetworkAccelerator.PrintLog("Set setRecommendationGameIP Success!");
	}

	public static void OnNetDelay(int millis)
	{
		if (!NetworkAccelerator.s_inited || NetworkAccelerator.started)
		{
			return;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("onNetDelay", new object[]
				{
					millis
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static NetworkAccelerator.AccelRecommendation_State getAccelRecommendation()
	{
		NetworkAccelerator.AccelRecommendation_State accelRecommendation_State = NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NONE;
		if (!NetworkAccelerator.s_inited || NetworkAccelerator.started)
		{
			return accelRecommendation_State;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				accelRecommendation_State = (NetworkAccelerator.AccelRecommendation_State)androidJavaClass.CallStatic<int>("getAccelRecommendation", new object[0]);
				NetworkAccelerator.PrintLog("getAccelRecommendation :" + accelRecommendation_State);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return accelRecommendation_State;
	}

	public static void ClearUDPCache()
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("clearUDPCache", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static long GetDelay()
	{
		if (!NetworkAccelerator.s_started)
		{
			return -1L;
		}
		long result = -1L;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				result = androidJavaClass.CallStatic<long>("getLong", new object[]
				{
					NetworkAccelerator.KEY_GET_NETDELAY
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return result;
	}

	public static string GetEffect()
	{
		if (!NetworkAccelerator.s_started)
		{
			return null;
		}
		string result = null;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				result = androidJavaClass.CallStatic<string>("getString", new object[]
				{
					NetworkAccelerator.KEY_GET_ACCEL_EFFECT
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return result;
	}

	public static int GetNetType()
	{
		int result = -1;
		if (!NetworkAccelerator.s_inited)
		{
			return result;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				result = androidJavaClass.CallStatic<int>("getCurrentConnectionType", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return result;
	}

	public static bool isAccerating()
	{
		if (!NetworkAccelerator.s_started)
		{
			return false;
		}
		bool result = false;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				result = androidJavaClass.CallStatic<bool>("isUDPProxy", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
		return result;
	}

	public static void GoFront()
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		NetworkAccelerator.PrintLog("Begin GoFront");
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("gameForeground", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static void GoBack()
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		NetworkAccelerator.PrintLog("Begin GoBack");
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("gameBackground", new object[0]);
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	private static void setFreeFlowUser(int isFreeFlowUser)
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		NetworkAccelerator.PrintLog("BesetFreeFlowUser " + isFreeFlowUser);
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setFreeFlowUser", new object[]
				{
					isFreeFlowUser
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	private static void setGameId(int worldID)
	{
		if (!NetworkAccelerator.s_inited)
		{
			return;
		}
		NetworkAccelerator.PrintLog("worldID " + worldID);
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("setGameId", new object[]
				{
					worldID
				});
			}
		}
		catch (Exception ex)
		{
			NetworkAccelerator.PrintLog("ex " + ex.ToString());
		}
	}

	public static bool IsCommercialized()
	{
		return NetworkAccelerator.s_mode == NetworkAccelerator.XunYouMode.Gray || NetworkAccelerator.s_mode == NetworkAccelerator.XunYouMode.Official;
	}

	public static void TryToSendExpireTime()
	{
		if (NetworkAccelerator.s_mode == NetworkAccelerator.XunYouMode.Official)
		{
			NetworkAccelerator.UserStatus userStatus = NetworkAccelerator.GetUserStatus();
			if (userStatus == NetworkAccelerator.UserStatus.InUse || userStatus == NetworkAccelerator.UserStatus.Expired)
			{
				string vIPValidTime = NetworkAccelerator.getVIPValidTime();
				uint dwServEndTime = Utility.ToUtcSeconds(Utility.StrToDateTime(vIPValidTime, "yyyy-MM-dd HH:mm:ss"));
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1480u);
				CSPKG_CHKXUNYOUSERV_REQ cSPKG_CHKXUNYOUSERV_REQ = new CSPKG_CHKXUNYOUSERV_REQ();
				cSPKG_CHKXUNYOUSERV_REQ.dwServEndTime = dwServEndTime;
				cSPkg.stPkgData.stChkXunyouServ = cSPKG_CHKXUNYOUSERV_REQ;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}
	}

	public static void TryToOpenTips()
	{
		bool flag = false;
		NetworkAccelerator.AccelRecommendation_State accelRecommendation = NetworkAccelerator.getAccelRecommendation();
		if (accelRecommendation >= NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
		{
			flag = true;
		}
		if (flag)
		{
			if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
			{
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetAccelRecommendationWIFI", null, true);
			}
			else if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
			{
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetAccelRecommendation", null, true);
			}
		}
		if (NetworkAccelerator.IsCommercialized())
		{
			int num = 0;
			int num2 = 0;
			if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
			{
				num = PlayerPrefs.GetInt("NET_ACC_RECOMMENDED_V2", 0);
				num2 = PlayerPrefs.GetInt("NET_ACC_RECOMMENDED_FIRST_SETTLEMENT", 0);
			}
			else if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
			{
				num = PlayerPrefs.GetInt("NET_ACC_RECOMMENDED_V2_WIFI", 0);
				num2 = PlayerPrefs.GetInt("NET_ACC_RECOMMENDED_FIRST_SETTLEMENT_WIFI", 0);
			}
			int srv2CltGlobalValue = (int)GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_XUNYOU_TIPS_POP_CYCLE);
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			NetworkAccelerator.UserStatus userStatus = NetworkAccelerator.GetUserStatus();
			if (userStatus == NetworkAccelerator.UserStatus.FreeTrial)
			{
				string vIPValidTime = NetworkAccelerator.getVIPValidTime();
				int num3 = (int)Utility.ToUtcSeconds(Utility.StrToDateTime(vIPValidTime, "yyyy-MM-dd HH:mm:ss"));
				int num4 = num3 - currentUTCTime;
				if (num4 >= 0 && num4 < 86400 && currentUTCTime - num2 >= 86400)
				{
					NetworkAccelerator.PopTips(NetworkAccelerator.PopTipsType.Buy, accelRecommendation);
					if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
					{
						PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_FIRST_SETTLEMENT_WIFI", currentUTCTime);
					}
					else
					{
						PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_FIRST_SETTLEMENT", currentUTCTime);
					}
				}
			}
			else if (userStatus == NetworkAccelerator.UserStatus.Qualified)
			{
				if (currentUTCTime - srv2CltGlobalValue * 86400 >= num && flag)
				{
					NetworkAccelerator.PopTips(NetworkAccelerator.PopTipsType.Trial, accelRecommendation);
					if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
					{
						PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_V2", currentUTCTime);
					}
					else if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
					{
						PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_V2_WIFI", currentUTCTime);
					}
				}
			}
			else if ((userStatus == NetworkAccelerator.UserStatus.Expired || userStatus == NetworkAccelerator.UserStatus.TrialExpired) && currentUTCTime - srv2CltGlobalValue * 86400 >= num && flag)
			{
				NetworkAccelerator.PopTips(NetworkAccelerator.PopTipsType.Buy, accelRecommendation);
				if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
				{
					PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_V2", currentUTCTime);
				}
				else if (accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
				{
					PlayerPrefs.SetInt("NET_ACC_RECOMMENDED_V2_WIFI", currentUTCTime);
				}
			}
		}
		else
		{
			if (!PlayerPrefs.HasKey("NET_ACC_RECOMMENDED") && flag && accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_NOTICE)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX"), enUIEventID.NetworkAccelerator_TurnOn, enUIEventID.NetworkAccelerator_Ignore, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_CANCEL"), false);
				PlayerPrefs.SetString("NET_ACC_RECOMMENDED", "Y");
			}
			if (!PlayerPrefs.HasKey("NET_ACC_RECOMMENDED_WIFI") && flag && accelRecommendation == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_WIFI_MSGBOX"), enUIEventID.NetworkAccelerator_WIFI_TurnOn, enUIEventID.NetworkAccelerator_Ignore, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_CANCEL"), false);
				PlayerPrefs.SetString("NET_ACC_RECOMMENDED_WIFI", "Y");
			}
		}
	}

	public static void PopTips(NetworkAccelerator.PopTipsType type, NetworkAccelerator.AccelRecommendation_State curState)
	{
		if (type != NetworkAccelerator.PopTipsType.Trial)
		{
			if (type != NetworkAccelerator.PopTipsType.Buy)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX"), enUIEventID.Partner_OpenXunYou_Buy, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX_CANCEL"), false);
			}
			else if (curState == NetworkAccelerator.AccelRecommendation_State.ACCEL_RECOMMENDATION_WIFI)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_WIFI_MSGBOX"), enUIEventID.NetworkAccelerator_WIFI_TurnOn, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_BUY_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_BUY_MSGBOX_CANCEL"), false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_BUY_MSGBOX"), enUIEventID.Partner_OpenXunYou_Buy, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_BUY_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_BUY_MSGBOX_CANCEL"), false);
			}
		}
		else
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX"), enUIEventID.Partner_OpenXunYou_Buy, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_TRIAL_MSGBOX_CANCEL"), false);
		}
	}
}

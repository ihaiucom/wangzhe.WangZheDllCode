using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using com.tencent.gsdk;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GSDKsys : MonoSingleton<GSDKsys>
{

	public enum GSDKConfigMode
	{
		diable,
		mna,
		speed,
		ALL
	}

	public string m_LastQueryStr = string.Empty;

	public GSDKsys.GSDKConfigMode m_GSDKConfig;

	public NetworkAccelerator.XunYouMode m_NetACCConfig;

	private bool m_bInit;

	private string m_LastIP = string.Empty;

	private int m_LastPort;

	private int m_bUseSpeed = 1;

	public int m_GsdkSpeedFlag = -1000;

	private static bool m_bLog = true;

	private KartinRet m_lastQuery = new KartinRet();

	private bool m_bStartKartin;

	private bool m_bDealyUpdate;

	public static string Signal_Value_ImgGreen = "UGUI/Sprite/System/Setting/SignalValue.prefab";

	public static string Signal_Value_ImgBg = "UGUI/Sprite/System/Setting/SignalValueBg.prefab";

	public static string Signal_Value_ImgRed = "UGUI/Sprite/System/Setting/SignalValueRed.prefab";

	public static string Signal_Value_ImgYellow = "UGUI/Sprite/System/Setting/SignalValueYellow.prefab";

	public bool Inited
	{
		get
		{
			return this.m_bInit;
		}
	}

	public bool UseGSdkSpeed
	{
		get
		{
			return this.m_bUseSpeed == 0;
		}
	}

	private static void PrintLog(string log)
	{
		if (GSDKsys.m_bLog)
		{
			Debug.Log("[NetSpeed GSDK] " + log);
		}
	}

	public bool IsMnaUseInBattleSetting()
	{
		return GameDataMgr.svr2CltCfgDict != null && GameDataMgr.svr2CltCfgDict.ContainsKey(36u);
	}

	public void SelectAccSys(uint gsdk, uint netacc)
	{
		this.m_GSDKConfig = (GSDKsys.GSDKConfigMode)gsdk;
		this.m_NetACCConfig = (NetworkAccelerator.XunYouMode)netacc;
		GSDKsys.PrintLog(string.Concat(new object[]
		{
			"selectaccsys ",
			gsdk,
			" ",
			netacc
		}));
		if (this.m_NetACCConfig > NetworkAccelerator.XunYouMode.Disable)
		{
			NetworkAccelerator.InitACC(this.m_NetACCConfig);
		}
		if (this.m_GSDKConfig > GSDKsys.GSDKConfigMode.diable)
		{
			this.InitGSDK();
		}
	}

	public void DetermineWhichSpeed()
	{
		if (!NetworkAccelerator.IsCommercialized())
		{
			if (NetworkAccelerator.Mode == NetworkAccelerator.XunYouMode.Disable)
			{
				NetworkAccelerator.SetNetAccConfig(false);
			}
			else if (!NetworkAccelerator.Inited)
			{
				NetworkAccelerator.SetNetAccConfig(false);
			}
			else if (NetworkAccelerator.IsAutoNetAccConfigOpen() || NetworkAccelerator.IsNetAccConfigOpen())
			{
				NetworkAccelerator.SetNetAccConfig(true);
			}
			else
			{
				NetworkAccelerator.SetNetAccConfig(false);
			}
		}
		else if (!NetworkAccelerator.isAccelOpened())
		{
			this.StartGSDKSpeed(true);
		}
	}

	public void StartGSDKSpeed(bool open)
	{
		if (this.m_bInit)
		{
			if (open)
			{
				if (this.m_GSDKConfig == GSDKsys.GSDKConfigMode.speed || this.m_GSDKConfig == GSDKsys.GSDKConfigMode.ALL)
				{
					this.SetUseSpeed(true);
				}
				else
				{
					this.SetUseSpeed(false);
				}
			}
			else
			{
				this.SetUseSpeed(false);
			}
		}
		else
		{
			this.SetUseSpeed(false);
		}
	}

	public void SetUseSpeed(bool bUse)
	{
		if (bUse)
		{
			this.m_bUseSpeed = 0;
		}
		else
		{
			this.m_bUseSpeed = 1;
		}
		GSDKsys.PrintLog("SetUseSpeed " + this.m_bUseSpeed);
	}

	public void InitGSDK()
	{
		if (this.m_bInit)
		{
			GSDKsys.PrintLog("already init");
			GSDK.SetZoneId(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID);
			return;
		}
		this.m_GsdkSpeedFlag = -1000;
		GSDK.Init(ApolloConfig.appID, false, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID);
		this.m_bInit = true;
		this.m_bUseSpeed = 1;
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCheckMna, new CUIEventManager.OnUIEventHandler(this.OnCheckMna));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCheckMnaTimeOut, new CUIEventManager.OnUIEventHandler(this.OnCheckMnaTimeOut));
		this.SetUserName();
		GSDK.SetObserver(delegate(StartSpeedRet ret)
		{
			this.m_GsdkSpeedFlag = ret.flag;
		});
		GSDK.SetKartinObserver(delegate(KartinRet result)
		{
			GSDKsys.PrintLog("StartKartin result  " + Time.realtimeSinceStartup);
			GSDKsys.PrintLog(result.ToString());
			this.m_lastQuery = result;
			this.m_LastQueryStr = result.ToString();
			this.m_bStartKartin = false;
			this.m_bDealyUpdate = true;
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		});
		GSDKsys.PrintLog(" init succ");
	}

	private void OnCheckMna(CUIEvent uiEvent)
	{
		this.m_bStartKartin = true;
		Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(10, enUIEventID.Settings_OnCheckMnaTimeOut);
		this.setStartKartinButtonEnable();
		this.StartKartin(DateTime.Now.ToString());
	}

	private void OnCheckMnaTimeOut(CUIEvent uiEvent)
	{
		this.UpdateUI(true);
	}

	private void SetBarColor(Image image, int bRed)
	{
		if (image)
		{
			if (bRed == 2)
			{
				image.color = new Color(0.882f, 0.21168f, 0.21952f);
			}
			else if (bRed == 1)
			{
				image.color = new Color(8.83568f, 0.70168f, 0.22736001f);
			}
			else if (bRed == 0)
			{
				image.color = new Color(0.08624f, 0.71736f, 0.29792f);
			}
			else
			{
				image.color = new Color(0.18816f, 0.2156f, 0.27832f);
			}
		}
	}

	private void SetTextColor(Text text, int bRed, string str)
	{
		if (text)
		{
			if (bRed == 2)
			{
				text.text = string.Format("<color=#ff3638>{0}</color>", str);
			}
			else if (bRed == 1)
			{
				text.text = string.Format("<color=#ffb33a>{0}</color>", str);
			}
			else if (bRed == 0)
			{
				text.text = string.Format("<color=#16B74C>{0}</color>", str);
			}
			else
			{
				text.text = string.Format("<color=#303747>{0}</color>", str);
			}
		}
	}

	private void SetDefault()
	{
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			this.m_lastQuery.jump_network = 4;
		}
		else
		{
			this.m_lastQuery.jump_network = -2;
		}
		this.m_lastQuery.jump_export = 0;
		this.m_lastQuery.jump_router = 0;
		this.m_lastQuery.jump_terminal = 0;
		this.m_lastQuery.export_status = -1;
		this.m_lastQuery.router_status = -1;
		this.m_lastQuery.terminal_status = -1;
		this.m_lastQuery.jump_signal = -1;
		this.m_lastQuery.export_desc = (this.m_lastQuery.router_desc = (this.m_lastQuery.terminal_desc = "等待实时监测"));
		this.m_lastQuery.signal_desc = string.Empty;
	}

	private void Update()
	{
		if (!this.m_bDealyUpdate)
		{
			return;
		}
		this.m_bDealyUpdate = false;
		this.UpdateUI(false);
	}

	public void UpdateUI(bool bDefault = false)
	{
		this.m_bDealyUpdate = false;
		CUIFormScript getCurForm = Singleton<CSettingsSys>.GetInstance().GetCurForm;
		if (getCurForm == null)
		{
			return;
		}
		GameObject gameObject = getCurForm.m_formWidgets[95];
		if (gameObject == null)
		{
			return;
		}
		GSDKsys.PrintLog("Update UI");
		Transform transform = gameObject.transform;
		if (bDefault)
		{
			this.m_bStartKartin = false;
			this.SetDefault();
		}
		else
		{
			if (this.m_lastQuery.flag != 0)
			{
				if (this.m_lastQuery.flag >= -7 && this.m_lastQuery.flag <= -1)
				{
					string text = string.Format("Mna_{0}", Mathf.Abs(this.m_lastQuery.flag));
					text = Singleton<CTextManager>.GetInstance().GetText(text);
					Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("网络诊断未知错误", false, 1.5f, null, new object[0]);
				}
				this.SetDefault();
				this.setStartKartinButtonEnable();
				return;
			}
			Transform transform2 = transform.transform.FindChild("TopBar");
			if (transform2)
			{
				transform2.GetComponent<CUIAnimatorScript>().PlayAnimator("DateBarLoop");
			}
		}
		int jump_network = this.m_lastQuery.jump_network;
		for (int i = 0; i < 3; i++)
		{
			Transform transform3 = transform.transform.FindChild(string.Format("TopBar/DateBar{0}", i + 1));
			if (transform3 != null)
			{
				Transform transform4 = transform3.FindChild("Bar");
				Transform transform5 = transform3.FindChild("SomeTitle");
				Transform transform6 = transform3.FindChild("txt8");
				Transform transform7 = transform3.FindChild("desc");
				if (transform5)
				{
					if (i == 1)
					{
						if (jump_network == 4)
						{
							transform5.GetComponent<Text>().text = "路由器延迟";
						}
						else
						{
							transform5.GetComponent<Text>().text = "基站延迟";
						}
					}
					else if (i == 0)
					{
						transform5.GetComponent<Text>().text = "社区宽带延迟";
					}
					else if (i == 2)
					{
						transform5.GetComponent<Text>().text = "共享WIFI设备";
					}
				}
				if (jump_network == 4)
				{
					transform3.gameObject.CustomSetActive(true);
					if (i == 0)
					{
						this.SetTextColor(transform6.GetComponent<Text>(), this.m_lastQuery.export_status, this.m_lastQuery.jump_export.ToString() + "ms");
						transform7.GetComponent<Text>().text = this.m_lastQuery.export_desc.ToString();
						this.SetBarColor(transform4.GetComponent<Image>(), this.m_lastQuery.export_status);
					}
					else if (i == 1)
					{
						this.SetTextColor(transform6.GetComponent<Text>(), this.m_lastQuery.router_status, this.m_lastQuery.jump_router.ToString() + "ms");
						transform7.GetComponent<Text>().text = this.m_lastQuery.router_desc.ToString();
						this.SetBarColor(transform4.GetComponent<Image>(), this.m_lastQuery.router_status);
					}
					else if (i == 2)
					{
						this.SetTextColor(transform6.GetComponent<Text>(), this.m_lastQuery.terminal_status, this.m_lastQuery.jump_terminal.ToString() + "台");
						transform7.GetComponent<Text>().text = this.m_lastQuery.terminal_desc.ToString();
						this.SetBarColor(transform4.GetComponent<Image>(), this.m_lastQuery.terminal_status);
					}
				}
				else if (i == 0 || i == 2)
				{
					transform3.gameObject.CustomSetActive(false);
				}
				else if (i == 1)
				{
					transform3.gameObject.CustomSetActive(true);
					this.SetTextColor(transform6.GetComponent<Text>(), this.m_lastQuery.export_status, this.m_lastQuery.jump_export.ToString() + "ms");
					transform7.GetComponent<Text>().text = this.m_lastQuery.export_desc.ToString();
					this.SetBarColor(transform4.GetComponent<Image>(), this.m_lastQuery.export_status);
				}
			}
		}
		Transform transform8 = transform.transform.FindChild("TopBar/netType");
		if (transform8)
		{
			Text component = transform8.GetComponent<Text>();
			if (component)
			{
				if (jump_network == 4)
				{
					component.text = "WIFI";
				}
				else if (jump_network == 3)
				{
					component.text = "4G";
				}
				else if (jump_network == 2)
				{
					component.text = "3G";
				}
				else if (jump_network == 1)
				{
					component.text = "2G";
				}
				else
				{
					component.text = "非WIFI网络";
				}
			}
		}
		Transform root = transform.transform.FindChild("TopBar/signalVaule");
		this.SetSignalValueUI(root, this.m_lastQuery.jump_signal);
		Transform transform9 = transform.FindChild("TopBar/signaldesc2");
		if (transform9)
		{
			transform9.GetComponent<Text>().text = this.m_lastQuery.signal_desc;
		}
		this.setStartKartinButtonEnable();
	}

	private void setStartKartinButtonEnable()
	{
		CUIFormScript getCurForm = Singleton<CSettingsSys>.GetInstance().GetCurForm;
		if (getCurForm == null)
		{
			return;
		}
		GameObject gameObject = getCurForm.m_formWidgets[95];
		if (gameObject == null)
		{
			return;
		}
		Transform transform = gameObject.transform;
		Transform transform2 = transform.FindChild("Bottom/Button_Check");
		if (transform2)
		{
			transform2.gameObject.CustomSetActive(!this.m_bStartKartin);
		}
		transform2 = transform.FindChild("Bottom/Text");
		if (transform2)
		{
			transform2.gameObject.CustomSetActive(this.m_bStartKartin);
		}
	}

	private void SetSignalValueUI(Transform root, int signalValue)
	{
		if (root == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			Transform transform = root.FindChild("signal" + i);
			if (i <= signalValue)
			{
				if (this.m_lastQuery.signal_status == 0)
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), GSDKsys.Signal_Value_ImgGreen, null, true, false, false, false);
				}
				else if (this.m_lastQuery.signal_status == 1)
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), GSDKsys.Signal_Value_ImgYellow, null, true, false, false, false);
				}
				else if (this.m_lastQuery.signal_status == 2)
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), GSDKsys.Signal_Value_ImgRed, null, true, false, false, false);
				}
				else
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), GSDKsys.Signal_Value_ImgBg, null, true, false, false, false);
				}
			}
			else
			{
				CUIUtility.SetImageSprite(transform.GetComponent<Image>(), GSDKsys.Signal_Value_ImgBg, null, true, false, false, false);
			}
		}
	}

	public void SetUserName()
	{
		if (this.m_bInit)
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo != null)
			{
				GSDK.SetUserName((int)ApolloConfig.platform, accountInfo.OpenId);
			}
		}
	}

	public void StartSpeed(string vip, int vport)
	{
		if (this.m_bInit && this.m_bUseSpeed == 0)
		{
			this.m_GsdkSpeedFlag = -1000;
			this.m_LastIP = vip;
			this.m_LastPort = vport;
			GSDK.StartSpeed(vip, vport, 1, "libapollo.so", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, this.m_bUseSpeed);
			GSDKsys.PrintLog("start speed");
		}
	}

	public void EndSpeed()
	{
		if (this.m_bInit && this.m_bUseSpeed == 0)
		{
			GSDK.EndSpeed(this.m_LastIP, this.m_LastPort);
			GSDKsys.PrintLog("end speed");
		}
	}

	public void StartKartin(string tag)
	{
		if (this.m_bInit)
		{
			GSDKsys.PrintLog("StartKartin " + Time.realtimeSinceStartup);
			GSDK.QueryKartin(tag);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (this.m_bInit)
		{
			if (!pause)
			{
				GSDK.GoFront();
			}
			else
			{
				GSDK.GoBack();
			}
		}
		if (NetworkAccelerator.Inited)
		{
			if (!pause)
			{
				NetworkAccelerator.GoFront();
			}
			else
			{
				NetworkAccelerator.GoBack();
			}
		}
	}

	public SpeedInfo GetSpeedInfo(string vip, int vport)
	{
		if (this.m_bInit)
		{
			return GSDK.GetSpeedInfo(vip, vport);
		}
		return new SpeedInfo();
	}
}

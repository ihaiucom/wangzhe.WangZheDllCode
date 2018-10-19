using Apollo;
using Assets.Scripts.UI;
using com.tencent.pandora;
using CSProtocol;
using MiniJSON;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameSystem
{
	public class PandroaSys : MonoSingleton<PandroaSys>
	{
		public enum PandoraModuleType
		{
			action,
			shop,
			lucky,
			friend,
			MAX
		}

		private enum PandoraType
		{
			showIcon,
			showRedPoint,
			showLoading
		}

		private struct PandoraInfo
		{
			public PandroaSys.PandoraType type;

			public PandroaSys.PandoraModuleType module;

			public string tableID;

			public string tablename;

			public int content;
		}

		private const int MAX_PANDORA_TAB_UI = 5;

		private bool m_bShowPopNew;

		private bool m_bInit;

		public bool m_bOpenWeixinZone;

		public bool m_bShowWeixinZone;

		private bool m_bstartOPenRedBox;

		private bool m_bstartOPenRedBoxNew;

		public bool m_bShowBoxBtn;

		public bool m_bShowRedPoint;

		private int[] m_nPandoraActionTabCount = new int[5];

		private ListView<string>[] m_PandoraActionTabName = new ListView<string>[5];

		private Dictionary<string, PandroaSys.PandoraInfo>[] m_showIconDic = new Dictionary<string, PandroaSys.PandoraInfo>[5];

		private Dictionary<string, PandroaSys.PandoraInfo>[] m_showRedPointDic = new Dictionary<string, PandroaSys.PandoraInfo>[5];

		private uint m_addRelationWorldID;

		private uint m_addRealtionUID;

		private int m_mentorType;

		public bool ShowRedPoint
		{
			get
			{
				return this.m_bShowRedPoint;
			}
		}

		protected override void Init()
		{
			base.Init();
			this.InitTabData();
		}

		public void PausePandoraSys(bool bPause)
		{
			if (this.m_bInit)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (bPause)
				{
					dictionary["type"] = "inMainSence";
					dictionary["content"] = "0";
					Pandora.Instance.Do(dictionary);
				}
				else
				{
					dictionary["type"] = "inMainSence";
					dictionary["content"] = "1";
					Pandora.Instance.Do(dictionary);
				}
			}
		}

		public void InitSys()
		{
			if (!this.m_bInit)
			{
				this.InitEvent();
				Debug.Log("Pandora InitSys");
				this.m_bShowPopNew = false;
				this.m_bShowBoxBtn = false;
				this.m_bShowRedPoint = false;
				this.InitTabData();
				Pandora.Instance.Init();
				Pandora.Instance.SetQueryFriendsListCallback(new Func<string>(PandroaSys.GetFriendsID));
				Pandora.Instance.SetQuerySearchConfigCallback(new Func<string>(PandroaSys.GetSocialConfigs));
				Pandora.Instance.SetGetGameImgByPathCallback(new Func<GameObject, string, int>(this.OnGetImageFromPath));
				Pandora.Instance.SetQueryImgPathCallback(new Func<int, int, string>(this.OnGetGradeIcon));
				Pandora.Instance.SetQueryRankShowNameCallback(new Func<int, int, string>(this.OnGetGradeName));
				Pandora.Instance.SetCallback(new Func<Dictionary<string, object>, Action<string>, string>(this.HandlePandoraCall));
				this.InitPara();
				this.m_bInit = true;
			}
			Singleton<EventRouter>.GetInstance().AddEventHandler<int, int>(EventID.GPS_DATA_GOT, new Action<int, int>(this.OnGPSDataGot));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg, enFriendSearchSource>("Friend_Search", new Action<CSPkg, enFriendSearchSource>(this.OnSearchFriend));
		}

		public void UninitSys()
		{
			Debug.Log("Pandora UnInitSys");
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<int, int>(EventID.GPS_DATA_GOT, new Action<int, int>(this.OnGPSDataGot));
			this.m_bInit = false;
			this.m_bShowPopNew = false;
			this.m_bShowBoxBtn = false;
			this.m_bShowRedPoint = false;
			this.m_bOpenWeixinZone = false;
			this.m_bShowWeixinZone = false;
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
			Pandora.Instance.Logout();
		}

		private void InitEvent()
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
		}

		private void OnShowActBox(CUIEvent uiEvent)
		{
			if (this.m_bInit)
			{
				this.ShowActBox();
			}
		}

		private static string GetFriendsID()
		{
			return Singleton<CFriendContoller>.GetInstance().model.GetAllFriendsIDs();
		}

		private static string GetSocialConfigs()
		{
			return Singleton<CPlayerSocialInfoController>.GetInstance().GetSocialConfigStr();
		}

		public void ShowActiveActBoxBtn(CUIFormScript uiForm)
		{
			if (uiForm == null)
			{
				return;
			}
			if (this.m_bInit)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("pandroa_Btn_Text");
				Transform transform = uiForm.gameObject.transform.Find("Panel/PandroaBtn");
				if (transform)
				{
					if (this.m_bShowBoxBtn)
					{
						transform.gameObject.CustomSetActive(true);
					}
					else
					{
						transform.gameObject.CustomSetActive(false);
					}
					Transform transform2 = transform.Find("Hotspot");
					if (transform2)
					{
						if (this.m_bShowRedPoint)
						{
							transform2.gameObject.CustomSetActive(true);
						}
						else
						{
							transform2.gameObject.CustomSetActive(false);
						}
					}
					Transform transform3 = transform.Find("Text");
					if (transform3)
					{
						Text component = transform3.GetComponent<Text>();
						if (component)
						{
							component.text = text;
						}
					}
				}
			}
			else
			{
				Transform transform4 = uiForm.gameObject.transform.Find("Panel/PandroaBtn");
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(false);
					Transform transform5 = transform4.Find("Hotspot");
					if (transform5)
					{
						transform5.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void InitPara()
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			string value = "Smoba";
			string openId = accountInfo.OpenId;
			string value2 = "qq";
			string value3 = string.Empty;
			string value4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
			string value5 = string.Empty;
			string value6 = string.Empty;
			foreach (ApolloToken current in accountInfo.TokenList)
			{
				if (ApolloConfig.platform == ApolloPlatform.Wechat)
				{
					if (current.Type == ApolloTokenType.Access)
					{
						value5 = current.Value;
					}
				}
				else if (ApolloConfig.platform == ApolloPlatform.QQ)
				{
					if (current.Type == ApolloTokenType.Pay)
					{
						value6 = current.Value;
					}
					if (current.Type == ApolloTokenType.Access)
					{
						value5 = current.Value;
					}
				}
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				value2 = "qq";
				if (Application.platform == RuntimePlatform.Android)
				{
					value3 = "1";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					value3 = "2";
				}
			}
			else if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				value2 = "wx";
				if (Application.platform == RuntimePlatform.Android)
				{
					value3 = "3";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					value3 = "4";
				}
			}
			string value7 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
			string value8 = ApolloConfig.appID;
			if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				value8 = ApolloConfig.WXAppID;
			}
			string appVersion = CVersion.GetAppVersion();
			string value9 = "1";
			GameObject gameObject = base.gameObject;
			Pandora.Instance.SetPandoraParent(gameObject);
			Pandora.Instance.SetPanelBaseDepth(1000);
			Pandora.Instance.SetCallback(new Action<Dictionary<string, string>>(this.OnPandoraEvent));
			Pandora.Instance.SetGetDjImageCallback(new Func<GameObject, int, int, int>(this.OnGetDjImageCallback));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("sOpenId", openId);
			dictionary.Add("sServiceType", value);
			dictionary.Add("sAcountType", value2);
			dictionary.Add("sArea", value3);
			dictionary.Add("sPartition", value7);
			dictionary.Add("sAppId", value8);
			dictionary.Add("sRoleId", value4);
			dictionary.Add("sAccessToken", value5);
			dictionary.Add("sPayToken", value6);
			dictionary.Add("sGameVer", appVersion);
			dictionary.Add("sPlatID", value9);
			Pandora.Instance.SetUserData(dictionary);
		}

		public void InitWechatLink()
		{
			if (ApolloConfig.platform != ApolloPlatform.Wechat)
			{
				return;
			}
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			string value = "Smoba";
			string openId = accountInfo.OpenId;
			string value2 = "qq";
			string value3 = string.Empty;
			string value4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
			string value5 = string.Empty;
			string value6 = string.Empty;
			foreach (ApolloToken current in accountInfo.TokenList)
			{
				if (ApolloConfig.platform == ApolloPlatform.Wechat)
				{
					if (current.Type == ApolloTokenType.Access)
					{
						value5 = current.Value;
					}
				}
				else if (ApolloConfig.platform == ApolloPlatform.QQ)
				{
					if (current.Type == ApolloTokenType.Pay)
					{
						value6 = current.Value;
					}
					if (current.Type == ApolloTokenType.Access)
					{
						value5 = current.Value;
					}
				}
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				value2 = "qq";
				if (Application.platform == RuntimePlatform.Android)
				{
					value3 = "1";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					value3 = "2";
				}
			}
			else if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				value2 = "wx";
				if (Application.platform == RuntimePlatform.Android)
				{
					value3 = "3";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					value3 = "4";
				}
			}
			string value7 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
			string value8 = ApolloConfig.appID;
			if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				value8 = ApolloConfig.WXAppID;
			}
			string appVersion = CVersion.GetAppVersion();
			string value9 = "1";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("sOpenId", openId);
			dictionary.Add("sServiceType", value);
			dictionary.Add("sAcountType", value2);
			dictionary.Add("sArea", value3);
			dictionary.Add("sPartition", value7);
			dictionary.Add("sAppId", value8);
			dictionary.Add("sRoleId", value4);
			dictionary.Add("sAccessToken", value5);
			dictionary.Add("sPayToken", value6);
			dictionary.Add("sGameVer", appVersion);
			dictionary.Add("sPlatID", value9);
			this.m_bShowWeixinZone = false;
			if (ApolloConfig.platform == ApolloPlatform.Wechat && this.m_bOpenWeixinZone)
			{
				int @int = PlayerPrefs.GetInt("SHOW_WEIXINZONE");
				if (@int >= 1)
				{
					this.m_bShowWeixinZone = true;
				}
				WeChatLink.Instance.BeginGetGameZoneUrl(dictionary, new Action<Dictionary<string, string>>(this.OnGetGameZoneUrl));
			}
		}

		public void StartOpenRedBox(int bWin, int bMvp, int bLegaendary, int bPENTAKILL, int bQUATARYKIL, int bTRIPLEKILL, string mode)
		{
			Debug.Log("Pandora StartOpenRedBox1");
			if (this.m_bInit)
			{
				this.m_bstartOPenRedBox = true;
				Debug.Log("Pandora StartOpenRedBox2");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "RedPacket";
				dictionary["content"] = "1";
				dictionary["mode"] = mode;
				dictionary["is_legendary"] = bLegaendary.ToString();
				dictionary["is_mvp"] = bMvp.ToString();
				dictionary["is_penta_kill"] = bPENTAKILL.ToString();
				dictionary["is_quadra_kill"] = bQUATARYKIL.ToString();
				dictionary["is_triple_kill"] = bTRIPLEKILL.ToString();
				dictionary["is_victory"] = bWin.ToString();
				Pandora.Instance.Do(dictionary);
			}
		}

		public void StopRedBox()
		{
			Debug.Log("Pandora StopRedBox1");
			if (this.m_bInit && this.m_bstartOPenRedBox)
			{
				this.m_bstartOPenRedBox = false;
				Debug.Log("Pandora StopRedBox2");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "RedPacket";
				dictionary["content"] = "0";
				Pandora.Instance.Do(dictionary);
			}
		}

		public void SendLBSPos()
		{
			if (!Singleton<CFriendContoller>.GetInstance().IsLocationShareEnable())
			{
				this.OnGPSDataGot(0, 0);
			}
			else if (!MonoSingleton<GPSSys>.instance.bGetGPSData)
			{
				MonoSingleton<GPSSys>.instance.StartGPS();
				Singleton<CUIManager>.instance.OpenTips("正在定位，请稍后...", false, 1.5f, null, new object[0]);
			}
			else
			{
				this.OnGPSDataGot(MonoSingleton<GPSSys>.instance.iLongitude, MonoSingleton<GPSSys>.instance.iLatitude);
			}
		}

		private void OnGPSDataGot(int longitude, int latitude)
		{
			Debug.Log("Pandora Send LBS Pos");
			if (this.m_bInit)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "updateLocation";
				dictionary["longitude"] = longitude.ToString();
				dictionary["latitude"] = latitude.ToString();
				Pandora.Instance.Do(dictionary);
			}
		}

		private void OnSearchFriend(CSPkg msg, enFriendSearchSource searchSource)
		{
			if (searchSource != enFriendSearchSource.PandoraSystem || !this.m_bInit)
			{
				return;
			}
			SCPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerRsp = msg.stPkgData.stFriendSearchPlayerRsp;
			if (stFriendSearchPlayerRsp.dwResult != 0u)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendSearchPlayerRsp.dwResult), false, 1.5f, null, new object[0]);
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = "SearchResult";
			dictionary["UID"] = stFriendSearchPlayerRsp.stUserInfo.stUin.ullUid.ToString();
			dictionary["worldID"] = stFriendSearchPlayerRsp.stUserInfo.stUin.dwLogicWorldId.ToString();
			dictionary["openid"] = StringHelper.UTF8BytesToString(ref stFriendSearchPlayerRsp.stUserInfo.szOpenId);
			dictionary["userName"] = StringHelper.UTF8BytesToString(ref stFriendSearchPlayerRsp.stUserInfo.szUserName);
			dictionary["headUrl"] = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref stFriendSearchPlayerRsp.stUserInfo.szHeadUrl);
			dictionary["level"] = stFriendSearchPlayerRsp.stUserInfo.dwLevel.ToString();
			dictionary["pvpLvl"] = stFriendSearchPlayerRsp.stUserInfo.dwPvpLvl.ToString();
			dictionary["rankClass"] = stFriendSearchPlayerRsp.stUserInfo.dwRankClass.ToString();
			dictionary["masterLvl"] = stFriendSearchPlayerRsp.stUserInfo.dwMasterLvl.ToString();
			dictionary["gender"] = stFriendSearchPlayerRsp.stUserInfo.bGender.ToString();
			dictionary["location"] = StringHelper.UTF8BytesToString(ref stFriendSearchPlayerRsp.stFriendCard.szPlace);
			dictionary["searchTarget"] = Singleton<CPlayerSocialInfoController>.GetInstance().GetSearchStr(0, (int)stFriendSearchPlayerRsp.stFriendCard.dwSearchType);
			dictionary["durationTime"] = Singleton<CPlayerSocialInfoController>.GetInstance().GetSearchStr(1, (int)stFriendSearchPlayerRsp.stFriendCard.dwDay) + "-" + Singleton<CPlayerSocialInfoController>.GetInstance().GetSearchStr(2, (int)stFriendSearchPlayerRsp.stFriendCard.dwHour);
			dictionary["studentNum"] = stFriendSearchPlayerRsp.stUserInfo.dwStudentNum.ToString();
			dictionary["rankShowGrade"] = stFriendSearchPlayerRsp.stUserInfo.stRankShowGrade.bGrade.ToString();
			dictionary["rankLogicGrade"] = CLadderSystem.GetGradeDataByLogicGrade((int)stFriendSearchPlayerRsp.stUserInfo.stRankShowGrade.bGrade).bLogicGrade.ToString();
			dictionary["rankScore"] = stFriendSearchPlayerRsp.stUserInfo.stRankShowGrade.dwScore.ToString();
			dictionary["isOnline"] = stFriendSearchPlayerRsp.stUserInfo.bIsOnline.ToString();
			dictionary["vipLevel"] = stFriendSearchPlayerRsp.stUserInfo.stGameVip.dwCurLevel.ToString();
			Pandora.Instance.Do(dictionary);
		}

		public void StartOpeninSettlement(int playeNum, COM_GAME_TYPE gameType, int isWarm, int isCPU, enSelectType bpCLone, int mapID, int isLuanDou, int isFireHole, int bWin, int bMvp, int bLegaendary, int bPENTAKILL, int bQUATARYKIL, int bTRIPLEKILL)
		{
			Debug.Log("Pandora StartOpenRedBox1New");
			if (this.m_bInit)
			{
				this.m_bstartOPenRedBoxNew = true;
				Debug.Log("Pandora StartOpenRedBox2New");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "inSettlement";
				dictionary["content"] = "1";
				dictionary["mode"] = gameType.ToString();
				dictionary["playernum"] = playeNum.ToString();
				dictionary["is_warm"] = isWarm.ToString();
				dictionary["is_cpu"] = isCPU.ToString();
				dictionary["is_bpclone"] = bpCLone.ToString();
				dictionary["mapid"] = mapID.ToString();
				dictionary["is_luandou"] = isLuanDou.ToString();
				dictionary["is_firehole"] = isFireHole.ToString();
				dictionary["is_legendary"] = bLegaendary.ToString();
				dictionary["is_mvp"] = bMvp.ToString();
				dictionary["is_penta_kill"] = bPENTAKILL.ToString();
				dictionary["is_quadra_kill"] = bQUATARYKIL.ToString();
				dictionary["is_triple_kill"] = bTRIPLEKILL.ToString();
				dictionary["is_victory"] = bWin.ToString();
				Pandora.Instance.Do(dictionary);
			}
		}

		public void StopinSettlement()
		{
			Debug.Log("Pandora StopRedBox1");
			if (this.m_bInit && this.m_bstartOPenRedBoxNew)
			{
				this.m_bstartOPenRedBoxNew = false;
				Debug.Log("Pandora StopRedBox2New");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "inSettlement";
				dictionary["content"] = "0";
				Pandora.Instance.Do(dictionary);
			}
		}

		private void OnGetGameZoneUrl(Dictionary<string, string> mDic)
		{
			if (mDic != null && mDic.ContainsKey("showGameZone"))
			{
				if (mDic["showGameZone"] == "1")
				{
					Debug.Log("revce OnGetGameZoneUrl ");
					this.m_bShowWeixinZone = true;
					PlayerPrefs.SetInt("SHOW_WEIXINZONE", 2);
					PlayerPrefs.Save();
				}
				else
				{
					this.m_bShowWeixinZone = false;
					PlayerPrefs.SetInt("SHOW_WEIXINZONE", 0);
					PlayerPrefs.Save();
				}
			}
		}

		public void ShowActBox()
		{
			Debug.Log("Pandora ShowActBox1");
			if (this.m_bInit)
			{
				Debug.Log("Pandora ShowActBox2");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "open";
				dictionary["content"] = "Lucky";
				Pandora.Instance.Do(dictionary);
			}
		}

		public void ShowPopNews()
		{
			if (this.m_bInit && !this.m_bShowPopNew)
			{
				this.m_bShowPopNew = true;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["type"] = "open";
				dictionary["content"] = "LuckyPop";
				Pandora.Instance.Do(dictionary);
			}
		}

		public void InitPandoraTab(PandroaSys.PandoraModuleType ui, Transform trParent)
		{
			if (!this.m_bInit)
			{
				return;
			}
			if (ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return;
			}
			this.m_nPandoraActionTabCount[(int)ui] = 0;
			this.m_PandoraActionTabName[(int)ui].Clear();
			if (this.m_showIconDic[(int)ui] == null)
			{
				return;
			}
			bool flag = false;
			foreach (string current in this.m_showIconDic[(int)ui].Keys)
			{
				PandroaSys.PandoraInfo pandoraInfo = this.m_showIconDic[(int)ui][current];
				if (pandoraInfo.content > 0 && pandoraInfo.module == ui)
				{
					this.m_nPandoraActionTabCount[(int)ui]++;
					this.m_PandoraActionTabName[(int)ui].Add(pandoraInfo.tablename);
					flag = true;
				}
			}
			if (flag && trParent != null)
			{
				Pandora.Instance.SetPanelParent(trParent.gameObject, this.PandoraModuleTypeStr(ui));
			}
		}

		public void OnPandoraTabClick(PandroaSys.PandoraModuleType ui, int idxTab)
		{
			if (!this.m_bInit)
			{
				return;
			}
			if (ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return;
			}
			if (this.m_showIconDic[(int)ui] == null)
			{
				return;
			}
			int count = this.m_showIconDic[(int)ui].Count;
			if (idxTab < 0 && idxTab >= count)
			{
				return;
			}
			string pandoraTabName = this.GetPandoraTabName(ui, idxTab);
			string pandoraTabID = this.GetPandoraTabID(pandoraTabName, ui);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = "open";
			dictionary["module"] = this.PandoraModuleTypeStr(ui);
			dictionary["tab"] = pandoraTabID;
			Pandora.Instance.Do(dictionary);
		}

		public void ClosePandoraTabWindow(PandroaSys.PandoraModuleType ui)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["type"] = "close";
			dictionary["module"] = this.PandoraModuleTypeStr(ui);
			Pandora.Instance.Do(dictionary);
		}

		public int PandoraTabCount(PandroaSys.PandoraModuleType ui)
		{
			if (!this.m_bInit)
			{
				return 0;
			}
			if (ui < PandroaSys.PandoraModuleType.MAX)
			{
				return this.m_nPandoraActionTabCount[(int)ui];
			}
			return 0;
		}

		public int ShowPointCount(PandroaSys.PandoraModuleType ui)
		{
			int num = 0;
			if (!this.m_bInit || ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return 0;
			}
			foreach (string current in this.m_showRedPointDic[(int)ui].Keys)
			{
				PandroaSys.PandoraInfo pandoraInfo = this.m_showRedPointDic[(int)ui][current];
				if (pandoraInfo.content > 0)
				{
					num += pandoraInfo.content;
				}
			}
			return num;
		}

		public string GetPandoraTabName(PandroaSys.PandoraModuleType ui, int idx)
		{
			if (!this.m_bInit || ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return string.Empty;
			}
			int count = this.m_PandoraActionTabName[(int)ui].Count;
			if (idx >= 0 && idx < count)
			{
				return this.m_PandoraActionTabName[(int)ui][idx];
			}
			return string.Empty;
		}

		public int IsShowPandoraTabRedPointByTabIdx(PandroaSys.PandoraModuleType ui, int idxTab)
		{
			if (!this.m_bInit || ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return 0;
			}
			string pandoraTabName = this.GetPandoraTabName(ui, idxTab);
			string pandoraTabID = this.GetPandoraTabID(pandoraTabName, ui);
			return this.IsShowActionTabRedPoint(ui, pandoraTabID);
		}

		public string HandlePandoraCall(Dictionary<string, object> pDic, Action<string> action)
		{
			string result = string.Empty;
			Logger.DEBUG("OnPandoraEvent enter");
			if (!this.m_bInit || pDic == null)
			{
				return result;
			}
			if (pDic.ContainsKey("type"))
			{
				string text = pDic["type"] as string;
				if (text.Equals("transferHeadUrl"))
				{
					if (pDic.ContainsKey("headUrl"))
					{
						result = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(pDic["headUrl"].ToString());
					}
				}
				else if (text.Equals("refreshToken"))
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
					if (accountInfo != null)
					{
						dictionary["accessToken"] = Singleton<ApolloHelper>.GetInstance().GetAccessToken(ApolloConfig.platform);
						ApolloToken token = accountInfo.GetToken(ApolloTokenType.Pay);
						if (token != null)
						{
							dictionary["payToken"] = token.Value;
						}
					}
					result = Json.Serialize(dictionary);
				}
				else if (text.Equals("queryCardCity"))
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						result = StringHelper.UTF8BytesToString(ref masterRoleInfo.m_socialFriendCard.szPlace);
					}
				}
				else if (text.Equals("setItemCell"))
				{
					GameObject rootObj = null;
					GameObject itemCell = null;
					int itemType = 0;
					int itemID = 0;
					bool isHaveClickEvent = false;
					bool isClickToShowDetail = false;
					if (pDic.ContainsKey("itemGameObject"))
					{
						itemCell = (pDic["itemGameObject"] as GameObject);
					}
					if (pDic.ContainsKey("panelGameObject"))
					{
						rootObj = (pDic["panelGameObject"] as GameObject);
					}
					if (pDic.ContainsKey("itemType"))
					{
						string s = pDic["itemType"].ToString();
						int.TryParse(s, out itemType);
					}
					if (pDic.ContainsKey("itemId"))
					{
						string s2 = pDic["itemId"].ToString();
						int.TryParse(s2, out itemID);
					}
					if (pDic.ContainsKey("isHaveClickEvent"))
					{
						string s3 = pDic["isHaveClickEvent"].ToString();
						int num = 0;
						int.TryParse(s3, out num);
						if (num > 0)
						{
							isHaveClickEvent = true;
						}
					}
					if (pDic.ContainsKey("isClickToShowDetail"))
					{
						string s4 = pDic["isClickToShowDetail"].ToString();
						int num2 = 0;
						int.TryParse(s4, out num2);
						if (num2 > 0)
						{
							isClickToShowDetail = true;
						}
					}
					this.SetItemCell(rootObj, itemCell, itemType, itemID, isHaveClickEvent, isClickToShowDetail);
					result = string.Empty;
				}
				else if (text.Equals("queryConfigRankName"))
				{
					try
					{
						int jobType = Convert.ToInt32(pDic["rank"]);
						result = CHeroInfo.GetHeroJobStr((RES_HERO_JOB)jobType);
					}
					catch (Exception var_19_2AC)
					{
						Debug.LogError("queryConfigRankName wrong");
					}
				}
				else if (text.Equals("GetGradeSubIcon"))
				{
					try
					{
						ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade(Convert.ToInt32(pDic["rankGrade"]));
						byte bLogicGrade = gradeDataByLogicGrade.bLogicGrade;
						result = CLadderView.GetSubRankSmallIconPath(gradeDataByLogicGrade.bLogicGrade, Convert.ToUInt32(pDic["rankClass"]));
					}
					catch (Exception var_22_314)
					{
						Debug.LogError("GetGradeSubIcon wrong");
					}
				}
				else if (text.Equals("GetLogicGradeByShowGrade"))
				{
					try
					{
						ResRankGradeConf gradeDataByLogicGrade2 = CLadderSystem.GetGradeDataByLogicGrade(Convert.ToInt32(pDic["showGrade"]));
						result = gradeDataByLogicGrade2.bLogicGrade + string.Empty;
					}
					catch (Exception var_24_36D)
					{
						Debug.LogError("GetLogicGradeByShowGrade wrong");
					}
				}
			}
			return result;
		}

		public void OnPandoraEvent(Dictionary<string, string> dictData)
		{
			Logger.DEBUG("OnPandoraEvent enter");
			if (!this.m_bInit)
			{
				return;
			}
			if (dictData != null && dictData.ContainsKey("type"))
			{
				string text = dictData["type"];
				string text4 = string.Empty;
				if (dictData.ContainsKey("content"))
				{
					text4 = dictData["content"];
				}
				if (text == "redpoint")
				{
					int num = 0;
					int.TryParse(text4, out num);
					if (num <= 0)
					{
						this.m_bShowRedPoint = false;
					}
					else
					{
						this.m_bShowRedPoint = true;
					}
					Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
					Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
				}
				else if (text.Equals("showIcon"))
				{
					this.ProcessShowIcon(dictData);
				}
				else if (text.Equals("showRedPoint"))
				{
					this.ProcessShowRedPoint(dictData);
				}
				else if (text.Equals("closePopNews"))
				{
					this.m_bShowPopNew = true;
				}
				else if (text.Equals("addRelation"))
				{
					this.ProcessHandleAddRelationShip(dictData);
				}
				else if (text.Equals("searchFriend"))
				{
					FriendSysNetCore.Send_Serch_Player(dictData["useName"], enFriendSearchSource.PandoraSystem);
				}
				else if (text.Equals("ShowGameWindow"))
				{
					int num2 = 0;
					int.TryParse(text4, out num2);
					Logger.DEBUG("ShowGameWindow:" + num2);
					if (num2 > 0)
					{
						CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)num2, 0, 0, null);
					}
				}
				else if (text.Equals("refreshDiamond"))
				{
					Debug.Log("pandorasys ShowGameWindow");
					Singleton<CPaySystem>.GetInstance().OnPandroiaPaySuccss();
				}
				else if (text.Equals("share"))
				{
					string s = string.Empty;
					string url = string.Empty;
					string title = string.Empty;
					string desc = string.Empty;
					string text3 = string.Empty;
					string imgurl = string.Empty;
					int num3 = 0;
					if (dictData.ContainsKey("sendType"))
					{
						s = dictData["sendType"];
						num3++;
					}
					if (dictData.ContainsKey("url"))
					{
						url = dictData["url"];
						num3++;
					}
					if (dictData.ContainsKey("title"))
					{
						title = dictData["title"];
						num3++;
					}
					if (dictData.ContainsKey("desc"))
					{
						desc = dictData["desc"];
						num3++;
					}
					if (dictData.ContainsKey("static_kind"))
					{
						text3 = dictData["static_kind"];
						num3++;
					}
					if (dictData.ContainsKey("img"))
					{
						imgurl = dictData["img"];
						num3++;
					}
					if (num3 == 6)
					{
						int iSendType = 0;
						if (int.TryParse(s, out iSendType))
						{
							base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(imgurl, delegate(Texture2D text2)
							{
								Singleton<ApolloHelper>.GetInstance().ShareRedBox(iSendType, url, title, desc, text2, imgurl);
							}, 0));
						}
					}
					else
					{
						Debug.Log("pandroa sys parm error");
					}
				}
				else if (text.Equals("sendSNSMsg"))
				{
					int actID = 0;
					string openId = string.Empty;
					string title2 = string.Empty;
					string desc2 = string.Empty;
					string pic_url = string.Empty;
					string previewText = string.Empty;
					string gameTag = string.Empty;
					string extInfo = string.Empty;
					string mediaId = string.Empty;
					string messageExt = string.Empty;
					int num4 = 0;
					if (dictData.ContainsKey("openid"))
					{
						openId = dictData["openid"];
						num4++;
					}
					if (dictData.ContainsKey("title"))
					{
						title2 = dictData["title"];
						num4++;
					}
					if (dictData.ContainsKey("desc"))
					{
						desc2 = dictData["desc"];
						num4++;
					}
					if (dictData.ContainsKey("qqImageUrl"))
					{
						pic_url = dictData["qqImageUrl"];
						num4++;
					}
					if (dictData.ContainsKey("wxImageCode"))
					{
						mediaId = dictData["wxImageCode"];
						num4++;
					}
					if (dictData.ContainsKey("extInfo"))
					{
						extInfo = dictData["extInfo"];
						num4++;
					}
					if (dictData.ContainsKey("messageExt"))
					{
						messageExt = dictData["messageExt"];
						num4++;
					}
					if (dictData.ContainsKey("gameTag"))
					{
						gameTag = dictData["gameTag"];
						num4++;
					}
					if (dictData.ContainsKey("act"))
					{
						string s2 = string.Empty;
						s2 = dictData["act"];
						int.TryParse(s2, out actID);
						num4++;
					}
					if (dictData.ContainsKey("previewText"))
					{
						previewText = dictData["previewText"];
						num4++;
					}
					if (num4 == 10)
					{
						Singleton<ApolloHelper>.GetInstance().ShareSendHeartPandroa(actID, openId, title2, desc2, pic_url, previewText, gameTag, extInfo, mediaId, messageExt);
					}
				}
				else if (text.Equals("openUrl"))
				{
					if (!string.IsNullOrEmpty(text4))
					{
						CUICommonSystem.OpenUrl(text4, true, 0);
					}
				}
				else if (text.Equals("showLoading"))
				{
					int num5 = 0;
					int.TryParse(text4, out num5);
					if (num5 > 20)
					{
						num5 = 10;
					}
					if (num5 > 0)
					{
						Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, num5, enUIEventID.None);
					}
					else
					{
						Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
					}
				}
				else if (text.Equals("showItemTips"))
				{
					int num6 = 0;
					int.TryParse(text4, out num6);
					if (num6 == 0)
					{
						CUICommonSystem.CloseUseableTips();
					}
					else
					{
						int itemType = 0;
						int itemID = 0;
						int posX = 0;
						int posY = 0;
						int tipPos = 0;
						int num7 = 0;
						if (dictData.ContainsKey("itemtype"))
						{
							string s3 = dictData["itemtype"];
							int.TryParse(s3, out itemType);
							num7++;
						}
						if (dictData.ContainsKey("itemid"))
						{
							int.TryParse(dictData["itemid"], out itemID);
							num7++;
						}
						if (dictData.ContainsKey("posx"))
						{
							int.TryParse(dictData["posx"], out posX);
							num7++;
						}
						if (dictData.ContainsKey("posy"))
						{
							int.TryParse(dictData["posy"], out posY);
							num7++;
						}
						if (dictData.ContainsKey("tipspos"))
						{
							int.TryParse(dictData["tipspos"], out tipPos);
							num7++;
						}
						if (num7 == 5)
						{
							this.ShowItemTips(itemType, itemID, posX, posY, tipPos);
						}
					}
				}
				else if (text.Equals("checkLocation"))
				{
					this.SendLBSPos();
				}
				else if (text.Equals("showFriendCard") && dictData.ContainsKey("worldID") && dictData.ContainsKey("UID"))
				{
					Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(Convert.ToUInt64(dictData["UID"]), Convert.ToInt32(dictData["worldID"]), CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Social_Info);
				}
			}
		}

		private void ShowItemTips(int itemType, int itemID, int posX, int posY, int tipPos)
		{
			CUseable cUseable;
			if (itemType < 0)
			{
				cUseable = CUseableManager.CreateVirtualUseable((enVirtualItemType)Mathf.Abs(itemType), itemID);
			}
			else
			{
				cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)itemType, (uint)itemID, 0);
			}
			if (cUseable != null)
			{
				Singleton<CUICommonSystem>.GetInstance().OpenUseableTips(cUseable, (float)posX * 1f, (float)posY * 1f, (enUseableTipsPos)tipPos);
			}
		}

		private void InitTabData()
		{
			for (int i = 0; i < 5; i++)
			{
				this.m_showIconDic[i] = new Dictionary<string, PandroaSys.PandoraInfo>();
				this.m_showRedPointDic[i] = new Dictionary<string, PandroaSys.PandoraInfo>();
				this.m_PandoraActionTabName[i] = new ListView<string>();
				this.m_nPandoraActionTabCount[i] = 0;
			}
		}

		private string PandoraModuleTypeStr(PandroaSys.PandoraModuleType ui)
		{
			if (ui == PandroaSys.PandoraModuleType.action)
			{
				return "action";
			}
			if (ui == PandroaSys.PandoraModuleType.shop)
			{
				return "shop";
			}
			if (ui == PandroaSys.PandoraModuleType.friend)
			{
				return "friend";
			}
			return string.Empty;
		}

		private string GetPandoraTabID(string tabName, PandroaSys.PandoraModuleType moduleType)
		{
			if (moduleType >= PandroaSys.PandoraModuleType.MAX)
			{
				return string.Empty;
			}
			if (this.m_showIconDic[(int)moduleType] == null)
			{
				return string.Empty;
			}
			foreach (string current in this.m_showIconDic[(int)moduleType].Keys)
			{
				PandroaSys.PandoraInfo pandoraInfo = this.m_showIconDic[(int)moduleType][current];
				if (pandoraInfo.content > 0 && pandoraInfo.module == moduleType && tabName == pandoraInfo.tablename)
				{
					return pandoraInfo.tableID;
				}
			}
			return string.Empty;
		}

		private int IsShowActionTabRedPoint(PandroaSys.PandoraModuleType ui, string tabID)
		{
			if (ui >= PandroaSys.PandoraModuleType.MAX)
			{
				return 0;
			}
			PandroaSys.PandoraInfo pandoraInfo = default(PandroaSys.PandoraInfo);
			if (this.m_showRedPointDic[(int)ui].TryGetValue(tabID, out pandoraInfo) && pandoraInfo.content > 0)
			{
				return pandoraInfo.content;
			}
			return 0;
		}

		private void ProcessShowIcon(Dictionary<string, string> dictData)
		{
			if (dictData.ContainsKey("type"))
			{
				string text = dictData["type"];
				string text2 = string.Empty;
				string text3 = string.Empty;
				string tablename = string.Empty;
				string text4 = string.Empty;
				if (dictData.ContainsKey("content"))
				{
					text4 = dictData["content"];
				}
				if (dictData.ContainsKey("tab"))
				{
					text3 = dictData["tab"];
				}
				if (dictData.ContainsKey("showname"))
				{
					tablename = dictData["showname"];
				}
				if (dictData.ContainsKey("module"))
				{
					text2 = dictData["module"];
				}
				if (text2.Equals("lucky"))
				{
					if (text4.Equals("1"))
					{
						this.m_bShowBoxBtn = true;
					}
					else
					{
						this.m_bShowBoxBtn = false;
					}
					Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
				}
				else if (!string.IsNullOrEmpty(text3))
				{
					PandroaSys.PandoraModuleType pandoraModuleType;
					if (text2.Equals("action"))
					{
						pandoraModuleType = PandroaSys.PandoraModuleType.action;
					}
					else if (text2.Equals("shop"))
					{
						pandoraModuleType = PandroaSys.PandoraModuleType.shop;
					}
					else
					{
						if (!text2.Equals("friend"))
						{
							return;
						}
						pandoraModuleType = PandroaSys.PandoraModuleType.friend;
					}
					PandroaSys.PandoraInfo value;
					if (!this.m_showIconDic[(int)pandoraModuleType].ContainsKey(text3))
					{
						value = default(PandroaSys.PandoraInfo);
						this.m_showIconDic[(int)pandoraModuleType].Add(text3, value);
					}
					value = this.m_showIconDic[(int)pandoraModuleType][text3];
					if (text2.Equals("action"))
					{
						value.module = PandroaSys.PandoraModuleType.action;
					}
					else if (text2.Equals("shop"))
					{
						value.module = PandroaSys.PandoraModuleType.shop;
					}
					else if (text2.Equals("friend"))
					{
						value.module = PandroaSys.PandoraModuleType.friend;
					}
					value.type = PandroaSys.PandoraType.showIcon;
					value.tablename = tablename;
					value.tableID = text3;
					int.TryParse(text4, out value.content);
					this.m_showIconDic[(int)pandoraModuleType][text3] = value;
				}
			}
		}

		private void ProcessHandleAddRelationShip(Dictionary<string, string> dictData)
		{
			string value;
			string value2;
			string value3;
			if (dictData.TryGetValue("worldID", out value) && dictData.TryGetValue("UID", out value2) && dictData.TryGetValue("tag", out value3))
			{
				try
				{
					int num = Convert.ToInt32(value3);
					this.m_addRealtionUID = Convert.ToUInt32(value2);
					this.m_addRelationWorldID = Convert.ToUInt32(value);
					switch (num)
					{
					case 1:
					{
						string randomReuqestStr = CFriendView.Verfication.GetRandomReuqestStr("FriendVerify_Text_", 1, 4);
						Singleton<CUIManager>.GetInstance().OpenStringSenderBox(Singleton<CTextManager>.GetInstance().GetText("Friend_AddTitle"), Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), randomReuqestStr, new CUIManager.StringSendboxOnSend(this.OnFriendApplyVerifyBoxRetrun), randomReuqestStr);
						break;
					}
					case 2:
					{
						this.m_mentorType = 2;
						string randomReuqestStr2 = CFriendView.Verfication.GetRandomReuqestStr("Mentor_requestMentor_", 0, 3);
						Singleton<CUIManager>.GetInstance().OpenStringSenderBox(Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", new string[]
						{
							Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor")
						}), Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), randomReuqestStr2, new CUIManager.StringSendboxOnSend(this.OnMentorApplyVerifyBoxReturn), randomReuqestStr2);
						break;
					}
					case 3:
					{
						this.m_mentorType = 3;
						string randomReuqestStr3 = CFriendView.Verfication.GetRandomReuqestStr("Mentor_requestApprentice_", 0, 3);
						Singleton<CUIManager>.GetInstance().OpenStringSenderBox(Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", new string[]
						{
							Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice")
						}), Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), randomReuqestStr3, new CUIManager.StringSendboxOnSend(this.OnMentorApplyVerifyBoxReturn), randomReuqestStr3);
						break;
					}
					}
				}
				catch (Exception var_7_196)
				{
				}
			}
		}

		private void OnFriendApplyVerifyBoxRetrun(string str)
		{
			FriendSysNetCore.Send_Request_BeFriend((ulong)this.m_addRealtionUID, this.m_addRelationWorldID, str, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1);
		}

		private void OnMentorApplyVerifyBoxReturn(string str)
		{
			FriendSysNetCore.Send_Request_BeMentor((ulong)this.m_addRealtionUID, this.m_addRelationWorldID, this.m_mentorType, str);
		}

		private void ProcessShowRedPoint(Dictionary<string, string> dictData)
		{
			if (dictData.ContainsKey("type"))
			{
				string text = dictData["type"];
				string text2 = string.Empty;
				string text3 = string.Empty;
				string tablename = string.Empty;
				string s = string.Empty;
				if (dictData.ContainsKey("content"))
				{
					s = dictData["content"];
				}
				if (dictData.ContainsKey("tab"))
				{
					text3 = dictData["tab"];
				}
				if (dictData.ContainsKey("showname"))
				{
					tablename = dictData["showname"];
				}
				if (dictData.ContainsKey("module"))
				{
					text2 = dictData["module"];
				}
				if (!string.IsNullOrEmpty(text3))
				{
					PandroaSys.PandoraModuleType pandoraModuleType;
					if (text2.Equals("action"))
					{
						pandoraModuleType = PandroaSys.PandoraModuleType.action;
					}
					else if (text2.Equals("shop"))
					{
						pandoraModuleType = PandroaSys.PandoraModuleType.shop;
					}
					else
					{
						if (!text2.Equals("friend"))
						{
							return;
						}
						pandoraModuleType = PandroaSys.PandoraModuleType.friend;
					}
					PandroaSys.PandoraInfo value;
					if (this.m_showRedPointDic[(int)pandoraModuleType].ContainsKey(text3))
					{
						value = this.m_showRedPointDic[(int)pandoraModuleType][text3];
					}
					else
					{
						value = default(PandroaSys.PandoraInfo);
						this.m_showRedPointDic[(int)pandoraModuleType].Add(text3, value);
					}
					if (text2.Equals("action"))
					{
						value.module = PandroaSys.PandoraModuleType.action;
					}
					else if (text2.Equals("shop"))
					{
						value.module = PandroaSys.PandoraModuleType.shop;
					}
					else if (text2.Equals("friend"))
					{
						value.module = PandroaSys.PandoraModuleType.friend;
					}
					value.type = PandroaSys.PandoraType.showRedPoint;
					value.tablename = tablename;
					value.tablename = text3;
					int.TryParse(s, out value.content);
					this.m_showRedPointDic[(int)pandoraModuleType][text3] = value;
				}
				Singleton<ActivitySys>.GetInstance().UpdatePandoraRedPoint();
			}
		}

        private IEnumerator DeleteAddImage(GameObject imgObj, string path)
        {
            var srcImg = imgObj.GetComponent<Image>();
            if (srcImg == null)
            {
                yield break;
            }
            var srcImage2 = imgObj.GetComponent<Image2>();
            if (srcImage2 == null)
            {
                Object.Destroy(srcImg);
                yield return new WaitForEndOfFrame();
                if (imgObj != null)
                {
                    imgObj.AddComponent<Image2>();
                }
            }

            if (imgObj != null)
            {
                var img2 = imgObj.GetComponent<Image>();
                if (img2 != null)
                {
                    CUIUtility.SetImageSprite(img2, path, null, true, false, false, false);
                }
            }
        }
		

		public string OnGetGradeIcon(int rankGrade, int rankClass)
		{
			ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade(rankGrade);
			if (gradeDataByLogicGrade == null)
			{
				return string.Empty;
			}
			return CLadderView.GetRankSmallIconPath(gradeDataByLogicGrade.bLogicGrade, (uint)rankClass);
		}

		public string OnGetGradeName(int rankGrade, int rankClass)
		{
			ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade(rankGrade);
			if (gradeDataByLogicGrade == null)
			{
				return string.Empty;
			}
			return CLadderView.GetRankName(gradeDataByLogicGrade.bLogicGrade, (uint)rankClass);
		}

		public int OnGetImageFromPath(GameObject imgObj, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return -1;
			}
			base.StartCoroutine(this.DeleteAddImage(imgObj, path));
			return 0;
		}

		public int OnGetDjImageCallback(GameObject imgObj, int itemType, int itemID)
		{
			if (itemID < 0 || imgObj == null)
			{
				return -1;
			}
			CUseable cUseable;
			if (itemType < 0)
			{
				cUseable = CUseableManager.CreateVirtualUseable((enVirtualItemType)Mathf.Abs(itemType), itemID);
			}
			else
			{
				cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)itemType, (uint)itemID, 0);
			}
			if (cUseable != null)
			{
				string iconPath = cUseable.GetIconPath();
				base.StartCoroutine(this.DeleteAddImage(imgObj, iconPath));
				return 0;
			}
			return -2;
		}

		private void SetItemCell(GameObject rootObj, GameObject itemCell, int itemType, int itemID, bool isHaveClickEvent = false, bool isClickToShowDetail = false)
		{
			if (itemCell == null || rootObj == null)
			{
				return;
			}
			CUseable cUseable;
			if (itemType < 0)
			{
				cUseable = CUseableManager.CreateVirtualUseable((enVirtualItemType)Mathf.Abs(itemType), itemID);
			}
			else
			{
				cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)itemType, (uint)itemID, 0);
			}
			if (cUseable == null)
			{
				return;
			}
			Image component = itemCell.transform.Find("imgIcon").GetComponent<Image>();
			if (component)
			{
				base.StartCoroutine(this.DeleteAddImage(component.gameObject, cUseable.GetIconPath()));
			}
			string path = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", (int)(cUseable.m_grade + 1));
			base.StartCoroutine(this.DeleteAddImage(itemCell, path));
			Transform transform = itemCell.transform.Find("lblIconCount");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(true);
				Text component2 = transform.GetComponent<Text>();
				if (cUseable.m_stackCount < 10000)
				{
					component2.text = cUseable.m_stackCount.ToString();
				}
				else
				{
					component2.text = cUseable.m_stackCount / 10000 + "万";
				}
				CUICommonSystem.AppendMultipleText(component2, cUseable.m_stackMulti);
				if (cUseable.m_stackCount <= 0)
				{
					component2.gameObject.CustomSetActive(false);
				}
				if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					if (((CSymbolItem)cUseable).IsGuildSymbol())
					{
						component2.text = string.Empty;
					}
					else
					{
						component2.text = cUseable.GetSalableCount().ToString();
					}
				}
			}
			Transform transform2 = itemCell.transform.Find("imgSuipian");
			if (transform2 != null)
			{
				Image component3 = transform2.GetComponent<Image>();
				component3.gameObject.CustomSetActive(false);
				if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = cUseable as CItem;
					if (cItem.m_itemData.bClass == 2 || cItem.m_itemData.bClass == 3)
					{
						component3.gameObject.CustomSetActive(true);
					}
				}
			}
			Transform transform3 = itemCell.transform.Find("ItemName");
			if (transform3 != null)
			{
				Text component4 = transform3.gameObject.GetComponent<Text>();
				if (component4 != null)
				{
					component4.text = cUseable.m_name;
				}
			}
			Transform transform4 = itemCell.transform.Find("imgExperienceMark");
			if (transform4 != null)
			{
				if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(cUseable.m_baseID))
				{
					transform4.gameObject.CustomSetActive(true);
					base.StartCoroutine(this.DeleteAddImage(transform4.gameObject, CExperienceCardSystem.HeroExperienceCardMarkPath));
				}
				else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(cUseable.m_baseID))
				{
					transform4.gameObject.CustomSetActive(true);
					base.StartCoroutine(this.DeleteAddImage(transform4.gameObject, CExperienceCardSystem.SkinExperienceCardMarkPath));
				}
				else
				{
					transform4.gameObject.CustomSetActive(false);
				}
			}
			if (isHaveClickEvent)
			{
				CUIFormScript cUIFormScript = rootObj.GetComponent<CUIFormScript>();
				if (cUIFormScript == null)
				{
					cUIFormScript = rootObj.AddComponent<CUIFormScript>();
				}
				if (cUIFormScript != null)
				{
					CUIEventScript cUIEventScript = itemCell.GetComponent<CUIEventScript>();
					if (cUIEventScript == null)
					{
						cUIEventScript = itemCell.AddComponent<CUIEventScript>();
						cUIEventScript.Initialize(cUIFormScript);
					}
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.iconUseable = cUseable;
					cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
					cUIEventScript.SetUIEvent(enUIEventType.Up, enUIEventID.Tips_ItemInfoClose, eventParams);
				}
			}
			if (isClickToShowDetail)
			{
				CItem cItem2 = cUseable as CItem;
				if (cItem2 == null || cItem2.m_itemData.bIsView <= 0)
				{
					return;
				}
				CUIFormScript cUIFormScript2 = rootObj.GetComponent<CUIFormScript>();
				if (cUIFormScript2 == null)
				{
					cUIFormScript2 = rootObj.AddComponent<CUIFormScript>();
				}
				if (cUIFormScript2 != null)
				{
					CUIEventScript cUIEventScript2 = itemCell.GetComponent<CUIEventScript>();
					if (cUIEventScript2 == null)
					{
						cUIEventScript2 = itemCell.AddComponent<CUIEventScript>();
						cUIEventScript2.Initialize(cUIFormScript2);
					}
					cUIEventScript2.SetUIEvent(enUIEventType.Click, enUIEventID.GiftBag_OnShowDetail, new stUIEventParams
					{
						iconUseable = cUseable
					});
				}
			}
		}
	}
}

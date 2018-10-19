using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CLobbySystem : Singleton<CLobbySystem>
	{
		public enum LobbyFormWidget
		{
			None = -1,
			Reserve,
			RankingBtn,
			SnsHead,
			HeadImgBack,
			Rolling,
			LoudSpeakerRolling,
			LoudSpeakerRollingBg,
			PlatChannel,
			PlatChannelText,
			ImgZhiBo
		}

		public enum LobbyRankingBtnFormWidget
		{
			RankingBtnPanel
		}

		public enum enSysEntryFormWidget
		{
			WifiIcon,
			WifiInfo,
			WifiPing,
			GlodCoin,
			Dianquan,
			MailBtn,
			SettingBtn,
			Wifi_Bg,
			FriendBtn,
			MianLiuTxt
		}

		public static string LOBBY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby.prefab";

		public static string SYSENTRY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_SysTray.prefab";

		public static string RANKING_BTN_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_RankingBtn.prefab";

		public static string Pvp_BtnRes_PATH = CUIUtility.s_Sprite_System_Lobby_Dir + "PvpBtnDynamic.prefab";

		public static string LOBBY_FUN_UNLOCK_PATH = "UGUI/Form/System/Lobby/Form_FunUnLock.prefab";

		private CUIFormScript m_LobbyForm;

		private CUIFormScript m_SysEntryForm;

		private CUIFormScript m_RankingBtnForm;

		private Text m_PlayerName;

		private Text m_PvpLevel;

		private GameObject m_antiDisturbBits;

		private Image m_PvpExpImg;

		private Text m_PvpExpTxt;

		private RankingSystem.RankingSubView m_rankingType = RankingSystem.RankingSubView.Friend;

		private GameObject _rankingBtn;

		private GameObject hero_btn;

		private GameObject symbol_btn;

		private GameObject bag_btn;

		private GameObject task_btn;

		private GameObject social_btn;

		private GameObject addSkill_btn;

		private GameObject achievement_btn;

		public static bool AutoPopAllow = true;

		private static bool _autoPoped = false;

		private int myRankingNo = -1;

		private ListView<COMDT_FRIEND_INFO> rankFriendList;

		private GameObject m_QQbuluBtn;

		private bool m_bInLobby;

		public static uint s_CoinShowMaxValue = 990000u;

		public static uint s_CoinShowStepValue = 10000u;

		public SCPKG_REDDOTLIST_RSP m_serverRedDotTimeLineInfo = new SCPKG_REDDOTLIST_RSP();

		private Text m_lblGlodCoin;

		private Text m_lblDianquan;

		private Text m_lblDiamond;

		private GameObject m_wifiIcon;

		private GameObject m_wifiInfo;

		private GameObject m_textMianliu;

		private Text m_ping;

		private GameObject m_SysEntry;

		private DictionaryView<int, GameObject> m_Btns;

		public static string s_noNetStateName = "NoNet";

		public static string[] s_wifiStateName = new string[]
		{
			"Wifi_1",
			"Wifi_2",
			"Wifi_3"
		};

		public static string[] s_netStateName = new string[]
		{
			"Net_1",
			"Net_2",
			"Net_3"
		};

		public static Color[] s_WifiStateColor = new Color[]
		{
			Color.red,
			Color.yellow,
			Color.green
		};

		public int m_wifiIconCheckTicks = -1;

		public int m_wifiIconCheckMaxTicks = 6;

		public bool NeedRelogin;

		private Transform m_ShopTagIcon;

		private Transform m_ActivityIcon;

		public static string m_TagIConPath = "UGUI/Sprite/Dynamic/Flag/";

		public static bool IsPlatChannelOpen
		{
			get;
			set;
		}

		public bool IsInLobbyForm()
		{
			return this.m_bInLobby;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Sub, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Sub));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Origin, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Origin));
			Singleton<EventRouter>.instance.AddEventHandler("MasterAttributesChanged", new Action(this.UpdatePlayerData));
			Singleton<EventRouter>.instance.AddEventHandler("TaskUpdated", new Action(this.OnTaskUpdate));
			Singleton<EventRouter>.instance.AddEventHandler("Friend_LobbyIconRedDot_Refresh", new Action(this.OnFriendSysIconUpdate));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new Action(this.OnCheckRedDotByServerVersionWithLobby));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new Action(this.OnCheckDelMallEntryRedDot));
			Singleton<EventRouter>.instance.AddEventHandler("MailUnReadNumUpdate", new Action(this.OnMailUnReadUpdate));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnAchieveStateUpdate));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.SymbolEquipSuc, new Action(this.OnCheckSymbolEquipAlert));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this.OnBagItemsUpdate));
			Singleton<EventRouter>.instance.AddEventHandler("MasterPvpLevelChanged", new Action(this.OnCheckSymbolEquipAlert));
			Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
			Singleton<EventRouter>.instance.AddEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this.ValidateActivitySpot));
			Singleton<EventRouter>.instance.AddEventHandler("MasterJifenChanged", new Action(this.ValidateActivitySpot));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenQQRight, new CUIEventManager.OnUIEventHandler(this.OpenQQGameCenterRightForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_GotoBattleWebHome, new CUIEventManager.OnUIEventHandler(this.OnGotoBattleWebHome));
			Singleton<EventRouter>.GetInstance().AddEventHandler("CheckNewbieIntro", new Action(this.OnCheckNewbieIntro));
			Singleton<EventRouter>.GetInstance().AddEventHandler("VipInfoHadSet", new Action(this.UpdateQQVIPState));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.UpdateNobeIcon));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new Action(this.UpdateNobeHeadIdx));
			Singleton<EventRouter>.GetInstance().AddEventHandler("MasterPvpLevelChanged", new Action(this.OnPlayerLvlChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Rank_Friend_List", new Action(this.RefreshRankList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this.OnPlayerNameChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this.UpdatePlayerData));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.LOBBY_PURE_LOBBY_SHOW, new Action(this.OnPureLobbyShow));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this.UpdatePlayerData));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
			Singleton<EventRouter>.instance.RemoveEventHandler("MasterAttributesChanged", new Action(this.UpdatePlayerData));
			Singleton<EventRouter>.instance.RemoveEventHandler("TaskUpdated", new Action(this.OnTaskUpdate));
			Singleton<EventRouter>.instance.RemoveEventHandler("Friend_LobbyIconRedDot_Refresh", new Action(this.OnFriendSysIconUpdate));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new Action(this.OnCheckRedDotByServerVersionWithLobby));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new Action(this.OnCheckDelMallEntryRedDot));
			Singleton<EventRouter>.instance.RemoveEventHandler("MailUnReadNumUpdate", new Action(this.OnMailUnReadUpdate));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnAchieveStateUpdate));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.SymbolEquipSuc, new Action(this.OnCheckSymbolEquipAlert));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this.OnBagItemsUpdate));
			Singleton<EventRouter>.instance.RemoveEventHandler("MasterPvpLevelChanged", new Action(this.OnCheckSymbolEquipAlert));
			Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
			Singleton<EventRouter>.instance.RemoveEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this.ValidateActivitySpot));
			Singleton<EventRouter>.instance.RemoveEventHandler("MasterJifenChanged", new Action(this.ValidateActivitySpot));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_GotoBattleWebHome, new CUIEventManager.OnUIEventHandler(this.OnGotoBattleWebHome));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.UpdateNobeIcon));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new Action(this.UpdateNobeHeadIdx));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("MasterPvpLevelChanged", new Action(this.OnPlayerLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Rank_Friend_List", new Action(this.RefreshRankList));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this.UpdatePlayerData));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("CheckNewbieIntro", new Action(this.OnCheckNewbieIntro));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("VipInfoHadSet", new Action(this.UpdateQQVIPState));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.LOBBY_PURE_LOBBY_SHOW, new Action(this.OnPureLobbyShow));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this.UpdatePlayerData));
		}

		private void onOpenLobby(CUIEvent uiEvent)
		{
			this.m_LobbyForm = Singleton<CUIManager>.GetInstance().OpenForm(CLobbySystem.LOBBY_FORM_PATH, false, true);
			this.m_SysEntryForm = Singleton<CUIManager>.GetInstance().OpenForm(CLobbySystem.SYSENTRY_FORM_PATH, false, true);
			this.m_RankingBtnForm = Singleton<CUIManager>.GetInstance().OpenForm(CLobbySystem.RANKING_BTN_FORM_PATH, false, true);
			this.m_bInLobby = true;
			this.InitForm(this.m_LobbyForm);
			this.InitRankingBtnForm();
			this.InitOther(this.m_LobbyForm);
			this.InitSysEntryForm(this.m_SysEntryForm);
			this.InitTagIcon(this.m_LobbyForm);
			this.UpdatePlayerData();
			this.OnFriendSysIconUpdate();
			this.OnTaskUpdate();
			this.ValidateActivitySpot();
			this.OnMailUnReadUpdate();
			this.OnCheckSymbolEquipAlert();
			this.OnCheckUpdateClientVersion();
			CLobbySystem.SendRedDotTimeLineRsp();
			CLobbySystem.ShowWangZheCnt();
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
			Singleton<CMiShuSystem>.instance.CheckMiShuTalk(true);
			Singleton<CMiShuSystem>.instance.OnCheckFirstWin(null);
			Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
			Singleton<CUINewFlagSystem>.instance.SetNewFlagForLobbyAddedSkill(true);
			CPartnerSystem.RefreshSysEntryChargeRedDot();
			if (Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime > 0f)
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", "NULL"));
				list.Add(new KeyValuePair<string, string>("totaltime", (Time.time - Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime).ToString()));
				list.Add(new KeyValuePair<string, string>("errorCode", "0"));
				list.Add(new KeyValuePair<string, string>("error_msg", "0"));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_InLobby", list, true);
				Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime = 0f;
			}
			if (this.NeedRelogin)
			{
				this.NeedRelogin = false;
				LobbyMsgHandler.PopupRelogin();
			}
		}

		private void onOpenWeb(CUIEvent uiEvent)
		{
			string strUrl = "http://www.qq.com";
			CUICommonSystem.OpenUrl(strUrl, true, 0);
		}

		private void onCommon_WifiCheckTimer(CUIEvent uiEvent)
		{
			if (!this.m_bInLobby)
			{
				return;
			}
			this.CheckWifi();
			this.CheckMianLiu();
			CLobbySystem.CheckRedDotTimeLineShowState();
		}

		private void onCommon_ShowOrHideWifiInfo(CUIEvent uiEvent)
		{
			if (!this.m_bInLobby)
			{
				return;
			}
			this.ShowOrHideWifiInfo();
		}

		private void Lobby_LobbyFormShow(CUIEvent uiEvent)
		{
			if (!this.m_bInLobby)
			{
				return;
			}
			this.FullShow();
			CUICommonSystem.CloseCommonTips();
			CUICommonSystem.CloseUseableTips();
			Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
			Singleton<CChatController>.instance.ShowPanel(true, false);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_PrepareFight_Sub);
			Singleton<CFriendContoller>.instance.model.intimacyTipsMgr.CheckShouldShowTips(null);
			if (!MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
			{
				Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(false);
				Singleton<CAchievementSystem>.GetInstance().ProcessPvpBanMsg();
			}
			this.OnFriendSysIconUpdate();
			CPartnerSystem.RefreshSysEntryChargeRedDot();
		}

		private void Lobby_LobbyFormHide(CUIEvent uiEvent)
		{
			if (!this.m_bInLobby)
			{
				return;
			}
			this.MiniShow();
		}

		public void SetTopBarPriority(enFormPriority prioRity)
		{
			if (this.m_SysEntryForm == null)
			{
				return;
			}
			CUIFormScript component = this.m_SysEntryForm.GetComponent<CUIFormScript>();
			if (component != null)
			{
				component.SetPriority(prioRity);
			}
		}

		private void onErrorExit(CUIEvent uiEvent)
		{
			SGameApplication.Quit();
		}

		private void UpdatePlayerData()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null || this.m_LobbyForm == null)
			{
				return;
			}
			if (this.m_PlayerName != null)
			{
				this.m_PlayerName.text = masterRoleInfo.Name;
			}
			if (this.m_PvpExpImg != null)
			{
				this.m_PvpExpImg.CustomFillAmount(CPlayerProfile.Divide(masterRoleInfo.PvpExp, masterRoleInfo.PvpNeedExp));
				this.m_PvpExpTxt.text = masterRoleInfo.PvpExp + "/" + masterRoleInfo.PvpNeedExp;
			}
			if (this.m_PvpLevel != null)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel");
				if (!string.IsNullOrEmpty(text) && this.m_PvpLevel.text != null && masterRoleInfo != null)
				{
					this.m_PvpLevel.text = string.Format(text, masterRoleInfo.PvpLevel);
				}
			}
			if (this.m_antiDisturbBits != null)
			{
				this.m_antiDisturbBits.CustomSetActive((masterRoleInfo.OtherStatebBits & 1u) > 0u);
			}
			if (!CSysDynamicBlock.bSocialBlocked)
			{
				if (this.m_LobbyForm != null && this.m_LobbyForm.gameObject.activeSelf && masterRoleInfo != null)
				{
					GameObject widget = this.m_LobbyForm.GetWidget(2);
					if (widget != null && !string.IsNullOrEmpty(masterRoleInfo.HeadUrl))
					{
						CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
						component.SetImageUrl(masterRoleInfo.HeadUrl);
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false, true, masterRoleInfo.m_userPrivacyBits);
						Image component2 = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component2, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component2, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId, this.m_LobbyForm, 0.7f, false);
						bool flag = Singleton<HeadIconSys>.instance.UnReadFlagNum > 0u || masterRoleInfo.ShowGameRedDot;
						GameObject gameObject = Utility.FindChild(widget, "RedDot");
						if (gameObject != null)
						{
							if (flag)
							{
								CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
							}
							else
							{
								CUICommonSystem.DelRedDot(gameObject);
							}
						}
					}
				}
			}
			else if (this.m_LobbyForm != null && this.m_LobbyForm.gameObject.activeSelf)
			{
				GameObject widget2 = this.m_LobbyForm.GetWidget(2);
				if (widget2 != null)
				{
					CUIHttpImageScript component3 = widget2.GetComponent<CUIHttpImageScript>();
					if (component3)
					{
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component3.GetComponent<Image>(), 0, false, true, 0uL);
					}
				}
			}
			if (this.m_lblGlodCoin != null)
			{
				this.m_lblGlodCoin.text = this.GetCoinString(masterRoleInfo.GoldCoin);
			}
			if (this.m_lblDianquan != null)
			{
				this.m_lblDianquan.text = this.GetCoinString((uint)masterRoleInfo.DianQuan);
			}
			if (this.m_lblDiamond != null)
			{
				this.m_lblDiamond.text = this.GetCoinString(masterRoleInfo.Diamond);
			}
		}

		public void ShowHideRankingBtn(bool show)
		{
			if (this._rankingBtn != null)
			{
				if (CSysDynamicBlock.bSocialBlocked)
				{
					this._rankingBtn.CustomSetActive(false);
				}
				else
				{
					this._rankingBtn.CustomSetActive(show);
				}
			}
		}

        public void Play_UnLock_Animation(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            string str = string.Empty;
            switch (type)
            {
                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION:
                    str = "SocialBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK:
                    str = "TaskBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO:
                    str = "HeroBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG:
                    str = "BagBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND:
                    str = "FriendBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL:
                    str = "AddedSkillBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL:
                    str = "SymbolBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT:
                    str = "AchievementBtn";
                    break;
            }
            if (!string.IsNullOrEmpty(str))
            {
                stUIEventParams eventParams = new stUIEventParams
                {
                    tag = (int)type
                };
                CUIAnimatorScript component = Singleton<CUIManager>.instance.OpenForm(LOBBY_FUN_UNLOCK_PATH, false, true).GetComponent<CUIAnimatorScript>();
                component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.Lobby_UnlockAnimation_End, eventParams);
                component.PlayAnimator(str);
                Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_unlock", null);
            }
        }


		public void Clear()
		{
			this.m_rankingType = RankingSystem.RankingSubView.Friend;
		}

		private void UnInitWidget()
		{
			this._rankingBtn = null;
		}

		private void RefreshRankList(RankingSystem.RankingSubView rankingType)
		{
			this.m_rankingType = rankingType;
			this.RefreshRankList();
		}

		private void RefreshRankList()
		{
			if (this._rankingBtn == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this._rankingBtn, "RankingList");
			if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
			{
				this.myRankingNo = Singleton<RankingSystem>.instance.GetMyFriendRankNo();
				if (this.myRankingNo == -1)
				{
					return;
				}
				this.rankFriendList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT);
				if (this.rankFriendList == null)
				{
					return;
				}
				int num = this.rankFriendList.Count + 1;
				componetInChild.SetElementAmount(num);
				for (int i = 0; i < num; i++)
				{
					CUIListElementScript elemenet = componetInChild.GetElemenet(i);
					if (!(elemenet == null))
					{
						GameObject gameObject = elemenet.gameObject;
						if (!(gameObject == null))
						{
							this.OnUpdateRankingFriendElement(gameObject, i);
						}
					}
				}
			}
			else if (this.m_rankingType == RankingSystem.RankingSubView.All)
			{
				CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
				if (rankList != null)
				{
					int num = (int)rankList.dwItemNum;
					componetInChild.SetElementAmount(num);
					for (int j = 0; j < num; j++)
					{
						CUIListElementScript elemenet2 = componetInChild.GetElemenet(j);
						if (!(elemenet2 == null))
						{
							GameObject gameObject2 = elemenet2.gameObject;
							if (!(gameObject2 == null))
							{
								this.OnUpdateRankingAllElement(gameObject2, j);
							}
						}
					}
				}
			}
		}

		private void OnRankingListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
			{
				this.OnUpdateRankingFriendElement(srcWidget, srcWidgetIndexInBelongedList);
			}
			else if (this.m_rankingType == RankingSystem.RankingSubView.All)
			{
				this.OnUpdateRankingAllElement(srcWidget, srcWidgetIndexInBelongedList);
			}
		}

		private void OnUpdateRankingFriendElement(GameObject go, int index)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			string serverUrl = string.Empty;
			int num = (index <= this.myRankingNo) ? 0 : -1;
			Transform transform = go.transform;
			GameObject gameObject = transform.Find("HeadIcon").gameObject;
			GameObject gameObject2 = transform.transform.Find("HeadbgNo1").gameObject;
			GameObject gameObject3 = transform.transform.Find("123No").gameObject;
			int headIdx = 0;
			if (index == this.myRankingNo)
			{
				if (masterRoleInfo != null)
				{
					serverUrl = masterRoleInfo.HeadUrl;
					headIdx = (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId;
					GameObject gameObject4 = transform.transform.Find("QQVipIcon").gameObject;
					this.SetQQVip(gameObject4, true, 0, 0uL);
				}
			}
			else if (index + num < this.rankFriendList.Count)
			{
				serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref this.rankFriendList[index + num].szHeadUrl);
				headIdx = (int)this.rankFriendList[index + num].stGameVip.dwHeadIconId;
				GameObject gameObject5 = transform.transform.Find("QQVipIcon").gameObject;
				this.SetQQVip(gameObject5, false, (int)this.rankFriendList[index + num].dwQQVIPMask, this.rankFriendList[index + num].ullUserPrivacyBits);
			}
			gameObject2.CustomSetActive(index == 0);
			gameObject3.transform.GetChild(0).gameObject.CustomSetActive(0 == index);
			gameObject3.transform.GetChild(1).gameObject.CustomSetActive(1 == index);
			gameObject3.transform.GetChild(2).gameObject.CustomSetActive(2 == index);
			Image component = transform.transform.Find("NobeImag").GetComponent<Image>();
			if (component)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, headIdx);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component, headIdx, this.m_RankingBtnForm, 1f, true);
			}
			RankingView.SetUrlHeadIcon(gameObject, serverUrl);
		}

		private void OnUpdateRankingAllElement(GameObject go, int index)
		{
			CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
			if (rankList != null && go != null && index < rankList.astItemDetail.Length)
			{
				string serverUrl = string.Empty;
				Transform transform = go.transform.Find("HeadIcon");
				Transform transform2 = go.transform.Find("HeadbgNo1");
				Transform transform3 = go.transform.Find("123No");
				if (transform != null && transform2 != null && transform3 != null)
				{
					serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.szHeadUrl);
					transform2.gameObject.CustomSetActive(index == 0);
					RankingView.SetUrlHeadIcon(transform.gameObject, serverUrl);
					transform3.GetChild(0).gameObject.CustomSetActive(0 == index);
					transform3.GetChild(1).gameObject.CustomSetActive(1 == index);
					transform3.GetChild(2).gameObject.CustomSetActive(2 == index);
				}
				int dwHeadIconId = (int)rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.stGameVip.dwHeadIconId;
				Image componetInChild = Utility.GetComponetInChild<Image>(go, "NobeImag");
				if (componetInChild)
				{
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(componetInChild, dwHeadIconId);
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(componetInChild, dwHeadIconId, this.m_RankingBtnForm, 1f, true);
				}
				Transform transform4 = go.transform.Find("QQVipIcon");
				if (transform4)
				{
					this.SetQQVip(transform4.gameObject, false, (int)rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.dwVipLevel, rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.ullUserPrivacyBits);
				}
			}
		}

		private void OnPlayerNameChange()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (this.m_PlayerName != null && masterRoleInfo != null)
			{
				this.m_PlayerName.text = masterRoleInfo.Name;
			}
		}

		private void SetQQVip(GameObject QQVipIcon, bool bSelf, int mask = 0, ulong privacyMask = 0uL)
		{
			if (QQVipIcon == null)
			{
				return;
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					QQVipIcon.CustomSetActive(false);
					return;
				}
				if (bSelf)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(QQVipIcon.GetComponent<Image>());
					}
				}
				else
				{
					MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(QQVipIcon.GetComponent<Image>(), mask);
				}
			}
			else
			{
				QQVipIcon.CustomSetActive(false);
			}
		}

		private void ProcessQQVIP(CUIFormScript form)
		{
			if (null == form)
			{
				return;
			}
			Transform transform = form.transform.Find("VIPGroup/QQVIpBtn");
			GameObject obj = null;
			if (transform)
			{
				obj = transform.gameObject;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/QQVipIcon");
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					obj.CustomSetActive(false);
					gameObject.CustomSetActive(false);
					return;
				}
				obj.CustomSetActive(true);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(gameObject.GetComponent<Image>());
				}
			}
			else
			{
				obj.CustomSetActive(false);
				gameObject.CustomSetActive(false);
			}
		}

		private void UpdateQQVIPState()
		{
			if (!this.m_bInLobby || this.m_LobbyForm == null)
			{
				return;
			}
			Transform transform = this.m_LobbyForm.transform;
			Transform transform2 = transform.Find("VIPGroup/QQVIpBtn");
			GameObject obj = null;
			if (transform2)
			{
				obj = transform2.gameObject;
			}
			Transform transform3 = transform.Find("PlayerHead/NameGroup/QQVipIcon");
			GameObject obj2 = null;
			GameObject obj3 = null;
			if (transform3 != null)
			{
				obj2 = transform3.gameObject;
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					obj.CustomSetActive(false);
					obj2.CustomSetActive(false);
					obj3.CustomSetActive(false);
					return;
				}
				obj.CustomSetActive(true);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					if (masterRoleInfo.HasVip(16))
					{
						obj2.CustomSetActive(false);
						obj3.CustomSetActive(true);
					}
					else if (masterRoleInfo.HasVip(1))
					{
						obj2.CustomSetActive(true);
						obj3.CustomSetActive(false);
					}
					else
					{
						obj2.CustomSetActive(false);
						obj3.CustomSetActive(false);
					}
				}
			}
			else
			{
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				obj3.CustomSetActive(false);
			}
		}

		private void OpenQQVIPWealForm(CUIEvent uiEvent)
		{
			string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_QQVipPrivilege.prefab");
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
			if (cUIFormScript != null)
			{
				Singleton<QQVipWidget>.instance.SetData(cUIFormScript.gameObject, cUIFormScript);
			}
		}

		private void ShowPlatformRight()
		{
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Guest)
			{
				string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_GuestGameCenter.prefab");
				Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
			}
			else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				string formPath2 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_QQGameCenter.prefab");
				Singleton<CUIManager>.GetInstance().OpenForm(formPath2, false, true);
			}
			else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
			{
				string formPath3 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_WXGameCenter.prefab");
				Singleton<CUIManager>.GetInstance().OpenForm(formPath3, false, true);
			}
		}

		private void OpenGuestGameCenterRightForm(CUIEvent uiEvent)
		{
			this.ShowPlatformRight();
		}

		private void OpenWXGameCenterRightForm(CUIEvent uiEvent)
		{
			this.ShowPlatformRight();
		}

		private void OpenQQGameCenterRightForm(CUIEvent uiEvent)
		{
			this.ShowPlatformRight();
		}

		private void OpenWebHome(CUIEvent uiEvent)
		{
			ulong num = 0uL;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				num = masterRoleInfo.playerUllUID;
			}
			string platformArea = CUICommonSystem.GetPlatformArea();
			string strUrl = string.Concat(new object[]
			{
				"http://yxzj.qq.com/ingame/all/index.shtml?partition=",
				MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID,
				"&roleid=",
				num,
				"&area=",
				platformArea
			});
			CUICommonSystem.OpenUrl(strUrl, true, 0);
			if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_GongLueEntry))
			{
				CUIRedDotSystem.SetRedDotViewByVersion(enRedID.Lobby_GongLueEntry);
				if (this.m_LobbyForm != null)
				{
					Transform transform = this.m_LobbyForm.transform.Find("Popup/SignBtn");
					if (transform != null)
					{
						CUIRedDotSystem.DelRedDot(transform.gameObject);
					}
				}
			}
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenGongLue);
		}

		private void OnGotoBattleWebHome(CUIEvent uiEvent)
		{
			if (!MonoSingleton<TGASys>.GetInstance().Start())
			{
				ulong num = 0uL;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					num = masterRoleInfo.playerUllUID;
				}
				string platformArea = CUICommonSystem.GetPlatformArea();
				string text = Singleton<CTextManager>.instance.GetText("HttpUrl_BattleWebHome");
				text = string.Concat(new object[]
				{
					text,
					"?partition=",
					MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID,
					"&roleid=",
					num,
					"&area=",
					platformArea
				});
				CUICommonSystem.OpenUrl(text, true, 0);
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenBattleWebHome);
			}
		}

		protected void OnCloseForm(CUIEvent uiEvt)
		{
			this.m_bInLobby = false;
			this.ClearTagIcon();
			this.m_LobbyForm = null;
			this.m_SysEntryForm = null;
			this.m_RankingBtnForm = null;
			this.m_PlayerName = null;
			this.m_PvpLevel = null;
			this.m_antiDisturbBits = null;
			this.m_PvpExpImg = null;
			this.m_PvpExpTxt = null;
			this.hero_btn = null;
			this.symbol_btn = null;
			this.bag_btn = null;
			this.task_btn = null;
			this.social_btn = null;
			this.addSkill_btn = null;
			this.achievement_btn = null;
			Singleton<CUIManager>.GetInstance().CloseForm(CLobbySystem.SYSENTRY_FORM_PATH);
			Singleton<CUIManager>.GetInstance().CloseForm(CLobbySystem.RANKING_BTN_FORM_PATH);
			this.UnInitWidget();
		}

		protected void OnPrepareFight_Sub(CUIEvent uiEvt)
		{
			Transform transform = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn_sub");
			Transform transform2 = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(false);
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(true);
			}
		}

		protected void OnPrepareFight_Origin(CUIEvent uiEvt)
		{
			Transform transform = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn_sub");
			Transform transform2 = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(true);
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
			}
		}

		private void InitForm(CUIFormScript form)
		{
			Transform transform = form.transform;
			this.m_PlayerName = transform.Find("PlayerHead/NameGroup/PlayerName").GetComponent<Text>();
			this.m_PvpLevel = transform.Find("PlayerHead/pvpLevel").GetComponent<Text>();
			this.m_antiDisturbBits = transform.Find("PlayerHead/HttpImage/AntiDisturbBits").gameObject;
			this.m_PvpExpImg = transform.Find("PlayerHead/pvpExp/expBg/imgExp").GetComponent<Image>();
			this.m_PvpExpTxt = transform.Find("PlayerHead/pvpExp/expBg/txtExp").GetComponent<Text>();
			this.hero_btn = transform.Find("LobbyBottom/SysEntry/HeroBtn").gameObject;
			this.symbol_btn = transform.Find("LobbyBottom/SysEntry/SymbolBtn").gameObject;
			this.bag_btn = transform.Find("LobbyBottom/SysEntry/BagBtn").gameObject;
			this.task_btn = transform.Find("LobbyBottom/Newbie").gameObject;
			this.social_btn = transform.Find("LobbyBottom/SysEntry/SocialBtn").gameObject;
			this.addSkill_btn = transform.Find("LobbyBottom/SysEntry/AddedSkillBtn").gameObject;
			this.achievement_btn = transform.Find("LobbyBottom/SysEntry/AchievementBtn").gameObject;
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL);
			this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT);
			Transform transform2 = transform.Find("BtnCon/PvpBtn");
			Transform transform3 = transform.Find("BtnCon/LadderBtn");
			if (transform2)
			{
				CUICommonSystem.LoadUIPrefab(CLobbySystem.Pvp_BtnRes_PATH, "PvpBtnDynamic", transform2.gameObject, form);
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform4 = transform.Find("Popup");
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(false);
				}
				Transform transform5 = transform.Find("BtnCon/CompetitionBtn");
				if (transform5)
				{
					transform5.gameObject.CustomSetActive(false);
				}
				if (this.task_btn)
				{
					this.task_btn.CustomSetActive(false);
				}
				Transform transform6 = transform.Find("DiamondPayBtn");
				if (transform6)
				{
					transform6.gameObject.CustomSetActive(false);
				}
				Transform transform7 = transform.Find("Popup/BattleWebHome");
				if (transform7)
				{
					transform7.gameObject.CustomSetActive(false);
				}
			}
			Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
			if (component)
			{
				component.interactable = Singleton<CLadderSystem>.GetInstance().IsLevelQualified();
				Transform transform8 = component.transform.Find("Lock");
				if (transform8)
				{
					transform8.gameObject.CustomSetActive(!component.interactable);
					transform8.SetAsLastSibling();
				}
			}
			Button component2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
			if (component2)
			{
				bool flag = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
				component2.interactable = !flag;
				GameObject gameObject = component2.transform.FindChild("Lock").gameObject;
				gameObject.CustomSetActive(flag);
			}
			GameObject gameObject2 = transform.Find("PlayerHead/pvpExp/expEventPanel").gameObject;
			if (gameObject2 != null)
			{
				CUIEventScript cUIEventScript = gameObject2.GetComponent<CUIEventScript>();
				if (cUIEventScript == null)
				{
					cUIEventScript = gameObject2.AddComponent<CUIEventScript>();
					cUIEventScript.Initialize(form);
				}
				CUseable iconUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, 0);
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.iconUseable = iconUseable;
				eventParams.tag = 3;
				cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			}
			CLobbySystem.RefreshDianQuanPayButton(false);
			GameObject widget = form.GetWidget(7);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				widget.CustomSetActive(false);
			}
			else
			{
				Text component3 = form.GetWidget(8).GetComponent<Text>();
				if (GameDataMgr.globalInfoDatabin.GetDataByKey(204u).dwConfValue > 0u)
				{
					widget.CustomSetActive(CLobbySystem.IsPlatChannelOpen);
					component3.text = Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Open_Lobby_Msg");
				}
				else
				{
					widget.CustomSetActive(!CLobbySystem.IsPlatChannelOpen);
					component3.text = Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Not_Open_Lobby_Msg");
				}
			}
		}

		private void InitRankingBtnForm()
		{
			if (this.m_RankingBtnForm == null)
			{
				DebugHelper.Assert(false, "m_RankingBtnForm cannot be null!!!");
				return;
			}
			this._rankingBtn = this.m_RankingBtnForm.GetWidget(0);
			if (this._rankingBtn && CSysDynamicBlock.bSocialBlocked)
			{
				this._rankingBtn.CustomSetActive(false);
			}
			this.RefreshRankList();
		}

		private void InitOther(CUIFormScript m_FormScript)
		{
			Singleton<CTimerManager>.GetInstance().AddTimer(50, 1, new CTimer.OnTimeUpHandler(this.CheckNewbieIntro));
			this.ProcessQQVIP(m_FormScript);
			this.UpdateGameCenterState(m_FormScript);
			MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeTipsInfo();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.m_licenseInfo != null)
			{
				masterRoleInfo.m_licenseInfo.ReviewLicenseList();
			}
		}

		private void InitSysEntryForm(CUIFormScript form)
		{
			Transform transform = form.gameObject.transform;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			this.m_lblGlodCoin = transform.FindChild("PlayerBtn/GoldCoin/Text").GetComponent<Text>();
			this.m_lblDianquan = transform.FindChild("PlayerBtn/Dianquan/Text").GetComponent<Text>();
			this.m_lblDiamond = transform.FindChild("PlayerBtn/Diamond/Text").GetComponent<Text>();
			this.m_wifiIcon = form.GetWidget(0);
			this.m_wifiInfo = form.GetWidget(1);
			this.m_ping = form.GetWidget(2).GetComponent<Text>();
			this.m_textMianliu = form.GetWidget(9);
			this.m_lblGlodCoin.text = this.GetCoinString(masterRoleInfo.GoldCoin);
			this.m_lblDianquan.text = this.GetCoinString((uint)masterRoleInfo.DianQuan);
			this.m_lblDiamond.text = this.GetCoinString(masterRoleInfo.Diamond);
			GameObject gameObject = transform.Find("PlayerBtn/GoldCoin").gameObject;
			if (gameObject != null)
			{
				CUIEventScript cUIEventScript = gameObject.GetComponent<CUIEventScript>();
				if (cUIEventScript == null)
				{
					cUIEventScript = gameObject.AddComponent<CUIEventScript>();
					cUIEventScript.Initialize(form);
				}
				CUseable iconUseable = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN, (int)masterRoleInfo.GoldCoin);
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.iconUseable = iconUseable;
				eventParams.tag = 3;
				cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			}
			GameObject gameObject2 = transform.Find("PlayerBtn/Diamond").gameObject;
			if (gameObject2 != null)
			{
				CUIEventScript cUIEventScript2 = gameObject2.GetComponent<CUIEventScript>();
				if (cUIEventScript2 == null)
				{
					cUIEventScript2 = gameObject2.AddComponent<CUIEventScript>();
					cUIEventScript2.Initialize(form);
				}
				CUseable iconUseable2 = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND, (int)masterRoleInfo.Diamond);
				stUIEventParams eventParams2 = default(stUIEventParams);
				eventParams2.iconUseable = iconUseable2;
				eventParams2.tag = 3;
				cUIEventScript2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams2);
				cUIEventScript2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams2);
				cUIEventScript2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams2);
				cUIEventScript2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams2);
			}
			if (!ApolloConfig.payEnabled)
			{
				Transform transform2 = transform.Find("PlayerBtn/Dianquan/Button");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
			this.m_SysEntry = this.m_LobbyForm.gameObject.transform.Find("LobbyBottom/SysEntry").gameObject;
			this.m_Btns = new DictionaryView<int, GameObject>();
			this.m_Btns.Add(0, this.m_SysEntry.transform.Find("HeroBtn").gameObject);
			this.m_Btns.Add(1, this.m_SysEntry.transform.Find("SymbolBtn").gameObject);
			this.m_Btns.Add(2, this.m_SysEntry.transform.Find("AchievementBtn").gameObject);
			this.m_Btns.Add(3, this.m_SysEntry.transform.Find("BagBtn").gameObject);
			this.m_Btns.Add(5, this.m_SysEntry.transform.Find("SocialBtn").gameObject);
			this.m_Btns.Add(6, form.transform.Find("PlayerBtn/FriendBtn").gameObject);
			this.m_Btns.Add(7, this.m_SysEntry.transform.Find("AddedSkillBtn").gameObject);
			this.m_Btns.Add(8, form.transform.Find("PlayerBtn/MailBtn").gameObject);
			this.m_Btns.Add(9, Utility.FindChild(this.m_LobbyForm.gameObject, "Popup/ActBtn"));
			this.m_Btns.Add(10, Utility.FindChild(this.m_LobbyForm.gameObject, "Popup/BoardBtn"));
			this.m_Btns.Add(4, this.m_LobbyForm.gameObject.transform.Find("LobbyBottom/Newbie/RedDotPanel").gameObject);
		}

		public static void RefreshDianQuanPayButton(bool notifyFromSvr = false)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form != null)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				GameObject gameObject = form.transform.Find("DiamondPayBtn").gameObject;
				CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
				CTextManager instance = Singleton<CTextManager>.GetInstance();
				if (!masterRoleInfo.IsGuidedStateSet(22))
				{
					CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_FirstPay"));
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
					CUICommonSystem.DelRedDot(gameObject);
				}
				else if (!masterRoleInfo.IsGuidedStateSet(23))
				{
					CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_FirstPay"));
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else if (!masterRoleInfo.IsGuidedStateSet(24))
				{
					CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_Renewal"));
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
					CUICommonSystem.DelRedDot(gameObject);
				}
				else if (!masterRoleInfo.IsGuidedStateSet(25))
				{
					CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_Renewal"));
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else if (masterRoleInfo.IsClientBitsSet(0))
				{
					CUICommonSystem.SetButtonName(gameObject, instance.GetText("GotoTehuiShopName"));
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_TehuiShop);
				}
				else if (notifyFromSvr)
				{
					masterRoleInfo.SetClientBits(0, true, false);
					CLobbySystem.RefreshDianQuanPayButton(false);
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
		}

		private void UpdateNobeIcon()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (this.m_LobbyForm != null && this.m_LobbyForm.gameObject.activeSelf && masterRoleInfo != null)
			{
				GameObject widget = this.m_LobbyForm.GetWidget(2);
				if (widget != null)
				{
					CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false, true, masterRoleInfo.m_userPrivacyBits);
					Image component2 = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component2, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component2, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId, this.m_LobbyForm, 0.7f, false);
				}
			}
		}

		private void UpdateNobeHeadIdx()
		{
			int dwHeadIconId = (int)MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (this.m_LobbyForm != null && this.m_LobbyForm.gameObject.activeSelf && masterRoleInfo != null)
			{
				Image component = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
				if (component != null)
				{
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, dwHeadIconId);
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component, dwHeadIconId, this.m_LobbyForm, 0.7f, false);
				}
				this.RefreshRankList();
			}
		}

		private void OnCheckNewbieIntro()
		{
			Singleton<CTimerManager>.GetInstance().AddTimer(100, 1, delegate(int seq)
			{
				this.PopupNewbieIntro();
			});
		}

		private void CheckNewbieIntro(int timerSeq)
		{
			if (!this.PopupNewbieIntro() && !CLobbySystem._autoPoped)
			{
				CLobbySystem._autoPoped = true;
			}
		}

		private void OnPlayerLvlChange()
		{
			if (this.m_LobbyForm)
			{
				Transform transform = this.m_LobbyForm.transform;
				Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
				if (component)
				{
					component.interactable = Singleton<CLadderSystem>.GetInstance().IsLevelQualified();
					Transform transform2 = component.transform.Find("Lock");
					if (transform2)
					{
						transform2.gameObject.CustomSetActive(!component.interactable);
						transform2.SetAsLastSibling();
					}
				}
				Button component2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
				if (component2)
				{
					bool flag = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
					component2.interactable = !flag;
					GameObject gameObject = component2.transform.FindChild("Lock").gameObject;
					gameObject.CustomSetActive(flag);
				}
			}
		}

		private void StartAutoPopupChain(int timerSeq)
		{
			CLobbySystem.AutoPopAllow &= !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding;
			if (CLobbySystem.AutoPopAllow)
			{
				this.AutoPopup1_IDIP();
			}
		}

		private bool PopupNewbieIntro()
		{
			if (CSysDynamicBlock.bNewbieBlocked)
			{
				return true;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "Master Role info is NULL!");
			if (masterRoleInfo != null && !masterRoleInfo.IsNewbieAchieveSet(84) && Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab") == null)
			{
				masterRoleInfo.SetNewbieAchieve(84, true, true);
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Wealfare_CloseForm);
				return true;
			}
			return false;
		}

		private void AutoPopup1_IDIP()
		{
			if (MonoSingleton<IDIPSys>.GetInstance().RedPotState)
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.IDIP_OpenForm);
			}
			else
			{
				this.AutoPopup2_Activity();
			}
		}

		private void OnIDIPClose(CUIEvent uiEvt)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
			this.AutoPopup2_Activity();
		}

		private void AutoPopup2_Activity()
		{
			if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_OpenForm);
			}
		}

		private void OnActivityClose(CUIEvent uiEvt)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
		}

		public void Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			bool flag = Singleton<CFunctionUnlockSys>.instance.TipsHasShow(type);
			if (!flag && !Singleton<CFunctionUnlockSys>.instance.IsTypeHasCondition(type))
			{
				flag = true;
			}
			this.SetEnable(type, flag);
		}

		private void SetEnable(RES_SPECIALFUNCUNLOCK_TYPE type, bool bShow)
		{
			GameObject gameObject;
			if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO)
			{
				gameObject = this.hero_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL)
			{
				gameObject = this.symbol_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG)
			{
				gameObject = this.bag_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK)
			{
				gameObject = this.task_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION)
			{
				gameObject = this.social_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL)
			{
				gameObject = this.addSkill_btn;
			}
			else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT)
			{
				gameObject = this.achievement_btn;
			}
			else
			{
				gameObject = null;
			}
			if (gameObject == null)
			{
				return;
			}
			gameObject.CustomSetActive(bShow);
		}

		private void On_Lobby_UnlockAnimation_End(CUIEvent uievent)
		{
			Singleton<CUIManager>.instance.CloseForm(CLobbySystem.LOBBY_FUN_UNLOCK_PATH);
			Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_back", null);
			this.SetEnable((RES_SPECIALFUNCUNLOCK_TYPE)uievent.m_eventParams.tag, true);
		}

		private void On_Lobby_MysteryShopClose(CUIEvent uiEvent)
		{
			GameObject gameObject = Utility.FindChild(uiEvent.m_srcFormScript.gameObject, "Popup/BoardBtn/MysteryShop");
			Debug.LogWarning(string.Format("mystery shop icon on close:{0}", gameObject));
			gameObject.CustomSetActive(false);
		}

		private void UpdateGameCenterState(CUIFormScript form)
		{
			if (null == form)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			COM_PRIVILEGE_TYPE privilegeType = (masterRoleInfo != null) ? masterRoleInfo.m_privilegeType : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			GameObject obj = Utility.FindChild(form.gameObject, "VIPGroup/WXGameCenterBtn");
			GameObject obj2 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/WXGameCenterIcon");
			GameObject gameObject = Utility.FindChild(form.gameObject, "VIPGroup/BuLuoBtn");
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj, privilegeType, ApolloPlatform.Wechat, true, true, "特权", "福利");
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj2, privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(gameObject, privilegeType, ApolloPlatform.Wechat, true, false, string.Empty, string.Empty);
			if (gameObject.activeSelf && CSysDynamicBlock.bLobbyEntryBlocked)
			{
				gameObject.CustomSetActive(false);
			}
			GameObject obj3 = Utility.FindChild(form.gameObject, "VIPGroup/QQGameCenterBtn");
			GameObject obj4 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/QQGameCenterIcon");
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj3, privilegeType, ApolloPlatform.QQ, true, true, "特权", "福利");
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj4, privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			GameObject obj5 = Utility.FindChild(form.gameObject, "VIPGroup/GuestGameCenterBtn");
			GameObject obj6 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/GuestGameCenterIcon");
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj5, privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj6, privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
		}

		public void CheckWifi()
		{
			if (this.m_wifiIcon == null || this.m_wifiInfo == null || this.m_ping == null)
			{
				return;
			}
			int num = (int)Singleton<NetworkModule>.GetInstance().lobbyPing;
			num = ((num <= 100) ? num : ((num - 100) * 7 / 10 + 100));
			num = Mathf.Clamp(num, 0, 460);
			uint num2;
			if (num < 100)
			{
				num2 = 2u;
			}
			else if (num < 200)
			{
				num2 = 1u;
			}
			else
			{
				num2 = 0u;
			}
			if (this.m_wifiIconCheckTicks == -1 || this.m_wifiIconCheckTicks >= this.m_wifiIconCheckMaxTicks)
			{
				enNetWorkType netWorkType = CUICommonSystem.GetNetWorkType();
				if (netWorkType == enNetWorkType.enNone)
				{
					CUICommonSystem.PlayAnimator(this.m_wifiIcon, CLobbySystem.s_noNetStateName);
				}
				else if (netWorkType == enNetWorkType.enWifi)
				{
					CUICommonSystem.PlayAnimator(this.m_wifiIcon, CLobbySystem.s_wifiStateName[(int)((UIntPtr)num2)]);
				}
				else if (netWorkType == enNetWorkType.enNet)
				{
					CUICommonSystem.PlayAnimator(this.m_wifiIcon, CLobbySystem.s_netStateName[(int)((UIntPtr)num2)]);
				}
				this.m_wifiIconCheckTicks = 0;
			}
			else
			{
				this.m_wifiIconCheckTicks++;
			}
			if (this.m_wifiInfo && this.m_wifiInfo.activeInHierarchy)
			{
				this.m_ping.text = num + "ms";
			}
		}

		public void CheckMianLiu()
		{
			if (this.m_textMianliu != null)
			{
				if (MonoSingleton<CTongCaiSys>.instance.IsUsingTongcaiIp() && MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
				{
					this.m_textMianliu.CustomSetActive(true);
				}
				else
				{
					this.m_textMianliu.CustomSetActive(false);
				}
			}
		}

		public void ShowOrHideWifiInfo()
		{
			if (this.m_wifiInfo != null)
			{
				this.m_wifiInfo.CustomSetActive(!this.m_wifiInfo.activeInHierarchy);
			}
			this.CheckWifi();
		}

		public string GetCoinString(uint coinValue)
		{
			string result = coinValue.ToString();
			if (coinValue > CLobbySystem.s_CoinShowMaxValue)
			{
				int num = (int)(coinValue / CLobbySystem.s_CoinShowStepValue);
				result = string.Format("{0}万", num);
			}
			return result;
		}

		public void FullShow()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform;
			transform.Find("PlayerBtn/MailBtn").gameObject.CustomSetActive(true);
			transform.Find("PlayerBtn/SettingBtn").gameObject.CustomSetActive(true);
			transform.Find("PlayerBtn/FriendBtn").gameObject.CustomSetActive(true);
			CUICommonSystem.PlayLobbySysEntryFormFadeIn();
		}

		public void MiniShow()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform;
			transform.Find("PlayerBtn/MailBtn").gameObject.CustomSetActive(false);
			transform.Find("PlayerBtn/SettingBtn").gameObject.CustomSetActive(false);
			transform.Find("PlayerBtn/FriendBtn").gameObject.CustomSetActive(false);
			CUICommonSystem.PlayLobbySysEntryFormFadeIn();
		}

		public void AddRedDot(enSysEntryID sysEntryId, enRedDotPos redDotPos = enRedDotPos.enTopRight, int count = 0)
		{
			if (this.m_Btns != null)
			{
				GameObject target;
				this.m_Btns.TryGetValue((int)sysEntryId, out target);
				CUICommonSystem.AddRedDot(target, redDotPos, count, 0, 0);
			}
		}

		public void AddRedDotEx(enSysEntryID sysEntryId, enRedDotPos redDotPos = enRedDotPos.enTopRight, int alertNum = 0)
		{
			if (this.m_Btns != null)
			{
				GameObject target;
				this.m_Btns.TryGetValue((int)sysEntryId, out target);
				CUICommonSystem.AddRedDot(target, redDotPos, alertNum, 0, 0);
			}
		}

		public void DelRedDot(enSysEntryID sysEntryId)
		{
			if (this.m_Btns != null)
			{
				GameObject target;
				this.m_Btns.TryGetValue((int)sysEntryId, out target);
				CUICommonSystem.DelRedDot(target);
			}
		}

		private bool checkIsHaveRedDot()
		{
			bool result = false;
			DictionaryView<int, GameObject>.Enumerator enumerator = this.m_Btns.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, GameObject> current = enumerator.Current;
				if (CUICommonSystem.IsHaveRedDot(current.Value))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnTaskUpdate()
		{
			CTaskModel model = Singleton<CTaskSys>.instance.model;
			model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_HEROWAKE);
			model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_USUAL);
			model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_MAIN);
			int num = Singleton<CTaskSys>.instance.model.GetMainTask_RedDotCount();
			num += Singleton<CTaskSys>.instance.model.task_Data.GetTask_Count(enTaskTab.TAB_USUAL, CTask.State.Have_Done);
			if (num > 0)
			{
				this.AddRedDot(enSysEntryID.TaskBtn, enRedDotPos.enTopRight, num);
			}
			else
			{
				this.DelRedDot(enSysEntryID.TaskBtn);
			}
		}

		private void OnFriendSysIconUpdate()
		{
			int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.RequestFriend);
			int dataCount2 = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.MentorRequestList);
			bool flag = Singleton<CFriendContoller>.GetInstance().model.FRData.HasRedDot();
			bool flag2 = Singleton<CTaskSys>.instance.IsMentorTaskRedDot();
			if (dataCount > 0 || dataCount2 > 0 || flag || flag2)
			{
				this.AddRedDot(enSysEntryID.FriendBtn, enRedDotPos.enTopRight, 0);
			}
			else
			{
				this.DelRedDot(enSysEntryID.FriendBtn);
			}
		}

		private void OnCheckRedDotByServerVersionWithLobby()
		{
			if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) && CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) || CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab))
			{
				this.AddRedDot(enSysEntryID.MallBtn, enRedDotPos.enTopRight, 0);
			}
			this.CheckGotoWebEntryRedDot();
		}

		private void OnCheckDelMallEntryRedDot()
		{
			if (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) || !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) && !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab))
			{
				this.DelRedDot(enSysEntryID.MallBtn);
			}
		}

		private void CheckGotoWebEntryRedDot()
		{
			if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_GongLueEntry) && this.m_LobbyForm != null)
			{
				Transform transform = this.m_LobbyForm.transform.Find("Popup/SignBtn");
				if (transform != null)
				{
					CUIRedDotSystem.AddRedDot(transform.gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
			}
		}

		public void ValidateActivitySpot()
		{
			if (this.m_bInLobby)
			{
				int num = MonoSingleton<PandroaSys>.GetInstance().ShowPointCount(PandroaSys.PandoraModuleType.action);
				if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
				{
					uint num2 = Singleton<ActivitySys>.GetInstance().GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY);
					if (num > 0)
					{
						num2 += (uint)num;
					}
					this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, (int)num2);
				}
				else if (MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList)
				{
					this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, 0);
				}
				else if (MonoSingleton<PandroaSys>.GetInstance().ShowRedPoint)
				{
					this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, num);
				}
				else if (num > 0)
				{
					this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, num);
				}
				else
				{
					this.DelRedDot(enSysEntryID.ActivityBtn);
				}
			}
		}

		private void OnMailUnReadUpdate()
		{
			int unReadMailCount = Singleton<CMailSys>.instance.GetUnReadMailCount(true);
			if (this.m_LobbyForm != null)
			{
				if (unReadMailCount > 0)
				{
					this.AddRedDot(enSysEntryID.MailBtn, enRedDotPos.enTopRight, 0);
				}
				else
				{
					this.DelRedDot(enSysEntryID.MailBtn);
				}
			}
		}

		private void OnAchieveStateUpdate()
		{
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			if (masterAchieveInfo.HasRewardNotGot())
			{
				this.AddRedDot(enSysEntryID.AchievementBtn, enRedDotPos.enTopRight, 0);
			}
			else
			{
				this.DelRedDot(enSysEntryID.AchievementBtn);
			}
		}

		private void OnBagItemsUpdate()
		{
			this.ValidateActivitySpot();
			this.OnCheckSymbolEquipAlert();
		}

		public void OnCheckSymbolEquipAlert()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			int num;
			uint num2;
			if (masterRoleInfo.m_symbolInfo.CheckAnyWearSymbol(out num, out num2, 0))
			{
				this.AddRedDot(enSysEntryID.SymbolBtn, enRedDotPos.enTopRight, 0);
			}
			else
			{
				this.DelRedDot(enSysEntryID.SymbolBtn);
			}
		}

		private void OnPureLobbyShow()
		{
			if (CGuildHelper.IsLobbyFormGuildBtnShowRedDot())
			{
				this.AddRedDot(enSysEntryID.GuildBtn, enRedDotPos.enTopRight, 0);
			}
			else
			{
				this.DelRedDot(enSysEntryID.GuildBtn);
			}
			Singleton<CGuildMatchSystem>.GetInstance().ShowAllUnhandledOnlineInvitation();
		}

		private void OnCheckUpdateClientVersion()
		{
			if (Singleton<LobbyLogic>.instance.NeedUpdateClient)
			{
				Singleton<LobbyLogic>.instance.NeedUpdateClient = false;
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.None, false);
			}
		}

		public static void SendRedDotTimeLineRsp()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1470u);
			cSPkg.stPkgData.stRedDotListReq = new CSPKG_REDDOTLIST_REQ();
			cSPkg.stPkgData.stRedDotListReq.dwLastGetTime = 0u;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void ShowWangZheCnt()
		{
			CUIFormScript lobbyForm = Singleton<CLobbySystem>.instance.m_LobbyForm;
			if (lobbyForm == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				Transform transform = lobbyForm.transform.FindChild("BtnCon/LadderBtn/WangeZheCntPanel");
				if (transform != null)
				{
					if (masterRoleInfo.m_WangZheCnt > 0u)
					{
						transform.gameObject.CustomSetActive(true);
						Image component = transform.FindChild("KingMark").GetComponent<Image>();
						component.SetSprite(string.Format(CLadderSystem.ICON_KING_MARK_PATH, masterRoleInfo.m_WangZheCnt), lobbyForm, true, false, false, false);
					}
					else
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		[MessageHandler(1471)]
		public static void ReciveRedDotTimeLineInfo(CSPkg msg)
		{
			SCPKG_REDDOTLIST_RSP stRedDotListRsp = msg.stPkgData.stRedDotListRsp;
			Singleton<CLobbySystem>.instance.m_serverRedDotTimeLineInfo = stRedDotListRsp;
			CLobbySystem.CheckRedDotTimeLineShowState();
			CLobbySystem.SaveMallTagInfo();
		}

		public static void SaveMallTagInfo()
		{
			Singleton<CMallSystem>.GetInstance().ClearTagInfoList();
			SCPKG_REDDOTLIST_RSP serverRedDotTimeLineInfo = Singleton<CLobbySystem>.instance.m_serverRedDotTimeLineInfo;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			for (int i = 0; i < (int)serverRedDotTimeLineInfo.wRedDotCnt; i++)
			{
				COMDT_REDDOT_INFO cOMDT_REDDOT_INFO = serverRedDotTimeLineInfo.astRedDotList[i];
				if (cOMDT_REDDOT_INFO.bEnterType >= 4 && cOMDT_REDDOT_INFO.bEnterType <= 8 && currentUTCTime >= cOMDT_REDDOT_INFO.dwStartTime && currentUTCTime <= cOMDT_REDDOT_INFO.dwEndTime)
				{
					Singleton<CMallSystem>.GetInstance().SetTagInfo((int)(cOMDT_REDDOT_INFO.bEnterType - 4), cOMDT_REDDOT_INFO);
				}
			}
		}

		public static void CheckRedDotTimeLineShowState()
		{
			CUIFormScript lobbyForm = Singleton<CLobbySystem>.instance.m_LobbyForm;
			if (lobbyForm == null)
			{
				return;
			}
			GameObject widget = lobbyForm.GetWidget(9);
			CUICommonSystem.SetObjActive(widget, false);
			Singleton<CLobbySystem>.instance.HideTagIcon();
			SCPKG_REDDOTLIST_RSP serverRedDotTimeLineInfo = Singleton<CLobbySystem>.instance.m_serverRedDotTimeLineInfo;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			for (int i = 0; i < (int)serverRedDotTimeLineInfo.wRedDotCnt; i++)
			{
				COMDT_REDDOT_INFO cOMDT_REDDOT_INFO = serverRedDotTimeLineInfo.astRedDotList[i];
				if (cOMDT_REDDOT_INFO.bEnterType == 1 && cOMDT_REDDOT_INFO.bRedDotLabelType == 2)
				{
					if (currentUTCTime >= cOMDT_REDDOT_INFO.dwStartTime && currentUTCTime <= cOMDT_REDDOT_INFO.dwEndTime)
					{
						CUICommonSystem.SetObjActive(widget, true);
					}
				}
				else if ((cOMDT_REDDOT_INFO.bEnterType == 2 || cOMDT_REDDOT_INFO.bEnterType == 3) && currentUTCTime >= cOMDT_REDDOT_INFO.dwStartTime && currentUTCTime <= cOMDT_REDDOT_INFO.dwEndTime)
				{
					string iconPath = string.Format("{0}Type{1}.prefab", CLobbySystem.m_TagIConPath, (int)(cOMDT_REDDOT_INFO.bRedDotLabelType - 3 + 1));
					string text = Utility.UTF8Convert(cOMDT_REDDOT_INFO.szContent);
					if (cOMDT_REDDOT_INFO.bEnterType == 2)
					{
						Singleton<CLobbySystem>.instance.SetTagIcon(Singleton<CLobbySystem>.instance.m_ShopTagIcon, iconPath, text, true);
					}
					else if (cOMDT_REDDOT_INFO.bEnterType == 3)
					{
						Singleton<CLobbySystem>.instance.SetTagIcon(Singleton<CLobbySystem>.instance.m_ActivityIcon, iconPath, text, true);
					}
				}
			}
		}

		public void Test()
		{
			string iconPath = string.Format("{0}Type{1}.prefab", CLobbySystem.m_TagIConPath, 2);
			string text = "22";
			this.SetTagIcon(this.m_ShopTagIcon, iconPath, text, true);
			iconPath = string.Format("{0}Type{1}.prefab", CLobbySystem.m_TagIConPath, 3);
			text = "3";
			this.SetTagIcon(this.m_ActivityIcon, iconPath, text, true);
		}

		private void ClearTagIcon()
		{
			this.m_ShopTagIcon = null;
			this.m_ActivityIcon = null;
		}

		public void HideTagIcon()
		{
			this.SetTagIcon(this.m_ShopTagIcon, string.Empty, string.Empty, false);
			this.SetTagIcon(this.m_ActivityIcon, string.Empty, string.Empty, false);
		}

		private void InitTagIcon(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform;
			this.m_ShopTagIcon = transform.Find("Popup/BoardBtn/tagIcon");
			this.SetTagIcon(this.m_ShopTagIcon, string.Empty, string.Empty, false);
			this.m_ActivityIcon = transform.Find("Popup/ActBtn/tagIcon");
			this.SetTagIcon(this.m_ActivityIcon, string.Empty, string.Empty, false);
		}

		public void SetTagIcon(Transform imgObj, string iconPath, string text, bool bShow)
		{
			if (imgObj == null)
			{
				return;
			}
			imgObj.gameObject.CustomSetActive(bShow);
			if (bShow)
			{
				CUIUtility.SetImageSprite(imgObj.GetComponent<Image>(), iconPath, null, true, false, false, false);
				if (imgObj.GetComponentInChildren<Text>())
				{
					imgObj.GetComponentInChildren<Text>().text = text;
				}
			}
		}
	}
}

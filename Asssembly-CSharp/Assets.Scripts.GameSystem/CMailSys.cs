using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
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
	public class CMailSys : Singleton<CMailSys>
	{
		[Serializable]
		private struct stInvite
		{
			public ulong uid;

			public uint dwLogicWorldID;

			public uint time;

			public byte relationType;

			public byte inviteType;

			public byte processType;

			public string title;

			public byte bMapType;

			public uint dwMapId;

			public uint dwGameSvrEntity;

			public stInvite(string title, ulong uid, uint dwLogicWorldID, uint time, byte relationType, byte inviteType, byte processType, byte bMapType, uint dwMapId, uint dwGameSvrEntity)
			{
				this.title = title;
				this.uid = uid;
				this.dwLogicWorldID = dwLogicWorldID;
				this.time = time;
				this.relationType = relationType;
				this.inviteType = inviteType;
				this.processType = processType;
				this.bMapType = bMapType;
				this.dwMapId = dwMapId;
				this.dwGameSvrEntity = dwGameSvrEntity;
			}
		}

		public enum enMailSysFormWidget
		{
			GetAccessBtn,
			DeleteBtn,
			TitleText
		}

		public enum enMailWriteFormWidget
		{
			Mail_Title_Input,
			Mail_Content_Input,
			Mail_Send_Num_Text
		}

		public enum enProcessInviteType
		{
			Refuse,
			Accept,
			NoProcess
		}

		public const int c_driveingRefreshTimeDelta = 20;

		public const int c_refreshTimeDelta = 300;

		public const string FILE_FRIEND_INVITE_ARRAY = "SGAME_FILE_FRIEND_INVITE_ARRAY";

		public const int FILE_FRIEND_INVITE_ARRAY_LEN = 30;

		public static readonly string MAIL_FORM_PATH = "UGUI/Form/System/Mail/Form_Mail.prefab";

		public static readonly string MAIL_SYSTEM_FORM_PATH = "UGUI/Form/System/Mail/Form_SysMail.prefab";

		public static readonly string MAIL_WRITE_FORM_PATH = "UGUI/Form/System/Mail/Form_Mail_Write.prefab";

		public static readonly string MAIL_ASK_FOR_FORM_PATH = "UGUI/Form/System/Mail/Form_AskForMail.prefab";

		private ListView<CMail> m_friMailList = new ListView<CMail>();

		private ListView<CMail> m_sysMailList = new ListView<CMail>();

		private ListView<CMail> m_msgMailList = new ListView<CMail>();

		private ListView<CMail> m_askForMailList = new ListView<CMail>();

		private int m_friMailUnReadNum;

		private int m_sysMailUnReadNum;

		private int m_msgMailUnReadNum;

		private int m_askForMailUnReadNum;

		private bool m_friAccessAll;

		private bool m_sysAccessAll;

		private ListView<CUseable> m_accessList = new ListView<CUseable>();

		private CMailView m_mailView = new CMailView();

		private CSysMailView m_sysMailView = new CSysMailView();

		private CAskForMailView m_askForMailView = new CAskForMailView();

		private CMail m_mail;

		private uint m_mailFriVersion;

		private uint m_mailSysVersion;

		public int m_lastOpenTime;

		private List<CMailSys.stInvite> m_InviteList = new List<CMailSys.stInvite>();

		private bool bReadFile;

		public string offlineStr = "offline";

		public string onlineStr = "online";

		public string gamingStr = "gaming";

		public string inviteNoProcessStr = "gaming";

		public string inviteAcceptStr = "gaming";

		public string inviteRefuseStr = "gaming";

		public ListView<GuildMemInfo> m_mailGuildMemInfos = new ListLinqView<GuildMemInfo>();

		private int m_refreshGuildTimer = -1;

		public void LoadTxtStr()
		{
			this.offlineStr = Singleton<CTextManager>.instance.GetText("OfflineStr");
			this.onlineStr = Singleton<CTextManager>.instance.GetText("OnlineStr");
			this.gamingStr = Singleton<CTextManager>.instance.GetText("GamingStr");
			this.inviteNoProcessStr = Singleton<CTextManager>.instance.GetText("Invite_NoProcess");
			this.inviteAcceptStr = Singleton<CTextManager>.instance.GetText("Invite_Accept");
			this.inviteRefuseStr = Singleton<CTextManager>.instance.GetText("Invite_Refuse");
		}

		public void Clear()
		{
			this.m_mailView.SetUnReadNum(CustomMailType.FRIEND, 0);
			this.m_mailView.SetUnReadNum(CustomMailType.SYSTEM, 0);
			this.m_mailView.SetUnReadNum(CustomMailType.FRIEND_INVITE, 0);
			this.m_mailView.SetUnReadNum(CustomMailType.ASK_FOR, 0);
			this.m_friMailUnReadNum = 0;
			this.m_sysMailUnReadNum = 0;
			this.m_msgMailUnReadNum = 0;
			this.m_askForMailUnReadNum = 0;
			this.m_mailFriVersion = 0u;
			this.m_mailSysVersion = 0u;
			this.m_lastOpenTime = 0;
			this.m_friAccessAll = false;
			this.m_sysAccessAll = false;
			this.m_friMailList.Clear();
			this.m_sysMailList.Clear();
			this.m_msgMailList.Clear();
			this.m_InviteList.Clear();
			this.m_askForMailList.Clear();
			this.bReadFile = false;
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.RepeatReqMailList));
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMailForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendRead, new CUIEventManager.OnUIEventHandler(this.OnFriMailRead));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysRead, new CUIEventManager.OnUIEventHandler(this.OnSysMailRead));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForRead, new CUIEventManager.OnUIEventHandler(this.OnAskForMailRead));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseFriMailFrom));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSysMailFrom));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendAccessAll, new CUIEventManager.OnUIEventHandler(this.OnFriAccessAll));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysAccess, new CUIEventManager.OnUIEventHandler(this.OnSysAccess));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendAccess, new CUIEventManager.OnUIEventHandler(this.OnFriendAccess));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabFriend, new CUIEventManager.OnUIEventHandler(this.OnTabFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabSystem, new CUIEventManager.OnUIEventHandler(this.OnTabSysem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnTabMsgCenter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_TabGiftCenter, new CUIEventManager.OnUIEventHandler(this.OnTabAskFor));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysDelete, new CUIEventManager.OnUIEventHandler(this.OnSystemMailDelete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_SysAccessAll, new CUIEventManager.OnUIEventHandler(this.OnSysAccessAll));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_ListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnMailListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_AddToMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnAddToMsgCenter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_MsgCenterDeleteAll, new CUIEventManager.OnUIEventHandler(this.OnMsgCenterDeleteAll));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_JumpForm, new CUIEventManager.OnUIEventHandler(this.OnJumpForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_JumpUrl, new CUIEventManager.OnUIEventHandler(this.OnJumpUrl));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Form_OnClose, new CUIEventManager.OnUIEventHandler(this.OnMailFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Open_Mail_Write_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMailWriteForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Send_Guild_Mail, new CUIEventManager.OnUIEventHandler(this.OnSendGuildMail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_FriendDelete, new CUIEventManager.OnUIEventHandler(this.OnFriendMailDelete));
			Singleton<EventRouter>.GetInstance().AddEventHandler("MAIL_GUILD_MEM_UPDATE", new Action(this.OnGUILD_MEM_UPDATE));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new Action(this.OnLBS_User_Refresh));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Friend_Game_State_Change, new Action(this.OnFriendChg));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this.OnFriendOnlineChg));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_Invite, new CUIEventManager.OnUIEventHandler(this.OnMail_Invite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForRefuse, new CUIEventManager.OnUIEventHandler(this.OnAskForMailRefuse));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForAccept, new CUIEventManager.OnUIEventHandler(this.OnAskForMailAccept));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForAcceptConfirm, new CUIEventManager.OnUIEventHandler(this.OnAskForMailAcceptConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForSecurePwdConfirm, new CUIEventManager.OnUIEventHandler(this.OnAskForMailSecurePwdConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_AskForDel, new CUIEventManager.OnUIEventHandler(this.OnAskForMailDelete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mail_NoAskForFlagChange, new CUIEventManager.OnUIEventHandler(this.OnNoAskForFlagChange));
		}

		private void OnMail_Invite(CUIEvent uievent)
		{
			byte mapType = (byte)uievent.m_eventParams.heroId;
			uint weakGuideId = uievent.m_eventParams.weakGuideId;
			byte objSrc = (byte)uievent.m_eventParams.tag2;
			byte joinType = (byte)uievent.m_eventParams.tag3;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint taskId = uievent.m_eventParams.taskId;
			CInviteSystem.stInviteInfo inviteInfo = default(CInviteSystem.stInviteInfo);
			inviteInfo.playerUid = commonUInt64Param;
			inviteInfo.playerLogicWorldId = taskId;
			inviteInfo.joinType = (COM_INVITE_JOIN_TYPE)joinType;
			inviteInfo.objSrc = (CInviteView.enInviteListTab)objSrc;
			inviteInfo.gameEntity = (int)tagUInt;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(mapType, weakGuideId);
			if (pvpMapCommonInfo != null)
			{
				inviteInfo.maxTeamNum = (int)(pvpMapCommonInfo.bMaxAcntNum / 2);
			}
			DebugHelper.Assert(pvpMapCommonInfo != null, "----levelInfo is null... ");
			if (inviteInfo.joinType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM)
			{
				CRoomSystem.ReqCreateRoomAndInvite(weakGuideId, (COM_BATTLE_MAP_TYPE)mapType, inviteInfo);
			}
			else
			{
				CMatchingSystem.ReqCreateTeamAndInvite(weakGuideId, (COM_BATTLE_MAP_TYPE)mapType, inviteInfo);
			}
		}

		private void OnAskForMailRefuse(CUIEvent uiEvent)
		{
			int mailIndex = this.m_mail.mailIndex;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1388u);
			cSPkg.stPkgData.stAskforRefuseReq.iReqIndex = mailIndex;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnAskForMailAccept(CUIEvent uiEvent)
		{
			if (this.m_mail == null || this.m_mail.accessUseable == null || this.m_mail.accessUseable.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Mail_Ask_For_Err_Text_1", true, 1.5f, null, new object[0]);
				return;
			}
			int mailIndex = this.m_mail.mailIndex;
			CUseable cUseable = this.m_mail.accessUseable[0];
			uint num = 0u;
			switch (cUseable.m_type)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(cUseable.m_baseID);
				DebugHelper.Assert(dataByKey != null);
				if (dataByKey != null)
				{
					IHeroData heroData = CHeroDataFactory.CreateHeroData(cUseable.m_baseID);
					ResHeroPromotion resPromotion = heroData.promotion();
					stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
					for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
					{
						if (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)
						{
							num = payInfoSetOfGood.m_payInfos[i].m_payValue;
							break;
						}
					}
				}
				break;
			}
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
			{
				uint heroId = 0u;
				uint skinId = 0u;
				CSkinInfo.ResolveHeroSkin(cUseable.m_baseID, out heroId, out skinId);
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(cUseable.m_baseID);
				DebugHelper.Assert(heroSkin != null, "heroSkin is null");
				if (heroSkin != null)
				{
					stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
					for (int j = 0; j < skinPayInfoSet.m_payInfoCount; j++)
					{
						if (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.Diamond || skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan || skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan)
						{
							num = skinPayInfoSet.m_payInfos[j].m_payValue;
							break;
						}
					}
				}
				break;
			}
			}
			if (num == 0u)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Mail_Ask_For_Err_Text_2", true, 1.5f, null, new object[0]);
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.Mail_AskForAcceptConfirm;
			uIEvent.m_eventParams.tag = mailIndex;
			string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), cUseable.m_name, this.m_mail.from);
			CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, num, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, true, true, true);
		}

		private void OnAskForMailSecurePwdConfirm(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1390u);
			cSPkg.stPkgData.stAskforConfirmReq.iReqIndex = tag;
			StringHelper.StringToUTF8Bytes(uiEvent.m_eventParams.pwd, ref cSPkg.stPkgData.stAskforConfirmReq.szPswdStr);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnAskForMailAcceptConfirm(CUIEvent uiEvent)
		{
			if (this.m_mail == null || this.m_mail.accessUseable == null || this.m_mail.accessUseable.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Mail_Ask_For_Err_Text_1", true, 1.5f, null, new object[0]);
				return;
			}
			CUseable cUseable = this.m_mail.accessUseable[0];
			switch (cUseable.m_type)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
				CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_HERO_FOR_FRIEND, enUIEventID.Mail_AskForSecurePwdConfirm, uiEvent.m_eventParams);
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
				CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_SKIN_FOR_FRIEND, enUIEventID.Mail_AskForSecurePwdConfirm, uiEvent.m_eventParams);
				break;
			}
		}

		private void OnFriendOnlineChg(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldId, bool bOffline)
		{
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
		}

		private void OnFriendChg()
		{
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
		}

		private void OnLBS_User_Refresh()
		{
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
		}

		private void OnGUILD_MEM_UPDATE()
		{
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMailForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendRead, new CUIEventManager.OnUIEventHandler(this.OnFriMailRead));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysRead, new CUIEventManager.OnUIEventHandler(this.OnSysMailRead));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForRead, new CUIEventManager.OnUIEventHandler(this.OnAskForMailRead));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseFriMailFrom));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSysMailFrom));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendAccessAll, new CUIEventManager.OnUIEventHandler(this.OnFriAccessAll));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysAccess, new CUIEventManager.OnUIEventHandler(this.OnSysAccess));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_FriendAccess, new CUIEventManager.OnUIEventHandler(this.OnFriendAccess));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabFriend, new CUIEventManager.OnUIEventHandler(this.OnTabFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabSystem, new CUIEventManager.OnUIEventHandler(this.OnTabSysem));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnTabMsgCenter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_TabGiftCenter, new CUIEventManager.OnUIEventHandler(this.OnTabAskFor));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysDelete, new CUIEventManager.OnUIEventHandler(this.OnSystemMailDelete));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_SysAccessAll, new CUIEventManager.OnUIEventHandler(this.OnSysAccessAll));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_ListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnMailListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_AddToMsgCenter, new CUIEventManager.OnUIEventHandler(this.OnAddToMsgCenter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_MsgCenterDeleteAll, new CUIEventManager.OnUIEventHandler(this.OnMsgCenterDeleteAll));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_JumpForm, new CUIEventManager.OnUIEventHandler(this.OnJumpForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_JumpUrl, new CUIEventManager.OnUIEventHandler(this.OnJumpUrl));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_Form_OnClose, new CUIEventManager.OnUIEventHandler(this.OnMailFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_Invite, new CUIEventManager.OnUIEventHandler(this.OnMail_Invite));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForRefuse, new CUIEventManager.OnUIEventHandler(this.OnAskForMailRefuse));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForAccept, new CUIEventManager.OnUIEventHandler(this.OnAskForMailAccept));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForAcceptConfirm, new CUIEventManager.OnUIEventHandler(this.OnAskForMailAcceptConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForSecurePwdConfirm, new CUIEventManager.OnUIEventHandler(this.OnAskForMailSecurePwdConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_AskForDel, new CUIEventManager.OnUIEventHandler(this.OnAskForMailDelete));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mail_NoAskForFlagChange, new CUIEventManager.OnUIEventHandler(this.OnNoAskForFlagChange));
			base.UnInit();
		}

		public void InitLoginRsp(SCPKG_CMD_GAMELOGINRSP rsp)
		{
			Singleton<CTimerManager>.instance.AddTimer(300000, -1, new CTimer.OnTimeUpHandler(this.RepeatReqMailList));
		}

		private ListView<CMail> GetMailList(CustomMailType mailType)
		{
			if (mailType == CustomMailType.FRIEND)
			{
				return this.m_friMailList;
			}
			if (mailType == CustomMailType.SYSTEM)
			{
				return this.m_sysMailList;
			}
			if (mailType == CustomMailType.ASK_FOR)
			{
				return this.m_askForMailList;
			}
			return this.m_msgMailList;
		}

		private void SortMailList(CustomMailType mailType)
		{
			ListView<CMail> listView = null;
			if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
			}
			else if (mailType == CustomMailType.FRIEND_INVITE)
			{
				listView = this.m_msgMailList;
			}
			else if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
			}
			if (listView != null)
			{
				listView.Sort(new Comparison<CMail>(this.Comparison));
			}
		}

		private int Comparison(CMail mailA, CMail mailB)
		{
			if (mailA.mailState != mailB.mailState)
			{
				return (mailA.mailState - mailB.mailState) * 10000;
			}
			return (int)(mailB.sendTime - mailA.sendTime);
		}

		private CMail MailPop(CustomMailType mailType)
		{
			ListView<CMail> listView = null;
			if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
			}
			else if (mailType == CustomMailType.FRIEND_INVITE)
			{
				listView = this.m_msgMailList;
			}
			else if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
			}
			CMail cMail = null;
			for (int i = 0; i < listView.Count; i++)
			{
				if (cMail == null || cMail.sendTime > listView[i].sendTime)
				{
					cMail = listView[i];
				}
			}
			if (cMail != null)
			{
				listView.Remove(cMail);
			}
			return cMail;
		}

		private void OnOpenMailForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			this.m_friAccessAll = false;
			this.m_sysAccessAll = false;
			this.m_mailView.Open(CustomMailType.FRIEND);
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND, this.m_friMailList);
			this.m_mailView.UpdateMailList(CustomMailType.SYSTEM, this.m_sysMailList);
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
			this.m_mailView.UpdateMailList(CustomMailType.ASK_FOR, this.m_askForMailList);
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			if (currentUTCTime - this.m_lastOpenTime > 20)
			{
				this.ReqMailList(CustomMailType.SYSTEM, CustomMailOpt_MailListType.RefreshAll, true, 0);
				this.m_lastOpenTime = currentUTCTime;
			}
			if (!this.bReadFile)
			{
				this.ReadFriendInviteFile();
				this.bReadFile = true;
			}
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				this.m_refreshGuildTimer = Singleton<CTimerManager>.instance.AddTimer(10000, 0, new CTimer.OnTimeUpHandler(this.OnRefreshGuildTimer));
			}
		}

		private void OnRefreshGuildTimer(int timersequence)
		{
			Debug.Log("---cmailsys OnRefreshGuildTimer...");
			Singleton<CInviteSystem>.instance.SendSendGetGuildMemberGameStateReqRaw(this.m_mailGuildMemInfos);
		}

		public void AddGuildMemInfo(GuildMemInfo info)
		{
			if (info == null)
			{
				return;
			}
			if (this.GetGuildMemInfoIndex(info.stBriefInfo.uulUid) == -1)
			{
				this.m_mailGuildMemInfos.Add(info);
			}
		}

		private int GetGuildMemInfoIndex(ulong uid)
		{
			for (int i = 0; i < this.m_mailGuildMemInfos.Count; i++)
			{
				GuildMemInfo guildMemInfo = this.m_mailGuildMemInfos[i];
				if (guildMemInfo.stBriefInfo.uulUid == uid)
				{
					return i;
				}
			}
			return -1;
		}

		private void OnOpenSysMailForm(CMail mail, CustomMailType mailType)
		{
			this.m_friAccessAll = false;
			this.m_sysAccessAll = false;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_SYSTEM_FORM_PATH, false, true);
			GameObject widget = cUIFormScript.GetWidget(0);
			GameObject widget2 = cUIFormScript.GetWidget(1);
			bool flag = mailType == CustomMailType.FRIEND && mail.subType == 3;
			if (flag)
			{
				Text component = cUIFormScript.GetWidget(2).GetComponent<Text>();
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_Guild_Mail"));
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
			}
			else
			{
				widget2.CustomSetActive(false);
			}
			this.m_sysMailView.Form = cUIFormScript;
			this.m_sysMailView.mailType = mailType;
			this.m_sysMailView.Mail = mail;
			this.m_mail = mail;
		}

		private void OnOpenAskForMailForm(CMail mail, CustomMailType mailType)
		{
			this.m_friAccessAll = false;
			this.m_sysAccessAll = false;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_ASK_FOR_FORM_PATH, false, true);
			if (cUIFormScript == null)
			{
				DebugHelper.Assert(false, "mail ask for form is null");
				return;
			}
			this.m_askForMailView.Form = cUIFormScript;
			this.m_askForMailView.mailType = mailType;
			this.m_askForMailView.Mail = mail;
			this.m_mail = mail;
		}

		private void OnCloseFriMailFrom(CUIEvent uiEvent)
		{
		}

		private void OnCloseSysMailFrom(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CMailSys.MAIL_SYSTEM_FORM_PATH);
		}

		private void OnFriAccessAll(CUIEvent uiEvent)
		{
			this.m_accessList.Clear();
			int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_FRIEND);
			if (nearAccessIndex >= 0)
			{
				this.m_friAccessAll = true;
				this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[nearAccessIndex].mailIndex, 0);
			}
			else
			{
				this.m_friAccessAll = false;
			}
		}

		private void OnSysAccessAll(CUIEvent uiEvent)
		{
			int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_SYSTEM);
			if (nearAccessIndex >= 0)
			{
				this.m_sysAccessAll = true;
				this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysMailList[nearAccessIndex].mailIndex, 0);
			}
			else
			{
				this.m_sysAccessAll = false;
			}
		}

		private void OnSysAccess(CUIEvent uiEvent)
		{
			int mailIndex = this.m_mail.mailIndex;
			this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_SYSTEM, mailIndex, 0);
		}

		private void OnFriendAccess(CUIEvent uiEvent)
		{
			int mailIndex = this.m_mail.mailIndex;
			this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, mailIndex, 0);
		}

		private void OnTabFriend(CUIEvent uiEvent)
		{
			this.m_mailView.CurMailType = CustomMailType.FRIEND;
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND, this.m_friMailList);
			this.m_mailView.SetActiveTab(0);
		}

		private void OnTabSysem(CUIEvent uiEvent)
		{
			this.m_mailView.CurMailType = CustomMailType.SYSTEM;
			this.m_mailView.UpdateMailList(CustomMailType.SYSTEM, this.m_sysMailList);
			this.m_mailView.SetActiveTab(1);
		}

		private void OnTabMsgCenter(CUIEvent uiEvent)
		{
			this.m_mailView.CurMailType = CustomMailType.FRIEND_INVITE;
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
			this.m_mailView.SetActiveTab(2);
			this.m_msgMailUnReadNum = 0;
			this.UpdateUnReadNum();
		}

		private void OnTabAskFor(CUIEvent uiEvent)
		{
			this.m_mailView.CurMailType = CustomMailType.ASK_FOR;
			this.m_mailView.UpdateMailList(CustomMailType.ASK_FOR, this.m_askForMailList);
			this.m_mailView.SetActiveTab(3);
			this.m_askForMailUnReadNum = 0;
			this.UpdateUnReadNum();
		}

		private void OnSysMailRead(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_sysMailList.Count)
			{
				return;
			}
			this.OnMailRead(CustomMailType.SYSTEM, this.m_sysMailList[srcWidgetIndexInBelongedList].mailIndex);
		}

		private void OnFriMailRead(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			this.OnMailRead(CustomMailType.FRIEND, this.m_friMailList[srcWidgetIndexInBelongedList].mailIndex);
		}

		private void OnAskForMailRead(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_askForMailList.Count)
			{
				return;
			}
			this.OnMailRead(CustomMailType.ASK_FOR, this.m_askForMailList[srcWidgetIndexInBelongedList].mailIndex);
		}

		private void OnSystemMailDelete(CUIEvent uiEvent)
		{
			if (this.m_sysMailList != null && this.m_sysMailList.Count > 0)
			{
				int canBeDeleted = this.m_sysMailList[0].CanBeDeleted;
				if (canBeDeleted == 0)
				{
					this.ReqDeleteMail(CustomMailType.SYSTEM, this.m_sysMailList[0].mailIndex);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText(canBeDeleted.ToString()), false, 1.5f, null, new object[0]);
				}
			}
		}

		private void OnAskForMailDelete(CUIEvent uiEvent)
		{
			if (this.m_askForMailList != null && this.m_askForMailList.Count > 0)
			{
				int canBeDeleted = this.m_askForMailList[0].CanBeDeleted;
				if (canBeDeleted == 0)
				{
					this.ReqDeleteMail(CustomMailType.ASK_FOR, this.m_askForMailList[0].mailIndex);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText(canBeDeleted.ToString()), false, 1.5f, null, new object[0]);
				}
			}
		}

		private void OnNoAskForFlagChange(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_FORM_PATH);
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master Role Info is null");
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "PanelAskForMail/NoAskFor");
			if (gameObject != null)
			{
				bool isOn = gameObject.GetComponent<Toggle>().get_isOn();
				if (isOn != masterRoleInfo.IsNoAskFor)
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1225u);
					cSPkg.stPkgData.stAcntAskforFlagReq.bNoAskforFlag = (isOn ? 1 : 0);
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
				}
			}
		}

		private void OnFriendMailDelete(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CMailSys.MAIL_SYSTEM_FORM_PATH);
			if (this.m_friMailList != null && this.m_friMailList.Count > 0)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_FORM_PATH);
				if (form == null)
				{
					return;
				}
				int friendMailListSelectedIndex = this.m_mailView.GetFriendMailListSelectedIndex();
				if (friendMailListSelectedIndex < 0 || friendMailListSelectedIndex >= this.m_friMailList.Count)
				{
					return;
				}
				this.ReqDeleteMail(CustomMailType.FRIEND, this.m_friMailList[friendMailListSelectedIndex].mailIndex);
			}
		}

		private void OnMailListElementEnable(CUIEvent uiEvent)
		{
			CustomMailType curMailType = this.m_mailView.CurMailType;
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			ListView<CMail> mailList = this.GetMailList(curMailType);
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < mailList.Count)
			{
				this.m_mailView.UpdateListElenment(uiEvent.m_srcWidget, mailList[srcWidgetIndexInBelongedList]);
			}
		}

		private int GetNearAccessIndex(COM_MAIL_TYPE mailType)
		{
			int result = -1;
			if (mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
			{
				if (this.m_friMailList != null)
				{
					for (int i = 0; i < this.m_friMailList.Count; i++)
					{
						if (!this.m_friMailList[i].isAccess && this.m_friMailList[i].subType == 1)
						{
							result = i;
							break;
						}
					}
				}
			}
			else if (mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM && this.m_sysMailList != null)
			{
				for (int j = 0; j < this.m_sysMailList.Count; j++)
				{
					if (!this.m_sysMailList[j].isAccess && this.m_sysMailList[j].subType == 2)
					{
						result = j;
						break;
					}
				}
			}
			return result;
		}

		private void OnMailRead(CustomMailType mailType, int index)
		{
			CMail mail = this.GetMail(mailType, index);
			DebugHelper.Assert(mail != null, "mail cannot be find. mailType[{0}] , index[{1}]", new object[]
			{
				mailType,
				index
			});
			if (mail == null)
			{
				return;
			}
			if (!mail.isReceive)
			{
				this.ReqReadMail(mailType, index);
			}
			if (mailType == CustomMailType.ASK_FOR)
			{
				this.OnOpenAskForMailForm(mail, mailType);
			}
			else
			{
				this.OnOpenSysMailForm(mail, mailType);
			}
		}

		private void OnAddToMsgCenter(CUIEvent uiEvent)
		{
			COMDT_FRIEND_INFO friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(uiEvent.m_eventParams.tagStr, CFriendModel.FriendType.GameFriend);
			if (friendByName == null)
			{
				friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(uiEvent.m_eventParams.tagStr, CFriendModel.FriendType.SNS);
			}
			if (friendByName != null)
			{
			}
		}

		private void OnMsgCenterDeleteAll(CUIEvent uiEvent)
		{
			this.m_InviteList.Clear();
			this.WriteFriendInviteFile();
			this.m_msgMailList = this.ConvertToMailList();
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
			this.m_msgMailUnReadNum = 0;
			this.UpdateUnReadNum();
		}

		private void OnJumpForm(CUIEvent uiEvent)
		{
			CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)uiEvent.m_eventParams.tag, 0, 0, uiEvent.m_eventParams.tagList);
		}

		private void OnJumpUrl(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenUrl(uiEvent.m_eventParams.tagStr, true, 0);
		}

		private void OnMailFormClose(CUIEvent uiEvent)
		{
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_refreshGuildTimer);
			this.m_refreshGuildTimer = -1;
			this.m_friAccessAll = false;
			this.m_sysAccessAll = false;
			this.m_mailView.OnClose();
			this.m_mailGuildMemInfos.Clear();
		}

		private void OnOpenMailWriteForm(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_WRITE_FORM_PATH, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Text component = cUIFormScript.GetWidget(2).GetComponent<Text>();
			uint sendGuildMailCnt = (uint)CGuildHelper.GetSendGuildMailCnt();
			uint sendGuildMailLimit = CGuildHelper.GetSendGuildMailLimit();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_Today_Send_Num", new string[]
			{
				sendGuildMailCnt.ToString(),
				sendGuildMailLimit.ToString()
			}));
		}

		private void OnSendGuildMail(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_WRITE_FORM_PATH);
			if (form == null)
			{
				return;
			}
			InputField component = form.GetWidget(1).GetComponent<InputField>();
			string text = CUIUtility.RemoveEmoji(component.get_text()).Trim();
			if (string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Mail_Content_Cannot_Be_Empty", true, 1.5f, null, new object[0]);
				return;
			}
			InputField component2 = form.GetWidget(0).GetComponent<InputField>();
			string text2 = CUIUtility.RemoveEmoji(component2.get_text()).Trim();
			if (string.IsNullOrEmpty(text2) && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				text2 = Singleton<CTextManager>.GetInstance().GetText("Mail_Default_Guild_Mail_Title", new string[]
				{
					CGuildHelper.GetGuildName()
				});
			}
			Singleton<CGuildInfoController>.GetInstance().ReqSendGuildMail(text2, text);
		}

		public void AddFriendInviteMail(CUIEvent uiEvent, CMailSys.enProcessInviteType processType)
		{
			this.AddFriendInviteMail(uiEvent.m_eventParams.tagStr, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.taskId, (uint)CRoleInfo.GetCurrentUTCTime(), (byte)uiEvent.m_eventParams.tag2, (byte)uiEvent.m_eventParams.tag3, (byte)processType, (byte)uiEvent.m_eventParams.heroId, uiEvent.m_eventParams.weakGuideId, uiEvent.m_eventParams.tagUInt);
		}

		private void AddFriendInviteMail(string title, ulong uid, uint dwLogicWorldID, uint time, byte relationType, byte bInviteType, byte processType, byte bMapType, uint dwMapId, uint dwGameSvrEntity)
		{
			this.m_InviteList.Add(new CMailSys.stInvite(title, uid, dwLogicWorldID, time, relationType, bInviteType, processType, bMapType, dwMapId, dwGameSvrEntity));
			if (this.m_InviteList.get_Count() > 30)
			{
				this.m_InviteList.RemoveAt(0);
			}
			this.WriteFriendInviteFile();
			this.m_msgMailList = this.ConvertToMailList();
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
			if (processType == 2)
			{
				this.m_msgMailUnReadNum = 1;
			}
			this.UpdateUnReadNum();
		}

		private void WriteFriendInviteFile()
		{
			string cachePath = CFileManager.GetCachePath(string.Format("{0}_{1}", "SGAME_FILE_FRIEND_INVITE_ARRAY", Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID));
			byte[] bytes = Utility.ObjectToBytes(this.m_InviteList);
			Utility.WriteFile(cachePath, bytes);
		}

		private ListView<CMail> ConvertToMailList()
		{
			ListView<CMail> listView = new ListView<CMail>();
			for (int i = 0; i < this.m_InviteList.get_Count(); i++)
			{
				CMailSys.stInvite stInvite = this.m_InviteList.get_Item(i);
				listView.Add(new CMail
				{
					uid = stInvite.uid,
					dwLogicWorldID = stInvite.dwLogicWorldID,
					mailType = CustomMailType.FRIEND_INVITE,
					sendTime = stInvite.time,
					subject = stInvite.title,
					processType = stInvite.processType,
					relationType = stInvite.relationType,
					inviteType = stInvite.inviteType,
					bMapType = stInvite.bMapType,
					dwMapId = stInvite.dwMapId,
					dwGameSvrEntity = stInvite.dwGameSvrEntity
				});
			}
			listView.Sort(new Comparison<CMail>(this.ComMsgCenter));
			return listView;
		}

		private int ComMsgCenter(CMail mailA, CMail mailB)
		{
			return (int)(mailB.sendTime - mailA.sendTime);
		}

		private void ReadFriendInviteFile()
		{
			string cachePath = CFileManager.GetCachePath(string.Format("{0}_{1}", "SGAME_FILE_FRIEND_INVITE_ARRAY", Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID));
			byte[] array = Utility.ReadFile(cachePath);
			List<CMailSys.stInvite> list = null;
			try
			{
				if (array != null)
				{
					list = (Utility.BytesToObject(array) as List<CMailSys.stInvite>);
				}
			}
			catch (Exception var_3_49)
			{
			}
			if (list != null)
			{
				this.m_InviteList = list;
			}
			this.m_msgMailList = this.ConvertToMailList();
			this.m_mailView.UpdateMailList(CustomMailType.FRIEND_INVITE, this.m_msgMailList);
		}

		private void RepeatReqMailList(int timerSequence)
		{
			if (!Singleton<BattleLogic>.instance.isRuning && Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null && Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_FORM_PATH) == null)
			{
				this.ReqMailList(CustomMailType.SYSTEM, CustomMailOpt_MailListType.RefreshAll, false, 0);
				this.ReqMailList(CustomMailType.ASK_FOR, CustomMailOpt_MailListType.RefreshAskFor, false, 0);
			}
		}

		public CMail GetMail(CustomMailType mailType, int index)
		{
			ListView<CMail> listView = null;
			if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
			}
			else if (mailType == CustomMailType.FRIEND_INVITE)
			{
				listView = this.m_msgMailList;
			}
			else if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
			}
			if (listView != null)
			{
				for (int i = 0; i < listView.Count; i++)
				{
					if (listView[i].mailIndex == index)
					{
						return listView[i];
					}
				}
			}
			return null;
		}

		public void AddMail(CustomMailType mailType, CMail mail)
		{
			ListView<CMail> listView = null;
			if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
			}
			else if (mailType == CustomMailType.FRIEND_INVITE)
			{
				listView = this.m_msgMailList;
			}
			else if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if (mail.mailIndex == listView[i].mailIndex)
				{
					listView.RemoveAt(i);
					break;
				}
			}
			listView.Add(mail);
		}

		public bool DeleteMail(CustomMailType mailType, int index)
		{
			ListView<CMail> listView = null;
			if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
			}
			else if (mailType == CustomMailType.FRIEND_INVITE)
			{
				listView = this.m_msgMailList;
			}
			else if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if (listView[i].mailIndex == index)
				{
					listView.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void ReqMailList(CustomMailType mailType, CustomMailOpt_MailListType optType, bool isDiff = false, int startPos = 0)
		{
			uint dwVersion = 0u;
			if (mailType == CustomMailType.FRIEND)
			{
				dwVersion = this.m_mailFriVersion;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				dwVersion = this.m_mailSysVersion;
			}
			if (mailType != CustomMailType.ASK_FOR)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1400u);
				cSPkg.stPkgData.stMailOptReq.bOptType = 1;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetMailList = new CSDT_MAILOPTREQ_GETMAILLIST();
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bReqType = (byte)optType;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.dwVersion = dwVersion;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bIsDiff = (isDiff ? 1 : 0);
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetMailList.bStartPos = (byte)startPos;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(1379u);
				cSPkg2.stPkgData.stAskforReqGetReq.bOpt = 1;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, false);
			}
		}

		public void ReqSendMail(COM_MAIL_TYPE mailType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1400u);
			cSPkg.stPkgData.stMailOptReq.bOptType = 2;
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stSendMail = new CSDT_MAILOPTREQ_SENDMAIL();
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stSendMail.bMailType = (byte)mailType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void ReqReadMail(CustomMailType mailType, int index)
		{
			if (mailType != CustomMailType.ASK_FOR)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1400u);
				cSPkg.stPkgData.stMailOptReq.bOptType = 3;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stReadMail = new CSDT_MAILOPTREQ_READMAIL();
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stReadMail.bMailType = (byte)mailType;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stReadMail.iMailIndex = index;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(1386u);
				cSPkg2.stPkgData.stAskforReqReadReq.iReqIndex = index;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, false);
			}
		}

		public void ReqDeleteMail(CustomMailType mailType, int index)
		{
			if (mailType != CustomMailType.ASK_FOR)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1400u);
				cSPkg.stPkgData.stMailOptReq.bOptType = 4;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stDelMail = new CSDT_MAILOPTREQ_DELMAIL();
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stDelMail.bMailType = (byte)mailType;
				cSPkg.stPkgData.stMailOptReq.stOptInfo.stDelMail.iMailIndex = index;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(1382u);
				cSPkg2.stPkgData.stAskforReqDelReq.iReqIndex = index;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, true);
			}
		}

		public void ReqMailGetAccess(COM_MAIL_TYPE mailType, int index, int getAll = 0)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1400u);
			cSPkg.stPkgData.stMailOptReq.bOptType = 5;
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetAccess = new CSDT_MAILOPTREQ_GETACCESS();
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.bMailType = (byte)mailType;
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.iMailIndex = index;
			cSPkg.stPkgData.stMailOptReq.stOptInfo.stGetAccess.bGetAll = (byte)getAll;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void OnMailGetListRes(CSPkg msg, CustomMailType mailType)
		{
			ListView<CMail> listView = new ListView<CMail>();
			int num = 0;
			if (mailType == CustomMailType.ASK_FOR)
			{
				listView = this.m_askForMailList;
				num = 100;
			}
			else if (mailType == CustomMailType.FRIEND)
			{
				listView = this.m_friMailList;
				num = 30;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				listView = this.m_sysMailList;
				num = 100;
			}
			if (mailType != CustomMailType.ASK_FOR)
			{
				if (msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bIsDiff == 0)
				{
					listView.Clear();
				}
				for (int i = 0; i < (int)msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bCnt; i++)
				{
					this.AddMail(mailType, new CMail(mailType, ref msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.astMailInfo[i]));
				}
			}
			else
			{
				listView.Clear();
				for (int j = 0; j < (int)msg.stPkgData.stAskforReqGetRsp.wReqCnt; j++)
				{
					this.AddMail(mailType, new CMail(mailType, ref msg.stPkgData.stAskforReqGetRsp.astReqList[j]));
				}
			}
			while (listView.Count > num)
			{
				this.MailPop(mailType);
			}
			if (mailType == CustomMailType.FRIEND)
			{
				this.m_mailFriVersion = msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.dwVersion;
			}
			else if (mailType == CustomMailType.SYSTEM)
			{
				this.m_mailSysVersion = msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.dwVersion;
			}
			this.SortMailList(mailType);
			this.UpdateUnReadNum();
			this.m_mailView.UpdateMailList(mailType, listView);
		}

		public void OnMailSendMailRes(CSPkg msg)
		{
			COM_MAIL_TYPE bMailType = (COM_MAIL_TYPE)msg.stPkgData.stMailOptRes.stOptInfo.stSendMail.bMailType;
			switch (msg.stPkgData.stMailOptRes.stOptInfo.stSendMail.bResult)
			{
			case 3:
			{
				MAIL_OPT_MAILLISTTYPE optType = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTFRIEND;
				if (bMailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
				{
					optType = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTFRIEND;
				}
				else if (bMailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
				{
					optType = MAIL_OPT_MAILLISTTYPE.MAIL_OPT_MAILLISTSYS;
				}
				this.ReqMailList((CustomMailType)bMailType, (CustomMailOpt_MailListType)optType, false, 0);
				break;
			}
			case 4:
				Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
				this.m_sysAccessAll = false;
				break;
			case 5:
				Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
				this.m_sysAccessAll = false;
				break;
			case 6:
				Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_ListGeting"), false, 1.5f, null, new object[0]);
				break;
			case 8:
				if (this.m_friAccessAll)
				{
					Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
					this.m_accessList.Clear();
					this.m_friAccessAll = false;
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
				}
				break;
			}
		}

		public void OnMailReadMailRes(CSPkg msg)
		{
			CMail cMail = null;
			int num = -1;
			CustomMailType customMailType = CustomMailType.SYSTEM;
			if (msg.stPkgData.stMailOptRes != null)
			{
				customMailType = (CustomMailType)msg.stPkgData.stMailOptRes.stOptInfo.stReadMail.bMailType;
				num = msg.stPkgData.stMailOptRes.stOptInfo.stReadMail.iMailIndex;
				cMail = this.GetMail(customMailType, num);
			}
			else if (msg.stPkgData.stAskforReqReadRsp != null)
			{
				customMailType = CustomMailType.ASK_FOR;
				if (msg.stPkgData.stAskforReqReadRsp.iResult == 0)
				{
					num = msg.stPkgData.stAskforReqReadRsp.iReqIndex;
					cMail = this.GetMail(customMailType, num);
				}
			}
			DebugHelper.Assert(cMail != null, "mail cannot be find. mailType[{0}] , index[{1}]", new object[]
			{
				customMailType,
				num
			});
			if (cMail == null)
			{
				return;
			}
			if (msg.stPkgData.stMailOptRes != null)
			{
				cMail.Read(msg.stPkgData.stMailOptRes.stOptInfo.stReadMail);
			}
			else
			{
				cMail.Read();
			}
			this.m_sysMailView.Mail = cMail;
			this.m_mail = cMail;
			this.SortMailList(customMailType);
			if (customMailType == CustomMailType.FRIEND)
			{
				this.m_mailView.UpdateMailList(customMailType, this.m_friMailList);
			}
			else if (customMailType == CustomMailType.SYSTEM)
			{
				this.m_mailView.UpdateMailList(customMailType, this.m_sysMailList);
			}
			else if (customMailType == CustomMailType.ASK_FOR)
			{
				this.m_mailView.UpdateMailList(customMailType, this.m_askForMailList);
			}
			this.UpdateUnReadNum();
		}

		public void OnMailDeleteRes(CSPkg msg)
		{
			int index = -1;
			CustomMailType customMailType = CustomMailType.SYSTEM;
			if (msg.stPkgData.stMailOptRes != null)
			{
				customMailType = (CustomMailType)msg.stPkgData.stMailOptRes.stOptInfo.stDelMail.bMailType;
				index = msg.stPkgData.stMailOptRes.stOptInfo.stDelMail.iMailIndex;
			}
			else if (msg.stPkgData.stAskforReqDelRsp != null)
			{
				if (msg.stPkgData.stAskforReqDelRsp.iResult == 0)
				{
					customMailType = CustomMailType.ASK_FOR;
					index = msg.stPkgData.stAskforReqDelRsp.iReqIndex;
				}
				else
				{
					string strContent = Utility.ProtErrCodeToStr(1383, msg.stPkgData.stAskforReqDelRsp.iResult);
					Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
				}
			}
			this.DeleteMail(customMailType, index);
			ListView<CMail> mailList = null;
			if (customMailType == CustomMailType.FRIEND)
			{
				mailList = this.m_friMailList;
			}
			else if (customMailType == CustomMailType.SYSTEM)
			{
				mailList = this.m_sysMailList;
			}
			else if (customMailType == CustomMailType.ASK_FOR)
			{
				mailList = this.m_askForMailList;
			}
			this.UpdateUnReadNum();
			this.m_mailView.UpdateMailList(customMailType, mailList);
		}

		public void OnMailGetAccess(CSPkg msg)
		{
			switch (msg.stPkgData.stMailOptRes.stOptInfo.stGetAccess.bResult)
			{
			case 1:
			{
				CSDT_MAILOPTRES_GETACCESS stGetAccess = msg.stPkgData.stMailOptRes.stOptInfo.stGetAccess;
				CustomMailType bMailType = (CustomMailType)stGetAccess.bMailType;
				int iMailIndex = stGetAccess.iMailIndex;
				CMail mail = this.GetMail(bMailType, iMailIndex);
				if (mail != null)
				{
					if (bMailType == CustomMailType.FRIEND)
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							masterRoleInfo.getFriendCoinCnt++;
						}
					}
					mail.isAccess = true;
					CUseable[] array = LinqS.ToArray<CUseable>(CMailSys.StAccessToUseable(stGetAccess.astAccess, stGetAccess.astAccessFrom, (int)stGetAccess.bAccessCnt));
					this.OnCloseSysMailFrom(null);
					enUIEventID eventID = enUIEventID.None;
					if (this.m_friAccessAll)
					{
						CMailSys.ConnectVirtualList(ref this.m_accessList, array);
						int nearAccessIndex = this.GetNearAccessIndex(COM_MAIL_TYPE.COM_MAIL_FRIEND);
						if (nearAccessIndex >= 0)
						{
							this.ReqMailGetAccess(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friMailList[nearAccessIndex].mailIndex, 0);
						}
						else
						{
							Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
							this.m_accessList.Clear();
							this.m_friAccessAll = false;
							Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
						}
						this.UpdateUnReadNum();
						return;
					}
					if (this.m_sysAccessAll)
					{
						eventID = enUIEventID.Mail_SysAccessAll;
					}
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
					Singleton<CUIManager>.instance.OpenAwardTip(array, null, false, eventID, true, false, "Form_Award");
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null)
						{
							if (array[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
							{
								CHeroItem cHeroItem = array[i] as CHeroItem;
								if (cHeroItem != null && !(cHeroItem is CExpHeroItem))
								{
									CUICommonSystem.ShowNewHeroOrSkin(cHeroItem.m_baseID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
								}
							}
							else if (array[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
							{
								CHeroSkin cHeroSkin = array[i] as CHeroSkin;
								if (cHeroSkin != null && !(cHeroSkin is CExpHeroSkin))
								{
									CUICommonSystem.ShowNewHeroOrSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
								}
							}
							if (array[i].m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
							{
								if (array[i].ExtraFromType == 1)
								{
									int extraFromData = array[i].ExtraFromData;
									CUICommonSystem.ShowNewHeroOrSkin((uint)extraFromData, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint)array[i].m_stackCount, 0);
								}
								else if (array[i].ExtraFromType == 2)
								{
									int extraFromData2 = array[i].ExtraFromData;
									CUICommonSystem.ShowNewHeroOrSkin(0u, (uint)extraFromData2, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint)array[i].m_stackCount, 0);
								}
							}
						}
					}
				}
				break;
			}
			case 4:
				Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_PackageClean"), false, 1.5f, null, new object[0]);
				this.m_sysAccessAll = false;
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			case 5:
				Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("MailOp_Packagefull"), false, 1.5f, null, new object[0]);
				this.m_sysAccessAll = false;
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				break;
			case 8:
				if (this.m_friAccessAll)
				{
					if (this.m_accessList.Count > 0)
					{
						Singleton<CUIManager>.instance.OpenAwardTip(LinqS.ToArray<CUseable>(this.m_accessList), null, false, enUIEventID.None, false, false, "Form_Award");
					}
					this.m_accessList.Clear();
					this.m_friAccessAll = false;
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mailop_GetHeartLimit"), false, 1.5f, null, new object[0]);
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				}
				break;
			}
			this.UpdateUnReadNum();
		}

		public void OnMailUnReadRes(CSPkg msg)
		{
			this.ReqMailList(CustomMailType.FRIEND, CustomMailOpt_MailListType.ReqFriend, false, 0);
			this.ReqMailList(CustomMailType.SYSTEM, CustomMailOpt_MailListType.ReqSystem, false, 0);
		}

		public void UpdateUnReadNum()
		{
			this.m_friMailUnReadNum = 0;
			if (this.m_friMailList != null)
			{
				for (int i = 0; i < this.m_friMailList.Count; i++)
				{
					if (this.m_friMailList[i].mailState == COM_MAIL_STATE.COM_MAIL_UNREAD)
					{
						this.m_friMailUnReadNum++;
					}
				}
			}
			this.m_sysMailUnReadNum = 0;
			if (this.m_sysMailList != null)
			{
				for (int j = 0; j < this.m_sysMailList.Count; j++)
				{
					if (this.m_sysMailList[j].mailState == COM_MAIL_STATE.COM_MAIL_UNREAD)
					{
						this.m_sysMailUnReadNum++;
					}
				}
			}
			this.m_askForMailUnReadNum = 0;
			if (this.m_askForMailList != null)
			{
				for (int k = 0; k < this.m_askForMailList.Count; k++)
				{
					if (this.m_askForMailList[k].mailState == COM_MAIL_STATE.COM_MAIL_UNREAD)
					{
						this.m_askForMailUnReadNum++;
					}
				}
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null && !masterRoleInfo.bCanRecvCoin)
			{
				this.m_mailView.SetUnReadNum(CustomMailType.FRIEND, 0);
			}
			else
			{
				this.m_mailView.SetUnReadNum(CustomMailType.FRIEND, this.m_friMailUnReadNum);
			}
			this.m_mailView.SetUnReadNum(CustomMailType.SYSTEM, this.m_sysMailUnReadNum);
			this.m_mailView.SetUnReadNum(CustomMailType.FRIEND_INVITE, this.m_msgMailUnReadNum);
			this.m_mailView.SetUnReadNum(CustomMailType.ASK_FOR, this.m_askForMailUnReadNum);
			Singleton<EventRouter>.instance.BroadCastEvent("MailUnReadNumUpdate");
		}

		public int GetUnReadMailCount(bool bIgnoreFriend = false)
		{
			if (!bIgnoreFriend)
			{
				return this.m_friMailUnReadNum + this.m_sysMailUnReadNum + this.m_msgMailUnReadNum + this.m_askForMailUnReadNum;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo.bCanRecvCoin)
			{
				return this.m_friMailUnReadNum + this.m_sysMailUnReadNum + this.m_msgMailUnReadNum + this.m_askForMailUnReadNum;
			}
			return this.m_sysMailUnReadNum + this.m_msgMailUnReadNum + this.m_askForMailUnReadNum;
		}

		[MessageHandler(1380)]
		public static void OnAskForMsgRes(CSPkg msg)
		{
			Singleton<CMailSys>.instance.OnMailGetListRes(msg, CustomMailType.ASK_FOR);
		}

		[MessageHandler(1401)]
		public static void OnMailRes(CSPkg msg)
		{
			switch (msg.stPkgData.stMailOptRes.bOptType)
			{
			case 1:
				Singleton<CMailSys>.instance.OnMailGetListRes(msg, (CustomMailType)msg.stPkgData.stMailOptRes.stOptInfo.stGetMailList.bMailType);
				break;
			case 2:
				Singleton<CMailSys>.instance.OnMailSendMailRes(msg);
				break;
			case 3:
				Singleton<CMailSys>.instance.OnMailReadMailRes(msg);
				break;
			case 4:
				Singleton<CMailSys>.instance.OnMailDeleteRes(msg);
				break;
			case 5:
				Singleton<CMailSys>.instance.OnMailGetAccess(msg);
				break;
			case 6:
				Singleton<CMailSys>.instance.OnMailUnReadRes(msg);
				break;
			}
		}

		[MessageHandler(1387)]
		public static void OnAskForReadRes(CSPkg msg)
		{
			Singleton<CMailSys>.instance.OnMailReadMailRes(msg);
		}

		[MessageHandler(1389)]
		public static void OnAskForRefuseRes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAskforRefuseRsp.iResult == 0)
			{
				int iReqIndex = msg.stPkgData.stAskforRefuseRsp.iReqIndex;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1383u);
				cSPkg.stPkgData.stAskforReqDelRsp.iReqIndex = iReqIndex;
				cSPkg.stPkgData.stAskforReqDelRsp.iResult = 0;
				Singleton<CMailSys>.instance.OnMailDeleteRes(cSPkg);
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1389, msg.stPkgData.stAskforRefuseRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1391)]
		public static void OnAskForAcceptRes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAskforConfirmRsp.iResult == 0)
			{
				int iReqIndex = msg.stPkgData.stAskforConfirmRsp.iReqIndex;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1383u);
				cSPkg.stPkgData.stAskforReqDelRsp.iReqIndex = iReqIndex;
				cSPkg.stPkgData.stAskforReqDelRsp.iResult = 0;
				Singleton<CMailSys>.instance.OnMailDeleteRes(cSPkg);
				Singleton<CUIManager>.GetInstance().OpenTips("Buy_For_Friend_Success", true, 1.5f, null, new object[0]);
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1391, msg.stPkgData.stAskforConfirmRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1383)]
		public static void OnAskForDelRes(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CMailSys>.instance.OnMailDeleteRes(msg);
		}

		[MessageHandler(1226)]
		public static void OnNoAskForFlatChg(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master Role Info is null");
				return;
			}
			if (msg.stPkgData.stAcntAskforflagRsp.iResult == 0)
			{
				masterRoleInfo.IsNoAskFor = (msg.stPkgData.stAcntAskforflagRsp.bNoAskforFlag > 0);
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1226, msg.stPkgData.stAcntAskforflagRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_FORM_PATH);
				if (form == null)
				{
					return;
				}
				GameObject gameObject = Utility.FindChild(form.gameObject, "PanelAskForMail/NoAskFor");
				if (gameObject != null)
				{
					gameObject.GetComponent<Toggle>().set_isOn(masterRoleInfo.IsNoAskFor);
				}
			}
		}

		public static ListView<CUseable> StAccessToUseable(COMDT_MAILACCESS[] stAccess, CSDT_MAILACCESS_FROM[] stAccessFrom, int count)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < count; i++)
			{
				COM_MAILACCESS_TYPE bAccessType = (COM_MAILACCESS_TYPE)stAccess[i].bAccessType;
				CUseable cUseable = null;
				if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_HEART)
				{
					cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enHeart, (int)stAccess[i].stAccessInfo.stHeart.dwHeart);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_RONGYU)
				{
					cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int)stAccess[i].stAccessInfo.stRongYu.dwRongYuPoint);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_MASTERPOINT)
				{
					cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enMentorPoint, (int)stAccess[i].stAccessInfo.stMasterPoint.dwPoint);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_MONEY)
				{
					if (stAccess[i].stAccessInfo.stMoney.bType == 1)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enNoUsed, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 7)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 3)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enArenaCoin, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 4)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enBurningCoin, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 5)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGuildConstruct, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 6)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
					else if (stAccess[i].stAccessInfo.stMoney.bType == 2)
					{
						cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int)stAccess[i].stAccessInfo.stMoney.dwMoney);
					}
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_PROP)
				{
					COM_ITEM_TYPE wPropType = (COM_ITEM_TYPE)stAccess[i].stAccessInfo.stProp.wPropType;
					cUseable = CUseableManager.CreateUseable(wPropType, 0uL, stAccess[i].stAccessInfo.stProp.dwPropID, stAccess[i].stAccessInfo.stProp.iPropNum, 0);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_EXP)
				{
					cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, (int)stAccess[i].stAccessInfo.stExp.dwExp);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_HERO)
				{
					cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0uL, stAccess[i].stAccessInfo.stHero.dwHeroID, 1, 0);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_PIFU)
				{
					cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0uL, stAccess[i].stAccessInfo.stPiFu.dwSkinID, 1, 0);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_EXPHERO)
				{
					cUseable = CUseableManager.CreateExpUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0uL, stAccess[i].stAccessInfo.stExpHero.dwHeroID, stAccess[i].stAccessInfo.stExpHero.dwExpDays, 1, 0);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_EXPSKIN)
				{
					cUseable = CUseableManager.CreateExpUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0uL, stAccess[i].stAccessInfo.stExpSkin.dwSkinID, stAccess[i].stAccessInfo.stExpSkin.dwExpDays, 1, 0);
				}
				else if (bAccessType == COM_MAILACCESS_TYPE.COM_MAILACCESS_HEADIMG)
				{
					cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, 0uL, stAccess[i].stAccessInfo.stHeadImg.dwHeadImgID, 0, 0);
				}
				if (cUseable != null)
				{
					if (stAccessFrom != null && i < stAccessFrom.Length)
					{
						CSDT_MAILACCESS_FROM cSDT_MAILACCESS_FROM = stAccessFrom[i];
						if (cSDT_MAILACCESS_FROM != null)
						{
							if (cSDT_MAILACCESS_FROM.bFromType == 1)
							{
								cUseable.ExtraFromType = (int)cSDT_MAILACCESS_FROM.bFromType;
								cUseable.ExtraFromData = (int)cSDT_MAILACCESS_FROM.stFromInfo.stHeroInfo.dwHeroID;
							}
							else if (cSDT_MAILACCESS_FROM.bFromType == 2)
							{
								cUseable.ExtraFromType = (int)cSDT_MAILACCESS_FROM.bFromType;
								cUseable.ExtraFromData = (int)cSDT_MAILACCESS_FROM.stFromInfo.stSkinInfo.dwSkinID;
							}
						}
					}
					listView.Add(cUseable);
				}
			}
			return listView;
		}

		public static void ConnectVirtualList(ref ListView<CUseable> srcList1, CUseable[] srcList2)
		{
			for (int i = 0; i < srcList2.Length; i++)
			{
				bool flag = false;
				CVirtualItem cVirtualItem = srcList2[i] as CVirtualItem;
				if (cVirtualItem != null)
				{
					for (int j = 0; j < srcList1.Count; j++)
					{
						CVirtualItem cVirtualItem2 = srcList1[j] as CVirtualItem;
						if (cVirtualItem2 != null && cVirtualItem2.m_virtualType == cVirtualItem.m_virtualType)
						{
							cVirtualItem2.m_stackCount += cVirtualItem.m_stackCount;
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					srcList1.Add(srcList2[i]);
				}
			}
		}
	}
}

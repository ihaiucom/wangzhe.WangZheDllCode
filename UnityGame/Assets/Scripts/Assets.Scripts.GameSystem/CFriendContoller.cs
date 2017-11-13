using Apollo;
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
	public class CFriendContoller : Singleton<CFriendContoller>
	{
		public enum enMentorTab
		{
			MentorAndClassmate,
			Apprentice,
			Count
		}

		public const string MentorSearchResultNodeName = "MentorResult";

		public const string MentorSearchResultPath = "UGUI/Form/System/Friend/MentorResult.prefab";

		public const string MentorRecommendNodeName = "MentorRecommendList";

		public const string MentorSearchRecommendPath = "UGUI/Form/System/Friend/MentorRecommendList.prefab";

		public const string Evt_Mentor_OnGetRecommend = "Evt_Mentor_GetRecommend";

		public const string Evt_Mentor_OnGetResult = "Evt_Mentor_GetResult";

		public static string FriendFormPath = "UGUI/Form/System/Friend/FriendForm.prefab";

		public static string AddFriendFormPath = "UGUI/Form/System/Friend/AddFriend.prefab";

		public static string VerifyFriendFormPath = "UGUI/Form/System/Friend/Form_FriendVerification.prefab";

		public static string IntimacyRelaFormPath = "UGUI/Form/System/Friend/Form_IntimacyRela.prefab";

		public static string MentorRequestListFormPath = "UGUI/Form/System/Friend/Form_MentorRequestList.prefab";

		public static string MentorPrivilegeFormPath = "UGUI/Form/System/Friend/Form_MentorPrivilege.prefab";

		public static string MentorTaskFormPath = "UGUI/Form/System/Friend/Form_MentorTask.prefab";

		public static string FriendSingleListFormPath = "UGUI/Form/System/Friend/Form_SingleNodeList.prefab";

		public static string AddMentorFormPath = "UGUI/Form/System/Friend/Form_AddMentor.prefab";

		public int m_currMentorPrivilegeLv = 1;

		public CMentorListOffset[] m_mentorListOff;

		public CFriendModel model;

		public CFriendView view;

		public COMDT_FRIEND_INFO search_info;

		public static int MentorTabMask = 0;

		public static string[] s_mentorTabStr = null;

		public static string[] s_mentorTabName = new string[]
		{
			"Mentor_TabMentorNClassmate",
			"Mentor_TabApprentice"
		};

		public static SCPKG_MASTERSTUDENT_INFO m_mentorInfo = null;

		public static int s_addViewtype = 3;

		public string IntimacyUpInfo = string.Empty;

		public string IntimacyFullInfo = string.Empty;

		public int IntimacyUpValue;

		public string IntimacyLevelUpInfo = string.Empty;

		public bool bIntimacyLevelUp;

		public COMDT_ACNT_UNIQ m_mentorSelectedUin;

		private int m_mentorPrivilegePage = 1;

		public enFriendSingleListType singleListType
		{
			get;
			private set;
		}

		public ulong startCooldownTimestamp
		{
			get;
			set;
		}

		public CFriendContoller()
		{
			this.model = new CFriendModel();
			this.view = new CFriendView();
			this.m_mentorListOff = new CMentorListOffset[3];
			this.m_mentorListOff[1] = new CMentorListOffset();
			this.m_mentorListOff[2] = new CMentorListOffset();
		}

		public void ClearAll()
		{
			if (this.view != null)
			{
				this.view.Clear();
			}
			this.model.ClearAll();
			this.IntimacyUpInfo = string.Empty;
			this.IntimacyUpValue = 0;
			this.search_info = null;
			this.SetFilter(3u);
		}

		public override void Init()
		{
			base.Init();
			this.InitEvent();
		}

		public void Update()
		{
			this.view.Update();
		}

		private void InitEvent()
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_List", new Action<CSPkg>(this.On_FriendSys_Friend_Request_List));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Recommand_List", new Action<CSPkg>(this.On_FriendSys_Friend_Recommand_List));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg, enFriendSearchSource>("Friend_Search", new Action<CSPkg, enFriendSearchSource>(this.On_FriendSys_Friend_Search));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_RequestBeFriend", new Action<CSPkg>(this.On_FriendSys_Friend_RequestBeFriend));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg, CFriendModel.FriendType>("Friend_Confrim", new Action<CSPkg, CFriendModel.FriendType>(this.On_FriendSys_Friend_Confrim));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Deny", new Action<CSPkg>(this.On_FriendSys_Friend_Deny));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete", new Action<CSPkg>(this.On_FriendSys_Friend_Delete));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_ADD_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Delete_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Request_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Login_NTF", new Action<CSPkg>(this.On_Friend_Login_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_CHG_PROFILE", new Action<CSPkg>(this.On_Friend_SNS_CHG_PROFILE));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_NICNAME_NTF", new Action<CSPkg>(this.On_Friend_SNS_NICKNAME_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<float, float>("Friend_LBS_Location_Calced", new Action<float, float>(this.On_Friend_LBS_Location_Calced));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_RecommandFriend_Refresh", new Action(this.On_Friend_RecommandFriend_Refresh));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_FriendList_Refresh", new Action(this.On_Friend_FriendList_Refresh));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_SNSFriendList_Refresh", new Action(this.On_Friend_SNSFriendList_Refresh));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new Action(this.On_Friend_LBS_User_Refresh));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Invite_Success", new Action(this.On_GuildSys_Guild_Invite_Success));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Recommend_Success", new Action(this.On_GuildSys_Guild_Recommend_Success));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NEWDAY_NTF, new Action(this.OnNewDayNtf));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int, int>(EventID.GPS_DATA_GOT, new Action<int, int>(this.OnGPSDataGot));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_TabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenAddFriendForm, new CUIEventManager.OnUIEventHandler(this.On_OpenAddFriendForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RequestBeFriend, new CUIEventManager.OnUIEventHandler(this.On_AddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Accept_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Accept_RequestFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Refuse_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Refuse_RequestFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend, new CUIEventManager.OnUIEventHandler(this.On_DelFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_Friend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_FriendRequest, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_FriendRequest));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_OK, new CUIEventManager.OnUIEventHandler(this.On_DelFriend_OK));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_Cancle, new CUIEventManager.OnUIEventHandler(this.On_Friend_DelFriend_Cancle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_SendCoin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_SNSFriend_SendCoin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_InviteGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_InviteGuild));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RecommendGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_RecommendGuild));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CheckInfo, new CUIEventManager.OnUIEventHandler(this.On_Friend_CheckInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recommend_CheckInfo, new CUIEventManager.OnUIEventHandler(this.On_FriendRecommend_CheckInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Friend_List_ElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SingleList_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_SingleListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenBlacklist, new CUIEventManager.OnUIEventHandler(this.On_OpenBlackList));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenRquestlist, new CUIEventManager.OnUIEventHandler(this.On_OpenRequestList));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Invite_SNS_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Invite_SNS_Friend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Share_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_Share_SendCoin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_ReCall, new CUIEventManager.OnUIEventHandler(this.On_Friend_SNS_ReCall));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OB_Click, new CUIEventManager.OnUIEventHandler(this.On_Friend_OB_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.QQBOX_OnClick, new CUIEventManager.OnUIEventHandler(this.QQBox_OnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Share_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnShareToggle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Add_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnAddToggle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Chat_Button, new CUIEventManager.OnUIEventHandler(this.OnFriend_Chat_Button));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnFriend_Show_Rule));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Gift, new CUIEventManager.OnUIEventHandler(this.OnFriend_Gift_Button));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock_Ok, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtnOK));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_NoShare, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_NoShare));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nan, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nan));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nv, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nv));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Refresh, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Refresh));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_CheckInfo, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_CheckInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnFriend_Room_AddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Menu_Btn_Click, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_Menu_Btn_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_MentorQuest, new CUIEventManager.OnUIEventHandler(this.OnMentorTask_Btn_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_IWantMentor, new CUIEventManager.OnUIEventHandler(this.OnMentor_IWantMentor));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_IWantApprentice, new CUIEventManager.OnUIEventHandler(this.OnMentor_IWantApprentice));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_FriendFormTabChange, new CUIEventManager.OnUIEventHandler(this.OnMentorTabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_RequestListOnEnable, new CUIEventManager.OnUIEventHandler(this.On_MentorRequestListEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_AcceptRequest, new CUIEventManager.OnUIEventHandler(this.OnMentor_AcceptRequest));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_RefuseRequest, new CUIEventManager.OnUIEventHandler(this.OnMentor_RefuseRequest));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_AddMentor, new CUIEventManager.OnUIEventHandler(this.On_AddMentor));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_OpenRequestList, new CUIEventManager.OnUIEventHandler(this.OnMentor_RequestListClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_CloseRequestList, new CUIEventManager.OnUIEventHandler(this.OnMentorRequestListClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_OpenPrivilegePage, new CUIEventManager.OnUIEventHandler(this.OnMentor_OpenPrivilegePage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegePageLeft, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeLeftClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegePageRight, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeRightClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegeListEnable, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeListEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_GetMoreMentor, new CUIEventManager.OnUIEventHandler(this.OnMentorGetMoreMentorList));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Social_JumpFromFriendPage, new CUIEventManager.OnUIEventHandler(this.On_JumpToSocialCard));
		}

		private void On_Friend_FriendList_Refresh()
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void On_Friend_SNSFriendList_Refresh()
		{
			if (this.view != null && this.view.addFriendView != null && this.view.addFriendView.bShow)
			{
				this.view.addFriendView.Refresh(3);
			}
		}

		private void On_Friend_LBS_User_Refresh()
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void On_Friend_RecommandFriend_Refresh()
		{
		}

		private void On_Friend_Login_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS stNtfFriendLoginStatus = msg.stPkgData.stNtfFriendLoginStatus;
			CFriendModel.FriendType type = (stNtfFriendLoginStatus.bFriendType == 1) ? CFriendModel.FriendType.GameFriend : CFriendModel.FriendType.SNS;
			COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(type, stNtfFriendLoginStatus.stUin);
			if (info == null)
			{
				return;
			}
			info.bIsOnline = stNtfFriendLoginStatus.bLoginStatus;
			info.dwLastLoginTime = (uint)CRoleInfo.GetCurrentUTCTime();
			info.ullUserPrivacyBits = stNtfFriendLoginStatus.ullUserPrivacyBits;
			CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			if (friendInGaming != null)
			{
				friendInGaming.antiDisturbBits = stNtfFriendLoginStatus.dwOtherStateBits;
			}
			Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void On_Mentor_Login_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS stMasterStudentLoginNtf = msg.stPkgData.stMasterStudentLoginNtf;
			byte bType = stMasterStudentLoginNtf.bType;
			CFriendModel.FriendType type;
			if (bType != 2)
			{
				if (bType != 3)
				{
					return;
				}
				type = CFriendModel.FriendType.Apprentice;
			}
			else
			{
				type = CFriendModel.FriendType.Mentor;
			}
			COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(type, stMasterStudentLoginNtf.stUin);
			if (info == null)
			{
				return;
			}
			info.bIsOnline = stMasterStudentLoginNtf.bLoginStatus;
			info.dwLastLoginTime = (uint)CRoleInfo.GetCurrentUTCTime();
			this.model.SortGameFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void OnMentor_NoAskFor_Ntf(CSPkg msg)
		{
			SCPKG_MASTERSTUDENT_NOASKFOR_NTF stMasterStudentNoAskforNtf = msg.stPkgData.stMasterStudentNoAskforNtf;
			this.Handle_NoAskFor_Flag(stMasterStudentNoAskforNtf.stAcntUin.ullUid, stMasterStudentNoAskforNtf.stAcntUin.dwLogicWorldId, stMasterStudentNoAskforNtf.bNoAskforFlag);
			this.model.SortGameFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void OnFriend_NoAskFor_Ntf(CSPkg msg)
		{
			SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG stNtfFriendNoAskforFlag = msg.stPkgData.stNtfFriendNoAskforFlag;
			this.Handle_NoAskFor_Flag(stNtfFriendNoAskforFlag.stUin.ullUid, stNtfFriendNoAskforFlag.stUin.dwLogicWorldId, stNtfFriendNoAskforFlag.bNoAskforFlag);
			this.model.SortGameFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void On_Friend_GAME_STATE_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState = msg.stPkgData.stNtfFriendGameState;
			this.model.SetFriendGameState(stNtfFriendGameState.stAcntUniq.ullUid, stNtfFriendGameState.stAcntUniq.dwLogicWorldId, (COM_ACNT_GAME_STATE)stNtfFriendGameState.bGameState, stNtfFriendGameState.dwGameStartTime, string.Empty, false, true, stNtfFriendGameState.dwOtherStateBits);
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
		}

		private void On_Friend_Tab_FriendRequest(CUIEvent uievent)
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.On_Tab_Change(1);
			}
		}

		private void On_OpenBlackList(CUIEvent evt)
		{
			this.OpenSingleList(enFriendSingleListType.blackList);
		}

		private void On_OpenRequestList(CUIEvent evt)
		{
			this.OpenSingleList(enFriendSingleListType.requestList);
		}

		private void OpenSingleList(enFriendSingleListType listType)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.FriendSingleListFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
			GameObject widget = cUIFormScript.GetWidget(3);
			Text component2 = widget.transform.Find("Text").GetComponent<Text>();
			GameObject widget2 = cUIFormScript.GetWidget(6);
			GameObject widget3 = cUIFormScript.GetWidget(1);
			if (listType != enFriendSingleListType.requestList)
			{
				if (listType == enFriendSingleListType.blackList)
				{
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("Friend_Title_Blacklist"));
					component2.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoBlackList_Tip"));
					widget.CustomSetActive(this.model.GetBlackList().get_Count() == 0);
					RectTransform rectTransform = widget2.transform as RectTransform;
					RectTransform rectTransform2 = widget3.transform as RectTransform;
					rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height + rectTransform2.rect.height);
				}
			}
			else
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Friend_Title_Requests"));
				component2.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoRequest_Tip"));
				widget.CustomSetActive(this.model.GetDataCount(CFriendModel.FriendType.RequestFriend) == 0);
			}
			this.singleListType = listType;
			this.view.Refresh_SnsSwitch();
			this.view.Refresh_SingleList();
		}

		public CUIFormScript GetSingleListForm()
		{
			return Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendSingleListFormPath);
		}

		private void On_Friend_Tab_Friend(CUIEvent uievent)
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.On_Tab_Change(0);
			}
		}

		private void On_TabChange(CUIEvent uievent)
		{
			if (this.view != null && this.view.IsActive())
			{
				int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
				this.view.On_Tab_Change(selectedIndex);
			}
		}

		private void On_Friend_SNS_STATE_NTF(CSPkg msg)
		{
			SCPKG_NTF_SNS_FRIEND stNtfSnsFriend = msg.stPkgData.stNtfSnsFriend;
			ListView<COMDT_FRIEND_INFO> list = this.model.GetList(CFriendModel.FriendType.SNS);
			int num = 0;
			while ((long)num < (long)((ulong)stNtfSnsFriend.dwSnsFriendNum))
			{
				CSDT_SNS_FRIEND_INFO cSDT_SNS_FRIEND_INFO = stNtfSnsFriend.astSnsFriendList[num];
				if (cSDT_SNS_FRIEND_INFO != null && !this.FilterSameFriend(cSDT_SNS_FRIEND_INFO.stSnsFrindInfo, list))
				{
					this.model.Add(CFriendModel.FriendType.SNS, cSDT_SNS_FRIEND_INFO.stSnsFrindInfo, false);
					this.model.SetFriendGameState(cSDT_SNS_FRIEND_INFO.stSnsFrindInfo.stUin.ullUid, cSDT_SNS_FRIEND_INFO.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE)cSDT_SNS_FRIEND_INFO.bGameState, cSDT_SNS_FRIEND_INFO.dwGameStartTime, UT.Bytes2String(cSDT_SNS_FRIEND_INFO.szNickName), false, true, cSDT_SNS_FRIEND_INFO.dwOtherStateBits);
					this.Handle_CoinSend_Data(cSDT_SNS_FRIEND_INFO);
				}
				num++;
			}
			this.model.SortSNSFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
		}

		public static int GetSrvFriendTypeFromFriendType(CFriendModel.FriendType friendType)
		{
			switch (friendType)
			{
			case CFriendModel.FriendType.GameFriend:
				return 1;
			case CFriendModel.FriendType.SNS:
				return 2;
			case CFriendModel.FriendType.Mentor:
				return 6;
			case CFriendModel.FriendType.Apprentice:
				return 5;
			}
			return -1;
		}

		public static int GetFriendTypeFromSrvFriendType(COM_FRIEND_TYPE srvFriendType)
		{
			switch (srvFriendType)
			{
			case COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME:
				return 1;
			case COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS:
				return 4;
			case COM_FRIEND_TYPE.COM_FRIEND_TYPE_STUDENT:
				return 7;
			case COM_FRIEND_TYPE.COM_FRIEND_TYPE_MASTER:
				return 6;
			}
			return -1;
		}

		private void On_Friend_SNS_NICKNAME_NTF(CSPkg msg)
		{
			SCPKG_NTF_SNS_NICKNAME stNtfSnsNickName = msg.stPkgData.stNtfSnsNickName;
			uint num = Math.Min(stNtfSnsNickName.dwSnsFriendNum, 100u);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!CFriendModel.RemarkNames.ContainsKey(stNtfSnsNickName.astSnsNameList[num2].ullUid))
				{
					CFriendModel.RemarkNames.Add(stNtfSnsNickName.astSnsNameList[num2].ullUid, CUIUtility.RemoveEmoji(UT.Bytes2String(stNtfSnsNickName.astSnsNameList[num2].szNickName)));
				}
				num2++;
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public bool FilterSameFriend(COMDT_FRIEND_INFO info, ListView<COMDT_FRIEND_INFO> friendList)
		{
			if (friendList != null)
			{
				for (int i = 0; i < friendList.Count; i++)
				{
					if (friendList[i].stUin.ullUid == info.stUin.ullUid)
					{
						if (friendList[i].dwLastLoginTime < info.dwLastLoginTime)
						{
							friendList[i] = info;
						}
						return true;
					}
				}
			}
			return false;
		}

		public void OnReCallFriendNtf(CSPkg msg)
		{
			for (int i = 0; i < (int)msg.stPkgData.stNtfRecallFirend.wRecallNum; i++)
			{
				COMDT_RECALL_FRIEND cOMDT_RECALL_FRIEND = msg.stPkgData.stNtfRecallFirend.astRecallAcntList[i];
				if (cOMDT_RECALL_FRIEND != null)
				{
					this.Handle_Invite_Data(cOMDT_RECALL_FRIEND.stAcntUniq);
				}
			}
			this.model.friendRecruit.SetRecruiterRewardBits(msg.stPkgData.stNtfRecallFirend.dwRecruiterRewardBits);
			this.model.FRData.SetFirstChoiseState(msg.stPkgData.stNtfRecallFirend.bIntimacyRelationPrior);
		}

		public void OnSnsSwitchRsp(CSPkg msg)
		{
		}

		public void OnSnsSwitchNtf(CSPkg msg)
		{
			COMDT_ACNT_UNIQ stUin = msg.stPkgData.stNtfRefuseRecall.stUin;
			COMDT_FRIEND_INFO gameOrSnsFriend = this.model.GetGameOrSnsFriend(stUin.ullUid, stUin.dwLogicWorldId);
			gameOrSnsFriend.dwRefuseFriendBits = msg.stPkgData.stNtfRefuseRecall.dwRefuseFriendBits;
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
		}

		public void OnChangeIntimacy(CSPkg msg)
		{
			if (msg.stPkgData.stNtfChgIntimacy.dwResult == 0u)
			{
				SCPKG_CMD_NTF_CHG_INTIMACY stNtfChgIntimacy = msg.stPkgData.stNtfChgIntimacy;
				ushort num;
				CFriendModel.EIntimacyType eIntimacyType;
				bool flag;
				if (this.model.GetFriendIntimacy(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId, out num, out eIntimacyType, out flag))
				{
					int num2 = (int)(stNtfChgIntimacy.stIntimacData.wIntimacyValue - num);
					if (num2 > 0)
					{
						COMDT_FRIEND_INFO gameOrSnsFriend = this.model.GetGameOrSnsFriend(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId);
						if (gameOrSnsFriend != null)
						{
							string otherName = UT.Bytes2String(gameOrSnsFriend.szUserName);
							if (!Singleton<BattleLogic>.instance.isRuning)
							{
								bool flag2 = true;
								int num3 = IntimacyRelationViewUT.CalcRelaLevel((int)num);
								int num4 = IntimacyRelationViewUT.CalcRelaLevel((int)stNtfChgIntimacy.stIntimacData.wIntimacyValue);
								if (num4 > num3)
								{
									CFR cfr = this.model.FRData.GetCfr(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId);
									if (cfr != null && IntimacyRelationViewUT.IsRelaState(cfr.state))
									{
										this.model.intimacyTipsMgr.AddRelaLevelUpMsgBox(otherName, num4, cfr.state);
										flag2 = false;
									}
								}
								if (flag2)
								{
									this.model.intimacyTipsMgr.RecordPlayerUpValueTips(otherName, num2);
								}
								this.model.intimacyTipsMgr.CheckShouldShowTips(null);
							}
							else
							{
								bool flag3 = true;
								int num5 = IntimacyRelationViewUT.CalcRelaLevel((int)num);
								int num6 = IntimacyRelationViewUT.CalcRelaLevel((int)stNtfChgIntimacy.stIntimacData.wIntimacyValue);
								if (num6 > num5)
								{
									CFR cfr2 = this.model.FRData.GetCfr(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId);
									if (cfr2 != null && IntimacyRelationViewUT.IsRelaState(cfr2.state))
									{
										this.model.intimacyTipsMgr.AddRelaLevelUpMsgBox(otherName, num6, cfr2.state);
										flag3 = false;
									}
								}
								if (flag3)
								{
									this.model.intimacyTipsMgr.RecordPlayerUpValueTips(otherName, num2);
								}
							}
						}
					}
				}
				this.model.AddFriendIntimacy(stNtfChgIntimacy.stUin, stNtfChgIntimacy.stIntimacData);
				Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
				if (this.view != null && this.view.IsActive())
				{
					this.view.Refresh();
				}
			}
			else if (msg.stPkgData.stNtfChgIntimacy.dwResult == 170u)
			{
				this.IntimacyUpInfo = "跟朋友的亲密度已满";
				if (!Singleton<BattleLogic>.instance.isRuning)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.IntimacyUpInfo, false, 1.5f, null, new object[0]);
					this.IntimacyUpInfo = string.Empty;
				}
				SCPKG_CMD_NTF_CHG_INTIMACY stNtfChgIntimacy2 = msg.stPkgData.stNtfChgIntimacy;
				this.model.AddFriendIntimacy(stNtfChgIntimacy2.stUin, stNtfChgIntimacy2.stIntimacData);
				Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
				if (this.view != null && this.view.IsActive())
				{
					this.view.Refresh();
				}
			}
		}

		private void On_Friend_SNS_CHG_PROFILE(CSPkg msg)
		{
			SCPKG_CHG_SNS_FRIEND_PROFILE stSnsFriendChgProfile = msg.stPkgData.stSnsFriendChgProfile;
			this.model.Add(CFriendModel.FriendType.SNS, stSnsFriendChgProfile.stSnsFrindInfo, true);
			this.Handle_NoAskFor_Flag(stSnsFriendChgProfile.stSnsFrindInfo.stUin.ullUid, stSnsFriendChgProfile.stSnsFrindInfo.stUin.dwLogicWorldId, stSnsFriendChgProfile.bNoAskforFlag);
			this.model.SetFriendGameState(stSnsFriendChgProfile.stSnsFrindInfo.stUin.ullUid, stSnsFriendChgProfile.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE)stSnsFriendChgProfile.bGameState, 0u, string.Empty, true, true, stSnsFriendChgProfile.dwOtherStateBits);
			this.model.SortSNSFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
		}

		private void On_Friend_LBS_Location_Calced(float n, float e)
		{
		}

		private void On_JumpToSocialCard(CUIEvent evt)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(masterRoleInfo.playerUllUID, masterRoleInfo.logicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Social_Info);
			Singleton<CUINewFlagSystem>.GetInstance().SetNewFlagForSocialFriend(false);
		}

		private void On_FriendSys_Friend_List(CSPkg msg)
		{
			SCPKG_CMD_LIST_FRIEND stFriendListRsp = msg.stPkgData.stFriendListRsp;
			if (stFriendListRsp == null)
			{
				return;
			}
			int num = Mathf.Min(stFriendListRsp.astFrindList.Length, (int)stFriendListRsp.dwFriendNum);
			for (int i = 0; i < num; i++)
			{
				CSDT_FRIEND_INFO cSDT_FRIEND_INFO = stFriendListRsp.astFrindList[i];
				this.model.Add(CFriendModel.FriendType.GameFriend, cSDT_FRIEND_INFO.stFriendInfo, false);
				this.Handle_CoinSend_Data(cSDT_FRIEND_INFO);
				this.model.SetFriendGameState(cSDT_FRIEND_INFO.stFriendInfo.stUin.ullUid, cSDT_FRIEND_INFO.stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE)cSDT_FRIEND_INFO.bGameState, cSDT_FRIEND_INFO.dwGameStartTime, string.Empty, false, true, cSDT_FRIEND_INFO.dwOtherStateBits);
				this.model.AddFriendIntimacy(cSDT_FRIEND_INFO.stFriendInfo.stUin, cSDT_FRIEND_INFO.stIntimacyData);
				if (cSDT_FRIEND_INFO.stRecruitmentInfo != null)
				{
					this.model.friendRecruit.ParseFriend(cSDT_FRIEND_INFO.stFriendInfo, cSDT_FRIEND_INFO.stRecruitmentInfo);
				}
				this.Handle_NoAskFor_Flag(cSDT_FRIEND_INFO.stFriendInfo.stUin.ullUid, cSDT_FRIEND_INFO.stFriendInfo.stUin.dwLogicWorldId, cSDT_FRIEND_INFO.bNoAskforFlag);
			}
			Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Rank_Friend_List");
		}

		private void Handle_CoinSend_Data(CSDT_FRIEND_INFO info)
		{
			this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
		}

		private void Handle_NoAskFor_Flag(ulong ullUid, uint dwLogicWorldID, byte bNoAskFor)
		{
			stFriendByUUIDAndLogicID stFriendByUUIDAndLogicID = new stFriendByUUIDAndLogicID(ullUid, dwLogicWorldID, CFriendModel.FriendType.GameFriend);
			if (!CFriendModel.IsNoAskForDic.ContainsKey(stFriendByUUIDAndLogicID))
			{
				CFriendModel.IsNoAskForDic.Add(stFriendByUUIDAndLogicID, bNoAskFor > 0);
			}
			else
			{
				CFriendModel.IsNoAskForDic.set_Item(stFriendByUUIDAndLogicID, bNoAskFor > 0);
			}
		}

		public static bool IsNoAskFor(ulong ullUid, uint dwLogicWorldId)
		{
			stFriendByUUIDAndLogicID stFriendByUUIDAndLogicID = new stFriendByUUIDAndLogicID(ullUid, dwLogicWorldId, CFriendModel.FriendType.GameFriend);
			return CFriendModel.IsNoAskForDic.ContainsKey(stFriendByUUIDAndLogicID) && CFriendModel.IsNoAskForDic.get_Item(stFriendByUUIDAndLogicID);
		}

		private void Handle_CoinSend_Data(CSDT_SNS_FRIEND_INFO info)
		{
			this.Update_Send_Coin_Data(info.stSnsFrindInfo.stUin, (ulong)info.dwDonateTime);
		}

		private void Handle_CoinSend_Data_Mentor(CSDT_FRIEND_INFO info)
		{
			this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
		}

		private void Handle_CoinSend_Data_Apprentice(CSDT_FRIEND_INFO info)
		{
			this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
		}

		private void Handle_Invite_Data(COMDT_ACNT_UNIQ uin)
		{
			this.model.SnsReCallData.Add(uin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
		}

		private void Update_Send_Coin_Data(COMDT_ACNT_UNIQ uin, ulong donateAPSec)
		{
			DateTime dateTime = Utility.ToUtcTime2Local((long)donateAPSec);
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (dateTime2.get_Year() == dateTime.get_Year() && dateTime2.get_Month() == dateTime.get_Month() && dateTime2.get_Day() == dateTime.get_Day())
			{
				this.model.HeartData.Add(uin);
			}
			if (dateTime2.get_Year() < dateTime.get_Year() || (dateTime2.get_Year() == dateTime.get_Year() && dateTime2.get_Month() < dateTime.get_Month()) || dateTime2.get_Year() != dateTime.get_Year() || dateTime2.get_Month() != dateTime.get_Month() || dateTime2.get_Day() < dateTime.get_Day())
			{
			}
		}

		private void On_FriendSys_Friend_Request_List(CSPkg msg)
		{
			SCPKG_CMD_LIST_FRIENDREQ stFriendReqListRsp = msg.stPkgData.stFriendReqListRsp;
			if (stFriendReqListRsp == null)
			{
				return;
			}
			int num = Mathf.Min(stFriendReqListRsp.astVerificationList.Length, (int)stFriendReqListRsp.dwFriendReqNum);
			for (int i = 0; i < num; i++)
			{
				CSDT_VERIFICATION_INFO cSDT_VERIFICATION_INFO = stFriendReqListRsp.astVerificationList[i];
				if (cSDT_VERIFICATION_INFO.bIntimacyState == 0)
				{
					this.model.Add(CFriendModel.FriendType.RequestFriend, cSDT_VERIFICATION_INFO.stFriendInfo, false);
					this.model.AddFriendVerifyContent(cSDT_VERIFICATION_INFO.stFriendInfo.stUin.ullUid, cSDT_VERIFICATION_INFO.stFriendInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(cSDT_VERIFICATION_INFO.szVerificationInfo), CFriendModel.enVerifyDataSet.Friend, cSDT_VERIFICATION_INFO.stUserSource, 0);
				}
				else
				{
					this.model.FRData.ProcessOtherRequest(cSDT_VERIFICATION_INFO, true);
				}
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void OnMentor_RequestList(CSPkg msg)
		{
			SCPKG_MASTERREQ_LIST stMasterReqList = msg.stPkgData.stMasterReqList;
			if (stMasterReqList == null)
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)stMasterReqList.dwNum))
			{
				this.model.Add(CFriendModel.FriendType.MentorRequestList, stMasterReqList.astMasterReqList[num].stReqUserInfo, false);
				this.model.AddFriendVerifyContent(stMasterReqList.astMasterReqList[num].stReqUserInfo.stUin.ullUid, stMasterReqList.astMasterReqList[num].stReqUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stMasterReqList.astMasterReqList[num].szVerificationInfo), CFriendModel.enVerifyDataSet.Mentor, null, (int)stMasterReqList.astMasterReqList[num].bType);
				num++;
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public static CFriendModel.FriendType GetFriendType(int searchType)
		{
			if (searchType != 2 && searchType != 3)
			{
				return CFriendModel.FriendType.Recommend;
			}
			return CFriendModel.FriendType.MentorRecommend;
		}

		private void On_FriendSys_Friend_Recommand_List(CSPkg msg)
		{
			SCPKG_CMD_LIST_FREC stFRecRsp = msg.stPkgData.stFRecRsp;
			CFriendModel.FriendType friendType = CFriendContoller.GetFriendType((int)stFRecRsp.bType);
			this.model.Clear(friendType);
			if (stFRecRsp.dwResult == 0u)
			{
				int num = 0;
				while ((long)num < (long)((ulong)stFRecRsp.dwAcntNum))
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = stFRecRsp.astAcnts[num];
					if (friendType != CFriendModel.FriendType.GameFriend || this.model.GetInfo(CFriendModel.FriendType.GameFriend, cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId) == null)
					{
						this.model.Add(friendType, cOMDT_FRIEND_INFO, false);
					}
					num++;
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFRecRsp.dwResult), false, 1.5f, null, new object[0]);
			}
			if (this.view != null)
			{
				this.view.addFriendView.Refresh_Friend_Recommand_List(friendType);
			}
		}

		private void On_FriendSys_Friend_Search(CSPkg msg, enFriendSearchSource searchSource)
		{
			if (searchSource != enFriendSearchSource.FriendSystem)
			{
				return;
			}
			SCPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerRsp = msg.stPkgData.stFriendSearchPlayerRsp;
			if (stFriendSearchPlayerRsp.dwResult == 0u)
			{
				this.search_info = stFriendSearchPlayerRsp.stUserInfo;
				if (this.view != null)
				{
					this.view.Show_Search_Result(stFriendSearchPlayerRsp.stUserInfo);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendSearchPlayerRsp.dwResult), false, 1.5f, null, new object[0]);
			}
			this.view.addFriendView.Refresh_Friend_Recommand_List_Pos();
		}

		private void On_FriendSys_Friend_RequestBeFriend(CSPkg msg)
		{
			SCPKG_CMD_ADD_FRIEND stFriendAddRsp = msg.stPkgData.stFriendAddRsp;
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.instance.BroadCastEvent("Friend_SNSFriendList_Refresh");
			UT.ShowFriendNetResult(stFriendAddRsp.dwResult, UT.FriendResultType.RequestBeFriend);
		}

		private void On_FriendSys_Friend_Confrim(CSPkg msg, CFriendModel.FriendType friendType)
		{
			if (friendType != CFriendModel.FriendType.RequestFriend)
			{
				if (friendType != CFriendModel.FriendType.MentorRequestList)
				{
					return;
				}
				SCPKG_CONFIRM_MASTER_RSP stConfirmMasterRsp = msg.stPkgData.stConfirmMasterRsp;
				this.model.Remove(CFriendModel.FriendType.MentorRequestList, stConfirmMasterRsp.stUin);
			}
			else
			{
				SCPKG_CMD_ADD_FRIEND_CONFIRM stFriendAddConfirmRsp = msg.stPkgData.stFriendAddConfirmRsp;
				COMDT_FRIEND_INFO stUserInfo = stFriendAddConfirmRsp.stUserInfo;
				if (stFriendAddConfirmRsp.dwResult == 0u)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(string.Format(UT.GetText("Friend_Tips_BeFriend_Ok"), UT.Bytes2String(stUserInfo.szUserName)), false, 1.5f, null, new object[0]);
					this.model.Remove(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
					this.model.Add(CFriendModel.FriendType.GameFriend, stUserInfo, false);
					if (this.model.IsBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId))
					{
						this.model.RemoveFriendBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId);
					}
					this.model.AddFriendIntimacy(stUserInfo.stUin, stFriendAddConfirmRsp.stIntimacData);
					this.model.SortGameFriend();
					if (stFriendAddConfirmRsp.bFriendSrcType == 2)
					{
						this.model.friendRecruit.SetBeiZhaoMuZheRewardData(stUserInfo);
					}
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddConfirmRsp.dwResult), false, 1.5f, null, new object[0]);
					this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
				}
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void On_FriendSys_Friend_Deny(CSPkg msg)
		{
			SCPKG_CMD_ADD_FRIEND_DENY stFriendAddDenyRsp = msg.stPkgData.stFriendAddDenyRsp;
			if (stFriendAddDenyRsp.dwResult == 0u)
			{
				this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddDenyRsp.dwResult), false, 1.5f, null, new object[0]);
				this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
			}
		}

		private void On_FriendSys_Friend_Delete(CSPkg msg)
		{
			SCPKG_CMD_DEL_FRIEND stFriendDelRsp = msg.stPkgData.stFriendDelRsp;
			if (stFriendDelRsp.dwResult == 0u)
			{
				this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stFriendDelRsp.stUin);
			}
		}

		private void On_FriendSys_Friend_ADD_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_ADD stNtfFriendAdd = msg.stPkgData.stNtfFriendAdd;
			this.model.Remove(CFriendModel.FriendType.RequestFriend, stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId);
			if (this.model.IsBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId))
			{
				this.model.RemoveFriendBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId);
			}
			this.model.RemoveFriendVerifyContent(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend);
			this.model.AddFriendIntimacy(stNtfFriendAdd.stUserInfo.stUin, stNtfFriendAdd.stIntimacData);
			if (stNtfFriendAdd.bFriendSrcType == 2)
			{
				this.model.friendRecruit.SetZhaoMuZhe(stNtfFriendAdd.stUserInfo);
			}
			this.Add_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendAdd.stUserInfo);
		}

		private void On_FriendSys_Friend_Delete_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_DEL stNtfFriendDel = msg.stPkgData.stNtfFriendDel;
			this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendDel.stUin);
		}

		private void On_FriendSys_Friend_Request_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_REQUEST stNtfFriendRequest = msg.stPkgData.stNtfFriendRequest;
			if (stNtfFriendRequest == null)
			{
				return;
			}
			this.model.AddFriendVerifyContent(stNtfFriendRequest.stUserInfo.stUin.ullUid, stNtfFriendRequest.stUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stNtfFriendRequest.szVerificationInfo), CFriendModel.enVerifyDataSet.Friend, stNtfFriendRequest.stUserSource, 0);
			this.Add_And_Refresh(CFriendModel.FriendType.RequestFriend, stNtfFriendRequest.stUserInfo);
		}

		private void On_GuildSys_Guild_Invite_Success()
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void On_GuildSys_Guild_Recommend_Success()
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void Add_And_Refresh(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
		{
			this.model.Add(type, data, false);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			switch (type)
			{
			case CFriendModel.FriendType.Mentor:
				if (CFriendContoller.m_mentorInfo != null)
				{
					CFriendContoller.m_mentorInfo.stMaster = new CSDT_FRIEND_INFO();
					CFriendContoller.m_mentorInfo.stMaster.stFriendInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_INFO>(data);
					this.Handle_NoAskFor_Flag(CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.ullUid, CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId, CFriendContoller.m_mentorInfo.stMaster.bNoAskforFlag);
					masterRoleInfo.m_mentorInfo.szRoleName = CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.szUserName;
					CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
					if (profile != null && profile._mentorInfo != null)
					{
						profile._mentorInfo.szRoleName = data.szUserName;
					}
					Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
				}
				goto IL_2A3;
			case CFriendModel.FriendType.Apprentice:
			{
				if (CFriendContoller.m_mentorInfo == null)
				{
					goto IL_2A3;
				}
				bool flag = true;
				for (int i = 0; i < (int)CFriendContoller.m_mentorInfo.bStudentNum; i++)
				{
					if (CFriendContoller.m_mentorInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId == data.stUin.dwLogicWorldId && CFriendContoller.m_mentorInfo.astStudentList[i].stFriendInfo.stUin.ullUid == data.stUin.ullUid)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					CFriendContoller.m_mentorInfo.bStudentNum = CFriendContoller.m_mentorInfo.bStudentNum + 1;
					CFriendContoller.m_mentorInfo.astStudentList[(int)(CFriendContoller.m_mentorInfo.bStudentNum - 1)] = new CSDT_FRIEND_INFO();
					CFriendContoller.m_mentorInfo.astStudentList[(int)(CFriendContoller.m_mentorInfo.bStudentNum - 1)].stFriendInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_INFO>(data);
					this.Handle_NoAskFor_Flag(CFriendContoller.m_mentorInfo.astStudentList[(int)(CFriendContoller.m_mentorInfo.bStudentNum - 1)].stFriendInfo.stUin.ullUid, CFriendContoller.m_mentorInfo.astStudentList[(int)(CFriendContoller.m_mentorInfo.bStudentNum - 1)].stFriendInfo.stUin.dwLogicWorldId, CFriendContoller.m_mentorInfo.astStudentList[(int)(CFriendContoller.m_mentorInfo.bStudentNum - 1)].bNoAskforFlag);
					masterRoleInfo.m_mentorInfo.dwStudentNum += 1u;
					goto IL_2A3;
				}
				goto IL_2A3;
			}
			case CFriendModel.FriendType.MentorRequestList:
				this.view.RefreshMentorReqList();
				return;
			}
			if (type == CFriendModel.FriendType.GameFriend)
			{
				Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
			}
			IL_2A3:
			if (this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void Remove_And_Refresh(CFriendModel.FriendType type, COMDT_ACNT_UNIQ uniq)
		{
			this.model.Remove(type, uniq);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (type != CFriendModel.FriendType.Mentor)
			{
				if (type == CFriendModel.FriendType.Apprentice && CFriendContoller.m_mentorInfo != null)
				{
					int num = -1;
					for (int i = 0; i < (int)CFriendContoller.m_mentorInfo.bStudentNum; i++)
					{
						if (CFriendContoller.m_mentorInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId == uniq.dwLogicWorldId && CFriendContoller.m_mentorInfo.astStudentList[i].stFriendInfo.stUin.ullUid == uniq.ullUid)
						{
							num = i;
							break;
						}
					}
					if (num != -1)
					{
						for (int j = num; j < (int)(CFriendContoller.m_mentorInfo.bStudentNum - 1); j++)
						{
							CFriendContoller.m_mentorInfo.astStudentList[j] = CFriendContoller.m_mentorInfo.astStudentList[j + 1];
						}
						SCPKG_MASTERSTUDENT_INFO mentorInfo = CFriendContoller.m_mentorInfo;
						SCPKG_MASTERSTUDENT_INFO expr_E8 = mentorInfo;
						expr_E8.bStudentNum -= 1;
					}
				}
			}
			else if (CFriendContoller.m_mentorInfo != null)
			{
				CFriendContoller.m_mentorInfo.stMaster = new CSDT_FRIEND_INFO();
				masterRoleInfo.m_mentorInfo.ullMasterUid = 0uL;
				masterRoleInfo.m_mentorInfo.dwMasterLogicWorldID = 0u;
				CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
				if (profile != null && profile._mentorInfo != null)
				{
					profile._mentorInfo.szRoleName = null;
				}
				Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
			}
			if (this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void OnShareToggle(CUIEvent uievent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
			int num = 1;
			masterRoleInfo.snsSwitchBits = (uint)((ulong)masterRoleInfo.snsSwitchBits ^ (ulong)((long)num));
			FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC, (!flag) ? 1 : 0);
		}

		private void OnAddToggle(CUIEvent uievent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
			int num = 2;
			masterRoleInfo.snsSwitchBits = (uint)((ulong)masterRoleInfo.snsSwitchBits ^ (ulong)((long)num));
			FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND, (!flag) ? 1 : 0);
		}

		private void OnMentorRequestListClose(CUIEvent uievent)
		{
			GameObject widget = uievent.m_srcFormScript.GetWidget(1);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ);
			uint num = 8u;
			bool isOn = widget.GetComponent<Toggle>().get_isOn();
			if (flag != isOn)
			{
				if (isOn)
				{
					masterRoleInfo.snsSwitchBits |= num;
				}
				else
				{
					masterRoleInfo.snsSwitchBits &= ~num;
				}
			}
			FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ, flag ? 1 : 0);
		}

		private void OnFriend_Chat_Button(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			CFriendModel.FriendType friendTypeFromItemType = CFriendContoller.GetFriendTypeFromItemType((FriendShower.ItemType)uievent.m_eventParams.tag);
			COMDT_FRIEND_INFO info = this.model.GetInfo(friendTypeFromItemType, component.ullUid, component.dwLogicWorldID);
			if (info == null)
			{
				info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
				if (info == null)
				{
					return;
				}
			}
			Singleton<CChatController>.instance.Jump2FriendChat(info);
		}

		private void OnFriend_Gift_Button(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			if (!Utility.IsSamePlatformWithSelf(component.dwLogicWorldID))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("CS_PRESENTHEROSKIN_INVALID_PLAT", true, 1.5f, null, new object[0]);
				return;
			}
			bool isSns = false;
			if (this.model.GetInfo(CFriendContoller.GetFriendTypeFromItemType((FriendShower.ItemType)uievent.m_eventParams.tag), component.ullUid, component.dwLogicWorldID) == null)
			{
				isSns = true;
				if (this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID) == null)
				{
					return;
				}
			}
			Singleton<CMallSystem>.GetInstance().OpenGiftCenter(component.ullUid, component.dwLogicWorldID, isSns);
		}

		private void OnFriend_Show_Rule(CUIEvent uievent)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey(12u);
			if (this.view != null && this.view.CurTab == CFriendView.Tab.Mentor)
			{
				dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey(21u);
			}
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		private void OnCancleBlockBtn(CUIEvent uiEvent)
		{
			FriendShower component = uiEvent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			uiEvent.m_eventParams.commonUInt64Param1 = component.ullUid;
			uiEvent.m_eventParams.tagUInt = component.dwLogicWorldID;
			string blackName = this.model.GetBlackName(component.ullUid, component.dwLogicWorldID);
			string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Black_CancleBlockTip", new string[]
			{
				blackName
			}), new object[0]);
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Friend_CancleBlock_Ok, enUIEventID.Friend_CancleBlock_Cancle, uiEvent.m_eventParams, false);
		}

		private void OnCancleBlockBtnOK(CUIEvent evt)
		{
			ulong commonUInt64Param = evt.m_eventParams.commonUInt64Param1;
			uint tagUInt = evt.m_eventParams.tagUInt;
			FriendSysNetCore.Send_Cancle_Block(commonUInt64Param, tagUInt);
		}

		private void QQBox_OnClick(CUIEvent uievent)
		{
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				MonoSingleton<IDIPSys>.GetInstance().RequestQQBox();
			}
		}

		public bool IsLocationShareEnable()
		{
			return this.model != null && this.model.EnableShareLocation;
		}

		private void OnFriend_LBS_NoShare(CUIEvent evt)
		{
			this.model.EnableShareLocation = !this.model.EnableShareLocation;
			bool enableShareLocation = this.model.EnableShareLocation;
			FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_LBSSHARE, enableShareLocation ? 1 : 0);
			if (this.view != null)
			{
				this.view.SyncLBSShareBtnState();
				this.view.Refresh_List(this.view.CurTab);
			}
			if (!enableShareLocation)
			{
				FriendSysNetCore.Send_Clear_Location();
			}
			else if (!MonoSingleton<GPSSys>.instance.bGetGPSData)
			{
				MonoSingleton<GPSSys>.instance.StartGPS();
				Singleton<CUIManager>.instance.OpenTips("正在定位，请稍后...", false, 1.5f, null, new object[0]);
			}
			else
			{
				FriendSysNetCore.Send_Report_Clt_Location(MonoSingleton<GPSSys>.instance.iLongitude, MonoSingleton<GPSSys>.instance.iLatitude);
			}
		}

		public void SetFilter(uint value)
		{
			this.model.fileter = value;
			if (this.view != null)
			{
				this.view.SyncGenderToggleState();
				this.view.Refresh_List(this.view.CurTab);
			}
		}

		private void OnFriend_LBS_Nan(CUIEvent evt)
		{
			if (this.model.fileter != 1u)
			{
				this.SetFilter(this.model.NegFlag(this.model.fileter, 0));
			}
			else
			{
				this.SetFilter(this.model.fileter);
			}
		}

		private void OnFriend_LBS_Nv(CUIEvent evt)
		{
			if (this.model.fileter != 2u)
			{
				this.SetFilter(this.model.NegFlag(this.model.fileter, 1));
			}
			else
			{
				this.SetFilter(this.model.fileter);
			}
		}

		private void OnFriend_LBS_Refresh(CUIEvent evt)
		{
			int iLongitude = MonoSingleton<GPSSys>.instance.iLongitude;
			int iLatitude = MonoSingleton<GPSSys>.instance.iLatitude;
			bool isShowAlert = evt.m_eventParams.tag == 0;
			if (!CSysDynamicBlock.bFriendBlocked && (iLongitude == 0 || iLatitude == 0))
			{
				string text = Singleton<CTextManager>.instance.GetText("LBS_Location_Error");
				Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
				this.model.searchLBSZero = text;
				if (this.view != null && this.view.ifnoText != null)
				{
					this.view.ifnoText.set_text(text);
				}
			}
			FriendSysNetCore.Send_Search_LBS_Req(this.model.GetLBSGenterFilter(), iLongitude, iLatitude, isShowAlert);
			this.startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		private void OnFriend_LBS_CheckInfo(CUIEvent evt)
		{
			FriendShower component = evt.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int)component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		private void OnFriend_Room_AddFriend(CUIEvent evt)
		{
			if (evt.m_eventParams.commonUInt64Param1 == 0uL || evt.m_eventParams.commonUInt32Param1 == 0u)
			{
				return;
			}
			this.Open_Friend_Verify(evt.m_eventParams.commonUInt64Param1, evt.m_eventParams.commonUInt32Param1, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
		}

		private void OnIntimacyRela_Menu_Btn_Click(CUIEvent evt)
		{
			if (this.view != null && this.view.intimacyRelationView != null)
			{
				this.view.intimacyRelationView.Open();
			}
		}

		private void OnMentorTask_Btn_Click(CUIEvent evt)
		{
			if (this.view != null && this.view.mentorTaskView != null)
			{
				this.view.mentorTaskView.Open();
			}
		}

		private void On_OpenForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			this.view.OpenForm(uiEvent);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_FrientBtn);
			Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(5u, null, false);
		}

		private void On_CloseForm(CUIEvent uiEvent)
		{
			this.view.CloseForm();
		}

		private void On_OpenAddFriendForm(CUIEvent uiEvent)
		{
			this.view.OpenSearchForm(1);
		}

		public static uint GetMentorGradeLimit()
		{
			return GameDataMgr.globalInfoDatabin.GetDataByKey(271u).dwConfValue;
		}

		private FriendShower GetParentFriendShower(Transform node)
		{
			FriendShower component = node.GetComponent<FriendShower>();
			if (component != null)
			{
				return component;
			}
			if (node.parent != null)
			{
				return this.GetParentFriendShower(node.parent);
			}
			return null;
		}

		public void On_AddSomthing(CUIEvent evt, CFriendModel.FriendType friendType, bool ignoreSearchReasult = false)
		{
			COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = Singleton<CFriendContoller>.GetInstance().search_info;
			int bLogicGrade = (int)CLadderSystem.GetGradeDataByShowGrade((int)CFriendContoller.GetMentorGradeLimit()).bLogicGrade;
			enMentorState mentorState = CFriendContoller.GetMentorState(-1, null);
			if (this.model == null)
			{
				return;
			}
			if (evt.m_srcWidgetBelongedListScript == null && cOMDT_FRIEND_INFO != null && !ignoreSearchReasult)
			{
				int bLogicGrade2 = (int)CLadderSystem.GetGradeDataByShowGrade((int)cOMDT_FRIEND_INFO.stRankShowGrade.bGrade).bLogicGrade;
				this.Open_Friend_Verify(cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId, true, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, false);
			}
			else
			{
				FriendShower parentFriendShower = this.GetParentFriendShower(evt.m_srcWidget.transform);
				if (parentFriendShower == null)
				{
					return;
				}
				cOMDT_FRIEND_INFO = this.model.GetInfo(friendType, parentFriendShower.ullUid, parentFriendShower.dwLogicWorldID);
				if (cOMDT_FRIEND_INFO != null)
				{
					int bLogicGrade3 = (int)CLadderSystem.GetGradeDataByShowGrade((int)cOMDT_FRIEND_INFO.stRankShowGrade.bGrade).bLogicGrade;
				}
				else
				{
					CSDT_LBS_USER_INFO lBSUserInfo = this.model.GetLBSUserInfo(parentFriendShower.ullUid, parentFriendShower.dwLogicWorldID, CFriendModel.LBSGenderType.Both);
					if (lBSUserInfo != null)
					{
						cOMDT_FRIEND_INFO = lBSUserInfo.stLbsUserInfo;
					}
				}
				if (cOMDT_FRIEND_INFO != null)
				{
					if (this.view != null && this.view.CurTab == CFriendView.Tab.Friend_LBS)
					{
						this.Open_Friend_Verify(cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_LBS, -1, false);
					}
					else
					{
						this.Open_Friend_Verify(cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, false);
					}
				}
			}
		}

		private void On_AddFriend(CUIEvent evt)
		{
			if (evt.m_srcFormScript.m_formPath.Equals(CFriendContoller.AddFriendFormPath))
			{
				this.On_AddSomthing(evt, CFriendModel.FriendType.Recommend, false);
			}
			else if (evt.m_srcFormScript.m_formPath.Equals(CFriendContoller.FriendFormPath))
			{
				CFriendContoller.s_addViewtype = 1;
				this.On_AddSomthing(evt, (CFriendModel.FriendType)evt.m_eventParams.tag, true);
			}
		}

		private void On_AddMentor(CUIEvent evt)
		{
			this.On_AddSomthing(evt, CFriendModel.FriendType.MentorRecommend, false);
		}

		public void Open_Friend_Verify(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend, COM_ADD_FRIEND_TYPE addFriendType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, int useHeroId = -1, bool onlyAddFriend = true)
		{
			if (this.view != null && this.view.verficationView != null)
			{
				this.view.verficationView.Open(ullUid, dwLogicWorldId, bAddSearchFirend, addFriendType, useHeroId, onlyAddFriend);
			}
		}

		private void On_Accept_RequestFriend(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
			if (info == null)
			{
				return;
			}
			FriendSysNetCore.Send_Confrim_BeFriend(info.stUin);
		}

		private void On_Refuse_RequestFriend(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
			if (info == null)
			{
				return;
			}
			FriendSysNetCore.Send_DENY_BeFriend(info.stUin);
		}

		private void OnMentor_AcceptRequest(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			this.SendMentorRequestConfirmRefuse(component, COM_MASTERCOMFIRM_TYPE.COM_MASTERCOMFIRM_AGREE);
		}

		private void OnMentor_RefuseRequest(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			this.SendMentorRequestConfirmRefuse(component, COM_MASTERCOMFIRM_TYPE.COM_MASTERCOMFIRM_DENY);
		}

		public static int GetMentorServerState()
		{
			int result = -1;
			switch (CFriendContoller.GetMentorState(-1, null))
			{
			case enMentorState.IWantMentor:
			case enMentorState.IHasMentor:
				result = 3;
				break;
			case enMentorState.IWantApprentice:
			case enMentorState.IHasApprentice:
				result = 2;
				break;
			}
			return result;
		}

		public static int GetSrvMentorTypeByItemType(FriendShower.ItemType itemType)
		{
			switch (itemType)
			{
			case FriendShower.ItemType.AddMentor:
			case FriendShower.ItemType.Mentor:
				return 2;
			case FriendShower.ItemType.AddApprentice:
			case FriendShower.ItemType.Apprentice:
				return 3;
			default:
				return 0;
			}
		}

		private void SendMentorRequestConfirmRefuse(FriendShower shower, COM_MASTERCOMFIRM_TYPE confType)
		{
			if (shower == null)
			{
				return;
			}
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.MentorRequestList, shower.ullUid, shower.dwLogicWorldID);
			if (info == null)
			{
				return;
			}
			stFriendVerifyContent friendVerifyData = this.model.GetFriendVerifyData(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
			if (friendVerifyData == null)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5404u);
			cSPkg.stPkgData.stConfirmMasterReq.bConfirmType = (byte)confType;
			cSPkg.stPkgData.stConfirmMasterReq.stUin.ullUid = info.stUin.ullUid;
			cSPkg.stPkgData.stConfirmMasterReq.stUin.dwLogicWorldId = info.stUin.dwLogicWorldId;
			cSPkg.stPkgData.stConfirmMasterReq.bReqType = (byte)friendVerifyData.mentorType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void OnMentorInfoAdd(CSPkg msg)
		{
			SCPKG_MASTERSTUDENT_ADD stMasterStudentAdd = msg.stPkgData.stMasterStudentAdd;
			if (Utility.IsCanShowPrompt())
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_RelationshipMade", new string[]
				{
					UT.Bytes2String(stMasterStudentAdd.stDetailInfo.szUserName)
				}), false, 1.5f, null, new object[0]);
			}
			byte bType = stMasterStudentAdd.bType;
			if (bType != 2)
			{
				if (bType == 3)
				{
					this.Add_And_Refresh(CFriendModel.FriendType.Apprentice, stMasterStudentAdd.stDetailInfo);
				}
			}
			else
			{
				this.Add_And_Refresh(CFriendModel.FriendType.Mentor, stMasterStudentAdd.stDetailInfo);
			}
			this.Handle_NoAskFor_Flag(stMasterStudentAdd.stDetailInfo.stUin.ullUid, stMasterStudentAdd.stDetailInfo.stUin.dwLogicWorldId, stMasterStudentAdd.bNoAskforFlag);
		}

		public void OnMentorInfoRemove(CSPkg msg)
		{
			SCPKG_MASTERSTUDENT_DEL stMasterStudentDel = msg.stPkgData.stMasterStudentDel;
			byte bType = stMasterStudentDel.bType;
			if (bType != 2)
			{
				if (bType == 3)
				{
					this.Remove_And_Refresh(CFriendModel.FriendType.Apprentice, stMasterStudentDel.stUin);
				}
			}
			else
			{
				this.Remove_And_Refresh(CFriendModel.FriendType.Mentor, stMasterStudentDel.stUin);
			}
		}

		public void OnMentor_Reqest_NTF(CSPkg msg)
		{
			SCPKG_MASTERREQ_NTF stMasterReqNtf = msg.stPkgData.stMasterReqNtf;
			if (stMasterReqNtf == null)
			{
				return;
			}
			this.model.AddFriendVerifyContent(stMasterReqNtf.stInfo.stReqUserInfo.stUin.ullUid, stMasterReqNtf.stInfo.stReqUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stMasterReqNtf.stInfo.szVerificationInfo), CFriendModel.enVerifyDataSet.Mentor, null, (int)stMasterReqNtf.stInfo.bType);
			this.Add_And_Refresh(CFriendModel.FriendType.MentorRequestList, stMasterReqNtf.stInfo.stReqUserInfo);
		}

		private void On_DelFriend(CUIEvent evt)
		{
			FriendShower component = evt.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			evt.m_eventParams.commonUInt64Param1 = ((component != null) ? component.ullUid : 0uL);
			evt.m_eventParams.tagUInt = ((component != null) ? component.dwLogicWorldID : 0u);
			string text = UT.GetText("Friend_Tips_DelFriend");
			int tag = evt.m_eventParams.tag;
			if (tag == 8 || tag == 9)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("Mentor_DelConfirm");
			}
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Friend_DelFriend_OK, enUIEventID.Friend_DelFriend_Cancle, evt.m_eventParams, false);
		}

		private void On_DelFriend_OK(CUIEvent evt)
		{
			ulong commonUInt64Param = evt.m_eventParams.commonUInt64Param1;
			uint tagUInt = evt.m_eventParams.tagUInt;
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendContoller.GetFriendTypeFromItemType((FriendShower.ItemType)evt.m_eventParams.tag), commonUInt64Param, tagUInt);
			if (info == null)
			{
				return;
			}
			FriendSysNetCore.Send_Del_Friend(info, (FriendShower.ItemType)evt.m_eventParams.tag);
		}

		public static CFriendModel.FriendType GetFriendTypeFromItemType(FriendShower.ItemType itemType)
		{
			if (itemType == FriendShower.ItemType.Mentor)
			{
				return CFriendModel.FriendType.Mentor;
			}
			if (itemType != FriendShower.ItemType.Apprentice)
			{
				return CFriendModel.FriendType.GameFriend;
			}
			return CFriendModel.FriendType.Apprentice;
		}

		private void On_Friend_DelFriend_Cancle(CUIEvent evt)
		{
		}

		private void OnMentor_IWantMentor(CUIEvent evt)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint bLogicGrade = (uint)CLadderSystem.GetGradeDataByShowGrade((int)CFriendContoller.GetMentorGradeLimit()).bLogicGrade;
			switch (CFriendContoller.GetMentorState(-1, null))
			{
			case enMentorState.IWantMentor:
			case enMentorState.IWantApprentice:
			case enMentorState.IHasApprentice:
				this.view.OpenSearchForm(2);
				break;
			case enMentorState.IHasMentor:
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_HasMentor"), false, 1.5f, null, new object[0]);
				break;
			}
		}

		private void OnMentor_IWantApprentice(CUIEvent evt)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint bLogicGrade = (uint)CLadderSystem.GetGradeDataByShowGrade((int)CFriendContoller.GetMentorGradeLimit()).bLogicGrade;
			switch (CFriendContoller.GetMentorState(-1, null))
			{
			case enMentorState.IWantMentor:
			case enMentorState.IHasMentor:
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Grade2LowAsMentor"), false, 1.5f, null, new object[0]);
				break;
			case enMentorState.IWantApprentice:
			case enMentorState.IHasApprentice:
				this.view.OpenSearchForm(3);
				break;
			}
		}

		private void OnMentor_RequestListClick(CUIEvent evt)
		{
			this.view.OpenMentorRequestForm();
			this.view.Refresh();
		}

		private void OnMentorTabChange(CUIEvent evt)
		{
			this.view.OnMentorTabChange(evt);
		}

		public void OnMasterStudentInfo(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			SCPKG_MASTERSTUDENT_INFO stMasterStudentInfo = msg.stPkgData.stMasterStudentInfo;
			CFriendContoller.m_mentorInfo = stMasterStudentInfo;
			if (stMasterStudentInfo.stMaster.stFriendInfo.stUin.ullUid != 0uL)
			{
				COMDT_FRIEND_INFO stFriendInfo = stMasterStudentInfo.stMaster.stFriendInfo;
				COMDT_FRIEND_INFO expr_44 = stFriendInfo;
				expr_44.bStudentType |= 0;
				this.model.Add(CFriendModel.FriendType.Mentor, stMasterStudentInfo.stMaster.stFriendInfo, false);
				this.Handle_CoinSend_Data_Mentor(stMasterStudentInfo.stMaster);
				this.model.SetFriendGameState(stMasterStudentInfo.stMaster.stFriendInfo.stUin.ullUid, stMasterStudentInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE)stMasterStudentInfo.stMaster.bGameState, stMasterStudentInfo.stMaster.dwGameStartTime, string.Empty, false, false, 0u);
				this.Handle_NoAskFor_Flag(stMasterStudentInfo.stMaster.stFriendInfo.stUin.ullUid, stMasterStudentInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId, stMasterStudentInfo.stMaster.bNoAskforFlag);
			}
			for (int i = 0; i < (int)stMasterStudentInfo.bStudentNum; i++)
			{
				COMDT_FRIEND_INFO stFriendInfo2 = stMasterStudentInfo.astStudentList[i].stFriendInfo;
				COMDT_FRIEND_INFO expr_11C = stFriendInfo2;
				expr_11C.bStudentType |= 16;
				this.model.Add(CFriendModel.FriendType.Apprentice, stMasterStudentInfo.astStudentList[i].stFriendInfo, false);
				this.Handle_CoinSend_Data_Apprentice(stMasterStudentInfo.astStudentList[i]);
				this.model.SetFriendGameState(stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.ullUid, stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE)stMasterStudentInfo.astStudentList[i].bGameState, stMasterStudentInfo.astStudentList[i].dwGameStartTime, string.Empty, false, false, 0u);
				this.Handle_NoAskFor_Flag(stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.ullUid, stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId, stMasterStudentInfo.astStudentList[i].bNoAskforFlag);
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		public void OnMentor_GetAccountData(CSPkg msg)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (profile._mentorInfo != null)
			{
				profile._mentorInfo.dwMasterPoint = msg.stPkgData.stMasterAcntDataNtf.dwMasterPoint;
				profile._mentorInfo.dwFinishStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwFinishStudentNum;
				profile._mentorInfo.dwStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwProcessStudentNum;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.m_mentorInfo != null)
			{
				masterRoleInfo.m_mentorInfo.dwMasterPoint = msg.stPkgData.stMasterAcntDataNtf.dwMasterPoint;
				masterRoleInfo.m_mentorInfo.dwFinishStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwFinishStudentNum;
				masterRoleInfo.m_mentorInfo.dwStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwProcessStudentNum;
			}
			Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
		}

		public void OnMentor_GraduateNtf(CSPkg msg)
		{
			byte bType = msg.stPkgData.stGraduateNtf.bType;
			if (bType != 1)
			{
				if (bType == 2)
				{
					COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.Apprentice, msg.stPkgData.stGraduateNtf.stStudentUin.ullUid, msg.stPkgData.stGraduateNtf.stStudentUin.dwLogicWorldId);
					if (info != null)
					{
						info.bStudentType = ((info.bStudentType & 240) | 2);
					}
					CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
					if (profile != null && profile._mentorInfo != null)
					{
						profile._mentorInfo.dwFinishStudentNum += 1u;
					}
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null && masterRoleInfo.m_mentorInfo != null && masterRoleInfo.m_mentorInfo != null)
					{
						masterRoleInfo.m_mentorInfo.dwFinishStudentNum += 1u;
					}
				}
			}
			else if (CFriendContoller.m_mentorInfo != null)
			{
				CFriendContoller.m_mentorInfo.bStudentType = 2;
			}
			this.view.Refresh();
		}

		public void OnMentor_GetStudentList(CSPkg msg)
		{
			CFriendModel.FriendType type = CFriendModel.FriendType.Apprentice;
			int bListType = (int)msg.stPkgData.stGetStudentListRsp.bListType;
			enMentorRelationType enMentorRelationType = enMentorRelationType.schoolmate;
			CS_STUDENTLIST_TYPE cS_STUDENTLIST_TYPE = (CS_STUDENTLIST_TYPE)bListType;
			if (cS_STUDENTLIST_TYPE != CS_STUDENTLIST_TYPE.CS_STUDENTLIST_MINE)
			{
				if (cS_STUDENTLIST_TYPE == CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER)
				{
					type = CFriendModel.FriendType.Mentor;
					enMentorRelationType = enMentorRelationType.schoolmate;
				}
			}
			else
			{
				type = CFriendModel.FriendType.Apprentice;
				enMentorRelationType = enMentorRelationType.apprentice;
			}
			this.m_mentorListOff[bListType].m_type = (CS_STUDENTLIST_TYPE)bListType;
			this.m_mentorListOff[bListType].m_isEnd = (msg.stPkgData.stGetStudentListRsp.dwOver != 0u);
			int num = 0;
			while ((long)num < (long)((ulong)msg.stPkgData.stGetStudentListRsp.dwStudentNum))
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = msg.stPkgData.stGetStudentListRsp.astStudentList[num];
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO2 = cOMDT_FRIEND_INFO;
				COMDT_FRIEND_INFO expr_86 = cOMDT_FRIEND_INFO2;
				expr_86.bStudentType |= (byte)((byte)enMentorRelationType << 4);
				this.model.Add(type, cOMDT_FRIEND_INFO, false);
				num++;
			}
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
		}

		private void OnMentorGetMoreMentorList(CUIEvent uievt)
		{
			CS_STUDENTLIST_TYPE tag = (CS_STUDENTLIST_TYPE)uievt.m_eventParams.tag;
			FriendSysNetCore.SendGetStudentListReq(tag);
		}

		public bool HasMentor(SCPKG_MASTERSTUDENT_INFO info = null)
		{
			if (info != null && info != CFriendContoller.m_mentorInfo)
			{
				return info.stMaster.stFriendInfo.stUin.ullUid != 0uL;
			}
			ListView<COMDT_FRIEND_INFO> list = this.model.GetList(CFriendModel.FriendType.Mentor);
			if (list == null)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if ((list[i].bStudentType & 240) >> 4 != 2)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasApprentice(SCPKG_MASTERSTUDENT_INFO info = null)
		{
			if (info == null || info == CFriendContoller.m_mentorInfo)
			{
				ListView<COMDT_FRIEND_INFO> list = this.model.GetList(CFriendModel.FriendType.Apprentice);
				return list != null && list.Count != 0;
			}
			return info.bStudentNum != 0;
		}

		public static enMentorState GetMentorState(int iGrade = -1, SCPKG_MASTERSTUDENT_INFO info = null)
		{
			if (iGrade < 0)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (CFriendContoller.m_mentorInfo == null)
				{
					return enMentorState.None;
				}
				if (masterRoleInfo != null)
				{
					iGrade = (int)CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankHistoryHighestGrade).bLogicGrade;
				}
			}
			if (info == null)
			{
				info = CFriendContoller.m_mentorInfo;
			}
			if (iGrade < 0)
			{
				return enMentorState.None;
			}
			uint bLogicGrade = (uint)CLadderSystem.GetGradeDataByShowGrade((int)CFriendContoller.GetMentorGradeLimit()).bLogicGrade;
			if ((long)iGrade < (long)((ulong)bLogicGrade))
			{
				if (!Singleton<CFriendContoller>.GetInstance().HasMentor(info))
				{
					return enMentorState.IWantMentor;
				}
				return enMentorState.IHasMentor;
			}
			else
			{
				if (!Singleton<CFriendContoller>.GetInstance().HasApprentice(info))
				{
					return enMentorState.IWantApprentice;
				}
				return enMentorState.IHasApprentice;
			}
		}

		public int RefreshMentorTabData()
		{
			int num = 0;
			CFriendContoller.MentorTabMask = 0;
			if (CFriendContoller.s_mentorTabStr == null)
			{
				CFriendContoller.s_mentorTabStr = new string[2];
				for (int i = 0; i < 2; i++)
				{
					CFriendContoller.s_mentorTabStr[i] = Singleton<CTextManager>.GetInstance().GetText(CFriendContoller.s_mentorTabName[i]);
				}
			}
			if (CFriendContoller.m_mentorInfo == null)
			{
				return num;
			}
			if (this.HasMentor(null))
			{
				CFriendContoller.MentorTabMask |= 1;
				num++;
			}
			if (this.HasApprentice(null))
			{
				CFriendContoller.MentorTabMask |= 2;
				num++;
			}
			return num;
		}

		public void OnMentorApplyVerifyBoxRetrun(string str)
		{
			if (this.m_mentorSelectedUin == null)
			{
				return;
			}
			FriendSysNetCore.Send_Request_BeMentor(this.m_mentorSelectedUin.ullUid, this.m_mentorSelectedUin.dwLogicWorldId, CFriendContoller.s_addViewtype, str);
		}

		public string GetMentorStateString()
		{
			switch (CFriendContoller.GetMentorState(-1, null))
			{
			case enMentorState.IWantMentor:
			case enMentorState.IHasMentor:
				return Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor");
			case enMentorState.IWantApprentice:
			case enMentorState.IHasApprentice:
				return Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice");
			default:
				return null;
			}
		}

		private void On_Friend_SendCoin(CUIEvent uievent)
		{
			if (uievent.m_eventParams.tag == -1)
			{
				Debug.LogError("Error server friend type");
			}
			FriendSysNetCore.Send_FriendCoin(new COMDT_ACNT_UNIQ
			{
				ullUid = uievent.m_eventParams.commonUInt64Param1,
				dwLogicWorldId = (uint)uievent.m_eventParams.commonUInt64Param2
			}, (COM_FRIEND_TYPE)uievent.m_eventParams.tag);
		}

		private void On_SNSFriend_SendCoin(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint dwLogicWorldID = (uint)uievent.m_eventParams.commonUInt64Param2;
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.SNS, commonUInt64Param, dwLogicWorldID);
			if (info == null)
			{
				return;
			}
			FriendSysNetCore.Send_FriendCoin(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
		}

		private void GetFriendUid(CUIEvent uiEvent, ref ulong uid, ref int logicWorldId)
		{
			Transform parent = uiEvent.m_srcWidget.transform.parent.parent.parent;
			FriendShower component = parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			CFriendView.Tab selectedTab = this.view.GetSelectedTab();
			COMDT_FRIEND_INFO info;
			if (selectedTab != CFriendView.Tab.Mentor)
			{
				CFriendModel.FriendType type = (this.view.GetSelectedTab() == CFriendView.Tab.Friend_SNS) ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
				info = this.model.GetInfo(type, component.ullUid, component.dwLogicWorldID);
			}
			else
			{
				info = this.model.GetInfo(CFriendModel.FriendType.Mentor, component.ullUid, component.dwLogicWorldID);
				if (info == null)
				{
					info = this.model.GetInfo(CFriendModel.FriendType.Apprentice, component.ullUid, component.dwLogicWorldID);
				}
			}
			if (info == null)
			{
				return;
			}
			uid = info.stUin.ullUid;
			logicWorldId = (int)info.stUin.dwLogicWorldId;
		}

		private void On_Friend_InviteGuild(CUIEvent uiEvent)
		{
			ulong arg = 0uL;
			int arg2 = 0;
			this.GetFriendUid(uiEvent, ref arg, ref arg2);
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, int>("Guild_Invite", arg, arg2);
		}

		private void On_Friend_RecommendGuild(CUIEvent uiEvent)
		{
			ulong arg = 0uL;
			int arg2 = 0;
			this.GetFriendUid(uiEvent, ref arg, ref arg2);
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, int>("Guild_Recommend", arg, arg2);
		}

		private void On_Friend_CheckInfo(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int)component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		private void On_FriendRecommend_CheckInfo(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int)component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		private void OnMentor_OpenPrivilegePage(CUIEvent uievt)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.MentorPrivilegeFormPath, false, true);
			CFriendView.MentorPrivilege_SetMainList(cUIFormScript);
			CFriendView.MentorPrivilege_Refresh(cUIFormScript, 1);
			this.m_mentorPrivilegePage = 1;
			GameObject widget = cUIFormScript.GetWidget(5);
			widget.CustomSetActive(false);
		}

		private void OnMentor_PrivilegeLeftClick(CUIEvent uievt)
		{
			CUIFormScript srcFormScript = uievt.m_srcFormScript;
			CUIListScript component = srcFormScript.GetWidget(8).GetComponent<CUIListScript>();
			GameObject widget = srcFormScript.GetWidget(5);
			GameObject widget2 = srcFormScript.GetWidget(6);
			this.m_mentorPrivilegePage--;
			if (this.m_mentorPrivilegePage <= 1)
			{
				this.m_mentorPrivilegePage = 1;
				widget.CustomSetActive(false);
			}
			else
			{
				widget.CustomSetActive(true);
			}
			widget2.CustomSetActive(true);
			component.MoveElementInScrollArea(this.m_mentorPrivilegePage - 1, false);
			CFriendView.MentorPrivilege_Refresh(srcFormScript, this.m_mentorPrivilegePage);
		}

		private void OnMentor_PrivilegeRightClick(CUIEvent uievt)
		{
			CUIFormScript srcFormScript = uievt.m_srcFormScript;
			CUIListScript component = srcFormScript.GetWidget(8).GetComponent<CUIListScript>();
			int count = GameDataMgr.famousMentorDatabin.count;
			GameObject widget = srcFormScript.GetWidget(5);
			GameObject widget2 = srcFormScript.GetWidget(6);
			this.m_mentorPrivilegePage++;
			if (this.m_mentorPrivilegePage >= count)
			{
				this.m_mentorPrivilegePage = count;
				widget2.CustomSetActive(false);
			}
			else
			{
				widget2.CustomSetActive(true);
			}
			widget.CustomSetActive(true);
			component.MoveElementInScrollArea(this.m_mentorPrivilegePage - 1, false);
			CFriendView.MentorPrivilege_Refresh(srcFormScript, this.m_mentorPrivilegePage);
		}

		private void OnMentor_PrivilegeListEnable(CUIEvent uievt)
		{
			CUIFormScript srcFormScript = uievt.m_srcFormScript;
			CFriendView.MentorPrivilegeMainList_OnEnable(uievt);
		}

		private void On_Friend_List_ElementEnable(CUIEvent uievent)
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.On_List_ElementEnable(uievent);
			}
		}

		private void On_SingleListElementEnable(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			FriendShower component = uievent.m_srcWidget.GetComponent<FriendShower>();
			if (component != null)
			{
				enFriendSingleListType singleListType = this.singleListType;
				if (singleListType != enFriendSingleListType.requestList)
				{
					if (singleListType == enFriendSingleListType.blackList)
					{
						List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
						if (srcWidgetIndexInBelongedList < blackList.get_Count())
						{
							CFriendModel.stBlackName stBlackName = blackList.get_Item(srcWidgetIndexInBelongedList);
							if (component != null)
							{
								UT.ShowBlackListData(ref stBlackName, component);
							}
						}
					}
				}
				else
				{
					COMDT_FRIEND_INFO infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.RequestFriend, srcWidgetIndexInBelongedList);
					UT.ShowFriendData(infoAtIndex, component, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend, uievent.m_srcFormScript, true);
				}
			}
		}

		private void On_MentorRequestListEnable(CUIEvent uievt)
		{
			COMDT_FRIEND_INFO infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.MentorRequestList, uievt.m_srcWidgetIndexInBelongedList);
			FriendShower component = uievt.m_srcWidget.GetComponent<FriendShower>();
			UT.ShowFriendData(infoAtIndex, component, FriendShower.ItemType.MentorRequest, false, CFriendModel.FriendType.MentorRequestList, uievt.m_srcFormScript, true);
		}

		private void On_Friend_Invite_SNS_Friend(CUIEvent uievent)
		{
			if (this.view != null && this.view.IsActive())
			{
				this.view.On_Friend_Invite_SNS_Friend(uievent);
			}
		}

		private void On_Friend_Share_SendCoin(CUIEvent uievent)
		{
			try
			{
				if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
				{
					string openId = uievent.m_eventParams.snsFriendEventParams.openId;
					Singleton<ApolloHelper>.instance.ShareSendHeart(openId, Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_1"), Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_2"), ShareSys.SNS_SHARE_SEND_HEART);
					Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_7", true, 1.5f, null, new object[0]);
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception in On_Friend_Share_SendCoin, {0}", new object[]
				{
					ex.get_Message()
				});
			}
		}

		public void ShareTo_SNSFriend_ReCall(string openId)
		{
			if (!MonoSingleton<ShareSys>.instance.IsInstallPlatform())
			{
				return;
			}
			string text = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_11");
			string text2 = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_12");
			Singleton<ApolloHelper>.instance.ShareInviteFriend(openId, text, text2, ShareSys.SNS_SHARE_RECALL_FRIEND);
			Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_13", true, 1.5f, null, new object[0]);
		}

		private void On_Friend_SNS_ReCall(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
			if (info == null)
			{
				return;
			}
			FriendSysNetCore.ReCallSnsFriend(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
			if (!CFriendModel.IsOnSnsSwitch(info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC))
			{
				this.ShareTo_SNSFriend_ReCall(Utility.UTF8Convert(info.szOpenId));
			}
		}

		private void On_Friend_OB_Click(CUIEvent uievent)
		{
			FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
			if (component == null)
			{
				return;
			}
			COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
			if (info == null)
			{
				info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID);
			}
			if (info == null)
			{
				return;
			}
			Singleton<COBSystem>.instance.OBFriend(info.stUin);
		}

		private void OnGPSDataGot(int longitude, int latitude)
		{
			FriendSysNetCore.Send_Report_Clt_Location(longitude, latitude);
			FriendSysNetCore.Send_Search_LBS_Req((byte)Singleton<CFriendContoller>.instance.model.fileter, longitude, latitude, true);
		}

		private void OnNewDayNtf()
		{
			this.model.SnsReCallData.Clear();
			this.model.HeartData.Clear();
			if (this.view != null && this.view.IsActive())
			{
				this.view.Refresh();
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.getFriendCoinCnt = 0;
				Singleton<CMailSys>.instance.UpdateUnReadNum();
			}
		}

		public void On_SCID_CMD_BLACKLIST(CSPkg msg)
		{
			SCPKG_CMD_BLACKLIST stBlackListProfile = msg.stPkgData.stBlackListProfile;
			for (int i = 0; i < (int)stBlackListProfile.wBlackListNum; i++)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = stBlackListProfile.astBlackList[i];
				if (cOMDT_FRIEND_INFO != null)
				{
					this.model.AddFriendBlack(cOMDT_FRIEND_INFO);
				}
			}
		}

		public void OnSCPKG_CMD_DEFRIEND(CSPkg msg)
		{
			SCPKG_CMD_DEFRIEND stDeFriendRsp = msg.stPkgData.stDeFriendRsp;
			if (stDeFriendRsp.dwResult == 0u)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO;
				COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO;
				this.model.GetUser(stDeFriendRsp.stUin.ullUid, stDeFriendRsp.stUin.dwLogicWorldId, out cOMDT_FRIEND_INFO, out cOMDT_CHAT_PLAYER_INFO);
				if (cOMDT_FRIEND_INFO == null)
				{
					if (cOMDT_CHAT_PLAYER_INFO != null)
					{
						this.model.AddFriendBlack(cOMDT_CHAT_PLAYER_INFO, stDeFriendRsp.bGender, stDeFriendRsp.dwLastLoginTime);
						if (this.view != null && this.view.IsActive())
						{
							this.view.Refresh();
						}
					}
					else
					{
						DebugHelper.Assert(false, string.Concat(new object[]
						{
							"---black 找到不到 ulluid:",
							stDeFriendRsp.stUin.ullUid,
							",worldID:",
							stDeFriendRsp.stUin.dwLogicWorldId,
							",对应的玩家数据"
						}));
					}
				}
				else
				{
					this.model.AddFriendBlack(cOMDT_FRIEND_INFO);
					if (this.view != null && this.view.IsActive())
					{
						this.view.Refresh();
					}
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerBlock_Success);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stDeFriendRsp.dwResult), false, 1.5f, null, new object[0]);
			}
		}

		public void OnSCPKG_CMD_CANCEL_DEFRIEND(CSPkg msg)
		{
			SCPKG_CMD_CANCEL_DEFRIEND stCancelDeFriendRsp = msg.stPkgData.stCancelDeFriendRsp;
			if (stCancelDeFriendRsp.dwResult == 0u)
			{
				this.model.RemoveFriendBlack(stCancelDeFriendRsp.stUin.ullUid, stCancelDeFriendRsp.stUin.dwLogicWorldId);
				if (this.view != null && this.view.IsActive())
				{
					this.view.Refresh();
				}
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips("---black OnSCPKG_CMD_CANCEL_DEFRIEND error code:" + stCancelDeFriendRsp.dwResult, false, 1.5f, null, new object[0]);
			}
		}

		public void OnSCPKG_CMD_BLACKFORFRIENDREQ(CSPkg msg)
		{
		}
	}
}

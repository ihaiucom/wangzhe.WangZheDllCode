using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using MiniJSON;
using ResData;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameSystem
{
	public class CGuildInfoView : Singleton<CGuildInfoView>
	{
		public enum Tab
		{
			GuildInfo,
			GuildMember
		}

		public enum enGuildInfoFormWidget
		{
			InfoPanel,
			MemberPanel,
			Tab_List,
			FacePanel_Guild_Head_Image,
			FacePanel_Guild_Name_Text,
			FacePanel_Guild_Chairman_Text,
			MemberPanel_Guild_Member_Count_Text,
			MemberPanel_Guild_Bulletin_Text,
			MemberPanel_Setting_Button,
			MemberPanel_Member_List,
			MemberPanel_Modify_Bulletin_Image,
			MemberPanel_Add_Member_Limit_Image,
			InfoPanel_Profit_Text,
			MemberPanel_Apply_List_Button,
			MemberPanel_StarLevel_Panel,
			MemberPanel_QQGroup_Info_Text,
			MemberPanel_QQGroup_Button,
			InfoPanel_Sign_Button,
			MemberPanel_QQGroup_Button_Text,
			MemberPanel_QQGroup_Panel,
			InfoPanel_Guild_Match_Button,
			MemberPanel_Mail_Button,
			FacePanel,
			InfoPanel_Rankpoint_Member_List,
			InfoPanel_Rankpoint_Self_Panel,
			InfoPanel_Grade_Icon_Image,
			InfoPanel_Grade_Name_Text,
			InfoPanel_Week_Award_Panel,
			InfoPanel_Season_Award_Panel,
			InfoPanel_Personal_Best_Num_Text,
			InfoPanel_Season_Clear_Time_Text,
			InfoPanel_Guild_Season_Point_Num_Text,
			InfoPanel_Guild_Week_Rank_Num_Text,
			InfoPanel_Guild_Week_Not_In_Rank_Text,
			MemberPanel_Sort_Position_Button_Image,
			MemberPanel_Sort_WeekRankpoint_Button_Image,
			MemberPanel_Sort_SeasonRankpoint_Button_Image,
			MemberPanel_Sort_OnlineStatus_Button_Image,
			MemberPanel_Recruit_Button,
			MemberPanel_Recruit_CD_Timer,
			MemberPanel_Recruit_Button_Text,
			InfoPanel_Guild_Match_Start_Flag_Image,
			RankpointParticleMask,
			MemberParticleMask
		}

		public enum enGuildExchangePositionFormWidget
		{
			ViceChairman_List
		}

		public enum enGuildRankPointRankFormWidget
		{
			Guild_Icon_Image,
			Guild_Name_Text,
			Point_Num_Text,
			Member_Count_Text,
			Award_Panel,
			Rank_Tab_List,
			Rank_List,
			Rank_Panel,
			Season_Rank_Tab_List,
			Star_Panel,
			Grade_Image
		}

		public enum enGuildRankpointRankListTab
		{
			CurrentWeekRank,
			LastWeekRank,
			SeasonRank
		}

		public enum enGuildRankpointSeasonRankListTab
		{
			SelfRank,
			BestRank
		}

		public enum enGuildApplyListFormWidget
		{
			Apply_Member_List,
			Close_Red_Dot_Toggle
		}

		public enum enGuildSettingFormWidget
		{
			Guild_Icon_Image,
			Need_Approval_Slider,
			Need_Approval_Slider_Front_Text,
			GuildIconIdText,
			Change_Name_Button,
			Guild_Name_Text,
			Join_Level_Text,
			Join_Grade_Index_Text,
			Join_Grade_Name_Text,
			Join_Level_Up_Button,
			Join_Level_Down_Button,
			Join_Grade_Up_Button,
			Join_Grade_Down_Button,
			Platform_Group_Panel
		}

		public enum enGuildIconFormWidget
		{
			Icon_List
		}

		public enum enGuildPreviewFormWidget
		{
			Guild_List,
			Guild_Chairman_Panel,
			Guild_Bulletin_Panel,
			Guild_Operation_Panel,
			Guild_Search_Guild_Input,
			Guild_Chairman_Name_Text,
			Guild_Bulletin_Text,
			Guild_Chairman_Head_Image,
			Guild_Chairman_Level_Text
		}

		public enum enGuildSignSuccessFormWidget
		{
			Sign_Success_Text
		}

		public enum enGuildLogFormWidget
		{
			Content_Text,
			EmptyNode_Panel
		}

		private enum enGuildMemberListSortType
		{
			Default,
			PositionDesc,
			WeekRankpointDesc,
			SeasonRankpointDesc,
			OnlineStatus
		}

		public const string GuildInfoPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Info.prefab";

		public const string GuildExchangePositionPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab";

		public const string GuildRankPointRankPrefabPath = "UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab";

		public const string GuildApplyListPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab";

		public const string GuildApplyListCloseRedDotPrefKey = "Guild_ApplyList_CloseRedDot";

		public const string GuildSettingPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Setting.prefab";

		public const string GuildIconPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Icon.prefab";

		public const string GuildPreviewPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Preview.prefab";

		public const string GuildSignSuccessPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Sign_Success.prefab";

		public const string GuildLogPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Log.prefab";

		private const int GuildRankpointRuleTextIndex = 9;

		private CUIFormScript m_form;

		private CGuildInfoView.Tab m_curTab;

		private CGuildModel m_Model;

		private static Color s_Text_Color_Btn_Sign_Default = new Color(0.5882353f, 0.6784314f, 0.996078432f);

		private static float s_rankpointRankListWidgetAnchoredPositionY;

		private static float s_rankpointRankListWidgetSizeDeltaY;

		private CGuildInfoView.enGuildMemberListSortType m_curGuildMemberListSortType;

		private int m_lastSendReruitTime;

		private bool m_needSendRemindMsg;

		public bool NeedAutoOp;

		public CGuildInfoView.Tab CurTab
		{
			get
			{
				return this.m_curTab;
			}
			set
			{
				this.m_curTab = value;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CGuildInfoView.Tab>("Guild_Info_Tab_Change", this.m_curTab);
			}
		}

		public override void Init()
		{
			this.m_Model = Singleton<CGuildModel>.GetInstance();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Info_View_Change_Tab, new CUIEventManager.OnUIEventHandler(this.On_Tab_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Member_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_Select));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Add_Friend, new CUIEventManager.OnUIEventHandler(this.On_Guild_Add_Friend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Open, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Open));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Open_Icon_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Open_Icon_Form));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Guild_Icon_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Guild_Icon_Selected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Modify_Guild_Bulletin_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Modify_Guild_Bulletin_Form));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Confirm_Modify_Guild_Bulletin, new CUIEventManager.OnUIEventHandler(this.On_Guild_Confirm_Modify_Guild_Bulletin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Extend_Member_Limit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Extend_Member_Limit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Extend_Member_Limit_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Extend_Member_Limit_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Down_Join_Level_Num, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Down_Join_Level_Num));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Up_Join_Level_Num, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Up_Join_Level_Num));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Down_Join_Grade, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Down_Join_Grade));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Up_Join_Grade, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Up_Join_Grade));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Apply_List_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Apply_List_Form));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Application_Pass, new CUIEventManager.OnUIEventHandler(this.On_Guild_Application_Pass));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Application_Reject, new CUIEventManager.OnUIEventHandler(this.On_Guild_Application_Reject));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_ApplyList_Save_Pref, new CUIEventManager.OnUIEventHandler(this.On_Guild_ApplyList_Save_Pref));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Apply_List_Element_Head_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_Apply_List_Element_Head_Selected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Apply_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Apply_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Need_Approval_Slider_Value_Changed, new CUIEventManager.OnUIEventHandler(this.On_Guild_Need_Approval_Slider_Value_Changed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recommend_Invite, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recommend_Invite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recommend_Reject, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recommend_Reject));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Quit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Quit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Quit_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Quit_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Time_Up, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Time_Up));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Appoint_Vice_Chairman, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionAppointViceChairman));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Appoint_Vice_Chairman_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionAppointViceChairmanConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Fire_Member, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionFireMember));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Fire_Member_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionFireMemberConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Chairman_Transfer, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionChairmanTransfer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Chairman_Transfer_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionChairmanTransferConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Real_Transfer_Chairman, new CUIEventManager.OnUIEventHandler(this.On_Guild_Real_Transfer_Chairman));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Help, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Help));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Enter_Matching, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Enter_Matching));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Rank_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Rank_List_Tab_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Season_Rank_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Season_Rank_List_Tab_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Open_Rankpoint_Rank_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Open_Rankpoint_Rank_Form));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Member_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Member_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Rank_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Rank_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Request_Ranking_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Request_Ranking_List));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Guild_List_Element_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Guild_List_Element_Select));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Guild_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Request_More_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Request_More_Guild_List));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Preview_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Accept_Invite, new CUIEventManager.OnUIEventHandler(this.On_Guild_Accept_Invite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Sign, new CUIEventManager.OnUIEventHandler(this.On_Guild_Sign));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Log_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Log_Form));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Position_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Position_Desc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Week_Rankpoint_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Week_Rankpoint_Desc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Season_Rankpoint_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Season_Rankpoint_Desc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Online_Status_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Online_Status_Desc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recruit_Send_Recruit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recruit_Send_Recruit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recruit_Send_Recruit_CD_Timeout, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recruit_Send_Recruit_CD_Timeout));
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService != null)
			{
				iApolloSnsService.onQueryGroupInfoEvent += new OnQueryGroupInfoNotifyHandle(this.OnQueryPlatformGroupInfoNotify);
				iApolloSnsService.onCreateWXGroupEvent += new OnCreateWXGroupNotifyHandle(this.OnCreateWXGroupNotify);
				iApolloSnsService.onJoinWXGroupEvent += new OnJoinWXGroupNotifyHandle(this.OnJoinWXGroupNotify);
				Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloGroupResult>("Guild_WXGroup_UnLinkWXGroupNotify", new Action<ApolloGroupResult>(this.OnUnLinkWXGroupNotify));
				iApolloSnsService.onBindGroupEvent += new OnBindGroupNotifyHandle(this.OnBindQQGroupNotify);
				iApolloSnsService.onUnBindGroupEvent += new OnUnbindGroupNotifyHandle(this.OnUnBindQQGroupNotify);
				iApolloSnsService.onQueryGroupKeyEvent += new OnQueryGroupKeyNotifyHandle(this.OnQueryQQGroupKeyNotify);
			}
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_JoinPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_JoinPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_BindPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_BindPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_UnBindPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_UnBindPlatformGroupConfirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindPlatformGroupConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_RemindPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_RemindPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guide_Join_PlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guide_Join_PlatformGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guide_Bind_PlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guide_Bind_PlatformGroup));
			Singleton<EventRouter>.GetInstance().AddEventHandler<bool>("Guild_SendGuildMatchInviteToPlatformGroup_Result", new Action<bool>(this.OnSendGuildMatchInviteToPlatformGroupNotify));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", new Action<CGuildSystem.enPlatformGroupStatus, bool>(this.OnPlatformGroupStatusChange));
		}

		public void Clear()
		{
			this.m_needSendRemindMsg = false;
			this.NeedAutoOp = false;
		}

		public override void UnInit()
		{
			this.m_Model = null;
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService != null)
			{
				iApolloSnsService.onQueryGroupInfoEvent -= new OnQueryGroupInfoNotifyHandle(this.OnQueryPlatformGroupInfoNotify);
				iApolloSnsService.onCreateWXGroupEvent -= new OnCreateWXGroupNotifyHandle(this.OnCreateWXGroupNotify);
				iApolloSnsService.onJoinWXGroupEvent -= new OnJoinWXGroupNotifyHandle(this.OnJoinWXGroupNotify);
				Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloGroupResult>("Guild_WXGroup_UnLinkWXGroupNotify", new Action<ApolloGroupResult>(this.OnUnLinkWXGroupNotify));
				iApolloSnsService.onBindGroupEvent -= new OnBindGroupNotifyHandle(this.OnBindQQGroupNotify);
				iApolloSnsService.onUnBindGroupEvent -= new OnUnbindGroupNotifyHandle(this.OnUnBindQQGroupNotify);
				iApolloSnsService.onQueryGroupKeyEvent -= new OnQueryGroupKeyNotifyHandle(this.OnQueryQQGroupKeyNotify);
			}
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_JoinPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_JoinPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_BindPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_BindPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_UnBindPlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindPlatformGroup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_UnBindPlatformGroupConfirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindPlatformGroupConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_Guide_Join_PlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guide_Join_PlatformGroup));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Guild_Guide_Bind_PlatformGroup, new CUIEventManager.OnUIEventHandler(this.On_Guide_Bind_PlatformGroup));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>("Guild_SendGuildMatchInviteToPlatformGroup_Result", new Action<bool>(this.OnSendGuildMatchInviteToPlatformGroupNotify));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", new Action<CGuildSystem.enPlatformGroupStatus, bool>(this.OnPlatformGroupStatusChange));
		}

		private void On_Guild_Member_Select(CUIEvent uiEvent)
		{
			this.m_Model.CurrentSelectedMemberInfo = this.GetSelectedMemberInfo();
			if (this.m_Model.CurrentSelectedMemberInfo == null)
			{
				return;
			}
			if (this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid, this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.dwLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Base_Info);
			}
			else
			{
				bool isShowGuildAppointViceChairmanBtn = CGuildSystem.HasAppointViceChairmanAuthority() && CGuildSystem.CanBeAppointedToViceChairman(this.m_Model.CurrentSelectedMemberInfo.enPosition);
				bool isShowGuildTransferPositionBtn = CGuildSystem.HasTransferPositionAuthority();
				bool isShowGuildFireMemberBtn = CGuildSystem.HasFireMemberAuthority(this.m_Model.CurrentSelectedMemberInfo.enPosition);
				Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid, this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.dwLogicWorldId, isShowGuildAppointViceChairmanBtn, isShowGuildTransferPositionBtn, isShowGuildFireMemberBtn);
			}
		}

		private void On_Guild_Add_Friend(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			Singleton<CFriendContoller>.instance.Open_Friend_Verify(commonUInt64Param, commonUInt32Param, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
		}

		public void On_Tab_Change(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component != null)
			{
				int selectedIndex = component.GetSelectedIndex();
				this.CurTab = (CGuildInfoView.Tab)selectedIndex;
			}
			this.InitPanel();
		}

		private void On_Guild_Guild_Setting_Open(CUIEvent uiEvent)
		{
			if (!CGuildSystem.HasManageAuthority())
			{
				return;
			}
			this.OpenSettingForm();
		}

		private void On_Guild_Guild_Setting_Open_Icon_Form(CUIEvent uiEvent)
		{
			this.OpenGuildIconForm();
		}

		private void On_Guild_Guild_Setting_Guild_Icon_Selected(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int num = cUIListScript.GetSelectedIndex();
			if (num == -1)
			{
				num = 0;
			}
			CUIListElementScript elemenet = cUIListScript.GetElemenet(num);
			if (elemenet == null)
			{
				return;
			}
			Text component = elemenet.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>();
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
			if (form != null)
			{
				Text component2 = form.GetWidget(3).GetComponent<Text>();
				component2.text = component.text;
				Image component3 = form.GetWidget(0).GetComponent<Image>();
				component3.SetSprite(elemenet.transform.Find("imgIcon").GetComponent<Image>());
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Guild_Setting_Guild_Icon_Selected(): settingForm is null!!!");
			}
			srcFormScript.Close();
		}

		private void On_Guild_Guild_Setting_Confirm(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			Text component = srcFormScript.GetWidget(3).GetComponent<Text>();
			uint arg = 0u;
			if (component != null)
			{
				try
				{
					arg = Convert.ToUInt32(component.text);
				}
				catch (Exception ex)
				{
					DebugHelper.Assert(false, "Failed convert form {0} to uint32, On_Guild_Guild_Setting_Confirm, Exception={1}", new object[]
					{
						component.text,
						ex.Message
					});
				}
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("Guild_Setting_Modify_Icon", arg);
			Slider component2 = srcFormScript.GetWidget(1).GetComponent<Slider>();
			uint num = Convert.ToUInt32((int)component2.value == 0);
			uint num2 = 4294967295u;
			num2 &= num;
			Text component3 = srcFormScript.GetWidget(6).GetComponent<Text>();
			Text component4 = srcFormScript.GetWidget(7).GetComponent<Text>();
			int arg2;
			if (!int.TryParse(component3.text, out arg2))
			{
				DebugHelper.Assert(false, "等级限制文本不是一个数字：{0}", new object[]
				{
					component3.text
				});
			}
			int arg3;
			if (!int.TryParse(component4.text, out arg3))
			{
				DebugHelper.Assert(false, "段位限制文本不是一个数字：{0}", new object[]
				{
					component4.text
				});
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, int, int>("Request_Guild_Setting_Modify", num2, arg2, arg3);
		}

		private void On_Guild_Open_Modify_Guild_Bulletin_Form(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
			Singleton<CUIManager>.GetInstance().OpenEditForm(Singleton<CTextManager>.GetInstance().GetText("Guild_Modify_Bulletin"), component.text, enUIEventID.Guild_Confirm_Modify_Guild_Bulletin);
		}

		private void On_Guild_Confirm_Modify_Guild_Bulletin(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			Text component = srcFormScript.GetWidget(1).GetComponent<Text>();
			string text = CUIUtility.RemoveEmoji(component.text).Trim();
			if (string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Bulletin_Empty", true, 1.5f, null, new object[0]);
				return;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_Modify_Bulletin", text);
		}

		private void On_Guild_Extend_Member_Limit(CUIEvent uiEvent)
		{
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "4";
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "4";
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
			int bLevel = (int)this.m_Model.CurrentGuildInfo.briefInfo.bLevel;
			if (CGuildHelper.IsGuildMaxLevel(bLevel))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Member_Count_Reach_Limit", true, 1.5f, null, new object[0]);
				return;
			}
			int upgradeCostDianQuanByLevel = CGuildHelper.GetUpgradeCostDianQuanByLevel(bLevel);
			int maxGuildMemberCountByLevel = CGuildHelper.GetMaxGuildMemberCountByLevel(bLevel + 1);
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Extend_Member_Limit_Confirm_Msg", new string[]
			{
				upgradeCostDianQuanByLevel.ToString(),
				maxGuildMemberCountByLevel.ToString()
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Extend_Member_Limit_Confirm, enUIEventID.None, false);
		}

		private void On_Guild_Extend_Member_Limit_Confirm(CUIEvent uiEvent)
		{
			int bLevel = (int)this.m_Model.CurrentGuildInfo.briefInfo.bLevel;
			int upgradeCostDianQuanByLevel = CGuildHelper.GetUpgradeCostDianQuanByLevel(bLevel);
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().DianQuan < (ulong)((long)upgradeCostDianQuanByLevel))
			{
				CUICommonSystem.OpenDianQuanNotEnoughTip();
				return;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Request_Extend_Member_Limit");
		}

		private void On_Guild_Setting_Down_Join_Level_Num(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(6).GetComponent<Text>();
			int num;
			if (int.TryParse(component.text, out num))
			{
				int num2 = num - 1;
				if ((long)num2 >= (long)((ulong)CGuildHelper.GetGuildMemberMinPvpLevel()))
				{
					component.text = num2.ToString();
				}
				if ((long)num2 <= (long)((ulong)CGuildHelper.GetGuildMemberMinPvpLevel()))
				{
					Button component2 = srcFormScript.GetWidget(10).GetComponent<Button>();
					CUICommonSystem.SetButtonEnable(component2, false, false, true);
				}
				Button component3 = srcFormScript.GetWidget(9).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component3, true, true, true);
			}
		}

		private void On_Guild_Setting_Up_Join_Level_Num(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(6).GetComponent<Text>();
			int num;
			if (int.TryParse(component.text, out num))
			{
				int num2 = num + 1;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null && (long)num2 <= (long)((ulong)masterRoleInfo.GetMaxPvpLevel()))
				{
					component.text = num2.ToString();
				}
				if (masterRoleInfo != null && (long)num2 >= (long)((ulong)masterRoleInfo.GetMaxPvpLevel()))
				{
					Button component2 = srcFormScript.GetWidget(9).GetComponent<Button>();
					CUICommonSystem.SetButtonEnable(component2, false, false, true);
				}
				Button component3 = srcFormScript.GetWidget(10).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component3, true, true, true);
			}
		}

		private void On_Guild_Setting_Down_Join_Grade(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
			Text component2 = srcFormScript.GetWidget(8).GetComponent<Text>();
			int inShowGrade;
			if (int.TryParse(component.text, out inShowGrade))
			{
				byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade(inShowGrade).bLogicGrade;
				int num = (int)(bLogicGrade - 1);
				if (num >= 1)
				{
					ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade(num);
					component.text = gradeDataByLogicGrade.bGrade.ToString();
					component2.text = ((num != 1) ? StringHelper.UTF8BytesToString(ref gradeDataByLogicGrade.szGradeDesc) : Singleton<CTextManager>.GetInstance().GetText("Guild_No_Grade_Limit_Tip"));
				}
				if (num <= 1)
				{
					Button component3 = srcFormScript.GetWidget(12).GetComponent<Button>();
					CUICommonSystem.SetButtonEnable(component3, false, false, true);
				}
				Button component4 = srcFormScript.GetWidget(11).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component4, true, true, true);
			}
		}

		private void On_Guild_Setting_Up_Join_Grade(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
			Text component2 = srcFormScript.GetWidget(8).GetComponent<Text>();
			int inShowGrade;
			if (int.TryParse(component.text, out inShowGrade))
			{
				byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade(inShowGrade).bLogicGrade;
				int num = (int)(bLogicGrade + 1);
				if (num <= CLadderSystem.MAX_RANK_LEVEL)
				{
					ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade(num);
					component.text = gradeDataByLogicGrade.bGrade.ToString();
					component2.text = StringHelper.UTF8BytesToString(ref gradeDataByLogicGrade.szGradeDesc);
				}
				if (num >= CLadderSystem.MAX_RANK_LEVEL)
				{
					Button component3 = srcFormScript.GetWidget(11).GetComponent<Button>();
					CUICommonSystem.SetButtonEnable(component3, false, false, true);
				}
				Button component4 = srcFormScript.GetWidget(12).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component4, true, true, true);
			}
		}

		private void On_Guild_Need_Approval_Slider_Value_Changed(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			int num = (int)uiEvent.m_eventParams.sliderValue;
			Text component = srcFormScript.GetWidget(2).GetComponent<Text>();
			component.text = ((num != 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_No") : Singleton<CTextManager>.GetInstance().GetText("Common_Yes"));
		}

		private void On_Guild_Open_Apply_List_Form(CUIEvent uiEvent)
		{
			this.OpenApplyListForm();
			Singleton<CGuildInfoController>.GetInstance().RequestApplyList(0);
		}

		private void OpenApplyListForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.RefreshApplyListCloseRedDotToggle(cUIFormScript);
		}

		private void On_Guild_Application_Pass(CUIEvent uiEvent)
		{
			if (CGuildHelper.IsGuildMemberFull())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Our_Member_Full", true, 1.5f, null, new object[0]);
				return;
			}
			CUIListScript component = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
			if (elemenet == null)
			{
				return;
			}
			Text component2 = elemenet.transform.Find("txtUidData").GetComponent<Text>();
			ulong num;
			if (ulong.TryParse(component2.text, out num))
			{
				if (CGuildHelper.GetGuildMemberInfoByUid(num) != null)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Has_Member_With_Same_Uid", true, 1.5f, null, new object[0]);
					return;
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte>("Guild_Approve", num, 1);
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Application_Pass(): txtUid.text={0}", new object[]
				{
					component2.text
				});
			}
		}

		private void On_Guild_Application_Reject(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
			if (elemenet == null)
			{
				return;
			}
			Text component2 = elemenet.transform.Find("txtUidData").GetComponent<Text>();
			ulong arg;
			if (ulong.TryParse(component2.text, out arg))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte>("Guild_Approve", arg, 0);
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Application_Reject(): txtUid.text={0}", new object[]
				{
					component2.text
				});
			}
		}

		private void On_Guild_Recommend_Invite(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
			if (elemenet == null)
			{
				return;
			}
			Transform transform = elemenet.transform;
			Text component2 = transform.Find("txtUidData").GetComponent<Text>();
			Text component3 = transform.Find("txtLogicWorldIdData").GetComponent<Text>();
			ulong num;
			int arg;
			if (ulong.TryParse(component2.text, out num) && int.TryParse(component3.text, out arg))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, int>("Guild_Invite", num, arg);
				this.m_Model.RemoveRecommendInfo(num);
				this.RefreshApplyListPanel();
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Recommend_Invite(): txtUid.text={0}", new object[]
				{
					component2.text
				});
			}
		}

		private void On_Guild_Recommend_Reject(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
			if (elemenet == null)
			{
				return;
			}
			Transform transform = elemenet.transform;
			Text component2 = transform.Find("txtUidData").GetComponent<Text>();
			Text component3 = transform.Find("txtLogicWorldIdData").GetComponent<Text>();
			ulong arg;
			int arg2;
			if (ulong.TryParse(component2.text, out arg) && int.TryParse(component3.text, out arg2))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, int>("Guild_Reject_Recommend", arg, arg2);
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Recommend_Reject(): txtUid.text={0}", new object[]
				{
					component2.text
				});
			}
		}

		private void On_Guild_Guild_Apply_Time_Up(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
			if (elemenet == null)
			{
				return;
			}
			Text component2 = elemenet.transform.Find("txtUidData").GetComponent<Text>();
			ulong arg;
			if (ulong.TryParse(component2.text, out arg))
			{
				elemenet.gameObject.CustomSetActive(false);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Apply_Time_Up", arg);
			}
			else
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Guild_Apply_Time_Up(): txtUid.text={0}", new object[]
				{
					component2.text
				});
			}
		}

		private void On_Guild_ApplyList_Save_Pref(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			Toggle component = srcFormScript.GetWidget(1).GetComponent<Toggle>();
			PlayerPrefs.SetInt("Guild_ApplyList_CloseRedDot", (!component.isOn) ? 0 : 1);
			this.RefreshApplyRedDot();
		}

		private void On_Guild_Apply_List_Element_Head_Selected(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.GetRecommendInfoCount())
			{
				List<stRecommendInfo> recommendInfo = this.m_Model.GetRecommendInfo();
				if (recommendInfo != null)
				{
					stRecommendInfo stRecommendInfo = recommendInfo[uiEvent.m_srcWidgetIndexInBelongedList];
					Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(stRecommendInfo.uid, stRecommendInfo.logicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
				}
			}
			else
			{
				List<stApplicantInfo> applicants = this.m_Model.GetApplicants();
				if (applicants != null)
				{
					stApplicantInfo stApplicantInfo = applicants[uiEvent.m_srcWidgetIndexInBelongedList - this.m_Model.GetRecommendInfoCount()];
					Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(stApplicantInfo.stBriefInfo.uulUid, stApplicantInfo.stBriefInfo.dwLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
				}
			}
		}

		private void On_Guild_Apply_List_Element_Enabled(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.GetRecommendInfoCount())
			{
				List<stRecommendInfo> recommendInfo = this.m_Model.GetRecommendInfo();
				CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
				bool isAdmin = CGuildSystem.HasManageAuthority();
				if (recommendInfo != null && cUIListElementScript != null)
				{
					this.SetRecommendItem(cUIListElementScript, recommendInfo[uiEvent.m_srcWidgetIndexInBelongedList], isAdmin);
				}
			}
			else
			{
				List<stApplicantInfo> applicants = this.m_Model.GetApplicants();
				CUIListElementScript cUIListElementScript2 = uiEvent.m_srcWidgetScript as CUIListElementScript;
				bool isAdmin2 = CGuildSystem.HasManageAuthority();
				int num = uiEvent.m_srcWidgetIndexInBelongedList - this.m_Model.GetRecommendInfoCount();
				if (applicants != null && cUIListElementScript2 != null && num < this.m_Model.GetApplicantsCount())
				{
					this.SetApplicantItem(cUIListElementScript2, applicants[num], isAdmin2);
				}
			}
		}

		private void On_Guild_Guild_Apply_Quit(CUIEvent uiEvent)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN && this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum > 1 && CGuildHelper.IsSelfInGuildMemberList())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_ChairMan_Quit_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(7u).dwConfValue;
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, (int)dwConfValue);
			GuildMemInfo playerGuildMemberInfo = this.m_Model.GetPlayerGuildMemberInfo();
			uint num = (playerGuildMemberInfo != null) ? (playerGuildMemberInfo.dwConstruct * GameDataMgr.guildMiscDatabin.GetDataByKey(20u).dwConfValue / 100u) : 0u;
			uint num2 = (playerGuildMemberInfo != null) ? (playerGuildMemberInfo.dwConstruct - num) : 0u;
			string text;
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN && CGuildHelper.IsSelfInGuildMemberList())
			{
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Chairman_Quit_Confirm");
			}
			else
			{
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Quit_Confirm", new string[]
				{
					timeSpan.TotalHours.ToString(),
					num.ToString(),
					num2.ToString()
				});
			}
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Guild_Apply_Quit_Confirm, enUIEventID.Guild_Guild_Apply_Quit_Cancel, false);
		}

		private void On_Guild_Guild_Apply_Quit_Confirm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Quit");
		}

		private void OnGuildPositionAppointViceChairman(CUIEvent uiEvent)
		{
			if (!CGuildSystem.HasAppointViceChairmanAuthority())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_You_Have_No_Authority", true, 1.5f, null, new object[0]);
				return;
			}
			if (!CGuildSystem.CanBeAppointedToViceChairman(this.m_Model.CurrentSelectedMemberInfo.enPosition))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_Vice_Chairman_Position_Limit_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			if (CGuildHelper.IsViceChairmanFull())
			{
				List<ulong> uids;
				List<string> names;
				CGuildHelper.GetViceChairmanUidAndName(out uids, out names);
				this.OpenExchangePositionForm(names, uids);
			}
			else
			{
				string sName = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName;
				string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Appoint_Vice_Chairman_Confirm", new string[]
				{
					sName
				});
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Appoint_Vice_Chairman_Confirm, enUIEventID.None, false);
			}
		}

		private void OnGuildPositionAppointViceChairmanConfirm(CUIEvent uiEvent)
		{
			ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
			byte arg = 4;
			ulong arg2 = 0uL;
			if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab") != null)
			{
				CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
				CUIListScript component = srcFormScript.GetWidget(0).GetComponent<CUIListScript>();
				Text component2 = component.GetSelectedElement().transform.Find("Uid").GetComponent<Text>();
				arg2 = ulong.Parse(component2.text);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte, ulong, string>("Guild_Position_Appoint", uulUid, arg, arg2, string.Empty);
		}

		private void OnGuildPositionFireMember(CUIEvent uiEvent)
		{
			COM_PLAYER_GUILD_STATE enPosition = this.m_Model.CurrentSelectedMemberInfo.enPosition;
			if (!CGuildSystem.HasFireMemberAuthority(enPosition))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Fire_Member_You_Have_No_Authority", true, 1.5f, null, new object[0]);
				return;
			}
			string sName = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName;
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Fire_Member_Confirm", new string[]
			{
				sName,
				sName,
				sName,
				sName
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Fire_Member_Confirm, enUIEventID.None, false);
		}

		private void OnGuildPositionFireMemberConfirm(CUIEvent uiEvent)
		{
			ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Position_Confirm_Fire_Member", uulUid);
		}

		private void OnGuildPositionChairmanTransfer(CUIEvent uiEvent)
		{
			if (!CGuildSystem.HasTransferPositionAuthority())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Only_Chairman_Can_Transfer_Position", true, 1.5f, null, new object[0]);
				return;
			}
			if (this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Cannot_Transfer_To_Self", true, 1.5f, null, new object[0]);
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Position_Transfer_Confirm", new string[]
			{
				this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Chairman_Transfer_Confirm, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("Guild_Transfer_Btn"), string.Empty, false);
		}

		private void OnGuildPositionChairmanTransferConfirm(CUIEvent uiEvent)
		{
			CSecurePwdSystem.TryToValidate(enOpPurpose.GUILD_TRANSFER_CHAIRMAN, enUIEventID.Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm, default(stUIEventParams));
		}

		private void On_Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm(CUIEvent uiEvent)
		{
			string pwd = uiEvent.m_eventParams.pwd;
			if (string.IsNullOrEmpty(pwd))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Guild_Transfer_Chairman_Without_Secure_Pwd"), enUIEventID.Guild_Real_Transfer_Chairman, enUIEventID.None, false);
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Real_Transfer_Chairman, new stUIEventParams
				{
					pwd = uiEvent.m_eventParams.pwd
				});
			}
		}

		private void On_Guild_Real_Transfer_Chairman(CUIEvent uiEvent)
		{
			ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
			byte arg = 3;
			ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			string pwd = uiEvent.m_eventParams.pwd;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte, ulong, string>("Guild_Position_Appoint", uulUid, arg, playerUllUID, pwd);
		}

		public bool IsShow()
		{
			return Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab") != null;
		}

		public bool IsApplyListFormShow()
		{
			return Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab") != null;
		}

		public bool IsRankpointRankFormShow()
		{
			return Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab") != null;
		}

		public void OpenForm()
		{
			if (this.IsShow())
			{
				return;
			}
			this.m_form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab", false, true);
			this.InitTab();
			this.RefreshApplyRedDot();
			Singleton<EventRouter>.GetInstance().BroadCastEvent<enGuildRankpointRankListType>("Guild_Request_Rankpoint_Rank_List", enGuildRankpointRankListType.CurrentWeek);
		}

		public void CloseForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab");
		}

		public void CloseAllGuildForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Icon.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Log.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Sign_Success.prefab");
			Singleton<CGuildMatchSystem>.GetInstance().CloseAllGuildMatchForm();
		}

		public void InitTab()
		{
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Guild_Info_Tab_Info"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Info_Tab_Member")
			};
			GameObject widget = this.m_form.GetWidget(2);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < component.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
			component.m_alwaysDispatchSelectedChangeEvent = true;
			component.SelectElement(0, true);
		}

		public void InitPanel()
		{
			this.m_form.GetWidget(0).CustomSetActive(false);
			this.m_form.GetWidget(1).CustomSetActive(false);
			CGuildInfoView.Tab curTab = this.CurTab;
			if (curTab != CGuildInfoView.Tab.GuildInfo)
			{
				if (curTab == CGuildInfoView.Tab.GuildMember)
				{
					this.m_curGuildMemberListSortType = CGuildInfoView.enGuildMemberListSortType.Default;
					this.RefreshMemberPanel();
				}
			}
			else
			{
				this.RefreshInfoPanel();
				this.RefreshPlatformGroupPanel();
			}
		}

		public void RefreshInfoPanel()
		{
			if (this.m_form == null || this.CurTab != CGuildInfoView.Tab.GuildInfo)
			{
				return;
			}
			this.m_form.GetWidget(0).CustomSetActive(true);
			this.RefreshWeekRankNumPanel();
			this.RefreshInfoPanelSignBtn();
			this.RefreshRankpointMemberListPanel();
			this.RefreshRankpointInfoPanel();
			this.RefreshGuildMatchPanel();
			this.RefreshFacePanel();
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				GameObject widget = this.m_form.GetWidget(20);
				if (widget)
				{
					widget.CustomSetActive(false);
				}
			}
		}

		public void RefreshMemberPanel()
		{
			if (this.m_form == null || this.CurTab != CGuildInfoView.Tab.GuildMember)
			{
				return;
			}
			this.RefreshMemberList();
			this.RefreshMemberCntPanel();
			this.RefreshMemberPanelApplyBtnRedDot();
			GameObject widget = this.m_form.GetWidget(21);
			widget.CustomSetActive(CGuildSystem.HasWirteGuildMailAuthority());
			GameObject widget2 = this.m_form.GetWidget(38);
			widget2.CustomSetActive(CGuildSystem.HasManageAuthority());
			GameObject widget3 = this.m_form.GetWidget(39);
			uint guildSendRecruitCd = this.GetGuildSendRecruitCd();
			long num = (long)((ulong)guildSendRecruitCd - (ulong)((long)(CRoleInfo.GetCurrentUTCTime() - this.m_lastSendReruitTime)));
			if (this.m_lastSendReruitTime > 0 && num > 0L)
			{
				widget3.CustomSetActive(true);
				CUITimerScript component = widget3.GetComponent<CUITimerScript>();
				component.SetTotalTime((float)num);
				component.StartTimer();
				GameObject widget4 = this.m_form.GetWidget(40);
				widget4.CustomSetActive(false);
			}
			else
			{
				widget3.CustomSetActive(false);
			}
			GameObject widget5 = this.m_form.GetWidget(8);
			widget5.CustomSetActive(CGuildSystem.HasManageAuthority());
			this.RefreshBulletinPanel();
			this.RefreshFacePanel();
		}

		private void RefreshMemberList()
		{
			if (this.m_form == null)
			{
				return;
			}
			this.m_form.GetWidget(1).CustomSetActive(true);
			CUIListScript component = this.m_form.GetWidget(9).GetComponent<CUIListScript>();
			int bMemberNum = (int)this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum;
			if (bMemberNum > 0)
			{
				this.SortGuildMemberList();
			}
			component.SetElementAmount(bMemberNum);
			component.m_alwaysDispatchSelectedChangeEvent = true;
			this.DisplayGuildMemberListSortBtn();
		}

		public void RefreshFacePanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
			if (currentGuildInfo == null)
			{
				DebugHelper.Assert(false, "GuildInfo is null!!!");
				return;
			}
			this.m_form.GetWidget(22).CustomSetActive(true);
			Image component = this.m_form.GetWidget(3).GetComponent<Image>();
			Text component2 = this.m_form.GetWidget(4).GetComponent<Text>();
			Transform transform = this.m_form.GetWidget(14).transform;
			Text component3 = this.m_form.GetWidget(5).GetComponent<Text>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + currentGuildInfo.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
			component2.text = currentGuildInfo.briefInfo.sName;
			CGuildHelper.SetStarLevelPanel(currentGuildInfo.star, transform, this.m_form);
			component3.text = currentGuildInfo.chairman.stBriefInfo.sName;
		}

		public void RefreshBulletinPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
			if (currentGuildInfo == null)
			{
				DebugHelper.Assert(false, "CGuildInfoView.RefreshInfoPanel(): guildInfo is null!!!");
				return;
			}
			Text component = this.m_form.GetWidget(7).GetComponent<Text>();
			component.text = currentGuildInfo.briefInfo.sBulletin;
			GameObject widget = this.m_form.GetWidget(10);
			widget.CustomSetActive(CGuildSystem.HasManageAuthority());
		}

		public void RefreshPlatformGroupPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject obj = Utility.FindChild(this.m_form.gameObject, "pnlBg/pnlInfo/pnlHead/pnlPlatformGroup");
			obj.CustomSetActive(false);
			if (Singleton<CGuildSystem>.GetInstance().IsOpenPlatformGroupFunc())
			{
				Debug.Log(string.Format("**GuildPlatformGroup** RefreshPlatformGroupPanel Wechat Platform", new object[0]));
				if (this.m_Model.PlatformGroupStatus == CGuildSystem.enPlatformGroupStatus.Resolve)
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_PlatformGroup_Refresh_Group_Panel");
				}
				else
				{
					this.RefreshPlatformGroupPanel(this.m_Model.PlatformGroupStatus, this.m_Model.IsSelfInPlatformGroup);
				}
			}
		}

		public void RefreshMemberCntPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
			if (currentGuildInfo == null)
			{
				DebugHelper.Assert(false, "GuildInfo is null!!!");
				return;
			}
			GameObject widget = this.m_form.GetWidget(11);
			widget.CustomSetActive(!CGuildHelper.IsGuildMaxLevel((int)currentGuildInfo.briefInfo.bLevel));
			Text component = this.m_form.GetWidget(6).GetComponent<Text>();
			component.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Member_Count_Format", new string[]
			{
				currentGuildInfo.briefInfo.bMemberNum.ToString(),
				CGuildHelper.GetMaxGuildMemberCountByLevel((int)currentGuildInfo.briefInfo.bLevel).ToString()
			});
		}

		public void RefreshSettingBtn()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(8);
			widget.CustomSetActive(CGuildSystem.HasManageAuthority());
		}

		public void RefreshApplyListPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab");
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.GetWidget(0).GetComponent<CUIListScript>();
			bool flag = CGuildSystem.HasManageAuthority();
			int recommendInfoCount = this.m_Model.GetRecommendInfoCount();
			int applicantsCount = this.m_Model.GetApplicantsCount();
			List<stRecommendInfo> arg_64_0 = (recommendInfoCount != 0) ? this.m_Model.GetRecommendInfo() : null;
			List<stApplicantInfo> arg_7E_0 = (applicantsCount != 0) ? this.m_Model.GetApplicants() : null;
			component.SetElementAmount(recommendInfoCount + applicantsCount);
			if (recommendInfoCount + applicantsCount == 0)
			{
				CGuildSystem.s_isApplyAndRecommendListEmpty = true;
				this.RefreshApplyRedDot();
			}
		}

		private void RefreshApplyListCloseRedDotToggle(CUIFormScript applyListForm)
		{
			bool isOn = this.IsApplyListCloseRedDotPrefOn();
			Toggle component = applyListForm.GetWidget(1).GetComponent<Toggle>();
			component.isOn = isOn;
		}

		private void OpenSettingForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Image component = cUIFormScript.GetWidget(0).GetComponent<Image>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, cUIFormScript, true, false, false, false);
			Text component2 = cUIFormScript.GetWidget(3).GetComponent<Text>();
			component2.text = this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId.ToString();
			Slider component3 = cUIFormScript.GetWidget(1).GetComponent<Slider>();
			bool flag = CGuildHelper.IsGuildNeedApproval(this.m_Model.CurrentGuildInfo.briefInfo.dwSettingMask);
			component3.value = (float)((!flag) ? 1 : 0);
			Text component4 = cUIFormScript.GetWidget(5).GetComponent<Text>();
			component4.text = this.m_Model.CurrentGuildInfo.briefInfo.sName;
			GameObject gameObject = cUIFormScript.GetWidget(4).gameObject;
			gameObject.CustomSetActive(CGuildSystem.HasGuildNameChangeAuthority());
			Text component5 = cUIFormScript.GetWidget(6).GetComponent<Text>();
			component5.text = this.m_Model.CurrentGuildInfo.briefInfo.LevelLimit.ToString();
			if ((uint)this.m_Model.CurrentGuildInfo.briefInfo.LevelLimit <= CGuildHelper.GetGuildMemberMinPvpLevel())
			{
				Button component6 = cUIFormScript.GetWidget(10).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component6, false, false, true);
			}
			Text component7 = cUIFormScript.GetWidget(8).GetComponent<Text>();
			component7.text = CGuildHelper.GetLadderGradeLimitText((int)this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit);
			if (CLadderSystem.GetGradeDataByShowGrade((int)this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit).bLogicGrade <= 1)
			{
				Button component8 = cUIFormScript.GetWidget(12).GetComponent<Button>();
				CUICommonSystem.SetButtonEnable(component8, false, false, true);
			}
			Text component9 = cUIFormScript.GetWidget(7).GetComponent<Text>();
			component9.text = this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit.ToString();
			GameObject widget = cUIFormScript.GetWidget(13);
			widget.CustomSetActive(false);
			if (Singleton<CGuildSystem>.GetInstance().IsOpenPlatformGroupFunc())
			{
				if (this.m_Model.PlatformGroupStatus == CGuildSystem.enPlatformGroupStatus.Resolve)
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_PlatformGroup_Refresh_Group_Panel");
				}
				else
				{
					this.RefreshPlatformGroupPanelInSettingForm(this.m_Model.PlatformGroupStatus, this.m_Model.IsSelfInPlatformGroup);
				}
			}
		}

		public void OpenGuildIconForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Icon.prefab", false, true);
			CUIListScript listScript = cUIFormScript.GetWidget(0).GetComponent<CUIListScript>();
			int showGuildIconCount = this.GetShowGuildIconCount();
			listScript.SetElementAmount(showGuildIconCount);
			int i = 0;
			GameDataMgr.guildIconDatabin.Accept(delegate(ResGuildIcon x)
			{
				if (x.dwGuildMatchSeasonRankLimit == 0u || (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() && x.dwGuildMatchSeasonRankLimit > 0u && x.dwGuildMatchSeasonRankLimit == this.m_Model.CurrentGuildInfo.GuildMatchInfo.dwLastSeasonRankNo))
				{
					CUIListElementScript elemenet = listScript.GetElemenet(i);
					this.SetGuildIcon(elemenet, x);
					i++;
				}
			});
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				listScript.SetElementSelectChangedEvent(enUIEventID.Guild_Guild_Setting_Guild_Icon_Selected);
			}
		}

		private int GetShowGuildIconCount()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				uint dwLastSeasonRankNo = this.m_Model.CurrentGuildInfo.GuildMatchInfo.dwLastSeasonRankNo;
				if (0u < dwLastSeasonRankNo && dwLastSeasonRankNo <= 3u)
				{
					return GameDataMgr.guildIconDatabin.count - 3 + 1;
				}
			}
			return GameDataMgr.guildIconDatabin.count - 3;
		}

		private void SetApplicantItem(CUIListElementScript listElementScript, stApplicantInfo info, bool isAdmin)
		{
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			Transform transform = listElementScript.transform;
			Text component = transform.Find("txtUidData").GetComponent<Text>();
			Text component2 = transform.Find("txtLogicWorldIdData").GetComponent<Text>();
			CUIHttpImageScript component3 = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
			Image component4 = component3.transform.Find("NobeIcon").GetComponent<Image>();
			Image component5 = component3.transform.Find("NobeImag").GetComponent<Image>();
			Text component6 = transform.Find("nameGroup/txtName").GetComponent<Text>();
			Text component7 = transform.Find("txtLevel").GetComponent<Text>();
			Image component8 = transform.Find("nameGroup/imgGender").GetComponent<Image>();
			Transform transform2 = transform.Find("nameGroup/MentorIcon");
			GameObject gameObject = transform.Find("RankCon").gameObject;
			GameObject gameObject2 = transform.Find("btnGroup").gameObject;
			CUITimerScript component9 = transform.Find("Timer").GetComponent<CUITimerScript>();
			Text component10 = transform.Find("txtRecommender").GetComponent<Text>();
			component3.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component4, (int)info.stBriefInfo.stVip.level, false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component5, (int)info.stBriefInfo.stVip.headIconId);
			component.text = info.stBriefInfo.uulUid.ToString();
			component2.text = info.stBriefInfo.uulUid.ToString();
			component6.text = info.stBriefInfo.sName;
			Text arg_1CB_0 = component7;
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Level_Format", new string[]
			{
				info.stBriefInfo.dwLevel.ToString()
			});
			component7.text = text;
			arg_1CB_0.text = text;
			CUIUtility.SetGenderImageSprite(component8, info.stBriefInfo.gender);
			int bGrade = (int)info.stBriefInfo.rankGrade.bGrade;
			CLadderView.ShowRankDetail(gameObject, (byte)bGrade, info.stBriefInfo.dwClassOfRank, false, true);
			if (isAdmin)
			{
				gameObject2.CustomSetActive(true);
			}
			else
			{
				gameObject2.CustomSetActive(false);
			}
			if (transform2 != null)
			{
				UT.SetMentorLv(transform2.gameObject, info.stBriefInfo.mentorLvl);
			}
			int dwConfValue = (int)GameDataMgr.guildMiscDatabin.GetDataByKey(9u).dwConfValue;
			int num;
			if (info.dwApplyTime == 0)
			{
				num = dwConfValue;
			}
			else
			{
				num = info.dwApplyTime + dwConfValue - currentUTCTime;
				if (num < 0)
				{
					num = 0;
				}
			}
			component9.SetTotalTime((float)num);
			component9.StartTimer();
			component10.text = string.Empty;
		}

		private void SetRecommendItem(CUIListElementScript listElementScript, stRecommendInfo info, bool isAdmin)
		{
			Transform transform = listElementScript.transform;
			Text component = transform.Find("txtUidData").GetComponent<Text>();
			Text component2 = transform.Find("txtLogicWorldIdData").GetComponent<Text>();
			CUIHttpImageScript component3 = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
			Image component4 = component3.transform.Find("NobeIcon").GetComponent<Image>();
			Image component5 = component3.transform.Find("NobeImag").GetComponent<Image>();
			Text component6 = transform.Find("nameGroup/txtName").GetComponent<Text>();
			Text component7 = transform.Find("txtLevel").GetComponent<Text>();
			Image component8 = transform.Find("nameGroup/imgGender").GetComponent<Image>();
			Transform transform2 = transform.Find("nameGroup/MentorIcon");
			GameObject gameObject = transform.Find("RankCon").gameObject;
			Text component9 = transform.Find("txtRecommender").GetComponent<Text>();
			GameObject gameObject2 = transform.Find("btnGroup").gameObject;
			GameObject gameObject3 = transform.Find("btnGroup/btnPass").gameObject;
			GameObject gameObject4 = transform.Find("btnGroup/btnReject").gameObject;
			component3.SetImageUrl(CGuildHelper.GetHeadUrl(info.headUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component4, (int)info.stVip.level, false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component5, (int)info.stVip.headIconId);
			component.text = info.uid.ToString();
			component2.text = info.logicWorldID.ToString();
			component6.text = info.name;
			component7.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Level_Format", new string[]
			{
				info.level.ToString()
			});
			CUIUtility.SetGenderImageSprite(component8, info.gender);
			int bGrade = (int)info.rankGrade.bGrade;
			CLadderView.ShowRankDetail(gameObject, (byte)bGrade, info.rankClass, false, true);
			component9.text = info.recommendName;
			if (transform2 != null)
			{
				UT.SetMentorLv(transform2.gameObject, info.mentorLvl);
			}
			if (isAdmin)
			{
				gameObject2.CustomSetActive(true);
				gameObject3.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Recommend_Invite);
				gameObject3.transform.Find("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Common_Invite");
				gameObject4.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Recommend_Reject);
			}
			else
			{
				gameObject2.CustomSetActive(false);
			}
		}

		private GuildMemInfo GetSelectedMemberInfo()
		{
			if (this.m_form == null)
			{
				return null;
			}
			if (this.CurTab == CGuildInfoView.Tab.GuildInfo)
			{
				CUIListScript component = this.m_form.GetWidget(23).GetComponent<CUIListScript>();
				int selectedIndex = component.GetSelectedIndex();
				return CGuildHelper.GetGuildMemberInfoByUid(this.m_Model.RankpointMemberInfoList[selectedIndex].Key);
			}
			if (this.CurTab == CGuildInfoView.Tab.GuildMember)
			{
				CUIListScript component2 = this.m_form.GetWidget(9).GetComponent<CUIListScript>();
				int selectedIndex2 = component2.GetSelectedIndex();
				return this.m_Model.CurrentGuildInfo.listMemInfo[selectedIndex2];
			}
			return null;
		}

		public void OpenExchangePositionForm(List<string> names, List<ulong> uids)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			CUIListScript component = cUIFormScript.GetWidget(0).GetComponent<CUIListScript>();
			component.SetElementAmount(names.Count);
			for (int i = 0; i < names.Count; i++)
			{
				Transform transform = component.GetElemenet(i).transform;
				transform.Find("Name").GetComponent<Text>().text = names[i];
				transform.Find("Uid").GetComponent<Text>().text = uids[i].ToString();
			}
			component.SelectElement(0, true);
		}

		private void SortGuildMemberList()
		{
			switch (this.m_curGuildMemberListSortType)
			{
			case CGuildInfoView.enGuildMemberListSortType.Default:
				this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListPositionDesc));
				break;
			case CGuildInfoView.enGuildMemberListSortType.PositionDesc:
				this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListPositionDesc));
				break;
			case CGuildInfoView.enGuildMemberListSortType.WeekRankpointDesc:
				this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListWeekRankpointDesc));
				break;
			case CGuildInfoView.enGuildMemberListSortType.SeasonRankpointDesc:
				this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListSeasonRankpointDesc));
				break;
			case CGuildInfoView.enGuildMemberListSortType.OnlineStatus:
				this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListOnlineStatusDesc));
				break;
			}
		}

		private int SortGuildMemberListDefault(GuildMemInfo info1, GuildMemInfo info2)
		{
			if (info1.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID && info2.stBriefInfo.uulUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				return -1;
			}
			if (info1.stBriefInfo.uulUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID && info2.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				return 1;
			}
			if (info1.enPosition.CompareTo(info2.enPosition) == 0)
			{
				return -info1.RankInfo.weekRankPoint.CompareTo(info2.RankInfo.weekRankPoint);
			}
			return info1.enPosition.CompareTo(info2.enPosition);
		}

		private int SortGuildMemberListPositionDesc(GuildMemInfo info1, GuildMemInfo info2)
		{
			return info1.enPosition.CompareTo(info2.enPosition);
		}

		private int SortGuildMemberListWeekRankpointDesc(GuildMemInfo info1, GuildMemInfo info2)
		{
			return -info1.RankInfo.weekRankPoint.CompareTo(info2.RankInfo.weekRankPoint);
		}

		private int SortGuildMemberListSeasonRankpointDesc(GuildMemInfo info1, GuildMemInfo info2)
		{
			return -info1.RankInfo.totalRankPoint.CompareTo(info2.RankInfo.totalRankPoint);
		}

		private int SortGuildMemberListOnlineStatusDesc(GuildMemInfo info1, GuildMemInfo info2)
		{
			if (info1.stBriefInfo.dwGameEntity != 0u && info2.stBriefInfo.dwGameEntity == 0u)
			{
				return -1;
			}
			if (info1.stBriefInfo.dwGameEntity == 0u && info2.stBriefInfo.dwGameEntity != 0u)
			{
				return 1;
			}
			return -info1.LastLoginTime.CompareTo(info2.LastLoginTime);
		}

		private void On_Guild_Rankpoint_Help(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(9);
		}

		private void On_Guild_Rankpoint_Enter_Matching(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
		}

		private void On_Guild_Rankpoint_Rank_List_Tab_Change(CUIEvent uiEvent)
		{
			this.RefreshRankpointRankList(uiEvent);
			this.SetRankpointRankListWidgetPositionAndSize(uiEvent);
		}

		private void On_Guild_Rankpoint_Season_Rank_List_Tab_Change(CUIEvent uiEvent)
		{
			this.RefreshRankpointSeasonRankList(uiEvent);
		}

		private void On_Guild_Rankpoint_Open_Rankpoint_Rank_Form(CUIEvent uiEvent)
		{
			this.OpenRankpointRankForm();
		}

		private void On_Guild_Rankpoint_Member_List_Element_Enabled(CUIEvent uiEvent)
		{
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript == null)
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Rankpoint_Member_List_Element_Enabled(): listElement is null!!!");
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			ulong key = this.m_Model.RankpointMemberInfoList[srcWidgetIndexInBelongedList].Key;
			GuildMemInfo guildMemberInfoByUid = this.m_Model.GetGuildMemberInfoByUid(key);
			this.SetRankpointMemberItem(cUIListElementScript.transform, guildMemberInfoByUid, srcWidgetIndexInBelongedList, this.m_form, true);
		}

		private void On_Guild_Rankpoint_Rank_List_Element_Enabled(CUIEvent uiEvent)
		{
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript == null)
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Rankpoint_Rank_List_Element_Enabled(): listElement is null!!!");
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			enGuildRankpointRankListType rankListTypeByTabSelectedIndex = this.GetRankListTypeByTabSelectedIndex(uiEvent.m_srcFormScript);
			ListView<RankpointRankInfo> listView = this.m_Model.RankpointRankInfoLists[(int)rankListTypeByTabSelectedIndex];
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= listView.Count)
			{
				DebugHelper.Assert(false, "rankListType {0}, elementIndex {1} is out of list range", new object[]
				{
					rankListTypeByTabSelectedIndex,
					srcWidgetIndexInBelongedList
				});
				return;
			}
			if (listView[srcWidgetIndexInBelongedList] == null)
			{
				DebugHelper.Assert(false, "rankListType {0}, elementIndex {1} object is null", new object[]
				{
					rankListTypeByTabSelectedIndex,
					srcWidgetIndexInBelongedList
				});
				return;
			}
			this.SetRankpointGuildRankListItem(cUIListElementScript, listView[srcWidgetIndexInBelongedList], rankListTypeByTabSelectedIndex, listView[0].rankNo);
		}

		private enGuildRankpointRankListType GetRankListTypeByTabSelectedIndex(CUIFormScript rankpointRankForm)
		{
			CUIListScript component = rankpointRankForm.GetWidget(5).GetComponent<CUIListScript>();
			CUIListScript component2 = rankpointRankForm.GetWidget(8).GetComponent<CUIListScript>();
			enGuildRankpointRankListType result = enGuildRankpointRankListType.CurrentWeek;
			if (component.GetSelectedIndex() == 0)
			{
				result = enGuildRankpointRankListType.CurrentWeek;
			}
			else if (component.GetSelectedIndex() == 1)
			{
				result = enGuildRankpointRankListType.LastWeek;
			}
			else if (component.GetSelectedIndex() == 2)
			{
				if (component2.GetSelectedIndex() == 0)
				{
					result = enGuildRankpointRankListType.SeasonSelf;
				}
				else if (component2.GetSelectedIndex() == 1)
				{
					result = enGuildRankpointRankListType.SeasonBest;
				}
			}
			return result;
		}

		public void RefreshRankpointMemberListPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			this.InitRankpointMemberList();
			CUIListScript component = this.m_form.GetWidget(23).GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_Model.RankpointMemberInfoList.Count);
			component.m_alwaysDispatchSelectedChangeEvent = true;
			this.SetRankpointMemberPlayerItem();
		}

		private void InitRankpointMemberList()
		{
			int count = this.m_Model.CurrentGuildInfo.listMemInfo.Count;
			this.m_Model.RankpointMemberInfoList.Clear();
			for (int i = 0; i < count; i++)
			{
				this.m_Model.RankpointMemberInfoList.Add(new KeyValuePair<ulong, MemberRankInfo>(this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid, this.m_Model.CurrentGuildInfo.listMemInfo[i].RankInfo));
			}
			this.m_Model.RankpointMemberInfoList.Sort(delegate(KeyValuePair<ulong, MemberRankInfo> info1, KeyValuePair<ulong, MemberRankInfo> info2)
			{
				MemberRankInfo value = info1.Value;
				MemberRankInfo value2 = info2.Value;
				if (value.totalRankPoint == value2.totalRankPoint)
				{
					return -value.weekRankPoint.CompareTo(value2.weekRankPoint);
				}
				return -value.totalRankPoint.CompareTo(value2.totalRankPoint);
			});
		}

		public void RefreshRankpointInfoPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(31).GetComponent<Text>();
			Image component2 = this.m_form.GetWidget(25).GetComponent<Image>();
			Text component3 = this.m_form.GetWidget(26).GetComponent<Text>();
			GameObject widget = this.m_form.GetWidget(27);
			GameObject widget2 = this.m_form.GetWidget(28);
			Text component4 = this.m_form.GetWidget(29).GetComponent<Text>();
			Text component5 = this.m_form.GetWidget(30).GetComponent<Text>();
			component.text = this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint.ToString();
			component2.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint), this.m_form, true, false, false, false);
			component3.text = CGuildHelper.GetGradeName(this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint);
			this.RefreshProfitPanel();
			RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(enGuildRankpointRankListType.CurrentWeek);
			this.SetRankpointAwardPanel(this.m_form, widget, true, playerGuildRankpointRankInfo.rankNo, 0u);
			this.SetRankpointAwardPanel(this.m_form, widget2, false, 0u, CGuildHelper.GetGuildGrade());
			GuildMemInfo playerGuildMemberInfo = this.m_Model.GetPlayerGuildMemberInfo();
			if (playerGuildMemberInfo != null)
			{
				component4.text = playerGuildMemberInfo.RankInfo.weekRankPoint.ToString();
			}
			component5.text = CGuildHelper.GetRankpointClearTimeFormatString();
		}

		public void RefreshGuildMatchPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(41);
			widget.CustomSetActive(Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime());
			GameObject widget2 = this.m_form.GetWidget(20);
			if (CGuildHelper.IsGuildInfoFormGuildMatchBtnShowRedDot())
			{
				CUICommonSystem.AddRedDot(widget2, enRedDotPos.enTopLeft, 0, 20, -20);
			}
			else
			{
				CUICommonSystem.DelRedDot(widget2);
			}
		}

		public void DelGuildMatchBtnNewFlag()
		{
			if (this.m_form == null)
			{
				return;
			}
		}

		public void RefreshWeekRankNumPanel()
		{
			GameObject widget = this.m_form.GetWidget(32);
			GameObject widget2 = this.m_form.GetWidget(33);
			RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(enGuildRankpointRankListType.CurrentWeek);
			if (playerGuildRankpointRankInfo.rankNo > 0u)
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget.GetComponent<Text>().text = playerGuildRankpointRankInfo.rankNo.ToString();
			}
			else
			{
				widget2.CustomSetActive(true);
				widget.CustomSetActive(false);
			}
		}

		public void OpenRankpointRankForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
			if (cUIFormScript == null)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab", false, true);
			}
			GameObject widget = cUIFormScript.GetWidget(6);
			RectTransform rectTransform = widget.transform as RectTransform;
			CGuildInfoView.s_rankpointRankListWidgetAnchoredPositionY = rectTransform.anchoredPosition.y;
			CGuildInfoView.s_rankpointRankListWidgetSizeDeltaY = rectTransform.sizeDelta.y;
			this.InitRankpointRankTab(cUIFormScript);
			this.InitRankpointSeasonRankTab(cUIFormScript);
		}

		private void SetRankpointMemberPlayerItem()
		{
			Transform transform = this.m_form.GetWidget(24).transform;
			GuildMemInfo guildMemberInfoByUid = this.m_Model.GetGuildMemberInfoByUid(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			int rankpointMemberListPlayerIndex = CGuildHelper.GetRankpointMemberListPlayerIndex();
			this.SetRankpointMemberItem(transform, guildMemberInfoByUid, rankpointMemberListPlayerIndex, this.m_form, false);
		}

		private void SetRankpointMemberItem(Transform itemTransform, GuildMemInfo info, int elementIndex, CUIFormScript form, bool headParticleMask)
		{
			if (info == null)
			{
				DebugHelper.Assert(false, "guildMemInfo is null!!!");
				return;
			}
			CUIHttpImageScript component = itemTransform.Find("imgMemberIcon").GetComponent<CUIHttpImageScript>();
			Image component2 = component.transform.Find("NobeIcon").GetComponent<Image>();
			Image component3 = component.transform.Find("NobeImag").GetComponent<Image>();
			Text component4 = itemTransform.Find("txtMemberName").GetComponent<Text>();
			Text component5 = itemTransform.Find("txtSeasonPointNum").GetComponent<Text>();
			Text component6 = itemTransform.Find("txtWeekPointNum").GetComponent<Text>();
			Transform transform = itemTransform.Find("rank").transform;
			CUICommonSystem.SetRankDisplay((uint)(elementIndex + 1), transform);
			component.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId));
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component3, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId), form, 0.77f, headParticleMask);
			component4.text = info.stBriefInfo.sName;
			component5.text = info.RankInfo.totalRankPoint.ToString();
			component6.text = info.RankInfo.weekRankPoint.ToString();
		}

		private void SetRankpointGuildRankListItem(CUIListElementScript listElement, RankpointRankInfo info, enGuildRankpointRankListType rankListType, uint firstRankNo)
		{
			if (listElement == null)
			{
				DebugHelper.Assert(false, "listElement is null!!!");
				return;
			}
			if (info == null)
			{
				DebugHelper.Assert(false, "rankpointRankInfo is null!!!");
				return;
			}
			Image component = listElement.transform.Find("imgGuildIconBg/imgGuildIcon").GetComponent<Image>();
			Text component2 = listElement.transform.Find("txtGuildName").GetComponent<Text>();
			Text component3 = listElement.transform.Find("pnlPoint/txtPointNum").GetComponent<Text>();
			Text component4 = listElement.transform.Find("txtMemberCount").GetComponent<Text>();
			Transform transform = listElement.transform.Find("starLevel").transform;
			Image component5 = listElement.transform.Find("imgGrade").GetComponent<Image>();
			Transform transform2 = listElement.transform.Find("rank").transform;
			CUICommonSystem.SetRankDisplay(info.rankNo - this.CalculateRankNoOffset(rankListType, firstRankNo), transform2);
			component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.guildHeadId, listElement.m_belongedFormScript, true, false, false, false);
			component2.text = info.guildName;
			component3.text = info.rankScore.ToString();
			component4.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Member_Format", new string[]
			{
				info.memberNum.ToString(),
				CGuildHelper.GetMaxGuildMemberCountByLevel((int)info.guildLevel).ToString()
			});
			CGuildHelper.SetStarLevelPanel(info.star, transform, listElement.m_belongedFormScript);
			component5.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.rankScore), listElement.m_belongedFormScript, true, false, false, false);
			GameObject gameObject = listElement.transform.Find("pnlAward").gameObject;
			this.SetRankpointAwardPanel(listElement.m_belongedFormScript, gameObject, CGuildHelper.IsWeekRankpointRank(rankListType), info.rankNo, CGuildHelper.GetGradeByRankpointScore(info.rankScore));
		}

		private uint CalculateRankNoOffset(enGuildRankpointRankListType rankListType, uint firstRankNo)
		{
			uint result = 0u;
			if (rankListType == enGuildRankpointRankListType.SeasonSelf)
			{
				result = firstRankNo - 1u;
			}
			return result;
		}

		public void SetRankpointPlayerGuildRank(RankpointRankInfo info, CUIFormScript rankForm = null)
		{
			if (rankForm == null)
			{
				rankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
			}
			if (rankForm == null || info == null)
			{
				return;
			}
			Transform transform = rankForm.GetWidget(7).transform;
			Image component = rankForm.GetWidget(0).GetComponent<Image>();
			Text component2 = rankForm.GetWidget(1).GetComponent<Text>();
			Text component3 = rankForm.GetWidget(2).GetComponent<Text>();
			Text component4 = rankForm.GetWidget(3).GetComponent<Text>();
			Transform transform2 = rankForm.GetWidget(9).transform;
			Image component5 = rankForm.GetWidget(10).GetComponent<Image>();
			enGuildRankpointRankListType rankListTypeByTabSelectedIndex = this.GetRankListTypeByTabSelectedIndex(rankForm);
			if (rankListTypeByTabSelectedIndex == enGuildRankpointRankListType.SeasonSelf && info.rankNo > 0u && this.m_Model.RankpointRankInfoLists[(int)rankListTypeByTabSelectedIndex].Count > 0)
			{
				uint rankNo = this.m_Model.RankpointRankInfoLists[(int)rankListTypeByTabSelectedIndex][0].rankNo;
				CUICommonSystem.SetRankDisplay(info.rankNo - this.CalculateRankNoOffset(rankListTypeByTabSelectedIndex, rankNo), transform);
			}
			else
			{
				CUICommonSystem.SetRankDisplay(info.rankNo, transform);
			}
			component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.guildHeadId, rankForm, true, false, false, false);
			component2.text = info.guildName;
			component3.text = info.rankScore.ToString();
			component4.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Member_Format", new string[]
			{
				info.memberNum.ToString(),
				CGuildHelper.GetMaxGuildMemberCountByLevel((int)info.guildLevel).ToString()
			});
			CGuildHelper.SetStarLevelPanel(info.star, transform2, rankForm);
			component5.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.rankScore), rankForm, true, false, false, false);
			GameObject gameObject = rankForm.GetWidget(4).gameObject;
			this.SetRankpointAwardPanel(rankForm, gameObject, CGuildHelper.IsWeekRankpointRank(rankListTypeByTabSelectedIndex), info.rankNo, CGuildHelper.GetGradeByRankpointScore(info.rankScore));
		}

		private void SetRankpointAwardPanel(CUIFormScript form, GameObject awardPanel, bool isWeekAward, uint rankNo, uint grade)
		{
			if (form == null || awardPanel == null)
			{
				return;
			}
			Transform transform = awardPanel.transform;
			Image component = transform.Find("imgMoney").GetComponent<Image>();
			Text component2 = transform.Find("txtMoney").GetComponent<Text>();
			if (component == null || component2 == null)
			{
				return;
			}
			if (isWeekAward)
			{
				component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90001", form, true, false, false, false);
				component2.text = CGuildHelper.GetRankpointWeekAwardCoin(rankNo).ToString();
			}
			else
			{
				component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90005", form, true, false, false, false);
				component2.text = CGuildHelper.GetRankpointSeasonAwardDiamond(grade).ToString();
			}
		}

		private void InitRankpointRankTab(CUIFormScript rankpointRankForm = null)
		{
			if (rankpointRankForm == null)
			{
				rankpointRankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
			}
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_This_Week_Rank"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Last_Week_Rank"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Season_Rank")
			};
			CUIListScript component = rankpointRankForm.GetWidget(5).GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
			component.SelectElement(0, true);
		}

		private void InitRankpointSeasonRankTab(CUIFormScript rankpointRankForm = null)
		{
			if (rankpointRankForm == null)
			{
				rankpointRankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
			}
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Current_Grade_Rank"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Best_Rank")
			};
			GameObject widget = rankpointRankForm.GetWidget(8);
			widget.CustomSetActive(false);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
		}

		public void RefreshRankpointRankList(CUIEvent uiEvent = null)
		{
			CUIFormScript cUIFormScript;
			CUIListScript component;
			if (uiEvent == null)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
				if (cUIFormScript == null)
				{
					return;
				}
				component = cUIFormScript.GetWidget(5).GetComponent<CUIListScript>();
			}
			else
			{
				cUIFormScript = uiEvent.m_srcFormScript;
				component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			}
			int selectedIndex = component.GetSelectedIndex();
			GameObject widget = cUIFormScript.GetWidget(8);
			switch (selectedIndex)
			{
			case 0:
				widget.CustomSetActive(false);
				this.SetRankpointRankList(enGuildRankpointRankListType.CurrentWeek, cUIFormScript);
				break;
			case 1:
				widget.CustomSetActive(false);
				this.SetRankpointRankList(enGuildRankpointRankListType.LastWeek, cUIFormScript);
				break;
			case 2:
			{
				widget.CustomSetActive(true);
				CUIListScript component2 = widget.GetComponent<CUIListScript>();
				component2.SelectElement(0, true);
				break;
			}
			}
		}

		private void SetRankpointRankListWidgetPositionAndSize(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			GameObject widget = srcFormScript.GetWidget(6);
			RectTransform rectTransform = widget.transform as RectTransform;
			CUIListScript component = srcFormScript.GetWidget(5).GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			GameObject widget2 = srcFormScript.GetWidget(8);
			float num = (widget2.transform as RectTransform).rect.height / 2f;
			if (selectedIndex == 0 || selectedIndex == 1)
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, CGuildInfoView.s_rankpointRankListWidgetAnchoredPositionY + num);
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CGuildInfoView.s_rankpointRankListWidgetSizeDeltaY + num);
			}
			else if (selectedIndex == 2)
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, CGuildInfoView.s_rankpointRankListWidgetAnchoredPositionY);
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CGuildInfoView.s_rankpointRankListWidgetSizeDeltaY);
			}
		}

		private void SetRankpointRankList(enGuildRankpointRankListType rankListType, CUIFormScript rankForm)
		{
			if (this.m_Model.RankpointRankGottens[(int)rankListType])
			{
				CUIListScript component = rankForm.GetWidget(6).GetComponent<CUIListScript>();
				component.SetElementAmount(this.m_Model.RankpointRankInfoLists[(int)rankListType].Count);
				if (component.GetElementAmount() > 0)
				{
					component.SelectElement(0, true);
				}
				RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(rankListType);
				this.SetRankpointPlayerGuildRank(playerGuildRankpointRankInfo, rankForm);
			}
			else
			{
				this.ClearRankpointRankList(rankForm);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<enGuildRankpointRankListType>("Guild_Request_Rankpoint_Rank_List", rankListType);
			}
		}

		private void ClearRankpointRankList(CUIFormScript rankForm)
		{
			CUIListScript component = rankForm.GetWidget(6).GetComponent<CUIListScript>();
			component.SetElementAmount(0);
		}

		public void RefreshRankpointSeasonRankList(CUIEvent uiEvent = null)
		{
			CUIFormScript cUIFormScript;
			CUIListScript component;
			if (uiEvent == null)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
				if (cUIFormScript == null)
				{
					return;
				}
				component = cUIFormScript.GetWidget(8).GetComponent<CUIListScript>();
			}
			else
			{
				cUIFormScript = uiEvent.m_srcFormScript;
				component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			}
			int selectedIndex = component.GetSelectedIndex();
			if (selectedIndex == 0)
			{
				this.SetRankpointRankList(enGuildRankpointRankListType.SeasonSelf, cUIFormScript);
			}
			else if (selectedIndex == 1)
			{
				this.SetRankpointRankList(enGuildRankpointRankListType.SeasonBest, cUIFormScript);
			}
		}

		private void SetGuildIcon(CUIListElementScript listElementScript, ResGuildIcon info)
		{
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.dwIcon;
			Image component = listElementScript.transform.Find("imgIcon").GetComponent<Image>();
			component.SetSprite(prefabPath, listElementScript.m_belongedFormScript, true, false, false, false);
			Text component2 = listElementScript.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>();
			component2.text = info.dwIcon.ToString();
		}

		public void RefreshInfoPanelGuildIcon()
		{
			if (this.m_form == null)
			{
				return;
			}
			Image component = this.m_form.GetWidget(3).GetComponent<Image>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
		}

		public void RefreshInfoPanelGuildBulletin()
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(7).GetComponent<Text>();
			component.text = this.m_Model.CurrentGuildInfo.briefInfo.sBulletin;
		}

		public void RefreshInfoPanelGuildMemberCount()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(6);
			if (widget == null)
			{
				DebugHelper.Assert(false, "CGuildInfoView.RefreshInfoPanelGuildMemberCount(): guildMemberCountGo is null!!!");
				return;
			}
			Text component = widget.GetComponent<Text>();
			component.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Member_Count_Format", new string[]
			{
				this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum.ToString(),
				CGuildHelper.GetMaxGuildMemberCountByLevel((int)this.m_Model.CurrentGuildInfo.briefInfo.bLevel).ToString()
			});
			GameObject widget2 = this.m_form.GetWidget(11);
			widget2.CustomSetActive(!CGuildHelper.IsGuildMaxLevel((int)this.m_Model.CurrentGuildInfo.briefInfo.bLevel));
		}

		public void RefreshProfitPanel()
		{
			Text component = this.m_form.GetWidget(12).GetComponent<Text>();
			component.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Profit_Desc", new string[]
			{
				CGuildHelper.GetCoinProfitPercentage(CGuildHelper.GetGuildLevel()).ToString()
			});
		}

		public void On_Guild_Preview_Request_Ranking_List(CUIEvent uiEvent)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Guild_Preview_Get_Ranking_List", 0, 20);
		}

		public void OpenGuildPreviewForm(bool isHideListExtraContent = false)
		{
			Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab", false, true);
			this.RefreshPreviewForm(false);
		}

		public void RefreshPreviewForm(bool isHideListExtraContent = false)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab");
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(1);
			GameObject widget3 = form.GetWidget(2);
			GameObject widget4 = form.GetWidget(3);
			widget4.CustomSetActive(false);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			int guildInfoCount = this.m_Model.GetGuildInfoCount();
			component.SetElementAmount(guildInfoCount);
			if (guildInfoCount > 0)
			{
				component.SelectElement(0, true);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(true);
			}
			else
			{
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
			}
			if (isHideListExtraContent)
			{
				component.HideExtraContent();
			}
		}

		private void SetPreviewGuildListItem(CUIListElementScript listElementScript, GuildInfo info)
		{
			Transform transform = listElementScript.transform;
			Image component = transform.Find("imgIconBg/imgIcon").GetComponent<Image>();
			Text component2 = transform.Find("pnlName/txtName").GetComponent<Text>();
			Image component3 = transform.Find("pnlName/imgGrade").GetComponent<Image>();
			Text component4 = transform.Find("txtMemCnt").GetComponent<Text>();
			GameObject gameObject = transform.Find("imgInvited").gameObject;
			Text component5 = transform.Find("txtChairmanName").GetComponent<Text>();
			Transform panelTransform = transform.Find("pnlStarLevel");
			Text component6 = transform.Find("pnlPoint/txtPointNum").GetComponent<Text>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
			component2.text = info.briefInfo.sName;
			component3.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.RankInfo.totalRankPoint), this.m_form, true, false, false, false);
			component4.text = info.briefInfo.bMemberNum + "/" + CGuildHelper.GetMaxGuildMemberCountByLevel((int)info.briefInfo.bLevel);
			component5.text = info.chairman.stBriefInfo.sName;
			CGuildHelper.SetStarLevelPanel(info.star, panelTransform, listElementScript.m_belongedFormScript);
			component6.text = info.RankInfo.totalRankPoint.ToString();
			if (info.briefInfo.uulUid == this.m_Model.m_InviteGuildUuid && !Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				gameObject.CustomSetActive(true);
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		private void SetGuildMemberListItem(CUIListElementScript listElementScript, GuildMemInfo memberInfo)
		{
			Transform transform = listElementScript.transform;
			CUIHttpImageScript component = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
			Image component2 = component.transform.Find("NobeIcon").GetComponent<Image>();
			Image component3 = component.transform.Find("NobeImag").GetComponent<Image>();
			Text component4 = transform.Find("txtName").GetComponent<Text>();
			GameObject gameObject = transform.Find("pnlPosition/imgPositionChairman").gameObject;
			GameObject gameObject2 = transform.Find("pnlPosition/imgPositionOther").gameObject;
			Text component5 = transform.Find("pnlPosition/txtPosition").GetComponent<Text>();
			GameObject gameObject3 = transform.Find("pnlPosition/imgLeader").gameObject;
			Text component6 = transform.Find("txtWeekRankpoint").GetComponent<Text>();
			Text component7 = transform.Find("txtSeasonRankpoint").GetComponent<Text>();
			Text component8 = transform.Find("txtOnline").GetComponent<Text>();
			component.SetImageUrl(CGuildHelper.GetHeadUrl(memberInfo.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(memberInfo.stBriefInfo.uulUid, memberInfo.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(memberInfo.stBriefInfo.uulUid, memberInfo.stBriefInfo.stVip.headIconId));
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component3, CGuildHelper.GetNobeHeadIconId(memberInfo.stBriefInfo.uulUid, memberInfo.stBriefInfo.stVip.headIconId), this.m_form, 0.77f, true);
			component4.text = memberInfo.stBriefInfo.sName;
			gameObject.CustomSetActive(COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN == memberInfo.enPosition);
			gameObject2.CustomSetActive(COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN != memberInfo.enPosition);
			component5.text = CGuildHelper.GetPositionName(memberInfo.enPosition);
			gameObject3.CustomSetActive(Convert.ToBoolean(memberInfo.GuildMatchInfo.bIsLeader));
			component6.text = memberInfo.RankInfo.weekRankPoint.ToString();
			component7.text = memberInfo.RankInfo.totalRankPoint.ToString();
			if (CGuildHelper.IsMemberOnline(memberInfo))
			{
				component8.text = Singleton<CTextManager>.GetInstance().GetText("Common_Online");
			}
			else
			{
				component8.text = Utility.GetRecentOnlineTimeString((int)memberInfo.LastLoginTime);
			}
		}

		public void RefreshGuildName()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
			if (form != null)
			{
				Text component = form.GetWidget(5).GetComponent<Text>();
				component.text = this.m_Model.CurrentGuildInfo.briefInfo.sName;
			}
			if (this.m_form != null)
			{
				Text component2 = this.m_form.GetWidget(4).GetComponent<Text>();
				component2.text = this.m_Model.CurrentGuildInfo.briefInfo.sName;
			}
		}

		public void On_Guild_Preview_Guild_List_Element_Select(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIHttpImageScript component = srcFormScript.GetWidget(7).GetComponent<CUIHttpImageScript>();
			Image component2 = component.transform.Find("NobeIcon").GetComponent<Image>();
			Image component3 = component.transform.Find("NobeImag").GetComponent<Image>();
			Text component4 = srcFormScript.GetWidget(5).GetComponent<Text>();
			Text component5 = srcFormScript.GetWidget(8).GetComponent<Text>();
			Text component6 = srcFormScript.GetWidget(6).GetComponent<Text>();
			GameObject widget = srcFormScript.GetWidget(3);
			CUIListScript cUIListScript = (CUIListScript)uiEvent.m_srcWidgetScript;
			GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(cUIListScript.GetSelectedIndex());
			if (guildInfoByIndex == null)
			{
				DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Preview_Guild_List_Element_Select(): guildInfo is null, uiEvent.m_srcWidgetIndexInBelongedList={0}", new object[]
				{
					uiEvent.m_srcWidgetIndexInBelongedList
				});
				return;
			}
			component.SetImageUrl(CGuildHelper.GetHeadUrl(guildInfoByIndex.chairman.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.headIconId));
			component4.text = guildInfoByIndex.chairman.stBriefInfo.sName;
			component5.text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", new string[]
			{
				guildInfoByIndex.chairman.stBriefInfo.dwLevel.ToString()
			});
			component6.text = guildInfoByIndex.briefInfo.sBulletin;
			widget.CustomSetActive(guildInfoByIndex.briefInfo.uulUid == this.m_Model.m_InviteGuildUuid);
		}

		private void On_Guild_Preview_Guild_List_Element_Enabled(CUIEvent uiEvent)
		{
			GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
			if (guildInfoByIndex != null)
			{
				this.SetPreviewGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, guildInfoByIndex);
			}
		}

		private void On_Guild_Preview_Request_More_Guild_List(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = srcFormScript.GetWidget(0).GetComponent<CUIListScript>();
			int elementAmount = component.GetElementAmount();
			if (elementAmount > 0)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", elementAmount + 1, 20);
			}
		}

		private void On_Guild_Guild_Search(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			string text = srcFormScript.GetWidget(4).GetComponent<InputField>().text;
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CGuildSystem>.GetInstance().SearchGuild(0uL, 0, text, 0, false);
			}
		}

		private void On_Guild_Accept_Invite(CUIEvent uiEvent)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Accept_Invite");
		}

		private void On_Guild_Member_List_Element_Enabled(CUIEvent uiEvent)
		{
			if (0 <= uiEvent.m_srcWidgetIndexInBelongedList && uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.CurrentGuildInfo.listMemInfo.Count)
			{
				GuildMemInfo guildMemInfo = this.m_Model.CurrentGuildInfo.listMemInfo[uiEvent.m_srcWidgetIndexInBelongedList];
				if (guildMemInfo != null)
				{
					this.SetGuildMemberListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, guildMemInfo);
				}
			}
		}

		private void On_Guild_Sign(CUIEvent uiEvent)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Sign");
		}

		private void On_Guild_Open_Log_Form(CUIEvent uiEvent)
		{
			Singleton<CGuildInfoController>.GetInstance().RequestGetGuildEvent();
		}

		private void On_Guild_Member_List_Sort_Position_Desc(CUIEvent uiEvent)
		{
			if (this.m_curGuildMemberListSortType != CGuildInfoView.enGuildMemberListSortType.PositionDesc)
			{
				this.m_curGuildMemberListSortType = CGuildInfoView.enGuildMemberListSortType.PositionDesc;
				this.RefreshMemberList();
			}
		}

		private void On_Guild_Member_List_Sort_Week_Rankpoint_Desc(CUIEvent uiEvent)
		{
			if (this.m_curGuildMemberListSortType != CGuildInfoView.enGuildMemberListSortType.WeekRankpointDesc)
			{
				this.m_curGuildMemberListSortType = CGuildInfoView.enGuildMemberListSortType.WeekRankpointDesc;
				this.RefreshMemberList();
			}
		}

		private void On_Guild_Member_List_Sort_Season_Rankpoint_Desc(CUIEvent uiEvent)
		{
			if (this.m_curGuildMemberListSortType != CGuildInfoView.enGuildMemberListSortType.SeasonRankpointDesc)
			{
				this.m_curGuildMemberListSortType = CGuildInfoView.enGuildMemberListSortType.SeasonRankpointDesc;
				this.RefreshMemberList();
			}
		}

		private void On_Guild_Member_List_Sort_Online_Status_Desc(CUIEvent uiEvent)
		{
			if (this.m_curGuildMemberListSortType != CGuildInfoView.enGuildMemberListSortType.OnlineStatus)
			{
				this.m_curGuildMemberListSortType = CGuildInfoView.enGuildMemberListSortType.OnlineStatus;
				this.RefreshMemberList();
			}
		}

		private void On_Guild_Recruit_Send_Recruit(CUIEvent uiEvent)
		{
			Singleton<CGuildInfoController>.GetInstance().RequestSendGuildRecruitReq();
			GameObject widget = this.m_form.GetWidget(39);
			widget.CustomSetActive(true);
			CUITimerScript component = widget.GetComponent<CUITimerScript>();
			uint guildSendRecruitCd = this.GetGuildSendRecruitCd();
			component.SetTotalTime(guildSendRecruitCd);
			component.StartTimer();
			this.m_lastSendReruitTime = CRoleInfo.GetCurrentUTCTime();
			Button component2 = this.m_form.GetWidget(38).GetComponent<Button>();
			CUICommonSystem.SetButtonEnable(component2, false, false, true);
			GameObject widget2 = this.m_form.GetWidget(40);
			widget2.CustomSetActive(false);
		}

		private void On_Guild_Recruit_Send_Recruit_CD_Timeout(CUIEvent uiEvent)
		{
			GameObject widget = this.m_form.GetWidget(40);
			widget.CustomSetActive(true);
			Button component = this.m_form.GetWidget(38).GetComponent<Button>();
			CUICommonSystem.SetButtonEnable(component, true, true, true);
			GameObject widget2 = this.m_form.GetWidget(39);
			widget2.CustomSetActive(false);
		}

		private uint GetGuildSendRecruitCd()
		{
			return GameDataMgr.guildMiscDatabin.GetDataByKey(52u).dwConfValue;
		}

		private void DisplayGuildMemberListSortBtn()
		{
			GameObject widget = this.m_form.GetWidget(34);
			GameObject widget2 = this.m_form.GetWidget(35);
			GameObject widget3 = this.m_form.GetWidget(36);
			GameObject widget4 = this.m_form.GetWidget(37);
			switch (this.m_curGuildMemberListSortType)
			{
			case CGuildInfoView.enGuildMemberListSortType.Default:
			case CGuildInfoView.enGuildMemberListSortType.PositionDesc:
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(false);
				break;
			case CGuildInfoView.enGuildMemberListSortType.WeekRankpointDesc:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(false);
				break;
			case CGuildInfoView.enGuildMemberListSortType.SeasonRankpointDesc:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(true);
				widget4.CustomSetActive(false);
				break;
			case CGuildInfoView.enGuildMemberListSortType.OnlineStatus:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(true);
				break;
			}
		}

		private bool IsShowApplyRedDot()
		{
			return CGuildSystem.HasManageAuthority() && !this.IsApplyListCloseRedDotPrefOn() && !CGuildSystem.s_isApplyAndRecommendListEmpty;
		}

		private bool IsApplyListCloseRedDotPrefOn()
		{
			return PlayerPrefs.GetInt("Guild_ApplyList_CloseRedDot") > 0;
		}

		public void RefreshMemberTabRedDot()
		{
			if (this.m_form == null)
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(2).GetComponent<CUIListScript>();
			GameObject gameObject = component.GetElemenet(1).gameObject;
			if (this.IsShowApplyRedDot())
			{
				CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(gameObject);
			}
		}

		public void RefreshMemberPanelApplyBtnRedDot()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(13);
			if (this.IsShowApplyRedDot())
			{
				CUICommonSystem.AddRedDot(widget, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(widget);
			}
		}

		public void RefreshApplyRedDot()
		{
			this.RefreshMemberTabRedDot();
			this.RefreshMemberPanelApplyBtnRedDot();
		}

		public void RefreshInfoPanelSignBtn()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(17);
			Button component = widget.GetComponent<Button>();
			if (CGuildHelper.IsPlayerSigned())
			{
				CUICommonSystem.DelRedDot(widget);
				CUICommonSystem.SetButtonEnable(component, false, false, true);
				CUICommonSystem.SetButtonName(widget, Singleton<CTextManager>.GetInstance().GetText("Common_Signed"));
			}
			else
			{
				CUICommonSystem.AddRedDot(widget, enRedDotPos.enTopRight, 0, 0, 0);
				CUICommonSystem.SetButtonEnable(component, true, true, true);
				Text componentInChildren = widget.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.color = CGuildInfoView.s_Text_Color_Btn_Sign_Default;
				}
				CUICommonSystem.SetButtonName(widget, Singleton<CTextManager>.GetInstance().GetText("Common_Sign"));
			}
		}

		public void OpenSignSuccessForm(string tip)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Sign_Success.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
			component.text = tip;
		}

		public void OpenLogForm(SCPKG_GUILD_EVENT_RSP rsp)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Log.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(1);
			widget.CustomSetActive(rsp.bNum == 0);
			Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
			component.text = string.Empty;
			for (int i = (int)(rsp.bNum - 1); i >= 0; i--)
			{
				uint dwEventTime = rsp.astContent[i].dwEventTime;
				string str = "<color=#717b93>[" + Utility.GetUtcToLocalTimeStringFormat((ulong)dwEventTime, "yyyy.M.d") + "]</color>";
				string str2 = StringHelper.UTF8BytesToString(ref rsp.astContent[i].szContent);
				Text expr_9E = component;
				expr_9E.text = expr_9E.text + str + str2 + "\n";
			}
		}

		private void On_Guild_BindPlatformGroup(CUIEvent uiEvent)
		{
			if (!this.PlatformCheck())
			{
				return;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_PlatformGroup_Request_Group_Guild_Id");
		}

		private void On_Guide_Join_PlatformGroup(CUIEvent uiEvent)
		{
			switch (this.m_Model.PlatformGroupStatus)
			{
			case CGuildSystem.enPlatformGroupStatus.Bound:
				if (!this.m_Model.IsSelfInPlatformGroup)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_JoinPlatformGroup);
				}
				break;
			case CGuildSystem.enPlatformGroupStatus.UnBound:
				this.SendRemindMsg();
				break;
			case CGuildSystem.enPlatformGroupStatus.Resolve:
				this.NeedAutoOp = true;
				break;
			}
		}

		private void On_Guide_Bind_PlatformGroup(CUIEvent uiEvent)
		{
			switch (this.m_Model.PlatformGroupStatus)
			{
			case CGuildSystem.enPlatformGroupStatus.Bound:
				this.SendRemindMsg();
				break;
			case CGuildSystem.enPlatformGroupStatus.UnBound:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_BindPlatformGroup);
				break;
			case CGuildSystem.enPlatformGroupStatus.Resolve:
				this.NeedAutoOp = true;
				break;
			}
		}

		private bool PlatformCheck()
		{
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
			{
				if (!Singleton<ApolloHelper>.GetInstance().IsPlatformInstalled(ApolloPlatform.Wechat))
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Platform_Not_Installed"), false);
					return false;
				}
			}
			else if (!Singleton<ApolloHelper>.GetInstance().IsPlatformInstalled(ApolloPlatform.QQ))
			{
				return false;
			}
			return true;
		}

		private void On_Guild_RemindPlatformGroup(CUIEvent uiEvent)
		{
			this.SendRemindMsg();
		}

		private void On_Guild_UnBindPlatformGroup(CUIEvent uiEvent)
		{
			string strContent = string.Empty;
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				strContent = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Unbind_Confirm");
			}
			else
			{
				strContent = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Unbind_Confirm");
			}
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Guild_UnBindPlatformGroupConfirm, enUIEventID.None, false);
		}

		private void On_Guild_UnBindPlatformGroupConfirm(CUIEvent uiEvent)
		{
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
				if (iApolloSnsService == null)
				{
					return;
				}
				string groupOpenId = this.m_Model.CurrentGuildInfo.groupOpenId;
				string text = CGuildHelper.GetGroupGuildId().ToString();
				if (!string.IsNullOrEmpty(groupOpenId))
				{
					this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
					iApolloSnsService.UnbindQQGroup(groupOpenId, text);
					Debug.Log(string.Format("**GuildPlatformGroup** On_Guild_UnBindPlatformGroupConfirm guildOpenId={0} guild32BitsUid={1}", groupOpenId, text));
				}
				else
				{
					Debug.Log("**GuildPlatformGroup** On_Guild_UnBindPlatformGroupConfirm guildOpenId is null");
					Singleton<CUIManager>.GetInstance().OpenTips(string.Format("{0}{1}", Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_UnBind_Err_Sys"), " guildOpenId is null"), false, 1.5f, null, new object[0]);
				}
			}
			else
			{
				this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
				MonoSingleton<ShareSys>.GetInstance().StartCoroutine(this.UnLinkWxGroup(new Action<string>(this.UnLinkWxGroupRsConvert)));
			}
		}

		private void On_Guild_JoinPlatformGroup(CUIEvent uiEvent)
		{
			if (!this.PlatformCheck())
			{
				return;
			}
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService == null)
			{
				return;
			}
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				if (!string.IsNullOrEmpty(this.m_Model.CurrentGuildInfo.groupKey))
				{
					this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
					iApolloSnsService.JoinQQGroup(this.m_Model.CurrentGuildInfo.groupKey);
					Singleton<CTimerManager>.GetInstance().AddTimer(2000, 1, new CTimer.OnTimeUpHandler(this.QueryPlatformGroupInfo));
					Debug.Log(string.Format("**GuildPlatformGroup** On_Guild_JoinPlatformGroup JoinQQGroup: groupKey={0}", this.m_Model.CurrentGuildInfo.groupKey));
				}
				else
				{
					this.QueryQQGroupKey();
					Singleton<CUIManager>.GetInstance().OpenTips(string.Format("{0}{1}", Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Join_Err"), " group key is null"), false, 1.5f, null, new object[0]);
					Debug.Log("**GuildPlatformGroup** On_Guild_JoinPlatformGroup JoinQQGroup: group key is null!!!");
				}
			}
			else
			{
				this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					DebugHelper.Assert(false, "Master Role Info is null");
				}
				string text = (masterRoleInfo != null) ? masterRoleInfo.Name : null;
				string wxGroupUnionId = this.GetWxGroupUnionId(false);
				iApolloSnsService.JoinWXGroup(wxGroupUnionId, text);
				Debug.Log(string.Format("**GuildPlatformGroup** On_Guild_JoinPlatformGroup JoinWXGroup: unionId={0} playerName={1}", wxGroupUnionId, text));
			}
		}

		public void SendRemindMsg()
		{
			int num = 0;
			ResGuildMisc dataByKey = GameDataMgr.guildMiscDatabin.GetDataByKey(62u);
			if (dataByKey != null && dataByKey.dwConfValue > 0u)
			{
				num = (int)dataByKey.dwConfValue;
			}
			int @int = PlayerPrefs.GetInt("PLATFORM_GROUP_REMIND_OP_INTERVAL_KEY", 0);
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			if (currentUTCTime - @int <= num)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Group_Send_Remind_Err_Frequency", true, 1.5f, null, new object[0]);
				return;
			}
			int currentUTCTime2 = CRoleInfo.GetCurrentUTCTime();
			string text = string.Empty;
			if (!CGuildSystem.HasManageQQGroupAuthority())
			{
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Remind_Bind_Chat_Text");
			}
			else
			{
				string arg = Utility.CreateMD5Hash(string.Format("{0}{1}{2}", currentUTCTime2, 4042, "j9cWiS6lPjw4g"), Utility.MD5_STRING_CASE.LOWER);
				text = string.Format("[{0}|{1}|{2}]", currentUTCTime2, 4042, arg);
			}
			CChatNetUT.Send_Guild_Chat(text);
			CChatNetUT.Send_GetChat_Req(EChatChannel.Guild);
			Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Group_Send_Remind_Success", true, 1.5f, null, new object[0]);
			PlayerPrefs.SetInt("PLATFORM_GROUP_REMIND_OP_INTERVAL_KEY", currentUTCTime);
			PlayerPrefs.Save();
		}

		public void BindPlatformGroup()
		{
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService == null)
			{
				return;
			}
			string text = CGuildHelper.GetGroupGuildId().ToString();
			string text2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Bind_Guild_Name", new string[]
			{
				CGuildHelper.GetGuildName()
			}), new object[0]);
			string text3 = CGuildHelper.GetGuildLogicWorldId().ToString();
			this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				string bindQQGroupSignature = CGuildHelper.GetBindQQGroupSignature();
				iApolloSnsService.BindQQGroup(text, text2, text3, bindQQGroupSignature);
				if (this.m_form != null)
				{
					GameObject widget = this.m_form.GetWidget(19);
					if (widget != null)
					{
						widget.CustomSetActive(false);
					}
				}
				Debug.Log(string.Format("**GuildPlatformGroup** BindQQGroup guild32BitsUid={0} guildName={1} guildZoneId={2} guildSignatureMd5={3}", new object[]
				{
					text,
					text2,
					text3,
					bindQQGroupSignature
				}));
			}
			else
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					DebugHelper.Assert(false, "Master Role Info is null");
				}
				string text4 = (masterRoleInfo != null) ? masterRoleInfo.Name : null;
				string wxGroupUnionId = this.GetWxGroupUnionId(false);
				iApolloSnsService.CreateWXGroup(wxGroupUnionId, text2, text4);
				Debug.Log(string.Format("**GuildPlatformGroup** CreateWXGroup unionId={0} guildName={1} playerName={2}", wxGroupUnionId, text2, text4));
			}
		}

		private void OnQueryPlatformGroupInfoNotify(ApolloGroupResult groupRet)
		{
			string text = string.Empty;
			CGuildSystem.enPlatformGroupStatus enPlatformGroupStatus;
			bool flag;
			if (groupRet.result == ApolloResult.Success)
			{
				enPlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.Bound;
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
				{
					flag = true;
					this.m_Model.CurrentGuildInfo.groupKey = groupRet.mQQGroupInfo.groupKey;
					if (this.m_Model.CurrentGuildInfo.groupOpenId != groupRet.mQQGroupInfo.groupOpenid)
					{
						Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", groupRet.mQQGroupInfo.groupOpenid);
					}
					Debug.Log(string.Format("**GuildPlatformGroup** OnQueryPlatformGroupInfoNotify Old groupOpenId={0} New groupOpenId={1}", this.m_Model.CurrentGuildInfo.groupOpenId, groupRet.mQQGroupInfo.groupOpenid));
					this.m_Model.CurrentGuildInfo.groupOpenId = groupRet.mQQGroupInfo.groupOpenid;
				}
				else
				{
					ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(true);
					if (accountInfo != null)
					{
						flag = (!string.IsNullOrEmpty(groupRet.mWXGroupInfo.openIdList) && groupRet.mWXGroupInfo.openIdList.IndexOf(accountInfo.OpenId) != -1);
						string wxGroupUnionId = this.GetWxGroupUnionId(false);
						if (this.m_Model.CurrentGuildInfo.groupOpenId != wxGroupUnionId)
						{
							Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", wxGroupUnionId);
						}
						this.m_Model.CurrentGuildInfo.groupOpenId = wxGroupUnionId;
					}
					else
					{
						enPlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.Resolve;
						flag = false;
						Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Query_Group_Info_Err", true, 1.5f, null, new object[0]);
					}
				}
				if (this.m_needSendRemindMsg)
				{
					this.m_needSendRemindMsg = false;
					this.SendRemindMsg();
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Group_Bind_Success", true, 1.5f, null, new object[0]);
					Singleton<CGuildInfoController>.GetInstance().SendPlatformGroupLog(enPlatformGroupStatus, groupRet.mQQGroupInfo.groupOpenid);
				}
			}
			else
			{
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
				{
					if (groupRet.errorCode == 2003)
					{
						enPlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.Bound;
						flag = false;
						Debug.Log(string.Format("**GuildPlatformGroup** OnQueryPlatformGroupInfoNotify groupOpenid={0}", this.m_Model.CurrentGuildInfo.groupOpenId));
						this.QueryQQGroupKey();
					}
					else
					{
						enPlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.UnBound;
						flag = false;
						text = this.GetQueryQQErrMsg(groupRet);
						Debug.Log(string.Format("**GuildPlatformGroup** OnQueryPlatformGroupInfoNotify boundStatus={0} isSelfInGroup={1} groupName={2} resultMsg={3} groupOpenid={4}", new object[]
						{
							enPlatformGroupStatus,
							flag,
							groupRet.mQQGroupInfo.groupName,
							text,
							groupRet.mQQGroupInfo.groupOpenid
						}));
					}
				}
				else
				{
					enPlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.UnBound;
					flag = false;
					text = this.GetWxErrMsg(groupRet);
					Debug.Log(string.Format("**GuildPlatformGroup** OnQueryPlatformGroupInfoNotify boundStatus={0} isSelfInGroup={1} chatRoomURL={2} resultMsg={3}", new object[]
					{
						enPlatformGroupStatus,
						flag,
						groupRet.mWXGroupInfo.chatRoomURL,
						text
					}));
				}
				Debug.Log(string.Format("**GuildPlatformGroup** OnQueryPlatformGroupInfoNotify boundStatus={0} isSelfInGroup={1} groupName={2} resultMsg={3} resultCode={4}", new object[]
				{
					enPlatformGroupStatus,
					flag,
					null,
					text,
					groupRet.result
				}));
			}
			this.m_Model.SetPlatformGroupStatus(enPlatformGroupStatus, flag);
		}

		public void QueryPlatformGroupInfo(int sequence)
		{
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				this.QueryQQGroupInfo(sequence);
			}
			else
			{
				this.QueryWxGroupInfo(sequence);
			}
		}

		private void OnSendGuildMatchInviteToPlatformGroupNotify(bool rs)
		{
			if (rs)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_WXGroup_Send_Guild_Match_Invite_Success", true, 1.5f, null, new object[0]);
			}
		}

		private void OnPlatformGroupStatusChange(CGuildSystem.enPlatformGroupStatus status, bool isSelfInGroup)
		{
			this.RefreshPlatformGroupPanel(status, isSelfInGroup);
			this.RefreshPlatformGroupPanelInSettingForm(status, isSelfInGroup);
		}

		public string GetWxGroupUnionId(bool needIncrease = false)
		{
			string text = CGuildHelper.GetGuildLogicWorldId().ToString();
			string arg = text.PadLeft(5, '0');
			string arg2 = Convert.ToString(CGuildHelper.GetGroupGuildId()).PadLeft(32, '0');
			string arg3 = Convert.ToString(this.GetWxIncreasingId(false)).PadLeft(9, '0');
			return string.Format("{0}{1}{2}", arg, arg2, arg3);
		}

		private uint GetWxIncreasingId(bool needIncrease)
		{
			uint num;
			if (string.IsNullOrEmpty(this.m_Model.CurrentGuildInfo.groupOpenId))
			{
				num = 0u;
			}
			else
			{
				num = Convert.ToUInt32(this.m_Model.CurrentGuildInfo.groupOpenId.Substring(5, 9));
			}
			if (needIncrease)
			{
				num += 1u;
			}
			return num;
		}

		private void QueryWxGroupInfo(int sequence)
		{
			Debug.Log(string.Format("**GuildPlatformGroup** QueryWxGroupInfo Enter", new object[0]));
			this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.Resolve, false);
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(true);
			if (iApolloSnsService != null && accountInfo != null)
			{
				Debug.Log(string.Format("**GuildPlatformGroup** QueryWxGroupInfo try to queryWXGroupInfo", new object[0]));
				string wxGroupUnionId = this.GetWxGroupUnionId(false);
				string openId = accountInfo.OpenId;
				Debug.Log(string.Format("**GuildPlatformGroup** QueryWxGroupInfo unionId={0} openId={1}", wxGroupUnionId, openId));
				iApolloSnsService.QueryWXGroupInfo(wxGroupUnionId, openId);
			}
			else
			{
				Debug.Log(string.Format("**GuildPlatformGroup** QueryWxGroupInfo sns or accountInfo is null", new object[0]));
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Query_Group_Info_Err_Account_Info_Null", true, 1.5f, null, new object[0]);
			}
		}

		private void OnCreateWXGroupNotify(ApolloGroupResult groupRet)
		{
			if (groupRet.result != ApolloResult.Success)
			{
				if (groupRet.errorCode == -10005)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_WXGroup_Bind_Err_Group_Exist", true, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.GetWxErrMsg(groupRet), false, 1.5f, null, new object[0]);
				}
			}
			Debug.Log(string.Format("**GuildPlatformGroup** OnCreateWXGroupNotify groupRet={0} errorCode={1}", groupRet.result, groupRet.errorCode));
			this.m_needSendRemindMsg = true;
			Singleton<CTimerManager>.GetInstance().AddTimer(2000, 1, new CTimer.OnTimeUpHandler(this.QueryPlatformGroupInfo));
		}

		private void OnJoinWXGroupNotify(ApolloGroupResult groupRet)
		{
			Debug.Log(string.Format("**GuildPlatformGroup** OnJoinWXGroupNotify groupRet={0} msg={1}, rs={2}", groupRet.errorCode, groupRet.desc, groupRet.result));
			Singleton<CTimerManager>.GetInstance().AddTimer(2000, 1, new CTimer.OnTimeUpHandler(this.QueryPlatformGroupInfo));
		}

		private void OnUnLinkWXGroupNotify(ApolloGroupResult groupRet)
		{
			string wxErrMsg = this.GetWxErrMsg(groupRet);
			Singleton<CUIManager>.GetInstance().OpenTips(wxErrMsg, false, 1.5f, null, new object[0]);
			Debug.Log(string.Format("**GuildPlatformGroup** OnUnLinkWXGroupNotify resultMsg={0}", wxErrMsg));
			if (groupRet.result == ApolloResult.Success)
			{
				this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.UnBound, false);
				Singleton<CGuildInfoController>.GetInstance().SendPlatformGroupLog(CGuildSystem.enPlatformGroupStatus.UnBound, this.m_Model.CurrentGuildInfo.groupOpenId);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", string.Empty);
			}
		}

		private void UnLinkWxGroupRsConvert(string s)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(s) as Dictionary<string, object>;
			ApolloGroupResult apolloGroupResult = new ApolloGroupResult();
			if (dictionary == null)
			{
				Debug.Log(string.Format("**GuildPlatformGroup** UnLinkWxGroupRsConvert json={0}", s));
				apolloGroupResult.result = ApolloResult.Error;
				apolloGroupResult.errorCode = -2;
				apolloGroupResult.desc = s;
			}
			else
			{
				int num = Convert.ToInt32(dictionary["ret"]);
				string text = Convert.ToString(dictionary["msg"]);
				if (num != 0)
				{
					int num2 = num;
					if (num2 != -20000)
					{
						apolloGroupResult.result = ApolloResult.Error;
						apolloGroupResult.errorCode = num;
						apolloGroupResult.desc = text;
					}
					else
					{
						int num3 = text.IndexOf(",");
						if (num3 != -1)
						{
							int errorCode = Convert.ToInt32(text.Substring(0, num3 + 1));
							string desc = text.Substring(num3 + 1);
							apolloGroupResult.result = ApolloResult.Error;
							apolloGroupResult.errorCode = errorCode;
							apolloGroupResult.desc = desc;
						}
						else
						{
							apolloGroupResult.result = ApolloResult.Error;
							apolloGroupResult.errorCode = num;
							apolloGroupResult.desc = text;
						}
					}
				}
				else
				{
					apolloGroupResult.result = ApolloResult.Success;
					apolloGroupResult.errorCode = num;
					apolloGroupResult.desc = text;
				}
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<ApolloGroupResult>("Guild_WXGroup_UnLinkWXGroupNotify", apolloGroupResult);
		}

        private IEnumerator UnLinkWxGroup(Action<string> callback)
        {
            string url = "http://msdk.qq.com/relation/wxunlink_group";
            var timestamp = Utility.ToUtcSeconds(DateTime.Now);
            var appID = Singleton<ApolloHelper>.GetInstance().GetAppId();
            appID = ApolloConfig.WXAppID;
            var sig = Utility.CreateMD5Hash("{ApolloConfig.MsdkKey}{_timestamp___1}", Utility.MD5_STRING_CASE.LOWER);
            var accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(true);
            if (accountInfo == null)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Platform_Query_Group_Info_Err_Account_Info_Null", true, 1.5f, null, new object[0]);
                Debug.Log("**GuildPlatformGroup** UnLinkWxGroup accountInfo is null!!!");
                yield break;
            }
            url = "{<url>__0}?timestamp={<timestamp>__1}&appid={<appID>__2}&sig={<sig>__3}&openid={<accountInfo>__4.OpenId}&encode=2";
            var accessToken = Singleton<ApolloHelper>.GetInstance().GetAccessToken(ApolloPlatform.Wechat);
            var unionId = GetWxGroupUnionId(false);
            var postDataDic = new Dictionary<string, string>();
            postDataDic["accessToken"] = accessToken;
            postDataDic["groupid"] = unionId;
            var postJson = Json.Serialize(postDataDic);
            var bytes = Encoding.ASCII.GetBytes(postJson.ToCharArray());
            Debug.Log("**GuildPlatformGroup** UnLinkWxGroup url={<url>__0} accessToken={<accessToken>__5} groupid={<unionId>__6}");
            var www = new WWW(url, bytes);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                if (callback != null)
                {
                    callback(www.error);
                }
                yield break;
            }

            if (callback != null)
            {
                callback(www.text);
            }
            www.Dispose();
        }

		private string GetWxErrMsg(ApolloGroupResult groupRet)
		{
			string result;
			if (groupRet.result == ApolloResult.Success)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Success");
			}
			else if (groupRet.errorCode == -1)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_System");
			}
			else if (groupRet.errorCode == -10000)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Inner");
			}
			else if (groupRet.errorCode == -10001)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_No_Auth");
			}
			else if (groupRet.errorCode == -10002)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Param");
			}
			else if (groupRet.errorCode == -10003)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Link_Not_Exist");
			}
			else if (groupRet.errorCode == -10004)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Fetch_Url");
			}
			else if (groupRet.errorCode == -10005)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Group_Duplicate");
			}
			else if (groupRet.errorCode == -10006)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_MAXIMUM");
			}
			else if (groupRet.errorCode == -10007)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Group_Not_Exist");
			}
			else if (groupRet.errorCode == -10008)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_Group_Limit_One_Day");
			}
			else
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Err_System");
			}
			return result;
		}

		private void OnBindQQGroupNotify(ApolloGroupResult groupRet)
		{
			Debug.Log(string.Format("**GuildPlatformGroup** OnBindQQGroupNotify groupRet={0} errorCode={1}", groupRet.result, groupRet.errorCode));
			this.m_needSendRemindMsg = true;
			Singleton<CTimerManager>.GetInstance().AddTimer(2000, 1, new CTimer.OnTimeUpHandler(this.QueryPlatformGroupInfo));
		}

		private void OnUnBindQQGroupNotify(ApolloGroupResult groupRet)
		{
			string text;
			if (groupRet.result == ApolloResult.Success)
			{
				this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.UnBound, false);
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Success");
				Singleton<CGuildInfoController>.GetInstance().SendPlatformGroupLog(this.m_Model.PlatformGroupStatus, this.m_Model.CurrentGuildInfo.groupOpenId);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", string.Empty);
			}
			else if (groupRet.errorCode == 2001)
			{
				text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_QQ_Group_Not_Bind");
			}
			else if (groupRet.errorCode == 2003)
			{
				text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Login_Session_Expired");
			}
			else if (groupRet.errorCode == 2004)
			{
				text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Too_Many_Operations");
			}
			else if (groupRet.errorCode == 2005)
			{
				text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Param_Error");
			}
			else
			{
				text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_System_Error");
			}
			Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
			Debug.Log(string.Format("**GuildPlatformGroup** OnUnBindQQGroupNotify resultMsg={0}", text));
		}

		public void QueryQQGroupInfo(int timerSequence)
		{
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService != null)
			{
				string text = CGuildHelper.GetGroupGuildId().ToString();
				string text2 = CGuildHelper.GetGuildLogicWorldId().ToString();
				Debug.Log(string.Format("**GuildPlatformGroup** QueryQQGroupInfo guild32BitsUid={0} zoneId={1}", text, text2));
				iApolloSnsService.QueryQQGroupInfo(text, text2);
			}
		}

		public void QueryQQGroupKey()
		{
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService != null)
			{
				Debug.Log(string.Format("**GuildPlatformGroup** QueryQQGroupKey groupOpenId={0}", this.m_Model.CurrentGuildInfo.groupOpenId));
				iApolloSnsService.QueryQQGroupKey(this.m_Model.CurrentGuildInfo.groupOpenId);
			}
		}

		private void OnQueryQQGroupKeyNotify(ApolloGroupResult groupRet)
		{
			if (groupRet.result == ApolloResult.Success)
			{
				this.m_Model.CurrentGuildInfo.groupKey = groupRet.mQQGroupInfo.groupKey;
				Debug.Log(string.Format("**GuildPlatformGroup** OnQueryQQGroupKeyNotify groupKey={0}", this.m_Model.CurrentGuildInfo.groupKey));
				if (this.NeedAutoOp)
				{
					this.NeedAutoOp = false;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_JoinPlatformGroup);
				}
			}
			else
			{
				Debug.Log(string.Format("**GuildPlatformGroup** OnQueryQQGroupKeyNotify groupRet={0} errorCode={1}", groupRet.result, groupRet.errorCode));
			}
		}

		private string GetQueryQQErrMsg(ApolloGroupResult groupRet)
		{
			string result;
			if (groupRet.errorCode == 0)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Success");
			}
			else if (groupRet.errorCode == 2002)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Group_Not_Bind");
			}
			else if (groupRet.errorCode == 2003)
			{
				result = string.Concat(new object[]
				{
					groupRet.errorCode,
					Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Not_In_Group"),
					"groupOpenid=",
					this.m_Model.CurrentGuildInfo.groupOpenId
				});
			}
			else if (groupRet.errorCode == 2007)
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Group_Not_Exist");
			}
			else
			{
				result = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_System_Error");
			}
			return result;
		}

		public void RefreshPlatformGroupPanel(CGuildSystem.enPlatformGroupStatus status, bool isSelfInGroup = false)
		{
			if (this.m_form == null)
			{
				return;
			}
			ApolloPlatform curPlatform = Singleton<ApolloHelper>.GetInstance().CurPlatform;
			if (curPlatform != ApolloPlatform.QQ && curPlatform != ApolloPlatform.Wechat)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(this.m_form.gameObject, "pnlBg/pnlInfo/pnlHead/pnlPlatformGroup");
			if (gameObject == null)
			{
				DebugHelper.Assert(false, "[CGuildInfoView] RefreshPlatformGroupPanel is null");
				return;
			}
			GameObject gameObject2 = Utility.FindChild(gameObject, "btnQQGroup");
			GameObject gameObject3 = Utility.FindChild(gameObject, "btnWxGroup");
			if (gameObject2 == null || gameObject3 == null)
			{
				DebugHelper.Assert(false, "[CGuildInfoView] qqBtnGo or wxBtnGo is null");
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject2, "Text");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject3, "Text");
			if (componetInChild == null || componetInChild2 == null)
			{
				DebugHelper.Assert(false, "[CGuildInfoView] qqBtnText or wxBtnText is null");
				return;
			}
			string text = string.Empty;
			gameObject.CustomSetActive(true);
			GameObject obj;
			Text text2;
			CUIEventScript component;
			Button component2;
			string text3;
			string text5;
			string text6;
			string text7;
			if (curPlatform == ApolloPlatform.QQ)
			{
				gameObject2.CustomSetActive(true);
				gameObject3.CustomSetActive(false);
				obj = gameObject2;
				text2 = componetInChild;
				component = gameObject2.GetComponent<CUIEventScript>();
				component2 = gameObject2.GetComponent<Button>();
				text3 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Bind");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Unbind");
				text5 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Join");
				text6 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Remind_Bind");
				text7 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Remind_Join");
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_Operation_Text_Resolve");
			}
			else
			{
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(true);
				obj = gameObject3;
				text2 = componetInChild2;
				component = gameObject3.GetComponent<CUIEventScript>();
				component2 = gameObject3.GetComponent<Button>();
				text3 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_Bind");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_UnBind");
				text5 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_Join");
				text6 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_Remind_Bind");
				text7 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_Remind_Join");
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_Operation_Text_Resolve");
			}
			if (status == CGuildSystem.enPlatformGroupStatus.Bound)
			{
				if (CGuildSystem.HasManageQQGroupAuthority())
				{
					if (this.NeedAutoOp)
					{
						this.NeedAutoOp = false;
						this.SendRemindMsg();
					}
					if (isSelfInGroup)
					{
						text2.text = text7;
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_RemindPlatformGroup);
						CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
						obj.CustomSetActive(true);
					}
					else
					{
						text2.text = text5;
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_JoinPlatformGroup);
						CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
						obj.CustomSetActive(true);
					}
				}
				else if (isSelfInGroup)
				{
					obj.CustomSetActive(false);
				}
				else
				{
					text2.text = text5;
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_JoinPlatformGroup);
					CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
					obj.CustomSetActive(true);
					if (curPlatform == ApolloPlatform.Wechat && this.NeedAutoOp)
					{
						this.NeedAutoOp = false;
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_JoinPlatformGroup);
					}
				}
			}
			else if (status == CGuildSystem.enPlatformGroupStatus.UnBound)
			{
				if (CGuildSystem.HasManageQQGroupAuthority())
				{
					text2.text = text3;
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_BindPlatformGroup);
					CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
					obj.CustomSetActive(true);
					if (this.NeedAutoOp)
					{
						this.NeedAutoOp = false;
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_BindPlatformGroup);
					}
				}
				else
				{
					text2.text = text6;
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_RemindPlatformGroup);
					CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
					obj.CustomSetActive(true);
					if (this.NeedAutoOp)
					{
						this.NeedAutoOp = false;
						this.SendRemindMsg();
					}
				}
			}
			else
			{
				text2.text = text;
				CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), false, true);
				obj.CustomSetActive(true);
			}
		}

		private void RefreshPlatformGroupPanelInSettingForm(CGuildSystem.enPlatformGroupStatus status, bool isSelfInGroup = false)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
			if (form == null)
			{
				return;
			}
			ApolloPlatform curPlatform = Singleton<ApolloHelper>.GetInstance().CurPlatform;
			if (curPlatform != ApolloPlatform.QQ && curPlatform != ApolloPlatform.Wechat)
			{
				return;
			}
			GameObject widget = form.GetWidget(13);
			GameObject gameObject = Utility.FindChild(widget, "btnQQGroup");
			GameObject gameObject2 = Utility.FindChild(widget, "btnWxGroup");
			if (gameObject == null || gameObject2 == null)
			{
				DebugHelper.Assert(false, "[CGuildInfoView] qqBtnGo or wxBtnGo is null");
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "Text");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject2, "Text");
			if (componetInChild == null || componetInChild2 == null)
			{
				DebugHelper.Assert(false, "[CGuildInfoView] qqBtnText or wxBtnText is null");
				return;
			}
			string text = string.Empty;
			widget.CustomSetActive(true);
			GameObject obj;
			Text text2;
			CUIEventScript component;
			Button component2;
			string text3;
			if (curPlatform == ApolloPlatform.QQ)
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				obj = gameObject;
				text2 = componetInChild;
				component = gameObject.GetComponent<CUIEventScript>();
				component2 = gameObject.GetComponent<Button>();
				text3 = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Operation_Text_Unbind");
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_Operation_Text_Resolve");
			}
			else
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(true);
				obj = gameObject2;
				text2 = componetInChild2;
				component = gameObject2.GetComponent<CUIEventScript>();
				component2 = gameObject2.GetComponent<Button>();
				text3 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Operation_Text_UnBind");
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_Operation_Text_Resolve");
			}
			if (status == CGuildSystem.enPlatformGroupStatus.Bound)
			{
				if (CGuildSystem.HasManageQQGroupAuthority())
				{
					if (isSelfInGroup)
					{
						text2.text = text3;
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_UnBindPlatformGroup);
						CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), true, true);
						obj.CustomSetActive(true);
					}
					else
					{
						obj.CustomSetActive(false);
					}
				}
				else
				{
					obj.CustomSetActive(false);
				}
			}
			else if (status == CGuildSystem.enPlatformGroupStatus.UnBound)
			{
				obj.CustomSetActive(false);
			}
			else
			{
				text2.text = text;
				CUICommonSystem.SetButtonEnableWithShader(component2.GetComponent<Button>(), false, true);
				obj.CustomSetActive(true);
			}
		}
	}
}

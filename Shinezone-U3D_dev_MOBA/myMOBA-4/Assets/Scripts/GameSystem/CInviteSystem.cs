using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CInviteSystem : Singleton<CInviteSystem>
	{
		private enum enInviteState
		{
			None,
			Invited,
			BeRejcet
		}

		private class InviteState
		{
			public ulong uid;

			public uint time;

			public CInviteSystem.enInviteState state;
		}

		public struct stInviteInfo
		{
			public ulong playerUid;

			public uint playerLogicWorldId;

			public COM_INVITE_JOIN_TYPE joinType;

			public CInviteView.enInviteListTab objSrc;

			public int gameEntity;

			public int maxTeamNum;

			public void Clear()
			{
				this.playerUid = 0uL;
				this.playerLogicWorldId = 0u;
				this.joinType = COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_NULL;
				this.objSrc = CInviteView.enInviteListTab.Friend;
				this.gameEntity = 0;
				this.maxTeamNum = 0;
			}
		}

		private const int REFRESH_GUILD_MEMBER_GAME_STATE_WAIT_MILLISECONDS = 3000;

		private const int REFRESH_GUILD_MEMBER_GAME_STATE_SECONDS = 10;

		private const int WEIGHT_ONLINE = 20000000;

		private const int WEIGHT_INTIMACY_LEVEL = 20000;

		private const int WEIGHT_VIP_LEVEL = 1000;

		private const int WEIGHT_RANK_LEVEL = 50;

		private const int WEIGHT_PVP_LEVEL = 1;

		private ListView<CInviteSystem.InviteState> m_stateList = new ListView<CInviteSystem.InviteState>();

		public static string PATH_INVITE_FORM = "UGUI/Form/System/PvP/Form_InviteFriend.prefab";

		private COM_INVITE_JOIN_TYPE m_inviteType;

		private ListView<COMDT_FRIEND_INFO> allFriendList_internal;

		private ListView<GuildMemInfo> m_allGuildMemberList;

		private bool m_isNeedRefreshGuildMemberPanel = true;

		private bool m_isFirstlySelectGuildMemberTab = true;

		private static uint s_refreshLBSTime;

		private uint lastRefreshLBSTime;

		public int gameTimer = -1;

		private CInviteSystem.stInviteInfo m_inviteInfo;

		public static bool s_isInviteFriendImmidiately;

		private BeInviteMenu m_beInviteForm = new BeInviteMenu();

		private ListView<COMDT_FRIEND_INFO> m_allFriendList
		{
			get
			{
				if (this.allFriendList_internal == null)
				{
					this.SortAllFriendList();
				}
				return this.allFriendList_internal;
			}
		}

		public CInviteSystem.stInviteInfo InviteInfo
		{
			get
			{
				return this.m_inviteInfo;
			}
			set
			{
				this.m_inviteInfo = value;
			}
		}

		public static uint RefreshLBSTime
		{
			get
			{
				if (CInviteSystem.s_refreshLBSTime == 0u)
				{
					ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(216u);
					if (dataByKey == null)
					{
						CInviteSystem.s_refreshLBSTime = 60u;
					}
					else
					{
						CInviteSystem.s_refreshLBSTime = dataByKey.dwConfValue;
					}
				}
				return CInviteSystem.s_refreshLBSTime;
			}
		}

		public COM_INVITE_JOIN_TYPE InviteType
		{
			get
			{
				return this.m_inviteType;
			}
		}

		public void CheckInviteListGameTimer()
		{
			if (this.gameTimer == -1)
			{
				this.gameTimer = Singleton<CTimerManager>.instance.AddTimer(65000, 0, new CTimer.OnTimeUpHandler(this.OnInviteListGameTimer));
			}
		}

		public void ClearInviteListGameTimer()
		{
			if (this.gameTimer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.gameTimer);
				this.gameTimer = -1;
			}
		}

		public void OnInviteListGameTimer(int index)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				CUIListScript component = form.transform.Find("Title/ListTab").GetComponent<CUIListScript>();
				if (component != null)
				{
					CInviteView.enInviteListTab inviteListTab = CInviteView.GetInviteListTab(component.GetSelectedIndex());
					if (inviteListTab == CInviteView.enInviteListTab.Friend)
					{
						CInviteView.SetInviteFriendData(form, this.m_inviteType);
					}
					else if (inviteListTab == CInviteView.enInviteListTab.GuildMember)
					{
						CInviteView.SetInviteGuildMemberData(form);
					}
				}
			}
			else
			{
				this.ClearInviteListGameTimer();
			}
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_Lobby_MsgBox, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_MsgBox));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_Lobby_CloseTip, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_CloseTip));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_Lobby_MsgBox_Ok, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_MsgBox_Ok));
			Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, new Action<byte, string>(this.OnInviteRoomErrCodeNtf));
			Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this.OnInviteTeamErrCodeNtf));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Friend_Game_State_Change, new Action(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_Game_State_Refresh", new Action(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new Action(this.OnLBSListChg));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.RECEIVE_RESERVE_DATA_CHANGE, new Action(this.OnRECEIVE_RESERVE_DATA_CHANGE));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_Lobby_MsgBox, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_MsgBox));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_Lobby_CloseTip, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_CloseTip));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_Lobby_MsgBox_Ok, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reverse_Lobby_MsgBox_Ok));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, new Action<byte, string>(this.OnInviteRoomErrCodeNtf));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this.OnInviteTeamErrCodeNtf));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Friend_Game_State_Change, new Action(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Friend_Game_State_Refresh", new Action(this.OnFriendOnlineChg));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Friend_LBS_User_Refresh", new Action(this.OnLBSListChg));
		}

		public void Load_Config_Text()
		{
			if (this.m_beInviteForm != null)
			{
				this.m_beInviteForm.LoadConfig();
			}
		}

		private bool InInviteCdList(ulong uid, uint time)
		{
			for (int i = 0; i < this.m_stateList.Count; i++)
			{
				if (uid == this.m_stateList[i].uid)
				{
					return time - this.m_stateList[i].time < GameDataMgr.globalInfoDatabin.GetDataByKey(156u).dwConfValue;
				}
			}
			return false;
		}

		private void AddInviteStateList(ulong uid, uint time, CInviteSystem.enInviteState state)
		{
			for (int i = 0; i < this.m_stateList.Count; i++)
			{
				if (uid == this.m_stateList[i].uid)
				{
					this.m_stateList[i].uid = uid;
					this.m_stateList[i].time = time;
					this.m_stateList[i].state = state;
					return;
				}
			}
			CInviteSystem.InviteState inviteState = new CInviteSystem.InviteState();
			inviteState.uid = uid;
			inviteState.time = time;
			inviteState.state = state;
			this.m_stateList.Add(inviteState);
		}

		private void ChangeInviteStateList(ulong uid, CInviteSystem.enInviteState state)
		{
			for (int i = 0; i < this.m_stateList.Count; i++)
			{
				if (uid == this.m_stateList[i].uid)
				{
					this.m_stateList[i].state = state;
					return;
				}
			}
		}

		private CInviteSystem.enInviteState GetInviteState(ulong uid)
		{
			for (int i = 0; i < this.m_stateList.Count; i++)
			{
				if (uid == this.m_stateList[i].uid)
				{
					return this.m_stateList[i].state;
				}
			}
			return CInviteSystem.enInviteState.None;
		}

		public string GetInviteStateStr(ulong uid, bool isGuildMatchInvite = false)
		{
			CInviteSystem.enInviteState inviteState = this.GetInviteState(uid);
			if (inviteState == CInviteSystem.enInviteState.None || (inviteState == CInviteSystem.enInviteState.BeRejcet && isGuildMatchInvite))
			{
				return string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
			}
			if (inviteState == CInviteSystem.enInviteState.Invited)
			{
				return string.Format("<color=#ffffff>{0}</color>", Singleton<CTextManager>.instance.GetText("Guild_Has_Invited"));
			}
			if (inviteState == CInviteSystem.enInviteState.BeRejcet)
			{
				return string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2"));
			}
			return string.Empty;
		}

		private uint GetNextInviteSec(ulong uid, uint time)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(156u).dwConfValue;
			for (int i = 0; i < this.m_stateList.Count; i++)
			{
				if (uid == this.m_stateList[i].uid)
				{
					return this.m_stateList[i].time + dwConfValue - time;
				}
			}
			return 0u;
		}

		public void OpenInviteForm(COM_INVITE_JOIN_TYPE inviteType, bool bNotShowInViteBtn = false)
		{
			this.m_stateList.Clear();
			this.m_isFirstlySelectGuildMemberTab = true;
			this.SortAllFriendList();
			this.m_allGuildMemberList = this.CreateGuildMemberInviteList();
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CInviteSystem.PATH_INVITE_FORM, false, true);
			if (cUIFormScript != null)
			{
				this.m_inviteType = inviteType;
				CInviteView.InitListTab(cUIFormScript);
				CInviteView.SetInviteFriendData(cUIFormScript, inviteType);
				this.DispatchInviteUIEvent();
				if (bNotShowInViteBtn)
				{
					Transform transform = cUIFormScript.transform.Find("Panel_Friend/Bottom/ShareInviteButton");
					if (transform)
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
			}
			if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
			{
				Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0uL, 0u);
				Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Team);
				Singleton<CChatController>.instance.ShowPanel(true, false);
				Singleton<CChatController>.instance.view.UpView(true);
				Singleton<CChatController>.instance.model.sysData.ClearEntryText();
			}
		}

		private void DispatchInviteUIEvent()
		{
			if (CInviteSystem.s_isInviteFriendImmidiately)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventParams = default(stUIEventParams);
				uIEvent.m_eventParams.tag = (int)this.m_inviteInfo.joinType;
				uIEvent.m_eventParams.commonUInt64Param1 = this.m_inviteInfo.playerUid;
				uIEvent.m_eventParams.tag2 = (int)this.m_inviteInfo.playerLogicWorldId;
				if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.Friend)
				{
					uIEvent.m_eventID = enUIEventID.Invite_SendInviteFriend;
				}
				else if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.GuildMember)
				{
					uIEvent.m_eventID = enUIEventID.Invite_SendInviteGuildMember;
				}
				else if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.LBS)
				{
					uIEvent.m_eventID = enUIEventID.Invite_SendInviteLBS;
					uIEvent.m_eventParams.tag3 = this.m_inviteInfo.gameEntity;
				}
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
				this.m_inviteInfo.Clear();
			}
		}

		public void CloseInviteForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CInviteSystem.PATH_INVITE_FORM);
			Singleton<CInviteSystem>.instance.ClearInviteListGameTimer();
			if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
			{
				Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0uL, 0u);
				Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
				Singleton<CChatController>.instance.ShowPanel(true, false);
				Singleton<CChatController>.instance.view.UpView(false);
				Singleton<CChatController>.instance.model.sysData.ClearEntryText();
			}
			Singleton<CFriendContoller>.instance.model.friendReserve.dataList[1].Clear();
		}

		private void RefreshGuildMemberInvitePanel()
		{
			if (CInviteSystem.IsGuildMatchInvite())
			{
				Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchGuildMemberInvitePanel();
			}
			else
			{
				this.RefreshNormalGuildMemberInvitePanel();
			}
		}

		private void RefreshNormalGuildMemberInvitePanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				this.SortAllGuildMemberList();
				CInviteView.SetInviteGuildMemberData(form);
			}
		}

		private void OnInvite_SendInviteFriend(CUIEvent uiEvent)
		{
			COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE)uiEvent.m_eventParams.tag;
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			uint tag2 = (uint)uiEvent.m_eventParams.tag2;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			if (this.InInviteCdList(commonUInt64Param, currentUTCTime))
			{
				Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, new object[]
				{
					this.GetNextInviteSec(commonUInt64Param, currentUTCTime)
				});
				return;
			}
			COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, commonUInt64Param, tag2);
			COMDT_FRIEND_INFO info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, commonUInt64Param, tag2);
			COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = (info != null) ? info : info2;
			if (cOMDT_FRIEND_INFO == null)
			{
				return;
			}
			byte bFriendType = 1;
			if (info2 != null && (info == null || (info.bIsOnline == 0 && info2.bIsOnline == 1)))
			{
				bFriendType = 2;
			}
			if (tag == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2017u);
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = cOMDT_FRIEND_INFO.stUin.ullUid;
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = bFriendType;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else if (tag == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(2024u);
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = cOMDT_FRIEND_INFO.stUin.ullUid;
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = bFriendType;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, false);
			}
			this.SetGameStateTextWidget(uiEvent.m_srcWidget);
			this.AddInviteStateList(commonUInt64Param, currentUTCTime, CInviteSystem.enInviteState.Invited);
		}

		private void OnInvite_SendInviteLBS(CUIEvent uiEvent)
		{
			COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE)uiEvent.m_eventParams.tag;
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			uint tag2 = (uint)uiEvent.m_eventParams.tag2;
			uint tag3 = (uint)uiEvent.m_eventParams.tag3;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			if (this.InInviteCdList(commonUInt64Param, currentUTCTime))
			{
				Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, new object[]
				{
					this.GetNextInviteSec(commonUInt64Param, currentUTCTime)
				});
				return;
			}
			if (tag == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2017u);
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = commonUInt64Param;
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = tag2;
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = 3;
				cSPkg.stPkgData.stInviteFriendJoinRoomReq.dwGameSvrEntity = tag3;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else if (tag == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(2024u);
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = commonUInt64Param;
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = tag2;
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = 3;
				cSPkg2.stPkgData.stInviteFriendJoinTeamReq.dwGameSvrEntity = tag3;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg2, false);
			}
			this.SetGameStateTextWidget(uiEvent.m_srcWidget);
			this.AddInviteStateList(commonUInt64Param, currentUTCTime, CInviteSystem.enInviteState.Invited);
		}

		private void OnInvite_SendInviteGuildMember(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2034u);
			cSPkg.stPkgData.stInviteGuildMemberJoinReq.iInviteType = tag;
			cSPkg.stPkgData.stInviteGuildMemberJoinReq.ullInviteeUid = commonUInt64Param;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			if (uiEvent.m_srcWidget != null)
			{
				uiEvent.m_srcWidget.CustomSetActive(false);
				this.SetGameStateTextWidget(uiEvent.m_srcWidget);
			}
		}

		private void SetGameStateTextWidget(GameObject srcWidget)
		{
			if (srcWidget != null)
			{
				Transform parent = srcWidget.transform.parent;
				if (parent != null)
				{
					Transform transform = parent.Find("Online");
					if (transform)
					{
						Text component = transform.GetComponent<Text>();
						component.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
					}
				}
			}
		}

		private void OnInvite_Accept(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2021u);
			cSPkg.stPkgData.stInviteJoinGameRsp.bIndex = (byte)uiEvent.m_eventParams.tag;
			cSPkg.stPkgData.stInviteJoinGameRsp.bResult = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<WatchController>.GetInstance().Stop();
			Singleton<CUIManager>.GetInstance().CloseMessageBox();
			Singleton<CMailSys>.instance.AddFriendInviteMail(uiEvent, CMailSys.enProcessInviteType.Accept);
		}

		private void OnInvite_Reject(CUIEvent uiEvent)
		{
			if (this.m_beInviteForm != null)
			{
				this.m_beInviteForm.ShowRefuse();
			}
		}

		private void OnInvite_FriendListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_allFriendList.Count)
			{
				CInviteView.UpdateFriendListElement(srcWidget, this.m_allFriendList[srcWidgetIndexInBelongedList]);
			}
		}

		private void OnInvite_GuildMemberListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_allGuildMemberList.Count)
			{
				CInviteView.UpdateGuildMemberListElement(srcWidget, this.m_allGuildMemberList[srcWidgetIndexInBelongedList], false);
			}
		}

		private void OnInvite_LBSListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			ListView<CSDT_LBS_USER_INFO> lBSList = Singleton<CFriendContoller>.instance.model.GetLBSList(CFriendModel.LBSGenderType.Both);
			if (lBSList == null)
			{
				return;
			}
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < lBSList.Count)
			{
				CInviteView.UpdateLBSListElement(srcWidget, lBSList[srcWidgetIndexInBelongedList]);
			}
		}

		private void OnInvite_TabChange(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			GameObject widget = srcFormScript.GetWidget(0);
			GameObject widget2 = srcFormScript.GetWidget(1);
			GameObject widget3 = srcFormScript.GetWidget(10);
			CInviteView.enInviteListTab inviteListTab = CInviteView.GetInviteListTab(component.GetSelectedIndex());
			if (inviteListTab == CInviteView.enInviteListTab.Friend)
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
			}
			else if (inviteListTab == CInviteView.enInviteListTab.GuildMember)
			{
				widget2.CustomSetActive(true);
				widget.CustomSetActive(false);
				widget3.CustomSetActive(false);
				if (this.m_isFirstlySelectGuildMemberTab)
				{
					this.SendGetGuildMemberGameStateReq();
					CUITimerScript component2 = srcFormScript.GetWidget(8).GetComponent<CUITimerScript>();
					this.SetAndStartRefreshGuildMemberGameStateTimer(component2);
					this.m_isFirstlySelectGuildMemberTab = false;
				}
			}
			else if (inviteListTab == CInviteView.enInviteListTab.LBS)
			{
				widget2.CustomSetActive(false);
				widget.CustomSetActive(false);
				widget3.CustomSetActive(true);
				CInviteView.SetLBSData(srcFormScript, this.m_inviteType);
				uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
				if (currentUTCTime - this.lastRefreshLBSTime > CInviteSystem.RefreshLBSTime)
				{
					this.lastRefreshLBSTime = currentUTCTime;
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Friend_LBS_Refresh;
					cUIEvent.m_eventParams.tag = 1;
					Singleton<CUIEventManager>.instance.DispatchUIEvent(cUIEvent);
				}
			}
		}

		public void SetAndStartRefreshGuildMemberGameStateTimer(CUITimerScript timer)
		{
			timer.SetTotalTime(1000f);
			timer.SetOnChangedIntervalTime(10f);
			timer.StartTimer();
		}

		private void OnInvite_RefreshGameStateTimeout(CUIEvent uiEvent)
		{
			this.SendGetGuildMemberGameStateReq();
		}

		private void OnInvite_Reverse_Lobby_MsgBox_Ok(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			FriendSysNetCore.SendReserveReq(commonUInt64Param, tagUInt);
			Singleton<CFriendContoller>.instance.model.friendReserve.SetData(commonUInt64Param, tagUInt, 0, FriendReserve.ReserveDataType.Send);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				CInviteView.SetInviteFriendData(form, this.m_inviteType);
			}
		}

		private void OnInvite_Reverse_Lobby_MsgBox(CUIEvent uievent)
		{
			stUIEventParams par = default(stUIEventParams);
			par.commonUInt64Param1 = uievent.m_eventParams.commonUInt64Param1;
			par.tagUInt = uievent.m_eventParams.tagUInt;
			par.tagStr = uievent.m_eventParams.tagStr;
			string strContent = string.Format("确定预约{0}一起玩游戏吗", uievent.m_eventParams.tagStr);
			Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(strContent, enUIEventID.Invite_Reverse_Lobby_MsgBox_Ok, enUIEventID.Invite_Reverse_Lobby_MsgBox_Cancle, par, false);
		}

		private void OnInvite_Reverse_Lobby_CloseTip(CUIEvent uievent)
		{
			uievent.m_srcWidget.transform.parent.gameObject.CustomSetActive(false);
		}

		public void SortAllFriendList()
		{
			this.allFriendList_internal = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend(false);
			this.allFriendList_internal.Sort(new Comparison<COMDT_FRIEND_INFO>(CInviteSystem.InviteFriendsSort));
		}

		public ListView<GuildMemInfo> CreateGuildMemberInviteList()
		{
			ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
			ListView<GuildMemInfo> listView = new ListView<GuildMemInfo>();
			listView.AddRange(guildMemberInfos);
			for (int i = listView.Count - 1; i >= 0; i--)
			{
				if (CGuildHelper.IsSelf(listView[i].stBriefInfo.uulUid) || Singleton<CFriendContoller>.instance.model.IsBlack(listView[i].stBriefInfo.uulUid, (uint)listView[i].stBriefInfo.dwLogicWorldId) || (CInviteSystem.IsGuildMatchInvite() && CGuildHelper.IsGuildMatchReachMatchCntLimit(listView[i])))
				{
					listView.RemoveAt(i);
				}
			}
			return listView;
		}

		private void SortAllGuildMemberList()
		{
			if (this.m_allGuildMemberList != null)
			{
				this.m_allGuildMemberList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
			}
		}

		[MessageHandler(2018)]
		public static void OnInviteFriendRoom(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			byte bErrcode = msg.stPkgData.stInviteFriendJoinRoomRsp.bErrcode;
			string arg = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName);
			if (bErrcode == 20)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
			}
			else if (bErrcode == 13)
			{
				if (msg.stPkgData.stInviteFriendJoinRoomRsp.dwFriendStartTime <= 0u)
				{
					Singleton<CFriendContoller>.instance.model.SetFriendGameState(msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.ullUid, msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.dwLogicWorldId, COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM, 0u, string.Empty, false, false, 0u);
				}
				else
				{
					Singleton<CFriendContoller>.instance.model.SetFriendGameState(msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.ullUid, msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.dwLogicWorldId, COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME, msg.stPkgData.stInviteFriendJoinRoomRsp.dwFriendStartTime, string.Empty, false, false, 0u);
				}
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
			}
			else if (bErrcode == 31)
			{
				CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.ullUid, msg.stPkgData.stInviteFriendJoinRoomRsp.stFriendUin.dwLogicWorldId);
				if (friendInGaming != null)
				{
					friendInGaming.antiDisturbBits |= 1u;
				}
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
				string refuseReason = CUIUtility.RemoveEmoji(Utility.UTF8Convert(msg.stPkgData.stInviteFriendJoinRoomRsp.szDenyReason)).Trim();
				string inviteRoomFailReason = CInviteSystem.GetInviteRoomFailReason(StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName), (int)bErrcode, refuseReason);
				Singleton<CUIManager>.GetInstance().OpenTips(inviteRoomFailReason, false, 1.5f, null, new object[0]);
			}
			else
			{
				string refuseReason2 = CUIUtility.RemoveEmoji(Utility.UTF8Convert(msg.stPkgData.stInviteFriendJoinRoomRsp.szDenyReason)).Trim();
				string inviteRoomFailReason2 = CInviteSystem.GetInviteRoomFailReason(StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName), (int)bErrcode, refuseReason2);
				if (bErrcode == 14)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(inviteRoomFailReason2, false, 3f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(inviteRoomFailReason2, false, 1.5f, null, new object[0]);
				}
			}
			Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, bErrcode, arg);
		}

		[MessageHandler(2025)]
		public static void OnInviteFriendTeam(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			byte bErrcode = msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode;
			string text = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinTeamRsp.szFriendName);
			uint timePunished = 0u;
			uint punishType = 1u;
			if (msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode == 17)
			{
				timePunished = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam;
				punishType = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam2;
			}
			else if (msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode == 5)
			{
				if (msg.stPkgData.stInviteFriendJoinTeamRsp.dwFriendStartTime <= 0u)
				{
					Singleton<CFriendContoller>.instance.model.SetFriendGameState(msg.stPkgData.stInviteFriendJoinTeamRsp.stFriendUin.ullUid, msg.stPkgData.stInviteFriendJoinTeamRsp.stFriendUin.dwLogicWorldId, COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM, 0u, string.Empty, false, false, 0u);
				}
				else
				{
					Singleton<CFriendContoller>.instance.model.SetFriendGameState(msg.stPkgData.stInviteFriendJoinTeamRsp.stFriendUin.ullUid, msg.stPkgData.stInviteFriendJoinTeamRsp.stFriendUin.dwLogicWorldId, COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME, msg.stPkgData.stInviteFriendJoinTeamRsp.dwFriendStartTime, string.Empty, false, false, 0u);
				}
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
			}
			string refuseReason = CUIUtility.RemoveEmoji(Utility.UTF8Convert(msg.stPkgData.stInviteFriendJoinTeamRsp.szDenyReason)).Trim();
			string inviteTeamFailReason = CInviteSystem.GetInviteTeamFailReason(text, (int)bErrcode, timePunished, punishType, refuseReason);
			if (bErrcode == 6)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(inviteTeamFailReason, false, 3f, null, new object[0]);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(inviteTeamFailReason, false, 1.5f, null, new object[0]);
			}
			Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, bErrcode, text);
		}

		[MessageHandler(2020)]
		public static void OnGetInvited(CSPkg msg)
		{
			if (!Singleton<CInviteSystem>.GetInstance().IsCanBeInvited(msg.stPkgData.stInviteJoinGameReq.stInviterInfo.ullUid, msg.stPkgData.stInviteJoinGameReq.stInviterInfo.dwLogicWorldID))
			{
				return;
			}
			Singleton<CInviteSystem>.instance.ShowNewBeingInvitedUI(msg.stPkgData.stInviteJoinGameReq);
			MonoSingleton<TGASys>.GetInstance().battleInvitation(msg.stPkgData.stInviteJoinGameReq);
		}

		public void ShowNewBeingInvitedUI(SCPKG_INVITE_JOIN_GAME_REQ info)
		{
			this.m_beInviteForm.Open(info);
		}

		private void OnInvateFriendTimeOut(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcFormScript != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(uiEvent.m_srcFormScript);
				Singleton<CMailSys>.instance.AddFriendInviteMail(uiEvent, CMailSys.enProcessInviteType.NoProcess);
			}
		}

		[MessageHandler(2036)]
		public static void ReceiveGetGuildMemberGameStateRsp(CSPkg msg)
		{
			SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP stGetGuildMemberGameStateRsp = msg.stPkgData.stGetGuildMemberGameStateRsp;
			Singleton<CInviteSystem>.GetInstance().RefreshGuildMemberGameState(stGetGuildMemberGameStateRsp);
			Singleton<EventRouter>.instance.BroadCastEvent("MAIL_GUILD_MEM_UPDATE");
		}

		private void RefreshGuildMemberGameState(SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP rsp)
		{
			this.SetGuildMemberInviteListGameState(rsp, this.GetGuildMemberInviteList());
			Singleton<CInviteSystem>.GetInstance().RefreshGuildMemberInvitePanel();
			Singleton<CInviteSystem>.GetInstance().m_isNeedRefreshGuildMemberPanel = false;
		}

		public void SetGuildMemberInviteListGameState(SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP rsp, ListView<GuildMemInfo> guildMemberList)
		{
			if (guildMemberList == null)
			{
				return;
			}
			for (int i = 0; i < rsp.iMemberCnt; i++)
			{
				for (int j = 0; j < guildMemberList.Count; j++)
				{
					if (CGuildHelper.IsMemberOnline(guildMemberList[j]) && rsp.astMemberInfo[i].ullUid == guildMemberList[j].stBriefInfo.uulUid)
					{
						guildMemberList[j].GameState = (COM_ACNT_GAME_STATE)rsp.astMemberInfo[i].bGameState;
						guildMemberList[j].dwGameStartTime = rsp.astMemberInfo[i].dwGameStartTime;
						guildMemberList[j].antiDisturbBits = rsp.astMemberInfo[i].dwOtherStateBits;
					}
				}
			}
		}

		public CUIListElementScript GetListItem(string username)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form)
			{
				GameObject gameObject = form.transform.Find("Panel_Friend/List").gameObject;
				CUIListScript component = gameObject.GetComponent<CUIListScript>();
				for (int i = 0; i < component.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					if (elemenet != null)
					{
						Text component2 = elemenet.gameObject.transform.Find("PlayerName").GetComponent<Text>();
						if (component2.text == username)
						{
							return elemenet;
						}
					}
				}
			}
			return null;
		}

		private static string GetInviteString(SCPKG_INVITE_JOIN_GAME_REQ msg)
		{
			string text = Utility.UTF8Convert(msg.stInviterInfo.szName);
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			uint dwRelationMask = msg.stInviterInfo.dwRelationMask;
			if ((dwRelationMask & 1u) > 0u)
			{
				text2 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
			}
			else if ((dwRelationMask & 2u) > 0u)
			{
				text2 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
			}
			else if ((dwRelationMask & 4u) > 0u)
			{
				text2 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
			}
			else
			{
				text2 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
			}
			if (msg.bInviteType == 1)
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stRoomDetail.bMapType, msg.stInviteDetail.stRoomDetail.dwMapId);
				if (pvpMapCommonInfo != null)
				{
					text4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
					{
						((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
						((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
						Utility.UTF8Convert(pvpMapCommonInfo.szName)
					});
				}
				text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
			}
			else if (msg.bInviteType == 2)
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stTeamDetail.bMapType, msg.stInviteDetail.stTeamDetail.dwMapId);
				if (pvpMapCommonInfo2 != null)
				{
					text4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
					{
						((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
						((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
						Utility.UTF8Convert(pvpMapCommonInfo2.szName)
					});
				}
				if (msg.stInviteDetail.stTeamDetail.bMapType == 3)
				{
					text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
				}
				else if (msg.stInviteDetail.stTeamDetail.bPkAI == 1)
				{
					text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_2");
				}
				else
				{
					text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_3");
				}
			}
			return Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", new string[]
			{
				text,
				text2,
				text3,
				text4
			});
		}

		private static string GetInviteRoomFailReason(string fName, int errCode, string refuseReason = "")
		{
			string result = string.Empty;
			switch (errCode)
			{
			case 11:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);
				break;
			case 12:
			{
				COMDT_FRIEND_INFO friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
				if (friendByName == null)
				{
					friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
				}
				if (friendByName != null)
				{
					friendByName.bIsOnline = 0;
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_Game_State_Refresh");
				}
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
				break;
			}
			case 13:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);
				break;
			case 14:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName, refuseReason);
				break;
			case 21:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Version_Different");
				break;
			case 22:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");
				break;
			case 23:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");
				break;
			case 24:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");
				break;
			case 29:
				result = Singleton<CTextManager>.GetInstance().GetText("CS_ROOMERR_PLAT_CHANNEL_CLOSE");
				break;
			case 30:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Room_Ban_Pick_Hero_Limit_Accept");
				break;
			case 31:
				result = Singleton<CTextManager>.GetInstance().GetText("CS_ROOMERR_ROOM_ANTI_DISTURB");
				break;
			}
			return result;
		}

		private static string GetInviteTeamFailReason(string fName, int errCode, uint timePunished = 0u, uint punishType = 1u, string refuseReason = "")
		{
			string result = string.Empty;
			switch (errCode)
			{
			case 3:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);
				break;
			case 4:
			{
				COMDT_FRIEND_INFO friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
				if (friendByName == null)
				{
					friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
				}
				if (friendByName != null)
				{
					friendByName.bIsOnline = 0;
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_Game_State_Refresh");
				}
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
				break;
			}
			case 5:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);
				break;
			case 6:
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName, refuseReason);
				break;
			case 9:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Team_Member_Full");
				break;
			case 12:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_4");
				break;
			case 14:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");
				break;
			case 15:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");
				break;
			case 16:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");
				break;
			case 17:
			{
				string arg = string.Format("{0}分{1}秒", timePunished / 60u, timePunished % 60u);
				string key = string.Empty;
				switch (punishType)
				{
				case 1u:
					key = "PVP_Invite_Punished";
					break;
				case 2u:
					key = "PVP_Invite_HangUpPunished";
					break;
				case 3u:
					key = "PVP_Invite_CreditPunished";
					break;
				default:
					key = "PVP_Invite_Punished";
					break;
				}
				result = string.Format(Singleton<CTextManager>.GetInstance().GetText(key), fName, arg);
				break;
			}
			case 18:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_1");
				break;
			case 19:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_2");
				break;
			case 20:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_3");
				break;
			case 21:
				result = Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_5");
				break;
			case 23:
				result = Singleton<CTextManager>.GetInstance().GetText("Invite_Err_Credit_Limit");
				break;
			case 26:
				result = Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_PLAT_CHANNEL_CLOSE");
				break;
			case 27:
				result = Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_OBING");
				break;
			}
			return result;
		}

		private void OnInviteRoomErrCodeNtf(byte errorCode, string userName)
		{
			this.OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM, errorCode, userName);
		}

		private void OnInviteTeamErrCodeNtf(byte errorCode, string userName)
		{
			this.OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM, errorCode, userName);
		}

		private void OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE inviteType, byte errorCode, string userName)
		{
			if ((inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM && errorCode == 14) || (inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM && errorCode == 6))
			{
				COMDT_FRIEND_INFO friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.GameFriend);
				if (friendByName == null)
				{
					friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.SNS);
				}
				if (friendByName == null)
				{
					return;
				}
				CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(friendByName.stUin.ullUid, friendByName.stUin.dwLogicWorldId);
				if (friendInGaming != null)
				{
					userName = CInviteView.ConnectPlayerNameAndNickName(Utility.BytesConvert(userName), friendInGaming.NickName);
				}
				CUIListElementScript listItem = this.GetListItem(userName);
				if (listItem != null)
				{
					Text component = listItem.transform.FindChild("Online").GetComponent<Text>();
					component.text = string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2"));
				}
				this.ChangeInviteStateList(friendByName.stUin.ullUid, CInviteSystem.enInviteState.BeRejcet);
			}
		}

		private void OnFriendChange(CSPkg msg)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				this.SortAllFriendList();
				CInviteView.SetInviteFriendData(form, this.m_inviteType);
			}
		}

		private void OnRECEIVE_RESERVE_DATA_CHANGE()
		{
			this.OnFriendChange(null);
		}

		private void OnFriendOnlineChg()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				this.SortAllFriendList();
				CInviteView.SetInviteFriendData(form, this.m_inviteType);
			}
		}

		private void OnFriendOnlineChg(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldId, bool bOffline)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				this.SortAllFriendList();
				CInviteView.SetInviteFriendData(form, this.m_inviteType);
			}
		}

		private void OnLBSListChg()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
			if (form != null)
			{
				CInviteView.SetLBSData(form, this.m_inviteType);
			}
		}

		public string GetInviteFriendName(ulong friendUid, uint friendLogicWorldId)
		{
			if (this.m_allFriendList != null)
			{
				for (int i = 0; i < this.m_allFriendList.Count; i++)
				{
					if (friendUid == this.m_allFriendList[i].stUin.ullUid && friendLogicWorldId == this.m_allFriendList[i].stUin.dwLogicWorldId)
					{
						return StringHelper.UTF8BytesToString(ref this.m_allFriendList[i].szUserName);
					}
				}
			}
			return string.Empty;
		}

		public string GetInviteGuildMemberName(ulong guildMemberUid)
		{
			if (this.m_allGuildMemberList != null)
			{
				for (int i = 0; i < this.m_allGuildMemberList.Count; i++)
				{
					if (guildMemberUid == this.m_allGuildMemberList[i].stBriefInfo.uulUid)
					{
						return this.m_allGuildMemberList[i].stBriefInfo.sName;
					}
				}
			}
			return string.Empty;
		}

		public ListView<COMDT_FRIEND_INFO> GetAllFriendList()
		{
			return this.m_allFriendList;
		}

		public ListView<GuildMemInfo> GetAllGuildMemberList()
		{
			return this.m_allGuildMemberList;
		}

		public void SendGetGuildMemberGameStateReq()
		{
			ListView<GuildMemInfo> guildMemberInviteList = this.GetGuildMemberInviteList();
			if (guildMemberInviteList == null)
			{
				DebugHelper.Assert(false, "guildMemberInviteList is null!!!");
				return;
			}
			this.SendSendGetGuildMemberGameStateReqRaw(guildMemberInviteList);
			this.m_isNeedRefreshGuildMemberPanel = true;
			Singleton<CTimerManager>.GetInstance().AddTimer(3000, 1, new CTimer.OnTimeUpHandler(this.OnRefreshGuildMemberGameStateTimeout));
		}

		public void SendSendGetGuildMemberGameStateReqRaw(ListView<GuildMemInfo> guildMemberList)
		{
			if (guildMemberList == null)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2035u);
			CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq = cSPkg.stPkgData.stGetGuildMemberGameStateReq;
			int num = 0;
			for (int i = 0; i < guildMemberList.Count; i++)
			{
				if (CGuildHelper.IsMemberOnline(guildMemberList[i]))
				{
					stGetGuildMemberGameStateReq.MemberUid[num] = guildMemberList[i].stBriefInfo.uulUid;
					num++;
				}
			}
			stGetGuildMemberGameStateReq.iMemberCnt = num;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private ListView<GuildMemInfo> GetGuildMemberInviteList()
		{
			return (!CInviteSystem.IsGuildMatchInvite()) ? this.m_allGuildMemberList : Singleton<CGuildMatchSystem>.GetInstance().GetGuildMemberInviteList();
		}

		private void OnRefreshGuildMemberGameStateTimeout(int timerSequence)
		{
			if (this.m_isNeedRefreshGuildMemberPanel)
			{
				this.RefreshGuildMemberInvitePanel();
				this.m_isNeedRefreshGuildMemberPanel = false;
			}
		}

		private static int InviteFriendsSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
		{
			uint num = 0u;
			uint num2 = 0u;
			if (l.bIsOnline == 1)
			{
				COM_ACNT_GAME_STATE cOM_ACNT_GAME_STATE = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
				CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(l.stUin.ullUid, l.stUin.dwLogicWorldId);
				if (friendInGaming != null)
				{
					cOM_ACNT_GAME_STATE = friendInGaming.State;
				}
				if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					if (friendInGaming != null && (friendInGaming.antiDisturbBits & 1u) == 0u)
					{
						num = 4u;
					}
					else
					{
						num = 3u;
					}
				}
				else if (cOM_ACNT_GAME_STATE == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					num = 2u;
				}
				else
				{
					num = 1u;
				}
			}
			if (r.bIsOnline == 1)
			{
				COM_ACNT_GAME_STATE cOM_ACNT_GAME_STATE2 = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
				CFriendModel.FriendInGame friendInGaming2 = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(r.stUin.ullUid, r.stUin.dwLogicWorldId);
				if (friendInGaming2 != null)
				{
					cOM_ACNT_GAME_STATE2 = friendInGaming2.State;
				}
				if (cOM_ACNT_GAME_STATE2 == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					if (friendInGaming2 != null && (friendInGaming2.antiDisturbBits & 1u) == 0u)
					{
						num2 = 4u;
					}
					else
					{
						num2 = 3u;
					}
				}
				else if (cOM_ACNT_GAME_STATE2 == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					num2 = 2u;
				}
				else
				{
					num2 = 1u;
				}
			}
			CFriendModel model = Singleton<CFriendContoller>.instance.model;
			ushort num3;
			CFriendModel.EIntimacyType eIntimacyType;
			bool flag;
			model.GetFriendIntimacy(r.stUin.ullUid, r.stUin.dwLogicWorldId, out num3, out eIntimacyType, out flag);
			ushort num4;
			CFriendModel.EIntimacyType eIntimacyType2;
			bool flag2;
			model.GetFriendIntimacy(l.stUin.ullUid, l.stUin.dwLogicWorldId, out num4, out eIntimacyType2, out flag2);
			return (int)((ulong)(20000000u * (num2 - num)) + (ulong)((long)(20000 * (num3 - num4))) + (ulong)(1000u * (r.stGameVip.dwCurLevel - l.stGameVip.dwCurLevel)) + (ulong)(50u * (r.dwRankClass - l.dwRankClass)) + (ulong)(1u * (r.dwPvpLvl - l.dwPvpLvl)));
		}

		private static int GuildMemberComparison(GuildMemInfo l, GuildMemInfo r)
		{
			uint num = 0u;
			uint num2 = 0u;
			if (CGuildHelper.IsMemberOnline(l))
			{
				if (l.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					num = 3u;
				}
				else if (l.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					num = 2u;
				}
				else
				{
					num = 1u;
				}
			}
			if (CGuildHelper.IsMemberOnline(r))
			{
				if (r.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
				{
					num2 = 3u;
				}
				else if (r.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
				{
					num2 = 2u;
				}
				else
				{
					num2 = 1u;
				}
			}
			return (int)(20000000u * (num2 - num) + 1000u * (r.stBriefInfo.stVip.level - l.stBriefInfo.stVip.level) + 50u * (r.stBriefInfo.dwClassOfRank - l.stBriefInfo.dwClassOfRank) + 1u * (r.stBriefInfo.dwLevel - l.stBriefInfo.dwLevel));
		}

		public void Clear()
		{
			this.lastRefreshLBSTime = 0u;
		}

		public static bool IsGuildMatchInvite()
		{
			return Singleton<CUIManager>.GetInstance().GetForm(CGuildMatchSystem.GuildMatchFormPath) != null;
		}

		public bool IsCanBeInvited(ulong inviterUid, uint inviterLogicWorldId)
		{
			return Utility.IsCanShowPrompt() && !Singleton<CFriendContoller>.instance.model.IsBlack(inviterUid, inviterLogicWorldId);
		}
	}
}

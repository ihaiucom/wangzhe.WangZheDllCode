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
	public class CChatController : Singleton<CChatController>
	{
		public class Chat_Model_EventID
		{
			public const string Chat_GetMsg_NTF = "On_Chat_GetMsg_NTF";

			public const string Chat_Offline_GetMsg_NTF = "Chat_Offline_GetMsg_NTF";

			public const string Chat_FriendChatData_Change = "Chat_FriendChatData_Change";

			public const string Chat_LobbyChatData_Change = "Chat_LobbyChatData_Change";

			public const string Chat_GuildChatData_Change = "Chat_GuildChatData_Change";

			public const string Chat_GuildMatchTeamChatData_Change = "Chat_GuildMatchTeamChatData_Change";

			public const string Chat_Friend_Online_Change = "Chat_Friend_Online_Change";

			public const string Chat_PlayerLevel_Set = "Chat_PlayerLevel_Set";

			public const string Chat_ChatEntry_Change = "Chat_ChatEntry_Change";

			public const string Chat_RoomChatData_Change = "Chat_RoomChatData_Change";

			public const string Chat_HeorSelectChatData_Change = "Chat_HeorSelectChatData_Change";

			public const string Chat_TeamChat_Change = "Chat_TeamChat_Change";

			public const string Chat_Settle_Change = "Chat_Settle_Change";

			public const string Chat_PlayerLeaveSettle_Ntf = "Chat_PlayerLeaveSettle_Ntf";

			public const string Chat_Friend_NoAskFor_Change = "Chat_Friend_NoAskFor_Change";
		}

		public enum enCheckChatResult
		{
			Success,
			CdLimit,
			BanLimit,
			FreeCntLimit,
			FunUnlockLimit,
			EmptyLimit
		}

		public class ActionBtn
		{
			public GameObject btn;

			public string name;

			public CUIFormScript form;

			public ActionBtn(GameObject actionBtn, string btnName, CUIFormScript belongedForm)
			{
				this.btn = actionBtn;
				this.name = btnName;
				this.form = belongedForm;
			}
		}

		public static string fmt = "{0}：{1}";

		public static string fmt_blue_name = "<color=#224c87>{0}：</color>{1}";

		public static string fmt_gold_name = "<color=#c9a634>{0}：</color>{1}";

		public static string ChatFormPath = "UGUI/Form/System/Chat/ChatForm.prefab";

		public static string ChatEntryPath = "UGUI/Form/System/Chat/ChatEntry.prefab";

		public static string ChatSelectHeroPath_Normal = "UGUI/Form/System/Chat/Form_SelectChat_Normal.prefab";

		public static string ChatSelectHeroPath_BanPick = "UGUI/Form/System/Chat/Form_SelectChat_BanPick.prefab";

		public static string ChatPlayerInfo = "UGUI/Form/System/Chat/Form_ChatPlayerInfo.prefab";

		public static int MaxCount = 50;

		public static int init_chatTime = 4;

		public static int step_time = 2;

		public static int max_ChatTime = 30;

		public int cur_chatTimer_totalTime = 4;

		public bool bSendChat = true;

		private int chatTimer = -1;

		public CChatModel model;

		public CChatView view;

		public CHeroSelectChatView HeroSelectChatView;

		public AllColorParser ColorParser = new AllColorParser();

		private static int GuildRecruitRequestTimeInterval = 10;

		private int recruitTimer = -1;

		private List<string> m_cachedLeaveRoomPlayerNames;

		private DictionaryView<enUIEventID, ListView<CChatController.ActionBtn>> actionBtnsDic = new DictionaryView<enUIEventID, ListView<CChatController.ActionBtn>>();

		public CChatController()
		{
			this.model = new CChatModel();
			this.view = new CChatView();
			this.HeroSelectChatView = new CHeroSelectChatView();
			this.cur_chatTimer_totalTime = CChatController.init_chatTime;
			this.chatTimer = Singleton<CTimerManager>.GetInstance().AddTimer(CChatController.init_chatTime * 1000, -1, new CTimer.OnTimeUpHandler(this.On_Send_GetChat_Req));
			UT.ResetTimer(this.chatTimer, true);
			this.recruitTimer = Singleton<CTimerManager>.GetInstance().AddTimer(CChatController.GuildRecruitRequestTimeInterval * 1000, -1, new CTimer.OnTimeUpHandler(this.On_Send_GetGuildRecruit_Req));
			UT.ResetTimer(this.recruitTimer, true);
		}

		public void SetEntryVisible(bool bShow)
		{
			if (this.view != null)
			{
				this.view.SetEntryVisible(bShow);
			}
		}

		public void ClearAll()
		{
			if (this.model != null)
			{
				this.model.ClearAll();
			}
			if (this.actionBtnsDic != null)
			{
				DictionaryView<enUIEventID, ListView<CChatController.ActionBtn>>.Enumerator enumerator = this.actionBtnsDic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<enUIEventID, ListView<CChatController.ActionBtn>> current = enumerator.Current;
					ListView<CChatController.ActionBtn> value = current.get_Value();
					if (value != null)
					{
						value.Clear();
					}
				}
				this.actionBtnsDic.Clear();
			}
		}

		public void SetChatTimerEnable(bool b)
		{
			UT.ResetTimer(this.chatTimer, !b);
		}

		public void SetGuildRecruitTimerEnable(bool b)
		{
			UT.ResetTimer(this.recruitTimer, !b);
		}

		private void On_Send_GetChat_Req(int timerSequence)
		{
			if (this.bSendChat)
			{
				CChatNetUT.Send_GetChat_Req(EChatChannel.Lobby);
			}
		}

		private void On_Send_GetGuildRecruit_Req(int timerSequence)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.PvpLevel >= CGuildHelper.GetGuildMemberMinPvpLevel() && Singleton<CGuildSystem>.GetInstance() != null && !Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				uint lastRecruitInfoTime = this.GetLastRecruitInfoTime();
				CChatNetUT.RequestGetGuildRecruitReq(lastRecruitInfoTime);
			}
		}

		private uint GetLastRecruitInfoTime()
		{
			uint num = 0u;
			if (this.model != null && this.model.sysData != null && this.model.sysData.m_guildRecruitInfos.get_Count() > 0)
			{
				for (int i = 0; i < this.model.sysData.m_guildRecruitInfos.get_Count(); i++)
				{
					if (num < this.model.sysData.m_guildRecruitInfos.get_Item(i).sendTime)
					{
						num = this.model.sysData.m_guildRecruitInfos.get_Item(i).sendTime;
					}
				}
			}
			return num;
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
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("On_Chat_GetMsg_NTF", new Action<CSPkg>(this.On_Chat_GetMsg_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Chat_Offline_GetMsg_NTF", new Action<CSPkg>(this.On_Chat_Offline_GetMsg_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Chat_PlayerLeaveSettle_Ntf", new Action<CSPkg>(this.On_Chat_PlayerLeaveSettle_Ntf));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this.On_Chat_Friend_Online_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_PlayerLevel_Set", new Action(this.On_Chat_PlayerLevel_Set));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_HeorSelectChatData_Change", new Action(this.On_Chat_HeorSelectChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler<string>(EventID.ROLLING_SYSTEM_CHAT_INFO_RECEIVED, new Action<string>(this.On_Rolling_SystemChatInfoReceived));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int>(EventID.ERRCODE_NTF, new Action<int>(this.OnErrorCodeNtf));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", new Action<CGuildSystem.enPlatformGroupStatus, bool>(this.OnGuildPlatformGroupStatusChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_Chat_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_Chat_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_Chat_Tab_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ToolBar_Voice, new CUIEventManager.OnUIEventHandler(this.On_Chat_ToolBar_Voice));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ToolBar_Input, new CUIEventManager.OnUIEventHandler(this.On_Chat_ToolBar_Input));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ToolBar_Face, new CUIEventManager.OnUIEventHandler(this.On_Chat_ToolBar_Face));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ToolBar_Add, new CUIEventManager.OnUIEventHandler(this.On_Chat_ToolBar_Add));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Conent_Head_Icon, new CUIEventManager.OnUIEventHandler(this.On_Chat_Conent_Head_Icon));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_FriendTab_Item, new CUIEventManager.OnUIEventHandler(this.On_Chat_FriendTab_Item));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_List_FriendItem_Change, new CUIEventManager.OnUIEventHandler(this.On_Friend_TabList_Selected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_EntryPanel_Click, new CUIEventManager.OnUIEventHandler(this.On_Chat_EntryPanel_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_FaceList_Selected, new CUIEventManager.OnUIEventHandler(this.On_Chat_FaceList_Selected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ScreenButton_Click, new CUIEventManager.OnUIEventHandler(this.On_Chat_ScreenButton_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_SendButton_Click, new CUIEventManager.OnUIEventHandler(this.On_Chat_Text_Send));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Cost_Send, new CUIEventManager.OnUIEventHandler(this.On_Chat_Cost_Send));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ClearText_Click, new CUIEventManager.OnUIEventHandler(this.On_Chat_ClearText_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_LobbyChat_Elem_Enable, new CUIEventManager.OnUIEventHandler(this.On_List_ElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_FriendChat_Elem_Enable, new CUIEventManager.OnUIEventHandler(this.On_Chat_FriendChat_Elem_Enable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Timer_Changed, new CUIEventManager.OnUIEventHandler(this.OnChatTimerChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_Chat_Bubble_Close, new CUIEventManager.OnUIEventHandler(this.OnChatBubbleClose));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_Settle_Change", new Action(this.On_Chat_Settle_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Form_Open_Mini_Player_Info_Form, new CUIEventManager.OnUIEventHandler(this.OpenMiniPlayerInfoForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ClickBubble, new CUIEventManager.OnUIEventHandler(this.OnChat_ClickBubble));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_Bottom_BtnClick, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_Bottom_BtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_TabChange, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_TabChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_TemplateList_Click, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_TemplateList_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_List_ElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_Tab_Input, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Tab_Input));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_Tab_Voice, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Tab_Voice));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_Send_Text, new CUIEventManager.OnUIEventHandler(this.On_Chat_Hero_Select_Send));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_EntryNode_Open, new CUIEventManager.OnUIEventHandler(this.On_Speaker_EntryNode_Open));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_EntryNode_TimeUp, new CUIEventManager.OnUIEventHandler(this.On_Speaker_EntryNode_TimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnChatHeroSelectOpenSpeaker));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Hero_Select_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnChatHeroSelectOpenMic));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Guild_Recruit_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Chat_Guild_Recruit_List_Element_Enabled));
		}

		public void SubmitRefreshEvent()
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_FriendChatData_Change", new Action(this.On_Chat_FriendChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_LobbyChatData_Change", new Action(this.On_Chat_LobbyChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_GuildChatData_Change", new Action(this.On_Chat_GuildChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_GuildMatchTeamChatData_Change", new Action(this.On_Chat_GuildMatchTeamChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_ChatEntry_Change", new Action(this.On_Chat_ChatEntry_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_RoomChatData_Change", new Action(this.On_Chat_RoomChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_TeamChat_Change", new Action(this.On_Chat_TeamChatData_Change));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Enter_Guild", new Action(this.On_Guild_EnterGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Leave_Guild", new Action(this.On_Guild_LeaveGuild));
			if (this.view != null)
			{
				this.view.InitCheckTimer();
			}
		}

		public void CancleRefreshEvent()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_FriendChatData_Change", new Action(this.On_Chat_FriendChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_LobbyChatData_Change", new Action(this.On_Chat_LobbyChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_GuildChatData_Change", new Action(this.On_Chat_GuildChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_GuildMatchTeamChatData_Change", new Action(this.On_Chat_GuildMatchTeamChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_ChatEntry_Change", new Action(this.On_Chat_ChatEntry_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_RoomChatData_Change", new Action(this.On_Chat_RoomChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_TeamChat_Change", new Action(this.On_Chat_TeamChatData_Change));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Guild_Enter_Guild", new Action(this.On_Guild_EnterGuild));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Guild_Leave_Guild", new Action(this.On_Guild_LeaveGuild));
		}

		public void CreateForm()
		{
			if (this.view == null)
			{
				return;
			}
			this.view.CreateDetailChatForm();
		}

		public void ShowPanel(bool bShow, bool bDetail)
		{
			if (this.view == null)
			{
				return;
			}
			this.view.HideDetailChatForm();
			if (bShow)
			{
				if (bDetail)
				{
					this.view.ShowDetailChatForm();
				}
				else
				{
					this.view.ShowEntryForm();
				}
			}
		}

		public void ClearAllPanel()
		{
			if (this.view == null)
			{
				return;
			}
			this.view.ClearChatForm();
			Singleton<CUIManager>.GetInstance().CloseForm(CChatController.ChatFormPath);
		}

		public void Jump2FriendChat(COMDT_FRIEND_INFO info)
		{
			if (info == null)
			{
				return;
			}
			if (this.view != null)
			{
				this.view.Jump2FriendChat(info);
			}
		}

		private void On_Chat_FaceList_Selected(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.On_Chat_FaceList_Selected(uievent);
			}
		}

		private void On_Chat_EntryPanel_Click(CUIEvent uievent)
		{
			this.ShowPanel(true, true);
			if (this.view == null || this.view.chatForm == null || this.view.chatForm.gameObject == null)
			{
				return;
			}
			this.DoChatOpenningAnim();
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(this.view.chatForm.gameObject, "Timer");
			if (componetInChild == null)
			{
				return;
			}
			componetInChild.ReStartTimer();
		}

		private void On_Friend_TabList_Selected(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.On_Friend_TabList_Selected(uievent);
			}
		}

		private void On_Chat_Text_Send(CUIEvent uiEvent)
		{
			if (this.view != null && this.view.bShow)
			{
				string inputText = this.view.GetInputText();
				CChatController.enCheckChatResult enCheckChatResult = this.CheckSend(this.view.CurTab, inputText, true);
				if (enCheckChatResult == CChatController.enCheckChatResult.Success)
				{
					Singleton<CChatController>.GetInstance().On_InputFiled_EndEdit(inputText);
					this.view.ClearInputText();
				}
				else if (enCheckChatResult == CChatController.enCheckChatResult.EmptyLimit)
				{
					Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_10", true, 1.5f, null, new object[0]);
				}
				else if (enCheckChatResult == CChatController.enCheckChatResult.FunUnlockLimit)
				{
					Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_6", true, 1.5f, null, new object[0]);
				}
				else if (enCheckChatResult == CChatController.enCheckChatResult.BanLimit)
				{
					DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_DENYCHAT);
					string strContent = string.Format("您已被禁言！禁言截止时间为{0}年{1}月{2}日{3}时{4}分", new object[]
					{
						banTime.get_Year(),
						banTime.get_Month(),
						banTime.get_Day(),
						banTime.get_Hour(),
						banTime.get_Minute()
					});
					Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
					this.view.ClearInputText();
				}
				else if (enCheckChatResult == CChatController.enCheckChatResult.CdLimit)
				{
					CChatChannel channel = this.model.channelMgr.GetChannel(this.view.CurTab);
					int left_CDTime = channel.Get_Left_CDTime();
					Singleton<CUIManager>.instance.OpenTips("chat_input_cd", true, 1.5f, null, new object[0]);
					this.view.ClearInputText();
				}
				else if (enCheckChatResult == CChatController.enCheckChatResult.FreeCntLimit)
				{
					uiEvent.m_eventParams.tagStr = inputText;
					CMallSystem.TryToPay(enPayPurpose.Chat, string.Empty, enPayType.DiamondAndDianQuan, (uint)Singleton<CChatController>.instance.model.sysData.chatCostNum, enUIEventID.Chat_Cost_Send, ref uiEvent.m_eventParams, enUIEventID.None, false, true, false);
				}
			}
		}

		private void On_Chat_Cost_Send(CUIEvent cuiEvent)
		{
			string tagStr = cuiEvent.m_eventParams.tagStr;
			Singleton<CChatController>.GetInstance().On_InputFiled_EndEdit(tagStr);
			if (this.view != null && this.view.bShow)
			{
				this.view.ClearInputText();
			}
		}

		private void On_Chat_ClearText_Click(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.ClearInputText();
			}
		}

		private void On_List_ElementEnable(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.On_List_ElementEnable(uievent);
			}
		}

		private void On_Chat_FriendChat_Elem_Enable(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.On_FriendsList_ElementEnable(uievent);
			}
		}

		private void On_Speaker_EntryNode_Open(CUIEvent uievent)
		{
			if (this.view != null)
			{
				this.view.OpenSpeakerEntryNode(uievent.m_eventParams.tagStr);
			}
		}

		private void On_Speaker_EntryNode_TimeUp(CUIEvent uievent)
		{
			if (this.view != null)
			{
				this.view.CloseSpeakerEntryNode();
			}
		}

		public CChatController.enCheckChatResult CheckSend(EChatChannel channel, string content = "", bool bContentCheck = false)
		{
			if (bContentCheck && string.IsNullOrEmpty(content))
			{
				return CChatController.enCheckChatResult.EmptyLimit;
			}
			if (channel == EChatChannel.Lobby && !Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_CHAT))
			{
				return CChatController.enCheckChatResult.FunUnlockLimit;
			}
			DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_DENYCHAT);
			DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (banTime > dateTime)
			{
				return CChatController.enCheckChatResult.BanLimit;
			}
			CChatChannel channel2 = this.model.channelMgr.GetChannel(this.view.CurTab);
			if (!channel2.IsInputValid())
			{
				return CChatController.enCheckChatResult.CdLimit;
			}
			if (channel == EChatChannel.Lobby && !CSysDynamicBlock.bChatPayBlock && Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt <= 0u)
			{
				return CChatController.enCheckChatResult.FreeCntLimit;
			}
			return CChatController.enCheckChatResult.Success;
		}

		public bool On_InputFiled_EndEdit(string content)
		{
			if (this.view.CurTab == EChatChannel.Friend_Chat)
			{
				CChatNetUT.Send_Private_Chat(this.model.sysData.ullUid, this.model.sysData.dwLogicWorldId, content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.Friend);
			}
			if (this.view.CurTab == EChatChannel.Lobby)
			{
				CChatNetUT.Send_Lobby_Chat(content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.Lobby);
			}
			if (this.view.CurTab == EChatChannel.Guild)
			{
				CChatNetUT.Send_Guild_Chat(content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.Guild);
			}
			if (this.view.CurTab == EChatChannel.GuildMatchTeam)
			{
				CChatNetUT.Send_GuildMatchTeam_Chat(content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.GuildMatchTeam);
			}
			if (this.view.CurTab == EChatChannel.Room)
			{
				CChatNetUT.Send_Room_Chat(content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.Room);
			}
			if (this.view.CurTab == EChatChannel.Team)
			{
				CChatNetUT.Send_Team_Chat(content);
				CChatNetUT.Send_GetChat_Req(EChatChannel.Team);
			}
			if (this.view.CurTab == EChatChannel.Settle)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_isWarmBattle)
				{
					CChatEntity chatEnt = CChatUT.Build_4_Self(content);
					this.model.channelMgr.Add_ChatEntity(chatEnt, EChatChannel.Settle, 0uL, 0u);
					string a = string.Format(CChatController.fmt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name, content);
					this.model.sysData.Add_NewContent_Entry(a, EChatChannel.Settle);
					this.model.sysData.LastChannel = EChatChannel.Settle;
					Singleton<EventRouter>.instance.BroadCastEvent("Chat_Settle_Change");
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
				}
				else
				{
					CChatNetUT.Send_Settle_Chat(content);
					CChatNetUT.Send_GetChat_Req(EChatChannel.Settle);
				}
			}
			return true;
		}

		private void OnChatTimerChanged(CUIEvent cuiEvent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.Refresh_ChatInputView();
			}
		}

		private void OnChatBubbleClose(CUIEvent cuiEvent)
		{
			this.HeroSelectChatView.OnChatBubbleClose(cuiEvent.m_srcWidget);
		}

		private void On_Chat_OpenForm(CUIEvent uievent)
		{
			this.view.ShowDetailChatForm();
		}

		private void OnChatHeroSelectOpenSpeaker(CUIEvent uievent)
		{
			this.HeroSelectChatView.OnChatHeroSelectOpenSpeaker(false);
		}

		private void OnChatHeroSelectOpenMic(CUIEvent uievent)
		{
			this.HeroSelectChatView.OnChatHeroSelectOpenMic(true);
		}

		private void On_Chat_Guild_Recruit_List_Element_Enabled(CUIEvent uiEvent)
		{
			this.view.SetGuildRecruitListElement(uiEvent);
		}

		public bool IsMicUIOpen()
		{
			return this.HeroSelectChatView.IsUIOpenMic();
		}

		public void DoChatOpenningAnim()
		{
			if (this.view != null && this.view.Anim != null)
			{
				this.view.Anim.SetBool("ChatForm_Fade", true);
			}
		}

		public void DoChatClosingAnim()
		{
			if (this.view != null && this.view.Anim != null)
			{
				this.view.Anim.SetBool("ChatForm_Fade", false);
			}
		}

		private void On_Chat_CloseForm(CUIEvent uievent)
		{
			this.DoChatClosingAnim();
		}

		public void OnClosingAnimEnd()
		{
			if (Singleton<CUIManager>.instance.GetForm(CFriendContoller.FriendFormPath) != null)
			{
				this.view.HideDetailChatForm();
			}
			else
			{
				this.ShowPanel(true, false);
			}
		}

		private void On_Chat_Tab_Change(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				CUIListScript component = uievent.m_srcWidget.GetComponent<CUIListScript>();
				int index = component.GetSelectedIndex();
				index = component.GetElemenet(index).GetComponent<CUIEventScript>().m_onClickEventParams.tag;
				this.view.On_Tab_Change(index);
				this.view.ReBuildTabText();
			}
		}

		private void On_Chat_ToolBar_Voice(CUIEvent uievent)
		{
		}

		private void On_Chat_ToolBar_Input(CUIEvent uievent)
		{
		}

		private void On_Chat_ToolBar_Face(CUIEvent uievent)
		{
			if (this.view != null && this.view.bShow)
			{
				this.view.SetChatFaceShow(true);
			}
		}

		private void On_Chat_ToolBar_Add(CUIEvent uievent)
		{
		}

		private void On_Chat_Conent_Head_Icon(CUIEvent uievent)
		{
		}

		private void On_Chat_FriendTab_Item(CUIEvent uievent)
		{
		}

		private void OpenMiniPlayerInfoForm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag2;
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			if (masterRoleInfo.playerUllUID == commonUInt64Param)
			{
				return;
			}
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mini_Player_Info_Open_Form;
			cUIEvent.m_srcFormScript = uiEvent.m_srcFormScript;
			cUIEvent.m_eventParams.tag = 2;
			cUIEvent.m_eventParams.commonUInt64Param1 = commonUInt64Param;
			cUIEvent.m_eventParams.tag2 = tag;
			cUIEvent.m_eventParams.tagStr = uiEvent.m_eventParams.tagStr;
			cUIEvent.m_eventParams.tagStr1 = uiEvent.m_eventParams.tagStr1;
			cUIEvent.m_eventParams.pwd = uiEvent.m_eventParams.pwd;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnChat_ClickBubble(CUIEvent uiEvent)
		{
			if (this.view != null)
			{
				this.view.bRefreshNew = true;
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
			}
		}

		private void On_Chat_Hero_Select_OpenForm(CUIEvent uievent)
		{
			this.HeroSelectChatView.OpenForm();
		}

		private void On_Chat_Hero_Select_CloseForm(CUIEvent uievent)
		{
			this.HeroSelectChatView.CloseForm();
		}

		private void On_Chat_Hero_Select_Bottom_BtnClick(CUIEvent uievent)
		{
			this.HeroSelectChatView.On_Bottom_Btn_Click();
		}

		private void On_Chat_Hero_Select_TabChange(CUIEvent uievent)
		{
			int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			this.HeroSelectChatView.On_Tab_Change(selectedIndex);
		}

		public void Hide_SelectChat_MidNode()
		{
			if (this.HeroSelectChatView != null)
			{
				this.HeroSelectChatView.Show_SelectChat_MidNode(false);
			}
		}

		public void SetEntryNodeVoiceBtnShowable(bool bShow)
		{
			if (this.HeroSelectChatView != null)
			{
				this.HeroSelectChatView.SetEntryNodeVoiceBtnShowable(bShow);
			}
		}

		public void Set_Show_Bottom(bool bShow)
		{
			if (this.HeroSelectChatView != null)
			{
				this.HeroSelectChatView.Set_Show_Bottom(bShow);
			}
		}

		private void On_Chat_Hero_Select_TemplateList_Click(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			this.HeroSelectChatView.On_List_Item_Click(srcWidgetIndexInBelongedList);
		}

		private void On_Chat_Hero_Tab_Input(CUIEvent uievent)
		{
			this.HeroSelectChatView.ChatType = CHeroSelectChatView.enChatType.Text;
		}

		private void On_Chat_Hero_Tab_Voice(CUIEvent uievent)
		{
			this.HeroSelectChatView.ChatType = CHeroSelectChatView.enChatType.Voice;
		}

		private void On_Chat_Hero_Select_Send(CUIEvent uievent)
		{
			this.HeroSelectChatView.On_Send_Text();
		}

		private void On_Chat_Hero_Select_List_ElementEnable(CUIEvent uievent)
		{
			this.HeroSelectChatView.On_List_ElementEnable(uievent);
		}

		private void On_Chat_HeorSelectChatData_Change()
		{
			this.HeroSelectChatView.On_Chat_HeorSelectChatData_Change();
		}

		private void On_Chat_PlayerLevel_Set()
		{
			ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel);
			CChatChannelMgr channelMgr = Singleton<CChatController>.GetInstance().model.channelMgr;
			channelMgr.GetChannel(EChatChannel.Lobby).InitChat_InputTimer((int)dataByKey.dwChatCD);
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel == CGuildHelper.GetGuildMemberMinPvpLevel() && channelMgr.ChatTab == CChatChannelMgr.EChatTab.Normal)
			{
				channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
			}
		}

		private void On_Chat_Offline_GetMsg_NTF(CSPkg msg)
		{
			SCPKG_OFFLINE_CHAT_NTF stOfflineChatNtf = msg.stPkgData.stOfflineChatNtf;
			for (int i = 0; i < (int)stOfflineChatNtf.bChatCnt; i++)
			{
				CSDT_OFFLINE_CHAT_INFO cSDT_OFFLINE_CHAT_INFO = stOfflineChatNtf.astChatInfo[i];
				if (cSDT_OFFLINE_CHAT_INFO != null)
				{
					COMDT_CHAT_PLAYER_INFO stFrom = cSDT_OFFLINE_CHAT_INFO.stChatMsg.stChatMsg.stFrom;
					COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(stFrom.ullUid, (uint)stFrom.iLogicWorldID);
					if (gameOrSnsFriend != null)
					{
						CChatChannel cChatChannel = this.model.channelMgr._getChannel(EChatChannel.Friend, stFrom.ullUid, (uint)stFrom.iLogicWorldID);
						if (cChatChannel == null)
						{
							cChatChannel = this.model.channelMgr.CreateChannel(EChatChannel.Friend, stFrom.ullUid, (uint)stFrom.iLogicWorldID);
							cChatChannel.bOffline = true;
							if (gameOrSnsFriend.bIsOnline == 0)
							{
								cChatChannel.list.Add(CChatUT.Build_4_OfflineInfo(Singleton<CTextManager>.instance.GetText("FriendChat_Offline_Info")));
							}
							cChatChannel.list.Add(CChatUT.Build_4_Time((int)cSDT_OFFLINE_CHAT_INFO.stChatMsg.dwChatTime));
						}
						CChatEntity cChatEntity = CChatUT.Build_4_Offline_Friend(cSDT_OFFLINE_CHAT_INFO.stChatMsg);
						if (cChatEntity != null)
						{
							Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = false;
							Singleton<CChatController>.instance.view.ChatParser.maxWidth = CChatParser.chat_list_max_width;
							Singleton<CChatController>.instance.view.ChatParser.Parse(cChatEntity.text, CChatParser.start_x, cChatEntity);
							cChatChannel.Add(cChatEntity);
							this.model.AddOfflineChatIndex(stFrom.ullUid, (uint)stFrom.iLogicWorldID, cSDT_OFFLINE_CHAT_INFO.iIndex);
							if (i == (int)(stOfflineChatNtf.bChatCnt - 1))
							{
								string rawText = UT.Bytes2String(cSDT_OFFLINE_CHAT_INFO.stChatMsg.stChatMsg.szContent);
								string a = CChatUT.Build_4_EntryString(EChatChannel.Friend, stFrom.ullUid, (uint)stFrom.iLogicWorldID, rawText);
								this.model.sysData.Add_NewContent_Entry(a, EChatChannel.Friend);
								Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
								this.model.sysData.LastChannel = EChatChannel.Friend;
							}
						}
					}
				}
			}
		}

		private void On_Chat_PlayerLeaveSettle_Ntf(CSPkg msg)
		{
			string playerName = Singleton<BattleStatistic>.GetInstance().GetPlayerName(msg.stPkgData.stLeaveSettleUiNtf.ullUid, msg.stPkgData.stLeaveSettleUiNtf.dwLogicWorldID);
			if (Singleton<SettlementSystem>.GetInstance().IsInSettlementState())
			{
				this.BuildPlayerLeaveRoomSystemMsg(playerName);
			}
			else if (Singleton<BattleLogic>.GetInstance().isRuning)
			{
				if (this.m_cachedLeaveRoomPlayerNames == null)
				{
					this.m_cachedLeaveRoomPlayerNames = new List<string>();
				}
				this.m_cachedLeaveRoomPlayerNames.Add(playerName);
			}
		}

		public void BuildWarmBattlePlayerLeaveRoomSystemMsg()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null || !curLvelContext.m_isWarmBattle)
			{
				return;
			}
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			if (playerKDAStat == null)
			{
				return;
			}
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.get_Value().IsComputer)
				{
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					this.BuildPlayerLeaveRoomSystemMsg(current2.get_Value().PlayerName);
				}
			}
		}

		public void BuildCachedPlayerLeaveRoomSystemMsg()
		{
			if (this.m_cachedLeaveRoomPlayerNames != null && this.m_cachedLeaveRoomPlayerNames.get_Count() > 0)
			{
				for (int i = 0; i < this.m_cachedLeaveRoomPlayerNames.get_Count(); i++)
				{
					this.BuildPlayerLeaveRoomSystemMsg(this.m_cachedLeaveRoomPlayerNames.get_Item(i));
				}
			}
		}

		public void ClearCachedPlayerLeaveRoomSystemMsg()
		{
			if (this.m_cachedLeaveRoomPlayerNames != null)
			{
				this.m_cachedLeaveRoomPlayerNames.Clear();
			}
		}

		private void BuildPlayerLeaveRoomSystemMsg(string playerName)
		{
			CChatChannel channel = this.model.channelMgr.GetChannel(EChatChannel.Settle);
			if (channel == null)
			{
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("Chat_Somebody_Leave_Room", new string[]
			{
				playerName
			});
			channel.Add(CChatUT.Build_4_LeaveRoom(text));
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_RoomChatData_Change");
			this.model.sysData.Add_NewContent_Entry_ColorFlag(text, EChatChannel.Settle);
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
		}

		public void BuildPlayerGiveThumbsSysTemMsg(string giveThumbsPlayer, string recieveThumbsPlayer)
		{
			CChatChannel channel = this.model.channelMgr.GetChannel(EChatChannel.Settle);
			if (channel == null)
			{
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("Chat_Somebody_Give_A_Thumbs", new string[]
			{
				giveThumbsPlayer,
				recieveThumbsPlayer
			});
			channel.Add(CChatUT.Build_4_LeaveRoom(text));
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_RoomChatData_Change");
			this.model.sysData.Add_NewContent_Entry_ColorFlag(text, EChatChannel.Settle);
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
		}

		public void RefreshGuildRecruitInfo(SCPKG_GET_GUILD_RECRUIT_RSP rsp)
		{
			if (this.model == null || this.model.sysData == null)
			{
				return;
			}
			for (int i = 0; i < rsp.stInfo.iNum; i++)
			{
				GuildRecruitInfo guildRecruitInfo = default(GuildRecruitInfo);
				COMDT_GUILD_RECRUIT_INFO cOMDT_GUILD_RECRUIT_INFO = rsp.stInfo.astInfo[i];
				guildRecruitInfo.senderUid = cOMDT_GUILD_RECRUIT_INFO.ullSenderUid;
				guildRecruitInfo.senderLevel = cOMDT_GUILD_RECRUIT_INFO.bSenderLevel;
				guildRecruitInfo.guildId = cOMDT_GUILD_RECRUIT_INFO.ullGuildID;
				guildRecruitInfo.guildLogicWorldId = cOMDT_GUILD_RECRUIT_INFO.iLogicWorldID;
				guildRecruitInfo.sendTime = cOMDT_GUILD_RECRUIT_INFO.dwSendTime;
				guildRecruitInfo.guildName = StringHelper.UTF8BytesToString(ref cOMDT_GUILD_RECRUIT_INFO.szGuildName);
				guildRecruitInfo.senderName = StringHelper.UTF8BytesToString(ref cOMDT_GUILD_RECRUIT_INFO.szSendName);
				guildRecruitInfo.senderHeadUrl = StringHelper.UTF8BytesToString(ref cOMDT_GUILD_RECRUIT_INFO.szSenderHeadUrl);
				guildRecruitInfo.limitGrade = CGuildHelper.GetFixedGuildGradeLimit(cOMDT_GUILD_RECRUIT_INFO.bLimitGrade);
				guildRecruitInfo.limitLevel = CGuildHelper.GetFixedGuildLevelLimit(cOMDT_GUILD_RECRUIT_INFO.bLimitLevel);
				this.model.sysData.m_guildRecruitInfos.Add(guildRecruitInfo);
			}
			int num = this.model.sysData.m_guildRecruitInfos.get_Count() - 100;
			if (num > 0)
			{
				this.model.sysData.m_guildRecruitInfos.RemoveRange(0, num);
			}
		}

		private void On_Chat_GetMsg_NTF(CSPkg msg)
		{
			SCPKG_CMD_CHAT_NTF stChatNtf = msg.stPkgData.stChatNtf;
			if (stChatNtf.dwTimeStamp != 0u)
			{
				Singleton<CChatController>.GetInstance().model.SetTimeStamp(EChatChannel.Lobby, stChatNtf.dwTimeStamp);
			}
			Singleton<CChatController>.GetInstance().model.SetRestFreeCnt(EChatChannel.Lobby, stChatNtf.dwRestChatFreeCnt);
			int num = Mathf.Min(stChatNtf.astChatMsg.Length, (int)stChatNtf.bMsgCnt);
			if (num == 0)
			{
				this.cur_chatTimer_totalTime += CChatController.step_time;
				if (this.cur_chatTimer_totalTime >= CChatController.max_ChatTime)
				{
					this.cur_chatTimer_totalTime = CChatController.max_ChatTime;
				}
				Singleton<CTimerManager>.instance.ResetTimerTotalTime(this.chatTimer, this.cur_chatTimer_totalTime * 1000);
				return;
			}
			this.cur_chatTimer_totalTime = CChatController.init_chatTime;
			Singleton<CTimerManager>.instance.ResetTimerTotalTime(this.chatTimer, this.cur_chatTimer_totalTime * 1000);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			for (int i = num - 1; i >= 0; i--)
			{
				COMDT_CHAT_MSG cOMDT_CHAT_MSG = stChatNtf.astChatMsg[i];
				if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Friend)
				{
					COMDT_CHAT_PLAYER_INFO stFrom = cOMDT_CHAT_MSG.stContent.stPrivate.stFrom;
					if (stFrom.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
					{
						string text = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stPrivate.szContent);
						CChatEntity chatEnt = CChatUT.Build_4_Self(text);
						this.model.channelMgr.Add_CurChatFriend(chatEnt);
						string a = string.Format(CChatController.fmt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name, text);
						this.model.sysData.Add_NewContent_Entry(a, EChatChannel.Friend);
						Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_FriendChatData_Change");
						Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
						this.model.sysData.LastChannel = EChatChannel.Friend;
						flag = true;
					}
					else
					{
						if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom.ullUid, (uint)stFrom.iLogicWorldID))
						{
							return;
						}
						string rawText = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stPrivate.szContent);
						CChatEntity chatEnt2 = CChatUT.Build_4_Friend(cOMDT_CHAT_MSG.stContent.stPrivate);
						this.model.channelMgr.Add_ChatEntity(chatEnt2, EChatChannel.Friend, stFrom.ullUid, (uint)stFrom.iLogicWorldID);
						string a2 = CChatUT.Build_4_EntryString(EChatChannel.Friend, stFrom.ullUid, (uint)stFrom.iLogicWorldID, rawText);
						this.model.sysData.Add_NewContent_Entry(a2, EChatChannel.Friend);
						this.model.sysData.LastChannel = EChatChannel.Friend;
						flag = true;
					}
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Lobby)
				{
					COMDT_CHAT_PLAYER_INFO stFrom2 = cOMDT_CHAT_MSG.stContent.stLogicWord.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom2.ullUid, (uint)stFrom2.iLogicWorldID))
					{
						return;
					}
					bool flag8 = stFrom2.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag8);
					}
					this.model.Add_Palyer_Info(cOMDT_CHAT_MSG.stContent.stLogicWord.stFrom);
					CChatEntity cChatEntity = CChatUT.Build_4_Lobby(cOMDT_CHAT_MSG.stContent.stLogicWord);
					if (flag8)
					{
						cChatEntity.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity, EChatChannel.Lobby, 0uL, 0u);
					string rawText2 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stLogicWord.szContent);
					string a3 = CChatUT.Build_4_EntryString(EChatChannel.Lobby, stFrom2.ullUid, (uint)stFrom2.iLogicWorldID, rawText2);
					this.model.sysData.Add_NewContent_Entry(a3, EChatChannel.Lobby);
					this.model.sysData.LastChannel = EChatChannel.Lobby;
					flag2 = true;
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Guild)
				{
					COMDT_CHAT_PLAYER_INFO stFrom3 = cOMDT_CHAT_MSG.stContent.stGuild.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom3.ullUid, (uint)stFrom3.iLogicWorldID))
					{
						return;
					}
					bool flag9 = stFrom3.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag9);
					}
					this.model.Add_Palyer_Info(cOMDT_CHAT_MSG.stContent.stGuild.stFrom);
					CChatEntity cChatEntity2 = CChatUT.Build_4_Guild(cOMDT_CHAT_MSG.stContent.stGuild);
					if (flag9)
					{
						cChatEntity2.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity2, EChatChannel.Guild, 0uL, 0u);
					string rawText3 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stGuild.szContent);
					string a4 = CChatUT.Build_4_EntryString(EChatChannel.Guild, stFrom3.ullUid, (uint)stFrom3.iLogicWorldID, rawText3);
					this.model.sysData.Add_NewContent_Entry(a4, EChatChannel.Guild);
					this.model.sysData.LastChannel = EChatChannel.Guild;
					flag4 = true;
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.GuildMatchTeam)
				{
					COMDT_CHAT_PLAYER_INFO stFrom4 = cOMDT_CHAT_MSG.stContent.stGuildTeam.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom4.ullUid, (uint)stFrom4.iLogicWorldID))
					{
						return;
					}
					bool flag10 = stFrom4.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag10);
					}
					this.model.Add_Palyer_Info(stFrom4);
					CChatEntity cChatEntity3 = CChatUT.Build_4_GuildMatchTeam(cOMDT_CHAT_MSG.stContent.stGuildTeam);
					if (flag10)
					{
						cChatEntity3.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity3, EChatChannel.GuildMatchTeam, 0uL, 0u);
					string rawText4 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stGuildTeam.szContent);
					string a5 = CChatUT.Build_4_EntryString(EChatChannel.GuildMatchTeam, stFrom4.ullUid, (uint)stFrom4.iLogicWorldID, rawText4);
					this.model.sysData.Add_NewContent_Entry(a5, EChatChannel.GuildMatchTeam);
					this.model.sysData.LastChannel = EChatChannel.GuildMatchTeam;
					flag5 = true;
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Room)
				{
					COMDT_CHAT_PLAYER_INFO stFrom5 = cOMDT_CHAT_MSG.stContent.stRoom.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom5.ullUid, (uint)stFrom5.iLogicWorldID))
					{
						return;
					}
					bool flag11 = stFrom5.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag11);
					}
					this.model.Add_Palyer_Info(stFrom5);
					CChatEntity cChatEntity4 = CChatUT.Build_4_Room(cOMDT_CHAT_MSG.stContent.stRoom);
					if (flag11)
					{
						cChatEntity4.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity4, EChatChannel.Room, 0uL, 0u);
					this.model.sysData.LastChannel = EChatChannel.Room;
					string rawText5 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stRoom.szContent);
					string a6 = CChatUT.Build_4_EntryString(EChatChannel.Room, stFrom5.ullUid, (uint)stFrom5.iLogicWorldID, rawText5);
					this.model.sysData.Add_NewContent_Entry(a6, EChatChannel.Room);
					flag3 = true;
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_RoomChatData_Change");
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Select_Hero)
				{
					COMDT_CHAT_PLAYER_INFO stFrom6 = cOMDT_CHAT_MSG.stContent.stBattle.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom6.ullUid, (uint)stFrom6.iLogicWorldID))
					{
						return;
					}
					bool flag12 = stFrom6.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag12);
					}
					this.model.Add_Palyer_Info(cOMDT_CHAT_MSG.stContent.stBattle.stFrom);
					CChatEntity chatEnt3 = CChatUT.Build_4_SelectHero(cOMDT_CHAT_MSG.stContent.stBattle);
					this.model.channelMgr.Add_ChatEntity(chatEnt3, EChatChannel.Select_Hero, 0uL, 0u);
					Singleton<EventRouter>.instance.BroadCastEvent("Chat_HeorSelectChatData_Change");
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Team)
				{
					COMDT_CHAT_PLAYER_INFO stFrom7 = cOMDT_CHAT_MSG.stContent.stTeam.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom7.ullUid, (uint)stFrom7.iLogicWorldID))
					{
						return;
					}
					bool flag13 = stFrom7.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag13);
					}
					this.model.Add_Palyer_Info(cOMDT_CHAT_MSG.stContent.stTeam.stFrom);
					CChatEntity cChatEntity5 = CChatUT.Build_4_Team(cOMDT_CHAT_MSG.stContent.stTeam);
					if (flag13)
					{
						cChatEntity5.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity5, EChatChannel.Team, 0uL, 0u);
					string rawText6 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stTeam.szContent);
					string a7 = CChatUT.Build_4_EntryString(EChatChannel.Team, stFrom7.ullUid, (uint)stFrom7.iLogicWorldID, rawText6);
					this.model.sysData.Add_NewContent_Entry(a7, EChatChannel.Team);
					this.model.sysData.LastChannel = EChatChannel.Team;
					flag6 = true;
					Singleton<EventRouter>.instance.BroadCastEvent("Chat_TeamChat_Change");
				}
				else if (CChatUT.Convert_ChatMsgType_Channel(cOMDT_CHAT_MSG.bType) == EChatChannel.Settle)
				{
					COMDT_CHAT_PLAYER_INFO stFrom8 = cOMDT_CHAT_MSG.stContent.stSettle.stFrom;
					if (Singleton<CFriendContoller>.instance.model.IsBlack(stFrom8.ullUid, (uint)stFrom8.iLogicWorldID))
					{
						return;
					}
					bool flag14 = stFrom8.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
					if (this.view != null)
					{
						this.view.bRefreshNew = (!this.view.IsCheckHistory() || flag14);
					}
					this.model.Add_Palyer_Info(cOMDT_CHAT_MSG.stContent.stSettle.stFrom);
					CChatEntity cChatEntity6 = CChatUT.Build_4_Settle(cOMDT_CHAT_MSG.stContent.stSettle);
					if (flag14)
					{
						cChatEntity6.type = EChaterType.Self;
					}
					this.model.channelMgr.Add_ChatEntity(cChatEntity6, EChatChannel.Settle, 0uL, 0u);
					string rawText7 = UT.Bytes2String(cOMDT_CHAT_MSG.stContent.stSettle.szContent);
					string a8 = CChatUT.Build_4_EntryString(EChatChannel.Settle, stFrom8.ullUid, (uint)stFrom8.iLogicWorldID, rawText7);
					this.model.sysData.Add_NewContent_Entry(a8, EChatChannel.Settle);
					this.model.sysData.LastChannel = EChatChannel.Settle;
					flag7 = true;
					Singleton<EventRouter>.instance.BroadCastEvent("Chat_Settle_Change");
				}
				else if (cOMDT_CHAT_MSG.bType == 7)
				{
					Singleton<InBattleMsgMgr>.instance.Handle_InBattleMsg_Ntf(cOMDT_CHAT_MSG.stContent.stInBattle);
				}
			}
			if (flag2)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_LobbyChatData_Change");
			}
			if (flag)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_FriendChatData_Change");
			}
			if (flag4)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_GuildChatData_Change");
			}
			if (flag5)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_GuildMatchTeamChatData_Change");
			}
			if ((flag2 || flag || flag4) && this.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Normal)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
			if ((flag || flag3) && this.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Room)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
			if ((flag || flag6) && this.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Team)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
			if ((flag5 || flag4 || flag) && this.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.GuildMatch)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
			if (flag7 && this.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Settle)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
		}

		private void On_Chat_Friend_Online_Change(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldId, bool bOffline)
		{
			if (this.view != null && this.view.bShow)
			{
				if (Singleton<CChatController>.GetInstance().model.sysData.ullUid == ullUid && Singleton<CChatController>.GetInstance().model.sysData.dwLogicWorldId == dwLogicWorldId)
				{
					CChatChannel cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr._getChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
					if (cChatChannel == null)
					{
						cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.CreateChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
					}
					if (cChatChannel != null)
					{
						CChatEntity cChatEntity = CChatUT.Build_4_OfflineOrOnline(bOffline);
						if (cChatEntity != null)
						{
							cChatChannel.bOffline = bOffline;
							cChatChannel.Add(cChatEntity);
						}
					}
				}
				if (this.view.CurTab == EChatChannel.Friend)
				{
					this.view.Process_Friend_Tip();
				}
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void OnErrorCodeNtf(int errorCode)
		{
			if (errorCode == 138)
			{
				CChatChannel channel = this.model.channelMgr.GetChannel(EChatChannel.Lobby);
				if (channel != null)
				{
					channel.ClearCd();
					if (this.view != null)
					{
						this.view.Refresh_ChatInputView();
					}
				}
			}
			else if (errorCode == 137)
			{
				CChatChannel channel2 = this.model.channelMgr.GetChannel(EChatChannel.Lobby);
				if (channel2 != null)
				{
					channel2.Start_InputCD();
					if (this.view != null)
					{
						this.view.Refresh_ChatInputView();
					}
				}
			}
		}

		private void On_Chat_FriendChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_LobbyChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(false, EChatChannel.Lobby);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_GuildChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_GuildMatchTeamChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_RoomChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_TeamChatData_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_Settle_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.view.Refresh_All_RedPoint();
			}
		}

		private void On_Chat_ChatEntry_Change()
		{
			if (this.view != null)
			{
				this.view.Refresh_EntryForm();
			}
		}

		private void On_Chat_ScreenButton_Click(CUIEvent uievent)
		{
			if (this.view != null)
			{
				this.view.On_Chat_ScreenButton_Click();
			}
		}

		private void On_Guild_EnterGuild()
		{
			this.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
		}

		private void On_Guild_LeaveGuild()
		{
			this.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
		}

		private void On_Rolling_SystemChatInfoReceived(string content)
		{
			CChatEntity chatEnt = CChatUT.Build_4_System(content);
			this.model.channelMgr.Add_ChatEntity(chatEnt, EChatChannel.Lobby, 0uL, 0u);
		}

		private void OnGuildPlatformGroupStatusChange(CGuildSystem.enPlatformGroupStatus status, bool isSelfInGroup)
		{
			if (this.actionBtnsDic.ContainsKey(enUIEventID.Guild_JoinPlatformGroup))
			{
				CChatParser cChatParser = new CChatParser();
				ListView<CChatController.ActionBtn> listView = this.actionBtnsDic[enUIEventID.Guild_JoinPlatformGroup];
				for (int i = 0; i < listView.Count; i++)
				{
					CChatController.ActionBtn actionBtn = listView[i];
					if (status != CGuildSystem.enPlatformGroupStatus.Bound || isSelfInGroup)
					{
						actionBtn.btn.CustomSetActive(false);
					}
					else
					{
						CUIEventScript cUIEventScript = actionBtn.btn.GetComponent<CUIEventScript>();
						if (cUIEventScript == null)
						{
							cUIEventScript = actionBtn.btn.AddComponent<CUIEventScript>();
							cUIEventScript.Initialize(actionBtn.form);
							cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_JoinPlatformGroup);
						}
						float num = 0f;
						for (int j = 0; j < actionBtn.name.get_Length(); j++)
						{
							char ch = actionBtn.name.get_Chars(i);
							num += (float)CChatParser.GetCharacterWidth(ch, cChatParser.viewFontSize);
						}
						num += 16f;
						RectTransform component = actionBtn.btn.GetComponent<RectTransform>();
						Text componetInChild = Utility.GetComponetInChild<Text>(actionBtn.btn, "Text");
						if (component != null && componetInChild != null)
						{
							componetInChild.set_text(actionBtn.name);
							component.sizeDelta = new Vector2(num, component.sizeDelta.y);
							actionBtn.btn.CustomSetActive(true);
						}
						else
						{
							actionBtn.btn.CustomSetActive(false);
						}
					}
					listView.Remove(actionBtn);
				}
			}
		}

		public void FilterActionBtn(enUIEventID uiEventID, GameObject btn, string btnName, CUIFormScript form)
		{
			CChatController.ActionBtn item = new CChatController.ActionBtn(btn, btnName, form);
			if (this.actionBtnsDic.ContainsKey(uiEventID))
			{
				this.actionBtnsDic[uiEventID].Add(item);
			}
			else
			{
				this.actionBtnsDic.Add(uiEventID, new ListView<CChatController.ActionBtn>());
				this.actionBtnsDic[uiEventID].Add(item);
			}
			if (uiEventID == enUIEventID.Guild_JoinPlatformGroup)
			{
				Singleton<CGuildSystem>.GetInstance().JudgePlatformGroupJoind();
			}
		}
	}
}

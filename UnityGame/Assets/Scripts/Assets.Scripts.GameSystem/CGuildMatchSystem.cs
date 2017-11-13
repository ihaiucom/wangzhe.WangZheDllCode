using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CGuildMatchSystem : Singleton<CGuildMatchSystem>
	{
		public enum enGuildMatchRecordFormWidget
		{
			Record_List
		}

		public enum enRankTab
		{
			GuildScore,
			MemberScore,
			MemberInviteOrInvitation
		}

		public class TeamPlayerInfo
		{
			public ulong Uid;

			public string Name;

			public string HeadUrl;

			public bool IsReady;

			public TeamPlayerInfo(ulong uid, string name, string headUrl, bool isReady)
			{
				this.Uid = uid;
				this.Name = name;
				this.HeadUrl = headUrl;
				this.IsReady = isReady;
			}
		}

		private struct stSignUpInfo
		{
			public ulong uid;

			public byte pos1;

			public byte pos2;

			public string memo;

			public string[] heroNames;

			public byte[] winPercents;
		}

		private enum enSignUpListSortType
		{
			Default,
			RankGradeDesc,
			LevelDesc,
			PosDesc,
			StateDesc
		}

		private struct stInvitationInfo
		{
			public CSDT_GUILDMATCH_INVITEINFO invitationInfo;

			public bool isValid;
		}

		public const int TeamSlotCount = 5;

		public const int InviteMessageBoxAutoCloseTimeSeconds = 10;

		public const int RemindButtonCdSeconds = 10;

		public const int NeedRequestNewRecordTimeMilliSeconds = 300000;

		private const int GuildMatchRuleTextIndex = 15;

		public const string CommonYellowColorStartMark = "<color=#e49316>";

		public const string CommonGreenColorStartMark = "<color=#27b56a>";

		public const string ColorEndMark = "</color>";

		public const int SignUpListInviteBtnClickTimeMilliSeconds = 10000;

		public const string PrefKeyNextGuildMatchEndTimeRecrod = "GuildMatch_GuildMatchEndTimeRecrod";

		public const string PrefKeyIsGuildMatchBtnClicked = "GuildMatch_GuildMatchBtnClicked";

		public const string PrefKeyIsInvitationTabClicked = "GuildMatch_InvitationTabClicked";

		private bool m_isNeedRequestNewRecord = true;

		public static readonly string GuildMatchFormPath = "UGUI/Form/System/Guild/Form_Guild_Match.prefab";

		public static readonly string GuildMatchRecordFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_Record.prefab";

		public static readonly string GuildMatchSignUpCardFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_SignUpCard.prefab";

		public static readonly string GuildMatchSignUpListFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_SignUpList.prefab";

		public static readonly string GuildMatchInvitationFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_Invitation.prefab";

		public static readonly string GuildMatchOnlineInvitationFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_OnlineInvitation.prefab";

		private bool m_isReady;

		public bool OpenWhenInvitedFromPlatformGroup;

		private CUIFormScript m_form;

		private CUIFormScript m_matchRecordForm;

		private CUIFormScript m_createSignUpCardForm;

		private CUIFormScript m_modifySignUpCardForm;

		private CUIFormScript m_signUpListForm;

		private CUIFormScript m_invitationForm;

		private List<KeyValuePair<ulong, ListView<CGuildMatchSystem.TeamPlayerInfo>>> m_teamInfos;

		private ListView<GuildMemInfo> m_guildMemberInviteList;

		private ListView<GuildMemInfo> m_guildMemberScoreList;

		private CSDT_RANKING_LIST_ITEM_INFO[] m_guildSeasonScores;

		private CSDT_RANKING_LIST_ITEM_INFO[] m_guildWeekScores;

		private COMDT_GUILD_MATCH_HISTORY_INFO[] m_matchRecords;

		private byte m_signUpPos1;

		private byte m_signUpPos2;

		private string[] m_signUpPosTexts;

		private string m_signUpMemoText;

		private bool m_isSignedUp;

		private bool m_isSignUpCardBubbleShowed;

		private bool m_isSignUpListBubbleShowed;

		private List<CGuildMatchSystem.stSignUpInfo> m_signUpInfos;

		private CGuildMatchSystem.enSignUpListSortType m_curSignUpListSortType;

		private List<CGuildMatchSystem.stInvitationInfo> m_invitationInfos;

		private ulong m_curViewInvitationInviterUid;

		public bool m_isHaveNewSignUpInfo;

		private List<ulong> m_unhandledOnlineInvitationInviterUids;

		private List<ulong> m_openedOnlineInvitationInviterUids;

		public bool IsReady
		{
			get
			{
				return this.m_isReady;
			}
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnMatchFormOpened, new CUIEventManager.OnUIEventHandler(this.OnMatchFormOpened));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnMatchFormClosed, new CUIEventManager.OnUIEventHandler(this.OnMatchFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnMatchRecordFormClosed, new CUIEventManager.OnUIEventHandler(this.OnMatchRecordFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchRecordForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchRecordForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_StartGame, new CUIEventManager.OnUIEventHandler(this.OnStartGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OBGame, new CUIEventManager.OnUIEventHandler(this.OnOBGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ReadyGame, new CUIEventManager.OnUIEventHandler(this.OnReadyGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_CancelReadyGame, new CUIEventManager.OnUIEventHandler(this.OnCancelReadyGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RankTabChanged, new CUIEventManager.OnUIEventHandler(this.OnRankTabChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_GuildScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnGuildScoreListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_MemberScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnMemberScoreListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_MemberInviteListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnMemberInviteListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Invite, new CUIEventManager.OnUIEventHandler(this.OnInvite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Kick, new CUIEventManager.OnUIEventHandler(this.OnKick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_KickConfirm, new CUIEventManager.OnUIEventHandler(this.OnKickConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AppointOrCancelLeader, new CUIEventManager.OnUIEventHandler(this.OnAppointOrCancelLeader));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AppointOrCancelLeaderConfirm, new CUIEventManager.OnUIEventHandler(this.OnAppointOrCancelLeaderConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Accept_Invite, new CUIEventManager.OnUIEventHandler(this.OnAcceptInvite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Refuse_Invite, new CUIEventManager.OnUIEventHandler(this.OnRefuseInvite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnRefreshGameStateTimeout));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ObWaitingTimeout, new CUIEventManager.OnUIEventHandler(this.OnObWaitingTimeout));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RecordListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnRecordListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RankSubTitleSliderValueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubRankSliderValueChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchFormAndReadyGame, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchFormAndReadyGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Remind_Ready, new CUIEventManager.OnUIEventHandler(this.OnRemindReady));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Open_Rule, new CUIEventManager.OnUIEventHandler(this.OnOpenRule));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Team_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.OnTeamListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_InviteConfirm, new CUIEventManager.OnUIEventHandler(this.OnInviteConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RemindButtonCdOver, new CUIEventManager.OnUIEventHandler(this.OnRemindButtonCdOver));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenSignUpCardForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSignUpCardForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenSignUpListForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSignUpListForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_CreateSignUpCard, new CUIEventManager.OnUIEventHandler(this.OnCreateSignUpCard));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenModifySignUpCardForm, new CUIEventManager.OnUIEventHandler(this.OnOpenModifySignUpCardForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ModifySignUpCard, new CUIEventManager.OnUIEventHandler(this.OnModifySignUpCard));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ViewInvitation, new CUIEventManager.OnUIEventHandler(this.OnViewInvitation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AcceptInvitationBtnClick, new CUIEventManager.OnUIEventHandler(this.OnAcceptInvitationBtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RefuseInvitationBtnClick, new CUIEventManager.OnUIEventHandler(this.OnRefuseInvitationBtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpCardPos1Click, new CUIEventManager.OnUIEventHandler(this.OnSignUpCardPos1Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpCardSelectPos1, new CUIEventManager.OnUIEventHandler(this.OnSignUpCardSelectPos1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpCardPos2Click, new CUIEventManager.OnUIEventHandler(this.OnSignUpCardPos2Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpCardSelectPos2, new CUIEventManager.OnUIEventHandler(this.OnSignUpCardSelectPos2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListInviteBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSignUpListInviteBtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnSignUpListElementEnabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListSortRankGradeDesc, new CUIEventManager.OnUIEventHandler(this.OnSignUpListSortRankGradeDesc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListSortLevelDesc, new CUIEventManager.OnUIEventHandler(this.OnSignUpListSortLevelDesc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListSortPosDesc, new CUIEventManager.OnUIEventHandler(this.OnSignUpListSortPosDesc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_SignUpListSortStateDesc, new CUIEventManager.OnUIEventHandler(this.OnSignUpListSortStateDesc));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnSignUpCardFormClosed, new CUIEventManager.OnUIEventHandler(this.OnSignUpCardFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnSignUpListFormClosed, new CUIEventManager.OnUIEventHandler(this.OnSignUpListFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnInvitationFormClosed, new CUIEventManager.OnUIEventHandler(this.OnInvitationFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnOnlineInvitationFormClosed, new CUIEventManager.OnUIEventHandler(this.OnOnlineInvitationFormClosed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Send_Guild_Match_Invite_To_Platform_Group, new CUIEventManager.OnUIEventHandler(this.OnSendGuildMatchInviteToPlatFormGroup));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Send_Guild_Match_Invite_Btn_Timer_Up, new CUIEventManager.OnUIEventHandler(this.OnSendGuildMatchInviteToPlatFormGroupTimerUp));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Guild_Get_Guild_Match_Season_Rank", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetGuildMatchSeasonRank));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Guild_Get_Guild_Match_Week_Rank", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetGuildMatchWeekRank));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Leave_Guild", new Action(this.OnLeaveGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this.OnPlayerNameChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", new Action<CGuildSystem.enPlatformGroupStatus, bool>(this.OnPlatformGroupStatusChange));
		}

		public void Clear()
		{
			this.m_isReady = false;
			this.OpenWhenInvitedFromPlatformGroup = false;
			if (this.m_teamInfos != null)
			{
				this.m_teamInfos.Clear();
				this.m_teamInfos = null;
			}
			if (this.m_guildMemberInviteList != null)
			{
				this.m_guildMemberInviteList.Clear();
				this.m_guildMemberInviteList = null;
			}
			if (this.m_guildMemberScoreList != null)
			{
				this.m_guildMemberScoreList.Clear();
				this.m_guildMemberScoreList = null;
			}
			this.m_guildSeasonScores = null;
			this.m_guildWeekScores = null;
			this.m_matchRecords = null;
			this.m_isNeedRequestNewRecord = true;
			if (this.m_signUpInfos != null)
			{
				this.m_signUpInfos.Clear();
				this.m_signUpInfos = null;
			}
			this.m_isSignedUp = false;
			this.m_isHaveNewSignUpInfo = false;
			this.m_signUpPos1 = 0;
			this.m_signUpPos2 = 0;
			this.m_signUpMemoText = null;
			this.m_isSignUpCardBubbleShowed = false;
			this.m_isSignUpListBubbleShowed = false;
			if (this.m_invitationInfos != null)
			{
				this.m_invitationInfos.Clear();
				this.m_invitationInfos = null;
			}
			this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.Default;
			this.m_curViewInvitationInviterUid = 0uL;
			this.m_isHaveNewSignUpInfo = false;
			if (this.m_unhandledOnlineInvitationInviterUids != null)
			{
				this.m_unhandledOnlineInvitationInviterUids.Clear();
				this.m_unhandledOnlineInvitationInviterUids = null;
			}
			if (this.m_openedOnlineInvitationInviterUids != null)
			{
				this.m_openedOnlineInvitationInviterUids.Clear();
				this.m_openedOnlineInvitationInviterUids = null;
			}
		}

		public void CloseAllGuildMatchForm()
		{
			if (this.m_form != null)
			{
				this.m_form.Close();
			}
			if (this.m_matchRecordForm != null)
			{
				this.m_matchRecordForm.Close();
			}
			if (this.m_createSignUpCardForm != null)
			{
				this.m_createSignUpCardForm.Close();
			}
			if (this.m_modifySignUpCardForm != null)
			{
				this.m_modifySignUpCardForm.Close();
			}
			if (this.m_signUpListForm != null)
			{
				this.m_signUpListForm.Close();
			}
			if (this.m_invitationForm != null)
			{
				this.m_invitationForm.Close();
			}
			Singleton<CUIManager>.GetInstance().CloseForm(CGuildMatchSystem.GuildMatchOnlineInvitationFormPath);
		}

		public void CreateGuildMatchAllTeams()
		{
			if (this.m_teamInfos != null)
			{
				this.m_teamInfos.Clear();
				this.m_teamInfos = null;
			}
			this.m_teamInfos = new List<KeyValuePair<ulong, ListView<CGuildMatchSystem.TeamPlayerInfo>>>();
			ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
			if (guildMemberInfos == null)
			{
				return;
			}
			for (int i = 0; i < guildMemberInfos.Count; i++)
			{
				if (guildMemberInfos[i].GuildMatchInfo.ullTeamLeaderUid > 0uL)
				{
					bool flag = false;
					for (int j = 0; j < this.m_teamInfos.get_Count(); j++)
					{
						if (this.m_teamInfos.get_Item(j).get_Key() == guildMemberInfos[i].GuildMatchInfo.ullTeamLeaderUid)
						{
							if (this.IsTeamLeader(guildMemberInfos[i].stBriefInfo.uulUid, this.m_teamInfos.get_Item(j).get_Key()))
							{
								CGuildMatchSystem.TeamPlayerInfo item = this.CreateTeamPlayerInfoObj(guildMemberInfos[i]);
								this.m_teamInfos.get_Item(j).get_Value().Insert(0, item);
							}
							else if (!this.FindAndReplaceEmptyPlayerSlot(this.m_teamInfos.get_Item(j).get_Value(), guildMemberInfos[i]))
							{
								CGuildMatchSystem.TeamPlayerInfo item2 = this.CreateTeamPlayerInfoObj(guildMemberInfos[i]);
								if (this.m_teamInfos.get_Item(j).get_Value().Count < 5)
								{
									this.m_teamInfos.get_Item(j).get_Value().Add(item2);
								}
							}
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.CreateNewTeam(guildMemberInfos[i]);
					}
				}
			}
			this.m_isReady = true;
			if (this.OpenWhenInvitedFromPlatformGroup)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_OpenMatchForm);
				this.OpenWhenInvitedFromPlatformGroup = false;
			}
		}

		private void CreateNewTeam(GuildMemInfo guildMemberInfo)
		{
			if (guildMemberInfo == null || this.m_teamInfos == null)
			{
				return;
			}
			ListView<CGuildMatchSystem.TeamPlayerInfo> listView = new ListView<CGuildMatchSystem.TeamPlayerInfo>();
			listView.Add(this.CreateTeamPlayerInfoObj(guildMemberInfo));
			KeyValuePair<ulong, ListView<CGuildMatchSystem.TeamPlayerInfo>> keyValuePair = new KeyValuePair<ulong, ListView<CGuildMatchSystem.TeamPlayerInfo>>(guildMemberInfo.GuildMatchInfo.ullTeamLeaderUid, listView);
			if (this.IsSameTeamWithSelf(keyValuePair.get_Key()))
			{
				this.m_teamInfos.Insert(0, keyValuePair);
			}
			else
			{
				this.m_teamInfos.Add(keyValuePair);
			}
		}

		private bool IsTeamLeader(ulong playerUid, ulong teamLeaderUid)
		{
			return playerUid == teamLeaderUid;
		}

		private bool IsSameTeamWithSelf(ulong playerTeamLeaderUid)
		{
			return playerTeamLeaderUid == CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.ullTeamLeaderUid;
		}

		public bool IsSelfBelongedTeamLeader()
		{
			return CGuildHelper.GetPlayerGuildMemberInfo() != null && Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID == CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.ullTeamLeaderUid;
		}

		public bool IsSelfInAnyTeam()
		{
			ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			if (this.m_teamInfos != null)
			{
				for (int i = 0; i < this.m_teamInfos.get_Count(); i++)
				{
					for (int j = 0; j < this.m_teamInfos.get_Item(i).get_Value().Count; j++)
					{
						if (this.m_teamInfos.get_Item(i).get_Value()[j].Uid == playerUllUID)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsReadyForGame(ListView<CGuildMatchSystem.TeamPlayerInfo> teamPlayerInfos, ulong playerUid)
		{
			if (teamPlayerInfos != null)
			{
				for (int i = 0; i < teamPlayerInfos.Count; i++)
				{
					if (teamPlayerInfos[i].Uid == playerUid)
					{
						return teamPlayerInfos[i].IsReady;
					}
				}
			}
			return false;
		}

		private bool IsTeamAllPlayerReadyForGame(ListView<CGuildMatchSystem.TeamPlayerInfo> teamPlayerInfos, ulong teamLeaderUid)
		{
			if (teamPlayerInfos == null)
			{
				return false;
			}
			if (teamPlayerInfos.Count < 5)
			{
				return false;
			}
			for (int i = 0; i < 5; i++)
			{
				if (teamPlayerInfos[i].Uid == 0uL || (!teamPlayerInfos[i].IsReady && !this.IsTeamLeader(teamPlayerInfos[i].Uid, teamLeaderUid)))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsInTeam(ulong memberTeamLeaderUid, ulong teamLeaderUid)
		{
			return memberTeamLeaderUid == teamLeaderUid && memberTeamLeaderUid != 0uL;
		}

		public bool IsInAnyTeam(ulong memberUid)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(memberUid);
			return guildMemberInfoByUid != null && guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid > 0uL;
		}

		private void AddMemberToTeam(ulong memberNewTeamLeaderUid, GuildMemInfo memberInfo)
		{
			if (this.m_teamInfos == null || memberInfo == null)
			{
				return;
			}
			for (int i = 0; i < this.m_teamInfos.get_Count(); i++)
			{
				if (memberNewTeamLeaderUid == this.m_teamInfos.get_Item(i).get_Key())
				{
					if (!this.FindAndReplaceEmptyPlayerSlot(this.m_teamInfos.get_Item(i).get_Value(), memberInfo))
					{
						CGuildMatchSystem.TeamPlayerInfo item = this.CreateTeamPlayerInfoObj(memberInfo);
						if (this.m_teamInfos.get_Item(i).get_Value().Count < 5)
						{
							this.m_teamInfos.get_Item(i).get_Value().Add(item);
						}
					}
					if (CGuildHelper.IsSelf(memberInfo.stBriefInfo.uulUid) && i != 0)
					{
						KeyValuePair<ulong, ListView<CGuildMatchSystem.TeamPlayerInfo>> keyValuePair = this.m_teamInfos.get_Item(i);
						this.m_teamInfos.RemoveAt(i);
						this.m_teamInfos.Insert(0, keyValuePair);
					}
					return;
				}
			}
		}

		private bool FindAndReplaceEmptyPlayerSlot(ListView<CGuildMatchSystem.TeamPlayerInfo> teamPlayerInfos, GuildMemInfo memberInfo)
		{
			for (int i = 0; i < teamPlayerInfos.Count; i++)
			{
				if (teamPlayerInfos[i].Uid == 0uL)
				{
					teamPlayerInfos[i].Uid = memberInfo.stBriefInfo.uulUid;
					teamPlayerInfos[i].Name = memberInfo.stBriefInfo.sName;
					teamPlayerInfos[i].HeadUrl = memberInfo.stBriefInfo.szHeadUrl;
					teamPlayerInfos[i].IsReady = Convert.ToBoolean(memberInfo.GuildMatchInfo.bIsReady);
					return true;
				}
			}
			return false;
		}

		public void SetTeamInfo(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF ntf)
		{
			if (this.m_teamInfos == null)
			{
				DebugHelper.Assert(false, "m_teamInfos is null!!!");
				return;
			}
			for (int i = 0; i < (int)ntf.bCnt; i++)
			{
				ulong ullUid = ntf.astChgInfo[i].ullUid;
				ulong ullTeamLeaderUid = ntf.astChgInfo[i].ullTeamLeaderUid;
				this.RemoveMemberFromOldTeam(ullUid);
				if (ullUid == ullTeamLeaderUid)
				{
					this.CreateNewTeam(CGuildHelper.GetGuildMemberInfoByUid(ullUid));
				}
				else if (ullTeamLeaderUid > 0uL)
				{
					this.AddMemberToTeam(ullTeamLeaderUid, CGuildHelper.GetGuildMemberInfoByUid(ullUid));
				}
			}
		}

		private void RemoveMemberFromOldTeam(ulong memberUid)
		{
			if (this.m_teamInfos == null)
			{
				return;
			}
			for (int i = this.m_teamInfos.get_Count() - 1; i >= 0; i--)
			{
				if (this.IsTeamLeader(memberUid, this.m_teamInfos.get_Item(i).get_Key()))
				{
					this.m_teamInfos.RemoveAt(i);
					this.PromptKickedFromTeamTip(memberUid);
				}
				else
				{
					for (int j = 0; j < this.m_teamInfos.get_Item(i).get_Value().Count; j++)
					{
						if (memberUid == this.m_teamInfos.get_Item(i).get_Value()[j].Uid)
						{
							this.RemoveMemberFromTeam(this.m_teamInfos.get_Item(i).get_Value()[j]);
							this.PromptKickedFromTeamTip(memberUid);
						}
					}
				}
			}
		}

		private void PromptKickedFromTeamTip(ulong memberUid)
		{
			if (CGuildHelper.IsSelf(memberUid) && Utility.IsCanShowPrompt())
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Kicked_Tip"), true, 1.5f, null, new object[0]);
			}
		}

		public void SetTeamInfo(SCPKG_CHG_GUILD_MATCH_LEADER_NTF ntf)
		{
			if (this.m_teamInfos == null)
			{
				DebugHelper.Assert(false, "m_teamInfos is null!!!");
				return;
			}
			if (ntf.ullTeamLeaderUid > 0uL)
			{
				for (int i = 0; i < this.m_teamInfos.get_Count(); i++)
				{
					if (this.IsTeamLeader(ntf.ullTeamLeaderUid, this.m_teamInfos.get_Item(i).get_Key()))
					{
						return;
					}
				}
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(ntf.ullUid);
				this.CreateNewTeam(guildMemberInfoByUid);
			}
			else
			{
				for (int j = 0; j < this.m_teamInfos.get_Count(); j++)
				{
					if (this.IsTeamLeader(ntf.ullUid, this.m_teamInfos.get_Item(j).get_Key()))
					{
						this.m_teamInfos.RemoveAt(j);
						break;
					}
				}
			}
		}

		public void SetTeamMemberReadyState(SCPKG_SET_GUILD_MATCH_READY_NTF ntf)
		{
			if (this.m_teamInfos == null)
			{
				DebugHelper.Assert(false, "m_teamInfos is null!!!");
				return;
			}
			bool flag = false;
			for (int i = 0; i < (int)ntf.bCnt; i++)
			{
				for (int j = 0; j < this.m_teamInfos.get_Count(); j++)
				{
					for (int k = 0; k < this.m_teamInfos.get_Item(j).get_Value().Count; k++)
					{
						if (this.m_teamInfos.get_Item(j).get_Value()[k].Uid == ntf.astInfo[i].ullUid)
						{
							this.m_teamInfos.get_Item(j).get_Value()[k].IsReady = Convert.ToBoolean(ntf.astInfo[i].bIsReady);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}

		public void SetTeamMemberReadyState(ulong teamMemberUid, bool isReady)
		{
			if (this.m_teamInfos == null)
			{
				DebugHelper.Assert(false, "m_teamInfos is null!!!");
				return;
			}
			for (int i = 0; i < this.m_teamInfos.get_Count(); i++)
			{
				for (int j = 0; j < this.m_teamInfos.get_Item(i).get_Value().Count; j++)
				{
					if (this.m_teamInfos.get_Item(i).get_Value()[j].Uid == teamMemberUid)
					{
						this.m_teamInfos.get_Item(i).get_Value()[j].IsReady = isReady;
						return;
					}
				}
			}
		}

		private void RemoveMemberFromTeam(CGuildMatchSystem.TeamPlayerInfo teamPlayerInfo)
		{
			teamPlayerInfo.Uid = 0uL;
			teamPlayerInfo.Name = string.Empty;
			teamPlayerInfo.HeadUrl = string.Empty;
			teamPlayerInfo.IsReady = false;
		}

		private void OnMatchFormOpened(CUIEvent uiEvent)
		{
			CChatUT.EnterGuildMatch();
		}

		private void OnMatchFormClosed(CUIEvent uiEvent)
		{
			CChatUT.LeaveGuildMatch();
			this.m_form = null;
			Singleton<CGuildInfoView>.GetInstance().RefreshGuildMatchPanel();
		}

		private void OnOpenMatchForm(CUIEvent uiEvent)
		{
			Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(11u, null, false);
			PlayerPrefs.SetInt("GuildMatch_GuildMatchBtnClicked", 1);
			Singleton<CGuildInfoView>.GetInstance().DelGuildMatchBtnNewFlag();
			this.OpenMatchForm(false);
		}

		private void OnOpenMatchRecordForm(CUIEvent uiEvent)
		{
			if (this.m_isNeedRequestNewRecord)
			{
				this.m_isNeedRequestNewRecord = false;
				Singleton<CTimerManager>.GetInstance().AddTimer(300000, 1, new CTimer.OnTimeUpHandler(this.OnRequestNewRecordTimeout));
				this.RequestGetGuildMatchHistory();
			}
			else
			{
				this.OpenMatchRecordForm();
			}
		}

		private void OnRequestNewRecordTimeout(int timerSequence)
		{
			this.m_isNeedRequestNewRecord = true;
		}

		private void OnStartGame(CUIEvent uiEvent)
		{
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			if (CGuildHelper.IsGuildMatchReachMatchCntLimit(playerGuildMemberInfo))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(this.GetReachMatchCntLimitTip(), false, 1.5f, null, new object[0]);
				return;
			}
			this.RequestStartGuildMatch();
		}

		private void OnOBGame(CUIEvent uiEvent)
		{
			this.RequestObGuildMatch(uiEvent.m_eventParams.commonUInt64Param1);
		}

		private void OnReadyGame(CUIEvent uiEvent)
		{
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			if (CGuildHelper.IsGuildMatchReachMatchCntLimit(playerGuildMemberInfo))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(this.GetReachMatchCntLimitTip(), false, 1.5f, null, new object[0]);
				return;
			}
			if (!this.IsInGuildMatchTime())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Not_Open", true, 1.5f, null, new object[0]);
			}
			this.RequestSetGuildMatchReady(true);
		}

		private void OpenMatchForm(bool isReadyGame = false)
		{
			this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchFormPath, false, true);
			this.RefreshGuildMatchForm();
			if (this.IsSelfBelongedTeamLeader())
			{
				uint guildMatchNextEndTime = CUICommonSystem.GetGuildMatchNextEndTime();
				PlayerPrefs.SetInt("GuildMatch_GuildMatchEndTimeRecrod", (int)guildMatchNextEndTime);
			}
			this.RequestGetGuildMatchSeasonRank();
			this.RequestGetGuildMatchWeekRank();
			if (!this.IsSelfBelongedTeamLeader())
			{
				this.RequestGuildMatchGetInvitation();
			}
			this.RequestGetGuildMatchSignUpList();
			if (isReadyGame)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_ReadyGame);
			}
		}

		private void OpenMatchRecordForm()
		{
			this.m_matchRecordForm = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchRecordFormPath, false, true);
			this.RefreshMatchRecordForm();
		}

		private void OnMatchRecordFormClosed(CUIEvent uiEvent)
		{
			this.m_matchRecordForm = null;
		}

		private string GetReachMatchCntLimitTip()
		{
			return Singleton<CTextManager>.GetInstance().GetText(CGuildHelper.IsGuildMatchLeaderPosition(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) ? "GuildMatch_Leader_Match_Times_Reach_Limit" : "GuildMatch_Normal_Player_Match_Times_Reach_Limit");
		}

		private void OnCancelReadyGame(CUIEvent uiEvent)
		{
			this.RequestSetGuildMatchReady(false);
		}

		private void OnRankTabChanged(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = srcFormScript.GetWidget(4).GetComponent<CUIListScript>();
			GameObject widget = srcFormScript.GetWidget(6);
			GameObject widget2 = srcFormScript.GetWidget(7);
			GameObject widget3 = srcFormScript.GetWidget(28);
			GameObject widget4 = srcFormScript.GetWidget(8);
			GameObject widget5 = srcFormScript.GetWidget(29);
			GameObject widget6 = srcFormScript.GetWidget(27);
			GameObject widget7 = srcFormScript.GetWidget(13);
			GameObject widget8 = srcFormScript.GetWidget(9);
			GameObject widget9 = srcFormScript.GetWidget(18);
			switch (component.GetSelectedIndex())
			{
			case 0:
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget6.CustomSetActive(false);
				widget7.CustomSetActive(true);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(true);
				this.RefreshGuildScoreRankList();
				this.SetSelfGuildScorePanel();
				this.RefreshSubRankPanel(CGuildMatchSystem.enRankTab.GuildScore);
				break;
			case 1:
				widget2.CustomSetActive(true);
				widget.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget6.CustomSetActive(false);
				widget7.CustomSetActive(false);
				widget8.CustomSetActive(true);
				widget9.CustomSetActive(true);
				this.RefreshMemberScoreRankList();
				this.SetMemberScoreListElement(widget8, this.GetSelfIndexInGuildMemberScoreList());
				this.RefreshSubRankPanel(CGuildMatchSystem.enRankTab.MemberScore);
				break;
			case 2:
				if (this.IsSelfBelongedTeamLeader())
				{
					widget3.CustomSetActive(true);
					widget4.CustomSetActive(true);
					widget5.CustomSetActive(false);
					if (Singleton<CGuildSystem>.GetInstance().IsOpenPlatformGroupFunc())
					{
						CGuildModel model = Singleton<CGuildSystem>.GetInstance().Model;
						if (model.PlatformGroupStatus == CGuildSystem.enPlatformGroupStatus.Resolve)
						{
							Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_PlatformGroup_Refresh_Group_Panel");
						}
						else
						{
							Singleton<EventRouter>.GetInstance().BroadCastEvent<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", model.PlatformGroupStatus, model.IsSelfInPlatformGroup);
						}
					}
					widget6.CustomSetActive(false);
				}
				else
				{
					widget3.CustomSetActive(false);
					widget6.CustomSetActive(true);
					this.RequestGuildMatchGetInvitation();
					PlayerPrefs.SetInt("GuildMatch_InvitationTabClicked", 1);
				}
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget7.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				break;
			}
		}

		private void RefreshGuildScoreRankList()
		{
			if (this.m_form == null)
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(6).GetComponent<CUIListScript>();
			CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
			int elementAmount = (guildScoreRankInfo == null) ? 0 : guildScoreRankInfo.Length;
			component.SetElementAmount(elementAmount);
		}

		private void RefreshMemberScoreRankList()
		{
			if (this.m_form == null)
			{
				return;
			}
			if (this.m_guildMemberScoreList == null)
			{
				this.m_guildMemberScoreList = new ListView<GuildMemInfo>();
			}
			this.m_guildMemberScoreList.Clear();
			ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
			this.m_guildMemberScoreList.AddRange(guildMemberInfos);
			if (this.IsCurrentSeasonScoreTab())
			{
				this.m_guildMemberScoreList.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberSeasonScoreList));
			}
			else
			{
				this.m_guildMemberScoreList.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberWeekScoreList));
			}
			CUIListScript component = this.m_form.GetWidget(7).GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_guildMemberScoreList.Count);
		}

		private int SortGuildMemberSeasonScoreList(GuildMemInfo info1, GuildMemInfo info2)
		{
			return -info1.GuildMatchInfo.dwScore.CompareTo(info2.GuildMatchInfo.dwScore);
		}

		private int SortGuildMemberWeekScoreList(GuildMemInfo info1, GuildMemInfo info2)
		{
			return -info1.GuildMatchInfo.dwWeekScore.CompareTo(info2.GuildMatchInfo.dwScore);
		}

		private bool IsCurrentSeasonScoreTab()
		{
			if (this.m_form == null)
			{
				return false;
			}
			Slider component = this.m_form.GetWidget(21).GetComponent<Slider>();
			return (int)component.get_value() == 0;
		}

		private void InitMemberInviteList()
		{
			if (this.m_form == null)
			{
				return;
			}
			this.m_guildMemberInviteList = Singleton<CInviteSystem>.GetInstance().CreateGuildMemberInviteList();
			this.m_guildMemberInviteList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
			Singleton<CInviteSystem>.GetInstance().SendGetGuildMemberGameStateReq();
			CUITimerScript component = this.m_form.GetWidget(11).GetComponent<CUITimerScript>();
			Singleton<CInviteSystem>.GetInstance().SetAndStartRefreshGuildMemberGameStateTimer(component);
		}

		public void RefreshGuildMatchGuildMemberInvitePanel()
		{
			if (this.m_guildMemberInviteList != null)
			{
				this.m_guildMemberInviteList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
				this.SetInviteGuildMemberData();
			}
		}

		public void SetInviteGuildMemberData()
		{
			if (this.m_form == null)
			{
				return;
			}
			ListView<GuildMemInfo> guildMemberInviteList = this.GetGuildMemberInviteList();
			if (guildMemberInviteList == null)
			{
				return;
			}
			int count = guildMemberInviteList.Count;
			int num = 0;
			this.RefreshInviteGuildMemberList(count);
			for (int i = 0; i < count; i++)
			{
				if (CGuildHelper.IsMemberOnline(guildMemberInviteList[i]))
				{
					num++;
				}
			}
			Text component = this.m_form.GetWidget(5).GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online_Member", new string[]
			{
				num.ToString(),
				count.ToString()
			}));
		}

		public void RefreshInviteGuildMemberList(int allGuildMemberLen)
		{
			if (this.m_form == null)
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(8).GetComponent<CUIListScript>();
			component.SetElementAmount(allGuildMemberLen);
		}

		private void RefreshInvitationList()
		{
			if (this.IsSelfBelongedTeamLeader())
			{
				return;
			}
			if (this.m_form == null || this.m_invitationInfos == null)
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(4).GetComponent<CUIListScript>();
			CUIListElementScript elemenet = component.GetElemenet(2);
			if (elemenet != null)
			{
				GameObject gameObject = elemenet.gameObject;
				if (CGuildHelper.IsGuildMatchFormInvitationTabShowRedDot())
				{
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(gameObject);
				}
			}
			CUIListScript component2 = this.m_form.GetWidget(27).GetComponent<CUIListScript>();
			int count = this.m_invitationInfos.get_Count();
			component2.SetElementAmount(count);
			for (int i = 0; i < count; i++)
			{
				Transform transform = component2.GetElemenet(i).transform;
				CUIHttpImageScript component3 = transform.Find("HeadBg/imgHead").GetComponent<CUIHttpImageScript>();
				Image component4 = transform.Find("NobeIcon").GetComponent<Image>();
				Image component5 = transform.Find("HeadBg/NobeImag").GetComponent<Image>();
				Text component6 = transform.Find("PlayerName").GetComponent<Text>();
				Text component7 = transform.Find("StateText").GetComponent<Text>();
				GameObject gameObject2 = transform.Find("ViewButton").gameObject;
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(this.m_invitationInfos.get_Item(i).invitationInfo.ullUid);
				if (guildMemberInfoByUid != null)
				{
					component3.SetImageUrl(CGuildHelper.GetHeadUrl(guildMemberInfoByUid.stBriefInfo.szHeadUrl));
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component4, CGuildHelper.GetNobeLevel(guildMemberInfoByUid.stBriefInfo.uulUid, guildMemberInfoByUid.stBriefInfo.stVip.level), false, true, 0uL);
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component5, CGuildHelper.GetNobeHeadIconId(guildMemberInfoByUid.stBriefInfo.uulUid, guildMemberInfoByUid.stBriefInfo.stVip.headIconId));
					component6.set_text(guildMemberInfoByUid.stBriefInfo.sName);
					component7.set_text(this.GetInvitationStateText((COM_GUILDMATCH_INVITECARD_STATE)this.m_invitationInfos.get_Item(i).invitationInfo.bState, this.m_invitationInfos.get_Item(i).isValid));
					this.SetInvitationViewBtnRedDot(gameObject2, (COM_GUILDMATCH_INVITECARD_STATE)this.m_invitationInfos.get_Item(i).invitationInfo.bState, this.m_invitationInfos.get_Item(i).isValid);
				}
			}
		}

		private void SetInvitationViewBtnRedDot(GameObject viewBtnGo, COM_GUILDMATCH_INVITECARD_STATE state, bool isInvitationValid)
		{
			if (isInvitationValid && state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_NULL)
			{
				CUICommonSystem.AddRedDot(viewBtnGo, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(viewBtnGo);
			}
		}

		private string GetInvitationStateText(COM_GUILDMATCH_INVITECARD_STATE state, bool isValid)
		{
			string result = string.Empty;
			if (!isValid)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Expired");
			}
			else if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_NULL)
			{
				result = "<color=#e49316>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Wait_For_Response") + "</color>";
			}
			else if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_ACCEPT)
			{
				result = "<color=#00ff00>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Accepted") + "</color>";
			}
			else if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_REFUSE)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Rejected");
			}
			return result;
		}

		private void OnGuildScoreListElementEnabled(CUIEvent uiEvent)
		{
			this.SetGuildScoreListElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
		}

		private void OnMemberScoreListElementEnabled(CUIEvent uiEvent)
		{
			this.SetMemberScoreListElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
		}

		private void SetGuildScoreListElement(GameObject listElementGo, int guildScoreListIndex)
		{
			if (listElementGo == null)
			{
				return;
			}
			CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
			if (guildScoreRankInfo == null)
			{
				return;
			}
			if (guildScoreListIndex < 0 || guildScoreListIndex >= guildScoreRankInfo.Length)
			{
				DebugHelper.Assert(false, "guildScoreListIndex out of range: " + guildScoreListIndex);
				return;
			}
			CSDT_RANKING_LIST_ITEM_INFO cSDT_RANKING_LIST_ITEM_INFO = guildScoreRankInfo[guildScoreListIndex];
			if (cSDT_RANKING_LIST_ITEM_INFO == null)
			{
				return;
			}
			Transform transform = listElementGo.transform;
			Transform rankTransform = transform.Find("rank");
			Image component = transform.Find("imgHeadBg/imgHead").GetComponent<Image>();
			Text component2 = transform.Find("txtName").GetComponent<Text>();
			Text component3 = transform.Find("txtScore").GetComponent<Text>();
			CUICommonSystem.SetRankDisplay((uint)(guildScoreListIndex + 1), rankTransform);
			component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stGuildMatch.dwGuildHeadID, this.m_form, true, false, false, false);
			component2.set_text(StringHelper.UTF8BytesToString(ref cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stGuildMatch.szGuildName));
			component3.set_text(cSDT_RANKING_LIST_ITEM_INFO.dwRankScore.ToString());
		}

		private void SetSelfGuildScorePanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			Transform transform = this.m_form.GetWidget(14).transform;
			Image component = this.m_form.GetWidget(15).GetComponent<Image>();
			Text component2 = this.m_form.GetWidget(16).GetComponent<Text>();
			Text component3 = this.m_form.GetWidget(17).GetComponent<Text>();
			CUICommonSystem.SetRankDisplay(this.GetSelfGuildScoreRankNo(), transform);
			component.SetSprite(CGuildHelper.GetGuildHeadPath(), this.m_form, true, false, false, false);
			component2.set_text(CGuildHelper.GetGuildName());
			component3.set_text(this.IsCurrentSeasonScoreTab() ? CGuildHelper.GetGuildMatchSeasonScore().ToString() : CGuildHelper.GetGuildMatchWeekScore().ToString());
		}

		private void RefreshSubRankPanel(CGuildMatchSystem.enRankTab rankTab)
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(19).GetComponent<Text>();
			if (rankTab == CGuildMatchSystem.enRankTab.GuildScore)
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Guild_Score_Rank"));
			}
			else if (rankTab == CGuildMatchSystem.enRankTab.MemberScore)
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Member_Score_Rank"));
			}
		}

		private uint GetSelfGuildScoreRankNo()
		{
			CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
			if (guildScoreRankInfo != null)
			{
				ulong guildUid = CGuildHelper.GetGuildUid();
				for (int i = 0; i < guildScoreRankInfo.Length; i++)
				{
					if (guildUid == ulong.Parse(StringHelper.UTF8BytesToString(ref guildScoreRankInfo[i].szOpenID)))
					{
						return guildScoreRankInfo[i].dwRankNo;
					}
				}
			}
			return 0u;
		}

		private void SetMemberScoreListElement(GameObject listElementGo, int guildMemberScoreListIndex)
		{
			if (listElementGo == null || this.m_guildMemberScoreList == null)
			{
				return;
			}
			if (guildMemberScoreListIndex < 0 || guildMemberScoreListIndex >= this.m_guildMemberScoreList.Count)
			{
				DebugHelper.Assert(false, "guildMemberScoreListIndex out of range: " + guildMemberScoreListIndex);
				return;
			}
			GuildMemInfo guildMemInfo = this.m_guildMemberScoreList[guildMemberScoreListIndex];
			if (guildMemInfo == null)
			{
				return;
			}
			Transform transform = listElementGo.transform;
			Transform rankTransform = transform.Find("rank");
			CUIHttpImageScript component = transform.Find("imgHeadBg/imgHead").GetComponent<CUIHttpImageScript>();
			Image component2 = transform.Find("NobeIcon").GetComponent<Image>();
			Image component3 = transform.Find("imgHeadBg/NobeImag").GetComponent<Image>();
			Text component4 = transform.Find("txtName").GetComponent<Text>();
			Text component5 = transform.Find("txtScore").GetComponent<Text>();
			CUICommonSystem.SetRankDisplay((uint)(guildMemberScoreListIndex + 1), rankTransform);
			component.SetImageUrl(CGuildHelper.GetHeadUrl(guildMemInfo.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(guildMemInfo.stBriefInfo.uulUid, guildMemInfo.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(guildMemInfo.stBriefInfo.uulUid, guildMemInfo.stBriefInfo.stVip.headIconId));
			component4.set_text(guildMemInfo.stBriefInfo.sName);
			component5.set_text(this.IsCurrentSeasonScoreTab() ? guildMemInfo.GuildMatchInfo.dwScore.ToString() : guildMemInfo.GuildMatchInfo.dwWeekScore.ToString());
		}

		private void OnMemberInviteListElementEnabled(CUIEvent uiEvent)
		{
			if (this.m_guildMemberInviteList == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_guildMemberInviteList.Count)
			{
				CInviteView.UpdateGuildMemberListElement(srcWidget, this.m_guildMemberInviteList[srcWidgetIndexInBelongedList], true);
			}
		}

		private void OnInvite(CUIEvent uiEvent)
		{
			if (this.IsTeamFull(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Team_Member_Full", true, 1.5f, null, new object[0]);
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_guildMemberInviteList.Count)
			{
				DebugHelper.Assert(false, "error guild match invite list element index {0}", new object[]
				{
					srcWidgetIndexInBelongedList
				});
				return;
			}
			ulong uulUid = this.m_guildMemberInviteList[srcWidgetIndexInBelongedList].stBriefInfo.uulUid;
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(uulUid);
			if (CGuildMatchSystem.IsInGuildMatchJoinLimitTime(guildMemberInfoByUid))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Member_Join_Time_Limit_Join_Match_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			string empty = string.Empty;
			if (this.IsMemberInOtherTeam(uulUid, ref empty))
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Member_Already_In_Other_Team", new string[]
				{
					empty
				});
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_InviteConfirm, enUIEventID.None, new stUIEventParams
				{
					commonUInt64Param1 = uulUid,
					commonGameObject = uiEvent.m_srcWidget
				}, false);
				return;
			}
			this.RealInvite(uulUid, uiEvent.m_srcWidget);
		}

		private void OnInviteConfirm(CUIEvent uiEvent)
		{
			this.RealInvite(uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonGameObject);
		}

		private int GetSendGuildMatchInviteLeftTime()
		{
			int num = 0;
			ResGuildMisc dataByKey = GameDataMgr.guildMiscDatabin.GetDataByKey(63u);
			if (dataByKey != null && dataByKey.dwConfValue > 0u)
			{
				num = (int)dataByKey.dwConfValue;
			}
			int @int = PlayerPrefs.GetInt("SEND_INVITE_TO_PLATFORM_GROUP_INTERVAL_KEY", 0);
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			int num2 = currentUTCTime - @int;
			return num - num2;
		}

		private void OnSendGuildMatchInviteToPlatFormGroup(CUIEvent uiEvent)
		{
			int sendGuildMatchInviteLeftTime = this.GetSendGuildMatchInviteLeftTime();
			if (sendGuildMatchInviteLeftTime > 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_WXGroup_Send_Guild_Match_Invite_Err_Frequency", true, 1.5f, null, new object[0]);
			}
			else
			{
				this.SendInviteToPlatformGroup();
				PlayerPrefs.SetInt("SEND_INVITE_TO_PLATFORM_GROUP_INTERVAL_KEY", CRoleInfo.GetCurrentUTCTime());
				PlayerPrefs.Save();
				this.RefreshPlatformGroupInviteBtn();
			}
		}

		private void OnSendGuildMatchInviteToPlatFormGroupTimerUp(CUIEvent uiEvent)
		{
			this.RefreshPlatformGroupInviteBtn();
		}

		public void SendInviteToPlatformGroup()
		{
			Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
			if (!MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master RoleInfo is null");
				return;
			}
			IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
			if (iApolloSnsService == null)
			{
				DebugHelper.Assert(false, "IApollo.Instance.GetService(ApolloServiceType.Sns) == null");
				return;
			}
			MonoSingleton<ShareSys>.instance.OnShareCallBack();
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Guild_Match_Invite_Title");
			string text2 = Singleton<CTextManager>.GetInstance().GetText("Guild_WXGroup_Guild_Match_Invite_Memo");
			string text3 = string.Format("{0}_{1}_{2}", "GuildMatchInvite", (int)Singleton<ApolloHelper>.GetInstance().CurPlatform, masterRoleInfo.m_baseGuildInfo.uulUid);
			string imgUrl = "http://image.smoba.qq.com/yywj/share_pic/120.png";
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
				if (string.IsNullOrEmpty(Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.groupKey))
				{
					Singleton<CGuildInfoView>.GetInstance().QueryQQGroupKey();
					Singleton<CUIManager>.GetInstance().OpenTips(string.Format("{0}{1}", Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Group_UnBind_Err_Sys"), " groupKey is null"), false, 1.5f, null, new object[0]);
				}
				string groupKey = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.groupKey;
				if (accountInfo != null)
				{
					string targetUrl = string.Concat(new string[]
					{
						"http://gamecenter.qq.com/gcjump?appid=1104466820&pf=invite&from=androidqq&plat=qq&originuin=",
						accountInfo.OpenId,
						"&ADTAG=gameobj.msg_invite&",
						ApolloHelper.QQ_SHARE_GAMEDATA,
						"=",
						text3
					});
					iApolloSnsService.SendToQQGameFriend(1, groupKey, text, text2, targetUrl, imgUrl, text, "MSG_INVITE", ShareSys.SNS_GUILD_MATCH_INVITE);
				}
			}
			else
			{
				iApolloSnsService.SendToWXGroup(1, 1, Singleton<CGuildInfoView>.GetInstance().GetWxGroupUnionId(false), text, text2, text3, "MSG_INVITE", imgUrl, ShareSys.SNS_GUILD_MATCH_INVITE);
			}
		}

		private void RealInvite(ulong invitedMemberUid, GameObject inviteBtnGo)
		{
			this.RequestInviteGuildMatchMember(invitedMemberUid);
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(invitedMemberUid);
			if (guildMemberInfoByUid != null && !CGuildHelper.IsMemberOnline(guildMemberInfoByUid))
			{
				guildMemberInfoByUid.isGuildMatchOfflineInvitedByHostPlayer = true;
			}
			this.SetInvitedRelatedWidgets(inviteBtnGo, inviteBtnGo.transform.parent.Find("TeamState").GetComponent<Text>());
		}

		private void SetInvitedRelatedWidgets(GameObject inviteButtonGo, Text teamStateText)
		{
			inviteButtonGo.CustomSetActive(false);
			teamStateText.gameObject.CustomSetActive(true);
			teamStateText.set_text("<color=#e49316>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Wait_For_Response") + "</color>");
		}

		private bool IsMemberInOtherTeam(ulong invitedMemberUid, ref string teamName)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(invitedMemberUid);
			if (guildMemberInfoByUid != null && guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid > 0uL && guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
			{
				teamName = this.GetTeamName(guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid);
				return true;
			}
			return false;
		}

		private void OnKick(CUIEvent uiEvent)
		{
			stUIEventParams par = default(stUIEventParams);
			par.commonUInt64Param1 = uiEvent.m_eventParams.commonUInt64Param1;
			par.commonBool = uiEvent.m_eventParams.commonBool;
			bool commonBool = uiEvent.m_eventParams.commonBool;
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText(commonBool ? "GuildMatch_Leave_Team_Confirm_Msg" : "GuildMatch_Kick_Confirm_Msg"), enUIEventID.Guild_Match_KickConfirm, enUIEventID.None, par, false);
		}

		private void OnKickConfirm(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			bool commonBool = uiEvent.m_eventParams.commonBool;
			if (commonBool)
			{
				this.RequestLeaveGuildMatchTeam();
			}
			else
			{
				this.RequestKickGuildMatchMember(commonUInt64Param);
			}
		}

		private void OnAppointOrCancelLeader(CUIEvent uiEvent)
		{
			if (!CGuildSystem.HasAppointMatchLeaderAuthority())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_You_Have_No_Authority", true, 1.5f, null, new object[0]);
				return;
			}
			GuildMemInfo currentSelectedMemberInfo = Singleton<CGuildModel>.GetInstance().CurrentSelectedMemberInfo;
			if (currentSelectedMemberInfo == null)
			{
				DebugHelper.Assert(false, "guildMemInfo is null!!!");
				return;
			}
			if (!CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo))
			{
				if (this.IsLeaderNumFull())
				{
					Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Leader_Full", true, 1.5f, null, new object[0]);
					return;
				}
				if (CGuildMatchSystem.IsInGuildMatchJoinLimitTime(currentSelectedMemberInfo))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Member_Join_Time_Limit_Appoint_Leader_Tip", true, 1.5f, null, new object[0]);
					return;
				}
			}
			string text = Singleton<CTextManager>.GetInstance().GetText(CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo) ? "GuildMatch_Cancel_Leader_Confirm" : "GuildMatch_Apooint_Leader_Confirm", new string[]
			{
				currentSelectedMemberInfo.stBriefInfo.sName
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_AppointOrCancelLeaderConfirm, enUIEventID.None, false);
		}

		private void OnAppointOrCancelLeaderConfirm(CUIEvent uiEvent)
		{
			GuildMemInfo currentSelectedMemberInfo = Singleton<CGuildModel>.GetInstance().CurrentSelectedMemberInfo;
			if (currentSelectedMemberInfo == null)
			{
				DebugHelper.Assert(false, "guildMemInfo is null!!!");
				return;
			}
			this.RequestChangeGuildMatchLeader(currentSelectedMemberInfo.stBriefInfo.uulUid, !CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo));
		}

		private void OnAcceptInvite(CUIEvent uiEvent)
		{
			this.RequestDealGuildMatchMemberInvite(uiEvent.m_eventParams.commonUInt64Param1, true);
		}

		private void OnRefuseInvite(CUIEvent uiEvent)
		{
			bool commonBool = uiEvent.m_eventParams.commonBool;
			int tag = uiEvent.m_eventParams.tag;
			if (commonBool)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(tag);
			}
			this.RequestDealGuildMatchMemberInvite(uiEvent.m_eventParams.commonUInt64Param1, false);
		}

		private void OnRefreshGameStateTimeout(CUIEvent uiEvent)
		{
			this.RequestGetGuildMemberGameState();
		}

		private void OnObWaitingTimeout(CUIEvent uiEvent)
		{
			Transform parent = uiEvent.m_srcWidget.transform.parent;
			Button component = parent.GetComponent<Button>();
			CUICommonSystem.SetButtonEnable(component, true, true, true);
		}

		private void OnRecordListElementEnabled(CUIEvent uiEvent)
		{
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidget == null || this.m_matchRecords == null)
			{
				return;
			}
			if (uiEvent.m_srcWidgetIndexInBelongedList < 0 || uiEvent.m_srcWidgetIndexInBelongedList >= this.m_matchRecords.Length)
			{
				return;
			}
			Transform transform = srcWidget.transform;
			COMDT_GUILD_MATCH_HISTORY_INFO cOMDT_GUILD_MATCH_HISTORY_INFO = this.m_matchRecords[this.m_matchRecords.Length - 1 - uiEvent.m_srcWidgetIndexInBelongedList];
			if (cOMDT_GUILD_MATCH_HISTORY_INFO == null)
			{
				return;
			}
			Text component = transform.Find("txtMatchTime").GetComponent<Text>();
			component.set_text(Utility.GetUtcToLocalTimeStringFormat((ulong)cOMDT_GUILD_MATCH_HISTORY_INFO.dwMatchTime, Singleton<CTextManager>.GetInstance().GetText("Common_DateTime_Format2")));
			CUIListScript component2 = transform.Find("MatchMemberList").GetComponent<CUIListScript>();
			component2.SetElementAmount(5);
			int num = 1;
			for (int i = 0; i < (int)cOMDT_GUILD_MATCH_HISTORY_INFO.bMemNum; i++)
			{
				CUIListElementScript elemenet;
				if (Convert.ToBoolean(cOMDT_GUILD_MATCH_HISTORY_INFO.astMemInfo[i].bIsTeamLeader))
				{
					elemenet = component2.GetElemenet(0);
				}
				else
				{
					elemenet = component2.GetElemenet(num++);
				}
				this.SetMatchRecordMemberListElement(elemenet, cOMDT_GUILD_MATCH_HISTORY_INFO.astMemInfo[i]);
			}
			Text component3 = transform.Find("txtMatchScore").GetComponent<Text>();
			component3.set_text(cOMDT_GUILD_MATCH_HISTORY_INFO.iScore.ToString());
		}

		private void OnSubRankSliderValueChanged(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			Text component = srcFormScript.GetWidget(20).GetComponent<Text>();
			int num = (int)uiEvent.m_eventParams.sliderValue;
			component.set_text(Singleton<CTextManager>.GetInstance().GetText((num == 0) ? "GuildMatch_Slider_Text_Season" : "GuildMatch_Slider_Text_Week"));
			CUIListScript component2 = srcFormScript.GetWidget(4).GetComponent<CUIListScript>();
			if (component2.GetSelectedIndex() == 0)
			{
				this.RefreshGuildScoreRankList();
				this.SetSelfGuildScorePanel();
			}
			else if (component2.GetSelectedIndex() == 1)
			{
				this.RefreshMemberScoreRankList();
				GameObject widget = srcFormScript.GetWidget(9);
				this.SetMemberScoreListElement(widget, this.GetSelfIndexInGuildMemberScoreList());
			}
		}

		private void OnOpenMatchFormAndReadyGame(CUIEvent uiEvent)
		{
			this.OpenMatchForm(true);
		}

		private void OnRemindReady(CUIEvent uiEvent)
		{
			this.RequestGuildMatchRemind(uiEvent.m_eventParams.commonUInt64Param1);
			GameObject srcWidget = uiEvent.m_srcWidget;
			Button component = srcWidget.GetComponent<Button>();
			CUICommonSystem.SetButtonEnable(component, false, false, true);
			CUITimerScript cUITimerScript = srcWidget.GetComponent<CUITimerScript>();
			if (cUITimerScript == null)
			{
				cUITimerScript = srcWidget.AddComponent<CUITimerScript>();
			}
			cUITimerScript.SetTotalTime(10f);
			cUITimerScript.SetTimerEventId(enTimerEventType.TimeUp, enUIEventID.Guild_Match_RemindButtonCdOver);
			cUITimerScript.StartTimer();
		}

		private void OnRemindButtonCdOver(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidget != null)
			{
				Button component = uiEvent.m_srcWidget.GetComponent<Button>();
				if (component != null)
				{
					CUICommonSystem.SetButtonEnable(component, true, true, true);
				}
			}
		}

		private void OnOpenRule(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(15);
		}

		private void OnTeamListElementEnabled(CUIEvent uiEvent)
		{
			if (this.m_teamInfos == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_teamInfos.get_Count())
			{
				CUIListElementScript teamListElement = uiEvent.m_srcWidgetScript as CUIListElementScript;
				this.SetTeamListElement(teamListElement, this.m_teamInfos.get_Item(srcWidgetIndexInBelongedList).get_Key(), this.m_teamInfos.get_Item(srcWidgetIndexInBelongedList).get_Value());
			}
		}

		private void SetMatchRecordMemberListElement(CUIListElementScript memberListElement, COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO recordMemberInfo)
		{
			if (memberListElement != null)
			{
				Transform transform = memberListElement.transform;
				CUIHttpImageScript component = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
				component.SetImageUrl(CGuildHelper.GetHeadUrl(StringHelper.UTF8BytesToString(ref recordMemberInfo.szHeadUrl)));
				Text component2 = transform.Find("txtName").GetComponent<Text>();
				component2.set_text(StringHelper.UTF8BytesToString(ref recordMemberInfo.szName));
				GameObject gameObject = transform.Find("imgLeaderMark").gameObject;
				gameObject.CustomSetActive(Convert.ToBoolean(recordMemberInfo.bIsTeamLeader));
			}
		}

		private void OnGetGuildMatchSeasonRank(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			this.m_guildSeasonScores = new CSDT_RANKING_LIST_ITEM_INFO[rsp.stRankingListDetail.stOfSucc.dwItemNum];
			int num = 0;
			while ((long)num < (long)((ulong)rsp.stRankingListDetail.stOfSucc.dwItemNum))
			{
				this.m_guildSeasonScores[num] = rsp.stRankingListDetail.stOfSucc.astItemDetail[num];
				num++;
			}
			if (this.IsCurrentSeasonScoreTab())
			{
				this.RefreshGuildScoreRankListAndSelfGuildScorePanel();
			}
		}

		private void OnGetGuildMatchWeekRank(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			this.m_guildWeekScores = new CSDT_RANKING_LIST_ITEM_INFO[rsp.stRankingListDetail.stOfSucc.dwItemNum];
			int num = 0;
			while ((long)num < (long)((ulong)rsp.stRankingListDetail.stOfSucc.dwItemNum))
			{
				this.m_guildWeekScores[num] = rsp.stRankingListDetail.stOfSucc.astItemDetail[num];
				num++;
			}
			if (!this.IsCurrentSeasonScoreTab())
			{
				this.RefreshGuildScoreRankListAndSelfGuildScorePanel();
			}
		}

		private void OnLeaveGuild()
		{
			this.Clear();
		}

		private void OnPlayerNameChange()
		{
			if (this.m_teamInfos != null)
			{
				for (int i = 0; i < this.m_teamInfos.get_Count(); i++)
				{
					int count = this.m_teamInfos.get_Item(i).get_Value().Count;
					for (int j = 0; j < count; j++)
					{
						if (CGuildHelper.IsSelf(this.m_teamInfos.get_Item(i).get_Value()[j].Uid))
						{
							this.m_teamInfos.get_Item(i).get_Value()[j].Name = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name;
							return;
						}
					}
				}
			}
		}

		private void RefreshGuildScoreRankListAndSelfGuildScorePanel()
		{
			if (this.m_form != null)
			{
				CUIListScript component = this.m_form.GetWidget(4).GetComponent<CUIListScript>();
				if (component.GetSelectedIndex() == 0)
				{
					this.RefreshGuildScoreRankList();
					this.SetSelfGuildScorePanel();
				}
			}
		}

		private void RefreshGuildMatchForm()
		{
			this.RefreshGuildHead();
			this.RefreshGuildName();
			this.RefreshGuildMatchScore();
			this.RefreshTeamList();
			this.RefreshGuildMatchLeftMatchCnt();
			this.RefreshGuildMatchOpenTime();
			this.RefreshGuildMatchSignUpPanel();
			this.RefreshRankTabList();
		}

		private void RefreshGuildHead()
		{
			if (this.m_form == null)
			{
				return;
			}
			Image component = this.m_form.GetWidget(0).GetComponent<Image>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
		}

		private void RefreshGuildName()
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(1).GetComponent<Text>();
			component.set_text(CGuildHelper.GetGuildName());
		}

		private void RefreshGuildMatchScore()
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(2).GetComponent<Text>();
			component.set_text(Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchInfo.dwScore.ToString());
		}

		private void RefreshGuildMatchLeftMatchCnt()
		{
			if (this.m_form == null)
			{
				return;
			}
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			if (playerGuildMemberInfo == null)
			{
				DebugHelper.Assert(false, "selfMemInfo is null!!!");
				return;
			}
			int guildMatchLeftCntInCurRound = CGuildHelper.GetGuildMatchLeftCntInCurRound((int)playerGuildMemberInfo.GuildMatchInfo.bWeekMatchCnt);
			Text component = this.m_form.GetWidget(12).GetComponent<Text>();
			if (guildMatchLeftCntInCurRound <= 0)
			{
				component.set_text(string.Concat(new object[]
				{
					"<color=red>",
					guildMatchLeftCntInCurRound,
					"</color>",
					Singleton<CTextManager>.GetInstance().GetText("Common_Times")
				}));
			}
			else
			{
				component.set_text(string.Concat(new object[]
				{
					"<color=white>",
					guildMatchLeftCntInCurRound,
					"</color>",
					Singleton<CTextManager>.GetInstance().GetText("Common_Times")
				}));
			}
		}

		private void RefreshGuildMatchOpenTime()
		{
			if (this.m_form == null)
			{
				return;
			}
			Text component = this.m_form.GetWidget(10).GetComponent<Text>();
			component.set_text(this.GetGuildMatchOpenTimeDesc());
		}

		private string GetGuildMatchOpenTimeDesc()
		{
			ResRewardMatchTimeInfo resRewardMatchTimeInfo = null;
			uint guildMatchMapId = this.GetGuildMatchMapId();
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(6u, guildMatchMapId), out resRewardMatchTimeInfo);
			return resRewardMatchTimeInfo.szTimeTips;
		}

		public uint GetGuildMatchMapId()
		{
			return GameDataMgr.guildMiscDatabin.GetDataByKey(47u).dwConfValue;
		}

		private void RefreshGuildMatchSignUpPanel()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(23);
			GameObject widget2 = this.m_form.GetWidget(24);
			GameObject widget3 = this.m_form.GetWidget(25);
			Text component = this.m_form.GetWidget(26).GetComponent<Text>();
			if (this.IsCanSignUp())
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				if (!this.m_isSignUpCardBubbleShowed)
				{
					widget3.CustomSetActive(true);
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Prompt"));
					this.m_isSignUpCardBubbleShowed = true;
				}
				else
				{
					widget3.CustomSetActive(false);
				}
			}
			else
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				if (!this.m_isSignUpListBubbleShowed)
				{
					widget3.CustomSetActive(true);
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Check_Prompt"));
					this.m_isSignUpListBubbleShowed = true;
				}
				else
				{
					widget3.CustomSetActive(false);
				}
				this.RefreshGuildMatchSignUpPanelRedDot(widget2);
			}
		}

		private void RefreshGuildMatchSignUpPanelRedDot(GameObject btnSignUpListGo = null)
		{
			if (btnSignUpListGo == null && this.m_form != null)
			{
				btnSignUpListGo = this.m_form.GetWidget(24);
			}
			if (CGuildHelper.IsGuildMatchFormSignUpListBtnShowRedDot())
			{
				CUICommonSystem.AddRedDot(btnSignUpListGo, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(btnSignUpListGo);
			}
		}

		public void RefreshTeamList()
		{
			if (this.m_form == null || this.m_teamInfos == null)
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(3).GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_teamInfos.get_Count());
			this.RefreshMoreLeaderPanel();
		}

		public void RefreshMoreLeaderPanel()
		{
			GameObject widget = this.m_form.GetWidget(22);
			int curLeaderNum = this.GetCurLeaderNum();
			int totalLeaderNum = this.GetTotalLeaderNum();
			if (CGuildSystem.HasAppointMatchLeaderAuthority() && curLeaderNum < totalLeaderNum)
			{
				widget.CustomSetActive(true);
				Text component = widget.GetComponent<Text>();
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Match_Appoint_More_Team_Leader_Tip", new string[]
				{
					curLeaderNum.ToString(),
					totalLeaderNum.ToString()
				}));
			}
			else
			{
				widget.CustomSetActive(false);
			}
		}

		private void RefreshRankTabList()
		{
			if (this.m_form == null)
			{
				return;
			}
			ListView<string> listView = new ListView<string>();
			listView.Add(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_GuildMatchGuildScore"));
			listView.Add(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_GuildMatchMemberScore"));
			if (this.IsSelfBelongedTeamLeader())
			{
				listView.Add(Singleton<CTextManager>.GetInstance().GetText("Common_Invite"));
			}
			else
			{
				listView.Add(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invitation"));
			}
			CUIListScript component = this.m_form.GetWidget(4).GetComponent<CUIListScript>();
			component.SetElementAmount(listView.Count);
			for (int i = 0; i < listView.Count; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.transform.Find("txtName").GetComponent<Text>();
				component2.set_text(listView[i]);
			}
			if (this.IsSelfBelongedTeamLeader())
			{
				component.SelectElement(2, true);
				CUIListElementScript elemenet2 = component.GetElemenet(2);
				if (elemenet2 != null)
				{
					GameObject gameObject = elemenet2.gameObject;
					if (CGuildHelper.IsGuildMatchFormInviteTabShowRedDot())
					{
						CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					}
					else
					{
						CUICommonSystem.DelRedDot(gameObject);
					}
				}
				this.InitMemberInviteList();
			}
			else
			{
				component.SelectElement(0, true);
			}
		}

		private void SetTeamListElement(CUIListElementScript teamListElement, ulong teamLeaderUid, ListView<CGuildMatchSystem.TeamPlayerInfo> teamPlayerInfos)
		{
			if (teamListElement == null || teamPlayerInfos == null)
			{
				return;
			}
			Transform transform = teamListElement.transform;
			if (teamPlayerInfos[0] != null)
			{
				Text component = transform.Find("imgTeamTitleBg/txtTeamName").GetComponent<Text>();
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Team_Name", new string[]
				{
					teamPlayerInfos[0].Name
				}));
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			Transform transform2 = transform.Find("imgTeamTitleBg/pnlStatusAndOperation");
			GameObject gameObject = transform2.Find("btnStartGame").gameObject;
			GameObject gameObject2 = transform2.Find("btnReadyGame").gameObject;
			GameObject gameObject3 = transform2.Find("btnCancelReadyGame").gameObject;
			GameObject gameObject4 = transform2.Find("btnOBGame").gameObject;
			GameObject gameObject5 = transform2.Find("TagTeamStatusBlue").gameObject;
			GameObject gameObject6 = transform2.Find("TagTeamStatusYellow").gameObject;
			GameObject gameObject7 = transform2.Find("txtTeamStatus").gameObject;
			gameObject7.CustomSetActive(true);
			Text component2 = gameObject7.GetComponent<Text>();
			gameObject5.CustomSetActive(false);
			gameObject6.CustomSetActive(true);
			if (playerGuildMemberInfo != null && this.IsInTeam(playerGuildMemberInfo.GuildMatchInfo.ullTeamLeaderUid, teamLeaderUid))
			{
				if (this.IsTeamLeader(masterRoleInfo.playerUllUID, teamLeaderUid))
				{
					gameObject.CustomSetActive(true);
					gameObject2.CustomSetActive(false);
					gameObject3.CustomSetActive(false);
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Prepare"));
					bool flag = this.IsTeamAllPlayerReadyForGame(teamPlayerInfos, teamLeaderUid);
					CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), flag, flag, true);
				}
				else if (this.IsReadyForGame(teamPlayerInfos, masterRoleInfo.playerUllUID))
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(false);
					gameObject3.CustomSetActive(true);
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_In_Prepare"));
				}
				else
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					gameObject3.CustomSetActive(false);
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Please_Prepare"));
				}
				gameObject4.CustomSetActive(false);
			}
			else
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				ulong obUid = this.GetObUid(teamLeaderUid);
				gameObject4.CustomSetActive(obUid > 0uL);
				uint num = 0u;
				bool flag2 = this.IsInObDelayedTime(teamLeaderUid, out num);
				if (flag2)
				{
					CUICommonSystem.SetButtonEnable(gameObject4.GetComponent<Button>(), false, false, true);
					CUITimerScript component3 = gameObject4.transform.Find("obWaitingTimer").GetComponent<CUITimerScript>();
					component3.SetTotalTime(num);
					component3.StartTimer();
				}
				if (obUid > 0uL)
				{
					CUIEventScript component4 = gameObject4.GetComponent<CUIEventScript>();
					component4.m_onClickEventParams.commonUInt64Param1 = obUid;
				}
				if (flag2)
				{
					component2.set_text(string.Empty);
				}
				else
				{
					string text = this.GetMatchStartedTimeStr(teamLeaderUid);
					if (string.IsNullOrEmpty(text))
					{
						text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Prepare");
					}
					else
					{
						gameObject5.CustomSetActive(true);
						gameObject6.CustomSetActive(false);
					}
					component2.set_text(text);
				}
			}
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(teamLeaderUid);
			GameObject gameObject8 = transform.Find("imgContinuousWin").gameObject;
			if (guildMemberInfoByUid.GuildMatchInfo.bContinueWin > 0)
			{
				gameObject8.CustomSetActive(true);
				Text component5 = gameObject8.transform.Find("txtContinuousWin").GetComponent<Text>();
				component5.set_text(guildMemberInfoByUid.GuildMatchInfo.bContinueWin + Singleton<CTextManager>.GetInstance().GetText("Common_Continues_Win"));
			}
			else
			{
				gameObject8.CustomSetActive(false);
			}
			Text component6 = transform.Find("imgTeamScore/txtTeamScore").GetComponent<Text>();
			component6.set_text(guildMemberInfoByUid.GuildMatchInfo.dwScore.ToString());
			CUIListScript component7 = transform.Find("PlayerList").GetComponent<CUIListScript>();
			component7.SetElementAmount(5);
			for (int i = 0; i < 5; i++)
			{
				this.SetPlayerListElement(component7.GetElemenet(i), (i < teamPlayerInfos.Count) ? teamPlayerInfos[i] : null, teamLeaderUid);
			}
		}

		private void SetPlayerListElement(CUIListElementScript playerListElement, CGuildMatchSystem.TeamPlayerInfo playerInfo, ulong teamLeaderUid)
		{
			if (playerListElement == null)
			{
				return;
			}
			Transform transform = playerListElement.transform;
			GameObject gameObject = transform.Find("imgQuestion").gameObject;
			GameObject gameObject2 = transform.Find("btnKick").gameObject;
			GameObject gameObject3 = transform.Find("imgHead").gameObject;
			GameObject gameObject4 = transform.Find("txtPlayerName").gameObject;
			GameObject gameObject5 = transform.Find("imgLeader").gameObject;
			GameObject gameObject6 = transform.Find("imgReady").gameObject;
			if (this.IsSlotOccupied(playerInfo))
			{
				gameObject.CustomSetActive(false);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(true);
				gameObject5.CustomSetActive(playerListElement.m_index == 0);
				Text component = gameObject4.GetComponent<Text>();
				component.set_text(playerInfo.Name);
				if (CGuildHelper.IsSelf(playerInfo.Uid))
				{
					component.set_color(CUIUtility.s_Text_Color_Self);
				}
				else
				{
					component.set_color(CUIUtility.s_Text_Color_White);
				}
				CUIHttpImageScript component2 = gameObject3.GetComponent<CUIHttpImageScript>();
				component2.SetImageUrl(CGuildHelper.GetHeadUrl(playerInfo.HeadUrl));
				bool flag = playerInfo.Uid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
				bool flag2 = this.IsTeamLeader(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID, teamLeaderUid);
				gameObject6.CustomSetActive(this.IsSameTeamWithSelf(teamLeaderUid) && !this.IsTeamLeader(playerInfo.Uid, teamLeaderUid) && playerInfo.IsReady);
				if ((flag || flag2) && !this.IsTeamLeader(playerInfo.Uid, teamLeaderUid) && !playerInfo.IsReady)
				{
					gameObject2.CustomSetActive(true);
					CUIEventScript component3 = gameObject2.GetComponent<CUIEventScript>();
					component3.m_onClickEventParams.commonUInt64Param1 = playerInfo.Uid;
					component3.m_onClickEventParams.commonBool = flag;
				}
				else
				{
					gameObject2.CustomSetActive(false);
				}
			}
			else
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
			}
		}

		private bool IsSlotOccupied(CGuildMatchSystem.TeamPlayerInfo playerInfo)
		{
			return playerInfo != null && playerInfo.Uid > 0uL;
		}

		private void RefreshMatchRecordForm()
		{
			if (this.m_matchRecordForm == null || this.m_matchRecords == null || this.m_matchRecords.Length == 0)
			{
				return;
			}
			CUIListScript component = this.m_matchRecordForm.GetWidget(0).GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_matchRecords.Length);
		}

		public ListView<GuildMemInfo> GetGuildMemberInviteList()
		{
			return this.m_guildMemberInviteList;
		}

		public bool IsTeamFull(ulong teamLeaderUid)
		{
			if (this.m_teamInfos != null)
			{
				int i = 0;
				while (i < this.m_teamInfos.get_Count())
				{
					if (this.m_teamInfos.get_Item(i).get_Key() == teamLeaderUid)
					{
						if (this.m_teamInfos.get_Item(i).get_Value().Count < 5)
						{
							return false;
						}
						for (int j = 0; j < this.m_teamInfos.get_Item(i).get_Value().Count; j++)
						{
							if (!this.IsSlotOccupied(this.m_teamInfos.get_Item(i).get_Value()[j]))
							{
								return false;
							}
						}
						return true;
					}
					else
					{
						i++;
					}
				}
			}
			return false;
		}

		private CGuildMatchSystem.TeamPlayerInfo CreateTeamPlayerInfoObj(GuildMemInfo guildMemInfo)
		{
			return new CGuildMatchSystem.TeamPlayerInfo(guildMemInfo.stBriefInfo.uulUid, guildMemInfo.stBriefInfo.sName, guildMemInfo.stBriefInfo.szHeadUrl, Convert.ToBoolean(guildMemInfo.GuildMatchInfo.bIsReady));
		}

		private bool IsNeedOpenGuildMatchForm(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF ntf)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(CGuildMatchSystem.GuildMatchFormPath) != null || !Utility.IsCanShowPrompt())
			{
				return false;
			}
			for (int i = 0; i < (int)ntf.bCnt; i++)
			{
				if (ntf.astChgInfo[i].ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID && ntf.astChgInfo[i].ullTeamLeaderUid > 0uL)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsLeaderNumFull()
		{
			int curLeaderNum = this.GetCurLeaderNum();
			int totalLeaderNum = this.GetTotalLeaderNum();
			return curLeaderNum >= totalLeaderNum;
		}

		private int GetCurLeaderNum()
		{
			ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
			int num = 0;
			for (int i = 0; i < guildMemberInfos.Count; i++)
			{
				if (Convert.ToBoolean(guildMemberInfos[i].GuildMatchInfo.bIsLeader))
				{
					num++;
				}
			}
			return num;
		}

		private int GetTotalLeaderNum()
		{
			ResGuildLevel dataByKey = GameDataMgr.guildLevelDatabin.GetDataByKey((long)CGuildHelper.GetGuildLevel());
			if (dataByKey != null)
			{
				return (int)dataByKey.bTeamCnt;
			}
			return 0;
		}

		private int GetSelfIndexInGuildMemberScoreList()
		{
			if (this.m_guildMemberScoreList == null)
			{
				return -1;
			}
			for (int i = 0; i < this.m_guildMemberScoreList.Count; i++)
			{
				if (this.m_guildMemberScoreList[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
				{
					return i;
				}
			}
			return -1;
		}

		private ulong GetObUid(ulong teamLeaderUid)
		{
			ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
			if (guildMatchObInfos != null)
			{
				for (int i = 0; i < guildMatchObInfos.Count; i++)
				{
					if (teamLeaderUid == guildMatchObInfos[i].ullUid)
					{
						return teamLeaderUid;
					}
				}
			}
			return 0uL;
		}

		private string GetMatchStartedTimeStr(ulong teamLeaderUid)
		{
			ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
			uint num = 0u;
			if (guildMatchObInfos != null)
			{
				for (int i = 0; i < guildMatchObInfos.Count; i++)
				{
					if (teamLeaderUid == guildMatchObInfos[i].ullUid)
					{
						num = guildMatchObInfos[i].dwBeginTime;
						break;
					}
				}
			}
			if (num > 0u)
			{
				uint num2 = (uint)(CRoleInfo.GetCurrentUTCTime() - (int)num);
				TimeSpan timeSpan = new TimeSpan((long)((ulong)num2 * 10000000uL));
				return Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Game_Started", new string[]
				{
					((int)timeSpan.get_TotalMinutes()).ToString()
				});
			}
			return string.Empty;
		}

		private bool IsInObDelayedTime(ulong teamLeaderUid, out uint obWaitingTime)
		{
			ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
			uint num = 0u;
			if (guildMatchObInfos != null)
			{
				for (int i = 0; i < guildMatchObInfos.Count; i++)
				{
					if (teamLeaderUid == guildMatchObInfos[i].ullUid)
					{
						num = guildMatchObInfos[i].dwBeginTime;
						break;
					}
				}
			}
			if (num > 0u)
			{
				uint num2 = (uint)(CRoleInfo.GetCurrentUTCTime() - (int)num);
				uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(215u).dwConfValue;
				if (dwConfValue > num2)
				{
					obWaitingTime = dwConfValue - num2;
					return true;
				}
			}
			obWaitingTime = 0u;
			return false;
		}

		public bool IsInObDelayedTime(ulong obUid)
		{
			uint num;
			return this.IsInObDelayedTime(obUid, out num);
		}

		public static bool IsInGuildMatchJoinLimitTime(GuildMemInfo guildMemInfo)
		{
			if (guildMemInfo != null)
			{
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(49u).dwConfValue;
				return (long)CRoleInfo.GetCurrentUTCTime() - (long)((ulong)guildMemInfo.JoinTime) < (long)((ulong)dwConfValue);
			}
			return false;
		}

		private CSDT_RANKING_LIST_ITEM_INFO[] GetGuildScoreRankInfo()
		{
			return this.IsCurrentSeasonScoreTab() ? this.m_guildSeasonScores : this.m_guildWeekScores;
		}

		public List<COBSystem.stOBGuild> GetGuidMatchObInfo()
		{
			List<COBSystem.stOBGuild> list = new List<COBSystem.stOBGuild>();
			GuildInfo currentGuildInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo;
			if (currentGuildInfo == null || currentGuildInfo.GuildMatchObInfos == null)
			{
				return list;
			}
			for (int i = 0; i < currentGuildInfo.GuildMatchObInfos.Count; i++)
			{
				if (currentGuildInfo.GuildMatchObInfos[i].dwBeginTime > 0u)
				{
					for (int j = 0; j < currentGuildInfo.GuildMatchObInfos[i].astHeroInfo.Length; j++)
					{
						GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid);
						if (guildMemberInfoByUid != null)
						{
							list.Add(new COBSystem.stOBGuild
							{
								obUid = currentGuildInfo.GuildMatchObInfos[i].ullUid,
								playerUid = currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid,
								playerName = guildMemberInfoByUid.stBriefInfo.sName,
								teamName = this.GetTeamName(currentGuildInfo.GuildMatchObInfos[i].ullUid),
								headUrl = CGuildHelper.GetHeadUrl(guildMemberInfoByUid.stBriefInfo.szHeadUrl),
								dwHeroID = currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].dwHeroID,
								dwStartTime = currentGuildInfo.GuildMatchObInfos[i].dwBeginTime,
								bGrade = guildMemberInfoByUid.stBriefInfo.rankGrade.bGrade,
								dwClass = guildMemberInfoByUid.stBriefInfo.dwClassOfRank,
								dwObserveNum = currentGuildInfo.GuildMatchObInfos[i].dwOBCnt
							});
						}
					}
				}
			}
			return list;
		}

		private string GetTeamName(ulong teamLeaderUid)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(teamLeaderUid);
			if (guildMemberInfoByUid != null)
			{
				return guildMemberInfoByUid.stBriefInfo.sName;
			}
			return string.Empty;
		}

		public bool IsInGuildMatchTime()
		{
			uint guildMatchMapId = this.GetGuildMatchMapId();
			return CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_GUILDMATCH, guildMatchMapId).matchState == enMatchOpenState.enMatchOpen_InActiveTime;
		}

		public bool IsCanSignUp()
		{
			return this.IsInSignUpTime() && !this.IsSelfInAnyTeam() && !this.m_isSignedUp;
		}

		public bool IsInSignUpTime()
		{
			uint guildMatchMapId = this.GetGuildMatchMapId();
			return CUICommonSystem.IsInSignUpTime(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_GUILDMATCH, guildMatchMapId);
		}

		public bool IsSingUpListEmpty()
		{
			return this.m_signUpInfos == null || this.m_signUpInfos.get_Count() == 0;
		}

		public bool IsHaveInvitationUnhandled()
		{
			if (this.m_invitationInfos != null)
			{
				for (int i = 0; i < this.m_invitationInfos.get_Count(); i++)
				{
					if (this.m_invitationInfos.get_Item(i).invitationInfo.bState == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private string GetSignUpTimeText()
		{
			return Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Game_Time");
		}

		private void OnSignUpCardFormClosed(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(0).GetComponent<Text>();
			if (string.Equals(component.get_text(), Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_New_Card")))
			{
				this.m_createSignUpCardForm = null;
			}
			else if (string.Equals(component.get_text(), Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Modify_Card")))
			{
				this.m_modifySignUpCardForm = null;
			}
			else
			{
				DebugHelper.Assert(false, "Error title text: {0}", new object[]
				{
					component.get_text()
				});
			}
		}

		private void OnSignUpListFormClosed(CUIEvent uiEvent)
		{
			this.m_signUpListForm = null;
		}

		private void OnInvitationFormClosed(CUIEvent uiEvent)
		{
			this.m_invitationForm = null;
		}

		private void OnOnlineInvitationFormClosed(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			ulong commonUInt64Param = srcFormScript.GetFormEventParams(enFormEventType.Close).commonUInt64Param1;
			if (this.m_openedOnlineInvitationInviterUids != null)
			{
				this.m_openedOnlineInvitationInviterUids.Remove(commonUInt64Param);
			}
		}

		private void OnOpenSignUpCardForm(CUIEvent uiEvent)
		{
			if (!this.IsCanSignUp())
			{
				this.RefreshGuildMatchSignUpPanel();
				return;
			}
			this.m_createSignUpCardForm = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchSignUpCardFormPath, false, true);
			if (this.m_createSignUpCardForm == null)
			{
				return;
			}
			this.SetSignUpCardInfo(false, this.m_createSignUpCardForm);
		}

		private void SetSignUpCardInfo(bool isModify, CUIFormScript signUpCardForm)
		{
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			Text component = signUpCardForm.GetWidget(0).GetComponent<Text>();
			GameObject widget = signUpCardForm.GetWidget(12);
			Text component2 = signUpCardForm.GetWidget(1).GetComponent<Text>();
			Text component3 = signUpCardForm.GetWidget(2).GetComponent<Text>();
			Text component4 = signUpCardForm.GetWidget(14).GetComponent<Text>();
			GameObject widget2 = signUpCardForm.GetWidget(3);
			GameObject widget3 = signUpCardForm.GetWidget(4);
			Text component5 = signUpCardForm.GetWidget(8).GetComponent<Text>();
			Text component6 = signUpCardForm.GetWidget(9).GetComponent<Text>();
			Text component7 = signUpCardForm.GetWidget(5).GetComponent<Text>();
			InputField component8 = signUpCardForm.GetWidget(6).GetComponent<InputField>();
			Text component9 = signUpCardForm.GetWidget(7).GetComponent<Text>();
			CUIEventScript component10 = signUpCardForm.GetWidget(13).GetComponent<CUIEventScript>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText(isModify ? "GuildMatch_SignUp_Modify_Card" : "GuildMatch_SignUp_New_Card"));
			widget.CustomSetActive(!isModify);
			component2.set_text(playerGuildMemberInfo.stBriefInfo.sName + "   Lv." + playerGuildMemberInfo.stBriefInfo.dwLevel);
			this.SetSignUpRankDesc(component3, component4, playerGuildMemberInfo);
			widget2.CustomSetActive(false);
			widget3.CustomSetActive(false);
			this.InitPosDropList(widget2);
			this.InitPosDropList(widget3);
			component5.set_text(this.m_signUpPosTexts[(int)this.m_signUpPos1]);
			component6.set_text(this.m_signUpPosTexts[(int)this.m_signUpPos2]);
			component7.set_text(string.Empty);
			ListView<COMDT_MOST_USED_HERO_INFO> sortedAllMostUsedHeroInfos = this.GetSortedAllMostUsedHeroInfos();
			if (sortedAllMostUsedHeroInfos != null)
			{
				int num = Mathf.Min(3, sortedAllMostUsedHeroInfos.Count);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < num; i++)
				{
					COMDT_MOST_USED_HERO_INFO cOMDT_MOST_USED_HERO_INFO = sortedAllMostUsedHeroInfos[i];
					if (cOMDT_MOST_USED_HERO_INFO != null)
					{
						stringBuilder.Append(CHeroInfo.GetHeroName(cOMDT_MOST_USED_HERO_INFO.dwHeroID));
						stringBuilder.Append(" ");
					}
				}
				component7.set_text(stringBuilder.ToString());
			}
			if (this.m_isSignedUp && this.m_signUpMemoText != null)
			{
				component8.set_text(this.m_signUpMemoText);
			}
			else
			{
				component8.set_text(string.Empty);
				component8.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Default_Memo"));
			}
			component9.set_text(this.GetSignUpTimeText());
			if (isModify)
			{
				component10.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Match_ModifySignUpCard);
			}
		}

		private void SetSignUpRankDesc(Text txtRankGrade, Text txtRankStar, GuildMemInfo memberInfo)
		{
			byte bGrade = memberInfo.stBriefInfo.rankGrade.bGrade;
			int dwScore = (int)memberInfo.stBriefInfo.rankGrade.dwScore;
			txtRankGrade.set_text(CLadderView.GetRankName(bGrade, memberInfo.stBriefInfo.dwClassOfRank));
			txtRankStar.set_text("X" + dwScore);
		}

		private void SetSignUpStateColumn(Transform element, ulong memberUid, bool isNeedHighlightText = false)
		{
			GameObject gameObject = element.Find("txtState").gameObject;
			gameObject.CustomSetActive(true);
			Text component = gameObject.GetComponent<Text>();
			GameObject gameObject2 = element.Find("btnInvite").gameObject;
			gameObject2.CustomSetActive(false);
			if (this.IsSelfBelongedTeamLeader())
			{
				if (this.IsInAnyTeam(memberUid))
				{
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_In_Team"));
				}
				else
				{
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					component.set_text(string.Empty);
				}
			}
			else if (this.IsInAnyTeam(memberUid))
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_In_Team"));
			}
			else if (isNeedHighlightText)
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_In_Examine"));
			}
			else
			{
				component.set_text("<color=#27b56a>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_In_Examine") + "</color>");
			}
		}

		private void InitPosDropList(GameObject posDropListGo)
		{
			this.InitPosTexts();
			CUIListScript component = posDropListGo.GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_signUpPosTexts.Length);
			for (int i = 0; i < this.m_signUpPosTexts.Length; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				if (elemenet != null)
				{
					Text component2 = elemenet.transform.Find("Text").GetComponent<Text>();
					component2.set_text(this.m_signUpPosTexts[i]);
				}
			}
		}

		private void InitPosTexts()
		{
			if (this.m_signUpPosTexts == null || this.m_signUpPosTexts.Length == 0)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Please_Check");
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
				string text3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
				string text5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
				string text6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
				string text7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
				this.m_signUpPosTexts = new string[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6,
					text7
				};
			}
		}

		private void OnOpenSignUpListForm(CUIEvent uiEvent)
		{
			if (this.IsCanSignUp())
			{
				this.RefreshGuildMatchSignUpPanel();
				return;
			}
			this.OpenSignUpListForm();
			this.RequestGetGuildMatchSignUpList();
			this.m_isHaveNewSignUpInfo = false;
			this.RefreshGuildMatchSignUpPanel();
		}

		private void OpenSignUpListForm()
		{
			this.m_signUpListForm = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchSignUpListFormPath, false, true);
			if (this.m_signUpListForm == null)
			{
				return;
			}
			Text component = this.m_signUpListForm.GetWidget(0).GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_List_Title", new string[]
			{
				CGuildHelper.GetGuildName()
			}));
			this.InitPosTexts();
			this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.Default;
			this.RefreshSignUpList();
			Text component2 = this.m_signUpListForm.GetWidget(2).GetComponent<Text>();
			component2.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_SignUp_Time_Prefix") + "<color=#e49316>" + this.GetSignUpTimeText() + "</color>");
			GameObject widget = this.m_signUpListForm.GetWidget(3);
			widget.CustomSetActive(this.m_isSignedUp);
			GameObject widget2 = this.m_signUpListForm.GetWidget(4);
			widget2.CustomSetActive(this.m_isSignedUp && !this.IsSelfInAnyTeam());
		}

		private void RefreshSignUpList()
		{
			if (this.m_signUpListForm == null)
			{
				return;
			}
			CUIListScript component = this.m_signUpListForm.GetWidget(1).GetComponent<CUIListScript>();
			if (this.m_signUpInfos != null)
			{
				if (this.m_signUpInfos.get_Count() > 0)
				{
					this.SortSignUpList();
				}
				component.SetElementAmount(this.m_signUpInfos.get_Count());
			}
		}

		private void SortSignUpList()
		{
			switch (this.m_curSignUpListSortType)
			{
			case CGuildMatchSystem.enSignUpListSortType.RankGradeDesc:
				this.m_signUpInfos.Sort(new Comparison<CGuildMatchSystem.stSignUpInfo>(this.SortSignUpListRankGradeDesc));
				break;
			case CGuildMatchSystem.enSignUpListSortType.LevelDesc:
				this.m_signUpInfos.Sort(new Comparison<CGuildMatchSystem.stSignUpInfo>(this.SortSignUpListLevelDesc));
				break;
			case CGuildMatchSystem.enSignUpListSortType.PosDesc:
				this.m_signUpInfos.Sort(new Comparison<CGuildMatchSystem.stSignUpInfo>(this.SortSignUpListPoslDesc));
				break;
			case CGuildMatchSystem.enSignUpListSortType.StateDesc:
				this.m_signUpInfos.Sort(new Comparison<CGuildMatchSystem.stSignUpInfo>(this.SortSignUpListStateDesc));
				break;
			}
		}

		private int SortSignUpListRankGradeDesc(CGuildMatchSystem.stSignUpInfo info1, CGuildMatchSystem.stSignUpInfo info2)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(info1.uid);
			if (guildMemberInfoByUid == null)
			{
				DebugHelper.Assert(false, "guildMemberInfo[uid={0}] is null!!!", new object[]
				{
					info1.uid
				});
				return 0;
			}
			GuildMemInfo guildMemberInfoByUid2 = CGuildHelper.GetGuildMemberInfoByUid(info2.uid);
			if (guildMemberInfoByUid2 == null)
			{
				DebugHelper.Assert(false, "guildMemberInfo[uid={0}] is null!!!", new object[]
				{
					info2.uid
				});
				return 0;
			}
			byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade((int)guildMemberInfoByUid.stBriefInfo.rankGrade.bGrade).bLogicGrade;
			byte bLogicGrade2 = CLadderSystem.GetGradeDataByShowGrade((int)guildMemberInfoByUid2.stBriefInfo.rankGrade.bGrade).bLogicGrade;
			if (bLogicGrade == bLogicGrade2)
			{
				int dwScore = (int)guildMemberInfoByUid.stBriefInfo.rankGrade.dwScore;
				int dwScore2 = (int)guildMemberInfoByUid2.stBriefInfo.rankGrade.dwScore;
				return dwScore2.CompareTo(dwScore);
			}
			return bLogicGrade2.CompareTo(bLogicGrade);
		}

		private int SortSignUpListLevelDesc(CGuildMatchSystem.stSignUpInfo info1, CGuildMatchSystem.stSignUpInfo info2)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(info1.uid);
			if (guildMemberInfoByUid == null)
			{
				DebugHelper.Assert(false, "guildMemberInfo[uid={0}] is null!!!", new object[]
				{
					info1.uid
				});
				return 0;
			}
			GuildMemInfo guildMemberInfoByUid2 = CGuildHelper.GetGuildMemberInfoByUid(info2.uid);
			if (guildMemberInfoByUid2 == null)
			{
				DebugHelper.Assert(false, "guildMemberInfo[uid={0}] is null!!!", new object[]
				{
					info2.uid
				});
				return 0;
			}
			return guildMemberInfoByUid2.stBriefInfo.dwLevel.CompareTo(guildMemberInfoByUid.stBriefInfo.dwLevel);
		}

		private int SortSignUpListPoslDesc(CGuildMatchSystem.stSignUpInfo info1, CGuildMatchSystem.stSignUpInfo info2)
		{
			if (info1.pos1 == info2.pos1)
			{
				return -info1.pos2.CompareTo(info2.pos2);
			}
			return -info1.pos1.CompareTo(info2.pos1);
		}

		private int SortSignUpListStateDesc(CGuildMatchSystem.stSignUpInfo info1, CGuildMatchSystem.stSignUpInfo info2)
		{
			bool flag = this.IsInAnyTeam(info1.uid);
			bool flag2 = this.IsInAnyTeam(info2.uid);
			if (this.IsSelfBelongedTeamLeader())
			{
				if (flag)
				{
					if (flag2)
					{
						return info1.uid.CompareTo(info2.uid);
					}
					return 1;
				}
				else
				{
					if (flag2)
					{
						return -1;
					}
					bool flag3 = this.IsSignUpListClickBtnActive(info1.uid);
					bool flag4 = this.IsSignUpListClickBtnActive(info2.uid);
					if (flag3 && !flag4)
					{
						return -1;
					}
					if (!flag3 && flag4)
					{
						return 1;
					}
					return info1.uid.CompareTo(info2.uid);
				}
			}
			else
			{
				if (!flag && flag2)
				{
					return -1;
				}
				if (flag && !flag2)
				{
					return 1;
				}
				return info1.uid.CompareTo(info2.uid);
			}
		}

		private bool IsSignUpListClickBtnActive(ulong targetUid)
		{
			if (this.m_signUpListForm != null)
			{
				CUIListScript component = this.m_signUpListForm.GetWidget(1).GetComponent<CUIListScript>();
				for (int i = 0; i < component.GetElementAmount(); i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					if (elemenet != null)
					{
						Transform transform = elemenet.transform;
						Text component2 = transform.Find("txtUidData").GetComponent<Text>();
						ulong num;
						if (ulong.TryParse(component2.get_text(), ref num) && num == targetUid)
						{
							GameObject gameObject = transform.Find("btnInvite").gameObject;
							return gameObject.activeInHierarchy;
						}
					}
				}
			}
			return false;
		}

		private void OnSignUpListSortRankGradeDesc(CUIEvent uiEvent)
		{
			if (this.m_curSignUpListSortType != CGuildMatchSystem.enSignUpListSortType.RankGradeDesc)
			{
				this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.RankGradeDesc;
				this.RefreshSignUpList();
			}
		}

		private void OnSignUpListSortLevelDesc(CUIEvent uiEvent)
		{
			if (this.m_curSignUpListSortType != CGuildMatchSystem.enSignUpListSortType.LevelDesc)
			{
				this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.LevelDesc;
				this.RefreshSignUpList();
			}
		}

		private void OnSignUpListSortPosDesc(CUIEvent uiEvent)
		{
			if (this.m_curSignUpListSortType != CGuildMatchSystem.enSignUpListSortType.PosDesc)
			{
				this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.PosDesc;
				this.RefreshSignUpList();
			}
		}

		private void OnSignUpListSortStateDesc(CUIEvent uiEvent)
		{
			this.m_curSignUpListSortType = CGuildMatchSystem.enSignUpListSortType.StateDesc;
			this.RefreshSignUpList();
		}

		private void OnCreateSignUpCard(CUIEvent uiEvent)
		{
			if (this.m_createSignUpCardForm == null)
			{
				return;
			}
			InputField component = this.m_createSignUpCardForm.GetWidget(6).GetComponent<InputField>();
			this.m_signUpMemoText = component.get_text();
			this.RequestGuildMatchSignUp(this.m_signUpPos1, this.m_signUpPos2, component.get_text());
		}

		private void OnOpenModifySignUpCardForm(CUIEvent uiEvent)
		{
			this.m_modifySignUpCardForm = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchSignUpCardFormPath, false, true);
			if (this.m_modifySignUpCardForm == null)
			{
				return;
			}
			this.SetSignUpCardInfo(true, this.m_modifySignUpCardForm);
		}

		private void OnModifySignUpCard(CUIEvent uiEvent)
		{
			if (this.m_modifySignUpCardForm == null)
			{
				return;
			}
			InputField component = this.m_modifySignUpCardForm.GetWidget(6).GetComponent<InputField>();
			this.m_signUpMemoText = component.get_text();
			this.RequestModifyGuildMatchSignUp(this.m_signUpPos1, this.m_signUpPos2, component.get_text());
		}

		private int GetSortedMostUsedHeroInfos(ref COMDT_GUILDMATCH_SIGNUP_HEROINFO[] infos)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				ListView<COMDT_MOST_USED_HERO_INFO> listView = new ListView<COMDT_MOST_USED_HERO_INFO>();
				int num = 0;
				while ((long)num < (long)((ulong)masterRoleInfo.MostUsedHeroDetail.dwHeroNum))
				{
					listView.Add(masterRoleInfo.MostUsedHeroDetail.astHeroInfoList[num]);
					num++;
				}
				this.SortMostUsedHeroList(listView);
				int num2 = Mathf.Min(3, listView.Count);
				for (int i = 0; i < num2; i++)
				{
					infos[i].dwHeroID = listView[i].dwHeroID;
					infos[i].dwGameWinNum = listView[i].dwGameWinNum;
					infos[i].dwGameLoseNum = listView[i].dwGameLoseNum;
				}
				return num2;
			}
			return 0;
		}

		private ListView<COMDT_MOST_USED_HERO_INFO> GetSortedAllMostUsedHeroInfos()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				ListView<COMDT_MOST_USED_HERO_INFO> listView = new ListView<COMDT_MOST_USED_HERO_INFO>();
				int num = 0;
				while ((long)num < (long)((ulong)masterRoleInfo.MostUsedHeroDetail.dwHeroNum))
				{
					listView.Add(masterRoleInfo.MostUsedHeroDetail.astHeroInfoList[num]);
					num++;
				}
				this.SortMostUsedHeroList(listView);
				return listView;
			}
			return null;
		}

		private void SortMostUsedHeroList(ListView<COMDT_MOST_USED_HERO_INFO> mostUsedHeroList)
		{
			if (mostUsedHeroList != null)
			{
				mostUsedHeroList.Sort(delegate(COMDT_MOST_USED_HERO_INFO b, COMDT_MOST_USED_HERO_INFO a)
				{
					if (a == null && b == null)
					{
						return 0;
					}
					if (a != null && b == null)
					{
						return 1;
					}
					if (a == null && b != null)
					{
						return -1;
					}
					if (a.dwGameWinNum + a.dwGameLoseNum > b.dwGameWinNum + b.dwGameLoseNum)
					{
						return 1;
					}
					if (a.dwGameWinNum + a.dwGameLoseNum == b.dwGameWinNum + b.dwGameLoseNum)
					{
						return 0;
					}
					return -1;
				});
			}
		}

		private void OnViewInvitation(CUIEvent uiEvent)
		{
			if (this.m_invitationInfos == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (0 <= srcWidgetIndexInBelongedList && srcWidgetIndexInBelongedList < this.m_invitationInfos.get_Count())
			{
				this.m_curViewInvitationInviterUid = this.m_invitationInfos.get_Item(srcWidgetIndexInBelongedList).invitationInfo.ullUid;
				if (!this.CheckInvitationValid())
				{
					return;
				}
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(this.m_invitationInfos.get_Item(srcWidgetIndexInBelongedList).invitationInfo.ullUid);
				if (guildMemberInfoByUid != null)
				{
					string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invitation_Content", new string[]
					{
						guildMemberInfoByUid.stBriefInfo.sName,
						this.GetGuildMatchOpenTimeDesc()
					});
					COM_GUILDMATCH_INVITECARD_STATE bState = (COM_GUILDMATCH_INVITECARD_STATE)this.m_invitationInfos.get_Item(srcWidgetIndexInBelongedList).invitationInfo.bState;
					this.OpenInvitationForm(text, bState);
				}
			}
		}

		private void OnAcceptInvitationBtnClick(CUIEvent uiEvent)
		{
			ulong num = uiEvent.m_eventParams.commonBool ? uiEvent.m_eventParams.commonUInt64Param1 : this.m_curViewInvitationInviterUid;
			if (num > 0uL)
			{
				GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
				if (playerGuildMemberInfo == null)
				{
					return;
				}
				if (!this.CheckInvitationValid())
				{
					return;
				}
				if (this.IsSelfInAnyTeam() && num != playerGuildMemberInfo.GuildMatchInfo.ullTeamLeaderUid)
				{
					string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Accept_Invitation_Confirm_Msg");
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_Accept_Invite, enUIEventID.None, new stUIEventParams
					{
						commonUInt64Param1 = num
					}, false);
				}
				else
				{
					this.RequestDealGuildMatchMemberInvite(num, true);
				}
			}
			else
			{
				DebugHelper.Assert(false, "inviterUid must above 0!!!");
			}
		}

		private void OnRefuseInvitationBtnClick(CUIEvent uiEvent)
		{
			bool commonBool = uiEvent.m_eventParams.commonBool;
			ulong num = commonBool ? uiEvent.m_eventParams.commonUInt64Param1 : this.m_curViewInvitationInviterUid;
			if (num <= 0uL)
			{
				DebugHelper.Assert(false, "inviterUid must above 0!!!");
				return;
			}
			if (!this.CheckInvitationValid())
			{
				Singleton<CUIManager>.GetInstance().CloseForm(uiEvent.m_srcFormScript);
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Refuse_Invitation_Confirm");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_Refuse_Invite, enUIEventID.None, new stUIEventParams
			{
				commonUInt64Param1 = num,
				commonBool = commonBool,
				tag = uiEvent.m_srcFormScript.GetSequence()
			}, false);
		}

		private bool CheckInvitationValid()
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(this.m_curViewInvitationInviterUid);
			if (guildMemberInfoByUid != null && guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid != this.m_curViewInvitationInviterUid && this.m_invitationInfos != null)
			{
				for (int i = 0; i < this.m_invitationInfos.get_Count(); i++)
				{
					if (this.m_curViewInvitationInviterUid == this.m_invitationInfos.get_Item(i).invitationInfo.ullUid)
					{
						this.m_invitationInfos.set_Item(i, new CGuildMatchSystem.stInvitationInfo
						{
							invitationInfo = this.m_invitationInfos.get_Item(i).invitationInfo
						});
						Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Team_Destroyed", true, 1.5f, null, new object[0]);
						return false;
					}
				}
			}
			return true;
		}

		private bool IsInvitationValid(ulong inviterUid)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(inviterUid);
			return guildMemberInfoByUid != null && guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid == inviterUid;
		}

		private void OnSignUpCardPos1Click(CUIEvent uiEvent)
		{
			this.SignUpCardPosClick(uiEvent, 3);
		}

		private void OnSignUpCardPos2Click(CUIEvent uiEvent)
		{
			this.SignUpCardPosClick(uiEvent, 4);
		}

		private void SignUpCardPosClick(CUIEvent uiEvent, int widgetId)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject widget = srcFormScript.GetWidget(widgetId);
			widget.CustomSetActive(!widget.activeSelf);
		}

		private void OnSignUpCardSelectPos1(CUIEvent uiEvent)
		{
			this.SignUpCardSelectPos(uiEvent, 8, ref this.m_signUpPos1);
		}

		private void OnSignUpCardSelectPos2(CUIEvent uiEvent)
		{
			this.SignUpCardSelectPos(uiEvent, 9, ref this.m_signUpPos2);
		}

		private void SignUpCardSelectPos(CUIEvent uiEvent, int widgetId, ref byte signUpPos)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			signUpPos = (byte)cUIListScript.GetSelectedIndex();
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Text component = srcFormScript.GetWidget(widgetId).GetComponent<Text>();
			Transform transform = cUIListScript.GetSelectedElement().transform.Find("Text");
			if (transform != null)
			{
				component.set_text(transform.GetComponent<Text>().get_text());
			}
			cUIListScript.gameObject.CustomSetActive(false);
		}

		private void OnSignUpListInviteBtnClick(CUIEvent uiEvent)
		{
			GameObject srcWidget = uiEvent.m_srcWidget;
			Transform parent = srcWidget.transform.parent;
			if (parent != null)
			{
				Transform transform = parent.Find("txtState");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
					Text component = transform.GetComponent<Text>();
					component.set_text("<color=#27b56a>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invite_State_Wait_For_Response") + "</color>");
				}
				Text component2 = parent.Find("txtUidData").GetComponent<Text>();
				ulong num;
				if (ulong.TryParse(component2.get_text(), ref num))
				{
					if (this.IsTeamFull(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Team_Member_Full", true, 1.5f, null, new object[0]);
						return;
					}
					GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(num);
					if (CGuildHelper.IsGuildMatchReachMatchCntLimit(guildMemberInfoByUid))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_SignUp_Invitee_Match_Cnt_Full", true, 1.5f, null, new object[0]);
						return;
					}
					if (CGuildMatchSystem.IsInGuildMatchJoinLimitTime(guildMemberInfoByUid))
					{
						Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Member_Join_Time_Limit_Join_Match_Tip", true, 1.5f, null, new object[0]);
						return;
					}
					srcWidget.CustomSetActive(false);
					this.RequestInviteGuildMatchMember(num);
				}
				else
				{
					DebugHelper.Assert(false, "Cannot parse string {0} to UInt64!!!", new object[]
					{
						component2.get_text()
					});
				}
			}
		}

		private void OnSignUpListElementEnabled(CUIEvent uiEvent)
		{
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidget == null || this.m_signUpInfos == null || this.m_signUpInfos.get_Count() == 0 || uiEvent.m_srcWidgetIndexInBelongedList < 0 || uiEvent.m_srcWidgetIndexInBelongedList >= this.m_signUpInfos.get_Count())
			{
				return;
			}
			Transform transform = srcWidget.transform;
			Text component = transform.Find("txtUidData").GetComponent<Text>();
			CUIHttpImageScript component2 = transform.Find("imgMemberIcon").GetComponent<CUIHttpImageScript>();
			Image component3 = component2.transform.Find("NobeIcon").GetComponent<Image>();
			Image component4 = component2.transform.Find("NobeImag").GetComponent<Image>();
			Text component5 = transform.Find("txtNickName").GetComponent<Text>();
			Text component6 = transform.Find("txtRankGrade").GetComponent<Text>();
			Text component7 = transform.Find("txtRankStar").GetComponent<Text>();
			Text component8 = transform.Find("txtLevel").GetComponent<Text>();
			Text component9 = transform.Find("txtMostUsedHero").GetComponent<Text>();
			Text component10 = transform.Find("txtSignUpPosition").GetComponent<Text>();
			Text component11 = transform.Find("txtMemo").GetComponent<Text>();
			Text component12 = transform.Find("txtState").GetComponent<Text>();
			CGuildMatchSystem.stSignUpInfo stSignUpInfo = this.m_signUpInfos.get_Item(uiEvent.m_srcWidgetIndexInBelongedList);
			if (stSignUpInfo.uid == 0uL)
			{
				DebugHelper.Assert(false, "signUpInfo.uid = 0 !!!");
				return;
			}
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(stSignUpInfo.uid);
			if (guildMemberInfoByUid == null)
			{
				return;
			}
			component.set_text(stSignUpInfo.uid.ToString());
			component2.SetImageUrl(CGuildHelper.GetHeadUrl(guildMemberInfoByUid.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component3, CGuildHelper.GetNobeLevel(guildMemberInfoByUid.stBriefInfo.uulUid, guildMemberInfoByUid.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component4, CGuildHelper.GetNobeHeadIconId(guildMemberInfoByUid.stBriefInfo.uulUid, guildMemberInfoByUid.stBriefInfo.stVip.headIconId));
			component5.set_text(guildMemberInfoByUid.stBriefInfo.sName);
			this.SetSignUpRankDesc(component6, component7, guildMemberInfoByUid);
			component8.set_text("Lv." + guildMemberInfoByUid.stBriefInfo.dwLevel);
			component9.set_text(string.Empty);
			if (stSignUpInfo.heroNames != null && stSignUpInfo.heroNames.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < stSignUpInfo.heroNames.Length; i++)
				{
					stringBuilder.Append(stSignUpInfo.heroNames[i]);
					stringBuilder.Append("(");
					stringBuilder.Append(stSignUpInfo.winPercents[i]);
					stringBuilder.Append("%)");
				}
				component9.set_text(stringBuilder.ToString());
			}
			else
			{
				component9.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_None"));
			}
			if (this.m_signUpPosTexts != null && this.m_signUpPosTexts.Length > 0)
			{
				component10.set_text(((stSignUpInfo.pos1 == 0) ? "--" : this.m_signUpPosTexts[(int)stSignUpInfo.pos1]) + " " + ((stSignUpInfo.pos2 == 0) ? "--" : this.m_signUpPosTexts[(int)stSignUpInfo.pos2]));
			}
			component11.set_text((!string.IsNullOrEmpty(stSignUpInfo.memo)) ? stSignUpInfo.memo : Singleton<CTextManager>.GetInstance().GetText("Common_None"));
			bool flag = (this.IsSelfBelongedTeamLeader() && this.IsInTeam(guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid, Singleton<CRoleInfoManager>.GetInstance().masterUUID)) || (!this.IsSelfBelongedTeamLeader() && stSignUpInfo.uid == Singleton<CRoleInfoManager>.GetInstance().masterUUID);
			this.SetSignUpStateColumn(transform, stSignUpInfo.uid, false);
			if (flag)
			{
				this.SetHighlightedText(new Text[]
				{
					component5,
					component6,
					component7,
					component8,
					component9,
					component10,
					component11,
					component12
				});
			}
		}

		private void SetHighlightedText(params Text[] txtComponents)
		{
			for (int i = 0; i < txtComponents.Length; i++)
			{
				txtComponents[i].set_text("<color=#e49316>" + txtComponents[i].get_text() + "</color>");
			}
		}

		private void RefreshSelfSignUpListInfo()
		{
			if (this.m_signUpInfos != null)
			{
				for (int i = 0; i < this.m_signUpInfos.get_Count(); i++)
				{
					if (this.m_signUpInfos.get_Item(i).uid == Singleton<CRoleInfoManager>.GetInstance().masterUUID)
					{
						CGuildMatchSystem.stSignUpInfo stSignUpInfo = default(CGuildMatchSystem.stSignUpInfo);
						stSignUpInfo.pos1 = this.m_signUpPos1;
						stSignUpInfo.pos2 = this.m_signUpPos2;
						stSignUpInfo.memo = ((this.m_signUpMemoText != null) ? this.m_signUpMemoText : string.Empty);
						stSignUpInfo.uid = this.m_signUpInfos.get_Item(i).uid;
						stSignUpInfo.heroNames = this.m_signUpInfos.get_Item(i).heroNames;
						stSignUpInfo.winPercents = this.m_signUpInfos.get_Item(i).winPercents;
						this.m_signUpInfos.set_Item(i, stSignUpInfo);
					}
				}
				if (this.m_signUpListForm != null)
				{
					CUIListScript component = this.m_signUpListForm.GetWidget(1).GetComponent<CUIListScript>();
					if (this.m_signUpInfos != null)
					{
						component.SetElementAmount(this.m_signUpInfos.get_Count());
					}
				}
			}
		}

		public void OpenInvitationForm(string content, COM_GUILDMATCH_INVITECARD_STATE state)
		{
			this.m_invitationForm = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchInvitationFormPath, false, true);
			if (this.m_invitationForm == null)
			{
				return;
			}
			Text component = this.m_invitationForm.GetWidget(0).GetComponent<Text>();
			component.set_text(content);
			this.RefreshInvitationStatePanel(state);
		}

		private void RefreshInvitationStatePanel(COM_GUILDMATCH_INVITECARD_STATE state)
		{
			if (this.m_invitationForm == null)
			{
				return;
			}
			GameObject widget = this.m_invitationForm.GetWidget(1);
			GameObject widget2 = this.m_invitationForm.GetWidget(2);
			GameObject widget3 = this.m_invitationForm.GetWidget(3);
			if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_NULL)
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
			}
			else if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_ACCEPT)
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(true);
				widget2.GetComponent<Text>().set_text("<color=#27b56a>" + Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invitation_Accepted_Tip") + "</color>");
			}
			else if (state == COM_GUILDMATCH_INVITECARD_STATE.COM_GUILDMATCH_INVITECARD_STATE_REFUSE)
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				widget3.CustomSetActive(false);
				widget2.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invitation_Rejected_Tip"));
			}
		}

		private void CloseInvitationForm()
		{
			if (this.m_invitationForm != null)
			{
				this.m_invitationForm.Close();
			}
		}

		private void RemoveHandledInvitation(ulong inviterUid)
		{
			if (this.m_invitationInfos != null)
			{
				for (int i = 0; i < this.m_invitationInfos.get_Count(); i++)
				{
					if (inviterUid == this.m_invitationInfos.get_Item(i).invitationInfo.ullUid)
					{
						this.m_invitationInfos.RemoveAt(i);
						break;
					}
				}
			}
			this.RefreshInvitationList();
		}

		public void OpenOnlineInvitationForm(ulong inviterUid)
		{
			if (inviterUid == 0uL)
			{
				return;
			}
			if (this.m_openedOnlineInvitationInviterUids != null && this.m_openedOnlineInvitationInviterUids.Contains(inviterUid))
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CGuildMatchSystem.GuildMatchOnlineInvitationFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			if (this.m_openedOnlineInvitationInviterUids == null)
			{
				this.m_openedOnlineInvitationInviterUids = new List<ulong>();
			}
			this.m_openedOnlineInvitationInviterUids.Add(inviterUid);
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(inviterUid);
			if (guildMemberInfoByUid == null)
			{
				return;
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_SignUp_Invitation_Content", new string[]
			{
				guildMemberInfoByUid.stBriefInfo.sName,
				this.GetGuildMatchOpenTimeDesc()
			});
			Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
			component.set_text(text);
			CUIEventScript component2 = cUIFormScript.GetWidget(2).GetComponent<CUIEventScript>();
			component2.m_onClickEventParams.commonUInt64Param1 = inviterUid;
			component2.m_onClickEventParams.commonBool = true;
			CUIEventScript component3 = cUIFormScript.GetWidget(1).GetComponent<CUIEventScript>();
			component3.m_onClickEventParams.commonUInt64Param1 = inviterUid;
			component3.m_onClickEventParams.commonBool = true;
			cUIFormScript.SetFormEventParams(enFormEventType.Close, new stUIEventParams
			{
				commonUInt64Param1 = inviterUid
			});
		}

		public void ShowAllUnhandledOnlineInvitation()
		{
			if (this.m_unhandledOnlineInvitationInviterUids != null)
			{
				for (int i = 0; i < this.m_unhandledOnlineInvitationInviterUids.get_Count(); i++)
				{
					this.OpenOnlineInvitationForm(this.m_unhandledOnlineInvitationInviterUids.get_Item(i));
				}
				this.m_unhandledOnlineInvitationInviterUids.Clear();
			}
		}

		private void RequestChangeGuildMatchLeader(ulong leaderUid, bool isAppoint)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5301u);
			cSPkg.stPkgData.stChgGuildMatchLeaderReq.ullUid = leaderUid;
			cSPkg.stPkgData.stChgGuildMatchLeaderReq.bAppoint = Convert.ToByte(isAppoint);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestInviteGuildMatchMember(ulong memberUid)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5303u);
			cSPkg.stPkgData.stInviteGuildMatchMemberReq.ullUid = memberUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestDealGuildMatchMemberInvite(ulong teamLeaderUid, bool isAgree)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5305u);
			cSPkg.stPkgData.stDealGuildMatchMemberInvite.ullTeamLeaderUid = teamLeaderUid;
			cSPkg.stPkgData.stDealGuildMatchMemberInvite.bAgree = Convert.ToByte(isAgree);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestKickGuildMatchMember(ulong memberUid)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5307u);
			cSPkg.stPkgData.stKickGuildMatchMemberReq.ullUid = memberUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestLeaveGuildMatchTeam()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5308u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestStartGuildMatch()
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5310u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetGuildMemberGameState()
		{
			if (this.m_guildMemberInviteList == null)
			{
				DebugHelper.Assert(false, "m_guildMemberInviteList is null!!!");
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2035u);
			CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq = cSPkg.stPkgData.stGetGuildMemberGameStateReq;
			int num = 0;
			for (int i = 0; i < this.m_guildMemberInviteList.Count; i++)
			{
				if (CGuildHelper.IsMemberOnline(this.m_guildMemberInviteList[i]))
				{
					stGetGuildMemberGameStateReq.MemberUid[num] = this.m_guildMemberInviteList[i].stBriefInfo.uulUid;
					num++;
				}
			}
			stGetGuildMemberGameStateReq.iMemberCnt = num;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestSetGuildMatchReady(bool isReady)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5311u);
			cSPkg.stPkgData.stSetGuildMatchReadyReq.bIsReady = Convert.ToByte(isReady);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestObGuildMatch(ulong obUid)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5317u);
			cSPkg.stPkgData.stOBGuildMatchReq.ullOBUid = obUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestGuildMatchRemind(ulong remindUid)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5322u);
			cSPkg.stPkgData.stGuildMatchRemindReq.ullRemindUid = remindUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetGuildMatchHistory()
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5319u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestGetGuildMatchSeasonRank()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.iStart = 1;
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 66;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestGetGuildMatchWeekRank()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.iStart = 1;
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 67;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestGuildOBCount()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5324u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGuildMatchSignUp(byte signUpPos1, byte signUpPos2, string memoText)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5295u);
			cSPkg.stPkgData.stGuildMatchSignUpReq.bPos1 = signUpPos1;
			cSPkg.stPkgData.stGuildMatchSignUpReq.bPos2 = signUpPos2;
			StringHelper.StringToUTF8Bytes(memoText, ref cSPkg.stPkgData.stGuildMatchSignUpReq.szBeiZhu);
			cSPkg.stPkgData.stGuildMatchSignUpReq.bHeroNum = (byte)this.GetSortedMostUsedHeroInfos(ref cSPkg.stPkgData.stGuildMatchSignUpReq.astHeroInfo);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void RequestModifyGuildMatchSignUp(byte signUpPos1, byte signUpPos2, string memoText)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5297u);
			cSPkg.stPkgData.stModGuildMatchSignUpReq.bPos1 = signUpPos1;
			cSPkg.stPkgData.stModGuildMatchSignUpReq.bPos2 = signUpPos2;
			StringHelper.StringToUTF8Bytes(memoText, ref cSPkg.stPkgData.stModGuildMatchSignUpReq.szBeiZhu);
			cSPkg.stPkgData.stModGuildMatchSignUpReq.bHeroNum = (byte)this.GetSortedMostUsedHeroInfos(ref cSPkg.stPkgData.stModGuildMatchSignUpReq.astHeroInfo);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void RequestGuildMatchGetInvitation()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5299u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetGuildMatchSignUpList()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5293u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnPlatformGroupStatusChange(CGuildSystem.enPlatformGroupStatus status, bool isSelfInGroup)
		{
			if (status != CGuildSystem.enPlatformGroupStatus.Bound || !isSelfInGroup)
			{
				return;
			}
			this.RefreshPlatformGroupInviteBtn();
		}

		private void RefreshPlatformGroupInviteBtn()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CGuildMatchSystem.GuildMatchFormPath);
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.GetWidget(4).GetComponent<CUIListScript>();
			CGuildMatchSystem.enRankTab selectedIndex = (CGuildMatchSystem.enRankTab)component.GetSelectedIndex();
			if (selectedIndex == CGuildMatchSystem.enRankTab.MemberInviteOrInvitation && this.IsSelfBelongedTeamLeader())
			{
				GameObject widget = form.GetWidget(29);
				widget.CustomSetActive(true);
				Button button = null;
				GameObject gameObject = null;
				CUITimerScript cUITimerScript = null;
				if (widget != null)
				{
					button = Utility.GetComponetInChild<Button>(widget, "Button");
					gameObject = Utility.FindChild(widget, "Button/CountDown");
					GameObject obj = Utility.FindChild(widget, "Button/IconQQ");
					GameObject obj2 = Utility.FindChild(widget, "Button/IconWeixin");
					if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
					{
						obj.CustomSetActive(true);
						obj2.CustomSetActive(false);
					}
					else
					{
						obj.CustomSetActive(false);
						obj2.CustomSetActive(true);
					}
					cUITimerScript = Utility.GetComponetInChild<CUITimerScript>(widget, "Button/CountDown/timer");
				}
				int sendGuildMatchInviteLeftTime = this.GetSendGuildMatchInviteLeftTime();
				Debug.Log(string.Format("**GuildPlatformGroup** RefreshPlatformGroupInviteBtn leftTime={0} inviteBtn={1} inviteCdGo={2} cdTimerScript={3}", new object[]
				{
					sendGuildMatchInviteLeftTime,
					(button != null) ? button.name : null,
					(gameObject != null) ? gameObject.name : null,
					cUITimerScript
				}));
				if (sendGuildMatchInviteLeftTime > 0 && gameObject != null && cUITimerScript != null)
				{
					CUICommonSystem.SetButtonEnableWithShader(button, false, false);
					gameObject.CustomSetActive(true);
					cUITimerScript.SetTotalTime((float)sendGuildMatchInviteLeftTime);
					cUITimerScript.ReStartTimer();
				}
				else
				{
					CUICommonSystem.SetButtonEnableWithShader(button, true, true);
					gameObject.CustomSetActive(false);
				}
			}
		}

		[MessageHandler(5302)]
		public static void ReceiveChangeGuildMatchLeaderNtf(CSPkg msg)
		{
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberInfo(msg.stPkgData.stChgGuildMatchLeaderNtf);
			Singleton<CGuildMatchSystem>.GetInstance().SetTeamInfo(msg.stPkgData.stChgGuildMatchLeaderNtf);
			bool flag2 = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			Singleton<CPlayerInfoSystem>.GetInstance().SetAppointMatchLeaderBtn();
			Singleton<CGuildInfoView>.GetInstance().RefreshMemberPanel();
			Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
			if (flag != flag2)
			{
				Singleton<CGuildMatchSystem>.GetInstance().RefreshRankTabList();
			}
			if (CGuildSystem.HasAppointMatchLeaderAuthority())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Appoint_Or_Leader_Success", true, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(5321)]
		public static void ReceiveChangeGuildMatchLeaderRsp(CSPkg msg)
		{
			SCPKG_CHG_GUILD_MATCH_LEADER_RSP stChgGuildMatchLeaderRsp = msg.stPkgData.stChgGuildMatchLeaderRsp;
			if (CGuildSystem.IsError((int)stChgGuildMatchLeaderRsp.bErrorCode))
			{
				return;
			}
		}

		[MessageHandler(5304)]
		public static void ReceiveInviteGuildMatchMemberNtf(CSPkg msg)
		{
			SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF stInviteGuildMatchMemberNtf = msg.stPkgData.stInviteGuildMatchMemberNtf;
			if (Singleton<CFriendContoller>.instance.model.IsBlack(stInviteGuildMatchMemberNtf.ullTeamLeaderUid, (uint)CGuildHelper.GetMemberLogicWorldId(stInviteGuildMatchMemberNtf.ullTeamLeaderUid)))
			{
				return;
			}
			CGuildMatchSystem instance = Singleton<CGuildMatchSystem>.GetInstance();
			if (!Utility.IsCanShowPrompt())
			{
				if (instance.m_unhandledOnlineInvitationInviterUids == null)
				{
					instance.m_unhandledOnlineInvitationInviterUids = new List<ulong>();
				}
				if (!instance.m_unhandledOnlineInvitationInviterUids.Contains(stInviteGuildMatchMemberNtf.ullTeamLeaderUid))
				{
					instance.m_unhandledOnlineInvitationInviterUids.Add(stInviteGuildMatchMemberNtf.ullTeamLeaderUid);
				}
				return;
			}
			instance.OpenOnlineInvitationForm(stInviteGuildMatchMemberNtf.ullTeamLeaderUid);
		}

		[MessageHandler(5306)]
		public static void ReceiveGuildMatchMemberInviteRsp(CSPkg msg)
		{
			SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP stGuildMatchMemberInviteRsp = msg.stPkgData.stGuildMatchMemberInviteRsp;
			if (stGuildMatchMemberInviteRsp.bErrorCode == 30 && Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID == stGuildMatchMemberInviteRsp.ullInviter)
			{
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(stGuildMatchMemberInviteRsp.ullInvitee);
				string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Invitee_Refuse", new string[]
				{
					guildMemberInfoByUid.stBriefInfo.sName
				});
				Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
			}
			else if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID == stGuildMatchMemberInviteRsp.ullInvitee)
			{
				if (stGuildMatchMemberInviteRsp.bErrorCode == 42)
				{
					Singleton<CGuildMatchSystem>.GetInstance().CloseInvitationForm();
					Singleton<CGuildMatchSystem>.GetInstance().RemoveHandledInvitation(stGuildMatchMemberInviteRsp.ullInviter);
				}
				else if (stGuildMatchMemberInviteRsp.bErrorCode == 0)
				{
					Singleton<CGuildMatchSystem>.GetInstance().RemoveHandledInvitation(stGuildMatchMemberInviteRsp.ullInviter);
				}
			}
			CGuildSystem.IsError((int)stGuildMatchMemberInviteRsp.bErrorCode);
		}

		[MessageHandler(5309)]
		public static void ReceiveGuildMatchMemberChangeNtf(CSPkg msg)
		{
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberInfo(msg.stPkgData.stGuildMatchMemberChgNtf);
			Singleton<CGuildMatchSystem>.GetInstance().SetTeamInfo(msg.stPkgData.stGuildMatchMemberChgNtf);
			bool flag2 = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			if (Singleton<CGuildMatchSystem>.GetInstance().IsNeedOpenGuildMatchForm(msg.stPkgData.stGuildMatchMemberChgNtf))
			{
				Singleton<CGuildMatchSystem>.GetInstance().OpenMatchForm(false);
			}
			else
			{
				Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
				Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchSignUpPanel();
				if (flag != flag2)
				{
					Singleton<CGuildMatchSystem>.GetInstance().RefreshRankTabList();
				}
			}
		}

		[MessageHandler(5312)]
		public static void ReceiveSetGuildMatchReadyRsp(CSPkg msg)
		{
			CGuildSystem.IsError((int)msg.stPkgData.stSetGuildMatchReadyRsp.bErrorCode);
		}

		[MessageHandler(5313)]
		public static void ReceiveSetGuildMatchReadyNtf(CSPkg msg)
		{
			Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberReadyState(msg.stPkgData.stSetGuildMatchReadyNtf);
			Singleton<CGuildMatchSystem>.GetInstance().SetTeamMemberReadyState(msg.stPkgData.stSetGuildMatchReadyNtf);
			Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
		}

		[MessageHandler(5314)]
		public static void ReceiveGuildMatchScoreChangeNtf(CSPkg msg)
		{
			Singleton<CGuildModel>.GetInstance().SetGuildMatchScore(msg.stPkgData.stGuildMatchScoreChgNtf);
			Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchScore();
		}

		[MessageHandler(5315)]
		public static void ReceiveStartGuildMatchRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CGuildSystem.IsError((int)msg.stPkgData.stStartGuildMatchRsp.bErrorCode);
		}

		[MessageHandler(5316)]
		public static void ReceiveGuildMatchObInfoChg(CSPkg msg)
		{
			GuildInfo currentGuildInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo;
			if (currentGuildInfo == null || currentGuildInfo.GuildMatchObInfos == null)
			{
				return;
			}
			COMDT_GUILD_MATCH_OB_INFO stChgInfo = msg.stPkgData.stGuildMatchOBInfoChg.stChgInfo;
			ulong ullUid = stChgInfo.ullUid;
			uint dwBeginTime = stChgInfo.dwBeginTime;
			bool flag = false;
			for (int i = currentGuildInfo.GuildMatchObInfos.Count - 1; i >= 0; i--)
			{
				if (currentGuildInfo.GuildMatchObInfos[i].ullUid == ullUid)
				{
					if (dwBeginTime > 0u)
					{
						currentGuildInfo.GuildMatchObInfos[i].dwBeginTime = dwBeginTime;
						currentGuildInfo.GuildMatchObInfos[i].dwOBCnt = stChgInfo.dwOBCnt;
						currentGuildInfo.GuildMatchObInfos[i].astHeroInfo = new COMDT_GUILD_MATCH_PLAYER_HERO[5];
						for (int j = 0; j < 5; j++)
						{
							currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j] = new COMDT_GUILD_MATCH_PLAYER_HERO();
							currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid = stChgInfo.astHeroInfo[j].ullUid;
							currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].dwHeroID = stChgInfo.astHeroInfo[j].dwHeroID;
						}
					}
					else
					{
						currentGuildInfo.GuildMatchObInfos.RemoveAt(i);
					}
					flag = true;
				}
			}
			if (!flag)
			{
				currentGuildInfo.GuildMatchObInfos.Add(stChgInfo);
			}
		}

		[MessageHandler(5318)]
		public static void ReceiveObGuildMatchRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stOBGuildMatchRsp.iResult == 0)
			{
				if (Singleton<WatchController>.GetInstance().StartObserve(msg.stPkgData.stOBGuildMatchRsp.stTgwinfo))
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				}
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stOBGuildMatchRsp.iResult), true, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(5323)]
		public static void ReceiveGuildMatchRemindNtf(CSPkg msg)
		{
			if (!Utility.IsCanShowPrompt())
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Guild_Match_Remind_Msg"), enUIEventID.Guild_Match_OpenMatchFormAndReadyGame, enUIEventID.None, false);
		}

		[MessageHandler(5320)]
		public static void ReceiveGetGuildMatchHistoryRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_GET_GUILD_MATCH_HISTORY_RSP stGetGuildMatchHistoryRsp = msg.stPkgData.stGetGuildMatchHistoryRsp;
			Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords = new COMDT_GUILD_MATCH_HISTORY_INFO[(int)stGetGuildMatchHistoryRsp.bMatchNum];
			for (int i = 0; i < Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords.Length; i++)
			{
				Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords[i] = stGetGuildMatchHistoryRsp.astMatchInfo[i];
			}
			Singleton<CGuildMatchSystem>.GetInstance().OpenMatchRecordForm();
		}

		[MessageHandler(5325)]
		public static void ReceiveGuildOBCountRsp(CSPkg msg)
		{
			Singleton<COBSystem>.GetInstance().SetGuildMatchOBCount(msg.stPkgData.stGetGuildMatchOBCntRsp);
		}

		[MessageHandler(5296)]
		public static void ReceiveGuildMatchSignUp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (CGuildSystem.IsError(msg.stPkgData.stGuildMatchSignUpRsp.iResult))
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_SignUp_Success_Tip", true, 1.5f, null, new object[0]);
			Singleton<CGuildMatchSystem>.GetInstance().m_isSignedUp = true;
			Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchSignUpPanel();
		}

		[MessageHandler(5298)]
		public static void ReceiveModifyGuildMatchSignUp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (CGuildSystem.IsError(msg.stPkgData.stModGuildMatchSignUpRsp.iResult))
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_SignUp_Modify_Success", true, 1.5f, null, new object[0]);
			Singleton<CGuildMatchSystem>.GetInstance().RefreshSelfSignUpListInfo();
		}

		[MessageHandler(5300)]
		public static void ReceiveGuildMatchGetInvitation(CSPkg msg)
		{
			CGuildMatchSystem instance = Singleton<CGuildMatchSystem>.GetInstance();
			if (instance.m_invitationInfos == null)
			{
				instance.m_invitationInfos = new List<CGuildMatchSystem.stInvitationInfo>();
			}
			else
			{
				instance.m_invitationInfos.Clear();
			}
			SCPKG_GUILD_MATCH_GETINVITE_RSP stGuildMatchGetInviteRsp = msg.stPkgData.stGuildMatchGetInviteRsp;
			for (int i = 0; i < (int)stGuildMatchGetInviteRsp.bInviteNum; i++)
			{
				CGuildMatchSystem.stInvitationInfo stInvitationInfo = new CGuildMatchSystem.stInvitationInfo
				{
					invitationInfo = stGuildMatchGetInviteRsp.astInviteList[i],
					isValid = instance.IsInvitationValid(stGuildMatchGetInviteRsp.astInviteList[i].ullUid)
				};
				instance.m_invitationInfos.Add(stInvitationInfo);
			}
			Singleton<CGuildMatchSystem>.GetInstance().RefreshInvitationList();
		}

		private bool IsHaveNewSignUpInfo(SCPKG_GUILD_MATCH_SIGNUPLIST_RSP rsp)
		{
			if ((this.m_signUpInfos == null || this.m_signUpInfos.get_Count() == 0) && rsp.bSingUpNum > 0)
			{
				return true;
			}
			if (this.m_signUpInfos == null)
			{
				return false;
			}
			for (int i = 0; i < (int)rsp.bSingUpNum; i++)
			{
				bool flag = true;
				for (int j = 0; j < this.m_signUpInfos.get_Count(); j++)
				{
					if (rsp.astSingUpList[i].ullUid == this.m_signUpInfos.get_Item(j).uid)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		[MessageHandler(5294)]
		public static void ReceiveGetGuildMatchSignUpList(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_GUILD_MATCH_SIGNUPLIST_RSP stGuildMatchSignUpListRsp = msg.stPkgData.stGuildMatchSignUpListRsp;
			CGuildMatchSystem instance = Singleton<CGuildMatchSystem>.GetInstance();
			instance.m_isHaveNewSignUpInfo = instance.IsHaveNewSignUpInfo(stGuildMatchSignUpListRsp);
			if (instance.m_isHaveNewSignUpInfo)
			{
				instance.RefreshGuildMatchSignUpPanelRedDot(null);
			}
			if (instance.m_signUpInfos == null)
			{
				instance.m_signUpInfos = new List<CGuildMatchSystem.stSignUpInfo>();
			}
			else
			{
				instance.m_signUpInfos.Clear();
			}
			for (int i = (int)(stGuildMatchSignUpListRsp.bSingUpNum - 1); i >= 0; i--)
			{
				CGuildMatchSystem.stSignUpInfo stSignUpInfo = default(CGuildMatchSystem.stSignUpInfo);
				CSDT_GUILDMATCH_SIGNUPINFO cSDT_GUILDMATCH_SIGNUPINFO = stGuildMatchSignUpListRsp.astSingUpList[i];
				stSignUpInfo.uid = cSDT_GUILDMATCH_SIGNUPINFO.ullUid;
				stSignUpInfo.pos1 = cSDT_GUILDMATCH_SIGNUPINFO.bPos1;
				stSignUpInfo.pos2 = cSDT_GUILDMATCH_SIGNUPINFO.bPos2;
				stSignUpInfo.memo = StringHelper.UTF8BytesToString(ref cSDT_GUILDMATCH_SIGNUPINFO.szBeiZhu);
				stSignUpInfo.heroNames = new string[(int)cSDT_GUILDMATCH_SIGNUPINFO.bHeroNum];
				stSignUpInfo.winPercents = new byte[(int)cSDT_GUILDMATCH_SIGNUPINFO.bHeroNum];
				for (int j = 0; j < (int)cSDT_GUILDMATCH_SIGNUPINFO.bHeroNum; j++)
				{
					stSignUpInfo.heroNames[j] = CHeroInfo.GetHeroName(cSDT_GUILDMATCH_SIGNUPINFO.astHeroInfo[j].dwHeroID);
					stSignUpInfo.winPercents[j] = (byte)(100u * cSDT_GUILDMATCH_SIGNUPINFO.astHeroInfo[j].dwGameWinNum / (cSDT_GUILDMATCH_SIGNUPINFO.astHeroInfo[j].dwGameWinNum + cSDT_GUILDMATCH_SIGNUPINFO.astHeroInfo[j].dwGameLoseNum));
				}
				instance.m_signUpInfos.Add(stSignUpInfo);
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(cSDT_GUILDMATCH_SIGNUPINFO.ullUid);
				if (guildMemberInfoByUid != null)
				{
					guildMemberInfoByUid.isGuildMatchSignedUp = true;
				}
			}
			Singleton<CGuildMatchSystem>.GetInstance().RefreshSignUpList();
		}

		[MessageHandler(5292)]
		public static void ReceiveGuildMatchSelfSignUpInfo(CSPkg msg)
		{
			Singleton<CGuildMatchSystem>.GetInstance().m_isSignedUp = true;
			SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP stGuildMatchSelfSignUpInfoRsp = msg.stPkgData.stGuildMatchSelfSignUpInfoRsp;
			Singleton<CGuildMatchSystem>.GetInstance().m_signUpPos1 = stGuildMatchSelfSignUpInfoRsp.bPos1;
			Singleton<CGuildMatchSystem>.GetInstance().m_signUpPos2 = stGuildMatchSelfSignUpInfoRsp.bPos2;
			Singleton<CGuildMatchSystem>.GetInstance().m_signUpMemoText = StringHelper.UTF8BytesToString(ref stGuildMatchSelfSignUpInfoRsp.szBeiZhu);
		}
	}
}

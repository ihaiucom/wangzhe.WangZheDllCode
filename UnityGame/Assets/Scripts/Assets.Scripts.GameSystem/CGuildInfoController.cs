using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CGuildInfoController : Singleton<CGuildInfoController>
	{
		private CGuildInfoView m_View;

		private CGuildModel m_Model;

		public override void Init()
		{
			base.Init();
			this.m_Model = Singleton<CGuildModel>.GetInstance();
			this.m_View = Singleton<CGuildInfoView>.GetInstance();
			Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo>("Receive_Guild_Info_Success", new Action<GuildInfo>(this.OnReceiveGuildInfoSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Receive_Guild_Info_Failed", new Action(this.OnReceiveGuildInfoFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo>("Guild_Create_Or_Add_Success", new Action<GuildInfo>(this.OnCreateOrAddSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_New_Applicant", new Action(this.OnNewApplicant));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint, int, int>("Request_Guild_Setting_Modify", new Action<uint, int, int>(this.OnRequestGuildSettingModify));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Setting_Modify_Success", new Action(this.OnGuildSettingModifySuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("Guild_Setting_Modify_Failed", new Action<uint>(this.OnGuildSettingModifyFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("Guild_Setting_Modify_Icon", new Action<uint>(this.OnGuildSettingModifyIcon));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Setting_Modify_Icon_Success", new Action(this.OnGuildSettingModifyIconSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<string>("Guild_Modify_Bulletin", new Action<string>(this.OnGuildModifyBulletin));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Modify_Bulletin_Success", new Action(this.OnGuildModifyBulletinSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Request_Extend_Member_Limit", new Action(this.OnGuildRequestExtendMemberLimit));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, byte>("Guild_Approve", new Action<ulong, byte>(this.OnGuildApprove));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_New_Member", new Action(this.OnNewMember));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit", new Action(this.OnGuildQuit));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit_Success", new Action(this.OnGuildQuitSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit_Failed", new Action(this.OnGuildQuitFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Member_Quit", new Action(this.OnMemberQuit));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Apply_Time_Up", new Action<ulong>(this.OnGuildApplyTimeUp));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, int>("Guild_Invite", new Action<ulong, int>(this.OnGuildInvite));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, int>("Guild_Recommend", new Action<ulong, int>(this.OnGuildRecommend));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, int>("Guild_Reject_Recommend", new Action<ulong, int>(this.OnGuildRejectRecommend));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, byte, ulong, string>("Guild_Position_Appoint", new Action<ulong, byte, ulong, string>(this.OnGuildPositionAppoint));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Position_Confirm_Fire_Member", new Action<ulong>(this.OnGuildPositionConfirmFireMember));
			Singleton<EventRouter>.GetInstance().AddEventHandler<enGuildRankpointRankListType>("Guild_Request_Rankpoint_Rank_List", new Action<enGuildRankpointRankListType>(this.OnGuildRankpointRequestRankList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int, int>("Guild_Preview_Get_Ranking_List", new Action<int, int>(this.OnGuildPreviewRequestRankingList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ListView<GuildInfo>, bool>("Receive_Guild_List_Success", new Action<ListView<GuildInfo>, bool>(this.OnReceiveGuildListSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo, int>("Receive_Guild_Search_Success", new Action<GuildInfo, int>(this.OnReceiveGuildSearchSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Accept_Invite", new Action(this.OnAcceptInvite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Hyperlink_Search_Guild, new CUIEventManager.OnUIEventHandler(this.OnHyperLinkSearchGuild));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Hyperlink_Search_PrepareGuild, new CUIEventManager.OnUIEventHandler(this.OnHyperLinkSearchPrepareGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Sign", new Action(this.OnGuildSignIn));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_PlatformGroup_Refresh_Group_Panel", new Action(this.OnRefreshPlatformGroupPanel));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_PlatformGroup_Request_Group_Guild_Id", new Action(this.OnRequestGroupGuildId));
			Singleton<EventRouter>.GetInstance().AddEventHandler<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", new Action<string>(this.OnSetGuildGroupOpenId));
			Singleton<EventRouter>.GetInstance().AddEventHandler("MasterPvpLevelChanged", new Action(this.OnMasterPvpLevelChanged));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_GUILD_NAME_CHANGE, new Action(this.OnGuildNameChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(this.OnGlobalRefreshTime));
		}

		public void OpenForm()
		{
			this.m_View.OpenForm();
		}

		public void CloseForm()
		{
			this.m_View.CloseForm();
		}

		public void OnReceiveGuildInfoFailed()
		{
		}

		public void OnReceiveGuildInfoSuccess(GuildInfo info)
		{
			this.m_Model.CurrentGuildInfo = info;
			Singleton<CGuildMatchSystem>.GetInstance().CreateGuildMatchAllTeams();
		}

		public void OnRequestGuildSettingModify(uint mask, int levelLimit, int gradeLimit)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			this.RequestGuildSettingModify(mask, levelLimit, gradeLimit);
		}

		public void OnGuildSettingModifySuccess()
		{
		}

		public void OnGuildSettingModifyFailed(uint mask)
		{
		}

		private void OnGuildSettingModifyIcon(uint iconId)
		{
			this.RequestChgGuildHeadIdReq(iconId);
		}

		private void OnGuildSettingModifyIconSuccess()
		{
			this.m_View.RefreshInfoPanelGuildIcon();
		}

		private void OnGuildModifyBulletin(string bulletinText)
		{
			this.RequestChgGuildNotice(bulletinText);
		}

		private void OnGuildModifyBulletinSuccess()
		{
			this.m_View.RefreshInfoPanelGuildBulletin();
		}

		private void OnGuildRequestExtendMemberLimit()
		{
			this.RequestUpgradeGuildByDianQuan((uint)this.m_Model.CurrentGuildInfo.briefInfo.bLevel);
		}

		public void OnCreateOrAddSuccess(GuildInfo info)
		{
			this.m_Model.CurrentGuildInfo = info;
			if (Singleton<CGuildListView>.GetInstance().IsShow())
			{
				Singleton<CGuildListView>.GetInstance().CloseForm();
				this.m_View.OpenForm();
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab");
			if (form != null)
			{
				form.Close();
				this.m_View.OpenForm();
			}
			Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Success_Tip", new string[]
			{
				info.briefInfo.sName
			}), false, 1.5f, null, new object[0]);
			Singleton<CGuildMatchSystem>.GetInstance().CreateGuildMatchAllTeams();
		}

		public void OnNewApplicant()
		{
			this.m_View.RefreshApplyListPanel();
		}

		public void OnGuildApprove(ulong uulUid, byte result)
		{
			stApplicantInfo applicantByUid = this.m_Model.GetApplicantByUid(uulUid);
			COMDT_GUILD_MEMBER_BRIEF_INFO cOMDT_GUILD_MEMBER_BRIEF_INFO = new COMDT_GUILD_MEMBER_BRIEF_INFO();
			if (applicantByUid.stBriefInfo.uulUid == 0uL)
			{
				this.m_View.RefreshApplyListPanel();
			}
			else
			{
				this.m_Model.RemoveApplicant(applicantByUid.stBriefInfo.uulUid);
				cOMDT_GUILD_MEMBER_BRIEF_INFO.dwGameEntity = applicantByUid.stBriefInfo.dwGameEntity;
				StringHelper.StringToUTF8Bytes(applicantByUid.stBriefInfo.szHeadUrl, ref cOMDT_GUILD_MEMBER_BRIEF_INFO.szHeadUrl);
				cOMDT_GUILD_MEMBER_BRIEF_INFO.dwLevel = applicantByUid.stBriefInfo.dwLevel;
				cOMDT_GUILD_MEMBER_BRIEF_INFO.dwAbility = applicantByUid.stBriefInfo.dwAbility;
				cOMDT_GUILD_MEMBER_BRIEF_INFO.iLogicWorldID = applicantByUid.stBriefInfo.dwLogicWorldId;
				StringHelper.StringToUTF8Bytes(applicantByUid.stBriefInfo.sName, ref cOMDT_GUILD_MEMBER_BRIEF_INFO.szName);
				cOMDT_GUILD_MEMBER_BRIEF_INFO.ullUid = applicantByUid.stBriefInfo.uulUid;
				cOMDT_GUILD_MEMBER_BRIEF_INFO.stVip.dwScore = applicantByUid.stBriefInfo.stVip.score;
				cOMDT_GUILD_MEMBER_BRIEF_INFO.stVip.dwCurLevel = applicantByUid.stBriefInfo.stVip.level;
				cOMDT_GUILD_MEMBER_BRIEF_INFO.stVip.dwHeadIconId = applicantByUid.stBriefInfo.stVip.headIconId;
				this.GuildApprove(cOMDT_GUILD_MEMBER_BRIEF_INFO, result);
			}
		}

		public void OnNewMember()
		{
			this.m_View.RefreshInfoPanel();
			this.m_View.RefreshMemberPanel();
		}

		public void OnMemberQuit()
		{
			this.m_View.RefreshInfoPanel();
			this.m_View.RefreshMemberPanel();
		}

		public void OnRequestApplyList()
		{
			this.RequestApplyList(0);
		}

		public void OnReceiveApplyListSuccess(List<stApplicantInfo> applicantList)
		{
			this.m_Model.AddApplicants(applicantList);
			this.m_Model.m_isApplyListReceived = true;
			if (this.m_Model.m_isRecommendListReceived)
			{
				this.m_View.RefreshApplyListPanel();
				this.m_Model.m_isApplyListReceived = false;
				this.m_Model.m_isRecommendListReceived = false;
			}
		}

		public void OnGuildQuit()
		{
			this.m_Model.SetPlayerGuildStateToTemp();
			this.RequestQuitGuild();
		}

		public void OnGuildQuitSuccess()
		{
			this.m_View.CloseForm();
		}

		public void OnGuildQuitFailed()
		{
		}

		public void OnGuildApplyTimeUp(ulong uuid)
		{
			this.m_Model.RemoveApplicant(uuid);
		}

		public void OnGuildInvite(ulong uuid, int logicWorldId)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			this.InviteFriend(this.m_Model.CurrentGuildInfo.briefInfo.sName, uuid, logicWorldId);
		}

		public void OnGuildRecommend(ulong uuid, int logicWorldId)
		{
			this.SendRecommendReq(uuid, logicWorldId);
		}

		private void SendRecommendReq(ulong uuid, int logicWorldId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2234u);
			cSPkg.stPkgData.stGuildRecommendReq.ullAcntUid = uuid;
			cSPkg.stPkgData.stGuildRecommendReq.iAcntLogicWorldID = logicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void OnReceiveRecommendListSuccess(List<stRecommendInfo> recommendList)
		{
			this.m_Model.AddRecommendInfoList(recommendList);
			this.m_Model.m_isRecommendListReceived = true;
			if (this.m_Model.m_isApplyListReceived)
			{
				this.m_View.RefreshApplyListPanel();
				this.m_Model.m_isApplyListReceived = false;
				this.m_Model.m_isRecommendListReceived = false;
			}
		}

		public void OnGuildRejectRecommend(ulong uuid, int logicWorldId)
		{
			this.SendRejectRecommend(uuid, logicWorldId);
			this.m_Model.RemoveRecommendInfo(uuid);
			this.m_View.RefreshApplyListPanel();
		}

		private void SendRejectRecommend(ulong uuid, int logicWorldId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2239u);
			cSPkg.stPkgData.stRejectGuildRecommend.ullPlayerUid = uuid;
			cSPkg.stPkgData.stRejectGuildRecommend.iPlayerLogicWorldID = logicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void OnGuildPositionAppoint(ulong uid, byte position, ulong replaceUid, string pwd)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form != null)
			{
				form.Close();
			}
			this.RequestGuildAppointPosition(uid, position, replaceUid, pwd);
		}

		public void OnGuildPositionConfirmFireMember(ulong uid)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form != null)
			{
				form.Close();
			}
			this.RequestGuildFireMember(uid);
		}

		public void OnGuildRankpointRequestRankList(enGuildRankpointRankListType rankListType)
		{
			if (CGuildHelper.IsNeedRequestNewRankpoinRank(rankListType))
			{
				switch (rankListType)
				{
				case enGuildRankpointRankListType.CurrentWeek:
					this.RequestGetRankpointWeekRank(0);
					break;
				case enGuildRankpointRankListType.LastWeek:
					this.RequestGetRankpointWeekRank(1);
					break;
				case enGuildRankpointRankListType.SeasonSelf:
					this.RequestGetRankpointSeasonRankBySpecialScore();
					this.RequestGetPlayerGuildRankInfo();
					break;
				case enGuildRankpointRankListType.SeasonBest:
					this.RequestGetRankpointSeasonRank();
					this.RequestGetPlayerGuildRankInfo();
					break;
				}
			}
		}

		private void OnGuildPreviewRequestRankingList(int start, int limit)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 3;
			cSPkg.stPkgData.stGetRankingListReq.iImageFlag = 0;
			cSPkg.stPkgData.stGetRankingListReq.iStart = start;
			cSPkg.stPkgData.stGetRankingListReq.iLimit = limit;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnReceiveGuildListSuccess(ListView<GuildInfo> guildList, bool firstPage)
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			if (firstPage)
			{
				this.m_Model.ClearGuildInfoList();
			}
			this.m_Model.AddGuildInfoList(guildList);
			this.m_View.OpenGuildPreviewForm(false);
		}

		private void OnReceiveGuildSearchSuccess(GuildInfo info, int searchType)
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			this.m_Model.ClearGuildInfoList();
			this.m_Model.AddGuildInfo(info);
			this.m_View.RefreshPreviewForm(true);
		}

		public void OnHyperLinkSearchGuild(CUIEvent uiEvent)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Normal_Guild", true, 1.5f, null, new object[0]);
				return;
			}
			if (Singleton<CGuildSystem>.GetInstance().IsInPrepareGuild())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Prepare_Guild", true, 1.5f, null, new object[0]);
				return;
			}
			this.m_Model.m_InvitePlayerUuid = uiEvent.m_eventParams.commonUInt64Param2;
			this.m_Model.m_InviteGuildUuid = uiEvent.m_eventParams.commonUInt64Param1;
			this.m_Model.m_InviteGuildLogicWorldId = uiEvent.m_eventParams.tag;
			Singleton<CGuildSystem>.GetInstance().SearchGuild(this.m_Model.m_InviteGuildUuid, this.m_Model.m_InviteGuildLogicWorldId, string.Empty, 1, false);
		}

		public void OnHyperLinkSearchPrepareGuild(CUIEvent uiEvent)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Normal_Guild", true, 1.5f, null, new object[0]);
				return;
			}
			if (Singleton<CGuildSystem>.GetInstance().IsInPrepareGuild())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Prepare_Guild", true, 1.5f, null, new object[0]);
				return;
			}
			this.m_Model.m_InvitePlayerUuid = uiEvent.m_eventParams.commonUInt64Param2;
			this.m_Model.m_InviteGuildUuid = uiEvent.m_eventParams.commonUInt64Param1;
			Singleton<CGuildSystem>.GetInstance().SearchGuild(this.m_Model.m_InviteGuildUuid, this.m_Model.m_InviteGuildLogicWorldId, string.Empty, 1, true);
		}

		public void OnAcceptInvite()
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_No_Guild_Can_Accept_Invite_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			if (CGuildHelper.IsInLastQuitGuildCd())
			{
				return;
			}
			if (this.m_Model.m_InviteGuildUuid != 0uL)
			{
				this.RequestDealInviteReq(1);
			}
		}

		private void OnMasterPvpLevelChanged()
		{
			if (this.m_Model != null && this.m_Model.CurrentGuildInfo != null && this.m_Model.CurrentGuildInfo.listMemInfo != null)
			{
				for (int i = 0; i < this.m_Model.CurrentGuildInfo.listMemInfo.Count; i++)
				{
					if (this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
					{
						this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.dwLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
						return;
					}
				}
			}
		}

		private void OnGuildNameChange()
		{
			this.m_Model.CurrentGuildInfo.briefInfo.sName = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name;
			this.m_View.RefreshGuildName();
		}

		private void OnGlobalRefreshTime()
		{
			this.ChangeGuildSignState(false);
			CGuildHelper.SetSendGuildMailCnt(0);
		}

		private void OnGuildSignIn()
		{
			this.RequestGuildSignIn();
		}

		public void ChangeGuildSignState(bool isSigned)
		{
			CGuildHelper.SetPlayerSigned(isSigned);
			if (this.m_View != null)
			{
				this.m_View.RefreshInfoPanelSignBtn();
			}
		}

		private void OnRefreshPlatformGroupPanel()
		{
			if (this.m_Model.CurrentGuildInfo.groupGuildId == 0u)
			{
				this.m_Model.SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus.UnBound, false);
			}
			else
			{
				this.m_View.QueryPlatformGroupInfo(0);
			}
		}

		private void OnRequestGroupGuildId()
		{
			this.RequestGroupGuildId();
		}

		private void OnSetGuildGroupOpenId(string groupOpenId)
		{
			this.RequestSetGuildGroupOpenId(groupOpenId);
		}

		public void SendPlatformGroupLog(CGuildSystem.enPlatformGroupStatus status, string openid)
		{
			CSPkg cSPkg = null;
			if (status != CGuildSystem.enPlatformGroupStatus.Bound)
			{
				if (status == CGuildSystem.enPlatformGroupStatus.UnBound)
				{
					cSPkg = NetworkModule.CreateDefaultCSPKG(2292u);
					StringHelper.StringToUTF8Bytes(openid, ref cSPkg.stPkgData.stGuildUnBindQunReq.szGroupOpenID);
				}
			}
			else
			{
				cSPkg = NetworkModule.CreateDefaultCSPKG(2291u);
				StringHelper.StringToUTF8Bytes(openid, ref cSPkg.stPkgData.stGuildBindQunReq.szGroupOpenID);
			}
			if (cSPkg != null)
			{
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		private void InviteFriend(string name, ulong uuid, int logicWorldId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2229u);
			cSPkg.stPkgData.stGuildInviteReq.ullBeInviteUid = uuid;
			cSPkg.stPkgData.stGuildInviteReq.iBeInviteLogicWorldID = logicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void RequestGuildInfo()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2205u);
			cSPkg.stPkgData.stGetGuildInfoReq.ullGuildID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGuildSettingModify(uint mask, int levelLimit, int gradeLimit)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2217u);
			cSPkg.stPkgData.stModifyGuildSettingReq.dwSettingMask = mask;
			cSPkg.stPkgData.stModifyGuildSettingReq.bLimitLevel = (byte)levelLimit;
			cSPkg.stPkgData.stModifyGuildSettingReq.bLimitGrade = (byte)gradeLimit;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void GuildApprove(COMDT_GUILD_MEMBER_BRIEF_INFO info, byte result)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2225u);
			cSPkg.stPkgData.stApproveJoinGuildApply.stApplyInfo = info;
			cSPkg.stPkgData.stApproveJoinGuildApply.bAgree = result;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			this.m_View.RefreshApplyListPanel();
		}

		public void RequestApplyList(int page)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2219u);
			cSPkg.stPkgData.stGetGuildApplyListReq.bPageId = (byte)page;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestQuitGuild()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2226u);
			cSPkg.stPkgData.stQuitGuildReq.bBlank = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGuildAppointPosition(ulong uid, byte position, ulong replaceUid, string pwd)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2249u);
			cSPkg.stPkgData.stAppointPositionReq.ullUid = uid;
			cSPkg.stPkgData.stAppointPositionReq.bPosition = position;
			cSPkg.stPkgData.stAppointPositionReq.ullReplaceUid = replaceUid;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stAppointPositionReq.szPswdInfo);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGuildFireMember(ulong uid)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2252u);
			cSPkg.stPkgData.stFireGuildMemberReq.ullUid = uid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetRankpointWeekRank(int imageFlag)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 4;
			cSPkg.stPkgData.stGetRankingListReq.iStart = 1;
			cSPkg.stPkgData.stGetRankingListReq.iLimit = 100;
			cSPkg.stPkgData.stGetRankingListReq.iImageFlag = imageFlag;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetRankpointSeasonRank()
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 16;
			cSPkg.stPkgData.stGetRankingListReq.iStart = 1;
			cSPkg.stPkgData.stGetRankingListReq.iLimit = 100;
			cSPkg.stPkgData.stGetRankingListReq.iImageFlag = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetRankpointSeasonRankBySpecialScore()
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2613u);
			cSPkg.stPkgData.stGetRankListBySpecialScoreReq.bNumberType = 16;
			cSPkg.stPkgData.stGetRankListBySpecialScoreReq.iScore = (int)this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGetPlayerGuildRankInfo()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2615u);
			cSPkg.stPkgData.stGetSpecialGuildRankInfoReq.bNumberType = 16;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestChgGuildHeadIdReq(uint guildHeadId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2272u);
			cSPkg.stPkgData.stChgGuildHeadIDReq.dwHeadID = guildHeadId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestChgGuildNotice(string bulletinText)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2274u);
			StringHelper.StringToUTF8Bytes(bulletinText, ref cSPkg.stPkgData.stChgGuildNoticeReq.szNotice);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestDealInviteReq(byte agree)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2233u);
			cSPkg.stPkgData.stDealGuildInvite.bAgree = agree;
			cSPkg.stPkgData.stDealGuildInvite.ullInviteUid = this.m_Model.m_InvitePlayerUuid;
			cSPkg.stPkgData.stDealGuildInvite.ullGuildID = this.m_Model.m_InviteGuildUuid;
			cSPkg.stPkgData.stDealGuildInvite.iGuildLogicWorldID = this.m_Model.m_InviteGuildLogicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestUpgradeGuildByDianQuan(uint curGuildLevel)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2276u);
			cSPkg.stPkgData.stUpgradeGuildByCouponsReq.dwCurLevel = curGuildLevel;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void RequestGuildSignIn()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2278u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void ReqSendGuildMail(string title, string content)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1430u);
			StringHelper.StringToUTF8Bytes(title, ref cSPkg.stPkgData.stSendGuildMailReq.szSubject);
			StringHelper.StringToUTF8Bytes(content, ref cSPkg.stPkgData.stSendGuildMailReq.szContent);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void RequestGetGuildEvent()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2285u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void RequestSendGuildRecruitReq()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2287u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void RequestGroupGuildId()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2281u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			Debug.Log("Request 32 bit guild id");
		}

		private void RequestSetGuildGroupOpenId(string groupOpenId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2283u);
			StringHelper.StringToUTF8Bytes(groupOpenId, ref cSPkg.stPkgData.stSetGuildGroupOpenIDReq.szGroupOpenID);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			Debug.Log("Send groupOpenId to server, groupOpenId=" + groupOpenId);
		}
	}
}

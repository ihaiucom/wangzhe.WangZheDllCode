using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CGuildListController : Singleton<CGuildListController>
	{
		private CGuildListView m_View;

		private CGuildModel m_Model;

		public override void Init()
		{
			base.Init();
			this.m_Model = Singleton<CGuildModel>.GetInstance();
			this.m_View = Singleton<CGuildListView>.GetInstance();
			Singleton<EventRouter>.GetInstance().AddEventHandler<int, int>("Request_Guild_List", new Action<int, int>(this.OnRequestGuildList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ListView<GuildInfo>, bool>("Receive_Guild_List_Success", new Action<ListView<GuildInfo>, bool>(this.OnReceiveGuildListSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int>("Request_PrepareGuild_List", new Action<int>(this.OnRequestPrepareGuildList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ListView<PrepareGuildInfo>, uint, byte, byte>("Receive_PrepareGuild_List_Success", new Action<ListView<PrepareGuildInfo>, uint, byte, byte>(this.OnReceivePrepareGuildList));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Request_PrepareGuild_Info", new Action(this.OnRequestPrepareGuldInfo));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo>("Receive_PrepareGuild_Info_Success", new Action<PrepareGuildInfo>(this.OnRequestPrepareGuldInfoSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Receive_PrepareGuild_Info_Failed", new Action(this.OnRequestPrepareGuldInfoFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler<stPrepareGuildCreateInfo>("PrepareGuild_Create", new Action<stPrepareGuildCreateInfo>(this.OnRequestCreatePrepareGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo>("Receive_PrepareGuild_Create_Success", new Action<PrepareGuildInfo>(this.OnReceivePrepareGuildCreateSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Receive_PrepareGuild_Create_Failed", new Action(this.OnReceivePrepareGuildCreateFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo>("Request_Apply_Guild_Join", new Action<GuildInfo>(this.OnRequestApplyJoinGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler<stAppliedGuildInfo>("Receive_Apply_Guild_Join_Success", new Action<stAppliedGuildInfo>(this.OnReceiveApplyJoinGuildSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<stAppliedGuildInfo>("Receive_Apply_Guild_Join_Failed", new Action<stAppliedGuildInfo>(this.OnReceiveApplyJoinGuildFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo>("PrepareGuild_Join", new Action<PrepareGuildInfo>(this.OnRequestJoinPrepareGuild));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo>("Receive_PrepareGuild_Join_Success", new Action<PrepareGuildInfo>(this.OnReceivePrepareGuildJoinSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo>("Receive_PrepareGuild_Join_Rsp", new Action<PrepareGuildInfo>(this.OnReceivePrepareGuildJoinFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit_Success", new Action(this.OnGuildQuitSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo, int>("Receive_Guild_Search_Success", new Action<GuildInfo, int>(this.OnReceiveGuildSearchSuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PrepareGuildInfo, int>("Receive_Search_Prepare_Guild_Success", new Action<PrepareGuildInfo, int>(this.OnReceiveSearchPrepareGuildSuccess));
		}

		private void OnRequestGuildList(int start, int limit)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = 3;
			cSPkg.stPkgData.stGetRankingListReq.iImageFlag = 0;
			cSPkg.stPkgData.stGetRankingListReq.iStart = start;
			cSPkg.stPkgData.stGetRankingListReq.iLimit = limit;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void OnRequestPrepareGuildList(int pageId)
		{
			this.RequestPrepareGuildList(pageId);
		}

		public void OnRequestApplyJoinGuild(GuildInfo info)
		{
			this.m_Model.SetPlayerGuildStateToTemp();
			this.RequestApplyJoinGuild(info.briefInfo.uulUid, info.briefInfo.LogicWorldId, false);
		}

		public void OnRequestCreatePrepareGuild(stPrepareGuildCreateInfo info)
		{
			this.m_Model.SetPlayerGuildStateToTemp();
			this.RequestCreatePrepareGuild(info);
		}

		public void OnRequestJoinPrepareGuild(PrepareGuildInfo info)
		{
			this.m_Model.SetPlayerGuildStateToTemp();
			this.RequestJoinPrepareGuild(info);
		}

		public void OnRequestPrepareGuldInfo()
		{
			this.RequestPrepareGuildInfo();
		}

		public void OnGuildQuitSuccess()
		{
			this.m_View.OpenForm(CGuildListView.Tab.None, true);
		}

		public void OnGuild_Join()
		{
		}

		public void OnGuild_Join_Failed()
		{
		}

		public void OpenForm()
		{
			this.m_View.OpenForm(CGuildListView.Tab.None, true);
		}

		public void CloseForm()
		{
			this.m_View.CloseForm();
		}

		private void OnReceiveGuildListSuccess(ListView<GuildInfo> guildList, bool firstPage)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			if (firstPage)
			{
				this.m_Model.ClearGuildInfoList();
			}
			this.m_Model.AddGuildInfoList(guildList);
			this.m_View.RefreshGuildListPanel(false);
		}

		private void RequestPrepareGuildList(int page)
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2203u);
			cSPkg.stPkgData.stGetPrepareGuildListReq.bPageID = (byte)page;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnReceivePrepareGuildList(ListView<PrepareGuildInfo> guildList, uint totalCnt, byte pageId, byte thisCnt)
		{
			if (pageId == 0)
			{
				this.m_Model.ClearPrepareGuildInfoList();
			}
			this.m_Model.AddPrepareGuildInfoList(guildList);
			if (CGuildHelper.IsLastPage((int)pageId, totalCnt, 10))
			{
				this.m_View.RefreshPrepareGuildPanel(false, pageId, true);
			}
			else
			{
				this.m_View.RefreshPrepareGuildPanel(false, pageId, false);
			}
		}

		public void RequestApplyJoinGuild(ulong guildId, int guildLogicWorldId, bool isRecruit)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2221u);
			cSPkg.stPkgData.stApplyJoinGuildReq.ullGuildID = guildId;
			cSPkg.stPkgData.stApplyJoinGuildReq.iGuildLogicWorldID = guildLogicWorldId;
			cSPkg.stPkgData.stApplyJoinGuildReq.bIsZhaomu = Convert.ToByte(isRecruit);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void OnReceiveApplyJoinGuildSuccess(stAppliedGuildInfo info)
		{
			Singleton<CUIManager>.GetInstance().OpenTips("Guild_Send_Apply_Success_Tip", true, 1.5f, null, new object[0]);
			this.m_View.RefreshGuildListPanel(false);
		}

		public void OnReceiveApplyJoinGuildFailed(stAppliedGuildInfo info)
		{
			this.m_View.RefreshGuildListPanel(false);
		}

		private void RequestJoinPrepareGuild(PrepareGuildInfo info)
		{
			if (info == null)
			{
				DebugHelper.Assert(false, "CGuildListController.RequestJoinPrepareGuild(): info is null!!!");
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2211u);
			cSPkg.stPkgData.stJoinPrepareGuildReq.ullGuildID = info.stBriefInfo.uulUid;
			cSPkg.stPkgData.stJoinPrepareGuildReq.iGuildLogicWorldID = info.stBriefInfo.dwLogicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnReceivePrepareGuildJoinSuccess(PrepareGuildInfo info)
		{
			this.m_View.RefreshPrepareGuildPanel(false, 0, false);
		}

		private void OnReceivePrepareGuildJoinFailed(PrepareGuildInfo info)
		{
			this.m_View.RefreshPrepareGuildPanel(false, 0, false);
		}

		private void RequestCreatePrepareGuild(stPrepareGuildCreateInfo info)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2209u);
			StringHelper.StringToUTF8Bytes(info.sName, ref cSPkg.stPkgData.stCreateGuildReq.szName);
			StringHelper.StringToUTF8Bytes(info.sBulletin, ref cSPkg.stPkgData.stCreateGuildReq.szNotice);
			cSPkg.stPkgData.stCreateGuildReq.bIsOnlyFriend = Convert.ToByte(info.isOnlyFriend);
			cSPkg.stPkgData.stCreateGuildReq.dwHeadID = info.dwHeadId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnReceivePrepareGuildCreateSuccess(PrepareGuildInfo info)
		{
			Singleton<CUIManager>.GetInstance().OpenTips("Guild_Prepare_Guild_Created_Tip", true, 1.5f, null, new object[0]);
			this.m_View.SelectTabElement(CGuildListView.Tab.PrepareGuild, true);
		}

		private void OnReceivePrepareGuildCreateFailed()
		{
		}

		private void RequestPrepareGuildInfo()
		{
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2207u);
			cSPkg.stPkgData.stGetPrepareGuildInfoReq.ullGuildID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		private void OnRequestPrepareGuldInfoSuccess(PrepareGuildInfo info)
		{
			this.m_View.RefreshPrepareGuildPanel(false, 0, false);
		}

		private void OnRequestPrepareGuldInfoFailed()
		{
			this.m_View.RefreshPrepareGuildPanel(false, 0, false);
		}

		private void OnReceiveSearchPrepareGuildSuccess(PrepareGuildInfo prepareGuildInfo, int searchType)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			this.m_Model.ClearPrepareGuildInfoList();
			this.m_Model.AddPrepareGuildInfo(prepareGuildInfo);
			if (searchType == 0)
			{
				this.m_View.RefreshPrepareGuildPanel(true, 0, true);
			}
			else if (searchType == 1)
			{
				this.m_View.OpenForm(CGuildListView.Tab.PrepareGuild, false);
				this.m_View.RefreshPrepareGuildPanel(true, 0, true);
			}
		}

		private void OnReceiveGuildSearchSuccess(GuildInfo guildInfo, int searchType)
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			this.m_Model.ClearGuildInfoList();
			this.m_Model.AddGuildInfo(guildInfo);
			if (searchType == 0)
			{
				this.m_View.RefreshGuildListPanel(true);
			}
			else if (searchType == 1)
			{
				Singleton<CGuildInfoView>.GetInstance().OpenGuildPreviewForm(true);
			}
		}
	}
}

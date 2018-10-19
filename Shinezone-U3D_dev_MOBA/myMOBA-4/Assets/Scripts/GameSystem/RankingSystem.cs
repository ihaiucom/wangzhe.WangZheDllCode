using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class RankingSystem : Singleton<RankingSystem>
	{
		public enum RankingType
		{
			None = -1,
			PvpRank,
			Power,
			HeroNum,
			SkinNum,
			Ladder,
			Achievement,
			WinCount,
			ConWinCount,
			ConsumeQuan,
			MentorPoint,
			GameVip,
			God,
			Arena,
			MaxNum
		}

		public enum RankingSubView
		{
			All,
			Friend,
			GuildMember
		}

		protected struct LocalRankingInfo
		{
			public uint LastRetrieveTime;

			public CSDT_RANKING_LIST_SUCC ListInfo;

			public CSDT_GET_RANKING_ACNT_DETAIL_SELF SelfInfo;

			public CSDT_RANKING_LIST_SUCC BackupListInfo;
		}

		private const string AnimCondition = "IsDisplayRankingPanel";

		private const int MaxRankItemCountPerPage = 100;

		private CUIFormScript _form;

		private CUIListScript _tabList;

		private CUIListScript _viewList;

		private ScrollRect _scroll;

		private CUIListScript _rankingList;

		private GameObject _myselfInfo;

		private Animator _animator;

		public static readonly string s_rankingForm = string.Format("{0}{1}", "UGUI/Form/System/", "Ranking/Form_Ranking.prefab");

		public static readonly string s_rankingGodDetailForm = string.Format("{0}{1}", "UGUI/Form/System/", "Ranking/Form_RankingGodDetail.prefab");

		private readonly RankingSystem.LocalRankingInfo[] _rankingInfo = new RankingSystem.LocalRankingInfo[13];

		private Dictionary<uint, RankingSystem.LocalRankingInfo> _godRankInfo = new Dictionary<uint, RankingSystem.LocalRankingInfo>();

		private CSDT_RANKING_LIST_SUCC _dailyRankMatchInfo;

		private ListView<COMDT_FRIEND_INFO> _sortedFriendRankList;

		private uint _myLastFriendRank = 9999999u;

		private bool _rankingListReady;

		private bool _rankingSelfReady;

		private bool _rankingBackupListReady;

		private RankingSystem.RankingType _curRankingType = RankingSystem.RankingType.None;

		private RankingSystem.RankingSubView _curSubViewType;

		private int _curRankingListIndex = -1;

		private string _allViewName;

		private string _friendViewName;

        private Dictionary<stFriendByUUIDAndLogicID, int> _coinSentFriendDic = new Dictionary<stFriendByUUIDAndLogicID, int>();


        private RankingSystem.RankingSubView _defualtSubViewType = RankingSystem.RankingSubView.Friend;

		private uint m_heroMasterId;

		private ListView<ResHeroCfgInfo> m_heroList;

		private int m_curRankGodViewIndex;

		private CSDT_TRANK_TLOG_INFO[] m_uiTlog = new CSDT_TRANK_TLOG_INFO[68];

		public CSDT_TRANK_TLOG_INFO[] GetUiTlog()
		{
			int num = 0;
			for (int i = 0; i < this.m_uiTlog.Length; i++)
			{
				if (this.m_uiTlog[i].dwCnt > 0u)
				{
					num++;
				}
			}
			CSDT_TRANK_TLOG_INFO[] array = new CSDT_TRANK_TLOG_INFO[num];
			num = 0;
			for (int j = 0; j < this.m_uiTlog.Length; j++)
			{
				if (this.m_uiTlog[j].dwCnt > 0u)
				{
					array[num] = new CSDT_TRANK_TLOG_INFO();
					array[num].dwType = this.m_uiTlog[j].dwType;
					array[num].dwCnt = this.m_uiTlog[j].dwCnt;
					num++;
				}
			}
			return array;
		}

		public void ClearUiTlog()
		{
			for (int i = 0; i < this.m_uiTlog.Length; i++)
			{
				this.m_uiTlog[i] = new CSDT_TRANK_TLOG_INFO();
			}
		}

		private RankingSystem.LocalRankingInfo GetLocalRankingInfo(RankingSystem.RankingType rankingType, uint subType = 0u)
		{
			if (rankingType > RankingSystem.RankingType.None && rankingType <= RankingSystem.RankingType.GameVip)
			{
				return this._rankingInfo[(int)rankingType];
			}
			if (rankingType == RankingSystem.RankingType.God && this._godRankInfo.ContainsKey(subType))
			{
				return this._godRankInfo[subType];
			}
			return default(RankingSystem.LocalRankingInfo);
		}

		private static uint GetLocalHeroId()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			uint @int = (uint)PlayerPrefs.GetInt(string.Format("Sgame_uid_{0}_rank_hero_id", masterRoleInfo.playerUllUID));
			if (GameDataMgr.heroDatabin.GetDataByKey(@int) != null)
			{
				return @int;
			}
			return GameDataMgr.heroDatabin.GetAnyData().dwCfgID;
		}

		private static void SetLocalHeroId(uint heroId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (GameDataMgr.heroDatabin.GetDataByKey(heroId) != null)
			{
				PlayerPrefs.SetInt(string.Format("Sgame_uid_{0}_rank_hero_id", masterRoleInfo.playerUllUID), (int)heroId);
				PlayerPrefs.Save();
			}
		}

		private COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO GetSelfHeroMasterInfo(uint heroId)
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(RankingSystem.RankingType.God, heroId);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (localRankingInfo.ListInfo == null)
			{
				return null;
			}
			int num = 0;
			while ((long)num < (long)((ulong)localRankingInfo.ListInfo.dwItemNum))
			{
				if (localRankingInfo.ListInfo.astItemDetail[num].stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo.ullUid == masterRoleInfo.playerUllUID)
				{
					return localRankingInfo.ListInfo.astItemDetail[num].stExtraInfo.stDetailInfo.stMasterHero;
				}
				num++;
			}
			return null;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ShowAllRankType, new CUIEventManager.OnUIEventHandler(this.OnShowAllRankTypeMenu));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeView, new CUIEventManager.OnUIEventHandler(this.OnChangeSubViewTab));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToLadder, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToLadder));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToHeroCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToHeroCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToSkinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToSkinCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToAchievement, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToAchievement));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToWinCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToConWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToConWinCount));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToVip, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToVip));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToArena, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToArena));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToGod, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToGod));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToMentorPoint, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToMentorPoint));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ArenaElementEnable, new CUIEventManager.OnUIEventHandler(RankingView.OnRankingArenaElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HoldDetail, new CUIEventManager.OnUIEventHandler(this.OnHoldDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ElementEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ArenaAddFriend, new CUIEventManager.OnUIEventHandler(this.OnArenaAddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickListItem, new CUIEventManager.OnUIEventHandler(this.OnClickOneListItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickMe, new CUIEventManager.OnUIEventHandler(this.OnClickMe));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickCloseBtn, new CUIEventManager.OnUIEventHandler(this.DoHideAnimation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSnsSendCoin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Friend_GAME_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSendCoin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Rule_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenRuleForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenDetailForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Click_HeroChg_Detail_Tab, new CUIEventManager.OnUIEventHandler(this.OnGodDetailTabClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Title_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgTitleClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Click_Detail_Equip, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailEquipClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Detail_Symbol_Enable, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailSymbolEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Close_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnRankingCloseDetailForm));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Ranking_Get_Ranking_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankingList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("Ranking_Get_Ranking_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetRankingAccountInfo));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_CMD_DONATE_FRIEND_POINT>("Friend_Send_Coin_Done", new Action<SCPKG_CMD_DONATE_FRIEND_POINT>(this.OnCoinSent));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Ranking_Get_Ranking_Daily_RankMatch", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankingDailyRankMatch));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Rank_Arena_List", new Action(this.OnRankArenaList));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Arena_Fighter_Changed", new Action(this.OnArenaFighterChanged));
			Singleton<EventRouter>.GetInstance().AddEventHandler("Arena_Record_List", new Action(this.OnArenaRecordList));
			for (int i = 0; i < this._rankingInfo.Length; i++)
			{
				this._rankingInfo[i].LastRetrieveTime = 0u;
				this._rankingInfo[i].ListInfo = null;
				this._rankingInfo[i].SelfInfo = null;
				this._rankingInfo[i].BackupListInfo = null;
			}
			this.ClearUiTlog();
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_OpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_CloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ShowAllRankType, new CUIEventManager.OnUIEventHandler(this.OnShowAllRankTypeMenu));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeView, new CUIEventManager.OnUIEventHandler(this.OnChangeSubViewTab));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToLadder, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToLadder));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToHeroCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToHeroCount));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToSkinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToSkinCount));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToAchievement, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToAchievement));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToWinCount));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToConWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToConWinCount));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToVip, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToVip));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToArena, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToArena));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToGod, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToGod));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToMentorPoint, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToMentorPoint));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ArenaElementEnable, new CUIEventManager.OnUIEventHandler(RankingView.OnRankingArenaElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HoldDetail, new CUIEventManager.OnUIEventHandler(this.OnHoldDetail));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ElementEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ArenaAddFriend, new CUIEventManager.OnUIEventHandler(this.OnArenaAddFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickListItem, new CUIEventManager.OnUIEventHandler(this.OnClickOneListItem));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickMe, new CUIEventManager.OnUIEventHandler(this.OnClickMe));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickCloseBtn, new CUIEventManager.OnUIEventHandler(this.DoHideAnimation));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSnsSendCoin));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Friend_GAME_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSendCoin));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenHeroForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Rule_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenRuleForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenDetailForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Click_HeroChg_Detail_Tab, new CUIEventManager.OnUIEventHandler(this.OnGodDetailTabClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Title_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgTitleClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Click_Detail_Equip, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailEquipClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Detail_Symbol_Enable, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailSymbolEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Close_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnRankingCloseDetailForm));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Rank_Arena_List", new Action(this.OnRankArenaList));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Arena_Fighter_Changed", new Action(this.OnArenaFighterChanged));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("Arena_Record_List", new Action(this.OnArenaRecordList));
		}

		internal void Clear()
		{
			this._tabList = null;
			this._rankingList = null;
			this._animator = null;
			this._form = null;
			this._curRankingType = RankingSystem.RankingType.None;
			this._curSubViewType = RankingSystem.RankingSubView.All;
		}

		public void ClearAll()
		{
			this.Clear();
			this._defualtSubViewType = RankingSystem.RankingSubView.Friend;
		}

		protected void OnShowAllRankTypeMenu(CUIEvent uiEvent)
		{
			if (this._form.m_formWidgets[14].activeSelf)
			{
				RankingView.HideAllRankMenu();
			}
			else
			{
				RankingView.ShowAllRankMenu();
			}
		}

		protected void OnRanking_OpenForm(CUIEvent uiEvent)
		{
			Singleton<CLobbySystem>.instance.ShowHideRankingBtn(false);
			bool isShowAllRankTypeBtn = Singleton<CUIManager>.GetInstance().GetForm(CLadderSystem.FORM_LADDER_ENTRY) == null;
			this.InitWidget(isShowAllRankTypeBtn);
			this.TryToChangeRankType(RankingSystem.RankingType.Ladder);
			this.DoDisplayAnimation();
		}

		public void OpenRankingForm(RankingSystem.RankingType rankingType, uint subType = 0u)
		{
			if (rankingType == RankingSystem.RankingType.God)
			{
				this.m_heroMasterId = subType;
				RankingSystem.SetLocalHeroId(subType);
			}
			Singleton<CLobbySystem>.instance.ShowHideRankingBtn(false);
			bool isShowAllRankTypeBtn = Singleton<CUIManager>.GetInstance().GetForm(CLadderSystem.FORM_LADDER_ENTRY) == null;
			this.InitWidget(isShowAllRankTypeBtn);
			this.TryToChangeRankType(rankingType);
			this.DoDisplayAnimation();
		}

		private void RetrieveNewList()
		{
			RankingSystem.RankingType curRankingType = this._curRankingType;
			if (curRankingType == RankingSystem.RankingType.Ladder && this.IsDailyRankMatchEmpty())
			{
				this.ReqRankingList(COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_DAILY_RANKMATCH, 0);
			}
			if (curRankingType == RankingSystem.RankingType.God)
			{
				this.ReqRankingList(RankingSystem.ConvertRankingServerType(curRankingType), (int)this.m_heroMasterId);
			}
			else
			{
				this.ReqRankingDetail(-1, true, curRankingType);
				this.ReqRankingList(RankingSystem.ConvertRankingServerType(curRankingType), 0);
			}
			this._rankingListReady = (this._rankingSelfReady = false);
		}

		protected void RetrieveSortedFriendRankList()
		{
			this._myLastFriendRank = 9999999u;
			this._sortedFriendRankList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(RankingSystem.ConvertRankingServerType(this._curRankingType));
		}

		protected bool NeedToRetrieveNewList()
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
			return localRankingInfo.SelfInfo == null || localRankingInfo.ListInfo == null || localRankingInfo.LastRetrieveTime == 0u || CRoleInfo.GetCurrentUTCTime() >= (int)(localRankingInfo.LastRetrieveTime + localRankingInfo.ListInfo.dwTimeLimit);
		}

		protected void OnRanking_CloseForm(CUIEvent uiEvent)
		{
			this.Clear();
			Singleton<CLobbySystem>.instance.ShowHideRankingBtn(true);
		}

		internal void InitWidget(bool isShowAllRankTypeBtn = true)
		{
			this._form = Singleton<CUIManager>.GetInstance().OpenForm(RankingSystem.s_rankingForm, false, true);
			this._animator = this._form.gameObject.GetComponent<Animator>();
			this._tabList = this._form.m_formWidgets[1].GetComponent<CUIListScript>();
			this._viewList = this._form.m_formWidgets[13].GetComponent<CUIListScript>();
			this._rankingList = this._form.m_formWidgets[3].GetComponent<CUIListScript>();
			this._scroll = this._form.m_formWidgets[4].GetComponent<ScrollRect>();
			this._myselfInfo = this._form.m_formWidgets[8];
			this._scroll.elasticity = 0.08f;
			this._rankingList.SetElementAmount(0);
			this._rankingList.m_alwaysDispatchSelectedChangeEvent = true;
			this._tabList.SetElementAmount(1);
			this._tabList.SelectElement(0, true);
			this._viewList.SetElementAmount(2);
			CUIListElementScript elemenet = this._viewList.GetElemenet(0);
			if (elemenet != null)
			{
				this._allViewName = Singleton<CTextManager>.GetInstance().GetText("ranking_ViewAll");
				this._friendViewName = Singleton<CTextManager>.GetInstance().GetText("ranking_ViewFriend");
				elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = this._allViewName;
			}
			elemenet = this._viewList.GetElemenet(1);
			if (elemenet != null)
			{
				elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = this._friendViewName;
			}
			this._form.m_formWidgets[14].CustomSetActive(false);
			this.SetMenuElementText();
			this.TryToChangeRankType(RankingSystem.RankingType.Ladder);
			if (this._defualtSubViewType == RankingSystem.RankingSubView.All)
			{
				this._viewList.SelectElement(0, true);
			}
			else if (this._defualtSubViewType == RankingSystem.RankingSubView.Friend)
			{
				this._viewList.SelectElement(1, true);
			}
			GameObject widget = this._form.GetWidget(16);
			widget.CustomSetActive(isShowAllRankTypeBtn);
		}

		private void SetMenuElementText()
		{
			GameObject gameObject = this._form.m_formWidgets[14];
			if (gameObject != null)
			{
				Transform transform = gameObject.transform.FindChild("ListElement0");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_LadderRankName");
				transform = gameObject.transform.FindChild("ListElement1");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_HeroCountRankName");
				transform = gameObject.transform.FindChild("ListElement2");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_SkinCountRankName");
				transform = gameObject.transform.FindChild("ListElement3");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_AchieveRankName");
				transform = gameObject.transform.FindChild("ListElement4");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_WinCountRankName");
				transform = gameObject.transform.FindChild("ListElement5");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConWinRankName");
				transform = gameObject.transform.FindChild("ListElement6");
				transform.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_GameVIPRankName");
			}
		}

		private void UpdateTabText()
		{
			CUIListElementScript elemenet = this._tabList.GetElemenet(0);
			if (elemenet == null)
			{
				return;
			}
			string text = null;
			if (this._curRankingType == RankingSystem.RankingType.HeroNum)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_HeroCountRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.SkinNum)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_SkinCountRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.Ladder)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_LadderRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.Achievement)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_AchieveRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.WinCount)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_WinCountRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.ConWinCount)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConWinRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.ConsumeQuan)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConsumeQuanRankName");
			}
			else if (this._curRankingType == RankingSystem.RankingType.GameVip)
			{
				text = Singleton<CTextManager>.GetInstance().GetText("ranking_GameVIPRankName");
			}
			else
			{
				if (this._curRankingType == RankingSystem.RankingType.Arena)
				{
					text = Singleton<CTextManager>.GetInstance().GetText("ranking_ArenaRankName");
					elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = text;
					return;
				}
				if (this._curRankingType == RankingSystem.RankingType.God)
				{
					text = Singleton<CTextManager>.GetInstance().GetText("ranking_GodRankName");
					elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = text;
					return;
				}
				if (this._curRankingType == RankingSystem.RankingType.MentorPoint)
				{
					text = Singleton<CTextManager>.GetInstance().GetText("ranking_MentorPointName");
					elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = text;
					return;
				}
			}
			RankingSystem.RankingSubView curSubViewType = this._curSubViewType;
			if (curSubViewType != RankingSystem.RankingSubView.All)
			{
				if (curSubViewType == RankingSystem.RankingSubView.Friend)
				{
					elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = string.Format("{0}{1}", this._friendViewName, text);
				}
			}
			else
			{
				elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = string.Format("{0}{1}", this._allViewName, text);
			}
		}

		protected void TryToChangeRankType(RankingSystem.RankingType rankType)
		{
			if (rankType == this._curRankingType)
			{
				return;
			}
			this._form.m_formWidgets[15].CustomSetActive(false);
			this._form.m_formWidgets[21].CustomSetActive(false);
			this._form.m_formWidgets[23].CustomSetActive(false);
			this._curRankingType = rankType;
			this.UpdateTabText();
			if (rankType == RankingSystem.RankingType.Arena)
			{
				if (this._viewList != null && this._viewList.gameObject != null)
				{
					this._viewList.gameObject.CustomSetActive(false);
					this._form.m_formWidgets[3].CustomSetActive(false);
					this._form.m_formWidgets[17].CustomSetActive(true);
					this._form.m_formWidgets[8].CustomSetActive(false);
					this._form.m_formWidgets[18].CustomSetActive(true);
					this._form.m_formWidgets[19].CustomSetActive(false);
					RankingView.UpdateArenaSelfInfo();
					RankingView.RefreshRankArena();
					CArenaSystem.SendGetRecordMSG(false);
					CArenaSystem.SendGetFightHeroListMSG(false);
					CArenaSystem.SendGetRankListMSG(false);
				}
			}
			else
			{
				if (this._viewList != null && this._viewList.gameObject != null)
				{
					this._viewList.gameObject.CustomSetActive(rankType != RankingSystem.RankingType.God);
				}
				this._form.m_formWidgets[3].CustomSetActive(true);
				this._form.m_formWidgets[17].CustomSetActive(false);
				this._form.m_formWidgets[8].CustomSetActive(true);
				this._form.m_formWidgets[18].CustomSetActive(false);
				this.UpdateGodTitle();
				this.CommitUpdate();
			}
		}

		private void UpdateGodTitle()
		{
			GameObject obj = this._form.m_formWidgets[19];
			if (this._curRankingType == RankingSystem.RankingType.God)
			{
				obj.CustomSetActive(true);
				this.m_heroList = RankingSystem.GetHeroList(enHeroJobType.All, false);
				for (int i = 0; i < this.m_heroList.Count; i++)
				{
					if (this.m_heroList[i].dwCfgID == this.m_heroMasterId)
					{
						RankingView.UpdateRankGodTitle(this.m_heroList[i]);
						break;
					}
				}
			}
			else
			{
				obj.CustomSetActive(false);
			}
			RankingView.ResetRankListPos(this._curRankingType);
		}

		protected void OnChangeRankToLadder(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.Ladder);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.Ladder);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToHeroCount(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.HeroNum);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.HeroNum);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToSkinCount(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.SkinNum);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.SkinNum);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToAchievement(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.Achievement);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.Achievement);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToWinCount(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.WinCount);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.WinCount);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToConWinCount(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.ConWinCount);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.ConWinCount);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToVip(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.GameVip);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.GameVip);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToMentorPoint(CUIEvent uiEvent)
		{
			this.TryToChangeRankType(RankingSystem.RankingType.MentorPoint);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.MentorPoint);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeRankToArena(CUIEvent uiEvent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
			{
				this.TryToChangeRankType(RankingSystem.RankingType.Arena);
				RankingView.HideAllRankMenu();
				COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.Arena);
				this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
				this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(9u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		protected void OnChangeRankToGod(CUIEvent uiEvent)
		{
			this.m_heroMasterId = RankingSystem.GetLocalHeroId();
			this.TryToChangeRankType(RankingSystem.RankingType.God);
			RankingView.HideAllRankMenu();
			COM_APOLLO_TRANK_SCORE_TYPE cOM_APOLLO_TRANK_SCORE_TYPE = RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.God);
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwType = (uint)cOM_APOLLO_TRANK_SCORE_TYPE;
			this.m_uiTlog[(int)cOM_APOLLO_TRANK_SCORE_TYPE].dwCnt += 1u;
		}

		protected void OnChangeSubViewTab(CUIEvent uiEvent)
		{
			if (this._curSubViewType == (RankingSystem.RankingSubView)this._viewList.GetSelectedIndex())
			{
				return;
			}
			this._defualtSubViewType = (this._curSubViewType = (RankingSystem.RankingSubView)this._viewList.GetSelectedIndex());
			this._form.m_formWidgets[15].CustomSetActive(false);
			this._form.m_formWidgets[21].CustomSetActive(false);
			this._form.m_formWidgets[23].CustomSetActive(false);
			this.UpdateTabText();
			if (this._curRankingType == RankingSystem.RankingType.Arena || this._curRankingType == RankingSystem.RankingType.God)
			{
				return;
			}
			this.CommitUpdate();
			Singleton<EventRouter>.GetInstance().BroadCastEvent<RankingSystem.RankingSubView>("Rank_List", this._curSubViewType);
		}

		public void OnHideAnimationEnd()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(RankingSystem.s_rankingForm);
		}

		private void DoDisplayAnimation()
		{
			if (this._animator != null)
			{
				this._animator.SetBool("IsDisplayRankingPanel", true);
			}
		}

		protected void DoHideAnimation(CUIEvent uiEvent)
		{
			if (this._animator != null)
			{
				this._animator.SetBool("IsDisplayRankingPanel", false);
			}
		}

		private void OnFriendSnsSendCoin(CUIEvent uiEvent)
		{
			stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(uiEvent.m_eventParams.commonUInt64Param1, (uint)uiEvent.m_eventParams.commonUInt64Param2, CFriendModel.FriendType.SNS);
			if (!this._coinSentFriendDic.ContainsKey(key))
			{
				this._coinSentFriendDic.Add(key, uiEvent.m_eventParams.tag);
			}
			uiEvent.m_eventID = enUIEventID.Friend_SNS_SendCoin;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
		}

		private void OnFriendSendCoin(CUIEvent uiEvent)
		{
			stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(uiEvent.m_eventParams.commonUInt64Param1, (uint)uiEvent.m_eventParams.commonUInt64Param2, CFriendModel.FriendType.GameFriend);
			if (!this._coinSentFriendDic.ContainsKey(key))
			{
				this._coinSentFriendDic.Add(key, uiEvent.m_eventParams.tag);
			}
			uiEvent.m_eventID = enUIEventID.Friend_SendCoin;
			uiEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
		}

		private void OnArenaFighterChanged()
		{
			if (this._curRankingType == RankingSystem.RankingType.Arena)
			{
				RankingView.UpdateArenaSelfInfo();
			}
		}

		private void OnArenaRecordList()
		{
			if (this._curRankingType == RankingSystem.RankingType.Arena)
			{
				RankingView.UpdateArenaSelfInfo();
			}
		}

		private static ListView<ResHeroCfgInfo> GetHeroList(enHeroJobType jobType, bool bOwn = false)
		{
			ListView<ResHeroCfgInfo> listView = new ListView<ResHeroCfgInfo>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem ResetHeroList role is null");
			if (masterRoleInfo != null)
			{
				ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
				for (int i = 0; i < allHeroList.Count; i++)
				{
					if ((jobType == enHeroJobType.All || allHeroList[i].bMainJob == (byte)jobType || allHeroList[i].bMinorJob == (byte)jobType) && (!bOwn || masterRoleInfo.IsHaveHero(allHeroList[i].dwCfgID, false)))
					{
						listView.Add(allHeroList[i]);
					}
				}
			}
			return listView;
		}

		private void OnGodOpenHeroForm(CUIEvent uiEvent)
		{
			this.m_heroList = RankingSystem.GetHeroList(enHeroJobType.All, false);
			RankingView.OpenHeroChooseForm();
			RankingView.RefreshGodHeroForm(this.m_heroList);
		}

		private void OnGodOpenRuleForm(CUIEvent uiEvent)
		{
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey(14u);
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		private void OnGodOpenDetailForm(CUIEvent uiEvent)
		{
			this.m_curRankGodViewIndex = uiEvent.m_eventParams.tag;
			RankingView.OnRankGodDetailEquipClick(null, string.Empty, string.Empty);
			RankingView.ShowRankGodDetailPanel();
			RankingView.UpdateGodFindBtns(this._rankingList, this.m_curRankGodViewIndex);
		}

		private void OnGodDetailTabClick(CUIEvent uiEvent)
		{
			if (this._curRankingType != RankingSystem.RankingType.God)
			{
				return;
			}
			CUIListScript cUIListScript = (CUIListScript)uiEvent.m_srcWidgetScript;
			int selectedIndex = cUIListScript.GetSelectedIndex();
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
			if (localRankingInfo.ListInfo != null && this.m_curRankGodViewIndex >= 0 && (long)this.m_curRankGodViewIndex < (long)((ulong)localRankingInfo.ListInfo.dwItemNum))
			{
				RankingView.OnRankGodDetailTab(selectedIndex, localRankingInfo.ListInfo.astItemDetail[this.m_curRankGodViewIndex].stExtraInfo.stDetailInfo.stMasterHero, this.m_heroMasterId);
			}
		}

		private void OnGodHeroChgTitleClick(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = (CUIListScript)uiEvent.m_srcWidgetScript;
			enHeroJobType selectedIndex = (enHeroJobType)cUIListScript.GetSelectedIndex();
			this.m_heroList = RankingSystem.GetHeroList(selectedIndex, false);
			RankingView.RefreshGodHeroForm(this.m_heroList);
		}

		private void OnGodHeroChgItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_heroList.Count)
			{
				RankingView.OnHeroItemEnable(uiEvent, this.m_heroList[srcWidgetIndexInBelongedList]);
			}
		}

		private void OnGodHeroChgItemClick(CUIEvent uiEvent)
		{
			this.m_heroMasterId = uiEvent.m_eventParams.heroId;
			RankingSystem.SetLocalHeroId(this.m_heroMasterId);
			Singleton<CUIManager>.GetInstance().CloseForm(RankingView.s_ChooseHeroPath);
			this.CommitUpdate();
		}

		private void OnRankingCloseDetailForm(CUIEvent uiEvent)
		{
			RankingView.UpdateGodFindBtns(this._rankingList, -1);
		}

		private void OnRankingDetailEquipClick(CUIEvent uiEvent)
		{
			RankingView.OnRankGodDetailEquipClick(uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_eventParams.tagStr, uiEvent.m_eventParams.tagStr1);
		}

		private void OnRankingDetailSymbolEnable(CUIEvent uiEvent)
		{
			RankingView.UpdateSymbolItem(uiEvent.m_eventParams.symbolParam.symbol, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
		}

		private void OnRankArenaList()
		{
			if (this._curRankingType == RankingSystem.RankingType.Arena)
			{
				RankingView.RefreshRankArena();
			}
		}

		protected void CommitUpdate()
		{
			if (this._rankingList.GetSelectedElement() != null)
			{
				this._rankingList.GetSelectedElement().ChangeDisplay(false);
			}
			this._curRankingListIndex = -1;
			if (this.NeedToRetrieveNewList())
			{
				this.RetrieveNewList();
			}
			else
			{
				this._rankingList.MoveElementInScrollArea(0, true);
				this.UpdateAllElementInView();
				this.UpdateSelfInfo();
			}
		}

		public int GetMyFriendRankNo()
		{
			int result = -1;
			RankingSystem.RankingType rankingType = RankingSystem.RankingType.Ladder;
			if (this._rankingInfo[(int)rankingType].SelfInfo == null)
			{
				return result;
			}
			uint dwScore = this._rankingInfo[(int)rankingType].SelfInfo.dwScore;
			ListView<COMDT_FRIEND_INFO> sortedRankingFriendList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(RankingSystem.ConvertRankingServerType(rankingType));
			uint num = (uint)(sortedRankingFriendList.Count + 1);
			uint num2 = 0u;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num))
			{
				uint num4 = 0u;
				result = num3;
				if (num3 < sortedRankingFriendList.Count)
				{
					num4 = sortedRankingFriendList[num3].RankVal[(int)RankingSystem.ConvertRankingServerType(rankingType)];
					num2 = sortedRankingFriendList[num3].dwPvpLvl;
				}
				if (num3 < sortedRankingFriendList.Count && dwScore >= num4 && (rankingType != RankingSystem.RankingType.Ladder || dwScore != num4 || masterRoleInfo.PvpLevel >= num2))
				{
					break;
				}
				num3++;
			}
			return result;
		}

		protected void UpdateAllElementInView()
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
			uint num;
			if (this._curRankingType == RankingSystem.RankingType.God)
			{
				num = localRankingInfo.ListInfo.dwItemNum;
				if (localRankingInfo.BackupListInfo != null)
				{
					num += localRankingInfo.BackupListInfo.dwItemNum;
				}
				this._rankingList.SetElementAmount((int)num);
				this._rankingList.MoveElementInScrollArea(0, true);
				this._form.m_formWidgets[21].CustomSetActive(num == 0u);
				this._form.m_formWidgets[15].CustomSetActive(false);
				this._form.m_formWidgets[23].CustomSetActive(false);
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (this._curSubViewType != RankingSystem.RankingSubView.All)
			{
				int elementAmount = this._rankingList.GetElementAmount();
				for (int i = 0; i < elementAmount; i++)
				{
					if (this._rankingList.GetElemenet(i) != null && this._rankingList.IsElementInScrollArea(i))
					{
						this.EmptyOneElement(this._rankingList.GetElemenet(i).gameObject, i);
					}
				}
			}
			RankingSystem.RankingSubView curSubViewType = this._curSubViewType;
			if (curSubViewType != RankingSystem.RankingSubView.Friend)
			{
				num = localRankingInfo.ListInfo.dwItemNum;
				if (localRankingInfo.BackupListInfo != null)
				{
					num += localRankingInfo.BackupListInfo.dwItemNum;
				}
			}
			else
			{
				this.RetrieveSortedFriendRankList();
				num = (uint)(this._sortedFriendRankList.Count + 1);
				uint dwScore = localRankingInfo.SelfInfo.dwScore;
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					this._myLastFriendRank = (uint)num2;
					uint num3 = 0u;
					uint num4 = 0u;
					if (num2 < this._sortedFriendRankList.Count)
					{
						num3 = this._sortedFriendRankList[num2].RankVal[(int)RankingSystem.ConvertRankingServerType(this._curRankingType)];
						num4 = this._sortedFriendRankList[num2].dwPvpLvl;
					}
					if (num2 < this._sortedFriendRankList.Count && dwScore >= num3 && (this._curRankingType != RankingSystem.RankingType.Ladder || dwScore != num3 || masterRoleInfo.PvpLevel >= num4))
					{
						break;
					}
					num2++;
				}
			}
			this._rankingList.SetElementAmount((int)num);
			this._rankingList.MoveElementInScrollArea(0, true);
			int num5 = 0;
			while ((long)num5 < (long)((ulong)num))
			{
				if (this._rankingList.GetElemenet(num5) != null && this._rankingList.IsElementInScrollArea(num5))
				{
					this.UpdateOneElement(this._rankingList.GetElemenet(num5).gameObject, num5);
				}
				num5++;
			}
			this._form.m_formWidgets[21].CustomSetActive(false);
			if (this._curRankingType == RankingSystem.RankingType.Achievement)
			{
				this._form.m_formWidgets[15].CustomSetActive(false);
				this._form.m_formWidgets[23].CustomSetActive(num == 0u);
			}
			else
			{
				this._form.m_formWidgets[15].CustomSetActive(num == 0u);
				this._form.m_formWidgets[23].CustomSetActive(false);
			}
		}

		private bool IsMyFriendRankIndex(int index)
		{
			return (long)index == (long)((ulong)this._myLastFriendRank);
		}

		private void UpdateOneElement(GameObject objElement, int viewIndex)
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, 0u);
			if (localRankingInfo.ListInfo == null)
			{
				return;
			}
			RankingItemHelper component = objElement.GetComponent<RankingItemHelper>();
			uint num = 0u;
			string text = string.Empty;
			uint num2 = 1u;
			string serverUrl = null;
			ulong num3 = 0uL;
			uint num4 = 0u;
			uint level = 0u;
			uint headIdx = 0u;
			uint num5 = 0u;
			ulong privacyBits = 0uL;
			COM_PRIVILEGE_TYPE privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			if (this._curSubViewType == RankingSystem.RankingSubView.Friend)
			{
				if (this.IsMyFriendRankIndex(viewIndex))
				{
					CSDT_GET_RANKING_ACNT_DETAIL_SELF selfInfo = localRankingInfo.SelfInfo;
					if (selfInfo != null && Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)
					{
						num = selfInfo.dwScore;
						text = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name;
						num2 = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
						serverUrl = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HeadUrl;
						privilegeType = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType;
						privacyBits = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_userPrivacyBits;
						num3 = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
						num4 = (uint)MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
						level = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel;
						headIdx = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId;
						num5 = 913913u;
						RankingView.SetGameObjChildText(this._myselfInfo, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
						RankingView.SetGameObjChildText(this._myselfInfo, "PlayerLv", string.Format("Lv.{0}", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel.ToString(CultureInfo.InvariantCulture)));
					}
				}
				else
				{
					int num6 = this.ConvertFriendRankIndex(viewIndex);
					if (this._sortedFriendRankList != null && num6 < this._sortedFriendRankList.Count && num6 >= 0)
					{
						COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this._sortedFriendRankList[num6];
						if (cOMDT_FRIEND_INFO != null)
						{
							num = cOMDT_FRIEND_INFO.RankVal[(int)RankingSystem.ConvertRankingServerType(this._curRankingType)];
							text = StringHelper.UTF8BytesToString(ref cOMDT_FRIEND_INFO.szUserName);
							num2 = cOMDT_FRIEND_INFO.dwPvpLvl;
							serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref cOMDT_FRIEND_INFO.szHeadUrl);
							num3 = cOMDT_FRIEND_INFO.stUin.ullUid;
							num4 = cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
							level = cOMDT_FRIEND_INFO.stGameVip.dwCurLevel;
							headIdx = cOMDT_FRIEND_INFO.stGameVip.dwHeadIconId;
							num5 = cOMDT_FRIEND_INFO.dwQQVIPMask;
							privacyBits = cOMDT_FRIEND_INFO.ullUserPrivacyBits;
							privilegeType = (COM_PRIVILEGE_TYPE)cOMDT_FRIEND_INFO.bPrivilege;
							CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(num3, num4);
							string text2 = string.Empty;
							if (friendInGaming != null)
							{
								text2 = friendInGaming.NickName;
							}
							if (!string.IsNullOrEmpty(text2))
							{
								text = string.Format("{0}({1})", StringHelper.UTF8BytesToString(ref cOMDT_FRIEND_INFO.szUserName), text2);
							}
						}
					}
				}
			}
			else
			{
				CSDT_RANKING_LIST_ITEM_INFO cSDT_RANKING_LIST_ITEM_INFO = null;
				if (localRankingInfo.ListInfo.astItemDetail != null && viewIndex < localRankingInfo.ListInfo.astItemDetail.Length && (long)viewIndex < (long)((ulong)localRankingInfo.ListInfo.dwItemNum))
				{
					cSDT_RANKING_LIST_ITEM_INFO = localRankingInfo.ListInfo.astItemDetail[viewIndex];
				}
				else
				{
					int num7 = viewIndex - 100;
					if (localRankingInfo.BackupListInfo != null && localRankingInfo.BackupListInfo.astItemDetail != null && num7 >= 0 && num7 < localRankingInfo.BackupListInfo.astItemDetail.Length && (long)num7 < (long)((ulong)localRankingInfo.BackupListInfo.dwItemNum))
					{
						cSDT_RANKING_LIST_ITEM_INFO = localRankingInfo.BackupListInfo.astItemDetail[num7];
					}
				}
				if (cSDT_RANKING_LIST_ITEM_INFO != null)
				{
					num = cSDT_RANKING_LIST_ITEM_INFO.dwRankScore;
					COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this._curRankingType, viewIndex, 0u);
					if (rankItemDetailInfo != null)
					{
						text = StringHelper.UTF8BytesToString(ref rankItemDetailInfo.szPlayerName);
						num2 = rankItemDetailInfo.dwPvpLevel;
						serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankItemDetailInfo.szHeadUrl);
						num3 = rankItemDetailInfo.ullUid;
						num4 = (uint)rankItemDetailInfo.iLogicWorldId;
						level = rankItemDetailInfo.stGameVip.dwCurLevel;
						headIdx = rankItemDetailInfo.stGameVip.dwHeadIconId;
						privilegeType = (COM_PRIVILEGE_TYPE)rankItemDetailInfo.bPrivilege;
						num5 = rankItemDetailInfo.dwVipLevel;
						privacyBits = rankItemDetailInfo.ullUserPrivacyBits;
					}
				}
			}
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			RankingView.SetGameObjChildText(objElement, "NameGroup/PlayerName", text);
			RankingView.SetGameObjChildText(objElement, "PlayerLv", string.Format("Lv.{0}", Math.Max(1u, num2)));
			RankingView.SetUrlHeadIcon(component.HeadIcon, serverUrl);
			RankingView.SetPlatChannel(objElement, num4);
			component.FindBtn.CustomSetActive(false);
			if (this._curRankingType == RankingSystem.RankingType.Ladder)
			{
				component.LadderGo.CustomSetActive(true);
				objElement.transform.FindChild("Value").gameObject.CustomSetActive(false);
				objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(false);
			}
			else
			{
				component.LadderGo.CustomSetActive(false);
				objElement.transform.FindChild("Value").gameObject.CustomSetActive(true);
				objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
			}
			switch (this._curRankingType)
			{
			case RankingSystem.RankingType.PvpRank:
			{
				RankingView.SetGameObjChildText(objElement, "ValueType", RankingSystem.GetPvpRankNameEx(num));
				int num8 = 1;
				int num9 = 0;
				RankingSystem.ConvertPvpLevelAndPhase(num, out num8, out num9);
				RankingView.SetGameObjChildText(objElement, "Value", num9.ToString(CultureInfo.InvariantCulture));
				break;
			}
			case RankingSystem.RankingType.HeroNum:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroCountName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.SkinNum:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemSkinCountName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.Ladder:
			{
				byte rankGrade = 0;
				uint num10 = 0u;
				if (this._curSubViewType == RankingSystem.RankingSubView.Friend)
				{
					if (this.IsMyFriendRankIndex(viewIndex))
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							rankGrade = masterRoleInfo.m_rankGrade;
							num10 = masterRoleInfo.m_rankScore;
						}
					}
					else
					{
						int num11 = this.ConvertFriendRankIndex(viewIndex);
						if (this._sortedFriendRankList != null && num11 < this._sortedFriendRankList.Count && num11 >= 0)
						{
							COMDT_FRIEND_INFO cOMDT_FRIEND_INFO2 = this._sortedFriendRankList[num11];
							if (cOMDT_FRIEND_INFO2 != null)
							{
								rankGrade = cOMDT_FRIEND_INFO2.stRankShowGrade.bGrade;
								num10 = cOMDT_FRIEND_INFO2.stRankShowGrade.dwScore;
							}
						}
					}
				}
				else
				{
					CSDT_RANKING_LIST_ITEM_INFO cSDT_RANKING_LIST_ITEM_INFO2 = null;
					if (localRankingInfo.ListInfo.astItemDetail != null && viewIndex < localRankingInfo.ListInfo.astItemDetail.Length && (long)viewIndex < (long)((ulong)localRankingInfo.ListInfo.dwItemNum))
					{
						cSDT_RANKING_LIST_ITEM_INFO2 = localRankingInfo.ListInfo.astItemDetail[viewIndex];
					}
					else
					{
						int num12 = viewIndex - 100;
						if (localRankingInfo.BackupListInfo != null && localRankingInfo.BackupListInfo.astItemDetail != null && num12 >= 0 && num12 < localRankingInfo.BackupListInfo.astItemDetail.Length && (long)num12 < (long)((ulong)localRankingInfo.BackupListInfo.dwItemNum))
						{
							cSDT_RANKING_LIST_ITEM_INFO2 = localRankingInfo.BackupListInfo.astItemDetail[num12];
						}
					}
					if (cSDT_RANKING_LIST_ITEM_INFO2 != null)
					{
						COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo2 = this.GetRankItemDetailInfo(this._curRankingType, viewIndex, 0u);
						if (rankItemDetailInfo2 != null)
						{
							rankGrade = rankItemDetailInfo2.stRankInfo.bShowGradeOfRank;
							num10 = rankItemDetailInfo2.stRankInfo.dwScoreOfRank;
						}
					}
				}
				if (num2 >= CLadderSystem.REQ_PLAYER_LEVEL && ((!Utility.IsSelf(num3, (int)num4)) ? Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(false, (int)rankGrade, (int)num10) : Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(true, -1, -1)))
				{
					CLadderView.ShowRankDetail(component.LadderGo, rankGrade, this.GetRankClass(num3), 1u, false, true, false, true, true);
					component.LadderXing.GetComponent<Text>().text = string.Format("x{0}", num10);
				}
				else
				{
					RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemNoLadderName"));
					component.LadderGo.CustomSetActive(false);
					objElement.transform.FindChild("Value").gameObject.CustomSetActive(false);
					objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
				}
				break;
			}
			case RankingSystem.RankingType.Achievement:
			{
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemAchieveName"));
				CTrophyRewardInfo trophyRewardInfoByPoint = CAchieveInfo2.GetMasterAchieveInfo().GetTrophyRewardInfoByPoint(num);
				if (trophyRewardInfoByPoint != null)
				{
					RankingView.SetGameObjChildText(objElement, "Value", trophyRewardInfoByPoint.Cfg.dwTrophyLvl.ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					RankingView.SetGameObjChildText(objElement, "Value", 1.ToString(CultureInfo.InvariantCulture));
				}
				break;
			}
			case RankingSystem.RankingType.WinCount:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemWinCountName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.ConWinCount:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConWinCountName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.ConsumeQuan:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConsumeQuanName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.MentorPoint:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_MentorPoint"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.GameVip:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemGameVIPName"));
				RankingView.SetGameObjChildText(objElement, "Value", num.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.God:
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", new string[]
				{
					"0",
					"0"
				}));
				RankingView.SetGameObjChildText(objElement, "Value", string.Empty);
				break;
			}
			uint rankNumber = (uint)(viewIndex + 1);
			RankingView.RankingNumSet(rankNumber, component);
			COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, num3, num4);
			COMDT_FRIEND_INFO info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, num3, num4);
			bool flag = info != null;
			bool flag2 = info2 != null;
			COMDT_ACNT_UNIQ cOMDT_ACNT_UNIQ = new COMDT_ACNT_UNIQ();
			cOMDT_ACNT_UNIQ.ullUid = num3;
			cOMDT_ACNT_UNIQ.dwLogicWorldId = num4;
			if (this._curSubViewType == RankingSystem.RankingSubView.Friend)
			{
				component.AddFriend.CustomSetActive(false);
			}
			else
			{
				uint num13 = (uint)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
				component.AddFriend.CustomSetActive(!flag && !flag2 && (ulong)num13 != num3);
			}
			CUIEventScript component2 = component.SendCoin.GetComponent<CUIEventScript>();
			component.AntiDisturbBits.CustomSetActive(false);
			if (flag2)
			{
				bool bEnable = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(cOMDT_ACNT_UNIQ);
				component.ShowSendButton(bEnable);
				component2.m_onClickEventID = enUIEventID.Ranking_Friend_SNS_SendCoin;
				component2.m_onClickEventParams.tag = viewIndex;
				component2.m_onClickEventParams.commonUInt64Param1 = num3;
				component2.m_onClickEventParams.commonUInt64Param2 = (ulong)num4;
				component.Online.CustomSetActive(true);
				if (component.Online != null)
				{
					Text componetInChild = Utility.GetComponetInChild<Text>(component.Online, "Text");
					CFriendModel.FriendInGame friendInGaming2 = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(info2.stUin.ullUid, info2.stUin.dwLogicWorldId);
					component.AntiDisturbBits.CustomSetActive(info2.bIsOnline >= 0 && friendInGaming2 != null && (friendInGaming2.antiDisturbBits & 1u) == 1u);
					if (componetInChild != null)
					{
						if (info2.bIsOnline >= 0)
						{
							componetInChild.text = string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
						}
						else
						{
							componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Common_Offline");
						}
					}
				}
			}
			else if (flag)
			{
				bool bEnable2 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(cOMDT_ACNT_UNIQ);
				component.ShowSendButton(bEnable2);
				component2.m_onClickEventID = enUIEventID.Ranking_Friend_GAME_SendCoin;
				component2.m_onClickEventParams.tag = viewIndex;
				component2.m_onClickEventParams.commonUInt64Param1 = num3;
				component2.m_onClickEventParams.commonUInt64Param2 = (ulong)num4;
				component.Online.CustomSetActive(true);
				if (component.Online != null)
				{
					Text componetInChild2 = Utility.GetComponetInChild<Text>(component.Online, "Text");
					CFriendModel.FriendInGame friendInGaming3 = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(info.stUin.ullUid, info.stUin.dwLogicWorldId);
					component.AntiDisturbBits.CustomSetActive(info.bIsOnline >= 0 && friendInGaming3 != null && (friendInGaming3.antiDisturbBits & 1u) == 1u);
					if (componetInChild2 != null)
					{
						if (info.bIsOnline >= 0)
						{
							componetInChild2.text = string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
						}
						else
						{
							componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Common_Offline");
						}
					}
				}
			}
			else
			{
				CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo2 != null)
				{
					if (this._curSubViewType == RankingSystem.RankingSubView.Friend)
					{
						if (this.IsMyFriendRankIndex(viewIndex))
						{
							component.AntiDisturbBits.CustomSetActive((masterRoleInfo2.OtherStatebBits & 1u) > 0u);
						}
					}
					else if (this._curSubViewType == RankingSystem.RankingSubView.All && masterRoleInfo2.playerUllUID == num3 && (long)masterRoleInfo2.logicWorldID == (long)((ulong)num4))
					{
						component.AntiDisturbBits.CustomSetActive((masterRoleInfo2.OtherStatebBits & 1u) > 0u);
					}
				}
				component.SendCoin.CustomSetActive(false);
				component.Online.CustomSetActive(false);
				component.Online.CustomSetActive(false);
			}
			if (num5 == 913913u)
			{
				MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
			}
			else
			{
				MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int)num5);
			}
			CRoleInfo masterRoleInfo3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo3 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int)level, false, masterRoleInfo3.playerUllUID == num3, privacyBits);
			}
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int)headIdx);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component.HeadIconFrame.GetComponent<Image>(), (int)headIdx, this._form, 1f, true);
		}

		protected int ConvertFriendRankIndex(int rankNo)
		{
			int num = rankNo;
			if ((long)rankNo >= (long)((ulong)this._myLastFriendRank))
			{
				num--;
			}
			return num;
		}

		private void EmptyOneElement(GameObject objElement, int viewIndex)
		{
			RankingItemHelper component = objElement.GetComponent<RankingItemHelper>();
			component.RankingNumText.CustomSetActive(false);
		}

		private static void ConvertPvpLevelAndPhase(uint score, out int level, out int remaining)
		{
			level = 1;
			uint num = score;
			int num2 = 1;
			int num3 = GameDataMgr.acntPvpExpDatabin.Count();
			for (int i = 1; i <= num3 - 1; i++)
			{
				ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)i));
				if (num < dataByKey.dwNeedExp)
				{
					num2 = i;
					break;
				}
				num -= dataByKey.dwNeedExp;
				num2 = i + 1;
			}
			level = num2;
			remaining = (int)num;
		}

		private static string GetPvpRankNameEx(uint score)
		{
			int level = 1;
			int num = 0;
			RankingSystem.ConvertPvpLevelAndPhase(score, out level, out num);
			return RankingSystem.GetPvpRankName(level);
		}

		private static string GetPvpRankName(int level)
		{
			ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)level));
			return (dataByKey != null) ? string.Format("Lv.{0}", dataByKey.bLevel) : string.Empty;
		}

		protected void UpdateSelfInfo()
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			CSDT_GET_RANKING_ACNT_DETAIL_SELF selfInfo = localRankingInfo.SelfInfo;
			RankingItemHelper component = this._myselfInfo.GetComponent<RankingItemHelper>();
			uint pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
			RankingView.SetGameObjChildText(this._myselfInfo, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
			RankingView.SetGameObjChildText(this._myselfInfo, "PlayerLv", string.Format("Lv.{0}", pvpLevel.ToString(CultureInfo.InvariantCulture)));
			RankingView.SetHostUrlHeadIcon(component.HeadIcon);
			if (this._curRankingType == RankingSystem.RankingType.Ladder)
			{
				component.LadderGo.CustomSetActive(true);
				this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(false);
				this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(false);
			}
			else
			{
				component.LadderGo.CustomSetActive(false);
				this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(true);
				this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
			}
			switch (this._curRankingType)
			{
			case RankingSystem.RankingType.PvpRank:
			{
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", RankingSystem.GetPvpRankNameEx(selfInfo.dwScore));
				int num = 1;
				int num2 = 0;
				RankingSystem.ConvertPvpLevelAndPhase(selfInfo.dwScore, out num, out num2);
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", num2.ToString(CultureInfo.InvariantCulture));
				ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)num));
				if (dataByKey != null)
				{
				}
				break;
			}
			case RankingSystem.RankingType.HeroNum:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroCountName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.SkinNum:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemSkinCountName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.Ladder:
				if (pvpLevel >= CLadderSystem.REQ_PLAYER_LEVEL && Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(true, -1, -1))
				{
					CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo2 != null)
					{
						byte rankGrade = masterRoleInfo2.m_rankGrade;
						CLadderView.ShowRankDetail(component.LadderGo, rankGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), 1u, false, true, false, true, true);
						uint rankScore = masterRoleInfo2.m_rankScore;
						component.LadderXing.GetComponent<Text>().text = string.Format("x{0}", rankScore);
					}
				}
				else
				{
					RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemNoLadderName"));
					component.LadderGo.CustomSetActive(false);
					this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(false);
					this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
				}
				break;
			case RankingSystem.RankingType.Achievement:
			{
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemAchieveName"));
				CTrophyRewardInfo trophyRewardInfoByPoint = CAchieveInfo2.GetMasterAchieveInfo().GetTrophyRewardInfoByPoint(selfInfo.dwScore);
				if (trophyRewardInfoByPoint != null)
				{
					RankingView.SetGameObjChildText(this._myselfInfo, "Value", trophyRewardInfoByPoint.Cfg.dwTrophyLvl.ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					RankingView.SetGameObjChildText(this._myselfInfo, "Value", 1.ToString(CultureInfo.InvariantCulture));
				}
				break;
			}
			case RankingSystem.RankingType.WinCount:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemWinCountName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.ConWinCount:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConWinCountName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.ConsumeQuan:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConsumeQuanName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.MentorPoint:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_MentorPoint"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.GameVip:
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemGameVIPName"));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
				break;
			case RankingSystem.RankingType.God:
			{
				COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO selfHeroMasterInfo = this.GetSelfHeroMasterInfo(this.m_heroMasterId);
				uint num3 = 0u;
				uint num4 = 0u;
				if (selfHeroMasterInfo != null && selfHeroMasterInfo.dwGameCnt != 0u)
				{
					num3 = selfHeroMasterInfo.dwWinCnt * 100u / selfHeroMasterInfo.dwGameCnt;
					num4 = selfHeroMasterInfo.dwWinCnt;
				}
				else
				{
					CHeroInfo heroInfo = masterRoleInfo.GetHeroInfo(this.m_heroMasterId, false);
					if (heroInfo != null && heroInfo.m_masterHeroFightCnt != 0u)
					{
						num3 = heroInfo.m_masterHeroWinCnt * 100u / heroInfo.m_masterHeroFightCnt;
						num4 = heroInfo.m_masterHeroWinCnt;
					}
				}
				RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", new string[]
				{
					num3.ToString(),
					num4.ToString()
				}));
				RankingView.SetGameObjChildText(this._myselfInfo, "Value", string.Empty);
				break;
			}
			}
			uint rankNumber;
			if (this._curRankingType != RankingSystem.RankingType.God && this._curSubViewType == RankingSystem.RankingSubView.Friend)
			{
				rankNumber = this._myLastFriendRank + 1u;
			}
			else
			{
				rankNumber = selfInfo.dwRankNo;
			}
			if (selfInfo.iRankChgNo == 0 || this._curSubViewType != RankingSystem.RankingSubView.All)
			{
				component.RankingUpDownIcon.CustomSetActive(false);
				RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", "--");
			}
			else if (selfInfo.iRankChgNo > 0)
			{
				component.RankingUpDownIcon.CustomSetActive(true);
				component.RankingUpDownIcon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", selfInfo.iRankChgNo.ToString(CultureInfo.InvariantCulture));
			}
			else if (selfInfo.iRankChgNo < 0)
			{
				component.RankingUpDownIcon.CustomSetActive(true);
				component.RankingUpDownIcon.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
				RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", (-selfInfo.iRankChgNo).ToString(CultureInfo.InvariantCulture));
			}
			RankingView.RankingNumSet(rankNumber, component);
			MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_userPrivacyBits);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component.HeadIconFrame.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId, this._form, 1f, false);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
		}

		protected void OnElementEnable(CUIEvent uiEvent)
		{
			if (this._curRankingType != RankingSystem.RankingType.God)
			{
				this.UpdateOneElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
				CUIListElementScript component = uiEvent.m_srcWidget.GetComponent<CUIListElementScript>();
				if (component != null && (this._curRankingListIndex == -1 || uiEvent.m_srcWidgetIndexInBelongedList != this._curRankingListIndex))
				{
					component.ChangeDisplay(false);
				}
			}
			else
			{
				RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
				RankingView.UpdateOneGodElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList, localRankingInfo.ListInfo);
			}
		}

		protected void OnHoldDetail(CUIEvent uiEvent)
		{
			if (this._rankingList == null)
			{
				return;
			}
			this._curRankingListIndex = this._rankingList.GetSelectedIndex();
			ulong num = 0uL;
			int num2 = 0;
			if (this._curSubViewType == RankingSystem.RankingSubView.Friend && this._curRankingType != RankingSystem.RankingType.God)
			{
				if ((long)this._curRankingListIndex == (long)((ulong)this._myLastFriendRank))
				{
					return;
				}
				int index = this.ConvertFriendRankIndex(this._curRankingListIndex);
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this._sortedFriendRankList[index];
				num = cOMDT_FRIEND_INFO.stUin.ullUid;
				num2 = (int)cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
			}
			else
			{
				COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this._curRankingType, this._curRankingListIndex, this.m_heroMasterId);
				if (rankItemDetailInfo != null)
				{
					num = rankItemDetailInfo.ullUid;
					num2 = rankItemDetailInfo.iLogicWorldId;
					if (num == Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID && num2 == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID)
					{
						return;
					}
				}
			}
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mini_Player_Info_Open_Form;
			cUIEvent.m_srcFormScript = uiEvent.m_srcFormScript;
			cUIEvent.m_eventParams.tag = 1;
			cUIEvent.m_eventParams.commonUInt64Param1 = num;
			cUIEvent.m_eventParams.tag2 = num2;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		protected void OnAddFriend(CUIEvent uiEvent)
		{
			if (this._rankingList == null)
			{
				return;
			}
			this._curRankingListIndex = this._rankingList.GetSelectedIndex();
			ulong ullUid = 0uL;
			int dwLogicWorldId = 0;
			if (this._curSubViewType == RankingSystem.RankingSubView.Friend)
			{
				if ((long)this._curRankingListIndex != (long)((ulong)this._myLastFriendRank))
				{
					int index = this.ConvertFriendRankIndex(this._curRankingListIndex);
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this._sortedFriendRankList[index];
					ullUid = cOMDT_FRIEND_INFO.stUin.ullUid;
					dwLogicWorldId = (int)cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
				}
				else
				{
					ullUid = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
					dwLogicWorldId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
				}
			}
			else
			{
				COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this._curRankingType, this._curRankingListIndex, 0u);
				if (rankItemDetailInfo != null)
				{
					ullUid = rankItemDetailInfo.ullUid;
					dwLogicWorldId = rankItemDetailInfo.iLogicWorldId;
				}
			}
			Singleton<CFriendContoller>.instance.Open_Friend_Verify(ullUid, (uint)dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
		}

		private void OnArenaAddFriend(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			int tag = uiEvent.m_eventParams.tag;
			Singleton<CFriendContoller>.instance.Open_Friend_Verify(commonUInt64Param, (uint)tag, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
		}

		protected void OnClickOneListItem(CUIEvent uiEvent)
		{
			if (this._rankingList == null)
			{
				return;
			}
			this._curRankingListIndex = this._rankingList.GetSelectedIndex();
			this._rankingList.GetSelectedElement().ChangeDisplay(true);
		}

		protected void OnClickMe(CUIEvent uiEvent)
		{
			if (this._rankingList != null && this._rankingList.GetSelectedElement() != null)
			{
				this._rankingList.GetSelectedElement().ChangeDisplay(false);
			}
		}

		protected static COM_APOLLO_TRANK_SCORE_TYPE ConvertRankingServerType(RankingSystem.RankingType rankType)
		{
			COM_APOLLO_TRANK_SCORE_TYPE result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_NULL;
			switch (rankType)
			{
			case RankingSystem.RankingType.PvpRank:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_PVP_EXP;
				break;
			case RankingSystem.RankingType.Power:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_POWER;
				break;
			case RankingSystem.RankingType.HeroNum:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_HERO_NUM;
				break;
			case RankingSystem.RankingType.SkinNum:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_SKIN_NUM;
				break;
			case RankingSystem.RankingType.Ladder:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT;
				break;
			case RankingSystem.RankingType.Achievement:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_ACHIEVEMENT;
				break;
			case RankingSystem.RankingType.WinCount:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_WIN_GAMENUM;
				break;
			case RankingSystem.RankingType.ConWinCount:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_CONTINOUS_WIN;
				break;
			case RankingSystem.RankingType.ConsumeQuan:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_USE_COUPONS;
				break;
			case RankingSystem.RankingType.MentorPoint:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTERPOINT;
				break;
			case RankingSystem.RankingType.GameVip:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_VIP_SCORE;
				break;
			case RankingSystem.RankingType.God:
				result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTER_HERO;
				break;
			}
			return result;
		}

        protected static RankingType ConvertRankingLocalType(COM_APOLLO_TRANK_SCORE_TYPE rankType)
        {
            RankingType power = RankingType.Power;
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = rankType;
            switch (com_apollo_trank_score_type)
            {
                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_POWER:
                    return RankingType.Power;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_PVP_EXP:
                    return RankingType.PvpRank;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_HERO_NUM:
                    return RankingType.HeroNum;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_SKIN_NUM:
                    return RankingType.SkinNum;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT:
                    return RankingType.Ladder;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_ACHIEVEMENT:
                    return RankingType.Achievement;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_WIN_GAMENUM:
                    return RankingType.WinCount;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_CONTINOUS_WIN:
                    return RankingType.ConWinCount;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_USE_COUPONS:
                    return RankingType.ConsumeQuan;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_VIP_SCORE:
                    return RankingType.GameVip;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTERPOINT:
                    return RankingType.MentorPoint;
            }
            if (com_apollo_trank_score_type != COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTER_HERO)
            {
                return power;
            }
            return RankingType.God;
        }


		protected void ReqRankingList(COM_APOLLO_TRANK_SCORE_TYPE rankType, int subType = 0)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.iStart = 1;
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = (byte)rankType;
			cSPkg.stPkgData.stGetRankingListReq.iSubType = subType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			this._rankingBackupListReady = true;
		}

		protected void ReqRankingDetail(int listIndex, bool isSelf = false, RankingSystem.RankingType rankType = RankingSystem.RankingType.None)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2604u);
			if (isSelf)
			{
				cSPkg.stPkgData.stGetRankingAcntInfoReq.bNumberType = (byte)RankingSystem.ConvertRankingServerType(rankType);
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void SendReqRankingDetail()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2604u);
			cSPkg.stPkgData.stGetRankingAcntInfoReq.bNumberType = 7;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void OnGetRankingList(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			Singleton<RankingSystem>.instance.ImpResRankingList(rsp);
		}

		public void ImpResRankingList(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			if (this._form == null)
			{
				return;
			}
			RankingSystem.RankingType rankingType = RankingSystem.ConvertRankingLocalType((COM_APOLLO_TRANK_SCORE_TYPE)rsp.stRankingListDetail.stOfSucc.bNumberType);
			if (rsp.stRankingListDetail.stOfSucc.iStart == 1)
			{
				this._rankingInfo[(int)rankingType].LastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
				if (rsp.stRankingListDetail.stOfSucc.bNumberType == 7)
				{
					this._rankingInfo[(int)rankingType].ListInfo = this.GetLadderRankingInfoList(rsp.stRankingListDetail.stOfSucc);
				}
				else if (rsp.stRankingListDetail.stOfSucc.bNumberType == 65)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
					uint dwSubType = rsp.dwSubType;
					RankingSystem.LocalRankingInfo value = default(RankingSystem.LocalRankingInfo);
					value.ListInfo = rsp.stRankingListDetail.stOfSucc;
					value.LastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
					bool flag = false;
					value.SelfInfo = new CSDT_GET_RANKING_ACNT_DETAIL_SELF();
					int num = 0;
					while ((long)num < (long)((ulong)value.ListInfo.dwItemNum))
					{
						COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAcntInfo = value.ListInfo.astItemDetail[num].stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo;
						if (stAcntInfo.ullUid == masterRoleInfo.playerUllUID)
						{
							value.SelfInfo.bNumberType = value.ListInfo.bNumberType;
							value.SelfInfo.dwScore = 0u;
							value.SelfInfo.dwRankNo = (uint)(num + 1);
							value.SelfInfo.iRankChgNo = 0;
							flag = true;
							break;
						}
						num++;
					}
					if (!flag)
					{
						value.SelfInfo.bNumberType = value.ListInfo.bNumberType;
						value.SelfInfo.dwScore = 0u;
						value.SelfInfo.dwRankNo = 0u;
						value.SelfInfo.iRankChgNo = 0;
					}
					if (!this._godRankInfo.ContainsKey(dwSubType))
					{
						this._godRankInfo.Add(dwSubType, value);
					}
					else
					{
						this._godRankInfo[dwSubType] = value;
					}
					this._rankingSelfReady = true;
					this._rankingListReady = true;
					this._rankingBackupListReady = true;
				}
				else
				{
					this._rankingInfo[(int)rankingType].ListInfo = rsp.stRankingListDetail.stOfSucc;
				}
				this._rankingListReady = true;
			}
			else if (rsp.stRankingListDetail.stOfSucc.iStart == 101)
			{
				this._rankingInfo[(int)rankingType].LastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
				this._rankingInfo[(int)rankingType].BackupListInfo = rsp.stRankingListDetail.stOfSucc;
				this._rankingBackupListReady = true;
			}
			if (rankingType == this._curRankingType && this._rankingListReady && this._rankingSelfReady && this._rankingBackupListReady)
			{
				this.UpdateGodTitle();
				this.UpdateAllElementInView();
				this.UpdateSelfInfo();
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<RankingSystem.RankingType>("Ranking_List_Change", rankingType);
			this.TryToUnlock();
		}

		public void OnGetRankingDailyRankMatch(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			if (rsp.stRankingListDetail.stOfSucc.bNumberType != 64)
			{
				return;
			}
			this._dailyRankMatchInfo = rsp.stRankingListDetail.stOfSucc;
		}

		private CSDT_RANKING_LIST_SUCC GetLadderRankingInfoList(CSDT_RANKING_LIST_SUCC realTimeInfo)
		{
			if (this.IsDailyRankMatchEmpty())
			{
				return realTimeInfo;
			}
			CSDT_RANKING_LIST_SUCC cSDT_RANKING_LIST_SUCC = new CSDT_RANKING_LIST_SUCC();
			cSDT_RANKING_LIST_SUCC.iLogicWorldID = realTimeInfo.iLogicWorldID;
			cSDT_RANKING_LIST_SUCC.dwTimeLimit = realTimeInfo.dwTimeLimit;
			cSDT_RANKING_LIST_SUCC.bNumberType = realTimeInfo.bNumberType;
			cSDT_RANKING_LIST_SUCC.iStart = realTimeInfo.iStart;
			cSDT_RANKING_LIST_SUCC.iLimit = realTimeInfo.iLimit;
			cSDT_RANKING_LIST_SUCC.bImage = realTimeInfo.bImage;
			cSDT_RANKING_LIST_SUCC.dwItemNum = realTimeInfo.dwItemNum;
			cSDT_RANKING_LIST_SUCC.astItemDetail = new CSDT_RANKING_LIST_ITEM_INFO[realTimeInfo.dwItemNum];
			bool[] array = new bool[realTimeInfo.dwItemNum];
			uint num = 0u;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this._dailyRankMatchInfo.dwItemNum))
			{
				int num3 = 0;
				while ((long)num3 < (long)((ulong)realTimeInfo.dwItemNum))
				{
					if (this._dailyRankMatchInfo.astItemDetail[num2].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == realTimeInfo.astItemDetail[num3].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid)
					{
						if (CLadderSystem.IsMaxRankGrade(realTimeInfo.astItemDetail[num3].stExtraInfo.stDetailInfo.stDailyRankMatch.stRankInfo.bShowGradeOfRank))
						{
							cSDT_RANKING_LIST_SUCC.astItemDetail[(int)((UIntPtr)num)] = realTimeInfo.astItemDetail[num3];
							cSDT_RANKING_LIST_SUCC.astItemDetail[(int)((UIntPtr)num)].dwRankNo = num + 1u;
							num += 1u;
							array[num3] = true;
						}
						break;
					}
					num3++;
				}
				num2++;
			}
			int num4 = 0;
			while ((long)num4 < (long)((ulong)realTimeInfo.dwItemNum))
			{
				if (!array[num4])
				{
					cSDT_RANKING_LIST_SUCC.astItemDetail[(int)((UIntPtr)num)] = realTimeInfo.astItemDetail[num4];
					cSDT_RANKING_LIST_SUCC.astItemDetail[(int)((UIntPtr)num)].dwRankNo = num + 1u;
					num += 1u;
				}
				num4++;
			}
			DebugHelper.Assert(num == realTimeInfo.dwItemNum, "resultInfoIndex != realTimeInfo.dwItemNum. Please check code!!!");
			return cSDT_RANKING_LIST_SUCC;
		}

		private bool IsDailyRankMatchEmpty()
		{
			return this._dailyRankMatchInfo == null || this._dailyRankMatchInfo.dwItemNum == 0u;
		}

		private void OnCoinSent(SCPKG_CMD_DONATE_FRIEND_POINT data)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			int friendTypeFromSrvFriendType = CFriendContoller.GetFriendTypeFromSrvFriendType((COM_FRIEND_TYPE)data.bFriendType);
			if (friendTypeFromSrvFriendType == -1)
			{
				Debug.LogError("Friend type" + data.bFriendType + " has not handle yet!");
				return;
			}
			CFriendModel.FriendType type = (CFriendModel.FriendType)friendTypeFromSrvFriendType;
			ulong ullUid = data.stUin.ullUid;
			uint dwLogicWorldId = data.stUin.dwLogicWorldId;
			stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(ullUid, dwLogicWorldId, type);
			int num = -1;
			if (this._coinSentFriendDic.TryGetValue(key, out num))
			{
				this._coinSentFriendDic.Remove(key);
			}
			else
			{
				num = -1;
			}
			if (this._curRankingType == RankingSystem.RankingType.Arena && num >= 0)
			{
				CUIListScript component = form.GetWidget(17).GetComponent<CUIListScript>();
				CUIListElementScript elemenet = component.GetElemenet(num);
				if (elemenet != null)
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Ranking_ArenaElementEnable;
					cUIEvent.m_srcFormScript = form;
					cUIEvent.m_srcWidget = elemenet.gameObject;
					cUIEvent.m_srcWidgetIndexInBelongedList = num;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				}
				return;
			}
			if (this._curRankingType == RankingSystem.RankingType.God && num >= 0)
			{
				return;
			}
			if (num >= 0 && this._rankingList != null)
			{
				CUIListElementScript elemenet2 = this._rankingList.GetElemenet(num);
				if (elemenet2 != null)
				{
					CUIEvent cUIEvent2 = new CUIEvent();
					cUIEvent2.m_eventID = enUIEventID.Ranking_ElementEnable;
					cUIEvent2.m_srcFormScript = form;
					cUIEvent2.m_srcWidget = elemenet2.gameObject;
					cUIEvent2.m_srcWidgetIndexInBelongedList = num;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
					return;
				}
			}
			if (form != null)
			{
				this.UpdateAllElementInView();
			}
		}

		public void OnGetRankingAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
		{
			Singleton<RankingSystem>.instance.ImpResRankingDetail(rsp);
		}

		public void ImpResRankingDetail(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
		{
			RankingSystem.RankingType rankingType = RankingSystem.ConvertRankingLocalType((COM_APOLLO_TRANK_SCORE_TYPE)rsp.stAcntRankingDetail.stOfSucc.bNumberType);
			this._rankingInfo[(int)rankingType].LastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
			this._rankingInfo[(int)rankingType].SelfInfo = rsp.stAcntRankingDetail.stOfSucc;
			if (rankingType == RankingSystem.RankingType.Ladder)
			{
				this._rankingInfo[(int)rankingType].SelfInfo.dwRankNo = this.GetLadderRankNo(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("Rank_Friend_List");
			if (this._form == null)
			{
				return;
			}
			this._rankingSelfReady = true;
			if (rankingType == this._curRankingType && this._rankingSelfReady && this._rankingListReady)
			{
				this.UpdateAllElementInView();
				this.UpdateSelfInfo();
			}
			this.TryToUnlock();
			this.ReqRankingList(RankingSystem.ConvertRankingServerType(RankingSystem.RankingType.Achievement), 0);
		}

		private void TryToUnlock()
		{
			if (!this._rankingListReady || !this._rankingSelfReady || !this._rankingBackupListReady)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			this._rankingListReady = (this._rankingSelfReady = (this._rankingBackupListReady = false));
		}

		private COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER GetRankItemDetailInfo(RankingSystem.RankingType rankType, int listIndex, uint subType = 0u)
		{
			RankingSystem.LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(rankType, subType);
			CSDT_RANKING_LIST_ITEM_INFO cSDT_RANKING_LIST_ITEM_INFO;
			if (localRankingInfo.ListInfo != null && listIndex < localRankingInfo.ListInfo.astItemDetail.Length && (long)listIndex < (long)((ulong)localRankingInfo.ListInfo.dwItemNum))
			{
				cSDT_RANKING_LIST_ITEM_INFO = localRankingInfo.ListInfo.astItemDetail[listIndex];
			}
			else if (localRankingInfo.BackupListInfo != null && listIndex - 100 >= 0 && listIndex - 100 < localRankingInfo.BackupListInfo.astItemDetail.Length && (long)(listIndex - 100) < (long)((ulong)localRankingInfo.BackupListInfo.dwItemNum))
			{
				cSDT_RANKING_LIST_ITEM_INFO = localRankingInfo.BackupListInfo.astItemDetail[listIndex - 100];
			}
			else
			{
				cSDT_RANKING_LIST_ITEM_INFO = new CSDT_RANKING_LIST_ITEM_INFO();
			}
			switch (rankType)
			{
			case RankingSystem.RankingType.PvpRank:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stPvpExp;
			case RankingSystem.RankingType.Power:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stPower;
			case RankingSystem.RankingType.HeroNum:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stHeroNum;
			case RankingSystem.RankingType.SkinNum:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stSkinNum;
			case RankingSystem.RankingType.Ladder:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stLadderPoint;
			case RankingSystem.RankingType.Achievement:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stAchievement;
			case RankingSystem.RankingType.WinCount:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stWinGameNum;
			case RankingSystem.RankingType.ConWinCount:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stContinousWin;
			case RankingSystem.RankingType.ConsumeQuan:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stUseCoupons;
			case RankingSystem.RankingType.MentorPoint:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterPoint;
			case RankingSystem.RankingType.GameVip:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stVipScore;
			case RankingSystem.RankingType.God:
				return cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo;
			default:
				return new COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER();
			}
		}

		public CSDT_RANKING_LIST_SUCC GetRankList(RankingSystem.RankingType rankingType)
		{
			return this._rankingInfo[(int)rankingType].ListInfo;
		}

		public uint GetRankClass(ulong playerUid)
		{
			if (this._dailyRankMatchInfo != null)
			{
				int num = 0;
				while ((long)num < (long)((ulong)this._dailyRankMatchInfo.dwItemNum))
				{
					if (this._dailyRankMatchInfo.astItemDetail[num].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == playerUid)
					{
						return this._dailyRankMatchInfo.astItemDetail[num].dwRankNo;
					}
					num++;
				}
			}
			return 0u;
		}

		public uint GetLadderRankNo(ulong playerUid)
		{
			CSDT_RANKING_LIST_SUCC listInfo = this._rankingInfo[4].ListInfo;
			if (listInfo != null)
			{
				int num = 0;
				while ((long)num < (long)((ulong)listInfo.dwItemNum))
				{
					if (listInfo.astItemDetail[num].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == playerUid)
					{
						return listInfo.astItemDetail[num].dwRankNo;
					}
					num++;
				}
			}
			return 0u;
		}
	}
}

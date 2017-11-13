using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CUnionBattleRankSystem : Singleton<CUnionBattleRankSystem>
	{
		public enum enWidget
		{
			enWidGet_RankList,
			enWidGet_SelfInfo,
			enWidGet_MatchTypeMenu
		}

		protected struct stUnionRankInfo
		{
			public uint lastRetrieveTime;

			public CSDT_RANKING_LIST_SUCC listInfo;

			public int selfIndex;
		}

		private const int DAY_REWARD_NUM = 6;

		private CUnionBattleRankSystem.stUnionRankInfo[] m_UnionRankInfo = new CUnionBattleRankSystem.stUnionRankInfo[4];

		public static string UNION_RANK_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionRank";

		private enUnionRankType m_CurSelRankType = enUnionRankType.enRankType_None;

		private enUnionRankMatchType m_CurSelRankMatchType = enUnionRankMatchType.enRankMatchType_None;

		private int m_CurSelRankItemIndex = -1;

		private uint m_CurSelMapId;

		private ResRewardMatchLevelInfo m_CurMapInfo = new ResRewardMatchLevelInfo();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_Rank, new CUIEventManager.OnUIEventHandler(this.OnClickRank));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_MatchType_Menu, new CUIEventManager.OnUIEventHandler(this.OnClickMatchTypeMenu));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_ClickDetail, new CUIEventManager.OnUIEventHandler(this.OnClickDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_DateList_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("UnionRank_Get_Rank_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankList));
			Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("UnionRank_Get_Rank_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetAccountInfo));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_Rank, new CUIEventManager.OnUIEventHandler(this.OnClickRank));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_MatchType_Menu, new CUIEventManager.OnUIEventHandler(this.OnClickMatchTypeMenu));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Rank_ClickDetail, new CUIEventManager.OnUIEventHandler(this.OnClickDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_DateList_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_LIST_RSP>("UnionRank_Get_Rank_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankList));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("UnionRank_Get_Rank_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetAccountInfo));
		}

		public void Clear()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_UnionRankInfo[i].lastRetrieveTime = 0u;
				this.m_UnionRankInfo[i].listInfo = null;
				this.m_UnionRankInfo[i].selfIndex = -1;
			}
		}

		private void OnClickRank(CUIEvent uiEvt)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleRankSystem.UNION_RANK_PATH, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.Clear();
			this.initWidget();
			CUIListScript component = cUIFormScript.GetWidget(2).GetComponent<CUIListScript>();
			if (component && component.GetElementAmount() > 0)
			{
				component.SelectElement(0, true);
				CUIEventScript component2 = component.GetElemenet(0).GetComponent<CUIEventScript>();
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(component2.m_onClickEventID, component2.m_onClickEventParams);
			}
		}

		private void OnClickMatchTypeMenu(CUIEvent uiEvt)
		{
			this.SelectRankMatchType(uiEvt);
		}

		private void OnElementEnable(CUIEvent uiEvt)
		{
			this.RefreshOneWinCntElement(uiEvt.m_srcWidget, uiEvt.m_srcWidgetIndexInBelongedList);
		}

		private void OnGetRankList(SCPKG_GET_RANKING_LIST_RSP rankList)
		{
			enUnionRankType enUnionRankType = CUnionBattleRankSystem.ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE)rankList.stRankingListDetail.stOfSucc.bNumberType);
			if (enUnionRankType == enUnionRankType.enRankType_None)
			{
				return;
			}
			this.m_UnionRankInfo[(int)enUnionRankType].lastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
			this.m_UnionRankInfo[(int)enUnionRankType].listInfo = rankList.stRankingListDetail.stOfSucc;
			CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int)enUnionRankType].listInfo;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			this.m_UnionRankInfo[(int)enUnionRankType].selfIndex = -1;
			int num = 0;
			while ((long)num < (long)((ulong)listInfo.dwItemNum))
			{
				COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(enUnionRankType, num);
				if (masterRoleInfo.playerUllUID == rankItemDetailInfo.ullUid)
				{
					this.m_UnionRankInfo[(int)enUnionRankType].selfIndex = num;
				}
				num++;
			}
			this.RefreshWinCntRankList();
			this.RefreshAcntInfo();
		}

		private void OnGetAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP acntInfo)
		{
			enUnionRankType enUnionRankType = CUnionBattleRankSystem.ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE)acntInfo.stAcntRankingDetail.stOfSucc.bNumberType);
			if (enUnionRankType == enUnionRankType.enRankType_None)
			{
				return;
			}
			this.m_UnionRankInfo[(int)enUnionRankType].lastRetrieveTime = (uint)CRoleInfo.GetCurrentUTCTime();
			this.RefreshAcntInfo();
		}

		private void OnClickDetail(CUIEvent uiEvt)
		{
			int selectedIndex = uiEvt.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetSelectedIndex();
			this.m_CurSelRankItemIndex = selectedIndex;
			COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this.m_CurSelRankType, this.m_CurSelRankItemIndex);
			if (rankItemDetailInfo != null)
			{
				ulong ullUid = rankItemDetailInfo.ullUid;
				int iLogicWorldId = rankItemDetailInfo.iLogicWorldId;
				if (ullUid == Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID && iLogicWorldId == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID)
				{
					Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(ullUid, iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Base_Info);
				}
				else
				{
					Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(ullUid, iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
				}
			}
		}

		public void initWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleRankSystem.UNION_RANK_PATH);
			if (form != null)
			{
				this.m_CurSelRankType = enUnionRankType.enRankType_None;
				this.m_CurSelRankMatchType = enUnionRankMatchType.enRankMatchType_None;
				this.m_CurSelRankItemIndex = -1;
				this.m_CurSelMapId = 0u;
				CUIListScript component = form.GetWidget(2).GetComponent<CUIListScript>();
				int unionBattleMapCount = CUnionBattleEntrySystem.GetUnionBattleMapCount();
				component.SetElementAmount(unionBattleMapCount);
				int num = 0;
				for (int i = 0; i < unionBattleMapCount; i++)
				{
					ResRewardMatchLevelInfo unionBattleMapInfoByIndex = CUnionBattleEntrySystem.GetUnionBattleMapInfoByIndex(i);
					if (CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, unionBattleMapInfoByIndex.dwMapId))
					{
						CUIListElementScript elemenet = component.GetElemenet(num);
						if (elemenet != null)
						{
							CUIEventScript component2 = elemenet.GetComponent<CUIEventScript>();
							elemenet.transform.FindChild("Text").GetComponent<Text>().set_text(unionBattleMapInfoByIndex.szMatchName);
							component2.m_onClickEventParams.tagUInt = unionBattleMapInfoByIndex.dwMapId;
							component2.m_onClickEventParams.commonUInt32Param1 = unionBattleMapInfoByIndex.dwMatchType;
						}
						num++;
					}
				}
				if (num != unionBattleMapCount)
				{
					component.SetElementAmount(num);
				}
			}
		}

		private void SelectRankMatchType(CUIEvent uiEvt)
		{
			uint tagUInt = uiEvt.m_eventParams.tagUInt;
			enUnionRankMatchType commonUInt32Param = (enUnionRankMatchType)uiEvt.m_eventParams.commonUInt32Param1;
			if (this.m_CurSelMapId == tagUInt)
			{
				return;
			}
			this.m_CurSelMapId = tagUInt;
			this.m_CurSelRankMatchType = commonUInt32Param;
			this.m_CurMapInfo = CUnionBattleEntrySystem.GetUnionBattleMapInfoByMapID(this.m_CurSelMapId);
			this.m_CurSelRankType = CUnionBattleRankSystem.ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE)this.m_CurMapInfo.dwRankType);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleRankSystem.UNION_RANK_PATH);
			if (form == null)
			{
				return;
			}
			if (this.IsNeedToRetrieveRankTypeInfo(this.m_CurSelRankType))
			{
				this.RetrieveRankTypeInfo(this.m_CurSelRankType);
			}
			this.RefreshWinCntRankList();
			this.RefreshAcntInfo();
		}

		private void RefreshAcntInfo()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleRankSystem.UNION_RANK_PATH);
			if (form == null)
			{
				return;
			}
			CSDT_RANKING_LIST_ITEM_INFO actRankInfo = this.GetActRankInfo(this.m_CurSelRankType);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			GameObject widget = form.GetWidget(1);
			RankingItemHelper component = widget.GetComponent<RankingItemHelper>();
			uint num = 0u;
			if (actRankInfo != null && masterRoleInfo != null)
			{
				widget.CustomSetActive(true);
				string name = masterRoleInfo.Name;
				uint level = masterRoleInfo.Level;
				num = actRankInfo.dwRankNo;
				uint num2;
				if (num == 0u)
				{
					num2 = CUnionBattleEntrySystem.GetRewardMatchStateByMapId(this.m_CurSelMapId).dwPerfectCnt;
				}
				else
				{
					num2 = actRankInfo.dwRankScore;
				}
				widget.transform.FindChild("Value").gameObject.CustomSetActive(true);
				widget.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
				CUnionBattleRankSystem.SetGameObjChildText(widget, "Value", num2.ToString(CultureInfo.get_InvariantCulture()));
				CUnionBattleRankSystem.SetGameObjChildText(widget, "NameGroup/PlayerName", masterRoleInfo.Name);
				CUnionBattleRankSystem.SetGameObjChildText(widget, "PlayerLv", string.Format("Lv.{0}", level.ToString(CultureInfo.get_InvariantCulture())));
			}
			CUnionBattleRankSystem.RankNobSet(num, component);
			MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
			GameObject gameObject = widget.transform.FindChild("HeadItemCell").gameObject;
			CUICommonSystem.SetHostHeadItemCell(gameObject);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
		}

		private void RefreshWinCntRankList()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleRankSystem.UNION_RANK_PATH);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int)this.m_CurSelRankType].listInfo;
			Transform transform = widget.transform.FindChild("RankingList");
			Transform transform2 = widget.transform.FindChild("NoRankTxt");
			if (listInfo == null || listInfo.dwItemNum == 0u)
			{
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(true);
				return;
			}
			transform.gameObject.CustomSetActive(true);
			transform2.gameObject.CustomSetActive(false);
			int dwItemNum = (int)listInfo.dwItemNum;
			CUIListScript component = transform.GetComponent<CUIListScript>();
			component.SetElementAmount(dwItemNum);
			component.MoveElementInScrollArea(0, true);
			for (int i = 0; i < dwItemNum; i++)
			{
				if (component.GetElemenet(i) != null && component.IsElementInScrollArea(i))
				{
					this.RefreshOneWinCntElement(component.GetElemenet(i).gameObject, i);
				}
			}
		}

		private void RefreshOneWinCntElement(GameObject element, int index)
		{
			CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int)this.m_CurSelRankType].listInfo;
			if (element != null && listInfo != null && index < listInfo.astItemDetail.Length && (long)index < (long)((ulong)listInfo.dwItemNum))
			{
				RankingItemHelper component = element.GetComponent<RankingItemHelper>();
				string text = string.Empty;
				uint dwRankScore = listInfo.astItemDetail[index].dwRankScore;
				COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this.m_CurSelRankType, index);
				text = StringHelper.UTF8BytesToString(ref rankItemDetailInfo.szPlayerName);
				uint dwPvpLevel = rankItemDetailInfo.dwPvpLevel;
				string serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankItemDetailInfo.szHeadUrl);
				uint dwCurLevel = rankItemDetailInfo.stGameVip.dwCurLevel;
				uint dwHeadIconId = rankItemDetailInfo.stGameVip.dwHeadIconId;
				COM_PRIVILEGE_TYPE bPrivilege = (COM_PRIVILEGE_TYPE)rankItemDetailInfo.bPrivilege;
				uint dwVipLevel = rankItemDetailInfo.dwVipLevel;
				MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, bPrivilege, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
				MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, bPrivilege, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
				MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, bPrivilege, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
				CUnionBattleRankSystem.SetGameObjChildText(element, "NameGroup/PlayerName", text);
				CUnionBattleRankSystem.SetGameObjChildText(element, "PlayerLv", string.Format("Lv.{0}", Math.Max(1u, dwPvpLevel)));
				element.transform.FindChild("Value").gameObject.CustomSetActive(true);
				CUnionBattleRankSystem.SetGameObjChildText(element, "Value", dwRankScore.ToString(CultureInfo.get_InvariantCulture()));
				uint rankNumber = (uint)(index + 1);
				CUnionBattleRankSystem.RankNobSet(rankNumber, component);
				if (!CSysDynamicBlock.bSocialBlocked)
				{
					if (rankItemDetailInfo.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
					{
						MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
						RankingView.SetHostUrlHeadIcon(component.HeadIcon);
					}
					else
					{
						MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int)dwVipLevel);
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int)dwCurLevel, false, false, rankItemDetailInfo.ullUserPrivacyBits);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int)dwHeadIconId);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, bPrivilege, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, bPrivilege, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, bPrivilege, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
						RankingView.SetUrlHeadIcon(component.HeadIcon, serverUrl);
					}
				}
			}
		}

		private void RetrieveRankTypeInfo(enUnionRankType rankType)
		{
			COM_APOLLO_TRANK_SCORE_TYPE rankType2 = CUnionBattleRankSystem.ConvertLocalToSeverRankType(rankType);
			CUnionBattleRankSystem.ReqRankListInfo(rankType2);
		}

		public static enUnionRankType ConvertSeverToLocalRankType(COM_APOLLO_TRANK_SCORE_TYPE rankType)
		{
			enUnionRankType result = enUnionRankType.enRankType_None;
			if (rankType >= COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_LOW_COIN_WIN && rankType <= COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_HIGH_DIAMOND_WIN)
			{
				result = (enUnionRankType)(rankType - 60);
			}
			return result;
		}

		public static COM_APOLLO_TRANK_SCORE_TYPE ConvertLocalToSeverRankType(enUnionRankType rankType)
		{
			COM_APOLLO_TRANK_SCORE_TYPE result = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_NULL;
			if (rankType > enUnionRankType.enRankType_None && rankType < enUnionRankType.enRankType_Count)
			{
				result = (COM_APOLLO_TRANK_SCORE_TYPE)(rankType + 60);
			}
			return result;
		}

		public bool IsNeedToRetrieveRankTypeInfo(enUnionRankType rankType)
		{
			return rankType > enUnionRankType.enRankType_None && rankType < enUnionRankType.enRankType_Count && (this.m_UnionRankInfo[(int)rankType].listInfo == null || this.m_UnionRankInfo[(int)rankType].lastRetrieveTime == 0u || CRoleInfo.GetCurrentUTCTime() >= (int)(this.m_UnionRankInfo[(int)rankType].lastRetrieveTime + this.m_UnionRankInfo[(int)rankType].listInfo.dwTimeLimit));
		}

		private static void SetGameObjChildText(GameObject parentObj, string childName, string text)
		{
			if (parentObj != null)
			{
				Text component = parentObj.transform.FindChild(childName).gameObject.GetComponent<Text>();
				component.set_text(text);
			}
		}

		private static void RankNobSet(uint rankNumber, RankingItemHelper rankingHelper)
		{
			rankingHelper.RankingNumText.CustomSetActive(false);
			rankingHelper.No1.CustomSetActive(false);
			rankingHelper.No2.CustomSetActive(false);
			rankingHelper.No3.CustomSetActive(false);
			rankingHelper.No1BG.CustomSetActive(false);
			if (rankNumber == 0u)
			{
				if (rankingHelper.NoRankingText != null)
				{
					rankingHelper.NoRankingText.CustomSetActive(true);
				}
			}
			else
			{
				if (rankingHelper.NoRankingText != null)
				{
					rankingHelper.NoRankingText.CustomSetActive(false);
				}
				switch (rankNumber)
				{
				case 1u:
					rankingHelper.No1.CustomSetActive(true);
					if (rankingHelper.No1BG != null)
					{
						rankingHelper.No1BG.CustomSetActive(true);
					}
					break;
				case 2u:
					rankingHelper.No2.CustomSetActive(true);
					break;
				case 3u:
					rankingHelper.No3.CustomSetActive(true);
					break;
				default:
					rankingHelper.RankingNumText.CustomSetActive(true);
					rankingHelper.RankingNumText.GetComponent<Text>().set_text(string.Format("{0}", rankNumber));
					break;
				}
			}
		}

		private COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER GetRankItemDetailInfo(enUnionRankType rankType, int listIndex)
		{
			CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int)this.m_CurSelRankType].listInfo;
			if (listInfo == null || listIndex >= listInfo.astItemDetail.Length || (long)listIndex >= (long)((ulong)listInfo.dwItemNum))
			{
				return new COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER();
			}
			switch (rankType)
			{
			case enUnionRankType.enRankMatchType_WinCntCoinCoinMatchLow:
				return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stLowCoinWin;
			case enUnionRankType.enRankMatchType_WinCntCoinMatchHigh:
				return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stHighCoinWin;
			case enUnionRankType.enRankMatchType_WinCntDiamondMatchLow:
				return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stLowDiamondWin;
			case enUnionRankType.enRankMatchType_WinCntDiamondMatchHigh:
				return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stHighDiamondWin;
			default:
				return new COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER();
			}
		}

		private CSDT_RANKING_LIST_ITEM_INFO GetActRankInfo(enUnionRankType rankType)
		{
			CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int)this.m_CurSelRankType].listInfo;
			int selfIndex = this.m_UnionRankInfo[(int)this.m_CurSelRankType].selfIndex;
			if (listInfo == null || selfIndex < 0 || selfIndex >= listInfo.astItemDetail.Length || (long)selfIndex >= (long)((ulong)listInfo.dwItemNum))
			{
				return new CSDT_RANKING_LIST_ITEM_INFO();
			}
			return listInfo.astItemDetail[selfIndex];
		}

		public static void ReqRankListInfo(COM_APOLLO_TRANK_SCORE_TYPE rankType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2602u);
			cSPkg.stPkgData.stGetRankingListReq.bNumberType = (byte)rankType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}
	}
}

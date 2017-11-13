using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CUnionBattleEntrySystem : Singleton<CUnionBattleEntrySystem>
	{
		private enum enUnionBattleEntryWidget
		{
			enEntry_Btn1,
			enEntry_Btn2,
			enEntry_PlayerCount
		}

		private enum enUnionSecBattleEntryWidget
		{
			enEntry_Btn1,
			enEntry_Btn2,
			enEntry_Btn3
		}

		private enum enUnionConfirmEntryWidget
		{
			enIntroTxt,
			enOpenTimesTxt,
			enBtnStart,
			enTicketItem
		}

		private enum enUnionThirdEntryWidget
		{
			enEntry_WinCountTxt,
			enEntry_OpenTimeTxt,
			enEntry_PanelLoseTimes,
			enEntry_StartFightBtn,
			enEntry_GetAwardBtn,
			enEntry_HeadItemCell,
			enEntry_TicketItemCell,
			enEntry_MapName,
			enEntry_MatchName,
			enEntry_PanelRight,
			enEntry_LoseTwoTimesTips
		}

		private enum enUnionRewardIntroWidget
		{
			enEntry_RewardItems
		}

		public enum enLastWinLoseCntChgType
		{
			enChgType_None,
			enChgType_Win,
			enChgType_Lose
		}

		public struct stLastWinLoseCntChginfo
		{
			public uint mapId;

			public CUnionBattleEntrySystem.enLastWinLoseCntChgType winLoseChgType;
		}

		private const RES_BATTLE_MAP_TYPE m_ResMapType = RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH;

		public static string UNION_ENTRY_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntry";

		public static string UNION_ENTRY_SECOND_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntrySecond";

		public static string UNION_CONFIRM_ENTRY_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleConfirmEntry";

		public static string UNION_ENTRY_THIRD_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntryThird";

		public static string UNION_ENTRY_REWARDINTRO_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleRewardIntro";

		private uint m_selectMapID;

		private ResRewardMatchLevelInfo m_selectMapRes;

		private ResRewardMatchTimeInfo m_selectTimeInfo;

		private COMDT_REWARDMATCH_RECORD m_selectStateInfo;

		public SCPKG_GETAWARDPOOL_RSP m_awardPoolInfo = new SCPKG_GETAWARDPOOL_RSP();

		public SCPKG_MATCHPOINT_NTF m_personInfo = new SCPKG_MATCHPOINT_NTF();

		public SCPKG_GET_MATCHINFO_RSP m_baseInfo = new SCPKG_GET_MATCHINFO_RSP();

		public COMDT_REWARDMATCH_DATA m_stateInfo = new COMDT_REWARDMATCH_DATA();

		public CUnionBattleEntrySystem.stLastWinLoseCntChginfo[] m_WinLoseChgInfo;

		private readonly int m_unionBattleRuleId = 10;

		private readonly int m_unionConfirmEntryRuleId = 13;

		public static byte MAX_LOSE_TIME = 3;

		private uint maxMatchRoundCnt;

		private static readonly uint SECOND_ONE_DAY = 86400u;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_ClickEntry, new CUIEventManager.OnUIEventHandler(this.Open_BattleEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_BattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.Open_SecondBattleEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_SubBattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnClickSecondUIBattleEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_SingleStartMatch, new CUIEventManager.OnUIEventHandler(this.OnClickStartSingleMatch));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_ConfirmBuyItem, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_BuyTiketClick, new CUIEventManager.OnUIEventHandler(this.OnBuyTiketClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_Rule, new CUIEventManager.OnUIEventHandler(this.OnClickRule));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_RewardMatch_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRewardMatchTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_battle_Click_StartOneMatchRound, new CUIEventManager.OnUIEventHandler(this.OnClickStartMatch));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_RewardIntro, new CUIEventManager.OnUIEventHandler(this.OnClickRewardIntro));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_GetReward, new CUIEventManager.OnUIEventHandler(this.OnClickGetReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_GotReward, new CUIEventManager.OnUIEventHandler(this.OnGotReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_RewardIntro_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRewardElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_ExChgRewwad, new CUIEventManager.OnUIEventHandler(this.OnClickExChgReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_CliCk_LoseTips, new CUIEventManager.OnUIEventHandler(this.OnClickLoseTips));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_GetRewardTips, new CUIEventManager.OnUIEventHandler(this.OnClickGetRewardTips));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_BattleVideo, new CUIEventManager.OnUIEventHandler(this.OnClickBattleVideo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_BattleNews, new CUIEventManager.OnUIEventHandler(this.OnClickBattleNews));
			int unionBattleMapCount = CUnionBattleEntrySystem.GetUnionBattleMapCount();
			this.m_WinLoseChgInfo = new CUnionBattleEntrySystem.stLastWinLoseCntChginfo[unionBattleMapCount];
			for (int i = 0; i < unionBattleMapCount; i++)
			{
				this.m_WinLoseChgInfo[i].mapId = CUnionBattleEntrySystem.GetUnionBattleMapInfoByIndex(i).dwMapId;
			}
			this.maxMatchRoundCnt = GameDataMgr.globalInfoDatabin.GetDataByKey(200u).dwConfValue;
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_ClickEntry, new CUIEventManager.OnUIEventHandler(this.Open_BattleEntry));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_BattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.Open_SecondBattleEntry));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_SubBattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnClickSecondUIBattleEntry));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_SingleStartMatch, new CUIEventManager.OnUIEventHandler(this.OnClickStartSingleMatch));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_ConfirmBuyItem, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyItem));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_BuyTiketClick, new CUIEventManager.OnUIEventHandler(this.OnBuyTiketClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_Rule, new CUIEventManager.OnUIEventHandler(this.OnClickRule));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_RewardMatch_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRewardMatchTimeUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_battle_Click_StartOneMatchRound, new CUIEventManager.OnUIEventHandler(this.OnClickStartMatch));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_RewardIntro, new CUIEventManager.OnUIEventHandler(this.OnClickRewardIntro));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_GetReward, new CUIEventManager.OnUIEventHandler(this.OnClickGetReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_GotReward, new CUIEventManager.OnUIEventHandler(this.OnGotReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_RewardIntro_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRewardElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_ExChgRewwad, new CUIEventManager.OnUIEventHandler(this.OnClickExChgReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_CliCk_LoseTips, new CUIEventManager.OnUIEventHandler(this.OnClickLoseTips));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_GetRewardTips, new CUIEventManager.OnUIEventHandler(this.OnClickGetRewardTips));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_BattleVideo, new CUIEventManager.OnUIEventHandler(this.OnClickBattleVideo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_BattleNews, new CUIEventManager.OnUIEventHandler(this.OnClickBattleNews));
			this.m_awardPoolInfo = null;
			this.m_personInfo = null;
			this.m_baseInfo = null;
			this.m_stateInfo = null;
			this.m_WinLoseChgInfo = null;
		}

		private void Open_BattleEntry(CUIEvent uiEvt)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			if (this.IsUnionBattleOpen(uiEvt))
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_ENTRY_PATH, false, true);
				this.initFirstFormWidget();
				CUnionBattleEntrySystem.SendGetUnionBattleBaseInfoReq();
			}
		}

		public bool IsUnionBattleOpen(CUIEvent uiEvt)
		{
			if (this.IsUnionFuncLocked())
			{
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips2", true, 1.5f, null, new object[0]);
				return false;
			}
			if (Singleton<CMatchingSystem>.instance.IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return false;
			}
			return true;
		}

		private void Open_SecondBattleEntry(CUIEvent uiEvt)
		{
			int tag = uiEvt.m_eventParams.tag;
			if (tag != 0)
			{
				if (tag == 1)
				{
					Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips3", true, 1.5f, null, new object[0]);
				}
			}
			else
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_ENTRY_SECOND_PATH, false, true);
				CUnionBattleEntrySystem.SendGetUnionBattleStateReq();
				this.initSecondFormWidget();
				Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(4u, null, false);
			}
		}

		private void OnClickSecondUIBattleEntry(CUIEvent uiEvt)
		{
			this.m_selectMapID = uiEvt.m_eventParams.tagUInt;
			this.m_selectMapRes = CUnionBattleEntrySystem.GetUnionBattleMapInfoByMapID(this.m_selectMapID);
			this.m_selectStateInfo = this.GetMapStateInfo(this.m_selectMapID);
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(5u, this.m_selectMapID), out this.m_selectTimeInfo);
			switch (uiEvt.m_eventParams.tag)
			{
			case 0:
			case 1:
				if (!uiEvt.m_eventParams.commonBool)
				{
					Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips15", true, 1.5f, null, new object[0]);
					return;
				}
				if (this.m_selectStateInfo == null)
				{
					return;
				}
				if (this.m_selectStateInfo.bState == 0)
				{
					Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_CONFIRM_ENTRY_PATH, false, true);
					this.initConfirmFormWidget();
				}
				else
				{
					CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_ENTRY_THIRD_PATH, false, true);
					this.initThirdFormWidget();
				}
				break;
			case 2:
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips3", true, 1.5f, null, new object[0]);
				break;
			}
		}

		private void OnClickStartMatch(CUIEvent uiEvt)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			stMatchOpenInfo matchOpenState = CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, this.m_selectMapID);
			if (matchOpenState.matchState == enMatchOpenState.enMatchOpen_InActiveTime && (uint)this.m_selectStateInfo.bMatchCnt < this.maxMatchRoundCnt)
			{
				int num = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID) + useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumFreeItemID);
				int dwCousumItemNum = (int)this.m_selectMapRes.dwCousumItemNum;
				if (num >= dwCousumItemNum)
				{
					int num2 = 10;
					if ((int)this.m_selectStateInfo.bMatchCnt < num2 || this.m_selectStateInfo.dwClearTime < Utility.GetGlobalRefreshTimeSeconds())
					{
						CUnionBattleEntrySystem.SendChgMatchStateReq(this.m_selectMapID, 1);
					}
				}
				else
				{
					int num3 = dwCousumItemNum - num;
					CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, num3);
					if (cUseable != null)
					{
						int payValue = (int)(cUseable.GetBuyPrice((RES_SHOPBUY_COINTYPE)this.m_selectMapRes.bCoinType) * (uint)num3);
						enPayType payType = CMallSystem.ResBuyTypeToPayType((int)this.m_selectMapRes.bCoinType);
						stUIEventParams stUIEventParams = default(stUIEventParams);
						stUIEventParams.tag = num3;
						CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips5", new string[]
						{
							num3.ToString(),
							cUseable.m_name
						}), payType, (uint)payValue, enUIEventID.Union_Battle_ConfirmBuyItem, ref stUIEventParams, enUIEventID.None, true, true, false);
					}
				}
			}
			else if (matchOpenState.matchState != enMatchOpenState.enMatchOpen_InActiveTime)
			{
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips4", true, 1.5f, null, new object[0]);
			}
			else if ((uint)this.m_selectStateInfo.bMatchCnt >= this.maxMatchRoundCnt)
			{
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips17", true, 1.5f, null, new object[0]);
			}
		}

		private void OnBuyTiketClick(CUIEvent uiEvt)
		{
			CUIEvent cUIEvent = new CUIEvent();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.tagUInt = this.m_selectMapRes.dwConsumPayItemID;
			eventParams.tag = (int)this.m_selectMapRes.bCoinType;
			cUIEvent.m_srcFormScript = uiEvt.m_srcFormScript;
			cUIEvent.m_srcWidget = uiEvt.m_srcWidget;
			cUIEvent.m_eventParams = eventParams;
			BuyPickDialog.Show(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, eventParams.tagUInt, (RES_SHOPBUY_COINTYPE)eventParams.tag, 10000f, 99u, null, null, new BuyPickDialog.OnConfirmBuyCommonDelegate(this.OnBuyPickDialogConfirm), cUIEvent);
		}

		private void OnBuyPickDialogConfirm(CUIEvent uiEvent, uint count)
		{
			int num = (int)count;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			int tag = uiEvent.m_eventParams.tag;
			CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, tagUInt, num);
			if (cUseable != null)
			{
				int payValue = (int)(cUseable.GetBuyPrice((RES_SHOPBUY_COINTYPE)tag) * (uint)num);
				enPayType payType = CMallSystem.ResBuyTypeToPayType(tag);
				stUIEventParams stUIEventParams = default(stUIEventParams);
				stUIEventParams.tag = num;
				CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips5", new string[]
				{
					num.ToString(),
					cUseable.m_name
				}), payType, (uint)payValue, enUIEventID.Union_Battle_ConfirmBuyItem, ref stUIEventParams, enUIEventID.None, true, true, false);
			}
		}

		private void OnConfirmBuyItem(CUIEvent uiEvt)
		{
			CUnionBattleEntrySystem.SendBuyTicketRequest(this.m_selectMapID, (uint)uiEvt.m_eventParams.tag);
		}

		private void OnClickRule(CUIEvent uiEvt)
		{
			int unionBattleRuleId = this.m_unionBattleRuleId;
			Singleton<CUIManager>.GetInstance().OpenInfoForm(unionBattleRuleId);
		}

		private void OnRewardMatchTimeUp(CUIEvent uiEvt)
		{
			this.initSecondFormWidget();
		}

		private void OnClickStartSingleMatch(CUIEvent uiEvt)
		{
			this.SendBeginMatchReq();
		}

		private void OnClickRewardIntro(CUIEvent uiEvt)
		{
			Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_ENTRY_REWARDINTRO_PATH, false, true);
			this.initRewardIntroFormWidget();
		}

		private void OnClickGetReward(CUIEvent uiEvt)
		{
			CUnionBattleEntrySystem.SendChgMatchStateReq(this.m_selectMapID, 0);
		}

		private void OnGotReward(CUIEvent uiEvt)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CUnionBattleEntrySystem.UNION_ENTRY_THIRD_PATH);
		}

		private void initFirstFormWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_ENTRY_PATH);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(1);
			if (widget == null || widget2 == null)
			{
				return;
			}
			widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
			widget2.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(171u).dwConfValue;
			if ((ulong)this.m_baseInfo.dwPlayerNum >= (ulong)((long)dwConfValue))
			{
				string text = Singleton<CTextManager>.instance.GetText("Union_Battle_Tips12", new string[]
				{
					this.m_baseInfo.dwPlayerNum.ToString()
				});
				CUICommonSystem.SetTextContent(form.GetWidget(2), text);
				CUICommonSystem.SetObjActive(form.GetWidget(2), true);
			}
			else
			{
				CUICommonSystem.SetObjActive(form.GetWidget(2), false);
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				if (widget2)
				{
					widget2.CustomSetActive(false);
				}
				Transform transform = form.transform.FindChild("panelBottom/ButtonBattleVideo");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
				Transform transform2 = form.transform.FindChild("panelBottom/ButtonBattleNews");
				if (transform2)
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
		}

		private void initSecondFormWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_ENTRY_SECOND_PATH);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(1);
			GameObject widget3 = form.GetWidget(2);
			if (widget == null || widget2 == null || widget3 == null)
			{
				return;
			}
			uint[] array = new uint[10];
			bool[] array2 = new bool[10];
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Union_1"), ref array[0]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Union_2"), ref array[1]);
			array2[0] = CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, array[0]);
			array2[1] = CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, array[1]);
			widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
			widget2.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
			widget3.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 2;
			widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = array[0];
			widget2.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = array[1];
			widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.commonBool = array2[0];
			widget2.GetComponent<CUIMiniEventScript>().m_onClickEventParams.commonBool = array2[1];
			widget.transform.FindChild("Lock").gameObject.CustomSetActive(!array2[0]);
			widget2.transform.FindChild("Lock").gameObject.CustomSetActive(!array2[1]);
			this.ShowCountDownTime(widget, array2[0]);
			this.ShowCountDownTime(widget2, array2[1]);
			widget.transform.FindChild("Desc/MapNameTxt").GetComponent<Text>().set_text(CUnionBattleEntrySystem.GetUnionBattleMapInfoByMapID(widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt).stLevelCommonInfo.szName);
			widget2.transform.FindChild("Desc/MapNameTxt").GetComponent<Text>().set_text(CUnionBattleEntrySystem.GetUnionBattleMapInfoByMapID(widget2.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt).stLevelCommonInfo.szName);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				if (widget3)
				{
					widget3.CustomSetActive(false);
				}
				if (widget2)
				{
					widget2.CustomSetActive(false);
				}
				GameObject gameObject = form.transform.FindChild("panelBottom/Button").gameObject;
				if (gameObject)
				{
					gameObject.CustomSetActive(false);
				}
			}
		}

		private void initConfirmFormWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_CONFIRM_ENTRY_PATH);
			if (form == null)
			{
				return;
			}
			Text component = form.GetWidget(0).GetComponent<Text>();
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long)this.m_unionConfirmEntryRuleId);
			component.set_text(dataByKey.szContent);
			Text component2 = form.GetWidget(1).GetComponent<Text>();
			component2.set_text(string.Format(component2.get_text(), this.m_selectStateInfo.bMatchCnt, this.maxMatchRoundCnt));
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(3);
			GameObject gameObject = widget.transform.FindChild("lblIconCount").gameObject;
			Text component3 = gameObject.GetComponent<Text>();
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
			CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
			CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false, false, false);
			component3.set_text(string.Format("x{0}", useableStackCount));
			gameObject.CustomSetActive(true);
		}

		private void initThirdFormWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_ENTRY_THIRD_PATH);
			if (form == null)
			{
				return;
			}
			if (this.m_selectStateInfo == null)
			{
				return;
			}
			Transform transform = form.GetWidget(2).transform;
			int bLossCnt = (int)this.m_selectStateInfo.bLossCnt;
			GameObject[] array = new GameObject[(int)CUnionBattleEntrySystem.MAX_LOSE_TIME];
			array[0] = transform.FindChild("loseicon_1").gameObject;
			array[1] = transform.FindChild("loseicon_2").gameObject;
			array[2] = transform.FindChild("loseicon_3").gameObject;
			for (int i = 0; i < (int)CUnionBattleEntrySystem.MAX_LOSE_TIME; i++)
			{
				if (i + 1 <= bLossCnt)
				{
					array[i].CustomSetActive(true);
				}
				else
				{
					array[i].CustomSetActive(false);
				}
			}
			if (bLossCnt == 2 && this.m_selectStateInfo.bState != 2)
			{
				GameObject gameObject = form.GetWidget(9).gameObject;
				CUICommonSystem.PlayAnimator(gameObject, "TipsLose_Anim_In_2");
				Text component = form.GetWidget(10).transform.FindChild("Text").GetComponent<Text>();
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips20"));
			}
			Transform transform2 = form.GetWidget(0).transform;
			CUnionBattleEntrySystem.stLastWinLoseCntChginfo stLastWinLoseCntChginfo = default(CUnionBattleEntrySystem.stLastWinLoseCntChginfo);
			CUnionBattleEntrySystem.GetWinloSeCntChgInfo(this.m_selectMapID, ref stLastWinLoseCntChginfo);
			if (stLastWinLoseCntChginfo.winLoseChgType == CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_Win && this.m_selectStateInfo.bWinCnt != 0)
			{
				CUICommonSystem.PlayAnimator(transform2.gameObject, string.Format("VictoryTimes_{0}-{1}", ((int)(this.m_selectStateInfo.bWinCnt - 1)).ToString(), this.m_selectStateInfo.bWinCnt.ToString()));
				CUnionBattleEntrySystem.SetWinLoseCntChgInfo(this.m_selectMapID, CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_None);
				transform2.FindChild("Img_WinCnt").gameObject.CustomSetActive(false);
			}
			else
			{
				GameObject gameObject2 = transform2.FindChild("Img_WinCnt").gameObject;
				gameObject2.GetComponent<Image>().SetSprite(string.Format("{0}{1}{2}", "UGUI/Sprite/System/", "UnionBattle/", this.m_selectStateInfo.bWinCnt), form, true, false, false, false);
				gameObject2.CustomSetActive(true);
				CUnionBattleEntrySystem.SetWinLoseCntChgInfo(this.m_selectMapID, CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_None);
			}
			if (stLastWinLoseCntChginfo.winLoseChgType == CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_Lose)
			{
				CUICommonSystem.PlayAnimator(transform.gameObject, string.Format("Lose_Anim_{0}", this.m_selectStateInfo.bLossCnt));
				CUnionBattleEntrySystem.SetWinLoseCntChgInfo(this.m_selectMapID, CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_None);
			}
			if (this.m_selectMapRes != null)
			{
				form.GetWidget(7).GetComponent<Text>().set_text(this.m_selectMapRes.stLevelCommonInfo.szName);
				form.GetWidget(8).GetComponent<Text>().set_text(this.m_selectMapRes.szMatchName);
			}
			ResRewardMatchTimeInfo resRewardMatchTimeInfo = null;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(5u, this.m_selectMapID), out resRewardMatchTimeInfo);
			if (resRewardMatchTimeInfo != null)
			{
				Text component2 = form.GetWidget(1).GetComponent<Text>();
				component2.set_text(resRewardMatchTimeInfo.szTimeTips);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (this.m_selectMapRes != null)
			{
				int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
				CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
				GameObject widget = form.GetWidget(6);
				Text component3 = widget.transform.Find("lblIconCount").GetComponent<Text>();
				CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false, false, false);
				component3.set_text("x" + useableStackCount.ToString());
				component3.gameObject.CustomSetActive(true);
				GameObject widget2 = form.GetWidget(5);
				CUICommonSystem.SetHostHeadItemCell(widget2);
				GameObject widget3 = form.GetWidget(3);
				GameObject widget4 = form.GetWidget(4);
				bool flag = this.m_selectStateInfo.bState == 2;
				widget3.CustomSetActive(!flag);
				widget4.CustomSetActive(flag);
				if (flag)
				{
					GameObject gameObject3 = form.GetWidget(9).gameObject;
					CUICommonSystem.PlayAnimator(gameObject3, "TipsAward_Anim_In_2");
					Text component4 = widget4.transform.FindChild("Tips/Text").GetComponent<Text>();
					if (this.m_selectStateInfo.bWinCnt == this.m_selectMapRes.bWinCnt)
					{
						component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips18"));
					}
					else
					{
						component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips19"));
					}
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform3 = form.transform.FindChild("Root/PanelRight/Btn_award");
				if (transform3)
				{
					transform3.gameObject.CustomSetActive(false);
				}
			}
		}

		private void initTirdFormTicket()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_ENTRY_THIRD_PATH);
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
			CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
			GameObject widget = form.GetWidget(6);
			Text component = widget.transform.Find("lblIconCount").GetComponent<Text>();
			CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false, false, false);
			component.set_text("x" + useableStackCount.ToString());
			component.gameObject.CustomSetActive(true);
		}

		private void initRewardIntroFormWidget()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUnionBattleEntrySystem.UNION_ENTRY_REWARDINTRO_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.GetWidget(0).transform;
			CUIListScript component = transform.GetComponent<CUIListScript>();
			int num = (int)(this.m_selectMapRes.bWinCnt + 2);
			component.SetElementAmount(num);
			for (int i = 0; i <= num; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				if (elemenet != null && component.IsElementInScrollArea(i))
				{
					this.refreshOneRewardElement(form, component.GetElemenet(i).gameObject, i);
				}
			}
		}

		private void refreshOneRewardElement(CUIFormScript form, GameObject element, int index)
		{
			if (element == null || form == null)
			{
				return;
			}
			int key = index - 1;
			element.transform.FindChild("PanelReward").gameObject.CustomSetActive(index != 0);
			element.transform.FindChild("PanelTitle").gameObject.CustomSetActive(index == 0);
			if (index == 0)
			{
				return;
			}
			Text component = element.transform.FindChild("PanelReward/Text").GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips16", new string[]
			{
				key.ToString()
			}));
			CUIListScript component2 = element.transform.FindChild("PanelReward/IconContainer").GetComponent<CUIListScript>();
			GameObject gameObject = element.transform.FindChild("PanelReward/itemBaoXiang").gameObject;
			ResRewardMatchReward dataByKey = GameDataMgr.unionBattleWinCntRewardDatabin.GetDataByKey(GameDataMgr.GetDoubleKey(this.m_selectMapID, (uint)key));
			if (dataByKey == null || dataByKey.astRewardItem[0].bRewardType == 0)
			{
				component2.SetElementAmount(0);
				gameObject.CustomSetActive(false);
				return;
			}
			ResRewardInfo resRewardInfo = dataByKey.astRewardItem[0];
			CUseable itemUseable = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resRewardInfo.bRewardType, (int)resRewardInfo.dwRewardNum, resRewardInfo.dwRewardID);
			CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false, false, false);
			gameObject.CustomSetActive(true);
			ResPropInfo dataByKey2 = GameDataMgr.itemDatabin.GetDataByKey(resRewardInfo.dwRewardID);
			if (dataByKey2 == null)
			{
				return;
			}
			ResRandomRewardStore dataByKey3 = GameDataMgr.randomRewardDB.GetDataByKey((long)((int)dataByKey2.EftParam[0]));
			if (dataByKey3 == null)
			{
				return;
			}
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < dataByKey3.astRewardDetail.Length; i++)
			{
				if (dataByKey3.astRewardDetail[i].bItemType == 0)
				{
					break;
				}
				ResDT_RandomRewardInfo resDT_RandomRewardInfo = dataByKey3.astRewardDetail[i];
				listView.Add(CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resDT_RandomRewardInfo.bItemType, (int)resDT_RandomRewardInfo.dwLowCnt, resDT_RandomRewardInfo.dwItemID));
			}
			component2.SetElementAmount(listView.Count);
			int count = listView.Count;
			for (int j = 0; j < count; j++)
			{
				CUIListElementScript elemenet = component2.GetElemenet(j);
				CUICommonSystem.SetItemCell(form, elemenet.gameObject, listView[j], true, false, false, false);
				Text component3 = elemenet.transform.FindChild("lblIconCount").GetComponent<Text>();
				ResDT_RandomRewardInfo resDT_RandomRewardInfo2 = dataByKey3.astRewardDetail[j];
				if (resDT_RandomRewardInfo2.dwLowCnt != resDT_RandomRewardInfo2.dwHighCnt)
				{
					component3.set_text(string.Format("{0}~{1}", resDT_RandomRewardInfo2.dwLowCnt, resDT_RandomRewardInfo2.dwHighCnt));
				}
				if (j + 1 >= count)
				{
					elemenet.gameObject.transform.FindChild("Add").gameObject.CustomSetActive(false);
				}
				else
				{
					elemenet.gameObject.transform.FindChild("Add").gameObject.CustomSetActive(true);
				}
			}
		}

		private void OnRewardElementEnable(CUIEvent uiEvt)
		{
			this.refreshOneRewardElement(uiEvt.m_srcFormScript, uiEvt.m_srcWidget, uiEvt.m_srcWidgetIndexInBelongedList);
		}

		private void OnClickExChgReward(CUIEvent uiEvt)
		{
			CUICommonSystem.JumpForm(RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_NOTICE, 0, 0, null);
		}

		private void OnClickLoseTips(CUIEvent uiEvt)
		{
			CUIAnimatorScript component = uiEvt.m_srcFormScript.GetWidget(9).GetComponent<CUIAnimatorScript>();
			if (!uiEvt.m_srcWidget.activeInHierarchy || component.m_currentAnimatorStateName == "TipsLose_Anim_Out")
			{
				return;
			}
			component.PlayAnimator("TipsLose_Anim_Out");
		}

		private void OnClickGetRewardTips(CUIEvent uiEvt)
		{
			CUIAnimatorScript component = uiEvt.m_srcFormScript.GetWidget(9).GetComponent<CUIAnimatorScript>();
			if (!uiEvt.m_srcWidget.activeInHierarchy || component.m_currentAnimatorStateName == "TipsAward_Anim_Out")
			{
				return;
			}
			component.PlayAnimator("TipsAward_Anim_Out");
		}

		private void OnClickBattleVideo(CUIEvent uiEvt)
		{
			CUICommonSystem.OpenUrl(Singleton<CTextManager>.GetInstance().GetText("HttpUrl_BattleVideo"), true, 0);
		}

		private void OnClickBattleNews(CUIEvent uiEvt)
		{
			CUICommonSystem.OpenUrl(Singleton<CTextManager>.GetInstance().GetText("HttpUrl_BattleNews"), true, 0);
		}

		private void StartARoundRewardMatch()
		{
			Singleton<CUIManager>.GetInstance().OpenForm(CUnionBattleEntrySystem.UNION_ENTRY_THIRD_PATH, false, true);
			this.initThirdFormWidget();
			Singleton<CUIManager>.GetInstance().CloseForm(CUnionBattleEntrySystem.UNION_CONFIRM_ENTRY_PATH);
		}

		private void ShowReward(COMDT_REWARDMATCH_RECORD matchState)
		{
			ResRewardMatchReward dataByKey = GameDataMgr.unionBattleWinCntRewardDatabin.GetDataByKey(GameDataMgr.GetDoubleKey(this.m_selectStateInfo.dwMapId, (uint)this.m_selectStateInfo.bWinCnt));
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < dataByKey.astRewardItem.Length; i++)
			{
				if (dataByKey.astRewardItem[i].bRewardType == 0)
				{
					break;
				}
				if (dataByKey.astRewardItem[i].bRewardType != 20 || (Singleton<HeadIconSys>.GetInstance().GetInfo(dataByKey.astRewardItem[i].dwRewardID) == null && !CSysDynamicBlock.bSocialBlocked))
				{
					ResRewardInfo resRewardInfo = dataByKey.astRewardItem[i];
					listView.Add(CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resRewardInfo.bRewardType, (int)resRewardInfo.dwRewardNum, resRewardInfo.dwRewardID));
				}
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips8");
			Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(listView), text, true, enUIEventID.Union_Battle_GotReward, false, true, "Form_AwardGold");
		}

		private void ShowCountDownTime(GameObject Btn, bool bMapOpened)
		{
			if (Btn == null)
			{
				return;
			}
			Transform transform = Btn.transform;
			uint tagUInt = Btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt;
			Transform transform2 = transform.FindChild("Desc");
			if (transform2 != null)
			{
				GameObject gameObject = transform2.FindChild("Text").gameObject;
				GameObject gameObject2 = transform2.FindChild("Timer").gameObject;
				if (!bMapOpened)
				{
					gameObject2.CustomSetActive(false);
					gameObject.CustomSetActive(false);
				}
				else
				{
					int num = 0;
					uint num2 = 0u;
					CUICommonSystem.GetTimeUtilOpen(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, tagUInt, out num2, out num);
					if (num == 0 && num2 == 0u)
					{
						gameObject2.CustomSetActive(false);
						gameObject.CustomSetActive(true);
						gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips6"));
						return;
					}
					if (num2 < CUnionBattleEntrySystem.SECOND_ONE_DAY)
					{
						gameObject.CustomSetActive(false);
						gameObject2.CustomSetActive(true);
						CUITimerScript component = gameObject2.transform.FindChild("Text").GetComponent<CUITimerScript>();
						component.SetTotalTime(num2);
						component.StartTimer();
					}
					else
					{
						int num3 = num;
						gameObject.CustomSetActive(true);
						gameObject2.CustomSetActive(false);
						string text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips7");
						gameObject.GetComponent<Text>().set_text(string.Format(text, num3));
					}
				}
			}
		}

		private COMDT_REWARDMATCH_RECORD GetMapStateInfo(uint mapId)
		{
			COMDT_REWARDMATCH_RECORD result = null;
			for (int i = 0; i < (int)this.m_stateInfo.bRecordCnt; i++)
			{
				if (mapId == this.m_stateInfo.astRecord[i].dwMapId)
				{
					result = this.m_stateInfo.astRecord[i];
				}
			}
			return result;
		}

		private COMDT_MATCHPOINT GetMapPersonMatchPoint()
		{
			if (this.m_personInfo == null)
			{
				return null;
			}
			uint selectMapID = this.m_selectMapID;
			COMDT_MATCHPOINT result = null;
			COMDT_MATCHPOINT[] astPointList = this.m_personInfo.astPointList;
			int num = 0;
			while ((long)num < (long)((ulong)this.m_personInfo.dwCount))
			{
				if (selectMapID == astPointList[num].dwMapId)
				{
					result = astPointList[num];
					break;
				}
				num++;
			}
			return result;
		}

		public bool IsUnionFuncLocked()
		{
			return !Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_REWARDMATCH);
		}

		public ResCommReward GetResCommonReward(uint awardID)
		{
			return GameDataMgr.commonRewardDatabin.GetDataByKey(awardID);
		}

		public static bool HasMatchInActiveTime()
		{
			bool result = false;
			int unionBattleMapCount = CUnionBattleEntrySystem.GetUnionBattleMapCount();
			for (int i = 0; i < unionBattleMapCount; i++)
			{
				if (CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, CUnionBattleEntrySystem.GetUnionBattleMapInfoByIndex(i).dwMapId).matchState == enMatchOpenState.enMatchOpen_InActiveTime)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static COMDT_REWARDMATCH_RECORD GetRewardMatchStateByMapId(uint mapId)
		{
			COMDT_REWARDMATCH_DATA stateInfo = Singleton<CUnionBattleEntrySystem>.GetInstance().m_stateInfo;
			int num = stateInfo.astRecord.Length;
			COMDT_REWARDMATCH_RECORD result = new COMDT_REWARDMATCH_RECORD();
			for (int i = 0; i < num; i++)
			{
				if (stateInfo.astRecord[i].dwMapId == mapId)
				{
					return stateInfo.astRecord[i];
				}
			}
			return result;
		}

		public static void GetWinloSeCntChgInfo(uint dwMapId, ref CUnionBattleEntrySystem.stLastWinLoseCntChginfo outWinLoseChginfo)
		{
			CUnionBattleEntrySystem.stLastWinLoseCntChginfo[] winLoseChgInfo = Singleton<CUnionBattleEntrySystem>.instance.m_WinLoseChgInfo;
			for (int i = 0; i < winLoseChgInfo.Length; i++)
			{
				if (winLoseChgInfo[i].mapId == dwMapId)
				{
					outWinLoseChginfo = winLoseChgInfo[i];
					break;
				}
			}
		}

		public static void SetWinLoseCntChgInfo(uint dwMapId, CUnionBattleEntrySystem.enLastWinLoseCntChgType type)
		{
			CUnionBattleEntrySystem.stLastWinLoseCntChginfo[] winLoseChgInfo = Singleton<CUnionBattleEntrySystem>.instance.m_WinLoseChgInfo;
			for (int i = 0; i < winLoseChgInfo.Length; i++)
			{
				if (winLoseChgInfo[i].mapId == dwMapId)
				{
					winLoseChgInfo[i].winLoseChgType = type;
					break;
				}
			}
		}

		public static ResRewardMatchLevelInfo GetUnionBattleMapInfoByIndex(int mapIndex)
		{
			return GameDataMgr.uinionBattleLevelDatabin.GetDataByIndex(mapIndex);
		}

		public static ResRewardMatchLevelInfo GetUnionBattleMapInfoByMapID(uint mapID)
		{
			return GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
		}

		public static int GetUnionBattleMapCount()
		{
			return GameDataMgr.uinionBattleLevelDatabin.count;
		}

		private void SendBeginMatchReq()
		{
			CMatchingSystem.ReqStartSingleMatching(this.m_selectMapID, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_REWARDMATCH);
		}

		public static void SendAwartPoolReq(uint mapID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5101u);
			cSPkg.stPkgData.stGetAwardPoolReq.dwMapId = mapID;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void SendBuyTicketRequest(uint mapID, uint itemNum)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5104u);
			cSPkg.stPkgData.stBuyMatchTicketReq.dwMapId = mapID;
			cSPkg.stPkgData.stBuyMatchTicketReq.dwNum = itemNum;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendGetUnionBattleBaseInfoReq()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5106u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendGetUnionBattleStateReq()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5108u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendChgMatchStateReq(uint mapId, byte bState)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5110u);
			cSPkg.stPkgData.stRewardMatchStateChgReq.dwMapId = mapId;
			cSPkg.stPkgData.stRewardMatchStateChgReq.bState = bState;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(5102)]
		public static void ReciveAwardPoolInfo(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			Singleton<CUnionBattleEntrySystem>.instance.m_awardPoolInfo = msg.stPkgData.stGetAwardPoolRsp;
		}

		[MessageHandler(5103)]
		public static void RecivePersonInfo(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			Singleton<CUnionBattleEntrySystem>.instance.m_personInfo = msg.stPkgData.stMatchPointNtf;
		}

		[MessageHandler(5105)]
		public static void ReciveBuyTicketInfo(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			if (msg.stPkgData.stBuyMatchTicketRsp.iResult == 0)
			{
				Singleton<CUnionBattleEntrySystem>.instance.initConfirmFormWidget();
				Singleton<CUnionBattleEntrySystem>.instance.initTirdFormTicket();
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips14", true, 1.5f, null, new object[0]);
			}
			else if (msg.stPkgData.stBuyMatchTicketRsp.iResult == 1)
			{
				Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips13", true, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(5107)]
		public static void ReciveUnionBattleBaseInfo(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			Singleton<CUnionBattleEntrySystem>.instance.m_baseInfo = msg.stPkgData.stGetMatchInfoRsp;
			Singleton<CUnionBattleEntrySystem>.instance.initFirstFormWidget();
		}

		[MessageHandler(5109)]
		public static void ReciveUnionBattleStateRsp(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			Singleton<CUnionBattleEntrySystem>.instance.m_stateInfo = msg.stPkgData.stGetRewardMatchInfoRsp.stRewardMatchInfo;
		}

		[MessageHandler(5111)]
		public static void UnionBattleStateChg(CSPkg msg)
		{
			COMDT_REWARDMATCH_RECORD stChgInfo = msg.stPkgData.stRewardMatchInfoChgNtf.stChgInfo;
			COMDT_REWARDMATCH_DATA stateInfo = Singleton<CUnionBattleEntrySystem>.instance.m_stateInfo;
			for (int i = 0; i < (int)stateInfo.bRecordCnt; i++)
			{
				if (stateInfo.astRecord[i].dwMapId == stChgInfo.dwMapId)
				{
					if (stateInfo.astRecord[i].bState == 0 && stChgInfo.bState == 1)
					{
						Singleton<CUnionBattleEntrySystem>.instance.StartARoundRewardMatch();
					}
					else if (stateInfo.astRecord[i].bState == 2 && stChgInfo.bState == 0)
					{
						Singleton<CUnionBattleEntrySystem>.instance.ShowReward(stChgInfo);
					}
					if (stateInfo.astRecord[i].bWinCnt != stChgInfo.bWinCnt)
					{
						CUnionBattleEntrySystem.SetWinLoseCntChgInfo(stChgInfo.dwMapId, CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_Win);
					}
					else if (stateInfo.astRecord[i].bLossCnt != stChgInfo.bLossCnt)
					{
						CUnionBattleEntrySystem.SetWinLoseCntChgInfo(stChgInfo.dwMapId, CUnionBattleEntrySystem.enLastWinLoseCntChgType.enChgType_Lose);
					}
					stateInfo.astRecord[i] = stChgInfo;
					break;
				}
			}
		}
	}
}

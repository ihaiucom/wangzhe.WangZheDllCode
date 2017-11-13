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
	public class CAchievementSystem : Singleton<CAchievementSystem>
	{
		public enum enTypeMenu
		{
			All,
			Not_Finish,
			Type_Max
		}

		public enum enOverviewFormWidget
		{
			Menu,
			SeryList,
			TrophyLevel,
			TrophyRank,
			NotGotRewardLevelText,
			NotGotRewards,
			GetRewardBtn,
			TrophyProgressImg,
			TrophyProgressTxt,
			NotInRank,
			Icon
		}

		public const ushort TROPHY_RULE_ID = 16;

		private const string OverviewFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab";

		private const string TrophyDetailFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab";

		private const string TrophyRewardsFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab";

		public CAchievementSystem.enTypeMenu m_CurMenu;

		private string[] m_TypeMenuNameKeys = new string[]
		{
			Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Menu_All"),
			Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Menu_Not_Done")
		};

		private ListView<CAchieveItem2> m_CurAchieveSeries = new ListView<CAchieveItem2>();

		private CAchieveItem2 m_CurAchieveItem;

		private int m_curAchievementType;

		private CAchieveShareComponent m_shareComponent;

		private ListView<CTrophyRewardInfo> m_TrophyRewardInfoWithRewardList;

		private static ListView<SCPKG_PVPBAN_NTF> CachedPunishMsg;

		public CAchievementSystem.enTypeMenu CurMenu
		{
			get
			{
				return this.m_CurMenu;
			}
			set
			{
				this.m_CurMenu = value;
			}
		}

		public override void Init()
		{
			base.Init();
			this.m_CurAchieveItem = null;
			this.m_shareComponent = new CAchieveShareComponent();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Open_Overview_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenOverviewForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_ShowShareBtn, new CUIEventManager.OnUIEventHandler(this.OnAchievementShowShareBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Filter_Menu_Change, new CUIEventManager.OnUIEventHandler(this.OnMenuChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnTrophyClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnAchievementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Browse_All_Rewards, new CUIEventManager.OnUIEventHandler(this.OnBrowseAllRewards));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Reward_Info_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyRewardInfoEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Get_Trophy_Reward, new CUIEventManager.OnUIEventHandler(this.OnGetTrophyReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
			Singleton<EventRouter>.instance.AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>(EventID.ACHIEVE_GET_RANKING_ACCOUNT_INFO, new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.HandleAchieveGetRankingAccountInfo));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.LOBBY_STATE_ENTER, new Action(this.HandleLobbyStateEnter));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnTrophyStateChange));
			Singleton<EventRouter>.instance.AddEventHandler<RankingSystem.RankingType>("Ranking_List_Change", new Action<RankingSystem.RankingType>(this.RankingListChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NOBE_LevelUp_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnNobeLevelUpFormClose));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Get_Product_OK, new Action(this.OnMallGetProductOk));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG, new Action(this.ProcessPvpBanMsg));
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_CurAchieveItem = null;
			CAchievementSystem.CachedPunishMsg = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Open_Overview_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenOverviewForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_ShowShareBtn, new CUIEventManager.OnUIEventHandler(this.OnAchievementShowShareBtn));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Filter_Menu_Change, new CUIEventManager.OnUIEventHandler(this.OnMenuChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnTrophyClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnAchievementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Browse_All_Rewards, new CUIEventManager.OnUIEventHandler(this.OnBrowseAllRewards));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Reward_Info_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyRewardInfoEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Get_Trophy_Reward, new CUIEventManager.OnUIEventHandler(this.OnGetTrophyReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
			Singleton<EventRouter>.instance.RemoveEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>(EventID.ACHIEVE_GET_RANKING_ACCOUNT_INFO, new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.HandleAchieveGetRankingAccountInfo));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.LOBBY_STATE_ENTER, new Action(this.HandleLobbyStateEnter));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnTrophyStateChange));
			Singleton<EventRouter>.instance.RemoveEventHandler<RankingSystem.RankingType>("Ranking_List_Change", new Action<RankingSystem.RankingType>(this.RankingListChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NOBE_LevelUp_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnNobeLevelUpFormClose));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Get_Product_OK, new Action(this.OnMallGetProductOk));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG, new Action(this.ProcessPvpBanMsg));
			this.m_shareComponent = null;
		}

		private void OnNobeLevelUpFormClose(CUIEvent uiEvent)
		{
			Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(true);
		}

		private void OnShowRule(CUIEvent uiEvent)
		{
			ushort key = 16;
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint)key);
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		private void OnMenuChange(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex();
			if (selectedIndex < 0 || selectedIndex > 2)
			{
				DebugHelper.Assert(false, "Achievement type form selected menu indx out of range!");
				return;
			}
			this.CurMenu = (CAchievementSystem.enTypeMenu)selectedIndex;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			this.m_CurAchieveSeries = new ListView<CAchieveItem2>();
			CAchievementSystem.enTypeMenu curMenu = this.CurMenu;
			if (curMenu != CAchievementSystem.enTypeMenu.All)
			{
				if (curMenu == CAchievementSystem.enTypeMenu.Not_Finish)
				{
					this.m_CurAchieveSeries = masterAchieveInfo.GetTrophies(enTrophyState.UnFinish);
					this.m_CurAchieveSeries.Sort(new CAchieveSort());
				}
			}
			else
			{
				this.m_CurAchieveSeries = masterAchieveInfo.GetTrophies(enTrophyState.All);
				this.m_CurAchieveSeries.Sort(new CAchieveSort());
			}
			this.RefreshOverviewForm(uiEvent.m_srcFormScript);
		}

		private void OnTrophyEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CurAchieveSeries.Count)
			{
				return;
			}
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript == null)
			{
				DebugHelper.Assert(false, "achievement sery enable elementscript is null");
				return;
			}
			CAchieveItem2 cAchieveItem = this.m_CurAchieveSeries[srcWidgetIndexInBelongedList];
			CUIEventScript component = cUIListElementScript.GetComponent<CUIEventScript>();
			component.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Trophy_Click, new stUIEventParams
			{
				commonUInt32Param1 = cAchieveItem.Cfg.dwID
			});
			GameObject widget = cUIListElementScript.GetWidget(0);
			GameObject widget2 = cUIListElementScript.GetWidget(1);
			GameObject widget3 = cUIListElementScript.GetWidget(2);
			GameObject widget4 = cUIListElementScript.GetWidget(3);
			GameObject widget5 = cUIListElementScript.GetWidget(4);
			GameObject widget6 = cUIListElementScript.GetWidget(5);
			widget2.CustomSetActive(false);
			Image component2 = widget.GetComponent<Image>();
			Image component3 = widget5.GetComponent<Image>();
			Text component4 = widget3.GetComponent<Text>();
			Text component5 = widget4.GetComponent<Text>();
			if (component2 == null || component3 == null || component4 == null || component5 == null)
			{
				return;
			}
			CAchieveItem2 cAchieveItem2 = cAchieveItem.TryToGetMostRecentlyDoneItem();
			if (cAchieveItem2 == null)
			{
				component2.SetSprite(CUIUtility.GetSpritePrefeb(cAchieveItem.GetAchieveImagePath(), false, false), false);
				CAchievementSystem.SetAchieveBaseIcon(widget5.transform, cAchieveItem, null);
				component4.set_text(cAchieveItem.Cfg.szName);
				component5.set_text(cAchieveItem.GetGotTimeText(false, false));
				widget6.CustomSetActive(true);
			}
			else
			{
				component2.SetSprite(CUIUtility.GetSpritePrefeb(cAchieveItem2.GetAchieveImagePath(), false, false), false);
				CAchievementSystem.SetAchieveBaseIcon(widget5.transform, cAchieveItem2, null);
				component4.set_text(cAchieveItem2.Cfg.szName);
				if (cAchieveItem == cAchieveItem2)
				{
					component5.set_text(cAchieveItem.GetGotTimeText(false, false));
				}
				else
				{
					component5.set_text(cAchieveItem2.GetGotTimeText(false, true));
				}
				widget6.CustomSetActive(false);
			}
		}

		private void OnTrophyClick(CUIEvent uiEvent)
		{
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			this.ShowTrophyDetail(masterAchieveInfo, commonUInt32Param);
		}

		public void ShowTrophyDetail(CAchieveInfo2 achieveInfo, uint achievementID)
		{
			CAchieveItem2 cAchieveItem = null;
			if (achieveInfo.m_AchiveItemDic.ContainsKey(achievementID))
			{
				cAchieveItem = achieveInfo.m_AchiveItemDic[achievementID];
			}
			if (cAchieveItem == null)
			{
				return;
			}
			this.m_CurAchieveItem = cAchieveItem.GetHead();
			int achievementCnt = this.m_CurAchieveItem.GetAchievementCnt();
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab", false, true);
			if (cUIFormScript != null)
			{
				Text component = cUIFormScript.GetWidget(1).GetComponent<Text>();
				if (component != null)
				{
					component.set_text(this.m_CurAchieveItem.Cfg.szName);
				}
				Text component2 = cUIFormScript.GetWidget(2).GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text(this.m_CurAchieveItem.GetAchievementTips());
				}
				CUIListScript component3 = cUIFormScript.GetWidget(0).GetComponent<CUIListScript>();
				if (component3 != null)
				{
					component3.SetElementAmount(achievementCnt);
				}
			}
		}

		private void OnAchievementEnable(CUIEvent uiEvent)
		{
			if (this.m_CurAchieveItem == null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab");
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			int achievementCnt = this.m_CurAchieveItem.GetAchievementCnt();
			CAchieveItem2 cAchieveItem = this.m_CurAchieveItem;
			bool bActive = srcWidgetIndexInBelongedList != achievementCnt - 1;
			for (int i = 0; i < srcWidgetIndexInBelongedList; i++)
			{
				cAchieveItem = cAchieveItem.Next;
			}
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			GameObject widget = cUIListElementScript.GetWidget(0);
			GameObject widget2 = cUIListElementScript.GetWidget(1);
			GameObject widget3 = cUIListElementScript.GetWidget(2);
			GameObject widget4 = cUIListElementScript.GetWidget(3);
			GameObject widget5 = cUIListElementScript.GetWidget(4);
			GameObject widget6 = cUIListElementScript.GetWidget(5);
			GameObject widget7 = cUIListElementScript.GetWidget(6);
			GameObject widget8 = cUIListElementScript.GetWidget(7);
			GameObject widget9 = cUIListElementScript.GetWidget(8);
			GameObject widget10 = cUIListElementScript.GetWidget(9);
			widget4.CustomSetActive(bActive);
			Image component = widget.GetComponent<Image>();
			Image component2 = widget9.GetComponent<Image>();
			Text component3 = widget2.GetComponent<Text>();
			Text component4 = widget3.GetComponent<Text>();
			Text component5 = widget10.GetComponent<Text>();
			Text component6 = widget5.GetComponent<Text>();
			Text component7 = widget6.GetComponent<Text>();
			if (component == null || component3 == null || component4 == null || component6 == null || component7 == null || component2 == null || component5 == null)
			{
				return;
			}
			component.SetSprite(CUIUtility.GetSpritePrefeb(cAchieveItem.GetAchieveImagePath(), false, false), false);
			CAchievementSystem.SetAchieveBaseIcon(widget9.transform, cAchieveItem, null);
			component3.set_text(cAchieveItem.Cfg.szName);
			component4.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Achievement_Score"), cAchieveItem.Cfg.dwPoint));
			if (cAchieveItem.IsFinish())
			{
				if (cAchieveItem.DoneTime == 0u)
				{
					widget10.CustomSetActive(false);
				}
				else
				{
					widget10.CustomSetActive(true);
					component5.set_text(string.Format("{0:yyyy.M.d}", Utility.ToUtcTime2Local((long)((ulong)cAchieveItem.DoneTime))));
				}
				component6.set_text(cAchieveItem.GetAchievementDesc());
				component7.set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
				widget7.CustomSetActive(true);
				widget8.CustomSetActive(false);
			}
			else
			{
				widget10.CustomSetActive(false);
				component6.set_text(cAchieveItem.GetAchievementDesc());
				widget6.CustomSetActive(false);
				widget7.CustomSetActive(false);
				widget8.CustomSetActive(true);
			}
		}

		private void OnBrowseAllRewards(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab", false, true);
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			CUIListScript component = cUIFormScript.GetWidget(0).GetComponent<CUIListScript>();
			if (component != null)
			{
				this.m_TrophyRewardInfoWithRewardList = masterAchieveInfo.GetTrophyRewardInfoWithRewards();
				this.m_TrophyRewardInfoWithRewardList.Sort(new CTrophyRewardInfoSort());
				component.SetElementAmount(this.m_TrophyRewardInfoWithRewardList.Count);
			}
		}

		private void OnTrophyRewardInfoEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			int num = (this.m_TrophyRewardInfoWithRewardList == null) ? 0 : this.m_TrophyRewardInfoWithRewardList.Count;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= num)
			{
				return;
			}
			CTrophyRewardInfo cTrophyRewardInfo = this.m_TrophyRewardInfoWithRewardList[srcWidgetIndexInBelongedList];
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript == null)
			{
				DebugHelper.Assert(false, "achievement reward enable elementscript is null");
				return;
			}
			GameObject widget = cUIListElementScript.GetWidget(0);
			GameObject widget2 = cUIListElementScript.GetWidget(1);
			GameObject widget3 = cUIListElementScript.GetWidget(2);
			GameObject widget4 = cUIListElementScript.GetWidget(3);
			GameObject widget5 = cUIListElementScript.GetWidget(4);
			GameObject widget6 = cUIListElementScript.GetWidget(5);
			GameObject widget7 = cUIListElementScript.GetWidget(6);
			GameObject widget8 = cUIListElementScript.GetWidget(7);
			GameObject widget9 = cUIListElementScript.GetWidget(8);
			GameObject widget10 = cUIListElementScript.GetWidget(9);
			GameObject widget11 = cUIListElementScript.GetWidget(10);
			if (cTrophyRewardInfo.HasGotAward())
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				widget5.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget6.CustomSetActive(true);
				Text component = widget6.GetComponent<Text>();
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Got"));
				widget9.CustomSetActive(false);
			}
			else if (cTrophyRewardInfo.IsFinish())
			{
				uint num2 = 0u;
				for (int i = 0; i < 3; i++)
				{
					if (cTrophyRewardInfo.Cfg.astReqReward[i].dwRewardNum != 0u)
					{
						num2 += 1u;
					}
				}
				if (num2 == 0u)
				{
					widget.CustomSetActive(false);
					widget2.CustomSetActive(true);
					widget5.CustomSetActive(false);
					widget8.CustomSetActive(false);
					widget6.CustomSetActive(true);
					Text component2 = widget6.GetComponent<Text>();
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Done"));
				}
				else
				{
					widget.CustomSetActive(true);
					widget2.CustomSetActive(false);
					widget5.CustomSetActive(false);
					widget8.CustomSetActive(true);
					widget6.CustomSetActive(false);
					CUIEventScript component3 = widget8.GetComponent<CUIEventScript>();
					if (component3 != null)
					{
						component3.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Get_Trophy_Reward, new stUIEventParams
						{
							tag = cTrophyRewardInfo.Index
						});
					}
				}
				widget9.CustomSetActive(false);
			}
			else
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				widget5.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget6.CustomSetActive(true);
				Text component4 = widget6.GetComponent<Text>();
				if (component4 != null)
				{
					component4.set_text((cTrophyRewardInfo.State == TrophyState.OnGoing) ? Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_OnGoing") : Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Not_Done"));
				}
				widget9.CustomSetActive(false);
			}
			Image component5 = widget3.GetComponent<Image>();
			component5.SetSprite(cTrophyRewardInfo.GetTrophyImagePath(), cUIListElementScript.m_belongedFormScript, true, false, false, false);
			Text component6 = widget4.GetComponent<Text>();
			if (component6 != null)
			{
				component6.set_text(cTrophyRewardInfo.Cfg.szTrophyDesc);
			}
			CUIListScript component7 = widget7.GetComponent<CUIListScript>();
			if (component7 != null)
			{
				CUseable[] trophyRewards = cTrophyRewardInfo.GetTrophyRewards();
				component7.SetElementAmount(trophyRewards.Length);
				for (int j = 0; j < trophyRewards.Length; j++)
				{
					CUIListElementScript elemenet = component7.GetElemenet(j);
					GameObject widget12 = elemenet.GetWidget(0);
					if (widget12 != null)
					{
						CUseable cUseable = trophyRewards[j];
						if (cUseable == null)
						{
							component7.SetElementAmount(0);
							return;
						}
						if (trophyRewards.Length >= 5)
						{
							CUICommonSystem.SetItemCell(component7.m_belongedFormScript, widget12, cUseable, false, false, false, false);
						}
						else
						{
							CUICommonSystem.SetItemCell(component7.m_belongedFormScript, widget12, cUseable, true, false, false, false);
						}
						if (cUseable.m_stackCount == 1)
						{
							Utility.FindChild(widget12, "cntBg").CustomSetActive(false);
							Utility.FindChild(widget12, "lblIconCount").CustomSetActive(false);
						}
						else
						{
							Utility.FindChild(widget12, "cntBg").CustomSetActive(true);
							Utility.FindChild(widget12, "lblIconCount").CustomSetActive(true);
						}
					}
				}
			}
		}

		private void OnGetTrophyReward(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			CTrophyRewardInfo[] trophyRewardInfoArr = masterAchieveInfo.TrophyRewardInfoArr;
			if (tag < 0 || tag >= trophyRewardInfoArr.Length)
			{
				return;
			}
			CTrophyRewardInfo cTrophyRewardInfo = trophyRewardInfoArr[tag];
			this.SendGetTrophyRewardReq(cTrophyRewardInfo.Cfg.dwTrophyLvl);
		}

		private void OnAchievementOpenOverviewForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			Singleton<CUIManager>.GetInstance().CloseForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab", false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			this.InitTypeMenu(cUIFormScript);
			Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(18u, null, false);
		}

		private void OnAchievementShowShareBtn(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			srcFormScript.GetWidget(2).CustomSetActive(true);
			srcFormScript.GetWidget(1).CustomSetActive(false);
		}

		public void ProcessPvpBanMsg()
		{
			if (Singleton<BattleLogic>.GetInstance().isRuning || Singleton<CMatchingSystem>.GetInstance().IsInMatching || Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam || Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX) != null)
			{
				return;
			}
			if (CAchievementSystem.CachedPunishMsg != null && CAchievementSystem.CachedPunishMsg.Count > 0)
			{
				for (int i = CAchievementSystem.CachedPunishMsg.Count - 1; i >= 0; i--)
				{
					string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Achievement_10"), Utility.GetUtcToLocalTimeStringFormat((ulong)CAchievementSystem.CachedPunishMsg[i].dwPunishEndTime, "yyyy年M月d日 H:m:s"));
					Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
					CAchievementSystem.CachedPunishMsg.RemoveAt(i);
				}
			}
		}

		private void OnMallGetProductOk()
		{
			Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(true);
		}

		private void HandleAchieveGetRankingAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
		{
			if (rsp.stAcntRankingDetail.stOfSucc.bNumberType != 8)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "HandleAchieveGetRankingAccountInfo::Master Role Info Is Null");
				CAchieveInfo2.AddWorldRank(0, 0uL, rsp.stAcntRankingDetail.stOfSucc.dwRankNo);
			}
			else
			{
				CAchieveInfo2.AddWorldRank(masterRoleInfo.logicWorldID, masterRoleInfo.playerUllUID, rsp.stAcntRankingDetail.stOfSucc.dwRankNo);
			}
		}

		private void RankingListChange(RankingSystem.RankingType rankType)
		{
			if (rankType == RankingSystem.RankingType.Achievement)
			{
				CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.GetInstance().GetRankList(RankingSystem.RankingType.Achievement);
				if (rankList != null)
				{
					int num = 0;
					while ((long)num < (long)((ulong)rankList.dwItemNum))
					{
						CAchieveInfo2.AddWorldRank(rankList.astItemDetail[num].stExtraInfo.stDetailInfo.stAchievement.iLogicWorldId, rankList.astItemDetail[num].stExtraInfo.stDetailInfo.stAchievement.ullUid, (uint)(rankList.iStart + num));
						num++;
					}
				}
			}
		}

		private void HandleLobbyStateEnter()
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
		}

		private void OnTrophyStateChange()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab");
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab");
			if (form2 != null)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Achievement_Browse_All_Rewards);
			}
			if (form != null)
			{
				this.RefreshOverviewForm(form);
			}
		}

		public static void AddPunishMsg(SCPKG_PVPBAN_NTF msg)
		{
			if (CAchievementSystem.CachedPunishMsg == null)
			{
				CAchievementSystem.CachedPunishMsg = new ListView<SCPKG_PVPBAN_NTF>();
			}
			CAchievementSystem.CachedPunishMsg.Add(msg);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG);
		}

		public static void SetAchieveBaseIcon(Transform trans, CAchieveItem2 achieveItem, CUIFormScript form = null)
		{
			if (achieveItem == null)
			{
				return;
			}
			if (trans == null)
			{
				return;
			}
			Image component = trans.GetComponent<Image>();
			component.SetSprite(achieveItem.GetAchievementBgIconPath(), form, true, false, false, false);
			Transform transform = trans.Find("goldEffect");
			Transform transform2 = trans.Find("silverEffect");
			if (!achieveItem.IsFinish())
			{
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(false);
				return;
			}
			uint dwAchieveLvl = achieveItem.Cfg.dwAchieveLvl;
			if (dwAchieveLvl >= 3u)
			{
				transform.gameObject.CustomSetActive(true);
				transform2.gameObject.CustomSetActive(false);
			}
			else if (dwAchieveLvl == 2u)
			{
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(true);
			}
			else
			{
				transform.gameObject.CustomSetActive(false);
				transform2.gameObject.CustomSetActive(false);
			}
		}

		public void ProcessMostRecentlyDoneAchievements(bool force = false)
		{
			if (this.m_shareComponent != null)
			{
				this.m_shareComponent.Process(force);
			}
		}

		private void InitTypeMenu(CUIFormScript form)
		{
			int num = 2;
			CUIListScript component = form.GetWidget(0).GetComponent<CUIListScript>();
			component.SetElementAmount(2);
			for (int i = 0; i < num; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				GameObject widget = elemenet.GetWidget(0);
				if (widget != null)
				{
					Text component2 = widget.GetComponent<Text>();
					if (component2 != null)
					{
						component2.set_text(Singleton<CTextManager>.GetInstance().GetText(this.m_TypeMenuNameKeys[i]));
					}
				}
			}
			component.SelectElement(0, true);
		}

		private void RefreshOverviewForm(CUIFormScript overviewForm = null)
		{
			if (overviewForm == null)
			{
				overviewForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab");
			}
			if (overviewForm == null)
			{
				return;
			}
			CUIListScript component = overviewForm.GetWidget(1).GetComponent<CUIListScript>();
			if (component != null)
			{
				component.SetElementAmount(this.m_CurAchieveSeries.Count);
			}
			GameObject widget = overviewForm.GetWidget(2);
			GameObject widget2 = overviewForm.GetWidget(3);
			GameObject widget3 = overviewForm.GetWidget(7);
			GameObject widget4 = overviewForm.GetWidget(8);
			GameObject widget5 = overviewForm.GetWidget(9);
			GameObject widget6 = overviewForm.GetWidget(10);
			GameObject widget7 = overviewForm.GetWidget(4);
			GameObject widget8 = overviewForm.GetWidget(5);
			GameObject widget9 = overviewForm.GetWidget(6);
			if (widget == null || widget2 == null || widget3 == null || widget4 == null || widget7 == null || widget8 == null || widget9 == null || widget5 == null || widget6 == null)
			{
				DebugHelper.Assert(false, "Some of Trophy overview form widgets is null");
				return;
			}
			Text component2 = widget.GetComponent<Text>();
			Text component3 = widget2.GetComponent<Text>();
			Image component4 = widget6.GetComponent<Image>();
			Image component5 = widget3.GetComponent<Image>();
			Text component6 = widget4.GetComponent<Text>();
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			if (masterAchieveInfo.LastDoneTrophyRewardInfo != null)
			{
				component4.SetSprite(masterAchieveInfo.LastDoneTrophyRewardInfo.GetTrophyImagePath(), overviewForm, true, false, false, false);
			}
			component2.set_text((masterAchieveInfo.LastDoneTrophyRewardInfo == null) ? "0" : string.Format("{0}", masterAchieveInfo.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl));
			if (masterAchieveInfo.GetWorldRank() == 0u)
			{
				widget5.CustomSetActive(true);
				widget2.CustomSetActive(false);
			}
			else
			{
				widget5.CustomSetActive(false);
				widget2.CustomSetActive(true);
				component3.set_text(masterAchieveInfo.GetWorldRank().ToString());
			}
			uint num = 0u;
			uint num2 = 0u;
			masterAchieveInfo.GetTrophyProgress(ref num, ref num2);
			CTrophyRewardInfo trophyRewardInfoByPoint = masterAchieveInfo.GetTrophyRewardInfoByPoint(num);
			CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(trophyRewardInfoByPoint.Index + 1);
			component5.set_fillAmount(Utility.Divide(num - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.MaxPoint - trophyRewardInfoByIndex.MinPoint));
			component6.set_text(string.Format("{0}/{1}", num - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.MaxPoint - trophyRewardInfoByIndex.MinPoint));
			Text component7 = widget7.GetComponent<Text>();
			CUIListScript component8 = widget8.GetComponent<CUIListScript>();
			CUIEventScript component9 = widget9.GetComponent<CUIEventScript>();
			CTrophyRewardInfo firstTrophyRewardInfoAwardNotGot = masterAchieveInfo.GetFirstTrophyRewardInfoAwardNotGot();
			if (firstTrophyRewardInfoAwardNotGot == null)
			{
				widget7.CustomSetActive(false);
				widget9.CustomSetActive(false);
				component8.SetElementAmount(0);
			}
			else
			{
				bool flag = false;
				CUseable[] trophyRewards = firstTrophyRewardInfoAwardNotGot.GetTrophyRewards();
				if (!firstTrophyRewardInfoAwardNotGot.HasGotAward() && firstTrophyRewardInfoAwardNotGot.IsFinish())
				{
					flag = true;
				}
				widget7.CustomSetActive(true);
				component7.set_text(string.Format("{0}级奖励：", firstTrophyRewardInfoAwardNotGot.Cfg.dwTrophyLvl));
				component8.SetElementAmount(trophyRewards.Length);
				for (int i = 0; i < trophyRewards.Length; i++)
				{
					CUIListElementScript elemenet = component8.GetElemenet(i);
					CUICommonSystem.SetItemCell(overviewForm, elemenet.GetWidget(0), trophyRewards[i], false, false, false, false);
				}
				widget9.CustomSetActive(true);
				if (flag)
				{
					CUICommonSystem.SetButtonEnableWithShader(widget9.GetComponent<Button>(), true, true);
					component9.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Get_Trophy_Reward, new stUIEventParams
					{
						tag = firstTrophyRewardInfoAwardNotGot.Index
					});
				}
				else
				{
					CUICommonSystem.SetButtonEnableWithShader(widget9.GetComponent<Button>(), false, true);
				}
			}
		}

		public void SendGetTrophyRewardReq(uint id)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4410u);
			cSPkg.stPkgData.stGetTrophyLvlRewardReq.dwTrophyLvl = id;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void SendReqGetRankingAcountInfo()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2604u);
			cSPkg.stPkgData.stGetRankingAcntInfoReq.bNumberType = 8;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void SendGetAchieveRewardReq(uint achieveId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4404u);
			cSPkg.stPkgData.stGetAchievementRewardReq = new CSPKG_GET_ACHIEVEMENT_REWARD_REQ();
			cSPkg.stPkgData.stGetAchievementRewardReq.dwAchievementID = achieveId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(4401)]
		public static void OnRecieveAchieveInfo(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_ACHIEVEMENT_INFO_NTF stGetAchievememtInfoNtf = msg.stPkgData.stGetAchievememtInfoNtf;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			masterAchieveInfo.OnServerAchieveInfo(ref stGetAchievememtInfoNtf.stAchievementInfo);
		}

		[MessageHandler(4411)]
		public static void OnReceiveGetTrophyRewardRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_GET_TROPHYLVL_REWARD_RSP stGetTrophyLvlmentRewardRsp = msg.stPkgData.stGetTrophyLvlmentRewardRsp;
			if (stGetTrophyLvlmentRewardRsp.iResult < 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(string.Format("奖励领取失败，请稍后再试 {0}", stGetTrophyLvlmentRewardRsp.iResult), false, 1.5f, null, new object[0]);
				return;
			}
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			int num = (int)(stGetTrophyLvlmentRewardRsp.dwTrophyLvl - 1u);
			if (num < 0 || (long)num > (long)((ulong)masterAchieveInfo.TrophyRewardInfoArrCnt))
			{
				return;
			}
			CTrophyRewardInfo cTrophyRewardInfo = masterAchieveInfo.TrophyRewardInfoArr[num];
			CUseable[] trophyRewards = cTrophyRewardInfo.GetTrophyRewards();
			Singleton<CUIManager>.instance.OpenAwardTip(trophyRewards, null, false, enUIEventID.None, false, false, "Form_Award");
			cTrophyRewardInfo.State = TrophyState.GotRewards;
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
		}

		[MessageHandler(4412)]
		public static void OnTrophyLevelUp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_NTF_TROPHYLVLUP stNtfTrophyLvlUp = msg.stPkgData.stNtfTrophyLvlUp;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			masterAchieveInfo.TrophyLevelUp(stNtfTrophyLvlUp.dwOldTrophyLvl, stNtfTrophyLvlUp.dwNewTrophyLvl);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
		}

		[MessageHandler(4402)]
		public static void OnNotifyAchieveStateChange(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_ACHIEVEMENT_STATE_CHG_NTF stAchievementStateChgNtf = msg.stPkgData.stAchievementStateChgNtf;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			masterAchieveInfo.ChangeAchieveState(ref stAchievementStateChgNtf.stAchievementData);
			Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(false);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
		}

		[MessageHandler(4403)]
		public static void OnNotifyAchieveDoneDataChange(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF stAchievementDoneDataChgNtf = msg.stPkgData.stAchievementDoneDataChgNtf;
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			masterAchieveInfo.OnAchieveDoneDataChange(stAchievementDoneDataChgNtf.stAchievementDoneData);
		}

		[MessageHandler(5503)]
		public static void OnNotifyPvpBanMsg(CSPkg msg)
		{
			CAchievementSystem.AddPunishMsg(msg.stPkgData.stPvPBanNtf);
		}
	}
}

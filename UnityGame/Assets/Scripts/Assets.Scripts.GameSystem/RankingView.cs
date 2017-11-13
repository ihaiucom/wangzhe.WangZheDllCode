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
	public class RankingView
	{
		public static string s_ChooseHeroPath = "UGUI/Form/System/CustomRecommendEquip/Form_ChooseHero.prefab";

		private static Vector2 s_ListPos1 = new Vector2(10f, 0f);

		private static Vector2 s_ListPos2 = new Vector2(10f, -100f);

		private static Vector2 s_ListSize1 = new Vector2(-20f, 0f);

		private static Vector2 s_ListSize2 = new Vector2(-20f, -100f);

		private static uint RANK_GOD_WIN_CNT = 0u;

		public static void OpenHeroChooseForm()
		{
			enHeroJobType selectIndex = enHeroJobType.All;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(RankingView.s_ChooseHeroPath, false, true);
			GameObject widget = cUIFormScript.GetWidget(2);
			GameObject widget2 = cUIFormScript.GetWidget(0);
			if (widget2 != null)
			{
				CUIListElementScript componetInChild = Utility.GetComponetInChild<CUIListElementScript>(widget, "ScrollRect/Content/ListElement_Template");
				componetInChild.m_onEnableEventID = enUIEventID.Ranking_HeroChg_Hero_Item_Enable;
			}
			if (widget2 != null)
			{
				GameObject widget3 = cUIFormScript.GetWidget(1);
				widget3.CustomSetActive(false);
				string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
				string text3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
				string text5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
				string text6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
				string text7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
				string[] titleList = new string[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6,
					text7
				};
				CUICommonSystem.InitMenuPanel(widget2, titleList, (int)selectIndex, true);
				CUIListScript component = widget2.GetComponent<CUIListScript>();
				component.m_listSelectChangedEventID = enUIEventID.Ranking_HeroChg_Title_Click;
			}
		}

		public static void RefreshGodHeroForm(ListView<ResHeroCfgInfo> heroList)
		{
			if (heroList == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingView.s_ChooseHeroPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			if (widget != null)
			{
				CUIListScript component = widget.GetComponent<CUIListScript>();
				component.SetElementAmount(heroList.Count);
			}
		}

		public static void OnHeroItemEnable(CUIEvent uiEvent, ResHeroCfgInfo heroCfgInfo)
		{
			if (heroCfgInfo != null && uiEvent.m_srcWidget != null)
			{
				GameObject gameObject = Utility.FindChild(uiEvent.m_srcWidget, "heroItemCell");
				if (gameObject != null)
				{
					CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, gameObject, heroCfgInfo.szImagePath, enHeroHeadType.enIcon, false, false);
					CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
					if (component != null)
					{
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Ranking_HeroChg_Hero_Click, new stUIEventParams
						{
							heroId = heroCfgInfo.dwCfgID
						});
					}
					GameObject obj = Utility.FindChild(gameObject, "TxtFree");
					obj.CustomSetActive(false);
					GameObject obj2 = Utility.FindChild(gameObject, "equipedPanel");
					obj2.CustomSetActive(false);
				}
			}
		}

		public static void ShowAllRankMenu()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.m_formWidgets[14];
			gameObject.CustomSetActive(true);
			GameObject obj = Utility.FindChild(gameObject, "ListElement8/Lock");
			obj.CustomSetActive(!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA));
		}

		public static void HideAllRankMenu()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			form.m_formWidgets[14].CustomSetActive(false);
		}

		public static void UpdateOneGodElement(GameObject objElement, int viewIndex, CSDT_RANKING_LIST_SUCC curRankingList)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
			if (curRankingList == null)
			{
				return;
			}
			if (objElement == null)
			{
				return;
			}
			RankingItemHelper component = objElement.GetComponent<RankingItemHelper>();
			if (component == null)
			{
				return;
			}
			CSDT_RANKING_LIST_ITEM_INFO cSDT_RANKING_LIST_ITEM_INFO = curRankingList.astItemDetail[viewIndex];
			if (cSDT_RANKING_LIST_ITEM_INFO == null)
			{
				return;
			}
			string text = string.Empty;
			uint num = 1u;
			string serverUrl = null;
			ulong num2 = 0uL;
			uint logicWorldId = 0u;
			uint level = 0u;
			uint headIdx = 0u;
			uint num3 = 0u;
			ulong privacyBits = 0uL;
			COM_PRIVILEGE_TYPE privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAcntInfo = cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo;
			if (stAcntInfo != null)
			{
				text = StringHelper.UTF8BytesToString(ref stAcntInfo.szPlayerName);
				num = stAcntInfo.dwPvpLevel;
				serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref stAcntInfo.szHeadUrl);
				num2 = stAcntInfo.ullUid;
				logicWorldId = (uint)stAcntInfo.iLogicWorldId;
				level = stAcntInfo.stGameVip.dwCurLevel;
				headIdx = stAcntInfo.stGameVip.dwHeadIconId;
				privilegeType = (COM_PRIVILEGE_TYPE)stAcntInfo.bPrivilege;
				num3 = stAcntInfo.dwVipLevel;
				privacyBits = stAcntInfo.ullUserPrivacyBits;
			}
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			RankingView.SetGameObjChildText(objElement, "NameGroup/PlayerName", text);
			RankingView.SetGameObjChildText(objElement, "PlayerLv", string.Format("Lv.{0}", Math.Max(1u, num)));
			RankingView.SetUrlHeadIcon(component.HeadIcon, serverUrl);
			RankingView.SetPlatChannel(objElement, logicWorldId);
			component.LadderGo.CustomSetActive(false);
			objElement.transform.FindChild("Value").gameObject.CustomSetActive(true);
			objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
			component.FindBtn.CustomSetActive(true);
			component.FindBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = viewIndex;
			Utility.FindChild(component.FindBtn, "Select").CustomSetActive(false);
			float num4 = CPlayerProfile.Divide(cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt * 100u, cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.dwGameCnt);
			if (RankingView.RANK_GOD_WIN_CNT == 0u)
			{
				RankingView.RANK_GOD_WIN_CNT = GameDataMgr.globalInfoDatabin.GetDataByKey(226u).dwConfValue;
			}
			if (cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt >= RankingView.RANK_GOD_WIN_CNT)
			{
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", new string[]
				{
					num4.ToString("F2"),
					cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt.ToString()
				}));
			}
			else
			{
				RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterNameLess100", new string[]
				{
					cSDT_RANKING_LIST_ITEM_INFO.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt.ToString()
				}));
			}
			RankingView.SetGameObjChildText(objElement, "Value", string.Empty);
			uint rankNumber = (uint)(viewIndex + 1);
			RankingView.RankingNumSet(rankNumber, component);
			component.AddFriend.CustomSetActive(false);
			component.SendCoin.CustomSetActive(false);
			component.Online.CustomSetActive(false);
			if (num3 == 913913u)
			{
				MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
			}
			else
			{
				MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int)num3);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int)level, false, masterRoleInfo.playerUllUID == num2, privacyBits);
			}
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int)headIdx);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component.HeadIconFrame.GetComponent<Image>(), (int)headIdx, form, 1f, true);
		}

		public static void RankingNumSet(uint rankNumber, RankingItemHelper rankingHelper)
		{
			rankingHelper.RankingNumText.CustomSetActive(false);
			rankingHelper.No1.CustomSetActive(false);
			rankingHelper.No2.CustomSetActive(false);
			rankingHelper.No3.CustomSetActive(false);
			rankingHelper.No1BG.CustomSetActive(false);
			rankingHelper.No1IconFrame.CustomSetActive(false);
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
					if (rankingHelper.No1BG != null && rankingHelper.No1IconFrame != null)
					{
						rankingHelper.No1BG.CustomSetActive(true);
						rankingHelper.No1IconFrame.CustomSetActive(true);
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

		public static void OnRankingArenaElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			GameObject gameObject = Utility.FindChild(srcWidget, "addFriendBtn");
			GameObject gameObject2 = Utility.FindChild(srcWidget, "sendButton");
			if (Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData.bMemberType == 1)
			{
				COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
				ulong ullUid = fighterInfo.ullUid;
				uint logicWorldID = (uint)MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
				COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, ullUid, logicWorldID);
				COMDT_FRIEND_INFO info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, ullUid, logicWorldID);
				bool flag = info != null;
				bool flag2 = info2 != null;
				if (!flag && !flag2)
				{
					ulong num = (ulong)((uint)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID);
					gameObject.CustomSetActive(num != fighterInfo.ullUid);
					gameObject2.CustomSetActive(false);
					CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(gameObject, "AddFriend");
					componetInChild.m_onClickEventID = enUIEventID.Ranking_ArenaAddFriend;
					componetInChild.m_onClickEventParams.tag = (int)logicWorldID;
					componetInChild.m_onClickEventParams.commonUInt64Param1 = fighterInfo.ullUid;
				}
				else
				{
					COMDT_ACNT_UNIQ cOMDT_ACNT_UNIQ = (info == null) ? info2.stUin : info.stUin;
					gameObject.CustomSetActive(false);
					gameObject2.CustomSetActive(true);
					bool flag3 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(cOMDT_ACNT_UNIQ);
					CUICommonSystem.SetButtonEnableWithShader(gameObject2.GetComponent<Button>(), flag3, true);
					if (flag3)
					{
						CUIEventScript component = gameObject2.GetComponent<CUIEventScript>();
						if (flag)
						{
							component.m_onClickEventID = enUIEventID.Ranking_Friend_GAME_SendCoin;
						}
						else
						{
							component.m_onClickEventID = enUIEventID.Friend_SendCoin;
						}
						component.m_onClickEventParams.tag = srcWidgetIndexInBelongedList;
						component.m_onClickEventParams.commonUInt64Param1 = cOMDT_ACNT_UNIQ.ullUid;
						component.m_onClickEventParams.commonUInt64Param2 = (ulong)cOMDT_ACNT_UNIQ.dwLogicWorldId;
					}
				}
			}
			else
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
			}
			CArenaSystem.Arena_RankElementEnable(uiEvent);
		}

		public static void RefreshRankArena()
		{
			if (Singleton<CArenaSystem>.instance.m_rankInfoList == null)
			{
				return;
			}
			if (Singleton<CArenaSystem>.instance.m_fightHeroInfoList == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(17);
			if (widget == null)
			{
				return;
			}
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SetElementAmount((int)Singleton<CArenaSystem>.instance.m_rankInfoList.bFigterNum);
			component.MoveElementInScrollArea(0, true);
		}

		public static void UpdateArenaSelfInfo()
		{
			if (Singleton<CArenaSystem>.instance.m_fightHeroInfoList == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			uint pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
			List<uint> arenaDefHeroList = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList;
			GameObject widget = form.GetWidget(18);
			RankingItemHelper component = widget.GetComponent<RankingItemHelper>();
			RankingView.SetGameObjChildText(widget, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
			RankingView.SetGameObjChildText(widget, "PlayerLv", string.Format("Lv.{0}", pvpLevel.ToString(CultureInfo.get_InvariantCulture())));
			RankingView.SetHostUrlHeadIcon(Utility.FindChild(widget, "HeadIcon"));
			MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(Utility.GetComponetInChild<Image>(widget, "NameGroup/QQVipIcon"));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(Utility.GetComponetInChild<Image>(widget, "NobeIcon"), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(Utility.GetComponetInChild<Image>(widget, "HeadFrame"), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(Utility.GetComponetInChild<Image>(widget, "HeadFrame"), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId, form, 1f, false);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/WXIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/QQGameCenterIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/GuestGameCenterIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			RankingView.RankingNumSet(Singleton<CArenaSystem>.instance.m_fightHeroInfoList.stArenaInfo.dwSelfRank, component);
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject = Utility.FindChild(widget, string.Format("listHero/heroItemCell{0}", (i + 1).ToString()));
				if (arenaDefHeroList.get_Count() > i)
				{
					gameObject.CustomSetActive(true);
					IHeroData data = CHeroDataFactory.CreateHeroData(arenaDefHeroList.get_Item(i));
					CUICommonSystem.SetHeroItemData(form, gameObject, data, enHeroHeadType.enIcon, false, true);
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
			ListView<COMDT_ARENA_FIGHT_RECORD> recordList = Singleton<CArenaSystem>.instance.m_recordList;
			ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			int num = 0;
			if (recordList != null && recordList.Count > 0)
			{
				if (recordList[0].ullAtkerUid == playerUllUID)
				{
					if (recordList[0].bResult == 1)
					{
						num = (int)(recordList[0].dwAtkerRank - recordList[0].dwTargetRank);
					}
				}
				else if (recordList[0].bResult == 1)
				{
					num = (int)(recordList[0].dwAtkerRank - recordList[0].dwTargetRank);
				}
			}
			GameObject gameObject2 = Utility.FindChild(widget, "ChangeIcon");
			if (num == 0)
			{
				gameObject2.CustomSetActive(false);
				RankingView.SetGameObjChildText(widget, "ChangeNum", "--");
			}
			else if (num > 0)
			{
				gameObject2.CustomSetActive(true);
				gameObject2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				RankingView.SetGameObjChildText(widget, "ChangeNum", num.ToString(CultureInfo.get_InvariantCulture()));
			}
			else if (num < 0)
			{
				gameObject2.CustomSetActive(true);
				gameObject2.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
				RankingView.SetGameObjChildText(widget, "ChangeNum", num.ToString(CultureInfo.get_InvariantCulture()));
			}
		}

		public static void UpdateRankGodTitle(ResHeroCfgInfo heroCfgInfo)
		{
			if (heroCfgInfo == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.m_formWidgets[19];
			if (gameObject == null)
			{
				return;
			}
			GameObject item = Utility.FindChild(gameObject, "heroItemCell");
			CUICommonSystem.SetHeroItemImage(form, item, heroCfgInfo.szImagePath, enHeroHeadType.enIcon, false, false);
			Utility.GetComponetInChild<Text>(gameObject, "PlayerName").set_text(Singleton<CTextManager>.instance.GetText("RankGodHeroName", new string[]
			{
				heroCfgInfo.szHeroTitle,
				heroCfgInfo.szName
			}));
		}

		public static void ResetRankListPos(RankingSystem.RankingType rankingType)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.m_formWidgets[3];
			if (gameObject == null)
			{
				return;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			CUIListScript component2 = gameObject.GetComponent<CUIListScript>();
			if (rankingType != RankingSystem.RankingType.God)
			{
				component.anchoredPosition = RankingView.s_ListPos1;
				component.sizeDelta = RankingView.s_ListSize1;
			}
			else
			{
				component.anchoredPosition = RankingView.s_ListPos2;
				component.sizeDelta = RankingView.s_ListSize2;
			}
			component2.m_scrollAreaSize = new Vector2(component.rect.width, component.rect.height);
		}

		public static void ShowRankGodDetailPanel()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(RankingSystem.s_rankingGodDetailForm, false, true);
			GameObject widget = cUIFormScript.GetWidget(0);
			RankingView.InitRankGodDetailTab();
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "TitleTab");
			componetInChild.SelectElement(0, true);
		}

		public static void UpdateGodFindBtns(CUIListScript uiList, int index)
		{
			if (uiList == null)
			{
				return;
			}
			int elementAmount = uiList.GetElementAmount();
			for (int i = 0; i < elementAmount; i++)
			{
				if (uiList.GetElemenet(i) != null && uiList.GetElemenet(i).gameObject != null && uiList.IsElementInScrollArea(i))
				{
					Utility.FindChild(uiList.GetElemenet(i).gameObject, "FindBtn/Select").CustomSetActive(index == i);
				}
			}
		}

		public static bool IsRankWidgetActive(RankingFormWidget widget)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
			return !(form == null) && form.GetWidget((int)widget).activeSelf;
		}

		public static void OnRankGodDetailTab(int tabIndex, COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO masterHeroInfo, uint heroId)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			if (widget == null)
			{
				return;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "Panel_EquipInfo");
			GameObject gameObject2 = Utility.FindChild(widget, "Panel_SymbolInfo");
			string text = Utility.UTF8Convert(masterHeroInfo.stAcntInfo.szPlayerName);
			string szName = dataByKey.szName;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			if (tabIndex == 0)
			{
				gameObject.CustomSetActive(true);
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(gameObject, "List");
				int num = (int)masterHeroInfo.stEquipList.bEquipNum;
				ushort[] array = new ushort[6];
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						array[i] = (ushort)masterHeroInfo.stEquipList.EquipID[i];
					}
				}
				else
				{
					ResRecommendEquipInBattle defaultRecommendEquipInfo = Singleton<CEquipSystem>.instance.GetDefaultRecommendEquipInfo(heroId, 1u);
					if (defaultRecommendEquipInfo != null)
					{
						array = defaultRecommendEquipInfo.RecommendEquipID;
					}
					num = array.Length;
				}
				componetInChild.SetElementAmount(num);
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject3 = componetInChild.GetElemenet(j).gameObject;
					CUIEventScript component = gameObject3.GetComponent<CUIEventScript>();
					ushort num2 = array[j];
					CEquipInfo equipInfo = CEquipSystem.GetEquipInfo(num2);
					component.m_onClickEventParams.battleEquipPar.equipInfo = CEquipSystem.GetEquipInfo(num2);
					component.m_onClickEventParams.tagStr = text;
					component.m_onClickEventParams.tagStr1 = szName;
					CUICommonSystem.SetEquipIcon(num2, gameObject3, form);
				}
				if (num > 0)
				{
					componetInChild.SelectElement(0, true);
					componetInChild.GetElemenet(0).GetComponent<CUIEventScript>().OnPointerClick(null);
					CUIEventScript component2 = componetInChild.GetElemenet(0).GetComponent<CUIEventScript>();
					Singleton<CUIEventManager>.instance.DispatchUIEvent(component2.m_onClickEventID, component2.m_onClickEventParams);
				}
				else
				{
					componetInChild.SelectElement(-1, true);
				}
			}
			else if (tabIndex == 1)
			{
				ListView<CSymbolItem> listView = new ListView<CSymbolItem>();
				for (int k = 0; k < (int)masterHeroInfo.stSymbolPageInfo.bSymbolPosNum; k++)
				{
					bool flag = false;
					for (int l = 0; l < listView.Count; l++)
					{
						if (listView[l].m_baseID == masterHeroInfo.stSymbolPageInfo.SymbolId[k])
						{
							listView[l].m_stackCount++;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						CSymbolItem item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, masterHeroInfo.stSymbolPageInfo.SymbolId[k], 1) as CSymbolItem;
						listView.Add(item);
					}
				}
				CSymbolWearController.SortSymbolList(ref listView);
				gameObject2.CustomSetActive(true);
				CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(gameObject2, "List");
				componetInChild2.SetElementAmount(listView.Count);
				int num3 = 0;
				for (int m = 0; m < listView.Count; m++)
				{
					GameObject gameObject4 = componetInChild2.GetElemenet(m).gameObject;
					Image componetInChild3 = Utility.GetComponetInChild<Image>(gameObject4, "imgIcon");
					componetInChild3.SetSprite(listView[m].GetIconPath(), form, true, false, false, false);
					Text componetInChild4 = Utility.GetComponetInChild<Text>(gameObject4, "SymbolName");
					componetInChild4.set_text(listView[m].m_name);
					Text componetInChild5 = Utility.GetComponetInChild<Text>(gameObject4, "SymbolDesc");
					componetInChild5.set_text(CSymbolSystem.GetSymbolAttString(listView[m], true).TrimEnd(new char[]
					{
						'\n'
					}));
					Text componetInChild6 = Utility.GetComponetInChild<Text>(gameObject4, "lblIconCount");
					componetInChild6.set_text(string.Format("x{0}", listView[m].m_stackCount));
					num3 += (int)listView[m].m_SymbolData.wLevel * listView[m].m_stackCount;
				}
				Utility.GetComponetInChild<Text>(gameObject2, "symbolPageLvlText").set_text(num3.ToString());
				Utility.GetComponetInChild<Text>(gameObject2, "heroSymbolText").set_text(Singleton<CTextManager>.instance.GetText("RankGodHeroSymbolDesc", new string[]
				{
					text,
					szName
				}));
			}
		}

		public static void OnRankGodDetailEquipClick(CEquipInfo equipInfo, string playerName, string heroName)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			if (widget == null)
			{
				return;
			}
			GameObject p = Utility.FindChild(widget, "Panel_EquipInfo");
			Text componetInChild = Utility.GetComponetInChild<Text>(p, "heroEquipText");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(p, "equipNameText");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(p, "Panel_euipProperty/equipPropertyDescText");
			componetInChild2.set_text((equipInfo == null) ? string.Empty : equipInfo.m_equipName);
			componetInChild3.set_text((equipInfo == null) ? string.Empty : equipInfo.m_equipPropertyDesc);
			componetInChild.set_text(Singleton<CTextManager>.instance.GetText("RankGodHeroEquipDesc", new string[]
			{
				playerName,
				heroName
			}));
		}

		public static void InitRankGodDetailTab()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.GetWidget(0), "TitleTab");
			string[] titleList = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Ranking_God_Tips_3"),
				Singleton<CTextManager>.instance.GetText("Ranking_God_Tips_4")
			};
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(gameObject, "TitleTab");
			CUICommonSystem.InitMenuPanel(gameObject, titleList, 0, true);
		}

		public static void UpdateSymbolItem(CSymbolItem symbol, GameObject element, CUIFormScript form)
		{
			if (symbol == null)
			{
				return;
			}
			if (element == null)
			{
				return;
			}
			if (form == null)
			{
				return;
			}
			Image component = element.transform.Find("iconImage").GetComponent<Image>();
			Text component2 = element.transform.Find("countText").GetComponent<Text>();
			Text component3 = element.transform.Find("nameText").GetComponent<Text>();
			Text component4 = element.transform.Find("descText").GetComponent<Text>();
			component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, symbol.m_SymbolData.dwIcon), form, true, false, false, false);
			component2.set_text(string.Format("x{0}", symbol.m_stackCount.ToString()));
			component3.set_text(symbol.m_SymbolData.szName);
			component4.set_text(CSymbolSystem.GetSymbolAttString(symbol.m_baseID, true));
		}

		public static void SetPlatChannel(GameObject parentObj, uint logicWorldId)
		{
			if (parentObj != null)
			{
				Transform transform = parentObj.transform.Find("NameGroup/PlatChannelIcon");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(!Utility.IsSamePlatformWithSelf(logicWorldId));
					if (CSysDynamicBlock.bLobbyEntryBlocked)
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public static void SetUrlHeadIcon(GameObject headIcon, string serverUrl)
		{
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			CUIHttpImageScript component = headIcon.GetComponent<CUIHttpImageScript>();
			component.SetImageUrl(serverUrl);
		}

		public static void SetHostUrlHeadIcon(GameObject headIcon)
		{
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
			CUIHttpImageScript component = headIcon.GetComponent<CUIHttpImageScript>();
			component.SetImageUrl(headUrl);
		}

		public static void SetGameObjChildText(GameObject parentObj, string childName, string text)
		{
			if (parentObj != null)
			{
				Text component = parentObj.transform.FindChild(childName).gameObject.GetComponent<Text>();
				component.set_text(text);
			}
		}
	}
}

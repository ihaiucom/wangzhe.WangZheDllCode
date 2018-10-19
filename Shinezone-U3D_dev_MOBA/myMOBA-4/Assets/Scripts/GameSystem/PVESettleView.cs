using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class PVESettleView
	{
		public enum AwardWidgets
		{
			None = -1,
			Reserve,
			ItemDetailPanel
		}

		public const string STAR_WIN_ANIM_NAME = "Win_Show";

		public const string REWARD_ANIM_1_NAME = "Box_Show_2";

		public const string REWARD_ANIM_2_NAME = "AppearThePrizes_2";

		private const float expBarWidth = 260f;

		private const float TweenTime = 2f;

		private static LTDescr _expLTD;

		private static LTDescr _coinLTD;

		private static float _expFrom;

		private static float _expTo;

		private static float _coinFrom;

		private static float _coinTo;

		private static Text _coinTweenText;

		private static COMDT_REWARD_MULTIPLE_DETAIL _coinMulti;

		private static RectTransform _expTweenRect;

		private static GameObject _continueBtn1;

		private static GameObject _continueBtn2;

		private static uint _lvUpGrade;

		public static void SetStarFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData, ref StarCondition[] starArr)
		{
			GameObject gameObject = form.transform.Find("Root").gameObject;
			int num = 0;
			for (int i = 1; i < 4; i++)
			{
				GameObject gameObject2 = form.transform.Find(string.Format("Root/Condition{0}", i)).gameObject;
				Text component = gameObject2.transform.Find("Condition_text").gameObject.GetComponent<Text>();
				component.text = starArr[i - 1].ConditionName;
				if (!starArr[i - 1].bCompelete)
				{
					string name = string.Empty;
					if (i == 2)
					{
						name = "Condition_Star1";
					}
					else
					{
						name = "Condition_Star";
					}
					gameObject2.transform.Find(name).gameObject.CustomSetActive(false);
					component.color = CUIUtility.s_Color_Grey;
				}
				else
				{
					num++;
				}
			}
			for (int j = 1; j < 4; j++)
			{
				if (num < j)
				{
					form.transform.Find(string.Format("Root/Panel_Star/Star{0}", j)).gameObject.CustomSetActive(false);
				}
			}
			CUICommonSystem.PlayAnimator(gameObject, "Win_Show");
		}

		public static void SetExpFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("Root/Panel_Exp/Exp_Player").gameObject;
			PVEPlayerItem pVEPlayerItem = new PVEPlayerItem(gameObject);
			pVEPlayerItem.addExp(settleData.stAcntInfo.dwSettleExp);
			CUI3DImageScript component = form.transform.Find("Root/3DImage").gameObject.GetComponent<CUI3DImageScript>();
			DebugHelper.Assert(component != null);
			int num = 1;
			for (int i = 0; i < (int)settleData.stHeroList.bNum; i++)
			{
				uint dwHeroConfID = settleData.stHeroList.astHeroList[i].dwHeroConfID;
				gameObject = form.transform.Find(string.Format("Root/Panel_Exp/Exp_Hero{0}", num)).gameObject;
				CHeroInfo cHeroInfo;
				if (masterRoleInfo.GetHeroInfoDic().TryGetValue(dwHeroConfID, out cHeroInfo))
				{
					ResHeroCfgInfo cfgInfo = cHeroInfo.cfgInfo;
					PVEHeroItem pVEHeroItem = new PVEHeroItem(gameObject, cfgInfo.dwCfgID);
					if (num <= (int)settleData.stHeroList.bNum)
					{
						gameObject.CustomSetActive(true);
						pVEHeroItem.addExp(settleData.stHeroList.astHeroList[num - 1].dwSettleExp);
						int heroWearSkinId = (int)masterRoleInfo.GetHeroWearSkinId(cfgInfo.dwCfgID);
						string objectName = CUICommonSystem.GetHeroPrefabPath(cfgInfo.dwCfgID, heroWearSkinId, true).ObjectName;
						GameObject model = component.AddGameObjectToPath(objectName, false, string.Format("_root/Hero{0}", num));
						CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
						instance.Set3DModel(model);
						instance.InitAnimatList();
						instance.InitAnimatSoundList(cfgInfo.dwCfgID, (uint)heroWearSkinId);
						instance.OnModePlayAnima("idleshow2");
					}
				}
				num++;
			}
		}

		private static void Show3DModel(CUIFormScript belongForm)
		{
			CUI3DImageScript cUI3DImageScript = null;
			Transform transform = belongForm.transform.Find("Root/Panel_Award/3DImage");
			if (transform != null)
			{
				cUI3DImageScript = transform.GetComponent<CUI3DImageScript>();
			}
			if (cUI3DImageScript == null)
			{
				return;
			}
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			PlayerKDA hostKDA = playerKDAStat.GetHostKDA();
			if (hostKDA == null)
			{
				return;
			}
			ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
			uint num = 0u;
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					num = (uint)current.HeroId;
					break;
				}
			}
			int heroWearSkinId = (int)Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(num);
			GameObject gameObject = cUI3DImageScript.AddGameObject(CUICommonSystem.GetHeroPrefabPath(num, heroWearSkinId, true).ObjectName, false, false);
			CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
			instance.Set3DModel(gameObject);
			if (gameObject == null)
			{
				return;
			}
			instance.InitAnimatList();
			instance.InitAnimatSoundList(num, (uint)heroWearSkinId);
		}

		private static void ShowReward(CUIFormScript belongForm, COMDT_SETTLE_RESULT_DETAIL settleData)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return;
			}
			GameObject gameObject = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/Panel_Gold").gameObject;
			Text component = gameObject.transform.Find("GoldNum").gameObject.GetComponent<Text>();
			GameObject gameObject2 = gameObject.transform.Find("GoldMax").gameObject;
			if (settleData.stAcntInfo.bReachDailyLimit > 0)
			{
				gameObject2.CustomSetActive(true);
			}
			else
			{
				gameObject2.CustomSetActive(false);
			}
			component.text = "0";
			COMDT_REWARD_DETAIL stReward = settleData.stReward;
			COMDT_ACNT_INFO stAcntInfo = settleData.stAcntInfo;
			if (stAcntInfo != null)
			{
				GameObject gameObject3 = belongForm.transform.FindChild("Root/Panel_Award/Award/Panel_PlayerExp/PvpExpNode").gameObject;
				Text component2 = gameObject3.transform.FindChild("PvpExpTxt").gameObject.GetComponent<Text>();
				Text component3 = gameObject3.transform.FindChild("AddPvpExpTxt").gameObject.GetComponent<Text>();
				RectTransform component4 = gameObject3.transform.FindChild("PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
				RectTransform component5 = gameObject3.transform.FindChild("PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
				Text component6 = gameObject3.transform.FindChild("PlayerName").gameObject.GetComponent<Text>();
				CUIHttpImageScript component7 = gameObject3.transform.FindChild("HeadImage").gameObject.GetComponent<CUIHttpImageScript>();
				Text component8 = gameObject3.transform.FindChild("PvpLevelTxt").gameObject.GetComponent<Text>();
				Image component9 = gameObject3.transform.FindChild("NobeIcon").gameObject.GetComponent<Image>();
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component9, (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
				Image component10 = gameObject3.transform.FindChild("HeadFrame").gameObject.GetComponent<Image>();
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component10, (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
				component8.text = string.Format("Lv.{0}", stAcntInfo.dwPvpLv.ToString());
				ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)stAcntInfo.dwPvpLv));
				GameObject gameObject4 = gameObject3.transform.FindChild("ExpMax").gameObject;
				if (stAcntInfo.bExpDailyLimit == 0)
				{
					gameObject4.CustomSetActive(false);
				}
				component2.text = string.Format("{0}/{1}", stAcntInfo.dwPvpExp, dataByKey.dwNeedExp);
				component3.text = string.Format("+{0}", stAcntInfo.dwPvpSettleExp);
				CUICommonSystem.AppendMultipleText(component3, CUseable.GetMultiple(stAcntInfo.dwPvpSettleBaseExp, ref settleData.stMultipleDetail, 15, -1));
				component6.text = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name;
				string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
				if (!CSysDynamicBlock.bLobbyEntryBlocked)
				{
					component7.SetImageUrl(headUrl);
				}
				if (stAcntInfo.dwPvpSettleExp > 0u)
				{
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
				}
				float num = 0f;
				if (stAcntInfo.dwPvpExp < stAcntInfo.dwPvpSettleExp)
				{
					component4.sizeDelta = new Vector2(num * 260f, component4.sizeDelta.y);
					PVESettleView._lvUpGrade = stAcntInfo.dwPvpLv;
				}
				else
				{
					num = (stAcntInfo.dwPvpExp - stAcntInfo.dwPvpSettleExp) / dataByKey.dwNeedExp;
					component4.sizeDelta = new Vector2(num * 260f, component4.sizeDelta.y);
					PVESettleView._lvUpGrade = 0u;
				}
				float expTo = stAcntInfo.dwPvpExp / dataByKey.dwNeedExp;
				PVESettleView._expFrom = num;
				PVESettleView._expTo = expTo;
				component5.sizeDelta = new Vector2(num * 260f, component5.sizeDelta.y);
				PVESettleView._expTweenRect = component5;
				PVESettleView._coinFrom = 0f;
				PVESettleView._coinTo = 0f;
				for (int i = 0; i < (int)stReward.bNum; i++)
				{
					COMDT_REWARD_INFO cOMDT_REWARD_INFO = stReward.astRewardDetail[i];
					byte bType = cOMDT_REWARD_INFO.bType;
					if (bType == 11)
					{
						PVESettleView._coinTo = cOMDT_REWARD_INFO.stRewardInfo.dwPvpCoin;
						PVESettleView._coinMulti = settleData.stMultipleDetail;
					}
				}
				PVESettleView._coinTweenText = component;
				PVESettleView.DoCoinAndExpTween();
			}
			ListView<COMDT_REWARD_INFO> listView = new ListView<COMDT_REWARD_INFO>();
			GameObject gameObject5 = belongForm.transform.Find("Root/Panel_Award/Award/Panel_QQVIPGold").gameObject;
			if (gameObject5 != null)
			{
				gameObject5.CustomSetActive(false);
			}
			GameObject gameObject6 = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/FirstGain").gameObject;
			if (gameObject6 != null)
			{
				gameObject6.CustomSetActive(false);
			}
			for (int j = 0; j < (int)stReward.bNum; j++)
			{
				COMDT_REWARD_INFO cOMDT_REWARD_INFO = stReward.astRewardDetail[j];
				byte bType = cOMDT_REWARD_INFO.bType;
				if (bType != 6)
				{
					if (bType == 11)
					{
						CUICommonSystem.AppendMultipleText(component, CUseable.GetMultiple(stAcntInfo.dwPvpSettleBaseCoin, ref settleData.stMultipleDetail, 0, -1));
						if (gameObject5 != null)
						{
							gameObject5.CustomSetActive(false);
							Text component11 = gameObject5.transform.FindChild("Text_Value").gameObject.GetComponent<Text>();
							GameObject gameObject7 = gameObject5.transform.FindChild("Icon_QQVIP").gameObject;
							GameObject gameObject8 = gameObject5.transform.FindChild("Icon_QQSVIP").gameObject;
							gameObject7.CustomSetActive(false);
							gameObject8.CustomSetActive(false);
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							uint qqVipExtraCoin = CUseable.GetQqVipExtraCoin(cOMDT_REWARD_INFO.stRewardInfo.dwPvpCoin, ref settleData.stMultipleDetail, 0);
							if (masterRoleInfo != null && qqVipExtraCoin > 0u)
							{
								component11.text = string.Format("+{0}", qqVipExtraCoin);
								if (masterRoleInfo.HasVip(16))
								{
									gameObject5.CustomSetActive(true);
									gameObject8.CustomSetActive(true);
								}
								else if (masterRoleInfo.HasVip(1))
								{
									gameObject5.CustomSetActive(true);
									gameObject7.CustomSetActive(true);
								}
							}
							gameObject5.CustomSetActive(false);
						}
					}
				}
				else
				{
					listView.Add(stReward.astRewardDetail[j]);
					if (gameObject6 != null)
					{
						gameObject6.CustomSetActive(false);
					}
				}
			}
			GameObject gameObject9 = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/itemCell").gameObject;
			gameObject9.CustomSetActive(false);
			if (listView.Count > 0)
			{
				Text component12 = gameObject9.transform.FindChild("ItemName").gameObject.GetComponent<Text>();
				gameObject9.CustomSetActive(true);
				COMDT_REWARD_INFO cOMDT_REWARD_INFO = listView[0];
				PVESettleView.SetItemEtcCell(belongForm, gameObject9, component12, cOMDT_REWARD_INFO, settleData);
			}
		}

		public static void SetRewardFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
		{
			Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSceneBgPath, form);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("Root/Panel_Interactable/Button_Next").gameObject;
			if (curLvelContext.IsGameTypeActivity())
			{
				gameObject.CustomSetActive(false);
			}
			else
			{
				int nextLevelId = CAdventureSys.GetNextLevelId(curLvelContext.m_chapterNo, (int)curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
				if (nextLevelId != 0)
				{
					if (Singleton<CAdventureSys>.GetInstance().IsLevelOpen(nextLevelId))
					{
						gameObject.CustomSetActive(true);
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
			gameObject.CustomSetActive(false);
			PVESettleView.Show3DModel(form);
			GameObject gameObject2 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty1").gameObject;
			GameObject gameObject3 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty2").gameObject;
			GameObject gameObject4 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty3").gameObject;
			Text component = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaName").gameObject.GetComponent<Text>();
			if (curLvelContext.m_levelDifficulty == 1)
			{
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
			}
			else if (curLvelContext.m_levelDifficulty == 2)
			{
				gameObject2.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
			}
			else if (curLvelContext.m_levelDifficulty == 3)
			{
				gameObject3.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
			}
			component.text = string.Format(curLvelContext.m_levelName, new object[0]);
			PVESettleView._continueBtn1 = form.transform.Find("Root/Panel_Interactable/Button_Once").gameObject;
			PVESettleView._continueBtn2 = form.transform.Find("Root/Panel_Interactable/Button_ReturnLobby").gameObject;
			PVESettleView._continueBtn1.CustomSetActive(true);
			PVESettleView._continueBtn2.CustomSetActive(true);
			PVESettleView.ShowReward(form, settleData);
			CUICommonSystem.PlayAnimator(form.gameObject, "Box_Show_2");
		}

		private static void DoCoinAndExpTween()
		{
			if (PVESettleView._expTweenRect != null && PVESettleView._expTweenRect.gameObject != null)
			{
				PVESettleView._expLTD = LeanTween.value(PVESettleView._expTweenRect.gameObject, delegate(float value)
				{
					if (PVESettleView._expTweenRect != null && PVESettleView._expTweenRect.gameObject != null)
					{
						PVESettleView._expTweenRect.sizeDelta = new Vector2(value * 260f, PVESettleView._expTweenRect.sizeDelta.y);
						if (value >= PVESettleView._expTo)
						{
							PVESettleView.DoExpTweenEnd();
						}
					}
				}, PVESettleView._expFrom, PVESettleView._expTo, 2f);
			}
			if (PVESettleView._coinTweenText != null && PVESettleView._coinTweenText.gameObject != null)
			{
				PVESettleView._coinLTD = LeanTween.value(PVESettleView._coinTweenText.gameObject, delegate(float value)
				{
					if (PVESettleView._coinTweenText != null && PVESettleView._coinTweenText.gameObject != null)
					{
						PVESettleView._coinTweenText.text = string.Format("+{0}", value.ToString("N0"));
						if (value >= PVESettleView._coinTo)
						{
							PVESettleView.DoCoinTweenEnd();
						}
					}
				}, PVESettleView._coinFrom, PVESettleView._coinTo, 2f);
			}
		}

		public static void DoCoinTweenEnd()
		{
			if (PVESettleView._coinLTD != null && PVESettleView._coinTweenText != null)
			{
				PVESettleView._coinTweenText.text = string.Format("+{0}", PVESettleView._coinTo.ToString("N0"));
				if (PVESettleView._coinMulti != null && Singleton<BattleStatistic>.GetInstance().acntInfo != null)
				{
					COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
					CUICommonSystem.AppendMultipleText(PVESettleView._coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref PVESettleView._coinMulti, 0, -1));
				}
				PVESettleView._coinLTD.cancel();
				PVESettleView._coinLTD = null;
				PVESettleView._coinTweenText = null;
				PVESettleView._coinMulti = null;
			}
		}

		private static void DoExpTweenEnd()
		{
			if (PVESettleView._expTweenRect != null && PVESettleView._expLTD != null)
			{
				PVESettleView._expTweenRect.sizeDelta = new Vector2(PVESettleView._expTo * 260f, PVESettleView._expTweenRect.sizeDelta.y);
				PVESettleView._expLTD.cancel();
				PVESettleView._expLTD = null;
				PVESettleView._expTweenRect = null;
			}
			if (PVESettleView._continueBtn1 != null)
			{
				PVESettleView._continueBtn1.CustomSetActive(true);
				PVESettleView._continueBtn1 = null;
			}
			if (PVESettleView._continueBtn2 != null)
			{
				PVESettleView._continueBtn2.CustomSetActive(true);
				PVESettleView._continueBtn2 = null;
			}
			if (PVESettleView._lvUpGrade > 1u)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)(PVESettleView._lvUpGrade - 1u);
				cUIEvent.m_eventParams.tag2 = (int)PVESettleView._lvUpGrade;
				CUIEvent uiEvent = cUIEvent;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
			PVESettleView._lvUpGrade = 0u;
		}

		public static void SetItemEtcCell(CUIFormScript form, GameObject item, Text name, COMDT_REWARD_INFO rewardInfo, COMDT_SETTLE_RESULT_DETAIL settleData)
		{
			byte bType = rewardInfo.bType;
			switch (bType)
			{
			case 1:
			{
				CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, 0uL, rewardInfo.stRewardInfo.stItem.dwItemID, (int)rewardInfo.stRewardInfo.stItem.dwCnt, 0);
				cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
				CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
				ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(rewardInfo.stRewardInfo.stItem.dwItemID);
				if (dataByKey != null)
				{
					name.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
				}
				break;
			}
			case 2:
				break;
			case 3:
			{
				CUseable cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int)rewardInfo.stRewardInfo.dwCoupons);
				cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
				CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
				name.text = cUseable.m_name;
				break;
			}
			case 4:
			{
				CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0uL, rewardInfo.stRewardInfo.stEquip.dwEquipID, (int)rewardInfo.stRewardInfo.stEquip.dwCnt, 0);
				cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
				CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
				ResEquipInfo dataByKey2 = GameDataMgr.equipInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stEquip.dwEquipID);
				if (dataByKey2 != null)
				{
					name.text = StringHelper.UTF8BytesToString(ref dataByKey2.szName);
				}
				break;
			}
			case 5:
			{
				CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0uL, rewardInfo.stRewardInfo.stHero.dwHeroID, (int)rewardInfo.stRewardInfo.stHero.dwCnt, 0);
				cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
				CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
				ResHeroCfgInfo dataByKey3 = GameDataMgr.heroDatabin.GetDataByKey(rewardInfo.stRewardInfo.stHero.dwHeroID);
				if (dataByKey3 != null)
				{
					name.text = StringHelper.UTF8BytesToString(ref dataByKey3.szName);
				}
				break;
			}
			case 6:
			{
				CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0uL, rewardInfo.stRewardInfo.stSymbol.dwSymbolID, (int)rewardInfo.stRewardInfo.stSymbol.dwCnt, 0);
				cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
				CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
				ResSymbolInfo dataByKey4 = GameDataMgr.symbolInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stSymbol.dwSymbolID);
				if (dataByKey4 != null)
				{
					name.text = StringHelper.UTF8BytesToString(ref dataByKey4.szName);
				}
				break;
			}
			default:
				if (bType == 16)
				{
					CUseable cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int)rewardInfo.stRewardInfo.dwDiamond);
					cUseable.SetMultiple(ref settleData.stMultipleDetail, true);
					CUICommonSystem.SetItemCell(form, item, cUseable, true, false, false, false);
					name.text = cUseable.m_name;
				}
				break;
			}
		}

		public static void ShowPlayerLevelUp(CUIFormScript form, int oldLvl, int newLvl)
		{
			if (form != null)
			{
				GameObject gameObject = form.transform.Find("PlayerLvlUp").gameObject;
				Text component = gameObject.transform.Find("bg/TxtPlayerLvl").gameObject.GetComponent<Text>();
				component.text = newLvl.ToString();
				Text component2 = gameObject.transform.Find("bg/TxtPlayerBeforeLvl").gameObject.GetComponent<Text>();
				component2.text = oldLvl.ToString();
				ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey((uint)oldLvl);
				DebugHelper.Assert(dataByKey != null, "Can't find acnt exp config -- level {0}", new object[]
				{
					oldLvl
				});
				ResAcntExpInfo dataByKey2 = GameDataMgr.acntExpDatabin.GetDataByKey((uint)newLvl);
				DebugHelper.Assert(dataByKey2 != null, "Can't find acnt exp config -- level {0}", new object[]
				{
					newLvl
				});
				Transform transform = gameObject.transform.Find("Panel/groupPanel/symbolPosPanel");
				int symbolPosOpenCnt = CSymbolInfo.GetSymbolPosOpenCnt(oldLvl);
				int symbolPosOpenCnt2 = CSymbolInfo.GetSymbolPosOpenCnt(newLvl);
				bool hasBuy = false;
				CRoleInfo master = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (master != null && symbolPosOpenCnt < symbolPosOpenCnt2)
				{
					GameDataMgr.symbolPosDatabin.Accept(delegate(ResSymbolPos rule)
					{
						if (rule != null && (int)rule.wOpenLevel == newLvl)
						{
							hasBuy = master.m_symbolInfo.IsGridPosHasBuy((int)rule.bSymbolPos);
						}
					});
				}
				transform.gameObject.CustomSetActive(!hasBuy && symbolPosOpenCnt2 > symbolPosOpenCnt);
				if (!hasBuy && symbolPosOpenCnt2 > symbolPosOpenCnt)
				{
					Text component3 = transform.Find("curCntText").gameObject.GetComponent<Text>();
					component3.text = symbolPosOpenCnt.ToString();
					Text component4 = transform.Find("levelUpCntText").gameObject.GetComponent<Text>();
					component4.text = symbolPosOpenCnt2.ToString();
				}
				Transform transform2 = gameObject.transform.Find("Panel/groupPanel/symbolLevelPanel");
				int symbolLvlLimit = CSymbolInfo.GetSymbolLvlLimit(oldLvl);
				int symbolLvlLimit2 = CSymbolInfo.GetSymbolLvlLimit(newLvl);
				transform2.gameObject.CustomSetActive(symbolLvlLimit2 > symbolLvlLimit);
				if (symbolLvlLimit2 > symbolLvlLimit)
				{
					Text component5 = transform2.Find("curCntText").gameObject.GetComponent<Text>();
					Text component6 = transform2.Find("levelUpCntText").gameObject.GetComponent<Text>();
					component5.text = symbolLvlLimit.ToString();
					component6.text = symbolLvlLimit2.ToString();
				}
				Transform transform3 = gameObject.transform.Find("Panel/groupPanel/symbolPageCntPanel");
				ResHeroSymbolLvl dataByKey3 = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((long)newLvl);
				if (dataByKey3 != null)
				{
					transform3.gameObject.CustomSetActive(dataByKey3.bPresentSymbolPage > 0);
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo == null)
					{
						return;
					}
					if (dataByKey3.bPresentSymbolPage > 0 && masterRoleInfo != null)
					{
						Text component7 = transform3.Find("curCntText").gameObject.GetComponent<Text>();
						Text component8 = transform3.Find("levelUpCntText").gameObject.GetComponent<Text>();
						component7.text = (masterRoleInfo.m_symbolInfo.m_pageCount - 1).ToString();
						component8.text = masterRoleInfo.m_symbolInfo.m_pageCount.ToString();
					}
				}
			}
		}

		public static void OnStarWinAnimEnd(CUIFormScript starForm, ref StarCondition[] starArr)
		{
			PVESettleView.GoToStarAnimState(starForm, ref starArr, string.Empty);
		}

		public static void StopStarAnim(CUIFormScript starForm)
		{
			if (starForm.transform.Find("EscapeAnim").gameObject.activeSelf)
			{
				starForm.transform.Find("EscapeAnim").gameObject.CustomSetActive(false);
				starForm.transform.Find("Panel_Interactable").gameObject.CustomSetActive(true);
				StarCondition[] condition = Singleton<PVESettleSys>.GetInstance().GetCondition();
				PVESettleView.GoToStarAnimState(starForm, ref condition, "_Done");
			}
		}

		public static void StopExpAnim(CUIFormScript expForm)
		{
			if (expForm.transform.Find("Root/EscapeAnim").gameObject.activeSelf)
			{
				expForm.transform.Find("Root/EscapeAnim").gameObject.CustomSetActive(false);
				expForm.transform.Find("Root/Panel_Interactable").gameObject.CustomSetActive(true);
			}
		}

		public static void StopRewardAnim(CUIFormScript rewardForm)
		{
			if (rewardForm != null && rewardForm.transform.Find("Root/EscapeAnim").gameObject.activeSelf)
			{
				rewardForm.transform.Find("Root/EscapeAnim").gameObject.CustomSetActive(false);
				rewardForm.transform.Find("Root/Panel_Interactable").gameObject.CustomSetActive(true);
				Animator component = rewardForm.gameObject.GetComponent<Animator>();
				component.Play("AppearThePrizes_2_Done");
				Singleton<PVESettleSys>.instance.OnAwardDisplayEnd();
			}
		}

		private static void GoToStarAnimState(CUIFormScript starForm, ref StarCondition[] starArr, string animSuffix = "")
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < 3; i++)
			{
				if (starArr[i].bCompelete)
				{
					if (i == 1)
					{
						flag2 = true;
					}
					if (i == 2)
					{
						flag3 = true;
					}
				}
				if (!starArr[i].bCompelete && i == 0)
				{
					flag = true;
				}
			}
			GameObject gameObject = starForm.transform.Find("Root").gameObject;
			if (flag)
			{
				CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_3{0}", animSuffix));
			}
			else if (flag2)
			{
				if (flag3)
				{
					CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_3{0}", animSuffix));
				}
				else
				{
					CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_2{0}", animSuffix));
				}
			}
			else if (flag3)
			{
				CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_1_3{0}", animSuffix));
			}
			else
			{
				CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_1{0}", animSuffix));
			}
		}
	}
}

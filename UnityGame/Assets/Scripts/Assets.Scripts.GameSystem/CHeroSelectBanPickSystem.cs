using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CHeroSelectBanPickSystem : Singleton<CHeroSelectBanPickSystem>
	{
		private const float c_countDownCheckTime = 6.1f;

		public const int c_banHeroCountMax = 3;

		public static string s_heroSelectFormPath = "UGUI/Form/System/HeroSelect/Form_HeroSelectBanPick.prefab";

		public static string s_symbolPropPanelPath = "Bottom/Panel_SymbolProp";

		private ListView<IHeroData> m_banHeroList;

		private IHeroData m_selectBanHeroData;

		private enHeroJobType m_heroSelectJobType;

		private ListView<IHeroData> m_canUseHeroListByJob = new ListView<IHeroData>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_FormClose, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_HeroJobMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroJobMenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SkinSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnSkinSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SelectHero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SelectHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_CenterHeroItemEnable, new CUIEventManager.OnUIEventHandler(this.CenterHeroItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_ConfirmHeroSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_ConfirmHeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroReq, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroReq));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroAllow, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroAllow));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroCanel, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroCanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_PageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageItemSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSymbolPageSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillOpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenAddedSkillPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillSelected, new CUIEventManager.OnUIEventHandler(this.OnSelectedAddedSkill));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmAddedSkill));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseAddedSkillPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_OnTimerCountDown, new CUIEventManager.OnUIEventHandler(this.OnTimerCountDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_ChangeRcmdEquipPlan, new CUIEventManager.OnUIEventHandler(this.OnChangeRcmdEquipPlan));
		}

		public void CloseForm()
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
			Singleton<CUIManager>.GetInstance().CloseForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
		}

		private void HeroSelect_OnClose(CUIEvent uiEvent)
		{
			this.m_banHeroList = null;
			this.m_selectBanHeroData = null;
			this.m_heroSelectJobType = enHeroJobType.All;
			this.m_canUseHeroListByJob.Clear();
			Singleton<CHeroSelectBaseSystem>.instance.Clear();
			Singleton<CSoundManager>.GetInstance().UnLoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
			Singleton<CSoundManager>.GetInstance().UnLoadBank("Newguide_Voice_BanPick", CSoundManager.BankType.Lobby);
		}

		public void OpenForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSelectBanPickSystem.s_heroSelectFormPath, false, true);
			if (cUIFormScript == null || Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
			{
				return;
			}
			this.m_banHeroList = CHeroDataFactory.GetBanHeroList();
			this.InitSystem(cUIFormScript);
			this.RefreshAll();
			Singleton<CSoundManager>.GetInstance().LoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
			Singleton<CSoundManager>.GetInstance().LoadBank("Newguide_Voice_BanPick", CSoundManager.BankType.Lobby);
			Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(cUIFormScript.transform.FindChild("Bottom/Panel_RcmdEquipPlan").gameObject, enNewFlagKey.New_SelectHero_CustomEquip_V14, enNewFlagPos.enTopRight, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
		}

		public void InitSystem(CUIFormScript form)
		{
			CUICommonSystem.SetObjActive(form.transform.Find("Top/Timer/CountDownMovie"), false);
			this.InitAddedSkillPanel();
			this.InitMenu(false);
			Singleton<CReplayKitSys>.GetInstance().InitReplayKit(form.transform.Find("ReplayKit"), true, true);
		}

		public void InitMenu(bool isResetListSelect = false)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find("PanelCenter/TabListHero").gameObject;
			GameObject gameObject2 = form.gameObject.transform.Find("PanelCenter/TabListSkin").gameObject;
			string[] strTitleList = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Choose_Skin")
			};
			string[] strTitleList2 = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer"),
				Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid")
			};
			Transform targetTrans = form.transform.Find("PanelCenter/ListHostHeroInfo");
			Transform targetTrans2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				this.InitSubMenu(gameObject, strTitleList2, true);
				this.InitSubMenu(gameObject2, strTitleList, false);
				CUICommonSystem.SetObjActive(targetTrans, true);
				CUICommonSystem.SetObjActive(targetTrans2, false);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				if (Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
				{
					this.InitSubMenu(gameObject, strTitleList2, false);
					this.InitSubMenu(gameObject2, strTitleList, true);
					CUICommonSystem.SetObjActive(targetTrans, false);
					CUICommonSystem.SetObjActive(targetTrans2, true);
				}
				else
				{
					this.InitSubMenu(gameObject, strTitleList2, true);
					this.InitSubMenu(gameObject2, strTitleList, false);
					CUICommonSystem.SetObjActive(targetTrans, true);
					CUICommonSystem.SetObjActive(targetTrans2, false);
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap || Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
			{
				this.InitSubMenu(gameObject, strTitleList2, false);
				this.InitSubMenu(gameObject2, strTitleList, true);
				CUICommonSystem.SetObjActive(targetTrans, false);
				CUICommonSystem.SetObjActive(targetTrans2, true);
			}
			this.ResetHeroSelectJobType();
			if (isResetListSelect)
			{
				this.InitHeroList(form, true);
			}
		}

		private void InitSubMenu(GameObject menuObj, string[] strTitleList, bool isShow)
		{
			if (isShow)
			{
				CUICommonSystem.InitMenuPanel(menuObj, strTitleList, 0, false);
				CUICommonSystem.SetObjActive(menuObj, true);
			}
			else
			{
				CUICommonSystem.SetObjActive(menuObj, false);
			}
		}

		public void RefreshAll()
		{
			this.RefreshTop();
			this.RefreshBottom();
			this.RefreshLeft();
			this.RefreshRight();
			this.RefreshCenter();
			this.RefreshSwapPanel();
		}

		public void RefreshTop()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("Top/Timer");
			Transform transform2 = form.transform.Find("Top/Tips");
			Text component = form.transform.Find("Top/Tips/lblTitle").GetComponent<Text>();
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				component.gameObject.CustomSetActive(true);
				transform2.gameObject.CustomSetActive(true);
				transform.gameObject.CustomSetActive(false);
				component.set_text(Singleton<CTextManager>.instance.GetText("BP_Title_1"));
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				component.gameObject.CustomSetActive(true);
				transform2.gameObject.CustomSetActive(true);
				transform.gameObject.CustomSetActive(false);
				component.set_text(Singleton<CTextManager>.instance.GetText("BP_Title_2"));
			}
			else
			{
				transform2.gameObject.CustomSetActive(false);
				transform.gameObject.CustomSetActive(true);
			}
			CUIListScript component2 = form.transform.Find("Top/LeftListBan").GetComponent<CUIListScript>();
			CUIListScript component3 = form.transform.Find("Top/RightListBan").GetComponent<CUIListScript>();
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			COM_PLAYERCAMP camp;
			COM_PLAYERCAMP camp2;
			if (masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				camp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			else
			{
				camp = masterMemberInfo.camp;
				camp2 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp);
			}
			this.InitBanHeroList(component2, camp);
			this.InitBanHeroList(component3, camp2);
		}

		public void RefreshBottom()
		{
			this.RefreshSymbolPage();
			this.RefreshAddedSkillItem();
			this.RefreshRcmdEquipPlanPanel();
		}

		public void RefreshLeft()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.transform.Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			COM_PLAYERCAMP cOM_PLAYERCAMP = masterMemberInfo.camp;
			if (cOM_PLAYERCAMP == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			this.InitTeamHeroList(component, cOM_PLAYERCAMP);
		}

		public void RefreshRight()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			CUIListScript component = form.transform.Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
			COM_PLAYERCAMP camp;
			if (masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			else
			{
				camp = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp);
			}
			this.InitTeamHeroList(component, camp);
		}

		public void RefreshCenter()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			this.InitHeroList(form, false);
			this.InitSkinList(form, 0u);
		}

		public void RefreshSwapPanel()
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("PanelSwap/PanelSwapHero");
			transform.gameObject.CustomSetActive(false);
			if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enIdle)
			{
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID);
			MemberInfo memberInfo2 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID);
			if (masterMemberInfo == null || memberInfo == null || memberInfo2 == null)
			{
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
			IHeroData heroData2 = CHeroDataFactory.CreateHeroData(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
			IHeroData heroData3 = CHeroDataFactory.CreateHeroData(memberInfo2.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
			if (heroData == null || heroData2 == null || heroData3 == null)
			{
				return;
			}
			GameObject gameObject = transform.Find("heroItemCell1").gameObject;
			GameObject gameObject2 = transform.Find("heroItemCell2").gameObject;
			GameObject gameObject3 = transform.Find("btnConfirmSwap").gameObject;
			GameObject gameObject4 = transform.Find("btnConfirmSwapCanel").gameObject;
			if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
			{
				CUICommonSystem.SetHeroItemData(form, gameObject, heroData2, enHeroHeadType.enIcon, false, true);
				CUICommonSystem.SetHeroItemData(form, gameObject2, heroData, enHeroHeadType.enIcon, false, true);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(true);
			}
			else
			{
				CUICommonSystem.SetHeroItemData(form, gameObject, heroData3, enHeroHeadType.enIcon, false, true);
				CUICommonSystem.SetHeroItemData(form, gameObject2, heroData, enHeroHeadType.enIcon, false, true);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(true);
			}
			RectTransform rectTransform = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(masterMemberInfo.ullUid, masterMemberInfo.camp) as RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			RectTransform rectTransform2 = transform.transform as RectTransform;
			rectTransform2.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width, rectTransform.anchoredPosition.y);
			transform.gameObject.CustomSetActive(true);
		}

		public void InitBanHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
		{
			List<uint> banHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetBanHeroList(camp);
			listScript.SetElementAmount(Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount);
			for (int i = 0; i < banHeroList.get_Count(); i++)
			{
				Transform transform = listScript.GetElemenet(i).transform;
				IHeroData heroData = CHeroDataFactory.CreateHeroData(banHeroList.get_Item(i));
				if (heroData != null)
				{
					CUICommonSystem.SetObjActive(transform.transform.Find("imageIcon"), true);
					CUICommonSystem.SetHeroItemData(listScript.m_belongedFormScript, transform.gameObject, heroData, enHeroHeadType.enBustCircle, false, true);
				}
			}
		}

		public void InitTeamHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
		{
			List<uint> teamHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetTeamHeroList(camp);
			listScript.SetElementAmount(teamHeroList.get_Count());
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			for (int i = 0; i < teamHeroList.get_Count(); i++)
			{
				ListView<MemberInfo> listView = Singleton<CHeroSelectBaseSystem>.instance.roomInfo[camp];
				MemberInfo memberInfo = listView[i];
				uint num = teamHeroList.get_Item(i);
				if (listView == null || memberInfo == null)
				{
					return;
				}
				Transform transform = listScript.GetElemenet(i).transform;
				GameObject gameObject = transform.Find("BgState/NormalBg").gameObject;
				GameObject gameObject2 = transform.Find("BgState/NextBg").gameObject;
				GameObject gameObject3 = transform.Find("BgState/CurrentBg").gameObject;
				CUITimerScript component = transform.Find("BgState/CurrentBg/Timer").GetComponent<CUITimerScript>();
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				component.gameObject.CustomSetActive(false);
				if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
				{
					gameObject.CustomSetActive(true);
				}
				else
				{
					if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan && Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enPick)
					{
						return;
					}
					if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(memberInfo))
					{
						gameObject3.CustomSetActive(true);
						component.gameObject.CustomSetActive(true);
						if (!component.IsRunning())
						{
							component.SetTotalTime(Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.dwTimeout / 1000u);
							component.ReStartTimer();
						}
					}
					else if (Singleton<CHeroSelectBaseSystem>.instance.IsNextBanOrPickMember(memberInfo))
					{
						gameObject2.CustomSetActive(true);
						component.EndTimer();
					}
					else
					{
						gameObject.CustomSetActive(true);
						component.EndTimer();
					}
				}
				GameObject gameObject4 = transform.Find("heroItemCell").gameObject;
				Text component2 = gameObject4.transform.Find("lblName").gameObject.GetComponent<Text>();
				GameObject gameObject5 = transform.Find("heroItemCell/readyIcon").gameObject;
				Image component3 = gameObject4.transform.Find("imageIcon").gameObject.GetComponent<Image>();
				if (num != 0u)
				{
					IHeroData heroData = CHeroDataFactory.CreateHeroData(num);
					if (heroData != null)
					{
						CUICommonSystem.SetHeroItemData(listScript.m_belongedFormScript, gameObject4, heroData, enHeroHeadType.enIcon, false, true);
					}
					component3.gameObject.CustomSetActive(true);
				}
				if (memberInfo.camp == masterMemberInfo.camp || masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					if (memberInfo == masterMemberInfo)
					{
						component2.set_text(Singleton<CTextManager>.instance.GetText("Pvp_PlayerName", new string[]
						{
							memberInfo.MemberName
						}));
					}
					else
					{
						component2.set_text(memberInfo.MemberName);
					}
				}
				else
				{
					component2.set_text(Singleton<CTextManager>.instance.GetText("Matching_Tip_9", new string[]
					{
						(memberInfo.dwPosOfCamp + 1u).ToString()
					}));
				}
				gameObject5.CustomSetActive(memberInfo.isPrepare);
				CUICommonSystem.SetObjActive(gameObject4.transform.Find("VoiceIcon"), false);
				Button component4 = transform.Find("ExchangeBtn").GetComponent<Button>();
				if (masterMemberInfo.camp != camp)
				{
					component4.gameObject.CustomSetActive(false);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
				{
					component4.gameObject.CustomSetActive(false);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState != enSwapHeroState.enReqing && memberInfo != masterMemberInfo)
				{
					if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(masterMemberInfo, num) && Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(memberInfo, masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID))
					{
						component4.gameObject.CustomSetActive(true);
						CUIEventScript component5 = component4.GetComponent<CUIEventScript>();
						if (component5 != null)
						{
							component5.m_onClickEventParams.tagUInt = memberInfo.dwObjId;
						}
					}
					else
					{
						component4.gameObject.CustomSetActive(false);
					}
				}
				else
				{
					component4.gameObject.CustomSetActive(false);
				}
				GameObject selSkillCell = transform.Find("selSkillItemCell").gameObject;
				uint selSkillID = memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
				if (selSkillID != 0u && camp == masterMemberInfo.camp)
				{
					GameDataMgr.addedSkiilDatabin.Accept(delegate(ResSkillUnlock rule)
					{
						if (rule != null && rule.dwUnlockSkillID == selSkillID)
						{
							ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(selSkillID);
							if (dataByKey != null)
							{
								string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
								selSkillCell.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, listScript.m_belongedFormScript, true, false, false, false);
								selSkillCell.CustomSetActive(true);
							}
							else
							{
								DebugHelper.Assert(false, string.Format("SelSkill ResSkillCfgInfo[{0}] can not be find!!", selSkillID));
							}
						}
					});
				}
				else
				{
					selSkillCell.gameObject.CustomSetActive(false);
				}
				if (memberInfo.camp == masterMemberInfo.camp)
				{
					Transform transform2 = transform.Find("RecentUseHeroPanel");
					if (transform2 != null)
					{
						if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(memberInfo) || Singleton<CHeroSelectBaseSystem>.instance.IsPickedMember(memberInfo) || Singleton<CHeroSelectBaseSystem>.instance.gameType != enSelectGameType.enLadder)
						{
							transform2.gameObject.CustomSetActive(false);
							selSkillCell.CustomSetActive(selSkillCell.activeSelf);
						}
						else
						{
							selSkillCell.CustomSetActive(false);
							transform2.gameObject.CustomSetActive(true);
							int num2 = 0;
							while (num2 < 3 && num2 < memberInfo.recentUsedHero.astHeroInfo.Length)
							{
								Transform transform3 = transform2.transform.FindChild(string.Format("Element{0}", num2));
								if (transform3 != null && !CLadderSystem.IsRecentUsedHeroMaskSet(ref memberInfo.recentUsedHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE) && (long)num2 < (long)((ulong)memberInfo.recentUsedHero.dwHeroNum) && memberInfo.recentUsedHero.astHeroInfo[num2].dwHeroID != 0u)
								{
									CUICommonSystem.SetObjActive(transform3.transform.Find("imageIcon"), true);
									IHeroData data = CHeroDataFactory.CreateHeroData(memberInfo.recentUsedHero.astHeroInfo[num2].dwHeroID);
									CUICommonSystem.SetHeroItemData(listScript.m_belongedFormScript, transform3.gameObject, data, enHeroHeadType.enBustCircle, false, true);
								}
								else
								{
									CUICommonSystem.SetObjActive(transform3.transform.Find("imageIcon"), false);
								}
								num2++;
							}
						}
					}
				}
				else
				{
					Transform transform4 = transform.Find("RecentUseHeroPanel");
					if (transform4 != null)
					{
						transform4.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void InitFullHeroListData(ListView<IHeroData> sourceList)
		{
			this.m_canUseHeroListByJob.Clear();
			for (int i = 0; i < sourceList.Count; i++)
			{
				if (this.m_heroSelectJobType == enHeroJobType.All || sourceList[i].heroCfgInfo.bMainJob == (byte)this.m_heroSelectJobType || sourceList[i].heroCfgInfo.bMinorJob == (byte)this.m_heroSelectJobType)
				{
					this.m_canUseHeroListByJob.Add(sourceList[i]);
				}
			}
			CHeroOverviewSystem.SortHeroList(ref this.m_canUseHeroListByJob, Singleton<CHeroSelectBaseSystem>.instance.m_sortType, false);
		}

		public void InitHeroList(CUIFormScript form, bool isResetSelect = false)
		{
			CUIListScript component = form.transform.Find("PanelCenter/ListHostHeroInfo").GetComponent<CUIListScript>();
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				this.InitFullHeroListData(this.m_banHeroList);
				component.SetElementAmount(this.m_canUseHeroListByJob.Count);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				this.InitFullHeroListData(Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList);
				component.SetElementAmount(this.m_canUseHeroListByJob.Count);
			}
			else
			{
				component.gameObject.CustomSetActive(false);
			}
			Button component2 = form.transform.Find("PanelCenter/ListHostHeroInfo/btnConfirmSelectHero").GetComponent<Button>();
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap && masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				if (masterMemberInfo == null)
				{
					return;
				}
				if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo) && Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan && this.m_selectBanHeroData != null)
				{
					CUICommonSystem.SetButtonEnableWithShader(component2, true, true);
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo) && Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick && Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0) != 0u)
				{
					CUICommonSystem.SetButtonEnableWithShader(component2, true, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnableWithShader(component2, false, true);
				}
				if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
				{
					CUICommonSystem.SetButtonName(component2.gameObject, Singleton<CTextManager>.instance.GetText("BP_SureButton_1"));
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
				{
					CUICommonSystem.SetButtonName(component2.gameObject, Singleton<CTextManager>.instance.GetText("BP_SureButton_2"));
				}
			}
			else
			{
				CUICommonSystem.SetButtonEnableWithShader(component2, false, true);
			}
			if (isResetSelect)
			{
				component.SelectElement(-1, true);
			}
		}

		public void InitSkinList(CUIFormScript form, uint customHeroID = 0u)
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo() == null)
			{
				return;
			}
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			if (customHeroID != 0u)
			{
				num = customHeroID;
			}
			ListView<ResHeroSkin> listView = new ListView<ResHeroSkin>();
			ListView<ResHeroSkin> listView2 = new ListView<ResHeroSkin>();
			int index = -1;
			if (num != 0u)
			{
				ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(num);
				for (int i = 0; i < availableSkinByHeroId.Count; i++)
				{
					ResHeroSkin resHeroSkin = availableSkinByHeroId[i];
					if (masterRoleInfo.IsCanUseSkin(num, resHeroSkin.dwSkinID) || CBagSystem.CanUseSkinExpCard(resHeroSkin.dwID))
					{
						listView.Add(resHeroSkin);
					}
					else
					{
						listView2.Add(resHeroSkin);
					}
					if (masterRoleInfo.GetHeroWearSkinId(num) == resHeroSkin.dwSkinID)
					{
						index = listView.Count - 1;
					}
				}
				listView.AddRange(listView2);
			}
			Transform transform = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
			Transform transform2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo/panelEffect");
			if (transform == null)
			{
				return;
			}
			CUIListScript[] array = new CUIListScript[]
			{
				transform.GetComponent<CUIListScript>()
			};
			for (int j = 0; j < array.Length; j++)
			{
				CUIListScript cUIListScript = array[j];
				cUIListScript.SetElementAmount(listView.Count);
				for (int k = 0; k < listView.Count; k++)
				{
					CUIListElementScript elemenet = cUIListScript.GetElemenet(k);
					Transform transform3 = cUIListScript.GetElemenet(k).transform;
					Image component = transform3.Find("imageIcon").GetComponent<Image>();
					Image component2 = transform3.Find("imageIconGray").GetComponent<Image>();
					Text component3 = transform3.Find("lblName").GetComponent<Text>();
					GameObject gameObject = transform3.Find("imgExperienceMark").gameObject;
					Transform transform4 = transform3.Find("expCardPanel");
					ResHeroSkin resHeroSkin2 = listView[k];
					bool flag = masterRoleInfo.IsValidExperienceSkin(num, resHeroSkin2.dwSkinID);
					gameObject.CustomSetActive(flag);
					bool flag2 = !masterRoleInfo.IsCanUseSkin(num, resHeroSkin2.dwSkinID) && CBagSystem.CanUseSkinExpCard(resHeroSkin2.dwID);
					RectTransform rectTransform = (RectTransform)component3.transform;
					RectTransform rectTransform2 = (RectTransform)transform4;
					if (flag2)
					{
						transform4.gameObject.CustomSetActive(true);
						rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform2.anchoredPosition.y + rectTransform.rect.height);
					}
					else
					{
						transform4.gameObject.CustomSetActive(false);
						rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform2.anchoredPosition.y);
					}
					if (masterRoleInfo.IsCanUseSkin(num, resHeroSkin2.dwSkinID) || CBagSystem.CanUseSkinExpCard(resHeroSkin2.dwID))
					{
						component.gameObject.CustomSetActive(true);
						component2.gameObject.CustomSetActive(false);
						elemenet.enabled = true;
					}
					else
					{
						component.gameObject.CustomSetActive(false);
						component2.gameObject.CustomSetActive(true);
						elemenet.enabled = false;
					}
					GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref resHeroSkin2.szSkinPicID), true, true);
					component.SetSprite(spritePrefeb, false);
					component2.SetSprite(spritePrefeb, false);
					component3.set_text(StringHelper.UTF8BytesToString(ref resHeroSkin2.szSkinName));
					CUIEventScript component4 = transform3.GetComponent<CUIEventScript>();
					component4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSelect_BanPick_SkinSelect, new stUIEventParams
					{
						tagUInt = resHeroSkin2.dwSkinID,
						commonBool = flag
					});
				}
				cUIListScript.SelectElement(index, true);
			}
			transform2.gameObject.CustomSetActive(false);
		}

		private void CenterHeroItemEnable(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			CUIListScript srcWidgetBelongedListScript = uiEvent.m_srcWidgetBelongedListScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcFormScript == null || srcWidgetBelongedListScript == null || srcWidget == null || masterRoleInfo == null || srcWidgetIndexInBelongedList < 0)
			{
				return;
			}
			IHeroData heroData = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
			if (heroData == null)
			{
				return;
			}
			CUIListElementScript component = srcWidget.GetComponent<CUIListElementScript>();
			if (component == null)
			{
				return;
			}
			GameObject gameObject = srcWidget.transform.Find("heroItemCell").gameObject;
			GameObject gameObject2 = gameObject.transform.Find("TxtFree").gameObject;
			GameObject gameObject3 = gameObject.transform.Find("TxtCreditFree").gameObject;
			GameObject gameObject4 = gameObject.transform.Find("imgExperienceMark").gameObject;
			Transform transform = gameObject.transform.Find("expCardPanel");
			CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
			CUIEventScript component3 = srcWidget.GetComponent<CUIEventScript>();
			gameObject2.CustomSetActive(false);
			gameObject3.CustomSetActive(false);
			gameObject4.CustomSetActive(false);
			transform.gameObject.CustomSetActive(false);
			component2.enabled = false;
			component3.enabled = false;
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				bool flag = masterRoleInfo.IsFreeHero(heroData.cfgID);
				bool flag2 = masterRoleInfo.IsCreditFreeHero(heroData.cfgID);
				gameObject2.CustomSetActive(flag && !flag2);
				gameObject3.CustomSetActive(flag2);
				if (masterRoleInfo.IsValidExperienceHero(heroData.cfgID))
				{
					gameObject4.CustomSetActive(true);
				}
				else
				{
					gameObject4.CustomSetActive(false);
				}
			}
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan || Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
				{
					if (Singleton<CHeroSelectBaseSystem>.instance.IsBanByHeroID(heroData.cfgID) || Singleton<CHeroSelectBaseSystem>.instance.IsHeroExist(heroData.cfgID))
					{
						CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, heroData, enHeroHeadType.enIcon, true, true);
					}
					else
					{
						component2.enabled = true;
						component3.enabled = true;
						CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, heroData, enHeroHeadType.enIcon, false, true);
					}
				}
				else
				{
					CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, heroData, enHeroHeadType.enIcon, true, true);
				}
				if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
				{
					if (this.m_selectBanHeroData != null)
					{
						if (heroData.cfgID == this.m_selectBanHeroData.heroCfgInfo.dwCfgID)
						{
							component.ChangeDisplay(true);
						}
						else
						{
							component.ChangeDisplay(false);
						}
					}
					else
					{
						component.ChangeDisplay(false);
					}
				}
				else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
				{
					if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount > 0)
					{
						if (heroData.cfgID == Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0))
						{
							component.ChangeDisplay(true);
						}
						else
						{
							component.ChangeDisplay(false);
						}
					}
					else
					{
						component.ChangeDisplay(false);
					}
				}
				return;
			}
		}

		public void StartEndTimer(int totlaTimes)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("Top/Timer/CountDown");
			if (transform == null)
			{
				return;
			}
			CUITimerScript component = transform.GetComponent<CUITimerScript>();
			if (component == null)
			{
				return;
			}
			component.SetTotalTime((float)totlaTimes);
			component.m_timerType = enTimerType.CountDown;
			component.ReStartTimer();
			component.gameObject.CustomSetActive(true);
		}

		private void OnTimerCountDown(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcFormScript == null || uiEvent.m_srcWidget == null)
			{
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			Transform transform = srcFormScript.transform.Find("Top/Timer/CountDownMovie");
			CUITimerScript component = uiEvent.m_srcWidget.GetComponent<CUITimerScript>();
			if (component.GetCurrentTime() <= 6.1f && !transform.gameObject.activeSelf)
			{
				transform.gameObject.CustomSetActive(true);
				component.gameObject.CustomSetActive(false);
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_daojishi", null);
				Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_5", null);
			}
		}

		private void OnChangeRcmdEquipPlan(CUIEvent uiEvent)
		{
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			if (num == 0u)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.m_rcmdEquipInfo != null)
			{
				stRcmdEquipListInfo rcmdEquipListInfo = masterRoleInfo.m_rcmdEquipInfo.GetRcmdEquipListInfo(num);
				CEquipSystem.OpenSelfEquipPlanForm(num, ref rcmdEquipListInfo, enUIEventID.CustomEquip_UseEquipPlanListItem, false, false);
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form != null)
			{
				Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(form.transform.FindChild("Bottom/Panel_RcmdEquipPlan").gameObject, enNewFlagKey.New_SelectHero_CustomEquip_V14, true);
			}
		}

		private void OnHeroJobMenuSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.m_heroSelectJobType = (enHeroJobType)selectedIndex;
			this.InitHeroList(uiEvent.m_srcFormScript, false);
		}

		private void ResetHeroSelectJobType()
		{
			this.m_heroSelectJobType = enHeroJobType.All;
		}

		private void HeroSelect_SelectHero(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_BanPick_Swicth", null);
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
			{
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_canUseHeroListByJob.Count && Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
				{
					this.m_selectBanHeroData = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
					this.RefreshCenter();
				}
				return;
			}
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_canUseHeroListByJob.Count)
				{
					return;
				}
				IHeroData heroData = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
				if (heroData == null)
				{
					return;
				}
				CHeroSelectBaseSystem.SendHeroSelectMsg(0, 0, heroData.cfgID);
			}
		}

		private void HeroSelect_ConfirmHeroSelect(CUIEvent uiEvent)
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
			{
				if (this.m_selectBanHeroData != null)
				{
					CHeroSelectBaseSystem.SendBanHeroMsg(this.m_selectBanHeroData.cfgID);
				}
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
			{
				CHeroSelectBaseSystem.SendMuliPrepareToBattleMsg();
			}
		}

		private void HeroSelect_SwapHeroReq(CUIEvent uiEvent)
		{
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			CHeroSelectBaseSystem.SendSwapHeroMsg(tagUInt);
		}

		private void HeroSelect_SwapHeroAllow(CUIEvent uiEvent)
		{
			CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(1);
		}

		private void HeroSelect_SwapHeroCanel(CUIEvent uiEvent)
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
			{
				CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(0);
			}
			else if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enReqing)
			{
				CHeroSelectBaseSystem.SendCanelSwapHeroMsg();
			}
		}

		private void HeroSelect_OnSkinSelect(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			bool commonBool = uiEvent.m_eventParams.commonBool;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
			Transform transform2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo/panelEffect/List");
			if (transform == null || transform2 == null)
			{
				return;
			}
			CUIListScript component = transform.GetComponent<CUIListScript>();
			if (masterRoleInfo.IsCanUseSkin(num, tagUInt))
			{
				this.InitSkinEffect(transform2.gameObject, num, tagUInt);
			}
			else
			{
				component.SelectElement(component.GetLastSelectedIndex(), true);
			}
			if (masterRoleInfo.IsCanUseSkin(num, tagUInt))
			{
				if (masterRoleInfo.GetHeroWearSkinId(num) != tagUInt)
				{
					CHeroInfoSystem2.ReqWearHeroSkin(num, tagUInt, true);
				}
			}
			else
			{
				CHeroSkinBuyManager.OpenBuyHeroSkinForm3D(num, tagUInt, false);
			}
		}

		private void InitSkinEffect(GameObject objList, uint heroID, uint skinID)
		{
			CSkinInfo.GetHeroSkinProp(heroID, skinID, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr, ref CHeroSelectBaseSystem.s_propImgArr);
			CUICommonSystem.SetListProp(objList, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
		}

		public void OnHeroSkinWearSuc(uint heroId, uint skinId)
		{
			this.RefreshCenter();
		}

		public void OpenSymbolPropPanel(Transform propPanel, int pageIndex)
		{
			GameObject gameObject = propPanel.gameObject;
			GameObject gameObject2 = gameObject.gameObject.transform.Find("basePropPanel").gameObject;
			GameObject gameObject3 = gameObject2.transform.Find("List").gameObject;
			CSymbolSystem.RefreshSymbolPageProp(gameObject3, pageIndex, true);
			gameObject.gameObject.CustomSetActive(true);
		}

		private void OnCloseSymbolProp(CUIEvent uiEvent)
		{
			this.SetEffectNoteVisiable(true);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find(CHeroSelectBanPickSystem.s_symbolPropPanelPath).gameObject;
			gameObject.gameObject.CustomSetActive(false);
		}

		public void OnSymbolPageDownBtnClick(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CHeroInfo cHeroInfo;
			if (masterRoleInfo.GetHeroInfo(num, out cHeroInfo, true))
			{
				Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OpenSymbolPageForm(enSymbolPageOpenSrc.enHeroSelectBanPic, cHeroInfo.m_selectPageIndex);
			}
			else if (masterRoleInfo.IsFreeHero(num))
			{
				int freeHeroSymbolId = (int)masterRoleInfo.GetFreeHeroSymbolId(num);
				Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OpenSymbolPageForm(enSymbolPageOpenSrc.enHeroSelectBanPic, freeHeroSymbolId);
			}
			else
			{
				Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OpenSymbolPageForm(enSymbolPageOpenSrc.enHeroSelectBanPic, 0);
			}
		}

		public void OnHeroSymbolPageSelect(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			if (num == 0u)
			{
				return;
			}
			CHeroSelectBaseSystem.SendHeroSelectSymbolPage(num, srcWidgetIndexInBelongedList, false);
		}

		public void OnSymbolPageChange()
		{
			this.RefreshSymbolPage();
		}

		public void OnRcmdEquipPlanChangeSuccess()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			this.RefreshRcmdEquipPlanPanel();
			Transform transform = form.gameObject.transform.Find("Bottom/Panel_RcmdEquipPlan");
			if (transform != null)
			{
				CUICommonSystem.PlayAnimator(transform.gameObject, "EquipChange_Anim");
			}
		}

		public void RefreshRcmdEquipPlanPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			Transform transform = form.gameObject.transform.Find("Bottom/Panel_RcmdEquipPlan");
			transform.gameObject.CustomSetActive(false);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "RefreshRcmdEquipPlanPlan role is null");
				return;
			}
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			if (GameDataMgr.heroDatabin.GetDataByKey(num) != null)
			{
				uint rcmdEquipCurUseId = masterRoleInfo.m_rcmdEquipInfo.GetRcmdEquipCurUseId(num);
				string rcmdEquipPlanName = masterRoleInfo.m_rcmdEquipInfo.GetRcmdEquipPlanName(num, rcmdEquipCurUseId);
				Text component = transform.Find("Text").GetComponent<Text>();
				component.set_text(rcmdEquipPlanName);
				transform.gameObject.CustomSetActive(true);
			}
		}

		public void RefreshSymbolPage()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			Transform transform = form.gameObject.transform.Find("Bottom/Panel_SymbolChange");
			transform.gameObject.CustomSetActive(false);
			if (Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL) && Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan && GameDataMgr.heroDatabin.GetDataByKey(num) != null)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				CHeroInfo cHeroInfo;
				int selectIndex;
				if (masterRoleInfo.GetHeroInfo(num, out cHeroInfo, true))
				{
					selectIndex = cHeroInfo.m_selectPageIndex;
				}
				else
				{
					selectIndex = (int)masterRoleInfo.GetFreeHeroSymbolId(num);
				}
				transform.gameObject.CustomSetActive(true);
				CHeroSelectBanPickSystem.SetPageDropListDataByHeroSelect(transform.gameObject, selectIndex);
			}
			else
			{
				transform.gameObject.CustomSetActive(false);
			}
		}

		public static void SetPageDropListDataByHeroSelect(GameObject panelObj, int selectIndex)
		{
			if (panelObj == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			Text component = panelObj.transform.Find("Button_Down/Text").GetComponent<Text>();
			component.set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageName(selectIndex));
			Text component2 = panelObj.transform.Find("Button_Down/SymbolLevel/Text").GetComponent<Text>();
			component2.set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString());
			Text component3 = panelObj.transform.Find("Button_Down/SymbolLevel/Text").GetComponent<Text>();
			component3.set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString());
		}

		public void RefreshAddedSkillItem()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("Bottom/AddedSkillItem").gameObject;
			gameObject.CustomSetActive(false);
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (!CAddSkillSys.IsSelSkillAvailable() || Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan || num == 0u || masterMemberInfo == null)
			{
				return;
			}
			uint dwSelSkillID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(dwSelSkillID);
			bool flag = true;
			if (dataByKey == null)
			{
				DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwSelSkillID));
				return;
			}
			gameObject.CustomSetActive(true);
			string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
			Image component = gameObject.transform.Find("Icon").GetComponent<Image>();
			component.SetSprite(prefabPath, form, true, false, false, false);
			string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, num);
			if (flag)
			{
				form.transform.Find("PanelAddSkill/AddSkillTitletxt").GetComponent<Text>().set_text(dataByKey.szSkillName);
				form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().set_text(skillDescLobby);
				form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int)dwSelSkillID;
				ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
				for (int i = 0; i < selSkillAvailable.Count; i++)
				{
					if (selSkillAvailable[i].dwUnlockSkillID == dwSelSkillID)
					{
						CUIToggleListScript component2 = form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>();
						component2.SelectElement(i, true);
						break;
					}
				}
			}
		}

		public void InitAddedSkillPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() == null)
			{
				return;
			}
			if (CAddSkillSys.IsSelSkillAvailable())
			{
				CUIToggleListScript component = form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>();
				ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
				component.SetElementAmount(selSkillAvailable.Count);
				int num = 0;
				ResSkillUnlock resSkillUnlock;
				for (int i = 0; i < selSkillAvailable.Count; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					CUIEventScript component2 = elemenet.GetComponent<CUIEventScript>();
					resSkillUnlock = selSkillAvailable[i];
					uint dwUnlockSkillID = resSkillUnlock.dwUnlockSkillID;
					ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(dwUnlockSkillID);
					if (dataByKey != null)
					{
						component2.m_onClickEventID = enUIEventID.HeroSelect_BanPick_AddedSkillSelected;
						component2.m_onClickEventParams.tag = (int)resSkillUnlock.dwUnlockSkillID;
						string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
						Image component3 = elemenet.transform.Find("Icon").GetComponent<Image>();
						component3.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
						elemenet.transform.Find("SkillNameTxt").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szSkillName));
						if ((ulong)dwUnlockSkillID == (ulong)((long)CAddSkillSys.SendSkillId))
						{
							CUICommonSystem.SetObjActive(elemenet.transform.FindChild("HelpBtn"), true);
						}
					}
					else
					{
						DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwUnlockSkillID));
					}
				}
				component.SelectElement(num, true);
				resSkillUnlock = GameDataMgr.addedSkiilDatabin.GetDataByIndex(num);
			}
			form.transform.Find("Bottom/AddedSkillItem").gameObject.CustomSetActive(false);
			form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
		}

		public void OnSelectedAddedSkill(CUIEvent uiEvent)
		{
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			if (num == 0u)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			uint tag = (uint)uiEvent.m_eventParams.tag;
			form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int)tag;
			ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(tag);
			if (dataByKey == null)
			{
				return;
			}
			string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, num);
			form.transform.Find("PanelAddSkill/AddSkillTitletxt").GetComponent<Text>().set_text(dataByKey.szSkillName);
			form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().set_text(skillDescLobby);
		}

		public void OnConfirmAddedSkill(CUIEvent uiEvent)
		{
			uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.get_Item(0);
			uint tag = (uint)uiEvent.m_eventParams.tag;
			if (num == 0u || Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill == null || !CAddSkillSys.IsSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, tag))
			{
				DebugHelper.Assert(false, string.Format("CHeroSelectBanPickSystem heroID[{0}] addedSkillID[{1}]", num, tag));
			}
			else
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1166u);
				cSPkg.stPkgData.stUnlockSkillSelReq.dwHeroID = num;
				cSPkg.stPkgData.stUnlockSkillSelReq.dwSkillID = tag;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			this.OnCloseAddedSkillPanel(null);
		}

		public void OnOpenAddedSkillPanel(CUIEvent uiEvent)
		{
			this.SetEffectNoteVisiable(false);
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(true);
			if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
			{
				Singleton<CChatController>.instance.Hide_SelectChat_MidNode();
				Singleton<CChatController>.instance.Set_Show_Bottom(false);
				Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(false);
			}
		}

		public void OnCloseAddedSkillPanel(CUIEvent uiEvent)
		{
			this.SetEffectNoteVisiable(true);
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
			if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
			{
				Singleton<CChatController>.instance.Set_Show_Bottom(true);
				Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(true);
			}
		}

		private void SetEffectNoteVisiable(bool isShow)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.transform.Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
			CUIListScript component2 = form.transform.Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
			CUIListScript[] array = new CUIListScript[]
			{
				component,
				component2
			};
			for (int i = 0; i < array.Length; i++)
			{
				CUIListScript cUIListScript = array[i];
				int elementAmount = cUIListScript.GetElementAmount();
				for (int j = 0; j < elementAmount; j++)
				{
					Transform transform = cUIListScript.GetElemenet(j).transform;
					CUICommonSystem.SetObjActive(transform.Find("BgState/CurrentBg/UI_BR_effect"), isShow);
				}
			}
		}

		public void PlayStepTitleAnimation()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			CUICommonSystem.PlayAnimation(form.transform.Find("Top/Tips"), null);
		}

		public void PlayCurrentBgAnimation()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CHeroSelectBanPickSystem.s_heroSelectFormPath);
			if (form == null)
			{
				return;
			}
			CSDT_BAN_PICK_STATE_INFO stCurState = Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState;
			for (int i = 0; i < (int)stCurState.bPosNum; i++)
			{
				MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP)stCurState.bCamp, (int)stCurState.szPosList[i]);
				if (memberInfo != null)
				{
					Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(memberInfo.ullUid, memberInfo.camp);
					if (teamPlayerElement == null)
					{
						return;
					}
					CUICommonSystem.PlayAnimation(teamPlayerElement.Find("BgState/CurrentBg"), null);
				}
			}
		}
	}
}

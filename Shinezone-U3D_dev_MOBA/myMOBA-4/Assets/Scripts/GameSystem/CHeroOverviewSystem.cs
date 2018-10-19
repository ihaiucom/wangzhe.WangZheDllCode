using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CHeroOverviewSystem : Singleton<CHeroOverviewSystem>
	{
		public const string HERO_OVERVIEW_HERO_OWN_FLAG_KEY = "Hero_Overview_Hero_Own_Flag_Key";

		public static string s_heroViewFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Overview.prefab";

		protected ListView<IHeroData> m_heroList = new ListView<IHeroData>();

		protected enHeroJobType m_selectHeroType;

		private bool m_ownFlag;

		private CMallSortHelper.HeroViewSortType m_heroSortType;

		private bool m_bSortDesc;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroViewForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroView_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroView_ItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroView_MenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SortTypeClick, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SortTypeSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenStoryUrl, new CUIEventManager.OnUIEventHandler(this.OnOpenStoryUrl));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			this.m_ownFlag = (PlayerPrefs.GetInt("Hero_Overview_Hero_Own_Flag_Key", 0) != 0);
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroViewForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroView_CloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroView_ItemEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroView_MenuSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_CloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_SortTypeClick, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_SortTypeSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeSelect));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
		}

		public void OnOpenHeroViewForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroOverviewSystem.s_heroViewFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority0);
			this.m_selectHeroType = enHeroJobType.All;
			CMallSortHelper.CreateHeroViewSorter().SetSortType(this.m_heroSortType);
			this.ResetHeroListData(true);
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
			GameObject gameObject = cUIFormScript.transform.Find("Panel_Menu/List").gameObject;
			CUICommonSystem.InitMenuPanel(gameObject, titleList, (int)this.m_selectHeroType, true);
			this.RefreshHeroOwnFlag();
			this.ResetHeroSortTypeList();
			GameObject widget = cUIFormScript.GetWidget(4);
			Singleton<CUINewFlagSystem>.instance.AddNewFlag(widget, enNewFlagKey.New_HeroOverViewOpenStoryUrl_V14, enNewFlagPos.enTopRight, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroListBtn);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = cUIFormScript.transform.FindChild("Panel_Hero/Panel_Hero_TitleBg/BtnStoryWeb");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		public void OnHeroView_CloseForm(CUIEvent uiEvent)
		{
			Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority1);
		}

		private void ResetHeroListData(bool bResetData = true)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel_Hero/List").gameObject;
			CUIListScript component = gameObject2.GetComponent<CUIListScript>();
			this.FilterHeroData(!bResetData);
			component.SetElementAmount(this.m_heroList.Count);
		}

		private void FilterHeroData(bool bOnlySort)
		{
			ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
			if (!bOnlySort)
			{
				this.m_heroList.Clear();
				for (int i = 0; i < allHeroList.Count; i++)
				{
					if (this.m_selectHeroType == enHeroJobType.All || allHeroList[i].bMainJob == (byte)this.m_selectHeroType || allHeroList[i].bMinorJob == (byte)this.m_selectHeroType)
					{
						IHeroData heroData = CHeroDataFactory.CreateHeroData(allHeroList[i].dwCfgID);
						CMallItem cMallItem = new CMallItem(heroData.cfgID, CMallItem.IconType.Normal);
						if (this.m_ownFlag)
						{
							if (cMallItem.Owned(false))
							{
								this.m_heroList.Add(heroData);
							}
						}
						else
						{
							this.m_heroList.Add(heroData);
						}
					}
				}
			}
			HeroViewSortImp heroViewSortImp = CMallSortHelper.CreateHeroViewSorter();
			if (heroViewSortImp != null)
			{
				heroViewSortImp.SetSortType(this.m_heroSortType);
				this.m_heroList.Sort(heroViewSortImp);
				if (this.m_bSortDesc)
				{
					this.m_heroList.Reverse();
				}
			}
		}

		public static void SortHeroList(ref ListView<IHeroData> heroList, CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Default, bool bDesc = false)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return;
			}
			HeroViewSortImp heroViewSortImp = CMallSortHelper.CreateHeroViewSorter();
			if (heroViewSortImp != null)
			{
				heroViewSortImp.SetSortType(sortType);
				heroList.Sort(heroViewSortImp);
			}
			if (bDesc)
			{
				heroList.Reverse();
			}
		}

		public void OnHeroView_ItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_heroList.Count)
			{
				return;
			}
			GameObject srcWidget = uiEvent.m_srcWidget;
			GameObject gameObject = srcWidget.transform.Find("heroItem").gameObject;
			CHeroOverviewSystem.SetPveHeroItemData(uiEvent.m_srcFormScript, gameObject, this.m_heroList[srcWidgetIndexInBelongedList]);
		}

		public static void SetPveHeroItemData(CUIFormScript formScript, GameObject listItem, IHeroData data)
		{
			if (listItem == null || data == null)
			{
				return;
			}
			bool bPlayerOwn = data.bPlayerOwn;
			Transform transform = listItem.transform;
			Transform transform2 = transform.Find("heroProficiencyImg");
			Transform transform3 = transform.Find("heroProficiencyBgImg");
			CUICommonSystem.SetHeroProficiencyIconImage(formScript, transform2.gameObject, (int)data.proficiencyLV);
			CUICommonSystem.SetHeroProficiencyBgImage(formScript, transform3.gameObject, (int)data.proficiencyLV, false);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				transform3.GetComponent<Image>().color = ((!masterRoleInfo.IsHaveHero(data.cfgID, true)) ? CUIUtility.s_Color_GrayShader : Color.white);
				transform2.GetComponent<Image>().color = ((!masterRoleInfo.IsHaveHero(data.cfgID, true)) ? CUIUtility.s_Color_GrayShader : Color.white);
			}
			bool flag = false;
			bool flag2 = false;
			if (masterRoleInfo != null)
			{
				flag = masterRoleInfo.IsFreeHero(data.cfgID);
				flag2 = masterRoleInfo.IsCreditFreeHero(data.cfgID);
				bool flag3 = masterRoleInfo.IsValidExperienceHero(data.cfgID);
				CUICommonSystem.SetHeroItemImage(formScript, listItem, masterRoleInfo.GetHeroSkinPic(data.cfgID), enHeroHeadType.enBust, !bPlayerOwn && !flag && !flag3, true);
			}
			GameObject gameObject = transform.Find("profession").gameObject;
			CUICommonSystem.SetHeroJob(formScript, gameObject, (enHeroJobType)data.heroType);
			Text component = transform.Find("heroNameText").GetComponent<Text>();
			component.text = data.heroName;
			Transform transform4 = transform.Find("TxtFree");
			Transform transform5 = transform.Find("TxtCreditFree");
			if (transform4 != null)
			{
				transform4.gameObject.CustomSetActive(flag && !flag2);
			}
			if (transform5 != null)
			{
				transform5.gameObject.CustomSetActive(flag2);
			}
			GameObject gameObject2 = transform.Find("imgExperienceMark").gameObject;
			gameObject2.CustomSetActive(data.IsValidExperienceHero());
			CUIEventScript component2 = listItem.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = data.cfgID;
			eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
			component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
		}

		public void OnHeroView_MenuSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			if (selectedIndex != (int)this.m_selectHeroType)
			{
				this.m_selectHeroType = (enHeroJobType)selectedIndex;
				this.ResetHeroListData(true);
			}
		}

		private void OnHeroInfo_Compose(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1110u);
			cSPkg.stPkgData.stItemComp.wTargetType = 4;
			cSPkg.stPkgData.stItemComp.dwTargetID = uiEvent.m_eventParams.heroId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void OnNtyAddHero(uint id)
		{
			this.ResetHeroListData(true);
		}

		private void OnHeroInfoChange(uint heroId)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CHeroInfo cHeroInfo;
			if (masterRoleInfo.GetHeroInfo(heroId, out cHeroInfo, false))
			{
				this.RefreshHeroListElement(heroId);
			}
		}

		private void RefreshHeroListElement(uint heroId)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel_Hero/List").gameObject;
			CUIListScript component = gameObject2.GetComponent<CUIListScript>();
			for (int i = 0; i < this.m_heroList.Count; i++)
			{
				if (this.m_heroList[i].cfgID == heroId)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					if (elemenet != null)
					{
						GameObject gameObject3 = elemenet.gameObject.transform.Find("heroItem").gameObject;
						CHeroOverviewSystem.SetPveHeroItemData(form, gameObject3, this.m_heroList[i]);
					}
					break;
				}
			}
		}

		private void OnHeroInfo_CloseForm(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority0);
			CHeroOverviewSystem.SortHeroList(ref this.m_heroList, this.m_heroSortType, this.m_bSortDesc);
			GameObject gameObject = form.transform.Find("Panel_Hero/List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_heroList.Count);
		}

		public int GetHeroIndexByConfigId(uint inCfgId)
		{
			for (int i = 0; i < this.m_heroList.Count; i++)
			{
				IHeroData heroData = this.m_heroList[i];
				if (heroData.cfgID == inCfgId)
				{
					return i;
				}
			}
			return 0;
		}

		public IHeroData GetHeroDataBuyIndex(int index)
		{
			if (index >= 0 && index < this.m_heroList.Count)
			{
				return this.m_heroList[index];
			}
			return null;
		}

		public int GetHeroListCount()
		{
			return this.m_heroList.Count;
		}

		private void OnHeroSortTypeClick(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			if (widget != null)
			{
				widget.CustomSetActive(!widget.activeSelf);
			}
		}

		private void OnHeroSortTypeSelect(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
			CMallSortHelper.HeroViewSortType srcWidgetIndexInBelongedList = (CMallSortHelper.HeroViewSortType)uiEvent.m_srcWidgetIndexInBelongedList;
			HeroViewSortImp heroViewSortImp = CMallSortHelper.CreateHeroViewSorter();
			heroViewSortImp.SetSortType(srcWidgetIndexInBelongedList);
			if (this.m_heroSortType != srcWidgetIndexInBelongedList)
			{
				this.m_bSortDesc = false;
				this.m_heroSortType = srcWidgetIndexInBelongedList;
				this.ResetHeroListData(true);
			}
			else if (srcWidgetIndexInBelongedList != CMallSortHelper.HeroViewSortType.Default)
			{
				this.m_bSortDesc = !this.m_bSortDesc;
				this.ResetHeroListData(false);
			}
			GameObject widget2 = form.GetWidget(1);
			if (widget2 != null)
			{
				widget2.GetComponent<Text>().text = CMallSortHelper.CreateHeroViewSorter().GetSortTypeName(this.m_heroSortType);
			}
		}

		private void OnHeroOwnFlagChange(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(3);
			if (widget != null)
			{
				this.m_ownFlag = widget.GetComponent<Toggle>().isOn;
				PlayerPrefs.SetInt("Hero_Overview_Hero_Own_Flag_Key", (!this.m_ownFlag) ? 0 : 1);
				PlayerPrefs.Save();
			}
			this.RefreshHeroOwnFlag();
			this.ResetHeroListData(true);
		}

		private void OnOpenStoryUrl(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenUrl(Singleton<CTextManager>.instance.GetText("HeroView_StoryUrl"), true, 0);
			if (uiEvent.m_srcWidget.gameObject != null)
			{
				Singleton<CUINewFlagSystem>.instance.DelNewFlag(uiEvent.m_srcWidget.gameObject, enNewFlagKey.New_HeroOverViewOpenStoryUrl_V14, true);
			}
		}

		private void RefreshHeroOwnFlag()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(3);
			if (widget != null)
			{
				widget.GetComponent<Toggle>().isOn = this.m_ownFlag;
			}
		}

		private void ResetHeroSortTypeList()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroOverviewSystem.s_heroViewFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			if (widget != null)
			{
				CUIListScript component = widget.GetComponent<CUIListScript>();
				if (component != null)
				{
					int num = 4;
					component.SetElementAmount(num);
					for (int i = 0; i < num; i++)
					{
						CUIListElementScript elemenet = component.GetElemenet(i);
						Transform transform = elemenet.transform.Find("Text");
						if (transform != null)
						{
							transform.GetComponent<Text>().text = CMallSortHelper.CreateHeroViewSorter().GetSortTypeName((CMallSortHelper.HeroViewSortType)i);
						}
					}
					component.SelectElement((int)this.m_heroSortType, true);
				}
				widget.CustomSetActive(false);
			}
			GameObject widget2 = form.GetWidget(1);
			if (widget2 != null)
			{
				widget2.GetComponent<Text>().text = CMallSortHelper.CreateHeroViewSorter().GetSortTypeName(this.m_heroSortType);
			}
		}
	}
}

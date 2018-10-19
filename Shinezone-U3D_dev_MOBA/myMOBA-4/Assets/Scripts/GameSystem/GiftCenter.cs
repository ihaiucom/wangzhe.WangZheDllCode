using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class GiftCenter
	{
		public enum Widgets
		{
			MenuList,
			SubMenuList,
			ItemList,
			FakeLoading,
			SortTitleText,
			SortList
		}

		public enum MenuPage
		{
			Hero,
			Skin
		}

		private CUIFormScript _giftCenterForm;

		private CUIListScript _itemList;

		private CUIListScript _menuList;

		private CUIListScript _subMenuList;

		private GameObject _fakeLoading;

		private CUIListScript _sortList;

		private GameObject _sortTitle;

		private ulong _curFriendUid;

		private uint _curWorldId;

		private bool _curFriendIsSns;

		private GiftCenter.MenuPage _curMenuPage;

		private enHeroJobType _curJobView;

		private CMallSortHelper.SkinSortType _curSkinSortType;

		private CMallSortHelper.HeroSortType _curHeroSortType;

		private stPayInfoSet _payInfoTemp;

		private ListView<ResHeroCfgInfo> _heroList = new ListView<ResHeroCfgInfo>();

		private int _heroTotalNum;

		private ListView<ResHeroSkin> _skinList = new ListView<ResHeroSkin>();

		private int _skinTotalNum;

		private List<int> _filterTempList = new List<int>();

		private int _tempListNum;

		private enHeroJobType _curFilterJob;

		private GiftCenter.MenuPage _curFilterMenuPage = GiftCenter.MenuPage.Skin;

		private CMallSortHelper.SkinSortType _curFilterSkinSortType;

		private CMallSortHelper.HeroSortType _curFilterHeroSortType;

		public void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftCenter_Open, new CUIEventManager.OnUIEventHandler(this.OnOpenGiftCenter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftCenter_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseGiftCenter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftFadeInAnim_End, new CUIEventManager.OnUIEventHandler(this.OnFadeInAnimEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_OnGiftMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnMenuChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_OnGiftSubMenueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubMenuChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftEnable, new CUIEventManager.OnUIEventHandler(this.OnItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortListClicked));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftSortChange, new CUIEventManager.OnUIEventHandler(this.OnSortChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftShowDetail, new CUIEventManager.OnUIEventHandler(this.OnShowDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftGive, new CUIEventManager.OnUIEventHandler(this.OnGiveFriend));
		}

		public void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftCenter_Open, new CUIEventManager.OnUIEventHandler(this.OnOpenGiftCenter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftCenter_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseGiftCenter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftFadeInAnim_End, new CUIEventManager.OnUIEventHandler(this.OnFadeInAnimEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_OnGiftMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnMenuChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_OnGiftSubMenueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubMenuChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftEnable, new CUIEventManager.OnUIEventHandler(this.OnItemEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortListClicked));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftSortChange, new CUIEventManager.OnUIEventHandler(this.OnSortChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftShowDetail, new CUIEventManager.OnUIEventHandler(this.OnShowDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftGive, new CUIEventManager.OnUIEventHandler(this.OnGiveFriend));
		}

		private void OnOpenGiftCenter(CUIEvent uiEvent)
		{
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= 5u)
			{
				Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(uiEvent.m_srcWidget, enNewFlagKey.New_Skin_Gift_V15, true);
			}
			this.OpenGiftCenter(0uL, 0u, false);
		}

		public void OpenGiftCenter(ulong uId = 0uL, uint worldId = 0u, bool isSns = false)
		{
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel < 5u)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Gift_LevelLimit"), false, 1.5f, null, new object[0]);
				return;
			}
			if (this._giftCenterForm == null)
			{
				this._curFriendUid = uId;
				this._curWorldId = worldId;
				this._curFriendIsSns = isSns;
				this._giftCenterForm = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/System/", "Mall/Form_GiftCenter.prefab"), false, true);
				if (this._giftCenterForm != null)
				{
					this.InitAllWidgets();
					this.PrepareData();
					this.InitMenuAndSubMenu();
				}
			}
		}

		private void OnCloseGiftCenter(CUIEvent uiEvent)
		{
			if (this._giftCenterForm != null)
			{
				this.UnInitAllWidgets();
				this._giftCenterForm = null;
				this.DataClearWhenClose();
			}
		}

		private void OnFadeInAnimEnd(CUIEvent uiEvent)
		{
			this.UpdateItemList();
			this._fakeLoading.CustomSetActive(false);
		}

		private void OnMenuChanged(CUIEvent uiEvent)
		{
			if (this._menuList != null)
			{
				int selectedIndex = this._menuList.GetSelectedIndex();
				this._curMenuPage = (GiftCenter.MenuPage)selectedIndex;
				this._curSkinSortType = CMallSortHelper.SkinSortType.Default;
				this._curHeroSortType = CMallSortHelper.HeroSortType.Default;
				this.SetSortContent();
				if (this._curJobView != enHeroJobType.All)
				{
					this.RealIndexGetReady(false);
				}
				this.UpdateItemList();
			}
		}

		private void OnSubMenuChanged(CUIEvent uiEvent)
		{
			if (this._subMenuList != null)
			{
				int selectedIndex = this._subMenuList.GetSelectedIndex();
				this._curJobView = (enHeroJobType)selectedIndex;
				if (this._curJobView != enHeroJobType.All)
				{
					this.RealIndexGetReady(false);
				}
				this.UpdateItemList();
			}
		}

		private void InitAllWidgets()
		{
			this._itemList = this._giftCenterForm.m_formWidgets[2].GetComponent<CUIListScript>();
			this._menuList = this._giftCenterForm.m_formWidgets[0].GetComponent<CUIListScript>();
			this._subMenuList = this._giftCenterForm.m_formWidgets[1].GetComponent<CUIListScript>();
			this._fakeLoading = this._giftCenterForm.m_formWidgets[3];
			this._fakeLoading.CustomSetActive(false);
			this._sortList = this._giftCenterForm.m_formWidgets[5].GetComponent<CUIListScript>();
			this._sortTitle = this._giftCenterForm.m_formWidgets[4];
		}

		private void UnInitAllWidgets()
		{
			this._itemList = null;
			this._menuList = null;
			this._subMenuList = null;
			this._fakeLoading = null;
			this._sortList = null;
			this._sortTitle = null;
		}

		private void InitMenuAndSubMenu()
		{
			if (this._heroTotalNum > 0 && this._skinTotalNum > 0)
			{
				this._menuList.SetElementAmount(2);
				this._curMenuPage = GiftCenter.MenuPage.Hero;
				CUIListElementScript elemenet = this._menuList.GetElemenet(1);
				if (elemenet != null)
				{
					elemenet.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
				}
				elemenet = this._menuList.GetElemenet(0);
				if (elemenet != null)
				{
					elemenet.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
				}
			}
			else if (this._heroTotalNum > 0)
			{
				this._menuList.SetElementAmount(1);
				this._curMenuPage = GiftCenter.MenuPage.Hero;
				CUIListElementScript elemenet2 = this._menuList.GetElemenet(0);
				if (elemenet2 != null)
				{
					elemenet2.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
				}
			}
			else
			{
				this._menuList.SetElementAmount(1);
				this._curMenuPage = GiftCenter.MenuPage.Skin;
				CUIListElementScript elemenet3 = this._menuList.GetElemenet(0);
				if (elemenet3 != null)
				{
					elemenet3.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
				}
			}
			this._menuList.SelectElement(0, false);
			this._subMenuList.SetElementAmount(7);
			for (int i = 0; i < 7; i++)
			{
				CUIListElementScript elemenet4 = this._subMenuList.GetElemenet(i);
				if (elemenet4 != null)
				{
					elemenet4.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = this.GetJobName((enHeroJobType)i);
				}
			}
			this._subMenuList.SelectElement(0, false);
			this._curJobView = enHeroJobType.All;
			this.SetSortContent();
		}

		private void SetSortContent()
		{
			GiftCenter.MenuPage curMenuPage = this._curMenuPage;
			if (curMenuPage != GiftCenter.MenuPage.Hero)
			{
				if (curMenuPage == GiftCenter.MenuPage.Skin)
				{
					int elementAmount = 5;
					this._sortList.SetElementAmount(elementAmount);
					for (int i = 0; i < 5; i++)
					{
						CUIListElementScript elemenet = this._sortList.GetElemenet(i);
						Transform transform = elemenet.transform.Find("Text");
						Text component = transform.GetComponent<Text>();
						if (component != null)
						{
							component.text = CMallSortHelper.CreateSkinSorter().GetSortTypeName((CMallSortHelper.SkinSortType)i);
						}
					}
				}
			}
			else
			{
				int elementAmount = 4;
				this._sortList.SetElementAmount(elementAmount);
				int num = 0;
				for (int j = 0; j < 5; j++)
				{
					if (j != 3)
					{
						CUIListElementScript elemenet2 = this._sortList.GetElemenet(num);
						Transform transform2 = elemenet2.transform.Find("Text");
						Text component2 = transform2.GetComponent<Text>();
						if (component2 != null)
						{
							component2.text = CMallSortHelper.CreateHeroSorter().GetSortTypeName((CMallSortHelper.HeroSortType)j);
						}
						num++;
					}
				}
			}
			this._sortList.m_alwaysDispatchSelectedChangeEvent = true;
			this.SetSelectedElementInSortList();
			this.SetCurSortTitleName();
		}

		private void SetCurSortType()
		{
			if (this._sortList.GetSelectedIndex() == 3)
			{
				this._curHeroSortType = CMallSortHelper.HeroSortType.ReleaseTime;
				this._curSkinSortType = CMallSortHelper.SkinSortType.ReleaseTime;
			}
			else
			{
				this._curHeroSortType = (CMallSortHelper.HeroSortType)this._sortList.GetSelectedIndex();
				this._curSkinSortType = (CMallSortHelper.SkinSortType)this._sortList.GetSelectedIndex();
			}
		}

		private void SetSelectedElementInSortList()
		{
			if (this._curMenuPage == GiftCenter.MenuPage.Hero && this._curHeroSortType == CMallSortHelper.HeroSortType.ReleaseTime)
			{
				this._sortList.SelectElement(3, true);
			}
			else if (this._curMenuPage == GiftCenter.MenuPage.Hero)
			{
				this._sortList.SelectElement((int)this._curHeroSortType, true);
			}
			else
			{
				this._sortList.SelectElement((int)this._curSkinSortType, true);
			}
		}

		private void SetCurSortTitleName()
		{
			if (this._curMenuPage == GiftCenter.MenuPage.Hero)
			{
				this._sortTitle.GetComponent<Text>().text = CMallSortHelper.CreateHeroSorter().GetSortTypeName(this._curHeroSortType);
			}
			else
			{
				this._sortTitle.GetComponent<Text>().text = CMallSortHelper.CreateSkinSorter().GetSortTypeName(this._curSkinSortType);
			}
		}

		private string GetJobName(enHeroJobType job)
		{
			string result = null;
			switch (job)
			{
			case enHeroJobType.All:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
				break;
			case enHeroJobType.Tank:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
				break;
			case enHeroJobType.Soldier:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
				break;
			case enHeroJobType.Assassin:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
				break;
			case enHeroJobType.Master:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
				break;
			case enHeroJobType.Archer:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
				break;
			case enHeroJobType.Aid:
				result = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
				break;
			}
			return result;
		}

		private void UpdateItemList()
		{
			if (this._itemList == null)
			{
				return;
			}
			if (this._curMenuPage == GiftCenter.MenuPage.Skin)
			{
				this._itemList.SetElementAmount((this._curJobView != enHeroJobType.All) ? this._tempListNum : this._skinTotalNum);
			}
			else
			{
				this._itemList.SetElementAmount((this._curJobView != enHeroJobType.All) ? this._tempListNum : this._heroTotalNum);
			}
		}

		private void OnItemEnable(CUIEvent uiEvent)
		{
			int num = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (this._curJobView != enHeroJobType.All && num < this._filterTempList.Count)
			{
				num = this._filterTempList[num];
			}
			if (this._curMenuPage == GiftCenter.MenuPage.Skin && num < this._skinList.Count)
			{
				ResHeroSkin skinInfo = this._skinList[num];
				CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
				this.SetSkinItem(component, skinInfo, uiEvent.m_srcFormScript);
			}
			else if (num < this._heroList.Count)
			{
				ResHeroCfgInfo heroInfo = this._heroList[num];
				CMallItemWidget component2 = srcWidget.GetComponent<CMallItemWidget>();
				this.SetHeroItem(component2, heroInfo, uiEvent.m_srcFormScript);
			}
		}

		private void OnSortChanged(CUIEvent uiEvent)
		{
			if (this._sortList != null)
			{
				int selectedIndex = this._sortList.GetSelectedIndex();
				this.SetCurSortType();
				this._sortList.gameObject.CustomSetActive(false);
				this.SetCurSortTitleName();
				this.RealIndexGetReady(true);
				this.UpdateItemList();
			}
		}

		private void OnShowGiveFriendSkin(uint heroId, uint skinId, uint price)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin == null)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/System/", "Mall/Form_GiveHeroSkin_3D.prefab"), false, true);
			Text component = cUIFormScript.transform.Find("Panel/skinNameText").GetComponent<Text>();
			component.text = heroSkin.szSkinName;
			GameObject gameObject = cUIFormScript.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
			CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
			CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
			Text component2 = cUIFormScript.transform.Find("Panel/pricePanel/priceText").GetComponent<Text>();
			component2.text = price.ToString();
			CUIEventScript component3 = cUIFormScript.transform.Find("Panel/BtnGroup/buyButton").gameObject.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.heroSkinParam.heroId = heroId;
			eventParams.heroSkinParam.skinId = skinId;
			eventParams.heroSkinParam.isCanCharge = true;
			eventParams.commonUInt64Param1 = this._curFriendUid;
			eventParams.commonBool = this._curFriendIsSns;
			eventParams.commonUInt32Param1 = this._curWorldId;
			component3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, eventParams);
			CUI3DImageScript component4 = cUIFormScript.transform.Find("Panel/3DImage").gameObject.GetComponent<CUI3DImageScript>();
			ObjNameData heroPrefabPath = CUICommonSystem.GetHeroPrefabPath(heroId, (int)skinId, true);
			GameObject gameObject2 = component4.AddGameObject(heroPrefabPath.ObjectName, false, false);
			if (gameObject2 != null)
			{
				if (heroPrefabPath.ActorInfo != null)
				{
					gameObject2.transform.localScale = new Vector3(heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale);
				}
				DynamicShadow.EnableDynamicShow(component4.gameObject, true);
				CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
				instance.Set3DModel(gameObject2);
				instance.InitAnimatList();
				instance.InitAnimatSoundList(heroId, skinId);
				instance.OnModePlayAnima("Come");
			}
		}

		private void OnShowDetail(CUIEvent euiEvent)
		{
			uint heroId = euiEvent.m_eventParams.openHeroFormPar.heroId;
			uint skinId = euiEvent.m_eventParams.openHeroFormPar.skinId;
			uint commonUInt32Param = euiEvent.m_eventParams.commonUInt32Param1;
			this.OnShowGiveFriendSkin(heroId, skinId, commonUInt32Param);
		}

		private void OnGiveFriend(CUIEvent euiEvent)
		{
		}

		private void OnSortListClicked(CUIEvent euiEvent)
		{
			this._sortList.gameObject.CustomSetActive(!this._sortList.gameObject.activeSelf);
		}

		private void SetHeroItem(CMallItemWidget mallWidget, ResHeroCfgInfo heroInfo, CUIFormScript form)
		{
			Image component = mallWidget.m_icon.GetComponent<Image>();
			component.color = CUIUtility.s_Color_White;
			string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, heroInfo.szImagePath);
			component.SetSprite(prefabPath, form, false, true, true, true);
			mallWidget.m_skinLabel.CustomSetActive(false);
			mallWidget.m_topNameLeftText.GetComponent<Text>().text = heroInfo.szName;
			mallWidget.m_topNameRightText.CustomSetActive(false);
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroInfo.dwCfgID);
			if (heroData != null)
			{
				ResHeroPromotion resHeroPromotion = heroData.promotion();
				this._payInfoTemp = CMallSystem.GetPayInfoSetOfGood(heroInfo, resHeroPromotion);
				uint num = this.SetItemPriceInfo(mallWidget, ref this._payInfoTemp);
				this.SetItemTag(mallWidget, resHeroPromotion, null, form);
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.heroId = heroInfo.dwCfgID;
				eventParams.commonUInt64Param1 = this._curFriendUid;
				eventParams.commonBool = this._curFriendIsSns;
				eventParams.commonUInt32Param1 = this._curWorldId;
				mallWidget.m_buyBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_OpenBuyHeroForFriend, eventParams);
				mallWidget.m_item.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.None, eventParams);
				CMallItem cMallItem = new CMallItem(heroData.cfgID, CMallItem.IconType.Normal);
				Text componetInChild = Utility.GetComponetInChild<Text>(mallWidget.m_askForBtn, "Text");
				Button component2 = mallWidget.m_askForBtn.GetComponent<Button>();
				CUIEventScript component3 = mallWidget.m_askForBtn.GetComponent<CUIEventScript>();
				if (!cMallItem.CanBeAskFor())
				{
					if (cMallItem.Owned(false) && componetInChild != null)
					{
						componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
					}
					if (component2 != null)
					{
						component2.enabled = false;
					}
					if (component3 != null)
					{
						component3.enabled = false;
					}
				}
				else
				{
					if (componetInChild != null)
					{
						componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Ask_For_Friend_Op");
					}
					if (component2 != null)
					{
						component2.enabled = true;
					}
					if (component3 != null)
					{
						component3.enabled = true;
					}
					mallWidget.m_askForBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_OpenAskForFriend, eventParams);
				}
			}
		}

		private void SetSkinItem(CMallItemWidget mallWidget, ResHeroSkin skinInfo, CUIFormScript form)
		{
			Image component = mallWidget.m_icon.GetComponent<Image>();
			component.color = CUIUtility.s_Color_White;
			string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, skinInfo.szSkinPicID);
			component.SetSprite(prefabPath, form, false, true, true, true);
			mallWidget.m_skinLabel.CustomSetActive(true);
			CUICommonSystem.SetHeroSkinLabelPic(form, mallWidget.m_skinLabel, skinInfo.dwHeroID, skinInfo.dwSkinID);
			mallWidget.m_topNameLeftText.GetComponent<Text>().text = skinInfo.szHeroName;
			mallWidget.m_topNameRightText.CustomSetActive(true);
			mallWidget.m_topNameRightText.GetComponent<Text>().text = skinInfo.szSkinName;
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(skinInfo.dwHeroID, skinInfo.dwSkinID);
			this._payInfoTemp = CMallSystem.GetPayInfoSetOfGood(skinInfo, skinPromotion);
			uint commonUInt32Param = this.SetItemPriceInfo(mallWidget, ref this._payInfoTemp);
			this.SetItemTag(mallWidget, null, skinPromotion, form);
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = skinInfo.dwHeroID;
			eventParams.openHeroFormPar.skinId = skinInfo.dwSkinID;
			eventParams.commonUInt32Param1 = commonUInt32Param;
			mallWidget.m_item.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mall_GiftShowDetail, eventParams);
			stUIEventParams eventParams2 = default(stUIEventParams);
			eventParams2.heroSkinParam.heroId = skinInfo.dwHeroID;
			eventParams2.heroSkinParam.skinId = skinInfo.dwSkinID;
			eventParams2.heroSkinParam.isCanCharge = true;
			eventParams2.commonUInt64Param1 = this._curFriendUid;
			eventParams2.commonBool = this._curFriendIsSns;
			eventParams2.commonUInt32Param1 = this._curWorldId;
			mallWidget.m_buyBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, eventParams2);
			CMallItem cMallItem = new CMallItem(skinInfo.dwHeroID, skinInfo.dwSkinID, CMallItem.IconType.Normal);
			Text componetInChild = Utility.GetComponetInChild<Text>(mallWidget.m_askForBtn, "Text");
			Button component2 = mallWidget.m_askForBtn.GetComponent<Button>();
			CUIEventScript component3 = mallWidget.m_askForBtn.GetComponent<CUIEventScript>();
			if (!cMallItem.CanBeAskFor())
			{
				if (cMallItem.Owned(false) && componetInChild != null)
				{
					componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own");
				}
				if (component2 != null)
				{
					component2.enabled = false;
				}
				if (component3 != null)
				{
					component3.enabled = false;
				}
			}
			else
			{
				if (componetInChild != null)
				{
					componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Ask_For_Friend_Op");
				}
				if (component2 != null)
				{
					component2.enabled = true;
				}
				if (component3 != null)
				{
					component3.enabled = true;
				}
				mallWidget.m_askForBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenAskForFriend, eventParams2);
			}
		}

		public CMallItem.OldPriceType GetOldPriceType()
		{
			switch (this._payInfoTemp.m_payInfoCount)
			{
			case 0:
				return CMallItem.OldPriceType.None;
			case 1:
				if (this._payInfoTemp.m_payInfos[0].m_oriValue != this._payInfoTemp.m_payInfos[0].m_payValue)
				{
					return CMallItem.OldPriceType.FirstOne;
				}
				return CMallItem.OldPriceType.None;
			case 2:
				if (this._payInfoTemp.m_payInfos[0].m_oriValue != this._payInfoTemp.m_payInfos[0].m_payValue && this._payInfoTemp.m_payInfos[1].m_oriValue != this._payInfoTemp.m_payInfos[1].m_payValue)
				{
					return CMallItem.OldPriceType.Both;
				}
				if (this._payInfoTemp.m_payInfos[0].m_oriValue != this._payInfoTemp.m_payInfos[0].m_payValue)
				{
					return CMallItem.OldPriceType.FirstOne;
				}
				if (this._payInfoTemp.m_payInfos[1].m_oriValue != this._payInfoTemp.m_payInfos[1].m_payValue)
				{
					return CMallItem.OldPriceType.SecondOne;
				}
				return CMallItem.OldPriceType.None;
			default:
				return CMallItem.OldPriceType.None;
			}
		}

		public uint SetItemPriceInfo(CMallItemWidget itemWidget, ref stPayInfoSet payInfoSet)
		{
			uint result = 0u;
			if (itemWidget.m_priceContainer == null)
			{
				return result;
			}
			itemWidget.m_priceContainer.SetActive(true);
			CMallItem.OldPriceType oldPriceType = this.GetOldPriceType();
			CUIListScript component = itemWidget.m_priceContainer.GetComponent<CUIListScript>();
			component.SetElementAmount(1);
			itemWidget.m_orTextContainer.CustomSetActive(false);
			CUIListElementScript elemenet = component.GetElemenet(0);
			if (elemenet == null)
			{
				return result;
			}
			GameObject widget = elemenet.GetWidget(0);
			GameObject widget2 = elemenet.GetWidget(1);
			GameObject widget3 = elemenet.GetWidget(2);
			GameObject widget4 = elemenet.GetWidget(4);
			GameObject widget5 = elemenet.GetWidget(3);
			GameObject widget6 = elemenet.GetWidget(5);
			if (widget == null || widget2 == null || widget3 == null || widget4 == null || widget5 == null || widget6 == null)
			{
				return result;
			}
			for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
			{
				if (payInfoSet.m_payInfos[i].m_payType == enPayType.Diamond || payInfoSet.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)
				{
					payInfoSet.m_payInfos[i].m_payType = enPayType.DianQuan;
				}
			}
			for (int j = 0; j < payInfoSet.m_payInfoCount; j++)
			{
				if (payInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan || payInfoSet.m_payInfos[j].m_payType == enPayType.Diamond || payInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan)
				{
					result = payInfoSet.m_payInfos[j].m_payValue;
					switch (oldPriceType)
					{
					case CMallItem.OldPriceType.None:
					{
						itemWidget.m_middleOrText.CustomSetActive(true);
						itemWidget.m_bottomOrText.CustomSetActive(false);
						widget.SetActive(false);
						widget2.SetActive(false);
						widget3.SetActive(false);
						widget5.SetActive(true);
						Text component2 = widget5.GetComponent<Text>();
						component2.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
						Image component3 = widget6.GetComponent<Image>();
						component3.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						break;
					}
					case CMallItem.OldPriceType.FirstOne:
						itemWidget.m_middleOrText.CustomSetActive(false);
						itemWidget.m_bottomOrText.CustomSetActive(true);
						if (j == 0)
						{
							widget2.SetActive(false);
							widget5.SetActive(false);
							widget.SetActive(true);
							widget3.SetActive(true);
							Text component4 = widget.GetComponent<Text>();
							component4.text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
							Text component5 = widget3.GetComponent<Text>();
							component5.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
							Image component6 = widget4.GetComponent<Image>();
							component6.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						}
						else
						{
							widget2.SetActive(false);
							widget.SetActive(false);
							widget5.SetActive(false);
							widget3.SetActive(true);
							Text component7 = widget3.GetComponent<Text>();
							component7.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
							Image component8 = widget4.GetComponent<Image>();
							component8.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						}
						break;
					case CMallItem.OldPriceType.SecondOne:
						itemWidget.m_middleOrText.CustomSetActive(false);
						itemWidget.m_bottomOrText.CustomSetActive(true);
						if (j == 1)
						{
							widget2.SetActive(false);
							widget5.SetActive(false);
							widget.SetActive(true);
							widget3.SetActive(true);
							Text component9 = widget.GetComponent<Text>();
							component9.text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
							Text component10 = widget3.GetComponent<Text>();
							component10.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
							Image component11 = widget4.GetComponent<Image>();
							component11.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						}
						else
						{
							widget2.SetActive(false);
							widget.SetActive(false);
							widget5.SetActive(false);
							widget3.SetActive(true);
							Text component12 = widget3.GetComponent<Text>();
							component12.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
							Image component13 = widget4.GetComponent<Image>();
							component13.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						}
						break;
					case CMallItem.OldPriceType.Both:
					{
						itemWidget.m_middleOrText.CustomSetActive(true);
						itemWidget.m_bottomOrText.CustomSetActive(false);
						widget2.SetActive(false);
						widget5.SetActive(false);
						widget.SetActive(true);
						widget3.SetActive(true);
						Text component14 = widget.GetComponent<Text>();
						component14.text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
						Text component15 = widget3.GetComponent<Text>();
						component15.text = payInfoSet.m_payInfos[j].m_payValue.ToString();
						Image component16 = widget4.GetComponent<Image>();
						component16.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false, false);
						break;
					}
					}
					break;
				}
			}
			return result;
		}

		private void SetItemTag(CMallItemWidget itemWidget, ResHeroPromotion heroPromotion, ResSkinPromotion skinPromotion, CUIFormScript form)
		{
			string text = null;
			string text2 = null;
			RES_LUCKYDRAW_ITEMTAG rES_LUCKYDRAW_ITEMTAG = RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
			if (heroPromotion != null)
			{
				rES_LUCKYDRAW_ITEMTAG = (RES_LUCKYDRAW_ITEMTAG)heroPromotion.bTag;
			}
			else if (skinPromotion != null)
			{
				rES_LUCKYDRAW_ITEMTAG = (RES_LUCKYDRAW_ITEMTAG)skinPromotion.bTag;
			}
			switch (rES_LUCKYDRAW_ITEMTAG)
			{
			case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
			{
				int num = 0;
				uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
				if (heroPromotion != null)
				{
					if (heroPromotion.dwOnTimeGen > currentUTCTime)
					{
						num = (int)(heroPromotion.dwOffTimeGen - heroPromotion.dwOnTimeGen);
					}
					else
					{
						num = (int)(heroPromotion.dwOffTimeGen - currentUTCTime);
					}
				}
				else if (skinPromotion != null)
				{
					if (skinPromotion.dwOnTimeGen > currentUTCTime)
					{
						num = (int)(skinPromotion.dwOffTimeGen - skinPromotion.dwOnTimeGen);
					}
					else
					{
						num = (int)(skinPromotion.dwOffTimeGen - currentUTCTime);
					}
				}
				if (num > 0)
				{
					int num2 = (int)Math.Ceiling((double)num / 86400.0);
					if (num2 > 0)
					{
						text = "UGUI/Sprite/Common/Product_Unusual.prefab";
						text2 = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", new string[]
						{
							num2.ToString()
						});
					}
				}
				break;
			}
			case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
				text = "UGUI/Sprite/Common/Product_New.prefab";
				text2 = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
				break;
			case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
				text = "UGUI/Sprite/Common/Product_Hot.prefab";
				text2 = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
				break;
			case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
			{
				float num3 = 100f;
				if (heroPromotion != null)
				{
					num3 = heroPromotion.dwDiscount / 10f;
				}
				else if (skinPromotion != null)
				{
					num3 = skinPromotion.dwDiscount / 10f;
				}
				text = "UGUI/Sprite/Common/Product_Discount.prefab";
				if (Math.Abs(num3 % 1f) < 1.401298E-45f)
				{
					text2 = string.Format("{0}折", ((int)num3).ToString("D"));
				}
				else
				{
					text2 = string.Format("{0}折", num3.ToString("0.0"));
				}
				break;
			}
			}
			if (itemWidget.m_tagContainer != null && (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2)))
			{
				itemWidget.m_tagContainer.SetActive(true);
				Image component = itemWidget.m_tagContainer.GetComponent<Image>();
				component.SetSprite(text, form, false, true, true, false);
				if (itemWidget.m_tagText != null)
				{
					Text component2 = itemWidget.m_tagText.GetComponent<Text>();
					component2.text = text2;
				}
			}
			else
			{
				itemWidget.m_tagContainer.CustomSetActive(false);
			}
		}

		private void PrepareData()
		{
			GameDataMgr.GetAllHeroList(ref this._heroList, out this._heroTotalNum, enHeroJobType.All, true, true);
			GameDataMgr.GetAllSkinList(ref this._skinList, out this._skinTotalNum, enHeroJobType.All, false, true, true);
		}

		private void CleanSortSkinList()
		{
			CMallSortHelper.CreateSkinSorter().SetSortType(this._curSkinSortType);
			if (this._skinList.Count > this._skinTotalNum)
			{
				this._skinList.RemoveRange(this._skinTotalNum + 1, this._skinList.Count - this._skinTotalNum);
			}
			this._skinList.Sort(CMallSortHelper.CreateSkinSorter());
			if (CMallSortHelper.CreateSkinSorter().IsDesc())
			{
				this._skinList.Reverse();
			}
		}

		private void CleanSortHeroList()
		{
			CMallSortHelper.CreateHeroSorter().SetSortType(this._curHeroSortType);
			if (this._heroList.Count > this._heroTotalNum)
			{
				this._heroList.RemoveRange(this._heroTotalNum + 1, this._heroList.Count - this._heroTotalNum);
			}
			this._heroList.Sort(CMallSortHelper.CreateHeroSorter());
			if (CMallSortHelper.CreateHeroSorter().IsDesc())
			{
				this._heroList.Reverse();
			}
		}

		private void RealIndexGetReady(bool forceUpdate = false)
		{
			if (!forceUpdate && this._curFilterJob == this._curJobView && this._curFilterMenuPage == this._curMenuPage && ((this._curFilterMenuPage == GiftCenter.MenuPage.Hero && this._curFilterHeroSortType == this._curHeroSortType) || (this._curFilterMenuPage == GiftCenter.MenuPage.Skin && this._curFilterSkinSortType == this._curSkinSortType)))
			{
				return;
			}
			this._curFilterJob = this._curJobView;
			this._curFilterMenuPage = this._curMenuPage;
			int num = 0;
			if (this._curMenuPage == GiftCenter.MenuPage.Skin)
			{
				this._curFilterSkinSortType = this._curSkinSortType;
				this.CleanSortSkinList();
				for (int i = 0; i < this._skinTotalNum; i++)
				{
					ResHeroSkin resHeroSkin = this._skinList[i];
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resHeroSkin.dwHeroID);
					if (dataByKey.bMainJob == (byte)this._curJobView || dataByKey.bMinorJob == (byte)this._curJobView)
					{
						if (num < this._filterTempList.Count)
						{
							this._filterTempList[num] = i;
						}
						else
						{
							this._filterTempList.Add(i);
						}
						num++;
					}
				}
				this._tempListNum = num;
			}
			else
			{
				this._curFilterHeroSortType = this._curHeroSortType;
				this.CleanSortHeroList();
				for (int j = 0; j < this._heroTotalNum; j++)
				{
					ResHeroCfgInfo resHeroCfgInfo = this._heroList[j];
					if (resHeroCfgInfo.bMainJob == (byte)this._curJobView || resHeroCfgInfo.bMinorJob == (byte)this._curJobView)
					{
						if (num < this._filterTempList.Count)
						{
							this._filterTempList[num] = j;
						}
						else
						{
							this._filterTempList.Add(j);
						}
						num++;
					}
				}
				this._tempListNum = num;
			}
		}

		private void DataClearWhenClose()
		{
			this._filterTempList.Clear();
			this._tempListNum = 0;
		}
	}
}

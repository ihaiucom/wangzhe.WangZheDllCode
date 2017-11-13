using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

[MessageHandlerClass]
public class CMallBoutiqueController : Singleton<CMallBoutiqueController>
{
	private ListView<ResBoutiqueConf> m_NewArrivalListView;

	private ListView<ResBoutiqueConf> m_HotSaleListView;

	public override void Init()
	{
		base.Init();
		this.m_NewArrivalListView = new ListView<ResBoutiqueConf>();
		this.m_HotSaleListView = new ListView<ResBoutiqueConf>();
	}

	public override void UnInit()
	{
		base.UnInit();
		this.m_NewArrivalListView = null;
		this.m_HotSaleListView = null;
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
	}

	public void Load(CUIFormScript form)
	{
		CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Boutique", "pnlBoutique", form.GetWidget(3), form);
	}

	public bool Loaded(CUIFormScript form)
	{
		GameObject x = Utility.FindChild(form.GetWidget(3), "pnlBoutique");
		return !(x == null);
	}

	public void Draw(CUIFormScript form)
	{
		this.InitElements();
		if (!this.RefreshData())
		{
			Singleton<CUIManager>.GetInstance().OpenTips("Initializing data failed", false, 1.5f, null, new object[0]);
			Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			return;
		}
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
		Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
		Singleton<EventRouter>.GetInstance().AddEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
		Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
		Singleton<EventRouter>.GetInstance().AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
		this.InitBannerCtrl();
		this.RefreshNewArrivals();
		this.RefreshHotSale();
	}

	private bool RefreshData()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		DebugHelper.Assert(masterRoleInfo != null, "Owned::Master Role Info Is Null");
		this.m_HotSaleListView.Clear();
		this.m_NewArrivalListView.Clear();
		DictionaryView<uint, ResBoutiqueConf>.Enumerator enumerator = GameDataMgr.boutiqueDict.GetEnumerator();
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		while (enumerator.MoveNext())
		{
			KeyValuePair<uint, ResBoutiqueConf> current = enumerator.Current;
			ResBoutiqueConf value = current.get_Value();
			if ((ulong)value.dwOnTimeGen <= (ulong)((long)currentUTCTime) && (ulong)value.dwOffTimeGen >= (ulong)((long)currentUTCTime))
			{
				RES_BOUTIQUE_TYPE bBoutiqueType = (RES_BOUTIQUE_TYPE)value.bBoutiqueType;
				ListView<ResBoutiqueConf> listView = null;
				RES_BOUTIQUE_TYPE rES_BOUTIQUE_TYPE = bBoutiqueType;
				if (rES_BOUTIQUE_TYPE != RES_BOUTIQUE_TYPE.RES_BOUTIQUE_TYPE_NEW_ARRIVAL)
				{
					if (rES_BOUTIQUE_TYPE == RES_BOUTIQUE_TYPE.RES_BOUTIQUE_TYPE_HOT_SALE)
					{
						listView = this.m_HotSaleListView;
					}
				}
				else
				{
					listView = this.m_NewArrivalListView;
				}
				switch (value.wItemType)
				{
				case 2:
				case 5:
				{
					CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(value.dwItemID);
					if (product != null && product.IsOnSale == 1 && listView != null)
					{
						listView.Add(value);
					}
					break;
				}
				case 4:
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(value.dwItemID);
					if (dataByKey != null && GameDataMgr.IsHeroAvailable(dataByKey.dwCfgID) && listView != null)
					{
						listView.Add(value);
					}
					break;
				}
				case 7:
				{
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(value.dwItemID);
					if (heroSkin != null)
					{
						ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
						if (dataByKey2 != null)
						{
							bool flag = true;
							if (masterRoleInfo != null)
							{
								flag = masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false);
							}
							if (GameDataMgr.IsSkinAvailable(heroSkin.dwID) && !flag && GameDataMgr.IsHeroAvailable(dataByKey2.dwCfgID) && listView != null)
							{
								listView.Add(value);
							}
						}
					}
					break;
				}
				}
			}
		}
		return true;
	}

	public void InitElements()
	{
		CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
		if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
		{
			return;
		}
		Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlBoutique").CustomSetActive(true);
	}

	public void InitBannerCtrl()
	{
		CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
		if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
		{
			return;
		}
		GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Left/Top/StepList");
		if (gameObject == null)
		{
			DebugHelper.Assert(false, "banner img object is null");
			return;
		}
		BannerImageCtrl component = gameObject.GetComponent<BannerImageCtrl>();
		if (component == null)
		{
			DebugHelper.Assert(false, "banner img ctrl is null");
			return;
		}
		component.InitSys();
	}

	public void RefreshNewArrivals()
	{
		CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
		if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
		{
			return;
		}
		CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Left/Bottom/NewArrivals");
		componetInChild.SetElementAmount(this.m_NewArrivalListView.Count);
	}

	public void RefreshHotSale()
	{
		CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
		if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
		{
			return;
		}
		CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Right/Hot");
		componetInChild.SetElementAmount(this.m_HotSaleListView.Count);
	}

	private void OnNewArrivalEnable(CUIEvent uiEvent)
	{
		int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_NewArrivalListView.Count)
		{
			return;
		}
		GameObject srcWidget = uiEvent.m_srcWidget;
		if (srcWidget == null)
		{
			return;
		}
		CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
		if (component == null)
		{
			return;
		}
		ResBoutiqueConf resBoutiqueConf = this.m_NewArrivalListView[srcWidgetIndexInBelongedList];
		DebugHelper.Assert(resBoutiqueConf != null, "new arrival cfg is null");
		if (resBoutiqueConf == null)
		{
			return;
		}
		switch (resBoutiqueConf.wItemType)
		{
		case 4:
		{
			CMallItem item = new CMallItem(resBoutiqueConf.dwItemID, CMallItem.IconType.Normal);
			Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
			break;
		}
		case 7:
		{
			uint heroID = 0u;
			uint skinID = 0u;
			CSkinInfo.ResolveHeroSkin(resBoutiqueConf.dwItemID, out heroID, out skinID);
			CMallItem item2 = new CMallItem(heroID, skinID, CMallItem.IconType.Normal);
			Singleton<CMallSystem>.GetInstance().SetMallItem(component, item2);
			break;
		}
		}
	}

	private void OnHotSaleEnable(CUIEvent uiEvent)
	{
		int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_HotSaleListView.Count)
		{
			return;
		}
		GameObject srcWidget = uiEvent.m_srcWidget;
		if (srcWidget == null)
		{
			return;
		}
		CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
		if (component == null)
		{
			return;
		}
		ResBoutiqueConf resBoutiqueConf = this.m_HotSaleListView[srcWidgetIndexInBelongedList];
		DebugHelper.Assert(resBoutiqueConf != null, "hot sale cfg is null");
		if (resBoutiqueConf == null)
		{
			return;
		}
		switch (resBoutiqueConf.wItemType)
		{
		case 2:
		{
			CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(resBoutiqueConf.dwItemID);
			CMallItem item = new CMallItem(product, CMallItem.IconType.Small);
			Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
			break;
		}
		case 4:
		{
			CMallItem item2 = new CMallItem(resBoutiqueConf.dwItemID, CMallItem.IconType.Small);
			Singleton<CMallSystem>.GetInstance().SetMallItem(component, item2);
			break;
		}
		case 7:
		{
			uint heroID = 0u;
			uint skinID = 0u;
			CSkinInfo.ResolveHeroSkin(resBoutiqueConf.dwItemID, out heroID, out skinID);
			CMallItem item3 = new CMallItem(heroID, skinID, CMallItem.IconType.Small);
			Singleton<CMallSystem>.GetInstance().SetMallItem(component, item3);
			break;
		}
		}
	}

	public void OnFactoryProductClick(CUIEvent uiEvent)
	{
		ListView<CMallFactoryShopController.ShopProduct> products = Singleton<CMallFactoryShopController>.GetInstance().GetProducts();
		if (products == null)
		{
			stUIEventParams par = default(stUIEventParams);
			par.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab, par);
			return;
		}
		if (uiEvent.m_eventParams.tag < 0 || uiEvent.m_eventParams.tag >= products.Count)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox("internal error: out of index range.", false);
			return;
		}
		CMallFactoryShopController.ShopProduct shopProduct = products[uiEvent.m_eventParams.tag];
		if (!shopProduct.CanBuy())
		{
			return;
		}
		if (shopProduct.Type == COM_ITEM_TYPE.COM_OBJTYPE_HERO || shopProduct.Type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy;
			cUIEvent.m_eventParams.commonUInt64Param1 = (ulong)shopProduct.Key;
			cUIEvent.m_eventParams.commonUInt32Param1 = 1u;
			Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(shopProduct, 1u, false, cUIEvent);
		}
		else
		{
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			CUseable useableByBaseID = useableContainer.GetUseableByBaseID(shopProduct.Type, shopProduct.ID);
			uint num = 0u;
			if (shopProduct.LimitCount > 0u)
			{
				num = shopProduct.LimitCount - shopProduct.BoughtCount;
			}
			if (useableByBaseID != null)
			{
				uint num2 = (uint)(useableByBaseID.m_stackMax - useableByBaseID.m_stackCount);
				if (num2 < num || num == 0u)
				{
					num = num2;
				}
			}
			CUIEvent cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy;
			cUIEvent2.m_eventParams.commonUInt64Param1 = (ulong)shopProduct.Key;
			BuyPickDialog.Show(shopProduct.Type, shopProduct.ID, shopProduct.CoinType, shopProduct.RealDiscount, num, new BuyPickDialog.OnConfirmBuyDelegate(Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct), shopProduct, null, cUIEvent2);
		}
	}

	public void OnFactoryProductConfirmBuy(CUIEvent uiEvent)
	{
		uint key = (uint)uiEvent.m_eventParams.commonUInt64Param1;
		uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
		CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
		if (product != null)
		{
			Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(product, commonUInt32Param);
		}
	}

	private void OnMallTabChange()
	{
		CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
		if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
		{
			return;
		}
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this.OnMallTabChange));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
	}

	private void OnFactoryProductBought(CMallFactoryShopController.ShopProduct shopProduct)
	{
		if (shopProduct.IsOnSale != 1)
		{
			this.RefreshData();
			this.RefreshHotSale();
		}
	}

	private void OnNtyAddSkin(uint heroId, uint skinId, uint addReason)
	{
		this.RefreshData();
		this.RefreshHotSale();
		this.RefreshNewArrivals();
	}

	private void OnNtyAddHero(uint id)
	{
		this.RefreshData();
		this.RefreshHotSale();
		this.RefreshNewArrivals();
	}
}

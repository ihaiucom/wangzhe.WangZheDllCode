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
	public class MySteryShop : Singleton<MySteryShop>
	{
		public COMDT_AKALISHOP_DETAIL m_AkaliShopDetail;

		public COMDT_ACNT_AKALISHOP_INFO m_AkaliShopInfoSvr;

		public byte m_bRequestDiscount;

		public byte m_bRequestBuyCount;

		public static string MYSTERY_ROLL_DISCOUNT_FORM_PATH = "UGUI/Form/System/Mall/Form_NewDiscount.prefab";

		public override void Init()
		{
			base.Init();
			this.m_AkaliShopDetail = null;
			this.InitEvent();
		}

		public override void UnInit()
		{
			this.m_AkaliShopDetail = null;
			this.UnInitEvent();
			this.m_bRequestDiscount = 0;
			this.m_bRequestBuyCount = 0;
		}

		private void InitEvent()
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyItem));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
			instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Default_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnDefaultItemClick));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnAddHeroSkin));
		}

		private void UnInitEvent()
		{
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyItem));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Default_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnDefaultItemClick));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnAddHeroSkin));
		}

		public bool IsShopAvailable()
		{
			return this.m_AkaliShopDetail != null && this.m_AkaliShopInfoSvr != null && this.GetTimeToClose() > 0;
		}

		public bool IsGetDisCount()
		{
			return this.m_AkaliShopInfoSvr != null && this.m_bRequestDiscount > 0;
		}

		public int GetDisCount()
		{
			if (this.m_AkaliShopDetail != null)
			{
				return this.m_AkaliShopDetail.iShowDiscount;
			}
			return -1;
		}

		public void ClearSvrData()
		{
			this.m_AkaliShopDetail = null;
			this.m_AkaliShopInfoSvr = null;
			this.m_bRequestDiscount = 0;
			this.m_bRequestBuyCount = 0;
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Mystery", "pnlMystery", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlMystery");
			return !(x == null);
		}

		public int GetTimeToClose()
		{
			if (this.m_AkaliShopDetail == null)
			{
				return 0;
			}
			TimeSpan timeSpan = Utility.ToUtcTime2Local((long)((ulong)this.m_AkaliShopDetail.dwEndTime)) - Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (timeSpan.TotalSeconds > 0.0)
			{
				return (timeSpan.TotalSeconds <= 2147483647.0) ? ((int)timeSpan.TotalSeconds) : 2147483647;
			}
			return 0;
		}

		private void RefreshTimer()
		{
			if (!this.IsShopAvailable())
			{
				return;
			}
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/TimeLeft/Timer");
			int timeToClose = this.GetTimeToClose();
			if (timeToClose > 86400)
			{
				componetInChild.m_timerDisplayType = enTimerDisplayType.D_H_M_S;
			}
			else
			{
				componetInChild.m_timerDisplayType = enTimerDisplayType.H_M_S;
			}
			componetInChild.SetTotalTime((float)timeToClose);
			componetInChild.StartTimer();
		}

		private void OnShopTimeEnd(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenTips("神秘商店已关闭", false, 1.5f, null, new object[0]);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_CloseForm);
		}

		public void OnNtyAddHero(uint id)
		{
			this.UpdateUI();
		}

		private void OnAddHeroSkin(uint heroId, uint skinId, uint addReason)
		{
			this.UpdateUI();
		}

		public void UpdateUI()
		{
			if (Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Mystery)
			{
				return;
			}
			if (this.m_AkaliShopDetail == null)
			{
				return;
			}
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			if (Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Mystery)
			{
				return;
			}
			Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery").CustomSetActive(true);
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/List");
			GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/default");
			if (!this.IsGetDisCount())
			{
				obj.CustomSetActive(false);
				obj2.CustomSetActive(true);
			}
			else
			{
				obj.CustomSetActive(true);
				obj2.CustomSetActive(false);
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/List");
				if (componetInChild != null)
				{
					componetInChild.SetElementAmount((int)this.m_AkaliShopDetail.bGoodsCnt);
				}
			}
			this.RefreshBanner();
			this.RefreshTimer();
		}

		private void OnElementEnable(CUIEvent uiEvent)
		{
			if (this.m_AkaliShopDetail == null)
			{
				return;
			}
			if (!this.IsGetDisCount())
			{
				this.OnDefaultItem(uiEvent);
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList > (int)this.m_AkaliShopDetail.bGoodsCnt)
			{
				return;
			}
			COMDT_AKALISHOP_GOODS cOMDT_AKALISHOP_GOODS = this.m_AkaliShopDetail.astGoodsList[srcWidgetIndexInBelongedList];
			if (cOMDT_AKALISHOP_GOODS != null)
			{
				this.UpdateItem(uiEvent, cOMDT_AKALISHOP_GOODS);
			}
		}

		private void OnDefaultItem(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcFormScript == null || srcWidget == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(srcWidget, "defualtItem");
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
			}
			GameObject gameObject2 = Utility.FindChild(srcWidget, "heroItem");
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(false);
			}
			GameObject gameObject3 = Utility.FindChild(srcWidget, "imgExperienceMark");
			if (gameObject3)
			{
				gameObject3.CustomSetActive(false);
			}
			GameObject gameObject4 = Utility.FindChild(srcWidget, "ButtonGroup/BuyBtn/Text");
			if (gameObject4)
			{
				gameObject4.GetComponent<Text>().text = "敬请期待";
			}
		}

		private void OnDefaultItemClick(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Mall_Mystery_Default_Item_Click_Tips"), false);
		}

		private void UpdateItem(CUIEvent uiEvent, COMDT_AKALISHOP_GOODS productInfo)
		{
			if (productInfo == null)
			{
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcFormScript == null || srcWidget == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(srcWidget, "heroItem");
			if (gameObject == null)
			{
				DebugHelper.Assert(gameObject != null, "hero item is null");
				return;
			}
			GameObject gameObject2 = Utility.FindChild(srcWidget, "defualtItem");
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(false);
			}
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "heroDataPanel/heroNamePanel/heroNameText");
			GameObject gameObject3 = Utility.FindChild(gameObject, "heroDataPanel/heroNamePanel/heroSkinText");
			if (gameObject3 == null)
			{
				return;
			}
			Text component = gameObject3.GetComponent<Text>();
			GameObject gameObject4 = Utility.FindChild(gameObject, "tag");
			if (gameObject4 == null)
			{
				return;
			}
			GameObject gameObject5 = Utility.FindChild(gameObject, "profession");
			if (gameObject5 == null)
			{
				return;
			}
			GameObject gameObject6 = Utility.FindChild(srcWidget, "imgExperienceMark");
			if (gameObject6 == null)
			{
				return;
			}
			if (gameObject6)
			{
				gameObject6.CustomSetActive(true);
			}
			GameObject gameObject7 = Utility.FindChild(gameObject, "skinLabelImage");
			if (gameObject7 == null)
			{
				return;
			}
			GameObject gameObject8 = Utility.FindChild(gameObject, "heroDataPanel/heroPricePanel");
			if (gameObject8 == null)
			{
				return;
			}
			gameObject8.CustomSetActive(false);
			GameObject gameObject9 = Utility.FindChild(srcWidget, "ButtonGroup/BuyBtn");
			if (gameObject9 == null)
			{
				return;
			}
			gameObject9.CustomSetActive(false);
			Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject9, "Text");
			Button component2 = gameObject9.GetComponent<Button>();
			if (component2 == null)
			{
				return;
			}
			CUIEventScript component3 = gameObject9.GetComponent<CUIEventScript>();
			if (component3 == null)
			{
				return;
			}
			component3.enabled = false;
			component2.enabled = false;
			GameObject gameObject10 = Utility.FindChild(srcWidget, "ButtonGroup/LinkBtn");
			if (gameObject10 == null)
			{
				return;
			}
			gameObject10.CustomSetActive(false);
			Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject10, "Text");
			Button component4 = gameObject10.GetComponent<Button>();
			if (component4 == null)
			{
				return;
			}
			CUIEventScript component5 = gameObject10.GetComponent<CUIEventScript>();
			if (component5 == null)
			{
				return;
			}
			component5.enabled = false;
			component4.enabled = false;
			COM_ITEM_TYPE wItemType = (COM_ITEM_TYPE)productInfo.wItemType;
			uint dwItemID = productInfo.dwItemID;
			switch (wItemType)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(dwItemID);
				DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
				if (dataByKey == null)
				{
					return;
				}
				ResHeroShop resHeroShop = null;
				GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out resHeroShop);
				CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, gameObject, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
				gameObject5.CustomSetActive(false);
				gameObject7.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				if (componetInChild != null)
				{
					componetInChild.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
				}
				if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
				{
					gameObject9.CustomSetActive(true);
					componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
					gameObject4.CustomSetActive(false);
					gameObject6.CustomSetActive(false);
				}
				else
				{
					gameObject6.CustomSetActive(masterRoleInfo.IsValidExperienceHero(dataByKey.dwCfgID));
					gameObject8.CustomSetActive(true);
					gameObject9.CustomSetActive(true);
					componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Buy");
					component3.enabled = true;
					component2.enabled = true;
					this.UpdateItemPricePnl(srcFormScript, gameObject8.transform, gameObject4.transform, productInfo);
					component3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, new stUIEventParams
					{
						tag = uiEvent.m_srcWidgetIndexInBelongedList
					});
				}
				break;
			}
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(dwItemID);
				DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
				if (heroSkin == null)
				{
					return;
				}
				ResHeroSkinShop resHeroSkinShop = null;
				GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwSkinID, out resHeroSkinShop);
				ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
				DebugHelper.Assert(dataByKey2 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
				if (dataByKey2 == null)
				{
					return;
				}
				CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, gameObject.gameObject, heroSkin.szSkinPicID, enHeroHeadType.enBust, false, true);
				gameObject5.CustomSetActive(false);
				CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, gameObject7, heroSkin.dwHeroID, heroSkin.dwSkinID);
				gameObject3.CustomSetActive(true);
				if (componetInChild != null)
				{
					componetInChild.text = StringHelper.UTF8BytesToString(ref dataByKey2.szName);
				}
				if (component != null)
				{
					component.text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
				}
				if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
				{
					gameObject9.CustomSetActive(true);
					componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own");
					gameObject4.CustomSetActive(false);
					gameObject6.CustomSetActive(false);
				}
				else
				{
					gameObject6.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(heroSkin.dwHeroID, heroSkin.dwSkinID));
					gameObject8.CustomSetActive(true);
					gameObject10.CustomSetActive(false);
					this.UpdateItemPricePnl(srcFormScript, gameObject8.transform, gameObject4.transform, productInfo);
					if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
					{
						gameObject9.CustomSetActive(true);
						component3.enabled = true;
						componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
						component2.enabled = true;
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.openHeroFormPar.heroId = heroSkin.dwHeroID;
						eventParams.openHeroFormPar.skinId = heroSkin.dwSkinID;
						eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
						component3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
						this.UpdateItemPricePnl(srcFormScript, gameObject8.transform, gameObject4.transform, productInfo);
					}
					else
					{
						gameObject10.CustomSetActive(false);
						gameObject9.CustomSetActive(true);
						componetInChild2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Buy");
						component3.enabled = true;
						component2.enabled = true;
						component3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, new stUIEventParams
						{
							tag = uiEvent.m_srcWidgetIndexInBelongedList
						});
					}
				}
				break;
			}
			}
		}

		private void UpdateItemPricePnl(CUIFormScript form, Transform pricePnlTrans, Transform tagTrans, COMDT_AKALISHOP_GOODS productInfo)
		{
			GameObject obj = Utility.FindChild(pricePnlTrans.gameObject, "oldPricePanel");
			Text componetInChild = Utility.GetComponetInChild<Text>(pricePnlTrans.gameObject, "oldPricePanel/oldPriceText");
			Image componetInChild2 = Utility.GetComponetInChild<Image>(pricePnlTrans.gameObject, "newPricePanel/costImage");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(pricePnlTrans.gameObject, "newPricePanel/newCostText");
			Text componetInChild4 = Utility.GetComponetInChild<Text>(tagTrans.gameObject, "Text");
			tagTrans.gameObject.CustomSetActive(false);
			obj.CustomSetActive(false);
			tagTrans.gameObject.CustomSetActive(true);
			float num = productInfo.dwItemDiscount / 10u;
			if (Math.Abs(num % 1f) < 1.401298E-45f)
			{
				componetInChild4.text = string.Format("{0}折", ((int)num).ToString("D"));
			}
			else
			{
				componetInChild4.text = string.Format("{0}折", num.ToString("0.0"));
			}
			obj.CustomSetActive(true);
			componetInChild.text = productInfo.dwOrigPrice.ToString();
			componetInChild3.text = productInfo.dwRealPrice.ToString();
			componetInChild2.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.DianQuan), form, true, false, false, false);
		}

		public string GetDiscountNumIconPath(uint discount)
		{
			if (discount > 0u && discount < 100u)
			{
				return CUIUtility.s_Sprite_System_Mall_Dir + string.Format("Discount_Bg_N{0}", discount / 10u);
			}
			return string.Format("{0}{1}", CUIUtility.s_Sprite_System_Mall_Dir, "Discount_Bg_WenHao");
		}

		private void RefreshBanner()
		{
			this.RefreshDiscount();
		}

		private void RefreshDiscount()
		{
			if (this.m_AkaliShopDetail == null)
			{
				return;
			}
			if (this.m_AkaliShopInfoSvr == null)
			{
				return;
			}
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject obj = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/GetDiscountBtn");
			GameObject gameObject = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/BoughtLimit");
			GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/Discount/");
			GameObject obj3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/UnkownDiscount");
			GameObject obj4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/DiscountAfter");
			GameObject gameObject2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/DiscountAfter/DiscountBg");
			if (this.IsGetDisCount())
			{
				obj.CustomSetActive(false);
				obj3.CustomSetActive(false);
				gameObject.CustomSetActive(true);
				obj2.CustomSetActive(false);
				gameObject2.CustomSetActive(true);
				obj4.CustomSetActive(true);
				if (gameObject2 != null)
				{
					Image componetInChild = Utility.GetComponetInChild<Image>(gameObject2, "Num");
					if (componetInChild != null)
					{
						componetInChild.SetSprite(this.GetDiscountNumIconPath((uint)this.m_AkaliShopDetail.iShowDiscount), mallForm, true, false, false, false);
					}
				}
				if (gameObject != null)
				{
					Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "Cnt");
					if (componetInChild2 != null)
					{
						componetInChild2.text = string.Format("{0}/{1}", this.m_bRequestBuyCount, this.m_AkaliShopDetail.bMaxBuyCnt);
					}
				}
			}
			else
			{
				obj.CustomSetActive(true);
				obj3.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				obj2.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				obj4.CustomSetActive(false);
			}
		}

		private void OnRollDiscount(CUIEvent uiEvent)
		{
			if (!this.IsShopAvailable())
			{
				DebugHelper.Assert(false, "神秘商店未开启");
				return;
			}
			if (this.IsGetDisCount())
			{
				DebugHelper.Assert(false, "随机折扣不能重复获取");
				return;
			}
			this.RequestDisCount();
		}

		private void OnOpenBuyItem(CUIEvent uiEvent)
		{
			if (!this.IsShopAvailable())
			{
				return;
			}
			if (this.m_AkaliShopDetail == null || this.m_AkaliShopInfoSvr == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList > (int)this.m_AkaliShopDetail.bGoodsCnt)
			{
				return;
			}
			COMDT_AKALISHOP_GOODS cOMDT_AKALISHOP_GOODS = this.m_AkaliShopDetail.astGoodsList[srcWidgetIndexInBelongedList];
			if (cOMDT_AKALISHOP_GOODS != null)
			{
				this.OpenBuy(uiEvent.m_srcFormScript, ref cOMDT_AKALISHOP_GOODS);
			}
		}

		public void OpenBuy(CUIFormScript form, ref COMDT_AKALISHOP_GOODS productInfo)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			switch (productInfo.wItemType)
			{
			case 4:
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(productInfo.dwItemID);
				DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
				if (dataByKey == null)
				{
					return;
				}
				if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
				{
					stUIEventParams par = default(stUIEventParams);
					par.openHeroFormPar.heroId = dataByKey.dwCfgID;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, par);
					return;
				}
				stPayInfoSet payInfoSet = default(stPayInfoSet);
				payInfoSet.m_payInfoCount = 1;
				payInfoSet.m_payInfos = new stPayInfo[1];
				stPayInfo stPayInfo = default(stPayInfo);
				stPayInfo.m_oriValue = productInfo.dwOrigPrice;
				stPayInfo.m_payValue = productInfo.dwRealPrice;
				stPayInfo.m_payType = enPayType.DianQuan;
				payInfoSet.m_payInfos[0] = stPayInfo;
				CHeroSkinBuyManager.OpenBuyHeroForm(form, dataByKey.dwCfgID, payInfoSet, enUIEventID.Mall_Mystery_On_Buy_Item);
				break;
			}
			case 7:
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(productInfo.dwItemID);
				DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
				if (heroSkin == null)
				{
					return;
				}
				ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
				DebugHelper.Assert(dataByKey2 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
				if (dataByKey2 == null)
				{
					return;
				}
				if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
				{
					stUIEventParams par2 = default(stUIEventParams);
					par2.openHeroFormPar.heroId = heroSkin.dwHeroID;
					par2.openHeroFormPar.skinId = heroSkin.dwSkinID;
					par2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, par2);
					return;
				}
				if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
				{
					stUIEventParams par3 = default(stUIEventParams);
					par3.heroId = heroSkin.dwHeroID;
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format("暂未拥有英雄{0}，是否购买", StringHelper.UTF8BytesToString(ref dataByKey2.szName)), enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, enUIEventID.None, par3, false);
					return;
				}
				stPayInfoSet payInfoSet2 = default(stPayInfoSet);
				payInfoSet2.m_payInfoCount = 1;
				payInfoSet2.m_payInfos = new stPayInfo[1];
				stPayInfo stPayInfo2 = default(stPayInfo);
				stPayInfo2.m_oriValue = productInfo.dwOrigPrice;
				stPayInfo2.m_payValue = productInfo.dwRealPrice;
				stPayInfo2.m_payType = enPayType.DianQuan;
				payInfoSet2.m_payInfos[0] = stPayInfo2;
				CHeroSkinBuyManager.OpenBuyHeroSkinForm(heroSkin.dwHeroID, heroSkin.dwSkinID, true, payInfoSet2, enUIEventID.Mall_Mystery_On_Buy_Item);
				break;
			}
			}
		}

		private void OnBuyItem(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			string goodName = string.Empty;
			COM_ITEM_TYPE cOM_ITEM_TYPE;
			uint num;
			if (uiEvent.m_eventParams.heroSkinParam.skinId != 0u)
			{
				cOM_ITEM_TYPE = COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN;
				num = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(num);
				DebugHelper.Assert(heroSkin != null, string.Format("找不到皮肤{0}的配置信息", num));
				if (heroSkin == null)
				{
					return;
				}
				goodName = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
			}
			else
			{
				if (uiEvent.m_eventParams.heroId == 0u)
				{
					DebugHelper.Assert(false, "神秘商店购买不支持该物品类型");
					return;
				}
				cOM_ITEM_TYPE = COM_ITEM_TYPE.COM_OBJTYPE_HERO;
				num = uiEvent.m_eventParams.heroId;
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(num);
				DebugHelper.Assert(dataByKey != null, string.Format("找不到英雄{0}的配置信息", num));
				if (dataByKey == null)
				{
					return;
				}
				goodName = StringHelper.UTF8BytesToString(ref dataByKey.szName);
			}
			int productID = this.GetProductID(num);
			if (productID < 0)
			{
				DebugHelper.Assert(false, string.Format("神秘商店找不到该物品{0}/{1}", Enum.GetName(typeof(COM_ITEM_TYPE), cOM_ITEM_TYPE), num));
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.Mall_Mystery_On_Confirm_Buy_Item;
			uIEvent.m_eventParams.tag = productID;
			CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, commonUInt32Param, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
		}

		private void OnConfirmuyItem(CUIEvent uiEvent)
		{
			if (this.m_AkaliShopDetail == null || this.m_AkaliShopInfoSvr == null)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			if (tag < 0)
			{
				DebugHelper.Assert(false, "商品ID不能为0");
				Singleton<CUIManager>.GetInstance().OpenTips("该商品无法购买", false, 1.5f, null, new object[0]);
				return;
			}
			if (this.m_bRequestBuyCount >= this.m_AkaliShopDetail.bMaxBuyCnt)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("您的购买次数已达到神秘商店限购的次数，欢迎下次再来", false, 1.5f, null, new object[0]);
				return;
			}
			this.RequestBuyItem((byte)tag);
		}

		private int GetProductID(uint itemID)
		{
			if (this.m_AkaliShopDetail != null)
			{
				for (int i = 0; i < (int)this.m_AkaliShopDetail.bGoodsCnt; i++)
				{
					if (this.m_AkaliShopDetail.astGoodsList[i].dwItemID == itemID)
					{
						return i;
					}
				}
			}
			return -1;
		}

		[MessageHandler(1406)]
		public static void ReceiveAkAlishopDetail(CSPkg msg)
		{
			Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail = msg.stPkgData.stAkaliShopInfo.stAkaliShopData;
			Singleton<MySteryShop>.GetInstance().m_AkaliShopInfoSvr = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy;
			Singleton<MySteryShop>.GetInstance().m_bRequestDiscount = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy.bAlreadyGet;
			Singleton<MySteryShop>.GetInstance().m_bRequestBuyCount = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy.bBuyCnt;
		}

		private void RequestDisCount()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1409u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1410)]
		public static void ReceiveDiscount(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAkaliShopFlagRsp.iResult == 0)
			{
				Singleton<MySteryShop>.GetInstance().m_bRequestDiscount = 1;
				Singleton<MySteryShop>.GetInstance().UpdateUI();
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(MySteryShop.MYSTERY_ROLL_DISCOUNT_FORM_PATH, false, true);
				DebugHelper.Assert(cUIFormScript != null, "获得随机折扣form失败");
				if (cUIFormScript == null)
				{
					return;
				}
				CUICommonSystem.PlayAnimator(Utility.FindChild(cUIFormScript.gameObject, "Panel_NewDiscount/Content/Discount"), string.Format("Discount_{0}", Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.iShowDiscount / 10));
			}
		}

		private void RequestBuyItem(byte buyIdx)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1407u);
			cSPkg.stPkgData.stAkaliShopBuyReq.bBuyIdx = buyIdx;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1408)]
		public static void ReceiveBuyItem(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAkaliShopBuyRsp.iResult == 0)
			{
				MySteryShop expr_24 = Singleton<MySteryShop>.GetInstance();
				expr_24.m_bRequestBuyCount += 1;
				Singleton<MySteryShop>.GetInstance().UpdateUI();
				byte bBuyIdx = msg.stPkgData.stAkaliShopBuyRsp.bBuyIdx;
				if (bBuyIdx < Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.bGoodsCnt)
				{
					uint dwItemID = Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[(int)bBuyIdx].dwItemID;
					if (Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[(int)bBuyIdx].wItemType == 4)
					{
						CUICommonSystem.ShowNewHeroOrSkin(dwItemID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
					}
					else if (Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[(int)bBuyIdx].wItemType == 7)
					{
					}
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1408, msg.stPkgData.stAkaliShopBuyRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}
	}
}

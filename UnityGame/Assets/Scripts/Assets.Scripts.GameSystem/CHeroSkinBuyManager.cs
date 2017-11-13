using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CHeroSkinBuyManager : Singleton<CHeroSkinBuyManager>
	{
		public enum enBuyHeroSkinFormWidget
		{
			Buy_Rank_Limit_Text,
			Buy_Button,
			Feature_List,
			Buy_For_Friend_Button
		}

		public enum enBuyHeroSkin3DFormWidget
		{
			Buy_Rank_Limit_Text,
			Buy_Button
		}

		public const int MAX_PRESENT_MSG_LENGTH = 50;

		public static string s_buyHeroSkinFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin.prefab";

		public static string s_buyHeroSkin3DFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin_3D.prefab";

		public static string s_heroBuyFormPath = "UGUI/Form/System/Mall/Form_MallBuyHero.prefab";

		public static string s_heroBuyFriendPath = "UGUI/Form/System/HeroInfo/Form_BuyForFriend.prefab";

		public static string s_leaveMsgPath = "UGUI/Form/System/HeroInfo/Form_LeaveMessage.prefab";

		public static string s_leaveMsgAskForPath = "UGUI/Form/System/HeroInfo/Form_LeaveMessageAskFor.prefab";

		private RES_ITEM_TYPE m_giftType;

		private enFriendGiftOpType m_giftOpType;

		private uint m_buyHeroIDForFriend;

		private uint m_buySkinIDForFriend;

		private uint m_buyPriceForFriend;

		private uint m_buyOldPriceForFriend;

		private ListView<COMDT_FRIEND_INFO> m_friendList;

		private ListView<ResAskforTemplet> m_askForTemplateList;

		private uint m_maxAskForTemplateID;

		private string m_lastCustomMsg;

		private ListView<COMDT_FRIEND_INFO> detailFriendList = new ListLinqView<COMDT_FRIEND_INFO>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenBuyHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuySkinForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_Buy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyConfirm, new CUIEventManager.OnUIEventHandler(this.OnHeroSkinBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SecurePwdConfirmBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SecurePwdConfirmBuyHeroForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroSkinForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SecurePwdConfirmBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SecurePwdConfirmBuySkinForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroSkinForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroSkinForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuySkinLeaveMsgForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroLeaveMsgForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_AskForHeroSkin, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_AskForHeroSkin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_AskForHero, new CUIEventManager.OnUIEventHandler(this.OnHeroView_AskForHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenAskForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenAskForHeroFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenAskForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenAskForSkinFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_LeaveMsgTypeSelect, new CUIEventManager.OnUIEventHandler(this.OnPredefinedMsgTypeSelected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_AskForHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroForFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_SearchFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SearchFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnFriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnFriendListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnCloseBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnCloseBuySkinForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnUseSkinExpCard, new CUIEventManager.OnUIEventHandler(this.OnUseSkinExpCard));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPcik_factoyShopTipsForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPcik_factoyShopTipsCancelForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsCancelForm));
			this.m_askForTemplateList = new ListView<ResAskforTemplet>();
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPcik_factoyShopTipsForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPcik_factoyShopTipsCancelForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsCancelForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroSkin_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuySkinLeaveMsgForFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroLeaveMsgForFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroSkin_AskForHeroSkin, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_AskForHeroSkin));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_AskForHero, new CUIEventManager.OnUIEventHandler(this.OnHeroView_AskForHero));
			this.m_askForTemplateList.Clear();
		}

		private void OnBuyPcik_factoyShopTipsForm(CUIEvent uiEvent)
		{
			if (uiEvent.m_eventParams.heroId > 0u)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uiEvent.m_eventParams.heroId);
				ResHeroShop resHeroShop = null;
				if (dataByKey != null)
				{
					GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out resHeroShop);
					if (resHeroShop != null && resHeroShop.dwFactoryID > 0u)
					{
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab);
						CMallFactoryShopController.ShopProduct theSp = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[resHeroShop.dwFactoryID]);
						Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(theSp);
					}
				}
			}
			else if (uiEvent.m_eventParams.heroSkinParam.heroId > 0u && uiEvent.m_eventParams.heroSkinParam.skinId > 0u)
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
				if (heroSkin != null)
				{
					ResHeroSkinShop resHeroSkinShop = null;
					GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out resHeroSkinShop);
					if (resHeroSkinShop != null && resHeroSkinShop.dwFactoryID > 0u)
					{
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab);
						CMallFactoryShopController.ShopProduct theSp2 = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[resHeroSkinShop.dwFactoryID]);
						Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(theSp2);
					}
				}
			}
		}

		private void OnBuyPcik_factoyShopTipsCancelForm(CUIEvent uiEvent)
		{
			if (uiEvent.m_eventParams.tag3 == 1)
			{
				CHeroSkinBuyManager.OpenBuyHeroForm(uiEvent.m_srcFormScript, uiEvent.m_eventParams.heroId, default(stPayInfoSet), enUIEventID.None);
			}
			else if (uiEvent.m_eventParams.tag3 == 2)
			{
				CHeroSkinBuyManager.OpenBuyHeroSkinForm(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId, uiEvent.m_eventParams.heroSkinParam.isCanCharge, default(stPayInfoSet), enUIEventID.None);
			}
		}

		private void OnOpenBuyHeroForm(CUIEvent uiEvent)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uiEvent.m_eventParams.heroId);
			ResHeroShop resHeroShop = null;
			if (dataByKey != null)
			{
				GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out resHeroShop);
				if (resHeroShop != null && resHeroShop.dwFactoryID > 0u)
				{
					ResSpecSale resSpecSale = null;
					GameDataMgr.specSaleDict.TryGetValue(resHeroShop.dwFactoryID, out resSpecSale);
					if (resSpecSale != null)
					{
						int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
						if ((long)currentUTCTime >= (long)((ulong)resSpecSale.dwOnTimeGen) && (long)currentUTCTime < (long)((ulong)resSpecSale.dwOffTimeGen))
						{
							CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)resSpecSale.dwSpecSaleType, resSpecSale.dwSpecSaleId, 0);
							if (cUseable != null)
							{
								string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("FactoyBuy_Tips_Confirm"), resHeroShop.szName, cUseable.m_name);
								uiEvent.m_eventParams.tag3 = 1;
								Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPcik_factoyShopTipsForm, enUIEventID.BuyPcik_factoyShopTipsCancelForm, uiEvent.m_eventParams, "去看看", "不用了", false, string.Empty);
								return;
							}
						}
					}
				}
			}
			CHeroSkinBuyManager.OpenBuyHeroForm(uiEvent.m_srcFormScript, uiEvent.m_eventParams.heroId, default(stPayInfoSet), enUIEventID.None);
		}

		public static void OpenBuyHeroForm(CUIFormScript srcform, uint heroId, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_heroBuyFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey == null)
			{
				return;
			}
			Text component = cUIFormScript.transform.Find("heroInfoPanel/title/Text").GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Title"));
			GameObject gameObject = cUIFormScript.transform.Find("heroInfoPanel/heroItem").gameObject;
			Text component2 = gameObject.transform.Find("heroNameText").GetComponent<Text>();
			CUICommonSystem.SetHeroItemImage(cUIFormScript, gameObject, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
			component2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
			GameObject gameObject2 = cUIFormScript.transform.Find("heroInfoPanel/heroPricePanel").gameObject;
			if (payInfoSet.m_payInfoCount > 0)
			{
				CHeroSkinBuyManager.SetHeroBuyPricePanel(cUIFormScript, gameObject2, ref payInfoSet, heroId, btnClickEventID);
			}
			else
			{
				IHeroData heroData = CHeroDataFactory.CreateHeroData(heroId);
				ResHeroPromotion resPromotion = heroData.promotion();
				stPayInfoSet stPayInfoSet = default(stPayInfoSet);
				stPayInfoSet = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
				CHeroSkinBuyManager.SetHeroBuyPricePanel(cUIFormScript, gameObject2, ref stPayInfoSet, heroId, btnClickEventID);
			}
			Transform transform = cUIFormScript.transform.Find("heroInfoPanel/buyForFriendBtn");
			if (transform)
			{
				if (CHeroSkinBuyManager.ShouldShowBuyForFriend(false, heroId, 0u, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
				{
					transform.gameObject.CustomSetActive(true);
					CUIEventScript component3 = transform.GetComponent<CUIEventScript>();
					if (component3)
					{
						component3.m_onClickEventParams.heroId = heroId;
					}
				}
				else
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		public static void SetHeroBuyPricePanel(CUIFormScript formScript, GameObject pricePanel, ref stPayInfoSet payInfoSet, uint heroID, enUIEventID btnClickEventID = enUIEventID.None)
		{
			if (null == formScript || pricePanel == null)
			{
				return;
			}
			Transform transform = pricePanel.transform.Find("pnlCoinBuy");
			Transform transform2 = pricePanel.transform.Find("pnlDiamondBuy");
			Transform transform3 = pricePanel.transform.Find("Text");
			if (transform == null || transform2 == null)
			{
				return;
			}
			transform.gameObject.CustomSetActive(false);
			transform2.gameObject.CustomSetActive(false);
			if (transform3 != null)
			{
				transform3.gameObject.CustomSetActive(payInfoSet.m_payInfoCount > 1);
			}
			GameObject gameObject = Utility.FindChild(formScript.gameObject, "heroInfoPanel/BtnGroup/coinBuyBtn");
			GameObject gameObject2 = Utility.FindChild(formScript.gameObject, "heroInfoPanel/BtnGroup/diamondBuyBtn");
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
			{
				stPayInfo stPayInfo = payInfoSet.m_payInfos[i];
				GameObject gameObject3;
				if (payInfoSet.m_payInfos[i].m_payType == enPayType.GoldCoin)
				{
					transform.gameObject.CustomSetActive(true);
					CHeroSkinBuyManager.SetPayInfoPanel(formScript, transform, ref stPayInfo, heroID, btnClickEventID);
					gameObject3 = gameObject;
				}
				else
				{
					transform2.gameObject.CustomSetActive(true);
					CHeroSkinBuyManager.SetPayInfoPanel(formScript, transform2, ref stPayInfo, heroID, btnClickEventID);
					gameObject3 = gameObject2;
				}
				gameObject3.CustomSetActive(true);
				if (gameObject3 != null)
				{
					Text componetInChild = Utility.GetComponetInChild<Text>(gameObject3, "Text");
					if (componetInChild != null)
					{
						componetInChild.set_text(CMallSystem.GetPriceTypeBuyString(stPayInfo.m_payType));
					}
					CUIEventScript component = gameObject3.GetComponent<CUIEventScript>();
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.tag = (int)stPayInfo.m_payType;
					eventParams.commonUInt32Param1 = stPayInfo.m_payValue;
					eventParams.heroId = heroID;
					if (btnClickEventID != enUIEventID.None)
					{
						component.SetUIEvent(enUIEventType.Click, btnClickEventID, eventParams);
					}
					else
					{
						component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_BuyHero, eventParams);
					}
				}
			}
		}

		public static void SetPayCostIcon(CUIFormScript formScript, Transform costIcon, enPayType payType)
		{
			if (null == formScript || null == costIcon)
			{
				return;
			}
			Image component = costIcon.GetComponent<Image>();
			if (component != null)
			{
				component.SetSprite(CMallSystem.GetPayTypeIconPath(payType), formScript, true, false, false, false);
			}
		}

		public static void SetPayCostTypeText(Transform costTypeText, enPayType payType)
		{
			if (costTypeText != null)
			{
				Text component = costTypeText.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(CMallSystem.GetPayTypeText(payType));
				}
			}
		}

		public static void SetPayCurrentPrice(Transform currentPrice, uint payValue)
		{
			if (currentPrice != null)
			{
				Text component = currentPrice.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(payValue.ToString());
				}
			}
		}

		public static void SetPayOldPrice(Transform oldPrice, uint oriValue)
		{
			if (oldPrice != null)
			{
				Text component = oldPrice.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(oriValue.ToString());
				}
			}
		}

		private static void SetPayInfoPanel(CUIFormScript formScript, Transform payInfoPanel, ref stPayInfo payInfo, uint heroID, enUIEventID btnClickEventID)
		{
			Transform costIcon = payInfoPanel.Find("costImage");
			CHeroSkinBuyManager.SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
			Transform costTypeText = payInfoPanel.Find("costTypeText");
			CHeroSkinBuyManager.SetPayCostTypeText(costTypeText, payInfo.m_payType);
			Transform transform = payInfoPanel.Find("costPanel");
			if (transform != null)
			{
				Transform transform2 = transform.Find("oldPricePanel");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
					Transform oldPrice = transform2.Find("oldPriceText");
					CHeroSkinBuyManager.SetPayOldPrice(oldPrice, payInfo.m_oriValue);
				}
				Transform currentPrice = transform.Find("costText");
				CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, payInfo.m_payValue);
			}
		}

		public void OnHeroSkin_Buy(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			bool isCanCharge = uiEvent.m_eventParams.heroSkinParam.isCanCharge;
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			string goodName = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.HeroSkin_BuyConfirm;
			uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
			uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
			uIEvent.m_eventParams.tag = (int)tag;
			uIEvent.m_eventParams.commonUInt32Param1 = commonUInt32Param;
			CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, commonUInt32Param, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, isCanCharge, false);
		}

		public void OnHeroSkinBuyConfirm(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			BUY_HEROSKIN_TYPE buyType;
			switch (tag)
			{
			case enPayType.DianQuan:
				buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_COUPONS;
				break;
			case enPayType.Diamond:
				buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
				break;
			case enPayType.DiamondAndDianQuan:
				buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_MIXPAY;
				break;
			default:
				buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
				break;
			}
			CHeroSkinBuyManager.ReqBuyHeroSkin(heroId, skinId, buyType, false);
		}

		private void OnOpenBuySkinForm(CUIEvent uiEvent)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
			if (heroSkin != null)
			{
				ResHeroSkinShop resHeroSkinShop = null;
				GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out resHeroSkinShop);
				if (resHeroSkinShop != null && resHeroSkinShop.dwFactoryID > 0u)
				{
					ResSpecSale resSpecSale = null;
					GameDataMgr.specSaleDict.TryGetValue(resHeroSkinShop.dwFactoryID, out resSpecSale);
					if (resSpecSale != null)
					{
						int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
						if ((long)currentUTCTime >= (long)((ulong)resSpecSale.dwOnTimeGen) && (long)currentUTCTime < (long)((ulong)resSpecSale.dwOffTimeGen))
						{
							CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)resSpecSale.dwSpecSaleType, resSpecSale.dwSpecSaleId, 0);
							if (cUseable != null)
							{
								string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("FactoyBuy_Tips_Confirm"), resHeroSkinShop.szSkinName, cUseable.m_name);
								uiEvent.m_eventParams.tag3 = 2;
								Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPcik_factoyShopTipsForm, enUIEventID.BuyPcik_factoyShopTipsCancelForm, uiEvent.m_eventParams, "去看看", "不用了", false, string.Empty);
								return;
							}
						}
					}
				}
			}
			CHeroSkinBuyManager.OpenBuyHeroSkinForm(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId, uiEvent.m_eventParams.heroSkinParam.isCanCharge, default(stPayInfoSet), enUIEventID.None);
		}

		public static void OpenBuyHeroSkinForm(uint heroId, uint skinId, bool isCanCharge, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_buyHeroSkinFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin == null)
			{
				return;
			}
			Transform transform = cUIFormScript.gameObject.transform.Find("Panel");
			Image component = transform.Find("skinBgImage/skinIconImage").GetComponent<Image>();
			string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
			component.SetSprite(prefabPath, cUIFormScript, true, false, false, true);
			Text component2 = transform.Find("skinNameText").GetComponent<Text>();
			component2.set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
			GameObject widget = cUIFormScript.GetWidget(2);
			CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
			int num = 0;
			int num2 = 37;
			if (CHeroInfoSystem2.s_propArr == null || CHeroInfoSystem2.s_propPctArr == null)
			{
				num = 0;
			}
			else
			{
				for (int i = 0; i < num2; i++)
				{
					if (CHeroInfoSystem2.s_propArr[i] > 0 || CHeroInfoSystem2.s_propPctArr[i] > 0)
					{
						num++;
					}
				}
			}
			CUIListScript component3 = widget.GetComponent<CUIListScript>();
			int num3 = 0;
			bool skinFeatureCnt = CSkinInfo.GetSkinFeatureCnt(heroId, skinId, out num3);
			int elementAmount = num + num3;
			component3.SetElementAmount(elementAmount);
			CHeroSkinBuyManager.SetSkinListProp(widget, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
			for (int j = 0; j < num3; j++)
			{
				CUIListElementScript elemenet = component3.GetElemenet(j + num);
				GameObject widget2 = elemenet.GetWidget(0);
				string prefabPath2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, heroSkin.astFeature[j].szIconPath);
				widget2.GetComponent<Image>().SetSprite(prefabPath2, elemenet.m_belongedFormScript, true, false, false, false);
				GameObject widget3 = elemenet.GetWidget(1);
				widget3.GetComponent<Text>().set_text(heroSkin.astFeature[j].szDesc);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (payInfoSet.m_payInfoCount == 0)
			{
				ResSkinPromotion resPromotion = new ResSkinPromotion();
				resPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
				payInfoSet = CMallSystem.GetPayInfoSetOfGood(heroSkin, resPromotion);
			}
			Transform skinPricePanel = transform.Find("skinPricePanel");
			GameObject widget4 = cUIFormScript.GetWidget(1);
			GameObject widget5 = cUIFormScript.GetWidget(3);
			if (payInfoSet.m_payInfoCount > 0)
			{
				CHeroSkinBuyManager.SetSkinPricePanel(cUIFormScript, skinPricePanel, ref payInfoSet.m_payInfos[0]);
				if (masterRoleInfo != null)
				{
					if (!masterRoleInfo.IsHaveHero(heroId, false))
					{
						if (widget4 != null)
						{
							Transform transform2 = widget4.transform.Find("Text");
							if (transform2 != null)
							{
								Text component4 = transform2.GetComponent<Text>();
								if (component4 != null)
								{
									component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Text_1"));
								}
							}
							CUIEventScript component5 = widget4.GetComponent<CUIEventScript>();
							if (component5 != null)
							{
								stUIEventParams eventParams = default(stUIEventParams);
								eventParams.openHeroFormPar.heroId = heroId;
								eventParams.openHeroFormPar.skinId = skinId;
								eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
								component5.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
							}
						}
					}
					else if (widget4 != null)
					{
						CUIEventScript component6 = widget4.GetComponent<CUIEventScript>();
						if (component6 != null)
						{
							CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
							if (btnClickEventID == enUIEventID.None)
							{
								uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
							}
							else
							{
								uIEvent.m_eventID = btnClickEventID;
							}
							uIEvent.m_eventParams.tag = (int)payInfoSet.m_payInfos[0].m_payType;
							uIEvent.m_eventParams.commonUInt32Param1 = payInfoSet.m_payInfos[0].m_payValue;
							uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
							uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
							uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
							component6.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
						}
					}
				}
			}
			if (widget5)
			{
				if (CHeroSkinBuyManager.ShouldShowBuyForFriend(true, heroId, skinId, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
				{
					widget5.CustomSetActive(true);
					CUIEventScript component7 = widget5.GetComponent<CUIEventScript>();
					if (component7)
					{
						component7.m_onClickEventParams.heroSkinParam.heroId = heroId;
						component7.m_onClickEventParams.heroSkinParam.skinId = skinId;
					}
				}
				else
				{
					widget5.CustomSetActive(false);
				}
			}
			GameObject widget6 = cUIFormScript.GetWidget(0);
			CHeroSkinBuyManager.SetRankLimitWidgets(heroId, skinId, widget6, widget4);
		}

		private static void SetSkinListProp(GameObject listObj, ref int[] propArr, ref int[] propPctArr, ref string[] propImgArr)
		{
			int num = 37;
			if (listObj == null || propArr == null || propPctArr == null || propArr.Length != num || propPctArr.Length != num)
			{
				listObj.CustomSetActive(false);
				return;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0)
				{
					num2++;
				}
			}
			listObj.CustomSetActive(true);
			CUIListScript component = listObj.GetComponent<CUIListScript>();
			num2 = 0;
			for (int j = 0; j < num; j++)
			{
				if (propArr[j] > 0 || propPctArr[j] > 0)
				{
					CUIListElementScript elemenet = component.GetElemenet(num2);
					if (!(elemenet == null))
					{
						Image component2 = elemenet.GetWidget(0).GetComponent<Image>();
						if (component2 != null && !string.IsNullOrEmpty(propImgArr[j]))
						{
							string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, propImgArr[j]);
							component2.SetSprite(prefabPath, component.m_belongedFormScript, true, false, false, false);
						}
						Text component3 = elemenet.GetWidget(1).GetComponent<Text>();
						if (component3 != null)
						{
							if (propArr[j] > 0)
							{
								component3.set_text(string.Format("{0} +{1}", CUICommonSystem.s_attNameList[j], propArr[j]));
							}
							else if (propPctArr[j] > 0)
							{
								component3.set_text(string.Format("{0} +{1}", CUICommonSystem.s_attNameList[j], CUICommonSystem.GetValuePercent(propPctArr[j])));
							}
						}
						num2++;
					}
				}
			}
		}

		private static void SetRankLimitWidgets(uint heroId, uint skinId, GameObject rankLimitTextGo, GameObject buyButtonGo)
		{
			RES_RANK_LIMIT_TYPE rES_RANK_LIMIT_TYPE;
			byte rankBigGrade;
			ulong time;
			bool flag;
			if (CSkinInfo.IsBuyForbiddenForRankBigGrade(heroId, skinId, out rES_RANK_LIMIT_TYPE, out rankBigGrade, out time, out flag))
			{
				Button component = buyButtonGo.GetComponent<Button>();
				CUICommonSystem.SetButtonEnableWithShader(component, false, true);
			}
			if (CSkinInfo.IsCanBuy(heroId, skinId) && flag)
			{
				rankLimitTextGo.CustomSetActive(true);
				Text component2 = rankLimitTextGo.GetComponent<Text>();
				component2.set_text(string.Empty);
				string rankBigGradeName = CLadderView.GetRankBigGradeName(rankBigGrade);
				switch (rES_RANK_LIMIT_TYPE)
				{
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_CURGRADE:
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Current_Grade", new string[]
					{
						rankBigGradeName
					}));
					break;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_SEASONGRADE:
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Season_Highest_Grade", new string[]
					{
						rankBigGradeName
					}));
					break;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_MAXGRADE:
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Highest_Grade", new string[]
					{
						rankBigGradeName
					}));
					break;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_HISTORYGRADE:
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Grade", new string[]
					{
						Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(time),
						rankBigGradeName
					}));
					break;
				}
			}
			else
			{
				rankLimitTextGo.CustomSetActive(false);
			}
		}

		private static void SetSkinPricePanel(CUIFormScript formScript, Transform skinPricePanel, ref stPayInfo payInfo)
		{
			if (null == formScript || null == skinPricePanel)
			{
				return;
			}
			Transform costIcon = skinPricePanel.Find("costImage");
			CHeroSkinBuyManager.SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
			Transform costTypeText = skinPricePanel.Find("costTypeText");
			CHeroSkinBuyManager.SetPayCostTypeText(costTypeText, payInfo.m_payType);
			Transform transform = skinPricePanel.Find("costPanel");
			if (transform != null)
			{
				Transform transform2 = transform.Find("oldPricePanel");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
					Transform oldPrice = transform2.Find("oldPriceText");
					CHeroSkinBuyManager.SetPayOldPrice(oldPrice, payInfo.m_oriValue);
				}
				Transform currentPrice = transform.Find("costText");
				CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, payInfo.m_payValue);
			}
		}

		private void OnUseSkinExpCard(CUIEvent uiEvent)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
			if (heroSkin != null)
			{
				CBagSystem.UseSkinExpCard(heroSkin.dwID);
			}
		}

		public static void OpenBuyHeroSkinForm3D(uint heroId, uint skinId, bool isCanCharge)
		{
			if (skinId == 0u)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_buyHeroSkin3DFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin == null)
			{
				return;
			}
			Transform transform = cUIFormScript.transform.Find("Panel");
			Text component = transform.Find("skinNameText").GetComponent<Text>();
			component.set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
			GameObject gameObject = transform.Find("Panel_Prop/List_Prop").gameObject;
			CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
			CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "OpenBuyHeroSkinForm3D role is null");
				return;
			}
			Transform transform2 = transform.Find("BtnGroup/useExpCardButton");
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
				if (CBagSystem.CanUseSkinExpCard(heroSkin.dwID))
				{
					transform2.gameObject.CustomSetActive(true);
					CUIEventScript component2 = transform2.GetComponent<CUIEventScript>();
					if (component2 != null)
					{
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.heroSkinParam.heroId = heroId;
						eventParams.heroSkinParam.skinId = skinId;
						component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OnUseSkinExpCard, eventParams);
					}
				}
			}
			Transform transform3 = transform.Find("pricePanel");
			Transform transform4 = transform.Find("getPathText");
			Transform transform5 = transform.Find("BtnGroup/buyButton");
			if (transform3 != null && transform5 != null)
			{
				transform3.gameObject.CustomSetActive(false);
				transform5.gameObject.CustomSetActive(false);
			}
			if (transform4 != null)
			{
				transform4.gameObject.CustomSetActive(false);
			}
			if (masterRoleInfo.IsHaveHero(heroId, false) && CSkinInfo.IsCanBuy(heroId, skinId))
			{
				stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroSkin.dwHeroID, heroSkin.dwSkinID);
				if (skinPayInfoSet.m_payInfoCount <= 0)
				{
					return;
				}
				if (transform3 != null && transform5 != null)
				{
					transform3.gameObject.CustomSetActive(true);
					transform5.gameObject.CustomSetActive(true);
					Transform transform6 = transform3.Find("costImage");
					if (transform6 != null)
					{
						Image component3 = transform6.gameObject.GetComponent<Image>();
						if (component3 != null)
						{
							component3.SetSprite(CMallSystem.GetPayTypeIconPath(skinPayInfoSet.m_payInfos[0].m_payType), cUIFormScript, true, false, false, false);
						}
					}
					Transform transform7 = transform3.Find("priceText");
					if (transform7 != null)
					{
						Text component4 = transform7.gameObject.GetComponent<Text>();
						if (component4 != null)
						{
							component4.set_text(skinPayInfoSet.m_payInfos[0].m_payValue.ToString());
						}
					}
					if (transform5 != null)
					{
						CUIEventScript component5 = transform5.gameObject.GetComponent<CUIEventScript>();
						if (component5 != null)
						{
							CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
							uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
							uIEvent.m_eventParams.tag = (int)skinPayInfoSet.m_payInfos[0].m_payType;
							uIEvent.m_eventParams.commonUInt32Param1 = skinPayInfoSet.m_payInfos[0].m_payValue;
							uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
							uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
							uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
							component5.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
						}
					}
				}
			}
			else if (transform4 != null)
			{
				transform4.gameObject.CustomSetActive(true);
				if (masterRoleInfo.IsHaveHero(heroId, false))
				{
					transform4.GetComponent<Text>().set_text(CHeroInfoSystem2.GetSkinCannotBuyStr(heroSkin));
				}
				else
				{
					transform4.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("HeroSelect_GetHeroFirstTip"));
				}
			}
			CUI3DImageScript component6 = transform.Find("3DImage").gameObject.GetComponent<CUI3DImageScript>();
			ObjNameData heroPrefabPath = CUICommonSystem.GetHeroPrefabPath(heroId, (int)skinId, true);
			GameObject gameObject2 = component6.AddGameObject(heroPrefabPath.ObjectName, false, false);
			if (gameObject2 != null)
			{
				if (heroPrefabPath.ActorInfo != null)
				{
					gameObject2.transform.localScale = new Vector3(heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale);
				}
				DynamicShadow.EnableDynamicShow(component6.gameObject, true);
				CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
				instance.Set3DModel(gameObject2);
				instance.InitAnimatList();
				instance.InitAnimatSoundList(heroId, skinId);
				instance.OnModePlayAnima("Come");
			}
			GameObject widget = cUIFormScript.GetWidget(0);
			GameObject widget2 = cUIFormScript.GetWidget(1);
			CHeroSkinBuyManager.SetRankLimitWidgets(heroId, skinId, widget, widget2);
		}

		public void OnHeroInfo_BuyHero(CUIEvent uiEvent)
		{
			enPayType tag = (enPayType)uiEvent.m_eventParams.tag;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint heroId = uiEvent.m_eventParams.heroId;
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			DebugHelper.Assert(dataByKey != null);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHero;
			uIEvent.m_eventParams.heroId = heroId;
			switch (tag)
			{
			case enPayType.GoldCoin:
				uIEvent.m_eventParams.tag = 1;
				break;
			case enPayType.DianQuan:
				uIEvent.m_eventParams.tag = 0;
				break;
			case enPayType.Diamond:
				uIEvent.m_eventParams.tag = 2;
				break;
			case enPayType.DiamondAndDianQuan:
				uIEvent.m_eventParams.tag = 3;
				break;
			}
			CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, commonUInt32Param, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
		}

		public void OnHeroInfo_ConfirmBuyHero(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			CHeroSkinBuyManager.ReqBuyHero(uiEvent.m_eventParams.heroId, tag);
		}

		public static void ReqBuyHero(uint HeroId, int BuyType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1817u);
			cSPkg.stPkgData.stBuyHeroReq.dwHeroID = HeroId;
			cSPkg.stPkgData.stBuyHeroReq.bBuyType = (byte)BuyType;
			IHeroData heroData = CHeroDataFactory.CreateHeroData(HeroId);
			if (heroData != null)
			{
				ResHeroPromotion resHeroPromotion = heroData.promotion();
				if (resHeroPromotion != null)
				{
					cSPkg.stPkgData.stBuyHeroReq.bIsPromotion = Convert.ToByte(true);
				}
				else
				{
					cSPkg.stPkgData.stBuyHeroReq.bIsPromotion = 0;
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public static void ReqBuyHeroSkin(uint heroId, uint skinId, BUY_HEROSKIN_TYPE buyType, bool isSendGameSvr = false)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1819u);
			cSPkg.stPkgData.stBuyHeroSkinReq.dwHeroID = heroId;
			cSPkg.stPkgData.stBuyHeroSkinReq.dwSkinID = skinId;
			cSPkg.stPkgData.stBuyHeroSkinReq.bBuyType = (byte)buyType;
			ResSkinPromotion resSkinPromotion = new ResSkinPromotion();
			stPayInfoSet stPayInfoSet = default(stPayInfoSet);
			resSkinPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
			if (resSkinPromotion != null)
			{
				cSPkg.stPkgData.stBuyHeroSkinReq.bIsPromotion = Convert.ToByte(true);
			}
			else
			{
				cSPkg.stPkgData.stBuyHeroSkinReq.bIsPromotion = 0;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqBuyHeroForFriend(uint heroId, ref COMDT_ACNT_UNIQ friendUin, string pwd = "", string msg = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1830u);
			cSPkg.stPkgData.stPresentHeroReq.stFriendUin = friendUin;
			cSPkg.stPkgData.stPresentHeroReq.dwHeroID = heroId;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stPresentHeroReq.szPswdInfo);
			StringHelper.StringToUTF8Bytes(msg, ref cSPkg.stPkgData.stPresentHeroReq.szPresentMsg);
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroId);
			if (heroData != null)
			{
				ResHeroPromotion resHeroPromotion = heroData.promotion();
				if (resHeroPromotion != null)
				{
					cSPkg.stPkgData.stPresentHeroReq.bIsPromotion = Convert.ToByte(true);
				}
				else
				{
					cSPkg.stPkgData.stPresentHeroReq.bIsPromotion = 0;
				}
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqBuySkinForFriend(uint heroId, uint skinId, ref COMDT_ACNT_UNIQ friendUin, string pwd = "", string msg = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1832u);
			cSPkg.stPkgData.stPresentSkinReq.stFriendUin = friendUin;
			cSPkg.stPkgData.stPresentSkinReq.dwSkinID = CSkinInfo.GetSkinCfgId(heroId, skinId);
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stPresentSkinReq.szPswdInfo);
			StringHelper.StringToUTF8Bytes(msg, ref cSPkg.stPkgData.stPresentSkinReq.szPresentMsg);
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
			if (skinPromotion != null)
			{
				cSPkg.stPkgData.stPresentSkinReq.bIsPromotion = Convert.ToByte(true);
			}
			else
			{
				cSPkg.stPkgData.stPresentSkinReq.bIsPromotion = 0;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnHeroInfo_OpenBuyHeroForFriend(CUIEvent uiEvent)
		{
			uint heroId = uiEvent.m_eventParams.heroId;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_heroBuyFriendPath, false, true);
			if (cUIFormScript != null)
			{
				this.InitBuyForFriendForm(cUIFormScript, RES_ITEM_TYPE.RES_ITEMTYPE_HERO, heroId, 0u, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, enFriendGiftOpType.Present);
			}
		}

		private void OnHeroInfo_OpenBuyHeroSkinForFriend(CUIEvent uiEvent)
		{
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_heroBuyFriendPath, false, true);
			if (cUIFormScript != null)
			{
				this.InitBuyForFriendForm(cUIFormScript, RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN, heroId, skinId, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, enFriendGiftOpType.Present);
			}
		}

		private void OnHeroInfo_BuyHeroForFriend(CUIEvent uiEvent)
		{
			uiEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHeroForFriend;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			int tag = uiEvent.m_eventParams.tag;
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long)tag);
			string tagStr = uiEvent.m_eventParams.tagStr;
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			InputField component = Utility.FindChild(srcFormScript.gameObject, "Panel/msgContainer/InputField").GetComponent<InputField>();
			if (component != null)
			{
				uiEvent.m_eventParams.tagStr1 = CUIUtility.RemoveEmoji(Utility.UTF8Convert(component.get_text()));
			}
			DebugHelper.Assert(dataByKey != null);
			if (dataByKey != null)
			{
				string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), dataByKey.szName, tagStr);
				CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, commonUInt32Param, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
			}
		}

		private void OnHeroInfo_BuySkinLeaveMsgForFriend(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag2;
			if (CSkinInfo.GetHeroSkin(this.m_buyHeroIDForFriend, this.m_buySkinIDForFriend) == null)
			{
				DebugHelper.Assert(false, "heroSkin is null");
				return;
			}
			CUIFormScript cUIFormScript;
			if (this.m_giftOpType == enFriendGiftOpType.Present)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_leaveMsgPath, false, true);
			}
			else
			{
				if (this.LimitAskForFriend(tag))
				{
					return;
				}
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_leaveMsgAskForPath, false, true);
			}
			if (cUIFormScript != null)
			{
				this.InitLeaveMsgForFriendForm(cUIFormScript, tag);
			}
		}

		private void OnHeroInfo_BuyHeroLeaveMsgForFriend(CUIEvent uiEvent)
		{
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			if (GameDataMgr.heroDatabin.GetDataByKey(heroId) == null)
			{
				DebugHelper.Assert(false, "heroCfg is null");
				return;
			}
			int tag = uiEvent.m_eventParams.tag2;
			CUIFormScript cUIFormScript;
			if (this.m_giftOpType == enFriendGiftOpType.Present)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_leaveMsgPath, false, true);
			}
			else
			{
				if (this.LimitAskForFriend(tag))
				{
					return;
				}
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_leaveMsgAskForPath, false, true);
			}
			if (cUIFormScript != null)
			{
				this.InitLeaveMsgForFriendForm(cUIFormScript, tag);
			}
		}

		private void OnHeroSkin_AskForHeroSkin(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1384u);
			cSPkg.stPkgData.stAskforReqSendReq.stAskforUniq.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
			cSPkg.stPkgData.stAskforReqSendReq.stAskforUniq.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.wItemType = 7;
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.dwItemID = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.dwItemCnt = 1u;
			int num;
			uint dwMsgID;
			string str;
			this.GetAskForMsg(uiEvent.m_srcFormScript, out num, out dwMsgID, out str);
			if (num == 1)
			{
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.bMsgType = 1;
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgString = new COMDT_ACNT_ASKFORMSG_STRING();
				StringHelper.StringToUTF8Bytes(str, ref cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgString.szContentStr);
			}
			else
			{
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.bMsgType = 0;
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgIDInfo = new COMDT_ACNT_ASKFORMSG_IDINFO();
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgIDInfo.dwMsgID = dwMsgID;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private bool GetAskForMsg(CUIFormScript form, out int msgType, out uint msgId, out string msg)
		{
			msgType = 1;
			msgId = 1u;
			msg = string.Empty;
			if (form == null)
			{
				return false;
			}
			msgType = 1;
			msgId = 1u;
			msg = string.Empty;
			GameObject gameObject = Utility.FindChild(form.gameObject, "Panel/msgTypeContainer");
			InputField componetInChild = Utility.GetComponetInChild<InputField>(form.gameObject, "Panel/msgContainer/InputField");
			CUIListScript cUIListScript = null;
			if (gameObject != null)
			{
				cUIListScript = Utility.GetComponetInChild<CUIListScript>(gameObject, "List");
			}
			if (cUIListScript != null)
			{
				int selectedIndex = cUIListScript.GetSelectedIndex();
				int count = this.m_askForTemplateList.Count;
				if (selectedIndex >= 0 || selectedIndex < count)
				{
					uint dwID = this.m_askForTemplateList[selectedIndex].dwID;
					if (dwID < this.m_maxAskForTemplateID)
					{
						msgType = 0;
						msgId = dwID;
					}
					else
					{
						msgType = 1;
						if (componetInChild != null)
						{
							msg = CUIUtility.RemoveEmoji(componetInChild.get_text());
						}
					}
				}
			}
			return true;
		}

		private void OnHeroView_AskForHero(CUIEvent uiEvent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1384u);
			cSPkg.stPkgData.stAskforReqSendReq.stAskforUniq.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
			cSPkg.stPkgData.stAskforReqSendReq.stAskforUniq.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.wItemType = 4;
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.dwItemID = (uint)uiEvent.m_eventParams.tag;
			cSPkg.stPkgData.stAskforReqSendReq.stItemInfo.dwItemCnt = 1u;
			int num;
			uint dwMsgID;
			string str;
			this.GetAskForMsg(uiEvent.m_srcFormScript, out num, out dwMsgID, out str);
			if (num == 1)
			{
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.bMsgType = 1;
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgString = new COMDT_ACNT_ASKFORMSG_STRING();
				StringHelper.StringToUTF8Bytes(str, ref cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgString.szContentStr);
			}
			else
			{
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.bMsgType = 0;
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgIDInfo = new COMDT_ACNT_ASKFORMSG_IDINFO();
				cSPkg.stPkgData.stAskforReqSendReq.stMsgInfo.stMsgInfo.stMsgIDInfo.dwMsgID = dwMsgID;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnHeroInfo_BuyHeroSkinForFriend(CUIEvent uiEvent)
		{
			uiEvent.m_eventID = enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			string tagStr = uiEvent.m_eventParams.tagStr;
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			InputField component = Utility.FindChild(srcFormScript.gameObject, "Panel/msgContainer/InputField").GetComponent<InputField>();
			if (component != null)
			{
				uiEvent.m_eventParams.tagStr1 = CUIUtility.RemoveEmoji(Utility.UTF8Convert(component.get_text()));
			}
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			DebugHelper.Assert(heroSkin != null);
			if (heroSkin != null)
			{
				string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), heroSkin.szSkinName, tagStr);
				CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, commonUInt32Param, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
			}
		}

		private void OnHeroInfo_ConfirmBuyHeroForFriend(CUIEvent uiEvent)
		{
			CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_HERO_FOR_FRIEND, enUIEventID.HeroView_SecurePwdConfirmBuyHeroForFriend, uiEvent.m_eventParams);
		}

		private void OnHeroInfo_ConfirmBuyHeroSkinForFriend(CUIEvent uiEvent)
		{
			CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_SKIN_FOR_FRIEND, enUIEventID.HeroView_SecurePwdConfirmBuyHeroSkinForFriend, uiEvent.m_eventParams);
		}

		private void OnHeroInfo_SecurePwdConfirmBuyHeroForFriend(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			COMDT_ACNT_UNIQ cOMDT_ACNT_UNIQ = new COMDT_ACNT_UNIQ();
			cOMDT_ACNT_UNIQ.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
			cOMDT_ACNT_UNIQ.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
			string pwd = uiEvent.m_eventParams.pwd;
			string tagStr = uiEvent.m_eventParams.tagStr1;
			CHeroSkinBuyManager.ReqBuyHeroForFriend((uint)tag, ref cOMDT_ACNT_UNIQ, pwd, tagStr);
		}

		private void OnHeroInfo_SecurePwdConfirmBuySkinForFriend(CUIEvent uiEvent)
		{
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			COMDT_ACNT_UNIQ cOMDT_ACNT_UNIQ = new COMDT_ACNT_UNIQ();
			cOMDT_ACNT_UNIQ.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
			cOMDT_ACNT_UNIQ.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
			string pwd = uiEvent.m_eventParams.pwd;
			string tagStr = uiEvent.m_eventParams.tagStr1;
			CHeroSkinBuyManager.ReqBuySkinForFriend(heroId, skinId, ref cOMDT_ACNT_UNIQ, pwd, tagStr);
		}

		private void OnHeroInfo_OpenAskForHeroFriend(CUIEvent uiEvent)
		{
			if (this.LimitAskForSend())
			{
				return;
			}
			uint heroId = uiEvent.m_eventParams.heroId;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_heroBuyFriendPath, false, true);
			if (cUIFormScript != null)
			{
				this.InitBuyForFriendForm(cUIFormScript, RES_ITEM_TYPE.RES_ITEMTYPE_HERO, heroId, 0u, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, enFriendGiftOpType.AskFor);
			}
		}

		private void OnHeroInfo_OpenAskForSkinFriend(CUIEvent uiEvent)
		{
			if (this.LimitAskForSend())
			{
				return;
			}
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroSkinBuyManager.s_heroBuyFriendPath, false, true);
			if (cUIFormScript != null)
			{
				this.InitBuyForFriendForm(cUIFormScript, RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN, heroId, skinId, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, enFriendGiftOpType.AskFor);
			}
		}

		private bool LimitAskForFriend(int friendIndex)
		{
			if (this.m_friendList != null && friendIndex >= 0 && friendIndex < this.m_friendList.Count && CFriendContoller.IsNoAskFor(this.m_friendList[friendIndex].stUin.ullUid, this.m_friendList[friendIndex].stUin.dwLogicWorldId))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("No_Ask_For_Tip", true, 1.5f, null, new object[0]);
				return true;
			}
			return false;
		}

		private bool LimitAskForSend()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master Role Info is null");
				return true;
			}
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_ASKFOR_MAXNUM);
			if (srv2CltGlobalValue <= masterRoleInfo.SendAskforReqCnt)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Ask_For_Send_Cnt_Limit", true, 1.5f, null, new object[0]);
				return true;
			}
			return false;
		}

		private void OnPredefinedMsgTypeSelected(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex();
			int count = this.m_askForTemplateList.Count;
			if (selectedIndex < 0 || selectedIndex >= count)
			{
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			InputField componetInChild = Utility.GetComponetInChild<InputField>(srcFormScript.gameObject, "Panel/msgContainer/InputField");
			if (componetInChild == null)
			{
				return;
			}
			int lastSelectedIndex = cUIListScript.GetLastSelectedIndex();
			if (lastSelectedIndex >= 0 && lastSelectedIndex < count && this.m_askForTemplateList[lastSelectedIndex].dwID == this.m_maxAskForTemplateID)
			{
				this.m_lastCustomMsg = componetInChild.get_text();
			}
			if (this.m_askForTemplateList[selectedIndex].dwID == this.m_maxAskForTemplateID)
			{
				if (!string.IsNullOrEmpty(this.m_lastCustomMsg))
				{
					componetInChild.set_text(this.m_lastCustomMsg);
					componetInChild.get_placeholder().GetComponent<Text>().set_text(string.Empty);
				}
				else
				{
					componetInChild.set_text(string.Empty);
					componetInChild.get_placeholder().GetComponent<Text>().set_text(this.m_askForTemplateList[selectedIndex].szContent);
				}
				componetInChild.enabled = true;
			}
			else
			{
				componetInChild.get_placeholder().GetComponent<Text>().set_text(string.Format(this.m_askForTemplateList[selectedIndex].szContent, uiEvent.m_eventParams.tagStr));
				componetInChild.set_text(string.Empty);
				componetInChild.enabled = false;
			}
		}

		private void InitBuyForFriendForm(CUIFormScript form, RES_ITEM_TYPE giftType, uint heroId, uint skinId = 0u, ulong friendUid = 0uL, uint worldId = 0u, enFriendGiftOpType opType = enFriendGiftOpType.Present)
		{
			uint num = 0u;
			uint num2 = 0u;
			uint[] conditionParam = Singleton<CFunctionUnlockSys>.instance.GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO, RES_UNLOCKCONDITION_TYPE.RES_UNLOCKCONDITIONTYPE_ABOVELEVEL);
			uint num3 = (conditionParam.Length > 1) ? conditionParam[0] : 1u;
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_ASKFOR_SHOWDAY);
			if (giftType == RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN)
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
				DebugHelper.Assert(heroSkin != null);
				if (heroSkin != null)
				{
					Text component = form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
					if (opType == enFriendGiftOpType.Present)
					{
						component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Give_Title"));
					}
					else
					{
						component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Skin_Friend_Title"));
					}
					Image component2 = form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
					string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
					component2.SetSprite(prefabPath, form, false, true, true, true);
					Text component3 = form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
					component3.set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
					form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(true);
					GameObject gameObject = form.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
					CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
					CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
					Transform transform = form.transform.Find("Panel/skinPricePanel");
					Transform costIcon = transform.Find("costImage");
					CHeroSkinBuyManager.SetPayCostIcon(form, costIcon, enPayType.DianQuan);
					Transform costTypeText = transform.Find("costTypeText");
					CHeroSkinBuyManager.SetPayCostTypeText(costTypeText, enPayType.DianQuan);
					Transform transform2 = transform.Find("costPanel");
					if (transform2 != null)
					{
						stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
						for (int i = 0; i < skinPayInfoSet.m_payInfoCount; i++)
						{
							if (skinPayInfoSet.m_payInfos[i].m_payType == enPayType.Diamond || skinPayInfoSet.m_payInfos[i].m_payType == enPayType.DianQuan || skinPayInfoSet.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)
							{
								num2 = skinPayInfoSet.m_payInfos[i].m_payValue;
								num = skinPayInfoSet.m_payInfos[i].m_oriValue;
								break;
							}
						}
						Transform transform3 = transform2.Find("oldPricePanel");
						if (transform3 != null)
						{
							transform3.gameObject.CustomSetActive(num != num2);
							Transform oldPrice = transform3.Find("oldPriceText");
							CHeroSkinBuyManager.SetPayOldPrice(oldPrice, num);
						}
						Transform currentPrice = transform2.Find("costText");
						CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, num2);
					}
					Transform transform4 = form.transform.Find("Panel/TipTxt");
					if (transform4 != null)
					{
						Text component4 = transform4.GetComponent<Text>();
						if (component4 != null)
						{
							if (opType == enFriendGiftOpType.Present)
							{
								component4.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Buy_Skin_For_Friend_Tip"), num3));
							}
							else
							{
								component4.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Skin_Friend_Tip"), num3, srv2CltGlobalValue));
							}
						}
					}
				}
			}
			else
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
				DebugHelper.Assert(dataByKey != null);
				if (dataByKey != null)
				{
					Text component5 = form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
					if (opType == enFriendGiftOpType.Present)
					{
						component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Give_Title"));
					}
					else
					{
						component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Hero_Friend_Title"));
					}
					Text component6 = form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
					component6.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
					Image component7 = form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
					component7.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, false, true, true, true);
					form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(false);
					Transform transform5 = form.transform.Find("Panel/skinPricePanel");
					Transform costIcon2 = transform5.Find("costImage");
					CHeroSkinBuyManager.SetPayCostIcon(form, costIcon2, enPayType.DianQuan);
					Transform costTypeText2 = transform5.Find("costTypeText");
					CHeroSkinBuyManager.SetPayCostTypeText(costTypeText2, enPayType.DianQuan);
					Transform transform6 = transform5.Find("costPanel");
					if (transform6)
					{
						IHeroData heroData = CHeroDataFactory.CreateHeroData(heroId);
						ResHeroPromotion resPromotion = heroData.promotion();
						stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
						for (int j = 0; j < payInfoSetOfGood.m_payInfoCount; j++)
						{
							if (payInfoSetOfGood.m_payInfos[j].m_payType == enPayType.Diamond || payInfoSetOfGood.m_payInfos[j].m_payType == enPayType.DianQuan || payInfoSetOfGood.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan)
							{
								num2 = payInfoSetOfGood.m_payInfos[j].m_payValue;
								num = payInfoSetOfGood.m_payInfos[j].m_oriValue;
								break;
							}
						}
						Transform transform7 = transform6.Find("oldPricePanel");
						if (transform7 != null)
						{
							transform7.gameObject.CustomSetActive(num != num2);
							Transform oldPrice2 = transform7.Find("oldPriceText");
							CHeroSkinBuyManager.SetPayOldPrice(oldPrice2, num);
						}
						Transform currentPrice2 = transform6.Find("costText");
						CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice2, num2);
					}
					Transform transform8 = form.transform.Find("Panel/TipTxt");
					if (transform8 != null)
					{
						Text component8 = transform8.GetComponent<Text>();
						if (component8 != null)
						{
							if (opType == enFriendGiftOpType.Present)
							{
								component8.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Buy_Hero_For_Friend_Tip"), num3));
							}
							else
							{
								component8.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Hero_Friend_Tip"), num3, srv2CltGlobalValue));
							}
						}
					}
				}
			}
			CUIEventScript component9 = form.transform.Find("Panel/SearchFriend/Button").GetComponent<CUIEventScript>();
			component9.m_onClickEventParams.friendHeroSkinPar.giftType = giftType;
			component9.m_onClickEventParams.friendHeroSkinPar.giftOpType = opType;
			component9.m_onClickEventParams.friendHeroSkinPar.heroId = heroId;
			component9.m_onClickEventParams.friendHeroSkinPar.skinId = skinId;
			component9.m_onClickEventParams.friendHeroSkinPar.price = num2;
			component9.m_onClickEventParams.friendHeroSkinPar.oldPrice = num;
			ListView<COMDT_FRIEND_INFO> filteredFriends = this.GetFilteredFriends(opType);
			CUIListScript component10 = form.transform.Find("Panel/List").GetComponent<CUIListScript>();
			this.UpdateFriendList(ref filteredFriends, ref component10, giftType, heroId, skinId, num2, num, friendUid, worldId, opType);
		}

		private ListView<COMDT_FRIEND_INFO> GetFilteredFriends(enFriendGiftOpType opType)
		{
			if (opType == enFriendGiftOpType.Present)
			{
				return Singleton<CFriendContoller>.GetInstance().model.GetAllFriend(true);
			}
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_ASKFOR_OPENSWITCH);
			bool flag = (srv2CltGlobalValue & 1u) > 0u;
			bool flag2 = (srv2CltGlobalValue & 2u) > 0u;
			bool flag3 = (srv2CltGlobalValue & 4u) > 0u;
			ListView<COMDT_FRIEND_INFO> listView = new ListView<COMDT_FRIEND_INFO>();
			ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.GameFriend);
			ListView<COMDT_FRIEND_INFO> list2 = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.SNS);
			ListView<COMDT_FRIEND_INFO> list3 = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.Mentor);
			ListView<COMDT_FRIEND_INFO> list4 = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.Apprentice);
			if (flag && flag2 && flag3)
			{
				listView.AddRange(list2);
				for (int i = 0; i < list.Count; i++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list[i], listView))
					{
						listView.Add(list[i]);
					}
				}
				for (int j = 0; j < list3.Count; j++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list3[j], listView))
					{
						listView.Add(list3[j]);
					}
				}
				for (int k = 0; k < list4.Count; k++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list4[k], listView))
					{
						listView.Add(list4[k]);
					}
				}
			}
			else if (flag && flag2)
			{
				listView.AddRange(list2);
				for (int l = 0; l < list.Count; l++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list[l], listView))
					{
						listView.Add(list[l]);
					}
				}
			}
			else if (flag && flag3)
			{
				listView.AddRange(list);
				for (int m = 0; m < list3.Count; m++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list3[m], listView))
					{
						listView.Add(list3[m]);
					}
				}
				for (int n = 0; n < list4.Count; n++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list4[n], listView))
					{
						listView.Add(list4[n]);
					}
				}
			}
			else if (flag2 && flag3)
			{
				listView.AddRange(list2);
				for (int num = 0; num < list3.Count; num++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list3[num], listView))
					{
						listView.Add(list3[num]);
					}
				}
				for (int num2 = 0; num2 < list4.Count; num2++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list4[num2], listView))
					{
						listView.Add(list4[num2]);
					}
				}
			}
			else if (flag2)
			{
				listView.AddRange(list2);
			}
			else if (flag)
			{
				listView.AddRange(list);
			}
			else if (flag3)
			{
				listView.AddRange(list3);
				for (int num3 = 0; num3 < list4.Count; num3++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(list4[num3], listView))
					{
						listView.Add(list4[num3]);
					}
				}
			}
			return listView;
		}

		private void InitLeaveMsgForFriendForm(CUIFormScript form, int friendIndex = 0)
		{
			uint[] conditionParam = Singleton<CFunctionUnlockSys>.instance.GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO, RES_UNLOCKCONDITION_TYPE.RES_UNLOCKCONDITIONTYPE_ABOVELEVEL);
			uint num = (conditionParam.Length > 1) ? conditionParam[0] : 1u;
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_ASKFOR_SHOWDAY);
			string tagStr = string.Empty;
			if (this.m_giftType == RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN)
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_buyHeroIDForFriend, this.m_buySkinIDForFriend);
				Text component = form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
				if (this.m_giftOpType == enFriendGiftOpType.Present)
				{
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Give_Title"));
				}
				else
				{
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Skin_Friend_Title"));
				}
				Image component2 = form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
				component2.SetSprite(prefabPath, form, false, true, true, true);
				Text component3 = form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
				string text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
				component3.set_text(text);
				tagStr = text;
				form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(true);
				GameObject gameObject = form.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
				CSkinInfo.GetHeroSkinProp(this.m_buyHeroIDForFriend, this.m_buySkinIDForFriend, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
				CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
				Transform transform = form.transform.Find("Panel/skinPricePanel");
				Transform costIcon = transform.Find("costImage");
				CHeroSkinBuyManager.SetPayCostIcon(form, costIcon, enPayType.DianQuan);
				Transform costTypeText = transform.Find("costTypeText");
				CHeroSkinBuyManager.SetPayCostTypeText(costTypeText, enPayType.DianQuan);
				Transform transform2 = transform.Find("costPanel");
				if (transform2 != null)
				{
					Transform transform3 = transform2.Find("oldPricePanel");
					if (transform3 != null)
					{
						transform3.gameObject.CustomSetActive(this.m_buyOldPriceForFriend != this.m_buyPriceForFriend);
						Transform oldPrice = transform3.Find("oldPriceText");
						CHeroSkinBuyManager.SetPayOldPrice(oldPrice, this.m_buyOldPriceForFriend);
					}
					Transform currentPrice = transform2.Find("costText");
					CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, this.m_buyPriceForFriend);
				}
				Transform transform4 = form.transform.Find("Panel/TipTxt");
				if (transform4 != null)
				{
					Text component4 = transform4.GetComponent<Text>();
					if (component4 != null)
					{
						if (this.m_giftOpType == enFriendGiftOpType.Present)
						{
							component4.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Buy_Skin_For_Friend_Tip"), num));
						}
						else
						{
							component4.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Skin_Friend_Title"), num, srv2CltGlobalValue));
						}
					}
				}
			}
			else
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_buyHeroIDForFriend);
				DebugHelper.Assert(dataByKey != null);
				if (dataByKey != null)
				{
					Text component5 = form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
					if (this.m_giftOpType == enFriendGiftOpType.Present)
					{
						component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Give_Title"));
					}
					else
					{
						component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Hero_Friend_Title"));
					}
					Text component6 = form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
					string text2 = StringHelper.UTF8BytesToString(ref dataByKey.szName);
					component6.set_text(text2);
					tagStr = text2;
					Image component7 = form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
					component7.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, false, true, true, true);
					form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(false);
					Transform transform5 = form.transform.Find("Panel/skinPricePanel");
					Transform costIcon2 = transform5.Find("costImage");
					CHeroSkinBuyManager.SetPayCostIcon(form, costIcon2, enPayType.DianQuan);
					Transform costTypeText2 = transform5.Find("costTypeText");
					CHeroSkinBuyManager.SetPayCostTypeText(costTypeText2, enPayType.DianQuan);
					Transform transform6 = transform5.Find("costPanel");
					if (transform6)
					{
						Transform transform7 = transform6.Find("oldPricePanel");
						if (transform7 != null)
						{
							transform7.gameObject.CustomSetActive(this.m_buyOldPriceForFriend != this.m_buyPriceForFriend);
							Transform oldPrice2 = transform7.Find("oldPriceText");
							CHeroSkinBuyManager.SetPayOldPrice(oldPrice2, this.m_buyOldPriceForFriend);
						}
						Transform currentPrice2 = transform6.Find("costText");
						CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice2, this.m_buyPriceForFriend);
					}
				}
				Transform transform8 = form.transform.Find("Panel/TipTxt");
				Text component8 = transform8.GetComponent<Text>();
				if (component8 != null)
				{
					if (this.m_giftOpType == enFriendGiftOpType.Present)
					{
						component8.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Buy_Hero_For_Friend_Tip"), num));
					}
					else
					{
						component8.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Hero_Friend_Title"), num, srv2CltGlobalValue));
					}
				}
			}
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "Panel/targetFriend");
			CHeroSkinBuyManager.RefreshFriendListElementForGift(gameObject2, this.m_friendList[friendIndex]);
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject2, "to");
			if (componetInChild != null)
			{
				if (this.m_giftOpType == enFriendGiftOpType.Present)
				{
					componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("Buy_For_To"));
				}
				else
				{
					componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_From"));
				}
			}
			GameObject gameObject3 = Utility.FindChild(form.gameObject, "Panel/InviteButton");
			if (gameObject3 != null)
			{
				Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject3, "Text");
				if (this.m_giftOpType == enFriendGiftOpType.Present)
				{
					componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Op"));
				}
				else
				{
					componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Friend_Op"));
				}
				CUIEventScript component9 = gameObject3.GetComponent<CUIEventScript>();
				component9.m_onClickEventParams.commonUInt64Param1 = this.m_friendList[friendIndex].stUin.ullUid;
				component9.m_onClickEventParams.tagUInt = this.m_friendList[friendIndex].stUin.dwLogicWorldId;
				component9.m_onClickEventParams.tagStr = CUIUtility.RemoveEmoji(Utility.UTF8Convert(this.m_friendList[friendIndex].szUserName));
				if (this.m_giftType == RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN)
				{
					if (this.m_giftOpType == enFriendGiftOpType.Present)
					{
						component9.m_onClickEventID = enUIEventID.HeroSkin_BuyHeroSkinForFriend;
					}
					else
					{
						component9.m_onClickEventID = enUIEventID.HeroSkin_AskForHeroSkin;
					}
					component9.m_onClickEventParams.heroSkinParam.heroId = this.m_buyHeroIDForFriend;
					component9.m_onClickEventParams.heroSkinParam.skinId = this.m_buySkinIDForFriend;
					component9.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
				}
				else
				{
					if (this.m_giftOpType == enFriendGiftOpType.Present)
					{
						component9.m_onClickEventID = enUIEventID.HeroView_BuyHeroForFriend;
					}
					else
					{
						component9.m_onClickEventID = enUIEventID.HeroView_AskForHero;
					}
					component9.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
					component9.m_onClickEventParams.tag = (int)this.m_buyHeroIDForFriend;
				}
			}
			InputField component10 = Utility.FindChild(form.gameObject, "Panel/msgContainer/InputField").GetComponent<InputField>();
			if (component10 != null)
			{
				component10.get_onValueChange().AddListener(new UnityAction<string>(this.On_InputFiled_ValueChange));
			}
			GameObject gameObject4 = Utility.FindChild(form.gameObject, "Panel/msgTypeContainer");
			if (this.m_giftOpType == enFriendGiftOpType.AskFor)
			{
				gameObject4.CustomSetActive(true);
				this.m_askForTemplateList.Clear();
				Dictionary<long, object>.Enumerator enumerator = GameDataMgr.askForTemplateDatabin.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, object> current = enumerator.get_Current();
					ResAskforTemplet resAskforTemplet = current.get_Value() as ResAskforTemplet;
					if (resAskforTemplet.wItemType == (ushort)this.m_giftType)
					{
						if (resAskforTemplet.dwID > this.m_maxAskForTemplateID)
						{
							this.m_maxAskForTemplateID = resAskforTemplet.dwID;
						}
						this.m_askForTemplateList.Add(resAskforTemplet);
					}
				}
				if (gameObject4 != null)
				{
					CUIListScript componetInChild3 = Utility.GetComponetInChild<CUIListScript>(gameObject4, "List");
					int count = this.m_askForTemplateList.Count;
					componetInChild3.SetElementAmount(count);
					for (int i = 0; i < count; i++)
					{
						CUIListElementScript elemenet = componetInChild3.GetElemenet(i);
						Text component11 = elemenet.GetWidget(0).GetComponent<Text>();
						if (component11 != null)
						{
							component11.set_text(Singleton<CTextManager>.GetInstance().GetText(string.Format("HeroSkinBuy_Ask_For_Template_Name_{0}", this.m_askForTemplateList[i].dwID)));
						}
					}
					componetInChild3.m_listSelectChangedEventID = enUIEventID.HeroSkin_LeaveMsgTypeSelect;
					componetInChild3.m_listSelectChangedEventParams.tagStr = tagStr;
					componetInChild3.SelectElement(0, true);
				}
			}
			else
			{
				gameObject4.CustomSetActive(false);
			}
		}

		private void On_InputFiled_ValueChange(string arg0)
		{
			string text = CUIUtility.RemoveEmoji(Utility.UTF8Convert(arg0));
			if (text.get_Length() > 50)
			{
				CUIFormScript form;
				if (this.m_giftOpType == enFriendGiftOpType.Present)
				{
					form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSkinBuyManager.s_leaveMsgPath);
				}
				else
				{
					form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSkinBuyManager.s_leaveMsgAskForPath);
				}
				if (form == null)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), 50), false);
					return;
				}
				InputField component = Utility.FindChild(form.gameObject, "Panel/msgContainer/InputField").GetComponent<InputField>();
				if (component != null)
				{
					component.DeactivateInputField();
					component.set_text(text.Substring(0, 50));
					CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/Common/Form_MessageBox.prefab");
					if (form2 == null)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), 50), false);
					}
				}
			}
		}

		private void UpdateFriendList(ref ListView<COMDT_FRIEND_INFO> allFriends, ref CUIListScript list, RES_ITEM_TYPE giftType, uint heroId, uint skinId, uint price, uint oldPrice, ulong friendUid = 0uL, uint worldId = 0u, enFriendGiftOpType opType = enFriendGiftOpType.Present)
		{
			if (friendUid > 0uL && worldId > 0u)
			{
				this.detailFriendList.Clear();
				for (int i = 0; i < allFriends.Count; i++)
				{
					if (allFriends[i].stUin.ullUid == friendUid && allFriends[i].stUin.dwLogicWorldId == worldId)
					{
						this.detailFriendList.Add(allFriends[i]);
						this.m_friendList = this.detailFriendList;
						break;
					}
				}
			}
			else
			{
				this.m_friendList = allFriends;
			}
			this.m_giftType = giftType;
			this.m_giftOpType = opType;
			this.m_buyHeroIDForFriend = heroId;
			this.m_buySkinIDForFriend = skinId;
			this.m_buyPriceForFriend = price;
			this.m_buyOldPriceForFriend = oldPrice;
			list.SetElementAmount(0);
			list.SetElementAmount(this.m_friendList.Count);
		}

		private void OnFriendListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			CHeroSkinBuyManager.RefreshFriendListElementForGift(uiEvent.m_srcWidget, this.m_friendList[srcWidgetIndexInBelongedList]);
			GameObject gameObject = Utility.FindChild(uiEvent.m_srcWidget, "InviteButton");
			if (gameObject != null)
			{
				Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "Text");
				if (this.m_giftOpType == enFriendGiftOpType.Present)
				{
					componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Op"));
				}
				else
				{
					componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Friend_Op"));
				}
				CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
				if (this.m_giftType == RES_ITEM_TYPE.RES_ITEMTYPE_HEROSKIN)
				{
					component.m_onClickEventID = enUIEventID.HeroSkin_LeaveMsgForFriend;
					component.m_onClickEventParams.tag2 = srcWidgetIndexInBelongedList;
				}
				else
				{
					component.m_onClickEventID = enUIEventID.HeroView_LeaveMsgForFriend;
					component.m_onClickEventParams.tag2 = srcWidgetIndexInBelongedList;
				}
			}
		}

		private void OnCloseBuySkinForm(CUIEvent uiEvent)
		{
			Singleton<CHeroSelectNormalSystem>.instance.ResetHero3DObj();
		}

		public static void RefreshFriendListElementForGift(GameObject element, COMDT_FRIEND_INFO friend)
		{
			CInviteView.UpdateFriendListElementBase(element, ref friend);
			Transform transform = element.transform.Find("Gender");
			if (transform)
			{
				COM_SNSGENDER bGender = (COM_SNSGENDER)friend.bGender;
				transform.gameObject.CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
				if (bGender == COM_SNSGENDER.COM_SNSGENDER_MALE)
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				}
				else if (bGender == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
				{
					CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				}
			}
		}

		public static bool ShouldShowBuyForFriend(bool bSkin, uint heroId, uint skinId = 0u, bool forceNotShow = false)
		{
			if (forceNotShow)
			{
				return false;
			}
			if (bSkin)
			{
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
				DebugHelper.Assert(heroSkin != null);
				if (heroSkin != null)
				{
					stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroSkin);
					return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && GameDataMgr.IsSkinCanBuyNow(heroSkin.dwID) && GameDataMgr.IsSkinCanBuyForFriend(heroSkin.dwID);
				}
			}
			else
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
				DebugHelper.Assert(dataByKey != null);
				if (dataByKey != null)
				{
					stPayInfoSet payInfoSetOfGood2 = CMallSystem.GetPayInfoSetOfGood(dataByKey);
					bool flag = false;
					for (int i = 0; i < payInfoSetOfGood2.m_payInfoCount; i++)
					{
						if (payInfoSetOfGood2.m_payInfos[i].m_payType == enPayType.Diamond || payInfoSetOfGood2.m_payInfos[i].m_payType == enPayType.DianQuan || payInfoSetOfGood2.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)
						{
							flag = true;
							break;
						}
					}
					return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && flag && GameDataMgr.IsHeroAvailableAtShop(dataByKey.dwCfgID) && GameDataMgr.IsHeroCanBuyForFriend(dataByKey.dwCfgID);
				}
			}
			return false;
		}

		private void OnHeroInfo_SearchFriend(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			CUIListScript component = srcFormScript.transform.Find("Panel/List").GetComponent<CUIListScript>();
			InputField component2 = srcFormScript.transform.Find("Panel/SearchFriend/InputField").GetComponent<InputField>();
			if (component2)
			{
				ListView<COMDT_FRIEND_INFO> listView = this.GetFilteredFriends(this.m_giftOpType);
				if (component2.get_text() != string.Empty)
				{
					ListView<COMDT_FRIEND_INFO> listView2 = listView;
					listView = new ListView<COMDT_FRIEND_INFO>();
					for (int i = 0; i < listView2.Count; i++)
					{
						COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = listView2[i];
						if (StringHelper.UTF8BytesToString(ref cOMDT_FRIEND_INFO.szUserName).Contains(component2.get_text()))
						{
							listView.Add(cOMDT_FRIEND_INFO);
						}
					}
				}
				RES_ITEM_TYPE giftType = uiEvent.m_eventParams.friendHeroSkinPar.giftType;
				enFriendGiftOpType giftOpType = uiEvent.m_eventParams.friendHeroSkinPar.giftOpType;
				uint heroId = uiEvent.m_eventParams.friendHeroSkinPar.heroId;
				uint skinId = uiEvent.m_eventParams.friendHeroSkinPar.skinId;
				uint price = uiEvent.m_eventParams.friendHeroSkinPar.price;
				uint oldPrice = uiEvent.m_eventParams.friendHeroSkinPar.oldPrice;
				if (listView.Count == 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Friend_SearchNoResult"), false, 1.5f, null, new object[0]);
				}
				this.UpdateFriendList(ref listView, ref component, giftType, heroId, skinId, price, oldPrice, 0uL, 0u, giftOpType);
			}
		}

		[MessageHandler(1818)]
		public static void OnBuyHero(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stBuyHeroRsp.iResult == 0)
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(msg.stPkgData.stBuyHeroRsp.dwHeroID);
				DebugHelper.Assert(dataByKey != null);
				Singleton<CHeroInfoSystem2>.GetInstance().OnNtyAddHero(msg.stPkgData.stBuyHeroRsp.dwHeroID);
				CUICommonSystem.ShowNewHeroOrSkin(msg.stPkgData.stBuyHeroRsp.dwHeroID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1818, msg.stPkgData.stBuyHeroRsp.iResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1820)]
		public static void OnBuyHeroSkinRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_BUYHEROSKIN_RSP stBuyHeroSkinRsp = msg.stPkgData.stBuyHeroSkinRsp;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "OnBuyHeroSkinRsp::Master Role Info Is Null");
			if (masterRoleInfo == null)
			{
				return;
			}
			if (stBuyHeroSkinRsp.iResult == 0)
			{
				masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
				Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
				CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
			}
			else
			{
				CS_ADDHEROSKIN_ERRCODE iResult = (CS_ADDHEROSKIN_ERRCODE)stBuyHeroSkinRsp.iResult;
				CTextManager instance = Singleton<CTextManager>.GetInstance();
				switch (iResult)
				{
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKININVALID:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Invalid"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_PROMOTION:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongSale"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_BUYFAIL:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongMethod"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_NOHERO:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_NoHero"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKINHAS:
					if (stBuyHeroSkinRsp.dwHeroID != 0u && stBuyHeroSkinRsp.dwSkinID != 0u && !masterRoleInfo.IsHaveHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, false))
					{
						masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
						Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
						CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
						return;
					}
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_AlreadyHave"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COINLIMIT:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Money"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COUPONS:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Dianjuan"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_OTHER:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Other"), false, 1.5f, null, new object[0]);
					break;
				case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_RANKGRADE:
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_RankGrade"), false, 1.5f, null, new object[0]);
					break;
				default:
					Singleton<CUIManager>.GetInstance().OpenTips(iResult.ToString(), false, 1.5f, null, new object[0]);
					break;
				}
			}
		}

		[MessageHandler(1831)]
		public static void OnBuyHeroForFriend(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stPresentHeroRsp.iResult == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Success"), false);
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1833, msg.stPkgData.stPresentHeroRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1833)]
		public static void OnBuySkinForFriend(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stPresentSkinRsp.iResult == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Success"), false);
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1833, msg.stPkgData.stPresentSkinRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1385)]
		public static void OnSendAskForRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAskforReqSendRsp.iResult == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Ask_For_Send_Success", true, 1.5f, null, new object[0]);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					DebugHelper.Assert(false, "Master Role Info is null");
					return;
				}
				masterRoleInfo.SendAskforReqCnt += 1u;
				masterRoleInfo.SendAskforReqTime = (uint)CRoleInfo.GetCurrentUTCTime();
			}
			else
			{
				string strContent = Utility.ProtErrCodeToStr(1385, msg.stPkgData.stAskforReqSendRsp.iResult);
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
			}
		}
	}
}

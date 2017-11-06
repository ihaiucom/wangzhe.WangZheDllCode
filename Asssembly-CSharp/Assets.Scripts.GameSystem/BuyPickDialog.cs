using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BuyPickDialog
	{
		public delegate void OnConfirmBuyDelegate(CMallFactoryShopController.ShopProduct shopProduct, uint count, bool needConfirm, CUIEvent uiEvent);

		public delegate void OnConfirmBuyCommonDelegate(CUIEvent uievent, uint count);

		public static string s_Form_Path = "UGUI/Form/Common/Form_BuyPick.prefab";

		public static string s_Gift_Form_Path = "UGUI/Form/Common/Form_BuyPick_Gift.prefab";

		public static string s_Gift_Big_Icon_Form_Path = "UGUI/Form/Common/Form_BuyPick_Gift_Big_Icon.prefab";

		private GameObject _root;

		private Text _countText;

		private Text _costText;

		private Text _descText;

		private CUseable _usb;

		private CUseable _coinUsb;

		private RES_SHOPBUY_COINTYPE _coinType;

		private float _realDiscount;

		private uint _count;

		private uint _maxCount;

		private bool _bHeroSkinGift;

		private bool _bDynamicCorrectPrice;

		private uint _heroSkinGiftCost;

		private BuyPickDialog.OnConfirmBuyDelegate _onConfirm;

		private CMallFactoryShopController.ShopProduct _callContext;

		private BuyPickDialog.OnConfirmBuyCommonDelegate _onConfirmdCommon;

		private CUIEvent _uieventPars;

		private bool m_bShowBigIcon;

		private static BuyPickDialog s_theDlg;

		public BuyPickDialog(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, BuyPickDialog.OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, BuyPickDialog.OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(BuyPickDialog.s_Form_Path, false, true);
			if (null != cUIFormScript)
			{
				this._root = cUIFormScript.gameObject;
				this._usb = CUseableManager.CreateUseable(type, id, 0);
				this._bHeroSkinGift = false;
				this._bDynamicCorrectPrice = false;
				this._heroSkinGiftCost = 0u;
				this._count = 1u;
				this._maxCount = maxCount;
				if (this._maxCount == 0u)
				{
					this._maxCount = 999u;
				}
				this._onConfirm = onConfirm;
				this._callContext = callContext;
				this._onConfirmdCommon = onConfirmCommon;
				this._uieventPars = uieventPars;
				this._coinType = coinType;
				this._realDiscount = discount;
				if (this._usb != null)
				{
					this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
					this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
					this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/Desc/Image/Text");
					if (this._descText != null)
					{
						this._descText.set_text(string.IsNullOrEmpty(this._usb.m_mallDescription) ? this._usb.m_description : this._usb.m_mallDescription);
					}
					Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/Icon").SetSprite(CUIUtility.GetSpritePrefeb(this._usb.GetIconPath(), false, false), false);
					Utility.GetComponetInChild<Text>(this._root, "Panel/Name").set_text(this._usb.m_name);
					this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
					if (this._coinUsb != null)
					{
						Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
						Utility.GetComponetInChild<Text>(this._root, "Panel/Price").set_text(CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._usb.GetBuyPrice(coinType), this._realDiscount).ToString());
					}
					Image componetInChild = Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/imgExperienceMark");
					if (componetInChild != null)
					{
						if (this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(this._usb.m_baseID))
						{
							componetInChild.gameObject.CustomSetActive(true);
							componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
						}
						else if (this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(this._usb.m_baseID))
						{
							componetInChild.gameObject.CustomSetActive(true);
							componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
						}
						else
						{
							componetInChild.gameObject.CustomSetActive(false);
						}
					}
				}
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
				this.ValidateDynamic();
			}
		}

		public BuyPickDialog(bool isGift, COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, BuyPickDialog.OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, BuyPickDialog.OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null, bool bfromFactoyShop = false)
		{
			this.m_bShowBigIcon = (callContext != null && callContext.GetSpecialIconPath() != null);
			CUIFormScript cUIFormScript;
			if (this.m_bShowBigIcon)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(BuyPickDialog.s_Gift_Big_Icon_Form_Path, false, true);
			}
			else
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(BuyPickDialog.s_Gift_Form_Path, false, true);
			}
			if (null != cUIFormScript)
			{
				this._root = cUIFormScript.gameObject;
				this._usb = CUseableManager.CreateUseable(type, id, 0);
				this._count = 1u;
				this._bHeroSkinGift = false;
				this._bDynamicCorrectPrice = false;
				this._heroSkinGiftCost = 0u;
				this._maxCount = maxCount;
				if (this._maxCount == 0u)
				{
					this._maxCount = 999u;
				}
				this._onConfirm = onConfirm;
				this._callContext = callContext;
				this._onConfirmdCommon = onConfirmCommon;
				this._uieventPars = uieventPars;
				this._coinType = coinType;
				this._realDiscount = discount;
				if (this._usb != null)
				{
					this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
					this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
					this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/lblDesc");
					CItem cItem = new CItem(0uL, id, 0, 0);
					uint key = (uint)cItem.m_itemData.EftParam[0];
					ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(key);
					ListView<CUseable> listView = new ListView<CUseable>();
					for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
					{
						if (dataByKey.astRewardDetail[i].bItemType != 0)
						{
							CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[i].bItemType, (int)dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
							if (cUseable != null)
							{
								listView.Add(cUseable);
							}
						}
					}
					if (this._descText != null)
					{
						this._descText.set_text(string.IsNullOrEmpty(cItem.m_mallDescription) ? cItem.m_description : cItem.m_mallDescription);
					}
					uint num = 0u;
					int num2 = 0;
					if (this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem2 = (CItem)this._usb;
						if (cItem2 != null && cItem2.m_itemData != null && cItem2.m_itemData.bType == 4)
						{
							this._bDynamicCorrectPrice = (cItem2.m_itemData.EftParam[3] > 0f);
						}
					}
					CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(cUIFormScript.gameObject, "Panel/itemGroup");
					componetInChild.SetElementAmount(listView.Count);
					for (int j = 0; j < listView.Count; j++)
					{
						CUIListElementScript elemenet = componetInChild.GetElemenet(j);
						this.UpdateElement(elemenet, listView[j], this.m_bShowBigIcon);
						if (listView[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
						{
							this._bHeroSkinGift = true;
							CHeroItem cHeroItem = listView[j] as CHeroItem;
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo != null && masterRoleInfo.IsOwnHero(cHeroItem.m_heroData.dwCfgID))
							{
								num += CHeroInfo.GetHeroCost(cHeroItem.m_heroData.dwCfgID, coinType);
								num2++;
							}
						}
						else if (listView[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
						{
							this._bHeroSkinGift = true;
							CHeroSkin cHeroSkin = listView[j] as CHeroSkin;
							CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo2 != null && masterRoleInfo2.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false))
							{
								num += CSkinInfo.GetHeroSkinCost(cHeroSkin.m_heroId, cHeroSkin.m_skinId, coinType);
								num2++;
							}
						}
					}
					this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
					if (this._coinUsb != null)
					{
						Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
					}
					Text componetInChild2 = Utility.GetComponetInChild<Text>(this._root, "Panel/costDescText");
					componetInChild2.set_text(string.Empty);
					if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
					{
						uint buyPrice = this._usb.GetBuyPrice(coinType);
						Button componetInChild3 = Utility.GetComponetInChild<Button>(this._root, "Panel/Button_Sale");
						if (num2 >= listView.Count)
						{
							CUICommonSystem.SetButtonEnableWithShader(componetInChild3, false, true);
							componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("Gift_Can_Not_Buy_Tip"));
							this._heroSkinGiftCost = 0u;
						}
						else
						{
							CUICommonSystem.SetButtonEnableWithShader(componetInChild3, true, true);
							componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("Gift_Own_Hero_Skin_Tip"));
							uint num3 = CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(buyPrice - num, this._realDiscount);
							if (buyPrice >= num && num3 >= buyPrice / 10u)
							{
								this._heroSkinGiftCost = num3;
							}
							else
							{
								this._heroSkinGiftCost = buyPrice / 10u;
							}
						}
						if (this._callContext != null)
						{
							this._callContext.m_bChangeGiftPrice = true;
							this._callContext.m_newGiftPrice = this._heroSkinGiftCost;
						}
					}
				}
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
				this.ValidateDynamic();
			}
		}

		private void UpdateElement(CUIListElementScript elementScript, CUseable useable, bool isShowBigIcon)
		{
			CUIFormScript belongedFormScript = elementScript.m_belongedFormScript;
			GameObject widget = elementScript.GetWidget(0);
			GameObject widget2 = elementScript.GetWidget(1);
			GameObject widget3 = elementScript.GetWidget(2);
			if (useable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_HERO && useable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				isShowBigIcon = false;
			}
			if (!isShowBigIcon)
			{
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
				{
					CHeroItem cHeroItem = useable as CHeroItem;
					if (cHeroItem == null)
					{
						return;
					}
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null && masterRoleInfo.IsOwnHero(cHeroItem.m_heroData.dwCfgID))
					{
						widget3.CustomSetActive(true);
					}
					else
					{
						widget3.CustomSetActive(false);
					}
				}
				else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
				{
					CHeroSkin cHeroSkin = useable as CHeroSkin;
					if (cHeroSkin == null)
					{
						return;
					}
					CRoleInfo masterRoleInfo2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo2 != null && masterRoleInfo2.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false))
					{
						widget3.CustomSetActive(true);
					}
					else
					{
						widget3.CustomSetActive(false);
					}
				}
				else
				{
					widget3.CustomSetActive(false);
				}
				CUICommonSystem.SetItemCell(elementScript.m_belongedFormScript, elementScript.GetWidget(0), useable, true, false, false, false);
			}
			else
			{
				widget.CustomSetActive(false);
				widget2.CustomSetActive(true);
				Image componetInChild = Utility.GetComponetInChild<Image>(widget2, "imageIcon");
				GameObject gameObject = Utility.FindChild(widget2, "skinLabelImage");
				GameObject gameObject2 = Utility.FindChild(widget2, "nameContainer/heroNameText");
				Text component = gameObject2.GetComponent<Text>();
				GameObject gameObject3 = Utility.FindChild(widget2, "nameContainer/heroSkinText");
				Text component2 = gameObject3.GetComponent<Text>();
				if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
				{
					CHeroItem cHeroItem2 = useable as CHeroItem;
					if (cHeroItem2 == null)
					{
						return;
					}
					CRoleInfo masterRoleInfo3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo3 != null && masterRoleInfo3.IsOwnHero(cHeroItem2.m_heroData.dwCfgID))
					{
						widget3.CustomSetActive(true);
					}
					else
					{
						widget3.CustomSetActive(false);
					}
					string prefabPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + cHeroItem2.m_iconID;
					componetInChild.SetSprite(prefabPath, belongedFormScript, false, true, true, true);
					gameObject2.CustomSetActive(true);
					component.set_text(useable.m_name);
					gameObject.CustomSetActive(false);
					gameObject3.CustomSetActive(false);
				}
				else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
				{
					CHeroSkin cHeroSkin2 = useable as CHeroSkin;
					if (cHeroSkin2 == null)
					{
						return;
					}
					IHeroData heroData = CHeroDataFactory.CreateHeroData(cHeroSkin2.m_heroId);
					CRoleInfo masterRoleInfo4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo4 != null && masterRoleInfo4.IsHaveHeroSkin(cHeroSkin2.m_heroId, cHeroSkin2.m_skinId, false))
					{
						widget3.CustomSetActive(true);
					}
					else
					{
						widget3.CustomSetActive(false);
					}
					string prefabPath2 = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + cHeroSkin2.m_iconID;
					componetInChild.SetSprite(prefabPath2, belongedFormScript, false, true, true, true);
					gameObject2.CustomSetActive(true);
					component.set_text(heroData.heroName);
					gameObject.CustomSetActive(true);
					CUICommonSystem.SetHeroSkinLabelPic(belongedFormScript, gameObject, cHeroSkin2.m_heroId, cHeroSkin2.m_skinId);
					gameObject3.CustomSetActive(true);
					component2.set_text(useable.m_name);
				}
			}
		}

		private void ValidateDynamic()
		{
			if (this._usb != null)
			{
				this._countText.set_text(this._count.ToString());
				if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
				{
					this._costText.set_text((this._count * this._heroSkinGiftCost).ToString());
				}
				else
				{
					this._costText.set_text(CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._count * this._usb.GetBuyPrice(this._coinType), this._realDiscount).ToString());
				}
			}
		}

		private void OnClickAdd(CUIEvent uiEvent)
		{
			if (this._count < this._maxCount)
			{
				this._count += 1u;
				this.ValidateDynamic();
			}
		}

		private void OnClickDec(CUIEvent uiEvent)
		{
			if (this._count > 1u)
			{
				this._count -= 1u;
				this.ValidateDynamic();
			}
		}

		private void OnClickMax(CUIEvent uiEvent)
		{
			if (this._count != this._maxCount)
			{
				this._count = this._maxCount;
				this.ValidateDynamic();
			}
		}

		private void OnConfirmFactoryShopBuy(CUIEvent uiEvent)
		{
			this.OnClose(true);
		}

		private void OnClickConfirm(CUIEvent uiEvent)
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				this.OnClose(false);
				return;
			}
			if (this._callContext != null)
			{
				string text = this._callContext.isHaveHeroSkin();
				if (text != string.Empty)
				{
					string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("FactoyBuy_Tips"), text);
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPick_ConfirmFactoryShopBuy, enUIEventID.BuyPick_Cancel, false);
					return;
				}
			}
			this.OnClose(true);
		}

		private void OnClickCancel(CUIEvent uiEvent)
		{
			this.OnClose(false);
		}

		private void OnClose(bool isOk)
		{
			if (isOk && this._onConfirm != null)
			{
				this._onConfirm(this._callContext, this._count, false, this._uieventPars);
			}
			else if (isOk && this._onConfirmdCommon != null)
			{
				this._onConfirmdCommon(this._uieventPars, this._count);
			}
			Singleton<CUIManager>.GetInstance().CloseForm(BuyPickDialog.s_Form_Path);
			Singleton<CUIManager>.GetInstance().CloseForm(BuyPickDialog.s_Gift_Form_Path);
			Singleton<CUIManager>.GetInstance().CloseForm(BuyPickDialog.s_Gift_Big_Icon_Form_Path);
		}

		private void OnCloseForm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
			BuyPickDialog.s_theDlg = null;
		}

		public static void Show(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, BuyPickDialog.OnConfirmBuyDelegate onClose, CMallFactoryShopController.ShopProduct callContext = null, BuyPickDialog.OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
		{
			if (BuyPickDialog.s_theDlg == null)
			{
				if (type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = new CItem(0uL, id, 0, 0);
					if (cItem.m_itemData.bIsView != 0)
					{
						BuyPickDialog.s_theDlg = new BuyPickDialog(true, type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars, false);
					}
					else
					{
						BuyPickDialog.s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
					}
				}
				else
				{
					BuyPickDialog.s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
				}
				if (BuyPickDialog.s_theDlg._root == null)
				{
					BuyPickDialog.s_theDlg = null;
				}
			}
		}
	}
}

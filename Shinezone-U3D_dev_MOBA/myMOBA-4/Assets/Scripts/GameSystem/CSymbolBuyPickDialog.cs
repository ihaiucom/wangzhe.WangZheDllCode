using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CSymbolBuyPickDialog
	{
		public delegate void OnConfirmBuyDelegate(ResSymbolInfo symbol, uint count, bool needConfirm, CUIEvent uiEvent);

		public delegate void OnConfirmBuyCommonDelegate(CUIEvent uievent, uint count);

		public const uint MAX_SYMBOL_CNT = 10u;

		public static string s_Form_Path = "UGUI/Form/System/Symbol/Form_BuyPick_Symbol.prefab";

		private GameObject _root;

		private GameObject _itemCell;

		private Text _countText;

		private Text _costText;

		private Text _descText;

		private CSymbolItem _usb;

		private CUseable _coinUsb;

		private RES_SHOPBUY_COINTYPE _coinType;

		private float _realDiscount;

		private uint _count;

		private uint _maxCount;

		private ResSymbolInfo _callContext;

		private CSymbolBuyPickDialog.OnConfirmBuyDelegate _onConfirm;

		private CSymbolBuyPickDialog.OnConfirmBuyCommonDelegate _onConfirmdCommon;

		private CUIEvent _uieventPars;

		private static CSymbolBuyPickDialog s_theDlg;

		public CSymbolBuyPickDialog(ResSymbolInfo symbolInfo, RES_SHOPBUY_COINTYPE coinType, float discount, CSymbolBuyPickDialog.OnConfirmBuyDelegate onConfirm, CSymbolBuyPickDialog.OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			this._usb = (useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID) as CSymbolItem);
			if (this._usb == null)
			{
				this._usb = (CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID, 0) as CSymbolItem);
			}
			if (this._usb == null)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolBuyPickDialog.s_Form_Path, false, true);
			if (null != cUIFormScript)
			{
				this._root = cUIFormScript.gameObject;
				this._itemCell = Utility.FindChild(this._root, "Panel/itemCell");
				this._callContext = symbolInfo;
				this._count = 1u;
				if ((long)this._usb.m_stackCount >= 10L)
				{
					this._maxCount = 1u;
				}
				else
				{
					this._maxCount = (uint)(10 - this._usb.m_stackCount);
				}
				this._onConfirm = onConfirm;
				this._onConfirmdCommon = onConfirmCommon;
				this._uieventPars = uieventPars;
				this._coinType = coinType;
				this._realDiscount = discount;
				if (this._usb != null)
				{
					this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
					this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
					CUICommonSystem.SetItemCell(cUIFormScript, this._itemCell, this._usb, true, false, false, false);
					this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/itemCell/SymbolDesc");
					if (this._descText != null)
					{
						this._descText.text = CSymbolSystem.GetSymbolAttString(symbolInfo.dwID, true);
					}
					this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
					if (this._coinUsb != null)
					{
						Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
						Utility.GetComponetInChild<Text>(this._root, "Panel/Price").text = this._usb.GetBuyPrice(this._coinType).ToString();
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

		private void ValidateDynamic()
		{
			if (this._usb != null)
			{
				this._countText.text = this._count.ToString();
				this._costText.text = (this._count * this._usb.GetBuyPrice(this._coinType)).ToString();
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
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				this.OnClose(false);
				return;
			}
			if (this._usb == null)
			{
				this.OnClose(false);
				return;
			}
			if (masterRoleInfo.SymbolCoin < this._count * this._usb.GetBuyPrice(this._coinType))
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Not_Enough_Tip");
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Symbol_Jump_To_MiShu, enUIEventID.None, false);
				this.OnClose(false);
				return;
			}
			if ((long)this._usb.m_stackCount + (long)((ulong)this._count) > 10L)
			{
				string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Buy_Count_Limit"), this._usb.m_stackCount, this._usb.m_name);
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPick_ConfirmFactoryShopBuy, enUIEventID.BuyPick_Cancel, false);
				return;
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
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolBuyPickDialog.s_Form_Path);
			CSymbolBuyPickDialog.s_theDlg = null;
		}

		private void OnCloseForm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
			CSymbolBuyPickDialog.s_theDlg = null;
		}

		public static void Show(ResSymbolInfo symbolInfo, RES_SHOPBUY_COINTYPE coinType, float discount, CSymbolBuyPickDialog.OnConfirmBuyDelegate onClose, CSymbolBuyPickDialog.OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
		{
			if (CSymbolBuyPickDialog.s_theDlg == null)
			{
				CSymbolBuyPickDialog.s_theDlg = new CSymbolBuyPickDialog(symbolInfo, coinType, discount, onClose, onConfirmCommon, uieventPars);
				if (CSymbolBuyPickDialog.s_theDlg._root == null)
				{
					CSymbolBuyPickDialog.s_theDlg = null;
				}
			}
		}
	}
}

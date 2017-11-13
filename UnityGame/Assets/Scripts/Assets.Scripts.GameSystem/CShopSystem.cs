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
	[MessageHandlerClass]
	public class CShopSystem : Singleton<CShopSystem>
	{
		public enum enShopFormWidget
		{
			Info_Panel,
			Items_Panel,
			Tab,
			TopBar,
			Left_Refresh_Count_Text,
			Sub_Tab,
			Manual_Refresh_Panel
		}

		public enum Tab
		{
			Fix_Shop,
			Pvp_Symbol_Shop,
			Burning_Exp_Shop,
			Arena_Shop,
			Guild_Shop
		}

		public enum SubTab
		{
			Guild_Item_Shop,
			Guild_HeadImage_Shop
		}

		public const ushort AUTO_REFRESH_WAITING_TIME = 1500;

		public const byte ITEMS_PERPAGE = 6;

		public string sShopFormPath = "UGUI/Form/System/Shop/Form_Shop.prefab";

		public string sMysteryShopFormPath = "UGUI/Form/System/Shop/Form_Mystery_Shop.prefab";

		public string sShopBuyFormPath = "UGUI/Form/System/Shop/Form_Shop_Buy_Item.prefab";

		public int MAX_SALE_ITEM_CNT = 2;

		public CUseable[] aSaleItems;

		public ushort m_saleItemsCnt;

		public bool m_isMysteryShopOpenedOnce;

		public bool m_isMysteryShopRefused;

		public CUIFormScript m_ShopForm;

		public bool m_IsShopFormOpen;

		public int m_currentSelectItemIdx;

		public Dictionary<uint, stShopInfo> m_Shops;

		public DictionaryView<uint, stShopItemInfo[]> m_ShopItems;

		public RES_SHOP_TYPE m_CurShopType;

		public RES_SHOP_TYPE m_LastNormalShop;

		private CShopSystem.Tab m_CurTab;

		private byte m_CurPage;

		private int m_TimerSeq;

		private bool m_IsNormalShopItemsInited;

		public static uint s_CoinShowMaxValue = 990000u;

		public static uint s_CoinShowStepValue = 10000u;

		public CShopSystem.Tab CurTab
		{
			get
			{
				return this.m_CurTab;
			}
			set
			{
				this.m_CurTab = value;
				switch (this.m_CurTab)
				{
				case CShopSystem.Tab.Fix_Shop:
					this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_FIXED;
					break;
				case CShopSystem.Tab.Pvp_Symbol_Shop:
					this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL;
					break;
				case CShopSystem.Tab.Burning_Exp_Shop:
					this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_BURNING;
					break;
				case CShopSystem.Tab.Arena_Shop:
					this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_ARENA;
					break;
				case CShopSystem.Tab.Guild_Shop:
					this.m_CurShopType = ((this.GetSubTabSelectedIndex() == 1) ? RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE : RES_SHOP_TYPE.RES_SHOPTYPE_GUILD);
					break;
				}
			}
		}

		public CShopSystem()
		{
			this.m_IsShopFormOpen = false;
			this.m_isMysteryShopOpenedOnce = false;
			this.m_isMysteryShopRefused = false;
			this.m_currentSelectItemIdx = -1;
			this.m_CurTab = CShopSystem.Tab.Fix_Shop;
			this.m_IsNormalShopItemsInited = false;
		}

		public bool IsNormalShopItemsInited()
		{
			return this.m_IsNormalShopItemsInited;
		}

		public override void Init()
		{
			base.Init();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Shop_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenForm));
			instance.AddUIEventListener(enUIEventID.Shop_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseForm));
			instance.AddUIEventListener(enUIEventID.Shop_SelectTab, new CUIEventManager.OnUIEventHandler(this.OnShop_Tab_Change));
			instance.AddUIEventListener(enUIEventID.Shop_SelectSubTab, new CUIEventManager.OnUIEventHandler(this.OnShop_SubTab_Change));
			instance.AddUIEventListener(enUIEventID.Shop_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnShop_SelectItem));
			instance.AddUIEventListener(enUIEventID.Shop_CloseItemForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseItemBuyForm));
			instance.AddUIEventListener(enUIEventID.Shop_BuyItem, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItem));
			instance.AddUIEventListener(enUIEventID.Shop_ManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ManualRefresh));
			instance.AddUIEventListener(enUIEventID.Shop_ConfirmManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ConfirmManualRefresh));
			instance.AddUIEventListener(enUIEventID.Shop_SaleTipCancel, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipCancel));
			instance.AddUIEventListener(enUIEventID.Shop_SaleTipConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipConfirm));
			instance.AddUIEventListener(enUIEventID.Shop_AutoRefreshTimerTimeUp, new CUIEventManager.OnUIEventHandler(this.OnShop_AutoRefreshTimerTimeUp));
			instance.AddUIEventListener(enUIEventID.Shop_ReturnToShopForm, new CUIEventManager.OnUIEventHandler(this.OnShop_ReturnToShopForm));
			instance.AddUIEventListener(enUIEventID.Shop_OpenGuildShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenGuildShopForm));
			instance.AddUIEventListener(enUIEventID.Shop_GetBurningCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoin));
			instance.AddUIEventListener(enUIEventID.Shop_GetBurningCoinConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoinConfirm));
			instance.AddUIEventListener(enUIEventID.Shop_GetArenaCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoin));
			instance.AddUIEventListener(enUIEventID.Shop_GetArenaCoinConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoinConfirm));
			instance.AddUIEventListener(enUIEventID.Shop_Page_Up, new CUIEventManager.OnUIEventHandler(this.OnShop_PageUp));
			instance.AddUIEventListener(enUIEventID.Shop_Page_Down, new CUIEventManager.OnUIEventHandler(this.OnShop_PageDown));
			instance.AddUIEventListener(enUIEventID.Shop_OpenArenaShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenArenaShopForm));
			instance.AddUIEventListener(enUIEventID.Shop_OpenBurningShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenBurningShopForm));
			instance.AddUIEventListener(enUIEventID.Shop_BuyItem_Confirm, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItemConfirm));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Shop_Auto_Refresh_Shop_Items, new Action(this.OnRequestAutoRefreshShopItems));
			Singleton<EventRouter>.GetInstance().AddEventHandler<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, new Action<RES_SHOP_TYPE>(this.OnReceiveShopItems));
			Singleton<EventRouter>.instance.AddEventHandler("MasterAttributesChanged", new Action(this.MasterAttrChanged));
			this.m_Shops = new Dictionary<uint, stShopInfo>();
			this.m_ShopItems = new DictionaryView<uint, stShopItemInfo[]>();
			this.m_TimerSeq = 0;
		}

		public override void UnInit()
		{
			base.UnInit();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Shop_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_SelectTab, new CUIEventManager.OnUIEventHandler(this.OnShop_Tab_Change));
			instance.RemoveUIEventListener(enUIEventID.Shop_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnShop_SelectItem));
			instance.RemoveUIEventListener(enUIEventID.Shop_CloseItemForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseItemBuyForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_BuyItem, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItem));
			instance.RemoveUIEventListener(enUIEventID.Shop_ManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ManualRefresh));
			instance.RemoveUIEventListener(enUIEventID.Shop_ConfirmManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ConfirmManualRefresh));
			instance.RemoveUIEventListener(enUIEventID.Shop_SaleTipCancel, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipCancel));
			instance.RemoveUIEventListener(enUIEventID.Shop_SaleTipConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipConfirm));
			instance.RemoveUIEventListener(enUIEventID.Shop_AutoRefreshTimerTimeUp, new CUIEventManager.OnUIEventHandler(this.OnShop_AutoRefreshTimerTimeUp));
			instance.RemoveUIEventListener(enUIEventID.Shop_ReturnToShopForm, new CUIEventManager.OnUIEventHandler(this.OnShop_ReturnToShopForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_GetBurningCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoin));
			instance.RemoveUIEventListener(enUIEventID.Shop_GetArenaCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoin));
			instance.RemoveUIEventListener(enUIEventID.Shop_Page_Up, new CUIEventManager.OnUIEventHandler(this.OnShop_PageUp));
			instance.RemoveUIEventListener(enUIEventID.Shop_Page_Down, new CUIEventManager.OnUIEventHandler(this.OnShop_PageDown));
			instance.RemoveUIEventListener(enUIEventID.Shop_OpenHonorShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenHonorShopForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_OpenArenaShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenArenaShopForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_OpenBurningShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenBurningShopForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_OpenGuildShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenGuildShopForm));
			instance.RemoveUIEventListener(enUIEventID.Shop_BuyItem_Confirm, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItemConfirm));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Shop_Auto_Refresh_Shop_Items, new Action(this.OnRequestAutoRefreshShopItems));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, new Action<RES_SHOP_TYPE>(this.OnReceiveShopItems));
			Singleton<EventRouter>.instance.RemoveEventHandler("MasterAttributesChanged", new Action(this.MasterAttrChanged));
			this.m_ShopForm = null;
		}

		public string GetCoinString(uint coinValue)
		{
			string result = coinValue.ToString();
			if (coinValue > CShopSystem.s_CoinShowMaxValue)
			{
				int num = (int)(coinValue / CShopSystem.s_CoinShowStepValue);
				result = string.Format("{0}万", num);
			}
			return result;
		}

		private bool NeedAutoRefreshShop(RES_SHOP_TYPE shopType)
		{
			ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint)((ushort)shopType));
			if (dataByKey == null)
			{
				return false;
			}
			if (this.m_Shops.ContainsKey((uint)shopType) && this.m_ShopItems.ContainsKey((uint)shopType) && this.m_ShopItems[(uint)shopType].Length == 0)
			{
				return true;
			}
			stShopInfo stShopInfo = new stShopInfo();
			bool flag = this.m_Shops.TryGetValue((uint)shopType, ref stShopInfo);
			if (stShopInfo == null)
			{
				return false;
			}
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 1);
			DateTime dateTime2 = dateTime.AddSeconds((double)CRoleInfo.GetCurrentUTCTime());
			DateTime dateTime3 = dateTime.AddSeconds((double)stShopInfo.dwAutoRefreshTime);
			DateTime dateTime4 = default(DateTime);
			int[] refreshTime = dataByKey.RefreshTime;
			for (int i = 0; i < refreshTime.Length; i++)
			{
				if (refreshTime[i] == 0)
				{
					return false;
				}
				DateTime dateTime5 = new DateTime(dateTime3.get_Year(), dateTime3.get_Month(), dateTime3.get_Day(), 0, 0, 0, 1);
				int num = refreshTime[i] / 100;
				int num2 = refreshTime[i] % 100;
				DateTime dateTime6 = dateTime5.AddSeconds((double)(num * 3600) + (double)(num2 * 60) - 28800.0);
				while (DateTime.Compare(dateTime6, dateTime2) <= 0)
				{
					if (DateTime.Compare(dateTime3, dateTime6) < 0 && DateTime.Compare(dateTime6, dateTime2) < 0)
					{
						return true;
					}
					dateTime6 = dateTime6.AddDays(1.0);
				}
			}
			return flag && stShopInfo.dwMaxRefreshTime == 0;
		}

		public bool OpenMysteryShopActiveTip()
		{
			return false;
		}

		public bool IsMysteryShopAvailable()
		{
			return false;
		}

		public void InitTab()
		{
			if (this.m_ShopForm == null || !this.m_IsShopFormOpen)
			{
				return;
			}
			CShopSystem.Tab[] array = (CShopSystem.Tab[])Enum.GetValues(typeof(CShopSystem.Tab));
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			byte b = 0;
			while ((int)b < array.Length)
			{
				switch (array[(int)b])
				{
				case CShopSystem.Tab.Fix_Shop:
					if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_FIXED))
					{
						list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed"));
						list2.Add(0);
					}
					break;
				case CShopSystem.Tab.Pvp_Symbol_Shop:
					if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL))
					{
						list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Pvp_Symbol"));
						list2.Add(1);
					}
					break;
				case CShopSystem.Tab.Burning_Exp_Shop:
					if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_BURNING))
					{
						list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Burning_Exp"));
						list2.Add(2);
					}
					break;
				case CShopSystem.Tab.Arena_Shop:
					if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_ARENA))
					{
						list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Arena"));
						list2.Add(3);
					}
					break;
				case CShopSystem.Tab.Guild_Shop:
					if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_GUILD))
					{
						list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Guild"));
						list2.Add(4);
					}
					break;
				}
				b += 1;
			}
			GameObject widget = this.m_ShopForm.GetWidget(2);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			if (component != null)
			{
				component.SetElementAmount(list.get_Count());
				int index = 0;
				for (int i = 0; i < component.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					Text component2 = elemenet.gameObject.transform.Find("txtTab").GetComponent<Text>();
					component2.set_text(list.get_Item(i));
					Text component3 = elemenet.gameObject.transform.Find("txtShopTypeData").GetComponent<Text>();
					component3.set_text(list2.get_Item(i).ToString());
					if (this.CurTab == (CShopSystem.Tab)list2.get_Item(i))
					{
						index = i;
					}
				}
				component.m_alwaysDispatchSelectedChangeEvent = true;
				component.SelectElement(index, true);
			}
		}

		public void InitSubTab()
		{
			GameObject widget = this.m_ShopForm.GetWidget(5);
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Guild_Shop_Tab_Item"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Shop_Tab_HeadFrame")
			};
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				Text component2 = component.GetElemenet(i).transform.Find("Text").GetComponent<Text>();
				component2.set_text(array[i]);
			}
		}

		public void InitShop()
		{
			this.m_CurPage = 0;
			if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
			{
				Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
				return;
			}
			switch (this.CurTab)
			{
			case CShopSystem.Tab.Fix_Shop:
			case CShopSystem.Tab.Pvp_Symbol_Shop:
			case CShopSystem.Tab.Burning_Exp_Shop:
			case CShopSystem.Tab.Arena_Shop:
			case CShopSystem.Tab.Guild_Shop:
				Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
				break;
			}
		}

		public void RefreshShop(RES_SHOP_TYPE shopType)
		{
			if (!this.m_IsShopFormOpen)
			{
				return;
			}
			switch (shopType)
			{
			case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
				if (this.CurTab != CShopSystem.Tab.Fix_Shop)
				{
					return;
				}
				break;
			case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
				if (this.CurTab != CShopSystem.Tab.Pvp_Symbol_Shop)
				{
					return;
				}
				break;
			case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
				if (this.CurTab != CShopSystem.Tab.Burning_Exp_Shop)
				{
					return;
				}
				break;
			case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
				if (this.CurTab != CShopSystem.Tab.Arena_Shop)
				{
					return;
				}
				break;
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
				if (this.CurTab != CShopSystem.Tab.Guild_Shop)
				{
					return;
				}
				break;
			}
			this.RefreshTopBar();
			this.RefreshShopItemsPanel();
			this.RefreshInfoPanel();
			this.RefreshReturnToShopEntry();
			this.RefreshBuyPnl();
		}

		public void InitTopBar()
		{
			if (this.m_ShopForm != null)
			{
				GameObject widget = this.m_ShopForm.GetWidget(3);
				if (widget != null)
				{
					widget.CustomSetActive(false);
				}
			}
		}

		public void RefreshTopBar()
		{
			if (this.m_ShopForm == null)
			{
				return;
			}
			GameObject widget = this.m_ShopForm.GetWidget(3);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform;
			GameObject gameObject = transform.FindChild("Coin").gameObject;
			GameObject gameObject2 = transform.FindChild("Gold").gameObject;
			GameObject gameObject3 = transform.FindChild("PvpCoin").gameObject;
			GameObject gameObject4 = transform.FindChild("BurningCoin").gameObject;
			GameObject gameObject5 = transform.FindChild("ArenaCoin").gameObject;
			GameObject gameObject6 = transform.FindChild("GuildConstruct").gameObject;
			switch (this.m_CurShopType)
			{
			case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
			case RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY:
			{
				widget.CustomSetActive(true);
				gameObject.CustomSetActive(true);
				Text component = gameObject.transform.FindChild("Text").GetComponent<Text>();
				if (component != null)
				{
					component.set_text(this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin));
				}
				gameObject2.CustomSetActive(true);
				component = gameObject2.transform.FindChild("Text").GetComponent<Text>();
				if (component != null)
				{
					component.set_text(this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin));
				}
				gameObject3.CustomSetActive(true);
				component = gameObject3.transform.FindChild("Text").GetComponent<Text>();
				if (component != null)
				{
					component.set_text(this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin));
				}
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				break;
			}
			case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
				widget.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				break;
			case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
			{
				widget.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(true);
				Text component2 = gameObject4.transform.FindChild("Text").GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text(this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().BurningCoin));
				}
				gameObject5.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				break;
			}
			case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
			{
				widget.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(true);
				Text component3 = gameObject5.transform.FindChild("Text").GetComponent<Text>();
				if (component3 != null)
				{
					component3.set_text(this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ArenaCoin));
				}
				gameObject6.CustomSetActive(false);
				break;
			}
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
			{
				widget.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject6.CustomSetActive(true);
				Text component4 = gameObject6.transform.FindChild("Text").GetComponent<Text>();
				if (component4 != null)
				{
					component4.set_text(this.GetCoinString(CGuildHelper.GetPlayerGuildConstruct()));
				}
				break;
			}
			}
		}

		public void RefreshShopItemsPanel()
		{
			if (this.m_ShopForm == null || !this.m_IsShopFormOpen)
			{
				return;
			}
			GameObject widget = this.m_ShopForm.GetWidget(1);
			if (widget != null)
			{
				CUIListScript component = widget.transform.FindChild("List").GetComponent<CUIListScript>();
				if (component != null)
				{
					byte b = 0;
					if (this.m_ShopItems.ContainsKey((uint)this.m_CurShopType))
					{
						b = Convert.ToByte(Math.Ceiling((double)this.m_ShopItems[(uint)this.m_CurShopType].Length / 6.0));
					}
					if (b <= 1)
					{
						GameObject gameObject = component.transform.FindChild("Scrollbar").gameObject;
						if (gameObject != null)
						{
							gameObject.CustomSetActive(false);
						}
					}
					else
					{
						GameObject gameObject2 = component.transform.FindChild("Scrollbar").gameObject;
						if (gameObject2 != null)
						{
							gameObject2.CustomSetActive(true);
						}
					}
					component.SetElementAmount((int)b);
					for (byte b2 = 0; b2 < b; b2 += 1)
					{
						CUIListElementScript elemenet = component.GetElemenet((int)b2);
						if (this.m_CurPage == b2)
						{
							component.MoveElementInScrollArea((int)this.m_CurPage, true);
						}
						this.SetPageItems(elemenet, b2);
					}
					component.m_alwaysDispatchSelectedChangeEvent = false;
				}
			}
		}

		private void SetPageItems(CUIListElementScript listElementScript, byte page)
		{
			int i = (int)(6 * page);
			int num = i + 6;
			GameObject gameObject = listElementScript.gameObject;
			byte b = 0;
			while (i < num)
			{
				Transform transform = gameObject.transform.FindChild("pnlItem" + b);
				if (i >= this.m_ShopItems[(uint)this.m_CurShopType].Length)
				{
					if (transform != null)
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
				else if (transform != null)
				{
					GameObject gameObject2 = transform.gameObject;
					gameObject2.CustomSetActive(true);
					CUIEventScript cUIEventScript = gameObject2.GetComponent<CUIEventScript>();
					if (cUIEventScript == null)
					{
						cUIEventScript = gameObject2.AddComponent<CUIEventScript>();
						cUIEventScript.Initialize(listElementScript.m_belongedFormScript);
					}
					cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Shop_SelectItem, new stUIEventParams
					{
						tag = i
					});
					this.SetItemInfo(gameObject2, ref this.m_ShopItems[(uint)this.m_CurShopType][i], i);
				}
				b += 1;
				i++;
			}
		}

		private void SetItemInfo(GameObject item, ref stShopItemInfo info, int slotOffset)
		{
			Singleton<CShopSystem>.GetInstance().GetShopItemPrice(ref info);
			Transform transform = item.transform;
			GameObject gameObject = transform.Find("itemCell").gameObject;
			GameObject gameObject2 = transform.Find("txtName").gameObject;
			GameObject gameObject3 = transform.Find("pnlPrice").gameObject;
			Transform transform2 = gameObject3.transform;
			GameObject gameObject4 = transform.Find("pnlLock").gameObject;
			GameObject gameObject5 = transform2.Find("imgCoin").gameObject;
			GameObject gameObject6 = transform2.Find("imgGold").gameObject;
			GameObject gameObject7 = transform2.Find("imgPvp").gameObject;
			GameObject gameObject8 = transform2.Find("imgBurning").gameObject;
			GameObject gameObject9 = transform2.Find("imgArena").gameObject;
			GameObject gameObject10 = transform2.Find("imgGuild").gameObject;
			GameObject gameObject11 = transform.Find("pnlDiscount").gameObject;
			GameObject gameObject12 = transform.Find("imgNormal").gameObject;
			GameObject gameObject13 = transform.Find("imgGray").gameObject;
			GameObject gameObject14 = transform.Find("imgSoldOut").gameObject;
			GameObject gameObject15 = transform.Find("pnlStatus").gameObject;
			Transform transform3 = gameObject11.transform;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			Text component = transform.Find("txtName").GetComponent<Text>();
			Text component2 = transform3.Find("txtDiscount").GetComponent<Text>();
			Text component3 = transform2.Find("txtPrice").GetComponent<Text>();
			switch (info.enCostType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				gameObject6.CustomSetActive(true);
				gameObject5.CustomSetActive(false);
				gameObject7.CustomSetActive(false);
				gameObject8.CustomSetActive(false);
				gameObject9.CustomSetActive(false);
				gameObject10.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				gameObject7.CustomSetActive(true);
				gameObject6.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject8.CustomSetActive(false);
				gameObject9.CustomSetActive(false);
				gameObject10.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				gameObject8.CustomSetActive(true);
				gameObject7.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject9.CustomSetActive(false);
				gameObject10.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				gameObject9.CustomSetActive(true);
				gameObject7.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject8.CustomSetActive(false);
				gameObject10.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				gameObject10.CustomSetActive(true);
				gameObject7.CustomSetActive(false);
				gameObject6.CustomSetActive(false);
				gameObject5.CustomSetActive(false);
				gameObject8.CustomSetActive(false);
				gameObject9.CustomSetActive(false);
				break;
			}
			CUseable cUseable = CUseableManager.CreateUseable(info.enItemType, info.dwItemId, (int)info.wItemCnt);
			if (cUseable == null)
			{
				return;
			}
			bool flag = this.IsOnlyAndOwned(ref info);
			string name = cUseable.m_name;
			component.set_text(name);
			info.sName = name;
			component3.set_text(info.fPrice.ToString("N0"));
			switch (info.enCostType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				if (masterRoleInfo.DianQuan < info.fPrice)
				{
					component3.set_color(new Color(1f, 0.4f, 0.4f, 1f));
				}
				else
				{
					component3.set_color(new Color(1f, 0.91f, 0.45f, 1f));
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				if (masterRoleInfo.GoldCoin < info.fPrice)
				{
					component3.set_color(new Color(1f, 0.4f, 0.4f, 1f));
				}
				else
				{
					component3.set_color(new Color(1f, 0.91f, 0.45f, 1f));
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				if (masterRoleInfo.BurningCoin < info.fPrice)
				{
					component3.set_color(new Color(1f, 0.4f, 0.4f, 1f));
				}
				else
				{
					component3.set_color(new Color(1f, 0.91f, 0.45f, 1f));
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				if (masterRoleInfo.ArenaCoin < info.fPrice)
				{
					component3.set_color(new Color(1f, 0.4f, 0.4f, 1f));
				}
				else
				{
					component3.set_color(new Color(1f, 0.91f, 0.45f, 1f));
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				if (CGuildHelper.GetPlayerGuildConstruct() < info.fPrice)
				{
					component3.set_color(new Color(1f, 0.4f, 0.4f, 1f));
				}
				else
				{
					component3.set_color(new Color(1f, 0.91f, 0.45f, 1f));
				}
				break;
			}
			if (info.isSoldOut)
			{
				component3.set_color(new Color(0.7176f, 0.7176f, 0.7176f, 1f));
				gameObject12.CustomSetActive(false);
				gameObject13.CustomSetActive(true);
				gameObject14.CustomSetActive(true);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
			}
			else
			{
				gameObject12.CustomSetActive(true);
				gameObject13.CustomSetActive(false);
				gameObject14.CustomSetActive(false);
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(true);
				CUIFormScript belongedFormScript = item.GetComponent<CUIEventScript>().m_belongedFormScript;
				CUICommonSystem.SetItemCell(belongedFormScript, gameObject, cUseable, false, false, false, false);
			}
			if (info.wSaleDiscount != 100)
			{
				gameObject11.CustomSetActive(true);
				component2.set_text(((float)info.wSaleDiscount / 10f).ToString("F1") + Singleton<CTextManager>.GetInstance().GetText("Shop_Discount"));
			}
			else
			{
				gameObject11.CustomSetActive(false);
			}
			if (this.IsSlotLocked(slotOffset))
			{
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(true);
				this.SetItemLockPanel(gameObject4, slotOffset);
			}
			else
			{
				gameObject2.CustomSetActive(true);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(false);
			}
			if (flag)
			{
				gameObject15.CustomSetActive(true);
				gameObject2.CustomSetActive(true);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
			}
			else
			{
				gameObject15.CustomSetActive(false);
			}
		}

		private void SetItemLockPanel(GameObject lockPanel, int slotOffset)
		{
			Text component = lockPanel.transform.Find("txtUnlockTip").GetComponent<Text>();
			if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD)
			{
				uint starLevelForOpenGuildItemShopSlot = CGuildHelper.GetStarLevelForOpenGuildItemShopSlot(slotOffset);
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Shop_Guild_Item_Shop_Unlock_Tip", new string[]
				{
					starLevelForOpenGuildItemShopSlot.ToString()
				}));
			}
			else if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE)
			{
				string gradeNameForOpenGuildHeadImageShopSlot = CGuildHelper.GetGradeNameForOpenGuildHeadImageShopSlot(slotOffset);
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Shop_Guild_Head_Image_Shop_Unlock_Tip", new string[]
				{
					gradeNameForOpenGuildHeadImageShopSlot
				}));
			}
		}

		private bool IsSlotLocked(int slotOffset)
		{
			return (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD && (long)slotOffset >= (long)((ulong)CGuildHelper.GetGuildItemShopOpenSlotCount())) || (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE && (long)slotOffset >= (long)((ulong)CGuildHelper.GetGuildHeadImageShopOpenSlotCount()));
		}

		private bool IsOnlyAndOwned(ref stShopItemInfo info)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			bool result = false;
			switch (info.enItemType)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
				result = masterRoleInfo.IsHaveHero(info.dwItemId, false);
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
				result = masterRoleInfo.IsHaveHeroSkin(info.dwItemId, false);
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG:
				result = (Singleton<HeadIconSys>.GetInstance().GetInfo(info.dwItemId) != null);
				break;
			}
			return result;
		}

		public void RefreshInfoPanel()
		{
			switch (this.m_CurShopType)
			{
			case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
			case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
			case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
			case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
			case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
			{
				GameObject widget = this.m_ShopForm.GetWidget(0);
				GameObject gameObject = widget.transform.Find("pnlAutoRefresh/pnlRefreshTime/Text").gameObject;
				GameObject gameObject2 = widget.transform.Find("pnlManualRefresh/btnRefresh").gameObject;
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 1);
				DateTime dateTime2 = dateTime.AddSeconds((double)CRoleInfo.GetCurrentUTCTime());
				Text component = gameObject.GetComponent<Text>();
				CUITimerScript component2 = widget.transform.Find("pnlAutoRefresh/timerAutoRefresh/timer").GetComponent<CUITimerScript>();
				DateTime nextAutoRefreshTime = this.GetNextAutoRefreshTime(this.m_CurShopType);
				gameObject2.CustomSetActive(true);
				if (nextAutoRefreshTime == DateTime.MinValue)
				{
					gameObject.CustomSetActive(false);
				}
				else
				{
					float totalTime = (float)((nextAutoRefreshTime - dateTime2).get_TotalSeconds() - 28800.0);
					component2.SetTotalTime(totalTime);
					component2.StartTimer();
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("Shop_Next_Refresh_Time_Prefix") + " <color=#f5be17>" + nextAutoRefreshTime.ToString("HH:mm") + "</color>");
					gameObject.CustomSetActive(true);
				}
				Text component3 = this.m_ShopForm.GetWidget(4).GetComponent<Text>();
				int manualRefreshMaxCnt = CShopSystem.GetManualRefreshMaxCnt(this.m_CurShopType);
				this.ResetManualRefreshCountIfNeed();
				int manualRefreshedCnt = this.GetManualRefreshedCnt();
				component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Shop_Today_Left_Cnt", new string[]
				{
					(manualRefreshMaxCnt - manualRefreshedCnt).ToString()
				}));
				break;
			}
			case RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY:
			{
				GameObject gameObject3 = this.m_ShopForm.transform.Find("pnlShop/pnlInfo/timerMisteryAvailable").gameObject;
				GameObject gameObject4 = gameObject3.transform.Find("timer").gameObject;
				CUITimerScript component4 = gameObject4.GetComponent<CUITimerScript>();
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				stShopInfo stShopInfo = new stShopInfo();
				if (this.m_Shops.TryGetValue(2u, ref stShopInfo))
				{
					int dwItemValidTime = stShopInfo.dwItemValidTime;
					int num = dwItemValidTime - currentUTCTime;
					if (num < 0)
					{
						num = 0;
					}
					component4.SetTotalTime((float)num);
					component4.StartTimer();
					gameObject3.CustomSetActive(true);
				}
				else
				{
					gameObject3.CustomSetActive(false);
				}
				break;
			}
			}
		}

		private void ResetManualRefreshCountIfNeed()
		{
			stShopInfo stShopInfo;
			if (this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
			{
				uint globalRefreshTimeSeconds = Utility.GetGlobalRefreshTimeSeconds();
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				if ((long)stShopInfo.dwManualRefreshTime < (long)((ulong)globalRefreshTimeSeconds) && (ulong)globalRefreshTimeSeconds <= (ulong)((long)currentUTCTime))
				{
					stShopInfo.dwManualRefreshCnt = 1;
				}
			}
		}

		private void RefreshBuyPnl()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(this.sShopBuyFormPath);
			if (form == null || this.m_currentSelectItemIdx == -1)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			stShopItemInfo stShopItemInfo = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint)this.m_CurShopType][this.m_currentSelectItemIdx];
			if (stShopItemInfo.isSoldOut)
			{
				return;
			}
			Singleton<CShopSystem>.GetInstance().GetShopItemPrice(ref stShopItemInfo);
			GameObject gameObject = Utility.FindChild(form.gameObject, "Panel_Left/pnlItem/itemCell");
			GameObject obj = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgCoin");
			GameObject obj2 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgGold");
			GameObject obj3 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgPvp");
			GameObject obj4 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgBurning");
			GameObject obj5 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgArena");
			GameObject obj6 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgGuild");
			Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlTitle/lblName");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlStatus/lblCount");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlStatus/lblCountTitleSuffix");
			Text componetInChild4 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/txtPrice");
			Text componetInChild5 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlMemo/pnlDes/Text");
			Text componetInChild6 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/txtCnt");
			CUseable cUseable = CUseableManager.CreateUseable(stShopItemInfo.enItemType, stShopItemInfo.dwItemId, (int)stShopItemInfo.wItemCnt);
			if (cUseable == null)
			{
				return;
			}
			switch (stShopItemInfo.enCostType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				if (componetInChild4 != null)
				{
					if (masterRoleInfo.DianQuan < stShopItemInfo.fPrice)
					{
						componetInChild4.set_color(new Color(1f, 0.4f, 0.4f, 1f));
					}
					else
					{
						componetInChild4.set_color(new Color(1f, 0.91f, 0.45f, 1f));
					}
				}
				obj2.CustomSetActive(true);
				obj.CustomSetActive(false);
				obj3.CustomSetActive(false);
				obj4.CustomSetActive(false);
				obj5.CustomSetActive(false);
				obj6.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				if (componetInChild4 != null)
				{
					if (masterRoleInfo.GoldCoin < stShopItemInfo.fPrice)
					{
						componetInChild4.set_color(new Color(1f, 0.4f, 0.4f, 1f));
					}
					else
					{
						componetInChild4.set_color(new Color(1f, 0.91f, 0.45f, 1f));
					}
				}
				obj3.CustomSetActive(true);
				obj2.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj4.CustomSetActive(false);
				obj5.CustomSetActive(false);
				obj6.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				if (componetInChild4 != null)
				{
					if (masterRoleInfo.BurningCoin < stShopItemInfo.fPrice)
					{
						componetInChild4.set_color(new Color(1f, 0.4f, 0.4f, 1f));
					}
					else
					{
						componetInChild4.set_color(new Color(1f, 0.91f, 0.45f, 1f));
					}
				}
				obj4.CustomSetActive(true);
				obj3.CustomSetActive(false);
				obj2.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj5.CustomSetActive(false);
				obj6.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				if (componetInChild4 != null)
				{
					if (masterRoleInfo.ArenaCoin < stShopItemInfo.fPrice)
					{
						componetInChild4.set_color(new Color(1f, 0.4f, 0.4f, 1f));
					}
					else
					{
						componetInChild4.set_color(new Color(1f, 0.91f, 0.45f, 1f));
					}
				}
				obj5.CustomSetActive(true);
				obj3.CustomSetActive(false);
				obj2.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj4.CustomSetActive(false);
				obj6.CustomSetActive(false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				if (componetInChild4 != null)
				{
					if (CGuildHelper.GetPlayerGuildConstruct() < stShopItemInfo.fPrice)
					{
						componetInChild4.set_color(new Color(1f, 0.4f, 0.4f, 1f));
					}
					else
					{
						componetInChild4.set_color(new Color(1f, 0.91f, 0.45f, 1f));
					}
				}
				obj6.CustomSetActive(true);
				obj3.CustomSetActive(false);
				obj2.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj4.CustomSetActive(false);
				obj5.CustomSetActive(false);
				break;
			}
			int num = 0;
			if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
			{
				if (masterRoleInfo.IsHaveHero(stShopItemInfo.dwItemId, false))
				{
					num = 1;
				}
			}
			else if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				uint heroId = 0u;
				uint skinId = 0u;
				CSkinInfo.ResolveHeroSkin(stShopItemInfo.dwItemId, out heroId, out skinId);
				if (masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
				{
					num = 1;
				}
			}
			else
			{
				CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
				num = useableContainer.GetUseableStackCount(stShopItemInfo.enItemType, stShopItemInfo.dwItemId);
			}
			if (componetInChild2 != null)
			{
				componetInChild2.set_text(num.ToString());
			}
			if (componetInChild3 != null)
			{
				componetInChild3.set_text(Singleton<CTextManager>.GetInstance().GetText("Shop_Item_Buy_Status_Unit"));
			}
			if (componetInChild != null)
			{
				componetInChild.set_text(cUseable.m_name);
			}
			if (componetInChild4 != null)
			{
				componetInChild4.set_text(stShopItemInfo.fPrice.ToString());
			}
			if (componetInChild5 != null)
			{
				componetInChild5.set_text(cUseable.m_description);
			}
			if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = (CItem)cUseable;
				if ((cItem.m_itemData.bClass == 2 || cItem.m_itemData.bClass == 3) && componetInChild5 != null)
				{
					componetInChild5.set_text(CUIUtility.StringReplace(componetInChild5.get_text(), new string[]
					{
						num.ToString()
					}));
				}
			}
			else if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				CEquip cEquip = (CEquip)cUseable;
				if (cEquip != null && componetInChild5 != null)
				{
					componetInChild5.set_text(cEquip.m_description + "\n<color=#a52a2aff>" + CUICommonSystem.GetFuncEftDesc(ref cEquip.m_equipData.astFuncEftList, false) + "</color>");
				}
			}
			else if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
			}
			if (componetInChild6 != null)
			{
				componetInChild6.set_text(stShopItemInfo.wItemCnt + Singleton<CTextManager>.GetInstance().GetText("Shop_Item_Buy_Status_Unit"));
			}
			if (gameObject != null)
			{
				CUICommonSystem.SetItemCell(form, gameObject, cUseable, false, false, false, false);
			}
		}

		private void RefreshReturnToShopEntry()
		{
			if (this.m_CurShopType != RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
			{
				return;
			}
			if (this.IsMysteryShopAvailable())
			{
				this.m_ShopForm.transform.Find("pnlShop/imgReturnToShop").gameObject.CustomSetActive(true);
			}
			else
			{
				this.m_ShopForm.transform.Find("pnlShop/imgReturnToShop").gameObject.CustomSetActive(false);
			}
		}

		private DateTime GetNextAutoRefreshTime(RES_SHOP_TYPE shopType)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 1);
			DateTime dateTime2 = dateTime.AddSeconds((double)CRoleInfo.GetCurrentUTCTime());
			DateTime dateTime3 = new DateTime(dateTime2.get_Year(), dateTime2.get_Month(), dateTime2.get_Day(), 0, 0, 0, 1);
			DateTime dateTime4 = default(DateTime);
			DateTime dateTime5 = default(DateTime);
			int[] refreshTime = GameDataMgr.shopTypeDatabin.GetDataByKey((uint)((ushort)shopType)).RefreshTime;
			for (int i = 0; i < refreshTime.Length; i++)
			{
				if (refreshTime[i] == 0 && i == 0)
				{
					return dateTime4;
				}
				if (refreshTime[i] == 0)
				{
					return dateTime5.AddSeconds(115200.0);
				}
				int num = refreshTime[i] / 100;
				int num2 = refreshTime[i] % 100;
				dateTime4 = dateTime3.AddSeconds((double)(num * 3600) + (double)(num2 * 60) - 28800.0);
				if (dateTime5 == DateTime.MinValue)
				{
					dateTime5 = dateTime4;
				}
				if (DateTime.Compare(dateTime2, dateTime4) < 0)
				{
					return dateTime4.AddSeconds(28800.0);
				}
				if (i == refreshTime.Length - 1)
				{
					return dateTime5.AddSeconds(115200.0);
				}
			}
			return dateTime4;
		}

		private void GetShopItemPrice(ref stShopItemInfo info)
		{
			uint num = 0u;
			CUseable cUseable = CUseableManager.CreateUseable(info.enItemType, info.dwItemId, (int)info.wItemCnt);
			if (cUseable != null)
			{
				num = cUseable.GetBuyPrice(info.enCostType);
			}
			float fPrice;
			if (info.wSaleDiscount != 100)
			{
				fPrice = (uint)info.wItemCnt * num * (uint)info.wSaleDiscount / 100u;
			}
			else
			{
				fPrice = (uint)info.wItemCnt * num;
			}
			info.fPrice = fPrice;
		}

		private ResShopRefreshCost GetCost(RES_SHOP_TYPE shopType, int refreshCnt)
		{
			return GameDataMgr.shopRefreshCostDatabin.FindIf((ResShopRefreshCost x) => (RES_SHOP_TYPE)x.wShopType == shopType && refreshCnt == (int)x.wRefreshFreq);
		}

		private int EnoughMoneyToRefresh(ref string msg)
		{
			stShopInfo stShopInfo = new stShopInfo();
			if (!this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
			{
				msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
				return -7;
			}
			ResShopRefreshCost cost = this.GetCost(stShopInfo.enShopType, stShopInfo.dwManualRefreshCnt);
			if (cost == null)
			{
				return 0;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			switch (cost.bCostType)
			{
			case 2:
				if (masterRoleInfo.DianQuan < (ulong)cost.dwCostPrice)
				{
					msg = Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Not_Enough");
					return -1;
				}
				break;
			case 4:
				if (masterRoleInfo.GoldCoin < cost.dwCostPrice)
				{
					msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_GoldCoin_Not_Enough2");
					return -3;
				}
				break;
			case 5:
				if (masterRoleInfo.BurningCoin < cost.dwCostPrice)
				{
					msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Burning_Not_Enough2");
					return -4;
				}
				break;
			case 6:
				if (masterRoleInfo.ArenaCoin < cost.dwCostPrice)
				{
					msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Arena_Not_Enough2");
					return -5;
				}
				break;
			case 8:
				if (CGuildHelper.GetPlayerGuildConstruct() < cost.dwCostPrice)
				{
					msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Guild_Not_Enough");
					return -6;
				}
				break;
			}
			return 0;
		}

		private bool ManualRefreshLimit(ref string msg)
		{
			stShopInfo stShopInfo = new stShopInfo();
			if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
			{
				msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
				return true;
			}
			if (!this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
			{
				msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
				return true;
			}
			if (stShopInfo.dwManualRefreshLimit < stShopInfo.dwManualRefreshCnt)
			{
				msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Limit");
				return true;
			}
			if (stShopInfo.bManualRefreshSent)
			{
				msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Req_Sent");
				return true;
			}
			return false;
		}

		private void InitSaleTip()
		{
			GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			if (curUseableCount <= 0)
			{
				gameObject.CustomSetActive(false);
				return;
			}
			this.aSaleItems = new CUseable[this.MAX_SALE_ITEM_CNT];
			this.m_saleItemsCnt = 0;
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if ((int)this.m_saleItemsCnt == this.MAX_SALE_ITEM_CNT)
				{
					break;
				}
				switch (useableByIndex.m_type)
				{
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
				{
					CItem cItem = (CItem)useableByIndex;
					if (cItem.m_itemData.bIsAutoSale == 1)
					{
						this.aSaleItems[(int)this.m_saleItemsCnt] = useableByIndex;
						this.m_saleItemsCnt += 1;
					}
					break;
				}
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
				{
					CEquip cEquip = (CEquip)useableByIndex;
					if (cEquip.m_equipData.bIsAutoSale == 1)
					{
						this.aSaleItems[(int)this.m_saleItemsCnt] = useableByIndex;
						this.m_saleItemsCnt += 1;
					}
					break;
				}
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
				{
					CSymbolItem cSymbolItem = (CSymbolItem)useableByIndex;
					if (cSymbolItem.m_SymbolData.bIsAutoSale == 1)
					{
						this.aSaleItems[(int)this.m_saleItemsCnt] = useableByIndex;
						this.m_saleItemsCnt += 1;
					}
					break;
				}
				}
			}
			if (this.m_saleItemsCnt > 0)
			{
				GameObject gameObject2 = gameObject.transform.Find("pnlTip/pnlItems/List").gameObject;
				Text component = gameObject.transform.Find("pnlTip/pnlContainer/pnlPrice/txtCnt").GetComponent<Text>();
				CUIListScript component2 = gameObject2.GetComponent<CUIListScript>();
				component2.SetElementAmount((int)this.m_saleItemsCnt);
				uint num = 0u;
				byte b = 0;
				while ((ushort)b < this.m_saleItemsCnt)
				{
					stShopItemInfo stShopItemInfo = default(stShopItemInfo);
					CUseable cUseable = this.aSaleItems[(int)b];
					switch (cUseable.m_type)
					{
					case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
					{
						CItem cItem2 = (CItem)cUseable;
						stShopItemInfo.sName = cItem2.m_name;
						stShopItemInfo.dwItemId = cItem2.m_baseID;
						stShopItemInfo.wItemCnt = (ushort)cItem2.m_stackCount;
						stShopItemInfo.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP;
						stShopItemInfo.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
						stShopItemInfo.wSaleDiscount = 100;
						stShopItemInfo.isSoldOut = false;
						break;
					}
					case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
					{
						CEquip cEquip2 = (CEquip)cUseable;
						stShopItemInfo.sName = cEquip2.m_name;
						stShopItemInfo.dwItemId = cEquip2.m_baseID;
						stShopItemInfo.wItemCnt = (ushort)cEquip2.m_stackCount;
						stShopItemInfo.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP;
						stShopItemInfo.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
						stShopItemInfo.wSaleDiscount = 100;
						stShopItemInfo.isSoldOut = false;
						break;
					}
					case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
					{
						CSymbolItem cSymbolItem2 = (CSymbolItem)cUseable;
						stShopItemInfo.sName = cSymbolItem2.m_name;
						stShopItemInfo.dwItemId = cSymbolItem2.m_baseID;
						stShopItemInfo.wItemCnt = (ushort)cSymbolItem2.m_stackCount;
						stShopItemInfo.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL;
						stShopItemInfo.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
						stShopItemInfo.wSaleDiscount = 100;
						stShopItemInfo.isSoldOut = false;
						break;
					}
					}
					CUIListElementScript elemenet = component2.GetElemenet((int)b);
					this.SetSaleItemInfo(elemenet, ref stShopItemInfo);
					num += (uint)stShopItemInfo.fPrice;
					b += 1;
				}
				component.set_text(num.ToString());
				gameObject.CustomSetActive(true);
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		private void SetSaleItemInfo(CUIListElementScript listElementScript, ref stShopItemInfo info)
		{
			GameObject gameObject = listElementScript.gameObject;
			Transform transform = gameObject.transform;
			GameObject gameObject2 = transform.Find("itemCell").gameObject;
			Text component = transform.Find("txtName").GetComponent<Text>();
			Text component2 = transform.Find("txtCnt").GetComponent<Text>();
			CUseable cUseable = null;
			uint num = 0u;
			switch (info.enItemType)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
				cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, info.dwItemId, (int)info.wItemCnt);
				if (cUseable != null)
				{
					num = GameDataMgr.itemDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
				}
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
				cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, info.dwItemId, (int)info.wItemCnt);
				if (cUseable != null)
				{
					num = GameDataMgr.equipInfoDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
				}
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
				cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, info.dwItemId, (int)info.wItemCnt);
				if (cUseable != null)
				{
					num = GameDataMgr.itemDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
				}
				break;
			}
			if (cUseable == null)
			{
				return;
			}
			component.set_text(info.sName);
			component2.set_text("×" + info.wItemCnt);
			uint num2 = (uint)info.wItemCnt * num;
			info.fPrice = num2;
			CUICommonSystem.SetItemCell(listElementScript.m_belongedFormScript, gameObject2, cUseable, false, false, false, false);
		}

		private string GetShopCharacterTip(string tipKeyPrefix)
		{
			string[] array = new string[10];
			byte b = 1;
			while ((int)b <= array.Length)
			{
				array[(int)(b - 1)] = Singleton<CTextManager>.GetInstance().GetText(tipKeyPrefix + b);
				b += 1;
			}
			int num = new Random().Next(0, 10);
			return array[num];
		}

		private void UseOldShopItems(int seq)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, this.m_CurShopType);
		}

		private bool IsShopOpen(RES_SHOP_TYPE shopType)
		{
			ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint)((ushort)shopType));
			if (dataByKey == null)
			{
				DebugHelper.Assert(false, "CShopSystem.IsShopOpen(): resShopType is null, shopType={0}", new object[]
				{
					shopType
				});
				return false;
			}
			return dataByKey.bIsOpen == 1;
		}

		private bool IsShopAvailable(RES_SHOP_TYPE shopType)
		{
			bool flag = this.IsShopOpen(shopType);
			if (shopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD || shopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE)
			{
				bool flag2 = Singleton<CGuildSystem>.GetInstance().IsInNormalGuild();
				return flag && flag2;
			}
			return flag;
		}

		private int GetManualRefreshedCnt()
		{
			stShopInfo shopInfo = new stShopInfo();
			if (this.m_Shops != null && this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref shopInfo))
			{
				return CShopSystem.GetManualRefreshedCnt(shopInfo);
			}
			return 0;
		}

		private static int GetManualRefreshedCnt(stShopInfo shopInfo)
		{
			return shopInfo.dwManualRefreshCnt - 1;
		}

		private static int GetManualRefreshMaxCnt(RES_SHOP_TYPE shopType)
		{
			int num = 0;
			for (int i = 0; i < GameDataMgr.shopRefreshCostDatabin.count; i++)
			{
				if (GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wShopType == (ushort)shopType && (int)GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wRefreshFreq > num)
				{
					num = (int)GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wRefreshFreq;
				}
			}
			return num;
		}

		private void OnShop_OpenForm(CUIEvent uiEvent)
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP))
			{
				if (this.m_IsShopFormOpen)
				{
					return;
				}
				this.m_ShopForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sShopFormPath, false, true);
				this.m_IsShopFormOpen = true;
				Singleton<CSoundManager>.GetInstance().LoadBank("Store_VO", CSoundManager.BankType.Lobby);
				this.InitTopBar();
				this.InitTab();
				this.InitSubTab();
			}
			else
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(12u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			}
		}

		private void OnShop_OpenHonorShopForm(CUIEvent uiEvent)
		{
		}

		private void OnShop_OpenBurningShopForm(CUIEvent uiEvent)
		{
			this.CurTab = CShopSystem.Tab.Burning_Exp_Shop;
			uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
		}

		private void OnShop_OpenArenaShopForm(CUIEvent uiEvent)
		{
			this.CurTab = CShopSystem.Tab.Arena_Shop;
			uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
		}

		private void OnShop_OpenGuildShopForm(CUIEvent uiEvent)
		{
			this.CurTab = CShopSystem.Tab.Guild_Shop;
			uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
		}

		private void OnShop_CloseForm(CUIEvent uiEvent)
		{
			if (!this.m_IsShopFormOpen)
			{
				return;
			}
			this.m_IsShopFormOpen = false;
			Singleton<CSoundManager>.GetInstance().UnLoadBank("Store_VO", CSoundManager.BankType.Lobby);
			Singleton<CUIManager>.GetInstance().CloseForm(this.sShopFormPath);
			this.m_ShopForm = null;
		}

		private void OnShop_ReturnToShopForm(CUIEvent uiEvent)
		{
			this.m_IsShopFormOpen = false;
			this.m_CurShopType = this.m_LastNormalShop;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_OpenForm);
		}

		private void OnShop_Tab_Change(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				DebugHelper.Assert(false, "CShopSystem.OnShop_Tab_Change(): lst is null!!!");
				return;
			}
			CUIListElementScript selectedElement = component.GetSelectedElement();
			Text component2 = selectedElement.transform.Find("txtShopTypeData").GetComponent<Text>();
			int curTab;
			if (!int.TryParse(component2.get_text(), ref curTab))
			{
				DebugHelper.Assert(false, "CShopSystem.OnShop_Tab_Change(): txtShopTypeData.text={0}", new object[]
				{
					component2.get_text()
				});
				return;
			}
			bool flag = true;
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(6);
			widget.CustomSetActive(true);
			GameObject widget2 = uiEvent.m_srcFormScript.GetWidget(5);
			widget2.CustomSetActive(false);
			switch (curTab)
			{
			case 0:
				if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP))
				{
					ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(12u);
					Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
					flag = false;
				}
				break;
			case 1:
				if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOLSHOP))
				{
					ResSpecialFucUnlock dataByKey2 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(14u);
					Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey2.szLockedTip), false, 1.5f, null, new object[0]);
					flag = false;
				}
				break;
			case 2:
				if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG))
				{
					ResSpecialFucUnlock dataByKey3 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(4u);
					Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey3.szLockedTip), false, 1.5f, null, new object[0]);
					flag = false;
				}
				break;
			case 3:
				if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
				{
					ResSpecialFucUnlock dataByKey4 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(9u);
					Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey4.szLockedTip), false, 1.5f, null, new object[0]);
					flag = false;
				}
				break;
			case 4:
				if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_GUILDSHOP))
				{
					ResSpecialFucUnlock dataByKey5 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(17u);
					Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey5.szLockedTip), false, 1.5f, null, new object[0]);
					flag = false;
				}
				break;
			}
			if (!flag)
			{
				int lastSelectedIndex = component.GetLastSelectedIndex();
				if (lastSelectedIndex != -1)
				{
					component.m_alwaysDispatchSelectedChangeEvent = true;
					component.SelectElement(lastSelectedIndex, true);
				}
			}
			else
			{
				this.CurTab = (CShopSystem.Tab)curTab;
				if (this.CurTab == CShopSystem.Tab.Guild_Shop)
				{
					widget2.CustomSetActive(true);
					CUIListScript component3 = widget2.GetComponent<CUIListScript>();
					component3.SelectElement(0, true);
				}
				else
				{
					this.InitShop();
				}
			}
		}

		private void OnShop_SubTab_Change(CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(6);
			int subTabSelectedIndex = this.GetSubTabSelectedIndex();
			CShopSystem.SubTab subTab = (CShopSystem.SubTab)subTabSelectedIndex;
			if (subTab != CShopSystem.SubTab.Guild_Item_Shop)
			{
				if (subTab == CShopSystem.SubTab.Guild_HeadImage_Shop)
				{
					this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE;
					widget.CustomSetActive(false);
				}
			}
			else
			{
				this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_GUILD;
				widget.CustomSetActive(true);
			}
			this.CurTab = CShopSystem.Tab.Guild_Shop;
			this.InitShop();
		}

		private int GetSubTabSelectedIndex()
		{
			if (this.m_ShopForm != null)
			{
				CUIListScript component = this.m_ShopForm.GetWidget(5).GetComponent<CUIListScript>();
				return component.GetSelectedIndex();
			}
			return 0;
		}

		private void OnShop_SelectItem(CUIEvent uiEvent)
		{
			this.m_currentSelectItemIdx = uiEvent.m_eventParams.tag;
			if (this.m_currentSelectItemIdx < 0 || this.m_currentSelectItemIdx >= Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint)this.m_CurShopType].Length)
			{
				return;
			}
			stShopItemInfo stShopItemInfo = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint)this.m_CurShopType][this.m_currentSelectItemIdx];
			if (stShopItemInfo.isSoldOut || this.IsSlotLocked(this.m_currentSelectItemIdx) || this.IsOnlyAndOwned(ref stShopItemInfo))
			{
				return;
			}
			CUIFormScript x = Singleton<CUIManager>.GetInstance().OpenForm(this.sShopBuyFormPath, false, true);
			if (x == null)
			{
				return;
			}
			this.RefreshBuyPnl();
		}

		private void OnShop_CloseItemBuyForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(this.sShopBuyFormPath);
		}

		private void OnShop_BuyItem(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			stShopItemInfo stShopItemInfo = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint)this.m_CurShopType][this.m_currentSelectItemIdx];
			switch (stShopItemInfo.enCostType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				if (masterRoleInfo.DianQuan < stShopItemInfo.fPrice)
				{
					CUICommonSystem.OpenDianQuanNotEnoughTip();
					return;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				if (masterRoleInfo.GoldCoin < stShopItemInfo.fPrice)
				{
					CUICommonSystem.OpenGoldCoinNotEnoughTip();
					return;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				if (masterRoleInfo.BurningCoin < stShopItemInfo.fPrice)
				{
					CUICommonSystem.OpenBurningCoinNotEnoughTip();
					return;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				if (masterRoleInfo.ArenaCoin < stShopItemInfo.fPrice)
				{
					CUICommonSystem.OpenArenaCoinNotEnoughTip();
					return;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				if (CGuildHelper.GetPlayerGuildConstruct() < stShopItemInfo.fPrice)
				{
					CUICommonSystem.OpenGuildCoinNotEnoughTip();
					return;
				}
				break;
			}
			int num = 0;
			if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
			{
				if (masterRoleInfo.IsHaveHero(stShopItemInfo.dwItemId, false))
				{
					num = 1;
				}
			}
			else if (stShopItemInfo.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				uint heroId = 0u;
				uint skinId = 0u;
				CSkinInfo.ResolveHeroSkin(stShopItemInfo.dwItemId, out heroId, out skinId);
				if (masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
				{
					num = 1;
				}
			}
			else
			{
				CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
				num = useableContainer.GetUseableStackCount(stShopItemInfo.enItemType, stShopItemInfo.dwItemId);
			}
			CUseable cUseable = CUseableManager.CreateUseable(stShopItemInfo.enItemType, stShopItemInfo.dwItemId, 0);
			if (num >= cUseable.m_stackMax)
			{
				switch (stShopItemInfo.enItemType)
				{
				case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("hasOwnHero"), false, 1.5f, null, new object[0]);
					return;
				case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Hero_SkinBuy_Has_Own"), false, 1.5f, null, new object[0]);
					return;
				}
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_1", new string[]
				{
					cUseable.m_name,
					cUseable.m_stackMax.ToString()
				}), false, 1.5f, null, new object[0]);
				return;
			}
			switch (stShopItemInfo.enCostType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Pay_Tip"), stShopItemInfo.fPrice, stShopItemInfo.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Pay_Tip"), stShopItemInfo.fPrice, stShopItemInfo.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_Burning_Coin_Pay_Tip"), stShopItemInfo.fPrice, stShopItemInfo.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_Arena_Coin_Pay_Tip"), stShopItemInfo.fPrice, stShopItemInfo.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_GuildCoin_Pay_Tip"), stShopItemInfo.fPrice, stShopItemInfo.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
				break;
			}
			Singleton<CUIManager>.GetInstance().CloseForm(this.sShopBuyFormPath);
		}

		private void OnShop_BuyItemConfirm(CUIEvent uiEvent)
		{
			int iRefreshTime = 0;
			stShopInfo stShopInfo = new stShopInfo();
			if (this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
			{
				iRefreshTime = stShopInfo.dwMaxRefreshTime;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1125u);
			CSPKG_CMD_ITEMBUY cSPKG_CMD_ITEMBUY = new CSPKG_CMD_ITEMBUY();
			cSPKG_CMD_ITEMBUY.wShopType = (ushort)this.m_CurShopType;
			cSPKG_CMD_ITEMBUY.bItemIdx = (byte)this.m_currentSelectItemIdx;
			cSPKG_CMD_ITEMBUY.iRefreshTime = iRefreshTime;
			cSPkg.stPkgData.stItemBuyReq = cSPKG_CMD_ITEMBUY;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void OnShop_ManualRefresh(CUIEvent uiEvent)
		{
			string strContent = string.Empty;
			if (this.ManualRefreshLimit(ref strContent))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
				return;
			}
			int num = this.EnoughMoneyToRefresh(ref strContent);
			if (num != 0)
			{
				int num2 = num;
				if (num2 == -2)
				{
					CUICommonSystem.OpenGoldCoinNotEnoughTip();
					return;
				}
				if (num2 != -1)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
					return;
				}
				CUICommonSystem.OpenDianQuanNotEnoughTip();
				return;
			}
			else
			{
				stShopInfo stShopInfo = new stShopInfo();
				if (!this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
				{
					return;
				}
				ResShopRefreshCost cost = this.GetCost(stShopInfo.enShopType, stShopInfo.dwManualRefreshCnt);
				if (cost == null)
				{
					strContent = CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Shop_Manual_Refresh_Tip"), new string[]
					{
						"0",
						Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Coin"),
						(stShopInfo.dwManualRefreshCnt - 1).ToString()
					});
				}
				else
				{
					string text = string.Empty;
					switch (cost.bCostType)
					{
					case 2:
						text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");
						goto IL_19A;
					case 4:
						text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
						goto IL_19A;
					case 5:
						text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Burning_Coin");
						goto IL_19A;
					case 6:
						text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Arena_Coin");
						goto IL_19A;
					case 8:
						text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Guild_Coin");
						goto IL_19A;
					}
					text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
					IL_19A:
					strContent = CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Shop_Manual_Refresh_Tip"), new string[]
					{
						cost.dwCostPrice.ToString(),
						text,
						(stShopInfo.dwManualRefreshCnt - 1).ToString()
					});
				}
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Shop_ConfirmManualRefresh, enUIEventID.Shop_CancelManualRefresh, false);
				return;
			}
		}

		private void OnShop_ConfirmManualRefresh(CUIEvent uiEvent)
		{
			stShopInfo stShopInfo = new stShopInfo();
			if (!this.m_Shops.TryGetValue((uint)this.m_CurShopType, ref stShopInfo))
			{
				return;
			}
			stShopInfo.bManualRefreshSent = true;
			this.m_Shops.set_Item((uint)this.m_CurShopType, stShopInfo);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1119u);
			CSPKG_CMD_MANUALREFRESH cSPKG_CMD_MANUALREFRESH = new CSPKG_CMD_MANUALREFRESH();
			cSPKG_CMD_MANUALREFRESH.wShopType = (ushort)this.m_CurShopType;
			cSPkg.stPkgData.stManualRefresh = cSPKG_CMD_MANUALREFRESH;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void OnShop_SaleTipCancel(CUIEvent uiEvent)
		{
			GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
		}

		public void OnShop_SaleTipConfirm(CUIEvent uiEvent)
		{
			GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
			ListView<CSDT_ITEM_DELINFO> listView = new ListView<CSDT_ITEM_DELINFO>();
			for (int i = 0; i < (int)this.m_saleItemsCnt; i++)
			{
				CUseable cUseable = this.aSaleItems[i];
				if (cUseable != null)
				{
					listView.Add(new CSDT_ITEM_DELINFO
					{
						ullUniqueID = cUseable.m_objID,
						iItemCnt = (int)((ushort)cUseable.m_stackCount)
					});
				}
			}
			if (listView.Count > 0)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1101u);
				CSDT_ITEM_DELLIST cSDT_ITEM_DELLIST = new CSDT_ITEM_DELLIST();
				cSDT_ITEM_DELLIST.wItemCnt = (ushort)listView.Count;
				cSDT_ITEM_DELLIST.astItemList = LinqS.ToArray<CSDT_ITEM_DELINFO>(listView);
				cSPkg.stPkgData.stItemSale.stSaleList = cSDT_ITEM_DELLIST;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		private void OnShop_AutoRefreshTimerTimeUp(CUIEvent uiEvent)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
		}

		private void OnShop_PageUp(CUIEvent uiEvent)
		{
			if (!this.m_IsShopFormOpen)
			{
				return;
			}
			GameObject widget = this.m_ShopForm.GetWidget(1);
			CUIListScript component = widget.transform.FindChild("List").GetComponent<CUIListScript>();
			this.m_CurPage -= 1;
			if (component != null)
			{
				component.MoveElementInScrollArea((int)this.m_CurPage, true);
			}
		}

		private void OnShop_PageDown(CUIEvent uiEvent)
		{
			if (!this.m_IsShopFormOpen)
			{
				return;
			}
			GameObject widget = this.m_ShopForm.GetWidget(1);
			CUIListScript component = widget.transform.FindChild("List").GetComponent<CUIListScript>();
			this.m_CurPage += 1;
			if (component != null)
			{
				component.MoveElementInScrollArea((int)this.m_CurPage, true);
			}
		}

		private void OnShop_GetBurningCoin(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Shop_Expedition_Shop_Go"), enUIEventID.Shop_GetBurningCoinConfirm, enUIEventID.None, false);
		}

		private void OnShop_GetBurningCoinConfirm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseForm);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseItemForm);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
		}

		private void OnShop_GetArenaCoin(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Shop_Arena_Shop_Go"), enUIEventID.Shop_GetArenaCoinConfirm, enUIEventID.None, false);
		}

		private void OnShop_GetArenaCoinConfirm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseForm);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseItemForm);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
		}

		private void MasterAttrChanged()
		{
			if (this.m_IsShopFormOpen)
			{
				this.RefreshShop(this.m_CurShopType);
			}
		}

		private void OnRequestAutoRefreshShopItems()
		{
			if (!this.NeedAutoRefreshShop(this.m_CurShopType))
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, this.m_CurShopType);
				Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1118u);
			CSPKG_CMD_AUTOREFRESH cSPKG_CMD_AUTOREFRESH = new CSPKG_CMD_AUTOREFRESH();
			cSPKG_CMD_AUTOREFRESH.wShopType = (ushort)this.m_CurShopType;
			cSPkg.stPkgData.stAutoRefresh = cSPKG_CMD_AUTOREFRESH;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			this.m_TimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(1500, 1, new CTimer.OnTimeUpHandler(this.UseOldShopItems));
		}

		private void OnReceiveShopItems(RES_SHOP_TYPE shopType)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_TimerSeq);
			if (shopType == RES_SHOP_TYPE.RES_SHOPTYPE_FIXED)
			{
				this.m_IsNormalShopItemsInited = true;
			}
			this.RefreshShop(shopType);
		}

		[MessageHandler(1127)]
		public static void ShopTimeOut(CSPkg msg)
		{
			SCPKG_NTF_SHOPTIMEOUT stShopTimeOut = msg.stPkgData.stShopTimeOut;
			string text = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Shop_Time_Out");
			string text2 = string.Empty;
			RES_SHOP_TYPE wShopType = (RES_SHOP_TYPE)stShopTimeOut.wShopType;
			if (wShopType != RES_SHOP_TYPE.RES_SHOPTYPE_FIXED)
			{
				if (wShopType != RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
				{
					text2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed");
				}
				else
				{
					text2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Mystery");
				}
			}
			else
			{
				text2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed");
			}
			text = CUIUtility.StringReplace(text, new string[]
			{
				text2
			});
			Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
		}

		[MessageHandler(1126)]
		public static void ReceiveItemBuy(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CShopSystem instance = Singleton<CShopSystem>.GetInstance();
			SCPKG_CMD_ITEMBUY stItemBuyRsp = msg.stPkgData.stItemBuyRsp;
			int num = 0;
			stShopInfo stShopInfo = new stShopInfo();
			if (instance.m_Shops.TryGetValue((uint)instance.m_CurShopType, ref stShopInfo))
			{
				num = stShopInfo.dwMaxRefreshTime;
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.shoppingSuccess, new uint[0]);
			if (stItemBuyRsp.iRefreshTime != num)
			{
				return;
			}
			instance.m_ShopItems[(uint)instance.m_CurShopType][(int)stItemBuyRsp.bItemIdx].isSoldOut = true;
			Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, instance.m_CurShopType);
			stShopItemInfo stShopItemInfo = instance.m_ShopItems[(uint)instance.m_CurShopType][(int)stItemBuyRsp.bItemIdx];
			switch (stShopItemInfo.enItemType)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
				CUICommonSystem.ShowNewHeroOrSkin(stShopItemInfo.dwItemId, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority2, 0u, 0);
				break;
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
			{
				uint heroId = 0u;
				uint skinId = 0u;
				CSkinInfo.ResolveHeroSkin(stShopItemInfo.dwItemId, out heroId, out skinId);
				CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority2, 0u, 0);
				break;
			}
			}
		}

		[MessageHandler(1120)]
		public static void ReceiveShopItems(CSPkg msg)
		{
			COMDT_SHOP_DETAIL stShopInfo = msg.stPkgData.stShopDetail.stShopInfo;
			COMDT_SHOP_ITEMLIST stItemList = stShopInfo.stItemList;
			COMDT_SHOP_ITEMINFO[] astShopItem = stItemList.astShopItem;
			RES_SHOP_TYPE wShopType = (RES_SHOP_TYPE)stShopInfo.wShopType;
			CShopSystem instance = Singleton<CShopSystem>.GetInstance();
			stShopInfo stShopInfo2 = new stShopInfo();
			stShopInfo2.enShopType = wShopType;
			stShopInfo2.dwAutoRefreshTime = stShopInfo.iAutoRefreshTime;
			stShopInfo2.dwManualRefreshTime = stShopInfo.iManualRefreshTime;
			stShopInfo2.dwApRefreshTime = stShopInfo.iAPRefreshTime;
			stShopInfo2.dwManualRefreshCnt = stShopInfo.iManualRefreshCnt + 1;
			ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint)((ushort)wShopType));
			if (dataByKey != null)
			{
				stShopInfo2.dwManualRefreshLimit = dataByKey.iManualLimit;
			}
			else
			{
				stShopInfo2.dwManualRefreshLimit = 0;
			}
			stShopInfo2.dwMaxRefreshTime = stShopInfo2.dwAutoRefreshTime;
			if (stShopInfo2.dwMaxRefreshTime < stShopInfo2.dwManualRefreshTime)
			{
				stShopInfo2.dwMaxRefreshTime = stShopInfo2.dwManualRefreshTime;
			}
			else if (stShopInfo2.dwMaxRefreshTime < stShopInfo2.dwApRefreshTime)
			{
				stShopInfo2.dwMaxRefreshTime = stShopInfo2.dwApRefreshTime;
			}
			stShopInfo2.dwItemValidTime = stShopInfo.iItemValidTime;
			stShopInfo2.bManualRefreshSent = false;
			if (!instance.m_Shops.ContainsKey((uint)wShopType))
			{
				instance.m_Shops.Add((uint)wShopType, stShopInfo2);
			}
			else
			{
				instance.m_Shops.set_Item((uint)wShopType, stShopInfo2);
			}
			if (instance.m_ShopItems.ContainsKey((uint)wShopType))
			{
				instance.m_ShopItems[(uint)wShopType] = new stShopItemInfo[(int)stItemList.bItemCnt];
			}
			else
			{
				instance.m_ShopItems.Add((uint)wShopType, new stShopItemInfo[(int)stItemList.bItemCnt]);
			}
			for (byte b = 0; b < stItemList.bItemCnt; b += 1)
			{
				COMDT_SHOP_ITEMINFO cOMDT_SHOP_ITEMINFO = astShopItem[(int)b];
				stShopItemInfo stShopItemInfo = default(stShopItemInfo);
				stShopItemInfo.dwItemId = cOMDT_SHOP_ITEMINFO.dwItemID;
				stShopItemInfo.wItemCnt = cOMDT_SHOP_ITEMINFO.wItemCnt;
				stShopItemInfo.enItemType = (COM_ITEM_TYPE)cOMDT_SHOP_ITEMINFO.wItemType;
				stShopItemInfo.enCostType = (RES_SHOPBUY_COINTYPE)cOMDT_SHOP_ITEMINFO.bCostType;
				stShopItemInfo.wSaleDiscount = cOMDT_SHOP_ITEMINFO.wSaleDiscout;
				if (cOMDT_SHOP_ITEMINFO.bIsBuy == 1)
				{
					stShopItemInfo.isSoldOut = true;
				}
				else
				{
					stShopItemInfo.isSoldOut = false;
				}
				instance.m_ShopItems[(uint)wShopType][(int)b] = stShopItemInfo;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, wShopType);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}
	}
}

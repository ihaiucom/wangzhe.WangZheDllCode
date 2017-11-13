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
	public class CMallSystem : Singleton<CMallSystem>
	{
		public enum enMallFormWidget
		{
			Tab,
			Action_Mask,
			Top_Common,
			Body_Container,
			Lottery_Result_Mask,
			Action_Mask_Reset_Timer,
			Skip_Mask,
			Skip_Mask_Reset_Timer,
			Update_Sub_Module_Timer,
			BG,
			SortContainer,
			LoadingIcon,
			BG_2,
			SymbolCoinCntPanel
		}

		public enum Tab
		{
			None = -1,
			Mystery,
			Boutique,
			Recommend,
			Hero,
			Skin,
			Symbol_Make,
			Factory_Shop,
			Roulette,
			TabCount
		}

		private const int MAX_TAGINFO_COUNT = 8;

		public static string[] s_payPurposeNameKeys = new string[]
		{
			"PayPurpose_Buy",
			"PayPurpose_Relive",
			"PayPurpose_GetSymbolGift",
			"PayPurpose_Roulette",
			"PayPurpose_Open",
			"PayPurpose_Chat",
			"PayPurpose_RecommendLottery"
		};

		public static string[] s_payTypeNameKeys = new string[]
		{
			"PayType_NotSupport",
			"PayType_GoldCoin",
			"PayType_DianQuan",
			"PayType_Diamond",
			"PayType_Diamond",
			"PayType_BurningCoin",
			"PayType_ArenaCoin",
			"PayType_GuildCoin",
			"PayType_SymbolCoin"
		};

		private List<CMallSystem.Tab> m_AvailableTabList;

		public string sMallFormPath = "UGUI/Form/System/Mall/Form_Mall.prefab";

		public bool m_IsMallFormOpen;

		public CUIFormScript m_MallForm;

		private CMallSystem.Tab m_CurTab;

		private int m_TargetId;

		private int m_FreeDrawSymbolTimerSeq = -1;

		public bool IsNewHeroShow;

		public static Dictionary<enPayType, CSDT_LUCKYDRAW_INFO> luckyDrawDic = new Dictionary<enPayType, CSDT_LUCKYDRAW_INFO>();

		public CMallHeroController m_heroMallCtrl = new CMallHeroController();

		public CMallSkinController m_skinMallCtrl = new CMallSkinController();

		private COMDT_REDDOT_INFO[] m_TagInfoList = new COMDT_REDDOT_INFO[8];

		private GiftCenter _giftCenter = new GiftCenter();

		public DateTime m_PlayerRegisterTime = default(DateTime);

		public CMallSystem.Tab CurTab
		{
			get
			{
				return this.m_CurTab;
			}
			set
			{
				this.m_CurTab = value;
			}
		}

		public int TargetID
		{
			get
			{
				int targetId = this.m_TargetId;
				this.m_TargetId = 0;
				return targetId;
			}
			set
			{
				this.m_TargetId = value;
			}
		}

		public void ClearTagInfoList()
		{
			for (int i = 0; i < this.m_TagInfoList.Length; i++)
			{
				this.m_TagInfoList[i] = new COMDT_REDDOT_INFO();
			}
		}

		public void SetTagInfo(int idx, COMDT_REDDOT_INFO info)
		{
			if (idx >= 0 && idx < 8)
			{
				this.m_TagInfoList[idx] = info;
			}
		}

		private COMDT_REDDOT_INFO GetTagInfo(int idx)
		{
			COMDT_REDDOT_INFO cOMDT_REDDOT_INFO = null;
			if (this.m_TagInfoList != null)
			{
				for (int i = 0; i < this.m_TagInfoList.Length; i++)
				{
					cOMDT_REDDOT_INFO = this.m_TagInfoList[i];
					if (cOMDT_REDDOT_INFO != null && (int)cOMDT_REDDOT_INFO.bRedDotLabelType == idx)
					{
						break;
					}
				}
			}
			return cOMDT_REDDOT_INFO;
		}

		private void SetTagImg(int idx, GameObject imgObj)
		{
			if (imgObj == null)
			{
				return;
			}
			COMDT_REDDOT_INFO tagInfo = this.GetTagInfo(idx);
			if (tagInfo == null)
			{
				return;
			}
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			if (currentUTCTime >= tagInfo.dwStartTime && currentUTCTime <= tagInfo.dwEndTime)
			{
				imgObj.CustomSetActive(true);
				string prefabPath = string.Format("{0}Type{1}.prefab", CLobbySystem.m_TagIConPath, (int)(tagInfo.bRedDotLabelType - 3 + 1));
				CUIUtility.SetImageSprite(imgObj.GetComponent<Image>(), prefabPath, null, true, false, false, false);
				if (imgObj.GetComponentInChildren<Text>())
				{
					imgObj.GetComponentInChildren<Text>().set_text(Utility.UTF8Convert(tagInfo.szContent));
				}
			}
			else
			{
				imgObj.CustomSetActive(false);
			}
		}

		private void SetTagIcon(Transform imgObj, string iconPath, string text, bool bShow)
		{
			if (imgObj == null)
			{
				return;
			}
			imgObj.gameObject.CustomSetActive(bShow);
			if (bShow)
			{
				CUIUtility.SetImageSprite(imgObj.GetComponent<Image>(), iconPath, null, true, false, false, false);
				if (imgObj.GetComponentInChildren<Text>())
				{
					imgObj.GetComponentInChildren<Text>().set_text(text);
				}
			}
		}

		public void OpenGiftCenter(ulong uId, uint worldId, bool isSns = false)
		{
			this._giftCenter.OpenGiftCenter(uId, worldId, isSns);
		}

		public override void Init()
		{
			base.Init();
			this.ClearTagInfoList();
			if (this.m_heroMallCtrl == null)
			{
				this.m_heroMallCtrl = new CMallHeroController();
			}
			this.m_heroMallCtrl.Init();
			if (this.m_skinMallCtrl == null)
			{
				this.m_skinMallCtrl = new CMallSkinController();
			}
			this.m_skinMallCtrl.Init();
			this.m_AvailableTabList = new List<CMallSystem.Tab>(8);
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
			instance.AddUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
			instance.AddUIEventListener(enUIEventID.Mall_Mall_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnMallTabChange));
			instance.AddUIEventListener(enUIEventID.Mall_GoToBoutiqueTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoBoutique));
			instance.AddUIEventListener(enUIEventID.HeroInfo_GotoMall, new CUIEventManager.OnUIEventHandler(this.OnMalOpenHeroTab));
			instance.AddUIEventListener(enUIEventID.Mall_Open_Factory_Shop_Tab, new CUIEventManager.OnUIEventHandler(this.OnMallOpenFactoryShopTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToSymbolTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSymbolTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToMysteryTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToMysteryTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSkinTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToTreasureTab));
			instance.AddUIEventListener(enUIEventID.Mall_GotoCouponsTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoCouponsTreasureTab));
			instance.AddUIEventListener(enUIEventID.Mall_GotoDianmondTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoDiamondTreasureTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToRecommendHeroTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendHeroTab));
			instance.AddUIEventListener(enUIEventID.Mall_GoToRecommendSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendSkinTab));
			instance.AddUIEventListener(enUIEventID.Mall_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallActionMaskReset));
			instance.AddUIEventListener(enUIEventID.Mall_Skip_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallSkipMaskReset));
			instance.AddUIEventListener(enUIEventID.Mall_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectSortType));
			instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Change, new CUIEventManager.OnUIEventHandler(this.OnSortTypeChange));
			instance.AddUIEventListener(enUIEventID.Mall_Buy_Product_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyProduct));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Set_Free_Draw_Timer, new Action(this.SetFreeDrawTimer));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sub_Module_Loaded, new Action(this.OnSubModuleLoaded));
			Singleton<EventRouter>.instance.AddEventHandler<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, new Action<CMallSystem.Tab>(this.RefreshTabRedDot));
			this.m_MallForm = null;
			this.m_IsMallFormOpen = false;
			this.m_CurTab = CMallSystem.Tab.None;
			this.m_TargetId = 0;
			this._giftCenter.Init();
		}

		public override void UnInit()
		{
			base.UnInit();
			this._giftCenter.UnInit();
			if (this.m_heroMallCtrl != null)
			{
				this.m_heroMallCtrl.UnInit();
			}
			if (this.m_skinMallCtrl != null)
			{
				this.m_skinMallCtrl.UnInit();
			}
			this.m_AvailableTabList = null;
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
			instance.RemoveUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
			instance.RemoveUIEventListener(enUIEventID.Mall_Mall_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnMallTabChange));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToBoutiqueTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoBoutique));
			instance.RemoveUIEventListener(enUIEventID.HeroInfo_GotoMall, new CUIEventManager.OnUIEventHandler(this.OnMalOpenHeroTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_Open_Factory_Shop_Tab, new CUIEventManager.OnUIEventHandler(this.OnMallOpenFactoryShopTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToSymbolTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSymbolTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToMysteryTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToMysteryTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSkinTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToTreasureTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GotoCouponsTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoCouponsTreasureTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GotoDianmondTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoDiamondTreasureTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToRecommendHeroTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendHeroTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_GoToRecommendSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendSkinTab));
			instance.RemoveUIEventListener(enUIEventID.Mall_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallActionMaskReset));
			instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallSkipMaskReset));
			instance.RemoveUIEventListener(enUIEventID.Mall_Buy_Product_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyProduct));
			instance.RemoveUIEventListener(enUIEventID.Mall_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectSortType));
			instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Change, new CUIEventManager.OnUIEventHandler(this.OnSortTypeChange));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Set_Free_Draw_Timer, new Action(this.SetFreeDrawTimer));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Sub_Module_Loaded, new Action(this.OnSubModuleLoaded));
			Singleton<EventRouter>.instance.RemoveEventHandler<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, new Action<CMallSystem.Tab>(this.RefreshTabRedDot));
			this.m_MallForm = null;
		}

		public bool IsOpenMall()
		{
			return !CSysDynamicBlock.bLobbyEntryBlocked;
		}

		public void OnMallOpenForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "1";
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "1";
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
			if (!this.IsOpenMall())
			{
				return;
			}
			this.m_MallForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sMallFormPath, false, true);
			if (this.m_MallForm == null)
			{
				return;
			}
			this.m_IsMallFormOpen = true;
			if (uiEvent.m_eventParams.tag != 1)
			{
				if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
				{
					this.CurTab = CMallSystem.Tab.Mystery;
				}
				else
				{
					this.CurTab = CMallSystem.Tab.Boutique;
				}
			}
			if (uiEvent.m_eventParams.tag2 != 0)
			{
				this.TargetID = uiEvent.m_eventParams.tag2;
			}
			else
			{
				this.TargetID = 0;
			}
			this.InitTab();
			GameObject gameObject = this.m_MallForm.gameObject.transform.FindChild("TopCommon/Button_Gift").gameObject;
			Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(gameObject, enNewFlagKey.New_Skin_Gift_V15, enNewFlagPos.enTopRight, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Del_RedDotCheck);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenMall);
		}

		private void OnMallCloseForm(CUIEvent uiEvent)
		{
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_Stop", null);
			Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
			if (!this.m_IsMallFormOpen)
			{
				return;
			}
			this.m_IsMallFormOpen = false;
			Singleton<CUIManager>.GetInstance().CloseForm(this.sMallFormPath);
			Singleton<CResourceManager>.instance.UnloadUnusedAssets();
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Close_Mall);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Del_RedDotCheck);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
			this.m_MallForm = null;
		}

		private void OnMallTabChange(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			Singleton<CUIManager>.GetInstance().CloseTips();
			CUICommonSystem.CloseCommonTips();
			CUICommonSystem.CloseUseableTips();
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int num = component.GetSelectedIndex();
			if (num < 0 || num >= this.m_AvailableTabList.get_Count())
			{
				num = 0;
			}
			this.CurTab = this.m_AvailableTabList.get_Item(num);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Change_Tab);
			if (this.m_MallForm != null)
			{
				Transform transform = this.m_MallForm.transform.Find("TopCommon/Button_Gift");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(this.CurTab == CMallSystem.Tab.Boutique);
				}
				Transform transform2 = this.m_MallForm.transform.Find("TopCommon/Button_Crystal");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(this.CurTab == CMallSystem.Tab.Roulette);
				}
				if (this.CurTab == CMallSystem.Tab.Symbol_Make)
				{
					GameObject widget = this.m_MallForm.GetWidget(13);
					CSymbolSystem.RefreshSymbolCntText(true);
					CUIEventScript component2 = widget.GetComponent<CUIEventScript>();
					if (component2 != null)
					{
						CUseable iconUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, 0);
						stUIEventParams eventParams = default(stUIEventParams);
						eventParams.iconUseable = iconUseable;
						eventParams.tag = 3;
						component2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
						component2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
						component2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
						component2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
					}
				}
				else
				{
					this.m_MallForm.GetWidget(13).CustomSetActive(false);
				}
			}
			if (this.m_MallForm != null && this.m_IsMallFormOpen)
			{
				Transform transform3 = this.m_MallForm.transform.Find("pnlBodyBg/pnlBoutique");
				if (transform3 != null)
				{
					transform3.gameObject.CustomSetActive(false);
				}
				Transform transform4 = this.m_MallForm.transform.Find("pnlBodyBg/pnlRecommend");
				if (transform4 != null)
				{
					transform4.gameObject.CustomSetActive(false);
				}
				Transform transform5 = this.m_MallForm.transform.Find("pnlBodyBg/pnlFactoryShop");
				if (transform5 != null)
				{
					transform5.gameObject.CustomSetActive(false);
				}
				Transform transform6 = this.m_MallForm.transform.Find("pnlBodyBg/pnlBuyHero");
				if (transform6 != null)
				{
					transform6.gameObject.CustomSetActive(false);
				}
				Transform transform7 = this.m_MallForm.transform.Find("pnlBodyBg/pnlBuySkin");
				if (transform7 != null)
				{
					transform7.gameObject.CustomSetActive(false);
				}
				Transform transform8 = this.m_MallForm.transform.Find(string.Format("{0}{1}", "pnlBodyBg/", CSymbolSystem.s_symbolMakePanel));
				if (transform8 != null)
				{
					transform8.gameObject.CustomSetActive(false);
				}
				Transform transform9 = this.m_MallForm.transform.Find("pnlBodyBg/pnlRoulette");
				if (transform9 != null)
				{
					transform9.gameObject.CustomSetActive(false);
				}
				Transform transform10 = this.m_MallForm.transform.Find("pnlBodyBg/pnlMystery");
				if (transform10 != null)
				{
					transform10.gameObject.CustomSetActive(false);
				}
			}
			this.LoadSubModule();
			if (component.GetLastSelectedIndex() != -1)
			{
				enRedID enRedID = CMallSystem.TabToRedID(this.CurTab);
				if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID))
				{
					CUIListElementScript selectedElement = component.GetSelectedElement();
					if (selectedElement != null)
					{
						CUIRedDotSystem.SetRedDotViewByVersion(enRedID);
						CUICommonSystem.DelRedDot(selectedElement.gameObject);
					}
				}
			}
		}

		private void OnUpdateSubModule(CUIEvent uiEvent)
		{
			this.m_MallForm.GetWidget(11).CustomSetActive(false);
			if (this.m_CurTab == CMallSystem.Tab.Recommend)
			{
				this.m_MallForm.GetWidget(9).CustomSetActive(false);
				this.m_MallForm.GetWidget(12).CustomSetActive(true);
			}
			else
			{
				this.m_MallForm.GetWidget(9).CustomSetActive(true);
				this.m_MallForm.GetWidget(12).CustomSetActive(false);
			}
			this.m_MallForm.GetWidget(3).CustomSetActive(true);
			switch (this.m_CurTab)
			{
			case CMallSystem.Tab.Mystery:
				Singleton<MySteryShop>.GetInstance().UpdateUI();
				break;
			case CMallSystem.Tab.Boutique:
				Singleton<CMallBoutiqueController>.GetInstance().Draw(this.m_MallForm);
				break;
			case CMallSystem.Tab.Recommend:
				Singleton<CMallRecommendController>.GetInstance().Draw(this.m_MallForm);
				break;
			case CMallSystem.Tab.Hero:
				this.m_heroMallCtrl.Draw(this.m_MallForm);
				break;
			case CMallSystem.Tab.Skin:
				this.m_skinMallCtrl.Draw(this.m_MallForm);
				break;
			case CMallSystem.Tab.Symbol_Make:
				Singleton<CSymbolMakeController>.GetInstance().SwitchToSymbolMakePanel(this.m_MallForm);
				break;
			case CMallSystem.Tab.Factory_Shop:
				Singleton<CMallFactoryShopController>.GetInstance().Draw(this.m_MallForm);
				break;
			case CMallSystem.Tab.Roulette:
				Singleton<CMallRouletteController>.GetInstance().Draw(this.m_MallForm);
				break;
			}
		}

		private void OnSelectSortType(CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(10);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("dropList/List");
			transform.gameObject.CustomSetActive(!transform.gameObject.activeSelf);
		}

		private void OnSortTypeChange(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex();
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(10);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("dropList/List");
			transform.gameObject.CustomSetActive(false);
			CMallSystem.Tab curTab = this.CurTab;
			if (curTab != CMallSystem.Tab.Hero)
			{
				if (curTab == CMallSystem.Tab.Skin && Enum.IsDefined(typeof(CMallSortHelper.SkinSortType), selectedIndex))
				{
					CMallSortHelper.CreateSkinSorter().SetSortType((CMallSortHelper.SkinSortType)selectedIndex);
					this.UpdateSortContainer();
					Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sort_Type_Changed);
				}
			}
			else if (Enum.IsDefined(typeof(CMallSortHelper.HeroSortType), selectedIndex))
			{
				CMallSortHelper.CreateHeroSorter().SetSortType((CMallSortHelper.HeroSortType)selectedIndex);
				this.UpdateSortContainer();
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sort_Type_Changed);
			}
		}

		private void OnConfirmBuyProduct(CUIEvent uiEvent)
		{
			uint key = (uint)uiEvent.m_eventParams.commonUInt64Param1;
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
			if (product != null)
			{
				Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(product, commonUInt32Param);
			}
		}

		private void OnMallGotoBoutique(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Boutique;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMalOpenHeroTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Hero;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallOpenFactoryShopTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Factory_Shop;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			cUIEvent.m_eventParams.tag2 = uiEvent.m_eventParams.tag2;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGotoSymbolTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Symbol_Make;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGoToMysteryTab(CUIEvent uiEvent)
		{
			if (!Singleton<MySteryShop>.GetInstance().IsShopAvailable())
			{
				return;
			}
			this.CurTab = CMallSystem.Tab.Mystery;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGotoSkinTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Skin;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGoToTreasureTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Roulette;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGotoCouponsTreasureTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Roulette;
			Singleton<CMallRouletteController>.GetInstance().CurTab = CMallRouletteController.Tab.DianQuan;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGotoDiamondTreasureTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Roulette;
			Singleton<CMallRouletteController>.GetInstance().CurTab = CMallRouletteController.Tab.Diamond;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGoToRecommendHeroTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Recommend;
			Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Hero;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallGoToRecommendSkinTab(CUIEvent uiEvent)
		{
			this.CurTab = CMallSystem.Tab.Recommend;
			Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Skin;
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Mall_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
		}

		private void OnMallActionMaskReset(CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(1);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
		}

		private void OnMallSkipMaskReset(CUIEvent uiEvent)
		{
			GameObject widget = uiEvent.m_srcFormScript.GetWidget(6);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
		}

		public void RefreshTabRedDot(CMallSystem.Tab tab)
		{
			if (this.m_MallForm == null || !this.m_IsMallFormOpen)
			{
				return;
			}
			int tabIndex = this.GetTabIndex(tab);
			GameObject widget = this.m_MallForm.GetWidget(0);
			CUIListScript cUIListScript = (widget != null) ? widget.GetComponent<CUIListScript>() : null;
			if (cUIListScript != null)
			{
				int elementAmount = cUIListScript.GetElementAmount();
				if (tabIndex >= 0 && tabIndex < elementAmount)
				{
					CUIListElementScript elemenet = cUIListScript.GetElemenet(tabIndex);
					if (elemenet == null)
					{
						return;
					}
					enRedID redID = CMallSystem.TabToRedID(tab);
					if (CUIRedDotSystem.IsShowRedDotByVersion(redID) || CUIRedDotSystem.IsShowRedDotByLogic(redID))
					{
						CUIRedDotSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					}
					if (tab == CMallSystem.Tab.Symbol_Make && !CUIRedDotSystem.IsShowRedDotByLogic(redID) && !CUIRedDotSystem.IsShowRedDotByVersion(redID))
					{
						CUIRedDotSystem.DelRedDot(elemenet.gameObject);
					}
					if (tab == CMallSystem.Tab.Mystery && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) || !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))
					{
						CUIRedDotSystem.DelRedDot(elemenet.gameObject);
					}
				}
			}
		}

		public void SetFreeDrawTimer()
		{
			if (!(Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState))
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_FreeDrawSymbolTimerSeq);
			int num = 4;
			if (num < 0 || num >= masterRoleInfo.m_freeDrawInfo.Length)
			{
				return;
			}
			int num2 = Math.Max(0, masterRoleInfo.m_freeDrawInfo[num].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime());
			long num3 = (long)(num2 * 1000);
			if (num3 < 2147483647L)
			{
				this.m_FreeDrawSymbolTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int)num3, 1, new CTimer.OnTimeUpHandler(this.SymbolFreeDrawTimerHandler));
			}
		}

		public void SymbolFreeDrawTimerHandler(int seq)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			int num = 4;
			if (masterRoleInfo.m_freeDrawInfo[num].dwLeftFreeDrawCnt == 0 && masterRoleInfo.m_freeDrawInfo[num].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime() <= 0)
			{
				masterRoleInfo.m_freeDrawInfo[num].dwLeftFreeDrawCnt = 1;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref seq);
			this.m_FreeDrawSymbolTimerSeq = -1;
		}

		public bool HasFreeDrawCnt(enRedID redID)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return false;
			}
			if (redID == enRedID.Mall_SymbolTab)
			{
				int num = 4;
				if (masterRoleInfo.m_freeDrawInfo[num].dwLeftFreeDrawCnt > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasFreeDrawCnt()
		{
			return false;
		}

		public bool HasFreeDrawCnt(CMallSystem.Tab tab)
		{
			return false;
		}

		public void OnSubModuleLoaded()
		{
		}

		public void LoadSubModule()
		{
			DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
			if (this.m_MallForm == null)
			{
				return;
			}
			bool flag = false;
			if (this.m_MallForm.GetWidget(3) != null)
			{
				switch (this.m_CurTab)
				{
				case CMallSystem.Tab.Mystery:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					flag = Singleton<MySteryShop>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<MySteryShop>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Boutique:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					flag = Singleton<CMallBoutiqueController>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<CMallBoutiqueController>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Recommend:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					flag = Singleton<CMallRecommendController>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<CMallRecommendController>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Hero:
					this.m_MallForm.GetWidget(10).CustomSetActive(true);
					this.ResetSortTypeList();
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "5";
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "5";
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
					flag = this.m_heroMallCtrl.Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						this.m_heroMallCtrl.Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Skin:
					this.m_MallForm.GetWidget(10).CustomSetActive(true);
					this.ResetSortTypeList();
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "6";
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "6";
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
					flag = this.m_skinMallCtrl.Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						this.m_skinMallCtrl.Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Symbol_Make:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "2";
					Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "2";
					Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
					Singleton<CSymbolMakeController>.GetInstance().Source = enSymbolMakeSource.Mall;
					flag = Singleton<CSymbolMakeController>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<CSymbolMakeController>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Factory_Shop:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					flag = Singleton<CMallFactoryShopController>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<CMallFactoryShopController>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				case CMallSystem.Tab.Roulette:
					this.m_MallForm.GetWidget(10).CustomSetActive(false);
					flag = Singleton<CMallRouletteController>.GetInstance().Loaded(this.m_MallForm);
					if (!flag)
					{
						this.m_MallForm.GetWidget(11).CustomSetActive(true);
						Singleton<CMallRouletteController>.GetInstance().Load(this.m_MallForm);
						this.m_MallForm.GetWidget(3).CustomSetActive(false);
					}
					break;
				}
			}
			if (!flag)
			{
				GameObject widget = this.m_MallForm.GetWidget(8);
				if (widget != null)
				{
					CUITimerScript component = widget.GetComponent<CUITimerScript>();
					if (component != null)
					{
						component.ReStartTimer();
					}
				}
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Update_Sub_Module);
			}
		}

		private void ResetSortTypeList()
		{
			DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
			if (this.m_MallForm == null)
			{
				return;
			}
			GameObject widget = this.m_MallForm.GetWidget(10);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("dropList/List");
			if (transform == null)
			{
				return;
			}
			CUIListScript component = transform.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			int num = 0;
			CMallSystem.Tab curTab = this.CurTab;
			if (curTab != CMallSystem.Tab.Hero)
			{
				if (curTab == CMallSystem.Tab.Skin)
				{
					num = 5;
					component.SetElementAmount(num);
				}
			}
			else
			{
				num = 5;
				component.SetElementAmount(num);
			}
			for (int i = 0; i < num; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				if (!(elemenet == null))
				{
					Transform transform2 = elemenet.transform.Find("Text");
					if (!(transform2 == null))
					{
						Text component2 = transform2.GetComponent<Text>();
						if (component2 != null)
						{
							curTab = this.CurTab;
							if (curTab != CMallSystem.Tab.Hero)
							{
								if (curTab == CMallSystem.Tab.Skin)
								{
									component2.set_text(CMallSortHelper.CreateSkinSorter().GetSortTypeName((CMallSortHelper.SkinSortType)i));
								}
							}
							else
							{
								component2.set_text(CMallSortHelper.CreateHeroSorter().GetSortTypeName((CMallSortHelper.HeroSortType)i));
							}
						}
					}
				}
			}
			component.m_alwaysDispatchSelectedChangeEvent = true;
			component.SelectElement(0, true);
		}

		private void UpdateSortContainer()
		{
			DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
			if (this.m_MallForm == null)
			{
				return;
			}
			GameObject widget = this.m_MallForm.GetWidget(10);
			if (widget == null)
			{
				return;
			}
			Transform x = widget.transform.Find("dropList/Button_Down/Text");
			if (x == null)
			{
				return;
			}
			CMallSystem.Tab curTab = this.CurTab;
			if (curTab != CMallSystem.Tab.Hero)
			{
				if (curTab == CMallSystem.Tab.Skin)
				{
					Text component = widget.transform.Find("dropList/Button_Down/Text").GetComponent<Text>();
					if (component != null)
					{
						IMallSort<CMallSortHelper.SkinSortType> mallSort = CMallSortHelper.CreateSkinSorter();
						component.set_text(mallSort.GetSortTypeName(mallSort.GetCurSortType()));
					}
				}
			}
			else
			{
				Text component2 = widget.transform.Find("dropList/Button_Down/Text").GetComponent<Text>();
				if (component2 != null)
				{
					IMallSort<CMallSortHelper.HeroSortType> mallSort2 = CMallSortHelper.CreateHeroSorter();
					component2.set_text(mallSort2.GetSortTypeName(mallSort2.GetCurSortType()));
				}
			}
		}

		private void SetAvailableTabs()
		{
			this.m_AvailableTabList.Clear();
			int num = 8;
			for (int i = 0; i < num; i++)
			{
				if (i == 0)
				{
					if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
					{
						this.m_AvailableTabList.Add(CMallSystem.Tab.Mystery);
					}
				}
				else
				{
					this.m_AvailableTabList.Add((CMallSystem.Tab)i);
				}
			}
		}

		public int GetTabIndex(CMallSystem.Tab tab)
		{
			return this.m_AvailableTabList.FindIndex((CMallSystem.Tab t) => tab == t);
		}

		private void InitTab()
		{
			this.SetAvailableTabs();
			string[] array = new string[this.m_AvailableTabList.get_Count()];
			byte b = 0;
			while ((int)b < array.Length)
			{
				switch (this.m_AvailableTabList.get_Item((int)b))
				{
				case CMallSystem.Tab.Mystery:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Mystery");
					break;
				case CMallSystem.Tab.Boutique:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Boutique");
					break;
				case CMallSystem.Tab.Recommend:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_New");
					break;
				case CMallSystem.Tab.Hero:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
					break;
				case CMallSystem.Tab.Skin:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
					break;
				case CMallSystem.Tab.Symbol_Make:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Symbol_Gifts");
					break;
				case CMallSystem.Tab.Factory_Shop:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Factory_Shop");
					break;
				case CMallSystem.Tab.Roulette:
					array[(int)b] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Roulette");
					break;
				}
				b += 1;
			}
			GameObject widget = this.m_MallForm.GetWidget(0);
			CUIListScript cUIListScript = (widget != null) ? widget.GetComponent<CUIListScript>() : null;
			if (cUIListScript != null)
			{
				cUIListScript.SetElementAmount(array.Length);
				for (int i = 0; i < cUIListScript.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = cUIListScript.GetElemenet(i);
					if (elemenet != null)
					{
						Text component = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
						component.set_text(array[i]);
						GameObject gameObject = Utility.FindChild(elemenet.gameObject, "tag");
						gameObject.CustomSetActive(false);
						if (gameObject != null)
						{
							if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Mystery)
							{
								Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "Text");
								if (componetInChild != null)
								{
									componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_LIMIT"));
									gameObject.CustomSetActive(true);
								}
							}
							if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Symbol_Make && (CMallSystem.GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE) != null || CMallSystem.GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE) != null || CMallSystem.GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE) != null || CMallSystem.GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE) != null))
							{
								Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "Text");
								if (componetInChild2 != null)
								{
									componetInChild2.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_DISCOUNT"));
									gameObject.CustomSetActive(true);
								}
							}
							if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Roulette)
							{
								stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.Diamond);
								stPayInfo payInfo2 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.Diamond);
								stPayInfo payInfo3 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.DianQuan);
								stPayInfo payInfo4 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.DianQuan);
								if (payInfo.m_payValue < payInfo.m_oriValue || payInfo2.m_payValue < payInfo2.m_oriValue || payInfo3.m_payValue < payInfo3.m_oriValue || payInfo4.m_payValue < payInfo4.m_oriValue)
								{
									Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject, "Text");
									if (componetInChild3 != null)
									{
										componetInChild3.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_DISCOUNT"));
										gameObject.CustomSetActive(true);
									}
								}
								if (Singleton<CMallRouletteController>.GetInstance().IsProbabilityDoubled(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL))
								{
									Text componetInChild4 = Utility.GetComponetInChild<Text>(gameObject, "Text");
									if (componetInChild4 != null)
									{
										componetInChild4.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_PROBABILITY_DOUBLED"));
										gameObject.CustomSetActive(true);
									}
								}
							}
							if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Boutique)
							{
								this.SetTagImg(4, gameObject);
							}
							else if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Recommend)
							{
								this.SetTagImg(5, gameObject);
							}
							else if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Hero)
							{
								this.SetTagImg(6, gameObject);
							}
							else if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Skin)
							{
								this.SetTagImg(7, gameObject);
							}
							else if (this.m_AvailableTabList.get_Item(i) == CMallSystem.Tab.Factory_Shop)
							{
								this.SetTagImg(8, gameObject);
							}
						}
						if (i >= 0 && i < this.m_AvailableTabList.get_Count())
						{
							this.RefreshTabRedDot(this.m_AvailableTabList.get_Item(i));
						}
					}
				}
				int tabIndex = this.GetTabIndex(this.CurTab);
				if (tabIndex >= 0 && tabIndex < this.m_AvailableTabList.get_Count())
				{
					cUIListScript.SelectElement(tabIndex, true);
				}
				else
				{
					cUIListScript.SelectElement(0, true);
				}
			}
		}

		public static string GetProductTagIconPath(int tagType, bool owned = false)
		{
			if (owned)
			{
				return "UGUI/Sprite/Common/Product_New.prefab";
			}
			switch (tagType)
			{
			case 1:
				return "UGUI/Sprite/Common/Product_Unusual.prefab";
			case 2:
				return "UGUI/Sprite/Common/Product_New.prefab";
			case 3:
				return "UGUI/Sprite/Common/Product_Hot.prefab";
			case 4:
				return "UGUI/Sprite/Common/Product_Discount.prefab";
			default:
				return null;
			}
		}

		public static enRedID TabToRedID(CMallSystem.Tab tab)
		{
			enRedID result = (enRedID)tab;
			switch (tab)
			{
			case CMallSystem.Tab.Mystery:
				result = enRedID.Mall_MysteryTab;
				break;
			case CMallSystem.Tab.Boutique:
				result = enRedID.Mall_BoutiqueTab;
				break;
			case CMallSystem.Tab.Recommend:
				result = enRedID.Mall_RecommendTab;
				break;
			case CMallSystem.Tab.Hero:
				result = enRedID.Mall_HeroTab;
				break;
			case CMallSystem.Tab.Skin:
				result = enRedID.Mall_HeroSkinTab;
				break;
			case CMallSystem.Tab.Symbol_Make:
				result = enRedID.Mall_SymbolTab;
				break;
			case CMallSystem.Tab.Factory_Shop:
				result = enRedID.Mall_SaleTab;
				break;
			case CMallSystem.Tab.Roulette:
				result = enRedID.Mall_LotteryTab;
				break;
			}
			return result;
		}

		public void ToggleActionMask(bool active, float totalTime = 30f)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject widget = mallForm.GetWidget(1);
			GameObject widget2 = mallForm.GetWidget(5);
			CUITimerScript cUITimerScript = null;
			if (widget2 != null)
			{
				cUITimerScript = widget2.GetComponent<CUITimerScript>();
			}
			if (widget != null)
			{
				widget.CustomSetActive(active);
				if (cUITimerScript != null)
				{
					if (active)
					{
						cUITimerScript.SetTotalTime(totalTime);
						cUITimerScript.StartTimer();
					}
					else
					{
						cUITimerScript.EndTimer();
					}
				}
			}
		}

		public void ToggleSkipAnimationMask(bool active, float totalTime = 30f)
		{
			CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
			if (mallForm == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
			{
				return;
			}
			GameObject widget = mallForm.GetWidget(6);
			GameObject widget2 = mallForm.GetWidget(7);
			CUITimerScript cUITimerScript = null;
			if (widget2 != null)
			{
				cUITimerScript = widget2.GetComponent<CUITimerScript>();
			}
			if (widget != null)
			{
				widget.CustomSetActive(active);
				if (cUITimerScript != null)
				{
					if (active)
					{
						cUITimerScript.SetTotalTime(totalTime);
						cUITimerScript.StartTimer();
					}
					else
					{
						cUITimerScript.EndTimer();
					}
				}
			}
		}

		public void SetMallItem(CMallItemWidget itemWidget, CMallItem item)
		{
			if (item == null || this.m_MallForm == null || !this.m_IsMallFormOpen)
			{
				return;
			}
			CMallItem.ItemType itemType = item.Type();
			CMallItem.IconType iconType = item.GetIconType();
			bool flag = item.Owned(false);
			bool flag2 = item.CanSendFriend();
			this.SetItemImage(itemWidget, item);
			switch (itemType)
			{
			case CMallItem.ItemType.Hero:
			{
				CUIEventScript component = itemWidget.m_item.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.openHeroFormPar.heroId = item.HeroID();
				eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
				component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
				break;
			}
			case CMallItem.ItemType.Skin:
			{
				CUIEventScript component2 = itemWidget.m_item.GetComponent<CUIEventScript>();
				stUIEventParams eventParams2 = default(stUIEventParams);
				eventParams2.openHeroFormPar.heroId = item.HeroID();
				eventParams2.openHeroFormPar.skinId = item.SkinID();
				eventParams2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
				component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams2);
				break;
			}
			case CMallItem.ItemType.Item:
			{
				CUIEventScript component3 = itemWidget.m_item.GetComponent<CUIEventScript>();
				component3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, new stUIEventParams
				{
					tag = item.ProductIdx()
				});
				break;
			}
			}
			this.SetItemLabel(itemWidget, item, false);
			stPayInfoSet stPayInfoSet = item.PayInfoSet();
			if (stPayInfoSet.m_payInfoCount == 0 || flag)
			{
				this.SetItemName(itemWidget, item, CMallItemWidget.NamePosition.Bottom);
			}
			else
			{
				this.SetItemName(itemWidget, item, CMallItemWidget.NamePosition.Top);
			}
			Button button = null;
			Text text = null;
			Text text2 = null;
			CUIEventScript cUIEventScript = null;
			Button button2 = null;
			Text text3 = null;
			CUIEventScript cUIEventScript2 = null;
			itemWidget.m_priceContainer.CustomSetActive(false);
			if (itemWidget.m_buyBtn != null)
			{
				button = itemWidget.m_buyBtn.GetComponent<Button>();
				text = itemWidget.m_buyBtnText.GetComponent<Text>();
				cUIEventScript = itemWidget.m_buyBtn.GetComponent<CUIEventScript>();
			}
			if (itemWidget.m_buyBtnOwnedText != null)
			{
				text2 = itemWidget.m_buyBtnOwnedText.GetComponent<Text>();
			}
			if (itemWidget.m_linkBtn != null)
			{
				button2 = itemWidget.m_linkBtn.GetComponent<Button>();
				text3 = itemWidget.m_linkBtnText.GetComponent<Text>();
				cUIEventScript2 = itemWidget.m_linkBtn.GetComponent<CUIEventScript>();
			}
			itemWidget.m_linkBtn.CustomSetActive(false);
			if (flag && !flag2)
			{
				if (button != null && cUIEventScript != null)
				{
					button.enabled = false;
					cUIEventScript.enabled = false;
				}
				itemWidget.m_priceContainer.CustomSetActive(false);
				itemWidget.m_buyBtn.CustomSetActive(true);
				itemWidget.m_orTextContainer.CustomSetActive(false);
				if (text != null)
				{
					CMallItem.ItemType itemType2 = itemType;
					if (itemType2 != CMallItem.ItemType.Hero)
					{
						if (itemType2 == CMallItem.ItemType.Skin)
						{
							text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own"));
						}
					}
					else
					{
						text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
					}
				}
				itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
				this.SetItemTag(itemWidget, item, false);
				itemWidget.m_experienceMask.CustomSetActive(false);
				if (iconType == CMallItem.IconType.Small)
				{
					itemWidget.m_specialOwnedText.CustomSetActive(true);
				}
				else
				{
					itemWidget.m_specialOwnedText.CustomSetActive(false);
				}
			}
			else if (flag)
			{
				itemWidget.m_experienceMask.CustomSetActive(false);
				if (iconType == CMallItem.IconType.Small)
				{
					itemWidget.m_specialOwnedText.CustomSetActive(true);
				}
				else
				{
					itemWidget.m_specialOwnedText.CustomSetActive(false);
					itemWidget.m_buyBtnOwnedText.CustomSetActive(true);
					CMallItem.ItemType itemType2;
					if (text2 != null)
					{
						itemType2 = itemType;
						if (itemType2 != CMallItem.ItemType.Hero)
						{
							if (itemType2 == CMallItem.ItemType.Skin)
							{
								text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own"));
							}
						}
						else
						{
							text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
						}
					}
					itemWidget.m_buyBtn.CustomSetActive(true);
					if (text != null)
					{
						text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy_For_Friend"));
					}
					itemWidget.m_orTextContainer.CustomSetActive(false);
					itemType2 = itemType;
					if (itemType2 != CMallItem.ItemType.Hero)
					{
						if (itemType2 == CMallItem.ItemType.Skin)
						{
							if (button != null)
							{
								stUIEventParams eventParams3 = default(stUIEventParams);
								eventParams3.heroSkinParam.heroId = item.HeroID();
								eventParams3.heroSkinParam.skinId = item.SkinID();
								eventParams3.heroSkinParam.isCanCharge = true;
								eventParams3.commonUInt64Param1 = 0uL;
								eventParams3.commonBool = false;
								eventParams3.commonUInt32Param1 = 0u;
								button.enabled = true;
								cUIEventScript.enabled = true;
								cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, eventParams3);
							}
						}
					}
					else if (button != null)
					{
						stUIEventParams eventParams4 = default(stUIEventParams);
						eventParams4.heroId = item.HeroID();
						eventParams4.commonUInt64Param1 = 0uL;
						eventParams4.commonBool = false;
						eventParams4.commonUInt32Param1 = 0u;
						button.enabled = true;
						cUIEventScript.enabled = true;
						cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_OpenBuyHeroForFriend, eventParams4);
					}
				}
			}
			else
			{
				itemWidget.m_experienceMask.CustomSetActive(item.IsValidExperience());
				this.SetItemTag(itemWidget, item, false);
				if (stPayInfoSet.m_payInfoCount <= 0)
				{
					itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
					itemWidget.m_buyBtn.CustomSetActive(false);
					itemWidget.m_orTextContainer.CustomSetActive(false);
					itemWidget.m_linkBtn.CustomSetActive(true);
					string text4 = item.ObtWay();
					if (text3 != null)
					{
						if (string.IsNullOrEmpty(text4))
						{
							text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_SkinState_CannotBuy"));
						}
						else
						{
							text3.set_text(text4);
						}
					}
					byte b = item.ObtWayType();
					if (b > 0)
					{
						if (button2 != null && cUIEventScript2 != null && text3 != null)
						{
							button2.enabled = true;
							text3.set_text(item.ObtWay());
							cUIEventScript2.enabled = true;
							cUIEventScript2.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Jump_Form, new stUIEventParams
							{
								tag = (int)b
							});
						}
					}
					else if (button2 != null && cUIEventScript2 != null && text3 != null)
					{
						button2.enabled = false;
						cUIEventScript2.enabled = false;
					}
				}
				else
				{
					itemWidget.m_buyBtn.CustomSetActive(true);
					switch (itemType)
					{
					case CMallItem.ItemType.Hero:
						itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
						if (button != null && cUIEventScript != null && text != null)
						{
							button.enabled = true;
							text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
							cUIEventScript.enabled = true;
							cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenBuyHeroForm, new stUIEventParams
							{
								heroId = item.HeroID()
							});
						}
						this.SetItemPriceInfo(itemWidget, item, ref stPayInfoSet);
						break;
					case CMallItem.ItemType.Skin:
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						DebugHelper.Assert(masterRoleInfo != null, "SetMallItem::Master Role Info Is Null");
						if (masterRoleInfo == null)
						{
							return;
						}
						bool flag3 = masterRoleInfo.IsCanBuySkinButNotHaveHero(item.HeroID(), item.SkinID());
						if (flag3)
						{
							if (flag2)
							{
								if (text2 != null)
								{
									text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
								}
								itemWidget.m_buyBtnOwnedText.CustomSetActive(true);
								if (button != null && cUIEventScript != null && text != null)
								{
									cUIEventScript.enabled = true;
									text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy_For_Friend"));
									button.enabled = true;
									stUIEventParams eventParams5 = default(stUIEventParams);
									eventParams5.heroSkinParam.heroId = item.HeroID();
									eventParams5.heroSkinParam.skinId = item.SkinID();
									eventParams5.heroSkinParam.isCanCharge = true;
									cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, eventParams5);
								}
							}
							else
							{
								itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
								if (button != null && cUIEventScript != null && text != null)
								{
									cUIEventScript.enabled = true;
									text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
									button.enabled = true;
									stUIEventParams eventParams6 = default(stUIEventParams);
									eventParams6.openHeroFormPar.heroId = item.HeroID();
									eventParams6.openHeroFormPar.skinId = item.SkinID();
									eventParams6.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
									cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams6);
								}
							}
						}
						else
						{
							itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
							if (button != null && cUIEventScript != null && text != null)
							{
								cUIEventScript.enabled = true;
								text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
								button.enabled = true;
								stUIEventParams eventParams7 = default(stUIEventParams);
								eventParams7.heroSkinParam.heroId = item.HeroID();
								eventParams7.heroSkinParam.skinId = item.SkinID();
								eventParams7.heroSkinParam.isCanCharge = true;
								cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, eventParams7);
							}
						}
						this.SetItemPriceInfo(itemWidget, item, ref stPayInfoSet);
						break;
					}
					case CMallItem.ItemType.Item:
					{
						if (button != null && cUIEventScript != null && text != null)
						{
							button.enabled = true;
							text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
							cUIEventScript.enabled = true;
							cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, new stUIEventParams
							{
								tag = item.ProductIdx()
							});
						}
						CUIEventScript component4 = itemWidget.m_item.GetComponent<CUIEventScript>();
						component4.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, new stUIEventParams
						{
							tag = item.ProductIdx()
						});
						this.SetItemPriceInfo(itemWidget, item, ref stPayInfoSet);
						break;
					}
					}
				}
			}
		}

		public void SetItemImage(CMallItemWidget itemWidget, CMallItem item)
		{
			if (null == itemWidget)
			{
				return;
			}
			if (itemWidget.m_icon != null)
			{
				Image component = itemWidget.m_icon.GetComponent<Image>();
				component.set_color(CUIUtility.s_Color_White);
				component.SetSprite(item.Icon(), this.m_MallForm, false, true, true, true);
			}
		}

		public void SetItemLabel(CMallItemWidget itemWidget, CMallItem item, bool useSmallIcon = false)
		{
			CMallItem.IconType iconType = item.GetIconType();
			switch (item.Type())
			{
			case CMallItem.ItemType.Hero:
				if (iconType == CMallItem.IconType.Small)
				{
					string prefabPath = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
					CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), prefabPath, this.m_MallForm, true, false, false, false);
				}
				else
				{
					itemWidget.m_skinLabel.CustomSetActive(false);
				}
				break;
			case CMallItem.ItemType.Skin:
				if (iconType == CMallItem.IconType.Small)
				{
					string prefabPath2 = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
					CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), prefabPath2, this.m_MallForm, true, false, false, false);
				}
				else
				{
					CUICommonSystem.SetHeroSkinLabelPic(this.m_MallForm, itemWidget.m_skinLabel, item.HeroID(), item.SkinID());
				}
				break;
			case CMallItem.ItemType.Item:
				if (iconType == CMallItem.IconType.Small)
				{
					string prefabPath3 = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
					CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), prefabPath3, this.m_MallForm, true, false, false, false);
				}
				break;
			}
		}

		public void SetItemName(CMallItemWidget itemWidget, CMallItem item, CMallItemWidget.NamePosition position)
		{
			if (position != CMallItemWidget.NamePosition.Top)
			{
				if (position == CMallItemWidget.NamePosition.Bottom)
				{
					itemWidget.m_topNameContainer.CustomSetActive(false);
					itemWidget.m_bottomNameContainer.CustomSetActive(true);
					if (itemWidget.m_bottomNameLeftText != null)
					{
						Text component = itemWidget.m_bottomNameLeftText.GetComponent<Text>();
						component.set_text(item.FirstName());
					}
					if (itemWidget.m_bottomNameRightText != null)
					{
						Text component2 = itemWidget.m_bottomNameRightText.GetComponent<Text>();
						string text = item.SecondName();
						if (string.IsNullOrEmpty(text))
						{
							itemWidget.m_bottomNameRightText.CustomSetActive(false);
						}
						else
						{
							itemWidget.m_bottomNameRightText.CustomSetActive(true);
							component2.set_text(item.SecondName());
						}
					}
				}
			}
			else
			{
				itemWidget.m_bottomNameContainer.CustomSetActive(false);
				itemWidget.m_topNameContainer.CustomSetActive(true);
				if (itemWidget.m_topNameLeftText != null)
				{
					Text component3 = itemWidget.m_topNameLeftText.GetComponent<Text>();
					component3.set_text(item.FirstName());
				}
				if (itemWidget.m_topNameRightText != null)
				{
					Text component4 = itemWidget.m_topNameRightText.GetComponent<Text>();
					string text2 = item.SecondName();
					if (string.IsNullOrEmpty(text2))
					{
						itemWidget.m_topNameRightText.CustomSetActive(false);
					}
					else
					{
						itemWidget.m_topNameRightText.CustomSetActive(true);
						component4.set_text(item.SecondName());
					}
				}
			}
		}

		public void SetItemTag(CMallItemWidget itemWidget, CMallItem item, bool owned = false)
		{
			string prefabPath = null;
			string text = null;
			bool flag = item.TagInfo(ref prefabPath, ref text, owned);
			if (flag && itemWidget.m_tagContainer != null)
			{
				itemWidget.m_tagContainer.SetActive(true);
				Image component = itemWidget.m_tagContainer.GetComponent<Image>();
				component.SetSprite(prefabPath, this.m_MallForm, false, true, true, false);
				if (itemWidget.m_tagText != null)
				{
					Text component2 = itemWidget.m_tagText.GetComponent<Text>();
					component2.set_text(text);
				}
			}
			else
			{
				itemWidget.m_tagContainer.CustomSetActive(false);
			}
		}

		public void SetItemPriceInfo(CMallItemWidget itemWidget, CMallItem item, ref stPayInfoSet payInfoSet)
		{
			if (itemWidget.m_priceContainer == null)
			{
				return;
			}
			itemWidget.m_priceContainer.SetActive(true);
			CUIListScript component = itemWidget.m_priceContainer.GetComponent<CUIListScript>();
			component.SetElementAmount(payInfoSet.m_payInfoCount);
			CMallItem.OldPriceType oldPriceType = item.GetOldPriceType();
			if (payInfoSet.m_payInfoCount < 2)
			{
				itemWidget.m_orTextContainer.CustomSetActive(false);
			}
			else
			{
				itemWidget.m_orTextContainer.CustomSetActive(true);
			}
			for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				if (elemenet == null)
				{
					break;
				}
				GameObject widget = elemenet.GetWidget(0);
				GameObject widget2 = elemenet.GetWidget(1);
				GameObject widget3 = elemenet.GetWidget(2);
				GameObject widget4 = elemenet.GetWidget(4);
				GameObject widget5 = elemenet.GetWidget(3);
				GameObject widget6 = elemenet.GetWidget(5);
				if (widget == null || widget2 == null || widget3 == null || widget4 == null || widget5 == null || widget6 == null)
				{
					break;
				}
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
					component2.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
					Image component3 = widget6.GetComponent<Image>();
					component3.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
					break;
				}
				case CMallItem.OldPriceType.FirstOne:
					itemWidget.m_middleOrText.CustomSetActive(false);
					itemWidget.m_bottomOrText.CustomSetActive(true);
					if (i == 0)
					{
						widget2.SetActive(false);
						widget5.SetActive(false);
						widget.SetActive(true);
						widget3.SetActive(true);
						Text component4 = widget.GetComponent<Text>();
						component4.set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
						Text component5 = widget3.GetComponent<Text>();
						component5.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
						Image component6 = widget4.GetComponent<Image>();
						component6.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
					}
					else
					{
						widget2.SetActive(false);
						widget.SetActive(false);
						widget5.SetActive(false);
						widget3.SetActive(true);
						Text component7 = widget3.GetComponent<Text>();
						component7.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
						Image component8 = widget4.GetComponent<Image>();
						component8.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
					}
					break;
				case CMallItem.OldPriceType.SecondOne:
					itemWidget.m_middleOrText.CustomSetActive(false);
					itemWidget.m_bottomOrText.CustomSetActive(true);
					if (i == 1)
					{
						widget2.SetActive(false);
						widget5.SetActive(false);
						widget.SetActive(true);
						widget3.SetActive(true);
						Text component9 = widget.GetComponent<Text>();
						component9.set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
						Text component10 = widget3.GetComponent<Text>();
						component10.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
						Image component11 = widget4.GetComponent<Image>();
						component11.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
					}
					else
					{
						widget2.SetActive(false);
						widget.SetActive(false);
						widget5.SetActive(false);
						widget3.SetActive(true);
						Text component12 = widget3.GetComponent<Text>();
						component12.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
						Image component13 = widget4.GetComponent<Image>();
						component13.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
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
					component14.set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
					Text component15 = widget3.GetComponent<Text>();
					component15.set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
					Image component16 = widget4.GetComponent<Image>();
					component16.SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
					break;
				}
				}
			}
		}

		public static void SetSkinBuyPricePanel(CUIFormScript formScript, Transform pricePanel, ref stPayInfo payInfo)
		{
			if (pricePanel == null)
			{
				return;
			}
			Transform transform = pricePanel.Find("newPricePanel");
			Transform transform2 = pricePanel.Find("oldPricePanel");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(false);
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
			}
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(true);
				Transform costIcon = transform.Find("costImage");
				CHeroSkinBuyManager.SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
				Transform currentPrice = transform.Find("newCostText");
				CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, payInfo.m_payValue);
			}
			if (transform2 != null && payInfo.m_payValue != payInfo.m_oriValue)
			{
				transform2.gameObject.CustomSetActive(true);
				Transform currentPrice2 = transform2.Find("oldPriceText");
				CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice2, payInfo.m_oriValue);
			}
		}

		private static stPayInfoSet CalLowestPayInfoSet(params stPayInfoSet[] values)
		{
			Dictionary<enPayType, stPayInfo> dictionary = new Dictionary<enPayType, stPayInfo>();
			for (int i = 0; i < values.Length; i++)
			{
				stPayInfoSet stPayInfoSet = values[i];
				for (int j = 0; j < stPayInfoSet.m_payInfoCount; j++)
				{
					if (stPayInfoSet.m_payInfos[j].m_payValue != 0u)
					{
						stPayInfo stPayInfo = default(stPayInfo);
						if (dictionary.TryGetValue(stPayInfoSet.m_payInfos[j].m_payType, ref stPayInfo))
						{
							if (stPayInfoSet.m_payInfos[j].m_payValue < stPayInfo.m_payValue)
							{
								dictionary.set_Item(stPayInfo.m_payType, stPayInfoSet.m_payInfos[j]);
							}
						}
						else
						{
							dictionary.Add(stPayInfoSet.m_payInfos[j].m_payType, stPayInfoSet.m_payInfos[j]);
						}
					}
				}
			}
			stPayInfoSet result = new stPayInfoSet(dictionary.get_Count());
			result.m_payInfoCount = dictionary.get_Count();
			dictionary.get_Values().CopyTo(result.m_payInfos, 0);
			return result;
		}

		public static bool IsinRegisterSales(ResDT_RegisterSale_Info stRegisterSale)
		{
			if (stRegisterSale.bIsValid != 1)
			{
				return false;
			}
			string szStartTimeStr = stRegisterSale.szStartTimeStr;
			if (string.IsNullOrEmpty(szStartTimeStr))
			{
				return false;
			}
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)stRegisterSale.dwStartTimeGen));
			if ((Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime - dateTime).get_TotalMilliseconds() < 0.0)
			{
				return false;
			}
			DateTime dateTime2 = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds(stRegisterSale.dwValidTime);
			DateTime dateTime3 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			return dateTime3 <= dateTime2;
		}

		public static string GetRegisterSalesHeroDay(ref ResHeroPromotion heroPromotion, ResHeroShop heroShop)
		{
			string result = null;
			ResDT_RegisterSale_Info stRegisterSale = heroShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			if (!flag)
			{
				return result;
			}
			bool flag2 = true;
			if (heroPromotion != null)
			{
				uint dwBuyCoin = heroPromotion.dwBuyCoin;
				uint dwBuyCoupons = heroPromotion.dwBuyCoupons;
				uint dwBuyDiamond = heroPromotion.dwBuyDiamond;
				if (flag)
				{
					if (stRegisterSale.dwBuyCoin > dwBuyCoin)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyCoupons > dwBuyCoupons)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyDiamond > dwBuyDiamond)
					{
						flag2 = false;
					}
				}
			}
			else
			{
				uint num = (heroShop != null) ? heroShop.dwBuyCoin : 1u;
				uint num2 = (heroShop != null) ? heroShop.dwBuyCoupons : 1u;
				uint num3 = (heroShop != null) ? heroShop.dwBuyDiamond : 1u;
				if (flag && heroShop != null)
				{
					if (stRegisterSale.dwBuyCoin > num)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyCoupons > num2)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyDiamond > num3)
					{
						flag2 = false;
					}
				}
			}
			if (flag2)
			{
				DateTime dateTime = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds(stRegisterSale.dwValidTime);
				DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				int num4 = (int)Math.Ceiling((dateTime - dateTime2).get_TotalSeconds() / 86400.0);
				if (num4 > 0)
				{
					result = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", new string[]
					{
						num4.ToString()
					});
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static string GetRegisterSalesSkinDay(ref ResSkinPromotion heroPromotion, ResHeroSkinShop heroShop)
		{
			string result = null;
			ResDT_RegisterSale_Info stRegisterSale = heroShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			if (!flag)
			{
				return result;
			}
			bool flag2 = true;
			if (heroPromotion != null)
			{
				uint dwBuyCoupons = heroPromotion.dwBuyCoupons;
				uint dwBuyDiamond = heroPromotion.dwBuyDiamond;
				if (flag)
				{
					if (stRegisterSale.dwBuyCoupons > dwBuyCoupons)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyDiamond > dwBuyDiamond)
					{
						flag2 = false;
					}
				}
			}
			else
			{
				uint num = (heroShop != null) ? heroShop.dwBuyCoupons : 1u;
				uint num2 = (heroShop != null) ? heroShop.dwBuyDiamond : 1u;
				if (flag && heroShop != null)
				{
					if (stRegisterSale.dwBuyCoupons > num)
					{
						flag2 = false;
					}
					if (stRegisterSale.dwBuyDiamond > num2)
					{
						flag2 = false;
					}
				}
			}
			if (flag2)
			{
				DateTime dateTime = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds(stRegisterSale.dwValidTime);
				DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				int num3 = (int)Math.Ceiling((dateTime - dateTime2).get_TotalSeconds() / 86400.0);
				if (num3 > 0)
				{
					result = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", new string[]
					{
						num3.ToString()
					});
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static stPayInfoSet GetPayInfoSetOfGood(ResHeroCfgInfo resHeroCfgInfo, ResHeroPromotion resPromotion)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(resHeroCfgInfo.dwCfgID, out resHeroShop);
			ResDT_RegisterSale_Info stRegisterSale = resHeroShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			if (resPromotion != null)
			{
				uint dwBuyCoin = resPromotion.dwBuyCoin;
				uint dwBuyCoupons = resPromotion.dwBuyCoupons;
				uint dwBuyDiamond = resPromotion.dwBuyDiamond;
				if (flag)
				{
					if (stRegisterSale.dwBuyCoin < dwBuyCoin)
					{
						dwBuyCoin = stRegisterSale.dwBuyCoin;
					}
					if (stRegisterSale.dwBuyCoupons < dwBuyCoupons)
					{
						dwBuyCoupons = stRegisterSale.dwBuyCoupons;
					}
					if (stRegisterSale.dwBuyDiamond < dwBuyDiamond)
					{
						dwBuyDiamond = stRegisterSale.dwBuyDiamond;
					}
				}
				return CMallSystem.GetPayInfoSetOfGood(resPromotion.bIsBuyCoin > 0, dwBuyCoin, (resHeroShop != null) ? resHeroShop.dwBuyCoin : 1u, resPromotion.bIsBuyCoupons > 0, dwBuyCoupons, (resHeroShop != null) ? resHeroShop.dwBuyCoupons : 1u, resPromotion.bIsBuyDiamond > 0, dwBuyDiamond, (resHeroShop != null) ? resHeroShop.dwBuyDiamond : 1u, 10000u);
			}
			uint num = (resHeroShop != null) ? resHeroShop.dwBuyCoin : 1u;
			uint num2 = (resHeroShop != null) ? resHeroShop.dwBuyCoupons : 1u;
			uint num3 = (resHeroShop != null) ? resHeroShop.dwBuyDiamond : 1u;
			if (flag && resHeroShop != null)
			{
				if (stRegisterSale.dwBuyCoin < num)
				{
					num = stRegisterSale.dwBuyCoin;
				}
				if (stRegisterSale.dwBuyCoupons < num2)
				{
					num2 = stRegisterSale.dwBuyCoupons;
				}
				if (stRegisterSale.dwBuyDiamond < num3)
				{
					num3 = stRegisterSale.dwBuyDiamond;
				}
			}
			return CMallSystem.GetPayInfoSetOfGood(resHeroShop != null && resHeroShop.bIsBuyCoin > 0, num, (resHeroShop != null) ? resHeroShop.dwBuyCoin : 1u, resHeroShop != null && resHeroShop.bIsBuyCoupons > 0, num2, (resHeroShop != null) ? resHeroShop.dwBuyCoupons : 1u, resHeroShop != null && resHeroShop.bIsBuyDiamond > 0, num3, (resHeroShop != null) ? resHeroShop.dwBuyDiamond : 1u, 10000u);
		}

		public static stPayInfoSet GetPayInfoSetOfGood(ResHeroSkin resHeroSkin, ResSkinPromotion resPromotion)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(resHeroSkin.dwID, out resHeroSkinShop);
			ResDT_RegisterSale_Info stRegisterSale = resHeroSkinShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			if (resPromotion != null)
			{
				uint dwBuyCoupons = resPromotion.dwBuyCoupons;
				uint dwBuyDiamond = resPromotion.dwBuyDiamond;
				if (flag)
				{
					if (stRegisterSale.dwBuyCoupons < dwBuyCoupons)
					{
						dwBuyCoupons = stRegisterSale.dwBuyCoupons;
					}
					if (stRegisterSale.dwBuyDiamond < dwBuyDiamond)
					{
						dwBuyDiamond = stRegisterSale.dwBuyDiamond;
					}
				}
				return CMallSystem.GetPayInfoSetOfGood(false, 0u, 0u, resPromotion.bIsBuyCoupons > 0, dwBuyCoupons, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u, resPromotion.bIsBuyDiamond > 0, dwBuyDiamond, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u, 10000u);
			}
			uint num = (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u;
			uint num2 = (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u;
			if (flag && resHeroSkinShop != null)
			{
				if (stRegisterSale.dwBuyCoupons < num)
				{
					num = stRegisterSale.dwBuyCoupons;
				}
				if (stRegisterSale.dwBuyDiamond < num2)
				{
					num2 = stRegisterSale.dwBuyDiamond;
				}
			}
			return CMallSystem.GetPayInfoSetOfGood(false, 0u, 0u, resHeroSkinShop != null && resHeroSkinShop.bIsBuyCoupons > 0, num, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u, resHeroSkinShop != null && resHeroSkinShop.bIsBuyDiamond > 0, num2, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u, 10000u);
		}

		public static stPayInfoSet GetPayInfoSetOfGood(ResHeroCfgInfo resHeroCfgInfo)
		{
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(resHeroCfgInfo.dwCfgID, out resHeroShop);
			ResDT_RegisterSale_Info stRegisterSale = resHeroShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			uint num = (resHeroShop != null) ? resHeroShop.dwBuyCoin : 1u;
			uint num2 = (resHeroShop != null) ? resHeroShop.dwBuyCoupons : 1u;
			uint num3 = (resHeroShop != null) ? resHeroShop.dwBuyDiamond : 1u;
			if (flag && resHeroShop != null)
			{
				if (stRegisterSale.dwBuyCoin < num)
				{
					num = stRegisterSale.dwBuyCoin;
				}
				if (stRegisterSale.dwBuyCoupons < num2)
				{
					num2 = stRegisterSale.dwBuyCoupons;
				}
				if (stRegisterSale.dwBuyDiamond < num3)
				{
					num3 = stRegisterSale.dwBuyDiamond;
				}
			}
			return CMallSystem.GetPayInfoSetOfGood(resHeroShop != null && resHeroShop.bIsBuyCoin > 0, num, (resHeroShop != null) ? resHeroShop.dwBuyCoin : 1u, resHeroShop != null && resHeroShop.bIsBuyCoupons > 0, num2, (resHeroShop != null) ? resHeroShop.dwBuyCoupons : 1u, resHeroShop != null && resHeroShop.bIsBuyDiamond > 0, num3, (resHeroShop != null) ? resHeroShop.dwBuyDiamond : 1u, 10000u);
		}

		public static stPayInfoSet GetPayInfoSetOfGood(ResHeroSkin resHeroSkin)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(resHeroSkin.dwID, out resHeroSkinShop);
			ResDT_RegisterSale_Info stRegisterSale = resHeroSkinShop.stRegisterSale;
			bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
			uint num = (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u;
			uint num2 = (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u;
			if (flag && resHeroSkinShop != null)
			{
				if (stRegisterSale.dwBuyCoupons < num)
				{
					num = stRegisterSale.dwBuyCoupons;
				}
				if (stRegisterSale.dwBuyDiamond < num2)
				{
					num2 = stRegisterSale.dwBuyDiamond;
				}
			}
			return CMallSystem.GetPayInfoSetOfGood(false, 0u, 0u, resHeroSkinShop != null && resHeroSkinShop.bIsBuyCoupons > 0, num, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyCoupons : 1u, resHeroSkinShop != null && resHeroSkinShop.bIsBuyDiamond > 0, num2, (resHeroSkinShop != null) ? resHeroSkinShop.dwBuyDiamond : 1u, 10000u);
		}

		public static stPayInfoSet GetPayInfoSetOfGood(bool canUseGoldCoin, uint goldCoinValue, bool canUseDianQuan, uint dianquanValue, bool canUseDiamond, uint diamondValue, uint discount = 10000u)
		{
			stPayInfoSet result = new stPayInfoSet(2);
			if (canUseGoldCoin)
			{
				result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.GoldCoin;
				result.m_payInfos[result.m_payInfoCount].m_payValue = goldCoinValue;
				result.m_payInfos[result.m_payInfoCount].m_oriValue = goldCoinValue;
				result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = 10000u;
				result.m_payInfoCount++;
			}
			if (canUseDianQuan || canUseDiamond)
			{
				if (canUseDianQuan && !canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.DianQuan;
					result.m_payInfos[result.m_payInfoCount].m_payValue = dianquanValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = dianquanValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = 10000u;
				}
				else if (!canUseDianQuan && canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.Diamond;
					result.m_payInfos[result.m_payInfoCount].m_payValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = 10000u;
				}
				else if (canUseDianQuan && canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.DiamondAndDianQuan;
					result.m_payInfos[result.m_payInfoCount].m_payValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = 10000u;
				}
				result.m_payInfoCount++;
			}
			if (discount < 10000u)
			{
				for (int i = 0; i < result.m_payInfoCount; i++)
				{
					result.m_payInfos[i].m_payValue = result.m_payInfos[i].m_payValue * discount / 10000u;
					result.m_payInfos[i].m_discountForDisplay = discount;
				}
			}
			return result;
		}

		public static stPayInfoSet GetPayInfoSetOfGood(bool canUseGoldCoin, uint goldCoinValue, uint oriGoldCoinValue, bool canUseDianQuan, uint dianquanValue, uint oriDianquanValue, bool canUseDiamond, uint diamondValue, uint oriDiamondValue, uint discount = 10000u)
		{
			stPayInfoSet result = new stPayInfoSet(2);
			if (canUseGoldCoin)
			{
				result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.GoldCoin;
				result.m_payInfos[result.m_payInfoCount].m_payValue = goldCoinValue;
				result.m_payInfos[result.m_payInfoCount].m_oriValue = oriGoldCoinValue;
				result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = discount / 1000u;
				result.m_payInfoCount++;
			}
			if (canUseDianQuan || canUseDiamond)
			{
				if (canUseDianQuan && !canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.DianQuan;
					result.m_payInfos[result.m_payInfoCount].m_payValue = dianquanValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = oriDianquanValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = discount / 1000u;
				}
				else if (!canUseDianQuan && canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.Diamond;
					result.m_payInfos[result.m_payInfoCount].m_payValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = oriDiamondValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = discount / 1000u;
				}
				else if (canUseDianQuan && canUseDiamond)
				{
					result.m_payInfos[result.m_payInfoCount].m_payType = enPayType.DiamondAndDianQuan;
					result.m_payInfos[result.m_payInfoCount].m_payValue = diamondValue;
					result.m_payInfos[result.m_payInfoCount].m_oriValue = oriDiamondValue;
					result.m_payInfos[result.m_payInfoCount].m_discountForDisplay = discount / 1000u;
				}
				result.m_payInfoCount++;
			}
			return result;
		}

		public static string GetPayTypeIconPath(enPayType payType)
		{
			switch (payType)
			{
			case enPayType.GoldCoin:
				return "UGUI/Sprite/Common/GoldCoin.prefab";
			case enPayType.DianQuan:
				return "UGUI/Sprite/Common/DianQuan.prefab";
			case enPayType.Diamond:
			case enPayType.DiamondAndDianQuan:
				return "UGUI/Sprite/Common/Diamond.prefab";
			default:
				return null;
			}
		}

		public static string GetPayTypeText(enPayType payType)
		{
			switch (payType)
			{
			case enPayType.GoldCoin:
				return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
			case enPayType.DianQuan:
				return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");
			case enPayType.Diamond:
			case enPayType.DiamondAndDianQuan:
				return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Diamond");
			default:
				return null;
			}
		}

		public static string GetPriceTypeBuyString(enPayType payType)
		{
			switch (payType)
			{
			case enPayType.GoldCoin:
				return Singleton<CTextManager>.GetInstance().GetText("Pay_As_GoldCoin");
			case enPayType.DianQuan:
				return Singleton<CTextManager>.GetInstance().GetText("Pay_As_DianQuan");
			case enPayType.Diamond:
			case enPayType.DiamondAndDianQuan:
				return Singleton<CTextManager>.GetInstance().GetText("Pay_As_Diamond");
			default:
				return string.Empty;
			}
		}

		public static void SetPayButton(CUIFormScript formScript, RectTransform buttonTransform, enPayType payType, uint payValue, enUIEventID eventID, ref stUIEventParams eventParams)
		{
			if (formScript == null || buttonTransform == null)
			{
				return;
			}
			Transform transform = buttonTransform.FindChild("Image");
			if (transform != null)
			{
				Image component = transform.gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.SetSprite(CMallSystem.GetPayTypeIconPath(payType), formScript, true, false, false, false);
				}
			}
			Transform transform2 = buttonTransform.FindChild("Text");
			if (transform2 != null)
			{
				Text component2 = transform2.gameObject.GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text((payValue > 0u) ? payValue.ToString() : "免费");
				}
			}
			CUIEventScript component3 = buttonTransform.gameObject.GetComponent<CUIEventScript>();
			if (component3 != null)
			{
				component3.SetUIEvent(enUIEventType.Click, eventID, eventParams);
			}
		}

		public static void SetPayButton(CUIFormScript formScript, RectTransform buttonTransform, enPayType payType, uint payValue, uint oriValue, enUIEventID eventID, ref stUIEventParams eventParams)
		{
			if (formScript == null || buttonTransform == null)
			{
				return;
			}
			Transform transform = buttonTransform.FindChild("Image");
			if (transform != null)
			{
				Image component = transform.gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.SetSprite(CMallSystem.GetPayTypeIconPath(payType), formScript, true, false, false, false);
				}
			}
			Transform transform2 = buttonTransform.FindChild("PriceContainer/Price");
			if (transform2 != null)
			{
				Text component2 = transform2.gameObject.GetComponent<Text>();
				if (component2 != null)
				{
					component2.set_text((payValue > 0u) ? payValue.ToString() : "免费");
				}
			}
			Transform transform3 = buttonTransform.FindChild("PriceContainer/OriPrice");
			if (payValue >= oriValue)
			{
				if (transform3 != null)
				{
					transform3.gameObject.CustomSetActive(false);
				}
			}
			else if (transform3 != null)
			{
				transform3.gameObject.CustomSetActive(true);
				Text component3 = transform3.gameObject.GetComponent<Text>();
				if (component3 != null)
				{
					component3.set_text((oriValue > 0u) ? oriValue.ToString() : "免费");
				}
			}
			CUIEventScript component4 = buttonTransform.gameObject.GetComponent<CUIEventScript>();
			if (component4 != null)
			{
				component4.SetUIEvent(enUIEventType.Click, eventID, eventParams);
			}
		}

		public static void TryToPay(enPayPurpose payPurpose, string goodName, enPayType payType, uint payValue, enUIEventID confirmEventID, ref stUIEventParams confirmEventParams, enUIEventID cancelEventID, bool needConfirm, bool guideToAchieveDianQuan, bool noQuestionMark = false)
		{
			if (!ApolloConfig.payEnabled)
			{
				guideToAchieveDianQuan = false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			uint currencyValueFromRoleInfo = CMallSystem.GetCurrencyValueFromRoleInfo(masterRoleInfo, payType);
			if (currencyValueFromRoleInfo >= payValue)
			{
				string text = string.Empty;
				if (payType == enPayType.DiamondAndDianQuan)
				{
					uint diamond = masterRoleInfo.Diamond;
					uint num = (uint)masterRoleInfo.DianQuan;
					if (diamond < payValue)
					{
						text = string.Format(Singleton<CTextManager>.GetInstance().GetText("MixPayNotice"), new object[]
						{
							(payValue - diamond).ToString(),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[3]),
							(payValue - diamond).ToString(),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[2])
						});
						needConfirm = true;
					}
				}
				if (needConfirm)
				{
					string strContent = string.Empty;
					if (noQuestionMark)
					{
						strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPayNoQuestionMark", new string[]
						{
							payValue.ToString(),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[(int)payType]),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payPurposeNameKeys[(int)payPurpose]),
							goodName,
							text
						}), new object[0]);
					}
					else
					{
						strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPay", new string[]
						{
							payValue.ToString(),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[(int)payType]),
							Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payPurposeNameKeys[(int)payPurpose]),
							goodName,
							text
						}), new object[0]);
					}
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, confirmEventID, cancelEventID, confirmEventParams, false);
				}
				else
				{
					CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
					uIEvent.m_eventID = confirmEventID;
					uIEvent.m_eventParams = confirmEventParams;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
				}
			}
			else if ((payType == enPayType.DianQuan || payType == enPayType.DiamondAndDianQuan) && guideToAchieveDianQuan)
			{
				string strContent2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnoughWithJumpToAchieve"), Singleton<CTextManager>.GetInstance().GetText((payType == enPayType.DiamondAndDianQuan) ? "Currency_DiamondAndDianQuan" : CMallSystem.s_payTypeNameKeys[2]));
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent2, enUIEventID.Pay_OpenBuyDianQuanPanel, cancelEventID, false);
			}
			else
			{
				string strContent3 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnough"), Singleton<CTextManager>.GetInstance().GetText((payType == enPayType.DiamondAndDianQuan) ? "Currency_DiamondAndDianQuan" : CMallSystem.s_payTypeNameKeys[(int)payType]));
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent3, cancelEventID, false);
			}
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id = goodName;
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_quantity = payValue.ToString();
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_way = payType.ToString();
		}

		public static enPayType ResBuyTypeToPayType(int resShopBuyType)
		{
			switch (resShopBuyType)
			{
			case 2:
				return enPayType.DianQuan;
			case 4:
				return enPayType.GoldCoin;
			case 5:
				return enPayType.BurningCoin;
			case 6:
				return enPayType.ArenaCoin;
			case 8:
				return enPayType.GuildCoin;
			case 9:
				return enPayType.SymbolCoin;
			case 10:
				return enPayType.Diamond;
			case 11:
				return enPayType.DiamondAndDianQuan;
			}
			return enPayType.NotSupport;
		}

		public static RES_SHOPBUY_COINTYPE PayTypeToResBuyCoinType(enPayType payType)
		{
			switch (payType)
			{
			case enPayType.GoldCoin:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
			case enPayType.DianQuan:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
			case enPayType.Diamond:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;
			case enPayType.DiamondAndDianQuan:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY;
			case enPayType.BurningCoin:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN;
			case enPayType.ArenaCoin:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN;
			case enPayType.GuildCoin:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN;
			case enPayType.SymbolCoin:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN;
			default:
				return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
			}
		}

		public static uint GetCurrencyValueFromRoleInfo(CRoleInfo roleInfo, enPayType payType)
		{
			switch (payType)
			{
			case enPayType.GoldCoin:
				return roleInfo.GoldCoin;
			case enPayType.DianQuan:
				return (uint)roleInfo.DianQuan;
			case enPayType.Diamond:
				return roleInfo.Diamond;
			case enPayType.DiamondAndDianQuan:
				return roleInfo.Diamond + (uint)roleInfo.DianQuan;
			case enPayType.BurningCoin:
				return roleInfo.BurningCoin;
			case enPayType.ArenaCoin:
				return roleInfo.ArenaCoin;
			case enPayType.GuildCoin:
				return CGuildHelper.GetPlayerGuildConstruct();
			case enPayType.SymbolCoin:
				return roleInfo.SymbolCoin;
			default:
				return 0u;
			}
		}

		public static ResShopPromotion GetShopPromotion(RES_SHOPBUY_TYPE type, RES_SHOPDRAW_SUBTYPE subType)
		{
			uint shopInfoCfgId = CPurchaseSys.GetShopInfoCfgId(type, (int)subType);
			if (GameDataMgr.shopPromotionDict == null)
			{
				return null;
			}
			if (GameDataMgr.shopPromotionDict.ContainsKey(shopInfoCfgId))
			{
				ListView<ResShopPromotion> listView = new ListView<ResShopPromotion>();
				if (GameDataMgr.shopPromotionDict.TryGetValue(shopInfoCfgId, out listView))
				{
					int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
					if (listView != null)
					{
						for (int i = 0; i < listView.Count; i++)
						{
							ResShopPromotion resShopPromotion = listView[i];
							if ((ulong)resShopPromotion.dwOnTimeGen <= (ulong)((long)currentUTCTime) && (long)currentUTCTime < (long)((ulong)resShopPromotion.dwOffTimeGen))
							{
								return resShopPromotion;
							}
						}
					}
					return null;
				}
			}
			return null;
		}

		[MessageHandler(1149)]
		public static void ReceivePropGift(CSPkg msg)
		{
			ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stGiftUseGet.stRewardInfo);
			int count = useableListFromReward.Count;
			if (count == 0)
			{
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), Singleton<CTextManager>.GetInstance().GetText("Bag_Text_4"), false, enUIEventID.None, false, false, "Form_Award");
			uint heroId = 0u;
			uint skinId = 0u;
			int i = 0;
			while (i < count)
			{
				switch (useableListFromReward[i].m_type)
				{
				case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
					CUICommonSystem.ShowNewHeroOrSkin(useableListFromReward[i].m_baseID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority4, 0u, 0);
					break;
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
				case COM_ITEM_TYPE.COM_OBJTYPE_ITEMGEAR:
					goto IL_E5;
				case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
					CSkinInfo.ResolveHeroSkin(useableListFromReward[i].m_baseID, out heroId, out skinId);
					CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority4, 0u, 0);
					break;
				default:
					goto IL_E5;
				}
				IL_DA:
				i++;
				continue;
				IL_E5:
				if (useableListFromReward[i].MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM && (useableListFromReward[i].ExtraFromType == 1 || useableListFromReward[i].ExtraFromType == 2))
				{
					if (useableListFromReward[i].ExtraFromType == 1)
					{
						heroId = (uint)useableListFromReward[i].ExtraFromData;
						CUICommonSystem.ShowNewHeroOrSkin(heroId, 0u, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority4, (uint)useableListFromReward[i].m_stackCount, 0);
					}
					else if (useableListFromReward[i].ExtraFromType == 2)
					{
						skinId = (uint)useableListFromReward[i].ExtraFromData;
						CUICommonSystem.ShowNewHeroOrSkin(0u, skinId, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority4, (uint)useableListFromReward[i].m_stackCount, 0);
					}
				}
				goto IL_DA;
			}
		}

		[MessageHandler(4800)]
		public static void ReceiveRouletteDataNTF(CSPkg msg)
		{
			CSDT_LUCKYDRAW_INFO cSDT_LUCKYDRAW_INFO = new CSDT_LUCKYDRAW_INFO();
			if (!CMallSystem.luckyDrawDic.TryGetValue(enPayType.Diamond, ref cSDT_LUCKYDRAW_INFO))
			{
				cSDT_LUCKYDRAW_INFO = msg.stPkgData.stLuckyDrawDataNtf.stDiamond;
				CMallSystem.luckyDrawDic.Add(enPayType.Diamond, cSDT_LUCKYDRAW_INFO);
			}
			else
			{
				cSDT_LUCKYDRAW_INFO.dwCnt = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwCnt;
				cSDT_LUCKYDRAW_INFO.dwDrawMask = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwDrawMask;
				cSDT_LUCKYDRAW_INFO.dwReachMask = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwReachMask;
				cSDT_LUCKYDRAW_INFO.dwLuckyPoint = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwLuckyPoint;
			}
			CSDT_LUCKYDRAW_INFO cSDT_LUCKYDRAW_INFO2 = new CSDT_LUCKYDRAW_INFO();
			if (!CMallSystem.luckyDrawDic.TryGetValue(enPayType.DianQuan, ref cSDT_LUCKYDRAW_INFO2))
			{
				cSDT_LUCKYDRAW_INFO2 = msg.stPkgData.stLuckyDrawDataNtf.stCoupons;
				CMallSystem.luckyDrawDic.Add(enPayType.DianQuan, cSDT_LUCKYDRAW_INFO2);
			}
			else
			{
				cSDT_LUCKYDRAW_INFO2.dwCnt = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwCnt;
				cSDT_LUCKYDRAW_INFO2.dwDrawMask = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwDrawMask;
				cSDT_LUCKYDRAW_INFO2.dwReachMask = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwReachMask;
				cSDT_LUCKYDRAW_INFO2.dwLuckyPoint = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwLuckyPoint;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Receive_Roulette_Data);
		}
	}
}

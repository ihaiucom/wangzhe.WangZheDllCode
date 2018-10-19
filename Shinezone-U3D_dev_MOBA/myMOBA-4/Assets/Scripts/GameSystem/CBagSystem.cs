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
	public class CBagSystem : Singleton<CBagSystem>
	{
		public static string s_itemGetSourceFormPath = "UGUI/Form/Common/Form_ItemGetSource.prefab";

		public static string s_bagFormPath = "UGUI/Form/System/Bag/Form_Bag.prefab";

		public static string s_bagSaleFormPath = "UGUI/Form/System/Bag/Form_Bag_Sale_Item.prefab";

		public static string s_bagUseFormPath = "UGUI/Form/System/Bag/Form_Bag_Use_Item.prefab";

		public static string s_bagAutoSaleFormPath = "UGUI/Form/System/Bag/Form_Bag_Sale_Item_OutTime.prefab";

		public static string s_openAwardpFormPath = "UGUI/Form/Common/Form_OpenAward.prefab";

		public CUseableContainer m_ContainerAll = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public CUseableContainer m_ContainerRecentGet = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public CUseableContainer m_ContainerItem = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public CUseableContainer m_ContainerGift = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public CUseableContainer m_ContainerExpCard = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public CUseableContainer m_ContainerSymbol = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);

		public enItemMenuType m_selectUseableType;

		public CUseableContainer m_selectUseableContainer;

		public CUseable m_selectUseable;

		public int m_saleCount = 1;

		public int m_useCount = 1;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OnCloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnBag_MenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_ItemElement_Enable, new CUIEventManager.OnUIEventHandler(this.OnBag_ItemElement_Enable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnBag_SelectItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleItem, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItem, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItemWithAnimation, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItemWithAnimation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItemWithAnimationConfirm, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItemWithAnimationConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnUseItemAnimationPlayOver, new CUIEventManager.OnUIEventHandler(this.OnUseItemAnimationPlayOver));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenSaleForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenSaleForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_CloseSaleForm, new CUIEventManager.OnUIEventHandler(this.OnBag_CloseSaleForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountDown, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountUp, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountMax, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountMax));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_ConfirSale, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_ConfirmSale));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmSale, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmSale));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenUseForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenUseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountDown, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountUp, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountMax, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountMax));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemSourceInfoOpen, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemSourceInfoOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemSourceElementClick, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemSourceElementClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmUse, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmUseItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmUseWithAnimation, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmUseWithAnimation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnAutoSaleBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickAutoSaleBtn));
		}

		private void OnBag_OnCloseForm(CUIEvent uiEvent)
		{
		}

		private void OnBag_OpenForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			this.OpenBagForm();
			this.CheckOpenAutoSaleForm();
		}

		public void OpenBagForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBagSystem.s_bagFormPath, false, true);
			Singleton<CBagSystem>.GetInstance().m_selectUseableType = enItemMenuType.All;
			Singleton<CBagSystem>.GetInstance().InitMenu(cUIFormScript.gameObject);
			Singleton<CBagSystem>.GetInstance().RefreshBagForm();
		}

		private void InitMenu(GameObject root)
		{
			string[] array = new string[]
			{
				"全部",
				"最近获得",
				"道具",
				"礼包",
				"体验卡",
				"铭文"
			};
			GameObject gameObject = root.transform.Find("TopCommon/Panel_Menu/List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < component.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
		}

		public void RefreshBagForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBagSystem.s_bagFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find("TopCommon/Panel_Menu/List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.m_alwaysDispatchSelectedChangeEvent = true;
			component.SelectElement((int)this.m_selectUseableType, true);
			component.m_alwaysDispatchSelectedChangeEvent = false;
		}

		private void OnBag_MenuSelect(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			GameObject gameObject = srcFormScript.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel_Left").gameObject;
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			int lastSelectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetLastSelectedIndex();
			this.m_selectUseableType = (enItemMenuType)selectedIndex;
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer == null)
			{
				return;
			}
			CUseableContainer containerBySelectType = this.GetContainerBySelectType(useableContainer);
			if (containerBySelectType == null)
			{
				return;
			}
			this.m_selectUseableContainer = containerBySelectType;
			containerBySelectType.SortItemBag();
			GameObject gameObject3 = gameObject.transform.Find("Panel_Right/List").gameObject;
			CUIListScript component = gameObject3.GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_selectUseableContainer.GetCurUseableCount());
			if (component.m_elementAmount == 0)
			{
				gameObject2.CustomSetActive(false);
			}
			else
			{
				int selectedIndex2 = component.GetSelectedIndex();
				if (selectedIndex2 < 0 || selectedIndex2 >= component.m_elementAmount || selectedIndex != lastSelectedIndex)
				{
					component.SelectElement(0, true);
					this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.gameObject, 0);
					component.MoveElementInScrollArea(0, true);
				}
				else
				{
					component.SelectElement(selectedIndex2, true);
					this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.gameObject, selectedIndex2);
					component.MoveElementInScrollArea(selectedIndex2, true);
				}
			}
		}

		private CUseableContainer GetContainerBySelectType(CUseableContainer allContainer)
		{
			CUseableContainer result = allContainer;
			int curUseableCount = allContainer.GetCurUseableCount();
			if (this.m_selectUseableType == enItemMenuType.All)
			{
				this.m_ContainerAll.Clear();
				for (int i = 0; i < curUseableCount; i++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(i);
					if (useableByIndex.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL || useableByIndex.GetSalableCount() > 0)
					{
						this.m_ContainerAll.Add(useableByIndex);
					}
				}
				result = this.m_ContainerAll;
			}
			else if (this.m_selectUseableType == enItemMenuType.RecentGet)
			{
				this.m_ContainerRecentGet.Clear();
				DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				for (int j = 0; j < curUseableCount; j++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(j);
					if ((dateTime.Date - Utility.ToUtcTime2Local((long)useableByIndex.m_getTime).Date).Days <= 3)
					{
						if (useableByIndex.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL || useableByIndex.GetSalableCount() > 0)
						{
							this.m_ContainerRecentGet.Add(useableByIndex);
						}
					}
				}
				result = this.m_ContainerRecentGet;
			}
			else if (this.m_selectUseableType == enItemMenuType.Item)
			{
				this.m_ContainerItem.Clear();
				for (int k = 0; k < curUseableCount; k++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(k);
					if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem = (CItem)useableByIndex;
						if (cItem.m_itemData.bClass == 1 && cItem.m_itemData.bType != 4 && cItem.m_itemData.bType != 10)
						{
							this.m_ContainerItem.Add(cItem);
						}
					}
				}
				result = this.m_ContainerItem;
			}
			else if (this.m_selectUseableType == enItemMenuType.Gift)
			{
				this.m_ContainerGift.Clear();
				for (int l = 0; l < curUseableCount; l++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(l);
					if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem2 = (CItem)useableByIndex;
						if (cItem2.m_itemData.bClass == 1 && cItem2.m_itemData.bType == 4)
						{
							this.m_ContainerGift.Add(cItem2);
						}
					}
				}
				result = this.m_ContainerGift;
			}
			else if (this.m_selectUseableType == enItemMenuType.ExpCard)
			{
				this.m_ContainerExpCard.Clear();
				for (int m = 0; m < curUseableCount; m++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(m);
					if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
					{
						CItem cItem3 = (CItem)useableByIndex;
						if (cItem3.m_itemData.bClass == 1 && cItem3.m_itemData.bType == 10)
						{
							this.m_ContainerExpCard.Add(cItem3);
						}
					}
				}
				result = this.m_ContainerExpCard;
			}
			else if (this.m_selectUseableType == enItemMenuType.Symbol)
			{
				this.m_ContainerSymbol.Clear();
				for (int n = 0; n < curUseableCount; n++)
				{
					CUseable useableByIndex = allContainer.GetUseableByIndex(n);
					if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL && useableByIndex.GetSalableCount() > 0)
					{
						this.m_ContainerSymbol.Add(useableByIndex);
					}
				}
				result = this.m_ContainerSymbol;
			}
			return result;
		}

		public GameObject GetGuideItem()
		{
			GameObject result = null;
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBagSystem.s_bagFormPath);
			if (form == null)
			{
				return null;
			}
			CUseableContainer selectUseableContainer = this.m_selectUseableContainer;
			for (int i = 0; i < selectUseableContainer.GetCurUseableCount(); i++)
			{
				CUseable useableByIndex = selectUseableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = useableByIndex as CItem;
					if (cItem.m_itemData.bClass == 1 && cItem.m_itemData.bType == 1)
					{
						GameObject gameObject = form.gameObject.transform.Find("Panel_Right/List").gameObject;
						CUIListScript component = gameObject.GetComponent<CUIListScript>();
						if (component.GetElementAmount() > i)
						{
							result = component.GetElemenet(i).gameObject.transform.Find("itemCell").gameObject;
							break;
						}
					}
				}
			}
			return result;
		}

		public int FindItemIndex(uint inItemId)
		{
			int result = -1;
			CUseable useableByBaseID = this.m_ContainerAll.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, inItemId);
			if (useableByBaseID != null)
			{
				result = this.m_ContainerAll.GetUsebableIndexByUid(useableByBaseID.m_objID);
			}
			return result;
		}

		private void OnBag_ItemElement_Enable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			GameObject gameObject = srcWidget.transform.Find("itemCell").gameObject;
			CUseable useableByIndex = this.m_selectUseableContainer.GetUseableByIndex(srcWidgetIndexInBelongedList);
			CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, gameObject, useableByIndex, false, false, false, false);
		}

		private void OnBag_SelectItem(CUIEvent uiEvent)
		{
			this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.gameObject, uiEvent.m_srcWidgetIndexInBelongedList);
		}

		private void RefreshSelectItem(CUIFormScript formScript, GameObject root, int selectIndex)
		{
			this.m_selectUseable = this.m_selectUseableContainer.GetUseableByIndex(selectIndex);
			if (this.m_selectUseable == null)
			{
				return;
			}
			GameObject gameObject = root.transform.Find("Panel_Left").gameObject;
			GameObject gameObject2 = root.transform.Find("Panel_Left/itemCell").gameObject;
			Text component = root.transform.Find("Panel_Left/lblName").GetComponent<Text>();
			CanvasGroup component2 = root.transform.Find("Panel_Left/pnlCountTitle").GetComponent<CanvasGroup>();
			Text component3 = component2.transform.Find("lblCount").GetComponent<Text>();
			Text component4 = root.transform.Find("Panel_Left/lblDesc").GetComponent<Text>();
			Text component5 = root.transform.Find("Panel_Left/lblPrice").GetComponent<Text>();
			Text component6 = root.transform.Find("Panel_Left/lblPriceTitle").GetComponent<Text>();
			Image component7 = root.transform.Find("Panel_Left/imgGold").GetComponent<Image>();
			Transform targetTrans = root.transform.Find("Panel_Left/pnlNotSaleTip");
			Button component8 = root.transform.Find("Panel_Left/BtnGroup/Button_Sale").GetComponent<Button>();
			Button component9 = root.transform.Find("Panel_Left/BtnGroup/Button_Use").GetComponent<Button>();
			Button component10 = root.transform.Find("Panel_Left/BtnGroup/Button_Source").GetComponent<Button>();
			CUICommonSystem.SetItemCell(formScript, gameObject2, this.m_selectUseable, false, false, false, false);
			component.text = this.m_selectUseable.m_name;
			component3.text = this.m_selectUseable.GetSalableCount().ToString();
			component4.text = this.m_selectUseable.m_description;
			component5.text = this.m_selectUseable.m_coinSale.ToString();
			component5.gameObject.CustomSetActive(true);
			component6.gameObject.CustomSetActive(true);
			component7.gameObject.CustomSetActive(true);
			CUICommonSystem.SetObjActive(targetTrans, false);
			component8.gameObject.CustomSetActive(true);
			component9.gameObject.CustomSetActive(true);
			component10.gameObject.CustomSetActive(false);
			if (this.m_selectUseable.m_isSale <= 0)
			{
				component5.gameObject.CustomSetActive(false);
				component6.gameObject.CustomSetActive(false);
				component7.gameObject.CustomSetActive(false);
				component8.gameObject.CustomSetActive(false);
				CUICommonSystem.SetObjActive(targetTrans, true);
			}
			if (this.m_selectUseable.m_bCanUse <= 0)
			{
				component9.gameObject.CustomSetActive(false);
			}
			if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL && ((CSymbolItem)this.m_selectUseable).IsGuildSymbol())
			{
				component2.alpha = 0f;
			}
			else
			{
				component2.alpha = 1f;
			}
			gameObject.CustomSetActive(true);
		}

		private void OnBag_SaleItem(CUIEvent uiEvent)
		{
		}

		private void OnBag_UseItem(CUIEvent uiEvent)
		{
			if (this.m_selectUseable == null)
			{
				return;
			}
			CItem cItem = this.m_selectUseable as CItem;
			if (cItem == null)
			{
				return;
			}
			if (cItem.m_itemData.bType == 11)
			{
				CSecurePwdSystem.TryToValidate(enOpPurpose.USE_BATTLERECORD_CARD, enUIEventID.Bag_OnSecurePwdConfirmUse, default(stUIEventParams));
			}
			else
			{
				CBagSystem.SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0u, this.m_useCount, string.Empty);
			}
		}

		private void OnSecurePwdConfirmUseItem(CUIEvent uiEvent)
		{
			if (this.m_selectUseable == null)
			{
				return;
			}
			CBagSystem.SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0u, this.m_useCount, uiEvent.m_eventParams.pwd);
		}

		private void OnBag_UseItemWithAnimation(CUIEvent uiEvent)
		{
			CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
			if (iconUseable == null)
			{
				return;
			}
			COM_ITEM_TYPE type = iconUseable.m_type;
			if (type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = iconUseable as CItem;
				if (cItem != null && cItem.m_itemData != null && cItem.m_itemData.bType == 11)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_6"), enUIEventID.Bag_UseItemWithAnimationConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
					return;
				}
			}
			this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, string.Empty);
		}

		private void OnBag_UseItemWithAnimationConfirm(CUIEvent uiEvent)
		{
			CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
			if (iconUseable == null)
			{
				return;
			}
			COM_ITEM_TYPE type = iconUseable.m_type;
			if (type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = iconUseable as CItem;
				if (cItem != null && cItem.m_itemData != null && cItem.m_itemData.bType == 11)
				{
					CSecurePwdSystem.TryToValidate(enOpPurpose.USE_BATTLERECORD_CARD, enUIEventID.Bag_OnSecurePwdConfirmUseWithAnimation, uiEvent.m_eventParams);
					return;
				}
			}
			this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, string.Empty);
		}

		private void OnSecurePwdConfirmUseWithAnimation(CUIEvent uiEvent)
		{
			this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, uiEvent.m_eventParams.pwd);
		}

		private void OnUseItemAnimationPlayOver(CUIEvent uiEvent)
		{
			if (uiEvent.m_eventParams.iconUseable == null)
			{
				return;
			}
			CBagSystem.SendItemUseMsgToServer(uiEvent.m_eventParams.iconUseable.m_objID, 0u, uiEvent.m_eventParams.tag, uiEvent.m_eventParams.pwd);
		}

		private void PlayItemUseAnimation(CUseable item, int itemCount, string pwd = "")
		{
			if (item == null)
			{
				return;
			}
			CItem cItem = (CItem)item;
			if (cItem != null)
			{
				if (cItem.m_itemData.bType == 4 && cItem.m_itemData.EftParam[2] != 0f)
				{
					CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBagSystem.s_openAwardpFormPath, false, true);
					if (cUIFormScript != null)
					{
						CUITimerScript component = cUIFormScript.transform.Find("Timer").GetComponent<CUITimerScript>();
						component.m_eventParams[1].iconUseable = item;
						component.m_eventParams[1].tag = itemCount;
						component.m_eventParams[1].pwd = pwd;
						component.EndTimer();
						component.StartTimer();
					}
				}
				else if (!this.CheckNameChangeCard(cItem))
				{
					CBagSystem.SendItemUseMsgToServer(item.m_objID, 0u, itemCount, pwd);
				}
			}
		}

		private void OnBag_OpenSaleForm(CUIEvent uiEvent)
		{
			this.m_saleCount = 1;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBagSystem.s_bagSaleFormPath, false, true);
			GameObject gameObject = cUIFormScript.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel_Left/itemCell").gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblName").GetComponent<Text>();
			Text component2 = gameObject.transform.Find("Panel_Left/lblCount").GetComponent<Text>();
			Text component3 = gameObject.transform.Find("Panel_Left/lblPrice1").GetComponent<Text>();
			Text component4 = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			Text component5 = gameObject.transform.Find("Panel_Left/lblPrice").GetComponent<Text>();
			CUICommonSystem.SetItemCell(cUIFormScript, gameObject2, this.m_selectUseable, true, false, false, false);
			DebugHelper.Assert(component != null && component2 != null && component3 != null && component4 != null && component5 != null);
			component.text = this.m_selectUseable.m_name;
			component2.text = this.m_selectUseable.GetSalableCount().ToString();
			component3.text = this.m_selectUseable.m_coinSale.ToString();
			component4.text = this.m_saleCount + "/" + this.m_selectUseable.GetSalableCount();
			component5.text = ((long)this.m_saleCount * (long)((ulong)this.m_selectUseable.m_coinSale)).ToString();
		}

		private void OnBag_CloseSaleForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CBagSystem.s_bagSaleFormPath);
		}

		private void OnBag_SaleForm_CountDown(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			Text component2 = gameObject.transform.Find("Panel_Left/lblPrice").GetComponent<Text>();
			if (this.m_saleCount > 1)
			{
				this.m_saleCount--;
			}
			component.text = this.m_saleCount + "/" + this.m_selectUseable.GetSalableCount();
			component2.text = ((long)this.m_saleCount * (long)((ulong)this.m_selectUseable.m_coinSale)).ToString();
		}

		private void OnBag_SaleForm_CountUp(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			Text component2 = gameObject.transform.Find("Panel_Left/lblPrice").GetComponent<Text>();
			int salableCount = this.m_selectUseable.GetSalableCount();
			if (this.m_saleCount < salableCount)
			{
				this.m_saleCount++;
			}
			component.text = this.m_saleCount + "/" + salableCount;
			component2.text = ((long)this.m_saleCount * (long)((ulong)this.m_selectUseable.m_coinSale)).ToString();
		}

		private void OnBag_SaleForm_CountMax(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			Text component2 = gameObject.transform.Find("Panel_Left/lblPrice").GetComponent<Text>();
			int salableCount = this.m_selectUseable.GetSalableCount();
			this.m_saleCount = salableCount;
			component.text = this.m_saleCount + "/" + salableCount;
			component2.text = ((long)this.m_saleCount * (long)((ulong)this.m_selectUseable.m_coinSale)).ToString();
		}

		private void OnBag_SaleForm_ConfirmSale(CUIEvent uiEvent)
		{
			if (this.m_selectUseable == null)
			{
				return;
			}
			if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				CSymbolItem cSymbolItem = this.m_selectUseable as CSymbolItem;
				if (cSymbolItem != null && cSymbolItem.IsSaleNeedSecurePwd())
				{
					CSecurePwdSystem.TryToValidate(enOpPurpose.SALE_SYMBOL, enUIEventID.Bag_OnSecurePwdConfirmSale, default(stUIEventParams));
					return;
				}
			}
			CBagSystem.SendItemSaleMsg(new ListView<CSDT_ITEM_DELINFO>
			{
				new CSDT_ITEM_DELINFO
				{
					ullUniqueID = this.m_selectUseable.m_objID,
					iItemCnt = (int)((ushort)this.m_saleCount)
				}
			}, string.Empty);
		}

		private void OnSecurePwdConfirmSale(CUIEvent uiEvent)
		{
			CBagSystem.SendItemSaleMsg(new ListView<CSDT_ITEM_DELINFO>
			{
				new CSDT_ITEM_DELINFO
				{
					ullUniqueID = this.m_selectUseable.m_objID,
					iItemCnt = (int)((ushort)this.m_saleCount)
				}
			}, uiEvent.m_eventParams.pwd);
		}

		private void OnBag_OpenUseForm(CUIEvent uiEvent)
		{
			if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = this.m_selectUseable as CItem;
				if (cItem.m_itemData.bType == 4 && cItem.m_itemData.EftParam[2] != 0f)
				{
					this.PlayItemUseAnimation(this.m_selectUseable, 1, string.Empty);
					return;
				}
				if (cItem.m_itemData.bType == 1 || cItem.m_itemData.bType == 7 || cItem.m_itemData.bType == 4 || cItem.m_itemData.bType == 10 || cItem.m_itemData.bType == 11)
				{
					if (this.CheckNameChangeCard(cItem))
					{
						return;
					}
					this.m_useCount = 1;
					if (cItem.m_itemData.bType == 11)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_6"), enUIEventID.Bag_UseItem, enUIEventID.None, false);
						return;
					}
					if (CItem.IsSkinExperienceCard(cItem.m_baseID))
					{
						uint skinCfgId = (uint)cItem.m_itemData.EftParam[1];
						if (!CBagSystem.CanUseSkinExpCard(skinCfgId))
						{
							Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Skin_Exp_Card_Can_Not_Use"), false, 1.5f, null, new object[0]);
							return;
						}
					}
					if (cItem.m_itemData.bIsBatchUse == 0)
					{
						CBagSystem.SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0u, this.m_useCount, string.Empty);
						return;
					}
					CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBagSystem.s_bagUseFormPath, false, true);
					GameObject gameObject = cUIFormScript.gameObject;
					GameObject gameObject2 = gameObject.transform.Find("Panel_Left/itemCell").gameObject;
					Text component = gameObject.transform.Find("Panel_Left/lblName").GetComponent<Text>();
					Text component2 = gameObject.transform.Find("Panel_Left/lblCount").GetComponent<Text>();
					Text component3 = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
					CUICommonSystem.SetItemCell(cUIFormScript, gameObject2, this.m_selectUseable, true, false, false, false);
					component.text = this.m_selectUseable.m_name;
					component2.text = this.m_selectUseable.GetSalableCount().ToString();
					if (cItem.m_itemData.wBatchUseCnt > 0 && (int)cItem.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount())
					{
						component3.text = this.m_useCount + "/" + cItem.m_itemData.wBatchUseCnt;
					}
					else
					{
						component3.text = this.m_useCount + "/" + this.m_selectUseable.GetSalableCount();
					}
				}
				if (CItem.IsHeroExChangeCoupons(cItem.m_baseID))
				{
					Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Hero;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Exchange_More);
					return;
				}
				if (CItem.IsSkinExChangeCoupons(cItem.m_baseID))
				{
					Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Skin;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Exchange_More);
					return;
				}
				if (CItem.IsCryStalItem(cItem.m_baseID))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToTreasureTab);
					return;
				}
				if (cItem.m_itemData.bType == 8)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Union_Battle_ClickEntry);
					return;
				}
				if (cItem.m_itemData.bType == 9)
				{
					stUIEventParams par = default(stUIEventParams);
					par.commonUInt32Param1 = cItem.m_itemData.dwID;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Speaker_Form_Open, par);
					return;
				}
			}
			else if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_OpenForm);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Lock_Tip", true, 1.5f, null, new object[0]);
				}
			}
		}

		private bool CheckNameChangeCard(CItem item)
		{
			if (item.m_itemData.bType == 1)
			{
				if (CItem.IsPlayerNameChangeCard(item.m_baseID))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.NameChange_OpenPlayerNameChangeForm);
					return true;
				}
				if (CItem.IsGuildNameChangeCard(item.m_baseID))
				{
					if (CGuildSystem.HasGuildNameChangeAuthority())
					{
						Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.NameChange_OpenGuildNameChangeForm);
					}
					else
					{
						Singleton<CUIManager>.GetInstance().OpenTips("NameChange_GuildOnlyChairman", true, 1.5f, null, new object[0]);
					}
					return true;
				}
			}
			return false;
		}

		private void OnBag_UseForm_CountDown(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			if (this.m_useCount > 1)
			{
				this.m_useCount--;
			}
			component.text = this.m_useCount + "/" + this.m_selectUseable.GetSalableCount();
		}

		private void OnBag_UseForm_CountUp(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			CItem cItem = this.m_selectUseable as CItem;
			int num;
			if (cItem != null && cItem.m_itemData != null && cItem.m_itemData.wBatchUseCnt > 0 && (int)cItem.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount())
			{
				num = (int)cItem.m_itemData.wBatchUseCnt;
			}
			else
			{
				num = this.m_selectUseable.GetSalableCount();
			}
			if (this.m_useCount < num)
			{
				this.m_useCount++;
			}
			component.text = this.m_useCount + "/" + num;
		}

		private void OnBag_UseForm_CountMax(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			Text component = gameObject.transform.Find("Panel_Left/lblCount1").GetComponent<Text>();
			CItem cItem = this.m_selectUseable as CItem;
			int num;
			if (cItem != null && cItem.m_itemData != null && cItem.m_itemData.wBatchUseCnt > 0 && (int)cItem.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount())
			{
				num = (int)cItem.m_itemData.wBatchUseCnt;
			}
			else
			{
				num = this.m_selectUseable.GetSalableCount();
			}
			this.m_useCount = num;
			component.text = this.m_useCount + "/" + num;
		}

		private void OnTips_ItemSourceInfoOpen(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CBagSystem.s_itemGetSourceFormPath, false, true);
			CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
			GameObject gameObject = cUIFormScript.gameObject.transform.Find("Panel/itemCell").gameObject;
			Text component = cUIFormScript.gameObject.transform.Find("Panel/lblName").GetComponent<Text>();
			Text component2 = cUIFormScript.gameObject.transform.Find("Panel/lblDesc").GetComponent<Text>();
			CUIListScript component3 = cUIFormScript.gameObject.transform.Find("Panel/List").GetComponent<CUIListScript>();
			CUICommonSystem.SetItemCell(cUIFormScript, gameObject, iconUseable, false, false, false, false);
			component.text = iconUseable.m_name;
			component2.text = CUIUtility.StringReplace(iconUseable.m_description, new string[]
			{
				iconUseable.GetSalableCount().ToString()
			});
			CUICommonSystem.SetGetInfoToList(cUIFormScript, component3, iconUseable);
		}

		private void OnTips_ItemSourceElementClick(CUIEvent uiEvent)
		{
			stItemGetInfoParams itemGetInfoParams = uiEvent.m_eventParams.itemGetInfoParams;
			if (itemGetInfoParams.isCanDo)
			{
				if (itemGetInfoParams.getType == 1)
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Adv_OpenLevelForm;
					cUIEvent.m_eventParams.tag = itemGetInfoParams.levelInfo.iCfgID;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				}
				else if (itemGetInfoParams.getType == 2)
				{
					RES_SHOP_TYPE tag = (RES_SHOP_TYPE)uiEvent.m_eventParams.tag;
					CUIEvent cUIEvent2 = new CUIEvent();
					if (tag == RES_SHOP_TYPE.RES_SHOPTYPE_ARENA)
					{
						cUIEvent2.m_eventID = enUIEventID.Shop_OpenArenaShop;
					}
					else if (tag == RES_SHOP_TYPE.RES_SHOPTYPE_BURNING)
					{
						cUIEvent2.m_eventID = enUIEventID.Shop_OpenBurningShop;
					}
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
				}
				else if (itemGetInfoParams.getType == 3)
				{
					if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
					{
						Singleton<CMallSystem>.GetInstance().CurTab = CMallSystem.Tab.Mystery;
					}
					else
					{
						Singleton<CMallSystem>.GetInstance().CurTab = CMallSystem.Tab.Boutique;
					}
					CUIEvent cUIEvent3 = new CUIEvent();
					cUIEvent3.m_eventID = enUIEventID.Mall_OpenForm;
					cUIEvent3.m_eventParams.tag = 1;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent3);
				}
				Singleton<CUIManager>.GetInstance().CloseForm(CBagSystem.s_itemGetSourceFormPath);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(itemGetInfoParams.errorStr, false, 1.5f, null, new object[0]);
			}
		}

		private CUseableContainer GetAutoSaleUseableContainer(ref uint salePrice)
		{
			CUseableContainer cUseableContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer == null)
			{
				return cUseableContainer;
			}
			int curUseableCount = useableContainer.GetCurUseableCount();
			DateTime t = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && useableByIndex.m_isSale > 0 && cUseableContainer.GetCurUseableCount() < 5)
				{
					CItem cItem = useableByIndex as CItem;
					ulong ullInvalidTime = cItem.m_itemData.ullInvalidTime;
					if (ullInvalidTime > 0uL)
					{
						DateTime t2 = Utility.ULongToDateTime(ullInvalidTime);
						if (DateTime.Compare(t, t2) > 0)
						{
							salePrice += useableByIndex.m_coinSale * (uint)useableByIndex.m_stackCount;
							cUseableContainer.Add(useableByIndex);
						}
					}
				}
			}
			return cUseableContainer;
		}

		private void CheckOpenAutoSaleForm()
		{
			uint num = 0u;
			CUseableContainer autoSaleUseableContainer = this.GetAutoSaleUseableContainer(ref num);
			if (autoSaleUseableContainer.GetCurUseableCount() <= 0)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CBagSystem.s_bagAutoSaleFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			CUIEventScript uIEventScript = CUICommonSystem.GetUIEventScript(cUIFormScript.transform.Find("btnGroup/Button_Use"));
			if (uIEventScript != null)
			{
				uIEventScript.m_onClickEventParams.useableContainer = autoSaleUseableContainer;
			}
			CUIListScript uIListScript = CUICommonSystem.GetUIListScript(cUIFormScript.transform.Find("IconContainer"));
			if (uIListScript == null)
			{
				return;
			}
			int curUseableCount = autoSaleUseableContainer.GetCurUseableCount();
			uIListScript.SetElementAmount(curUseableCount);
			for (int i = 0; i < curUseableCount; i++)
			{
				CUICommonSystem.SetItemCell(cUIFormScript, uIListScript.GetElemenet(i).gameObject, autoSaleUseableContainer.GetUseableByIndex(i), true, false, false, false);
			}
			CUICommonSystem.SetTextContent(cUIFormScript.transform.Find("bg/lblPrice"), num.ToString());
		}

		private void OnClickAutoSaleBtn(CUIEvent uiEvent)
		{
			CUseableContainer useableContainer = uiEvent.m_eventParams.useableContainer;
			if (useableContainer == null)
			{
				return;
			}
			int curUseableCount = useableContainer.GetCurUseableCount();
			ListView<CSDT_ITEM_DELINFO> listView = new ListView<CSDT_ITEM_DELINFO>();
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				listView.Add(new CSDT_ITEM_DELINFO
				{
					ullUniqueID = useableByIndex.m_objID,
					iItemCnt = useableByIndex.m_stackCount
				});
			}
			CBagSystem.SendItemSaleMsg(listView, string.Empty);
		}

		public static void SetGetInfoToList(CUIFormScript formScript, CUIListScript list, CUseable itemUseable)
		{
			ResDT_ItemSrc_Info[] astSrcInfo;
			if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				astSrcInfo = ((CItem)itemUseable).m_itemData.astSrcInfo;
			}
			else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				astSrcInfo = ((CEquip)itemUseable).m_equipData.astSrcInfo;
			}
			else
			{
				if (itemUseable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					return;
				}
				astSrcInfo = ((CSymbolItem)itemUseable).m_SymbolData.astSrcInfo;
			}
			int num = 0;
			for (int i = 0; i < astSrcInfo.Length; i++)
			{
				if (astSrcInfo[i].bType == 0)
				{
					break;
				}
				num++;
			}
			if (list != null)
			{
				list.SetElementAmount(num);
				for (int j = 0; j < num; j++)
				{
					Transform transform = list.GetElemenet(j).transform.Find("sourceCell");
					Text component = transform.Find("lblName").GetComponent<Text>();
					Text component2 = transform.Find("lblLevel").GetComponent<Text>();
					Button component3 = transform.Find("lblOpen").GetComponent<Button>();
					Text component4 = transform.Find("lblClose").GetComponent<Text>();
					Text component5 = transform.Find("Elite").GetComponent<Text>();
					byte bType = astSrcInfo[j].bType;
					int dwID = (int)astSrcInfo[j].dwID;
					bool flag = false;
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.itemGetInfoParams = default(stItemGetInfoParams);
					eventParams.itemGetInfoParams.getType = bType;
					if (bType == 1)
					{
						ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)dwID);
						flag = Singleton<CAdventureSys>.GetInstance().IsLevelOpen(dataByKey.iCfgID);
						eventParams.itemGetInfoParams.levelInfo = dataByKey;
						eventParams.itemGetInfoParams.isCanDo = flag;
						if (!flag)
						{
							eventParams.itemGetInfoParams.errorStr = Singleton<CTextManager>.instance.GetText("Hero_Level_Not_Open");
						}
						component.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
						component2.text = dataByKey.iChapterId + "-" + dataByKey.bLevelNo;
						if (component5 != null)
						{
							component5.gameObject.CustomSetActive(dataByKey.bLevelDifficulty == 2);
						}
					}
					component3.gameObject.CustomSetActive(flag);
					component4.gameObject.CustomSetActive(!flag);
					CUIEventScript cUIEventScript = component3.gameObject.GetComponent<CUIEventScript>();
					if (cUIEventScript == null)
					{
						cUIEventScript = transform.gameObject.AddComponent<CUIEventScript>();
						cUIEventScript.Initialize(formScript);
					}
					cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemSourceElementClick, eventParams);
				}
				list.gameObject.CustomSetActive(true);
			}
		}

		public static bool IsExpProp(COM_ITEM_TYPE itemType, uint itemId)
		{
			if (itemType != COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				return false;
			}
			ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
			return dataByKey != null && dataByKey.EftParam[0] == 1f;
		}

		public static ListView<IHeroData> GetExpCardHeroList()
		{
			ListView<IHeroData> listView = new ListView<IHeroData>();
			List<uint> list = new List<uint>();
			ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return listView;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			for (int i = 0; i < allHeroList.Count; i++)
			{
				uint dwCfgID = allHeroList[i].dwCfgID;
				if (!CHeroDataFactory.IsHeroCanUse(dwCfgID))
				{
					for (int j = 0; j < curUseableCount; j++)
					{
						CUseable useableByIndex = useableContainer.GetUseableByIndex(j);
						if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
						{
							CItem cItem = useableByIndex as CItem;
							if ((int)cItem.m_itemData.EftParam[0] == 4 && cItem.m_itemData.EftParam[1] == dwCfgID && !list.Contains(dwCfgID))
							{
								list.Add(dwCfgID);
							}
						}
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				listView.Add(CHeroDataFactory.CreateHeroData(list[k]));
			}
			return listView;
		}

		public static void UseHeroExpCard(uint heroID)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			CItem cItem = null;
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem2 = useableByIndex as CItem;
					if ((int)cItem2.m_itemData.EftParam[0] == 4 && cItem2.m_itemData.EftParam[1] == heroID)
					{
						if (cItem == null)
						{
							cItem = cItem2;
						}
						else if (cItem2.m_itemData.EftParam[2] < cItem.m_itemData.EftParam[2])
						{
							cItem = cItem2;
						}
					}
				}
			}
			if (cItem != null)
			{
				CBagSystem.SendItemUseMsgToServer(cItem.m_objID, 0u, 1, string.Empty);
			}
		}

		public static bool IsHaveSkinExpCard(uint skinCfgId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "IsHaveSkinExpCard role is null");
				return false;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = useableByIndex as CItem;
					if (cItem != null && (int)cItem.m_itemData.EftParam[0] == 5 && cItem.m_itemData.EftParam[1] == skinCfgId)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool CanUseSkinExpCard(uint skinCfgId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "IsHaveSkinExpCard role is null");
				return false;
			}
			if (!CBagSystem.IsHaveSkinExpCard(skinCfgId))
			{
				return false;
			}
			uint num = 0u;
			uint num2 = 0u;
			CSkinInfo.ResolveHeroSkin(skinCfgId, out num, out num2);
			return masterRoleInfo.IsHaveHero(num, true) || masterRoleInfo.IsFreeHero(num);
		}

		public static void UseSkinExpCard(uint skinCfgId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (!CBagSystem.CanUseSkinExpCard(skinCfgId))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Skin_Exp_Card_Can_Not_Use"), false, 1.5f, null, new object[0]);
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			CItem cItem = null;
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem2 = useableByIndex as CItem;
					if (cItem2 != null && (int)cItem2.m_itemData.EftParam[0] == 5 && cItem2.m_itemData.EftParam[1] == skinCfgId)
					{
						if (cItem == null)
						{
							cItem = cItem2;
						}
						else if (cItem2.m_itemData.EftParam[2] < cItem.m_itemData.EftParam[2])
						{
							cItem = cItem2;
						}
					}
				}
			}
			if (cItem != null)
			{
				CBagSystem.SendItemUseMsgToServer(cItem.m_objID, 0u, 1, string.Empty);
			}
		}

		public static void SendItemSaleMsg(ListView<CSDT_ITEM_DELINFO> itemList, string pwd = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1101u);
			CSDT_ITEM_DELLIST cSDT_ITEM_DELLIST = new CSDT_ITEM_DELLIST();
			cSDT_ITEM_DELLIST.wItemCnt = (ushort)itemList.Count;
			cSDT_ITEM_DELLIST.astItemList = LinqS.ToArray<CSDT_ITEM_DELINFO>(itemList);
			cSPkg.stPkgData.stItemSale.stSaleList = cSDT_ITEM_DELLIST;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stItemSale.szPswdInfo);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendItemUseMsgToServer(ulong itemID, uint heroID = 0u, int useCount = 0, string pwd = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1107u);
			cSPkg.stPkgData.stPropUse.ullUniqueID = itemID;
			cSPkg.stPkgData.stPropUse.dwHeroID = heroID;
			cSPkg.stPkgData.stPropUse.wItemCnt = (ushort)useCount;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stPropUse.szPswdInfo);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1102)]
		public static void ReciveItemAdd(CSPkg msg)
		{
			COMDT_ITEM_ADDLIST stAddList = msg.stPkgData.stItemAdd.stAddList;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer == null)
			{
				return;
			}
			int iCount = 0;
			for (int i = 0; i < (int)stAddList.wItemCnt; i++)
			{
				COMDT_ITEM_ADDINFO cOMDT_ITEM_ADDINFO = stAddList.astItemList[i];
				if (cOMDT_ITEM_ADDINFO.wItemType == 2)
				{
					iCount = cOMDT_ITEM_ADDINFO.stItemInfo.stPropInfo.iOverCnt;
				}
				else if (cOMDT_ITEM_ADDINFO.wItemType == 3)
				{
					iCount = cOMDT_ITEM_ADDINFO.stItemInfo.stEquipInfo.iOverCnt;
				}
				else if (cOMDT_ITEM_ADDINFO.wItemType == 5)
				{
					iCount = cOMDT_ITEM_ADDINFO.stItemInfo.stSymbolInfo.iOverCnt;
				}
				CUseable cUseable = useableContainer.Add((COM_ITEM_TYPE)cOMDT_ITEM_ADDINFO.wItemType, cOMDT_ITEM_ADDINFO.ullUniqueID, cOMDT_ITEM_ADDINFO.dwItemID, iCount, useableContainer.GetMaxAddTime());
				if (cUseable != null)
				{
					if (cUseable != null && cUseable.m_stackCount >= cUseable.m_stackMax && (cUseable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL || !((CSymbolItem)cUseable).IsGuildSymbol()))
					{
						Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_1", new string[]
						{
							cUseable.m_name,
							cUseable.m_stackMax.ToString()
						}), false, 1.5f, null, new object[0]);
					}
				}
			}
			//switch (msg.stPkgData.stItemAdd.bAddReason)
			//{
			//}
			Singleton<CBagSystem>.GetInstance().RefreshBagForm();
			Singleton<CHeroInfoSystem2>.GetInstance().RefreshHeroInfoForm();
			Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
		}

		[MessageHandler(1103)]
		public static void ReciveItemDel(CSPkg msg)
		{
			CSDT_ITEM_DELLIST stDelList = msg.stPkgData.stItemDel.stDelList;
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			CSDT_ITEM_DELINFO cSDT_ITEM_DELINFO = null;
			uint key = 0u;
			for (int i = 0; i < (int)stDelList.wItemCnt; i++)
			{
				cSDT_ITEM_DELINFO = stDelList.astItemList[i];
				if (cSDT_ITEM_DELINFO != null)
				{
					CUseable useableByObjID = useableContainer.GetUseableByObjID(cSDT_ITEM_DELINFO.ullUniqueID);
					if (useableByObjID != null)
					{
						key = useableByObjID.m_baseID;
					}
					useableContainer.Remove(cSDT_ITEM_DELINFO.ullUniqueID, cSDT_ITEM_DELINFO.iItemCnt);
				}
			}
			switch (msg.stPkgData.stItemDel.bDelReason)
			{
			case 3:
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Shop_Sale_Success"), false, 1.5f, null, new object[0]);
				Singleton<CUIManager>.GetInstance().CloseForm(CBagSystem.s_bagSaleFormPath);
				Singleton<EventRouter>.instance.BroadCastEvent<CSDT_ITEM_DELLIST>(EventID.BAG_ITEM_SALED, stDelList);
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_backpack_sell", null);
				break;
			case 4:
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_backpack_sell", null);
				if (cSDT_ITEM_DELINFO != null)
				{
					ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(key);
					if (dataByKey != null && dataByKey.iUseShowTip != 0)
					{
						Singleton<CUIManager>.GetInstance().OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ItemTakeEffectTip"), StringHelper.UTF8BytesToString(ref dataByKey.szName)), false, 1.5f, null, new object[0]);
					}
				}
				break;
			case 7:
				Singleton<CHeroInfoSystem2>.GetInstance().RefreshHeroInfoForm();
				break;
			}
			Singleton<CBagSystem>.GetInstance().RefreshBagForm();
			Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
		}

		[MessageHandler(1109)]
		public static void ReciveItemList(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int iCount = 0;
			for (int i = 0; i < (int)msg.stPkgData.stPkgDetail.stPkgInfo.wItemCnt; i++)
			{
				COMDT_ITEM_POSINFO cOMDT_ITEM_POSINFO = msg.stPkgData.stPkgDetail.stPkgInfo.astItemList[i];
				if (cOMDT_ITEM_POSINFO.wItemType == 2)
				{
					iCount = cOMDT_ITEM_POSINFO.stItemInfo.stPropInfo.iOverCnt;
				}
				else if (cOMDT_ITEM_POSINFO.wItemType == 3)
				{
					iCount = cOMDT_ITEM_POSINFO.stItemInfo.stEquipInfo.iOverCnt;
				}
				else if (cOMDT_ITEM_POSINFO.wItemType == 5)
				{
					iCount = cOMDT_ITEM_POSINFO.stItemInfo.stSymbolInfo.iOverCnt;
				}
				useableContainer.Add((COM_ITEM_TYPE)cOMDT_ITEM_POSINFO.wItemType, cOMDT_ITEM_POSINFO.ullUniqueID, cOMDT_ITEM_POSINFO.dwItemID, iCount, cOMDT_ITEM_POSINFO.iAddUpdTime);
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
		}

		[MessageHandler(1172)]
		public static void ReceiveItemUse(CSPkg msg)
		{
			uint dwPropID = msg.stPkgData.stPropUseRsp.dwPropID;
			CItem cItem = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwPropID, 0) as CItem;
			if (cItem == null)
			{
				return;
			}
			RES_PROP_TYPE_TYPE bType = (RES_PROP_TYPE_TYPE)cItem.m_itemData.bType;
			if (bType == RES_PROP_TYPE_TYPE.RES_PROP_TYPE_BATTLERECORD_CARD)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_5"), cItem.m_name), false, 1.5f, null, new object[0]);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					masterRoleInfo.CleanUpBattleRecord();
				}
			}
		}

		public void UpdateBagRed()
		{
		}
	}
}

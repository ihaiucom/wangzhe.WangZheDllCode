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
	public class CSymbolMakeController : Singleton<CSymbolMakeController>
	{
		public static string s_symbolBreakPath = "UGUI/Form/System/Symbol/Form_SymbolBreak.prefab";

		public static string s_symbolBreakDetailPath = "UGUI/Form/System/Symbol/Form_SymbolBreakDetail.prefab";

		public static string s_symbolTransformPath = "UGUI/Form/System/Symbol/Form_SymbolTransform.prefab";

		public static int s_maxSymbolBreakLevel = 4;

		private static ListView<ResSymbolInfo> s_allSymbolCfgList = new ListView<ResSymbolInfo>();

		private ListView<ResSymbolInfo> m_symbolMakeList = new ListView<ResSymbolInfo>();

		private ListView<CBreakSymbolItem>[] m_breakSymbolList = new ListView<CBreakSymbolItem>[CSymbolMakeController.s_maxSymbolBreakLevel];

		private ListView<CSDT_SYMBOLOPT_INFO> m_svrBreakSymbolList = new ListView<CSDT_SYMBOLOPT_INFO>();

		private int m_symbolFilterLevel = 1;

		private enSymbolType m_symbolFilterType;

		private ResSymbolInfo m_curTransformSymbol;

		private static int s_breakSymbolCoinCnt = 0;

		private ushort m_breakLevelMask;

		private int m_breakDetailIndex;

		private GameObject m_container;

		public enSymbolMakeSource Source
		{
			get;
			set;
		}

		public override void Init()
		{
			this.InitSymbolCfgList();
			this.InitBreakSymbolList();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakSymbolConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakExcessSymbolConfirm));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.LOTTERY_GET_NEW_SYMBOL, new Action(this.OnGetNewSymbol));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakSymbolConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakExcessSymbolConfirm));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.LOTTERY_GET_NEW_SYMBOL, new Action(this.OnGetNewSymbol));
		}

		public void Clear()
		{
			this.ClearSymbolMakeData();
		}

		public void Load(CUIFormScript form)
		{
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					CUICommonSystem.LoadUIPrefab(CSymbolSystem.s_symbolMakeMallModulePath, CSymbolSystem.s_symbolMakePanel, form.GetWidget(3), form);
				}
			}
			else
			{
				CUICommonSystem.LoadUIPrefab(CSymbolSystem.s_symbolMakeModulePath, CSymbolSystem.s_symbolMakePanel, form.GetWidget(4), form);
			}
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = null;
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					x = Utility.FindChild(form.GetWidget(3), CSymbolSystem.s_symbolMakePanel);
				}
			}
			else
			{
				x = Utility.FindChild(form.GetWidget(4), CSymbolSystem.s_symbolMakePanel);
			}
			return !(x == null);
		}

		private void SetContainer(CUIFormScript form)
		{
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					this.m_container = Utility.FindChild(form.GetWidget(3), CSymbolSystem.s_symbolMakePanel);
				}
			}
			else
			{
				this.m_container = Utility.FindChild(form.GetWidget(4), CSymbolSystem.s_symbolMakePanel);
			}
		}

		private void InitBreakSymbolList()
		{
			for (int i = 0; i < this.m_breakSymbolList.Length; i++)
			{
				if (this.m_breakSymbolList[i] == null)
				{
					this.m_breakSymbolList[i] = new ListView<CBreakSymbolItem>();
				}
				this.m_breakSymbolList[i].Clear();
			}
		}

		private void InitSymbolCfgList()
		{
			CSymbolMakeController.s_allSymbolCfgList.Clear();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.symbolInfoDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResSymbolInfo resSymbolInfo = (ResSymbolInfo)current.Value;
				if (resSymbolInfo != null && resSymbolInfo.bIsMakeShow > 0)
				{
					CSymbolMakeController.s_allSymbolCfgList.Add(resSymbolInfo);
				}
			}
		}

		private void ClearSymbolMakeData()
		{
			this.m_symbolFilterLevel = 1;
			this.m_symbolFilterType = enSymbolType.All;
		}

		public void SwitchToSymbolMakePanel(CUIFormScript form)
		{
			if (form == null || form.IsClosed())
			{
				return;
			}
			this.SetContainer(form);
			if (this.m_container == null)
			{
				return;
			}
			this.m_container.CustomSetActive(true);
			this.ClearSymbolMakeData();
			this.RefreshSymbolMakeForm();
			this.ToggleTitle();
			CSymbolSystem.RefreshSymbolCntText(true);
			CUseable iconUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, 0);
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.iconUseable = iconUseable;
			eventParams.tag = 3;
			CUIEventScript component = form.GetWidget(6).GetComponent<CUIEventScript>();
			if (component != null)
			{
				component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
				component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
				component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
				component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryMallSymbolMake, new uint[0]);
		}

		private void ToggleTitle()
		{
			if (this.m_container == null)
			{
				return;
			}
			GameObject obj = Utility.FindChild(this.m_container, "Title");
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source != enSymbolMakeSource.Mall)
				{
					obj.CustomSetActive(true);
				}
				else
				{
					obj.CustomSetActive(false);
				}
			}
			else
			{
				obj.CustomSetActive(true);
			}
		}

		public void RefreshSymbolMakeForm()
		{
			CUIFormScript x = null;
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					x = Singleton<CMallSystem>.GetInstance().m_MallForm;
				}
			}
			else
			{
				x = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
			}
			if (x == null)
			{
				return;
			}
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			string[] titleList = new string[]
			{
				instance.GetText("Symbol_Prop_Tab_All"),
				instance.GetText("Symbol_Prop_Tab_Atk"),
				instance.GetText("Symbol_Prop_Tab_Hp"),
				instance.GetText("Symbol_Prop_Tab_Defense"),
				instance.GetText("Symbol_Prop_Tab_Function"),
				instance.GetText("Symbol_Prop_Tab_HpSteal"),
				instance.GetText("Symbol_Prop_Tab_AtkSpeed"),
				instance.GetText("Symbol_Prop_Tab_Crit"),
				instance.GetText("Symbol_Prop_Tab_Penetrate")
			};
			GameObject listObj = Utility.FindChild(this.m_container, "typeList");
			CUICommonSystem.InitMenuPanel(listObj, titleList, (int)this.m_symbolFilterType, true);
			string[] titleList2 = new string[]
			{
				"1",
				"2",
				"3",
				"4",
				"5"
			};
			GameObject listObj2 = Utility.FindChild(this.m_container, "Panel_SymbolLevel/levelList");
			CUICommonSystem.InitMenuPanel(listObj2, titleList2, this.m_symbolFilterLevel - 1, true);
			Singleton<CSymbolSystem>.GetInstance().SetSymbolData();
			this.SetSymbolMakeListData();
			this.RefreshSymbolMakeList();
			this.SetAvailableBtn();
			Text componetInChild = Utility.GetComponetInChild<Text>(this.m_container, "Panel_SymbolBreak/breakCoinCntText");
			componetInChild.text = this.GetBreakExcessSymbolCoinCnt(65535).ToString();
		}

		private void SetAvailableBtn()
		{
			if (this.m_container == null)
			{
				return;
			}
			GameObject obj = Utility.FindChild(this.m_container, "Panel_SymbolBreak");
			GameObject gameObject = Utility.FindChild(this.m_container, "Panel_SymbolDraw");
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source != enSymbolMakeSource.Mall)
				{
					obj.CustomSetActive(true);
					gameObject.CustomSetActive(false);
				}
				else
				{
					obj.CustomSetActive(false);
					gameObject.CustomSetActive(true);
					GameObject target = Utility.FindChild(gameObject, "btnJump");
					if (Singleton<CMallSystem>.GetInstance().HasFreeDrawCnt(enRedID.Mall_SymbolTab))
					{
						CUIRedDotSystem.AddRedDot(target, enRedDotPos.enTopRight, 0, 0, 0);
					}
					else
					{
						CUIRedDotSystem.DelRedDot(target);
					}
				}
			}
			else
			{
				obj.CustomSetActive(true);
				gameObject.CustomSetActive(false);
			}
		}

		private bool SetBreakSymbolList()
		{
			bool result = false;
			this.m_svrBreakSymbolList.Clear();
			int num = this.m_breakSymbolList.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.m_breakSymbolList[i] != null)
				{
					for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
					{
						if (this.m_breakSymbolList[i][j].symbol != null && this.m_breakSymbolList[i][j].bBreak)
						{
							CSDT_SYMBOLOPT_INFO cSDT_SYMBOLOPT_INFO = new CSDT_SYMBOLOPT_INFO();
							cSDT_SYMBOLOPT_INFO.dwSymbolID = this.m_breakSymbolList[i][j].symbol.m_baseID;
							cSDT_SYMBOLOPT_INFO.iSymbolCnt = this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt();
							if (this.m_breakSymbolList[i][j].symbol.m_SymbolData.bNeedPswd > 0)
							{
								result = true;
							}
							this.m_svrBreakSymbolList.Add(cSDT_SYMBOLOPT_INFO);
						}
					}
				}
			}
			return result;
		}

		private bool CheckSymbolBreak(CSymbolItem symbol, ushort breakLvlMask)
		{
			return symbol != null && (int)symbol.m_SymbolData.wLevel < CSymbolInfo.s_maxSymbolLevel && symbol.m_stackCount > symbol.GetMaxWearCnt() && (1 << (int)symbol.m_SymbolData.wLevel & (int)breakLvlMask) != 0;
		}

		private void SetSymbolMakeListData()
		{
			this.m_symbolMakeList.Clear();
			int count = CSymbolMakeController.s_allSymbolCfgList.Count;
			for (int i = 0; i < count; i++)
			{
				if (CSymbolMakeController.s_allSymbolCfgList[i] != null && (int)CSymbolMakeController.s_allSymbolCfgList[i].wLevel == this.m_symbolFilterLevel && (this.m_symbolFilterType == enSymbolType.All || ((int)CSymbolMakeController.s_allSymbolCfgList[i].wType & 1 << (int)this.m_symbolFilterType) != 0))
				{
					this.m_symbolMakeList.Add(CSymbolMakeController.s_allSymbolCfgList[i]);
				}
			}
		}

		private void RefreshSymbolMakeList()
		{
			if (this.m_container == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this.m_container, "symbolMakeList");
			componetInChild.SetElementAmount(this.m_symbolMakeList.Count);
		}

		private void OnSymbolTypeMenuSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.m_symbolFilterType = (enSymbolType)selectedIndex;
			this.SetSymbolMakeListData();
			this.RefreshSymbolMakeList();
		}

		private void OnSymbolLevelMenuSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			this.m_symbolFilterLevel = component.GetSelectedIndex() + 1;
			this.SetSymbolMakeListData();
			this.RefreshSymbolMakeList();
		}

		private void OnSymbolMakeListEnable(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetIndexInBelongedList < 0 || uiEvent.m_srcWidgetIndexInBelongedList >= this.m_symbolMakeList.Count)
			{
				DebugHelper.Assert(false, "OnSymbolMakeListEnable index out of range");
				return;
			}
			CSymbolSystem.RefreshSymbolItem(this.m_symbolMakeList[uiEvent.m_srcWidgetIndexInBelongedList], uiEvent.m_srcWidget, uiEvent.m_srcFormScript, this.Source);
		}

		private void OnSymbolMakeListClick(CUIEvent uiEvent)
		{
			this.m_curTransformSymbol = uiEvent.m_eventParams.symbolTransParam.symbolCfgInfo;
			enSymbolMakeSource source = this.Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					CSymbolBuyPickDialog.Show(this.m_curTransformSymbol, RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN, 10000f, new CSymbolBuyPickDialog.OnConfirmBuyDelegate(this.BuySymbol), null, null);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenForm(CSymbolMakeController.s_symbolTransformPath, false, true);
			}
			this.RefreshSymbolTransformForm();
		}

		public void RefreshSymbolTransformForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolTransformPath);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (form == null || this.m_curTransformSymbol == null || masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("Panel_SymbolTranform/Panel_Content").gameObject;
			Image component = gameObject.transform.Find("iconImage").GetComponent<Image>();
			component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, this.m_curTransformSymbol.dwIcon), form, true, false, false, false);
			Text component2 = gameObject.transform.Find("nameText").GetComponent<Text>();
			component2.text = StringHelper.UTF8BytesToString(ref this.m_curTransformSymbol.szName);
			Text component3 = gameObject.transform.Find("countText").GetComponent<Text>();
			component3.text = string.Empty;
			int num = 0;
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (useableContainer != null)
			{
				num = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.m_curTransformSymbol.dwID);
				CTextManager instance = Singleton<CTextManager>.GetInstance();
				component3.text = ((num <= 0) ? instance.GetText("Symbol_Not_Own") : string.Format(instance.GetText("Symbol_Own_Cnt"), num));
			}
			GameObject gameObject2 = gameObject.transform.Find("symbolPropPanel").gameObject;
			CSymbolSystem.RefreshSymbolPropContent(gameObject2, this.m_curTransformSymbol.dwID);
			Text component4 = gameObject.transform.Find("makeCoinText").GetComponent<Text>();
			component4.text = this.m_curTransformSymbol.dwMakeCoin.ToString();
			Text component5 = gameObject.transform.Find("breakCoinText").GetComponent<Text>();
			component5.text = this.m_curTransformSymbol.dwBreakCoin.ToString();
			GameObject gameObject3 = gameObject.transform.Find("btnBreak").gameObject;
			if (num <= 0)
			{
				CUICommonSystem.SetButtonEnableWithShader(gameObject3.GetComponent<Button>(), false, true);
			}
			else
			{
				CUICommonSystem.SetButtonEnableWithShader(gameObject3.GetComponent<Button>(), true, true);
			}
		}

		private void OnMakeSymbolClick(CUIEvent uiEvent)
		{
			if (this.m_curTransformSymbol == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo.SymbolCoin >= this.m_curTransformSymbol.dwMakeCoin)
			{
				CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
				if (symbolByCfgID != null)
				{
					if (symbolByCfgID.m_stackCount >= symbolByCfgID.m_SymbolData.iOverLimit)
					{
						Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Make_MaxCnt_Limit", true, 1.5f, null, new object[0]);
					}
					else if (symbolByCfgID.m_stackCount >= CSymbolWearController.s_maxSameIDSymbolEquipNum)
					{
						string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Make_WearMaxLimit_Tip");
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemMakeConfirm, enUIEventID.None, false);
					}
					else
					{
						this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
					}
				}
				else
				{
					this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
				}
			}
			else if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Symbol_DynamicBlock_Coin_Not_Enough_Tip", true, 1.5f, null, new object[0]);
			}
			else
			{
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Not_Enough_Tip");
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text2, enUIEventID.Symbol_Jump_To_MiShu, enUIEventID.None, false);
			}
		}

		private void OnSendReqSymbolMake(uint symbolId, int count = 1)
		{
			if (Singleton<CSymbolSystem>.GetInstance().m_selectMenuType == enSymbolMenuType.SymbolRecommend)
			{
				uint curHeroId = Singleton<CSymbolSystem>.GetInstance().m_symbolRcmdCtrl.m_curHeroId;
				CSymbolMakeController.SendReqSymbolMake(curHeroId, symbolId, count);
			}
			else
			{
				CSymbolMakeController.SendReqSymbolMake(0u, symbolId, count);
			}
		}

		private void OnSendReqSymbolBreak(uint symbolId, int count = 1, string pwd = "")
		{
			if (Singleton<CSymbolSystem>.GetInstance().m_selectMenuType == enSymbolMenuType.SymbolRecommend)
			{
				uint curHeroId = Singleton<CSymbolSystem>.GetInstance().m_symbolRcmdCtrl.m_curHeroId;
				CSymbolMakeController.SendReqSymbolBreak(curHeroId, symbolId, count, pwd);
			}
			else
			{
				CSymbolMakeController.SendReqSymbolBreak(0u, symbolId, count, pwd);
			}
		}

		private void OnCoinNotEnoughGotoSymbolMall(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolMakeController.s_symbolTransformPath);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToSymbolTab);
		}

		private void OnItemMakeConfirm(CUIEvent uiEvent)
		{
			if (this.m_curTransformSymbol == null)
			{
				return;
			}
			this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
		}

		private void OnBreakSymbolClick(CUIEvent uiEvent)
		{
			if (this.m_curTransformSymbol == null)
			{
				return;
			}
			CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
			if (symbolByCfgID == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Symbol__Item_Not_Exist_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			if (symbolByCfgID.m_stackCount > symbolByCfgID.GetMaxWearCnt())
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Tip");
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
			}
			else
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Item_Wear_Tip"), masterRoleInfo.m_symbolInfo.GetMaxWearSymbolPageName(symbolByCfgID));
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
				}
			}
		}

		private CSymbolItem GetSymbolByCfgID(uint cfgId)
		{
			CSymbolItem result = null;
			ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
			for (int i = 0; i < allSymbolList.Count; i++)
			{
				CSymbolItem cSymbolItem = allSymbolList[i];
				if (cSymbolItem != null && cSymbolItem.m_baseID == cfgId)
				{
					result = cSymbolItem;
					break;
				}
			}
			return result;
		}

		private void BuySymbol(ResSymbolInfo symbol, uint count, bool needConfirm, CUIEvent uiEvent)
		{
			this.OnSendReqSymbolMake(symbol.dwID, (int)count);
		}

		private void OnBreakSymbolConfirm(CUIEvent uiEvent)
		{
			if (this.m_curTransformSymbol == null)
			{
				return;
			}
			CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
			if (symbolByCfgID != null)
			{
				if (symbolByCfgID.m_SymbolData.bNeedPswd > 0)
				{
					CSecurePwdSystem.TryToValidate(enOpPurpose.BREAK_SYMBOL, enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, default(stUIEventParams));
				}
				else
				{
					this.OnSendReqSymbolBreak(symbolByCfgID.m_baseID, 1, string.Empty);
				}
			}
		}

		private void OnSecurePwdBreakSymbolConfirm(CUIEvent uiEvent)
		{
			if (this.m_curTransformSymbol == null)
			{
				return;
			}
			CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
			if (symbolByCfgID != null)
			{
				this.OnSendReqSymbolBreak(symbolByCfgID.m_baseID, 1, uiEvent.m_eventParams.pwd);
			}
		}

		private int GetSelectBreakSymbolCoinCnt()
		{
			int num = 0;
			for (int i = 0; i < this.m_breakSymbolList.Length; i++)
			{
				if (this.m_breakSymbolList[i] != null)
				{
					for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
					{
						if (this.m_breakSymbolList[i][j].symbol != null && this.m_breakSymbolList[i][j].bBreak)
						{
							num += (this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt()) * (int)this.m_breakSymbolList[i][j].symbol.m_SymbolData.dwBreakCoin;
						}
					}
				}
			}
			return num;
		}

		private int GetBreakExcessSymbolCoinCnt(ushort breakLvlMask = 65535)
		{
			int num = 0;
			ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
			for (int i = 0; i < allSymbolList.Count; i++)
			{
				if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
				{
					num += (allSymbolList[i].m_stackCount - allSymbolList[i].GetMaxWearCnt()) * (int)allSymbolList[i].m_SymbolData.dwBreakCoin;
				}
			}
			return num;
		}

		private void OnReceiveBreakItemList()
		{
			for (int i = 0; i < this.m_breakSymbolList.Length; i++)
			{
				int j = 0;
				while (j < this.m_breakSymbolList[i].Count)
				{
					if (this.m_breakSymbolList[i][j].bBreak)
					{
						this.m_breakSymbolList[i].RemoveAt(j);
					}
					else
					{
						j++;
					}
				}
			}
		}

		private void OnBreakListItemEnable(CUIEvent uiEvent)
		{
			if (this.m_breakDetailIndex < 0 || this.m_breakDetailIndex >= this.m_breakSymbolList.Length || this.m_breakSymbolList[this.m_breakDetailIndex] == null || null == uiEvent.m_srcWidget)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_breakSymbolList[this.m_breakDetailIndex].Count)
			{
				return;
			}
			CBreakSymbolItem cBreakSymbolItem = this.m_breakSymbolList[this.m_breakDetailIndex][srcWidgetIndexInBelongedList];
			if (cBreakSymbolItem != null && cBreakSymbolItem.symbol != null)
			{
				GameObject srcWidget = uiEvent.m_srcWidget;
				Transform transform = srcWidget.transform.Find("itemCell");
				Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.SetSymbolListItem(uiEvent.m_srcFormScript, transform.gameObject, cBreakSymbolItem.symbol);
				Transform transform2 = transform.Find("selectFlag");
				if (transform2 != null)
				{
					CUIEventScript component = transform2.GetComponent<CUIEventScript>();
					if (component != null)
					{
						component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_BreakListItemSelToggle, new stUIEventParams
						{
							tag = srcWidgetIndexInBelongedList
						});
					}
					Toggle component2 = transform2.GetComponent<Toggle>();
					component2.isOn = cBreakSymbolItem.bBreakToggle;
				}
			}
		}

		private void OnBreakListItemSelToggle(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			if (tag < 0 || tag >= this.m_breakSymbolList[this.m_breakDetailIndex].Count)
			{
				return;
			}
			CBreakSymbolItem cBreakSymbolItem = this.m_breakSymbolList[this.m_breakDetailIndex][tag];
			if (cBreakSymbolItem != null && cBreakSymbolItem.symbol != null && uiEvent.m_srcWidget != null)
			{
				Toggle component = uiEvent.m_srcWidget.GetComponent<Toggle>();
				cBreakSymbolItem.bBreakToggle = component.isOn;
			}
		}

		private void OnOpenBreakDetailForm(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolMakeController.s_symbolBreakDetailPath, false, true);
			if (null == cUIFormScript)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			Transform transform = cUIFormScript.transform.Find("Panel_SymbolBreak/Panel_Content/List");
			if (transform != null)
			{
				CUIListScript component = transform.GetComponent<CUIListScript>();
				if (tag >= 0 && tag < this.m_breakSymbolList.Length && this.m_breakSymbolList[tag] != null)
				{
					this.m_breakDetailIndex = tag;
					this.m_breakSymbolList[tag].Sort();
					component.SetElementAmount(this.m_breakSymbolList[tag].Count);
				}
			}
		}

		private void OnBreakDetailFormConfirm(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolBreakDetailPath);
			if (null == form)
			{
				return;
			}
			if (this.m_breakDetailIndex >= 0 && this.m_breakDetailIndex < this.m_breakSymbolList.Length && this.m_breakSymbolList[this.m_breakDetailIndex] != null)
			{
				for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
				{
					if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle)
					{
						this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle;
					}
				}
			}
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolMakeController.s_symbolBreakDetailPath);
			this.RefreshSymbolBreakForm();
		}

		private void OnBreakDetailFormCancle(CUIEvent uiEvent)
		{
			if (this.m_breakDetailIndex >= 0 && this.m_breakDetailIndex < this.m_breakSymbolList.Length && this.m_breakSymbolList[this.m_breakDetailIndex] != null)
			{
				for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
				{
					if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak)
					{
						this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak;
					}
				}
			}
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolMakeController.s_symbolBreakDetailPath);
			this.RefreshSymbolBreakForm();
		}

		private void OnBreakExcessSymbolClick(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolMakeController.s_symbolBreakPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			for (int i = 0; i < this.m_breakSymbolList.Length; i++)
			{
				this.m_breakSymbolList[i].Clear();
			}
			this.m_breakLevelMask = 0;
			Transform transform = cUIFormScript.transform.Find("Panel_SymbolBreak/Panel_Content");
			for (int j = 0; j < CSymbolMakeController.s_maxSymbolBreakLevel; j++)
			{
				GameObject gameObject = transform.Find(string.Format("breakElement{0}", j)).gameObject;
				Transform transform2 = gameObject.transform.Find("OnBreakBtn");
				Transform transform3 = gameObject.transform.Find("OnBreakBtn/Checkmark");
				Transform transform4 = gameObject.transform.Find("detailButton");
				if (transform2 != null && transform3 != null)
				{
					transform3.gameObject.CustomSetActive(false);
					CUIEventScript component = transform2.GetComponent<CUIEventScript>();
					if (component != null)
					{
						component.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_SelectBreakLvlItem, new stUIEventParams
						{
							tag = j
						});
					}
				}
				if (transform4 != null)
				{
					transform4.gameObject.CustomSetActive(false);
					CUIEventScript component2 = transform4.GetComponent<CUIEventScript>();
					if (component2 != null)
					{
						component2.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_OpenBreakDetailForm, new stUIEventParams
						{
							tag = j
						});
					}
				}
			}
			this.RefreshSymbolBreakForm();
		}

		private void SetBreakSymbolListData(int index)
		{
			if (index >= 0 && index < this.m_breakSymbolList.Length)
			{
				ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
				int count = allSymbolList.Count;
				ushort breakLvlMask = (ushort)(1 << index + 1);
				for (int i = 0; i < count; i++)
				{
					if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
					{
						CBreakSymbolItem item = new CBreakSymbolItem(allSymbolList[i], true);
						this.m_breakSymbolList[index].Add(item);
					}
				}
			}
		}

		private void ClearBreakSymbolListData(int index)
		{
			if (index >= 0 && index < this.m_breakSymbolList.Length)
			{
				this.m_breakSymbolList[index].Clear();
			}
		}

		private bool IsAllSymbolBreak(int index)
		{
			if (index >= 0 && index < this.m_breakSymbolList.Length)
			{
				for (int i = 0; i < this.m_breakSymbolList[index].Count; i++)
				{
					if (!this.m_breakSymbolList[index][i].bBreak)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void OnSelectBreakLvlItem(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolMakeController.s_symbolBreakPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Transform transform = cUIFormScript.transform.Find(string.Format("Panel_SymbolBreak/Panel_Content/breakElement{0}", tag));
			if (transform != null)
			{
				Transform transform2 = transform.transform.Find("OnBreakBtn/Checkmark");
				Transform transform3 = transform.transform.Find("detailButton");
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(!transform2.gameObject.activeSelf);
					this.ClearBreakSymbolListData(tag);
					if (transform2.gameObject.activeSelf)
					{
						this.SetBreakSymbolListData(tag);
					}
					if (transform3 != null)
					{
						transform3.gameObject.CustomSetActive(transform2.gameObject.activeSelf);
					}
				}
			}
			this.RefreshSymbolBreakForm();
		}

		private void RefreshSymbolBreakForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolBreakPath);
			if (null == form)
			{
				return;
			}
			Transform transform = form.transform.Find("Panel_SymbolBreak/Panel_Content");
			int num = 0;
			for (int i = 0; i < CSymbolMakeController.s_maxSymbolBreakLevel; i++)
			{
				GameObject gameObject = transform.Find(string.Format("breakElement{0}", i)).gameObject;
				Transform transform2 = gameObject.transform.Find("OnBreakBtn/Checkmark");
				Transform transform3 = gameObject.transform.Find("OnBreakBtn/Text");
				if (transform2 != null && transform2.gameObject.activeSelf)
				{
					num |= 1 << i + 1;
				}
				if (transform3 != null)
				{
					if (this.IsAllSymbolBreak(i))
					{
						transform3.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakAllItem"), i + 1);
					}
					else
					{
						transform3.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakSomeItem"), i + 1);
					}
				}
			}
			this.m_breakLevelMask = (ushort)num;
			int selectBreakSymbolCoinCnt = this.GetSelectBreakSymbolCoinCnt();
			Text component = form.transform.Find("Panel_SymbolBreak/Panel_Bottom/Pl_countText/symbolCoinCntText").GetComponent<Text>();
			component.text = selectBreakSymbolCoinCnt.ToString();
		}

		private void OnBreakExcessSymbolClickConfirm(CUIEvent uiEvent)
		{
			if (this.m_breakLevelMask == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Select_BreakLvl_Tip", true, 1.5f, null, new object[0]);
			}
			else
			{
				bool flag = this.SetBreakSymbolList();
				if (this.m_svrBreakSymbolList.Count > 0)
				{
					if (flag)
					{
						CSecurePwdSystem.TryToValidate(enOpPurpose.BREAK_SYMBOL, enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, default(stUIEventParams));
					}
					else
					{
						CSymbolMakeController.SendReqExcessSymbolBreak(this.m_svrBreakSymbolList, string.Empty);
					}
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Symbol_No_More_Item_Break", true, 1.5f, null, new object[0]);
				}
			}
		}

		private void OnSecurePwdBreakExcessSymbolConfirm(CUIEvent uiEvent)
		{
			if (this.m_breakLevelMask == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Select_BreakLvl_Tip", true, 1.5f, null, new object[0]);
			}
			else if (this.m_svrBreakSymbolList.Count > 0)
			{
				CSymbolMakeController.SendReqExcessSymbolBreak(this.m_svrBreakSymbolList, uiEvent.m_eventParams.pwd);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Symbol_No_More_Item_Break", true, 1.5f, null, new object[0]);
			}
		}

		private void OnItemBreakAnimatorEnd(CUIEvent uiEvent)
		{
			if (CSymbolMakeController.s_breakSymbolCoinCnt > 0)
			{
				CUseable cUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, CSymbolMakeController.s_breakSymbolCoinCnt);
				if (cUseable != null)
				{
					CUseable[] items = new CUseable[]
					{
						cUseable
					};
					Singleton<CUIManager>.GetInstance().OpenAwardTip(items, null, false, enUIEventID.None, false, false, "Form_Award");
				}
				CSymbolMakeController.s_breakSymbolCoinCnt = 0;
			}
		}

		private void PlaySingleBreakAnimator()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolTransformPath);
			if (null == form)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("Panel_SymbolTranform/Panel_Content/iconImage").gameObject;
			CUICommonSystem.PlayAnimator(gameObject, "FenjieAnimation");
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_dange", null);
		}

		private void PlayBatchBreakAnimator()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolBreakPath);
			if (null == form)
			{
				return;
			}
			Transform transform = form.transform.Find("Panel_SymbolBreak/Panel_Content");
			for (int i = 0; i < CSymbolMakeController.s_maxSymbolBreakLevel; i++)
			{
				GameObject gameObject = transform.Find(string.Format("breakElement{0}", i)).gameObject;
				Transform transform2 = gameObject.transform.Find("OnBreakBtn/Checkmark");
				if (transform2 != null && transform2.gameObject.activeSelf)
				{
					GameObject gameObject2 = gameObject.transform.Find("Img_Lv").gameObject;
					CUICommonSystem.PlayAnimator(gameObject2, "FenjieAnimation");
				}
			}
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_piliang", null);
		}

		private void OnGetNewSymbol()
		{
			this.RefreshSymbolMakeForm();
		}

		public static void SendReqSymbolMake(uint heroId, uint symbolId, int cnt)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1155u);
			cSPkg.stPkgData.stSymbolMake.dwBelongHeroID = heroId;
			cSPkg.stPkgData.stSymbolMake.stSymbolInfo.dwSymbolID = symbolId;
			cSPkg.stPkgData.stSymbolMake.stSymbolInfo.iSymbolCnt = cnt;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendReqSymbolBreak(uint heroId, uint symbolId, int cnt, string pwd = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1156u);
			cSPkg.stPkgData.stSymbolBreak.dwBelongHeroID = heroId;
			cSPkg.stPkgData.stSymbolBreak.wSymbolCnt = 1;
			cSPkg.stPkgData.stSymbolBreak.bBreakType = 0;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stSymbolBreak.szPswdInfo);
			CSDT_SYMBOLOPT_INFO cSDT_SYMBOLOPT_INFO = new CSDT_SYMBOLOPT_INFO();
			cSDT_SYMBOLOPT_INFO.dwSymbolID = symbolId;
			cSDT_SYMBOLOPT_INFO.iSymbolCnt = cnt;
			cSPkg.stPkgData.stSymbolBreak.astSymbolList[0] = cSDT_SYMBOLOPT_INFO;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void SendReqExcessSymbolBreak(ListView<CSDT_SYMBOLOPT_INFO> breakSymbolList, string pwd = "")
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1156u);
			cSPkg.stPkgData.stSymbolBreak.wSymbolCnt = (ushort)breakSymbolList.Count;
			cSPkg.stPkgData.stSymbolBreak.bBreakType = 1;
			StringHelper.StringToUTF8Bytes(pwd, ref cSPkg.stPkgData.stSymbolBreak.szPswdInfo);
			int num = 0;
			while (num < breakSymbolList.Count && num < 400)
			{
				cSPkg.stPkgData.stSymbolBreak.astSymbolList[num] = breakSymbolList[num];
				num++;
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1157)]
		public static void ReciveSymbolMakeRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_SYMBOL_MAKE stSymbolMakeRsp = msg.stPkgData.stSymbolMakeRsp;
			if (stSymbolMakeRsp.iResult == 0)
			{
				CUseable cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, stSymbolMakeRsp.stSymbolInfo.dwSymbolID, stSymbolMakeRsp.stSymbolInfo.iSymbolCnt);
				if (cUseable != null)
				{
					CSymbolItem cSymbolItem = (CSymbolItem)cUseable;
					if (cSymbolItem != null)
					{
						CUseableContainer cUseableContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
						cUseableContainer.Add(cUseable);
						CUICommonSystem.ShowSymbol(cUseableContainer, enUIEventID.None);
					}
				}
				Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolTransformForm();
				Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolMakeForm();
			}
		}

		[MessageHandler(1158)]
		public static void ReciveSymbolBreakRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_SYMBOL_BREAK stSymbolBreakRsp = msg.stPkgData.stSymbolBreakRsp;
			int num = 0;
			for (int i = 0; i < (int)stSymbolBreakRsp.wSymbolCnt; i++)
			{
				ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(stSymbolBreakRsp.astSymbolList[i].dwSymbolID);
				if (dataByKey != null)
				{
					num += (int)(dataByKey.dwBreakCoin * (uint)stSymbolBreakRsp.astSymbolList[i].iSymbolCnt);
				}
			}
			CSymbolMakeController.s_breakSymbolCoinCnt = num;
			if (num > 0)
			{
				if (stSymbolBreakRsp.bBreakType == 0)
				{
					Singleton<CSymbolMakeController>.GetInstance().PlaySingleBreakAnimator();
					Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolTransformForm();
				}
				else if (stSymbolBreakRsp.bBreakType == 1)
				{
					Singleton<CSymbolMakeController>.GetInstance().PlayBatchBreakAnimator();
					Singleton<CSymbolMakeController>.GetInstance().OnReceiveBreakItemList();
					Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolBreakForm();
				}
			}
		}
	}
}

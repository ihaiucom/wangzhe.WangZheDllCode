using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CSymbolSystem : Singleton<CSymbolSystem>
	{
		public static string s_symbolFormPath = "UGUI/Form/System/Symbol/Form_Symbol.prefab";

		public static string s_symbolPageFormPath = "UGUI/Form/System/Symbol/Form_SymbolPage.prefab";

		public static string s_symbolEquipModulePath = "UGUI/Form/System/Symbol/Panel_SymbolEquip.prefab";

		public static string s_symbolMakeModulePath = "UGUI/Form/System/Symbol/Panel_SymbolMake.prefab";

		public static string s_symbolMakeMallModulePath = "UGUI/Form/System/Symbol/Panel_SymbolMake_Shop.prefab";

		public static string s_symbolRecommendModulePath = "UGUI/Form/System/Symbol/Panel_SymbolRecommend.prefab";

		public static string s_symbolEquipPanel = "Panel_SymbolEquip";

		public static string s_symbolMakePanel = "Panel_SymbolMake";

		public static string s_symbolRecommendPanel = "Panel_SymbolRecommend";

		public enSymbolMenuType m_selectMenuType;

		private ListView<CSymbolItem> m_symbolList = new ListView<CSymbolItem>();

		public static int[] s_symbolPagePropArr = new int[37];

		public static int[] s_symbolPagePropPctArr = new int[37];

		public static int[] s_symbolPropValAddArr = new int[37];

		public static int[] s_symbolPropPctAddArr = new int[37];

		public CSymbolWearController m_symbolWearCtrl = new CSymbolWearController();

		public CSymbolRecommendController m_symbolRcmdCtrl = new CSymbolRecommendController();

		public override void Init()
		{
			this.m_symbolWearCtrl.Init(this);
			this.m_symbolRcmdCtrl.Init(this);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Jump_To_MiShu, new CUIEventManager.OnUIEventHandler(this.OnJumpToMishu));
			Singleton<EventRouter>.instance.AddEventHandler("MasterSymbolCoinChanged", new Action(this.OnSymbolCoinChanged));
		}

		public override void UnInit()
		{
			this.m_symbolWearCtrl.UnInit();
			this.m_symbolRcmdCtrl.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			Singleton<EventRouter>.instance.RemoveEventHandler("MasterSymbolCoinChanged", new Action(this.OnSymbolCoinChanged));
		}

		public void Clear()
		{
			this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
			this.m_symbolWearCtrl.Clear();
			this.m_symbolRcmdCtrl.Clear();
		}

		private void OnOpenSymbolForm(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
			this.OpenSymbolForm();
			Singleton<CLobbySystem>.GetInstance().OnCheckSymbolEquipAlert();
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_SymbolBtn);
		}

		private void OnOpenSymbolFormToMakeTab(CUIEvent uiEvent)
		{
			this.m_selectMenuType = enSymbolMenuType.SymbolMake;
			this.OpenSymbolForm();
		}

		private void OnJumpToMishu(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolMakeController.s_symbolTransformPath);
			stUIEventParams par = default(stUIEventParams);
			par.tag = 3;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Task_OpenForm, par);
		}

		private void OpenSymbolForm()
		{
			MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(14, true, false);
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolSystem.s_symbolFormPath, false, true);
			if (cUIFormScript != null)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Symbol");
				string text2 = Singleton<CTextManager>.GetInstance().GetText("SymbolRcmd_Tab");
				string text3 = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Make");
				string[] titleList = new string[]
				{
					text,
					text2,
					text3
				};
				GameObject widget = cUIFormScript.GetWidget(0);
				CUICommonSystem.InitMenuPanel(widget, titleList, (int)this.m_selectMenuType, true);
				CUIListScript component = widget.GetComponent<CUIListScript>();
				this.m_symbolWearCtrl.m_symbolPageOpenSrc = enSymbolPageOpenSrc.enSymbol;
			}
		}

		private void OnSymbolFormClose(CUIEvent uiEvent)
		{
			this.m_symbolRcmdCtrl.OnSymbolFormClose();
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_ItemInfoClose);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_CommonInfoClose);
		}

		private void OnUpdateSubModule(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = srcFormScript.GetWidget(0).GetComponent<CUIListScript>();
			srcFormScript.GetWidget(8).CustomSetActive(false);
			switch (this.m_selectMenuType)
			{
			case enSymbolMenuType.SymbolEquip:
				srcFormScript.GetWidget(5).CustomSetActive(true);
				this.m_symbolWearCtrl.SwitchToSymbolWearPanel(srcFormScript);
				break;
			case enSymbolMenuType.SymbolRecommend:
				srcFormScript.GetWidget(3).CustomSetActive(true);
				this.m_symbolRcmdCtrl.SwitchToSymbolRcmdPanel(srcFormScript);
				Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(7u, null, false);
				break;
			case enSymbolMenuType.SymbolMake:
				srcFormScript.GetWidget(4).CustomSetActive(true);
				Singleton<CSymbolMakeController>.GetInstance().SwitchToSymbolMakePanel(srcFormScript);
				break;
			}
		}

		private void OnMenuSelect(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (null == component)
			{
				return;
			}
			int selectedIndex = component.GetSelectedIndex();
			this.m_selectMenuType = (enSymbolMenuType)selectedIndex;
			this.LoadSubModule(srcFormScript, uiEvent);
		}

		public void LoadSubModule(CUIFormScript form, CUIEvent uiEvent = null)
		{
			bool flag = false;
			form.GetWidget(5).CustomSetActive(false);
			form.GetWidget(4).CustomSetActive(false);
			form.GetWidget(3).CustomSetActive(false);
			switch (this.m_selectMenuType)
			{
			case enSymbolMenuType.SymbolEquip:
				flag = this.m_symbolWearCtrl.Loaded(form);
				if (!flag)
				{
					form.GetWidget(8).CustomSetActive(true);
					this.m_symbolWearCtrl.Load(form);
				}
				break;
			case enSymbolMenuType.SymbolRecommend:
				flag = this.m_symbolRcmdCtrl.Loaded(form);
				if (!flag)
				{
					form.GetWidget(8).CustomSetActive(true);
					this.m_symbolRcmdCtrl.Load(form);
				}
				break;
			case enSymbolMenuType.SymbolMake:
				Singleton<CSymbolMakeController>.GetInstance().Source = enSymbolMakeSource.SymbolManage;
				flag = Singleton<CSymbolMakeController>.GetInstance().Loaded(form);
				if (!flag)
				{
					form.GetWidget(8).CustomSetActive(true);
					Singleton<CSymbolMakeController>.GetInstance().Load(form);
				}
				break;
			}
			uiEvent.m_srcFormScript.GetWidget(1).CustomSetActive(this.m_selectMenuType == enSymbolMenuType.SymbolEquip);
			uiEvent.m_srcFormScript.GetWidget(6).CustomSetActive(false);
			if (!flag)
			{
				GameObject widget = form.GetWidget(7);
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
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Symbol_Update_Sub_Module;
				cUIEvent.m_srcFormScript = form;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			}
		}

		public void RefreshSymbolForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
			if (form == null)
			{
				return;
			}
			if (this.m_selectMenuType == enSymbolMenuType.SymbolEquip)
			{
				this.m_symbolWearCtrl.RefreshSymbolEquipPanel();
			}
			else if (this.m_selectMenuType == enSymbolMenuType.SymbolMake)
			{
				Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolMakeForm();
			}
			else if (this.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
			{
				this.m_symbolRcmdCtrl.RefreshSymbolRcmdPanel();
			}
		}

		public static void RefreshSymbolItem(ResSymbolInfo symbolInfo, GameObject widget, CUIFormScript form, enSymbolMakeSource source = enSymbolMakeSource.SymbolManage)
		{
			if (symbolInfo == null || widget == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
			CUseable useableByBaseID = useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID);
			Image component = widget.transform.Find("iconImage").GetComponent<Image>();
			Text component2 = widget.transform.Find("countText").GetComponent<Text>();
			Text component3 = widget.transform.Find("nameText").GetComponent<Text>();
			Text component4 = widget.transform.Find("descText").GetComponent<Text>();
			Transform transform = widget.transform.Find("Price");
			if (transform != null)
			{
				Text componetInChild = Utility.GetComponetInChild<Text>(transform.gameObject, "Text");
				if (componetInChild != null)
				{
					componetInChild.text = symbolInfo.dwMakeCoin.ToString();
				}
				CUIEventScript component5 = transform.GetComponent<CUIEventScript>();
				if (component5 != null)
				{
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.symbolTransParam.symbolCfgInfo = symbolInfo;
					component5.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_ListItemClick, eventParams);
				}
			}
			component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, symbolInfo.dwIcon), form, true, false, false, false);
			component2.text = ((useableByBaseID == null) ? "0" : useableByBaseID.m_stackCount.ToString());
			component3.text = symbolInfo.szName;
			component4.text = CSymbolSystem.GetSymbolAttString(symbolInfo.dwID, true);
			CUIEventScript component6 = widget.GetComponent<CUIEventScript>();
			if (component6 != null)
			{
				stUIEventParams eventParams2 = default(stUIEventParams);
				eventParams2.symbolTransParam.symbolCfgInfo = symbolInfo;
				component6.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_ListItemClick, eventParams2);
			}
			if (source == enSymbolMakeSource.Mall)
			{
				CUICommonSystem.PlayAnimator(widget, "Symbol_Normal");
			}
			else if (useableByBaseID != null)
			{
				CUICommonSystem.PlayAnimator(widget, "Symbol_Normal");
			}
			else
			{
				CUICommonSystem.PlayAnimator(widget, "Symbol_Disabled");
			}
		}

		private void OnSymbolCoinChanged()
		{
			CSymbolSystem.RefreshSymbolCntText(false);
		}

		public static void RefreshSymbolCntText(bool forceShow = false)
		{
			GameObject gameObject = null;
			enSymbolMakeSource source = Singleton<CSymbolMakeController>.GetInstance().Source;
			if (source != enSymbolMakeSource.SymbolManage)
			{
				if (source == enSymbolMakeSource.Mall)
				{
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
					if (form == null)
					{
						return;
					}
					gameObject = form.GetWidget(13);
				}
			}
			else
			{
				CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
				if (form2 == null)
				{
					return;
				}
				gameObject = form2.GetWidget(6);
			}
			if (gameObject == null)
			{
				return;
			}
			Text component = gameObject.transform.Find("symbolCoinCntText").GetComponent<Text>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (component != null)
			{
				if (masterRoleInfo != null)
				{
					component.text = masterRoleInfo.SymbolCoin.ToString();
				}
				else
				{
					component.text = string.Empty;
				}
			}
			if (forceShow)
			{
				gameObject.CustomSetActive(true);
			}
		}

		public void SetSymbolData()
		{
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			int curUseableCount = useableContainer.GetCurUseableCount();
			this.m_symbolList.Clear();
			for (int i = 0; i < curUseableCount; i++)
			{
				CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
				if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					CSymbolItem cSymbolItem = (CSymbolItem)useableByIndex;
					if (cSymbolItem != null)
					{
						this.m_symbolList.Add(cSymbolItem);
					}
				}
			}
		}

		public ListView<CSymbolItem> GetAllSymbolList()
		{
			return this.m_symbolList;
		}

		public CSymbolItem GetSymbolByObjID(ulong objID)
		{
			CSymbolItem result = null;
			for (int i = 0; i < this.m_symbolList.Count; i++)
			{
				CSymbolItem cSymbolItem = this.m_symbolList[i];
				if (cSymbolItem != null && cSymbolItem.m_objID == objID)
				{
					result = cSymbolItem;
					break;
				}
			}
			return result;
		}

		public static void RefreshSymbolPageProp(GameObject propListPanel, int pageIndex, bool bPvp = true)
		{
			if (propListPanel == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref CSymbolSystem.s_symbolPagePropArr, ref CSymbolSystem.s_symbolPagePropPctArr, bPvp);
			CSymbolSystem.RefreshPropPanel(propListPanel, ref CSymbolSystem.s_symbolPagePropArr, ref CSymbolSystem.s_symbolPagePropPctArr);
		}

		public static void RefreshSymbolPagePveEnhanceProp(GameObject propList, int pageIndex)
		{
			if (propList == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref CSymbolSystem.s_symbolPagePropArr, ref CSymbolSystem.s_symbolPagePropPctArr, true);
			masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref CSymbolSystem.s_symbolPropValAddArr, ref CSymbolSystem.s_symbolPropPctAddArr, false);
			int num = 37;
			for (int i = 0; i < num; i++)
			{
				CSymbolSystem.s_symbolPropValAddArr[i] = CSymbolSystem.s_symbolPropValAddArr[i] - CSymbolSystem.s_symbolPagePropArr[i];
				CSymbolSystem.s_symbolPropPctAddArr[i] = CSymbolSystem.s_symbolPropPctAddArr[i] - CSymbolSystem.s_symbolPagePropPctArr[i];
			}
			CSymbolSystem.RefreshPropPanel(propList, ref CSymbolSystem.s_symbolPropValAddArr, ref CSymbolSystem.s_symbolPropPctAddArr);
		}

		public static void RefreshPropPanel(GameObject propPanel, ref int[] propArr, ref int[] propPctArr)
		{
			int num = 37;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0)
				{
					num2++;
				}
			}
			CUIListScript component = propPanel.GetComponent<CUIListScript>();
			component.SetElementAmount(num2);
			num2 = 0;
			for (int j = 0; j < num; j++)
			{
				if (propArr[j] > 0 || propPctArr[j] > 0)
				{
					CUIListElementScript elemenet = component.GetElemenet(num2);
					DebugHelper.Assert(elemenet != null);
					if (elemenet != null)
					{
						Text component2 = elemenet.gameObject.transform.Find("titleText").GetComponent<Text>();
						Text component3 = elemenet.gameObject.transform.Find("valueText").GetComponent<Text>();
						DebugHelper.Assert(component2 != null);
						if (component2 != null)
						{
							component2.text = CUICommonSystem.s_attNameList[j];
						}
						DebugHelper.Assert(component3 != null);
						if (component3 != null)
						{
							if (propArr[j] > 0)
							{
								if (CUICommonSystem.s_pctFuncEftList.IndexOf((uint)j) != -1)
								{
									component3.text = string.Format("+{0}", CUICommonSystem.GetValuePercent(propArr[j] / 100));
								}
								else
								{
									component3.text = string.Format("+{0}", (float)propArr[j] / 100f);
								}
							}
							else if (propPctArr[j] > 0)
							{
								component3.text = string.Format("+{0}", CUICommonSystem.GetValuePercent(propPctArr[j]));
							}
						}
					}
					num2++;
				}
			}
		}

		public static string GetSymbolAttString(CSymbolItem symbol, bool bPvp = true)
		{
			if (bPvp)
			{
				return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astFuncEftList, true);
			}
			return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astPveEftList, true);
		}

		public static string GetSymbolAttString(uint cfgId, bool bPvp = true)
		{
			ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(cfgId);
			if (dataByKey == null)
			{
				return string.Empty;
			}
			if (bPvp)
			{
				return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astFuncEftList, true);
			}
			return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astPveEftList, true);
		}

		public static void RefreshSymbolPropContent(GameObject propPanel, uint symbolId)
		{
			ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
			if (propPanel == null || dataByKey == null)
			{
				return;
			}
			CSymbolSystem.GetSymbolProp(symbolId, ref CSymbolSystem.s_symbolPagePropArr, ref CSymbolSystem.s_symbolPagePropPctArr, true);
			CSymbolSystem.GetSymbolProp(symbolId, ref CSymbolSystem.s_symbolPropValAddArr, ref CSymbolSystem.s_symbolPropPctAddArr, false);
			int num = 37;
			for (int i = 0; i < num; i++)
			{
				CSymbolSystem.s_symbolPropValAddArr[i] = CSymbolSystem.s_symbolPropValAddArr[i] - CSymbolSystem.s_symbolPagePropArr[i];
				CSymbolSystem.s_symbolPropPctAddArr[i] = CSymbolSystem.s_symbolPropPctAddArr[i] - CSymbolSystem.s_symbolPagePropPctArr[i];
			}
			Transform transform = propPanel.transform.Find("pvpPropPanel");
			GameObject gameObject = transform.Find("propList").gameObject;
			CSymbolSystem.RefreshPropPanel(gameObject, ref CSymbolSystem.s_symbolPagePropArr, ref CSymbolSystem.s_symbolPagePropPctArr);
			Transform transform2 = propPanel.transform.Find("pveEnhancePropPanel");
			GameObject gameObject2 = transform2.Find("propList").gameObject;
			CSymbolSystem.RefreshPropPanel(gameObject2, ref CSymbolSystem.s_symbolPropValAddArr, ref CSymbolSystem.s_symbolPropPctAddArr);
		}

		public static void GetSymbolProp(uint symbolId, ref int[] propArr, ref int[] propPctArr, bool bPvp)
		{
			int num = 37;
			for (int i = 0; i < num; i++)
			{
				propArr[i] = 0;
				propPctArr[i] = 0;
			}
			ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
			if (dataByKey == null)
			{
				return;
			}
			ResDT_FuncEft_Obj[] array = (!bPvp) ? dataByKey.astPveEftList : dataByKey.astFuncEftList;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].wType != 0 && array[j].wType < 37 && array[j].iValue != 0)
				{
					if (array[j].bValType == 0)
					{
						propArr[(int)array[j].wType] += array[j].iValue;
					}
					else if (array[j].bValType == 1)
					{
						propPctArr[(int)array[j].wType] += array[j].iValue;
					}
				}
			}
		}

		public int GetSymbolListIndex(uint symbolCfgId)
		{
			return this.m_symbolWearCtrl.GetSymbolListIndex(symbolCfgId);
		}

		public static void SendQuerySymbol()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1132u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1133)]
		public static void ReciveSymbolDatail(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_SYMBOLDETAIL stSymbolDetail = msg.stPkgData.stSymbolDetail;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.m_symbolInfo.SetData(stSymbolDetail.stSymbolInfo);
				Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
			}
			else
			{
				DebugHelper.Assert(false, "ReciveSymbolDatail Master RoleInfo is NULL!!!");
			}
		}

		[MessageHandler(1186)]
		public static void NtfSymbolPageSyn(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.m_symbolInfo.SetSymbolPageCount((int)msg.stPkgData.stSymbolPageSyn.bSymbolPageCnt);
				Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
			}
			else
			{
				DebugHelper.Assert(false, "NtfSymbolPageSyn Master RoleInfo is NULL!!!");
			}
		}
	}
}

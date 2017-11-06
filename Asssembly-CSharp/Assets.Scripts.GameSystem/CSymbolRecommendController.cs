using Assets.Scripts.Framework;
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
	public class CSymbolRecommendController
	{
		private static string s_ChooseHeroPath = "UGUI/Form/System/Symbol/Form_ChooseHero.prefab";

		private DictionaryView<uint, List<uint>[]> m_cfgHeroRcmdSymbolList = new DictionaryView<uint, List<uint>[]>();

		public uint m_curHeroId;

		private ushort m_symbolRcmdLevel = 1;

		private CSymbolSystem m_symbolSys;

		private List<uint>[] m_rcmdSymbolList = new List<uint>[CSymbolInfo.s_maxSymbolLevel];

		private enHeroJobType m_curHeroJob;

		private bool m_bOwnHeroFlag = true;

		private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();

		public void Init(CSymbolSystem symbolSys)
		{
			this.m_symbolSys = symbolSys;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_OpenChangeHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChangeHeroForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_LevelListIndexChange, new CUIEventManager.OnUIEventHandler(this.OnSymbolRcmdLevelChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroTypeChange, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroOwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_RcmdListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRcmdListItemEnable));
		}

		public void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_OpenChangeHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChangeHeroForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_LevelListIndexChange, new CUIEventManager.OnUIEventHandler(this.OnSymbolRcmdLevelChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroTypeChange, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroOwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_RcmdListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRcmdListItemEnable));
		}

		public void Clear()
		{
		}

		private void GetHeroRcmdSymbolList()
		{
			if (this.m_cfgHeroRcmdSymbolList.TryGetValue(this.m_curHeroId, out this.m_rcmdSymbolList))
			{
				return;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_curHeroId);
			if (dataByKey == null)
			{
				DebugHelper.Assert(false, "GetHeroRcmdSymbolList heroCfgInfo is null heroId = " + this.m_curHeroId);
				return;
			}
			List<uint>[] array = new List<uint>[CSymbolInfo.s_maxSymbolLevel];
			HashSet<object> dataByKey2 = GameDataMgr.symbolRcmdDatabin.GetDataByKey(dataByKey.dwSymbolRcmdID);
			HashSet<object>.Enumerator enumerator = dataByKey2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ResSymbolRcmd resSymbolRcmd = (ResSymbolRcmd)enumerator.get_Current();
				if (array[(int)(resSymbolRcmd.wSymbolLvl - 1)] == null)
				{
					array[(int)(resSymbolRcmd.wSymbolLvl - 1)] = new List<uint>();
				}
				for (int i = 0; i < resSymbolRcmd.SymbolID.Length; i++)
				{
					if (resSymbolRcmd.SymbolID[i] > 0u)
					{
						array[(int)(resSymbolRcmd.wSymbolLvl - 1)].Add(resSymbolRcmd.SymbolID[i]);
					}
				}
			}
			this.m_cfgHeroRcmdSymbolList.Add(this.m_curHeroId, array);
			this.m_rcmdSymbolList = array;
		}

		public void OnSymbolFormClose()
		{
			if (this.m_curHeroId != 0u)
			{
				CSymbolRecommendController.SendReqSelectHeroLvl(this.m_curHeroId, this.m_symbolRcmdLevel);
			}
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab(CSymbolSystem.s_symbolRecommendModulePath, CSymbolSystem.s_symbolRecommendPanel, form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), CSymbolSystem.s_symbolRecommendPanel);
			return !(x == null);
		}

		public void SwitchToSymbolRcmdPanel(CUIFormScript form)
		{
			if (form == null || form.IsClosed())
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "SwitchToSymbolRcmdPanel role is null");
				return;
			}
			this.m_curHeroId = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId;
			this.m_symbolRcmdLevel = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdLevel;
			this.m_symbolRcmdLevel = (ushort)Math.Max(1, Math.Min((int)this.m_symbolRcmdLevel, CSymbolInfo.s_maxSymbolLevel));
			CSymbolSystem.RefreshSymbolCntText(true);
			this.RefreshSymbolRcmdPanel();
			string[] titleList = new string[]
			{
				"1",
				"2",
				"3",
				"4",
				"5"
			};
			GameObject listObj = Utility.FindChild(form.gameObject, "SymbolRecommend/Panel_SymbolRecommend/Panel_SymbolLevel/levelList");
			CUICommonSystem.InitMenuPanel(listObj, titleList, (int)(this.m_symbolRcmdLevel - 1), true);
		}

		private void RefreshHeroImage()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
			if (null == form)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "RefreshHeroImage role is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "SymbolRecommend/Panel_SymbolRecommend/heroItemCell");
			ResHeroCfgInfo dataByKey;
			if (masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId == 0u)
			{
				dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.GetFirstHeroId());
			}
			else
			{
				dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId);
			}
			if (dataByKey == null)
			{
				return;
			}
			masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = dataByKey.dwCfgID;
			this.m_curHeroId = dataByKey.dwCfgID;
			CUICommonSystem.SetHeroItemImage(form, gameObject.gameObject, dataByKey.szImagePath, enHeroHeadType.enIcon, false, false);
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "SymbolRecommend/Panel_SymbolRecommend/heroNameText");
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<Text>().set_text(dataByKey.szName);
			}
		}

		public void RefreshSymbolRcmdPanel()
		{
			this.m_symbolSys.SetSymbolData();
			this.RefreshHeroImage();
			this.RefreshRcmdSymbolListPanel();
		}

		private void OnSymbolRcmdLevelChange(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			this.m_symbolRcmdLevel = (ushort)(component.GetSelectedIndex() + 1);
			this.RefreshRcmdSymbolListPanel();
		}

		private void RefreshRcmdSymbolListPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
			if (null == form)
			{
				return;
			}
			this.GetHeroRcmdSymbolList();
			GameObject gameObject = Utility.FindChild(form.gameObject, "SymbolRecommend/Panel_SymbolRecommend/symbolRcmdList");
			if (gameObject != null && this.m_rcmdSymbolList[(int)(this.m_symbolRcmdLevel - 1)] != null)
			{
				gameObject.GetComponent<CUIListScript>().SetElementAmount(this.m_rcmdSymbolList[(int)(this.m_symbolRcmdLevel - 1)].get_Count());
				Transform transform = gameObject.transform.Find("Text");
				if (transform != null)
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_curHeroId);
					transform.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("SymbolRcmd_HeroLvlText"), dataByKey.szName, this.m_symbolRcmdLevel));
				}
			}
		}

		private void OnRcmdListItemEnable(CUIEvent uiEvent)
		{
			if (this.m_rcmdSymbolList == null || this.m_rcmdSymbolList[(int)(this.m_symbolRcmdLevel - 1)] == null)
			{
				return;
			}
			if (uiEvent.m_srcWidgetIndexInBelongedList < 0 || uiEvent.m_srcWidgetIndexInBelongedList >= this.m_rcmdSymbolList[(int)(this.m_symbolRcmdLevel - 1)].get_Count())
			{
				DebugHelper.Assert(false, "OnRcmdListItemEnable index out of range");
				return;
			}
			ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(this.m_rcmdSymbolList[(int)(this.m_symbolRcmdLevel - 1)].get_Item(uiEvent.m_srcWidgetIndexInBelongedList));
			CSymbolSystem.RefreshSymbolItem(dataByKey, uiEvent.m_srcWidget, uiEvent.m_srcFormScript, enSymbolMakeSource.SymbolManage);
		}

		private void OnOpenChangeHeroForm(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSymbolRecommendController.s_ChooseHeroPath, false, true);
			if (null == cUIFormScript)
			{
				return;
			}
			this.m_curHeroJob = enHeroJobType.All;
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget != null)
			{
				GameObject widget2 = cUIFormScript.GetWidget(1);
				if (widget2 != null)
				{
					widget2.GetComponent<Toggle>().set_isOn(this.m_bOwnHeroFlag);
				}
				string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
				string text3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
				string text5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
				string text6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
				string text7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
				string[] titleList = new string[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6,
					text7
				};
				CUICommonSystem.InitMenuPanel(widget, titleList, (int)this.m_curHeroJob, true);
			}
		}

		private void ResetHeroList(enHeroJobType jobType, bool bOwn)
		{
			this.m_heroList.Clear();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "CSymbolRcommendController ResetHeroList role is null");
			if (masterRoleInfo != null)
			{
				ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
				for (int i = 0; i < allHeroList.Count; i++)
				{
					if ((jobType == enHeroJobType.All || allHeroList[i].bMainJob == (byte)jobType || allHeroList[i].bMinorJob == (byte)jobType) && (!bOwn || masterRoleInfo.IsHaveHero(allHeroList[i].dwCfgID, false)))
					{
						this.m_heroList.Add(allHeroList[i]);
					}
				}
			}
		}

		private void RefreshHeroListPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolRecommendController.s_ChooseHeroPath);
			if (null == form)
			{
				return;
			}
			GameObject widget = form.GetWidget(1);
			if (widget != null)
			{
				this.m_bOwnHeroFlag = widget.GetComponent<Toggle>().get_isOn();
			}
			this.ResetHeroList(this.m_curHeroJob, this.m_bOwnHeroFlag);
			GameObject widget2 = form.GetWidget(2);
			if (widget2 != null)
			{
				CUIListScript component = widget2.GetComponent<CUIListScript>();
				component.SetElementAmount(this.m_heroList.Count);
			}
		}

		private void OnHeroOwnFlagChange(CUIEvent uiEvent)
		{
			this.RefreshHeroListPanel();
		}

		private void OnHeroTypeListSelect(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = (CUIListScript)uiEvent.m_srcWidgetScript;
			if (cUIListScript != null)
			{
				this.m_curHeroJob = (enHeroJobType)cUIListScript.GetSelectedIndex();
				this.RefreshHeroListPanel();
			}
		}

		private void OnHeroListElementEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_heroList.Count)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "CSymbolRcmdCtrl OnHeroListElementEnable role is null");
			if (masterRoleInfo == null)
			{
				return;
			}
			ResHeroCfgInfo resHeroCfgInfo = this.m_heroList[srcWidgetIndexInBelongedList];
			if (resHeroCfgInfo != null && uiEvent.m_srcWidget != null)
			{
				Transform transform = uiEvent.m_srcWidget.transform.Find("heroItemCell");
				if (transform != null)
				{
					CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, transform.gameObject, resHeroCfgInfo.szImagePath, enHeroHeadType.enIcon, !masterRoleInfo.IsHaveHero(resHeroCfgInfo.dwCfgID, false), false);
					CUIEventScript component = transform.GetComponent<CUIEventScript>();
					if (component != null)
					{
						component.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolRcmd_HeroListItemClick, new stUIEventParams
						{
							heroId = resHeroCfgInfo.dwCfgID
						});
					}
					Transform transform2 = transform.Find("TxtFree");
					if (transform2 != null)
					{
						transform2.gameObject.CustomSetActive(masterRoleInfo.IsFreeHero(resHeroCfgInfo.dwCfgID));
					}
				}
			}
		}

		private void OnHeroListElementClick(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSymbolRecommendController.s_ChooseHeroPath);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = uiEvent.m_eventParams.heroId;
				this.m_curHeroId = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId;
				this.RefreshSymbolRcmdPanel();
				CSymbolRecommendController.SendReqSelectHeroLvl(this.m_curHeroId, this.m_symbolRcmdLevel);
			}
		}

		public static void SendReqSelectHeroLvl(uint heroId, ushort level)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1182u);
			cSPkg.stPkgData.stSymbolRcmdSelReq.dwHeroID = heroId;
			cSPkg.stPkgData.stSymbolRcmdSelReq.wSymbolLvl = level;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void SendReqWearRcmdSymbol(int pageIndex)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1180u);
			cSPkg.stPkgData.stSymbolRcmdWearReq.bSymbolPageIdx = (byte)pageIndex;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1183)]
		public static void ReciveSymbolRcmdHeroLvl(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_SYMBOLRCMD_SEL stSymbolRcmdSelRsp = msg.stPkgData.stSymbolRcmdSelRsp;
			if (stSymbolRcmdSelRsp.iResult == 0)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null && masterRoleInfo.m_symbolInfo != null)
				{
					masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = stSymbolRcmdSelRsp.dwHeroID;
					masterRoleInfo.m_symbolInfo.m_selSymbolRcmdLevel = stSymbolRcmdSelRsp.wSymbolLvl;
				}
			}
		}

		[MessageHandler(1181)]
		public static void ReciveSymbolRcmdWearRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_SYMBOLRCMD_WEAR stSymbolRcmdWearRsp = msg.stPkgData.stSymbolRcmdWearRsp;
		}
	}
}

using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InBattleShortcutMenu
{
	public enum EItemState
	{
		None,
		Normal,
		Delete,
		Record
	}

	public static readonly string InBattleShortcutMenu_FORM_PATH = "UGUI/Form/System/Chat/Form_ShortcutChat.prefab";

	private CUIFormScript m_form;

	private int m_rightTabIndex = -1;

	private CUIListScript m_rightTablist;

	private CUIListScript m_rightContentList;

	private CUIListScript m_leftContentList;

	private GameObject ReviseBtn;

	private GameObject CancelBtn;

	private GameObject SureBtn;

	private bool m_bEdit;

	private TabElement curSelecTabElement;

	public bool EditMode
	{
		get
		{
			return this.m_bEdit;
		}
		set
		{
			this.m_bEdit = value;
			if (this.m_bEdit)
			{
				if (this.ReviseBtn != null)
				{
					this.ReviseBtn.CustomSetActive(false);
				}
				if (this.CancelBtn != null)
				{
					this.CancelBtn.CustomSetActive(true);
				}
				if (this.SureBtn != null)
				{
					this.SureBtn.CustomSetActive(true);
				}
			}
			else
			{
				if (this.ReviseBtn != null)
				{
					this.ReviseBtn.CustomSetActive(true);
				}
				if (this.CancelBtn != null)
				{
					this.CancelBtn.CustomSetActive(false);
				}
				if (this.SureBtn != null)
				{
					this.SureBtn.CustomSetActive(false);
				}
			}
			this.RefreshLeft();
		}
	}

	public int TabIndex
	{
		get
		{
			return this.m_rightTabIndex;
		}
		set
		{
			if (this.m_rightTabIndex == value)
			{
				return;
			}
			this.m_rightTabIndex = value;
			this.Refresh_Right_List(this.m_rightTabIndex);
		}
	}

	public void OpenForm()
	{
		this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(InBattleShortcutMenu.InBattleShortcutMenu_FORM_PATH, false, true);
		DebugHelper.Assert(this.m_form != null, "InBattleShortcutMenu m_form == null");
		if (this.m_form == null)
		{
			return;
		}
		this.RegEvent();
		this.m_rightTablist = this.m_form.transform.Find("Panel_Main/Panel_Right/Menu").GetComponent<CUIListScript>();
		this.m_rightContentList = this.m_form.transform.Find("Panel_Main/Panel_Right/ShortcutList").GetComponent<CUIListScript>();
		DebugHelper.Assert(this.m_rightTablist != null, "InBattleShortcutMenu m_rightTablist == null");
		DebugHelper.Assert(this.m_rightContentList != null, "InBattleShortcutMenu m_rightContentList == null");
		if (this.m_rightTablist == null)
		{
			return;
		}
		if (this.m_rightContentList == null)
		{
			return;
		}
		UT.SetTabList(Singleton<InBattleMsgMgr>.instance.title_list, 0, this.m_rightTablist);
		this.m_leftContentList = this.m_form.transform.Find("Panel_Main/Panel_Left/SelectedList").GetComponent<CUIListScript>();
		this.ReviseBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/ReviseBtn").gameObject;
		this.CancelBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/CancelBtn").gameObject;
		this.SureBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/SureBtn").gameObject;
		DebugHelper.Assert(this.m_leftContentList != null, "InBattleShortcutMenu m_leftContentList == null");
		DebugHelper.Assert(this.ReviseBtn != null, "InBattleShortcutMenu ReviseBtn == null");
		DebugHelper.Assert(this.CancelBtn != null, "InBattleShortcutMenu CancelBtn == null");
		DebugHelper.Assert(this.SureBtn != null, "InBattleShortcutMenu SureBtn == null");
		this.SetBtnNormal();
		this.RefreshLeft();
	}

	public void Clear()
	{
		this.UnRegEvent();
		this.m_bEdit = false;
		this.m_rightTabIndex = -1;
		this.curSelecTabElement = null;
		this.m_rightTablist = null;
		this.m_rightContentList = null;
		this.m_leftContentList = null;
		this.ReviseBtn = null;
		this.CancelBtn = null;
		this.SureBtn = null;
		this.m_form = null;
	}

	public void RegEvent()
	{
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_CloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_LeftItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_LeftItem_Enable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Delete, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Delete));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Record, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Record));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_UseDefault, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Change));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_OK, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_OK));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Cancle, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Cancle));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightTab_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightTab_Change));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Enable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightItem_Click, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Click));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_UseDefault_Ok, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault_Ok));
	}

	public void UnRegEvent()
	{
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_CloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_LeftItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_LeftItem_Enable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Delete, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Delete));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Record, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Record));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_UseDefault, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Change));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_OK, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_OK));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Cancle, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Cancle));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightTab_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightTab_Change));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Enable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightItem_Click, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Click));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_UseDefault_Ok, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault_Ok));
	}

	private void On_InBatShortcut_RightItem_Click(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		TabElement tabElement = Singleton<InBattleMsgMgr>.instance.GeTabElement(this.TabIndex, srcWidgetIndexInBelongedList);
		if (tabElement == null)
		{
			return;
		}
		this.curSelecTabElement = tabElement;
	}

	private void On_InBatShortcut_RightItem_Enable(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		TabElement tabElement = Singleton<InBattleMsgMgr>.instance.GeTabElement(this.TabIndex, srcWidgetIndexInBelongedList);
		InBattleMsgShower component = uievent.m_srcWidget.GetComponent<InBattleMsgShower>();
		if (component != null && tabElement != null)
		{
			string text = tabElement.configContent;
			if (tabElement.camp == 2)
			{
				text = "[全部] " + text;
			}
			component.Set(tabElement.cfgId, text);
		}
	}

	private void On_InBatShortcut_RightTab_Change(CUIEvent uievent)
	{
		int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
		this.TabIndex = selectedIndex;
	}

	private void On_InBatShortcut_Cancle(CUIEvent uievent)
	{
		InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
		instance.SyncData(instance.lastMenuEntList, instance.menuEntList);
		this.EditMode = false;
		this.RefreshLeft();
	}

	private void On_InBatShortcut_OK(CUIEvent uievent)
	{
		InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
		instance.SyncData(instance.menuEntList, instance.lastMenuEntList);
		InBattleMsgNetCore.SendShortCut_Config(instance.menuEntList);
		this.EditMode = false;
	}

	private void On_InBatShortcut_Change(CUIEvent uievent)
	{
		this.EditMode = true;
	}

	private void On_InBatShortcut_UseDefault(CUIEvent uievent)
	{
		Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("InBattleShortcutMenu_UseDefault"), enUIEventID.InBatShortcut_UseDefault_Ok, enUIEventID.None, false);
	}

	private void On_InBatShortcut_UseDefault_Ok(CUIEvent uievent)
	{
		Singleton<CUIManager>.instance.OpenTips("InBattleShortcutMenu_UseDefault_OK", true, 1.5f, null, new object[0]);
		Singleton<InBattleMsgMgr>.instance.UseDefaultShortcut();
		this.RefreshLeft();
	}

	private void On_InBatShortcut_Record(CUIEvent uievent)
	{
		if (this.curSelecTabElement == null)
		{
			Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("InBattleShortcutMenu_SelectRight"), false, 1.5f, null, new object[0]);
			return;
		}
		CUIListElementScript component = uievent.m_srcWidget.transform.parent.parent.GetComponent<CUIListElementScript>();
		if (component != null)
		{
			if (component.m_indexInlist < 0 || component.m_indexInlist >= Singleton<InBattleMsgMgr>.instance.menuEntList.Count)
			{
				return;
			}
			TabElement tabElement = Singleton<InBattleMsgMgr>.instance.menuEntList[component.m_indexInlist];
			if (tabElement == null)
			{
				return;
			}
			tabElement.cfgId = this.curSelecTabElement.cfgId;
			tabElement.configContent = this.curSelecTabElement.configContent;
			this.RefreshLeft();
		}
	}

	private void On_InBatShortcut_Delete(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		ListView<TabElement> menuEntList = Singleton<InBattleMsgMgr>.instance.menuEntList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= menuEntList.Count)
		{
			return;
		}
		TabElement tabElement = menuEntList[srcWidgetIndexInBelongedList];
		tabElement.cfgId = 0u;
		tabElement.configContent = string.Empty;
		this.SetLeftItemState(uievent.m_srcWidget.transform.parent.parent.gameObject, tabElement, InBattleShortcutMenu.EItemState.Record);
	}

	private void On_InBatShortcut_LeftItem_Enable(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		ListView<TabElement> menuEntList = Singleton<InBattleMsgMgr>.instance.menuEntList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= menuEntList.Count)
		{
			return;
		}
		TabElement tabElement = menuEntList[srcWidgetIndexInBelongedList];
		if (tabElement == null)
		{
			return;
		}
		if (!this.EditMode)
		{
			this.SetLeftItemState(uievent.m_srcWidget, tabElement, InBattleShortcutMenu.EItemState.Normal);
		}
		else if (tabElement.cfgId == 0u)
		{
			this.SetLeftItemState(uievent.m_srcWidget, tabElement, InBattleShortcutMenu.EItemState.Record);
		}
		else
		{
			this.SetLeftItemState(uievent.m_srcWidget, tabElement, InBattleShortcutMenu.EItemState.Delete);
		}
	}

	private void On_InBatShortcut_CloseForm(CUIEvent uievent)
	{
		this.On_InBatShortcut_Cancle(null);
		this.Clear();
	}

	private void SetLeftItemState(GameObject node, TabElement data, InBattleShortcutMenu.EItemState state)
	{
		if (data == null)
		{
			return;
		}
		Text component = node.transform.Find("titleText").GetComponent<Text>();
		GameObject gameObject = node.transform.Find("BtnGroup").gameObject;
		if (state == InBattleShortcutMenu.EItemState.Normal)
		{
			component.gameObject.CustomSetActive(true);
			component.set_text(data.configContent);
			gameObject.CustomSetActive(false);
		}
		else if (state == InBattleShortcutMenu.EItemState.Delete)
		{
			gameObject.CustomSetActive(true);
			component.gameObject.CustomSetActive(true);
			component.set_text(data.configContent);
			GameObject gameObject2 = gameObject.transform.Find("WriteBtn").gameObject;
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(false);
			}
			GameObject gameObject3 = gameObject.transform.Find("RemoveBtn").gameObject;
			if (gameObject3 != null)
			{
				gameObject3.CustomSetActive(true);
			}
		}
		else
		{
			if (state != InBattleShortcutMenu.EItemState.Record)
			{
				return;
			}
			gameObject.CustomSetActive(true);
			component.gameObject.CustomSetActive(false);
			GameObject gameObject4 = gameObject.transform.Find("WriteBtn").gameObject;
			if (gameObject4 != null)
			{
				gameObject4.CustomSetActive(true);
			}
			GameObject gameObject5 = gameObject.transform.Find("RemoveBtn").gameObject;
			if (gameObject5 != null)
			{
				gameObject5.CustomSetActive(false);
			}
		}
	}

	public void SetBtnNormal()
	{
		if (this.ReviseBtn != null)
		{
			this.ReviseBtn.CustomSetActive(true);
		}
		if (this.CancelBtn != null)
		{
			this.CancelBtn.CustomSetActive(false);
		}
		if (this.SureBtn != null)
		{
			this.SureBtn.CustomSetActive(false);
		}
	}

	public void SetBtnEdit()
	{
		if (this.ReviseBtn != null)
		{
			this.ReviseBtn.CustomSetActive(false);
		}
		if (this.CancelBtn != null)
		{
			this.CancelBtn.CustomSetActive(true);
		}
		if (this.SureBtn != null)
		{
			this.SureBtn.CustomSetActive(true);
		}
	}

	public void Refresh_Right_List(int tabIndex)
	{
		InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
		if (tabIndex >= instance.title_list.get_Count())
		{
			return;
		}
		ListView<TabElement> listView = null;
		if (tabIndex < instance.title_list.get_Count())
		{
			string key = instance.title_list.get_Item(tabIndex);
			instance.tabElements.TryGetValue(key, out listView);
		}
		if (listView != null)
		{
			this._refresh_right_list(this.m_rightContentList, listView);
		}
	}

	private void _refresh_right_list(CUIListScript listScript, ListView<TabElement> data_list)
	{
		if (listScript == null || data_list == null || data_list.Count == 0)
		{
			return;
		}
		int count = data_list.Count;
		listScript.SetElementAmount(count);
	}

	public void RefreshLeft()
	{
		this._refresh_left_list(this.m_leftContentList, Singleton<InBattleMsgMgr>.instance.menuEntList);
	}

	private void _refresh_left_list(CUIListScript listScript, ListView<TabElement> data_list)
	{
		if (listScript == null || data_list == null || data_list.Count == 0)
		{
			return;
		}
		int count = data_list.Count;
		listScript.SetElementAmount(count);
	}
}

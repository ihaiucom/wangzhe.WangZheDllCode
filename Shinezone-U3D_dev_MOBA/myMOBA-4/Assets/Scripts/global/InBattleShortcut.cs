using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InBattleShortcut
{
	public enum TabType
	{
		TabShortCut,
		TabReserve,
		Count
	}

	public static readonly string InBattleMsgView_FORM_PATH = "UGUI/Form/System/Chat/Form_InBattleChat.prefab";

	public static int InBat_Bubble_CDTime = 3000;

	private CUIFormScript m_CUIForm;

	public CDButton m_cdButton;

	private DictionaryView<ulong, BubbleTimerEntity> player_bubbleTime_map = new DictionaryView<ulong, BubbleTimerEntity>();

	public InBattleShortcut.TabType[] indexTabTypeMap = new InBattleShortcut.TabType[2];

	private CUIListScript tabList;

	private CUIListScript reserveList;

	private CUIListScript shortcutList;

	private GameObject noReceiveNode;

	private GameObject signalPanelChatBtn;

	public bool NoMoreReceiveReverse
	{
		get;
		private set;
	}

	public void CacheForm(CUIFormScript battleForm, bool bPVP, bool bPVE)
	{
		this.NoMoreReceiveReverse = false;
		this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(InBattleShortcut.InBattleMsgView_FORM_PATH, true, true);
		DebugHelper.Assert(this.m_CUIForm != null, "InbattleMsgView m_CUIForm == null");
		if (this.m_CUIForm == null)
		{
			return;
		}
		this.m_CUIForm.gameObject.CustomSetActive(false);
		Singleton<CSoundManager>.GetInstance().LoadBank("System_Call", CSoundManager.BankType.Battle);
		this.RegInBattleEvent();
		if (bPVP)
		{
			Singleton<InBattleMsgMgr>.instance.BuildInBatEnt();
		}
		this.reserveList = Utility.GetComponetInChild<CUIListScript>(this.m_CUIForm.gameObject, "chatTools/node/ListView/ReserveList");
		this.shortcutList = Utility.GetComponetInChild<CUIListScript>(this.m_CUIForm.gameObject, "chatTools/node/ListView/ShortCutList");
		this.noReceiveNode = Utility.FindChild(this.m_CUIForm.gameObject, "chatTools/node/NoReceiveNode");
		this.tabList = Utility.GetComponetInChild<CUIListScript>(this.m_CUIForm.gameObject, "chatTools/node/Tab/List");
		this.signalPanelChatBtn = Utility.FindChild(battleForm.gameObject, "PVPTopRightPanel/panelTopRight/SignalPanel/Button_Chat");
		DebugHelper.Assert(this.signalPanelChatBtn != null, "InbattleMsgView signalPanelChatBtn == null");
		if (this.signalPanelChatBtn != null)
		{
			this.signalPanelChatBtn.CustomSetActive(true);
		}
		this.m_cdButton = new CDButton(this.signalPanelChatBtn);
		GameObject gameObject = Utility.FindChild(this.m_CUIForm.gameObject, "chatTools/node/InputChat_Buttons");
		if (gameObject != null)
		{
			gameObject.CustomSetActive(GameSettings.InBattleInputChatEnable == 1 && bPVP);
		}
		if (bPVE)
		{
			GameObject obj = Utility.FindChild(battleForm.gameObject, "PVPTopRightPanel/panelTopRight/SignalPanel");
			obj.CustomSetActive(true);
		}
		this.BuildTabList(bPVP);
		if (bPVP)
		{
			this.Refresh_List(InBattleShortcut.TabType.TabShortCut);
		}
		this.Refresh_List(InBattleShortcut.TabType.TabReserve);
	}

	public void OpenForm(CUIFormScript battleForm, CUIEvent uiEvent)
	{
		if (this.m_CUIForm != null)
		{
			this.m_CUIForm.gameObject.CustomSetActive(true);
			if (this.shortcutList != null)
			{
				this.shortcutList.SelectElement(-1, false);
			}
			this.CalcRedPoint();
		}
	}

	public void Clear()
	{
		Singleton<CSoundManager>.GetInstance().UnLoadBank("System_Call", CSoundManager.BankType.Battle);
		this.UnRegInBattleEvent();
		Singleton<InBattleMsgMgr>.instance.inbatEntList.Clear();
		DictionaryView<ulong, BubbleTimerEntity>.Enumerator enumerator = this.player_bubbleTime_map.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<ulong, BubbleTimerEntity> current = enumerator.Current;
			BubbleTimerEntity value = current.Value;
			if (value != null)
			{
				value.Clear();
			}
		}
		this.player_bubbleTime_map.Clear();
		this.indexTabTypeMap = null;
		this.tabList = null;
		this.reserveList = null;
		this.shortcutList = null;
		this.noReceiveNode = null;
		this.NoMoreReceiveReverse = false;
		this.signalPanelChatBtn = null;
		if (this.m_cdButton != null)
		{
			this.m_cdButton.Clear();
			this.m_cdButton = null;
		}
		this.m_CUIForm = null;
		Singleton<CUIManager>.GetInstance().CloseForm(InBattleShortcut.InBattleMsgView_FORM_PATH);
	}

	private int GetTabIndex(InBattleShortcut.TabType tabType)
	{
		for (int i = 0; i < this.indexTabTypeMap.Length; i++)
		{
			if (this.indexTabTypeMap[i] == tabType)
			{
				return i;
			}
		}
		return -1;
	}

	private void BuildTabList(bool bPVP)
	{
		int num;
		if (bPVP)
		{
			this.indexTabTypeMap[0] = InBattleShortcut.TabType.TabShortCut;
			this.indexTabTypeMap[1] = InBattleShortcut.TabType.TabReserve;
			num = 2;
		}
		else
		{
			this.indexTabTypeMap[0] = InBattleShortcut.TabType.TabReserve;
			num = 1;
		}
		this.tabList.SetElementAmount(num);
		this.tabList.SelectElement(0, true);
		for (int i = 0; i < num; i++)
		{
			CUIListElementScript elemenet = this.tabList.GetElemenet(i);
			if (elemenet != null)
			{
				this.SetTabItem(elemenet.gameObject, this.indexTabTypeMap[i]);
			}
		}
	}

	private void SetTabItem(GameObject obj, InBattleShortcut.TabType type)
	{
		if (obj == null || type == InBattleShortcut.TabType.Count)
		{
			return;
		}
		GameObject obj2 = Utility.FindChild(obj, "img_shortCut");
		obj2.CustomSetActive(type == InBattleShortcut.TabType.TabShortCut);
		GameObject obj3 = Utility.FindChild(obj, "img_reserve");
		obj3.CustomSetActive(type == InBattleShortcut.TabType.TabReserve);
		Text componetInChild = Utility.GetComponetInChild<Text>(obj, "Text");
		if (componetInChild != null)
		{
			CFriendModel model = Singleton<CFriendContoller>.instance.model;
			componetInChild.text = ((type != InBattleShortcut.TabType.TabShortCut) ? model.friendReserve.Reserve_TabReserve : model.friendReserve.Reserve_TabShortCut);
		}
	}

	public void UpdatePlayerBubbleTimer(ulong playerid, uint heroid)
	{
		BubbleTimerEntity bubbleTimerEntity = null;
		this.player_bubbleTime_map.TryGetValue(playerid, out bubbleTimerEntity);
		if (bubbleTimerEntity == null)
		{
			bubbleTimerEntity = new BubbleTimerEntity(playerid, heroid, InBattleShortcut.InBat_Bubble_CDTime);
			this.player_bubbleTime_map.Add(playerid, bubbleTimerEntity);
		}
		bubbleTimerEntity.Start();
	}

	public void Show(bool bShow)
	{
		if (this.m_CUIForm != null)
		{
			this.m_CUIForm.gameObject.CustomSetActive(bShow);
		}
	}

	public void Update()
	{
		if (this.m_cdButton != null)
		{
			this.m_cdButton.Update();
		}
	}

	public void RegInBattleEvent()
	{
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_CloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Click));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_InBat_TabChange, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_TabChange));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_InBat_Accept, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_Accept));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_InBat_Refuse, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_Refuse));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_InBat_NoMore_Receive, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_NoMore_Receive));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Reverse_InBat_ReserveData_Enable, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_ReserveData_Enable));
		Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.RECEIVE_RESERVE_DATA_CHANGE, new Action(this.OnRECEIVE_RESERVE_DATA_CHANGE));
	}

	public void UnRegInBattleEvent()
	{
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_CloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Click));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_InBat_TabChange, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_TabChange));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_InBat_Accept, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_Accept));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_InBat_Refuse, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_Refuse));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_InBat_NoMore_Receive, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_NoMore_Receive));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Reverse_InBat_ReserveData_Enable, new CUIEventManager.OnUIEventHandler(this.On_Invite_Reverse_InBat_ReserveData_Enable));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.RECEIVE_RESERVE_DATA_CHANGE, new Action(this.OnRECEIVE_RESERVE_DATA_CHANGE));
	}

	public void On_InBattleMsg_TabChange(int index)
	{
	}

	public void Refresh_List(InBattleShortcut.TabType type)
	{
		if (type == InBattleShortcut.TabType.TabShortCut)
		{
			ListView<TabElement> inbatEntList = Singleton<InBattleMsgMgr>.instance.inbatEntList;
			if (inbatEntList != null)
			{
				this._refresh_list(this.shortcutList, inbatEntList);
			}
		}
		else if (type == InBattleShortcut.TabType.TabReserve)
		{
			ListView<FriendReserve.Ent> listView = Singleton<CFriendContoller>.instance.model.friendReserve.dataList[0];
			this.reserveList.SetElementAmount(listView.Count);
		}
	}

	public void InnerHandle_InBat_PreConfigMsg(COM_INBATTLE_CHAT_TYPE chatType, uint herocfgID, uint cfg_id, ulong ullUid)
	{
		ResInBatMsgHeroActCfg heroActCfg = Singleton<InBattleMsgMgr>.instance.GetHeroActCfg(herocfgID, cfg_id);
		ResInBatMsgCfg cfgData = Singleton<InBattleMsgMgr>.instance.GetCfgData(cfg_id);
		if (cfgData == null)
		{
			return;
		}
		if (heroActCfg != null)
		{
			InBattleMsgUT.ShowInBattleMsg(chatType, ullUid, herocfgID, heroActCfg.szContent, heroActCfg.szSound);
		}
		else
		{
			InBattleMsgUT.ShowInBattleMsg(chatType, ullUid, herocfgID, cfgData.szContent, cfgData.szSound);
		}
		if (chatType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_SIGNAL && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
		{
			Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(ullUid);
			ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = playerByUid.GetAllHeroes();
			for (int i = 0; i < allHeroes.Count; i++)
			{
				ActorRoot handle = allHeroes[i].handle;
				if (handle != null && (long)handle.TheActorMeta.ConfigId == (long)((ulong)herocfgID))
				{
					Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
					if (currentCamera != null)
					{
						Vector2 sreenLoc = currentCamera.WorldToScreenPoint(handle.HudControl.GetSmallMapPointer_WorldPosition());
						Singleton<CUIParticleSystem>.instance.AddParticle(cfgData.szMiniMapEffect, 2f, sreenLoc, null);
					}
				}
			}
		}
	}

	public void On_InBattleMsg_ListElement_Click(CUIEvent uiEvent)
	{
		this.Show(false);
		int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
		this.Send_Config_Chat(srcWidgetIndexInBelongedList);
	}

	public void On_Invite_Reverse_InBat_NoMore_Receive(CUIEvent uiEvent)
	{
		this.NoMoreReceiveReverse = true;
		this.noReceiveNode.CustomSetActive(false);
		FriendSysNetCore.SendReceiveReserveProcess(0uL, 0u, 0, this.NoMoreReceiveReverse);
	}

	private void On_Invite_Reverse_InBat_ReserveData_Enable(CUIEvent uievent)
	{
		ListView<FriendReserve.Ent> listView = Singleton<CFriendContoller>.instance.model.friendReserve.dataList[0];
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= listView.Count)
		{
			return;
		}
		FriendReserve.Ent ent = listView[srcWidgetIndexInBelongedList];
		Text componetInChild = Utility.GetComponetInChild<Text>(uievent.m_srcWidget.gameObject, "Text");
		if (componetInChild != null)
		{
			componetInChild.text = ent.inBattleContent;
		}
	}

	private void CalcRedPoint()
	{
		ListView<FriendReserve.Ent> listView = Singleton<CFriendContoller>.instance.model.friendReserve.dataList[0];
		int count = listView.Count;
		if (count > 0)
		{
			CUICommonSystem.AddRedDot(this.signalPanelChatBtn, enRedDotPos.enTopRight, count, 0, 0);
		}
		else
		{
			CUICommonSystem.DelRedDot(this.signalPanelChatBtn);
		}
		int tabIndex = this.GetTabIndex(InBattleShortcut.TabType.TabReserve);
		if (tabIndex != -1)
		{
			CUIListElementScript elemenet = this.tabList.GetElemenet(tabIndex);
			if (elemenet != null && elemenet.gameObject != null)
			{
				GameObject gameObject = Utility.FindChild(elemenet.gameObject, "RedDot");
				if (gameObject == null)
				{
					return;
				}
				if (count > 0)
				{
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, count, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(gameObject);
				}
			}
		}
	}

	private void OnRECEIVE_RESERVE_DATA_CHANGE()
	{
		this.CalcRedPoint();
		this.Refresh_List(InBattleShortcut.TabType.TabReserve);
	}

	private void On_Invite_Reverse_InBat_Refuse(CUIEvent uievent)
	{
		ListView<FriendReserve.Ent> listView = Singleton<CFriendContoller>.instance.model.friendReserve.dataList[0];
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= listView.Count)
		{
			return;
		}
		FriendReserve.Ent ent = listView[srcWidgetIndexInBelongedList];
		if (ent == null)
		{
			return;
		}
		FriendSysNetCore.SendReceiveReserveProcess(ent.ullUid, ent.dwLogicWorldId, 1, this.NoMoreReceiveReverse);
		listView.RemoveAt(srcWidgetIndexInBelongedList);
		this.OnRECEIVE_RESERVE_DATA_CHANGE();
	}

	private void On_Invite_Reverse_InBat_Accept(CUIEvent uievent)
	{
		ListView<FriendReserve.Ent> listView = Singleton<CFriendContoller>.instance.model.friendReserve.dataList[0];
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= listView.Count)
		{
			return;
		}
		FriendReserve.Ent ent = listView[srcWidgetIndexInBelongedList];
		if (ent == null)
		{
			return;
		}
		FriendSysNetCore.SendReceiveReserveProcess(ent.ullUid, ent.dwLogicWorldId, 2, this.NoMoreReceiveReverse);
		List<string> accept_receiveReserve = Singleton<CFriendContoller>.instance.model.friendReserve.accept_receiveReserve;
		if (!accept_receiveReserve.Contains(ent.name))
		{
			accept_receiveReserve.Add(ent.name);
		}
		listView.RemoveAt(srcWidgetIndexInBelongedList);
		this.OnRECEIVE_RESERVE_DATA_CHANGE();
	}

	public void On_Invite_Reverse_InBat_TabChange(CUIEvent uiEvent)
	{
		int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
		InBattleShortcut.TabType tabType = this.indexTabTypeMap[selectedIndex];
		CUIListElementScript elemenet = this.tabList.GetElemenet(selectedIndex);
		if (elemenet != null)
		{
			this.SetTabItem(elemenet.gameObject, tabType);
		}
		this.ShowTab(tabType);
	}

	private void ShowTab(InBattleShortcut.TabType tabType)
	{
		if (tabType == InBattleShortcut.TabType.TabReserve)
		{
			this.noReceiveNode.CustomSetActive(!this.NoMoreReceiveReverse);
			if (this.reserveList != null)
			{
				this.reserveList.gameObject.CustomSetActive(true);
			}
			if (this.shortcutList != null)
			{
				this.shortcutList.gameObject.CustomSetActive(false);
			}
		}
		else if (tabType == InBattleShortcut.TabType.TabShortCut)
		{
			this.noReceiveNode.CustomSetActive(false);
			if (this.reserveList != null)
			{
				this.reserveList.gameObject.CustomSetActive(false);
			}
			if (this.shortcutList != null)
			{
				this.shortcutList.gameObject.CustomSetActive(true);
			}
		}
	}

	public void Send_Config_Chat(int index)
	{
		if (this.m_cdButton == null || this.m_cdButton.startCooldownTimestamp != 0uL)
		{
			return;
		}
		if (index < 0 || index >= Singleton<InBattleMsgMgr>.instance.inbatEntList.Count)
		{
			return;
		}
		if (Singleton<InBattleMsgMgr>.instance.inbatEntList.Count == 0)
		{
			Singleton<InBattleMsgMgr>.instance.BuildInBatEnt();
		}
		TabElement tabElement = Singleton<InBattleMsgMgr>.instance.inbatEntList[index];
		if (tabElement == null)
		{
			return;
		}
		Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		if (hostPlayer == null)
		{
			return;
		}
		ResInBatMsgCfg cfgData = Singleton<InBattleMsgMgr>.instance.GetCfgData(tabElement.cfgId);
		DebugHelper.Assert(cfgData != null, "InbattleMsgView cfg_data == null");
		if (cfgData == null)
		{
			return;
		}
		SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
		if (curLvelContext == null)
		{
			return;
		}
		if (!Singleton<InBattleMsgMgr>.instance.ShouldBeThroughNet(curLvelContext))
		{
			if (tabElement.cfgId >= 1u && tabElement.cfgId <= 15u)
			{
				CPlayerBehaviorStat.Plus((CPlayerBehaviorStat.BehaviorType)tabElement.cfgId);
			}
			this.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE)cfgData.bShowType, hostPlayer.CaptainId, tabElement.cfgId, hostPlayer.PlayerUId);
		}
		else
		{
			if (tabElement.cfgId >= 1u && tabElement.cfgId <= 15u)
			{
				CPlayerBehaviorStat.Plus((CPlayerBehaviorStat.BehaviorType)tabElement.cfgId);
			}
			InBattleMsgNetCore.SendInBattleMsg_PreConfig(tabElement.cfgId, (COM_INBATTLE_CHAT_TYPE)cfgData.bShowType, hostPlayer.CaptainId);
		}
		if (this.m_cdButton != null)
		{
			ResInBatChannelCfg dataByKey = GameDataMgr.inBattleChannelDatabin.GetDataByKey((uint)cfgData.bInBatChannelID);
			if (dataByKey != null)
			{
				this.m_cdButton.StartCooldown(dataByKey.dwCdTime, null);
			}
			else
			{
				this.m_cdButton.StartCooldown(4000u, null);
			}
		}
	}

	private void _refresh_list(CUIListScript listScript, ListView<TabElement> data_list)
	{
		if (listScript == null || data_list == null || data_list.Count == 0)
		{
			return;
		}
		listScript.SetElementAmount(data_list.Count);
	}

	public void On_InBattleMsg_CloseForm(CUIEvent uiEvent)
	{
		this.Show(false);
	}

	public void On_InBattleMsg_ListElement_Enable(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= Singleton<InBattleMsgMgr>.instance.inbatEntList.Count)
		{
			return;
		}
		TabElement tabElement = Singleton<InBattleMsgMgr>.instance.inbatEntList[srcWidgetIndexInBelongedList];
		if (tabElement == null)
		{
			return;
		}
		InBattleMsgShower component = uievent.m_srcWidget.GetComponent<InBattleMsgShower>();
		if (component != null && tabElement != null)
		{
			component.Set(tabElement.cfgId, tabElement.configContent);
		}
	}
}

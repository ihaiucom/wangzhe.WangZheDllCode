using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class IntimacyRelationView
	{
		public class IntimacyTipsMgr
		{
			public enum EShowType
			{
				None = -1,
				ValueUpTips,
				LevelUpMsgBoxAll
			}

			public class Ent
			{
				public string content;

				public IntimacyRelationView.IntimacyTipsMgr.EShowType ShowType;

				public COM_INTIMACY_STATE state;

				public string name;

				public Ent(IntimacyRelationView.IntimacyTipsMgr.EShowType showType)
				{
					this.ShowType = showType;
				}

				public Ent(string content, IntimacyRelationView.IntimacyTipsMgr.EShowType showType, string name, COM_INTIMACY_STATE state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
				{
					this.content = content;
					this.ShowType = showType;
					this.name = name;
					this.state = state;
				}

				public void Clear()
				{
					this.content = string.Empty;
					this.ShowType = IntimacyRelationView.IntimacyTipsMgr.EShowType.None;
					this.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
					this.name = string.Empty;
				}

				public bool IsValid()
				{
					return !string.IsNullOrEmpty(this.content);
				}
			}

			public List<IntimacyRelationView.IntimacyTipsMgr.Ent> contents = new List<IntimacyRelationView.IntimacyTipsMgr.Ent>();

			private int deltaValue;

			public static string shareLevelUpForm = "UGUI/Form/System/Friend/Form_Intimacy.prefab";

			public IntimacyTipsMgr()
			{
				this.Init();
			}

			public void Init()
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_CheckShouldShowTips, new CUIEventManager.OnUIEventHandler(this.CheckShouldShowTips));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_ShareLevelUp, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_ShareLevelUp));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_ShareLevelUpMenu_OK, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_ShareLevelUpMenu_OK));
				this.contents.Clear();
				this.contents.Add(new IntimacyRelationView.IntimacyTipsMgr.Ent(string.Empty, IntimacyRelationView.IntimacyTipsMgr.EShowType.ValueUpTips, string.Empty, COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL));
			}

			public void Clear()
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_CheckShouldShowTips, new CUIEventManager.OnUIEventHandler(this.CheckShouldShowTips));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_ShareLevelUp, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_ShareLevelUp));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_ShareLevelUpMenu_OK, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_ShareLevelUpMenu_OK));
				this.contents.Clear();
			}

			public void CheckInBattleIntimacyShowTips()
			{
			}

			public void CheckShouldShowTips(CUIEvent uiEvent)
			{
				if (this.contents.Count > 0)
				{
					IntimacyRelationView.IntimacyTipsMgr.Ent ent = this.contents[this.contents.Count - 1];
					this.contents.RemoveAt(this.contents.Count - 1);
					if (ent.ShowType == IntimacyRelationView.IntimacyTipsMgr.EShowType.LevelUpMsgBoxAll)
					{
						string text = Singleton<CTextManager>.instance.GetText("RelaLevelUp_Title");
						string text2 = Singleton<CTextManager>.instance.GetText("RelaLevelUp_ShareText");
						stUIEventParams param = default(stUIEventParams);
						param.tag = (int)ent.state;
						param.tagStr = ent.name;
						Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(ent.content, enUIEventID.IntimacyRela_CheckShouldShowTips, enUIEventID.IntimacyRela_ShareLevelUp, param, string.Empty, text2, false, text);
					}
					else if (ent.ShowType == IntimacyRelationView.IntimacyTipsMgr.EShowType.ValueUpTips && !string.IsNullOrEmpty(ent.content))
					{
						string strContent = string.Format(UT.GetText("Intimacy_UpInfo"), ent.content, this.deltaValue);
						Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
						ent.content = string.Empty;
						this.deltaValue = 0;
					}
				}
			}

			public void AddRelaLevelUpMsgBox(string otherName, int newLevel, COM_INTIMACY_STATE state)
			{
				RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(state);
				if (relaTextCfg == null)
				{
					return;
				}
				string levelDescStr = IntimacyRelationViewUT.GetLevelDescStr(newLevel);
				string text = Singleton<CTextManager>.instance.GetText("RelaLevelUp_Info");
				string content = string.Format(text, new object[]
				{
					otherName,
					relaTextCfg.IntimRela_Type,
					levelDescStr,
					levelDescStr,
					relaTextCfg.IntimRela_Type
				});
				this.contents.Add(new IntimacyRelationView.IntimacyTipsMgr.Ent(content, IntimacyRelationView.IntimacyTipsMgr.EShowType.LevelUpMsgBoxAll, otherName, state));
			}

			private IntimacyRelationView.IntimacyTipsMgr.Ent Find(IntimacyRelationView.IntimacyTipsMgr.EShowType type)
			{
				for (int i = 0; i < this.contents.Count; i++)
				{
					IntimacyRelationView.IntimacyTipsMgr.Ent ent = this.contents[i];
					if (ent.ShowType == type)
					{
						return ent;
					}
				}
				return null;
			}

			public void RecordPlayerUpValueTips(string otherName, int deltaValue)
			{
				IntimacyRelationView.IntimacyTipsMgr.Ent ent = this.Find(IntimacyRelationView.IntimacyTipsMgr.EShowType.ValueUpTips);
				if (ent == null)
				{
					ent = new IntimacyRelationView.IntimacyTipsMgr.Ent(string.Empty, IntimacyRelationView.IntimacyTipsMgr.EShowType.ValueUpTips, string.Empty, COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL);
					this.contents.Insert(0, ent);
				}
				if (string.IsNullOrEmpty(ent.content))
				{
					ent.content = string.Format("{0}", otherName);
				}
				else
				{
					ent.content = string.Format("{0}, {1}", ent.content, otherName);
				}
				this.deltaValue = deltaValue;
			}

			private void OnIntimacyRela_ShareLevelUp(CUIEvent uievent)
			{
				MonoSingleton<ShareSys>.instance.OpenShareFriendRelationFrom((COM_INTIMACY_STATE)uievent.m_eventParams.tag, uievent.m_eventParams.tagStr);
			}

			private void OnIntimacyRela_ShareLevelUpMenu_OK(CUIEvent uievent)
			{
				Singleton<CUIManager>.instance.CloseForm(IntimacyRelationView.IntimacyTipsMgr.shareLevelUpForm);
				this.CheckShouldShowTips(null);
			}
		}

		public class RelationLevelUpView
		{
			public void Open(int intimacyValue, COM_INTIMACY_STATE state, string name)
			{
			}
		}

		public const string FRDataChange = "FRDataChange";

		public CUIListScript listScript;

		private CUIFormScript form;

		public void Open()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Menu_Close, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Menu_Close));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Show_Drop_List, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Show_Drop_List));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Drop_ListElement_Click));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_Drop_ListElement_Enable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_OK));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Cancle, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Cancle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRelation_Item_Enable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_FristChoise, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_FristChoise));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Delete_Relation, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Delete_Relation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_PrevState, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_PrevState));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Delete_Relation_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Delete_Relation_OK));
			Singleton<EventRouter>.GetInstance().AddEventHandler("FRDataChange", new Action(this.On_FRDataChange));
			this.form = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.IntimacyRelaFormPath, false, true);
			this.listScript = this.form.transform.FindChild("GameObject/list").GetComponent<CUIListScript>();
			this.form.transform.FindChild("GameObject/info/txt").GetComponent<Text>().text = UT.FRData().IntimRela_EmptyDataText;
			this.Refresh();
		}

		public void Clear()
		{
			this.listScript = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Menu_Close, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Menu_Close));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Show_Drop_List, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Show_Drop_List));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Drop_ListElement_Click));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_Drop_ListElement_Enable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_OK));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Cancle, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Cancle));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRelation_Item_Enable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_FristChoise, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_FristChoise));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Delete_Relation, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Delete_Relation));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_PrevState, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_PrevState));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Delete_Relation_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Delete_Relation_OK));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("FRDataChange", new Action(this.On_FRDataChange));
			CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
			ListView<CFR> list = model.FRData.GetList();
			for (int i = 0; i < list.Count; i++)
			{
				CFR cFR = list[i];
				cFR.choiseRelation = -1;
				cFR.bInShowChoiseRelaList = false;
				cFR.bRedDot = false;
			}
			if (Singleton<CFriendContoller>.instance.view != null)
			{
				Singleton<CFriendContoller>.instance.view.Refresh();
			}
			Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
			this.form = null;
		}

		private void On_FRDataChange()
		{
			this.Refresh();
		}

		public void Refresh()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.IntimacyRelaFormPath);
			if (cUIFormScript == null)
			{
				return;
			}
			bool flag = Singleton<CFriendContoller>.GetInstance().model.FRData.GetList().Count == 0;
			if (flag)
			{
				cUIFormScript.transform.FindChild("GameObject/info").gameObject.CustomSetActive(true);
				cUIFormScript.transform.FindChild("GameObject/list").gameObject.CustomSetActive(false);
			}
			else
			{
				cUIFormScript.transform.FindChild("GameObject/info").gameObject.CustomSetActive(false);
				cUIFormScript.transform.FindChild("GameObject/list").gameObject.CustomSetActive(true);
				this.Refresh_IntimacyRelation_List();
			}
		}

		public void Refresh_IntimacyRelation_List()
		{
			CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
			ListView<CFR> list = model.FRData.GetList();
			this.listScript.SetElementAmount(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				CUIListElementScript elemenet = this.listScript.GetElemenet(i);
				this.ShowIntimacyRelation_Item(elemenet, list[i], this.form);
			}
		}

		public void On_IntimacyRelation_Item_Enable(CUIEvent uievent)
		{
			if (uievent == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
			ListView<CFR> list = model.FRData.GetList();
			if (list == null)
			{
				return;
			}
			CFR cFR = null;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < list.Count)
			{
				cFR = list[srcWidgetIndexInBelongedList];
			}
			if (cFR != null && uievent.m_srcWidget != null)
			{
				CUIComponent component = uievent.m_srcWidget.GetComponent<CUIComponent>();
				if (component != null)
				{
					this.ShowIntimacyRelation_Item(component, cFR, this.form);
				}
			}
		}

		public void ShowIntimacyRelation_Item(CUIComponent com, CFR frData, CUIFormScript uiFrom)
		{
			if (com == null || frData == null)
			{
				return;
			}
			IntimacyRelationViewUT.Show_Item(com, frData, uiFrom);
		}

		private void On_IntimacyRela_Menu_Close(CUIEvent uievent)
		{
			this.Clear();
		}

		private void On_IntimacyRela_Cancle(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			COM_INTIMACY_STATE tag = (COM_INTIMACY_STATE)uievent.m_eventParams.tag;
			Singleton<CFriendContoller>.instance.model.FRData.ResetChoiseRelaState(commonUInt64Param, tagUInt);
			if (IntimacyRelationViewUT.IsRelaStateConfirm(tag))
			{
				COM_INTIMACY_STATE stateByConfirmState = IntimacyRelationViewUT.GetStateByConfirmState(tag);
				FriendRelationNetCore.Send_CHG_INTIMACY_DENY(commonUInt64Param, tagUInt, stateByConfirmState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
				CFriendRelationship.FRData.Add(commonUInt64Param, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0u, false);
			}
			if (IntimacyRelationViewUT.IsRelaStateDeny(tag))
			{
				COM_INTIMACY_STATE stateByDenyState = IntimacyRelationViewUT.GetStateByDenyState(tag);
				FriendRelationNetCore.Send_CHG_INTIMACY_DENY(commonUInt64Param, tagUInt, stateByDenyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
				CFriendRelationship.FRData.Add(commonUInt64Param, tagUInt, stateByDenyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0u, false);
			}
		}

		private void On_IntimacyRela_OK(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			COM_INTIMACY_STATE tag = (COM_INTIMACY_STATE)uievent.m_eventParams.tag;
			int tag2 = uievent.m_eventParams.tag2;
			if (tag == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL && tag2 != -1)
			{
				CFriendRelationship fRData = Singleton<CFriendContoller>.instance.model.FRData;
				CFriendRelationship.FRConfig cFGByIndex = fRData.GetCFGByIndex(tag2);
				if (cFGByIndex == null)
				{
					return;
				}
				if (IntimacyRelationViewUT.IsRelaState(cFGByIndex.state))
				{
					if (fRData.FindFrist(cFGByIndex.state) == null)
					{
						FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(commonUInt64Param, tagUInt, cFGByIndex.state, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
						Singleton<CFriendContoller>.instance.model.FRData.ResetChoiseRelaState(commonUInt64Param, tagUInt);
					}
					else
					{
						RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(cFGByIndex.state);
						if (relaTextCfg != null)
						{
							Singleton<CUIManager>.instance.OpenTips(relaTextCfg.IntimRela_Tips_AlreadyHas, true, 1.5f, null, new object[0]);
						}
					}
				}
			}
			else if (IntimacyRelationViewUT.IsRelaStateConfirm(tag))
			{
				FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(commonUInt64Param, tagUInt, IntimacyRelationViewUT.GetStateByConfirmState(tag), COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
			}
			else if (IntimacyRelationViewUT.IsRelaStateDeny(tag))
			{
				FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(commonUInt64Param, tagUInt, IntimacyRelationViewUT.GetStateByDenyState(tag), COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
			}
		}

		private void On_IntimacyRela_FristChoise(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt);
			if (cfr != null)
			{
				FriendRelationNetCore.Send_CHG_INTIMACYPRIORITY(cfr.state);
			}
		}

		private void On_IntimacyRela_PrevState(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt);
			if (IntimacyRelationViewUT.IsRelaStateConfirm(cfr.state) && !cfr.bReciveOthersRequest)
			{
				cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL;
			}
			if (IntimacyRelationViewUT.IsRelaStateDeny(cfr.state) && !cfr.bReciveOthersRequest)
			{
				cfr.state = IntimacyRelationViewUT.GetStateByDenyState(cfr.state);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
		}

		private void On_IntimacyRela_Delete_Relation_OK(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt);
			if (cfr != null && IntimacyRelationViewUT.IsRelaState(cfr.state))
			{
				FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(commonUInt64Param, tagUInt, cfr.state, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
			}
		}

		private void On_IntimacyRela_Delete_Relation(CUIEvent uievent)
		{
			stUIEventParams par = default(stUIEventParams);
			par.commonUInt64Param1 = uievent.m_eventParams.commonUInt64Param1;
			par.tagUInt = uievent.m_eventParams.tagUInt;
			string text = Singleton<CTextManager>.instance.GetText("Delete_Relation_Tips");
			Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(text, enUIEventID.IntimacyRela_Delete_Relation_OK, enUIEventID.None, par, false);
		}

		private void On_IntimacyRela_Show_Drop_List(CUIEvent uievent)
		{
			ulong commonUInt64Param = uievent.m_eventParams.commonUInt64Param1;
			uint tagUInt = uievent.m_eventParams.tagUInt;
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt);
			if (cfr != null)
			{
				cfr.bInShowChoiseRelaList = !cfr.bInShowChoiseRelaList;
				CUIComponent component = uievent.m_srcWidgetScript.m_widgets[0].GetComponent<CUIComponent>();
				CUIListScript component2 = uievent.m_srcWidgetScript.m_widgets[1].GetComponent<CUIListScript>();
				if (component2 != null)
				{
					component2.SetElementAmount(4);
				}
				IntimacyRelationViewUT.Show_Item(component, cfr, this.form);
			}
		}

		public void On_Drop_ListElement_Enable(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			CUIEventScript component = uievent.m_srcWidgetScript.GetComponent<CUIEventScript>();
			CUIComponent component2 = component.m_widgets[0].GetComponent<CUIComponent>();
			CUIEventScript component3 = component2.m_widgets[7].GetComponent<CUIEventScript>();
			ulong commonUInt64Param = component3.m_onClickEventParams.commonUInt64Param1;
			uint tagUInt = component3.m_onClickEventParams.tagUInt;
			if (Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt) == null)
			{
				return;
			}
			Text component4 = uievent.m_srcWidget.transform.Find("Text").GetComponent<Text>();
			if (component4 != null)
			{
				CFriendRelationship.FRConfig cFGByIndex = Singleton<CFriendContoller>.instance.model.FRData.GetCFGByIndex(srcWidgetIndexInBelongedList);
				if (cFGByIndex != null)
				{
					component4.text = cFGByIndex.cfgRelaStr;
				}
			}
		}

		private void On_IntimacyRela_Drop_ListElement_Click(CUIEvent uievent)
		{
			CUIComponent component = uievent.m_srcWidgetScript.m_widgets[0].GetComponent<CUIComponent>();
			CUIEventScript component2 = component.m_widgets[7].GetComponent<CUIEventScript>();
			ulong commonUInt64Param = component2.m_onClickEventParams.commonUInt64Param1;
			uint tagUInt = component2.m_onClickEventParams.tagUInt;
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(commonUInt64Param, tagUInt);
			if (cfr == null)
			{
				return;
			}
			cfr.bInShowChoiseRelaList = false;
			cfr.choiseRelation = uievent.m_srcWidgetIndexInBelongedList;
			IntimacyRelationViewUT.Show_Item(component, cfr, this.form);
		}
	}
}

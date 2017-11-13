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
	public class CLoudSpeakerSys : Singleton<CLoudSpeakerSys>
	{
		public const int SPEAKER_ID = 10041;

		public const int LOUDSPEAKER_ID = 10042;

		public const int REQ_TIME_DELTA = 5;

		public static readonly string SPEAKER_FORM_PATH = "UGUI/Form/System/LoudSpeaker/Form_LoudSpeaker.prefab";

		private static ResHornInfo speakerRes;

		private static ResHornInfo loudSpeakerRes;

		private int m_timerReq = -1;

		private int m_timerSpeaker = -1;

		private int m_timerLoudSpeaker = -1;

		private uint m_lastSpeakerBeginSec;

		private uint m_lastLoudSpeakerBeginSec;

		private uint m_itemID;

		private uint m_characterLimit;

		private static string s_characterLimitString = string.Empty;

		private ListView<COMDT_CHAT_MSG_HORN> speakerList = new ListView<COMDT_CHAT_MSG_HORN>();

		private ListView<COMDT_CHAT_MSG_HORN> loudSpeakerList = new ListView<COMDT_CHAT_MSG_HORN>();

		public COMDT_CHAT_MSG_HORN CurSpekaer;

		public COMDT_CHAT_MSG_HORN CurLoudSpeaker;

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Clsoe, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Send, new CUIEventManager.OnUIEventHandler(this.OnSpeakerSend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Update, new CUIEventManager.OnUIEventHandler(this.OnUpdateCharacterLimitText));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_OpenFactoryShop, new CUIEventManager.OnUIEventHandler(this.OnOpenFactoryShop));
			CLoudSpeakerSys.s_characterLimitString = Singleton<CTextManager>.instance.GetText("Speaker_CharacterLimit");
		}

		public static ResHornInfo GetRes(int speakerID)
		{
			if (speakerID == 10041)
			{
				if (CLoudSpeakerSys.speakerRes == null)
				{
					CLoudSpeakerSys.speakerRes = GameDataMgr.speakerDatabin.GetDataByKey(10041u);
				}
				return CLoudSpeakerSys.speakerRes;
			}
			if (speakerID == 10042)
			{
				if (CLoudSpeakerSys.loudSpeakerRes == null)
				{
					CLoudSpeakerSys.loudSpeakerRes = GameDataMgr.speakerDatabin.GetDataByKey(10042u);
				}
				return CLoudSpeakerSys.loudSpeakerRes;
			}
			return null;
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Clsoe, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Send, new CUIEventManager.OnUIEventHandler(this.OnSpeakerSend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Update, new CUIEventManager.OnUIEventHandler(this.OnUpdateCharacterLimitText));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_OpenFactoryShop, new CUIEventManager.OnUIEventHandler(this.OnOpenFactoryShop));
			base.UnInit();
		}

		public void StartReqTimer()
		{
			if (this.m_timerReq != -1)
			{
				return;
			}
			this.m_timerReq = Singleton<CTimerManager>.instance.AddTimer(5000, 0, new CTimer.OnTimeUpHandler(this.OnTimerReq));
			this.OnTimerReq(0);
		}

		public void Clear()
		{
			if (this.m_timerReq != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerReq);
			}
			this.m_timerReq = -1;
			if (this.m_timerSpeaker != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSpeaker);
			}
			this.m_timerSpeaker = -1;
			if (this.m_timerLoudSpeaker != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerLoudSpeaker);
			}
			this.m_timerLoudSpeaker = -1;
			this.m_lastSpeakerBeginSec = 0u;
			this.m_lastLoudSpeakerBeginSec = 0u;
			this.m_itemID = 0u;
			this.speakerList.Clear();
			this.loudSpeakerList.Clear();
			this.CurLoudSpeaker = null;
			this.CurSpekaer = null;
		}

		private void OnSpeakerFormOpen(CUIEvent uiEvent)
		{
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			this.OpenSpeakerForm(commonUInt32Param);
		}

		private void OnOpenFactoryShop(CUIEvent uiEvent)
		{
			uiEvent.m_eventID = enUIEventID.Chat_CloseForm;
			Singleton<CUIEventManager>.instance.DispatchUIEvent(uiEvent);
			uiEvent.m_eventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
			Singleton<CUIEventManager>.instance.DispatchUIEvent(uiEvent);
		}

		public void OpenSpeakerForm(uint itemID)
		{
			if (itemID == 10041u || itemID == 10042u)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return;
				}
				CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
				if (useableContainer == null)
				{
					return;
				}
				if (useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, itemID) == 0)
				{
					CMallFactoryShopController.ShopProduct product;
					if (itemID == 10041u)
					{
						product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(GameDataMgr.globalInfoDatabin.GetDataByKey(212u).dwConfValue);
					}
					else
					{
						product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(GameDataMgr.globalInfoDatabin.GetDataByKey(211u).dwConfValue);
					}
					if (product != null)
					{
						CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
						uIEvent.m_eventID = enUIEventID.Mall_Buy_Product_Confirm;
						uIEvent.m_eventParams.commonUInt64Param1 = (ulong)product.Key;
						uIEvent.m_eventParams.commonUInt32Param1 = 1u;
						Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(product, 1u, true, uIEvent);
					}
					return;
				}
				ResHornInfo dataByKey = GameDataMgr.speakerDatabin.GetDataByKey(itemID);
				this.m_itemID = itemID;
				this.m_characterLimit = dataByKey.dwWordLimit;
				if (dataByKey == null)
				{
					return;
				}
				CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CLoudSpeakerSys.SPEAKER_FORM_PATH, false, false);
				if (cUIFormScript == null || cUIFormScript.gameObject == null)
				{
					return;
				}
				GameObject obj = Utility.FindChild(cUIFormScript.gameObject, "pnlBg/Title/speakerText");
				GameObject obj2 = Utility.FindChild(cUIFormScript.gameObject, "pnlBg/Title/loudSpeakerText");
				GameObject obj3 = Utility.FindChild(cUIFormScript.gameObject, "pnlBg/Model/speaker");
				GameObject obj4 = Utility.FindChild(cUIFormScript.gameObject, "pnlBg/Model/loudspeaker");
				InputField componetInChild = Utility.GetComponetInChild<InputField>(cUIFormScript.gameObject, "pnlBg/Panel_Main/InputField");
				CUITimerScript componetInChild2 = Utility.GetComponetInChild<CUITimerScript>(cUIFormScript.gameObject, "Timer");
				componetInChild2.ReStartTimer();
				if (itemID == 10041u)
				{
					obj.CustomSetActive(true);
					obj2.CustomSetActive(false);
					obj3.CustomSetActive(true);
					obj4.CustomSetActive(false);
					componetInChild.set_characterLimit((int)this.m_characterLimit);
				}
				else
				{
					obj.CustomSetActive(false);
					obj2.CustomSetActive(true);
					obj3.CustomSetActive(false);
					obj4.CustomSetActive(true);
					componetInChild.set_characterLimit((int)this.m_characterLimit);
				}
			}
		}

		private void OnSpeakerFormClose(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CLoudSpeakerSys.SPEAKER_FORM_PATH);
		}

		private void OnSpeakerSend(CUIEvent uiEvent)
		{
			string inputText = this.GetInputText();
			if (string.IsNullOrEmpty(inputText))
			{
				Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_10", true, 1.5f, null, new object[0]);
				return;
			}
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			CUseable useableByBaseID = useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_itemID);
			if (useableByBaseID == null)
			{
				return;
			}
			this.OnSpeakerSend(inputText, useableByBaseID.m_objID);
		}

		private void OnUpdateCharacterLimitText(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLoudSpeakerSys.SPEAKER_FORM_PATH);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Main/CharLimitTxt");
			if (componetInChild == null)
			{
				return;
			}
			InputField componetInChild2 = Utility.GetComponetInChild<InputField>(form.gameObject, "pnlBg/Panel_Main/InputField");
			if (componetInChild2 == null)
			{
				return;
			}
			int num = (int)(this.m_characterLimit - (uint)componetInChild2.get_text().get_Length());
			if (num < 0)
			{
				num = 0;
			}
			componetInChild.set_text(string.Format(CLoudSpeakerSys.s_characterLimitString, num));
		}

		private string GetInputText()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLoudSpeakerSys.SPEAKER_FORM_PATH);
			if (form == null || form.gameObject == null)
			{
				return string.Empty;
			}
			InputField componetInChild = Utility.GetComponetInChild<InputField>(form.gameObject, "pnlBg/Panel_Main/InputField");
			return componetInChild.get_text();
		}

		private void OnSpeakerSend(string content, ulong uniqueID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1123u);
			cSPkg.stPkgData.stHornUseReq.ullUniqueID = uniqueID;
			byte[] array = Utility.BytesConvert(content);
			byte[] szContent = cSPkg.stPkgData.stHornUseReq.szContent;
			Array.Copy(array, szContent, Math.Min(array.Length, szContent.Length));
			szContent[szContent.Length - 1] = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void AddSpeakerArray(CS_HORN_TYPE type, COMDT_CHAT_MSG_HORN[] astMsgInfo, uint len)
		{
			ListView<COMDT_CHAT_MSG_HORN> listView = this.GetSpeakerList(type);
			int lastSpeakerBeginSec = (int)this.GetLastSpeakerBeginSec(type);
			int num = 0;
			while ((long)num < (long)((ulong)len))
			{
				if (lastSpeakerBeginSec < (int)astMsgInfo[num].dwBeginShowSec)
				{
					listView.Add(astMsgInfo[num]);
					Singleton<CChatController>.instance.model.Add_Palyer_Info(astMsgInfo[num].stFrom);
					if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
					{
						bool flag = astMsgInfo[num].stFrom.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
						if (Singleton<CChatController>.instance.view != null)
						{
							Singleton<CChatController>.instance.view.bRefreshNew = (!Singleton<CChatController>.instance.view.IsCheckHistory() || flag);
						}
						CChatEntity chatEnt = CChatUT.Build_4_Speaker(astMsgInfo[num]);
						Singleton<CChatController>.instance.model.channelMgr.Add_ChatEntity(chatEnt, EChatChannel.Lobby, 0uL, 0u);
						this.m_lastSpeakerBeginSec = Math.Max(astMsgInfo[num].dwBeginShowSec, this.m_lastSpeakerBeginSec);
					}
					else
					{
						this.m_lastLoudSpeakerBeginSec = Math.Max(astMsgInfo[num].dwBeginShowSec, this.m_lastLoudSpeakerBeginSec);
					}
				}
				num++;
			}
			if (len > 0u)
			{
				if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
				{
					this.OnSpeakerNodeOpen();
				}
				else if (type == CS_HORN_TYPE.CS_HORNTYPE_BIGER)
				{
					this.OnLoudSpeakerTipsOpen();
				}
			}
		}

		private ListView<COMDT_CHAT_MSG_HORN> GetSpeakerList(CS_HORN_TYPE type)
		{
			ListView<COMDT_CHAT_MSG_HORN> result;
			if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
			{
				result = this.speakerList;
			}
			else
			{
				result = this.loudSpeakerList;
			}
			return result;
		}

		private uint GetLastSpeakerBeginSec(CS_HORN_TYPE type)
		{
			if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
			{
				return this.m_lastSpeakerBeginSec;
			}
			return this.m_lastLoudSpeakerBeginSec;
		}

		private COMDT_CHAT_MSG_HORN PopSpeakerList(CS_HORN_TYPE type)
		{
			COMDT_CHAT_MSG_HORN result = null;
			ListView<COMDT_CHAT_MSG_HORN> listView = this.GetSpeakerList(type);
			if (listView.Count > 0)
			{
				result = listView[0];
				listView.RemoveAt(0);
			}
			return result;
		}

		[MessageHandler(1124)]
		public static void OnSpeakerSendRsp(CSPkg msg)
		{
			if (msg.stPkgData.stHornUseRsp.iResult == 0)
			{
				Singleton<CUIManager>.instance.CloseForm(CLoudSpeakerSys.SPEAKER_FORM_PATH);
			}
			else if (msg.stPkgData.stHornUseRsp.iResult == 2)
			{
				Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_2", true, 1.5f, null, new object[0]);
			}
			else if (msg.stPkgData.stHornUseRsp.iResult == 4)
			{
				Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_4", true, 1.5f, null, new object[0]);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_1", true, 1f, null, new object[]
				{
					msg.stPkgData.stHornUseRsp.iResult
				});
			}
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
		}

		private void OnTimerReq(int timerSequence)
		{
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				return;
			}
			if (this.speakerList.Count <= 2)
			{
				this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_SMALL, this.m_lastSpeakerBeginSec);
			}
			if (this.loudSpeakerList.Count <= 2)
			{
				this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_BIGER, this.m_lastLoudSpeakerBeginSec);
			}
		}

		private void GetSpeakerMsg(CS_HORN_TYPE type, uint beginSec)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1305u);
			cSPkg.stPkgData.stGetHornMsgReq.bHornType = (byte)type;
			cSPkg.stPkgData.stGetHornMsgReq.dwBeginShowSec = beginSec;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(1306)]
		public static void OnGetSpeakerMsgRsp(CSPkg msg)
		{
			CS_HORN_TYPE bHornType = (CS_HORN_TYPE)msg.stPkgData.stGetHornMsgRsp.bHornType;
			Singleton<CLoudSpeakerSys>.instance.AddSpeakerArray(bHornType, msg.stPkgData.stGetHornMsgRsp.astMsgInfo, (uint)msg.stPkgData.stGetHornMsgRsp.wMsgCnt);
			if (bHornType == CS_HORN_TYPE.CS_HORNTYPE_SMALL && msg.stPkgData.stGetHornMsgRsp.wMsgCnt > 0)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_LobbyChatData_Change");
			}
		}

		public bool IsLoudSpeakerShowing()
		{
			return this.m_timerLoudSpeaker != -1 && this.CurLoudSpeaker != null && (long)CRoleInfo.GetCurrentUTCTime() < (long)((ulong)this.CurLoudSpeaker.dwEndShowSec);
		}

		public bool IsSpeakerShowing()
		{
			return this.m_timerSpeaker != -1 && this.CurSpekaer != null && (long)CRoleInfo.GetCurrentUTCTime() < (long)((ulong)this.CurSpekaer.dwEndShowSec);
		}

		public void ShowLoudSpeaker(COMDT_CHAT_MSG_HORN data)
		{
			this.m_timerLoudSpeaker = Singleton<CTimerManager>.instance.AddTimer(1000, 0, new CTimer.OnTimeUpHandler(this.OnTimerLoudSpeaker));
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				this.loudSpeakerList.Clear();
			}
			else
			{
				CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
				if (form == null)
				{
					return;
				}
				CUIAutoScroller component = form.GetWidget(5).GetComponent<CUIAutoScroller>();
				if (component == null)
				{
					return;
				}
				GameObject widget = form.GetWidget(6);
				if (widget == null)
				{
					return;
				}
				string rawText = UT.Bytes2String(data.szContent);
				string str = CChatUT.Build_4_LoudSpeaker_EntryString(data.stFrom.ullUid, (uint)data.stFrom.iLogicWorldID, rawText);
				component.SetText(CUIUtility.RemoveEmoji(str));
				component.gameObject.CustomSetActive(true);
				widget.CustomSetActive(true);
				component.StopAutoScroll();
				component.StartAutoScroll(true);
				Singleton<CChatController>.instance.view.ShowLoudSpeaker(true, data);
			}
		}

		public void ShowSpeaker(COMDT_CHAT_MSG_HORN data)
		{
			string a = CChatUT.Build_4_Speaker_EntryString(data.stFrom.ullUid, (uint)data.stFrom.iLogicWorldID, UT.Bytes2String(data.szContent));
			this.m_timerSpeaker = Singleton<CTimerManager>.instance.AddTimer(1000, 0, new CTimer.OnTimeUpHandler(this.OnTimerSpeaker));
			if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Normal)
			{
				Singleton<CChatController>.instance.model.sysData.Add_NewContent_Entry_Speaker(a);
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
			}
		}

		private void OnTimerSpeaker(int timerSequence)
		{
			if ((long)CRoleInfo.GetCurrentUTCTime() >= (long)((ulong)this.GetSpeakerEndTime(CS_HORN_TYPE.CS_HORNTYPE_SMALL)))
			{
				Singleton<CTimerManager>.instance.RemoveTimer(timerSequence);
				this.m_timerSpeaker = -1;
				this.CurSpekaer = null;
				this.CurSpekaer = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_SMALL);
				if (this.CurSpekaer != null)
				{
					this.ShowSpeaker(this.CurSpekaer);
				}
				else
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Speaker_EntryNode_TimeUp);
					this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_SMALL, this.m_lastSpeakerBeginSec);
				}
			}
		}

		private uint GetSpeakerEndTime(CS_HORN_TYPE type)
		{
			if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
			{
				if (this.speakerList.Count > 0)
				{
					return this.speakerList[0].dwBeginShowSec;
				}
				if (this.CurSpekaer != null)
				{
					return this.CurSpekaer.dwEndShowSec;
				}
				return 0u;
			}
			else
			{
				if (this.loudSpeakerList.Count > 0)
				{
					return this.loudSpeakerList[0].dwBeginShowSec;
				}
				if (this.CurLoudSpeaker != null)
				{
					return this.CurLoudSpeaker.dwEndShowSec;
				}
				return 0u;
			}
		}

		private void OnTimerLoudSpeaker(int timerSequence)
		{
			if ((long)CRoleInfo.GetCurrentUTCTime() >= (long)((ulong)this.GetSpeakerEndTime(CS_HORN_TYPE.CS_HORNTYPE_BIGER)))
			{
				Singleton<CTimerManager>.instance.RemoveTimer(timerSequence);
				this.m_timerLoudSpeaker = -1;
				this.CurLoudSpeaker = null;
				CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
				if (form == null)
				{
					return;
				}
				CUIAutoScroller component = form.GetWidget(5).GetComponent<CUIAutoScroller>();
				if (component == null)
				{
					return;
				}
				GameObject widget = form.GetWidget(6);
				if (widget == null)
				{
					return;
				}
				component.StopAutoScroll();
				if (Singleton<CChatController>.instance.view != null)
				{
					Singleton<CChatController>.instance.view.ShowLoudSpeaker(false, null);
				}
				this.CurLoudSpeaker = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_BIGER);
				if (this.CurLoudSpeaker == null)
				{
					component.gameObject.CustomSetActive(false);
					widget.CustomSetActive(false);
					this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_BIGER, this.m_lastLoudSpeakerBeginSec);
				}
				else
				{
					component.gameObject.CustomSetActive(true);
					widget.CustomSetActive(true);
					this.ShowLoudSpeaker(this.CurLoudSpeaker);
				}
			}
		}

		private void OnLoudSpeakerTipsOpen()
		{
			if (!this.IsLoudSpeakerShowing())
			{
				this.CurLoudSpeaker = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_BIGER);
				if (this.CurLoudSpeaker == null)
				{
					return;
				}
				if (this.m_timerLoudSpeaker != -1)
				{
					Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerLoudSpeaker);
					this.m_timerLoudSpeaker = -1;
				}
				this.ShowLoudSpeaker(this.CurLoudSpeaker);
			}
		}

		private void OnSpeakerNodeOpen()
		{
			if (!this.IsSpeakerShowing())
			{
				this.CurSpekaer = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_SMALL);
				if (this.CurSpekaer == null)
				{
					return;
				}
				if (this.m_timerSpeaker != -1)
				{
					Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSpeaker);
					this.m_timerSpeaker = -1;
				}
				this.ShowSpeaker(this.CurSpekaer);
			}
		}
	}
}

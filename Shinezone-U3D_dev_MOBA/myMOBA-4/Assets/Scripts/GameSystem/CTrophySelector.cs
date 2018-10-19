using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CTrophySelector : Singleton<CTrophySelector>
	{
		public const string sSelectorFormPath = "UGUI/Form/System/Achieve/Form_Trophy_Select.prefab";

		private ListView<CAchieveItem2> m_CurTrophies;

		public byte TargetReplaceIdx;

		public CAchieveItem2 NewTrophy;

		public CAchieveItem2[] SelectedTrophies;

		public override void Init()
		{
			base.Init();
			this.m_CurTrophies = new ListView<CAchieveItem2>();
			this.SelectedTrophies = new CAchieveItem2[3];
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Change_Selected_Trophy, new CUIEventManager.OnUIEventHandler(this.OnChangeAchievement));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Selector_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Selector_Trophy_Select, new CUIEventManager.OnUIEventHandler(this.OnTrophySelectChange));
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_CurTrophies.RemoveRange(0, this.m_CurTrophies.Count);
			this.m_CurTrophies = null;
			this.SelectedTrophies = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Change_Selected_Trophy, new CUIEventManager.OnUIEventHandler(this.OnChangeAchievement));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Selector_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Selector_Trophy_Select, new CUIEventManager.OnUIEventHandler(this.OnTrophySelectChange));
		}

		private void OnChangeAchievement(CUIEvent uiEvent)
		{
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			ListView<CAchieveItem2> listView = new ListView<CAchieveItem2>();
			this.m_CurTrophies = masterAchieveInfo.GetTrophies(enTrophyState.Finish);
			for (int i = this.m_CurTrophies.Count - 1; i >= 0; i--)
			{
				if (this.m_CurTrophies[i] != null && Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, this.m_CurTrophies[i]) < 0)
				{
					listView.Add(this.m_CurTrophies[i]);
				}
			}
			if (listView.Count == 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_1"), false, 1.5f, null, new object[0]);
				return;
			}
			if (uiEvent.m_eventParams.tag >= 0 && uiEvent.m_eventParams.tag < this.SelectedTrophies.Length)
			{
				this.TargetReplaceIdx = (byte)uiEvent.m_eventParams.tag;
				CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab", false, true);
				this.RefreshAchievementSelectForm(form);
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenTips("数据异常，请稍后重试", false, 1.5f, null, new object[0]);
		}

		private void OnTrophyEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_CurTrophies.Count)
			{
				return;
			}
			CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
			if (cUIListElementScript == null)
			{
				DebugHelper.Assert(false, "achievement selector sery enable elementscript is null");
				return;
			}
			CAchieveItem2 cAchieveItem = this.m_CurTrophies[srcWidgetIndexInBelongedList];
			GameObject widget = cUIListElementScript.GetWidget(0);
			GameObject widget2 = cUIListElementScript.GetWidget(1);
			GameObject widget3 = cUIListElementScript.GetWidget(2);
			GameObject widget4 = cUIListElementScript.GetWidget(3);
			GameObject widget5 = cUIListElementScript.GetWidget(4);
			if (Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, cAchieveItem) >= 0)
			{
				widget2.CustomSetActive(true);
			}
			else
			{
				widget2.CustomSetActive(false);
			}
			Image component = widget.GetComponent<Image>();
			Image component2 = widget5.GetComponent<Image>();
			Text component3 = widget3.GetComponent<Text>();
			Text component4 = widget4.GetComponent<Text>();
			if (component == null || component2 == null || component3 == null || component4 == null)
			{
				return;
			}
			CAchieveItem2 cAchieveItem2 = cAchieveItem.TryToGetMostRecentlyDoneItem();
			if (cAchieveItem2 == null)
			{
				component.SetSprite(CUIUtility.GetSpritePrefeb(cAchieveItem.GetAchieveImagePath(), false, false), false);
				CAchievementSystem.SetAchieveBaseIcon(widget5.transform, cAchieveItem, null);
				component3.text = cAchieveItem.Cfg.szName;
				component4.text = cAchieveItem.GetGotTimeText(false, false);
			}
			else
			{
				component.SetSprite(CUIUtility.GetSpritePrefeb(cAchieveItem2.GetAchieveImagePath(), false, false), false);
				CAchievementSystem.SetAchieveBaseIcon(widget5.transform, cAchieveItem2, null);
				component3.text = cAchieveItem2.Cfg.szName;
				component4.text = cAchieveItem2.GetGotTimeText(false, false);
			}
		}

		private void OnTrophySelectChange(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int selectedIndex = cUIListScript.GetSelectedIndex();
			if (uiEvent.m_srcFormScript == null || selectedIndex < 0 || selectedIndex >= this.m_CurTrophies.Count)
			{
				return;
			}
			CAchieveItem2 cAchieveItem = this.m_CurTrophies[selectedIndex];
			if (cAchieveItem == null)
			{
				return;
			}
			if (Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, cAchieveItem) >= 0)
			{
				return;
			}
			this.NewTrophy = cAchieveItem;
			CAchieveItem2 cAchieveItem2 = this.NewTrophy.TryToGetMostRecentlyDoneItem();
			if (cAchieveItem2 != null)
			{
				this.SendChgAchieveReq(cAchieveItem2.ID, this.TargetReplaceIdx);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(string.Format("{0}{1}", Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_2"), -4), false, 1.5f, null, new object[0]);
			}
		}

		private void RefreshAchievementSelectForm(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			CUIListScript component = form.GetWidget(0).GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_CurTrophies.Count);
		}

		private void SendChgAchieveReq(uint id, byte idx)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4413u);
			cSPkg.stPkgData.stReqAchieveShow.dwAchieveID = id;
			cSPkg.stPkgData.stReqAchieveShow.bIndex = idx;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(4414)]
		public static void ReceiveChgTrophyRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stRspAchieveShow.iResult < 0)
			{
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab");
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_3"), false, 1.5f, null, new object[0]);
			}
			else
			{
				CTrophySelector instance = Singleton<CTrophySelector>.GetInstance();
				Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab");
				Singleton<EventRouter>.GetInstance().BroadCastEvent<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, msg.stPkgData.stRspAchieveShow.bIndex, instance.NewTrophy);
			}
		}
	}
}

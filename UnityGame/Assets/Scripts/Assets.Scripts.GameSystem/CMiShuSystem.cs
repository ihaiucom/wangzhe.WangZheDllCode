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
	public class CMiShuSystem : Singleton<CMiShuSystem>
	{
		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnClickMiShu, new CUIEventManager.OnUIEventHandler(this.OnClickMiShu));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCheckTalk, new CUIEventManager.OnUIEventHandler(this.OnCheckMiShuTalk));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCloseTalk, new CUIEventManager.OnUIEventHandler(this.OnCloseTalk));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGotoEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCheckFirstWin, new CUIEventManager.OnUIEventHandler(this.OnCheckFirstWin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnPlayAnimation, new CUIEventManager.OnUIEventHandler(this.OnPlayMishuAnimation));
		}

		private void InitSysBtn(Button btn, RES_GAME_ENTRANCE_TYPE entryType, GameObject txtObj, GameObject coinTextObj)
		{
			RES_SPECIALFUNCUNLOCK_TYPE type = CUICommonSystem.EntryTypeToUnlockType(entryType);
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(type))
			{
				txtObj.CustomSetActive(false);
				coinTextObj.CustomSetActive(true);
				CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
				btn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int)entryType;
			}
			else
			{
				txtObj.CustomSetActive(true);
				coinTextObj.CustomSetActive(false);
				CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
			}
		}

		private void OnClickGotoEntry(CUIEvent uiEvent)
		{
			RES_GAME_ENTRANCE_TYPE tag = (RES_GAME_ENTRANCE_TYPE)uiEvent.m_eventParams.tag;
			CUICommonSystem.JumpForm(tag, 0, 0, null);
		}

		public void InitList(int TabIndex, CUIListScript list)
		{
			ResMiShuInfo[] resList = this.GetResList(TabIndex);
			list.SetElementAmount(resList.Length);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			for (int i = 0; i < resList.Length; i++)
			{
				Transform transform = list.GetElemenet(i).transform;
				Image component = transform.Find("imgIcon").GetComponent<Image>();
				Text component2 = transform.Find("lblTitle").GetComponent<Text>();
				Text component3 = transform.Find("lblUnLock").GetComponent<Text>();
				Text component4 = transform.Find("lblDesc").GetComponent<Text>();
				Text component5 = transform.Find("lblCoinDesc").GetComponent<Text>();
				Button component6 = transform.Find("btnGoto").GetComponent<Button>();
				component.SetSprite(CUIUtility.s_Sprite_Dynamic_Task_Dir + resList[i].dwIconID, list.m_belongedFormScript, true, false, false, false);
				component2.set_text(resList[i].szName);
				component3.set_text(resList[i].szUnOpenDesc);
				component4.set_text(resList[i].szDesc);
				component5.set_text(string.Empty);
				this.InitSysBtn(component6, (RES_GAME_ENTRANCE_TYPE)resList[i].bGotoID, component3.gameObject, component5.gameObject);
				component5.gameObject.CustomSetActive(false);
			}
		}

		public ResMiShuInfo[] GetResList(int TabIndex)
		{
			List<ResMiShuInfo> list = new List<ResMiShuInfo>();
			GameDataMgr.miShuLib.Reload();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.miShuLib.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResMiShuInfo resMiShuInfo = (ResMiShuInfo)current.get_Value();
				if ((int)resMiShuInfo.bType == TabIndex)
				{
					list.Add(resMiShuInfo);
				}
			}
			return list.ToArray();
		}

		public void CheckMiShuTalk(bool isRestarTimer = true)
		{
			bool flag = false;
			string text = null;
			if (Singleton<CTaskSys>.instance.model.IsShowMainTaskTab_RedDotCount())
			{
				flag = true;
				text = Singleton<CTextManager>.instance.GetText("Secretary_Reward_Tips");
			}
			else
			{
				CTask maxIndex_TaskID_InState = Singleton<CTaskSys>.instance.model.GetMaxIndex_TaskID_InState(enTaskTab.TAB_USUAL, CTask.State.Have_Done);
				if (maxIndex_TaskID_InState != null)
				{
					flag = true;
					text = Singleton<CTextManager>.instance.GetText("Secretary_Reward_Tips");
				}
				else
				{
					maxIndex_TaskID_InState = Singleton<CTaskSys>.instance.model.GetMaxIndex_TaskID_InState(enTaskTab.TAB_USUAL, CTask.State.OnGoing);
					if (maxIndex_TaskID_InState != null)
					{
						flag = true;
						text = maxIndex_TaskID_InState.m_resTask.szMiShuDesc;
					}
				}
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("LobbyBottom/Newbie/TalkFrame");
			Text component = form.transform.Find("LobbyBottom/Newbie/TalkFrame/Text").GetComponent<Text>();
			CUITimerScript component2 = form.transform.Find("LobbyBottom/Newbie/TalkFrame/Timer").GetComponent<CUITimerScript>();
			if (flag)
			{
				transform.gameObject.CustomSetActive(true);
				component.set_text(text);
				component2.ReStartTimer();
			}
			else
			{
				transform.gameObject.CustomSetActive(false);
				component2.EndTimer();
			}
			if (isRestarTimer)
			{
				CUITimerScript component3 = form.transform.Find("LobbyBottom/Newbie/Timer").GetComponent<CUITimerScript>();
				component3.ReStartTimer();
			}
		}

		public void OnPlayMishuAnimation(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("LobbyBottom/Newbie/Image_DaJi");
			if (transform != null)
			{
				CUICommonSystem.PlayAnimator(transform.gameObject, "Blink_0" + Random.Range(1, 3));
			}
		}

		public void OnCheckMiShuTalk(CUIEvent uiEvent)
		{
			this.CheckMiShuTalk(true);
		}

		public void OnCloseTalk(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform.Find("LobbyBottom/Newbie/TalkFrame");
			CUITimerScript component = form.transform.Find("LobbyBottom/Newbie/TalkFrame/Timer").GetComponent<CUITimerScript>();
			transform.gameObject.CustomSetActive(false);
			component.EndTimer();
		}

		private void OnClickMiShu(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Task_OpenForm;
			cUIEvent.m_eventParams.tag = 1;
			if (Singleton<CTaskSys>.instance.model.IsShowMainTaskTab_RedDotCount())
			{
				cUIEvent.m_eventParams.tag = 0;
			}
			Singleton<CUIEventManager>.instance.DispatchUIEvent(cUIEvent);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_MishuBtn);
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterMishu, new uint[0]);
		}

		public void OnCheckFirstWin(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form == null || masterRoleInfo == null)
			{
				return;
			}
			Transform transform = form.transform.Find("Award");
			Text component = transform.Find("lblFirstWin").GetComponent<Text>();
			Image component2 = transform.Find("Icon").GetComponent<Image>();
			CUITimerScript component3 = transform.Find("Timer").GetComponent<CUITimerScript>();
			if (!masterRoleInfo.IsFirstWinOpen())
			{
				transform.gameObject.CustomSetActive(false);
				return;
			}
			transform.gameObject.CustomSetActive(true);
			float num = (float)masterRoleInfo.GetCurFirstWinRemainingTimeSec();
			if (num <= 0f)
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictory"));
				component2.set_color(CUIUtility.s_Color_White);
				component3.gameObject.CustomSetActive(false);
			}
			else
			{
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictoryCD"));
				component2.set_color(CUIUtility.s_Color_GrayShader);
				component3.gameObject.CustomSetActive(true);
				component3.SetTotalTime(num);
				component3.StartTimer();
			}
		}

		public void CheckActPlayModeTipsForLobby()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form && !CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = form.transform.Find("BtnCon/PvpBtn/ActModePanel");
				if (transform)
				{
					transform.SetAsLastSibling();
					transform.gameObject.CustomSetActive(this.IsHaveEntertrainModeOpen());
				}
			}
		}

		public void CheckActPlayModeTipsForPvpEntry()
		{
			uint num = 0u;
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), ref num);
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form && !CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = form.transform.Find("panelGroup1/btnGroup/ButtonEntertain/ActModePanel");
				if (transform)
				{
					transform.gameObject.CustomSetActive(this.IsHaveEntertrainModeOpen());
				}
			}
			if (form)
			{
				CUIMiniEventScript component = form.transform.FindChild("panelGroup5/btnGroup/Button1").GetComponent<CUIMiniEventScript>();
				CUIMiniEventScript component2 = form.transform.FindChild("panelGroup5/btnGroup/Button3").GetComponent<CUIMiniEventScript>();
				CUIMiniEventScript component3 = form.transform.FindChild("panelGroup5/btnGroup/Button4").GetComponent<CUIMiniEventScript>();
				this.refreshEntertainMentOpenStateUI(component.gameObject, component.m_onClickEventParams.tagUInt);
				this.refreshEntertainMentOpenStateUI(component2.gameObject, component2.m_onClickEventParams.tagUInt);
				this.refreshEntertainMentOpenStateUI(component3.gameObject, component3.m_onClickEventParams.tagUInt);
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					component3.gameObject.CustomSetActive(false);
				}
			}
		}

		public void refreshEntertainMentOpenStateUI(GameObject btn, uint mapId)
		{
			if (btn == null)
			{
				return;
			}
			Transform transform = btn.transform;
			if (transform)
			{
				Transform transform2 = transform.FindChild("Lock");
				Transform transform3 = transform.FindChild("Open");
				Transform transform4 = transform.FindChild("NotOpen");
				stMatchOpenInfo matchOpenState = CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_ENTERTAINMENT, mapId);
				CUIMiniEventScript component = transform.GetComponent<CUIMiniEventScript>();
				if (transform2 && transform3 && component)
				{
					transform.gameObject.CustomSetActive(false);
					transform2.gameObject.CustomSetActive(false);
					transform3.gameObject.CustomSetActive(false);
					if (transform4)
					{
						transform4.gameObject.CustomSetActive(false);
					}
					component.m_onClickEventParams.commonBool = false;
					if (matchOpenState.matchState == enMatchOpenState.enMatchOpen_InActiveTime)
					{
						CUICommonSystem.SetTextContent(transform3.Find("Text").gameObject, matchOpenState.descStr);
						transform3.gameObject.CustomSetActive(true);
						transform.gameObject.CustomSetActive(true);
						component.m_onClickEventParams.commonBool = true;
						if (CSysDynamicBlock.bLobbyEntryBlocked)
						{
							Transform transform5 = transform3.FindChild("Image");
							Transform transform6 = transform3.FindChild("TextOpen");
							if (transform5 && transform6)
							{
								transform5.gameObject.CustomSetActive(false);
								transform6.gameObject.CustomSetActive(false);
							}
						}
					}
					else if (matchOpenState.matchState == enMatchOpenState.enMatchOpen_NotInActiveTime)
					{
						CUICommonSystem.SetTextContent(transform2.Find("Text").gameObject, matchOpenState.descStr);
						transform2.gameObject.CustomSetActive(true);
						transform.gameObject.CustomSetActive(true);
					}
					else if (transform4)
					{
						transform.gameObject.CustomSetActive(true);
						transform4.gameObject.CustomSetActive(true);
					}
				}
			}
		}

		public bool IsHaveEntertrainModeOpen()
		{
			uint[] array = new uint[3];
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), ref array[0]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Clone"), ref array[1]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5CD"), ref array[2]);
			bool result = false;
			for (int i = 0; i < 3; i++)
			{
				if (CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_ENTERTAINMENT, array[i]).matchState == enMatchOpenState.enMatchOpen_InActiveTime)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static void SendUIClickToServer(enUIClickReprotID clickID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5002u);
			cSPkg.stPkgData.stCltActionStatistics.bActionType = 1;
			cSPkg.stPkgData.stCltActionStatistics.stActionData.construct((long)cSPkg.stPkgData.stCltActionStatistics.bActionType);
			cSPkg.stPkgData.stCltActionStatistics.stActionData.stSecretary.iID = (int)clickID;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void SendReqCoinGetPathData()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1282u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}
	}
}

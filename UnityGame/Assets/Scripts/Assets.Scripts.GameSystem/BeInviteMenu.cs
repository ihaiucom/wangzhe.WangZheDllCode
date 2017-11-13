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
	public class BeInviteMenu
	{
		private GameObject listNode;

		private CUIFormScript form;

		private CUIListScript listScript;

		private InputField m_inputFiled;

		private CUITimerScript timerScript;

		private GameObject m_sendBtn;

		private List<string> m_configTexts = new List<string>();

		public void Clear()
		{
			this.listNode = null;
			this.form = null;
			this.listScript = null;
			this.m_inputFiled = null;
			this.timerScript = null;
			this.m_sendBtn = null;
		}

		private void InitEvt()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ClickDown, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ClickList, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickList));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_Send, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_Send));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Form_Closed, new CUIEventManager.OnUIEventHandler(this.On_Invite_Form_Closed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ItemEnable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_BgClick, new CUIEventManager.OnUIEventHandler(this.ShowRefuseSendState));
		}

		private void UnInitEvt()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_ClickDown, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_ClickList, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickList));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_Send, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_Send));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Form_Closed, new CUIEventManager.OnUIEventHandler(this.On_Invite_Form_Closed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_BgClick, new CUIEventManager.OnUIEventHandler(this.ShowRefuseSendState));
		}

		public void LoadConfig()
		{
			bool flag = true;
			int num = 0;
			while (flag)
			{
				string text = string.Format("Invite_Refuse_Reason_{0}", num);
				string text2 = Singleton<CTextManager>.instance.GetText(text);
				if (!string.Equals(text, text2))
				{
					num++;
					if (!this.m_configTexts.Contains(text2))
					{
						this.m_configTexts.Add(text2);
					}
				}
				else
				{
					flag = false;
				}
			}
			if (this.m_configTexts.get_Count() == 0)
			{
				this.m_configTexts.Add("need config");
			}
		}

		public void Open(SCPKG_INVITE_JOIN_GAME_REQ info)
		{
			this.InitEvt();
			this.ShowNormal(info);
			if (this.listScript != null)
			{
				int count = this.m_configTexts.get_Count();
				this.listScript.SetElementAmount(count);
			}
		}

		public void ShowNormal(SCPKG_INVITE_JOIN_GAME_REQ info)
		{
			string text = CUIUtility.RemoveEmoji(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szName));
			stUIEventParams stUIEventParams = default(stUIEventParams);
			stUIEventParams.tag = (int)info.bIndex;
			int num = 15;
			int.TryParse(Singleton<CTextManager>.instance.GetText("MessageBox_Close_Time"), ref num);
			this.form = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_BeInvited.prefab"), false, true);
			GameObject gameObject = this.form.transform.Find("Panel/Panel/normal").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
			}
			GameObject gameObject2 = this.form.transform.Find("Panel/Panel/refuse").gameObject;
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(false);
			}
			this.listNode = this.form.transform.Find("Panel/Panel/refuse/reasonPanel/DropList").gameObject;
			this.listScript = this.listNode.transform.Find("List").GetComponent<CUIListScript>();
			this.m_inputFiled = this.form.transform.Find("Panel/Panel/refuse/reasonPanel/InputField").GetComponent<InputField>();
			this.m_sendBtn = this.form.transform.Find("Panel/Panel/refuse/btnGroup/Button_Send").gameObject;
			this.form.transform.Find("Panel/Panel/refuse/MatchInfo").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Invite_Refuse_Title"));
			if (this.form != null)
			{
				string text2 = null;
				string text3 = null;
				if (info.bInviteType == 1)
				{
					ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stRoomDetail.bMapType, info.stInviteDetail.stRoomDetail.dwMapId);
					stUIEventParams.heroId = (uint)info.stInviteDetail.stRoomDetail.bMapType;
					stUIEventParams.weakGuideId = info.stInviteDetail.stRoomDetail.dwMapId;
					if (pvpMapCommonInfo != null)
					{
						text2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
						{
							((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
							((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
							Utility.UTF8Convert(pvpMapCommonInfo.szName)
						});
					}
					text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
				}
				else if (info.bInviteType == 2)
				{
					ResDT_LevelCommonInfo pvpMapCommonInfo2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stTeamDetail.bMapType, info.stInviteDetail.stTeamDetail.dwMapId);
					stUIEventParams.heroId = (uint)info.stInviteDetail.stTeamDetail.bMapType;
					stUIEventParams.weakGuideId = info.stInviteDetail.stTeamDetail.dwMapId;
					if (pvpMapCommonInfo2 != null)
					{
						text2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
						{
							((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
							((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
							Utility.UTF8Convert(pvpMapCommonInfo2.szName)
						});
					}
					if (info.stInviteDetail.stTeamDetail.bMapType == 3)
					{
						text3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
					}
					else
					{
						text3 = Singleton<CTextManager>.GetInstance().GetText((info.stInviteDetail.stTeamDetail.bPkAI == 1) ? "Invite_Match_Type_2" : "Invite_Match_Type_3");
					}
				}
				string text4 = Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", new string[]
				{
					text3,
					text2
				});
				this.form.m_formWidgets[8].GetComponent<Text>().set_text(text4);
				uint dwRelationMask = info.stInviterInfo.dwRelationMask;
				string text5;
				if ((dwRelationMask & 1u) > 0u)
				{
					text5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
					stUIEventParams.tag2 = 0;
				}
				else if ((dwRelationMask & 2u) > 0u)
				{
					text5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
					stUIEventParams.tag2 = 1;
				}
				else if ((dwRelationMask & 4u) > 0u)
				{
					text5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
					stUIEventParams.tag2 = 2;
					CSDT_LBS_USER_INFO lBSUserInfo = Singleton<CFriendContoller>.instance.model.GetLBSUserInfo(info.stInviterInfo.ullUid, info.stInviterInfo.dwLogicWorldID, CFriendModel.LBSGenderType.Both);
					if (lBSUserInfo != null)
					{
						stUIEventParams.tagUInt = lBSUserInfo.dwGameSvrEntity;
					}
				}
				else
				{
					text5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
					stUIEventParams.tag2 = 3;
				}
				string text6 = string.Format(Singleton<CTextManager>.instance.GetText("Be_Invited_FromType"), text5);
				this.form.m_formWidgets[6].GetComponent<Text>().set_text(text6);
				stUIEventParams.tagStr = string.Format("<color=#FFFFFF>{0}</color> {1} {2}", text, text4, text6);
				stUIEventParams.commonUInt64Param1 = info.stInviterInfo.ullUid;
				stUIEventParams.taskId = info.stInviterInfo.dwLogicWorldID;
				stUIEventParams.tag3 = (int)info.bInviteType;
				if (num != 0)
				{
					Transform transform = this.form.transform.Find("closeTimer");
					if (transform != null)
					{
						this.timerScript = transform.GetComponent<CUITimerScript>();
						if (this.timerScript != null)
						{
							this.timerScript.enabled = true;
							this.timerScript.SetTotalTime((float)num);
							this.timerScript.StartTimer();
							this.timerScript.m_eventIDs[1] = enUIEventID.Invite_TimeOut;
							this.timerScript.m_eventParams[1] = stUIEventParams;
						}
					}
				}
				this.form.m_formWidgets[0].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_RefuseReason_Send, stUIEventParams);
				this.form.m_formWidgets[1].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_AcceptInvite, stUIEventParams);
				this.form.m_formWidgets[5].GetComponent<Text>().set_text(text);
				COM_SNSGENDER bGender = (COM_SNSGENDER)info.stInviterInfo.bGender;
				Image component = this.form.m_formWidgets[4].GetComponent<Image>();
				component.gameObject.CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
				if (bGender == COM_SNSGENDER.COM_SNSGENDER_MALE)
				{
					CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				}
				else if (bGender == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
				{
					CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
				}
				GameObject widget = this.form.GetWidget(9);
				UT.SetMentorLv(widget, (int)info.stInviterInfo.dwMasterLevel);
				CUIHttpImageScript component2 = this.form.m_formWidgets[3].GetComponent<CUIHttpImageScript>();
				component2.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szHeadUrl)));
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(this.form.m_formWidgets[2].GetComponent<Image>(), (int)info.stInviterInfo.dwHeadImgId);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(this.form.m_formWidgets[2].GetComponent<Image>(), (int)info.stInviterInfo.dwHeadImgId, this.form, 1f, false);
				this.form.m_formWidgets[7].CustomSetActive(info.stInviterInfo.bGradeOfRank > 0);
				if (info.stInviterInfo.bGradeOfRank > 0)
				{
					CLadderView.ShowRankDetail(this.form.m_formWidgets[7], info.stInviterInfo.bShowGradeOfRank, 0u, 1u, false, true, false, true, true);
				}
			}
		}

		public void ShowRefuse()
		{
			if (this.timerScript != null)
			{
				this.timerScript.enabled = false;
			}
			this.ShowRefuseSendState(null);
		}

		private void ShowRefuseSendState(CUIEvent uievt = null)
		{
			GameObject gameObject = this.form.transform.Find("Panel/Panel/normal").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
			GameObject gameObject2 = this.form.transform.Find("Panel/Panel/refuse").gameObject;
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(true);
			}
			this.m_sendBtn.CustomSetActive(true);
			this.listNode.CustomSetActive(false);
			this.m_inputFiled.enabled = true;
			if (string.IsNullOrEmpty(this.m_inputFiled.get_text()))
			{
				this.m_inputFiled.set_text(this.m_configTexts.get_Item(0));
			}
		}

		private void ShowRefuseListState()
		{
			GameObject gameObject = this.form.transform.Find("Panel/Panel/normal").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
			GameObject gameObject2 = this.form.transform.Find("Panel/Panel/refuse").gameObject;
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(true);
			}
			this.m_sendBtn.CustomSetActive(false);
			this.listNode.CustomSetActive(true);
			this.m_inputFiled.enabled = false;
		}

		public void On_InBattleMsg_ListElement_Enable(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_configTexts.get_Count())
			{
				return;
			}
			string text = this.m_configTexts.get_Item(srcWidgetIndexInBelongedList);
			if (text == null)
			{
				return;
			}
			Text component = uievent.m_srcWidget.transform.Find("Text").GetComponent<Text>();
			if (component != null && text != null)
			{
				component.set_text(text);
			}
		}

		private void On_Invite_Form_Closed(CUIEvent uievent)
		{
			this.UnInitEvt();
		}

		private void On_Invite_RefuseReason_Send(CUIEvent uievent)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2021u);
			cSPkg.stPkgData.stInviteJoinGameRsp.bIndex = (byte)uievent.m_eventParams.tag;
			cSPkg.stPkgData.stInviteJoinGameRsp.bResult = 14;
			string str = CUIUtility.RemoveEmoji(this.m_inputFiled.get_text());
			StringHelper.StringToUTF8Bytes(str, ref cSPkg.stPkgData.stInviteJoinGameRsp.szDenyReason);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<WatchController>.GetInstance().Stop();
			Singleton<CMailSys>.instance.AddFriendInviteMail(uievent, CMailSys.enProcessInviteType.Refuse);
		}

		private void On_Invite_RefuseReason_ClickList(CUIEvent uievent)
		{
			this.ShowRefuseSendState(null);
			string text = this.m_configTexts.get_Item(uievent.m_srcWidgetIndexInBelongedList);
			this.m_inputFiled.set_text(text);
		}

		private void On_Invite_RefuseReason_ClickDown(CUIEvent uievent)
		{
			GameObject gameObject = this.form.transform.Find("Panel/Panel/refuse").gameObject;
			if (gameObject != null)
			{
				if (this.listNode != null && this.listNode.gameObject.activeInHierarchy)
				{
					this.ShowRefuseSendState(null);
				}
				else
				{
					this.ShowRefuseListState();
				}
			}
			else
			{
				this.ShowRefuseListState();
			}
		}
	}
}

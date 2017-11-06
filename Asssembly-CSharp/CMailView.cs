using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CMailView
{
	private CUIFormScript m_CUIForm;

	private CUIListScript m_CUIListScriptTab;

	private GameObject m_panelFri;

	private GameObject m_panelSys;

	private GameObject m_panelMsg;

	private GameObject m_panelAskFor;

	private int m_friUnReadNum;

	private int m_sysUnReadNum;

	private int m_msgUnReadNum;

	private int m_askForUnReadNum;

	private GameObject m_SysDeleteBtn;

	private GameObject m_AskForDeleteBtn;

	private GameObject m_allReceiveSysButton;

	private GameObject m_allReceiveFriButton;

	private GameObject m_allDeleteMsgCenterButton;

	private CustomMailType m_curMailtype = CustomMailType.SYSTEM;

	public CustomMailType CurMailType
	{
		get
		{
			return this.m_curMailtype;
		}
		set
		{
			if (this.m_CUIListScriptTab == null)
			{
				return;
			}
			this.m_curMailtype = value;
			if (this.m_curMailtype == CustomMailType.FRIEND)
			{
				this.SetActiveTab(0);
				this.m_CUIListScriptTab.SelectElement(0, true);
			}
			else if (this.m_curMailtype == CustomMailType.SYSTEM)
			{
				this.SetActiveTab(1);
				this.m_CUIListScriptTab.SelectElement(1, true);
			}
			else if (this.m_curMailtype == CustomMailType.FRIEND_INVITE)
			{
				this.SetActiveTab(2);
				this.m_CUIListScriptTab.SelectElement(2, true);
			}
			else if (this.m_curMailtype == CustomMailType.ASK_FOR)
			{
				this.SetActiveTab(3);
				this.m_CUIListScriptTab.SelectElement(3, true);
			}
		}
	}

	public void Open(CustomMailType mailType)
	{
		this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_FORM_PATH, false, true);
		if (this.m_CUIForm == null)
		{
			return;
		}
		this.m_CUIListScriptTab = this.m_CUIForm.transform.FindChild("TopCommon/Panel_Menu/ListMenu").GetComponent<CUIListScript>();
		this.m_CUIListScriptTab.SetElementAmount(4);
		this.m_CUIListScriptTab.GetElemenet(0).transform.FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_Friend"));
		this.m_CUIListScriptTab.GetElemenet(1).transform.FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_System"));
		this.m_CUIListScriptTab.GetElemenet(2).transform.FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_MsgCenter"));
		this.m_CUIListScriptTab.GetElemenet(3).transform.FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_Ask_For_Tab"));
		this.m_CUIListScriptTab.GetElemenet(0).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabFriend);
		this.m_CUIListScriptTab.GetElemenet(1).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabSystem);
		this.m_CUIListScriptTab.GetElemenet(2).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabMsgCenter);
		this.m_CUIListScriptTab.GetElemenet(3).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabGiftCenter);
		this.m_panelFri = this.m_CUIForm.transform.FindChild("PanelFriMail").gameObject;
		this.m_panelSys = this.m_CUIForm.transform.FindChild("PanelSysMail").gameObject;
		this.m_panelMsg = this.m_CUIForm.transform.FindChild("PanelMsgMail").gameObject;
		this.m_panelAskFor = this.m_CUIForm.transform.FindChild("PanelAskForMail").gameObject;
		this.m_SysDeleteBtn = this.m_panelSys.transform.FindChild("ButtonGrid/DeleteButton").gameObject;
		this.m_AskForDeleteBtn = this.m_panelAskFor.transform.FindChild("ButtonGrid/DeleteButton").gameObject;
		this.m_allReceiveSysButton = this.m_panelSys.transform.FindChild("ButtonGrid/AllReceiveButton").gameObject;
		this.m_allReceiveFriButton = this.m_panelFri.transform.FindChild("AllReceiveButton").gameObject;
		this.m_allDeleteMsgCenterButton = this.m_panelMsg.transform.FindChild("AllDeleteButton").gameObject;
		this.SetUnReadNum(CustomMailType.FRIEND, this.m_friUnReadNum);
		this.SetUnReadNum(CustomMailType.SYSTEM, this.m_sysUnReadNum);
		this.SetUnReadNum(CustomMailType.FRIEND_INVITE, this.m_msgUnReadNum);
		this.SetUnReadNum(CustomMailType.ASK_FOR, this.m_askForUnReadNum);
		this.CurMailType = mailType;
	}

	public void OnClose()
	{
		this.m_CUIForm = null;
		this.m_CUIListScriptTab = null;
		this.m_panelFri = null;
		this.m_panelSys = null;
		this.m_panelMsg = null;
		this.m_SysDeleteBtn = null;
		this.m_allReceiveSysButton = null;
		this.m_allReceiveFriButton = null;
		this.m_allDeleteMsgCenterButton = null;
	}

	public void SetActiveTab(int index)
	{
		if (index == 0)
		{
			this.m_panelFri.CustomSetActive(true);
			this.m_panelSys.CustomSetActive(false);
			this.m_panelMsg.CustomSetActive(false);
			this.m_panelAskFor.CustomSetActive(false);
		}
		else if (index == 1)
		{
			this.m_panelFri.CustomSetActive(false);
			this.m_panelSys.CustomSetActive(true);
			this.m_panelMsg.CustomSetActive(false);
			this.m_panelAskFor.CustomSetActive(false);
		}
		else if (index == 2)
		{
			this.m_panelFri.CustomSetActive(false);
			this.m_panelSys.CustomSetActive(false);
			this.m_panelMsg.CustomSetActive(true);
			this.m_panelAskFor.CustomSetActive(false);
		}
		else if (index == 3)
		{
			this.m_panelFri.CustomSetActive(false);
			this.m_panelSys.CustomSetActive(false);
			this.m_panelMsg.CustomSetActive(false);
			this.m_panelAskFor.CustomSetActive(true);
		}
	}

	public void SetUnReadNum(CustomMailType mailtype, int unReadNum)
	{
		if (mailtype == CustomMailType.FRIEND)
		{
			this.m_friUnReadNum = unReadNum;
		}
		else if (mailtype == CustomMailType.SYSTEM)
		{
			this.m_sysUnReadNum = unReadNum;
		}
		else if (mailtype == CustomMailType.FRIEND_INVITE)
		{
			this.m_msgUnReadNum = unReadNum;
		}
		else if (mailtype == CustomMailType.ASK_FOR)
		{
			this.m_askForUnReadNum = unReadNum;
		}
		if (this.m_CUIListScriptTab == null)
		{
			return;
		}
		if (mailtype == CustomMailType.FRIEND && this.m_CUIListScriptTab.GetElemenet(0) != null)
		{
			if (unReadNum > 9)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else if (unReadNum > 0)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject, enRedDotPos.enTopRight, unReadNum, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(0).gameObject);
			}
		}
		else if (mailtype == CustomMailType.SYSTEM && this.m_CUIListScriptTab.GetElemenet(1) != null)
		{
			if (unReadNum > 9)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else if (unReadNum > 0)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject, enRedDotPos.enTopRight, unReadNum, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(1).gameObject);
			}
		}
		else if (mailtype == CustomMailType.FRIEND_INVITE && this.m_CUIListScriptTab.GetElemenet(2) != null)
		{
			if (unReadNum > 0)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(2).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(2).gameObject);
			}
		}
		else if (mailtype == CustomMailType.ASK_FOR && this.m_CUIListScriptTab.GetElemenet(3) != null)
		{
			if (unReadNum > 9)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(3).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
			}
			else if (unReadNum > 0)
			{
				CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(3).gameObject, enRedDotPos.enTopRight, unReadNum, 0, 0);
			}
			else
			{
				CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(3).gameObject);
			}
		}
	}

	public void UpdateMailList(CustomMailType mailtype, ListView<CMail> mailList)
	{
		if (this.m_CUIForm == null || mailList == null)
		{
			return;
		}
		CUIListElementScript cUIListElementScript = null;
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		int num = -1;
		if (mailtype == CustomMailType.FRIEND)
		{
			CUIListScript component = this.m_CUIForm.transform.FindChild("PanelFriMail/List").GetComponent<CUIListScript>();
			component.SetElementAmount(mailList.Count);
			for (int i = 0; i < mailList.Count; i++)
			{
				cUIListElementScript = component.GetElemenet(i);
				if (cUIListElementScript != null && cUIListElementScript.gameObject)
				{
					this.UpdateListElenment(cUIListElementScript.gameObject, mailList[i]);
				}
				if (num == -1 && mailList[i].subType == 1)
				{
					num = i;
				}
			}
			this.m_allReceiveFriButton.CustomSetActive(num >= 0);
		}
		else if (mailtype == CustomMailType.SYSTEM)
		{
			CUIListScript component2 = this.m_CUIForm.transform.FindChild("PanelSysMail/List").GetComponent<CUIListScript>();
			component2.SetElementAmount(mailList.Count);
			for (int j = 0; j < mailList.Count; j++)
			{
				if (cUIListElementScript != null && cUIListElementScript.gameObject)
				{
					this.UpdateListElenment(cUIListElementScript.gameObject, mailList[j]);
				}
				if (num == -1 && mailList[j].subType == 2)
				{
					num = j;
				}
			}
			this.m_allReceiveSysButton.CustomSetActive(num >= 0);
			this.m_SysDeleteBtn.CustomSetActive(mailList.Count > 0);
		}
		else if (mailtype == CustomMailType.FRIEND_INVITE)
		{
			CUIListScript component3 = this.m_CUIForm.transform.FindChild("PanelMsgMail/List").GetComponent<CUIListScript>();
			component3.SetElementAmount(mailList.Count);
			for (int k = 0; k < mailList.Count; k++)
			{
				if (cUIListElementScript != null && cUIListElementScript.gameObject)
				{
					this.UpdateListElenment(cUIListElementScript.gameObject, mailList[k]);
				}
			}
			this.m_allDeleteMsgCenterButton.CustomSetActive(mailList.Count > 0);
		}
		else if (mailtype == CustomMailType.ASK_FOR)
		{
			GameObject gameObject = Utility.FindChild(this.m_CUIForm.gameObject, "PanelAskForMail/NoAskFor");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (gameObject != null)
				{
					gameObject.GetComponent<Toggle>().set_isOn(masterRoleInfo.IsNoAskFor);
				}
			}
			else
			{
				DebugHelper.Assert(false, "Master Role Info is null");
				if (gameObject != null)
				{
					gameObject.GetComponent<Toggle>().set_isOn(false);
				}
			}
			CUIListScript component4 = this.m_CUIForm.transform.FindChild("PanelAskForMail/List").GetComponent<CUIListScript>();
			component4.SetElementAmount(mailList.Count);
			this.m_allDeleteMsgCenterButton.CustomSetActive(false);
			this.m_allReceiveSysButton.CustomSetActive(false);
			this.m_AskForDeleteBtn.CustomSetActive(mailList.Count > 0);
		}
	}

	public void UpdateListElenment(GameObject element, CMail mail)
	{
		int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
		Text componetInChild = Utility.GetComponetInChild<Text>(element, "Title");
		Text componetInChild2 = Utility.GetComponetInChild<Text>(element, "MailTime");
		GameObject obj = Utility.FindChild(element, "New");
		GameObject obj2 = Utility.FindChild(element, "ReadMailIcon");
		GameObject obj3 = Utility.FindChild(element, "UnReadMailIcon");
		GameObject obj4 = Utility.FindChild(element, "CoinImg");
		Text componetInChild3 = Utility.GetComponetInChild<Text>(element, "From");
		CUIHttpImageScript componetInChild4 = Utility.GetComponetInChild<CUIHttpImageScript>(element, "HeadBg/imgHead");
		GameObject obj5 = null;
		Text text = null;
		GameObject gameObject = Utility.FindChild(element, "OnlineBg");
		if (gameObject != null)
		{
			obj5 = gameObject.gameObject;
		}
		GameObject gameObject2 = Utility.FindChild(element, "Online");
		if (gameObject2 != null)
		{
			text = gameObject2.GetComponent<Text>();
		}
		componetInChild.set_text(mail.subject);
		componetInChild2.set_text(Utility.GetTimeBeforString((long)((ulong)mail.sendTime), (long)currentUTCTime));
		bool flag = mail.mailState == COM_MAIL_STATE.COM_MAIL_UNREAD;
		obj.CustomSetActive(flag);
		if (mail.mailType == CustomMailType.SYSTEM)
		{
			obj2.CustomSetActive(!flag);
			obj3.CustomSetActive(flag);
			componetInChild3.set_text(string.Empty);
			componetInChild4.gameObject.CustomSetActive(false);
			obj4.CustomSetActive(false);
			obj5.CustomSetActive(false);
			if (text != null)
			{
				text.gameObject.CustomSetActive(false);
			}
		}
		else if (mail.mailType == CustomMailType.FRIEND)
		{
			obj5.CustomSetActive(false);
			if (text != null)
			{
				text.gameObject.CustomSetActive(false);
			}
			obj2.CustomSetActive(false);
			obj3.CustomSetActive(false);
			componetInChild3.set_text(mail.from);
			componetInChild4.gameObject.CustomSetActive(true);
			if (mail.subType == 3)
			{
				obj4.CustomSetActive(false);
				componetInChild4.SetImageSprite(CGuildHelper.GetGuildHeadPath(), this.m_CUIForm);
			}
			else
			{
				obj4.CustomSetActive(true);
				if (!CSysDynamicBlock.bFriendBlocked)
				{
					COMDT_FRIEND_INFO friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.GameFriend);
					if (friendByName == null)
					{
						friendByName = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.SNS);
					}
					if (friendByName != null)
					{
						string url = Utility.UTF8Convert(friendByName.szHeadUrl);
						componetInChild4.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
					}
				}
			}
		}
		else if (mail.mailType == CustomMailType.FRIEND_INVITE)
		{
			obj5.CustomSetActive(true);
			if (text != null)
			{
				text.gameObject.CustomSetActive(true);
			}
			obj2.CustomSetActive(false);
			obj3.CustomSetActive(false);
			componetInChild3.set_text(string.Empty);
			componetInChild4.gameObject.CustomSetActive(true);
			obj4.CustomSetActive(false);
			Transform transform = element.transform.FindChild("invite_btn");
			GameObject obj6 = null;
			if (transform != null)
			{
				obj6 = transform.gameObject;
			}
			if (mail.relationType == 1)
			{
				GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(mail.uid);
				Singleton<CMailSys>.instance.AddGuildMemInfo(guildMemberInfoByUid);
			}
			this.SetEventParams(element, mail);
			string text2;
			string url2;
			bool flag2 = !this.GetOtherPlayerState((COM_INVITE_RELATION_TYPE)mail.relationType, mail.uid, mail.dwLogicWorldID, out text2, out url2);
			string processTypeString = this.GetProcessTypeString((CMailSys.enProcessInviteType)mail.processType);
			componetInChild.set_text(string.Format("{0} {1}", mail.subject, processTypeString));
			if (text != null)
			{
				text.set_text(text2);
			}
			componetInChild4.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url2));
			if (flag2)
			{
				CUIUtility.GetComponentInChildren<Image>(componetInChild4.gameObject).set_color(CUIUtility.s_Color_GrayShader);
			}
			else
			{
				CUIUtility.GetComponentInChildren<Image>(componetInChild4.gameObject).set_color(CUIUtility.s_Color_Full);
			}
			obj6.CustomSetActive(!flag2);
		}
		else if (mail.mailType == CustomMailType.ASK_FOR)
		{
			obj2.CustomSetActive(false);
			obj3.CustomSetActive(false);
			componetInChild3.set_text(mail.from);
			componetInChild4.gameObject.CustomSetActive(true);
			obj4.CustomSetActive(false);
			obj5.CustomSetActive(false);
			if (text != null)
			{
				text.gameObject.CustomSetActive(false);
			}
			if (!CSysDynamicBlock.bFriendBlocked)
			{
				CFriendModel.FriendType friendType = CFriendModel.FriendType.GameFriend;
				COMDT_FRIEND_INFO friendByName2 = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.GameFriend);
				if (friendByName2 == null)
				{
					friendType = CFriendModel.FriendType.SNS;
					friendByName2 = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.SNS);
				}
				if (friendByName2 != null)
				{
					string url3 = Utility.UTF8Convert(friendByName2.szHeadUrl);
					componetInChild4.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url3));
					UT.ShowFriendData(friendByName2, element.GetComponent<FriendShower>(), FriendShower.ItemType.Normal, false, friendType, Singleton<CUIManager>.GetInstance().GetForm(CMailSys.MAIL_FORM_PATH), true);
				}
			}
		}
	}

	private void SetEventParams(GameObject node, CMail mail)
	{
		if (node == null || mail == null)
		{
			return;
		}
		Transform transform = node.transform.FindChild("invite_btn");
		if (transform == null)
		{
			return;
		}
		CUIEventScript component = transform.GetComponent<CUIEventScript>();
		if (component == null)
		{
			return;
		}
		this.SetEventParams(component, mail);
	}

	private void SetEventParams(CUIEventScript com, CMail mail)
	{
		com.m_onClickEventParams.heroId = (uint)mail.bMapType;
		com.m_onClickEventParams.weakGuideId = mail.dwMapId;
		com.m_onClickEventParams.tag2 = (int)mail.relationType;
		com.m_onClickEventParams.tagUInt = mail.dwGameSvrEntity;
		com.m_onClickEventParams.commonUInt64Param1 = mail.uid;
		com.m_onClickEventParams.taskId = mail.dwLogicWorldID;
		com.m_onClickEventParams.tag3 = (int)mail.inviteType;
	}

	private string GetProcessTypeString(CMailSys.enProcessInviteType type)
	{
		switch (type)
		{
		case CMailSys.enProcessInviteType.Refuse:
			return Singleton<CMailSys>.instance.inviteRefuseStr;
		case CMailSys.enProcessInviteType.Accept:
			return Singleton<CMailSys>.instance.inviteAcceptStr;
		case CMailSys.enProcessInviteType.NoProcess:
			return Singleton<CMailSys>.instance.inviteNoProcessStr;
		default:
			return "error";
		}
	}

	private bool GetOtherPlayerState(COM_INVITE_RELATION_TYPE type, ulong uid, uint dwLogicWorldID, out string stateStr, out string headURL)
	{
		headURL = string.Empty;
		if (type == COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_FRIEND)
		{
			CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(uid, dwLogicWorldID);
			COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(uid, dwLogicWorldID);
			if (gameOrSnsFriend == null)
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			headURL = Utility.UTF8Convert(gameOrSnsFriend.szHeadUrl);
			if (gameOrSnsFriend.bIsOnline != 1)
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			stateStr = Singleton<CMailSys>.instance.onlineStr;
			if (friendInGaming == null)
			{
				return gameOrSnsFriend.bIsOnline == 1;
			}
			if (friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME)
			{
				if (friendInGaming.startTime > 0u)
				{
					stateStr = Singleton<CMailSys>.instance.gamingStr;
				}
				else
				{
					stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime"));
				}
			}
			else if (friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
			{
				stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming"));
			}
			return gameOrSnsFriend.bIsOnline == 1;
		}
		else if (type == COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_GUILDMEMBER)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(uid);
			if (guildMemberInfoByUid == null)
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			headURL = guildMemberInfoByUid.stBriefInfo.szHeadUrl;
			if (!CGuildHelper.IsMemberOnline(guildMemberInfoByUid))
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			stateStr = Singleton<CMailSys>.instance.onlineStr;
			if (guildMemberInfoByUid.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || guildMemberInfoByUid.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME)
			{
				if (guildMemberInfoByUid.dwGameStartTime > 0u)
				{
					stateStr = Singleton<CMailSys>.instance.gamingStr;
				}
				else
				{
					stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime"));
				}
			}
			return true;
		}
		else
		{
			if (type != COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_LBS)
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			CSDT_LBS_USER_INFO lBSUserInfo = Singleton<CFriendContoller>.instance.model.GetLBSUserInfo(uid, dwLogicWorldID, CFriendModel.LBSGenderType.Both);
			if (lBSUserInfo == null)
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
				return false;
			}
			headURL = Utility.UTF8Convert(lBSUserInfo.stLbsUserInfo.szHeadUrl);
			if (lBSUserInfo.stLbsUserInfo.bIsOnline == 1)
			{
				stateStr = Singleton<CMailSys>.instance.onlineStr;
			}
			else
			{
				stateStr = Singleton<CMailSys>.instance.offlineStr;
			}
			return lBSUserInfo.stLbsUserInfo.bIsOnline == 1;
		}
	}

	public int GetFriendMailListSelectedIndex()
	{
		if (this.m_panelFri != null)
		{
			CUIListScript component = this.m_panelFri.transform.Find("List").GetComponent<CUIListScript>();
			if (component != null)
			{
				return component.GetSelectedIndex();
			}
		}
		return -1;
	}
}

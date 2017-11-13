using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CMiniPlayerInfoSystem : Singleton<CMiniPlayerInfoSystem>
{
	public enum OpenSrc
	{
		None,
		Rank,
		Chat
	}

	public static string sPlayerInfoFormPath = "UGUI/Form/System/Player/Form_Mini_Player_Info.prefab";

	private int m_CurSelectedLogicWorld;

	private string m_ComplanText;

	private string m_ComplanName;

	private string m_ComplanOpenId;

	private CPlayerProfile m_PlayerProfile = new CPlayerProfile();

	private CMiniPlayerInfoSystem.OpenSrc m_OpenSrc;

	private CSPkg m_BackPlayeInfoMsg;

	private bool m_bUp;

	private ulong m_CurSelectedUuid
	{
		get;
		set;
	}

	public override void Init()
	{
		base.Init();
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMiniProfile));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Profile, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerInfoForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Invite_3v3, new CUIEventManager.OnUIEventHandler(this.OnInvite3v3));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Invite_5v5, new CUIEventManager.OnUIEventHandler(this.OnInvite5v5));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Block, new CUIEventManager.OnUIEventHandler(this.OnBlockFriend));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Block_Ok, new CUIEventManager.OnUIEventHandler(this.OnBlockOk));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Complaints, new CUIEventManager.OnUIEventHandler(this.OnComplaints));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_ComplaintsOK, new CUIEventManager.OnUIEventHandler(this.OnComplaintsOk));
		Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>(EventID.PlayerInfoSystem_Info_Received, new Action<CSPkg>(this.OnPlayerInfoSystemRecivedMsg));
		Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.PlayerBlock_Success, new Action(this.OnPlayerBlockSuccess));
	}

	public override void UnInit()
	{
		base.UnInit();
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMiniProfile));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Profile, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerInfoForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Invite_3v3, new CUIEventManager.OnUIEventHandler(this.OnInvite3v3));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Invite_5v5, new CUIEventManager.OnUIEventHandler(this.OnInvite5v5));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Block, new CUIEventManager.OnUIEventHandler(this.OnBlockFriend));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Block_Ok, new CUIEventManager.OnUIEventHandler(this.OnBlockOk));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_Complaints, new CUIEventManager.OnUIEventHandler(this.OnComplaints));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_ComplaintsOK, new CUIEventManager.OnUIEventHandler(this.OnComplaintsOk));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>(EventID.PlayerInfoSystem_Info_Received, new Action<CSPkg>(this.OnPlayerInfoSystemRecivedMsg));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerBlock_Success, new Action(this.OnPlayerBlockSuccess));
	}

	private void OnOpenMiniProfile(CUIEvent uiEvent)
	{
		ulong num = 0uL;
		int num2 = 0;
		this.m_bUp = false;
		this.m_OpenSrc = (CMiniPlayerInfoSystem.OpenSrc)uiEvent.m_eventParams.tag;
		CMiniPlayerInfoSystem.OpenSrc openSrc = this.m_OpenSrc;
		if (openSrc != CMiniPlayerInfoSystem.OpenSrc.Rank)
		{
			if (openSrc == CMiniPlayerInfoSystem.OpenSrc.Chat)
			{
				if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Lobby)
				{
					num = uiEvent.m_eventParams.commonUInt64Param1;
					num2 = uiEvent.m_eventParams.tag2;
					this.m_ComplanText = uiEvent.m_eventParams.tagStr;
					this.m_ComplanName = uiEvent.m_eventParams.tagStr1;
					this.m_ComplanOpenId = uiEvent.m_eventParams.pwd;
				}
				else if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Guild || Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.GuildMatchTeam)
				{
					num = uiEvent.m_eventParams.commonUInt64Param1;
					num2 = uiEvent.m_eventParams.tag2;
				}
				else if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Settle)
				{
					num = uiEvent.m_eventParams.commonUInt64Param1;
					num2 = uiEvent.m_eventParams.tag2;
					this.m_bUp = true;
				}
				else if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Friend_Chat)
				{
					CChatSysData sysData = Singleton<CChatController>.GetInstance().model.sysData;
					if (sysData == null)
					{
						Debug.LogError("Open mini profile failed, CChatSysData is null");
						return;
					}
					num = sysData.ullUid;
					num2 = (int)sysData.dwLogicWorldId;
				}
			}
		}
		else
		{
			num = uiEvent.m_eventParams.commonUInt64Param1;
			num2 = uiEvent.m_eventParams.tag2;
		}
		if (num > 0uL)
		{
			if (num == this.m_CurSelectedUuid && num2 == this.m_CurSelectedLogicWorld)
			{
				return;
			}
			this.m_CurSelectedUuid = num;
			this.m_CurSelectedLogicWorld = num2;
			Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(num, num2, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, false, CPlayerInfoSystem.Tab.Base_Info);
		}
	}

	private void OnAddFriend(CUIEvent uiEvent)
	{
		if (this.m_CurSelectedUuid > 0uL && this.m_CurSelectedLogicWorld > 0)
		{
			Singleton<CFriendContoller>.instance.Open_Friend_Verify(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
		}
	}

	private void OnOpenPlayerInfoForm(CUIEvent uiEvent)
	{
		if (this.m_CurSelectedUuid > 0uL && this.m_CurSelectedLogicWorld > 0)
		{
			Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(this.m_CurSelectedUuid, this.m_CurSelectedLogicWorld, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
		}
	}

	private void OnInvite3v3(CUIEvent uiEvent)
	{
		if (this.m_CurSelectedUuid > 0uL && this.m_CurSelectedLogicWorld > 0)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Matching_OpenEntry;
			cUIEvent.m_eventParams.tag = 3;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
				return;
			}
			CUIEvent cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Matching_BtnGroup_Click;
			cUIEvent2.m_srcFormScript = form;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
			cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Matching_Begin3v3Team;
			uint tagUInt = 0u;
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_3V3"), ref tagUInt);
			cUIEvent2.m_eventParams.tagUInt = tagUInt;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
		}
		Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
	}

	private void OnInvite5v5(CUIEvent uiEvent)
	{
		if (this.m_CurSelectedUuid > 0uL && this.m_CurSelectedLogicWorld > 0)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Matching_OpenEntry;
			cUIEvent.m_eventParams.tag = 3;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
				return;
			}
			CUIEvent cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Matching_BtnGroup_Click;
			cUIEvent2.m_srcFormScript = form;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
			cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Matching_Begin5v5Team;
			uint tagUInt = 0u;
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5"), ref tagUInt);
			cUIEvent2.m_eventParams.tagUInt = tagUInt;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
		}
		Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
	}

	private void OnInviteEntertainment(CUIEvent uiEvent)
	{
		if (this.m_CurSelectedUuid > 0uL && this.m_CurSelectedLogicWorld > 0)
		{
			CUIEvent cUIEvent = new CUIEvent();
			cUIEvent.m_eventID = enUIEventID.Matching_OpenEntry;
			cUIEvent.m_eventParams.tag = 3;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
				return;
			}
			CUIEvent cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.Matching_BtnGroup_Click;
			cUIEvent2.m_eventParams.tag = 1;
			cUIEvent2.m_srcFormScript = form;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
			cUIEvent2 = new CUIEvent();
			cUIEvent2.m_eventID = enUIEventID.MatchingExt_BeginMelee;
			uint tagUInt = 0u;
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_MELEE"), ref tagUInt);
			cUIEvent2.m_eventParams.tagUInt = tagUInt;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent2);
		}
		Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
	}

	private void OnCloseForm(CUIEvent uiEvent)
	{
		Singleton<CUIManager>.GetInstance().CloseForm(CMiniPlayerInfoSystem.sPlayerInfoFormPath);
		this.m_CurSelectedUuid = 0uL;
		this.m_CurSelectedLogicWorld = 0;
		this.m_ComplanText = string.Empty;
		this.m_ComplanName = string.Empty;
		this.m_BackPlayeInfoMsg = null;
	}

	private void OnBlockFriend(CUIEvent uiEvent)
	{
		string name = Singleton<CFriendContoller>.instance.model.GetName(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
		string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Black_BlockTip", new string[]
		{
			name
		}), new object[0]);
		Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Friend_Block_Ok, enUIEventID.Friend_Block_Cancle, false);
	}

	private void OnBlockOk(CUIEvent uiEvent)
	{
		FriendSysNetCore.Send_Block(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
	}

	private void OnComplaints(CUIEvent uiEvent)
	{
		string name = Singleton<CFriendContoller>.instance.model.GetName(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
		uint complanCount = Singleton<CChatController>.instance.model.complanCount;
		int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(345u).dwConfValue;
		if ((ulong)complanCount < (ulong)((long)dwConfValue))
		{
			string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Complaints_Tip", new string[]
			{
				name
			}), new object[0]);
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Chat_ComplaintsOK, enUIEventID.None, false);
		}
		else
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("COMPLAINT_Limit");
			Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
		}
	}

	private void OnComplaintsOk(CUIEvent uiEvent)
	{
		CChatView view = Singleton<CChatController>.instance.view;
		if (view == null || view.CurTab == EChatChannel.None || view.CurTab == EChatChannel.Default)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.m_ComplanText) || string.IsNullOrEmpty(this.m_ComplanName))
		{
			Singleton<CUIManager>.instance.OpenMessageBox(Singleton<CTextManager>.instance.GetText("COMPLAINT_Error"), false);
			return;
		}
		CChatNetUT.Send_Complaints_Chat_Req(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld, this.m_ComplanName, this.m_ComplanOpenId, this.m_ComplanText, (uint)CChatUT.Convert_Channel_ChatMsgType(view.CurTab));
		Singleton<CChatController>.instance.model.complanCount += 1u;
	}

	private void OnPlayerBlockSuccess()
	{
		if (this.m_BackPlayeInfoMsg != null)
		{
			this.OnPlayerInfoSystemRecivedMsg(this.m_BackPlayeInfoMsg);
		}
	}

	private void OnPlayerInfoSystemRecivedMsg(CSPkg msg)
	{
		if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode != 0)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode), false);
			return;
		}
		this.m_BackPlayeInfoMsg = msg;
		this.m_PlayerProfile.ConvertServerDetailData(msg.stPkgData.stGetAcntDetailInfoRsp.stAcntDetail.stOfSucc);
		CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMiniPlayerInfoSystem.sPlayerInfoFormPath, false, true);
		if (cUIFormScript == null)
		{
			return;
		}
		if (this.m_bUp)
		{
			cUIFormScript.SetPriority(enFormPriority.Priority5);
		}
		else
		{
			cUIFormScript.RestorePriority();
		}
		GameObject widget = cUIFormScript.GetWidget(0);
		RectTransform rectTransform = cUIFormScript.transform.Find("panel") as RectTransform;
		if (rectTransform == null)
		{
			Debug.LogError("mini player info form's panel is null");
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
		}
		CMiniPlayerInfoSystem.OpenSrc openSrc = this.m_OpenSrc;
		if (openSrc != CMiniPlayerInfoSystem.OpenSrc.Rank)
		{
			if (openSrc == CMiniPlayerInfoSystem.OpenSrc.Chat)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatFormPath);
				if (form == null)
				{
					Debug.LogError("can't get chat form");
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
					return;
				}
				RectTransform rectTransform2 = form.transform.Find("node/null") as RectTransform;
				if (rectTransform2 == null)
				{
					Debug.LogError("chat form's close btn is null");
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
					return;
				}
				widget.CustomSetActive(true);
				Vector3[] array = new Vector3[4];
				rectTransform2.GetWorldCorners(array);
				Vector2 vector = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array[3]);
				Vector2 screenPoint = new Vector2(0f, vector.y);
				Vector3[] array2 = new Vector3[4];
				rectTransform.GetWorldCorners(array2);
				float num = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array2[3]).x - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array2[0]).x;
				if (vector.x + num + 100f > (float)Screen.width)
				{
					screenPoint.x = (float)Screen.width - num - 15f;
				}
				else
				{
					screenPoint = new Vector2(vector.x + 20f, vector.y);
				}
				rectTransform.position = CUIUtility.ScreenToWorldPoint(cUIFormScript.GetCamera(), screenPoint, rectTransform.position.z);
			}
		}
		else
		{
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
			if (form2 == null)
			{
				Debug.LogError("can't get ranking form");
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
				return;
			}
			RectTransform rectTransform3 = form2.transform.Find("bg") as RectTransform;
			if (rectTransform3 == null)
			{
				Debug.LogError("ranking form's bg is null");
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
				return;
			}
			widget.CustomSetActive(true);
			Vector3[] array3 = new Vector3[4];
			rectTransform3.GetWorldCorners(array3);
			Vector2 vector2 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array3[2]);
			Vector2 screenPoint2 = new Vector2(0f, vector2.y - 100f);
			Vector3[] array4 = new Vector3[4];
			rectTransform.GetWorldCorners(array4);
			float num2 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array4[3]).x - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, array4[0]).x;
			if (vector2.x + 80f + num2 > (float)Screen.width)
			{
				screenPoint2.x = (float)Screen.width - num2 - 15f;
			}
			else
			{
				screenPoint2.x = vector2.x + 80f;
			}
			rectTransform.position = CUIUtility.ScreenToWorldPoint(cUIFormScript.GetCamera(), screenPoint2, rectTransform.position.z);
		}
		Text componetInChild = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "panel/Name/Text");
		if (componetInChild != null)
		{
			componetInChild.set_text(this.m_PlayerProfile.Name());
		}
		COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
		COMDT_FRIEND_INFO info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
		Text componetInChild2 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "panel/Online/Text");
		if (componetInChild2 != null)
		{
			if (this.m_PlayerProfile.IsOnLine())
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = (info == null) ? ((info2 == null) ? null : info2) : info;
				if (cOMDT_FRIEND_INFO != null)
				{
					COM_ACNT_GAME_STATE friendInGamingState = Singleton<CFriendContoller>.instance.model.GetFriendInGamingState(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld);
					if (friendInGamingState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
					{
						componetInChild2.set_text(string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online")));
					}
					else if (friendInGamingState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME || friendInGamingState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME)
					{
						componetInChild2.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming")));
					}
					else if (friendInGamingState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
					{
						componetInChild2.set_text(string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming")));
					}
				}
				else
				{
					componetInChild2.set_text(string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online")));
				}
			}
			else
			{
				Text text = componetInChild2;
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Common_Offline");
				componetInChild2.set_text(text2);
				text.set_text(text2);
			}
		}
		Text componetInChild3 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "panel/DuanWei/Text");
		if (componetInChild3 != null)
		{
			string rankName = CLadderView.GetRankName(this.m_PlayerProfile.GetRankGrade(), (uint)this.m_PlayerProfile.GetRankClass());
			componetInChild3.set_text(string.IsNullOrEmpty(rankName) ? Singleton<CTextManager>.GetInstance().GetText("Common_NoData") : rankName);
		}
		Text componetInChild4 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "panel/Team/Text");
		Text componetInChild5 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "panel/Position/Text");
		if (!CGuildSystem.IsInNormalGuild(this.m_PlayerProfile.GuildState) || string.IsNullOrEmpty(this.m_PlayerProfile.GuildName))
		{
			if (componetInChild4 != null)
			{
				componetInChild4.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild"));
			}
			if (componetInChild5 != null)
			{
				componetInChild5.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild"));
			}
		}
		else
		{
			if (componetInChild4 != null)
			{
				componetInChild4.set_text(this.m_PlayerProfile.GuildName);
			}
			if (componetInChild5 != null)
			{
				componetInChild5.set_text(CGuildHelper.GetPositionName(this.m_PlayerProfile.GuildState));
			}
		}
		GameObject obj = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/AddFriend");
		GameObject obj2 = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/Profile");
		GameObject obj3 = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/3v3");
		GameObject obj4 = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/5v5");
		GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/Block");
		GameObject obj5 = Utility.FindChild(cUIFormScript.gameObject, "panel/Btn/Complaints");
		obj5.CustomSetActive(false);
		obj2.CustomSetActive(true);
		openSrc = this.m_OpenSrc;
		if (openSrc != CMiniPlayerInfoSystem.OpenSrc.Rank)
		{
			if (openSrc == CMiniPlayerInfoSystem.OpenSrc.Chat)
			{
				switch (Singleton<CChatController>.GetInstance().view.CurTab)
				{
				case EChatChannel.Lobby:
				case EChatChannel.GuildMatchTeam:
					obj3.CustomSetActive(false);
					obj4.CustomSetActive(false);
					if (info != null || info2 != null)
					{
						obj.CustomSetActive(false);
					}
					else
					{
						obj.CustomSetActive(true);
					}
					gameObject.CustomSetActive(true);
					obj5.CustomSetActive(false);
					this.SetBlockButtonBlocked(gameObject, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld));
					break;
				case EChatChannel.Guild:
					if (info != null || info2 != null)
					{
						obj.CustomSetActive(false);
						obj3.CustomSetActive(true);
						obj4.CustomSetActive(true);
					}
					else
					{
						obj.CustomSetActive(true);
						obj3.CustomSetActive(false);
						obj4.CustomSetActive(false);
					}
					gameObject.CustomSetActive(true);
					obj5.CustomSetActive(false);
					this.SetBlockButtonBlocked(gameObject, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld));
					break;
				case EChatChannel.Friend_Chat:
					obj.CustomSetActive(false);
					obj3.CustomSetActive(true);
					obj4.CustomSetActive(true);
					gameObject.CustomSetActive(true);
					obj5.CustomSetActive(false);
					this.SetBlockButtonBlocked(gameObject, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld));
					break;
				case EChatChannel.Settle:
					obj.CustomSetActive(false);
					obj3.CustomSetActive(false);
					obj4.CustomSetActive(false);
					gameObject.CustomSetActive(true);
					obj5.CustomSetActive(false);
					this.SetBlockButtonBlocked(gameObject, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint)this.m_CurSelectedLogicWorld));
					break;
				}
			}
		}
		else
		{
			if (info != null || info2 != null)
			{
				obj.CustomSetActive(false);
				obj3.CustomSetActive(true);
				obj4.CustomSetActive(true);
			}
			else
			{
				obj.CustomSetActive(true);
				obj3.CustomSetActive(false);
				obj4.CustomSetActive(false);
			}
			gameObject.CustomSetActive(false);
			obj5.CustomSetActive(false);
		}
	}

	private void SetBlockButtonBlocked(GameObject blockNode, bool bHasBlocked)
	{
		if (blockNode == null)
		{
			return;
		}
		CUIEventScript component = blockNode.GetComponent<CUIEventScript>();
		if (component != null)
		{
			component.enabled = !bHasBlocked;
		}
		Button component2 = blockNode.GetComponent<Button>();
		if (component2 != null)
		{
			CUICommonSystem.SetButtonEnableWithShader(component2, !bHasBlocked, true);
		}
		string text = bHasBlocked ? "已屏蔽" : "屏蔽";
		Text component3 = blockNode.transform.Find("Text").GetComponent<Text>();
		if (component3 != null)
		{
			component3.set_text(text);
		}
	}
}

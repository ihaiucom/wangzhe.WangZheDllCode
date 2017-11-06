using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class COBSystem : Singleton<COBSystem>
	{
		private enum enOBTab
		{
			Null,
			Expert,
			Friend,
			Guild,
			Local
		}

		private enum enStatus
		{
			Normal,
			Editor
		}

		private struct stOBFriend
		{
			public COMDT_ACNT_UNIQ uin;

			public string friendName;

			public string headUrl;

			public COMDT_GAMEINFO_DETAIL gameDetail;
		}

		private struct stOBExpert
		{
			public COMDT_OB_DESK desk;

			public uint startTime;

			public uint observeNum;

			public COMDT_HEROLABEL heroLabel;
		}

		public class stOBGuild
		{
			public ulong obUid;

			public ulong playerUid;

			public string playerName;

			public string teamName;

			public string headUrl;

			public uint dwHeroID;

			public uint dwStartTime;

			public byte bGrade;

			public uint dwClass;

			public uint dwObserveNum;
		}

		private struct stOBLocal
		{
			public string path;

			public uint heroId;
		}

		public static readonly string OB_FORM_PATH = "UGUI/Form/System/OB/Form_OB.prefab";

		private COBSystem.enStatus curStatus;

		private List<COBSystem.stOBFriend> OBFriendList = new List<COBSystem.stOBFriend>();

		private List<COBSystem.stOBExpert> OBExpertList = new List<COBSystem.stOBExpert>();

		private List<COBSystem.stOBGuild> OBGuildList = new List<COBSystem.stOBGuild>();

		private ListView<GameReplayModule.ReplayFileInfo> OBLocalList = new ListView<GameReplayModule.ReplayFileInfo>();

		private static int m_lastGetExpertListTime = 0;

		public static int EXPERT_DETAL_SEC = 60;

		private static int m_lastGetFriendStateTime = 0;

		public static int FRIEND_DETAL_SEC = 60;

		private WatchEntryData m_watchEntryData;

		private static Vector2 s_content_pos = new Vector2(0f, -33f);

		private static Vector2 s_content_size = new Vector2(0f, -66f);

		private COBSystem.enOBTab CurTab
		{
			get
			{
				CUIFormScript form = Singleton<CUIManager>.instance.GetForm(COBSystem.OB_FORM_PATH);
				if (form == null)
				{
					return COBSystem.enOBTab.Null;
				}
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "Panel_Menu/List");
				if (componetInChild == null)
				{
					return COBSystem.enOBTab.Null;
				}
				CUIListElementScript selectedElement = componetInChild.GetSelectedElement();
				if (selectedElement == null)
				{
					return COBSystem.enOBTab.Null;
				}
				return selectedElement.m_onEnableEventParams.tag + COBSystem.enOBTab.Expert;
			}
		}

		public override void Init()
		{
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnOBFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnOBFormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Tab_Click, new CUIEventManager.OnUIEventHandler(this.OnOBVideoTabClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Enter, new CUIEventManager.OnUIEventHandler(this.OnVideoEnter));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Enter_Confirm, new CUIEventManager.OnUIEventHandler(this.OnVideoEnterConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Editor_Click, new CUIEventManager.OnUIEventHandler(this.OnEditorClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Delete, new CUIEventManager.OnUIEventHandler(this.OnVideoDelete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Delete_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmDelete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Expired_Delete, new CUIEventManager.OnUIEventHandler(this.OnExpiredDelete));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Expired_DeleteConfirmed, new CUIEventManager.OnUIEventHandler(this.OnExpiredDeleteConfirmed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Expired_DeleteCanceled, new CUIEventManager.OnUIEventHandler(this.OnExpiredDeleteCanceled));
			base.Init();
		}

		public override void UnInit()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnOBFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnOBFormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Tab_Click, new CUIEventManager.OnUIEventHandler(this.OnOBVideoTabClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Enter, new CUIEventManager.OnUIEventHandler(this.OnVideoEnter));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Enter_Confirm, new CUIEventManager.OnUIEventHandler(this.OnVideoEnterConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Editor_Click, new CUIEventManager.OnUIEventHandler(this.OnEditorClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Delete, new CUIEventManager.OnUIEventHandler(this.OnVideoDelete));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Delete_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmDelete));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Expired_DeleteConfirmed, new CUIEventManager.OnUIEventHandler(this.OnExpiredDeleteConfirmed));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Expired_DeleteCanceled, new CUIEventManager.OnUIEventHandler(this.OnExpiredDeleteCanceled));
			base.UnInit();
		}

		public void Clear()
		{
			this.OBFriendList.Clear();
			this.OBExpertList.Clear();
			this.OBLocalList.Clear();
			this.OBGuildList.Clear();
			COBSystem.m_lastGetExpertListTime = 0;
			COBSystem.m_lastGetFriendStateTime = 0;
			this.m_watchEntryData = null;
		}

		public bool IsInOBFriendList(ulong uid)
		{
			for (int i = 0; i < this.OBFriendList.get_Count(); i++)
			{
				if (this.OBFriendList.get_Item(i).uin.ullUid == uid)
				{
					return true;
				}
			}
			return false;
		}

		public void OnOBFormOpen(CUIEvent cuiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			if (CUICommonSystem.IsInMatchingWithAlert())
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(COBSystem.OB_FORM_PATH, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(cUIFormScript.gameObject, "Panel_Menu/List");
			this.InitTab(componetInChild);
			Transform transform = cUIFormScript.gameObject.transform.FindChild("Panel_Menu/BtnVideoMgr");
			if (transform && transform.gameObject)
			{
				if (!Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag())
				{
					transform.gameObject.CustomSetActive(false);
				}
				else
				{
					transform.gameObject.CustomSetActive(true);
				}
			}
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenOB);
		}

		private void InitTab(CUIListScript list)
		{
			if (list == null)
			{
				return;
			}
			bool flag = Singleton<CGuildSystem>.GetInstance().IsInNormalGuild();
			int num = Enum.GetValues(typeof(COBSystem.enOBTab)).get_Length() - 1;
			if (!flag)
			{
				num--;
			}
			list.SetElementAmount(num);
			string[] array = new string[]
			{
				"OB_Expert",
				"OB_Freind",
				"OB_Guild",
				"OB_Local"
			};
			for (int i = 0; i < num; i++)
			{
				CUIListElementScript elemenet = list.GetElemenet(i);
				int num2 = (i + 1 >= 3 && !flag) ? (i + 1) : i;
				elemenet.m_onEnableEventParams.tag = num2;
				Utility.GetComponetInChild<Text>(elemenet.gameObject, "Text").set_text(Singleton<CTextManager>.instance.GetText(array[num2]));
			}
			list.SelectElement(0, true);
		}

		private void OnOBFormClose(CUIEvent cuiEvent)
		{
			Singleton<GameJoy>.instance.CloseVideoListDialog();
			this.m_watchEntryData = null;
		}

		private void OnOBVideoTabClick(CUIEvent cuiEvent)
		{
			COBSystem.enOBTab curTab = this.CurTab;
			this.m_watchEntryData = null;
			switch (curTab)
			{
			case COBSystem.enOBTab.Expert:
				COBSystem.GetGreatMatch(false);
				break;
			case COBSystem.enOBTab.Friend:
				COBSystem.GetFriendsState();
				break;
			case COBSystem.enOBTab.Guild:
				this.OBGuildList = Singleton<CGuildMatchSystem>.GetInstance().GetGuidMatchObInfo();
				Singleton<CGuildMatchSystem>.GetInstance().RequestGuildOBCount();
				break;
			case COBSystem.enOBTab.Local:
				this.OBLocalList = Singleton<GameReplayModule>.instance.ListReplayFiles(false);
				break;
			}
			this.curStatus = COBSystem.enStatus.Normal;
			this.UpdateView();
		}

		private void OnEditorClick(CUIEvent cuiEvent)
		{
			if (this.CurTab == COBSystem.enOBTab.Local)
			{
				if (this.curStatus == COBSystem.enStatus.Normal)
				{
					this.curStatus = COBSystem.enStatus.Editor;
				}
				else
				{
					this.curStatus = COBSystem.enStatus.Normal;
				}
				this.UpdateView();
			}
		}

		private void OnVideoDelete(CUIEvent cuiEvent)
		{
			int srcWidgetIndexInBelongedList = cuiEvent.m_srcWidgetIndexInBelongedList;
			stUIEventParams par = default(stUIEventParams);
			par.tag = srcWidgetIndexInBelongedList;
			Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("OB_Desc_7"), enUIEventID.OB_Video_Delete_Confirm, enUIEventID.None, par, false);
		}

		private void OnConfirmDelete(CUIEvent cuiEvent)
		{
			int tag = cuiEvent.m_eventParams.tag;
			if (tag >= 0 && tag < this.OBLocalList.Count)
			{
				string path = this.OBLocalList[tag].path;
				try
				{
					File.Delete(path);
					this.OBLocalList.RemoveAt(tag);
					Singleton<CUIManager>.instance.OpenTips("OB_Desc_8", true, 1.5f, null, new object[0]);
				}
				catch
				{
					Singleton<CUIManager>.instance.OpenTips("OB_Desc_9", true, 1.5f, null, new object[0]);
				}
			}
			this.UpdateView();
		}

		private void OnExpiredDelete(CUIEvent evt)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Watching_DeleteAllExpiredFile_Confirm"), enUIEventID.OB_Video_Expired_DeleteConfirmed, enUIEventID.OB_Video_Expired_DeleteCanceled, false);
		}

		private void OnExpiredDeleteConfirmed(CUIEvent evt)
		{
			try
			{
				for (int i = this.OBLocalList.Count - 1; i >= 0; i--)
				{
					GameReplayModule.ReplayFileInfo replayFileInfo = this.OBLocalList[i];
					if (replayFileInfo.isExpired)
					{
						File.Delete(replayFileInfo.path);
						this.OBLocalList.RemoveAt(i);
					}
				}
				Singleton<CUIManager>.instance.OpenTips("OB_Desc_8", true, 1.5f, null, new object[0]);
			}
			catch (Exception var_2_6E)
			{
				Singleton<CUIManager>.instance.OpenTips("OB_Desc_9", true, 1.5f, null, new object[0]);
			}
			this.UpdateView();
		}

		private void OnExpiredDeleteCanceled(CUIEvent evt)
		{
		}

		public void UpdateView()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(COBSystem.OB_FORM_PATH);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(2);
			Text componetInChild = Utility.GetComponetInChild<Text>(widget, "Text");
			GameObject widget3 = form.GetWidget(1);
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "ContentList");
			RectTransform component = Utility.FindChild(componetInChild2.gameObject, "ScrollRect").GetComponent<RectTransform>();
			if (componetInChild2 == null)
			{
				return;
			}
			COBSystem.enOBTab curTab = this.CurTab;
			widget3.CustomSetActive(false);
			if (curTab == COBSystem.enOBTab.Expert)
			{
				widget2.CustomSetActive(false);
				component.anchoredPosition = Vector2.zero;
				component.sizeDelta = Vector2.zero;
				componetInChild2.SetElementAmount(this.OBExpertList.get_Count());
			}
			else if (curTab == COBSystem.enOBTab.Friend)
			{
				widget2.CustomSetActive(false);
				component.anchoredPosition = Vector2.zero;
				component.sizeDelta = Vector2.zero;
				componetInChild2.SetElementAmount(this.OBFriendList.get_Count());
			}
			else if (curTab == COBSystem.enOBTab.Guild)
			{
				widget2.CustomSetActive(false);
				component.anchoredPosition = Vector2.zero;
				component.sizeDelta = Vector2.zero;
				componetInChild2.SetElementAmount(this.OBGuildList.get_Count());
			}
			else if (curTab == COBSystem.enOBTab.Local)
			{
				int num = 0;
				foreach (GameReplayModule.ReplayFileInfo current in this.OBLocalList)
				{
					if (current.isExpired)
					{
						num++;
					}
				}
				widget2.CustomSetActive(this.OBLocalList.Count > 0);
				widget3.CustomSetActive(num > 0 && this.curStatus == COBSystem.enStatus.Editor);
				component.sizeDelta = COBSystem.s_content_size;
				component.anchoredPosition = COBSystem.s_content_pos;
				componetInChild.set_text(Singleton<CTextManager>.instance.GetText((this.curStatus == COBSystem.enStatus.Normal) ? "Common_Edit" : "Common_Close"));
				componetInChild2.SetElementAmount(this.OBLocalList.Count);
			}
		}

		private void OnElementEnable(CUIEvent cuiEvent)
		{
			COBSystem.enOBTab curTab = this.CurTab;
			if (curTab == COBSystem.enOBTab.Expert)
			{
				this.UpdateElement(cuiEvent.m_srcWidget, this.OBExpertList.get_Item(cuiEvent.m_srcWidgetIndexInBelongedList));
			}
			else if (curTab == COBSystem.enOBTab.Friend)
			{
				this.UpdateElement(cuiEvent.m_srcWidget, this.OBFriendList.get_Item(cuiEvent.m_srcWidgetIndexInBelongedList));
			}
			else if (curTab == COBSystem.enOBTab.Guild)
			{
				this.UpdateElement(cuiEvent.m_srcWidget, this.OBGuildList.get_Item(cuiEvent.m_srcWidgetIndexInBelongedList));
			}
			else if (curTab == COBSystem.enOBTab.Local)
			{
				this.UpdateElement(cuiEvent.m_srcWidget, this.OBLocalList[cuiEvent.m_srcWidgetIndexInBelongedList]);
			}
		}

		private void UpdateElement(GameObject element, COBSystem.stOBFriend OBFriend)
		{
			if (CFriendModel.RemarkNames != null && CFriendModel.RemarkNames.ContainsKey(OBFriend.uin.ullUid))
			{
				string empty = string.Empty;
				if (CFriendModel.RemarkNames.TryGetValue(OBFriend.uin.ullUid, ref empty))
				{
					if (!string.IsNullOrEmpty(empty))
					{
						this.UpdateElement(element, empty, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bShowRankGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, COBSystem.enOBTab.Friend, (int)OBFriend.gameDetail.dwObserveNum, false, this.curStatus, 0L, 0, 0u, string.Empty);
					}
					else
					{
						this.UpdateElement(element, OBFriend.friendName, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bShowRankGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, COBSystem.enOBTab.Friend, (int)OBFriend.gameDetail.dwObserveNum, false, this.curStatus, 0L, 0, 0u, string.Empty);
					}
				}
			}
			else
			{
				this.UpdateElement(element, OBFriend.friendName, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bShowRankGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, COBSystem.enOBTab.Friend, (int)OBFriend.gameDetail.dwObserveNum, false, this.curStatus, 0L, 0, 0u, string.Empty);
			}
		}

		private void UpdateElement(GameObject element, COBSystem.stOBGuild obGuild)
		{
			this.UpdateElement(element, obGuild.playerName, obGuild.headUrl, obGuild.bGrade, obGuild.dwClass, obGuild.dwHeroID, COBSystem.enOBTab.Guild, (int)obGuild.dwObserveNum, false, this.curStatus, 0L, 0, 0u, obGuild.teamName);
		}

		private void UpdateElement(GameObject element, COBSystem.stOBExpert OBExpert)
		{
			this.UpdateElement(element, Utility.UTF8Convert(OBExpert.heroLabel.szRoleName), Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(Utility.UTF8Convert(OBExpert.heroLabel.szHealUrl)), (byte)OBExpert.heroLabel.dwGrade, OBExpert.heroLabel.dwClass, OBExpert.heroLabel.dwHeroID, COBSystem.enOBTab.Expert, (int)OBExpert.observeNum, false, this.curStatus, 0L, 0, 0u, string.Empty);
		}

		private void UpdateElement(GameObject element, GameReplayModule.ReplayFileInfo OBLocal)
		{
			this.UpdateElement(element, OBLocal.userName, OBLocal.userHead, OBLocal.userRankGrade, OBLocal.userRankClass, OBLocal.heroId, COBSystem.enOBTab.Local, 0, OBLocal.isExpired, this.curStatus, OBLocal.startTime, OBLocal.mapType, OBLocal.mapId, string.Empty);
		}

		private void UpdateElement(GameObject element, string name, string headUrl, byte bGrade, uint subGrade, uint heroId, COBSystem.enOBTab curTab, int onlineNum, bool isExpired, COBSystem.enStatus status = COBSystem.enStatus.Normal, long localTicks = 0L, byte mapType = 0, uint mapId = 0u, string localName = "")
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(COBSystem.OB_FORM_PATH);
			if (form == null)
			{
				return;
			}
			CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(element, "HeadImg");
			Image componetInChild2 = Utility.GetComponetInChild<Image>(element, "HeroImg");
			Image componetInChild3 = Utility.GetComponetInChild<Image>(element, "RankImg");
			Image componetInChild4 = Utility.GetComponetInChild<Image>(element, "RankImg/SubRankImg");
			Text componetInChild5 = Utility.GetComponetInChild<Text>(element, "PlayerName");
			Text componetInChild6 = Utility.GetComponetInChild<Text>(element, "HeroName");
			GameObject obj = Utility.FindChild(element, "WatchImg");
			Text componetInChild7 = Utility.GetComponetInChild<Text>(element, "LocalTime");
			Text componetInChild8 = Utility.GetComponetInChild<Text>(element, "LocalMap");
			Text componetInChild9 = Utility.GetComponetInChild<Text>(element, "WatchImg/OnlineCount");
			GameObject obj2 = Utility.FindChild(element, "DeleteBtn");
			GameObject obj3 = Utility.FindChild(element, "ExpireMark");
			obj3.CustomSetActive(isExpired);
			componetInChild.SetImageUrl(headUrl);
			if (bGrade > 0)
			{
				componetInChild3.gameObject.CustomSetActive(true);
				componetInChild3.SetSprite(CLadderView.GetRankSmallIconPath(bGrade, subGrade), form, true, false, false, false);
				componetInChild4.SetSprite(CLadderView.GetSubRankSmallIconPath(bGrade, subGrade), form, true, false, false, false);
			}
			else
			{
				componetInChild3.gameObject.CustomSetActive(false);
			}
			componetInChild5.set_text(name);
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey != null)
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, CSkinInfo.GetHeroSkinPic(heroId, 0u));
				componetInChild2.SetSprite(prefabPath, form, false, true, true, false);
				componetInChild6.set_text(dataByKey.szName);
			}
			else
			{
				componetInChild6.set_text(string.Empty);
				DebugHelper.Assert(false, string.Format("COBSystem UpdateElement hero cfg[{0}] can not be found!", heroId));
			}
			if (curTab != COBSystem.enOBTab.Local)
			{
				obj.CustomSetActive(true);
				componetInChild9.set_text(Singleton<CTextManager>.instance.GetText("OB_Desc_3", new string[]
				{
					onlineNum.ToString()
				}));
				componetInChild7.gameObject.SetActive(false);
				obj2.CustomSetActive(false);
				componetInChild8.gameObject.CustomSetActive(false);
			}
			else
			{
				obj.CustomSetActive(false);
				componetInChild7.gameObject.SetActive(true);
				DateTime dateTime = new DateTime(localTicks);
				componetInChild7.set_text(Singleton<CTextManager>.instance.GetText("OB_Desc_12", new string[]
				{
					dateTime.get_Month().ToString(),
					dateTime.get_Day().ToString(),
					dateTime.get_Hour().ToString("D2"),
					dateTime.get_Minute().ToString("D2")
				}));
				obj2.CustomSetActive(status == COBSystem.enStatus.Editor);
				componetInChild8.gameObject.CustomSetActive(true);
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(mapType, mapId);
				if (pvpMapCommonInfo != null)
				{
					componetInChild8.set_text(pvpMapCommonInfo.szName);
				}
				else
				{
					componetInChild8.set_text(string.Empty);
				}
				componetInChild6.set_text(string.Empty);
			}
		}

		private void OnVideoEnter(CUIEvent cuiEvent)
		{
			COBSystem.enOBTab curTab = this.CurTab;
			if (curTab == COBSystem.enOBTab.Local)
			{
				CUIFormScript form = Singleton<CUIManager>.instance.GetForm(COBSystem.OB_FORM_PATH);
				if (form == null)
				{
					return;
				}
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "ContentList");
				if (componetInChild == null)
				{
					return;
				}
				int selectedIndex = componetInChild.GetSelectedIndex();
				GameReplayModule.ReplayFileInfo replayFileInfo = this.OBLocalList[selectedIndex];
				if (replayFileInfo != null && replayFileInfo.isExpired)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("OB_Desc_CannotPlayExpiredFile"), false, 1.5f, null, new object[0]);
					return;
				}
			}
			if (this.curStatus == COBSystem.enStatus.Editor)
			{
				return;
			}
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("OB_Desc_11"), enUIEventID.OB_Video_Enter_Confirm, enUIEventID.None, false);
		}

		public WatchEntryData GetWatchEntryData()
		{
			return this.m_watchEntryData;
		}

		private void OnVideoEnterConfirm(CUIEvent cuiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(COBSystem.OB_FORM_PATH);
			if (form == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "ContentList");
			if (componetInChild == null)
			{
				return;
			}
			int selectedIndex = componetInChild.GetSelectedIndex();
			COBSystem.enOBTab curTab = this.CurTab;
			if (curTab == COBSystem.enOBTab.Expert)
			{
				if (selectedIndex >= 0 && selectedIndex < this.OBExpertList.get_Count())
				{
					Singleton<WatchController>.GetInstance().TargetUID = this.OBExpertList.get_Item(selectedIndex).heroLabel.ullUid;
					COBSystem.SendOBServeGreat(this.OBExpertList.get_Item(selectedIndex).desk);
					this.m_watchEntryData = new WatchEntryData();
					this.m_watchEntryData.headUrl = StringHelper.UTF8BytesToString(ref this.OBExpertList.get_Item(selectedIndex).heroLabel.szHealUrl);
					this.m_watchEntryData.name = StringHelper.UTF8BytesToString(ref this.OBExpertList.get_Item(selectedIndex).heroLabel.szRoleName);
					this.m_watchEntryData.rankClass = this.OBExpertList.get_Item(selectedIndex).heroLabel.dwClass;
					this.m_watchEntryData.rankGrade = (byte)this.OBExpertList.get_Item(selectedIndex).heroLabel.dwGrade;
					this.m_watchEntryData.time = (long)CRoleInfo.GetCurrentUTCTime();
					this.m_watchEntryData.usedHeroId = this.OBExpertList.get_Item(selectedIndex).heroLabel.dwHeroID;
					this.m_watchEntryData.userUid = this.OBExpertList.get_Item(selectedIndex).heroLabel.ullUid;
				}
				int count = this.OBExpertList.get_Count();
			}
			else if (curTab == COBSystem.enOBTab.Friend)
			{
				if (selectedIndex >= 0 && selectedIndex < this.OBFriendList.get_Count())
				{
					Singleton<WatchController>.GetInstance().TargetUID = this.OBFriendList.get_Item(selectedIndex).uin.ullUid;
					COBSystem.SendOBServeFriend(this.OBFriendList.get_Item(selectedIndex).uin, -1);
					this.m_watchEntryData = new WatchEntryData();
					this.m_watchEntryData.headUrl = this.OBFriendList.get_Item(selectedIndex).headUrl;
					this.m_watchEntryData.name = this.OBFriendList.get_Item(selectedIndex).friendName;
					this.m_watchEntryData.rankClass = this.OBFriendList.get_Item(selectedIndex).gameDetail.dwClass;
					this.m_watchEntryData.rankGrade = this.OBFriendList.get_Item(selectedIndex).gameDetail.bShowRankGrade;
					this.m_watchEntryData.time = (long)CRoleInfo.GetCurrentUTCTime();
					this.m_watchEntryData.usedHeroId = this.OBFriendList.get_Item(selectedIndex).gameDetail.dwHeroID;
					this.m_watchEntryData.userUid = this.OBFriendList.get_Item(selectedIndex).uin.ullUid;
				}
				int count2 = this.OBFriendList.get_Count();
			}
			else if (curTab == COBSystem.enOBTab.Guild)
			{
				if (selectedIndex >= 0 && selectedIndex < this.OBGuildList.get_Count())
				{
					Singleton<WatchController>.GetInstance().TargetUID = this.OBGuildList.get_Item(selectedIndex).playerUid;
					Singleton<CGuildMatchSystem>.GetInstance().RequestObGuildMatch(this.OBGuildList.get_Item(selectedIndex).obUid);
					this.m_watchEntryData = new WatchEntryData();
					this.m_watchEntryData.headUrl = this.OBGuildList.get_Item(selectedIndex).headUrl;
					this.m_watchEntryData.name = this.OBGuildList.get_Item(selectedIndex).playerName;
					this.m_watchEntryData.rankClass = this.OBGuildList.get_Item(selectedIndex).dwClass;
					this.m_watchEntryData.rankGrade = this.OBGuildList.get_Item(selectedIndex).bGrade;
					this.m_watchEntryData.time = (long)CRoleInfo.GetCurrentUTCTime();
					this.m_watchEntryData.usedHeroId = this.OBGuildList.get_Item(selectedIndex).dwHeroID;
					this.m_watchEntryData.userUid = this.OBGuildList.get_Item(selectedIndex).playerUid;
				}
				int count3 = this.OBGuildList.get_Count();
			}
			else if (curTab == COBSystem.enOBTab.Local)
			{
				Singleton<WatchController>.GetInstance().TargetUID = Singleton<CRoleInfoManager>.instance.masterUUID;
				if (selectedIndex >= 0 && selectedIndex < this.OBLocalList.Count)
				{
					Singleton<WatchController>.GetInstance().StartReplay(this.OBLocalList[selectedIndex].path);
				}
				int count4 = this.OBLocalList.Count;
			}
		}

		public void OBFriend(COMDT_ACNT_UNIQ uin)
		{
			bool flag = false;
			for (int i = 0; i < this.OBFriendList.get_Count(); i++)
			{
				if (uin.ullUid == this.OBFriendList.get_Item(i).uin.ullUid)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				COBSystem.SendOBServeFriend(uin, -1);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips("OB_Error_1", false, 1.5f, null, new object[0]);
			}
		}

		private void On_Friend_SNS_STATE_NTF(CSPkg msg)
		{
			SCPKG_NTF_SNS_FRIEND stNtfSnsFriend = msg.stPkgData.stNtfSnsFriend;
			int num = 0;
			while ((long)num < (long)((ulong)stNtfSnsFriend.dwSnsFriendNum))
			{
				CSDT_SNS_FRIEND_INFO cSDT_SNS_FRIEND_INFO = stNtfSnsFriend.astSnsFriendList[num];
				if (cSDT_SNS_FRIEND_INFO != null && cSDT_SNS_FRIEND_INFO.bVideoState != 0)
				{
					if (cSDT_SNS_FRIEND_INFO.stGameInfo.stDetail == null)
					{
						DebugHelper.Assert(false, "SCPKG_NTF_SNS_FRIEND [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
					}
					else
					{
						bool flag = false;
						for (int i = 0; i < this.OBFriendList.get_Count(); i++)
						{
							if (stNtfSnsFriend.astSnsFriendList[num].stSnsFrindInfo.stUin.ullUid == this.OBFriendList.get_Item(i).uin.ullUid)
							{
								COBSystem.stOBFriend stOBFriend = this.OBFriendList.get_Item(i);
								stOBFriend.gameDetail = stNtfSnsFriend.astSnsFriendList[num].stGameInfo.stDetail;
								this.OBFriendList.set_Item(i, stOBFriend);
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							COBSystem.stOBFriend stOBFriend2 = default(COBSystem.stOBFriend);
							stOBFriend2.uin = stNtfSnsFriend.astSnsFriendList[num].stSnsFrindInfo.stUin;
							stOBFriend2.friendName = Utility.UTF8Convert(stNtfSnsFriend.astSnsFriendList[num].stSnsFrindInfo.szUserName);
							stOBFriend2.headUrl = Utility.UTF8Convert(stNtfSnsFriend.astSnsFriendList[num].stSnsFrindInfo.szHeadUrl);
							stOBFriend2.gameDetail = stNtfSnsFriend.astSnsFriendList[num].stGameInfo.stDetail;
							this.OBFriendList.Add(stOBFriend2);
						}
					}
				}
				num++;
			}
			this.UpdateView();
		}

		private void On_FriendSys_Friend_List(CSPkg msg)
		{
			SCPKG_CMD_LIST_FRIEND stFriendListRsp = msg.stPkgData.stFriendListRsp;
			int i = 0;
			while ((long)i < (long)((ulong)stFriendListRsp.dwFriendNum))
			{
				CSDT_FRIEND_INFO cSDT_FRIEND_INFO = stFriendListRsp.astFrindList[i];
				if (cSDT_FRIEND_INFO != null && cSDT_FRIEND_INFO.bVideoState != 0)
				{
					if (cSDT_FRIEND_INFO.stGameInfo.stDetail == null)
					{
						DebugHelper.Assert(false, "CSDT_FRIEND_INFO [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
					}
					else
					{
						bool flag = false;
						int num = 0;
						while (i < this.OBFriendList.get_Count())
						{
							if (stFriendListRsp.astFrindList[i].stFriendInfo.stUin.ullUid == this.OBFriendList.get_Item(num).uin.ullUid)
							{
								COBSystem.stOBFriend stOBFriend = this.OBFriendList.get_Item(num);
								stOBFriend.gameDetail = stFriendListRsp.astFrindList[i].stGameInfo.stDetail;
								this.OBFriendList.set_Item(num, stOBFriend);
								flag = true;
								break;
							}
							num++;
						}
						if (!flag)
						{
							COBSystem.stOBFriend stOBFriend2 = default(COBSystem.stOBFriend);
							stOBFriend2.uin = stFriendListRsp.astFrindList[i].stFriendInfo.stUin;
							stOBFriend2.friendName = Utility.UTF8Convert(stFriendListRsp.astFrindList[i].stFriendInfo.szUserName);
							stOBFriend2.headUrl = Utility.UTF8Convert(stFriendListRsp.astFrindList[i].stFriendInfo.szHeadUrl);
							stOBFriend2.gameDetail = stFriendListRsp.astFrindList[i].stGameInfo.stDetail;
							this.OBFriendList.Add(stOBFriend2);
						}
					}
				}
				i++;
			}
			this.UpdateView();
		}

		private void On_Friend_GAME_STATE_NTF(CSPkg msg)
		{
			SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState = msg.stPkgData.stNtfFriendGameState;
			COBSystem.stOBFriend stOBFriend;
			for (int i = 0; i < this.OBFriendList.get_Count(); i++)
			{
				if (this.OBFriendList.get_Item(i).uin.ullUid == stNtfFriendGameState.stAcntUniq.ullUid)
				{
					if (stNtfFriendGameState.bVideoState == 0)
					{
						this.OBFriendList.RemoveAt(i);
					}
					else if (stNtfFriendGameState.bVideoState == 1)
					{
						stOBFriend = this.OBFriendList.get_Item(i);
						stOBFriend.gameDetail = stNtfFriendGameState.stGameInfo.stDetail;
						this.OBFriendList.set_Item(i, stOBFriend);
					}
					this.UpdateView();
					return;
				}
			}
			if (stNtfFriendGameState.bVideoState == 0)
			{
				return;
			}
			if (stNtfFriendGameState.stGameInfo.stDetail == null)
			{
				DebugHelper.Assert(false, "SCPKG_CMD_NTF_FRIEND_GAME_STATE [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
				return;
			}
			COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, stNtfFriendGameState.stAcntUniq.ullUid, stNtfFriendGameState.stAcntUniq.dwLogicWorldId);
			if (info == null)
			{
				info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, stNtfFriendGameState.stAcntUniq.ullUid, stNtfFriendGameState.stAcntUniq.dwLogicWorldId);
			}
			if (info == null)
			{
				return;
			}
			stOBFriend = default(COBSystem.stOBFriend);
			stOBFriend.uin = stNtfFriendGameState.stAcntUniq;
			stOBFriend.friendName = Utility.UTF8Convert(info.szUserName);
			stOBFriend.headUrl = Utility.UTF8Convert(info.szHeadUrl);
			stOBFriend.gameDetail = stNtfFriendGameState.stGameInfo.stDetail;
			this.OBFriendList.Add(stOBFriend);
			this.UpdateView();
		}

		private void OnGetGreatMatch(CSPkg msg)
		{
			this.OBExpertList.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)msg.stPkgData.stGetGreatMatchRsp.dwCount))
			{
				int num2 = 0;
				while ((long)num2 < (long)((ulong)msg.stPkgData.stGetGreatMatchRsp.astList[num].dwLabelNum))
				{
					COBSystem.stOBExpert stOBExpert = default(COBSystem.stOBExpert);
					stOBExpert.desk = msg.stPkgData.stGetGreatMatchRsp.astList[num].stDesk;
					stOBExpert.startTime = msg.stPkgData.stGetGreatMatchRsp.astList[num].dwStartTime;
					stOBExpert.observeNum = msg.stPkgData.stGetGreatMatchRsp.astList[num].dwObserveNum;
					stOBExpert.heroLabel = msg.stPkgData.stGetGreatMatchRsp.astList[num].astLabel[num2];
					this.OBExpertList.Add(stOBExpert);
					num2++;
				}
				num++;
			}
			this.OBExpertList.Sort(new Comparison<COBSystem.stOBExpert>(this.SortByObserveNum));
			this.UpdateView();
		}

		private int SortByObserveNum(COBSystem.stOBExpert left, COBSystem.stOBExpert right)
		{
			return (int)(right.observeNum - left.observeNum);
		}

		public static void SendOBServeFriend(COMDT_ACNT_UNIQ uin, int friendType = -1)
		{
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5218u);
			if (friendType == -1)
			{
				friendType = (Singleton<CFriendContoller>.GetInstance().model.IsGameFriend(uin.ullUid, uin.dwLogicWorldId) ? 1 : 2);
			}
			cSPkg.stPkgData.stObserveFriendReq.bType = (byte)friendType;
			cSPkg.stPkgData.stObserveFriendReq.stFriendUniq = uin;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(5219)]
		public static void ON_OBSERVE_FRIEND_RSP(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			if (msg.stPkgData.stObserveFriendRsp.iResult == 0)
			{
				if (Singleton<WatchController>.GetInstance().StartObserve(msg.stPkgData.stObserveFriendRsp.stTgwinfo))
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				}
			}
			else
			{
				Singleton<COBSystem>.instance.UpdateView();
				Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stObserveFriendRsp.iResult), true, 1.5f, null, new object[0]);
			}
		}

		public static void SendOBServeGreat(COMDT_OB_DESK desk)
		{
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5220u);
			cSPkg.stPkgData.stObserveGreatReq.stDesk = desk;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(5221)]
		public static void ON_OBSERVE_GREAT_RSP(CSPkg msg)
		{
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
			if (msg.stPkgData.stObserveGreatRsp.iResult == 0)
			{
				if (Singleton<WatchController>.GetInstance().StartObserve(null))
				{
					Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				}
			}
			else
			{
				COBSystem.GetGreatMatch(true);
				Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stObserveGreatRsp.iResult), true, 1.5f, null, new object[0]);
			}
		}

		public static void GetGreatMatch(bool bForce = false)
		{
			if (bForce || CRoleInfo.GetCurrentUTCTime() - COBSystem.m_lastGetExpertListTime > COBSystem.EXPERT_DETAL_SEC)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5222u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
				COBSystem.m_lastGetExpertListTime = CRoleInfo.GetCurrentUTCTime();
			}
		}

		public static void GetFriendsState()
		{
			if (CRoleInfo.GetCurrentUTCTime() - COBSystem.m_lastGetFriendStateTime > COBSystem.FRIEND_DETAL_SEC)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5233u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				COBSystem.m_lastGetFriendStateTime = CRoleInfo.GetCurrentUTCTime();
			}
		}

		public void SetGuildMatchOBCount(SCPKG_GET_GUILD_MATCH_OB_CNT_RSP msg)
		{
			bool flag = false;
			for (int i = 0; i < (int)msg.bMatchCnt; i++)
			{
				COMDT_GUILD_MATCH_OB_CNT cOMDT_GUILD_MATCH_OB_CNT = msg.astMatchOBCnt[i];
				for (int j = 0; j < this.OBGuildList.get_Count(); j++)
				{
					if (this.OBGuildList.get_Item(j).obUid == cOMDT_GUILD_MATCH_OB_CNT.ullUid && this.OBGuildList.get_Item(j).dwObserveNum != cOMDT_GUILD_MATCH_OB_CNT.dwOBCnt)
					{
						this.OBGuildList.get_Item(j).dwObserveNum = cOMDT_GUILD_MATCH_OB_CNT.dwOBCnt;
						flag = true;
					}
				}
			}
			if (flag && this.CurTab == COBSystem.enOBTab.Guild)
			{
				this.UpdateView();
			}
		}

		[MessageHandler(5223)]
		public static void ON_GET_GREATMATCH_RSP(CSPkg msg)
		{
			Singleton<COBSystem>.instance.OnGetGreatMatch(msg);
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
		}
	}
}

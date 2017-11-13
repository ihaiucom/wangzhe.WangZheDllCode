using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
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
	public class CPlayerPvpHistoryController : Singleton<CPlayerPvpHistoryController>
	{
		private const int MAX_RECORD_COUNT = 100;

		private const int MaxAchievement = 8;

		private string m_WinMVPPath = CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Red_Mvp";

		private string m_LoseMVPPath = CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Blue_Mvp";

		private string DetailInfoFormPath = "UGUI/Form/System/Player/Form_History_Detail_Info";

		private bool bShowStatistics;

		private ListView<COMDT_PLAYER_FIGHT_RECORD> m_hostRecordList;

		private ListView<COMDT_PLAYER_FIGHT_RECORD> m_recordList = new ListView<COMDT_PLAYER_FIGHT_RECORD>();

		private ulong m_ullUid;

		private uint m_dwLogicWorldID;

		private COMDT_PLAYER_FIGHT_RECORD m_showRecord;

		private CPlayerProfile m_profile;

		private CUIFormScript m_form;

		private bool m_bBattleState;

		private static int mvpScoreOffset = 100;

		private List<stOnCall> m_onCallList = new List<stOnCall>();

		public void AddOnCall(stOnCall call)
		{
			for (int i = 0; i < this.m_onCallList.get_Count(); i++)
			{
				if (this.m_onCallList.get_Item(i).type == call.type)
				{
					this.m_onCallList.RemoveAt(i);
					i--;
				}
			}
			this.m_onCallList.Add(call);
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHistoryItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_DetailInfo, new CUIEventManager.OnUIEventHandler(this.OnClickDetailInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Close_Detail, new CUIEventManager.OnUIEventHandler(this.OnClickBackHistory));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Statistics, new CUIEventManager.OnUIEventHandler(this.OnClickStatistics));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnClickAddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_PvpHistory_OpenPlayerDetailInfo, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerDetailInfo));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.PlayerInfoSystem_Form_Close, new Action(this.Clear));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHistoryItemEnable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_DetailInfo, new CUIEventManager.OnUIEventHandler(this.OnClickDetailInfo));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Close_Detail, new CUIEventManager.OnUIEventHandler(this.OnClickBackHistory));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_Statistics, new CUIEventManager.OnUIEventHandler(this.OnClickStatistics));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_Click_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnClickAddFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_PvpHistory_OpenPlayerDetailInfo, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerDetailInfo));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerInfoSystem_Form_Close, new Action(this.Clear));
			this.m_form = null;
		}

		public bool Loaded(CUIFormScript form)
		{
			if (form == null)
			{
				return false;
			}
			GameObject widget = form.GetWidget(9);
			if (widget == null)
			{
				return false;
			}
			GameObject x = Utility.FindChild(widget, "pnlPvPHistory");
			return !(x == null);
		}

		public void Load(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/PvPHistory", "pnlPvPHistory", form.GetWidget(9), form);
		}

		public void Draw(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(9);
			if (widget == null)
			{
				return;
			}
			this.m_profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
			this.m_form = form;
			GameObject gameObject = Utility.FindChild(widget, "pnlPvPHistory");
			gameObject.CustomSetActive(true);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (CRoleInfo.IsPrivacyOpen(this.m_profile.m_userPrivacyBits, COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_FIGHT_HISTORY) && (this.m_profile.m_uuid != masterRoleInfo.playerUllUID || MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID != this.m_profile.m_iLogicWorldId))
			{
				CUIListScript component = gameObject.transform.FindChild("pnlContainer/HistoryList").GetComponent<CUIListScript>();
				component.SetElementAmount(0);
				Utility.FindChild(gameObject, "pnlContainer/title").CustomSetActive(false);
				CUICommonSystem.SetTextContent(component.m_ZeroTipsObj.transform.FindChild("Text"), Singleton<CTextManager>.GetInstance().GetText("Fight_History_PrivacyOpen"));
			}
			else
			{
				CUIListScript component2 = gameObject.transform.FindChild("pnlContainer/HistoryList").GetComponent<CUIListScript>();
				CUICommonSystem.SetTextContent(component2.m_ZeroTipsObj.transform.FindChild("Text"), Singleton<CTextManager>.GetInstance().GetText("Fight_History_NoneRecord"));
				if (this.m_ullUid == 0uL && this.m_dwLogicWorldID == 0u)
				{
					if (this.m_profile.m_uuid != masterRoleInfo.playerUllUID || MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID != this.m_profile.m_iLogicWorldId || this.m_hostRecordList == null)
					{
						this.ReqHistoryInfo(this.m_profile.m_uuid, (uint)this.m_profile.m_iLogicWorldId);
						component2.SetElementAmount(0);
						gameObject.transform.FindChild("pnlContainer/HistoryList_node").gameObject.CustomSetActive(false);
						GameObject obj = Utility.FindChild(gameObject, "pnlContainer/title");
						obj.CustomSetActive(this.m_recordList.Count != 0);
						return;
					}
					this.m_recordList.Clear();
					this.m_recordList.AddRange(this.m_hostRecordList);
				}
				this.initHistoryList();
			}
		}

		private void Clear()
		{
			this.m_ullUid = 0uL;
			this.m_dwLogicWorldID = 0u;
			this.m_recordList.Clear();
			this.m_profile = null;
			this.bShowStatistics = false;
			this.m_form = null;
		}

		private void OnHistoryItemEnable(CUIEvent uiEvt)
		{
			PvpHistoryItemHelper component = uiEvt.m_srcWidget.GetComponent<PvpHistoryItemHelper>();
			if (component == null || uiEvt.m_srcWidgetIndexInBelongedList >= this.m_recordList.Count || uiEvt.m_srcWidgetIndexInBelongedList < 0)
			{
				return;
			}
			COMDT_PLAYER_FIGHT_RECORD cOMDT_PLAYER_FIGHT_RECORD = this.m_recordList[uiEvt.m_srcWidgetIndexInBelongedList];
			COMDT_PLAYER_FIGHT_DATA playerFightData = this.GetPlayerFightData(this.m_profile.m_uuid, this.m_profile.m_iLogicWorldId, ref cOMDT_PLAYER_FIGHT_RECORD);
			if (playerFightData == null)
			{
				return;
			}
			Image component2 = component.Mvp.GetComponent<Image>();
			if (this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP))
			{
				component.Mvp.CustomSetActive(true);
				component2.SetSprite(this.m_WinMVPPath, uiEvt.m_srcFormScript, true, false, false, false);
			}
			else if (this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP))
			{
				component.Mvp.CustomSetActive(true);
				component2.SetSprite(this.m_LoseMVPPath, uiEvt.m_srcFormScript, true, false, false, false);
			}
			else
			{
				component.Mvp.CustomSetActive(false);
			}
			string strContent = (cOMDT_PLAYER_FIGHT_RECORD.bWinCamp == playerFightData.bPlayerCamp) ? Singleton<CTextManager>.instance.GetText("GameResult_Win") : Singleton<CTextManager>.instance.GetText("GameResult_Lose");
			CUICommonSystem.SetTextContent(component.reSesultText, strContent);
			component.KDAText.GetComponent<Text>().set_text(string.Format("{0} / {1} / {2}", playerFightData.bKill, playerFightData.bDead, playerFightData.bAssist));
			component.SetEuipItems(ref playerFightData, uiEvt.m_srcFormScript);
			component.headObj.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(playerFightData.dwHeroID, 0u)), this.m_form, true, false, false, false);
			component.FiveFriendItem.CustomSetActive(this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER5V5));
			component.FourFriendItem.CustomSetActive(this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER4));
			component.ThreeFriendItem.CustomSetActive(this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER3));
			component.TwoFriendItem.CustomSetActive(this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER2));
			component.FriendItem.CustomSetActive(this.CanShowPlayerWithFriend(playerFightData.dwRongyuFlag));
			COM_GAME_TYPE bGameType = (COM_GAME_TYPE)cOMDT_PLAYER_FIGHT_RECORD.bGameType;
			if (bGameType != COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)
			{
				if (bGameType != COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM)
				{
					if (bGameType != COM_GAME_TYPE.COM_MULTI_GAME_OF_GUILDMATCH)
					{
						ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(cOMDT_PLAYER_FIGHT_RECORD.bMapType, cOMDT_PLAYER_FIGHT_RECORD.dwMapID);
						if (pvpMapCommonInfo != null)
						{
							component.MatchTypeText.GetComponent<Text>().set_text(pvpMapCommonInfo.szGameMatchName);
						}
					}
					else
					{
						component.MatchTypeText.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("战队赛"));
					}
				}
				else
				{
					component.MatchTypeText.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips1"));
				}
			}
			else if (!this.IsFightAchiveSet(playerFightData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE))
			{
				component.MatchTypeText.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("人机对战"));
			}
			else
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(cOMDT_PLAYER_FIGHT_RECORD.bMapType, cOMDT_PLAYER_FIGHT_RECORD.dwMapID);
				if (pvpMapCommonInfo2 != null)
				{
					component.MatchTypeText.GetComponent<Text>().set_text(pvpMapCommonInfo2.szGameMatchName);
				}
			}
			DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			DateTime dateTime2 = Utility.ToUtcTime2Local((long)((ulong)cOMDT_PLAYER_FIGHT_RECORD.dwGameStartTime));
			int days = (dateTime.get_Date() - dateTime2.get_Date()).get_Days();
			Text component3 = component.time.transform.FindChild("Today").GetComponent<Text>();
			string text = string.Empty;
			switch (days)
			{
			case 0:
				text = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips3"), dateTime2.get_Hour(), dateTime2.get_Minute());
				break;
			case 1:
				text = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips4"), dateTime2.get_Hour(), dateTime2.get_Minute());
				break;
			case 2:
				text = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips5"), dateTime2.get_Hour(), dateTime2.get_Minute());
				break;
			default:
				text = string.Format(Singleton<CTextManager>.GetInstance().GetText("HistoryInfo_Tips6"), new object[]
				{
					dateTime2.get_Month(),
					dateTime2.get_Day(),
					dateTime2.get_Hour(),
					dateTime2.get_Minute()
				});
				break;
			}
			component3.set_text(text);
			component.ShowDetailBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = uiEvt.m_srcWidgetIndexInBelongedList;
			component.ShowDetailBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag2 = (int)playerFightData.bPlayerCamp;
		}

		private void OnClickDetailInfo(CUIEvent uiEvt)
		{
			if (!Singleton<CPlayerInfoSystem>.GetInstance().IsForbidShowPvpHistoryDetailInfo)
			{
				this.bShowStatistics = false;
				CUIFormScript detailForm = Singleton<CUIManager>.GetInstance().OpenForm(this.DetailInfoFormPath, true, true);
				this.initSettleInfoPanel(detailForm, uiEvt.m_eventParams.tag, uiEvt.m_eventParams.tag2, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PvpHistory_CanNotOpenPlayerInfoFormFromHere", true, 1.5f, null, new object[0]);
			}
		}

		private void OnClickBackHistory(CUIEvent uiEvt)
		{
			Singleton<CUIManager>.instance.CloseForm(this.DetailInfoFormPath);
		}

		private void OnClickStatistics(CUIEvent uiEvt)
		{
			Transform transform = uiEvt.m_srcFormScript.transform.FindChild("pnlContainer/Panel");
			if (transform == null)
			{
				return;
			}
			this.bShowStatistics = !this.bShowStatistics;
			transform.FindChild("LeftPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!this.bShowStatistics);
			transform.FindChild("LeftPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(this.bShowStatistics);
			transform.FindChild("RightPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!this.bShowStatistics);
			transform.FindChild("RightPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(this.bShowStatistics);
			transform.FindChild("SwitchStatistics").gameObject.CustomSetActive(!this.bShowStatistics);
			transform.FindChild("SwitchOverview").gameObject.CustomSetActive(this.bShowStatistics);
			CUIListScript component = transform.FindChild("LeftPlayerPanel/LeftPlayerList").GetComponent<CUIListScript>();
			CUIListScript component2 = transform.FindChild("RightPlayerPanel/RightPlayerList").GetComponent<CUIListScript>();
			if (component != null)
			{
				int elementAmount = component.GetElementAmount();
				for (int i = 0; i < elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					SettlementHelper component3 = elemenet.gameObject.GetComponent<SettlementHelper>();
					component3.Detail.CustomSetActive(!this.bShowStatistics);
					component3.Damage.CustomSetActive(this.bShowStatistics);
				}
			}
			if (component2 != null)
			{
				int elementAmount2 = component2.GetElementAmount();
				for (int j = 0; j < elementAmount2; j++)
				{
					CUIListElementScript elemenet2 = component2.GetElemenet(j);
					SettlementHelper component4 = elemenet2.gameObject.GetComponent<SettlementHelper>();
					component4.Detail.CustomSetActive(!this.bShowStatistics);
					component4.Damage.CustomSetActive(this.bShowStatistics);
				}
			}
		}

		private void OnClickAddFriend(CUIEvent uiEvent)
		{
			if (Singleton<SettlementSystem>.GetInstance().IsInSettlementState())
			{
				Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint)uiEvent.m_eventParams.commonUInt64Param2, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_PVP, uiEvent.m_eventParams.tag, true);
			}
			else
			{
				Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint)uiEvent.m_eventParams.commonUInt64Param2, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
			}
			uiEvent.m_srcWidget.CustomSetActive(false);
		}

		private void OnOpenPlayerDetailInfo(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			int tag = uiEvent.m_eventParams.tag;
			bool commonBool = uiEvent.m_eventParams.commonBool;
			if (Utility.IsValidPlayer(commonUInt64Param, tag) && !Utility.IsSelf(commonUInt64Param, tag) && !commonBool)
			{
				Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(commonUInt64Param, tag, CPlayerInfoSystem.DetailPlayerInfoSource.PvpHistoryDetailInfo, true, CPlayerInfoSystem.Tab.Base_Info);
			}
		}

		private void OnReciveFightData()
		{
			this.initHistoryList();
			while (this.m_onCallList.get_Count() != 0)
			{
				stOnCall stOnCall = this.m_onCallList.get_Item(0);
				this.m_onCallList.RemoveAt(0);
				this.DoOnCall(ref stOnCall);
			}
		}

		private void initHistoryList()
		{
			if (this.m_form == null)
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(9);
			if (widget == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "pnlPvPHistory");
			if (gameObject == null)
			{
				return;
			}
			CUIListScript component = gameObject.transform.FindChild("pnlContainer/HistoryList").GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_recordList.Count);
			GameObject obj = Utility.FindChild(gameObject, "pnlContainer/title");
			obj.CustomSetActive(this.m_recordList.Count != 0);
		}

		public void OpenDetailPvpHistory(uint dwDeskID, uint dwDeskSeq)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			stOnCall call;
			call.type = enOnCallType.ShowOnePvpHistory;
			call.arg0 = (ulong)dwDeskID;
			call.arg1 = (ulong)dwDeskSeq;
			call.arg2 = masterRoleInfo.playerUllUID;
			call.arg3 = (ulong)((long)masterRoleInfo.logicWorldID);
			Singleton<CPlayerInfoSystem>.instance.GetProfile().ConvertRoleInfoData(masterRoleInfo);
			this.m_profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			if (this.m_hostRecordList == null)
			{
				this.ReqHistoryInfo(masterRoleInfo.playerUllUID, (uint)masterRoleInfo.logicWorldID);
				Singleton<CPlayerPvpHistoryController>.instance.AddOnCall(call);
			}
			else
			{
				this.m_recordList.Clear();
				this.m_recordList.AddRange(this.m_hostRecordList);
				this.DoOnCall(ref call);
			}
		}

		private void DoOnCall(ref stOnCall call)
		{
			if (call.type == enOnCallType.ShowOnePvpHistory)
			{
				int num = -1;
				for (int i = 0; i < this.m_recordList.Count; i++)
				{
					if (this.m_recordList[i].dwDeskID == (uint)call.arg0 && this.m_recordList[i].dwDeskSeq == (uint)call.arg1)
					{
						num = i;
					}
				}
				if (num < 0 || num >= this.m_recordList.Count)
				{
					Singleton<CUIManager>.instance.OpenMessageBox(Singleton<CTextManager>.instance.GetText("GotoPvpHistoryErr1"), false);
					return;
				}
				COMDT_PLAYER_FIGHT_RECORD cOMDT_PLAYER_FIGHT_RECORD = this.m_recordList[num];
				COMDT_PLAYER_FIGHT_DATA playerFightData = this.GetPlayerFightData(call.arg2, (int)call.arg3, ref cOMDT_PLAYER_FIGHT_RECORD);
				if (playerFightData == null)
				{
					return;
				}
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Player_Info_PvpHistory_Click_DetailInfo;
				cUIEvent.m_eventParams.tag = num;
				cUIEvent.m_eventParams.tag2 = (int)playerFightData.bPlayerCamp;
				Singleton<CUIEventManager>.instance.DispatchUIEvent(cUIEvent);
			}
		}

		private void initSettleInfoPanel(CUIFormScript detailForm, int infoIndex, int hostPlayerCamp, bool bShowStatistics)
		{
			if (detailForm == null || infoIndex >= this.m_recordList.Count || infoIndex < 0)
			{
				return;
			}
			Transform transform = detailForm.transform.FindChild("pnlContainer/Panel");
			this.m_showRecord = this.m_recordList[infoIndex];
			uint num = this.m_showRecord.dwGameTime / 60u;
			uint num2 = this.m_showRecord.dwGameTime - num * 60u;
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)this.m_showRecord.dwGameStartTime));
			CUICommonSystem.SetTextContent(transform.FindChild("Time/Duration"), string.Format("{0:D2}:{1:D2}", num, num2));
			CUICommonSystem.SetTextContent(transform.FindChild("StartTime"), string.Format("{0:D4}/{1:D2}/{2:D2} {3}:{4}", new object[]
			{
				dateTime.get_Year(),
				dateTime.get_Month(),
				dateTime.get_Day(),
				dateTime.get_Hour(),
				dateTime.get_Minute()
			}));
			bool flag = (int)this.m_showRecord.bWinCamp == hostPlayerCamp;
			transform.FindChild("WinLoseTitle/Win").gameObject.CustomSetActive(flag);
			transform.FindChild("WinLoseTitle/Lose").gameObject.CustomSetActive(!flag);
			transform.FindChild("SwitchStatistics").gameObject.CustomSetActive(!bShowStatistics);
			transform.FindChild("SwitchOverview").gameObject.CustomSetActive(bShowStatistics);
			transform.FindChild("LeftPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!bShowStatistics);
			transform.FindChild("LeftPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(bShowStatistics);
			transform.FindChild("RightPlayerPanel/OverviewMenu").gameObject.CustomSetActive(!bShowStatistics);
			transform.FindChild("RightPlayerPanel/StatisticsMenu").gameObject.CustomSetActive(bShowStatistics);
			ListView<COMDT_PLAYER_FIGHT_DATA> campPlayerDataList = this.GetCampPlayerDataList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ref this.m_showRecord);
			ListView<COMDT_PLAYER_FIGHT_DATA> campPlayerDataList2 = this.GetCampPlayerDataList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ref this.m_showRecord);
			CUIListScript component = transform.FindChild("LeftPlayerPanel/LeftPlayerList").GetComponent<CUIListScript>();
			CUIListScript component2 = transform.FindChild("RightPlayerPanel/RightPlayerList").GetComponent<CUIListScript>();
			int num3 = 0;
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			int num7 = 0;
			uint num8 = 0u;
			uint num9 = 0u;
			uint num10 = 0u;
			int totlePlayerNum = campPlayerDataList.Count + campPlayerDataList2.Count;
			for (int i = 0; i < campPlayerDataList.Count; i++)
			{
				COMDT_PLAYER_FIGHT_DATA cOMDT_PLAYER_FIGHT_DATA = campPlayerDataList[i];
				num3 += (int)cOMDT_PLAYER_FIGHT_DATA.bKill;
				num4 += cOMDT_PLAYER_FIGHT_DATA.dwHurtToEnemy;
				num5 += cOMDT_PLAYER_FIGHT_DATA.dwHurtToHero;
				num6 += cOMDT_PLAYER_FIGHT_DATA.dwHurtTakenByEnemy;
			}
			for (int j = 0; j < campPlayerDataList2.Count; j++)
			{
				COMDT_PLAYER_FIGHT_DATA cOMDT_PLAYER_FIGHT_DATA2 = campPlayerDataList2[j];
				num7 += (int)cOMDT_PLAYER_FIGHT_DATA2.bKill;
				num8 += cOMDT_PLAYER_FIGHT_DATA2.dwHurtToEnemy;
				num9 += cOMDT_PLAYER_FIGHT_DATA2.dwHurtToHero;
				num10 += cOMDT_PLAYER_FIGHT_DATA2.dwHurtTakenByEnemy;
			}
			component.SetElementAmount(campPlayerDataList.Count);
			for (int k = 0; k < campPlayerDataList.Count; k++)
			{
				COMDT_PLAYER_FIGHT_DATA playerData = campPlayerDataList[k];
				CUIListElementScript elemenet = component.GetElemenet(k);
				this.initSettleListItem(elemenet, playerData, num4, num6, num5, totlePlayerNum);
			}
			component2.SetElementAmount(campPlayerDataList2.Count);
			for (int l = 0; l < campPlayerDataList2.Count; l++)
			{
				COMDT_PLAYER_FIGHT_DATA playerData2 = campPlayerDataList2[l];
				CUIListElementScript elemenet2 = component2.GetElemenet(l);
				this.initSettleListItem(elemenet2, playerData2, num8, num10, num9, totlePlayerNum);
			}
			CUICommonSystem.SetTextContent(transform.FindChild("TotalScore/LeftTotalKill"), num3.ToString());
			CUICommonSystem.SetTextContent(transform.FindChild("TotalScore/RightTotalKill"), num7.ToString());
		}

		private void initSettleListItem(CUIListElementScript listItem, COMDT_PLAYER_FIGHT_DATA playerData, uint inTotalDamage, uint inTotalTakenDamage, uint inTotalToHeroDamage, int totlePlayerNum)
		{
			if (listItem == null || playerData == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			SettlementHelper component = listItem.GetComponent<SettlementHelper>();
			component.Detail.CustomSetActive(true);
			component.Damage.CustomSetActive(false);
			this.UpdateEquip(component.Tianfu, playerData);
			this.UpdateAchievements(component.Achievements, playerData);
			component.WinMvpIcon.SetActive(false);
			component.WinMvpTxt.gameObject.SetActive(false);
			component.LoseMvpIcon.SetActive(false);
			component.LoseMvpTxt.gameObject.SetActive(false);
			component.NormalMvpIcon.SetActive(false);
			component.NormalMvpTxt.gameObject.SetActive(false);
			if (totlePlayerNum != 2)
			{
				if (this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP))
				{
					component.WinMvpIcon.SetActive(true);
					component.WinMvpTxt.gameObject.SetActive(true);
					component.LoseMvpIcon.SetActive(false);
					component.LoseMvpTxt.gameObject.SetActive(false);
					component.NormalMvpIcon.SetActive(false);
					component.NormalMvpTxt.gameObject.SetActive(false);
					component.WinMvpTxt.set_text((Math.Max((float)playerData.iMvpScoreTTH, 0f) / (float)CPlayerPvpHistoryController.mvpScoreOffset).ToString("F1"));
				}
				else if (this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP))
				{
					component.WinMvpIcon.SetActive(false);
					component.WinMvpTxt.gameObject.SetActive(false);
					component.LoseMvpIcon.SetActive(true);
					component.LoseMvpTxt.gameObject.SetActive(true);
					component.NormalMvpIcon.SetActive(false);
					component.NormalMvpTxt.gameObject.SetActive(false);
					component.LoseMvpTxt.set_text((Math.Max((float)playerData.iMvpScoreTTH, 0f) / (float)CPlayerPvpHistoryController.mvpScoreOffset).ToString("F1"));
				}
				else
				{
					component.WinMvpIcon.SetActive(false);
					component.WinMvpTxt.gameObject.SetActive(false);
					component.LoseMvpIcon.SetActive(false);
					component.LoseMvpTxt.gameObject.SetActive(false);
					component.NormalMvpIcon.SetActive(true);
					component.NormalMvpTxt.gameObject.SetActive(true);
					component.NormalMvpTxt.set_text((Math.Max((float)playerData.iMvpScoreTTH, 0f) / (float)CPlayerPvpHistoryController.mvpScoreOffset).ToString("F1"));
				}
			}
			component.PlayerName.GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref playerData.szPlayerName));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int)playerData.bPlayerVipLv, false, true, 0uL);
			if ((playerData.ullPlayerUid == 0uL || playerData.ullPlayerUid == this.m_profile.m_uuid) && !this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER))
			{
				component.PlayerName.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Self);
				component.ItsMe.CustomSetActive(true);
				if (masterRoleInfo.playerUllUID == this.m_profile.m_uuid)
				{
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
				}
			}
			else
			{
				component.ItsMe.CustomSetActive(false);
				if (playerData.bPlayerCamp == 1)
				{
					component.PlayerName.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Camp_1);
				}
				else
				{
					component.PlayerName.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Camp_2);
				}
			}
			if (playerData.ullPlayerUid == 0uL || playerData.ullPlayerUid == masterRoleInfo.playerUllUID || Singleton<CFriendContoller>.instance.model.IsGameFriend(playerData.ullPlayerUid, (uint)playerData.iPlayerLogicWorldID) || (this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER) && !this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE)))
			{
				component.AddFriend.CustomSetActive(false);
				component.m_AddfriendBtnShow = false;
			}
			else
			{
				component.AddFriend.CustomSetActive(true);
				component.m_AddfriendBtnShow = true;
				CUIEventScript component2 = component.AddFriend.GetComponent<CUIEventScript>();
				component2.m_onClickEventParams.commonUInt64Param1 = playerData.ullPlayerUid;
				component2.m_onClickEventParams.commonUInt64Param2 = (ulong)((long)playerData.iPlayerLogicWorldID);
				component2.m_onClickEventParams.tag = Singleton<SettlementSystem>.GetInstance().HostPlayerHeroId;
			}
			Image component3 = component.HeroIcon.GetComponent<Image>();
			component3.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(playerData.dwHeroID, 0u)), this.m_form, true, false, false, false);
			component.HeroLv.GetComponent<Text>().set_text(string.Format("{0}", playerData.bHeroLv));
			component.Kill.GetComponent<Text>().set_text(playerData.bKill.ToString());
			component.Death.GetComponent<Text>().set_text(playerData.bDead.ToString());
			component.Assist.GetComponent<Text>().set_text(playerData.bAssist.ToString());
			component.Coin.GetComponent<Text>().set_text(playerData.dwTotalCoin.ToString());
			uint num = Math.Max(1u, inTotalDamage);
			uint num2 = Math.Max(1u, inTotalTakenDamage);
			uint num3 = Math.Max(1u, inTotalToHeroDamage);
			component.Damage.transform.FindChild("TotalDamageBg/TotalDamage").gameObject.GetComponent<Text>().set_text(playerData.dwHurtToEnemy.ToString());
			component.Damage.transform.FindChild("TotalDamageBg/TotalDamageBar").gameObject.GetComponent<Image>().set_fillAmount(playerData.dwHurtToEnemy / num);
			component.Damage.transform.FindChild("TotalDamageBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", playerData.dwHurtToEnemy / num));
			component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamage").gameObject.GetComponent<Text>().set_text(playerData.dwHurtTakenByEnemy.ToString());
			component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamageBar").gameObject.GetComponent<Image>().set_fillAmount(playerData.dwHurtTakenByEnemy / num2);
			component.Damage.transform.FindChild("TotalTakenDamageBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", playerData.dwHurtTakenByEnemy / num2));
			component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHero").gameObject.GetComponent<Text>().set_text(playerData.dwHurtToHero.ToString());
			component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHeroBar").gameObject.GetComponent<Image>().set_fillAmount(playerData.dwHurtToHero / num3);
			component.Damage.transform.FindChild("TotalDamageHeroBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", playerData.dwHurtToHero / num3));
			component.Detail.CustomSetActive(true);
			component.Damage.CustomSetActive(false);
			CUIEventScript component4 = component.HeroIcon.GetComponent<CUIEventScript>();
			CUIEventScript component5 = component.HeroIconBg.GetComponent<CUIEventScript>();
			if (Utility.IsSelf(playerData.ullPlayerUid, playerData.iPlayerLogicWorldID))
			{
				if (component4 != null)
				{
					component4.m_onClickEventParams.commonUInt64Param1 = 0uL;
					component4.m_onClickEventParams.tag = 0;
				}
				if (component5 != null)
				{
					component5.m_onClickEventParams.commonUInt64Param1 = 0uL;
					component5.m_onClickEventParams.tag = 0;
				}
			}
			else
			{
				bool commonBool = this.IsFightAchiveSet(playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER);
				if (component4 != null)
				{
					component4.m_onClickEventParams.commonUInt64Param1 = playerData.ullPlayerUid;
					component4.m_onClickEventParams.tag = playerData.iPlayerLogicWorldID;
					component4.m_onClickEventParams.commonBool = commonBool;
				}
				if (component5 != null)
				{
					component5.m_onClickEventParams.commonUInt64Param1 = playerData.ullPlayerUid;
					component5.m_onClickEventParams.tag = playerData.iPlayerLogicWorldID;
					component5.m_onClickEventParams.commonBool = commonBool;
				}
			}
		}

		private void UpdateEquip(GameObject equip, COMDT_PLAYER_FIGHT_DATA kda)
		{
			int num = 1;
			if (equip == null || kda == null)
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				uint dwEquipID = kda.astEquipDetail[i].dwEquipID;
				Transform transform = equip.transform.FindChild(string.Format("TianFu{0}", num));
				if (dwEquipID != 0u && !(transform == null))
				{
					num++;
					CUICommonSystem.SetEquipIcon((ushort)dwEquipID, transform.gameObject, this.m_form);
				}
			}
			for (int j = num; j <= 6; j++)
			{
				Transform transform2 = equip.transform.FindChild(string.Format("TianFu{0}", j));
				if (!(transform2 == null))
				{
					transform2.gameObject.GetComponent<Image>().SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), this.m_form, true, false, false, false);
				}
			}
		}

		private void SetAchievementIcon(GameObject achievements, PvpAchievement type, int index)
		{
			if (index > 8 || achievements == null)
			{
				return;
			}
			Transform transform = achievements.transform.FindChild(string.Format("Achievement{0}", index));
			if (transform == null)
			{
				return;
			}
			if (type == PvpAchievement.NULL)
			{
				transform.gameObject.CustomSetActive(false);
			}
			else
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, type.ToString());
				transform.gameObject.CustomSetActive(true);
				transform.GetComponent<Image>().SetSprite(prefabPath, this.m_form, true, false, false, false);
			}
		}

		private void UpdateAchievements(GameObject achievements, COMDT_PLAYER_FIGHT_DATA kda)
		{
			int num = 1;
			bool flag = false;
			for (int i = 0; i < 12; i++)
			{
				switch (i)
				{
				case 0:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.Legendary, num);
						num++;
					}
					break;
				case 1:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL) && !flag)
					{
						this.SetAchievementIcon(achievements, PvpAchievement.PentaKill, num);
						num++;
						flag = true;
					}
					break;
				case 2:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL) && !flag)
					{
						this.SetAchievementIcon(achievements, PvpAchievement.QuataryKill, num);
						num++;
						flag = true;
					}
					break;
				case 3:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL) && !flag)
					{
						this.SetAchievementIcon(achievements, PvpAchievement.TripleKill, num);
						num++;
						flag = true;
					}
					break;
				case 5:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.KillMost, num);
						num++;
					}
					break;
				case 6:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.HurtMost, num);
						num++;
					}
					break;
				case 7:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.HurtTakenMost, num);
						num++;
					}
					break;
				case 8:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.AsssistMost, num);
						num++;
					}
					break;
				case 9:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.GetCoinMost, num);
						num++;
					}
					break;
				case 10:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.KillOrganMost, num);
						num++;
					}
					break;
				case 11:
					if (this.IsFightAchiveSet(kda.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY))
					{
						this.SetAchievementIcon(achievements, PvpAchievement.RunAway, num);
						num++;
					}
					break;
				}
			}
			for (int j = num; j <= 8; j++)
			{
				this.SetAchievementIcon(achievements, PvpAchievement.NULL, j);
			}
		}

		public void AddSelfRecordData(COMDT_PLAYER_FIGHT_RECORD record)
		{
			if (this.m_hostRecordList == null)
			{
				return;
			}
			if (this.m_hostRecordList.Count >= 100)
			{
				this.m_hostRecordList.RemoveAt(this.m_hostRecordList.Count - 1);
			}
			this.m_hostRecordList.Add(record);
			this.m_hostRecordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
		}

		private void CreatHistoryInfoRePortData(bool bHostPlayerWin, ref COMDT_PLAYER_FIGHT_RECORD record)
		{
			COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			record.bGameType = Convert.ToByte(curLvelContext.GetGameType());
			if (bHostPlayerWin)
			{
				record.bWinCamp = Convert.ToByte(playerCamp);
			}
			else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				record.bWinCamp = Convert.ToByte(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
			else
			{
				record.bWinCamp = Convert.ToByte(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
			ScoreBoard scoreBoard = Singleton<CBattleSystem>.GetInstance().FightForm.scoreBoard;
			if (scoreBoard != null)
			{
				record.dwGameStartTime = (uint)scoreBoard.GetStartTime();
				record.dwGameTime = (uint)scoreBoard.GetDuration();
			}
			record.bMapType = (byte)curLvelContext.m_mapType;
			record.dwMapID = (uint)curLvelContext.m_mapID;
			record.dwDeskID = curLvelContext.dwDeskId;
			record.dwDeskSeq = curLvelContext.dwDeskSeq;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetEnumerator();
			byte b = 0;
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				bool bWin = (value.PlayerCamp == playerCamp) ? bHostPlayerWin : (!bHostPlayerWin);
				this.CreateHistoryInfoPlayerData(value, bWin, curLvelContext.m_isWarmBattle, ref record.astPlayerFightData[(int)b]);
				b += 1;
			}
			record.bPlayerCnt = b;
			this.AddSelfRecordData(record);
		}

		private void CreateHistoryInfoPlayerData(PlayerKDA playerKDA, bool bWin, bool bWarmBattle, ref COMDT_PLAYER_FIGHT_DATA playerData)
		{
			if (playerKDA == null)
			{
				return;
			}
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() == null)
			{
				return;
			}
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			bool bOpen = false;
			bool bOpen2 = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				playerData.dwHeroID = (uint)enumerator.Current.HeroId;
				playerData.bHeroLv = (byte)enumerator.Current.SoulLevel;
				num4 += enumerator.Current.LegendaryNum;
				num5 += enumerator.Current.PentaKillNum;
				num6 += enumerator.Current.QuataryKillNum;
				num7 += enumerator.Current.TripleKillNum;
				num8 += enumerator.Current.DoubleKillNum;
				num += (uint)enumerator.Current.hurtToEnemy;
				num2 += (uint)enumerator.Current.hurtTakenByEnemy;
				num3 += (uint)enumerator.Current.hurtToHero;
				flag3 = (flag3 || enumerator.Current.bHurtTakenMost);
				flag2 = (flag2 || enumerator.Current.bGetCoinMost);
				flag = (flag || enumerator.Current.bHurtMost);
				flag5 = (flag5 || enumerator.Current.bAsssistMost);
				flag4 = (flag4 || enumerator.Current.bKillMost);
				flag6 = (flag6 || enumerator.Current.bKillOrganMost);
				stEquipInfo[] equips = enumerator.Current.Equips;
				byte b = 0;
				int i = 0;
				int num9 = 0;
				while (i < equips.Length)
				{
					if (equips[i].m_equipID != 0)
					{
						playerData.astEquipDetail[num9].bCnt = (byte)equips[i].m_amount;
						playerData.astEquipDetail[num9].dwEquipID = (uint)equips[i].m_equipID;
						b += 1;
						num9++;
					}
					i++;
				}
				playerData.bEquipNum = b;
			}
			if (bWin)
			{
				uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
				if (mvpPlayer != 0u)
				{
					bOpen = (mvpPlayer == playerKDA.PlayerId);
				}
			}
			else
			{
				uint mvpPlayer2 = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
				if (mvpPlayer2 != 0u)
				{
					bOpen2 = (mvpPlayer2 == playerKDA.PlayerId);
				}
			}
			StringHelper.StringToUTF8Bytes(playerKDA.PlayerName, ref playerData.szPlayerName);
			playerData.bPlayerCamp = (byte)playerKDA.PlayerCamp;
			playerData.ullPlayerUid = playerKDA.PlayerUid;
			playerData.iPlayerLogicWorldID = playerKDA.WorldId;
			playerData.bPlayerLv = (byte)playerKDA.PlayerLv;
			playerData.bPlayerVipLv = (byte)playerKDA.PlayerVipLv;
			playerData.bKill = (byte)playerKDA.numKill;
			playerData.bDead = (byte)playerKDA.numDead;
			playerData.bAssist = (byte)playerKDA.numAssist;
			playerData.dwHurtToEnemy = num;
			playerData.dwHurtTakenByEnemy = num2;
			playerData.dwHurtToHero = num3;
			float num10 = 0f;
			if (!Singleton<BattleStatistic>.instance.GetServerMvpScore(playerKDA.PlayerId, out num10))
			{
				num10 = (bWin ? playerKDA.MvpValue : (playerKDA.MvpValue * (GameDataMgr.globalInfoDatabin.GetDataByKey(296u).dwConfValue / 100f)));
			}
			if (bWin)
			{
				ResGlobalInfo resGlobalInfo = null;
				int num11 = 1;
				int num12 = 0;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(25u, out resGlobalInfo))
				{
					num11 = (int)resGlobalInfo.dwConfValue;
				}
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(26u, out resGlobalInfo))
				{
					num12 = (int)resGlobalInfo.dwConfValue;
				}
				num10 = num10 * (float)num11 + (float)num12;
			}
			else
			{
				ResGlobalInfo resGlobalInfo2 = null;
				int num13 = 1;
				int num14 = 0;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(27u, out resGlobalInfo2))
				{
					num13 = (int)resGlobalInfo2.dwConfValue;
				}
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(28u, out resGlobalInfo2))
				{
					num14 = (int)resGlobalInfo2.dwConfValue;
				}
				num10 = num10 * (float)num13 + (float)num14;
			}
			playerData.iMvpScoreTTH = Math.Max((int)(num10 * (float)CPlayerPvpHistoryController.mvpScoreOffset), 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE, num4 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL, num5 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL, num6 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL, num7 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_DOUBLEKILL, num8 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST, flag3);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST, flag2);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST, flag);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST, flag5);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST, flag4);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST, flag6);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_WINMVP, bOpen);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LOSEMVP, bOpen2);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (playerKDA.PlayerUid == masterRoleInfo.playerUllUID && playerKDA.WorldId == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID)
			{
				CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND, CPlayerPvpHistoryController.IsPlayWithFriend(playerKDA.PlayerCamp));
			}
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY, playerKDA.bRunaway || playerKDA.bDisconnect || playerKDA.bHangup);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER, playerKDA.IsComputer);
			CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE, bWarmBattle);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.GetGameType() == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER)
			{
				CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND, false);
				TeamInfo teamInfo = Singleton<CMatchingSystem>.instance.teamInfo;
				if (teamInfo != null)
				{
					switch (teamInfo.MemInfoList.Count)
					{
					case 2:
						CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER2, true);
						break;
					case 3:
						CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER3, true);
						break;
					case 4:
						CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER4, true);
						break;
					case 5:
						CPlayerPvpHistoryController.SetFightAchive(ref playerData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER5V5, true);
						break;
					}
				}
			}
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerKDA.PlayerId).Captain;
			if (captain && captain.handle.ValueComponent != null)
			{
				playerData.dwTotalCoin = (uint)captain.handle.ValueComponent.GetGoldCoinIncomeInBattle();
			}
		}

		public static bool IsPlayWithFriend(COM_PLAYERCAMP playerCmp)
		{
			bool result = false;
			List<Player> allCampPlayers = Singleton<GamePlayerCenter>.instance.GetAllCampPlayers(playerCmp);
			int count = allCampPlayers.get_Count();
			for (int i = 0; i < count; i++)
			{
				Player player = allCampPlayers.get_Item(i);
				if (!player.Computer && (Singleton<CFriendContoller>.instance.model.IsGameFriend(player.PlayerUId, (uint)player.LogicWrold) || Singleton<CFriendContoller>.instance.model.IsSnsFriend(player.PlayerUId, (uint)player.LogicWrold)))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private COMDT_PLAYER_FIGHT_DATA GetPlayerFightData(ulong uId, int iLogicWorldId, ref COMDT_PLAYER_FIGHT_RECORD record)
		{
			if (record == null)
			{
				return null;
			}
			for (int i = 0; i < (int)record.bPlayerCnt; i++)
			{
				COMDT_PLAYER_FIGHT_DATA cOMDT_PLAYER_FIGHT_DATA = record.astPlayerFightData[i];
				if ((cOMDT_PLAYER_FIGHT_DATA.ullPlayerUid == uId || cOMDT_PLAYER_FIGHT_DATA.ullPlayerUid == 0uL) && cOMDT_PLAYER_FIGHT_DATA.iPlayerLogicWorldID == iLogicWorldId)
				{
					return cOMDT_PLAYER_FIGHT_DATA;
				}
			}
			return null;
		}

		private bool IsFightAchiveSet(uint achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT achiveBit)
		{
			return ((ulong)achiveBits & 1uL << (int)(achiveBit & (COM_FIGHT_HISTORY_ACHIVE_BIT)31)) > 0uL;
		}

		private bool CanShowPlayerWithFriend(uint achiveBits)
		{
			return !this.IsFightAchiveSet(achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER5V5) && !this.IsFightAchiveSet(achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER4) && !this.IsFightAchiveSet(achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER3) && !this.IsFightAchiveSet(achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER2) && this.IsFightAchiveSet(achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND);
		}

		public static void SetFightAchive(ref uint achiveBits, COM_FIGHT_HISTORY_ACHIVE_BIT achiveBit, bool bOpen)
		{
			if (bOpen)
			{
				achiveBits |= 1u << (int)achiveBit;
			}
			else
			{
				achiveBits &= ~(1u << (int)achiveBit);
			}
		}

		private ListView<COMDT_PLAYER_FIGHT_DATA> GetCampPlayerDataList(COM_PLAYERCAMP playerCamp, ref COMDT_PLAYER_FIGHT_RECORD record)
		{
			ListView<COMDT_PLAYER_FIGHT_DATA> listView = new ListView<COMDT_PLAYER_FIGHT_DATA>();
			for (int i = 0; i < (int)record.bPlayerCnt; i++)
			{
				if (record.astPlayerFightData[i].bPlayerCamp == (byte)playerCamp)
				{
					listView.Add(record.astPlayerFightData[i]);
				}
			}
			return listView;
		}

		private static int ComparisonHistoryData(COMDT_PLAYER_FIGHT_RECORD a, COMDT_PLAYER_FIGHT_RECORD b)
		{
			if (a == null && b == null)
			{
				return 0;
			}
			if (a != null && b == null)
			{
				return -1;
			}
			if (a == null && b != null)
			{
				return 1;
			}
			if (a.dwGameStartTime > b.dwGameStartTime)
			{
				return -1;
			}
			if (a.dwGameStartTime < b.dwGameStartTime)
			{
				return 1;
			}
			return 0;
		}

		public void ClearHostData()
		{
			if (this.m_hostRecordList != null)
			{
				this.m_hostRecordList.Clear();
				this.m_hostRecordList = null;
			}
		}

		private void ReqHistoryInfo(ulong uid, uint logicId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1441u);
			cSPkg.stPkgData.stFightHistoryListReq.ullUid = uid;
			cSPkg.stPkgData.stFightHistoryListReq.dwLogicWorldID = logicId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public void CommitHistoryInfo(bool hostPlayerWin, COM_GAMEPK_TYPE isPkAI)
		{
			if (this.m_bBattleState)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1440u);
					this.CreatHistoryInfoRePortData(hostPlayerWin, ref cSPkg.stPkgData.stFightHistoryReq.stFightRecord);
					if (isPkAI == COM_GAMEPK_TYPE.COM_GAMEPK_WITH_AI || curLvelContext.m_isWarmBattle)
					{
						Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
					}
				}
				this.m_bBattleState = false;
			}
		}

		public void StartBattle()
		{
			this.m_bBattleState = true;
		}

		[MessageHandler(1442)]
		public static void ReciveHistoryInfo(CSPkg msg)
		{
			if (msg.stPkgData.stFightHistoryListRsp.bErrorCode == 0)
			{
				CSDT_FIGHTHISTORY_RECORD_DETAIL_SUCC stSucc = msg.stPkgData.stFightHistoryListRsp.stRecordDetail.stSucc;
				COMDT_PLAYER_FIGHT_RECORD[] astRecord = msg.stPkgData.stFightHistoryListRsp.stRecordDetail.stSucc.stRecordList.astRecord;
				Singleton<CPlayerPvpHistoryController>.GetInstance().m_ullUid = stSucc.ullUid;
				Singleton<CPlayerPvpHistoryController>.GetInstance().m_dwLogicWorldID = stSucc.dwLogicWorldID;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo.playerUllUID == stSucc.ullUid && (long)MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID == (long)((ulong)stSucc.dwLogicWorldID))
				{
					if (Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList == null)
					{
						Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList = new ListView<COMDT_PLAYER_FIGHT_RECORD>();
					}
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList.Clear();
					for (int i = 0; i < astRecord.Length; i++)
					{
						if (astRecord[i].bGameType == 0)
						{
							break;
						}
						Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList.Add(astRecord[i]);
					}
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Clear();
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.AddRange(Singleton<CPlayerPvpHistoryController>.GetInstance().m_hostRecordList);
				}
				else
				{
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Clear();
					for (int j = 0; j < astRecord.Length; j++)
					{
						if (astRecord[j].bGameType == 0)
						{
							break;
						}
						Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Add(astRecord[j]);
					}
					Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Sort(new Comparison<COMDT_PLAYER_FIGHT_RECORD>(CPlayerPvpHistoryController.ComparisonHistoryData));
				}
			}
			else if (msg.stPkgData.stFightHistoryListRsp.bErrorCode == 173)
			{
				Singleton<CPlayerPvpHistoryController>.GetInstance().m_recordList.Clear();
			}
			Singleton<CPlayerPvpHistoryController>.instance.OnReciveFightData();
			Singleton<CUIManager>.instance.CloseSendMsgAlert();
		}
	}
}

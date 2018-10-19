using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CGuildListView : Singleton<CGuildListView>
	{
		public enum Tab
		{
			Guild,
			PrepareGuild,
			CreateGuild,
			None
		}

		public enum enGuildListFormWidget
		{
			GuildCreatePanel_GuildNameText,
			GuildCreatePanel_GuildBulletinText,
			GuildCreatePanel_CostCoinText,
			GuildCreatePanel_CostDianQuanText,
			GuildCreatePanel_GuildIconImage,
			GuildListPanel,
			GuildPrepareListPanel,
			GuildCreatePanel,
			GuildCreatePanel_GuildIconIdText,
			GuildListPanel_SearchGuildInput,
			GuildListPanel_GuildBulletinText,
			GuildListPanel_GuildChairmanNameText,
			GuildPrepareListPanel_GuildListPanel,
			GuildPrepareListPanel_MemberListPanel,
			GuildPrepareListPanel_CreatorNameText,
			GuildPrepareListPanel_CreatorLevelText,
			GuildPrepareListPanel_CreatorBattleText,
			GuildPrepareListPanel_GuildIconImage,
			GuildPrepareListPanel_GuildNameText,
			GuildPrepareListPanel_GuildMemberCountText,
			GuildPrepareListPanel_GuildTimeoutTimer,
			GuildPrepareListPanel_BulletinText,
			GuildPrepareListPanel_SearchGuildInput,
			GuildPrepareListPanel_GuildCreatorPanel,
			GuildPrepareListPanel_GuildInfoPanel,
			GuildPrepareListPanel_GuildBulletinPanel,
			GuildPrepareListPanel_GuildOperationPanel,
			GuildPrepareListPanel_GuildList,
			GuildPrepareListPanel_GuildMemberList,
			GuildCreatePanel_IsOnlyFriendSliderHandleText,
			GuildListPanel_GuildList,
			GuildListPanel_GuildOperationPanel,
			GuildCreatePanel_IsOnlyFriendSlider,
			PanelTab,
			GuildListPanel_ChairmanHeadIconImage,
			GuildPrepareListPanel_CreatorHeadIconImage,
			GuildListPanel_GuildChairmanPanel,
			GuildListPanel_GuildBulletinPanel,
			GuildPrepareListPanel_PageIdDataText,
			GuildListPanel_GuildChairmanLevelText,
			GuildListPanel_ChairmanNobeBgImage,
			GuildListPanel_ChairmanNobeIconImage,
			GuildPrepareListPanel_MemberNobeBgImage,
			GuildPrepareListPanel_MemberNobeIconImage,
			GuildPrepareListPanel_CreatorNobeBgImage,
			GuildPrepareListPanel_CreatorNobeIconImage,
			GuildListPanel_GuildGradeIconImage,
			GuildListPanel_GuildGradeContentText,
			GuildListPanel_AwardProfitContentText
		}

		public enum enGuildIconFormWidget
		{
			Icon_List
		}

		public const string GuildListFormPrefabPath = "UGUI/Form/System/Guild/Form_Guild_List.prefab";

		public const string IconFormPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Icon.prefab";

		private const int GuildRuleTextIndex = 8;

		private CUIFormScript m_form;

		private CGuildListView.Tab m_curTab = CGuildListView.Tab.None;

		private GameObject m_curPanelGo;

		private CGuildModel m_Model;

		public CGuildListView.Tab CurTab
		{
			get
			{
				return this.m_curTab;
			}
			set
			{
				this.m_curTab = value;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CGuildListView.Tab>("Guild_List_Tab_Change", this.m_curTab);
			}
		}

		public CGuildListView()
		{
			this.m_Model = Singleton<CGuildModel>.GetInstance();
			this.Init();
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_List_View_Change_Tab, new CUIEventManager.OnUIEventHandler(this.On_Tab_Change));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Select_In_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Select));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Prepare_Guild_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Prepare_Guild_Select));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Join, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Join));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recruit_Apply_Join, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recruit_Apply_Join));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join_Cancel, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join_Cancel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Guild_List_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search_In_Guild_List_Panel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Prepare_Guild_List_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search_In_Prepare_Guild_List_Panel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Help, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Help));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Cancel, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Cancel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Modify_Icon, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Modify_Icon));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Icon_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Icon_Selected));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Timeout, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Timeout));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Only_Friend_Slider_Value_Changed, new CUIEventManager.OnUIEventHandler(this.On_Guild_Only_Friend_Slider_Value_Changed));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Request_More_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Request_More_Guild_List));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Prepare_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Prepare_Guild_List_Element_Enabled));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Requesst_More_Prepare_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Requesst_More_Prepare_Guild_List));
		}

		public override void UnInit()
		{
		}

		public bool IsShow()
		{
			return Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_List.prefab") != null;
		}

		private void On_Tab_Change(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component != null)
			{
				int selectedIndex = component.GetSelectedIndex();
				this.CurTab = (CGuildListView.Tab)selectedIndex;
				this.InitPanel();
			}
		}

		private void On_Guild_Guild_Select(CUIEvent uiEvent)
		{
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(selectedIndex);
			if (guildInfoByIndex != null)
			{
				CUIHttpImageScript component = this.m_form.GetWidget(34).GetComponent<CUIHttpImageScript>();
				component.SetImageUrl(CGuildHelper.GetHeadUrl(guildInfoByIndex.chairman.stBriefInfo.szHeadUrl));
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(this.m_form.GetWidget(41).GetComponent<Image>(), CGuildHelper.GetNobeLevel(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.level), false, true, 0uL);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(this.m_form.GetWidget(40).GetComponent<Image>(), CGuildHelper.GetNobeHeadIconId(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.headIconId));
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(this.m_form.GetWidget(40).GetComponent<Image>(), CGuildHelper.GetNobeHeadIconId(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.headIconId), this.m_form, 1f, false);
				this.m_form.GetWidget(10).GetComponent<Text>().text = guildInfoByIndex.briefInfo.sBulletin;
				this.m_form.GetWidget(11).GetComponent<Text>().text = guildInfoByIndex.chairman.stBriefInfo.sName;
				this.m_form.GetWidget(39).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", new string[]
				{
					guildInfoByIndex.chairman.stBriefInfo.dwLevel.ToString()
				});
				Image component2 = this.m_form.GetWidget(46).GetComponent<Image>();
				Text component3 = this.m_form.GetWidget(47).GetComponent<Text>();
				Text component4 = this.m_form.GetWidget(48).GetComponent<Text>();
				component2.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(guildInfoByIndex.RankInfo.totalRankPoint), this.m_form, true, false, false, false);
				component3.text = CGuildHelper.GetGradeName(guildInfoByIndex.RankInfo.totalRankPoint);
				component4.text = Singleton<CTextManager>.GetInstance().GetText("Guild_Profit_Desc", new string[]
				{
					CGuildHelper.GetCoinProfitPercentage((int)guildInfoByIndex.briefInfo.bLevel).ToString()
				});
			}
		}

		private void On_Guild_Guild_Join(CUIEvent uiEvent)
		{
			int selectedIndex = this.m_form.GetWidget(30).GetComponent<CUIListScript>().GetSelectedIndex();
			GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(selectedIndex);
			if (guildInfoByIndex == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			stGuildBriefInfo stBriefInfo = this.m_Model.GetAppliedGuildInfoByUid(guildInfoByIndex.briefInfo.uulUid).stBriefInfo;
			if (stBriefInfo.uulUid != 0uL)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Current_Guild_Has_Invited_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			if (CGuildHelper.IsInLastQuitGuildCd())
			{
				return;
			}
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum > 0)
			{
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(8u).dwConfValue;
				if ((long)(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum + 1) > (long)((ulong)dwConfValue))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Today_Apply_Reach_Limit_Tip", true, 1.5f, null, new object[0]);
					return;
				}
			}
			if (this.m_Model.IsInGuildStep())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_In_Guild_Step_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent<GuildInfo>("Request_Apply_Guild_Join", guildInfoByIndex);
		}

		private void On_Guild_Recruit_Apply_Join(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			int tag = uiEvent.m_eventParams.tag;
			stGuildBriefInfo stBriefInfo = this.m_Model.GetAppliedGuildInfoByUid(commonUInt64Param).stBriefInfo;
			if (stBriefInfo.uulUid != 0uL)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Current_Guild_Has_Invited_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			if (CGuildHelper.IsInLastQuitGuildCd())
			{
				return;
			}
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum > 0)
			{
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(8u).dwConfValue;
				if ((long)(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum + 1) > (long)((ulong)dwConfValue))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Today_Apply_Reach_Limit_Tip", true, 1.5f, null, new object[0]);
					return;
				}
			}
			Singleton<CGuildListController>.GetInstance().RequestApplyJoinGuild(commonUInt64Param, tag, true);
		}

		private void On_Guild_Prepare_Guild_Select(CUIEvent uiEvent)
		{
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(selectedIndex);
			DebugHelper.Assert(prepareGuildInfoByIndex != null);
			if (prepareGuildInfoByIndex == null)
			{
				return;
			}
			CUIHttpImageScript component = this.m_form.GetWidget(35).GetComponent<CUIHttpImageScript>();
			component.SetImageUrl(CGuildHelper.GetHeadUrl(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.szHeadUrl));
			Image component2 = this.m_form.GetWidget(45).GetComponent<Image>();
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.uulUid, prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.stVip.level), false, true, 0uL);
			Image component3 = this.m_form.GetWidget(44).GetComponent<Image>();
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.uulUid, prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.stVip.headIconId));
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component3, CGuildHelper.GetNobeHeadIconId(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.uulUid, prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.stVip.headIconId), this.m_form, 1f, false);
			Text component4 = this.m_form.GetWidget(14).GetComponent<Text>();
			component4.text = prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.sName;
			Text component5 = this.m_form.GetWidget(15).GetComponent<Text>();
			component5.text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", new string[]
			{
				prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.dwLevel.ToString()
			});
			Text component6 = this.m_form.GetWidget(16).GetComponent<Text>();
			component6.text = prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.dwGameEntity.ToString();
			Text component7 = this.m_form.GetWidget(21).GetComponent<Text>();
			component7.text = prepareGuildInfoByIndex.stBriefInfo.sBulletin;
		}

		private void On_Guild_PrepareGuild_Join(CUIEvent uiEvent)
		{
			CUIListScript component = this.m_form.GetWidget(27).GetComponent<CUIListScript>();
			PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(component.GetSelectedIndex());
			if (prepareGuildInfoByIndex != null && prepareGuildInfoByIndex.stBriefInfo.IsOnlyFriend && Singleton<CFriendContoller>.GetInstance().model.getFriendByName(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.sName, CFriendModel.FriendType.GameFriend) == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Only_Friend_Can_Join_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			uint guildMemberMinPvpLevel = CGuildHelper.GetGuildMemberMinPvpLevel();
			if (guildMemberMinPvpLevel != 0u && guildMemberMinPvpLevel > Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Level_Limit", new string[]
				{
					guildMemberMinPvpLevel.ToString()
				}), false, 1.5f, null, new object[0]);
				return;
			}
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime != 0u)
			{
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(6u).dwConfValue;
				int num = (int)((ulong)(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime + dwConfValue) - (ulong)((long)currentUTCTime));
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, num);
				if (num > 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Cannot_Apply_Tip", new string[]
					{
						((int)timeSpan.TotalMinutes).ToString(),
						timeSpan.Seconds.ToString()
					}), false, 1.5f, null, new object[0]);
					return;
				}
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Response_Tip");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_PrepareGuild_Join_Confirm, enUIEventID.Guild_PrepareGuild_Join_Cancel, false);
		}

		private void On_Guild_PrepareGuild_Join_Confirm(CUIEvent uiEvent)
		{
			if (this.m_Model.IsInGuildStep())
			{
				return;
			}
			CUIListScript component = this.m_form.GetWidget(27).GetComponent<CUIListScript>();
			PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(component.GetSelectedIndex());
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("PrepareGuild_Join", prepareGuildInfoByIndex);
		}

		private void On_Guild_PrepareGuild_Join_Cancel(CUIEvent uiEvent)
		{
		}

		private void On_Guild_Guild_Search_In_Guild_List_Panel(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			string text = srcFormScript.GetWidget(9).GetComponent<InputField>().text;
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CGuildSystem>.GetInstance().SearchGuild(0uL, 0, text, 0, false);
			}
		}

		private void On_Guild_Guild_Search_In_Prepare_Guild_List_Panel(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			string text = srcFormScript.GetWidget(22).GetComponent<InputField>().text;
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CGuildSystem>.GetInstance().SearchGuild(0uL, 0, text, 0, true);
			}
		}

		private void On_Guild_Guild_Help(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenInfoForm(8);
		}

		private void On_Guild_PrepareGuild_Create(CUIEvent uiEvent)
		{
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "3";
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "3";
			Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastCreateGuildTime != 0u)
			{
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(3u).dwConfValue;
				if ((long)currentUTCTime < (long)((ulong)(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastCreateGuildTime + dwConfValue)))
				{
					Singleton<CUIManager>.GetInstance().OpenTips("Guild_Last_Create_Time_Tip", true, 1.5f, null, new object[0]);
					return;
				}
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			uint dwConfValue2 = GameDataMgr.guildMiscDatabin.GetDataByKey(5u).dwConfValue;
			if (masterRoleInfo.DianQuan < (ulong)dwConfValue2)
			{
				CUICommonSystem.OpenDianQuanNotEnoughTip();
				return;
			}
			uint guildMemberMinPvpLevel = CGuildHelper.GetGuildMemberMinPvpLevel();
			if (guildMemberMinPvpLevel != 0u && guildMemberMinPvpLevel > Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Level_Limit", new string[]
				{
					guildMemberMinPvpLevel.ToString()
				}), false, 1.5f, null, new object[0]);
				return;
			}
			Text component = this.m_form.GetWidget(0).GetComponent<Text>();
			string text = CUIUtility.RemoveEmoji(component.text).Trim();
			if (string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Empty", true, 1.5f, null, new object[0]);
				return;
			}
			if (!Utility.IsValidText(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Invalid", true, 1.5f, null, new object[0]);
				return;
			}
			Text component2 = this.m_form.GetWidget(1).GetComponent<Text>();
			string value = CUIUtility.RemoveEmoji(component2.text).Trim();
			if (string.IsNullOrEmpty(value))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Bulletin_Empty", true, 1.5f, null, new object[0]);
				return;
			}
			uint dwConfValue3 = GameDataMgr.guildMiscDatabin.GetDataByKey(2u).dwConfValue;
			uint dwConfValue4 = GameDataMgr.guildMiscDatabin.GetDataByKey(1u).dwConfValue;
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, (int)dwConfValue3);
			string text2 = Singleton<CTextManager>.GetInstance().GetText("Guild_Create_Tip", new string[]
			{
				timeSpan.TotalHours.ToString(),
				dwConfValue4.ToString()
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text2, enUIEventID.Guild_PrepareGuild_Create_Confirm, enUIEventID.Guild_PrepareGuild_Create_Cancel, false);
		}

		private void On_Guild_PrepareGuild_Create_Confirm(CUIEvent uiEvent)
		{
			if (this.m_Model.IsInGuildStep())
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Guild_In_Guild_Step_Tip_2", true, 1.5f, null, new object[0]);
			}
			else
			{
				Text component = this.m_form.GetWidget(0).GetComponent<Text>();
				Text component2 = this.m_form.GetWidget(1).GetComponent<Text>();
				Text component3 = this.m_form.GetWidget(8).GetComponent<Text>();
				stPrepareGuildCreateInfo arg = default(stPrepareGuildCreateInfo);
				arg.sName = component.text.Trim();
				arg.sBulletin = component2.text.Trim();
				arg.dwHeadId = Convert.ToUInt32(component3.text);
				arg.isOnlyFriend = false;
				Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
				Singleton<EventRouter>.GetInstance().BroadCastEvent<stPrepareGuildCreateInfo>("PrepareGuild_Create", arg);
			}
		}

		private void On_Guild_PrepareGuild_Create_Cancel(CUIEvent uiEvent)
		{
		}

		private void On_Guild_PrepareGuild_Create_Modify_Icon(CUIEvent uiEvent)
		{
			Singleton<CGuildInfoView>.GetInstance().OpenGuildIconForm();
		}

		private void On_Guild_PrepareGuild_Create_Icon_Selected(CUIEvent uiEvent)
		{
			CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
			if (cUIListScript == null)
			{
				return;
			}
			int num = cUIListScript.GetSelectedIndex();
			if (num == -1)
			{
				num = 0;
			}
			CUIListElementScript elemenet = cUIListScript.GetElemenet(num);
			if (elemenet == null)
			{
				return;
			}
			Text component = elemenet.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>();
			Text component2 = this.m_form.GetWidget(8).GetComponent<Text>();
			component2.text = component.text;
			Image component3 = this.m_form.GetWidget(4).GetComponent<Image>();
			component3.SetSprite(elemenet.transform.Find("imgIcon").GetComponent<Image>());
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript != null)
			{
				srcFormScript.Close();
			}
		}

		private void On_Guild_PrepareGuild_Timeout(CUIEvent uiEvent)
		{
			COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
			if (guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE && guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", 0);
			}
			else
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent("Request_PrepareGuild_Info");
			}
		}

		private void On_Guild_Only_Friend_Slider_Value_Changed(CUIEvent uiEvent)
		{
			int num = (int)uiEvent.m_eventParams.sliderValue;
			Text component = this.m_form.GetWidget(29).GetComponent<Text>();
			component.text = ((num != 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_No") : Singleton<CTextManager>.GetInstance().GetText("Common_Yes"));
		}

		private void On_Guild_Guild_List_Element_Enabled(CUIEvent uiEvent)
		{
			GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
			if (guildInfoByIndex != null)
			{
				this.SetGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, guildInfoByIndex);
			}
		}

		private void On_Guild_Prepare_Guild_List_Element_Enabled(CUIEvent uiEvent)
		{
			PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
			if (prepareGuildInfoByIndex != null)
			{
				this.SetPrepareGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, prepareGuildInfoByIndex);
			}
		}

		private void On_Guild_Request_More_Guild_List(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = srcFormScript.GetWidget(30).GetComponent<CUIListScript>();
			int elementAmount = component.GetElementAmount();
			if (elementAmount > 0)
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", elementAmount + 1, 20);
			}
		}

		private void On_Guild_Requesst_More_Prepare_Guild_List(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			CUIListScript component = srcFormScript.GetWidget(27).GetComponent<CUIListScript>();
			int elementAmount = component.GetElementAmount();
			if (elementAmount > 0)
			{
				Text component2 = srcFormScript.GetWidget(38).GetComponent<Text>();
				int arg = int.Parse(component2.text) + 1;
				Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", arg);
			}
		}

		private void SetGuildIcon(CUIListElementScript listElementScript, ResGuildIcon info)
		{
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.dwIcon;
			Image component = listElementScript.transform.Find("imgIcon").GetComponent<Image>();
			component.SetSprite(prefabPath, listElementScript.m_belongedFormScript, true, false, false, false);
			Text component2 = listElementScript.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>();
			component2.text = info.dwIcon.ToString();
		}

		public void CloseForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_List.prefab");
		}

		public void OpenForm(CGuildListView.Tab selectTab = CGuildListView.Tab.None, bool isDispatchTabChangeEvent = true)
		{
			if (this.IsShow())
			{
				return;
			}
			this.m_form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_List.prefab", false, true);
			this.InitTabList();
			this.SelectTabElement(selectTab, isDispatchTabChangeEvent);
		}

		public void InitTabList()
		{
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Guild"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Prepare_Guild"),
				Singleton<CTextManager>.GetInstance().GetText("Guild_Create_Guild")
			};
			GameObject widget = this.m_form.GetWidget(33);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SetElementAmount(array.Length);
			for (int i = 0; i < component.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
		}

		public void SelectTabElement(CGuildListView.Tab defaultTab = CGuildListView.Tab.None, bool isDisableTabChangeEvent = true)
		{
			if (defaultTab == CGuildListView.Tab.None)
			{
				COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
				if (guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE && guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN)
				{
					this.CurTab = CGuildListView.Tab.Guild;
				}
				else
				{
					this.CurTab = CGuildListView.Tab.PrepareGuild;
				}
			}
			else
			{
				this.CurTab = defaultTab;
			}
			GameObject widget = this.m_form.GetWidget(33);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			component.SelectElement((int)this.CurTab, isDisableTabChangeEvent);
		}

		private void InitPanel()
		{
			this.m_form.GetWidget(5).gameObject.CustomSetActive(false);
			this.m_form.GetWidget(6).gameObject.CustomSetActive(false);
			this.m_form.GetWidget(7).gameObject.CustomSetActive(false);
			switch (this.CurTab)
			{
			case CGuildListView.Tab.Guild:
				Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", 1, 20);
				break;
			case CGuildListView.Tab.PrepareGuild:
			{
				COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
				if (guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE && guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN)
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", 0);
				}
				else
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent("Request_PrepareGuild_Info");
				}
				break;
			}
			case CGuildListView.Tab.CreateGuild:
				this.RefreshCreateGuildPanel();
				break;
			}
		}

		public void RefreshCreateGuildPanel()
		{
			if (!this.IsShow())
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(7);
			widget.CustomSetActive(true);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			Text component = this.m_form.GetWidget(3).GetComponent<Text>();
			uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(5u).dwConfValue;
			if (masterRoleInfo.DianQuan < (ulong)dwConfValue)
			{
				component.text = "<color=#a52a2aff>" + dwConfValue.ToString() + "</color>/" + masterRoleInfo.DianQuan.ToString();
			}
			else
			{
				component.text = "<color=#00ff00>" + dwConfValue.ToString() + "</color>/" + masterRoleInfo.DianQuan.ToString();
			}
			Image component2 = this.m_form.GetWidget(4).GetComponent<Image>();
			Text component3 = this.m_form.GetWidget(8).GetComponent<Text>();
			ResGuildIcon dataByIndex = GameDataMgr.guildIconDatabin.GetDataByIndex(3);
			if (dataByIndex != null)
			{
				string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + dataByIndex.dwIcon;
				component2.SetSprite(prefabPath, this.m_form, true, false, false, false);
				component3.text = dataByIndex.dwIcon.ToString();
			}
		}

		public void RefreshGuildListPanel(bool isHideListExtraContent = false)
		{
			if (!this.IsShow())
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(5);
			widget.CustomSetActive(true);
			switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
			{
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL:
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_WAIT_RESULT:
			{
				int guildInfoCount = this.m_Model.GetGuildInfoCount();
				CUIListScript component = this.m_form.GetWidget(30).GetComponent<CUIListScript>();
				component.SetElementAmount(guildInfoCount);
				GameObject widget2 = this.m_form.GetWidget(36);
				GameObject widget3 = this.m_form.GetWidget(37);
				GameObject widget4 = this.m_form.GetWidget(31);
				if (guildInfoCount > 0)
				{
					component.SelectElement(0, true);
					widget2.CustomSetActive(true);
					widget3.CustomSetActive(true);
					widget4.CustomSetActive(true);
				}
				else
				{
					widget2.CustomSetActive(false);
					widget3.CustomSetActive(false);
					widget4.CustomSetActive(false);
				}
				if (isHideListExtraContent)
				{
					component.HideExtraContent();
				}
				break;
			}
			}
		}

		public void RefreshPrepareGuildPanel(bool isForceShowPrepareGuildList = false, byte pageId = 0, bool isHideListExtraContent = false)
		{
			if (!this.IsShow())
			{
				return;
			}
			GameObject widget = this.m_form.GetWidget(6);
			widget.CustomSetActive(true);
			if (isForceShowPrepareGuildList)
			{
				this.RefreshPrepareGuildPanelPrepareGuildList(pageId, isHideListExtraContent);
				return;
			}
			switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
			{
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL:
				this.RefreshPrepareGuildPanelPrepareGuildList(pageId, isHideListExtraContent);
				break;
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
				this.RefreshPrepareGuildPanelMemberList();
				break;
			}
		}

		private void RefreshPrepareGuildPanelPrepareGuildList(byte pageId, bool isHideListExtraContent)
		{
			GameObject widget = this.m_form.GetWidget(12);
			GameObject widget2 = this.m_form.GetWidget(13);
			GameObject widget3 = this.m_form.GetWidget(23);
			GameObject widget4 = this.m_form.GetWidget(25);
			GameObject widget5 = this.m_form.GetWidget(26);
			GameObject widget6 = this.m_form.GetWidget(24);
			widget.CustomSetActive(true);
			widget2.CustomSetActive(false);
			widget6.CustomSetActive(false);
			CUIListScript component = this.m_form.GetWidget(27).GetComponent<CUIListScript>();
			int prepareGuildInfoCount = this.m_Model.GetPrepareGuildInfoCount();
			component.SetElementAmount(prepareGuildInfoCount);
			if (prepareGuildInfoCount > 0)
			{
				component.SelectElement(0, true);
				widget3.CustomSetActive(true);
				widget4.CustomSetActive(true);
				widget5.CustomSetActive(true);
			}
			else
			{
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(false);
				widget5.CustomSetActive(false);
			}
			if (isHideListExtraContent)
			{
				component.HideExtraContent();
			}
			Text component2 = this.m_form.GetWidget(38).GetComponent<Text>();
			component2.text = pageId.ToString();
		}

		private void RefreshPrepareGuildPanelMemberList()
		{
			GameObject widget = this.m_form.GetWidget(12);
			GameObject widget2 = this.m_form.GetWidget(13);
			GameObject widget3 = this.m_form.GetWidget(23);
			GameObject widget4 = this.m_form.GetWidget(25);
			GameObject widget5 = this.m_form.GetWidget(26);
			GameObject widget6 = this.m_form.GetWidget(24);
			widget2.CustomSetActive(true);
			widget3.CustomSetActive(true);
			widget6.CustomSetActive(true);
			widget.CustomSetActive(false);
			widget4.CustomSetActive(false);
			widget5.CustomSetActive(false);
			CUIListScript component = this.m_form.GetWidget(28).GetComponent<CUIListScript>();
			ListView<GuildMemInfo> memList = this.m_Model.CurrentPrepareGuildInfo.m_MemList;
			int num = (int)(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.bMemCnt - 1);
			component.SetElementAmount(num);
			if (num > 0)
			{
				component.SelectElement(0, true);
			}
			int num2 = 0;
			for (int i = 0; i < (int)this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.bMemCnt; i++)
			{
				if (memList[i].stBriefInfo.uulUid != this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid)
				{
					CUIListElementScript elemenet = component.GetElemenet(num2);
					if (elemenet != null)
					{
						this.SetPrepareGuildMemListItem(elemenet, memList[i]);
					}
					num2++;
				}
			}
			CUIHttpImageScript component2 = this.m_form.GetWidget(35).GetComponent<CUIHttpImageScript>();
			Image component3 = this.m_form.GetWidget(45).GetComponent<Image>();
			Image component4 = this.m_form.GetWidget(44).GetComponent<Image>();
			Text component5 = this.m_form.GetWidget(14).GetComponent<Text>();
			Text component6 = this.m_form.GetWidget(15).GetComponent<Text>();
			Text component7 = this.m_form.GetWidget(16).GetComponent<Text>();
			component2.SetImageUrl(CGuildHelper.GetHeadUrl(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl));
			component5.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.sName;
			component6.text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", new string[]
			{
				this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLevel.ToString()
			});
			component7.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwAbility.ToString();
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component3, CGuildHelper.GetNobeLevel(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid, this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component4, CGuildHelper.GetNobeHeadIconId(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid, this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId));
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component4, CGuildHelper.GetNobeHeadIconId(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid, this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId), this.m_form, 1f, false);
			Text component8 = this.m_form.GetWidget(18).GetComponent<Text>();
			Text component9 = this.m_form.GetWidget(19).GetComponent<Text>();
			Image component10 = this.m_form.GetWidget(17).GetComponent<Image>();
			uint num3 = GameDataMgr.guildMiscDatabin.GetDataByKey(1u).dwConfValue;
			num3 = ((num3 >= 0u) ? num3 : 0u);
			CUITimerScript component11 = this.m_form.GetWidget(20).GetComponent<CUITimerScript>();
			uint num4 = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.dwRequestTime + GameDataMgr.guildMiscDatabin.GetDataByKey(2u).dwConfValue;
			int num5 = (int)((ulong)num4 - (ulong)((long)CRoleInfo.GetCurrentUTCTime()));
			if (num5 < 0)
			{
				num5 = 0;
			}
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, num5);
			component11.SetTotalTime((float)timeSpan.TotalSeconds);
			component11.StartTimer();
			component8.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.sName;
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.dwHeadId;
			component10.SetSprite(prefabPath, this.m_form, true, false, false, false);
			component9.text = num.ToString() + "/" + num3.ToString();
		}

		private void SetGuildListItem(CUIListElementScript listElementScript, GuildInfo info)
		{
			Transform transform = listElementScript.transform;
			Image component = transform.Find("imgIcon").GetComponent<Image>();
			Text component2 = transform.Find("txtName").GetComponent<Text>();
			Text component3 = transform.Find("txtMemCnt").GetComponent<Text>();
			GameObject gameObject = transform.Find("imgApplied").gameObject;
			Text component4 = transform.Find("txtChairmanName").GetComponent<Text>();
			Text component5 = transform.Find("txtSeasonRankpoint").GetComponent<Text>();
			Text component6 = transform.Find("txtJoinLimit").GetComponent<Text>();
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.briefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
			component2.text = info.briefInfo.sName;
			component3.text = info.briefInfo.bMemberNum + "/" + CGuildHelper.GetMaxGuildMemberCountByLevel((int)info.briefInfo.bLevel);
			component4.text = info.chairman.stBriefInfo.sName;
			component5.text = info.RankInfo.totalRankPoint.ToString();
			component6.text = CGuildHelper.GetGuildJoinLimitText((int)info.briefInfo.LevelLimit, (int)info.briefInfo.GradeLimit, info.briefInfo.dwSettingMask);
			stGuildBriefInfo stBriefInfo = this.m_Model.GetAppliedGuildInfoByUid(info.briefInfo.uulUid).stBriefInfo;
			if (stBriefInfo.uulUid != 0uL)
			{
				gameObject.CustomSetActive(true);
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		private void SetPrepareGuildListItem(CUIListElementScript listElementScript, PrepareGuildInfo info)
		{
			Transform transform = listElementScript.transform;
			Image component = transform.Find("imgIcon").GetComponent<Image>();
			Text component2 = transform.Find("txtName").GetComponent<Text>();
			Text component3 = transform.Find("txtCreator").GetComponent<Text>();
			Text component4 = transform.Find("txtMemCnt").GetComponent<Text>();
			CUITimerScript component5 = transform.Find("timeoutTimer").GetComponent<CUITimerScript>();
			GameObject gameObject = transform.Find("imgOnlyFriend").gameObject;
			string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.stBriefInfo.dwHeadId;
			component.SetSprite(prefabPath, this.m_form, true, false, false, false);
			component2.text = info.stBriefInfo.sName;
			component3.text = info.stBriefInfo.stCreatePlayer.sName;
			component4.text = (int)(info.stBriefInfo.bMemCnt - 1) + "/" + GameDataMgr.guildMiscDatabin.GetDataByKey(1u).dwConfValue;
			uint num = info.stBriefInfo.dwRequestTime + GameDataMgr.guildMiscDatabin.GetDataByKey(2u).dwConfValue;
			int num2 = (int)((ulong)num - (ulong)((long)CRoleInfo.GetCurrentUTCTime()));
			if (num2 < 0)
			{
				num2 = 0;
			}
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, num2);
			component5.SetTotalTime((float)timeSpan.TotalSeconds);
			component5.StartTimer();
			gameObject.CustomSetActive(info.stBriefInfo.IsOnlyFriend);
		}

		private void SetPrepareGuildMemListItem(CUIListElementScript listElementScript, GuildMemInfo info)
		{
			Transform transform = listElementScript.transform;
			CUIHttpImageScript component = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
			Image component2 = component.transform.Find("NobeIcon").GetComponent<Image>();
			Image component3 = component.transform.Find("NobeImag").GetComponent<Image>();
			Text component4 = transform.Find("txtName").GetComponent<Text>();
			Text component5 = transform.Find("txtLevel").GetComponent<Text>();
			Text component6 = transform.Find("txtBattle").GetComponent<Text>();
			component.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component2, CGuildHelper.GetNobeLevel(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.level), false, true, 0uL);
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component3, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId));
			component4.text = info.stBriefInfo.sName;
			component5.text = info.stBriefInfo.dwLevel.ToString();
			component6.text = info.stBriefInfo.dwAbility.ToString();
		}

		private bool IsSliderOnlyFriendSelected()
		{
			Slider component = this.m_form.GetWidget(32).GetComponent<Slider>();
			return (int)component.value == 0;
		}
	}
}

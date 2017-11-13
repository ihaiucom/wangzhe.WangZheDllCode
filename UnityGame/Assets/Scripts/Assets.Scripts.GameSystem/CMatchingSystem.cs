using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
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
	internal class CMatchingSystem : Singleton<CMatchingSystem>
	{
		public static readonly string[] s_excludeForm = new string[]
		{
			CPlayerInfoSystem.sPlayerInfoFormPath,
			HeadIconSys.s_headImgChgForm,
			"UGUI/Form/Common/Form_NameChange.prefab",
			CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath
		};

		public static string PATH_MATCHING_ENTRY = "UGUI/Form/System/PvP/Form_PvPEntry.prefab";

		public static string PATH_MATCHING_MULTI = "UGUI/Form/System/PvP/Matching/Form_MultiMatching.prefab";

		public static string PATH_MATCHING_INMATCHING = "UGUI/Form/System/PvP/Matching/Form_InMatching.prefab";

		public static string PATH_MATCHING_CONFIRMBOX = "UGUI/Form/System/PvP/Matching/Form_MatchingConfirmBox.prefab";

		public static string PATH_MATCHING_WAITING = "UGUI/Form/System/PvP/Matching/Form_MatchWaiting.prefab";

		public static int s_PVP_RULE_ID = 6;

		public static int s_TRAIN_RULE_ID = 7;

		private bool bInMatching;

		private bool bInMatchingTeam;

		private uint mapId;

		private COM_BATTLE_MAP_TYPE mapType;

		private byte maxTeamNum;

		public int confirmPlayerNum;

		public TeamInfo teamInfo = new TeamInfo();

		public CacheMathingInfo cacheMathingInfo = new CacheMathingInfo();

		private int m_lastReqMathTime;

		public bool IsSelfTeamMaster
		{
			get
			{
				return this.teamInfo.stTeamMaster.ullUid == this.teamInfo.stSelfInfo.ullUid && this.teamInfo.stTeamMaster.iGameEntity == this.teamInfo.stSelfInfo.iGameEntity;
			}
		}

		public int currentMapPlayerNum
		{
			get;
			private set;
		}

		public bool IsInMatching
		{
			get
			{
				return this.bInMatching;
			}
		}

		public bool IsInMatchingTeam
		{
			get
			{
				return this.bInMatchingTeam;
			}
		}

		public static void CloseExcludeForm()
		{
			for (int i = 0; i < CMatchingSystem.s_excludeForm.Length; i++)
			{
				Singleton<CUIManager>.instance.CloseForm(CMatchingSystem.s_excludeForm[i]);
			}
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_OpenEntry, new CUIEventManager.OnUIEventHandler(this.OnMatchingRoom_OpenEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_StartMulti, new CUIEventManager.OnUIEventHandler(this.OnMatching_StartMulti));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_LeaveTeam, new CUIEventManager.OnUIEventHandler(this.OnMatching_LeaveTeam));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ReqLeave, new CUIEventManager.OnUIEventHandler(this.OnMatching_ReqLeave));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnMatching_KickPlayer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ConfirmMatch, new CUIEventManager.OnUIEventHandler(this.OnMatching_ConfirmGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_OpenConfirmBox, new CUIEventManager.OnUIEventHandler(this.OnMatching_OpenConfirmBox));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matchingt_ShowConfirmHead, new CUIEventManager.OnUIEventHandler(this.OnShowConfirmHead));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Waiting, new CUIEventManager.OnUIEventHandler(this.onMatchWatingTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RuleView, new CUIEventManager.OnUIEventHandler(this.OnMatching_RuleView));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ClickFogHelp, new CUIEventManager.OnUIEventHandler(this.OnClickFogHelp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_BtnGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnGroupClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_BtnGroup_ClickClose, new CUIEventManager.OnUIEventHandler(this.OnBtnGroupClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Robot_BtnGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnRobotBtnGroupClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin1v1Confirm, new CUIEventManager.OnUIEventHandler(this.OnMaching_Begin1v1Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin1v1, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin1v1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin3v3Team, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin3v3Multi));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5Team, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5Multi));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MatchingExt_BeginMelee, new CUIEventManager.OnUIEventHandler(this.OnMatching_BeginMeleeMulti));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MatchingExt_BeginEnterTrainMent, new CUIEventManager.OnUIEventHandler(this.OnMatching_BeginEnterTainment));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_EnterTainMentMore, new CUIEventManager.OnUIEventHandler(this.onMatching_ClickEnterTrainMentMore));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5LadderIn2, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5LadderIn2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5LadderIn5, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5LadderIn5));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Robot1V1, new CUIEventManager.OnUIEventHandler(this.OnMatching_Robot1V1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RobotTeamVERSUS, new CUIEventManager.OnUIEventHandler(this.OnMatching_RobotTeamVERSUS));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RobotTeamENTERTAINMENT, new CUIEventManager.OnUIEventHandler(this.OnMatching_RobotTeamENTERTAINMENT));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_1v1, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide1v1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_3v3, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide3v3));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_5v5, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide5v5));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Casting, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideCasting));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Jungle, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideJungle));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Training, new CUIEventManager.OnUIEventHandler(this.OnMatchingTraining));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuidePanel, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuidePanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuideAdvance, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideAdvance));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuideAdvanceConfirm, new CUIEventManager.OnUIEventHandler(this.OpenGuideAdvancePage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_1v1_ChooseHeroType, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide1v1ChooseHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_5v5_ChooseHeroType, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide5v5ChooseHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_1v1_Confirm, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide1v1Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_3v3_Confirm, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide3v3Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_5v5_Confirm, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide5v5Confirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Casting_Confirm, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideCastingConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Jungle_Confirm, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideJungleConfirm));
			Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this.OnInviteErrCodeNtf));
		}

		public void Clear()
		{
			this.bInMatchingTeam = false;
			this.bInMatching = false;
		}

		public TeamMember CreateTeamMemberInfo(COMDT_TEAMMEMBER_INFO info)
		{
			return new TeamMember
			{
				uID = 
				{
					ullUid = info.stMemberDetail.stMemberUniq.ullUid,
					iGameEntity = info.stMemberDetail.stMemberUniq.iGameEntity,
					iLogicWorldId = info.stMemberDetail.iMemberLogicWorldId
				},
				MemberName = StringHelper.UTF8BytesToString(ref info.stMemberDetail.szMemberName),
				dwMemberHeadId = info.stMemberDetail.dwMemberHeadId,
				dwMemberLevel = info.stMemberDetail.dwMemberLevel,
				dwPosOfTeam = info.dwPosOfTeam,
				bGradeOfRank = info.stMemberDetail.bShowGradeOfRank,
				snsHeadUrl = Utility.UTF8Convert(info.stMemberDetail.szMemberHeadUrl)
			};
		}

		public void InitTeamInfo(COMDT_TEAM_INFO teamData)
		{
			this.teamInfo.TeamId = teamData.dwTeamId;
			this.teamInfo.TeamSeq = teamData.dwTeamSeq;
			this.teamInfo.TeamEntity = teamData.iTeamEntity;
			this.teamInfo.TeamFeature = teamData.ullTeamFeature;
			this.teamInfo.stTeamInfo.bGameMode = teamData.stTeamInfo.bGameMode;
			this.teamInfo.stTeamInfo.bPkAI = teamData.stTeamInfo.bPkAI;
			this.teamInfo.stTeamInfo.bMapType = teamData.stTeamInfo.bMapType;
			this.teamInfo.stTeamInfo.dwMapId = teamData.stTeamInfo.dwMapId;
			this.teamInfo.stTeamInfo.bMaxTeamNum = teamData.stTeamInfo.bMaxTeamNum;
			this.teamInfo.stTeamInfo.iGradofRank = (int)teamData.stTeamInfo.bShowGradeOfRank;
			this.mapId = teamData.stTeamInfo.dwMapId;
			this.mapType = (COM_BATTLE_MAP_TYPE)teamData.stTeamInfo.bMapType;
			this.maxTeamNum = teamData.stTeamInfo.bMaxTeamNum;
			this.teamInfo.stSelfInfo.ullUid = teamData.stSelfInfo.ullUid;
			this.teamInfo.stSelfInfo.iGameEntity = teamData.stSelfInfo.iGameEntity;
			this.teamInfo.stTeamMaster.ullUid = teamData.stTeamMaster.ullUid;
			this.teamInfo.stTeamMaster.iGameEntity = teamData.stTeamMaster.iGameEntity;
			this.teamInfo.MemInfoList.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)teamData.stMemInfo.dwMemNum))
			{
				TeamMember item = this.CreateTeamMemberInfo(teamData.stMemInfo.astMemInfo[num]);
				this.teamInfo.MemInfoList.Add(item);
				num++;
			}
		}

		public void EndGame()
		{
			this.teamInfo = new TeamInfo();
		}

		public bool IsOpenMatchingSystem()
		{
			if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPMODE))
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(10u);
				Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
				return false;
			}
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return false;
			}
			if (this.IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return false;
			}
			return true;
		}

		private void OnMatchingRoom_OpenEntry(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
			if (!this.IsOpenMatchingSystem())
			{
				return;
			}
			if (uiEvent.m_eventParams.tag != 0)
			{
				this.OpenPvPEntry((enPvPEntryFormWidget)uiEvent.m_eventParams.tag);
			}
			else
			{
				this.OpenPvPEntry(enPvPEntryFormWidget.None);
			}
		}

		public void OpenPvPEntry(enPvPEntryFormWidget enOpenEntry = enPvPEntryFormWidget.PlayerBattleBtnGroupPanel)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (!cUIFormScript)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_ENTRY, false, true);
				CMatchingView.InitMatchingEntry(cUIFormScript);
				this.OnBtnGroupClose(null);
			}
			if (enOpenEntry != enPvPEntryFormWidget.None)
			{
				CUICommonSystem.SetObjActive(cUIFormScript.GetWidget((int)enOpenEntry), true);
				CUICommonSystem.SetObjActive(cUIFormScript.GetWidget(2), false);
			}
			if (enOpenEntry == enPvPEntryFormWidget.GuideBtnGroup)
			{
				this.ShowAwards(cUIFormScript);
				this.SetGuideEntryEvent(cUIFormScript);
				cUIFormScript.GetWidget(10).CustomSetActive(false);
			}
			if (enOpenEntry == enPvPEntryFormWidget.PlayerBattleBtnGroupPanel)
			{
				GameObject gameObject = cUIFormScript.transform.FindChild("panelGroup2/btnGroup/Button3").gameObject;
				this.EntertainMentAddLock(gameObject);
			}
			else if (enOpenEntry == enPvPEntryFormWidget.ComputerBattleBtnGroupPanel)
			{
				GameObject gameObject2 = cUIFormScript.transform.FindChild("panelGroup3/btnGroup/Button3").gameObject;
				this.EntertainMentAddLock(gameObject2);
			}
			this.ShowBonusImage(cUIFormScript);
		}

		private void OnBtnGroupClick(CUIEvent uiEvent)
		{
			int tag = uiEvent.m_eventParams.tag;
			COM_CLIENT_PLAY_TYPE type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_NULL;
			if (tag == 1)
			{
				type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_MATCH;
			}
			else if (tag == 2)
			{
				type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_ENTERTAINMENT;
			}
			else if (tag == 3)
			{
				type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_PKAI;
			}
			if (!Singleton<SCModuleControl>.instance.GetActiveModule(type))
			{
				Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
				return;
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (null == srcFormScript)
			{
				return;
			}
			GameObject btnObj = null;
			if (tag == 1)
			{
				srcFormScript.GetWidget(3).CustomSetActive(true);
				btnObj = srcFormScript.transform.FindChild("panelGroup2/btnGroup/Button3").gameObject;
			}
			else if (tag == 2)
			{
				srcFormScript.GetWidget(4).CustomSetActive(true);
				btnObj = srcFormScript.transform.FindChild("panelGroup3/btnGroup/Button3").gameObject;
			}
			else if (tag == 3)
			{
				srcFormScript.GetWidget(11).CustomSetActive(true);
			}
			srcFormScript.GetWidget(2).CustomSetActive(false);
			this.EntertainMentAddLock(btnObj);
			this.ShowBonusImage(srcFormScript);
		}

		private void OnBtnGroupClose(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				return;
			}
			form.GetWidget(3).CustomSetActive(false);
			form.GetWidget(4).CustomSetActive(false);
			form.GetWidget(9).CustomSetActive(false);
			form.GetWidget(11).CustomSetActive(false);
			form.GetWidget(10).CustomSetActive(true);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				form.GetWidget(10).CustomSetActive(false);
			}
			form.GetWidget(2).CustomSetActive(true);
			this.ShowBonusImage(form);
			this.HideRobotBtnGroup();
			Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForPvpEntry();
		}

		private void OnRobotBtnGroupClick(CUIEvent uiEvent)
		{
			this.HideRobotBtnGroup();
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				return;
			}
			int tag = uiEvent.m_eventParams.tag;
			if (tag < 0)
			{
				return;
			}
			if (tag == 3 && !uiEvent.m_srcWidget.GetComponent<Button>().get_interactable())
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(25u);
				Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
				return;
			}
			GameObject gameObject = null;
			if (tag == 0)
			{
				gameObject = form.GetWidget(5);
			}
			else if (tag == 1)
			{
				gameObject = form.GetWidget(6);
			}
			else if (tag == 2)
			{
				gameObject = form.GetWidget(7);
			}
			else if (tag == 3)
			{
				gameObject = form.GetWidget(8);
			}
			else if (tag == 4)
			{
				gameObject = form.GetWidget(12);
			}
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
			}
			if (tag == 1)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick3v3AI, new uint[0]);
			}
		}

		private void HideRobotBtnGroup()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form == null)
			{
				return;
			}
			form.GetWidget(5).CustomSetActive(false);
			form.GetWidget(6).CustomSetActive(false);
			form.GetWidget(7).CustomSetActive(false);
			form.GetWidget(8).CustomSetActive(false);
			form.GetWidget(12).CustomSetActive(false);
		}

		public void EntertainMentAddLock(GameObject btnObj)
		{
			if (btnObj == null)
			{
				return;
			}
			Transform transform = btnObj.transform;
			if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
			{
				transform.GetComponent<Button>().set_interactable(false);
				transform.FindChild("Lock").gameObject.CustomSetActive(true);
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(25u);
				transform.FindChild("Lock/Text").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szLockedTip));
			}
			else
			{
				transform.GetComponent<Button>().set_interactable(true);
				transform.FindChild("Lock").gameObject.CustomSetActive(false);
			}
		}

		private void OnMatching_RuleView(CUIEvent uiEvent)
		{
			int txtKey = CMatchingSystem.s_PVP_RULE_ID;
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form != null)
			{
				GameObject gameObject = form.transform.FindChild("panelGroup4").gameObject;
				if (gameObject != null && gameObject.activeSelf)
				{
					txtKey = CMatchingSystem.s_TRAIN_RULE_ID;
				}
			}
			Singleton<CUIManager>.GetInstance().OpenInfoForm(txtKey);
		}

		private void OnClickFogHelp(CUIEvent uiEvent)
		{
			Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(16u, null, false);
		}

		private void OnMatching_Begin1v1(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("PVP1V1_ConfirmTips"), enUIEventID.Matching_Begin1v1Confirm, enUIEventID.None, uiEvent.m_eventParams, false);
		}

		private void OnMaching_Begin1v1Confirm(CUIEvent uiEvent)
		{
			if (!this.CanReqMatch())
			{
				return;
			}
			this.ResetMatchTime();
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 1;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			CMatchingSystem.ReqStartSingleMatching(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS);
		}

		private void OnMatching_Begin3v3Multi(CUIEvent uiEvent)
		{
			if (MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick33Team, new uint[0]))
			{
				return;
			}
			if (!this.CanReqMatch())
			{
				return;
			}
			this.ResetMatchTime();
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 1;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
			Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.SINGLE_MATCH_3V3_ENTER;
		}

		private void OnMatching_Begin5v5Multi(CUIEvent uiEvent)
		{
			if (!this.CanReqMatch())
			{
				return;
			}
			if (MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick55Team, new uint[0]))
			{
				return;
			}
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_TIYAN_5V5_ENTRY_FLAG);
			if (srv2CltGlobalValue == 1u)
			{
				Singleton<CUIManager>.instance.OpenMessageBox(Singleton<CTextManager>.instance.GetText("Tiyanfu_Match_Forbid"), false);
				return;
			}
			uint num = 0u;
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Miwu"), ref num);
			if (uiEvent.m_eventParams.tagUInt == num && uiEvent.m_srcFormScript != null)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = uiEvent.m_eventID;
				cUIEvent.m_eventParams.tagUInt = uiEvent.m_eventParams.tagUInt;
				if (Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(15u, cUIEvent, false))
				{
					return;
				}
			}
			this.ResetMatchTime();
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 1;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
			Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.SINGLE_MATCH_5V5_ENTER;
		}

		private void OnMatching_BeginMeleeMulti(CUIEvent uiEvent)
		{
			if (!uiEvent.m_srcWidget.GetComponent<Button>().get_interactable())
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(25u);
				Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
				return;
			}
			if (!this.CanReqMatch())
			{
				return;
			}
			this.ResetMatchTime();
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 4;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
			Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
		}

		private void OnMatching_BeginEnterTainment(CUIEvent uiEvent)
		{
			if (!uiEvent.m_eventParams.commonBool)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Matching_Tip_6", true, 1.5f, null, new object[0]);
				return;
			}
			if (!this.CanReqMatch())
			{
				return;
			}
			bool flag = false;
			if (uiEvent.m_srcFormScript != null && uiEvent.m_eventParams.tag == 0)
			{
				flag = MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickFireMatch, new uint[0]);
			}
			if (uiEvent.m_eventParams.tag == 1)
			{
				CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
				uIEvent.m_eventID = enUIEventID.MatchingExt_BeginEnterTrainMent;
				uIEvent.m_eventParams.tag = uiEvent.m_eventParams.tag;
				uIEvent.m_eventParams.tagUInt = uiEvent.m_eventParams.tagUInt;
				uIEvent.m_eventParams.commonBool = uiEvent.m_eventParams.commonBool;
				flag = Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(2u, uIEvent, false);
			}
			if (flag)
			{
				return;
			}
			this.ResetMatchTime();
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 4;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
		}

		private void onMatching_ClickEnterTrainMentMore(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenTips("Matching_Tip_5", true, 1.5f, null, new object[0]);
		}

		private void OnMatching_Begin5v5LadderIn2(CUIEvent uiEvent)
		{
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = CLadderSystem.GetRankBattleMapID();
			this.cacheMathingInfo.mapType = 3;
			this.cacheMathingInfo.CanGameAgain = true;
			uint num = this.cacheMathingInfo.mapId;
			CMatchingSystem.ReqCreateTeam(num, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK, CLadderSystem.MultiLadderMaxTeamerNum(), COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
		}

		private void OnMatching_Begin5v5LadderIn5(CUIEvent uiEvent)
		{
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = CLadderSystem.GetRankBattleMapID();
			this.cacheMathingInfo.mapType = 3;
			this.cacheMathingInfo.CanGameAgain = true;
			uint num = this.cacheMathingInfo.mapId;
			CMatchingSystem.ReqCreateTeam(num, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK, 5, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
		}

		private void OnMatching_Robot1V1(CUIEvent uiEvent)
		{
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 1;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			CMatchingSystem.ReqStartSingleMatching(tagUInt, true, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS);
		}

		private void OnMatching_RobotTeamVERSUS(CUIEvent uiEvent)
		{
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 1;
			this.cacheMathingInfo.AILevel = (COM_AI_LEVEL)uiEvent.m_eventParams.tag;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			COM_AI_LEVEL tag = (COM_AI_LEVEL)uiEvent.m_eventParams.tag;
			COM_BATTLE_MAP_TYPE cOM_BATTLE_MAP_TYPE = COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, true, cOM_BATTLE_MAP_TYPE, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), tag, false);
		}

		private void OnMatching_RobotTeamENTERTAINMENT(CUIEvent uiEvent)
		{
			this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
			this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
			this.cacheMathingInfo.mapType = 4;
			this.cacheMathingInfo.AILevel = (COM_AI_LEVEL)uiEvent.m_eventParams.tag;
			this.cacheMathingInfo.CanGameAgain = true;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			COM_AI_LEVEL tag = (COM_AI_LEVEL)uiEvent.m_eventParams.tag;
			COM_BATTLE_MAP_TYPE cOM_BATTLE_MAP_TYPE = COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
			CMatchingSystem.ReqCreateTeam(tagUInt, true, cOM_BATTLE_MAP_TYPE, (int)(pvpMapCommonInfo.bMaxAcntNum / 2), tag, false);
		}

		private bool CanReqMatch()
		{
			return CRoleInfo.GetCurrentUTCTime() - this.m_lastReqMathTime > 1;
		}

		private void ResetMatchTime()
		{
			this.m_lastReqMathTime = CRoleInfo.GetCurrentUTCTime();
		}

		private void OnMatching_StartMulti(CUIEvent uiEvent)
		{
			if (this.mapId > 0u)
			{
				if (this.teamInfo.stTeamInfo.bMapType == 3 && this.teamInfo.stTeamInfo.bMaxTeamNum == 5)
				{
					if (this.teamInfo.MemInfoList.Count != 5)
					{
						DebugHelper.Assert(this.teamInfo.MemInfoList.Count == 5, "房间人数不足5人，不能开始5人匹配！");
					}
					else
					{
						CMatchingSystem.ReqStartMultiMatching();
					}
				}
				else
				{
					CMatchingSystem.ReqStartMultiMatching();
				}
			}
		}

		private static void OpenInMatchingForm(uint preTime)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_INMATCHING, false, true);
			if (cUIFormScript != null)
			{
				Transform textObjTrans = cUIFormScript.transform.Find("Panel/Predict_Time");
				uint num = preTime / 60u;
				uint num2 = preTime - num * 60u;
				uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_WAITTIME_MAX);
				if (preTime <= globeValue)
				{
					CUICommonSystem.SetTextContent(textObjTrans, Singleton<CTextManager>.instance.GetText("Lobby_MatchTime_TitleShow", new string[]
					{
						num.ToString("D2"),
						num2.ToString("D2")
					}));
				}
				else
				{
					CUICommonSystem.SetTextContent(textObjTrans, Singleton<CTextManager>.instance.GetText("Lobby_MatchTime_OverShow"));
				}
			}
		}

		private static void CloseInMatchingForm()
		{
			Singleton<CMatchingSystem>.GetInstance().bInMatching = false;
			Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
			Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_INMATCHING);
		}

		public static void OnPlayerLeaveMatching()
		{
			CMatchingSystem.CloseInMatchingForm();
		}

		private void OnMatching_OpenConfirmBox(CUIEvent uiEvent)
		{
			Utility.VibrateHelper();
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null, "Room Info is NULL!!!");
			if (roomInfo != null)
			{
				this.currentMapPlayerNum = 0;
				if (roomInfo.roomAttrib != null && roomInfo.roomAttrib.bMapType == 6)
				{
					this.currentMapPlayerNum = (int)CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId).bMaxAcntNum;
				}
				else
				{
					this.currentMapPlayerNum = (int)CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)this.mapType, this.mapId).bMaxAcntNum;
				}
				this.confirmPlayerNum = 0;
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX, false, true);
				CMatchingView.InitConfirmBox(cUIFormScript.gameObject, this.currentMapPlayerNum, ref roomInfo, cUIFormScript);
				MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onOpenMatchingConfirmBox, new uint[0]);
			}
		}

		private void OnMatching_ConfirmGame(CUIEvent uiEvent)
		{
			Button component = uiEvent.m_srcWidget.GetComponent<Button>();
			if (component.get_interactable())
			{
				if (CFakePvPHelper.bInFakeConfirm)
				{
					CFakePvPHelper.OnSelfConfirmed(uiEvent.m_srcFormScript.gameObject, this.currentMapPlayerNum);
				}
				else
				{
					CMatchingSystem.SendMatchingConfirm();
				}
			}
		}

		private void OnShowConfirmHead(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetScript != null)
			{
				uiEvent.m_srcWidgetScript.enabled = false;
			}
			if (uiEvent.m_srcWidget != null)
			{
				CUICommonSystem.SetObjActive(uiEvent.m_srcWidget.transform.Find("ready"), true);
				CUICommonSystem.SetObjActive(uiEvent.m_srcWidget.transform.Find("unready"), false);
			}
		}

		public void CloseMatchingConfirm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
		}

		private void OnInviteErrCodeNtf(byte errorCode, string userName)
		{
			if (errorCode == 20)
			{
				CMatchingSystem.CloseInMatchingForm();
			}
		}

		private void OnMatching_LeaveTeam(CUIEvent uiEvent)
		{
			if (this.bInMatchingTeam)
			{
				CMatchingSystem.ReqLeaveTeam();
			}
			else
			{
				DebugHelper.Assert(false, "Not In Matching Team");
			}
		}

		private void OnMatching_ReqLeave(CUIEvent uiEvent)
		{
			if (this.bInMatching)
			{
				CMatchingSystem.ReqLeaveMatching(uiEvent != null);
			}
			else
			{
				DebugHelper.Assert(false, "Not In Matching");
			}
		}

		private void OnMatching_KickPlayer(CUIEvent uiEvent)
		{
			if (this.IsSelfTeamMaster)
			{
				byte b = (byte)uiEvent.m_eventParams.tag;
				for (int i = 0; i < this.teamInfo.MemInfoList.Count; i++)
				{
					if (this.teamInfo.MemInfoList[i].dwPosOfTeam == (uint)b)
					{
						CMatchingSystem.ReqKickPlayer(this.teamInfo.MemInfoList[i].uID);
						break;
					}
				}
			}
			else
			{
				DebugHelper.Assert(false, "Not Team Master!");
			}
		}

		private static void MatchPunishmentWaiting(float time, int punishType)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_WAITING, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget != null)
			{
				Text component = widget.GetComponent<Text>();
				if (component != null)
				{
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("FailToEnterQuque"));
				}
			}
			GameObject widget2 = cUIFormScript.GetWidget(1);
			if (widget2 != null)
			{
				Text component2 = widget2.GetComponent<Text>();
				if (component2 != null)
				{
					switch (punishType)
					{
					case 1:
						component2.set_text(Singleton<CTextManager>.GetInstance().GetText("PunishmentDescribe"));
						break;
					case 2:
						component2.set_text(Singleton<CTextManager>.GetInstance().GetText("HangUpPunishmentDescribe"));
						break;
					case 3:
						component2.set_text(Singleton<CTextManager>.GetInstance().GetText("CreditPunishmentDescribe"));
						break;
					default:
						component2.set_text(Singleton<CTextManager>.GetInstance().GetText("PunishmentDescribe"));
						break;
					}
				}
			}
			GameObject widget3 = cUIFormScript.GetWidget(2);
			if (widget3 != null)
			{
				CUITimerScript component3 = widget3.GetComponent<CUITimerScript>();
				if (component3 != null)
				{
					component3.SetTotalTime(time);
					component3.StartTimer();
				}
			}
		}

		public void onMatchWatingTimeUp(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_WAITING);
		}

		public void OnTeam_ShareFriend_Team(CUIEvent uiEvent)
		{
			CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
			if (instance == null)
			{
				return;
			}
			uint num = instance.mapId;
			int num2 = (int)instance.mapType;
			int num3 = (int)instance.maxTeamNum;
			string text = string.Empty;
			string text2 = string.Empty;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)num2, num);
			text = pvpMapCommonInfo.szName;
			if (num2 == 3)
			{
				if (num3 == CLadderSystem.MultiLadderMaxTeamerNum())
				{
					if (num3 == 2)
					{
						text2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6");
					}
					else
					{
						text2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_8");
					}
				}
				else if (num3 == 5)
				{
					text2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_7");
				}
			}
			else
			{
				text2 = Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num3));
			}
			byte bInviterGradeOfRank = (byte)this.teamInfo.stTeamInfo.iGradofRank;
			byte bTeamGradeOfRank = (byte)this.teamInfo.stTeamInfo.iGradofRank;
			string text3 = Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_Title");
			string text4 = Singleton<CTextManager>.instance.GetText("Share_Room_Info_Desc", new string[]
			{
				text2,
				text
			});
			string roomInfo = MonoSingleton<ShareSys>.GetInstance().PackTeamData(this.teamInfo.stSelfInfo.ullUid, this.teamInfo.TeamId, this.teamInfo.TeamSeq, this.teamInfo.TeamEntity, this.teamInfo.TeamFeature, bInviterGradeOfRank, this.teamInfo.stTeamInfo.bGameMode, this.teamInfo.stTeamInfo.bPkAI, this.teamInfo.stTeamInfo.bMapType, this.teamInfo.stTeamInfo.dwMapId, (byte)this.cacheMathingInfo.AILevel, this.teamInfo.stTeamInfo.bMaxTeamNum, bTeamGradeOfRank);
			Singleton<ApolloHelper>.GetInstance().InviteFriendToRoom(text3, text4, roomInfo);
		}

		private void OnMatchingGuide1v1(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (CBattleGuideManager.CanSelectHeroOnTrainLevelEntry)
			{
				this.HideHeroChooseGrp(form);
				form.GetWidget(13).CustomSetActive(true);
			}
			else
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					LobbyLogic.ReqStartGuideLevel11(true, (uint)masterRoleInfo.acntMobaInfo.iSelectedHeroType);
				}
			}
		}

		private void OnMatchingGuide3v3(CUIEvent uiEvent)
		{
			LobbyLogic.ReqStartGuideLevel33(true);
		}

		private void OnMatchingGuide5v5(CUIEvent uiEvent)
		{
			if (CBattleGuideManager.CanSelectHeroOnTrainLevelEntry)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
				this.HideHeroChooseGrp(form);
				form.GetWidget(14).CustomSetActive(true);
			}
			else
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					LobbyLogic.ReqStartGuideLevel55(true, (uint)masterRoleInfo.acntMobaInfo.iSelectedHeroType);
				}
			}
		}

		private void OnMatchingGuideCasting(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				LobbyLogic.ReqStartGuideLevelCasting(masterRoleInfo.IsGuidedStateSet(83));
			}
		}

		private void OnMatchingGuideJungle(CUIEvent uiEvent)
		{
			LobbyLogic.ReqStartGuideLevelJungle(true);
		}

		private void OnMatchingGuide1v1ChooseHero(CUIEvent uiEvent)
		{
			LobbyLogic.ReqStartGuideLevel11(true, uiEvent.m_eventParams.tagUInt);
		}

		private void OnMatchingGuide5v5ChooseHero(CUIEvent uiEvent)
		{
			LobbyLogic.ReqStartGuideLevel55(true, uiEvent.m_eventParams.tagUInt);
		}

		public static void ReqStartTrainingLevel()
		{
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(120u).dwConfValue;
			LobbyLogic.ReqStartGuideLevelSelHero(true, dwConfValue);
		}

		private void OnMatchingGuide1v1Confirm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo.PvpLevel >= GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_TRAINLEVEL_CONFIRM_LEVEL_LIMIT))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_ConfirmTips1"), enUIEventID.Matching_Guide_1v1, enUIEventID.None, false);
			}
			else
			{
				uiEvent.m_eventID = enUIEventID.Matching_Guide_1v1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}

		private void OnMatchingGuide3v3Confirm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || !masterRoleInfo.IsGuidedStateSet(83))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_3"), false);
				return;
			}
			if (masterRoleInfo.PvpLevel >= GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_TRAINLEVEL_CONFIRM_LEVEL_LIMIT))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_ConfirmTips4"), enUIEventID.Matching_Guide_3v3, enUIEventID.None, false);
			}
			else
			{
				uiEvent.m_eventID = enUIEventID.Matching_Guide_3v3;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}

		private void OnMatchingGuide5v5Confirm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || !masterRoleInfo.IsGuidedStateSet(0))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_1"), false);
				return;
			}
			if (masterRoleInfo.PvpLevel >= GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_TRAINLEVEL_CONFIRM_LEVEL_LIMIT))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_ConfirmTips2"), enUIEventID.Matching_Guide_5v5, enUIEventID.None, false);
			}
			else
			{
				uiEvent.m_eventID = enUIEventID.Matching_Guide_5v5;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}

		private void OnMatchingGuideCastingConfirm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || !masterRoleInfo.IsGuidedStateSet(98))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_2"), false);
				return;
			}
			if (masterRoleInfo.PvpLevel >= GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_TRAINLEVEL_CONFIRM_LEVEL_LIMIT))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_ConfirmTips3"), enUIEventID.Matching_Guide_Casting, enUIEventID.None, false);
			}
			else
			{
				uiEvent.m_eventID = enUIEventID.Matching_Guide_Casting;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}

		private void OnMatchingGuideJungleConfirm(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || !masterRoleInfo.IsGuidedStateSet(85))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_4"), false);
				return;
			}
			if (masterRoleInfo.PvpLevel >= GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_TRAINLEVEL_CONFIRM_LEVEL_LIMIT))
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_ConfirmTips5"), enUIEventID.Matching_Guide_Jungle, enUIEventID.None, false);
			}
			else
			{
				uiEvent.m_eventID = enUIEventID.Matching_Guide_Jungle;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
		}

		private void OnMatchingTraining(CUIEvent uiEvent)
		{
			CMatchingSystem.ReqStartTrainingLevel();
		}

		private void OnMatchingGuidePanel(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CRoomSystem.PATH_CREATE_ROOM);
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (null == form)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
			}
			form = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
			if (form != null)
			{
				form.GetWidget(9).CustomSetActive(true);
				form.GetWidget(10).CustomSetActive(false);
				form.GetWidget(2).CustomSetActive(false);
				form.GetWidget(3).CustomSetActive(false);
				form.GetWidget(4).CustomSetActive(false);
				form.GetWidget(11).CustomSetActive(false);
				this.ShowAwards(form);
				this.ShowBonusImage(form);
				this.SetGuideEntryEvent(form);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					masterRoleInfo.SetNewbieAchieve(17, true, true);
				}
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryTrainLevelEntry, new uint[0]);
		}

		private void OpenGuideAdvancePage(CUIEvent uiEvent)
		{
			string strUrl = "http://pvp.qq.com/ingame/all/video_stage.shtml?partition=" + MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
			CUICommonSystem.OpenUrl(strUrl, true, 0);
		}

		private void OnMatchingGuideAdvance(CUIEvent uiEvent)
		{
			bool flag = GameDataMgr.globalInfoDatabin.GetDataByKey(155u).dwConfValue > 0u;
			if (flag)
			{
				Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Tutorial_Wifi_Alert"), enUIEventID.Matching_GuideAdvanceConfirm, enUIEventID.Matching_GuideAdvanceCancel, false);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips("Common_Not_Open", true, 1.5f, null, new object[0]);
			}
		}

		public void ShowBonusImage(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			GameObject gameObject = form.transform.FindChild("panelGroupBottom/ButtonTrain/ImageBonus").gameObject;
			if (masterRoleInfo != null && masterRoleInfo.IsTrainingLevelFin())
			{
				gameObject.CustomSetActive(false);
			}
			else
			{
				gameObject.CustomSetActive(true);
			}
		}

		private void ShowAwards(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			List<int> list = new List<int>();
			list.Add((int)GameDataMgr.globalInfoDatabin.GetDataByKey(119u).dwConfValue);
			list.Add((int)GameDataMgr.globalInfoDatabin.GetDataByKey(121u).dwConfValue);
			list.Add((int)GameDataMgr.globalInfoDatabin.GetDataByKey(116u).dwConfValue);
			list.Add((int)GameDataMgr.globalInfoDatabin.GetDataByKey(117u).dwConfValue);
			list.Add((int)GameDataMgr.globalInfoDatabin.GetDataByKey(118u).dwConfValue);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			List<int> list2 = new List<int>();
			list2.Add(83);
			list2.Add(84);
			list2.Add(0);
			list2.Add(85);
			list2.Add(98);
			int count = list.get_Count();
			for (int i = 0; i < count; i++)
			{
				int levelId = list.get_Item(i);
				CUseable item = this.QueryLevelAwardItem(levelId);
				string name = string.Format("panelGroup4/itemCell{0}", i + 2);
				string name2 = string.Format("panelGroup4/Complete{0}", i + 2);
				GameObject gameObject = form.transform.FindChild(name).gameObject;
				GameObject gameObject2 = form.transform.FindChild(name2).gameObject;
				bool bFin = masterRoleInfo.IsGuidedStateSet(list2.get_Item(i));
				this.ShowAwardTip(item, gameObject, form, bFin, gameObject2);
				Transform transform = form.GetWidget(9).transform.FindChild(string.Format("btnGroup/Button{0}", i + 2));
				if (this.IsTraningLevelLocked(masterRoleInfo, list2.get_Item(i)))
				{
					transform.gameObject.GetComponent<Image>().set_color(CUIUtility.s_Color_GrayShader);
					transform.FindChild("Lock").gameObject.CustomSetActive(true);
				}
				else
				{
					transform.gameObject.GetComponent<Image>().set_color(CUIUtility.s_Color_White);
					transform.FindChild("Lock").gameObject.CustomSetActive(false);
				}
			}
		}

		private bool IsTraningLevelLocked(CRoleInfo roleInfo, int trainingLevelCompletedBit)
		{
			switch (trainingLevelCompletedBit)
			{
			case 83:
				return !roleInfo.IsGuidedStateSet(98);
			case 84:
				return !roleInfo.IsGuidedStateSet(85);
			case 85:
				return !roleInfo.IsGuidedStateSet(83);
			default:
				return trainingLevelCompletedBit != 0 && trainingLevelCompletedBit == 98 && !roleInfo.IsGuidedStateSet(0);
			}
		}

		private void SetGuideEntryEvent(CUIFormScript form)
		{
			Transform transform = form.GetWidget(13).transform;
			Transform transform2 = form.GetWidget(14).transform;
			if (transform == null || transform2 == null)
			{
				return;
			}
			transform.FindChild("Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
			transform.FindChild("Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
			transform.FindChild("Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
			transform2.FindChild("Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
			transform2.FindChild("Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
			transform2.FindChild("Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
			transform.gameObject.CustomSetActive(false);
			transform2.gameObject.CustomSetActive(false);
		}

		private void HideHeroChooseGrp(CUIFormScript form)
		{
			GameObject widget = form.GetWidget(13);
			GameObject widget2 = form.GetWidget(14);
			widget.CustomSetActive(false);
			widget2.CustomSetActive(false);
		}

		private CUseable QueryLevelAwardItem(int levelId)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)levelId);
			if (dataByKey == null)
			{
				return null;
			}
			uint key = dataByKey.SettleIDDetail[0];
			ResCommonSettle dataByKey2 = GameDataMgr.settleDatabin.GetDataByKey(key);
			if (dataByKey2 == null)
			{
				return null;
			}
			uint dwRewardID = dataByKey2.astFirstCompleteReward[0].dwRewardID;
			ResRandomRewardStore dataByKey3 = GameDataMgr.randomRewardDB.GetDataByKey(dwRewardID);
			if (dataByKey3 == null)
			{
				return null;
			}
			return CUseableManager.GetUseableByRewardInfo(dataByKey3);
		}

		private void ShowAwardTip(CUseable item, GameObject itemCell, CUIFormScript form, bool bFin, GameObject itemComplete)
		{
			if (form == null || itemCell == null)
			{
				return;
			}
			if (item != null)
			{
				if (bFin)
				{
					itemCell.CustomSetActive(false);
					itemComplete.CustomSetActive(true);
				}
				else
				{
					itemCell.CustomSetActive(true);
					CMatchingSystem.SetItemCell(form, itemCell, item);
					itemComplete.CustomSetActive(false);
				}
			}
			else
			{
				itemCell.CustomSetActive(false);
				itemComplete.CustomSetActive(bFin);
			}
		}

		private static void SetItemCell(CUIFormScript formScript, GameObject itemCell, CUseable itemUseable)
		{
			Image component = itemCell.transform.Find("imgIcon").GetComponent<Image>();
			Text component2 = itemCell.transform.Find("lblIconCount").GetComponent<Text>();
			CUIUtility.SetImageSprite(component, itemUseable.GetIconPath(), formScript, true, false, false, false);
			component2.set_text(itemUseable.m_stackCount.ToString());
			CUICommonSystem.AppendMultipleText(component2, itemUseable.m_stackMulti);
			if (itemUseable.m_stackCount <= 0)
			{
				component2.gameObject.CustomSetActive(false);
			}
		}

		public static void ReqStartSingleMatching(uint MapId, bool bPkAI, COM_BATTLE_MAP_TYPE mapType = COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS)
		{
			Singleton<CMatchingSystem>.GetInstance().mapId = MapId;
			Singleton<CMatchingSystem>.GetInstance().mapType = mapType;
			Singleton<CMatchingSystem>.GetInstance().maxTeamNum = 1;
			Singleton<CMatchingSystem>.GetInstance().cacheMathingInfo.AILevel = COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2010u);
			cSPkg.stPkgData.stMatchReq.bMapType = Convert.ToByte(mapType);
			cSPkg.stPkgData.stMatchReq.dwMapId = MapId;
			cSPkg.stPkgData.stMatchReq.bIsPkAI = Convert.ToByte(bPkAI ? COM_GAMEPK_TYPE.COM_GAMEPK_WITH_AI : COM_GAMEPK_TYPE.COM_GAMEPK_WITHOUT_AI);
			cSPkg.stPkgData.stMatchReq.bGameMode = 1;
			cSPkg.stPkgData.stMatchReq.bAILevel = Convert.ToByte(COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			Singleton<WatchController>.GetInstance().Stop();
		}

		public static void ReqStartMultiMatching()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2028u);
			cSPkg.stPkgData.stOperTeamReq.stOper.iOperType = 1;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqCreateTeam(uint MapId, bool bPkAI, COM_BATTLE_MAP_TYPE mapType, int maxTeamNum, COM_AI_LEVEL npcAILevel = COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, bool isInviteFriendImmediately = false)
		{
			CInviteSystem.s_isInviteFriendImmidiately = isInviteFriendImmediately;
			DebugHelper.Assert(MapId != 0u, "MapId Should not be 0!!!");
			if (MapId > 0u)
			{
				Singleton<CMatchingSystem>.GetInstance().cacheMathingInfo.AILevel = npcAILevel;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2022u);
				Singleton<CMatchingSystem>.GetInstance().mapId = MapId;
				Singleton<CMatchingSystem>.GetInstance().mapType = mapType;
				Singleton<CMatchingSystem>.GetInstance().maxTeamNum = (byte)maxTeamNum;
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bGameMode = 1;
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.dwMapId = MapId;
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bMapType = (byte)mapType;
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bPkAI = Convert.ToByte(bPkAI ? COM_GAMEPK_TYPE.COM_GAMEPK_WITH_AI : COM_GAMEPK_TYPE.COM_GAMEPK_WITHOUT_AI);
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bAILevel = Convert.ToByte(npcAILevel);
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bShowGradeOfRank = 0;
				cSPkg.stPkgData.stCreateTeamReq.stBaseInfo.bMaxTeamNum = (byte)maxTeamNum;
				if (mapType == COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK)
				{
					cSPkg.stPkgData.stCreateTeamReq.dwLadderFlag = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_MULTILADDER_FLAG);
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
				Singleton<WatchController>.GetInstance().Stop();
			}
		}

		public static void ReqCreateTeamAndInvite(uint mapId, COM_BATTLE_MAP_TYPE mapType, CInviteSystem.stInviteInfo inviteInfo)
		{
			Singleton<CInviteSystem>.GetInstance().InviteInfo = inviteInfo;
			CMatchingSystem.ReqCreateTeam(mapId, false, mapType, inviteInfo.maxTeamNum, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, true);
		}

		public static void ReqKickPlayer(PlayerUniqueID uid)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2028u);
			cSPkg.stPkgData.stOperTeamReq.stOper.iOperType = 2;
			cSPkg.stPkgData.stOperTeamReq.stOper.stOperDetail.construct(2L);
			cSPkg.stPkgData.stOperTeamReq.stOper.stOperDetail.stKickOutTeamMember.ullUid = uid.ullUid;
			cSPkg.stPkgData.stOperTeamReq.stOper.stOperDetail.stKickOutTeamMember.iGameEntity = uid.iGameEntity;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqLeaveTeam()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2027u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void ReqLeaveMatching(bool bManual)
		{
			if (Singleton<CMatchingSystem>.GetInstance().bInMatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1023u);
				cSPkg.stPkgData.stQuitMultGameReq.bManualQuit = (bManual ? 1 : 0);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		public static void SendMatchingConfirm()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2031u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(2023)]
		public static void OnPlayerJoin(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stJoinTeamRsp.bErrCode == 0)
			{
				CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
				instance.mapId = msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.dwMapId;
				instance.mapType = (COM_BATTLE_MAP_TYPE)msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.bMapType;
				instance.maxTeamNum = msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.bMaxTeamNum;
				instance.bInMatchingTeam = true;
				instance.InitTeamInfo(msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc);
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_MULTI, false, true);
				Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
				Singleton<CInviteSystem>.GetInstance().OpenInviteForm(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM, false);
				CMatchingSystem.SetTeamData(cUIFormScript.gameObject, instance.teamInfo);
				instance.cacheMathingInfo.CanGameAgain = instance.IsSelfTeamMaster;
				if (!instance.IsSelfTeamMaster)
				{
					MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
				}
				if (MonoSingleton<ShareSys>.instance.IsQQGameTeamCreate())
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					string roomStr = MonoSingleton<ShareSys>.instance.PackQQGameTeamData(masterRoleInfo.playerUllUID, instance.teamInfo.TeamId, instance.teamInfo.TeamSeq, instance.teamInfo.TeamEntity, instance.teamInfo.TeamFeature, masterRoleInfo.m_rankGrade, instance.teamInfo.stTeamInfo.bGameMode, instance.teamInfo.stTeamInfo.bPkAI, (byte)instance.cacheMathingInfo.AILevel, (int)instance.teamInfo.stTeamInfo.bMaxTeamNum, instance.teamInfo.stTeamInfo.iGradofRank);
					MonoSingleton<ShareSys>.instance.SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.join, COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH, instance.teamInfo.stTeamInfo.bMapType, instance.teamInfo.stTeamInfo.dwMapId, roomStr, 0u, 0u);
				}
				CMatchingSystem.CloseExcludeForm();
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 17)
			{
				CMatchingSystem.MatchPunishmentWaiting(msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfBePunished.dwLeftSec, (int)msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfBePunished.bType);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 22)
			{
				Singleton<CUIManager>.instance.OpenTips("HuoKenPlayModeNotOpenTip", true, 1.5f, null, new object[0]);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 28)
			{
				uint dwLadderFlag = msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfForbidLadder.dwLadderFlag;
				if (dwLadderFlag != GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_MULTILADDER_FLAG))
				{
					CLadderSystem.SetMuiltLadderFlag(dwLadderFlag);
					Singleton<CLadderSystem>.GetInstance().RefreshLadderEntry();
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_Begin5v5LadderIn2);
				}
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 13)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 29)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_LADDER);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 32)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_ONEVSONE);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 31)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_THREEVSTHREE);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 30)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_FIVEVSFIVE);
			}
			else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 33)
			{
				MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_ENTERTAINMENT);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips(Utility.ProtErrCodeToStr(2023, (int)msg.stPkgData.stJoinTeamRsp.bErrCode), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(2026)]
		public static void OnTeamChange(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			TeamInfo teamInfo = Singleton<CMatchingSystem>.GetInstance().teamInfo;
			bool flag = false;
			if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType == 0)
			{
				TeamMember item = Singleton<CMatchingSystem>.GetInstance().CreateTeamMemberInfo(msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stPlayerAdd.stMemInfo);
				teamInfo.MemInfoList.Add(item);
				flag = true;
			}
			else if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType == 1)
			{
				COMDT_TEAM_MEMBER_UNIQ stLevelMember = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stPlayerLeave.stLevelMember;
				for (int i = 0; i < teamInfo.MemInfoList.Count; i++)
				{
					if (teamInfo.MemInfoList[i].uID.ullUid == stLevelMember.ullUid && teamInfo.MemInfoList[i].uID.iGameEntity == stLevelMember.iGameEntity)
					{
						teamInfo.MemInfoList.RemoveAt(i);
						break;
					}
				}
				flag = true;
			}
			else if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType == 2)
			{
				teamInfo.stTeamMaster.ullUid = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stMasterChg.stNewMaster.ullUid;
				teamInfo.stTeamMaster.iGameEntity = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stMasterChg.stNewMaster.iGameEntity;
				flag = true;
			}
			if (flag)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
				if (form != null)
				{
					CMatchingSystem.SetTeamData(form.gameObject, teamInfo);
				}
			}
		}

		[MessageHandler(2030)]
		public static void OnLeaveTeam(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stAcntLeaveRsp.bResult == 0)
			{
				Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
				Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_MULTI);
				Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
				Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
				MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(2030, (int)msg.stPkgData.stAcntLeaveRsp.bResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(2029)]
		public static void OnSelfBeKicked(CSPkg msg)
		{
			Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
			Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_MULTI);
			Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
			Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
			CChatUT.LeaveRoom();
			Singleton<CChatController>.instance.ShowPanel(false, false);
			Singleton<CUIManager>.GetInstance().OpenTips("PVP_Kick_Tip", true, 1.5f, null, new object[0]);
			MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
		}

		[MessageHandler(2011)]
		public static void OnStartMatching(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stMatchRsp.bResult == 2)
			{
				Singleton<CUIManager>.GetInstance().CloseAllFormExceptLobby(true);
				Singleton<CTopLobbyEntry>.instance.CloseForm();
				Singleton<CMatchingSystem>.GetInstance().bInMatching = true;
				CMatchingSystem.OpenInMatchingForm(msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.dwWaitTime);
				Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0uL, 0u);
				Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0uL, 0u);
				Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
				Singleton<CChatController>.instance.ShowPanel(true, false);
				Singleton<CChatController>.instance.view.UpView(false);
				Singleton<CChatController>.instance.model.sysData.ClearEntryText();
				if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.bReason == 18)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Othercancel"), false, 1.5f, null, new object[0]);
				}
				else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.bReason == 19)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Otherexit"), false, 1.5f, null, new object[0]);
					Singleton<CSoundManager>.GetInstance().PostEvent("UI_matching_lost", null);
				}
				MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.start, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			}
			else if (msg.stPkgData.stMatchRsp.bResult == 1)
			{
				CMatchingSystem.CloseInMatchingForm();
				Singleton<CRoomSystem>.GetInstance().BuildRoomInfo(msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchSucc);
				Singleton<CRoomSystem>.instance.SetRoomType(1);
				Singleton<CUIManager>.GetInstance().CloseAllFormExceptLobby(true);
			}
			else if (msg.stPkgData.stMatchRsp.bResult == 3)
			{
				if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 9)
				{
					if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stForBidLadder.bTeamerNum == 3)
					{
						Singleton<CUIManager>.GetInstance().OpenTips("Ladder_Matching_Err1", true, 1.5f, null, new object[0]);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stForBidLadder.bTeamerNum == 4)
					{
						Singleton<CUIManager>.GetInstance().OpenTips("Ladder_Matching_Err2", true, 1.5f, null, new object[0]);
					}
					else
					{
						Singleton<CUIManager>.GetInstance().OpenTips("禁止多人排位", false, 1.5f, null, new object[0]);
					}
				}
				else
				{
					CMatchingSystem.CloseInMatchingForm();
					if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 1)
					{
						MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 10)
					{
						MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_LADDER);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 11)
					{
						MonoSingleton<IDIPSys>.GetInstance().BanTimeTips(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_ONEVSONE);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 2)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("Union_Battle_Tips1"), false);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 3)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("Union_Battle_Tips4"), false);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 4)
					{
						float time = msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stBePunished.dwLeftSec;
						int bType = (int)msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stBePunished.bType;
						CMatchingSystem.MatchPunishmentWaiting(time, bType);
					}
					else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 5)
					{
						Singleton<CUIManager>.GetInstance().CloseForm(CMatchingSystem.PATH_MATCHING_MULTI);
						Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
						Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
						Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("PVP_Matching_Errpr_1"), false);
					}
					else
					{
						Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching_Errpr", true, 1f, null, new object[]
						{
							Utility.ProtErrCodeToStr(2011, msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode)
						});
					}
				}
			}
		}

		[MessageHandler(2032)]
		public static void OnPlayerConfirmMatching(CSPkg msg)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
			if (form != null)
			{
				Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum++;
				CMatchingView.UpdateConfirmBox(form.gameObject, msg.stPkgData.stRoomConfirmRsp.ullUid);
				RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
				if (roomInfo != null && roomInfo.roomAttrib.bWarmBattle)
				{
					CFakePvPHelper.UpdateConfirmBox(form.gameObject, Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum);
				}
			}
		}

		public static void SetTeamData(GameObject root, TeamInfo data)
		{
			uint dwMapId = data.stTeamInfo.dwMapId;
			int bMapType = (int)data.stTeamInfo.bMapType;
			int num = (int)(data.stTeamInfo.bMaxTeamNum * 2);
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)bMapType, dwMapId);
			root.transform.Find("Panel_Main/MapInfo/txtMapName").gameObject.GetComponent<Text>().set_text(pvpMapCommonInfo.szName);
			if (bMapType == 3)
			{
				if ((int)data.stTeamInfo.bMaxTeamNum == CLadderSystem.MultiLadderMaxTeamerNum())
				{
					if (data.stTeamInfo.bMaxTeamNum == 2)
					{
						root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6"));
					}
					else
					{
						root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_8"));
					}
				}
				else if (data.stTeamInfo.bMaxTeamNum == 5)
				{
					root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_7"));
				}
			}
			else
			{
				root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num / 2)));
			}
			Transform transform = root.transform.Find("Panel_Main/MapInfo/TextRank");
			Transform transform2 = root.transform.FindChild("Panel_Main/MapInfo/TextRank2");
			Transform transform3 = root.transform.Find("Panel_Main/MapInfo/TextRankbg");
			if (transform)
			{
				if (bMapType == 3)
				{
					transform.gameObject.CustomSetActive(true);
					transform3.gameObject.CustomSetActive(true);
					string text = Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit");
					byte rankBigGrade = CLadderSystem.GetRankBigGrade((byte)data.stTeamInfo.iGradofRank);
					int num2 = 0;
					string[] array = new string[3];
					string rankBigGradeName = CLadderView.GetRankBigGradeName(rankBigGrade - 1);
					if (!string.IsNullOrEmpty(rankBigGradeName))
					{
						array[num2++] = rankBigGradeName;
					}
					rankBigGradeName = CLadderView.GetRankBigGradeName(rankBigGrade);
					if (!string.IsNullOrEmpty(rankBigGradeName))
					{
						array[num2++] = rankBigGradeName;
					}
					rankBigGradeName = CLadderView.GetRankBigGradeName(rankBigGrade + 1);
					if (!string.IsNullOrEmpty(rankBigGradeName))
					{
						array[num2++] = rankBigGradeName;
					}
					if (num2 == 2)
					{
						transform.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit_3", array));
					}
					else if (num2 == 3)
					{
						transform.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit_2", array));
					}
				}
				else
				{
					transform.gameObject.CustomSetActive(false);
					transform3.gameObject.CustomSetActive(false);
				}
			}
			if (transform2 && data.stTeamInfo.bMapType == 3)
			{
				transform2.gameObject.CustomSetActive(true);
				switch (data.stTeamInfo.bMaxTeamNum)
				{
				case 3:
					CUICommonSystem.SetTextContent(transform2, Singleton<CTextManager>.GetInstance().GetText("Ladder_Team3_Match_Rule"));
					break;
				case 4:
					CUICommonSystem.SetTextContent(transform2, string.Format("{0}  {1}", Singleton<CTextManager>.GetInstance().GetText("Ladder_Team3_Match_Rule"), Singleton<CTextManager>.GetInstance().GetText("Ladder_Team4_Match_Rule")));
					break;
				case 5:
					CUICommonSystem.SetTextContent(transform2, string.Format("{0}", Singleton<CTextManager>.GetInstance().GetText("Ladder_Team5_Match_Rule")));
					break;
				default:
					transform2.gameObject.CustomSetActive(false);
					break;
				}
			}
			else
			{
				transform2.gameObject.CustomSetActive(false);
			}
			GameObject gameObject = root.transform.Find("Panel_Main/Btn_Matching").gameObject;
			gameObject.CustomSetActive(Singleton<CMatchingSystem>.GetInstance().IsSelfTeamMaster);
			GameObject gameObject2 = root.transform.Find("Panel_Main/Players/Player1").gameObject;
			TeamMember memberInfo = TeamInfo.GetMemberInfo(data, 1);
			CMatchingView.SetPlayerSlotData(gameObject2, memberInfo, num >= 2);
			gameObject2 = root.transform.Find("Panel_Main/Players/Player2").gameObject;
			memberInfo = TeamInfo.GetMemberInfo(data, 2);
			CMatchingView.SetPlayerSlotData(gameObject2, memberInfo, num >= 4);
			gameObject2 = root.transform.Find("Panel_Main/Players/Player3").gameObject;
			memberInfo = TeamInfo.GetMemberInfo(data, 3);
			CMatchingView.SetPlayerSlotData(gameObject2, memberInfo, num >= 6);
			gameObject2 = root.transform.Find("Panel_Main/Players/Player4").gameObject;
			memberInfo = TeamInfo.GetMemberInfo(data, 4);
			CMatchingView.SetPlayerSlotData(gameObject2, memberInfo, num >= 8);
			gameObject2 = root.transform.Find("Panel_Main/Players/Player5").gameObject;
			memberInfo = TeamInfo.GetMemberInfo(data, 5);
			CMatchingView.SetPlayerSlotData(gameObject2, memberInfo, num >= 10);
			Button component = gameObject.GetComponent<Button>();
			if (component)
			{
				if (data.stTeamInfo.bMapType == 3 && data.stTeamInfo.bMaxTeamNum == 5)
				{
					bool flag = data.MemInfoList.Count == 5;
					CUICommonSystem.SetButtonEnable(component, flag, flag, true);
				}
				else
				{
					CUICommonSystem.SetButtonEnable(component, true, true, true);
				}
			}
			GameObject gameObject3 = root.transform.Find("Panel_Main/BPTag").gameObject;
			if (bMapType == 3)
			{
				gameObject3.CustomSetActive(CMatchingSystem.HasBpGradeMember(data));
			}
			else
			{
				gameObject3.CustomSetActive(false);
			}
		}

		public static bool HasBpGradeMember(TeamInfo data)
		{
			if (data == null)
			{
				return false;
			}
			bool result = false;
			CLadderSystem instance = Singleton<CLadderSystem>.GetInstance();
			for (int i = 0; i < 5; i++)
			{
				TeamMember memberInfo = TeamInfo.GetMemberInfo(data, i);
				if (memberInfo != null && instance.IsUseBpMode(memberInfo.bGradeOfRank))
				{
					result = true;
				}
			}
			return result;
		}

		private static uint GetMapIDInner(byte MaxAcntNum)
		{
			return 0u;
		}

		public static uint Get1v1MapId()
		{
			return CMatchingSystem.GetMapIDInner(2);
		}

		public static uint Get2v2MapId()
		{
			return CMatchingSystem.GetMapIDInner(4);
		}

		public static uint Get3v3MapId()
		{
			return CMatchingSystem.GetMapIDInner(6);
		}

		public static uint Get5v5MapId()
		{
			return CMatchingSystem.GetMapIDInner(10);
		}

		public static uint GetCPMap3v3Id()
		{
			uint num = 0u;
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.cpLevelDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResCounterPartLevelInfo resCounterPartLevelInfo = (ResCounterPartLevelInfo)current.get_Value();
				if (resCounterPartLevelInfo.stLevelCommonInfo.bMaxAcntNum == 6 && resCounterPartLevelInfo.bIsSingle == 1)
				{
					num = resCounterPartLevelInfo.dwMapId;
					break;
				}
			}
			DebugHelper.Assert(num > 0u);
			return num;
		}
	}
}

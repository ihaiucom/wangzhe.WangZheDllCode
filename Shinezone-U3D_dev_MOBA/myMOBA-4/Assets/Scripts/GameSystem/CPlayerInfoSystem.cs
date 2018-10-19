using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CPlayerInfoSystem : Singleton<CPlayerInfoSystem>
	{
		public enum DetailPlayerInfoSource
		{
			DefaultOthers,
			Self,
			Guild,
			PvpHistoryDetailInfo
		}

		public enum enPlayerFormWidget
		{
			Tab,
			Base_Info_Tab,
			Pvp_Info_Tab,
			Change_Name_Button,
			CreditScore_Tab,
			License_Info_Tab,
			License_List,
			Common_Hero_info,
			Rule_Btn,
			Body,
			Juhua,
			Update_Sub_Module_Timer,
			Title,
			PersonSign,
			AppointOrCancelMatchLeader,
			ShareScreenShot
		}

		public enum Tab
		{
			Base_Info,
			Pvp_Info,
			Honor_Info,
			Common_Hero,
			PvpHistory_Info,
			PvpCreditScore_Info,
			Mentor_Info,
			Social_Info,
			Relation_Info
		}

		public const ushort PlAYER_INFO_RULE_ID = 3;

		public const ushort CREDIT_RULE_ID = 11;

		private CPlayerInfoSystem.DetailPlayerInfoSource _lastDetailSource;

		private bool _isShowGuildAppointViceChairmanBtn;

		private bool _isShowGuildTransferPositionBtn;

		private bool _isShowGuildFireMemberBtn;

		private bool isShowPlayerInfoDirectly = true;

		private CPlayerInfoSystem.Tab requestTab;

		private CPlayerInfoSystem.Tab m_CurTab;

		public static string sPlayerInfoFormPath = "UGUI/Form/System/Player/Form_Player_Info.prefab";

		private bool m_IsFormOpen;

		private CUIFormScript m_Form;

		private CPlayerProfile m_PlayerProfile = new CPlayerProfile();

		public bool IsForbidShowPvpHistoryDetailInfo
		{
			get;
			set;
		}

		public CPlayerInfoSystem.Tab CurTab
		{
			get
			{
				return this.m_CurTab;
			}
			set
			{
				this.m_CurTab = value;
			}
		}

		public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource sourceType = CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, bool isShowDirectly = true, CPlayerInfoSystem.Tab defaultTab = CPlayerInfoSystem.Tab.Base_Info)
		{
			this._lastDetailSource = sourceType;
			this.requestTab = defaultTab;
			if (this._lastDetailSource == CPlayerInfoSystem.DetailPlayerInfoSource.Self || CPlayerInfoSystem.isSelf(ullUid))
			{
				this.m_PlayerProfile.ConvertRoleInfoData(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo());
				this.OpenForm(defaultTab);
			}
			else if (ullUid > 0uL)
			{
				this.isShowPlayerInfoDirectly = isShowDirectly;
				this.ReqOtherPlayerDetailInfo(ullUid, iLogicWorldId);
			}
		}

		public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, bool isShowGuildAppointViceChairmanBtn, bool isShowGuildTransferPositionBtn, bool isShowGuildFireMemberBtn)
		{
			this._isShowGuildAppointViceChairmanBtn = isShowGuildAppointViceChairmanBtn;
			this._isShowGuildTransferPositionBtn = isShowGuildTransferPositionBtn;
			this._isShowGuildFireMemberBtn = isShowGuildFireMemberBtn;
			this.ShowPlayerDetailInfo(ullUid, iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.Guild, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		[MessageHandler(2607)]
		public static void ResPlyaerDetailInfo(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode == 0)
			{
				if (Singleton<CPlayerInfoSystem>.GetInstance().isShowPlayerInfoDirectly)
				{
					Singleton<CPlayerInfoSystem>.instance.ImpResDetailInfo(msg);
				}
				else
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>(EventID.PlayerInfoSystem_Info_Received, msg);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(2607, 163), false, 1.5f, null, new object[0]);
			}
		}

		private void ReqOtherPlayerDetailInfo(ulong ullUid, int iLogicWorldId)
		{
			if (ullUid <= 0uL)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2606u);
			cSPkg.stPkgData.stGetAcntDetailInfoReq.ullUid = ullUid;
			cSPkg.stPkgData.stGetAcntDetailInfoReq.iLogicWorldId = iLogicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void ReqOtherPlayerIntimacyRelation(ulong ullUid, int iLogicWorldId)
		{
			if (ullUid <= 0uL)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2617u);
			cSPkg.stPkgData.stGetIntimacyRelationReq.ullUid = ullUid;
			cSPkg.stPkgData.stGetIntimacyRelationReq.dwLogicWorldId = (uint)iLogicWorldId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(2618)]
		public static void ResPlyaerIntimacyRelation(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CPlayerInfoSystem>.instance.m_PlayerProfile.cacheIntimacyRelationRsp = msg.stPkgData.stGetIntimacyRelationRsp;
			Singleton<CPlayerInfoSystem>.instance.ImpResIntimacyRelation(Singleton<CPlayerInfoSystem>.instance.m_PlayerProfile.cacheIntimacyRelationRsp);
		}

		private void ImpResDetailInfo(CSPkg msg)
		{
			if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode != 0)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode), false);
				return;
			}
			this.m_PlayerProfile.ConvertServerDetailData(msg.stPkgData.stGetAcntDetailInfoRsp.stAcntDetail.stOfSucc);
			if (this._lastDetailSource == CPlayerInfoSystem.DetailPlayerInfoSource.PvpHistoryDetailInfo)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CPlayerInfoSystem.sPlayerInfoFormPath);
				Singleton<CPlayerInfoSystem>.GetInstance().IsForbidShowPvpHistoryDetailInfo = true;
			}
			this.OpenForm(this.requestTab);
		}

		private void ImpResIntimacyRelation(SCPKG_GET_INTIMACY_RELATION_RSP rsp)
		{
			if (rsp.dwResult != 0u)
			{
				if (rsp.dwResult == 231u)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("CS_ERR_GET_INTIMACY_RELATION_OFTEN", true, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", rsp.dwResult), false);
				}
				return;
			}
			if (this.m_Form == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this.m_Form.gameObject, "pnlBg/pnlBody/pnlRelation");
			uint dwIntimacyNum = rsp.dwIntimacyNum;
			CSDT_INTIMACY_RELATION_INFO[] astIntimacyList = rsp.astIntimacyList;
			GameObject obj = Utility.FindChild(componetInChild.gameObject, "info_node");
			obj.CustomSetActive(dwIntimacyNum == 0u);
			if ((long)componetInChild.m_elementAmount != (long)((ulong)dwIntimacyNum))
			{
				componetInChild.SetElementAmount((int)dwIntimacyNum);
			}
			int num = 0;
			while ((long)num < (long)((ulong)dwIntimacyNum))
			{
				CSDT_INTIMACY_RELATION_INFO cSDT_INTIMACY_RELATION_INFO = astIntimacyList[num];
				if (cSDT_INTIMACY_RELATION_INFO != null)
				{
					CUIListElementScript elemenet = componetInChild.GetElemenet(num);
					Image componetInChild2 = Utility.GetComponetInChild<Image>(elemenet.gameObject, "Image");
					IntimacyRelationViewUT.SetRelationBGImg(componetInChild2, cSDT_INTIMACY_RELATION_INFO.bIntimacyState);
					GameObject gameObject = Utility.FindChild(elemenet.gameObject, "top/ImgRank");
					if (gameObject != null)
					{
						Image component = gameObject.GetComponent<Image>();
						if (component != null)
						{
							string rankSmallIconPath = CLadderView.GetRankSmallIconPath(cSDT_INTIMACY_RELATION_INFO.bGradeOfRank, cSDT_INTIMACY_RELATION_INFO.dwRankClass);
							component.SetSprite(rankSmallIconPath, this.m_Form, true, false, false, false);
						}
						Image componetInChild3 = Utility.GetComponetInChild<Image>(gameObject, "ImgSubRank");
						if (componetInChild3 != null)
						{
							string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(cSDT_INTIMACY_RELATION_INFO.bGradeOfRank, cSDT_INTIMACY_RELATION_INFO.dwRankClass);
							componetInChild3.SetSprite(subRankSmallIconPath, this.m_Form, true, false, false, false);
						}
					}
					CUIHttpImageScript componetInChild4 = Utility.GetComponetInChild<CUIHttpImageScript>(elemenet.gameObject, "top/pnlSnsHead/HttpImage");
					UT.SetHttpImage(componetInChild4, cSDT_INTIMACY_RELATION_INFO.szHeadUrl);
					Image componetInChild5 = Utility.GetComponetInChild<Image>(elemenet.gameObject, "top/pnlSnsHead/HttpImage/NobeImag");
					if (componetInChild5 != null)
					{
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(componetInChild5, (int)cSDT_INTIMACY_RELATION_INFO.stGameVip.dwHeadIconId);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(componetInChild5, (int)cSDT_INTIMACY_RELATION_INFO.stGameVip.dwHeadIconId, this.m_Form, 1f, true);
					}
					GameObject gameObject2 = Utility.FindChild(elemenet.gameObject, "top/pnlSnsHead/NobeIcon");
					if (gameObject2)
					{
						MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(gameObject2.GetComponent<Image>(), (int)cSDT_INTIMACY_RELATION_INFO.stGameVip.dwCurLevel, false, true, 0uL);
					}
					Text componetInChild6 = Utility.GetComponetInChild<Text>(elemenet.gameObject, "top/node/name");
					if (componetInChild6 != null)
					{
						componetInChild6.text = UT.Bytes2String(cSDT_INTIMACY_RELATION_INFO.szUserName);
					}
					GameObject genderImage = Utility.FindChild(elemenet.gameObject, "top/node/gender");
					FriendShower.ShowGender(genderImage, (COM_SNSGENDER)cSDT_INTIMACY_RELATION_INFO.bGender);
					DebugHelper.Assert(IntimacyRelationViewUT.IsRelaState(cSDT_INTIMACY_RELATION_INFO.bIntimacyState));
					string relaIcon = IntimacyRelationViewUT.GetRelaIcon((int)cSDT_INTIMACY_RELATION_INFO.wIntimacyValue, cSDT_INTIMACY_RELATION_INFO.bIntimacyState);
					if (!string.IsNullOrEmpty(relaIcon))
					{
						Image componetInChild7 = Utility.GetComponetInChild<Image>(elemenet.gameObject, "middle/relaIcon");
						if (componetInChild7 != null)
						{
							componetInChild7.gameObject.CustomSetActive(true);
							componetInChild7.SetSprite(relaIcon, this.m_Form, true, false, false, false);
						}
					}
					Text componetInChild8 = Utility.GetComponetInChild<Text>(elemenet.gameObject, "middle/relaIcon/relaTxt");
					if (componetInChild8 != null)
					{
						RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(cSDT_INTIMACY_RELATION_INFO.bIntimacyState);
						if (relaTextCfg != null)
						{
							componetInChild8.text = relaTextCfg.IntimRela_Type;
						}
					}
					int nxtLevelValue = IntimacyRelationViewUT.GetNxtLevelValue((int)cSDT_INTIMACY_RELATION_INFO.wIntimacyValue);
					int curLevelDoorValue = IntimacyRelationViewUT.GetCurLevelDoorValue((int)cSDT_INTIMACY_RELATION_INFO.wIntimacyValue);
					Text componetInChild9 = Utility.GetComponetInChild<Text>(elemenet.gameObject, "middle/nxtLevelTxt");
					componetInChild9.text = string.Format("{0}/{1}", cSDT_INTIMACY_RELATION_INFO.wIntimacyValue, nxtLevelValue);
					Image componetInChild10 = Utility.GetComponetInChild<Image>(elemenet.gameObject, "middle/nxtLevelProgress/curValue");
					if (nxtLevelValue > curLevelDoorValue)
					{
						componetInChild10.fillAmount = (float)cSDT_INTIMACY_RELATION_INFO.wIntimacyValue / (float)nxtLevelValue;
					}
					else
					{
						componetInChild10.fillAmount = 1f;
					}
					Text componetInChild11 = Utility.GetComponetInChild<Text>(elemenet.gameObject, "middle/descText");
					if (componetInChild11 != null)
					{
						componetInChild11.text = UT.Bytes2String(cSDT_INTIMACY_RELATION_INFO.szSignatureInfo);
					}
				}
				num++;
			}
		}

		[MessageHandler(1194)]
		public static void ChangePersonSgin(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stSignatureRsp.dwResult != 0u)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Utility.ProtErrCodeToStr(1194, (int)msg.stPkgData.stSignatureRsp.dwResult), false);
			}
			else
			{
				if (Singleton<CPlayerInfoSystem>.GetInstance().CurTab == CPlayerInfoSystem.Tab.Base_Info)
				{
					Singleton<CPlayerInfoSystem>.GetInstance().UpdateBaseInfo();
				}
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					masterRoleInfo.PersonSign = Singleton<CPlayerInfoSystem>.instance.m_PlayerProfile.m_personSign;
				}
			}
		}

		public override void Init()
		{
			base.Init();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
			instance.AddUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
			instance.AddUIEventListener(enUIEventID.Player_Info_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoTabChange));
			instance.AddUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
			instance.AddUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
			instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
			instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
			instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
			instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
			instance.AddUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
			instance.AddUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
			instance.AddUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			instance.AddUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
			instance.AddUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
			instance.AddUIEventListener(enUIEventID.Player_Info_Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnAchievementTrophyClick));
			this.m_IsFormOpen = false;
			this.m_CurTab = CPlayerInfoSystem.Tab.Base_Info;
			this.m_Form = null;
			instance.AddUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
			instance.AddUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.UpdateNobeHeadIdx));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this.UpdateHeadFlag));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this.OnPlayerNameChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, new Action<byte, CAchieveItem2>(this.OnTrophySelectDone));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this.UpdateXinYueBtn));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnTrophyStateChange));
		}

		public override void UnInit()
		{
			base.UnInit();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
			instance.RemoveUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.UpdateNobeHeadIdx));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this.UpdateHeadFlag));
			instance.RemoveUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
			instance.RemoveUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
			instance.RemoveUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
			instance.RemoveUIEventListener(enUIEventID.Player_Info_Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnAchievementTrophyClick));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, new Action<byte, CAchieveItem2>(this.OnTrophySelectDone));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this.UpdateXinYueBtn));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this.OnTrophyStateChange));
		}

		public CPlayerProfile GetProfile()
		{
			return this.m_PlayerProfile;
		}

		public void OpenPvpInfo()
		{
			this.ShowPlayerDetailInfo(0uL, 0, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		public void OpenBaseInfo()
		{
			this.ShowPlayerDetailInfo(0uL, 0, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Base_Info);
		}

		public void OpenForm(CPlayerInfoSystem.Tab defaultTab = CPlayerInfoSystem.Tab.Base_Info)
		{
			if (this.m_IsFormOpen)
			{
				this.m_Form = Singleton<CUIManager>.GetInstance().GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			}
			else
			{
				this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(CPlayerInfoSystem.sPlayerInfoFormPath, true, true);
			}
			this.m_IsFormOpen = true;
			this.CurTab = CPlayerInfoSystem.Tab.Base_Info;
			this.InitTab();
			GameObject widget = this.m_Form.GetWidget(0);
			CUIListScript component = widget.GetComponent<CUIListScript>();
			if (defaultTab != CPlayerInfoSystem.Tab.Base_Info)
			{
				if (component != null)
				{
					component.SelectElement((int)defaultTab, true);
				}
				this.CurTab = defaultTab;
			}
			Singleton<CPlayerPvpInfoController>.instance.InitUI();
			Singleton<CPlayerCommonHeroInfoController>.instance.InitCommonHeroUI();
			if (component != null)
			{
				CUIListElementScript elemenet = component.GetElemenet(7);
				if (elemenet != null)
				{
					Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.gameObject, enNewFlagKey.New_Player_Jiaoyou_V15, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
				}
			}
		}

		private void ProcessNobeHeadIDx(CUIFormScript form, bool bshow)
		{
			GameObject obj = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHeadEditable/changeNobeheadicon");
			if (!CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid))
			{
				obj.CustomSetActive(false);
				return;
			}
			if (CSysDynamicBlock.bVipBlock)
			{
				bshow = false;
			}
			if (bshow)
			{
				obj.CustomSetActive(true);
			}
			else
			{
				obj.CustomSetActive(false);
			}
		}

		private void ProcessQQVIP(CUIFormScript form, bool bShow)
		{
			if (form == null)
			{
				return;
			}
			GameObject obj = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn");
			GameObject gameObject = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlHead/NameGroup/QQVipIcon");
			if (!CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid))
			{
				obj.CustomSetActive(false);
				MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(gameObject.GetComponent<Image>(), (int)this.m_PlayerProfile.qqVipMask);
				return;
			}
			GameObject gameObject2 = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn/Text");
			if (!bShow)
			{
				obj.CustomSetActive(false);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				return;
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				obj.CustomSetActive(true);
			}
			else
			{
				obj.CustomSetActive(false);
			}
			gameObject2.CustomSetActive(true);
			gameObject.CustomSetActive(false);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				obj.CustomSetActive(false);
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				return;
			}
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				obj.CustomSetActive(true);
			}
			else
			{
				obj.CustomSetActive(false);
			}
			Text component = gameObject2.GetComponent<Text>();
			component.text = Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(gameObject.GetComponent<Image>());
			}
		}

		private void OpenByQQVIP(CUIEvent uiEvent)
		{
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					if (masterRoleInfo.HasVip(16))
					{
						Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Super_Vip"), 1);
					}
					else if (masterRoleInfo.HasVip(1))
					{
						Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Vip"), 1);
					}
					else
					{
						Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip"), 1);
					}
				}
			}
		}

		public void ProcessCommonQQVip(GameObject parent)
		{
			if (!parent)
			{
				return;
			}
			GameObject gameObject = parent.transform.FindChild("QQSVipIcon").gameObject;
			GameObject gameObject2 = parent.transform.FindChild("QQVipIcon").gameObject;
			gameObject2.CustomSetActive(false);
			gameObject.CustomSetActive(false);
		}

		private void OnPlayerInfo_OpenForm(CUIEvent uiEvent)
		{
			this.ShowPlayerDetailInfo(0uL, 0, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true, CPlayerInfoSystem.Tab.Base_Info);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroHeadBtn);
		}

		private void OnPlayerInfo_CloseForm(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen)
			{
				return;
			}
			this.m_IsFormOpen = false;
			this.m_Form = null;
			this.IsForbidShowPvpHistoryDetailInfo = false;
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Form_Close);
		}

		private void OnPlayerInfoOpenPvpInfo(CUIEvent uiEvent)
		{
			this.OpenPvpInfo();
		}

		private void OnPlayerInfoOpenBaseInfo(CUIEvent uiEvent)
		{
			this.OpenBaseInfo();
		}

		private void OnPlayerInfoQuitGame(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_QuitGameTips"), enUIEventID.Player_Info_Quit_Game_Confirm, enUIEventID.None, false);
		}

		private void OnPlayerInfoQuitGameConfirm(CUIEvent uiEvent)
		{
			SGameApplication.Quit();
		}

		private void OnPlayerInfoMostUsedHeroItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidget == null)
			{
				return;
			}
			GameObject gameObject = srcWidget.transform.Find("heroItem").gameObject;
			ListView<COMDT_MOST_USED_HERO_INFO> listView = this.m_PlayerProfile.MostUsedHeroList();
			if (srcWidgetIndexInBelongedList >= listView.Count)
			{
				return;
			}
			COMDT_MOST_USED_HERO_INFO cOMDT_MOST_USED_HERO_INFO = listView[srcWidgetIndexInBelongedList];
			this.SetHeroItemData(uiEvent.m_srcFormScript, gameObject, cOMDT_MOST_USED_HERO_INFO);
			Text componetInChild = Utility.GetComponetInChild<Text>(srcWidget, "usedCnt");
			if (componetInChild != null)
			{
				componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Games_Cnt_Label"), cOMDT_MOST_USED_HERO_INFO.dwGameWinNum + cOMDT_MOST_USED_HERO_INFO.dwGameLoseNum);
			}
		}

		private void OnPlayerInfoMostUsedHeroItemClick(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_CloseForm, uiEvent.m_eventParams);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, uiEvent.m_eventParams);
		}

		private void OnPlayerInfoShowRule(CUIEvent uiEvent)
		{
			ushort key;
			if (this.m_CurTab == CPlayerInfoSystem.Tab.PvpCreditScore_Info)
			{
				key = 11;
			}
			else
			{
				key = 3;
			}
			ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint)key);
			if (dataByKey != null)
			{
				string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
				string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
				Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
			}
		}

		public void SetHeroItemData(CUIFormScript formScript, GameObject listItem, COMDT_MOST_USED_HERO_INFO heroInfo)
		{
			if (listItem == null || heroInfo == null)
			{
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData(heroInfo.dwHeroID);
			Transform transform = listItem.transform;
			ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(heroData.heroType, (int)heroInfo.dwProficiencyLv);
			if (heroProficiency != null)
			{
				listItem.GetComponent<Image>().SetSprite(string.Format("{0}{1}", "UGUI/Sprite/Dynamic/Quality/", StringHelper.UTF8BytesToString(ref heroProficiency.szImagePath)), formScript, true, false, false, false);
			}
			string heroSkinPic = CSkinInfo.GetHeroSkinPic(heroInfo.dwHeroID, 0u);
			CUICommonSystem.SetHeroItemImage(formScript, listItem, heroSkinPic, enHeroHeadType.enIcon, false, false);
			GameObject gameObject = transform.Find("profession").gameObject;
			CUICommonSystem.SetHeroJob(formScript, gameObject, (enHeroJobType)heroData.heroType);
			Text component = transform.Find("heroNameText").GetComponent<Text>();
			component.text = heroData.heroName;
			CUIEventScript component2 = listItem.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.openHeroFormPar.heroId = heroData.cfgID;
			eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
			component2.SetUIEvent(enUIEventType.Click, enUIEventID.Player_Info_Most_Used_Hero_Item_Click, eventParams);
		}

		private void OnPlayerInfoTabChange(CUIEvent uiEvent)
		{
			if (this.m_Form == null)
			{
				return;
			}
			if (!this.m_IsFormOpen)
			{
				return;
			}
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			int selectedIndex = component.GetSelectedIndex();
			this.CurTab = (CPlayerInfoSystem.Tab)selectedIndex;
			GameObject widget = this.m_Form.GetWidget(1);
			GameObject widget2 = this.m_Form.GetWidget(5);
			GameObject widget3 = this.m_Form.GetWidget(7);
			GameObject widget4 = this.m_Form.GetWidget(8);
			GameObject widget5 = this.m_Form.GetWidget(10);
			GameObject widget6 = this.m_Form.GetWidget(9);
			GameObject widget7 = this.m_Form.GetWidget(12);
			GameObject widget8 = this.m_Form.GetWidget(2);
			GameObject widget9 = this.m_Form.GetWidget(21);
			GameObject widget10 = this.m_Form.GetWidget(32);
			GameObject widget11 = this.m_Form.GetWidget(31);
			GameObject widget12 = this.m_Form.GetWidget(34);
			widget10.CustomSetActive(false);
			widget11.CustomSetActive(false);
			if (widget7 != null)
			{
				this.SetTitle(this.m_CurTab, widget7.transform);
			}
			Transform transform = this.m_Form.transform.Find("pnlBg/pnlBody/pnlHonorInfo");
			GameObject obj = null;
			if (transform != null)
			{
				obj = transform.gameObject;
			}
			Transform transform2 = this.m_Form.transform.Find("pnlBg/pnlBody/pnlPvPHistory");
			GameObject obj2 = null;
			if (transform2 != null)
			{
				obj2 = transform2.gameObject;
			}
			Transform transform3 = this.m_Form.transform.Find("pnlBg/pnlBody/pnlCreditScoreInfo");
			GameObject obj3 = null;
			if (transform3 != null)
			{
				obj3 = transform3.gameObject;
			}
			switch (this.m_CurTab)
			{
			case CPlayerInfoSystem.Tab.Base_Info:
			{
				widget11.CustomSetActive(true);
				widget6.CustomSetActive(true);
				widget5.CustomSetActive(false);
				widget.CustomSetActive(true);
				widget2.CustomSetActive(false);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				this.UpdateBaseInfo();
				this.ProcessQQVIP(this.m_Form, true);
				this.ProcessNobeHeadIDx(this.m_Form, true);
				int mentorLv = 0;
				if (this.m_PlayerProfile != null && this.m_PlayerProfile._mentorInfo != null)
				{
					mentorLv = (int)this.m_PlayerProfile._mentorInfo.dwMasterLevel;
				}
				GameObject mentorLvGo = Utility.FindChild(this.m_Form.gameObject, "pnlBg/pnlBody/pnlHead/NameGroup/MentorIcon");
				UT.SetMentorLv(mentorLvGo, mentorLv);
				break;
			}
			case CPlayerInfoSystem.Tab.Pvp_Info:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(true);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(true);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				this.UpdatePvpInfo2();
				break;
			case CPlayerInfoSystem.Tab.Honor_Info:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(true);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				this.LoadSubModule();
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickGloryPoints, new uint[0]);
				break;
			case CPlayerInfoSystem.Tab.Common_Hero:
				widget6.CustomSetActive(true);
				widget5.CustomSetActive(false);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				obj3.CustomSetActive(false);
				obj.CustomSetActive(false);
				widget4.CustomSetActive(true);
				widget3.CustomSetActive(true);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				Singleton<CPlayerCommonHeroInfoController>.instance.UpdateUI();
				break;
			case CPlayerInfoSystem.Tab.PvpHistory_Info:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(true);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				this.LoadSubModule();
				break;
			case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
				widget6.CustomSetActive(true);
				widget5.CustomSetActive(false);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(true);
				obj3.CustomSetActive(true);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget12.CustomSetActive(false);
				this.LoadSubModule();
				break;
			case CPlayerInfoSystem.Tab.Mentor_Info:
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(false);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(true);
				widget12.CustomSetActive(false);
				this.LoadSubModule();
				break;
			case CPlayerInfoSystem.Tab.Social_Info:
			{
				widget11.CustomSetActive(true);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(false);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget10.CustomSetActive(true);
				widget12.CustomSetActive(false);
				this.UpdateHeadRankInfo();
				this.LoadSubModule();
				GameObject widget13 = this.m_Form.GetWidget(0);
				CUIListScript component2 = widget13.GetComponent<CUIListScript>();
				if (component2 != null)
				{
					CUIListElementScript elemenet = component2.GetElemenet(7);
					if (elemenet != null)
					{
						Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(elemenet.gameObject, enNewFlagKey.New_Player_Jiaoyou_V15, true);
					}
				}
				Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(30u, null, false);
				break;
			}
			case CPlayerInfoSystem.Tab.Relation_Info:
			{
				widget11.CustomSetActive(false);
				widget.CustomSetActive(false);
				widget2.CustomSetActive(false);
				widget4.CustomSetActive(false);
				obj3.CustomSetActive(false);
				widget3.CustomSetActive(false);
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				widget8.CustomSetActive(false);
				widget9.CustomSetActive(false);
				widget10.CustomSetActive(false);
				widget12.CustomSetActive(true);
				CPlayerProfile profile = this.GetProfile();
				if (profile.cacheIntimacyRelationRsp == null)
				{
					this.ReqOtherPlayerIntimacyRelation(profile.m_uuid, profile.m_iLogicWorldId);
				}
				else
				{
					this.ImpResIntimacyRelation(profile.cacheIntimacyRelationRsp);
				}
				break;
			}
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Tab_Change);
		}

		public void LoadSubModule()
		{
			DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			bool flag = false;
			GameObject widget = this.m_Form.GetWidget(10);
			GameObject widget2 = this.m_Form.GetWidget(9);
			if (widget != null && widget2 != null)
			{
				switch (this.m_CurTab)
				{
				case CPlayerInfoSystem.Tab.Honor_Info:
					flag = Singleton<CPlayerHonorController>.GetInstance().Loaded(this.m_Form);
					if (!flag)
					{
						widget.CustomSetActive(true);
						Singleton<CPlayerHonorController>.GetInstance().Load(this.m_Form);
						widget2.CustomSetActive(false);
					}
					break;
				case CPlayerInfoSystem.Tab.PvpHistory_Info:
					flag = Singleton<CPlayerPvpHistoryController>.GetInstance().Loaded(this.m_Form);
					if (!flag)
					{
						widget.CustomSetActive(true);
						Singleton<CPlayerPvpHistoryController>.GetInstance().Load(this.m_Form);
						widget2.CustomSetActive(false);
					}
					break;
				case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
					flag = Singleton<CPlayerCreaditScoreController>.GetInstance().Loaded(this.m_Form);
					if (!flag)
					{
						widget.CustomSetActive(true);
						Singleton<CPlayerCreaditScoreController>.GetInstance().Load(this.m_Form);
						widget2.CustomSetActive(false);
					}
					break;
				case CPlayerInfoSystem.Tab.Mentor_Info:
					Singleton<CPlayerMentorInfoController>.instance.UpdateUI();
					break;
				case CPlayerInfoSystem.Tab.Social_Info:
					Singleton<CPlayerSocialInfoController>.GetInstance().Load(this.m_Form);
					break;
				}
			}
			if (!flag)
			{
				GameObject widget3 = this.m_Form.GetWidget(11);
				if (widget3 != null)
				{
					CUITimerScript component = widget3.GetComponent<CUITimerScript>();
					if (component != null)
					{
						component.ReStartTimer();
					}
				}
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_Update_Sub_Module);
			}
		}

		private void OnUpdateSubModule(CUIEvent uiEvent)
		{
			DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(10);
			GameObject widget2 = this.m_Form.GetWidget(9);
			widget2.CustomSetActive(true);
			widget.CustomSetActive(false);
			switch (this.m_CurTab)
			{
			case CPlayerInfoSystem.Tab.Honor_Info:
				Singleton<CPlayerHonorController>.GetInstance().Draw(this.m_Form);
				break;
			case CPlayerInfoSystem.Tab.PvpHistory_Info:
				Singleton<CPlayerPvpHistoryController>.GetInstance().Draw(this.m_Form);
				break;
			case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
				Singleton<CPlayerCreaditScoreController>.GetInstance().Draw(this.m_Form);
				break;
			}
		}

		private void RefreshLicenseInfoPanel(CUIFormScript form)
		{
			if (null == form)
			{
				return;
			}
			GameObject widget = form.GetWidget(6);
			if (null == widget)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.m_licenseInfo != null)
			{
				CUIListScript component = widget.GetComponent<CUIListScript>();
				if (component != null)
				{
					component.SetElementAmount(masterRoleInfo.m_licenseInfo.m_licenseList.Count);
				}
			}
		}

		private void OnLicenseListItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.m_licenseInfo != null)
			{
				CLicenseItem licenseItemByIndex = masterRoleInfo.m_licenseInfo.GetLicenseItemByIndex(srcWidgetIndexInBelongedList);
				if (srcWidget != null && licenseItemByIndex != null && licenseItemByIndex.m_resLicenseInfo != null)
				{
					Transform transform = srcWidget.transform.Find("licenseIcon");
					transform.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, licenseItemByIndex.m_resLicenseInfo.szIconPath), this.m_Form, true, false, false, false);
					Transform transform2 = srcWidget.transform.Find("licenseNameText");
					transform2.GetComponent<Text>().text = licenseItemByIndex.m_resLicenseInfo.szLicenseName;
					Transform transform3 = srcWidget.transform.Find("licenseStateText");
					if (licenseItemByIndex.m_getSecond > 0u)
					{
						DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)licenseItemByIndex.m_getSecond));
						transform3.GetComponent<Text>().text = string.Format("<color=#00d519>{0}/{1}/{2}</color>", dateTime.Year, dateTime.Month, dateTime.Day);
					}
					else
					{
						transform3.GetComponent<Text>().text = "<color=#fecb2f>未获得</color>";
					}
					Transform transform4 = srcWidget.transform.Find("licenseDescText");
					transform4.GetComponent<Text>().text = licenseItemByIndex.m_resLicenseInfo.szDesc;
				}
			}
		}

		public static bool isSelf(ulong playerUllUID)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo != null && masterRoleInfo.playerUllUID == playerUllUID;
		}

		private void InitTab()
		{
			if (this.m_Form == null || !this.m_IsFormOpen)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget != null && widget.activeSelf)
			{
				widget.CustomSetActive(false);
			}
			CPlayerInfoSystem.Tab[] array = (CPlayerInfoSystem.Tab[])Enum.GetValues(typeof(CPlayerInfoSystem.Tab));
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				CPlayerInfoSystem.Tab[] array2 = new CPlayerInfoSystem.Tab[array.Length];
				byte b = 0;
				byte b2 = 0;
				while ((int)b2 < array.Length)
				{
					switch (array[(int)b2])
					{
					case CPlayerInfoSystem.Tab.Base_Info:
					case CPlayerInfoSystem.Tab.Pvp_Info:
					case CPlayerInfoSystem.Tab.Honor_Info:
					case CPlayerInfoSystem.Tab.Common_Hero:
					case CPlayerInfoSystem.Tab.PvpHistory_Info:
					case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
					case CPlayerInfoSystem.Tab.Mentor_Info:
					{
						CPlayerInfoSystem.Tab[] arg_BB_0 = array2;
						byte expr_B2 = b;
                        b = (byte)(expr_B2 + (byte)1);
						arg_BB_0[(int)expr_B2] = array[(int)b2];
						break;
					}
					}
					b2 += 1;
				}
				array = new CPlayerInfoSystem.Tab[(int)b];
				for (byte b3 = 0; b3 < b; b3 += 1)
				{
					array[(int)b3] = array2[(int)b3];
				}
			}
			string[] array3 = new string[array.Length];
			byte b4 = 0;
			while ((int)b4 < array.Length)
			{
				switch (array[(int)b4])
				{
				case CPlayerInfoSystem.Tab.Base_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info");
					break;
				case CPlayerInfoSystem.Tab.Pvp_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info");
					break;
				case CPlayerInfoSystem.Tab.Honor_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info");
					break;
				case CPlayerInfoSystem.Tab.Common_Hero:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info");
					break;
				case CPlayerInfoSystem.Tab.PvpHistory_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info");
					break;
				case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info");
					break;
				case CPlayerInfoSystem.Tab.Mentor_Info:
					array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Mentor_Info");
					break;
				case CPlayerInfoSystem.Tab.Social_Info:
					if (!CSysDynamicBlock.bLobbyEntryBlocked)
					{
						array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Social_Info");
					}
					break;
				case CPlayerInfoSystem.Tab.Relation_Info:
					if (!CSysDynamicBlock.bLobbyEntryBlocked)
					{
						array3[(int)b4] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Relation_Info");
					}
					break;
				}
				b4 += 1;
			}
			GameObject widget2 = this.m_Form.GetWidget(0);
			CUIListScript component = widget2.GetComponent<CUIListScript>();
			if (component != null)
			{
				component.SetElementAmount(array3.Length);
				for (int i = 0; i < component.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
					component2.text = array3[i];
				}
				component.m_alwaysDispatchSelectedChangeEvent = true;
				component.SelectElement((int)this.CurTab, true);
			}
		}

		private void SetBaseInfoScrollable(bool scrollable = false)
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget == null || !widget.activeSelf)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "pnlContainer/pnlInfo/scrollRect");
			if (gameObject != null)
			{
				RectTransform component = gameObject.GetComponent<RectTransform>();
				ScrollRect component2 = gameObject.GetComponent<ScrollRect>();
				if (component != null)
				{
					if (scrollable)
					{
						component.offsetMin = new Vector2(component.offsetMin.x, 90f);
					}
					else
					{
						component.offsetMin = new Vector2(component.offsetMin.x, 0f);
					}
				}
				if (component2 != null)
				{
					component2.verticalNormalizedPosition = 1f;
				}
			}
		}

		private void DisplayCustomButton()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(9);
			GameObject widget2 = this.m_Form.GetWidget(1);
			if (widget2 == null || widget == null || !widget2.activeSelf)
			{
				return;
			}
			GameObject obj = Utility.FindChild(widget, "pnlHead/btnRename");
			GameObject obj2 = Utility.FindChild(widget, "pnlHead/btnShare");
			switch (this._lastDetailSource)
			{
			case CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers:
			case CPlayerInfoSystem.DetailPlayerInfoSource.PvpHistoryDetailInfo:
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				this.SetBaseInfoScrollable(false);
				this.SetAllGuildBtnActive(widget2, false);
				this.SetAllFriendBtnActive(widget2, false);
				this.SetAddFriendBtn(widget2);
				break;
			case CPlayerInfoSystem.DetailPlayerInfoSource.Self:
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				this.SetBaseInfoScrollable(false);
				this.SetAllGuildBtnActive(widget2, false);
				this.SetAppointMatchLeaderBtn();
				this.SetAllFriendBtnActive(widget2, false);
				this.SetAddFriendBtn(widget2);
				break;
			case CPlayerInfoSystem.DetailPlayerInfoSource.Guild:
				obj.CustomSetActive(false);
				obj2.CustomSetActive(false);
				this.SetBaseInfoScrollable(false);
				this.SetSingleGuildBtn(widget2);
				this.SetAllFriendBtnActive(widget2, false);
				this.SetAddFriendBtn(widget2);
				break;
			}
		}

		private void SetAllFriendBtnActive(GameObject root, bool isActive)
		{
			GameObject obj = Utility.FindChild(root, "pnlContainer/BtnGroup/btnSettings");
			GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnQuit");
			obj.CustomSetActive(isActive);
			obj2.CustomSetActive(isActive);
		}

		private void SetAddFriendBtn(GameObject root)
		{
			bool flag = CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid);
			GameObject gameObject = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAddFriend");
			if (CSysDynamicBlock.bFriendBlocked || flag || Singleton<CFriendContoller>.instance.model.IsGameFriend(this.m_PlayerProfile.m_uuid, (uint)this.m_PlayerProfile.m_iLogicWorldId))
			{
				gameObject.CustomSetActive(false);
				return;
			}
			gameObject.CustomSetActive(true);
			CUIEventScript componentInChildren = gameObject.GetComponentInChildren<CUIEventScript>();
			componentInChildren.m_onClickEventParams.commonUInt64Param1 = this.m_PlayerProfile.m_uuid;
			componentInChildren.m_onClickEventParams.commonUInt32Param1 = (uint)this.m_PlayerProfile.m_iLogicWorldId;
		}

		private void SetAllGuildBtnActive(GameObject root, bool isActive)
		{
			GameObject obj = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
			GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointOrCancelMatchLeader");
			GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
			GameObject obj4 = Utility.FindChild(root, "pnlContainer/btnFireMember");
			obj.CustomSetActive(isActive);
			obj2.CustomSetActive(isActive);
			obj3.CustomSetActive(isActive);
			obj4.CustomSetActive(isActive);
		}

		private void SetSingleGuildBtn(GameObject root)
		{
			GameObject obj = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
			GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
			GameObject obj3 = Utility.FindChild(root, "pnlContainer/btnFireMember");
			obj.CustomSetActive(this._isShowGuildAppointViceChairmanBtn);
			obj2.CustomSetActive(this._isShowGuildTransferPositionBtn);
			obj3.CustomSetActive(this._isShowGuildFireMemberBtn);
			this.SetAppointMatchLeaderBtn();
		}

		public void SetAppointMatchLeaderBtn()
		{
			if (this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(14);
			if (CGuildSystem.HasAppointMatchLeaderAuthority())
			{
				widget.CustomSetActive(true);
				this.SetAppointMatchLeaderBtnText(widget);
			}
			else
			{
				widget.CustomSetActive(false);
			}
		}

		private void SetAppointMatchLeaderBtnText(GameObject btnAppointOrCancelMatchLeader)
		{
			CUICommonSystem.SetButtonName(btnAppointOrCancelMatchLeader, (!CGuildHelper.IsGuildMatchLeaderPosition(this.m_PlayerProfile.m_uuid)) ? Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Apooint_Leader") : Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Cancel_Leader"));
		}

		private void UpdateNobeHeadIdx()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(9);
			if (widget == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "pnlHead/pnlImg/HttpImage/NobeImag");
			if (gameObject)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(gameObject.GetComponent<Image>(), (int)MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(gameObject.GetComponent<Image>(), (int)MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId, this.m_Form, 0.9f, false);
			}
		}

		private void OnPlayerNameChange()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(9);
			GameObject widget2 = this.m_Form.GetWidget(1);
			if (widget2 == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlHead/NameGroup/txtName");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (componetInChild != null && masterRoleInfo != null)
			{
				componetInChild.text = masterRoleInfo.Name;
			}
		}

		private void OnPersonSignEndEdit(string personSign)
		{
			if (string.Compare(personSign, this.m_PlayerProfile.m_personSign) != 0)
			{
				this.m_PlayerProfile.m_personSign = personSign;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1193u);
				StringHelper.StringToUTF8Bytes(personSign, ref cSPkg.stPkgData.stSignatureReq.szSignatureInfo);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		private void UpdateHeadFlag()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(this.m_Form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHeadEditable/changeNobeheadicon");
			if (gameObject != null)
			{
				bool flag = Singleton<HeadIconSys>.instance.UnReadFlagNum > 0u;
				if (flag)
				{
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(gameObject);
				}
			}
		}

		private void OnTrophySelectDone(byte idx, CAchieveItem2 item)
		{
			CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if ((int)idx < masterAchieveInfo.SelectedTrophies.Length)
			{
				masterAchieveInfo.SelectedTrophies[(int)idx] = item;
				CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
				profile.ConvertRoleInfoData(masterRoleInfo);
				this.UpdateBaseInfo();
			}
		}

		private void SetTitle(CPlayerInfoSystem.Tab tab, Transform titleTransform)
		{
			if (titleTransform == null)
			{
				return;
			}
			Text component = titleTransform.GetComponent<Text>();
			if (component == null)
			{
				return;
			}
			switch (tab)
			{
			case CPlayerInfoSystem.Tab.Base_Info:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info");
				break;
			case CPlayerInfoSystem.Tab.Pvp_Info:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info");
				break;
			case CPlayerInfoSystem.Tab.Honor_Info:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info");
				break;
			case CPlayerInfoSystem.Tab.Common_Hero:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info");
				break;
			case CPlayerInfoSystem.Tab.PvpHistory_Info:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info");
				break;
			case CPlayerInfoSystem.Tab.PvpCreditScore_Info:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info");
				break;
			default:
				component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Title");
				break;
			}
		}

		private void UpdateXinYueBtn()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget == null)
			{
				return;
			}
			Transform transform = widget.transform.Find("pnlContainer/BtnGroup/XYJLBBtn");
			if (transform)
			{
				transform.gameObject.CustomSetActive(!CSysDynamicBlock.bLobbyEntryBlocked);
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					if (masterRoleInfo.ShowGameRedDot)
					{
						CUICommonSystem.AddRedDot(transform.gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					}
					else
					{
						CUICommonSystem.DelRedDot(transform.gameObject);
					}
				}
			}
		}

		private void OnTrophyStateChange()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			if (this.CurTab != CPlayerInfoSystem.Tab.Base_Info)
			{
				return;
			}
			this.ProcessSelectedTrophies();
		}

		private void UpdateHeadInfo()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(9);
			if (widget == null)
			{
				return;
			}
			if (!CSysDynamicBlock.bSocialBlocked)
			{
				GameObject gameObject = Utility.FindChild(widget, "pnlHead/pnlImg/HttpImage");
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (gameObject != null && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()) && masterRoleInfo != null)
				{
					bool isSelf = masterRoleInfo.playerUllUID == this.m_PlayerProfile.m_uuid && masterRoleInfo.logicWorldID == this.m_PlayerProfile.m_iLogicWorldId;
					CUIHttpImageScript component = gameObject.GetComponent<CUIHttpImageScript>();
					component.SetImageUrl(this.m_PlayerProfile.HeadUrl());
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int)this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwCurLevel, false, isSelf, this.m_PlayerProfile.m_userPrivacyBits);
					GameObject gameObject2 = Utility.FindChild(widget, "pnlHead/pnlImg/HttpImage/NobeImag");
					if (gameObject2)
					{
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(gameObject2.GetComponent<Image>(), (int)this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId);
						MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(gameObject2.GetComponent<Image>(), (int)this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId, this.m_Form, 0.9f, false);
					}
				}
			}
			COM_PRIVILEGE_TYPE privilegeType = this.m_PlayerProfile.PrivilegeType();
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlHead/NameGroup/WXGameCenterIcon"), privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlHead/NameGroup/QQGameCenterIcon"), privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlHead/NameGroup/GuestGameCenterIcon"), privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			this.UpdateHeadRankInfo();
			Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlHead/Level");
			if (componetInChild != null)
			{
				componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel"), this.m_PlayerProfile.PvpLevel());
			}
			Image component2 = Utility.FindChild(widget, "pnlHead/NameGroup/Gender").GetComponent<Image>();
			component2.gameObject.CustomSetActive(this.m_PlayerProfile.Gender() != COM_SNSGENDER.COM_SNSGENDER_NONE);
			if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_MALE)
			{
				CUIUtility.SetImageSprite(component2, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
			else if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
			{
				CUIUtility.SetImageSprite(component2, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
			Text componetInChild2 = Utility.GetComponetInChild<Text>(widget, "pnlHead/NameGroup/txtName");
			if (componetInChild2 != null)
			{
				componetInChild2.text = this.m_PlayerProfile.Name();
			}
			GameObject obj = Utility.FindChild(widget, "pnlHead/Status/ExtraCoin");
			obj.CustomSetActive(false);
			GameObject obj2 = Utility.FindChild(widget, "pnlHead/Status/ExtraExp");
			obj2.CustomSetActive(false);
			GameObject gameObject3 = Utility.FindChild(widget, "pnlHead/GuildInfo/Name");
			GameObject gameObject4 = Utility.FindChild(widget, "pnlHead/GuildInfo/Position");
			if (gameObject3 != null && gameObject4 != null)
			{
				Text component3 = gameObject3.GetComponent<Text>();
				Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject4, "Text");
				if (!CGuildSystem.IsInNormalGuild(this.m_PlayerProfile.GuildState) || string.IsNullOrEmpty(this.m_PlayerProfile.GuildName))
				{
					if (component3 != null)
					{
						component3.text = Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild");
					}
					gameObject4.CustomSetActive(false);
				}
				else
				{
					if (component3 != null)
					{
						component3.text = this.m_PlayerProfile.GuildName;
					}
					gameObject4.CustomSetActive(true);
					if (componetInChild3 != null)
					{
						componetInChild3.text = CGuildHelper.GetPositionName(this.m_PlayerProfile.GuildState);
					}
				}
			}
		}

		private void UpdateHeadRankInfo()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(9);
			if (widget == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(widget, "pnlHead/Status/Rank");
			if (this.m_PlayerProfile.GetRankGrade() == 0 || this.CurTab == CPlayerInfoSystem.Tab.Base_Info)
			{
				if (gameObject)
				{
					gameObject.CustomSetActive(false);
				}
			}
			else
			{
				gameObject.CustomSetActive(true);
				Image image = null;
				Image image2 = null;
				if (gameObject != null)
				{
					image = Utility.GetComponetInChild<Image>(gameObject, "ImgRank");
					image2 = Utility.GetComponetInChild<Image>(gameObject, "ImgRank/ImgSubRank");
				}
				if (image != null)
				{
					string rankSmallIconPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), (uint)this.m_PlayerProfile.GetRankClass());
					image.SetSprite(rankSmallIconPath, this.m_Form, true, false, false, false);
				}
				if (image2 != null)
				{
					string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), (uint)this.m_PlayerProfile.GetRankClass());
					image2.SetSprite(subRankSmallIconPath, this.m_Form, true, false, false, false);
				}
			}
			GameObject gameObject2 = Utility.FindChild(widget, "pnlHead/Status/HisRank");
			if (this.m_PlayerProfile.GetHistoryHighestRankGrade() == 0 || this.CurTab == CPlayerInfoSystem.Tab.Base_Info)
			{
				if (gameObject2)
				{
					gameObject2.CustomSetActive(false);
				}
			}
			else
			{
				gameObject2.CustomSetActive(true);
				Image image3 = null;
				Image image4 = null;
				if (gameObject2 != null)
				{
					image3 = Utility.GetComponetInChild<Image>(gameObject2, "ImgRank");
					image4 = Utility.GetComponetInChild<Image>(gameObject2, "ImgRank/ImgSubRank");
				}
				if (image3 != null)
				{
					string rankSmallIconPath2 = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
					image3.SetSprite(rankSmallIconPath2, this.m_Form, true, false, false, false);
				}
				if (image4 != null)
				{
					string subRankSmallIconPath2 = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
					image4.SetSprite(subRankSmallIconPath2, this.m_Form, true, false, false, false);
				}
			}
		}

		private void UpdateBaseInfo()
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget == null)
			{
				return;
			}
			this.UpdateHeadInfo();
			this.DisplayCustomButton();
			this.UpdateHeadFlag();
			COM_PRIVILEGE_TYPE cOM_PRIVILEGE_TYPE = this.m_PlayerProfile.PrivilegeType();
			COM_PRIVILEGE_TYPE privilegeType = (cOM_PRIVILEGE_TYPE != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/WXGameCenter/WXGameCenterBtn"), privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterBtn"), cOM_PRIVILEGE_TYPE, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			COM_PRIVILEGE_TYPE privilegeType2 = (cOM_PRIVILEGE_TYPE != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterGrey"), privilegeType2, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
			MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/GuestGameCenter/GuestGameCenterBtn"), cOM_PRIVILEGE_TYPE, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
			GameObject gameObject = Utility.FindChild(widget, "pnlContainer/pnlInfo/RankWangZheCnt/Rank");
			if (this.m_PlayerProfile.GetRankGrade() == 0)
			{
				if (gameObject)
				{
					gameObject.CustomSetActive(false);
				}
			}
			else
			{
				gameObject.CustomSetActive(true);
				Image image = null;
				Image image2 = null;
				if (gameObject != null)
				{
					image = Utility.GetComponetInChild<Image>(gameObject, "ImgRank");
					image2 = Utility.GetComponetInChild<Image>(gameObject, "ImgRank/ImgSubRank");
				}
				if (image != null)
				{
					string rankSmallIconPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), (uint)this.m_PlayerProfile.GetRankClass());
					image.SetSprite(rankSmallIconPath, this.m_Form, true, false, false, false);
				}
				if (image2 != null)
				{
					string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), (uint)this.m_PlayerProfile.GetRankClass());
					image2.SetSprite(subRankSmallIconPath, this.m_Form, true, false, false, false);
				}
			}
			GameObject gameObject2 = Utility.FindChild(widget, "pnlContainer/pnlInfo/RankWangZheCnt/HisRank");
			if (this.m_PlayerProfile.GetHistoryHighestRankGrade() == 0)
			{
				if (gameObject2)
				{
					gameObject2.CustomSetActive(false);
				}
			}
			else
			{
				gameObject2.CustomSetActive(true);
				Image image3 = null;
				Image image4 = null;
				if (gameObject2 != null)
				{
					image3 = Utility.GetComponetInChild<Image>(gameObject2, "ImgRank");
					image4 = Utility.GetComponetInChild<Image>(gameObject2, "ImgRank/ImgSubRank");
				}
				if (image3 != null)
				{
					string rankSmallIconPath2 = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
					image3.SetSprite(rankSmallIconPath2, this.m_Form, true, false, false, false);
				}
				if (image4 != null)
				{
					string subRankSmallIconPath2 = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
					image4.SetSprite(subRankSmallIconPath2, this.m_Form, true, false, false, false);
				}
			}
			GameObject gameObject3 = Utility.FindChild(widget, "pnlContainer/pnlInfo/RankWangZheCnt/KingMark");
			if (gameObject3 != null)
			{
				Image component = gameObject3.transform.FindChild("KingMark").GetComponent<Image>();
				component.SetSprite(string.Format(CLadderSystem.ICON_KING_MARK_PATH, this.m_PlayerProfile._wangZheCnt), this.m_Form, true, false, false, false);
				GameObject gameObject4 = gameObject3.transform.FindChild("KingMarkEventPanel").gameObject;
				if (gameObject4 != null)
				{
					CUICommonSystem.SetCommonTipsEvent(this.m_Form, gameObject4, Singleton<CTextManager>.GetInstance().GetText("Ladder_KingMark_Tips"), enUseableTipsPos.enTop);
				}
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelGeiLiDuiYouCnt/numCnt");
			if (componetInChild != null)
			{
				componetInChild.text = (this.m_PlayerProfile.Pvp1V1TotalGameCnt() + this.m_PlayerProfile.Pvp3V3TotalGameCnt() + this.m_PlayerProfile.Pvp5V5TotalGameCnt() + this.m_PlayerProfile.EntertainmentTotalGameCnt() + this.m_PlayerProfile.PvpGuildTotalGameCnt() + this.m_PlayerProfile.RankTotalGameCnt()).ToString();
			}
			Text componetInChild2 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelKeJingDuiShouCnt/numCnt");
			if (componetInChild2 != null)
			{
				componetInChild2.text = (this.m_PlayerProfile._geiLiDuiYou + this.m_PlayerProfile._keJingDuiShou).ToString();
			}
			Text componetInChild3 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelHeroCnt/numCnt");
			if (componetInChild3 != null)
			{
				componetInChild3.text = this.m_PlayerProfile.HeroCnt().ToString();
			}
			Text componetInChild4 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelSkinCnt/numCnt");
			if (componetInChild4 != null)
			{
				componetInChild4.text = this.m_PlayerProfile.SkinCnt().ToString();
			}
			bool flag = CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid);
			GameObject widget2 = this.m_Form.GetWidget(3);
			widget2.CustomSetActive(flag);
			GameObject widget3 = this.m_Form.GetWidget(13);
			InputField component2 = widget3.GetComponent<InputField>();
			widget3.CustomSetActive(true);
			if (component2)
			{
				if (string.IsNullOrEmpty(this.m_PlayerProfile.m_personSign))
				{
					widget3.CustomSetActive(flag);
					component2.text = string.Empty;
				}
				else
				{
					component2.text = this.m_PlayerProfile.m_personSign;
				}
				if (flag)
				{
					component2.interactable = true;
					component2.onEndEdit.RemoveAllListeners();
					component2.onEndEdit.AddListener(new UnityAction<string>(this.OnPersonSignEndEdit));
				}
				else
				{
					component2.interactable = false;
				}
			}
			GameObject obj = Utility.FindChild(widget, "pnlContainer/BtnGroup/JFQBtn");
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				if (!CSysDynamicBlock.bJifenHallBlock)
				{
					obj.CustomSetActive(flag);
				}
				else
				{
					obj.CustomSetActive(false);
				}
			}
			else
			{
				obj.CustomSetActive(false);
			}
			GameObject gameObject5 = Utility.FindChild(widget, "pnlContainer/BtnGroup/BuLuoBtn");
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				gameObject5.CustomSetActive(flag);
				if (gameObject5)
				{
					Transform transform = gameObject5.transform.Find("Text");
					if (transform != null)
					{
						transform.GetComponent<Text>().text = "QQ部落";
					}
				}
			}
			else
			{
				gameObject5.CustomSetActive(false);
			}
			if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
			{
				gameObject5.CustomSetActive(false);
			}
			GameObject obj2 = Utility.FindChild(widget, "pnlContainer/BtnGroup/DeepLinkBtn");
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				obj2.CustomSetActive(false);
			}
			else
			{
				ulong curTime = (ulong)((long)CRoleInfo.GetCurrentUTCTime());
				if (MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.isTimeValid(curTime))
				{
					obj2.CustomSetActive(flag);
				}
				else
				{
					obj2.CustomSetActive(false);
				}
			}
			if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
			{
				obj2.CustomSetActive(false);
			}
			this.UpdateXinYueBtn();
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform2 = widget.transform.Find("pnlContainer/pnlHeadEditable/changeNobeheadicon");
				if (transform2)
				{
					transform2.gameObject.CustomSetActive(false);
				}
				Transform transform3 = widget.transform.Find("pnlContainer/BtnGroup/BuLuoBtn");
				if (transform3)
				{
					transform3.gameObject.CustomSetActive(false);
				}
				Transform transform4 = widget.transform.Find("pnlContainer/BtnGroup/QQVIPBtn");
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(false);
				}
				Transform transform5 = widget.transform.Find("pnlContainer/BtnGroup/JFQBtn");
				if (transform5)
				{
					transform5.gameObject.CustomSetActive(false);
				}
			}
			Image componetInChild5 = Utility.GetComponetInChild<Image>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Image/Icon");
			Text componetInChild6 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Level");
			GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Rank");
			Text componetInChild7 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Rank");
			GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Button");
			GameObject obj5 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/txtNotInRank");
			bool flag2 = CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid);
			if (flag2)
			{
				obj4.CustomSetActive(true);
			}
			else
			{
				obj4.CustomSetActive(false);
			}
			if (componetInChild5 != null)
			{
				CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
				if (achieveInfo.LastDoneTrophyRewardInfo != null)
				{
					componetInChild5.SetSprite(achieveInfo.LastDoneTrophyRewardInfo.GetTrophyImagePath(), this.m_Form, true, false, false, false);
				}
			}
			if (componetInChild6 != null)
			{
				componetInChild6.text = this.m_PlayerProfile._trophyRewardInfoLevel.ToString();
			}
			if (componetInChild7 != null)
			{
				if (this.m_PlayerProfile._trophyRank == 0u)
				{
					obj5.CustomSetActive(true);
					obj3.CustomSetActive(false);
				}
				else
				{
					obj5.CustomSetActive(false);
					obj3.CustomSetActive(true);
					componetInChild7.text = this.m_PlayerProfile._trophyRank.ToString();
				}
			}
			this.ProcessSelectedTrophies();
		}

		private void ProcessSelectedTrophies()
		{
			GameObject widget = this.m_Form.GetWidget(1);
			CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
			bool flag = CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid);
			bool flag2 = true;
			if (flag)
			{
				ListView<CAchieveItem2> trophies = achieveInfo.GetTrophies(enTrophyState.Finish);
				if (trophies.Count != 0)
				{
					flag2 = false;
				}
			}
			else
			{
				for (int i = 0; i < this.m_PlayerProfile._selectedTrophies.Length; i++)
				{
					if (this.m_PlayerProfile._selectedTrophies[i] != null)
					{
						flag2 = false;
						break;
					}
				}
			}
			ListView<CAchieveItem2> listView = new ListView<CAchieveItem2>();
			if (flag)
			{
				ListView<CAchieveItem2> trophies2 = achieveInfo.GetTrophies(enTrophyState.Finish);
				for (int j = trophies2.Count - 1; j >= 0; j--)
				{
					if (trophies2[j] != null && Array.IndexOf<CAchieveItem2>(this.m_PlayerProfile._selectedTrophies, trophies2[j]) < 0)
					{
						listView.Add(trophies2[j]);
					}
				}
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "pnlContainer/pnlTrophy/List");
			if (componetInChild == null)
			{
				DebugHelper.Assert(false, "Player Info selectedTrophyListScript is null!");
				return;
			}
			if (flag2)
			{
				componetInChild.SetElementAmount(0);
				if (flag)
				{
					Text component = componetInChild.GetWidget(0).GetComponent<Text>();
					component.text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Player_Info_Selected_Trophies_Self_No_Data");
				}
				else
				{
					Text component2 = componetInChild.GetWidget(0).GetComponent<Text>();
					component2.text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Player_Info_Selected_Trophies_Other_No_Data");
				}
			}
			else
			{
				componetInChild.SetElementAmount(this.m_PlayerProfile._selectedTrophies.Length);
				Singleton<CTrophySelector>.GetInstance().SelectedTrophies = this.m_PlayerProfile._selectedTrophies;
				for (int k = 0; k < this.m_PlayerProfile._selectedTrophies.Length; k++)
				{
					CUIListElementScript elemenet = componetInChild.GetElemenet(k);
					this.RefreshSelectedAchieveElement(elemenet, this.m_PlayerProfile._selectedTrophies[k], k, flag, listView);
				}
			}
		}

		private void RefreshSelectedAchieveElement(CUIListElementScript elementScript, CAchieveItem2 item, int index, bool isSelf, ListView<CAchieveItem2> filteredTrophies)
		{
			GameObject widget = elementScript.GetWidget(0);
			Image component = widget.GetComponent<Image>();
			GameObject widget2 = elementScript.GetWidget(1);
			GameObject widget3 = elementScript.GetWidget(2);
			Text component2 = widget3.GetComponent<Text>();
			GameObject widget4 = elementScript.GetWidget(3);
			Text component3 = widget4.GetComponent<Text>();
			GameObject widget5 = elementScript.GetWidget(4);
			GameObject widget6 = elementScript.GetWidget(5);
			CUIEventScript component4 = elementScript.GetComponent<CUIEventScript>();
			if (item == null)
			{
				widget.CustomSetActive(false);
				widget6.CustomSetActive(false);
				widget5.CustomSetActive(isSelf && filteredTrophies.Count > 0);
				widget2.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget4.CustomSetActive(!isSelf || filteredTrophies.Count == 0);
				component3.text = ((!isSelf) ? Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Chosen") : Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done"));
				if (filteredTrophies.Count > 0)
				{
					component4.enabled = true;
					component4.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Change_Selected_Trophy, new stUIEventParams
					{
						tag = index
					});
				}
				else
				{
					component4.enabled = false;
				}
			}
			else
			{
				widget.CustomSetActive(true);
				widget6.CustomSetActive(true);
				widget5.CustomSetActive(false);
				widget2.CustomSetActive(isSelf);
				widget3.CustomSetActive(true);
				widget4.CustomSetActive(true);
				CAchieveItem2 cAchieveItem = item.TryToGetMostRecentlyDoneItem();
				component2.text = cAchieveItem.Cfg.szName;
				component.SetSprite(cAchieveItem.GetAchieveImagePath(), elementScript.m_belongedFormScript, true, false, false, false);
				CAchievementSystem.SetAchieveBaseIcon(widget6.transform, cAchieveItem, elementScript.m_belongedFormScript);
				if (cAchieveItem.DoneTime == 0u)
				{
					component3.text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done");
				}
				else
				{
					component3.text = string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long)((ulong)cAchieveItem.DoneTime)), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
				}
				component4.enabled = true;
				component4.SetUIEvent(enUIEventType.Click, enUIEventID.Player_Info_Achievement_Trophy_Click, new stUIEventParams
				{
					commonUInt32Param1 = item.ID
				});
				if (isSelf)
				{
					CUIEventScript component5 = widget2.GetComponent<CUIEventScript>();
					component5.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Change_Selected_Trophy, new stUIEventParams
					{
						tag = index
					});
				}
			}
		}

		private void UpdatePvpInfo2()
		{
			Singleton<CPlayerPvpInfoController>.instance.UpdateUI();
		}

		private void DeepLinkClick(CUIEvent uiEvent)
		{
			if (ApolloConfig.platform == ApolloPlatform.Wechat && MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.bLoadSucc)
			{
				Debug.Log(string.Concat(new object[]
				{
					"deeplink ",
					MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType,
					" ",
					MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl
				}));
				Singleton<ApolloHelper>.GetInstance().OpenWeiXinDeeplink(MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType, MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl);
			}
		}

		private void OpenIntegralHall(CUIEvent uiEvent)
		{
			string text = "http://jfq.qq.com/comm/index_android.html";
			text = string.Format("{0}?partition={1}", text, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID);
			CUICommonSystem.OpenUrl(text, true, 0);
		}

		private void OpenQQBuluo(CUIEvent uievent)
		{
			if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
			{
				string strUrl = "http://xiaoqu.qq.com/cgi-bin/bar/qqgame/handle_ticket?redirect_url=http%3A%2F%2Fxiaoqu.qq.com%2Fmobile%2Fbarindex.html%3F%26_bid%3D%26_wv%3D1027%23bid%3D227061";
				CUICommonSystem.OpenUrl(strUrl, true, 0);
			}
			else if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				string strUrl2 = "http://game.weixin.qq.com/cgi-bin/h5/static/circle/index.html?jsapi=1&appid=wx95a3a4d7c627e07d&auth_type=2&ssid=12";
				CUICommonSystem.OpenUrl(strUrl2, true, 1);
			}
		}

		private void OnAchievementTrophyClick(CUIEvent uiEvent)
		{
			CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			Singleton<CAchievementSystem>.GetInstance().ShowTrophyDetail(achieveInfo, commonUInt32Param);
		}
	}
}

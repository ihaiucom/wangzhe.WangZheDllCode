using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CMatchingView
	{
		private const int s_pvpEntryCount = 23;

		private static int ShowDefaultHeadImgStartLogicLadderLevel = 0;

		private static COM_AI_LEVEL[] mapDifficultyList = new COM_AI_LEVEL[]
		{
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_EASY_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_HARD_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_EASY_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_HARD_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_EASY_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_HARD_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_EASY_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_HARD_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_EASY_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE,
			COM_AI_LEVEL.COM_AI_LEVEL_HARD_OF_NORMALBATTLE
		};

		public static void InitMatchingEntry(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			Transform transform = form.transform;
			uint[] array = new uint[23];
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_1V1"), ref array[0]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_3V3"), ref array[1]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_1V1_1"), ref array[2]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_1V1_1"), ref array[3]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_1V1_1"), ref array[4]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_3V3Team_1"), ref array[5]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_3V3Team_1"), ref array[6]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_3V3Team_1"), ref array[7]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5Team_1"), ref array[8]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5Team_1"), ref array[9]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5Team_1"), ref array[10]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_Melee_Team_1"), ref array[11]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_Melee_Team_1"), ref array[12]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_Melee_Team_1"), ref array[13]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5"), ref array[14]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_MELEE"), ref array[15]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), ref array[16]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Clone"), ref array[17]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5CD"), ref array[18]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Miwu"), ref array[19]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5TeamMiwu_1"), ref array[20]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5TeamMiwu_1"), ref array[21]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Robot_5V5TeamMiwu_1"), ref array[22]);
			CUIMiniEventScript[] array2 = new CUIMiniEventScript[]
			{
				transform.Find("panelGroup2/btnGroup/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup2/btnGroup/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button1/btnGrp/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button1/btnGrp/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button1/btnGrp/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button2/btnGrp/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button2/btnGrp/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button2/btnGrp/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button4/btnGrp/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button4/btnGrp/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button4/btnGrp/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button3/btnGrp/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button3/btnGrp/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button3/btnGrp/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup2/btnGroup/Button4").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup2/btnGroup/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup5/btnGroup/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup5/btnGroup/Button3").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup5/btnGroup/Button4").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup2/btnGroup/Button5").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button5/btnGrp/Button1").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button5/btnGrp/Button2").GetComponent<CUIMiniEventScript>(),
				transform.Find("panelGroup3/btnGroup/Button5/btnGrp/Button3").GetComponent<CUIMiniEventScript>()
			};
			for (int i = 0; i < array.Length; i++)
			{
				array2[i].m_onClickEventParams.tagUInt = array[i];
				array2[i].m_onClickEventParams.tag = (int)CMatchingView.mapDifficultyList[i];
			}
			transform.Find("panelGroup1/btnGroup/Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
			transform.Find("panelGroup1/btnGroup/Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 2;
			transform.Find("panelGroup1/btnGroup/ButtonEntertain").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 3;
			transform.Find("panelGroup3/btnGroup/Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
			transform.Find("panelGroup3/btnGroup/Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
			transform.Find("panelGroup3/btnGroup/Button4").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 2;
			transform.Find("panelGroup3/btnGroup/Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 3;
			transform.Find("panelGroup3/btnGroup/Button5").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 4;
			transform.FindChild("panelGroup5/btnGroup/Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
			transform.FindChild("panelGroup5/btnGroup/Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
			transform.FindChild("panelGroup5/btnGroup/Button4").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 2;
			CMatchingView.ShowDefaultHeadImgStartLogicLadderLevel = (int)CLadderSystem.GetGradeDataByShowGrade((int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_SHOW_DEFAULT_HEADIMG_START_LADDERLEVEL)).bLogicGrade;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			GameObject gameObject = transform.FindChild("CoinAndExp/DoubleCoin").gameObject;
			GameObject gameObject2 = transform.FindChild("CoinAndExp/DoubleExp").gameObject;
			gameObject2.CustomSetActive(false);
			gameObject.CustomSetActive(false);
			masterRoleInfo.UpdateCoinAndExpValidTime();
			if (masterRoleInfo.HaveExtraCoin())
			{
				gameObject.CustomSetActive(true);
				string text = string.Empty;
				string text2 = string.Empty;
				if (masterRoleInfo.GetCoinExpireHours() > 0)
				{
					string text3 = Singleton<CTextManager>.GetInstance().GetText("DoubleCoinExpireTimeTips");
					text = string.Format(text3, masterRoleInfo.GetCoinExpireHours() / 24, masterRoleInfo.GetCoinExpireHours() % 24);
				}
				if (masterRoleInfo.GetCoinWinCount() > 0u)
				{
					string text4 = Singleton<CTextManager>.GetInstance().GetText("DoubleCoinCountWinTips");
					text2 = string.Format(text4, masterRoleInfo.GetCoinWinCount());
				}
				if (string.IsNullOrEmpty(text))
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject, string.Format("{0}", text2), enUseableTipsPos.enBottom);
				}
				else if (string.IsNullOrEmpty(text2))
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject, string.Format("{0}", text), enUseableTipsPos.enBottom);
				}
				else
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject, string.Format("{0}\n{1}", text, text2), enUseableTipsPos.enBottom);
				}
			}
			if (masterRoleInfo.HaveExtraExp())
			{
				gameObject2.CustomSetActive(true);
				string text5 = string.Empty;
				string text6 = string.Empty;
				if (masterRoleInfo.GetExpExpireHours() > 0)
				{
					text5 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleExpExpireTimeTips"), masterRoleInfo.GetExpExpireHours() / 24, masterRoleInfo.GetExpExpireHours() % 24);
				}
				if (masterRoleInfo.GetExpWinCount() > 0u)
				{
					text6 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleExpCountWinTips"), masterRoleInfo.GetExpWinCount());
				}
				if (string.IsNullOrEmpty(text5))
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject2, string.Format("{0}", text6), enUseableTipsPos.enBottom);
				}
				else if (string.IsNullOrEmpty(text6))
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject2, string.Format("{0}", text5), enUseableTipsPos.enBottom);
				}
				else
				{
					CUICommonSystem.SetCommonTipsEvent(form, gameObject2, string.Format("{0}\n{1}", text5, text6), enUseableTipsPos.enBottom);
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform2 = form.transform.Find("panelBottom/btnShop");
				if (transform2)
				{
					transform2.gameObject.CustomSetActive(false);
				}
				Transform transform3 = form.transform.Find("CoinAndExp");
				if (transform3)
				{
					transform3.gameObject.CustomSetActive(false);
				}
			}
			GameObject gameObject3 = form.gameObject.transform.Find("Panel").gameObject;
			gameObject3.transform.Find("Name").gameObject.GetComponent<Text>().set_text(masterRoleInfo.Name);
			ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)masterRoleInfo.PvpLevel));
			DebugHelper.Assert(dataByKey != null);
			DebugHelper.Assert(dataByKey.dwNeedExp > 0u);
			GameObject gameObject4 = gameObject3.transform.Find("DegreeBarBg/bar").gameObject;
			gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(204f * Math.Min(1f, masterRoleInfo.PvpExp * 1f / dataByKey.dwNeedExp), 19f);
			gameObject3.transform.Find("DegreeTitle").gameObject.CustomSetActive(false);
			if ((ulong)masterRoleInfo.PvpLevel >= (ulong)((long)GameDataMgr.acntPvpExpDatabin.Count()))
			{
				gameObject3.transform.Find("DegreeNum").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("PVP_Level_Max"));
			}
			else
			{
				gameObject3.transform.Find("DegreeNum").gameObject.GetComponent<Text>().set_text(string.Format("{0}/{1}", masterRoleInfo.PvpExp, dataByKey.dwNeedExp));
			}
			gameObject3.transform.Find("DegreeIcon").gameObject.CustomSetActive(false);
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterPvpEntry, new uint[0]);
		}

		public static void SetPlayerSlotData(GameObject item, TeamMember memberInfo, bool bAvailable)
		{
			if (bAvailable)
			{
				bool bActive = false;
				bool isSelfTeamMaster = Singleton<CMatchingSystem>.GetInstance().IsSelfTeamMaster;
				if (memberInfo == null)
				{
					item.CustomSetActive(true);
					item.transform.Find("Occupied").gameObject.CustomSetActive(false);
				}
				else
				{
					PlayerUniqueID stTeamMaster = Singleton<CMatchingSystem>.GetInstance().teamInfo.stTeamMaster;
					bActive = (stTeamMaster.ullUid == memberInfo.uID.ullUid && stTeamMaster.iGameEntity == memberInfo.uID.iGameEntity);
					PlayerUniqueID stSelfInfo = Singleton<CMatchingSystem>.GetInstance().teamInfo.stSelfInfo;
					bool flag = stSelfInfo.ullUid == memberInfo.uID.ullUid && stSelfInfo.iGameEntity == memberInfo.uID.iGameEntity;
					item.CustomSetActive(true);
					item.transform.Find("Occupied").gameObject.CustomSetActive(true);
					CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(item, "Occupied/BtnKick");
					componetInChild.m_onClickEventParams.tag = (int)memberInfo.dwPosOfTeam;
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
					bool flag2 = true;
					if (form != null)
					{
						CUIListScript component = form.GetWidget(7).GetComponent<CUIListScript>();
						flag2 = (component.GetSelectedIndex() == 0);
					}
					string text = flag2 ? Singleton<CInviteSystem>.GetInstance().GetInviteFriendName(memberInfo.uID.ullUid, (uint)memberInfo.uID.iLogicWorldId) : Singleton<CInviteSystem>.GetInstance().GetInviteGuildMemberName(memberInfo.uID.ullUid);
					item.transform.Find("Occupied/txtPlayerName").GetComponent<Text>().set_text(string.IsNullOrEmpty(text) ? memberInfo.MemberName : text);
					item.transform.Find("Occupied/BtnKick").gameObject.CustomSetActive(isSelfTeamMaster && !flag);
					Transform transform = item.transform.Find("Occupied/HeadBg/NobeIcon");
					Transform transform2 = item.transform.Find("Occupied/HeadBg/NobeImag");
					Transform transform3 = item.transform.Find("Occupied/BtnAddFriend");
					if (flag)
					{
						if (!CSysDynamicBlock.bFriendBlocked)
						{
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo != null)
							{
								Transform transform4 = item.transform.Find("Occupied/HeadBg/imgHead");
								if (transform4)
								{
									transform4.GetComponent<CUIHttpImageScript>().SetImageUrl(masterRoleInfo.HeadUrl);
								}
							}
						}
						if (transform3 != null)
						{
							transform3.gameObject.CustomSetActive(false);
						}
					}
					else
					{
						Transform transform5 = item.transform.Find("Occupied/HeadBg/imgHead");
						if (!CSysDynamicBlock.bFriendBlocked)
						{
							if (transform5)
							{
								transform5.GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(memberInfo.snsHeadUrl));
							}
						}
						else if (transform5)
						{
							transform5.GetComponent<CUIHttpImageScript>().SetImageUrl(null);
						}
						if (!CSysDynamicBlock.bFriendBlocked)
						{
							if (Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(memberInfo.uID.ullUid, (uint)memberInfo.uID.iLogicWorldId) == null)
							{
								if (transform3 != null)
								{
									transform3.gameObject.CustomSetActive(true);
									CUIEventScript component2 = transform3.GetComponent<CUIEventScript>();
									if (component2 != null)
									{
										component2.m_onClickEventParams.commonUInt64Param1 = memberInfo.uID.ullUid;
										component2.m_onClickEventParams.commonUInt32Param1 = (uint)memberInfo.uID.iLogicWorldId;
									}
								}
							}
							else if (transform3 != null)
							{
								transform3.gameObject.CustomSetActive(false);
							}
						}
						else if (transform3 != null)
						{
							transform3.gameObject.CustomSetActive(false);
						}
					}
				}
				item.transform.Find("Occupied/imgOwner").gameObject.CustomSetActive(bActive);
			}
			else
			{
				item.CustomSetActive(false);
			}
		}

		public static void InitConfirmBox(GameObject root, int PlayerNum, ref RoomInfo roomInfo, CUIFormScript form)
		{
			if (root.transform.Find("Panel/Timer") != null)
			{
				CUITimerScript component = root.transform.Find("Panel/Timer").GetComponent<CUITimerScript>();
				if (component != null)
				{
					component.EndTimer();
					component.StartTimer();
				}
			}
			if (root.transform.Find("Panel/Panel/Timer") != null)
			{
				CUITimerScript component2 = root.transform.Find("Panel/Panel/Timer").GetComponent<CUITimerScript>();
				if (component2 != null)
				{
					component2.EndTimer();
					component2.StartTimer();
				}
			}
			Transform transform = root.transform.Find("Panel/Panel/stateGroup");
			GridLayoutGroup component3 = transform.GetComponent<GridLayoutGroup>();
			if (component3)
			{
				component3.set_constraintCount((PlayerNum == 6) ? 3 : 5);
			}
			bool flag = roomInfo.roomAttrib.bPkAI == 2;
			int num = (!roomInfo.roomAttrib.bWarmBattle && flag) ? (PlayerNum / 2) : PlayerNum;
			for (int i = 1; i <= 10; i++)
			{
				GameObject gameObject = transform.Find(string.Format("icon{0}", i)).gameObject;
				gameObject.CustomSetActive(num >= i);
			}
			int num2 = 1;
			for (COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP++)
			{
				ListView<MemberInfo> listView = roomInfo[cOM_PLAYERCAMP];
				for (int j = 0; j < listView.Count; j++)
				{
					MemberInfo memberInfo = listView[j];
					Transform transform2 = transform.Find(string.Format("icon{0}", num2));
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (!CSysDynamicBlock.bSocialBlocked)
					{
						if ((roomInfo.roomAttrib.bMapType == 3 && masterRoleInfo != null && (int)CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade).bLogicGrade >= CMatchingView.ShowDefaultHeadImgStartLogicLadderLevel) || roomInfo.roomAttrib.bMapType == 5)
						{
							Image component4 = transform2.Find("HttpImage").GetComponent<Image>();
							if (component4)
							{
								component4.SetSprite(CUIUtility.s_Sprite_Dynamic_BustPlayer_Dir + "Common_PlayerImg", form, true, false, false, false);
							}
						}
						else
						{
							CUIHttpImageScript component5 = transform2.Find("HttpImage").GetComponent<CUIHttpImageScript>();
							if (component5)
							{
								component5.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(memberInfo.MemberHeadUrl));
							}
						}
					}
					Transform transform3 = transform.Find(string.Format("icon{0}/ready", num2));
					if (transform3)
					{
						transform3.gameObject.CustomSetActive(false);
					}
					Transform transform4 = transform.Find(string.Format("icon{0}/unready", num2));
					if (transform4)
					{
						transform4.gameObject.CustomSetActive(true);
					}
					num2++;
				}
			}
			Transform transform5 = root.transform.Find("Panel/Panel/TxtReadyNum");
			if (transform5)
			{
				Text component6 = transform5.GetComponent<Text>();
				if (component6)
				{
					component6.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Matching_Confirm_PlayerNum"), 0, num));
				}
			}
			Transform transform6 = root.transform.Find("Panel/Panel/btnGroup/Button_Confirm");
			if (transform6)
			{
				Button component7 = transform6.GetComponent<Button>();
				component7.set_interactable(true);
			}
		}

		public static void UpdateConfirmBox(GameObject root, ulong confirmPlayerUid)
		{
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			DebugHelper.Assert(roomInfo != null, "Room Info is NULL!!!");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "RoleInfo is NULL!!!");
			int currentMapPlayerNum = Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum;
			int confirmPlayerNum = Singleton<CMatchingSystem>.GetInstance().confirmPlayerNum;
			Transform transform = root.transform.Find("Panel/Panel/stateGroup");
			if (transform)
			{
				if ((roomInfo.roomAttrib.bMapType == 3 && masterRoleInfo != null && (int)CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade).bLogicGrade >= CMatchingView.ShowDefaultHeadImgStartLogicLadderLevel && !roomInfo.roomAttrib.bWarmBattle) || roomInfo.roomAttrib.bMapType == 5)
				{
					if (masterRoleInfo.playerUllUID == confirmPlayerUid)
					{
						CUICommonSystem.SetObjActive(transform.Find(string.Format("icon{0}/ready", confirmPlayerNum)), true);
						CUICommonSystem.SetObjActive(transform.Find(string.Format("icon{0}/unready", confirmPlayerNum)), false);
					}
					else
					{
						CUITimerScript uITimerScript = CUICommonSystem.GetUITimerScript(transform.Find(string.Format("icon{0}", confirmPlayerNum)));
						if (uITimerScript != null)
						{
							uITimerScript.SetTotalTime((float)Random.Range(1, 4));
							uITimerScript.SetTimerEventId(enTimerEventType.TimeUp, enUIEventID.Matchingt_ShowConfirmHead);
							uITimerScript.enabled = true;
							uITimerScript.StartTimer();
						}
					}
				}
				else
				{
					int num = 1;
					for (COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1; cOM_PLAYERCAMP < COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT; cOM_PLAYERCAMP++)
					{
						ListView<MemberInfo> listView = roomInfo[cOM_PLAYERCAMP];
						for (int i = 0; i < listView.Count; i++)
						{
							MemberInfo memberInfo = listView[i];
							if (memberInfo.ullUid == confirmPlayerUid)
							{
								Transform transform2 = transform.Find(string.Format("icon{0}/ready", num));
								if (transform2)
								{
									transform2.gameObject.CustomSetActive(true);
								}
								Transform transform3 = transform.Find(string.Format("icon{0}/unready", num));
								if (transform3)
								{
									transform3.gameObject.CustomSetActive(false);
								}
								break;
							}
							num++;
						}
					}
				}
			}
			bool flag = roomInfo.roomAttrib.bPkAI == 2;
			int num2 = (!roomInfo.roomAttrib.bWarmBattle && flag) ? (currentMapPlayerNum / 2) : currentMapPlayerNum;
			Transform transform4 = root.transform.Find("Panel/Panel/TxtReadyNum");
			if (transform4)
			{
				Text component = transform4.GetComponent<Text>();
				if (component)
				{
					component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Matching_Confirm_PlayerNum"), confirmPlayerNum, num2));
				}
			}
			if (confirmPlayerUid == roomInfo.selfInfo.ullUid)
			{
				Transform transform5 = root.transform.Find("Panel/Panel/btnGroup/Button_Confirm");
				if (transform5 != null)
				{
					Button component2 = transform5.GetComponent<Button>();
					if (component2)
					{
						component2.set_interactable(false);
					}
				}
			}
		}
	}
}

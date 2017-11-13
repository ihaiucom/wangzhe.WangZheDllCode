using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CRoomView
	{
		public static void SetRoomData(GameObject root, RoomInfo data)
		{
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId);
			int num = (int)(pvpMapCommonInfo.bMaxAcntNum / 2);
			CRoomView.SetStartBtnStatus(root, data, (int)pvpMapCommonInfo.bMaxAcntNum);
			CRoomView.UpdateBtnStatus(root, data);
			if (data.fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
			{
				Transform transform = root.transform.Find("Btn_Back");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
			root.transform.Find("Panel_Main/MapInfo/txtMapName").gameObject.GetComponent<Text>().set_text(pvpMapCommonInfo.szName);
			root.transform.Find("Panel_Main/MapInfo/txtTeam").gameObject.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num)));
			MemberInfo masterMemberInfo = data.GetMasterMemberInfo();
			DebugHelper.Assert(masterMemberInfo != null);
			COM_ROOM_FROMTYPE fromType = data.fromType;
			for (int i = 1; i <= 5; i++)
			{
				GameObject gameObject = root.transform.Find(string.Format("Panel_Main/LeftPlayers/Left_Player{0}", i)).gameObject;
				MemberInfo memberInfo = data.GetMemberInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_1, i - 1);
				CRoomView.SetPlayerSlotData(gameObject, memberInfo, masterMemberInfo, COM_PLAYERCAMP.COM_PLAYERCAMP_1, i - 1, num >= i, fromType);
			}
			for (int j = 1; j <= 5; j++)
			{
				GameObject gameObject2 = root.transform.Find(string.Format("Panel_Main/RightPlayers/Right_Player{0}", j)).gameObject;
				MemberInfo memberInfo2 = data.GetMemberInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_2, j - 1);
				CRoomView.SetPlayerSlotData(gameObject2, memberInfo2, masterMemberInfo, COM_PLAYERCAMP.COM_PLAYERCAMP_2, j - 1, num >= j, fromType);
			}
			ResDT_LevelCommonInfo pvpMapCommonInfo2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId);
			CRoomView.SetComEnable(pvpMapCommonInfo2.stPickRuleInfo.bPickType != 3);
			if (fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
			{
				CRoomView.DontShowComEnable(false);
			}
		}

		public static void SetStartBtnStatus(GameObject root, RoomInfo data, int totalPlayer)
		{
			GameObject gameObject = root.transform.Find("Panel_Main/Btn_Start").gameObject;
			bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
			gameObject.CustomSetActive(isSelfRoomOwner);
			if (isSelfRoomOwner)
			{
				Button component = gameObject.GetComponent<Button>();
				int count = data[COM_PLAYERCAMP.COM_PLAYERCAMP_1].Count;
				int count2 = data[COM_PLAYERCAMP.COM_PLAYERCAMP_2].Count;
				bool interactable = count > 0 && count2 > 0;
				if (data.fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
				{
					interactable = (count + count2 == totalPlayer);
				}
				component.set_interactable(interactable);
			}
		}

		public static void UpdateBtnStatus(GameObject root, RoomInfo data)
		{
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(data.roomAttrib.bMapType, data.roomAttrib.dwMapId);
			int bMaxAcntNum = (int)pvpMapCommonInfo.bMaxAcntNum;
			bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
			GameObject gameObject = root.transform.Find("Panel_Main/bg1/LeftRobot").gameObject;
			GameObject gameObject2 = root.transform.Find("Panel_Main/bg2/RightRobot").gameObject;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (isSelfRoomOwner && masterRoleInfo != null)
			{
				MemberInfo memberInfo = data.GetMemberInfo(masterRoleInfo.playerUllUID);
				if (memberInfo != null)
				{
					COM_PLAYERCAMP cOM_PLAYERCAMP = (memberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_2 : COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					if (data.GetFreePos(memberInfo.camp, bMaxAcntNum) >= 0)
					{
						if (memberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
						{
							gameObject.CustomSetActive(true);
						}
						else
						{
							gameObject2.CustomSetActive(true);
						}
					}
					if (data.GetFreePos(cOM_PLAYERCAMP, bMaxAcntNum) >= 0)
					{
						if (cOM_PLAYERCAMP == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
						{
							gameObject.CustomSetActive(true);
						}
						else
						{
							gameObject2.CustomSetActive(true);
						}
					}
				}
			}
			CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
			component.m_onClickEventID = enUIEventID.Room_AddRobot;
			component.m_onClickEventParams.tag = 1;
			component = gameObject2.GetComponent<CUIEventScript>();
			component.m_onClickEventID = enUIEventID.Room_AddRobot;
			component.m_onClickEventParams.tag = 2;
		}

		private static void SetPlayerSlotData(GameObject item, MemberInfo memberInfo, MemberInfo masterMemberInfo, COM_PLAYERCAMP camp, int pos, bool bAvailable, COM_ROOM_FROMTYPE fromType)
		{
			if (bAvailable)
			{
				bool isSelfRoomOwner = Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner;
				if (memberInfo == null)
				{
					item.CustomSetActive(true);
					item.transform.Find("Occupied").gameObject.CustomSetActive(false);
					GameObject gameObject = item.transform.Find("BtnJoin").gameObject;
					CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
					if (fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
					{
						gameObject.CustomSetActive(false);
					}
					else
					{
						gameObject.CustomSetActive(true);
					}
					component.m_onClickEventID = enUIEventID.Room_ChangePos;
					component.m_onClickEventParams.tag = (int)camp;
					component.m_onClickEventParams.tag2 = pos;
					component.m_onClickEventParams.tag3 = 1;
				}
				else
				{
					item.CustomSetActive(true);
					item.transform.Find("Occupied").gameObject.CustomSetActive(true);
					item.transform.Find("BtnJoin").gameObject.CustomSetActive(false);
					PlayerUniqueID roomOwner = Singleton<CRoomSystem>.GetInstance().roomInfo.roomOwner;
					bool bActive = roomOwner.ullUid == memberInfo.ullUid;
					PlayerUniqueID selfInfo = Singleton<CRoomSystem>.GetInstance().roomInfo.selfInfo;
					bool flag = selfInfo.ullUid == memberInfo.ullUid;
					item.transform.Find("Occupied/imgOwner").gameObject.CustomSetActive(bActive);
					CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CInviteSystem.PATH_INVITE_FORM);
					bool flag2 = true;
					if (form != null)
					{
						CUIListScript component2 = form.GetWidget(7).GetComponent<CUIListScript>();
						flag2 = (component2.GetSelectedIndex() == 0);
					}
					string text = flag2 ? Singleton<CInviteSystem>.GetInstance().GetInviteFriendName(memberInfo.ullUid, (uint)memberInfo.iLogicWorldID) : Singleton<CInviteSystem>.GetInstance().GetInviteGuildMemberName(memberInfo.ullUid);
					item.transform.Find("Occupied/txtPlayerName").GetComponent<Text>().set_text(string.IsNullOrEmpty(text) ? memberInfo.MemberName : text);
					GameObject gameObject2 = Utility.FindChild(item, "Occupied/BtnSwap");
					Transform transform = item.transform.Find("Occupied/BtnAddFriend");
					if (flag)
					{
						if (!CSysDynamicBlock.bFriendBlocked)
						{
							CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
							if (masterRoleInfo != null)
							{
								item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<CUIHttpImageScript>().SetImageUrl(masterRoleInfo.HeadUrl);
							}
						}
						if (transform != null)
						{
							transform.gameObject.CustomSetActive(false);
						}
						gameObject2.CustomSetActive(false);
					}
					else if (memberInfo.RoomMemberType == 1u)
					{
						if (!string.IsNullOrEmpty(memberInfo.MemberHeadUrl))
						{
							if (!CSysDynamicBlock.bFriendBlocked)
							{
								item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(memberInfo.MemberHeadUrl));
							}
							else
							{
								CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
								if (form2 != null)
								{
									item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustPlayer_Dir + "Common_PlayerImg", form2, true, false, false, false);
								}
							}
						}
						if (masterMemberInfo.swapStatus == 0)
						{
							gameObject2.CustomSetActive(true);
						}
						else if (masterMemberInfo.swapStatus == 1)
						{
							gameObject2.CustomSetActive(false);
						}
						else if (masterMemberInfo.swapStatus == 2)
						{
							gameObject2.CustomSetActive(masterMemberInfo.swapUid != memberInfo.ullUid);
						}
						CUIEventScript component3 = gameObject2.GetComponent<CUIEventScript>();
						component3.m_onClickEventID = enUIEventID.Room_ChangePos;
						component3.m_onClickEventParams.tag = (int)camp;
						component3.m_onClickEventParams.tag2 = pos;
						component3.m_onClickEventParams.tag3 = 2;
						if (!CSysDynamicBlock.bFriendBlocked)
						{
							if (Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(memberInfo.ullUid, (uint)memberInfo.iLogicWorldID) == null)
							{
								if (transform != null)
								{
									transform.gameObject.CustomSetActive(true);
									CUIEventScript component4 = transform.GetComponent<CUIEventScript>();
									if (component4 != null)
									{
										component4.m_onClickEventParams.commonUInt64Param1 = memberInfo.ullUid;
										component4.m_onClickEventParams.commonUInt32Param1 = (uint)memberInfo.iLogicWorldID;
									}
								}
							}
							else if (transform != null)
							{
								transform.gameObject.CustomSetActive(false);
							}
						}
						else if (transform != null)
						{
							transform.gameObject.CustomSetActive(false);
						}
					}
					else if (memberInfo.RoomMemberType == 2u)
					{
						CUIFormScript form3 = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
						if (form3 != null)
						{
							item.transform.Find("Occupied/HeadBg/imgHead").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustPlayer_Dir + "Img_ComputerHead", form3, true, false, false, false);
						}
						gameObject2.CustomSetActive(false);
						if (transform != null)
						{
							transform.gameObject.CustomSetActive(false);
						}
					}
					item.transform.Find("Occupied/BtnKick").gameObject.CustomSetActive(isSelfRoomOwner && !flag);
					if (fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
					{
						item.transform.Find("Occupied/BtnKick").gameObject.CustomSetActive(false);
					}
				}
			}
			else
			{
				item.CustomSetActive(false);
			}
		}

		public static void SetChgEnable(bool enable)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			if (Singleton<CRoomSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			int num = 1;
			int num2 = 2;
			for (int i = num; i <= num2; i++)
			{
				COM_PLAYERCAMP cOM_PLAYERCAMP = (COM_PLAYERCAMP)i;
				for (int j = 0; j < 5; j++)
				{
					bool flag = masterMemberInfo.camp == cOM_PLAYERCAMP && (ulong)masterMemberInfo.dwPosOfCamp == (ulong)((long)j);
					bool flag2 = false;
					MemberInfo memberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo(cOM_PLAYERCAMP, j);
					if (memberInfo != null)
					{
						flag2 = (memberInfo.RoomMemberType == 2u);
					}
					string path = string.Format("Panel_Main/{0}{1}/Occupied/BtnSwap", (cOM_PLAYERCAMP == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "LeftPlayers/Left_Player" : "RightPlayers/Right_Player", j + 1);
					GameObject gameObject = Utility.FindChild(form.gameObject, path);
					if (gameObject == null)
					{
						return;
					}
					gameObject.CustomSetActive(!flag && !flag2 && enable);
				}
			}
		}

		public static void SetChgEnable(bool enable, COM_PLAYERCAMP camp, int pos)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			if (Singleton<CRoomSystem>.instance.roomInfo == null)
			{
				return;
			}
			string path = string.Format("Panel_Main/{0}{1}/Occupied/BtnSwap", (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "LeftPlayers/Left_Player" : "RightPlayers/Right_Player", pos + 1);
			GameObject gameObject = Utility.FindChild(form.gameObject, path);
			if (gameObject == null)
			{
				return;
			}
			gameObject.CustomSetActive(enable);
		}

		public static void SetSwapTimer(int totalSec, COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1, int pos = 0)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			int num = 1;
			int num2 = 2;
			for (int i = num; i <= num2; i++)
			{
				COM_PLAYERCAMP cOM_PLAYERCAMP = (COM_PLAYERCAMP)i;
				for (int j = 0; j < 5; j++)
				{
					string path = string.Format("Panel_Main/{0}{1}/Occupied/TimerSwap", (cOM_PLAYERCAMP == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "LeftPlayers/Left_Player" : "RightPlayers/Right_Player", j + 1);
					GameObject gameObject = Utility.FindChild(form.gameObject, path);
					if (camp == cOM_PLAYERCAMP && pos == j && totalSec > 0)
					{
						gameObject.CustomSetActive(true);
						CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
						component.SetTotalTime((float)totalSec);
						component.m_eventIDs[1] = enUIEventID.Room_ChangePos_TimeUp;
						component.ReStartTimer();
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public static void ShowSwapMsg(int totalSec, COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1, int pos = 0)
		{
			if (totalSec > 0)
			{
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CRoomSystem.PATH_ROOM_SWAP, false, true);
				if (cUIFormScript == null || cUIFormScript.gameObject == null)
				{
					return;
				}
				GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "SwapMessageBox");
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
				if (form == null || form.gameObject == null)
				{
					return;
				}
				GameObject gameObject2;
				if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					gameObject2 = Utility.FindChild(form.gameObject, string.Format("Panel_Main/LeftPlayers/Left_Player{0}", pos + 1));
				}
				else if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					gameObject2 = Utility.FindChild(form.gameObject, string.Format("Panel_Main/RightPlayers/Right_Player{0}", pos + 1));
				}
				else
				{
					gameObject2 = Utility.FindChild(form.gameObject, "Panel_Main/Observers");
				}
				if (gameObject2 == null)
				{
					return;
				}
				gameObject.CustomSetActive(true);
				Vector2 vector = CUIUtility.WorldToScreenPoint(form.GetCamera(), gameObject2.transform.position);
				Vector3 position = CUIUtility.ScreenToWorldPoint(cUIFormScript.GetCamera(), vector, gameObject.transform.position.z);
				gameObject.transform.position = position;
				vector = default(Vector2);
				vector = (gameObject.transform as RectTransform).anchoredPosition;
				vector.y += (float)((camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID) ? -20 : 28);
				(gameObject.transform as RectTransform).anchoredPosition = vector;
				CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
				component.SetTotalTime((float)totalSec);
				component.m_eventIDs[0] = enUIEventID.Room_ChangePos_Box_TimerChange;
				component.m_eventIDs[2] = enUIEventID.Room_ChangePos_Box_TimerChange;
				component.m_eventIDs[1] = enUIEventID.Room_ChangePos_TimeUp;
				component.m_eventParams[0].tag = pos;
				component.m_eventParams[0].tag2 = (int)camp;
				component.m_eventParams[2].tag = pos;
				component.m_eventParams[2].tag2 = (int)camp;
				component.ReStartTimer();
			}
			else
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_ROOM_SWAP);
			}
		}

		public static void UpdateSwapBox(COM_PLAYERCAMP camp, int pos)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM_SWAP);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.gameObject, "SwapMessageBox");
			CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "Content");
			componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Room_Change_Pos_Tip_3", new string[]
			{
				Singleton<CTextManager>.instance.GetText("RoomCamp_" + (int)camp),
				(pos + 1).ToString(),
				((int)component.GetCurrentTime()).ToString()
			}));
		}

		public static void ResetSwapView()
		{
			CRoomView.SetChgEnable(true);
			CRoomView.SetSwapTimer(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
			CRoomView.ShowSwapMsg(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
		}

		public static void SetComEnable(bool enable)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			Button componetInChild = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg1/LeftRobot");
			Button componetInChild2 = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg2/RightRobot");
			CUICommonSystem.SetButtonEnable(componetInChild, enable, enable, true);
			CUICommonSystem.SetButtonEnable(componetInChild2, enable, enable, true);
		}

		public static void DontShowComEnable(bool enable)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form == null || form.gameObject == null)
			{
				return;
			}
			Button componetInChild = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg1/LeftRobot");
			Button componetInChild2 = Utility.GetComponetInChild<Button>(form.gameObject, "Panel_Main/bg2/RightRobot");
			if (componetInChild)
			{
				componetInChild.gameObject.CustomSetActive(false);
			}
			if (componetInChild2)
			{
				componetInChild2.gameObject.CustomSetActive(false);
			}
		}
	}
}

using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CRoomSystem : Singleton<CRoomSystem>
	{
		public const int MAX_NUM_PER_TEAM = 5;

		public static string PATH_CREATE_ROOM = "UGUI/Form/System/PvP/Room/Form_CreateRoom.prefab";

		public static string PATH_ROOM = "UGUI/Form/System/PvP/Room/Form_Room.prefab";

		public static string PATH_ROOM_SWAP = "UGUI/Form/System/PvP/Room/Form_RoomSwapMessageBox.prefab";

		private static ulong NpcUlId = 1uL;

		private bool bInRoom;

		private ListView<RoomMapInfo> mapList;

		public RoomInfo roomInfo;

		private uint mapId;

		private byte mapType = 1;

		private uint MeleeMapId;

		public int m_roomType;

		public int RoomType
		{
			get
			{
				return this.m_roomType;
			}
		}

		public bool IsInRoom
		{
			get
			{
				return this.bInRoom;
			}
		}

		public bool IsSelfRoomOwner
		{
			get
			{
				return this.roomInfo.selfInfo.ullUid == this.roomInfo.roomOwner.ullUid && this.roomInfo.selfInfo.iGameEntity == this.roomInfo.roomOwner.iGameEntity;
			}
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_OpenCreateForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenCreateForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_CreateRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_CreateRoom));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_CloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_SelectMap, new CUIEventManager.OnUIEventHandler(this.OnRoom_SelectMap));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_OpenInvite, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenInvite));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_StartGame, new CUIEventManager.OnUIEventHandler(this.OnRoom_StartGame));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_AddRobot, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddRobot));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos, new CUIEventManager.OnUIEventHandler(this.OnRoom_ChangePos));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnRoom_KickPlayer));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_LeaveRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_LeaveRoom));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ShareRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_ShareFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Refuse, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosRefuse));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_ChangePos_Box_TimerChange, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosBoxTimerChange));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_On_Close, new CUIEventManager.OnUIEventHandler(this.OnRoomClose));
			CRoomObserve.RegisterEvents();
		}

		public override void UnInit()
		{
			this.roomInfo = null;
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_OpenCreateForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenCreateForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_CreateRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_CreateRoom));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRoom_CloseForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_SelectMap, new CUIEventManager.OnUIEventHandler(this.OnRoom_SelectMap));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_OpenInvite, new CUIEventManager.OnUIEventHandler(this.OnRoom_OpenInvite));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_StartGame, new CUIEventManager.OnUIEventHandler(this.OnRoom_StartGame));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_AddRobot, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddRobot));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos, new CUIEventManager.OnUIEventHandler(this.OnRoom_ChangePos));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnRoom_KickPlayer));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_LeaveRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_LeaveRoom));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnRoom_AddFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ShareRoom, new CUIEventManager.OnUIEventHandler(this.OnRoom_ShareFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosTimeUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Refuse, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosRefuse));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_ChangePos_Box_TimerChange, new CUIEventManager.OnUIEventHandler(this.OnRoomChangePosBoxTimerChange));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_On_Close, new CUIEventManager.OnUIEventHandler(this.OnRoomClose));
			CRoomObserve.UnRegisterEvents();
			base.UnInit();
		}

		public void Clear()
		{
			this.bInRoom = false;
			this.roomInfo = null;
			this.m_roomType = 0;
		}

		public void CloseRoom()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_CREATE_ROOM);
			Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_ROOM);
			Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
			Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
			CChatUT.LeaveRoom();
			this.bInRoom = false;
		}

		private void InitMaps(CUIFormScript rootFormScript)
		{
			this.mapList = new ListView<RoomMapInfo>();
			uint[] array = new uint[10];
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_1"), ref array[0]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_2"), ref array[1]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_3"), ref array[2]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_4"), ref array[3]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_5"), ref array[4]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_6"), ref array[5]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_7"), ref array[6]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_8"), ref array[7]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_9"), ref array[8]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Room_10"), ref array[9]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_MELEE"), ref this.MeleeMapId);
			uint[] array2 = new uint[10];
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_1"), ref array2[0]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_2"), ref array2[1]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_3"), ref array2[2]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_4"), ref array2[3]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_5"), ref array2[4]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_6"), ref array2[5]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_7"), ref array2[6]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_8"), ref array2[7]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_9"), ref array2[8]);
			uint.TryParse(Singleton<CTextManager>.instance.GetText("MapType_Room_10"), ref array2[9]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != 0u)
				{
					ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)array2[i], array[i]);
					if (pvpMapCommonInfo != null)
					{
						RoomMapInfo roomMapInfo = new RoomMapInfo();
						roomMapInfo.mapType = (byte)array2[i];
						roomMapInfo.mapID = array[i];
						this.mapList.Add(roomMapInfo);
					}
				}
			}
			GameObject gameObject = rootFormScript.transform.Find("Panel_Main/List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(this.mapList.Count);
			for (int j = 0; j < component.m_elementAmount; j++)
			{
				CUIListElementScript elemenet = component.GetElemenet(j);
				Image component2 = elemenet.transform.GetComponent<Image>();
				string prefabPath = CUIUtility.s_Sprite_Dynamic_PvpEntry_Dir + this.mapList[j].mapID;
				component2.SetSprite(prefabPath, rootFormScript, true, false, false, false);
				uint num = 0u;
				uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Miwu"), ref num);
				if (this.mapList[j].mapID == num)
				{
					elemenet.transform.FindChild("Flag").gameObject.CustomSetActive(true);
				}
			}
			component.SelectElement(-1, true);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = rootFormScript.transform.Find("panelGroupBottom");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnRoom_OpenCreateForm(CUIEvent uiEvent)
		{
			if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_ROOM))
			{
				Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CRoomSystem.PATH_CREATE_ROOM, false, true);
			this.InitMaps(cUIFormScript);
			this.ShowBonusImage(cUIFormScript);
			this.entertainmentAddLock(cUIFormScript);
		}

		private void OnRoom_CreateRoom(CUIEvent uiEvent)
		{
			if (this.mapId > 0u)
			{
				Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
				Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapType = this.mapType;
				Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId = this.mapId;
				CRoomSystem.ReqCreateRoom(this.mapId, this.mapType, false);
			}
		}

		private void OnRoom_CloseForm(CUIEvent uiEvent)
		{
			CRoomSystem.ReqLeaveRoom();
		}

		private void OnRoom_SelectMap(CUIEvent uiEvent)
		{
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			if (selectedIndex < 0 || selectedIndex >= this.mapList.Count)
			{
				return;
			}
			RoomMapInfo roomMapInfo = this.mapList[selectedIndex];
			this.mapId = roomMapInfo.mapID;
			this.mapType = roomMapInfo.mapType;
			if (this.mapId == this.MeleeMapId && !Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(25u);
				Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
				return;
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Room_CreateRoom);
		}

		private void OnRoom_OpenInvite(CUIEvent uiEvent)
		{
		}

		private void OnRoom_StartGame(CUIEvent uiEvent)
		{
			if (this.IsSelfRoomOwner)
			{
				Button component = uiEvent.m_srcWidget.GetComponent<Button>();
				if (component.get_interactable())
				{
					CRoomSystem.ReqStartGame();
				}
			}
			else
			{
				DebugHelper.Assert(false);
			}
		}

		private void OnRoom_AddRobot(CUIEvent uiEvent)
		{
			if (this.IsSelfRoomOwner)
			{
				COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
				if (uiEvent.m_eventParams.tag == 1)
				{
					camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				}
				else if (uiEvent.m_eventParams.tag == 2)
				{
					camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
				}
				CRoomSystem.ReqAddRobot(camp);
			}
			else
			{
				DebugHelper.Assert(false);
			}
		}

		private void OnRoom_ChangePos(CUIEvent uiEvent)
		{
			COM_PLAYERCAMP tag = (COM_PLAYERCAMP)uiEvent.m_eventParams.tag;
			int tag2 = uiEvent.m_eventParams.tag2;
			COM_CHGROOMPOS_TYPE tag3 = (COM_CHGROOMPOS_TYPE)uiEvent.m_eventParams.tag3;
			CRoomSystem.ReqChangeCamp(tag, tag2, tag3);
		}

		private void OnRoom_KickPlayer(CUIEvent uiEvent)
		{
			if (this.IsSelfRoomOwner)
			{
				GameObject gameObject = uiEvent.m_srcWidget.transform.parent.parent.gameObject;
				COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				int pos = 0;
				this.GetMemberPosInfo(gameObject, out camp, out pos);
				CRoomSystem.ReqKickPlayer(camp, pos);
			}
			else
			{
				DebugHelper.Assert(false, "Not Room Owner!");
			}
		}

		private void OnRoom_LeaveRoom(CUIEvent uiEvent)
		{
			CRoomSystem.ReqLeaveRoom();
		}

		private void OnRoom_AddFriend(CUIEvent uiEvent)
		{
			GameObject gameObject = uiEvent.m_srcWidget.transform.parent.parent.gameObject;
			COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			int num = 0;
			this.GetMemberPosInfo(gameObject, out cOM_PLAYERCAMP, out num);
			if (this.roomInfo != null)
			{
				MemberInfo memberInfo = this.roomInfo.GetMemberInfo(cOM_PLAYERCAMP, num);
				DebugHelper.Assert(memberInfo != null, "Room member info is NULL!! Camp -- {0}, Pos -- {1}", new object[]
				{
					cOM_PLAYERCAMP,
					num
				});
				if (memberInfo != null)
				{
					Singleton<CFriendContoller>.instance.Open_Friend_Verify(memberInfo.ullUid, (uint)memberInfo.iFromGameEntity, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
				}
			}
		}

		private void OnRoom_ShareFriend(CUIEvent uiEvent)
		{
			if (Singleton<CRoomSystem>.GetInstance().IsInRoom)
			{
				this.OnRoom_ShareFriend_Room(uiEvent);
			}
			else if (Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam)
			{
				Singleton<CMatchingSystem>.GetInstance().OnTeam_ShareFriend_Team(uiEvent);
			}
		}

		private void OnRoomChangePosTimeUp(CUIEvent uiEvent)
		{
			this.RestMasterSwapInfo();
		}

		private void RestMasterSwapInfo()
		{
			CRoomView.ResetSwapView();
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			masterMemberInfo.swapSeq = 0u;
			masterMemberInfo.swapStatus = 0;
			masterMemberInfo.swapUid = 0uL;
		}

		private void OnRoomChangePosConfirm(CUIEvent uiEvent)
		{
			if (Singleton<CRoomSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2037u);
			cSPkg.stPkgData.stChgRoomPosConfirmReq.bIsAccept = 1;
			cSPkg.stPkgData.stChgRoomPosConfirmReq.dwChgSeq = masterMemberInfo.swapSeq;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
		}

		private void OnRoomChangePosRefuse(CUIEvent uiEvent)
		{
			if (Singleton<CRoomSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2037u);
			cSPkg.stPkgData.stChgRoomPosConfirmReq.bIsAccept = 0;
			cSPkg.stPkgData.stChgRoomPosConfirmReq.dwChgSeq = masterMemberInfo.swapSeq;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
		}

		private void OnRoomChangePosBoxTimerChange(CUIEvent uiEvent)
		{
			CRoomView.UpdateSwapBox((COM_PLAYERCAMP)uiEvent.m_eventParams.tag2, uiEvent.m_eventParams.tag);
		}

		private void OnRoomClose(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CRoomSystem.PATH_ROOM_SWAP);
		}

		private void OnRoom_ShareFriend_Room(CUIEvent uiEvent)
		{
			if (this.roomInfo == null || this.roomInfo.roomAttrib == null)
			{
				return;
			}
			uint dwMapId = this.roomInfo.roomAttrib.dwMapId;
			int bMapType = (int)this.roomInfo.roomAttrib.bMapType;
			string text = string.Empty;
			string text2 = string.Empty;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte)bMapType, dwMapId);
			int num = (int)pvpMapCommonInfo.bMaxAcntNum;
			text = pvpMapCommonInfo.szName;
			if (bMapType == 3)
			{
				num = CLadderSystem.MultiLadderMaxTeamerNum() * 2;
				if (num / 2 == 2)
				{
					text2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6");
				}
				else
				{
					text2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_8");
				}
			}
			else
			{
				text2 = Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num / 2));
			}
			string text3 = Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_Title");
			string text4 = Singleton<CTextManager>.instance.GetText("Share_Room_Info_Desc", new string[]
			{
				text2,
				text
			});
			string text5 = MonoSingleton<ShareSys>.GetInstance().PackRoomData(this.roomInfo.iRoomEntity, this.roomInfo.dwRoomID, this.roomInfo.dwRoomSeq, this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId, this.roomInfo.roomAttrib.ullFeature);
			Singleton<ApolloHelper>.GetInstance().InviteFriendToRoom(text3, text4, text5);
		}

		private void GetMemberPosInfo(GameObject go, out COM_PLAYERCAMP Camp, out int Pos)
		{
			Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			if (go.name.StartsWith("Left"))
			{
				Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			else if (go.name.StartsWith("Right"))
			{
				Camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			Pos = 0;
			if (go.name.EndsWith("1"))
			{
				Pos = 0;
			}
			else if (go.name.EndsWith("2"))
			{
				Pos = 1;
			}
			else if (go.name.EndsWith("3"))
			{
				Pos = 2;
			}
			else if (go.name.EndsWith("4"))
			{
				Pos = 3;
			}
			else if (go.name.EndsWith("5"))
			{
				Pos = 4;
			}
		}

		private void ShowBonusImage(CUIFormScript form)
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

		private void entertainmentAddLock(CUIFormScript form)
		{
			CUIListScript component = form.transform.FindChild("Panel_Main/List").GetComponent<CUIListScript>();
			for (int i = 0; i < this.mapList.Count; i++)
			{
				if (this.mapList[i] == null)
				{
					break;
				}
				if (this.mapList[i].mapID == this.MeleeMapId)
				{
					Transform transform = component.GetElemenet(i).transform;
					if (transform != null)
					{
						if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
						{
							transform.GetComponent<Image>().set_color(CUIUtility.s_Color_Button_Disable);
							transform.FindChild("Lock").gameObject.CustomSetActive(true);
							ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(25u);
							transform.FindChild("Lock/Text").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szLockedTip));
						}
						else
						{
							transform.GetComponent<Image>().set_color(CUIUtility.s_Color_White);
							transform.FindChild("Lock").gameObject.CustomSetActive(false);
						}
					}
					break;
				}
			}
		}

		public void SetRoomType(int roomType)
		{
			this.m_roomType = roomType;
		}

		private MemberInfo CreateMemInfo(ref COMDT_ROOMMEMBER_DT memberDT, COM_PLAYERCAMP camp, bool bWarmBattle)
		{
			MemberInfo memberInfo = new MemberInfo();
			memberInfo.RoomMemberType = memberDT.dwRoomMemberType;
			memberInfo.dwPosOfCamp = memberDT.dwPosOfCamp;
			memberInfo.camp = camp;
			if (memberDT.dwRoomMemberType == 1u)
			{
				memberInfo.ullUid = memberDT.stMemberDetail.stMemberOfAcnt.ullUid;
				memberInfo.iFromGameEntity = memberDT.stMemberDetail.stMemberOfAcnt.iFromGameEntity;
				memberInfo.iLogicWorldID = memberDT.stMemberDetail.stMemberOfAcnt.iLogicWorldID;
				memberInfo.MemberName = StringHelper.UTF8BytesToString(ref memberDT.stMemberDetail.stMemberOfAcnt.szMemberName);
				memberInfo.dwMemberLevel = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberLevel;
				memberInfo.dwMemberPvpLevel = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberPvpLevel;
				memberInfo.dwMemberHeadId = memberDT.stMemberDetail.stMemberOfAcnt.dwMemberHeadId;
				memberInfo.MemberHeadUrl = StringHelper.UTF8BytesToString(ref memberDT.stMemberDetail.stMemberOfAcnt.szMemberHeadUrl);
				memberInfo.ChoiceHero = new COMDT_CHOICEHERO[1];
				memberInfo.ChoiceHero[0] = new COMDT_CHOICEHERO();
				memberInfo.recentUsedHero = new COMDT_RECENT_USED_HERO();
				memberInfo.canUseHero = new uint[0];
				memberInfo.isPrepare = false;
				memberInfo.dwObjId = 0u;
			}
			else if (memberDT.dwRoomMemberType == 2u)
			{
				MemberInfo memberInfo2 = memberInfo;
				ulong npcUlId = CRoomSystem.NpcUlId;
				CRoomSystem.NpcUlId = npcUlId + 1uL;
				memberInfo2.ullUid = npcUlId;
				memberInfo.iFromGameEntity = 0;
				memberInfo.MemberName = Singleton<CTextManager>.GetInstance().GetText("PVP_NPC");
				memberInfo.dwMemberLevel = (uint)memberDT.stMemberDetail.stMemberOfNpc.bLevel;
				memberInfo.dwMemberHeadId = 1u;
				memberInfo.ChoiceHero = new COMDT_CHOICEHERO[1];
				memberInfo.ChoiceHero[0] = new COMDT_CHOICEHERO();
				memberInfo.recentUsedHero = new COMDT_RECENT_USED_HERO();
				memberInfo.canUseHero = new uint[0];
				memberInfo.isPrepare = true;
				memberInfo.dwObjId = 0u;
				memberInfo.WarmNpc = memberDT.stMemberDetail.stMemberOfNpc.stDetail;
				if (bWarmBattle)
				{
					memberInfo.ullUid = memberInfo.WarmNpc.ullUid;
					memberInfo.dwMemberPvpLevel = memberInfo.WarmNpc.dwAcntPvpLevel;
					memberInfo.MemberName = StringHelper.UTF8BytesToString(ref memberInfo.WarmNpc.szUserName);
					memberInfo.MemberHeadUrl = StringHelper.UTF8BytesToString(ref memberInfo.WarmNpc.szUserHeadUrl);
					memberInfo.isPrepare = false;
				}
			}
			return memberInfo;
		}

		public void BuildRoomInfo(COMDT_MATCH_SUCC_DETAIL roomData)
		{
			this.roomInfo = new RoomInfo();
			this.roomInfo.iRoomEntity = roomData.iRoomEntity;
			this.roomInfo.dwRoomID = roomData.dwRoomID;
			this.roomInfo.dwRoomSeq = roomData.dwRoomSeq;
			this.roomInfo.roomAttrib.bGameMode = roomData.stRoomInfo.bGameMode;
			this.roomInfo.roomAttrib.bPkAI = roomData.stRoomInfo.bPkAI;
			this.roomInfo.roomAttrib.bMapType = roomData.stRoomInfo.bMapType;
			this.roomInfo.roomAttrib.dwMapId = roomData.stRoomInfo.dwMapId;
			this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(roomData.stRoomInfo.bIsWarmBattle);
			this.roomInfo.roomAttrib.npcAILevel = roomData.stRoomInfo.bAILevel;
			this.roomInfo.roomAttrib.ullFeature = roomData.stRoomInfo.ullFeature;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
			this.roomInfo.roomAttrib.judgeNum = (int)((pvpMapCommonInfo != null) ? pvpMapCommonInfo.dwJudgeNum : 0u);
			this.roomInfo.selfInfo.ullUid = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
			for (int i = 0; i < 3; i++)
			{
				COM_PLAYERCAMP camp = (COM_PLAYERCAMP)i;
				ListView<MemberInfo> listView = this.roomInfo[camp];
				listView.Clear();
				int num = 0;
				while ((long)num < (long)((ulong)roomData.stMemInfo.astCampMem[i].dwMemNum))
				{
					COMDT_ROOMMEMBER_DT cOMDT_ROOMMEMBER_DT = roomData.stMemInfo.astCampMem[i].astMemInfo[num];
					MemberInfo item = this.CreateMemInfo(ref cOMDT_ROOMMEMBER_DT, camp, this.roomInfo.roomAttrib.bWarmBattle);
					listView.Add(item);
					num++;
				}
				this.roomInfo.SortCampMemList(camp);
			}
		}

		public void BuildRoomInfo(COMDT_JOINMULTGAMERSP_SUCC roomData)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			this.roomInfo = new RoomInfo();
			this.roomInfo.fromType = (COM_ROOM_FROMTYPE)roomData.bExtraType;
			this.roomInfo.iRoomEntity = roomData.iRoomEntity;
			this.roomInfo.dwRoomID = roomData.dwRoomID;
			this.roomInfo.dwRoomSeq = roomData.dwRoomSeq;
			this.roomInfo.roomAttrib.bGameMode = roomData.stRoomInfo.bGameMode;
			this.roomInfo.roomAttrib.bPkAI = 0;
			this.roomInfo.roomAttrib.bMapType = roomData.stRoomInfo.bMapType;
			this.roomInfo.roomAttrib.dwMapId = roomData.stRoomInfo.dwMapId;
			this.roomInfo.roomAttrib.ullFeature = roomData.stRoomInfo.ullFeature;
			this.roomInfo.roomAttrib.bWarmBattle = false;
			this.roomInfo.roomAttrib.npcAILevel = 2;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
			this.roomInfo.roomAttrib.judgeNum = (int)((pvpMapCommonInfo != null) ? pvpMapCommonInfo.dwJudgeNum : 0u);
			this.roomInfo.selfInfo.ullUid = roomData.ullSelfUid;
			this.roomInfo.selfInfo.iGameEntity = roomData.iSelfGameEntity;
			this.roomInfo.roomOwner.ullUid = roomData.stRoomMaster.ullMasterUid;
			this.roomInfo.roomOwner.iGameEntity = roomData.stRoomMaster.iMasterGameEntity;
			for (int i = 0; i < 3; i++)
			{
				COM_PLAYERCAMP camp = (COM_PLAYERCAMP)i;
				ListView<MemberInfo> listView = this.roomInfo[camp];
				listView.Clear();
				int num = 0;
				while ((long)num < (long)((ulong)roomData.stMemInfo.astCampMem[i].dwMemNum))
				{
					COMDT_ROOMMEMBER_DT cOMDT_ROOMMEMBER_DT = roomData.stMemInfo.astCampMem[i].astMemInfo[num];
					MemberInfo memberInfo = this.CreateMemInfo(ref cOMDT_ROOMMEMBER_DT, camp, this.roomInfo.roomAttrib.bWarmBattle);
					if (memberInfo.ullUid == masterRoleInfo.playerUllUID)
					{
						this.roomInfo.selfObjID = memberInfo.dwObjId;
					}
					listView.Add(memberInfo);
					num++;
				}
				this.roomInfo.SortCampMemList(camp);
			}
		}

		public void UpdateRoomInfoReconnectPick(COMDT_DESKINFO inDeskInfo, CSDT_RECONN_CAMPPICKINFO[] inCampInfo)
		{
			this.roomInfo = new RoomInfo();
			this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
			this.roomInfo.roomAttrib.bPkAI = 0;
			this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
			this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
			this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(inDeskInfo.bIsWarmBattle);
			this.roomInfo.roomAttrib.npcAILevel = inDeskInfo.bAILevel;
			ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
			this.roomInfo.roomAttrib.judgeNum = (int)((pvpMapCommonInfo != null) ? pvpMapCommonInfo.dwJudgeNum : 0u);
			for (int i = 0; i < inCampInfo.Length; i++)
			{
				COM_PLAYERCAMP camp = i + COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				CSDT_RECONN_CAMPPICKINFO cSDT_RECONN_CAMPPICKINFO = inCampInfo[i];
				ListView<MemberInfo> listView = this.roomInfo[camp];
				listView.Clear();
				int num = 0;
				while ((long)num < (long)((ulong)cSDT_RECONN_CAMPPICKINFO.dwPlayerNum))
				{
					MemberInfo memberInfo = new MemberInfo();
					COMDT_PLAYERINFO stPlayerInfo = cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].stPickHeroInfo.stPlayerInfo;
					COMDT_ACNT_USABLE_HERO stUsableHero = cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].stPickHeroInfo.stUsableHero;
					memberInfo.isPrepare = (cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].bIsPickOK > 0);
					memberInfo.RoomMemberType = (uint)stPlayerInfo.bObjType;
					memberInfo.dwPosOfCamp = (uint)stPlayerInfo.bPosOfCamp;
					memberInfo.camp = camp;
					memberInfo.dwMemberLevel = stPlayerInfo.dwLevel;
					if (memberInfo.RoomMemberType == 1u)
					{
						memberInfo.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
						memberInfo.dwMemberPvpLevel = stPlayerInfo.stDetail.stPlayerOfAcnt.dwPvpLevel;
					}
					memberInfo.dwObjId = stPlayerInfo.dwObjId;
					memberInfo.MemberName = StringHelper.UTF8BytesToString(ref stPlayerInfo.szName);
					memberInfo.ChoiceHero = stPlayerInfo.astChoiceHero;
					memberInfo.canUseHero = stUsableHero.HeroDetail;
					memberInfo.recentUsedHero = cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].stPickHeroInfo.stRecentUsedHero;
					memberInfo.isGM = (cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].stPickHeroInfo.bIsGM > 0);
					if (stPlayerInfo.bObjType == 1)
					{
						memberInfo.dwMemberHeadId = stPlayerInfo.stDetail.stPlayerOfAcnt.dwHeadId;
						if (stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
						{
							this.roomInfo.selfObjID = stPlayerInfo.dwObjId;
							Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int)cSDT_RECONN_CAMPPICKINFO.astPlayerInfo[num].stPickHeroInfo.dwRandomHeroCnt);
						}
						memberInfo.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
					}
					else if (stPlayerInfo.bObjType == 2)
					{
						memberInfo.dwMemberHeadId = 1u;
						memberInfo.ullUid = stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
					}
					listView.Add(memberInfo);
					num++;
				}
				this.roomInfo.SortCampMemList(camp);
			}
		}

		public void UpdateRoomInfo(COMDT_DESKINFO inDeskInfo, CSDT_CAMPINFO[] inCampInfo)
		{
			uint selfObjID = 0u;
			if (this.roomInfo == null)
			{
				this.roomInfo = new RoomInfo();
				this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
				this.roomInfo.roomAttrib.bPkAI = 0;
				this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
				this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
				this.roomInfo.roomAttrib.bWarmBattle = Convert.ToBoolean(inDeskInfo.bIsWarmBattle);
				this.roomInfo.roomAttrib.npcAILevel = inDeskInfo.bAILevel;
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.roomInfo.roomAttrib.bMapType, this.roomInfo.roomAttrib.dwMapId);
				this.roomInfo.roomAttrib.judgeNum = (int)((pvpMapCommonInfo != null) ? pvpMapCommonInfo.dwJudgeNum : 0u);
				for (int i = 0; i < inCampInfo.Length; i++)
				{
					COM_PLAYERCAMP camp = i + COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					CSDT_CAMPINFO cSDT_CAMPINFO = inCampInfo[i];
					ListView<MemberInfo> listView = this.roomInfo[camp];
					listView.Clear();
					int num = 0;
					while ((long)num < (long)((ulong)cSDT_CAMPINFO.dwPlayerNum))
					{
						MemberInfo memberInfo = new MemberInfo();
						COMDT_PLAYERINFO stPlayerInfo = cSDT_CAMPINFO.astCampPlayerInfo[num].stPlayerInfo;
						COMDT_ACNT_USABLE_HERO stUsableHero = cSDT_CAMPINFO.astCampPlayerInfo[num].stUsableHero;
						memberInfo.RoomMemberType = (uint)stPlayerInfo.bObjType;
						memberInfo.dwPosOfCamp = (uint)stPlayerInfo.bPosOfCamp;
						memberInfo.camp = camp;
						memberInfo.dwMemberLevel = stPlayerInfo.dwLevel;
						if (memberInfo.RoomMemberType == 1u)
						{
							memberInfo.dwMemberPvpLevel = stPlayerInfo.stDetail.stPlayerOfAcnt.dwPvpLevel;
						}
						memberInfo.dwObjId = stPlayerInfo.dwObjId;
						memberInfo.MemberName = StringHelper.UTF8BytesToString(ref stPlayerInfo.szName);
						memberInfo.ChoiceHero = stPlayerInfo.astChoiceHero;
						memberInfo.canUseHero = stUsableHero.HeroDetail;
						memberInfo.recentUsedHero = cSDT_CAMPINFO.astCampPlayerInfo[num].stRecentUsedHero;
						if (stPlayerInfo.bObjType == 1)
						{
							memberInfo.dwMemberHeadId = stPlayerInfo.stDetail.stPlayerOfAcnt.dwHeadId;
							if (stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
							{
								selfObjID = stPlayerInfo.dwObjId;
								Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int)cSDT_CAMPINFO.astCampPlayerInfo[num].dwRandomHeroCnt);
							}
							memberInfo.ullUid = stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
						}
						else if (stPlayerInfo.bObjType == 2)
						{
							memberInfo.dwMemberHeadId = 1u;
							memberInfo.ullUid = stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
						}
						listView.Add(memberInfo);
						num++;
					}
					this.roomInfo.SortCampMemList(camp);
				}
			}
			else
			{
				this.roomInfo.roomAttrib.bGameMode = inDeskInfo.bGameMode;
				this.roomInfo.roomAttrib.bPkAI = 0;
				this.roomInfo.roomAttrib.bMapType = inDeskInfo.bMapType;
				this.roomInfo.roomAttrib.dwMapId = inDeskInfo.dwMapId;
				for (int j = 0; j < inCampInfo.Length; j++)
				{
					COM_PLAYERCAMP camp2 = j + COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					CSDT_CAMPINFO cSDT_CAMPINFO2 = inCampInfo[j];
					ListView<MemberInfo> listView2 = this.roomInfo[camp2];
					int num2 = 0;
					while ((long)num2 < (long)((ulong)cSDT_CAMPINFO2.dwPlayerNum))
					{
						COMDT_PLAYERINFO stPlayerInfo2 = cSDT_CAMPINFO2.astCampPlayerInfo[num2].stPlayerInfo;
						COMDT_ACNT_USABLE_HERO stUsableHero2 = cSDT_CAMPINFO2.astCampPlayerInfo[num2].stUsableHero;
						MemberInfo memberInfo2 = this.roomInfo.GetMemberInfo(camp2, (int)stPlayerInfo2.bPosOfCamp);
						if (memberInfo2 != null)
						{
							memberInfo2.dwObjId = stPlayerInfo2.dwObjId;
							memberInfo2.camp = camp2;
							memberInfo2.ChoiceHero = stPlayerInfo2.astChoiceHero;
							memberInfo2.canUseHero = stUsableHero2.HeroDetail;
							memberInfo2.recentUsedHero = cSDT_CAMPINFO2.astCampPlayerInfo[num2].stRecentUsedHero;
							if (stPlayerInfo2.bObjType == 1)
							{
								memberInfo2.dwMemberHeadId = stPlayerInfo2.stDetail.stPlayerOfAcnt.dwHeadId;
								if (stPlayerInfo2.stDetail.stPlayerOfAcnt.ullUid == Singleton<CRoleInfoManager>.instance.masterUUID)
								{
									selfObjID = stPlayerInfo2.dwObjId;
									Singleton<CHeroSelectBaseSystem>.instance.ResetRandHeroLeftCount((int)cSDT_CAMPINFO2.astCampPlayerInfo[num2].dwRandomHeroCnt);
								}
								memberInfo2.ullUid = stPlayerInfo2.stDetail.stPlayerOfAcnt.ullUid;
							}
							else if (stPlayerInfo2.bObjType == 2)
							{
								memberInfo2.dwMemberHeadId = 1u;
								memberInfo2.ullUid = stPlayerInfo2.stDetail.stPlayerOfNpc.ullFakeUid;
							}
						}
						num2++;
					}
					this.roomInfo.SortCampMemList(camp2);
				}
			}
			this.roomInfo.selfObjID = selfObjID;
		}

		public static void ReqCreateRoom(uint MapId, byte mapType, bool isInviteFriendImmediately = false)
		{
			CInviteSystem.s_isInviteFriendImmidiately = isInviteFriendImmediately;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1020u);
			StringHelper.StringToUTF8Bytes("testRoom", ref cSPkg.stPkgData.stCreateMultGameReq.szRoomName);
			cSPkg.stPkgData.stCreateMultGameReq.bMapType = mapType;
			cSPkg.stPkgData.stCreateMultGameReq.dwMapId = MapId;
			cSPkg.stPkgData.stCreateMultGameReq.bGameMode = 1;
			cSPkg.stPkgData.stCreateMultGameReq.bExtraType = 0;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqCreateRoomfromAPP(uint MapId, byte mapType, COMDT_QQSPROT_EXTRA qqextra, bool isInviteFriendImmediately = false)
		{
			CInviteSystem.s_isInviteFriendImmidiately = isInviteFriendImmediately;
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1020u);
			StringHelper.StringToUTF8Bytes("testRoom", ref cSPkg.stPkgData.stCreateMultGameReq.szRoomName);
			cSPkg.stPkgData.stCreateMultGameReq.bMapType = mapType;
			cSPkg.stPkgData.stCreateMultGameReq.dwMapId = MapId;
			cSPkg.stPkgData.stCreateMultGameReq.bGameMode = 1;
			cSPkg.stPkgData.stCreateMultGameReq.bExtraType = 1;
			cSPkg.stPkgData.stCreateMultGameReq.stExtraInfo.stQQSprotExtra = qqextra;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqCreateRoomAndInvite(uint mapId, COM_BATTLE_MAP_TYPE mapType, CInviteSystem.stInviteInfo inviteInfo)
		{
			Singleton<CInviteSystem>.GetInstance().InviteInfo = inviteInfo;
			CRoomSystem.ReqCreateRoom(mapId, (byte)mapType, true);
		}

		public static void ReqLeaveRoom()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1023u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqAddRobot(COM_PLAYERCAMP Camp)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2015u);
			cSPkg.stPkgData.stAddNpcReq.stNpcInfo.iCamp = (int)Camp;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqKickPlayer(COM_PLAYERCAMP Camp, int Pos)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2019u);
			cSPkg.stPkgData.stKickoutRoomMemberReq.stKickMemberInfo.iCamp = (int)Camp;
			cSPkg.stPkgData.stKickoutRoomMemberReq.stKickMemberInfo.bPosOfCamp = (byte)Pos;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqChangeCamp(COM_PLAYERCAMP Camp, int Pos, COM_CHGROOMPOS_TYPE type)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2033u);
			cSPkg.stPkgData.stChgMemberPosReq.bCamp = (byte)Camp;
			cSPkg.stPkgData.stChgMemberPosReq.bPosOfCamp = (byte)Pos;
			cSPkg.stPkgData.stChgMemberPosReq.bChgType = (byte)type;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		public static void ReqStartGame()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2013u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1022)]
		public static void OnPlayerJoinRoom(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stJoinMultGameRsp.iErrCode == 0)
			{
				Singleton<GameBuilder>.instance.EndGame();
				CRoomSystem instance = Singleton<CRoomSystem>.GetInstance();
				instance.bInRoom = true;
				instance.BuildRoomInfo(msg.stPkgData.stJoinMultGameRsp.stInfo.stOfSucc);
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CRoomSystem.PATH_ROOM, false, true);
				Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
				bool bNotShowInViteBtn = false;
				if (instance.roomInfo.fromType == COM_ROOM_FROMTYPE.COM_ROOM_FROM_QQSPROT)
				{
					bNotShowInViteBtn = true;
				}
				Singleton<CInviteSystem>.GetInstance().OpenInviteForm(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM, bNotShowInViteBtn);
				CChatUT.EnterRoom();
				CRoomView.SetRoomData(cUIFormScript.gameObject, instance.roomInfo);
				CRoomObserve.SetObservers(Utility.FindChild(cUIFormScript.gameObject, "Panel_Main/Observers"), instance.roomInfo.roomAttrib.judgeNum, instance.roomInfo[COM_PLAYERCAMP.COM_PLAYERCAMP_MID], instance.roomInfo.GetMasterMemberInfo());
				Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
				Singleton<CMatchingSystem>.instance.cacheMathingInfo.CanGameAgain = instance.IsSelfRoomOwner;
				if (!instance.IsSelfRoomOwner)
				{
					MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
				}
				if (MonoSingleton<ShareSys>.instance.IsQQGameTeamCreate())
				{
					string roomStr = MonoSingleton<ShareSys>.instance.PackQQGameTeamData(instance.roomInfo.iRoomEntity, instance.roomInfo.dwRoomID, instance.roomInfo.dwRoomSeq, instance.roomInfo.roomAttrib.ullFeature);
					MonoSingleton<ShareSys>.instance.SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.join, COM_ROOM_TYPE.COM_ROOM_TYPE_NORMAL, instance.roomInfo.roomAttrib.bMapType, instance.roomInfo.roomAttrib.dwMapId, roomStr, 0u, 0u);
				}
				CMatchingSystem.CloseExcludeForm();
			}
			else if (msg.stPkgData.stJoinMultGameRsp.iErrCode == 26)
			{
				DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
				string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", new object[]
				{
					banTime.get_Year(),
					banTime.get_Month(),
					banTime.get_Day(),
					banTime.get_Hour(),
					banTime.get_Minute()
				});
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Enter_Room_Error", true, 1f, null, new object[]
				{
					Utility.ProtErrCodeToStr(1022, msg.stPkgData.stJoinMultGameRsp.iErrCode)
				});
			}
		}

		[MessageHandler(2038)]
		public static void OnRoomChgPosNtf(CSPkg msg)
		{
			COM_CHGROOMPOS_RESULT bResult = (COM_CHGROOMPOS_RESULT)msg.stPkgData.stChgRoomPosNtf.bResult;
			COMDT_ROOMCHGPOS_DATA stChgPosData = msg.stPkgData.stChgRoomPosNtf.stChgPosData;
			if (Singleton<CRoomSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			switch (bResult)
			{
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_BEGIN:
				if (RoomInfo.IsSameMemeber(masterMemberInfo, (COM_PLAYERCAMP)stChgPosData.stMemberInfo.stSender.bCamp, (int)stChgPosData.stMemberInfo.stSender.bPosOfCamp))
				{
					MemberInfo memberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP)stChgPosData.stMemberInfo.stReceiver.bCamp, (int)stChgPosData.stMemberInfo.stReceiver.bPosOfCamp);
					if (memberInfo == null)
					{
						return;
					}
					CRoomView.SetChgEnable(false);
					CRoomView.SetSwapTimer((int)stChgPosData.stMemberInfo.dwTimeOutSec, (COM_PLAYERCAMP)stChgPosData.stMemberInfo.stReceiver.bCamp, (int)stChgPosData.stMemberInfo.stReceiver.bPosOfCamp);
					CRoomView.ShowSwapMsg(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
					masterMemberInfo.swapSeq = stChgPosData.stMemberInfo.dwChgPosSeq;
					masterMemberInfo.swapStatus = 1;
					masterMemberInfo.swapUid = memberInfo.ullUid;
					Singleton<CUIManager>.instance.CloseSendMsgAlert();
				}
				else if (RoomInfo.IsSameMemeber(masterMemberInfo, (COM_PLAYERCAMP)stChgPosData.stMemberInfo.stReceiver.bCamp, (int)stChgPosData.stMemberInfo.stReceiver.bPosOfCamp))
				{
					MemberInfo memberInfo2 = Singleton<CRoomSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP)stChgPosData.stMemberInfo.stSender.bCamp, (int)stChgPosData.stMemberInfo.stSender.bPosOfCamp);
					if (memberInfo2 == null)
					{
						return;
					}
					CRoomView.SetChgEnable(false, (COM_PLAYERCAMP)stChgPosData.stMemberInfo.stSender.bCamp, (int)stChgPosData.stMemberInfo.stSender.bPosOfCamp);
					CRoomView.SetSwapTimer(0, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0);
					CRoomView.ShowSwapMsg((int)stChgPosData.stMemberInfo.dwTimeOutSec, (COM_PLAYERCAMP)stChgPosData.stMemberInfo.stSender.bCamp, (int)stChgPosData.stMemberInfo.stSender.bPosOfCamp);
					masterMemberInfo.swapSeq = stChgPosData.stMemberInfo.dwChgPosSeq;
					masterMemberInfo.swapStatus = 2;
					masterMemberInfo.swapUid = memberInfo2.ullUid;
				}
				break;
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_BUSY:
				Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_4", true, 1.5f, null, new object[0]);
				Singleton<CUIManager>.instance.CloseSendMsgAlert();
				break;
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_NPC:
				Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_5", true, 1.5f, null, new object[0]);
				Singleton<CUIManager>.instance.CloseSendMsgAlert();
				break;
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_TIMEOUT:
				Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_6", true, 1.5f, null, new object[0]);
				Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
				break;
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_CANCEL:
				Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_7", true, 1.5f, null, new object[0]);
				Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
				break;
			case COM_CHGROOMPOS_RESULT.COM_CHGROOMPOS_REFUSE:
				Singleton<CUIManager>.instance.OpenTips("Room_Change_Pos_Tip_8", true, 1.5f, null, new object[0]);
				Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
				break;
			}
		}

		[MessageHandler(1024)]
		public static void OnLeaveRoom(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stQuitMultGameRsp.iErrCode == 0)
			{
				if (msg.stPkgData.stQuitMultGameRsp.bLevelFromType == 1)
				{
					Singleton<CRoomSystem>.GetInstance().bInRoom = false;
					Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_ROOM);
					Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
					Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
					CChatUT.LeaveRoom();
				}
				else if (msg.stPkgData.stQuitMultGameRsp.bLevelFromType == 2)
				{
					CMatchingSystem.OnPlayerLeaveMatching();
				}
				MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Exit_Room_Error", true, 1f, null, new object[]
				{
					Utility.ProtErrCodeToStr(1024, msg.stPkgData.stQuitMultGameRsp.iErrCode)
				});
			}
		}

		[MessageHandler(2014)]
		public static void OnRoomStarted(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stStartMultiGameRsp.bErrcode == 0)
			{
				Singleton<CRoomSystem>.instance.SetRoomType(2);
				MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.start, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Start_Game_Error", true, 1f, null, new object[]
				{
					Utility.ProtErrCodeToStr(2014, (int)msg.stPkgData.stStartMultiGameRsp.bErrcode)
				});
			}
		}

		[MessageHandler(1025)]
		public static void OnRoomChange(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
			if (roomInfo == null)
			{
				DebugHelper.Assert(false, "Room Info is NULL!!!");
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 0)
			{
				COM_PLAYERCAMP iCamp = (COM_PLAYERCAMP)msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerAdd.iCamp;
				MemberInfo item = Singleton<CRoomSystem>.GetInstance().CreateMemInfo(ref msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerAdd.stMemInfo, iCamp, roomInfo.roomAttrib.bWarmBattle);
				roomInfo[iCamp].Add(item);
				flag = true;
			}
			else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 1)
			{
				COM_PLAYERCAMP iCamp2 = (COM_PLAYERCAMP)msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerLeave.iCamp;
				int bPos = (int)msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stPlayerLeave.bPos;
				ListView<MemberInfo> listView = roomInfo[iCamp2];
				for (int i = 0; i < listView.Count; i++)
				{
					if ((ulong)listView[i].dwPosOfCamp == (ulong)((long)bPos))
					{
						listView.RemoveAt(i);
						break;
					}
				}
				flag = true;
			}
			else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 2)
			{
				Singleton<CRoomSystem>.GetInstance().bInRoom = false;
				Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_CREATE_ROOM);
				Singleton<CUIManager>.GetInstance().CloseForm(CRoomSystem.PATH_ROOM);
				Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
				Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
				CChatUT.LeaveRoom();
				Singleton<CChatController>.instance.ShowPanel(false, false);
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Room_Kick_Tip", true, 1.5f, null, new object[0]);
				MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0u, string.Empty, 0u, 0u);
			}
			else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 4)
			{
				roomInfo.roomOwner.ullUid = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stMasterChg.stNewMaster.ullMasterUid;
				roomInfo.roomOwner.iGameEntity = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stMasterChg.stNewMaster.iMasterGameEntity;
				flag = true;
			}
			else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 5)
			{
				COMDT_ROOMCHG_CHGMEMBERPOS stChgMemberPos = msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stChgMemberPos;
				ListView<MemberInfo> listView2 = roomInfo[(COM_PLAYERCAMP)stChgMemberPos.bOldCamp];
				ListView<MemberInfo> listView3 = roomInfo[(COM_PLAYERCAMP)stChgMemberPos.bNewCamp];
				MemberInfo memberInfo = roomInfo.GetMemberInfo(stChgMemberPos.ullMemberUid);
				if (memberInfo == null)
				{
					return;
				}
				if (memberInfo.camp == (COM_PLAYERCAMP)stChgMemberPos.bNewCamp && memberInfo.dwPosOfCamp == (uint)stChgMemberPos.bNewPosOfCamp)
				{
					return;
				}
				listView2.Remove(memberInfo);
				MemberInfo memberInfo2 = roomInfo.GetMemberInfo((COM_PLAYERCAMP)stChgMemberPos.bNewCamp, (int)stChgMemberPos.bNewPosOfCamp);
				DebugHelper.Assert(memberInfo != null, "srcMemberInfo is NULL!!");
				memberInfo.camp = (COM_PLAYERCAMP)stChgMemberPos.bNewCamp;
				memberInfo.dwPosOfCamp = (uint)stChgMemberPos.bNewPosOfCamp;
				listView3.Add(memberInfo);
				if (memberInfo2 != null)
				{
					listView3.Remove(memberInfo2);
					memberInfo2.camp = (COM_PLAYERCAMP)stChgMemberPos.bOldCamp;
					memberInfo2.dwPosOfCamp = (uint)stChgMemberPos.bOldPosOfCamp;
					listView2.Add(memberInfo2);
				}
				if (roomInfo.GetMasterMemberInfo().ullUid == stChgMemberPos.ullMemberUid)
				{
					flag2 = true;
				}
				flag = true;
			}
			else if (msg.stPkgData.stRoomChgNtf.stRoomChgInfo.iChgType == 3)
			{
				enRoomState bOldState = (enRoomState)msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stStateChg.bOldState;
				enRoomState bNewState = (enRoomState)msg.stPkgData.stRoomChgNtf.stRoomChgInfo.stChgInfo.stStateChg.bNewState;
				if (bOldState == enRoomState.E_ROOM_PREPARE && bNewState == enRoomState.E_ROOM_WAIT)
				{
					Singleton<LobbyLogic>.GetInstance().inMultiRoom = false;
					Singleton<CHeroSelectBaseSystem>.instance.CloseForm();
					Singleton<CUIManager>.GetInstance().OpenForm(CRoomSystem.PATH_ROOM, false, true);
					CChatUT.EnterRoom();
				}
				if (bOldState == enRoomState.E_ROOM_WAIT && bNewState == enRoomState.E_ROOM_CONFIRM)
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Matching_OpenConfirmBox;
					cUIEvent.m_eventParams.tag = (int)roomInfo.roomAttrib.bPkAI;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
					if (roomInfo.roomAttrib.bWarmBattle)
					{
						CFakePvPHelper.SetConfirmFakeData();
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				roomInfo[(COM_PLAYERCAMP)j].Sort(new Comparison<MemberInfo>(CRoomSystem.SortMemeberFun));
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoomSystem.PATH_ROOM);
			if (form != null)
			{
				if (flag)
				{
					CRoomView.SetRoomData(form.gameObject, roomInfo);
					CRoomObserve.SetObservers(Utility.FindChild(form.gameObject, "Panel_Main/Observers"), roomInfo.roomAttrib.judgeNum, roomInfo[COM_PLAYERCAMP.COM_PLAYERCAMP_MID], roomInfo.GetMasterMemberInfo());
				}
				if (flag2)
				{
					Singleton<CRoomSystem>.instance.RestMasterSwapInfo();
				}
			}
		}

		private static int SortMemeberFun(MemberInfo left, MemberInfo right)
		{
			return (int)(left.dwPosOfCamp - right.dwPosOfCamp);
		}
	}
}

using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameLogic;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class VoiceSys : MonoSingleton<VoiceSys>
	{
		public struct ROOMINFO
		{
			public ulong roomid;

			public ulong ullRoomKey;

			public uint memberid;

			public string openid;

			public ulong uuid;
		}

		public struct ROOMMate
		{
			public uint memID;

			public string openID;

			public ulong uuid;
		}

		public class VoiceState
		{
			public CS_VOICESTATE_TYPE state;

			public ulong uid;

			public VoiceState(ulong uid, CS_VOICESTATE_TYPE state)
			{
				this.uid = uid;
				this.state = state;
			}
		}

		private CApolloVoiceSys m_ApolloVoiceMgr;

		private bool m_isOpenMic;

		private bool m_isOpenSpeaker;

		private bool m_bInRoom;

		private VoiceSys.ROOMINFO m_MyRoomInfo = default(VoiceSys.ROOMINFO);

		private float m_UpdateTime;

		private bool m_bUpdateEnterRoomState;

		private List<VoiceSys.ROOMMate> m_RoomMateList = new List<VoiceSys.ROOMMate>();

		private bool m_bGetIsBattleSupportVoice;

		private bool m_IsBattleSupportVoice;

		private List<ulong> forbidMemberVoice = new List<ulong>();

		private bool m_bGobalUseVoice;

		private bool m_bUseMicOnUser;

		private bool m_bUseVoiceSysSetting;

		private float m_VoiceLevel = 100f;

		private int m_TotalVoiceTime = 10;

		private Transform m_SoundLevel_HeroSelect;

		private Transform m_VoiceBtn;

		private bool m_bInHeroSelectUI;

		public string m_Voice_Server_Not_Open_Tips = string.Empty;

		public string m_Voice_Cannot_JoinVoiceRoom = string.Empty;

		public string m_Voice_Cannot_OpenSetting = string.Empty;

		public string m_Voice_Battle_FirstTips = string.Empty;

		public string m_Voice_Battle_OpenSpeaker = string.Empty;

		public string m_Voice_Battle_CloseSpeaker = string.Empty;

		public string m_Voice_Battle_OpenMic = string.Empty;

		public string m_Voice_Battle_CloseMic = string.Empty;

		public string m_Voice_Battle_FIrstOPenSpeak = string.Empty;

		private float m_fStartTimeHeroSelect;

		private float m_fCurTimeHeroSelect;

		private float m_VoiceUpdateDelta = 0.1f;

		private bool m_bClickSound;

		private int m_nSoundBattleTimerID;

		private bool m_bSoundInBattle;

		public static int maxDeltaTime = 3000;

		private int m_timer = -1;

		private List<VoiceSys.VoiceState> m_voiceStateList = new List<VoiceSys.VoiceState>();

		private CApolloVoiceSys ApolloVoiceMgr
		{
			get
			{
				if (this.m_ApolloVoiceMgr == null)
				{
					return null;
				}
				if (this.m_ApolloVoiceMgr.CallApolloVoiceSDK == null)
				{
					return null;
				}
				return this.m_ApolloVoiceMgr;
			}
		}

		public bool UseMic
		{
			get
			{
				return this.m_isOpenMic;
			}
		}

		public bool UseSpeak
		{
			get
			{
				return this.m_isOpenSpeaker;
			}
		}

		public bool GlobalVoiceSetting
		{
			get
			{
				return this.m_bGobalUseVoice;
			}
		}

		public bool UseMicOnUser
		{
			get
			{
				return this.m_bUseMicOnUser;
			}
			set
			{
				this.m_bUseMicOnUser = value;
			}
		}

		public bool IsUseVoiceSysSetting
		{
			get
			{
				return this.m_bUseVoiceSysSetting;
			}
			set
			{
				this.m_bUseVoiceSysSetting = value;
				if (this.m_bUseVoiceSysSetting)
				{
					this.OpenSpeakers();
				}
				else
				{
					this.ClosenSpeakers();
				}
			}
		}

		public float VoiceLevel
		{
			get
			{
				return this.m_VoiceLevel;
			}
			set
			{
				this.m_VoiceLevel = value;
				this.SetSpeakerVolume((int)this.m_VoiceLevel);
			}
		}

		public int TotalVoiceTime
		{
			get
			{
				return this.m_TotalVoiceTime;
			}
		}

		public CS_VOICESTATE_TYPE lastSendVoiceState
		{
			get;
			set;
		}

		public CS_VOICESTATE_TYPE curVoiceState
		{
			get
			{
				if (this.IsOpenMic())
				{
					return CS_VOICESTATE_TYPE.CS_VOICESTATE_FULL;
				}
				if (this.IsOpenSpeak())
				{
					return CS_VOICESTATE_TYPE.CS_VOICESTATE_PART;
				}
				return CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
			}
		}

		[MessageHandler(3100)]
		public static void On_CreateVoiceRoom(CSPkg msg)
		{
			try
			{
				MonoSingleton<VoiceSys>.GetInstance().EnterRoom(msg.stPkgData.stCreateTvoipRoomNtf);
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception in CreateVoiceRoom, {0}\n {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
		}

		[MessageHandler(3101)]
		public static void On_UpdateRoomMateInfo(CSPkg msg)
		{
			try
			{
				MonoSingleton<VoiceSys>.GetInstance().UpdateRoomMateInfo(msg.stPkgData.stJoinTvoipRoomNtf);
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception in On_UpdateRoomMateInfo, {0}\n {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
		}

        private IEnumerator UpdateEnterRoomState()
        {
            var idx = 0;
            while (!m_bInRoom && (ApolloVoiceMgr != null))
            {
                idx++;
                if (idx < 200)
                {
                    var result = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._GetJoinRoomResult();
                    if (result == ApolloVoiceErr.APOLLO_VOICE_JOIN_SUCC)
                    {
                        m_bInRoom = true;
                        m_UpdateTime = Time.time;
                        if (IsUseVoiceSysSetting)
                        {
                            OpenSpeakers();
                            var bOpenMic = false;
                            if (Singleton<BattleLogic>.GetInstance().isRuning && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                            {
                                bOpenMic = Singleton<CBattleSystem>.GetInstance().FightForm.IsMicUIOpen;
                            }
                            else if (Singleton<CChatController>.GetInstance().IsMicUIOpen())
                            {
                                bOpenMic = true;
                            }

                            if (bOpenMic)
                            {
                                OpenMic();
                            }
                            else
                            {
                                CloseMic();
                            }
                        }
                        else
                        {
                            ClosenSpeakers();
                        }
                        ApolloVoiceMgr.CallApolloVoiceSDK._SetSpeakerVolume((int)VoiceLevel);
                        ApolloVoiceMgr.CallApolloVoiceSDK._SetMemberCount(2);
                        UpdateHeroSelectVoiceBtnState(true);
                        break;
                    }
                    else
                    {
                        Debug.Log(string.Format("JoinRoom failed :  Result : {0}, seq : {1}", result, idx));
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            m_bUpdateEnterRoomState = false;
        }

		private void UpdateRoomMateInfo(SCPKG_JOIN_TVOIP_ROOM_NTF updateInfo)
		{
			uint dwMemberID = updateInfo.stUserInfo.dwMemberID;
			ulong ullUid = updateInfo.stUserInfo.ullUid;
			string text = Utility.UTF8Convert(updateInfo.stUserInfo.szOpenID);
			bool flag = false;
			for (int i = 0; i < this.m_RoomMateList.Count; i++)
			{
				VoiceSys.ROOMMate rOOMMate = this.m_RoomMateList[i];
				if (rOOMMate.openID == text && rOOMMate.uuid == ullUid)
				{
					rOOMMate.memID = dwMemberID;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				VoiceSys.ROOMMate item = default(VoiceSys.ROOMMate);
				item.memID = dwMemberID;
				item.openID = text;
				item.uuid = ullUid;
				this.m_RoomMateList.Add(item);
			}
		}

		private void EnterRoom(SCPKG_CREATE_TVOIP_ROOM_NTF roomInfoSvr)
		{
			this.m_bGobalUseVoice = true;
			this.EnterRoomReal(roomInfoSvr);
		}

		private void EnterRoomReal(SCPKG_CREATE_TVOIP_ROOM_NTF roomInfoSvr)
		{
			if (this.m_bInRoom)
			{
			}
			this.m_bInRoom = false;
			this.LeaveRoom();
			this.UpdateHeroSelectVoiceBtnState(false);
			this.m_RoomMateList.Clear();
			string openId = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false).OpenId;
			this.m_MyRoomInfo = default(VoiceSys.ROOMINFO);
			if (this.m_ApolloVoiceMgr == null)
			{
				this.m_ApolloVoiceMgr = new CApolloVoiceSys();
				this.m_ApolloVoiceMgr.SysInitial();
				if (this.m_ApolloVoiceMgr != null)
				{
				}
				this.CreateEngine();
			}
			if (this.ApolloVoiceMgr != null)
			{
				string[] array = new string[3];
				for (int i = 0; i < 3; i++)
				{
					array[i] = string.Empty;
				}
				if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai() && MonoSingleton<CTongCaiSys>.instance.IsUsingTongcaiIp())
				{
					string str = "16285";
					for (int j = 0; j < (int)roomInfoSvr.wAccessIPCount; j++)
					{
						array[j] = "udp://" + ApolloConfig.loginOnlyIpTongCai + ":" + str;
					}
				}
				else
				{
					for (int k = 0; k < (int)roomInfoSvr.wAccessIPCount; k++)
					{
						uint dwIP = roomInfoSvr.astAccessIPList[k].dwIP;
						array[k] = "udp://" + IPAddress.Parse(((uint)IPAddress.NetworkToHostOrder((int)dwIP)).ToString()).ToString() + ":" + roomInfoSvr.astAccessIPList[k].wPort.ToString();
					}
				}
				this.m_MyRoomInfo.roomid = roomInfoSvr.ullRoomID;
				this.m_MyRoomInfo.ullRoomKey = roomInfoSvr.ullRoomKey;
				this.m_MyRoomInfo.openid = openId;
				bool flag = false;
				int num = 0;
				while ((long)num < (long)((ulong)roomInfoSvr.dwRoomUserCnt))
				{
					if (Utility.UTF8Convert(roomInfoSvr.astRoomUserList[num].szOpenID) == openId)
					{
						flag = true;
						this.m_MyRoomInfo.memberid = roomInfoSvr.astRoomUserList[num].dwMemberID;
						this.m_MyRoomInfo.uuid = roomInfoSvr.astRoomUserList[num].ullUid;
					}
					else
					{
						VoiceSys.ROOMMate item = default(VoiceSys.ROOMMate);
						item.openID = Utility.UTF8Convert(roomInfoSvr.astRoomUserList[num].szOpenID);
						item.memID = roomInfoSvr.astRoomUserList[num].dwMemberID;
						item.uuid = roomInfoSvr.astRoomUserList[num].ullUid;
						this.m_RoomMateList.Add(item);
					}
					num++;
				}
				if (!flag)
				{
				}
				string text = string.Format("roomid is {0}, roomkey is {1}, openid is {2}, memberid is {3}, accIP is {4}, {5}, {6}\n", new object[]
				{
					this.m_MyRoomInfo.roomid,
					this.m_MyRoomInfo.ullRoomKey,
					this.m_MyRoomInfo.openid,
					this.m_MyRoomInfo.memberid,
					array[0],
					array[1],
					array[2]
				});
				if (this.JoinVoiceRoom(array[0], array[1], array[2], (long)this.m_MyRoomInfo.roomid, (long)this.m_MyRoomInfo.ullRoomKey, (short)this.m_MyRoomInfo.memberid, this.m_MyRoomInfo.openid, 6000) == 0 && !this.m_bUpdateEnterRoomState)
				{
					this.m_bUpdateEnterRoomState = true;
					base.StartCoroutine(this.UpdateEnterRoomState());
				}
			}
		}

		public bool IsOpenSpeak()
		{
			return this.m_isOpenSpeaker;
		}

		public bool IsOpenMic()
		{
			return this.m_isOpenMic;
		}

		public void LeaveRoom()
		{
			try
			{
				this.m_bInRoom = false;
				this.m_isOpenMic = false;
				this.m_isOpenSpeaker = false;
				this.QuitRoom();
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception In VoiceSys.LeaveRoom, {0}\n {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
		}

		private int JoinVoiceRoom(string url1, string url2, string url3, long roomId, long roomKey, short memberId, string OpenId, int nTimeOut)
		{
			if (this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				return this.ApolloVoiceMgr.CallApolloVoiceSDK._JoinRoom(url1, url2, url3, roomId, roomKey, memberId, OpenId, nTimeOut);
			}
			return 3;
		}

		private void QuitRoom()
		{
			if (this.ApolloVoiceMgr != null)
			{
				int num = this.ApolloVoiceMgr.CallApolloVoiceSDK._QuitRoom((long)this.m_MyRoomInfo.roomid, (short)this.m_MyRoomInfo.memberid, this.m_MyRoomInfo.openid);
				if (num == 0)
				{
					this.m_bInRoom = false;
				}
				else
				{
					string text = string.Format("QuitRoom Err is {0}", num);
				}
			}
		}

		private void CreateEngine()
		{
			if (this.ApolloVoiceMgr != null)
			{
				ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._CreateApolloVoiceEngine(ApolloConfig.appID);
				if (apolloVoiceErr != ApolloVoiceErr.APOLLO_VOICE_SUCC)
				{
					string text = string.Format("CreateApolloVoiceEngine Err is {0}", apolloVoiceErr);
				}
			}
		}

		private void DestoyEngine()
		{
			if (this.ApolloVoiceMgr != null)
			{
				ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._DestoryApolloVoiceEngine();
				if (apolloVoiceErr != ApolloVoiceErr.APOLLO_VOICE_SUCC)
				{
					string text = string.Format("_DestoryApolloVoiceEngine Err is {0}", apolloVoiceErr);
				}
			}
		}

		public void OpenMic()
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenMic();
				if (apolloVoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
				{
					this.UpdateMyVoiceIcon(1);
					this.m_isOpenMic = true;
				}
				else
				{
					string text = string.Format("OpenMic Err is {0}", apolloVoiceErr);
				}
			}
			else
			{
				this.m_isOpenMic = true;
			}
		}

		public void CloseMic()
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseMic();
				if (apolloVoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
				{
					this.UpdateMyVoiceIcon(0);
					this.m_isOpenMic = false;
				}
				else
				{
					string text = string.Format("CloseMic Err is {0}", apolloVoiceErr);
				}
			}
			else
			{
				this.m_isOpenMic = false;
			}
		}

		public void OpenSpeakers()
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				if (!this.m_isOpenSpeaker)
				{
					ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenSpeaker();
					if (apolloVoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
					{
						this.m_isOpenSpeaker = true;
					}
					else
					{
						string text = string.Format("OpenSpeaker Err is {0}", apolloVoiceErr);
					}
				}
			}
			else
			{
				this.m_isOpenSpeaker = true;
			}
		}

		public void ClosenSpeakers()
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				if (this.m_isOpenSpeaker)
				{
					ApolloVoiceErr apolloVoiceErr = (ApolloVoiceErr)this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseSpeaker();
					if (apolloVoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
					{
						this.m_isOpenSpeaker = false;
					}
					else
					{
						string text = string.Format("CloseSpeaker Err is {0}", apolloVoiceErr);
					}
				}
			}
			else
			{
				this.m_isOpenSpeaker = false;
			}
		}

		private void onGetMemberState()
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				int[] array = new int[12];
				int num = this.ApolloVoiceMgr.CallApolloVoiceSDK._GetMemberState(array);
				if (num >= 0)
				{
					for (int i = 0; i < num; i++)
					{
						int memID = array[2 * i];
						int iMemberState = array[2 * i + 1];
						this.UpdateVoiceIcon(memID, iMemberState);
					}
				}
			}
		}

		public bool IsBattleSupportVoice()
		{
			if (this.m_bGetIsBattleSupportVoice)
			{
				return this.m_IsBattleSupportVoice;
			}
			this.m_bGetIsBattleSupportVoice = true;
			this.m_IsBattleSupportVoice = false;
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null)
			{
				return false;
			}
			int num = 0;
			COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
			List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(playerCamp);
			for (int i = 0; i < allCampPlayers.Count; i++)
			{
				Player player = allCampPlayers[i];
				if (player != null && !player.Computer)
				{
					num++;
				}
			}
			if (num >= 2)
			{
				this.m_IsBattleSupportVoice = true;
				return true;
			}
			this.m_IsBattleSupportVoice = false;
			return false;
		}

		private ulong FindUUIDByMemId(int memID)
		{
			if ((ulong)this.m_MyRoomInfo.memberid == (ulong)((long)memID))
			{
				return this.m_MyRoomInfo.uuid;
			}
			for (int i = 0; i < this.m_RoomMateList.Count; i++)
			{
				VoiceSys.ROOMMate rOOMMate = this.m_RoomMateList[i];
				if ((ulong)rOOMMate.memID == (ulong)((long)memID))
				{
					return rOOMMate.uuid;
				}
			}
			return 0uL;
		}

		private uint FindMemIdByUUID(ulong uuid)
		{
			if (this.m_MyRoomInfo.uuid == uuid)
			{
				return this.m_MyRoomInfo.memberid;
			}
			for (int i = 0; i < this.m_RoomMateList.Count; i++)
			{
				VoiceSys.ROOMMate rOOMMate = this.m_RoomMateList[i];
				if (rOOMMate.uuid == uuid)
				{
					return rOOMMate.memID;
				}
			}
			return 0u;
		}

		public void UpdateMyVoiceIcon(int iMemberState)
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			ulong num = 0uL;
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
			{
				num = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			}
			if (this.m_bInHeroSelectUI)
			{
				Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(num, masterMemberInfo.camp);
				if (teamPlayerElement != null)
				{
					Transform transform = teamPlayerElement.FindChild("heroItemCell/VoiceIcon");
					if (this.m_bInRoom && transform)
					{
						transform.gameObject.CustomSetActive(iMemberState >= 1);
					}
				}
			}
			else if (num > 0uL && Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer != null && this.m_bInRoom && hostPlayer.Captain && hostPlayer.Captain.handle != null && hostPlayer.Captain.handle.HudControl != null)
				{
					hostPlayer.Captain.handle.HudControl.ShowVoiceIcon(iMemberState >= 1);
				}
			}
		}

		private void UpdateVoiceIcon(int memID, int iMemberState)
		{
			if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
			{
				return;
			}
			MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
			if (masterMemberInfo == null)
			{
				return;
			}
			ulong num = this.FindUUIDByMemId(memID);
			if (this.m_bInHeroSelectUI)
			{
				Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(num, masterMemberInfo.camp);
				if (teamPlayerElement != null)
				{
					Transform transform = teamPlayerElement.FindChild("heroItemCell/VoiceIcon");
					if (this.m_bInRoom && transform)
					{
						transform.gameObject.CustomSetActive(iMemberState >= 1);
					}
				}
			}
			else if (num > 0uL && Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer != null && this.m_bInRoom)
				{
					COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
					List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(playerCamp);
					for (int i = 0; i < allCampPlayers.Count; i++)
					{
						Player player = allCampPlayers[i];
						if (this.m_bInRoom && player != null && player.PlayerUId == num)
						{
							if (player.Captain && player.Captain.handle != null && player.Captain.handle.HudControl != null)
							{
								player.Captain.handle.HudControl.ShowVoiceIcon(iMemberState >= 1);
							}
							break;
						}
					}
				}
			}
		}

		public void ClearInBattleForbidMember()
		{
			for (int i = 0; i < this.forbidMemberVoice.Count; i++)
			{
				ulong uid = this.forbidMemberVoice[i];
				this.ForbidMemberVoiceByUID(uid, false);
			}
			this.forbidMemberVoice.Clear();
		}

		public void SwitchForbidden(ulong uid)
		{
			if (!this.forbidMemberVoice.Contains(uid))
			{
				this.forbidMemberVoice.Add(uid);
				MonoSingleton<VoiceSys>.instance.ForbidMemberVoiceByUID(uid, true);
			}
			else
			{
				this.forbidMemberVoice.Remove(uid);
				MonoSingleton<VoiceSys>.instance.ForbidMemberVoiceByUID(uid, false);
			}
		}

		public bool IsForbid(ulong uid)
		{
			return this.forbidMemberVoice.Contains(uid);
		}

		public void ForbidMemberVoiceByUID(ulong uid, bool bForbidden)
		{
			this.ForbidMemberVoice((int)this.FindMemIdByUUID(uid), !bForbidden);
		}

		public void ForbidMemberVoice(int nMemberId, bool bForbidden)
		{
			if (!this.m_bInRoom || this.ApolloVoiceMgr == null || this.ApolloVoiceMgr.CallApolloVoiceSDK == null || this.ApolloVoiceMgr.CallApolloVoiceSDK._EnableMemberVoice(nMemberId, bForbidden) == 0)
			{
			}
		}

		private void SetSpeakerVolume(int level)
		{
			if (this.m_bInRoom && this.ApolloVoiceMgr != null && this.ApolloVoiceMgr.CallApolloVoiceSDK != null)
			{
				if (level <= 2)
				{
					level = 2;
				}
				this.ApolloVoiceMgr.CallApolloVoiceSDK._SetSpeakerVolume(level);
			}
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			if (this.ApolloVoiceMgr == null)
			{
				return;
			}
			if (pauseStatus)
			{
				if (this.ApolloVoiceMgr.CallApolloVoiceSDK._Pause() == 0)
				{
				}
				if (this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseMic() == 0)
				{
				}
				if (this.ApolloVoiceMgr.CallApolloVoiceSDK._CloseSpeaker() == 0)
				{
				}
			}
			else
			{
				if (this.ApolloVoiceMgr.CallApolloVoiceSDK._Resume() == 0)
				{
				}
				if (!this.m_isOpenMic || this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenMic() == 0)
				{
				}
				if (!this.m_isOpenSpeaker || this.ApolloVoiceMgr.CallApolloVoiceSDK._OpenSpeaker() == 0)
				{
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.LeaveRoom();
		}

		public void ShowVoiceBtn_HeroSelect(CUIFormScript formScript)
		{
			this.m_bInHeroSelectUI = true;
			if (formScript != null)
			{
				this.m_SoundLevel_HeroSelect = formScript.transform.Find("chatTools/volume_PL");
			}
		}

		private void UpdateSoundLevel_HeroSelect(float soudLevel, float leftSecond)
		{
			if (this.m_SoundLevel_HeroSelect)
			{
				this.m_SoundLevel_HeroSelect.Find("Volume").GetComponent<Image>().CustomFillAmount(soudLevel);
				this.m_SoundLevel_HeroSelect.Find("CountDown").GetComponent<Text>().text = string.Format("{0:0.00}", leftSecond);
			}
		}

		protected override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_HoldStart_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_VOCEBtn_HeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_Hold_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHold_VOCEBtn_HeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_HoldEnd_VOCEBtn, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_VOCEBtn_HeroSelect));
			this.m_Voice_Server_Not_Open_Tips = Singleton<CTextManager>.GetInstance().GetText("Voice_Server_Not_Open_Tips");
			this.m_Voice_Cannot_JoinVoiceRoom = Singleton<CTextManager>.GetInstance().GetText("Voice_Cannot_JoinVoiceRoom");
			this.m_Voice_Cannot_OpenSetting = Singleton<CTextManager>.GetInstance().GetText("Voice_Cannot_OpenSetting");
			this.m_Voice_Battle_FirstTips = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_FirstTips");
			this.m_Voice_Battle_OpenSpeaker = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_OpenSpeaker");
			this.m_Voice_Battle_CloseSpeaker = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_CloseSpeaker");
			this.m_Voice_Battle_OpenMic = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_OpenMic");
			this.m_Voice_Battle_CloseMic = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_CloseMic");
			this.m_Voice_Battle_FIrstOPenSpeak = Singleton<CTextManager>.GetInstance().GetText("Voice_Battle_FIrstOPenSpeak");
		}

		private void CloseHeroSelctVoice()
		{
			if (this.m_bClickSound)
			{
				this.m_bClickSound = false;
				this.CloseMic();
				if (this.m_SoundLevel_HeroSelect)
				{
					this.m_SoundLevel_HeroSelect.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnHoldStart_VOCEBtn_HeroSelect(CUIEvent uiEvent)
		{
			if (CFakePvPHelper.bInFakeSelect)
			{
				if (!this.m_bUseVoiceSysSetting)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Cannot_OpenSetting, false, 1.5f, null, new object[0]);
					return;
				}
			}
			else
			{
				if (!this.m_bGobalUseVoice)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Server_Not_Open_Tips, false, 1.5f, null, new object[0]);
					return;
				}
				if (!this.m_bInRoom)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(this.m_Voice_Cannot_JoinVoiceRoom, false, 1.5f, null, new object[0]);
					return;
				}
			}
			if (!this.m_bClickSound)
			{
				this.m_bClickSound = true;
				if (this.m_SoundLevel_HeroSelect)
				{
					this.m_SoundLevel_HeroSelect.gameObject.CustomSetActive(true);
				}
				this.m_fStartTimeHeroSelect = Time.time;
				this.m_fCurTimeHeroSelect = Time.time;
				this.UpdateSoundLevel_HeroSelect(UnityEngine.Random.Range(0f, 1f), 10f);
				this.OpenMic();
			}
		}

		private void OnHold_VOCEBtn_HeroSelect(CUIEvent uiEvent)
		{
			if (Time.time - this.m_fStartTimeHeroSelect >= (float)this.m_TotalVoiceTime)
			{
				this.CloseHeroSelctVoice();
			}
			else if (Time.time - this.m_fCurTimeHeroSelect >= this.m_VoiceUpdateDelta)
			{
				this.m_fCurTimeHeroSelect = Time.time;
				this.UpdateSoundLevel_HeroSelect(UnityEngine.Random.Range(0f, 1f), (float)this.m_TotalVoiceTime - (Time.time - this.m_fStartTimeHeroSelect));
			}
		}

		private void OnHoldEnd_VOCEBtn_HeroSelect(CUIEvent uiEvent)
		{
			this.CloseHeroSelctVoice();
		}

		private void UpdateHeroSelectVoiceBtnState(bool bEnable)
		{
			if (this.m_VoiceBtn)
			{
				if (!bEnable)
				{
					CUIEventScript component = this.m_VoiceBtn.GetComponent<CUIEventScript>();
					component.enabled = false;
					this.m_VoiceBtn.GetComponent<Image>().color = new Color(this.m_VoiceBtn.GetComponent<Image>().color.r, this.m_VoiceBtn.GetComponent<Image>().color.g, this.m_VoiceBtn.GetComponent<Image>().color.b, 0.37f);
					Text componentInChildren = this.m_VoiceBtn.GetComponentInChildren<Text>();
					componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
				}
				else
				{
					CUIEventScript component2 = this.m_VoiceBtn.GetComponent<CUIEventScript>();
					component2.enabled = true;
					this.m_VoiceBtn.GetComponent<Image>().color = new Color(this.m_VoiceBtn.GetComponent<Image>().color.r, this.m_VoiceBtn.GetComponent<Image>().color.g, this.m_VoiceBtn.GetComponent<Image>().color.b, 1f);
					Text componentInChildren2 = this.m_VoiceBtn.GetComponentInChildren<Text>();
					componentInChildren2.color = new Color(componentInChildren2.color.r, componentInChildren2.color.g, componentInChildren2.color.b, 1f);
				}
			}
		}

		public void HeroSelectTobattle()
		{
			try
			{
				this.m_bGetIsBattleSupportVoice = false;
				this.m_IsBattleSupportVoice = false;
				this.m_bInHeroSelectUI = false;
				this.m_SoundLevel_HeroSelect = null;
				this.m_VoiceBtn = null;
				MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
				this.CloseHeroSelctVoice();
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exception in HeroSelectTobattle {0} {1}", new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
		}

		private void OnTimerBattle(int timeSeq)
		{
			this.CloseSoundInBattle();
		}

		public bool IsInVoiceRoom()
		{
			return this.m_bInRoom;
		}

		public void OpenSoundInBattle()
		{
			if (this.m_bInRoom && !this.m_bSoundInBattle)
			{
				this.m_bSoundInBattle = true;
				this.OpenMic();
			}
		}

		public void CloseSoundInBattle()
		{
			if (this.m_bSoundInBattle)
			{
				this.m_bSoundInBattle = false;
				this.CloseMic();
				this.m_nSoundBattleTimerID = 0;
			}
		}

		[Conditional("USE_LOG")]
		protected void PrintLog(string message, string filename = null, bool append = false)
		{
		}

		private static void WriteLogtoFile(string filename, string strinfo, bool append = false)
		{
			Debug.Log("commonForTest.WriteLogtoFile");
			string path = "sdcard/" + filename + ".txt";
			using (StreamWriter streamWriter = new StreamWriter(path, append))
			{
				streamWriter.WriteLine(strinfo);
			}
		}

		private void Update()
		{
			if (this.m_bInRoom && Time.time - this.m_UpdateTime >= 1f)
			{
				this.m_UpdateTime = Time.time;
				this.onGetMemberState();
			}
		}

		public void ClearVoiceStateData()
		{
			this.m_voiceStateList.Clear();
			this.lastSendVoiceState = CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_timer);
			this.m_timer = -1;
		}

		public void StartSyncVoiceStateTimer(int ms = 4000)
		{
			if (this.m_timer == -1)
			{
				this.m_timer = Singleton<CTimerManager>.instance.AddTimer(ms, 0, new CTimer.OnTimeUpHandler(this.UpdateSyncVoiceState));
			}
		}

		public void UpdateSyncVoiceState(int index)
		{
			if (this.lastSendVoiceState != this.curVoiceState)
			{
				VoiceStateNetCore.Send_Acnt_VoiceState(this.curVoiceState);
				this.lastSendVoiceState = this.curVoiceState;
			}
		}

		public void SetVoiceState(ulong uid, CS_VOICESTATE_TYPE state)
		{
			for (int i = 0; i < this.m_voiceStateList.Count; i++)
			{
				VoiceSys.VoiceState voiceState = this.m_voiceStateList[i];
				if (voiceState != null)
				{
					if (voiceState.uid == uid)
					{
						voiceState.state = state;
						return;
					}
				}
			}
			this.m_voiceStateList.Add(new VoiceSys.VoiceState(uid, state));
		}

		public void SyncReconnectData(CSDT_RECONN_GAMEINGINFO info)
		{
			if (info != null && info.stCampVoiceState != null)
			{
				for (int i = 0; i < (int)info.stCampVoiceState.bPlayerNum; i++)
				{
					CSDT_RECONN_PLAYERVOICEINFO cSDT_RECONN_PLAYERVOICEINFO = info.stCampVoiceState.astPlayerVoiceState[i];
					if (cSDT_RECONN_PLAYERVOICEINFO != null)
					{
						this.SetVoiceState(cSDT_RECONN_PLAYERVOICEINFO.ullUid, (CS_VOICESTATE_TYPE)cSDT_RECONN_PLAYERVOICEINFO.bVoiceState);
					}
				}
			}
		}

		public CS_VOICESTATE_TYPE TryGetVoiceState(ulong uid)
		{
			for (int i = 0; i < this.m_voiceStateList.Count; i++)
			{
				VoiceSys.VoiceState voiceState = this.m_voiceStateList[i];
				if (voiceState != null)
				{
					if (voiceState.uid == uid)
					{
						return voiceState.state;
					}
				}
			}
			return CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE;
		}
	}
}

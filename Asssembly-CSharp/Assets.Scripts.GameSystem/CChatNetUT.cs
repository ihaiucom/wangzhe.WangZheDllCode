using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CChatNetUT
	{
		public static void Send_Complaints_Chat_Req(ulong ullUid, uint logicWorldId, string name, string openId, string content, uint channelType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1303u);
			cSPkg.stPkgData.stComplaintChatReq.ullComplaintRoleID = ullUid;
			StringHelper.StringToUTF8Bytes(name, ref cSPkg.stPkgData.stComplaintChatReq.szComplaintName);
			cSPkg.stPkgData.stComplaintChatReq.dwComplaintLogicworldID = logicWorldId;
			StringHelper.StringToUTF8Bytes(content, ref cSPkg.stPkgData.stComplaintChatReq.szComplaintChatContent);
			StringHelper.StringToUTF8Bytes(openId, ref cSPkg.stPkgData.stComplaintChatReq.szComplaintOpenID);
			cSPkg.stPkgData.stComplaintChatReq.dwComplaintChatType = channelType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(1304)]
		public static void OnSend_Complaints_Chat_Res(CSPkg msg)
		{
			int iResult = msg.stPkgData.stComplaintChatRsp.iResult;
			if (iResult == 0)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("COMPLAINT_OK");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
			}
			else if (iResult == 1)
			{
				string text2 = Singleton<CTextManager>.GetInstance().GetText("COMPLAINT_Limit");
				Singleton<CUIManager>.GetInstance().OpenMessageBox(text2, false);
			}
		}

		public static void Send_Private_Chat(ulong ullUid, uint logicWorldId, string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 2;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(2L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.ullUid = ullUid;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.iLogicWorldID = (int)logicWorldId;
			COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(ullUid, logicWorldId);
			if (gameOrSnsFriend != null)
			{
				cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.szName = gameOrSnsFriend.szUserName;
				cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.dwLevel = gameOrSnsFriend.dwLevel;
			}
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Debug.Log(string.Concat(new object[]
			{
				"--- send private chat, id:",
				ullUid,
				",logicworldid:",
				logicWorldId,
				",content:",
				text
			}));
		}

		public static void Send_Lobby_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 1;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(1L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stLogicWord.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby).Start_InputCD();
		}

		public static void Send_Guild_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 4;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(4L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stGuild.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_GuildMatchTeam_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 11;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(11L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stGuildTeam.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_Room_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 3;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(3L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stRoom.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_SelectHero_Chat(uint template_id)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 5;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(5L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.bChatType = 1;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.select(1L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.stContentID.dwTextID = template_id;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public static void Send_SelectHero_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 5;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(5L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.bChatType = 2;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.select(2L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.stContentStr.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public static void Send_Team_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 6;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(6L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stTeam.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_Settle_Chat(string text)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 8;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(8L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stSettle.szContent = UT.String2Bytes(text);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_Leave_Settle()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5232u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Send_GetChat_Req(EChatChannel channel)
		{
			if (!Singleton<NetworkModule>.GetInstance().lobbySvr.connected)
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1301u);
			cSPkg.stPkgData.stGetChatMsgReq.bChatType = (byte)CChatUT.Convert_Channel_ChatMsgType(channel);
			if (channel == EChatChannel.Lobby)
			{
				if (Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp != 0u)
				{
					cSPkg.stPkgData.stGetChatMsgReq.dwLastTimeStamp = Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp;
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				}
			}
			else
			{
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		[MessageHandler(1302)]
		public static void On_Chat_NTF(CSPkg msg)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("On_Chat_GetMsg_NTF", msg);
		}

		[MessageHandler(1307)]
		public static void On_Offline_Chat_NTF(CSPkg msg)
		{
			Debug.Log("---Chat On_Offline_Chat_NTF...");
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Chat_Offline_GetMsg_NTF", msg);
		}

		public static void Send_Clear_Offline(List<int> delIndexList)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1308u);
			cSPkg.stPkgData.stCleanOfflineChatReq.bDelChatCnt = (byte)delIndexList.get_Count();
			for (int i = 0; i < delIndexList.get_Count(); i++)
			{
				cSPkg.stPkgData.stCleanOfflineChatReq.ChatIndex[i] = delIndexList.get_Item(i);
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Debug.Log("---Chat Send_Clear_Offline Req, count:" + delIndexList.get_Count());
		}

		[MessageHandler(1309)]
		public static void ReceivePlayerLeaveSettlementRoomNtf(CSPkg msg)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Chat_PlayerLeaveSettle_Ntf", msg);
		}

		public static void RequestGetGuildRecruitReq(uint startTime)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2289u);
			cSPkg.stPkgData.stGetGuildRecruitReq.dwStartTime = startTime;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(2290)]
		public static void ReceiveGetGuildRecruitRsp(CSPkg msg)
		{
			Singleton<CChatController>.GetInstance().RefreshGuildRecruitInfo(msg.stPkgData.stGetGuildRecruitRsp);
		}
	}
}

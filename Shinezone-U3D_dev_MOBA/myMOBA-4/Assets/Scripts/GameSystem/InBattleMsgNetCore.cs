using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class InBattleMsgNetCore
	{
		public static void SendInBattleMsg_PreConfig(uint id, COM_INBATTLE_CHAT_TYPE msgType, uint heroID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 7;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(7L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.bChatType = (byte)msgType;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.select((long)msgType);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stFrom.dwAcntHeroID = heroID;
			if (msgType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_SIGNAL)
			{
				cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stSignalID.dwTextID = id;
			}
			else if (msgType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_BUBBLE)
			{
				cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stBubbleID.dwTextID = id;
			}
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public static void SendInBattleMsg_InputChat(string content, byte camp)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1300u);
			cSPkg.stPkgData.stChatReq.stChatMsg.bType = 7;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.select(7L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.bChatType = 3;
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.select(3L);
			cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stContentStr.bCampLimit = camp;
			StringHelper.StringToUTF8Bytes(content, ref cSPkg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stContentStr.szContent);
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public static void SendShortCut_Config(ListView<TabElement> list)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5224u);
			for (int i = 0; i < (int)Singleton<InBattleMsgMgr>.instance.totalCount; i++)
			{
				if (i < list.Count)
				{
					TabElement tabElement = list[i];
					if (tabElement != null)
					{
						COMDT_SELFDEFINE_DETAIL_CHATINFO cOMDT_SELFDEFINE_DETAIL_CHATINFO = cSPkg.stPkgData.stSelfDefineChatInfoChgReq.stChatInfo.astChatMsg[i];
						cOMDT_SELFDEFINE_DETAIL_CHATINFO.bChatType = 1;
						cOMDT_SELFDEFINE_DETAIL_CHATINFO.stChatInfo.select(1L);
						cOMDT_SELFDEFINE_DETAIL_CHATINFO.stChatInfo.stSignalID.dwTextID = tabElement.cfgId;
					}
				}
			}
			cSPkg.stPkgData.stSelfDefineChatInfoChgReq.stChatInfo.bMsgCnt = Singleton<InBattleMsgMgr>.instance.totalCount;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(5225)]
		public static void OnSendShortCut_Config_Rsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_SELFDEFINE_CHATINFO_CHG_RSP stSelfDefineChatInfoChgRsp = msg.stPkgData.stSelfDefineChatInfoChgRsp;
			if (stSelfDefineChatInfoChgRsp.bResult == 0)
			{
				Singleton<CUIManager>.instance.OpenTips("修改成功", false, 1.5f, null, new object[0]);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips("err code:" + stSelfDefineChatInfoChgRsp.bResult, false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(5240)]
		public static void OnObserveTipsCome(CSPkg msg)
		{
			InBattleInputChat inputChat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
			if (inputChat != null)
			{
				SCPKG_OBTIPS_NTF stOBTipsNtf = msg.stPkgData.stOBTipsNtf;
				COM_OBNUM_TIPS_TYPE bTipsType = (COM_OBNUM_TIPS_TYPE)stOBTipsNtf.bTipsType;
				string arg = string.Empty;
				Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(stOBTipsNtf.ullRecomUid);
				if (playerByUid != null)
				{
					arg = playerByUid.Name;
				}
				InBattleInputChat.InBatChatEntity ent = inputChat.ConstructColorFlagEnt(string.Format(Singleton<CTextManager>.GetInstance().GetText(bTipsType.ToString()), stOBTipsNtf.dwOBNum, arg));
				inputChat.Add(ent);
			}
		}
	}
}

using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[MessageHandlerClass]
	public class BurnExpeditionNetCore
	{
		public static void Send_Get_BURNING_PROGRESS_REQ()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2700u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(2701)]
		public static void On_GET_BURNING_PROGRESS_RSP(CSPkg msg)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_GET_BURNING_PROGRESS_RSP", msg);
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		public static void Send_GET_BURNING_REWARD_REQ(byte levelNo, int levelID)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2702u);
			cSPkg.stPkgData.stGetBurningRewardReq.bLevelNo = levelNo;
			cSPkg.stPkgData.stGetBurningRewardReq.iLevelID = levelID;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(2703)]
		public static void On_GET_BURNING_REWARD_RSP(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_GET_BURNING_REWARD_RSP", msg);
		}

		public static void Send_RESET_BURNING_PROGRESS_REQ(byte value)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2704u);
			cSPkg.stPkgData.stResetBurningProgressReq.bDifficultType = value;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(2705)]
		public static void On_RESET_BURNING_PROGRESS_RSP(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Burn_RESET_BURNING_PROGRESS_RSP", msg);
		}

		public static void Clear_ResetBurning_Limit()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1012u);
			cSPkg.stPkgData.stCheatCmd.iCheatCmd = 35;
			cSPkg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
			cSPkg.stPkgData.stCheatCmd.stCheatCmdDetail.stClrBurningLimit = new CSDT_CHEAT_CLR_BURNING_LIMIT();
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public static void Clear_ResetBurning_Power()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1012u);
			cSPkg.stPkgData.stCheatCmd.iCheatCmd = 39;
			cSPkg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}
	}
}

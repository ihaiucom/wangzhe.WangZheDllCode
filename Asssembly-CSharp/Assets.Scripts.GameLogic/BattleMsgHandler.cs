using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	[MessageHandlerClass]
	public class BattleMsgHandler
	{
		[MessageHandler(1044)]
		public static void onIsAccept_Aiplayer(CSPkg msg)
		{
			stUIEventParams par = default(stUIEventParams);
			par.commonUInt32Param1 = msg.stPkgData.stIsAcceptAiPlayerReq.dwAiPlayerObjID;
			Singleton<CUIManager>.GetInstance().OpenSmallMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trusteeship_Ask"), true, enUIEventID.Trusteeship_Accept, enUIEventID.Trusteeship_Cancel, par, 30, enUIEventID.Trusteeship_Cancel, Singleton<CTextManager>.GetInstance().GetText("FollowMe"), Singleton<CTextManager>.GetInstance().GetText("StayHome"), false);
		}

		[MessageHandler(1046)]
		public static void onHangup(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Hangup_Warnning"), false);
		}

		[MessageHandler(1099)]
		public static void OnBroadHangup(CSPkg msg)
		{
			Singleton<EventRouter>.instance.BroadCastEvent<HANGUP_TYPE, uint>(EventID.HangupNtf, (HANGUP_TYPE)msg.stPkgData.stHangUpNtf.bType, msg.stPkgData.stHangUpNtf.dwObjID);
		}

		[MessageHandler(1085)]
		public static void OnGameOverEvent(CSPkg msg)
		{
			if (Singleton<BattleLogic>.instance.isWaitGameEnd)
			{
				return;
			}
			if (msg.stPkgData.stNtfCltGameOver.bWinCamp > 0)
			{
				COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
				Singleton<BattleLogic>.instance.battleStat.iBattleResult = (((byte)playerCamp == msg.stPkgData.stNtfCltGameOver.bWinCamp) ? 1 : 2);
				COM_PLAYERCAMP camp;
				if (Singleton<BattleLogic>.instance.battleStat.iBattleResult == 1)
				{
					camp = BattleLogic.GetOppositeCmp(playerCamp);
				}
				else
				{
					camp = playerCamp;
				}
				BattleLogic.ForceKillCrystal((int)camp);
			}
			Singleton<CSurrenderSystem>.instance.DelayCloseSurrenderForm(5);
		}

		[MessageHandler(5606)]
		public static void OnRecvServerReqClientInfo(CSPkg msg)
		{
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5607u);
				if (cSPkg != null)
				{
					List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
					if (allPlayers != null)
					{
						byte b = 0;
						int count = allPlayers.get_Count();
						for (int i = 0; i < count; i++)
						{
							if (!allPlayers.get_Item(i).Computer)
							{
								cSPkg.stPkgData.stCltUploadDataRsp.astMemberData[(int)b].dwAcntObjID = allPlayers.get_Item(i).PlayerId;
								PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(allPlayers.get_Item(i).PlayerId);
								if (playerKDA != null)
								{
									cSPkg.stPkgData.stCltUploadDataRsp.astMemberData[(int)b].wAssistCnt = (ushort)playerKDA.numAssist;
									cSPkg.stPkgData.stCltUploadDataRsp.astMemberData[(int)b].wDeadCnt = (ushort)playerKDA.numDead;
									cSPkg.stPkgData.stCltUploadDataRsp.astMemberData[(int)b].wKillCnt = (ushort)playerKDA.numKill;
								}
								if (allPlayers.get_Item(i).Captain && allPlayers.get_Item(i).Captain.handle.ValueComponent != null)
								{
									cSPkg.stPkgData.stCltUploadDataRsp.astMemberData[(int)b].dwBattleCoin = (uint)allPlayers.get_Item(i).Captain.handle.ValueComponent.GetGoldCoinIncomeInBattle();
								}
								b += 1;
							}
						}
						cSPkg.stPkgData.stCltUploadDataRsp.bMemberNum = b;
					}
					Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
				}
			}
		}
	}
}

using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CPvPHeroShop : Singleton<CPvPHeroShop>
	{
		private static int ReGetFreeHeroTimer;

		public override void UnInit()
		{
			if (CPvPHeroShop.ReGetFreeHeroTimer != 0)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(CPvPHeroShop.ReGetFreeHeroTimer);
			}
			base.UnInit();
		}

		public void GetFreeHero(int seq = 0)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1814u);
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1815)]
		public static void OnGetFreeHero(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (CPvPHeroShop.ReGetFreeHeroTimer != 0)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(CPvPHeroShop.ReGetFreeHeroTimer);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.SetFreeHeroInfo(msg.stPkgData.stFreeHeroRsp.stFreeHero);
				masterRoleInfo.SetFreeHeroSymbol(msg.stPkgData.stFreeHeroRsp.stFreeHeroSymbol);
				if ((long)CRoleInfo.GetCurrentUTCTime() >= (long)((ulong)msg.stPkgData.stFreeHeroRsp.stFreeHero.dwDeadline))
				{
					CPvPHeroShop.ReGetFreeHeroTimer = Singleton<CTimerManager>.GetInstance().AddTimer(900000, 1, new CTimer.OnTimeUpHandler(Singleton<CPvPHeroShop>.GetInstance().GetFreeHero));
				}
			}
			else
			{
				DebugHelper.Assert(false, "Master RoleInfo is NULL!!!");
			}
		}
	}
}

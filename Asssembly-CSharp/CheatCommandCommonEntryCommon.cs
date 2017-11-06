using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommandEntry("工具"), MessageHandlerClass]
internal class CheatCommandCommonEntryCommon
{
	[CheatCommandEntryMethod("一键毕业", true, false)]
	public static string FinishSchool()
	{
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("FinishLevel", new string[]
		{
			string.Empty
		});
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("UnlockAllLevel", new string[]
		{
			string.Empty
		});
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("ClearStoreLimit", new string[]
		{
			string.Empty
		});
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("AddHero", new string[]
		{
			"0"
		});
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("UnlockPvPHero", new string[]
		{
			"0"
		});
		Singleton<CheatCommandsRepository>.instance.ExecuteCommand("SetNewbieGuideState", new string[]
		{
			string.Empty
		});
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("服务器/获取服务器时间", true, false)]
	public static string GetServerTime()
	{
		CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4001u);
		Singleton<NetworkModule>.instance.SendLobbyMsg(ref cSPkg, false);
		return "Wait server rsp.";
	}

	[MessageHandler(4002)]
	public static void OnServerTimeRSP(CSPkg msg)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			CRoleInfo.SetServerTime((int)msg.stPkgData.stServerTimeRsp.dwUTCSec);
			masterRoleInfo.SetGlobalRefreshTimer(msg.stPkgData.stServerTimeRsp.dwUTCSec, true);
		}
		MonoSingleton<ConsoleWindow>.instance.AddMessage(string.Format("{0}.{1}.{2} {3}:{4}:{5}", new object[]
		{
			msg.stPkgData.stServerTimeRsp.iYear,
			msg.stPkgData.stServerTimeRsp.iMonth,
			msg.stPkgData.stServerTimeRsp.iDay,
			msg.stPkgData.stServerTimeRsp.iHour,
			msg.stPkgData.stServerTimeRsp.iMin,
			msg.stPkgData.stServerTimeRsp.iSec
		}));
		Singleton<EventRouter>.instance.BroadCastEvent(EventID.EDITOR_REFRESH_GM_PANEL);
	}
}

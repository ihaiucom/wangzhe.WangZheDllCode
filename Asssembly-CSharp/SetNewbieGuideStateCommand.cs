using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("关卡/SetNewbieGuideState", "完成所有新手引导", 28)]
internal class SetNewbieGuideStateCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && Singleton<LobbyLogic>.GetInstance().isLogin)
		{
			CheatCmdRef.stDyeNewbieBit = new CSDT_CHEAT_DYE_NEWBIE_BIT();
			CheatCmdRef.stDyeNewbieBit.bOpenOrClose = 1;
			CheatCmdRef.stDyeNewbieBit.bIsAll = 1;
			CheatCmdRef.stDyeNewbieBit.dwApntBit = 0u;
			for (int i = 0; i < 106; i++)
			{
				masterRoleInfo.SetGuidedStateSet(i, true);
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuideAll(true, false, true);
			MonoSingleton<NewbieGuideManager>.GetInstance().ForceSetWeakGuideCompleteAll(false, false, true);
			masterRoleInfo.SyncNewbieAchieveToSvr(true);
			return CheatCommandBase.Done;
		}
		return "undone";
	}
}

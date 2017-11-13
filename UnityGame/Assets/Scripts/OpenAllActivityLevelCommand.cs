using CSProtocol;
using System;

[CheatCommand("关卡/副本/OpenAllActivityLevel", "活动副本全开", 26)]
internal class OpenAllActivityLevelCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stUnlockActivity = new CSDT_CHEAT_UNLOCK_ACTIVITY();
		CheatCmdRef.stUnlockActivity.bUnlock = 1;
		return CheatCommandBase.Done;
	}
}

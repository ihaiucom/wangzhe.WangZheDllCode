using CSProtocol;
using System;

[CheatCommand("关卡/副本/CloseActivityLevel", "活动副本GM关闭", 26)]
internal class CloseActivityLevelCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stUnlockActivity = new CSDT_CHEAT_UNLOCK_ACTIVITY();
		CheatCmdRef.stUnlockActivity.bUnlock = 0;
		return CheatCommandBase.Done;
	}
}

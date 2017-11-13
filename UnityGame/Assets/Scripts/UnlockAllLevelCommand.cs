using CSProtocol;
using System;

[CheatCommand("关卡/UnlockAllLevel", "解锁所有闯关关卡", 12)]
internal class UnlockAllLevelCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stUnlockLevel = new CSDT_CHEAT_UNLOCK_LEVEL();
		return CheatCommandBase.Done;
	}
}

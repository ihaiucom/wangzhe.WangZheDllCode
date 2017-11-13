using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("关卡/副本/ClearEliteLevel", "重置精英副本过关次数", 27)]
internal class ClearEliteLevelCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CAdventureSys.ResetElitePlayNum();
		CheatCmdRef.stClrEliteLevel = new CSDT_CHEAT_CLR_ELITE_LEVEL();
		return CheatCommandBase.Done;
	}
}

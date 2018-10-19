using CSProtocol;
using System;

[CheatCommand("工具/ClearNameChgCardCD", "清除改名CD", 81)]
internal class ClearNameChgCardCDCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stClrChgNameCD = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stClrChgNameCD.iValue = 1;
		return CheatCommandBase.Done;
	}
}

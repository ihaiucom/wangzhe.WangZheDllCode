using CSProtocol;
using System;

[CheatCommand("工具/清除招募数据", "清除招募数据", 92)]
internal class ClearRecruitmentBit : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		return CheatCommandBase.Done;
	}
}

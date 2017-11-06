using CSProtocol;
using System;

[CheatCommand("通用/商城/ClearStoreLimit", "清空商店购买限制", 14)]
internal class ClearStoreLimitCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		return CheatCommandBase.Done;
	}
}

using CSProtocol;
using System;

[CheatCommand("通用/战队/CreateGuild", "一键创建战队", 74)]
internal class CreateGuild : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stCreateGuild = new CSDT_CREATE_GUILD();
		CheatCmdRef.stCreateGuild.ullGuildID = 0uL;
		return CheatCommandBase.Done;
	}
}

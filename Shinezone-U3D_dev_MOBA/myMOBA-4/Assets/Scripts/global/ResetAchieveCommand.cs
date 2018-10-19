using CSProtocol;
using System;

[ArgumentDescription(1, typeof(uint), "成就ID", new object[]
{

}), CheatCommand("英雄/属性修改/其它/ResetAchieve", "重置成就", 84)]
internal class ResetAchieveCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		uint dwAchieveID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		CheatCmdRef.stResetAchieve = new CSDT_CHEAT_RESET_ACHIEVE();
		CheatCmdRef.stResetAchieve.dwAchieveID = dwAchieveID;
		return CheatCommandBase.Done;
	}
}

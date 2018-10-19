using CSProtocol;
using System;

[ArgumentDescription(1, typeof(uint), "成就ID", new object[]
{

}), CheatCommand("英雄/属性修改/其它/SetAchieveDone", "达成成就", 85)]
internal class SetAchieveDoneCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		uint dwAchieveID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		CheatCmdRef.stDoneAchieve = new CSDT_CHEAT_DONE_ACHIEVE();
		CheatCmdRef.stDoneAchieve.dwAchieveID = dwAchieveID;
		return CheatCommandBase.Done;
	}
}

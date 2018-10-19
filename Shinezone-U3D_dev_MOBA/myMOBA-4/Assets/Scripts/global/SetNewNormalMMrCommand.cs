using CSProtocol;
using System;

[ArgumentDescription(5, typeof(int), "连负场数", new object[]
{

}), ArgumentDescription(1, typeof(int), "MMR", new object[]
{

}), ArgumentDescription(2, typeof(int), "胜场", new object[]
{

}), ArgumentDescription(4, typeof(int), "连胜场数", new object[]
{

}), ArgumentDescription(0, typeof(int), "MMR类型", new object[]
{

}), ArgumentDescription(3, typeof(int), "总场数", new object[]
{

}), CheatCommand("英雄/属性/SetNewNormalMMr", "新MMR设置", 71)]
internal class SetNewNormalMMrCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stChgNewNormalMMR = new CSDT_CHEAT_CHG_NEW_NORMALMMR();
		CheatCmdRef.stChgNewNormalMMR.bMMRType = CheatCommandBase.SmartConvert<byte>(InArguments[0]);
		CheatCmdRef.stChgNewNormalMMR.iMMR = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stChgNewNormalMMR.dwWinNum = CheatCommandBase.SmartConvert<uint>(InArguments[2]);
		CheatCmdRef.stChgNewNormalMMR.dwTotalNum = CheatCommandBase.SmartConvert<uint>(InArguments[3]);
		CheatCmdRef.stChgNewNormalMMR.dwContinuousWin = CheatCommandBase.SmartConvert<uint>(InArguments[4]);
		CheatCmdRef.stChgNewNormalMMR.dwContinuousLose = CheatCommandBase.SmartConvert<uint>(InArguments[5]);
		return CheatCommandBase.Done;
	}
}

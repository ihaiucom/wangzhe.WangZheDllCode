using CSProtocol;
using System;

[ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 4, typeof(int), "分", "0", new object[]
{

}), ArgumentDescription(2, typeof(int), "日", new object[]
{

}), ArgumentDescription(3, typeof(int), "时", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 0, typeof(int), "年", "2016", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 5, typeof(int), "秒", "0", new object[]
{

}), ArgumentDescription(1, typeof(int), "月", new object[]
{

}), CheatCommand("工具/服务器/SetPvpBanTime", "设置禁止PVP截止时间", 91)]
internal class SetPvpBanTimeCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stSetPvpBanEndTime = new CSDT_CHEAT_SET_OFFSET_SEC();
		CheatCmdRef.stSetPvpBanEndTime.iYear = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		CheatCmdRef.stSetPvpBanEndTime.iMonth = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stSetPvpBanEndTime.iDay = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stSetPvpBanEndTime.iHour = CheatCommandBase.SmartConvert<int>(InArguments[3]);
		CheatCmdRef.stSetPvpBanEndTime.iMin = CheatCommandBase.SmartConvert<int>(InArguments[4]);
		CheatCmdRef.stSetPvpBanEndTime.iSec = CheatCommandBase.SmartConvert<int>(InArguments[5]);
		return CheatCommandBase.Done;
	}
}

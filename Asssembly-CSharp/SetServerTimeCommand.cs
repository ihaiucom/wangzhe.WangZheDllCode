using CSProtocol;
using System;

[ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 0, typeof(int), "年", "2016", new object[]
{

}), ArgumentDescription(1, typeof(int), "月", new object[]
{

}), ArgumentDescription(3, typeof(int), "时", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 5, typeof(int), "秒", "0", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 4, typeof(int), "分", "0", new object[]
{

}), ArgumentDescription(2, typeof(int), "日", new object[]
{

}), CheatCommand("工具/服务器/SetServerTime", "设置服务器时间", 31)]
internal class SetServerTimeCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stSetOffsetSec = new CSDT_CHEAT_SET_OFFSET_SEC();
		CheatCmdRef.stSetOffsetSec.iYear = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		CheatCmdRef.stSetOffsetSec.iMonth = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stSetOffsetSec.iDay = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stSetOffsetSec.iHour = CheatCommandBase.SmartConvert<int>(InArguments[3]);
		CheatCmdRef.stSetOffsetSec.iMin = CheatCommandBase.SmartConvert<int>(InArguments[4]);
		CheatCmdRef.stSetOffsetSec.iSec = CheatCommandBase.SmartConvert<int>(InArguments[5]);
		return CheatCommandBase.Done;
	}
}

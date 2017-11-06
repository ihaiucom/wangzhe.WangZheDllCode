using CSProtocol;
using System;

[ArgumentDescription(0, typeof(AcntInfoUpdateType), "属性类别", new object[]
{

}), ArgumentDescription(1, typeof(int), "数值", new object[]
{

}), CheatCommand("英雄/属性/UpAcntInfo", "改变玩家属性", 15)]
internal class UpAcntInfoCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stUpdAcntInfo = new CSDT_CHEAT_UPDACNTINFO();
		CheatCmdRef.stUpdAcntInfo.iUpdType = CheatCommandBase.StringToEnum(InArguments[0], typeof(AcntInfoUpdateType));
		CheatCmdRef.stUpdAcntInfo.iUpdValue = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		return CheatCommandBase.Done;
	}
}

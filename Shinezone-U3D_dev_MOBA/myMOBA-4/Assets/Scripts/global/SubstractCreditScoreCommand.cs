using CSProtocol;
using System;

[ArgumentDescription(0, typeof(EComplaintType), "扣信誉积分类型", new object[]
{

}), ArgumentDescription(1, typeof(int), "分数(负数才是减分)", new object[]
{

}), CheatCommand("英雄/属性修改/数值/SubstractCreditScore", "扣除信誉积分", 86)]
internal class SubstractCreditScoreCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		EComplaintType dwType = CheatCommandBase.SmartConvert<EComplaintType>(InArguments[0]);
		int iValue = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stDelCreditByType = new CSDT_CHEAT_DELCREDIT();
		CheatCmdRef.stDelCreditByType.dwType = (uint)dwType;
		CheatCmdRef.stDelCreditByType.iValue = iValue;
		return CheatCommandBase.Done;
	}
}

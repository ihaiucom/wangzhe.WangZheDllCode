using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "数量", new object[]
{

}), ArgumentDescription(0, typeof(EHonorType), "荣誉类型", new object[]
{

}), CheatCommand("英雄/属性修改/其它/ChangeHonorPoint", "加荣誉(局内)点", 72)]
internal class ChangeHonorPointCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		EHonorType iHonorID = CheatCommandBase.SmartConvert<EHonorType>(InArguments[0]);
		int iAddValue = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stChgHonorInfo = new CSDT_CHEAT_CHG_HONORINFO();
		CheatCmdRef.stChgHonorInfo.iHonorID = (int)iHonorID;
		CheatCmdRef.stChgHonorInfo.iAddValue = iAddValue;
		return CheatCommandBase.Done;
	}
}

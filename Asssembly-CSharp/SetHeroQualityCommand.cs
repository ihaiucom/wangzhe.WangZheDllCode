using CSProtocol;
using System;

[ArgumentDescription(2, typeof(int), "品阶", new object[]
{

}), ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), ArgumentDescription(1, typeof(int), "品质", new object[]
{

}), CheatCommand("英雄/属性修改/数值/SetHeroQyality", "设置品质", 25)]
internal class SetHeroQualityCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		int num = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		int num2 = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stSetHeroQuality = new CSDT_CHEAT_SETHEROQUALITY();
		CheatCmdRef.stSetHeroQuality.dwHeroID = (uint)dwHeroID;
		CheatCmdRef.stSetHeroQuality.stQuality.wQuality = (ushort)num;
		CheatCmdRef.stSetHeroQuality.stQuality.wSubQuality = (ushort)num2;
		return CheatCommandBase.Done;
	}
}

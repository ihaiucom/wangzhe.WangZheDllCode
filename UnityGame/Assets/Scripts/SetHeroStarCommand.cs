using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "星级", new object[]
{

}), ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), CheatCommand("英雄/属性修改/经验等级/SetHeroStar", "设置英雄星级", 24)]
internal class SetHeroStarCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		int dwStar = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stSetHeroStar = new CSDT_CHEAT_SETHEROSTAR();
		CheatCmdRef.stSetHeroStar.dwHeroID = (uint)dwHeroID;
		CheatCmdRef.stSetHeroStar.dwStar = (uint)dwStar;
		return CheatCommandBase.Done;
	}
}

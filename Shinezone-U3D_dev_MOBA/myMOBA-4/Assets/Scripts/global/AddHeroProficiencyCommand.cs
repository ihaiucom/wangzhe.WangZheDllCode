using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "熟练度经验值", new object[]
{

}), ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), CheatCommand("英雄/属性/AddHeroProficiency", "增加英雄熟练度经验值", 49)]
internal class AddHeroProficiencyCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		int iValue = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stAddHeroProficiency = new CSDT_CHEAT_HEROVAL();
		CheatCmdRef.stAddHeroProficiency.dwHeroID = (uint)dwHeroID;
		CheatCmdRef.stAddHeroProficiency.iValue = iValue;
		return CheatCommandBase.Done;
	}
}

using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "段位值", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetGradeOfRank", "排位赛段位（范围1-16）", 18)]
internal class SetGradeOfRankCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGradeOfRank = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetGradeOfRank.iValue = InValue;
	}
}

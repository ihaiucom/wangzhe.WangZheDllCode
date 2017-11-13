using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "MMR值", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetMMROfRank", "排位赛MMR值（范围0--5000）", 17)]
internal class SetMMROfRankCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetMMROfRank = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetMMROfRank.iValue = InValue;
	}
}

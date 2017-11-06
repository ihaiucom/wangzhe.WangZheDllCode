using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "连胜场", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetConWinCntOfRank", "排位赛连胜场数", 20)]
internal class SetConWinCntOfRankCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetConWinCntOfRank = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetConWinCntOfRank.iValue = InValue;
	}
}

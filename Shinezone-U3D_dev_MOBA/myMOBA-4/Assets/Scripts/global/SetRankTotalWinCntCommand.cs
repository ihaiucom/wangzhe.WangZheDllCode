using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数值", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetRankTotalWinCnt", "设置排位赛总胜利场次", 54)]
internal class SetRankTotalWinCntCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetRankTotalWinCnt = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetRankTotalWinCnt.iValue = InValue;
	}
}

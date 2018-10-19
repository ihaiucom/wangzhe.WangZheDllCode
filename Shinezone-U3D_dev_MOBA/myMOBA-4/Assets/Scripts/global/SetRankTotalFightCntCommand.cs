using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数值", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetRankTotalFightCnt", "设置排位赛总场次", 53)]
internal class SetRankTotalFightCntCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetRankTotalFightCnt = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetRankTotalFightCnt.iValue = InValue;
	}
}

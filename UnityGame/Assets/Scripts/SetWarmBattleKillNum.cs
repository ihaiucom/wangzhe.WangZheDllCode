using CSProtocol;
using System;

[ArgumentDescription(typeof(uint), "人头数", new object[]
{

}), CheatCommand("关卡/温暖局/SetWarmBattleKillNum", "设置温暖局人头数", 61)]
internal class SetWarmBattleKillNum : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stWarmBattleKillNum = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stWarmBattleKillNum.iValue = InValue;
	}
}

using CSProtocol;
using System;

[ArgumentDescription(typeof(uint), "连输次数", new object[]
{

}), CheatCommand("关卡/温暖局/SetWarmBattleConLoseCnt", "设置温暖局连输次数", 60)]
internal class SetWarmBattleConLoseCnt : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stWarmBattleConLoseCnt = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stWarmBattleConLoseCnt.iValue = InValue;
	}
}

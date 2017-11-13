using CSProtocol;
using System;

[ArgumentDescription(typeof(uint), "今日已进行温暖局场数", new object[]
{

}), CheatCommand("关卡/温暖局/SetTodayWarmCnt", "设置今日已进行温暖局场数", 58)]
internal class SetTodayWarmCnt : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stTodayWarmCnt = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stTodayWarmCnt.iValue = InValue;
	}
}

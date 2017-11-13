using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数值", new object[]
{

}), CheatCommand("英雄/英雄排位赛/SetMMROfNormal", "普通匹配MMR值（范围0-5000）", 22)]
internal class SetMMROfNormalCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetMMROfNormal = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetMMROfNormal.iValue = InValue;
	}
}

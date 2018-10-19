using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/经验等级/SetPvpLevel", "设置PVP等级", 38)]
internal class SetPvpLevelCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetPvpLevel = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetPvpLevel.iValue = InValue;
	}
}

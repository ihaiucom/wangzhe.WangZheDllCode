using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "经验值", new object[]
{

}), CheatCommand("英雄/属性修改/经验等级/AddAcntExp", "设置经验", 4)]
internal class AddAcntExpCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddAcntExp = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddAcntExp.iValue = InValue;
	}
}

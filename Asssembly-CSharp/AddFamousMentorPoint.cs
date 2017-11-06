using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "名师点", new object[]
{

}), CheatCommand("英雄/属性修改/其它/AddMasterPoint", "添加名师点", 87)]
internal class AddFamousMentorPoint : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddPvpExp = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddPvpExp.iValue = InValue;
	}
}

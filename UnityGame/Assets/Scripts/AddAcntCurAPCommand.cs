using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "体力值", new object[]
{

}), CheatCommand("英雄/属性修改/数值/AddAcntCurAP", "设置当前体力值", 6)]
internal class AddAcntCurAPCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddAcntCurAP = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddAcntCurAP.iValue = InValue;
	}
}

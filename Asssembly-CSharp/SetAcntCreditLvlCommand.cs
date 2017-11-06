using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "等级", new object[]
{

}), CheatCommand("英雄/属性修改/数值/SetAcntCreditLvl", "设置玩家信誉积分", 67)]
internal class SetAcntCreditLvlCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stChgAcntCreditValue = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stChgAcntCreditValue.iValue = InValue;
	}
}

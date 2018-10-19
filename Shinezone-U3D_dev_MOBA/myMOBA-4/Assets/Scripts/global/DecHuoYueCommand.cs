using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("工具/游戏/活跃度/Dec_Huoyue", "减活跃值", 57)]
internal class DecHuoYueCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stHuoYueDuOpt = new CSDT_CHEAT_HUOYUEDU();
		CheatCmdRef.stHuoYueDuOpt.iValue = InValue;
		CheatCmdRef.stHuoYueDuOpt.bOpt = 2;
	}
}

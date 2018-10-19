using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("工具/SetMaxFriendCount", "设置最大好友数量", 55)]
internal class SetMaxFriendCount : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetMaxFriendCnt = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stSetMaxFriendCnt.iValue = InValue;
	}
}

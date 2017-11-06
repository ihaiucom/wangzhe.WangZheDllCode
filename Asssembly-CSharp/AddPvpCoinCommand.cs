using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/其它/AddPvpCoin", "加荣誉值", 36)]
internal class AddPvpCoinCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddPvpCoin = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddPvpCoin.iValue = InValue;
	}
}

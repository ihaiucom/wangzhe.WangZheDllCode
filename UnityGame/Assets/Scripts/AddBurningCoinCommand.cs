using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/钱币/AddBurningCoin", "加远征币", 41)]
internal class AddBurningCoinCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddBurningCoin = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddBurningCoin.iValue = InValue;
	}
}

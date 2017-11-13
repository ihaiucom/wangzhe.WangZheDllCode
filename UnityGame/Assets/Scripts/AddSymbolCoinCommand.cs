using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/其它/AddSymbolCoin", "加符文碎片", 48)]
internal class AddSymbolCoinCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddSymbolCoin = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddSymbolCoin.iValue = InValue;
	}
}

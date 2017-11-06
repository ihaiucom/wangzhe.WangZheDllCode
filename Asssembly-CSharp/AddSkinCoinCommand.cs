using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/其它/AddSkinCoin", "加皮肤点", 44)]
internal class AddSkinCoinCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddSkinCoin = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stAddSkinCoin.iValue = InValue;
	}
}

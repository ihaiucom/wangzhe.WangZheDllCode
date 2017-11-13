using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "英雄id", new object[]
{

}), CheatCommand("英雄/解锁/AddHero", "获取英雄", 8)]
internal class AddHeroCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stAddHero = new CSDT_CHEAT_HERO();
		CheatCmdRef.stAddHero.dwHeroID = (uint)InValue;
	}
}

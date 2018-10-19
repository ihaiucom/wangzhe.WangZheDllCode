using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "英雄id", new object[]
{

}), CheatCommand("英雄/解锁/HeroSleep", "重置英雄觉醒", 52)]
internal class HeroSleepCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stHeroSleep = new CSDT_CHEAT_HERO();
		CheatCmdRef.stHeroSleep.dwHeroID = (uint)InValue;
	}
}

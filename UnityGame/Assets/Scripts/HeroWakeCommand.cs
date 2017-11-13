using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "英雄id", new object[]
{

}), CheatCommand("英雄/解锁/HeroWake", "一键英雄觉醒", 51)]
internal class HeroWakeCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stHeroWake = new CSDT_CHEAT_HERO();
		CheatCmdRef.stHeroWake.dwHeroID = (uint)InValue;
	}
}

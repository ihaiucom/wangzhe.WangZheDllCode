using CSProtocol;
using System;

[ArgumentDescription(0, typeof(int), "HeroID", new object[]
{

}), CheatCommand("英雄/属性/SetSkillLvlMax", "升满技能等级", 43)]
internal class SetSkillLvlMaxCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		CheatCmdRef.stSetSkillLvlMax = new CSDT_CHEAT_SET_SKILLLVL_MAX();
		CheatCmdRef.stSetSkillLvlMax.dwHeroID = (uint)dwHeroID;
		return CheatCommandBase.Done;
	}
}

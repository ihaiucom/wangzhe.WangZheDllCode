using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "战队周积分", new object[]
{

}), CheatCommand("通用/战队/SetGuildWeekScore", "设置战队战队赛周积分(需要重登陆)", 40)]
internal class SetGuildWeekScore : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildInfo = new CSDT_CHEAT_SET_GUILD_INFO();
		CheatCmdRef.stSetGuildInfo.iActive = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchScore = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekScore = InValue;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekRankNo = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchSeasonRankNo = -1;
	}
}

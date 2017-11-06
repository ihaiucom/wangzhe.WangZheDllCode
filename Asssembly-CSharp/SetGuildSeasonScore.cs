using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "战队赛季积分", new object[]
{

}), CheatCommand("通用/战队/SetGuildSeasonScore", "设置战队战队赛赛季积分(需要重登陆)", 40)]
internal class SetGuildSeasonScore : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildInfo = new CSDT_CHEAT_SET_GUILD_INFO();
		CheatCmdRef.stSetGuildInfo.iActive = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchScore = InValue;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekScore = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekRankNo = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchSeasonRankNo = -1;
	}
}

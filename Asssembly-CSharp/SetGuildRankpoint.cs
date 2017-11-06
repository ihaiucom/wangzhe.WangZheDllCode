using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "战队活跃点(竞技点)", new object[]
{

}), CheatCommand("通用/战队/SetGuildRankpoint", "设置战队活跃点(竞技点)", 40)]
internal class SetGuildRankpoint : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildInfo = new CSDT_CHEAT_SET_GUILD_INFO();
		CheatCmdRef.stSetGuildInfo.iActive = InValue;
		CheatCmdRef.stSetGuildInfo.iGuildMatchScore = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekScore = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchWeekRankNo = -1;
		CheatCmdRef.stSetGuildInfo.iGuildMatchSeasonRankNo = -1;
	}
}

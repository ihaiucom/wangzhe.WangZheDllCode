using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "个人赛季活跃点(竞技点)", new object[]
{

}), CheatCommand("通用/战队/SetGuildMemberTotalRankpoint", "设置个人赛季活跃点(竞技点)", 63)]
internal class SetGuildMemberTotalRankpoint : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildMemberInfo = new CSDT_CHEAT_SET_GUILD_MEMBER_INFO();
		CheatCmdRef.stSetGuildMemberInfo.iGuildCoin = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchWeekScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iContinueWin = -1;
		CheatCmdRef.stSetGuildMemberInfo.iWeekMatchCnt = -1;
		CheatCmdRef.stSetGuildMemberInfo.iTotalRankPoint = InValue;
		CheatCmdRef.stSetGuildMemberInfo.iWeekRankPoint = -1;
	}
}

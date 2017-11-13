using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "个人战队赛周比赛场数", new object[]
{

}), CheatCommand("通用/战队/SetGuildMemberWeekMatchCnt", "设置个人战队赛周比赛场数(需要重登陆)", 63)]
internal class SetGuildMemberWeekMatchCnt : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildMemberInfo = new CSDT_CHEAT_SET_GUILD_MEMBER_INFO();
		CheatCmdRef.stSetGuildMemberInfo.iGuildCoin = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchWeekScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iContinueWin = -1;
		CheatCmdRef.stSetGuildMemberInfo.iWeekMatchCnt = InValue;
		CheatCmdRef.stSetGuildMemberInfo.iTotalRankPoint = -1;
		CheatCmdRef.stSetGuildMemberInfo.iWeekRankPoint = -1;
	}
}

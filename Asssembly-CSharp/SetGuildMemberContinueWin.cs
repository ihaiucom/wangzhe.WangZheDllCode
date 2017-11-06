using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "个人战队赛连胜场数", new object[]
{

}), CheatCommand("通用/战队/SetGuildMemberContinueWin", "设置个人战队赛连胜场数(需要重登陆)", 63)]
internal class SetGuildMemberContinueWin : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CheatCmdRef.stSetGuildMemberInfo = new CSDT_CHEAT_SET_GUILD_MEMBER_INFO();
		CheatCmdRef.stSetGuildMemberInfo.iGuildCoin = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iGuildMatchWeekScore = -1;
		CheatCmdRef.stSetGuildMemberInfo.iContinueWin = InValue;
		CheatCmdRef.stSetGuildMemberInfo.iWeekMatchCnt = -1;
		CheatCmdRef.stSetGuildMemberInfo.iTotalRankPoint = -1;
		CheatCmdRef.stSetGuildMemberInfo.iWeekRankPoint = -1;
	}
}

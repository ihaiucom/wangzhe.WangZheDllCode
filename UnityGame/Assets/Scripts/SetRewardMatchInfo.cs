using CSProtocol;
using System;

[ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 5, typeof(int), "全胜次数", "-1", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 2, typeof(int), "胜场数", "-1", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 4, typeof(int), "比赛轮数", "-1", new object[]
{

}), ArgumentDescription(1, typeof(uint), "地图ID", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 3, typeof(int), "败场数", "-1", new object[]
{

}), CheatCommand("通用/赏金赛/SetRewardMatchPoint", "变更赏金赛信息", 73)]
internal class SetRewardMatchInfo : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stChgRewardMatchInfo = new CSDT_CHEAT_CHG_REWARDMATCH_INFO();
		CheatCmdRef.stChgRewardMatchInfo.dwMapId = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		CheatCmdRef.stChgRewardMatchInfo.iWinCnt = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		CheatCmdRef.stChgRewardMatchInfo.iLossCnt = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stChgRewardMatchInfo.iMatchCnt = CheatCommandBase.SmartConvert<int>(InArguments[3]);
		CheatCmdRef.stChgRewardMatchInfo.iPerfectCnt = CheatCommandBase.SmartConvert<int>(InArguments[4]);
		return CheatCommandBase.Done;
	}
}

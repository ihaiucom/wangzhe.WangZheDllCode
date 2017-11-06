using CSProtocol;
using System;

[ArgumentDescription(0, typeof(byte), "地图人数", new object[]
{

}), ArgumentDescription(1, typeof(uint), "比赛场数", new object[]
{

}), CheatCommand("关卡/温暖局/SetWarmBattleCnt", "设置温暖局地图比赛场数", 59)]
internal class SetWarmBattleCnt : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stWarmBattleCnt = new CSDT_CHEAT_WARMBATTLE_CNT();
		CheatCmdRef.stWarmBattleCnt.bMapMemberNum = CheatCommandBase.SmartConvert<byte>(InArguments[0]);
		CheatCmdRef.stWarmBattleCnt.dwBattleNum = CheatCommandBase.SmartConvert<uint>(InArguments[1]);
		return CheatCommandBase.Done;
	}
}

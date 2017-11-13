using CSProtocol;
using System;

[ArgumentDescription(3, typeof(uint), "败场", new object[]
{

}), ArgumentDescription(11, typeof(uint), "胜利MVP", new object[]
{

}), ArgumentDescription(5, typeof(ulong), "总输出总和", new object[]
{

}), ArgumentDescription(4, typeof(ulong), "KDA总和", new object[]
{

}), ArgumentDescription(6, typeof(ulong), "承受伤害总和", new object[]
{

}), ArgumentDescription(9, typeof(uint), "四杀总和", new object[]
{

}), ArgumentDescription(10, typeof(uint), "五杀总和", new object[]
{

}), ArgumentDescription(2, typeof(uint), "胜场", new object[]
{

}), ArgumentDescription(1, typeof(EGameType), "比赛类型", new object[]
{

}), ArgumentDescription(0, typeof(byte), "英雄ID", new object[]
{

}), ArgumentDescription(7, typeof(ulong), "对建筑伤害总和", new object[]
{

}), ArgumentDescription(8, typeof(uint), "三杀总和", new object[]
{

}), ArgumentDescription(12, typeof(uint), "失败MVP", new object[]
{

}), CheatCommand("英雄/历史战绩/AddCommonHero", "增加常用英雄统计", 77)]
internal class AddCommonHero : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stAddGameStatistic = new CSDT_CHEAT_ADD_GAME_STATISTIC();
		CheatCmdRef.stAddGameStatistic.dwHeroID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.bGameType = (byte)CheatCommandBase.SmartConvert<EGameType>(InArguments[1]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwWinNum = CheatCommandBase.SmartConvert<uint>(InArguments[2]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwLoseNum = CheatCommandBase.SmartConvert<uint>(InArguments[3]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.ullKDAPct = CheatCommandBase.SmartConvert<ulong>(InArguments[4]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.ullTotalHurt = CheatCommandBase.SmartConvert<ulong>(InArguments[5]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.ullTotalBeHurt = CheatCommandBase.SmartConvert<ulong>(InArguments[6]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.ullTotalHurtOrgan = CheatCommandBase.SmartConvert<ulong>(InArguments[7]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwTripleKill = CheatCommandBase.SmartConvert<uint>(InArguments[8]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwUltraKill = CheatCommandBase.SmartConvert<uint>(InArguments[9]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwRampage = CheatCommandBase.SmartConvert<uint>(InArguments[10]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwMvp = CheatCommandBase.SmartConvert<uint>(InArguments[11]);
		CheatCmdRef.stAddGameStatistic.stStatisticInfo.dwLoseSoul = CheatCommandBase.SmartConvert<uint>(InArguments[12]);
		return CheatCommandBase.Done;
	}
}

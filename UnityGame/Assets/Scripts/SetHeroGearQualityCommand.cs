using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "装备品质", new object[]
{

}), ArgumentDescription(2, typeof(int), "装备品阶", new object[]
{

}), ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), CheatCommand("英雄/属性修改/数值/SetGearQyality", "设置所有装备品阶", 45)]
internal class SetHeroGearQualityCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int dwHeroID = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		int iQuality = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		int iSubQuality = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stGearAdvAll = new CSDT_CHEAT_GEARADV_ALL();
		CheatCmdRef.stGearAdvAll.dwHeroID = (uint)dwHeroID;
		CheatCmdRef.stGearAdvAll.iQuality = iQuality;
		CheatCmdRef.stGearAdvAll.iSubQuality = iSubQuality;
		return CheatCommandBase.Done;
	}
}

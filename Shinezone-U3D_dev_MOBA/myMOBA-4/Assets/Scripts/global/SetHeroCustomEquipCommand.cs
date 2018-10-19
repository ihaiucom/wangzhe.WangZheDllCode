using CSProtocol;
using System;

[ArgumentDescription(6, typeof(int), "装备6", new object[]
{

}), ArgumentDescription(8, typeof(int), "1胜2负", new object[]
{

}), ArgumentDescription(5, typeof(int), "装备5", new object[]
{

}), ArgumentDescription(4, typeof(int), "装备4", new object[]
{

}), ArgumentDescription(2, typeof(int), "装备2", new object[]
{

}), ArgumentDescription(1, typeof(int), "装备1", new object[]
{

}), ArgumentDescription(3, typeof(int), "装备3", new object[]
{

}), ArgumentDescription(7, typeof(int), "强制刷新0或1", new object[]
{

}), ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), CheatCommand("英雄/属性/SetHeroCustomEquip", "设置英雄自定义装备", 64)]
internal class SetHeroCustomEquipCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stSetHeroCustomEquip = new CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP();
		CheatCmdRef.stSetHeroCustomEquip.dwHeroID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		CheatCmdRef.stSetHeroCustomEquip.bForceRefresh = CheatCommandBase.SmartConvert<byte>(InArguments[7]);
		CheatCmdRef.stSetHeroCustomEquip.bGameResult = CheatCommandBase.SmartConvert<byte>(InArguments[8]);
		CheatCmdRef.stSetHeroCustomEquip.stEquipInfo.dwEquipNum = 6u;
		for (int i = 0; i < 6; i++)
		{
			CheatCmdRef.stSetHeroCustomEquip.stEquipInfo.EquipID[i] = CheatCommandBase.SmartConvert<uint>(InArguments[i + 1]);
		}
		return CheatCommandBase.Done;
	}
}

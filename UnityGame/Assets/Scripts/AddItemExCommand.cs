using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "ID", new object[]
{

}), ArgumentDescription(2, typeof(int), "数量", new object[]
{

}), ArgumentDescription(0, typeof(int), "物品类型", new object[]
{

}), CheatCommand("英雄/属性/AddItemEx", "添加物品旧版", 7)]
internal class AddItemExCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		int dwItemID = CheatCommandBase.SmartConvert<int>(InArguments[1]);
		int num2 = CheatCommandBase.SmartConvert<int>(InArguments[2]);
		CheatCmdRef.stAddItem = new CSDT_CHEAT_ITEMINFO();
		CheatCmdRef.stAddItem.wItemType = (ushort)num;
		CheatCmdRef.stAddItem.dwItemID = (uint)dwItemID;
		CheatCmdRef.stAddItem.wItemCnt = (ushort)num2;
		return CheatCommandBase.Done;
	}
}

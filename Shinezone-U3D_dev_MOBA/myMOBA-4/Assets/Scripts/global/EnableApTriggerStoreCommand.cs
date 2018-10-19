using CSProtocol;
using System;

[CheatCommand("通用/商城/EnableApTriggerStore", "开启体力刷新商店", 34)]
internal class EnableApTriggerStoreCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stOpenAPRefreshShop = new CSDT_CHEAT_SHOPTYPE();
		CheatCmdRef.stOpenAPRefreshShop.wShopType = 2;
		return CheatCommandBase.Done;
	}
}

using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "数量", new object[]
{

}), CheatCommand("英雄/属性修改/钱币/BuyCoupons", "购买点券", 2)]
internal class BuyDiamondCommand : CommonValueChangeCommand
{
	protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		if (masterRoleInfo != null && (ulong)masterRoleInfo.UInt32ChgAdjust((uint)masterRoleInfo.DianQuan, InValue) > 2147483647uL)
		{
			DebugHelper.Assert(false, "超过点券最大值Int32.MaxValue！");
			return;
		}
		CheatCmdRef.stBuyCoupons = new CSDT_CHEAT_COMVAL();
		CheatCmdRef.stBuyCoupons.iValue = InValue;
	}
}

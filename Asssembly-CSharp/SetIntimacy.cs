using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[ArgumentDescription(3, typeof(ushort), "亲密度值", new object[]
{

}), ArgumentDescription(1, typeof(ulong), "ullUid", new object[]
{

}), ArgumentDescription(2, typeof(uint), "worldID", new object[]
{

}), CheatCommand("工具/SetIntimacy", "设置亲密度", 83)]
internal class SetIntimacy : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		ulong num = CheatCommandBase.SmartConvert<ulong>(InArguments[0]);
		uint num2 = CheatCommandBase.SmartConvert<uint>(InArguments[1]);
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		CheatCmdRef.stChgIntimacy = new CSDT_CHEAT_CHG_INTIMACY();
		CheatCmdRef.stChgIntimacy.stUin.ullUid = ((num == 0uL) ? masterRoleInfo.playerUllUID : num);
		CheatCmdRef.stChgIntimacy.stUin.dwLogicWorldId = (uint)((num2 == 0u) ? masterRoleInfo.logicWorldID : ((int)num2));
		CheatCmdRef.stChgIntimacy.wIntimacyValue = CheatCommandBase.SmartConvert<ushort>(InArguments[2]);
		return CheatCommandBase.Done;
	}
}

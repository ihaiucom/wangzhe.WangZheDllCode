using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

[CheatCommand("关卡/CheatCommandClearNewbieClientBits", "重置新手客户端位（新加的位，以前的位不重置）", 0)]
internal class CheatCommandClearNewbieClientBits : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && Singleton<LobbyLogic>.GetInstance().isLogin)
		{
			for (int i = 100; i < 200; i++)
			{
				masterRoleInfo.SetClientBits(i, false, false);
			}
			masterRoleInfo.SyncClientBitsToSvr();
		}
		return CheatCommandBase.Done;
	}
}

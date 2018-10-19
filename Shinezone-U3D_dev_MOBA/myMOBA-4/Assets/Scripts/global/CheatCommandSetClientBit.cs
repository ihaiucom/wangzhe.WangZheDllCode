using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

[ArgumentDescription(0, typeof(int), "客户端位ID", new object[]
{

}), ArgumentDescription(1, typeof(int), "开启或关闭(1置位0不置)", new object[]
{

}), CheatCommand("关卡/CheatCommandSetClientBit", "设置客户端位", 0)]
internal class CheatCommandSetClientBit : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		int num = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		bool bOpen = CheatCommandBase.SmartConvert<int>(InArguments[1]) == 1;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && Singleton<LobbyLogic>.GetInstance().isLogin)
		{
			if (num <= 0 || num >= 300)
			{
				return "客户端位位ID";
			}
			masterRoleInfo.SetClientBits(num - 100, bOpen, true);
		}
		return CheatCommandBase.Done;
	}
}

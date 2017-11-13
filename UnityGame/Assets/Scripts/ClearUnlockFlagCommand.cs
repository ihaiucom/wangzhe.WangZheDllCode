using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using UnityEngine;

[CheatCommand("工具/ClearUnlockFlag", "清除解锁功能标记", 0)]
internal class ClearUnlockFlagCommand : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() == null)
		{
			DebugHelper.Assert(false, "未获取到角色！");
			return CheatCommandBase.Done;
		}
		int num = 1;
		int num2 = 29;
		for (int i = num; i < num2; i++)
		{
			RES_SPECIALFUNCUNLOCK_TYPE rES_SPECIALFUNCUNLOCK_TYPE = (RES_SPECIALFUNCUNLOCK_TYPE)i;
			PlayerPrefs.SetInt(string.Concat(new object[]
			{
				"FunctionUnlockSys_",
				Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID,
				"_",
				rES_SPECIALFUNCUNLOCK_TYPE.ToString()
			}), 0);
		}
		PlayerPrefs.Save();
		return CheatCommandBase.Done;
	}
}

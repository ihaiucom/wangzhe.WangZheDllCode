using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;

[ArgumentDescription(0, typeof(int), "英雄id", new object[]
{

}), CheatCommand("英雄/解锁/UnlockPvPHero", "解锁PvP英雄(0表示所有)", 33)]
internal class UnlockPvPHero : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		string empty = string.Empty;
		if (this.CheckArguments(InArguments, out empty))
		{
			CheatCmdRef.stUnlockHeroPVPMask = new CSDT_CHEAT_UNLOCK_HEROPVPMASK();
			CheatCmdRef.stUnlockHeroPVPMask.dwHeroID = (uint)CheatCommandBase.SmartConvert<int>(InArguments[0]);
			return CheatCommandBase.Done;
		}
		return empty;
	}

	public override bool CheckArguments(string[] InArguments, out string OutMessage)
	{
		if (!base.CheckArguments(InArguments, out OutMessage))
		{
			return false;
		}
		bool flag = false;
		uint num = (uint)CheatCommandBase.SmartConvert<int>(InArguments[0]);
		if (num == 0u)
		{
			flag = true;
		}
		else
		{
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfoDic().GetEnumerator();
			while (enumerator.MoveNext())
			{
				uint arg_51_0 = num;
				KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
				if (arg_51_0 == current.Key)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			OutMessage = "错误的英雄ID";
			return false;
		}
		return true;
	}
}

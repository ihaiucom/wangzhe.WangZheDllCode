using Assets.Scripts.GameLogic;
using System;

[CheatCommand("工具/KillSoldiers", "死亡之指", 0)]
internal class KillSoldiersCommand : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		Singleton<GameObjMgr>.GetInstance().KillSoldiers();
		return CheatCommandBase.Done;
	}
}

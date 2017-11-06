using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("Fow/SwitchToCampOne", "切换至阵营一观战视角", 0)]
internal class CheatCommandSwitchToCampOne : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		Singleton<WatchController>.instance.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
		return CheatCommandBase.Done;
	}
}

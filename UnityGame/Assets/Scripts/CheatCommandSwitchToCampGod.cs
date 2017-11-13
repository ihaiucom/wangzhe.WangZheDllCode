using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("Fow/SwitchToCampGod", "切换至上帝观战视角", 0)]
internal class CheatCommandSwitchToCampGod : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		Singleton<WatchController>.instance.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
		return CheatCommandBase.Done;
	}
}

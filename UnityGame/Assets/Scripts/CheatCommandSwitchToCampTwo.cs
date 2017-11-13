using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[CheatCommand("Fow/SwitchToCampTwo", "切换至阵营二观战视角", 0)]
internal class CheatCommandSwitchToCampTwo : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		Singleton<WatchController>.instance.SwitchObserveCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
		return CheatCommandBase.Done;
	}
}

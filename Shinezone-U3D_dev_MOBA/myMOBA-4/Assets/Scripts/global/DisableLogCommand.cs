using System;

[CheatCommand("工具/DisableLog", "关闭日志", 0)]
internal class DisableLogCommand : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		DebugHelper.enableLog = false;
		return CheatCommandBase.Done;
	}
}

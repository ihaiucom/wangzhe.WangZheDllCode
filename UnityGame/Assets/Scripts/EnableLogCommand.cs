using System;

[CheatCommand("工具/EnableLog", "打开日志", 0)]
internal class EnableLogCommand : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		DebugHelper.enableLog = true;
		return CheatCommandBase.Done;
	}
}

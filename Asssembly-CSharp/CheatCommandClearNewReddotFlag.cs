using System;
using UnityEngine;

[CheatCommand("工具/ClearNewReddotFlag", "清除New标志标记", 0)]
internal class CheatCommandClearNewReddotFlag : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		PlayerPrefs.DeleteAll();
		return CheatCommandBase.Done;
	}
}

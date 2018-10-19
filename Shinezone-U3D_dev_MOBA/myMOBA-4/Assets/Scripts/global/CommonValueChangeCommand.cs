using CSProtocol;
using System;

internal abstract class CommonValueChangeCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		DebugHelper.Assert(InArguments.Length > 0);
		int inValue = CheatCommandBase.SmartConvert<int>(InArguments[0]);
		this.FillMessageField(ref CheatCmdRef, inValue);
		return CheatCommandBase.Done;
	}

	protected abstract void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue);
}

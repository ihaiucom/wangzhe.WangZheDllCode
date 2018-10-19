using System;

public interface ICheatCommand
{
	CheatCommandName command
	{
		get;
	}

	string comment
	{
		get;
	}

	ArgumentDescriptionAttribute[] argumentsTypes
	{
		get;
	}

	string description
	{
		get;
	}

	string fullyHelper
	{
		get;
	}

	bool isSupportInEditor
	{
		get;
	}

	bool isHiddenInMobile
	{
		get;
	}

	string[] arguments
	{
		get;
	}

	string StartProcess(string[] InArguments);

	bool CheckArguments(string[] InArguments, out string OutMessage);
}

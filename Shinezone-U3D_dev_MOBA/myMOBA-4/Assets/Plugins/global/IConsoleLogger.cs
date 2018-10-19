using System;

public interface IConsoleLogger
{
	string message
	{
		get;
	}

	void AddMessage(string InMessage);

	void Clear();
}

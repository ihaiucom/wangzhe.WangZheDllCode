using System;

public class MobileLogger : IConsoleLogger
{
	private string Message = string.Empty;

	public string message
	{
		get
		{
			return this.Message;
		}
	}

	public void AddMessage(string InMessage)
	{
		this.Message = InMessage;
	}

	public void Clear()
	{
		this.Message = string.Empty;
	}
}

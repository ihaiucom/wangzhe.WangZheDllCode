using System;

public class CheatCommandEntryAttribute : AutoRegisterAttribute
{
	public string group
	{
		get;
		protected set;
	}

	public CheatCommandEntryAttribute(string InGroup)
	{
		this.group = InGroup;
	}
}

using System;

public class AutoSingletonAttribute : Attribute
{
	public bool bAutoCreate;

	public AutoSingletonAttribute(bool bCreate)
	{
		this.bAutoCreate = bCreate;
	}
}

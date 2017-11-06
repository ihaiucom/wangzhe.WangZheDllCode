using System;

public class ArgumentAttribute : AutoRegisterAttribute
{
	public int order
	{
		get;
		protected set;
	}

	public ArgumentAttribute(int InOrder)
	{
		this.order = InOrder;
	}
}

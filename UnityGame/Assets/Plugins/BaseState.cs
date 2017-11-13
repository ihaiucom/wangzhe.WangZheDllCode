using System;

public abstract class BaseState : IState
{
	public virtual string name
	{
		get
		{
			return base.GetType().get_Name();
		}
	}

	public virtual void OnStateEnter()
	{
	}

	public virtual void OnStateLeave()
	{
	}

	public virtual void OnStateOverride()
	{
	}

	public virtual void OnStateResume()
	{
	}
}

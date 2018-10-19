using System;

public interface IState
{
	string name
	{
		get;
	}

	void OnStateEnter();

	void OnStateLeave();

	void OnStateOverride();

	void OnStateResume();
}

using System;
using UnityEngine;

public interface IConsoleView
{
	IConsoleLogger logger
	{
		get;
	}

	void Awake();

	void OnEnable();

	void OnDisable();

	void OnEnter();

	Rect SelectWindowRect();

	void OnConsole(int InWindowID);

	void OnToggleVisible(bool bVisible);

	void OnDestory();

	void OnUpdate();
}

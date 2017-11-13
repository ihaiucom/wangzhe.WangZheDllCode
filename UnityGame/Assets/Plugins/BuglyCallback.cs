using System;
using UnityEngine;

public abstract class BuglyCallback
{
	public abstract void OnApplicationLogCallbackHandler(string condition, string stackTrace, LogType type);
}

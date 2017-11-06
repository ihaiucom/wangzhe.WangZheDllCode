using System;
using UnityEngine;

public class SceneManagerHelper
{
	public static string ActiveSceneName
	{
		get
		{
			return Application.loadedLevelName;
		}
	}

	public static int ActiveSceneBuildIndex
	{
		get
		{
			return Application.loadedLevel;
		}
	}
}

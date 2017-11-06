using System;
using UnityEngine;

public static class GameObjectExtensions
{
	public static bool GetActive(this GameObject target)
	{
		return target.activeInHierarchy;
	}
}

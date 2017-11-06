using System;
using UnityEngine;

public static class GameObjectExtension
{
	public static void CustomSetActive(this GameObject obj, bool bActive)
	{
		if (obj != null && obj.activeSelf != bActive)
		{
			obj.SetActive(bActive);
		}
	}
}

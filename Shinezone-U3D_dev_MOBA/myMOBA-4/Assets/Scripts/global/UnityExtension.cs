using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtension
{
	public delegate bool FindChildDelegate(GameObject obj);

	public static List<GameObject> bfsList = new List<GameObject>();

	public static GameObject FindChildBFS(this GameObject gameObj, UnityExtension.FindChildDelegate findChild)
	{
		if (gameObj == null || findChild == null)
		{
			return null;
		}
		UnityExtension.bfsList.Add(gameObj);
		GameObject result = null;
		int i = 0;
		while (i < UnityExtension.bfsList.Count)
		{
			GameObject gameObject = UnityExtension.bfsList[i++];
			if (findChild(gameObject))
			{
				result = gameObject;
				break;
			}
			int childCount = gameObject.transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
				UnityExtension.bfsList.Add(gameObject2);
			}
		}
		UnityExtension.bfsList.Clear();
		return result;
	}

	public static GameObject FindChildBFS(this GameObject gameObj, string name)
	{
		if (gameObj == null)
		{
			return null;
		}
		UnityExtension.bfsList.Add(gameObj);
		GameObject result = null;
		int i = 0;
		while (i < UnityExtension.bfsList.Count)
		{
			GameObject gameObject = UnityExtension.bfsList[i++];
			if (gameObject.name == name)
			{
				result = gameObject;
				break;
			}
			int childCount = gameObject.transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
				UnityExtension.bfsList.Add(gameObject2);
			}
		}
		UnityExtension.bfsList.Clear();
		return result;
	}
}

using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class Extensions
{
	public static Dictionary<MethodInfo, ParameterInfo[]> ParametersOfMethods = new Dictionary<MethodInfo, ParameterInfo[]>();

	public static ParameterInfo[] GetCachedParemeters(this MethodInfo mo)
	{
		ParameterInfo[] parameters;
		if (!Extensions.ParametersOfMethods.TryGetValue(mo, ref parameters))
		{
			parameters = mo.GetParameters();
			Extensions.ParametersOfMethods.set_Item(mo, parameters);
		}
		return parameters;
	}

	public static PhotonView[] GetPhotonViewsInChildren(this GameObject go)
	{
		return go.GetComponentsInChildren<PhotonView>(true);
	}

	public static PhotonView GetPhotonView(this GameObject go)
	{
		return go.GetComponent<PhotonView>();
	}

	public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
	{
		return Quaternion.Angle(target, second) < maxAngle;
	}

	public static bool AlmostEquals(this float target, float second, float floatDiff)
	{
		return Mathf.Abs(target - second) < floatDiff;
	}

	public static void Merge(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		using (IEnumerator enumerator = addHash.get_Keys().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.get_Current();
				target.set_Item(current, addHash.get_Item(current));
			}
		}
	}

	public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		using (IEnumerator enumerator = addHash.get_Keys().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.get_Current();
				if (current is string)
				{
					target.set_Item(current, addHash.get_Item(current));
				}
			}
		}
	}

	public static string ToStringFull(this IDictionary origin)
	{
		return SupportClass.DictionaryToString(origin, false);
	}

	public static string ToStringFull(this object[] data)
	{
		if (data == null)
		{
			return "null";
		}
		string[] array = new string[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			object obj = data[i];
			array[i] = ((obj == null) ? "null" : obj.ToString());
		}
		return string.Join(", ", array);
	}

	public static Hashtable StripToStringKeys(this IDictionary original)
	{
		Hashtable hashtable = new Hashtable();
		if (original != null)
		{
			using (IEnumerator enumerator = original.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.get_Current();
					if (current is string)
					{
						hashtable.set_Item(current, original.get_Item(current));
					}
				}
			}
		}
		return hashtable;
	}

	public static void StripKeysWithNullValues(this IDictionary original)
	{
		object[] array = new object[original.get_Count()];
		int num = 0;
		using (IEnumerator enumerator = original.get_Keys().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.get_Current();
				array[num++] = current;
			}
		}
		for (int i = 0; i < array.Length; i++)
		{
			object obj = array[i];
			if (original.get_Item(obj) == null)
			{
				original.Remove(obj);
			}
		}
	}

	public static bool Contains(this int[] target, int nr)
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i] == nr)
			{
				return true;
			}
		}
		return false;
	}
}

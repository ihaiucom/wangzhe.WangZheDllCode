using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AkTriggerBase : MonoBehaviour
{
	public delegate void Trigger(GameObject in_gameObject);

	public AkTriggerBase.Trigger triggerDelegate;

	public static Dictionary<uint, string> GetAllDerivedTypes()
	{
		Type typeFromHandle = typeof(AkTriggerBase);
		Type[] types = typeFromHandle.Assembly.GetTypes();
		Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i].IsClass && (types[i].IsSubclassOf(typeFromHandle) || (typeFromHandle.IsAssignableFrom(types[i]) && typeFromHandle != types[i])))
			{
				string name = types[i].Name;
				dictionary.Add(AkUtilities.ShortIDGenerator.Compute(name), name);
			}
		}
		dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
		dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
		dictionary.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");
		return dictionary;
	}
}

using System;
using UnityEngine;

public class FriendlyName : PropertyAttribute
{
	public string friendlyName
	{
		get;
		protected set;
	}

	public FriendlyName(string InDisplayName)
	{
		this.friendlyName = InDisplayName;
	}
}

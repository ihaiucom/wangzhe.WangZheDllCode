using System;
using UnityEngine;

public class stResourceInfo
{
	private static ListView<string> s_extArray = new ListView<string>();

	private sbyte m_extensionIndex = -1;

	public byte m_flags;

	public int m_hashCode;

	public string m_fullPathInResourcesWithoutExtension_Renamed;

	public bool isRenamed
	{
		get
		{
			return this.m_fullPathInResourcesWithoutExtension_Renamed != null;
		}
	}

	public string extension
	{
		get
		{
			if ((int)this.m_extensionIndex == -1)
			{
				return null;
			}
			return stResourceInfo.s_extArray[(int)this.m_extensionIndex];
		}
		set
		{
			if (value == null)
			{
				return;
			}
			for (int i = 0; i < stResourceInfo.s_extArray.Count; i++)
			{
				if (value.Equals(stResourceInfo.s_extArray[i], StringComparison.OrdinalIgnoreCase))
				{
					this.m_extensionIndex = (sbyte)i;
					return;
				}
			}
			if (stResourceInfo.s_extArray.Count + 1 >= 127)
			{
				Debug.LogError("Data type overflow!!!!");
				throw new Exception("Data type overflow!!!!");
			}
			this.m_extensionIndex = (sbyte)stResourceInfo.s_extArray.Count;
			stResourceInfo.s_extArray.Add(value);
		}
	}
}

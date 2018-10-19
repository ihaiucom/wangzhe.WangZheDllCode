using System;
using UnityEngine;

public static class StringExtension
{
	public static readonly string asset_str = "Assets/";

	public static string RemoveExtension(this string s)
	{
		if (s == null)
		{
			return null;
		}
		int num = s.LastIndexOf('.');
		if (num == -1)
		{
			return s;
		}
		return s.Substring(0, num);
	}

	public static string FullPathToAssetPath(this string s)
	{
		if (s == null)
		{
			return null;
		}
		string text = StringExtension.asset_str + s.Substring(Application.dataPath.Length + 1);
		return text.Replace('\\', '/');
	}

	public static string AssetPathToFullPath(this string s)
	{
		if (s == null)
		{
			return null;
		}
		if (!s.StartsWith(StringExtension.asset_str))
		{
			return null;
		}
		string str = Application.dataPath;
		str += "/";
		return str + s.Remove(0, StringExtension.asset_str.Length);
	}

	public static string GetFileExtension(this string s)
	{
		int num = s.LastIndexOf('.');
		if (num == -1)
		{
			return null;
		}
		return s.Substring(num + 1);
	}

	public static string GetFileExtensionUpper(this string s)
	{
		string fileExtension = s.GetFileExtension();
		if (fileExtension == null)
		{
			return null;
		}
		return fileExtension.ToUpper();
	}

	public static string GetHierarchyName(this GameObject go)
	{
		if (go == null)
		{
			return "<null>";
		}
		string text = string.Empty;
		while (go != null)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = go.name;
			}
			else
			{
				text = go.name + "." + text;
			}
			Transform parent = go.transform.parent;
			go = ((!(parent != null)) ? null : parent.gameObject);
		}
		return text;
	}

	public static int JavaHashCode(this string s)
	{
		int num = 0;
		int length = s.Length;
		if (length > 0)
		{
			int num2 = 0;
			for (int i = 0; i < length; i++)
			{
				char c = s[num2++];
				num = 31 * num + (int)c;
			}
		}
		return num;
	}

	public static int JavaHashCodeIgnoreCase(this string s)
	{
		int num = 0;
		int length = s.Length;
		if (length > 0)
		{
			int num2 = 0;
			for (int i = 0; i < length; i++)
			{
				char c = s[num2++];
				if (c >= 'A' && c <= 'Z')
				{
					c += ' ';
				}
				num = 31 * num + (int)c;
			}
		}
		return num;
	}
}

using System;
using UnityEngine;

public class HeroMaterialUtility
{
	private const string token_shadow = " (Shadow)";

	private const string token_translucent = " (Translucent)";

	private const string token_occlusion = " (Occlusion)";

	public static bool IsHeroBattleShader(Material m)
	{
		if (m == null || m.shader == null)
		{
			return false;
		}
		int num = m.shader.name.IndexOf("S_Game_Hero/Hero_Battle");
		return num == 0;
	}

	public static void GetShaderProperty(string name, out bool shadow, out bool translucent, out bool occlusion)
	{
		shadow = (name.IndexOf(" (Shadow)") != -1);
		translucent = (name.IndexOf(" (Translucent)") != -1);
		occlusion = (name.IndexOf(" (Occlusion)") != -1);
	}

	public static string MakeShaderName(string name, bool shadow, bool translucent, bool occlusion)
	{
		int num = name.Length;
		int num2 = name.IndexOf(" (Shadow)");
		if (num2 != -1)
		{
			num = Mathf.Min(num2, num);
		}
		num2 = name.IndexOf(" (Translucent)");
		if (num2 != -1)
		{
			num = Mathf.Min(num2, num);
		}
		num2 = name.IndexOf(" (Occlusion)");
		if (num2 != -1)
		{
			num = Mathf.Min(num2, num);
		}
		string text;
		if (num == name.Length)
		{
			text = name;
		}
		else
		{
			text = name.Substring(0, num);
		}
		if (shadow)
		{
			text += " (Shadow)";
		}
		if (translucent)
		{
			text += " (Translucent)";
		}
		if (occlusion)
		{
			text += " (Occlusion)";
		}
		return text;
	}
}

using System;
using UnityEngine;

public class LeanTest
{
	public static int expected;

	private static int tests;

	private static int passes;

	public static void debug(string name, bool didPass, string failExplaination = null)
	{
		float num = LeanTest.printOutLength(name);
		int num2 = 40 - (int)(num * 1.05f);
		string text = string.Empty.PadRight(num2, "_".get_Chars(0));
		string text2 = string.Concat(new string[]
		{
			LeanTest.formatB(name),
			" ",
			text,
			" [ ",
			didPass ? LeanTest.formatC("pass", "green") : LeanTest.formatC("fail", "red"),
			" ]"
		});
		if (!didPass && failExplaination != null)
		{
			text2 = text2 + " - " + failExplaination;
		}
		Debug.Log(text2);
		if (didPass)
		{
			LeanTest.passes++;
		}
		LeanTest.tests++;
		if (LeanTest.tests == LeanTest.expected)
		{
			Debug.Log(string.Concat(new string[]
			{
				LeanTest.formatB("Final Report:"),
				" _____________________ PASSED: ",
				LeanTest.formatBC(string.Empty + LeanTest.passes, "green"),
				" FAILED: ",
				LeanTest.formatBC(string.Empty + (LeanTest.tests - LeanTest.passes), "red"),
				" "
			}));
		}
		else if (LeanTest.tests > LeanTest.expected)
		{
			Debug.Log(LeanTest.formatB("Too many tests for a final report!") + " set LeanTest.expected = " + LeanTest.tests);
		}
	}

	public static string padRight(int len)
	{
		string text = string.Empty;
		for (int i = 0; i < len; i++)
		{
			text += "_";
		}
		return text;
	}

	public static float printOutLength(string str)
	{
		float num = 0f;
		for (int i = 0; i < str.get_Length(); i++)
		{
			if (str.get_Chars(i) == "I".get_Chars(0))
			{
				num += 0.5f;
			}
			else if (str.get_Chars(i) == "J".get_Chars(0))
			{
				num += 0.85f;
			}
			else
			{
				num += 1f;
			}
		}
		return num;
	}

	public static string formatBC(string str, string color)
	{
		return LeanTest.formatC(LeanTest.formatB(str), color);
	}

	public static string formatB(string str)
	{
		return "<b>" + str + "</b>";
	}

	public static string formatC(string str, string color)
	{
		return string.Concat(new string[]
		{
			"<color=",
			color,
			">",
			str,
			"</color>"
		});
	}

	public static void overview()
	{
	}
}

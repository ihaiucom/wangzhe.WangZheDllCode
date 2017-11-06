using System;
using System.Diagnostics;

public class FileLogger
{
	private static object lockThis = new object();

	[Conditional("UNITY_EDITOR")]
	public static void Log(string s)
	{
	}
}

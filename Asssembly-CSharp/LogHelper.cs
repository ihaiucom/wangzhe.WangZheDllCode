using System;
using System.Diagnostics;

public class LogHelper
{
	private static byte[] m_fowVisibleData;

	private static byte[] m_fowBaseData;

	private static byte[] m_fowPermanentLitData;

	private static int[] m_surfCellData;

	[Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
	public static void LogRvo()
	{
	}
}

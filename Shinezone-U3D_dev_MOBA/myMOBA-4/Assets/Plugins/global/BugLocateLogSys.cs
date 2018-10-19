using System;
using System.IO;

public class BugLocateLogSys : Singleton<BugLocateLogSys>
{
	private StreamWriter m_sw;

	private StreamWriter SW
	{
		get
		{
			if (this.m_sw == null)
			{
			}
			return this.m_sw;
		}
	}

	public override void UnInit()
	{
		base.UnInit();
	}

	public void WriteLine<T>(T content)
	{
	}

	public static void Log(string msg)
	{
	}
}

using System;

namespace behaviac
{
	public static class LogManager
	{
		public static void SetLogFilePath(string logFilePath)
		{
		}

		public static void SetFlush(bool bFlush)
		{
		}

		public static void Log(Agent pAgent, string btMsg, EActionResult actionResult, LogMode mode)
		{
		}

		public static void Log(Agent pAgent, string typeName, string varName, string value)
		{
		}

		public static void Log(Agent pAgent, string btMsg, long time)
		{
		}

		public static void Log(LogMode mode, string filterString, string format, params object[] args)
		{
		}

		public static void Log(string format, params object[] args)
		{
		}

		public static void LogWorkspace(string format, params object[] args)
		{
		}

		public static void Warning(string format, params object[] args)
		{
			LogManager.Log(LogMode.ELM_log, "warning", format, args);
		}

		public static void Error(string format, params object[] args)
		{
			LogManager.Log(LogMode.ELM_log, "error", format, args);
		}

		public static void Flush(Agent pAgent)
		{
		}

		public static void Close()
		{
		}
	}
}

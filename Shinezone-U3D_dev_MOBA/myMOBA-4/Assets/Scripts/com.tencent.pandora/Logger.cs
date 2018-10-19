using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace com.tencent.pandora
{
	public class Logger : MonoBehaviour
	{
		private enum LogLevel
		{
			kDEBUG = 1,
			kINFO,
			kWARN,
			kERROR,
			kREPORT
		}

		private class LogUnit
		{
			public Logger.LogLevel level;

			public string msg = string.Empty;
		}

		private static bool isDestroyed = false;

		private static Queue logMsgQueue = Queue.Synchronized(new Queue());

		private static string curLogFileName = string.Empty;

		private static FileStream curLogFileStream = null;

		private static object lockObj = new object();

		private void Start()
		{
		}

		private void Update()
		{
			while (Logger.logMsgQueue.Count > 0)
			{
				Logger.LogUnit logUnit = Logger.logMsgQueue.Dequeue() as Logger.LogUnit;
				PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
				switch (logUnit.level)
				{
				case Logger.LogLevel.kDEBUG:
					if (pandoraImpl.GetIsDebug())
					{
						Logger.WriteLog(logUnit.msg);
					}
					break;
				case Logger.LogLevel.kINFO:
					if (pandoraImpl.GetIsDebug())
					{
						Logger.WriteLog(logUnit.msg);
					}
					break;
				case Logger.LogLevel.kWARN:
					if (pandoraImpl.GetIsDebug())
					{
						Logger.WriteLog(logUnit.msg);
					}
					break;
				case Logger.LogLevel.kERROR:
					if (pandoraImpl.GetIsDebug())
					{
						Logger.WriteLog(logUnit.msg);
					}
					break;
				case Logger.LogLevel.kREPORT:
					if (pandoraImpl.GetIsNetLog())
					{
						NetLogic netLogic = Pandora.Instance.GetNetLogic();
						netLogic.StreamReport(logUnit.msg);
					}
					break;
				}
			}
		}

		private void OnDestroy()
		{
			Logger.isDestroyed = true;
			if (Logger.curLogFileStream != null)
			{
				Logger.curLogFileStream.Close();
			}
		}

		private static void Enqueue(Logger.LogUnit unit)
		{
			Logger.logMsgQueue.Enqueue(unit);
		}

		public static string GetThreadId()
		{
			return string.Empty;
		}

		public static void DEBUG(string formatMsg)
		{
			string str = string.Concat(new object[]
			{
				"D",
				Logger.GetThreadId(),
				" ",
				Logger.GetFileName(),
				":",
				Logger.GetLineNum(),
				":",
				Logger.GetFuncName(),
				" ",
				formatMsg
			});
			string msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kDEBUG,
				msg = msg
			});
		}

		public static void DEBUG_LOGCAT(string formatMsg)
		{
			string str = string.Concat(new object[]
			{
				"D",
				Logger.GetThreadId(),
				" ",
				Logger.GetFileName(),
				":",
				Logger.GetLineNum(),
				":",
				Logger.GetFuncName(),
				" ",
				formatMsg
			});
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
			Debug.Log(text);
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kDEBUG,
				msg = text
			});
		}

		public static void INFO(string formatMsg)
		{
			string str = string.Concat(new object[]
			{
				"I",
				Logger.GetThreadId(),
				" ",
				Logger.GetFileName(),
				":",
				Logger.GetLineNum(),
				":",
				Logger.GetFuncName(),
				" ",
				formatMsg
			});
			string msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kINFO,
				msg = msg
			});
		}

		public static void ERROR(string formatMsg)
		{
			string str = string.Concat(new object[]
			{
				"E",
				Logger.GetThreadId(),
				" ",
				Logger.GetFileName(),
				":",
				Logger.GetLineNum(),
				":",
				Logger.GetFuncName(),
				" ",
				formatMsg
			});
			string msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kERROR,
				msg = msg
			});
		}

		public static void WARN(string formatMsg)
		{
			string str = string.Concat(new object[]
			{
				"W",
				Logger.GetThreadId(),
				" ",
				Logger.GetFileName(),
				":",
				Logger.GetLineNum(),
				":",
				Logger.GetFuncName(),
				" ",
				formatMsg
			});
			string msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kWARN,
				msg = msg
			});
		}

		public static void REPORT(string msg, int reportType, int returnCode)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["str_respara"] = msg;
			dictionary["uint_report_type"] = reportType;
			dictionary["uint_toreturncode"] = returnCode;
			UserData userData = Pandora.Instance.GetUserData();
			dictionary["str_openid"] = userData.sOpenId;
			string msg2 = Json.Serialize(dictionary);
			Logger.Enqueue(new Logger.LogUnit
			{
				level = Logger.LogLevel.kREPORT,
				msg = msg2
			});
		}

		private static void WriteLog(string msg)
		{
			try
			{
				string a = Pandora.Instance.GetLogPath() + "/log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
				if (a != Logger.curLogFileName)
				{
					Logger.curLogFileName = a;
					if (Logger.curLogFileStream != null)
					{
						Logger.curLogFileStream.Close();
						Logger.curLogFileStream = null;
					}
					Logger.curLogFileStream = new FileStream(Logger.curLogFileName, FileMode.Append);
				}
				if (Logger.curLogFileStream == null)
				{
					Logger.curLogFileStream = new FileStream(Logger.curLogFileName, FileMode.Append);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
				Logger.curLogFileStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception var_2_B0)
			{
			}
		}

		private static void SyncWriteLog(string msg)
		{
			if (Logger.isDestroyed)
			{
				return;
			}
			object obj = Logger.lockObj;
			lock (obj)
			{
				try
				{
					string a = Pandora.Instance.GetLogPath() + "/log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
					if (a != Logger.curLogFileName)
					{
						Logger.curLogFileName = a;
						if (Logger.curLogFileStream != null)
						{
							Logger.curLogFileStream.Close();
							Logger.curLogFileStream = null;
						}
						Logger.curLogFileStream = new FileStream(Logger.curLogFileName, FileMode.Append);
					}
					if (Logger.curLogFileStream == null)
					{
						Logger.curLogFileStream = new FileStream(Logger.curLogFileName, FileMode.Append);
					}
					byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
					Logger.curLogFileStream.Write(bytes, 0, bytes.Length);
					Logger.curLogFileStream.Flush();
				}
				catch (Exception var_3_D2)
				{
				}
			}
		}

		private static int GetLineNum()
		{
			int result;
			try
			{
				StackTrace stackTrace = new StackTrace(2, true);
				result = stackTrace.GetFrame(0).GetFileLineNumber();
			}
			catch (Exception var_1_1F)
			{
				result = 0;
			}
			return result;
		}

		private static string GetFileName()
		{
			string result;
			try
			{
				StackTrace stackTrace = new StackTrace(2, true);
				result = stackTrace.GetFrame(0).GetFileName();
			}
			catch (Exception var_1_1F)
			{
				result = "#";
			}
			return result;
		}

		private static string GetFuncName()
		{
			string result;
			try
			{
				StackTrace stackTrace = new StackTrace(2, true);
				result = stackTrace.GetFrame(0).GetMethod().Name;
			}
			catch (Exception var_1_24)
			{
				result = "#";
			}
			return result;
		}
	}
}

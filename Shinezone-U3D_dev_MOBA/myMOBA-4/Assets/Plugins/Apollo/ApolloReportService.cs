using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apollo
{
	public class ApolloReportService : ApolloObject, IApolloReportService, IApolloServiceBase
	{
		public static readonly ApolloReportService Instance = new ApolloReportService();

		private ApolloBuglyLogDelegate m_LogCallback;

		private ApolloReportService()
		{
			Console.WriteLine("ApolloReportService Create With ID:{0}", base.ObjectId);
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool ApolloReportEvent(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string eventName, [MarshalAs(UnmanagedType.LPStr)] string payload, bool isReal);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool ApolloEnableCrashReport(ulong objId, bool rdm, bool mta);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool ApolloReportInit(ulong objId, bool rdm, bool mta);

		public static void RegistExceptionHandler(string text, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tsf4g.apollo.report.UnityException");
				androidJavaClass.CallStatic("CatchException", new object[]
				{
					text + stackTrace
				});
			}
		}

		public void ApolloEnableCrashReport(bool rdm, bool mta)
		{
			ApolloReportService.ApolloEnableCrashReport(base.ObjectId, rdm, mta);
		}

		public void ApolloReportInit(bool rdm = true, bool mta = true)
		{
			ApolloReportService.ApolloReportInit(base.ObjectId, rdm, mta);
		}

		public bool ApolloRepoertEvent(string eventName, List<KeyValuePair<string, string>> events, bool isReal)
		{
			if (eventName == null)
			{
				return false;
			}
			string text = string.Empty;
			if (events != null)
			{
				for (int i = 0; i < events.Count; i++)
				{
					KeyValuePair<string, string> keyValuePair = events[i];
					if (keyValuePair.Key == null)
					{
						ADebug.LogError(string.Format("ApolloReportService e.Key null:eventName{0}", eventName));
					}
					else
					{
						if (keyValuePair.Value == null)
						{
							keyValuePair = new KeyValuePair<string, string>(keyValuePair.Key, string.Empty);
							ADebug.LogError(string.Format("ApolloReportService e.Value null:eventName{0}", eventName));
						}
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							keyValuePair.Key,
							":",
							keyValuePair.Value,
							","
						});
					}
				}
			}
			ApolloReportService.ApolloReportEvent(base.ObjectId, eventName, text, isReal);
			return true;
		}

		[Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
		public void EnableBuglyLog(bool enable)
		{
		}

		public void SetUserId(string userId)
		{
			BuglyAgent.SetUserId(userId);
		}

		public void HandleException(Exception e)
		{
			ADebug.Log("ApolloReportService HandleException:" + e.ToString());
			this.ReportException(e);
		}

		public void ReportException(Exception e)
		{
			ADebug.Log("ApolloReportService ReportException:" + e.ToString());
			BuglyAgent.ReportException(e, string.Empty);
		}

		public void ReportException(Exception e, string message)
		{
			ADebug.Log("ApolloReportService ReportException:" + e.ToString() + " message:" + message);
			BuglyAgent.ReportException(e, message);
		}

		public void ReportException(string name, string message, string stackTrace)
		{
			BuglyAgent.ReportException(name, message, stackTrace);
		}

		[Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
		public void SetGameObjectForCallback(string gameObject)
		{
		}

		public void SetAutoQuitApplication(bool autoExit)
		{
			BuglyAgent.ConfigAutoQuitApplication(autoExit);
		}

		public void EnableExceptionHandler(LogSeverity level)
		{
			BuglyAgent.EnableExceptionHandler();
			BuglyAgent.ConfigAutoReportLogLevel(level);
		}

		public void RegisterLogCallback(ApolloBuglyLogDelegate callback)
		{
			ADebug.Log("Apollo RegisterExceptionHandler");
			if (callback != null)
			{
				this.m_LogCallback = callback;
				BuglyAgent.RegisterLogCallback(new BuglyAgent.LogCallbackDelegate(this.apollo_bugly_log_callback));
			}
			else
			{
				BuglyAgent.RegisterLogCallback(null);
			}
		}

		private void apollo_bugly_log_callback(string log, string stackTrace, LogType type)
		{
			if (this.m_LogCallback != null)
			{
				this.m_LogCallback(log, stackTrace, type);
			}
		}
	}
}

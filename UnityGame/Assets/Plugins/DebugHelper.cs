using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
	private static DebugHelper instance = null;

	private static SLogTypeDef logMode = SLogTypeDef.LogType_Custom;

	private static SLogObj[] s_logers = new SLogObj[8];

	public SLogTypeDef cfgMode = SLogTypeDef.LogType_System;

	public static bool enableLog = true;

	private static string CachedDownloadReplayPath;

	private static string CachedLogRootPath;

	public static string logRootPath
	{
		get
		{
			if (DebugHelper.CachedLogRootPath == null)
			{
				string text = string.Format("{0}/Replay/", Application.persistentDataPath);
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				DebugHelper.CachedLogRootPath = text;
			}
			return DebugHelper.CachedLogRootPath;
		}
	}

	public static string downloadReplayPath
	{
		get
		{
			if (DebugHelper.CachedDownloadReplayPath == null)
			{
				string text = string.Format("{0}/DownloadReplay/", Application.persistentDataPath);
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				DebugHelper.CachedDownloadReplayPath = text;
			}
			return DebugHelper.CachedDownloadReplayPath;
		}
	}

	public static void OpenLoger(SLogCategory logType, string logFile)
	{
		DebugHelper.s_logers[(int)logType].Flush();
		DebugHelper.s_logers[(int)logType].Close();
		DebugHelper.s_logers[(int)logType].TargetPath = logFile;
	}

	public static void CloseLoger(SLogCategory logType)
	{
		DebugHelper.s_logers[(int)logType].Flush();
		DebugHelper.s_logers[(int)logType].Close();
		DebugHelper.s_logers[(int)logType].TargetPath = null;
	}

	public static void BeginLogs()
	{
		DebugHelper.CloseLogs();
		string logRootPath = DebugHelper.logRootPath;
		string text = DateTime.get_Now().ToString("yyyyMMdd_HHmmss");
		DebugHelper.OpenLoger(SLogCategory.Normal, string.Format("{0}/{1}_normal.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Skill, string.Format("{0}/{1}_skill.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Misc, string.Format("{0}/{1}_misc.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Msg, string.Format("{0}/{1}_msg.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Actor, string.Format("{0}/{1}_actor.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Rvo, string.Format("{0}/{1}_rvo.log", logRootPath, text));
		DebugHelper.OpenLoger(SLogCategory.Fow, string.Format("{0}/{1}_fow.log", logRootPath, text));
	}

	public static void CloseLogs()
	{
		DebugHelper.CloseLoger(SLogCategory.Normal);
		DebugHelper.CloseLoger(SLogCategory.Skill);
		DebugHelper.CloseLoger(SLogCategory.Misc);
		DebugHelper.CloseLoger(SLogCategory.Msg);
		DebugHelper.CloseLoger(SLogCategory.Actor);
		DebugHelper.CloseLoger(SLogCategory.Rvo);
		DebugHelper.CloseLoger(SLogCategory.Fow);
	}

	public static void ClearLogs(int passedMinutes = 60)
	{
		DateTime now = DateTime.get_Now();
		DirectoryInfo directoryInfo = new DirectoryInfo(DebugHelper.logRootPath);
		if (directoryInfo.get_Exists())
		{
			string[] files = Directory.GetFiles(directoryInfo.get_FullName(), "*.log", 0);
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					string text = files[i];
					FileInfo fileInfo = new FileInfo(text);
					if (fileInfo.get_Exists() && (now - fileInfo.get_CreationTime()).get_TotalMinutes() > (double)passedMinutes)
					{
						File.Delete(text);
					}
				}
				catch
				{
				}
			}
		}
	}

	public static SLogObj GetLoger(SLogCategory logType)
	{
		return DebugHelper.s_logers[(int)logType];
	}

	public static string GetLogerPath(SLogCategory logType)
	{
		return DebugHelper.s_logers[(int)logType].LastTargetPath;
	}

	[Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR")]
	public static void EditorAssert(bool InCondition, string InFormat = null, params object[] InParameters)
	{
		DebugHelper.Assert(InCondition, InFormat, InParameters);
	}

	[Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
	public static void Assert(bool InCondition)
	{
		DebugHelper.Assert(InCondition, null, null);
	}

	[Conditional("UNITY_ANDROID"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_IPHONE"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR")]
	public static void Assert(bool InCondition, string InFormat)
	{
		DebugHelper.Assert(InCondition, InFormat, null);
	}

	[Conditional("UNITY_IPHONE"), Conditional("UNITY_ANDROID"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR")]
	public static void Assert(bool InCondition, string InFormat, params object[] InParameters)
	{
		if (DebugHelper.enableLog && !InCondition)
		{
			try
			{
				string text = null;
				if (!string.IsNullOrEmpty(InFormat))
				{
					try
					{
						if (InParameters != null)
						{
							text = string.Format(InFormat, InParameters);
						}
						else
						{
							text = InFormat;
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					text = string.Format(" no assert detail, stacktrace is :{0}", Environment.get_StackTrace());
				}
				if (text != null)
				{
					string str = "Assert : " + text;
					DebugHelper.CustomLog(str);
				}
				else
				{
					Debug.LogWarning("Assert failed!");
				}
			}
			catch (Exception)
			{
			}
		}
	}

	[Conditional("UNITY_IPHONE"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR"), Conditional("UNITY_ANDROID")]
	public static void CustomLog(string str, params object[] InParameters)
	{
		if (DebugHelper.enableLog)
		{
			try
			{
				string className = "com.tencent.tmgp.sgame.SGameUtility";
				if (InParameters != null)
				{
					str = string.Format(str, InParameters);
				}
				str = DateTime.get_Now().ToString("T") + " " + str;
				Debug.Log(str);
				AndroidJavaClass androidJavaClass = new AndroidJavaClass(className);
				androidJavaClass.CallStatic("dtLog", new object[]
				{
					str
				});
				androidJavaClass.Dispose();
			}
			catch (Exception)
			{
			}
		}
	}

	[Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_EDITOR"), Conditional("UNITY_IPHONE")]
	public static void CustomLog(string str)
	{
		DebugHelper.CustomLog(str, null);
	}

	[Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN")]
	public static void LogInternal(SLogCategory logType, string logmsg)
	{
		if (DebugHelper.enableLog)
		{
			DebugHelper.s_logers[(int)logType].Log(logmsg);
		}
	}

	[Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN")]
	public static void Log(string logmsg)
	{
		if (!DebugHelper.enableLog || DebugHelper.logMode == SLogTypeDef.LogType_Custom)
		{
		}
	}

	public static string GetDownloadReplayPathWithoutCreate()
	{
		return DebugHelper.CachedDownloadReplayPath;
	}

	[Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
	public static void LogMisc(string logmsg)
	{
		if (!DebugHelper.enableLog || DebugHelper.logMode == SLogTypeDef.LogType_System || DebugHelper.logMode == SLogTypeDef.LogType_Custom)
		{
		}
	}

	[Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
	public static void LogError(string errmsg)
	{
		if (DebugHelper.enableLog)
		{
			Debug.LogError(errmsg);
		}
	}

	[Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
	public static void LogWarning(string warmsg)
	{
		if (DebugHelper.enableLog)
		{
			Debug.LogWarning(warmsg);
		}
	}

	[Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR")]
	public static void ConsoleLog(string logmsg)
	{
		if (DebugHelper.enableLog)
		{
			Debug.Log(logmsg);
		}
	}

	[Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN")]
	public static void ConsoleLogError(string logmsg)
	{
		if (DebugHelper.enableLog)
		{
			Debug.LogError(logmsg);
		}
	}

	[Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
	public static void ConsoleLogWarning(string logmsg)
	{
		if (DebugHelper.enableLog)
		{
			Debug.LogWarning(logmsg);
		}
	}

	private void Awake()
	{
		DebugHelper.Assert(DebugHelper.instance == null);
		DebugHelper.instance = this;
		DebugHelper.logMode = this.cfgMode;
		int num = 8;
		for (int i = 0; i < num; i++)
		{
			DebugHelper.s_logers[i] = new SLogObj();
		}
	}

	protected void OnDestroy()
	{
		int num = 8;
		for (int i = 0; i < num; i++)
		{
			DebugHelper.s_logers[i].Flush();
			DebugHelper.s_logers[i].Close();
		}
		Singleton<BackgroundWorker>.DestroyInstance();
	}

	[Conditional("UNITY_EDITOR")]
	public static void ClearConsole()
	{
	}
}

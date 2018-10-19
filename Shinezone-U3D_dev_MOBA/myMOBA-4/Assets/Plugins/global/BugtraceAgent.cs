using System;
using System.Reflection;
using UnityEngine;

public sealed class BugtraceAgent
{
	private const string SDK_PACKAGE = "com.tencent.tp.bugtrace";

	private static Application.LogCallback s_oldLogCallback;

	private static bool _isInitialized;

	private static readonly string CLASS_UNITYAGENT = "com.tencent.tp.bugtrace.BugtraceAgent";

	private static AndroidJavaObject _unityAgent;

	public static AndroidJavaObject UnityAgent
	{
		get
		{
			if (BugtraceAgent._unityAgent == null)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(BugtraceAgent.CLASS_UNITYAGENT))
				{
					BugtraceAgent._unityAgent = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				}
			}
			return BugtraceAgent._unityAgent;
		}
	}

	public static Application.LogCallback GetCurrentLogCallback()
	{
		Type typeFromHandle = typeof(Application);
		BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		FieldInfo field = typeFromHandle.GetField("s_LogCallback", bindingAttr);
		if (field != null && field.IsPrivate && field.IsStatic)
		{
			object value = field.GetValue(null);
			if (value != null)
			{
				return (Application.LogCallback)value;
			}
		}
		return null;
	}

	public static void EnableExceptionHandler()
	{
		if (BugtraceAgent._isInitialized)
		{
			return;
		}
		BugtraceAgent.RegisterExceptionHandler();
		BugtraceAgent._isInitialized = true;
	}

	public static void DisableExceptionHandler()
	{
		if (!BugtraceAgent._isInitialized)
		{
			return;
		}
		BugtraceAgent.UnregisterExceptionHandler();
		BugtraceAgent._isInitialized = false;
	}

	private static void RegisterExceptionHandler()
	{
		AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(BugtraceAgent.UncaughtExceptionHandler);
		BugtraceAgent.s_oldLogCallback = BugtraceAgent.GetCurrentLogCallback();
		Application.RegisterLogCallback(new Application.LogCallback(BugtraceAgent.LogCallbackHandler));
	}

	private static void UnregisterExceptionHandler()
	{
		AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(BugtraceAgent.UncaughtExceptionHandler);
		Application.RegisterLogCallback(BugtraceAgent.s_oldLogCallback);
	}

	private static void LogCallbackHandler(string condition, string stack, LogType type)
	{
		if (type == LogType.Exception)
		{
			BugtraceAgent.HandleException(condition, stack);
		}
		if (BugtraceAgent.s_oldLogCallback != null)
		{
			BugtraceAgent.s_oldLogCallback(condition, stack, type);
		}
	}

	private static void UncaughtExceptionHandler(object sender, UnhandledExceptionEventArgs args)
	{
		Exception ex = (Exception)args.ExceptionObject;
		if (ex != null)
		{
			BugtraceAgent.HandleException(ex.Message, ex.StackTrace);
		}
	}

	private static void HandleException(string reason, string stack)
	{
		string text = "crash-reportcsharpexception|";
		text = text + "cause:" + reason;
		text = text + "stack:" + stack;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tencent.tp.TssJavaMethod");
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("sendCmd", new object[]
				{
					text
				});
			}
		}
		catch
		{
		}
	}
}

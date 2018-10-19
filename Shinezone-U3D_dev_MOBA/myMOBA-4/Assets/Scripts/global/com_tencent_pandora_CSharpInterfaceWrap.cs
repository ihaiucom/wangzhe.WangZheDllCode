using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class com_tencent_pandora_CSharpInterfaceWrap
{
	private static Type classType = typeof(CSharpInterface);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("GetLogger", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetLogger)),
			new LuaMethod("GetPlatformDesc", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetPlatformDesc)),
			new LuaMethod("GetSDKVersion", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetSDKVersion)),
			new LuaMethod("WriteCookie", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.WriteCookie)),
			new LuaMethod("ReadCookie", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.ReadCookie)),
			new LuaMethod("IOSPay", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.IOSPay)),
			new LuaMethod("AndroidPay", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AndroidPay)),
			new LuaMethod("GetUserData", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetUserData)),
			new LuaMethod("AsyncSetImage", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AsyncSetImage)),
			new LuaMethod("ShowGameImg", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.ShowGameImg)),
			new LuaMethod("ShowGameIcon", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.ShowGameIcon)),
			new LuaMethod("GetGameImgPath", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetGameImgPath)),
			new LuaMethod("ShowGameImgByPath", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.ShowGameImgByPath)),
			new LuaMethod("QueryFriendsList", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.QueryFriendsList)),
			new LuaMethod("QuerySearchConfig", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.QuerySearchConfig)),
			new LuaMethod("QueryRankShowName", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.QueryRankShowName)),
			new LuaMethod("AsyncDownloadImage", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AsyncDownloadImage)),
			new LuaMethod("IsImageDownloaded", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.IsImageDownloaded)),
			new LuaMethod("GetTotalSwitch", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetTotalSwitch)),
			new LuaMethod("GetFunctionSwitch", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetFunctionSwitch)),
			new LuaMethod("CallGame", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.CallGame)),
			new LuaMethod("StreamReport", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.StreamReport)),
			new LuaMethod("CallBroker", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.CallBroker)),
			new LuaMethod("AssembleFont", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AssembleFont)),
			new LuaMethod("CreatePanel", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.CreatePanel)),
			new LuaMethod("DestroyPanel", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.DestroyPanel)),
			new LuaMethod("GetPanel", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetPanel)),
			new LuaMethod("GetPanelParent", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetPanelParent)),
			new LuaMethod("AddClick", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AddClick)),
			new LuaMethod("AddDragEndEvent", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AddDragEndEvent)),
			new LuaMethod("AddUGUIOnClickDown", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AddUGUIOnClickDown)),
			new LuaMethod("AddUGUIOnClickUp", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.AddUGUIOnClickUp)),
			new LuaMethod("ExecCallback", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.ExecCallback)),
			new LuaMethod("DoCmdFromGame", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.DoCmdFromGame)),
			new LuaMethod("NotifyIOSPayFinish", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.NotifyIOSPayFinish)),
			new LuaMethod("NotifyAndroidPayFinish", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.NotifyAndroidPayFinish)),
			new LuaMethod("NotifyPushData", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.NotifyPushData)),
			new LuaMethod("NotifyCloseAllPanel", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.NotifyCloseAllPanel)),
			new LuaMethod("UnloadUnusedAssets", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.UnloadUnusedAssets)),
			new LuaMethod("SetPosition", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.SetPosition)),
			new LuaMethod("SetScale", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.SetScale)),
			new LuaMethod("SetPosZ", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.SetPosZ)),
			new LuaMethod("SetTextString", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.SetTextString)),
			new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap._Createcom_tencent_pandora_CSharpInterface)),
			new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.GetClassType))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("isApplePlatform", new LuaCSFunction(com_tencent_pandora_CSharpInterfaceWrap.get_isApplePlatform), null)
		};
		LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.CSharpInterface", typeof(CSharpInterface), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _Createcom_tencent_pandora_CSharpInterface(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			CSharpInterface o = new CSharpInterface();
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.CSharpInterface.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, com_tencent_pandora_CSharpInterfaceWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_isApplePlatform(IntPtr L)
	{
		LuaScriptMgr.Push(L, CSharpInterface.isApplePlatform);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetLogger(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Logger logger = CSharpInterface.GetLogger();
		LuaScriptMgr.Push(L, logger);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetPlatformDesc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string platformDesc = CSharpInterface.GetPlatformDesc();
		LuaScriptMgr.Push(L, platformDesc);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetSDKVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string sDKVersion = CSharpInterface.GetSDKVersion();
		LuaScriptMgr.Push(L, sDKVersion);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int WriteCookie(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
		bool b = CSharpInterface.WriteCookie(luaString, luaString2);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ReadCookie(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string str = CSharpInterface.ReadCookie(luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IOSPay(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool b = CSharpInterface.IOSPay(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AndroidPay(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool b = CSharpInterface.AndroidPay(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetUserData(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		UserData userData = CSharpInterface.GetUserData();
		LuaScriptMgr.PushObject(L, userData);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AsyncSetImage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
		Image image = (Image)LuaScriptMgr.GetUnityObject(L, 3, typeof(Image));
		uint callId = (uint)LuaScriptMgr.GetNumber(L, 4);
		CSharpInterface.AsyncSetImage(luaString, luaString2, image, callId);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ShowGameImg(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		int djType = (int)LuaScriptMgr.GetNumber(L, 1);
		int djID = (int)LuaScriptMgr.GetNumber(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 3, typeof(GameObject));
		uint callId = (uint)LuaScriptMgr.GetNumber(L, 4);
		CSharpInterface.ShowGameImg(djType, djID, go, callId);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ShowGameIcon(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		int rankClass = (int)LuaScriptMgr.GetNumber(L, 2);
		int rankGrade = (int)LuaScriptMgr.GetNumber(L, 3);
		int d = CSharpInterface.ShowGameIcon(go, rankClass, rankGrade);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetGameImgPath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int rankClass = (int)LuaScriptMgr.GetNumber(L, 1);
		int rankGrade = (int)LuaScriptMgr.GetNumber(L, 2);
		string gameImgPath = CSharpInterface.GetGameImgPath(rankClass, rankGrade);
		LuaScriptMgr.Push(L, gameImgPath);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ShowGameImgByPath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		int d = CSharpInterface.ShowGameImgByPath(go, luaString);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int QueryFriendsList(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string str = CSharpInterface.QueryFriendsList();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int QuerySearchConfig(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string str = CSharpInterface.QuerySearchConfig();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int QueryRankShowName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int rankClass = (int)LuaScriptMgr.GetNumber(L, 1);
		int rankShowGrade = (int)LuaScriptMgr.GetNumber(L, 2);
		string str = CSharpInterface.QueryRankShowName(rankClass, rankShowGrade);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AsyncDownloadImage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.AsyncDownloadImage(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsImageDownloaded(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool b = CSharpInterface.IsImageDownloaded(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTotalSwitch(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool totalSwitch = CSharpInterface.GetTotalSwitch();
		LuaScriptMgr.Push(L, totalSwitch);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetFunctionSwitch(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool functionSwitch = CSharpInterface.GetFunctionSwitch(luaString);
		LuaScriptMgr.Push(L, functionSwitch);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CallGame(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			CSharpInterface.CallGame(luaString);
			return 0;
		}
		if (num == 2)
		{
			uint callId = (uint)LuaScriptMgr.GetNumber(L, 1);
			LuaTable luaTable = LuaScriptMgr.GetLuaTable(L, 2);
			string str = CSharpInterface.CallGame(callId, luaTable);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.CSharpInterface.CallGame");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int StreamReport(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		int reportType = (int)LuaScriptMgr.GetNumber(L, 2);
		int returnCode = (int)LuaScriptMgr.GetNumber(L, 3);
		CSharpInterface.StreamReport(luaString, reportType, returnCode);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CallBroker(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		uint callId = (uint)LuaScriptMgr.GetNumber(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		int cmdId = (int)LuaScriptMgr.GetNumber(L, 3);
		CSharpInterface.CallBroker(callId, luaString, cmdId);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AssembleFont(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
		int d = CSharpInterface.AssembleFont(luaString, luaString2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CreatePanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		uint callId = (uint)LuaScriptMgr.GetNumber(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		CSharpInterface.CreatePanel(callId, luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int DestroyPanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.DestroyPanel(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetPanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject panel = CSharpInterface.GetPanel(luaString);
		LuaScriptMgr.Push(L, panel);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetPanelParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject panelParent = CSharpInterface.GetPanelParent(luaString);
		LuaScriptMgr.Push(L, panelParent);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
		CSharpInterface.AddClick(go, luaFunction);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddDragEndEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
		CSharpInterface.AddDragEndEvent(go, luaFunction);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddUGUIOnClickDown(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
		CSharpInterface.AddUGUIOnClickDown(go, luaFunction);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddUGUIOnClickUp(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
		CSharpInterface.AddUGUIOnClickUp(go, luaFunction);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ExecCallback(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		uint callId = (uint)LuaScriptMgr.GetNumber(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		CSharpInterface.ExecCallback(callId, luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int DoCmdFromGame(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.DoCmdFromGame(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int NotifyIOSPayFinish(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.NotifyIOSPayFinish(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int NotifyAndroidPayFinish(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.NotifyAndroidPayFinish(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int NotifyPushData(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		CSharpInterface.NotifyPushData(luaString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int NotifyCloseAllPanel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		CSharpInterface.NotifyCloseAllPanel();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int UnloadUnusedAssets(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		CSharpInterface.UnloadUnusedAssets();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetPosition(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		float x = (float)LuaScriptMgr.GetNumber(L, 2);
		float y = (float)LuaScriptMgr.GetNumber(L, 3);
		float z = (float)LuaScriptMgr.GetNumber(L, 4);
		CSharpInterface.SetPosition(go, x, y, z);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetScale(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		float x = (float)LuaScriptMgr.GetNumber(L, 2);
		float y = (float)LuaScriptMgr.GetNumber(L, 3);
		float z = (float)LuaScriptMgr.GetNumber(L, 4);
		CSharpInterface.SetScale(go, x, y, z);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetPosZ(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		float z = (float)LuaScriptMgr.GetNumber(L, 2);
		CSharpInterface.SetPosZ(go, z);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetTextString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		GameObject go = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		string luaString2 = LuaScriptMgr.GetLuaString(L, 3);
		CSharpInterface.SetTextString(go, luaString, luaString2);
		return 0;
	}
}

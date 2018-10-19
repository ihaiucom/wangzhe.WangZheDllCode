using com.tencent.pandora;
using System;
using System.Collections.Generic;

public class com_tencent_pandora_UserDataWrap
{
	private static Type classType = typeof(UserData);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("IsRoleEmpty", new LuaCSFunction(com_tencent_pandora_UserDataWrap.IsRoleEmpty)),
			new LuaMethod("Clear", new LuaCSFunction(com_tencent_pandora_UserDataWrap.Clear)),
			new LuaMethod("Assgin", new LuaCSFunction(com_tencent_pandora_UserDataWrap.Assgin)),
			new LuaMethod("Refresh", new LuaCSFunction(com_tencent_pandora_UserDataWrap.Refresh)),
			new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_UserDataWrap._Createcom_tencent_pandora_UserData)),
			new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_UserDataWrap.GetClassType))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("sRoleId", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sRoleId), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sRoleId)),
			new LuaField("sOpenId", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sOpenId), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sOpenId)),
			new LuaField("sServiceType", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sServiceType), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sServiceType)),
			new LuaField("sAcountType", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sAcountType), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sAcountType)),
			new LuaField("sArea", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sArea), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sArea)),
			new LuaField("sPartition", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sPartition), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sPartition)),
			new LuaField("sAppId", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sAppId), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sAppId)),
			new LuaField("sAccessToken", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sAccessToken), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sAccessToken)),
			new LuaField("sPayToken", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sPayToken), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sPayToken)),
			new LuaField("sGameVer", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sGameVer), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sGameVer)),
			new LuaField("sPlatID", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sPlatID), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sPlatID)),
			new LuaField("sQQInstalled", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sQQInstalled), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sQQInstalled)),
			new LuaField("sWXInstalled", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sWXInstalled), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sWXInstalled)),
			new LuaField("sGameName", new LuaCSFunction(com_tencent_pandora_UserDataWrap.get_sGameName), new LuaCSFunction(com_tencent_pandora_UserDataWrap.set_sGameName))
		};
		LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.UserData", typeof(UserData), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _Createcom_tencent_pandora_UserData(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			UserData o = new UserData();
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.UserData.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, com_tencent_pandora_UserDataWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sRoleId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sRoleId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sRoleId on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sRoleId);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sOpenId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sOpenId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sOpenId on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sOpenId);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sServiceType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sServiceType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sServiceType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sServiceType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sAcountType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAcountType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAcountType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sAcountType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sArea(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sArea");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sArea on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sArea);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sPartition(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPartition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPartition on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sPartition);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sAppId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAppId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAppId on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sAppId);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sAccessToken(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAccessToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAccessToken on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sAccessToken);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sPayToken(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPayToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPayToken on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sPayToken);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sGameVer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sGameVer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sGameVer on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sGameVer);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sPlatID(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPlatID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPlatID on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sPlatID);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sQQInstalled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sQQInstalled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sQQInstalled on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sQQInstalled);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sWXInstalled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sWXInstalled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sWXInstalled on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sWXInstalled);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sGameName(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sGameName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sGameName on a nil value");
			}
		}
		LuaScriptMgr.Push(L, userData.sGameName);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sRoleId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sRoleId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sRoleId on a nil value");
			}
		}
		userData.sRoleId = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sOpenId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sOpenId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sOpenId on a nil value");
			}
		}
		userData.sOpenId = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sServiceType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sServiceType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sServiceType on a nil value");
			}
		}
		userData.sServiceType = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sAcountType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAcountType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAcountType on a nil value");
			}
		}
		userData.sAcountType = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sArea(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sArea");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sArea on a nil value");
			}
		}
		userData.sArea = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sPartition(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPartition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPartition on a nil value");
			}
		}
		userData.sPartition = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sAppId(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAppId");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAppId on a nil value");
			}
		}
		userData.sAppId = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sAccessToken(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sAccessToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sAccessToken on a nil value");
			}
		}
		userData.sAccessToken = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sPayToken(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPayToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPayToken on a nil value");
			}
		}
		userData.sPayToken = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sGameVer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sGameVer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sGameVer on a nil value");
			}
		}
		userData.sGameVer = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sPlatID(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sPlatID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sPlatID on a nil value");
			}
		}
		userData.sPlatID = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sQQInstalled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sQQInstalled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sQQInstalled on a nil value");
			}
		}
		userData.sQQInstalled = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sWXInstalled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sWXInstalled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sWXInstalled on a nil value");
			}
		}
		userData.sWXInstalled = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sGameName(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UserData userData = (UserData)luaObject;
		if (userData == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sGameName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sGameName on a nil value");
			}
		}
		userData.sGameName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsRoleEmpty(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UserData userData = (UserData)LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.UserData");
		bool b = userData.IsRoleEmpty();
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Clear(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UserData userData = (UserData)LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.UserData");
		userData.Clear();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Assgin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UserData userData = (UserData)LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.UserData");
		Dictionary<string, string> dictPara = (Dictionary<string, string>)LuaScriptMgr.GetNetObject(L, 2, typeof(Dictionary<string, string>));
		userData.Assgin(dictPara);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Refresh(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UserData userData = (UserData)LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.UserData");
		Dictionary<string, string> dictPara = (Dictionary<string, string>)LuaScriptMgr.GetNetObject(L, 2, typeof(Dictionary<string, string>));
		userData.Refresh(dictPara);
		return 0;
	}
}

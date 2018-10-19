using com.tencent.pandora;
using System;
using UnityEngine;

public class ObjectWrap
{
	private static Type classType = typeof(UnityEngine.Object);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Equals", new LuaCSFunction(ObjectWrap.Equals)),
			new LuaMethod("GetHashCode", new LuaCSFunction(ObjectWrap.GetHashCode)),
			new LuaMethod("GetInstanceID", new LuaCSFunction(ObjectWrap.GetInstanceID)),
			new LuaMethod("Instantiate", new LuaCSFunction(ObjectWrap.Instantiate)),
			new LuaMethod("FindObjectsOfType", new LuaCSFunction(ObjectWrap.FindObjectsOfType)),
			new LuaMethod("FindObjectOfType", new LuaCSFunction(ObjectWrap.FindObjectOfType)),
			new LuaMethod("DontDestroyOnLoad", new LuaCSFunction(ObjectWrap.DontDestroyOnLoad)),
			new LuaMethod("ToString", new LuaCSFunction(ObjectWrap.ToString)),
			new LuaMethod("DestroyObject", new LuaCSFunction(ObjectWrap.DestroyObject)),
			new LuaMethod("DestroyImmediate", new LuaCSFunction(ObjectWrap.DestroyImmediate)),
			new LuaMethod("Destroy", new LuaCSFunction(ObjectWrap.Destroy)),
			new LuaMethod("New", new LuaCSFunction(ObjectWrap._CreateObject)),
			new LuaMethod("GetClassType", new LuaCSFunction(ObjectWrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(ObjectWrap.Lua_ToString)),
			new LuaMethod("__eq", new LuaCSFunction(ObjectWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("name", new LuaCSFunction(ObjectWrap.get_name), new LuaCSFunction(ObjectWrap.set_name)),
			new LuaField("hideFlags", new LuaCSFunction(ObjectWrap.get_hideFlags), new LuaCSFunction(ObjectWrap.set_hideFlags))
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Object", typeof(UnityEngine.Object), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateObject(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			UnityEngine.Object obj = new UnityEngine.Object();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Object.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, ObjectWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_name(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)luaObject;
		if (@object == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name name");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index name on a nil value");
			}
		}
		LuaScriptMgr.Push(L, @object.name);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_hideFlags(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)luaObject;
		if (@object == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hideFlags");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
			}
		}
		LuaScriptMgr.Push(L, @object.hideFlags);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_name(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)luaObject;
		if (@object == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name name");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index name on a nil value");
			}
		}
		@object.name = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_hideFlags(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)luaObject;
		if (@object == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hideFlags");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
			}
		}
		@object.hideFlags = (HideFlags)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(HideFlags)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_ToString(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject != null)
		{
			LuaScriptMgr.Push(L, luaObject.ToString());
		}
		else
		{
			LuaScriptMgr.Push(L, "Table: UnityEngine.Object");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UnityEngine.Object @object = LuaScriptMgr.GetVarObject(L, 1) as UnityEngine.Object;
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = (!(@object != null)) ? (varObject == null) : @object.Equals(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object");
		int hashCode = @object.GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetInstanceID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object");
		int instanceID = @object.GetInstanceID();
		LuaScriptMgr.Push(L, instanceID);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Instantiate(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			UnityEngine.Object unityObject = LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object));
			UnityEngine.Object obj = UnityEngine.Object.Instantiate(unityObject);
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		if (num == 3)
		{
			UnityEngine.Object unityObject2 = LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object));
			Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
			Quaternion quaternion = LuaScriptMgr.GetQuaternion(L, 3);
			UnityEngine.Object obj2 = UnityEngine.Object.Instantiate(unityObject2, vector, quaternion);
			LuaScriptMgr.Push(L, obj2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Object.Instantiate");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindObjectsOfType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		UnityEngine.Object[] o = UnityEngine.Object.FindObjectsOfType(typeObject);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindObjectOfType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		UnityEngine.Object obj = UnityEngine.Object.FindObjectOfType(typeObject);
		LuaScriptMgr.Push(L, obj);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int DontDestroyOnLoad(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UnityEngine.Object unityObject = LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object));
		UnityEngine.Object.DontDestroyOnLoad(unityObject);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UnityEngine.Object @object = (UnityEngine.Object)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object");
		string str = @object.ToString();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UnityEngine.Object x = LuaScriptMgr.GetLuaObject(L, 1) as UnityEngine.Object;
		UnityEngine.Object y = LuaScriptMgr.GetLuaObject(L, 2) as UnityEngine.Object;
		bool b = x == y;
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int DestroyObject(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			UnityEngine.Object obj = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			LuaScriptMgr.__gc(L);
			UnityEngine.Object.DestroyObject(obj);
			return 0;
		}
		if (num == 2)
		{
			UnityEngine.Object obj2 = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			float t = (float)LuaScriptMgr.GetNumber(L, 2);
			UnityEngine.Object.DestroyObject(obj2, t);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyObject");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int DestroyImmediate(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			UnityEngine.Object obj = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			LuaScriptMgr.__gc(L);
			UnityEngine.Object.DestroyImmediate(obj);
			return 0;
		}
		if (num == 2)
		{
			UnityEngine.Object obj2 = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			bool boolean = LuaScriptMgr.GetBoolean(L, 2);
			UnityEngine.Object.DestroyImmediate(obj2, boolean);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyImmediate");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Destroy(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			UnityEngine.Object obj = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			LuaScriptMgr.__gc(L);
			UnityEngine.Object.Destroy(obj);
			return 0;
		}
		if (num == 2)
		{
			UnityEngine.Object obj2 = (UnityEngine.Object)LuaScriptMgr.GetLuaObject(L, 1);
			float t = (float)LuaScriptMgr.GetNumber(L, 2);
			UnityEngine.Object.Destroy(obj2, t);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Object.Destroy");
		return 0;
	}
}

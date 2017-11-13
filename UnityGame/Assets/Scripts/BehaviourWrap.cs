using com.tencent.pandora;
using System;
using UnityEngine;

public class BehaviourWrap
{
	private static Type classType = typeof(Behaviour);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("New", new LuaCSFunction(BehaviourWrap._CreateBehaviour)),
			new LuaMethod("GetClassType", new LuaCSFunction(BehaviourWrap.GetClassType)),
			new LuaMethod("__eq", new LuaCSFunction(BehaviourWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("enabled", new LuaCSFunction(BehaviourWrap.get_enabled), new LuaCSFunction(BehaviourWrap.set_enabled)),
			new LuaField("isActiveAndEnabled", new LuaCSFunction(BehaviourWrap.get_isActiveAndEnabled), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Behaviour", typeof(Behaviour), regs, fields, typeof(Component));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateBehaviour(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			Behaviour obj = new Behaviour();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Behaviour.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, BehaviourWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_enabled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Behaviour behaviour = (Behaviour)luaObject;
		if (behaviour == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name enabled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index enabled on a nil value");
			}
		}
		LuaScriptMgr.Push(L, behaviour.enabled);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_isActiveAndEnabled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Behaviour behaviour = (Behaviour)luaObject;
		if (behaviour == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isActiveAndEnabled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isActiveAndEnabled on a nil value");
			}
		}
		LuaScriptMgr.Push(L, behaviour.isActiveAndEnabled);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_enabled(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Behaviour behaviour = (Behaviour)luaObject;
		if (behaviour == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name enabled");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index enabled on a nil value");
			}
		}
		behaviour.enabled = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object x = LuaScriptMgr.GetLuaObject(L, 1) as Object;
		Object y = LuaScriptMgr.GetLuaObject(L, 2) as Object;
		bool b = x == y;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}

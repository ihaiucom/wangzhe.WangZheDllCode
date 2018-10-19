using com.tencent.pandora;
using System;

public class EnumWrap
{
	private static Type classType = typeof(Enum);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("GetTypeCode", new LuaCSFunction(EnumWrap.GetTypeCode)),
			new LuaMethod("GetValues", new LuaCSFunction(EnumWrap.GetValues)),
			new LuaMethod("GetNames", new LuaCSFunction(EnumWrap.GetNames)),
			new LuaMethod("GetName", new LuaCSFunction(EnumWrap.GetName)),
			new LuaMethod("IsDefined", new LuaCSFunction(EnumWrap.IsDefined)),
			new LuaMethod("GetUnderlyingType", new LuaCSFunction(EnumWrap.GetUnderlyingType)),
			new LuaMethod("Parse", new LuaCSFunction(EnumWrap.Parse)),
			new LuaMethod("CompareTo", new LuaCSFunction(EnumWrap.CompareTo)),
			new LuaMethod("ToString", new LuaCSFunction(EnumWrap.ToString)),
			new LuaMethod("ToObject", new LuaCSFunction(EnumWrap.ToObject)),
			new LuaMethod("Format", new LuaCSFunction(EnumWrap.Format)),
			new LuaMethod("GetHashCode", new LuaCSFunction(EnumWrap.GetHashCode)),
			new LuaMethod("Equals", new LuaCSFunction(EnumWrap.Equals)),
			new LuaMethod("New", new LuaCSFunction(EnumWrap._CreateEnum)),
			new LuaMethod("GetClassType", new LuaCSFunction(EnumWrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(EnumWrap.Lua_ToString)),
			new LuaMethod("__eq", new LuaCSFunction(EnumWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[0];
		LuaScriptMgr.RegisterLib(L, "System.Enum", typeof(Enum), regs, fields, null);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateEnum(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Enum class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, EnumWrap.classType);
		return 1;
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
			LuaScriptMgr.Push(L, "Table: System.Enum");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Enum @enum = (Enum)LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
		TypeCode typeCode = @enum.GetTypeCode();
		LuaScriptMgr.Push(L, typeCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetValues(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Array values = Enum.GetValues(typeObject);
		LuaScriptMgr.PushObject(L, values);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetNames(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		string[] names = Enum.GetNames(typeObject);
		LuaScriptMgr.PushArray(L, names);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		string name = Enum.GetName(typeObject, varObject);
		LuaScriptMgr.Push(L, name);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsDefined(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = Enum.IsDefined(typeObject, varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetUnderlyingType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type underlyingType = Enum.GetUnderlyingType(typeObject);
		LuaScriptMgr.Push(L, underlyingType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Parse(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			object o = Enum.Parse(typeObject, luaString);
			LuaScriptMgr.PushVarObject(L, o);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			object o2 = Enum.Parse(typeObject2, luaString2, boolean);
			LuaScriptMgr.PushVarObject(L, o2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Enum.Parse");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CompareTo(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Enum @enum = (Enum)LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		int d = @enum.CompareTo(varObject);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Enum @enum = (Enum)LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
			string str = @enum.ToString();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			Enum enum2 = (Enum)LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			string str2 = enum2.ToString(luaString);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Enum.ToString");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToObject(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(long)))
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			long value = (long)LuaDLL.lua_tonumber(L, 2);
			object o = Enum.ToObject(typeObject, value);
			LuaScriptMgr.PushVarObject(L, o);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(object)))
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			object varObject = LuaScriptMgr.GetVarObject(L, 2);
			object o2 = Enum.ToObject(typeObject2, varObject);
			LuaScriptMgr.PushVarObject(L, o2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Enum.ToObject");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Format(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		string luaString = LuaScriptMgr.GetLuaString(L, 3);
		string str = Enum.Format(typeObject, varObject, luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Enum @enum = LuaScriptMgr.GetLuaObject(L, 1) as Enum;
		Enum enum2 = LuaScriptMgr.GetLuaObject(L, 2) as Enum;
		bool b = @enum == enum2;
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Enum @enum = (Enum)LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
		int hashCode = @enum.GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Enum @enum = LuaScriptMgr.GetVarObject(L, 1) as Enum;
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = (@enum == null) ? (varObject == null) : @enum.Equals(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}

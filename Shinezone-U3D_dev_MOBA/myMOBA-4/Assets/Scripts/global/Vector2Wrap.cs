using com.tencent.pandora;
using System;
using UnityEngine;

public class Vector2Wrap
{
	private static Type classType = typeof(Vector2);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("get_Item", new LuaCSFunction(Vector2Wrap.get_Item)),
			new LuaMethod("set_Item", new LuaCSFunction(Vector2Wrap.set_Item)),
			new LuaMethod("Set", new LuaCSFunction(Vector2Wrap.Set)),
			new LuaMethod("Lerp", new LuaCSFunction(Vector2Wrap.Lerp)),
			new LuaMethod("MoveTowards", new LuaCSFunction(Vector2Wrap.MoveTowards)),
			new LuaMethod("Scale", new LuaCSFunction(Vector2Wrap.Scale)),
			new LuaMethod("Normalize", new LuaCSFunction(Vector2Wrap.Normalize)),
			new LuaMethod("ToString", new LuaCSFunction(Vector2Wrap.ToString)),
			new LuaMethod("GetHashCode", new LuaCSFunction(Vector2Wrap.GetHashCode)),
			new LuaMethod("Equals", new LuaCSFunction(Vector2Wrap.Equals)),
			new LuaMethod("Dot", new LuaCSFunction(Vector2Wrap.Dot)),
			new LuaMethod("Angle", new LuaCSFunction(Vector2Wrap.Angle)),
			new LuaMethod("Distance", new LuaCSFunction(Vector2Wrap.Distance)),
			new LuaMethod("ClampMagnitude", new LuaCSFunction(Vector2Wrap.ClampMagnitude)),
			new LuaMethod("SqrMagnitude", new LuaCSFunction(Vector2Wrap.SqrMagnitude)),
			new LuaMethod("Min", new LuaCSFunction(Vector2Wrap.Min)),
			new LuaMethod("Max", new LuaCSFunction(Vector2Wrap.Max)),
			new LuaMethod("SmoothDamp", new LuaCSFunction(Vector2Wrap.SmoothDamp)),
			new LuaMethod("New", new LuaCSFunction(Vector2Wrap._CreateVector2)),
			new LuaMethod("GetClassType", new LuaCSFunction(Vector2Wrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(Vector2Wrap.Lua_ToString)),
			new LuaMethod("__add", new LuaCSFunction(Vector2Wrap.Lua_Add)),
			new LuaMethod("__sub", new LuaCSFunction(Vector2Wrap.Lua_Sub)),
			new LuaMethod("__mul", new LuaCSFunction(Vector2Wrap.Lua_Mul)),
			new LuaMethod("__div", new LuaCSFunction(Vector2Wrap.Lua_Div)),
			new LuaMethod("__eq", new LuaCSFunction(Vector2Wrap.Lua_Eq)),
			new LuaMethod("__unm", new LuaCSFunction(Vector2Wrap.Lua_Neg))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("kEpsilon", new LuaCSFunction(Vector2Wrap.get_kEpsilon), null),
			new LuaField("x", new LuaCSFunction(Vector2Wrap.get_x), new LuaCSFunction(Vector2Wrap.set_x)),
			new LuaField("y", new LuaCSFunction(Vector2Wrap.get_y), new LuaCSFunction(Vector2Wrap.set_y)),
			new LuaField("normalized", new LuaCSFunction(Vector2Wrap.get_normalized), null),
			new LuaField("magnitude", new LuaCSFunction(Vector2Wrap.get_magnitude), null),
			new LuaField("sqrMagnitude", new LuaCSFunction(Vector2Wrap.get_sqrMagnitude), null),
			new LuaField("zero", new LuaCSFunction(Vector2Wrap.get_zero), null),
			new LuaField("one", new LuaCSFunction(Vector2Wrap.get_one), null),
			new LuaField("up", new LuaCSFunction(Vector2Wrap.get_up), null),
			new LuaField("right", new LuaCSFunction(Vector2Wrap.get_right), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Vector2", typeof(Vector2), regs, fields, null);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateVector2(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			float x = (float)LuaScriptMgr.GetNumber(L, 1);
			float y = (float)LuaScriptMgr.GetNumber(L, 2);
			Vector2 v = new Vector2(x, y);
			LuaScriptMgr.Push(L, v);
			return 1;
		}
		if (num == 0)
		{
			LuaScriptMgr.Push(L, default(Vector2));
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector2Wrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_kEpsilon(IntPtr L)
	{
		LuaScriptMgr.Push(L, 1E-05f);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_x(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name x");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index x on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector2)luaObject).x);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_y(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name y");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index y on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector2)luaObject).y);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_normalized(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name normalized");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index normalized on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector2)luaObject).normalized);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_magnitude(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name magnitude");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index magnitude on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector2)luaObject).magnitude);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sqrMagnitude(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sqrMagnitude");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sqrMagnitude on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector2)luaObject).sqrMagnitude);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_zero(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector2.zero);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_one(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector2.one);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_up(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector2.up);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_right(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector2.right);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_x(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name x");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index x on a nil value");
			}
		}
		Vector2 vector = (Vector2)luaObject;
		vector.x = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_y(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name y");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index y on a nil value");
			}
		}
		Vector2 vector = (Vector2)luaObject;
		vector.y = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, vector);
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
			LuaScriptMgr.Push(L, "Table: UnityEngine.Vector2");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Item(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
		int index = (int)LuaScriptMgr.GetNumber(L, 2);
		float d = vector[index];
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_Item(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
		int index = (int)LuaScriptMgr.GetNumber(L, 2);
		float value = (float)LuaScriptMgr.GetNumber(L, 3);
		vector[index] = value;
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Set(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
		float new_x = (float)LuaScriptMgr.GetNumber(L, 2);
		float new_y = (float)LuaScriptMgr.GetNumber(L, 3);
		vector.Set(new_x, new_y);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lerp(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		float t = (float)LuaScriptMgr.GetNumber(L, 3);
		Vector2 v = Vector2.Lerp(vector, vector2, t);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MoveTowards(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		float maxDistanceDelta = (float)LuaScriptMgr.GetNumber(L, 3);
		Vector2 v = Vector2.MoveTowards(vector, vector2, maxDistanceDelta);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Scale(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		vector.Scale(vector2);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Normalize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
		vector.Normalize();
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string str = ((Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).ToString();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			Vector2 vector = (Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			string str2 = vector.ToString(luaString);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.ToString");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int hashCode = ((Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = (Vector2)LuaScriptMgr.GetVarObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = vector.Equals(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Dot(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		float d = Vector2.Dot(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Angle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		float d = Vector2.Angle(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Distance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		float d = Vector2.Distance(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ClampMagnitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		float maxLength = (float)LuaScriptMgr.GetNumber(L, 2);
		Vector2 v = Vector2.ClampMagnitude(vector, maxLength);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SqrMagnitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		float d = ((Vector2)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).SqrMagnitude();
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Min(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = Vector2.Min(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Max(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = Vector2.Max(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SmoothDamp(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 4)
		{
			Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
			Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
			Vector2 vector3 = LuaScriptMgr.GetVector2(L, 3);
			float smoothTime = (float)LuaScriptMgr.GetNumber(L, 4);
			Vector2 v = Vector2.SmoothDamp(vector, vector2, ref vector3, smoothTime);
			LuaScriptMgr.Push(L, v);
			LuaScriptMgr.Push(L, vector3);
			return 2;
		}
		if (num == 5)
		{
			Vector2 vector4 = LuaScriptMgr.GetVector2(L, 1);
			Vector2 vector5 = LuaScriptMgr.GetVector2(L, 2);
			Vector2 vector6 = LuaScriptMgr.GetVector2(L, 3);
			float smoothTime2 = (float)LuaScriptMgr.GetNumber(L, 4);
			float maxSpeed = (float)LuaScriptMgr.GetNumber(L, 5);
			Vector2 v2 = Vector2.SmoothDamp(vector4, vector5, ref vector6, smoothTime2, maxSpeed);
			LuaScriptMgr.Push(L, v2);
			LuaScriptMgr.Push(L, vector6);
			return 2;
		}
		if (num == 6)
		{
			Vector2 vector7 = LuaScriptMgr.GetVector2(L, 1);
			Vector2 vector8 = LuaScriptMgr.GetVector2(L, 2);
			Vector2 vector9 = LuaScriptMgr.GetVector2(L, 3);
			float smoothTime3 = (float)LuaScriptMgr.GetNumber(L, 4);
			float maxSpeed2 = (float)LuaScriptMgr.GetNumber(L, 5);
			float deltaTime = (float)LuaScriptMgr.GetNumber(L, 6);
			Vector2 v3 = Vector2.SmoothDamp(vector7, vector8, ref vector9, smoothTime3, maxSpeed2, deltaTime);
			LuaScriptMgr.Push(L, v3);
			LuaScriptMgr.Push(L, vector9);
			return 2;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.SmoothDamp");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Add(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = vector + vector2;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Sub(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = vector - vector2;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Neg(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 v = -vector;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Mul(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(float), typeof(LuaTable)))
		{
			float d = (float)LuaDLL.lua_tonumber(L, 1);
			Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
			Vector2 v = d * vector;
			LuaScriptMgr.Push(L, v);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(LuaTable), typeof(float)))
		{
			Vector2 vector2 = LuaScriptMgr.GetVector2(L, 1);
			float d2 = (float)LuaDLL.lua_tonumber(L, 2);
			Vector2 v2 = vector2 * d2;
			LuaScriptMgr.Push(L, v2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.op_Multiply");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Div(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		float d = (float)LuaScriptMgr.GetNumber(L, 2);
		Vector2 v = vector / d;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
		Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
		bool b = vector == vector2;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}

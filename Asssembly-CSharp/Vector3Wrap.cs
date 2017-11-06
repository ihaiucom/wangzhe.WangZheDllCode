using com.tencent.pandora;
using System;
using UnityEngine;

public class Vector3Wrap
{
	private static Type classType = typeof(Vector3);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Lerp", new LuaCSFunction(Vector3Wrap.Lerp)),
			new LuaMethod("Slerp", new LuaCSFunction(Vector3Wrap.Slerp)),
			new LuaMethod("OrthoNormalize", new LuaCSFunction(Vector3Wrap.OrthoNormalize)),
			new LuaMethod("MoveTowards", new LuaCSFunction(Vector3Wrap.MoveTowards)),
			new LuaMethod("RotateTowards", new LuaCSFunction(Vector3Wrap.RotateTowards)),
			new LuaMethod("SmoothDamp", new LuaCSFunction(Vector3Wrap.SmoothDamp)),
			new LuaMethod("get_Item", new LuaCSFunction(Vector3Wrap.get_Item)),
			new LuaMethod("set_Item", new LuaCSFunction(Vector3Wrap.set_Item)),
			new LuaMethod("Set", new LuaCSFunction(Vector3Wrap.Set)),
			new LuaMethod("Scale", new LuaCSFunction(Vector3Wrap.Scale)),
			new LuaMethod("Cross", new LuaCSFunction(Vector3Wrap.Cross)),
			new LuaMethod("GetHashCode", new LuaCSFunction(Vector3Wrap.GetHashCode)),
			new LuaMethod("Equals", new LuaCSFunction(Vector3Wrap.Equals)),
			new LuaMethod("Reflect", new LuaCSFunction(Vector3Wrap.Reflect)),
			new LuaMethod("Normalize", new LuaCSFunction(Vector3Wrap.Normalize)),
			new LuaMethod("ToString", new LuaCSFunction(Vector3Wrap.ToString)),
			new LuaMethod("Dot", new LuaCSFunction(Vector3Wrap.Dot)),
			new LuaMethod("Project", new LuaCSFunction(Vector3Wrap.Project)),
			new LuaMethod("ProjectOnPlane", new LuaCSFunction(Vector3Wrap.ProjectOnPlane)),
			new LuaMethod("Angle", new LuaCSFunction(Vector3Wrap.Angle)),
			new LuaMethod("Distance", new LuaCSFunction(Vector3Wrap.Distance)),
			new LuaMethod("ClampMagnitude", new LuaCSFunction(Vector3Wrap.ClampMagnitude)),
			new LuaMethod("Magnitude", new LuaCSFunction(Vector3Wrap.Magnitude)),
			new LuaMethod("SqrMagnitude", new LuaCSFunction(Vector3Wrap.SqrMagnitude)),
			new LuaMethod("Min", new LuaCSFunction(Vector3Wrap.Min)),
			new LuaMethod("Max", new LuaCSFunction(Vector3Wrap.Max)),
			new LuaMethod("New", new LuaCSFunction(Vector3Wrap._CreateVector3)),
			new LuaMethod("GetClassType", new LuaCSFunction(Vector3Wrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(Vector3Wrap.Lua_ToString)),
			new LuaMethod("__add", new LuaCSFunction(Vector3Wrap.Lua_Add)),
			new LuaMethod("__sub", new LuaCSFunction(Vector3Wrap.Lua_Sub)),
			new LuaMethod("__mul", new LuaCSFunction(Vector3Wrap.Lua_Mul)),
			new LuaMethod("__div", new LuaCSFunction(Vector3Wrap.Lua_Div)),
			new LuaMethod("__eq", new LuaCSFunction(Vector3Wrap.Lua_Eq)),
			new LuaMethod("__unm", new LuaCSFunction(Vector3Wrap.Lua_Neg))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("kEpsilon", new LuaCSFunction(Vector3Wrap.get_kEpsilon), null),
			new LuaField("x", new LuaCSFunction(Vector3Wrap.get_x), new LuaCSFunction(Vector3Wrap.set_x)),
			new LuaField("y", new LuaCSFunction(Vector3Wrap.get_y), new LuaCSFunction(Vector3Wrap.set_y)),
			new LuaField("z", new LuaCSFunction(Vector3Wrap.get_z), new LuaCSFunction(Vector3Wrap.set_z)),
			new LuaField("normalized", new LuaCSFunction(Vector3Wrap.get_normalized), null),
			new LuaField("magnitude", new LuaCSFunction(Vector3Wrap.get_magnitude), null),
			new LuaField("sqrMagnitude", new LuaCSFunction(Vector3Wrap.get_sqrMagnitude), null),
			new LuaField("zero", new LuaCSFunction(Vector3Wrap.get_zero), null),
			new LuaField("one", new LuaCSFunction(Vector3Wrap.get_one), null),
			new LuaField("forward", new LuaCSFunction(Vector3Wrap.get_forward), null),
			new LuaField("back", new LuaCSFunction(Vector3Wrap.get_back), null),
			new LuaField("up", new LuaCSFunction(Vector3Wrap.get_up), null),
			new LuaField("down", new LuaCSFunction(Vector3Wrap.get_down), null),
			new LuaField("left", new LuaCSFunction(Vector3Wrap.get_left), null),
			new LuaField("right", new LuaCSFunction(Vector3Wrap.get_right), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Vector3", typeof(Vector3), regs, fields, null);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateVector3(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			float x = (float)LuaScriptMgr.GetNumber(L, 1);
			float y = (float)LuaScriptMgr.GetNumber(L, 2);
			Vector3 v = new Vector3(x, y);
			LuaScriptMgr.Push(L, v);
			return 1;
		}
		if (num == 3)
		{
			float x2 = (float)LuaScriptMgr.GetNumber(L, 1);
			float y2 = (float)LuaScriptMgr.GetNumber(L, 2);
			float z = (float)LuaScriptMgr.GetNumber(L, 3);
			Vector3 v2 = new Vector3(x2, y2, z);
			LuaScriptMgr.Push(L, v2);
			return 1;
		}
		if (num == 0)
		{
			LuaScriptMgr.Push(L, default(Vector3));
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector3.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3Wrap.classType);
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
		LuaScriptMgr.Push(L, ((Vector3)luaObject).x);
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
		LuaScriptMgr.Push(L, ((Vector3)luaObject).y);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_z(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name z");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index z on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Vector3)luaObject).z);
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
		LuaScriptMgr.Push(L, ((Vector3)luaObject).normalized);
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
		LuaScriptMgr.Push(L, ((Vector3)luaObject).magnitude);
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
		LuaScriptMgr.Push(L, ((Vector3)luaObject).sqrMagnitude);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_zero(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.zero);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_one(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.one);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_forward(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.forward);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_back(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.back);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_up(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.up);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_down(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.down);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_left(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.left);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_right(IntPtr L)
	{
		LuaScriptMgr.Push(L, Vector3.right);
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
		Vector3 vector = (Vector3)luaObject;
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
		Vector3 vector = (Vector3)luaObject;
		vector.y = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_z(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name z");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index z on a nil value");
			}
		}
		Vector3 vector = (Vector3)luaObject;
		vector.z = (float)LuaScriptMgr.GetNumber(L, 3);
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
			LuaScriptMgr.Push(L, "Table: UnityEngine.Vector3");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lerp(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float t = (float)LuaScriptMgr.GetNumber(L, 3);
		Vector3 v = Vector3.Lerp(vector, vector2, t);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Slerp(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float t = (float)LuaScriptMgr.GetNumber(L, 3);
		Vector3 v = Vector3.Slerp(vector, vector2, t);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int OrthoNormalize(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
			Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
			Vector3.OrthoNormalize(ref vector, ref vector2);
			LuaScriptMgr.Push(L, vector);
			LuaScriptMgr.Push(L, vector2);
			return 2;
		}
		if (num == 3)
		{
			Vector3 vector3 = LuaScriptMgr.GetVector3(L, 1);
			Vector3 vector4 = LuaScriptMgr.GetVector3(L, 2);
			Vector3 vector5 = LuaScriptMgr.GetVector3(L, 3);
			Vector3.OrthoNormalize(ref vector3, ref vector4, ref vector5);
			LuaScriptMgr.Push(L, vector3);
			LuaScriptMgr.Push(L, vector4);
			LuaScriptMgr.Push(L, vector5);
			return 3;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector3.OrthoNormalize");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MoveTowards(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float maxDistanceDelta = (float)LuaScriptMgr.GetNumber(L, 3);
		Vector3 v = Vector3.MoveTowards(vector, vector2, maxDistanceDelta);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int RotateTowards(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float maxRadiansDelta = (float)LuaScriptMgr.GetNumber(L, 3);
		float maxMagnitudeDelta = (float)LuaScriptMgr.GetNumber(L, 4);
		Vector3 v = Vector3.RotateTowards(vector, vector2, maxRadiansDelta, maxMagnitudeDelta);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SmoothDamp(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 4)
		{
			Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
			Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
			Vector3 vector3 = LuaScriptMgr.GetVector3(L, 3);
			float smoothTime = (float)LuaScriptMgr.GetNumber(L, 4);
			Vector3 v = Vector3.SmoothDamp(vector, vector2, ref vector3, smoothTime);
			LuaScriptMgr.Push(L, v);
			LuaScriptMgr.Push(L, vector3);
			return 2;
		}
		if (num == 5)
		{
			Vector3 vector4 = LuaScriptMgr.GetVector3(L, 1);
			Vector3 vector5 = LuaScriptMgr.GetVector3(L, 2);
			Vector3 vector6 = LuaScriptMgr.GetVector3(L, 3);
			float smoothTime2 = (float)LuaScriptMgr.GetNumber(L, 4);
			float maxSpeed = (float)LuaScriptMgr.GetNumber(L, 5);
			Vector3 v2 = Vector3.SmoothDamp(vector4, vector5, ref vector6, smoothTime2, maxSpeed);
			LuaScriptMgr.Push(L, v2);
			LuaScriptMgr.Push(L, vector6);
			return 2;
		}
		if (num == 6)
		{
			Vector3 vector7 = LuaScriptMgr.GetVector3(L, 1);
			Vector3 vector8 = LuaScriptMgr.GetVector3(L, 2);
			Vector3 vector9 = LuaScriptMgr.GetVector3(L, 3);
			float smoothTime3 = (float)LuaScriptMgr.GetNumber(L, 4);
			float maxSpeed2 = (float)LuaScriptMgr.GetNumber(L, 5);
			float deltaTime = (float)LuaScriptMgr.GetNumber(L, 6);
			Vector3 v3 = Vector3.SmoothDamp(vector7, vector8, ref vector9, smoothTime3, maxSpeed2, deltaTime);
			LuaScriptMgr.Push(L, v3);
			LuaScriptMgr.Push(L, vector9);
			return 2;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector3.SmoothDamp");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Item(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
		int index = (int)LuaScriptMgr.GetNumber(L, 2);
		float d = vector[index];
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_Item(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
		int index = (int)LuaScriptMgr.GetNumber(L, 2);
		float value = (float)LuaScriptMgr.GetNumber(L, 3);
		vector[index] = value;
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Set(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
		float new_x = (float)LuaScriptMgr.GetNumber(L, 2);
		float new_y = (float)LuaScriptMgr.GetNumber(L, 3);
		float new_z = (float)LuaScriptMgr.GetNumber(L, 4);
		vector.Set(new_x, new_y, new_z);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Scale(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		vector.Scale(vector2);
		LuaScriptMgr.SetValueObject(L, 1, vector);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Cross(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.Cross(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int hashCode = ((Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3")).GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = (Vector3)LuaScriptMgr.GetVarObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = vector.Equals(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Reflect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.Reflect(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Normalize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
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
			string str = ((Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3")).ToString();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			Vector3 vector = (Vector3)LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector3");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			string str2 = vector.ToString(luaString);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector3.ToString");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Dot(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float d = Vector3.Dot(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Project(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.Project(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ProjectOnPlane(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.ProjectOnPlane(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Angle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float d = Vector3.Angle(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Distance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		float d = Vector3.Distance(vector, vector2);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ClampMagnitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		float maxLength = (float)LuaScriptMgr.GetNumber(L, 2);
		Vector3 v = Vector3.ClampMagnitude(vector, maxLength);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Magnitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		float d = Vector3.Magnitude(vector);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SqrMagnitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		float d = Vector3.SqrMagnitude(vector);
		LuaScriptMgr.Push(L, d);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Min(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.Min(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Max(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = Vector3.Max(vector, vector2);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Add(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = vector + vector2;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Sub(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		Vector3 v = vector - vector2;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Neg(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 v = -vector;
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
			Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
			Vector3 v = d * vector;
			LuaScriptMgr.Push(L, v);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(LuaTable), typeof(float)))
		{
			Vector3 vector2 = LuaScriptMgr.GetVector3(L, 1);
			float d2 = (float)LuaDLL.lua_tonumber(L, 2);
			Vector3 v2 = vector2 * d2;
			LuaScriptMgr.Push(L, v2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Vector3.op_Multiply");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Div(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		float d = (float)LuaScriptMgr.GetNumber(L, 2);
		Vector3 v = vector / d;
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Vector3 vector = LuaScriptMgr.GetVector3(L, 1);
		Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
		bool b = vector == vector2;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}

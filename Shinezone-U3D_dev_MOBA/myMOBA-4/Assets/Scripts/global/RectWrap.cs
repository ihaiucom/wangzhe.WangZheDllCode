using com.tencent.pandora;
using System;
using UnityEngine;

public class RectWrap
{
	private static Type classType = typeof(Rect);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("MinMaxRect", new LuaCSFunction(RectWrap.MinMaxRect)),
			new LuaMethod("Set", new LuaCSFunction(RectWrap.Set)),
			new LuaMethod("ToString", new LuaCSFunction(RectWrap.ToString)),
			new LuaMethod("Contains", new LuaCSFunction(RectWrap.Contains)),
			new LuaMethod("Overlaps", new LuaCSFunction(RectWrap.Overlaps)),
			new LuaMethod("NormalizedToPoint", new LuaCSFunction(RectWrap.NormalizedToPoint)),
			new LuaMethod("PointToNormalized", new LuaCSFunction(RectWrap.PointToNormalized)),
			new LuaMethod("GetHashCode", new LuaCSFunction(RectWrap.GetHashCode)),
			new LuaMethod("Equals", new LuaCSFunction(RectWrap.Equals)),
			new LuaMethod("New", new LuaCSFunction(RectWrap._CreateRect)),
			new LuaMethod("GetClassType", new LuaCSFunction(RectWrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(RectWrap.Lua_ToString)),
			new LuaMethod("__eq", new LuaCSFunction(RectWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("x", new LuaCSFunction(RectWrap.get_x), new LuaCSFunction(RectWrap.set_x)),
			new LuaField("y", new LuaCSFunction(RectWrap.get_y), new LuaCSFunction(RectWrap.set_y)),
			new LuaField("position", new LuaCSFunction(RectWrap.get_position), new LuaCSFunction(RectWrap.set_position)),
			new LuaField("center", new LuaCSFunction(RectWrap.get_center), new LuaCSFunction(RectWrap.set_center)),
			new LuaField("min", new LuaCSFunction(RectWrap.get_min), new LuaCSFunction(RectWrap.set_min)),
			new LuaField("max", new LuaCSFunction(RectWrap.get_max), new LuaCSFunction(RectWrap.set_max)),
			new LuaField("width", new LuaCSFunction(RectWrap.get_width), new LuaCSFunction(RectWrap.set_width)),
			new LuaField("height", new LuaCSFunction(RectWrap.get_height), new LuaCSFunction(RectWrap.set_height)),
			new LuaField("size", new LuaCSFunction(RectWrap.get_size), new LuaCSFunction(RectWrap.set_size)),
			new LuaField("xMin", new LuaCSFunction(RectWrap.get_xMin), new LuaCSFunction(RectWrap.set_xMin)),
			new LuaField("yMin", new LuaCSFunction(RectWrap.get_yMin), new LuaCSFunction(RectWrap.set_yMin)),
			new LuaField("xMax", new LuaCSFunction(RectWrap.get_xMax), new LuaCSFunction(RectWrap.set_xMax)),
			new LuaField("yMax", new LuaCSFunction(RectWrap.get_yMax), new LuaCSFunction(RectWrap.set_yMax))
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Rect", typeof(Rect), regs, fields, null);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateRect(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Rect source = (Rect)LuaScriptMgr.GetNetObject(L, 1, typeof(Rect));
			Rect rect = new Rect(source);
			LuaScriptMgr.PushValue(L, rect);
			return 1;
		}
		if (num == 4)
		{
			float left = (float)LuaScriptMgr.GetNumber(L, 1);
			float top = (float)LuaScriptMgr.GetNumber(L, 2);
			float width = (float)LuaScriptMgr.GetNumber(L, 3);
			float height = (float)LuaScriptMgr.GetNumber(L, 4);
			Rect rect2 = new Rect(left, top, width, height);
			LuaScriptMgr.PushValue(L, rect2);
			return 1;
		}
		if (num == 0)
		{
			LuaScriptMgr.PushValue(L, default(Rect));
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Rect.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, RectWrap.classType);
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
		LuaScriptMgr.Push(L, ((Rect)luaObject).x);
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
		LuaScriptMgr.Push(L, ((Rect)luaObject).y);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_position(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name position");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index position on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).position);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_center(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name center");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index center on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).center);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_min(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name min");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index min on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).min);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_max(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name max");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index max on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).max);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_width(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name width");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index width on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).width);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_height(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name height");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index height on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).height);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_size(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name size");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index size on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).size);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_xMin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name xMin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index xMin on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).xMin);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_yMin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name yMin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index yMin on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).yMin);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_xMax(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name xMax");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index xMax on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).xMax);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_yMax(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name yMax");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index yMax on a nil value");
			}
		}
		LuaScriptMgr.Push(L, ((Rect)luaObject).yMax);
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
		Rect rect = (Rect)luaObject;
		rect.x = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
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
		Rect rect = (Rect)luaObject;
		rect.y = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_position(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name position");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index position on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.position = LuaScriptMgr.GetVector2(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_center(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name center");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index center on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.center = LuaScriptMgr.GetVector2(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_min(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name min");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index min on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.min = LuaScriptMgr.GetVector2(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_max(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name max");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index max on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.max = LuaScriptMgr.GetVector2(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_width(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name width");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index width on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.width = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_height(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name height");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index height on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.height = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_size(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name size");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index size on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.size = LuaScriptMgr.GetVector2(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_xMin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name xMin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index xMin on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.xMin = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_yMin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name yMin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index yMin on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.yMin = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_xMax(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name xMax");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index xMax on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.xMax = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_yMax(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name yMax");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index yMax on a nil value");
			}
		}
		Rect rect = (Rect)luaObject;
		rect.yMax = (float)LuaScriptMgr.GetNumber(L, 3);
		LuaScriptMgr.SetValueObject(L, 1, rect);
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
			LuaScriptMgr.Push(L, "Table: UnityEngine.Rect");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MinMaxRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		float left = (float)LuaScriptMgr.GetNumber(L, 1);
		float top = (float)LuaScriptMgr.GetNumber(L, 2);
		float right = (float)LuaScriptMgr.GetNumber(L, 3);
		float bottom = (float)LuaScriptMgr.GetNumber(L, 4);
		Rect rect = Rect.MinMaxRect(left, top, right, bottom);
		LuaScriptMgr.PushValue(L, rect);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Set(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		Rect rect = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
		float left = (float)LuaScriptMgr.GetNumber(L, 2);
		float top = (float)LuaScriptMgr.GetNumber(L, 3);
		float width = (float)LuaScriptMgr.GetNumber(L, 4);
		float height = (float)LuaScriptMgr.GetNumber(L, 5);
		rect.Set(left, top, width, height);
		LuaScriptMgr.SetValueObject(L, 1, rect);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string str = ((Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect")).ToString();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			Rect rect = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			string str2 = rect.ToString(luaString);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Rect.ToString");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Contains(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Rect), typeof(LuaTable)))
		{
			Rect rect = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
			bool b = rect.Contains(vector);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Rect), typeof(LuaTable)))
		{
			Rect rect2 = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
			bool b2 = rect2.Contains(vector2);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		if (num == 3)
		{
			Rect rect3 = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			Vector3 vector3 = LuaScriptMgr.GetVector3(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			bool b3 = rect3.Contains(vector3, boolean);
			LuaScriptMgr.Push(L, b3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Rect.Contains");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Overlaps(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Rect rect = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			Rect other = (Rect)LuaScriptMgr.GetNetObject(L, 2, typeof(Rect));
			bool b = rect.Overlaps(other);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 3)
		{
			Rect rect2 = (Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect");
			Rect other2 = (Rect)LuaScriptMgr.GetNetObject(L, 2, typeof(Rect));
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			bool b2 = rect2.Overlaps(other2, boolean);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Rect.Overlaps");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int NormalizedToPoint(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Rect rectangle = (Rect)LuaScriptMgr.GetNetObject(L, 1, typeof(Rect));
		Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = Rect.NormalizedToPoint(rectangle, vector);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int PointToNormalized(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Rect rectangle = (Rect)LuaScriptMgr.GetNetObject(L, 1, typeof(Rect));
		Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
		Vector2 v = Rect.PointToNormalized(rectangle, vector);
		LuaScriptMgr.Push(L, v);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int hashCode = ((Rect)LuaScriptMgr.GetNetObjectSelf(L, 1, "Rect")).GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Rect rect = (Rect)LuaScriptMgr.GetVarObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = rect.Equals(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Rect lhs = (Rect)LuaScriptMgr.GetVarObject(L, 1);
		Rect rhs = (Rect)LuaScriptMgr.GetVarObject(L, 2);
		bool b = lhs == rhs;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}

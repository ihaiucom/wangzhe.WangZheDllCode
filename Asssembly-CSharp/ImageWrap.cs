using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageWrap
{
	private static Type classType = typeof(Image);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("OnBeforeSerialize", new LuaCSFunction(ImageWrap.OnBeforeSerialize)),
			new LuaMethod("OnAfterDeserialize", new LuaCSFunction(ImageWrap.OnAfterDeserialize)),
			new LuaMethod("SetNativeSize", new LuaCSFunction(ImageWrap.SetNativeSize)),
			new LuaMethod("CalculateLayoutInputHorizontal", new LuaCSFunction(ImageWrap.CalculateLayoutInputHorizontal)),
			new LuaMethod("CalculateLayoutInputVertical", new LuaCSFunction(ImageWrap.CalculateLayoutInputVertical)),
			new LuaMethod("IsRaycastLocationValid", new LuaCSFunction(ImageWrap.IsRaycastLocationValid)),
			new LuaMethod("New", new LuaCSFunction(ImageWrap._CreateImage)),
			new LuaMethod("GetClassType", new LuaCSFunction(ImageWrap.GetClassType)),
			new LuaMethod("__eq", new LuaCSFunction(ImageWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("sprite", new LuaCSFunction(ImageWrap.get_sprite), new LuaCSFunction(ImageWrap.set_sprite)),
			new LuaField("overrideSprite", new LuaCSFunction(ImageWrap.get_overrideSprite), new LuaCSFunction(ImageWrap.set_overrideSprite)),
			new LuaField("type", new LuaCSFunction(ImageWrap.get_type), new LuaCSFunction(ImageWrap.set_type)),
			new LuaField("preserveAspect", new LuaCSFunction(ImageWrap.get_preserveAspect), new LuaCSFunction(ImageWrap.set_preserveAspect)),
			new LuaField("fillCenter", new LuaCSFunction(ImageWrap.get_fillCenter), new LuaCSFunction(ImageWrap.set_fillCenter)),
			new LuaField("fillMethod", new LuaCSFunction(ImageWrap.get_fillMethod), new LuaCSFunction(ImageWrap.set_fillMethod)),
			new LuaField("fillAmount", new LuaCSFunction(ImageWrap.get_fillAmount), new LuaCSFunction(ImageWrap.set_fillAmount)),
			new LuaField("fillClockwise", new LuaCSFunction(ImageWrap.get_fillClockwise), new LuaCSFunction(ImageWrap.set_fillClockwise)),
			new LuaField("fillOrigin", new LuaCSFunction(ImageWrap.get_fillOrigin), new LuaCSFunction(ImageWrap.set_fillOrigin)),
			new LuaField("eventAlphaThreshold", new LuaCSFunction(ImageWrap.get_eventAlphaThreshold), new LuaCSFunction(ImageWrap.set_eventAlphaThreshold)),
			new LuaField("mainTexture", new LuaCSFunction(ImageWrap.get_mainTexture), null),
			new LuaField("hasBorder", new LuaCSFunction(ImageWrap.get_hasBorder), null),
			new LuaField("pixelsPerUnit", new LuaCSFunction(ImageWrap.get_pixelsPerUnit), null),
			new LuaField("minWidth", new LuaCSFunction(ImageWrap.get_minWidth), null),
			new LuaField("preferredWidth", new LuaCSFunction(ImageWrap.get_preferredWidth), null),
			new LuaField("flexibleWidth", new LuaCSFunction(ImageWrap.get_flexibleWidth), null),
			new LuaField("minHeight", new LuaCSFunction(ImageWrap.get_minHeight), null),
			new LuaField("preferredHeight", new LuaCSFunction(ImageWrap.get_preferredHeight), null),
			new LuaField("flexibleHeight", new LuaCSFunction(ImageWrap.get_flexibleHeight), null),
			new LuaField("layoutPriority", new LuaCSFunction(ImageWrap.get_layoutPriority), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Image", typeof(Image), regs, fields, typeof(MaskableGraphic));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateImage(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Image class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, ImageWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_sprite(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sprite on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_sprite());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_overrideSprite(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name overrideSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index overrideSprite on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_overrideSprite());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_type(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name type");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index type on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_type());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_preserveAspect(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name preserveAspect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index preserveAspect on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_preserveAspect());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_fillCenter(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillCenter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillCenter on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_fillCenter());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_fillMethod(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillMethod");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillMethod on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_fillMethod());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_fillAmount(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_fillAmount());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_fillClockwise(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillClockwise");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillClockwise on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_fillClockwise());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_fillOrigin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillOrigin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillOrigin on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_fillOrigin());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_eventAlphaThreshold(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name eventAlphaThreshold");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index eventAlphaThreshold on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_eventAlphaThreshold());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_mainTexture(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mainTexture");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_mainTexture());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_hasBorder(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hasBorder");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hasBorder on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_hasBorder());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_pixelsPerUnit(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pixelsPerUnit");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pixelsPerUnit on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_pixelsPerUnit());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_minWidth(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name minWidth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index minWidth on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_minWidth());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_preferredWidth(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name preferredWidth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index preferredWidth on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_preferredWidth());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_flexibleWidth(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name flexibleWidth");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index flexibleWidth on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_flexibleWidth());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_minHeight(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name minHeight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index minHeight on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_minHeight());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_preferredHeight(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name preferredHeight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index preferredHeight on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_preferredHeight());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_flexibleHeight(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name flexibleHeight");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index flexibleHeight on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_flexibleHeight());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_layoutPriority(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name layoutPriority");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index layoutPriority on a nil value");
			}
		}
		LuaScriptMgr.Push(L, image.get_layoutPriority());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_sprite(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name sprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index sprite on a nil value");
			}
		}
		image.set_sprite((Sprite)LuaScriptMgr.GetUnityObject(L, 3, typeof(Sprite)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_overrideSprite(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name overrideSprite");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index overrideSprite on a nil value");
			}
		}
		image.set_overrideSprite((Sprite)LuaScriptMgr.GetUnityObject(L, 3, typeof(Sprite)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_type(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name type");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index type on a nil value");
			}
		}
		image.set_type((int)LuaScriptMgr.GetNetObject(L, 3, typeof(Image.Type)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_preserveAspect(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name preserveAspect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index preserveAspect on a nil value");
			}
		}
		image.set_preserveAspect(LuaScriptMgr.GetBoolean(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_fillCenter(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillCenter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillCenter on a nil value");
			}
		}
		image.set_fillCenter(LuaScriptMgr.GetBoolean(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_fillMethod(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillMethod");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillMethod on a nil value");
			}
		}
		image.set_fillMethod((int)LuaScriptMgr.GetNetObject(L, 3, typeof(Image.FillMethod)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_fillAmount(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillAmount on a nil value");
			}
		}
		image.set_fillAmount((float)LuaScriptMgr.GetNumber(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_fillClockwise(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillClockwise");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillClockwise on a nil value");
			}
		}
		image.set_fillClockwise(LuaScriptMgr.GetBoolean(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_fillOrigin(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name fillOrigin");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index fillOrigin on a nil value");
			}
		}
		image.set_fillOrigin((int)LuaScriptMgr.GetNumber(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_eventAlphaThreshold(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Image image = (Image)luaObject;
		if (image == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name eventAlphaThreshold");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index eventAlphaThreshold on a nil value");
			}
		}
		image.set_eventAlphaThreshold((float)LuaScriptMgr.GetNumber(L, 3));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int OnBeforeSerialize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		image.OnBeforeSerialize();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int OnAfterDeserialize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		image.OnAfterDeserialize();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetNativeSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		image.SetNativeSize();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CalculateLayoutInputHorizontal(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		image.CalculateLayoutInputHorizontal();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CalculateLayoutInputVertical(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		image.CalculateLayoutInputVertical();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsRaycastLocationValid(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Image image = (Image)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Image");
		Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
		Camera camera = (Camera)LuaScriptMgr.GetUnityObject(L, 3, typeof(Camera));
		bool b = image.IsRaycastLocationValid(vector, camera);
		LuaScriptMgr.Push(L, b);
		return 1;
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

using com.tencent.pandora;
using System;
using UnityEngine;

public class TextureWrap
{
	private static Type classType = typeof(Texture);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("SetGlobalAnisotropicFilteringLimits", new LuaCSFunction(TextureWrap.SetGlobalAnisotropicFilteringLimits)),
			new LuaMethod("GetNativeTexturePtr", new LuaCSFunction(TextureWrap.GetNativeTexturePtr)),
			new LuaMethod("GetNativeTextureID", new LuaCSFunction(TextureWrap.GetNativeTextureID)),
			new LuaMethod("New", new LuaCSFunction(TextureWrap._CreateTexture)),
			new LuaMethod("GetClassType", new LuaCSFunction(TextureWrap.GetClassType)),
			new LuaMethod("__eq", new LuaCSFunction(TextureWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("masterTextureLimit", new LuaCSFunction(TextureWrap.get_masterTextureLimit), new LuaCSFunction(TextureWrap.set_masterTextureLimit)),
			new LuaField("anisotropicFiltering", new LuaCSFunction(TextureWrap.get_anisotropicFiltering), new LuaCSFunction(TextureWrap.set_anisotropicFiltering)),
			new LuaField("width", new LuaCSFunction(TextureWrap.get_width), new LuaCSFunction(TextureWrap.set_width)),
			new LuaField("height", new LuaCSFunction(TextureWrap.get_height), new LuaCSFunction(TextureWrap.set_height)),
			new LuaField("filterMode", new LuaCSFunction(TextureWrap.get_filterMode), new LuaCSFunction(TextureWrap.set_filterMode)),
			new LuaField("anisoLevel", new LuaCSFunction(TextureWrap.get_anisoLevel), new LuaCSFunction(TextureWrap.set_anisoLevel)),
			new LuaField("wrapMode", new LuaCSFunction(TextureWrap.get_wrapMode), new LuaCSFunction(TextureWrap.set_wrapMode)),
			new LuaField("mipMapBias", new LuaCSFunction(TextureWrap.get_mipMapBias), new LuaCSFunction(TextureWrap.set_mipMapBias)),
			new LuaField("texelSize", new LuaCSFunction(TextureWrap.get_texelSize), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Texture", typeof(Texture), regs, fields, typeof(Object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateTexture(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			Texture obj = new Texture();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Texture.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, TextureWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_masterTextureLimit(IntPtr L)
	{
		LuaScriptMgr.Push(L, Texture.masterTextureLimit);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_anisotropicFiltering(IntPtr L)
	{
		LuaScriptMgr.Push(L, Texture.anisotropicFiltering);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_width(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
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
		LuaScriptMgr.Push(L, texture.width);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_height(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
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
		LuaScriptMgr.Push(L, texture.height);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_filterMode(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name filterMode");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index filterMode on a nil value");
			}
		}
		LuaScriptMgr.Push(L, texture.filterMode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_anisoLevel(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name anisoLevel");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index anisoLevel on a nil value");
			}
		}
		LuaScriptMgr.Push(L, texture.anisoLevel);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_wrapMode(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name wrapMode");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
			}
		}
		LuaScriptMgr.Push(L, texture.wrapMode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_mipMapBias(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mipMapBias");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mipMapBias on a nil value");
			}
		}
		LuaScriptMgr.Push(L, texture.mipMapBias);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_texelSize(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name texelSize");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index texelSize on a nil value");
			}
		}
		LuaScriptMgr.Push(L, texture.texelSize);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_masterTextureLimit(IntPtr L)
	{
		Texture.masterTextureLimit = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_anisotropicFiltering(IntPtr L)
	{
		Texture.anisotropicFiltering = (AnisotropicFiltering)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(AnisotropicFiltering)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_width(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
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
		texture.width = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_height(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
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
		texture.height = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_filterMode(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name filterMode");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index filterMode on a nil value");
			}
		}
		texture.filterMode = (FilterMode)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(FilterMode)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_anisoLevel(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name anisoLevel");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index anisoLevel on a nil value");
			}
		}
		texture.anisoLevel = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_wrapMode(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name wrapMode");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
			}
		}
		texture.wrapMode = (TextureWrapMode)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(TextureWrapMode)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_mipMapBias(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Texture texture = (Texture)luaObject;
		if (texture == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name mipMapBias");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index mipMapBias on a nil value");
			}
		}
		texture.mipMapBias = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetGlobalAnisotropicFilteringLimits(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int forcedMin = (int)LuaScriptMgr.GetNumber(L, 1);
		int globalMax = (int)LuaScriptMgr.GetNumber(L, 2);
		Texture.SetGlobalAnisotropicFilteringLimits(forcedMin, globalMax);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetNativeTexturePtr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Texture texture = (Texture)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Texture");
		IntPtr nativeTexturePtr = texture.GetNativeTexturePtr();
		LuaScriptMgr.Push(L, nativeTexturePtr);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetNativeTextureID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Texture texture = (Texture)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Texture");
		int nativeTextureID = texture.GetNativeTextureID();
		LuaScriptMgr.Push(L, nativeTextureID);
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

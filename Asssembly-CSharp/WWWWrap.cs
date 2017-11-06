using com.tencent.pandora;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WWWWrap
{
	private static Type classType = typeof(WWW);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Dispose", new LuaCSFunction(WWWWrap.Dispose)),
			new LuaMethod("InitWWW", new LuaCSFunction(WWWWrap.InitWWW)),
			new LuaMethod("EscapeURL", new LuaCSFunction(WWWWrap.EscapeURL)),
			new LuaMethod("UnEscapeURL", new LuaCSFunction(WWWWrap.UnEscapeURL)),
			new LuaMethod("GetAudioClip", new LuaCSFunction(WWWWrap.GetAudioClip)),
			new LuaMethod("GetAudioClipCompressed", new LuaCSFunction(WWWWrap.GetAudioClipCompressed)),
			new LuaMethod("LoadImageIntoTexture", new LuaCSFunction(WWWWrap.LoadImageIntoTexture)),
			new LuaMethod("LoadUnityWeb", new LuaCSFunction(WWWWrap.LoadUnityWeb)),
			new LuaMethod("LoadFromCacheOrDownload", new LuaCSFunction(WWWWrap.LoadFromCacheOrDownload)),
			new LuaMethod("New", new LuaCSFunction(WWWWrap._CreateWWW)),
			new LuaMethod("GetClassType", new LuaCSFunction(WWWWrap.GetClassType))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("responseHeaders", new LuaCSFunction(WWWWrap.get_responseHeaders), null),
			new LuaField("text", new LuaCSFunction(WWWWrap.get_text), null),
			new LuaField("bytes", new LuaCSFunction(WWWWrap.get_bytes), null),
			new LuaField("size", new LuaCSFunction(WWWWrap.get_size), null),
			new LuaField("error", new LuaCSFunction(WWWWrap.get_error), null),
			new LuaField("texture", new LuaCSFunction(WWWWrap.get_texture), null),
			new LuaField("textureNonReadable", new LuaCSFunction(WWWWrap.get_textureNonReadable), null),
			new LuaField("audioClip", new LuaCSFunction(WWWWrap.get_audioClip), null),
			new LuaField("isDone", new LuaCSFunction(WWWWrap.get_isDone), null),
			new LuaField("progress", new LuaCSFunction(WWWWrap.get_progress), null),
			new LuaField("uploadProgress", new LuaCSFunction(WWWWrap.get_uploadProgress), null),
			new LuaField("bytesDownloaded", new LuaCSFunction(WWWWrap.get_bytesDownloaded), null),
			new LuaField("url", new LuaCSFunction(WWWWrap.get_url), null),
			new LuaField("assetBundle", new LuaCSFunction(WWWWrap.get_assetBundle), null),
			new LuaField("threadPriority", new LuaCSFunction(WWWWrap.get_threadPriority), new LuaCSFunction(WWWWrap.set_threadPriority))
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.WWW", typeof(WWW), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateWWW(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			WWW o = new WWW(@string);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(byte[])))
		{
			string string2 = LuaScriptMgr.GetString(L, 1);
			byte[] arrayNumber = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
			WWW o2 = new WWW(string2, arrayNumber);
			LuaScriptMgr.PushObject(L, o2);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(WWWForm)))
		{
			string string3 = LuaScriptMgr.GetString(L, 1);
			WWWForm form = (WWWForm)LuaScriptMgr.GetNetObject(L, 2, typeof(WWWForm));
			WWW o3 = new WWW(string3, form);
			LuaScriptMgr.PushObject(L, o3);
			return 1;
		}
		if (num == 3)
		{
			string string4 = LuaScriptMgr.GetString(L, 1);
			byte[] arrayNumber2 = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
			Dictionary<string, string> headers = (Dictionary<string, string>)LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, string>));
			WWW o4 = new WWW(string4, arrayNumber2, headers);
			LuaScriptMgr.PushObject(L, o4);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, WWWWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_responseHeaders(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name responseHeaders");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index responseHeaders on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, wWW.responseHeaders);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_text(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name text");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index text on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.text);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_bytes(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name bytes");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index bytes on a nil value");
			}
		}
		LuaScriptMgr.PushArray(L, wWW.bytes);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_size(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
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
		LuaScriptMgr.Push(L, wWW.size);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_error(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name error");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index error on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.error);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_texture(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name texture");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index texture on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.texture);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_textureNonReadable(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name textureNonReadable");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index textureNonReadable on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.textureNonReadable);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_audioClip(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name audioClip");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index audioClip on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.audioClip);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_isDone(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isDone");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isDone on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.isDone);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_progress(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name progress");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index progress on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.progress);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_uploadProgress(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name uploadProgress");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index uploadProgress on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.uploadProgress);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_bytesDownloaded(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name bytesDownloaded");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index bytesDownloaded on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.bytesDownloaded);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_url(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name url");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index url on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.url);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_assetBundle(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name assetBundle");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index assetBundle on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.assetBundle);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_threadPriority(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name threadPriority");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index threadPriority on a nil value");
			}
		}
		LuaScriptMgr.Push(L, wWW.threadPriority);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_threadPriority(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		WWW wWW = (WWW)luaObject;
		if (wWW == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name threadPriority");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index threadPriority on a nil value");
			}
		}
		wWW.threadPriority = (ThreadPriority)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(ThreadPriority)));
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Dispose(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
		wWW.Dispose();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int InitWWW(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		byte[] arrayNumber = LuaScriptMgr.GetArrayNumber<byte>(L, 3);
		string[] arrayString = LuaScriptMgr.GetArrayString(L, 4);
		wWW.InitWWW(luaString, arrayNumber, arrayString);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int EscapeURL(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			string str = WWW.EscapeURL(luaString);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			Encoding e = (Encoding)LuaScriptMgr.GetNetObject(L, 2, typeof(Encoding));
			string str2 = WWW.EscapeURL(luaString2, e);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.EscapeURL");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int UnEscapeURL(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			string str = WWW.UnEscapeURL(luaString);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			Encoding e = (Encoding)LuaScriptMgr.GetNetObject(L, 2, typeof(Encoding));
			string str2 = WWW.UnEscapeURL(luaString2, e);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.UnEscapeURL");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetAudioClip(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			bool boolean = LuaScriptMgr.GetBoolean(L, 2);
			AudioClip audioClip = wWW.GetAudioClip(boolean);
			LuaScriptMgr.Push(L, audioClip);
			return 1;
		}
		if (num == 3)
		{
			WWW wWW2 = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			bool boolean2 = LuaScriptMgr.GetBoolean(L, 2);
			bool boolean3 = LuaScriptMgr.GetBoolean(L, 3);
			AudioClip audioClip2 = wWW2.GetAudioClip(boolean2, boolean3);
			LuaScriptMgr.Push(L, audioClip2);
			return 1;
		}
		if (num == 4)
		{
			WWW wWW3 = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			bool boolean4 = LuaScriptMgr.GetBoolean(L, 2);
			bool boolean5 = LuaScriptMgr.GetBoolean(L, 3);
			AudioType audioType = (AudioType)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(AudioType)));
			AudioClip audioClip3 = wWW3.GetAudioClip(boolean4, boolean5, audioType);
			LuaScriptMgr.Push(L, audioClip3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.GetAudioClip");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetAudioClipCompressed(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			AudioClip audioClipCompressed = wWW.GetAudioClipCompressed();
			LuaScriptMgr.Push(L, audioClipCompressed);
			return 1;
		}
		if (num == 2)
		{
			WWW wWW2 = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			bool boolean = LuaScriptMgr.GetBoolean(L, 2);
			AudioClip audioClipCompressed2 = wWW2.GetAudioClipCompressed(boolean);
			LuaScriptMgr.Push(L, audioClipCompressed2);
			return 1;
		}
		if (num == 3)
		{
			WWW wWW3 = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
			bool boolean2 = LuaScriptMgr.GetBoolean(L, 2);
			AudioType audioType = (AudioType)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(AudioType)));
			AudioClip audioClipCompressed3 = wWW3.GetAudioClipCompressed(boolean2, audioType);
			LuaScriptMgr.Push(L, audioClipCompressed3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.GetAudioClipCompressed");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LoadImageIntoTexture(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
		Texture2D tex = (Texture2D)LuaScriptMgr.GetUnityObject(L, 2, typeof(Texture2D));
		wWW.LoadImageIntoTexture(tex);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LoadUnityWeb(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WWW wWW = (WWW)LuaScriptMgr.GetNetObjectSelf(L, 1, "WWW");
		wWW.LoadUnityWeb();
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LoadFromCacheOrDownload(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			int version = (int)LuaScriptMgr.GetNumber(L, 2);
			WWW o = WWW.LoadFromCacheOrDownload(luaString, version);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		if (num == 3)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			int version2 = (int)LuaScriptMgr.GetNumber(L, 2);
			uint crc = (uint)LuaScriptMgr.GetNumber(L, 3);
			WWW o2 = WWW.LoadFromCacheOrDownload(luaString2, version2, crc);
			LuaScriptMgr.PushObject(L, o2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: WWW.LoadFromCacheOrDownload");
		return 0;
	}
}

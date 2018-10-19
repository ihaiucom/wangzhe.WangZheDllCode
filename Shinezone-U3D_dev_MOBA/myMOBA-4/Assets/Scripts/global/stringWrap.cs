using com.tencent.pandora;
using System;
using System.Globalization;
using System.Text;

public class stringWrap
{
	private static Type classType = typeof(string);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Clone", new LuaCSFunction(stringWrap.Clone)),
			new LuaMethod("GetTypeCode", new LuaCSFunction(stringWrap.GetTypeCode)),
			new LuaMethod("CopyTo", new LuaCSFunction(stringWrap.CopyTo)),
			new LuaMethod("ToCharArray", new LuaCSFunction(stringWrap.ToCharArray)),
			new LuaMethod("Split", new LuaCSFunction(stringWrap.Split)),
			new LuaMethod("Substring", new LuaCSFunction(stringWrap.Substring)),
			new LuaMethod("Trim", new LuaCSFunction(stringWrap.Trim)),
			new LuaMethod("TrimStart", new LuaCSFunction(stringWrap.TrimStart)),
			new LuaMethod("TrimEnd", new LuaCSFunction(stringWrap.TrimEnd)),
			new LuaMethod("Compare", new LuaCSFunction(stringWrap.Compare)),
			new LuaMethod("CompareTo", new LuaCSFunction(stringWrap.CompareTo)),
			new LuaMethod("CompareOrdinal", new LuaCSFunction(stringWrap.CompareOrdinal)),
			new LuaMethod("EndsWith", new LuaCSFunction(stringWrap.EndsWith)),
			new LuaMethod("IndexOfAny", new LuaCSFunction(stringWrap.IndexOfAny)),
			new LuaMethod("IndexOf", new LuaCSFunction(stringWrap.IndexOf)),
			new LuaMethod("LastIndexOf", new LuaCSFunction(stringWrap.LastIndexOf)),
			new LuaMethod("LastIndexOfAny", new LuaCSFunction(stringWrap.LastIndexOfAny)),
			new LuaMethod("Contains", new LuaCSFunction(stringWrap.Contains)),
			new LuaMethod("IsNullOrEmpty", new LuaCSFunction(stringWrap.IsNullOrEmpty)),
			new LuaMethod("Normalize", new LuaCSFunction(stringWrap.Normalize)),
			new LuaMethod("IsNormalized", new LuaCSFunction(stringWrap.IsNormalized)),
			new LuaMethod("Remove", new LuaCSFunction(stringWrap.Remove)),
			new LuaMethod("PadLeft", new LuaCSFunction(stringWrap.PadLeft)),
			new LuaMethod("PadRight", new LuaCSFunction(stringWrap.PadRight)),
			new LuaMethod("StartsWith", new LuaCSFunction(stringWrap.StartsWith)),
			new LuaMethod("Replace", new LuaCSFunction(stringWrap.Replace)),
			new LuaMethod("ToLower", new LuaCSFunction(stringWrap.ToLower)),
			new LuaMethod("ToLowerInvariant", new LuaCSFunction(stringWrap.ToLowerInvariant)),
			new LuaMethod("ToUpper", new LuaCSFunction(stringWrap.ToUpper)),
			new LuaMethod("ToUpperInvariant", new LuaCSFunction(stringWrap.ToUpperInvariant)),
			new LuaMethod("ToString", new LuaCSFunction(stringWrap.ToString)),
			new LuaMethod("Format", new LuaCSFunction(stringWrap.Format)),
			new LuaMethod("Copy", new LuaCSFunction(stringWrap.Copy)),
			new LuaMethod("Concat", new LuaCSFunction(stringWrap.Concat)),
			new LuaMethod("Insert", new LuaCSFunction(stringWrap.Insert)),
			new LuaMethod("Intern", new LuaCSFunction(stringWrap.Intern)),
			new LuaMethod("IsInterned", new LuaCSFunction(stringWrap.IsInterned)),
			new LuaMethod("Join", new LuaCSFunction(stringWrap.Join)),
			new LuaMethod("GetEnumerator", new LuaCSFunction(stringWrap.GetEnumerator)),
			new LuaMethod("GetHashCode", new LuaCSFunction(stringWrap.GetHashCode)),
			new LuaMethod("Equals", new LuaCSFunction(stringWrap.Equals)),
			new LuaMethod("New", new LuaCSFunction(stringWrap._Createstring)),
			new LuaMethod("GetClassType", new LuaCSFunction(stringWrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(stringWrap.Lua_ToString)),
			new LuaMethod("__eq", new LuaCSFunction(stringWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("Empty", new LuaCSFunction(stringWrap.get_Empty), null),
			new LuaField("Length", new LuaCSFunction(stringWrap.get_Length), null)
		};
		LuaScriptMgr.RegisterLib(L, "System.String", typeof(string), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _Createstring(IntPtr L)
	{
		LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
		if (luaTypes == LuaTypes.LUA_TSTRING)
		{
			string o = LuaDLL.lua_tostring(L, 1);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: String.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, stringWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Empty(IntPtr L)
	{
		LuaScriptMgr.Push(L, string.Empty);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Length(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		string text = (string)luaObject;
		if (text == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Length");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Length on a nil value");
			}
		}
		LuaScriptMgr.Push(L, text.Length);
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
			LuaScriptMgr.Push(L, "Table: System.String");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Clone(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		object o = text.Clone();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		TypeCode typeCode = text.GetTypeCode();
		LuaScriptMgr.Push(L, typeCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CopyTo(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		int sourceIndex = (int)LuaScriptMgr.GetNumber(L, 2);
		char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 3);
		int destinationIndex = (int)LuaScriptMgr.GetNumber(L, 4);
		int count = (int)LuaScriptMgr.GetNumber(L, 5);
		text.CopyTo(sourceIndex, arrayNumber, destinationIndex, count);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToCharArray(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] o = text.ToCharArray();
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 2);
			int length = (int)LuaScriptMgr.GetNumber(L, 3);
			char[] o2 = text2.ToCharArray(startIndex, length);
			LuaScriptMgr.PushArray(L, o2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.ToCharArray");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Split(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(StringSplitOptions)))
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			StringSplitOptions options = (StringSplitOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			string[] o = text.Split(arrayNumber, options);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(int)))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber2 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int count = (int)LuaDLL.lua_tonumber(L, 3);
			string[] o2 = text2.Split(arrayNumber2, count);
			LuaScriptMgr.PushArray(L, o2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string[]), typeof(StringSplitOptions)))
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string[] arrayString = LuaScriptMgr.GetArrayString(L, 2);
			StringSplitOptions options2 = (StringSplitOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			string[] o3 = text3.Split(arrayString, options2);
			LuaScriptMgr.PushArray(L, o3);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string[]), typeof(int), typeof(StringSplitOptions)))
		{
			string text4 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string[] arrayString2 = LuaScriptMgr.GetArrayString(L, 2);
			int count2 = (int)LuaDLL.lua_tonumber(L, 3);
			StringSplitOptions options3 = (StringSplitOptions)((int)LuaScriptMgr.GetLuaObject(L, 4));
			string[] o4 = text4.Split(arrayString2, count2, options3);
			LuaScriptMgr.PushArray(L, o4);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(int), typeof(StringSplitOptions)))
		{
			string text5 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int count3 = (int)LuaDLL.lua_tonumber(L, 3);
			StringSplitOptions options4 = (StringSplitOptions)((int)LuaScriptMgr.GetLuaObject(L, 4));
			string[] o5 = text5.Split(arrayNumber3, count3, options4);
			LuaScriptMgr.PushArray(L, o5);
			return 1;
		}
		if (LuaScriptMgr.CheckParamsType(L, typeof(char), 2, num - 1))
		{
			string text6 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber4 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			string[] o6 = text6.Split(arrayNumber4);
			LuaScriptMgr.PushArray(L, o6);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Split");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Substring(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 2);
			string str = text.Substring(startIndex);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int startIndex2 = (int)LuaScriptMgr.GetNumber(L, 2);
			int length = (int)LuaScriptMgr.GetNumber(L, 3);
			string str2 = text2.Substring(startIndex2, length);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Substring");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Trim(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string str = text.Trim();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (LuaScriptMgr.CheckParamsType(L, typeof(char), 2, num - 1))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			string str2 = text2.Trim(arrayNumber);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Trim");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int TrimStart(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
		string str = text.TrimStart(arrayNumber);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int TrimEnd(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
		string str = text.TrimEnd(arrayNumber);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Compare(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			int d = string.Compare(luaString, luaString2);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 3));
			int d2 = string.Compare(@string, string2, comparisonType);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(bool)))
		{
			string string3 = LuaScriptMgr.GetString(L, 1);
			string string4 = LuaScriptMgr.GetString(L, 2);
			bool ignoreCase = LuaDLL.lua_toboolean(L, 3);
			int d3 = string.Compare(string3, string4, ignoreCase);
			LuaScriptMgr.Push(L, d3);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(CultureInfo), typeof(CompareOptions)))
		{
			string string5 = LuaScriptMgr.GetString(L, 1);
			string string6 = LuaScriptMgr.GetString(L, 2);
			CultureInfo culture = (CultureInfo)LuaScriptMgr.GetLuaObject(L, 3);
			CompareOptions options = (CompareOptions)((int)LuaScriptMgr.GetLuaObject(L, 4));
			int d4 = string.Compare(string5, string6, culture, options);
			LuaScriptMgr.Push(L, d4);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(bool), typeof(CultureInfo)))
		{
			string string7 = LuaScriptMgr.GetString(L, 1);
			string string8 = LuaScriptMgr.GetString(L, 2);
			bool ignoreCase2 = LuaDLL.lua_toboolean(L, 3);
			CultureInfo culture2 = (CultureInfo)LuaScriptMgr.GetLuaObject(L, 4);
			int d5 = string.Compare(string7, string8, ignoreCase2, culture2);
			LuaScriptMgr.Push(L, d5);
			return 1;
		}
		if (num == 5)
		{
			string luaString3 = LuaScriptMgr.GetLuaString(L, 1);
			int indexA = (int)LuaScriptMgr.GetNumber(L, 2);
			string luaString4 = LuaScriptMgr.GetLuaString(L, 3);
			int indexB = (int)LuaScriptMgr.GetNumber(L, 4);
			int length = (int)LuaScriptMgr.GetNumber(L, 5);
			int d6 = string.Compare(luaString3, indexA, luaString4, indexB, length);
			LuaScriptMgr.Push(L, d6);
			return 1;
		}
		if (num == 6 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(StringComparison)))
		{
			string string9 = LuaScriptMgr.GetString(L, 1);
			int indexA2 = (int)LuaDLL.lua_tonumber(L, 2);
			string string10 = LuaScriptMgr.GetString(L, 3);
			int indexB2 = (int)LuaDLL.lua_tonumber(L, 4);
			int length2 = (int)LuaDLL.lua_tonumber(L, 5);
			StringComparison comparisonType2 = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 6));
			int d7 = string.Compare(string9, indexA2, string10, indexB2, length2, comparisonType2);
			LuaScriptMgr.Push(L, d7);
			return 1;
		}
		if (num == 6 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(bool)))
		{
			string string11 = LuaScriptMgr.GetString(L, 1);
			int indexA3 = (int)LuaDLL.lua_tonumber(L, 2);
			string string12 = LuaScriptMgr.GetString(L, 3);
			int indexB3 = (int)LuaDLL.lua_tonumber(L, 4);
			int length3 = (int)LuaDLL.lua_tonumber(L, 5);
			bool ignoreCase3 = LuaDLL.lua_toboolean(L, 6);
			int d8 = string.Compare(string11, indexA3, string12, indexB3, length3, ignoreCase3);
			LuaScriptMgr.Push(L, d8);
			return 1;
		}
		if (num == 7 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(CultureInfo), typeof(CompareOptions)))
		{
			string string13 = LuaScriptMgr.GetString(L, 1);
			int indexA4 = (int)LuaDLL.lua_tonumber(L, 2);
			string string14 = LuaScriptMgr.GetString(L, 3);
			int indexB4 = (int)LuaDLL.lua_tonumber(L, 4);
			int length4 = (int)LuaDLL.lua_tonumber(L, 5);
			CultureInfo culture3 = (CultureInfo)LuaScriptMgr.GetLuaObject(L, 6);
			CompareOptions options2 = (CompareOptions)((int)LuaScriptMgr.GetLuaObject(L, 7));
			int d9 = string.Compare(string13, indexA4, string14, indexB4, length4, culture3, options2);
			LuaScriptMgr.Push(L, d9);
			return 1;
		}
		if (num == 7 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(bool), typeof(CultureInfo)))
		{
			string string15 = LuaScriptMgr.GetString(L, 1);
			int indexA5 = (int)LuaDLL.lua_tonumber(L, 2);
			string string16 = LuaScriptMgr.GetString(L, 3);
			int indexB5 = (int)LuaDLL.lua_tonumber(L, 4);
			int length5 = (int)LuaDLL.lua_tonumber(L, 5);
			bool ignoreCase4 = LuaDLL.lua_toboolean(L, 6);
			CultureInfo culture4 = (CultureInfo)LuaScriptMgr.GetLuaObject(L, 7);
			int d10 = string.Compare(string15, indexA5, string16, indexB5, length5, ignoreCase4, culture4);
			LuaScriptMgr.Push(L, d10);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Compare");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CompareTo(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string @string = LuaScriptMgr.GetString(L, 2);
			int d = text.CompareTo(@string);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(object)))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			object varObject = LuaScriptMgr.GetVarObject(L, 2);
			int d2 = text2.CompareTo(varObject);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.CompareTo");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CompareOrdinal(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			int d = string.CompareOrdinal(luaString, luaString2);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 5)
		{
			string luaString3 = LuaScriptMgr.GetLuaString(L, 1);
			int indexA = (int)LuaScriptMgr.GetNumber(L, 2);
			string luaString4 = LuaScriptMgr.GetLuaString(L, 3);
			int indexB = (int)LuaScriptMgr.GetNumber(L, 4);
			int length = (int)LuaScriptMgr.GetNumber(L, 5);
			int d2 = string.CompareOrdinal(luaString3, indexA, luaString4, indexB, length);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.CompareOrdinal");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int EndsWith(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			bool b = text.EndsWith(luaString);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
			bool b2 = text2.EndsWith(luaString2, comparisonType);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		if (num == 4)
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			CultureInfo culture = (CultureInfo)LuaScriptMgr.GetNetObject(L, 4, typeof(CultureInfo));
			bool b3 = text3.EndsWith(luaString3, boolean, culture);
			LuaScriptMgr.Push(L, b3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.EndsWith");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IndexOfAny(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int d = text.IndexOfAny(arrayNumber);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber2 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 3);
			int d2 = text2.IndexOfAny(arrayNumber2, startIndex);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		if (num == 4)
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int startIndex2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int count = (int)LuaScriptMgr.GetNumber(L, 4);
			int d3 = text3.IndexOfAny(arrayNumber3, startIndex2, count);
			LuaScriptMgr.Push(L, d3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.IndexOfAny");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IndexOf(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char)))
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value = (char)LuaDLL.lua_tonumber(L, 2);
			int d = text.IndexOf(value);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string @string = LuaScriptMgr.GetString(L, 2);
			int d2 = text2.IndexOf(@string);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int)))
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string2 = LuaScriptMgr.GetString(L, 2);
			int startIndex = (int)LuaDLL.lua_tonumber(L, 3);
			int d3 = text3.IndexOf(string2, startIndex);
			LuaScriptMgr.Push(L, d3);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int)))
		{
			string text4 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value2 = (char)LuaDLL.lua_tonumber(L, 2);
			int startIndex2 = (int)LuaDLL.lua_tonumber(L, 3);
			int d4 = text4.IndexOf(value2, startIndex2);
			LuaScriptMgr.Push(L, d4);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
		{
			string text5 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string3 = LuaScriptMgr.GetString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 3));
			int d5 = text5.IndexOf(string3, comparisonType);
			LuaScriptMgr.Push(L, d5);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(int)))
		{
			string text6 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string4 = LuaScriptMgr.GetString(L, 2);
			int startIndex3 = (int)LuaDLL.lua_tonumber(L, 3);
			int count = (int)LuaDLL.lua_tonumber(L, 4);
			int d6 = text6.IndexOf(string4, startIndex3, count);
			LuaScriptMgr.Push(L, d6);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(StringComparison)))
		{
			string text7 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string5 = LuaScriptMgr.GetString(L, 2);
			int startIndex4 = (int)LuaDLL.lua_tonumber(L, 3);
			StringComparison comparisonType2 = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 4));
			int d7 = text7.IndexOf(string5, startIndex4, comparisonType2);
			LuaScriptMgr.Push(L, d7);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int), typeof(int)))
		{
			string text8 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value3 = (char)LuaDLL.lua_tonumber(L, 2);
			int startIndex5 = (int)LuaDLL.lua_tonumber(L, 3);
			int count2 = (int)LuaDLL.lua_tonumber(L, 4);
			int d8 = text8.IndexOf(value3, startIndex5, count2);
			LuaScriptMgr.Push(L, d8);
			return 1;
		}
		if (num == 5)
		{
			string text9 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			int startIndex6 = (int)LuaScriptMgr.GetNumber(L, 3);
			int count3 = (int)LuaScriptMgr.GetNumber(L, 4);
			StringComparison comparisonType3 = (StringComparison)((int)LuaScriptMgr.GetNetObject(L, 5, typeof(StringComparison)));
			int d9 = text9.IndexOf(luaString, startIndex6, count3, comparisonType3);
			LuaScriptMgr.Push(L, d9);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.IndexOf");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LastIndexOf(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char)))
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value = (char)LuaDLL.lua_tonumber(L, 2);
			int d = text.LastIndexOf(value);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string @string = LuaScriptMgr.GetString(L, 2);
			int d2 = text2.LastIndexOf(@string);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int)))
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string2 = LuaScriptMgr.GetString(L, 2);
			int startIndex = (int)LuaDLL.lua_tonumber(L, 3);
			int d3 = text3.LastIndexOf(string2, startIndex);
			LuaScriptMgr.Push(L, d3);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int)))
		{
			string text4 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value2 = (char)LuaDLL.lua_tonumber(L, 2);
			int startIndex2 = (int)LuaDLL.lua_tonumber(L, 3);
			int d4 = text4.LastIndexOf(value2, startIndex2);
			LuaScriptMgr.Push(L, d4);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
		{
			string text5 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string3 = LuaScriptMgr.GetString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 3));
			int d5 = text5.LastIndexOf(string3, comparisonType);
			LuaScriptMgr.Push(L, d5);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(int)))
		{
			string text6 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string4 = LuaScriptMgr.GetString(L, 2);
			int startIndex3 = (int)LuaDLL.lua_tonumber(L, 3);
			int count = (int)LuaDLL.lua_tonumber(L, 4);
			int d6 = text6.LastIndexOf(string4, startIndex3, count);
			LuaScriptMgr.Push(L, d6);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(StringComparison)))
		{
			string text7 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string string5 = LuaScriptMgr.GetString(L, 2);
			int startIndex4 = (int)LuaDLL.lua_tonumber(L, 3);
			StringComparison comparisonType2 = (StringComparison)((int)LuaScriptMgr.GetLuaObject(L, 4));
			int d7 = text7.LastIndexOf(string5, startIndex4, comparisonType2);
			LuaScriptMgr.Push(L, d7);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int), typeof(int)))
		{
			string text8 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char value3 = (char)LuaDLL.lua_tonumber(L, 2);
			int startIndex5 = (int)LuaDLL.lua_tonumber(L, 3);
			int count2 = (int)LuaDLL.lua_tonumber(L, 4);
			int d8 = text8.LastIndexOf(value3, startIndex5, count2);
			LuaScriptMgr.Push(L, d8);
			return 1;
		}
		if (num == 5)
		{
			string text9 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			int startIndex6 = (int)LuaScriptMgr.GetNumber(L, 3);
			int count3 = (int)LuaScriptMgr.GetNumber(L, 4);
			StringComparison comparisonType3 = (StringComparison)((int)LuaScriptMgr.GetNetObject(L, 5, typeof(StringComparison)));
			int d9 = text9.LastIndexOf(luaString, startIndex6, count3, comparisonType3);
			LuaScriptMgr.Push(L, d9);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.LastIndexOf");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LastIndexOfAny(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int d = text.LastIndexOfAny(arrayNumber);
			LuaScriptMgr.Push(L, d);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber2 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 3);
			int d2 = text2.LastIndexOfAny(arrayNumber2, startIndex);
			LuaScriptMgr.Push(L, d2);
			return 1;
		}
		if (num == 4)
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char[] arrayNumber3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
			int startIndex2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int count = (int)LuaScriptMgr.GetNumber(L, 4);
			int d3 = text3.LastIndexOfAny(arrayNumber3, startIndex2, count);
			LuaScriptMgr.Push(L, d3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.LastIndexOfAny");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Contains(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		bool b = text.Contains(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsNullOrEmpty(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool b = string.IsNullOrEmpty(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Normalize(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string str = text.Normalize();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			NormalizationForm normalizationForm = (NormalizationForm)((int)LuaScriptMgr.GetNetObject(L, 2, typeof(NormalizationForm)));
			string str2 = text2.Normalize(normalizationForm);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Normalize");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsNormalized(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			bool b = text.IsNormalized();
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 2)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			NormalizationForm normalizationForm = (NormalizationForm)((int)LuaScriptMgr.GetNetObject(L, 2, typeof(NormalizationForm)));
			bool b2 = text2.IsNormalized(normalizationForm);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.IsNormalized");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Remove(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 2);
			string str = text.Remove(startIndex);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int startIndex2 = (int)LuaScriptMgr.GetNumber(L, 2);
			int count = (int)LuaScriptMgr.GetNumber(L, 3);
			string str2 = text2.Remove(startIndex2, count);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Remove");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int PadLeft(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int totalWidth = (int)LuaScriptMgr.GetNumber(L, 2);
			string str = text.PadLeft(totalWidth);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int totalWidth2 = (int)LuaScriptMgr.GetNumber(L, 2);
			char paddingChar = (char)LuaScriptMgr.GetNumber(L, 3);
			string str2 = text2.PadLeft(totalWidth2, paddingChar);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.PadLeft");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int PadRight(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int totalWidth = (int)LuaScriptMgr.GetNumber(L, 2);
			string str = text.PadRight(totalWidth);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			int totalWidth2 = (int)LuaScriptMgr.GetNumber(L, 2);
			char paddingChar = (char)LuaScriptMgr.GetNumber(L, 3);
			string str2 = text2.PadRight(totalWidth2, paddingChar);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.PadRight");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int StartsWith(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			bool b = text.StartsWith(luaString);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 3)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
			bool b2 = text2.StartsWith(luaString2, comparisonType);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		if (num == 4)
		{
			string text3 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			CultureInfo culture = (CultureInfo)LuaScriptMgr.GetNetObject(L, 4, typeof(CultureInfo));
			bool b3 = text3.StartsWith(luaString3, boolean, culture);
			LuaScriptMgr.Push(L, b3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.StartsWith");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Replace(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string)))
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string @string = LuaScriptMgr.GetString(L, 2);
			string string2 = LuaScriptMgr.GetString(L, 3);
			string str = text.Replace(@string, string2);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(char)))
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			char oldChar = (char)LuaDLL.lua_tonumber(L, 2);
			char newChar = (char)LuaDLL.lua_tonumber(L, 3);
			string str2 = text2.Replace(oldChar, newChar);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Replace");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToLower(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string str = text.ToLower();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			CultureInfo culture = (CultureInfo)LuaScriptMgr.GetNetObject(L, 2, typeof(CultureInfo));
			string str2 = text2.ToLower(culture);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.ToLower");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToLowerInvariant(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		string str = text.ToLowerInvariant();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToUpper(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string str = text.ToUpper();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			CultureInfo culture = (CultureInfo)LuaScriptMgr.GetNetObject(L, 2, typeof(CultureInfo));
			string str2 = text2.ToUpper(culture);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.ToUpper");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToUpperInvariant(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		string str = text.ToUpperInvariant();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			string str = text.ToString();
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2)
		{
			string text2 = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
			IFormatProvider provider = (IFormatProvider)LuaScriptMgr.GetNetObject(L, 2, typeof(IFormatProvider));
			string str2 = text2.ToString(provider);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.ToString");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Format(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			object varObject = LuaScriptMgr.GetVarObject(L, 2);
			string str = string.Format(luaString, varObject);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(object), typeof(object)))
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 2);
			object varObject3 = LuaScriptMgr.GetVarObject(L, 3);
			string str2 = string.Format(@string, varObject2, varObject3);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		if (num == 4)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			object varObject4 = LuaScriptMgr.GetVarObject(L, 2);
			object varObject5 = LuaScriptMgr.GetVarObject(L, 3);
			object varObject6 = LuaScriptMgr.GetVarObject(L, 4);
			string str3 = string.Format(luaString2, varObject4, varObject5, varObject6);
			LuaScriptMgr.Push(L, str3);
			return 1;
		}
		if (LuaScriptMgr.CheckTypes(L, 1, typeof(IFormatProvider), typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(object), 3, num - 2))
		{
			IFormatProvider provider = (IFormatProvider)LuaScriptMgr.GetLuaObject(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			object[] paramsObject = LuaScriptMgr.GetParamsObject(L, 3, num - 2);
			string str4 = string.Format(provider, string2, paramsObject);
			LuaScriptMgr.Push(L, str4);
			return 1;
		}
		if (LuaScriptMgr.CheckTypes(L, 1, typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(object), 2, num - 1))
		{
			string string3 = LuaScriptMgr.GetString(L, 1);
			object[] paramsObject2 = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
			string str5 = string.Format(string3, paramsObject2);
			LuaScriptMgr.Push(L, str5);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Format");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Copy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string str = string.Copy(luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Concat(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			object varObject = LuaScriptMgr.GetVarObject(L, 1);
			string str = string.Concat(varObject);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			string str2 = @string + string2;
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object)))
		{
			object varObject2 = LuaScriptMgr.GetVarObject(L, 1);
			object varObject3 = LuaScriptMgr.GetVarObject(L, 2);
			string str3 = System.String.Concat(varObject2, varObject3);
			LuaScriptMgr.Push(L, str3);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string)))
		{
			string string3 = LuaScriptMgr.GetString(L, 1);
			string string4 = LuaScriptMgr.GetString(L, 2);
			string string5 = LuaScriptMgr.GetString(L, 3);
			string str4 = string3 + string4 + string5;
			LuaScriptMgr.Push(L, str4);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object), typeof(object)))
		{
			object varObject4 = LuaScriptMgr.GetVarObject(L, 1);
			object varObject5 = LuaScriptMgr.GetVarObject(L, 2);
			object varObject6 = LuaScriptMgr.GetVarObject(L, 3);
            string str5 = System.String.Concat(varObject4, varObject5, varObject6);
			LuaScriptMgr.Push(L, str5);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string), typeof(string)))
		{
			string string6 = LuaScriptMgr.GetString(L, 1);
			string string7 = LuaScriptMgr.GetString(L, 2);
			string string8 = LuaScriptMgr.GetString(L, 3);
			string string9 = LuaScriptMgr.GetString(L, 4);
			string str6 = string6 + string7 + string8 + string9;
			LuaScriptMgr.Push(L, str6);
			return 1;
		}
		if (num == 4 && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object), typeof(object), typeof(object)))
		{
			object varObject7 = LuaScriptMgr.GetVarObject(L, 1);
			object varObject8 = LuaScriptMgr.GetVarObject(L, 2);
			object varObject9 = LuaScriptMgr.GetVarObject(L, 3);
			object varObject10 = LuaScriptMgr.GetVarObject(L, 4);
			string str7 = string.Concat(new object[]
			{
				varObject7,
				varObject8,
				varObject9,
				varObject10
			});
			LuaScriptMgr.Push(L, str7);
			return 1;
		}
		if (LuaScriptMgr.CheckParamsType(L, typeof(string), 1, num))
		{
			string[] paramsString = LuaScriptMgr.GetParamsString(L, 1, num);
			string str8 = string.Concat(paramsString);
			LuaScriptMgr.Push(L, str8);
			return 1;
		}
		if (LuaScriptMgr.CheckParamsType(L, typeof(object), 1, num))
		{
			object[] paramsObject = LuaScriptMgr.GetParamsObject(L, 1, num);
			string str9 = string.Concat(paramsObject);
			LuaScriptMgr.Push(L, str9);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Concat");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Insert(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		int startIndex = (int)LuaScriptMgr.GetNumber(L, 2);
		string luaString = LuaScriptMgr.GetLuaString(L, 3);
		string str = text.Insert(startIndex, luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Intern(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string str = string.Intern(luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsInterned(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string str = string.IsInterned(luaString);
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Join(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			string[] arrayString = LuaScriptMgr.GetArrayString(L, 2);
			string str = string.Join(luaString, arrayString);
			LuaScriptMgr.Push(L, str);
			return 1;
		}
		if (num == 4)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			string[] arrayString2 = LuaScriptMgr.GetArrayString(L, 2);
			int startIndex = (int)LuaScriptMgr.GetNumber(L, 3);
			int count = (int)LuaScriptMgr.GetNumber(L, 4);
			string str2 = string.Join(luaString2, arrayString2, startIndex, count);
			LuaScriptMgr.Push(L, str2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Join");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetEnumerator(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		CharEnumerator enumerator = text.GetEnumerator();
		LuaScriptMgr.Push(L, enumerator);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string text = (string)LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
		int hashCode = text.GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
		bool b = luaString == luaString2;
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 2, typeof(string)))
		{
			string text = LuaScriptMgr.GetVarObject(L, 1) as string;
			string @string = LuaScriptMgr.GetString(L, 2);
			bool b = (text == null) ? (@string == null) : text.Equals(@string);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 2, typeof(object)))
		{
			string text2 = LuaScriptMgr.GetVarObject(L, 1) as string;
			object varObject = LuaScriptMgr.GetVarObject(L, 2);
			bool b2 = (text2 == null) ? (varObject == null) : text2.Equals(varObject);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		if (num == 3)
		{
			string text3 = LuaScriptMgr.GetVarObject(L, 1) as string;
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			StringComparison comparisonType = (StringComparison)((int)LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
			bool b3 = (text3 == null) ? (luaString == null) : text3.Equals(luaString, comparisonType);
			LuaScriptMgr.Push(L, b3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: string.Equals");
		return 0;
	}
}

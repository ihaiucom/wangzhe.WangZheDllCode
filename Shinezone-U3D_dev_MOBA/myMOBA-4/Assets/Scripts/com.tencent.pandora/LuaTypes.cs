using System;

namespace com.tencent.pandora
{
	public enum LuaTypes
	{
		LUA_TNONE = -1,
		LUA_TNIL,
		LUA_TNUMBER = 3,
		LUA_TSTRING,
		LUA_TBOOLEAN = 1,
		LUA_TTABLE = 5,
		LUA_TFUNCTION,
		LUA_TUSERDATA,
		LUA_TTHREAD,
		LUA_TLIGHTUSERDATA = 2
	}
}

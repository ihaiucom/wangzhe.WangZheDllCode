using System;

namespace com.tencent.pandora
{
	public enum LuaThreadStatus
	{
		LUA_YIELD = 1,
		LUA_ERRRUN,
		LUA_ERRSYNTAX,
		LUA_ERRMEM,
		LUA_ERRERR
	}
}

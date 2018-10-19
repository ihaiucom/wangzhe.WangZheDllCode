using System;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int LuaCSFunction(IntPtr luaState);
}

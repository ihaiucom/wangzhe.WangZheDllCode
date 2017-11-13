using System;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	[UnmanagedFunctionPointer]
	public delegate int LuaCSFunction(IntPtr luaState);
}

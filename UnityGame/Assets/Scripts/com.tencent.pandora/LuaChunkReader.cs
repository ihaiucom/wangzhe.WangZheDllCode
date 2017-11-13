using System;

namespace com.tencent.pandora
{
	public delegate string LuaChunkReader(IntPtr luaState, ref ReaderInfo data, ref uint size);
}

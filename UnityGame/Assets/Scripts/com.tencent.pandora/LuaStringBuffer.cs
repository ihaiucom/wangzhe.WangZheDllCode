using System;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	public class LuaStringBuffer
	{
		public byte[] buffer;

		public LuaStringBuffer(IntPtr source, int len)
		{
			this.buffer = new byte[len];
			Marshal.Copy(source, this.buffer, 0, len);
		}

		public LuaStringBuffer(byte[] buf)
		{
			this.buffer = buf;
		}
	}
}

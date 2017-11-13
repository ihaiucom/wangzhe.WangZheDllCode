using System;

namespace com.tencent.pandora
{
	public class LuaRef
	{
		public IntPtr L;

		public int reference;

		public LuaRef(IntPtr L, int reference)
		{
			this.L = L;
			this.reference = reference;
		}
	}
}

using System;

namespace com.tencent.pandora
{
	public static class ObjectExtends
	{
		public static object RefObject(this object obj)
		{
			return new WeakReference(obj).get_Target();
		}
	}
}

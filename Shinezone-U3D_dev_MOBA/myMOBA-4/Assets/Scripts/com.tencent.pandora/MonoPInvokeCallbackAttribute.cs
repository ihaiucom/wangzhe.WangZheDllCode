using System;

namespace com.tencent.pandora
{
	public class MonoPInvokeCallbackAttribute : Attribute
	{
		private Type type;

		public MonoPInvokeCallbackAttribute(Type t)
		{
			this.type = t;
		}
	}
}

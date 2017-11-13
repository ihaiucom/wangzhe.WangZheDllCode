using System;

namespace IIPSMobile
{
	[AttributeUsage]
	public sealed class MonoPInvokeCallbackAttribute : Attribute
	{
		public MonoPInvokeCallbackAttribute(Type t)
		{
		}
	}
}

using System;

namespace com.tencent.pandora
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class LuaHideAttribute : Attribute
	{
	}
}

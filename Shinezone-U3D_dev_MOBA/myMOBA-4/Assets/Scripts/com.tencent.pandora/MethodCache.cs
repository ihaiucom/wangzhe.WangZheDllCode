using System;
using System.Reflection;

namespace com.tencent.pandora
{
	internal struct MethodCache
	{
		private MethodBase _cachedMethod;

		public bool IsReturnVoid;

		public object[] args;

		public int[] outList;

		public MethodArgs[] argTypes;

		public MethodBase cachedMethod
		{
			get
			{
				return this._cachedMethod;
			}
			set
			{
				this._cachedMethod = value;
				MethodInfo methodInfo = value as MethodInfo;
				if (methodInfo != null)
				{
					this.IsReturnVoid = (methodInfo.ReturnType == typeof(void));
				}
			}
		}
	}
}

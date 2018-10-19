using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirDynamicInfo
	{
		[MarshalAs(UnmanagedType.LPStr)]
		public string appAttr;

		[MarshalAs(UnmanagedType.LPStr)]
		public string connectUrl;

		[MarshalAs(UnmanagedType.LPStr)]
		public string pingUrl;
	}
}

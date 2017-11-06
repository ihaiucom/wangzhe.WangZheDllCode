using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirDynamicInfo
	{
		[MarshalAs(20)]
		public string appAttr;

		[MarshalAs(20)]
		public string connectUrl;

		[MarshalAs(20)]
		public string pingUrl;
	}
}

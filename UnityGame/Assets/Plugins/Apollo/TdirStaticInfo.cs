using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirStaticInfo
	{
		public int cltAttr;

		public int cltAttr1;

		[MarshalAs(20)]
		public string appAttr;

		[MarshalAs(20)]
		public string curVersion;

		public int windowAttr;

		public int appID;

		public int cltFlag;

		public uint bitmapMask;

		[MarshalAs(20)]
		public string virConnUrl;
	}
}

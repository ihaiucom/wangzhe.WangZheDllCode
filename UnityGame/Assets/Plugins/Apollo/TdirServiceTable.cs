using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirServiceTable
	{
		public uint updateTime;

		public uint bitMap;

		public uint userAttr;

		public int zoneID;

		public uint appLen;

		[MarshalAs(30, SizeConst = 512)]
		public byte[] appBuff;
	}
}

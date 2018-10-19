using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirUserRoleInfo
	{
		public int zoneID;

		public ulong roleID;

		public uint lastLoginTime;

		[MarshalAs(UnmanagedType.LPStr)]
		public string roleName;

		[MarshalAs(UnmanagedType.LPStr)]
		public string roleLevel;

		public uint appLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] appBuff;
	}
}

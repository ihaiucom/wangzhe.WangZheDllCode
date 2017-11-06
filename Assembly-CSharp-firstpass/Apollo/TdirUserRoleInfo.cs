using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public struct TdirUserRoleInfo
	{
		public int zoneID;

		public ulong roleID;

		public uint lastLoginTime;

		[MarshalAs(20)]
		public string roleName;

		[MarshalAs(20)]
		public string roleLevel;

		public uint appLen;

		[MarshalAs(30, SizeConst = 256)]
		public byte[] appBuff;
	}
}

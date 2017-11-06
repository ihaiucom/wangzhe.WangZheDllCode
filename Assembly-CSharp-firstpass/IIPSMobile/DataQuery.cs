using System;
using System.Runtime.InteropServices;

namespace IIPSMobile
{
	public class DataQuery : IIPSMobileDataQueryInterface
	{
		public struct IIPS_FIND_FILE_INFO
		{
			public uint fileId;

			public uint fileSize;

			public byte isDirectory;
		}

		public struct IIPS_PACKAGE_INFO
		{
			[MarshalAs(30, SizeConst = 260)]
			public byte[] szPackageName;

			[MarshalAs(30, SizeConst = 260)]
			public byte[] szPackageFilePath;

			public ulong currentSize;

			public ulong totalSize;
		}

		public IntPtr mDataQuery = IntPtr.Zero;

		public DataQuery(IntPtr Query)
		{
			this.mDataQuery = Query;
		}

		[DllImport("apollo", ExactSpelling = true)]
		private static extern IntPtr GetIFileName(IntPtr dataQuery, uint fileId);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetIFileId(IntPtr dataQuery, [MarshalAs(20)] string szFileName);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetIFileSize(IntPtr dataQuery, uint fileId);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte IsIFileReady(IntPtr dataQuery, uint fileId);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte IsIFileDir(IntPtr dataQuery, uint fileId);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint IIPSFindFirstFile(IntPtr dataQuery, uint fileId, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte IIPSFindNextFile(IntPtr dataQuery, uint findHandle, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern byte IIPSFindClose(IntPtr dataQuery, uint findHandle);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetIfsPackagesInfo(IntPtr dataQuery, ref DataQuery.IIPS_PACKAGE_INFO pInfo, uint count);

		[DllImport("apollo", ExactSpelling = true)]
		private static extern uint GetLastDataQueryError(IntPtr dataQuery);

		public string GetFileName(uint fileId)
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return null;
			}
			IntPtr iFileName = DataQuery.GetIFileName(this.mDataQuery, fileId);
			return Marshal.PtrToStringAnsi(iFileName);
		}

		public uint GetFileId(string fileName)
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return 0u;
			}
			return DataQuery.GetIFileId(this.mDataQuery, fileName);
		}

		public uint GetFileSize(uint fileId)
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return 0u;
			}
			return DataQuery.GetIFileSize(this.mDataQuery, fileId);
		}

		public bool IsFileReady(uint fileId)
		{
			return !(this.mDataQuery == IntPtr.Zero) && DataQuery.IsIFileReady(this.mDataQuery, fileId) > 0;
		}

		public bool IsDirectory(uint fileId)
		{
			return !(this.mDataQuery == IntPtr.Zero) && DataQuery.IsIFileDir(this.mDataQuery, fileId) > 0;
		}

		public uint IIPSFindFirstFile(uint fileId, ref DataQuery.IIPS_FIND_FILE_INFO pInfo)
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return 0u;
			}
			return DataQuery.IIPSFindFirstFile(this.mDataQuery, fileId, ref pInfo);
		}

		public bool IIPSFindNextFile(uint findHandle, ref DataQuery.IIPS_FIND_FILE_INFO pInfo)
		{
			return !(this.mDataQuery == IntPtr.Zero) && DataQuery.IIPSFindNextFile(this.mDataQuery, findHandle, ref pInfo) > 0;
		}

		public bool IIPSFindClose(uint findHandle)
		{
			return !(this.mDataQuery == IntPtr.Zero) && DataQuery.IIPSFindClose(this.mDataQuery, findHandle) > 0;
		}

		public uint GetIfsPackagesInfo(ref DataQuery.IIPS_PACKAGE_INFO pInfo, uint count)
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return 0u;
			}
			return DataQuery.GetIfsPackagesInfo(this.mDataQuery, ref pInfo, count);
		}

		public uint GetLastDataQueryError()
		{
			if (this.mDataQuery == IntPtr.Zero)
			{
				return 0u;
			}
			return DataQuery.GetLastDataQueryError(this.mDataQuery);
		}
	}
}

using System;

namespace IIPSMobile
{
	public interface IIPSMobileDataQueryInterface
	{
		string GetFileName(uint fileId);

		uint GetFileId(string fileName);

		uint GetFileSize(uint fileId);

		bool IsFileReady(uint fileId);

		bool IsDirectory(uint fileId);

		uint IIPSFindFirstFile(uint fileId, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);

		bool IIPSFindNextFile(uint findHandle, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);

		bool IIPSFindClose(uint findHandle);

		uint GetIfsPackagesInfo(ref DataQuery.IIPS_PACKAGE_INFO pInfo, uint count);

		uint GetLastDataQueryError();
	}
}

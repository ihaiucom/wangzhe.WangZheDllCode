using System;

namespace IIPSMobile
{
	internal class IIPSMobileErrorCodeCheck
	{
		public enum first_module
		{
			first_module_init,
			first_module_data,
			first_module_version
		}

		public enum second_module
		{
			second_module_init,
			second_module_datamanager,
			second_module_datadownloader,
			second_module_dataqueryer,
			second_module_datareader,
			second_module_versionmanager = 1,
			second_module_version_action,
			second_module_update_action,
			second_module_extract_action,
			second_module_apkupdate_action,
			second_module_srcupdate_fulldiff_action,
			second_module_srcupdate_mergeifs_action,
			second_module_srcupdate_cures_action,
			second_module_srcupdate_filediff_action
		}

		public enum error_type_inside
		{
			error_type_inside_init,
			error_type_inside_download_error,
			error_type_inside_system_error,
			error_type_inside_module_error,
			error_type_inside_ifs_error,
			error_type_inside_should_not_happen
		}

		public enum error_type
		{
			error_type_init,
			error_type_network,
			error_type_net_timeout,
			error_type_device_hasno_space,
			error_type_other_system_error,
			error_type_other_error,
			error_type_cur_version_not_support_update,
			error_type_can_not_sure,
			error_type_cur_net_not_support_update
		}

		public struct ErrorCodeInfo
		{
			public bool m_bCheckOk;

			public int m_nErrorType;

			public int m_nFirstModule;

			public int m_nSecondModule;

			public int m_nInsideErrorType;

			public int m_nErrorCode;

			public int m_nLastCheckError;

			public ErrorCodeInfo(bool bOk)
			{
				this.m_bCheckOk = bOk;
				this.m_nFirstModule = 0;
				this.m_nSecondModule = 0;
				this.m_nErrorType = 0;
				this.m_nInsideErrorType = 0;
				this.m_nErrorCode = 0;
				this.m_nLastCheckError = 0;
			}
		}

		private int m_nLastCheckErrorCode;

		public IIPSMobileErrorCodeCheck.ErrorCodeInfo CheckIIPSErrorCode(int nErrorCode)
		{
			this.m_nLastCheckErrorCode = nErrorCode;
			int firstModuleType = this.GetFirstModuleType();
			int secondModuleType = this.GetSecondModuleType();
			int errorCodeType = this.GetErrorCodeType();
			int num = this.GetRealErrorCode();
			int nErrorType;
			if (errorCodeType == 1)
			{
				int downloadErrorType = this.GetDownloadErrorType(num);
				int num2 = this.GetReadDownloadErrorCode(num);
				if (downloadErrorType == 5)
				{
					if (num2 == 112 || num2 == 39 || num2 == 28)
					{
						nErrorType = 3;
					}
					else
					{
						nErrorType = 4;
					}
				}
				else if (downloadErrorType == 1)
				{
					nErrorType = 2;
					num2 = num;
				}
				else if (downloadErrorType == 2)
				{
					nErrorType = 1;
				}
				else
				{
					nErrorType = 5;
				}
				num = num2;
			}
			else if (errorCodeType == 2)
			{
				if (num == 112 || num == 39 || num == 28)
				{
					nErrorType = 3;
				}
				else
				{
					nErrorType = 4;
				}
			}
			else if (errorCodeType == 3)
			{
				if (firstModuleType == 1)
				{
					nErrorType = 5;
				}
				else if (firstModuleType == 2)
				{
					nErrorType = this.GetSecondModuleNoticeErrorType(secondModuleType, num);
				}
				else
				{
					nErrorType = 5;
				}
			}
			else if (errorCodeType == 4)
			{
				if (num == 112 || num == 39 || num == 28)
				{
					nErrorType = 3;
				}
				else
				{
					nErrorType = 4;
				}
			}
			else if (errorCodeType == 5)
			{
				nErrorType = 5;
			}
			else
			{
				nErrorType = 5;
			}
			IIPSMobileErrorCodeCheck.ErrorCodeInfo result = new IIPSMobileErrorCodeCheck.ErrorCodeInfo(false);
			result.m_bCheckOk = true;
			result.m_nErrorType = nErrorType;
			result.m_nFirstModule = firstModuleType;
			result.m_nSecondModule = secondModuleType;
			result.m_nInsideErrorType = errorCodeType;
			result.m_nErrorCode = num;
			result.m_nLastCheckError = this.m_nLastCheckErrorCode;
			return result;
		}

		private int GetFirstModuleType()
		{
			int num = this.m_nLastCheckErrorCode >> 23;
			return num & 7;
		}

		private int GetSecondModuleType()
		{
			int num = this.m_nLastCheckErrorCode >> 26;
			return num & 15;
		}

		private int GetErrorCodeType()
		{
			int num = this.m_nLastCheckErrorCode >> 20;
			return num & 7;
		}

		private int GetRealErrorCode()
		{
			return this.m_nLastCheckErrorCode & 1048575;
		}

		private int GetDownloadErrorType(int downloaderror)
		{
			int num = downloaderror >> 16;
			return num & 15;
		}

		private int GetReadDownloadErrorCode(int downloaderror)
		{
			return downloaderror & 65535;
		}

		private int GetSecondModuleNoticeErrorType(int secondModule, int errorcode)
		{
			int result;
			if (secondModule == 1)
			{
				result = 5;
			}
			else if (secondModule == 2)
			{
				if (errorcode == 15 || errorcode == 16 || errorcode == 17 || errorcode == 22)
				{
					result = 6;
				}
				else
				{
					result = 1;
				}
			}
			else if (secondModule == 5)
			{
				if (errorcode == 4)
				{
					result = 4;
				}
				else if (errorcode == 4006)
				{
					result = 8;
				}
				else
				{
					result = 5;
				}
			}
			else if (secondModule == 6)
			{
				result = 7;
			}
			else
			{
				result = 5;
			}
			return result;
		}
	}
}

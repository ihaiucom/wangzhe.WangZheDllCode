using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResMentorPrivilege : IUnpackable, tsf4g_csharp_interface
	{
		public byte[] szPrivilegeIcon_ByteArray;

		public byte[] szPrivilegeDesc_ByteArray;

		public string szPrivilegeIcon;

		public string szPrivilegeDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szPrivilegeIcon = 128u;

		public static readonly uint LENGTH_szPrivilegeDesc = 128u;

		public ResMentorPrivilege()
		{
			this.szPrivilegeIcon_ByteArray = new byte[1];
			this.szPrivilegeDesc_ByteArray = new byte[1];
			this.szPrivilegeIcon = string.Empty;
			this.szPrivilegeDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szPrivilegeIcon = StringHelper.UTF8BytesToString(ref this.szPrivilegeIcon_ByteArray);
			this.szPrivilegeIcon_ByteArray = null;
			this.szPrivilegeDesc = StringHelper.UTF8BytesToString(ref this.szPrivilegeDesc_ByteArray);
			this.szPrivilegeDesc_ByteArray = null;
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ResMentorPrivilege.CURRVERSION < cutVer)
			{
				cutVer = ResMentorPrivilege.CURRVERSION;
			}
			if (ResMentorPrivilege.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			uint num = 0u;
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szPrivilegeIcon_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResMentorPrivilege.LENGTH_szPrivilegeIcon))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPrivilegeIcon_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPrivilegeIcon_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPrivilegeIcon_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szPrivilegeIcon_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szPrivilegeDesc_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResMentorPrivilege.LENGTH_szPrivilegeDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPrivilegeDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPrivilegeDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPrivilegeDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szPrivilegeDesc_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			srcBuf.disableEndian();
			if (cutVer == 0u || ResMentorPrivilege.CURRVERSION < cutVer)
			{
				cutVer = ResMentorPrivilege.CURRVERSION;
			}
			if (ResMentorPrivilege.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 128;
			if (this.szPrivilegeIcon_ByteArray.GetLength(0) < num)
			{
				this.szPrivilegeIcon_ByteArray = new byte[ResMentorPrivilege.LENGTH_szPrivilegeIcon];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szPrivilegeIcon_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szPrivilegeDesc_ByteArray.GetLength(0) < num2)
			{
				this.szPrivilegeDesc_ByteArray = new byte[ResMentorPrivilege.LENGTH_szPrivilegeDesc];
			}
			errorType = srcBuf.readCString(ref this.szPrivilegeDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

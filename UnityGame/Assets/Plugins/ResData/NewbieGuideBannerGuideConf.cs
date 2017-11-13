using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class NewbieGuideBannerGuideConf : tsf4g_csharp_interface, IUnpackable
	{
		public ushort wID;

		public byte[] szRemark_ByteArray;

		public uint dwGuideBit;

		public byte[] szTitleName_ByteArray;

		public byte[] szBtnName_ByteArray;

		public NewbieGuideBannerPicStr[] astPicPath;

		public string szRemark;

		public string szTitleName;

		public string szBtnName;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szRemark = 60u;

		public static readonly uint LENGTH_szTitleName = 60u;

		public static readonly uint LENGTH_szBtnName = 60u;

		public NewbieGuideBannerGuideConf()
		{
			this.szRemark_ByteArray = new byte[1];
			this.szTitleName_ByteArray = new byte[1];
			this.szBtnName_ByteArray = new byte[1];
			this.astPicPath = new NewbieGuideBannerPicStr[5];
			for (int i = 0; i < 5; i++)
			{
				this.astPicPath[i] = new NewbieGuideBannerPicStr();
			}
			this.szRemark = string.Empty;
			this.szTitleName = string.Empty;
			this.szBtnName = string.Empty;
		}

		private void TransferData()
		{
			this.szRemark = StringHelper.UTF8BytesToString(ref this.szRemark_ByteArray);
			this.szRemark_ByteArray = null;
			this.szTitleName = StringHelper.UTF8BytesToString(ref this.szTitleName_ByteArray);
			this.szTitleName_ByteArray = null;
			this.szBtnName = StringHelper.UTF8BytesToString(ref this.szBtnName_ByteArray);
			this.szBtnName_ByteArray = null;
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
			if (cutVer == 0u || NewbieGuideBannerGuideConf.CURRVERSION < cutVer)
			{
				cutVer = NewbieGuideBannerGuideConf.CURRVERSION;
			}
			if (NewbieGuideBannerGuideConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szRemark_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)NewbieGuideBannerGuideConf.LENGTH_szRemark)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRemark_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRemark_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRemark_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szRemark_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuideBit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num3 > (uint)this.szTitleName_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)NewbieGuideBannerGuideConf.LENGTH_szTitleName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTitleName_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTitleName_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTitleName_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szTitleName_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szBtnName_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)NewbieGuideBannerGuideConf.LENGTH_szBtnName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBtnName_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBtnName_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBtnName_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szBtnName_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astPicPath[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || NewbieGuideBannerGuideConf.CURRVERSION < cutVer)
			{
				cutVer = NewbieGuideBannerGuideConf.CURRVERSION;
			}
			if (NewbieGuideBannerGuideConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 60;
			if (this.szRemark_ByteArray.GetLength(0) < num)
			{
				this.szRemark_ByteArray = new byte[NewbieGuideBannerGuideConf.LENGTH_szRemark];
			}
			errorType = srcBuf.readCString(ref this.szRemark_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuideBit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 60;
			if (this.szTitleName_ByteArray.GetLength(0) < num2)
			{
				this.szTitleName_ByteArray = new byte[NewbieGuideBannerGuideConf.LENGTH_szTitleName];
			}
			errorType = srcBuf.readCString(ref this.szTitleName_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 60;
			if (this.szBtnName_ByteArray.GetLength(0) < num3)
			{
				this.szBtnName_ByteArray = new byte[NewbieGuideBannerGuideConf.LENGTH_szBtnName];
			}
			errorType = srcBuf.readCString(ref this.szBtnName_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astPicPath[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.TransferData();
			return errorType;
		}
	}
}

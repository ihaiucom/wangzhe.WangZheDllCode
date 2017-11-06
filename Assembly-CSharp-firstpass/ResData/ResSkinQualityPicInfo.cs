using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSkinQualityPicInfo : tsf4g_csharp_interface, IUnpackable
	{
		public byte bQualityId;

		public byte[] szQualityPicPath_ByteArray;

		public byte[] szLabelPicPath_ByteArray;

		public string szQualityPicPath;

		public string szLabelPicPath;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szQualityPicPath = 128u;

		public static readonly uint LENGTH_szLabelPicPath = 128u;

		public ResSkinQualityPicInfo()
		{
			this.szQualityPicPath_ByteArray = new byte[1];
			this.szLabelPicPath_ByteArray = new byte[1];
			this.szQualityPicPath = string.Empty;
			this.szLabelPicPath = string.Empty;
		}

		private void TransferData()
		{
			this.szQualityPicPath = StringHelper.UTF8BytesToString(ref this.szQualityPicPath_ByteArray);
			this.szQualityPicPath_ByteArray = null;
			this.szLabelPicPath = StringHelper.UTF8BytesToString(ref this.szLabelPicPath_ByteArray);
			this.szLabelPicPath_ByteArray = null;
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
			if (cutVer == 0u || ResSkinQualityPicInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkinQualityPicInfo.CURRVERSION;
			}
			if (ResSkinQualityPicInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bQualityId);
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
			if (num > (uint)this.szQualityPicPath_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResSkinQualityPicInfo.LENGTH_szQualityPicPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szQualityPicPath_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szQualityPicPath_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szQualityPicPath_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szQualityPicPath_ByteArray) + 1;
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
			if (num3 > (uint)this.szLabelPicPath_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResSkinQualityPicInfo.LENGTH_szLabelPicPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLabelPicPath_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLabelPicPath_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLabelPicPath_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szLabelPicPath_ByteArray) + 1;
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
			if (cutVer == 0u || ResSkinQualityPicInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkinQualityPicInfo.CURRVERSION;
			}
			if (ResSkinQualityPicInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bQualityId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szQualityPicPath_ByteArray.GetLength(0) < num)
			{
				this.szQualityPicPath_ByteArray = new byte[ResSkinQualityPicInfo.LENGTH_szQualityPicPath];
			}
			errorType = srcBuf.readCString(ref this.szQualityPicPath_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szLabelPicPath_ByteArray.GetLength(0) < num2)
			{
				this.szLabelPicPath_ByteArray = new byte[ResSkinQualityPicInfo.LENGTH_szLabelPicPath];
			}
			errorType = srcBuf.readCString(ref this.szLabelPicPath_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

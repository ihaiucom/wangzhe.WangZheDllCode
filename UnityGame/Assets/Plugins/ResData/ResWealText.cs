using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResWealText : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public ResDT_WealCommon stCommon;

		public byte[] szJumpBtnText_ByteArray;

		public uint dwJumpEntrance;

		public byte[] szJumpBtnToUrl_ByteArray;

		public string szJumpBtnText;

		public string szJumpBtnToUrl;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szJumpBtnText = 20u;

		public static readonly uint LENGTH_szJumpBtnToUrl = 128u;

		public ResWealText()
		{
			this.stCommon = new ResDT_WealCommon();
			this.szJumpBtnText_ByteArray = new byte[1];
			this.szJumpBtnToUrl_ByteArray = new byte[1];
			this.szJumpBtnText = string.Empty;
			this.szJumpBtnToUrl = string.Empty;
		}

		private void TransferData()
		{
			this.szJumpBtnText = StringHelper.UTF8BytesToString(ref this.szJumpBtnText_ByteArray);
			this.szJumpBtnText_ByteArray = null;
			this.szJumpBtnToUrl = StringHelper.UTF8BytesToString(ref this.szJumpBtnToUrl_ByteArray);
			this.szJumpBtnToUrl_ByteArray = null;
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
			if (cutVer == 0u || ResWealText.CURRVERSION < cutVer)
			{
				cutVer = ResWealText.CURRVERSION;
			}
			if (ResWealText.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommon.unpack(ref srcBuf, cutVer);
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
			if (num > (uint)this.szJumpBtnText_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResWealText.LENGTH_szJumpBtnText)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szJumpBtnText_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szJumpBtnText_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szJumpBtnText_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szJumpBtnText_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwJumpEntrance);
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
			if (num3 > (uint)this.szJumpBtnToUrl_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResWealText.LENGTH_szJumpBtnToUrl)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szJumpBtnToUrl_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szJumpBtnToUrl_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szJumpBtnToUrl_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szJumpBtnToUrl_ByteArray) + 1;
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
			if (cutVer == 0u || ResWealText.CURRVERSION < cutVer)
			{
				cutVer = ResWealText.CURRVERSION;
			}
			if (ResWealText.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommon.load(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 20;
			if (this.szJumpBtnText_ByteArray.GetLength(0) < num)
			{
				this.szJumpBtnText_ByteArray = new byte[ResWealText.LENGTH_szJumpBtnText];
			}
			errorType = srcBuf.readCString(ref this.szJumpBtnText_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwJumpEntrance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szJumpBtnToUrl_ByteArray.GetLength(0) < num2)
			{
				this.szJumpBtnToUrl_ByteArray = new byte[ResWealText.LENGTH_szJumpBtnToUrl];
			}
			errorType = srcBuf.readCString(ref this.szJumpBtnToUrl_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResGuideTipInfo : tsf4g_csharp_interface, IUnpackable
	{
		public int iCfgID;

		public byte[] szImagePath_ByteArray;

		public byte[] szTipTitle_ByteArray;

		public byte[] szTipContent_ByteArray;

		public byte bTipPos;

		public byte[] szTipVoice_ByteArray;

		public string szImagePath;

		public string szTipTitle;

		public string szTipContent;

		public string szTipVoice;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szImagePath = 128u;

		public static readonly uint LENGTH_szTipTitle = 32u;

		public static readonly uint LENGTH_szTipContent = 1024u;

		public static readonly uint LENGTH_szTipVoice = 128u;

		public ResGuideTipInfo()
		{
			this.szImagePath_ByteArray = new byte[1];
			this.szTipTitle_ByteArray = new byte[1];
			this.szTipContent_ByteArray = new byte[1];
			this.szTipVoice_ByteArray = new byte[1];
			this.szImagePath = string.Empty;
			this.szTipTitle = string.Empty;
			this.szTipContent = string.Empty;
			this.szTipVoice = string.Empty;
		}

		private void TransferData()
		{
			this.szImagePath = StringHelper.UTF8BytesToString(ref this.szImagePath_ByteArray);
			this.szImagePath_ByteArray = null;
			this.szTipTitle = StringHelper.UTF8BytesToString(ref this.szTipTitle_ByteArray);
			this.szTipTitle_ByteArray = null;
			this.szTipContent = StringHelper.UTF8BytesToString(ref this.szTipContent_ByteArray);
			this.szTipContent_ByteArray = null;
			this.szTipVoice = StringHelper.UTF8BytesToString(ref this.szTipVoice_ByteArray);
			this.szTipVoice_ByteArray = null;
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
			if (cutVer == 0u || ResGuideTipInfo.CURRVERSION < cutVer)
			{
				cutVer = ResGuideTipInfo.CURRVERSION;
			}
			if (ResGuideTipInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
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
			if (num > (uint)this.szImagePath_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResGuideTipInfo.LENGTH_szImagePath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szImagePath_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szImagePath_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szImagePath_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szImagePath_ByteArray) + 1;
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
			if (num3 > (uint)this.szTipTitle_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResGuideTipInfo.LENGTH_szTipTitle)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTipTitle_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTipTitle_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTipTitle_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szTipTitle_ByteArray) + 1;
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
			if (num5 > (uint)this.szTipContent_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResGuideTipInfo.LENGTH_szTipContent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTipContent_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTipContent_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTipContent_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szTipContent_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bTipPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szTipVoice_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResGuideTipInfo.LENGTH_szTipVoice)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTipVoice_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTipVoice_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTipVoice_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szTipVoice_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
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
			if (cutVer == 0u || ResGuideTipInfo.CURRVERSION < cutVer)
			{
				cutVer = ResGuideTipInfo.CURRVERSION;
			}
			if (ResGuideTipInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szImagePath_ByteArray.GetLength(0) < num)
			{
				this.szImagePath_ByteArray = new byte[ResGuideTipInfo.LENGTH_szImagePath];
			}
			errorType = srcBuf.readCString(ref this.szImagePath_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szTipTitle_ByteArray.GetLength(0) < num2)
			{
				this.szTipTitle_ByteArray = new byte[ResGuideTipInfo.LENGTH_szTipTitle];
			}
			errorType = srcBuf.readCString(ref this.szTipTitle_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 1024;
			if (this.szTipContent_ByteArray.GetLength(0) < num3)
			{
				this.szTipContent_ByteArray = new byte[ResGuideTipInfo.LENGTH_szTipContent];
			}
			errorType = srcBuf.readCString(ref this.szTipContent_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTipPos);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szTipVoice_ByteArray.GetLength(0) < num4)
			{
				this.szTipVoice_ByteArray = new byte[ResGuideTipInfo.LENGTH_szTipVoice];
			}
			errorType = srcBuf.readCString(ref this.szTipVoice_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

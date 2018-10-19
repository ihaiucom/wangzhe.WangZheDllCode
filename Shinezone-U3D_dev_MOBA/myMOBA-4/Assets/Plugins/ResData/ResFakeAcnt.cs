using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResFakeAcnt : IUnpackable, tsf4g_csharp_interface
	{
		public byte[] szAcntName_ByteArray;

		public byte[] szAcntHeadUrl_ByteArray;

		public string szAcntName;

		public string szAcntHeadUrl;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szAcntName = 64u;

		public static readonly uint LENGTH_szAcntHeadUrl = 256u;

		public ResFakeAcnt()
		{
			this.szAcntName_ByteArray = new byte[1];
			this.szAcntHeadUrl_ByteArray = new byte[1];
			this.szAcntName = string.Empty;
			this.szAcntHeadUrl = string.Empty;
		}

		private void TransferData()
		{
			this.szAcntName = StringHelper.UTF8BytesToString(ref this.szAcntName_ByteArray);
			this.szAcntName_ByteArray = null;
			this.szAcntHeadUrl = StringHelper.UTF8BytesToString(ref this.szAcntHeadUrl_ByteArray);
			this.szAcntHeadUrl_ByteArray = null;
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
			if (cutVer == 0u || ResFakeAcnt.CURRVERSION < cutVer)
			{
				cutVer = ResFakeAcnt.CURRVERSION;
			}
			if (ResFakeAcnt.BASEVERSION > cutVer)
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
			if (num > (uint)this.szAcntName_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResFakeAcnt.LENGTH_szAcntName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAcntName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAcntName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAcntName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szAcntName_ByteArray) + 1;
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
			if (num3 > (uint)this.szAcntHeadUrl_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResFakeAcnt.LENGTH_szAcntHeadUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAcntHeadUrl_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAcntHeadUrl_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAcntHeadUrl_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szAcntHeadUrl_ByteArray) + 1;
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
			if (cutVer == 0u || ResFakeAcnt.CURRVERSION < cutVer)
			{
				cutVer = ResFakeAcnt.CURRVERSION;
			}
			if (ResFakeAcnt.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 64;
			if (this.szAcntName_ByteArray.GetLength(0) < num)
			{
				this.szAcntName_ByteArray = new byte[ResFakeAcnt.LENGTH_szAcntName];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szAcntName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 256;
			if (this.szAcntHeadUrl_ByteArray.GetLength(0) < num2)
			{
				this.szAcntHeadUrl_ByteArray = new byte[ResFakeAcnt.LENGTH_szAcntHeadUrl];
			}
			errorType = srcBuf.readCString(ref this.szAcntHeadUrl_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

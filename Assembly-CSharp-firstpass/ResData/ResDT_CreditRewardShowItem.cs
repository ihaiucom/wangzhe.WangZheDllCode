using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_CreditRewardShowItem : tsf4g_csharp_interface, IUnpackable
	{
		public byte[] szCreditRewardItemIcon_ByteArray;

		public byte[] szCreditRewardItemDesc_ByteArray;

		public string szCreditRewardItemIcon;

		public string szCreditRewardItemDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szCreditRewardItemIcon = 32u;

		public static readonly uint LENGTH_szCreditRewardItemDesc = 64u;

		public ResDT_CreditRewardShowItem()
		{
			this.szCreditRewardItemIcon_ByteArray = new byte[1];
			this.szCreditRewardItemDesc_ByteArray = new byte[1];
			this.szCreditRewardItemIcon = string.Empty;
			this.szCreditRewardItemDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szCreditRewardItemIcon = StringHelper.UTF8BytesToString(ref this.szCreditRewardItemIcon_ByteArray);
			this.szCreditRewardItemIcon_ByteArray = null;
			this.szCreditRewardItemDesc = StringHelper.UTF8BytesToString(ref this.szCreditRewardItemDesc_ByteArray);
			this.szCreditRewardItemDesc_ByteArray = null;
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
			if (cutVer == 0u || ResDT_CreditRewardShowItem.CURRVERSION < cutVer)
			{
				cutVer = ResDT_CreditRewardShowItem.CURRVERSION;
			}
			if (ResDT_CreditRewardShowItem.BASEVERSION > cutVer)
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
			if (num > (uint)this.szCreditRewardItemIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResDT_CreditRewardShowItem.LENGTH_szCreditRewardItemIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCreditRewardItemIcon_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCreditRewardItemIcon_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCreditRewardItemIcon_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szCreditRewardItemIcon_ByteArray) + 1;
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
			if (num3 > (uint)this.szCreditRewardItemDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResDT_CreditRewardShowItem.LENGTH_szCreditRewardItemDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCreditRewardItemDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCreditRewardItemDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCreditRewardItemDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szCreditRewardItemDesc_ByteArray) + 1;
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
			if (cutVer == 0u || ResDT_CreditRewardShowItem.CURRVERSION < cutVer)
			{
				cutVer = ResDT_CreditRewardShowItem.CURRVERSION;
			}
			if (ResDT_CreditRewardShowItem.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 32;
			if (this.szCreditRewardItemIcon_ByteArray.GetLength(0) < num)
			{
				this.szCreditRewardItemIcon_ByteArray = new byte[ResDT_CreditRewardShowItem.LENGTH_szCreditRewardItemIcon];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szCreditRewardItemIcon_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 64;
			if (this.szCreditRewardItemDesc_ByteArray.GetLength(0) < num2)
			{
				this.szCreditRewardItemDesc_ByteArray = new byte[ResDT_CreditRewardShowItem.LENGTH_szCreditRewardItemDesc];
			}
			errorType = srcBuf.readCString(ref this.szCreditRewardItemDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

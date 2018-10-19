using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResCreditLevelInfo : IUnpackable, tsf4g_csharp_interface
	{
		public uint dwID;

		public uint dwCreditThresholdLow;

		public uint dwCreditThresholdHigh;

		public byte bCreditLevel;

		public byte bCreditDayRewardSwitch;

		public byte[] szCreditDayRewardDesc_ByteArray;

		public uint dwCreditDayRewardID;

		public byte bCreditWeekRewardSwitch;

		public byte[] szCreditWeekRewardDesc_ByteArray;

		public uint dwCreditWeekRewardID;

		public int iSettlePvpExpTTH;

		public int iSettlePvpCoinTTH;

		public byte bCreditLevelResult;

		public byte[] szCreditLevelTxt_ByteArray;

		public ResDT_CreditRewardShowItem[] astCreditRewardDetail;

		public string szCreditDayRewardDesc;

		public string szCreditWeekRewardDesc;

		public string szCreditLevelTxt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szCreditDayRewardDesc = 256u;

		public static readonly uint LENGTH_szCreditWeekRewardDesc = 256u;

		public static readonly uint LENGTH_szCreditLevelTxt = 16u;

		public ResCreditLevelInfo()
		{
			this.szCreditDayRewardDesc_ByteArray = new byte[1];
			this.szCreditWeekRewardDesc_ByteArray = new byte[1];
			this.szCreditLevelTxt_ByteArray = new byte[1];
			this.astCreditRewardDetail = new ResDT_CreditRewardShowItem[3];
			for (int i = 0; i < 3; i++)
			{
				this.astCreditRewardDetail[i] = new ResDT_CreditRewardShowItem();
			}
			this.szCreditDayRewardDesc = string.Empty;
			this.szCreditWeekRewardDesc = string.Empty;
			this.szCreditLevelTxt = string.Empty;
		}

		private void TransferData()
		{
			this.szCreditDayRewardDesc = StringHelper.UTF8BytesToString(ref this.szCreditDayRewardDesc_ByteArray);
			this.szCreditDayRewardDesc_ByteArray = null;
			this.szCreditWeekRewardDesc = StringHelper.UTF8BytesToString(ref this.szCreditWeekRewardDesc_ByteArray);
			this.szCreditWeekRewardDesc_ByteArray = null;
			this.szCreditLevelTxt = StringHelper.UTF8BytesToString(ref this.szCreditLevelTxt_ByteArray);
			this.szCreditLevelTxt_ByteArray = null;
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
			if (cutVer == 0u || ResCreditLevelInfo.CURRVERSION < cutVer)
			{
				cutVer = ResCreditLevelInfo.CURRVERSION;
			}
			if (ResCreditLevelInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditThresholdLow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditThresholdHigh);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditDayRewardSwitch);
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
			if (num > (uint)this.szCreditDayRewardDesc_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResCreditLevelInfo.LENGTH_szCreditDayRewardDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCreditDayRewardDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCreditDayRewardDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCreditDayRewardDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szCreditDayRewardDesc_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditDayRewardID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditWeekRewardSwitch);
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
			if (num3 > (uint)this.szCreditWeekRewardDesc_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResCreditLevelInfo.LENGTH_szCreditWeekRewardDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCreditWeekRewardDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCreditWeekRewardDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCreditWeekRewardDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szCreditWeekRewardDesc_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditWeekRewardID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSettlePvpExpTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSettlePvpCoinTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditLevelResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num5 > (uint)this.szCreditLevelTxt_ByteArray.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)ResCreditLevelInfo.LENGTH_szCreditLevelTxt))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCreditLevelTxt_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCreditLevelTxt_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCreditLevelTxt_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szCreditLevelTxt_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astCreditRewardDetail[i].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResCreditLevelInfo.CURRVERSION < cutVer)
			{
				cutVer = ResCreditLevelInfo.CURRVERSION;
			}
			if (ResCreditLevelInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditThresholdLow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditThresholdHigh);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditDayRewardSwitch);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 256;
			if (this.szCreditDayRewardDesc_ByteArray.GetLength(0) < num)
			{
				this.szCreditDayRewardDesc_ByteArray = new byte[ResCreditLevelInfo.LENGTH_szCreditDayRewardDesc];
			}
			errorType = srcBuf.readCString(ref this.szCreditDayRewardDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditDayRewardID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditWeekRewardSwitch);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 256;
			if (this.szCreditWeekRewardDesc_ByteArray.GetLength(0) < num2)
			{
				this.szCreditWeekRewardDesc_ByteArray = new byte[ResCreditLevelInfo.LENGTH_szCreditWeekRewardDesc];
			}
			errorType = srcBuf.readCString(ref this.szCreditWeekRewardDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditWeekRewardID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSettlePvpExpTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSettlePvpCoinTTH);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCreditLevelResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 16;
			if (this.szCreditLevelTxt_ByteArray.GetLength(0) < num3)
			{
				this.szCreditLevelTxt_ByteArray = new byte[ResCreditLevelInfo.LENGTH_szCreditLevelTxt];
			}
			errorType = srcBuf.readCString(ref this.szCreditLevelTxt_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astCreditRewardDetail[i].load(ref srcBuf, cutVer);
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

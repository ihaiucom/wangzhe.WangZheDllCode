using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResHeadImage : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public byte[] szHeadDesc_ByteArray;

		public byte[] szHeadAccess_ByteArray;

		public byte[] szHeadIcon_ByteArray;

		public byte bHeadType;

		public byte[] szShowStartTime_ByteArray;

		public byte[] szShowEndTime_ByteArray;

		public uint dwShowStartGen;

		public uint dwShowEndGen;

		public uint dwValidSecond;

		public uint dwCouponsBuy;

		public uint dwPVPCoinBuy;

		public uint dwGuildCoinBuy;

		public uint dwDiamondBuy;

		public byte bIsBuyMixPay;

		public byte bSortWeight;

		public byte[] szEffect_ByteArray;

		public string szHeadDesc;

		public string szHeadAccess;

		public string szHeadIcon;

		public string szShowStartTime;

		public string szShowEndTime;

		public string szEffect;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szHeadDesc = 512u;

		public static readonly uint LENGTH_szHeadAccess = 512u;

		public static readonly uint LENGTH_szHeadIcon = 128u;

		public static readonly uint LENGTH_szShowStartTime = 16u;

		public static readonly uint LENGTH_szShowEndTime = 16u;

		public static readonly uint LENGTH_szEffect = 128u;

		public ResHeadImage()
		{
			this.szHeadDesc_ByteArray = new byte[1];
			this.szHeadAccess_ByteArray = new byte[1];
			this.szHeadIcon_ByteArray = new byte[1];
			this.szShowStartTime_ByteArray = new byte[1];
			this.szShowEndTime_ByteArray = new byte[1];
			this.szEffect_ByteArray = new byte[1];
			this.szHeadDesc = string.Empty;
			this.szHeadAccess = string.Empty;
			this.szHeadIcon = string.Empty;
			this.szShowStartTime = string.Empty;
			this.szShowEndTime = string.Empty;
			this.szEffect = string.Empty;
		}

		private void TransferData()
		{
			this.szHeadDesc = StringHelper.UTF8BytesToString(ref this.szHeadDesc_ByteArray);
			this.szHeadDesc_ByteArray = null;
			this.szHeadAccess = StringHelper.UTF8BytesToString(ref this.szHeadAccess_ByteArray);
			this.szHeadAccess_ByteArray = null;
			this.szHeadIcon = StringHelper.UTF8BytesToString(ref this.szHeadIcon_ByteArray);
			this.szHeadIcon_ByteArray = null;
			this.szShowStartTime = StringHelper.UTF8BytesToString(ref this.szShowStartTime_ByteArray);
			this.szShowStartTime_ByteArray = null;
			this.szShowEndTime = StringHelper.UTF8BytesToString(ref this.szShowEndTime_ByteArray);
			this.szShowEndTime_ByteArray = null;
			this.szEffect = StringHelper.UTF8BytesToString(ref this.szEffect_ByteArray);
			this.szEffect_ByteArray = null;
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
			if (cutVer == 0u || ResHeadImage.CURRVERSION < cutVer)
			{
				cutVer = ResHeadImage.CURRVERSION;
			}
			if (ResHeadImage.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
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
			if (num > (uint)this.szHeadDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResHeadImage.LENGTH_szHeadDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szHeadDesc_ByteArray) + 1;
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
			if (num3 > (uint)this.szHeadAccess_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResHeadImage.LENGTH_szHeadAccess)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadAccess_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadAccess_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadAccess_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szHeadAccess_ByteArray) + 1;
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
			if (num5 > (uint)this.szHeadIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResHeadImage.LENGTH_szHeadIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadIcon_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadIcon_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadIcon_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szHeadIcon_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bHeadType);
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
			if (num7 > (uint)this.szShowStartTime_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResHeadImage.LENGTH_szShowStartTime)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szShowStartTime_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szShowStartTime_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szShowStartTime_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szShowStartTime_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num9 = 0u;
			errorType = srcBuf.readUInt32(ref num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num9 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num9 > (uint)this.szShowEndTime_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResHeadImage.LENGTH_szShowEndTime)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szShowEndTime_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szShowEndTime_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szShowEndTime_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szShowEndTime_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowStartGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowEndGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwValidSecond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCouponsBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPVPCoinBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuildCoinBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDiamondBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyMixPay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSortWeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num11 = 0u;
			errorType = srcBuf.readUInt32(ref num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num11 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num11 > (uint)this.szEffect_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResHeadImage.LENGTH_szEffect)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szEffect_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szEffect_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szEffect_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szEffect_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
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
			if (cutVer == 0u || ResHeadImage.CURRVERSION < cutVer)
			{
				cutVer = ResHeadImage.CURRVERSION;
			}
			if (ResHeadImage.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 512;
			if (this.szHeadDesc_ByteArray.GetLength(0) < num)
			{
				this.szHeadDesc_ByteArray = new byte[ResHeadImage.LENGTH_szHeadDesc];
			}
			errorType = srcBuf.readCString(ref this.szHeadDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 512;
			if (this.szHeadAccess_ByteArray.GetLength(0) < num2)
			{
				this.szHeadAccess_ByteArray = new byte[ResHeadImage.LENGTH_szHeadAccess];
			}
			errorType = srcBuf.readCString(ref this.szHeadAccess_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szHeadIcon_ByteArray.GetLength(0) < num3)
			{
				this.szHeadIcon_ByteArray = new byte[ResHeadImage.LENGTH_szHeadIcon];
			}
			errorType = srcBuf.readCString(ref this.szHeadIcon_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHeadType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 16;
			if (this.szShowStartTime_ByteArray.GetLength(0) < num4)
			{
				this.szShowStartTime_ByteArray = new byte[ResHeadImage.LENGTH_szShowStartTime];
			}
			errorType = srcBuf.readCString(ref this.szShowStartTime_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 16;
			if (this.szShowEndTime_ByteArray.GetLength(0) < num5)
			{
				this.szShowEndTime_ByteArray = new byte[ResHeadImage.LENGTH_szShowEndTime];
			}
			errorType = srcBuf.readCString(ref this.szShowEndTime_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowStartGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowEndGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwValidSecond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCouponsBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPVPCoinBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuildCoinBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDiamondBuy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyMixPay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSortWeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szEffect_ByteArray.GetLength(0) < num6)
			{
				this.szEffect_ByteArray = new byte[ResHeadImage.LENGTH_szEffect];
			}
			errorType = srcBuf.readCString(ref this.szEffect_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

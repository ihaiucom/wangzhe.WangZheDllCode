using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResHeroSkinShop : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public uint dwHeroID;

		public byte[] szHeroName_ByteArray;

		public uint dwSkinID;

		public byte[] szSkinName_ByteArray;

		public byte[] szSkinDesc_ByteArray;

		public byte bIsLimitSkin;

		public byte[] szLimitQualityPic_ByteArray;

		public byte[] szLimitLabelPic_ByteArray;

		public byte bSkinQuality;

		public byte bGetPathType;

		public byte[] szGetPath_ByteArray;

		public byte bIsBuyCoupons;

		public uint dwBuyCoupons;

		public byte bIsBuySkinCoin;

		public uint dwBuySkinCoin;

		public byte bIsBuyDiamond;

		public uint dwBuyDiamond;

		public byte bIsBuyMixPay;

		public byte bIsBuyItem;

		public uint dwBuyItemCnt;

		public byte bIsPresent;

		public byte bIsAskfor;

		public uint dwSortId;

		public byte bPromotionCnt;

		public uint[] PromotionID;

		public uint dwChgItemCnt;

		public byte[] szOnTimeStr_ByteArray;

		public byte[] szOffTimeStr_ByteArray;

		public uint dwOnTimeGen;

		public uint dwOffTimeGen;

		public uint dwReleaseId;

		public byte bShowInShop;

		public byte bShowInMgr;

		public byte bRankLimitType;

		public byte bRankLimitGrade;

		public ulong ullRankLimitParam;

		public ResDT_RegisterSale_Info stRegisterSale;

		public uint dwFactoryID;

		public string szHeroName;

		public string szSkinName;

		public string szSkinDesc;

		public string szLimitQualityPic;

		public string szLimitLabelPic;

		public string szGetPath;

		public string szOnTimeStr;

		public string szOffTimeStr;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szHeroName = 32u;

		public static readonly uint LENGTH_szSkinName = 32u;

		public static readonly uint LENGTH_szSkinDesc = 256u;

		public static readonly uint LENGTH_szLimitQualityPic = 128u;

		public static readonly uint LENGTH_szLimitLabelPic = 128u;

		public static readonly uint LENGTH_szGetPath = 256u;

		public static readonly uint LENGTH_szOnTimeStr = 16u;

		public static readonly uint LENGTH_szOffTimeStr = 16u;

		public ResHeroSkinShop()
		{
			this.szHeroName_ByteArray = new byte[1];
			this.szSkinName_ByteArray = new byte[1];
			this.szSkinDesc_ByteArray = new byte[1];
			this.szLimitQualityPic_ByteArray = new byte[1];
			this.szLimitLabelPic_ByteArray = new byte[1];
			this.szGetPath_ByteArray = new byte[1];
			this.PromotionID = new uint[5];
			this.szOnTimeStr_ByteArray = new byte[1];
			this.szOffTimeStr_ByteArray = new byte[1];
			this.stRegisterSale = new ResDT_RegisterSale_Info();
			this.szHeroName = string.Empty;
			this.szSkinName = string.Empty;
			this.szSkinDesc = string.Empty;
			this.szLimitQualityPic = string.Empty;
			this.szLimitLabelPic = string.Empty;
			this.szGetPath = string.Empty;
			this.szOnTimeStr = string.Empty;
			this.szOffTimeStr = string.Empty;
		}

		private void TransferData()
		{
			this.szHeroName = StringHelper.UTF8BytesToString(ref this.szHeroName_ByteArray);
			this.szHeroName_ByteArray = null;
			this.szSkinName = StringHelper.UTF8BytesToString(ref this.szSkinName_ByteArray);
			this.szSkinName_ByteArray = null;
			this.szSkinDesc = StringHelper.UTF8BytesToString(ref this.szSkinDesc_ByteArray);
			this.szSkinDesc_ByteArray = null;
			this.szLimitQualityPic = StringHelper.UTF8BytesToString(ref this.szLimitQualityPic_ByteArray);
			this.szLimitQualityPic_ByteArray = null;
			this.szLimitLabelPic = StringHelper.UTF8BytesToString(ref this.szLimitLabelPic_ByteArray);
			this.szLimitLabelPic_ByteArray = null;
			this.szGetPath = StringHelper.UTF8BytesToString(ref this.szGetPath_ByteArray);
			this.szGetPath_ByteArray = null;
			this.szOnTimeStr = StringHelper.UTF8BytesToString(ref this.szOnTimeStr_ByteArray);
			this.szOnTimeStr_ByteArray = null;
			this.szOffTimeStr = StringHelper.UTF8BytesToString(ref this.szOffTimeStr_ByteArray);
			this.szOffTimeStr_ByteArray = null;
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
			if (cutVer == 0u || ResHeroSkinShop.CURRVERSION < cutVer)
			{
				cutVer = ResHeroSkinShop.CURRVERSION;
			}
			if (ResHeroSkinShop.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroID);
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
			if (num > (uint)this.szHeroName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResHeroSkinShop.LENGTH_szHeroName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szHeroName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkinID);
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
			if (num3 > (uint)this.szSkinName_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResHeroSkinShop.LENGTH_szSkinName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkinName_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkinName_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkinName_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSkinName_ByteArray) + 1;
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
			if (num5 > (uint)this.szSkinDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResHeroSkinShop.LENGTH_szSkinDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkinDesc_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkinDesc_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkinDesc_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szSkinDesc_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsLimitSkin);
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
			if (num7 > (uint)this.szLimitQualityPic_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResHeroSkinShop.LENGTH_szLimitQualityPic)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLimitQualityPic_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLimitQualityPic_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLimitQualityPic_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szLimitQualityPic_ByteArray) + 1;
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
			if (num9 > (uint)this.szLimitLabelPic_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResHeroSkinShop.LENGTH_szLimitLabelPic)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLimitLabelPic_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLimitLabelPic_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLimitLabelPic_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szLimitLabelPic_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bSkinQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGetPathType);
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
			if (num11 > (uint)this.szGetPath_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResHeroSkinShop.LENGTH_szGetPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGetPath_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGetPath_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGetPath_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szGetPath_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyCoupons);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyCoupons);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuySkinCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuySkinCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyDiamond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyDiamond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyMixPay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyItem);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsPresent);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAskfor);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSortId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPromotionCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = srcBuf.readUInt32(ref this.PromotionID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwChgItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num13 = 0u;
			errorType = srcBuf.readUInt32(ref num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num13 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num13 > (uint)this.szOnTimeStr_ByteArray.GetLength(0))
			{
				if ((ulong)num13 > (ulong)ResHeroSkinShop.LENGTH_szOnTimeStr)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szOnTimeStr_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szOnTimeStr_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szOnTimeStr_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szOnTimeStr_ByteArray) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num15 = 0u;
			errorType = srcBuf.readUInt32(ref num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num15 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num15 > (uint)this.szOffTimeStr_ByteArray.GetLength(0))
			{
				if ((ulong)num15 > (ulong)ResHeroSkinShop.LENGTH_szOffTimeStr)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szOffTimeStr_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szOffTimeStr_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szOffTimeStr_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szOffTimeStr_ByteArray) + 1;
			if ((ulong)num15 != (ulong)((long)num16))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwOnTimeGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOffTimeGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReleaseId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowInShop);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowInMgr);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRankLimitType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRankLimitGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullRankLimitParam);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRegisterSale.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFactoryID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || ResHeroSkinShop.CURRVERSION < cutVer)
			{
				cutVer = ResHeroSkinShop.CURRVERSION;
			}
			if (ResHeroSkinShop.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 32;
			if (this.szHeroName_ByteArray.GetLength(0) < num)
			{
				this.szHeroName_ByteArray = new byte[ResHeroSkinShop.LENGTH_szHeroName];
			}
			errorType = srcBuf.readCString(ref this.szHeroName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szSkinName_ByteArray.GetLength(0) < num2)
			{
				this.szSkinName_ByteArray = new byte[ResHeroSkinShop.LENGTH_szSkinName];
			}
			errorType = srcBuf.readCString(ref this.szSkinName_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 256;
			if (this.szSkinDesc_ByteArray.GetLength(0) < num3)
			{
				this.szSkinDesc_ByteArray = new byte[ResHeroSkinShop.LENGTH_szSkinDesc];
			}
			errorType = srcBuf.readCString(ref this.szSkinDesc_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsLimitSkin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szLimitQualityPic_ByteArray.GetLength(0) < num4)
			{
				this.szLimitQualityPic_ByteArray = new byte[ResHeroSkinShop.LENGTH_szLimitQualityPic];
			}
			errorType = srcBuf.readCString(ref this.szLimitQualityPic_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szLimitLabelPic_ByteArray.GetLength(0) < num5)
			{
				this.szLimitLabelPic_ByteArray = new byte[ResHeroSkinShop.LENGTH_szLimitLabelPic];
			}
			errorType = srcBuf.readCString(ref this.szLimitLabelPic_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkinQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGetPathType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 256;
			if (this.szGetPath_ByteArray.GetLength(0) < num6)
			{
				this.szGetPath_ByteArray = new byte[ResHeroSkinShop.LENGTH_szGetPath];
			}
			errorType = srcBuf.readCString(ref this.szGetPath_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyCoupons);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyCoupons);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuySkinCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuySkinCoin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyDiamond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyDiamond);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyMixPay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBuyItem);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsPresent);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAskfor);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSortId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPromotionCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = srcBuf.readUInt32(ref this.PromotionID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwChgItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 16;
			if (this.szOnTimeStr_ByteArray.GetLength(0) < num7)
			{
				this.szOnTimeStr_ByteArray = new byte[ResHeroSkinShop.LENGTH_szOnTimeStr];
			}
			errorType = srcBuf.readCString(ref this.szOnTimeStr_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 16;
			if (this.szOffTimeStr_ByteArray.GetLength(0) < num8)
			{
				this.szOffTimeStr_ByteArray = new byte[ResHeroSkinShop.LENGTH_szOffTimeStr];
			}
			errorType = srcBuf.readCString(ref this.szOffTimeStr_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOnTimeGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOffTimeGen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReleaseId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowInShop);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowInMgr);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRankLimitType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRankLimitGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullRankLimitParam);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRegisterSale.load(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFactoryID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

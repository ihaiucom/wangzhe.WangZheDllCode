using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRecommendEquipInBattle : IUnpackable, tsf4g_csharp_interface
	{
		public ushort wHeroID;

		public ushort wCombinationID;

		public byte[] szCombinationName_ByteArray;

		public byte[] szCombinationDesc_ByteArray;

		public ushort[] RecommendEquipID;

		public string szCombinationName;

		public string szCombinationDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szCombinationName = 128u;

		public static readonly uint LENGTH_szCombinationDesc = 128u;

		public ResRecommendEquipInBattle()
		{
			this.szCombinationName_ByteArray = new byte[1];
			this.szCombinationDesc_ByteArray = new byte[1];
			this.RecommendEquipID = new ushort[6];
			this.szCombinationName = string.Empty;
			this.szCombinationDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szCombinationName = StringHelper.UTF8BytesToString(ref this.szCombinationName_ByteArray);
			this.szCombinationName_ByteArray = null;
			this.szCombinationDesc = StringHelper.UTF8BytesToString(ref this.szCombinationDesc_ByteArray);
			this.szCombinationDesc_ByteArray = null;
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
			if (cutVer == 0u || ResRecommendEquipInBattle.CURRVERSION < cutVer)
			{
				cutVer = ResRecommendEquipInBattle.CURRVERSION;
			}
			if (ResRecommendEquipInBattle.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wCombinationID);
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
			if (num > (uint)this.szCombinationName_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResRecommendEquipInBattle.LENGTH_szCombinationName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCombinationName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCombinationName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCombinationName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szCombinationName_ByteArray) + 1;
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
			if (num3 > (uint)this.szCombinationDesc_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResRecommendEquipInBattle.LENGTH_szCombinationDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCombinationDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCombinationDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCombinationDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szCombinationDesc_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = srcBuf.readUInt16(ref this.RecommendEquipID[i]);
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
			if (cutVer == 0u || ResRecommendEquipInBattle.CURRVERSION < cutVer)
			{
				cutVer = ResRecommendEquipInBattle.CURRVERSION;
			}
			if (ResRecommendEquipInBattle.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wCombinationID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szCombinationName_ByteArray.GetLength(0) < num)
			{
				this.szCombinationName_ByteArray = new byte[ResRecommendEquipInBattle.LENGTH_szCombinationName];
			}
			errorType = srcBuf.readCString(ref this.szCombinationName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szCombinationDesc_ByteArray.GetLength(0) < num2)
			{
				this.szCombinationDesc_ByteArray = new byte[ResRecommendEquipInBattle.LENGTH_szCombinationDesc];
			}
			errorType = srcBuf.readCString(ref this.szCombinationDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = srcBuf.readUInt16(ref this.RecommendEquipID[i]);
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

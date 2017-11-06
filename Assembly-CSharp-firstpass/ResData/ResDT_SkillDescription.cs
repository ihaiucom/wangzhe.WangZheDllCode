using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_SkillDescription : tsf4g_csharp_interface, IUnpackable
	{
		public byte[] szSkillDescType_ByteArray;

		public byte[] szSkillDescBaseValue_ByteArray;

		public byte[] szSkillDescGrowth_ByteArray;

		public uint dwSkillDescValueType;

		public string szSkillDescType;

		public string szSkillDescBaseValue;

		public string szSkillDescGrowth;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szSkillDescType = 32u;

		public static readonly uint LENGTH_szSkillDescBaseValue = 32u;

		public static readonly uint LENGTH_szSkillDescGrowth = 32u;

		public ResDT_SkillDescription()
		{
			this.szSkillDescType_ByteArray = new byte[1];
			this.szSkillDescBaseValue_ByteArray = new byte[1];
			this.szSkillDescGrowth_ByteArray = new byte[1];
			this.szSkillDescType = string.Empty;
			this.szSkillDescBaseValue = string.Empty;
			this.szSkillDescGrowth = string.Empty;
		}

		private void TransferData()
		{
			this.szSkillDescType = StringHelper.UTF8BytesToString(ref this.szSkillDescType_ByteArray);
			this.szSkillDescType_ByteArray = null;
			this.szSkillDescBaseValue = StringHelper.UTF8BytesToString(ref this.szSkillDescBaseValue_ByteArray);
			this.szSkillDescBaseValue_ByteArray = null;
			this.szSkillDescGrowth = StringHelper.UTF8BytesToString(ref this.szSkillDescGrowth_ByteArray);
			this.szSkillDescGrowth_ByteArray = null;
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
			if (cutVer == 0u || ResDT_SkillDescription.CURRVERSION < cutVer)
			{
				cutVer = ResDT_SkillDescription.CURRVERSION;
			}
			if (ResDT_SkillDescription.BASEVERSION > cutVer)
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
			if (num > (uint)this.szSkillDescType_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResDT_SkillDescription.LENGTH_szSkillDescType)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillDescType_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillDescType_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillDescType_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szSkillDescType_ByteArray) + 1;
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
			if (num3 > (uint)this.szSkillDescBaseValue_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResDT_SkillDescription.LENGTH_szSkillDescBaseValue)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillDescBaseValue_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillDescBaseValue_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillDescBaseValue_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSkillDescBaseValue_ByteArray) + 1;
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
			if (num5 > (uint)this.szSkillDescGrowth_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResDT_SkillDescription.LENGTH_szSkillDescGrowth)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillDescGrowth_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillDescGrowth_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillDescGrowth_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szSkillDescGrowth_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkillDescValueType);
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
			if (cutVer == 0u || ResDT_SkillDescription.CURRVERSION < cutVer)
			{
				cutVer = ResDT_SkillDescription.CURRVERSION;
			}
			if (ResDT_SkillDescription.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 32;
			if (this.szSkillDescType_ByteArray.GetLength(0) < num)
			{
				this.szSkillDescType_ByteArray = new byte[ResDT_SkillDescription.LENGTH_szSkillDescType];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szSkillDescType_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szSkillDescBaseValue_ByteArray.GetLength(0) < num2)
			{
				this.szSkillDescBaseValue_ByteArray = new byte[ResDT_SkillDescription.LENGTH_szSkillDescBaseValue];
			}
			errorType = srcBuf.readCString(ref this.szSkillDescBaseValue_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 32;
			if (this.szSkillDescGrowth_ByteArray.GetLength(0) < num3)
			{
				this.szSkillDescGrowth_ByteArray = new byte[ResDT_SkillDescription.LENGTH_szSkillDescGrowth];
			}
			errorType = srcBuf.readCString(ref this.szSkillDescGrowth_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkillDescValueType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

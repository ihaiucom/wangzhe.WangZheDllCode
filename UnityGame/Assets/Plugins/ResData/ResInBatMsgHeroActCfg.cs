using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResInBatMsgHeroActCfg : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public uint dwHeroID;

		public uint dwActionID;

		public byte[] szContent_ByteArray;

		public byte[] szSound_ByteArray;

		public string szContent;

		public string szSound;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szContent = 128u;

		public static readonly uint LENGTH_szSound = 128u;

		public ResInBatMsgHeroActCfg()
		{
			this.szContent_ByteArray = new byte[1];
			this.szSound_ByteArray = new byte[1];
			this.szContent = string.Empty;
			this.szSound = string.Empty;
		}

		private void TransferData()
		{
			this.szContent = StringHelper.UTF8BytesToString(ref this.szContent_ByteArray);
			this.szContent_ByteArray = null;
			this.szSound = StringHelper.UTF8BytesToString(ref this.szSound_ByteArray);
			this.szSound_ByteArray = null;
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
			if (cutVer == 0u || ResInBatMsgHeroActCfg.CURRVERSION < cutVer)
			{
				cutVer = ResInBatMsgHeroActCfg.CURRVERSION;
			}
			if (ResInBatMsgHeroActCfg.BASEVERSION > cutVer)
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
			errorType = srcBuf.readUInt32(ref this.dwActionID);
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
			if (num > (uint)this.szContent_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResInBatMsgHeroActCfg.LENGTH_szContent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szContent_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szContent_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szContent_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szContent_ByteArray) + 1;
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
			if (num3 > (uint)this.szSound_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResInBatMsgHeroActCfg.LENGTH_szSound)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSound_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSound_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSound_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSound_ByteArray) + 1;
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
			if (cutVer == 0u || ResInBatMsgHeroActCfg.CURRVERSION < cutVer)
			{
				cutVer = ResInBatMsgHeroActCfg.CURRVERSION;
			}
			if (ResInBatMsgHeroActCfg.BASEVERSION > cutVer)
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
			errorType = srcBuf.readUInt32(ref this.dwActionID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szContent_ByteArray.GetLength(0) < num)
			{
				this.szContent_ByteArray = new byte[ResInBatMsgHeroActCfg.LENGTH_szContent];
			}
			errorType = srcBuf.readCString(ref this.szContent_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szSound_ByteArray.GetLength(0) < num2)
			{
				this.szSound_ByteArray = new byte[ResInBatMsgHeroActCfg.LENGTH_szSound];
			}
			errorType = srcBuf.readCString(ref this.szSound_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

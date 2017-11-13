using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResInBatMsgCfg : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public byte bShowType;

		public byte bCampVisible;

		public byte[] szContent_ByteArray;

		public byte[] szSound_ByteArray;

		public byte[] szMiniMapEffect_ByteArray;

		public byte[] szChannelTitle_ByteArray;

		public byte bInBatChannelID;

		public string szContent;

		public string szSound;

		public string szMiniMapEffect;

		public string szChannelTitle;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szContent = 128u;

		public static readonly uint LENGTH_szSound = 128u;

		public static readonly uint LENGTH_szMiniMapEffect = 128u;

		public static readonly uint LENGTH_szChannelTitle = 128u;

		public ResInBatMsgCfg()
		{
			this.szContent_ByteArray = new byte[1];
			this.szSound_ByteArray = new byte[1];
			this.szMiniMapEffect_ByteArray = new byte[1];
			this.szChannelTitle_ByteArray = new byte[1];
			this.szContent = string.Empty;
			this.szSound = string.Empty;
			this.szMiniMapEffect = string.Empty;
			this.szChannelTitle = string.Empty;
		}

		private void TransferData()
		{
			this.szContent = StringHelper.UTF8BytesToString(ref this.szContent_ByteArray);
			this.szContent_ByteArray = null;
			this.szSound = StringHelper.UTF8BytesToString(ref this.szSound_ByteArray);
			this.szSound_ByteArray = null;
			this.szMiniMapEffect = StringHelper.UTF8BytesToString(ref this.szMiniMapEffect_ByteArray);
			this.szMiniMapEffect_ByteArray = null;
			this.szChannelTitle = StringHelper.UTF8BytesToString(ref this.szChannelTitle_ByteArray);
			this.szChannelTitle_ByteArray = null;
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
			if (cutVer == 0u || ResInBatMsgCfg.CURRVERSION < cutVer)
			{
				cutVer = ResInBatMsgCfg.CURRVERSION;
			}
			if (ResInBatMsgCfg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCampVisible);
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
				if ((ulong)num > (ulong)ResInBatMsgCfg.LENGTH_szContent)
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
				if ((ulong)num3 > (ulong)ResInBatMsgCfg.LENGTH_szSound)
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
			if (num5 > (uint)this.szMiniMapEffect_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResInBatMsgCfg.LENGTH_szMiniMapEffect)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMiniMapEffect_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMiniMapEffect_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMiniMapEffect_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szMiniMapEffect_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num7 > (uint)this.szChannelTitle_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResInBatMsgCfg.LENGTH_szChannelTitle)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChannelTitle_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChannelTitle_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChannelTitle_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szChannelTitle_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bInBatChannelID);
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
			if (cutVer == 0u || ResInBatMsgCfg.CURRVERSION < cutVer)
			{
				cutVer = ResInBatMsgCfg.CURRVERSION;
			}
			if (ResInBatMsgCfg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCampVisible);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szContent_ByteArray.GetLength(0) < num)
			{
				this.szContent_ByteArray = new byte[ResInBatMsgCfg.LENGTH_szContent];
			}
			errorType = srcBuf.readCString(ref this.szContent_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szSound_ByteArray.GetLength(0) < num2)
			{
				this.szSound_ByteArray = new byte[ResInBatMsgCfg.LENGTH_szSound];
			}
			errorType = srcBuf.readCString(ref this.szSound_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szMiniMapEffect_ByteArray.GetLength(0) < num3)
			{
				this.szMiniMapEffect_ByteArray = new byte[ResInBatMsgCfg.LENGTH_szMiniMapEffect];
			}
			errorType = srcBuf.readCString(ref this.szMiniMapEffect_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szChannelTitle_ByteArray.GetLength(0) < num4)
			{
				this.szChannelTitle_ByteArray = new byte[ResInBatMsgCfg.LENGTH_szChannelTitle];
			}
			errorType = srcBuf.readCString(ref this.szChannelTitle_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bInBatChannelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

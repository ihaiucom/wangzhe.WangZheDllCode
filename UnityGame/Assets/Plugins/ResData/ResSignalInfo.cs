using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSignalInfo : tsf4g_csharp_interface, IUnpackable
	{
		public byte bSignalID;

		public byte bSignalType;

		public byte[] szUIIcon_ByteArray;

		public byte[] szText_ByteArray;

		public byte[] szSceneEffect_ByteArray;

		public byte[] szRealEffect_ByteArray;

		public byte[] szSound_ByteArray;

		public byte bTime;

		public byte bCooldownTime;

		public string szUIIcon;

		public string szText;

		public string szSceneEffect;

		public string szRealEffect;

		public string szSound;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szUIIcon = 128u;

		public static readonly uint LENGTH_szText = 128u;

		public static readonly uint LENGTH_szSceneEffect = 128u;

		public static readonly uint LENGTH_szRealEffect = 128u;

		public static readonly uint LENGTH_szSound = 128u;

		public ResSignalInfo()
		{
			this.szUIIcon_ByteArray = new byte[1];
			this.szText_ByteArray = new byte[1];
			this.szSceneEffect_ByteArray = new byte[1];
			this.szRealEffect_ByteArray = new byte[1];
			this.szSound_ByteArray = new byte[1];
			this.szUIIcon = string.Empty;
			this.szText = string.Empty;
			this.szSceneEffect = string.Empty;
			this.szRealEffect = string.Empty;
			this.szSound = string.Empty;
		}

		private void TransferData()
		{
			this.szUIIcon = StringHelper.UTF8BytesToString(ref this.szUIIcon_ByteArray);
			this.szUIIcon_ByteArray = null;
			this.szText = StringHelper.UTF8BytesToString(ref this.szText_ByteArray);
			this.szText_ByteArray = null;
			this.szSceneEffect = StringHelper.UTF8BytesToString(ref this.szSceneEffect_ByteArray);
			this.szSceneEffect_ByteArray = null;
			this.szRealEffect = StringHelper.UTF8BytesToString(ref this.szRealEffect_ByteArray);
			this.szRealEffect_ByteArray = null;
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
			if (cutVer == 0u || ResSignalInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSignalInfo.CURRVERSION;
			}
			if (ResSignalInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSignalID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSignalType);
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
			if (num > (uint)this.szUIIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResSignalInfo.LENGTH_szUIIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUIIcon_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUIIcon_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUIIcon_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szUIIcon_ByteArray) + 1;
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
			if (num3 > (uint)this.szText_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResSignalInfo.LENGTH_szText)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szText_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szText_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szText_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szText_ByteArray) + 1;
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
			if (num5 > (uint)this.szSceneEffect_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResSignalInfo.LENGTH_szSceneEffect)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSceneEffect_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSceneEffect_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSceneEffect_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szSceneEffect_ByteArray) + 1;
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
			if (num7 > (uint)this.szRealEffect_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResSignalInfo.LENGTH_szRealEffect)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRealEffect_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRealEffect_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRealEffect_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szRealEffect_ByteArray) + 1;
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
			if (num9 > (uint)this.szSound_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResSignalInfo.LENGTH_szSound)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSound_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSound_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSound_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szSound_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCooldownTime);
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
			if (cutVer == 0u || ResSignalInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSignalInfo.CURRVERSION;
			}
			if (ResSignalInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSignalID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSignalType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szUIIcon_ByteArray.GetLength(0) < num)
			{
				this.szUIIcon_ByteArray = new byte[ResSignalInfo.LENGTH_szUIIcon];
			}
			errorType = srcBuf.readCString(ref this.szUIIcon_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szText_ByteArray.GetLength(0) < num2)
			{
				this.szText_ByteArray = new byte[ResSignalInfo.LENGTH_szText];
			}
			errorType = srcBuf.readCString(ref this.szText_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szSceneEffect_ByteArray.GetLength(0) < num3)
			{
				this.szSceneEffect_ByteArray = new byte[ResSignalInfo.LENGTH_szSceneEffect];
			}
			errorType = srcBuf.readCString(ref this.szSceneEffect_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szRealEffect_ByteArray.GetLength(0) < num4)
			{
				this.szRealEffect_ByteArray = new byte[ResSignalInfo.LENGTH_szRealEffect];
			}
			errorType = srcBuf.readCString(ref this.szRealEffect_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szSound_ByteArray.GetLength(0) < num5)
			{
				this.szSound_ByteArray = new byte[ResSignalInfo.LENGTH_szSound];
			}
			errorType = srcBuf.readCString(ref this.szSound_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCooldownTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

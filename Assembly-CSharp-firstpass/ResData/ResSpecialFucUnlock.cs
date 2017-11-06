using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSpecialFucUnlock : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwFucID;

		public uint[] UnlockArray;

		public byte[] szLockedTip_ByteArray;

		public byte bIsShowUnlockTip;

		public byte[] szUnlockTip_ByteArray;

		public byte[] szUnlockTipIcon_ByteArray;

		public byte bIsAnd;

		public string szLockedTip;

		public string szUnlockTip;

		public string szUnlockTipIcon;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szLockedTip = 128u;

		public static readonly uint LENGTH_szUnlockTip = 128u;

		public static readonly uint LENGTH_szUnlockTipIcon = 128u;

		public ResSpecialFucUnlock()
		{
			this.UnlockArray = new uint[4];
			this.szLockedTip_ByteArray = new byte[1];
			this.szUnlockTip_ByteArray = new byte[1];
			this.szUnlockTipIcon_ByteArray = new byte[1];
			this.szLockedTip = string.Empty;
			this.szUnlockTip = string.Empty;
			this.szUnlockTipIcon = string.Empty;
		}

		private void TransferData()
		{
			this.szLockedTip = StringHelper.UTF8BytesToString(ref this.szLockedTip_ByteArray);
			this.szLockedTip_ByteArray = null;
			this.szUnlockTip = StringHelper.UTF8BytesToString(ref this.szUnlockTip_ByteArray);
			this.szUnlockTip_ByteArray = null;
			this.szUnlockTipIcon = StringHelper.UTF8BytesToString(ref this.szUnlockTipIcon_ByteArray);
			this.szUnlockTipIcon_ByteArray = null;
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
			if (cutVer == 0u || ResSpecialFucUnlock.CURRVERSION < cutVer)
			{
				cutVer = ResSpecialFucUnlock.CURRVERSION;
			}
			if (ResSpecialFucUnlock.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwFucID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 4; i++)
			{
				errorType = srcBuf.readUInt32(ref this.UnlockArray[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (num > (uint)this.szLockedTip_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResSpecialFucUnlock.LENGTH_szLockedTip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLockedTip_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLockedTip_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLockedTip_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szLockedTip_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsShowUnlockTip);
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
			if (num3 > (uint)this.szUnlockTip_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResSpecialFucUnlock.LENGTH_szUnlockTip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUnlockTip_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUnlockTip_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUnlockTip_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szUnlockTip_ByteArray) + 1;
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
			if (num5 > (uint)this.szUnlockTipIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResSpecialFucUnlock.LENGTH_szUnlockTipIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUnlockTipIcon_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUnlockTipIcon_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUnlockTipIcon_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szUnlockTipIcon_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAnd);
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
			if (cutVer == 0u || ResSpecialFucUnlock.CURRVERSION < cutVer)
			{
				cutVer = ResSpecialFucUnlock.CURRVERSION;
			}
			if (ResSpecialFucUnlock.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwFucID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 4; i++)
			{
				errorType = srcBuf.readUInt32(ref this.UnlockArray[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num = 128;
			if (this.szLockedTip_ByteArray.GetLength(0) < num)
			{
				this.szLockedTip_ByteArray = new byte[ResSpecialFucUnlock.LENGTH_szLockedTip];
			}
			errorType = srcBuf.readCString(ref this.szLockedTip_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsShowUnlockTip);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szUnlockTip_ByteArray.GetLength(0) < num2)
			{
				this.szUnlockTip_ByteArray = new byte[ResSpecialFucUnlock.LENGTH_szUnlockTip];
			}
			errorType = srcBuf.readCString(ref this.szUnlockTip_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szUnlockTipIcon_ByteArray.GetLength(0) < num3)
			{
				this.szUnlockTipIcon_ByteArray = new byte[ResSpecialFucUnlock.LENGTH_szUnlockTipIcon];
			}
			errorType = srcBuf.readCString(ref this.szUnlockTipIcon_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAnd);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

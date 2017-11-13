using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_LevelReward_UnlockInfo : tsf4g_csharp_interface, IUnpackable
	{
		public byte[] szUnlockID_ByteArray;

		public byte bGotoID;

		public byte[] szIcon_ByteArray;

		public string szUnlockID;

		public string szIcon;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szUnlockID = 128u;

		public static readonly uint LENGTH_szIcon = 128u;

		public ResDT_LevelReward_UnlockInfo()
		{
			this.szUnlockID_ByteArray = new byte[1];
			this.szIcon_ByteArray = new byte[1];
			this.szUnlockID = string.Empty;
			this.szIcon = string.Empty;
		}

		private void TransferData()
		{
			this.szUnlockID = StringHelper.UTF8BytesToString(ref this.szUnlockID_ByteArray);
			this.szUnlockID_ByteArray = null;
			this.szIcon = StringHelper.UTF8BytesToString(ref this.szIcon_ByteArray);
			this.szIcon_ByteArray = null;
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
			if (cutVer == 0u || ResDT_LevelReward_UnlockInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_LevelReward_UnlockInfo.CURRVERSION;
			}
			if (ResDT_LevelReward_UnlockInfo.BASEVERSION > cutVer)
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
			if (num > (uint)this.szUnlockID_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResDT_LevelReward_UnlockInfo.LENGTH_szUnlockID)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUnlockID_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUnlockID_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUnlockID_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szUnlockID_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bGotoID);
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
			if (num3 > (uint)this.szIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResDT_LevelReward_UnlockInfo.LENGTH_szIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIcon_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIcon_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIcon_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szIcon_ByteArray) + 1;
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
			if (cutVer == 0u || ResDT_LevelReward_UnlockInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_LevelReward_UnlockInfo.CURRVERSION;
			}
			if (ResDT_LevelReward_UnlockInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 128;
			if (this.szUnlockID_ByteArray.GetLength(0) < num)
			{
				this.szUnlockID_ByteArray = new byte[ResDT_LevelReward_UnlockInfo.LENGTH_szUnlockID];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szUnlockID_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGotoID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szIcon_ByteArray.GetLength(0) < num2)
			{
				this.szIcon_ByteArray = new byte[ResDT_LevelReward_UnlockInfo.LENGTH_szIcon];
			}
			errorType = srcBuf.readCString(ref this.szIcon_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResChapterInfo : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwChapterId;

		public byte[] szChapterName_ByteArray;

		public uint dwUnlockLevel;

		public ResDT_ChapterRewardInfo[] astNormalRewardDetail;

		public ResDT_ChapterRewardInfo[] astEliteRewardDetail;

		public ResDT_ChapterRewardInfo[] astMasterRewardDetail;

		public ResDT_ChapterRewardInfo[] astAbyssRewardDetail;

		public byte[] szChapterIcon_ByteArray;

		public byte[] szLockedTip_ByteArray;

		public byte[] szUnlockTip_ByteArray;

		public byte[] szChapterDesc_ByteArray;

		public string szChapterName;

		public string szChapterIcon;

		public string szLockedTip;

		public string szUnlockTip;

		public string szChapterDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szChapterName = 64u;

		public static readonly uint LENGTH_szChapterIcon = 128u;

		public static readonly uint LENGTH_szLockedTip = 128u;

		public static readonly uint LENGTH_szUnlockTip = 128u;

		public static readonly uint LENGTH_szChapterDesc = 150u;

		public ResChapterInfo()
		{
			this.szChapterName_ByteArray = new byte[1];
			this.astNormalRewardDetail = new ResDT_ChapterRewardInfo[5];
			for (int i = 0; i < 5; i++)
			{
				this.astNormalRewardDetail[i] = new ResDT_ChapterRewardInfo();
			}
			this.astEliteRewardDetail = new ResDT_ChapterRewardInfo[5];
			for (int j = 0; j < 5; j++)
			{
				this.astEliteRewardDetail[j] = new ResDT_ChapterRewardInfo();
			}
			this.astMasterRewardDetail = new ResDT_ChapterRewardInfo[5];
			for (int k = 0; k < 5; k++)
			{
				this.astMasterRewardDetail[k] = new ResDT_ChapterRewardInfo();
			}
			this.astAbyssRewardDetail = new ResDT_ChapterRewardInfo[5];
			for (int l = 0; l < 5; l++)
			{
				this.astAbyssRewardDetail[l] = new ResDT_ChapterRewardInfo();
			}
			this.szChapterIcon_ByteArray = new byte[1];
			this.szLockedTip_ByteArray = new byte[1];
			this.szUnlockTip_ByteArray = new byte[1];
			this.szChapterDesc_ByteArray = new byte[1];
			this.szChapterName = string.Empty;
			this.szChapterIcon = string.Empty;
			this.szLockedTip = string.Empty;
			this.szUnlockTip = string.Empty;
			this.szChapterDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szChapterName = StringHelper.UTF8BytesToString(ref this.szChapterName_ByteArray);
			this.szChapterName_ByteArray = null;
			this.szChapterIcon = StringHelper.UTF8BytesToString(ref this.szChapterIcon_ByteArray);
			this.szChapterIcon_ByteArray = null;
			this.szLockedTip = StringHelper.UTF8BytesToString(ref this.szLockedTip_ByteArray);
			this.szLockedTip_ByteArray = null;
			this.szUnlockTip = StringHelper.UTF8BytesToString(ref this.szUnlockTip_ByteArray);
			this.szUnlockTip_ByteArray = null;
			this.szChapterDesc = StringHelper.UTF8BytesToString(ref this.szChapterDesc_ByteArray);
			this.szChapterDesc_ByteArray = null;
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
			if (cutVer == 0u || ResChapterInfo.CURRVERSION < cutVer)
			{
				cutVer = ResChapterInfo.CURRVERSION;
			}
			if (ResChapterInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwChapterId);
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
			if (num > (uint)this.szChapterName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResChapterInfo.LENGTH_szChapterName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChapterName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChapterName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChapterName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szChapterName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwUnlockLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astNormalRewardDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astEliteRewardDetail[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 5; k++)
			{
				errorType = this.astMasterRewardDetail[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int l = 0; l < 5; l++)
			{
				errorType = this.astAbyssRewardDetail[l].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (num3 > (uint)this.szChapterIcon_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResChapterInfo.LENGTH_szChapterIcon)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChapterIcon_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChapterIcon_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChapterIcon_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szChapterIcon_ByteArray) + 1;
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
			if (num5 > (uint)this.szLockedTip_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResChapterInfo.LENGTH_szLockedTip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLockedTip_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLockedTip_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLockedTip_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szLockedTip_ByteArray) + 1;
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
			if (num7 > (uint)this.szUnlockTip_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResChapterInfo.LENGTH_szUnlockTip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUnlockTip_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUnlockTip_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUnlockTip_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szUnlockTip_ByteArray) + 1;
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
			if (num9 > (uint)this.szChapterDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResChapterInfo.LENGTH_szChapterDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szChapterDesc_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szChapterDesc_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szChapterDesc_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szChapterDesc_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
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
			if (cutVer == 0u || ResChapterInfo.CURRVERSION < cutVer)
			{
				cutVer = ResChapterInfo.CURRVERSION;
			}
			if (ResChapterInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwChapterId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 64;
			if (this.szChapterName_ByteArray.GetLength(0) < num)
			{
				this.szChapterName_ByteArray = new byte[ResChapterInfo.LENGTH_szChapterName];
			}
			errorType = srcBuf.readCString(ref this.szChapterName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUnlockLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astNormalRewardDetail[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astEliteRewardDetail[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 5; k++)
			{
				errorType = this.astMasterRewardDetail[k].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int l = 0; l < 5; l++)
			{
				errorType = this.astAbyssRewardDetail[l].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num2 = 128;
			if (this.szChapterIcon_ByteArray.GetLength(0) < num2)
			{
				this.szChapterIcon_ByteArray = new byte[ResChapterInfo.LENGTH_szChapterIcon];
			}
			errorType = srcBuf.readCString(ref this.szChapterIcon_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szLockedTip_ByteArray.GetLength(0) < num3)
			{
				this.szLockedTip_ByteArray = new byte[ResChapterInfo.LENGTH_szLockedTip];
			}
			errorType = srcBuf.readCString(ref this.szLockedTip_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szUnlockTip_ByteArray.GetLength(0) < num4)
			{
				this.szUnlockTip_ByteArray = new byte[ResChapterInfo.LENGTH_szUnlockTip];
			}
			errorType = srcBuf.readCString(ref this.szUnlockTip_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 150;
			if (this.szChapterDesc_ByteArray.GetLength(0) < num5)
			{
				this.szChapterDesc_ByteArray = new byte[ResChapterInfo.LENGTH_szChapterDesc];
			}
			errorType = srcBuf.readCString(ref this.szChapterDesc_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}

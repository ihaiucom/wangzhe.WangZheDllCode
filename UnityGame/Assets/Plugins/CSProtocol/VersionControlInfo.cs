using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class VersionControlInfo : tsf4g_csharp_interface, IPackable, IUnpackable
	{
		public byte[] szAllowedLowVer;

		public byte[] szCurrentVer;

		public ulong ullAllowedLowVerUint;

		public ulong ullCurrentVerUint;

		public int iReviewVerCount;

		public byte[][] aszReviewVer;

		public ulong[] ReviewVerUint;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szAllowedLowVer = 64u;

		public static readonly uint LENGTH_szCurrentVer = 64u;

		public static readonly uint LENGTH_aszReviewVer = 64u;

		public VersionControlInfo()
		{
			this.szAllowedLowVer = new byte[64];
			this.szCurrentVer = new byte[64];
			this.aszReviewVer = new byte[10][];
			for (int i = 0; i < 10; i++)
			{
				this.aszReviewVer[i] = new byte[64];
			}
			this.ReviewVerUint = new ulong[10];
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = new TdrWriteBuf(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			return errorType;
		}

		public TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || VersionControlInfo.CURRVERSION < cutVer)
			{
				cutVer = VersionControlInfo.CURRVERSION;
			}
			if (VersionControlInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int usedSize = destBuf.getUsedSize();
			TdrError.ErrorType errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szAllowedLowVer);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szAllowedLowVer, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szCurrentVer);
			if (num2 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szCurrentVer, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src2 = destBuf.getUsedSize() - usedSize4;
			errorType = destBuf.writeUInt32((uint)src2, usedSize3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullAllowedLowVerUint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullCurrentVerUint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iReviewVerCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.aszReviewVer.Length < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < this.iReviewVerCount; i++)
			{
				int usedSize5 = destBuf.getUsedSize();
				errorType = destBuf.reserve(4);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int usedSize6 = destBuf.getUsedSize();
				int num3 = TdrTypeUtil.cstrlen(this.aszReviewVer[i]);
				if (num3 >= 64)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				errorType = destBuf.writeCString(this.aszReviewVer[i], num3);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				errorType = destBuf.writeUInt8(0);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int src3 = destBuf.getUsedSize() - usedSize6;
				errorType = destBuf.writeUInt32((uint)src3, usedSize5);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (0 > this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.ReviewVerUint.Length < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < this.iReviewVerCount; j++)
			{
				errorType = destBuf.writeUInt64(this.ReviewVerUint[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
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
			if (cutVer == 0u || VersionControlInfo.CURRVERSION < cutVer)
			{
				cutVer = VersionControlInfo.CURRVERSION;
			}
			if (VersionControlInfo.BASEVERSION > cutVer)
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
			if (num > (uint)this.szAllowedLowVer.GetLength(0))
			{
				if ((ulong)num > (ulong)VersionControlInfo.LENGTH_szAllowedLowVer)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAllowedLowVer = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAllowedLowVer, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAllowedLowVer[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szAllowedLowVer) + 1;
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
			if (num3 > (uint)this.szCurrentVer.GetLength(0))
			{
				if ((ulong)num3 > (ulong)VersionControlInfo.LENGTH_szCurrentVer)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCurrentVer = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCurrentVer, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCurrentVer[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szCurrentVer) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt64(ref this.ullAllowedLowVerUint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullCurrentVerUint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iReviewVerCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.aszReviewVer = new byte[this.iReviewVerCount][];
			for (int i = 0; i < this.iReviewVerCount; i++)
			{
				this.aszReviewVer[i] = new byte[1];
			}
			for (int j = 0; j < this.iReviewVerCount; j++)
			{
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
				if (num5 > (uint)this.aszReviewVer[j].GetLength(0))
				{
					if ((ulong)num5 > (ulong)VersionControlInfo.LENGTH_aszReviewVer)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					this.aszReviewVer[j] = new byte[num5];
				}
				if (1u > num5)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
				}
				errorType = srcBuf.readCString(ref this.aszReviewVer[j], (int)num5);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				if (this.aszReviewVer[j][(int)(num5 - 1u)] != 0)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
				int num6 = TdrTypeUtil.cstrlen(this.aszReviewVer[j]) + 1;
				if ((ulong)num5 != (ulong)((long)num6))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
			}
			if (0 > this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iReviewVerCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.ReviewVerUint = new ulong[this.iReviewVerCount];
			for (int k = 0; k < this.iReviewVerCount; k++)
			{
				errorType = srcBuf.readUInt64(ref this.ReviewVerUint[k]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}
	}
}

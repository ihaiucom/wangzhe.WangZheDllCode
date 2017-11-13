using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResTrophyLvl : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwTrophyLvl;

		public uint dwTrophyScore;

		public byte[] szTrophyDesc_ByteArray;

		public uint dwImage;

		public ResRewardInfo[] astMailReward;

		public ResRewardInfo[] astReqReward;

		public string szTrophyDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szTrophyDesc = 64u;

		public ResTrophyLvl()
		{
			this.szTrophyDesc_ByteArray = new byte[1];
			this.astMailReward = new ResRewardInfo[3];
			for (int i = 0; i < 3; i++)
			{
				this.astMailReward[i] = new ResRewardInfo();
			}
			this.astReqReward = new ResRewardInfo[3];
			for (int j = 0; j < 3; j++)
			{
				this.astReqReward[j] = new ResRewardInfo();
			}
			this.szTrophyDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szTrophyDesc = StringHelper.UTF8BytesToString(ref this.szTrophyDesc_ByteArray);
			this.szTrophyDesc_ByteArray = null;
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
			if (cutVer == 0u || ResTrophyLvl.CURRVERSION < cutVer)
			{
				cutVer = ResTrophyLvl.CURRVERSION;
			}
			if (ResTrophyLvl.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTrophyLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTrophyScore);
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
			if (num > (uint)this.szTrophyDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResTrophyLvl.LENGTH_szTrophyDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTrophyDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTrophyDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTrophyDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szTrophyDesc_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwImage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astMailReward[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = this.astReqReward[j].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResTrophyLvl.CURRVERSION < cutVer)
			{
				cutVer = ResTrophyLvl.CURRVERSION;
			}
			if (ResTrophyLvl.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTrophyLvl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTrophyScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 64;
			if (this.szTrophyDesc_ByteArray.GetLength(0) < num)
			{
				this.szTrophyDesc_ByteArray = new byte[ResTrophyLvl.LENGTH_szTrophyDesc];
			}
			errorType = srcBuf.readCString(ref this.szTrophyDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwImage);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astMailReward[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = this.astReqReward[j].load(ref srcBuf, cutVer);
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

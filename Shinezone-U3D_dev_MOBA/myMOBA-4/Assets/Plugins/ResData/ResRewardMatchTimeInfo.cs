using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRewardMatchTimeInfo : IUnpackable, tsf4g_csharp_interface
	{
		public byte bMapType;

		public uint dwMapId;

		public ulong ullStartDate;

		public ulong ullEndDate;

		public byte bIsOpen;

		public byte[] szTimeTips_ByteArray;

		public byte bCycleType;

		public byte bCycleParmNum;

		public uint[] CycleParm;

		public byte bSignUpCycleParmNum;

		public uint[] SignUpCycleParm;

		public ResActTime[] astActTime;

		public ResActTime[] astSignUpActTime;

		public string szTimeTips;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szTimeTips = 64u;

		public ResRewardMatchTimeInfo()
		{
			this.szTimeTips_ByteArray = new byte[1];
			this.CycleParm = new uint[1];
			this.SignUpCycleParm = new uint[1];
			this.astActTime = new ResActTime[3];
			for (int i = 0; i < 3; i++)
			{
				this.astActTime[i] = new ResActTime();
			}
			this.astSignUpActTime = new ResActTime[6];
			for (int j = 0; j < 6; j++)
			{
				this.astSignUpActTime[j] = new ResActTime();
			}
			this.szTimeTips = string.Empty;
		}

		private void TransferData()
		{
			this.szTimeTips = StringHelper.UTF8BytesToString(ref this.szTimeTips_ByteArray);
			this.szTimeTips_ByteArray = null;
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
			if (cutVer == 0u || ResRewardMatchTimeInfo.CURRVERSION < cutVer)
			{
				cutVer = ResRewardMatchTimeInfo.CURRVERSION;
			}
			if (ResRewardMatchTimeInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMapType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMapId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullStartDate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullEndDate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpen);
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
			if (num > (uint)this.szTimeTips_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResRewardMatchTimeInfo.LENGTH_szTimeTips))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTimeTips_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTimeTips_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTimeTips_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szTimeTips_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bCycleType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCycleParmNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bCycleParmNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.CycleParm = new uint[(int)this.bCycleParmNum];
			for (int i = 0; i < (int)this.bCycleParmNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.CycleParm[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSignUpCycleParmNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bSignUpCycleParmNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.SignUpCycleParm = new uint[(int)this.bSignUpCycleParmNum];
			for (int j = 0; j < (int)this.bSignUpCycleParmNum; j++)
			{
				errorType = srcBuf.readUInt32(ref this.SignUpCycleParm[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astActTime[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int l = 0; l < 6; l++)
			{
				errorType = this.astSignUpActTime[l].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResRewardMatchTimeInfo.CURRVERSION < cutVer)
			{
				cutVer = ResRewardMatchTimeInfo.CURRVERSION;
			}
			if (ResRewardMatchTimeInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMapType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMapId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullStartDate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullEndDate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 64;
			if (this.szTimeTips_ByteArray.GetLength(0) < num)
			{
				this.szTimeTips_ByteArray = new byte[ResRewardMatchTimeInfo.LENGTH_szTimeTips];
			}
			errorType = srcBuf.readCString(ref this.szTimeTips_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCycleType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCycleParmNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.CycleParm.Length < 6)
			{
				this.CycleParm = new uint[6];
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = srcBuf.readUInt32(ref this.CycleParm[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSignUpCycleParmNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.SignUpCycleParm.Length < 6)
			{
				this.SignUpCycleParm = new uint[6];
			}
			for (int j = 0; j < 6; j++)
			{
				errorType = srcBuf.readUInt32(ref this.SignUpCycleParm[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astActTime[k].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int l = 0; l < 6; l++)
			{
				errorType = this.astSignUpActTime[l].load(ref srcBuf, cutVer);
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

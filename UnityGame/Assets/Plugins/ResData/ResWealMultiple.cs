using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResWealMultiple : tsf4g_csharp_interface, IUnpackable
	{
		public uint dwID;

		public ResDT_WealCommon stCommon;

		public uint dwPvpLvlLimit;

		public ushort wPeriodNum;

		public ResDT_WealMultiplePeriod[] astPeriod;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public ResWealMultiple()
		{
			this.stCommon = new ResDT_WealCommon();
			this.astPeriod = new ResDT_WealMultiplePeriod[1];
			this.astPeriod[0] = new ResDT_WealMultiplePeriod();
		}

		private void TransferData()
		{
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
			if (cutVer == 0u || ResWealMultiple.CURRVERSION < cutVer)
			{
				cutVer = ResWealMultiple.CURRVERSION;
			}
			if (ResWealMultiple.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommon.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLvlLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wPeriodNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.wPeriodNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astPeriod = new ResDT_WealMultiplePeriod[(int)this.wPeriodNum];
			for (int i = 0; i < (int)this.wPeriodNum; i++)
			{
				this.astPeriod[i] = new ResDT_WealMultiplePeriod();
			}
			for (int j = 0; j < (int)this.wPeriodNum; j++)
			{
				errorType = this.astPeriod[j].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResWealMultiple.CURRVERSION < cutVer)
			{
				cutVer = ResWealMultiple.CURRVERSION;
			}
			if (ResWealMultiple.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommon.load(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLvlLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wPeriodNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.astPeriod.Length < 4)
			{
				this.astPeriod = new ResDT_WealMultiplePeriod[4];
				for (int i = 0; i < 4; i++)
				{
					this.astPeriod[i] = new ResDT_WealMultiplePeriod();
				}
			}
			for (int j = 0; j < 4; j++)
			{
				errorType = this.astPeriod[j].load(ref srcBuf, cutVer);
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

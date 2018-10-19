using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResLuckyDrawExternReward : IUnpackable, tsf4g_csharp_interface
	{
		public byte bMoneyType;

		public byte bExternCnt;

		public ResDT_LuckyDrawExternReward[] astReward;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public ResLuckyDrawExternReward()
		{
			this.astReward = new ResDT_LuckyDrawExternReward[1];
			this.astReward[0] = new ResDT_LuckyDrawExternReward();
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
			if (cutVer == 0u || ResLuckyDrawExternReward.CURRVERSION < cutVer)
			{
				cutVer = ResLuckyDrawExternReward.CURRVERSION;
			}
			if (ResLuckyDrawExternReward.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMoneyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bExternCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bExternCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astReward = new ResDT_LuckyDrawExternReward[(int)this.bExternCnt];
			for (int i = 0; i < (int)this.bExternCnt; i++)
			{
				this.astReward[i] = new ResDT_LuckyDrawExternReward();
			}
			for (int j = 0; j < (int)this.bExternCnt; j++)
			{
				errorType = this.astReward[j].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResLuckyDrawExternReward.CURRVERSION < cutVer)
			{
				cutVer = ResLuckyDrawExternReward.CURRVERSION;
			}
			if (ResLuckyDrawExternReward.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMoneyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bExternCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.astReward.Length < 5)
			{
				this.astReward = new ResDT_LuckyDrawExternReward[5];
				for (int i = 0; i < 5; i++)
				{
					this.astReward[i] = new ResDT_LuckyDrawExternReward();
				}
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astReward[j].load(ref srcBuf, cutVer);
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

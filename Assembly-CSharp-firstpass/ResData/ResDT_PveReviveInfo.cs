using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_PveReviveInfo : tsf4g_csharp_interface, IUnpackable
	{
		public ResDT_CostInfo[] astReviveCost;

		public uint[] ReviveBuff;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public ResDT_PveReviveInfo()
		{
			this.astReviveCost = new ResDT_CostInfo[3];
			for (int i = 0; i < 3; i++)
			{
				this.astReviveCost[i] = new ResDT_CostInfo();
			}
			this.ReviveBuff = new uint[3];
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || ResDT_PveReviveInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_PveReviveInfo.CURRVERSION;
			}
			if (ResDT_PveReviveInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astReviveCost[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = srcBuf.readUInt32(ref this.ReviveBuff[j]);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || ResDT_PveReviveInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_PveReviveInfo.CURRVERSION;
			}
			if (ResDT_PveReviveInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astReviveCost[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				errorType = srcBuf.readUInt32(ref this.ReviveBuff[j]);
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

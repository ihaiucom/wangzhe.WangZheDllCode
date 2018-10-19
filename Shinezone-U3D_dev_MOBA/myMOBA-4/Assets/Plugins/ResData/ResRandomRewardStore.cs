using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRandomRewardStore : IUnpackable, tsf4g_csharp_interface
	{
		public uint dwRewardID;

		public byte[] szRewardDesc_ByteArray;

		public byte bCalculateType;

		public uint dwMissOverlie;

		public byte bRewardNum;

		public ResDT_RandomRewardInfo[] astRewardDetail;

		public string szRewardDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szRewardDesc = 100u;

		public ResRandomRewardStore()
		{
			this.szRewardDesc_ByteArray = new byte[1];
			this.astRewardDetail = new ResDT_RandomRewardInfo[1];
			this.astRewardDetail[0] = new ResDT_RandomRewardInfo();
			this.szRewardDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szRewardDesc = StringHelper.UTF8BytesToString(ref this.szRewardDesc_ByteArray);
			this.szRewardDesc_ByteArray = null;
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
			if (cutVer == 0u || ResRandomRewardStore.CURRVERSION < cutVer)
			{
				cutVer = ResRandomRewardStore.CURRVERSION;
			}
			if (ResRandomRewardStore.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRewardID);
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
			if (num > (uint)this.szRewardDesc_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResRandomRewardStore.LENGTH_szRewardDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRewardDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRewardDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRewardDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szRewardDesc_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bCalculateType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMissOverlie);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRewardNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bRewardNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astRewardDetail = new ResDT_RandomRewardInfo[(int)this.bRewardNum];
			for (int i = 0; i < (int)this.bRewardNum; i++)
			{
				this.astRewardDetail[i] = new ResDT_RandomRewardInfo();
			}
			for (int j = 0; j < (int)this.bRewardNum; j++)
			{
				errorType = this.astRewardDetail[j].unpack(ref srcBuf, cutVer);
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
			if (cutVer == 0u || ResRandomRewardStore.CURRVERSION < cutVer)
			{
				cutVer = ResRandomRewardStore.CURRVERSION;
			}
			if (ResRandomRewardStore.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwRewardID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 100;
			if (this.szRewardDesc_ByteArray.GetLength(0) < num)
			{
				this.szRewardDesc_ByteArray = new byte[ResRandomRewardStore.LENGTH_szRewardDesc];
			}
			errorType = srcBuf.readCString(ref this.szRewardDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCalculateType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMissOverlie);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRewardNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.astRewardDetail.Length < 20)
			{
				this.astRewardDetail = new ResDT_RandomRewardInfo[20];
				for (int i = 0; i < 20; i++)
				{
					this.astRewardDetail[i] = new ResDT_RandomRewardInfo();
				}
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = this.astRewardDetail[j].load(ref srcBuf, cutVer);
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

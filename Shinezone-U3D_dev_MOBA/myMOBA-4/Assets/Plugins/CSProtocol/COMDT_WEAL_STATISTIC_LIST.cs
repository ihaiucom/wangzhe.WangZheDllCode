using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_STATISTIC_LIST : ProtocolObject
	{
		public byte bListNum;

		public COMDT_WEAL_STATISTIC_DATA[] astStatisticList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 38u;

		public static readonly int CLASS_ID = 502;

		public COMDT_WEAL_STATISTIC_LIST()
		{
			this.astStatisticList = new COMDT_WEAL_STATISTIC_DATA[4];
			for (int i = 0; i < 4; i++)
			{
				this.astStatisticList[i] = (COMDT_WEAL_STATISTIC_DATA)ProtocolObjectPool.Get(COMDT_WEAL_STATISTIC_DATA.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_WEAL_STATISTIC_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_STATISTIC_LIST.CURRVERSION;
			}
			if (COMDT_WEAL_STATISTIC_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bListNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bListNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astStatisticList.Length < (int)this.bListNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bListNum; i++)
			{
				errorType = this.astStatisticList[i].pack(ref destBuf, cutVer);
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
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_WEAL_STATISTIC_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_STATISTIC_LIST.CURRVERSION;
			}
			if (COMDT_WEAL_STATISTIC_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bListNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bListNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bListNum; i++)
			{
				errorType = this.astStatisticList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WEAL_STATISTIC_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bListNum = 0;
			if (this.astStatisticList != null)
			{
				for (int i = 0; i < this.astStatisticList.Length; i++)
				{
					if (this.astStatisticList[i] != null)
					{
						this.astStatisticList[i].Release();
						this.astStatisticList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astStatisticList != null)
			{
				for (int i = 0; i < this.astStatisticList.Length; i++)
				{
					this.astStatisticList[i] = (COMDT_WEAL_STATISTIC_DATA)ProtocolObjectPool.Get(COMDT_WEAL_STATISTIC_DATA.CLASS_ID);
				}
			}
		}
	}
}

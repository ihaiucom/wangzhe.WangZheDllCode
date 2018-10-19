using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_STATISTIC_DATA : ProtocolObject
	{
		public uint dwWealID;

		public byte bSingleNum;

		public COMDT_STATISTIC_DATA_INFO_SINGLE[] astSingleDetail;

		public byte bMultiNum;

		public COMDT_STATISTIC_DATA_INFO_MULTI[] astMultiDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 38u;

		public static readonly int CLASS_ID = 501;

		public COMDT_WEAL_STATISTIC_DATA()
		{
			this.astSingleDetail = new COMDT_STATISTIC_DATA_INFO_SINGLE[20];
			for (int i = 0; i < 20; i++)
			{
				this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
			}
			this.astMultiDetail = new COMDT_STATISTIC_DATA_INFO_MULTI[40];
			for (int j = 0; j < 40; j++)
			{
				this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
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
			if (cutVer == 0u || COMDT_WEAL_STATISTIC_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_STATISTIC_DATA.CURRVERSION;
			}
			if (COMDT_WEAL_STATISTIC_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwWealID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSingleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSingleDetail.Length < (int)this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bSingleNum; i++)
			{
				errorType = this.astSingleDetail[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bMultiNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMultiDetail.Length < (int)this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bMultiNum; j++)
			{
				errorType = this.astMultiDetail[j].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_WEAL_STATISTIC_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_STATISTIC_DATA.CURRVERSION;
			}
			if (COMDT_WEAL_STATISTIC_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwWealID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSingleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bSingleNum; i++)
			{
				errorType = this.astSingleDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bMultiNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int j = 0; j < (int)this.bMultiNum; j++)
			{
				errorType = this.astMultiDetail[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WEAL_STATISTIC_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwWealID = 0u;
			this.bSingleNum = 0;
			if (this.astSingleDetail != null)
			{
				for (int i = 0; i < this.astSingleDetail.Length; i++)
				{
					if (this.astSingleDetail[i] != null)
					{
						this.astSingleDetail[i].Release();
						this.astSingleDetail[i] = null;
					}
				}
			}
			this.bMultiNum = 0;
			if (this.astMultiDetail != null)
			{
				for (int j = 0; j < this.astMultiDetail.Length; j++)
				{
					if (this.astMultiDetail[j] != null)
					{
						this.astMultiDetail[j].Release();
						this.astMultiDetail[j] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSingleDetail != null)
			{
				for (int i = 0; i < this.astSingleDetail.Length; i++)
				{
					this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
				}
			}
			if (this.astMultiDetail != null)
			{
				for (int j = 0; j < this.astMultiDetail.Length; j++)
				{
					this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
				}
			}
		}
	}
}

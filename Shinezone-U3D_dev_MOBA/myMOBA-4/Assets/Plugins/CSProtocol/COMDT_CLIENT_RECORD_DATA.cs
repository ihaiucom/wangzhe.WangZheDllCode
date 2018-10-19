using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_CLIENT_RECORD_DATA : ProtocolObject
	{
		public ushort wClientFPSNum;

		public COMDT_CLIENT_PERFORMANCE_KV_INFO[] astClientFPSDetail;

		public ushort wClientPingNum;

		public COMDT_CLIENT_PERFORMANCE_KV_INFO[] astClientPingDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 193;

		public COMDT_CLIENT_RECORD_DATA()
		{
			this.astClientFPSDetail = new COMDT_CLIENT_PERFORMANCE_KV_INFO[500];
			for (int i = 0; i < 500; i++)
			{
				this.astClientFPSDetail[i] = (COMDT_CLIENT_PERFORMANCE_KV_INFO)ProtocolObjectPool.Get(COMDT_CLIENT_PERFORMANCE_KV_INFO.CLASS_ID);
			}
			this.astClientPingDetail = new COMDT_CLIENT_PERFORMANCE_KV_INFO[500];
			for (int j = 0; j < 500; j++)
			{
				this.astClientPingDetail[j] = (COMDT_CLIENT_PERFORMANCE_KV_INFO)ProtocolObjectPool.Get(COMDT_CLIENT_PERFORMANCE_KV_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_CLIENT_RECORD_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CLIENT_RECORD_DATA.CURRVERSION;
			}
			if (COMDT_CLIENT_RECORD_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wClientFPSNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wClientFPSNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astClientFPSDetail.Length < (int)this.wClientFPSNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wClientFPSNum; i++)
			{
				errorType = this.astClientFPSDetail[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt16(this.wClientPingNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wClientPingNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astClientPingDetail.Length < (int)this.wClientPingNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.wClientPingNum; j++)
			{
				errorType = this.astClientPingDetail[j].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_CLIENT_RECORD_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CLIENT_RECORD_DATA.CURRVERSION;
			}
			if (COMDT_CLIENT_RECORD_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wClientFPSNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wClientFPSNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wClientFPSNum; i++)
			{
				errorType = this.astClientFPSDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt16(ref this.wClientPingNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500 < this.wClientPingNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int j = 0; j < (int)this.wClientPingNum; j++)
			{
				errorType = this.astClientPingDetail[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_CLIENT_RECORD_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wClientFPSNum = 0;
			if (this.astClientFPSDetail != null)
			{
				for (int i = 0; i < this.astClientFPSDetail.Length; i++)
				{
					if (this.astClientFPSDetail[i] != null)
					{
						this.astClientFPSDetail[i].Release();
						this.astClientFPSDetail[i] = null;
					}
				}
			}
			this.wClientPingNum = 0;
			if (this.astClientPingDetail != null)
			{
				for (int j = 0; j < this.astClientPingDetail.Length; j++)
				{
					if (this.astClientPingDetail[j] != null)
					{
						this.astClientPingDetail[j].Release();
						this.astClientPingDetail[j] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astClientFPSDetail != null)
			{
				for (int i = 0; i < this.astClientFPSDetail.Length; i++)
				{
					this.astClientFPSDetail[i] = (COMDT_CLIENT_PERFORMANCE_KV_INFO)ProtocolObjectPool.Get(COMDT_CLIENT_PERFORMANCE_KV_INFO.CLASS_ID);
				}
			}
			if (this.astClientPingDetail != null)
			{
				for (int j = 0; j < this.astClientPingDetail.Length; j++)
				{
					this.astClientPingDetail[j] = (COMDT_CLIENT_PERFORMANCE_KV_INFO)ProtocolObjectPool.Get(COMDT_CLIENT_PERFORMANCE_KV_INFO.CLASS_ID);
				}
			}
		}
	}
}

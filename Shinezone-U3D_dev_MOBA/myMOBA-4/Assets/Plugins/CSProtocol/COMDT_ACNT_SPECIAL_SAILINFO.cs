using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_SPECIAL_SAILINFO : ProtocolObject
	{
		public byte bSpecSaleCnt;

		public COMDT_SPECSALE[] astSpecSaleDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 92;

		public COMDT_ACNT_SPECIAL_SAILINFO()
		{
			this.astSpecSaleDetail = new COMDT_SPECSALE[20];
			for (int i = 0; i < 20; i++)
			{
				this.astSpecSaleDetail[i] = (COMDT_SPECSALE)ProtocolObjectPool.Get(COMDT_SPECSALE.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_SPECIAL_SAILINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SPECIAL_SAILINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SPECIAL_SAILINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bSpecSaleCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSpecSaleCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSpecSaleDetail.Length < (int)this.bSpecSaleCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bSpecSaleCnt; i++)
			{
				errorType = this.astSpecSaleDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_SPECIAL_SAILINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SPECIAL_SAILINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SPECIAL_SAILINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSpecSaleCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSpecSaleCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bSpecSaleCnt; i++)
			{
				errorType = this.astSpecSaleDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_SPECIAL_SAILINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bSpecSaleCnt = 0;
			if (this.astSpecSaleDetail != null)
			{
				for (int i = 0; i < this.astSpecSaleDetail.Length; i++)
				{
					if (this.astSpecSaleDetail[i] != null)
					{
						this.astSpecSaleDetail[i].Release();
						this.astSpecSaleDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSpecSaleDetail != null)
			{
				for (int i = 0; i < this.astSpecSaleDetail.Length; i++)
				{
					this.astSpecSaleDetail[i] = (COMDT_SPECSALE)ProtocolObjectPool.Get(COMDT_SPECSALE.CLASS_ID);
				}
			}
		}
	}
}

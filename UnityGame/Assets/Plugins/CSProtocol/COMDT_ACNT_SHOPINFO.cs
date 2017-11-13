using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_SHOPINFO : ProtocolObject
	{
		public byte bShopCnt;

		public COMDT_SHOP_DETAIL[] astShopDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 85;

		public COMDT_ACNT_SHOPINFO()
		{
			this.astShopDetail = new COMDT_SHOP_DETAIL[20];
			for (int i = 0; i < 20; i++)
			{
				this.astShopDetail[i] = (COMDT_SHOP_DETAIL)ProtocolObjectPool.Get(COMDT_SHOP_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_SHOPINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SHOPINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SHOPINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bShopCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bShopCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astShopDetail.Length < (int)this.bShopCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bShopCnt; i++)
			{
				errorType = this.astShopDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_SHOPINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_SHOPINFO.CURRVERSION;
			}
			if (COMDT_ACNT_SHOPINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bShopCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bShopCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bShopCnt; i++)
			{
				errorType = this.astShopDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_SHOPINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bShopCnt = 0;
			if (this.astShopDetail != null)
			{
				for (int i = 0; i < this.astShopDetail.Length; i++)
				{
					if (this.astShopDetail[i] != null)
					{
						this.astShopDetail[i].Release();
						this.astShopDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astShopDetail != null)
			{
				for (int i = 0; i < this.astShopDetail.Length; i++)
				{
					this.astShopDetail[i] = (COMDT_SHOP_DETAIL)ProtocolObjectPool.Get(COMDT_SHOP_DETAIL.CLASS_ID);
				}
			}
		}
	}
}

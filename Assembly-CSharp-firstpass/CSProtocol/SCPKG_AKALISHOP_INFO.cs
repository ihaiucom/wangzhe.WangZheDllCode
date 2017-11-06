using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_AKALISHOP_INFO : ProtocolObject
	{
		public COMDT_ACNT_AKALISHOP_INFO stAkaliShopBuy;

		public COMDT_AKALISHOP_DETAIL stAkaliShopData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 956;

		public SCPKG_AKALISHOP_INFO()
		{
			this.stAkaliShopBuy = (COMDT_ACNT_AKALISHOP_INFO)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_INFO.CLASS_ID);
			this.stAkaliShopData = (COMDT_AKALISHOP_DETAIL)ProtocolObjectPool.Get(COMDT_AKALISHOP_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_AKALISHOP_INFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_AKALISHOP_INFO.CURRVERSION;
			}
			if (SCPKG_AKALISHOP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAkaliShopBuy.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAkaliShopData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || SCPKG_AKALISHOP_INFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_AKALISHOP_INFO.CURRVERSION;
			}
			if (SCPKG_AKALISHOP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAkaliShopBuy.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAkaliShopData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_AKALISHOP_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stAkaliShopBuy != null)
			{
				this.stAkaliShopBuy.Release();
				this.stAkaliShopBuy = null;
			}
			if (this.stAkaliShopData != null)
			{
				this.stAkaliShopData.Release();
				this.stAkaliShopData = null;
			}
		}

		public override void OnUse()
		{
			this.stAkaliShopBuy = (COMDT_ACNT_AKALISHOP_INFO)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_INFO.CLASS_ID);
			this.stAkaliShopData = (COMDT_AKALISHOP_DETAIL)ProtocolObjectPool.Get(COMDT_AKALISHOP_DETAIL.CLASS_ID);
		}
	}
}

using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ITEM_PKGINFO : ProtocolObject
	{
		public COMDT_ACNT_ITEMINFO stItemInfo;

		public COMDT_ACNT_SHOPINFO stShopInfo;

		public COMDT_ACNT_SYMBOLINFO stSymbolInfo;

		public COMDT_ACNT_SPECIAL_SAILINFO stSpecialSaleInfo;

		public COMDT_ACNT_SCRET_SAILINFO stScretSaleInfo;

		public COMDT_ACNT_AKALISHOP_INFO stAkaliShopInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 151u;

		public static readonly uint VERSION_stScretSaleInfo = 55u;

		public static readonly uint VERSION_stAkaliShopInfo = 151u;

		public static readonly int CLASS_ID = 99;

		public COMDT_ITEM_PKGINFO()
		{
			this.stItemInfo = (COMDT_ACNT_ITEMINFO)ProtocolObjectPool.Get(COMDT_ACNT_ITEMINFO.CLASS_ID);
			this.stShopInfo = (COMDT_ACNT_SHOPINFO)ProtocolObjectPool.Get(COMDT_ACNT_SHOPINFO.CLASS_ID);
			this.stSymbolInfo = (COMDT_ACNT_SYMBOLINFO)ProtocolObjectPool.Get(COMDT_ACNT_SYMBOLINFO.CLASS_ID);
			this.stSpecialSaleInfo = (COMDT_ACNT_SPECIAL_SAILINFO)ProtocolObjectPool.Get(COMDT_ACNT_SPECIAL_SAILINFO.CLASS_ID);
			this.stScretSaleInfo = (COMDT_ACNT_SCRET_SAILINFO)ProtocolObjectPool.Get(COMDT_ACNT_SCRET_SAILINFO.CLASS_ID);
			this.stAkaliShopInfo = (COMDT_ACNT_AKALISHOP_INFO)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ITEM_PKGINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ITEM_PKGINFO.CURRVERSION;
			}
			if (COMDT_ITEM_PKGINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stItemInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stShopInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSpecialSaleInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ITEM_PKGINFO.VERSION_stScretSaleInfo <= cutVer)
			{
				errorType = this.stScretSaleInfo.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ITEM_PKGINFO.VERSION_stAkaliShopInfo <= cutVer)
			{
				errorType = this.stAkaliShopInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ITEM_PKGINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ITEM_PKGINFO.CURRVERSION;
			}
			if (COMDT_ITEM_PKGINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stItemInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stShopInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSpecialSaleInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ITEM_PKGINFO.VERSION_stScretSaleInfo <= cutVer)
			{
				errorType = this.stScretSaleInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stScretSaleInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ITEM_PKGINFO.VERSION_stAkaliShopInfo <= cutVer)
			{
				errorType = this.stAkaliShopInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stAkaliShopInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ITEM_PKGINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stItemInfo != null)
			{
				this.stItemInfo.Release();
				this.stItemInfo = null;
			}
			if (this.stShopInfo != null)
			{
				this.stShopInfo.Release();
				this.stShopInfo = null;
			}
			if (this.stSymbolInfo != null)
			{
				this.stSymbolInfo.Release();
				this.stSymbolInfo = null;
			}
			if (this.stSpecialSaleInfo != null)
			{
				this.stSpecialSaleInfo.Release();
				this.stSpecialSaleInfo = null;
			}
			if (this.stScretSaleInfo != null)
			{
				this.stScretSaleInfo.Release();
				this.stScretSaleInfo = null;
			}
			if (this.stAkaliShopInfo != null)
			{
				this.stAkaliShopInfo.Release();
				this.stAkaliShopInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stItemInfo = (COMDT_ACNT_ITEMINFO)ProtocolObjectPool.Get(COMDT_ACNT_ITEMINFO.CLASS_ID);
			this.stShopInfo = (COMDT_ACNT_SHOPINFO)ProtocolObjectPool.Get(COMDT_ACNT_SHOPINFO.CLASS_ID);
			this.stSymbolInfo = (COMDT_ACNT_SYMBOLINFO)ProtocolObjectPool.Get(COMDT_ACNT_SYMBOLINFO.CLASS_ID);
			this.stSpecialSaleInfo = (COMDT_ACNT_SPECIAL_SAILINFO)ProtocolObjectPool.Get(COMDT_ACNT_SPECIAL_SAILINFO.CLASS_ID);
			this.stScretSaleInfo = (COMDT_ACNT_SCRET_SAILINFO)ProtocolObjectPool.Get(COMDT_ACNT_SCRET_SAILINFO.CLASS_ID);
			this.stAkaliShopInfo = (COMDT_ACNT_AKALISHOP_INFO)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_INFO.CLASS_ID);
		}
	}
}

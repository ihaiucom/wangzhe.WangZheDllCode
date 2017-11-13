using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_ACNT_SHOPBUY_INFO : ProtocolObject
	{
		public int iGameSysTime;

		public CSDT_SHOPBUY_DRAWINFO[] astShopDrawInfo;

		public int[] LeftShopBuyCnt;

		public byte bCurCoinDrawStep;

		public uint dwOpenBoxByCouponsCnt;

		public uint dwDirectBuyItemCnt;

		public COMDT_DRAWCNT_RECORD stSymbolDrawCommon;

		public COMDT_DRAWCNT_RECORD stSymbolDrawSenior;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 71u;

		public static readonly int CLASS_ID = 723;

		public CSDT_ACNT_SHOPBUY_INFO()
		{
			this.astShopDrawInfo = new CSDT_SHOPBUY_DRAWINFO[15];
			for (int i = 0; i < 15; i++)
			{
				this.astShopDrawInfo[i] = (CSDT_SHOPBUY_DRAWINFO)ProtocolObjectPool.Get(CSDT_SHOPBUY_DRAWINFO.CLASS_ID);
			}
			this.LeftShopBuyCnt = new int[20];
			this.stSymbolDrawCommon = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
			this.stSymbolDrawSenior = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
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
			if (cutVer == 0u || CSDT_ACNT_SHOPBUY_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ACNT_SHOPBUY_INFO.CURRVERSION;
			}
			if (CSDT_ACNT_SHOPBUY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iGameSysTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astShopDrawInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = destBuf.writeInt32(this.LeftShopBuyCnt[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bCurCoinDrawStep);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwOpenBoxByCouponsCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDirectBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolDrawCommon.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolDrawSenior.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_ACNT_SHOPBUY_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ACNT_SHOPBUY_INFO.CURRVERSION;
			}
			if (CSDT_ACNT_SHOPBUY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iGameSysTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astShopDrawInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = srcBuf.readInt32(ref this.LeftShopBuyCnt[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bCurCoinDrawStep);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOpenBoxByCouponsCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDirectBuyItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolDrawCommon.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSymbolDrawSenior.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_ACNT_SHOPBUY_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iGameSysTime = 0;
			if (this.astShopDrawInfo != null)
			{
				for (int i = 0; i < this.astShopDrawInfo.Length; i++)
				{
					if (this.astShopDrawInfo[i] != null)
					{
						this.astShopDrawInfo[i].Release();
						this.astShopDrawInfo[i] = null;
					}
				}
			}
			this.bCurCoinDrawStep = 0;
			this.dwOpenBoxByCouponsCnt = 0u;
			this.dwDirectBuyItemCnt = 0u;
			if (this.stSymbolDrawCommon != null)
			{
				this.stSymbolDrawCommon.Release();
				this.stSymbolDrawCommon = null;
			}
			if (this.stSymbolDrawSenior != null)
			{
				this.stSymbolDrawSenior.Release();
				this.stSymbolDrawSenior = null;
			}
		}

		public override void OnUse()
		{
			if (this.astShopDrawInfo != null)
			{
				for (int i = 0; i < this.astShopDrawInfo.Length; i++)
				{
					this.astShopDrawInfo[i] = (CSDT_SHOPBUY_DRAWINFO)ProtocolObjectPool.Get(CSDT_SHOPBUY_DRAWINFO.CLASS_ID);
				}
			}
			this.stSymbolDrawCommon = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
			this.stSymbolDrawSenior = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
		}
	}
}

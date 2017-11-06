using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SHOPBUY_RECORD : ProtocolObject
	{
		public COMDT_DRAWCNT_RECORD[] astDrawRecord;

		public int iLimitRefreshTime;

		public int[] ShopBuyLimit;

		public COMDT_COINDRAW_INFO stCoinDrawInfo;

		public uint dwDirectBuyItemRefreshTime;

		public uint dwDirectBuyCurItemCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 71u;

		public static readonly uint VERSION_dwDirectBuyItemRefreshTime = 9u;

		public static readonly uint VERSION_dwDirectBuyCurItemCnt = 35u;

		public static readonly int CLASS_ID = 318;

		public COMDT_SHOPBUY_RECORD()
		{
			this.astDrawRecord = new COMDT_DRAWCNT_RECORD[15];
			for (int i = 0; i < 15; i++)
			{
				this.astDrawRecord[i] = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
			}
			this.ShopBuyLimit = new int[20];
			this.stCoinDrawInfo = (COMDT_COINDRAW_INFO)ProtocolObjectPool.Get(COMDT_COINDRAW_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SHOPBUY_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOPBUY_RECORD.CURRVERSION;
			}
			if (COMDT_SHOPBUY_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astDrawRecord[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeInt32(this.iLimitRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = destBuf.writeInt32(this.ShopBuyLimit[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stCoinDrawInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_SHOPBUY_RECORD.VERSION_dwDirectBuyItemRefreshTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDirectBuyItemRefreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SHOPBUY_RECORD.VERSION_dwDirectBuyCurItemCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDirectBuyCurItemCnt);
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
			if (cutVer == 0u || COMDT_SHOPBUY_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOPBUY_RECORD.CURRVERSION;
			}
			if (COMDT_SHOPBUY_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astDrawRecord[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iLimitRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = srcBuf.readInt32(ref this.ShopBuyLimit[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stCoinDrawInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_SHOPBUY_RECORD.VERSION_dwDirectBuyItemRefreshTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDirectBuyItemRefreshTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDirectBuyItemRefreshTime = 0u;
			}
			if (COMDT_SHOPBUY_RECORD.VERSION_dwDirectBuyCurItemCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDirectBuyCurItemCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDirectBuyCurItemCnt = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SHOPBUY_RECORD.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.astDrawRecord != null)
			{
				for (int i = 0; i < this.astDrawRecord.Length; i++)
				{
					if (this.astDrawRecord[i] != null)
					{
						this.astDrawRecord[i].Release();
						this.astDrawRecord[i] = null;
					}
				}
			}
			this.iLimitRefreshTime = 0;
			if (this.stCoinDrawInfo != null)
			{
				this.stCoinDrawInfo.Release();
				this.stCoinDrawInfo = null;
			}
			this.dwDirectBuyItemRefreshTime = 0u;
			this.dwDirectBuyCurItemCnt = 0u;
		}

		public override void OnUse()
		{
			if (this.astDrawRecord != null)
			{
				for (int i = 0; i < this.astDrawRecord.Length; i++)
				{
					this.astDrawRecord[i] = (COMDT_DRAWCNT_RECORD)ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
				}
			}
			this.stCoinDrawInfo = (COMDT_COINDRAW_INFO)ProtocolObjectPool.Get(COMDT_COINDRAW_INFO.CLASS_ID);
		}
	}
}

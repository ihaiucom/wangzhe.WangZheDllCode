using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_AKALISHOP_INFO : ProtocolObject
	{
		public byte bAlreadyGet;

		public uint dwBeginTime;

		public uint dwEndTime;

		public byte bBuyCnt;

		public COMDT_ACNT_AKALISHOP_BUY[] astBuyList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 96;

		public COMDT_ACNT_AKALISHOP_INFO()
		{
			this.astBuyList = new COMDT_ACNT_AKALISHOP_BUY[50];
			for (int i = 0; i < 50; i++)
			{
				this.astBuyList[i] = (COMDT_ACNT_AKALISHOP_BUY)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_BUY.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_AKALISHOP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_AKALISHOP_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_AKALISHOP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bAlreadyGet);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwBeginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bBuyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astBuyList.Length < (int)this.bBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bBuyCnt; i++)
			{
				errorType = this.astBuyList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_AKALISHOP_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_AKALISHOP_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_AKALISHOP_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bAlreadyGet);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBeginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBuyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bBuyCnt; i++)
			{
				errorType = this.astBuyList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_AKALISHOP_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bAlreadyGet = 0;
			this.dwBeginTime = 0u;
			this.dwEndTime = 0u;
			this.bBuyCnt = 0;
			if (this.astBuyList != null)
			{
				for (int i = 0; i < this.astBuyList.Length; i++)
				{
					if (this.astBuyList[i] != null)
					{
						this.astBuyList[i].Release();
						this.astBuyList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astBuyList != null)
			{
				for (int i = 0; i < this.astBuyList.Length; i++)
				{
					this.astBuyList[i] = (COMDT_ACNT_AKALISHOP_BUY)ProtocolObjectPool.Get(COMDT_ACNT_AKALISHOP_BUY.CLASS_ID);
				}
			}
		}
	}
}

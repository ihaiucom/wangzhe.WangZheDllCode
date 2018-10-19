using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_ITEM_BUYLIST : ProtocolObject
	{
		public ushort wBuyCnt;

		public CSDT_ITEM_BUYOBJ[] astBuyObj;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 920;

		public CSDT_ITEM_BUYLIST()
		{
			this.astBuyObj = new CSDT_ITEM_BUYOBJ[400];
			for (int i = 0; i < 400; i++)
			{
				this.astBuyObj[i] = (CSDT_ITEM_BUYOBJ)ProtocolObjectPool.Get(CSDT_ITEM_BUYOBJ.CLASS_ID);
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
			if (cutVer == 0u || CSDT_ITEM_BUYLIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ITEM_BUYLIST.CURRVERSION;
			}
			if (CSDT_ITEM_BUYLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wBuyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astBuyObj.Length < (int)this.wBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wBuyCnt; i++)
			{
				errorType = this.astBuyObj[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_ITEM_BUYLIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_ITEM_BUYLIST.CURRVERSION;
			}
			if (CSDT_ITEM_BUYLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wBuyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wBuyCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wBuyCnt; i++)
			{
				errorType = this.astBuyObj[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_ITEM_BUYLIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wBuyCnt = 0;
			if (this.astBuyObj != null)
			{
				for (int i = 0; i < this.astBuyObj.Length; i++)
				{
					if (this.astBuyObj[i] != null)
					{
						this.astBuyObj[i].Release();
						this.astBuyObj[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astBuyObj != null)
			{
				for (int i = 0; i < this.astBuyObj.Length; i++)
				{
					this.astBuyObj[i] = (CSDT_ITEM_BUYOBJ)ProtocolObjectPool.Get(CSDT_ITEM_BUYOBJ.CLASS_ID);
				}
			}
		}
	}
}

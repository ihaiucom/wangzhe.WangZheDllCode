using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SHOP_ITEMLIST : ProtocolObject
	{
		public byte bItemCnt;

		public COMDT_SHOP_ITEMINFO[] astShopItem;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 83;

		public COMDT_SHOP_ITEMLIST()
		{
			this.astShopItem = new COMDT_SHOP_ITEMINFO[12];
			for (int i = 0; i < 12; i++)
			{
				this.astShopItem[i] = (COMDT_SHOP_ITEMINFO)ProtocolObjectPool.Get(COMDT_SHOP_ITEMINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SHOP_ITEMLIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOP_ITEMLIST.CURRVERSION;
			}
			if (COMDT_SHOP_ITEMLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (12 < this.bItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astShopItem.Length < (int)this.bItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bItemCnt; i++)
			{
				errorType = this.astShopItem[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_SHOP_ITEMLIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOP_ITEMLIST.CURRVERSION;
			}
			if (COMDT_SHOP_ITEMLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (12 < this.bItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bItemCnt; i++)
			{
				errorType = this.astShopItem[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SHOP_ITEMLIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bItemCnt = 0;
			if (this.astShopItem != null)
			{
				for (int i = 0; i < this.astShopItem.Length; i++)
				{
					if (this.astShopItem[i] != null)
					{
						this.astShopItem[i].Release();
						this.astShopItem[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astShopItem != null)
			{
				for (int i = 0; i < this.astShopItem.Length; i++)
				{
					this.astShopItem[i] = (COMDT_SHOP_ITEMINFO)ProtocolObjectPool.Get(COMDT_SHOP_ITEMINFO.CLASS_ID);
				}
			}
		}
	}
}

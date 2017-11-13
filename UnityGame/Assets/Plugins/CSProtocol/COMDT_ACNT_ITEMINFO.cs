using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_ITEMINFO : ProtocolObject
	{
		public ushort wItemCnt;

		public COMDT_ITEM_POSINFO[] astItemList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 86;

		public COMDT_ACNT_ITEMINFO()
		{
			this.astItemList = new COMDT_ITEM_POSINFO[400];
			for (int i = 0; i < 400; i++)
			{
				this.astItemList[i] = (COMDT_ITEM_POSINFO)ProtocolObjectPool.Get(COMDT_ITEM_POSINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_ITEMINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_ITEMINFO.CURRVERSION;
			}
			if (COMDT_ACNT_ITEMINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astItemList.Length < (int)this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wItemCnt; i++)
			{
				errorType = this.astItemList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_ITEMINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_ITEMINFO.CURRVERSION;
			}
			if (COMDT_ACNT_ITEMINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wItemCnt; i++)
			{
				errorType = this.astItemList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_ITEMINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wItemCnt = 0;
			if (this.astItemList != null)
			{
				for (int i = 0; i < this.astItemList.Length; i++)
				{
					if (this.astItemList[i] != null)
					{
						this.astItemList[i].Release();
						this.astItemList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astItemList != null)
			{
				for (int i = 0; i < this.astItemList.Length; i++)
				{
					this.astItemList[i] = (COMDT_ITEM_POSINFO)ProtocolObjectPool.Get(COMDT_ITEM_POSINFO.CLASS_ID);
				}
			}
		}
	}
}

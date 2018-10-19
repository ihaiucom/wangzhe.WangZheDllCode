using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_IDIP_ADDITEM_LIST : ProtocolObject
	{
		public ushort wAddItemCnt;

		public COMDT_ITEM_SIMPINFO[] astAddItemInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 687;

		public COMDT_IDIP_ADDITEM_LIST()
		{
			this.astAddItemInfo = new COMDT_ITEM_SIMPINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astAddItemInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_IDIP_ADDITEM_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_IDIP_ADDITEM_LIST.CURRVERSION;
			}
			if (COMDT_IDIP_ADDITEM_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wAddItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.wAddItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAddItemInfo.Length < (int)this.wAddItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wAddItemCnt; i++)
			{
				errorType = this.astAddItemInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_IDIP_ADDITEM_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_IDIP_ADDITEM_LIST.CURRVERSION;
			}
			if (COMDT_IDIP_ADDITEM_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wAddItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.wAddItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wAddItemCnt; i++)
			{
				errorType = this.astAddItemInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_IDIP_ADDITEM_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wAddItemCnt = 0;
			if (this.astAddItemInfo != null)
			{
				for (int i = 0; i < this.astAddItemInfo.Length; i++)
				{
					if (this.astAddItemInfo[i] != null)
					{
						this.astAddItemInfo[i].Release();
						this.astAddItemInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astAddItemInfo != null)
			{
				for (int i = 0; i < this.astAddItemInfo.Length; i++)
				{
					this.astAddItemInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
				}
			}
		}
	}
}

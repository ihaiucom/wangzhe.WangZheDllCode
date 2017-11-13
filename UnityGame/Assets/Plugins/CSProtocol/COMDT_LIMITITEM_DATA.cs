using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_LIMITITEM_DATA : ProtocolObject
	{
		public ushort wLimitID;

		public ushort wItemCnt;

		public COMDT_ITEM_SIMPINFO[] astItemInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 395;

		public COMDT_LIMITITEM_DATA()
		{
			this.astItemInfo = new COMDT_ITEM_SIMPINFO[14];
			for (int i = 0; i < 14; i++)
			{
				this.astItemInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_LIMITITEM_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LIMITITEM_DATA.CURRVERSION;
			}
			if (COMDT_LIMITITEM_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wLimitID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (14 < this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astItemInfo.Length < (int)this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wItemCnt; i++)
			{
				errorType = this.astItemInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_LIMITITEM_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LIMITITEM_DATA.CURRVERSION;
			}
			if (COMDT_LIMITITEM_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wLimitID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (14 < this.wItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wItemCnt; i++)
			{
				errorType = this.astItemInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_LIMITITEM_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wLimitID = 0;
			this.wItemCnt = 0;
			if (this.astItemInfo != null)
			{
				for (int i = 0; i < this.astItemInfo.Length; i++)
				{
					if (this.astItemInfo[i] != null)
					{
						this.astItemInfo[i].Release();
						this.astItemInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astItemInfo != null)
			{
				for (int i = 0; i < this.astItemInfo.Length; i++)
				{
					this.astItemInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
				}
			}
		}
	}
}

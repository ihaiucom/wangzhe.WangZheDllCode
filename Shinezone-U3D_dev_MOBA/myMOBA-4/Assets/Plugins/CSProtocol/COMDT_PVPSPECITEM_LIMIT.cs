using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PVPSPECITEM_LIMIT : ProtocolObject
	{
		public uint dwLastRefreshTime;

		public byte bLimitItemCnt;

		public COMDT_PVPSPECITEM_LIMITOBJ[] astLimitItemList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 603;

		public COMDT_PVPSPECITEM_LIMIT()
		{
			this.astLimitItemList = new COMDT_PVPSPECITEM_LIMITOBJ[15];
			for (int i = 0; i < 15; i++)
			{
				this.astLimitItemList[i] = (COMDT_PVPSPECITEM_LIMITOBJ)ProtocolObjectPool.Get(COMDT_PVPSPECITEM_LIMITOBJ.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PVPSPECITEM_LIMIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPSPECITEM_LIMIT.CURRVERSION;
			}
			if (COMDT_PVPSPECITEM_LIMIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bLimitItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bLimitItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astLimitItemList.Length < (int)this.bLimitItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bLimitItemCnt; i++)
			{
				errorType = this.astLimitItemList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PVPSPECITEM_LIMIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPSPECITEM_LIMIT.CURRVERSION;
			}
			if (COMDT_PVPSPECITEM_LIMIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLimitItemCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bLimitItemCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bLimitItemCnt; i++)
			{
				errorType = this.astLimitItemList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PVPSPECITEM_LIMIT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastRefreshTime = 0u;
			this.bLimitItemCnt = 0;
			if (this.astLimitItemList != null)
			{
				for (int i = 0; i < this.astLimitItemList.Length; i++)
				{
					if (this.astLimitItemList[i] != null)
					{
						this.astLimitItemList[i].Release();
						this.astLimitItemList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astLimitItemList != null)
			{
				for (int i = 0; i < this.astLimitItemList.Length; i++)
				{
					this.astLimitItemList[i] = (COMDT_PVPSPECITEM_LIMITOBJ)ProtocolObjectPool.Get(COMDT_PVPSPECITEM_LIMITOBJ.CLASS_ID);
				}
			}
		}
	}
}

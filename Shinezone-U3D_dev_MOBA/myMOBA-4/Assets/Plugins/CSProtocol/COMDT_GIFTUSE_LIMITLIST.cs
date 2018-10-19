using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GIFTUSE_LIMITLIST : ProtocolObject
	{
		public ushort wLimitCnt;

		public COMDT_ITEM_SIMPINFO[] astLimitInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 593;

		public COMDT_GIFTUSE_LIMITLIST()
		{
			this.astLimitInfo = new COMDT_ITEM_SIMPINFO[60];
			for (int i = 0; i < 60; i++)
			{
				this.astLimitInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GIFTUSE_LIMITLIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GIFTUSE_LIMITLIST.CURRVERSION;
			}
			if (COMDT_GIFTUSE_LIMITLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wLimitCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (60 < this.wLimitCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astLimitInfo.Length < (int)this.wLimitCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wLimitCnt; i++)
			{
				errorType = this.astLimitInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_GIFTUSE_LIMITLIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GIFTUSE_LIMITLIST.CURRVERSION;
			}
			if (COMDT_GIFTUSE_LIMITLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wLimitCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (60 < this.wLimitCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wLimitCnt; i++)
			{
				errorType = this.astLimitInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GIFTUSE_LIMITLIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wLimitCnt = 0;
			if (this.astLimitInfo != null)
			{
				for (int i = 0; i < this.astLimitInfo.Length; i++)
				{
					if (this.astLimitInfo[i] != null)
					{
						this.astLimitInfo[i].Release();
						this.astLimitInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astLimitInfo != null)
			{
				for (int i = 0; i < this.astLimitInfo.Length; i++)
				{
					this.astLimitInfo[i] = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
				}
			}
		}
	}
}

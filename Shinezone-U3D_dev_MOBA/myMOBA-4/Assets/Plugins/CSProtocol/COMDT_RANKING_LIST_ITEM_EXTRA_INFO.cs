using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_INFO : ProtocolObject
	{
		public byte bNumberType;

		public COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL stDetailInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 520;

		public COMDT_RANKING_LIST_ITEM_EXTRA_INFO()
		{
			this.stDetailInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_INFO.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNumberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bNumberType;
			errorType = this.stDetailInfo.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_INFO.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNumberType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bNumberType;
			errorType = this.stDetailInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKING_LIST_ITEM_EXTRA_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bNumberType = 0;
			if (this.stDetailInfo != null)
			{
				this.stDetailInfo.Release();
				this.stDetailInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stDetailInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CLASS_ID);
		}
	}
}

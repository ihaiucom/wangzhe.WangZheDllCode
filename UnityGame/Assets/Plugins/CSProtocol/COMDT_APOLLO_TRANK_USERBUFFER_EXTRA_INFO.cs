using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO : ProtocolObject
	{
		public byte bExtraType;

		public COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_DATA stExtraData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 487;

		public COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO()
		{
			this.stExtraData = (COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_DATA)ProtocolObjectPool.Get(COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.CURRVERSION;
			}
			if (COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bExtraType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bExtraType;
			errorType = this.stExtraData.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.CURRVERSION;
			}
			if (COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bExtraType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bExtraType;
			errorType = this.stExtraData.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bExtraType = 0;
			if (this.stExtraData != null)
			{
				this.stExtraData.Release();
				this.stExtraData = null;
			}
		}

		public override void OnUse()
		{
			this.stExtraData = (COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_DATA)ProtocolObjectPool.Get(COMDT_APOLLO_TRANK_USERBUFFER_EXTRA_DATA.CLASS_ID);
		}
	}
}

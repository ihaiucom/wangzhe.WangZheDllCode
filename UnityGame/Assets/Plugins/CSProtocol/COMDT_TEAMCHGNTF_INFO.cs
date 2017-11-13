using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TEAMCHGNTF_INFO : ProtocolObject
	{
		public int iChgType;

		public COMDT_TEAMCHG_DT stChgInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 92u;

		public static readonly int CLASS_ID = 334;

		public COMDT_TEAMCHGNTF_INFO()
		{
			this.stChgInfo = (COMDT_TEAMCHG_DT)ProtocolObjectPool.Get(COMDT_TEAMCHG_DT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TEAMCHGNTF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAMCHGNTF_INFO.CURRVERSION;
			}
			if (COMDT_TEAMCHGNTF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iChgType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iChgType;
			errorType = this.stChgInfo.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TEAMCHGNTF_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAMCHGNTF_INFO.CURRVERSION;
			}
			if (COMDT_TEAMCHGNTF_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iChgType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iChgType;
			errorType = this.stChgInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TEAMCHGNTF_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iChgType = 0;
			if (this.stChgInfo != null)
			{
				this.stChgInfo.Release();
				this.stChgInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stChgInfo = (COMDT_TEAMCHG_DT)ProtocolObjectPool.Get(COMDT_TEAMCHG_DT.CLASS_ID);
		}
	}
}

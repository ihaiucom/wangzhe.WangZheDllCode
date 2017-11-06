using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP : ProtocolObject
	{
		public COMDT_GIFTUSE_LIMITCHK_INFO stChkInfo;

		public uint dwRefreshTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 696;

		public COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP()
		{
			this.stChkInfo = (COMDT_GIFTUSE_LIMITCHK_INFO)ProtocolObjectPool.Get(COMDT_GIFTUSE_LIMITCHK_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stChkInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRefreshTime);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stChkInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stChkInfo != null)
			{
				this.stChkInfo.Release();
				this.stChkInfo = null;
			}
			this.dwRefreshTime = 0u;
		}

		public override void OnUse()
		{
			this.stChkInfo = (COMDT_GIFTUSE_LIMITCHK_INFO)ProtocolObjectPool.Get(COMDT_GIFTUSE_LIMITCHK_INFO.CLASS_ID);
		}
	}
}

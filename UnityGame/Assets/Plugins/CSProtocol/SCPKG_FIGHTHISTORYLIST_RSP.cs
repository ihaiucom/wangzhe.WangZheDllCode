using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_FIGHTHISTORYLIST_RSP : ProtocolObject
	{
		public byte bErrorCode;

		public CSDT_FIGHTHISTORY_RECORD_DETAIL stRecordDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 206u;

		public static readonly int CLASS_ID = 969;

		public SCPKG_FIGHTHISTORYLIST_RSP()
		{
			this.stRecordDetail = (CSDT_FIGHTHISTORY_RECORD_DETAIL)ProtocolObjectPool.Get(CSDT_FIGHTHISTORY_RECORD_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_FIGHTHISTORYLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FIGHTHISTORYLIST_RSP.CURRVERSION;
			}
			if (SCPKG_FIGHTHISTORYLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bErrorCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bErrorCode;
			errorType = this.stRecordDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_FIGHTHISTORYLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FIGHTHISTORYLIST_RSP.CURRVERSION;
			}
			if (SCPKG_FIGHTHISTORYLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bErrorCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bErrorCode;
			errorType = this.stRecordDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_FIGHTHISTORYLIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bErrorCode = 0;
			if (this.stRecordDetail != null)
			{
				this.stRecordDetail.Release();
				this.stRecordDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stRecordDetail = (CSDT_FIGHTHISTORY_RECORD_DETAIL)ProtocolObjectPool.Get(CSDT_FIGHTHISTORY_RECORD_DETAIL.CLASS_ID);
		}
	}
}

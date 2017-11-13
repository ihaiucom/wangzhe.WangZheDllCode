using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MAILOPT_RES : ProtocolObject
	{
		public byte bOptType;

		public CSDT_MAILOPT_RESINFO stOptInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 173u;

		public static readonly int CLASS_ID = 1133;

		public SCPKG_MAILOPT_RES()
		{
			this.stOptInfo = (CSDT_MAILOPT_RESINFO)ProtocolObjectPool.Get(CSDT_MAILOPT_RESINFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_MAILOPT_RES.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MAILOPT_RES.CURRVERSION;
			}
			if (SCPKG_MAILOPT_RES.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bOptType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bOptType;
			errorType = this.stOptInfo.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_MAILOPT_RES.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MAILOPT_RES.CURRVERSION;
			}
			if (SCPKG_MAILOPT_RES.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bOptType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bOptType;
			errorType = this.stOptInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MAILOPT_RES.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bOptType = 0;
			if (this.stOptInfo != null)
			{
				this.stOptInfo.Release();
				this.stOptInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stOptInfo = (CSDT_MAILOPT_RESINFO)ProtocolObjectPool.Get(CSDT_MAILOPT_RESINFO.CLASS_ID);
		}
	}
}

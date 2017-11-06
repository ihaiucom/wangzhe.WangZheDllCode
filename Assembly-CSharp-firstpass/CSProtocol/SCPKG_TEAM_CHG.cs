using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_TEAM_CHG : ProtocolObject
	{
		public COMDT_TEAMCHGNTF_INFO stChgDt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 92u;

		public static readonly int CLASS_ID = 1199;

		public SCPKG_TEAM_CHG()
		{
			this.stChgDt = (COMDT_TEAMCHGNTF_INFO)ProtocolObjectPool.Get(COMDT_TEAMCHGNTF_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_TEAM_CHG.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_TEAM_CHG.CURRVERSION;
			}
			if (SCPKG_TEAM_CHG.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stChgDt.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_TEAM_CHG.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_TEAM_CHG.CURRVERSION;
			}
			if (SCPKG_TEAM_CHG.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stChgDt.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_TEAM_CHG.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stChgDt != null)
			{
				this.stChgDt.Release();
				this.stChgDt = null;
			}
		}

		public override void OnUse()
		{
			this.stChgDt = (COMDT_TEAMCHGNTF_INFO)ProtocolObjectPool.Get(COMDT_TEAMCHGNTF_INFO.CLASS_ID);
		}
	}
}

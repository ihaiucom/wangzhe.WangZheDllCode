using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_OPER_HERO_NTF : ProtocolObject
	{
		public byte bErrCode;

		public COMDT_PLAYERINFO stChoiceHero;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1177;

		public SCPKG_OPER_HERO_NTF()
		{
			this.stChoiceHero = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.bErrCode = 0;
			TdrError.ErrorType errorType = this.stChoiceHero.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || SCPKG_OPER_HERO_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_OPER_HERO_NTF.CURRVERSION;
			}
			if (SCPKG_OPER_HERO_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stChoiceHero.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_OPER_HERO_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_OPER_HERO_NTF.CURRVERSION;
			}
			if (SCPKG_OPER_HERO_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stChoiceHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_OPER_HERO_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bErrCode = 0;
			if (this.stChoiceHero != null)
			{
				this.stChoiceHero.Release();
				this.stChoiceHero = null;
			}
		}

		public override void OnUse()
		{
			this.stChoiceHero = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}
	}
}

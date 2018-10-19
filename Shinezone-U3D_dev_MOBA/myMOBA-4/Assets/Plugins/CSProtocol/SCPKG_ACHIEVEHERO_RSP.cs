using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_ACHIEVEHERO_RSP : ProtocolObject
	{
		public int iResult;

		public COMDT_HEROINFO stHeroInfo;

		public COMDT_HERO_SKIN stHeroSkin;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 865;

		public SCPKG_ACHIEVEHERO_RSP()
		{
			this.stHeroInfo = (COMDT_HEROINFO)ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID);
			this.stHeroSkin = (COMDT_HERO_SKIN)ProtocolObjectPool.Get(COMDT_HERO_SKIN.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.iResult = 0;
			TdrError.ErrorType errorType = this.stHeroInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroSkin.construct();
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
			if (cutVer == 0u || SCPKG_ACHIEVEHERO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACHIEVEHERO_RSP.CURRVERSION;
			}
			if (SCPKG_ACHIEVEHERO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroSkin.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_ACHIEVEHERO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACHIEVEHERO_RSP.CURRVERSION;
			}
			if (SCPKG_ACHIEVEHERO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroSkin.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_ACHIEVEHERO_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iResult = 0;
			if (this.stHeroInfo != null)
			{
				this.stHeroInfo.Release();
				this.stHeroInfo = null;
			}
			if (this.stHeroSkin != null)
			{
				this.stHeroSkin.Release();
				this.stHeroSkin = null;
			}
		}

		public override void OnUse()
		{
			this.stHeroInfo = (COMDT_HEROINFO)ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID);
			this.stHeroSkin = (COMDT_HERO_SKIN)ProtocolObjectPool.Get(COMDT_HERO_SKIN.CLASS_ID);
		}
	}
}

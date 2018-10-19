using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_FREEHERO_RSP : ProtocolObject
	{
		public COMDT_FREEHERO stFreeHero;

		public COMDT_FREEHERO_INACNT stFreeHeroSymbol;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 29u;

		public static readonly int CLASS_ID = 898;

		public SCPKG_FREEHERO_RSP()
		{
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_FREEHERO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FREEHERO_RSP.CURRVERSION;
			}
			if (SCPKG_FREEHERO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stFreeHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_FREEHERO_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FREEHERO_RSP.CURRVERSION;
			}
			if (SCPKG_FREEHERO_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stFreeHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_FREEHERO_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stFreeHero != null)
			{
				this.stFreeHero.Release();
				this.stFreeHero = null;
			}
			if (this.stFreeHeroSymbol != null)
			{
				this.stFreeHeroSymbol.Release();
				this.stFreeHeroSymbol = null;
			}
		}

		public override void OnUse()
		{
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
		}
	}
}

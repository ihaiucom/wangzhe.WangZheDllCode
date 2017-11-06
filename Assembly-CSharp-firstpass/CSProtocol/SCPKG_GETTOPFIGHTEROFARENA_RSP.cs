using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GETTOPFIGHTEROFARENA_RSP : ProtocolObject
	{
		public COMDT_ARENA_FIGHTER_INFO stTopFighter;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 67u;

		public static readonly int CLASS_ID = 1379;

		public SCPKG_GETTOPFIGHTEROFARENA_RSP()
		{
			this.stTopFighter = (COMDT_ARENA_FIGHTER_INFO)ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_INFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stTopFighter.construct();
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
			if (cutVer == 0u || SCPKG_GETTOPFIGHTEROFARENA_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GETTOPFIGHTEROFARENA_RSP.CURRVERSION;
			}
			if (SCPKG_GETTOPFIGHTEROFARENA_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stTopFighter.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GETTOPFIGHTEROFARENA_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GETTOPFIGHTEROFARENA_RSP.CURRVERSION;
			}
			if (SCPKG_GETTOPFIGHTEROFARENA_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stTopFighter.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GETTOPFIGHTEROFARENA_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stTopFighter != null)
			{
				this.stTopFighter.Release();
				this.stTopFighter = null;
			}
		}

		public override void OnUse()
		{
			this.stTopFighter = (COMDT_ARENA_FIGHTER_INFO)ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_INFO.CLASS_ID);
		}
	}
}

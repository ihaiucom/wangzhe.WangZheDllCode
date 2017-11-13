using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_PLAYERPICKINFO : ProtocolObject
	{
		public byte bIsPickOK;

		public CSDT_CAMPPLAYERINFO stPickHeroInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1272;

		public CSDT_RECONN_PLAYERPICKINFO()
		{
			this.stPickHeroInfo = (CSDT_CAMPPLAYERINFO)ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.bIsPickOK = 0;
			TdrError.ErrorType errorType = this.stPickHeroInfo.construct();
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
			if (cutVer == 0u || CSDT_RECONN_PLAYERPICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PLAYERPICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_PLAYERPICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bIsPickOK);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickHeroInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RECONN_PLAYERPICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PLAYERPICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_PLAYERPICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bIsPickOK);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickHeroInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_RECONN_PLAYERPICKINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bIsPickOK = 0;
			if (this.stPickHeroInfo != null)
			{
				this.stPickHeroInfo.Release();
				this.stPickHeroInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stPickHeroInfo = (CSDT_CAMPPLAYERINFO)ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
		}
	}
}

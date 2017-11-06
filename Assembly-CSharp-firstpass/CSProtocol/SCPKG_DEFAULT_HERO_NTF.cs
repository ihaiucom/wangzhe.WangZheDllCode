using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_DEFAULT_HERO_NTF : ProtocolObject
	{
		public byte bAcntNum;

		public COMDT_PLAYERINFO[] astDefaultHeroGrp;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1184;

		public SCPKG_DEFAULT_HERO_NTF()
		{
			this.astDefaultHeroGrp = new COMDT_PLAYERINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astDefaultHeroGrp[i] = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.bAcntNum = 0;
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astDefaultHeroGrp[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || SCPKG_DEFAULT_HERO_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_DEFAULT_HERO_NTF.CURRVERSION;
			}
			if (SCPKG_DEFAULT_HERO_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDefaultHeroGrp.Length < (int)this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bAcntNum; i++)
			{
				errorType = this.astDefaultHeroGrp[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || SCPKG_DEFAULT_HERO_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_DEFAULT_HERO_NTF.CURRVERSION;
			}
			if (SCPKG_DEFAULT_HERO_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bAcntNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bAcntNum; i++)
			{
				errorType = this.astDefaultHeroGrp[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_DEFAULT_HERO_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bAcntNum = 0;
			if (this.astDefaultHeroGrp != null)
			{
				for (int i = 0; i < this.astDefaultHeroGrp.Length; i++)
				{
					if (this.astDefaultHeroGrp[i] != null)
					{
						this.astDefaultHeroGrp[i].Release();
						this.astDefaultHeroGrp[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDefaultHeroGrp != null)
			{
				for (int i = 0; i < this.astDefaultHeroGrp.Length; i++)
				{
					this.astDefaultHeroGrp[i] = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
				}
			}
		}
	}
}

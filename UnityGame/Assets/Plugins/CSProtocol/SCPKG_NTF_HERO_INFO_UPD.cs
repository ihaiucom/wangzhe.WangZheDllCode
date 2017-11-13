using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_NTF_HERO_INFO_UPD : ProtocolObject
	{
		public uint dwHeroID;

		public int iHeroUpdNum;

		public SCDT_NTF_HERO_INFO_UPD[] astHeroUpdInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 894;

		public SCPKG_NTF_HERO_INFO_UPD()
		{
			this.astHeroUpdInfo = new SCDT_NTF_HERO_INFO_UPD[10];
			for (int i = 0; i < 10; i++)
			{
				this.astHeroUpdInfo[i] = (SCDT_NTF_HERO_INFO_UPD)ProtocolObjectPool.Get(SCDT_NTF_HERO_INFO_UPD.CLASS_ID);
			}
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
			if (cutVer == 0u || SCPKG_NTF_HERO_INFO_UPD.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_HERO_INFO_UPD.CURRVERSION;
			}
			if (SCPKG_NTF_HERO_INFO_UPD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iHeroUpdNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iHeroUpdNum)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iHeroUpdNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHeroUpdInfo.Length < this.iHeroUpdNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < this.iHeroUpdNum; i++)
			{
				errorType = this.astHeroUpdInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_NTF_HERO_INFO_UPD.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_HERO_INFO_UPD.CURRVERSION;
			}
			if (SCPKG_NTF_HERO_INFO_UPD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iHeroUpdNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iHeroUpdNum)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (10 < this.iHeroUpdNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < this.iHeroUpdNum; i++)
			{
				errorType = this.astHeroUpdInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_NTF_HERO_INFO_UPD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroID = 0u;
			this.iHeroUpdNum = 0;
			if (this.astHeroUpdInfo != null)
			{
				for (int i = 0; i < this.astHeroUpdInfo.Length; i++)
				{
					if (this.astHeroUpdInfo[i] != null)
					{
						this.astHeroUpdInfo[i].Release();
						this.astHeroUpdInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeroUpdInfo != null)
			{
				for (int i = 0; i < this.astHeroUpdInfo.Length; i++)
				{
					this.astHeroUpdInfo[i] = (SCDT_NTF_HERO_INFO_UPD)ProtocolObjectPool.Get(SCDT_NTF_HERO_INFO_UPD.CLASS_ID);
				}
			}
		}
	}
}

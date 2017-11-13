using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MULTGAME_BEGINBAN : ProtocolObject
	{
		public uint dwSelfObjID;

		public COMDT_DESKINFO stDeskInfo;

		public CSDT_CAMPINFO[] astCampInfo;

		public COMDT_FREEHERO stFreeHero;

		public COMDT_FREEHERO_INACNT stFreeHeroSymbol;

		public byte bBanPosNum;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 760;

		public SCPKG_MULTGAME_BEGINBAN()
		{
			this.stDeskInfo = (COMDT_DESKINFO)ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
			this.astCampInfo = new CSDT_CAMPINFO[2];
			for (int i = 0; i < 2; i++)
			{
				this.astCampInfo[i] = (CSDT_CAMPINFO)ProtocolObjectPool.Get(CSDT_CAMPINFO.CLASS_ID);
			}
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.dwSelfObjID = 0u;
			TdrError.ErrorType errorType = this.stDeskInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.bBanPosNum = 0;
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
			if (cutVer == 0u || SCPKG_MULTGAME_BEGINBAN.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAME_BEGINBAN.CURRVERSION;
			}
			if (SCPKG_MULTGAME_BEGINBAN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwSelfObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDeskInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bBanPosNum);
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
			if (cutVer == 0u || SCPKG_MULTGAME_BEGINBAN.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAME_BEGINBAN.CURRVERSION;
			}
			if (SCPKG_MULTGAME_BEGINBAN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwSelfObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDeskInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBanPosNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MULTGAME_BEGINBAN.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwSelfObjID = 0u;
			if (this.stDeskInfo != null)
			{
				this.stDeskInfo.Release();
				this.stDeskInfo = null;
			}
			if (this.astCampInfo != null)
			{
				for (int i = 0; i < this.astCampInfo.Length; i++)
				{
					if (this.astCampInfo[i] != null)
					{
						this.astCampInfo[i].Release();
						this.astCampInfo[i] = null;
					}
				}
			}
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
			this.bBanPosNum = 0;
		}

		public override void OnUse()
		{
			this.stDeskInfo = (COMDT_DESKINFO)ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
			if (this.astCampInfo != null)
			{
				for (int i = 0; i < this.astCampInfo.Length; i++)
				{
					this.astCampInfo[i] = (CSDT_CAMPINFO)ProtocolObjectPool.Get(CSDT_CAMPINFO.CLASS_ID);
				}
			}
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
		}
	}
}

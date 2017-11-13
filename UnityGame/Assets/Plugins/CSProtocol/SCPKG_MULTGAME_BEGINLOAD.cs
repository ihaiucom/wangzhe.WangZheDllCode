using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MULTGAME_BEGINLOAD : ProtocolObject
	{
		public uint dwDeskId;

		public uint dwDeskSeq;

		public byte bKFrapsLater;

		public uint dwKFrapsFreqMs;

		public byte bIsSlowUP;

		public byte bPreActFrap;

		public uint dwRandomSeed;

		public byte bGameType;

		public COMDT_DESKINFO stDeskInfo;

		public CSDT_CAMPINFO[] astCampInfo;

		public uint dwHaskChkFreq;

		public uint dwCltLogMask;

		public uint dwCltLogSize;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 763;

		public SCPKG_MULTGAME_BEGINLOAD()
		{
			this.stDeskInfo = (COMDT_DESKINFO)ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
			this.astCampInfo = new CSDT_CAMPINFO[2];
			for (int i = 0; i < 2; i++)
			{
				this.astCampInfo[i] = (CSDT_CAMPINFO)ProtocolObjectPool.Get(CSDT_CAMPINFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			this.dwDeskId = 0u;
			this.dwDeskSeq = 0u;
			this.bKFrapsLater = 0;
			this.dwKFrapsFreqMs = 0u;
			this.bIsSlowUP = 0;
			this.bPreActFrap = 0;
			this.dwRandomSeed = 0u;
			this.bGameType = 0;
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
			this.dwHaskChkFreq = 0u;
			this.dwCltLogMask = 0u;
			this.dwCltLogSize = 0u;
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
			if (cutVer == 0u || SCPKG_MULTGAME_BEGINLOAD.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAME_BEGINLOAD.CURRVERSION;
			}
			if (SCPKG_MULTGAME_BEGINLOAD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwDeskId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDeskSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bKFrapsLater);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwKFrapsFreqMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsSlowUP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPreActFrap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRandomSeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGameType);
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
			errorType = destBuf.writeUInt32(this.dwHaskChkFreq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCltLogMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCltLogSize);
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
			if (cutVer == 0u || SCPKG_MULTGAME_BEGINLOAD.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAME_BEGINLOAD.CURRVERSION;
			}
			if (SCPKG_MULTGAME_BEGINLOAD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwDeskId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDeskSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bKFrapsLater);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwKFrapsFreqMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsSlowUP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPreActFrap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRandomSeed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGameType);
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
			errorType = srcBuf.readUInt32(ref this.dwHaskChkFreq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCltLogMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCltLogSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MULTGAME_BEGINLOAD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwDeskId = 0u;
			this.dwDeskSeq = 0u;
			this.bKFrapsLater = 0;
			this.dwKFrapsFreqMs = 0u;
			this.bIsSlowUP = 0;
			this.bPreActFrap = 0;
			this.dwRandomSeed = 0u;
			this.bGameType = 0;
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
			this.dwHaskChkFreq = 0u;
			this.dwCltLogMask = 0u;
			this.dwCltLogSize = 0u;
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
		}
	}
}
